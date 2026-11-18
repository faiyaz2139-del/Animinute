using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

// 2026-10-26 v19: data for the 10 reports (see AnyminuteReports.cs). Read-only queries on existing tables.
public static partial class AnyminuteReports
{
    sealed class Login { public int UserId; public string Email = "", Status = "No Login", LastLogin = "", Mfa = "", Invited = ""; }
    static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o, Inv).Trim(); }
    static int I(object o) { return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o, Inv); }
    static string N(decimal v) { return v.ToString("0.00", Inv); }
    static decimal H(TimeSpan t) { return Math.Round((decimal)t.TotalHours, 2); }
    static TimeSpan P(object v) { try { return AnyminuteReportDurations.Parse(v); } catch (InvalidOperationException) { return TimeSpan.Zero; } catch (FormatException) { return TimeSpan.Zero; } catch (OverflowException) { return TimeSpan.Zero; } }
    static DataTable Q(SqlConnection c, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, null, sql, a); }
    static TimeZoneInfo zone;
    public static TimeZoneInfo Zone()
    {
        if (zone != null) return zone;
        foreach (string id in new[] { "Eastern Standard Time", "America/Toronto" })
            try { return zone = TimeZoneInfo.FindSystemTimeZoneById(id); } catch (TimeZoneNotFoundException) { } catch (InvalidTimeZoneException) { }
        return zone = TimeZoneInfo.Local;
    }
    public static DateTime Today() { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone()).Date; }
    static string Utc(object v, string fmt) { if (v == null || v == DBNull.Value) return ""; return TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(Convert.ToDateTime(v), DateTimeKind.Utc), Zone()).ToString(fmt, Inv); }
    static string Day(object v) { if (v == null || v == DBNull.Value) return ""; DateTime d; if (v is DateTime) d = (DateTime)v; else if (!DateTime.TryParse(Convert.ToString(v, Inv), Inv, DateTimeStyles.None, out d)) return S(v); return d.ToString("yyyy-MM-dd", Inv); }
    static string Clock(object v) { if (v == null || v == DBNull.Value) return ""; if (v is TimeSpan) return new DateTime(((TimeSpan)v).Ticks).ToString("HH:mm", Inv); DateTime d; return v is DateTime ? ((DateTime)v).ToString("HH:mm", Inv) : DateTime.TryParse(S(v), Inv, DateTimeStyles.None, out d) ? d.ToString("HH:mm", Inv) : S(v); }
    static DateTime Monday(DateTime d) { return d.Date.AddDays(-(((int)d.DayOfWeek + 6) % 7)); }
    static string Tier(int role, bool manages) { return role == 1 ? "Business Owner" : role == 5 ? AnyminuteEmployeeEditor.RoleDisplay("Administrator") : role == 4 ? (manages ? "Employee + Supervisor" : "Employee") : role == 12 ? "Accountant" : "Supervisor"; }
    static bool Sees(AmRptBiz b, int emp) { return b.Team == null || b.Team.Contains(emp); }

    // Everyone assigned to the business (one row per employee; several roles are joined).
    static DataTable People(SqlConnection c, AmRptBiz b)
    {
        string k = "AmRptPeople" + b.Id; var cached = HttpContext.Current.Items[k] as DataTable; if (cached != null) return cached;
        bool detail = AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteAssignmentDetail");
        var dt = Q(c, @"SELECT a.EmployeeID,MIN(a.UserTypeId) AS UserTypeId,MAX(e.FirstName) FirstName,MAX(e.MiddleName) MiddleName,MAX(e.LastName) LastName,MAX(e.EMail_primary) Email,
            MAX(COALESCE(NULLIF(e.MobilePhone,''),e.HomePhone)) Phone,MAX(e.HireDate) HireDate,MAX(e.IsActive) IsActive,MAX(e.Effective_Date) Effective,MAX(e.TerminationDate) Ended,
            MAX(CASE WHEN g.EmpId IS NULL THEN 0 ELSE 1 END) Manages," + (detail ? "MAX(d.EffectiveDate) DEff,MAX(d.EndDate) DEnd" : "NULL DEff,NULL DEnd") + @"
            FROM AssginBussiness a JOIN AddEmployee e ON e.EmployeeID=a.EmployeeID
            LEFT JOIN AssgineManager g ON g.BusinessID=a.BusinessID AND g.ManagerID=a.EmployeeID AND g.EmpId<>a.EmployeeID" +
            (detail ? " LEFT JOIN dbo.AnyminuteAssignmentDetail d ON d.BusinessID=a.BusinessID AND d.EmployeeID=a.EmployeeID" : "") + @"
            WHERE a.BusinessID=@b GROUP BY a.EmployeeID", "@b", b.Id);
        dt.Columns.Add("Name", typeof(string));
        foreach (DataRow r in dt.Rows) { string n = (S(r["FirstName"]) + " " + S(r["LastName"])).Trim(); r["Name"] = n == "" ? "Employee #" + S(r["EmployeeID"]) : n; }
        HttpContext.Current.Items[k] = dt; return dt;
    }
    static Dictionary<int, string> Names(SqlConnection c, AmRptBiz b) { var m = new Dictionary<int, string>(); foreach (DataRow r in People(c, b).Rows) m[I(r["EmployeeID"])] = S(r["Name"]); return m; }
    static string Name(Dictionary<int, string> m, int id) { string n; return id <= 0 ? "" : m.TryGetValue(id, out n) ? n : "Employee #" + id; }
    static IEnumerable<DataRow> Visible(SqlConnection c, AmRptBiz b, AmRptQuery q, bool inactiveFilter)
    {
        foreach (DataRow r in People(c, b).Rows)
        {
            int id = I(r["EmployeeID"]);
            if (!Sees(b, id) || (q.Emp > 0 && q.Emp != id)) continue;
            if (inactiveFilter && !q.Inactive && !q.Choice.ContainsKey("st") && S(r["IsActive"]).ToUpperInvariant() == "NO") continue;
            yield return r;
        }
    }
    static Dictionary<int, Login> Logins(SqlConnection c, AmRptBiz b)
    {
        bool sec = AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteLoginSecurity");
        bool last = sec && Q(c, "SELECT COL_LENGTH('dbo.AnyminuteLoginSecurity','LastLoginDate')").Rows[0][0] != DBNull.Value;
        var dt = Q(c, @"SELECT m.EmployeeID,MIN(u.UserId) UserId,MAX(u.Email) Email,MAX(CAST(ISNULL(u.Is_First_Time_Login,0) AS int)) FirstLogin," +
            (sec ? "MAX(CAST(ISNULL(s.LoginDisabled,0) AS int)) Disabled,MAX(CAST(ISNULL(s.TwoFactorEnabled,0) AS int)) Mfa,MAX(s.InvitationCancelledAtUTC) Cancelled," : "0 Disabled,NULL Mfa,NULL Cancelled,") +
            (last ? "MAX(s.LastLoginDate) LastLogin," : "NULL LastLogin,") +
            @"(SELECT MAX(x.RecordedAtUTC) FROM AnyminuteApprovalAudit x WHERE x.EmployeeID=m.EmployeeID AND x.Action IN('SendInvitation','CreateEmployee')) Invited
            FROM MappingTable m JOIN UserAcc u ON u.UserId=m.USERID" + (sec ? " LEFT JOIN dbo.AnyminuteLoginSecurity s ON s.UserId=u.UserId" : "") +
            " WHERE m.BusinessID=@b GROUP BY m.EmployeeID", "@b", b.Id);
        var map = new Dictionary<int, Login>();
        foreach (DataRow r in dt.Rows)
        {
            var l = new Login { UserId = I(r["UserId"]), Email = S(r["Email"]), LastLogin = Utc(r["LastLogin"], "yyyy-MM-dd HH:mm"), Invited = Utc(r["Invited"], "yyyy-MM-dd") };
            l.Status = I(r["Disabled"]) == 1 ? "Disabled" : r["Cancelled"] != DBNull.Value ? "Invitation Cancelled" : I(r["FirstLogin"]) == 1 ? "Invitation Pending" : "Enabled";
            l.Mfa = r["Mfa"] == DBNull.Value ? "" : I(r["Mfa"]) == 1 ? "MFA Enabled" : "Not Enrolled";
            map[I(r["EmployeeID"])] = l;
        }
        return map;
    }

    // ---------- run ----------
    public static AmRptResult Run(AmRptDef d, AmRptQuery q)
    {
        var bizs = Scope().Where(b => (q.Business == "" || q.Business == "all" || b.Id.ToString(Inv) == q.Business) && Allowed(Perm(d, b), q.Action)).ToList();
        if (bizs.Count == 0) throw new HttpException(403, "Your role does not have " + (q.Action == "view" ? "view" : q.Action.ToUpperInvariant() + " export") + " access to this report for the selected business.");
        DateTime today = Today();
        if (d.DateMode == "range")
        {
            if (!q.From.HasValue) q.From = new DateTime(today.Year, today.Month, 1);
            if (!q.To.HasValue) q.To = q.From.Value.AddMonths(1).AddDays(-1);
        }
        if (d.DateMode == "none") { q.From = null; q.To = null; }
        if (q.From.HasValue && q.To.HasValue && q.To < q.From) { var t = q.From; q.From = q.To; q.To = t; }
        if (d.DateMode == "range" && (q.To.Value - q.From.Value).TotalDays > 370) q.To = q.From.Value.AddDays(370);
        var r = new AmRptResult { Def = d, Generated = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Zone()).ToString("MMMM d, yyyy h:mm tt", Inv) };
        string period = q.From.HasValue || q.To.HasValue ? d.DateLabel + ": " + (q.From.HasValue ? q.From.Value.ToString("MMM d, yyyy", Inv) : "\u2026") + " \u2013 " + (q.To.HasValue ? q.To.Value.ToString("MMM d, yyyy", Inv) : "\u2026") : "";
        r.Company = bizs.Count == 1 ? bizs[0].Name : AnyminuteReportFiles.ActorCompany(); // v51: report brand = the business reported on
        r.Subtitle = "Business: " + (bizs.Count == 1 ? bizs[0].Name : "All authorized businesses (" + bizs.Count + ")") + (period == "" ? "" : "  \u2022  " + period) + "  \u2022  Generated: " + r.Generated + " (Toronto time)";
        using (var c = AnyminuteTimesheetRepair.Open())
            switch (d.Key)
            {
                case "time-card": case "payroll-hours": TimeRows(c, r, q, bizs, d.Key == "payroll-hours"); break;
                case "work-schedule": Schedule(c, r, q, bizs); break;
                case "approval-audit": Audit(c, r, q, bizs); break;
                case "user-access": Users(c, r, q, bizs); break;
                case "project-cost": Projects(c, r, q, bizs); break;
                case "business-assignment": Assignments(c, r, q, bizs); break;
                case "employee-master": Master(c, r, q, bizs); break;
                case "pay-rate": Rates(c, r, q, bizs); break;
                case "accountant-access": Accountants(r, q, bizs); break;
            }
        var f = new List<string>();
        if (q.Emp > 0) f.Add("Employee #" + q.Emp); if (q.Proj > 0) f.Add("Project #" + q.Proj); if (q.Inactive) f.Add("Including inactive employees");
        foreach (var ch in d.Choices) { string v; if (q.Choice.TryGetValue(ch[0], out v)) f.Add(ch[1] + ": " + v); }
        r.FilterText = String.Join("; ", f);
        return r;
    }
    // Choice filters match a column of the detail section; their options are the distinct values found before filtering.
    static AmRptSection Filter(AmRptResult r, AmRptQuery q, AmRptSection s)
    {
        foreach (var ch in r.Def.Choices)
        {
            int col = Array.IndexOf(s.Cols, ch[2]); if (col < 0) continue;
            r.Options[ch[0]] = s.Rows.Select(x => x[col] ?? "").Where(v => v != "").Distinct().OrderBy(v => v).ToList();
        }
        foreach (var ch in r.Def.Choices)
        {
            string v; int col = Array.IndexOf(s.Cols, ch[2]);
            if (col >= 0 && q.Choice.TryGetValue(ch[0], out v)) s.Rows = s.Rows.Where(x => String.Equals(x[col], v, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        return s;
    }
    static int Ix(AmRptSection s, string col) { return Array.IndexOf(s.Cols, col); }
    static decimal Dec(string v) { decimal d; return Decimal.TryParse(v, NumberStyles.Number, Inv, out d) ? d : 0m; }
    static void Kpi(AmRptResult r, string k, object v) { r.Kpis.Add(new KeyValuePair<string, string>(k, Convert.ToString(v, Inv))); }

    // Time Card Detail + Payroll Hours: TimeSheetEntry day rows (ProjectId 0); approval from AnyminuteWeek / day status 2.
    static void TimeRows(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs, bool payroll)
    {
        var s = new AmRptSection { Title = payroll ? "Approved Time Entries" : "Time Card Detail", Cols = new[] { "Business", "Employee ID", "Employee", "Work Date", "Day", "Project", "Attendance", "Start Time", "End Time", "Gross Hours", "Unpaid Break (hrs)", "Net Hours", "Regular Hours", "Overtime Hours", "Approval Status", "Approved By", "Approved On" } };
        DateTime from = q.From.Value, to = q.To.Value.AddDays(1);
        foreach (var b in bizs)
        {
            var names = Names(c, b);
            var weeks = new Dictionary<string, DataRow>();
            foreach (DataRow w in Q(c, "SELECT EmployeeID,WeekStart,State,ApprovedBy,ApprovedAtUTC FROM AnyminuteWeek WHERE BusinessID=@b AND WeekStart>=@f AND WeekStart<@t", "@b", b.Id, "@f", Monday(from), "@t", to).Rows)
                weeks[S(w["EmployeeID"]) + "|" + Convert.ToDateTime(w["WeekStart"]).ToString("yyyy-MM-dd", Inv)] = w;
            var rows = Q(c, @"SELECT t.EmpId,t.TimeSheetDate,t.ReqularStart,t.Regularend,t.Regular,t.BreakTotal,t.Total,t.Over1_5x,t.Over2x,t.Status,t.Worked,ISNULL(t.ProjectId,0) ProjectId,p.ProjectName
                FROM TimeSheetEntry t LEFT JOIN AddProject p ON p.ProjectID=t.ProjectId WHERE t.BussID=@b AND t.TimeSheetDate>=@f AND t.TimeSheetDate<@t ORDER BY t.TimeSheetDate,t.EmpId", "@b", b.Id, "@f", from, "@t", to);
            var projects = new Dictionary<string, List<string>>(); var projectIds = new Dictionary<string, HashSet<int>>();
            foreach (DataRow t in rows.Rows)
            {
                if (I(t["ProjectId"]) == 0) continue; string k = S(t["EmpId"]) + "|" + Day(t["TimeSheetDate"]);
                if (!projects.ContainsKey(k)) { projects[k] = new List<string>(); projectIds[k] = new HashSet<int>(); }
                string pn = S(t["ProjectName"]); if (pn != "" && !projects[k].Contains(pn)) projects[k].Add(pn); projectIds[k].Add(I(t["ProjectId"]));
            }
            foreach (DataRow t in rows.Rows)
            {
                int e = I(t["EmpId"]); if (I(t["ProjectId"]) != 0 || !Sees(b, e) || (q.Emp > 0 && q.Emp != e)) continue;
                DateTime date = Convert.ToDateTime(t["TimeSheetDate"]); string key = e + "|" + date.ToString("yyyy-MM-dd", Inv);
                if (q.Proj > 0 && (!projectIds.ContainsKey(key) || !projectIds[key].Contains(q.Proj))) continue;
                DataRow w; weeks.TryGetValue(e + "|" + Monday(date).ToString("yyyy-MM-dd", Inv), out w);
                string state = w == null ? "Not Submitted" : S(w["State"]); if (I(t["Status"]) == 2) state = "Approved";
                bool worked = S(t["Worked"]).Equals("Yes", StringComparison.OrdinalIgnoreCase) || (S(t["Worked"]) == "" && I(t["Status"]) != 4);
                TimeSpan gross = P(t["Regular"]), brk = P(t["BreakTotal"]), net = P(t["Total"]), ot = P(t["Over1_5x"]) + P(t["Over2x"]);
                decimal reg = Math.Max(0m, H(net) - H(ot));
                bool approvedWeek = w != null && S(w["State"]) == "Approved";
                s.Rows.Add(new[] { b.Name, e.ToString(Inv), Name(names, e), date.ToString("yyyy-MM-dd", Inv), date.ToString("dddd", Inv), projects.ContainsKey(key) ? String.Join(", ", projects[key]) : "",
                    worked ? "Worked" : "Day Off", worked ? Clock(t["ReqularStart"]) : "", worked ? Clock(t["Regularend"]) : "", N(H(gross)), N(H(brk)), N(H(net)), N(reg), N(H(ot)), state,
                    approvedWeek ? Name(names, I(w["ApprovedBy"])) : "", approvedWeek ? Utc(w["ApprovedAtUTC"], "yyyy-MM-dd HH:mm") : "" });
            }
        }
        Filter(r, q, s);
        int iB = 0, iId = 1, iE = 2, iA = Ix(s, "Attendance"), iG = Ix(s, "Gross Hours"), iBr = Ix(s, "Unpaid Break (hrs)"), iN = Ix(s, "Net Hours"), iR = Ix(s, "Regular Hours"), iO = Ix(s, "Overtime Hours"), iS = Ix(s, "Approval Status");
        var groups = s.Rows.GroupBy(x => x[iB] + "|" + x[iId]).ToList();
        if (payroll)
        {
            var sum = new AmRptSection { Title = "Payroll Summary", Cols = new[] { "Business", "Employee ID", "Employee", "Entries", "Approved Entries", "Gross Hours", "Unpaid Break", "Net Hours", "Regular Hours", "Overtime Hours", "Approval Coverage" } };
            foreach (var g in groups) { int n = g.Count(x => x[iA] == "Worked"), a = g.Count(x => x[iA] == "Worked" && x[iS] == "Approved"); var f = g.First();
                sum.Rows.Add(new[] { f[iB], f[iId], f[iE], n.ToString(Inv), a.ToString(Inv), N(g.Sum(x => Dec(x[iG]))), N(g.Sum(x => Dec(x[iBr]))), N(g.Sum(x => Dec(x[iN]))), N(g.Sum(x => Dec(x[iR]))), N(g.Sum(x => Dec(x[iO]))), n == 0 ? "" : Math.Round(100m * a / n).ToString(Inv) + "%" }); }
            var exp = new AmRptSection { Title = "Payroll Export", Cols = new[] { "Business", "Employee ID", "Employee Name", "Pay Period Start", "Pay Period End", "Regular Hours", "Overtime Hours", "Total Payable Hours" } };
            foreach (var g in groups) { var ap = g.Where(x => x[iS] == "Approved").ToList(); var f = g.First();
                exp.Rows.Add(new[] { f[iB], f[iId], f[iE], q.From.Value.ToString("yyyy-MM-dd", Inv), q.To.Value.ToString("yyyy-MM-dd", Inv), N(ap.Sum(x => Dec(x[iR]))), N(ap.Sum(x => Dec(x[iO]))), N(ap.Sum(x => Dec(x[iN]))) }); }
            r.Sections.Add(sum); r.Sections.Add(s); r.Sections.Add(exp);
            Kpi(r, "Employees", groups.Count); Kpi(r, "Net Hours", N(s.Rows.Sum(x => Dec(x[iN])))); Kpi(r, "Overtime Hours", N(s.Rows.Sum(x => Dec(x[iO])))); Kpi(r, "Approved Net Hours", N(s.Rows.Where(x => x[iS] == "Approved").Sum(x => Dec(x[iN]))));
            r.Note = "Payroll Export counts approved entries only. Overtime = recorded 1.5x + 2x overtime; Regular = Net - Overtime.";
        }
        else
        {
            var sum = new AmRptSection { Title = "Summary", Cols = new[] { "Business", "Employee ID", "Employee", "Days Worked", "Gross Hours", "Unpaid Break (hrs)", "Net Hours", "Approved Net Hours" } };
            foreach (var g in groups) { var f = g.First();
                sum.Rows.Add(new[] { f[iB], f[iId], f[iE], g.Count(x => x[iA] == "Worked").ToString(Inv), N(g.Sum(x => Dec(x[iG]))), N(g.Sum(x => Dec(x[iBr]))), N(g.Sum(x => Dec(x[iN]))), N(g.Where(x => x[iS] == "Approved").Sum(x => Dec(x[iN]))) }); }
            r.Sections.Add(s); r.Sections.Add(sum);
            Kpi(r, "Time Entries", s.Rows.Count); Kpi(r, "Gross Hours", N(s.Rows.Sum(x => Dec(x[iG])))); Kpi(r, "Net Hours", N(s.Rows.Sum(x => Dec(x[iN])))); Kpi(r, "Approved Net Hours", N(s.Rows.Where(x => x[iS] == "Approved").Sum(x => Dec(x[iN]))));
        }
    }

    static void Schedule(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Daily Schedule", Cols = new[] { "Business", "Employee ID", "Employee", "Work Date", "Day", "Day Status", "Shift Start", "Shift End", "Unpaid Break (min)", "Scheduled Hours", "Notes" } };
        if (!AnyminuteEmployeeEditor.HasTable(c, null, AnyminuteWeeklySchedule.Table)) r.Note = "Work schedules need DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql (not installed), so no schedules are listed.";
        else foreach (var b in bizs)
            {
                var names = Names(c, b);
                foreach (DataRow x in Q(c, "SELECT EmployeeID,WorkDate,ScheduleType,StartTime,EndTime,BreakMinutes,Notes FROM dbo.AnyminuteWeeklySchedule WHERE BusinessID=@b AND WorkDate BETWEEN @f AND @t ORDER BY WorkDate,EmployeeID", "@b", b.Id, "@f", q.From.Value, "@t", q.To.Value).Rows)
                {
                    int e = I(x["EmployeeID"]); if (!Sees(b, e) || (q.Emp > 0 && q.Emp != e)) continue;
                    bool work = S(x["ScheduleType"]) == "Working"; DateTime d = Convert.ToDateTime(x["WorkDate"]); decimal h = 0;
                    if (work && x["StartTime"] != DBNull.Value && x["EndTime"] != DBNull.Value) { var span = (TimeSpan)x["EndTime"] - (TimeSpan)x["StartTime"]; h = Math.Max(0m, Math.Round(((decimal)span.TotalMinutes - I(x["BreakMinutes"])) / 60m, 2)); }
                    s.Rows.Add(new[] { b.Name, e.ToString(Inv), Name(names, e), d.ToString("yyyy-MM-dd", Inv), d.ToString("dddd", Inv), work ? "Working" : "Day Off", work ? Clock(x["StartTime"]) : "", work ? Clock(x["EndTime"]) : "", work ? I(x["BreakMinutes"]).ToString(Inv) : "", N(h), S(x["Notes"]) });
                }
            }
        Filter(r, q, s); int iH = Ix(s, "Scheduled Hours"), iD = Ix(s, "Day Status");
        var emp = new AmRptSection { Title = "Employee Overview", Cols = new[] { "Business", "Employee ID", "Employee", "Working Days", "Days Off", "Scheduled Hours" } };
        foreach (var g in s.Rows.GroupBy(x => x[0] + "|" + x[1])) { var f = g.First(); emp.Rows.Add(new[] { f[0], f[1], f[2], g.Count(x => x[iD] == "Working").ToString(Inv), g.Count(x => x[iD] != "Working").ToString(Inv), N(g.Sum(x => Dec(x[iH]))) }); }
        var biz = new AmRptSection { Title = "Business Summary", Cols = new[] { "Business", "Employees", "Working Shifts", "Scheduled Hours", "Days Off" } };
        foreach (var g in s.Rows.GroupBy(x => x[0])) biz.Rows.Add(new[] { g.Key, g.Select(x => x[1]).Distinct().Count().ToString(Inv), g.Count(x => x[iD] == "Working").ToString(Inv), N(g.Sum(x => Dec(x[iH]))), g.Count(x => x[iD] != "Working").ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(emp); r.Sections.Add(biz);
        Kpi(r, "Employees", emp.Rows.Count); Kpi(r, "Scheduled Shifts", s.Rows.Count(x => x[iD] == "Working")); Kpi(r, "Scheduled Hours", N(s.Rows.Sum(x => Dec(x[iH])))); Kpi(r, "Days Off", s.Rows.Count(x => x[iD] != "Working"));
    }

    static void Audit(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Approval Event Log", Cols = new[] { "Audit Event ID", "Action Timestamp (Local)", "Business", "Employee ID", "Employee Name", "Approval Scope", "Period / Date", "Action", "Previous Status", "New Status", "Action By" } };
        var tz = Zone(); DateTime f = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(q.From.Value, DateTimeKind.Unspecified), tz), t = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(q.To.Value.AddDays(1), DateTimeKind.Unspecified), tz);
        foreach (var b in bizs)
        {
            var names = Names(c, b);
            foreach (DataRow x in Q(c, @"SELECT AuditID,ActorID,EmployeeID,TimeSheetDate,Action,BeforeValue,AfterValue,RecordedAtUTC FROM AnyminuteApprovalAudit
                WHERE BusinessID=@b AND RecordedAtUTC>=@f AND RecordedAtUTC<@t AND (Action LIKE '%Week' OR Action IN('ApproveDay','ConfirmEstimate')) ORDER BY RecordedAtUTC", "@b", b.Id, "@f", f, "@t", t).Rows)
            {
                int e = I(x["EmployeeID"]); if (!Sees(b, e) || (q.Emp > 0 && q.Emp != e)) continue;
                string a = S(x["Action"]); bool week = a.EndsWith("Week", StringComparison.Ordinal);
                string label = a == "ApproveDay" ? "Approved" : a == "ConfirmEstimate" ? "Clock-out Confirmed" : a.Replace("Week", "") == "Submit" ? "Submitted" : a.Replace("Week", "") == "Approve" ? "Approved" : a.Replace("Week", "") == "Reject" ? "Rejected" : a.Replace("Week", "") == "Reopen" ? "Reopened" : a.Replace("Week", "");
                string before = S(x["BeforeValue"]), after = S(x["AfterValue"]);
                if (a == "ApproveDay") { before = "Pending"; after = "Approved"; } else if (a == "ConfirmEstimate") { before = "Estimated"; after = "Confirmed " + (after.Length >= 16 ? after.Substring(11, 5) : after); }
                s.Rows.Add(new[] { "AUD-" + S(x["AuditID"]), Utc(x["RecordedAtUTC"], "yyyy-MM-dd HH:mm"), b.Name, e.ToString(Inv), Name(names, e), week ? "Weekly" : "Daily", week ? "Week of " + Day(x["TimeSheetDate"]) : Day(x["TimeSheetDate"]), label, before, after, Name(names, I(x["ActorID"])) });
            }
        }
        Filter(r, q, s); int iA = Ix(s, "Action");
        var sum = new AmRptSection { Title = "Audit Summary", Cols = new[] { "Business", "Submitted", "Approved", "Rejected", "Reopened", "Total Actions" } };
        foreach (var g in s.Rows.GroupBy(x => x[2])) sum.Rows.Add(new[] { g.Key, g.Count(x => x[iA] == "Submitted").ToString(Inv), g.Count(x => x[iA] == "Approved").ToString(Inv), g.Count(x => x[iA] == "Rejected").ToString(Inv), g.Count(x => x[iA] == "Reopened").ToString(Inv), g.Count().ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(sum);
        Kpi(r, "Total Actions", s.Rows.Count); Kpi(r, "Approved", s.Rows.Count(x => x[iA] == "Approved")); Kpi(r, "Rejected", s.Rows.Count(x => x[iA] == "Rejected")); Kpi(r, "Reopened", s.Rows.Count(x => x[iA] == "Reopened"));
    }

    static void Users(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "User Accounts", Cols = new[] { "User ID", "Employee ID", "Employee Name", "Login Email", "Business", "Role", "Employment Status", "Login Status", "Invited On", "Last Login (Local)", "MFA Status" } };
        foreach (var b in bizs)
        {
            var logins = Logins(c, b);
            foreach (DataRow p in Visible(c, b, q, true))
            {
                int e = I(p["EmployeeID"]); Login l; if (!logins.TryGetValue(e, out l)) l = new Login();
                s.Rows.Add(new[] { l.UserId > 0 ? l.UserId.ToString(Inv) : "", e.ToString(Inv), S(p["Name"]), l.Email != "" ? l.Email : S(p["Email"]), b.Name, Tier(I(p["UserTypeId"]), I(p["Manages"]) == 1), S(p["IsActive"]).ToUpperInvariant() == "NO" ? "Inactive" : "Active", l.Status, l.Invited, l.LastLogin, l.Mfa });
            }
        }
        Filter(r, q, s); int iL = Ix(s, "Login Status");
        var sum = new AmRptSection { Title = "Summary", Cols = new[] { "Metric", "Count" } };
        foreach (var m in new[] { "Enabled", "Invitation Pending", "Invitation Cancelled", "Disabled", "No Login" }) sum.Rows.Add(new[] { m, s.Rows.Count(x => x[iL] == m).ToString(Inv) });
        sum.Rows.Insert(0, new[] { "Users listed", s.Rows.Count.ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(sum);
        Kpi(r, "Users", s.Rows.Count); Kpi(r, "Enabled", s.Rows.Count(x => x[iL] == "Enabled")); Kpi(r, "Pending Invitations", s.Rows.Count(x => x[iL] == "Invitation Pending")); Kpi(r, "Disabled / No Login", s.Rows.Count(x => x[iL] == "Disabled" || x[iL] == "No Login"));
    }

    static void Projects(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Project Time Entries", Cols = new[] { "Business", "Project ID", "Project", "Employee ID", "Employee", "Work Date", "Hours", "Pay Rate ($/hr)", "Labour Cost", "Approval Status" } };
        DateTime from = q.From.Value, to = q.To.Value.AddDays(1); bool hidden = false;
        foreach (var b in bizs)
        {
            var names = Names(c, b);
            var rates = Q(c, "SELECT EmployeeId,Rate,RateType,ISNULL(effectivedate,StartDate) Eff FROM RateDetails WHERE EmployeeId IN(SELECT EmployeeID FROM AssginBussiness WHERE BusinessID=@b) ORDER BY ISNULL(effectivedate,StartDate) DESC,RateId DESC", "@b", b.Id).Rows.Cast<DataRow>().ToList();
            var weeks = new Dictionary<string, string>();
            foreach (DataRow w in Q(c, "SELECT EmployeeID,WeekStart,State FROM AnyminuteWeek WHERE BusinessID=@b AND WeekStart>=@f AND WeekStart<@t", "@b", b.Id, "@f", Monday(from), "@t", to).Rows) weeks[S(w["EmployeeID"]) + "|" + Day(w["WeekStart"])] = S(w["State"]);
            foreach (DataRow x in Q(c, @"SELECT t.EmpId,t.TimeSheetDate,t.Total,t.Status,t.ProjectId,p.ProjectName FROM TimeSheetEntry t JOIN AddProject p ON p.ProjectID=t.ProjectId
                WHERE t.BussID=@b AND t.ProjectId>0 AND t.TimeSheetDate>=@f AND t.TimeSheetDate<@t ORDER BY p.ProjectName,t.TimeSheetDate", "@b", b.Id, "@f", from, "@t", to).Rows)
            {
                int e = I(x["EmpId"]), pid = I(x["ProjectId"]); if (!Sees(b, e) || (q.Emp > 0 && q.Emp != e) || (q.Proj > 0 && q.Proj != pid)) continue;
                DateTime d = Convert.ToDateTime(x["TimeSheetDate"]); decimal h = H(P(x["Total"]));
                var rate = rates.FirstOrDefault(z => I(z["EmployeeId"]) == e && z["Eff"] != DBNull.Value && Convert.ToDateTime(z["Eff"]) <= d);
                bool hourly = rate != null && rate["Rate"] != DBNull.Value && (S(rate["RateType"]) == "" || S(rate["RateType"]).IndexOf("hour", StringComparison.OrdinalIgnoreCase) >= 0);
                decimal rv = hourly ? Math.Round(Convert.ToDecimal(rate["Rate"], Inv), 2) : 0m; if (!b.Costs) hidden = true;
                string st; if (!weeks.TryGetValue(e + "|" + Monday(d).ToString("yyyy-MM-dd", Inv), out st)) st = "Not Submitted"; if (I(x["Status"]) == 2) st = "Approved";
                s.Rows.Add(new[] { b.Name, pid.ToString(Inv), S(x["ProjectName"]), e.ToString(Inv), Name(names, e), d.ToString("yyyy-MM-dd", Inv), N(h), b.Costs && hourly ? N(rv) : "", b.Costs && hourly ? N(Math.Round(h * rv, 2)) : "", st });
            }
        }
        Filter(r, q, s); int iH = Ix(s, "Hours"), iC = Ix(s, "Labour Cost");
        var sum = new AmRptSection { Title = "Project Summary", Cols = new[] { "Business", "Project ID", "Project", "Employees", "Entries", "Total Hrs", "Total Labour Cost" } };
        foreach (var g in s.Rows.GroupBy(x => x[0] + "|" + x[1])) { var f = g.First(); sum.Rows.Add(new[] { f[0], f[1], f[2], g.Select(x => x[3]).Distinct().Count().ToString(Inv), g.Count().ToString(Inv), N(g.Sum(x => Dec(x[iH]))), N(g.Sum(x => Dec(x[iC]))) }); }
        var biz = new AmRptSection { Title = "Business Summary", Cols = new[] { "Business", "Projects", "Total Hrs", "Labour Cost" } };
        foreach (var g in s.Rows.GroupBy(x => x[0])) biz.Rows.Add(new[] { g.Key, g.Select(x => x[1]).Distinct().Count().ToString(Inv), N(g.Sum(x => Dec(x[iH]))), N(g.Sum(x => Dec(x[iC]))) });
        r.Sections.Add(sum); r.Sections.Add(s); r.Sections.Add(biz);
        Kpi(r, "Projects", sum.Rows.Count); Kpi(r, "Total Hours", N(s.Rows.Sum(x => Dec(x[iH])))); Kpi(r, "Labour Cost", N(s.Rows.Sum(x => Dec(x[iC])))); Kpi(r, "Employees", s.Rows.Select(x => x[0] + x[3]).Distinct().Count());
        r.Note = "Labour cost = project hours x the employee's hourly rate effective on the work date (RateDetails)." + (hidden ? " Pay rates and costs are shown to administrators only." : "") + " Non-hourly rates are not costed.";
    }

    static void Assignments(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Business Assignments", Cols = new[] { "Employee ID", "Employee Name", "Business", "Role", "Employee Role", "Supervisor Role", "Assignment Status", "Effective Start", "Effective End" } };
        foreach (var b in bizs)
            foreach (DataRow p in Visible(c, b, q, true))
            {
                int role = I(p["UserTypeId"]); bool manages = I(p["Manages"]) == 1;
                string start = Day(p["DEff"] != DBNull.Value ? p["DEff"] : p["Effective"] != DBNull.Value ? p["Effective"] : p["HireDate"]), end = Day(p["DEnd"] != DBNull.Value ? p["DEnd"] : p["Ended"]);
                if (q.From.HasValue && (start == "" || String.CompareOrdinal(start, q.From.Value.ToString("yyyy-MM-dd", Inv)) < 0)) continue;
                if (q.To.HasValue && (start == "" || String.CompareOrdinal(start, q.To.Value.ToString("yyyy-MM-dd", Inv)) > 0)) continue;
                s.Rows.Add(new[] { S(p["EmployeeID"]), S(p["Name"]), b.Name, Tier(role, manages), role == 4 ? "Yes" : "No", role != 4 || manages ? "Yes" : "No", S(p["IsActive"]).ToUpperInvariant() == "NO" ? "Inactive" : "Active", start, end });
            }
        Filter(r, q, s);
        var sum = new AmRptSection { Title = "Business Summary", Cols = new[] { "Business", "Total Assignments", "Active", "Employee Roles", "Supervisor Roles", "Dual Roles" } };
        foreach (var g in s.Rows.GroupBy(x => x[2])) sum.Rows.Add(new[] { g.Key, g.Count().ToString(Inv), g.Count(x => x[6] == "Active").ToString(Inv), g.Count(x => x[4] == "Yes").ToString(Inv), g.Count(x => x[5] == "Yes").ToString(Inv), g.Count(x => x[4] == "Yes" && x[5] == "Yes").ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(sum);
        Kpi(r, "Assignments", s.Rows.Count); Kpi(r, "Active", s.Rows.Count(x => x[6] == "Active")); Kpi(r, "Employee Roles", s.Rows.Count(x => x[4] == "Yes")); Kpi(r, "Supervisor Roles", s.Rows.Count(x => x[5] == "Yes"));
    }

    static void Master(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Employee Master", Cols = new[] { "Employee ID", "First Name", "Middle Name", "Last Name", "Full Name", "Email", "Phone", "Business", "Role", "Hire Date", "Employment Status", "Login Access" } };
        foreach (var b in bizs)
        {
            var logins = Logins(c, b);
            foreach (DataRow p in Visible(c, b, q, true))
            {
                string hire = Day(p["HireDate"]);
                if (q.From.HasValue && (hire == "" || String.CompareOrdinal(hire, q.From.Value.ToString("yyyy-MM-dd", Inv)) < 0)) continue;
                if (q.To.HasValue && (hire == "" || String.CompareOrdinal(hire, q.To.Value.ToString("yyyy-MM-dd", Inv)) > 0)) continue;
                Login l; logins.TryGetValue(I(p["EmployeeID"]), out l);
                s.Rows.Add(new[] { S(p["EmployeeID"]), S(p["FirstName"]), S(p["MiddleName"]), S(p["LastName"]), S(p["Name"]), S(p["Email"]), S(p["Phone"]), b.Name, Tier(I(p["UserTypeId"]), I(p["Manages"]) == 1), hire, S(p["IsActive"]).ToUpperInvariant() == "NO" ? "Inactive" : "Active", l == null ? "No Login" : l.Status });
            }
        }
        Filter(r, q, s);
        var sum = new AmRptSection { Title = "Business Summary", Cols = new[] { "Business", "Employees", "Active", "Inactive", "With Login" } };
        foreach (var g in s.Rows.GroupBy(x => x[7])) sum.Rows.Add(new[] { g.Key, g.Count().ToString(Inv), g.Count(x => x[10] == "Active").ToString(Inv), g.Count(x => x[10] == "Inactive").ToString(Inv), g.Count(x => x[11] != "No Login").ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(sum);
        Kpi(r, "Total Employees", s.Rows.Count); Kpi(r, "Active", s.Rows.Count(x => x[10] == "Active")); Kpi(r, "Inactive", s.Rows.Count(x => x[10] == "Inactive")); Kpi(r, "Businesses", sum.Rows.Count);
    }

    static void Rates(SqlConnection c, AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Pay Rate History", Cols = new[] { "Employee ID", "Employee Name", "Business", "Pay Type", "Rate (CAD)", "Effective From", "Effective To", "Rate Status", "Job Title", "Reason / Notes", "Changed By", "Recorded On" } };
        bool reasons = AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteRateChangeReason"); string today = Today().ToString("yyyy-MM-dd", Inv);
        foreach (var b in bizs)
        {
            var names = Names(c, b); var people = Visible(c, b, q, true).ToDictionary(p => I(p["EmployeeID"]), p => S(p["Name"]));
            var dt = Q(c, "SELECT r.RateId,r.EmployeeId,r.Rate,r.RateType,ISNULL(r.effectivedate,r.StartDate) Eff,r.EndDate,r.JobTitle," + (reasons ? "x.Reason,x.ChangedBy,x.ChangedAtUTC" : "NULL Reason,NULL ChangedBy,NULL ChangedAtUTC") +
                " FROM RateDetails r" + (reasons ? " LEFT JOIN dbo.AnyminuteRateChangeReason x ON x.RateId=r.RateId" : "") + " WHERE r.EmployeeId IN(SELECT EmployeeID FROM AssginBussiness WHERE BusinessID=@b) ORDER BY r.EmployeeId,ISNULL(r.effectivedate,r.StartDate),r.RateId", "@b", b.Id);
            foreach (var g in dt.Rows.Cast<DataRow>().GroupBy(x => I(x["EmployeeId"])))
            {
                if (!people.ContainsKey(g.Key)) continue; var list = g.ToList(); int current = -1;
                for (int i = 0; i < list.Count; i++) if (Day(list[i]["Eff"]) != "" && String.CompareOrdinal(Day(list[i]["Eff"]), today) <= 0) current = i;
                for (int i = 0; i < list.Count; i++)
                {
                    var x = list[i]; string eff = Day(x["Eff"]), end = Day(x["EndDate"]);
                    if (end == "" && i + 1 < list.Count && list[i + 1]["Eff"] != DBNull.Value) end = Convert.ToDateTime(list[i + 1]["Eff"]).AddDays(-1).ToString("yyyy-MM-dd", Inv);
                    if (q.From.HasValue && (eff == "" || String.CompareOrdinal(eff, q.From.Value.ToString("yyyy-MM-dd", Inv)) < 0)) continue;
                    if (q.To.HasValue && (eff == "" || String.CompareOrdinal(eff, q.To.Value.ToString("yyyy-MM-dd", Inv)) > 0)) continue;
                    s.Rows.Add(new[] { g.Key.ToString(Inv), people[g.Key], b.Name, S(x["RateType"]), x["Rate"] == DBNull.Value ? "" : N(Convert.ToDecimal(x["Rate"], Inv)), eff, end, i == current ? "Current" : String.CompareOrdinal(eff, today) > 0 ? "Future" : "Historical", S(x["JobTitle"]), S(x["Reason"]), x["ChangedBy"] == DBNull.Value ? "" : Name(names, I(x["ChangedBy"])), Utc(x["ChangedAtUTC"], "yyyy-MM-dd") });
                }
            }
        }
        Filter(r, q, s);
        var sum = new AmRptSection { Title = "Rate Summary", Cols = new[] { "Employee ID", "Employee", "Business", "Current Rate", "Pay Type", "Rate Records" } };
        foreach (var g in s.Rows.GroupBy(x => x[2] + "|" + x[0])) { var f = g.FirstOrDefault(x => x[7] == "Current") ?? g.Last(); sum.Rows.Add(new[] { f[0], f[1], f[2], f[7] == "Current" ? f[4] : "", f[3], g.Count().ToString(Inv) }); }
        r.Sections.Add(sum); r.Sections.Add(s);
        Kpi(r, "Employees", sum.Rows.Count); Kpi(r, "Rate Records", s.Rows.Count); Kpi(r, "Current Rates", s.Rows.Count(x => x[7] == "Current")); Kpi(r, "Future Rates", s.Rows.Count(x => x[7] == "Future"));
        if (!reasons) r.Note = "Change reasons need DB_REQUIRED/REQUIRED_01_EmployeeModule.sql.";
    }

    static void Accountants(AmRptResult r, AmRptQuery q, List<AmRptBiz> bizs)
    {
        var s = new AmRptSection { Title = "Authorized Users", Cols = new[] { "Name", "Firm Name", "Login Email", "Business(es)", "Access Type", "Access Status", "Granted / Invited", "Expires", "Permissions" } };
        var ids = new HashSet<int>(bizs.Select(b => b.Id)); List<AnyminuteAccessRow> rows;
        try { rows = AnyminuteAccessService.List(); } catch (HttpException) { rows = new List<AnyminuteAccessRow>(); } catch (InvalidOperationException) { rows = new List<AnyminuteAccessRow>(); }
        foreach (var a in rows)
        {
            if (!a.AllBusinesses && !a.BusinessIds.Any(ids.Contains)) continue;
            var perms = new List<string>(); if (a.PayRates) perms.Add("View pay rates"); if (a.Export) perms.Add("Export reports"); if (a.ApprovePayroll) perms.Add("Approve payroll"); if (a.ManageAccess) perms.Add("Manage access");
            s.Rows.Add(new[] { a.Name, String.IsNullOrWhiteSpace(a.Firm) ? "" : a.Firm, a.Email, a.BusinessText, a.Accountant && a.Payroll ? "Accountant + Payroll Administrator" : a.Payroll ? "Payroll Administrator" : "Accountant", a.Status, a.Invited.HasValue ? a.Invited.Value.ToString("yyyy-MM-dd", Inv) : "", a.Expires.HasValue ? a.Expires.Value.ToString("yyyy-MM-dd", Inv) : "", String.Join(", ", perms) });
        }
        Filter(r, q, s);
        var sum = new AmRptSection { Title = "Summary", Cols = new[] { "Metric", "Value" } };
        sum.Rows.Add(new[] { "Authorized users", s.Rows.Count.ToString(Inv) });
        foreach (var st in new[] { "Active", "Invited", "Expired", "Revoked" }) sum.Rows.Add(new[] { st, s.Rows.Count(x => x[5] == st).ToString(Inv) });
        r.Sections.Add(s); r.Sections.Add(sum);
        Kpi(r, "Authorized Users", s.Rows.Count); Kpi(r, "Active", s.Rows.Count(x => x[5] == "Active")); Kpi(r, "Pending Invitations", s.Rows.Count(x => x[5] == "Invited")); Kpi(r, "Firms", s.Rows.Select(x => x[1]).Where(x => x != "").Distinct().Count());
        if (!AnyminuteAccessService.Installed()) r.Note = "Firm names, access types and expiry need DB_REQUIRED/REQUIRED_04_AccountantAccess.sql.";
    }
}
