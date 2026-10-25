<%@ Page Title="Business" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Add_Business.aspx.cs" Inherits="Add_Business" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <link rel="stylesheet" href="css/AnyminuteBusinessEditor.css?v=20261024v10" />
 <script src="Scripts/AnyminuteBusinessEditor.js?v=20261024v10" defer="defer"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="am-business-editor" id="amBusinessEditor">
 <header class="am-business-header am-business-header-v2">
  <nav aria-label="Breadcrumb" class="am-business-breadcrumb"><a href="Dashboard.aspx">Home</a><span aria-hidden="true">&rsaquo;</span><a href="Dashboard.aspx?view=businesses">Business</a><span aria-hidden="true">&rsaquo;</span><asp:Label ID="BusinessBreadcrumb" runat="server" Text="Add Business" /></nav>
  <h1 class="am-business-title"><asp:Label ID="BusinessPageTitle" runat="server" Text="Add Business" /></h1>
  <div class="am-business-actions">
   <asp:Label ID="BusinessStatusBadge" runat="server" Visible="false" CssClass="am-business-status" />
   <a id="btnback" runat="server" href="Dashboard.aspx?view=businesses" class="am-business-button am-business-secondary"><span aria-hidden="true">&larr;</span> Back to List</a>
   <asp:LinkButton ID="SaveBusinessTop" runat="server" OnClick="btnSaveBusinessDetails_Click" CausesValidation="false" CssClass="am-business-button"><svg class="am-business-save-icon" viewBox="0 0 24 24" width="16" height="16" aria-hidden="true" focusable="false"><path fill="currentColor" d="M17 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2V7l-4-4zm-5 16a3 3 0 1 1 0-6 3 3 0 0 1 0 6zm3-10H5V5h10v4z"/></svg><asp:Literal ID="SaveTopText" runat="server" Text="Save" /></asp:LinkButton>
  </div>
 </header>
 <asp:Panel ID="BusinessFlash" runat="server" Visible="false" CssClass="am-business-flash" role="status" aria-live="polite" ClientIDMode="Static"><span class="am-business-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="BusinessFlashText" runat="server" /><button type="button" class="am-business-flash-close" aria-label="Dismiss message" onclick="this.parentNode.style.display='none';">&times;</button></asp:Panel>
 <asp:Label ID="SaveError" runat="server" Visible="false" CssClass="am-business-flash am-business-flash-error" role="alert" ClientIDMode="Static" />
 <asp:Label ID="Label_Maximum_Limit" runat="server" CssClass="am-business-error" role="alert" />
 <asp:HiddenField ID="hdn_companyname" runat="server" /><asp:HiddenField ID="hfdBusinessId" runat="server" />
 <div class="am-business-details-layout">
  <section class="am-business-card am-business-logo-box" aria-labelledby="business-logo-heading">
   <h2 id="business-logo-heading" class="am-visually-hidden">Business Logo</h2>
   <div id="uploadArea" class="am-logo-uploader" runat="server" ClientIDMode="Static">
    <label for="FileUpload1" class="am-logo-circle" title="Click to upload a logo">
     <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" CssClass="am-business-preview" AlternateText="Business logo preview" ImageUrl="" />
     <span class="am-logo-initials" aria-hidden="true"><asp:Literal ID="LogoInitials" runat="server" /></span>
     <span class="am-logo-empty">
      <svg viewBox="0 0 64 48" width="58" height="44" aria-hidden="true" focusable="false"><path fill="#1768d6" d="M51.6 18.2A19 19 0 0 0 15.1 13 14.5 14.5 0 0 0 16 42h11V31h-6.5L32 19.5 43.5 31H37v11h13a12 12 0 0 0 1.6-23.8z"/></svg>
      <strong>Click to upload</strong>
      <small>400 &times; 400 px</small>
     </span>
    </label>
    <asp:FileUpload ID="FileUpload1" runat="server" ClientIDMode="Static" accept="image/png,image/jpeg" CssClass="am-business-file am-visually-hidden-input" aria-describedby="business-logo-help business-logo-error" />
   </div>
   <label for="FileUpload1" id="ChangeLogoButton" runat="server" class="am-business-button am-business-secondary am-logo-change"><span aria-hidden="true">&#10548;</span> Change Logo</label>
   <p id="business-logo-help" class="am-business-help">PNG or JPG &middot; Max 2 MB<br />(Recommended 400 &times; 400 px)</p>
   <p id="business-logo-error" class="am-business-error" role="alert" hidden="hidden"></p>
   <asp:Button ID="RemoveBusinessLogo" runat="server" Text="Remove logo" CssClass="am-logo-remove" CausesValidation="false" />
  </section>
  <section id="BusinessDetailsCard" runat="server" class="am-business-card am-fields-v3" aria-labelledby="business-details-heading">
   <h2 id="business-details-heading">Business Details</h2>
   <div class="am-business-grid am-grid-12">
