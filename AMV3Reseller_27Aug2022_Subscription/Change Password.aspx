<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Change Password.aspx.cs" Inherits="Change_Password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="js/label_control_font_small.js"></script>
    <script src="js/AddEmpVal.js" type="text/javascript" lang="Javascript"></script>
    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <%--passwordstrenth--%>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js"></script>
    <style>
        #frmCheckPassword {
            border-top: #F0F0F0 2px solid;
            background: #808080;
            padding: 10px;
        }

        .demoInputBox {
            padding: 7px;
            border: #F0F0F0 1px solid;
            border-radius: 4px;
        }

        #password-strength-status {
            padding: 5px 10px;
            color: #FFFFFF;
            border-radius: 4px;
            margin-top: 5px;
        }

        #password-Match {
            padding: 5px 10px;
            color: #FFFFFF;
            border-radius: 4px;
            margin-top: 5px;
        }

        .medium-password {
            background-color: #b7d60a;
            border: #BBB418 1px solid;
        }

        .weak-password {
            background-color: #ce1d14;
            border: #AA4502 1px solid;
        }

        .strong-password {
            background-color: #12CC1A;
            border: #0FA015 1px solid;
        }
    </style>
    <script>
        function checkPasswordStrength() {
            var number = /([0-9])/;
            var alphabets = /([a-zA-Z])/;
            var special_characters = /([~,!,@,#,$,%,^,&,*,-,_,+,=,?,>,<])/;
            if ($('#TxtNewPswrd').val().length < 6) {
                $('#password-strength-status').removeClass();
                $('#password-strength-status').addClass('weak-password');
                $('#password-strength-status').html("Weak (should be atleast 8 characters.)");
            } else {
                if ($('#TxtNewPswrd').val().match(number) && $('#TxtNewPswrd').val().match(alphabets) && $('#TxtNewPswrd').val().match(special_characters)) {
                    $('#password-strength-status').removeClass();
                    $('#password-strength-status').addClass('strong-password');
                    $('#password-strength-status').html("Strong");
                } else {
                    $('#password-strength-status').removeClass();
                    $('#password-strength-status').addClass('medium-password');
                    $('#password-strength-status').html("Medium (should include alphabets atleast one Upper Case one Lower Case, numbers and special characters.)");
                }
            }
        }
    </script>
    <%--passwordstrenth--%>

    <style>
        .btn2 {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 0px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .btn2:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 0px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <h2 style="float: left; margin-top: 10px;">Change Password</h2>
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
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Change Password" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">

            <%--======Current Password====--%>
            <div class="row" style="margin-top: 20px;">
                <div class="col-md-3">
                    <div class="form-group">
                        <label for="Name">Old Password</label>
                        <asp:TextBox ID="TxtCrntPswrd" required="true" runat="server" TextMode="Password"
                            CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <%--======New Password====--%>
                <div class="col-md-3">
                    <div class="form-group">
                        <label for="Name">New Password</label>
                        <asp:TextBox ID="TxtNewPswrd" ClientIDMode="Static" TextMode="Password" required="true"
                            onKeyUp="checkPasswordStrength();" runat="server" CssClass="form-control"></asp:TextBox>
                        <div id="password-strength-status"></div>
                    </div>

                </div>
                <%--======Confirm Password====--%>
                <div class="col-md-3">
                    <div class="form-group">
                        <label for="Name">Confirm Password</label>
                        <asp:TextBox ID="TxtCnfrmPswrd" required="true" runat="server"
                            onKeyUp="ConfirmPass();"
                            TextMode="Password" CssClass="form-control"></asp:TextBox>
                        <div id="password-Match1"></div>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="form-group"><%--<br />--%>
                        <label for="Name">&nbsp;</label>
                        <asp:Button ID="BtnUpdtPswrd" runat="server" CssClass="btn2 btn-primary" OnClick="BtnUpdtPswrd_Click" 
                            Text="Save" />
                        <button style="display: none;" class="btn btn-danger">Cancel</button>
                        <asp:Label ID="lbl_msg" Font-Bold="True" BackColor="#FFFF66" ForeColor="#FF3300"
                            runat="server" Text=""></asp:Label>
                    </div>
                </div>
            </div>
            <%--======BTN CHNG PASWRD====--%>

        </div>
    </div>

</asp:Content>