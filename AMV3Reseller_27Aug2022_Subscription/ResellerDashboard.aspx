<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="ResellerDashboard.aspx.cs" Inherits="AnyminuteResellerDashboardPage" Title="Reseller Dashboard" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261103v55" rel="stylesheet" /><script src="Scripts/AnyminuteSupport.js?v=20261103v55" defer="defer"></script>
<style>/* v55 Reseller PPT s1/s9 */
.rsl-kpis{display:grid;grid-template-columns:repeat(3,1fr);gap:14px;margin-bottom:16px}.rsl-kpis .sa-kpi-icon svg{width:24px;height:24px}
.rsl-qa{display:grid;grid-template-columns:repeat(5,1fr);gap:14px}.rsl-qa a{display:flex;flex-direction:column;align-items:center;gap:8px;padding:18px 10px;border:1px solid var(--sa-line,#dfe5ee);border-radius:12px;background:#fff;color:#14233c;font-weight:600;text-align:center;text-decoration:none}
.rsl-qa a:hover{border-color:#1f6fe5;box-shadow:0 2px 10px rgba(31,111,229,.12)}.rsl-qa svg{width:28px;height:28px;color:#1f6fe5}
.rsl-set{display:grid;grid-template-columns:repeat(3,1fr);gap:14px}.rsl-set a{display:block;padding:18px;border:1px solid var(--sa-line,#dfe5ee);border-radius:12px;background:#fff;color:#14233c;text-decoration:none}.rsl-set a b{display:block;margin-bottom:4px}
@media (max-width:1100px){.rsl-kpis{grid-template-columns:1fr 1fr}.rsl-qa{grid-template-columns:repeat(3,1fr)}.rsl-set{grid-template-columns:1fr}}
</style></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page sa-rsl-portal">
 <asp:Literal ID="Crumbs" runat="server" />
 <header class="sa-head"><div><h1><asp:Literal ID="Title1" runat="server" /></h1><p><asp:Literal ID="Lead" runat="server" /></p></div><asp:Literal ID="HeadActions" runat="server" /></header>
 <asp:Literal ID="Flash" runat="server" />
 <asp:Literal ID="Content" runat="server" />
</section>
</asp:Content>
