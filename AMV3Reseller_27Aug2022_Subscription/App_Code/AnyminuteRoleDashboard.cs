using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-26 v18 (Pandey deck, slides 1-3): Employee / Supervisor dashboards and role navigation.
// Owners, administrators and accountants keep the existing dashboard and navigation unchanged.
// "Supervisor" = a business membership whose role is not Employee (4) (Manager 2, Team Leader 13, ...), or any
// membership that manages employees in AssgineManager. A supervisor is also an "employee" of that business when
// the membership is Employee (4) or the person has their own time cards, time entries, schedule or project assignment there.
// Every number comes from existing tables; nothing is invented. Destination pages still authorize every request.
public sealed class AnyminuteBusinessRole
{
    public int BusinessID; public string Name; public int Actor; public int Role;
    public bool Employee; public bool Supervisor; public bool Legacy;
    public string Label { get { return Employee && Supervisor ? "Employee + Supervisor" : Supervisor ? "Supervisor Only" : "Employee"; } }
}
public sealed class AnyminuteDashDay { public DateTime Date; public string Business; public bool Working; public TimeSpan Start, End; public decimal Hours; }
public sealed class AnyminuteDashCard { public int BusinessID, EmployeeID; public string Business, Employee, State; public DateTime Week; public DateTime? Submitted; public decimal Hours; }
public sealed class AnyminuteDashShift { public int BusinessID, EmployeeID; public string Business, Employee, Notes, Project; public DateTime Date; public TimeSpan Start, End; public decimal Hours; }
public sealed class AnyminuteDashModel
{
    public string Kind = "employee";   // employee | supervisor (the view rendered)
    public List<AnyminuteBusinessRole> Roles = new List<AnyminuteBusinessRole>();
    public string Selected = "";       // BusinessID or "all"
    public bool CanSwitch;             // both views are possible for the selection
    public bool DualRole;              // v20: Employee somewhere and Supervisor somewhere (Switch Dashboard is offered)
    public DateTime Monday;
    // employee view
    public decimal Hours, Scheduled; public int Shifts, Pending, Projects;
    public List<AnyminuteDashDay> Days = new List<AnyminuteDashDay>();
    public List<AnyminuteDashCard> Recent = new List<AnyminuteDashCard>();
    // supervisor view
    public int Approvals, Team, ActiveSchedules, ActiveProjects;
    public decimal TeamScheduled, TeamSubmitted, TeamApproved, TeamPending;
    public List<AnyminuteDashCard> Queue = new List<AnyminuteDashCard>();
    public List<AnyminuteDashShift> Upcoming = new List<AnyminuteDashShift>();
    public string ReportHref; public bool Directory;
    public bool All { get { return Selected == "all"; } }
    public IEnumerable<AnyminuteBusinessRole> Scope { get { return All ? Roles : Roles.Where(r => Convert.ToString(r.BusinessID) == Selected); } }
}

public static class AnyminuteRoleDashboard
{
    const string ItemKey = "AnyminuteRoleDashboard.Roles", SessionBusiness = "AmDashBusiness", SessionMode = "AmDashMode";
    static readonly int[] LegacyRoles = { 1, 5, 12 };

