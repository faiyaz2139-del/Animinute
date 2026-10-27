<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="LoginAccess.aspx.cs" Inherits="AnyminuteLoginAccessPage" Title="Login &amp; Access" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteReports.css?v=20261027v20" rel="stylesheet" /><script src="Scripts/AnyminuteReports.js?v=20261027v20" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-reports am-login-access">
 <header class="am-rp-head"><div><h1>Login &amp; Access</h1><p>See who can sign in to AnyMinute and open an employee&rsquo;s Login &amp; Access settings to enable, disable, invite or reset.</p></div>
  <div data-rp-form="1" class="am-rp-context" data-action="LoginAccess.aspx"><label><span>Business</span><select name="BusinessId" data-rp-autosubmit="1"><option value="">All my businesses</option><asp:Literal ID="BusinessOptions" runat="server" /></select></label></div></header>
 <div class="am-rp-kpis"><asp:Literal ID="Kpis" runat="server" /></div>
 <div class="am-rp-card"><div class="am-rp-card-head"><h2>Employees</h2><input type="search" placeholder="Search by name, email, business..." data-rp-search="1" data-rp-scope="#LoginAccessTable tbody tr" aria-label="Search employees" /></div>
  <div class="am-rp-table-wrap"><asp:Literal ID="Table" runat="server" /></div></div>
</section>
</asp:Content>
