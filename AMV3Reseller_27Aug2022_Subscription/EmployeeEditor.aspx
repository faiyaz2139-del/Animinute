<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeEditor.aspx.cs" Inherits="AnyminuteEmployeeEditorPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261024v3" rel="stylesheet" /><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-dialog-page am-employees-v2 am-editor-v3">
 <header class="am-dialog-head"><div><h1><asp:Literal ID="PageTitle" runat="server" /></h1><p><asp:Literal ID="PageSubtitle" runat="server" /></p></div><asp:HyperLink ID="CloseLink" runat="server" NavigateUrl="~/Users.aspx" CssClass="am-dialog-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <asp:Panel ID="Tabs" runat="server" Visible="false" CssClass="am-tabs" role="tablist"><asp:Literal ID="TabLinks" runat="server" /></asp:Panel>
 <div class="am-dialog-body">
  <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />

  <asp:Panel ID="HeaderCard" runat="server" Visible="false" CssClass="am-info-card am-head-v3"><span class="am-avatar am-avatar-lg"><asp:Literal ID="HeadInitials" runat="server" /></span>
   <div class="am-info-main"><div><strong class="am-head-name"><asp:Literal ID="HeadName" runat="server" /></strong> <asp:Label ID="HeadStatus" runat="server" /></div><asp:Label ID="HeadRole" runat="server" CssClass="am-hint" Visible="false" />
    <div class="am-info-meta"><span>&#9993; <asp:Literal ID="HeadEmail" runat="server" /></span><span>&#9742; <asp:Literal ID="HeadPhone" runat="server" /></span><asp:Label ID="HeadDobWrap" runat="server"><span>&#128197; <asp:Literal ID="HeadDob" runat="server" /></span></asp:Label><asp:Label ID="HeadBusinessWrap" runat="server"><span>&#128188; <asp:Literal ID="HeadBusiness" runat="server" /></span></asp:Label></div></div>
   <asp:Panel ID="HeadAccessBox" runat="server" CssClass="am-access-box am-access-v3"><div><span>Login Access</span> <asp:Label ID="HeadAccess" runat="server" /></div><asp:HyperLink ID="HeadAccessLink" runat="server" CssClass="am-button secondary">Manage Login &amp; Access &rarr;</asp:HyperLink></asp:Panel>
   <asp:Panel ID="HeadBusinessBox" runat="server" Visible="false" CssClass="am-head-business"><label class="am-field"><span>Business</span><asp:DropDownList ID="HeadBusinessSelect" runat="server" AutoPostBack="true" OnSelectedIndexChanged="HeadBusinessChanged" /></label></asp:Panel>
  </asp:Panel>

  <asp:Panel ID="InfoSection" runat="server" CssClass="am-section"><h2>Employee Information</h2><div class="am-grid am-cols-4">
   <label class="am-field"><span>First Name <span class="am-req">(Required)</span></span><asp:TextBox ID="FirstName" runat="server" MaxLength="50" placeholder="Enter first name" /></label>
   <label class="am-field"><span>Last Name <span class="am-req">(Required)</span></span><asp:TextBox ID="LastName" runat="server" MaxLength="50" placeholder="Enter last name" /></label>
   <label class="am-field"><span>Email <span class="am-hint">(To grant online access)</span></span><asp:TextBox ID="Email" runat="server" TextMode="Email" MaxLength="100" placeholder="Enter email address" /></label>
   <label class="am-field"><span>Contact Phone <span class="am-req">(Required)</span></span><span class="am-phone"><span class="am-phone-cc" aria-label="Canada +1">&#127464;&#127462; +1</span><asp:TextBox ID="Phone" runat="server" TextMode="Phone" MaxLength="20" placeholder="(416) 555-1234" /></span></label>
   <label class="am-field"><span>Mobile <span class="am-hint">(Optional)</span></span><span class="am-phone"><span class="am-phone-cc" aria-label="Canada +1">&#127464;&#127462; +1</span><asp:TextBox ID="Mobile" runat="server" TextMode="Phone" MaxLength="20" placeholder="(416) 555-1234" /></span></label>
   <label class="am-field"><span>Date of Birth <span class="am-hint">(Optional)</span></span><asp:TextBox ID="DateOfBirth" runat="server" TextMode="Date" /></label>
   <label class="am-field"><span>Gender</span><asp:DropDownList ID="Gender" runat="server"><asp:ListItem Value="">Select</asp:ListItem><asp:ListItem>Male</asp:ListItem><asp:ListItem>Female</asp:ListItem><asp:ListItem>Other</asp:ListItem></asp:DropDownList></label>
   <label class="am-field"><span>SIN Number</span><asp:TextBox ID="Sin" runat="server" MaxLength="11" autocomplete="off" placeholder="### ### ###" /></label>
   <label class="am-field"><span>Hire Date <span class="am-req">(Required)</span></span><asp:TextBox ID="HireDate" runat="server" TextMode="Date" /><span class="am-req am-small">* Hire Date cannot be changed or edited.</span></label>
   <label class="am-field"><span>SIN Expiry Date <span class="am-hint">(If Any)</span></span><asp:TextBox ID="SinExpiry" runat="server" TextMode="Date" /></label>
   <label class="am-field"><span>Employee Status <span class="am-req">(Required)</span></span><asp:DropDownList ID="Status" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Not Active</asp:ListItem><asp:ListItem>Terminated</asp:ListItem></asp:DropDownList></label>
  </div><div hidden="hidden"><asp:TextBox ID="MiddleName" runat="server" MaxLength="50" /></div></asp:Panel>

  <asp:Panel ID="LookupSection" runat="server" CssClass="am-section" DefaultButton="SearchPostal"><h2>&#128205; Address Lookup</h2><p class="am-hint">Enter a postal code to search for an address, then select one from the list below or add a new address.</p>
   <div class="am-lookup-v3">
    <label class="am-field"><span>Enter Postal Code <span class="am-req">(Required)</span></span><span class="am-search-row"><asp:TextBox ID="LookupPostal" runat="server" MaxLength="10" placeholder="Enter postal code" /><asp:LinkButton ID="SearchPostal" runat="server" OnClick="SearchPostal_Click" CausesValidation="false" CssClass="am-button" ToolTip="Search">&#128269;<span class="am-visually-hidden"> Search</span></asp:LinkButton></span></label>
    <label class="am-field"><span>Select Address from List or Add New <span class="am-req">(Required)</span></span><asp:DropDownList ID="LookupChoices" runat="server" AutoPostBack="true" OnSelectedIndexChanged="LookupChoice_Changed"><asp:ListItem Value="">Search a postal code first</asp:ListItem></asp:DropDownList></label>
    <div class="am-field"><span>&nbsp;</span><asp:LinkButton ID="AddNewAddress" runat="server" OnClick="AddNewAddress_Click" CausesValidation="false" CssClass="am-button secondary">+&nbsp; Add New Address</asp:LinkButton></div>
   </div></asp:Panel>

  <asp:Panel ID="AddressSection" runat="server" CssClass="am-section"><h2><asp:Literal ID="AddressTitle" runat="server" Text="Employee Address" /></h2><p class="am-note">When an address is selected from the list, the fields below will be automatically filled. You can also enter or edit the address manually.</p><div class="am-grid am-cols-3">
   <label class="am-field"><span>Street Number <span class="am-req">(Required)</span></span><asp:TextBox ID="StreetNo" runat="server" MaxLength="20" /></label>
   <label class="am-field"><span>Address Line 1 <span class="am-req">(Required)</span></span><asp:TextBox ID="Line1" runat="server" MaxLength="200" /></label>
   <label class="am-field"><span>Address Line 2 <span class="am-hint">(Optional)</span></span><asp:TextBox ID="Line2" runat="server" MaxLength="200" placeholder="Enter address line 2 (e.g. Unit, Floor, etc.)" /></label>
   <label class="am-field"><span>Suite Number</span><asp:TextBox ID="SuiteNumber" runat="server" MaxLength="20" /></label>
   <label class="am-field"><span>Suite Type</span><asp:DropDownList ID="SuiteType" runat="server"><asp:ListItem Value="">Select</asp:ListItem><asp:ListItem>Suite</asp:ListItem><asp:ListItem>Apt</asp:ListItem><asp:ListItem>Unit</asp:ListItem></asp:DropDownList></label>
   <label class="am-field"><span>City <span class="am-req">(Required)</span></span><asp:TextBox ID="City" runat="server" MaxLength="100" /></label>
   <label class="am-field"><span>Province/State <span class="am-req">(Required)</span></span><asp:TextBox ID="Province" runat="server" MaxLength="100" list="am-emp-provinces" CssClass="am-select-look" /></label>
   <label class="am-field"><span>Postal Code <span class="am-req">(Required)</span></span><asp:TextBox ID="PostalCode" runat="server" MaxLength="20" /></label>
   <label class="am-field"><span>Country</span><asp:DropDownList ID="Country" runat="server"><asp:ListItem>Canada</asp:ListItem><asp:ListItem>United States</asp:ListItem></asp:DropDownList></label>
  </div><datalist id="am-emp-provinces"><option value="ALBERTA"></option><option value="BRITISH COLUMBIA"></option><option value="MANITOBA"></option><option value="NEW BRUNSWICK"></option><option value="NEWFOUNDLAND AND LABRADOR"></option><option value="NORTHWEST TERRITORIES"></option><option value="NOVA SCOTIA"></option><option value="NUNAVUT"></option><option value="ONTARIO"></option><option value="PRINCE EDWARD ISLAND"></option><option value="QUEBEC"></option><option value="SASKATCHEWAN"></option><option value="YUKON"></option></datalist></asp:Panel>

  <%-- Add mode: assignments are collected here and created together with the employee. --%>
  <asp:Panel ID="NewAssignments" runat="server" CssClass="am-section"><div class="am-section-head"><div><h2>Business Assignments</h2><p class="am-hint">Assign this employee to one or more businesses with roles and pay details.</p></div><asp:LinkButton ID="OpenAssignment" runat="server" OnClick="OpenAssignment_Click" CausesValidation="false" CssClass="am-button">+&nbsp; Add Assignment</asp:LinkButton></div>
   <div class="am-table-wrap"><table class="am-data"><thead><tr><th>Business</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Effective Date</th><th>End Date</th><th>Status</th><th>Action</th></tr></thead><tbody>
   <asp:Repeater ID="Pending" runat="server" OnItemCommand="Pending_ItemCommand"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><%#: Eval("RoleText") %></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("RateText") %></td><td><%#: Eval("EffectiveText") %></td><td><%#: Eval("EndText") %></td><td><span class='<%# AnyminuteEmployeeEditor.StatusPill(Eval("Status")) %>'><%#: Eval("Status") %></span></td><td><asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' CausesValidation="false" CssClass="am-act am-act-del" ToolTip="Remove">&#128465;</asp:LinkButton></td></tr></ItemTemplate></asp:Repeater>
   <asp:PlaceHolder ID="NoPending" runat="server"><tr><td colspan="9" class="am-empty-v3"><span aria-hidden="true">&#128188;</span><strong>No business assignments</strong><span>Click "Add Assignment" to assign this employee to a business.</span></td></tr></asp:PlaceHolder>
   </tbody></table></div></asp:Panel>

  <asp:Panel ID="NewAccess" runat="server" CssClass="am-section"><h2>Login &amp; Access</h2><p class="am-hint">Control whether this employee can sign in to AnyMinute.</p>
   <div class="am-login-v3">
    <div class="am-login-radios"><strong>Login Access <span class="am-help" title="Enabled employees can sign in with their email after setting a password.">?</span></strong><asp:RadioButton ID="LoginOn" runat="server" GroupName="login" Checked="true" Text="Enable Login Access" CssClass="am-radio" /><asp:RadioButton ID="LoginOff" runat="server" GroupName="login" Text="Disable Login Access" CssClass="am-radio" /></div>
    <div class="am-invite-box"><asp:CheckBox ID="SendInvite" runat="server" Checked="true" Text="Send email invitation to activate account" /><span class="am-hint">The employee will receive a secure account setup invitation after being saved.</span></div>
    <span id="am-login-pill" class="am-login-pill">&#128274; Login Access: Enabled</span>
   </div></asp:Panel>

  <%-- Edit mode tabs --%>
  <asp:Panel ID="AssignmentsTab" runat="server" Visible="false" CssClass="am-section"><div class="am-dialog-head" style="padding:0 0 12px;border:0"><h2>Business Assignments</h2><asp:HyperLink ID="AddAssignmentLink" runat="server" CssClass="am-button">+ Add Business Assignment</asp:HyperLink></div>
   <div class="am-table-wrap"><table class="am-data"><thead><tr><th>Business</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Effective Date</th><th>Status</th><th>Action</th></tr></thead><tbody>
   <asp:Repeater ID="Assignments" runat="server"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><div class="am-role-chips"><asp:Repeater runat="server" DataSource='<%# Eval("Roles") %>'><ItemTemplate><span class='<%# "am-role-chip " + AnyminuteEmployeeEditor.RoleCss(Container.DataItem) %>'><%#: Container.DataItem %></span></ItemTemplate></asp:Repeater></div></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("PayRateText") %></td><td><%#: Eval("EffectiveText") %></td><td><span class='<%# AnyminuteEmployeeEditor.StatusPill(Eval("Status")) %>'><%#: Eval("Status") %></span></td><td><a class="am-act" title="Edit assignment" href='<%# "EmployeeAssignment.aspx?EmployeeId=" + Eval("EmployeeId") + "&BusinessId=" + Eval("BusinessId") %>'>&#9998;</a></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table></div></asp:Panel>

  <asp:Panel ID="RateTab" runat="server" Visible="false">
   <div class="am-section am-card-v3"><h2><asp:Literal ID="RateFormTitle" runat="server" Text="Current Pay Rate" /></h2><p class="am-hint"><asp:Literal ID="RateFormHint" runat="server" /></p><div class="am-grid am-cols-4">
    <label class="am-field"><span>Rate Type <span class="am-req">(Required)</span></span><asp:DropDownList ID="RateType" runat="server" /></label>
    <label class="am-field"><span><asp:Literal ID="RateValueLabel" runat="server" Text="Current Pay Rate" /> <span class="am-req">(Required)</span></span><span class="am-money"><asp:TextBox ID="RateValue" runat="server" TextMode="Number" step="0.01" min="0" /></span></label>
    <label class="am-field"><span>Effective Date <span class="am-req">(Required)</span></span><asp:TextBox ID="RateEffective" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>Reason for Change <span class="am-req">(Required)</span></span><asp:TextBox ID="RateReason" runat="server" TextMode="MultiLine" Rows="2" MaxLength="200" ClientIDMode="Static" placeholder="e.g. Increase in hourly pay rate." /><span class="am-counter"><span id="am-reason-left">200</span></span></label>
   </div><div class="am-check-row"><asp:LinkButton ID="UpdateRate" runat="server" OnClick="UpdateRate_Click" CssClass="am-button"><asp:Literal ID="UpdateRateText" runat="server" Text="Update Current Rate" /></asp:LinkButton><asp:LinkButton ID="RateCancel" runat="server" OnClick="RateCancel_Click" CausesValidation="false" CssClass="am-button secondary">Cancel</asp:LinkButton></div></div>
   <div class="am-section am-card-v3"><div class="am-section-head"><div><h2>Rate History</h2><p class="am-hint">View the employee's pay rate history for the selected business. Historical rates are read-only.</p></div><asp:LinkButton ID="AddNewRate" runat="server" OnClick="AddNewRate_Click" CausesValidation="false" CssClass="am-button">+&nbsp; Add New Rate</asp:LinkButton></div>
    <label class="am-field am-inline-field"><span>Select Business <span class="am-req">(Required)</span></span><asp:DropDownList ID="RateBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RateBusinessChanged" /></label>
    <div class="am-table-wrap"><table class="am-data"><thead><tr><th>#</th><th>Rate Type</th><th>Start Date</th><th>End Date</th><th>Pay Rate</th><th>Status</th><th>Action</th></tr></thead><tbody>
    <asp:Repeater ID="RateRows" runat="server" OnItemCommand="RateRows_ItemCommand"><ItemTemplate><tr><td><%# Eval("No") %></td><td><%#: Eval("RateType") %></td><td><%#: Eval("StartText") %></td><td><%#: Eval("EndText") %></td><td><%#: Eval("RateText") %></td><td><span class='<%# (bool)Eval("Current") ? "am-pill-green" : "am-pill-blue" %>'><%# (bool)Eval("Current") ? "Current" : "Historical" %></span></td><td><asp:LinkButton runat="server" Visible='<%# (bool)Eval("Current") %>' CommandName="EditCurrent" CausesValidation="false" CssClass="am-link-act">&#9998; Edit</asp:LinkButton><asp:Label runat="server" Visible='<%# !(bool)Eval("Current") %>' CssClass="am-hint" ToolTip="Historical rates are locked">&#128274; Locked</asp:Label></td></tr></ItemTemplate></asp:Repeater>
    </tbody></table></div></div>
   <div class="am-rate-info"><span class="am-rate-info-icon" aria-hidden="true">&#128274;</span><div><strong>Important Information About Pay Rates</strong><p>Current pay rate can be edited to update the employee's rate. Historical rates are locked and read-only. Use "Add New Rate" for future changes. Any corrections to historical data require a controlled, audited process.</p>
    <ul><li>The current rate is editable and will take effect from the selected effective date.</li><li>Use Add New Rate to create a new rate for future changes.</li><li>Historical rates are locked and read-only (no edit or delete).</li><li>Corrections to historical data require a controlled, audited process.</li></ul></div></div>
  </asp:Panel>

  <asp:Panel ID="AccessTab" runat="server" Visible="false">
   <div class="am-cards-3">
    <div class="am-card-v3"><h3>&#128274; Current Access Status</h3><asp:Panel ID="AccessStatusBox" runat="server" CssClass="am-status-box"><strong><asp:Literal ID="AccessState" runat="server" /></strong><p><asp:Literal ID="AccessStateText" runat="server" /></p></asp:Panel></div>
    <div class="am-card-v3"><h3>&#128100; Account Information</h3>
     <label class="am-field"><span>Username</span><span class="am-tagged"><asp:TextBox ID="AccessUser" runat="server" ReadOnly="true" /><em>Generated</em></span></label>
     <label class="am-field"><span>Login Email</span><span class="am-tagged"><asp:TextBox ID="AccessEmail" runat="server" ReadOnly="true" /><em>&#9993;</em></span></label>
     <label class="am-field"><span>Role for Portal Access</span><asp:DropDownList ID="AccessRole" runat="server" /></label></div>
    <div class="am-card-v3 am-actions-v3"><h3>&#9881; Actions</h3>
     <asp:LinkButton ID="ResendInvite" runat="server" OnClick="Resend_Click" CausesValidation="false" CssClass="am-button am-block">&#10148; Resend Invitation Email</asp:LinkButton><span class="am-hint">Send a new invitation email to the employee.</span>
     <asp:LinkButton ID="CopyLink" runat="server" OnClick="CopyLink_Click" CausesValidation="false" CssClass="am-button secondary am-block">&#128279; Copy Invitation Link</asp:LinkButton><span class="am-hint">Copy a secure activation link to share with the employee.</span>
     <asp:LinkButton ID="CancelInvite" runat="server" OnClick="CancelInvite_Click" CausesValidation="false" CssClass="am-button am-outline-danger am-block" OnClientClick="return confirm('Cancel the pending invitation? This will disable login access.');">&#8856; Cancel Invitation</asp:LinkButton><span class="am-hint">Cancel the pending invitation. This will disable login access.</span></div>
   </div>
   <asp:TextBox ID="InviteLink" runat="server" Visible="false" ReadOnly="true" ClientIDMode="Static" CssClass="am-invite-link" />
   <div class="am-cards-3">
    <div class="am-card-v3"><h3>&#128274; Login Access Settings</h3>
     <div class="am-radio-block"><asp:RadioButton ID="AccessOn" runat="server" GroupName="access" Text="Enable Login Access" /><span class="am-hint">Employee can sign in to AnyMinute.</span></div>
     <div class="am-radio-block"><asp:RadioButton ID="AccessOff" runat="server" GroupName="access" Text="Disable Login Access" /><span class="am-hint">Prevent the employee from signing in. (Existing invitation will be cancelled)</span></div></div>
    <div class="am-card-v3"><h3>&#128737; Security Settings</h3>
     <div class="am-radio-block"><asp:CheckBox ID="WelcomeEmail" runat="server" Checked="true" Text="Send welcome email when resending invitation" /><span class="am-hint">The employee will receive a secure email with instructions to set their password.</span></div>
     <div class="am-radio-block"><asp:CheckBox ID="RequireChange" runat="server" Text="Require password change at next login" /> <span class="am-help" title="The employee must set a new password the next time they sign in.">i</span></div>
     <div class="am-radio-block"><asp:CheckBox ID="TwoFactor" runat="server" Text="Enable two-factor authentication (2FA)" /> <span class="am-help" title="Stored as a setting. The sign-in page does not ask for a second code yet.">i</span></div></div>
    <div class="am-card-v3"><h3>&#128339; Access History</h3><div class="am-table-wrap"><table class="am-data"><thead><tr><th>Date</th><th>Action</th><th>Changed By</th><th>Notes</th></tr></thead><tbody>
     <asp:Repeater ID="AccessRows" runat="server"><ItemTemplate><tr><td><%#: Eval("WhenText") %></td><td><%#: Eval("ActionText") %></td><td><%#: Eval("ChangedBy") %></td><td><%#: Eval("AfterValue") %></td></tr></ItemTemplate></asp:Repeater>
     <asp:PlaceHolder ID="NoAccessRows" runat="server"><tr><td>&mdash;</td><td>&mdash;</td><td>&mdash;</td><td>No login yet.</td></tr></asp:PlaceHolder>
    </tbody></table></div></div>
   </div>
  </asp:Panel>
 </div>
 <footer class="am-dialog-foot" id="Footer" runat="server">
  <asp:LinkButton ID="Deactivate" runat="server" Visible="false" OnClick="Deactivate_Click" CausesValidation="false" CssClass="am-button am-outline-danger" style="margin-right:auto" OnClientClick="return confirm('Deactivate this employee in all your businesses?');">&#128465; Deactivate Employee</asp:LinkButton>
  <asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="~/Users.aspx" CssClass="am-button secondary">Cancel</asp:HyperLink>
  <asp:LinkButton ID="SaveEmployee" runat="server" OnClick="Save_Click" CssClass="am-button">Save Employee</asp:LinkButton>
  <asp:LinkButton ID="SaveAccess" runat="server" Visible="false" OnClick="SaveAccess_Click" CausesValidation="false" CssClass="am-button">Save Access Settings</asp:LinkButton>
 </footer>
