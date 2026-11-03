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

// 2026-11-03 v55 (Reseller PPT collage): Reseller portal home. view = (none) Dashboard (s1 KPIs + Quick Actions) | businesses
// (businesses under the reseller's subscribers) | plans (s9 Subscription Plans catalogue, read-only) | usage (Usage & Limits,
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
        Flash.Text = SA.TakeFlash();
        string view = SA.Get("view");
        // Permission gates (nav hides these too): businesses = businesses or subscribers, plans, usage.
        if ((view == "businesses" && !rsl.Can("businesses") && !rsl.Can("subscribers")) || (view == "plans" && !rsl.Can("plans")) || (view == "usage" && !rsl.Can("usage")))
        { Flash.Text += "<div class='sa-flash sa-warn'>Your reseller account does not have access to that page.</div>"; view = ""; }
        using (var c = SA.Open())
        {
            try
            {
                if (view == "businesses") Businesses(c);
                else if (view == "plans") Plans(c);
                else if (view == "usage") Usage(c);
                else if (view == "settings") Settings();
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
        Title1.Text = "Reseller Dashboard"; Lead.Text = "Manage your subscribers, businesses and subscription plans.";
        var t = RP.Subscribers(c, rsl).Rows.Cast<DataRow>().ToList();
        int act = t.Count(r => RP.StatusOf(r) == "Active"), ina = t.Count(r => RP.StatusOf(r) == "Inactive" || RP.StatusOf(r) == "Suspended");
        var sb = new StringBuilder("<div class='rsl-kpis'>")
            .Append(Kpi("users", "blue", "Total Subscribers", t.Count)).Append(Kpi("check", "green", "Active", act)).Append(Kpi("x", "red", "Inactive", ina))
            .Append(Kpi("biz", "purple", "Total Businesses", t.Sum(r => Convert.ToInt32(r["Businesses"])))).Append(Kpi("users", "blue", "Total Employees", t.Sum(r => Convert.ToInt32(r["Employees"]))))
            .Append(Kpi("folder", "purple", "Total Projects", t.Sum(r => Convert.ToInt32(r["Projects"])))).Append("</div>");
        var qa = new List<string>();
        Func<string, string, string, string> a = (href, icon, label) => "<a href='" + href + "'>" + Ic(icon) + "<span>" + label + "</span></a>";
        if (rsl.Can("subscribers")) { qa.Add(a("SupportSubscribers.aspx?view=add", "useradd", "Add Subscriber")); qa.Add(a("SupportSubscribers.aspx", "list", "View Subscribers")); }
        if (rsl.Can("plans")) qa.Add(a("ResellerDashboard.aspx?view=plans", "doc", "Subscription Plans"));
        if (rsl.Can("usage")) qa.Add(a("ResellerDashboard.aspx?view=usage", "bars", "Usage Limits"));
        if (rsl.Can("reports")) qa.Add(a("SupportReports.aspx", "report", "Reports"));
        sb.Append("<div class='sa-card'><h2>Quick Actions</h2>").Append(qa.Count == 0 ? "<p class='sa-muted'>No actions are enabled for your reseller account. Contact AnyMinute Support to update your permissions.</p>" : "<div class='rsl-qa'>" + String.Join("", qa.ToArray()) + "</div>").Append("</div>");
        Content.Text = sb.ToString();
    }
    // Businesses nav: businesses under this reseller's subscribers (search + pager); View opens the subscriber's Businesses tab.
    void Businesses(SqlConnection c)
    {
        Crumbs.Text = Crumb("Businesses"); Title1.Text = "Businesses"; Lead.Text = "Businesses under your subscribers.";
        string q = SA.Get("q"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1; int size = SA.PageSizeArg(10);
        var all = RP.Businesses(c, rsl).Rows.Cast<DataRow>().ToList();
        var rows = all.Where(r => q.Length == 0 || (SA.S(r["CompanyName"]) + " " + SA.S(r["Name"]) + " " + SA.S(r["Email"])).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
        int pages = Math.Max(1, (rows.Count + size - 1) / size); if (page > pages) page = pages;
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-filters' data-sa-get='ResellerDashboard.aspx'><input type='hidden' name='view' value='businesses' /><input type='hidden' name='size' value='").Append(size)
            .Append("' /><label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>").Append(Ic("search")).Append("</i><input type='search' name='q' value='").Append(SA.A(q)).Append("' placeholder='Search by business or subscriber...' /></label>")
            .Append("<a class='sa-btn sa-btn-outline' href='ResellerDashboard.aspx?view=businesses'>Clear</a><button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>")
            .Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>#</th><th>Business Name</th><th>Subscriber</th><th class='sa-c'>Employees</th><th class='sa-c'>Projects</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        int n = (page - 1) * size;
        foreach (var r in rows.Skip((page - 1) * size).Take(size))
        {
            string sub = "SupportSubscribers.aspx?user=" + Convert.ToInt32(r["UserId"]), st = SA.S(r["St"]); bool open = rsl.Can("subscribers");
            sb.Append("<tr><td>").Append(++n).Append("</td><td>").Append(SA.H(r["CompanyName"])).Append("</td><td>").Append(open ? "<a class='sa-link' href='" + sub + "'>" + SA.H(SA.S(r["Name"]).Length > 0 ? r["Name"] : r["Email"]) + "</a>" : SA.H(SA.S(r["Name"]).Length > 0 ? r["Name"] : r["Email"]))
              .Append("</td><td class='sa-c'>").Append(r["Employees"]).Append("</td><td class='sa-c'>").Append(r["Projects"]).Append("</td><td>").Append(SA.Pill(st, st == "Active" ? "green" : "grey")).Append("</td><td class='sa-c'>")
              .Append(open ? "<a class='sa-btn sa-btn-sm sa-btn-outline' href='" + sub + "&amp;view=businesses'>View</a>" : "&mdash;").Append("</td></tr>");
        }
        if (rows.Count == 0) sb.Append("<tr><td colspan='7' class='sa-empty'>No businesses found.</td></tr>");
        sb.Append("</tbody></table></div>").Append(SA.Pager46(rows.Count, page, size, "ResellerDashboard.aspx?view=businesses&q=" + HttpUtility.UrlEncode(q), "business")).Append("</div>");
        Content.Text = sb.ToString();
    }
    static string Money(object o) { return o == null || o == DBNull.Value ? "Custom" : "$" + Convert.ToDecimal(o).ToString("#,0.00", CultureInfo.InvariantCulture); }
    static string Num(object o) { return o == null || o == DBNull.Value ? "Custom" : SA.H(o); }
    // s9: Subscription Plans catalogue (AnyminuteSubscriptionPlan, same source as Support Admin > Subscription Plans). Read-only:
    // the reseller permission is "View subscription plans", so + Add Plan / Edit are not offered (FIX_REPORT_v55).
    void Plans(SqlConnection c)
    {
        Title1.Text = "Subscription Plans"; Lead.Text = "Subscription plans, limits, pricing and features available to your subscribers.";
        if (!SA.Has(c, "AnyminuteSubscriptionPlan")) { Content.Text = "<div class='sa-card'><p class='sa-muted'>Subscription plans are not available yet. Contact AnyMinute Support.</p></div>"; return; }
        var t = SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan ORDER BY PlanID"); int id; Int32.TryParse(SA.Get("plan"), out id);
        var sb = new StringBuilder("<div class='sa-card'><div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>#</th><th>Plan Name</th><th>Description</th><th>Monthly Price (CAD)</th><th>Annual Price (CAD)</th><th class='sa-c'>Businesses</th><th class='sa-c'>Employees</th><th class='sa-c'>Projects</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        int n = 0; DataRow pick = null;
        foreach (DataRow r in t.Rows)
        {
            bool on = r["IsActive"] == DBNull.Value || Convert.ToBoolean(r["IsActive"]); if (Convert.ToInt32(r["PlanID"]) == id) pick = r;
            sb.Append("<tr><td>").Append(++n).Append("</td><td><b>").Append(SA.H(r["PlanName"])).Append("</b></td><td>").Append(SA.H(r["Description"])).Append("</td><td>").Append(Money(r["MonthlyPrice"])).Append("</td><td>").Append(Money(r["AnnualPrice"]))
              .Append("</td><td class='sa-c'>").Append(Num(r["MaxBusinesses"])).Append("</td><td class='sa-c'>").Append(Num(r["MaxEmployees"])).Append("</td><td class='sa-c'>").Append(Num(r["MaxProjects"])).Append("</td><td>").Append(SA.Pill(on ? "Active" : "Inactive", on ? "green" : "grey"))
              .Append("</td><td class='sa-c'><a class='sa-btn sa-btn-sm sa-btn-outline' href='ResellerDashboard.aspx?view=plans&amp;plan=").Append(r["PlanID"]).Append("#plan'>View</a></td></tr>");
        }
        if (n == 0) sb.Append("<tr><td colspan='10' class='sa-empty'>No plans found.</td></tr>");
        sb.Append("</tbody></table></div><div class='sa-pager'><span>Showing 1 to ").Append(n).Append(" of ").Append(n).Append(" plans</span></div></div>");
        if (pick != null)
        {
            Func<string, string, string> kv = (k, v) => "<tr><th>" + k + "</th><td>" + (v.Length > 0 ? v : "&mdash;") + "</td></tr>";
            Func<string, string> col = k => t.Columns.Contains(k) ? SA.H(pick[k]) : "";
            sb.Append("<div class='sa-card' id='plan'><div class='sa-panel-head'><h2>").Append(SA.H(pick["PlanName"])).Append("</h2><a class='sa-btn sa-btn-outline sa-btn-sm' href='ResellerDashboard.aspx?view=plans'>Close</a></div><table class='sa-kv'>")
              .Append(kv("Description", col("Description"))).Append(kv("Monthly Price (CAD)", Money(pick["MonthlyPrice"]))).Append(kv("Annual Price (CAD)", Money(pick["AnnualPrice"]))).Append(kv("Billing Frequency", col("BillingFrequency")))
              .Append(kv("Max Businesses", Num(pick["MaxBusinesses"]))).Append(kv("Max Employees", Num(pick["MaxEmployees"]))).Append(kv("Max Projects", Num(pick["MaxProjects"]))).Append(kv("Trial Days", col("TrialDays")))
              .Append(kv("Features", col("Features").Replace("\n", "<br />"))).Append("</table></div>");
        }
        Content.Text = sb.ToString();
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
            + "<div class='rsl-set'><a href='MyProfile.aspx'><b>My Profile</b><span class='sa-muted'>Name, contact details and photo</span></a><a href='LoginSecurity.aspx'><b>Login &amp; Security</b><span class='sa-muted'>Sign-in email, password and two-step verification</span></a>"
            + "<a href='Change%20Password.aspx'><b>Change Password</b><span class='sa-muted'>Set a new sign-in password</span></a></div>";
    }
}
