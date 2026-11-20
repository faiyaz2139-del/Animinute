<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeAssignment.aspx.cs" Inherits="AnyminuteEmployeeAssignmentPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261120v93" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261120v93" rel="stylesheet" /><link href="css/AnyminuteFlash.css?v=20261120v93" rel="stylesheet" />
<style>
.am-jobtitle-row{display:flex;gap:6px;align-items:center;flex-wrap:nowrap;width:100%;min-width:0}
.am-field .am-jobtitle-row select{flex:1 1 auto;min-width:0;width:auto;min-height:38px;height:38px;border:1px solid #cfd9e5;border-radius:7px;padding:7px 10px;background:#fff;box-shadow:none}
.am-jobtitle-row .am-jobtitle-btn{display:inline-flex;align-items:center;justify-content:center;gap:4px;white-space:nowrap;flex:0 0 auto;box-sizing:border-box;height:26px;min-height:26px;max-height:26px;padding:0 8px;margin:0;font-size:12px;font-weight:600;line-height:1;border-radius:5px;text-decoration:none;box-shadow:none;background:#fff}
.am-jobtitle-row .am-jobtitle-btn-add{border:1px solid #1768e8;color:#1768e8!important;background:#fff!important}
.am-jobtitle-row .am-jobtitle-btn-muted{border:1px solid #c5ced8;color:#5e6a78!important;background:#fff!important}
.am-jobtitle-editor{display:flex;gap:8px;align-items:center;flex-wrap:wrap;margin-top:8px}
.am-jobtitle-editor input{flex:1 1 220px;min-height:38px;border:1px solid #cfd9e5;border-radius:7px;padding:7px 10px;background:#fff}
.am-jobtitle-editor .am-button{min-height:38px;padding:0 12px;margin:0}
</style>
</asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- v86: Add / Edit Business Assignment is a full page (no pop-up): the list below is hidden while the editor is open. --%>
<asp:Panel ID="AssignList" runat="server">
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
</asp:Panel>
<asp:Panel ID="Editor" runat="server" Visible="false" CssClass="am-editor-page am-employees-v2" DefaultButton="SaveAssignment">
 <div class="am-backbar"><asp:LinkButton ID="EditorBack" runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="am-back"><span aria-hidden="true">&larr;</span> Back</asp:LinkButton></div>
 <div class="am-dialog-page">
  <header class="am-dialog-head"><div><h1 id="am-assign-title"><asp:Literal ID="EditorTitle" runat="server" /></h1><p><asp:Literal ID="EditorSubtitle" runat="server" /></p></div></header>
  <div class="am-dialog-body">
   <asp:Label ID="EditorError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
   <asp:PlaceHolder ID="EditorPersonCard" runat="server">
   <div class="am-info-card"><span class="am-avatar"><asp:Literal ID="EditorInitials" runat="server" /></span><div class="am-info-main"><strong><asp:Literal ID="EditorName" runat="server" /></strong><div class="am-info-meta"><span><asp:Literal ID="EditorEmail" runat="server" /></span><span><asp:Literal ID="EditorPhone" runat="server" /></span></div></div></div>
   </asp:PlaceHolder>
   <div class="am-section"><h2>Assignment Details</h2><div class="am-grid am-cols-2">
    <label class="am-field"><span>Business <span class="am-req">*</span></span><asp:DropDownList ID="Business" runat="server" AutoPostBack="true" OnSelectedIndexChanged="BusinessChanged" /></label>
    <div class="am-field"><span>Assign Role <asp:PlaceHolder ID="RolesPlural" runat="server">s</asp:PlaceHolder> <span class="am-req">*</span></span><asp:CheckBoxList ID="Roles" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="am-check-row" /><asp:PlaceHolder ID="RolesHint" runat="server"><span class="am-hint">The first ticked role is the main role used for permissions.</span></asp:PlaceHolder></div>
    <asp:PlaceHolder ID="JobTitleRow" runat="server">
    <div class="am-field"><span>Job Title <span class="am-req">*</span></span>
     <div class="am-jobtitle-row"><asp:DropDownList ID="JobTitleList" runat="server" /><asp:LinkButton ID="JobTitleAdd" runat="server" OnClick="JobTitleAdd_Click" CausesValidation="false" CssClass="am-jobtitle-btn am-jobtitle-btn-add"><span aria-hidden="true">+</span> Add</asp:LinkButton><asp:LinkButton ID="JobTitleEdit" runat="server" OnClick="JobTitleEdit_Click" CausesValidation="false" CssClass="am-jobtitle-btn am-jobtitle-btn-muted">&#9998; Edit</asp:LinkButton><asp:LinkButton ID="JobTitleDelete" runat="server" OnClick="JobTitleDelete_Click" CausesValidation="false" CssClass="am-jobtitle-btn am-jobtitle-btn-muted">&#128465; Delete</asp:LinkButton></div>
     <asp:Panel ID="JobTitleEditor" runat="server" Visible="false" CssClass="am-jobtitle-editor"><asp:TextBox ID="JobTitleName" runat="server" MaxLength="150" /><asp:LinkButton ID="JobTitleSave" runat="server" OnClick="JobTitleSave_Click" CausesValidation="false" CssClass="am-button">Save</asp:LinkButton><asp:LinkButton ID="JobTitleCancel" runat="server" OnClick="JobTitleCancel_Click" CausesValidation="false" CssClass="am-button secondary">Cancel</asp:LinkButton></asp:Panel>
    </div>
    <div class="am-jobtitle-spacer" aria-hidden="true"></div>
    </asp:PlaceHolder>
    <label class="am-field"><span>Employee Type <span class="am-req">*</span></span><asp:DropDownList ID="EmployeeType" runat="server"><asp:ListItem>Regular</asp:ListItem><asp:ListItem>Project</asp:ListItem></asp:DropDownList></label>
    <label class="am-field"><span>Pay Type <span class="am-req">*</span></span><asp:DropDownList ID="PayType" runat="server" /></label>
    <label class="am-field"><span>Pay Rate <span class="am-req">*</span></span><span class="am-money"><asp:TextBox ID="PayRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Billing Rate <span class="am-hint">(Optional)</span></span><span class="am-money"><asp:TextBox ID="BillingRate" runat="server" TextMode="Number" step="0.01" min="0" placeholder="0.00" /></span></label>
    <label class="am-field"><span>Effective Date <span class="am-req">*</span></span><asp:TextBox ID="EffectiveDate" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>End Date <span class="am-hint">(Optional)</span></span><asp:TextBox ID="EndDate" runat="server" TextMode="Date" /></label>
    <label class="am-field"><span>Assignment Status <span class="am-req">*</span></span><asp:DropDownList ID="AssignmentStatus" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Not Active</asp:ListItem><asp:ListItem>Terminated</asp:ListItem></asp:DropDownList></label>
    <asp:PlaceHolder ID="AddProjectField" runat="server">
    <label class="am-field"><span>Default Project</span><asp:DropDownList ID="DefaultProjectAdd" runat="server" /></label>
    <label class="am-field am-span-2"><span>Notes</span><asp:TextBox ID="NotesAdd" runat="server" TextMode="MultiLine" MaxLength="500" ClientIDMode="Static" placeholder="Add any notes about this assignment..." /><span class="am-counter"><span id="am-notes-count-add">0</span>/500</span></label>
    </asp:PlaceHolder>
   </div></div>
   <asp:PlaceHolder ID="EditExtraSection" runat="server">
   <div class="am-section"><h2>Additional Settings <span class="am-hint">(Optional)</span></h2><div class="am-grid am-cols-2">
    <label class="am-field"><span>Default Project</span><asp:DropDownList ID="DefaultProject" runat="server" /></label>
    <label class="am-field am-span-2"><span>Notes</span><asp:TextBox ID="Notes" runat="server" TextMode="MultiLine" MaxLength="500" ClientIDMode="Static" placeholder="Add any notes about this assignment..." /><span class="am-counter"><span id="am-notes-count">0</span>/500</span></label>
   </div></div>
   </asp:PlaceHolder>
  </div>
  <footer class="am-dialog-foot"><asp:LinkButton runat="server" OnClick="Cancel_Click" CausesValidation="false" CssClass="am-button secondary">Cancel</asp:LinkButton><asp:LinkButton ID="SaveAssignment" runat="server" OnClick="Save_Click" CssClass="am-button"><asp:Literal ID="SaveText" runat="server" /></asp:LinkButton></footer>
 </div>
</asp:Panel>
<script>(function(){function hook(id,countId){var n=document.getElementById(id),o=document.getElementById(countId);if(!n||!o)return;function u(){if(n.value.length>500)n.value=n.value.slice(0,500);o.textContent=n.value.length;}n.addEventListener('input',u);u();}hook('Notes','am-notes-count');hook('NotesAdd','am-notes-count-add');var f=document.querySelector('.am-editor-page');if(f){var first=f.querySelector('select,input');if(first)first.focus();}})();</script>
</asp:Content>
