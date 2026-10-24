<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="BusinessOwnerProfile.aspx.cs" Inherits="BusinessOwnerProfile" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">

    <link href="css/bootstrap.min.css" rel="stylesheet" />
     <style>
        .buttonlink {
            text-align: center;
            /*margin-top:10px;*/
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonlink:hover {
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

        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            
            border: 1px solid #607DF2;
            display: block;
            background-color:white ;
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
            background-color:white ;
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

        .panelnew {
            /*margin-top:30px;*/
            /*border-radius: 20px;*/
            /*border: 1px solid #0275d8;*/
            width:1000px;
        }
    </style>
    
    
    <script src="Assets/assets/js/SweetAlert.js"></script>
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="float: left; margin-top: 10px;">Business Owner Profile</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <a href="MyProfile.aspx" id="btnback" runat="server" class="buttonhyperlink" style="margin-top: 10px;
                        float: right;">Home</a>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
                <br /><br />
                <a href="MyProfile.aspx" id="A2" runat="server">
                    Back
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Business Owner Profile" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

     <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                 <div class="col-md-3">

                    </div>
                <div class="col-md-6">
                    <div><h4 style="text-decoration: none;">Personal Details</h4></div>
                      <div class="row">
                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Title</label>
                                
                               <asp:DropDownList ID="txtLegalName" runat="server" CssClass="form-control">
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
                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Middle Name</label>
                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Last Name</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6" style="display:none;">
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
                        <div class="col-md-6" style="display:none;">
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
                        
                        <div class="col-md-6">
                            <div class="form-group">
                                <%--<label for="Name">Contact Phone</label>&nbsp;<span style="font-weight:bold">(Required)</span>--%>
                                <label for="Name">Phone</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Mobile</label>
                                <asp:TextBox ID="txt_mobilenumber" runat="server" ClientIDMode="Static" CssClass="form-control" MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">City</label>
                                <asp:TextBox ID="txt_city" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Province</label>
                                <asp:TextBox ID="txt_province" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6">
                            <div class="form-group">
                                <label for="Name">Login Email</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtMainEmail" runat="server" ClientIDMode="Static" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Confirm Login Email</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtConLoginEmail" ClientIDMode="Static" runat="server" CssClass="form-control" Enabled="false"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Contact Email Same as Login Email?</label>
                                <asp:DropDownList ID="ddlCopyLoginEmail" runat="server" onchange="Copymail(this)" CssClass="form-control">
                                <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                <asp:ListItem Value="No">No</asp:ListItem>
                                </asp:DropDownList>                    
                            </div>
                        </div>
                        <div class="col-md-6" style="display:none;">
                            <div class="form-group">
                                <label for="Name">Contact Email</label>&nbsp;<asp:CheckBox Visible="false" Text="Same as Login Email"
                                    OnClick="copyvalue1(this)" runat="server" />
                                <asp:TextBox ID="txtCCEmail" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="hfdCCEmail" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                         <div class="form-group">
                            <div class="col-md-12">
                                <center>
                                    <asp:Button ID="btnUpdate" runat="server" CssClass="btn2 btn-primary" OnClick="btnUpdate_Click" Text="Save" />
                                </center>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-3">

                </div>

            </div>
            </div>
         </div>
</asp:Content>

