<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="AnyminuteReportsPage" Title="Reports" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteReports.css?v=20261027v22" rel="stylesheet" /><script src="Scripts/AnyminuteReports.js?v=20261027v22" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-reports">
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-rp-error" role="alert" />
 <asp:Literal ID="HubHtml" runat="server" />
</section>
</asp:Content>
