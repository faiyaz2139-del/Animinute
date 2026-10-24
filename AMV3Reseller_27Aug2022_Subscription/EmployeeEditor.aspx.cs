using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
// 2026-10-24 v2: Add Employee (new ppt slide 8) and Edit Employee with tabs Employee Information, Address,
// Business Assignments, Rate History and Login & Access (slides 9-11).
public partial class AnyminuteEmployeeEditorPage : System.Web.UI.Page
{
    private int employee; private string tab = "profile"; private AnyminuteEmployeeCardView card;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private List<string[]> PendingRows { get { var l = ViewState["Pending"] as List<string[]>; if (l == null) { l = new List<string[]>(); ViewState["Pending"] = l; } return l; } }
    private string OriginalDob { get { return Convert.ToString(ViewState["Dob"]); } set { ViewState["Dob"] = value; } }
    private string OriginalSinExpiry { get { return Convert.ToString(ViewState["SinX"]); } set { ViewState["SinX"] = value; } }
    private bool Editing { get { return employee > 0; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); AnyminuteFormAccess.OwnerSomewhere(); ViewStateUserKey = Session.SessionID;
        string id = Request.QueryString["EmployeeId"];
        if (id != null && (!Int32.TryParse(id, out employee) || employee <= 0)) throw new HttpException(400, "Select an employee.");
        string t = (Request.QueryString["tab"] ?? "profile").ToLowerInvariant();
        if (new[] { "profile", "address", "assignments", "rate", "access" }.Contains(t)) tab = t;
        foreach (string p in AnyminuteEmployeeEditor.PayTypes) { NewPayType.Items.Add(p); RateType.Items.Add(p); }
        if (Editing) card = AnyminuteEmployeeEditor.RequireEditable(employee);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        string flash = AnyminuteFlash.Take("editor"); if (flash != null) ShowFlash(flash);
        if (!Editing) SetupAdd(); else SetupEdit();
    }

