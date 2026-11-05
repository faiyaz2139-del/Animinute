<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportTickets.aspx.cs" Inherits="AnyminuteSupportTicketsPage" Title="Support Tickets" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261105v66" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261105v66" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page">
 <header class="sa-head"><div><h1>Support Tickets</h1><p>View and manage support tickets from subscribers.</p></div>
  <a class="sa-btn" href="SupportTickets.aspx?create=1">+ Create Ticket</a></header>
 <asp:Literal ID="Flash" runat="server" />
 <div class="sa-kpis"><asp:Literal ID="Kpis" runat="server" /></div>
 <div class="sa-split">
  <div class="sa-card sa-main"><asp:Literal ID="Filters" runat="server" /><asp:Literal ID="Table" runat="server" /></div>
  <asp:Literal ID="Panel" runat="server" />
 </div>
</section>
</asp:Content>
