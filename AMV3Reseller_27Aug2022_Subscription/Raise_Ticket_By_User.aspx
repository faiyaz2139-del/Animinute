<%@ Page Title="Create Ticket" Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="Raise_Ticket_By_User.aspx.cs" Inherits="Raise_Ticket_By_User" %>
<%-- 2026-10-28 v27: moved to Modern.master; Page_Load no longer reads Session["NameOfLoggedInUser"] (set only by Main.master AFTER the
     content Page_Load, so it was null on the first visit -> NullReferenceException -> "SOMETHING WENT WRONG" for every role). --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="am-title"><div><h1>Create Ticket</h1><p>Tell us about the problem and our support team will get back to you.</p></div>
 <div class="am-actions"><a id="A2" runat="server" href="Ticket_List.aspx" class="am-button secondary">View Tickets</a><a id="A1" runat="server" href="Dashboard.aspx" class="am-button secondary">Back</a></div></div>
<asp:Label ID="Notice" runat="server" Visible="false" CssClass="am-notice" role="status" />
<section class="am-card">
 <div class="am-fields">
  <label>Full Name *<asp:TextBox ID="txt_fullname" required="true" runat="server" MaxLength="150" /></label>
  <label>Email Address *<asp:TextBox ID="txt_Email" required="true" runat="server" MaxLength="150" /></label>
  <label>Phone Number *<asp:TextBox ID="txt_Phone" required="true" runat="server" MaxLength="12" ClientIDMode="Static" placeholder="555-555-5555" /></label>
  <label>Issue Type *<asp:DropDownList ID="ddl_problemtype" required="true" runat="server">
    <asp:ListItem Value="">Please Select</asp:ListItem><asp:ListItem Value="1">Login Issue</asp:ListItem><asp:ListItem Value="2">Password Issue</asp:ListItem>
    <asp:ListItem Value="3">Application Crash Issue</asp:ListItem><asp:ListItem Value="4">Functionality Issue</asp:ListItem><asp:ListItem Value="5">Other</asp:ListItem></asp:DropDownList></label>
  <label style="grid-column:1/-1">Description of the Issue *<asp:TextBox ID="txt_Describe" TextMode="MultiLine" Rows="5" required="true" runat="server" /></label>
  <label>File Upload (optional)<asp:FileUpload ID="FileUpload" accept=".png,.jpg,.jpeg,.pdf,.doc,.docx,.xls,.xlsx" runat="server" /></label>
  <asp:HiddenField ID="ddl_module" runat="server" Value="1" />
 </div>
 <div class="am-actions" style="margin-top:18px"><asp:Button ID="btn_save" runat="server" OnClick="btn_save_Click" Text="Save" CssClass="am-button" />
  <asp:Button ID="btn_Reset" runat="server" OnClick="btn_Reset_Click" Text="Reset" CssClass="am-button secondary" formnovalidate="formnovalidate" UseSubmitBehavior="false" /></div>
</section>
<script>(function(){var p=document.getElementById('txt_Phone');if(!p)return;p.addEventListener('input',function(){var v=p.value.replace(/\D/g,'').slice(0,10);p.value=v.length>6?v.slice(0,3)+'-'+v.slice(3,6)+'-'+v.slice(6):v.length>3?v.slice(0,3)+'-'+v.slice(3):v;});})();</script>
</asp:Content>
