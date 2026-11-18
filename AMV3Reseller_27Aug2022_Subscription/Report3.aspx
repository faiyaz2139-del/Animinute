<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report3.aspx.cs" Inherits="Report3" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server"><title>AnyMinute - Employee Report</title>
<style>body{font-family:Arial,sans-serif;margin:24px;color:#25334a} .bar{display:flex;gap:12px;flex-wrap:wrap;align-items:end;margin:12px 0}.field{display:flex;flex-direction:column;gap:5px}input,select{padding:7px} .action{padding:8px 12px;margin:3px} .table{margin-top:15px;overflow:auto}h2{margin-bottom:12px}</style>
<link href="css/AnyminuteTypography.css?v=20261117v88" rel="stylesheet" /><link href="css/AnyminuteReference.css?v=20261117v88" rel="stylesheet" /><link href="css/AnyminuteFlat.css?v=20261117v88" rel="stylesheet" /></head>
<body class="am-reference am-typography"><form id="form1" runat="server">
<asp:ScriptManager ID="ScriptManager1" runat="server" />
<h2>Employee Time Sheet Report</h2>
<div class="bar">
<div class="field"><label>Business</label><asp:DropDownList ID="ddl_Business" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_Business_SelectedIndexChanged"><asp:ListItem Value="">Please Select</asp:ListItem></asp:DropDownList></div>
<div class="field"><label>Employee</label><asp:DropDownList ID="ddlemployee" runat="server"><asp:ListItem Value="">Please Select</asp:ListItem></asp:DropDownList></div>
<div class="field"><label>Report status</label><asp:DropDownList ID="DropDownList_Report_Status" runat="server"><asp:ListItem Value="2">Approved</asp:ListItem><asp:ListItem Value="1">Pending</asp:ListItem><asp:ListItem Value="0">All</asp:ListItem></asp:DropDownList></div>
</div>
<div class="bar">
<div class="field"><label>Range</label><asp:DropDownList ID="ddrange" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddrange_SelectedIndexChanged"><asp:ListItem Value="">Select range</asp:ListItem><asp:ListItem>Current week</asp:ListItem><asp:ListItem>Last three weeks</asp:ListItem><asp:ListItem>Last 30 days</asp:ListItem><asp:ListItem>Last 60 days</asp:ListItem><asp:ListItem>Last 90 days</asp:ListItem><asp:ListItem>Current month</asp:ListItem><asp:ListItem>Custom</asp:ListItem></asp:DropDownList></div>
<div class="field"><label>From (MM/DD/YYYY)</label><asp:TextBox ID="txtFromDate" runat="server" /></div><div class="field"><label>To (MM/DD/YYYY)</label><asp:TextBox ID="txtToDate" runat="server" /></div>
<asp:Button ID="btnshow" runat="server" Text="Update report" CssClass="action" OnClick="btnshow_Click" />
</div>
<asp:Panel ID="pnlhideandshow" runat="server">
<asp:LinkButton ID="lnkExcel" runat="server" OnClick="lnkExcel_Click">Download Excel</asp:LinkButton> &nbsp; | &nbsp;
<asp:LinkButton ID="lnkDirectPdf" runat="server" OnClick="lnkDirectPdf_Click">Download PDF</asp:LinkButton> &nbsp; | &nbsp;
<asp:LinkButton ID="lnkCsvFile" runat="server" OnClick="lnkCsvFile_Click">Download CSV</asp:LinkButton> &nbsp; | &nbsp;
<asp:LinkButton ID="lnkbtn" runat="server" OnClick="lnkbtn_Click">Compare date ranges</asp:LinkButton>
</asp:Panel>
<div id="divrange2" runat="server" style="display:none">
<div class="bar"><div class="field"><label>Second range</label><asp:DropDownList ID="ddrange2" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddrange2_SelectedIndexChanged"><asp:ListItem Value="">Select range</asp:ListItem><asp:ListItem>Current week</asp:ListItem><asp:ListItem>Last three weeks</asp:ListItem><asp:ListItem>Last 30 days</asp:ListItem><asp:ListItem>Last 60 days</asp:ListItem><asp:ListItem>Last 90 days</asp:ListItem><asp:ListItem>Current month</asp:ListItem><asp:ListItem>Custom</asp:ListItem></asp:DropDownList></div>
<div class="field"><label>From (MM/DD/YYYY)</label><asp:TextBox ID="txtFromDate2" runat="server" /></div><div class="field"><label>To (MM/DD/YYYY)</label><asp:TextBox ID="txtToDate2" runat="server" /></div>
<asp:ImageButton ID="ImageButton2" runat="server" AlternateText="Close comparison" OnClick="ImageButton2_Click" ImageUrl="~/Assets/assets/img/logo1.jpg" Width="24" Height="24" /></div>
</div>
<asp:HiddenField ID="hdnvisible" runat="server" Value="NO" /><asp:HiddenField ID="hdnrange" runat="server"/><asp:HiddenField ID="hdnrange2" runat="server"/><asp:HiddenField ID="hdnDate" runat="server"/><asp:HiddenField ID="hdnDate1" runat="server"/><asp:HiddenField ID="hdndate3" runat="server"/><asp:HiddenField ID="hdndate4" runat="server"/><asp:HiddenField ID="hdnfrmdt" runat="server"/><asp:HiddenField ID="hdntodt" runat="server"/>
<div class="table">
<asp:GridView ID="GVBusiness" runat="server" AutoGenerateColumns="true" AllowPaging="true" PageSize="25" OnPageIndexChanging="GVBusiness_PageIndexChanging" OnPreRender="GVBusiness_PreRender" OnRowCommand="GVBusiness_RowCommand" OnRowDataBound="GVBusiness_RowDataBound">
<Columns><asp:TemplateField HeaderText="Download / approval"><ItemTemplate>
<asp:HiddenField ID="hdncomid" runat="server" Value='<%# Eval("BusinessID") %>' />
<asp:HiddenField ID="hfdEmpid" runat="server" Value='<%# Eval("EmployeeID") %>' />
<asp:Label ID="lblEmployeeName" runat="server" Text='<%# Eval("CompanyName") %>' />
<asp:DropDownList ID="ddapproval" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddapproval_SelectedIndexChanged"><asp:ListItem Value="">Choose</asp:ListItem><asp:ListItem Value="1">Excel</asp:ListItem><asp:ListItem Value="2">Refresh</asp:ListItem></asp:DropDownList>
<asp:LinkButton ID="rowExcel" runat="server" CommandName="Excel" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>Excel</asp:LinkButton> |
<asp:LinkButton ID="rowPDF" runat="server" CommandName="PDF" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>PDF</asp:LinkButton> |
<asp:LinkButton ID="rowCSV" runat="server" CommandName="CSV" CommandArgument='<%# ((GridViewRow)Container).RowIndex %>'>CSV</asp:LinkButton>
</ItemTemplate></asp:TemplateField></Columns></asp:GridView>
<asp:GridView ID="GVCompare1" runat="server" AutoGenerateColumns="true" AllowPaging="true" PageSize="25" OnPageIndexChanging="GVCompare1_PageIndexChanging" OnPreRender="GVCompare1_PreRender" />
<asp:GridView ID="GVCompare2" runat="server" AutoGenerateColumns="true" AllowPaging="true" PageSize="25" OnPageIndexChanging="GVCompare2_PageIndexChanging" OnPreRender="GVCompare2_PreRender" />
</div>
<div style="display:none"><asp:GridView ID="GvExport" runat="server" AutoGenerateColumns="true" /><asp:GridView ID="GvExport1" runat="server" AutoGenerateColumns="true" /></div>
</form></body></html>
