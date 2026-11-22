<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="UserLoginAccess.aspx.cs" Inherits="AnyminuteUserLoginAccessPage" Title="Login &amp; Access" %>
<asp:Content ID="LaHead" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteReports.css?v=20261122v94" rel="stylesheet" /><script src="Scripts/AnyminuteReports.js?v=20261122v94" defer="defer"></script></asp:Content>
<asp:Content ID="LaBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v38: was LoginAccess.aspx (live ErrorPages before any page code ran); clean page, unique control IDs. v27: roles without owner/admin access get this notice instead of the 403 error page. --%>
<%-- v90: chrome matches the Businesses list (am-title, am-stats, am-toolbar, am-table). --%>
<asp:PlaceHolder ID="DeniedView" runat="server" Visible="false"><section class="am-card am-login-access"><div class="am-title"><div><h1>Login &amp; Access</h1></div></div>
 <div class="am-notice am-error" role="alert"><strong>Access denied.</strong> Only business owners and administrators can manage login access. If you need an employee's login enabled, disabled or reset, ask the business owner. <a href="Dashboard.aspx">Back to Dashboard</a></div></section></asp:PlaceHolder>
<asp:PlaceHolder ID="AllowedView" runat="server">
<section class="am-card am-login-access">
 <div class="am-title"><div><h1>Login &amp; Access</h1><p>See who can sign in to AnyMinute and open an employee&rsquo;s Login &amp; Access settings to enable, disable, invite or reset.</p></div></div>
 <div class="am-stats" id="LoginKpisWrap"><asp:Literal ID="LoginKpis" runat="server" /></div>
 <div data-rp-form="1" class="am-toolbar" data-action="UserLoginAccess.aspx"><label><span>Business</span><select name="BusinessId" data-rp-autosubmit="1"><option value="">All my businesses</option><asp:Literal ID="BusinessOptions" runat="server" /></select></label><label><span>Search employees</span><input type="search" placeholder="Search by name, email, business..." data-rp-search="1" data-rp-scope="#LoginAccessTable tbody tr" aria-label="Search employees" /></label></div>
 <div class="am-table-wrap"><asp:Literal ID="LoginGrid" runat="server" /></div>
</section>
</asp:PlaceHolder>
</asp:Content>
