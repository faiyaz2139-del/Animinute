<%@ Page Title="Business" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Add_Business.aspx.cs" Inherits="Add_Business" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <link rel="stylesheet" href="css/AnyminuteBusinessEditor.css?v=20261101v42" />
 <script src="Scripts/AnyminuteBusinessEditor.js?v=20261030v32" defer="defer"></script>
 <script src="Scripts/AnyminuteBusinessAutosave.js?v=20261026v19" defer="defer"></script>
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
  <div class="am-al-layout am-al-single">
  <section class="am-al-main" aria-labelledby="address-lookup-heading">
   <h2 id="address-lookup-heading">Address Lookup</h2>
   <p class="am-al-sub">Search for an address using a postal or ZIP code, select a result, or enter the address manually.</p>
   <asp:Panel ID="AddressSearchPanel" runat="server" DefaultButton="lnkSearch" CssClass="am-al-search">
    <div class="am-al-f am-al-country"><label for="LookupCountry">Country <b class="am-al-req">*</b></label><span class="am-al-flagsel"><span class="am-al-flag" id="amLookupFlag"><svg class="am-flag-ca" viewBox="0 0 24 16" width="24" height="16" aria-hidden="true" focusable="false"><rect width="24" height="16" fill="#fff"/><rect width="6" height="16" fill="#d52b1e"/><rect x="18" width="6" height="16" fill="#d52b1e"/><path fill="#d52b1e" d="M12 3.2l.9 1.8 1-.4-.3 2.4 1.5-1.2.4.9 1.3-.3-.5 1.6.6.3L14.6 10l.3.9-2.6-.3.1 2.2h-.8l.1-2.2-2.6.3.3-.9L7.1 8.3l.6-.3-.5-1.6 1.3.3.4-.9 1.5 1.2-.3-2.4 1 .4z"/></svg><svg class="am-flag-us" viewBox="0 0 24 16" width="24" height="16" aria-hidden="true" focusable="false"><rect width="24" height="16" fill="#fff"/><path fill="#b22234" d="M0 0h24v1.23H0zm0 2.46h24v1.23H0zm0 2.46h24v1.23H0zm0 2.46h24v1.23H0zm0 2.46h24v1.23H0zm0 2.46h24v1.23H0zm0 2.46h24V16H0z"/><rect width="10" height="8.6" fill="#3c3b6e"/></svg></span><asp:DropDownList ID="LookupCountry" runat="server" ClientIDMode="Static"><asp:ListItem Value="Canada" Text="Canada" /><asp:ListItem Value="United States" Text="United States" /></asp:DropDownList></span></div>
    <div class="am-al-f am-al-postal"><label for="txtSearchPostalCode">Postal / ZIP Code <b class="am-al-req">*</b></label><span class="am-al-clearwrap"><asp:TextBox ID="txtSearchPostalCode" ClientIDMode="Static" runat="server" MaxLength="10" autocomplete="postal-code" placeholder="e.g. M1B 2W1" /><button type="button" class="am-al-clear" data-al-clear="1" aria-label="Clear postal code">&times;</button></span></div>
    <asp:LinkButton ID="lnkSearch" runat="server" OnClick="lnkSearch_Click" CausesValidation="false" CssClass="am-al-btn am-al-primary"><svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2.4"/><path d="M16.5 16.5L21 21" stroke="currentColor" stroke-width="2.4" stroke-linecap="round"/></svg><span>Search Address</span></asp:LinkButton>
    <asp:LinkButton ID="AddNewAddress" runat="server" OnClick="AddNewAddress_Click" OnClientClick="if(window.AnyminuteBusinessAddress){AnyminuteBusinessAddress.openNew();return false;}" CausesValidation="false" CssClass="am-al-btn am-al-outline"><svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false"><path fill="currentColor" d="M3 17.25V21h3.75L17.8 9.94l-3.75-3.75L3 17.25zM20.7 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg><span>Enter Address Manually</span></asp:LinkButton>
   </asp:Panel>
   <div class="am-al-info"><span class="am-al-i" aria-hidden="true">i</span><span id="amLookupHint">Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).</span></div>
   <asp:Label ID="AddressMessage" ClientIDMode="Static" runat="server" Visible="false" CssClass="am-business-error" role="alert" tabindex="-1" />
   <div id="resultdiv" runat="server" class="am-al-results">
    <label for="AddressChoices">Select Address from Search Results</label>
    <asp:DropDownList ID="AddressChoices" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="AddressChoices_SelectedIndexChanged" />
    <asp:LinkButton ID="UseThisAddress" runat="server" Visible="false" Enabled="false" Text="Use This Address" CausesValidation="false" OnClick="UseThisAddress_Click" />
    <asp:LinkButton ID="ClearBusinessAddress" runat="server" Visible="false" Text="Clear Address" OnClick="ClearBusinessAddress_Click" CausesValidation="false" />
   </div>
   <div id="divAdd" runat="server" class="am-al-details" aria-labelledby="business-address-heading">
    <div class="am-al-details-head"><h3 id="business-address-heading">Address Details</h3><span class="am-al-pill">Manual Entry</span></div>
    <div class="am-al-grid">
     <div class="am-al-f"><label for="txtStreetNo">Street Number <b class="am-al-req">*</b></label><asp:TextBox ID="txtStreetNo" runat="server" ClientIDMode="Static" aria-required="true" placeholder="e.g. 3112" /></div>
     <div class="am-al-f am-al-span2"><label for="txtStreetName">Address Line 1 <b class="am-al-req">*</b></label><asp:TextBox ID="txtStreetName" runat="server" ClientIDMode="Static" autocomplete="address-line1" aria-required="true" placeholder="e.g. Krome Ave" /></div>
     <div class="am-al-f"><label for="txtStreetType">Address Line 2 <span class="am-al-opt">(Optional)</span></label><asp:TextBox ID="txtStreetType" runat="server" ClientIDMode="Static" autocomplete="address-line2" placeholder="e.g. Unit, Floor, Building, etc." /></div>
     <div class="am-al-f"><label for="ddlSuitType">Suite Type <span class="am-al-opt">(Optional)</span></label><asp:DropDownList ID="ddlSuitType" runat="server" ClientIDMode="Static"><asp:ListItem Value="" Text="Please Select" /><asp:ListItem Value="Apartment" Text="Apartment" /><asp:ListItem Value="Suite" Text="Suite" /><asp:ListItem Value="Unit" Text="Unit" /></asp:DropDownList></div>
     <div class="am-al-f"><label for="txtSuiteNumber">Suite Number <span class="am-al-opt">(Optional)</span></label><asp:TextBox ID="txtSuiteNumber" runat="server" ClientIDMode="Static" placeholder="e.g. 200" /></div>
     <div class="am-al-subhead" role="heading" aria-level="4">City, State/Province and Country</div>
     <div class="am-al-f"><label for="txtCity">City <b class="am-al-req">*</b></label><asp:TextBox ID="txtCity" runat="server" ClientIDMode="Static" autocomplete="address-level2" aria-required="true" placeholder="e.g. Miami" /></div>
     <div class="am-al-f"><label for="amProvinceSel">Province / State <b class="am-al-req">*</b></label><select id="amProvinceSel" class="am-al-proxy" data-proxy-for="txtProvince" aria-required="true"></select><asp:TextBox ID="txtProvince" runat="server" ClientIDMode="Static" CssClass="am-al-proxied" autocomplete="address-level1" aria-hidden="true" tabindex="-1" /></div>
     <div class="am-al-f"><label for="txtPostalCode">Postal / ZIP Code <b class="am-al-req">*</b></label><asp:TextBox ID="txtPostalCode" runat="server" ClientIDMode="Static" MaxLength="10" autocomplete="postal-code" aria-required="true" placeholder="e.g. 33185" /></div>
     <div id="CountryField" runat="server" class="am-al-f am-al-span2 am-al-countryf"><label for="amCountrySel">Country <b class="am-al-req">*</b></label><select id="amCountrySel" class="am-al-proxy" data-proxy-for="Country" aria-required="true"><option value="Canada">Canada</option><option value="United States">United States</option></select><asp:TextBox ID="Country" runat="server" ClientIDMode="Static" CssClass="am-al-proxied" Text="Canada" MaxLength="100" aria-hidden="true" tabindex="-1" /></div>
    </div>
    <div class="am-al-actions"><button type="button" class="am-al-btn am-al-outline am-al-wide" data-al-cancel="1">Cancel</button><button type="button" class="am-al-btn am-al-primary am-al-wide" data-al-save="1">Save Changes</button></div>
   </div>
  </section>
  <%-- v42 (PPT 05 s1, "Remove this box"): How Postal Code Search Works help panel removed. --%>
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
   <header class="am-md-head"><div><h2 id="amNewAddressTitle">Enter Address Manually</h2><p>Enter the business address details below. All required fields must be completed.</p></div><button type="button" class="am-addr-x" data-addr-close="1" aria-label="Close">&times;</button></header>
   <div class="am-md-body">
    <div class="am-al-info"><span class="am-al-i" aria-hidden="true">i</span><span>If you cannot find the address using the postal code search, you can enter the address manually.<br />All fields marked with * are required.</span></div>
    <p class="am-business-error" id="amNewAddressError" role="alert" hidden="hidden"></p>
    <fieldset class="am-md-sec"><legend>Address Details</legend><div class="am-md-grid">
     <label><span>Street Number <b class="am-al-req">*</b></span><input type="text" data-addr="txtStreetNo" data-req="1" maxlength="50" placeholder="e.g. 3112" /></label>
     <label class="am-al-span2"><span>Address Line 1 <b class="am-al-req">*</b></span><input type="text" data-addr="txtStreetName" data-req="1" maxlength="50" placeholder="e.g. Krome Ave" /></label>
     <label><span>Address Line 2 <i class="am-al-opt">(Optional)</i></span><input type="text" data-addr="txtStreetType" maxlength="50" placeholder="e.g. Unit, Floor, Building, etc." /></label>
     <label><span>Suite Type <i class="am-al-opt">(Optional)</i></span><select data-addr="ddlSuitType"><option value="">Please Select</option><option>Apartment</option><option>Suite</option><option>Unit</option></select></label>
     <label><span>Suite Number <i class="am-al-opt">(Optional)</i></span><input type="text" data-addr="txtSuiteNumber" maxlength="50" placeholder="e.g. 200" /></label>
    </div></fieldset>
    <fieldset class="am-md-sec"><legend>City, State/Province and Country</legend><div class="am-md-grid">
     <label><span>City <b class="am-al-req">*</b></span><input type="text" data-addr="txtCity" data-req="1" maxlength="50" placeholder="e.g. Miami" /></label>
     <label><span>Province / State <b class="am-al-req">*</b></span><select data-addr="txtProvince" data-req="1" data-province-for="amMdCountry"></select></label>
     <label><span>Postal / ZIP Code</span><input type="text" data-addr="txtPostalCode" maxlength="10" placeholder="e.g. 33185" /></label>
     <label class="am-al-span2"><span>Country <b class="am-al-req">*</b></span><select id="amMdCountry" data-addr="Country" data-req="1"><option value="Canada">Canada</option><option value="United States">United States</option></select></label>
    </div></fieldset>
   </div>
   <footer class="am-md-foot"><button type="button" class="am-al-btn am-al-outline am-al-wide" data-addr-close="1">Cancel</button><button type="button" class="am-al-btn am-al-primary am-al-wide" data-addr-apply="1">Save Address</button></footer>
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
