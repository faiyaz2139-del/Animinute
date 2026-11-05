<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="BusinessAccountant.aspx.cs" Inherits="BusinessAccountant" %>
<%-- v65 (PPT s4/s5): the old Assign Accountant screen is removed. This URL only redirects to Accountant & Payroll Access. --%>
<asp:Content ID="AccountantHead" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="AccountantBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"><p><a href="AccountantAccess.aspx">Open Accountant &amp; Payroll Access</a></p></asp:Content>
