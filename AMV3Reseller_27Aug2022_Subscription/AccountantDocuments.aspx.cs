using System;
using System.Linq;
using System.Web;

// 2026-10-31 v37 (PPT s1: left navigation "Documents" + Quick Action "Download Documents"). Accountant / payroll-agency
// Documents page: client payroll documents for each assigned business (the reports the business shares with the accountant,
// as PDF or Excel for a chosen period). Downloads are logged (optional log table) and served by Reports.aspx, which re-checks
// the Report Access permission for every file.
public partial class AnyminuteAccountantDocuments : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "Documents";
        if (!AnyminuteAccountantDashboard.Applies()) { Response.Redirect("Reports.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
        string b = Request.QueryString["b"], period = Request.QueryString["p"];
        if (Request.QueryString["dl"] == "1")
        {
            string key = (Request.QueryString["r"] ?? "").Trim().ToLowerInvariant(), export = (Request.QueryString["export"] ?? "").ToLowerInvariant();
            int business; var def = AnyminuteReports.Def(key);
            if (def != null && Int32.TryParse(b, out business) && (export == "pdf" || export == "excel"))
            {
                string from = Request.QueryString["from"], to = Request.QueryString["to"];
                AnyminuteAccountantDashboard.Log(business, "Document downloaded", def.Name + (String.IsNullOrEmpty(from) ? "" : " \u2013 " + from + " to " + to) + " (" + (export == "pdf" ? "PDF" : "Excel") + ")");
                Response.Redirect("Reports.aspx?r=" + Uri.EscapeDataString(key) + "&b=" + business + (String.IsNullOrEmpty(from) ? "" : "&from=" + Uri.EscapeDataString(from) + "&to=" + Uri.EscapeDataString(to ?? "")) + "&export=" + export + "&src=docs", false);
                Context.ApplicationInstance.CompleteRequest(); return;
            }
            PageError.Visible = true; PageError.Text = "<p class='am-flash am-ap-error' role='alert'>This document link is not valid. Choose a document below.</p>";
        }
        try { DocumentsHtml.Text = AnyminuteAccountantDashboard.RenderDocuments(String.IsNullOrEmpty(b) ? null : b, period); }
        catch (System.Threading.ThreadAbortException) { throw; }
        catch (Exception ex)
        {
            log4net.LogManager.GetLogger("Log").Error("AccountantDocuments failed", ex);
            PageError.Visible = true; PageError.Text = "<p class='am-flash am-ap-error' role='alert'>Documents could not be loaded right now. Please try again.</p>";
        }
    }
}
