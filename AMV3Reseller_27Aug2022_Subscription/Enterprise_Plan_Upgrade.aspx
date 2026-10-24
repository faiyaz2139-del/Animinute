<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Enterprise_Plan_Upgrade.aspx.cs" Inherits="Enterprise_Plan_Upgrade" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script lang="Javascript" type="text/javascript">
        function IsDigitsEnteredKey(evt) {
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

    <%--SweetAlert Javascript--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <style>
        .buttonhyperlink {
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

        .buttonhyperlink2 {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 10px 25px;
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
            background-color: #607DF2;
            border-radius: 1px;
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

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6" >
                    <h2 style="text-align: left; margin-top: 12px;">Upgrade To Enterprise Plan</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6 text-right">
                    <a href="ProfilesPage.aspx" id="btnback" runat="server" class="buttonhyperlink" style="margin-top: 10px;
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
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Upgrade To Enterprise Plan" runat="server" />
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">

            <%--<div class="row">
                <div class="col-md-4">
                </div>
                <div class="col-md-4">--%>
                    
                    <%--<h4 style="font-weight:bold;color:#808080;">Customize Your Plan</h4>--%>
                    <div class="row">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>No of Business</label>
                                <asp:TextBox ID="txtnoofbusiness" ClientIDMode="Static" MaxLength="7" onkeypress="return IsDigitsEnteredKey(event)" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label>No of Users</label>
                                <asp:TextBox ID="txtnoofuser" ClientIDMode="Static" MaxLength="7" onkeypress="return IsDigitsEnteredKey(event)" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <%--<div class="col-md-12">
                            <div class="form-group">
                                <label>Plan Start Date</label>
                                <asp:TextBox ID="txtplanstartdate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group">
                                <label>Plan End Date</label>
                                <asp:TextBox ID="txtplanenddate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>--%>
                        <div class="col-md-3">
                            <div class="form-group">
                                <center>
                                    <label>&nbsp;</label>
                                    <asp:Button runat="server" ID="btnSaveEnterprisePlan" OnClick="btnSaveEnterprisePlan_Click" Text="Submit" CssClass="buttonhyperlink2"/>
                                </center>
                            </div>
                        </div>
                    </div>

                <%--</div>
                <div class="col-md-4">
                </div>
            </div>--%>                

        </div>
    </div>

</asp:Content>