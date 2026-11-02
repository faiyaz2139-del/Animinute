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
using AD = AnyminuteSupportAddress;

// 2026-11-01 v46 (t148u/t149u): list = Subscribers / Businesses table pattern, 10 per page, kebab-only Actions; Add / Edit use the Address module.
// 2026-11-01 v42 (PPT 02 20261028B s1-s2, PPT 04 s1-s2): list without the legacy banner / RSL sub-line, date-only Created Date,
// the deck's Reseller Actions menu (View Details, Edit, View Subscribers, View Businesses, Login & Access, Password Reset,
// Permissions & Limits, Activity History, Deactivate). The view page keeps only Reseller Information + Account & Limits;
// subscribers / businesses / activity history open as their own screens from the menu.
// 2026-10-29 v30: list + Add Reseller re-laid out to the deck 2 mocks (Pandey post-v29 notes).
// 2026-10-29 v28 (Shamsu deck 2 slides 1-2): Support Admin > Resellers. List (KPIs, search, Status / Plan / Sort, Clear Filters,
// Export, pagination, View + row menu), Add Reseller form (information, permissions, account & access with a generated temporary
// password that is stored only as a salted SHA-256 hash, optional welcome email, limits, internal notes) and the reseller view
// with subscriber assignment. Data: AnyminuteReseller / AnyminuteResellerSubscriber (DB_REQUIRED/REQUIRED_09). Every change is audited.
public partial class AnyminuteSupportResellersPage : System.Web.UI.Page
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static readonly string[] Sorts = { "Created Date (Newest)", "Created Date (Oldest)", "Name (A-Z)", "Name (Z-A)", "Most Subscribers" }; // v30 mock labels
    static readonly int[] Sizes = { 10, 20, 50 };
    int PageSize = 10; // v46 (t148u): at least 10 rows at a time by default, like Subscribers / Businesses
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        // v46: JSON postal lookup for the Address module (Add / Edit Reseller).
        if (SA.Get("lookup") == "1") { Response.Clear(); Response.ContentType = "application/json"; Response.Write(AD.LookupJson(SA.Get("postal"))); Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest(); return; }
        Flash.Text = SA.TakeFlash();
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(); return; }
        using (var c = SA.Open())
        {
            bool ready = RS.Ready(c);
            if (!ready) Flash.Text += "<div class='sa-flash sa-warn'>" + SA.H(RS.Missing) + "</div>";
            int id;
            if (SA.Get("add") == "1") { Title = "Add Reseller"; Body1.Text = AddForm(ready); return; }
            if (ready && Int32.TryParse(SA.Get("edit"), out id)) { var e0 = Row(c, id); Title = "Edit Reseller"; Body1.Text = e0 == null ? NotFound : AddForm(true, e0); return; }
            if (ready && Int32.TryParse(SA.Get("view"), out id)) { string tab = SA.Get("tab"); Body1.Text = tab == "subscribers" || tab == "businesses" || tab == "history" ? Sub(c, id, tab) : View(c, id); return; }
            var all = ready ? RS.List(c).Rows.Cast<DataRow>().ToList() : new List<DataRow>();
            var rows = Filtered(all);
            if (SA.Get("export") == "excel") { Export(rows); return; }
            Body1.Text = List(all, rows, ready);
        }
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
            case "Created Date (Oldest)": case "Oldest First": rows = rows.OrderBy(r => r["CreatedAtUTC"]); break;
            case "Name (A-Z)": case "Name A-Z": rows = rows.OrderBy(r => SA.S(r["ResellerName"]), StringComparer.OrdinalIgnoreCase); break;
            case "Name (Z-A)": case "Name Z-A": rows = rows.OrderByDescending(r => SA.S(r["ResellerName"]), StringComparer.OrdinalIgnoreCase); break;
            case "Most Subscribers": rows = rows.OrderByDescending(r => Convert.ToInt32(r["Subscribers"])); break;
        }
        return rows.ToList();
    }
    // v30 (Pandey post-v29, Shamsu deck 2 Resellers mock): header with + Add Reseller only, KPI cards, filter bar (search with icon,
    // Status / Plan / Sort By, Clear Filters button), "Reseller List" card with Export, full table chrome even when empty,
    // "Showing x to y of z resellers", prev / page / next and Show [n] per page.
    string List(List<DataRow> all, List<DataRow> rows, bool ready)
    {
        string q = SA.Get("q"), st = SA.Get("status"), plan = SA.Get("plan"), sort = SA.Get("sort"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        int size; if (Int32.TryParse(SA.Get("size"), out size) && Sizes.Contains(size)) PageSize = size;
        int pages = Math.Max(1, (rows.Count + PageSize - 1) / PageSize); if (page > pages) page = pages;
        string qs = "q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(st) + "&plan=" + HttpUtility.UrlEncode(plan) + "&sort=" + HttpUtility.UrlEncode(sort) + "&size=" + PageSize.ToString(Inv);
        Func<string, string, string, object, string, string> kpi = (icon, label, tone, n, sub) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + icon + "</i><div><span>" + label + "</span><strong>" + n + "</strong>" + (sub == null ? "" : "<small class='sa-muted'>" + sub + "</small>") + "</div></div>";
        var sb = new StringBuilder("<header class='sa-head'><div><h1>Resellers</h1><p>Manage reseller accounts. Resellers can create and manage their own subscribers.</p></div>")
            .Append(ready ? "<a class='sa-btn sa-btn-lg' href='SupportResellers.aspx?add=1'>+ Add Reseller</a>" : "<span class='sa-btn sa-btn-lg' aria-disabled='true' style='opacity:.5'>+ Add Reseller</span>").Append("</header>");
        sb.Append("<div class='sa-kpis'>").Append(kpi("&#128101;", "Total Resellers", "blue", all.Count, null)).Append(kpi("&#128101;", "Active Resellers", "green", all.Count(r => St(r) == "Active"), null))
          .Append(kpi("&#128101;", "Inactive Resellers", "red", all.Count(r => St(r) == "Inactive"), null)).Append(kpi("&#128100;", "Total Subscribers", "purple", all.Sum(r => Convert.ToInt32(r["Subscribers"])), "Under All Resellers")).Append("</div>");
        sb.Append("<div class='sa-card sa-rsl-bar'><div class='sa-filters' data-sa-get='SupportResellers.aspx'><input type='hidden' name='size' value='").Append(PageSize).Append("' />")
          .Append("<label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>&#128269;</i><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by reseller name, contact, or email...' /></label>")
          .Append("<label><span>Status</span><select name='status'>").Append(SA.Options(new[] { "Active", "Inactive" }, st, "All")).Append("</select></label>")
          .Append("<label><span>Plan</span><select name='plan'>").Append(SA.Options(RS.Plans, plan, "All")).Append("</select></label>")
          .Append("<label><span>Sort By</span><select name='sort'>").Append(SA.Options(Sorts, Sorts.Contains(sort) ? sort : Sorts[0], null)).Append("</select></label>")
          .Append("<a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>Clear Filters</a></div></div>");
        sb.Append("<div class='sa-card'><div class='sa-rpt-cardhead'><h2>Reseller List</h2><a class='sa-btn sa-btn-outline sa-btn-sm' href='SupportResellers.aspx?").Append(SA.A(qs)).Append("&amp;export=excel'>&#8682; Export</a></div>")
          // v46 (t148u/t149u): same table pattern as Subscribers / Businesses - fits the card (no sideways scroll), name link,
          // status pill, kebab-only Actions (View Details is the first menu item), full menu shown as a fixed pop-over.
          .Append("<div class='sa-table-wrap'><table class='sa-table sa-list46'><thead><tr><th>Sl. No.</th><th>Reseller Name</th><th>Contact Person</th><th>Email ID</th><th>Plan Name</th><th class='sa-c'>Subscribers</th><th class='sa-c'>Businesses</th><th>Created Date</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        if (rows.Count == 0)
            sb.Append("<tr><td colspan='10' class='sa-rpt-empty'>").Append(all.Count == 0 ? (ready ? "No resellers yet. Use + Add Reseller to create the first one." : "Reseller tables are not installed yet (run DB_REQUIRED/REQUIRED_09_Resellers.sql).") : "No resellers match these filters.").Append("</td></tr>");
        else
        {
            int n = (page - 1) * PageSize;
            foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
            {
                int id = Convert.ToInt32(r["ResellerID"]); string s = St(r); bool off = Convert.ToBoolean(r["LoginDisabled"]);
                sb.Append("<tr><td>").Append(++n).Append("</td><td class='sa-wrap'><a class='sa-link' href='SupportResellers.aspx?view=").Append(id).Append("'>").Append(SA.H(r["ResellerName"])).Append("</a><small class='sa-muted'>").Append(SA.H(RS.Rsl(r["ResellerID"]))).Append("</small></td><td class='sa-wrap'>").Append(SA.H(r["ContactPerson"]))
                  .Append(SA.S(r["Phone"]).Length > 0 ? "<small class='sa-muted'>" + SA.H(r["Phone"]) + "</small>" : "").Append("</td><td class='sa-wrap'>").Append(SA.H(r["Email"])).Append("</td><td>").Append(SA.S(r["PlanName"]).Length > 0 ? SA.H(r["PlanName"]) : "&mdash;")
                  .Append("</td><td class='sa-c'>").Append(r["Subscribers"]).Append("</td><td class='sa-c'>").Append(r["Businesses"]).Append("</td><td class='sa-nowrap'>").Append(CDay(r["CreatedAtUTC"])).Append("</td><td>").Append(SA.Pill(s, s == "Active" ? "green" : "red"))
                  .Append("</td><td class='sa-c'><details class='sa-menu'><summary class='sa-kebab' aria-label='Reseller actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Reseller Actions</h4><small>").Append(SA.H(r["ResellerName"])).Append(" &middot; ").Append(s).Append("</small>")
                  .Append(Menu(id, s, off))
                  .Append("</div></details></td></tr>");
            }
        }
        sb.Append("</tbody></table></div><input type='hidden' name='sa_id' value='' />");
        // v46 footer (Subscribers pattern): Showing x to y of z resellers | [10 per page] | < 1 2 3 ... >
        string bq = "SupportResellers.aspx?q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(st) + "&plan=" + HttpUtility.UrlEncode(plan) + "&sort=" + HttpUtility.UrlEncode(sort);
        Func<int, string> pg = i => SA.A(bq + "&size=" + PageSize.ToString(Inv) + "&page=" + i.ToString(Inv));
        sb.Append("<div class='sa-pager sa-rsl-pager'><span>Showing ").Append(rows.Count == 0 ? 0 : (page - 1) * PageSize + 1).Append(" to ").Append(Math.Min(rows.Count, page * PageSize)).Append(" of ").Append(rows.Count).Append(" reseller").Append(rows.Count == 1 ? "" : "s").Append("</span>")
          .Append("<label class='sa-rsl-size'><select aria-label='Rows per page' onchange=\"location.href='").Append(SA.A(bq)).Append("&size='+this.value\">");
        foreach (int z in Sizes) sb.Append("<option value='").Append(z).Append("'").Append(z == PageSize ? " selected" : "").Append(">").Append(z).Append(" per page</option>");
        sb.Append("</select></label><nav aria-label='Pages'>").Append(page > 1 ? "<a href='" + pg(page - 1) + "' aria-label='Previous page'>&lsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&lsaquo;</span>");
        int p0 = Math.Max(1, Math.Min(page - 2, pages - 4)), p1 = Math.Min(pages, p0 + 4);
        Func<int, string> pa = i => "<a href='" + pg(i) + "'" + (i == page ? " class='on' aria-current='page'" : "") + ">" + i + "</a>";
        if (p0 > 1) sb.Append(pa(1)).Append(p0 > 2 ? "<span class='sa-pg-gap' aria-hidden='true'>&hellip;</span>" : "");
        for (int i = p0; i <= p1; i++) sb.Append(pa(i));
        if (p1 < pages) sb.Append(p1 < pages - 1 ? "<span class='sa-pg-gap' aria-hidden='true'>&hellip;</span>" : "").Append(pa(pages));
        sb.Append(page < pages ? "<a href='" + pg(page + 1) + "' aria-label='Next page'>&rsaquo;</a>" : "<span class='sa-pg-off' aria-hidden='true'>&rsaquo;</span>").Append("</nav></div>");
        return sb.Append("</div>").ToString();
    }
    static string CDay(object utc) { return utc == null || utc == DBNull.Value ? "&mdash;" : SA.Day(DateTime.SpecifyKind(Convert.ToDateTime(utc), DateTimeKind.Utc).ToLocalTime()); }
    const string NotFound = "<div class='sa-card'><p>That reseller was not found. <a class='sa-link' href='SupportResellers.aspx'>Back to Resellers</a></p></div>";
    static DataRow Row(SqlConnection c, int id) { return RS.List(c).Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["ResellerID"]) == id); }
    // v42 PPT 02 s2 "Recommended Three-Dot Menu" (labels and descriptions as on the slide).
    static string Menu(int id, string s, bool off)
    {
        string v = "SupportResellers.aspx?view=" + id;
        Func<string, string, string, string, string> a = (href, ico, b, sm) => "<a class='sa-menu-item' href='" + href + "'><i aria-hidden='true'>" + ico + "</i><span><b>" + b + "</b><small>" + sm + "</small></span></a>";
        Func<string, string, string, string, string, string, string> p = (op, conf, ico, b, sm, cls) => "<button type='submit' name='sa_op' value='" + op + "' formnovalidate='formnovalidate' class='sa-menu-item" + cls + "' data-sa-set='sa_id=" + id + "' data-sa-confirm='" + conf + "'><i aria-hidden='true'>" + ico + "</i><span><b>" + b + "</b><small>" + sm + "</small></span></button>";
        return a(v, "&#128065;", "View Reseller Details", "View profile, subscription limits and information")
            + a("SupportResellers.aspx?edit=" + id, "&#9998;", "Edit Reseller", "Update reseller contact and account details")
            + a(v + "&amp;tab=subscribers", "&#128101;", "View Subscribers", "See subscribers created by this reseller")
            + a(v + "&amp;tab=businesses", "&#127970;", "View Businesses", "See businesses belonging to reseller subscribers")
            + p("login", (off ? "Enable" : "Disable") + " login for this reseller?", "&#128274;", "Manage Login &amp; Access", "Enable or disable reseller login access", "")
            + p("resetpw", "Send a new temporary password to this reseller?", "&#9993;", "Send Password Reset", "Send a secure reset link to the reseller", "")
            + a("SupportResellers.aspx?edit=" + id + "#permissions", "&#9776;", "Manage Permissions &amp; Limits", "Modify authorized permissions and limits")
            + a(v + "&amp;tab=history", "&#8634;", "View Activity History", "Review actions and account changes")
            + "<hr class='sa-menu-sep' />"
            + (s == "Active" ? p("status", "Deactivate this reseller?", "&#10006;", "Deactivate Reseller", "Deactivate account after confirmation", " sa-menu-danger")
                             : p("status", "Activate this reseller?", "&#10004;", "Activate Reseller", "Activate account after confirmation", ""));
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
    static readonly Dictionary<string, string> Cols = new Dictionary<string, string> { { "rs_name", "ResellerName" }, { "rs_contact", "ContactPerson" }, { "rs_email", "Email" }, { "rs_phone", "Phone" }, { "rs_alt", "AltPhone" },
        { "rs_address", "Address" }, { "rs_login", "LoginEmail" }, { "rs_status", "Status" }, { "rs_plan", "PlanName" }, { "rs_maxsub", "MaxSubscribers" }, { "rs_maxbiz", "MaxBusinessesPerSubscriber" }, { "rs_notes", "Notes" } };
    // v42: e != null = Edit Reseller (PPT 02 s2 "Edit Reseller" / "Manage Permissions & Limits"); same layout as Add Reseller.
    string AddForm(bool ready, DataRow e = null)
    {
        string dis = ready ? "" : " disabled";
        Func<string, string> D = k => e == null ? "" : SA.S(e[Cols[k]]);
        Func<string, string, string, string, string> input = (label, name, type, extra) => "<label class='sa-field'><span>" + label + "</span><input type='" + type + "' name='" + name + "' value='" + SA.A(Val(name, D(name))) + "'" + extra + (e != null && name == "rs_login" ? " readonly" : "") + dis + " /></label>";
        if (e != null) return EditForm(e, dis, D, input);
        // v30 (Pandey post-v29, Shamsu deck 2 Add Reseller mock): two columns - left Reseller Information + Permissions,
        // right Account & Access (Generate beside the password) + Limits + Notes; Back to Resellers top right; Cancel / Create bottom right.
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; <strong>Add Reseller</strong></nav><header class='sa-head'><div class='sa-head-ico'><i class='sa-kpi-icon sa-blue' aria-hidden='true'>&#128100;+</i><div><h1>Add Reseller</h1><p>Create a new reseller account. The reseller will be able to create and manage their own subscribers.</p></div></div>")
            .Append("<a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>&larr; Back to Resellers</a></header>");
        sb.Append("<div class='sa-rsl-form sa-rsl-cols'><div class='sa-rsl-col'><div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#127970;</i> Reseller Information</h2>")
          .Append(input("Reseller Name *", "rs_name", "text", " required maxlength='150' placeholder='Enter reseller name (e.g. ABC IT Solutions)'"))
          .Append("<div class='sa-grid2'>")
          .Append(input("Contact Person *", "rs_contact", "text", " required maxlength='150' placeholder='Enter contact person name'"))
          .Append(input("Email *", "rs_email", "email", " required maxlength='200' placeholder='Enter email address'"))
          .Append(input("Phone *", "rs_phone", "tel", " required maxlength='30' placeholder='Enter phone number'"))
          .Append(input("Alternate Phone", "rs_alt", "tel", " maxlength='30' placeholder='Enter alternate phone number'"))
          .Append("</div></div>"); // v46: the Address textarea is replaced by the required Address module below
        sb.Append("<div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#9881;</i> Permissions</h2><p class='sa-muted'>Select what the reseller can access and manage.</p><div class='sa-rsl-perms sa-rsl-perms1'>");
        foreach (var p in RS.PermissionList)
        {
            bool on = IsPostBack ? Request.Form.GetValues("rs_perm") != null && Request.Form.GetValues("rs_perm").Contains(p[0]) : p[2] == "1";
            if (p[0] == "upgrades") sb.Append("<span class='sa-rsl-gap' aria-hidden='true'></span>");
            sb.Append("<label class='sa-check'><input type='checkbox' name='rs_perm' value='").Append(p[0]).Append("'").Append(on ? " checked" : "").Append(dis).Append(" /> ").Append(SA.H(p[1])).Append("</label>");
        }
        string pw = Val("rs_pw", RS.NewTempPassword());
        sb.Append("</div></div></div><div class='sa-rsl-col'><div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128274;</i> Account &amp; Access</h2>")
          .Append(input("Login Email *", "rs_login", "email", " required maxlength='50' placeholder='Enter login email address'"))
          .Append("<label class='sa-field'><span>Temporary Password *</span><span class='sa-rsl-pw'><input type='text' name='rs_pw' id='rs_pw' value='").Append(SA.A(pw)).Append("' required minlength='8' maxlength='50' autocomplete='off'").Append(dis)
          .Append(" /><button type='button' class='sa-btn sa-btn-outline' data-sa-genpw='rs_pw'").Append(dis).Append(">&#8635; Generate</button></span><small class='sa-muted'>At least 8 characters. Stored only as a secure hash; shown once after saving.</small></label>")
          .Append("<label class='sa-check'><input type='checkbox' name='rs_welcome' value='1'").Append(!IsPostBack || SA.Form("rs_welcome") == "1" ? " checked" : "").Append(dis).Append(" /> Send welcome email to reseller</label>")
          .Append("<p class='sa-muted sa-small'>&#9432; A welcome email with login details will be sent to the reseller.</p><div class='sa-grid2'>")
          .Append("<label class='sa-field'><span>Status *</span><select name='rs_status'").Append(dis).Append(">").Append(SA.Options(new[] { "Active", "Inactive" }, Val("rs_status", "Active"), null)).Append("</select></label>")
          .Append("<label class='sa-field'><span>Plan</span><select name='rs_plan'").Append(dis).Append(">").Append(SA.Options(RS.Plans, Val("rs_plan", "Standard"), null)).Append("</select></label></div></div>");
        sb.Append("<div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128202;</i> Limits</h2><p class='sa-muted'>Set limits for the reseller.</p><div class='sa-grid2'>")
          .Append(input("Maximum Subscribers", "rs_maxsub", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("value=''", "value='" + (IsPostBack ? "" : "50") + "'").Replace("</label>", "<small class='sa-muted'>Leave blank for unlimited.</small></label>"))
          .Append(input("Maximum Businesses per Subscriber", "rs_maxbiz", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("value=''", "value='" + (IsPostBack ? "" : "10") + "'").Replace("</label>", "<small class='sa-muted'>Leave blank for unlimited.</small></label>"))
          .Append("</div></div>");
        sb.Append("<div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128221;</i> Notes</h2><p class='sa-muted'>Internal Notes (Support Admin Only)</p><label class='sa-field'><span class='sr-only'>Internal Notes</span><textarea name='rs_notes' rows='3' maxlength='1000' data-sa-counter='1000' placeholder='Enter any additional notes about this reseller...'").Append(dis).Append(">")
          .Append(SA.H(Val("rs_notes", ""))).Append("</textarea><small data-sa-count='1'>0/1000</small></label></div></div></div>");
        sb.Append(AddressCard(IsPostBack ? AD.FromForm("rs_addr") : new AD.Addr()));
        sb.Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='create'").Append(dis).Append(">Create Reseller</button></div>");
        return sb.ToString();
    }
    // v46 (Address deck s1 "Require Address Module"): Business-style Address Lookup + Address Details, required for resellers.
    static string AddressCard(AD.Addr a)
    {
        return "<div class='sa-rsl-addr' id='address'>" + AD.Render("rs_addr", a, true, "SupportResellers.aspx?lookup=1&postal=", "Reseller address (required). Search using a postal or ZIP code, select a result, or enter the address manually.") + "</div>";
    }
    string EditForm(DataRow e, string dis, Func<string, string> D, Func<string, string, string, string, string> input)
    {
        int id = Convert.ToInt32(e["ResellerID"]); string csv = IsPostBack ? String.Join(",", Request.Form.GetValues("rs_perm") ?? new string[0]) : SA.S(e["Permissions"]);
        var on = new HashSet<string>(csv.Split(','));
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; <a href='SupportResellers.aspx?view=").Append(id).Append("'>").Append(SA.H(e["ResellerName"])).Append("</a> &rsaquo; <strong>Edit Reseller</strong></nav>")
            .Append("<header class='sa-head'><div class='sa-head-ico'><i class='sa-kpi-icon sa-blue' aria-hidden='true'>&#9998;</i><div><h1>Edit Reseller</h1><p>Update reseller contact and account details.</p></div></div><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx?view=").Append(id).Append("'>&larr; Back to Reseller</a></header>")
            .Append("<input type='hidden' name='sa_id' value='").Append(id).Append("' /><div class='sa-rsl-form sa-rsl-cols'><div class='sa-rsl-col'><div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#127970;</i> Reseller Information</h2>")
            .Append(input("Reseller Name *", "rs_name", "text", " required maxlength='150'")).Append("<div class='sa-grid2'>").Append(input("Contact Person *", "rs_contact", "text", " required maxlength='150'"))
            .Append(input("Email *", "rs_email", "email", " required maxlength='200'")).Append(input("Phone *", "rs_phone", "tel", " required maxlength='30'")).Append(input("Alternate Phone", "rs_alt", "tel", " maxlength='30'"))
            .Append("</div></div>")
            .Append("<div class='sa-card' id='permissions'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#9881;</i> Permissions</h2><p class='sa-muted'>Select what the reseller can access and manage.</p><div class='sa-rsl-perms sa-rsl-perms1'>");
        foreach (var p in RS.PermissionList)
        {
            if (p[0] == "upgrades") sb.Append("<span class='sa-rsl-gap' aria-hidden='true'></span>");
            sb.Append("<label class='sa-check'><input type='checkbox' name='rs_perm' value='").Append(p[0]).Append("'").Append(on.Contains(p[0]) ? " checked" : "").Append(dis).Append(" /> ").Append(SA.H(p[1])).Append("</label>");
        }
        sb.Append("</div></div></div><div class='sa-rsl-col'><div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128274;</i> Account &amp; Access</h2>")
          .Append(input("Login Email", "rs_login", "email", "")).Append("<p class='sa-muted sa-small'>&#9432; Use Send Password Reset from the Reseller Actions menu to issue a new temporary password.</p><div class='sa-grid2'>")
          .Append("<label class='sa-field'><span>Status *</span><select name='rs_status'").Append(dis).Append(">").Append(SA.Options(new[] { "Active", "Inactive" }, Val("rs_status", D("rs_status").Length == 0 ? "Active" : D("rs_status")), null)).Append("</select></label>")
          .Append("<label class='sa-field'><span>Plan</span><select name='rs_plan'").Append(dis).Append(">").Append(SA.Options(RS.Plans, Val("rs_plan", D("rs_plan")), null)).Append("</select></label></div></div>")
          .Append("<div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128202;</i> Limits</h2><p class='sa-muted'>Set limits for the reseller.</p><div class='sa-grid2'>")
          .Append(input("Maximum Subscribers", "rs_maxsub", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("</label>", "<small class='sa-muted'>Leave blank for unlimited.</small></label>"))
          .Append(input("Maximum Businesses per Subscriber", "rs_maxbiz", "number", " min='0' max='100000' placeholder='Unlimited'").Replace("</label>", "<small class='sa-muted'>Leave blank for unlimited.</small></label>")).Append("</div></div>")
          .Append("<div class='sa-card'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#128221;</i> Notes</h2><p class='sa-muted'>Internal Notes (Support Admin Only)</p><label class='sa-field'><span class='sr-only'>Internal Notes</span><textarea name='rs_notes' rows='3' maxlength='1000' data-sa-counter='1000'").Append(dis).Append(">")
          .Append(SA.H(Val("rs_notes", D("rs_notes")))).Append("</textarea><small data-sa-count='1'>0/1000</small></label></div></div></div>")
          .Append(AddressCard(IsPostBack ? AD.FromForm("rs_addr") : AD.Parse(D("rs_address"))))
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx?view=").Append(id).Append("'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='update'").Append(dis).Append(">Save Changes</button></div>");
        return sb.ToString();
    }
    static object N(string s) { return String.IsNullOrEmpty(s) ? (object)DBNull.Value : s; }
    static readonly Regex Mail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    void Create(SqlConnection c)
    {
        string name = SA.Form("rs_name"), contact = SA.Form("rs_contact"), email = SA.Form("rs_email"), phone = SA.Form("rs_phone"), alt = SA.Form("rs_alt"), addr = "", plan = SA.Form("rs_plan"),
            login = SA.Form("rs_login"), pw = SA.Form("rs_pw"), status = SA.Form("rs_status") == "Inactive" ? "Inactive" : "Active", notes = SA.Form("rs_notes"), ms = SA.Form("rs_maxsub"), mb = SA.Form("rs_maxbiz");
        var errs = new List<string>();
        // v46 (Address deck s1): required structured address from the Address module, stored as one composed line in Address.
        var ad = AD.FromForm("rs_addr"); errs.AddRange(AD.Validate(ad, true)); addr = AD.Compose(ad);
        if (name.Length == 0) errs.Add("Reseller name is required."); if (contact.Length == 0) errs.Add("Contact person is required.");
        if (!Mail.IsMatch(email)) errs.Add("Enter a valid email address."); if (!Regex.IsMatch(phone, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Enter a valid phone number.");
        if (alt.Length > 0 && !Regex.IsMatch(alt, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Alternate phone is not valid.");
        if (!Mail.IsMatch(login)) errs.Add("Enter a valid login email."); if (pw.Length < 8) errs.Add("Temporary password must be at least 8 characters.");
        if (!RS.Plans.Contains(plan)) plan = "Standard";
        int x; int? maxSub = null, maxBiz = null;
        if (ms.Length > 0) { if (Int32.TryParse(ms, out x) && x >= 0) maxSub = x; else errs.Add("Max subscribers must be a whole number."); }
        if (mb.Length > 0) { if (Int32.TryParse(mb, out x) && x >= 0) maxBiz = x; else errs.Add("Max businesses per subscriber must be a whole number."); }
        if (errs.Count == 0 && SA.Q(c, "SELECT 1 FROM AnyminuteReseller WHERE LoginEmail=@l", "@l", login).Rows.Count > 0) errs.Add("Another reseller already uses this login email.");
        // v44: the login must fit SP_LOGIN (nvarchar(50)) and must not belong to another AnyMinute login.
        if (login.Length > RS.MaxLoginEmail) errs.Add("Login email must be " + RS.MaxLoginEmail + " characters or fewer.");
        if (pw.Length > RS.MaxPassword) errs.Add("Temporary password must be " + RS.MaxPassword + " characters or fewer.");
        if (errs.Count == 0 && RS.LoginTaken(c, login)) errs.Add("This login email is already registered to another AnyMinute account.");
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
        // v44: create the sign-in (UserAcc) with this exact password so the welcome email credentials work on Login.aspx.
        string loginErr; try { loginErr = RS.EnsureLogin(c, SA.Q(c, "SELECT * FROM AnyminuteReseller WHERE ResellerID=@i", "@i", id).Rows[0], pw); } catch (Exception ex) { loginErr = ex.Message; }
        if (loginErr != null) { SA.Audit(c, null, "Reseller", id.ToString(Inv), "Login Setup Failed", null, null, null, login, loginErr); SA.Flash(msg + " Sign-in was NOT set up: " + loginErr + " No welcome email was sent; fix this and use Send Password Reset."); Go("?view=" + id); return; }
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
    void Update(SqlConnection c, int id, DataRow old)
    {
        string name = SA.Form("rs_name"), contact = SA.Form("rs_contact"), email = SA.Form("rs_email"), phone = SA.Form("rs_phone"), alt = SA.Form("rs_alt"), addr = "", plan = SA.Form("rs_plan"),
            status = SA.Form("rs_status") == "Inactive" ? "Inactive" : "Active", notes = SA.Form("rs_notes"), ms = SA.Form("rs_maxsub"), mb = SA.Form("rs_maxbiz");
        var errs = new List<string>();
        // v46 (Address deck s1): required structured address from the Address module, stored as one composed line in Address.
        var ad = AD.FromForm("rs_addr"); errs.AddRange(AD.Validate(ad, true)); addr = AD.Compose(ad);
        if (name.Length == 0) errs.Add("Reseller name is required."); if (contact.Length == 0) errs.Add("Contact person is required.");
        if (!Mail.IsMatch(email)) errs.Add("Enter a valid email address."); if (!Regex.IsMatch(phone, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Enter a valid phone number.");
        if (alt.Length > 0 && !Regex.IsMatch(alt, @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Alternate phone is not valid.");
        if (!RS.Plans.Contains(plan)) plan = SA.S(old["PlanName"]);
        int x; int? maxSub = null, maxBiz = null;
        if (ms.Length > 0) { if (Int32.TryParse(ms, out x) && x >= 0) maxSub = x; else errs.Add("Max subscribers must be a whole number."); }
        if (mb.Length > 0) { if (Int32.TryParse(mb, out x) && x >= 0) maxBiz = x; else errs.Add("Max businesses per subscriber must be a whole number."); }
        if (errs.Count > 0) { Flash.Text = "<div class='sa-flash sa-warn'>Please fix the following:<ul>" + String.Join("", errs.Select(m => "<li>" + SA.H(m) + "</li>").ToArray()) + "</ul></div>"; Body1.Text = AddForm(true, Row(c, id)); return; }
        var perms = Request.Form.GetValues("rs_perm") ?? new string[0];
        string csv = String.Join(",", RS.PermissionList.Select(p => p[0]).Where(perms.Contains).ToArray());
        Func<object, string> lim = o => o == null || o == DBNull.Value ? "unlimited" : Convert.ToString(o, Inv);
        string before = SA.S(old["ResellerName"]) + " | " + SA.S(old["Email"]) + " | " + SA.S(old["PlanName"]) + " | " + St(old) + " | limits " + lim(old["MaxSubscribers"]) + "/" + lim(old["MaxBusinessesPerSubscriber"]) + " | " + SA.S(old["Address"]),
            after = name + " | " + email + " | " + plan + " | " + status + " | limits " + lim(maxSub) + "/" + lim(maxBiz) + " | " + addr;
        using (var tx = c.BeginTransaction())
        {
            AnyminuteTimesheetRepair.Execute(c, tx, @"UPDATE AnyminuteReseller SET ResellerName=@n,ContactPerson=@c,Email=@e,Phone=@p,AltPhone=@a,Address=@ad,PlanName=@pl,Status=@st,Permissions=@pm,MaxSubscribers=@ms,MaxBusinessesPerSubscriber=@mb,Notes=@no,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@by WHERE ResellerID=@i",
                "@n", name, "@c", contact, "@e", email, "@p", phone, "@a", N(alt), "@ad", N(addr), "@pl", plan, "@st", status, "@pm", csv,
                "@ms", maxSub.HasValue ? (object)maxSub.Value : DBNull.Value, "@mb", maxBiz.HasValue ? (object)maxBiz.Value : DBNull.Value, "@no", N(notes), "@by", SA.ActorName(), "@i", id);
            SA.Audit(c, tx, "Reseller", id.ToString(Inv), "Reseller Updated", null, null, before, after, "Permissions: " + RS.PermissionText(csv));
            tx.Commit();
        }
        SA.Flash("Reseller " + name + " updated."); Go("?view=" + id);
    }
    // ---------- View ----------
    string View(SqlConnection c, int id)
    {
        var t = Row(c, id);
        if (t == null) return NotFound;
        Title = SA.S(t["ResellerName"]);
        Func<string, object, string> kv = (k, v) => "<div><dt>" + k + "</dt><dd>" + (SA.S(v).Length == 0 ? "&mdash;" : SA.H(v)) + "</dd></div>";
        Func<object, string> lim = o => o == DBNull.Value ? "Unlimited" : SA.S(o);
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; ").Append(SA.H(t["ResellerName"])).Append("</nav><header class='sa-head'><div><h1>").Append(SA.H(t["ResellerName"])).Append("</h1><p>")
            .Append(RS.Rsl(id)).Append(" &middot; ").Append(SA.Pill(St(t), St(t) == "Active" ? "green" : "red")).Append(" &middot; Login ").Append(Convert.ToBoolean(t["LoginDisabled"]) ? "disabled" : "enabled").Append("</p></div><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx'>Back</a></header>");
        sb.Append("<div class='sa-grid2'><div class='sa-card'><h2>Reseller Information</h2><dl class='sa-kv'>").Append(kv("Contact Person", t["ContactPerson"])).Append(kv("Email", t["Email"])).Append(kv("Phone", t["Phone"])).Append(kv("Alternate Phone", t["AltPhone"]))
          .Append(SA.S(t["Address"]).Trim().Length > 0 ? kv("Address", t["Address"]) : "<div><dt>Address</dt><dd>&mdash; <a class='sa-link' href='SupportResellers.aspx?edit=" + id + "'>Add address</a></dd></div>").Append(kv("Plan", t["PlanName"]))
          // v47: Created shows the date (was the creator's name only, e.g. "Support Admin"); the creator follows it.
          .Append("<div><dt>Created Date</dt><dd>").Append(CDay(t["CreatedAtUTC"])).Append(SA.S(t["CreatedBy"]).Length > 0 ? " <small class='sa-muted'>by " + SA.H(t["CreatedBy"]) + "</small>" : "").Append("</dd></div>").Append("</dl></div><div class='sa-card'><h2>Account &amp; Limits</h2><dl class='sa-kv'>").Append(kv("Login Email", t["LoginEmail"]))
          .Append(kv("Invitation", t["InvitationStatus"])).Append(kv("Max Subscribers", lim(t["MaxSubscribers"]))).Append(kv("Max Businesses / Subscriber", lim(t["MaxBusinessesPerSubscriber"]))).Append(kv("Subscribers", t["Subscribers"])).Append(kv("Businesses", t["Businesses"]))
          .Append(kv("Permissions", RS.PermissionText(SA.S(t["Permissions"])))).Append(kv("Internal Notes", t["Notes"])).Append("</dl></div></div>");
        return sb.ToString();
    }
    // v42 PPT 02 s2 menu targets (moved off the view page per PPT 04 s2): View Subscribers (with Assign), View Businesses, View Activity History.
    string Sub(SqlConnection c, int id, string tab)
    {
        var t = Row(c, id); if (t == null) return NotFound;
        string label = tab == "subscribers" ? "Subscribers" : tab == "businesses" ? "Businesses" : "Activity History";
        Title = SA.S(t["ResellerName"]) + " - " + label;
        var sb = new StringBuilder("<nav class='sa-crumbs'><a href='SupportResellers.aspx'>Resellers</a> &rsaquo; <a href='SupportResellers.aspx?view=").Append(id).Append("'>").Append(SA.H(t["ResellerName"])).Append("</a> &rsaquo; <strong>").Append(label).Append("</strong></nav>")
            .Append("<header class='sa-head'><div><h1>").Append(label).Append("</h1><p>").Append(SA.H(t["ResellerName"])).Append("</p></div><a class='sa-btn sa-btn-outline' href='SupportResellers.aspx?view=").Append(id).Append("'>Back</a></header><div class='sa-card'>");
        if (tab == "history") return sb.Append(SA.HistoryTable(SA.History(c, "Reseller", id.ToString(Inv)), "No activity recorded for this reseller yet.")).Append("</div>").ToString();
        if (tab == "businesses")
        {
            var b = SA.Q(c, @"SELECT DISTINCT b.BusinessID,b.CompanyName,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Owner,u.Email,u.UserId FROM AnyminuteResellerSubscriber s JOIN UserAcc u ON u.UserId=s.UserId
                LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId JOIN AssginBussiness a ON a.EmployeeID=u.EmployeeId AND a.UserTypeId=1 JOIN AddBusiness b ON b.BusinessID=a.BusinessID AND ISNULL(b.Status,'')<>'2' WHERE s.ResellerID=@i ORDER BY b.CompanyName", "@i", id);
            sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>#</th><th>Business Name</th><th>Subscriber</th><th>Email</th></tr></thead><tbody>"); int n = 0;
            foreach (DataRow r in b.Rows) sb.Append("<tr><td>").Append(++n).Append("</td><td><strong>").Append(SA.H(r["CompanyName"])).Append("</strong></td><td><a class='sa-link' href='SupportSubscribers.aspx?user=").Append(r["UserId"]).Append("'>").Append(SA.H(r["Owner"])).Append("</a></td><td>").Append(SA.H(r["Email"])).Append("</td></tr>");
            if (n == 0) sb.Append("<tr><td colspan='4' class='sa-rpt-empty'>No businesses under this reseller's subscribers yet.</td></tr>");
            return sb.Append("</tbody></table></div></div>").ToString();
        }
        var subs = RS.Subscribers(c, 0).Rows.Cast<DataRow>().ToList(); var mine = subs.Where(x => x["ResellerID"] != DBNull.Value && Convert.ToInt32(x["ResellerID"]) == id).ToList();
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Subscriber ID</th><th>Name</th><th>Email</th><th>Plan</th><th>Businesses</th><th>Assigned</th></tr></thead><tbody>");
        foreach (var x in mine) sb.Append("<tr><td>").Append(RS.Sub(x["UserId"])).Append("</td><td><a class='sa-link' href='SupportSubscribers.aspx?user=").Append(x["UserId"]).Append("'>").Append(SA.H(x["Name"])).Append("</a></td><td>").Append(SA.H(x["Email"])).Append("</td><td>").Append(SA.H(x["PlanName"])).Append("</td><td>").Append(x["Businesses"]).Append("</td><td>").Append(SA.When(x["AssignedAtUTC"])).Append("</td></tr>");
        if (mine.Count == 0) sb.Append("<tr><td colspan='6' class='sa-rpt-empty'>No subscribers assigned to this reseller yet.</td></tr>");
        sb.Append("</tbody></table></div>");
        var free = subs.Where(x => x["ResellerID"] == DBNull.Value).ToList();
        sb.Append("<h3>Assign Subscriber</h3><div class='sa-filters'><input type='hidden' name='sa_id' value='").Append(id).Append("' /><label class='sa-grow'><span>Unassigned subscribers</span><select name='rs_user'><option value=''>Select subscriber</option>");
        foreach (var x in free) sb.Append("<option value='").Append(x["UserId"]).Append("'>").Append(SA.H(RS.Sub(x["UserId"]) + " - " + (SA.S(x["Name"]).Length > 0 ? SA.S(x["Name"]) : SA.S(x["Email"])) + " (" + SA.S(x["Email"]) + ")")).Append("</option>");
        return sb.Append("</select></label><button type='submit' class='sa-btn' name='sa_op' value='assign' formnovalidate='formnovalidate'>Assign to Reseller</button></div><p class='sa-muted sa-small'>Each subscriber belongs to one reseller. The max subscriber limit is enforced.</p></div>").ToString();
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
                try { RS.SyncAccess(c, SA.Q(c, "SELECT * FROM AnyminuteReseller WHERE ResellerID=@i", "@i", id).Rows[0], 0); } catch (SqlException) { }
                SA.Flash(name + " is now " + ns + "."); Go(""); return;
            }
            if (op == "login")
            {
                SA.X(c, null, "UPDATE AnyminuteReseller SET LoginDisabled=@d,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@b WHERE ResellerID=@i", "@d", !off, "@b", SA.ActorName(), "@i", id);
                SA.Audit(c, null, "Reseller", key, off ? "Login Enabled" : "Login Disabled", null, null, off ? "Disabled" : "Enabled", off ? "Enabled" : "Disabled", null);
                try { RS.SyncAccess(c, SA.Q(c, "SELECT * FROM AnyminuteReseller WHERE ResellerID=@i", "@i", id).Rows[0], 0); } catch (SqlException) { }
                SA.Flash("Login " + (off ? "enabled" : "disabled") + " for " + name + "."); Go(""); return;
            }
            if (op == "update") { Update(c, id, r.Rows[0]); return; }
            if (op == "resetpw")
            {
                // v42 PPT 02 s2 "Send Password Reset": there is no reseller portal sign-in page to link to yet, so a new temporary
                // password is issued (salted hash only) and emailed when IsSendMail is on; shown once otherwise.
                string pw = RS.NewTempPassword(), salt = RS.Salt(), mail = SA.S(r.Rows[0]["Email"]);
                SA.X(c, null, "UPDATE AnyminuteReseller SET TempPasswordHash=@h,TempPasswordSalt=@s,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@b WHERE ResellerID=@i", "@h", RS.Hash(salt, pw), "@s", salt, "@b", SA.ActorName(), "@i", id);
                // v44: the new password is also written to the reseller's sign-in (created here for resellers added before v44).
                string loginErr; try { loginErr = RS.EnsureLogin(c, r.Rows[0], pw); } catch (Exception ex) { loginErr = ex.Message; }
                if (loginErr != null) { SA.Flash("Password not reset: " + loginErr); Go(""); return; }
                SA.Audit(c, null, "Reseller", key, "Password Reset", null, null, null, mail, "New temporary password issued by Support Admin.");
                string msg = "New temporary password for " + name + ": " + pw + " (shown once; share it securely).";
                bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
                if (send && Mail.IsMatch(mail))
                    try
                    {
                        new EmailSettingController().SendHtmlFormattedEmail(mail, "Your AnyMinute reseller password was reset", "<p>Hello " + HttpUtility.HtmlEncode(SA.S(r.Rows[0]["ContactPerson"])) + ",</p><p>A new temporary password was issued for your AnyMinute reseller account.</p><p>Login email: <strong>" + HttpUtility.HtmlEncode(SA.S(r.Rows[0]["LoginEmail"])) + "</strong><br />Temporary password: <strong>" + HttpUtility.HtmlEncode(pw) + "</strong></p><p>Please change this password the next time you sign in.</p><p>Thank you,<br />The AnyMinute Team</p>");
                        msg = "Password reset email sent to " + mail + ".";
                    }
                    catch (Exception ex) { msg += " Email could not be sent: " + ex.Message; }
                SA.Flash(msg); Go(""); return;
            }
            if (op == "assign")
            {
                int u; if (!Int32.TryParse(SA.Form("rs_user"), out u)) { SA.Flash("Choose a subscriber to assign."); Go("?view=" + id + "&tab=subscribers"); return; }
                object max = r.Rows[0]["MaxSubscribers"];
                int count = Convert.ToInt32(SA.Q(c, "SELECT COUNT(*) FROM AnyminuteResellerSubscriber WHERE ResellerID=@i", "@i", id).Rows[0][0]);
                if (max != DBNull.Value && count >= Convert.ToInt32(max)) { SA.Flash(name + " has reached its limit of " + max + " subscribers."); Go("?view=" + id + "&tab=subscribers"); return; }
                if (SA.Q(c, "SELECT 1 FROM AnyminuteResellerSubscriber WHERE UserId=@u", "@u", u).Rows.Count > 0) { SA.Flash("That subscriber already belongs to a reseller."); Go("?view=" + id + "&tab=subscribers"); return; }
                SA.X(c, null, "INSERT AnyminuteResellerSubscriber(UserId,ResellerID,AssignedBy) VALUES(@u,@i,@b)", "@u", u, "@i", id, "@b", SA.ActorName());
                SA.Audit(c, null, "Reseller", key, "Subscriber Assigned", null, null, null, RS.Sub(u), null);
                SA.Flash(RS.Sub(u) + " assigned to " + name + "."); Go("?view=" + id + "&tab=subscribers"); return;
            }
            Go("");
        }
    }
}
