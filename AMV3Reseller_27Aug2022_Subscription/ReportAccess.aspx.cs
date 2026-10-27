using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

// 2026-10-26 v19 (slide 6): User Access > Report Access. Owners set the role matrix (View / PDF / Excel x Employee /
// Supervisor / Administrator) for the 10 reports, globally or per business. Stored in dbo.AnyminuteReportAccess (REQUIRED_06).
public partial class AnyminuteReportAccessPage : System.Web.UI.Page
{
    int me; bool installed;
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); ViewStateUserKey = Session.SessionID; AnyminuteFormAccess.OwnerSomewhere(); me = Convert.ToInt32(Session["EmployeeId"]); }
    List<AmRptBiz> Owned() { return AnyminuteReports.Scope().Where(b => AnyminuteModernScope.Owns(b.Id)).ToList(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        installed = AnyminuteReports.Installed();
        if (!IsPostBack)
        {
            string f = AnyminuteFlash.Take("reportaccess"); if (!String.IsNullOrEmpty(f)) { Flash.Visible = true; FlashText.Text = HttpUtility.HtmlEncode(f); }
            foreach (var b in Owned()) OverrideBusiness.Items.Add(new ListItem(b.Name, b.Id.ToString()));
            int pick; if (Int32.TryParse(Request.QueryString["BusinessId"], out pick) && OverrideBusiness.Items.FindByValue(pick.ToString()) != null) OverrideBusiness.SelectedValue = pick.ToString();
        }
        UseGlobal.InputAttributes["data-ra-toggle"] = "OverridePanel";
        Bind(!IsPostBack);
    }
    int Business() { int b; return Int32.TryParse(OverrideBusiness.SelectedValue, out b) ? b : 0; }
    void Bind(bool resetToggle)
    {
        ModuleNote.Visible = !installed; SaveDefaults.Enabled = SaveOverride.Enabled = installed;
        bool o; Dictionary<string, AmRptPerm[]> global, biz;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            global = AnyminuteReports.Matrix(c, me, 0, out o);
            int b = Business(); biz = b > 0 ? AnyminuteReports.Matrix(c, AnyminuteReports.OwnerOf(c, b), b, out o) : global;
            if (b == 0) o = false;
        }
        bool custom = installed && !AnyminuteReports.Defs.All(d => Enumerable.Range(0, 3).All(i => Same(global[d.Key][i], AnyminuteReports.Defaults()[d.Key][i])));
        DefaultChip.Text = custom ? "Custom Defaults Active" : "Default Active";
        DefaultMatrix.Text = AnyminuteReports.MatrixHtml(global, !installed, "m_");
        if (resetToggle) UseGlobal.Checked = !o;
        OverrideChip.Text = o ? "Override Active" : "Using Global Defaults"; OverrideChip.CssClass = o ? "am-rp-pill is-warn" : "am-rp-pill is-ok";
        OverrideMatrix.Text = AnyminuteReports.MatrixHtml(biz, !installed || UseGlobal.Checked, "o_");
        OverridePanel.Attributes["class"] = "am-ra-override" + (UseGlobal.Checked ? " is-disabled" : "");
        SaveOverride.Visible = OverrideBusiness.Items.Count > 0;
    }
    static bool Same(AmRptPerm a, AmRptPerm b) { return a.View == b.View && a.Pdf == b.Pdf && a.Excel == b.Excel; }
    protected void OverrideBusiness_Changed(object sender, EventArgs e) { Bind(true); }
    void Done(string m) { AnyminuteReports.Forget(); AnyminuteFlash.Set("reportaccess", m); Response.Redirect("~/ReportAccess.aspx" + (Business() > 0 ? "?BusinessId=" + Business() : ""), false); Context.ApplicationInstance.CompleteRequest(); }
    void Fail(Exception ex) { PageError.Visible = true; PageError.Text = HttpUtility.HtmlEncode(ex.Message); Bind(false); }
    protected void SaveDefaults_Click(object sender, EventArgs e)
    {
        try
        {
            var m = AnyminuteReports.ReadMatrix(Request.Form, "m_");
            var owners = new HashSet<int> { me };
            using (var c = AnyminuteTimesheetRepair.Open()) foreach (var b in Owned()) { int o = AnyminuteReports.OwnerOf(c, b.Id); if (o > 0) owners.Add(o); }
            foreach (int o in owners) AnyminuteReports.Save(o, 0, m);
            Done("Default report access saved. It applies to every business without an override.");
        }
        catch (InvalidOperationException ex) { Fail(ex); } catch (HttpException ex) { Fail(ex); }
    }
    protected void SaveOverride_Click(object sender, EventArgs e)
    {
        try
        {
            int b = Business(); if (b <= 0 || !AnyminuteModernScope.Owns(b)) throw new InvalidOperationException("Select a business you own.");
            int owner; using (var c = AnyminuteTimesheetRepair.Open()) owner = AnyminuteReports.OwnerOf(c, b);
            if (UseGlobal.Checked) { AnyminuteReports.ClearOverride(owner, b); Done(OverrideBusiness.SelectedItem.Text + " now uses the global defaults."); }
            else { AnyminuteReports.Save(owner, b, AnyminuteReports.ReadMatrix(Request.Form, "o_")); Done("Report access override saved for " + OverrideBusiness.SelectedItem.Text + "."); }
        }
        catch (InvalidOperationException ex) { Fail(ex); } catch (HttpException ex) { Fail(ex); }
    }
}
