using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
// 2026-10-24 v10: Rate History Module (deck H). One employee + business: edit the current rate, add a new rate,
// read-only history. Uses the same rate services as the Employee editor (RateDetails + audit), owner-only.
public partial class AnyminuteRateHistoryPage : System.Web.UI.Page
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private int employee; private AnyminuteEmployeeCardView card;
    private bool AddingRate { get { return ViewState["AddingRate"] is bool && (bool)ViewState["AddingRate"]; } set { ViewState["AddingRate"] = value; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); AnyminuteFormAccess.OwnerSomewhere(); ViewStateUserKey = Session.SessionID;
        string id = Request.QueryString["EmployeeId"] ?? Request.QueryString["EmployeeIDEdit"];
        if (id == null || !Int32.TryParse(id, out employee) || employee <= 0) throw new HttpException(400, "Select an employee.");
        card = AnyminuteEmployeeEditor.RequireEditable(employee);
        foreach (string p in AnyminuteEmployeeEditor.PayTypes) RateType.Items.Add(p);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string flash = AnyminuteFlash.Take("rate"); if (flash != null) { FlashText.Text = HttpUtility.HtmlEncode(flash); Flash.Visible = true; }
        Initials.Text = HttpUtility.HtmlEncode(card.Initials); EmployeeName.Text = HttpUtility.HtmlEncode(card.Name);
        EmployeeEmail.Text = HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(card.Email) ? "\u2014" : card.Email);
        EmployeePhone.Text = HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(card.Phone) ? "\u2014" : card.Phone);
        BackLink.NavigateUrl = "~/EmployeeEditor.aspx?EmployeeId=" + employee + "&tab=assignments"; BackLink.Text = "Back to Employee";
        if (IsPostBack) return;
        foreach (var a in card.Assignments.GroupBy(x => x.BusinessId).Select(g => g.First()))
        {
            RateBusiness.Items.Add(new ListItem(a.Business, a.BusinessId.ToString(Inv)));
            HeadBusiness.Items.Add(new ListItem(a.Business, a.BusinessId.ToString(Inv)));
        }
        string q = Request.QueryString["BusinessId"] ?? Request.QueryString["BussEditId"];
        if (q != null && RateBusiness.Items.FindByValue(q) != null) RateBusiness.SelectedValue = HeadBusiness.SelectedValue = q;
        BindRate(true);
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        FormTitle.Text = AddingRate ? "Add New Rate" : "Current Pay Rate";
        FormHint.Text = AddingRate ? "Create a new rate for a future change. The current rate will end the day before the new effective date." : "Update the employee's current pay rate. Changes will take effect from the selected effective date.";
        RateLabel.Text = AddingRate ? "New Pay Rate" : "Current Pay Rate"; SaveText.Text = AddingRate ? "Add New Rate" : "Update Current Rate";
        int b; var a = Int32.TryParse(RateBusiness.SelectedValue, out b) ? card.Assignments.FirstOrDefault(x => x.BusinessId == b) : null;
        EmployeeRole.Text = HttpUtility.HtmlEncode(a == null ? "" : a.Role);
        string status = a == null || String.IsNullOrWhiteSpace(a.Status) ? card.Status : a.Status;
        StatusBadge.Text = HttpUtility.HtmlEncode(status); StatusBadge.CssClass = "am-rm-badge " + (status == "Active" ? "am-rm-on" : "am-rm-off");
    }
    private void BindRate(bool fill)
    {
        int b; if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) { NoRates.Visible = true; return; }
        var t = AnyminuteEmployeeEditor.RateHistory(AnyminuteEmployeeEditor.RecordFor(card, b));
        t.Columns.Add("StartIso"); t.Columns.Add("EndIso");
        foreach (DataRow r in t.Rows)
        {
            r["StartIso"] = r["effectivedate"] == DBNull.Value ? "\u2014" : Convert.ToDateTime(r["effectivedate"]).ToString("yyyy-MM-dd", Inv);
            bool hasEnd = Convert.ToString(r["Is_Having_EndDate"]) == "True" && r["EndDate"] != DBNull.Value && !(bool)r["Current"];
            r["EndIso"] = hasEnd ? Convert.ToDateTime(r["EndDate"]).ToString("yyyy-MM-dd", Inv) : "\u2014";
        }
        RateRows.DataSource = t; RateRows.DataBind(); NoRates.Visible = t.Rows.Count == 0;
        if (!fill) return;
        var current = t.Rows.Cast<DataRow>().FirstOrDefault(r => (bool)r["Current"]);
        if (AddingRate || current == null)
        {
            RateValue.Text = ""; RateEffective.Text = DateTime.Today.AddDays(current == null ? 0 : 1).ToString("yyyy-MM-dd", Inv);
            if (current != null) Select(RateType, Convert.ToString(current["RateType"]));
        }
        else
        {
            Select(RateType, Convert.ToString(current["RateType"]));
            RateValue.Text = Convert.ToDecimal(current["Rate"]).ToString("0.00", Inv);
            RateEffective.Text = Convert.ToDateTime(current["effectivedate"]).ToString("yyyy-MM-dd", Inv);
        }
        RateReason.Text = "";
    }
    private static void Select(DropDownList list, string value) { if (!String.IsNullOrWhiteSpace(value) && list.Items.FindByValue(value) == null) list.Items.Add(value); if (value != null && list.Items.FindByValue(value) != null) list.SelectedValue = value; }
    private void SwitchBusiness(string value) { RateBusiness.SelectedValue = HeadBusiness.SelectedValue = value; AddingRate = false; BindRate(true); }
    protected void RateBusinessChanged(object sender, EventArgs e) { SwitchBusiness(RateBusiness.SelectedValue); }
    protected void HeadBusinessChanged(object sender, EventArgs e) { SwitchBusiness(HeadBusiness.SelectedValue); }
    protected void AddNewRate_Click(object sender, EventArgs e) { AddingRate = true; BindRate(true); RateValue.Focus(); }
    protected void CancelRate_Click(object sender, EventArgs e) { AddingRate = false; BindRate(true); }
    protected void RateRows_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "EditCurrent") { AddingRate = false; BindRate(true); RateValue.Focus(); } }
    protected void SaveRate_Click(object sender, EventArgs e)
    {
        try
        {
            int b; decimal rate; DateTime eff;
            if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) throw new InvalidOperationException("Select a business.");
            if (!Decimal.TryParse(RateValue.Text, NumberStyles.Number, Inv, out rate)) throw new InvalidOperationException("Enter the pay rate.");
            if (!DateTime.TryParseExact(RateEffective.Text, "yyyy-MM-dd", Inv, DateTimeStyles.None, out eff)) throw new InvalidOperationException("Enter the effective date as yyyy-mm-dd.");
            if (AddingRate) AnyminuteEmployeeEditor.ChangeRate(employee, b, RateType.SelectedValue, rate, eff, RateReason.Text);
            else AnyminuteEmployeeEditor.UpdateCurrentRate(employee, b, RateType.SelectedValue, rate, eff, RateReason.Text);
            AnyminuteFlash.Set("rate", AddingRate ? "New pay rate added successfully." : "Current pay rate updated successfully.");
            Response.Redirect("~/RateHistory.aspx?EmployeeId=" + employee + "&BusinessId=" + b, false); Context.ApplicationInstance.CompleteRequest();
        }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; BindRate(false); }
    }
}
