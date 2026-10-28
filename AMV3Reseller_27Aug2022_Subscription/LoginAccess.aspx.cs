using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-26 v19 (t77u): User Access > Login & Access. Lists login status for every employee of the businesses the
// signed-in user owns or administers; "Manage" opens the existing EmployeeEditor Login & Access tab (unchanged).
public partial class AnyminuteLoginAccessPage : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login();
        // v27 (live: manager rs3 got the error page): Support Admin uses the subscriber console; other roles see an access-denied notice.
        if (AnyminuteSupportAdmin.IsSupportAdminOnly()) { Response.Redirect("SupportSubscribers.aspx", false); Context.ApplicationInstance.CompleteRequest(); denied = true; return; }
        try { denied = !AnyminuteReports.Scope().Any(b => b.Tier == 2); }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("LoginAccess scope check failed", ex); denied = true; }
    }
    bool denied;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (denied) { Denied.Visible = true; Allowed.Visible = false; return; }
        int business; Int32.TryParse(Request.QueryString["BusinessId"], out business);
        var sb = new StringBuilder();
        foreach (var b in AnyminuteReports.Scope().Where(x => x.Tier == 2))
            sb.Append("<option value=\"").Append(b.Id).Append("\"").Append(b.Id == business ? " selected" : "").Append(">").Append(HttpUtility.HtmlEncode(b.Name)).Append("</option>");
        BusinessOptions.Text = sb.ToString();
        var rows = AnyminuteReports.LoginRows(business);
        Func<string, string, string> k = (l, v) => "<div><span>" + l + "</span><strong>" + v + "</strong></div>";
        Kpis.Text = k("Employees", rows.Select(r => r[0]).Distinct().Count().ToString(CultureInfo.InvariantCulture)) + k("Login Enabled", rows.Count(r => r[6] == "Enabled").ToString(CultureInfo.InvariantCulture))
            + k("Invitation Pending", rows.Count(r => r[6] == "Invitation Pending").ToString(CultureInfo.InvariantCulture)) + k("Disabled / No Login", rows.Count(r => r[6] == "Disabled" || r[6] == "No Login").ToString(CultureInfo.InvariantCulture));
        Table.Text = AnyminuteReports.LoginHtml(rows);
    }
}