    // Role per accessible business, cached for the request.
    public static List<AnyminuteBusinessRole> Roles()
    {
        var h = HttpContext.Current;
        var cached = h.Items[ItemKey] as List<AnyminuteBusinessRole>;
        if (cached != null) return cached;
        var list = new List<AnyminuteBusinessRole>();
        var typeNames = new Dictionary<int, string>();
        int sessionRole; Int32.TryParse(Convert.ToString(h.Session["UserTypeid"]), out sessionRole);
        using (var c = AnyminuteTimesheetRepair.Open())
            foreach (DataRow b in AnyminuteModernScope.Businesses().Rows)
            {
                int business = Convert.ToInt32(b["BusinessID"]), role;
                int actor;
                try { actor = AnyminuteModernScope.Actor(c, null, business, out role); }
                catch (InvalidOperationException) { continue; }
                var r = new AnyminuteBusinessRole { BusinessID = business, Name = Convert.ToString(b["CompanyName"]), Actor = actor, Role = role };
                if (LegacyRoles.Contains(role)) { r.Legacy = true; list.Add(r); continue; }
                bool manages = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP(1) 1 FROM AssgineManager WHERE BusinessID=@b AND ManagerID=@a AND EmpId<>@a", "@b", business, "@a", actor).Rows.Count > 0;
                // v20 (Pandey #1): a membership is Supervisor only when it really supervises - it manages employees in
                // AssgineManager, or its role is Manager (2) / Team Leader (13) / a custom role named like a supervisor.
                // Every other role (Employee 4, custom staff roles, unset) is an Employee membership, so a pure employee in
                // several businesses no longer gets Switch Dashboard, Roles by Business or the Employees link.
                r.Supervisor = manages || SupervisorRole(c, role, typeNames);
                r.Employee = role == 4 || !r.Supervisor || OwnWork(c, business, actor);
                list.Add(r);
            }
        if (sessionRole == 1 || sessionRole == 5 || sessionRole == 12) foreach (var r in list) r.Legacy = true;
        h.Items[ItemKey] = list;
        return list;
    }
    static bool SupervisorRole(SqlConnection c, int role, Dictionary<int, string> names)
    {
        if (role == 2 || role == 13) return true;
        if (role == 4 || role <= 0) return false;
        string n;
        if (!names.TryGetValue(role, out n))
        {
            var dt = AnyminuteTimesheetRepair.Query(c, null, "SELECT UserType FROM UserType WHERE User_TypeId=@r", "@r", role);
            n = dt.Rows.Count == 1 ? Convert.ToString(dt.Rows[0][0]).Trim().ToLowerInvariant() : ""; names[role] = n;
        }
        return n.Contains("manag") || n.Contains("supervis") || n.Contains("lead") || n.Contains("foreman") || n.Contains("scheduler");
    }
    static bool OwnWork(SqlConnection c, int b, int a)
    {
        string sql = @"SELECT CASE WHEN EXISTS(SELECT 1 FROM AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@a)
            OR EXISTS(SELECT 1 FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@a)
            OR EXISTS(SELECT 1 FROM ASSIGNPROJECT WHERE BUSINESSID=@b AND EMPLOYEEID=@a)" +
            (AnyminuteEmployeeEditor.HasTable(c, null, AnyminuteWeeklySchedule.Table) ? " OR EXISTS(SELECT 1 FROM dbo.AnyminuteWeeklySchedule WHERE BusinessID=@b AND EmployeeID=@a)" : "") +
            " THEN 1 ELSE 0 END";
        return Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, sql, "@b", b, "@a", a).Rows[0][0]) == 1;
    }

    // True when this user gets the deck's employee/supervisor experience (no owner/admin/accountant membership).
    public static bool Applies()
    {
        var h = HttpContext.Current;
        if (h == null || h.Session == null || h.Session["UserId"] == null || h.Session["EmployeeId"] == null) return false;
        int sessionRole; Int32.TryParse(Convert.ToString(h.Session["UserTypeid"]), out sessionRole);
        if (LegacyRoles.Contains(sessionRole)) return false; // owners/admins/accountants: unchanged dashboard + menu, no extra queries
        try { var roles = Roles(); return roles.Count > 0 && !roles.Any(r => r.Legacy); }
        catch (HttpException) { return false; }
    }

    // Reads ?b= / ?mode= (falls back to the session) and keeps both in the session so the navigation follows.
    public static void Select(AnyminuteDashModel m, string qBusiness, string qMode)
    {
        var s = HttpContext.Current.Session;
        string sel = qBusiness ?? Convert.ToString(s[SessionBusiness]);
        if (sel != "all" && !m.Roles.Any(r => Convert.ToString(r.BusinessID) == sel)) sel = null;
        if (sel == "all" && m.Roles.Count < 2) sel = null;
        if (sel == null) sel = m.Roles.Count == 1 ? Convert.ToString(m.Roles[0].BusinessID) : (m.Roles.Any(r => r.Employee) ? "all" : Convert.ToString(m.Roles[0].BusinessID));
        m.Selected = sel;
        bool emp = m.Scope.Any(r => r.Employee), sup = m.Scope.Any(r => r.Supervisor);
        string chosen = qMode ?? Convert.ToString(s[SessionMode]);
        if (chosen != "supervisor") chosen = "employee";
        string mode = chosen;
        if (!emp) mode = "supervisor"; if (!sup) mode = "employee";
        m.Kind = mode; m.CanSwitch = emp && sup;
        m.DualRole = m.Roles.Any(r => r.Employee) && m.Roles.Any(r => r.Supervisor);
        // v20 (Pandey #2): remember the dashboard the user picked, not the one forced by a supervisor-only (or
        // employee-only) business, so going back to an Employee + Supervisor business restores their choice.
        s[SessionBusiness] = sel; s[SessionMode] = chosen;
    }

    public static string CurrentMode()
    {
        var s = HttpContext.Current.Session; var roles = Roles();
        string sel = Convert.ToString(s[SessionBusiness]), mode = Convert.ToString(s[SessionMode]);
        var scope = roles.Where(r => sel == "all" || sel == "" || Convert.ToString(r.BusinessID) == sel).ToList();
        if (scope.Count == 0) scope = roles;
        bool emp = scope.Any(r => r.Employee), sup = scope.Any(r => r.Supervisor);
        if (!emp) return "supervisor"; if (!sup) return "employee";
        return mode == "supervisor" ? "supervisor" : "employee";
    }

    // v20 (images 1/3/4): header line under "Welcome! name" - "Employee | Multiple Businesses",
    // "Employee & Supervisor | M.A. Consulting Corp.", "Supervisor | Food Services Inc.". Null for owners/admins.
    public static string HeaderContext()
    {
        try
        {
            if (!Applies()) return null;
            var roles = Roles(); if (roles.Count == 0) return null;
            string sel = Convert.ToString(HttpContext.Current.Session[SessionBusiness]);
            var picked = roles.FirstOrDefault(r => Convert.ToString(r.BusinessID) == sel);
            if (roles.Count == 1) picked = roles[0];
            var scope = picked != null ? new List<AnyminuteBusinessRole> { picked } : roles;
            bool emp = scope.Any(r => r.Employee), sup = scope.Any(r => r.Supervisor);
            string role = emp && sup ? "Employee & Supervisor" : sup ? "Supervisor" : "Employee";
            return role + " | " + (picked != null ? picked.Name : "Multiple Businesses");
        }
        catch (HttpException) { return null; }
        catch (InvalidOperationException) { return null; }
    }

    // Slide 1 / 3 left navigation. Labels follow the deck; every target is an existing page.
    public static List<KeyValuePair<string, string>> NavLinks(IEnumerable<MenuViewModel> permitted)
    {
        var roles = Roles(); string mode = CurrentMode(); var links = new List<KeyValuePair<string, string>>();
        Func<string, string> u = VirtualPathUtility.ToAbsolute;
        string report = AnyminuteReports.HubHref(permitted); // v19: Reports hub
        bool directory = AnyminuteDirectoryScope.CanReadDirectory();
        links.Add(new KeyValuePair<string, string>("Home", u("~/Dashboard.aspx")));
        if (mode == "supervisor")
        {
            links.Add(new KeyValuePair<string, string>("Team Time Cards", u("~/TimeCards.aspx?status=Submitted")));
            links.Add(new KeyValuePair<string, string>("Work Schedules", u("~/WeeklyEmployeeSchedule.aspx")));
            if (directory) links.Add(new KeyValuePair<string, string>("Employees", u("~/Users.aspx")));
            links.Add(new KeyValuePair<string, string>("Projects", u("~/Projects.aspx")));
            if (report != null) links.Add(new KeyValuePair<string, string>("Reports", report));
            links.Add(new KeyValuePair<string, string>("My Businesses", u("~/Dashboard.aspx?view=businesses")));
        }
        else
        {
            links.Add(new KeyValuePair<string, string>("My Time Card", u("~/WeekTimeCard.aspx")));
            links.Add(new KeyValuePair<string, string>("My Schedule", u("~/WeeklyEmployeeSchedule.aspx")));
            links.Add(new KeyValuePair<string, string>("My Projects", u("~/Projects.aspx")));
            // Slide 2: an Employee + Supervisor keeps Employees; slide 1 (employee only): no Employees link.
            if (directory && roles.Any(r => r.Supervisor)) links.Add(new KeyValuePair<string, string>("Employees", u("~/Users.aspx")));
            if (report != null) links.Add(new KeyValuePair<string, string>("Reports", report));
            links.Add(new KeyValuePair<string, string>("My Businesses", u("~/Dashboard.aspx?view=businesses")));
        }
        return links;
    }

    static TimeSpan Sum(SqlConnection c, int b, int e, DateTime monday)
    {
        TimeSpan t = TimeSpan.Zero;
        foreach (DataRow row in AnyminuteTimesheetRepair.Query(c, null, "SELECT Total FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0", "@b", b, "@e", e, "@w", monday).Rows)
        { try { t += AnyminuteReportDurations.Parse(row[0]); } catch (InvalidOperationException) { } }
        return t;
    }
    static decimal H(TimeSpan t) { return Math.Round((decimal)t.TotalHours, 2); }
    static decimal Hrs(TimeSpan s, TimeSpan e, int brk) { if (e <= s) return 0m; double m = (e - s).TotalMinutes - brk; return m <= 0 ? 0m : Math.Round((decimal)m / 60m, 2); }

    public static AnyminuteDashModel Load(string qBusiness, string qMode)
    {
        var m = new AnyminuteDashModel { Roles = Roles(), Monday = AnyminuteWeekService.Monday(DateTime.Today) };
        Select(m, qBusiness, qMode);
        m.ReportHref = AnyminuteReports.HubHref(AnyminuteUiNavigation.Load()); m.Directory = AnyminuteDirectoryScope.CanReadDirectory();
        DateTime today = DateTime.Today, sunday = m.Monday.AddDays(6);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool sched = AnyminuteEmployeeEditor.HasTable(c, null, AnyminuteWeeklySchedule.Table);
            if (m.Kind == "employee")
                foreach (var r in m.Scope.Where(x => x.Employee))
                {
                    m.Hours += H(Sum(c, r.BusinessID, r.Actor, m.Monday));
                    if (sched)
                        foreach (DataRow d in AnyminuteTimesheetRepair.Query(c, null, "SELECT WorkDate,ScheduleType,StartTime,EndTime,BreakMinutes FROM dbo.AnyminuteWeeklySchedule WHERE BusinessID=@b AND EmployeeID=@e AND WorkDate BETWEEN @f AND @t ORDER BY WorkDate", "@b", r.BusinessID, "@e", r.Actor, "@f", m.Monday, "@t", sunday).Rows)
                        {
                            var day = new AnyminuteDashDay { Date = Convert.ToDateTime(d["WorkDate"]).Date, Business = r.Name, Working = Convert.ToString(d["ScheduleType"]) == "Working" };
                            if (day.Working && d["StartTime"] != DBNull.Value && d["EndTime"] != DBNull.Value) { day.Start = (TimeSpan)d["StartTime"]; day.End = (TimeSpan)d["EndTime"]; day.Hours = Hrs(day.Start, day.End, Convert.ToInt32(d["BreakMinutes"])); }
                            m.Days.Add(day); m.Scheduled += day.Hours; if (day.Working && day.Date >= today) m.Shifts++;
                        }
                    m.Pending += Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(*) FROM AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@e AND State='Submitted'", "@b", r.BusinessID, "@e", r.Actor).Rows[0][0]);
                    m.Projects += Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(DISTINCT p.ProjectID) FROM ASSIGNPROJECT x JOIN AddProject p ON p.ProjectID=x.PROJECTID WHERE x.BUSINESSID=@b AND x.EMPLOYEEID=@e AND ISNULL(p.Status,'')<>'D'", "@b", r.BusinessID, "@e", r.Actor).Rows[0][0]);
                    foreach (DataRow w in AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP(5) WeekStart,State,SubmittedAtUTC FROM AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@e ORDER BY WeekStart DESC", "@b", r.BusinessID, "@e", r.Actor).Rows)
                        m.Recent.Add(Card(c, r, r.Actor, "", w));
                }
            else
                foreach (var r in m.Scope.Where(x => x.Supervisor))
                {
                    var team = AnyminuteModernScope.Users(r.BusinessID).Rows.Cast<DataRow>().Where(u => Convert.ToInt32(u["EmployeeID"]) != r.Actor)
                        .GroupBy(u => Convert.ToInt32(u["EmployeeID"])).ToDictionary(g => g.Key, g => Convert.ToString(g.First()["EmpName"]));
                    m.Team += team.Count; m.ActiveProjects += AnyminuteModernScope.Projects(r.BusinessID).Rows.Count;
                    if (team.Count == 0) continue;
                    string ids = String.Join(",", team.Keys.Select(k => k.ToString(CultureInfo.InvariantCulture)));
                    // Approval queue: same rule as TimeCards.aspx?status=Submitted (Authorize, never your own card).
                    foreach (DataRow w in AnyminuteTimesheetRepair.Query(c, null, "SELECT EmployeeID,WeekStart,State,SubmittedAtUTC FROM AnyminuteWeek WHERE BusinessID=@b AND State='Submitted' AND EmployeeID IN (" + ids + ") ORDER BY SubmittedAtUTC DESC", "@b", r.BusinessID).Rows)
                    {
                        int e = Convert.ToInt32(w["EmployeeID"]);
                        try { AnyminuteTimesheetRepair.Authorize(c, null, Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]), Convert.ToInt32(HttpContext.Current.Session["UserTypeid"]), r.BusinessID, e, false); }
                        catch (InvalidOperationException) { continue; }
                        m.Approvals++; m.Queue.Add(Card(c, r, e, team[e], w));
                    }
                    foreach (DataRow w in AnyminuteTimesheetRepair.Query(c, null, "SELECT EmployeeID,State FROM AnyminuteWeek WHERE BusinessID=@b AND WeekStart=@w AND State IN('Submitted','Approved') AND EmployeeID IN (" + ids + ")", "@b", r.BusinessID, "@w", m.Monday).Rows)
                    {
                        decimal h = H(Sum(c, r.BusinessID, Convert.ToInt32(w["EmployeeID"]), m.Monday)); m.TeamSubmitted += h;
                        if (Convert.ToString(w["State"]) == "Approved") m.TeamApproved += h; else m.TeamPending += h;
                    }
                    if (!sched) continue;
                    var active = new HashSet<int>();
                    // v20 (image 4): Upcoming Schedules show the employee's assigned project (schedules have no project field).
                    var projects = new Dictionary<int, List<string>>();
                    foreach (DataRow p in AnyminuteTimesheetRepair.Query(c, null, "SELECT DISTINCT x.EMPLOYEEID,p.ProjectName FROM ASSIGNPROJECT x JOIN AddProject p ON p.ProjectID=x.PROJECTID WHERE x.BUSINESSID=@b AND ISNULL(p.Status,'')<>'D' AND x.EMPLOYEEID IN (" + ids + ")", "@b", r.BusinessID).Rows)
                    {
                        int pe = Convert.ToInt32(p["EMPLOYEEID"]); List<string> pl;
                        if (!projects.TryGetValue(pe, out pl)) projects[pe] = pl = new List<string>();
                        pl.Add(Convert.ToString(p["ProjectName"]).Trim());
                    }
                    foreach (DataRow d in AnyminuteTimesheetRepair.Query(c, null, "SELECT EmployeeID,WorkDate,StartTime,EndTime,BreakMinutes,Notes FROM dbo.AnyminuteWeeklySchedule WHERE BusinessID=@b AND ScheduleType='Working' AND WorkDate BETWEEN @f AND @t AND EmployeeID IN (" + ids + ") ORDER BY WorkDate,StartTime", "@b", r.BusinessID, "@f", m.Monday, "@t", sunday).Rows)
                    {
                        int e = Convert.ToInt32(d["EmployeeID"]); active.Add(e);
                        var sh = new AnyminuteDashShift { BusinessID = r.BusinessID, EmployeeID = e, Business = r.Name, Employee = team[e], Date = Convert.ToDateTime(d["WorkDate"]).Date, Notes = d["Notes"] == DBNull.Value ? "" : Convert.ToString(d["Notes"]) };
                        if (d["StartTime"] != DBNull.Value && d["EndTime"] != DBNull.Value) { sh.Start = (TimeSpan)d["StartTime"]; sh.End = (TimeSpan)d["EndTime"]; sh.Hours = Hrs(sh.Start, sh.End, Convert.ToInt32(d["BreakMinutes"])); }
                        List<string> names; if (projects.TryGetValue(e, out names)) sh.Project = names.Count <= 2 ? String.Join(", ", names) : names[0] + ", " + names[1] + " +" + (names.Count - 2);
                        m.TeamScheduled += sh.Hours; m.Upcoming.Add(sh); // v20: whole week (Mon-Sun), as in the design
                    }
                    m.ActiveSchedules += active.Count;
                }
        }
        m.Recent = m.Recent.OrderByDescending(x => x.Week).Take(5).ToList();
        m.Queue = m.Queue.Take(10).ToList(); m.Upcoming = m.Upcoming.OrderBy(x => x.Date).ThenBy(x => x.Start).Take(12).ToList();
        return m;
    }
    static AnyminuteDashCard Card(SqlConnection c, AnyminuteBusinessRole r, int e, string name, DataRow w)
    {
        var week = Convert.ToDateTime(w["WeekStart"]).Date;
        DateTime? sub = null; if (w["SubmittedAtUTC"] != DBNull.Value) sub = DateTime.SpecifyKind(Convert.ToDateTime(w["SubmittedAtUTC"]), DateTimeKind.Utc).ToLocalTime();
        return new AnyminuteDashCard { BusinessID = r.BusinessID, EmployeeID = e, Business = r.Name, Employee = name, State = Convert.ToString(w["State"]).Trim(), Week = week, Submitted = sub, Hours = H(Sum(c, r.BusinessID, e, week)) };
    }

    // ---------- HTML (encoded; no user markup) ----------
    static string E(string s) { return HttpUtility.HtmlEncode(s ?? ""); }
    static string A(string s) { return HttpUtility.HtmlAttributeEncode(s ?? ""); }
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static string D(DateTime d) { return d.ToString("MMM dd, yyyy", Inv); }
    static string N(decimal d) { return d.ToString("0.00", Inv); }
    static string T(TimeSpan t) { return DateTime.Today.Add(t).ToString("h:mm tt", Inv); }
    static string Url(string page, AnyminuteDashModel m, params string[] extra)
    {
        string q = String.Join("&", extra.Where(x => !String.IsNullOrEmpty(x)));
        return page + (q.Length > 0 ? "?" + q : "");
    }
    static string Biz(AnyminuteDashModel m) { return m.All ? "" : "BusinessId=" + m.Selected; }

    // v19 (t79u): every KPI card opens the page behind its number.
    static void Stat(StringBuilder h, string icon, string title, string value, string sub, string href)
    {
        if (String.IsNullOrEmpty(href)) { Stat(h, icon, title, value, sub); return; }
        h.Append("<a class='am-card am-stat am-stat-link' href='").Append(A(href)).Append("' title='").Append(A("Open " + title)).Append("'><span class='am-stat-icon' data-am-icon='").Append(icon).Append("' aria-hidden='true'></span><div><small>")
         .Append(E(title)).Append("</small><strong>").Append(E(value)).Append("</strong><small>").Append(E(sub)).Append("</small></div></a>");
    }
    static void Stat(StringBuilder h, string icon, string title, string value, string sub)
    {
        h.Append("<div class='am-card am-stat'><span class='am-stat-icon' data-am-icon='").Append(icon).Append("' aria-hidden='true'></span><div><small>")
         .Append(E(title)).Append("</small><strong>").Append(E(value)).Append("</strong><small>").Append(E(sub)).Append("</small></div></div>");
    }
    static void Action(StringBuilder h, string icon, string title, string sub, string href)
    {
        h.Append("<a class='am-rd-action' href='").Append(A(href)).Append("'><span data-am-icon='").Append(icon).Append("' aria-hidden='true'></span><span><strong>")
         .Append(E(title)).Append("</strong><small>").Append(E(sub)).Append("</small></span></a>");
    }

    public static string Render(AnyminuteDashModel m, string firstName)
    {
        var h = new StringBuilder("<div class='am-rd' data-am-dashboard='" + m.Kind + "'>");
        bool sup = m.Kind == "supervisor";
        h.Append("<div class='am-title'><div><h1>").Append(sup ? "Supervisor Dashboard" : "Employee Dashboard").Append("</h1><p>")
         .Append(sup ? "Manage your team&#39;s time cards, schedules and employees." : "Here is your schedule and time card information.").Append("</p></div></div>");
        // v20 (Pandey #1-#3). Pickers:
        //  - one business, employee only (image 1 left): fixed business name + role pill;
        //  - several businesses, or any supervisor role (images 1 right, 3, 4): Select Business (All My Businesses (N)
        //    when N > 1; role tags only when the user supervises somewhere);
        //  - Switch Dashboard whenever the user is Employee somewhere and Supervisor somewhere (image 3). For a
        //    supervisor-only / employee-only business the other dashboard is listed but disabled.
        // Right card: Your Roles by Business for dual-role users (image 3); "Your Role for This Business: Supervisor"
        // for supervisor-only users (image 4); none for a pure employee (image 1).
        bool anySup = m.Roles.Any(r => r.Supervisor);
        h.Append("<div class='am-rd-top").Append(anySup ? "" : " am-rd-top-full").Append("'><div class='am-card am-rd-pickers'>");
        if (m.Roles.Count == 1 && !anySup)
        {
            var r = m.Roles[0];
            h.Append("<div class='am-rd-single'><span data-am-icon='business' aria-hidden='true'></span><strong>").Append(E(r.Name)).Append("</strong><span class='am-rd-pill'>Employee</span></div>");
        }
        else
        {
            h.Append("<label class='am-rd-field'><span>Select Business</span><select id='AmDashBusiness' onchange=\"location.href='Dashboard.aspx?b='+encodeURIComponent(this.value)\">");
            if (m.Roles.Count > 1) h.Append("<option value='all'").Append(m.All ? " selected" : "").Append(">All My Businesses (").Append(m.Roles.Count).Append(")</option>");
            foreach (var r in m.Roles)
                h.Append("<option value='").Append(r.BusinessID).Append("'").Append(Convert.ToString(r.BusinessID) == m.Selected ? " selected" : "").Append(">")
                 .Append(E(r.Name + (anySup ? " (" + r.Label + ")" : ""))).Append("</option>");
            h.Append("</select></label>");
        }
        if (m.DualRole)
        {
            bool scopeEmp = m.Scope.Any(r => r.Employee), scopeSup = m.Scope.Any(r => r.Supervisor);
            h.Append("<label class='am-rd-field'><span>Switch Dashboard</span><select id='AmDashMode' onchange=\"location.href='Dashboard.aspx?b=")
             .Append(A(HttpUtility.UrlEncode(m.Selected))).Append("&amp;mode='+this.value\"><option value='employee'").Append(sup ? "" : " selected").Append(scopeEmp ? "" : " disabled")
             .Append(">Employee Dashboard").Append(scopeEmp ? "" : " (not available for this business)").Append("</option><option value='supervisor'").Append(sup ? " selected" : "").Append(scopeSup ? "" : " disabled")
             .Append(">Supervisor Dashboard").Append(scopeSup ? "" : " (not available for this business)").Append("</option></select></label>");
        }
        h.Append("</div>");
        if (m.DualRole)
        {
            h.Append("<div class='am-card am-rd-roles'><h2><span data-am-icon='business' aria-hidden='true'></span>Your Roles by Business</h2><table class='am-table'><colgroup><col /><col class='am-rd-rolecol' /><col class='am-rd-rolecol' /></colgroup><thead><tr><th>Business</th><th>Employee</th><th>Supervisor</th></tr></thead><tbody>");
            foreach (var r in m.Roles)
                h.Append("<tr").Append(Convert.ToString(r.BusinessID) == m.Selected ? " class='is-current'" : "").Append("><td>").Append(E(r.Name)).Append("</td><td>").Append(r.Employee ? "<span class='am-rd-yes' role='img' aria-label='Yes'>&#10003;</span>" : "<span class='am-rd-no' role='img' aria-label='No'>&#8212;</span>")
                 .Append("</td><td>").Append(r.Supervisor ? "<span class='am-rd-yes' role='img' aria-label='Yes'>&#10003;</span>" : "<span class='am-rd-no' role='img' aria-label='No'>&#8212;</span>").Append("</td></tr>");
            h.Append("</tbody></table></div>");
        }
        else if (anySup)
            h.Append("<div class='am-card am-rd-rolecard'><span data-am-icon='people' aria-hidden='true'></span><div><small>Your Role for This Business</small><b>Supervisor</b><small>You can approve employee time cards, manage schedules and view reports.</small></div></div>");
        h.Append("</div>");
        if (sup) Supervisor(h, m); else Employee(h, m);
        return h.Append("</div>").ToString();
    }

    static void Employee(StringBuilder h, AnyminuteDashModel m)
    {
        h.Append("<div class='am-stats'>");
        Stat(h, "clock", "Hours Worked This Week", N(m.Hours) + " hrs", "Scheduled: " + N(m.Scheduled) + " hrs", Url("WeekTimeCard.aspx", m, Biz(m)));
        string one = m.Roles.Count > 1 && !m.All ? m.Scope.Select(r => r.Name).FirstOrDefault() : null; // v20: multi-business, one selected
        Stat(h, "dashboard", "Upcoming Shifts", m.Shifts.ToString(Inv), m.All && m.Roles.Count > 1 ? "This week (All businesses)" : one != null ? "This week (" + one + ")" : "This week", Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)));
        Stat(h, "check", "My Pending Time Cards", m.Pending.ToString(Inv), "Waiting for approval", "TimeCards.aspx");
        Stat(h, "folder", "My Projects", m.Projects.ToString(Inv), "Active projects", Url("Projects.aspx", m, Biz(m)));
        h.Append("</div>");
        // My Schedule week strip
        h.Append("<section class='am-card'><div class='am-rd-head'><h2>My Schedule").Append(m.All && m.Roles.Count > 1 ? " (All Businesses)" : one != null ? " \u2013 " + E(one) : "").Append("<small>")
         .Append(E(m.Monday.ToString("MMMM d, yyyy", Inv) + " \u2013 " + m.Monday.AddDays(6).ToString("MMMM d, yyyy", Inv))).Append("</small></h2><a href='")
         .Append(A(Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)))).Append("'>View Full Schedule &#8250;</a></div><div class='am-rd-week'>");
        for (int i = 0; i < 7; i++)
        {
            var date = m.Monday.AddDays(i); var days = m.Days.Where(x => x.Date == date).ToList();
            h.Append("<div class='am-rd-day'><div class='am-rd-dayname'><b>").Append(date.ToString("ddd", Inv)).Append("</b><small>").Append(date.ToString("MMM d", Inv)).Append("</small></div>");
            var work = days.Where(x => x.Working).ToList();
            if (work.Count == 0) h.Append("<div class='am-rd-off'><span data-am-icon='dashboard' aria-hidden='true'></span>").Append(days.Count == 0 ? "Not scheduled" : "Day Off").Append("</div>");
            foreach (var d in work)
                h.Append("<div class='am-rd-shift'>").Append(m.All && m.Roles.Count > 1 ? "<b>" + E(d.Business) + "</b>" : "<b>Working</b>")
                 .Append("<span>").Append(E(T(d.Start) + " \u2013 " + T(d.End))).Append("</span><strong>").Append(N(d.Hours)).Append(" hrs</strong></div>");
            h.Append("</div>");
        }
        h.Append("</div></section>");
        bool both = m.CanSwitch;
        h.Append("<div class='am-rd-grid").Append(both ? "" : " am-rd-stack").Append("'><section class='am-card'><div class='am-rd-head'><h2>").Append(both ? "My Recent Time Cards" : "Recent Time Cards").Append(m.All && m.Roles.Count > 1 ? " (All Businesses)" : "")
         .Append("</h2><a href='TimeCards.aspx'>View All &#8250;</a></div><div class='am-table-wrap'><table class='am-table'><thead><tr>").Append(m.All && m.Roles.Count > 1 ? "<th>Business</th>" : "")
         .Append("<th>Week</th><th>Total Hours</th><th>Status</th><th>Submitted On</th><th>Action</th></tr></thead><tbody>");
        foreach (var c in m.Recent)
            h.Append("<tr>").Append(m.All && m.Roles.Count > 1 ? "<td><strong>" + E(c.Business) + "</strong></td>" : "").Append("<td>").Append(E(c.Week.ToString("MMM dd", Inv) + " \u2013 " + D(c.Week.AddDays(6))))
             .Append("</td><td>").Append(N(c.Hours)).Append("</td><td><span class='am-badge ").Append(A(c.State)).Append("'>").Append(E(c.State)).Append("</span></td><td>")
             .Append(c.Submitted.HasValue ? E(D(c.Submitted.Value)) : "&#8212;").Append("</td><td><a class='am-button secondary am-rd-view' href='")
             .Append(A("WeekTimeCard.aspx?BusinessId=" + c.BusinessID + "&Week=" + c.Week.ToString("yyyy-MM-dd", Inv))).Append("'>View</a></td></tr>");
        if (m.Recent.Count == 0) h.Append("<tr><td colspan='6' class='am-muted'>No time cards yet.</td></tr>");
        h.Append("</tbody></table></div></section>");
        h.Append("<section class='am-card'><h2>Quick Actions</h2><div class='am-rd-actions'>");
        Action(h, "clock", "My Time Card", "Enter and submit my hours", Url("WeekTimeCard.aspx", m, Biz(m)));
        Action(h, "dashboard", "View My Schedule", "See upcoming shifts and days off", Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)));
        Action(h, "folder", "My Projects", "View my assigned projects", Url("Projects.aspx", m, Biz(m)));
        if (both) Action(h, "check", "Team Approvals", "Switch to Supervisor view", "Dashboard.aspx?b=" + HttpUtility.UrlEncode(m.Selected) + "&mode=supervisor");
        else Action(h, "business", "My Businesses", "View business details", "Dashboard.aspx?view=businesses");
        h.Append("</div></section></div>");
    }

    static void Supervisor(StringBuilder h, AnyminuteDashModel m)
    {
        bool multi = m.All && m.Roles.Count(r => r.Supervisor) > 1;
        h.Append("<div class='am-stats am-rd-sup'>");
        Stat(h, "check", "Pending Approvals", m.Approvals.ToString(Inv), "Time cards waiting for approval", "TimeCards.aspx?status=Submitted");
        Stat(h, "people", "Team Employees", m.Team.ToString(Inv), "Active employees", m.Directory ? Url("Users.aspx", m, Biz(m)) : null); // v20: image 4 labels
        Stat(h, "dashboard", "Active Schedules", m.ActiveSchedules.ToString(Inv), "Employees this week", Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)));
        Stat(h, "folder", "Active Projects", m.ActiveProjects.ToString(Inv), "Team projects", Url("Projects.aspx", m, Biz(m)));
        h.Append("</div><div class='am-rd-grid'><div>");
        h.Append("<section class='am-card'><div class='am-rd-head'><h2>Pending Time Card Approvals</h2><a href='TimeCards.aspx?status=Submitted'>View All &#8250;</a></div><div class='am-table-wrap'><table class='am-table'><thead><tr><th>Employee</th>")
         .Append(multi ? "<th>Business</th>" : "").Append("<th>Week</th><th>Total Hours</th><th>Status</th><th>Submitted On</th><th>Action</th></tr></thead><tbody>");
        foreach (var c in m.Queue)
            h.Append("<tr><td><strong>").Append(E(c.Employee)).Append("</strong></td>").Append(multi ? "<td>" + E(c.Business) + "</td>" : "").Append("<td>").Append(E(c.Week.ToString("MMM dd", Inv) + " \u2013 " + D(c.Week.AddDays(6))))
             .Append("</td><td>").Append(N(c.Hours)).Append("</td><td><span class='am-badge Submitted'>Submitted</span></td><td>").Append(c.Submitted.HasValue ? E(D(c.Submitted.Value)) : "&#8212;")
             .Append("</td><td><a class='am-button secondary am-rd-view' href='").Append(A(AnyminuteModernScope.Url("WeekTimeCard.aspx", c.BusinessID, c.EmployeeID) + "&Week=" + c.Week.ToString("yyyy-MM-dd", Inv))).Append("'>Review</a></td></tr>");
        if (m.Queue.Count == 0) h.Append("<tr><td colspan='7' class='am-muted'>No submitted time cards are waiting for your approval.</td></tr>");
        h.Append("</tbody></table></div></section>");
        h.Append("<section class='am-card'><div class='am-rd-head'><h2>Upcoming Schedules (This Week)</h2><a href='").Append(A(Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)))).Append("'>View Full Schedule &#8250;</a></div><div class='am-table-wrap'><table class='am-table'><thead><tr><th>Employee</th>")
         .Append(multi ? "<th>Business</th>" : "").Append("<th>Date</th><th>Shift</th><th>Hours</th><th>Project</th><th>Status</th></tr></thead><tbody>");
        foreach (var s in m.Upcoming)
            h.Append("<tr><td>").Append(E(s.Employee)).Append("</td>").Append(multi ? "<td>" + E(s.Business) + "</td>" : "").Append("<td>").Append(E(s.Date.ToString("MMM dd, yyyy (ddd)", Inv)))
             .Append("</td><td>").Append(E(T(s.Start) + " \u2013 " + T(s.End))).Append("</td><td>").Append(N(s.Hours)).Append("</td><td>").Append(String.IsNullOrEmpty(s.Project) ? (String.IsNullOrEmpty(s.Notes) ? "&#8212;" : E(s.Notes)) : E(s.Project))
             .Append("</td><td><span class='am-badge Active'>Scheduled</span></td></tr>");
        if (m.Upcoming.Count == 0) h.Append("<tr><td colspan='7' class='am-muted'>No shifts are scheduled for your team this week.</td></tr>");
        h.Append("</tbody></table></div></section></div><div>");
        h.Append("<section class='am-card'><h2>").Append(m.CanSwitch ? "Supervisor Actions" : "Quick Actions").Append("</h2><div class='am-rd-actions'>");
        Action(h, "clock", "Review Time Cards", "Approve or reject employee time cards", "TimeCards.aspx?status=Submitted");
        Action(h, "dashboard", "Manage Schedules", "Create and edit employee schedules", Url("WeeklyEmployeeSchedule.aspx", m, Biz(m)));
        if (m.Directory) Action(h, "people", "View Employees", "See team members and details", "Users.aspx");
        if (m.ReportHref != null) Action(h, "reports", m.CanSwitch ? "Team Reports" : "Reports", "View time and attendance reports", m.ReportHref.EndsWith("/Reports.aspx", StringComparison.OrdinalIgnoreCase) ? m.ReportHref + "?b=" + HttpUtility.UrlEncode(m.Selected) : m.ReportHref);
        h.Append("</div></section>");
        h.Append("<section class='am-card am-rd-summary'><h2>Team Summary (This Week)</h2><dl>");
        foreach (var kv in new[] { new KeyValuePair<string, decimal>("Total Scheduled Hours", m.TeamScheduled), new KeyValuePair<string, decimal>("Total Submitted Hours", m.TeamSubmitted), new KeyValuePair<string, decimal>("Approved Hours", m.TeamApproved), new KeyValuePair<string, decimal>("Pending Hours", m.TeamPending) })
            h.Append("<div><dt>").Append(E(kv.Key)).Append("</dt><dd>").Append(N(kv.Value)).Append("</dd></div>");
        h.Append("</dl></section></div></div>");
    }
}
