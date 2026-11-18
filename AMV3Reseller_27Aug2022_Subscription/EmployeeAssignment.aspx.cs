using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
// 2026-10-24 v2: "Edit Business Assignment" page with Add/Edit Business Assignment dialog (new ppt slides 4-6).
public partial class AnyminuteEmployeeAssignmentPage : System.Web.UI.Page
{
    private int employee; private AnyminuteEmployeeCardView card;
    private int EditingBusiness { get { return Convert.ToInt32(ViewState["EditingBusiness"] ?? 0); } set { ViewState["EditingBusiness"] = value; } }
    // v88/v89: the Assign Roles option named "Administrator" (standard User_TypeId 5) is labelled "Supervisor" (display only, trimmed). Accountant (12) stays hidden as in v86.
    // UI only - a hidden role the assignment already holds is kept, in its original position, when the form is saved.
    static readonly int[] HiddenRoles = { 12 };
    private int[] OriginalRoles { get { return (int[])ViewState["OriginalRoles"] ?? new int[0]; } set { ViewState["OriginalRoles"] = value; } }
    private int[] KeptRoles { get { return (int[])ViewState["KeptRoles"] ?? new int[0]; } set { ViewState["KeptRoles"] = value; } }
    private static string StatusPill(object status) { return AnyminuteEmployeeEditor.StatusPill(status); }
    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); AnyminuteFormAccess.OwnerSomewhere(); ViewStateUserKey = Session.SessionID;
        if (!Int32.TryParse(Request.QueryString["EmployeeId"], out employee) || employee <= 0) throw new HttpException(400, "Select an employee.");
        card = AnyminuteEmployeeEditor.RequireEditable(employee);
        CloseLink.NavigateUrl = "~/EmployeeDetails.aspx?EmployeeId=" + employee + "&tab=assignments";
        foreach (string t in AnyminuteEmployeeEditor.PayTypes) PayType.Items.Add(t);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string flash = AnyminuteFlash.Take("assignment"); if (flash != null) { FlashText.Text = HttpUtility.HtmlEncode(flash); Flash.Visible = true; }
        if (IsPostBack) return;
        BindPage();
        int b; string selected = Request.QueryString["BusinessId"];
        if (selected != null) { if (!Int32.TryParse(selected, out b) || !card.Assignments.Any(a => a.BusinessId == b)) throw new HttpException(403, "Business outside this employee's assignments."); OpenEditor(b); ViewState["Direct"] = true; }
        else if (Request.QueryString["mode"] == "add") { OpenEditor(0); ViewState["Direct"] = true; }
    }
    private void BindPage()
    {
        Person.Text = EditorName.Text = HttpUtility.HtmlEncode(card.Name); PersonInitials.Text = EditorInitials.Text = HttpUtility.HtmlEncode(card.Initials);
        PersonEmail.Text = EditorEmail.Text = HttpUtility.HtmlEncode(card.Email); PersonPhone.Text = EditorPhone.Text = HttpUtility.HtmlEncode(card.Phone);
        PersonStatus.Text = HttpUtility.HtmlEncode(card.Status); PersonStatus.CssClass = StatusPill(card.Status);
        var rows = AnyminuteEmployeeEditor.Assignments(card); Existing.DataSource = rows; Existing.DataBind(); AssignmentCount.Text = rows.Count.ToString(CultureInfo.InvariantCulture);
        if (!AnyminuteEmployeeEditor.ModuleInstalled()) { SetupNote.Text = "Extra roles, billing rate, end date, default project and notes are saved after the database update DB_REQUIRED/REQUIRED_01_EmployeeModule.sql is installed. Main role, type, pay and status save now."; SetupNote.Visible = true; }
    }
    private void OpenEditor(int business)
    {
        EditingBusiness = business; EditorError.Visible = false; Editor.Visible = true;
        EditorTitle.Text = business == 0 ? "Add Business Assignment" : "Edit Business Assignment";
        EditorSubtitle.Text = business == 0 ? "Assign this employee to a business and set roles and pay." : "Update roles, pay and status for this business.";
        SaveText.Text = business == 0 ? "+ Add Assignment" : "Save Changes";
        Business.Items.Clear(); Business.Items.Add(new ListItem("Select a business", ""));
        foreach (DataRow b in AnyminuteModernScope.Businesses().Rows)
        {
            int id = Convert.ToInt32(b["BusinessID"]); if (!AnyminuteModernScope.Owns(id)) continue;
            bool assigned = card.Assignments.Any(a => a.BusinessId == id);
            if (business == 0 && assigned) continue; if (business != 0 && id != business) continue;
            Business.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]), id.ToString(CultureInfo.InvariantCulture)));
        }
        Business.Enabled = business == 0;
        var row = business == 0 ? null : AnyminuteEmployeeEditor.Assignments(card).First(a => a.BusinessId == business);
        if (row != null) Business.SelectedValue = business.ToString(CultureInfo.InvariantCulture);
        BindChoices(row == null ? new int[0] : row.RoleIds, row == null ? 0 : row.ProjectId);
        EmployeeType.SelectedValue = row != null && row.EmployeeType == "Project" ? "Project" : "Regular";
        PayType.SelectedValue = row != null && PayType.Items.FindByValue(row.PayType ?? "") != null ? row.PayType : "Hourly";
        PayRate.Text = row != null && row.PayRate.HasValue ? row.PayRate.Value.ToString("0.00", CultureInfo.InvariantCulture) : "";
        BillingRate.Text = row != null && row.BillingRate.HasValue ? row.BillingRate.Value.ToString("0.00", CultureInfo.InvariantCulture) : "";
        EffectiveDate.Text = (row != null && row.EffectiveDate.HasValue ? row.EffectiveDate.Value : DateTime.Today).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        EndDate.Text = row != null && row.EndDate.HasValue ? row.EndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
        AssignmentStatus.SelectedValue = row != null && AnyminuteEmployeeEditor.StatusValues.Contains(row.Status) ? row.Status : "Active";
        Notes.Text = row != null ? row.Notes : "";
    }
    private void BindChoices(int[] selectedRoles, int project)
    {
        Roles.Items.Clear(); DefaultProject.Items.Clear(); KeptRoles = selectedRoles.Where(x => HiddenRoles.Contains(x)).ToArray(); OriginalRoles = selectedRoles; DefaultProject.Items.Add(new ListItem("Select project", "0"));
        int b; if (!Int32.TryParse(Business.SelectedValue, out b)) return;
        using (var c = AnyminuteTimesheetRepair.Open()) AnyminuteApprovalAssignments.RequireOwner(c, null, b);
        foreach (DataRow r in AnyminuteEmployeeEditor.Roles(b).Rows) { if (HiddenRoles.Contains(Convert.ToInt32(r["User_TypeId"]))) continue; var item = new ListItem(AnyminuteEmployeeEditor.RoleDisplay(r["UserType"]), Convert.ToString(r["User_TypeId"])); item.Selected = selectedRoles.Contains(Convert.ToInt32(r["User_TypeId"])); Roles.Items.Add(item); }
        foreach (DataRow p in AnyminuteEmployeeEditor.Projects(b).Rows) DefaultProject.Items.Add(new ListItem(Convert.ToString(p["ProjectName"]), Convert.ToString(p["ProjectID"])));
        if (DefaultProject.Items.FindByValue(project.ToString(CultureInfo.InvariantCulture)) != null) DefaultProject.SelectedValue = project.ToString(CultureInfo.InvariantCulture);
    }
    protected void BusinessChanged(object sender, EventArgs e) { BindChoices(new int[0], 0); }
    protected void Add_Click(object sender, EventArgs e) { OpenEditor(0); }
    // v86: full-page editor. Back/Cancel returns to where it was opened from (Employee Details when opened directly, else the list).
    protected void Cancel_Click(object sender, EventArgs e)
    {
        Editor.Visible = false; EditingBusiness = 0;
        if (ViewState["Direct"] != null) { Response.Redirect("EmployeeDetails.aspx?EmployeeId=" + employee + "&tab=assignments", false); Context.ApplicationInstance.CompleteRequest(); }
    }
    protected void Page_PreRender(object sender, EventArgs e) { AssignList.Visible = !Editor.Visible; }
    protected void Existing_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int b; if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out b)) throw new HttpException(400, "Invalid business.");
        if (e.CommandName == "EditRow") { OpenEditor(b); return; }
        if (e.CommandName != "DeleteRow") return;
        try { AnyminuteEmployeeEditor.RemoveAssignment(employee, b); AnyminuteFlash.Set("assignment", "Business assignment removed."); Reload(); }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; }
    }
    private void Reload()
    {
        // The removed record may have been the one the URL points at; fall back to the employee list.
        bool stillExists = true; try { AnyminuteEmployeesView.RequireEmployee(employee); } catch (HttpException) { stillExists = false; }
        Response.Redirect(stillExists ? "EmployeeAssignment.aspx?EmployeeId=" + employee : "Users.aspx", false); Context.ApplicationInstance.CompleteRequest();
    }
    protected void Save_Click(object sender, EventArgs e)
    {
        var input = new AnyminuteAssignmentInput(); int b; decimal rate, billing; DateTime eff, end;
        try
        {
            if (!Int32.TryParse(Business.SelectedValue, out b)) throw new InvalidOperationException("Select a business.");
            if (EditingBusiness != 0 && b != EditingBusiness) throw new HttpException(400, "Business changed unexpectedly.");
            input.BusinessId = b;
            var ticked = Roles.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => Int32.Parse(i.Value, CultureInfo.InvariantCulture)).ToList();
            var kept = KeptRoles; var original = OriginalRoles;
            // A kept (no longer offered) role that was the main role stays first; other kept roles follow the ticked ones.
            if (kept.Length > 0 && original.Length > 0 && kept.Contains(original[0])) input.Roles = new[] { original[0] }.Concat(ticked).Concat(kept.Where(x => x != original[0])).ToArray();
            else input.Roles = ticked.Concat(kept).ToArray();
            input.EmployeeType = EmployeeType.SelectedValue; input.PayType = PayType.SelectedValue; input.Status = AssignmentStatus.SelectedValue;
            if (!Decimal.TryParse(PayRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out rate)) throw new InvalidOperationException("Enter the pay rate.");
            input.PayRate = rate;
            if (BillingRate.Text.Trim().Length > 0) { if (!Decimal.TryParse(BillingRate.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out billing)) throw new InvalidOperationException("Enter a valid billing rate."); input.BillingRate = billing; }
            if (!DateTime.TryParse(EffectiveDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out eff)) throw new InvalidOperationException("Enter the effective date.");
            input.EffectiveDate = eff;
            if (EndDate.Text.Trim().Length > 0) { if (!DateTime.TryParse(EndDate.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out end)) throw new InvalidOperationException("Enter a valid end date."); input.EndDate = end; }
            int project; Int32.TryParse(DefaultProject.SelectedValue, out project); input.ProjectId = project; input.Notes = Notes.Text.Trim();
            AnyminuteEmployeeEditor.SaveAssignment(employee, input);
            AnyminuteFlash.Set("assignment", EditingBusiness == 0 ? "Business assignment added successfully." : "Business assignment updated successfully.");
            Response.Redirect("EmployeeAssignment.aspx?EmployeeId=" + employee, false); Context.ApplicationInstance.CompleteRequest();
        }
        catch (InvalidOperationException error) { EditorError.Text = HttpUtility.HtmlEncode(error.Message); EditorError.Visible = true; Editor.Visible = true; }
    }
}
