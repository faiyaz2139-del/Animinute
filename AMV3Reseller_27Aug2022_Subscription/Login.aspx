<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login | AnyMinute</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />    

    <%--Added By firoz--%>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/jquery.min.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <%--end--%>

    <link rel="icon" href="Assets/assets/img/logo.jpg" type="image/x-icon" />

    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script src="js/jquery.min.js"></script>
    
    <style>
        body {
            background-color: whitesmoke;
        }

        :-moz-any-link:focus {
            outline: none !important;
        }

        input:active {
            outline: none !important;
        }

        :focus {
            outline: none;
        }

        ::-moz-focus-inner {
            border: 0;
        }

        input {
            outline: none;
        }

        input[type=submit].btn-block {
            outline: none;
        }

        btn::-moz-focus-inner, a::-moz-focus-inner {
            border: 0;
        }

        btn:focus {
            outline: none !important;
        }

        .btn:hover {
            background-color: #607DF2;
            color: white;
        }

        .btn2:hover {
            background-color: white;
            color: #607DF2;
        }

        .form-control {
            text-align: center;
        }

        .panel {
            text-align: center;
            height: auto;
            border-radius: 1px;
        }

        .boxshadow {
            -webkit-box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
            -moz-box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
            box-shadow: -1px 13px 28px -12px rgba(0,0,0,0.8);
        }

        .btn {
            border: 1px solid #607DF2;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            display: block;
            height: 45px;
            outline: none;
        }

        .btn2 {
            border: 1px solid #607DF2;
            background-color: white;
            border-radius: 30px;
            color: #607DF2;
            height: 40px;
            outline: 0px;
        }

        .btn:current {
            border-radius: 30px;
        }

        .outer {
            display: table;
            position: absolute;
            top: 0;
            left: 0;
            height: 100%;
            width: 100%;
        }

        .middle {
            vertical-align: middle;
            top: 50%;
            left: 50%;
        }


        .inner {
            margin-left: auto;
            margin-right: auto;
            width: auto;
        }
    </style>
    <script type="text/javascript" language="javascript">
     function DisableBackButton() {
       window.history.forward()
      }
     DisableBackButton();
     window.onload = DisableBackButton;
     window.onpageshow = function(evt) { if (evt.persisted) DisableBackButton() }
     window.onunload = function() { void (0) }
    </script>

    <script type="text/javascript">
        var submit_click_count = 0;
        function Prevent_Double_Click() {
            if (++submit_click_count > 1) {
                alert('Already procesing. Please be patient.');
                return false;
            }
        }
    </script>
</head>
<body>
    <div class="container">
            <div class="col-md-3 col-sm-12 col-xs-12"></div>
            <div class="col-md-5 col-sm-12 col-xs-12"  >
        <form id="form1" class="panel panel-default form-group" style="margin-top: 10%;" runat="server">

            <div style="vertical-align: middle;">

                <div runat="server" id="LoginPanel" visible="true">

                    <div class="panel-body" style="padding-top: 30px;">
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center" style="margin-top: -30px;">
                                <img src="Assets/assets/img/logo.jpg" />
                                <h5 style="font-style: italic; color: dimgray;">Record from Anywhere!</h5>
                            </div>
                        </div>


                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="form-group" data-validate="Username is required">
                                    <asp:TextBox ID="txtemail" runat="server" class="form-control" placeholder="Email"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="form-group" data-validate="Password is required">
                                    <asp:TextBox ID="txtpassword" ClientIDMode="Static" TextMode="Password" runat="server"
                                        class="form-control"
                                        placeholder="Password"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row" style="height: 20px;"></div>

                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:Button ID="btnLogin" OnClick="btnLogin_Click" runat="server"  Text="SIGN IN" CssClass="btn btn-block"/>
                                <asp:HiddenField ID="hdnemployeetype" runat="server" />
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12 text-center" style="margin-top: 20px;">
                                <asp:LinkButton ID="btnForgetPasssword" OnClick="btnForgetPasssword_Click" runat="server" Text="Forgot Password?"/>
                                <br />
                                <a style="color:black;">Don't have AnyMinute Account Yet?</a><asp:LinkButton ID="LinkButton_Sign_Up_New" OnClick="LinkButton_Sign_Up_New_Click" runat="server" Text=" Sign Up now."/><br />
                                <asp:LinkButton ID="LinkButton_Sign_Up_Subscriber" OnClick="LinkButton_Sign_Up_Subscriber_Click" Visible="false" runat="server" Text="New Subscriber"/>&nbsp;&nbsp;
                                <asp:LinkButton ID="LinkButton_Sign_Up_Reseller" OnClick="LinkButton_Sign_Up_Reseller_Click" Visible="false" runat="server" Text="Reseller Account"/>
                            </div>
                        </div>
                    </div>

                </div>

                <div runat="server" id="ForgetPasswordPanel" visible="false">
                    <div class="panel-body">

                        <div class="text-center">
                            <img src="Assets/assets/img/logo.jpg" />
                        </div>

                        <div class="row" style="height: 40px;"></div>

                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <div class="form-group">
                                    <asp:TextBox ID="txtLoginEmail_ForgetPasswordReset" runat="server" class="form-control"
                                        placeholder="Login Email"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row" style="height: 20px;"></div>

                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:Button ID="btnForgetPassword_SendMail" OnClick="btnForgetPassword_SendMail_Click"
                                    runat="server"
                                    Text="SUBMIT" CssClass="btn btn-block" OnClientClick="return Prevent_Double_Click();" />
                            </div>
                        </div>
                        <div class="col-md-12 col-sm-12 col-xs-12" style="height: 10px;"></div>
                        <div class="row">
                            <div class="col-md-12 col-sm-12 col-xs-12">
                                <asp:LinkButton ID="btnBack_ForgetPassword" OnClick="btnBack_ForgetPassword_Click"
                                    runat="server" Text="Back" />
                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </form>
        </div>
    </div>
</body>
</html>
