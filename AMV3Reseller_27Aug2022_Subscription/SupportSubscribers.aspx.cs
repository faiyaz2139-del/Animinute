using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-28 v25 deck 4 (Subscriber Login & Access): subscriber list, Manage Subscriber Login & Access
// (current login info, Enable/Disable with mandatory reason, Resend Invitation, Send Password Reset, View Login History),
// the Send Password Reset confirm page and Login Activity History. Login access uses the existing
// AnyminuteLoginSecurity.LoginDisabled flag (REQUIRED_01, already honoured by Login.aspx); history and the
// sign-in / failed sign-in events (Login.aspx hook) are stored in AnyminuteSupportAudit (REQUIRED_07).
// 2026-10-28 v26 strict pass: Businesses / Employees / Projects / Subscription & Billing tabs (read-only, same owner scoping as
// AnyminuteModernScope), Active Sessions + session revocation on Disable (AnyminuteSessionGuard, REQUIRED_08), 24-hour one-time
// reset links (AnyminutePasswordReset), and IP location in Login Activity History (AnyminuteGeoIp, optional GeoIpLookupUrl).
// 2026-11-01 v42 (PPT 01 "20261027A" s1-s4, s6; PPT 03 s2 "This is not what I send"): Subscriber Management list (KPIs, search /
// Plan / Status filters, deck columns, status toggle, Subscriber Actions menu, "Showing x to y of z subscribers" + per page),
// Subscriber Details as the default subscriber screen (header, Subscription Limits, Edit / Send Password Reset / More, deck tabs,
// Businesses tab columns) and Edit Subscriber with the Confirm Changes + Reason for Change step. Login & Access moved to view=access.
// The login email stays read-only here (v39: changes go through Change Login Email verification).
public partial class AnyminuteSupportSubscribersPage : System.Web.UI.Page
{
    const int PageSize = 10;
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    static string Sub(int user) { return "SUB-" + user.ToString("000", CultureInfo.InvariantCulture); }
    static string Initials(string name) { var p = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); return p.Length == 0 ? "?" : (p[0].Substring(0, 1) + (p.Length > 1 ? p[p.Length - 1].Substring(0, 1) : "")).ToUpperInvariant(); }
    DataTable Subscribers(SqlConnection c, int user)
    {
        bool sec = SA.Has(c, "AnyminuteLoginSecurity"), last = sec && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteLoginSecurity", "LastLoginDate");
        return SA.Q(c, @"SELECT u.UserId,u.EmployeeId,u.Email,u.Status,u.Is_First_Time_Login,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.MiddleName,'')+' '+ISNULL(ae.LastName,''))) Name,ae.MobilePhone,
            pl.PlanName,pl.Id PlanRowId,pl.StartDate PlanStart,pl.EndDate NextBilling,pl.Business PlanBiz,pl.No_Of_Users PlanUsers,ae.FirstName,ae.MiddleName,ae.LastName,ea.MainPhone,
            LTRIM(RTRIM(ISNULL(ea.NumberSuffix,'')+' '+ISNULL(ea.StreetName,'')+' '+ISNULL(ea.StreetType,''))) Street,COALESCE(ci.CityName,ea.City) City,COALESCE(pr.ProvinceName,ea.Province) Province,ea.Postalcode,
            " + AnyminuteResellers.OwnedBiz + @" Businesses,(SELECT COUNT(DISTINCT a2.EmployeeID) FROM AssginBussiness a2 WHERE a2.EmployeeID<>u.EmployeeId AND a2.BusinessID IN (SELECT a.BusinessID FROM AssginBussiness a WHERE a.EmployeeID=u.EmployeeId AND a.UserTypeId=1)) Users,
            (SELECT MIN(p.StartDate) FROM PlanHistory p WHERE p.EmailId=u.Email) Since," +
            (sec ? "ISNULL(s.LoginDisabled,0) LoginDisabled," : "CAST(0 AS bit) LoginDisabled,") + (last ? "s.LastLoginDate" : "CAST(NULL AS datetime2) LastLoginDate") +
            " FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId LEFT JOIN EmpAddress ea ON ea.EmpAddressId=ae.ADDRESSID" +
            " LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=ea.City LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=ea.Province" +
            " OUTER APPLY (SELECT TOP 1 p.Id,p.PlanName,p.StartDate,p.EndDate,p.Business,p.No_Of_Users FROM PlanHistory p WHERE p.EmailId=u.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) pl" + (sec ? " LEFT JOIN AnyminuteLoginSecurity s ON s.UserId=u.UserId" : "") +
            " WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)" + (user > 0 ? " AND u.UserId=@u" : "") + " ORDER BY u.UserId DESC", "@u", user);
    }
    static bool Enabled(DataRow r) { return !Convert.ToBoolean(r["LoginDisabled"]) && (r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"])); }
    static string StatusOf(DataRow r) { return !Enabled(r) ? "Inactive" : r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]) ? "Pending" : "Active"; }
    static string Tone(string st) { return st == "Active" ? "green" : st == "Pending" ? "amber" : "red"; }
    static string Name(DataRow r) { string n = SA.S(r["Name"]).Replace("  ", " "); return n.Length > 0 ? n : SA.S(r["Email"]); }
    protected void Page_Load(object sender, EventArgs e)
    {
        int user; Int32.TryParse(SA.Get("user"), out user);
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(user); return; }
        Flash.Text = SA.TakeFlash();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteSupportAudit") && SA.Has(c, "AnyminuteLoginSecurity");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteSupportAudit", "AnyminuteLoginSecurity");
            if (user <= 0) { List(c); return; }
            var t = Subscribers(c, user);
            if (t.Rows.Count != 1) { Title1.Text = "Subscriber not found"; Content.Text = "<p><a class='sa-link' href='SupportSubscribers.aspx'>Back to Subscribers</a></p>"; return; }
            var r = t.Rows[0]; string view = SA.Get("view");
            string crumb = view == "history" ? "Login Activity History" : view == "reset" ? "Send Password Reset" : view == "edit" ? "Edit Subscriber" : view == "access" ? "Login &amp; Access" : "";
            Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; " + (crumb.Length == 0 ? "<span>Subscriber Details</span>" : "<a href='SupportSubscribers.aspx?user=" + user + "'>Subscriber Details</a> &rsaquo; <span>" + crumb + "</span>") + "</nav>";
            if (view == "" || view == "businesses" || view == "employees" || view == "projects" || view == "billing") { Title1.Text = "Subscriber Details"; Lead.Text = ""; Content.Text = Details(c, r) + DTabs(c, r, view.Length == 0 ? "businesses" : view) + (view == "billing" ? Billing(c, r) : view == "employees" || view == "projects" ? Related(c, r, view) : Biz(c, r)); }
            else if (view == "edit") { Title1.Text = "Edit Subscriber"; Lead.Text = "Update subscriber profile, contact, plan limits and account status."; Content.Text = Edit(c, r, null); }
            else if (view == "history") { Title1.Text = "Subscriber Login Activity History"; Lead.Text = "View sign-in activity, account changes and security events for this subscriber."; Content.Text = Header(c, r) + Tabs(c, r, "history") + History(c, user); }
            else if (view == "reset") { Title1.Text = "Send Password Reset"; Lead.Text = "Send a secure password reset link to the subscriber."; Content.Text = Reset(r, ready); }
            else { Title1.Text = "Manage Subscriber Login &amp; Access"; Lead.Text = "Subscriber Details &rsaquo; Login &amp; Access"; Content.Text = Header(c, r) + Tabs(c, r, "access") + Access(c, r, ready); }
        }
    }
    static readonly int[] Sizes = { 10, 20, 50 };
    static string Menu(int u)
    {
        string b = "SupportSubscribers.aspx?user=" + u;
        Func<string, string, string, string, string> a = (href, ico, t, sm) => "<a class='sa-menu-item' href='" + href + "'><i aria-hidden='true'>" + ico + "</i><span><b>" + t + "</b><small>" + sm + "</small></span></a>";
        // v42 PPT 01 s2 "Recommended three-dot Actions menu" (labels / descriptions as on the slide)
        return a(b, "&#128065;", "View Subscriber Details", "Profile, businesses, employees and projects") + a(b + "&amp;view=edit", "&#9998;", "Edit Subscriber", "Update personal and contact information")
            + a(b + "&amp;view=access", "&#128274;", "Manage Login &amp; Access", "Enable or disable access, send reset link") + a(b + "&amp;view=edit#plan", "&#9636;", "Change Subscription Plan", "Authorized plan upgrade or downgrade")
            + a(b + "&amp;view=billing", "&#128200;", "View Usage &amp; Limits", "Business, employee and project allowances") + a(b + "&amp;view=history", "&#8634;", "View Activity Log", "Account changes and support history")
            + a(b + "&amp;view=access", "&#10006;", "Suspend Subscriber", "Restricted action requiring confirmation");
    }
    void List(SqlConnection c)
    {
        Title1.Text = "Subscriber Management"; Lead.Text = "Create subscribers, activate accounts, manage businesses, employees and projects.";
        string q = SA.Get("q"), plan = SA.Get("plan"), st = SA.Get("status"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        int size; if (!Int32.TryParse(SA.Get("size"), out size) || !Sizes.Contains(size)) size = PageSize;
        var all = Subscribers(c, 0).Rows.Cast<DataRow>().ToList();
        var rows = all.Where(r => (q.Length == 0 || (Name(r) + " " + SA.S(r["Email"]) + " " + SA.S(r["MobilePhone"]) + " " + SA.S(r["MainPhone"])).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
            && (plan.Length == 0 || SA.S(r["PlanName"]) == plan) && (st.Length == 0 || StatusOf(r) == st)).ToList();
        int pages = Math.Max(1, (rows.Count + size - 1) / size); if (page > pages) page = pages;
        Func<string, string, string, int, string> kpi = (icon, label, tone, cnt) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + icon + "</i><div><span>" + label + "</span><strong>" + cnt + "</strong></div></div>";
        var sb = new StringBuilder("<div class='sa-kpis'>").Append(kpi("&#128101;", "Total Subscribers", "blue", all.Count)).Append(kpi("&#10004;", "Active Subscribers", "green", all.Count(r => StatusOf(r) == "Active")))
            .Append(kpi("&#10006;", "Inactive Subscribers", "red", all.Count(r => StatusOf(r) == "Inactive"))).Append(kpi("&#9203;", "Trial Subscribers", "purple", all.Count(r => SA.S(r["PlanName"]).IndexOf("TRIAL", StringComparison.OrdinalIgnoreCase) >= 0))).Append("</div>");
        sb.Append("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><input type='hidden' name='size' value='").Append(size).Append("' /><label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>&#128269;</i><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by name, email or phone number...' /></label>")
          .Append("<label><span>Plan</span><select name='plan'>").Append(SA.Options(all.Select(r => SA.S(r["PlanName"])).Where(x => x.Length > 0).Distinct().OrderBy(x => x), plan, "All Plans")).Append("</select></label>")
          .Append("<label><span>Status</span><select name='status'>").Append(SA.Options(new[] { "Active", "Inactive", "Pending" }, st, "All Statuses")).Append("</select></label>")
          .Append("<a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx'>Clear</a><button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table sa-sub-list'><thead><tr><th><input type='checkbox' aria-label='Select all' data-sa-all='sa_sel' /></th><th>Sl. No.</th><th>Subscriber Name</th><th>Plan Name</th><th>No. of Businesses</th><th>No. of Users</th><th>Contact Number</th><th>Email ID</th><th>Activation Date</th><th>Status</th><th>Actions</th></tr></thead><tbody>");
        int n = (page - 1) * size;
        foreach (var r in rows.Skip((page - 1) * size).Take(size))
        {
            int u = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + u, sts = StatusOf(r), phone = SA.S(r["MainPhone"]).Length > 0 ? SA.S(r["MainPhone"]) : SA.S(r["MobilePhone"]);
            sb.Append("<tr><td><input type='checkbox' name='sa_sel' value='").Append(u).Append("' aria-label='Select' /></td><td>").Append(++n).Append("</td><td><a class='sa-link' href='").Append(b).Append("'><strong>").Append(SA.H(Name(r))).Append("</strong></a><br /><a class='sa-link sa-small' href='").Append(b).Append("&amp;view=reset'>Change Password</a></td><td>")
              .Append(SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;").Append("</td><td><a class='sa-link' href='").Append(b).Append("&amp;view=businesses'>&#127970; ").Append(r["Businesses"]).Append("</a></td><td><a class='sa-link' href='").Append(b).Append("&amp;view=employees'>").Append(r["Users"])
              .Append("</a></td><td>").Append(phone.Length > 0 ? SA.H(phone) : "&mdash;").Append("</td><td>").Append(SA.H(r["Email"])).Append("</td><td>").Append(SA.Day(r["Since"])).Append("</td><td class='sa-nowrap'><a class='sa-switch").Append(sts == "Inactive" ? "" : " on").Append("' href='").Append(b).Append("&amp;view=access' title='Manage Login &amp; Access' aria-label='Status: ").Append(sts).Append("'><i></i></a> ").Append(SA.Pill(sts, Tone(sts)))
              .Append("</td><td><details class='sa-menu'><summary aria-label='Subscriber actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Subscriber Actions</h4><small>").Append(SA.H(Name(r))).Append(" &middot; ").Append(sts).Append("</small>").Append(Menu(u)).Append("</div></details></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='11' class='sa-empty'>No subscribers found.</td></tr>");
        string bq = "SupportSubscribers.aspx?q=" + HttpUtility.UrlEncode(q) + "&plan=" + HttpUtility.UrlEncode(plan) + "&status=" + HttpUtility.UrlEncode(st);
        Func<int, string> pg = i => SA.A(bq + "&size=" + size + "&page=" + i);
        sb.Append("</tbody></table></div><div class='sa-pager sa-rsl-pager'><span>Showing ").Append(rows.Count == 0 ? 0 : (page - 1) * size + 1).Append(" to ").Append(Math.Min(rows.Count, page * size)).Append(" of ").Append(rows.Count).Append(" subscriber").Append(rows.Count == 1 ? "" : "s").Append("</span>")
          .Append("<label class='sa-rsl-size'><select onchange=\"location.href='").Append(SA.A(bq)).Append("&size='+this.value\">");
        foreach (int z in Sizes) sb.Append("<option value='").Append(z).Append("'").Append(z == size ? " selected" : "").Append(">").Append(z).Append(" per page</option>");
        sb.Append("</select></label><nav aria-label='Pages'>").Append(page > 1 ? "<a href='" + pg(page - 1) + "' aria-label='Previous page'>&lsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&lsaquo;</span>");
        for (int i = Math.Max(1, page - 4); i <= Math.Min(pages, Math.Max(1, page - 4) + 9); i++) sb.Append("<a href='").Append(pg(i)).Append("'").Append(i == page ? " class='on' aria-current='page'" : "").Append(">").Append(i).Append("</a>");
        sb.Append(page < pages ? "<a href='" + pg(page + 1) + "' aria-label='Next page'>&rsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&rsaquo;</span>").Append("</nav></div></div>");
        Content.Text = sb.ToString();
    }
    // v42 PPT 01 s3/s4 Subscriber Details header + Subscription Limits.
    static string Details(SqlConnection c, DataRow r)
    {
        int u = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + u, sts = StatusOf(r), plan = SA.S(r["PlanName"]);
        int[] n; try { n = Counts(c, r); } catch (SqlException) { n = new[] { 0, 0, 0 }; }
        string projLimit = "";
        if (plan.Length > 0 && SA.Has(c, "AnyminuteSubscriptionPlan"))
            try { var p = SA.Q(c, "SELECT TOP 1 MaxProjects FROM AnyminuteSubscriptionPlan WHERE " + String.Format(SA.PlanKeySql, "PlanName") + "=@k", "@k", SA.PlanKey(plan)); if (p.Rows.Count == 1) projLimit = SA.S(p.Rows[0][0]); } catch (SqlException) { }
        Func<string, string, string> kv = (k, v) => "<div><small>" + k + "</small><strong>" + (v.Length == 0 ? "&mdash;" : v) + "</strong></div>";
        Func<string, int, string, string> bar = (label, used, lim) =>
        {
            int l; bool has = Int32.TryParse(lim, out l) && l > 0; int pct = has ? Math.Min(100, used * 100 / l) : 0;
            return "<div class='sa-sd-lim'><span>" + label + "</span><strong>" + used + " / " + (lim.Length > 0 ? SA.H(lim) : "&mdash;") + "</strong><span class='sa-bar'><i style='width:" + pct + "%'></i></span></div>";
        };
        string addr = String.Join(", ", new[] { SA.S(r["Street"]), SA.S(r["City"]), (SA.S(r["Province"]) + " " + SA.S(r["Postalcode"])).Trim() }.Where(x => x.Length > 0).ToArray());
        string phone = SA.S(r["MainPhone"]).Length > 0 ? SA.S(r["MainPhone"]) : SA.S(r["MobilePhone"]);
        return "<div class='sa-card sa-sd'><div class='sa-sd-top'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><h2>" + SA.H(Name(r)) + " " + SA.Pill(sts, Tone(sts)) + "</h2><small>" + (plan.Length > 0 ? SA.H(plan) : "No active plan") + " &middot; Subscriber since " + SA.Day(r["Since"]) + "</small></div>"
            + "<div class='sa-sd-btns'><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=edit'>&#9998; Edit</a><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=reset'>&#9993; Send Password Reset</a>"
            + "<details class='sa-menu'><summary class='sa-btn sa-btn-outline'>More &#9662;</summary><div class='sa-menu-pop'><h4>Subscriber Actions</h4><small>" + SA.H(Name(r)) + "</small>" + Menu(u) + "</div></details></div></div>"
            + "<div class='sa-sd-body'><div class='sa-sd-info'>" + kv("Email", SA.H(r["Email"])) + kv("Phone", SA.H(phone)) + kv("Address", SA.H(addr)) + kv("Activation Date", SA.S(r["Since"]).Length > 0 ? SA.Day(r["Since"]) : "") + kv("Next Billing Date", SA.S(r["NextBilling"]).Length > 0 ? SA.Day(r["NextBilling"]) : "") + "</div>"
            + "<div class='sa-sd-limits'><h3>Subscription Limits" + (plan.Length > 0 ? " (" + SA.H(plan) + ")" : "") + "</h3>" + bar("Businesses", n[0], SA.S(r["PlanBiz"])) + bar("Employees", n[1], SA.S(r["PlanUsers"])) + bar("Projects", n[2], projLimit)
            + "<a class='sa-link sa-small' href='" + b + "&amp;view=edit#plan'>Change Subscription Plan</a></div></div></div>";
    }
    static string DTabs(SqlConnection c, DataRow r, string on)
    {
        int user = Convert.ToInt32(r["UserId"]); int[] n; try { n = Counts(c, r); } catch (SqlException) { n = null; }
        Func<string, string, string, string> t = (key, label, href) => "<a href='" + href + "'" + (key == on ? " class='on' aria-current='page'" : "") + ">" + label + "</a>";
        string u = "SupportSubscribers.aspx?user=" + user + "&amp;view="; Func<int, string> k = i => n == null ? "" : " (" + n[i] + ")";
        return "<nav class='sa-tabs'>" + t("businesses", "Businesses" + k(0), u + "businesses") + t("employees", "Employees" + k(1), u + "employees") + t("projects", "Projects" + k(2), u + "projects")
            + t("billing", "Subscription &amp; Billing", u + "billing") + t("support", "Support History", "SupportTickets.aspx?q=" + HttpUtility.UrlEncode(SA.S(r["Email"]))) + t("history", "Activity Log", u + "history") + "</nav>";
    }
    // Businesses tab (PPT 01 s3): Sl. No., Business Name, Industry, No. of Employees, No. of Projects, Status, Actions.
    static string Biz(SqlConnection c, DataRow r)
    {
        List<DataRow> rows;
        try
        {
            rows = SA.Q(c, BizCte + @"SELECT b.BusinessID,b.CompanyName,(SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=b.BusinessID AND a.EmployeeID<>@a) Employees,
                (SELECT COUNT(*) FROM AddProject p WHERE p.BusinessID=b.BusinessID) Projects,CASE WHEN ISNULL(b.Status,'')='2' THEN 'Deleted' ELSE 'Active' END Status FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID ORDER BY b.CompanyName", ScopeArgs(r)).Rows.Cast<DataRow>().ToList();
        }
        catch (SqlException ex) { return "<div class='sa-card'><p class='sa-muted'>Could not load this list: " + SA.H(ex.Message) + "</p></div>"; }
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Sl. No.</th><th>Business Name</th><th>Industry</th><th>No. of Employees</th><th>No. of Projects</th><th>Status</th><th>Actions</th></tr></thead><tbody>");
        int i = 0;
        foreach (var x in rows)
            sb.Append("<tr><td>").Append(++i).Append("</td><td><strong>").Append(SA.H(x["CompanyName"])).Append("</strong></td><td>&mdash;</td><td>").Append(x["Employees"]).Append("</td><td>").Append(x["Projects"]).Append("</td><td>").Append(SA.Pill(SA.S(x["Status"]), SA.S(x["Status"]) == "Active" ? "green" : "grey"))
              .Append("</td><td><a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportBusinesses.aspx?q=").Append(SA.A(HttpUtility.UrlEncode(SA.S(x["CompanyName"])))).Append("'>View</a></td></tr>");
        if (rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No businesses found.</td></tr>");
        return sb.Append("</tbody></table></div></div>").ToString();
    }
    // v42 PPT 01 s6 Edit Subscriber. vals == null: first render; otherwise re-render after a validation error.
    static readonly string[] EF = { "ed_first", "ed_middle", "ed_last", "ed_phone", "ed_mobile", "ed_status", "ed_biz", "ed_users", "ed_start", "ed_end" };
    static string[] Current(DataRow r)
    {
        Func<object, string> d = o => o == null || o == DBNull.Value ? "" : Convert.ToDateTime(o, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return new[] { SA.S(r["FirstName"]).Trim(), SA.S(r["MiddleName"]).Trim(), SA.S(r["LastName"]).Trim(), SA.S(r["MainPhone"]), SA.S(r["MobilePhone"]), r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]) ? "Active" : "Inactive", SA.S(r["PlanBiz"]), SA.S(r["PlanUsers"]), d(r["PlanStart"]), d(r["NextBilling"]) };
    }
    static readonly string[] EL = { "First Name", "Middle Name", "Last Name", "Contact Phone", "Mobile Phone", "Account Status", "Business Limit", "Employee Limit", "Start Date", "Next Billing Date" };
    string Edit(SqlConnection c, DataRow r, string[] vals)
    {
        int u = Convert.ToInt32(r["UserId"]); var v = vals ?? Current(r); bool plan = r["PlanRowId"] != DBNull.Value;
        Func<int, string, string, string> f = (i, type, extra) => "<label class='sa-field'><span>" + EL[i] + "</span><input type='" + type + "' name='" + EF[i] + "' value='" + SA.A(v[i]) + "'" + extra + " /></label>";
        var sb = new StringBuilder("<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-grid2'><div class='sa-card'><h2>Personal Information</h2><div class='sa-grid3'>")
            .Append(f(0, "text", " required maxlength='150'")).Append(f(1, "text", " maxlength='50'")).Append(f(2, "text", " maxlength='50'")).Append("</div></div>")
            .Append("<div class='sa-card'><h2>Contact Information</h2><label class='sa-field'><span>Email</span><input value='").Append(SA.A(r["Email"])).Append("' readonly /><small class='sa-muted'>Login email changes use Change Login Email verification.</small></label><div class='sa-grid2'>")
            .Append(f(3, "tel", " maxlength='30'")).Append(f(4, "tel", " maxlength='30'")).Append("</div></div></div>");
        sb.Append("<div class='sa-grid2'><div class='sa-card' id='plan'><h2>Plan &amp; Limits</h2><p><small class='sa-muted'>Current Plan</small><br /><strong>").Append(SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "No active plan").Append("</strong> <a class='sa-link sa-small' href='SupportPlans.aspx'>View plans</a></p>")
          .Append(plan ? "<div class='sa-grid2'>" + f(6, "number", " min='0' max='100000'") + f(7, "number", " min='0' max='100000'") + "</div>" : "<p class='sa-muted'>No active plan record, so limits and dates cannot be changed here.</p>")
          .Append("</div><div class='sa-card'><h2>Account Status</h2>");
        foreach (var o in new[] { "Active", "Inactive" }) sb.Append("<label class='sa-radio'><input type='radio' name='ed_status' value='").Append(o).Append("'").Append(v[5] == o ? " checked" : "").Append(" /> ").Append(o).Append("</label> ");
        sb.Append("<p class='sa-muted sa-small'>To suspend sign-in with a reason, use <a class='sa-link' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=access'>Manage Login &amp; Access</a>.</p>")
          .Append(plan ? "<h2>Subscription Dates</h2><div class='sa-grid2'>" + f(8, "date", "") + f(9, "date", "") + "</div>" : "").Append("</div></div>");
        string addr = String.Join(", ", new[] { SA.S(r["Street"]), SA.S(r["City"]), (SA.S(r["Province"]) + " " + SA.S(r["Postalcode"])).Trim() }.Where(x => x.Length > 0).ToArray());
        sb.Append("<div class='sa-grid2'><div class='sa-card'><h2>Address</h2><p>").Append(addr.Length > 0 ? SA.H(addr) : "&mdash;").Append("</p></div><div class='sa-card'><h2>Recent Activity (Last 5)</h2>");
        DataTable h = null; try { if (SA.Has(c, "AnyminuteSupportAudit")) h = SA.Q(c, "SELECT TOP 5 * FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k ORDER BY ChangedAtUTC DESC,AuditID DESC", "@k", u.ToString(CultureInfo.InvariantCulture)); } catch (SqlException) { }
        if (h == null || h.Rows.Count == 0) sb.Append("<p class='sa-muted'>No activity recorded yet.</p>");
        else { sb.Append("<ul class='sa-list'>"); foreach (DataRow x in h.Rows) sb.Append("<li><strong>").Append(SA.H(x["Action"])).Append("</strong> <small class='sa-muted'>").Append(SA.When(x["ChangedAtUTC"]).Replace("<br />", " ")).Append("</small></li>"); sb.Append("</ul>"); }
        sb.Append("<a class='sa-link' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=history'>View Full History</a></div></div>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(u).Append("'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='edit'>Save Changes</button></div>");
        return sb.ToString();
    }
    static string Header(SqlConnection c, DataRow r)
    {
        int u = Convert.ToInt32(r["UserId"]);
        return "<div class='sa-card sa-subhead'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><h2>" + SA.H(Name(r)) + "</h2><small>Subscriber ID: " + Sub(u) + "<br />" + SA.H(r["Email"]) + (SA.S(r["MobilePhone"]).Length > 0 ? " &middot; " + SA.H(r["MobilePhone"]) : "") + "</small></div>"
            + "<div><small>Current Plan</small><strong>" + (SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;") + "</strong><a class='sa-link' href='SupportPlans.aspx'>View Plan Details</a></div>"
            + "<div><small>Subscriber Since</small><strong>" + SA.Day(r["Since"]) + "</strong></div><div><small>Last Login</small><strong>" + SA.When(r["LastLoginDate"]) + "</strong></div>"
            + "<div>" + (Enabled(r) ? SA.Pill("Active", "green") : SA.Pill("Disabled", "red")) + "</div></div>";
    }
    // Businesses the subscriber owns: the owner's own AssginBussiness rows (UserTypeId 1) or businesses mapped to the owner's login
    // (same rule as AnyminuteModernScope). Deleted businesses (Status 2) are shown but not counted.
    const string BizCte = @"WITH biz AS (SELECT DISTINCT a.BusinessID FROM AssginBussiness a WHERE (a.EmployeeID=@a AND a.UserTypeId=1)
        OR EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=@u AND m.BusinessID=a.BusinessID AND m.EmployeeID=a.EmployeeID AND m.UserTypeId=a.UserTypeId)) ";
    static object[] ScopeArgs(DataRow r) { return new object[] { "@a", Convert.ToInt32(r["EmployeeId"]), "@u", Convert.ToInt32(r["UserId"]) }; }
    static int[] Counts(SqlConnection c, DataRow r)
    {
        var t = SA.Q(c, BizCte + @"SELECT (SELECT COUNT(*) FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID WHERE ISNULL(b.Status,'')<>'2'),
            (SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a JOIN biz ON biz.BusinessID=a.BusinessID JOIN AddEmployee ae ON ae.EmployeeID=a.EmployeeID WHERE a.EmployeeID<>@a AND ae.TerminationDate IS NULL),
            (SELECT COUNT(*) FROM AddProject p JOIN biz ON biz.BusinessID=p.BusinessID)", ScopeArgs(r));
        return new[] { Convert.ToInt32(t.Rows[0][0]), Convert.ToInt32(t.Rows[0][1]), Convert.ToInt32(t.Rows[0][2]) };
    }
    static string Tabs(SqlConnection c, DataRow r, string on)
    {
        int user = Convert.ToInt32(r["UserId"]); string email = SA.S(r["Email"]); int[] n;
        try { n = Counts(c, r); } catch (SqlException) { n = null; }
        Func<string, string, string, string> t = (key, label, href) => "<a href='" + href + "'" + (key == on ? " class='on' aria-current='page'" : "") + ">" + label + "</a>";
        string u = "SupportSubscribers.aspx?user=" + user + "&amp;view=";
        Func<int, string> k = i => n == null ? "" : " (" + n[i] + ")";
        return "<nav class='sa-tabs'>" + t("profile", "Profile", "SupportSubscribers.aspx?user=" + user) + t("businesses", "Businesses" + k(0), u + "businesses") + t("employees", "Employees" + k(1), u + "employees") + t("projects", "Projects" + k(2), u + "projects")
            + t("billing", "Subscription &amp; Billing", u + "billing") + t("access", "Login &amp; Access", u + "access")
            + t("support", "Support History", "SupportTickets.aspx?q=" + HttpUtility.UrlEncode(email)) + t("history", "Login Activity History", u + "history") + "</nav>";
    }
    // Read-only lists for the Businesses / Employees / Projects tabs (edits stay with the subscriber's own pages).
    static string Related(SqlConnection c, DataRow r, string view)
    {
        string q = SA.Get("q"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        string sql; string[] head;
        if (view == "businesses") { head = new[] { "Business", "Operating Name", "Primary Contact", "City", "Province", "Employees", "Projects", "Status" };
            sql = BizCte + @"SELECT b.CompanyName,b.OperatingName,b.PrimaryContactName,COALESCE(ci.CityName,b.City) City,COALESCE(pr.ProvinceName,b.Province) Province,
                (SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=b.BusinessID AND a.EmployeeID<>@a) Employees,(SELECT COUNT(*) FROM AddProject p WHERE p.BusinessID=b.BusinessID) Projects,
                CASE WHEN ISNULL(b.Status,'')='2' THEN 'Deleted' ELSE 'Active' END Status
                FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(b.City AS nvarchar(50)) LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=CAST(b.Province AS nvarchar(50)) ORDER BY b.CompanyName"; }
        else if (view == "employees") { head = new[] { "Employee", "Email", "Phone", "Role", "Business", "Hire Date", "Status" };
            sql = BizCte + @"SELECT LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Employee,ea.MainEmail Email,ae.MobilePhone Phone,ut.UserType Role,b.CompanyName Business,ae.HireDate,
                CASE WHEN ae.TerminationDate IS NOT NULL THEN 'Terminated' ELSE 'Active' END Status
                FROM AssginBussiness a JOIN biz ON biz.BusinessID=a.BusinessID JOIN AddEmployee ae ON ae.EmployeeID=a.EmployeeID JOIN AddBusiness b ON b.BusinessID=a.BusinessID
                LEFT JOIN EmpAddress ea ON ea.EmpAddressId=ae.ADDRESSID LEFT JOIN UserType ut ON ut.User_TypeId=a.UserTypeId WHERE a.EmployeeID<>@a ORDER BY Employee,b.CompanyName"; }
        else { head = new[] { "Project", "Business", "Start Date", "End Date", "Status" };
            sql = BizCte + @"SELECT p.ProjectName Project,b.CompanyName Business,p.startDate,p.EndDate,CASE WHEN p.EndDate IS NOT NULL AND p.EndDate<CAST(GETDATE() AS date) THEN 'Ended' ELSE 'Active' END Status
                FROM AddProject p JOIN biz ON biz.BusinessID=p.BusinessID JOIN AddBusiness b ON b.BusinessID=p.BusinessID ORDER BY p.ProjectName"; }
        List<DataRow> rows;
        try { rows = SA.Q(c, sql, ScopeArgs(r)).Rows.Cast<DataRow>().ToList(); }
        catch (SqlException ex) { return "<div class='sa-card'><p class='sa-muted'>Could not load this list: " + SA.H(ex.Message) + "</p></div>"; }
        if (q.Length > 0) rows = rows.Where(x => String.Join(" ", x.ItemArray.Select(SA.S)).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><input type='hidden' name='user' value='" + r["UserId"] + "' /><input type='hidden' name='view' value='" + view + "' />"
            + "<label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='" + SA.A(q) + "' placeholder='Search " + view + "...' /></label><button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>"
            + "<p class='sa-muted'>Read-only view. The subscriber manages these records from their own AnyMinute pages.</p><div class='sa-table-wrap'><table class='sa-table'><thead><tr>");
        foreach (var h in head) sb.Append("<th>").Append(h).Append("</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var x in rows.Skip((page - 1) * PageSize).Take(PageSize))
        {
            sb.Append("<tr>");
            for (int i = 0; i < head.Length; i++)
            {
                object v = x[i]; string cell = v is DateTime ? SA.Day(v) : SA.S(v).Length > 0 ? SA.H(v) : "&mdash;";
                if (head[i] == "Status") cell = SA.Pill(SA.S(v), SA.S(v) == "Active" ? "green" : "grey");
                sb.Append(i == 0 ? "<td><strong>" + cell + "</strong></td>" : "<td>" + cell + "</td>");
            }
            sb.Append("</tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='").Append(head.Length).Append("' class='sa-empty'>No ").Append(view).Append(" found.</td></tr>");
        return sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, "user=" + r["UserId"] + "&view=" + view + "&q=" + HttpUtility.UrlEncode(q))).Append("</div>").ToString();
    }
    // Subscription & Billing: PlanHistory rows (the limits the app enforces) next to actual usage and the plan catalogue.
    static string Billing(SqlConnection c, DataRow r)
    {
        var plans = SA.Q(c, "SELECT PlanName,StartDate,EndDate,AmountPaid,No_Of_Users,Business,IsActive,Is_Cancelled FROM PlanHistory WHERE EmailId=@m OR EmpId=@e ORDER BY StartDate DESC", "@m", SA.S(r["Email"]), "@e", Convert.ToInt32(r["EmployeeId"]));
        int[] n; try { n = Counts(c, r); } catch (SqlException) { n = new[] { 0, 0, 0 }; }
        DataRow act = plans.Rows.Cast<DataRow>().FirstOrDefault(x => SA.S(x["IsActive"]) == "1" || SA.S(x["IsActive"]) == "True");
        string cat = "&mdash;";
        if (act != null && SA.Has(c, "AnyminuteSubscriptionPlan"))
        {
            var p = SA.Q(c, "SELECT TOP 1 MaxProjects,TrialDays FROM AnyminuteSubscriptionPlan WHERE " + String.Format(SA.PlanKeySql, "PlanName") + "=@k", "@k", SA.PlanKey(SA.S(act["PlanName"])));
            if (p.Rows.Count == 1) cat = SA.S(p.Rows[0]["MaxProjects"]).Length > 0 ? SA.S(p.Rows[0]["MaxProjects"]) : "Custom";
        }
        Func<string, int, string, string, string> kpi = (label, used, limit, note) => "<div class='sa-card'><small>" + label + "</small><h2>" + used + " / " + (limit.Length > 0 ? SA.H(limit) : "&mdash;") + "</h2><small class='sa-muted'>" + note + "</small></div>";
        var sb = new StringBuilder("<div class='sa-grid3'>")
            .Append(kpi("Businesses used / plan limit", n[0], act == null ? "" : SA.S(act["Business"]), "Enforced from PlanHistory.Business"))
            .Append(kpi("Employees used / plan limit", n[1], act == null ? "" : SA.S(act["No_Of_Users"]), "Enforced from PlanHistory.No_Of_Users"))
            .Append(kpi("Projects used / catalogue limit", n[2], cat == "&mdash;" ? "" : cat, "Catalogue value; not enforced by the app"))
            .Append("</div><div class='sa-card'><h2>Plan History</h2><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Plan</th><th>Start</th><th>End</th><th>Amount Paid</th><th>Employees Limit</th><th>Business Limit</th><th>Status</th></tr></thead><tbody>");
        foreach (DataRow x in plans.Rows)
        {
            bool on = SA.S(x["IsActive"]) == "1" || SA.S(x["IsActive"]) == "True", cancelled = SA.S(x["Is_Cancelled"]) == "1" || SA.S(x["Is_Cancelled"]) == "True";
            sb.Append("<tr><td><strong>").Append(SA.H(x["PlanName"])).Append("</strong></td><td>").Append(SA.Day(x["StartDate"])).Append("</td><td>").Append(SA.Day(x["EndDate"])).Append("</td><td>").Append(SA.S(x["AmountPaid"]).Length > 0 ? "$" + SA.H(x["AmountPaid"]) : "&mdash;")
              .Append("</td><td>").Append(SA.H(x["No_Of_Users"])).Append("</td><td>").Append(SA.H(x["Business"])).Append("</td><td>").Append(cancelled ? SA.Pill("Cancelled", "red") : on ? SA.Pill("Active", "green") : SA.Pill("Inactive", "grey")).Append("</td></tr>");
        }
        if (plans.Rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No plan history for this subscriber.</td></tr>");
        return sb.Append("</tbody></table></div><p class='sa-muted'>Payments are processed by the existing subscription pages; Stripe billing details are not shown here. <a class='sa-link' href='SupportPlans.aspx?view=usage'>Usage &amp; Limits for all plans</a></p></div>").ToString();
    }
    string Access(SqlConnection c, DataRow r, bool ready)
    {
        int u = Convert.ToInt32(r["UserId"]); bool en = Enabled(r), first = r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]);
        string lastReset = "&mdash;";
        if (SA.Has(c, "AnyminuteSupportAudit")) { var lr = SA.Q(c, "SELECT TOP 1 ChangedAtUTC FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k AND Action='Password Reset' ORDER BY ChangedAtUTC DESC", "@k", u.ToString(CultureInfo.InvariantCulture)); if (lr.Rows.Count == 1) lastReset = SA.When(lr.Rows[0][0]).Replace("<br />", " "); }
        string dis = ready ? "" : " disabled";
        var sb = new StringBuilder("<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-grid2'><div class='sa-card'><h2>Current Login Information</h2><table class='sa-kv'>");
        sb.Append("<tr><th>Account Status</th><td>").Append(SA.Pill(r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]) ? "Active" : "Inactive", r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]) ? "green" : "grey")).Append("</td></tr>")
          .Append("<tr><th>Login Access</th><td>").Append(en ? SA.Pill("Enabled", "green") : SA.Pill("Disabled", "red")).Append("</td></tr>")
          .Append("<tr><th>Invitation Status</th><td>").Append(first ? "Pending" : "Accepted").Append("</td></tr>")
          .Append("<tr><th>Last Login</th><td>").Append(SA.When(r["LastLoginDate"]).Replace("<br />", " ")).Append("</td></tr>")
          .Append("<tr><th>Last Password Reset</th><td>").Append(lastReset).Append("</td></tr>")
          .Append("<tr><th>Active Sessions</th><td>").Append(Sessions(c, u)).Append("</td></tr></table></div>");
        sb.Append("<div class='sa-card'><h2>Change Login Access</h2><label class='sa-radio sa-radio-block'><input type='radio' name='sa_access' value='1'").Append(en ? " checked" : "").Append(dis).Append(" /><span><b>Enable Login Access</b><small>Allow the subscriber to sign in to AnyMinute.</small></span></label>")
          .Append("<label class='sa-radio sa-radio-block'><input type='radio' name='sa_access' value='0'").Append(!en ? " checked" : "").Append(dis).Append(" /><span><b>Disable Login Access</b><small>Prevent the subscriber from signing in. Active sessions will be revoked immediately.</small></span></label>")
          .Append("<div class='sa-flash sa-warn'>Disabling login will prevent this subscriber from signing in. Existing businesses, employees, projects and data will remain preserved.</div>")
          .Append("<label class='sa-field'><span>Reason for Access Change *</span><textarea name='sa_reason' rows='3' maxlength='500' placeholder='Enter reason for changing subscriber login access...' data-sa-counter='500'").Append(dis).Append("></textarea><small data-sa-count='1'>0/500</small></label></div></div>");
        sb.Append("<div class='sa-card'><h2>Additional Account Actions</h2><div class='sa-actions3'>")
          .Append("<button type='submit' class='sa-action' name='sa_op' value='invite' data-sa-confirm='Send a new activation invitation to ").Append(SA.A(r["Email"])).Append("?'").Append(dis).Append("><b>&#9993; Resend Invitation</b><small>Send a new activation invitation to the subscriber.</small></button>")
          .Append("<a class='sa-action' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=reset'><b>&#128273; Send Password Reset</b><small>Send a secure password reset link to the subscriber.</small></a>")
          .Append("<a class='sa-action' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=history'><b>&#8634; View Login History</b><small>View recent sign-in activity and login history.</small></a></div>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='access'").Append(dis).Append(">&#128190; Save Changes</button></div></div>");
        return sb.ToString();
    }
    static string Sessions(SqlConnection c, int u)
    {
        DataTable t; try { t = AnyminuteSessionGuard.Active(c, u); } catch (SqlException) { t = null; }
        if (t == null) return "<span class='sa-muted'>Not tracked (run REQUIRED_08)</span>";
        if (t.Rows.Count == 0) return "0";
        var r = t.Rows[0];
        return t.Rows.Count + " <small class='sa-muted'>(last activity " + SA.When(r["LastSeenAtUTC"]).Replace("<br />", " ") + (SA.S(r["IpAddress"]).Length > 0 ? " from " + SA.H(r["IpAddress"]) : "") + ")</small>";
    }
    static string Reset(DataRow r, bool ready)
    {
        int u = Convert.ToInt32(r["UserId"]); string email = SA.S(r["Email"]), first = Name(r).Split(' ')[0];
        return "<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-card sa-steps'>"
            + "<section><h2><b class='sa-step'>1</b> Subscriber Information</h2><div class='sa-subhead sa-flat'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><strong>" + SA.H(Name(r)) + "</strong><small>Subscriber ID: " + Sub(u) + "<br />" + SA.H(email) + "</small></div>"
            + "<div><small>Account Status</small>" + SA.Pill(r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]) ? "Active" : "Inactive", "green") + "</div><div><small>Login Access</small>" + (Enabled(r) ? SA.Pill("Enabled", "green") : SA.Pill("Disabled", "red")) + "</div><div><small>Last Login</small><strong>" + SA.When(r["LastLoginDate"]) + "</strong></div></div></section>"
            + "<section><h2><b class='sa-step'>2</b> Password Reset Email</h2><div class='sa-grid2'><div><label class='sa-field'><span>Send to Email Address</span><input value='" + SA.A(email) + "' readonly /></label><small class='sa-muted'>A secure password reset link will be sent to this email address.</small></div>"
            + "<div class='sa-preview'><small>Message Preview (Email Subject and Content)</small><p><b>Subject: Reset Your AnyMinute Password</b></p><p>Hello " + SA.H(first) + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password. This link will expire in 24 hours and can be used once.</p><p><span class='sa-link'>Reset Your Password</span></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p></div></div></section>"
            + "<section><h2><b class='sa-step'>3</b> Confirm and Send</h2><div class='sa-flash sa-warn'><b>Please confirm before sending.</b><ul><li>A secure password reset link will be sent to " + SA.H(email) + ".</li><li>The link will expire in 24 hours and works only once; sending a new link cancels earlier ones.</li><li>The previous password will remain invalid after the reset.</li><li>This action will be recorded in the activity log.</li></ul></div></section>"
            + "<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=" + u + "'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='reset'" + (ready ? "" : " disabled") + ">&#10148; Send Password Reset</button></div></div>";
    }
    string History(SqlConnection c, int user)
    {
        if (!SA.Has(c, "AnyminuteSupportAudit")) return "<div class='sa-card'><p class='sa-muted'>" + SA.H(SA.MissingTables) + "</p></div>";
        string from = SA.Get("from"), to = SA.Get("to"), ev = SA.Get("event"), st = SA.Get("status"), ip = SA.Get("ip"), dev = SA.Get("device");
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        var all = SA.Q(c, "SELECT * FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k ORDER BY ChangedAtUTC DESC,AuditID DESC", "@k", user.ToString(CultureInfo.InvariantCulture)).Rows.Cast<DataRow>().ToList();
        DateTime f, t; bool hf = DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out f), ht = DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
        var rows = all.Where(r =>
        {
            var d = DateTime.SpecifyKind(Convert.ToDateTime(r["ChangedAtUTC"]), DateTimeKind.Utc).ToLocalTime().Date;
            if (hf && d < f) return false; if (ht && d > t) return false;
            if (ev.Length > 0 && SA.S(r["Action"]) != ev) return false;
            bool ok = r["Success"] == DBNull.Value || Convert.ToBoolean(r["Success"]);
            if (st == "Success" && !ok) return false; if (st == "Failed" && ok) return false;
            if (ip.Length > 0 && !SA.S(r["IpAddress"]).Contains(ip)) return false;
            if (dev.Length > 0 && SA.S(r["Device"]).IndexOf(dev, StringComparison.OrdinalIgnoreCase) < 0) return false;
            return true;
        }).ToList();
        var events = all.Select(r => SA.S(r["Action"])).Distinct().OrderBy(x => x);
        bool locCol = AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteSupportAudit", "Location"); int budget = 3000;
        var devices = all.Select(r => SA.S(r["Device"]).Split('/')[0].Trim()).Where(x => x.Length > 0).Distinct().OrderBy(x => x);
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><input type='hidden' name='user' value='" + user + "' /><input type='hidden' name='view' value='history' />"
            + "<label class='sa-range'><span>Date Range</span><span><input type='date' name='from' value='" + SA.A(from) + "' aria-label='From' /> &ndash; <input type='date' name='to' value='" + SA.A(to) + "' aria-label='To' /></span></label>"
            + "<label><span>Event Type</span><select name='event'>" + SA.Options(events, ev, "All Events") + "</select></label>"
            + "<label><span>Status</span><select name='status'>" + SA.Options(new[] { "Success", "Failed" }, st, "All Statuses") + "</select></label>"
            + "<label><span>IP Address</span><input name='ip' value='" + SA.A(ip) + "' placeholder='Search IP address...' /></label>"
            + "<label><span>Device</span><select name='device'>" + SA.Options(devices, dev, "All Devices") + "</select></label>"
            + "<button type='button' class='sa-btn' data-sa-apply='1'>&#128269; Search</button><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=" + user + "&amp;view=history'>&#8635; Clear</a></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Date &amp; Time</th><th>Event Type</th><th>Description</th><th>IP Address</th><th>Location</th><th>Device / Browser</th><th>Performed By</th><th>Status</th></tr></thead><tbody>");
        foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
        {
            bool ok = r["Success"] == DBNull.Value || Convert.ToBoolean(r["Success"]);
            string by = SA.S(r["ChangedByName"]); bool self = by == "Subscriber";
            sb.Append("<tr><td>").Append(SA.When(r["ChangedAtUTC"])).Append("</td><td><strong>").Append(SA.H(r["Action"])).Append("</strong></td><td>").Append(SA.H(r["Comments"])).Append("</td><td>").Append(SA.S(r["IpAddress"]).Length > 0 ? SA.H(r["IpAddress"]) : "&mdash;")
              .Append("</td><td>").Append(Location(c, r, locCol, ref budget)).Append("</td><td>").Append(SA.S(r["Device"]).Length > 0 ? SA.H(r["Device"]) : "&mdash;").Append("</td><td>").Append(self ? "Subscriber" : SA.H(by) + "<br /><small>(Support Admin)</small>")
              .Append("</td><td>").Append(SA.Pill(ok ? "Success" : "Failed", ok ? "green" : "red")).Append("</td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='8' class='sa-empty'>No login activity recorded yet. Sign-ins are recorded from v25 onward.</td></tr>");
        string baseQ = "user=" + user + "&view=history&from=" + from + "&to=" + to + "&event=" + HttpUtility.UrlEncode(ev) + "&status=" + st + "&ip=" + HttpUtility.UrlEncode(ip) + "&device=" + HttpUtility.UrlEncode(dev);
        return sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, baseQ).Replace(" of " + rows.Count + "</span>", " of " + rows.Count + " activities</span>")).Append("</div>").ToString();
    }
    // Stored location, else best-effort lookup (AnyminuteGeoIp) saved back to the audit row so it is resolved once.
    static string Location(SqlConnection c, DataRow r, bool locCol, ref int budget)
    {
        string ip = SA.S(r["IpAddress"]);
        if (ip.Length == 0) return "&mdash;";
        if (locCol && SA.S(r["Location"]).Length > 0) return SA.H(r["Location"]);
        string loc = AnyminuteGeoIp.Locate(ip, ref budget);
        if (loc == AnyminuteGeoIp.NotConfigured) return "<span class='sa-muted' title='Public IP locations need a lookup service: set GeoIpLookupUrl in web.config.'>Not resolved</span>";
        if (loc.Length == 0) return "<span class='sa-muted'>Unknown</span>";
        if (locCol) try { SA.X(c, null, "UPDATE AnyminuteSupportAudit SET Location=@l WHERE AuditID=@id AND Location IS NULL", "@l", loc.Length > 120 ? loc.Substring(0, 120) : loc, "@id", r["AuditID"]); } catch (SqlException) { }
        return SA.H(loc);
    }
    void Act(int user)
    {
        string op = SA.Form("sa_op"); int u; Int32.TryParse(SA.Form("sa_user"), out u);
        if (u <= 0 || u != user) { SA.Flash("Subscriber not found."); Back(0, null); return; }
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteSupportAudit") || !SA.Has(c, "AnyminuteLoginSecurity")) { SA.Flash(SA.MissingTables); Back(u, null); return; }
            var t = Subscribers(c, u); if (t.Rows.Count != 1) { SA.Flash("Subscriber not found."); Back(0, null); return; }
            var r = t.Rows[0]; string key = u.ToString(CultureInfo.InvariantCulture), email = SA.S(r["Email"]);
            if (op == "edit" || op == "editsave") { EditPost(c, r, op == "editsave"); return; }
            if (op == "access")
            {
                bool enable = SA.Form("sa_access") == "1", was = Enabled(r); string reason = SA.Form("sa_reason");
                if (reason.Length == 0) { SA.Flash("Enter the reason for changing login access."); Back(u, null); return; }
                if (reason.Length > 500) reason = reason.Substring(0, 500);
                if (u == SA.ActorId()) { SA.Flash("You cannot change your own login access here."); Back(u, null); return; }
                if (enable == was) { SA.Flash("Login access is already " + (was ? "enabled" : "disabled") + "."); Back(u, null); return; }
                using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u) INSERT AnyminuteLoginSecurity(UserId,LoginDisabled,RequirePasswordChange,TwoFactorEnabled,UpdatedAtUTC,UpdatedBy) VALUES(@u,0,0,0,SYSUTCDATETIME(),@a);
                        UPDATE AnyminuteLoginSecurity SET LoginDisabled=@d,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE UserId=@u", "@u", u, "@a", SA.ActorId(), "@d", !enable);
                    if (enable) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE UserAcc SET Status=1 WHERE UserId=@u AND Status=0", "@u", u);
                    int ended = enable ? 0 : AnyminuteSessionGuard.Revoke(c, tx, u, "Login access disabled by Support Admin");
                    SA.Audit(c, tx, "Login", key, enable ? "Login Access Enabled" : "Login Access Disabled", was ? "Enabled" : "Disabled", enable ? "Enabled" : "Disabled", null, null, reason + (enable ? "" : ended >= 0 ? " [Sessions revoked: " + ended + "]" : ""));
                    tx.Commit();
                    if (!enable) AnyminuteSessionGuard.Revoked(u);
                }
                SA.Flash(enable ? "Login access enabled." : "Login access disabled. The subscriber can no longer sign in and active sessions were revoked.");
                Back(u, null); return;
            }
            if (op == "invite" || op == "reset")
            {
                if (email.Length == 0 || email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) { SA.Flash("This subscriber has no email address."); Back(u, null); return; }
                bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
                if (!send) { SA.Flash("Email sending is turned off for this site (IsSendMail), so nothing was sent."); Back(u, op == "reset" ? "reset" : null); return; }
                var link = SA.Q(c, "SELECT TOP 1 ae.EmployeeID FROM EmpAddress a JOIN AddEmployee ae ON a.EmpAddressId=ae.ADDRESSID WHERE a.MainEmail=@m ORDER BY ae.EmployeeID", "@m", email);
                int linkEmployee = link.Rows.Count == 1 ? Convert.ToInt32(link.Rows[0][0]) : Convert.ToInt32(r["EmployeeId"]);
                string url = ConfigurationManager.AppSettings["ResetUrl"] + linkEmployee, first = HttpUtility.HtmlEncode(Name(r).Split(' ')[0]);
                var mail = new EmailSettingController();
                if (op == "reset")
                {
                    // v26: one-time token, valid 24 hours (REQUIRED_08). Without the table the legacy link is refused rather than sent unprotected.
                    if (!AnyminutePasswordReset.Ready(c)) { SA.Flash("Run DB_REQUIRED/REQUIRED_08_SupportAdminStrict.sql to enable 24-hour password reset links. Nothing was sent."); Back(u, "reset"); return; }
                    url += "&rt=" + AnyminutePasswordReset.Issue(c, null, u, linkEmployee, email, SA.ActorId());
                    string body = "<p>Hello " + first + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password. This link will expire in 24 hours and can be used once.</p><p><a href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\">Reset Your Password</a></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p>";
                    mail.SendHtmlFormattedEmail(email, "Reset Your AnyMinute Password", body);
                    SA.Audit(c, null, "Login", key, "Password Reset", null, null, null, email, "Password reset link sent to subscriber (expires in 24 hours, one-time use).");
                    SA.Flash("Password reset link sent to " + email + ".");
                }
                else
                {
                    var tpl = SA.Q(c, "SELECT TOP 1 * FROM TemplateMaster WHERE TempType=1");
                    if (tpl.Rows.Count == 0) { SA.Flash("The invitation email template (TemplateMaster TempType 1) is missing, so no invitation was sent."); Back(u, null); return; }
                    var acc = SA.Q(c, "SELECT uid FROM UserAcc WHERE UserId=@u", "@u", u);
                    string body = mail.PopulateBodyWithUrl(first, "Welcome", url, SA.S(tpl.Rows[0]["description"]), SA.S(tpl.Rows[0]["TempName"]), email + " <br />User Id - " + SA.S(acc.Rows[0]["uid"]), "");
                    mail.SendHtmlFormattedEmail(email, SA.S(tpl.Rows[0]["Subject"]), body);
                    SA.Audit(c, null, "Login", key, "Activation Email", null, null, null, email, "Activation invitation re-sent.");
                    SA.Flash("Invitation email sent to " + email + ".");
                }
                Back(u, null); return;
            }
        }
        Back(u, null);
    }
    // v42: null = Login & Access (its own view now), "" = Subscriber Details.
    void Back(int user, string view) { if (view == null) view = "access"; Response.Redirect("SupportSubscribers.aspx" + (user > 0 ? "?user=" + user + (view.Length > 0 ? "&view=" + view : "") : ""), false); Context.ApplicationInstance.CompleteRequest(); }
    // v42 PPT 01 s6: Save Changes -> Confirm Changes (Field / Previous Value / New Value + Reason for Change *) -> Subscriber Updated Successfully.
    void EditPost(SqlConnection c, DataRow r, bool save)
    {
        int u = Convert.ToInt32(r["UserId"]); var cur = Current(r); var nv = EF.Select(k => SA.Form(k).Trim()).ToArray(); bool plan = r["PlanRowId"] != DBNull.Value;
        if (nv[5] != "Inactive") nv[5] = "Active";
        if (!plan) { nv[6] = cur[6]; nv[7] = cur[7]; nv[8] = cur[8]; nv[9] = cur[9]; }
        var errs = new List<string>(); int x; DateTime d1, d2;
        if (nv[0].Length == 0) errs.Add("First name is required.");
        foreach (int i in new[] { 3, 4 }) if (nv[i].Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(nv[i], @"^[0-9+()\-.\s]{7,30}$")) errs.Add(EL[i] + " is not valid.");
        foreach (int i in new[] { 6, 7 }) if (plan && nv[i].Length > 0 && (!Int32.TryParse(nv[i], out x) || x < 0)) errs.Add(EL[i] + " must be a whole number.");
        bool h1 = DateTime.TryParseExact(nv[8], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1), h2 = DateTime.TryParseExact(nv[9], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
        if (plan && (nv[8].Length > 0 && !h1 || nv[9].Length > 0 && !h2)) errs.Add("Enter valid subscription dates.");
        if (plan && h1 && h2 && d2 < d1) errs.Add("Next Billing Date must be after the Start Date.");
        if (u == SA.ActorId() && nv[5] == "Inactive") errs.Add("You cannot deactivate your own account here.");
        var changed = Enumerable.Range(0, EF.Length).Where(i => nv[i] != cur[i]).ToList();
        Title1.Text = "Edit Subscriber"; Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; <a href='SupportSubscribers.aspx?user=" + u + "'>Subscriber Details</a> &rsaquo; <span>Edit Subscriber</span></nav>";
        if (errs.Count > 0 || changed.Count == 0)
        {
            Flash.Text = errs.Count > 0 ? "<div class='sa-flash sa-warn'>Please fix the following:<ul>" + String.Join("", errs.Select(m => "<li>" + SA.H(m) + "</li>").ToArray()) + "</ul></div>" : "<div class='sa-flash'>No changes to save.</div>";
            Content.Text = Edit(c, r, nv); return;
        }
        string reason = SA.Form("ed_reason");
        if (!save || reason.Length == 0)
        {
            var sb = new StringBuilder("<input type='hidden' name='sa_user' value='" + u + "' />");
            for (int i = 0; i < EF.Length; i++) sb.Append("<input type='hidden' name='").Append(EF[i]).Append("' value='").Append(SA.A(nv[i])).Append("' />");
            sb.Append("<div class='sa-card'><h2>Confirm Changes</h2><p>Please review the changes below before saving.</p><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Field</th><th>Previous Value</th><th>New Value</th></tr></thead><tbody>");
            foreach (int i in changed) sb.Append("<tr><td>").Append(EL[i]).Append("</td><td>").Append(cur[i].Length > 0 ? SA.H(cur[i]) : "&mdash;").Append("</td><td><strong>").Append(nv[i].Length > 0 ? SA.H(nv[i]) : "&mdash;").Append("</strong></td></tr>");
            sb.Append("</tbody></table></div><label class='sa-field'><span>Reason for Change *</span><textarea name='ed_reason' rows='3' maxlength='500' required data-sa-counter='500' placeholder='Enter reason for this change...'></textarea><small data-sa-count='1'>0/500</small></label>")
              .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=edit'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='editsave'>Confirm &amp; Save</button></div></div>");
            if (save) Flash.Text = "<div class='sa-flash sa-warn'>Enter the reason for this change.</div>";
            Content.Text = sb.ToString(); return;
        }
        if (reason.Length > 500) reason = reason.Substring(0, 500);
        Func<string, object> N = t => t.Length == 0 ? (object)DBNull.Value : t;
        using (var tx = c.BeginTransaction())
        {
            int e = Convert.ToInt32(r["EmployeeId"]);
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddEmployee SET FirstName=@f,MiddleName=@m,LastName=@l,MobilePhone=@mo WHERE EmployeeID=@e", "@f", nv[0], "@m", N(nv[1]), "@l", N(nv[2]), "@mo", N(nv[4]), "@e", e);
            if (changed.Contains(3)) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE ea SET MainPhone=@p FROM EmpAddress ea JOIN AddEmployee ae ON ae.ADDRESSID=ea.EmpAddressId WHERE ae.EmployeeID=@e", "@p", N(nv[3]), "@e", e);
            if (changed.Contains(5)) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE UserAcc SET Status=@s WHERE UserId=@u", "@s", nv[5] == "Active", "@u", u);
            if (plan && changed.Any(i => i >= 6))
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE PlanHistory SET Business=@b,No_Of_Users=@n,StartDate=@sd,EndDate=@ed WHERE Id=@id", "@b", N(nv[6]), "@n", N(nv[7]), "@sd", h1 ? (object)d1 : DBNull.Value, "@ed", h2 ? (object)d2 : DBNull.Value, "@id", Convert.ToInt32(r["PlanRowId"]));
            SA.Audit(c, tx, "Login", u.ToString(CultureInfo.InvariantCulture), "Subscriber Updated", null, null, String.Join("; ", changed.Select(i => EL[i] + ": " + cur[i]).ToArray()), String.Join("; ", changed.Select(i => EL[i] + ": " + nv[i]).ToArray()), reason);
            tx.Commit();
        }
        SA.Flash("Subscriber Updated Successfully."); Back(u, "");
    }
}
