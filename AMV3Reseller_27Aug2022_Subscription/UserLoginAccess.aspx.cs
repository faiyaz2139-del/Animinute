using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-31 v38 (DOCX: subscriber "Login & Access" still -> ErrorPages/index.aspx?aspxerrorpath=/LoginAccess.aspx even with v37's
// Page_Error, i.e. the failure happened before any page code ran: page compile/type load). The page now lives here with
// unique control IDs (no "Table"/"Head"/"Body"), no static field initializer; LoginAccess.aspx is a code-free redirect.
// 2026-10-26 v19 (t77u): User Access > Login & Access. Lists login status for every employee of the businesses the
// signed-in user owns or administers; "Manage" opens the existing EmployeeEditor Login & Access tab (unchanged).
// v29 (live v28: owner ava + admin stl3 landed on ErrorPages): every step is now guarded. A failing business or
// query is logged and shown as an in-page notice; the page itself never throws to the custom error page.
public partial class AnyminuteUserLoginAccessPage : System.Web.UI.Page
{
    bool denied, done;
    // v38: no static logger field (a type-initializer failure happens before Page_Error can run).
    static log4net.ILog Log { get { return log4net.LogManager.GetLogger("Log"); } }
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
    // v37 (DOCX images 5/6: subscriber "Login & Access" -> ErrorPages/index.aspx?aspxerrorpath=/LoginAccess.aspx).
    // Any remaining failure (page or master: page-action evidence, navigation, header) is logged with its stack and the
    // user stays on a Login & Access page with a notice instead of the generic error page.
    protected void Page_Load(object sender, EventArgs e)
    {
        try { LoadPage(); }
        catch (System.Threading.ThreadAbortException) { throw; }
        catch (Exception ex)
        {
            Log.Error("LoginAccess page load failed", ex);
            DeniedView.Visible = false; AllowedView.Visible = true;
            LoginGrid.Text = "<div class=\"am-notice am-error\" role=\"alert\">Login &amp; Access could not be loaded right now. The details were logged for support. Try again, or open an employee from Employees &gt; Employee List to manage their login.</div>";
        }
    }
    protected void Page_Error(object sender, EventArgs e)
    {
        var ex = Server.GetLastError();
        if (ex is System.Threading.ThreadAbortException) return;
        Log.Error("LoginAccess unhandled error", ex);
        Server.ClearError();
        Response.Clear(); Response.StatusCode = 200; Response.ContentType = "text/html";
        Response.Write("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Login &amp; Access</title><link href=\"css/AnyminuteModern.css\" rel=\"stylesheet\" /></head><body class=\"am-modern\" style=\"padding:40px\"><div class=\"am-card\" style=\"max-width:720px;margin:0 auto\"><h1 style=\"font-size:22px;margin:0 0 10px\">Login &amp; Access</h1><div class=\"am-notice am-error\" role=\"alert\">This page could not be displayed. The details were logged for support.</div><p><a class=\"am-button\" href=\"UserLoginAccess.aspx\">Try again</a> <a class=\"am-button secondary\" href=\"Dashboard.aspx\">Back to Dashboard</a></p></div></body></html>");
        Context.ApplicationInstance.CompleteRequest();
    }
    void LoadPage()
    {
        if (done) return;
        if (denied) { DeniedView.Visible = true; AllowedView.Visible = false; return; }
        int business; Int32.TryParse(Request.QueryString["BusinessId"], out business);
        var admin = Admin();
        if (business > 0 && !admin.Any(b => b.Id == business)) business = 0; // not one of mine: show all of mine
        var sb = new StringBuilder();
        foreach (var b in admin)
            sb.Append("<option value=\"").Append(b.Id.ToString(CultureInfo.InvariantCulture)).Append("\"").Append(b.Id == business ? " selected" : "").Append(">").Append(HttpUtility.HtmlEncode(b.Name)).Append("</option>");
        BusinessOptions.Text = sb.ToString();
        List<string[]> rows; string problem;
        rows = AnyminuteReports.LoginRowsSafe(business, admin, out problem);
        Func<string, string, string> k = (l, v) => "<div class=\"am-stat\"><div><small>" + l + "</small><strong>" + v + "</strong></div></div>";
        LoginKpis.Text = k("Employees", rows.Select(r => r[0]).Distinct().Count().ToString(CultureInfo.InvariantCulture)) + k("Login Enabled", rows.Count(r => r[6] == "Enabled").ToString(CultureInfo.InvariantCulture))
            + k("Invitation Pending", rows.Count(r => r[6] == "Invitation Pending").ToString(CultureInfo.InvariantCulture)) + k("Disabled / No Login", rows.Count(r => r[6] == "Disabled" || r[6] == "No Login").ToString(CultureInfo.InvariantCulture));
        string table;
        try { table = AnyminuteReports.LoginHtml(rows); }
        catch (Exception ex) { Log.Warn("LoginAccess table render failed", ex); table = ""; problem = "The employee list could not be displayed."; }
        LoginGrid.Text = (problem == null ? "" : "<div class=\"am-notice am-error\" role=\"alert\">" + HttpUtility.HtmlEncode(problem) + " The details were logged for support. Try again, or open an employee from Users &rsaquo; Edit.</div>") + table;
    }
}
