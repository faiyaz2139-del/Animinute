<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="BusinessUsers.aspx.cs" Inherits="BusinessUsers" %>
<asp:Content ID="UserHead" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="UserBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <h2>Edit User</h2>
  <p>Select a user from this business to edit their details.</p>
  <asp:HyperLink ID="Back" runat="server" NavigateUrl="~/Home_Page.aspx" Text="Back to businesses" />
  <asp:GridView ID="Users" runat="server" AutoGenerateColumns="false" CssClass="table" EmptyDataText="No users assigned to this business.">
    <Columns>
      <asp:BoundField DataField="empname" HeaderText="User" HtmlEncode="true" />
      <asp:BoundField DataField="EmployeeID" HeaderText="Employee ID" />
      <asp:HyperLinkField Text="Edit User" DataNavigateUrlFields="EmployeeID,BusinessID" DataNavigateUrlFormatString="AddEditRole_Add_Employee.aspx?EmployeeIDEdit={0}&amp;BussEditId={1}&amp;BusinessScoped=1" />
    </Columns>
  </asp:GridView>
</asp:Content>
