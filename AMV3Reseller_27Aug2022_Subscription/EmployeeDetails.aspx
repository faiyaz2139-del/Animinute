<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeDetails.aspx.cs" Inherits="AnyminuteEmployeeDetails" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261025v13" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261116v83" rel="stylesheet" /><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteEmployeeDetails.css?v=20261116v86" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v86 (Employee Details deck, 9 slides): page rebuilt to the deck layout - title + read-only note, profile hero with Edit,
     icon side menu, Business Assignment selector card and one redesigned panel per menu item. --%>
<section class="am-employees am-ed">
<header class="am-ed-title"><h1>Employee Details</h1><p>View employee information and assignments. This is a read-only view. Use Edit to make changes.</p></header>
<asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
<div class="am-ed-hero"><span class="am-ed-initials"><asp:Literal ID="Initials" runat="server" /></span><div class="am-ed-hero-main"><h2><asp:Literal ID="EmployeeName" runat="server" /></h2><div class="am-ed-hero-meta"><span><%= I("mail") %><asp:Literal ID="Email" runat="server" /></span><span><%= I("phone") %><asp:Literal ID="Phone" runat="server" /></span><asp:Label ID="EmployeeStatus" runat="server" /></div></div><asp:HyperLink ID="EditBasic" runat="server" CssClass="am-ed-btn am-ed-btn-primary am-ed-hero-edit" Visible="false" /></div>
<asp:Label ID="Notice" runat="server" Visible="false" CssClass="am-notice" role="alert" />
<div class="am-ed-layout"><nav class="am-ed-nav" aria-label="Employee details sections"><asp:HyperLink ID="ProfileTab" runat="server" /><asp:HyperLink ID="AssignmentsTab" runat="server" /><asp:HyperLink ID="PayTab" runat="server" /><asp:HyperLink ID="TimeTab" runat="server" /><asp:HyperLink ID="DocumentsTab" runat="server" /><hr /><asp:HyperLink ID="AddressTab" runat="server" /><asp:HyperLink ID="ProjectsTab" runat="server" /><asp:HyperLink ID="AccessTab" runat="server" /><asp:HyperLink ID="ActivityTab" runat="server" /></nav><div class="am-ed-main">
<div class="am-ed-card am-ed-context"><span class="am-ed-icon-box"><%= I("building") %></span><div><strong>Business Assignment</strong><small>Select the business this employee is assigned to.</small></div><asp:DropDownList ID="ContextAssignment" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ContextChanged" aria-label="Business assignment" CssClass="am-ed-select" /></div>

<asp:Panel ID="ProfilePane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("user") %></span><div><h2>Profile Information</h2><p>Core employee information. Use Edit to update these details.</p></div><span class="am-ed-readonly"><%= I("eye") %>Read-only view</span></div>
 <div class="am-ed-profile-grid">
  <div class="am-ed-info"><h3>Employee Information</h3>
   <div class="am-ed-info-row"><%= I("user") %><span>Name</span><strong><asp:Literal ID="InfoName" runat="server" /></strong></div>
   <div class="am-ed-info-row"><%= I("mail") %><span>Email</span><strong><asp:Literal ID="InfoEmail" runat="server" /></strong></div>
   <div class="am-ed-info-row"><%= I("phone") %><span>Phone</span><strong><asp:Literal ID="InfoPhone" runat="server" /></strong></div>
   <div class="am-ed-info-row"><%= I("activity") %><span>Status</span><strong><asp:Label ID="InfoStatus" runat="server" /></strong></div>
  </div>
  <div class="am-ed-profile-side">
   <div class="am-ed-tiles">
    <div class="am-ed-tile"><%= I("users") %><div><span>Role</span><strong><asp:Literal ID="InfoRole" runat="server" /></strong></div></div>
    <div class="am-ed-tile"><%= I("briefcase") %><div><span>Employment Type</span><strong><asp:Literal ID="InfoType" runat="server" /></strong></div></div>
    <div class="am-ed-tile"><%= I("calendar") %><div><span>Join Date</span><strong><asp:Literal ID="InfoJoin" runat="server" /></strong></div></div>
    <div class="am-ed-tile"><%= I("building") %><div><span>Assigned Business</span><strong><asp:Literal ID="InfoBusiness" runat="server" /></strong></div></div>
   </div>
   <div class="am-ed-stats">
    <div><%= I("calendar") %><strong><asp:Literal ID="StatTime" runat="server" /></strong><span>Time Cards</span><small>This Period</small></div>
    <div><%= I("layers") %><strong><asp:Literal ID="StatProjects" runat="server" /></strong><span>Projects</span><small>Assigned</small></div>
    <div><%= I("file") %><strong><asp:Literal ID="StatDocs" runat="server" /></strong><span>Documents</span><small>Uploaded</small></div>
   </div>
  </div>
 </div>
