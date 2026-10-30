using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
// 2026-10-30 v33 (Shamsu deck "Accountant & Payroll Agency Dashboard"): accountant / payroll-agency logins get the deck
// dashboard (Select Business, 4 KPI cards, Assigned Businesses table, Quick Actions, Recent Activity) and the deck's flat
// navigation. Read-only: every figure comes from tables the app already has (no new SQL objects); every link is an existing page.
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

    static string ReportHref(IEnumerable<MenuViewModel> menu)
    {
        try { return AnyminuteReports.HubHref(menu ?? AnyminuteUiNavigation.Load()); } catch (HttpException) { return null; } catch (InvalidOperationException) { return null; }
    }
    static string DocumentsHref(IEnumerable<MenuViewModel> menu)
    {
        try { return AnyminuteUiNavigation.PermittedHref(menu ?? AnyminuteUiNavigation.Load(), "document.aspx", "document2.aspx", "document3.aspx"); } catch (HttpException) { return null; }
    }

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
        add("Documents", DocumentsHref(menu));
        add("Access History", U("~/Dashboard.aspx#am-ap-activity"));
        return l;
    }

    sealed class Biz { public int Id, Employees, Pending, Rejected; public string Name, Contact, Phone, Access; public DateTime? Last; }

    public static string Render(string selected)
    {
        var rows = AnyminuteModernScope.Businesses();
        var all = new List<Biz>();
        var activity = new List<object[]>();
        int me = Convert.ToInt32(HttpContext.Current.Session["UserId"]);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool weeks = AnyminuteTimesheetRepair.Query(c, null, "SELECT OBJECT_ID('dbo.AnyminuteWeek','U')").Rows[0][0] != DBNull.Value;
            bool invites = AnyminuteAccessService.Installed(c, null);
            foreach (DataRow r in rows.Rows)
            {
                var b = new Biz { Id = Convert.ToInt32(r["BusinessID"]), Name = Convert.ToString(r["CompanyName"]), Contact = Convert.ToString(r["PrimaryContactName"]), Access = "Accountant" };
                b.Employees = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=@b AND a.UserTypeId<>12", "@b", b.Id).Rows[0][0]);
                if (weeks)
                {
                    var w = AnyminuteTimesheetRepair.Query(c, null, @"SELECT SUM(CASE WHEN State='Submitted' THEN 1 ELSE 0 END),SUM(CASE WHEN State='Rejected' THEN 1 ELSE 0 END),
                        MAX(CASE WHEN State='Approved' THEN WeekStart END) FROM dbo.AnyminuteWeek WHERE BusinessID=@b", "@b", b.Id).Rows[0];
                    b.Pending = w[0] == DBNull.Value ? 0 : Convert.ToInt32(w[0]); b.Rejected = w[1] == DBNull.Value ? 0 : Convert.ToInt32(w[1]);
                    if (w[2] != DBNull.Value) b.Last = Convert.ToDateTime(w[2]);
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
            if (weeks && all.Count > 0)
            {
                string ids = String.Join(",", all.Select(b => b.Id.ToString(Inv)));
                foreach (DataRow a in AnyminuteTimesheetRepair.Query(c, null, @"SELECT TOP(4) w.BusinessID,w.State,w.WeekStart,w.ChangedAtUTC,dbo.FN_DESC('AddEmployee',w.EmployeeID)
                    FROM dbo.AnyminuteWeek w WHERE w.BusinessID IN (" + ids + ") AND w.State IN('Submitted','Approved','Rejected') AND w.ChangedAtUTC IS NOT NULL ORDER BY w.ChangedAtUTC DESC").Rows)
                    activity.Add(a.ItemArray);
            }
        }
        if (selected != null && !all.Any(b => b.Id.ToString(Inv) == selected)) selected = null;
        var scope = selected == null ? all : all.Where(b => b.Id.ToString(Inv) == selected).ToList();
        var menu = AnyminuteUiNavigation.Load().ToList();
        string report = ReportHref(menu), docs = DocumentsHref(menu);
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
        Kpi(s, "orange", "clock", "Pending Payroll Reviews", scope.Sum(b => b.Pending).ToString(Inv), U("~/TimeCards.aspx?status=Submitted"));
        Kpi(s, "purple", "reports", "Available Reports", reports.ToString(Inv), report ?? "#");
        s.Append("</div>");

        s.Append("<section class='am-card am-ap-biz'><div class='am-ap-sechead'><h2>").Append(Svg("business")).Append("Assigned Businesses</h2>")
         .Append("<label class='am-ap-search'><span class='sr-only'>Search businesses</span><input type='search' data-search='#am-ap-grid' placeholder='Search businesses...' /></label></div>")
         .Append("<div class='am-table-wrap'><table class='am-table' id='am-ap-grid'><thead><tr><th>Business Name</th><th>Contact</th><th>Access Type</th><th>Employees</th><th>Payroll Status</th><th>Last Payroll Period</th><th>Action</th></tr></thead><tbody>");
        foreach (var b in scope)
        {
            string status = b.Rejected > 0 ? "Action Required" : b.Pending > 0 ? "Pending Review" : "Up to Date";
            string cls = b.Rejected > 0 ? "red" : b.Pending > 0 ? "amber" : "green";
            // v34 product guidance: Open = working screen (approve queue for this business) when work is waiting;
            // View = read-only summary (this dashboard scoped to the business) when payroll is up to date.
            bool open = cls != "green";
            string href = open ? U("~/TimeCards.aspx?status=Submitted&BusinessId=" + b.Id) : U("~/Dashboard.aspx?b=" + b.Id);
            s.Append("<tr><td><a class='am-ap-name' href='").Append(A(U("~/Dashboard.aspx?b=" + b.Id))).Append("'>").Append(H(b.Name)).Append("</a></td>")
             .Append("<td><strong>").Append(H(b.Contact)).Append("</strong></td>")
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
        Quick(s, "orange", "doc", "Download Documents", "Access and download client documents", docs ?? report);
        s.Append("</div></section>");

        s.Append("<section class='am-card am-ap-activity' id='am-ap-activity'><div class='am-ap-sechead'><div><h2>").Append(Svg("history")).Append("Recent Activity</h2><p>Latest actions across your assigned businesses.</p></div><a href='")
         .Append(A(U("~/TimeCards.aspx"))).Append("'>View All Activity &rarr;</a></div><ul>");
        foreach (var a in activity)
        {
            int bid = Convert.ToInt32(a[0]); string state = Convert.ToString(a[1]); DateTime ws = Convert.ToDateTime(a[2]), at = Convert.ToDateTime(a[3]);
            string what = state == "Approved" ? "Payroll hours reviewed" : state == "Rejected" ? "Payroll hours returned" : "Payroll hours submitted";
            var biz = all.FirstOrDefault(b => b.Id == bid);
            s.Append("<li><span class='am-ap-ico am-ap-").Append(state == "Approved" ? "blue" : state == "Rejected" ? "orange" : "green").Append("'>").Append(Svg("clock")).Append("</span><div><strong>").Append(what)
             .Append("</strong><small>").Append(H((biz == null ? "" : biz.Name) + " \u2013 " + Convert.ToString(a[4]).Trim() + " \u2013 " + ws.ToString("MMM d", Inv) + " \u2013 " + ws.AddDays(6).ToString("MMM d, yyyy", Inv)))
             .Append("</small></div><time>").Append(H(Ago(at))).Append("</time></li>");
        }
        if (activity.Count == 0) s.Append("<li class='am-ap-empty'>No payroll activity yet.</li>");
        s.Append("</ul></section></div></div>");
        return s.ToString();
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
