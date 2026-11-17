using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-11-16 v86: Subscriber (Business Administrator) Dashboard redesign - header + Business filter, 6 KPI cards, Quick Actions,
// Needs Your Attention, Weekly Hours Overview (Scheduled vs Submitted, Mon-Sun), Business Overview table and Recent Activity.
// Read-only: every number comes from existing tables (AddBusiness, AssginBussiness, AnyminuteWeek, TimeSheetEntry,
// AnyminuteWeeklySchedule, AddProject, UserAcc, AnyminuteApprovalAudit); 0 / empty states when there is no data.
public static class AnyminuteSubscriberDashboard
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static string H(object s) { return HttpUtility.HtmlEncode(Convert.ToString(s)); }
    static string A(string s) { return HttpUtility.HtmlAttributeEncode(s); }
    static string U(string s) { return VirtualPathUtility.ToAbsolute(s); }
    static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o).Trim(); }
    static bool Has(System.Data.SqlClient.SqlConnection c, string t) { return AnyminuteEmployeeEditor.HasTable(c, null, t); }
    static DataTable Q(System.Data.SqlClient.SqlConnection c, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, null, sql, a); }
    static int N(System.Data.SqlClient.SqlConnection c, string sql, params object[] a) { var t = Q(c, sql, a); return t.Rows.Count == 0 || t.Rows[0][0] == DBNull.Value ? 0 : Convert.ToInt32(t.Rows[0][0]); }
    // Subscriber accounts only (UserTypeid 1); accountants and employee/supervisor role dashboards keep their own screens.
    public static bool Applies()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null || h.Session["EmployeeId"] == null || h.Session["UserId"] == null) return false;
        return Convert.ToString(h.Session["UserTypeid"]).Trim() == "1" && !AnyminuteAccountantDashboard.Applies();
    }
    static readonly Dictionary<string, string> Icons = new Dictionary<string, string> {
        {"building","<rect x='4' y='3' width='16' height='18' rx='1'/><path d='M9 7h1M14 7h1M9 11h1M14 11h1M9 15h1M14 15h1M10 21v-3h4v3'/>"},
        {"users","<circle cx='9' cy='8' r='3.5'/><path d='M2.5 20c0-3.5 3-5.5 6.5-5.5s6.5 2 6.5 5.5'/><path d='M16 4.5a3.5 3.5 0 0 1 0 7'/><path d='M18 14.6c2.2.6 3.5 2.4 3.5 5.4'/>"},
        {"clock","<circle cx='12' cy='12' r='9'/><path d='M12 7v5l3 2'/>"},
        {"check","<circle cx='12' cy='12' r='9'/><path d='m8 12.5 3 3 5-6'/>"},
        {"calendar","<rect x='3' y='5' width='18' height='16' rx='2'/><path d='M3 10h18M8 3v4M16 3v4'/>"},
        {"folder","<path d='M3 6a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z'/>"},
        {"plus","<path d='M12 5v14M5 12h14'/>"},
        {"useradd","<circle cx='9' cy='8' r='4'/><path d='M2 21c0-4 3-6 7-6s7 2 7 6M19 8v6M16 11h6'/>"},
        {"link","<path d='M10 14a4 4 0 0 0 5.7 0l3-3a4 4 0 0 0-5.7-5.7l-1 1'/><path d='M14 10a4 4 0 0 0-5.7 0l-3 3a4 4 0 0 0 5.7 5.7l1-1'/>"},
        {"file","<path d='M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8z'/><path d='M14 3v5h5M9 13h6M9 17h6'/>"},
        {"alert","<path d='M12 3 2 20h20z'/><path d='M12 10v4M12 17h.01'/>"},
        {"mail","<rect x='3' y='5' width='18' height='14' rx='2'/><path d='m3 7 9 6 9-6'/>"},
        {"chevron","<path d='m9 6 6 6-6 6'/>"},
        {"down","<path d='m6 9 6 6 6-6'/>"},
        {"dots","<circle cx='12' cy='5' r='1.6' fill='currentColor'/><circle cx='12' cy='12' r='1.6' fill='currentColor'/><circle cx='12' cy='19' r='1.6' fill='currentColor'/>"},
        {"activity","<path d='M3 12h4l3-7 4 14 3-7h4'/>"}};
    static string I(string n) { string p; return Icons.TryGetValue(n, out p) ? "<svg class='am-sd-ico' viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='1.8' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'>" + p + "</svg>" : ""; }
    static string Ago(DateTime utc)
    {
        var d = DateTime.UtcNow - DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        if (d.TotalMinutes < 1) return "Just now"; if (d.TotalMinutes < 60) return (int)d.TotalMinutes + " min ago"; if (d.TotalHours < 24) return (int)d.TotalHours + (d.TotalHours < 2 ? " hour ago" : " hours ago");
        if (d.TotalDays < 7) return (int)d.TotalDays + ((int)d.TotalDays == 1 ? " day ago" : " days ago");
        return DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime().ToString("MMM d, yyyy", Inv);
    }
    const string Hours = "SUM(CAST(ISNULL(DATEDIFF(second,CAST('00:00' AS time),TRY_CONVERT(time,LTRIM(RTRIM(t.Total)))),0) AS bigint))";

    public static string Render(string businessArg, string weekArg, bool allActivity)
    {
        var all = AnyminuteModernScope.Businesses();
        var businesses = all.Rows.Cast<DataRow>().Select(r => new { Id = Convert.ToInt32(r["BusinessID"]), Name = S(r["CompanyName"]), Active = S(r["Status"]) == "1" }).ToList();
        int pick; if (!Int32.TryParse(businessArg, out pick) || !businesses.Any(b => b.Id == pick)) pick = 0;
        var scope = businesses.Where(b => pick == 0 || b.Id == pick).ToList();
        string ids = scope.Count == 0 ? "0" : String.Join(",", scope.Select(b => b.Id.ToString(Inv)));
        string qs = pick > 0 ? "&b=" + pick : "";
        DateTime thisWeek = AnyminuteWeekService.Monday(DateTime.Today), week;
        if (!DateTime.TryParseExact(weekArg ?? "", "yyyy-MM-dd", Inv, DateTimeStyles.None, out week) || week > thisWeek || week < thisWeek.AddDays(-7 * 11)) week = thisWeek; else week = AnyminuteWeekService.Monday(week);
        var s = new StringBuilder("<div class='am-sd'>");
        // header + business filter
        s.Append("<div class='am-sd-head'><div><h1>Dashboard</h1><p>Overview of your businesses, employees, time cards and approvals.</p></div><label class='am-sd-filter'><span class='am-sd-filter-label'>Business</span><span class='am-sd-select-wrap'>").Append(I("building"))
         .Append("<select aria-label='Business' onchange=\"location.href='").Append(A(U("~/Dashboard.aspx"))).Append("'+(this.value?'?b='+this.value:'')\"><option value=''>All Businesses</option>");
        foreach (var b in businesses) s.Append("<option value='").Append(b.Id).Append("'").Append(b.Id == pick ? " selected" : "").Append(">").Append(H(b.Name)).Append("</option>");
        s.Append("</select></span></label></div>");
        if (allActivity) { s.Append(ActivityList(ids, 100, true, qs)).Append("</div>"); return s.ToString(); }

        int activeEmployees = 0, draftCards = 0, pending = 0, scheduledToday = 0, projects = 0, missing = 0, invites = 0, noSchedule = 0;
        long schedSec = 0, subSec = 0, apprSec = 0; var schedDay = new long[7]; var subDay = new long[7];
        var overview = new List<object[]>();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool weekTable = Has(c, "AnyminuteWeek"), sched = Has(c, "AnyminuteWeeklySchedule"), detail = Has(c, "AnyminuteProjectDetail");
            // Active employees: people (not the owner role) with an active assignment in the selected businesses.
            const string ActiveEmp = "SELECT DISTINCT a.BusinessID,a.EmployeeID FROM AssginBussiness a JOIN AddEmployee e ON e.EmployeeID=a.EmployeeID WHERE a.BusinessID IN({0}) AND a.UserTypeId<>1 AND UPPER(LTRIM(RTRIM(ISNULL(e.IsActive,'YES'))))<>'NO' AND ISNULL(e.Employee_Status,'Active') NOT IN('Not Active','Terminated')";
            var active = Q(c, String.Format(ActiveEmp, ids));
            activeEmployees = active.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r["EmployeeID"])).Distinct().Count();
            if (weekTable)
            {
                pending = N(c, "SELECT COUNT(*) FROM AnyminuteWeek w WHERE w.BusinessID IN(" + ids + ") AND w.State='Submitted'");
                draftCards = N(c, "SELECT COUNT(*) FROM AnyminuteWeek w WHERE w.BusinessID IN(" + ids + ") AND w.State IN('Draft','Rejected') AND w.WeekStart<=@w AND EXISTS(SELECT 1 FROM TimeSheetEntry t WHERE t.BussID=w.BusinessID AND t.EmpId=w.EmployeeID AND ISNULL(t.ProjectId,0)=0 AND t.TimeSheetDate>=w.WeekStart AND t.TimeSheetDate<DATEADD(day,7,w.WeekStart))", "@w", thisWeek);
            }
            projects = N(c, "SELECT COUNT(*) FROM AddProject p" + (detail ? " LEFT JOIN AnyminuteProjectDetail d ON d.ProjectID=p.ProjectID" : "") + " WHERE p.BusinessID IN(" + ids + ") AND ISNULL(p.Status,'')<>'D'" + (detail ? " AND ISNULL(d.ProjectStatus,'Active')='Active'" : ""));
            if (sched)
            {
                scheduledToday = N(c, "SELECT COUNT(DISTINCT EmployeeID) FROM AnyminuteWeeklySchedule WHERE BusinessID IN(" + ids + ") AND WorkDate=@d AND ScheduleType='Working'", "@d", DateTime.Today);
                missing = N(c, "SELECT COUNT(DISTINCT CAST(s.BusinessID AS varchar(12))+':'+CAST(s.EmployeeID AS varchar(12))) FROM AnyminuteWeeklySchedule s WHERE s.BusinessID IN(" + ids + ") AND s.ScheduleType='Working' AND s.WorkDate>=@f AND s.WorkDate<@t AND NOT EXISTS(SELECT 1 FROM TimeSheetEntry t WHERE t.BussID=s.BusinessID AND t.EmpId=s.EmployeeID AND t.TimeSheetDate=s.WorkDate AND ISNULL(t.ProjectId,0)=0)", "@f", DateTime.Today.AddDays(-7), "@t", DateTime.Today);
                noSchedule = N(c, "SELECT COUNT(*) FROM (" + String.Format(ActiveEmp, ids) + ") x WHERE NOT EXISTS(SELECT 1 FROM AnyminuteWeeklySchedule s WHERE s.BusinessID=x.BusinessID AND s.EmployeeID=x.EmployeeID AND s.WorkDate>=@f AND s.WorkDate<@t)", "@f", thisWeek, "@t", thisWeek.AddDays(7));
                foreach (DataRow r in Q(c, "SELECT WorkDate,SUM(CASE WHEN EndTime>StartTime THEN DATEDIFF(second,StartTime,EndTime)-ISNULL(BreakMinutes,0)*60 ELSE 0 END) Sec FROM AnyminuteWeeklySchedule WHERE BusinessID IN(" + ids + ") AND ScheduleType='Working' AND WorkDate>=@f AND WorkDate<@t GROUP BY WorkDate", "@f", week, "@t", week.AddDays(7)).Rows)
                { int d = (int)(Convert.ToDateTime(r["WorkDate"]).Date - week).TotalDays; long v = Math.Max(0, Convert.ToInt64(r["Sec"])); if (d >= 0 && d < 7) { schedDay[d] += v; schedSec += v; } }
            }
            else noSchedule = 0;
            invites = N(c, "SELECT COUNT(DISTINCT u.UserId) FROM UserAcc u JOIN MappingTable m ON m.USERID=u.UserId WHERE m.BusinessID IN(" + ids + ") AND m.UserTypeId<>1 AND u.Is_First_Time_Login=1");
            if (weekTable)
                foreach (DataRow r in Q(c, "SELECT t.TimeSheetDate,w.State," + Hours + " Sec FROM TimeSheetEntry t JOIN AnyminuteWeek w ON w.BusinessID=t.BussID AND w.EmployeeID=t.EmpId AND w.WeekStart=@w WHERE t.BussID IN(" + ids + ") AND ISNULL(t.ProjectId,0)=0 AND t.TimeSheetDate>=@w AND t.TimeSheetDate<@t AND w.State IN('Submitted','Approved') GROUP BY t.TimeSheetDate,w.State", "@w", week, "@t", week.AddDays(7)).Rows)
                {
                    int d = (int)(Convert.ToDateTime(r["TimeSheetDate"]).Date - week).TotalDays; long v = Convert.ToInt64(r["Sec"]); if (d < 0 || d > 6) continue;
                    subDay[d] += v; subSec += v; if (S(r["State"]) == "Approved") apprSec += v;
                }
            foreach (var b in scope)
            {
                int emp = active.Rows.Cast<DataRow>().Count(r => Convert.ToInt32(r["BusinessID"]) == b.Id);
                int pend = weekTable ? N(c, "SELECT COUNT(*) FROM AnyminuteWeek WHERE BusinessID=@b AND State='Submitted'", "@b", b.Id) : 0;
                int prj = N(c, "SELECT COUNT(*) FROM AddProject WHERE BusinessID=@b AND ISNULL(Status,'')<>'D'", "@b", b.Id);
                overview.Add(new object[] { b.Id, b.Name, emp, pend, prj, b.Active });
            }
        }
        // KPI cards
        s.Append("<div class='am-sd-kpis'>");
        Kpi(s, "building", "blue", "Active Businesses", scope.Count(b => b.Active), U("~/Dashboard.aspx?view=businesses"));
        Kpi(s, "users", "green", "Active Employees", activeEmployees, U("~/Users.aspx"));
        Kpi(s, "clock", "amber", "Pending Time Cards", draftCards, U("~/TimeCards.aspx"));
        Kpi(s, "check", "purple", "Pending Approvals", pending, U("~/TimeCards.aspx?status=Submitted"));
        Kpi(s, "calendar", "teal", "Scheduled Today", scheduledToday, U("~/WeeklyEmployeeSchedule.aspx"));
        Kpi(s, "folder", "indigo", "Active Projects", projects, U("~/Projects.aspx"));
        s.Append("</div>");
        // Quick actions
        s.Append("<section class='am-sd-card'><h2>Quick Actions</h2><div class='am-sd-quick'>");
        Quick(s, "building", "Add Business", U("~/Add_Business.aspx"), false);
        Quick(s, "useradd", "Add Employee", U("~/EmployeeEditor.aspx"), true);
        Quick(s, "link", "Assign Employee", U("~/Users.aspx"), false);
        Quick(s, "file", "Review Time Cards", U("~/TimeCards.aspx"), false);
        Quick(s, "check", "Approve Timesheets", U("~/TimeCards.aspx?status=Submitted"), false);
        Quick(s, "calendar", "Work Schedule", U("~/WeeklyEmployeeSchedule.aspx"), false);
        s.Append("</div></section>");
        // Needs attention + weekly hours
        s.Append("<div class='am-sd-row'><section class='am-sd-card am-sd-attention'><h2>Needs Your Attention</h2><ul>");
        Attention(s, "clock", "amber", "Timesheets Pending Approval", "Submitted weekly time cards waiting for review", pending, U("~/TimeCards.aspx?status=Submitted"));
        Attention(s, "alert", "red", "Missing Time Entries", "Scheduled work days in the last 7 days with no time entry", missing, U("~/TimeCards.aspx"));
        Attention(s, "mail", "blue", "Pending Login Invitations", "Employees who have not signed in yet", invites, U("~/UserLoginAccess.aspx"));
        Attention(s, "calendar", "purple", "Employees Without Schedules", "Active employees with no schedule this week", noSchedule, U("~/WeeklyEmployeeSchedule.aspx"));
        s.Append("</ul></section>");
        s.Append("<section class='am-sd-card am-sd-hours'><div class='am-sd-card-head'><h2>Weekly Hours Overview</h2><select aria-label='Week' onchange=\"location.href='").Append(A(U("~/Dashboard.aspx"))).Append("?w='+this.value+'").Append(A(qs)).Append("'\">");
        for (int i = 0; i < 12; i++) { var m = thisWeek.AddDays(-7 * i); s.Append("<option value='").Append(m.ToString("yyyy-MM-dd", Inv)).Append("'").Append(m == week ? " selected" : "").Append(">").Append(H(m.ToString("MMM d", Inv) + " \u2013 " + m.AddDays(6).ToString("MMM d, yyyy", Inv))).Append("</option>"); }
        s.Append("</select></div><div class='am-sd-totals'>")
         .Append("<div><span>Scheduled Hours</span><strong>").Append(Hrs(schedSec)).Append("</strong></div>")
         .Append("<div><span>Submitted Hours</span><strong>").Append(Hrs(subSec)).Append("</strong></div>")
         .Append("<div><span>Approved Hours</span><strong>").Append(Hrs(apprSec)).Append("</strong></div></div>");
        double max = Math.Max(8, Math.Ceiling(Math.Max(schedDay.Max(), subDay.Max()) / 3600.0 / 8) * 8);
        s.Append("<div class='am-sd-chart' role='img' aria-label='Scheduled vs submitted hours by day'><div class='am-sd-chart-y'>");
        for (int i = 4; i >= 0; i--) s.Append("<span>").Append((max * i / 4).ToString("0.#", Inv)).Append("</span>");
        s.Append("</div><div class='am-sd-chart-bars'>");
        string[] days = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        for (int d = 0; d < 7; d++)
            s.Append("<div class='am-sd-day' title='").Append(A(days[d] + ": " + Hrs(schedDay[d]) + " scheduled, " + Hrs(subDay[d]) + " submitted")).Append("'><div class='am-sd-pair'><i class='am-sd-sched' style='height:").Append(Pct(schedDay[d], max)).Append("%'></i><i class='am-sd-sub' style='height:").Append(Pct(subDay[d], max)).Append("%'></i></div><span>").Append(days[d]).Append("</span></div>");
        s.Append("</div></div><div class='am-sd-legend'><span><i class='am-sd-sched'></i>Scheduled</span><span><i class='am-sd-sub'></i>Submitted</span></div></section></div>");
        // Business overview + recent activity
        s.Append("<div class='am-sd-row am-sd-row-b'><section class='am-sd-card'><div class='am-sd-card-head'><h2>Business Overview</h2><a class='am-sd-link' href='").Append(A(U("~/Dashboard.aspx?view=businesses"))).Append("'>View All</a></div><div class='am-sd-table-wrap'><table class='am-sd-table'><thead><tr><th>Business</th><th>Employees</th><th>Pending Approvals</th><th>Projects</th><th>Status</th><th>Actions</th></tr></thead><tbody>");
        foreach (var r in overview)
        {
            int id = (int)r[0]; bool on = (bool)r[5]; bool own = AnyminuteModernScope.Owns(id);
            s.Append("<tr><td><span class='am-sd-biz'><span class='am-sd-biz-icon'>").Append(I("building")).Append("</span><strong>").Append(H(r[1])).Append("</strong></span></td><td>").Append(r[2]).Append("</td><td>").Append(r[3]).Append("</td><td>").Append(r[4])
             .Append("</td><td><span class='am-sd-pill ").Append(on ? "am-sd-ok" : "am-sd-off").Append("'>").Append(on ? "Active" : "Not Active").Append("</span></td><td><details class='am-sd-menu'><summary aria-label='Actions for ").Append(A(S(r[1]))).Append("'>").Append(I("dots")).Append("</summary><div>")
             .Append("<a href='").Append(A(U("~/BusinessProfile.aspx?BusinessId=" + id))).Append("'>Business Profile</a>")
             .Append(own ? "<a href='" + A(U("~/Add_Business.aspx?BusinessIDEdit=" + id)) + "'>Edit Business</a>" : "")
             .Append("<a href='").Append(A(U("~/Users.aspx?BusinessId=" + id))).Append("'>Employees</a><a href='").Append(A(U("~/Projects.aspx?BusinessId=" + id))).Append("'>Projects</a><a href='").Append(A(U("~/TimeCards.aspx?BusinessId=" + id))).Append("'>Time Cards</a></div></details></td></tr>");
        }
        if (overview.Count == 0) s.Append("<tr><td colspan='6' class='am-sd-empty'>No businesses yet. Use Add Business to create one.</td></tr>");
        s.Append("</tbody></table></div></section>");
        s.Append(ActivityList(ids, 6, false, qs)).Append("</div></div>");
        return s.ToString();
    }
    static string Hrs(long sec) { return (sec / 3600.0).ToString("0.##", Inv) + " h"; }
    static string Pct(long sec, double max) { return Math.Min(100, sec / 3600.0 / max * 100).ToString("0.##", Inv); }
    static void Kpi(StringBuilder s, string icon, string tone, string label, int value, string href)
    { s.Append("<a class='am-sd-kpi' href='").Append(A(href)).Append("'><span class='am-sd-kpi-icon am-sd-").Append(tone).Append("'>").Append(I(icon)).Append("</span><div><span>").Append(H(label)).Append("</span><strong>").Append(value.ToString(Inv)).Append("</strong></div></a>"); }
    static void Quick(StringBuilder s, string icon, string label, string href, bool primary)
    { s.Append("<a class='am-sd-qa").Append(primary ? " is-primary" : "").Append("' href='").Append(A(href)).Append("'>").Append(I(icon)).Append("<span>").Append(H(label)).Append("</span></a>"); }
    static void Attention(StringBuilder s, string icon, string tone, string title, string hint, int count, string href)
    { s.Append("<li><a href='").Append(A(href)).Append("'><span class='am-sd-att-icon am-sd-").Append(tone).Append("'>").Append(I(icon)).Append("</span><span class='am-sd-att-text'><strong>").Append(H(title)).Append("</strong><small>").Append(H(hint)).Append("</small></span><span class='am-sd-count am-sd-").Append(count > 0 ? tone : "zero").Append("'>").Append(count).Append("</span>").Append(I("chevron")).Append("</a></li>"); }
    static readonly Dictionary<string, string> Verbs = new Dictionary<string, string> {
        {"Submit","submitted a weekly time card"},{"Approve","approved a weekly time card"},{"Reject","rejected a weekly time card"},{"SaveDraft","saved a time card draft"},
        {"CreateEmployee","added an employee"},{"EditEmployeeProfile","updated an employee profile"},{"SendInvitation","sent a login invitation"},{"ResetPassword","sent a password reset"},
        {"BusinessStatus","changed a business status"},{"SaveAssignment","updated a business assignment"},{"RemoveAssignment","removed a business assignment"},{"ChangeRate","changed a pay rate"},{"UpdateRate","updated a pay rate"}};
    static string ActivityList(string ids, int top, bool page, string qs)
    {
        var s = new StringBuilder("<section class='am-sd-card am-sd-activity'><div class='am-sd-card-head'><h2>Recent Activity</h2>");
        s.Append(page ? "<a class='am-sd-link' href='" + A(U("~/Dashboard.aspx") + (qs.Length > 0 ? "?" + qs.Substring(1) : "")) + "'>Back to Dashboard</a>" : "<a class='am-sd-link' href='" + A(U("~/Dashboard.aspx?view=activity") + qs) + "'>View All</a>");
        s.Append("</div><ul>"); int n = 0;
        using (var c = AnyminuteTimesheetRepair.Open())
            if (Has(c, "AnyminuteApprovalAudit"))
                foreach (DataRow r in Q(c, "SELECT TOP(" + top + ") a.Action,a.RecordedAtUTC,a.EmployeeID,a.ActorID,dbo.FN_DESC('AddEmployee',a.ActorID) Actor,dbo.FN_DESC('AddEmployee',a.EmployeeID) Emp,b.CompanyName FROM AnyminuteApprovalAudit a LEFT JOIN AddBusiness b ON b.BusinessID=a.BusinessID WHERE a.BusinessID IN(" + ids + ") ORDER BY a.RecordedAtUTC DESC,a.AuditID DESC").Rows)
                {
                    string action = S(r["Action"]), verb; if (!Verbs.TryGetValue(action, out verb)) verb = System.Text.RegularExpressions.Regex.Replace(action, "(?<=[a-z0-9])(?=[A-Z])", " ").ToLowerInvariant();
                    string actor = S(r["Actor"]).Length > 0 ? S(r["Actor"]) : "System", emp = S(r["Emp"]);
                    bool self = S(r["ActorID"]) == S(r["EmployeeID"]);
                    s.Append("<li><span class='am-sd-act-icon'>").Append(I(action.IndexOf("Rate", StringComparison.Ordinal) >= 0 ? "activity" : action == "Submit" || action == "Approve" || action == "Reject" || action == "SaveDraft" ? "clock" : action.IndexOf("Business", StringComparison.Ordinal) >= 0 ? "building" : "users"))
                     .Append("</span><span><strong>").Append(H(actor)).Append("</strong> ").Append(H(verb)).Append(!self && emp.Length > 0 ? " for <strong>" + H(emp) + "</strong>" : "")
                     .Append("<small>").Append(H(S(r["CompanyName"]))).Append(S(r["CompanyName"]).Length > 0 ? " &middot; " : "").Append(H(Ago(Convert.ToDateTime(r["RecordedAtUTC"])))).Append("</small></span></li>");
                    n++;
                }
        if (n == 0) s.Append("<li class='am-sd-empty'>No recent activity.</li>");
        return s.Append("</ul></section>").ToString();
    }
}