</asp:Panel>

<asp:Panel ID="AssignmentsPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("briefcase") %></span><div><h2>Business Assignments</h2><p>Manage this employee's business assignments, roles, and pay rates.</p></div><asp:HyperLink ID="AddAssignment" runat="server" CssClass="am-ed-btn am-ed-btn-primary" Visible="false" /></div>
 <div class="am-ed-table-wrap"><table class="am-ed-table"><thead><tr><th>Business</th><th>Job Title</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Billing Rate</th><th>Effective Date</th><th>End Date</th><th>Status</th><th>Actions</th></tr></thead><tbody>
 <asp:Repeater ID="Assignments" runat="server" OnItemCommand="AssignmentCommand"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><%#: JobTitle(Eval("EmployeeId")) %></td><td><div class="am-ed-chips"><asp:Repeater runat="server" DataSource='<%# Eval("Roles") %>'><ItemTemplate><span class='<%# "am-ed-chip " + RoleChip(Container.DataItem) %>'><%#: Container.DataItem %></span></ItemTemplate></asp:Repeater></div></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("PayRateText") %></td><td><%#: Money(Eval("BillingRate")) %></td><td><%#: LongDate(Eval("EffectiveDate")) %></td><td><%#: LongDate(Eval("EndDate")) %></td><td><span class='<%# Pill(AssignStatus(Eval("Status"),Eval("EndDate"))) %>'><%#: AssignStatus(Eval("Status"),Eval("EndDate")) %></span></td><td class="am-ed-actions"><asp:HyperLink runat="server" CssClass="am-ed-act" ToolTip="Edit assignment" Visible='<%# CanManage %>' NavigateUrl='<%# "EmployeeAssignment.aspx?EmployeeId="+Eval("EmployeeId")+"&BusinessId="+Eval("BusinessId") %>' Text='<%# I("pencil") %>' /><asp:PlaceHolder runat="server" Visible='<%# CanManage %>'><details class="am-ed-menu"><summary class="am-ed-act" title="More actions"><%# I("dots") %></summary><div><asp:LinkButton runat="server" CommandName="RemoveAssignment" CommandArgument='<%# Eval("BusinessId") %>' CausesValidation="false" OnClientClick="return confirm('Remove this business assignment? Time cards and history are kept.');">Remove assignment</asp:LinkButton></div></details></asp:PlaceHolder></td></tr></ItemTemplate></asp:Repeater>
 </tbody></table></div>
</asp:Panel>

<asp:Panel ID="PayPane" runat="server" CssClass="am-ed-pay">
 <div class="am-ed-card am-ed-pane">
  <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("dollar") %></span><div><h2>Pay Rate History</h2><p>Track changes to the employee's pay and billing rates over time.</p></div></div>
  <div class="am-ed-box am-ed-summary"><h3>Current Pay Summary</h3><div class="am-ed-summary-grid">
   <div class="am-ed-summary-rate"><span class="am-ed-icon-soft"><%= I("coins") %></span><div><span><asp:Literal ID="CurrentRateLabel" runat="server" /></span><strong><asp:Literal ID="CurrentRate" runat="server" /></strong></div></div>
   <div class="am-ed-summary-rate"><span class="am-ed-icon-soft"><%= I("tag") %></span><div><span>Current Billing Rate</span><strong><asp:Literal ID="CurrentBilling" runat="server" /></strong></div></div>
   <dl class="am-ed-summary-list"><dt><%= I("calendar") %>Effective Date</dt><dd><asp:Literal ID="CurrentEffective" runat="server" /></dd><dt>Pay Type</dt><dd><asp:Literal ID="CurrentType" runat="server" /></dd><dt>Status</dt><dd><asp:Label ID="CurrentStatus" runat="server" /></dd></dl>
  </div></div>
  <div class="am-ed-box"><div class="am-ed-box-head"><h3>Rate History</h3><asp:LinkButton ID="ExportRates" runat="server" CssClass="am-ed-btn am-ed-btn-outline am-ed-btn-sm" OnClick="ExportRates_Click" CausesValidation="false" /></div>
   <div class="am-ed-table-wrap"><table class="am-ed-table am-ed-sortable"><thead><tr><th data-sort>Effective Date</th><th data-sort>End Date</th><th data-sort>Pay Type</th><th data-sort>Pay Rate</th><th data-sort>Billing Rate</th><th>Changed By</th><th>Reason / Notes</th><th>Status</th></tr></thead><tbody>
   <asp:Repeater ID="Rates" runat="server"><ItemTemplate><tr><td data-v='<%# Eval("EffSort") %>'><%#: Eval("Eff") %></td><td data-v='<%# Eval("EndSort") %>'><%#: Eval("End") %></td><td><%#: Eval("Type") %></td><td data-v='<%# Eval("RateSort") %>'><%#: Eval("Rate") %></td><td><%#: Eval("Billing") %></td><td><%#: Eval("By") %></td><td><%#: Eval("Reason") %></td><td><span class='<%# Pill(Convert.ToString(Eval("Status"))) %>'><%#: Eval("Status") %></span></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table><asp:Label ID="RatesEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No pay rates recorded." /></div>
  </div>
 </div>
 <aside class="am-ed-card am-ed-audit"><div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("activity") %></span><div><h2>Change Audit</h2><p>Timeline of pay rate changes and key events.</p></div></div><asp:Literal ID="RateAudit" runat="server" /></aside>
