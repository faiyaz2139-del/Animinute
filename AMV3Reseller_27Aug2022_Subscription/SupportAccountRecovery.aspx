<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportAccountRecovery.aspx.cs" Inherits="AnyminuteSupportAccountRecoveryPage" Title="Account Recovery" %>
<asp:Content ID="ArHead" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteLoginSecurity.css?v=20261031v39" rel="stylesheet" /></asp:Content>
<asp:Content ID="ArBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v39 (Change Login Email DOC s8): Support Admin starts a recovery; the user must still open the link sent to the new email.
     Support never overwrites a login email directly. Every action is written to EmailChangeAudit. --%>
<section class="am-ls">
 <div class="am-title"><div><h1>Account Recovery</h1><p>For users who cannot sign in or no longer have access to their login email.</p></div></div>
 <asp:Label ID="Notice" runat="server" Visible="false" CssClass="am-notice" role="status" />
 <section class="am-card">
  <h2>Start Login Email Recovery</h2>
  <p class="am-muted">Verify the user&rsquo;s identity first (for example: call back on the phone number on file and confirm employer, hire date and a recent pay period). AnyMinute sends a one-time verification link to the new email; the login email changes only after the user opens it. The current email is notified.</p>
  <div class="am-fields am-ls-fields">
   <label><span>Current Login Email <em aria-hidden="true">*</em></span><asp:TextBox ID="CurrentEmail" runat="server" MaxLength="50" required="required" /></label>
   <label><span>New Email Address <em aria-hidden="true">*</em></span><asp:TextBox ID="NewEmail" runat="server" TextMode="Email" MaxLength="50" required="required" /></label>
   <label><span>Identity verification performed <em aria-hidden="true">*</em></span><asp:TextBox ID="IdentityNote" runat="server" MaxLength="200" required="required" /></label>
  </div>
  <div class="am-actions am-ls-actions"><asp:Button ID="Start" runat="server" Text="Send Verification to New Email" OnClick="Start_Click" CssClass="am-button" /></div>
 </section>
</section>
</asp:Content>
