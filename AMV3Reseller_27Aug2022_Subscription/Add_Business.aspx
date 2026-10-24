<%@ Page Title="Business" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Add_Business.aspx.cs" Inherits="Add_Business" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <link rel="stylesheet" href="css/AnyminuteBusinessEditor.css?v=20261023r2" />
 <script src="Scripts/AnyminuteBusinessEditor.js?v=20261023r2" defer="defer"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="am-business-editor" id="amBusinessEditor">
 <nav aria-label="Breadcrumb" class="am-business-breadcrumb"><a href="Dashboard.aspx">Home</a><span aria-hidden="true">/</span><a href="Dashboard.aspx?view=businesses">Businesses</a><span aria-hidden="true">/</span><asp:Label ID="BusinessBreadcrumb" runat="server" Text="Add Business" /></nav>
 <header class="am-business-header">
  <div><h1><asp:Label ID="BusinessPageTitle" runat="server" Text="Add Business" /></h1><p>Business details, logo, contacts and address.</p></div>
  <div class="am-business-actions">
   <a id="btnback" runat="server" href="Dashboard.aspx?view=businesses" class="am-business-button am-business-secondary">Back to List</a>
   <asp:Button ID="SaveBusinessTop" runat="server" Text="Save" OnClick="btnSaveBusinessDetails_Click" CausesValidation="false" CssClass="am-business-button" />
  </div>
 </header>
 <asp:Label ID="Label_Maximum_Limit" runat="server" CssClass="am-business-error" role="alert" />
 <asp:HiddenField ID="hdn_companyname" runat="server" /><asp:HiddenField ID="hfdBusinessId" runat="server" />
 <div class="am-business-details-layout">
  <section class="am-business-card am-business-logo-box" aria-labelledby="business-logo-heading">
   <h2 id="business-logo-heading">Business Logo</h2>
   <div id="uploadArea" class="am-business-upload">
    <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" CssClass="am-business-preview" AlternateText="Business logo preview" ImageUrl="" />
    <asp:Label runat="server" AssociatedControlID="FileUpload1" CssClass="am-business-upload-label" Text="Click to upload or change logo" />
    <asp:FileUpload ID="FileUpload1" runat="server" accept="image/png,image/jpeg" CssClass="am-business-file" aria-describedby="business-logo-help business-logo-error" />
   </div>
   <p id="business-logo-help" class="am-business-help">PNG or JPG · Max 2 MB<br />Recommended 400 × 400 px</p>
   <p id="business-logo-error" class="am-business-error" role="alert" hidden="hidden"></p>
   <asp:Button ID="RemoveBusinessLogo" runat="server" Text="Remove Logo" CssClass="am-business-button am-business-secondary" CausesValidation="false" />
  </section>
  <section class="am-business-card" aria-labelledby="business-details-heading">
   <h2 id="business-details-heading">Business Details</h2>
   <div class="am-business-grid am-business-grid-two">
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtCompanyName" Text="Company Name" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtCompanyName" runat="server" CssClass="am-business-input" MaxLength="200" autocomplete="organization" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtOperatingName" Text="Operating Name" /><asp:TextBox ID="txtOperatingName" runat="server" CssClass="am-business-input" MaxLength="200" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="PrmContName" Text="Primary Contact Name" /> <span class="am-required">(Required)</span><asp:TextBox ID="PrmContName" runat="server" CssClass="am-business-input" autocomplete="name" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="PrmContNmbr" Text="Primary Contact Number" /> <span class="am-required">(Required)</span><asp:TextBox ID="PrmContNmbr" runat="server" CssClass="am-business-input" TextMode="Phone" MaxLength="24" autocomplete="tel" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtMainEmail" Text="Email" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtMainEmail" runat="server" CssClass="am-business-input" TextMode="Email" autocomplete="email" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtBusinessNumber" Text="Business Number" /><asp:TextBox ID="txtBusinessNumber" runat="server" CssClass="am-business-input"  /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="ddlLegalEnityType" Text="Legal Entity Type" /><asp:DropDownList ID="ddlLegalEnityType" runat="server" CssClass="am-business-input">
                                                
                                                <asp:ListItem>Select</asp:ListItem>
                                                <asp:ListItem>Sole Proprietorships</asp:ListItem>
                                                <asp:ListItem>Partnerships</asp:ListItem>
                                                <asp:ListItem>Corporations</asp:ListItem>
                                                <asp:ListItem>S Corporations</asp:ListItem>
                                                <asp:ListItem>Limited Liability Company (LLC) </asp:ListItem>
                                            </asp:DropDownList></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtWebsite" Text="Website" /><asp:TextBox ID="txtWebsite" runat="server" CssClass="am-business-input" placeholder="www.example.com" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="ddlClientStats" Text="Status" /><asp:DropDownList ID="ddlClientStats" runat="server" CssClass="am-business-input">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                                                <asp:ListItem Value="2">Not Active</asp:ListItem>
                                            </asp:DropDownList></div>
   </div>
   <details ID="ExtraBusinessDetails" runat="server" class="am-business-extra" open="open"><summary>Additional contact and registration details</summary><div class="am-business-grid am-business-grid-two">
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtMainPhone" Text="Contact Phone" /><asp:TextBox ID="txtMainPhone" runat="server" CssClass="am-business-input" TextMode="Phone" MaxLength="24" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtMobile" Text="Mobile" /><asp:TextBox ID="txtMobile" runat="server" CssClass="am-business-input" TextMode="Phone" MaxLength="24" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtWorkPhone" Text="Work Phone" /><asp:TextBox ID="txtWorkPhone" runat="server" CssClass="am-business-input" TextMode="Phone" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtCCEmail" Text="CC Email" /><asp:TextBox ID="txtCCEmail" runat="server" CssClass="am-business-input" TextMode="Email" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtFax" Text="Fax" /><asp:TextBox ID="txtFax" runat="server" CssClass="am-business-input"  /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="BsnsRegDate" Text="Business Registration Date" /><asp:TextBox ID="BsnsRegDate" runat="server" CssClass="am-business-input" TextMode="Date" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="DateBusnsAssgnDate" Text="Business Agreement Date" /><asp:TextBox ID="DateBusnsAssgnDate" runat="server" CssClass="am-business-input" TextMode="Date" ReadOnly="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtOther1" Text="Other Contact Details" /><asp:TextBox ID="txtOther1" runat="server" CssClass="am-business-input"  /></div>
   </div><p class="am-business-help">Leave Contact Phone blank to use the primary contact number. Mobile is optional.</p></details>
  </section>
 </div>
 <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional"><ContentTemplate>
  <section class="am-business-card" aria-labelledby="address-lookup-heading">
   <h2 id="address-lookup-heading">Address Lookup</h2>
   <p>Enter a postal code to search, then select an address or add one manually.</p>
   <div class="am-business-lookup">
    <div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtSearchPostalCode" Text="Enter Postal Code" /><asp:Panel ID="AddressSearchPanel" runat="server" DefaultButton="lnkSearch" CssClass="am-business-search-group">
     <asp:TextBox ID="txtSearchPostalCode" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="am-business-input" autocomplete="postal-code" />
     <asp:LinkButton ID="lnkSearch" runat="server" Text="Search" OnClick="lnkSearch_Click" CausesValidation="false" CssClass="am-business-button" />
    </asp:Panel></div>
    <div id="resultdiv" runat="server" visible="false" class="am-business-results">
     <asp:Label runat="server" AssociatedControlID="AddressChoices" Text="Select an address from the list" />
     <asp:DropDownList ID="AddressChoices" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AddressChoices_SelectedIndexChanged" CssClass="am-business-input" />
     <asp:LinkButton ID="UseThisAddress" runat="server" Enabled="false" Text="Use This Address" CausesValidation="false" OnClick="UseThisAddress_Click" CssClass="am-business-button am-business-secondary" />
    </div>
   </div>
   <div class="am-business-actions">
    <asp:LinkButton ID="AddNewAddress" runat="server" Text="Add New Address" OnClick="AddNewAddress_Click" CausesValidation="false" CssClass="am-business-button am-business-secondary" />
    <asp:LinkButton ID="ClearBusinessAddress" runat="server" Text="Clear Address" OnClick="ClearBusinessAddress_Click" CausesValidation="false" CssClass="am-business-button am-business-secondary" />
   </div>
  </section>
  <asp:Label ID="AddressMessage" ClientIDMode="Static" runat="server" Visible="false" CssClass="am-business-error" role="alert" tabindex="-1" />
  <div id="divAdd" runat="server" class="am-business-card" style="display:block;" aria-labelledby="business-address-heading">
   <h2 id="business-address-heading">Business Address</h2>
   <p class="am-business-hint">Selecting an address fills the fields below. Check the street number, add a suite if needed, and correct any details before saving.</p>
   <div class="am-business-grid am-business-grid-three">
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtStreetNo" Text="Street Number" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtStreetNo" runat="server" CssClass="am-business-input" ClientIDMode="Static" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtStreetName" Text="Address Line 1" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtStreetName" runat="server" CssClass="am-business-input" ClientIDMode="Static" autocomplete="address-line1" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtStreetType" Text="Address Line 2" /><asp:TextBox ID="txtStreetType" runat="server" CssClass="am-business-input" autocomplete="address-line2" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtSuiteNumber" Text="Suite Number" /><asp:TextBox ID="txtSuiteNumber" runat="server" CssClass="am-business-input"  /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="ddlSuitType" Text="Suite Type" /><asp:DropDownList ID="ddlSuitType" runat="server" CssClass="am-business-input">
                                                        <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                        <asp:ListItem Value="Apartment" Text="Apartment"></asp:ListItem>
                                                        <asp:ListItem Value="Suite" Text="Suite"></asp:ListItem>
                                                        <asp:ListItem Value="Unit" Text="Unit"></asp:ListItem>
                                                    </asp:DropDownList></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtCity" Text="City" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtCity" runat="server" CssClass="am-business-input" ClientIDMode="Static" autocomplete="address-level2" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtProvince" Text="Province / State" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtProvince" runat="server" CssClass="am-business-input" ClientIDMode="Static" autocomplete="address-level1" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtPostalCode" Text="Postal Code" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtPostalCode" runat="server" CssClass="am-business-input" ClientIDMode="Static" MaxLength="10" autocomplete="postal-code" aria-required="true" /></div>