</asp:Panel>

<asp:Panel ID="TimePane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("clock") %></span><div><h2>Time Cards</h2><p>View and manage the employee's time cards, hours, and approvals.</p></div></div>
 <div class="am-ed-filters am-ed-time-filters">
  <label><span>Week / Date Range</span><span class="am-ed-select-icon"><%= I("calendar") %><asp:DropDownList ID="TimeWeek" runat="server" AutoPostBack="true" CssClass="am-ed-select" /></span></label>
  <label><span>Status</span><asp:DropDownList ID="TimeStatus" runat="server" AutoPostBack="true" CssClass="am-ed-select"><asp:ListItem Value="">All Statuses</asp:ListItem><asp:ListItem>Draft</asp:ListItem><asp:ListItem>Pending</asp:ListItem><asp:ListItem>Approved</asp:ListItem><asp:ListItem>Rejected</asp:ListItem></asp:DropDownList></label>
  <asp:HyperLink ID="TimeClear" runat="server" CssClass="am-ed-link" Text="Clear Filters" />
  <details class="am-ed-menu am-ed-export"><summary class="am-ed-btn am-ed-btn-outline am-ed-btn-sm"><%= I("download") %>Export <%= I("chevron") %></summary><div><asp:LinkButton ID="ExportTime" runat="server" OnClick="ExportTime_Click" CausesValidation="false">Export CSV</asp:LinkButton></div></details>
 </div>
 <div class="am-ed-kpis am-ed-kpis-4">
  <div class="am-ed-kpi am-k-blue"><span class="am-ed-kpi-icon"><%= I("clock") %></span><div><span>Total Hours</span><strong><asp:Literal ID="KpiTotal" runat="server" /></strong></div></div>
  <div class="am-ed-kpi am-k-green"><span class="am-ed-kpi-icon"><%= I("check") %></span><div><span>Approved Hours</span><strong><asp:Literal ID="KpiApproved" runat="server" /></strong></div></div>
  <div class="am-ed-kpi am-k-amber"><span class="am-ed-kpi-icon"><%= I("clock") %></span><div><span>Pending Hours</span><strong><asp:Literal ID="KpiPending" runat="server" /></strong></div></div>
  <div class="am-ed-kpi am-k-purple"><span class="am-ed-kpi-icon"><%= I("layers") %></span><div><span>Overtime Hours</span><strong><asp:Literal ID="KpiOvertime" runat="server" /></strong></div></div>
 </div>
 <div class="am-ed-time-grid">
  <div class="am-ed-box"><div class="am-ed-box-head"><h3>Recent Time Cards</h3><asp:HyperLink ID="TimeViewAll" runat="server" CssClass="am-ed-link" /></div>
   <div class="am-ed-table-wrap"><table class="am-ed-table am-ed-table-sm am-ed-sortable"><thead><tr><th data-sort>Week</th><th data-sort>Regular Hours</th><th data-sort>Overtime Hours</th><th data-sort>Status</th><th data-sort>Submitted On</th><th data-sort>Approved By</th><th>Actions</th></tr></thead><tbody>
   <asp:Repeater ID="TimeRows" runat="server"><ItemTemplate><tr><td data-v='<%# Eval("WeekSort") %>'><%#: Eval("WeekText") %></td><td data-v='<%# Eval("Regular") %>'><%#: Eval("RegularText") %></td><td data-v='<%# Eval("Overtime") %>'><%#: Eval("OvertimeText") %></td><td><span class='<%# Pill(Convert.ToString(Eval("Status"))) %>'><%# StatusIcon(Eval("Status")) %><%#: Eval("Status") %></span></td><td data-v='<%# Eval("SubmittedSort") %>'><%#: Eval("SubmittedText") %></td><td><%#: Eval("ApprovedBy") %></td><td class="am-ed-actions"><a class="am-ed-act" title="View time card" href='<%#: Eval("Url") %>'><%# I("eye") %></a><details class="am-ed-menu"><summary class="am-ed-act" title="More actions"><%# I("dots") %></summary><div><a href='<%#: Eval("Url") %>'>Open time card</a></div></details></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table><asp:Label ID="TimeEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No time cards for the selected filters." /></div>
  </div>
  <div class="am-ed-box am-ed-trend"><div class="am-ed-box-head"><h3>Hours Trend <span title="Regular and overtime hours per week"><%= I("info") %></span></h3><asp:DropDownList ID="TrendRange" runat="server" AutoPostBack="true" CssClass="am-ed-select am-ed-select-sm"><asp:ListItem Value="6">Last 6 Weeks</asp:ListItem><asp:ListItem Value="12">Last 12 Weeks</asp:ListItem></asp:DropDownList></div><asp:Literal ID="TrendChart" runat="server" /><div class="am-ed-legend"><span><i class="am-l-reg"></i>Regular Hours</span><span><i class="am-l-ot"></i>Overtime Hours</span></div></div>
 </div>
