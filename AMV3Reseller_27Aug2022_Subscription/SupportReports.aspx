<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportReports.aspx.cs" Inherits="AnyminuteSupportReportsPage" Title="Support Admin Reports" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261101v46" rel="stylesheet" /><link href="css/AnyminuteReportLayout.css?v=20261101v46" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261101v46" defer="defer"></script><script src="Scripts/AnyminuteReportLayout.js?v=20261101v46" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page am-rl">
 <asp:Literal ID="Head1" runat="server" />
 <div class="am-rl-body"><asp:Literal ID="Nav" runat="server" />
 <div class="am-rl-main"><asp:Literal ID="Body1" runat="server" /></div></div>
</section>
</asp:Content>
