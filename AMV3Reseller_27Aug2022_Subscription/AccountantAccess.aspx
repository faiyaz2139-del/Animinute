<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="AccountantAccess.aspx.cs" Inherits="AnyminuteAccountantAccessPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261025v14" rel="stylesheet" /><link href="css/AnyminuteAccess.css?v=20261105v65" rel="stylesheet" /><script src="Scripts/AnyminuteProjects.js?v=20261024v8" defer></script><script src="Scripts/AnyminuteAccess.js?v=20261105v65" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v65 (PPT s5 strict, mock image5): Accountant & Payroll Access = page header, Invite card (3 columns + note/Clear/Send), Invited & Active Users card (tabs + search + filters + 6-column table). Old metric cards / 9-column table / Assign Accountant UI removed. --%>
<section class="am-projects am-access am-acx" data-build="v65-20261105">
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
 <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-pj-info">Invitations, firm names, roles, permissions and expiry dates need the database update DB_REQUIRED/REQUIRED_04_AccountantAccess.sql. Existing accountants are listed below.</asp:Label>
 <header class="am-acx-top">
  <span class="am-acx-topicon" aria-hidden="true"><svg viewBox="0 0 24 24"><path fill="currentColor" d="M16 11a4 4 0 1 0-3.9-5A4 4 0 0 1 16 11zM8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-7 1.5-7 4.5V20h14v-2.5C15 14.5 11 13 8 13zm8 0c-.3 0-.7 0-1.1.1A5 5 0 0 1 17 17.5V20h6v-2.5c0-3-4-4.5-7-4.5z"/></svg></span>
  <div class="am-acx-toptext"><h1>Accountant &amp; Payroll Access</h1><p>Invite external professionals and manage their access to your businesses.</p></div>
 </header>

 <div class="am-acx-card" id="am-ac-inviteform">
  <h2 class="am-acx-title">Invite Accountant or Payroll Administrator</h2>
  <asp:Label ID="InvError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <div class="am-acx-cols">
   <div class="am-acx-col">
    <div class="am-acx-colhead"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 12a5 5 0 1 0 0-10 5 5 0 0 0 0 10zm0 2c-4 0-9 2-9 6v2h18v-2c0-4-5-6-9-6z"/></svg><h3>User Information</h3></div>
    <div class="am-acx-colbody">
     <label class="am-acx-field"><span>Full Name <em class="am-acx-req">*</em></span><asp:TextBox ID="InvFullName" runat="server" MaxLength="150" placeholder="Enter full name" /></label>
     <label class="am-acx-field"><span>Firm Name <small>(optional)</small></span><asp:TextBox ID="InvFirmName" runat="server" MaxLength="150" placeholder="Enter firm name" /></label>
     <label class="am-acx-field"><span>Email Address <em class="am-acx-req">*</em></span><asp:TextBox ID="InvEmail" runat="server" MaxLength="200" TextMode="Email" placeholder="Enter email address" /></label>
     <label class="am-acx-field"><span>Phone Number <small>(optional)</small></span><asp:TextBox ID="InvPhone" runat="server" MaxLength="40" TextMode="Phone" placeholder="Enter phone number" /></label>
    </div>
   </div>
   <div class="am-acx-col">
    <div class="am-acx-colhead"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M6 10V8a6 6 0 1 1 12 0v2h1a1 1 0 0 1 1 1v10a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V11a1 1 0 0 1 1-1h1zm2 0h8V8a4 4 0 1 0-8 0v2z"/></svg><h3>Access &amp; Permissions</h3></div>
    <div class="am-acx-colbody">
     <div class="am-acx-hint">Select one or both roles.</div>
     <label class="am-acx-role"><asp:CheckBox ID="InvTypeAccountant" runat="server" /><span><strong>Accountant</strong><small>Can view reports, time cards, payroll summaries and accounting exports.</small></span></label>
     <label class="am-acx-role"><asp:CheckBox ID="InvTypePayroll" runat="server" /><span><strong>Payroll Administrator</strong><small>Can access payroll information, prepare payroll and manage payroll processing.</small></span></label>
     <div class="am-acx-sub">Additional Permissions</div>
     <div class="am-acx-perms">
      <label><asp:CheckBox ID="InvPermPayRates" runat="server" /> View employee pay rates</label>
      <label><asp:CheckBox ID="InvPermApprove" runat="server" /> Approve payroll</label>
      <label><asp:CheckBox ID="InvPermExport" runat="server" /> Export reports</label>
      <label><asp:CheckBox ID="InvPermManage" runat="server" /> Manage other users' access</label>
     </div>
    </div>
   </div>
   <div class="am-acx-col">
    <div class="am-acx-colhead"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M3 21V5l8-3v19H3zm10 0V8l8 3v10h-8zM5 7v2h2V7H5zm0 4v2h2v-2H5zm0 4v2h2v-2H5zm10-2v2h2v-2h-2zm0 4v2h2v-2h-2z"/></svg><h3>Business Access</h3></div>
    <div class="am-acx-colbody">
     <div class="am-acx-label">Select Business(es) <em class="am-acx-req">*</em></div>
     <label class="am-acx-search"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search businesses..." data-ac-bizsearch="1" aria-label="Search businesses" /></label>
     <div class="am-acx-bizlist"><asp:Repeater ID="InvBusinesses" runat="server"><ItemTemplate><label class="am-ac-biz" data-ac-text="<%#: Eval("Value") %>"><input type="checkbox" name="biz" value="<%# Eval("Key") %>"<%# Picked((int)Eval("Key")) ? " checked=\"checked\"" : "" %> /> <%#: Eval("Value") %></label></ItemTemplate></asp:Repeater></div>
     <label class="am-acx-field am-acx-expiry"><span>Access Expiry <small>(optional)</small></span><asp:TextBox ID="InvExpiry" runat="server" TextMode="Date" /></label>
    </div>
   </div>
  </div>
  <div class="am-acx-foot">
   <span class="am-acx-note"><span aria-hidden="true">&#9432;</span> Access starts only after the invitation is accepted.</span>
   <a href="AccountantAccess.aspx" class="am-acx-btn am-acx-outline">Clear form</a>
   <asp:Button ID="InvSend" runat="server" Text="Send Invitation" OnClick="InvSend_Click" CssClass="am-acx-btn am-acx-primary" />
  </div>
 </div>

 <div class="am-acx-card">
  <h2 class="am-acx-title">Invited &amp; Active Users</h2>
  <div class="am-acx-bar">
   <div class="am-acx-tabs" role="tablist">
    <button type="button" role="tab" class="is-on" data-ac-tab="">All (<asp:Literal ID="TabAll" runat="server" Text="0" />)</button>
    <button type="button" role="tab" data-ac-tab="Invited">Pending (<asp:Literal ID="TabPending" runat="server" Text="0" />)</button>
    <button type="button" role="tab" data-ac-tab="Active">Active (<asp:Literal ID="TabActive" runat="server" Text="0" />)</button>
   </div>
   <input type="hidden" data-ac-filter="status" value="" />
   <label class="am-acx-search am-acx-listsearch"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search by name or email" data-ac-search="1" aria-label="Search by name or email" /></label>
   <select data-ac-filter="biz" aria-label="Business"><option value="">All businesses</option><asp:Literal ID="BusinessOptions" runat="server" /></select>
   <select data-ac-filter="type" aria-label="Role"><option value="">All roles</option><option value="accountant">Accountant</option><option value="payroll">Payroll Administrator</option></select>
  </div>
  <div class="am-acx-tablewrap">
   <table class="am-acx-table" id="AcTable">
    <thead><tr><th>Name / Email</th><th>Role</th><th>Business Access</th><th>Status</th><th>Invited On</th><th>Actions</th></tr></thead>
    <tbody>
    <asp:Repeater ID="Rows" runat="server" OnItemCommand="Rows_ItemCommand"><ItemTemplate>
     <tr class="am-ac-row" data-ac-text="<%#: Eval("Name") %> <%#: Eval("Firm") %> <%#: Eval("Email") %>" data-biz=",<%# String.Join(",", ((AnyminuteAccessRow)Container.DataItem).BusinessIds) %>," data-type="<%# Eval("TypeKey") %>" data-status="<%# Eval("Status") %>">
      <td><strong class="am-acx-name"><%#: Eval("Name") %></strong><span class="am-acx-mail"><%#: Eval("Email") %></span></td>
      <td><%#: RoleText((AnyminuteAccessRow)Container.DataItem) %></td>
      <td><%#: BizText((AnyminuteAccessRow)Container.DataItem) %></td>
      <td><span class="am-acx-pill am-acx-st-<%# Convert.ToString(Eval("Status")).ToLowerInvariant() %>"><%# StatusIcon(Eval("Status")) %><%#: StatusText(Eval("Status")) %></span></td>
      <td><%# Eval("InvitedText") %></td>
      <td class="am-acx-actions">
       <%-- v65 (PPT s6): Action = View button; everything else (Edit access, Resend, Cancel invitation, Remove / Delete) under the three-dot menu. No pencil/trash/PDF icons. --%>
       <a class="am-acx-btn am-acx-view" href="<%# ResolveUrl("~/AccountantInvite.aspx?" + Eval("Key") + "&view=1") %>">View</a>
       <asp:PlaceHolder runat="server" Visible='<%# Live(Eval("Status")) %>'>
       <span class="am-pj-more am-acx-more"><button type="button" class="am-pj-kebab am-acx-kebab" aria-haspopup="true" aria-expanded="false" title="More actions" aria-label="More actions">&#8942;</button>
        <span class="am-pj-menu am-acx-menu" role="menu">
         <a role="menuitem" href="<%# ResolveUrl("~/AccountantInvite.aspx?" + Eval("Key")) %>">Edit access</a>
         <asp:LinkButton runat="server" role="menuitem" CommandName="Resend" CommandArgument='<%# Eval("InviteId") %>' CausesValidation="false" Visible='<%# Convert.ToString(Eval("Status")) == "Invited" %>'>Resend invitation</asp:LinkButton>
         <asp:LinkButton runat="server" role="menuitem" CssClass="am-acx-danger" CommandName="Revoke" CommandArgument='<%# Eval("Key") %>' CausesValidation="false" Visible='<%# Convert.ToString(Eval("Status")) == "Invited" %>' OnClientClick="return confirm('Cancel this invitation?');">Cancel invitation</asp:LinkButton>
         <asp:LinkButton runat="server" role="menuitem" CssClass="am-acx-danger" CommandName="Revoke" CommandArgument='<%# Eval("Key") %>' CausesValidation="false" Visible='<%# Convert.ToString(Eval("Status")) == "Active" %>' OnClientClick="return confirm('Remove this person\'s access to your businesses? Their existing records are kept.');">Remove access</asp:LinkButton>
         <asp:LinkButton runat="server" role="menuitem" CssClass="am-acx-danger" CommandName="Revoke" CommandArgument='<%# Eval("Key") %>' CausesValidation="false" OnClientClick="return confirm('Delete this user\'s access? Their existing records are kept.');">Delete</asp:LinkButton>
        </span></span>
       </asp:PlaceHolder>
      </td></tr>
    </ItemTemplate></asp:Repeater>
    <tr class="am-ac-none" hidden="hidden"><td colspan="6" class="am-acx-empty">No users match the current filters.</td></tr>
    </tbody>
   </table>
  </div>
  <p class="am-acx-showing" data-ac-showing="1"><asp:Literal ID="Showing" runat="server" /></p>
 </div>
</section>
</asp:Content>