</asp:Panel>

<asp:Panel ID="DocumentsPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("file") %></span><div><h2>Documents</h2><p>Manage employee documents. Upload, view, and track document details and expiry dates.</p></div><asp:PlaceHolder ID="UploadButton" runat="server"><button type="button" class="am-ed-btn am-ed-btn-primary" data-am-toggle="UploadForm"><%= I("upload") %>Upload Document</button></asp:PlaceHolder></div>
 <asp:Panel ID="UploadForm" runat="server" CssClass="am-ed-upload" ClientIDMode="Static">
  <label><span>Document</span><asp:FileUpload ID="DocFile" runat="server" accept=".pdf,.doc,.docx,.png,.jpg,.jpeg" /></label>
  <label><span>Type</span><asp:DropDownList ID="DocType" runat="server" CssClass="am-ed-select" /></label>
  <label><span>Related Business</span><asp:DropDownList ID="DocBusiness" runat="server" CssClass="am-ed-select" /></label>
  <label><span>Expiry Date <small>(Optional)</small></span><asp:TextBox ID="DocExpiry" runat="server" TextMode="Date" CssClass="am-ed-select" /></label>
  <asp:Button ID="DocUpload" runat="server" Text="Upload" CssClass="am-ed-btn am-ed-btn-primary" OnClick="DocUpload_Click" CausesValidation="false" />
 </asp:Panel>
 <div class="am-ed-filters am-ed-doc-filters" data-am-docfilter>
  <label class="am-ed-search"><%= I("search") %><input type="search" placeholder="Search documents by name..." data-f="name" aria-label="Search documents by name" /></label>
  <select class="am-ed-select" data-f="type" aria-label="Type"><option value="">All Types</option><asp:Literal ID="DocTypeOptions" runat="server" /></select>
  <select class="am-ed-select" data-f="status" aria-label="Status"><option value="">All Statuses</option><option>Active</option><option>Expired</option></select>
  <a href="#" class="am-ed-link" data-f="clear"><%= I("refresh") %>Clear Filters</a>
 </div>
 <div class="am-ed-table-wrap"><table class="am-ed-table am-ed-sortable" data-am-doctable><thead><tr><th data-sort>Document Name</th><th data-sort>Type</th><th data-sort>Related Business</th><th data-sort>Uploaded On</th><th data-sort>Expiry Date</th><th data-sort>Uploaded By</th><th data-sort>Status</th><th>Actions</th></tr></thead><tbody>
 <asp:Repeater ID="Docs" runat="server" OnItemCommand="DocCommand"><ItemTemplate><tr data-name='<%#: Convert.ToString(Eval("FileName")).ToLowerInvariant() %>' data-type='<%#: Eval("Type") %>' data-status='<%#: Eval("Status") %>'><td><span class="am-ed-doc"><span class="am-ed-pdf"><%#: FileBadge(Eval("FileName")) %></span><strong><%#: Eval("FileName") %></strong></span></td><td><span class="am-ed-chip am-chip-blue"><%#: Eval("Type") %></span></td><td><%#: Eval("Business") %></td><td data-v='<%# Eval("UploadedSort") %>'><%#: Eval("UploadedText") %></td><td data-v='<%# Eval("ExpirySort") %>'><%#: Eval("ExpiryText") %></td><td><%#: Eval("UploadedBy") %></td><td><span class='<%# Pill(Convert.ToString(Eval("Status"))) %>'><%#: Eval("Status") %></span></td><td class="am-ed-actions"><a class="am-ed-act am-ed-act-box" title="View document" target="_blank" rel="noopener" href='<%#: DocUrl(Eval("Id"), false) %>'><%# I("eye") %></a><details class="am-ed-menu"><summary class="am-ed-act" title="More actions"><%# I("dots") %></summary><div><a href='<%#: DocUrl(Eval("Id"), true) %>'>Download</a><asp:LinkButton runat="server" Visible='<%# CanManage %>' CommandName="DeleteDoc" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick="return confirm('Delete this document? This cannot be undone.');">Delete</asp:LinkButton></div></details></td></tr></ItemTemplate></asp:Repeater>
 </tbody></table><asp:Label ID="DocsEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No documents uploaded yet." /></div>
