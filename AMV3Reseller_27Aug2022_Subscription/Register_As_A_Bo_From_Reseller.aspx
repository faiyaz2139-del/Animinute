<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Register_As_A_Bo_From_Reseller.aspx.cs" Inherits="Register_As_A_Bo_From_Reseller" %>

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

    <script lang="Javascript" type="text/javascript">
        function NumbersOnly(evt) {
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

    <style>
        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .btn2 {
            background-color: #607DF2;
            border-radius: 1px;
            border: none;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .btn2:hover {
            background-color: #607DF2;
            border-radius: 1px;
            border: none;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>
    <link href="css/rounduploadimagecontrol.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmailConfirm" BehaviorID="mpe" runat="server"
        PopupControlID="pnlEmailConfirm" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>

    <asp:HiddenField ID="hfdEmpID" runat="server" />
    <asp:HiddenField ID="hfdUserId" runat="server" />

    <div class="row" style="display:none;">
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

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="text-align: left; margin-top: 12px;">Add Subscriber</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <asp:HyperLink ID="HyperLink_Back" runat="server" Text="Home" NavigateUrl="~/Accountant.aspx"
                        class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Accountant.aspx" id="A1" runat="server" style="font-size:large;" >
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Add Subscriber" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Accountant.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="tab-content">
                <div id="home" class="tab-pane fade in active show">

                    <div class="col-md-3">
                        <div style="margin-top:-5px;margin-left:-120px;">
                            <!-- Upload Area -->
                            <center>
                            <div id="uploadArea" class="upload-area">
                                <!-- Drop Zoon -->
                                <div id="dropZoon" class="upload-area__drop-zoon drop-zoon">
                                    <span class="drop-zoon__icon">
                                        <%--<i class='fa fa-image'></i>--%>
                                    </span>
                                    <p style="font-weight:bolder;font-size:medium;margin-top:12px;" class="drop-zoon__paragraph">Click Here to Upload Image</p>
                                    <span id="loadingText" class="drop-zoon__loading-text"></span>
                                    <%--<img src="~/Uploads/anonymoususer.jpg" alt="Preview Image" id="previewImage" class="drop-zoon__preview-image" draggable="false">--%>
                                    <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" class="drop-zoon__preview-image" Height="180px" ImageUrl=""/>
                                    <asp:FileUpload
                                        runat="server"
                                        ID="FileUpload1"
                                        type="file"
                                        name="file"
                                        Text="Upload Photo"
                                        class="drop-zoon__file-input"
                                        accept="image/*"/>
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

                        <div class="form-group">
                            <table border="1">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_txt_currentPlan" Text="Current Plan - " Font-Bold="true" Font-Size="X-Large" runat="server" />
                                        <asp:Label ID="Label_Plan_Name" ForeColor="#607DF2" Text="" Font-Bold="true" Font-Size="X-Large" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Allowed_Subscriber" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Allowed_Business" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Allowed_User" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                            </table><br />

                            <table border="1">
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Assigned_Subscriber" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Assigned_Business" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Label ID="Label_Assigned_User" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                                    </td>
                                </tr>
                            </table>
                            <%--<asp:Label ID="Label_txt_currentPlan" Text="Current Plan - " Font-Bold="true" Font-Size="X-Large" runat="server" />
                            <asp:Label ID="Label_Plan_Name" ForeColor="#607DF2" Text="" Font-Bold="true" Font-Size="X-Large" runat="server" /><br />
                            <asp:Label ID="Label_Allowed_Subscriber" Font-Bold="true" Font-Size="Larger" Text="" runat="server" /><br />
                            <asp:Label ID="Label_Allowed_Business" Font-Bold="true" Font-Size="Larger" Text="" runat="server" /><br />
                            <asp:Label ID="Label_Allowed_User" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />
                            <br /><br />
                            <asp:Label ID="Label_Assigned_Subscriber" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" /><br />
                            <asp:Label ID="Label_Assigned_Business" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" /><br />
                            <asp:Label ID="Label_Assigned_User" ForeColor="#607DF2" Font-Bold="true" Font-Size="Larger" Text="" runat="server" />--%>
                        </div>
                    </div>

                    <div class="col-md-8">

                        
                        <div class="row">
                        <div class="col-md-1">
                            <br /><br /><br />
                            
                        </div>

                        <div class="col-md-8">
                            <div class="row text-center">
                                <asp:Label ID="Label_Cancelled_Message" Font-Bold="true" Font-Size="XX-Large" ForeColor="Red" runat="server" />
                                <h3 style="font-weight:bold">New Subscriber Information</h3>
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
                            <div class="col-md-6" style="display:none;">
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
                                        <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">The email id you've entered is not vaild. Please try again
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
                                            <div class="col-md-1" style="display:none;">
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
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="Name">Phone</label>&nbsp;<span style="font-weight: bold"></span>
                                    <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)"
                                        runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="Name" >Ext</label>&nbsp;<span style="font-weight: bold"></span>
                                    <asp:TextBox ID="txtExtPh" MaxLength="4" onkeypress="return isNumberKey(event)"
                                        runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-4">
                                <div class="form-group">
                                    <label for="Name" >Mobile</label>
                                    <asp:TextBox ID="txt_mobilenumber" runat="server" ClientIDMode="Static" CssClass="form-control"
                                        MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-md-2" style="display:none;">
                                <div class="form-group">
                                    <label for="Name" >Ext</label>
                                    <asp:TextBox ID="txtExtMob" runat="server" ClientIDMode="Static" CssClass="form-control"
                                        MaxLength="4" onkeypress="return isNumberKey(event)"></asp:TextBox>
                                </div>
                            </div>
                            </div>

                            <div class="row">
                                <asp:UpdatePanel ID="up22" runat="server">
                                    <ContentTemplate>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Total Assigned Businesses</label>
                                        <asp:TextBox ID="TextBox_Total_Assigned_Businesses" ClientIDMode="Static" MaxLength="9" OnTextChanged="TextBox_Total_Assigned_Businesses_TextChanged" 
                                            AutoPostBack="true" runat="server" onkeypress="return NumbersOnly(event)" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                        <div class="form-group">
                                        <label for="Name">Total Assigned Users</label>
                                        <asp:TextBox ID="TextBox_Total_Assigned_Users" ClientIDMode="Static" MaxLength="9" OnTextChanged="TextBox_Total_Assigned_Users_TextChanged" 
                                            AutoPostBack="true" runat="server" onkeypress="return NumbersOnly(event)" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="TextBox_Total_Assigned_Businesses" />
                                        <asp:AsyncPostBackTrigger ControlID="TextBox_Total_Assigned_Users" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>

                            <div class="row">
                                <div class="col-md-4"></div>
                                <%--<asp:UpdatePanel ID="up33" runat="server">
                                    <ContentTemplate>--%>
                                <div class="col-md-4 text-center">
                                    <div class="form-group">
                                        <%--<asp:UpdatePanel ID="updPnl" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>--%>
                                        <asp:LinkButton ID="Button_Register_As_A_BO" runat="server" Width="50%" OnClick="Button_Register_As_A_BO_Click"
                                            ClientIDMode="Static" Text="Save" CssClass="btn2 btn-primary" />
                                        <asp:LinkButton ID="UpdateButton_Register_As_A_BO" runat="server" Width="50%" OnClick="UpdateButton_Register_As_A_BO_Click"
                                            ClientIDMode="Static" Visible="false" Text="Save" CssClass="btn2 btn-primary" />
                                        <%--</ContentTemplate>
                                            <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="Button_Register_As_A_BO" />
                                        <asp:AsyncPostBackTrigger ControlID="UpdateButton_Register_As_A_BO" />
                                        </Triggers>
                                        </asp:UpdatePanel>--%>
                                    </div>
                                </div>
                                    <%--</ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="Button_Register_As_A_BO" />
                                        <asp:AsyncPostBackTrigger ControlID="UpdateButton_Register_As_A_BO" />
                                    </Triggers>
                                </asp:UpdatePanel>--%>
                                <div class="col-md-4"></div>
                            </div>

                            </div>
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
                            <div class="col-md-4">
                               
                            </div>
                            <div class="col-md-6" style="display: none;">
                                <div class="form-group">
                                    <label for="Name">Middle Name</label>
                                    <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-md-4">
                              
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
                                    <div class="col-md-4">
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
                                    <div class="col-md-4">
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

                                    <div class="col-md-4">
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
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:LinkButton ID="lnkratechange" Visible="false" CssClass="btn btn-primary" runat="server"
                                                Text="Change Rate"></asp:LinkButton>
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

                        </div>


                        <h4 style="border-bottom: 2px solid #349fce">Insert New Address</h4>
                        <div class="row">
                            <div class="col-md-4">
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

                            <div class="col-md-4">
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
                            <div class="col-md-4">
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

                            <div class="col-md-4">
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
                            <div class="col-md-4">
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <asp:Button ID="btnSaveAddres" Width="100%" Visible="false" Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                            ValidationGroup="AddressBusinessDetails" CssClass="btn btn-primary" Text="Save"
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

