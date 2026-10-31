using System;
using System.Web;

// 2026-10-26 v19 (slides 1-5): Reports hub. Select Report -> Filters -> Preview -> Export PDF | Export Excel.
// Everything is a GET (bookmarkable, no ViewState); AnyminuteReports checks the role permission for every action.
public partial class AnyminuteReportsPage : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        var q = AmRptQuery.Parse(Request.QueryString);
        if (q.Action != "view")
        {
            // v37 Recent Activity (accountants only, optional table); Documents downloads are logged by AccountantDocuments.aspx.
            if (Request.QueryString["src"] != "docs") { int lb; Int32.TryParse(q.Business, out lb); var ld = AnyminuteReports.Def(q.Key); if (ld != null) AnyminuteAccountantDashboard.Log(lb, (q.Key.StartsWith("payroll") ? "Payroll report" : "Report") + " downloaded", ld.Name + " (" + (q.Action == "pdf" ? "PDF" : "Excel") + ")"); }
            try { AnyminuteReports.Export(Context, q); return; }
            catch (HttpException ex) { PageError.Visible = true; PageError.Text = HttpUtility.HtmlEncode(ex.Message); q.Action = "view"; }
        }
        HubHtml.Text = AnyminuteReports.Hub(q);
    }
}
