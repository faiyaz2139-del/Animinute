<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="SupportAdminProfile.aspx.cs" Inherits="SupportAdminProfile" %>
<%-- 2026-11-03 v58 (Support Admin PPT s2/s3 "Can't Edit the Profile"): My profile for Support Admin is a real, editable profile in the
     Support Admin console (Modern.master), replacing the old Main.master form with its Home / Back to List links. --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSupport.css?v=20261105v66" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="sa-page">
 <header class="sa-head"><div><h1>My Profile</h1><p>Your Support Admin account and contact details.</p></div></header>
 <asp:Literal ID="Flash" runat="server" />
 <asp:Panel ID="ProfileForm" runat="server" CssClass="sa-card" DefaultButton="btnsave">
  <h2>Personal Details</h2>
  <div class="sa-row3">
   <label class="sa-field"><span>Title</span><asp:DropDownList ID="ddlLegalName" runat="server"><asp:ListItem Value="">Select</asp:ListItem><asp:ListItem Value="Mr.">Mr.</asp:ListItem><asp:ListItem Value="Miss.">Miss.</asp:ListItem><asp:ListItem Value="Ms.">Ms.</asp:ListItem><asp:ListItem Value="Mrs.">Mrs.</asp:ListItem><asp:ListItem Value="Mx.">Mx.</asp:ListItem></asp:DropDownList></label>
   <label class="sa-field"><span>First Name <i class="sa-req">*</i></span><asp:TextBox ID="txtFirstName" runat="server" MaxLength="50" required="required" /></label>
   <label class="sa-field"><span>Middle Name</span><asp:TextBox ID="txtMiddleName" runat="server" MaxLength="50" /></label>
   <label class="sa-field"><span>Last Name <i class="sa-req">*</i></span><asp:TextBox ID="txtLastName" runat="server" MaxLength="50" required="required" /></label>
   <label class="sa-field"><span>Date of Birth</span><asp:TextBox ID="txtdob" runat="server" TextMode="Date" max="9999-12-31" /></label>
   <label class="sa-field"><span>Gender</span><asp:DropDownList ID="ddlgender" runat="server"><asp:ListItem Value="">Select</asp:ListItem><asp:ListItem Value="Male">Male</asp:ListItem><asp:ListItem Value="Female">Female</asp:ListItem><asp:ListItem Value="Others">Others</asp:ListItem></asp:DropDownList></label>
  </div>
  <h2 style="margin-top:12px">Contact Details</h2>
  <div class="sa-row3">
   <label class="sa-field"><span>Main Phone <i class="sa-req">*</i></span><asp:TextBox ID="txtMainPhone" runat="server" MaxLength="15" required="required" placeholder="416-555-0100" /></label>
   <label class="sa-field"><span>Mobile</span><asp:TextBox ID="txt_mobilenumber" runat="server" MaxLength="15" /></label>
   <label class="sa-field"><span>Login Email</span><asp:TextBox ID="txtMainEmail" runat="server" ReadOnly="true" /></label>
   <label class="sa-field"><span>City</span><asp:TextBox ID="txt_city" runat="server" MaxLength="50" /></label>
   <label class="sa-field"><span>Province</span><asp:TextBox ID="txt_province" runat="server" MaxLength="50" /></label>
  </div>
  <p class="sa-muted sa-small">To change your sign-in email use <a class="sa-link" href="LoginSecurity.aspx">Login &amp; security</a> (the new email is verified first). To change your password use <a class="sa-link" href="Change%20Password.aspx">Change password</a>.</p>
  <div class="sa-panel-foot"><a class="sa-btn sa-btn-outline" href="SupportSubscribers.aspx">Cancel</a><asp:Button ID="btnsave" runat="server" CssClass="sa-btn" Text="Save Changes" OnClick="btnsave_Click" /></div>
 </asp:Panel>
</section>
</asp:Content>
