<%@ Page Title="" Language="C#" MasterPageFile="~/MasterWithoutMenu.master" AutoEventWireup="true" CodeFile="Register_As_A_Bo.aspx.cs" Inherits="Register_As_A_Bo" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script lang="Javascript" type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)
                return false;

            return true;
        }
    </script>

    <%--To turn autocomplete off in all textboxes except the textboxes which are under the update panel--%>
    <script type="text/javascript">
        $(document).ready(function () { $("input").attr("autocomplete", "off"); });
    </script>

    <script>
        $(document).ready(function () {
            $('#txtMainPhone').keyup(function (event) {
                addHyphen(this);
            });
        });

        $(document).ready(function () {
            $('#txt_mobilenumber').keyup(function (event) {
                addHyphen(this);
            });
        });

        function addHyphen(element) {
            let val = $(element).val().split('-').join('');   // Remove dash (-) if mistakenly entered.
            let n = val.length;

            if (n <= 9) {
                let finalVal = val.match(/.{1,3}/g).join('-');    // Add (-) after 3rd every char.
                $(element).val(finalVal);		// Update the input box.
            }
        }
    </script>

    <%--SweetAlert Javascript--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

   <link href="css/upgrade.css" rel="stylesheet" />
    <style type="text/css">
        .buttonClass {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }

            .buttonClass:hover {
                padding: 2px 20px;
                text-decoration: none;
                border: solid 1px #607DF2;
                color: white;
                background-color: #607DF2;
                border-radius: 3px;
            }
    </style>
    <link href="css/rounduploadimagecontrol.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:HiddenField ID="hdf_stripePlans" runat="server" />
    <asp:HiddenField ID="hdf_selectedPlanPriceId" runat="server" />
    <asp:HiddenField ID="hdf_selectedPlanId" runat="server" />
    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmailConfirm" BehaviorID="mpe" runat="server"
        PopupControlID="pnlEmailConfirm" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>


    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-4 col-sm-4 col-xs-4">
                    <asp:Image ID="Image1" runat="server" Height="50px" Width="50px" ImageUrl="~/Assets/assets/img/logo.jpg" Style="padding-top: 10px;" />
                </div>
                <div class="col-md-4 col-sm-4 col-xs-4 text-center" style="padding-top: 13px;">
                    <asp:Label Font-Bold="true" Font-Size="X-Large" Text="New Subscriber Registration" runat="server" />
                </div>
                <div class="col-md-4 col-sm-4 col-xs-4 text-right">
                    <a href="Login.aspx" id="btnback" runat="server" style="margin-top: 20px; float: right;">Home</a><%--class="buttonhyperlink"--%>
                </div>
            </div>
        </div>
    </div>



    <asp:HiddenField ID="hfdEmpID" runat="server" />
    <asp:HiddenField ID="hfdUserId" runat="server" />

    <div class="row" style="display: none;">
        <div class="col-md-12" style="text-align: center;">
            <ul class="nav nav-pills">
                <li class="active"><a data-toggle="tab" id="homemenu" href="#home" style="margin: 10px; font-size: 18px; display: none;">Add Business Owner</a></li>
                <li><a data-toggle="tab" href="#menu1" id="menu1menu" class="btn btn-primary" style="margin: 10px; font-size: 18px; display: none;">Address and Contacts</a></li>
                <li><a data-toggle="tab" href="#menu4" id="menu4menu" class="btn btn-primary" style="margin: 10px; font-size: 18px; display: none;">User Role and Credential</a></li>
                <li><a data-toggle="tab" href="#menu3" id="menu3menu" class="btn btn-primary" style="margin: 10px; font-size: 18px; display: none;">Payment Info</a></li>
                <li><a data-toggle="tab" href="#menu2" id="menu2menu" class="btn btn-primary" style="margin: 10px; font-size: 18px; display: none;">Assign Menu</a></li>
            </ul>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="tab-content">
                <div id="home" class="tab-pane fade in active show">

                    <div class="col-md-2">
                        <div style="margin-top:-5px;margin-left:-80px;">
                                <!-- Upload Area -->
                                <center>
                                    <div id="uploadArea" class="upload-area">
                                        <!-- Drop Zoon -->
                                        <div id="dropZoon" class="upload-area__drop-zoon drop-zoon">
                                            <span class="drop-zoon__icon">
                                                <%--<i class='fa fa-image'></i>--%>
                                            </span>
                                            <p style="font-weight: bolder; font-size:medium;margin-top:12px;" class="drop-zoon__paragraph">Click Here to Upload Image</p>
                                            <span id="loadingText" class="drop-zoon__loading-text"></span>
                                            <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" class="drop-zoon__preview-image" Height="180px" ImageUrl="" />
                                            <asp:FileUpload
                                                runat="server"
                                                ID="FileUpload1"
                                                type="file"
                                                name="file"
                                                Text="Upload Photo"
                                                class="drop-zoon__file-input"
                                                accept="image/*" />
                                        </div>
                                        <!-- End Drop Zoon -->
                                    </div>
                                </center>
                                <!-- End Upload Area -->

                                <script type="text/javascript">
                                    $(function () {
                                        // Select Upload-Area
                                        var uploadArea = document.querySelector('#uploadArea');
                                        var dropZoon = document.querySelector('#dropZoon');
                                        var loadingText = document.querySelector('#loadingText');
                                        var fileInput = document.querySelector('#<%=FileUpload1.ClientID %>');
                                        var previewImage = document.querySelector('#previewImage');
                                        var imagesTypes = [
                                            "jpeg",
                                            "png",
                                            "svg",
                                            "gif"
                                        ];

                                        // When (drop-zoon) has (dragover) Event 
                                        dropZoon.addEventListener('dragover', function (event) {
                                            // Prevent Default Behavior 
                                            event.preventDefault();
                                            dropZoon.classList.add('drop-zoon--over');
                                        });

                                        // When (drop-zoon) has (dragleave) Event 
                                        dropZoon.addEventListener('dragleave', function (event) {
                                            dropZoon.classList.remove('drop-zoon--over');
                                        });

                                        // When (drop-zoon) has (drop) Event 
                                        dropZoon.addEventListener('drop', function (event) {
                                            event.preventDefault();
                                            dropZoon.classList.remove('drop-zoon--over');
                                            const file = event.dataTransfer.files[0];
                                            uploadFile(file);
                                        });

                                        // When (drop-zoon) has (click) Event 
                                        dropZoon.addEventListener('click', function (event) {
                                            fileInput.click();
                                        });

                                        // When (fileInput) has (change) Event 
                                        fileInput.addEventListener('change', function (event) {
                                            var file = event.target.files[0];
                                            uploadFile(file);
                                        });

                                        // Upload File Function
                                        function uploadFile(file) {
                                            var fileReader = new FileReader();
                                            var fileType = file.type;
                                            var fileSize = file.size;

                                            // If File Is Passed from the (File Validation) Function
                                            if (fileValidate(fileType, fileSize)) {
                                                dropZoon.classList.add('drop-zoon--Uploaded');
                                                loadingText.style.display = "block";
                                                previewImage.style.display = 'none';
                                                $('.drop-zoon__paragraph').show();
                                                // After File Reader Loaded 
                                                fileReader.addEventListener('load', function () {
                                                    setTimeout(function () {
                                                        uploadArea.classList.add('upload-area--open');
                                                        loadingText.style.display = "none";
                                                        previewImage.style.display = 'block';

                                                    }, 500); // 0.5s

                                                    // Add The (fileReader) Result Inside (previewImage) Source
                                                    previewImage.setAttribute('src', fileReader.result);
                                                });

                                                // Read (file) As Data Url 
                                                fileReader.readAsDataURL(file);
                                            } else {
                                                this; // (this) Represent The fileValidate(fileType, fileSize) Function
                                            };
                                        };

                                        // Simple File Validate Function
                                        function fileValidate(fileType, fileSize) {
                                            // File Type Validation
                                            let isImage = imagesTypes.filter(function (type) {
                                                return fileType.indexOf(`image/${type}`) !== -1
                                            });
                                            // If The Uploaded File Is An Image
                                            if (isImage.length !== 0) {
                                                // Check, If File Size Is 2MB or Less
                                                if (fileSize <= 2000000) { // 2MB :)
                                                    return true;
                                                } else { // Else File Size
                                                    return alert('Please Your File Should be 2 Megabytes or Less');
                                                };
                                            } else { // Else File Type 
                                                return alert('Please make sure to upload An Image File Type');
                                            };
                                        };
                                    });
                                </script>
                        </div>
                    </div>

                    <div class="col-md-8">

                        <div class="row">

                            <%--Earlier the file upload control was in this div for profile pic so not deleted this div--%>
                            <%--<div class="col-lg-4 col-md-4">
                                <br />
                                <br />
                                <br />
                                <br />

                                
                            </div>--%>

                            <div class="col-md-12">
                                <div class="row text-center">
                                    <%--<h4>New Subscriber Registration</h4>--%>
                                </div>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">First Name</label>
                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Last Name</label>
                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6" style="display: none;">
                                        <div class="form-group">
                                            <label for="Name">Company Name</label>
                                            <asp:TextBox ID="TextBox_Company_Name" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                            

                                    <asp:UpdatePanel ID="up11" runat="server">
                                        <ContentTemplate>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Email</label>
                                                    <asp:TextBox ID="txtMainEmail" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                        OnTextChanged="txtMainEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                    <div id="divValid" runat="server" visible="false">
                                                        <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">The email id you've entered is not vaild. Please try again.
                                                        </span>
                                                    </div>
                                                    <asp:HiddenField ID="hfdMainEmail" runat="server" />
                                                    <cc1:AutoCompleteExtender ServiceMethod="GetCompletionList"
                                                        MinimumPrefixLength="1"
                                                        CompletionInterval="10"
                                                        EnableCaching="false"
                                                        CompletionSetCount="1"
                                                        TargetControlID="txtMainEmail"
                                                        ID="AutoCompleteExtender1"
                                                        runat="server"
                                                        FirstRowSelected="false">
                                                    </cc1:AutoCompleteExtender>

                                                </div>
                                            </div>
                                            <div class="col-md-6" id="divconfirm" runat="server" visible="false">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="form-group">
                                                            <label for="Name">Confirm Email</label>
                                                            <asp:TextBox ID="txtConLoginEmail" ClientIDMode="Static" runat="server" CssClass="form-control" OnTextChanged="txtConLoginEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                                            <div id="div1msg" runat="server">
                                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">Please re-enter the email id.</span>
                                                            </div>
                                                            <div id="div2stmsg" runat="server" visible="false">
                                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">Email and Confirm Email is not matching, please verify.
                                                                </span>
                                                            </div>


                                                        </div>
                                                    </div>
                                                    <div class="col-md-1" style="display: none;">
                                                        <br />
                                                        <div id="divtick" runat="server" visible="false" style="margin-top: 20px;">
                                                            <span class="fa fa-check fa-3x" aria-hidden="true" style="color: green"></span>
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </ContentTemplate>
                                        <Triggers>
                                            <asp:AsyncPostBackTrigger ControlID="txtMainEmail" />
                                            <asp:AsyncPostBackTrigger ControlID="txtConLoginEmail" />
                                        </Triggers>
                                    </asp:UpdatePanel>
                                </div>

                                <div class="row">
                                    <div class="col-lg-4 col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Phone</label>
                                            <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-4 col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Ext</label>
                                            <asp:TextBox ID="txtExtPh" MaxLength="4" onkeypress="return isNumberKey(event)"
                                                runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4 col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Mobile</label>
                                            <asp:TextBox ID="txt_mobilenumber" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-2" style="display: none;">
                                        <div class="form-group">
                                            <label for="Name">Ext</label>
                                            <asp:TextBox ID="txtExtMob" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                MaxLength="4" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>




                            </div>

                        </div>

                        <div class="row">
                            <%--<div class="col-lg-4 col-md-4"></div>--%>
                            <div class="col-md-12">
                                                <label for="Name">Enter Postal Code</label>&nbsp;
                                        <%--<span style="color: black; font-weight: bolder;">(Required)</span>--%>
                                                <div class="row">
                                                    <div class="col-md-3">
                                                        <div class="form-group">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="TextBox_SearchPostalCode" Width="150px" MaxLength="6" Style="text-transform: uppercase"
                                                                            runat="server" CssClass="form-control" 
                                                                            ClientIDMode="Static"></asp:TextBox> <%--onkeyup="addHyphenn(this)"--%>
                                                                        <script>
                                                                            function addHyphenn(element) {
                                                                                let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                                let n = val.length;
                                                                                let finalVal = val;
                                                                                var number_regex = /^[0-9]\d*$/;


                                                                                let c = val.charAt(n - 1);
                                                                                if (n == 2 || n == 4 || n == 6) { //num    
                                                                                    if (!number_regex.test(c)) {
                                                                                        finalVal = val.slice(0, -1);
                                                                                    }
                                                                                }
                                                                                else if (n == 1 || n == 3 || n == 5) {
                                                                                    if (number_regex.test(c)) {
                                                                                        finalVal = val.slice(0, -1);
                                                                                    }
                                                                                }
                                                                                if (n <= 6) {
                                                                                    try {
                                                                                        finalVal = finalVal.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                                    }
                                                                                    catch (err) { }
                                                                                    // Update the input box.
                                                                                }
                                                                                $(element).val(finalVal);
                                                                            }
                                                                        </script>

                                                                    </td>
                                                                    <td>
                                                                        <asp:LinkButton Style="font-size: x-large;" Height="35.5px" CssClass="buttonClass"
                                                                            ID="LinkButton_Search_Address" runat="server" OnClientClick="return validatefields();"
                                                                            Text="" OnClick="LinkButton_Search_Address_Click"><i class="fa fa-search" aria-hidden="true"></i>
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                        </div>

                        <div class="row">
                                            <%--<div class="col-lg-4 col-md-4"></div>--%>
                                            <div class="col-md-12">
                                                <div id="Div_Search_Address_List" style="border: 0px solid #349fce; border-radius: 0px; padding: 0px;"
                                                    runat="server" visible="false">
                                                    <div>
                                                        <span style="color: gray;">SELECT THE ADDRESS FROM THE LIST BELOW OR ADD NEW</span>
                                                        <%--<span style="color: black; font-weight: bolder">(Required)</span>--%>
                                                    </div>
                                                    <div>
                                                        <asp:GridView ID="gvAddressList" runat="server" ClientIDMode="Static"
                                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False"
                                                            GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found">
                                                            <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                                            <AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />
                                                            <%--<HeaderStyle CssClass="bg-clouds" />--%>
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Address">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="LinkButton_Copy_Searched_Address" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SEARCHADDRESS")%>'
                                                                            OnClick="LinkButton_Copy_Searched_Address_Click" OnClientClick="GetSelectedRow(this)" />
                                                                        <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>



                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div>No records found.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                </div>

                                        <div id="divAddres" runat="server" style="display: none">
                                            <div class="row">
                                                <div class="col-md-12">
                                                    <h4 style="text-decoration: none;">Insert New Address</h4>
                                                    <div class="row">
                                                        <div class="col-lg-4 col-md-4">
                                                            <div class="form-group">
                                                                <label for="Name">Street Number</label><%--&nbsp;<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="TextBox_Street_Number" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-8">
                                                            <div class="form-group">
                                                                <label for="Name">Address - Line 1</label><%--&nbsp;<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="TextBox_AddressLine1" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <div class="form-group">
                                                                <label for="Name">Address - Line 2</label>
                                                                <asp:TextBox ID="TextBox_AddressLine2" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label for="Name">Suite Number</label>
                                                                <asp:TextBox ID="TextBox_SuiteNumber" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label>Suite Type</label><br />
                                                                <asp:DropDownList ID="DropDownList_SuiteType"
                                                                    CssClass="form-control" AppendDataBoundItems="true" runat="server" Width="100%">
                                                                    <asp:ListItem Value="Apartment">Apartment</asp:ListItem>
                                                                    <asp:ListItem Value="Suite">Suite</asp:ListItem>
                                                                    <asp:ListItem Value="Unit">Unit</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label for="Name">City</label><%--&nbsp;<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="TextBox_City" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                                <cc1:AutoCompleteExtender ServiceMethod="GetAutoCompleteDataCity"
                                                                    MinimumPrefixLength="1"
                                                                    CompletionInterval="10"
                                                                    EnableCaching="false"
                                                                    CompletionSetCount="3"
                                                                    TargetControlID="txtCity"
                                                                    ID="AutoCompleteExtender2"
                                                                    runat="server"
                                                                    FirstRowSelected="false">
                                                                </cc1:AutoCompleteExtender>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label for="Name">Province/State</label><%--&nbsp;<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="TextBox_Province" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                                <cc1:AutoCompleteExtender ServiceMethod="GetAutoCompleteDataProvince"
                                                                    MinimumPrefixLength="1"
                                                                    CompletionInterval="10"
                                                                    EnableCaching="false"
                                                                    CompletionSetCount="3"
                                                                    TargetControlID="txtProvince"
                                                                    ID="AutoCompleteExtender3"
                                                                    runat="server"
                                                                    FirstRowSelected="false">
                                                                </cc1:AutoCompleteExtender>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label for="Name">Postal Code</label><%--&nbsp;<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="TextBox_PostalCode" MaxLength="6" runat="server" CssClass="form-control"
                                                                    ClientIDMode="Static" Style="text-transform: uppercase"></asp:TextBox> <%--onkeyup="addHyphennew(this)"--%>
                                                                <script>
                                                                    /* firoz 30*/
                                                                    function GetSelectedRow(lnk) {

                                                                        var row = lnk.parentNode.parentNode;

                                                                        if (row.rowIndex === 0 || row.rowIndex === 1) {
                                                                            $("#NextAdd").attr("style", "display: block");
                                                                        }
                                                                    }
                                                                    /*End*/

                                                                    function addHyphennew(element) {
                                                                        let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                        let n = val.length;
                                                                        let finalVal = val;
                                                                        var number_regex = /^[0-9]\d*$/;


                                                                        let c = val.charAt(n - 1);
                                                                        if (n == 2 || n == 4 || n == 6) { //num    
                                                                            if (!number_regex.test(c)) {
                                                                                finalVal = val.slice(0, -1);
                                                                            }
                                                                        }
                                                                        else if (n == 1 || n == 3 || n == 5) {
                                                                            if (number_regex.test(c)) {
                                                                                finalVal = val.slice(0, -1);
                                                                            }
                                                                        }
                                                                        if (n <= 6) {
                                                                            try {
                                                                                finalVal = finalVal.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                            }
                                                                            catch (err) { }
                                                                            // Update the input box.
                                                                        }
                                                                        $(element).val(finalVal);
                                                                    }
                                                                </script>
                                                            </div>
                                                        </div>
                                                    </div>


                                                </div>
                                            </div>

                                        </div>

                                            </div>
                                        
                                        

                            </div>

                        <%--firoz--%>
                        <%--<div class="container">--%>
                            <div class="row">
                                <%--<div class="col-md-12">--%>
                                    <div class="pricing-area">
                                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                                            <div class="single-price">
                                                <div class="deal-top">
                                                    <h3>Basic Plan</h3>
                                                    <h4>FREE <span class="sup"></span></h4>
                                                </div>
                                                <div class="deal-bottom">
                                                    <ul class="deal-item">
                                                        <li>1 Business</li>
                                                        <li>5 Users</li>
                                                        <li>24 Hours Support</li>
                                                    </ul>
                                                    <div class="btn-area">
                                                        <%--<a href="#">Sign Up</a>--%>
                                                        <asp:LinkButton ID="LinkButton1" runat="server" OnClick="Button_Register_As_A_BO_Click"
                                                            Text="Sign Up" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>                                    
                                    </div>
                                        <script type="text/javascript">
                                            $(function () {
                                                var plans = JSON.parse($('[id*="hdf_stripePlans"]').val());
                                                var templatePlan = "";
                                                var plansWithPrice = plans.data.filter(function (plan) {
                                                    return plan.default_price !== null && ["BASIC", "STANDARD LAN", "STANDARD PLAN"].includes(plan.name.toUpperCase());
                                                });
                                                plansWithPrice.forEach(function (plan) {
                                                    var metaTemplate = "";
                                                    Object.values(plan.metadata).forEach(function (meta) {
                                                        metaTemplate = metaTemplate + `<li>${meta}</li>`;
                                                    });
                                                    var metadataTemplate = `<ul class="deal-item">
                                                                        ${metaTemplate}
                                                                    </ul>`
                                                    var priceInfo = "";
                                                    if (plan.default_price) {
                                                        priceInfo = `<h4>${plan.default_price.unit_amount / 100} ${plan.default_price.currency.toUpperCase()}</h4>`;
                                                    }
                                                    var template = `<div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                                                            <div class="single-price">
                                                                <div class="deal-top">
                                                                    <h3>${plan.name}</h3>
                                                                    ${priceInfo}
                                                                </div>
                                                                <div class="deal-bottom">
                                                                   ${metadataTemplate}
                                                                    <div class="btn-area">
                                                                        <a class="signUp" href="javascript:void(0)"
                                                                        data-pr="${plan.default_price.id}" 
                                                                        data-pl="${plan.id}">Sign Up</a>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>`;
                                                    templatePlan = templatePlan + template;
                                                });

                                                $(".pricing-area").append(templatePlan);

                                                $(".signUp").on("click", function () {
                                                    $('[id*="hdf_selectedPlanPriceId"]').val($(this).data("pr"));
                                                    $('[id*="hdf_selectedPlanId"]').val($(this).data("pl"));
                                                    document.getElementById("<%=LinkButton1.ClientID %>").click();
                                                });
                                            });

                                        </script>

                                        <div class="col-lg-4 col-md-4 col-sm-6 col-xs-12">
                                            <div class="single-price">
                                                <div class="deal-top">
                                                    <h3>Enterprise Plan</h3>
                                                    <h4 style="color: #16A086;"><span class="sup">.</span></h4>
                                                </div>
                                                <div class="deal-bottom">
                                                    <ul class="deal-item">
                                                        <li>Contact Us</li>
                                                          <li>Customize as per Requirement</li>
                                                          <li>24 Hours Support</li>
                                                    </ul>
                                                    <div class="btn-area">
                                                        <%--<a href="#">Sign Up</a>--%>
                                                        <asp:LinkButton ID="Add_EnterprisePlan" runat="server" OnClick="Add_EnterprisePlan_Click"
                                                                        ClientIDMode="Static" Text="Sign Up"/>
                                                          <asp:HiddenField ID="HiddenField_CheckEnterprise_Plan" runat="server" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    
                                <%--</div>--%>
                            </div>
                        <%--</div>--%>
                        <%--end--%>

                    </div>
                    
                    
                    
                    
                    <div class="row">
                        <div class="col-lg-4 col-md-4"></div>
                        <div class="col-lg-4 col-md-4 text-center">
                            <div class="form-group">
                                <asp:LinkButton ID="Button_Register_As_A_BO" runat="server" Width="50%" OnClick="Button_Register_As_A_BO_Click"
                                    ClientIDMode="Static" Visible="false" Text="Save" CssClass="btn2 btn-primary" />
                                <asp:LinkButton ID="UpdateButton_Register_As_A_BO" runat="server" Width="50%"
                                    ClientIDMode="Static" Visible="false" Text="Save" CssClass="btn2 btn-primary" Style="margin-top: -20px;" />
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4"></div>
                    </div>






                    <div class="row" style="display: none;">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="title">User Type</label>
                                
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">Title</label>
                                <asp:TextBox ID="txtLegalName1" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                <asp:DropDownList ID="txtLegalName" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                    <asp:ListItem Value="Mr.">Mr.</asp:ListItem>
                                    <asp:ListItem Value="Miss.">Miss.</asp:ListItem>
                                    <asp:ListItem Value="Ms.">Ms.</asp:ListItem>
                                    <asp:ListItem Value="Mx.">Mx.</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4">
                        </div>
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">Middle Name</label>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-4 col-md-4">
                        </div>
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="txtdob">Date of Birth</label>
                                <asp:TextBox ID="txtdob" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                    type="date" max="9999-12-31" placeholder="DD/MM/YYYY"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="Reqtxtdob" ValidationGroup="persnoldetails" runat="server"
                                    ControlToValidate="txtdob" Display="None" ErrorMessage="This Field Is Requried" />
                                <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server" TargetControlID="Reqtxtdob"
                                    PopupPosition="Right" HighlightCssClass="validatorCalloutHighlight">
                                    <Animations>
                                          <OnShow>
                                           <Sequence>
                                            <HideAction Visible="true" />
                                              <FadeIn Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                           </Sequence>
                                          </OnShow>

                                           <OnHide>
                                           <Sequence>
                                           <FadeOut Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                           <HideAction Visible="false" />
                                           </Sequence>
                                           </OnHide>
                                    </Animations>
                                </cc1:ValidatorCalloutExtender>
                            </div>
                        </div>
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="title">Gender</label>
                                <asp:DropDownList ID="ddlgender" runat="server" CssClass="form-control"
                                    Width="100%">
                                    <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                    <asp:ListItem Value="Male">Male</asp:ListItem>
                                    <asp:ListItem Value="Female">Female</asp:ListItem>
                                    <asp:ListItem Value="Others">Others</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">Social Insurance Number</label>
                                <asp:TextBox ID="txtSocialNumer" runat="server" placeholder="## ### ##" CssClass="form-control"></asp:TextBox>


                            </div>
                        </div>



                    </div>
                    <div class="row">
                        <div class="col-md-12" style="display: none;">
                            <h4 style="text-decoration: none;">Contact Details</h4>
                        </div>


                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">City</label>
                                <asp:TextBox ID="txt_city" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">Province</label>
                                <asp:TextBox ID="txt_province" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>


                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <%--<asp:CheckBox Visible="true" Text="Same as Login Email"
                                    OnClick="copyvalue1(this)" runat="server" />--%>
                                <label for="Name">Contact Email Same as Login Email?</label>
                                <asp:DropDownList ID="ddlCopyLoginEmail" runat="server" onchange="Copymail(this)"
                                    CssClass="form-control">
                                    <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                    <asp:ListItem Value="No">No</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6" style="display: none;">
                            <div class="form-group">
                                <label for="Name">Contact Email</label>&nbsp;<asp:CheckBox Visible="false" Text="Same as Login Email"
                                    OnClick="copyvalue1(this)" runat="server" />
                                <asp:TextBox ID="txtCCEmail" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="hfdCCEmail" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                    </div>

                    <div class="row" style="display: none;">
                        <div class="col-md-12">
                            <h4 style="border-bottom: 2px solid #349fce">Employment Details</h4>
                        </div>
                        <div class="col-md-12">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="txtdob">Hire Date</label>
                                        <asp:TextBox ID="txtHiredate" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                            type="date"
                                            placeholder="DD/MM/YYYY"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ReqtxtHiredate" ValidationGroup="persnoldetails"
                                            runat="server" ControlToValidate="txtHiredate" Display="None" ErrorMessage="This Field Is Requried" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender7" runat="server" TargetControlID="ReqtxtHiredate"
                                            PopupPosition="Right"
                                            HighlightCssClass="validatorCalloutHighlight">
                                            <Animations>
                                          <OnShow>
                                          <Sequence>
                                          <HideAction Visible="true" />
                                         <FadeIn Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                         </Sequence>
                                         </OnShow>

                                        <OnHide>
                                        <Sequence>
                                        <FadeOut Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                        <HideAction Visible="false" />
                                        </Sequence>
                                        </OnHide>
                                            </Animations>
                                        </cc1:ValidatorCalloutExtender>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">

                                        <label for="txtdob">Release Date</label>
                                        <asp:TextBox ID="txtReleaseDate" runat="server" class="form-control" Max="9999-12-31"
                                            type="date"></asp:TextBox>


                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label for="Name">Occupation</label>
                                        <asp:TextBox ID="txtOccupation" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="title">Is Employee EI Exempty?</label>
                                        <asp:DropDownList ID="ddlEmpEI" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label for="title">Is Employee CPP Exempty?</label>
                                        <asp:DropDownList ID="ddlEmpCPP" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <label for="title">Retain Vacation?</label>
                                        <asp:DropDownList ID="ddlRTVA" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                </div>
                            </div>

                        </div>

                        <div class="col-md-3">
                            <div class="form-group">
                                <asp:Button ID="btnSavePersonalDetails" Visible="false" runat="server" formnovalidate
                                    ValidationGroup="persnoldetails" 
                                    Width="100%" Style="margin-bottom: -12px;" CssClass="btn btn-primary" Text="Save" />

                            </div>
                        </div>
                    </div>

                    <div class="row" runat="server" style="display: none;">
                        <div class="col-md-12">
                            <h4 style="border-bottom: 2px solid #349fce">Payment Info</h4>
                        </div>
                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-lg-4 col-md-4">
                                    <div class="form-group">
                                        <label for="title">Billing Type</label>
                                        <asp:DropDownList ID="ddlBillingType" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Hourly">Hourly</asp:ListItem>
                                            <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                            <asp:ListItem Value="Bi-Weekly">Bi-Weekly</asp:ListItem>
                                            <asp:ListItem Value="Forth Nightly">Forth Nightly</asp:ListItem>
                                            <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                            <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4">
                                    <div class="form-group">
                                        <label for="txtCity">Billing Rate($)</label>
                                        <asp:TextBox ID="txtBillRate" runat="server" CssClass="form-control"></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="ReqtxtBillRate"
                                            ControlToValidate="txtBillRate" runat="server"
                                            Display="None"
                                            ErrorMessage="Only Numbers allowed"
                                            ValidationExpression="^[1-9]\d*(\.\d+)?$">
                                        </asp:RegularExpressionValidator>
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender1" runat="server" TargetControlID="ReqtxtBillRate"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                    </div>
                                </div>

                                <div class="col-lg-4 col-md-4">
                                    <div class="form-group" style="display: none;">
                                        <%--<label for="effectiv">Effective Date</label>--%>
                                        <label for="effectiv">Start Date</label>
                                        <asp:TextBox ID="txteffectivedate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <%--<div class="col-md-2">
                               <div class="form-group">
                                   <div style="height:30px">

                                       </div>

                                    </div>
                                </div>--%>
                                <div class="col-md-2" style="display: none;">
                                    <div class="form-group">
                                        <label for="title">Job Title</label>
                                        <asp:DropDownList ID="ddlJobTitle" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Cashier">Cashier</asp:ListItem>
                                            <asp:ListItem Value="Food Handler">Food Handler</asp:ListItem>
                                            <asp:ListItem Value="Cook">Cook</asp:ListItem>
                                            <asp:ListItem Value="Manager">Manager</asp:ListItem>
                                            <asp:ListItem Value="Contact Center Manager">Contact Center Manager</asp:ListItem>
                                            <asp:ListItem Value="Others">Others</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <asp:Button ID="btnSavePay" Visible="false" runat="server" Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                            formnovalidate Text="Save"
                                            CssClass="btn btn-primary" />
                                        <asp:HiddenField ID="hfdEditRateId" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-4 col-md-4">
                                    <div class="form-group">
                                        <asp:LinkButton ID="lnkratechange" Visible="false" CssClass="btn btn-primary" runat="server"
                                            Text="Change Rate" ></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="col-md-2"></div>
                        <div class="col-md-8"></div>
                        <div class="col-md-2">

                            <a href="#" id="NextPer" class="btn btn-primary btn-round pull-right" style="margin: 10px; font-size: 18px;">Next
                            </a>


                        </div>
                        <script>
                            $("#NextPer").click(function () {
                                $("#home").attr("class", "tab-pane fade in")
                                $("#menu1").attr("class", "tab-pane fade in active show")
                            });
                        </script>
                    </div>



                    <div class="col-md-2">


                    </div>

                </div>

                <div class="row">
                </div>

                <div class="col-md-3">
                </div>

                <div class="col-md-6">

                    <div class="row">
                        <div class="col-md-2" style="display: none;">
                            <asp:Button ID="btnsaveRole" runat="server" formnovalidate OnClick="btnsaveRole_Click"
                                Text="Save" CssClass="btn btn-primary" Style="margin: 15px; margin-top: -20px; width: 100px;" />
                        </div>
                        <div class="col-md-0"></div>
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-5"></div>
                        <div class="col-md-12">
                            <center>
                                <br />
                                <asp:HyperLink Style="font-size: x-large" Text="Back To Login Page" ID="HyperLink_Back_To_Login_Page" NavigateUrl="Login.aspx" Visible="false" runat="server" />
                            </center>
                        </div>
                    </div>

                </div>

                <div class="col-md-3">
                </div>


                <div style="display: none">
                    <div id="menu1" class="tab-pane fade in">

                        <div class="row" style="border-bottom: 2px solid #349fce">
                            <div class="col-md-6">
                                <h4>Address and Contacts</h4>
                            </div>
                            <div class="col-md-6">
                                <div class="pull-right">
                                    <h4>
                                        <asp:Label ID="lblEmpNameForAddressCont" runat="server" Visible="false"></asp:Label>
                                    </h4>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-2">
                                <div class="form-group">
                                    <label for="Name">*Enter Postal Code</label>
                                    <asp:TextBox ID="txtSearchPostalCode" runat="server" CssClass="form-control"></asp:TextBox>

                                </div>

                            </div>

                            <div class="col-md-2">
                                <div class="form-group">
                                    <div style="height: 28.5px">
                                    </div>
                                    <asp:LinkButton Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                        ID="lnkSearch" runat="server" CssClass="btn btn-primary" Text="" >
                                            Click For Search <i class="fa fa-search" aria-hidden="true"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div id="resultdiv" class="box" runat="server" visible="false">
                                    
                                </div>
                            </div>


                        </div>


                        <h4 style="border-bottom: 2px solid #349fce">Insert New Address</h4>
                        <div class="row">
                            <div class="col-lg-4 col-md-4">
                                <div class="box1">
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Street Number</label>
                                            <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group" style="display: none;">
                                        <div class="col-md-6">
                                            <label for="ddlnumbersuffix">Number Suffix</label>
                                            <asp:DropDownList ID="ddlNumberSuffix"
                                                CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                                Width="100%">
                                                <asp:ListItem Text="Please Select" Value=""></asp:ListItem>
                                                <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                                <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                                <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                                <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                                <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                                <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>


                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label for="Name">Street 1</label>
                                            <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>



                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label for="Name">Street 2</label>
                                            <asp:TextBox ID="txtStreetType" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>



                                    <div class="col-md-9">
                                        <div class="form-group" style="visibility: hidden;">
                                            <label for="Name">Street Direction</label>
                                            <asp:TextBox ID="txtStreetDirection" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>

                                        </div>

                                    </div>


                                    <div class="col-md-10">
                                        <div class="form-group" style="display: none;">
                                            <label>SuiteType</label><br />
                                            <asp:DropDownList ID="ddlSuitType"
                                                CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                                Width="100%">
                                                <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                <asp:ListItem Value="Duplex" Text="Duplex"></asp:ListItem>
                                                <asp:ListItem Value="Simplex" Text="Simplex"></asp:ListItem>
                                                <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                                <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                                <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>


                                </div>


                            </div>

                            <div class="col-lg-4 col-md-4">
                                <div class="box1">
                                    <div class="form-group" style="display: none;">
                                        <div class="col-md-10">
                                            <label for="Name">Street Type</label>
                                            <asp:TextBox ID="txtStreetType1" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group" style="display: none;">
                                        <div class="col-md-8">
                                            <label for="Name">Street Direction</label>
                                            <asp:TextBox ID="txtStreetDirection1" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="col-md-9" style="display: none;">
                                        <div class="form-group">
                                            <label for="ddlsuitetype2">SuiteType</label><br />
                                            <asp:DropDownList ID="ddlSuitType1"
                                                CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                                Width="100%">
                                                <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                <asp:ListItem Value="Duplex" Text="Duplex"></asp:ListItem>
                                                <asp:ListItem Value="Simplex" Text="Simplex"></asp:ListItem>
                                                <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                                <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                                <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>



                                    <div class="col-md-7">
                                        <div class="form-group">
                                            <label for="Name">Suite Number</label>
                                            <asp:TextBox ID="txtSuiteNumber" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group" style="display: none;">
                                            <label>SuiteSuffix</label><br />
                                            <asp:DropDownList ID="ddlsuitesuffix"
                                                CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                                Width="100%">
                                                <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                <asp:ListItem Value="Du" Text="Du"></asp:ListItem>
                                                <asp:ListItem Value="Si" Text="Si"></asp:ListItem>
                                                <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                                <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                                <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>


                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label for="Name">City</label>
                                            <asp:TextBox ID="txtCity" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label for="Name">Province</label>
                                            <asp:TextBox ID="txtProvince" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-md-7">
                                        <div class="form-group">
                                            <label for="Name">Postal Code</label>
                                            <asp:TextBox ID="txtPostalCode" runat="server" CssClass="form-control"></asp:TextBox>
                                            <asp:RegularExpressionValidator Display="Dynamic" ControlToValidate="txtPostalCode"
                                                ID="RegularExpressionValidator3" ValidationExpression="^[a-zA-Z0-9'@&#.\s]{6,6}$"
                                                runat="server" ErrorMessage="Postal Code length must be 6 characters"></asp:RegularExpressionValidator>

                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>





                        <h4 style="border-bottom: 2px solid #349fce">Add Contacts</h4>
                        <div class="row">
                            <div class="col-lg-4 col-md-4">
                                <div class="box1">
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Home Phone</label>

                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Work Phone</label>
                                            <asp:TextBox ID="txtWorkPhone" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Mobile</label>
                                            <asp:TextBox ID="txtMobile" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Fax</label>
                                            <asp:TextBox ID="txtFax" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4 col-md-4">
                                <div class="box1">
                                    <div class="form-group">
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Secondary Email</label>

                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Website</label>
                                            <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-12">
                                            <label for="Name">Other1</label>
                                            <asp:TextBox ID="txtOther1" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="row">
                            <div class="col-lg-4 col-md-4">
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <asp:Button ID="btnSaveAddres" Width="100%" Visible="false" Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                            ValidationGroup="AddressBusinessDetails"
                                            CssClass="btn btn-primary" Text="Save"
                                            runat="server"></asp:Button>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-2">

                                <a data-toggle="tab" id="PreviousAdd" href="#home" class="btn btn-primary btn-round pull-left"
                                    style="margin: 10px; font-size: 18px;">Previous

                                </a>
                                <script>
                                    $("#PreviousAdd").click(function () {
                                        $("#home").attr("class", "tab-pane fade in active show")
                                        $("#menu1").attr("class", "tab-pane fade in")
                                    });
                                </script>
                            </div>
                            <div class="col-md-8"></div>
                            <div class="col-md-2">

                                <a data-toggle="tab" href="#menu4" id="NextAdd" class="btn btn-primary btn-round pull-right"
                                    style="margin: 10px; font-size: 18px;">Next
                                </a>
                                <script>
                                    $("#NextAdd").click(function () {

                                        $("#menu1").attr("class", "tab-pane fade in")
                                        $("#menu4").attr("class", "tab-pane fade in active show")
                                    });
                                </script>

                            </div>
                        </div>


                    </div>

                    <div id="menu4" class="tab-pane fade in">


                        <div class="row" style="border-bottom: 2px solid #349fce">
                            <div class="col-md-6">
                                <h4>User Role</h4>
                            </div>
                            <div class="col-md-6">
                                <div class="pull-right">
                                    <h4>
                                        <asp:Label ID="lblEmpNameForUserCredentials" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>


                        <style>
                            .boxshadow {
                                -webkit-box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
                                -moz-box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
                                box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
                            }
                        </style>
                        <div class="row">
                            <div class="col-lg-3 col-md-3 boxshadow" style="border: 1px solid #808080; padding: 10px; margin: 10px; border-radius: 5px;">
                                <div class="form-group">

                                    <asp:RadioButtonList ID="rdoBtnUserTypeList" runat="server" RepeatDirection="Horizontal"
                                        CellPadding="10" CellSpacing="50" RepeatColumns="2">
                                    </asp:RadioButtonList>
                                </div>
                            </div>
                            <div class="col-lg-3" style="padding: 10px; margin: 10px;">
                            </div>
                        </div>
                        <br />

                        <div class="row">
                            <div class="col-md-2">

                                <a data-toggle="tab" href="#menu1" id="PreviousUser" class="btn btn-primary btn-round pull-left"
                                    style="margin: 10px; font-size: 18px;">Previous

                                </a>
                                <script>
                                    $("#PreviousUser").click(function () {
                                        $("#menu4").attr("class", "tab-pane fade in")
                                        $("#menu1").attr("class", "tab-pane fade in active show")
                                    });
                                </script>

                            </div>
                        </div>

                    </div>
                    <div id="menu3" class="tab-pane fade in">


                        <div class="row" style="border-bottom: 2px solid #349fce">
                            <div class="col-md-6">
                                <h4>Payment Info</h4>
                            </div>
                            <div class="col-md-6">
                                <div class="pull-right">
                                    <h4>
                                        <asp:Label ID="lblEmpNameForPaymentInfo" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="form-group">
                                <asp:Label ID="lblNameempRateHistory" runat="server"></asp:Label>
                            </div>
                        </div>
                        <%--<div class="row">--%>
                        <div class="row">


                            <div class="col-md-12" style="margin-top: 10px;">
                                <div class="row" style="padding: 10px;">



                                    <div class="row">

                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <asp:Label ID="Label1" runat="server"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-3">
                                            <div class="form-group">

                                                <asp:DropDownList ID="ddlrateHistoryBillType" runat="server" CssClass="form-control"
                                                    ValidationGroup="AddRateHistory">
                                                    <asp:ListItem Value="" Selected="True"> Select Rate Type</asp:ListItem>
                                                    <asp:ListItem Value="Hourly">Hourly</asp:ListItem>
                                                    <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                                    <asp:ListItem Value="Bi-Weekly">Bi-Weekly</asp:ListItem>
                                                    <asp:ListItem Value="Forth Nightly">Forth Nightly</asp:ListItem>
                                                    <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                                    <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="AddRateHistory"
                                                    runat="server" ControlToValidate="ddlrateHistoryBillType" Display="None" ErrorMessage="This Field Is Requried" />
                                                <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender8" runat="server" TargetControlID="RequiredFieldValidator1"
                                                    HighlightCssClass="validatorCalloutHighlight">
                                                </cc1:ValidatorCalloutExtender>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <asp:TextBox ID="txtrateHistoryBillRate" runat="server" TextMode="Number" ValidationGroup="AddRateHistory"
                                                    CssClass="form-control" placeholder="Enter Rate"></asp:TextBox>
                                                <asp:HiddenField ID="HiddenField1" runat="server" />

                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">

                                                <asp:TextBox ID="txtrateHistoryEffectiveDate" placeholder="Start Date" ValidationGroup="AddRateHistory"
                                                    runat="server" CssClass="form-control" onfocus="(this.type='date')" onfocusout="(this.type='text')"></asp:TextBox>


                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <asp:Button ID="btnrateHistoryChangeRate" runat="server" ValidationGroup="AddRateHistory"
                                                    CssClass="btn btn-primary" Text="Change Rate" />
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                
                            </div>
                        </div>



                        <div class="row" style="display: none;">
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="txtdob">Hire Date</label>

                                    <asp:TextBox ID="txthdate" runat="server" type="date" Max="9999-12-31" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="txtdob">Termination Date</label>
                                    <asp:TextBox ID="TxtTermDT" runat="server" class="form-control" type="date" Max="9999-12-31"
                                        placeholder="DD/MM/YYYY"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <br />
                        <br />


                    </div>




                    <div id="menu2" class="tab-pane fade in">

                        <div class="row" style="border-bottom: 2px solid #349fce">
                            <div class="col-md-6">
                                <h4>Assign Menu</h4>
                            </div>
                            <div class="col-md-6">
                                <div class="pull-right">
                                    <h4>
                                        <asp:Label ID="lblEmpNameAssignMenu" runat="server"></asp:Label></h4>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6">



                                <div class="row">
                                    <div class="col-lg-4 col-md-2   ">
                                        <div class="form-group ">
                                            <asp:Button ID="btnAssginelinSave" runat="server" formnovalidate Text="Assign Menu"
                                                Width="100%"
                                                Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                                CssClass="btn btn-primary"
                                                ValidationGroup="NumericValidate" />
                                            <br />
                                        </div>
                                    </div>




                                </div>
                            </div>

                            <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                                <div class="row">
                                    <div class="col-lg-6 col-md-6">
                                        <div class="form-group">

                                            <asp:TreeView ID="tvLinks" ClientIDMode="Static" runat="server" ImageSet="Simple2">
                                                <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                                <NodeStyle Font-Names="Verdana" Font-Size="14pt" ForeColor="Black"
                                                    HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                                <ParentNodeStyle Font-Bold="False" />
                                                <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD"
                                                    HorizontalPadding="0px" VerticalPadding="0px" />
                                            </asp:TreeView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

