<%@ Page Title="Assign Users to Manager" Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="AssgineManager.aspx.cs" Inherits="AssgineManager" %>
<%-- 2026-10-28 v27: modern UI (no old header / Home button); business, manager and user pickers are always populated; saves go through
     AnyminuteApprovalAssignments (same circular-chain guard and [AM-<build>] messages as GridviewNewDesign). Legacy file name kept. --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><style>.am-am-list{display:grid;grid-template-columns:repeat(auto-fill,minmax(260px,1fr));gap:6px 18px;margin:8px 0}.am-am-list td{padding:4px 0}.am-am-list label{font-weight:500;margin-left:6px}.am-am-list input[disabled]+label{color:#8a95a3}</style></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="am-title"><div><h1>Assign Users to Manager</h1><p>Choose who each manager approves. A user who already approves this manager (directly or through other managers) cannot be placed under them - that would make a circular approval chain.</p></div>
 <div class="am-actions"><a href="Dashboard.aspx" class="am-button secondary">Back to Dashboard</a></div></div>
<asp:Label ID="Notice" runat="server" Visible="false" CssClass="am-notice" role="status" />
<asp:PlaceHolder ID="Editor" runat="server">
<section class="am-card">
 <div class="am-fields">
  <label>Business *<asp:DropDownList ID="ddlbusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlbusiness_SelectedIndexChanged" /></label>
  <label>Manager *<asp:DropDownList ID="ddlmanager" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlmanager_SelectedIndexChanged" /></label>
 </div>
 <asp:PlaceHolder ID="People" runat="server" Visible="false">
  <h2 style="font-size:16px;margin:20px 0 4px">Users approved by <asp:Literal ID="ManagerName" runat="server" /></h2>
  <p class="am-muted">Tick the employees this manager approves. Greyed-out names cannot be assigned (reason shown).</p>
  <asp:CheckBoxList ID="ddlemp" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="am-am-list" />
  <asp:Label ID="Empty" runat="server" Visible="false" CssClass="am-muted" Text="No active employees are available in this business." />
  <asp:Literal ID="Stale" runat="server" />
  <asp:HiddenField ID="hfSelected" runat="server" />
  <div class="am-actions" style="margin-top:16px"><asp:Button ID="btnSave" runat="server" Text="Save Assignments" CssClass="am-button" OnClick="btnSave_Click" /></div>
 </asp:PlaceHolder>
</section>
</asp:PlaceHolder>
</asp:Content>
