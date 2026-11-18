<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VerifyLoginEmail.aspx.cs" Inherits="AnyminuteVerifyLoginEmailPage" %>
<!DOCTYPE html>
<html lang="en"><head runat="server"><meta charset="utf-8" /><meta name="anyminute-build" content="v39-20261031" /><meta name="viewport" content="width=device-width,initial-scale=1" /><meta name="referrer" content="no-referrer" /><title>Verify Login Email | AnyMinute</title><link rel="icon" href="Assets/assets/img/logo.jpg" type="image/x-icon" /><link href="css/AnyminuteLoginSecurity.css?v=20261031v39" rel="stylesheet" /><link href="css/AnyminuteTypography.css?v=20261118v89" rel="stylesheet" /><link href="css/AnyminuteReference.css?v=20261118v89" rel="stylesheet" /><link href="css/AnyminuteFlat.css?v=20261118v89" rel="stylesheet" /></head>
<body class="am-reference am-typography am-ls-verify"><form id="VerifyForm" runat="server">
<%-- v39 (Change Login Email DOC step 3): one-time link from the verification email. Works signed in or signed out; the change happens on the Confirm click (POST), so mail scanners that prefetch the link cannot use it. --%>
<main class="am-ls-box"><img src="Assets/assets/img/logo.jpg" alt="AnyMinute" class="am-ls-logo" />
 <h1><asp:Literal ID="Heading" runat="server" /></h1><p><asp:Literal ID="Message" runat="server" /></p>
 <asp:Button ID="Confirm" runat="server" Text="Confirm New Login Email" OnClick="Confirm_Click" CssClass="am-ls-btn" />
 <asp:HyperLink ID="Action" runat="server" NavigateUrl="~/Login.aspx" CssClass="am-ls-btn" Text="Go to Sign In" Visible="false" /></main>
</form></body></html>
