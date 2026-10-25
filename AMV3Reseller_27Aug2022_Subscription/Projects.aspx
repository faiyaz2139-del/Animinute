<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="Projects.aspx.cs" Inherits="AnyminuteProjects" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261025v13" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer></script><script src="Scripts/AnyminuteProjects.js?v=20261024v8" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-projects" id="Root" runat="server">
<div class="am-pj-main">
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
 <header class="am-pj-heading"><div><h1>Projects</h1><p>View and manage all projects across businesses.</p></div><asp:HyperLink ID="AddProject" runat="server" NavigateUrl="~/ProjectEditor.aspx" CssClass="am-pj-btn am-pj-primary"><span aria-hidden="true" class="am-pj-plus">+</span> Add Project</asp:HyperLink></header>
 <div class="am-pj-metrics">
  <div class="am-pj-metric am-pj-m-blue"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M6 2h8l6 6v14H6z" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linejoin="round"/><path d="M14 2v6h6M9 13h6M9 17h6" fill="none" stroke="currentColor" stroke-width="1.8"/></svg></span><div>Total Projects<strong><asp:Literal ID="TotalCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-green"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M16 11a4 4 0 1 0-3.9-5A4 4 0 0 1 16 11zM8 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3 0-7 1.5-7 4.5V20h14v-2.5C15 14.5 11 13 8 13zm8 0c-.3 0-.7 0-1.1.1A5 5 0 0 1 17 17.5V20h6v-2.5c0-3-4-4.5-7-4.5z"/></svg></span><div>Assigned Projects<strong><asp:Literal ID="AssignedCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-amber"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M12 12a5 5 0 1 0 0-10 5 5 0 0 0 0 10zm0 2c-4 0-9 2-9 6v2h18v-2c0-4-5-6-9-6z"/></svg></span><div>Not Assigned Projects<strong><asp:Literal ID="NotAssignedCount" runat="server" Text="0" /></strong></div></div>
  <div class="am-pj-metric am-pj-m-purple"><span class="am-pj-micon"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="9" fill="none" stroke="currentColor" stroke-width="2.4"/><path fill="currentColor" d="M12 3a9 9 0 0 1 9 9h-9z"/></svg></span><div>Completed Projects<strong><asp:Literal ID="CompletedCount" runat="server" Text="0" /></strong></div></div>
 </div>
 <div class="am-pj-card">
  <asp:Panel runat="server" DefaultButton="ApplyFilters" CssClass="am-pj-filters">
   <label class="am-pj-search"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2"/><path d="M20 20l-4-4" stroke="currentColor" stroke-width="2"/></svg><span class="am-visually-hidden">Search</span><asp:TextBox ID="Search" runat="server" placeholder="Search project name or business name..." data-pj-search="#PjTable" /></label>
   <asp:DropDownList ID="BusinessFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" aria-label="Business" />
   <asp:DropDownList ID="StatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" aria-label="Project status"><asp:ListItem Value="">All Status</asp:ListItem><asp:ListItem>Active</asp:ListItem><asp:ListItem>Completed</asp:ListItem><asp:ListItem>Inactive</asp:ListItem><asp:ListItem>Archived</asp:ListItem></asp:DropDownList>
   <asp:DropDownList ID="AssignFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Filter_Changed" aria-label="Assignment status"><asp:ListItem Value="">All Assignment Status</asp:ListItem><asp:ListItem>Assigned</asp:ListItem><asp:ListItem>Not Assigned</asp:ListItem></asp:DropDownList>
   <div class="am-pj-range am-datefmt"><svg class="am-pj-range-ic" viewBox="0 0 24 24" aria-hidden="true"><rect x="3" y="5" width="18" height="16" rx="2" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M3 10h18M8 3v4M16 3v4" stroke="currentColor" stroke-width="1.8"/></svg><asp:TextBox ID="FromDate" runat="server" TextMode="Date" data-placeholder="Start Date" data-nobutton="1" aria-label="Start date from" AutoPostBack="true" OnTextChanged="Filter_Changed" /><span class="am-pj-dash">&ndash;</span><asp:TextBox ID="ToDate" runat="server" TextMode="Date" data-placeholder="End Date" aria-label="End date to" AutoPostBack="true" OnTextChanged="Filter_Changed" /></div>
   <asp:Button ID="ApplyFilters" runat="server" Text="Apply" OnClick="Filter_Changed" CssClass="am-visually-hidden" CausesValidation="false" />
  </asp:Panel>
  <div class="am-pj-table-wrap">
   <table class="am-pj-table" id="PjTable">
    <thead><tr><th>Project Name</th><th>Business Name</th><th>Start Date</th><th>End Date</th><th class="am-c">Project Status</th><th class="am-c">No. of<br />Employees</th><th class="am-c">Assignment Status</th><th class="am-c">Total Hours</th><th class="am-c">Action</th></tr></thead>
    <asp:Repeater ID="Groups" runat="server" OnItemDataBound="Groups_ItemDataBound"><ItemTemplate>
    <tbody class="am-pj-group" data-pj-group="<%# Eval("BusinessId") %>">
     <tr class="am-pj-group-head"><td colspan="9"><button type="button" class="am-pj-chev" aria-expanded="true" aria-label="Collapse group"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M6 9l6 6 6-6" fill="none" stroke="currentColor" stroke-width="2.4"/></svg></button><strong><%#: Eval("Business") %></strong><span class="am-pj-count"><%# Eval("Count") %> <%# (int)Eval("Count") == 1 ? "Project" : "Projects" %></span></td></tr>
     <asp:Repeater ID="Rows" runat="server" OnItemCommand="Rows_ItemCommand"><ItemTemplate>
     <tr class="am-pj-row<%# (int)Eval("ProjectId") == SelectedDelete ? " am-pj-selected" : "" %>" data-pj-text="<%#: Eval("Name") %> <%#: Eval("Business") %>">
      <td class="am-pj-name"><%#: Eval("Name") %></td><td class="am-pj-muted"><%#: Eval("Business") %></td><td><%# Eval("StartText") %></td><td><%# Eval("EndText") %></td>
      <td class="am-c"><span class="am-pj-pill am-pj-st-<%# Convert.ToString(Eval("Status")).ToLowerInvariant() %>"><%#: Eval("Status") %></span></td>
      <td class="am-c"><%# Eval("Employees") %></td>
      <td class="am-c"><span class="am-pj-pill <%# (int)Eval("Employees") > 0 ? "am-pj-assigned" : "am-pj-unassigned" %>"><%# Eval("AssignmentStatus") %></span></td>
      <td class="am-c"><%# Eval("HoursText") %></td>
      <td class="am-c am-pj-actions">
       <a class="am-pj-ic" title="View Project" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId") + "&tab=details") %>"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7S1 12 1 12z" fill="currentColor"/><circle cx="12" cy="12" r="3.2" fill="#fff"/></svg><span class="am-visually-hidden">View</span></a>
       <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("CanManage") %>'><a class="am-pj-ic" title="Edit Project" href="<%# ResolveUrl("~/ProjectEditor.aspx?ProjectId=" + Eval("ProjectId")) %>"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M3 21h4L18.5 9.5l-4-4L3 17z" fill="currentColor"/><path d="M15.5 4.5l4 4 1.5-1.5-4-4z" fill="currentColor"/></svg><span class="am-visually-hidden">Edit</span></a>
       <a class="am-pj-ic" title="Manage Employees" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId")) %>"><svg viewBox="0 0 24 24" aria-hidden="true"><path fill="currentColor" d="M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm0 2c-3.3 0-8 1.7-8 5v2h16v-2c0-3.3-4.7-5-8-5zm8.5-2.5a3 3 0 1 0 0-6v6zm1 2.6c1.9.8 3.5 2.2 3.5 4.9v2h-3v-2c0-2-.2-3.6-.5-4.9z"/></svg><span class="am-visually-hidden">Manage Employees</span></a></asp:PlaceHolder>
       <span class="am-pj-more"><button type="button" class="am-pj-ic am-pj-kebab" aria-haspopup="true" aria-expanded="false" title="More actions"><svg viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="5" r="2" fill="currentColor"/><circle cx="12" cy="12" r="2" fill="currentColor"/><circle cx="12" cy="19" r="2" fill="currentColor"/></svg><span class="am-visually-hidden">More</span></button>
        <span class="am-pj-menu" role="menu">
         <a role="menuitem" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId") + "&tab=details") %>"><svg viewBox="0 0 24 24"><path d="M1 12s4-7 11-7 11 7 11 7-4 7-11 7S1 12 1 12z" fill="none" stroke="currentColor" stroke-width="1.8"/><circle cx="12" cy="12" r="3" fill="currentColor"/></svg>View Project</a>
         <asp:PlaceHolder runat="server" Visible='<%# (bool)Eval("CanManage") %>'>
         <a role="menuitem" href="<%# ResolveUrl("~/ProjectEditor.aspx?ProjectId=" + Eval("ProjectId")) %>"><svg viewBox="0 0 24 24"><path d="M4 20h4L19 9l-4-4L4 16z" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Edit Project</a>
         <a role="menuitem" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId")) %>"><svg viewBox="0 0 24 24"><circle cx="9" cy="8" r="3.5" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M2 20c0-4 3.5-6 7-6s7 2 7 6M17 5a3 3 0 0 1 0 6M19 14c2 .7 3 2.5 3 5" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Manage Employees</a>
         <a role="menuitem" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId") + "&tab=add") %>"><svg viewBox="0 0 24 24"><circle cx="9" cy="8" r="3.5" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M2 20c0-4 3.5-6 7-6s7 2 7 6M19 8v6M16 11h6" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Assign Employees</a>
         <a role="menuitem" href="<%# ResolveUrl("~/ProjectAssignments.aspx?ProjectId=" + Eval("ProjectId") + "&tab=assigned&unassign=1") %>"><svg viewBox="0 0 24 24"><circle cx="9" cy="8" r="3.5" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M2 20c0-4 3.5-6 7-6s7 2 7 6M16 11h6" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Unassign Employees</a>
         <span class="am-pj-menu-sep"></span>
         <asp:LinkButton runat="server" role="menuitem" CommandName="Archive" CommandArgument='<%# Eval("ProjectId") %>' CausesValidation="false" Visible='<%# Convert.ToString(Eval("Status")) != "Archived" %>' OnClientClick="return confirm('Archive this project? Current assignments end today; time entries and history are kept.');"><svg viewBox="0 0 24 24"><rect x="3" y="4" width="18" height="5" rx="1" fill="none" stroke="currentColor" stroke-width="1.8"/><path d="M5 9v11h14V9M10 13h4" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Archive Project</asp:LinkButton>
         <a role="menuitem" class="am-pj-danger" href="<%# DeleteUrl((int)Eval("ProjectId")) %>"><svg viewBox="0 0 24 24"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="1.8"/></svg>Delete Project</a>
         </asp:PlaceHolder>
        </span></span>
      </td></tr>
     </ItemTemplate></asp:Repeater>
    </tbody>
    </ItemTemplate></asp:Repeater>
    <tbody><asp:PlaceHolder ID="Empty" runat="server" Visible="false"><tr><td colspan="9" class="am-pj-empty">No projects match the current filters.</td></tr></asp:PlaceHolder></tbody>
   </table>
  </div>
 </div>