</asp:Panel>

<asp:Panel ID="AddressPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("pin") %></span><div><h2>Address</h2><p>Employee address details for contact, compliance, and records.</p></div><asp:HyperLink ID="AddressHistoryLink" runat="server" CssClass="am-ed-btn am-ed-btn-outline" Visible="false" /><asp:HyperLink ID="EditAddress" runat="server" CssClass="am-ed-btn am-ed-btn-primary" Visible="false" /></div>
 <asp:Label ID="AddressScope" runat="server" CssClass="am-ed-empty" Visible="false" />
 <asp:PlaceHolder ID="AddressBody" runat="server">
 <div class="am-ed-address-grid">
  <div class="am-ed-box am-ed-address"><div class="am-ed-address-head"><span class="am-ed-icon-soft"><%= I("home") %></span><div><strong>Residential Address</strong><span><asp:Literal ID="ResLine" runat="server" /></span></div></div><dl><dt>City</dt><dd><asp:Literal ID="ResCity" runat="server" /></dd><dt>Province/State</dt><dd><asp:Literal ID="ResProvince" runat="server" /></dd><dt>Postal Code</dt><dd><asp:Literal ID="ResPostal" runat="server" /></dd><dt>Country</dt><dd><asp:Literal ID="ResCountry" runat="server" /></dd></dl></div>
  <div class="am-ed-box am-ed-address"><div class="am-ed-address-head"><span class="am-ed-icon-soft"><%= I("mail") %></span><div><strong>Mailing Address</strong><span><asp:Literal ID="MailLine" runat="server" /></span></div></div><dl><dt>City</dt><dd><asp:Literal ID="MailCity" runat="server" /></dd><dt>Province/State</dt><dd><asp:Literal ID="MailProvince" runat="server" /></dd><dt>Postal Code</dt><dd><asp:Literal ID="MailPostal" runat="server" /></dd><dt>Country</dt><dd><asp:Literal ID="MailCountry" runat="server" /></dd></dl></div>
 </div>
 <div class="am-ed-box am-ed-address-foot"><div><%= I("calendar") %><span><strong>Last Updated</strong><small><asp:Literal ID="AddressUpdated" runat="server" /></small></span></div><div><%= I("shield") %><strong>Verification Status</strong><asp:Label ID="AddressVerified" runat="server" /></div></div>
 <asp:Panel ID="AddressHistoryPanel" runat="server" Visible="false" CssClass="am-ed-box"><div class="am-ed-box-head"><h3>Address History</h3></div><div class="am-ed-table-wrap"><table class="am-ed-table am-ed-table-sm"><thead><tr><th>Date &amp; Time</th><th>Change</th><th>Changed By</th></tr></thead><tbody><asp:Repeater ID="AddressHistory" runat="server"><ItemTemplate><tr><td><%#: Eval("When") %></td><td><%#: Eval("AfterValue") %></td><td><%#: Eval("ChangedBy") %></td></tr></ItemTemplate></asp:Repeater></tbody></table><asp:Label ID="AddressHistoryEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No address changes recorded." /></div></asp:Panel>
 </asp:PlaceHolder>
 <div class="am-ed-infobar"><%= I("info") %>Address changes are recorded and maintained in the Address History for audit and compliance purposes.</div>
