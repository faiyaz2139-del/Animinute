<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportReports.aspx.cs" Inherits="AnyminuteSupportReportsPage" Title="Support Admin Reports" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261029v31" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261029v31" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page sa-rpt">
 <asp:Literal ID="Nav" runat="server" />
 <div class="sa-rpt-main"><asp:Literal ID="Body1" runat="server" /></div>
</section>
</asp:Content>
