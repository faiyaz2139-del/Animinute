using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;
using RP = AnyminuteResellerPortal;
using AD = AnyminuteSupportAddress;

// 2026-11-03 v55 (Reseller PPT collage): Reseller portal home. view = (none) Dashboard (s1 KPIs + Quick Actions) | businesses
// (businesses under the reseller's subscribers) | plans (s9 Subscription Plans; v60 + Add Plan for own plans) | usage (Usage & Limits,
// read-only) | settings (profile / security links). Subscribers (s2-s8) live on SupportSubscribers.aspx in reseller mode.
public partial class AnyminuteResellerDashboardPage : System.Web.UI.Page
{
    RslCtx rsl;
    protected void Page_Init(object sender, EventArgs e) { rsl = RP.Require(); }
    static readonly Dictionary<string, string> Icons = new Dictionary<string, string>
    {
        { "users", "<path d='M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2'/><circle cx='9' cy='7' r='4'/><path d='M23 21v-2a4 4 0 0 0-3-3.9M16 3.1a4 4 0 0 1 0 7.8'/>" },
        { "check", "<circle cx='12' cy='12' r='10'/><path d='m8 12 3 3 5-6'/>" },
        { "x", "<circle cx='12' cy='12' r='10'/><path d='m15 9-6 6M9 9l6 6'/>" },
        { "biz", "<rect x='4' y='2' width='16' height='20' rx='1'/><path d='M9 22v-4h6v4M8 6h2M14 6h2M8 10h2M14 10h2M8 14h2M14 14h2'/>" },
        { "folder", "<path d='M22 19a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h5l2 3h9a2 2 0 0 1 2 2z'/>" },
        { "useradd", "<path d='M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2'/><circle cx='9' cy='7' r='4'/><path d='M19 8v6M22 11h-6'/>" },
        { "list", "<path d='M8 6h13M8 12h13M8 18h13M3 6h.01M3 12h.01M3 18h.01'/>" },
        { "doc", "<path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'/><path d='M14 2v6h6M8 13h8M8 17h8'/>" },
        { "bars", "<path d='M3 3v18h18'/><path d='M8 17v-5M13 17V8M18 17v-9'/>" },
        { "report", "<path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'/><path d='M14 2v6h6M9 15h6M9 11h2'/>" },
        { "search", "<circle cx='11' cy='11' r='8'/><path d='m21 21-4.3-4.3'/>" }
    };
    static string Ic(string n) { return "<svg viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true' focusable='false'>" + Icons[n] + "</svg>"; }
    static string Kpi(string icon, string tone, string label, int n) { return "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + Ic(icon) + "</i><div><span>" + label + "</span><strong>" + n.ToString("#,0", CultureInfo.InvariantCulture) + "</strong></div></div>"; }
    static string Used(int used, object lim) { string l = SA.S(lim); return used + (l.Length > 0 ? " / " + SA.H(l) : ""); }
    static string Crumb(string label) { return "<nav class='sa-crumbs' aria-label='Breadcrumb'><a href='ResellerDashboard.aspx'>Dashboard</a> &rsaquo; <span>" + label + "</span></nav>"; }
    protected void Page_Load(object sender, EventArgs e)
    {
        // v67: JSON postal lookup for the Address module (My Profile, Add / Edit Business).
        if (SA.Get("lookup") == "1") { Response.Clear(); Response.ContentType = "application/json"; Response.Write(AD.LookupJson(SA.Get("postal"))); Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest(); return; }
        Flash.Text = SA.TakeFlash();
        string view = SA.Get("view");
        // Permission gates (v56: nav always shows all 7; gates stay here): businesses = businesses or subscribers, plans, usage.
        // v61: a gated page keeps its own title + highlighted nav item and explains the missing permission (v56-v60 bounced to the Dashboard).
        if ((view == "businesses" && !rsl.Can("businesses") && !rsl.Can("subscribers")) || (view == "plans" && !rsl.Can("plans")) || (view == "usage" && !rsl.Can("usage")))
        {
            string t = view == "plans" ? "Subscription Plans" : view == "usage" ? "Usage &amp; Limits" : "Businesses";
            Crumbs.Text = Crumb(t); Title1.Text = t; Lead.Text = "";
            Content.Text = "<div class='sa-card'><p class='sa-muted'>Your reseller account does not have the " + t + " permission. Contact AnyMinute Support to turn it on.</p></div>"; return;
        }
        using (var c = SA.Open())
        {
            try
            {
                if (view == "businesses") Businesses(c);
                else if (view == "plans") Plans(c);
                else if (view == "usage") Usage(c);
                else if (view == "settings") Settings();
                else if (view == "profile") ShowMyProfile(c);
                else Dashboard(c);
            }
            catch (SqlException ex)
            {
                log4net.LogManager.GetLogger("Log").Error("Reseller portal " + view, ex);
                Content.Text = "<div class='sa-card'><p class='sa-muted'>This page could not be loaded right now. Please try again or contact AnyMinute Support.</p></div>";
            }
        }
    }
    // s1: Reseller Dashboard - Total / Active / Inactive Subscribers, Total Businesses / Employees / Projects + Quick Actions.
    void Dashboard(SqlConnection c)
    {
        Crumbs.Text = "<nav class='sa-crumbs rsl-home-crumb' aria-label='Breadcrumb'><span>Reseller</span></nav>"; // v56 PPT s1: bold Reseller above the title
        Title1.Text = "Reseller Dashboard"; Lead.Text = "Manage your subscribers, businesses and subscription plans.";
        var t = RP.Subscribers(c, rsl).Rows.Cast<DataRow>().ToList();
        int act = t.Count(r => RP.StatusOf(r) == "Active"), ina = t.Count(r => RP.StatusOf(r) == "Inactive" || RP.StatusOf(r) == "Suspended");
        var sb = new StringBuilder("<div class='rsl-kpis'>")
            .Append(Kpi("users", "blue", "Total Subscribers", t.Count)).Append(Kpi("check", "green", "Active", act)).Append(Kpi("x", "red", "Inactive", ina))
            .Append(Kpi("biz", "purple", "Total Businesses", t.Sum(r => Convert.ToInt32(r["Businesses"])))).Append(Kpi("users", "blue", "Total Employees", t.Sum(r => Convert.ToInt32(r["Employees"]))))
            .Append(Kpi("folder", "purple", "Total Projects", t.Sum(r => Convert.ToInt32(r["Projects"])))).Append("</div>");
        // v56 (PPT s1 strict): all five Quick Actions always show; the target pages keep their server-side permission gates (flash + redirect).
        Func<string, string, string, string> a = (href, icon, label) => "<a href='" + href + "'>" + Ic(icon) + "<span>" + label + "</span></a>";
        sb.Append("<div class='sa-card rsl-qa-card'><h2>Quick Actions</h2><div class='rsl-qa'>")
            .Append(a("SupportSubscribers.aspx?view=add", "useradd", "Add Subscriber")).Append(a("SupportSubscribers.aspx", "list", "View Subscribers"))
            .Append(a("ResellerDashboard.aspx?view=plans", "doc", "Subscription Plans")).Append(a("ResellerDashboard.aspx?view=usage", "bars", "Usage Limits"))
            .Append(a("SupportReports.aspx", "report", "Reports")).Append("</div></div>");
        Content.Text = sb.ToString();
    }
    // Businesses nav: businesses under this reseller's subscribers (search + pager).
    // v67 (Reseller PPT s5): + Add Business (header), Actions = View + kebab (Edit Business, View Subscriber), Add / Edit Business form
    // scoped to the reseller's own subscribers (subscriber picker on Add). Saves through AnyminuteBusinessSave (same SPs as the subscriber
    // Add_Business page). A business id outside RP.Businesses(c, rsl) is never shown, edited or saved.
    void Businesses(SqlConnection c)
    {
        Crumbs.Text = Crumb("Businesses"); Title1.Text = "Businesses"; Lead.Text = "Businesses under your subscribers.";
        HeadActions.Text = "<a class='sa-btn' href='ResellerDashboard.aspx?view=businesses&amp;add=1#biz-form'><b aria-hidden='true'>+</b> Add Business</a>";
        string q = SA.Get("q"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1; int size = SA.PageSizeArg(10);
        var all = RP.Businesses(c, rsl).Rows.Cast<DataRow>().ToList();
        string form = null;
        if (IsPostBack && SA.Form("rp_op") == "biz") { form = BizSave(c, all); if (form == "") return; }
        int eid; Int32.TryParse(SA.Get("edit"), out eid); int vid; Int32.TryParse(SA.Get("biz"), out vid);
        if (form == null && SA.Get("add") == "1") form = BizForm(c, null, null);
        else if (form == null && eid > 0)
        {
            var own = all.FirstOrDefault(r => Convert.ToInt32(r["BusinessID"]) == eid && SA.S(r["St"]) != "Deleted");
            form = own == null ? "<div class='sa-card' id='biz-form'><p class='sa-muted'>This business is not under one of your subscribers, so it cannot be edited here.</p></div>" : BizForm(c, own, null);
        }
        var rows = all.Where(r => q.Length == 0 || (SA.S(r["CompanyName"]) + " " + SA.S(r["Name"]) + " " + SA.S(r["Email"])).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        int pages = Math.Max(1, (rows.Count + size - 1) / size); if (page > pages) page = pages;
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-filters' data-sa-get='ResellerDashboard.aspx'><input type='hidden' name='view' value='businesses' /><input type='hidden' name='size' value='").Append(size)
            .Append("' /><label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>").Append(Ic("search")).Append("</i><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by business or subscriber...' /></label>")
            .Append("<a class='sa-btn sa-btn-outline' href='ResellerDashboard.aspx?view=businesses'>Clear</a><button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>")
            .Append("<div class='sa-table-wrap' style='overflow:visible'><table class='sa-table'><thead><tr><th>#</th><th>Business Name</th><th>Subscriber</th><th class='sa-c'>Employees</th><th class='sa-c'>Projects</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        int n = (page - 1) * size; DataRow pick = null;
        foreach (var r in all) if (Convert.ToInt32(r["BusinessID"]) == vid) pick = r;
        foreach (var r in rows.Skip((page - 1) * size).Take(size))
        {
            int bid = Convert.ToInt32(r["BusinessID"]);
            string sub = "SupportSubscribers.aspx?user=" + Convert.ToInt32(r["UserId"]), st = SA.S(r["St"]); bool open = rsl.Can("subscribers");
            string viewHref = "ResellerDashboard.aspx?view=businesses&amp;biz=" + bid + "#biz";
            sb.Append("<tr><td>").Append(++n).Append("</td><td>").Append(SA.H(r["CompanyName"])).Append("</td><td>").Append(open ? "<a class='sa-link' href='" + sub + "'>" + SA.H(SA.S(r["Name"]).Length > 0 ? r["Name"] : r["Email"]) + "</a>" : SA.H(SA.S(r["Name"]).Length > 0 ? r["Name"] : r["Email"]))
              .Append("</td><td class='sa-c'>").Append(r["Employees"]).Append("</td><td class='sa-c'>").Append(r["Projects"]).Append("</td><td>").Append(SA.Pill(st, st == "Active" ? "green" : "grey")).Append("</td><td class='sa-c sa-nowrap'>")
              .Append("<a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(viewHref).Append("'>View</a> <details class='sa-menu'><summary class='sa-kebab' aria-label='Business actions'><svg class='sa-i' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' aria-hidden='true' focusable='false'><circle cx='12' cy='5' r='1'/><circle cx='12' cy='12' r='1'/><circle cx='12' cy='19' r='1'/></svg></summary><div class='sa-menu-pop'><h4>Business Actions</h4><small>").Append(SA.H(r["CompanyName"])).Append("</small>")
              .Append("<a class='sa-menu-item' href='").Append(viewHref).Append("'><span><b>View Business</b><small>Business details and address</small></span></a>")
              .Append(st != "Deleted" ? "<a class='sa-menu-item' href='ResellerDashboard.aspx?view=businesses&amp;edit=" + bid + "#biz-form'><span><b>Edit Business</b><small>Name, contact, status and address</small></span></a>" : "")
              .Append(open ? "<a class='sa-menu-item' href='" + sub + "&amp;view=businesses'><span><b>View Subscriber</b><small>The subscriber's businesses tab</small></span></a>" : "").Append("</div></details></td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No businesses found.</td></tr>");
        sb.Append("</tbody></table></div>").Append(SA.Pager46(rows.Count, page, size, "ResellerDashboard.aspx?view=businesses&q=" + HttpUtility.UrlEncode(q), "business")).Append("</div>");
        if (pick != null) sb.Append(BizView(c, pick));
        Content.Text = sb.ToString() + (form ?? "");
    }
    static readonly string[] Legal = { "Sole Proprietorships", "Partnerships", "Corporations", "S Corporations", "Limited Liability Company (LLC)" };
    static DataRow BizRow(SqlConnection c, int id) { var t = SA.Q(c, "SELECT * FROM AddBusiness WHERE BusinessID=@b", "@b", id); return t.Rows.Count == 1 ? t.Rows[0] : null; }
    static DataRow BizAddr(SqlConnection c, DataRow b) { int a; if (b == null || !Int32.TryParse(SA.S(b["ADDRESSID"]), out a) || a <= 0) return null; var t = SA.Q(c, "SELECT * FROM BusinessAddress WHERE BusinessAddressId=@a", "@a", a); return t.Rows.Count == 1 ? t.Rows[0] : null; }
    static string Col(DataRow r, string k) { return r != null && r.Table.Columns.Contains(k) ? SA.S(r[k]).Trim() : ""; }
    static string Pick(string v) { v = (v ?? "").Trim(); return v.Equals("Please Select", StringComparison.OrdinalIgnoreCase) || v == "0" ? "" : v; }
    static AD.Addr BizAd(DataRow a)
    {
        string country = Col(a, "Country").Equals("United States", StringComparison.OrdinalIgnoreCase) ? "United States" : "Canada", prov = Col(a, "Province");
        var x = new AD.Addr { StreetNo = Col(a, "StreetNo"), Line1 = Col(a, "StreetName"), Line2 = Col(a, "StreetType"), SuiteType = AD.SuiteTypes.FirstOrDefault(z => z.Equals(Pick(Col(a, "SuitType")), StringComparison.OrdinalIgnoreCase)) ?? "",
            SuiteNo = Col(a, "SuiteNumber"), City = Col(a, "City"), Province = (country == "United States" ? AD.States : AD.Canada).FirstOrDefault(z => z.Equals(prov, StringComparison.OrdinalIgnoreCase)) ?? prov, Country = country };
        x.Postal = AD.NormalPostal(Col(a, "Postalcode"), country); return x;
    }
    // View Business card (read-only).
    string BizView(SqlConnection c, DataRow r)
    {
        var b = BizRow(c, Convert.ToInt32(r["BusinessID"])); var a = BizAddr(c, b);
        Func<string, string, string> kv = (k, v) => "<tr><th>" + k + "</th><td>" + (v.Length > 0 ? SA.H(v) : "&mdash;") + "</td></tr>";
        string st = Col(b, "Status"); bool del = SA.S(r["St"]) == "Deleted";
        return "<div class='sa-card' id='biz'><div class='sa-panel-head'><h2>" + SA.H(r["CompanyName"]) + "</h2><div>" + (!del ? "<a class='sa-btn sa-btn-sm' href='ResellerDashboard.aspx?view=businesses&amp;edit=" + Convert.ToInt32(r["BusinessID"]) + "#biz-form'>Edit</a> " : "")
            + "<a class='sa-btn sa-btn-outline sa-btn-sm' href='ResellerDashboard.aspx?view=businesses'>Close</a></div></div><table class='sa-kv'>"
            + kv("Subscriber", SA.S(r["Name"]).Length > 0 ? SA.S(r["Name"]) + " (" + SA.S(r["Email"]) + ")" : SA.S(r["Email"])) + kv("Operating Name", Col(b, "OperatingName")) + kv("Business Number", Col(b, "BusinessNumber"))
            + kv("Legal Entity Type", Pick(Col(b, "LegalEntityType")) == "Select" ? "" : Pick(Col(b, "LegalEntityType"))) + kv("Primary Contact", Col(b, "PrimaryContactName")) + kv("Contact Number", Col(b, "PrimaryContactNumber")) + kv("Primary Email", Col(a, "MainEmail"))
            + kv("Status", del ? "Deleted" : st == "0" ? "Not Active" : st == "2" ? "Not Active (deactivated by plan)" : "Active") + kv("Address", a == null ? "" : AD.Compose(BizAd(a))) + "</table></div>";
    }
    // Add / Edit Business card. own = RP.Businesses row (edit) or null (add); v = posted values after a failed save.
    string BizForm(SqlConnection c, DataRow own, Dictionary<string, string> v)
    {
        bool add = own == null && (v == null || v["bz_id"] == "0"); bool can = rsl.Can("businesses"); string dis = can ? "" : " disabled";
        int id = own != null ? Convert.ToInt32(own["BusinessID"]) : v != null ? Convert.ToInt32(v["bz_id"]) : 0;
        DataRow b = id > 0 ? BizRow(c, id) : null, a = BizAddr(c, b);
        Func<string, string, string> val = (k, col) => v != null ? (v.ContainsKey(k) ? v[k] : "") : b == null ? "" : k == "bz_email" ? Col(a, "MainEmail") : Col(b, col);
        Func<string, string, string, string, string> input = (label, k, col, extra) => "<label class='sa-field'><span>" + label + "</span><input name='" + k + "' value='" + SA.A(val(k, col)) + "' " + extra + dis + " /></label>";
        string legal = val("bz_legal", "LegalEntityType").Trim(), st = v != null ? v["bz_status"] : b == null ? "1" : Col(b, "Status");
        var sb = new StringBuilder("<div class='sa-card rsl-bizform' id='biz-form'><div class='sa-panel-head'><div><h2>").Append(add ? "Add Business" : "Edit Business").Append("</h2><small>")
            .Append(add ? "Create a business for one of your subscribers." : "Update the business details, status and address.").Append("</small></div><a class='sa-x' href='ResellerDashboard.aspx?view=businesses' aria-label='Close'>&times;</a></div>");
        if (!can) sb.Append("<div class='sa-flash sa-warn'>Your reseller account does not have the Businesses permission. Contact AnyMinute Support to add or edit businesses.</div>");
        sb.Append("<input type='hidden' name='bz_id' value='").Append(id).Append("' />");
        if (add)
        {
            string sel = v != null ? v["bz_sub"] : SA.Get("sub");
            sb.Append("<label class='sa-field'><span>Subscriber <em class='sa-req'>*</em></span><select name='bz_sub' required").Append(dis).Append("><option value=''>Select subscriber</option>");
            foreach (DataRow s in RP.Subscribers(c, rsl).Rows)
            {
                string u = Convert.ToInt32(s["UserId"]).ToString(CultureInfo.InvariantCulture), nm = SA.S(s["Name"]).Length > 0 ? SA.S(s["Name"]) + " (" + SA.S(s["Email"]) + ")" : SA.S(s["Email"]);
                sb.Append("<option value='").Append(u).Append("'").Append(u == sel ? " selected" : "").Append(">").Append(SA.H(nm)).Append("</option>");
            }
            sb.Append("</select></label>");
        }
        else sb.Append("<label class='sa-field'><span>Subscriber</span><input value='").Append(SA.A(own != null ? (SA.S(own["Name"]).Length > 0 ? own["Name"] : own["Email"]) : v.ContainsKey("bz_subname") ? v["bz_subname"] : "")).Append("' readonly /></label>");
        sb.Append("<h2>Business Information</h2><div class='sa-grid2'>").Append(input("Company Name <em class='sa-req'>*</em>", "bz_name", "CompanyName", "maxlength='100' required")).Append(input("Operating Name", "bz_oper", "OperatingName", "maxlength='100'"))
          .Append(input("Business Number", "bz_num", "BusinessNumber", "maxlength='50'"))
          .Append("<label class='sa-field'><span>Legal Entity Type</span><select name='bz_legal'").Append(dis).Append("><option value='Select'>Select legal entity type</option>");
        var opts = Legal.ToList(); if (legal.Length > 0 && legal != "Select" && !opts.Any(o => o.Equals(legal, StringComparison.OrdinalIgnoreCase))) opts.Add(legal);
        foreach (var o in opts) sb.Append("<option").Append(o.Equals(legal, StringComparison.OrdinalIgnoreCase) ? " selected" : "").Append(">").Append(SA.H(o)).Append("</option>");
        sb.Append("</select></label>").Append(input("Business Registration Date", "bz_reg", "BusinessRegistrationDate", "maxlength='30' placeholder='yyyy-mm-dd'"))
          .Append("<label class='sa-field'><span>Status</span><select name='bz_status'").Append(dis).Append("><option value='1'").Append(st != "0" && st != "2" ? " selected" : "").Append(">Active</option><option value='0'").Append(st == "0" ? " selected" : "").Append(">Not Active</option>")
          .Append(st == "2" ? "<option value='2' selected>Not Active (deactivated by plan)</option>" : "").Append("</select></label></div>")
          .Append("<h2>Primary Contact</h2><div class='sa-grid3'>").Append(input("Primary Contact Name <em class='sa-req'>*</em>", "bz_cname", "PrimaryContactName", "maxlength='100' required"))
          .Append(input("Primary Contact Number <em class='sa-req'>*</em>", "bz_cnum", "PrimaryContactNumber", "type='tel' maxlength='20' required placeholder='10 digits'")).Append(input("Primary Email <em class='sa-req'>*</em>", "bz_email", "", "type='email' maxlength='100' required")).Append("</div>");
        // Address module (same Address Lookup + Address Details as Support Admin / Reseller); Edit = collapsed card + Manage Address.
        const string sub = "Search for the business address using a postal or ZIP code, select a result, or enter the address manually.", lk = "ResellerDashboard.aspx?lookup=1&postal=";
        if (add) sb.Append("<h2>Address</h2>").Append(AD.Render("bz_addr", v != null ? AD.FromForm("bz_addr") : new AD.Addr(), true, lk, sub));
        else { bool open = IsPostBack && AD.Opened("bz_addr"); var cur = a == null ? new AD.Addr() : BizAd(a); sb.Append(AD.Gate("bz_addr", cur.Any ? AD.Compose(cur) : "", open, AD.Render("bz_addr", open ? AD.FromForm("bz_addr") : cur, true, lk, sub))); }
        sb.Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='ResellerDashboard.aspx?view=businesses'>Cancel</a><button type='submit' class='sa-btn' name='rp_op' value='biz'").Append(dis).Append(">").Append(add ? "Save Business" : "Save Changes").Append("</button></div></div>");
        return sb.ToString();
    }
    // POST rp_op=biz: validate, scope-check, save through AnyminuteBusinessSave (Sp_InsertBusiness / SP_UpdateBusinessDetailsWithoutImage +
    // Sp_InsertBusinessAddress), audit, redirect. Returns "" after a redirect, or the form with errors.
    string BizSave(SqlConnection c, List<DataRow> all)
    {
        var keys = new[] { "bz_id", "bz_sub", "bz_name", "bz_oper", "bz_num", "bz_legal", "bz_reg", "bz_status", "bz_cname", "bz_cnum", "bz_email" };
        var v = keys.ToDictionary(k => k, k => SA.Form(k)); int id = SA.FormId("bz_id"); v["bz_id"] = id.ToString(CultureInfo.InvariantCulture);
        var errs = new List<string>(); DataRow own = null, subRow = null; bool add = id == 0;
        if (!rsl.Can("businesses")) errs.Add("Your reseller account does not have the Businesses permission.");
        if (!add) { own = all.FirstOrDefault(r => Convert.ToInt32(r["BusinessID"]) == id && SA.S(r["St"]) != "Deleted"); if (own == null) { errs.Add("You can only edit businesses of your own subscribers."); id = 0; v["bz_id"] = "0"; add = true; } else v["bz_subname"] = SA.S(own["Name"]).Length > 0 ? SA.S(own["Name"]) : SA.S(own["Email"]); }
        int subUser = own != null ? Convert.ToInt32(own["UserId"]) : 0;
        if (add) { int u; Int32.TryParse(v["bz_sub"], out u); subRow = RP.Subscribers(c, rsl).Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["UserId"]) == u); if (subRow == null) errs.Add("Select one of your subscribers."); else subUser = u; }
        string name = v["bz_name"], cname = v["bz_cname"], email = v["bz_email"], digits = System.Text.RegularExpressions.Regex.Replace(v["bz_cnum"], @"\D", "");
        if (name.Length == 0) errs.Add("Company name is required."); else if (name.Length > 100) errs.Add("Company name is too long (100 characters).");
        if (cname.Length == 0) errs.Add("Primary contact name is required.");
        if (digits.Length != 10) errs.Add("Enter a valid 10-digit primary contact number.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) errs.Add("Enter a valid primary email address.");
        if (name.Length > 0 && Convert.ToInt32(SA.Q(c, "SELECT COUNT(*) FROM AddBusiness WHERE CompanyName=@n AND BusinessID<>@b", "@n", name, "@b", id).Rows[0][0]) > 0) errs.Add("Business name already exists.");
        string status = v["bz_status"] == "0" ? "0" : v["bz_status"] == "2" && own != null ? "2" : "1";
        DataRow b = own != null ? BizRow(c, id) : null, a = BizAddr(c, b);
        bool opened = add || AD.Opened("bz_addr"); var ad = opened ? AD.FromForm("bz_addr") : (a == null ? new AD.Addr() : BizAd(a));
        if (opened) errs.AddRange(AD.Validate(ad, true)); else if (a == null) errs.Add("Address is required. Click Manage Address to enter it.");
        // Plan / reseller business limits on Add (Add_Business blocks the same way through Label_Maximum_Limit).
        if (add && subRow != null)
        {
            int used = Convert.ToInt32(subRow["Businesses"]), lim;
            if (Int32.TryParse(SA.S(subRow["PlanBiz"]), out lim) && lim > 0 && used >= lim) errs.Add("This subscriber has reached the plan's business limit (" + lim + ").");
            var mb = SA.Q(c, "SELECT MaxBusinessesPerSubscriber FROM AnyminuteReseller WHERE ResellerID=@r", "@r", rsl.ResellerId);
            if (mb.Rows.Count == 1 && mb.Rows[0][0] != DBNull.Value && used >= Convert.ToInt32(mb.Rows[0][0])) errs.Add("Your reseller account allows " + Convert.ToInt32(mb.Rows[0][0]) + " businesses per subscriber.");
        }
        int ownerEmp = 0, mother = 4;
        if (errs.Count == 0)
        {
            var ua = SA.Q(c, "SELECT EmployeeId,MotherOrgid FROM UserAcc WHERE UserId=@u", "@u", subUser);
            if (ua.Rows.Count == 1) { Int32.TryParse(SA.S(ua.Rows[0]["EmployeeId"]), out ownerEmp); Int32.TryParse(SA.S(ua.Rows[0]["MotherOrgid"]), out mother); }
            if (b != null) Int32.TryParse(Col(b, "MotherOrgId"), out mother);
            if (ownerEmp <= 0) errs.Add("The subscriber has no business owner record, so a business cannot be saved for it.");
        }
        if (errs.Count > 0) { Flash.Text += "<div class='sa-flash sa-warn' role='alert'>Please fix the following:<ul>" + String.Join("", errs.Select(x => "<li>" + SA.H(x) + "</li>").ToArray()) + "</ul></div>"; return BizForm(c, add ? null : own, v); }
        var details = new Dictionary<string, object> {
            {"CompanyName", name}, {"OperatingName", v["bz_oper"]}, {"BusinessNumber", v["bz_num"]}, {"LegalEntityType", Legal.Contains(v["bz_legal"]) || (b != null && v["bz_legal"] == Col(b, "LegalEntityType")) ? v["bz_legal"] : "Select"},
            {"BusinessRegistrationDate", v["bz_reg"]}, {"PrimaryContactName", cname}, {"PrimaryContactNumber", v["bz_cnum"]}, {"BusinessAgreementDate", DateTime.Now},
            {"Status", status}, {"MotherOrgId", mother}, {"CareOf", b != null ? Col(b, "CareOf") : ""} };
        Func<string, string, string> keep = (k, dflt) => a != null ? Col(a, k) : dflt;
        var address = new Dictionary<string, object> {
            {"Postalcode", AnyminuteBusinessAddressInput.Postal(ad.Postal)}, {"NumberSuffix", keep("NumberSuffix", "Please Select")}, {"StreetName", ad.Line1}, {"StreetType", ad.Line2}, {"StreetDirection", keep("StreetDirection", "")},
            {"SuitType", ad.SuiteType.Length > 0 ? ad.SuiteType : "Please Select"}, {"SuiteNumber", ad.SuiteNo}, {"SuitSuffix", keep("SuitSuffix", "Please Select")}, {"City", ad.City}, {"Province", ad.Province},
            {"WorkPhone", keep("WorkPhone", "")}, {"MainPhone", a != null && Col(a, "MainPhone").Length > 0 ? Col(a, "MainPhone") : v["bz_cnum"]}, {"Mobile", keep("Mobile", "")}, {"Fax", keep("Fax", "")},
            {"MainEmail", email}, {"CCEmail", keep("CCEmail", "")}, {"Website", keep("Website", "")}, {"Other1", keep("Other1", "")}, {"StreetNo", ad.StreetNo} };
        int saved;
        try { saved = AnyminuteBusinessSave.Save(id, details, address, ad.Country, false, "", ownerEmp); }
        catch (InvalidOperationException ex) { Flash.Text += "<div class='sa-flash sa-warn' role='alert'>" + SA.H(ex.Message) + " No changes were saved.</div>"; return BizForm(c, add ? null : own, v); }
        try { using (var tx = c.BeginTransaction()) { SA.Audit(c, tx, "Business", saved.ToString(CultureInfo.InvariantCulture), add ? "Business Added (Reseller)" : "Business Updated (Reseller)", b == null ? null : Col(b, "Status"), status, b == null ? null : Col(b, "CompanyName"), name, "Reseller " + rsl.Code + " (" + rsl.Name + ") for subscriber " + subUser); tx.Commit(); } }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("Reseller business audit skipped", ex); }
        SA.Flash(add ? "Business \"" + name + "\" added." : "Business \"" + name + "\" updated.");
        Response.Redirect("ResellerDashboard.aspx?view=businesses&biz=" + saved + "#biz", false); Context.ApplicationInstance.CompleteRequest(); return "";
    }
    // v67 (Reseller PPT s3): My Profile - View (default) / Edit (Add when the details are still empty) the reseller's own
    // AnyminuteReseller row: company (ResellerName), contact first / last (ContactPerson), Email, Phone, Alternate Phone and the Address
    // module (stored as one composed line in Address, like Support Admin > Edit Reseller). No schema change.
    void ShowMyProfile(SqlConnection c)
    {
        Crumbs.Text = "<nav class='sa-crumbs' aria-label='Breadcrumb'><a href='ResellerDashboard.aspx'>Dashboard</a> &rsaquo; <a href='ResellerDashboard.aspx?view=settings'>Settings</a> &rsaquo; <span>My Profile</span></nav>";
        Title1.Text = "My Profile"; Lead.Text = "Your reseller company, contact and address details.";
        var t = SA.Q(c, "SELECT TOP 1 * FROM AnyminuteReseller WHERE ResellerID=@r", "@r", rsl.ResellerId);
        if (t.Rows.Count == 0) { Content.Text = "<div class='sa-card'><p class='sa-muted'>Your reseller profile could not be found. Contact AnyMinute Support.</p></div>"; return; }
        var r = t.Rows[0];
        string cp = Col(r, "ContactPerson"); int sp = cp.IndexOf(' '); string first = sp > 0 ? cp.Substring(0, sp) : cp, last = sp > 0 ? cp.Substring(sp + 1).Trim() : "";
        string stored = Col(r, "Address");
        bool empty = (Col(r, "Phone") + stored).Trim().Length == 0 || cp.Length == 0;
        var v = new Dictionary<string, string> { { "pf_name", Col(r, "ResellerName") }, { "pf_first", first }, { "pf_last", last }, { "pf_email", Col(r, "Email") }, { "pf_phone", Col(r, "Phone") }, { "pf_alt", Col(r, "AltPhone") } };
        bool edit = SA.Get("edit") == "1"; List<string> errs = null;
        if (IsPostBack && SA.Form("rp_op") == "profile")
        {
            foreach (var k in v.Keys.ToList()) v[k] = SA.Form(k);
            errs = new List<string>(); edit = true;
            var ad = AD.FromForm("pf_addr"); string addr = AD.Compose(ad);
            var shown = AD.Parse(stored); if (addr == AD.Compose(shown)) addr = stored; else if (ad.Any) errs.AddRange(AD.Validate(ad, false)); else addr = "";
            if (v["pf_name"].Length == 0) errs.Add("Company name is required."); else if (v["pf_name"].Length > 150) errs.Add("Company name is too long (150 characters).");
            if (v["pf_first"].Length == 0) errs.Add("Contact first name is required.");
            string contact = (v["pf_first"] + " " + v["pf_last"]).Trim(); if (contact.Length > 150) errs.Add("Contact name is too long (150 characters).");
            if (!System.Text.RegularExpressions.Regex.IsMatch(v["pf_email"], @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || v["pf_email"].Length > 150) errs.Add("Enter a valid email address.");
            if (!System.Text.RegularExpressions.Regex.IsMatch(v["pf_phone"], @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Enter a valid phone number.");
            if (v["pf_alt"].Length > 0 && !System.Text.RegularExpressions.Regex.IsMatch(v["pf_alt"], @"^[0-9+()\-.\s]{7,30}$")) errs.Add("Alternate phone is not valid.");
            if (addr.Length > 400) errs.Add("Address is too long.");
            if (errs.Count == 0)
            {
                using (var tx = c.BeginTransaction())
                {
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteReseller SET ResellerName=@n,ContactPerson=@cp,Email=@e,Phone=@p,AltPhone=@a,Address=@ad,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@by WHERE ResellerID=@r",
                        "@n", v["pf_name"], "@cp", contact, "@e", v["pf_email"], "@p", v["pf_phone"], "@a", v["pf_alt"].Length > 0 ? (object)v["pf_alt"] : DBNull.Value, "@ad", addr.Length > 0 ? (object)addr : DBNull.Value, "@by", SA.ActorName(), "@r", rsl.ResellerId);
                    SA.Audit(c, tx, "Reseller", rsl.ResellerId.ToString(CultureInfo.InvariantCulture), "Profile Updated (Reseller)", null, null, Col(r, "ResellerName") + " | " + cp + " | " + Col(r, "Email") + " | " + Col(r, "Phone") + " | " + stored, v["pf_name"] + " | " + contact + " | " + v["pf_email"] + " | " + v["pf_phone"] + " | " + addr, "Reseller " + rsl.Code + " (My Profile)");
                    tx.Commit();
                }
                Session.Remove("AmRslPortal"); // header / welcome name refresh
                SA.Flash("Your profile was saved.");
                Response.Redirect("ResellerDashboard.aspx?view=profile", false); Context.ApplicationInstance.CompleteRequest(); return;
            }
            Flash.Text += "<div class='sa-flash sa-warn' role='alert'>Please fix the following:<ul>" + String.Join("", errs.Select(x => "<li>" + SA.H(x) + "</li>").ToArray()) + "</ul></div>";
        }
        var sb = new StringBuilder("<div class='sa-card rsl-prof'><div class='rsl-prof-head'><span class='sa-avatar'>").Append(SA.H(AnyminuteAccountantDashboard.Initials(v["pf_name"].Length > 0 ? v["pf_name"] : rsl.Name))).Append("</span><div><h2 style='margin:0'>").Append(SA.H(v["pf_name"]))
            .Append("</h2><small class='sa-muted'>Reseller ID ").Append(SA.H(rsl.Code)).Append(" &middot; Sign-in ").Append(SA.H(Col(r, "LoginEmail"))).Append("</small></div></div>");
        if (!edit)
        {
            HeadActions.Text = "<a class='sa-btn' href='ResellerDashboard.aspx?view=profile&amp;edit=1'>" + (empty ? "<b aria-hidden='true'>+</b> Add Profile Details" : "&#9998; Edit Profile") + "</a>";
            Func<string, string, string> kv = (k, x) => "<tr><th>" + k + "</th><td>" + (x.Length > 0 ? SA.H(x) : "&mdash;") + "</td></tr>";
            sb.Append("<h2>Company &amp; Contact</h2><table class='sa-kv'>").Append(kv("Company Name", v["pf_name"])).Append(kv("Contact First Name", first)).Append(kv("Contact Last Name", last)).Append(kv("Email", v["pf_email"]))
              .Append(kv("Phone", v["pf_phone"])).Append(kv("Alternate Phone", v["pf_alt"])).Append("</table><h2>Address</h2><table class='sa-kv'>");
            var pa = AD.Parse(stored);
            if (stored.Length == 0) sb.Append("<tr><td class='sa-muted'>No address yet. <a class='sa-link' href='ResellerDashboard.aspx?view=profile&amp;edit=1'>Add address</a></td></tr>");
            else sb.Append(kv("Address Line 1", (pa.StreetNo + " " + pa.Line1).Trim())).Append(kv("Address Line 2", (pa.Line2 + (pa.SuiteNo.Length > 0 ? " " + pa.SuiteType + " " + pa.SuiteNo : "")).Trim())).Append(kv("City", pa.City)).Append(kv("Province / State", pa.Province)).Append(kv("Postal / ZIP Code", pa.Postal)).Append(kv("Country", pa.Country)).Append(kv("Full Address", stored));
            sb.Append("</table></div>");
            Content.Text = sb.ToString(); return;
        }
        Func<string, string, string, string> input = (label, k, extra) => "<label class='sa-field'><span>" + label + "</span><input name='" + k + "' value='" + SA.A(v[k]) + "' " + extra + " /></label>";
        sb.Append("<h2>Company &amp; Contact</h2>").Append(input("Company Name <em class='sa-req'>*</em>", "pf_name", "maxlength='150' required")).Append("<div class='sa-grid2'>")
          .Append(input("Contact First Name <em class='sa-req'>*</em>", "pf_first", "maxlength='75' required")).Append(input("Contact Last Name", "pf_last", "maxlength='75'"))
          .Append(input("Email <em class='sa-req'>*</em>", "pf_email", "type='email' maxlength='150' required")).Append(input("Phone <em class='sa-req'>*</em>", "pf_phone", "type='tel' maxlength='30' required"))
          .Append(input("Alternate Phone", "pf_alt", "type='tel' maxlength='30'")).Append("</div><h2>Address</h2>")
          .Append(AD.Render("pf_addr", IsPostBack ? AD.FromForm("pf_addr") : AD.Parse(stored), false, "ResellerDashboard.aspx?lookup=1&postal=", "Search for your business address using a postal or ZIP code, select a result, or enter the address manually."))
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='ResellerDashboard.aspx?view=profile'>Cancel</a><button type='submit' class='sa-btn' name='rp_op' value='profile'>Save</button></div></div>");
        Content.Text = sb.ToString();
    }
    static string Money(object o) { return o == null || o == DBNull.Value ? "Custom" : "$" + Convert.ToDecimal(o).ToString("#,0.00", CultureInfo.InvariantCulture); }
    static string Num(object o) { return o == null || o == DBNull.Value ? "Custom" : SA.H(o); }
    // s9: Subscription Plans (AnyminuteSubscriptionPlan, same source as Support Admin > Subscription Plans).
    // v60 (docx t190ua0 #3, PPT s9 strict): + Add Plan, columns #, Plan Name, Description, Monthly / Annual Price (CAD), Businesses, Employees,
    // Projects, Status, Actions (View + three-dot), "Showing x to y of z plans" + per page + pager. A reseller with the "plans" permission
    // creates (and edits) its OWN plans: AnyminuteSubscriptionPlan.ResellerID = this reseller (OPTIONAL_06). AnyMinute catalogue plans
    // (ResellerID NULL) stay read-only here; other resellers' plans are never listed.
    void Plans(SqlConnection c)
    {
        Title1.Text = "Subscription Plans"; Lead.Text = "Create and manage subscription plans, limits, pricing and features.";
        if (!SA.Has(c, "AnyminuteSubscriptionPlan")) { Content.Text = "<div class='sa-card'><p class='sa-muted'>Subscription plans are not available yet. Contact AnyMinute Support.</p></div>"; return; }
        // v61: + Add Plan / Save applies OPTIONAL_06 on demand (RP.EnsureOwnPlans) so the button works without a manual DB step.
        bool own = (SA.Get("add") == "1" || (IsPostBack && SA.Form("rp_op") == "save")) && rsl.Can("plans") ? RP.EnsureOwnPlans(c) : RP.OwnPlans(c);
        HeadActions.Text = "<a class='sa-btn' href='ResellerDashboard.aspx?view=plans&amp;add=1#plan-form'><b aria-hidden='true'>+</b> Add Plan</a>";
        var t = own ? SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan WHERE ResellerID IS NULL OR ResellerID=@rsl ORDER BY PlanID", "@rsl", rsl.ResellerId) : SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan ORDER BY PlanID");
        Func<DataRow, bool> mine = r => own && r["ResellerID"] != DBNull.Value && Convert.ToInt32(r["ResellerID"]) == rsl.ResellerId;
        string form = null;
        if (IsPostBack && SA.Form("rp_op") == "save") form = PlanSave(c, t, own, mine);
        int id; Int32.TryParse(SA.Get("plan"), out id); int eid; Int32.TryParse(SA.Get("edit"), out eid);
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1; int size = SA.PageSizeArg(10);
        var all = t.Rows.Cast<DataRow>().ToList(); int pages = Math.Max(1, (all.Count + size - 1) / size); if (page > pages) page = pages;
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-table-wrap'><table class='sa-table rsl-plans'><thead><tr><th>#</th><th>Plan Name</th><th>Description</th><th>Monthly Price (CAD)</th><th>Annual Price (CAD)</th><th class='sa-c'>Businesses</th><th class='sa-c'>Employees</th><th class='sa-c'>Projects</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        int n = (page - 1) * size; DataRow pick = null, edit = null;
        foreach (DataRow r in all) { int pid = Convert.ToInt32(r["PlanID"]); if (pid == id) pick = r; if (pid == eid && mine(r)) edit = r; }
        foreach (DataRow r in all.Skip((page - 1) * size).Take(size))
        {
            bool on = r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"]); int pid = Convert.ToInt32(r["PlanID"]); string view = "ResellerDashboard.aspx?view=plans&amp;plan=" + pid + "#plan";
            sb.Append("<tr><td>").Append(++n).Append("</td><td><b>").Append(SA.H(r["PlanName"])).Append("</b>").Append(mine(r) ? " <small class='sa-muted'>(Your plan)</small>" : "").Append("</td><td>").Append(SA.H(r["Description"])).Append("</td><td>").Append(Money(r["MonthlyPrice"])).Append("</td><td>").Append(Money(r["AnnualPrice"]))
              .Append("</td><td class='sa-c'>").Append(Num(r["MaxBusinesses"])).Append("</td><td class='sa-c'>").Append(Num(r["MaxEmployees"])).Append("</td><td class='sa-c'>").Append(Num(r["MaxProjects"])).Append("</td><td>").Append(SA.Pill(on ? "Active" : "Inactive", on ? "green" : "grey"))
              .Append("</td><td class='sa-c sa-nowrap'><a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(view).Append("'>View</a> <details class='sa-menu'><summary class='sa-kebab' aria-label='Plan actions'><svg class='sa-i' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' aria-hidden='true' focusable='false'><circle cx='12' cy='5' r='1'/><circle cx='12' cy='12' r='1'/><circle cx='12' cy='19' r='1'/></svg></summary><div class='sa-menu-pop'><h4>Plan Actions</h4><small>").Append(SA.H(r["PlanName"])).Append("</small>")
              .Append("<a class='sa-menu-item' href='").Append(view).Append("'><span><b>View Plan Details</b><small>Pricing, limits and features</small></span></a>")
              .Append(mine(r) ? "<a class='sa-menu-item' href='ResellerDashboard.aspx?view=plans&amp;edit=" + pid + "#plan-form'><span><b>Edit Plan</b><small>Description, pricing, limits and status</small></span></a>" : "")
              .Append(rsl.Can("usage") ? "<a class='sa-menu-item' href='ResellerDashboard.aspx?view=usage'><span><b>Usage &amp; Limits</b><small>Subscribers against their plan limits</small></span></a>" : "").Append("</div></details></td></tr>");
        }
        if (all.Count == 0) sb.Append("<tr><td colspan='10' class='sa-empty'>No plans found.</td></tr>");
        sb.Append("</tbody></table></div>").Append(SA.Pager46(all.Count, page, size, "ResellerDashboard.aspx?view=plans", "plan")).Append("</div>");
        if (pick != null)
        {
            Func<string, string, string> kv = (k, v) => "<tr><th>" + k + "</th><td>" + (v.Length > 0 ? v : "&mdash;") + "</td></tr>";
            Func<string, string> col = k => t.Columns.Contains(k) ? SA.H(pick[k]) : "";
            sb.Append("<div class='sa-card' id='plan'><div class='sa-panel-head'><h2>").Append(SA.H(pick["PlanName"])).Append("</h2><a class='sa-btn sa-btn-outline sa-btn-sm' href='ResellerDashboard.aspx?view=plans'>Close</a></div><table class='sa-kv'>")
              .Append(kv("Description", col("Description"))).Append(kv("Monthly Price (CAD)", Money(pick["MonthlyPrice"]))).Append(kv("Annual Price (CAD)", Money(pick["AnnualPrice"]))).Append(kv("Billing Frequency", col("BillingFrequency")))
              .Append(kv("Max Businesses", Num(pick["MaxBusinesses"]))).Append(kv("Max Employees", Num(pick["MaxEmployees"]))).Append(kv("Max Projects", Num(pick["MaxProjects"]))).Append(kv("Trial Days", col("TrialDays")))
              .Append(kv("Features", col("Features").Replace("\n", "<br />"))).Append(kv("Owner", mine(pick) ? "Your reseller plan" : "AnyMinute catalogue")).Append("</table></div>");
        }
        if (form == null && (SA.Get("add") == "1" || edit != null)) form = PlanForm(own, edit, null);
        Content.Text = sb.ToString() + (form ?? "");
    }
    static string Raw(object o, string fmt) { return o == null || o == DBNull.Value ? "" : fmt == null ? Convert.ToString(o, CultureInfo.InvariantCulture) : Convert.ToDecimal(o).ToString(fmt, CultureInfo.InvariantCulture); }
    // Add / Edit Plan card (Support Admin Add Subscription Plan fields). v = posted values after a failed save.
    string PlanForm(bool own, DataRow edit, Dictionary<string, string> v)
    {
        bool add = edit == null && (v == null || v["rp_id"] == "0"); bool can = own && rsl.Can("plans"); string dis = can ? "" : " disabled";
        Func<string, string, string> val = (k, col) => v != null ? (v.ContainsKey(k) ? v[k] : "") : edit == null ? (k == "rp_trial" ? "14" : "") : Raw(edit[col], col.EndsWith("Price") ? "0.00" : null);
        Func<string, string, string, string> input = (label, k, extra) => "<label class='sa-field'><span>" + label + "</span><input name='" + k + "' value='" + SA.A(val(k, Col(k))) + "' " + extra + dis + " /></label>";
        string freq = val("rp_freq", "BillingFrequency"), act = v != null ? (v.ContainsKey("rp_active") ? v["rp_active"] : "1") : edit == null || Convert.ToBoolean(edit["IsActive"]) ? "1" : "0";
        var sb = new StringBuilder("<div class='sa-card rsl-plan-form' id='plan-form'><div class='sa-panel-head'><div><h2>").Append(add ? "Add Plan" : "Edit Plan").Append("</h2><small>").Append(add ? "Create a subscription plan for your subscribers with limits and pricing." : "Update your plan's description, pricing, limits and status.").Append("</small></div><a class='sa-x' href='ResellerDashboard.aspx?view=plans' aria-label='Close'>&times;</a></div>");
        if (!own) sb.Append("<div class='sa-flash sa-warn'>Adding reseller plans needs a one-time database update (DB_OPTIONAL/OPTIONAL_06_ResellerPlans.sql). Contact AnyMinute Support.</div>");
        else if (!rsl.Can("plans")) sb.Append("<div class='sa-flash sa-warn'>Your reseller account does not have the Subscription Plans permission. Contact AnyMinute Support to add plans.</div>");
        sb.Append("<input type='hidden' name='rp_id' value='").Append(edit != null ? Convert.ToInt32(edit["PlanID"]) : v != null ? Convert.ToInt32(v["rp_id"]) : 0).Append("' />")
          .Append(add ? input("Plan Name *", "rp_name", "maxlength='100' required") : "<label class='sa-field'><span>Plan Name</span><input value='" + SA.A(edit != null ? SA.S(edit["PlanName"]) : v["rp_name"]) + "' readonly /><input type='hidden' name='rp_name' value='" + SA.A(edit != null ? SA.S(edit["PlanName"]) : v["rp_name"]) + "' /></label>")
          .Append(input("Description", "rp_desc", "maxlength='300'"))
          .Append("<div class='sa-grid3'>").Append(input("Monthly Price (CAD)", "rp_monthly", "inputmode='decimal' placeholder='Blank = Custom'")).Append(input("Annual Price (CAD)", "rp_annual", "inputmode='decimal' placeholder='Blank = Custom'"))
          .Append("<label class='sa-field'><span>Billing Frequency</span><select name='rp_freq'").Append(dis).Append("><option").Append(freq != "Annual" ? " selected" : "").Append(">Monthly</option><option").Append(freq == "Annual" ? " selected" : "").Append(">Annual</option></select></label></div>")
          .Append("<div class='sa-grid3'>").Append(input("Max Businesses", "rp_biz", "type='number' min='0' max='100000' placeholder='Custom'")).Append(input("Max Employees", "rp_emp", "type='number' min='0' max='100000' placeholder='Custom'")).Append(input("Max Projects", "rp_proj", "type='number' min='0' max='100000' placeholder='Custom'")).Append("</div>")
          .Append("<div class='sa-grid2'>").Append(input("Trial Duration (Days)", "rp_trial", "type='number' min='0' max='365'"))
          .Append("<fieldset class='sa-field'><legend>Plan Status</legend><label class='sa-radio'><input type='radio' name='rp_active' value='1'").Append(act != "0" ? " checked" : "").Append(dis).Append(" /> Active</label> <label class='sa-radio'><input type='radio' name='rp_active' value='0'").Append(act == "0" ? " checked" : "").Append(dis).Append(" /> Inactive</label></fieldset></div>")
          .Append("<label class='sa-field'><span>Plan Features (Optional, one per line)</span><textarea name='rp_features' rows='3'").Append(dis).Append(">").Append(SA.H(val("rp_features", "Features"))).Append("</textarea></label>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='ResellerDashboard.aspx?view=plans'>Cancel</a><button type='submit' class='sa-btn' name='rp_op' value='save'").Append(dis).Append(">").Append(add ? "Save Plan" : "Save Changes").Append("</button></div></div>");
        return sb.ToString();
    }
    static string Col(string k)
    {
        switch (k) { case "rp_name": return "PlanName"; case "rp_desc": return "Description"; case "rp_monthly": return "MonthlyPrice"; case "rp_annual": return "AnnualPrice"; case "rp_freq": return "BillingFrequency";
            case "rp_biz": return "MaxBusinesses"; case "rp_emp": return "MaxEmployees"; case "rp_proj": return "MaxProjects"; case "rp_trial": return "TrialDays"; case "rp_features": return "Features"; default: return k; }
    }
    // POST rp_op=save: validate, insert (ResellerID = this reseller) or update one of this reseller's plans, audit, redirect. Returns the
    // form with errors when the save fails.
    string PlanSave(SqlConnection c, DataTable t, bool own, Func<DataRow, bool> mine)
    {
        var keys = new[] { "rp_id", "rp_name", "rp_desc", "rp_monthly", "rp_annual", "rp_freq", "rp_biz", "rp_emp", "rp_proj", "rp_trial", "rp_features", "rp_active" };
        var v = keys.ToDictionary(k => k, k => SA.Form(k)); int id = SA.FormId("rp_id"); v["rp_id"] = id.ToString(CultureInfo.InvariantCulture);
        var errs = new List<string>(); DataRow cur = null;
        if (!own) errs.Add("Adding reseller plans needs a database update (OPTIONAL_06_ResellerPlans.sql).");
        else if (!rsl.Can("plans")) errs.Add("Your reseller account does not have the Subscription Plans permission.");
        if (id > 0) { cur = t.Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["PlanID"]) == id); if (cur == null || !mine(cur)) errs.Add("You can only edit plans your reseller account created."); else v["rp_name"] = SA.S(cur["PlanName"]); }
        string name = v["rp_name"].Trim(), desc = v["rp_desc"].Trim(), feat = v["rp_features"], freq = v["rp_freq"] == "Annual" ? "Annual" : "Monthly"; bool active = v["rp_active"] != "0";
        if (name.Length == 0) errs.Add("Plan name is required."); else if (name.Length > 100) errs.Add("Plan name is too long (100 characters).");
        if (desc.Length > 300) errs.Add("Description is too long (300 characters).");
        // Plan names are matched by key across PlanHistory (SA.PlanKey), so a new name must not collide with ANY existing plan (all resellers + catalogue).
        if (id == 0 && name.Length > 0 && Convert.ToInt32(SA.Q(c, "SELECT COUNT(*) FROM AnyminuteSubscriptionPlan WHERE " + String.Format(SA.PlanKeySql, "PlanName") + "=@k", "@k", SA.PlanKey(name)).Rows[0][0]) > 0) errs.Add("A plan with this name already exists. Choose another name.");
        Func<string, string, object> dec = (k, label) => { string s = v[k].Replace("$", "").Replace(",", "").Trim(); decimal d; if (s.Length == 0) return DBNull.Value; if (!Decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out d) || d < 0 || d > 1000000) { errs.Add(label + " must be blank (Custom) or a positive amount."); return DBNull.Value; } return Math.Round(d, 2); };
        Func<string, string, object> num = (k, label) => { string s = v[k].Trim(); int i; if (s.Length == 0) return DBNull.Value; if (!Int32.TryParse(s, out i) || i < 0 || i > 100000) { errs.Add(label + " must be blank (Custom) or a whole number."); return DBNull.Value; } return i; };
        object m = dec("rp_monthly", "Monthly price"), a = dec("rp_annual", "Annual price"), b = num("rp_biz", "Max Businesses"), e = num("rp_emp", "Max Employees"), p = num("rp_proj", "Max Projects");
        int trial; if (!Int32.TryParse(v["rp_trial"].Trim(), out trial) || trial < 0 || trial > 365) errs.Add("Trial duration must be 0-365 days.");
        if (errs.Count > 0) { Flash.Text += "<div class='sa-flash sa-warn' role='alert'>Please fix the following:<ul>" + String.Join("", errs.Select(x => "<li>" + SA.H(x) + "</li>").ToArray()) + "</ul></div>"; return PlanForm(own, null, v); }
        using (var tx = c.BeginTransaction())
        {
            var args = new object[] { "@n", name, "@d", desc, "@m", m, "@a", a, "@b", b, "@e", e, "@p", p, "@t", trial, "@f", freq, "@ft", feat, "@ac", active, "@u", SA.ActorId(), "@r", rsl.ResellerId, "@i", id };
            if (id == 0) id = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, @"INSERT AnyminuteSubscriptionPlan(PlanName,Description,MonthlyPrice,AnnualPrice,MaxBusinesses,MaxEmployees,MaxProjects,TrialDays,BillingFrequency,Features,IsActive,UpdatedBy,ResellerID)
                VALUES(@n,@d,@m,@a,@b,@e,@p,@t,@f,@ft,@ac,@u,@r); SELECT CAST(SCOPE_IDENTITY() AS int)", args).Rows[0][0]);
            else AnyminuteTimesheetRepair.Execute(c, tx, @"UPDATE AnyminuteSubscriptionPlan SET Description=@d,MonthlyPrice=@m,AnnualPrice=@a,MaxBusinesses=@b,MaxEmployees=@e,MaxProjects=@p,TrialDays=@t,BillingFrequency=@f,Features=@ft,IsActive=@ac,UpdatedBy=@u,UpdatedAtUTC=SYSUTCDATETIME() WHERE PlanID=@i AND ResellerID=@r", args);
            string summary = "Monthly " + Money(m) + "; Annual " + Money(a) + "; Businesses " + Num(b) + "; Employees " + Num(e) + "; Projects " + Num(p) + "; Trial " + trial + " days; " + (active ? "Active" : "Inactive");
            SA.Audit(c, tx, "Plan", id.ToString(CultureInfo.InvariantCulture), cur == null ? "Plan Added (Reseller)" : "Plan Updated (Reseller)", cur == null ? null : (Convert.ToBoolean(cur["IsActive"]) ? "Active" : "Inactive"), active ? "Active" : "Inactive", cur == null ? null : SA.S(cur["PlanName"]), name + ": " + summary, "Reseller " + rsl.Code + " (" + rsl.Name + ")");
            tx.Commit();
        }
        SA.Flash((cur == null ? "Plan \"" + name + "\" added." : "Plan \"" + name + "\" updated.") + " It is available to your subscribers in Add / Edit Subscriber.");
        Response.Redirect("ResellerDashboard.aspx?view=plans&plan=" + id + "#plan", false); Context.ApplicationInstance.CompleteRequest(); return "";
    }
    // Usage & Limits (read-only): each subscriber's Businesses / Employees / Projects used against the active plan limits.
    void Usage(SqlConnection c)
    {
        Crumbs.Text = Crumb("Usage &amp; Limits"); Title1.Text = "Usage &amp; Limits"; Lead.Text = "Businesses, employees and projects used by your subscribers against their plan limits.";
        var t = RP.Subscribers(c, rsl).Rows.Cast<DataRow>().ToList();
        var proj = new Dictionary<string, string>();
        if (SA.Has(c, "AnyminuteSubscriptionPlan")) foreach (DataRow p in SA.Q(c, "SELECT PlanName,MaxProjects FROM AnyminuteSubscriptionPlan").Rows) proj[SA.PlanKey(SA.S(p["PlanName"]))] = SA.S(p["MaxProjects"]);
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>#</th><th>Subscriber</th><th>Plan</th><th class='sa-c'>Businesses</th><th class='sa-c'>Employees</th><th class='sa-c'>Projects</th><th>Status</th></tr></thead><tbody>");
        int n = 0;
        foreach (var r in t)
        {
            string plan = SA.S(r["PlanName"]), pl; proj.TryGetValue(SA.PlanKey(plan), out pl); string st = RP.StatusOf(r);
            string name = SA.S(r["Name"]).Length > 0 ? SA.H(r["Name"]) : SA.H(r["Email"]);
            sb.Append("<tr><td>").Append(++n).Append("</td><td>").Append(rsl.Can("subscribers") ? "<a class='sa-link' href='SupportSubscribers.aspx?user=" + Convert.ToInt32(r["UserId"]) + "&amp;view=billing'>" + name + "</a>" : name)
              .Append("</td><td>").Append(plan.Length > 0 ? SA.H(plan) : "&mdash;").Append("</td><td class='sa-c'>").Append(Used(Convert.ToInt32(r["Businesses"]), r["PlanBiz"])).Append("</td><td class='sa-c'>").Append(Used(Convert.ToInt32(r["Employees"]), r["PlanUsers"]))
              .Append("</td><td class='sa-c'>").Append(Used(Convert.ToInt32(r["Projects"]), pl ?? "")).Append("</td><td>").Append(SA.Pill(st, st == "Active" ? "green" : st == "Pending" ? "amber" : st == "Suspended" ? "red" : "grey")).Append("</td></tr>");
        }
        if (n == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No subscribers yet.</td></tr>");
        Content.Text = sb.Append("</tbody></table></div></div>").ToString();
    }
    // Settings: reseller account summary (read-only; Support Admin manages it) + profile / security links.
    void Settings()
    {
        Crumbs.Text = Crumb("Settings"); Title1.Text = "Settings"; Lead.Text = "Your reseller account, profile and sign-in security.";
        Func<string, string, string> kv = (k, v) => "<tr><th>" + k + "</th><td>" + (v.Length > 0 ? v : "&mdash;") + "</td></tr>";
        string perms = AnyminuteResellers.PermissionText(String.Join(",", rsl.Perms.ToArray()));
        Content.Text = "<div class='sa-card'><h2>Reseller Account</h2><table class='sa-kv'>" + kv("Reseller", SA.H(rsl.Name)) + kv("Reseller ID", SA.H(rsl.Code)) + kv("Permissions", SA.H(perms))
            + kv("Subscriber Limit", rsl.MaxSubscribers.HasValue ? rsl.MaxSubscribers.Value.ToString(CultureInfo.InvariantCulture) : "Unlimited") + "</table><p class='sa-muted'>Contact AnyMinute Support to change your reseller details or permissions.</p></div>"
            + "<div class='rsl-set'><a href='ResellerDashboard.aspx?view=profile'><b>My Profile</b><span class='sa-muted'>Company, contact details and address</span></a><a href='LoginSecurity.aspx'><b>Login &amp; Security</b><span class='sa-muted'>Sign-in email, password and two-step verification</span></a>"
            + "<a href='Change%20Password.aspx'><b>Change Password</b><span class='sa-muted'>Set a new sign-in password</span></a></div>";
    }
}
