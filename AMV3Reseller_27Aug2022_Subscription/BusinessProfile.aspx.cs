using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

// 2026-11-16 v86: read-only Business Profile. Access = businesses in the signed-in user's scope; Edit = owners only.
public partial class AnyminuteBusinessProfilePage : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); }
    static string H(object v) { return HttpUtility.HtmlEncode(Convert.ToString(v ?? "").Trim()); }
    static string Col(DataRow r, string c) { return r.Table.Columns.Contains(c) && r[c] != DBNull.Value ? Convert.ToString(r[c]).Trim() : ""; }
    static string Date(string v) { DateTime d; return DateTime.TryParse(v, out d) ? d.ToString("MMM d, yyyy") : v; }
    static void Field(StringBuilder s, string label, string value)
    {
        s.Append("<div class=\"am-bp-field\"><span>").Append(H(label)).Append("</span><strong>")
         .Append(String.IsNullOrEmpty(value) || value == "Select" ? "&mdash;" : H(value)).Append("</strong></div>");
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        int id; Int32.TryParse(Request.QueryString["BusinessId"], out id);
        var scope = AnyminuteModernScope.Businesses();
        if (id <= 0 || !scope.AsEnumerable().Any(r => Convert.ToInt32(r["BusinessID"]) == id))
        { ProfileLit.Text = "<div class=\"am-sd-card am-bp-empty\"><h1>Business Profile</h1><p>This business was not found or you do not have access to it.</p><a class=\"am-button secondary\" href=\"Dashboard.aspx?view=businesses\">Back to Businesses</a></div>"; return; }
        DataRow b; DataTable addr = null; string accountant = "";
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var t = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 * FROM AddBusiness WITH (NOLOCK) WHERE BusinessID=@b", "@b", id);
            if (t.Rows.Count == 0) { Response.Redirect("Dashboard.aspx?view=businesses", false); Context.ApplicationInstance.CompleteRequest(); return; }
            b = t.Rows[0];
            int a; if (Int32.TryParse(Col(b, "ADDRESSID"), out a) && a > 0)
                addr = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 * FROM BusinessAddress WITH (NOLOCK) WHERE BusinessAddressId=@a", "@a", a);
            int m; if (Int32.TryParse(Col(b, "MotherOrgId"), out m) && m > 0)
            { var mo = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 Email FROM MotherOrg WITH (NOLOCK) WHERE MotherOrgID=@m", "@m", m); if (mo.Rows.Count > 0) accountant = Col(mo.Rows[0], "Email"); }
        }
        bool owner = AnyminuteModernScope.Owns(id);
        string name = Col(b, "CompanyName"), logo = Col(b, "CareOf");
        bool active = Col(b, "Status") == "1";
        var s = new StringBuilder();
        s.Append("<header class=\"am-bp-head\"><div class=\"am-bp-title\">");
        if (logo.Length > 0 && logo.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 && File.Exists(Server.MapPath("~/Uploads/" + logo)))
            s.Append("<img class=\"am-bp-logo\" alt=\"\" src=\"").Append(H(ResolveUrl("~/Uploads/" + logo))).Append("\" />");
        else s.Append("<span class=\"am-bp-logo am-bp-initial\" aria-hidden=\"true\">").Append(H(name.Length > 0 ? name.Substring(0, 1).ToUpperInvariant() : "B")).Append("</span>");
        s.Append("<div><h1>Business Profile</h1><p>").Append(H(name)).Append(" <span class=\"am-sd-pill ").Append(active ? "am-sd-ok" : "am-sd-off").Append("\">").Append(active ? "Active" : "Inactive").Append("</span></p></div></div>");
        if (owner) s.Append("<a class=\"am-button primary\" href=\"Add_Business.aspx?BusinessIDEdit=").Append(id).Append("\">Edit</a>");
        s.Append("</header>");
        s.Append("<div class=\"am-sd-card am-bp-card\"><h2>Business Details</h2><div class=\"am-bp-grid\">");
        Field(s, "Business Name", name); Field(s, "Operating Name", Col(b, "OperatingName")); Field(s, "Business Number", Col(b, "BusinessNumber"));
        Field(s, "Legal Entity Type", Col(b, "LegalEntityType")); Field(s, "Business Registration Date", Date(Col(b, "BusinessRegistrationDate")));
        Field(s, "Business Agreement Date", Date(Col(b, "BusinessAgreementDate"))); Field(s, "Status", active ? "Active" : "Inactive");
        s.Append("</div></div>");
        s.Append("<div class=\"am-sd-card am-bp-card\"><h2>Contact</h2><div class=\"am-bp-grid\">");
        Field(s, "Primary Contact Name", Col(b, "PrimaryContactName")); Field(s, "Primary Contact Number", Col(b, "PrimaryContactNumber")); Field(s, "Accountant Email", accountant);
        s.Append("</div></div>");
        s.Append("<div class=\"am-sd-card am-bp-card\"><h2>Business Address</h2><div class=\"am-bp-grid\">");
        if (addr != null && addr.Rows.Count > 0)
        {
            var r = addr.Rows[0];
            string unit = String.Join(" ", new[] { Col(r, "SuitType"), Col(r, "SuiteNumber"), Col(r, "SuitSuffix") }.Where(x => x.Length > 0 && x != "Select"));
            string street = String.Join(" ", new[] { Col(r, "StreetNo") + Col(r, "NumberSuffix").Replace("Select", ""), Col(r, "StreetName"), Col(r, "StreetType"), Col(r, "StreetDirection") }.Where(x => x.Trim().Length > 0 && x != "Select"));
            Field(s, "Street", street); Field(s, "Unit / Suite", unit); Field(s, "City", Col(r, "City")); Field(s, "Postal Code", Col(r, "Postalcode")); Field(s, "Country", Col(r, "Country"));
        }
        else s.Append("<p class=\"am-sd-empty\">No address on file.</p>");
        s.Append("</div></div>");
        ProfileLit.Text = s.ToString();
    }
}
