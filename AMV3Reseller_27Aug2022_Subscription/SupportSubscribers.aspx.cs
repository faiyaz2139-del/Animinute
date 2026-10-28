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
            Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; <a href='SupportSubscribers.aspx?user=" + user + "'>Subscriber Details</a> &rsaquo; <span>" + (view == "history" ? "Login Activity History" : view == "reset" ? "Send Password Reset" : "Login &amp; Access") + "</span></nav>";
            if (view == "history") { Title1.Text = "Subscriber Login Activity History"; Lead.Text = "View sign-in activity, account changes and security events for this subscriber."; Content.Text = Header(c, r) + Tabs(user, "history", SA.S(r["Email"])) + History(c, user); }
            else if (view == "reset") { Title1.Text = "Send Password Reset"; Lead.Text = "Send a secure password reset link to the subscriber."; Content.Text = Reset(r, ready); }
            else { Title1.Text = "Manage Subscriber Login &amp; Access"; Lead.Text = "Subscriber Details &rsaquo; Login &amp; Access"; Content.Text = Header(c, r) + Tabs(user, "access", SA.S(r["Email"])) + Access(c, r, ready); }
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
    static string Tabs(int user, string on, string email)
    {
        Func<string, string, string, string> t = (key, label, href) => "<a href='" + href + "'" + (key == on ? " class='on' aria-current='page'" : "") + ">" + label + "</a>";
        return "<nav class='sa-tabs'>" + t("profile", "Profile", "SuperAdmin_Approve_Subscriber.aspx") + t("billing", "Subscription &amp; Billing", "SupportPlans.aspx?view=usage") + t("access", "Login &amp; Access", "SupportSubscribers.aspx?user=" + user)
            + t("support", "Support History", "SupportTickets.aspx?q=" + HttpUtility.UrlEncode(email)) + t("history", "Login Activity History", "SupportSubscribers.aspx?user=" + user + "&amp;view=history") + "</nav>";
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
          .Append("<tr><th>Active Sessions</th><td><span class='sa-muted'>Not tracked</span></td></tr></table></div>");
        sb.Append("<div class='sa-card'><h2>Change Login Access</h2><label class='sa-radio sa-radio-block'><input type='radio' name='sa_access' value='1'").Append(en ? " checked" : "").Append(dis).Append(" /><span><b>Enable Login Access</b><small>Allow the subscriber to sign in to AnyMinute.</small></span></label>")
          .Append("<label class='sa-radio sa-radio-block'><input type='radio' name='sa_access' value='0'").Append(!en ? " checked" : "").Append(dis).Append(" /><span><b>Disable Login Access</b><small>Prevent the subscriber from signing in. The next sign-in is blocked.</small></span></label>")
          .Append("<div class='sa-flash sa-warn'>Disabling login will prevent this subscriber from signing in. Existing businesses, employees, projects and data will remain preserved.</div>")
          .Append("<label class='sa-field'><span>Reason for Access Change *</span><textarea name='sa_reason' rows='3' maxlength='500' placeholder='Enter reason for changing subscriber login access...' data-sa-counter='500'").Append(dis).Append("></textarea><small data-sa-count='1'>0/500</small></label></div></div>");
        sb.Append("<div class='sa-card'><h2>Additional Account Actions</h2><div class='sa-actions3'>")
          .Append("<button type='submit' class='sa-action' name='sa_op' value='invite' data-sa-confirm='Send a new activation invitation to ").Append(SA.A(r["Email"])).Append("?'").Append(dis).Append("><b>&#9993; Resend Invitation</b><small>Send a new activation invitation to the subscriber.</small></button>")
          .Append("<a class='sa-action' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=reset'><b>&#128273; Send Password Reset</b><small>Send a secure password reset link to the subscriber.</small></a>")
          .Append("<a class='sa-action' href='SupportSubscribers.aspx?user=").Append(u).Append("&amp;view=history'><b>&#8634; View Login History</b><small>View recent sign-in activity and login history.</small></a></div>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='access'").Append(dis).Append(">&#128190; Save Changes</button></div></div>");
        return sb.ToString();
    }
    static string Reset(DataRow r, bool ready)
    {
        int u = Convert.ToInt32(r["UserId"]); string email = SA.S(r["Email"]), first = Name(r).Split(' ')[0];
        return "<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-card sa-steps'>"
            + "<section><h2><b class='sa-step'>1</b> Subscriber Information</h2><div class='sa-subhead sa-flat'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><strong>" + SA.H(Name(r)) + "</strong><small>Subscriber ID: " + Sub(u) + "<br />" + SA.H(email) + "</small></div>"
            + "<div><small>Account Status</small>" + SA.Pill(r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]) ? "Active" : "Inactive", "green") + "</div><div><small>Login Access</small>" + (Enabled(r) ? SA.Pill("Enabled", "green") : SA.Pill("Disabled", "red")) + "</div><div><small>Last Login</small><strong>" + SA.When(r["LastLoginDate"]) + "</strong></div></div></section>"
            + "<section><h2><b class='sa-step'>2</b> Password Reset Email</h2><div class='sa-grid2'><div><label class='sa-field'><span>Send to Email Address</span><input value='" + SA.A(email) + "' readonly /></label><small class='sa-muted'>A secure password reset link will be sent to this email address.</small></div>"
            + "<div class='sa-preview'><small>Message Preview (Email Subject and Content)</small><p><b>Subject: Reset Your AnyMinute Password</b></p><p>Hello " + SA.H(first) + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password.</p><p><span class='sa-link'>Reset Your Password</span></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p></div></div></section>"
            + "<section><h2><b class='sa-step'>3</b> Confirm and Send</h2><div class='sa-flash sa-warn'><b>Please confirm before sending.</b><ul><li>A secure password reset link will be sent to " + SA.H(email) + ".</li><li>The link opens the existing AnyMinute reset page (ResetPassord.aspx).</li><li>The current password keeps working until the subscriber sets a new one.</li><li>This action will be recorded in the activity log.</li></ul></div></section>"
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
              .Append("</td><td><span class='sa-muted'>&mdash;</span></td><td>").Append(SA.S(r["Device"]).Length > 0 ? SA.H(r["Device"]) : "&mdash;").Append("</td><td>").Append(self ? "Subscriber" : SA.H(by) + "<br /><small>(Support Admin)</small>")
              .Append("</td><td>").Append(SA.Pill(ok ? "Success" : "Failed", ok ? "green" : "red")).Append("</td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='8' class='sa-empty'>No login activity recorded yet. Sign-ins are recorded from v25 onward.</td></tr>");
        string baseQ = "user=" + user + "&view=history&from=" + from + "&to=" + to + "&event=" + HttpUtility.UrlEncode(ev) + "&status=" + st + "&ip=" + HttpUtility.UrlEncode(ip) + "&device=" + HttpUtility.UrlEncode(dev);
        return sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, baseQ).Replace(" of " + rows.Count + "</span>", " of " + rows.Count + " activities</span>")).Append("</div>").ToString();
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
                    SA.Audit(c, tx, "Login", key, enable ? "Login Access Enabled" : "Login Access Disabled", was ? "Enabled" : "Disabled", enable ? "Enabled" : "Disabled", null, null, reason);
                    tx.Commit();
                }
                SA.Flash(enable ? "Login access enabled." : "Login access disabled. The subscriber can no longer sign in.");
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
                    string body = "<p>Hello " + first + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password.</p><p><a href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\">Reset Your Password</a></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p>";
                    mail.SendHtmlFormattedEmail(email, "Reset Your AnyMinute Password", body);
                    SA.Audit(c, null, "Login", key, "Password Reset", null, null, null, email, "Password reset link sent to subscriber.");
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
