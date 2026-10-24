<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Edit_EmployeeV2.aspx.cs" Inherits="Add_EmployeeV2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
    <script lang="Javascript" type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)
                return false;

            return true;
        }

    </script>
    <script>
        $(document).ready(function () {
            $("#<%=ddlCatagoryRole.ClientID%>").select2({
                placeholder: "Select Role",
                allowClear: true
            });
        });
    </script>
    <script src="Assets/assets/js/core/bootstrap.min.js"></script>
    <script src="Assets/assets/js/core/jquery.3.2.1.min.js"></script>
    <script>
        $(document).ready(function () {

            $('#txtMainPhone').keyup(function (event) {
                addHyphen(this);
            });
        });

        $(document).ready(function () {
            $('#txtMobile').keyup(function (event) {
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
    <style>
        .modalBackground {
            background-color: Black;
            /*filter: alpha(opacity=60);*/
            opacity: 0.6;
        }

        .modalPopup {
            background-color: whitesmoke;
            width: auto;
            border: 2px solid #607DF2;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup .body {
            min-height: 50px;
            line-height: 30px;
            /*font-weight: bold;*/
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            overflow-y: scroll;
            height: auto;
            width: 650px;
        }


        .buttonlink {
            text-align: center;
            margin-top: 10px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonlink:hover {
            background-color: #607DF2;
            border-radius: 30px;
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

        .buttonsquarelink {
            text-align: center;
            margin-top: 10px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink:hover {
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

        .buttonsquarelink2 {
            text-align: center;
            padding-top: 200px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink2:hover {
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
            padding: 10px 5px;
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
            padding: 10px 5px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .btnblank {
            border: 1px solid #607DF2;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            height: 40px;
            font-family: sans-serif;
            font-size: large;
        }

        .btnblank:hover {
            border: 1px solid #607DF2;
            background-color: #607DF2;
            border-radius: 1px;
            height: 40px;
            color: white;
            font-family: sans-serif;
            font-size: large;
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
            /*padding: 2px 20px;
            text-decoration: none;
            border: solid 1px gray;
            color:gray;
            background-color: white;*/
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }
    </style>

    <style type="text/css">
        .buttonClassEye {
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
        }

        .buttonClassEye:hover {
            /*padding: 2px 20px;
            text-decoration: none;
            border: solid 1px gray;
            color:gray;
            background-color: white;*/
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
        }

        /*Added by Firoz*/
        #header-wrap {
            height: auto;
            width: 100%;
            /*margin-top: 30px;*/
            margin-bottom: -1px !important;
        }
        /*Added by Firoz*/

        h5 {
            width: 100%;
            text-align: left;
            border-bottom: 1px solid #000;
            line-height: 0.1em;
            margin: 10px 0 20px;
        }

        h5 span {
            background: #fff;
            padding: 0 10px;
        }

        .checkbox label, .radio label {
            margin-left: 0px !important;
        }
    </style>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <%--<script>
        $(document).ready(function () {
            $("#homemenu").click(function () {
                document.cookie = "tabid=#home";
                $("#home").attr("class", "tab-pane fade in active show");
                $("#permission").attr("class", "tab-pane fade in");
                $("#ChangeBuss").attr("class", "tab-pane fade in");
            });

            $("#permissionmenu").click(function () {
                document.cookie = "tabid=#permission";
                $("#home").attr("class", "tab-pane fade in");
                $("#permissionmenu").attr("class", "tab-pane fade in active show");
                $("#ChangeBuss").attr("class", "tab-pane fade in");
            });


            $("#menu1menu").click(function () {
                document.cookie = "tabid=#ChangeBuss";
                $("#home").attr("class", "tab-pane fade in");
                $("#permissionmenu").attr("class", "tab-pane fade in");
                $("#ChangeBuss").attr("class", "tab-pane fade in active show");
            });

            function readCookie(name) {
                var nameEQ = name + "=";
                var ca = document.cookie.split(';');
                for (var i = 0; i < ca.length; i++) {
                    var c = ca[i];
                    while (c.charAt(0) == ' ') c = c.substring(1, c.length);
                    if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
                }
                return null;
            }

            var x = readCookie("tabid");
            //<li id="homemid" class="active"><a data-toggle="tab" id="homemenu" href="#home" style="font-size:15px;text-transform:uppercase;">Personal Details</a></li>
            //    <li id="addmid"><a data-toggle="tab" href="#menu1" id="menu1menu" style="font-size:15px;text-transform:uppercase;">Address and Contacts</a></li>
            //    <li id="rolemid"><a data-toggle="tab" href="#menu4" id="menu4menu" style="font-size:15px;text-transform:uppercase;">User Role and Credential</a></li>
            //    <li id="paymeid">

            //alert(arr[1]);
            if (x == '#home') {
                $("#home").attr("class", "tab-pane fade in active show");
            }
            else if (x == '#permission') {
                $("#permission").attr("class", "tab-pane fade in active show");
                $("#home").attr("class", "tab-pane fade in");
                $("#ChangeBuss").attr("class", "tab-pane fade in");
                $("#LichangeBuss").attr("class", "");
                $("#Lirolemid").attr("class", "active");
                $("#Lihomemid").attr("class", "");
            }
            else if (x == '#ChangeBuss') {
                $("#ChangeBuss").attr("class", "tab-pane fade in active show");
                $("#home").attr("class", "tab-pane fade in");
                $("#permissionmenu").attr("class", "tab-pane fade in");
                $("#LichangeBuss").attr("class", "active");
                $("#Lirolemid").attr("class", "");
                $("#Lihomemid").attr("class", "");
            }

            $("#txtStreetDirection").change(function () {
                var a = $("#txtStreetDirection").val();
                $("#txtStreetDirection1").val(a);
            });
        });
    </script>--%>
    <link href="css/rounduploadimagecontrol.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">



    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmailConfirm" runat="server"
        PopupControlID="pnlAssginBuss" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>

    
             <asp:Panel ID="pnlAssginBuss" runat="server" ClientIDMode="Static" CssClass="modalPopup" Style="display: none;">
                 
        <div class="header">
            <asp:Label ID="lblBuss" runat="server"></asp:Label>
            <asp:HiddenField ID="hfdbussidpop" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body">
            <div class="row">
                <asp:UpdatePanel ID="updpop" runat="server" UpdateMode="Conditional">
      <Triggers>
          <asp:AsyncPostBackTrigger ControlID="ddlRoleNamepop" />
      
      </Triggers>
        <ContentTemplate>
                <div class="col-md-6" style="overflow: auto;">
                    <div class="form-group">
                        <label>Role Name</label>
                        <asp:DropDownList ID="ddlRoleNamepop" runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlRoleNamepop_SelectedIndexChanged">
                            <asp:ListItem Value="" Selected="True"> Select Role</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
              </ContentTemplate>
    </asp:UpdatePanel>
                <div class="col-md-6">
                    <div class="form-group">
                        <label>User Type</label>
                        <asp:DropDownList ID="ddlUserType" ClientIDMode="Static" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                        </asp:DropDownList>
                    </div>
                    <script>

                        $('#ddlUserType').change(function () {
                            var val = $(this).val();
                            var e = document.getElementById('Project');
                            if (val == 'P') {
                                e.style.display = 'block';
                            }
                            else {
                                e.style.display = 'none';
                            }

                        })
                    </script>
                </div>
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="form-group" id="Project" style="display: none; overflow: auto;">
                        <label>Project Name</label>
                        <asp:CheckBoxList ID="ddlProject" runat="server" AppendDataBoundItems="true" CellSpacing="2">
                        </asp:CheckBoxList>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="form-group">
                        <asp:LinkButton ID="btnassignee" runat="server" Width="100%" Text="Assignee" CssClass="btn btn-primary" OnClick="btnassignee_Click" />

                    </div>

                </div>
            </div>
            <div class="row" style="height: 400px; overflow: auto;" >
                 <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
                     <Triggers>
                         <asp:AsyncPostBackTrigger ControlID="GridView_User_Role_Permission_Pop" />
                     </Triggers>
                     <ContentTemplate>
                          <asp:GridView ID="GridView_User_Role_Permission_Pop" runat="server" ClientIDMode="Static"
                    CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False"
                    GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found" Font-Size="Medium" OnRowDataBound="GridView_User_Role_Permission_Pop_RowDataBound">
                    <%--OnRowCommand="GridView_User_Role_Permission_RowCommand"
            OnPreRender="GridView_User_Role_Permission_PreRender"
             OnRowDataBound="GridView_User_Role_Permission_RowDataBound" --%>
                    <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                    <%--<AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />--%>
                    <%--<HeaderStyle CssClass="bg-clouds" />--%>

                    <Columns>
                        <asp:TemplateField HeaderText="User Permission Role Name" ControlStyle-Font-Size="Large" ControlStyle-ForeColor="#808080">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />
                                <asp:HiddenField ID="HiddenField_Role_Permission_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Id")%>' />
                                 <asp:HiddenField ID="hfdUserTypeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserTypeId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Permission Role Id">
                            <ItemTemplate>
                                <asp:DropDownList Class="form-control" ID="DropDownList_User_Role_Permission_Access_Status" runat="server" >
                                    <asp:ListItem Value="NO" Text="NO" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="YES" Text="YES"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        <div>No records found.</div>
                    </EmptyDataTemplate>
                </asp:GridView>
                     </ContentTemplate>
                     </asp:UpdatePanel>
               
            </div>

              
        </div>
           
            
    </asp:Panel>
        

   






    <div style="position: relative">
        <h4>Edit Employee
            <asp:Label ID="lblEmpNameForAddressCont"
                CssClass="text-muted"
                Style="font-size: 12px; margin-left: 16px"
                runat="server"></asp:Label>
        </h4>
    </div>

    <div class="row" id="header-wrap">
        <div class="col-md-12">
            <ul class="nav nav-tabs">
                <li id="Lihomemid" class="active">
                    <a data-toggle="tab" id="homemenu" href="#home" style="font-size: 15px; text-transform: uppercase;">Personal Details</a>
                </li>
                <li id="Lirolemid" style="display:none;">
                    <a data-toggle="tab"
                        href="#permission"
                        id="permissionmenu" style="font-size: 15px; text-transform: uppercase;">Permission Role
                    </a>
                </li>
            </ul>
        </div>
    </div>
    <div id="container">
        <div class="panel panel-default">
            <div class="panel-body">
                <asp:HiddenField ID="hfdEmpID" runat="server" />
                <asp:HiddenField ID="hfdUserId" runat="server" />
                <div class="tab-content">
                    <div id="home" class="tab-pane fade in active show">
                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">



                                
                                <div class="row">
                                    <h5><span>Profile</span></h5>
                                </div>
                                <div class="row">
                                    <div class="col-md-3">
                                        <div>
                                            <!-- Upload Area -->
                            <center>
                            <div id="uploadArea" class="upload-area">
                                <!-- Drop Zoon -->
                                <div id="dropZoon" class="upload-area__drop-zoon drop-zoon">
                                    <span class="drop-zoon__icon">
                                        <i class='fa fa-image'></i>
                                    </span>
                                    <p style="font-weight:bolder;font-size:x-large;" class="drop-zoon__paragraph">Employee Photo</p>
                                    <span id="loadingText" class="drop-zoon__loading-text"></span>
                                    <%--<img src="~/Uploads/anonymoususer.jpg" alt="Preview Image" id="previewImage" class="drop-zoon__preview-image" draggable="false">--%>
                                    <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" class="drop-zoon__preview-image" Height="180px" ImageUrl=""/>
                                    <asp:FileUpload
                                        runat="server"
                                        ID="fupd"
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
                                    var fileInput = document.querySelector('#<%=fupd.ClientID %>');
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
                                    <div class="col-md-9">
                                        <div class="row">

                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="Name">First Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Middle Name</label>
                                            <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Last Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

                                        </div>
                                    </div>
                                    <div class="col-md-3" style="display:none;">
                                        <div class="form-group">
                                            <label for="title">Employee Status</label>
                                            <asp:DropDownList ID="ddlEmpStatus" runat="server" ClientIDMode="Static" CssClass="form-control">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Selected="True" Value="YES">Active</asp:ListItem>
                                                <asp:ListItem Value="NO">InActive</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                        <div class="row">

                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <label for="txtdob">Date of Birth</label>
                                                    <asp:TextBox ID="txtdob" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                                        type="date" Max="9999-12-31" placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="Reqtxtdob" ValidationGroup="persnoldetails" runat="server"
                                                        ControlToValidate="txtdob" Display="None" ErrorMessage="This Field Is Requried" />
                                                    <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server" TargetControlID="Reqtxtdob"
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
                                                    <asp:HiddenField ID="hfddboEncrpty" runat="server" />
                                                </div>
                                            </div>
                                            <div class="col-md-4">
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
                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <label for="txtdob">Hire Date</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtHiredate" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                                        type="date"
                                                        placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
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

                                        </div>
                                        
                                    </div>
                                </div>

                                <asp:UpdatePanel ID="up21" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="row">
                                                    <div class="col-md-12">
                                                        <div class="form-group">
                                                            <label for="Name">SIN Number</label>
                                                            <asp:TextBox ID="txtSocialNumer" MaxLength="9" runat="server" placeholder="### ### ###"
                                                                CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            <asp:HiddenField ID="hfdSocialEncrpty" runat="server" />
                                                        </div>
                                                    </div>

                                                    <div class="col-md-2" style="display:none;">
                                                        <div class="form-group">
                                                            <div style="height: 25px;"></div>
                                                            <asp:LinkButton ID="lnkView" CausesValidation="false" runat="server" Font-Size="30px"
                                                                CssClass="buttonClassEye" OnClick="lnkView_Click" Visible="false">
                                                                <i id="eye" runat="server" style="font-size: 30px;" class="fa fa-eye" aria-hidden="true"></i>
                                                            </asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="txtdob">SIN Expiry Date (if any)</label>
                                                    <asp:TextBox ID="txtSINExpiry" runat="server" class="form-control" type="date" Max="9999-12-31"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="lnkView" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                
                                <div class="row" style="display:none;">
                                    <h5><span>Password Mangement</span></h5>
                                </div>
                                <div class="row" style="display:none;">

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Temporary Password</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtTempPassword" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Confirm Temporary Password</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtcnfirmTempPassword" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                                <div class="row">
                                    <h5><span>Notification</span></h5>
                                </div>
                                <asp:UpdatePanel ID="up11" runat="server">
                                    <ContentTemplate>
                                        <div class="row">

                                            <div class="col-md-3">

                                                <div class="form-group">
                                                    <label for="Name">Email</label>&nbsp;<span style="font-weight: bold">(To grant online access )</span>

                                                    <asp:TextBox ID="txtMainEmail" runat="server" ClientIDMode="Static" CssClass="form-control" OnTextChanged="txtMainEmail_TextChanged"
                                                        AutoPostBack="true"></asp:TextBox>
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
                                                <div style="margin-left:20px;display:none;">
                                                    <asp:CheckBox ID="chkCofimNotificatin" runat="server" CssClass="checkbox" Text="Recive Email Notification" />
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="row">
                                                    <div class="col-md-12" id="divconfirm" runat="server" visible="false">
                                                        <div class="form-group">
                                                            <label for="Name">Confirm Email</label>
                                                            <asp:TextBox ID="txtConLoginEmail" ClientIDMode="Static" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtConLoginEmail_TextChanged"></asp:TextBox>
                                                            <div id="div1msg" runat="server">
                                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">Please re-enter the Confirm email id.</span>
                                                            </div>
                                                            <div id="div2stmsg" runat="server" visible="false">
                                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">Email and Confirm Email is not matching, please verify.
                                                                </span>
                                                            </div>


                                                        </div>
                                                    </div>
                                                    <div class="col-md-1" style="display:none;">
                                                        <div id="divtick" runat="server" visible="false" style="margin-top: 20px;">
                                                            <span class="fa fa-check fa-3x" aria-hidden="true" style="color: green"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3">
                                                <div class="form-group">
                                                    <label for="Name">Contact Phone</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                        runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>

                                                </div>

                                            </div>
                                            <div class="col-md-3">
                                                <div class="form-group">
                                                    <label for="Name">Mobile</label>
                                                    <asp:TextBox ID="txtMobile" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                        runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>


                                                </div>
                                            </div>

                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="txtMainEmail" />
                                        <asp:AsyncPostBackTrigger ControlID="txtConLoginEmail" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <div class="row">
                                    <h5><span>Address</span></h5>
                                </div>
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>

                                        <div class="row">

                                            <div class="col-md-6">
                                                <label for="Name">Enter Postal Code</label>&nbsp;
                                        <span style="color: black; font-weight: bolder;">(Required)</span>
                                                <div class="row">
                                                    <div class="col-md-3">
                                                        <div class="form-group">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSearchPostalCode" Width="150px" MaxLength="7" Style="text-transform: uppercase"
                                                                            runat="server" CssClass="form-control"
                                                                            OnTextChanged="txtSearchPostalCode_TextChanged" AutoPostBack="false" ClientIDMode="Static"
                                                                            onkeyup="addHyphenn(this)"></asp:TextBox>
                                                                        <script>
                                                                            //$(document).ready(function () {
                                                                            //    $('#txtSearchPostalCode').keyup(function (event) {
                                                                            //        addHyphenn(this);
                                                                            //    });
                                                                            //});

                                                                            //function addHyphenn(element) {
                                                                            //    let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                            //    let n = val.length;

                                                                            //    if (n <= 6) {
                                                                            //        let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                            //        $(element).val(finalVal);		// Update the input box.
                                                                            //    }
                                                                            //}
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
                                                                            ID="lnkSearch" runat="server" OnClientClick="return validatefields();"
                                                                            Text="" OnClick="lnkSearch_Click"><i class="fa fa-search" aria-hidden="true"></i>
                                                                        </asp:LinkButton>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </div>

                                                    <div class="col-md-1" style="margin-left: -40px; margin-top: 0px;">
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-md-3"></div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-2"></div>
                                            <div class="col-md-8">
                                                <div id="resultdiv" style="border: 0px solid #349fce; border-radius: 0px; padding: 0px;"
                                                    runat="server" visible="false">
                                                    <div>
                                                        <span style="color: gray;">SELECT THE ADDRESS FROM THE LIST BELOW OR ADD NEW</span>
                                                        <span style="color: black; font-weight: bolder">(Required)</span>
                                                    </div>
                                                    <br />
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
                                                                        <asp:LinkButton ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SEARCHADDRESS")%>'
                                                                            OnClick="StartDate_Click" OnClientClick="GetSelectedRow(this)" />
                                                                        <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>



                                                            </Columns>
                                                            <EmptyDataTemplate>
                                                                <div>No records found.</div>
                                                            </EmptyDataTemplate>
                                                        </asp:GridView>
                                                    </div>
                                                    <div style="display: none">
                                                        <asp:RadioButtonList ID="rdobtnlist" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rdobtnlist_SelectedIndexChanged"
                                                            AutoPostBack="true" Visible="false" Width="500px">
                                                            <asp:ListItem Text=" Add New Address" Value="0"></asp:ListItem>
                                                        </asp:RadioButtonList>
                                                    </div>
                                                </div>
                                                <div class="row">
                                                    <div class="col-md-2">
                                                    </div>
                                                    <div class="col-md-2"></div>
                                                </div>
                                            </div>
                                            <div class="col-md-"></div>
                                        </div>
                                        <div class="clearfix"></div>

                                        <div id="divAddres" runat="server" style="display: none">
                                            <div class="row">
                                                <div class="col-md-2"></div>
                                                <div class="col-md-8">
                                                    <h4 style="text-decoration: none;">Insert New Address</h4>
                                                    <div class="row">
                                                        <div class="col-md-4">
                                                            <div class="form-group">
                                                                <label for="Name">Street Number</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                                <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-8">
                                                            <div class="form-group">
                                                                <label for="Name">Address - Line 1</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                                <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-12">
                                                            <div class="form-group">
                                                                <label for="Name">Address - Line 2</label>
                                                                <asp:TextBox ID="txtStreetType" runat="server" CssClass="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <label for="Name">Suite Number</label>
                                                                <asp:TextBox ID="txtSuiteNumber" CssClass="form-control" runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-md-6">
                                                            <div class="form-group">
                                                                <div style="display: none;">
                                                                    <label for="Name">Street Direction</label>
                                                                    <asp:TextBox ID="txtStreetDirection" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                                                </div>
                                                                <label>Suite Type</label><br />

                                                                <asp:DropDownList ID="DropDownList2"
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
                                                                <label for="Name">City</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                                <asp:TextBox ID="txtCity" required runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
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
                                                                <label for="Name">Province</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                                <asp:TextBox ID="txtProvince" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
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
                                                                <label for="Name">Postal Code</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                                <asp:TextBox ID="txtPostalCode" MaxLength="7" runat="server" CssClass="form-control"
                                                                    ClientIDMode="Static" onkeyup="addHyphennew(this)" Style="text-transform: uppercase"></asp:TextBox>
                                                                <script>
                                                                    //$(document).ready(function () {
                                                                    //    $('#txtPostalCode').keyup(function (event) {
                                                                    //        addHyphenn(this);
                                                                    //    });
                                                                    //});

                                                                    /* firoz 30*/
                                                                    function GetSelectedRow(lnk) {

                                                                        var row = lnk.parentNode.parentNode;

                                                                        if (row.rowIndex === 0 || row.rowIndex === 1) {
                                                                            $("#NextAdd").attr("style", "display: block");
                                                                        }
                                                                    }
                                                                    /*End*/

                                                                    //function addHyphenn(element) {
                                                                    //    let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                    //    let n = val.length;

                                                                    //    if (n <= 6) {
                                                                    //        let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                    //        $(element).val(finalVal);		// Update the input box.
                                                                    //    }
                                                                    //}
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
                                                <div class="col-md-2"></div>
                                            </div>


                                            <div class="col-md-12">
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
                                            <%--end--%>

                                            <div class="row">
                                                <div class="col-md-4">
                                                    <div class="form-group">
                                                        <div class="col-md-12">
                                                            <asp:LinkButton ID="btnSaveAddres" Width="100%" Visible="false" Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                                                ValidationGroup="AddressBusinessDetails"
                                                                OnClick="btnSaveAddres_Click" CssClass="btn btn-primary" Text="Save"
                                                                runat="server"></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />

                                    </Triggers>
                                </asp:UpdatePanel>
                                <div class="row">
                                    <h5><span>Payment Details</span></h5>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="title">Pay Type</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:DropDownList ID="ddlBillingType" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <label for="txtCity1">Pay Rate</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                        <asp:TextBox ID="txtBillRate" runat="server" CssClass="form-control allownumericwithdecimal" ClientIDMode="Static"></asp:TextBox>
                                        <script>

                                            $(".allownumericwithdecimal").on("keypress keyup blur", function (event) {
                                                //this.value = this.value.replace(/[^0-9\.]/g,'');
                                                $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                                                if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                                                    event.preventDefault();
                                                }
                                            });
                                        </script>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="title">InActive Date</label>
                                            <asp:TextBox ID="txtInActiveDate" runat="server" Max="9999-12-31" TextMode="Date"
                                                ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="title">EI Exemptedpted</label>
                                            <asp:DropDownList ID="ddlEmpEI" runat="server" CssClass="form-control">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="title">CPP Exempted</label>
                                            <asp:DropDownList ID="ddlEmpCPP" runat="server" CssClass="form-control">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="title">Retain Vacation?</label>
                                            <asp:DropDownList ID="ddlRTVA" runat="server" CssClass="form-control">
                                                <asp:ListItem Value=""> Select</asp:ListItem>
                                                <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                                <asp:ListItem Value="No">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-4"></div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:LinkButton ID="LinkButton_EditEmployee" runat="server" Text="Save" OnClick="LinkButton_EditEmployee_Click" Visible="false" Width="100%" CssClass="btn2 btn-primary"/>
                                        </div>
                                    </div>
                                </div>

                            </div>


                        </div>
                    </div>


                    <div id="permission" class="tab-pane fade in" style="display:none;">


                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <div class="row">
                                </div>
                                <div class="row">

                                    <div class="row" style="">

                                        <div class="col-md-4" style="margin-top: 0px; border: 0px solid #349fce; border-radius: 10px;">
                                            <h5><span>User Role</span></h5>
                                            <div class="form-group" style="overflow: auto; height: 200px;">
                                                <asp:RadioButtonList ID="rdoBtnUserTypeList" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rdoBtnUserTypeList_SelectedIndexChanged" AutoPostBack="true"
                                                    CellPadding="10" CssClass="radioButtonList" CellSpacing="50" RepeatColumns="1">
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>

                                        <div class="col-md-8">
                                            <div>
                                                <h5><span>Change Business</span></h5>
                                            </div>
                                            <div class="row">
                                                <div class="col-md-5">
                                                    <div>
                                                        <%--<h5><span>Available Business Names</span></h5>--%>
                                                        <br />
                                                    </div>
                                                    <div style="height: 200px; overflow-y: scroll">
                                                        <asp:GridView ID="gvAvBu" runat="server" ClientIDMode="Static"
                                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                                            GridLines="None" EmptyDataText="No Records Found" ShowHeader="false">
                                                            <Columns>

                                                                <asp:TemplateField ControlStyle-Font-Size="Large" ControlStyle-ForeColor="#808080">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkbussname" runat="server" CssClass="chkSelect"></asp:CheckBox>
                                                                        <asp:HiddenField ID="hfdBusinessID" runat="server" Value='<%# Eval("BusinessID") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="bussname" runat="server" Text='<%# Eval("BusinessName") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </div>
                                                </div>
                                                <div class="col-md-2">
                                                    <div style="margin-top: 20px">
                                                        <asp:LinkButton ID="btnSendBuss" runat="server" CssClass="btn btn-primary" Width="100%" Text=">>" OnClick="btnSendBuss_Click" />
                                                    </div>
                                                    <div style="margin-top: 20px">
                                                        <asp:LinkButton ID="btnBackBuss" runat="server" CssClass="btn btn-primary" Width="100%" Text="<<" OnClick="btnBackBuss_Click" />
                                                    </div>
                                                </div>
                                                <div class="col-md-5">
                                                    <div>
                                                        <%--<h5><span>Selected Business Names</span></h5>--%>
                                                        <h5><span>Assigned Business</span></h5>
                                                    </div>
                                                    <div style="height: 200px; overflow-y: scroll">
                                                        <asp:GridView ID="GvSelectBuss" runat="server" ClientIDMode="Static"
                                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                                            GridLines="None" EmptyDataText="No Records Found" ShowHeader="false">
                                                            <Columns>

                                                                <asp:TemplateField ControlStyle-Font-Size="Large" ControlStyle-ForeColor="#808080">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkbussname" runat="server" CssClass="chkSelect"></asp:CheckBox>
                                                                        <asp:HiddenField ID="hfdBusinessID" runat="server" Value='<%# Eval("BusinessID") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="bussname" runat="server" Text='<%# Eval("BusinessName") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnlRole" runat="server" Text='<%# Eval("Role") %>' CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="assgin" OnClick="lnlRole_Click"></asp:LinkButton>
                                                                        <asp:HiddenField ID="hfdRoleid" runat="server" Value='<%# Eval("Roleid") %>' />
                                                                        <asp:HiddenField ID="hfdProjectid" runat="server" Value='<%# Eval("Projectid") %>' />
                                                                        <asp:HiddenField ID="hfdProjectType" runat="server" Value='<%# Eval("ProjectType") %>' />
                                                                        <asp:HiddenField ID="hfdPermissionId" runat="server" Value='<%# Eval("Permission") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </div>
                                                    <script>
                                                        function divFunction(id) {
                                                            //Some code
                                                        }
                                                    </script>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">

                                        <div class="col-md-4">
                                            <div class="row">
                                                <div class="col-md-8">
                                                    <div class="form-inline">
                                                        <div class="form-group">
                                                            <label>Enter New Role *</label>
                                                            <asp:TextBox runat="server" class="form-control"
                                                                ID="txtRole"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>
                                                <div class="col-md-4">
                                                    <asp:LinkButton class="btn btn-default" ID="btnSaveNewRole" runat="server" Style="margin-top: 20px;"
                                                        Text="Add/Edit Role" OnClick="btnSaveNewRole_Click"></asp:LinkButton>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>
                            </div>
                            <div class="col-md-2"></div>
                        </div>

                        <div class="row" style="visibility:hidden;">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <h5><span>Categorization of Role</span></h5>
                                <div style="margin-bottom: 25px;">
                                    <asp:DropDownList ID="ddlCatagoryRole" runat="server" AppendDataBoundItems="true" CssClass="form-control" Width="100%" OnSelectedIndexChanged="ddlCatagoryRole_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                </div>
                                <asp:UpdatePanel ID="UpdatePanel_User_Role_Permission" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="GridView_User_Role_Permission" runat="server" ClientIDMode="Static"
                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False" OnRowDataBound="GridView_User_Role_Permission_RowDataBound"
                                            GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found">
                                            <%--OnRowCommand="GridView_User_Role_Permission_RowCommand"
            OnPreRender="GridView_User_Role_Permission_PreRender"
             OnRowDataBound="GridView_User_Role_Permission_RowDataBound" --%>
                                            <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                            <%--<AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />--%>
                                            <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                            <Columns>
                                                <asp:TemplateField HeaderText="User Permission Role Name" ControlStyle-Font-Size="Large" ControlStyle-ForeColor="#808080">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />
                                                        <asp:HiddenField ID="HiddenField_Role_Permission_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Id")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="User Permission Role Id">
                                                    <ItemTemplate>
                                                        <asp:DropDownList Class="form-control" ID="DropDownList_User_Role_Permission_Access_Status" runat="server" AutoPostBack="true">
                                                            <asp:ListItem Value="NO" Text="NO" Selected="True"></asp:ListItem>
                                                            <asp:ListItem Value="YES" Text="YES"></asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div>No records found.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                           
                            <div class="col-md-2"></div>

                        </div>
                        <div class="row">
                            
                                <center>
                                    <asp:LinkButton ID="btnFinalSave" runat="server" Text="Save" Width="150px" CssClass="btn btn-primary" OnClick="btnFinalSave_Click" />
                                </center>
                            
                        </div>
                    </div>


                </div>
            </div>

        </div>

    </div>
</asp:Content>