<div class="am-business-field am-f-company"><asp:Label runat="server" AssociatedControlID="txtCompanyName" Text="Company Name" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtCompanyName" runat="server" CssClass="am-business-input" MaxLength="200" autocomplete="organization" aria-required="true" placeholder="Enter company name" /></div>
<div class="am-business-field am-f-operating"><asp:Label runat="server" AssociatedControlID="txtOperatingName" Text="Operating Name" /><asp:TextBox ID="txtOperatingName" runat="server" CssClass="am-business-input" MaxLength="200" placeholder="Enter operating name" /></div>
<div class="am-business-field am-f-cname"><asp:Label runat="server" AssociatedControlID="PrmContName" Text="Primary Contact Name" /> <span class="am-required">(Required)</span><asp:TextBox ID="PrmContName" runat="server" CssClass="am-business-input" autocomplete="name" aria-required="true" placeholder="Enter primary contact name" /></div>
<div class="am-business-field am-f-cnumber"><asp:Label runat="server" AssociatedControlID="PrmContNmbr" Text="Primary Contact Number" /> <span class="am-required">(Required)</span><asp:TextBox ID="PrmContNmbr" runat="server" CssClass="am-business-input" TextMode="Phone" MaxLength="24" autocomplete="tel" aria-required="true" placeholder="Enter primary contact number" /></div>
<div class="am-business-field am-f-email"><asp:Label runat="server" AssociatedControlID="txtMainEmail" Text="Email" /> <span class="am-required">(Required)</span><span class="am-input-icon am-icon-mail"><asp:TextBox ID="txtMainEmail" runat="server" CssClass="am-business-input" TextMode="Email" autocomplete="email" aria-required="true" placeholder="Enter email address" /></span></div>
<div class="am-business-field am-f-bnumber"><asp:Label runat="server" AssociatedControlID="txtBusinessNumber" Text="Business Number" /><asp:TextBox ID="txtBusinessNumber" runat="server" CssClass="am-business-input" placeholder="Enter business number" /></div>
<div class="am-business-field am-f-legal"><asp:Label runat="server" AssociatedControlID="ddlLegalEnityType" Text="Legal Entity Type" /><asp:DropDownList ID="ddlLegalEnityType" runat="server" CssClass="am-business-input">
                                                
                                                <asp:ListItem Value="Select">Select legal entity type</asp:ListItem>
                                                <asp:ListItem Value="Sole Proprietorships">Sole Proprietorship</asp:ListItem>
                                                <asp:ListItem Value="Partnerships">Partnership</asp:ListItem>
                                                <asp:ListItem Value="Corporations">Corporation</asp:ListItem>
                                                <asp:ListItem Value="S Corporations">S Corporation</asp:ListItem>
                                                <asp:ListItem>Limited Liability Company (LLC) </asp:ListItem>
                                            </asp:DropDownList></div>
