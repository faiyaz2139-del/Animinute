<%@ Page Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="My_Time_Report.aspx.cs" Inherits="My_Time_Report" %>
<asp:Content ID="ReportHead" ContentPlaceHolderID="head" runat="server"><title>My Timesheet Report</title></asp:Content>
<asp:Content ID="ReportBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<h2>My Timesheet Report</h2>
<p>This report contains only your saved timesheets.</p>
<asp:Label runat="server" AssociatedControlID="Businesses" Text="Business" />
<asp:DropDownList ID="Businesses" runat="server" />
<asp:Label runat="server" AssociatedControlID="FromDate" Text="From" />
<asp:TextBox ID="FromDate" runat="server" TextMode="Date" />
<asp:Label runat="server" AssociatedControlID="ToDate" Text="To" />
<asp:TextBox ID="ToDate" runat="server" TextMode="Date" />
<asp:Button ID="ShowReport" runat="server" Text="Update Report" OnClick="ShowReport_Click" />
<asp:Button ID="DownloadReport" runat="server" Text="Download CSV (Excel)" OnClick="DownloadReport_Click" />
<p><asp:Label ID="Message" runat="server" role="status" /></p>
<asp:GridView ID="Results" runat="server" AutoGenerateColumns="true" CssClass="table table-striped" EmptyDataText="No saved timesheets in this date range." />
</asp:Content>