<div class="am-business-field"><asp:Label runat="server" AssociatedControlID="Country" Text="Country" /> <span class="am-required">(Required)</span><asp:TextBox ID="Country" runat="server" CssClass="am-business-input" Text="Canada" MaxLength="100" autocomplete="country-name" aria-required="true" /></div>
   </div>
  </div>
  <div class="am-business-compat" hidden="hidden" aria-hidden="true">
   <asp:GridView ID="gvAddressList" runat="server" Visible="false" AutoGenerateColumns="false"><Columns><asp:TemplateField><ItemTemplate>
    <asp:LinkButton ID="StartDate" runat="server" CausesValidation="false" Text='<%#DataBinder.Eval(Container.DataItem, "SEARCHADDRESS")%>' OnClick="StartDate_Click" />
   </ItemTemplate></asp:TemplateField></Columns></asp:GridView>
   <asp:RadioButtonList ID="rdobtnlist" runat="server" Visible="false" />
   <asp:TextBox ID="txtStreetDirection" runat="server" />
   <asp:TextBox ID="txtStreetDirection1" runat="server" />
   <asp:TextBox ID="txtStreetType1" runat="server" />
<asp:DropDownList ID="ddlNumberSuffix" runat="server" CssClass="am-business-input">
                                        <asp:ListItem Text="Please Select" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                        <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                    </asp:DropDownList><asp:DropDownList ID="ddlsuitesuffix" runat="server" CssClass="am-business-input">
                                        <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                        <asp:ListItem Value="Apartment" Text="Apartment"></asp:ListItem>
                                        <asp:ListItem Value="Suite" Text="Suite"></asp:ListItem>
                                        <asp:ListItem Value="Unit" Text="Unit"></asp:ListItem>

                                    </asp:DropDownList>   <asp:LinkButton ID="btnSaveAddres" runat="server" Text="Save" Visible="false" OnClick="btnSaveAddres_Click" CausesValidation="false" />
   <asp:LinkButton ID="btnUpdateAddress" runat="server" Text="Save" Visible="false" OnClick="btnUpdateAddress_Click" CausesValidation="false" />
  </div>
 </ContentTemplate><Triggers>
  <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
  <asp:AsyncPostBackTrigger ControlID="AddressChoices" EventName="SelectedIndexChanged" />
  <asp:AsyncPostBackTrigger ControlID="UseThisAddress" EventName="Click" />
  <asp:AsyncPostBackTrigger ControlID="AddNewAddress" EventName="Click" />
  <asp:AsyncPostBackTrigger ControlID="ClearBusinessAddress" EventName="Click" />
  <asp:PostBackTrigger ControlID="btnSaveAddres" />
  <asp:PostBackTrigger ControlID="btnUpdateAddress" />
 </Triggers></asp:UpdatePanel>
 <footer class="am-business-actions am-business-footer">
  <asp:LinkButton ID="CancelBusiness" runat="server" Text="Cancel" OnClick="CancelBusiness_Click" CausesValidation="false" CssClass="am-business-button am-business-secondary" />
  <asp:LinkButton ID="btnSaveBusinessDetails" runat="server" Text="Save" OnClick="btnSaveBusinessDetails_Click" CausesValidation="false" CssClass="am-business-button" />
  <asp:LinkButton ID="btnUpdateBusinessDetails" runat="server" Text="Save Changes" OnClick="btnUpdateBusinessDetails_Click" Visible="false" CausesValidation="false" CssClass="am-business-button" />
  <asp:Button ID="DeactivateBusiness" runat="server" Text="Deactivate Business" CssClass="am-business-button am-business-danger" CausesValidation="false" />
 </footer>
 <div class="am-business-compat" hidden="hidden" aria-hidden="true">
  <div ID="divAdmin" runat="server"><asp:TextBox ID="txtaccountantmail" runat="server" Visible="false" Placeholder="Enter business owner email ID" /></div>
  <asp:Label ID="lblAddressBusName" runat="server" />
 </div>
</div>
</asp:Content>