<div class="am-business-field am-f-website"><asp:Label runat="server" AssociatedControlID="txtWebsite" Text="Website" /><span class="am-input-icon am-icon-globe"><asp:TextBox ID="txtWebsite" runat="server" CssClass="am-business-input" placeholder="Enter website (e.g. www.example.com)" /></span></div>
<div id="StatusField" runat="server" class="am-business-field am-f-status"><asp:Label runat="server" AssociatedControlID="ddlClientStats" Text="Status" /><asp:DropDownList ID="ddlClientStats" runat="server" CssClass="am-business-input">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Value="1" Selected="True">Active</asp:ListItem>
                                                <asp:ListItem Value="0">Not Active</asp:ListItem>
                                            </asp:DropDownList></div>
   </div>
   <%-- v3: not on the deck; kept hidden so existing values are preserved on save --%><details ID="ExtraBusinessDetails" runat="server" class="am-business-extra" hidden="hidden" aria-hidden="true"><summary>Additional contact and registration details</summary><div class="am-business-grid am-business-grid-two">
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
   <h2 id="address-lookup-heading" class="am-heading-icon"><svg viewBox="0 0 24 24" width="20" height="20" aria-hidden="true" focusable="false"><path fill="currentColor" d="M12 2a7 7 0 0 0-7 7c0 5.25 7 13 7 13s7-7.75 7-13a7 7 0 0 0-7-7zm0 9.5A2.5 2.5 0 1 1 12 6.5a2.5 2.5 0 0 1 0 5z"/></svg>Address Lookup</h2>
   <p>Enter a postal code to search for an address, then select one from the list below or add a new address.</p>
   <div class="am-business-lookup">
    <div class="am-business-field"><asp:Label runat="server" AssociatedControlID="txtSearchPostalCode" Text="Enter Postal Code" /> <span class="am-required">(Required)</span><asp:Panel ID="AddressSearchPanel" runat="server" DefaultButton="lnkSearch" CssClass="am-business-search-group">
     <asp:TextBox ID="txtSearchPostalCode" ClientIDMode="Static" runat="server" MaxLength="10" CssClass="am-business-input" autocomplete="postal-code" placeholder="Enter postal code" />
     <asp:LinkButton ID="lnkSearch" runat="server" OnClick="lnkSearch_Click" CausesValidation="false" CssClass="am-business-button am-search-icon" ToolTip="Search postal code"><svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false"><path fill="currentColor" d="M15.5 14h-.79l-.28-.27A6.47 6.47 0 0 0 16 9.5 6.5 6.5 0 1 0 9.5 16c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L20.49 19l-4.99-5zm-6 0C7.01 14 5 11.99 5 9.5S7.01 5 9.5 5 14 7.01 14 9.5 11.99 14 9.5 14z"/></svg><span class="am-visually-hidden">Search</span></asp:LinkButton>
    </asp:Panel></div>
    <div id="resultdiv" runat="server" class="am-business-results">
     <asp:Label runat="server" AssociatedControlID="AddressChoices" Text="Select Address from List or Add New" /> <span class="am-required">(Required)</span>
     <div class="am-results-row">
      <asp:DropDownList ID="AddressChoices" runat="server" AutoPostBack="true" OnSelectedIndexChanged="AddressChoices_SelectedIndexChanged" CssClass="am-business-input" />
      <asp:LinkButton ID="UseThisAddress" runat="server" Enabled="false" Text="Use This Address" CausesValidation="false" OnClick="UseThisAddress_Click" CssClass="am-business-button am-use-address" />
     </div>
    </div>
    <div class="am-lookup-actions">
     <asp:LinkButton ID="AddNewAddress" runat="server" Text="+&nbsp; Add New Address" OnClick="AddNewAddress_Click" OnClientClick="if(window.AnyminuteBusinessAddress){AnyminuteBusinessAddress.openNew();return false;}" CausesValidation="false" CssClass="am-business-button am-business-secondary" />
     <asp:LinkButton ID="ClearBusinessAddress" runat="server" Text="Clear Address" OnClick="ClearBusinessAddress_Click" CausesValidation="false" CssClass="am-business-button am-business-secondary" OnClientClick="return confirm('Clear the business address fields? Business details are kept.');" />
    </div>
   </div>
  </section>
  <asp:Label ID="AddressMessage" ClientIDMode="Static" runat="server" Visible="false" CssClass="am-business-error" role="alert" tabindex="-1" />
  <div id="divAdd" runat="server" class="am-business-card" style="display:block;" aria-labelledby="business-address-heading">
   <h2 id="business-address-heading">Business Address</h2>
   <p class="am-business-hint">When an address is selected from the list and you click Use This Address, the fields below are filled automatically. Fields are locked: click the pencil in a field to edit it.</p>
   <div class="am-business-grid am-grid-12 am-address-v3">
