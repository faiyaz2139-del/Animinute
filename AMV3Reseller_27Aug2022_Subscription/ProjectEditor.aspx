<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="ProjectEditor.aspx.cs" Inherits="AnyminuteProjectEditorPage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteProjects.css?v=20261025v13" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-projects am-pj-dialog am-pj-editor">
 <header class="am-pj-dhead"><div><h1><asp:Literal ID="PageTitle" runat="server" Text="Add Project" /></h1><p><asp:Literal ID="PageSubtitle" runat="server" Text="Enter the project details and assign it to a business." /></p></div><asp:HyperLink ID="CloseLink" runat="server" NavigateUrl="~/Projects.aspx" CssClass="am-pj-close" ToolTip="Close" aria-label="Close">&times;</asp:HyperLink></header>
 <div class="am-pj-dbody">
  <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
  <asp:Label ID="ModuleNote" runat="server" Visible="false" CssClass="am-pj-info">Description and Project Status are saved after the Projects database update (DB_REQUIRED/REQUIRED_03_ProjectModule.sql) is installed.</asp:Label>
  <div class="am-pj-band"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M6 2h8l6 6v14H6z" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linejoin="round"/><path d="M14 2v6h6M9 13h6M9 17h6" fill="none" stroke="currentColor" stroke-width="1.8"/></svg><h2>Project Information</h2></div>
  <div class="am-pj-form am-datefmt">
   <label class="am-pj-field"><span>Project Name <em class="am-pj-req">(Required)</em></span><asp:TextBox ID="ProjectName" runat="server" MaxLength="100" placeholder="Enter project name" /></label>
   <label class="am-pj-field"><span>Business Name <em class="am-pj-req">(Required)</em></span><asp:DropDownList ID="Business" runat="server" /></label>
   <asp:Panel ID="BusinessLocked" runat="server" Visible="false" CssClass="am-pj-info"><span class="am-pj-info-ic" aria-hidden="true">i</span><span>Business name cannot be changed because this project has employee assignments or time entries.</span></asp:Panel>
   <div class="am-pj-two">
    <label class="am-pj-field"><span>Start Date <em class="am-pj-req">(Required)</em></span><asp:TextBox ID="StartDate" runat="server" TextMode="Date" data-placeholder="yyyy-mm-dd" /></label>
    <label class="am-pj-field"><span>End Date <em class="am-pj-opt">(Optional)</em></span><asp:TextBox ID="EndDate" runat="server" TextMode="Date" data-placeholder="yyyy-mm-dd" /></label>
   </div>
   <label class="am-pj-field"><span>Project Status <em class="am-pj-req">(Required)</em></span><asp:DropDownList ID="Status" runat="server"><asp:ListItem>Active</asp:ListItem><asp:ListItem>Completed</asp:ListItem><asp:ListItem>Inactive</asp:ListItem></asp:DropDownList></label>
   <label class="am-pj-field"><span>Description <em class="am-pj-opt">(Optional)</em></span><asp:TextBox ID="Description" runat="server" TextMode="MultiLine" Rows="3" MaxLength="1000" placeholder="Enter project description" /></label>
  </div>
 </div>
 <footer class="am-pj-dfoot">
  <asp:HyperLink ID="DeleteLink" runat="server" Visible="false" CssClass="am-pj-btn am-pj-danger-outline am-pj-left"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg>Delete Project</asp:HyperLink>
  <asp:HyperLink ID="CancelLink" runat="server" NavigateUrl="~/Projects.aspx" CssClass="am-pj-btn am-pj-outline">Cancel</asp:HyperLink>
  <asp:Button ID="SaveButton" runat="server" Text="Save Project" OnClick="Save_Click" CssClass="am-pj-btn am-pj-primary" />
 </footer>
</section>
</asp:Content>
