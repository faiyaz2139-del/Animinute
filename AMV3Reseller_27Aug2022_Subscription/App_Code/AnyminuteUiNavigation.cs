using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
// Same permission-based renderer used by both shells. Page backends still authorize all actions.
public static class AnyminuteUiNavigation
{
    static bool HasAccount()
    {
        int user,employee;var h=HttpContext.Current;
        return h!=null && h.Session!=null && Int32.TryParse(Convert.ToString(h.Session["UserId"]),out user) && user>0 &&
            Int32.TryParse(Convert.ToString(h.Session["EmployeeId"]),out employee) && employee>0;
    }
    public static IEnumerable<MenuViewModel> Load()
    {
        var session=HttpContext.Current.Session;
        string roleName=Convert.ToString(session["UserType"]).Trim();int roleId;
        if(Int32.TryParse(Convert.ToString(session["UserTypeid"]),out roleId) && roleId>0)
        {
            using(var c=AnyminuteTimesheetRepair.Open())
            {
                var roles=AnyminuteTimesheetRepair.Query(c,null,"SELECT UserType FROM UserType WHERE User_TypeId=@r","@r",roleId);
                if(roles.Rows.Count==1 && !String.IsNullOrWhiteSpace(Convert.ToString(roles.Rows[0][0])))roleName=Convert.ToString(roles.Rows[0][0]).Trim();
            }
        }
        var items=new MenuHandler().GetMenusDetails(roleName).ToList();
        // 2026-10-25 v16: the Calendar group (Schedular.aspx "Time Sheet Schedule") is no longer in the left navigation;
        // the page still opens by URL, so the manager-only Calendar link is not added any more.
        if(HasAccount())
        {
            Add(items,"Dashboard","Dashboard.aspx",-201);
            Add(items,"Time Cards","TimeCards.aspx",-202);
            Add(items,"Notifications","Notifications.aspx",-203);
            // v16: Work Schedule (was "Weekly Schedule") for every signed-in account; employees see their own planned
            // schedule read-only, owners/managers edit it (WeeklyEmployeeSchedule.aspx authorizes every load/save).
            Add(items,"Work Schedule","WeeklyEmployeeSchedule.aspx",-216);
            // v17: employees enter their own Mon-Sun times in My Time Card (WeekTimeCard.aspx; no BusinessId -> first business).
            if(!AnyminuteDirectoryScope.CanReadDirectory())Add(items,"My Time Card","WeekTimeCard.aspx",-217);
            if(AnyminuteDirectoryScope.CanReadDirectory())
            {Add(items,"Employees","Users.aspx",-204);Add(items,"Approvals","TimeCards.aspx?status=Submitted",-205);}
            bool owner=false;
            foreach(System.Data.DataRow row in AnyminuteModernScope.Businesses().Rows)
                if(AnyminuteModernScope.Owns(Convert.ToInt32(row["BusinessID"]))) {owner=true;break;}
            if(owner)
            {
                Add(items,"Payroll ready","PayrollReady.aspx",-206);Add(items,"Settings","BusinessSettings.aspx",-207);
                Add(items,"Edit Business","BusinessActions.aspx?action=edit",-210);
                // v16: "Accountant & Payroll Access" is back in the menu, now under the User Access group (below Reports).
                Add(items,"Accountant & Payroll Access","AccountantAccess.aspx",-211);
                Add(items,"Add User","EmployeeEditor.aspx",-212);
                Add(items,"Edit User","Users.aspx?action=edit",-213);
                Add(items,"Add Project","ProjectEditor.aspx",-214); // v8 Deck F: new Add Project page
                Add(items,"Edit Project","Projects.aspx",-215); // v8 Deck F: edit from the Projects list
                // v11 Deck I Weekly Schedule is added above as "Work Schedule" (v16).
            }
        }
        return items.OrderBy(item=>item.Seq);
    }
    static void Add(IList<MenuViewModel> items,string name,string href,int id)
    {
        if(!items.Any(item=>String.Equals(item.Href,href,StringComparison.OrdinalIgnoreCase)))
            items.Add(new MenuViewModel {Id=id,Seq=900-id,Name=name,Href=href,Icon="",SubMenu=new List<MenuViewModel>()});
    }
    // 2026-10-24 v5 sidebar order: Dashboard, Quick Actions, Businesses, Employees, Projects, Time Cards, Approvals,
    // Reports, Settings, Notifications, Payroll, Calendar, Account, Need Help. "More tools" (the old catch-all) is gone:
    // Assign Accountant lives under Businesses, Assign User under Employees, tickets under Need Help.
    // Every link still comes from the role-permitted menu; only Change Password, Help Center and Chat are added,
    // and those were already offered to every signed-in user in the header menus.
    // 2026-10-25 v16: + Work Schedule (after Time Cards) and User Access (after Reports); Calendar and Account are not shown.
    const int GDashboard=0,GQuick=1,GBusinesses=2,GEmployees=3,GProjects=4,GTimeCards=5,GWorkSchedule=6,GApprovals=7,GReports=8,GUserAccess=9,GSettings=10,GNotifications=11,GPayroll=12,GCalendar=13,GAccount=14,GHelp=15;
    private static IEnumerable<MenuViewModel> UiMenuGroups(IEnumerable<MenuViewModel> permitted)
    {
        var all = permitted.ToList();
        var leaves = new List<MenuViewModel>();
        foreach (var item in all) CollectUiMenuItems(item, leaves);
        string[] names = { "Dashboard", "Quick Actions", "Businesses", "Employees", "Projects", "Time Cards", "Work Schedule", "Approvals", "Reports", "User Access", "Settings", "Notifications", "Payroll", "Calendar", "Account", "Need Help" };
        var groups = new List<MenuViewModel>();
        for (int i = 0; i < names.Length; i++)
            groups.Add(new MenuViewModel { Id = -500-i, Seq = i, Name = names[i], Href = "#", Icon = "", SubMenu = new List<MenuViewModel>() });
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var labels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        MenuViewModel employeesFallback = null;
        foreach (var item in leaves)
        {
            string href = UiMenuHref(item), key = CanonicalRoute(href);
            if (!seen.Add(key)) continue;
            string route = href.ToLowerInvariant(), label = Convert.ToString(item.Name).Trim().ToLowerInvariant();
            // Change Password and Plan Upgrade are added once, together, at the top of Account (below).
            if (route.Contains("enterprise_plan") || route.Contains("change%20password") || route.Contains("change password") || label.Contains("plan upgrade")) continue;
            int group = GAccount;
            // v16: Assign User is removed from the left navigation (the page still opens by URL).
            if (label == "assign user" || route.Contains("assignuser")) continue;
            if (label.Contains("accountant") || route.Contains("accountantaccess") || route.Contains("businessaccountant") || route.Contains("assignaccountant")) group=GUserAccess;
            else if (route.Contains("ticket") || label.Contains("ticket")) group=GHelp;
            else if (route.Contains("businessactions")) group=GBusinesses;
            else if (label.Contains("setting")) group=GSettings;
            else if (route.Contains("notification")) group=GNotifications;
            else if (route.Contains("payroll")) group=GPayroll;
            else if (route.Contains("weeklyemployeeschedule")) { group=GWorkSchedule; item.Name="Work Schedule"; }
            else if (route.Contains("schedular") || route.Contains("scheduler")) group=GCalendar; // v16: Time Sheet Schedule / Calendar, not shown
            else if (route.Contains("view=businesses")) group=GBusinesses;
            else if (route.Contains("dashboard") || label == "home" || label == "dashboard" || route.EndsWith("home_page.aspx")) group=GDashboard;
            else if (route.Contains("report") || label.Contains("report")) group=GReports;
            else if (label.Contains("approv")) group=GApprovals;
            else if (route.Contains("time_sheet") || route.Contains("timecard") || label.Contains("time card")) group=GTimeCards;
            else if (route.Contains("project")) group=GProjects;
            else if (route.Contains("employee") || route.Contains("users.aspx") || label=="users" || label=="employees" || label=="add user" || label=="edit user") group=GEmployees;
            else if (route.Contains("business") || route.Contains("bussiness")) group=GBusinesses;
            // One Dashboard only: every dashboard alias (Home, role dashboards) collapses into the single top item.
            if (group==GDashboard && groups[group].SubMenu.Count>0) continue;
            if (group==GEmployees)
            {
                string page = route.Split('?')[0];
                if (page.EndsWith("employeeeditor.aspx") || page.EndsWith("add_employeev2.aspx")) { item.Name="Add Employee"; item.Href="EmployeeEditor.aspx"; }
                else if (route.Contains("users.aspx?action=edit") || page.EndsWith("addeditrole_add_employee.aspx")) item.Name="Edit Employee";
                else if (label != "assign user") { if (employeesFallback == null) employeesFallback = item; continue; }
            }
            // v8 Deck F: legacy Add_Project routes open the new Projects UI.
            if (group==GProjects && (route.Contains("add_project.aspx?mode=add") || route.Split('?')[0].EndsWith("projecteditor.aspx"))) { item.Name="Add Project"; item.Href="ProjectEditor.aspx"; }
            else if (group==GProjects && route.Contains("add_project.aspx?mode=edit")) { item.Name="Edit Project"; item.Href="Projects.aspx"; }
            else if (group==GProjects && route.Split('?')[0].EndsWith("add_project.aspx")) { item.Name="Add Project"; item.Href="ProjectEditor.aspx"; }
            // v9 Deck G: every Assign Accountant / legacy accountant route opens the new Accountant & Payroll Access list.
            // v16: it lives under User Access (not Businesses).
            if (group==GUserAccess) { item.Name="Accountant & Payroll Access"; item.Href="AccountantAccess.aspx"; }
            // Same label twice in one menu (e.g. two Assign Accountant routes, two Dashboards) is shown once.
            if (!labels.Add(group + "|" + item.Name.Trim())) continue;
            groups[group].SubMenu.Add(item);
        }
        if (!groups[GEmployees].SubMenu.Any(m => m.Name == "Edit Employee") && employeesFallback != null) { employeesFallback.Name = "Edit Employee"; groups[GEmployees].SubMenu.Add(employeesFallback); }
        // v16: Edit Business always sits under Businesses next to Add Business. Owners get the Edit Business picker
        // (BusinessActions.aspx?action=edit); otherwise it opens the Businesses list, whose rows carry the per-business Edit.
        if (groups[GBusinesses].SubMenu.Any(m => (m.Name ?? "").Trim() == "Add Business") && !groups[GBusinesses].SubMenu.Any(m => (m.Name ?? "").Trim() == "Edit Business"))
            groups[GBusinesses].SubMenu.Add(Link(-631, "Edit Business", VirtualPathUtility.ToAbsolute("~/Dashboard.aspx?view=businesses&mode=edit")));
        Func<string,string[],int> rank = (n, order) => { int i = Array.IndexOf(order, n); return i < 0 ? order.Length : i; };
        groups[GBusinesses].SubMenu = groups[GBusinesses].SubMenu.OrderBy(m => rank(m.Name, new[] { "Add Business", "Edit Business", "Accountant & Payroll Access" })).ToList();
        groups[GEmployees].SubMenu = groups[GEmployees].SubMenu.OrderBy(m => rank(m.Name, new[] { "Add Employee", "Edit Employee", "Assign User" })).ToList();
        groups[GProjects].SubMenu = groups[GProjects].SubMenu.OrderBy(m => rank(m.Name, new[] { "Add Project", "Edit Project" })).ToList();
        // Account: Change Password, then Plan Upgrade directly below it (only where the role already has an upgrade page).
        var account = new List<MenuViewModel> { Link(-601, "Change Password", VirtualPathUtility.ToAbsolute("~/Change%20Password.aspx")) };
        string upgrade = UpgradeHref(all);
        if (upgrade != null) account.Add(Link(-602, "Plan Upgrade", upgrade));
        account.AddRange(groups[GAccount].SubMenu); groups[GAccount].SubMenu = account;
        // Need Help: Help Center, the role's ticket pages, then Chat (the existing HelpChat.aspx page).
        var help = new List<MenuViewModel> { Link(-611, "Help Center", VirtualPathUtility.ToAbsolute("~/Help.aspx")) };
        help.AddRange(groups[GHelp].SubMenu);
        help.Add(Link(-612, "Chat", VirtualPathUtility.ToAbsolute("~/HelpChat.aspx")));
        groups[GHelp].SubMenu = help;
        // Quick Actions: shortcuts to links this role already has.
        var quick = new List<MenuViewModel>();
        foreach (var pick in new[] { new object[] { GBusinesses, "Add Business" }, new object[] { GEmployees, "Add Employee" }, new object[] { GProjects, "Add Project" }, new object[] { GApprovals, null } })
        {
            var source = groups[(int)pick[0]].SubMenu.FirstOrDefault(m => pick[1] == null || m.Name == (string)pick[1]);
            if (source != null) quick.Add(Link(-620 - quick.Count, pick[1] == null ? "View Approvals" : (string)pick[1], UiMenuHref(source)));
        }
        groups[GQuick].SubMenu = quick;
        foreach (var group in groups.Where(g => g.SubMenu.Count > 0))
        {
            if (group.Seq == GQuick || group.Seq == GAccount || group.Seq == GHelp || group.Seq == GUserAccess) continue; // pure sections: the header only opens/closes
            group.Href = group.SubMenu[0].Href;
            // Without a landing page, Businesses/Employees/Projects open as sections so Add X stays visible as a child link.
            if ((group.Seq == GBusinesses || group.Seq == GEmployees || group.Seq == GProjects) && group.SubMenu.Count > 1) group.Href = "#";
            if (group.Name == "Dashboard" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Dashboard.aspx");
            if (group.Name == "Businesses" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Dashboard.aspx?view=businesses");
            if (group.Name == "Projects" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Projects.aspx");
            if (group.Name == "Employees" && HasAccount() && AnyminuteDirectoryScope.CanReadDirectory()) group.Href = VirtualPathUtility.ToAbsolute("~/Users.aspx");
        }
        // 2026-10-24 v10: Quick Actions, Settings, Payroll and Need Help are no longer sidebar groups
        // (help, tickets and chat stay in the header Help menu; quick actions are on the Dashboard).
        // 2026-10-25 v15 (t52u): the Account group and all its links (Change Password, Plan Upgrade, ...) are removed from the
        // left navigation; Change password / Plan Upgrade stay in the header profile menu and every page still opens by URL.
        // 2026-10-25 v16: the Calendar group (Calendar / Time Sheet Schedule) is removed as well; Account stays removed for every role.
        return groups.Where(g => g.SubMenu.Count > 0 && g.Seq != GQuick && g.Seq != GSettings && g.Seq != GPayroll && g.Seq != GHelp && g.Seq != GAccount && g.Seq != GCalendar);
    }
    static MenuViewModel Link(int id, string name, string href) { return new MenuViewModel { Id = id, Seq = 0, Name = name, Href = href, Icon = "", SubMenu = new List<MenuViewModel>() }; }

    // Compare resolved destinations, not labels or relative/rooted spelling.
    private static string CanonicalRoute(string href)
    {
        string route = (href ?? "#").Trim();
        int fragment = route.IndexOf('#');
        if(fragment >= 0) route = route.Substring(0,fragment);
        if(route.StartsWith("~/",StringComparison.Ordinal)) route = route.Substring(2);
        string app = HttpRuntime.AppDomainAppVirtualPath ?? "/";
        if(app != "/" && route.StartsWith(app+"/",StringComparison.OrdinalIgnoreCase)) route=route.Substring(app.Length+1);
        return route.TrimStart('/').ToLowerInvariant();
    }

    private static void CollectUiMenuItems(MenuViewModel item, IList<MenuViewModel> leaves)
    {
        if (!String.IsNullOrWhiteSpace(item.Href) && item.Href != "#") leaves.Add(item);
        if (item.SubMenu != null) foreach (var child in item.SubMenu) CollectUiMenuItems(child, leaves);
    }

    private static string UiMenuHref(MenuViewModel item)
    {
        string label = Convert.ToString(item.Name).Trim();
        string href = item.Href ?? "#";
        string page = System.IO.Path.GetFileName(href.Split('?')[0]).ToLowerInvariant();
        if(page=="dashboard.aspx" && href.IndexOf("view=businesses",StringComparison.OrdinalIgnoreCase)>=0)return href;
        if (HasAccount() && (page=="dashboard.aspx" || page=="home_page.aspx" || page=="owner_dashboard.aspx" || page=="employee_dashboard.aspx" || page=="manager_dashboard.aspx"))
            return VirtualPathUtility.ToAbsolute("~/Dashboard.aspx");
        if(page=="add_project.aspx" && href.IndexOf('?')<0 && href.IndexOf('#')<0)
            return VirtualPathUtility.ToAbsolute("~/ProjectEditor.aspx");
        if (item.Id < 0) return href;
        if (label.Equals("Employees", StringComparison.OrdinalIgnoreCase) ||
            label.Equals("Users", StringComparison.OrdinalIgnoreCase) ||
            label.Equals("Employee List", StringComparison.OrdinalIgnoreCase) ||
            label.Equals("User List", StringComparison.OrdinalIgnoreCase))
        {
            // The destination rechecks persisted account/business roles on every request.
            return VirtualPathUtility.ToAbsolute("~/Users.aspx");
        }
        if (HasAccount() && System.IO.Path.GetFileName(href.Split('?')[0]).Equals("Home_Page.aspx",StringComparison.OrdinalIgnoreCase))
            return VirtualPathUtility.ToAbsolute("~/Dashboard.aspx");
        return href;
    }

    public static string Render(IEnumerable<MenuViewModel> menu)
    {
        var html = new StringBuilder();
        foreach (var item in UiMenuGroups(menu))
        {
            var childrenMenu = item.SubMenu.Where(child => CanonicalRoute(UiMenuHref(child)) != CanonicalRoute(UiMenuHref(item))).ToList();
            if (item.Name == "Dashboard") childrenMenu.Clear();
            bool children = childrenMenu.Count > 0;
            html.Append("<li" + (children ? " class='am-nav-parent'" : "") + "><a href='")
                .Append(HttpUtility.HtmlAttributeEncode(UiMenuHref(item))).Append("'")
                .Append(children ? " aria-expanded='false'" : "").Append(item.Href != "#" ? " data-am-primary='true'" : "").Append("><i class='")
                .Append(HttpUtility.HtmlAttributeEncode(item.Icon ?? "")).Append("' aria-hidden='true'></i><span>")
                .Append(HttpUtility.HtmlEncode(item.Name)).Append("</span></a>");
            if (children)
            {
                html.Append("<button type='button' class='am-nav-expand' aria-expanded='false' aria-label='Expand ")
                    .Append(HttpUtility.HtmlAttributeEncode(item.Name)).Append("'><span aria-hidden='true' class='am-nav-chevron'></span></button>");
                html.Append(BindSubMenu(childrenMenu));
            }
            html.Append("</li>");
        }
        return html.ToString();
    }

    public static string PermittedHref(IEnumerable<MenuViewModel> permitted, params string[] pages)
    {
        var leaves=new List<MenuViewModel>();
        foreach(var item in permitted) CollectUiMenuItems(item,leaves);
        foreach(var item in leaves)
        {
            string href=item.Href ?? "";
            if(href.IndexOf(':')>=0 || href.StartsWith("//",StringComparison.Ordinal))continue;
            string name=System.IO.Path.GetFileName(href.Split('?')[0]);
            if(pages.Any(page=>String.Equals(page,name,StringComparison.OrdinalIgnoreCase)))
                return href.StartsWith("~/",StringComparison.Ordinal)?VirtualPathUtility.ToAbsolute(href):href;
        }
        return null;
    }
    public static string ReportHref(IEnumerable<MenuViewModel> permitted)
    {
        var leaves=new List<MenuViewModel>();
        foreach(var item in permitted)CollectUiMenuItems(item,leaves);
        var reports=leaves.Where(item=>(item.Href ?? "").IndexOf("report",StringComparison.OrdinalIgnoreCase)>=0).ToList();
        return PermittedHref(reports,reports.Select(item=>System.IO.Path.GetFileName(item.Href.Split('?')[0])).ToArray());
    }
    public static string HeaderHelp(IEnumerable<MenuViewModel> permitted)
    {
        var items=permitted.ToList();var html=new StringBuilder();
        html.Append("<details class='am-header-help'><summary aria-label='Help and support'><span data-am-icon='help' aria-hidden='true'></span></summary><div class='am-account-popover'><strong>Need help using AnyMinute?</strong>");
        HeaderLink(html,VirtualPathUtility.ToAbsolute("~/Help.aspx"),"Help Center","help");
        string tickets=PermittedHref(items,"Ticket_List.aspx","Raise_Ticket_By_User.aspx");

        if(tickets!=null)HeaderLink(html,tickets,"Tickets","ticket");
        HeaderLink(html,VirtualPathUtility.ToAbsolute("~/HelpChat.aspx"),"Chat","message");
        string suggestion=PermittedHref(items,"Raise_Ticket_By_User.aspx");
        if(suggestion!=null)HeaderLink(html,suggestion,"Share suggestion","message");
        html.Append("</div></details>");return html.ToString();
    }
    // 2026-10-24 v10: one Plan Upgrade link for the sidebar Account group and the header profile menu.
    // Uses the role's permitted upgrade page; subscribers/owners whose menu lacks it get Enterprise_Plan_Upgrade.aspx.
    public static string UpgradeHref(IEnumerable<MenuViewModel> permitted)
    {
        string href=PermittedHref(permitted,"Enterprise_Plan_Upgrade.aspx","Enterprise_Plan.aspx","Enterprise_Plan_Reseller.aspx");
        if(href!=null)return href;
        var session=HttpContext.Current==null?null:HttpContext.Current.Session;
        if(session==null||session["EmployeeId"]==null)return null;
        string typeId=Convert.ToString(session["UserTypeid"]).Trim(),type=Convert.ToString(session["UserType"]);
        bool owner=typeId=="1"||type.IndexOf("Owner",StringComparison.OrdinalIgnoreCase)>=0||type.IndexOf("Subscriber",StringComparison.OrdinalIgnoreCase)>=0;
        return owner?VirtualPathUtility.ToAbsolute("~/Enterprise_Plan_Upgrade.aspx"):null;
    }
    public static string AccountUpgrade(IEnumerable<MenuViewModel> permitted)
    {
        string href=UpgradeHref(permitted);
        if(href==null)return "";
        var html=new StringBuilder();HeaderLink(html,href,"Plan Upgrade","upgrade");return html.ToString();
    }

    private static void HeaderLink(StringBuilder html,string href,string label,string icon)
    {
        html.Append("<a href='").Append(HttpUtility.HtmlAttributeEncode(href)).Append("'><span data-am-icon='")
            .Append(icon).Append("' aria-hidden='true'></span>").Append(HttpUtility.HtmlEncode(label)).Append("</a>");
    }

    private static string BindSubMenu(IEnumerable<MenuViewModel> menu)
    {
        var html = new StringBuilder("<ul class='am-nav-children'>");
        foreach (var item in menu)
            html.Append("<li><a href='").Append(HttpUtility.HtmlAttributeEncode(UiMenuHref(item)))
                .Append("'><i class='").Append(HttpUtility.HtmlAttributeEncode(item.Icon ?? ""))
                .Append("' aria-hidden='true'></i><span>").Append(HttpUtility.HtmlEncode(item.Name)).Append("</span></a></li>");
        return html.Append("</ul>").ToString();
    }
}
