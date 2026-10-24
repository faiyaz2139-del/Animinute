<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="BusinessAccountant.aspx.cs" Inherits="BusinessAccountant" %>
<asp:Content ID="AccountantHead" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="AccountantBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <h2>Assign Accountant</h2>
  <asp:HyperLink ID="Back" runat="server" NavigateUrl="~/Home_Page.aspx" Text="Back to businesses" />
  <p>Assign an existing accountant to this business. Use Assign Accountant to grant access, or Unassign below to remove access to this business.</p>
  <div class="am-accountant-form"><label for="Accountants">Accountant</label><asp:DropDownList ClientIDMode="Static" ID="Accountants" runat="server" CssClass="form-control" />
  <asp:Button ID="Assign" runat="server" Text="Assign Accountant" OnClick="Assign_Click" />
  </div><asp:Label ID="Result" runat="server" />
  <asp:GridView ID="Assigned" runat="server" AutoGenerateColumns="false" CssClass="table" EmptyDataText="No accountant assigned." OnRowCommand="Assigned_RowCommand">
    <Columns>
      <asp:BoundField DataField="empname" HeaderText="Accountant" HtmlEncode="true" />
      <asp:TemplateField><ItemTemplate>
        <asp:LinkButton ID="Remove" runat="server" Text="Unassign" CommandName="Unassign" CommandArgument='<%# Eval("USERID") %>' OnClientClick="return confirm('Remove this accountant’s access to this business?');" />
      </ItemTemplate></asp:TemplateField>
    </Columns>
  </asp:GridView>
</asp:Content>