</asp:Panel>

<asp:Panel ID="ProjectsPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("layers") %></span><div><h2>Projects</h2><p>Projects assigned to this employee, including role, schedule, and time tracking.</p></div><asp:HyperLink ID="ProjectEditor" runat="server" CssClass="am-ed-btn am-ed-btn-primary" Visible="false" /><asp:HyperLink ID="ManageAssignments" runat="server" CssClass="am-ed-btn am-ed-btn-outline" Visible="false" /></div>
 <div class="am-ed-kpis am-ed-kpis-5">
  <div class="am-ed-kpi"><span class="am-ed-kpi-icon am-k-blue"><%= I("briefcase") %></span><div><strong><asp:Literal ID="PrjAssigned" runat="server" /></strong><span>Assigned Projects</span></div></div>
  <div class="am-ed-kpi"><span class="am-ed-kpi-icon am-k-blue"><%= I("clock") %></span><div><strong><asp:Literal ID="PrjHours" runat="server" /></strong><span>Hours Logged<br />(Total)</span></div></div>
  <div class="am-ed-kpi"><span class="am-ed-kpi-icon am-k-blue"><%= I("play") %></span><div><strong><asp:Literal ID="PrjActive" runat="server" /></strong><span>Active Projects</span></div></div>
  <div class="am-ed-kpi"><span class="am-ed-kpi-icon am-k-blue"><%= I("pause") %></span><div><strong><asp:Literal ID="PrjHold" runat="server" /></strong><span>On Hold</span></div></div>
  <div class="am-ed-kpi"><span class="am-ed-kpi-icon am-k-green"><%= I("checkcircle") %></span><div><strong><asp:Literal ID="PrjDone" runat="server" /></strong><span>Completed</span></div></div>
 </div>
 <div class="am-ed-table-wrap"><table class="am-ed-table am-ed-sortable"><thead><tr><th data-sort>Project Name</th><th data-sort>Client / Business</th><th data-sort>Role on Project</th><th data-sort>Start Date</th><th data-sort>End Date</th><th data-sort>Hours Logged</th><th data-sort>Status</th><th>Actions</th></tr></thead><tbody>
 <asp:Repeater ID="Projects" runat="server"><ItemTemplate><tr><td><a class="am-ed-project" href='<%#: "ProjectView.aspx?ProjectId="+Eval("ProjectId") %>'><%# I("folder") %><%#: Eval("Name") %></a></td><td><%#: Eval("Business") %></td><td><%#: Eval("Role") %></td><td data-v='<%# Eval("StartSort") %>'><%#: Eval("StartText") %></td><td data-v='<%# Eval("EndSort") %>'><%#: Eval("EndText") %></td><td data-v='<%# Eval("Hours") %>'><%#: Eval("HoursText") %></td><td><span class='<%# Pill(Convert.ToString(Eval("Status"))) %>'><%# StatusIcon(Eval("Status")) %><%#: Eval("Status") %></span></td><td class="am-ed-actions"><asp:HyperLink runat="server" CssClass="am-ed-act" ToolTip="Edit project" Visible='<%# CanManage %>' NavigateUrl='<%# "ProjectEditor.aspx?ProjectId="+Eval("ProjectId") %>' Text='<%# I("pencil") %>' /><details class="am-ed-menu"><summary class="am-ed-act" title="More actions"><%# I("dots") %></summary><div><a href='<%#: "ProjectView.aspx?ProjectId="+Eval("ProjectId") %>'>View project</a><asp:HyperLink runat="server" Visible='<%# CanManage %>' NavigateUrl='<%# "ProjectAssignments.aspx?ProjectId="+Eval("ProjectId") %>' Text="Manage assignments" /></div></details></td></tr></ItemTemplate></asp:Repeater>
 </tbody></table><asp:Label ID="ProjectsEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No projects assigned in this business." /></div>
