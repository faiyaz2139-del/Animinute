<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportAddresses.aspx.cs" Inherits="AnyminuteSupportAddressesPage" Title="Address Verification" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261114v81" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261114v81" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page">
 <header class="sa-head"><div><h1>Address Verification</h1><p>Review, correct and verify manually entered addresses.</p></div>
  <div class="sa-head-btns"><a class="sa-btn" href="SupportAddresses.aspx?tab=verified&amp;add=1">&#43; Add Address</a><a class="sa-btn sa-btn-outline" href="SupportAddresses.aspx?tab=verified&amp;import=1">&#8593; Import Address</a><a class="sa-btn sa-btn-outline" href="SupportAddresses.aspx?tab=verified">&#9745; View Verified Addresses</a></div></header>
 <asp:Literal ID="Flash" runat="server" />
 <%-- v32: workflow strip removed (tabs + table stay). --%>
 <div class="sa-split">
  <div class="sa-card sa-main"><asp:Literal ID="Tabs" runat="server" /><asp:Literal ID="Filters" runat="server" /><asp:Literal ID="Table" runat="server" /></div>
  <asp:Literal ID="Panel" runat="server" />
 </div>
</section>
</asp:Content>
