<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="AccountantAccess.aspx.cs" Inherits="AnyminuteAccountantAccessPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261025v13" rel="stylesheet" /><link href="css/AnyminuteAccess.css?v=20261024v9" rel="stylesheet" /><script src="Scripts/AnyminuteProjects.js?v=20261024v8" defer></script><script src="Scripts/AnyminuteAccess.js?v=20261024v9" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-projects am-access">
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
 <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-pj-info">Invitations, firm names, access types, permissions and expiry dates need the database update DB_REQUIRED/REQUIRED_04_AccountantAccess.sql. Existing accountants are listed below.</asp:Label>
 <header class="am-pj-heading"><div><h1>Accountant &amp; Payroll Access</h1><p>Manage external accountants and payroll administrators who can access your businesses.</p></div><asp:HyperLink ID="InviteLink" runat="server" NavigateUrl="~/AccountantInvite.aspx" CssClass="am-pj-btn am-pj-primary am-ac-invite"><span aria-hidden="true" class="am-pj-plus">+</span> Invite Accountant &amp; Payroll Admin</asp:HyperLink></header>
 <div class="am-pj-metrics">
  <div class="am-pj-metric am-pj-m-blue"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M16 11a4 4 0 1 0-3.9-5A4 4 0 0 1 16 11zM8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-7 1.5-7 4.5V20h14v-2.5C15 14.5 11 13 8 13zm8 0c-.3 0-.7 0-1.1.1A5 5 0 0 1 17 17.5V20h6v-2.5c0-3-4-4.5-7-4.5z"/></svg></span><div>Total Authorized Users<strong><asp:Literal ID="TotalCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-green"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="10" fill="currentColor"/><path d="M7 12.5l3.2 3.2L17 9" fill="none" stroke="#fff" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"/></svg></span><div>Active Users<strong><asp:Literal ID="ActiveCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-amber"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="10" fill="currentColor"/><path d="M12 6.5V12l3.5 2.5" fill="none" stroke="#fff" stroke-width="2.2" stroke-linecap="round"/></svg></span><div>Pending Invitations<strong><asp:Literal ID="PendingCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-purple"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M3 21V5l8-3v19H3zm10 0V8l8 3v10h-8zM5 7v2h2V7H5zm0 4v2h2v-2H5zm0 4v2h2v-2H5zm3-8v2h1V7H8zm0 4v2h1v-2H8zm0 4v2h1v-2H8zm7-2v2h2v-2h-2zm0 4v2h2v-2h-2zm3-4v2h1v-2h-1zm0 4v2h1v-2h-1z"/></svg></span><div>Businesses<strong><asp:Literal ID="BusinessCount" runat="server" Text="0" /></strong></div></div>
 </div>
 <div class="am-ac-filters">
  <label><span>Business</span><select data-ac-filter="biz" aria-label="Business"><option value="">All Businesses</option><asp:Literal ID="BusinessOptions" runat="server" /></select></label>
  <label><span>Access Type</span><select data-ac-filter="type" aria-label="Access type"><option value="">All Access Types</option><option value="accountant">Accountant</option><option value="payroll">Payroll Administrator</option></select></label>
  <label><span>Status</span><select data-ac-filter="status" aria-label="Status"><option value="">All Statuses</option><option>Active</option><option>Invited</option><option>Expired</option><option>Revoked</option></select></label>
  <label class="am-pj-search am-ac-search"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search by name, firm name, email..." data-ac-search="1" aria-label="Search by name, firm name, email" /></label>
 </div>
 <div class="am-pj-card">
  <div class="am-pj-table-wrap">
   <table class="am-pj-table am-ac-table" id="AcTable">
    <thead><tr><th>Name</th><th>Firm Name</th><th>Email</th><th>Business(es)</th><th>Access Type</th><th>Status</th><th>Invited Date</th><th>Expiry Date</th><th class="am-c">Action</th></tr></thead>
    <tbody>
    <asp:Repeater ID="Rows" runat="server" OnItemCommand="Rows_ItemCommand"><ItemTemplate>
     <tr class="am-ac-row" data-ac-text="<%#: Eval("Name") %> <%#: Eval("Firm") %> <%#: Eval("Email") %>" data-biz=",<%# String.Join(",", ((AnyminuteAccessRow)Container.DataItem).BusinessIds) %>," data-type="<%# Eval("TypeKey") %>" data-status="<%# Eval("Status") %>">
      <td><span class="am-ac-person"><span class="am-ac-avatar am-ac-tone<%# Eval("Tone") %>"><%#: Eval("Initials") %></span><strong><%#: Eval("Name") %></strong></span></td>
      <td><%#: Eval("FirmText") %></td>
      <td class="am-ac-email"><%#: Eval("Email") %></td>
      <td><%#: Eval("BusinessText") %></td>
      <td class="am-ac-types"><asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("Accountant") %>'><span class="am-ac-badge am-ac-acc">Accountant</span></asp:PlaceHolder><asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("Payroll") %>'><span class="am-ac-badge am-ac-pay">Payroll Administrator</span></asp:PlaceHolder></td>
      <td><span class="am-pj-pill am-ac-st-<%# Convert.ToString(Eval("Status")).ToLowerInvariant() %>"><%# Eval("Status") %></span></td>
      <td><%# Eval("InvitedText") %></td>
      <td><%# Eval("ExpiryText") %></td>
      <td class="am-c am-pj-actions">
       <asp:PlaceHolder runat="server" Visible='<%# Live(Eval("Status")) %>'>
       <a class="am-pj-ic" title="Edit access" href="<%# ResolveUrl("~/AccountantInvite.aspx?" + Eval("Key")) %>"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 21h4L18.5 9.5l-4-4L3 17z" fill="currentColor"/><path d="M15.5 4.5l4 4 1.5-1.5-4-4z" fill="currentColor"/></svg><span class="am-visually-hidden">Edit</span></a>
       <asp:LinkButton runat="server" CssClass="am-pj-ic am-pj-red" CommandName="Revoke" CommandArgument='<%# Eval("Key") %>' CausesValidation="false" ToolTip="Revoke access" OnClientClick="return confirm('Revoke this person\'s access to your businesses? Their existing records are kept.');"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg><span class="am-visually-hidden">Revoke</span></asp:LinkButton>
       <span class="am-pj-more"><button type="button" class="am-pj-ic am-pj-kebab" aria-haspopup="true" aria-expanded="false" title="More actions"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="5" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="19" r="2" fill="currentColor"/></svg><span class="am-visually-hidden">More</span></button>
        <span class="am-pj-menu" role="menu">
         <a role="menuitem" href="<%# ResolveUrl("~/AccountantInvite.aspx?" + Eval("Key")) %>"><svg viewBox="0 0 24 24"><path d="M4 20h4L19 9l-4-4L4 16z" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Edit Access</a>
         <asp:LinkButton runat="server" role="menuitem" CommandName="Resend" CommandArgument='<%# Eval("InviteId") %>' CausesValidation="false" Visible='<%# Convert.ToString(Eval("Status")) == "Invited" %>'><svg viewBox="0 0 24 24"><path d="M3 5h18v14H3z" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M3 6l9 7 9-7" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Resend Invitation</asp:LinkButton>
         <span class="am-pj-menu-sep"></span>
         <asp:LinkButton runat="server" role="menuitem" CssClass="am-pj-danger" CommandName="Revoke" CommandArgument='<%# Eval("Key") %>' CausesValidation="false" OnClientClick="return confirm('Revoke this person\'s access to your businesses? Their existing records are kept.');"><svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M6 6l12 12" stroke="currentColor" stroke-width="1.8"/></svg>Revoke Access</asp:LinkButton>
        </span></span>
       </asp:PlaceHolder>
       <asp:PlaceHolder runat="server" Visible='<%# !Live(Eval("Status")) %>'><span class="am-pj-muted">&#8212;</span></asp:PlaceHolder>
      </td></tr>
    </ItemTemplate></asp:Repeater>
    <tr class="am-ac-none" hidden="hidden"><td colspan="9" class="am-pj-empty">No users match the current filters.</td></tr>
    </tbody>
   </table>
  </div>
 </div>
 <p class="am-ac-showing" data-ac-showing="1"><asp:Literal ID="Showing" runat="server" /></p>
</section>
</asp:Content>
