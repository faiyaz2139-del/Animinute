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
        if(String.Equals(roleName,"Manager",StringComparison.OrdinalIgnoreCase)) Add(items,"Calendar","Schedular.aspx",-208);
        if(HasAccount())
        {
            Add(items,"Dashboard","Dashboard.aspx",-201);
            Add(items,"Time Cards","TimeCards.aspx",-202);
            Add(items,"Notifications","Notifications.aspx",-203);
            if(AnyminuteDirectoryScope.CanReadDirectory())
            {Add(items,"Employees","Users.aspx",-204);Add(items,"Approvals","TimeCards.aspx?status=Submitted",-205);}
            bool owner=false;
            foreach(System.Data.DataRow row in AnyminuteModernScope.Businesses().Rows)
                if(AnyminuteModernScope.Owns(Convert.ToInt32(row["BusinessID"]))) {owner=true;break;}
            if(owner)
            {
                Add(items,"Payroll ready","PayrollReady.aspx",-206);Add(items,"Settings","BusinessSettings.aspx",-207);
                Add(items,"Edit Business","BusinessActions.aspx?action=edit",-210);
                Add(items,"Assign Accountant","BusinessActions.aspx?action=accountant",-211);
                Add(items,"Add User","EmployeeEditor.aspx",-212);
                Add(items,"Edit User","Users.aspx?action=edit",-213);
                Add(items,"Add Project","Add_Project.aspx?mode=add#add_project",-214);
                Add(items,"Edit Project","Add_Project.aspx?mode=edit#view_business_wise_project",-215);
            }
        }
        return items.OrderBy(item=>item.Seq);
    }
    static void Add(IList<MenuViewModel> items,string name,string href,int id)
    {
        if(!items.Any(item=>String.Equals(item.Href,href,StringComparison.OrdinalIgnoreCase)))
            items.Add(new MenuViewModel {Id=id,Seq=900-id,Name=name,Href=href,Icon="",SubMenu=new List<MenuViewModel>()});
    }
    private static IEnumerable<MenuViewModel> UiMenuGroups(IEnumerable<MenuViewModel> permitted)
    {
        var leaves = new List<MenuViewModel>();
        foreach (var item in permitted) CollectUiMenuItems(item, leaves);
        string[] names = { "Dashboard", "Businesses", "Employees", "Projects", "Time Cards", "Approvals", "Reports", "Settings", "Notifications", "Payroll", "Calendar", "Account" };
        var groups = new List<MenuViewModel>();
        for (int i = 0; i < names.Length; i++)
            groups.Add(new MenuViewModel { Id = -500-i, Seq = i, Name = names[i], Href = "#", Icon = "", SubMenu = new List<MenuViewModel>() });
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        MenuViewModel employeesFallback = null;
        foreach (var item in leaves)
        {
            string href = UiMenuHref(item), key = CanonicalRoute(href);
            if (!seen.Add(key)) continue;
            string route = href.ToLowerInvariant(), label = Convert.ToString(item.Name).ToLowerInvariant();
            // Account links have one home in the account menu, never a duplicate sidebar row.
            if (route.Contains("enterprise_plan") || route.Contains("change%20password") || route.Contains("change password") || label.Contains("plan upgrade")) continue;
            int group = 11;
            if (label.Contains("accountant") || route.Contains("businessactions")) group=1;
            else if (label.Contains("setting")) group=7;
            else if (route.Contains("notification")) group=8;
            else if (route.Contains("payroll")) group=9;
            else if (route.Contains("schedular") || route.Contains("scheduler")) group=10;
            else if (route.Contains("view=businesses")) group=1;
            else if (route.Contains("dashboard") || label == "home" || label == "dashboard" || route.EndsWith("home_page.aspx")) group=0;
            else if (route.Contains("report") || label.Contains("report")) group=6;
            else if (label.Contains("approv")) group=5;
            else if (route.Contains("time_sheet") || route.Contains("timecard") || label.Contains("time card")) group=4;
            else if (route.Contains("project")) group=3;
            else if (route.Contains("employee") || route.Contains("users.aspx") || label=="users" || label=="employees") group=2;
            else if (route.Contains("business") || route.Contains("bussiness")) group=1;
            // Canonical dashboard aliases all resolve to the same top-level item.
            if (group==0 && groups[group].SubMenu.Count>0) continue;
            // Employees submenu matches the design: exactly "Add User" and "Edit User".
            // Legacy employee pages stay reachable from the Employees list, they are just not repeated here.
            if (group==2)
            {
                string page = route.Split('?')[0];
                if (page.EndsWith("employeeeditor.aspx")) item.Name="Add User";
                else if (route.Contains("users.aspx?action=edit")) item.Name="Edit User";
                else { if (employeesFallback == null) employeesFallback = item; continue; }
            }
            if (group==3 && route.Contains("add_project.aspx?mode=add")) item.Name="Add Project";
            if (group==3 && route.Contains("add_project.aspx?mode=edit")) item.Name="Edit Project";
            groups[group].SubMenu.Add(item);
        }
        if (groups[2].SubMenu.Count == 0 && employeesFallback != null) { employeesFallback.Name = "Edit User"; groups[2].SubMenu.Add(employeesFallback); }
        groups[2].SubMenu = groups[2].SubMenu.OrderBy(m => m.Name == "Add User" ? 0 : 1).ToList();
        foreach (var group in groups.Where(g => g.SubMenu.Count > 0))
        {
            group.Href = group.SubMenu[0].Href;
            if (group.Name == "Dashboard" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Dashboard.aspx");
            if (group.Name == "Businesses" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Dashboard.aspx?view=businesses");
            if (group.Name == "Projects" && HasAccount()) group.Href = VirtualPathUtility.ToAbsolute("~/Projects.aspx");
            if (group.Name == "Employees" && HasAccount() && AnyminuteDirectoryScope.CanReadDirectory()) group.Href = VirtualPathUtility.ToAbsolute("~/Users.aspx");
        }
        return groups.Where(g => g.SubMenu.Count > 0);
    }

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
            return VirtualPathUtility.ToAbsolute("~/Add_Project.aspx")+"?mode=add#add_project";
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
    public static string AccountUpgrade(IEnumerable<MenuViewModel> permitted)
    {
        string href=PermittedHref(permitted,"Enterprise_Plan_Upgrade.aspx","Enterprise_Plan.aspx","Enterprise_Plan_Reseller.aspx");
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
