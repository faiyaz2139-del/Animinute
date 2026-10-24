<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeEditor.aspx.cs" Inherits="AnyminuteEmployeeEditorPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-dialog-page am-employees-v2">
 <header class="am-dialog-head"><div><h1><asp:Literal ID="PageTitle" runat="server" /></h1><p><asp:Literal ID="PageSubtitle" runat="server" /></p></div><asp:HyperLink ID="CloseLink" runat="server" NavigateUrl="~/Users.aspx" CssClass="am-dialog-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <div class="am-dialog-body">
  <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <asp:Panel ID="HeaderCard" runat="server" Visible="false" CssClass="am-info-card"><span class="am-avatar"><asp:Literal ID="HeadInitials" runat="server" /></span><div class="am-info-main"><strong><asp:Literal ID="HeadName" runat="server" /></strong> <asp:Label ID="HeadStatus" runat="server" /><div class="am-info-meta"><span>&#9993; <asp:Literal ID="HeadEmail" runat="server" /></span><span>&#9742; <asp:Literal ID="HeadPhone" runat="server" /></span><span>ID: <asp:Literal ID="HeadId" runat="server" /></span></div></div><div class="am-access-box"><span>Login Access</span><strong><asp:Literal ID="HeadAccess" runat="server" /></strong><asp:HyperLink ID="HeadAccessLink" runat="server">Manage</asp:HyperLink></div></asp:Panel>
  <asp:Panel ID="Tabs" runat="server" Visible="false" CssClass="am-tabs" role="tablist"><asp:Literal ID="TabLinks" runat="server" /></asp:Panel>

  <asp:Panel ID="InfoSection" runat="server" CssClass="am-section"><h2>Employee Information</h2><div class="am-grid am-cols-3">
   <label class="am-field"><span>First Name <span class="am-req">*</span></span><asp:TextBox ID="FirstName" runat="server" MaxLength="50" /></label>
   <label class="am-field"><span>Middle Name</span><asp:TextBox ID="MiddleName" runat="server" MaxLength="50" /></label>
   <label class="am-field"><span>Last Name <span class="am-req">*</span></span><asp:TextBox ID="LastName" runat="server" MaxLength="50" /></label>
   <label class="am-field"><span>Email</span><asp:TextBox ID="Email" runat="server" TextMode="Email" MaxLength="100" /></label>
   <label class="am-field"><span>Contact Phone <span class="am-req">*</span></span><asp:TextBox ID="Phone" runat="server" TextMode="Phone" MaxLength="20" /></label>
   <label class="am-field"><span>Mobile Phone</span><asp:TextBox ID="Mobile" runat="server" TextMode="Phone" MaxLength="20" /></label>
   <label class="am-field"><span>Date of Birth</span><asp:TextBox ID="DateOfBirth" runat="server" TextMode="Date" /></label>
   <label class="am-field"><span>Gender</span><asp:DropDownList ID="Gender" runat="server"><asp:ListItem Value="">Select gender</asp:ListItem><asp:ListItem>Male</asp:ListItem><asp:ListItem>Female</asp:ListItem><asp:ListItem>Other</asp:ListItem></asp:DropDownList></label>
   <label class="am-field"><span>Hire Date <span class="am-req">*</span></span><asp:TextBox ID="HireDate" runat="server" TextMode="Date" /></label>
   <label class="am-field"><span>SIN / SSN</span><asp:TextBox ID="Sin" runat="server" MaxLength="20" autocomplete="off" /></label>
   <label class="am-field"><span>SIN Expiry</span><asp:TextBox ID="SinExpiry" runat="server" TextMode="Date" /></label>
   <label class="am-field"><span>Employee Status <span class="am-req">*</span></span><asp:DropDownList ID="Status" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Not Active</asp:ListItem><asp:ListItem>Terminated</asp:ListItem></asp:DropDownList></label>
  </div></asp:Panel>

  <asp:Panel ID="AddressSection" runat="server" CssClass="am-section"><h2>Address</h2><div class="am-grid am-cols-3">
   <label class="am-field"><span>Street No <span class="am-req">*</span></span><asp:TextBox ID="StreetNo" runat="server" MaxLength="20" /></label>
   <label class="am-field am-span-2"><span>Address Line 1 <span class="am-req">*</span></span><asp:TextBox ID="Line1" runat="server" MaxLength="200" /></label>
   <label class="am-field am-span-2"><span>Address Line 2</span><asp:TextBox ID="Line2" runat="server" MaxLength="200" /></label>
   <label class="am-field"><span>Suite Type</span><asp:DropDownList ID="SuiteType" runat="server"><asp:ListItem Value="">None</asp:ListItem><asp:ListItem>Suite</asp:ListItem><asp:ListItem>Apt</asp:ListItem><asp:ListItem>Unit</asp:ListItem></asp:DropDownList></label>
   <label class="am-field"><span>Suite / Unit No</span><asp:TextBox ID="SuiteNumber" runat="server" MaxLength="20" /></label>
   <label class="am-field"><span>City <span class="am-req">*</span></span><asp:TextBox ID="City" runat="server" MaxLength="100" /></label>
   <label class="am-field"><span>Province / State <span class="am-req">*</span></span><asp:TextBox ID="Province" runat="server" MaxLength="100" /></label>
   <label class="am-field"><span>Postal Code <span class="am-req">*</span></span><asp:TextBox ID="PostalCode" runat="server" MaxLength="20" /></label>
   <label class="am-field"><span>Country</span><asp:TextBox ID="Country" runat="server" MaxLength="100" Text="Canada" /></label>
  </div></asp:Panel>

  <%-- Add mode: assignments collected on the page and created together with the employee. --%>
  <asp:Panel ID="NewAssignments" runat="server" CssClass="am-section"><h2>Business Assignments <span class="am-req">*</span></h2>
   <div class="am-table-wrap"><table class="am-data"><thead><tr><th>Business</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Effective Date</th><th>Status</th><th>Action</th></tr></thead><tbody>
   <asp:Repeater ID="Pending" runat="server" OnItemCommand="Pending_ItemCommand"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><%#: Eval("RoleText") %></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("RateText") %></td><td><%#: Eval("EffectiveText") %></td><td><%#: Eval("Status") %></td><td><asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Container.ItemIndex %>' CausesValidation="false" CssClass="am-act am-act-del" ToolTip="Remove">&#128465;</asp:LinkButton></td></tr></ItemTemplate></asp:Repeater>
   <asp:PlaceHolder ID="NoPending" runat="server"><tr><td colspan="8" class="am-hint">No business assigned yet. Add at least one below.</td></tr></asp:PlaceHolder>
   </tbody></table></div>
   <div class="am-grid am-cols-3" style="margin-top:14px">
    <label class="am-field"><span>Business</span><asp:DropDownList ID="NewBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="NewBusinessChanged" /></label>
    <div class="am-field am-span-2"><span>Roles</span><asp:CheckBoxList ID="NewRoles" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="am-check-row" /></div>
    <label class="am-field"><span>Employee Type</span><asp:DropDownList ID="NewType" runat="server"><asp:ListItem>Regular</asp:ListItem><asp:ListItem>Project</asp:ListItem></asp:DropDownList></label>
    <label class="am-field"><span>Pay Type</span><asp:DropDownList ID="NewPayType" runat="server" /></label>
    <label class="am-field"><span>Pay Rate</span><span class="am-money"><asp:TextBox ID="NewRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Effective Date</span><asp:TextBox ID="NewEffective" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>Default Project</span><asp:DropDownList ID="NewProject" runat="server" /></label>
    <div class="am-field"><span>&nbsp;</span><asp:LinkButton ID="AddPending" runat="server" OnClick="AddPending_Click" CausesValidation="false" CssClass="am-button secondary">+ Add Business Assignment</asp:LinkButton></div>
   </div>
  </asp:Panel>

  <asp:Panel ID="NewAccess" runat="server" CssClass="am-section"><h2>Login &amp; Access</h2>
   <div class="am-check-row"><asp:RadioButton ID="LoginOn" runat="server" GroupName="login" Checked="true" Text="Enable login access" /><asp:RadioButton ID="LoginOff" runat="server" GroupName="login" Text="Disable login access" /></div>
   <div class="am-check-row"><asp:CheckBox ID="SendInvite" runat="server" Checked="true" Text="Send email invitation to set up login" /></div>
   <p class="am-hint">The invitation is sent only when you save, only if an email is entered, and only if email sending (IsSendMail) is turned on for this site.</p>
  </asp:Panel>

  <%-- Edit mode tabs --%>
  <asp:Panel ID="AssignmentsTab" runat="server" Visible="false" CssClass="am-section"><div class="am-dialog-head" style="padding:0 0 12px;border:0"><h2>Business Assignments</h2><asp:HyperLink ID="AddAssignmentLink" runat="server" CssClass="am-button">+ Add Business Assignment</asp:HyperLink></div>
   <div class="am-table-wrap"><table class="am-data"><thead><tr><th>Business</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Effective Date</th><th>Status</th><th>Action</th></tr></thead><tbody>
   <asp:Repeater ID="Assignments" runat="server"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><div class="am-role-chips"><asp:Repeater runat="server" DataSource='<%# Eval("Roles") %>'><ItemTemplate><span class='<%# "am-role-chip " + AnyminuteEmployeeEditor.RoleCss(Container.DataItem) %>'><%#: Container.DataItem %></span></ItemTemplate></asp:Repeater></div></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("PayRateText") %></td><td><%#: Eval("EffectiveText") %></td><td><span class='<%# AnyminuteEmployeeEditor.StatusPill(Eval("Status")) %>'><%#: Eval("Status") %></span></td><td><a class="am-act" title="Edit assignment" href='<%# "EmployeeAssignment.aspx?EmployeeId=" + Eval("EmployeeId") + "&BusinessId=" + Eval("BusinessId") %>'>&#9998;</a></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table></div></asp:Panel>

  <asp:Panel ID="RateTab" runat="server" Visible="false" CssClass="am-section">
   <div class="am-section"><h2>Current Pay Rate</h2><div class="am-grid am-cols-3">
    <label class="am-field" id="RateBusinessField" runat="server"><span>Business</span><asp:DropDownList ID="RateBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RateBusinessChanged" /></label>
    <label class="am-field"><span>Rate Type <span class="am-req">*</span></span><asp:DropDownList ID="RateType" runat="server" /></label>
    <label class="am-field"><span>Rate <span class="am-req">*</span></span><span class="am-money"><asp:TextBox ID="RateValue" runat="server" TextMode="Number" step="0.01" min="0" /></span></label>
    <label class="am-field"><span>Effective Date <span class="am-req">*</span></span><asp:TextBox ID="RateEffective" runat="server" TextMode="Date" /></label>
    <label class="am-field am-span-2"><span>Reason for Change <span class="am-req">*</span></span><asp:TextBox ID="RateReason" runat="server" MaxLength="200" ClientIDMode="Static" placeholder="e.g. Annual review" /><span class="am-counter"><span id="am-reason-count">0</span>/200</span></label>
   </div><div class="am-check-row" style="justify-content:flex-end"><asp:HyperLink ID="RateCancel" runat="server" CssClass="am-button secondary">Cancel</asp:HyperLink><asp:LinkButton ID="UpdateRate" runat="server" OnClick="UpdateRate_Click" CssClass="am-button">Update Rate</asp:LinkButton></div></div>
   <div class="am-section"><h2>Rate History</h2><div class="am-table-wrap"><table class="am-data"><thead><tr><th>#</th><th>Rate Type</th><th>Start Date</th><th>End Date</th><th>Rate</th><th>Reason</th><th>Status</th><th>Action</th></tr></thead><tbody>
    <asp:Repeater ID="RateRows" runat="server"><ItemTemplate><tr><td><%# Eval("No") %></td><td><%#: Eval("RateType") %></td><td><%#: Eval("StartText") %></td><td><%#: Eval("EndText") %></td><td><%#: Eval("RateText") %></td><td><%#: Eval("Reason") %></td><td><span class='<%# (bool)Eval("Current") ? "am-pill-green" : "am-pill-grey" %>'><%# (bool)Eval("Current") ? "Current" : "Historical" %></span></td><td><%# (bool)Eval("Current") ? "<a class=\"am-act\" href=\"#RateReason\" title=\"Edit the current rate above\">&#9998;</a>" : "<span class=\"am-hint\" title=\"Historical rates are locked\">&#128274; Locked</span>" %></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table></div>
   <p class="am-note">Changing the rate keeps history: the previous rate is closed the day before the new effective date and historical rates are locked so past time cards are not affected.</p></div>
  </asp:Panel>

  <asp:Panel ID="AccessTab" runat="server" Visible="false" CssClass="am-section">
   <div class="am-cards-3">
    <div class="am-info-card"><div class="am-info-main"><span class="am-hint">Access Status</span><strong><asp:Literal ID="AccessState" runat="server" /></strong><div class="am-info-meta"><span>Invitation sent: <asp:Literal ID="AccessSent" runat="server" /></span><span>Last change: <asp:Literal ID="AccessChanged" runat="server" /></span></div></div></div>
    <div class="am-info-card"><div class="am-info-main"><span class="am-hint">Account Information</span><strong><asp:Literal ID="AccessUser" runat="server" /></strong><div class="am-info-meta"><span>Role: <asp:Literal ID="AccessRole" runat="server" /></span></div></div></div>
    <div class="am-info-card"><div class="am-info-main"><span class="am-hint">Actions</span><div class="am-check-row"><asp:LinkButton ID="ResendInvite" runat="server" OnClick="Resend_Click" CausesValidation="false" CssClass="am-button secondary">Resend Invitation</asp:LinkButton><asp:LinkButton ID="CopyLink" runat="server" OnClick="CopyLink_Click" CausesValidation="false" CssClass="am-button secondary">Copy Link</asp:LinkButton><asp:LinkButton ID="CancelInvite" runat="server" OnClick="CancelInvite_Click" CausesValidation="false" CssClass="am-button am-outline-danger" OnClientClick="return confirm('Cancel the invitation and disable login access?');">Cancel Invitation</asp:LinkButton></div></div></div>
   </div>
   <asp:TextBox ID="InviteLink" runat="server" Visible="false" ReadOnly="true" ClientIDMode="Static" CssClass="am-invite-link" />
   <div class="am-section"><h2>Login Settings</h2><div class="am-check-row"><asp:RadioButton ID="AccessOn" runat="server" GroupName="access" Text="Enable login access" /><asp:RadioButton ID="AccessOff" runat="server" GroupName="access" Text="Disable login access" /></div></div>
   <div class="am-section"><h2>Security</h2>
    <div class="am-check-row"><asp:CheckBox ID="WelcomeEmail" runat="server" Text="Send welcome email when access is enabled" /></div>
    <div class="am-check-row"><asp:CheckBox ID="RequireChange" runat="server" Text="Require password change at next login" /></div>
    <div class="am-check-row"><asp:CheckBox ID="TwoFactor" runat="server" Text="Enable two-factor authentication" /></div>
    <p class="am-hint">Two-factor authentication is stored as a setting; the login page does not yet ask for a second factor.</p>
    <div class="am-check-row" style="justify-content:flex-end"><asp:LinkButton ID="SaveAccess" runat="server" OnClick="SaveAccess_Click" CausesValidation="false" CssClass="am-button">Save Access Settings</asp:LinkButton></div></div>
   <div class="am-section"><h2>Access History</h2><div class="am-table-wrap"><table class="am-data"><thead><tr><th>Date</th><th>Action</th><th>Details</th><th>Changed By</th></tr></thead><tbody>
    <asp:Repeater ID="AccessRows" runat="server"><ItemTemplate><tr><td><%#: Eval("WhenText") %></td><td><%#: Eval("Action") %></td><td><%#: Eval("AfterValue") %></td><td><%#: Eval("ChangedBy") %></td></tr></ItemTemplate></asp:Repeater>
    <asp:PlaceHolder ID="NoAccessRows" runat="server"><tr><td colspan="4" class="am-hint">No access changes recorded yet.</td></tr></asp:PlaceHolder>
   </tbody></table></div></div>
  </asp:Panel>
 </div>
 <footer class="am-dialog-foot" id="Footer" runat="server">
  <asp:LinkButton ID="Deactivate" runat="server" Visible="false" OnClick="Deactivate_Click" CausesValidation="false" CssClass="am-button am-danger" style="margin-right:auto" OnClientClick="return confirm('Deactivate this employee in all your businesses?');">Deactivate Employee</asp:LinkButton>
  <asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="~/Users.aspx" CssClass="am-button secondary">Cancel</asp:HyperLink>
  <asp:LinkButton ID="SaveEmployee" runat="server" OnClick="Save_Click" CssClass="am-button">Save Employee</asp:LinkButton>
 </footer>
</section>
<script>(function(){function c(i,o,m){var n=document.getElementById(i),s=document.getElementById(o);if(!n||!s)return;function u(){if(n.value.length>m)n.value=n.value.slice(0,m);s.textContent=n.value.length;}n.addEventListener('input',u);u();}c('RateReason','am-reason-count',200);var l=document.getElementById('InviteLink');if(l){l.select();try{if(navigator.clipboard)navigator.clipboard.writeText(l.value);}catch(e){}}var f=document.querySelector('.am-employees-v2 .am-flash');if(f){f.setAttribute('tabindex','-1');f.focus();}})();</script>
</asp:Content>
