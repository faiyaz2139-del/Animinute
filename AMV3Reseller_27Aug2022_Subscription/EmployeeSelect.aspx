<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="EmployeeSelect.aspx.cs" Inherits="AnyminuteEmployeeSelectPage" Title="Edit Employee" %>
<%-- 2026-11-07 v70 (docx s3): Employees > Edit Employee opens this picker (subscriber's own employees only); the selected
     employee opens in the new EmployeeEditor.aspx?EmployeeId=... instead of the old AddEditRole_Add_Employee.aspx page. --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteEmployees.css?v=20261025v13" rel="stylesheet" /><link href="css/AnyminuteEmployeesV2.css?v=20261026v17" rel="stylesheet" />
<style>
.am-empsel{max-width:760px}
.am-empsel .am-empsel-f{display:flex;flex-direction:column;gap:6px;margin:0 0 16px}
.am-empsel .am-empsel-f>label{font-weight:700;font-size:14px;color:#0f172a}
.am-empsel .am-empsel-f .am-req{color:#e5282a}
.am-empsel input[type=search],.am-empsel select{width:100%;height:42px;border:1px solid #cfd8e3;border-radius:8px;padding:0 12px;font-size:14px;background:#fff;box-sizing:border-box}
.am-empsel input:focus,.am-empsel select:focus{border-color:#1a5fd6;outline:0;box-shadow:0 0 0 3px rgba(26,95,214,.15)}
.am-empsel .am-empsel-count{color:#5f7389;font-size:13px}
.am-empsel .am-empsel-empty{padding:18px;border:1px dashed #cfd8e3;border-radius:10px;color:#5f7389;text-align:center}
.am-empsel .am-empsel-err{display:block;margin:0 0 14px;padding:10px 14px;border:1px solid #f5c2c0;background:#fdecea;color:#9b1c1c;border-radius:8px}
.am-empsel .am-dialog-foot .am-button[disabled]{opacity:.55;cursor:not-allowed}
</style></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-dialog-page am-employees-v2 am-empsel">
 <header class="am-dialog-head"><div><h1>Edit Employee</h1><p>Select an employee to open and update their details.</p></div><asp:HyperLink runat="server" NavigateUrl="~/Users.aspx" CssClass="am-dialog-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <div class="am-dialog-body">
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-empsel-err" role="alert" />
  <asp:Panel ID="PickPanel" runat="server">
   <div class="am-empsel-f"><label for="amEmpSelSearch">Search Employee</label><input type="search" id="amEmpSelSearch" placeholder="Search by name, email or phone" autocomplete="off" /></div>
   <div class="am-empsel-f"><label for="EmployeeList">Select Employee <span class="am-req">*</span></label>
    <asp:DropDownList ID="EmployeeList" runat="server" ClientIDMode="Static" />
    <span class="am-empsel-count" id="amEmpSelCount"><asp:Literal ID="CountText" runat="server" /></span></div>
  </asp:Panel>
  <asp:Panel ID="EmptyPanel" runat="server" Visible="false" CssClass="am-empsel-empty">No employees found for your businesses. <a href="EmployeeEditor.aspx">Add Employee</a></asp:Panel>
 </div>
 <div class="am-dialog-foot"><a class="am-button secondary" href="Users.aspx">Cancel</a><asp:Button ID="EditSelected" runat="server" Text="Edit Employee" CssClass="am-button" OnClick="EditSelected_Click" ClientIDMode="Static" /></div>
</section>
<script>
(function(){var sel=document.getElementById('EmployeeList'),q=document.getElementById('amEmpSelSearch'),btn=document.getElementById('EditSelected'),cnt=document.getElementById('amEmpSelCount');if(!sel||!q)return;
var all=[].slice.call(sel.options).map(function(o){return{v:o.value,t:o.text,k:(o.getAttribute('data-k')||o.text).toLowerCase()};});
function sync(){if(btn)btn.disabled=!sel.value;}
function draw(){var term=q.value.trim().toLowerCase(),keep=sel.value,n=0;sel.innerHTML='';all.forEach(function(a){if(a.v&&term&&a.k.indexOf(term)<0)return;var o=new Option(a.t,a.v);if(a.v){n++;}sel.add(o);});
if([].some.call(sel.options,function(o){return o.value===keep;}))sel.value=keep;else sel.value='';if(cnt)cnt.textContent=n+(n===1?' employee':' employees')+(term?' match your search':'');sync();}
q.addEventListener('input',draw);sel.addEventListener('change',sync);sel.addEventListener('dblclick',function(){if(sel.value&&btn)btn.click();});sync();})();
</script>
</asp:Content>