</asp:Panel>

<asp:Panel ID="AccessPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("lock") %></span><div><h2>Login &amp; Access</h2><p>Manage this employee's account access, security, and login activity.</p></div></div>
 <asp:Label ID="AccessMessage" runat="server" CssClass="am-ed-empty" Visible="false" />
 <asp:PlaceHolder ID="AccessBody" runat="server">
 <div class="am-ed-access-grid">
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("user") %></span><div><strong>Account Status</strong><small>Overall account state in AnyMinute.</small></div><asp:Label ID="AccAccount" runat="server" /></div>
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("key") %></span><div><strong>Login Access</strong><small>Can this employee log in to AnyMinute?</small></div><asp:Label ID="AccLogin" runat="server" /></div>
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("send") %></span><div><strong>Invitation Status</strong><small>Status of the account invitation.</small></div><asp:Label ID="AccInvite" runat="server" /><asp:LinkButton ID="ResendInvite" runat="server" CssClass="am-ed-btn am-ed-btn-soft am-ed-btn-sm" OnClick="ResendInvite_Click" CausesValidation="false" /></div>
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("calendar") %></span><div><strong>Last Login</strong><small>Most recent successful login.</small></div><b class="am-ed-access-value"><asp:Literal ID="AccLastLogin" runat="server" /></b></div>
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("shield") %></span><div><strong>Last Password Reset</strong><small>When the password was last reset.</small><b><asp:Literal ID="AccLastReset" runat="server" /></b></div><asp:LinkButton ID="SendReset" runat="server" CssClass="am-ed-btn am-ed-btn-soft am-ed-btn-sm" OnClick="SendReset_Click" CausesValidation="false" OnClientClick="return confirm('Send a password reset email to this employee?');" /></div>
  <div class="am-ed-box am-ed-access"><span class="am-ed-icon-soft"><%= I("smartphone") %></span><div><strong>MFA / OTP Status</strong><small>Two-factor authentication (OTP) status.</small></div><asp:Label ID="AccMfa" runat="server" /></div>
  <div class="am-ed-box am-ed-access am-ed-access-wide"><span class="am-ed-icon-soft"><%= I("layers") %></span><div><strong>Active Sessions</strong><small>Currently active login sessions for this employee.</small></div><b class="am-ed-access-value"><asp:Literal ID="AccSessions" runat="server" /></b><asp:HyperLink ID="LoginHistoryLink" runat="server" CssClass="am-ed-btn am-ed-btn-soft am-ed-btn-sm" /></div>
 </div>
 <asp:Panel ID="LoginHistoryPanel" runat="server" Visible="false" CssClass="am-ed-box"><div class="am-ed-box-head"><h3>Login History</h3></div><div class="am-ed-table-wrap"><table class="am-ed-table am-ed-table-sm"><thead><tr><th>Signed In</th><th>Last Activity</th><th>Device</th><th>Ended</th></tr></thead><tbody><asp:Repeater ID="LoginHistory" runat="server"><ItemTemplate><tr><td><%#: Eval("Started") %></td><td><%#: Eval("Seen") %></td><td><%#: Eval("Device") %></td><td><%#: Eval("Ended") %></td></tr></ItemTemplate></asp:Repeater></tbody></table><asp:Label ID="LoginHistoryEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No sign-ins recorded." /></div></asp:Panel>
 </asp:PlaceHolder>
 <div class="am-ed-infobar"><%= I("info") %>All access changes (e.g., login access, password reset, MFA, invitation) are recorded in the audit log.</div>
</asp:Panel>