</section>

<%-- Add mode: Add Business Assignment dialog (same fields as the assignment dialog, slides 5-6) --%>
<asp:Panel ID="AssignmentModal" runat="server" Visible="false" CssClass="am-modal-shade" role="dialog" aria-modal="true" aria-labelledby="am-new-assign-title" DefaultButton="AddPending">
 <div class="am-dialog-page">
  <header class="am-dialog-head"><div><h1 id="am-new-assign-title">Add Business Assignment</h1><p>Assign this employee to a business and set roles and pay.</p></div><asp:LinkButton runat="server" OnClick="CloseAssignment_Click" CausesValidation="false" CssClass="am-dialog-close" ToolTip="Close">&times;</asp:LinkButton></header>
  <div class="am-dialog-body">
   <asp:Label ID="ModalError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
   <div class="am-section"><h2>Assignment Details</h2><div class="am-grid am-cols-2">
    <label class="am-field"><span>Business <span class="am-req">*</span></span><asp:DropDownList ID="NewBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="NewBusinessChanged" /></label>
    <div class="am-field"><span>Assign Roles <span class="am-req">*</span></span><asp:CheckBoxList ID="NewRoles" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="am-check-row" /></div>
    <label class="am-field"><span>Employee Type <span class="am-req">*</span></span><asp:DropDownList ID="NewType" runat="server"><asp:ListItem>Regular</asp:ListItem><asp:ListItem>Project</asp:ListItem></asp:DropDownList></label>
    <label class="am-field"><span>Pay Type <span class="am-req">*</span></span><asp:DropDownList ID="NewPayType" runat="server" /></label>
    <label class="am-field"><span>Pay Rate <span class="am-req">*</span></span><span class="am-money"><asp:TextBox ID="NewRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Billing Rate <span class="am-hint">(Optional)</span></span><span class="am-money"><asp:TextBox ID="NewBilling" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Effective Date <span class="am-req">*</span></span><asp:TextBox ID="NewEffective" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>End Date <span class="am-hint">(Optional)</span></span><asp:TextBox ID="NewEnd" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>Assignment Status <span class="am-req">*</span></span><asp:DropDownList ID="NewStatus" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Not Active</asp:ListItem><asp:ListItem>Terminated</asp:ListItem></asp:DropDownList></label>
    <label class="am-field"><span>Default Project</span><asp:DropDownList ID="NewProject" runat="server" /></label>
    <label class="am-field am-span-2"><span>Notes</span><asp:TextBox ID="NewNotes" runat="server" TextMode="MultiLine" MaxLength="500" ClientIDMode="Static" placeholder="Add any notes about this assignment..." /><span class="am-counter"><span id="am-notes-count">0</span>/500</span></label>
   </div></div>
  </div>
  <footer class="am-dialog-foot"><asp:LinkButton runat="server" OnClick="CloseAssignment_Click" CausesValidation="false" CssClass="am-button secondary">Cancel</asp:LinkButton><asp:LinkButton ID="AddPending" runat="server" OnClick="AddPending_Click" CausesValidation="false" CssClass="am-button">+ Add Assignment</asp:LinkButton></footer>
 </div>
