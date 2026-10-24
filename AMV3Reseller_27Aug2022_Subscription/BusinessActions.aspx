<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="BusinessActions.aspx.cs" Inherits="AnyminuteBusinessActions" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-card"><h1><asp:Literal ID="ActionTitle" runat="server" /></h1><p>Select a business you own to continue.</p>
<div class="am-toolbar"><label>Business<asp:DropDownList ID="Business" runat="server" /></label><asp:Button ID="Continue" runat="server" Text="Continue" CssClass="am-button" OnClick="Continue_Click" /></div>
<asp:Label ID="Message" runat="server" role="alert" /><a href="Dashboard.aspx?view=businesses">Back to Businesses</a></section>
</asp:Content>
