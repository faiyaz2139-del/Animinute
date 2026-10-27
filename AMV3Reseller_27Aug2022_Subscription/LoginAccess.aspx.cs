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
        if (!AnyminuteReports.Scope().Any(b => b.Tier == 2)) throw new HttpException(403, "Only business owners and administrators can manage login access.");
    }
    protected void Page_Load(object sender, EventArgs e)
    {
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
