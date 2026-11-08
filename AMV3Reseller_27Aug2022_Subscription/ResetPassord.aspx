<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ResetPassord.aspx.cs" Inherits="ResetPassord" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reset Password | AnyMinute</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.1/jquery.min.js"></script>
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>
    <script src="https://use.fontawesome.com/4fed6e96ae.js"></script>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <link rel="icon" href="Assets/assets/img/logo.jpg" type="image/x-icon" />

      <%--<script>
        $("#txtConfirmPass").change(function () {
            alert("The text has been changed.");
        });
    </script>--%>

    <style>
        body {
            background-color: whitesmoke;
        }
        :-moz-any-link:focus {
          outline: none !important;
        }
        input:active { outline:none !important;} 

        :focus {
            outline: none;
        }

        ::-moz-focus-inner {
            border: 0;
        }

        input {
            outline: none;
        }
        input[type=submit].btn-block
        {
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
            /*width: 400px;*/
            height: auto;
            /*border: 2px solid #607DF2;*/
            /*border:2px solid #0094ff;*/
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
            border-radius:1px;
            color: white;
            display: block;
            /*font-weight:bold;*/
            height: 45px;
            /*font-family: sans-serif;
            font-size: large;*/
            outline: none;
        }

        .btn2 {
            border: 1px solid #607DF2;
            background-color: white;
            border-radius: 30px;
            color: #607DF2;
            /*font-weight:bold;*/
            height: 40px;
            outline: 0px;
            /*font-family: sans-serif;
            font-size: large;*/
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
          display: table-cell;
          vertical-align: middle;
        }

        .inner {
          margin-left: auto;
          margin-right: auto;
          width: 500px;
          /*whatever width you want*/
        }
    </style>

<link href="css/AnyminuteTypography.css?v=20261108v73" rel="stylesheet" /></head>
<body class="am-typography">
    <form id="form1" runat="server">
       
        <div class="outer">
        <div class="middle">
        <div class="inner">

        <div class="panel panel-default" runat="server" id="OldPasswordPanel" visible="true">  <%--panel panel-default boxshadow--%>

                <div class="panel-body">
                    <div class="text-center">
                        <img src="Assets/assets/img/logo.jpg" />
                        <%--<h5 style="font-style:italic;color:dimgray;" >Record from Anywhere!</h5>--%>
                        <h3 style="font-style:italic;color:dimgray;text-align:center;">Hello!  <asp:Label ID="Label1" runat="server"></asp:Label></h3> 
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group" data-validate="Username is required">
                            
                                <asp:TextBox ID="txtOldPassword" placeholder="Please Enter The Temporary Password" ClientIDMode="Static" Required="true" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Button ID="Submit" runat="server" CssClass="btn btn-block" Text="Submit" OnClick="Submit_Click" />
                        </div>
                       
                    </div>

                </div>
            </div>
        <div class="panel panel-default" runat="server" id="LoginPanel" visible="false">  <%--panel panel-default boxshadow--%>

                <div class="panel-body">
                    <div class="text-center">
                        <img src="Assets/assets/img/logo.jpg" />
                        <%--<h5 style="font-style:italic;color:dimgray;" >Record from Anywhere!</h5>--%>
                        <h3 style="font-style:italic;color:dimgray;text-align:center;">Hello!  <asp:Label ID="lblname" runat="server"></asp:Label></h3> 
                    </div>

                    <div class="row" style="height: 40px;"></div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group" data-validate="Username is required">
                                <%--<asp:TextBox ID="txtNewPass" placeholder="New Password" ClientIDMode="Static" Required="true" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>--%>
                                <asp:TextBox ID="txtNewPass" placeholder="Please Enter New Password" ClientIDMode="Static" Required="true" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group" data-validate="Password is required">
                                <%--<asp:TextBox ID="txtConfirmPass" placeholder="Confirm Password" ClientIDMode="Static" Required="true" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>--%>
                                <asp:TextBox ID="txtConfirmPass" placeholder="Please Re-Enter New Password" ClientIDMode="Static" Required="true" TextMode="Password" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row" style="height: 20px;"></div>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:Button ID="btnReset" runat="server" CssClass="btn btn-block" Text="Change Password" OnClick="btnReset_Click" />
                        </div>
                        <div class="col-md-12" style="height: 10px;"></div>
                    </div>

                </div>
            </div>

</div>
        </div>
        </div>


    </form>
</body>
</html>
