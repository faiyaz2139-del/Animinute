<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="AccountantInvite.aspx.cs" Inherits="AnyminuteAccountantInvitePage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261025v13" rel="stylesheet" /><link href="css/AnyminuteAccess.css?v=20261024v9" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v9" defer></script><script src="Scripts/AnyminuteAccess.js?v=20261024v9" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-projects am-pj-dialog am-access am-ac-invite-page">
 <header class="am-pj-dhead"><div><h1><asp:Literal ID="PageTitle" runat="server" Text="Invite Accountant or Payroll Administrator" /></h1><p><asp:Literal ID="PageSubtitle" runat="server" Text="Send an invitation to an external accountant or payroll administrator." /></p></div><asp:HyperLink ID="CloseLink" runat="server" NavigateUrl="~/AccountantAccess.aspx" CssClass="am-pj-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <div class="am-pj-dbody">
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-pj-info">Run DB_REQUIRED/REQUIRED_04_AccountantAccess.sql before sending invitations.</asp:Label>
  <div class="am-ac-cols">
   <div class="am-ac-col">
    <div class="am-ac-head"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 12a5 5 0 1 0 0-10 5 5 0 0 0 0 10zm0 2c-4 0-9 2-9 6v2h18v-2c0-4-5-6-9-6z"/></svg><h2>User Information</h2></div>
    <div class="am-ac-body">
     <label class="am-pj-field"><span>Full Name <em class="am-pj-req">*</em></span><asp:TextBox ID="FullName" runat="server" MaxLength="150" placeholder="Enter full name" /></label>
     <label class="am-pj-field"><span>Firm Name <em class="am-pj-req">*</em></span><asp:TextBox ID="FirmName" runat="server" MaxLength="150" placeholder="Enter firm name" /></label>
     <label class="am-pj-field"><span>Email Address <em class="am-pj-req">*</em></span><asp:TextBox ID="Email" runat="server" MaxLength="200" TextMode="Email" placeholder="name@firm.com" /></label>
     <label class="am-pj-field"><span>Phone Number</span><asp:TextBox ID="Phone" runat="server" MaxLength="40" TextMode="Phone" placeholder="Enter phone number" /></label>
    </div>
   </div>
   <div class="am-ac-col">
    <div class="am-ac-head"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M6 10V8a6 6 0 1 1 12 0v2h1a1 1 0 0 1 1 1v10a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V11a1 1 0 0 1 1-1h1zm2 0h8V8a4 4 0 1 0-8 0v2z"/></svg><h2>Access &amp; Permissions</h2></div>
    <div class="am-ac-body">
     <div class="am-ac-label">Select Access Type <em class="am-pj-req">*</em></div>
     <label class="am-ac-type"><asp:CheckBox ID="TypeAccountant" runat="server" /><span><strong>Accountant</strong><small>Can view reports, time cards, payroll summaries and accounting exports.</small></span></label>
     <label class="am-ac-type"><asp:CheckBox ID="TypePayroll" runat="server" /><span><strong>Payroll Administrator</strong><small>Can access payroll information, prepare payroll and manage payroll processing.</small></span></label>
     <fieldset class="am-ac-perms"><legend>Additional Permissions (Optional)</legend>
      <label><asp:CheckBox ID="PermPayRates" runat="server" /> View employee pay rates</label>
      <label><asp:CheckBox ID="PermApprove" runat="server" /> Approve payroll</label>
      <label><asp:CheckBox ID="PermExport" runat="server" /> Export reports</label>
      <label><asp:CheckBox ID="PermManage" runat="server" /> Manage other users' access</label>
     </fieldset>
    </div>
   </div>
   <div class="am-ac-col">
    <div class="am-ac-head"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M3 21V5l8-3v19H3zm10 0V8l8 3v10h-8zM5 7v2h2V7H5zm0 4v2h2v-2H5zm0 4v2h2v-2H5zm10-2v2h2v-2h-2zm0 4v2h2v-2h-2z"/></svg><h2>Business Access</h2></div>
    <div class="am-ac-body">
     <div class="am-ac-label">Select Business(es) <em class="am-pj-req">*</em></div>
     <div class="am-ac-bizbox">
      <label class="am-pj-search am-ac-bizsearch"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search businesses..." data-ac-bizsearch="1" aria-label="Search businesses" /></label>
      <div class="am-ac-bizlist"><asp:Repeater ID="Businesses" runat="server"><ItemTemplate><label class="am-ac-biz" data-ac-text="<%#: Eval("Value") %>"><input type="checkbox" name="biz" value="<%# Eval("Key") %>"<%# Picked((int)Eval("Key")) ? " checked=\"checked\"" : "" %> /> <%#: Eval("Value") %></label></ItemTemplate></asp:Repeater></div>
     </div>
     <label class="am-pj-field am-ac-expiry am-datefmt"><span>Access Expiry (Optional)</span><asp:TextBox ID="Expiry" runat="server" TextMode="Date" data-placeholder="Select expiry date..." /></label>
    </div>
   </div>
  </div>
 </div>
 <footer class="am-pj-dfoot">
  <asp:LinkButton ID="RevokeButton" runat="server" Visible="false" CssClass="am-pj-btn am-pj-danger-outline am-pj-left" OnClick="Revoke_Click" CausesValidation="false" OnClientClick="return confirm('Revoke this person\'s access to your businesses? Their existing records are kept.');">Revoke Access</asp:LinkButton>
  <asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="~/AccountantAccess.aspx" CssClass="am-pj-btn am-pj-outline">Cancel</asp:HyperLink>
  <asp:Button ID="SaveButton" runat="server" Text="Send Invitation" OnClick="Save_Click" CssClass="am-pj-btn am-pj-primary" />
 </footer>
</section>
</asp:Content>
