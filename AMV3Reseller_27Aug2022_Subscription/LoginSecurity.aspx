<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="LoginSecurity.aspx.cs" Inherits="AnyminuteLoginSecurityPage" Title="Login &amp; Security" %>
<asp:Content ID="LsHead" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteLoginSecurity.css?v=20261031v39" rel="stylesheet" /></asp:Content>
<asp:Content ID="LsBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v39 (Change Login Email DOC): My Profile > Login & Security > Change Login Email. Self-service for every role; own login only. --%>
<section class="am-ls">
 <nav class="am-ls-crumbs" aria-label="Breadcrumb"><a href="MyProfile.aspx">My Profile</a> <span aria-hidden="true">&rsaquo;</span> <span>Login &amp; Security</span> <span aria-hidden="true">&rsaquo;</span> <span aria-current="page">Change Login Email</span></nav>
 <div class="am-title"><div><h1>Login &amp; Security</h1><p>Your sign-in details for every business you work with in AnyMinute.</p></div></div>
 <asp:Label ID="Notice" runat="server" Visible="false" CssClass="am-notice" role="status" />
 <section class="am-card">
  <h2>Current Login Information</h2>
  <dl class="am-ls-info">
   <div><dt>User Name</dt><dd><asp:Literal ID="UserName" runat="server" /></dd></div>
   <div><dt>Current Login Email</dt><dd><asp:Literal ID="CurrentEmail" runat="server" /></dd></div>
   <div><dt>Email Verification Status</dt><dd><asp:Literal ID="VerifyStatus" runat="server" /></dd></div>
  </dl>
  <asp:PlaceHolder ID="PendingView" runat="server" Visible="false"><div class="am-notice" role="status"><strong>Pending Email Verification.</strong> We sent a verification link to <strong><asp:Literal ID="PendingEmail" runat="server" /></strong> (valid until <asp:Literal ID="PendingUntil" runat="server" />). Your current login email keeps working until the link is opened.
   <asp:LinkButton ID="CancelPending" runat="server" CausesValidation="false" OnClick="CancelPending_Click" CssClass="am-ls-link">Cancel this request</asp:LinkButton></div></asp:PlaceHolder>
 </section>
 <section class="am-card">
  <h2>Change Login Email</h2>
  <p class="am-muted">Changing your login email will not affect your business assignments, roles, timesheets, or existing data.</p>
  <div class="am-fields am-ls-fields">
   <label><span>New Email Address <em aria-hidden="true">*</em></span><asp:TextBox ID="NewEmail" runat="server" TextMode="Email" MaxLength="50" autocomplete="email" required="required" /></label>
   <label><span>Confirm New Email Address <em aria-hidden="true">*</em></span><asp:TextBox ID="ConfirmEmail" runat="server" TextMode="Email" MaxLength="50" autocomplete="off" required="required" /></label>
   <label><span>Current Password <em aria-hidden="true">*</em></span><asp:TextBox ID="Password" runat="server" TextMode="Password" autocomplete="current-password" required="required" /></label>
  </div>
  <div class="am-actions am-ls-actions"><asp:Button ID="Send" runat="server" Text="Send Verification Email" OnClick="Send_Click" CssClass="am-button" /><a href="MyProfile.aspx" class="am-button secondary">Cancel</a></div>
  <p class="am-muted am-ls-small">The link is sent to the new address, expires in 24 hours and can be used once. We will let your current email know once the change is complete. Business owners and other subscribers cannot change your login email.</p>
  <p class="am-muted am-ls-small">Can&rsquo;t access your account or your current email? Contact AnyMinute Support for secure Account Recovery.</p>
 </section>
 <asp:PlaceHolder ID="HistoryView" runat="server" Visible="false"><section class="am-card"><h2>Security Activity</h2><div class="am-table-wrap"><table class="am-table"><thead><tr><th>Date (UTC)</th><th>From</th><th>To</th><th>Method</th><th>Result</th></tr></thead><tbody><asp:Literal ID="HistoryRows" runat="server" /></tbody></table></div></section></asp:PlaceHolder>
</section>
</asp:Content>
