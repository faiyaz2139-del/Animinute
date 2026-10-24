<%@ Control Language="C#" AutoEventWireup="true" CodeFile="AnyminuteUiOverview.ascx.cs" Inherits="AnyminuteUiOverview" %>
<asp:HiddenField ID="UiDirectoryAccess" runat="server" ClientIDMode="Static" />
<asp:Panel ID="amServerOverview" runat="server" ClientIDMode="Static" CssClass="am-stats am-overview-server" role="region" aria-label="Business overview">
<div class="am-stat"><span class="am-stat-icon" data-am-icon="business"></span><div><small>Total Businesses</small><strong><asp:Literal ID="BusinessesCount" runat="server" /></strong><small>Within your access</small></div></div>
<div class="am-stat"><span class="am-stat-icon" data-am-icon="people"></span><div><small>Total Employees</small><strong><asp:Literal ID="EmployeesCount" runat="server" /></strong><small>Distinct assigned employee records</small></div></div>
<asp:Panel ID="PendingCard" runat="server" CssClass="am-stat"><span class="am-stat-icon" data-am-icon="clock"></span><div><small>Pending Approvals</small><strong><asp:Literal ID="PendingCount" runat="server" /></strong><small>Submitted weeks</small></div></asp:Panel>
<div class="am-stat"><span class="am-stat-icon" data-am-icon="folder"></span><div><small>Active Projects</small><strong><asp:Literal ID="ProjectsCount" runat="server" /></strong><small>Active projects in your businesses</small></div></div>
</asp:Panel>
