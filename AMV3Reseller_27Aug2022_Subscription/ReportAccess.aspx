<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="ReportAccess.aspx.cs" Inherits="AnyminuteReportAccessPage" Title="Report Access" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteReports.css?v=20261026v19" rel="stylesheet" /><script src="Scripts/AnyminuteReports.js?v=20261026v19" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-reports am-report-access">
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-rp-error" role="alert" />
 <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-rp-error">Report Access settings need the database update DB_REQUIRED/REQUIRED_06_ReportAccess.sql. Until it is run, the default permissions below apply and cannot be saved.</asp:Label>
 <header class="am-rp-head"><div><h1>Report Access Defaults</h1><p>Set global report permissions by role. These defaults apply to all businesses unless you create a business-specific override.</p></div></header>
 <div class="am-ra-layout">
  <div>
   <div class="am-rp-banner"><strong>i</strong><div><b>Default Role Permissions</b><br/>Employee, Supervisor and Administrator permissions are applied to all businesses by default. View, PDF and Excel access can be set independently.</div><asp:Label ID="DefaultChip" runat="server" CssClass="am-rp-pill is-ok am-ra-chip" Text="Default Active" /></div>
   <div class="am-rp-card"><h2>Global Default Report Access</h2><div class="am-rp-table-wrap"><asp:Literal ID="DefaultMatrix" runat="server" /></div></div>
   <div class="am-rp-card"><h2>Business Overrides</h2>
    <div class="am-ra-override-bar"><label><span>Select Business</span> <asp:DropDownList ID="OverrideBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="OverrideBusiness_Changed" /></label>
     <label class="am-ra-switch"><asp:CheckBox ID="UseGlobal" runat="server" /> <span><b>Use Global Defaults</b> Turn this off only if this business needs different report access.</span></label>
     <asp:Label ID="OverrideChip" runat="server" CssClass="am-rp-pill is-ok" Text="Using Global Defaults" /></div>
    <div id="OverridePanel" class="am-ra-override" runat="server" ClientIDMode="Static"><p class="am-rp-sub">Override settings for the selected business. When saved, they replace the global defaults for that business only.</p><div class="am-rp-table-wrap"><asp:Literal ID="OverrideMatrix" runat="server" /></div></div>
    <div class="am-rp-buttons"><asp:Button ID="SaveOverride" runat="server" Text="Save Business Override" CssClass="am-rp-btn" OnClick="SaveOverride_Click" /></div>
   </div>
   <div class="am-rp-buttons am-ra-actions"><a class="am-rp-btn" href="Dashboard.aspx">Cancel</a><asp:Button ID="SaveDefaults" runat="server" Text="Save Default Settings" CssClass="am-rp-btn am-rp-primary" OnClick="SaveDefaults_Click" /></div>
  </div>
  <aside class="am-rp-card am-ra-how"><h2>How It Works</h2><ol>
   <li><b>Set Global Defaults</b><span>Select View, PDF and Excel permissions for each role. These apply to all businesses.</span></li>
   <li><b>Create Business Override (Optional)</b><span>If a business needs different report access, turn off Use Global Defaults and save an override.</span></li>
   <li><b>Overrides Replace Defaults</b><span>When an override exists, it replaces the global default for that business only.</span></li>
   <li><b>Easy Management</b><span>Keep global defaults enabled for consistency and override only when needed.</span></li></ol>
   <p class="am-rp-note">Roles: <b>Administrator</b> = business owner or administrator; <b>Supervisor</b> = manager, team leader, accountant or an employee who supervises others; <b>Employee</b> = employee. Supervisors see only their own and their team's data; employees see only their own.</p></aside>
 </div>
</section>
</asp:Content>
