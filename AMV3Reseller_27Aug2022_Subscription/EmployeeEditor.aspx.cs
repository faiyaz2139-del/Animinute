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
        Root.Attributes["class"] = "am-dialog-page am-employees-v2 am-editor-v3 " + (Editing ? "am-mode-edit" : "am-mode-add");
        if (!Editing) SetupAdd(); else SetupEdit();
    }

    // ---------- Add Employee ----------
    private void SetupAdd()
    {
        PageTitle.Text = "Add Employee"; PageSubtitle.Text = "Enter the employee details and assign to one or more businesses with roles and pay details.";
        // v7 Deck D slide 8: Login & Access block removed from Add mode, replaced by an info banner (panel kept, managed after save).
        NewAssignments.Visible = LookupSection.Visible = AddNotice.Visible = true; NewAccess.Visible = false; SaveEmployee.Text = "Save Employee";
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
    protected void NewBusinessChanged(object sender, EventArgs e) { BindNewChoices(); AssignmentModal.Visible = true; }
    private void BindPending()
    {
        Pending.DataSource = PendingRows.Select(r => new { Business = r[1], RoleText = r[3], EmployeeType = r[4], PayType = r[5], RateText = "$" + Decimal.Parse(r[6], Inv).ToString("N2", Inv), EffectiveText = DateTime.Parse(r[7], Inv).ToString("MMM d, yyyy", Inv), EndText = r[9].Length > 0 ? DateTime.Parse(r[9], Inv).ToString("MMM d, yyyy", Inv) : "\u2014", Status = r[10] }).ToList();
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
            DateTime end; string endText = "";
            if (NewEnd.Text.Trim().Length > 0) { if (!DateTime.TryParse(NewEnd.Text, Inv, DateTimeStyles.None, out end) || end.Date < eff.Date) throw new InvalidOperationException("End date cannot be before the effective date."); endText = end.ToString("yyyy-MM-dd", Inv); }
            decimal billing; string billingText = "";
            if (NewBilling.Text.Trim().Length > 0) { if (!Decimal.TryParse(NewBilling.Text, NumberStyles.Number, Inv, out billing) || billing < 0) throw new InvalidOperationException("Enter a valid billing rate."); billingText = billing.ToString(Inv); }
            string notes = NewNotes.Text.Trim(); if (notes.Length > 500) throw new InvalidOperationException("Notes can be up to 500 characters.");
            PendingRows.Add(new[] { b.ToString(Inv), NewBusiness.SelectedItem.Text, String.Join(",", roles.Select(r => r.Value)), String.Join(", ", roles.Select(r => r.Text)), NewType.SelectedValue, NewPayType.SelectedValue, rate.ToString(Inv), eff.ToString("yyyy-MM-dd", Inv), NewProject.SelectedValue, endText, NewStatus.SelectedValue, billingText, notes });
            NewRate.Text = NewBilling.Text = NewEnd.Text = NewNotes.Text = ""; AssignmentModal.Visible = false; BindPending();
        }
        catch (InvalidOperationException error) { ModalError.Text = HttpUtility.HtmlEncode(error.Message); ModalError.Visible = true; AssignmentModal.Visible = true; }
    }
    protected void OpenAssignment_Click(object sender, EventArgs e) { ModalError.Visible = false; AssignmentModal.Visible = true; if (NewEffective.Text.Length == 0) NewEffective.Text = DateTime.Today.ToString("yyyy-MM-dd", Inv); }
    protected void CloseAssignment_Click(object sender, EventArgs e) { AssignmentModal.Visible = false; }
    // ---------- Address lookup (same postal-code search as Add Business) ----------
    protected void SearchPostal_Click(object sender, EventArgs e)
    {
        string postal = AnyminuteBusinessAddressInput.Postal(LookupPostal.Text);
        LookupChoices.Items.Clear();
        if (!System.Text.RegularExpressions.Regex.IsMatch(postal, @"^[A-Z0-9-]{5,10}$")) { LookupChoices.Items.Add(new ListItem("Search a postal code first", "")); ShowError("Enter a postal code of 5 to 10 letters or digits."); return; }
        DataTable matches = new CommanClass().GetSearchAddress(postal); ViewState["EmpMatches"] = matches; ViewState["EmpPostal"] = postal;
        LookupChoices.Items.Add(new ListItem("Select an address", ""));
        for (int i = 0; i < matches.Rows.Count; i++) { string label = Convert.ToString(matches.Rows[i]["SEARCHADDRESS"]).Trim(); if (label.Length > 0 && !String.Equals(label, "ADD NEW ADDRESS", StringComparison.OrdinalIgnoreCase)) LookupChoices.Items.Add(new ListItem(label, i.ToString(Inv))); }
        if (LookupChoices.Items.Count == 1) { LookupChoices.Items[0].Text = "No matching addresses found"; ShowError("No matching addresses found. Check the postal code or use Add New Address to enter it manually."); }
    }
    protected void LookupChoice_Changed(object sender, EventArgs e)
    {
        var matches = ViewState["EmpMatches"] as DataTable; int i;
        if (matches == null || !Int32.TryParse(LookupChoices.SelectedValue, out i) || i < 0 || i >= matches.Rows.Count) return;
        var a = AnyminuteBusinessAddressInput.FromRow(matches.Rows[i], Convert.ToString(ViewState["EmpPostal"]));
        StreetNo.Text = a.Number; Line1.Text = a.Street; Line2.Text = a.Line2; City.Text = a.City; Select(Province, (a.Province ?? "").Trim().ToUpperInvariant()); PostalCode.Text = a.PostalCode; Select(Country, a.Country);
    }
    protected void AddNewAddress_Click(object sender, EventArgs e)
    {
        StreetNo.Text = Line1.Text = Line2.Text = SuiteNumber.Text = City.Text = ""; Province.SelectedIndex = 0; SuiteType.SelectedIndex = 0;
        PostalCode.Text = AnyminuteBusinessAddressInput.Postal(LookupPostal.Text); Select(Country, "Canada"); StreetNo.Focus();
    }
    protected void Pending_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int i; if (e.CommandName == "Remove" && Int32.TryParse(Convert.ToString(e.CommandArgument), out i) && i >= 0 && i < PendingRows.Count) PendingRows.RemoveAt(i);
        BindPending();
    }

    // ---------- Edit Employee ----------
    private void SetupEdit()
    {
        PageTitle.Text = "Edit Employee"; PageSubtitle.Text = "Update employee details, manage business assignments, pay rate history, and control login access.";
        NewAssignments.Visible = NewAccess.Visible = false; HeaderCard.Visible = Tabs.Visible = true; Deactivate.Visible = card.Status != "Terminated";
        CloseLink.NavigateUrl = CancelLink.NavigateUrl = "~/Users.aspx"; SaveEmployee.Text = "Save Employee";
        HeadInitials.Text = HttpUtility.HtmlEncode(card.Initials); HeadName.Text = HttpUtility.HtmlEncode(card.Name); HeadEmail.Text = HttpUtility.HtmlEncode(card.Email);
        HeadPhone.Text = HttpUtility.HtmlEncode(card.Phone);
        HeadStatus.Text = HttpUtility.HtmlEncode(tab == "access" ? "Employee " + card.Status : card.Status); HeadStatus.CssClass = AnyminuteEmployeeEditor.StatusPill(card.Status);
        HeadBusiness.Text = HttpUtility.HtmlEncode(String.Join(", ", card.Assignments.Select(x => x.Business).Distinct()));
        var access = AnyminuteEmployeeEditor.Access(card);
        HeadAccess.Text = HttpUtility.HtmlEncode(access.State); HeadAccess.CssClass = access.State == "Enabled" ? "am-pill-green" : access.State == "Disabled" ? "am-pill-red" : "am-pill-amber";
        HeadAccessLink.NavigateUrl = Url("access"); HeadLastLogin.Text = HttpUtility.HtmlEncode(access.LastLogin);
        var tabs = new[] { new[] { "profile", "Employee Information" }, new[] { "address", "Address" }, new[] { "assignments", "Business Assignments" }, new[] { "rate", "Rate History" }, new[] { "access", "Login &amp; Access" } };
        var sb = new StringBuilder();
        foreach (var t in tabs) sb.Append("<a role=\"tab\" href=\"").Append(HttpUtility.HtmlAttributeEncode(ResolveUrl(Url(t[0])))).Append("\"").Append(t[0] == tab ? " class=\"is-active\" aria-selected=\"true\"" : "").Append(">").Append(t[1]).Append("</a>");
        TabLinks.Text = sb.ToString();
        bool form = tab == "profile" || tab == "address";
        InfoSection.Visible = LookupSection.Visible = AddressSection.Visible = form; SaveEmployee.Visible = form; SaveAccess.Visible = tab == "access";
        // Deck B slide 9: the Employee Information tab shows Employee Information, Address, Employee Address and Business Assignments together.
        if (form) { if (tab == "address") InfoSection.Style["display"] = "none"; if (!IsPostBack) LoadProfile(); }
        AssignmentsTab.Visible = tab == "assignments" || tab == "profile"; RateTab.Visible = tab == "rate"; AccessTab.Visible = tab == "access";
        if (AssignmentsTab.Visible) { AddAssignmentLink.NavigateUrl = "~/EmployeeAssignment.aspx?EmployeeId=" + employee + "&mode=add"; var rows = AnyminuteEmployeeEditor.Assignments(card); AssignCount.Text = rows.Count.ToString(Inv); Assignments.DataSource = rows; Assignments.DataBind(); }
        // Slide 11 header shows date of birth; slide 10 header shows the role and a Business selector instead of the access box.
        HeadDobWrap.Visible = false;
        if (tab == "access" || form) { string dob = IsoDate(AnyminuteEmployeeEditor.LoadProfile(employee).DateOfBirth); HeadDob.Text = dob; HeadDobWrap.Visible = dob.Length > 0; }
        if (form) { HeadAccessBox.CssClass += " am-access-s9"; HeadAccessLink.Text = "Go to Login &amp; Access &rarr;"; }
        if (tab == "rate") { HeadAccessBox.Visible = false; HeadBusinessBox.Visible = true; HeadRole.Visible = true; HeadBusinessWrap.Visible = false; Footer.Visible = false; SetupRate(); }
        if (tab == "access") BindAccess(access, !IsPostBack);
    }
    private string Url(string t) { return "~/EmployeeEditor.aspx?EmployeeId=" + employee + "&tab=" + t; }
    private void LoadProfile()
    {
        var p = AnyminuteEmployeeEditor.LoadProfile(employee);
        FirstName.Text = p.FirstName; MiddleName.Text = p.MiddleName; LastName.Text = p.LastName; Email.Text = p.Email; Phone.Text = p.Phone; Mobile.Text = p.Mobile;
        OriginalDob = p.DateOfBirth; DateOfBirth.Text = IsoDate(p.DateOfBirth); OriginalSinExpiry = p.SinExpiry; SinExpiry.Text = IsoDate(p.SinExpiry);
        Select(Gender, p.Gender); Sin.Text = p.Sin; Select(Status, p.Status);
        HireDate.Text = p.HireDate.HasValue ? p.HireDate.Value.ToString("yyyy-MM-dd", Inv) : ""; HireDate.ReadOnly = true; HireDate.ToolTip = "Hire date is kept from the original record.";
        StreetNo.Text = p.StreetNo; Line1.Text = p.Line1; Line2.Text = p.Line2; Select(SuiteType, p.SuiteType); SuiteNumber.Text = p.SuiteNumber;
        City.Text = p.City; Select(Province, p.Province); PostalCode.Text = p.PostalCode; Select(Country, String.IsNullOrWhiteSpace(p.Country) ? "Canada" : p.Country);
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
        p.City = City.Text.Trim(); p.Province = Province.SelectedValue.Trim(); p.PostalCode = PostalCode.Text.Trim(); p.Country = Country.SelectedValue;
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
            var rows = PendingRows.Select(r => new AnyminuteAssignmentInput { BusinessId = Int32.Parse(r[0], Inv), Roles = r[2].Split(',').Select(x => Int32.Parse(x, Inv)).ToArray(), EmployeeType = r[4], PayType = r[5], PayRate = Decimal.Parse(r[6], Inv), EffectiveDate = DateTime.Parse(r[7], Inv), ProjectId = Int32.Parse(r[8], Inv), EndDate = r[9].Length > 0 ? (DateTime?)DateTime.Parse(r[9], Inv) : null, Status = r[10], BillingRate = r[11].Length > 0 ? (decimal?)Decimal.Parse(r[11], Inv) : null, Notes = r[12] }).ToList();
            string notice; int created = AnyminuteEmployeeEditor.Create(p, rows, NewAccess.Visible ? LoginOn.Checked : true, NewAccess.Visible ? SendInvite.Checked && LoginOn.Checked : false, out notice);
            if (!NewAccess.Visible) notice = (notice + " Send the login invitation from the employee's Login & Access tab.").Trim();
            // Assignment status other than Active is applied after the legacy create (which always starts Active).
            foreach (var r in rows.Where(x => x.Status != "Active"))
                try { AnyminuteEmployeeEditor.SaveAssignment(created, r); } catch (InvalidOperationException statusError) { notice = (notice + " " + statusError.Message).Trim(); }
            AnyminuteFlash.Set("employees", "Employee \u201c" + p.FirstName + " " + p.LastName + "\u201d was added successfully." + (String.IsNullOrEmpty(notice) ? "" : " " + notice));
            Go("~/Users.aspx");
        }
        catch (InvalidOperationException error) { ShowError(error.Message); }
    }
    protected void Assignments_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int b; if (e.CommandName != "End" || !Int32.TryParse(Convert.ToString(e.CommandArgument), out b)) return;
        try { AnyminuteEmployeeEditor.EndAssignment(employee, b); AnyminuteFlash.Set("editor", "Assignment removed (set to Not Active, end date today)."); Go(Url(tab)); }
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

    // ---------- Rate History (slide 10) ----------
    private bool AddingRate { get { return Convert.ToBoolean(ViewState["AddingRate"] ?? false); } set { ViewState["AddingRate"] = value; } }
    private void SetupRate()
    {
        if (!IsPostBack)
        {
            foreach (var a in card.Assignments.GroupBy(x => x.BusinessId).Select(g => g.First())) { RateBusiness.Items.Add(new ListItem(a.Business, a.BusinessId.ToString(Inv))); HeadBusinessSelect.Items.Add(new ListItem(a.Business, a.BusinessId.ToString(Inv))); }
            string q = Request.QueryString["BusinessId"]; if (q != null && RateBusiness.Items.FindByValue(q) != null) RateBusiness.SelectedValue = HeadBusinessSelect.SelectedValue = q;
            BindRate(true);
        }
        SetRateMode();
    }
    private void SetRateMode()
    {
        RateFormTitle.Text = AddingRate ? "Add New Rate" : "Current Pay Rate";
        RateFormHint.Text = AddingRate ? "Create a new rate for a future change. The current rate will end the day before the new effective date." : "Update the employee's current pay rate. Changes will take effect from the selected effective date.";
        RateValueLabel.Text = AddingRate ? "New Pay Rate" : "Current Pay Rate"; UpdateRateText.Text = AddingRate ? "Add New Rate" : "Update Current Rate";
        int b; var a = Int32.TryParse(RateBusiness.SelectedValue, out b) ? card.Assignments.FirstOrDefault(x => x.BusinessId == b) : null;
        HeadRole.Text = a == null ? "" : HttpUtility.HtmlEncode(a.Role);
    }
    private void BindRate(bool fill)
    {
        int b; if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) return;
        var t = AnyminuteEmployeeEditor.RateHistory(AnyminuteEmployeeEditor.RecordFor(card, b)); RateRows.DataSource = t; RateRows.DataBind();
        if (!fill) return;
        var current = t.Rows.Cast<DataRow>().FirstOrDefault(r => (bool)r["Current"]);
        if (AddingRate || current == null) { RateValue.Text = ""; RateEffective.Text = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd", Inv); if (current != null) Select(RateType, Convert.ToString(current["RateType"])); }
        else { Select(RateType, Convert.ToString(current["RateType"])); RateValue.Text = Convert.ToDecimal(current["Rate"]).ToString("0.00", Inv); RateEffective.Text = Convert.ToDateTime(current["effectivedate"]).ToString("yyyy-MM-dd", Inv); }
        RateReason.Text = "";
    }
    private void SwitchBusiness(string value) { RateBusiness.SelectedValue = HeadBusinessSelect.SelectedValue = value; AddingRate = false; BindRate(true); SetRateMode(); }
    protected void RateBusinessChanged(object sender, EventArgs e) { SwitchBusiness(RateBusiness.SelectedValue); }
    protected void HeadBusinessChanged(object sender, EventArgs e) { SwitchBusiness(HeadBusinessSelect.SelectedValue); }
    protected void AddNewRate_Click(object sender, EventArgs e) { AddingRate = true; BindRate(true); SetRateMode(); RateValue.Focus(); }
    protected void RateCancel_Click(object sender, EventArgs e) { AddingRate = false; BindRate(true); SetRateMode(); }
    protected void RateRows_ItemCommand(object source, RepeaterCommandEventArgs e) { if (e.CommandName == "EditCurrent") { AddingRate = false; BindRate(true); SetRateMode(); RateValue.Focus(); } }
    protected void UpdateRate_Click(object sender, EventArgs e)
    {
        try
        {
            int b; decimal rate; DateTime eff;
            if (!Int32.TryParse(RateBusiness.SelectedValue, out b)) throw new InvalidOperationException("Select a business.");
            if (!Decimal.TryParse(RateValue.Text, NumberStyles.Number, Inv, out rate)) throw new InvalidOperationException("Enter the pay rate.");
            if (!DateTime.TryParse(RateEffective.Text, Inv, DateTimeStyles.None, out eff)) throw new InvalidOperationException("Enter the effective date.");
            if (AddingRate) AnyminuteEmployeeEditor.ChangeRate(employee, b, RateType.SelectedValue, rate, eff, RateReason.Text);
            else AnyminuteEmployeeEditor.UpdateCurrentRate(employee, b, RateType.SelectedValue, rate, eff, RateReason.Text);
            AnyminuteFlash.Set("editor", AddingRate ? "New pay rate added successfully." : "Current pay rate updated successfully."); Go(Url("rate") + "&BusinessId=" + b);
        }
        catch (InvalidOperationException error) { ShowError(error.Message); BindRate(false); }
    }

    // ---------- Login & Access (slide 11) ----------
    private static readonly Dictionary<string, string> ActionNames = new Dictionary<string, string> { { "SendInvitation", "Invitation Sent" }, { "CreateEmployee", "Account Created" }, { "LoginEnabled", "Login Access Enabled" }, { "LoginDisabled", "Login Access Disabled" }, { "CancelInvitation", "Invitation Cancelled" }, { "CopyInvitationLink", "Invitation Link Copied" }, { "RequirePasswordChange", "Password Change Required" }, { "TwoFactorChanged", "2FA Setting Changed" }, { "PortalRoleChanged", "Portal Role Changed" } };
    private void BindAccess(AnyminuteEmployeeEditor.AccessState a, bool fill)
    {
        AccessState.Text = HttpUtility.HtmlEncode(a.State);
        AccessStateText.Text = HttpUtility.HtmlEncode(a.UserId == 0 ? "No login account is linked to this employee yet."
            : !a.Enabled ? "Login access is disabled. The employee cannot sign in until access is enabled."
            : a.Pending ? "A secure invitation email has been sent to " + (a.Email.Length > 0 ? a.Email : "the employee") + (a.SentText != "\u2014" ? " on " + a.SentText : "") + ". The employee needs to set their password to activate login access."
            : "The employee has activated their account and can sign in to AnyMinute.");
        AccessStatusBox.CssClass = "am-status-box " + (!a.Enabled ? "am-off" : a.Pending ? "am-pending" : "am-on");
        AccessUser.Text = a.Username; AccessEmail.Text = a.Email;
        ResendInvite.Enabled = CopyLink.Enabled = a.UserId > 0; CancelInvite.Enabled = a.UserId > 0 && a.Enabled;
        if (fill)
        {
            AccessOn.Checked = a.Enabled; AccessOff.Checked = !a.Enabled; RequireChange.Checked = a.RequireChange; TwoFactor.Checked = a.TwoFactor;
            var first = card.First; AccessRole.Items.Clear();
            // v16: Role for Portal Access offers only Employee, Manager and Administrator (Accountant, Product Manager,
            // Senior Area Manager and other custom roles removed). An existing other role stays listed so it is not changed silently.
            foreach (DataRow r in AnyminuteEmployeeEditor.Roles(first.BusinessId).Rows)
            {
                int id = Convert.ToInt32(r["User_TypeId"]);
                if (id == 2 || id == 4 || id == 5 || id == first.RoleId) AccessRole.Items.Add(new ListItem(Convert.ToString(r["UserType"]), id.ToString(Inv)));
            }
            Select(AccessRole, first.RoleId.ToString(Inv)); AccessRole.ToolTip = "Role in " + first.Business;
        }
        if (!a.TablesReady) ShowError(AnyminuteEmployeeEditor.MissingTables.Replace(" Nothing was changed.", " Login settings cannot be saved until it is installed."));
        var h = AnyminuteEmployeeEditor.AccessHistory(card); h.Columns.Add("WhenText"); h.Columns.Add("ActionText");
        foreach (DataRow r in h.Rows)
        {
            r["WhenText"] = r["RecordedAtUTC"] == DBNull.Value ? "" : DateTime.SpecifyKind(Convert.ToDateTime(r["RecordedAtUTC"]), DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd hh:mm tt", Inv);
            string k = Convert.ToString(r["Action"]); r["ActionText"] = ActionNames.ContainsKey(k) ? ActionNames[k] : k;
        }
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
            int role; var first = card.First;
            if (Int32.TryParse(AccessRole.SelectedValue, out role) && role != first.RoleId && AccessRole.Items.FindByValue(role.ToString(Inv)) != null)
            {
                AnyminuteEmployeeAssignmentService.SaveRole(AnyminuteEmployeeEditor.RecordFor(card, first.BusinessId), first.BusinessId, role);
                m = (m == AnyminuteEmployeeEditor.MissingTables ? "Portal role updated. Login settings need the database update." : m + " Portal role updated.");
            }
            return m;
        });
    }
    protected void Resend_Click(object sender, EventArgs e) { AccessResult(() => AnyminuteEmployeeEditor.SendInvitation(employee, WelcomeEmail.Checked)); }
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
