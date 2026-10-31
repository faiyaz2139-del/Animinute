using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
// 2026-10-30 v33 (Shamsu deck "Accountant & Payroll Agency Dashboard"): accountant / payroll-agency logins get the deck
// dashboard (Select Business, 4 KPI cards, Assigned Businesses table, Quick Actions, Recent Activity) and the deck's flat
// navigation. Read-only: every figure comes from tables the app already has; every link is an existing page.
// v37 (PPT s1 strict + "accountant sees a different dashboard"): legacy 0/Admin logins land here too, Contact email fixed,
// Documents page, Access History page, nav icons, optional activity log (DB_OPTIONAL/OPTIONAL_03).
public static class AnyminuteAccountantDashboard
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static string H(object o) { return HttpUtility.HtmlEncode(Convert.ToString(o)); }
    static string A(object o) { return HttpUtility.HtmlAttributeEncode(Convert.ToString(o)); }
    static string U(string p) { return VirtualPathUtility.ToAbsolute(p); }

    // Same accountant test Dashboard.aspx has used since v27.
    public static bool Applies()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null || h.Session["EmployeeId"] == null) return false;
        string t = Convert.ToString(h.Session["UserTypeid"]).Trim();
        return t == "12" || (t == "0" && Convert.ToString(h.Session["UserType"]) == "Admin");
    }
    // v37: legacy accountant / payroll-agency logins (UserAcc 0/"Admin", EmployeeId 0, linked to a MotherOrg) used to land on the
    // old Accountant_Dashboard.aspx (the "different dashboard"). Their businesses are the MotherOrg's businesses.
    public static bool Legacy()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null) return false;
        return Convert.ToString(h.Session["UserTypeid"]).Trim() == "0" && Convert.ToString(h.Session["UserType"]) == "Admin";
    }
    public static DataTable Businesses()
    {
        if (!Legacy()) return AnyminuteModernScope.Businesses();
        AnyminuteModernScope.Login(); int m; Int32.TryParse(Convert.ToString(HttpContext.Current.Session["MotherOrgid"]), out m);
        using (var c = AnyminuteTimesheetRepair.Open())
            return AnyminuteTimesheetRepair.Query(c, null, "SELECT b.BusinessID,b.CompanyName,b.PrimaryContactName,b.Status FROM AddBusiness b WHERE b.Status<>2 AND b.MotherOrgId=@m AND @m>0 ORDER BY b.CompanyName", "@m", m);
    }

    // v37 (PPT s1 header): "Welcome! <firm>" / "Accountant & Payroll Admin", bell, initials circle + firm name.
    public static string Firm()
    {
        var h = HttpContext.Current; var cached = h.Items["AmApFirm"] as string; if (cached != null) return cached;
        string firm = "";
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (Legacy())
                {
                    int m; Int32.TryParse(Convert.ToString(h.Session["MotherOrgid"]), out m);
                    var r = AnyminuteTimesheetRepair.Query(c, null, "SELECT MotherOrgName FROM MotherOrg WHERE MotherOrgID=@m", "@m", m);
                    if (r.Rows.Count == 1) firm = Convert.ToString(r.Rows[0][0]).Trim();
                }
                else if (AnyminuteAccessService.Installed(c, null))
                {
                    var r = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 FirmName FROM AnyminuteAccessInvite WHERE UserID=@u AND Status<>'Revoked' AND LTRIM(RTRIM(ISNULL(FirmName,'')))<>'' ORDER BY InviteID DESC", "@u", Convert.ToInt32(h.Session["UserId"]));
                    if (r.Rows.Count == 1) firm = Convert.ToString(r.Rows[0][0]).Trim();
                }
            }
        }
        catch (System.Data.SqlClient.SqlException) { firm = ""; }
        h.Items["AmApFirm"] = firm; return firm;
    }
    public static string Initials(string name)
    {
        var parts = (name ?? "").Split(new[] { ' ', '-', '&' }, StringSplitOptions.RemoveEmptyEntries).Where(p => Char.IsLetterOrDigit(p[0])).ToArray();
        if (parts.Length == 0) return "?";
        return (parts[0].Substring(0, 1) + (parts.Length > 1 ? parts[parts.Length - 1].Substring(0, 1) : "")).ToUpperInvariant();
    }

    // v37: optional activity log (DB_OPTIONAL/OPTIONAL_03_AccountantActivityLog.sql) for Recent Activity / Access History:
    // report and document downloads, employee list views. Never throws; does nothing when the table is not installed.
    public static void Log(int business, string action, string detail)
    {
        try
        {
            var h = HttpContext.Current; if (h == null || h.Session == null || !Applies()) return;
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (!AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteAccessLog")) return;
                AnyminuteTimesheetRepair.Execute(c, null, "INSERT dbo.AnyminuteAccessLog(UserID,BusinessID,Action,Detail,AtUTC) VALUES(@u,@b,@a,@d,SYSUTCDATETIME())",
                    "@u", Convert.ToInt32(h.Session["UserId"]), "@b", business > 0 ? (object)business : DBNull.Value, "@a", Cut(action, 60), "@d", Cut(detail, 200));
            }
        }
        catch (Exception) { }
    }
    static string Cut(string s, int n) { s = (s ?? "").Trim(); return s.Length > n ? s.Substring(0, n) : s; }
    public static string NavIcon(string name)
    {
        string d = name == "Home" ? "M3 11l9-8 9 8M5 10v10h5v-6h4v6h5V10"
            : name == "Assigned Businesses" ? "M6 21V3h12v18M3 21h18M9 7h1m4 0h1M9 11h1m4 0h1M10 21v-5h4v5"
            : name == "Employees" ? "M9 11a3 3 0 100-6 3 3 0 000 6zM3 20c0-3 3-5 6-5s6 2 6 5M16 11a3 3 0 100-6M18 15c2 .5 3 2.4 3 5"
            : name == "Payroll Hours" ? "M12 21a9 9 0 100-18 9 9 0 000 18zM12 7v5l3 2"
            : name == "Reports" ? "M6 20V10M12 20V4M18 20v-7"
            : name == "Documents" ? "M7 3h7l5 5v13H7zM14 3v5h5M10 13h6M10 17h6"
            : "M3 12a9 9 0 103-6.7L3 8M3 3v5h5M12 7v5l3 2";
        return "<svg class='am-ap-navico' viewBox='0 0 24 24' width='20' height='20' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true' focusable='false'><path d='" + d + "'/></svg>";
    }

    static string ReportHref(IEnumerable<MenuViewModel> menu)
    {
        try { return AnyminuteReports.HubHref(menu ?? AnyminuteUiNavigation.Load()); } catch (HttpException) { return null; } catch (InvalidOperationException) { return null; }
    }
    // v37 (PPT s1 Documents / Download Documents): the accountant Documents page (client payroll documents per business).
    static string DocumentsHref(IEnumerable<MenuViewModel> menu) { return U("~/AccountantDocuments.aspx"); }

    // Deck left navigation: Home, Assigned Businesses, Employees, Payroll Hours, Reports, Documents, Access History.
    public static List<KeyValuePair<string, string>> NavLinks(IEnumerable<MenuViewModel> menu)
    {
        var l = new List<KeyValuePair<string, string>>();
        Action<string, string> add = (n, h) => { if (!String.IsNullOrEmpty(h)) l.Add(new KeyValuePair<string, string>(n, h)); };
        add("Home", U("~/Dashboard.aspx"));
        add("Assigned Businesses", U("~/Dashboard.aspx?view=businesses"));
        add("Employees", U("~/Users.aspx"));
        add("Payroll Hours", U("~/TimeCards.aspx"));
        add("Reports", ReportHref(menu));
        add("Documents", DocumentsHref(menu)); // v37: real Documents page (AccountantDocuments.aspx)
        add("Access History", U("~/Dashboard.aspx?view=activity")); // v37: full activity / access history page
        return l;
    }

    sealed class Biz { public int Id, Employees, Pending, Rejected; public string Name, Contact, Email, Phone, Access; public DateTime? Last, FirstPending; }

    public static string Render(string selected)
    {
        var rows = Businesses();
        var all = new List<Biz>();
        int me = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool weeks = AnyminuteTimesheetRepair.Query(c, null, "SELECT OBJECT_ID('dbo.AnyminuteWeek','U')").Rows[0][0] != DBNull.Value;
            bool invites = AnyminuteAccessService.Installed(c, null);
            foreach (DataRow r in rows.Rows)
            {
                var b = new Biz { Id = Convert.ToInt32(r["BusinessID"]), Name = Convert.ToString(r["CompanyName"]), Contact = Convert.ToString(r["PrimaryContactName"]), Access = "Accountant" };
                // v37 (PPT s1, "Contact email must show"): v36 read AddBusiness.EMail_primary, a column that exists only on AddEmployee,
                // so the query failed silently and no email appeared. The business email is BusinessAddress.MainEmail (CCEmail next),
                // then the business owner's login / address email; the contact name falls back to the owner's name.
                try
                {
                    var em = AnyminuteTimesheetRepair.Query(c, null, @"SELECT TOP 1 COALESCE(NULLIF(LTRIM(RTRIM(ba.MainEmail)),''),NULLIF(LTRIM(RTRIM(ba.CCEmail)),''),
                        (SELECT TOP 1 NULLIF(LTRIM(RTRIM(ea.MainEmail)),'') FROM AssginBussiness o JOIN AddEmployee e ON e.EmployeeID=o.EmployeeID JOIN EmpAddress ea ON ea.EmpAddressId=e.ADDRESSID WHERE o.BusinessID=b.BusinessID AND o.UserTypeId=1 AND ea.MainEmail NOT LIKE '%blank'),
                        (SELECT TOP 1 NULLIF(LTRIM(RTRIM(u.Email)),'') FROM AssginBussiness o JOIN UserAcc u ON u.EmployeeId=o.EmployeeID WHERE o.BusinessID=b.BusinessID AND o.UserTypeId=1 AND u.Email NOT LIKE '%blank')) AS Email,
                        (SELECT TOP 1 dbo.FN_DESC('AddEmployee',o.EmployeeID) FROM AssginBussiness o WHERE o.BusinessID=b.BusinessID AND o.UserTypeId=1) AS OwnerName
                        FROM AddBusiness b LEFT JOIN BusinessAddress ba ON ba.BusinessAddressId=b.ADDRESSID WHERE b.BusinessID=@b", "@b", b.Id);
                    if (em.Rows.Count == 1)
                    {
                        if (em.Rows[0]["Email"] != DBNull.Value) b.Email = Convert.ToString(em.Rows[0]["Email"]).Trim();
                        if (String.IsNullOrWhiteSpace(b.Contact) && em.Rows[0]["OwnerName"] != DBNull.Value) b.Contact = Convert.ToString(em.Rows[0]["OwnerName"]).Trim();
                    }
                }
                catch (System.Data.SqlClient.SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("Accountant dashboard contact email failed for business " + b.Id, ex); b.Email = null; }
                b.Employees = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=@b AND a.UserTypeId<>12", "@b", b.Id).Rows[0][0]);
                if (weeks)
                {
                    var w = AnyminuteTimesheetRepair.Query(c, null, @"SELECT SUM(CASE WHEN State='Submitted' THEN 1 ELSE 0 END),SUM(CASE WHEN State='Rejected' THEN 1 ELSE 0 END),
                        MAX(CASE WHEN State='Approved' THEN WeekStart END) FROM dbo.AnyminuteWeek WHERE BusinessID=@b", "@b", b.Id).Rows[0];
                    b.Rejected = w[1] == DBNull.Value ? 0 : Convert.ToInt32(w[1]);
                    if (w[2] != DBNull.Value) b.Last = Convert.ToDateTime(w[2]);
                    // v35 (live smoke kk3): Pending Payroll Reviews counted every Submitted week in the business, but the
                    // Submitted queue (TimeCards.aspx?status=Submitted) lists only employees this user may approve. Count
                    // only those weeks (same Users(business) + Authorize(..., false) checks as the queue) and remember the
                    // oldest one so Open lands on a week that actually has rows.
                    var reviewable = new HashSet<int>();
                    int sessionEmp = Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]), sessionRole = Convert.ToInt32(HttpContext.Current.Session["UserTypeid"]);
                    try
                    {
                        foreach (DataRow u in AnyminuteModernScope.Users(b.Id).Rows)
                        {
                            int emp = Convert.ToInt32(u["EmployeeID"]);
                            try { AnyminuteTimesheetRepair.Authorize(c, null, sessionEmp, sessionRole, b.Id, emp, false); reviewable.Add(emp); }
                            catch (InvalidOperationException) { }
                        }
                    }
                    catch (InvalidOperationException) { } // v37: legacy agency logins have no per-business membership row
                    catch (HttpException) { }
                    foreach (DataRow p in AnyminuteTimesheetRepair.Query(c, null, "SELECT EmployeeID,WeekStart FROM dbo.AnyminuteWeek WHERE BusinessID=@b AND State='Submitted'", "@b", b.Id).Rows)
                    {
                        if (!reviewable.Contains(Convert.ToInt32(p["EmployeeID"]))) continue;
                        b.Pending++; DateTime ws = Convert.ToDateTime(p["WeekStart"]).Date;
                        if (!b.FirstPending.HasValue || ws < b.FirstPending.Value) b.FirstPending = ws;
                    }
                }
                if (invites)
                {
                    var t = AnyminuteTimesheetRepair.Query(c, null, @"SELECT TOP(1) i.IsAccountant,i.IsPayrollAdmin FROM dbo.AnyminuteAccessInvite i JOIN dbo.AnyminuteAccessInviteBusiness x ON x.InviteID=i.InviteID
                        WHERE i.UserID=@u AND x.BusinessID=@b AND i.Status<>'Revoked' ORDER BY i.InviteID DESC", "@u", me, "@b", b.Id);
                    if (t.Rows.Count == 1)
                    {
                        bool acc = Convert.ToBoolean(t.Rows[0][0]), pay = Convert.ToBoolean(t.Rows[0][1]);
                        b.Access = acc && pay ? "Both" : pay ? "Payroll Admin" : "Accountant";
                    }
                }
                all.Add(b);
            }
        }
        if (selected != null && !all.Any(b => b.Id.ToString(Inv) == selected)) selected = null;
        var scope = selected == null ? all : all.Where(b => b.Id.ToString(Inv) == selected).ToList();
        var menu = AnyminuteUiNavigation.Load().ToList();
        string report = ReportHref(menu), docs = DocumentsHref(menu);
        var activity = Activity(all.ToDictionary(b => b.Id, b => b.Name), 4);
        int reports = 0; try { reports = AnyminuteReports.Available(selected ?? "all").Count; } catch (HttpException) { } catch (InvalidOperationException) { } catch (System.Data.SqlClient.SqlException) { }

        var s = new StringBuilder("<div class='am-ap'>");
        s.Append("<div class='am-ap-head'><div><h1>Accountant &amp; Payroll Dashboard</h1><p>Manage assigned businesses, review payroll data, and export reports.</p></div>")
         .Append("<label class='am-card am-ap-select'>").Append(Svg("business")).Append("<span><small>Select Business</small><select onchange=\"location.href='Dashboard.aspx'+(this.value?'?b='+encodeURIComponent(this.value):'')\">")
         .Append("<option value=''>All Assigned Businesses (").Append(all.Count).Append(")</option>");
        foreach (var b in all) s.Append("<option value='").Append(b.Id).Append("'").Append(b.Id.ToString(Inv) == selected ? " selected" : "").Append(">").Append(H(b.Name)).Append("</option>");
        s.Append("</select></span></label></div>");

        s.Append("<div class='am-ap-kpis'>");
        Kpi(s, "blue", "business", "Assigned Businesses", scope.Count.ToString(Inv), U("~/Dashboard.aspx?view=businesses"));
        Kpi(s, "green", "people", "Employees in Access", scope.Sum(b => b.Employees).ToString(Inv), U("~/Users.aspx"));
        var firstPending = scope.Where(b => b.FirstPending.HasValue).Select(b => b.FirstPending.Value).DefaultIfEmpty(DateTime.MinValue).Min();
        Kpi(s, "orange", "clock", "Pending Payroll Reviews", scope.Sum(b => b.Pending).ToString(Inv), U("~/TimeCards.aspx?status=Submitted" + (firstPending > DateTime.MinValue ? "&Week=" + firstPending.ToString("yyyy-MM-dd", Inv) : "")));
        Kpi(s, "purple", "reports", "Available Reports", reports.ToString(Inv), report ?? "#");
        s.Append("</div>");

        s.Append("<section class='am-card am-ap-biz'><div class='am-ap-sechead'><h2>").Append(Svg("business")).Append("Assigned Businesses</h2>")
         .Append("<label class='am-ap-search'><span class='sr-only'>Search businesses</span><input type='search' data-search='#am-ap-grid' placeholder='Search businesses...' /></label></div>")
         .Append("<div class='am-table-wrap'><table class='am-table' id='am-ap-grid'><thead><tr><th>Business Name</th><th>Contact</th><th>Access Type</th><th>Employees</th><th>Payroll Status</th><th>Last Payroll Period</th><th>Action</th></tr></thead><tbody>");
        foreach (var b in scope)
        {
            string status = b.Rejected > 0 ? "Action Required" : b.Pending > 0 ? "Pending Review" : "Up to Date";
            string cls = b.Rejected > 0 ? "red" : b.Pending > 0 ? "amber" : "green";
            // v36 (PPT s1 + s2, strict): as in the mock, Up to Date rows get Open (full working screen: Payroll Hours for this
            // business) and Pending Review / Action Required rows get View (read-only details: this dashboard scoped to the
            // business, with status and history). The pending approval queue stays on the Pending Payroll Reviews KPI (BusinessId + Week).
            bool open = cls == "green";
            string href = open ? U("~/TimeCards.aspx?BusinessId=" + b.Id + (b.FirstPending.HasValue ? "&Week=" + b.FirstPending.Value.ToString("yyyy-MM-dd", Inv) : "")) : U("~/Dashboard.aspx?b=" + b.Id + (b.FirstPending.HasValue ? "&Week=" + b.FirstPending.Value.ToString("yyyy-MM-dd", Inv) : ""));
            s.Append("<tr><td><a class='am-ap-name' href='").Append(A(U("~/Dashboard.aspx?b=" + b.Id))).Append("'>").Append(H(b.Name)).Append("</a></td>")
             .Append("<td><strong>").Append(H(b.Contact)).Append("</strong>").Append(String.IsNullOrEmpty(b.Email) ? "" : "<span class='am-ap-email'>" + H(b.Email) + "</span>").Append("</td>")
             .Append("<td><span class='am-ap-pill am-ap-").Append(b.Access == "Both" ? "both" : b.Access == "Payroll Admin" ? "pay" : "acc").Append("'>").Append(H(b.Access)).Append("</span></td>")
             .Append("<td>").Append(b.Employees).Append("</td>")
             .Append("<td><span class='am-ap-status am-ap-").Append(cls).Append("'>").Append(status).Append("</span></td>")
             .Append("<td>").Append(b.Last.HasValue ? H(b.Last.Value.ToString("MMM d", Inv) + " \u2013 " + b.Last.Value.AddDays(6).ToString("MMM d, yyyy", Inv)) : "\u2014").Append("</td>")
             .Append("<td><a class='am-button").Append(open ? "" : " secondary").Append(" am-ap-act' href='").Append(A(href)).Append("'>").Append(open ? "Open" : "View").Append("</a></td></tr>");
        }
        if (scope.Count == 0) s.Append("<tr><td colspan='7' class='am-ap-empty'>No businesses have shared accountant access with you yet. Ask the business owner to invite you from Accountant &amp; Payroll Access.</td></tr>");
        s.Append("</tbody></table></div></section>");

        s.Append("<div class='am-ap-bottom'><section class='am-card am-ap-quick'><div class='am-ap-sechead'><div><h2>").Append(Svg("bolt")).Append("Quick Actions</h2><p>Common tasks to help you manage payroll and client data.</p></div></div><div class='am-ap-qgrid'>");
        Quick(s, "blue", "clock", "View Payroll Hours", "Review and approve employee hours", U("~/TimeCards.aspx"));
        Quick(s, "green", "reports", "Export Reports", "Generate and download reports", report);
        Quick(s, "purple", "people", "Open Employee List", "View all employees across businesses", U("~/Users.aspx"));
        // v37 (PPT s1, strict): Download Documents opens the accountant Documents page.
        Quick(s, "orange", "doc", "Download Documents", "Access and download client documents", docs);
        s.Append("</div></section>");

        s.Append("<section class='am-card am-ap-activity' id='am-ap-activity'><div class='am-ap-sechead'><div><h2>").Append(Svg("history")).Append("Recent Activity</h2><p>Latest actions across your assigned businesses.</p></div><a href='")
         .Append(A(U("~/Dashboard.aspx?view=activity"))).Append("'>View All Activity &rarr;</a></div><ul>");
        ActivityItems(s, activity);
        s.Append("</ul></section></div></div>");
        return s.ToString();
    }

    // ---------- v37 Recent Activity / Access History ----------
    sealed class Act { public DateTime At; public string What, Text, Tone, Icon; }
    static List<Act> Activity(Dictionary<int, string> names, int top)
    {
        var list = new List<Act>(); if (names.Count == 0) return list;
        string ids = String.Join(",", names.Keys.Select(i => i.ToString(Inv)));
        Func<int, string> N = id => { string n; return names.TryGetValue(id, out n) ? n : ""; };
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (AnyminuteTimesheetRepair.Query(c, null, "SELECT OBJECT_ID('dbo.AnyminuteWeek','U')").Rows[0][0] != DBNull.Value)
                    foreach (DataRow a in AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP(" + top + @") w.BusinessID,w.State,w.WeekStart,w.ChangedAtUTC,dbo.FN_DESC('AddEmployee',w.EmployeeID) Emp
                        FROM dbo.AnyminuteWeek w WHERE w.BusinessID IN (" + ids + ") AND w.State IN('Submitted','Approved','Rejected') AND w.ChangedAtUTC IS NOT NULL ORDER BY w.ChangedAtUTC DESC").Rows)
                    {
                        string state = Convert.ToString(a["State"]); DateTime ws = Convert.ToDateTime(a["WeekStart"]);
                        list.Add(new Act { At = Convert.ToDateTime(a["ChangedAtUTC"]), What = state == "Approved" ? "Payroll hours reviewed" : state == "Rejected" ? "Payroll hours returned" : "Payroll hours submitted",
                            Text = N(Convert.ToInt32(a["BusinessID"])) + " \u2013 " + Convert.ToString(a["Emp"]).Trim() + " \u2013 " + ws.ToString("MMM d", Inv) + " \u2013 " + ws.AddDays(6).ToString("MMM d, yyyy", Inv),
                            Tone = state == "Rejected" ? "orange" : "blue", Icon = "clock" });
                    }
                if (AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteAccessLog"))
                    foreach (DataRow a in AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP(" + top + ") BusinessID,Action,Detail,AtUTC FROM dbo.AnyminuteAccessLog WHERE UserID=@u ORDER BY AtUTC DESC", "@u", Convert.ToInt32(HttpContext.Current.Session["UserId"])).Rows)
                    {
                        string what = Convert.ToString(a["Action"]); int bid = a["BusinessID"] == DBNull.Value ? 0 : Convert.ToInt32(a["BusinessID"]);
                        string detail = Convert.ToString(a["Detail"]).Trim(), biz = N(bid);
                        list.Add(new Act { At = Convert.ToDateTime(a["AtUTC"]), What = what, Text = biz.Length > 0 && detail.Length > 0 ? biz + " \u2013 " + detail : biz + detail,
                            Tone = what.StartsWith("Employee", StringComparison.Ordinal) ? "purple" : what.StartsWith("Document", StringComparison.Ordinal) ? "orange" : "green", Icon = what.StartsWith("Employee", StringComparison.Ordinal) ? "people" : "doc" });
                    }
            }
        }
        catch (System.Data.SqlClient.SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("Accountant activity failed", ex); }
        return list.OrderByDescending(a => a.At).Take(top).ToList();
    }
    static void ActivityItems(StringBuilder s, List<Act> activity)
    {
        foreach (var a in activity)
            s.Append("<li><span class='am-ap-ico am-ap-").Append(a.Tone).Append("'>").Append(Svg(a.Icon)).Append("</span><div><strong>").Append(H(a.What))
             .Append("</strong><small>").Append(H(a.Text)).Append("</small></div><time>").Append(H(Ago(a.At))).Append("</time></li>");
        if (activity.Count == 0) s.Append("<li class='am-ap-empty'>No activity yet.</li>");
    }
    // Access History (left navigation) / View All Activity: the last 100 actions across the assigned businesses.
    public static string RenderActivity()
    {
        var names = Businesses().Rows.Cast<DataRow>().ToDictionary(r => Convert.ToInt32(r["BusinessID"]), r => Convert.ToString(r["CompanyName"]));
        var s = new StringBuilder("<div class='am-ap'><div class='am-ap-head'><div><h1>Access History</h1><p>Latest actions across your assigned businesses: payroll hours, report and document downloads, employee list views.</p></div></div>");
        s.Append("<section class='am-card am-ap-activity am-ap-history'><ul>"); ActivityItems(s, Activity(names, 100)); s.Append("</ul></section></div>");
        return s.ToString();
    }

    // ---------- v37 Documents (PPT s1 nav "Documents" + Quick Action "Download Documents") ----------
    // Client payroll documents per assigned business, generated from the business's reports the accountant may export
    // (same permission matrix as Reports). Downloads go through AccountantDocuments.aspx?dl=1 so they are logged.
    public static string RenderDocuments(string selected, string period)
    {
        var names = Businesses().Rows.Cast<DataRow>().ToDictionary(r => Convert.ToInt32(r["BusinessID"]), r => Convert.ToString(r["CompanyName"]));
        if (selected != null && !names.ContainsKey(ToInt(selected))) selected = null;
        DateTime from, to; string label; Period(period, out from, out to, out label);
        List<AmRptBiz> scope; try { scope = AnyminuteReports.Scope(); } catch (InvalidOperationException) { scope = new List<AmRptBiz>(); } catch (HttpException) { scope = new List<AmRptBiz>(); }
        var s = new StringBuilder("<div class='am-ap am-ap-docs'><div class='am-ap-head'><div><h1>Documents</h1><p>Access and download client documents for your assigned businesses.</p></div>");
        s.Append("<div class='am-ap-docfilters'><label class='am-card am-ap-select'>").Append(Svg("business")).Append("<span><small>Select Business</small><select data-am-doc='b'>")
         .Append("<option value=''>All Assigned Businesses (").Append(names.Count).Append(")</option>");
        foreach (var n in names) s.Append("<option value='").Append(n.Key).Append("'").Append(n.Key.ToString(Inv) == selected ? " selected" : "").Append(">").Append(H(n.Value)).Append("</option>");
        s.Append("</select></span></label><label class='am-card am-ap-select'>").Append(Svg("clock")).Append("<span><small>Period</small><select data-am-doc='p'>");
        foreach (var p in new[] { new[] { "month", "This month" }, new[] { "lastmonth", "Last month" }, new[] { "week", "This week" }, new[] { "lastweek", "Last week" }, new[] { "year", "This year" } })
            s.Append("<option value='").Append(p[0]).Append("'").Append(p[0] == (period ?? "month") ? " selected" : "").Append(">").Append(p[1]).Append("</option>");
        s.Append("</select></span></label></div></div>");
        s.Append("<section class='am-card am-ap-biz'><div class='am-ap-sechead'><h2>").Append(Svg("doc")).Append("Client Documents <small class='am-ap-period'>").Append(H(label)).Append("</small></h2>")
         .Append("<label class='am-ap-search'><span class='sr-only'>Search documents</span><input type='search' data-search='#am-ap-docs' placeholder='Search documents...' /></label></div>")
         .Append("<div class='am-table-wrap'><table class='am-table' id='am-ap-docs' data-am-filter='1'><thead><tr><th>Document</th><th>Business</th><th>Period</th><th>Download</th></tr></thead><tbody>");
        int count = 0;
        foreach (var b in scope.Where(x => names.ContainsKey(x.Id) && (selected == null || x.Id.ToString(Inv) == selected)).OrderBy(x => x.Name))
            foreach (var d in AnyminuteReports.Defs)
            {
                AmRptPerm p; try { p = AnyminuteReports.Perm(d, b); } catch (Exception) { continue; }
                if (!p.Pdf && !p.Excel) continue; count++;
                string q = "AccountantDocuments.aspx?dl=1&r=" + Uri.EscapeDataString(d.Key) + "&b=" + b.Id + (d.DateMode == "none" ? "" : "&from=" + from.ToString("yyyy-MM-dd", Inv) + "&to=" + to.ToString("yyyy-MM-dd", Inv));
                s.Append("<tr><td><span class='am-ap-docname'>").Append(Svg("doc")).Append("<span><strong>").Append(H(d.Name)).Append("</strong><small>").Append(H(d.Desc)).Append("</small></span></span></td><td>")
                 .Append(H(b.Name)).Append("</td><td>").Append(d.DateMode == "none" ? "Current" : H(label)).Append("</td><td class='am-ap-dl'>");
                if (p.Pdf) s.Append("<a class='am-button am-ap-act' href='").Append(A(U("~/" + q + "&export=pdf"))).Append("'>PDF</a> ");
                if (p.Excel) s.Append("<a class='am-button secondary am-ap-act' href='").Append(A(U("~/" + q + "&export=excel"))).Append("'>Excel</a>");
                s.Append("</td></tr>");
            }
        if (count == 0) s.Append("<tr><td colspan='4' class='am-ap-empty'>No documents are shared with you for this business yet. The business owner controls this in Report Access.</td></tr>");
        s.Append("</tbody></table></div></section></div>");
        s.Append("<script>document.querySelectorAll('[data-am-doc]').forEach(function(x){x.addEventListener('change',function(){var b=document.querySelector(\"[data-am-doc='b']\").value,p=document.querySelector(\"[data-am-doc='p']\").value;location.href='AccountantDocuments.aspx?p='+encodeURIComponent(p)+(b?'&b='+encodeURIComponent(b):'');});});</script>");
        return s.ToString();
    }
    static int ToInt(string v) { int i; return Int32.TryParse(v, out i) ? i : 0; }
    public static void Period(string period, out DateTime from, out DateTime to, out string label)
    {
        var t = DateTime.Today; int dow = ((int)t.DayOfWeek + 6) % 7;
        switch (period)
        {
            case "lastmonth": from = new DateTime(t.Year, t.Month, 1).AddMonths(-1); to = from.AddMonths(1).AddDays(-1); break;
            case "week": from = t.AddDays(-dow); to = from.AddDays(6); break;
            case "lastweek": from = t.AddDays(-dow - 7); to = from.AddDays(6); break;
            case "year": from = new DateTime(t.Year, 1, 1); to = new DateTime(t.Year, 12, 31); break;
            default: from = new DateTime(t.Year, t.Month, 1); to = from.AddMonths(1).AddDays(-1); break;
        }
        label = from.ToString("MMM d", Inv) + " \u2013 " + to.ToString("MMM d, yyyy", Inv);
    }

    static string Ago(DateTime utc)
    {
        var d = DateTime.UtcNow - DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        if (d.TotalMinutes < 1) return "just now";
        if (d.TotalHours < 1) return (int)d.TotalMinutes + " min ago";
        if (d.TotalDays < 1) return (int)d.TotalHours + ((int)d.TotalHours == 1 ? " hour ago" : " hours ago");
        return (int)d.TotalDays + ((int)d.TotalDays == 1 ? " day ago" : " days ago");
    }
    static void Kpi(StringBuilder s, string tone, string icon, string label, string value, string href)
    {
        s.Append("<a class='am-card am-ap-kpi am-ap-").Append(tone).Append("' href='").Append(A(href)).Append("'><span class='am-ap-ico'>").Append(Svg(icon))
         .Append("</span><span><small>").Append(H(label)).Append("</small><strong>").Append(H(value)).Append("</strong></span><span class='am-ap-chev' aria-hidden='true'>&rsaquo;</span></a>");
    }
    static void Quick(StringBuilder s, string tone, string icon, string title, string text, string href)
    {
        if (String.IsNullOrEmpty(href)) return;
        s.Append("<a class='am-ap-q am-ap-").Append(tone).Append("' href='").Append(A(href)).Append("'><span class='am-ap-ico'>").Append(Svg(icon)).Append("</span><span><strong>")
         .Append(H(title)).Append("</strong><small>").Append(H(text)).Append("</small></span><span class='am-ap-chev' aria-hidden='true'>&rsaquo;</span></a>");
    }
    static string Svg(string n)
    {
        string d = n == "business" ? "M6 21V3h12v18M3 21h18M9 7h1m4 0h1M9 11h1m4 0h1M10 21v-5h4v5"
            : n == "people" ? "M9 11a3 3 0 100-6 3 3 0 000 6zM3 20c0-3 3-5 6-5s6 2 6 5M16 11a3 3 0 100-6M18 15c2 .5 3 2.4 3 5"
            : n == "clock" ? "M12 21a9 9 0 100-18 9 9 0 000 18zM12 7v5l3 2"
            : n == "reports" ? "M6 20V10M12 20V4M18 20v-7"
            : n == "doc" ? "M7 3h7l5 5v13H7zM14 3v5h5M10 13h6M10 17h6"
            : n == "bolt" ? "M13 2L4 14h7l-1 8 9-12h-7z"
            : "M3 12a9 9 0 103-6.7L3 8M3 3v5h5M12 7v5l3 2";
        return "<svg viewBox='0 0 24 24' width='22' height='22' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'><path d='" + d + "'/></svg>";
    }
}