<div class="am-business-field am-a3"><asp:Label runat="server" AssociatedControlID="txtStreetNo" Text="Street Number" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtStreetNo" runat="server" CssClass="am-business-input" ClientIDMode="Static" aria-required="true" /></div>
<div class="am-business-field am-a3"><asp:Label runat="server" AssociatedControlID="txtStreetName" Text="Address Line 1" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtStreetName" runat="server" CssClass="am-business-input" ClientIDMode="Static" autocomplete="address-line1" aria-required="true" /></div>
<div class="am-business-field am-a3"><asp:Label runat="server" AssociatedControlID="txtStreetType" Text="Address Line 2" /><asp:TextBox ID="txtStreetType" runat="server" ClientIDMode="Static" CssClass="am-business-input" autocomplete="address-line2" placeholder="Enter address line 2 (e.g. Unit, Floor, etc.)" /></div>
<div class="am-business-field am-a3"><asp:Label runat="server" AssociatedControlID="txtSuiteNumber" Text="Suite Number" /><asp:TextBox ID="txtSuiteNumber" runat="server" ClientIDMode="Static" CssClass="am-business-input"  /></div>
<div class="am-business-field am-a3"><asp:Label runat="server" AssociatedControlID="ddlSuitType" Text="Suite Type" /><asp:DropDownList ID="ddlSuitType" runat="server" ClientIDMode="Static" CssClass="am-business-input">
                                                        <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                        <asp:ListItem Value="Apartment" Text="Apartment"></asp:ListItem>
                                                        <asp:ListItem Value="Suite" Text="Suite"></asp:ListItem>
                                                        <asp:ListItem Value="Unit" Text="Unit"></asp:ListItem>
                                                    </asp:DropDownList></div>