</asp:Panel>
<script>(function(){
function c(i,o,m,left){var n=document.getElementById(i),s=document.getElementById(o);if(!n||!s)return;function u(){if(n.value.length>m)n.value=n.value.slice(0,m);s.textContent=left?(m-n.value.length):n.value.length;}n.addEventListener('input',u);u();}
c('RateReason','am-reason-left',200,true);c('NewNotes','am-notes-count',500,false);
var l=document.getElementById('InviteLink');if(l){l.select();try{if(navigator.clipboard)navigator.clipboard.writeText(l.value);}catch(e){}}
var pill=document.getElementById('am-login-pill'),on=document.querySelector('.am-login-radios input[value="LoginOn"]'),off=document.querySelector('.am-login-radios input[value="LoginOff"]'),inv=document.querySelector('.am-invite-box input[type=checkbox]');
function p(){if(!pill||!on)return;var e=on.checked;pill.className='am-login-pill'+(e?'':' am-off');pill.textContent=(e?'\uD83D\uDD12 Login Access: Enabled':'\uD83D\uDD12 Login Access: Disabled');if(inv){inv.disabled=!e;}}
if(on){on.addEventListener('change',p);off.addEventListener('change',p);p();}
var f=document.querySelector('.am-employees-v2 .am-flash');if(f){f.setAttribute('tabindex','-1');f.focus();}
})();</script>
</asp:Content>
