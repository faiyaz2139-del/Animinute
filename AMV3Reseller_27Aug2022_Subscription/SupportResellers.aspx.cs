using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-10-29 v28 (Shamsu deck 2 slides 1-2): Support Admin > Resellers. List (KPIs, search, Status / Plan / Sort, Clear Filters,
// Export, pagination, View + row menu), Add Reseller form (information, permissions, account & access with a generated temporary
// password that is stored only as a salted SHA-256 hash, optional welcome email, limits, internal notes) and the reseller view
// with subscriber assignment. Data: AnyminuteReseller / AnyminuteResellerSubscriber (DB_REQUIRED/REQUIRED_09). Every change is audited.
public partial class AnyminuteSupportResellersPage : System.Web.UI.Page
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static readonly string[] Sorts = { "Newest First", "Oldest First", "Name A-Z", "Name Z-A", "Most Subscribers" };
    const int PageSize = 10;
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        Flash.Text = SA.TakeFlash();
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(); return; }
        using (var c = SA.Open())
        {
            bool ready = RS.Ready(c);
            if (!ready) Flash.Text += "<div class='sa-flash sa-warn'>" + SA.H(RS.Missing) + "</div>";
            int id;
            if (SA.Get("add") == "1") { Title = "Add Reseller"; Body1.Text = AddForm(ready); return; }
            if (ready && Int32.TryParse(SA.Get("view"), out id)) { Body1.Text = View(c, id); return; }
            var all = ready ? RS.List(c).Rows.Cast<DataRow>().ToList() : new List<DataRow>();
            var rows = Filtered(all);
            if (SA.Get("export") == "excel") { Export(rows); return; }
            Body1.Text = List(all, rows, ready);
        }
    }
    // v29 (live v28: Support landing lists "ABC Cleaning Services" as Type Reseller while this page shows 0): the landing
    // reads legacy partner sign-ups (Register_Partner via SP_Get_Reseller_List); this module uses AnyminuteReseller.
    // Show the legacy records here (read-only) so the two pages no longer contradict; nothing is migrated automatically.
    static string LegacyNote()
    {
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (!SA.Has(c, "Register_Partner")) return "";
                var dt = SA.Q(c, "SELECT LTRIM(RTRIM(ISNULL(FirstName,'')+' '+ISNULL(LastName,''))) Name,EMail_primary Email FROM Register_Partner ORDER BY PartnerID");
                if (dt.Rows.Count == 0) return "";
                var names = dt.Rows.Cast<DataRow>().Select(r => { string n = AnyminuteDisplayText.Tidy(SA.S(r["Name"])), m = SA.S(r["Email"]); return SA.H(n.Length > 0 ? n + (m.Length > 0 ? " (" + m + ")" : "") : m); });
                return "<div class='sa-flash sa-warn' role='status'><strong>" + dt.Rows.Count.ToString(Inv) + " legacy reseller sign-up" + (dt.Rows.Count == 1 ? "" : "s") + "</strong> from the old partner registration appear" + (dt.Rows.Count == 1 ? "s" : "") +
                    " as Type &ldquo;Reseller&rdquo; on the Support landing list: " + String.Join(", ", names) + ". They are not managed resellers yet; use <em>Add Reseller</em> to set one up here.</div>";
            }
        }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("Legacy reseller note failed", ex); return ""; }
    }
    static string St(DataRow r) { string s = SA.S(r["Status"]); return s.Length == 0 ? "Active" : s; }
    List<DataRow> Filtered(List<DataRow> all)
    {
        string q = SA.Get("q"), st = SA.Get("status"), plan = SA.Get("plan"), sort = SA.Get("sort");
        IEnumerable<DataRow> rows = all;
        if (q.Length > 0) rows = rows.Where(r => new[] { "ResellerName", "ContactPerson", "Email", "Phone", "LoginEmail" }.Any(k => SA.S(r[k]).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) || RS.Rsl(r["ResellerID"]).Equals(q, StringComparison.OrdinalIgnoreCase));
        if (st.Length > 0) rows = rows.Where(r => St(r) == st);
        if (plan.Length > 0) rows = rows.Where(r => SA.S(r["PlanName"]) == plan);
        switch (sort)
        {
            case "Oldest First": rows = rows.OrderBy(r => r["CreatedAtUTC"]); break;
            case "Name A-Z": rows = rows.OrderBy(r => SA.S(r["ResellerName"]), StringComparer.OrdinalIgnoreCase); break;
            case "Name Z-A": rows = rows.OrderByDescending(r => SA.S(r["ResellerName"]), StringComparer.OrdinalIgnoreCase); break;
            case "Most Subscribers": rows = rows.OrderByDescending(r => Convert.ToInt32(r["Subscribers"])); break;
        }
        return rows.ToList();
    }
    string List(List<DataRow> all, List<DataRow> rows, bool ready)
    {
        string q = SA.Get("q"), st = SA.Get("status"), plan = SA.Get("plan"), sort = SA.Get("sort"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        string qs = "q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(st) + "&plan=" + HttpUtility.UrlEncode(plan) + "&sort=" + HttpUtility.UrlEncode(sort);
        Func<string, string, string, object, string> kpi = (icon, label, tone, n) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + icon + "</i><div><span>" + label + "</span><strong>" + n + "</strong></div></div>";
        var sb = new StringBuilder("<header class='sa-head'><div><h1>Resellers</h1><p>Manage resellers, their subscribers and access.</p></div><div class='sa-rpt-export'>")
            .Append("<a class='sa-btn sa-btn-outline' href='SupportResellers.aspx?").Append(SA.A(qs)).Append("&amp;export=excel'>&#128200; Export</a>")
            .Append(ready ? "<a class='sa-btn' href='SupportResellers.aspx?add=1'>+ Add Reseller</a>" : "<span class='sa-btn' aria-disabled='true' style='opacity:.5'>+ Add Reseller</span>").Append("</div></header>");
        sb.Append("<div class='sa-kpis'>").Append(kpi("&#128101;", "Total Resellers", "blue", all.Count)).Append(kpi("&#10004;", "Active Resellers", "green", all.Count(r => St(r) == "Active")))
          .Append(kpi("&#10006;", "Inactive Resellers", "red", all.Count(r => St(r) == "Inactive"))).Append(kpi("&#128100;", "Total Subscribers", "purple", all.Sum(r => Convert.ToInt32(r["Subscribers"])))).Append("</div>");
        sb.Append(LegacyNote()); // v29
        sb.Append("<div class='sa-card'><div class='sa-filters' data-sa-get='SupportResellers.aspx'><label class='sa-grow'><span>Search</span><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by reseller name, email or phone...' /></label>")
          .Append("<label><span>Status</span><select name='status'>").Append(SA.Options(new[] { "Active", "Inactive" }, st, "All Status")).Append("</select></label>")
          .Append("<label><span>Plan</span><select name='plan'>").Append(SA.Options(RS.Plans, plan, "All Plans")).Append("</select></label>")
          .Append("<label><span>Sort By</span><select name='sort'>").Append(SA.Options(Sorts, sort.Length == 0 ? Sorts[0] : sort, null)).Append("</select></label>")
          .Append("<button type='button' class='sa-btn' data-sa-apply='1'>Search</button><a class='sa-link' href='SupportResellers.aspx'>Clear Filters</a></div>");
        if (rows.Count == 0) sb.Append("<p class='sa-rpt-empty'>").Append(all.Count == 0 ? (ready ? "No resellers yet. Use + Add Reseller to create the first one." : "Reseller tables are not installed yet.") : "No resellers match these filters.").Append("</p>");
        else
        {
            sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>#</th><th>Reseller Name</th><th>Contact Person</th><th>Email</th><th>Phone</th><th>Plan</th><th>Status</th><th>Subscribers</th><th>Businesses</th><th>Created Date</th><th>Actions</th></tr></thead><tbody>");
            int n = (page - 1) * PageSize;
            foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
            {
                int id = Convert.ToInt32(r["ResellerID"]); string s = St(r); bool off = Convert.ToBoolean(r["LoginDisabled"]);
                sb.Append("<tr><td>").Append(++n).Append("</td><td><strong>").Append(SA.H(r["ResellerName"])).Append("</strong><br /><small class='sa-muted'>").Append(RS.Rsl(id)).Append("</small></td><td>").Append(SA.H(r["ContactPerson"]))
                  .Append("</td><td>").Append(SA.H(r["Email"])).Append("</td><td>").Append(SA.H(r["Phone"])).Append("</td><td>").Append(SA.H(r["PlanName"])).Append("</td><td>").Append(SA.Pill(s, s == "Active" ? "green" : "red"))
                  .Append("</td><td>").Append(r["Subscribers"]).Append("</td><td>").Append(r["Businesses"]).Append("</td><td>").Append(SA.When(r["CreatedAtUTC"])).Append("</td><td class='sa-nowrap'><a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportResellers.aspx?view=").Append(id).Append("'>View</a> ")
                  .Append("<details class='sa-menu'><summary aria-label='Reseller actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Reseller Actions</h4><small>").Append(SA.H(r["ResellerName"])).Append(" &middot; ").Append(s).Append("</small>")
                  .Append("<a class='sa-menu-item' href='SupportResellers.aspx?view=").Append(id).Append("'><i aria-hidden='true'>&#128065;</i><span><b>View Details</b><small>Profile, permissions and subscribers</small></span></a>")
                  .Append("<button type='submit' name='sa_op' value='status' formnovalidate='formnovalidate' class='sa-menu-item' data-sa-set='sa_id=").Append(id).Append("' data-sa-confirm='").Append(s == "Active" ? "Deactivate" : "Activate").Append(" this reseller?'><i aria-hidden='true'>&#9211;</i><span><b>").Append(s == "Active" ? "Deactivate Reseller" : "Activate Reseller").Append("</b><small>Change the reseller account status</small></span></button>")
                  .Append("<button type='submit' name='sa_op' value='login' formnovalidate='formnovalidate' class='sa-menu-item' data-sa-set='sa_id=").Append(id).Append("' data-sa-confirm='").Append(off ? "Enable" : "Disable").Append(" login for this reseller?'><i aria-hidden='true'>&#128274;</i><span><b>").Append(off ? "Enable Login" : "Disable Login").Append("</b><small>Control reseller portal access</small></span></button>")
                  .Append("</div></details></td></tr>");
            }
            sb.Append("</tbody></table></div><input type='hidden' name='sa_id' value='' />").Append(SA.Pager(rows.Count, page, PageSize, qs));
        }
        return sb.Append("</div>").ToString();
    }
    void Export(List<DataRow> rows)
    {
        var s = new AmRptSection { Title = "Resellers", Cols = new[] { "#", "Reseller ID", "Reseller Name", "Contact Person", "Email", "Phone", "Plan", "Status", "Subscribers", "Businesses", "Created Date" } };
        int n = 0;
        foreach (var r in rows) s.Rows.Add(new[] { (++n).ToString(Inv), RS.Rsl(r["ResellerID"]), SA.S(r["ResellerName"]), SA.S(r["ContactPerson"]), SA.S(r["Email"]), SA.S(r["Phone"]), SA.S(r["PlanName"]), St(r), SA.S(r["Subscribers"]), SA.S(r["Businesses"]),
            r["CreatedAtUTC"] == DBNull.Value ? "" : DateTime.SpecifyKind(Convert.ToDateTime(r["CreatedAtUTC"]), DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd", Inv) });
        var res = new AmRptResult { Def = new AmRptDef { Key = "sa-resellers", Name = "Resellers" }, Subtitle = rows.Count + " reseller(s)", FilterText = "Search: " + SA.Get("q") + "; Status: " + SA.Get("status") + "; Plan: " + SA.Get("plan"), Generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm", Inv) + " by " + SA.ActorName() };
        res.Sections.Add(s);
        Response.Clear(); Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.AddHeader("Content-Disposition", "attachment; filename=\"AnyMinute_Resellers_" + DateTime.Now.ToString("yyyyMMdd", Inv) + ".xlsx\"");
        using (var ms = new System.IO.MemoryStream()) { AnyminuteReportFiles.Xlsx(res, ms); ms.WriteTo(Response.OutputStream); }
        Response.Flush(); Context.ApplicationInstance.CompleteRequest();
    }
    // ---------- Add Reseller ----------
    string Val(string k, string d) { return IsPostBack ? SA.Form(k) : d; }
    string AddForm(bool ready)
    {
        string dis = ready ? "" : " disabled";
        Func<string, string, string, string, string> input = (label, name, type, extra) => "<label class='sa-field'><span>" + label + "</span><input type='" + type + "' name='" + name + "' value='" + SA.A(Val(name, "")) + "'" + extra + dis + " /></label>";
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; Add Reseller</nav><header class='sa-head'><div><h1>Add Reseller</h1><p>Create a reseller account, set what they can do and their limits.</p></div></header>");
        sb.Append("<div class='sa-rsl-form'><div class='sa-card'><h2>Reseller Information</h2><div class='sa-grid2'>")
          .Append(input("Reseller Name *", "rs_name", "text", " required maxlength='150' placeholder='Enter reseller company name'"))
          .Append(input("Contact Person *", "rs_contact", "text", " required maxlength='150' placeholder='Enter contact person name'"))
          .Append(input("Email Address *", "rs_email", "email", " required maxlength='200' placeholder='name@company.com'"))
          .Append(input("Phone Number *", "rs_phone", "tel", " required maxlength='30' placeholder='(555) 123-4567'"))
          .Append(input("Alternate Phone", "rs_alt", "tel", " maxlength='30' placeholder='Optional'"))
          .Append("<label class='sa-field'><span>Plan</span><select name='rs_plan'").Append(dis).Append(">").Append(SA.Options(RS.Plans, Val("rs_plan", "Standard"), null)).Append("</select></label>")
          .Append("</div><label class='sa-field'><span>Address</span><textarea name='rs_address' rows='2' maxlength='400' placeholder='Street, city, province / state, postal code, country'").Append(dis).Append(">").Append(SA.H(Val("rs_address", ""))).Append("</textarea></label></div>");
        sb.Append("<div class='sa-card'><h2>Permissions</h2><p class='sa-muted'>Choose what this reseller can do.</p><div class='sa-rsl-perms'>");
        foreach (var p in RS.PermissionList)
        {
            bool on = IsPostBack ? Request.Form.GetValues("rs_perm") != null && Request.Form.GetValues("rs_perm").Contains(p[0]) : p[2] == "1";
            sb.Append("<label class='sa-check'><input type='checkbox' name='rs_perm' value='").Append(p[0]).Append("'").Append(on ? " checked" : "").Append(dis).Append(" /> ").Append(SA.H(p[1])).Append("</label>");
        }
        string pw = Val("rs_pw", RS.NewTempPassword());
        sb.Append("</div></div><div class='sa-card'><h2>Account &amp; Access</h2><div class='sa-grid2'>")
          .Append(input("Login Email *", "rs_login", "email", " required maxlength='200' placeholder='Used to sign in to the reseller portal'"))
          .Append("<label class='sa-field'><span>Temporary Password *</span><span class='sa-rsl-pw'><input type='text' name='rs_pw' id='rs_pw' value='").Append(SA.A(pw)).Append("' required minlength='8' maxlength='64' autocomplete='off'").Append(dis)
          .Append(" /><button type='button' class='sa-btn sa-btn-outline sa-btn-sm' data-sa-genpw='rs_pw'").Append(dis).Append(">Generate</button></span><small>At least 8 characters. Stored only as a secure hash; shown once after saving.</small></label>")
          .Append("<label class='sa-check'><input type='checkbox' name='rs_welcome' value='1'").Append(!IsPostBack || SA.Form("rs_welcome") == "1" ? " checked" : "").Append(dis).Append(" /> Send welcome email with login details</label>")
          .Append("<label class='sa-field'><span>Status</span><select name='rs_status'").Append(dis).Append(">").Append(SA.Options(new[] { "Active", "Inactive" }, Val("rs_status", "Active"), null)).Append("</select></label></div></div>");
        sb.Append("<div class='sa-card'><h2>Limits</h2><div class='sa-grid2'>")
          .Append(input("Max Subscribers", "rs_maxsub", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("value=''", "value='" + (IsPostBack ? "" : "50") + "'"))
          .Append(input("Max Businesses per Subscriber", "rs_maxbiz", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("value=''", "value='" + (IsPostBack ? "" : "10") + "'"))
          .Append("</div><p class='sa-muted sa-small'>Leave blank for unlimited.</p></div>");
        sb.Append("<div class='sa-card'><h2>Internal Notes</h2><label class='sa-field'><span class='sr-only'>Internal Notes</span><textarea name='rs_notes' rows='3' maxlength='1000' data-sa-counter='1000' placeholder='Notes visible to Support Admins only'").Append(dis).Append(">")
          .Append(SA.H(Val("rs_notes", ""))).Append("</textarea><small data-sa-count='1'>0/1000</small></label></div>");
        sb.Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='create'").Append(dis).Append(">Create Reseller</button></div></div>");
        return sb.ToString();
    }
    static object N(string s) { return String.IsNullOrEmpty(s) ? (object)DBNull.Value : s; }
    static readonly Regex Mail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    void Create(SqlConnection c)
    {
        string name = SA.Form("rs_name"), contact = SA.Form("rs_contact"), email = SA.Form("rs_email"), phone = SA.Form("rs_phone"), alt = SA.Form("rs_alt"), addr = SA.Form("rs_address"), plan = SA.Form("rs_plan"),
            login = SA.Form("rs_login"), pw = SA.Form("rs_pw"), status = SA.Form("rs_status") == "Inactive" ? "Inactive" : "Active", notes = SA.Form("rs_notes"), ms = SA.Form("rs_maxsub"), mb = SA.Form("rs_maxbiz");
        var errs = new List<string>();
        if (name.Length == 0) errs.Add("Reseller name is required."); if (contact.Length == 0) errs.Add("Contact person is required.");
        if (!Mail.IsMatch(email)) errs.Add("Enter a valid email address."); if (!Regex.IsMatch(phone, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Enter a valid phone number.");
        if (alt.Length > 0 && !Regex.IsMatch(alt, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Alternate phone is not valid.");
        if (!Mail.IsMatch(login)) errs.Add("Enter a valid login email."); if (pw.Length < 8) errs.Add("Temporary password must be at least 8 characters.");
        if (!RS.Plans.Contains(plan)) plan = "Standard";
        int x; int? maxSub = null, maxBiz = null;
        if (ms.Length > 0) { if (Int32.TryParse(ms, out x) && x >= 0) maxSub = x; else errs.Add("Max subscribers must be a whole number."); }
        if (mb.Length > 0) { if (Int32.TryParse(mb, out x) && x >= 0) maxBiz = x; else errs.Add("Max businesses per subscriber must be a whole number."); }
        if (errs.Count == 0 && SA.Q(c, "SELECT 1 FROM AnyminuteReseller WHERE LoginEmail=@l", "@l", login).Rows.Count > 0) errs.Add("Another reseller already uses this login email.");
        if (errs.Count > 0) { Flash.Text = "<div class='sa-flash sa-warn'>Please fix the following:<ul>" + String.Join("", errs.Select(m => "<li>" + SA.H(m) + "</li>").ToArray()) + "</ul></div>"; Body1.Text = AddForm(true); return; }
        var perms = Request.Form.GetValues("rs_perm") ?? new string[0];
        string csv = String.Join(",", RS.PermissionList.Select(p => p[0]).Where(perms.Contains).ToArray()), salt = RS.Salt();
        int id;
        using (var tx = c.BeginTransaction())
        {
            id = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, @"INSERT AnyminuteReseller(ResellerName,ContactPerson,Email,Phone,AltPhone,Address,PlanName,LoginEmail,TempPasswordHash,TempPasswordSalt,Status,Permissions,MaxSubscribers,MaxBusinessesPerSubscriber,Notes,CreatedBy,UpdatedBy)
                VALUES(@n,@c,@e,@p,@a,@ad,@pl,@l,@h,@s,@st,@pm,@ms,@mb,@no,@by,@by); SELECT CAST(SCOPE_IDENTITY() AS int);",
                "@n", name, "@c", contact, "@e", email, "@p", phone, "@a", N(alt), "@ad", N(addr), "@pl", plan, "@l", login, "@h", RS.Hash(salt, pw), "@s", salt, "@st", status, "@pm", csv,
                "@ms", maxSub.HasValue ? (object)maxSub.Value : DBNull.Value, "@mb", maxBiz.HasValue ? (object)maxBiz.Value : DBNull.Value, "@no", N(notes), "@by", SA.ActorName()).Rows[0][0]);
            SA.Audit(c, tx, "Reseller", id.ToString(Inv), "Reseller Created", null, status, null, name + " | " + email + " | " + plan + " | limits " + (maxSub.HasValue ? maxSub.Value.ToString(Inv) : "unlimited") + "/" + (maxBiz.HasValue ? maxBiz.Value.ToString(Inv) : "unlimited"), "Permissions: " + RS.PermissionText(csv));
            tx.Commit();
        }
        string msg = "Reseller " + name + " (" + RS.Rsl(id) + ") created. Temporary password: " + pw + " (shown once; share it securely).";
        if (SA.Form("rs_welcome") == "1")
        {
            bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
            if (!send) msg += " Welcome email not sent: email sending is turned off for this site (IsSendMail).";
            else
            {
                try
                {
                    string body = "<p>Hello " + HttpUtility.HtmlEncode(contact) + ",</p><p>Your AnyMinute reseller account for <strong>" + HttpUtility.HtmlEncode(name) + "</strong> has been created.</p><p>Login email: <strong>" + HttpUtility.HtmlEncode(login)
                        + "</strong><br />Temporary password: <strong>" + HttpUtility.HtmlEncode(pw) + "</strong></p><p>Please change this password the first time you sign in.</p><p>Thank you,<br />The AnyMinute Team</p>";
                    new EmailSettingController().SendHtmlFormattedEmail(email, "Welcome to AnyMinute - your reseller account", body);
                    SA.X(c, null, "UPDATE AnyminuteReseller SET WelcomeEmailSentAtUTC=SYSUTCDATETIME() WHERE ResellerID=@i", "@i", id);
                    SA.Audit(c, null, "Reseller", id.ToString(Inv), "Welcome Email Sent", null, null, null, email, "Welcome email with login details sent.");
                    msg += " Welcome email sent to " + email + ".";
                }
                catch (Exception ex) { msg += " Welcome email could not be sent: " + ex.Message; }
            }
        }
        SA.Flash(msg); Go("?view=" + id);
    }
    // ---------- View ----------
    string View(SqlConnection c, int id)
    {
        var t = RS.List(c).Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["ResellerID"]) == id);
        if (t == null) return "<div class='sa-card'><p>That reseller was not found. <a class='sa-link' href='SupportResellers.aspx'>Back to Resellers</a></p></div>";
        Title = SA.S(t["ResellerName"]);
        Func<string, object, string> kv = (k, v) => "<div><dt>" + k + "</dt><dd>" + (SA.S(v).Length == 0 ? "&mdash;" : SA.H(v)) + "</dd></div>";
        Func<object, string> lim = o => o == DBNull.Value ? "Unlimited" : SA.S(o);
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; ").Append(SA.H(t["ResellerName"])).Append("</nav><header class='sa-head'><div><h1>").Append(SA.H(t["ResellerName"])).Append("</h1><p>")
            .Append(RS.Rsl(id)).Append(" &middot; ").Append(SA.Pill(St(t), St(t) == "Active" ? "green" : "red")).Append(" &middot; Login ").Append(Convert.ToBoolean(t["LoginDisabled"]) ? "disabled" : "enabled").Append("</p></div><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>Back</a></header>");
        sb.Append("<div class='sa-grid2'><div class='sa-card'><h2>Reseller Information</h2><dl class='sa-kv'>").Append(kv("Contact Person", t["ContactPerson"])).Append(kv("Email", t["Email"])).Append(kv("Phone", t["Phone"])).Append(kv("Alternate Phone", t["AltPhone"]))
          .Append(kv("Address", t["Address"])).Append(kv("Plan", t["PlanName"])).Append(kv("Created", SA.S(t["CreatedBy"]))).Append("</dl></div><div class='sa-card'><h2>Account &amp; Limits</h2><dl class='sa-kv'>").Append(kv("Login Email", t["LoginEmail"]))
          .Append(kv("Invitation", t["InvitationStatus"])).Append(kv("Max Subscribers", lim(t["MaxSubscribers"]))).Append(kv("Max Businesses / Subscriber", lim(t["MaxBusinessesPerSubscriber"]))).Append(kv("Subscribers", t["Subscribers"])).Append(kv("Businesses", t["Businesses"]))
          .Append(kv("Permissions", RS.PermissionText(SA.S(t["Permissions"])))).Append(kv("Internal Notes", t["Notes"])).Append("</dl></div></div>");
        var subs = RS.Subscribers(c, 0).Rows.Cast<DataRow>().ToList(); var mine = subs.Where(s => s["ResellerID"] != DBNull.Value && Convert.ToInt32(s["ResellerID"]) == id).ToList();
        sb.Append("<div class='sa-card'><div class='sa-rpt-cardhead'><h2>Subscribers (").Append(mine.Count).Append(")</h2></div>");
        if (mine.Count == 0) sb.Append("<p class='sa-rpt-empty'>No subscribers assigned to this reseller yet.</p>");
        else
        {
            sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Subscriber ID</th><th>Name</th><th>Email</th><th>Plan</th><th>Businesses</th><th>Assigned</th></tr></thead><tbody>");
            foreach (var s in mine) sb.Append("<tr><td>").Append(RS.Sub(s["UserId"])).Append("</td><td>").Append(SA.H(s["Name"])).Append("</td><td>").Append(SA.H(s["Email"])).Append("</td><td>").Append(SA.H(s["PlanName"])).Append("</td><td>").Append(s["Businesses"]).Append("</td><td>").Append(SA.When(s["AssignedAtUTC"])).Append("</td></tr>");
            sb.Append("</tbody></table></div>");
        }
        var free = subs.Where(s => s["ResellerID"] == DBNull.Value).ToList();
        sb.Append("<h3>Assign Subscriber</h3><div class='sa-filters'><input type='hidden' name='sa_id' value='").Append(id).Append("' /><label class='sa-grow'><span>Unassigned subscribers</span><select name='rs_user'><option value=''>Select subscriber</option>");
        foreach (var s in free) sb.Append("<option value='").Append(s["UserId"]).Append("'>").Append(SA.H(RS.Sub(s["UserId"]) + " - " + (SA.S(s["Name"]).Length > 0 ? SA.S(s["Name"]) : SA.S(s["Email"])) + " (" + SA.S(s["Email"]) + ")")).Append("</option>");
        sb.Append("</select></label><button type='submit' class='sa-btn' name='sa_op' value='assign' formnovalidate='formnovalidate'>Assign to Reseller</button></div><p class='sa-muted sa-small'>Each subscriber belongs to one reseller. The max subscriber limit is enforced.</p></div>");
        sb.Append("<div class='sa-card'><h2>Activity History</h2>").Append(SA.HistoryTable(SA.History(c, "Reseller", id.ToString(Inv)), "No activity recorded for this reseller yet.")).Append("</div>");
        return sb.ToString();
    }
    // ---------- POST actions ----------
    void Go(string query) { Response.Redirect("SupportResellers.aspx" + query, false); Context.ApplicationInstance.CompleteRequest(); }
    void Act()
    {
        using (var c = SA.Open())
        {
            if (!RS.Ready(c)) { SA.Flash(RS.Missing); Go(""); return; }
            string op = SA.Form("sa_op");
            if (op == "create") { Create(c); return; }
            int id; Int32.TryParse(SA.Form("sa_id"), out id);
            var r = SA.Q(c, "SELECT * FROM AnyminuteReseller WHERE ResellerID=@i", "@i", id);
            if (r.Rows.Count != 1) { SA.Flash("That reseller was not found."); Go(""); return; }
            string key = id.ToString(Inv), name = SA.S(r.Rows[0]["ResellerName"]), st = St(r.Rows[0]); bool off = Convert.ToBoolean(r.Rows[0]["LoginDisabled"]);
            if (op == "status")
            {
                string ns = st == "Active" ? "Inactive" : "Active";
                SA.X(c, null, "UPDATE AnyminuteReseller SET Status=@s,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@b WHERE ResellerID=@i", "@s", ns, "@b", SA.ActorName(), "@i", id);
                SA.Audit(c, null, "Reseller", key, ns == "Active" ? "Reseller Activated" : "Reseller Deactivated", st, ns, null, null, null);
                SA.Flash(name + " is now " + ns + "."); Go(""); return;
            }
            if (op == "login")
            {
                SA.X(c, null, "UPDATE AnyminuteReseller SET LoginDisabled=@d,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@b WHERE ResellerID=@i", "@d", !off, "@b", SA.ActorName(), "@i", id);
                SA.Audit(c, null, "Reseller", key, off ? "Login Enabled" : "Login Disabled", null, null, off ? "Disabled" : "Enabled", off ? "Enabled" : "Disabled", null);
                SA.Flash("Login " + (off ? "enabled" : "disabled") + " for " + name + "."); Go(""); return;
            }
            if (op == "assign")
            {
                int u; if (!Int32.TryParse(SA.Form("rs_user"), out u)) { SA.Flash("Choose a subscriber to assign."); Go("?view=" + id); return; }
                object max = r.Rows[0]["MaxSubscribers"];
                int count = Convert.ToInt32(SA.Q(c, "SELECT COUNT(*) FROM AnyminuteResellerSubscriber WHERE ResellerID=@i", "@i", id).Rows[0][0]);
                if (max != DBNull.Value && count >= Convert.ToInt32(max)) { SA.Flash(name + " has reached its limit of " + max + " subscribers."); Go("?view=" + id); return; }
                if (SA.Q(c, "SELECT 1 FROM AnyminuteResellerSubscriber WHERE UserId=@u", "@u", u).Rows.Count > 0) { SA.Flash("That subscriber already belongs to a reseller."); Go("?view=" + id); return; }
                SA.X(c, null, "INSERT AnyminuteResellerSubscriber(UserId,ResellerID,AssignedBy) VALUES(@u,@i,@b)", "@u", u, "@i", id, "@b", SA.ActorName());
                SA.Audit(c, null, "Reseller", key, "Subscriber Assigned", null, null, null, RS.Sub(u), null);
                SA.Flash(RS.Sub(u) + " assigned to " + name + "."); Go("?view=" + id); return;
            }
            Go("");
        }
    }
}
