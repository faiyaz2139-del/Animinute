<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AddSuppAdmin.aspx.cs" Inherits="AddSuppAdmin" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script src="js/label_control_font_small.js"></script>
    <style>
        .GridPager a, .GridPager span {
            display: block;
            height: 35px;
            width: 35px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            padding: 10px;
        }

        .GridPager a {
            background-color: #fff;
            color: #969696;
            border: 1px solid #dedede;
        }

            .GridPager a:hover {
                color: #337ab7;
            }

        .GridPager span {
            background-color: #337ab7;
            color: #fff;
            border: 1px solid #dedede;
        }

        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
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
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 10px 5px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink2 {
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

        .buttonhyperlink2:hover {
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

    </style>
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
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6">
                    <h2 style="float: left; margin-top: 10px;">Add Support Admin</h2>
                </div>
                <div class="col-md-6">
                    <a href="SupportAdminList.aspx" id="btnback" runat="server" class="buttonhyperlink2" style="margin-top: 10px;
                        float: right;">Home</a>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="col-md-3"></div>
                    <div class="col-md-6">
                     <div class="row">
                             <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Title</label>
                                
                                <asp:DropDownList ID="ddlLegalName" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                    <asp:ListItem Value="Mr.">Mr.</asp:ListItem>
                                    <asp:ListItem Value="Miss.">Miss.</asp:ListItem>
                                    <asp:ListItem Value="Ms.">Ms.</asp:ListItem>
                                    <asp:ListItem Value="Mx.">Mx.</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                         <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">First Name</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtFirstName" required runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Middle Name</label>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Last Name</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtLastName" required runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="txtdob">Date of Birth </label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtdob" required ValidationGroup="persnoldetails" runat="server" class="form-control"
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
                        <div class="col-md-6">
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
                         <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Contact Phone</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtMainPhone" required TextMode="Phone"  MaxLength="12" onkeypress="return isNumberKey(event)" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Mobile</label>
                                <asp:TextBox ID="txt_mobilenumber"  runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">City</label>
                                <asp:TextBox ID="txt_city" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Province</label>
                                <asp:TextBox ID="txt_province" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                         <asp:UpdatePanel ID="up11" runat="server" >
                                                    <ContentTemplate>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Login Email</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                
                                <asp:TextBox ID="txtMainEmail" TextMode="Email" required runat="server" ClientIDMode="Static" CssClass="form-control" OnTextChanged="txtMainEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                <div id="divValid" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild. Please try again
                                                </span>
                                            </div>
                                                        <asp:HiddenField ID="hfdemail" runat="server" />
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
                                            <div class="col-md-10">
                                                 <div class="form-group">
                                            <label for="Name">Confirm Email Address</label>
                                            <asp:TextBox ID="txtConLoginEmail" ClientIDMode="Static" runat="server" CssClass="form-control"  OnTextChanged="txtConLoginEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <div id="div1msg" runat="server">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                Please re-enter the Contact's email address</span>
                                            </div>
                                            <div id="div2stmsg" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild. Please try again
                                                </span>
                                            </div>
                                            

                                        </div>
                                            </div>
                                            <div class="col-md-1">
                                                <div id="divtick" runat="server" visible="false" style="margin-top:20px;">
                                                <span class="fa fa-check fa-3x" aria-hidden="true" style="color:green"></span>
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
                            <div class="form-group">
                            <div class="col-md-3 col-lg-3 col-sm-12 col-xs-12"></div>
                            <div class="col-md-3 col-lg-3 col-sm-12 col-xs-12">
                            <asp:Button ID="btnsave" runat="server" Width="100%" CssClass="buttonhyperlink" OnClick="btnsave_Click" Text="Save" />
                            </div>
                        </div>
                            </div>
                        </div>
                     <div class="col-md-3"></div>
                         </div>
                </div>
            </div>
    </div>
  
</asp:Content>

