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
    <style>
        /* v41 social sign-in */
        .am-social-or{display:flex;align-items:center;gap:10px;color:#8a94a3;font-size:12px;margin:16px 0 12px}.am-social-or:before,.am-social-or:after{content:"";flex:1;height:1px;background:#e2e8f0}
        .am-social-btn{display:flex;align-items:center;justify-content:center;gap:10px;width:100%;margin-bottom:10px;padding:9px 12px;border:1px solid #d0d7e2;border-radius:6px;background:#fff;color:#1f2937!important;font-weight:600;text-decoration:none!important}
        .am-social-btn:hover{background:#f5f8fc;border-color:#b8c3d3}
    </style>
<link href="css/AnyminuteTypography.css?v=20261117v88" rel="stylesheet" /><link href="css/AnyminuteReference.css?v=20261117v88" rel="stylesheet" /><link href="css/AnyminuteFlat.css?v=20261117v88" rel="stylesheet" /></head>
<body class="am-reference am-typography">
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
                        <%-- v41: Google / Microsoft sign-in for every role (links to the existing User ID by verified login email). --%>
                        <asp:PlaceHolder ID="SocialLogin" runat="server">
                        <div class="row"><div class="col-md-12 col-sm-12 col-xs-12">
                            <div class="am-social-or"><span>or</span></div>
                            <asp:HyperLink ID="GoogleLogin" runat="server" NavigateUrl="~/ExternalLogin.aspx?p=google" CssClass="am-social-btn"><svg width="18" height="18" viewBox="0 0 48 48" aria-hidden="true"><path fill="#EA4335" d="M24 9.5c3.5 0 6.6 1.2 9.1 3.6l6.8-6.8C35.8 2.4 30.3 0 24 0 14.6 0 6.6 5.4 2.7 13.3l7.9 6.1C12.5 13.6 17.8 9.5 24 9.5z"/><path fill="#4285F4" d="M46.5 24.5c0-1.6-.1-3.1-.4-4.5H24v9h12.7c-.6 3-2.3 5.5-4.8 7.2l7.7 6c4.5-4.2 6.9-10.3 6.9-17.7z"/><path fill="#FBBC05" d="M10.6 28.6c-.5-1.4-.8-3-.8-4.6s.3-3.2.8-4.6l-7.9-6.1C1 16.6 0 20.2 0 24s1 7.4 2.7 10.7l7.9-6.1z"/><path fill="#34A853" d="M24 48c6.5 0 11.9-2.1 15.9-5.8l-7.7-6c-2.2 1.5-5 2.3-8.2 2.3-6.2 0-11.5-4.1-13.4-9.9l-7.9 6.1C6.6 42.6 14.6 48 24 48z"/></svg><span>Continue with Google</span></asp:HyperLink>
                            <asp:HyperLink ID="MicrosoftLogin" runat="server" NavigateUrl="~/ExternalLogin.aspx?p=microsoft" CssClass="am-social-btn"><svg width="18" height="18" viewBox="0 0 21 21" aria-hidden="true"><rect x="1" y="1" width="9" height="9" fill="#F25022"/><rect x="11" y="1" width="9" height="9" fill="#7FBA00"/><rect x="1" y="11" width="9" height="9" fill="#00A4EF"/><rect x="11" y="11" width="9" height="9" fill="#FFB900"/></svg><span>Continue with Microsoft</span></asp:HyperLink>
                        </div></div>
                        </asp:PlaceHolder>
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