<div class="am-business-field am-a4 am-row-start"><asp:Label runat="server" AssociatedControlID="txtCity" Text="City" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtCity" runat="server" CssClass="am-business-input" ClientIDMode="Static" autocomplete="address-level2" aria-required="true" /></div>
<div class="am-business-field am-a4"><asp:Label runat="server" AssociatedControlID="txtProvince" Text="Province/State" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtProvince" runat="server" CssClass="am-business-input am-select-look" list="am-provinces" ClientIDMode="Static" autocomplete="address-level1" aria-required="true" /></div>
<div class="am-business-field am-a4"><asp:Label runat="server" AssociatedControlID="txtPostalCode" Text="Postal Code" /> <span class="am-required">(Required)</span><asp:TextBox ID="txtPostalCode" runat="server" CssClass="am-business-input" ClientIDMode="Static" MaxLength="10" autocomplete="postal-code" aria-required="true" /></div>
<div id="CountryField" runat="server" class="am-business-field am-a4"><asp:Label runat="server" AssociatedControlID="Country" Text="Country" /> <span class="am-required">(Required)</span><asp:TextBox ID="Country" runat="server" ClientIDMode="Static" CssClass="am-business-input am-select-look" list="am-countries" Text="Canada" MaxLength="100" autocomplete="country-name" aria-required="true" /></div>
   </div>
  </div>
  <datalist id="am-provinces"><option value="ALBERTA"></option><option value="BRITISH COLUMBIA"></option><option value="MANITOBA"></option><option value="NEW BRUNSWICK"></option><option value="NEWFOUNDLAND AND LABRADOR"></option><option value="NORTHWEST TERRITORIES"></option><option value="NOVA SCOTIA"></option><option value="NUNAVUT"></option><option value="ONTARIO"></option><option value="PRINCE EDWARD ISLAND"></option><option value="QUEBEC"></option><option value="SASKATCHEWAN"></option><option value="YUKON"></option></datalist><datalist id="am-countries"><option value="Canada"></option><option value="United States"></option></datalist>
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
 <div class="am-addr-modal" id="amNewAddress" role="dialog" aria-modal="true" aria-labelledby="amNewAddressTitle" hidden="hidden">
  <div class="am-addr-dialog">
   <header><h2 id="amNewAddressTitle">Add New Address</h2><button type="button" class="am-addr-x" data-addr-close="1" aria-label="Close">&times;</button></header>
   <p class="am-business-hint">Enter the address. It is copied into Business Address; nothing is saved until you click Save at the bottom of the page.</p>
   <p class="am-business-error" id="amNewAddressError" role="alert" hidden="hidden"></p>
   <div class="am-addr-grid">
    <label><span>Street Number <span class="am-required">(Required)</span></span><input type="text" data-addr="txtStreetNo" data-req="1" maxlength="50" /></label>
    <label><span>Address Line 1 <span class="am-required">(Required)</span></span><input type="text" data-addr="txtStreetName" data-req="1" maxlength="50" /></label>
    <label><span>Address Line 2</span><input type="text" data-addr="txtStreetType" maxlength="50" placeholder="Unit, Floor, etc." /></label>
    <label><span>Suite Number</span><input type="text" data-addr="txtSuiteNumber" maxlength="50" /></label>
    <label><span>Suite Type</span><select data-addr="ddlSuitType"><option value="">Please Select</option><option>Apartment</option><option>Suite</option><option>Unit</option></select></label>
    <label><span>City <span class="am-required">(Required)</span></span><input type="text" data-addr="txtCity" data-req="1" maxlength="50" /></label>
    <label><span>Province/State <span class="am-required">(Required)</span></span><input type="text" data-addr="txtProvince" data-req="1" list="am-provinces" maxlength="50" /></label>
    <label><span>Postal Code <span class="am-required">(Required)</span></span><input type="text" data-addr="txtPostalCode" data-req="1" maxlength="10" /></label>
    <label><span>Country <span class="am-required">(Required)</span></span><input type="text" data-addr="Country" data-req="1" list="am-countries" maxlength="100" value="Canada" /></label>
   </div>
   <footer><button type="button" class="am-business-button am-business-secondary" data-addr-close="1">Cancel</button><button type="button" class="am-business-button" data-addr-apply="1">Use This Address</button></footer>
  </div>
 </div>
 <footer class="am-business-actions am-business-footer">
  <asp:LinkButton ID="CancelBusiness" runat="server" Text="Cancel" OnClick="CancelBusiness_Click" CausesValidation="false" CssClass="am-business-button am-business-secondary" />
  <asp:LinkButton ID="btnSaveBusinessDetails" runat="server" Text="Save" OnClick="btnSaveBusinessDetails_Click" CausesValidation="false" CssClass="am-business-button" />
  <asp:LinkButton ID="btnUpdateBusinessDetails" runat="server" Text="Save Changes" OnClick="btnUpdateBusinessDetails_Click" Visible="false" CausesValidation="false" CssClass="am-business-button" />
  <asp:Button ID="DeactivateBusiness" runat="server" Visible="false" Text="Deactivate Business" CssClass="am-business-button am-business-danger" CausesValidation="false" />
 </footer>
 <div class="am-business-compat" hidden="hidden" aria-hidden="true">
  <div ID="divAdmin" runat="server"><asp:TextBox ID="txtaccountantmail" runat="server" Visible="false" Placeholder="Enter business owner email ID" /></div>
  <asp:Label ID="lblAddressBusName" runat="server" />
 </div>
</div>
</asp:Content>
