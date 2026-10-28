<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportPlans.aspx.cs" Inherits="AnyminuteSupportPlansPage" Title="Subscription Plans" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261028v25" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261028v25" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page">
 <header class="sa-head"><div><h1><asp:Literal ID="Title1" runat="server" /></h1><p><asp:Literal ID="Lead" runat="server" /></p></div>
  <a class="sa-btn" href="SupportPlans.aspx?add=1">+ Add Plan</a></header>
 <asp:Literal ID="Flash" runat="server" />
 <div class="sa-card"><asp:Literal ID="Table" runat="server" /></div>
 <asp:Literal ID="Editor" runat="server" />
 <asp:Literal ID="Modal" runat="server" />
</section>
</asp:Content>
