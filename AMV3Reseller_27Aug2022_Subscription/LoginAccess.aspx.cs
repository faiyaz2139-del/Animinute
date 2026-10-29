using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-26 v19 (t77u): User Access > Login & Access. Lists login status for every employee of the businesses the
// signed-in user owns or administers; "Manage" opens the existing EmployeeEditor Login & Access tab (unchanged).
// v29 (live v28: owner ava + admin stl3 landed on ErrorPages): every step is now guarded. A failing business or
// query is logged and shown as an in-page notice; the page itself never throws to the custom error page.
public partial class AnyminuteLoginAccessPage : System.Web.UI.Page
{
    bool denied, done;
    static readonly log4net.ILog Log = log4net.LogManager.GetLogger("Log");
    protected void Page_Init(object sender, EventArgs e)
    {
        try { AnyminuteModernScope.Login(); }
        catch (HttpException) { Response.Redirect("Login.aspx", false); Context.ApplicationInstance.CompleteRequest(); done = true; return; }
        catch (Exception ex) { Log.Warn("LoginAccess sign-in check failed", ex); }
        // v27: Support Admin uses the subscriber console; other roles see an access-denied notice.
        if (AnyminuteSupportAdmin.IsSupportAdminOnly()) { Response.Redirect("SupportSubscribers.aspx", false); Context.ApplicationInstance.CompleteRequest(); done = true; return; }
        denied = Admin().Count == 0;
    }
    // Businesses where the signed-in user is owner/administrator (Tier 2). Never throws.
    static List<AmRptBiz> Admin()
    {
        try { return AnyminuteReports.Scope().Where(b => b.Tier == 2).ToList(); }
        catch (Exception ex) { Log.Warn("LoginAccess scope check failed", ex); return AnyminuteReports.AdminScopeFallback(); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (done) return;
        if (denied) { Denied.Visible = true; Allowed.Visible = false; return; }
        int business; Int32.TryParse(Request.QueryString["BusinessId"], out business);
        var admin = Admin();
        if (business > 0 && !admin.Any(b => b.Id == business)) business = 0; // not one of mine: show all of mine
        var sb = new StringBuilder();
        foreach (var b in admin)
            sb.Append("<option value=\"").Append(b.Id.ToString(CultureInfo.InvariantCulture)).Append("\"").Append(b.Id == business ? " selected" : "").Append(">").Append(HttpUtility.HtmlEncode(b.Name)).Append("</option>");
        BusinessOptions.Text = sb.ToString();
        List<string[]> rows; string problem;
        rows = AnyminuteReports.LoginRowsSafe(business, admin, out problem);
        Func<string, string, string> k = (l, v) => "<div><span>" + l + "</span><strong>" + v + "</strong></div>";
        Kpis.Text = k("Employees", rows.Select(r => r[0]).Distinct().Count().ToString(CultureInfo.InvariantCulture)) + k("Login Enabled", rows.Count(r => r[6] == "Enabled").ToString(CultureInfo.InvariantCulture))
            + k("Invitation Pending", rows.Count(r => r[6] == "Invitation Pending").ToString(CultureInfo.InvariantCulture)) + k("Disabled / No Login", rows.Count(r => r[6] == "Disabled" || r[6] == "No Login").ToString(CultureInfo.InvariantCulture));
        string table;
        try { table = AnyminuteReports.LoginHtml(rows); }
        catch (Exception ex) { Log.Warn("LoginAccess table render failed", ex); table = ""; problem = "The employee list could not be displayed."; }
        Table.Text = (problem == null ? "" : "<div class=\"am-notice am-error\" role=\"alert\">" + HttpUtility.HtmlEncode(problem) + " The details were logged for support. Try again, or open an employee from Users &rsaquo; Edit.</div>") + table;
    }
}
