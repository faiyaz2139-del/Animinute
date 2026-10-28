<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportAddresses.aspx.cs" Inherits="AnyminuteSupportAddressesPage" Title="Address Verification" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261028v26" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261028v26" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page">
 <header class="sa-head"><div><h1>Address Verification</h1><p>Review, correct and verify manually entered addresses.</p></div>
  <a class="sa-btn sa-btn-outline" href="SupportAddresses.aspx?tab=verified">&#9745; View Verified Addresses</a></header>
 <asp:Literal ID="Flash" runat="server" />
 <ol class="sa-flow" aria-label="Address verification workflow">
  <li class="sa-flow-1"><b>1</b><strong>Manual Address Submitted</strong><span>Employee or subscriber enters address details manually.</span></li>
  <li class="sa-flow-2"><b>2</b><strong>Pending Verification</strong><span>Address is added to the Support Admin review queue.</span></li>
  <li class="sa-flow-3"><b>3</b><strong>Support Reviews</strong><span>Check and correct the address if needed.</span></li>
  <li class="sa-flow-4"><div class="sa-flow-ok"><b>4A</b><strong>Approve</strong><span>Save to Verified Address Database.</span></div><div class="sa-flow-no"><b>4B</b><strong>Reject</strong><span>Return for correction with reason.</span></div></li>
  <li class="sa-flow-5"><b>5</b><strong>Verified Address Database</strong><span>Approved addresses are available for reuse in address lookup.</span></li>
 </ol>
 <div class="sa-split">
  <div class="sa-card sa-main"><asp:Literal ID="Tabs" runat="server" /><asp:Literal ID="Filters" runat="server" /><asp:Literal ID="Table" runat="server" /></div>
  <asp:Literal ID="Panel" runat="server" />
 </div>
</section>
</asp:Content>
