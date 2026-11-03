using AD = AnyminuteSupportAddress;
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
    // v45 PPT 01 s3/s4/s6: line icons (inline SVG, currentColor) so the screens carry the slide icons instead of emoji.
    static readonly Dictionary<string, string> Icons = new Dictionary<string, string>
    {
        { "mail", "<rect x='2' y='4' width='20' height='16' rx='2'/><path d='m22 6-10 7L2 6'/>" },
        { "phone", "<path d='M22 16.9v3a2 2 0 0 1-2.2 2 19.8 19.8 0 0 1-8.6-3.1 19.5 19.5 0 0 1-6-6A19.8 19.8 0 0 1 2.1 4.2 2 2 0 0 1 4.1 2h3a2 2 0 0 1 2 1.7c.1 1 .4 1.9.7 2.8a2 2 0 0 1-.5 2.1L8.1 9.9a16 16 0 0 0 6 6l1.3-1.3a2 2 0 0 1 2.1-.4c.9.3 1.8.6 2.8.7a2 2 0 0 1 1.7 2z'/>" },
        { "pin", "<path d='M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0 1 18 0z'/><circle cx='12' cy='10' r='3'/>" },
        { "cal", "<rect x='3' y='4' width='18' height='18' rx='2'/><path d='M16 2v4M8 2v4M3 10h18M8 14h2M14 14h2M8 18h2'/>" },
        { "id", "<rect x='2' y='5' width='20' height='14' rx='2'/><circle cx='8' cy='12' r='2'/><path d='M14 10h5M14 14h3'/>" },
        { "biz", "<rect x='4' y='2' width='16' height='20' rx='1'/><path d='M9 22v-4h6v4M8 6h2M14 6h2M8 10h2M14 10h2M8 14h2M14 14h2'/>" },
        { "users", "<path d='M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2'/><circle cx='9' cy='7' r='4'/><path d='M23 21v-2a4 4 0 0 0-3-3.9M16 3.1a4 4 0 0 1 0 7.8'/>" },
        { "folder", "<path d='M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z'/>" },
        { "gear", "<circle cx='12' cy='12' r='3'/><path d='M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1A1.7 1.7 0 0 0 9 19.4a1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1A1.7 1.7 0 0 0 4.6 9a1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1z'/>" },
        { "edit", "<path d='M12 20h9'/><path d='M16.5 3.5a2.1 2.1 0 0 1 3 3L7 19l-4 1 1-4z'/>" },
        { "more", "<circle cx='5' cy='12' r='1'/><circle cx='12' cy='12' r='1'/><circle cx='19' cy='12' r='1'/>" },
        { "vmore", "<circle cx='12' cy='5' r='1'/><circle cx='12' cy='12' r='1'/><circle cx='12' cy='19' r='1'/>" },
        { "chev", "<path d='m6 9 6 6 6-6'/>" },
        { "search", "<circle cx='11' cy='11' r='8'/><path d='m21 21-4.3-4.3'/>" },
        { "refresh", "<path d='M23 4v6h-6M1 20v-6h6'/><path d='M3.5 9a9 9 0 0 1 14.9-3.4L23 10M1 14l4.6 4.4A9 9 0 0 0 20.5 15'/>" },
        { "plus", "<path d='M12 5v14M5 12h14'/>" },
        { "save", "<path d='M19 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h11l5 5v11a2 2 0 0 1-2 2z'/><path d='M17 21v-8H7v8M7 3v5h8'/>" },
        { "open", "<path d='M18 13v6a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h6'/><path d='M15 3h6v6M10 14 21 3'/>" },
        { "check", "<path d='M20 6 9 17l-5-5'/>" },
        { "swap", "<path d='m17 1 4 4-4 4'/><path d='M3 11V9a4 4 0 0 1 4-4h14'/><path d='m7 23-4-4 4-4'/><path d='M21 13v2a4 4 0 0 1-4 4H3'/>" },
        { "home", "<path d='m3 9 9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z'/>" },
        { "info", "<circle cx='12' cy='12' r='10'/><path d='M12 16v-4M12 8h.01'/>" }
    };
    static string Ic(string name) { return "<svg class='sa-i' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true' focusable='false'>" + Icons[name] + "</svg>"; }
    static string CrumbBar(string trail) { return "<nav class='sa-crumbs sa-crumbs-v45' aria-label='Breadcrumb'><a href='SupportSubscribers.aspx'>" + Ic("home") + " Subscribers</a>" + trail + "</nav>"; }
    static string CrumbSep(string inner) { return " <span class='sa-crumb-sep' aria-hidden='true'>&rsaquo;</span> " + inner; }
    // Usage tile (icon, label, used / limit, coloured bar) shared by Subscriber Details and Edit Subscriber.
    static string Tile(string icon, string tone, string label, int used, string lim)
    {
        int l; bool has = Int32.TryParse(lim, out l) && l > 0; int pct = has ? Math.Min(100, used * 100 / l) : 0;
        return "<div class='sa-tile'><span class='sa-tile-ico sa-t-" + tone + "'>" + Ic(icon) + "</span><div class='sa-tile-txt'><small>" + label + "</small><strong>" + used + " / " + (lim.Length > 0 ? SA.H(lim) : "&mdash;") + "</strong><span class='sa-bar sa-bar-" + tone + "'><i style='width:" + pct + "%'></i></span></div></div>";
    }
    static string ProjectLimit(SqlConnection c, string plan)
    {
        if (plan.Length == 0 || !SA.Has(c, "AnyminuteSubscriptionPlan")) return "";
        try { var p = SA.Q(c, "SELECT TOP 1 MaxProjects FROM AnyminuteSubscriptionPlan WHERE " + String.Format(SA.PlanKeySql, "PlanName") + "=@k", "@k", SA.PlanKey(plan)); return p.Rows.Count == 1 ? SA.S(p.Rows[0][0]) : ""; } catch (SqlException) { return ""; }
    }
    // v52 (NOTES_v52 E): every street fragment goes through Clean() so "Please Select" never reaches the header address.
    static string Addr(DataRow r) { string street = String.Join(" ", new[] { Clean(r["AStreetNo"]), Clean(r["AStreetName"]), Clean(r["AStreetType"]) }.Where(x => x.Length > 0).ToArray()); if (street.Length == 0) street = Clean(r["Street"]); return String.Join(", ", new[] { street, Clean(r["City"]), Clean(r["Province"]), Clean(r["Postalcode"]) }.Where(x => x.Trim().Length > 0).ToArray()); }
    // v47: a Pending subscriber's More menu leads with Activate Subscriber (Edit Subscriber with Active preselected; Confirm + Reason
    // still required) and Resend Invitation (the existing Login & Access "invite" action). withUser: the page has no sa_user field yet.
    static string MoreMenu(DataRow r, bool withUser)
    {
        int u = Convert.ToInt32(r["UserId"]); string pend = "";
        if (StatusOf(r) == "Pending")
            pend = "<a class='sa-menu-item' href='SupportSubscribers.aspx?user=" + u + "&amp;view=edit&amp;st=Active#sa-status'><i aria-hidden='true'>&#10004;</i><span><b>Activate Subscriber</b><small>Set the account to Active (confirmation and reason required)</small></span></a>"
                + "<button type='submit' class='sa-menu-item' name='sa_op' value='invite' formnovalidate='formnovalidate' data-sa-confirm='Send a new activation invitation with a new temporary password to " + SA.A(r["Email"]) + "?'><i aria-hidden='true'>&#9993;</i><span><b>Resend Invitation</b><small>New activation email with a new temporary password</small></span></button>";
        return (withUser ? "<input type='hidden' name='sa_user' value='" + u + "' />" : "") + "<details class='sa-menu'><summary class='sa-btn sa-btn-outline'>" + Ic("more") + " More " + Ic("chev") + "</summary><div class='sa-menu-pop'><h4>Subscriber Actions</h4><small>" + SA.H(Name(r)) + "</small>" + pend + Menu(u) + "</div></details>";
    }
    DataTable Subscribers(SqlConnection c, int user)
    {
        bool sec = SA.Has(c, "AnyminuteLoginSecurity"), last = sec && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteLoginSecurity", "LastLoginDate");
        return SA.Q(c, @"SELECT u.UserId,u.EmployeeId,u.Email,u.Status,u.Is_First_Time_Login,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.MiddleName,'')+' '+ISNULL(ae.LastName,''))) Name,ae.MobilePhone,
            pl.PlanName,pl.Id PlanRowId,pl.StartDate PlanStart,pl.EndDate NextBilling,pl.Business PlanBiz,pl.No_Of_Users PlanUsers,ae.FirstName,ae.MiddleName,ae.LastName,ea.MainPhone,
            LTRIM(RTRIM(ISNULL(ea.StreetNo,'')+' '+ISNULL(NULLIF(ea.NumberSuffix,'Please Select'),'')+' '+ISNULL(NULLIF(ea.StreetName,'Please Select'),'')+' '+ISNULL(NULLIF(ea.StreetType,'Please Select'),''))) Street,COALESCE(ci.CityName,ea.City) City,COALESCE(pr.ProvinceName,ea.Province) Province,ea.Postalcode,
            ae.ADDRESSID AddrId,ea.StreetNo AStreetNo,ea.StreetName AStreetName,ea.StreetType AStreetType,ea.SuitType ASuitType,ea.SuiteNumber ASuiteNo," + (AnyminuteEmployeeEditor.HasColumn(c, null, "EmpAddress", "Country") ? "ea.Country ACountry," : "CAST(NULL AS nvarchar(50)) ACountry,") + @"
            " + AnyminuteResellers.OwnedBiz + @" Businesses,(SELECT COUNT(DISTINCT a2.EmployeeID) FROM AssginBussiness a2 WHERE a2.EmployeeID<>u.EmployeeId AND a2.BusinessID IN (SELECT a.BusinessID FROM AssginBussiness a WHERE a.EmployeeID=u.EmployeeId AND a.UserTypeId=1)) Users,
            (SELECT MIN(p.StartDate) FROM PlanHistory p WHERE p.EmailId=u.Email) Since," +
            (sec ? "ISNULL(s.LoginDisabled,0) LoginDisabled," : "CAST(0 AS bit) LoginDisabled,") + (last ? "s.LastLoginDate" : "CAST(NULL AS datetime2) LastLoginDate") +
            " FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId LEFT JOIN EmpAddress ea ON ea.EmpAddressId=ae.ADDRESSID" +
            " LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=ea.City LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=ea.Province" +
            " OUTER APPLY (SELECT TOP 1 p.Id,p.PlanName,p.StartDate,p.EndDate,p.Business,p.No_Of_Users FROM PlanHistory p WHERE p.EmailId=u.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) pl" + (sec ? " LEFT JOIN AnyminuteLoginSecurity s ON s.UserId=u.UserId" : "") +
            " WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)" + (user > 0 ? " AND u.UserId=@u" : "") + " ORDER BY u.UserId DESC", "@u", user);
    }
    static bool Enabled(DataRow r) { return !Convert.ToBoolean(r["LoginDisabled"]) && (r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"])); }
    // v47 (Nov 2 live smoke): four states, the same ones Add Subscriber writes (AnyminuteSubscriberCreate):
    // Inactive = UserAcc.Status 0; Suspended = login disabled (AnyminuteLoginSecurity); Pending = first sign-in not done yet; else Active.
    static bool AccOn(DataRow r) { return r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]); }
    static string StatusOf(DataRow r) { return !AccOn(r) ? "Inactive" : Convert.ToBoolean(r["LoginDisabled"]) ? "Suspended" : r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]) ? "Pending" : "Active"; }
    static string Tone(string st) { return st == "Active" ? "green" : st == "Pending" ? "amber" : st == "Suspended" ? "red" : "grey"; }
    // Edit Subscriber status label (Add Subscriber wording: "Pending Activation").
    static string EditStatusOf(DataRow r) { string st = StatusOf(r); return st == "Pending" ? AStatus[0] : st; }
    static string Name(DataRow r) { string n = SA.S(r["Name"]).Replace("  ", " "); return n.Length > 0 ? n : SA.S(r["Email"]); }
    protected void Page_Load(object sender, EventArgs e)
    {
        int user; Int32.TryParse(SA.Get("user"), out user);
        if (SA.Get("lookup") == "1") { Lookup(); return; }
        string op0 = SA.Form("sa_op");
        if (IsPostBack && (op0 == "add" || op0 == "addsave" || op0 == "addback")) { AddPost(); return; }
        if (IsPostBack && op0.Length > 0) { Act(user); return; }
        Flash.Text = SA.TakeFlash();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteSupportAudit") && SA.Has(c, "AnyminuteLoginSecurity");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteSupportAudit", "AnyminuteLoginSecurity");
            if (user <= 0) { if (SA.Get("view") == "add") AddView(c, null); else List(c); return; }
            var t = Subscribers(c, user);
            if (t.Rows.Count != 1) { Title1.Text = "Subscriber not found"; Content.Text = "<p><a class='sa-link' href='SupportSubscribers.aspx'>Back to Subscribers</a></p>"; return; }
            var r = t.Rows[0]; string view = SA.Get("view");
            string crumb = view == "history" ? "Login Activity History" : view == "reset" ? "Send Password Reset" : view == "edit" ? "Edit Subscriber" : view == "access" ? "Login &amp; Access" : view == "updated" ? "Edit Subscriber" : view == "bizedit" ? "Edit Business" : "";
            // v45 PPT 01 s3/s4/s6: home-icon breadcrumb "Subscribers > Subscriber Details" (Edit: "Subscribers > Edit Subscriber").
            Crumbs.Text = CrumbBar(crumb.Length == 0 ? CrumbSep("<span>Subscriber Details</span>") : view == "edit" || view == "updated" ? CrumbSep("<span>Edit Subscriber</span>") : CrumbSep("<a href='SupportSubscribers.aspx?user=" + user + "'>Subscriber Details</a>") + CrumbSep("<span>" + crumb + "</span>"));
            // v45 PPT 01 s3/s4: Subscriber Details has no page title; the subscriber header card is the top of the page.
            // v51 PPT s1-s4: every tab (Businesses / Employees / Projects / Subscription & Billing / Login & Access / Support History /
            // Activity Log) renders inside the same Subscriber Details shell - the header card stays, only the tab body changes.
            // v53 (NOTES_v53 #2): access and history are back inside the sticky Details + DTabs shell (v51 pattern) - the header card
            // never changes across Businesses / Employees / Projects / Billing / Login & Access / Support History / Login Activity History.
            if (view == "" || view == "businesses" || view == "employees" || view == "projects" || view == "billing" || view == "access" || view == "support" || view == "history")
            {
                Title1.Text = ""; Lead.Text = ""; string on = view.Length == 0 ? "businesses" : view;
                Crumbs.Text = CrumbBar(CrumbSep(on == "businesses" ? "<span>Subscriber Details</span>" : "<a href='SupportSubscribers.aspx?user=" + user + "'>Subscriber Details</a>") + (on == "access" ? CrumbSep("<span>Login &amp; Access</span>") : on == "support" ? CrumbSep("<span>Support History</span>") : on == "history" ? CrumbSep("<span>Login Activity History</span>") : ""));
                if (on == "history") { var nm = Name(r).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); SubName = nm.Length > 1 ? nm[0] + " " + nm[nm.Length - 1] : Name(r); }
                string body = on == "billing" ? Billing(c, r) : on == "employees" || on == "projects" ? Related(c, r, on) : on == "access" ? "<div class='sa-tabpad'>" + Access(c, r, ready) + "</div>" : on == "support" ? Support(c, r) : on == "history" ? History(c, user) : Biz(c, r);
                Content.Text = Details(c, r) + "<div class='sa-tabbed'>" + DTabs(c, r, on) + body + "</div>";
            }
            else if (view == "bizedit") { int bz; Int32.TryParse(SA.Get("biz"), out bz); Title1.Text = ""; Lead.Text = ""; Content.Text = Details(c, r) + "<div class='sa-tabbed'>" + DTabs(c, r, "businesses") + BizEdit(c, r, bz, null, null) + "</div>"; }
            else if (view == "edit")
            {
                // v47: ?st=Active (Pending "Activate Subscriber") preselects the new status; the Confirm Changes + Reason step still applies.
                string[] pre = null; string want = SA.Get("st");
                if (AStatus.Contains(want) && want != EditStatusOf(r)) { pre = Current(r); pre[6] = want; Flash.Text += "<div class='sa-flash sa-info'>Status set to " + SA.H(want) + ". Review and click Save Changes to confirm.</div>"; }
                Title1.Text = "Edit Subscriber"; Lead.Text = "Update subscriber information, plan and limits."; Content.Text = Edit(c, r, pre);
            }
            else if (view == "created")
            {
                Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; <span>Add Subscriber</span></nav>"; Title1.Text = "Add Subscriber"; Lead.Text = "";
                Content.Text = Done("Subscriber Created Successfully!", SA.Get("inv") == "1" ? "An activation invitation with the sign-in email and temporary password has been sent to " + SA.H(r["Email"]) + "." : "The subscriber account for " + SA.H(r["Email"]) + " has been created.", "", "View Subscriber Details", "SupportSubscribers.aspx?user=" + user, "Add Another Subscriber", "SupportSubscribers.aspx?view=add");
            }
            else if (view == "updated")
            {
                // v45 PPT 01 s6 step 4: the success message is a dialog over Edit Subscriber.
                Title1.Text = "Edit Subscriber"; Lead.Text = "Update subscriber information, plan and limits.";
                Content.Text = Edit(c, r, null) + UpdatedModal(user);
            }
            else if (view == "reset") { Title1.Text = "Send Password Reset"; Lead.Text = "Send a secure password reset link to the subscriber."; Content.Text = Reset(r, ready); }
            else { Response.Redirect("SupportSubscribers.aspx?user=" + user, false); Context.ApplicationInstance.CompleteRequest(); }
        }
    }
    static readonly int[] Sizes = { 10, 20, 50 };
    // v45 PPT 01 s3/s4 list: "3 / 5" businesses and "18 / 50" users (used / plan limit).
    static string Used(object used, object limit) { string l = SA.S(limit).Trim(); return SA.H(used) + (l.Length > 0 ? " / " + SA.H(l) : ""); }
    static string Menu(int u)
    {
        string b = "SupportSubscribers.aspx?user=" + u;
        Func<string, string, string, string, string> a = (href, ico, t, sm) => "<a class='sa-menu-item' href='" + href + "'><i aria-hidden='true'>" + ico + "</i><span><b>" + t + "</b><small>" + sm + "</small></span></a>";
        // v42 PPT 01 s2 "Recommended three-dot Actions menu" (labels / descriptions as on the slide)
        // v51 (held note): "View Subscriber Details" removed - the list has its own View button.
        return a(b + "&amp;view=edit", "&#9998;", "Edit Subscriber", "Update personal and contact information")
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
        // v43 PPT 01 s1: sortable column headers (Subscriber Name, Plan Name, No. of Businesses, No. of Users, Email ID, Activation Date).
        string sort = SA.Get("sort"), dir = SA.Get("dir") == "desc" ? "desc" : "asc"; Func<DataRow, object> key = null;
        if (sort == "name") key = r => Name(r).ToLowerInvariant(); else if (sort == "plan") key = r => SA.S(r["PlanName"]).ToLowerInvariant();
        else if (sort == "biz") key = r => Convert.ToInt32(r["Businesses"]); else if (sort == "users") key = r => Convert.ToInt32(r["Users"]);
        else if (sort == "email") key = r => SA.S(r["Email"]).ToLowerInvariant(); else if (sort == "since") key = r => r["Since"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["Since"]);
        else sort = "";
        if (key != null) rows = (dir == "desc" ? rows.OrderByDescending(key) : rows.OrderBy(key)).ToList();
        string bq0 = "SupportSubscribers.aspx?q=" + HttpUtility.UrlEncode(q) + "&plan=" + HttpUtility.UrlEncode(plan) + "&status=" + HttpUtility.UrlEncode(st), bq = bq0 + (sort.Length > 0 ? "&sort=" + sort + "&dir=" + dir : "");
        Func<string, string, string> th = (label, k) => "<th><a class='sa-sort" + (sort == k ? " on" : "") + "' href='" + SA.A(bq0 + "&size=" + size + "&sort=" + k + "&dir=" + (sort == k && dir == "asc" ? "desc" : "asc")) + "'>" + label + " <i aria-hidden='true'>" + (sort == k ? (dir == "asc" ? "&#9650;" : "&#9660;") : "&#8661;") + "</i></a></th>";
        HeadActions.Text = "<a class='sa-btn' href='SupportSubscribers.aspx?view=add'>+ Add Subscriber</a>";
        int pages = Math.Max(1, (rows.Count + size - 1) / size); if (page > pages) page = pages;
        Func<string, string, string, int, string> kpi = (icon, label, tone, cnt) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + icon + "</i><div><span>" + label + "</span><strong>" + cnt + "</strong></div></div>";
        var sb = new StringBuilder("<div class='sa-kpis'>").Append(kpi("&#128101;", "Total Subscribers", "blue", all.Count)).Append(kpi("&#10004;", "Active Subscribers", "green", all.Count(r => StatusOf(r) == "Active")))
            .Append(kpi("&#10006;", "Inactive Subscribers", "red", all.Count(r => StatusOf(r) == "Inactive" || StatusOf(r) == "Suspended"))).Append(kpi("&#9203;", "Trial Subscribers", "purple", all.Count(r => SA.S(r["PlanName"]).IndexOf("TRIAL", StringComparison.OrdinalIgnoreCase) >= 0))).Append("</div>");
        sb.Append("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><input type='hidden' name='size' value='").Append(size).Append("' /><label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>" + Ic("search") + "</i><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by name, email or phone number...' /></label>")
          .Append("<label><span>Plan</span><select name='plan'>").Append(SA.Options(all.Select(r => SA.S(r["PlanName"])).Where(x => x.Length > 0).Distinct().OrderBy(x => x), plan, "All Plans")).Append("</select></label>")
          .Append("<label><span>Status</span><select name='status'>").Append(SA.Options(new[] { "Active", "Inactive", "Pending", "Suspended" }, st, "All Statuses")).Append("</select></label>")
          .Append("<a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx'>" + Ic("refresh") + " Clear</a><button type='button' class='sa-btn' data-sa-apply='1'>" + Ic("search") + " Search</button></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table sa-sub-list'><thead><tr><th><input type='checkbox' aria-label='Select all' data-sa-all='sa_sel' /></th><th>Sl. No.</th>").Append(th("Subscriber Name", "name")).Append(th("Plan Name", "plan")).Append(th("No. of Businesses", "biz")).Append(th("No. of Users", "users")).Append("<th>Contact Number</th>").Append(th("Email ID", "email")).Append(th("Activation Date", "since")).Append("<th>Status</th><th class='sa-c'>Actions</th><th class='sa-c'><span class='sr-only'>More actions</span></th></tr></thead><tbody>");
        int n = (page - 1) * size;
        foreach (var r in rows.Skip((page - 1) * size).Take(size))
        {
            int u = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + u, sts = StatusOf(r), phone = SA.S(r["MainPhone"]).Length > 0 ? SA.S(r["MainPhone"]) : SA.S(r["MobilePhone"]);
            sb.Append("<tr><td><input type='checkbox' name='sa_sel' value='").Append(u).Append("' aria-label='Select' /></td><td>").Append(++n).Append("</td><td><a class='sa-link' href='").Append(b).Append("'>").Append(SA.H(Name(r))).Append("</a></td><td>")
              .Append(SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;").Append("</td><td class='sa-c'>").Append(Used(r["Businesses"], r["PlanBiz"])).Append("</td><td class='sa-c'>").Append(Used(r["Users"], r["PlanUsers"]))
              .Append("</td><td>").Append(phone.Length > 0 ? SA.H(phone) : "&mdash;").Append("</td><td>").Append(SA.H(r["Email"])).Append("</td><td>").Append(r["Since"] == DBNull.Value ? "&mdash;" : D(r["Since"])).Append("</td><td class='sa-nowrap'>").Append(SA.Pill(sts, Tone(sts))) // v46 (t150u): status toggle removed - status pill only, as on Resellers / Businesses
              // v51 (held note): View button in Actions, the three-dot menu in the next column.
              .Append("</td><td class='sa-c'><a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(b).Append("'>View</a></td><td class='sa-c'><details class='sa-menu'><summary class='sa-kebab' aria-label='Subscriber actions'>" + Ic("vmore") + "</summary><div class='sa-menu-pop'><h4>Subscriber Actions</h4><small>").Append(SA.H(Name(r))).Append(" &middot; ").Append(sts).Append("</small>").Append(Menu(u)).Append("</div></details></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='12' class='sa-empty'>No subscribers found.</td></tr>");
        Func<int, string> pg = i => SA.A(bq + "&size=" + size + "&page=" + i);
        sb.Append("</tbody></table></div><div class='sa-pager sa-rsl-pager'><span>Showing ").Append(rows.Count == 0 ? 0 : (page - 1) * size + 1).Append(" to ").Append(Math.Min(rows.Count, page * size)).Append(" of ").Append(rows.Count).Append(" subscriber").Append(rows.Count == 1 ? "" : "s").Append("</span>")
          .Append("<label class='sa-rsl-size'><select onchange=\"location.href='").Append(SA.A(bq)).Append("&size='+this.value\">");
        foreach (int z in Sizes) sb.Append("<option value='").Append(z).Append("'").Append(z == size ? " selected" : "").Append(">").Append(z).Append(" per page</option>");
        sb.Append("</select></label><nav aria-label='Pages'>").Append(page > 1 ? "<a href='" + pg(page - 1) + "' aria-label='Previous page'>&lsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&lsaquo;</span>");
        // v45 PPT 01 s4 pager: 1 2 3 4 5 ... 13 (first window, ellipsis, last page).
        int p0 = Math.Max(1, Math.Min(page - 2, pages - 4)), p1 = Math.Min(pages, p0 + 4);
        Func<int, string> pa = i => "<a href='" + pg(i) + "'" + (i == page ? " class='on' aria-current='page'" : "") + ">" + i + "</a>";
        if (p0 > 1) sb.Append(pa(1)).Append(p0 > 2 ? "<span class='sa-pg-gap' aria-hidden='true'>&hellip;</span>" : "");
        for (int i = p0; i <= p1; i++) sb.Append(pa(i));
        if (p1 < pages) sb.Append(p1 < pages - 1 ? "<span class='sa-pg-gap' aria-hidden='true'>&hellip;</span>" : "").Append(pa(pages));
        sb.Append(page < pages ? "<a href='" + pg(page + 1) + "' aria-label='Next page'>&rsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&rsaquo;</span>").Append("</nav></div></div>");
        Content.Text = sb.ToString();
    }
    // v42 PPT 01 s3/s4 Subscriber Details; v45 laid out as on the slides: large avatar, name + status, PLAN | Subscriber since,
    // Edit / Send Password Reset / More; Email + Phone, Address, Activation + Next Billing Date icon columns;
    // Subscription Limits (PLAN) with Upgrade Plan + settings and the Businesses / Employees / Projects tiles.
    static string Details(SqlConnection c, DataRow r)
    {
        int u = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + u, sts = StatusOf(r), plan = SA.S(r["PlanName"]);
        int[] n; try { n = Counts(c, r); } catch (SqlException) { n = new[] { 0, 0, 0 }; }
        string projLimit = ProjectLimit(c, plan), addr = Addr(r), phone = SA.S(r["MainPhone"]).Length > 0 ? SA.S(r["MainPhone"]) : SA.S(r["MobilePhone"]);
        Func<string, string, string, bool, string> ii = (icon, k, v, strong) => "<div class='sa-ii'>" + Ic(icon) + "<div><small>" + k + "</small>" + (strong ? "<strong>" : "<span>") + (v.Length == 0 ? "&mdash;" : v) + (strong ? "</strong>" : "</span>") + "</div></div>";
        return "<div class='sa-card sa-sd sa-sd45'><div class='sa-sd-top'><span class='sa-avatar sa-avatar-lg'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><h2>" + SA.H(Name(r)) + " " + SA.Pill(sts, Tone(sts)) + "</h2><div class='sa-sd-sub'>" + (plan.Length > 0 ? SA.H(plan) : "No active plan") + "<span class='sa-vsep' aria-hidden='true'></span>Subscriber since " + SA.Day(r["Since"]) + "</div></div>"
            + "<div class='sa-sd-btns'><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=edit'>" + Ic("edit") + " Edit</a><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=reset'>" + Ic("mail") + " Send Password Reset</a>" + MoreMenu(r, SA.Get("view") != "bizedit" && SA.Get("view") != "access") + "</div></div>"
            + "<div class='sa-sd-body'><div class='sa-sd-info'><div class='sa-sd-col'>" + ii("mail", "Email", SA.H(r["Email"]), false) + ii("phone", "Phone", SA.H(phone), true) + "</div><div class='sa-sd-col'>" + ii("pin", "Address", SA.H(addr), false) + "</div>"
            + "<div class='sa-sd-col'>" + ii("cal", "Activation Date", SA.S(r["Since"]).Length > 0 ? SA.Day(r["Since"]) : "", true) + ii("cal", "Next Billing Date", SA.S(r["NextBilling"]).Length > 0 ? SA.Day(r["NextBilling"]) : "", true) + "</div></div>"
            + "<div class='sa-sd-limits'><div class='sa-sd-limhead'><h3>Subscription Limits" + (plan.Length > 0 ? " (" + SA.H(plan) + ")" : "") + "</h3><a class='sa-btn sa-btn-outline sa-btn-sm' href='" + b + "&amp;view=edit#plan'>Upgrade Plan</a><a class='sa-icon-btn' href='" + b + "&amp;view=edit#plan' title='Manage Custom Limits' aria-label='Manage Custom Limits'>" + Ic("gear") + "</a></div>"
            + "<div class='sa-tiles'>" + Tile("biz", "blue", "Businesses", n[0], SA.S(r["PlanBiz"])) + Tile("users", "green", "Employees", n[1], SA.S(r["PlanUsers"])) + Tile("folder", "purple", "Projects", n[2], projLimit) + "</div></div></div></div>";
    }
    // v50 PPT s1 (access): avatar, name, SUB-ID, email | Current Plan + View Plan Details | Subscriber Since | Active.
    // v50 PPT s3 (history): name + Active, "SUB-ID | Plan | email | phone" | Subscription Since | Last Login.
    static string Header(SqlConnection c, DataRow r, bool history)
    {
        int u = Convert.ToInt32(r["UserId"]); string plan = SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;";
        string badge = Enabled(r) ? SA.Pill("Active", "green") : SA.Pill("Disabled", "red");
        if (history)
            return "<div class='sa-card sa-subhead'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><h2>" + SA.H(Name(r)) + " " + badge + "</h2><small>Subscriber ID: " + Sub(u) + " &nbsp;|&nbsp; " + plan + " &nbsp;|&nbsp; " + SA.H(r["Email"])
                + (SA.S(r["MobilePhone"]).Length > 0 ? " &nbsp;|&nbsp; " + SA.H(r["MobilePhone"]) : "") + "</small></div>"
                + "<div><small>&#128197; Subscription Since</small><strong>" + SA.Day(r["Since"]) + "</strong></div><div><small>&#128339; Last Login</small><strong>" + SA.When(r["LastLoginDate"]) + "</strong></div></div>";
        return "<div class='sa-card sa-subhead'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><h2>" + SA.H(Name(r)) + "</h2><small>Subscriber ID: " + Sub(u) + "<br />" + SA.H(r["Email"]) + "</small></div>"
            + "<div><small>Current Plan</small><strong>" + plan + "</strong><a class='sa-link' href='SupportPlans.aspx'>View Plan Details</a></div>"
            + "<div><small>Subscriber Since</small><strong>" + SA.Day(r["Since"]) + "</strong></div><div>" + badge + "</div></div>";
    }
    // v52 (NOTES_v52 D): history-page tabs, Login Activity History selected; Support History stays in-app (view=support).
    static string Tabs(SqlConnection c, DataRow r, string on)
    {
        int user = Convert.ToInt32(r["UserId"]); int[] n;
        try { n = Counts(c, r); } catch (SqlException) { n = null; }
        Func<string, string, string, string> t = (key, label, href) => "<a href='" + href + "'" + (key == on ? " class='on' aria-current='page'" : "") + ">" + label + "</a>";
        string u = "SupportSubscribers.aspx?user=" + user + "&amp;view=";
        Func<int, string> k = i => n == null ? "" : " (" + n[i] + ")";
        return "<nav class='sa-tabs'>" + t("profile", "Profile", "SupportSubscribers.aspx?user=" + user) + t("businesses", "Businesses" + k(0), u + "businesses") + t("employees", "Employees" + k(1), u + "employees") + t("projects", "Projects" + k(2), u + "projects")
            + t("billing", "Subscription &amp; Billing", u + "billing") + t("access", "Login &amp; Access", u + "access")
            + t("support", "Support History", u + "support") + t("history", "Login Activity History", u + "history") + "</nav>";
    }
    static string DTabs(SqlConnection c, DataRow r, string on)
    {
        int user = Convert.ToInt32(r["UserId"]); int[] n; try { n = Counts(c, r); } catch (SqlException) { n = null; }
        Func<string, string, string, string> t = (key, label, href) => "<a href='" + href + "'" + (key == on ? " class='on' aria-current='page'" : "") + ">" + label + "</a>";
        string u = "SupportSubscribers.aspx?user=" + user + "&amp;view="; Func<int, string> k = i => n == null ? "" : " (" + n[i] + ")";
        return "<nav class='sa-tabs sa-tabs45'>" + t("businesses", "Businesses" + k(0), u + "businesses") + t("employees", "Employees" + k(1), u + "employees") + t("projects", "Projects" + k(2), u + "projects")
            + t("billing", "Subscription &amp; Billing", u + "billing") + t("access", "Login &amp; Access", u + "access") + t("support", "Support History", u + "support") + t("history", "Login Activity History", u + "history") + "</nav>";
    }
    // Businesses tab (PPT 01 s3/s4): "Businesses" + "List of businesses under this subscriber." + "+ Add Business";
    // Sl. No., Business Name, Industry, No. of Employees, No. of Projects, Status, Actions (Edit + three-dot menu).
    static string Biz(SqlConnection c, DataRow r)
    {
        int user = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + user;
        List<DataRow> rows;
        try
        {
            rows = SA.Q(c, BizCte + @"SELECT b.BusinessID,b.CompanyName,(SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=b.BusinessID AND a.EmployeeID<>@a) Employees,
                (SELECT COUNT(*) FROM AddProject p WHERE p.BusinessID=b.BusinessID) Projects,CASE WHEN ISNULL(b.Status,'')='2' THEN 'Deleted' ELSE 'Active' END Status FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID ORDER BY b.CompanyName", ScopeArgs(r)).Rows.Cast<DataRow>().ToList();
        }
        catch (SqlException ex) { return "<div class='sa-card'><p class='sa-muted'>Could not load this list: " + SA.H(ex.Message) + "</p></div>"; }
        int active = rows.Count(x => SA.S(x["Status"]) == "Active"); string lim = SA.S(r["PlanBiz"]);
        // Support has no business-create path of its own (business creation is the owner's session flow: Sp_InsertBusiness +
        // AssginBussiness + MappingTable + address), so Add Business explains where it is done instead of inserting half a business.
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-panel-head sa-biz-head'><div><h2>Businesses</h2><small>List of businesses under this subscriber.</small></div>")
            .Append("<details class='sa-menu sa-addbiz'><summary class='sa-btn'>").Append(Ic("plus")).Append(" Add Business</summary><div class='sa-menu-pop'><h4>Add Business</h4><p>New businesses are added by the subscriber from their own account (Business &rsaquo; Add Business), so ownership, address and plan limits are applied by the standard business setup.</p><p><b>Plan limit:</b> ")
            .Append(active).Append(" of ").Append(lim.Length > 0 ? SA.H(lim) : "&mdash;").Append(" businesses used.</p><a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(b).Append("&amp;view=edit#plan'>Upgrade Plan</a></div></details></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table sa-biz-table'><thead><tr><th>Sl. No.</th><th>Business Name</th><th>Industry</th><th class='sa-c'>No. of Employees</th><th class='sa-c'>No. of Projects</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        int i = 0;
        foreach (var x in rows)
        {
            int id = Convert.ToInt32(x["BusinessID"]); string name = SA.S(x["CompanyName"]), q = SA.A(HttpUtility.UrlEncode(name));
            sb.Append("<tr><td>").Append(++i).Append("</td><td><a class='sa-link' href='SupportBusinesses.aspx?q=").Append(q).Append("'>").Append(SA.H(name)).Append("</a></td><td>&mdash;</td><td class='sa-c'>").Append(x["Employees"]).Append("</td><td class='sa-c'>").Append(x["Projects"]).Append("</td><td>").Append(SA.Pill(SA.S(x["Status"]), SA.S(x["Status"]) == "Active" ? "green" : "grey"))
              .Append("</td><td class='sa-c sa-nowrap'><a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(b).Append("&amp;view=bizedit&amp;biz=").Append(id).Append("'>").Append(Ic("edit")).Append(" Edit</a> <details class='sa-menu'><summary class='sa-kebab' aria-label='Business actions'>").Append(Ic("vmore")).Append("</summary><div class='sa-menu-pop'><h4>Business Actions</h4><small>").Append(SA.H(name)).Append("</small>")
              .Append("<a class='sa-menu-item' href='").Append(b).Append("&amp;view=bizedit&amp;biz=").Append(id).Append("'><span><b>Edit Business</b><small>Name and primary contact</small></span></a>")
              .Append("<a class='sa-menu-item' href='SupportBusinesses.aspx?q=").Append(q).Append("'><span><b>View Business</b><small>Open in Businesses</small></span></a>")
              .Append("<a class='sa-menu-item' href='").Append(b).Append("&amp;view=employees'><span><b>View Employees</b><small>Employees of this subscriber</small></span></a>")
              .Append("<a class='sa-menu-item' href='").Append(b).Append("&amp;view=projects'><span><b>View Projects</b><small>Projects of this subscriber</small></span></a></div></details></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No businesses found.</td></tr>");
        return sb.Append("</tbody></table></div></div>").ToString();
    }
    // v43 PPT 01 s6 Edit Subscriber (header card, editable email, Subscription Plan & Limits with Change Plan / Manage Custom Limits,
    // Account Status select, Subscription Dates, Address Lookup, Recent Activity table). vals == null: first render.
    static readonly string[] EF = { "ed_first", "ed_middle", "ed_last", "ed_email", "ed_phone", "ed_mobile", "ed_status", "ed_plan", "ed_biz", "ed_users", "ed_start", "ed_end", "ed_addr" };
    static string D(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToDateTime(o, CultureInfo.InvariantCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); }
    static string[] Current(DataRow r)
    {
        return new[] { SA.S(r["FirstName"]).Trim(), SA.S(r["MiddleName"]).Trim(), SA.S(r["LastName"]).Trim(), SA.S(r["Email"]).Trim(), SA.S(r["MainPhone"]), SA.S(r["MobilePhone"]), EditStatusOf(r), SA.S(r["PlanName"]), SA.S(r["PlanBiz"]), SA.S(r["PlanUsers"]), D(r["PlanStart"]), D(r["NextBilling"]), AD.Compose(SubAddr(r)) };
    }
    // v51 (held note): Edit Subscriber address uses the Reseller Address Lookup module (AnyminuteSupportAddress), read from / written to
    // the subscriber's EmpAddress row the same way Add Subscriber writes it (StreetNo, StreetName = Line 1, StreetType = Line 2, SuitType, SuiteNumber).
    static string Clean(object o) { string s = SA.S(o).Trim(); return s.Equals("Please Select", StringComparison.OrdinalIgnoreCase) || s.Equals("Select", StringComparison.OrdinalIgnoreCase) ? "" : s; }
    static AD.Addr SubAddr(DataRow r)
    {
        string pc = Clean(r["Postalcode"]), k = Clean(r["ACountry"]), prov = Clean(r["Province"]);
        string country = k == "United States" || k == "USA" || k == "US" ? "United States" : k.Length == 0 && System.Text.RegularExpressions.Regex.IsMatch(pc, @"^\d{5}(-\d{4})?$") ? "United States" : "Canada";
        string pm = (country == "United States" ? AD.States : AD.Canada).FirstOrDefault(x => x.Equals(prov, StringComparison.OrdinalIgnoreCase)) ?? prov;
        string st = Clean(r["ASuitType"]); st = AD.SuiteTypes.FirstOrDefault(x => x.Equals(st, StringComparison.OrdinalIgnoreCase)) ?? "";
        var a = new AD.Addr { StreetNo = Clean(r["AStreetNo"]), Line1 = Clean(r["AStreetName"]), Line2 = Clean(r["AStreetType"]), SuiteType = st, SuiteNo = Clean(r["ASuiteNo"]), City = Clean(r["City"]), Province = pm, Country = country };
        a.Postal = AD.NormalPostal(pc, country); if (!a.Any) a.SuiteType = "";
        return a;
    }
    static readonly string[] EL = { "First Name", "Middle Name", "Last Name", "Email Address", "Contact Phone", "Mobile Phone", "Account Status", "Plan", "Max Businesses", "Max Employees", "Start Date", "Next Billing Date", "Address" };
    // Plan catalogue (AnyminuteSubscriptionPlan) for the plan selects; null when the catalogue table is missing.
    static DataTable Plans(SqlConnection c)
    {
        try { return SA.Has(c, "AnyminuteSubscriptionPlan") ? SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan ORDER BY PlanID") : null; } catch (SqlException) { return null; }
    }
    static DataRow PlanRow(DataTable p, string name) { if (p == null || String.IsNullOrEmpty(name)) return null; string k = SA.PlanKey(name); return p.Rows.Cast<DataRow>().FirstOrDefault(x => SA.PlanKey(SA.S(x["PlanName"])) == k); }
    static string PlanLabel(string name) { var t = CultureInfo.InvariantCulture.TextInfo; return t.ToTitleCase(name.Trim().ToLowerInvariant()); }
    static string Lim(DataRow p, string col) { return p == null || !p.Table.Columns.Contains(col) ? "" : SA.S(p[col]); }
    // <option value='BASIC PLAN' data-b data-e data-p> (empty limit = Custom, editable).
    static string PlanOptions(DataTable p, string sel, string placeholder)
    {
        var sb = new StringBuilder(placeholder == null ? "" : "<option value=''>" + placeholder + "</option>"); bool found = false;
        if (p != null) foreach (DataRow x in p.Rows)
        {
            if (p.Columns.Contains("IsActive") && SA.S(x["IsActive"]) == "False" && SA.PlanKey(SA.S(x["PlanName"])) != SA.PlanKey(sel)) continue;
            string v = SA.PlanKey(SA.S(x["PlanName"])) + " PLAN"; bool on = sel.Length > 0 && SA.PlanKey(sel) == SA.PlanKey(v); found |= on;
            sb.Append("<option value='").Append(SA.A(v)).Append("' data-b='").Append(SA.A(Lim(x, "MaxBusinesses"))).Append("' data-e='").Append(SA.A(Lim(x, "MaxEmployees"))).Append("' data-p='").Append(SA.A(Lim(x, "MaxProjects"))).Append("'")
              .Append(on ? " selected" : "").Append(">").Append(SA.H(PlanLabel(SA.S(x["PlanName"])))).Append("</option>");
        }
        if (!found && sel.Length > 0) sb.Append("<option value='").Append(SA.A(sel)).Append("' selected>").Append(SA.H(PlanLabel(sel))).Append("</option>");
        return sb.ToString();
    }
    string Edit(SqlConnection c, DataRow r, string[] vals)
    {
        int u = Convert.ToInt32(r["UserId"]); var v = vals ?? Current(r); bool plan = r["PlanRowId"] != DBNull.Value; string b = "SupportSubscribers.aspx?user=" + u, sts = StatusOf(r);
        int[] n; try { n = Counts(c, r); } catch (SqlException) { n = new[] { 0, 0, 0 }; }
        var plans = Plans(c); string proj = Lim(PlanRow(plans, v[7]), "MaxProjects"); bool planChanged = SA.PlanKey(v[7]) != SA.PlanKey(SA.S(r["PlanName"]));
        string planName = SA.S(r["PlanName"]).Length > 0 ? SA.H(PlanLabel(SA.S(r["PlanName"]))) : "No active plan";
        HeadActions.Text = "<div class='sa-sd-btns'><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=reset'>" + Ic("mail") + " Send Password Reset</a>" + MoreMenu(r, false) + "</div>";
        Func<int, string, string, bool, string> f = (i, type, extra, req) => "<label class='sa-field'><span>" + EL[i] + (req ? " <em class='sa-req'>*</em>" : "") + "</span><input type='" + type + "' name='" + EF[i] + "' value='" + SA.A(v[i]) + "'" + extra + (req ? " required" : "") + " /></label>";
        Func<string, string, string, string> meta = (icon, k, val) => "<div class='sa-meta'>" + Ic(icon) + "<div><small>" + k + "</small><strong>" + val + "</strong></div></div>";
        var sb = new StringBuilder("<input type='hidden' name='sa_user' value='" + u + "' />");
        // v45 PPT 01 s6: header card (avatar, name + status, "Enterprise Plan | Subscriber since", Subscriber ID / Activation / Next Billing with icons).
        sb.Append("<div class='sa-card sa-sd sa-ed-head'><div class='sa-sd-top'><span class='sa-avatar sa-avatar-lg'>").Append(SA.H(Initials(Name(r)))).Append("</span><div class='sa-grow'><h2>").Append(SA.H(Name(r))).Append(" ").Append(SA.Pill(sts, Tone(sts))).Append("</h2><div class='sa-sd-sub'>")
          .Append(planName).Append("<span class='sa-vsep' aria-hidden='true'></span>Subscriber since ").Append(SA.Day(r["Since"])).Append("</div></div><div class='sa-metas'>")
          .Append(meta("id", "Subscriber ID", Sub(u))).Append(meta("cal", "Activation Date", SA.S(r["Since"]).Length > 0 ? SA.Day(r["Since"]) : "&mdash;")).Append(meta("cal", "Next Billing Date", SA.S(r["NextBilling"]).Length > 0 ? SA.Day(r["NextBilling"]) : "&mdash;")).Append("</div></div></div>");
        sb.Append("<div class='sa-card'><h2>Personal Information</h2><div class='sa-grid3'>").Append(f(0, "text", " maxlength='150'", true)).Append(f(1, "text", " maxlength='50'", false)).Append(f(2, "text", " maxlength='50'", true)).Append("</div></div>")
          .Append("<div class='sa-card'><h2>Contact Information</h2><div class='sa-grid3'>").Append(f(3, "email", " maxlength='50'", true)).Append(f(4, "tel", " maxlength='30'", true)).Append(f(5, "tel", " maxlength='30'", false)).Append("</div></div>");
        sb.Append("<div class='sa-card' id='plan'><div class='sa-panel-head'><h2>Subscription Plan &amp; Limits</h2>").Append(plan ? "<button type='button' class='sa-btn sa-btn-outline' data-sa-reveal='sa-plan-pick'>" + Ic("swap") + " Change Plan</button>" : "").Append("</div>");
        if (!plan) sb.Append("<p class='sa-muted'>No active plan record, so plan, limits and dates cannot be changed here.</p><input type='hidden' name='ed_plan' value='").Append(SA.A(v[7])).Append("' />");
        else
        {
            sb.Append("<div class='sa-plan-row'><div class='sa-plan-cur'><small>Current Plan</small><div class='sa-plan-name'><strong>").Append(planName).Append("</strong> <a class='sa-link' href='SupportPlans.aspx'>View Plan Details</a></div>")
              .Append("<label class='sa-field' id='sa-plan-pick'").Append(planChanged ? "" : " hidden").Append("><span>New Plan</span><select name='ed_plan' data-sa-plan='1' data-sa-proj='ed_proj'>").Append(PlanOptions(plans, v[7], null)).Append("</select></label></div>")
              .Append("<div class='sa-plan-use'>").Append(Tile("biz", "blue", "Businesses", n[0], v[8])).Append(Tile("users", "green", "Employees", n[1], v[9])).Append(Tile("folder", "purple", "Projects", n[2], proj)).Append("</div></div>")
              .Append("<div class='sa-limits45' data-sa-limits='1'>").Append(f(8, "number", " min='0' max='100000' readonly data-sa-lim='b'", false)).Append(f(9, "number", " min='0' max='100000' readonly data-sa-lim='e'", false))
              .Append("<label class='sa-field'><span>Max Projects</span><input id='ed_proj' value='").Append(SA.A(proj.Length > 0 ? proj : "Custom")).Append("' readonly /></label>")
              .Append("<button type='button' class='sa-btn sa-btn-outline' data-sa-unlock='1'>").Append(Ic("gear")).Append(" Manage Custom Limits</button></div>");
        }
        sb.Append("</div><div class='sa-card' id='sa-status'><h2>Account Status</h2><div class='sa-status45'><label class='sa-field'><span>Status <em class='sa-req'>*</em></span><select name='ed_status' class='sa-status-sel")
          .Append(v[6] == "Active" ? " on" : "").Append("' onchange=\"this.className='sa-status-sel'+(this.value=='Active'?' on':'')\">");
        // v47: all four states (was Active / Inactive only, so a Pending subscriber showed "Active" and saving activated it).
        foreach (var o in AStatus) sb.Append("<option").Append(v[6] == o ? " selected" : "").Append(">").Append(o).Append("</option>");
        sb.Append("</select></label><div class='sa-flash sa-info'>").Append(Ic("info")).Append("<span>Suspended or inactive accounts cannot sign in to the system.<br />You can send an activation invitation to pending accounts.</span></div></div></div>")
          .Append("<div class='sa-card'><h2>Subscription Dates</h2>").Append(plan ? "<div class='sa-grid2'>" + f(10, "date", "", true) + f(11, "date", "", true) + "</div>" : "<p class='sa-muted'>No active plan record.</p>").Append("</div>");
        // v53 (NOTES_v53 #4, img_02 -> img_01): collapsed Address card (current address or Please Select + Manage Address);
        // Manage Address reveals the same Address Lookup + Address Details as Edit Reseller in place (no SupportAddresses.aspx).
        { bool aOpen = IsPostBack && AD.Opened("ed_addr"); var sa0 = SubAddr(r);
          sb.Append(AD.Gate("ed_addr", AD.Compose(sa0), aOpen, "<div class='sa-card sa-sub-addr'><h2>Address <span class='sa-muted sa-small'>(Optional)</span></h2>" + AD.Render("ed_addr", aOpen ? AD.FromForm("ed_addr") : sa0, false, "SupportSubscribers.aspx?lookup=1&postal=", "Search for the subscriber address using a postal or ZIP code, select a result, or enter the address manually.") + "</div>")); }
        sb.Append("<div class='sa-card'><div class='sa-panel-head'><h2>Recent Activity <span class='sa-light'>(Last 5)</span></h2><a class='sa-btn sa-btn-outline sa-btn-sm' href='").Append(b).Append("&amp;view=history'>").Append(Ic("open")).Append(" View Full History</a></div>");
        DataTable h = null; try { if (SA.Has(c, "AnyminuteSupportAudit")) h = SA.Q(c, "SELECT TOP 5 * FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k ORDER BY ChangedAtUTC DESC,AuditID DESC", "@k", u.ToString(CultureInfo.InvariantCulture)); } catch (SqlException) { }
        sb.Append("<div class='sa-table-wrap'><table class='sa-table sa-act-table'><thead><tr><th>Date &amp; Time</th><th>Changed By</th><th>Field</th><th>Previous Value</th><th>New Value</th><th>Reason</th></tr></thead><tbody>");
        if (h == null || h.Rows.Count == 0) sb.Append("<tr><td colspan='6' class='sa-empty'>No activity recorded yet.</td></tr>");
        else foreach (DataRow x in h.Rows)
        {
            Func<string, string, string> pv = (a, z) => { string s = SA.S(x[a]).Length > 0 ? SA.S(x[a]) : SA.S(x[z]); return s.Length > 0 ? SA.H(s) : "&mdash;"; };
            sb.Append("<tr><td>").Append(SA.When(x["ChangedAtUTC"]).Replace("<br />", " ")).Append("</td><td>").Append(SA.S(x["ChangedByName"]).Length > 0 ? SA.H(x["ChangedByName"]) : "&mdash;").Append("</td><td>").Append(SA.H(x["Action"])).Append("</td><td>")
              .Append(pv("PrevValue", "PrevStatus")).Append("</td><td>").Append(pv("NewValue", "NewStatus")).Append("</td><td>").Append(SA.S(x["Comments"]).Length > 0 ? SA.H(x["Comments"]) : "&mdash;").Append("</td></tr>");
        }
        sb.Append("</tbody></table></div></div><div class='sa-panel-foot sa-ed-foot'><a class='sa-btn sa-btn-outline' href='").Append(b).Append("'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='edit'>").Append(Ic("save")).Append(" Save Changes</button></div>");
        return sb.ToString();
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
    // v51 PPT s2: Support History tab - this subscriber's tickets (Ticket.EmailId = subscriber email) inside the Subscriber Details shell.
    // Opening one ticket still uses the Support Tickets detail panel (reply / assign / status actions live there).
    static string Support(SqlConnection c, DataRow r)
    {
        int user = Convert.ToInt32(r["UserId"]); string email = SA.S(r["Email"]).Trim();
        string q = SA.Get("q"), status = SA.Get("status"), prio = SA.Get("priority"), from = SA.Get("from"), to = SA.Get("to");
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        DateTime f, t; bool hf = DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out f), ht = DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
        List<DataRow> all;
        try
        {
            bool meta = SA.Has(c, "AnyminuteTicketMeta");
            all = SA.Q(c, "SELECT t.TicketId,t.TicketNo,t.Description,t.StatusType,t.TicketSeverity,t.ProblemType,t.Module,t.TicketRaiseDate," + (meta ? "m.Subject,m.Category,m.Priority,m.AssignedTo,ISNULL(m.Archived,0) Archived" : "CAST(NULL AS nvarchar(200)) Subject,CAST(NULL AS nvarchar(60)) Category,CAST(NULL AS varchar(10)) Priority,CAST(NULL AS nvarchar(150)) AssignedTo,CAST(0 AS bit) Archived")
                + ",(SELECT MAX(k.CommentsDate) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) LastComment FROM Ticket t" + (meta ? " LEFT JOIN AnyminuteTicketMeta m ON m.TicketId=t.TicketId" : "") + " WHERE LTRIM(RTRIM(t.EmailId))=@m ORDER BY t.TicketId DESC", "@m", email).Rows.Cast<DataRow>().ToList();
        }
        catch (SqlException ex) { return "<div class='sa-card'><p class='sa-muted'>Could not load support history: " + SA.H(ex.Message) + "</p></div>"; }
        Func<DataRow, string> subj = x => { string s = SA.S(x["Subject"]); if (s.Length > 0) return s; s = SA.S(x["Description"]).Replace("\r", " ").Replace("\n", " "); return s.Length > 48 ? s.Substring(0, 45) + "..." : (s.Length == 0 ? "(no subject)" : s); };
        Func<DataRow, string> prOf = x => { string s = SA.S(x["Priority"]); if (s.Length == 0) s = SA.S(x["TicketSeverity"]); return SA.Priorities.FirstOrDefault(p => p.Equals(s, StringComparison.OrdinalIgnoreCase)) ?? "Medium"; };
        Func<DataRow, string> catOf = x => { string s = SA.S(x["Category"]); return s.Length > 0 ? s : SA.LegacyCategory(x["ProblemType"], x["Module"]); };
        var live = all.Where(x => !Convert.ToBoolean(x["Archived"])).ToList();
        var rows = live.Where(x =>
        {
            if (status.Length > 0 && SA.StatusLabel(x["StatusType"]) != status) return false;
            if (prio.Length > 0 && prOf(x) != prio) return false;
            if (q.Length > 0 && (SA.S(x["TicketNo"]) + " " + subj(x) + " " + SA.S(x["Description"])).IndexOf(q, StringComparison.OrdinalIgnoreCase) < 0) return false;
            if (x["TicketRaiseDate"] != DBNull.Value) { var d = Convert.ToDateTime(x["TicketRaiseDate"]).Date; if (hf && d < f) return false; if (ht && d > t) return false; }
            return true;
        }).ToList();
        Func<string, string, int, string> kpi = (label, tone, n) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>&#9742;</i><div><span>" + label + "</span><strong>" + n + "</strong></div></div>";
        var sb = new StringBuilder("<div class='sa-tabpad'><div class='sa-panel-head sa-biz-head'><div><h2>Support History</h2><small>Support tickets raised by this subscriber.</small></div><a class='sa-btn' href='SupportTickets.aspx?create=1'>+ Create Ticket</a></div>");
        sb.Append("<div class='sa-kpis'>").Append(kpi("Total Tickets", "blue", live.Count)).Append(kpi("Open Tickets", "green", live.Count(x => SA.StatusLabel(x["StatusType"]) == "Open")))
          .Append(kpi("In Progress", "amber", live.Count(x => SA.StatusLabel(x["StatusType"]) == "In Progress"))).Append(kpi("Resolved", "purple", live.Count(x => SA.StatusLabel(x["StatusType"]) == "Resolved"))).Append("</div>");
        sb.Append("<div class='sa-filters' data-sa-get='SupportSubscribers.aspx'><input type='hidden' name='user' value='").Append(user).Append("' /><input type='hidden' name='view' value='support' />")
          .Append("<label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by ticket number or subject...' /></label>")
          .Append("<label><span>Status</span><select name='status'>").Append(SA.Options(SA.TicketStatuses, status, "All Statuses")).Append("</select></label>")
          .Append("<label><span>Priority</span><select name='priority'>").Append(SA.Options(SA.Priorities, prio, "All Priorities")).Append("</select></label>")
          .Append("<label class='sa-range'><span>Date Range</span><span><input type='date' name='from' value='").Append(SA.A(from)).Append("' aria-label='From' /> &ndash; <input type='date' name='to' value='").Append(SA.A(to)).Append("' aria-label='To' /></span></label>")
          .Append("<button type='button' class='sa-btn' data-sa-apply='1'>&#128269; Search</button><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(user).Append("&amp;view=support'>&#8635; Clear</a></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Ticket No.</th><th>Subject</th><th>Category</th><th>Priority</th><th>Status</th><th>Created Date</th><th>Last Updated</th><th>Assigned To</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        foreach (var x in rows.Skip((page - 1) * PageSize).Take(PageSize))
        {
            int tid = Convert.ToInt32(x["TicketId"]); string st = SA.StatusLabel(x["StatusType"]), pr = prOf(x), no = SA.S(x["TicketNo"]).Length > 0 ? SA.S(x["TicketNo"]) : "TK-" + tid, href = "SupportTickets.aspx?id=" + tid + "&amp;panel=details";
            sb.Append("<tr><td><a class='sa-link' href='").Append(href).Append("'>#").Append(SA.H(no.TrimStart('#'))).Append("</a></td><td>").Append(SA.H(subj(x))).Append("</td><td>").Append(SA.H(catOf(x))).Append("</td><td>")
              .Append(SA.Pill(pr, pr == "High" ? "red" : pr == "Medium" ? "amber" : "green")).Append("</td><td>").Append(SA.Pill(st, SA.StatusTone(st))).Append("</td><td>").Append(x["TicketRaiseDate"] == DBNull.Value ? "&mdash;" : SA.Day(x["TicketRaiseDate"]))
              .Append("</td><td>").Append(x["LastComment"] == DBNull.Value ? "&mdash;" : SA.Day(x["LastComment"])).Append("</td><td>").Append(SA.S(x["AssignedTo"]).Length > 0 ? SA.H(x["AssignedTo"]) : "<span class='sa-muted'>Unassigned</span>")
              .Append("</td><td class='sa-c'><a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(href).Append("'>View</a></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='9' class='sa-empty'>No support tickets for this subscriber.</td></tr>");
        string bq = "user=" + user + "&view=support&q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(status) + "&priority=" + HttpUtility.UrlEncode(prio) + "&from=" + from + "&to=" + to;
        return sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, bq).Replace(" of " + rows.Count + "</span>", " of " + rows.Count + " tickets</span>")).Append("</div>").ToString();
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
            .Append(kpi("Businesses used / plan limit", n[0], act == null ? "" : SA.S(act["Business"]), "Allowed by the current plan"))
            .Append(kpi("Employees used / plan limit", n[1], act == null ? "" : SA.S(act["No_Of_Users"]), "Allowed by the current plan"))
            .Append(kpi("Projects used / plan limit", n[2], cat == "&mdash;" ? "" : cat, "Included in the plan"))
            .Append("</div><div class='sa-card'><h2>Plan History</h2><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Plan</th><th>Start</th><th>End</th><th>Amount Paid</th><th>Employees Limit</th><th>Business Limit</th><th>Status</th></tr></thead><tbody>");
        foreach (DataRow x in plans.Rows)
        {
            bool on = SA.S(x["IsActive"]) == "1" || SA.S(x["IsActive"]) == "True", cancelled = SA.S(x["Is_Cancelled"]) == "1" || SA.S(x["Is_Cancelled"]) == "True";
            sb.Append("<tr><td><strong>").Append(SA.H(x["PlanName"])).Append("</strong></td><td>").Append(SA.Day(x["StartDate"])).Append("</td><td>").Append(SA.Day(x["EndDate"])).Append("</td><td>").Append(SA.S(x["AmountPaid"]).Length > 0 ? "$" + SA.H(x["AmountPaid"]) : "&mdash;")
              .Append("</td><td>").Append(SA.H(x["No_Of_Users"])).Append("</td><td>").Append(SA.H(x["Business"])).Append("</td><td>").Append(cancelled ? SA.Pill("Cancelled", "red") : on ? SA.Pill("Active", "green") : SA.Pill("Inactive", "grey")).Append("</td></tr>");
        }
        if (plans.Rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No plan history for this subscriber.</td></tr>");
        return sb.Append("</tbody></table></div><p class='sa-muted'><a class='sa-link' href='SupportPlans.aspx?view=usage'>Usage &amp; Limits for all plans</a></p></div>").ToString();
    }
    string Access(SqlConnection c, DataRow r, bool ready)
    {
        int u = Convert.ToInt32(r["UserId"]); bool en = Enabled(r), first = r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]);
        string lastReset = "&mdash;";
        if (SA.Has(c, "AnyminuteSupportAudit")) { var lr = SA.Q(c, "SELECT TOP 1 ChangedAtUTC FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k AND Action='Password Reset' ORDER BY ChangedAtUTC DESC", "@k", u.ToString(CultureInfo.InvariantCulture)); if (lr.Rows.Count == 1) lastReset = SA.When(lr.Rows[0][0]).Replace("<br />", " "); }
        string dis = ready ? "" : " disabled";
        var sb = new StringBuilder("<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-grid2'><div class='sa-card'><h2>Current Login Information</h2><table class='sa-kv'>");
        sb.Append("<tr><th>Account Status</th><td>").Append(SA.Pill(StatusOf(r), Tone(StatusOf(r)))).Append("</td></tr>")
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
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=").Append(u).Append("'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='access'").Append(dis).Append(">&#128190; Save Changes</button></div></div>");
        return sb.ToString();
    }
    static string Sessions(SqlConnection c, int u)
    {
        DataTable t; try { t = AnyminuteSessionGuard.Active(c, u); } catch (SqlException) { t = null; }
        if (t == null) return "<span class='sa-muted'>Not tracked (run REQUIRED_08)</span>";
        if (t.Rows.Count == 0) return "0";
        var r = t.Rows[0];
        if (t.Rows.Count == 1) return "1 (Current session)"; // v50 PPT s1
        return t.Rows.Count + " <small class='sa-muted'>(last activity " + SA.When(r["LastSeenAtUTC"]).Replace("<br />", " ") + (SA.S(r["IpAddress"]).Length > 0 ? " from " + SA.H(r["IpAddress"]) : "") + ")</small>";
    }
    static string Reset(DataRow r, bool ready)
    {
        int u = Convert.ToInt32(r["UserId"]); string email = SA.S(r["Email"]), first = Name(r).Split(' ')[0];
        return "<input type='hidden' name='sa_user' value='" + u + "' /><div class='sa-card sa-steps'>"
            + "<section><h2><b class='sa-step'>1</b> Subscriber Information</h2><div class='sa-subhead sa-flat'><span class='sa-avatar'>" + SA.H(Initials(Name(r))) + "</span><div class='sa-grow'><strong>" + SA.H(Name(r)) + "</strong><small>Subscriber ID: " + Sub(u) + "<br />" + SA.H(email) + "</small></div>"
            + "<div><small>Account Status</small>" + SA.Pill(StatusOf(r), Tone(StatusOf(r))) + "</div><div><small>Login Access</small>" + (Enabled(r) ? SA.Pill("Enabled", "green") : SA.Pill("Disabled", "red")) + "</div><div><small>Last Login</small><strong>" + SA.When(r["LastLoginDate"]) + "</strong></div></div></section>"
            + "<section><h2><b class='sa-step'>2</b> Password Reset Email</h2><div class='sa-grid2'><div><label class='sa-field'><span>Send to Email Address</span><input type='email' name='sa_email' maxlength='256' required value='" + SA.A(email) + "' oninput=\"document.getElementById('saRstTo').textContent=this.value\" /></label><small class='sa-muted'>A secure password reset link will be sent to this email address.</small></div>"
            + "<div class='sa-preview'><small>Message Preview (Email Subject and Content)</small><p><b>Subject: Reset Your AnyMinute Password</b></p><p>Hello " + SA.H(first) + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password. This link will expire in 24 hours.</p><p><span class='sa-link'>Reset Your Password</span></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p></div></div></section>"
            + "<section><h2><b class='sa-step'>3</b> Confirm and Send</h2><div class='sa-flash sa-warn'><b>Please confirm before sending.</b><ul><li>A secure password reset link will be sent to <span id='saRstTo'>" + SA.H(email) + "</span>.</li><li>The link will expire in 24 hours.</li><li>The previous password will remain invalid after the reset.</li><li>This action will be recorded in the activity log.</li></ul></div></section>"
            + "<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx?user=" + u + "'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='reset'" + (ready ? "" : " disabled") + ">&#10148; Send Password Reset</button></div></div>";
    }
    string SubName = "";
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
            if (ev.Length > 0 && Ev(r) != ev) return false;
            bool ok = r["Success"] == DBNull.Value || Convert.ToBoolean(r["Success"]);
            if (st == "Success" && !ok) return false; if (st == "Failed" && ok) return false;
            if (ip.Length > 0 && !SA.S(r["IpAddress"]).Contains(ip)) return false;
            if (dev.Length > 0 && SA.S(r["Device"]).IndexOf(dev, StringComparison.OrdinalIgnoreCase) < 0) return false;
            return true;
        }).ToList();
        var events = MockEvents.Concat(all.Select(Ev)).Distinct().OrderBy(x => x);
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
            string by = SA.S(r["ChangedByName"]); bool self = by == "Subscriber"; string evn = Ev(r);
            sb.Append("<tr><td>").Append(SA.When(r["ChangedAtUTC"])).Append("</td><td><strong>").Append(EvIcon(evn)).Append(SA.H(evn)).Append("</strong></td><td>").Append(SA.H(r["Comments"])).Append("</td><td>").Append(SA.S(r["IpAddress"]).Length > 0 ? SA.H(r["IpAddress"]) : "&mdash;")
              .Append("</td><td>").Append(Location(c, r, locCol, ref budget)).Append("</td><td>").Append(SA.S(r["Device"]).Length > 0 ? SA.H(r["Device"]) : "&mdash;").Append("</td><td>").Append(self ? SA.H(SubName) + "<br /><small>(Subscriber)</small>" : "Support Admin<br /><small>(" + SA.H(by) + ")</small>")
              .Append("</td><td>").Append(SA.Pill(ok ? "Success" : "Failed", ok ? "green" : "red")).Append("</td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='8' class='sa-empty'>No login activity recorded yet. Sign-ins are recorded from v25 onward.</td></tr>");
        string baseQ = "user=" + user + "&view=history&from=" + from + "&to=" + to + "&event=" + HttpUtility.UrlEncode(ev) + "&status=" + st + "&ip=" + HttpUtility.UrlEncode(ip) + "&device=" + HttpUtility.UrlEncode(dev);
        return sb.Append("</tbody></table></div>").Append(SA.Pager(rows.Count, page, PageSize, baseQ).Replace(" of " + rows.Count + "</span>", " of " + rows.Count + " activities</span>")).Append("</div>").ToString();
    }
    // v50 PPT s3 event labels: the subscriber-created audit row reads "Account Created" on this page.
    static readonly string[] MockEvents = { "Sign In", "Password Reset", "Failed Sign In", "Activation Email", "Account Created", "Login Access Enabled" };
    static string Ev(DataRow r) { string a = SA.S(r["Action"]); return a == "Subscriber Created" ? "Account Created" : a; }
    static string EvIcon(string e)
    {
        string i = e == "Sign In" ? "&#8680;" : e == "Failed Sign In" ? "&#128274;" : e == "Password Reset" ? "&#128273;" : e == "Activation Email" ? "&#9993;" : e == "Account Created" ? "&#128100;" : e.StartsWith("Login Access") ? "&#128275;" : "";
        return i.Length == 0 ? "" : "<span class='sa-ev-ico' style='color:#1a73e8' aria-hidden='true'>" + i + "</span> ";
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
            if (op == "bizsave") { Title1.Text = ""; Crumbs.Text = CrumbBar(CrumbSep("<a href='SupportSubscribers.aspx?user=" + u + "'>Subscriber Details</a>") + CrumbSep("<span>Edit Business</span>")); BizPost(c, r); return; }
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
                    // v50 PPT s2: "Send to Email Address" is editable; default is the subscriber's sign-in email.
                    string to = SA.Form("sa_email").Trim();
                    if (to.Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(to, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) { SA.Flash("Enter a valid email address to send the reset link to."); Back(u, "reset"); return; }
                    if (to.Length > 0) email = to;
                    // v26: one-time token, valid 24 hours (REQUIRED_08). Without the table the legacy link is refused rather than sent unprotected.
                    if (!AnyminutePasswordReset.Ready(c)) { SA.Flash("Run DB_REQUIRED/REQUIRED_08_SupportAdminStrict.sql to enable 24-hour password reset links. Nothing was sent."); Back(u, "reset"); return; }
                    url += "&rt=" + AnyminutePasswordReset.Issue(c, null, u, linkEmployee, email, SA.ActorId());
                    string body = "<p>Hello " + first + ",</p><p>We received a request to reset your AnyMinute account password. Click the link below to set a new password. This link will expire in 24 hours.</p><p><a href=\"" + HttpUtility.HtmlAttributeEncode(url) + "\">Reset Your Password</a></p><p>If you did not request this, please ignore this email.</p><p>Thank you,<br />The AnyMinute Team</p>";
                    mail.SendHtmlFormattedEmail(email, "Reset Your AnyMinute Password", body);
                    SA.Audit(c, null, "Login", key, "Password Reset", null, null, null, email, "Password reset link sent to subscriber (expires in 24 hours, one-time use).");
                    SA.Flash("Password reset link sent to " + email + ".");
                }
                else
                {
                    // v45: the invitation carries a sign-in password. A still-pending account gets a NEW temporary password (shown once here);
                    // an active account keeps its password (the email says to use it or Forgot Password).
                    string pw = StatusOf(r) == "Pending" ? AnyminuteSubscriberCreate.SetTempPassword(c, u, "Temporary password re-issued with the resent activation invitation.") : null;
                    string sent = AnyminuteSubscriberCreate.SendInvitation(c, u, pw);
                    SA.Flash((sent ?? "Invitation email sent to " + email + (pw != null ? " with a new temporary password." : ".")) + (pw != null ? " Temporary password (shown once): " + pw : ""));
                }
                Back(u, null); return;
            }
        }
        Back(u, null);
    }
    // v43 PPT 01 s5 Add Subscriber: form -> Confirm and Send Activation Invitation -> busy overlay -> Subscriber Created Successfully!
    static readonly string[] AF = { "ad_first", "ad_middle", "ad_last", "ad_email", "ad_phone", "ad_mobile", "ad_plan", "ad_biz", "ad_users", "ad_status", "ad_invite", "ad_start", "ad_end",
        // v46 (Address deck s2): Address module fields (13-21) replace the old bottom Street / City / Province / Postal / Country block.
        "ad_addr_no", "ad_addr_line1", "ad_addr_line2", "ad_addr_suitetype", "ad_addr_suiteno", "ad_addr_city", "ad_addr_prov", "ad_addr_postal", "ad_addr_country" };
    static AnyminuteSupportAddress.Addr AddrOf(string[] v)
    {
        var a = new AnyminuteSupportAddress.Addr { StreetNo = v[13], Line1 = v[14], Line2 = v[15], SuiteType = AnyminuteSupportAddress.SuiteTypes.Contains(v[16]) ? v[16] : "", SuiteNo = v[17], City = v[18], Province = v[19], Country = v[21] == "United States" ? "United States" : "Canada" };
        a.Postal = AnyminuteSupportAddress.NormalPostal(v[20], a.Country); return a;
    }
    static readonly string[] AStatus = { "Pending Activation", "Active", "Suspended", "Inactive" };
    static readonly string[] Provinces = { "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland and Labrador", "Northwest Territories", "Nova Scotia", "Nunavut", "Ontario", "Prince Edward Island", "Quebec", "Saskatchewan", "Yukon" };
    void AddHead()
    {
        Title1.Text = "Add Subscriber"; Lead.Text = "Create a new subscriber account and send an activation invitation.";
        Crumbs.Text = "<nav class='sa-crumbs'><a href='SupportSubscribers.aspx'>Subscribers</a> &rsaquo; <span>Add Subscriber</span></nav>";
    }
    void AddView(SqlConnection c, string[] v)
    {
        AddHead();
        if (v == null) { v = new string[AF.Length]; for (int i = 0; i < v.Length; i++) v[i] = ""; v[9] = AStatus[0]; v[10] = "1"; v[11] = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); v[12] = DateTime.Today.AddYears(1).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture); v[21] = "Canada"; }
        var plans = Plans(c); var pr = PlanRow(plans, v[6]); string pb = Lim(pr, "MaxBusinesses"), pe = Lim(pr, "MaxEmployees"), pp = Lim(pr, "MaxProjects"); bool custom = pr != null && (pb.Length == 0 || pe.Length == 0);
        Func<string, int, string, string, bool, string> f = (label, i, type, extra, req) => "<label class='sa-field'><span>" + label + (req ? " *" : "") + "</span><input type='" + type + "' name='" + AF[i] + "' value='" + SA.A(v[i]) + "'" + extra + (req ? " required" : "") + " /></label>";
        var sb = new StringBuilder("<div class='sa-card sa-add'>");
        sb.Append("<section><h2>Personal Information</h2><div class='sa-grid3'>").Append(f("First Name", 0, "text", " maxlength='50' placeholder='Enter first name'", true)).Append(f("Middle Name", 1, "text", " maxlength='50' placeholder='Enter middle name'", false)).Append(f("Last Name", 2, "text", " maxlength='50' placeholder='Enter last name'", true)).Append("</div></section>")
          .Append("<section><h2>Contact Information</h2><div class='sa-grid3'>").Append(f("Email Address", 3, "email", " maxlength='50' placeholder='Enter email address'", true)).Append(f("Contact Phone", 4, "tel", " maxlength='30' placeholder='Enter contact phone'", true)).Append(f("Mobile Phone", 5, "tel", " maxlength='30' placeholder='Enter mobile phone'", false)).Append("</div></section>");
        sb.Append("<section><h2>Subscription Plan</h2><div class='sa-grid2'><div><label class='sa-field'><span>Plan Name *</span><select name='ad_plan' required data-sa-plan='1' data-sa-proj='ad_proj'>").Append(PlanOptions(plans, v[6], "Select plan")).Append("</select></label><a class='sa-link sa-small' href='SupportPlans.aspx' target='_blank'>View Plan Details</a></div>")
          .Append("<div class='sa-plan-auto'><span>Plan Limits (Automatically set)</span><div class='sa-grid3' data-sa-limits='1'>")
          .Append("<label class='sa-field'><span>Businesses</span><input type='number' name='ad_biz' min='0' max='100000' data-sa-lim='b' value='").Append(SA.A(custom ? v[7] : pb)).Append("'").Append(custom ? "" : " readonly").Append(" /></label>")
          .Append("<label class='sa-field'><span>Employees</span><input type='number' name='ad_users' min='0' max='100000' data-sa-lim='e' value='").Append(SA.A(custom ? v[8] : pe)).Append("'").Append(custom ? "" : " readonly").Append(" /></label>")
          .Append("<label class='sa-field'><span>Projects</span><input id='ad_proj' value='").Append(SA.A(pr == null ? "" : pp.Length > 0 ? pp : "Custom")).Append("' readonly /></label></div><small class='sa-muted'>Limits come from the selected plan. Custom plans need the limits entered.</small></div></div></section>");
        sb.Append("<section><h2>Account Activation</h2><div class='sa-grid2'><div><span class='sa-label'>Subscriber Status *</span><div class='sa-radios'>");
        foreach (var o in AStatus) sb.Append("<label class='sa-radio'><input type='radio' name='ad_status' value='").Append(o).Append("'").Append(v[9] == o ? " checked" : "").Append(" /> ").Append(o).Append("</label> ");
        sb.Append("</div></div><label class='sa-check sa-check-box'><input type='checkbox' name='ad_invite' value='1'").Append(v[10] == "1" ? " checked" : "").Append(" /> <span><b>Send Activation Invitation</b><small>An invitation email will be sent to the subscriber to set their password.</small></span></label></div></section>")
          .Append("<section><h2>Subscription Dates</h2><div class='sa-grid2'>").Append(f("Start Date", 11, "date", "", true)).Append(f("Next Billing Date", 12, "date", "", true)).Append("</div></section>");
        // v46 (Address deck s2 "Address module in Subscriber - Remove the bottom"): the Business Address Lookup module replaces
        // the old Address (Optional) block. Optional; once started, the required fields must be completed.
        sb.Append("<section class='sa-sub-addr'><h2>Address <span class='sa-muted sa-small'>(Optional)</span></h2>").Append(AnyminuteSupportAddress.Render("ad_addr", AddrOf(v), false, "SupportSubscribers.aspx?lookup=1&postal=", "Search for the subscriber address using a postal or ZIP code, select a result, or enter the address manually.")).Append("</section>");
        sb.Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportSubscribers.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='add'>&#128190; Save Subscriber</button></div></div>");
        Content.Text = sb.ToString();
    }
    void AddPost()
    {
        string op = SA.Form("sa_op"); var v = AF.Select(k => SA.Form(k).Trim()).ToArray();
        if (!AStatus.Contains(v[9])) v[9] = AStatus[0]; v[10] = v[10] == "1" ? "1" : "0";
        using (var c = SA.Open())
        {
            if (op == "addback") { AddView(c, v); return; }
            var plans = Plans(c); var pr = PlanRow(plans, v[6]);
            if (pr != null) { if (Lim(pr, "MaxBusinesses").Length > 0) v[7] = Lim(pr, "MaxBusinesses"); if (Lim(pr, "MaxEmployees").Length > 0) v[8] = Lim(pr, "MaxEmployees"); v[6] = SA.PlanKey(SA.S(pr["PlanName"])) + " PLAN"; }
            var errs = new List<string>(); int nb = 0, ne = 0; DateTime d1, d2;
            if (!SA.Has(c, "AnyminuteSupportAudit")) errs.Add(SA.MissingTables);
            if (v[0].Length == 0) errs.Add("First name is required."); if (v[2].Length == 0) errs.Add("Last name is required.");
            if (!AnyminuteLoginEmail.ValidFormat(v[3])) errs.Add("Enter a valid email address (up to 50 characters).");
            else if (AnyminuteSubscriberCreate.EmailTaken(c, v[3])) errs.Add("This email address is already registered to another AnyMinute account.");
            if (v[4].Length == 0) errs.Add("Contact phone is required.");
            foreach (int i in new[] { 4, 5 }) if (v[i].Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(v[i], @"^[0-9+()\-.\s]{7,30}$")) errs.Add((i == 4 ? "Contact phone" : "Mobile phone") + " is not valid.");
            if (pr == null) errs.Add("Select a plan.");
            else if (!Int32.TryParse(v[7], out nb) || nb < 0 || !Int32.TryParse(v[8], out ne) || ne < 0) errs.Add("Enter the business and employee limits for this plan.");
            bool h1 = DateTime.TryParseExact(v[11], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1), h2 = DateTime.TryParseExact(v[12], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
            if (!h1 || !h2) errs.Add("Enter valid subscription dates."); else if (d2 <= d1) errs.Add("Next Billing Date must be after the Start Date.");
            errs.AddRange(AnyminuteSupportAddress.Validate(AddrOf(v), false));
            if (errs.Count > 0) { Flash.Text = "<div class='sa-flash sa-warn'>Please fix the following:<ul>" + String.Join("", errs.Select(m => "<li>" + SA.H(m) + "</li>").ToArray()) + "</ul></div>"; AddView(c, v); return; }
            var adr = AddrOf(v); v[20] = adr.Postal;
            string addr = AnyminuteSupportAddress.Compose(adr);
            if (op != "addsave")
            {
                AddHead();
                var sb = new StringBuilder();
                for (int i = 0; i < AF.Length; i++) sb.Append("<input type='hidden' name='").Append(AF[i]).Append("' value='").Append(SA.A(v[i])).Append("' />");
                string pp = Lim(pr, "MaxProjects");
                Func<string, string, string> row = (k, val) => "<tr><th>" + k + "</th><td>" + (val.Length > 0 ? val : "&mdash;") + "</td></tr>";
                sb.Append("<div class='sa-modal-back'><div class='sa-modal-box' role='dialog' aria-labelledby='sa-cf-h'><div class='sa-panel-head'><h2 id='sa-cf-h'>Confirm and Send Activation Invitation</h2><button type='submit' class='sa-x' name='sa_op' value='addback' formnovalidate aria-label='Close'>&times;</button></div>")
                  .Append("<p class='sa-muted'>Please review the information below. The subscriber will be created").Append(v[10] == "1" ? " and an activation invitation will be sent." : ".").Append("</p><table class='sa-kv'>")
                  .Append(row("Name", SA.H(String.Join(" ", new[] { v[0], v[1], v[2] }.Where(x => x.Length > 0).ToArray())))).Append(row("Email", SA.H(v[3]))).Append(row("Phone", SA.H(v[4]))).Append(row("Plan", SA.H(PlanLabel(v[6]))))
                  .Append(row("Plan Limits", nb + " Businesses, " + ne + " Employees, " + (pp.Length > 0 ? SA.H(pp) : "Custom") + " Projects")).Append(row("Status", SA.H(v[9]))).Append(row("Send Activation Invitation", v[10] == "1" ? "Yes" : "No"))
                  .Append(row("Start Date", SA.Day(d1))).Append(row("Next Billing Date", SA.Day(d2))).Append(row("Address", SA.H(addr))).Append("</table>")
                  .Append("<div class='sa-panel-foot'><button type='submit' class='sa-btn sa-btn-outline' name='sa_op' value='addback' formnovalidate>Cancel</button><button type='submit' class='sa-btn' name='sa_op' value='addsave' data-sa-busy='1'>")
                  .Append(v[10] == "1" ? "&#9993; Confirm &amp; Send Invitation" : "Confirm &amp; Create").Append("</button></div></div></div>")
                  .Append("<div class='sa-busy' data-sa-busy-box='1' hidden><div class='sa-modal-box'><span class='sa-spin' aria-hidden='true'></span><h2>Creating Subscriber Account</h2><p class='sa-muted'>Please wait while we create the account").Append(v[10] == "1" ? " and send the activation invitation" : "").Append("...</p></div></div>");
                Content.Text = sb.ToString(); return;
            }
            var input = new AnyminuteSubscriberCreate.Input
            {
                First = v[0], Middle = v[1], Last = v[2], Email = v[3], Phone = v[4], Mobile = v[5], PlanName = v[6], Businesses = nb, Employees = ne, Start = d1, NextBilling = d2, Invite = v[10] == "1",
                Status = v[9] == AStatus[0] ? "Pending" : v[9], StreetNo = adr.StreetNo, Street = adr.Line1, Line2 = adr.Line2, SuiteType = adr.SuiteType, SuiteNo = adr.SuiteNo, City = adr.City, Province = adr.Province, Postal = adr.Postal, Country = adr.Any ? adr.Country : ""
            };
            int user;
            try { user = AnyminuteSubscriberCreate.Create(input); }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("Log").Error("Support Add Subscriber failed", ex);
                Flash.Text = "<div class='sa-flash sa-warn'>The subscriber could not be created: " + SA.H(ex.Message) + "</div>"; AddView(c, v); return;
            }
            string sent = null;
            if (input.Invite) { try { sent = AnyminuteSubscriberCreate.SendInvitation(c, user, input.TempPassword); } catch (Exception ex) { sent = "The invitation email could not be sent: " + ex.Message; } }
            // v45: the temporary password is shown once to Support (like the v44 reseller screen) and is in the welcome email.
            SA.Flash((sent != null ? sent + " " : "") + "Sign-in email: " + input.Email + ". Temporary password (shown once): " + input.TempPassword);
            Response.Redirect("SupportSubscribers.aspx?user=" + user + "&view=created&inv=" + (input.Invite && sent == null ? "1" : "0"), false); Context.ApplicationInstance.CompleteRequest();
        }
    }
    // ?lookup=1&postal=: address search for the Add Subscriber form (same lookup as the address pages).
    void Lookup()
    {
        Response.Clear(); Response.ContentType = "application/json"; Response.Write(AnyminuteSupportAddress.LookupJson(SA.Get("postal"))); Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest();
    }
    // v42: null = Login & Access (its own view now), "" = Subscriber Details.
    void Back(int user, string view) { if (view == null) view = "access"; Response.Redirect("SupportSubscribers.aspx" + (user > 0 ? "?user=" + user + (view.Length > 0 ? "&view=" + view : "") : ""), false); Context.ApplicationInstance.CompleteRequest(); }
    // v42/v43 PPT 01 s6: Save Changes -> Confirm Changes (Field / Previous Value / New Value + Reason for Change *) -> Subscriber Updated Successfully!
    void EditPost(SqlConnection c, DataRow r, bool save)
    {
        int u = Convert.ToInt32(r["UserId"]); var cur = Current(r); var nv = EF.Select(k => SA.Form(k).Trim()).ToArray(); bool plan = r["PlanRowId"] != DBNull.Value;
        if (!AStatus.Contains(nv[6])) nv[6] = cur[6]; // v47: unknown / missing value keeps the current status (never silently Active)
        if (!plan) { nv[7] = cur[7]; nv[8] = cur[8]; nv[9] = cur[9]; nv[10] = cur[10]; nv[11] = cur[11]; }
        var nAddr = AD.FromForm("ed_addr"); nv[12] = AD.Compose(nAddr);
        if (!AD.Opened("ed_addr")) { nv[12] = cur[12]; nAddr = SubAddr(r); } // v53: Manage Address not opened = address unchanged
        else { // untouched form = no change, even when a legacy province value has no match in the Province / State select
            var shown = SubAddr(r); if (!(shown.Country == "United States" ? AD.States : AD.Canada).Contains(shown.Province)) shown.Province = "";
            if (nv[12] == AD.Compose(shown)) { nv[12] = cur[12]; nAddr = SubAddr(r); }
        }
        var plans = Plans(c); DataRow np = null;
        if (SA.PlanKey(nv[7]) == SA.PlanKey(cur[7])) nv[7] = cur[7];
        else
        {
            np = PlanRow(plans, nv[7]);
            if (np != null) { if (nv[8].Length == 0) nv[8] = Lim(np, "MaxBusinesses"); if (nv[9].Length == 0) nv[9] = Lim(np, "MaxEmployees"); }
        }
        var errs = new List<string>(); int x; DateTime d1, d2;
        if (nv[0].Length == 0) errs.Add("First name is required.");
        if (nv[2].Length == 0) errs.Add("Last name is required.");
        bool emailChanged = !String.Equals(nv[3], cur[3], StringComparison.OrdinalIgnoreCase);
        if (!emailChanged) nv[3] = cur[3];
        else if (!AnyminuteLoginEmail.ValidFormat(nv[3])) errs.Add("Enter a valid email address (up to 50 characters).");
        else if (AnyminuteSubscriberCreate.EmailTaken(c, nv[3])) errs.Add("This email address is already registered to another AnyMinute account.");
        if (nv[4].Length == 0) errs.Add("Contact phone is required.");
        foreach (int i in new[] { 4, 5 }) if (nv[i].Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(nv[i], @"^[0-9+()\-.\s]{7,30}$")) errs.Add(EL[i] + " is not valid.");
        if (plan && nv[7] != cur[7] && np == null) errs.Add("Select a plan from the plan catalogue.");
        foreach (int i in new[] { 8, 9 }) if (plan && nv[i].Length > 0 && (!Int32.TryParse(nv[i], out x) || x < 0)) errs.Add(EL[i] + " must be a whole number.");
        bool h1 = DateTime.TryParseExact(nv[10], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d1), h2 = DateTime.TryParseExact(nv[11], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d2);
        if (plan && (!h1 || !h2)) errs.Add("Enter valid subscription dates.");
        if (plan && h1 && h2 && d2 < d1) errs.Add("Next Billing Date must be after the Start Date.");
        if (nv[12] != cur[12]) { errs.AddRange(AD.Validate(nAddr, false)); if (r["AddrId"] == DBNull.Value) errs.Add("This subscriber has no contact record, so the address cannot be saved here."); }
        if (u == SA.ActorId() && nv[6] != cur[6] && nv[6] != "Active") errs.Add("You cannot deactivate or suspend your own account here.");
        var changed = Enumerable.Range(0, EF.Length).Where(i => nv[i] != cur[i]).ToList();
        Title1.Text = "Edit Subscriber"; Lead.Text = "Update subscriber information, plan and limits."; Crumbs.Text = CrumbBar(CrumbSep("<span>Edit Subscriber</span>"));
        if (errs.Count > 0 || changed.Count == 0)
        {
            Flash.Text = errs.Count > 0 ? "<div class='sa-flash sa-warn'>Please fix the following:<ul>" + String.Join("", errs.Select(m => "<li>" + SA.H(m) + "</li>").ToArray()) + "</ul></div>" : "<div class='sa-flash'>No changes to save.</div>";
            Content.Text = Edit(c, r, nv); return;
        }
        string reason = SA.Form("ed_reason");
        if (!save || reason.Length == 0)
        {
            Content.Text = Edit(c, r, nv) + ConfirmModal(u, changed, cur, nv, emailChanged);
            if (save) Flash.Text = "<div class='sa-flash sa-warn'>Enter the reason for this change.</div>";
            return;
        }
        if (reason.Length > 500) reason = reason.Substring(0, 500);
        Func<string, object> N = t => t.Length == 0 ? (object)DBNull.Value : t; bool revoke = false;
        using (var tx = c.BeginTransaction())
        {
            int e = Convert.ToInt32(r["EmployeeId"]);
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddEmployee SET FirstName=@f,MiddleName=@m,LastName=@l,MobilePhone=@mo WHERE EmployeeID=@e", "@f", nv[0], "@m", N(nv[1]), "@l", N(nv[2]), "@mo", N(nv[5]), "@e", e);
            if (changed.Contains(4)) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE ea SET MainPhone=@p FROM EmpAddress ea JOIN AddEmployee ae ON ae.ADDRESSID=ea.EmpAddressId WHERE ae.EmployeeID=@e", "@p", N(nv[4]), "@e", e);
            // v47: status written the same way Add Subscriber writes it. Pending Activation = Status 1 + first-time sign-in pending;
            // Active = Status 1, first sign-in done; Suspended = login disabled (sessions revoked); Inactive = Status 0 (sessions revoked).
            if (changed.Contains(6))
            {
                bool off = nv[6] == "Suspended" || nv[6] == "Inactive";
                if (nv[6] == "Inactive") AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE UserAcc SET Status=0 WHERE UserId=@u", "@u", u);
                else AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE UserAcc SET Status=1,Is_First_Time_Login=@f WHERE UserId=@u", "@f", nv[6] == AStatus[0], "@u", u);
                if (nv[6] != "Inactive")
                    AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u) INSERT AnyminuteLoginSecurity(UserId,LoginDisabled,RequirePasswordChange,TwoFactorEnabled,UpdatedAtUTC,UpdatedBy) VALUES(@u,@d,0,0,SYSUTCDATETIME(),@a)
                        ELSE UPDATE AnyminuteLoginSecurity SET LoginDisabled=@d,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE UserId=@u", "@u", u, "@d", nv[6] == "Suspended", "@a", SA.ActorId());
                if (off) revoke = true;
            }
            if (changed.Contains(12))
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE ea SET StreetNo=@no,NumberSuffix='',StreetName=@l1,StreetType=@l2,SuitType=@st,SuiteNumber=@sn,City=@ci,Province=@pr,Postalcode=@pc" + (AnyminuteEmployeeEditor.HasColumn(c, tx, "EmpAddress", "Country") ? ",Country=@k" : "") + " FROM EmpAddress ea JOIN AddEmployee ae ON ae.ADDRESSID=ea.EmpAddressId WHERE ae.EmployeeID=@e",
                    "@no", nAddr.StreetNo, "@l1", nAddr.Line1, "@l2", nAddr.Line2, "@st", nAddr.SuiteType, "@sn", nAddr.SuiteNo, "@ci", nAddr.City, "@pr", nAddr.Province, "@pc", nAddr.Postal, "@k", nAddr.Any ? nAddr.Country : "", "@e", e);
            if (plan && changed.Any(i => i >= 7 && i <= 11))
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE PlanHistory SET PlanName=@pn,Business=@b,No_Of_Users=@n,StartDate=@sd,EndDate=@ed WHERE Id=@id", "@pn", nv[7], "@b", N(nv[8]), "@n", N(nv[9]), "@sd", d1, "@ed", d2, "@id", Convert.ToInt32(r["PlanRowId"]));
            if (revoke) AnyminuteSessionGuard.Revoke(c, tx, u, "Subscriber " + nv[6].ToLowerInvariant() + " by Support Admin");
            var logged = changed.Where(i => i != 3).ToList();
            if (logged.Count > 0) SA.Audit(c, tx, "Login", u.ToString(CultureInfo.InvariantCulture), "Subscriber Updated", null, null, String.Join("; ", logged.Select(i => EL[i] + ": " + cur[i]).ToArray()), String.Join("; ", logged.Select(i => EL[i] + ": " + nv[i]).ToArray()), reason);
            tx.Commit();
        }
        if (revoke) AnyminuteSessionGuard.Revoked(u);
        // v39 rule kept: the login email is never written directly; the new address gets the verification invitation.
        if (emailChanged)
        {
            string err = AnyminuteLoginEmail.Recovery(SA.ActorId(), cur[3], nv[3], "Support Admin edit: " + reason);
            if (err != null) SA.Flash("Other changes were saved, but the email change was not started: " + err);
            else SA.Audit(c, null, "Login", u.ToString(CultureInfo.InvariantCulture), "Email Change Invitation", null, null, cur[3], nv[3], reason);
        }
        Back(u, "updated");
    }
    // v45 PPT 01 s6 step 3: Confirm Changes is a dialog over the Edit Subscriber form (the form underneath still carries the new values).
    static string ConfirmModal(int u, List<int> changed, string[] cur, string[] nv, bool emailChanged)
    {
        var sb = new StringBuilder("<div class='sa-modal sa-modal45' data-sa-modal='1'><div class='sa-modal-box sa-cf45' role='dialog' aria-modal='true' aria-labelledby='sa-cf-h'><button type='button' class='sa-x' data-sa-close='1' aria-label='Close'>&times;</button>")
            .Append("<h2 id='sa-cf-h'>Confirm Changes</h2><p class='sa-muted'>Please review the changes below before saving.</p><div class='sa-table-wrap'><table class='sa-table sa-cf-table'><thead><tr><th>Field</th><th>Previous Value</th><th>New Value</th></tr></thead><tbody>");
        foreach (int i in changed) sb.Append("<tr><td>").Append(EL[i]).Append("</td><td>").Append(cur[i].Length > 0 ? SA.H(cur[i]) : "&mdash;").Append("</td><td>").Append(nv[i].Length > 0 ? SA.H(nv[i]) : "&mdash;").Append("</td></tr>");
        sb.Append("</tbody></table></div>").Append(emailChanged ? "<div class='sa-flash sa-info'>" + Ic("info") + "<span>An invitation email will be sent to the new email address. The login email changes once the subscriber confirms it.</span></div>" : "")
          .Append("<label class='sa-field sa-reason'><span>Reason for Change <em class='sa-req'>*</em></span><textarea name='ed_reason' rows='3' maxlength='500' required data-sa-counter='500' placeholder='Enter reason for this change...'>").Append(SA.H(SA.Form("ed_reason"))).Append("</textarea><small data-sa-count='1'>0/500</small></label>")
          .Append("<div class='sa-modal-foot'><button type='button' class='sa-btn sa-btn-outline' data-sa-close='1'>Cancel</button><button type='submit' class='sa-btn' name='sa_op' value='editsave'>").Append(Ic("check")).Append(" Confirm &amp; Save</button></div></div></div>");
        return sb.ToString();
    }
    // v45 PPT 01 s6 step 4: Subscriber Updated Successfully! dialog (outline View Subscriber Details, filled Back to Subscriber List).
    static string UpdatedModal(int u)
    {
        string b = "SupportSubscribers.aspx?user=" + u;
        return "<div class='sa-modal sa-modal45' data-sa-modal='1'><div class='sa-modal-box sa-ok45' role='dialog' aria-modal='true' aria-labelledby='sa-ok-h'><a class='sa-x' href='" + b + "' aria-label='Close'>&times;</a>"
            + "<span class='sa-ok-ico' aria-hidden='true'><i>" + Ic("check") + "</i></span><h2 id='sa-ok-h'>Subscriber Updated Successfully!</h2><p>The subscriber information has been updated and the changes have been recorded in the activity log.</p>"
            + "<div class='sa-flash sa-info'>" + Ic("info") + "<span>If the email address was changed, an invitation email was sent to the new email address.</span></div>"
            + "<div class='sa-ok-btns'><a class='sa-btn sa-btn-outline' href='" + b + "'>View Subscriber Details</a><a class='sa-btn' href='SupportSubscribers.aspx'>Back to Subscriber List</a></div></div></div>";
    }
    // v45 PPT 01 s3/s4 Businesses tab "Edit": business name (the same AddBusiness / AssginBussiness / AssgineManager name update the
    // owner's Edit Business uses), limited to businesses this subscriber owns, with a reason in the support audit.
    string BizEdit(SqlConnection c, DataRow r, int biz, string name, string reason)
    {
        int u = Convert.ToInt32(r["UserId"]); string b = "SupportSubscribers.aspx?user=" + u;
        var t = SA.Q(c, BizCte + "SELECT b.BusinessID,b.CompanyName FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID WHERE b.BusinessID=@b", ScopeArgs(r).Concat(new object[] { "@b", biz }).ToArray());
        if (t.Rows.Count != 1) return "<div class='sa-card'><p>This business does not belong to this subscriber.</p><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=businesses'>Back to Businesses</a></div>";
        return "<input type='hidden' name='sa_user' value='" + u + "' /><input type='hidden' name='bz_id' value='" + biz + "' /><div class='sa-card'><h2>Edit Business</h2><div class='sa-grid2'><label class='sa-field'><span>Business Name <em class='sa-req'>*</em></span><input name='bz_name' maxlength='50' required value='"
            + SA.A(name ?? SA.S(t.Rows[0]["CompanyName"])) + "' /></label><label class='sa-field'><span>Reason for Change <em class='sa-req'>*</em></span><input name='bz_reason' maxlength='500' required value='" + SA.A(reason ?? "") + "' placeholder='Enter reason for this change...' /></label></div>"
            + "<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='" + b + "&amp;view=businesses'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='bizsave'>" + Ic("save") + " Save Changes</button></div></div>";
    }
    void BizPost(SqlConnection c, DataRow r)
    {
        int u = Convert.ToInt32(r["UserId"]), biz; Int32.TryParse(SA.Form("bz_id"), out biz); string name = SA.Form("bz_name"), reason = SA.Form("bz_reason");
        var t = SA.Q(c, BizCte + "SELECT b.CompanyName FROM AddBusiness b JOIN biz ON biz.BusinessID=b.BusinessID WHERE b.BusinessID=@b", ScopeArgs(r).Concat(new object[] { "@b", biz }).ToArray());
        string err = t.Rows.Count != 1 ? "This business does not belong to this subscriber." : name.Length == 0 || name.Length > 50 ? "Enter a business name (up to 50 characters)." : reason.Length == 0 ? "Enter the reason for this change." : null;
        if (err == null && SA.Q(c, "SELECT COUNT(*) FROM AddBusiness WHERE CompanyName=@n AND BusinessID<>@b", "@n", name, "@b", biz).Rows[0][0].ToString() != "0") err = "Business name already exists.";
        string old = t.Rows.Count == 1 ? SA.S(t.Rows[0][0]) : "";
        if (err == null && old != name)
        {
            if (reason.Length > 500) reason = reason.Substring(0, 500);
            using (var tx = c.BeginTransaction())
            {
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddBusiness SET CompanyName=@n WHERE BusinessID=@b; UPDATE AssginBussiness SET BusinessName=@n WHERE BusinessID=@b; UPDATE AssgineManager SET BusinessName=@n WHERE BusinessID=@b", "@n", name, "@b", biz);
                SA.Audit(c, tx, "Login", u.ToString(CultureInfo.InvariantCulture), "Business Updated", null, null, "Business Name: " + old, "Business Name: " + name, reason);
                tx.Commit();
            }
        }
        if (err != null) { Flash.Text = "<div class='sa-flash sa-warn'>" + SA.H(err) + "</div>"; Content.Text = Details(c, r) + "<div class='sa-tabbed'>" + DTabs(c, r, "businesses") + BizEdit(c, r, biz, name, reason) + "</div>"; return; }
        SA.Flash(old == name ? "No changes to save." : "Business updated."); Back(u, "businesses");
    }
    // v43 PPT 01 s6 / s5 success screens.
    static string Done(string title, string text, string info, string b1, string h1, string b2, string h2)
    {
        return "<div class='sa-card sa-done'><i class='sa-done-ico' aria-hidden='true'>&#10004;</i><h2>" + title + "</h2><p>" + text + "</p>" + (info.Length > 0 ? "<div class='sa-flash sa-info'>&#9432; " + info + "</div>" : "")
            + "<div class='sa-done-btns'><a class='sa-btn' href='" + h1 + "'>" + b1 + "</a><a class='sa-btn sa-btn-outline' href='" + h2 + "'>" + b2 + "</a></div></div>";
    }
}