    // ---------- Add Employee ----------
    private void SetupAdd()
    {
        PageTitle.Text = "Add Employee"; PageSubtitle.Text = "Enter the employee details, assign businesses and set up login access.";
        NewAssignments.Visible = NewAccess.Visible = true; SaveEmployee.Text = "Save Employee";
        if (IsPostBack) return;
        HireDate.Text = NewEffective.Text = DateTime.Today.ToString("yyyy-MM-dd", Inv);
        NewBusiness.Items.Add(new ListItem("Select a business", ""));
        foreach (DataRow b in AnyminuteModernScope.Businesses().Rows) { int bid = Convert.ToInt32(b["BusinessID"]); if (AnyminuteModernScope.Owns(bid)) NewBusiness.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]), bid.ToString(Inv))); }
        if (NewBusiness.Items.Count == 2) NewBusiness.SelectedIndex = 1;
        BindNewChoices(); BindPending();
    }
    private void BindNewChoices()
    {
        NewRoles.Items.Clear(); NewProject.Items.Clear(); NewProject.Items.Add(new ListItem("None", "0"));
        int b; if (!Int32.TryParse(NewBusiness.SelectedValue, out b) || !AnyminuteModernScope.Owns(b)) return;
        foreach (DataRow r in AnyminuteEmployeeEditor.Roles(b).Rows) NewRoles.Items.Add(new ListItem(Convert.ToString(r["UserType"]), Convert.ToString(r["User_TypeId"])));
        var employeeRole = NewRoles.Items.FindByValue("4"); if (employeeRole != null) employeeRole.Selected = true;
        foreach (DataRow p in AnyminuteEmployeeEditor.Projects(b).Rows) NewProject.Items.Add(new ListItem(Convert.ToString(p["ProjectName"]), Convert.ToString(p["ProjectID"])));
    }
    protected void NewBusinessChanged(object sender, EventArgs e) { BindNewChoices(); }
    private void BindPending()
    {
        Pending.DataSource = PendingRows.Select(r => new { Business = r[1], RoleText = r[3], EmployeeType = r[4], PayType = r[5], RateText = "$" + Decimal.Parse(r[6], Inv).ToString("N2", Inv), EffectiveText = DateTime.Parse(r[7], Inv).ToString("MMM d, yyyy", Inv), Status = "Active" }).ToList();
        Pending.DataBind(); NoPending.Visible = PendingRows.Count == 0;
    }
    protected void AddPending_Click(object sender, EventArgs e)
    {
        try
        {
            int b; decimal rate; DateTime eff;
            if (!Int32.TryParse(NewBusiness.SelectedValue, out b) || !AnyminuteModernScope.Owns(b)) throw new InvalidOperationException("Select a business.");
            if (PendingRows.Any(r => r[0] == b.ToString(Inv))) throw new InvalidOperationException("This business is already in the list.");
            var roles = NewRoles.Items.Cast<ListItem>().Where(i => i.Selected).ToList(); if (roles.Count == 0) throw new InvalidOperationException("Select at least one role.");
            if (!Decimal.TryParse(NewRate.Text, NumberStyles.Number, Inv, out rate) || rate <= 0) throw new InvalidOperationException("Enter a pay rate greater than zero.");
            if (!DateTime.TryParse(NewEffective.Text, Inv, DateTimeStyles.None, out eff)) throw new InvalidOperationException("Enter the effective date.");
            PendingRows.Add(new[] { b.ToString(Inv), NewBusiness.SelectedItem.Text, String.Join(",", roles.Select(r => r.Value)), String.Join(", ", roles.Select(r => r.Text)), NewType.SelectedValue, NewPayType.SelectedValue, rate.ToString(Inv), eff.ToString("yyyy-MM-dd", Inv), NewProject.SelectedValue });
            NewRate.Text = ""; BindPending();
        }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }
    protected void Pending_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int i; if (e.CommandName == "Remove" && Int32.TryParse(Convert.ToString(e.CommandArgument), out i) && i >= 0 && i < PendingRows.Count) PendingRows.RemoveAt(i);
        BindPending();
    }

    // ---------- Edit Employee ----------
    private void SetupEdit()
    {
        PageTitle.Text = "Edit Employee"; PageSubtitle.Text = "Update employee details, assignments, pay and login access.";
        NewAssignments.Visible = NewAccess.Visible = false; HeaderCard.Visible = Tabs.Visible = true; Deactivate.Visible = card.Status != "Terminated";
        CloseLink.NavigateUrl = CancelLink.NavigateUrl = "~/Users.aspx"; SaveEmployee.Text = "Save Employee";
        HeadInitials.Text = HttpUtility.HtmlEncode(card.Initials); HeadName.Text = HttpUtility.HtmlEncode(card.Name); HeadEmail.Text = HttpUtility.HtmlEncode(card.Email);
        HeadPhone.Text = HttpUtility.HtmlEncode(card.Phone); HeadId.Text = employee.ToString(Inv); HeadStatus.Text = HttpUtility.HtmlEncode(card.Status); HeadStatus.CssClass = AnyminuteEmployeeEditor.StatusPill(card.Status);
        var access = AnyminuteEmployeeEditor.Access(card); HeadAccess.Text = HttpUtility.HtmlEncode(access.State); HeadAccessLink.NavigateUrl = Url("access");
        var tabs = new[] { new[] { "profile", "Employee Information" }, new[] { "address", "Address" }, new[] { "assignments", "Business Assignments" }, new[] { "rate", "Rate History" }, new[] { "access", "Login &amp; Access" } };
        var sb = new StringBuilder();
        foreach (var t in tabs) sb.Append("<a role=\"tab\" href=\"").Append(HttpUtility.HtmlAttributeEncode(ResolveUrl(Url(t[0])))).Append("\"").Append(t[0] == tab ? " class=\"is-active\" aria-selected=\"true\"" : "").Append(">").Append(t[1]).Append("</a>");
        TabLinks.Text = sb.ToString();
        bool form = tab == "profile" || tab == "address";
        InfoSection.Visible = AddressSection.Visible = form; SaveEmployee.Visible = form;
        if (form) { (tab == "profile" ? AddressSection : InfoSection).Style["display"] = "none"; if (!IsPostBack) LoadProfile(); }
        AssignmentsTab.Visible = tab == "assignments"; RateTab.Visible = tab == "rate"; AccessTab.Visible = tab == "access";
        if (tab == "assignments") { AddAssignmentLink.NavigateUrl = "~/EmployeeAssignment.aspx?EmployeeId=" + employee + "&mode=add"; Assignments.DataSource = AnyminuteEmployeeEditor.Assignments(card); Assignments.DataBind(); }
        if (tab == "rate") SetupRate();
        if (tab == "access") BindAccess(access, !IsPostBack);
    }
    private string Url(string t) { return "~/EmployeeEditor.aspx?EmployeeId=" + employee + "&tab=" + t; }
    private void LoadProfile()
    {
        var p = AnyminuteEmployeeEditor.LoadProfile(employee);
        FirstName.Text = p.FirstName; MiddleName.Text = p.MiddleName; LastName.Text = p.LastName; Email.Text = p.Email; Phone.Text = p.Phone; Mobile.Text = p.Mobile;
        OriginalDob = p.DateOfBirth; DateOfBirth.Text = IsoDate(p.DateOfBirth); OriginalSinExpiry = p.SinExpiry; SinExpiry.Text = IsoDate(p.SinExpiry);
        Select(Gender, p.Gender); Sin.Text = p.Sin; Select(Status, p.Status);
        HireDate.Text = p.HireDate.HasValue ? p.HireDate.Value.ToString("yyyy-MM-dd", Inv) : ""; HireDate.Enabled = false; HireDate.ToolTip = "Hire date is kept from the original record.";
        StreetNo.Text = p.StreetNo; Line1.Text = p.Line1; Line2.Text = p.Line2; Select(SuiteType, p.SuiteType); SuiteNumber.Text = p.SuiteNumber;
        City.Text = p.City; Province.Text = p.Province; PostalCode.Text = p.PostalCode; Country.Text = p.Country ?? "";
    }
    private static string IsoDate(string value) { DateTime d; return !String.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out d) ? d.ToString("yyyy-MM-dd", Inv) : ""; }
    private static void Select(DropDownList list, string value) { if (value != null && list.Items.FindByValue(value) == null && value.Trim().Length > 0) list.Items.Add(value); if (value != null && list.Items.FindByValue(value) != null) list.SelectedValue = value; }
    private AnyminuteEmployeeProfile ReadProfile()
    {
        var p = new AnyminuteEmployeeProfile();
        p.FirstName = FirstName.Text.Trim(); p.MiddleName = MiddleName.Text.Trim(); p.LastName = LastName.Text.Trim(); p.Email = Email.Text.Trim(); p.Phone = Phone.Text.Trim(); p.Mobile = Mobile.Text.Trim();
        // Keep a legacy value the date box could not show instead of wiping it.
        p.DateOfBirth = DateOfBirth.Text.Trim().Length > 0 ? DateOfBirth.Text.Trim() : (Editing && IsoDate(OriginalDob).Length == 0 ? OriginalDob : "");
        p.SinExpiry = SinExpiry.Text.Trim().Length > 0 ? SinExpiry.Text.Trim() : (Editing && IsoDate(OriginalSinExpiry).Length == 0 ? OriginalSinExpiry : "");
        p.Gender = Gender.SelectedValue; p.Sin = Sin.Text.Trim(); p.Status = Status.SelectedValue;
        DateTime hire; if (DateTime.TryParse(HireDate.Text, Inv, DateTimeStyles.None, out hire)) p.HireDate = hire;
        p.StreetNo = StreetNo.Text.Trim(); p.Line1 = Line1.Text.Trim(); p.Line2 = Line2.Text.Trim(); p.SuiteType = SuiteType.SelectedValue; p.SuiteNumber = SuiteNumber.Text.Trim();
        p.City = City.Text.Trim(); p.Province = Province.Text.Trim(); p.PostalCode = PostalCode.Text.Trim(); p.Country = Country.Text.Trim();
        return p;
    }
    protected void Save_Click(object sender, EventArgs e)
    {
        try
        {
            var p = ReadProfile();
            if (Editing)
            {
                AnyminuteEmployeeEditor.SaveProfile(employee, p);
                AnyminuteFlash.Set("editor", "Employee details for \u201c" + p.FirstName + " " + p.LastName + "\u201d were updated successfully.");
                Go(Url(tab)); return;
            }
            var rows = PendingRows.Select(r => new AnyminuteAssignmentInput { BusinessId = Int32.Parse(r[0], Inv), Roles = r[2].Split(',').Select(x => Int32.Parse(x, Inv)).ToArray(), EmployeeType = r[4], PayType = r[5], PayRate = Decimal.Parse(r[6], Inv), EffectiveDate = DateTime.Parse(r[7], Inv), ProjectId = Int32.Parse(r[8], Inv) }).ToList();
            string notice; AnyminuteEmployeeEditor.Create(p, rows, LoginOn.Checked, SendInvite.Checked && LoginOn.Checked, out notice);
            AnyminuteFlash.Set("employees", "Employee \u201c" + p.FirstName + " " + p.LastName + "\u201d was added successfully." + (String.IsNullOrEmpty(notice) ? "" : " " + notice));
            Go("~/Users.aspx");
        }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }
    protected void Deactivate_Click(object sender, EventArgs e)
    {
        try
        {
            var p = AnyminuteEmployeeEditor.LoadProfile(employee); p.Status = "Not Active";
            AnyminuteEmployeeEditor.SaveProfile(employee, p);
            AnyminuteFlash.Set("editor", "Employee was deactivated."); Go(Url(tab));
        }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }

    // ---------- Rate History ----------
    private void SetupRate()
    {
        RateCancel.NavigateUrl = Url("rate");
        if (!IsPostBack)
        {
            foreach (var a in card.Assignments.GroupBy(x => x.BusinessId).Select(g => g.First())) RateBusiness.Items.Add(new ListItem(a.Business, a.BusinessId.ToString(Inv)));
            string q = Request.QueryString["BusinessId"]; if (q != null && RateBusiness.Items.FindByValue(q) != null) RateBusiness.SelectedValue = q;
            RateBusinessField.Visible = RateBusiness.Items.Count > 1; BindRate(true);
        }
    }
    private void BindRate(bool fill)
    {
        int b; if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) return;
        var t = AnyminuteEmployeeEditor.RateHistory(AnyminuteEmployeeEditor.RecordFor(card, b)); RateRows.DataSource = t; RateRows.DataBind();
        if (!fill) return;
        var current = t.Rows.Cast<DataRow>().FirstOrDefault(r => (bool)r["Current"]);
        if (current != null) { Select(RateType, Convert.ToString(current["RateType"])); RateValue.Text = Convert.ToDecimal(current["Rate"]).ToString("0.00", Inv); }
        RateEffective.Text = DateTime.Today.ToString("yyyy-MM-dd", Inv); RateReason.Text = "";
    }
    protected void RateBusinessChanged(object sender, EventArgs e) { BindRate(true); }
    protected void UpdateRate_Click(object sender, EventArgs e)
    {
        try
        {
            int b; decimal rate; DateTime eff;
            if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) throw new InvalidOperationException("Select a business.");
            if (!Decimal.TryParse(RateValue.Text, NumberStyles.Number, Inv, out rate)) throw new InvalidOperationException("Enter the rate.");
            if (!DateTime.TryParse(RateEffective.Text, Inv, DateTimeStyles.None, out eff)) throw new InvalidOperationException("Enter the effective date.");
            AnyminuteEmployeeEditor.ChangeRate(employee, b, RateType.SelectedValue, rate, eff, RateReason.Text);
            AnyminuteFlash.Set("editor", "Pay rate updated successfully."); Go(Url("rate") + "&BusinessId=" + b);
        }
        catch (InvalidOperationException error) { ShowError(error.Message); BindRate(false); }
    }

    // ---------- Login & Access ----------
    private void BindAccess(AnyminuteEmployeeEditor.AccessState a, bool fill)
    {
        AccessState.Text = HttpUtility.HtmlEncode(a.State); AccessSent.Text = HttpUtility.HtmlEncode(a.SentText); AccessChanged.Text = HttpUtility.HtmlEncode(a.LastChange);
        AccessUser.Text = HttpUtility.HtmlEncode(a.Username.Length > 0 ? a.Username : a.Email); AccessRole.Text = HttpUtility.HtmlEncode(a.Role);
        ResendInvite.Enabled = CopyLink.Enabled = a.UserId > 0; CancelInvite.Enabled = a.UserId > 0 && a.Enabled;
        if (fill) { AccessOn.Checked = a.Enabled; AccessOff.Checked = !a.Enabled; RequireChange.Checked = a.RequireChange; TwoFactor.Checked = a.TwoFactor; }
        if (!a.TablesReady) ShowError(AnyminuteEmployeeEditor.MissingTables.Replace(" Nothing was changed.", " Settings below are read-only until it is installed."));
        var h = AnyminuteEmployeeEditor.AccessHistory(card); h.Columns.Add("WhenText");
        foreach (DataRow r in h.Rows) r["WhenText"] = r["RecordedAtUTC"] == DBNull.Value ? "" : DateTime.SpecifyKind(Convert.ToDateTime(r["RecordedAtUTC"]), DateTimeKind.Utc).ToLocalTime().ToString("MMM d, yyyy h:mm tt", Inv);
        AccessRows.DataSource = h; AccessRows.DataBind(); NoAccessRows.Visible = h.Rows.Count == 0;
    }
    private void AccessResult(Func<string> action)
    {
        try { string m = action(); if (m == AnyminuteEmployeeEditor.MissingTables) ShowError(m); else { AnyminuteFlash.Set("editor", m); Go(Url("access")); } }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }
    protected void SaveAccess_Click(object sender, EventArgs e)
    {
        AccessResult(() =>
        {
            string m = AnyminuteEmployeeEditor.SaveAccess(employee, AccessOn.Checked, RequireChange.Checked, TwoFactor.Checked);
            if (m != AnyminuteEmployeeEditor.MissingTables && AccessOn.Checked && WelcomeEmail.Checked) m += " " + AnyminuteEmployeeEditor.SendInvitation(employee, true);
            return m;
        });
    }
    protected void Resend_Click(object sender, EventArgs e) { AccessResult(() => AnyminuteEmployeeEditor.SendInvitation(employee, false)); }
    protected void CancelInvite_Click(object sender, EventArgs e) { AccessResult(() => AnyminuteEmployeeEditor.CancelInvitation(employee)); }
    protected void CopyLink_Click(object sender, EventArgs e)
    {
        try { InviteLink.Text = AnyminuteEmployeeEditor.InvitationLink(employee); InviteLink.Visible = true; ShowFlash("Invitation link copied. You can also copy it from the box below."); }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }

    private void ShowFlash(string m) { FlashText.Text = HttpUtility.HtmlEncode(m); Flash.Visible = true; }
    private void ShowError(string m) { PageError.Text = HttpUtility.HtmlEncode(m); PageError.Visible = true; }
    private void Go(string url) { Response.Redirect(url, false); Context.ApplicationInstance.CompleteRequest(); }
}
