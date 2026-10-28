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
            (SELECT TOP 1 p.PlanName FROM PlanHistory p WHERE p.EmailId=u.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) PlanName,
            (SELECT MIN(p.StartDate) FROM PlanHistory p WHERE p.EmailId=u.Email) Since," +
            (sec ? "ISNULL(s.LoginDisabled,0) LoginDisabled," : "CAST(0 AS bit) LoginDisabled,") + (last ? "s.LastLoginDate" : "CAST(NULL AS datetime2) LastLoginDate") +
            " FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId" + (sec ? " LEFT JOIN AnyminuteLoginSecurity s ON s.UserId=u.UserId" : "") +
            " WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)" + (user > 0 ? " AND u.UserId=@u" : "") + " ORDER BY u.UserId DESC", "@u", user);
    }
    static bool Enabled(DataRow r) { return !Convert.ToBoolean(r["LoginDisabled"]) && (r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"])); }
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
            string crumb = view == "history" ? "Login Activity History" : view == "reset" ? "Send Password Reset" : view == "businesses" ? "Businesses" : view == "employees" ? "Employees" : view == "projects" ? "Projects" : view == "billing" ? "Subscription &amp; Billing" : "Login &amp; Access";
            Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; <a href='SupportSubscribers.aspx?user=" + user + "'>Subscriber Details</a> &rsaquo; <span>" + crumb + "</span></nav>";
            if (view == "businesses" || view == "employees" || view == "projects" || view == "billing") { Title1.Text = "Subscriber Details"; Lead.Text = "Subscriber Details &rsaquo; " + crumb + " (read-only)"; Content.Text = Header(c, r) + Tabs(c, r, view) + (view == "billing" ? Billing(c, r) : Related(c, r, view)); }
            else if (view == "history") { Title1.Text = "Subscriber Login Activity History"; Lead.Text = "View sign-in activity, account changes and security events for this subscriber."; Content.Text = Header(c, r) + Tabs(c, r, "history") + History(c, user); }
            else if (view == "reset") { Title1.Text = "Send Password Reset"; Lead.Text = "Send a secure password reset link to the subscriber."; Content.Text = Reset(r, ready); }
            else { Title1.Text = "Manage Subscriber Login &amp; Access"; Lead.Text = "Subscriber Details &rsaquo; Login &amp; Access"; Content.Text = Header(c, r) + Tabs(c, r, "access") + Access(c, r, ready); }
        }
    }
    void List(SqlConnection c)
    {
        Title1.Text = "Subscribers"; Lead.Text = "Find a subscriber to manage login access, send a password reset or review login history.";
        string q = SA.Get("q"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        var rows = Subscribers(c, 0).Rows.Cast<DataRow>().Where(r => q.Length == 0 || (Name(r) + " " + SA.S(r["Email"]) + " " + SA.S(r["MobilePhone"]) + " " + Sub(Convert.ToInt32(r["UserId"]))).ToLowerInvariant().Contains(q.ToLowerInvariant())).ToList();
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='" + SA.A(q) + "' placeholder='Search subscriber by name, email or phone...' /></label><button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Subscriber</th><th>Subscriber ID</th><th>Email</th><th>Phone</th><th>Current Plan</th><th>Login Access</th><th>Last Login</th><th>Actions</th></tr></thead><tbody>");
        foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
        {
            int u = Convert.ToInt32(r["UserId"]);
            sb.Append("<tr><td><span class='sa-avatar sa-avatar-sm'>").Append(SA.H(Initials(Name(r)))).Append("</span> <strong>").Append(SA.H(Name(r))).Append("</strong></td><td>").Append(Sub(u)).Append("</td><td>").Append(SA.H(r["Email"])).Append("</td><td>").Append(SA.H(r["MobilePhone"]))
              .Append("</td><td>").Append(SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;").Append("</td><td>").Append(Enabled(r) ? SA.Pill("Enabled", "green") : SA.Pill("Disabled", "red")).Append("</td><td>").Append(SA.When(r["LastLoginDate"]))
              .Append("</td><td class='sa-actions'><a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(u).Append("'>Manage</a> <a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=history'>History</a></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='8' class='sa-empty'>No subscribers found.</td></tr>");
        Content.Text = sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, "q=" + HttpUtility.UrlEncode(q))).Append("</div>").ToString();
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
        return "<nav class='sa-tabs'>" + t("profile", "Profile", "SuperAdmin_Approve_Subscriber.aspx") + t("businesses", "Businesses" + k(0), u + "businesses") + t("employees", "Employees" + k(1), u + "employees") + t("projects", "Projects" + k(2), u + "projects")
            + t("billing", "Subscription &amp; Billing", u + "billing") + t("access", "Login &amp; Access", "SupportSubscribers.aspx?user=" + user)
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
    void Back(int user, string view) { Response.Redirect("SupportSubscribers.aspx" + (user > 0 ? "?user=" + user + (view != null ? "&view=" + view : "") : ""), false); Context.ApplicationInstance.CompleteRequest(); }
}
