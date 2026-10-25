<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="RateHistory.aspx.cs" Inherits="AnyminuteRateHistoryPage" Title="Rate History" %>
<%-- 2026-10-24 v10: Rate History Module (deck H). Opened from "Update Rate" links (Change_Rate_EmployeeV2.aspx redirects here). --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteRateHistory.css?v=20261024v10" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-rate-module" aria-labelledby="am-rate-title">
 <header class="am-rm-head"><div><h1 id="am-rate-title">Rate History Module</h1><p>Manage current and historical pay rates for one employee and business</p></div><asp:HyperLink ID="BackLink" runat="server" NavigateUrl="~/Users.aspx" CssClass="am-rm-btn am-rm-ghost">Back to Employees</asp:HyperLink></header>
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />

 <div class="am-rm-card am-rm-person">
  <span class="am-rm-avatar" aria-hidden="true"><asp:Literal ID="Initials" runat="server" /></span>
  <div class="am-rm-who"><div><strong><asp:Literal ID="EmployeeName" runat="server" /></strong> <asp:Label ID="StatusBadge" runat="server" CssClass="am-rm-badge" /></div><span class="am-rm-muted"><asp:Literal ID="EmployeeRole" runat="server" /></span></div>
  <span class="am-rm-meta"><svg viewBox="0 0 24 24" aria-hidden="true"><rect x="3" y="5" width="18" height="14" rx="2"/><path d="M3 7l9 6 9-6"/></svg><asp:Literal ID="EmployeeEmail" runat="server" /></span>
  <span class="am-rm-meta"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 4h4l2 5-2.5 1.5a11 11 0 0 0 5 5L15 13l5 2v4a2 2 0 0 1-2 2A16 16 0 0 1 3 6a2 2 0 0 1 2-2"/></svg><asp:Literal ID="EmployeePhone" runat="server" /></span>
  <label class="am-rm-business"><svg viewBox="0 0 24 24" aria-hidden="true"><rect x="4" y="3" width="16" height="18" rx="1"/><path d="M9 7h2M13 7h2M9 11h2M13 11h2M10 21v-4h4v4"/></svg><span><span class="am-rm-muted">Business</span><asp:DropDownList ID="HeadBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="HeadBusinessChanged" aria-label="Business" /></span></label>
 </div>

 <div class="am-rm-card">
  <h2><asp:Literal ID="FormTitle" runat="server" Text="Current Pay Rate" /></h2>
  <p class="am-rm-muted"><asp:Literal ID="FormHint" runat="server" /></p>
  <div class="am-rm-form">
   <label class="am-rm-field"><span>Rate Type <em>(Required)</em></span><asp:DropDownList ID="RateType" runat="server" /></label>
   <label class="am-rm-field"><span><asp:Literal ID="RateLabel" runat="server" Text="Current Pay Rate" /> <em>(Required)</em></span><span class="am-rm-money"><span aria-hidden="true">$</span><asp:TextBox ID="RateValue" runat="server" TextMode="Number" step="0.01" min="0.01" max="1000000" /></span></label>
   <label class="am-rm-field am-datefmt"><span>Effective Date <em>(Required)</em></span><asp:TextBox ID="RateEffective" runat="server" TextMode="Date" /></label>
   <label class="am-rm-field am-rm-reason"><span>Reason for Change <em>(Required)</em></span><asp:TextBox ID="RateReason" runat="server" TextMode="MultiLine" Rows="2" MaxLength="200" ClientIDMode="Static" placeholder="e.g. Increase in hourly pay rate." /><span class="am-rm-counter" id="am-rm-left" aria-live="polite">200</span></label>
  </div>
  <div class="am-rm-actions"><asp:LinkButton ID="SaveRate" runat="server" OnClick="SaveRate_Click" CssClass="am-rm-btn"><asp:Literal ID="SaveText" runat="server" Text="Update Current Rate" /></asp:LinkButton><asp:LinkButton ID="CancelRate" runat="server" OnClick="CancelRate_Click" CausesValidation="false" CssClass="am-rm-btn am-rm-ghost">Cancel</asp:LinkButton></div>
 </div>

 <div class="am-rm-card">
  <div class="am-rm-section-head"><div><h2>Rate History</h2><p class="am-rm-muted">View the employee's pay rate history for the selected business. Historical rates are read-only.</p></div></div>
  <div class="am-rm-filter"><label class="am-rm-field am-rm-inline"><span>Select Business <em>(Required)</em></span><asp:DropDownList ID="RateBusiness" runat="server" AutoPostBack="true" OnSelectedIndexChanged="RateBusinessChanged" /></label><asp:LinkButton ID="AddNewRate" runat="server" OnClick="AddNewRate_Click" CausesValidation="false" CssClass="am-rm-btn"><span aria-hidden="true">+</span>&nbsp; Add New Rate</asp:LinkButton></div>
  <div class="am-rm-table"><table><thead><tr><th>#</th><th>Rate Type</th><th>Start Date</th><th>End Date</th><th>Pay Rate</th><th>Status</th><th>Action</th></tr></thead><tbody>
  <asp:Repeater ID="RateRows" runat="server" OnItemCommand="RateRows_ItemCommand"><ItemTemplate><tr><td><%# Eval("No") %></td><td><%#: Eval("RateType") %></td><td><%#: Eval("StartIso") %></td><td><%#: Eval("EndIso") %></td><td><%#: Eval("RateText") %></td><td><span class='<%# (bool)Eval("Current") ? "am-rm-pill am-rm-current" : "am-rm-pill am-rm-hist" %>'><%# (bool)Eval("Current") ? "Current" : "Historical" %></span></td><td><asp:LinkButton runat="server" Visible='<%# (bool)Eval("Current") %>' CommandName="EditCurrent" CausesValidation="false" CssClass="am-rm-edit"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 20h4L19 9l-4-4L4 16z"/></svg>Edit</asp:LinkButton><span class="am-rm-locked" title="Historical rates are locked" runat="server" visible='<%# !(bool)Eval("Current") %>'><svg viewBox="0 0 24 24" aria-hidden="true"><rect x="5" y="10" width="14" height="11" rx="2"/><path d="M8 10V7a4 4 0 0 1 8 0v3"/></svg>Locked</span></td></tr></ItemTemplate></asp:Repeater>
  <asp:PlaceHolder ID="NoRates" runat="server" Visible="false"><tr><td colspan="7" class="am-rm-empty">No pay rates are recorded for this business yet. Use Add New Rate to create one.</td></tr></asp:PlaceHolder>
  </tbody></table></div>
 </div>

 <div class="am-rm-info"><span class="am-rm-info-icon" aria-hidden="true"><svg viewBox="0 0 24 24"><rect x="5" y="10" width="14" height="11" rx="2"/><path d="M8 10V7a4 4 0 0 1 8 0v3"/></svg></span><div><strong>Important Information About Pay Rates</strong><p>Current pay rate can be edited to update the employee's rate. Historical rates are locked and read-only. Use "Add New Rate" for future changes. Any corrections to historical data require a controlled, audited process.</p>
  <ul><li>The current rate is editable and will take effect from the selected effective date.</li><li>Use Add New Rate to create a new rate for future changes.</li><li>Historical rates are locked and read-only (no edit or delete).</li><li>Corrections to historical data require a controlled, audited process.</li></ul></div></div>
</section>
<script>(function(){var t=document.getElementById('RateReason'),o=document.getElementById('am-rm-left');if(!t||!o)return;function u(){o.textContent=String(200-t.value.length);}t.addEventListener('input',u);u();})();</script>
</asp:Content>
