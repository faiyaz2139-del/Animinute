<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="ProjectAssignments.aspx.cs" Inherits="AnyminuteProjectAssignmentsPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261024v8" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer></script><script src="Scripts/AnyminuteProjects.js?v=20261024v8" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-projects am-pj-dialog am-pj-assign">
 <header class="am-pj-dhead"><div><h1>Manage Project Assignments</h1><p>Project: <asp:Literal ID="HeadProject" runat="server" /> <span class="am-pj-bar">|</span> Business: <asp:Literal ID="HeadBusiness" runat="server" /></p></div><asp:HyperLink ID="CloseLink" runat="server" NavigateUrl="~/Projects.aspx" CssClass="am-pj-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <div class="am-pj-statusbar">
  <div><span>Project Status</span><asp:Label ID="HeadStatus" runat="server" CssClass="am-pj-pill" /></div>
  <div><span>Start Date</span><strong><asp:Literal ID="HeadStart" runat="server" /></strong></div>
  <div><span>End Date</span><strong><asp:Literal ID="HeadEnd" runat="server" /></strong></div>
  <div><span>Total Assigned Employees</span><strong class="am-pj-big"><asp:Literal ID="HeadTotal" runat="server" /></strong></div>
 </div>
 <nav class="am-pj-tabs" role="tablist"><asp:Literal ID="TabLinks" runat="server" /></nav>
 <div class="am-pj-dbody">
  <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-pj-info">Assigned dates, effective end dates and history appear after the Projects database update (DB_REQUIRED/REQUIRED_03_ProjectModule.sql) is installed.</asp:Label>

  <asp:Panel ID="AssignedSection" runat="server" CssClass="am-pj-section" data-pj-scope="assigned">
   <div class="am-pj-shead"><div class="am-pj-stitle"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M16 11a4 4 0 1 0-3.9-5A4 4 0 0 1 16 11zM8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-7 1.5-7 4.5V20h14v-2.5C15 14.5 11 13 8 13zm8 0c-.3 0-.7 0-1.1.1A5 5 0 0 1 17 17.5V20h6v-2.5c0-3-4-4.5-7-4.5z"/></svg><h2>Currently Assigned Employees (<asp:Literal ID="AssignedCount" runat="server" />)</h2></div>
    <label class="am-pj-search am-pj-search-sm"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search assigned employees..." data-pj-search="#PjAssigned" aria-label="Search assigned employees" /></label></div>
   <div class="am-pj-table-wrap"><table class="am-pj-table am-pj-grid" id="PjAssigned">
    <thead><tr><th class="am-pj-cb"><input type="checkbox" data-pj-all="sel" aria-label="Select all assigned" /></th><th>Employee Name</th><th>Role</th><th>Department</th><th>Assigned Date</th><th>Effective End Date</th><th class="am-c">Status</th><th class="am-c">Action</th></tr></thead>
    <tbody><asp:Repeater ID="Members" runat="server" OnItemCommand="Members_ItemCommand"><ItemTemplate><tr data-pj-text="<%#: Eval("Name") %> <%#: Eval("Role") %> <%#: Eval("Department") %>">
     <td class="am-pj-cb"><input type="checkbox" name="sel" value="<%# Eval("EmployeeId") %>" aria-label="Select <%#: Eval("Name") %>" /></td>
     <td><%#: Eval("Name") %></td><td><%#: Eval("Role") %></td><td><%#: Eval("Department") %></td><td><%# Eval("AssignedText") %></td><td><%# Eval("EndText") %></td>
     <td class="am-c"><span class="am-pj-pill <%# Convert.ToString(Eval("Status")) == "Active" ? "am-pj-st-active" : "am-pj-st-ending" %>"><%# Eval("Status") %></span></td>
     <td class="am-c am-pj-actions"><a class="am-pj-ic am-pj-blue" title="Edit employee" href="<%# ResolveUrl("~/EmployeeEditor.aspx?EmployeeId=" + Eval("EmployeeId")) %>"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 21h4L18.5 9.5l-4-4L3 17z" fill="currentColor"/><path d="M15.5 4.5l4 4 1.5-1.5-4-4z" fill="currentColor"/></svg><span class="am-visually-hidden">Edit</span></a>
      <asp:LinkButton runat="server" CssClass="am-pj-ic am-pj-red" CommandName="Unassign" CommandArgument='<%# Eval("EmployeeId") %>' CausesValidation="false" ToolTip="Unassign from project" Visible='<%# CanManage %>' OnClientClick="return confirm('Unassign this employee from the project? The project and existing time entries are kept.');"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg><span class="am-visually-hidden">Unassign</span></asp:LinkButton></td></tr></ItemTemplate></asp:Repeater>
     <asp:PlaceHolder ID="NoMembers" runat="server" Visible="false"><tr><td colspan="8" class="am-pj-empty">No employees are assigned to this project.</td></tr></asp:PlaceHolder></tbody>
   </table></div>
   <asp:Panel ID="AssignedActions" runat="server" CssClass="am-pj-row-actions">
    <asp:LinkButton ID="UnassignSelected" runat="server" CssClass="am-pj-btn am-pj-danger-outline" OnClick="UnassignSelected_Click" CausesValidation="false" data-pj-count="sel" OnClientClick="return AnyminuteProjects.confirmSelected('sel','Unassign the selected employees from this project? The project and existing time entries are kept.');"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg><span>Unassign Selected (<span data-pj-n="sel">0</span>)</span></asp:LinkButton>
    <span class="am-pj-right">
     <span class="am-pj-enddate"><button type="button" class="am-pj-btn am-pj-outline" data-pj-toggle="#PjEndBox"><svg viewBox="0 0 24 24" aria-hidden="true"><rect x="3" y="5" width="18" height="16" rx="2" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M3 10h18M8 3v4M16 3v4" stroke="currentColor" stroke-width="1.8"/></svg>Set Effective End Date</button>
      <span class="am-pj-pop am-datefmt" id="PjEndBox" hidden="hidden"><label>Effective end date for selected employees<asp:TextBox ID="EndDateBox" runat="server" TextMode="Date" data-placeholder="yyyy-mm-dd" /></label><asp:LinkButton ID="ApplyEndDate" runat="server" CssClass="am-pj-btn am-pj-primary" OnClick="ApplyEndDate_Click" CausesValidation="false" OnClientClick="return AnyminuteProjects.confirmSelected('sel','Set the effective end date for the selected employees?');">Apply</asp:LinkButton></span></span>
     <asp:LinkButton ID="ExportButton" runat="server" CssClass="am-pj-btn am-pj-outline" OnClick="Export_Click" CausesValidation="false"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 3v12M7 10l5 5 5-5M4 19h16" fill="none" stroke="currentColor" stroke-width="2"/></svg>Export to Excel</asp:LinkButton>
    </span>
   </asp:Panel>
  </asp:Panel>

  <asp:Panel ID="AddSection" runat="server" CssClass="am-pj-section" data-pj-scope="add">
   <div class="am-pj-shead"><div class="am-pj-stitle"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3.3 0-8 1.7-8 5v2h16v-2c0-3.3-4.7-5-8-5zm10-5V6h-2v2h-2v2h2v2h2v-2h2V8z"/></svg><h2>Add / Assign Additional Employees</h2></div>
    <label class="am-pj-search am-pj-search-sm"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><input type="search" placeholder="Search employee by name, role or department..." data-pj-search="#PjCandidates" aria-label="Search employees" /></label></div>
   <div class="am-pj-addgrid">
    <div>
     <div class="am-pj-cfilters"><select data-pj-filter="role" aria-label="Role"><option value="">All Roles</option><asp:Literal ID="RoleOptions" runat="server" /></select><select data-pj-filter="dept" aria-label="Department"><option value="">All Departments</option><asp:Literal ID="DeptOptions" runat="server" /></select><select data-pj-filter="active" aria-label="Employee status"><option value="1">Active Employees</option><option value="">All Employees</option></select></div>
     <div class="am-pj-table-wrap"><table class="am-pj-table am-pj-grid" id="PjCandidates">
      <thead><tr><th class="am-pj-cb"><input type="checkbox" data-pj-all="add" aria-label="Select all employees" /></th><th>Employee Name</th><th>Role</th><th>Department</th></tr></thead>
      <tbody><asp:Repeater ID="Candidates" runat="server"><ItemTemplate><tr data-pj-text="<%#: Eval("Name") %> <%#: Eval("Role") %> <%#: Eval("Department") %>" data-role="<%#: Eval("Role") %>" data-dept="<%#: Eval("Department") %>" data-active="<%# (bool)Eval("Active") ? "1" : "0" %>">
       <td class="am-pj-cb"><input type="checkbox" name="add" value="<%# Eval("EmployeeId") %>" aria-label="Select <%#: Eval("Name") %>" /></td><td class="am-pj-cname"><%#: Eval("Name") %></td><td class="am-pj-crole"><%#: Eval("Role") %></td><td class="am-pj-cdept"><%#: Eval("Department") %></td></tr></ItemTemplate></asp:Repeater>
       <asp:PlaceHolder ID="NoCandidates" runat="server" Visible="false"><tr><td colspan="4" class="am-pj-empty">Every eligible employee of this business is already assigned.</td></tr></asp:PlaceHolder></tbody>
     </table></div>
    </div>
    <div class="am-pj-selected"><div class="am-pj-selhead"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M16 11a4 4 0 1 0-3.9-5A4 4 0 0 1 16 11zM8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-7 1.5-7 4.5V20h14v-2.5C15 14.5 11 13 8 13zm8 0c-.3 0-.7 0-1.1.1A5 5 0 0 1 17 17.5V20h6v-2.5c0-3-4-4.5-7-4.5z"/></svg><span>Selected Employees (<span data-pj-n="add">0</span>)</span></div>
     <table class="am-pj-table am-pj-grid"><thead><tr><th>Employee Name</th><th>Role</th><th>Department</th><th class="am-c">Remove</th></tr></thead><tbody id="PjSelected"><tr class="am-pj-selempty"><td colspan="4" class="am-pj-empty">No employees selected.</td></tr></tbody></table>
    </div>
   </div>
   <div class="am-pj-dfoot am-pj-inline-foot"><asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="~/Projects.aspx" CssClass="am-pj-btn am-pj-outline">Cancel</asp:HyperLink><asp:LinkButton ID="AssignSelected" runat="server" CssClass="am-pj-btn am-pj-primary" OnClick="AssignSelected_Click" CausesValidation="false" OnClientClick="return AnyminuteProjects.confirmSelected('add',null);"><span>Assign Selected (<span data-pj-n="add">0</span>)</span></asp:LinkButton></div>
  </asp:Panel>

  <asp:Panel ID="HistorySection" runat="server" CssClass="am-pj-section">
   <div class="am-pj-shead"><div class="am-pj-stitle"><h2>Assignment History</h2></div></div>
   <div class="am-pj-table-wrap"><table class="am-pj-table am-pj-grid"><thead><tr><th>Date &amp; Time</th><th>Employee</th><th>Action</th><th>Effective Date</th><th>Details</th><th>By</th></tr></thead>
    <tbody><asp:Repeater ID="History" runat="server"><ItemTemplate><tr><td><%# LocalTime(Eval("RecordedAtUTC")) %></td><td><%#: Eval("Employee") %></td><td><%#: Eval("Action") %></td><td><%# Day(Eval("EffectiveDate")) %></td><td><%#: Eval("Note") %></td><td><%#: Eval("ActorName") %></td></tr></ItemTemplate></asp:Repeater>
     <asp:PlaceHolder ID="NoHistory" runat="server" Visible="false"><tr><td colspan="6" class="am-pj-empty">No assignment history recorded yet.</td></tr></asp:PlaceHolder></tbody></table></div>
  </asp:Panel>

  <asp:Panel ID="DetailsSection" runat="server" CssClass="am-pj-section">
   <div class="am-pj-shead"><div class="am-pj-stitle"><h2>Project Details</h2></div><asp:HyperLink ID="EditProjectLink" runat="server" CssClass="am-pj-btn am-pj-outline">Edit Project</asp:HyperLink></div>
   <dl class="am-pj-summary am-pj-details">
    <dt>Project Name</dt><dd><asp:Literal ID="DName" runat="server" /></dd><dt>Business Name</dt><dd><asp:Literal ID="DBusiness" runat="server" /></dd>
    <dt>Start Date</dt><dd><asp:Literal ID="DStart" runat="server" /></dd><dt>End Date</dt><dd><asp:Literal ID="DEnd" runat="server" /></dd>
    <dt>Project Status</dt><dd><asp:Literal ID="DStatus" runat="server" /></dd><dt>Assignment Status</dt><dd><asp:Literal ID="DAssign" runat="server" /></dd>
    <dt>No. of Employees</dt><dd><asp:Literal ID="DEmployees" runat="server" /></dd><dt>Total Hours</dt><dd><asp:Literal ID="DHours" runat="server" /></dd>
    <dt>Description</dt><dd><asp:Literal ID="DDescription" runat="server" /></dd>
   </dl>
  </asp:Panel>
 </div>
</section>
</asp:Content>