</div>

<asp:Panel ID="DeletePanel" runat="server" Visible="false" CssClass="am-pj-side" role="dialog" aria-labelledby="pj-del-title">
 <asp:HyperLink ID="DeleteClose" runat="server" CssClass="am-pj-side-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink>
 <div class="am-pj-side-head"><span class="am-pj-del-badge"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg></span><h2 id="pj-del-title">Delete Project</h2></div>
 <p class="am-pj-side-intro">You can delete this project only if it has no assigned employees and no time entries.</p>
 <dl class="am-pj-summary">
  <dt>Project Name</dt><dd><asp:Literal ID="DelName" runat="server" /></dd>
  <dt>Business Name</dt><dd><asp:Literal ID="DelBusiness" runat="server" /></dd>
  <dt>Assignment Status</dt><dd><asp:Label ID="DelAssign" runat="server" CssClass="am-pj-pill" /></dd>
  <dt>No. of Employees</dt><dd><asp:Literal ID="DelEmployees" runat="server" /></dd>
  <dt>Total Hours</dt><dd><asp:Literal ID="DelHours" runat="server" /></dd>
 </dl>
 <h3>Deletion Requirements</h3>
 <ul class="am-pj-checks"><asp:Literal ID="DelChecks" runat="server" /></ul>
 <div class="am-pj-warn"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M12 2L1 21h22z" fill="#f5a524"/><path d="M12 9v5M12 17v.5" stroke="#fff" stroke-width="2.2" stroke-linecap="round"/></svg><span>If any records exist, use <strong>Archive Project</strong> instead of Delete.</span></div>
 <h3>Examples</h3>
 <div class="am-pj-example am-pj-ok"><span class="am-pj-dot">&#10003;</span><span>Not Assigned + No Time Entries<br /><strong>= Can Delete</strong></span></div>
 <div class="am-pj-example am-pj-no"><span class="am-pj-dot">&#10005;</span><span>Not Assigned + Has Time Entries<br /><strong>= Cannot Delete, Archive Instead</strong></span></div>
 <div class="am-pj-side-foot">
  <asp:HyperLink ID="DeleteCancel" runat="server" CssClass="am-pj-btn am-pj-outline">Cancel</asp:HyperLink>
  <asp:LinkButton ID="ArchiveInstead" runat="server" CssClass="am-pj-btn am-pj-outline" OnClick="ArchiveInstead_Click" CausesValidation="false" OnClientClick="return confirm('Archive this project? Current assignments end today; time entries and history are kept.');">Archive Instead</asp:LinkButton>
  <asp:LinkButton ID="DeleteProject" runat="server" CssClass="am-pj-btn am-pj-danger-btn" OnClick="DeleteProject_Click" CausesValidation="false" OnClientClick="return confirm('Delete this project? This cannot be undone from the Projects screen.');">Delete Project</asp:LinkButton>
 </div>
</asp:Panel>
</section>
</asp:Content>