<asp:Panel ID="ActivityPane" runat="server" CssClass="am-ed-card am-ed-pane">
 <div class="am-ed-pane-head"><span class="am-ed-icon-round"><%= I("history") %></span><div><h2>Activity History</h2><p>Audit trail of changes, actions, and system events for this employee.</p></div></div>
 <div class="am-ed-filters am-ed-activity-filters">
  <label><span>Date Range</span><span class="am-ed-select-icon"><%= I("calendar") %><asp:DropDownList ID="ActRange" runat="server" AutoPostBack="true" CssClass="am-ed-select" /></span></label>
  <label><span>Event Type</span><asp:DropDownList ID="ActType" runat="server" AutoPostBack="true" CssClass="am-ed-select" /></label>
  <label><span>Changed By</span><span class="am-ed-select-icon"><%= I("user") %><asp:DropDownList ID="ActUser" runat="server" AutoPostBack="true" CssClass="am-ed-select" /></span></label>
  <asp:HyperLink ID="ActReset" runat="server" CssClass="am-ed-btn am-ed-btn-outline am-ed-btn-sm" />
 </div>
 <div class="am-ed-table-wrap"><table class="am-ed-table am-ed-sortable"><thead><tr><th data-sort>Date &amp; Time</th><th data-sort>Event Type</th><th>Description</th><th>Previous Value</th><th>New Value</th><th data-sort>Changed By</th><th data-sort>Related Business</th><th data-sort>Status</th></tr></thead><tbody>
 <asp:Repeater ID="Activity" runat="server"><ItemTemplate><tr><td data-v='<%# Eval("Sort") %>'><%#: Eval("When") %></td><td><span class="am-ed-event"><%# I(Convert.ToString(Eval("Icon"))) %><%#: Eval("Event") %></span></td><td><%#: Eval("Description") %></td><td><%#: Eval("Before") %></td><td><%#: Eval("After") %></td><td><%#: Eval("By") %></td><td><%#: Eval("Business") %></td><td><span class="am-ed-pill am-pill-ok"><%# I("check") %>Completed</span></td></tr></ItemTemplate></asp:Repeater>
 </tbody></table><asp:Label ID="ActivityEmpty" runat="server" CssClass="am-ed-empty" Visible="false" Text="No recorded activity for the selected filters." /></div>
</asp:Panel>
</div></div>
<div id="deactivate"><asp:Panel ID="DeactivateBox" runat="server" CssClass="am-employee-actions"><asp:Button ID="Deactivate" runat="server" Text="Deactivate Employee" CssClass="am-button secondary" Visible="false" OnClick="Deactivate_Click" OnClientClick="return confirm('Mark active employee records inactive? Existing timecards and pay history will be retained.');" /><span>Employee status and login access are managed separately.</span></asp:Panel></div>
</section>
<script>
(function(){
 // v86: client-side column sort (th[data-sort]), Upload Document toggle, and document filters.
 function val(td){var v=td.getAttribute('data-v');return v!==null?v:td.textContent.trim().toLowerCase();}
 document.querySelectorAll('table.am-ed-sortable').forEach(function(t){t.querySelectorAll('th[data-sort]').forEach(function(th,i){var idx=Array.prototype.indexOf.call(th.parentNode.children,th);th.addEventListener('click',function(){var body=t.tBodies[0],rows=Array.prototype.slice.call(body.rows),asc=th.getAttribute('data-dir')!=='asc';t.querySelectorAll('th[data-sort]').forEach(function(h){h.removeAttribute('data-dir');});th.setAttribute('data-dir',asc?'asc':'desc');rows.sort(function(a,b){var x=val(a.cells[idx]),y=val(b.cells[idx]),nx=parseFloat(x),ny=parseFloat(y);var r=(!isNaN(nx)&&!isNaN(ny)&&/^[-\d.]+$/.test(x)&&/^[-\d.]+$/.test(y))?nx-ny:x.localeCompare(y);return asc?r:-r;});rows.forEach(function(r){body.appendChild(r);});});});});
 document.querySelectorAll('[data-am-toggle]').forEach(function(b){b.addEventListener('click',function(){var el=document.getElementById(b.getAttribute('data-am-toggle'));if(el)el.classList.toggle('is-open');});});
 var f=document.querySelector('[data-am-docfilter]'),t=document.querySelector('[data-am-doctable]');
 if(f&&t){var n=f.querySelector('[data-f=name]'),ty=f.querySelector('[data-f=type]'),st=f.querySelector('[data-f=status]');function run(){var q=n.value.trim().toLowerCase();Array.prototype.forEach.call(t.tBodies[0].rows,function(r){var ok=(!q||r.getAttribute('data-name').indexOf(q)>=0)&&(!ty.value||r.getAttribute('data-type')===ty.value)&&(!st.value||r.getAttribute('data-status')===st.value);r.style.display=ok?'':'none';});}
  n.addEventListener('input',run);ty.addEventListener('change',run);st.addEventListener('change',run);f.querySelector('[data-f=clear]').addEventListener('click',function(e){e.preventDefault();n.value='';ty.value='';st.value='';run();});}
})();
</script>
</asp:Content>
