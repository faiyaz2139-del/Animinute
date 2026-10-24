<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeAssignment.aspx.cs" Inherits="AnyminuteEmployeeAssignmentPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-dialog-page am-employees-v2">
 <header class="am-dialog-head"><div><h1>Edit Business Assignment</h1><p>Manage the businesses, roles and pay for this employee.</p></div>
  <div class="am-check-row"><asp:LinkButton ID="AddAssignment" runat="server" OnClick="Add_Click" CausesValidation="false" CssClass="am-button"><span aria-hidden="true">+</span> Add Assignment</asp:LinkButton><asp:HyperLink ID="CloseLink" runat="server" CssClass="am-dialog-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></div></header>
 <div class="am-dialog-body">
  <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <asp:Label ID="SetupNote" runat="server" Visible="false" CssClass="am-note" />
  <div class="am-section"><h2>Employee Information</h2><div class="am-info-card" style="margin:0"><span class="am-avatar"><asp:Literal ID="PersonInitials" runat="server" /></span><div class="am-info-main"><strong><asp:Literal ID="Person" runat="server" /></strong><div class="am-info-meta"><span>&#9993; <asp:Literal ID="PersonEmail" runat="server" /></span><span>&#9742; <asp:Literal ID="PersonPhone" runat="server" /></span></div></div><asp:Label ID="PersonStatus" runat="server" /></div></div>
  <div class="am-section"><h2>Existing Business Assignments (<asp:Literal ID="AssignmentCount" runat="server" />)</h2>
   <div class="am-table-wrap"><table class="am-data"><thead><tr><th>Business</th><th>Roles</th><th>Employee Type</th><th>Pay Type</th><th>Pay Rate</th><th>Effective Date</th><th>End Date</th><th>Status</th><th>Action</th></tr></thead><tbody>
   <asp:Repeater ID="Existing" runat="server" OnItemCommand="Existing_ItemCommand"><ItemTemplate><tr><td><strong><%#: Eval("Business") %></strong></td><td><div class="am-role-chips"><asp:Repeater runat="server" DataSource='<%# Eval("Roles") %>'><ItemTemplate><span class='<%# "am-role-chip " + AnyminuteEmployeeEditor.RoleCss(Container.DataItem) %>'><%#: Container.DataItem %></span></ItemTemplate></asp:Repeater></div></td><td><%#: Eval("EmployeeType") %></td><td><%#: Eval("PayType") %></td><td><%#: Eval("PayRateText") %></td><td><%#: Eval("EffectiveText") %></td><td><%#: Eval("EndText") %></td><td><span class='<%# AnyminuteEmployeeEditor.StatusPill(Eval("Status")) %>'><%#: Eval("Status") %></span></td>
   <td style="white-space:nowrap"><asp:LinkButton runat="server" CommandName="EditRow" CommandArgument='<%# Eval("BusinessId") %>' CausesValidation="false" CssClass="am-act" ToolTip="Edit assignment">&#9998;</asp:LinkButton><asp:LinkButton runat="server" CommandName="DeleteRow" CommandArgument='<%# Eval("BusinessId") %>' CausesValidation="false" CssClass="am-act am-act-del" ToolTip="Remove assignment" OnClientClick="return confirm('Remove this business assignment? Time cards and history are kept.');">&#128465;</asp:LinkButton></td></tr></ItemTemplate></asp:Repeater>
   </tbody></table></div></div>
 </div>
</section>
<asp:Panel ID="Editor" runat="server" Visible="false" CssClass="am-modal-shade" role="dialog" aria-modal="true" aria-labelledby="am-assign-title" DefaultButton="SaveAssignment">
 <div class="am-dialog-page">
  <header class="am-dialog-head"><div><h1 id="am-assign-title"><asp:Literal ID="EditorTitle" runat="server" /></h1><p><asp:Literal ID="EditorSubtitle" runat="server" /></p></div><asp:LinkButton runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="am-dialog-close" ToolTip="Close">&times;</asp:LinkButton></header>
  <div class="am-dialog-body">
   <asp:Label ID="EditorError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
   <div class="am-info-card"><span class="am-avatar"><asp:Literal ID="EditorInitials" runat="server" /></span><div class="am-info-main"><strong><asp:Literal ID="EditorName" runat="server" /></strong><div class="am-info-meta"><span><asp:Literal ID="EditorEmail" runat="server" /></span><span><asp:Literal ID="EditorPhone" runat="server" /></span></div></div></div>
   <div class="am-section"><h2>Assignment Details</h2><div class="am-grid am-cols-2">
    <label class="am-field"><span>Business <span class="am-req">*</span></span><asp:DropDownList ID="Business" runat="server" AutoPostBack="true" OnSelectedIndexChanged="BusinessChanged" /></label>
    <div class="am-field"><span>Assign Roles <span class="am-req">*</span></span><asp:CheckBoxList ID="Roles" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="am-check-row" /><span class="am-hint">The first ticked role is the main role used for permissions.</span></div>
    <label class="am-field"><span>Employee Type <span class="am-req">*</span></span><asp:DropDownList ID="EmployeeType" runat="server"><asp:ListItem>Regular</asp:ListItem><asp:ListItem>Project</asp:ListItem></asp:DropDownList></label>
    <label class="am-field"><span>Pay Type <span class="am-req">*</span></span><asp:DropDownList ID="PayType" runat="server" /></label>
    <label class="am-field"><span>Pay Rate <span class="am-req">*</span></span><span class="am-money"><asp:TextBox ID="PayRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Billing Rate <span class="am-hint">(Optional)</span></span><span class="am-money"><asp:TextBox ID="BillingRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Effective Date <span class="am-req">*</span></span><asp:TextBox ID="EffectiveDate" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>End Date <span class="am-hint">(Optional)</span></span><asp:TextBox ID="EndDate" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>Assignment Status <span class="am-req">*</span></span><asp:DropDownList ID="AssignmentStatus" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Not Active</asp:ListItem><asp:ListItem>Terminated</asp:ListItem></asp:DropDownList></label>
   </div></div>
   <div class="am-section"><h2>Additional Settings <span class="am-hint">(Optional)</span></h2><div class="am-grid am-cols-2">
    <label class="am-field"><span>Default Project</span><asp:DropDownList ID="DefaultProject" runat="server" /></label>
    <label class="am-field am-span-2"><span>Notes</span><asp:TextBox ID="Notes" runat="server" TextMode="MultiLine" MaxLength="500" ClientIDMode="Static" placeholder="Add any notes about this assignment..." /><span class="am-counter"><span id="am-notes-count">0</span>/500</span></label>
   </div></div>
  </div>
  <footer class="am-dialog-foot"><asp:LinkButton runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="am-button secondary">Cancel</asp:LinkButton><asp:LinkButton ID="SaveAssignment" runat="server" OnClick="Save_Click" CssClass="am-button"><asp:Literal ID="SaveText" runat="server" /></asp:LinkButton></footer>
 </div>
</asp:Panel>
<script>(function(){var n=document.getElementById('Notes'),o=document.getElementById('am-notes-count');if(!n||!o)return;function u(){if(n.value.length>500)n.value=n.value.slice(0,500);o.textContent=n.value.length;}n.addEventListener('input',u);u();var f=n.closest('.am-modal-shade');if(f){var first=f.querySelector('select,input');if(first)first.focus();}})();</script>
</asp:Content>
