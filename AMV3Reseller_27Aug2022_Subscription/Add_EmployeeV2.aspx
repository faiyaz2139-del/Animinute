<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Add_EmployeeV2.aspx.cs" Inherits="Add_EmployeeV2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="js/label_control_font_small.js"></script>
    <script src="js/AddEmpVal.js" type="text/javascript" lang="Javascript"></script>


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

    <script type="text/javascript">
         function IsNumberWithOneDecimal(txt, evt) {
             var charCode = (evt.which) ? evt.which : event.keyCode
             if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charCode == 8)) {
                 return false;
             }// else {
             //    var len = txt.value.length;
             //    var index = txt.value.indexOf('.');
             //    if (index > 0 && charCode == 46) {
             //        return false;
             //    }
             //    if (index > 0) {
             //        if ((len + 0) - index > 0) {
             //            return false;
             //        }
             //    }
             //}
             return true;
         }
     </script>

    <script type = "text/javascript">
         function Confirm() {
             var confirm_value = document.createElement("INPUT");
             confirm_value.type = "hidden";
             confirm_value.name = "confirm_value";
             if (confirm("Do you want to Delete Record?")) {
                 confirm_value.value = "Yes";
             } else {
                 confirm_value.value = "No";
             }
             document.forms[0].appendChild(confirm_value);


         }
     </script>

    <%--<script src="Assets/assets/js/core/bootstrap.min.js"></script>
    <script src="Assets/assets/js/core/jquery.3.2.1.min.js"></script>--%>
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
        function addSpace(element) {
            let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
            let n = val.length;

            if (n <= 9) {
                let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
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

        .nav-tabs {
            border-bottom: 0px solid #fff !important;
        }

            .nav-tabs > li.active > a, .nav-tabs > li.active > a:focus, .nav-tabs > li.active > a:hover {
                color: #555;
                cursor: default;
                background-color: #fff;
                border-left: 1px solid #fff !important;
                border-right: 1px solid #fff !important;
                border-top: 0px solid #fff !important;
                border-bottom-color: #fff !important;
            }

            .nav-tabs > li > a:hover {
                border-bottom-color: #0275d8 !important;
            }

            .nav-tabs > li.active {
                border-bottom: 1px solid #0275d8 !important;
            }

        .radioButtonList input[type="radio"] {
            margin-left: 10px;
            margin-right: 1px;
        }

        .radioButtonList label {
            margin-left: 10px;
        }

        /*MOBILE_ISSUE_SURYA- Role permission popup issue*/
        #GridView_User_Role_Permission_Pop td {
            padding: 1px 5px 1px 5px !important;
        }

        #GridView_User_Role_Permission_Pop {
            margin-bottom: 299px !important;
        }

        @media (max-width:767px) {
            #pnlAssginBuss, #pnlAssginBuss .body {
                width: 100% !important;
                padding-bottom: 20px;
            }

            #pnlAssginBuss {
                top: 0px !important;
                max-height: 100vh !important;
            }
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
            margin-top: 30px;
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

        h6 {
            width: 100%;
            text-align: left;
            font-size: medium !important;
        }
    </style>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>


    <script>
        $(document).ready(function () {
            $("#homemenu").click(function () {
                document.cookie = "tabid=#home";
                $("#home").attr("class", "tab-pane fade in active show tab-sm-fs");
                //$("#permission").attr("class", "tab-pane fade in tab-sm-fs");
                $("#project").attr("class", "tab-pane fade in tab-sm-fs");
                $("#ratedetils").attr("class", "tab-pane fade in tab-sm-fs");
            });

            //$("#permissionmenu").click(function () {
            //    document.cookie = "tabid=#permission";
            //    $("#home").attr("class", "tab-pane fade in tab-sm-fs");
            //    $("#permissionmenu").attr("class", "tab-pane fade in active show tab-sm-fs");
            //    $("#ChangeBuss").attr("class", "tab-pane fade in tab-sm-fs");

            //});
            $("#projectmenu").click(function () {
                document.cookie = "tabid=#project";
                $("#home").attr("class", "tab-pane fade in tab-sm-fs");
                $("#project").attr("class", "tab-pane fade in active show tab-sm-fs");
                $("#ratedetils").attr("class", "tab-pane fade in tab-sm-fs");
                //$("#permissionmenu").attr("class", "tab-pane fade in tab-sm-fs");
                //$("#permission").attr("class", "tab-pane fade in tab-sm-fs");
                $("#ChangeBuss").attr("class", "tab-pane fade in tab-sm-fs");

            });


            $("#ratedetailsmenu").click(function () {
                document.cookie = "tabid=#ratedetils";
                //$("#permission").attr("class", "tab-pane fade in");
                $("#project").attr("class", "tab-pane fade in");
                $("#home").attr("class", "tab-pane fade in");
                $("#ratedetils").attr("class", "tab-pane fade in active show");
                $("#LiRateDetails").attr("class", "active");
                $("#LiProject").attr("class", "");
                //$("#Lirolemid").attr("class", "");
                $("#Lihomemid").attr("class", "");
            });



            //function readCookie(name) {
            //    var nameEQ = name + "=";
            //    var ca = document.cookie.split(';');
            //    for (var i = 0; i < ca.length; i++) {
            //        var c = ca[i];
            //        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
            //        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
            //    }
            //    return null;
            //}

            //var x = readCookie("tabid");

            ////<li id="homemid" class="active"><a data-toggle="tab" id="homemenu" href="#home" style="font-size:15px;text-transform:uppercase;">Personal Details</a></li>
            ////    <li id="addmid"><a data-toggle="tab" href="#menu1" id="menu1menu" style="font-size:15px;text-transform:uppercase;">Address and Contacts</a></li>
            ////    <li id="rolemid"><a data-toggle="tab" href="#menu4" id="menu4menu" style="font-size:15px;text-transform:uppercase;">User Role and Credential</a></li>
            ////    <li id="paymeid">

            ////alert(arr[1]);
            //if (x == '#home') {
            //    $("#home").attr("class", "tab-pane fade in active show");

            //}
            //else if (x == '#permission') {
            //    $("#permission").attr("class", "tab-pane fade in active show");
            //    $("#home").attr("class", "tab-pane fade in");
            //    $("#ChangeBuss").attr("class", "tab-pane fade in");
            //    $("#LichangeBuss").attr("class", "");
            //    $("#Lirolemid").attr("class", "active");
            //    $("#Lihomemid").attr("class", "");

            //}
            //else if (x == '#ChangeBuss') {
            //    $("#ChangeBuss").attr("class", "tab-pane fade in active show");
            //    $("#home").attr("class", "tab-pane fade in");
            //    $("#permissionmenu").attr("class", "tab-pane fade in");
            //    $("#LichangeBuss").attr("class", "active");
            //    $("#Lirolemid").attr("class", "");
            //    $("#Lihomemid").attr("class", "");

            //}


            $("#txtStreetDirection").change(function () {
                var a = $("#txtStreetDirection").val();
                $("#txtStreetDirection1").val(a);
            });

        });
    </script>
    <link href="css/rounduploadimagecontrol.css" rel="stylesheet" />
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

        .nav-tabs {
            border-bottom: 0px solid #fff !important;
        }

            .nav-tabs > li.active > a, .nav-tabs > li.active > a:focus, .nav-tabs > li.active > a:hover {
                color: #555;
                cursor: default;
                background-color: #fff;
                border-left: 1px solid #fff !important;
                border-right: 1px solid #fff !important;
                border-top: 0px solid #fff !important;
                border-bottom-color: #fff !important;
            }

            .nav-tabs > li > a:hover {
                border-bottom-color: #0275d8 !important;
            }

            .nav-tabs > li.active {
                border-bottom: 1px solid #0275d8 !important;
            }

        .radioButtonList input[type="radio"] {
            margin-left: 10px;
            margin-right: 1px;
        }

        .radioButtonList label {
            margin-left: 10px;
        }

        /*MOBILE_ISSUE_SURYA- Role permission popup issue*/
        #GridView_User_Role_Permission_Pop td {
            padding: 1px 5px 1px 5px !important;
        }

        #GridView_User_Role_Permission_Pop {
            margin-bottom: 299px !important;
        }

        @media (max-width:767px) {
            #pnlAssginBuss, #pnlAssginBuss .body {
                width: 100% !important;
                padding-bottom: 20px;
            }

            #pnlAssginBuss {
                top: 0px !important;
                max-height: 100vh !important;
            }
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
            margin-top: 30px;
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

        h6 {
            width: 100%;
            text-align: left;
            font-size: medium !important;
        }
    </style>

    <style>
        .buttonhyperlink222 {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 2px 15px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink222:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 2px 15px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>


    <link href="css/rounduploadimagecontrol.css" rel="stylesheet" />
    <link href="css/ButtonAsLinkbutton.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <%-- PopUp start --%>
    <asp:LinkButton ID="lnkDummy" runat="server" Style="display:none" CausesValidation="false"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalAddRolePop" runat="server"
        PopupControlID="pnlAddRolePop" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlAddRolePop" runat="server" ClientIDMode="Static" CssClass="modalPopup" Style="display: none; max-height: 550px;">
        <div class="header">
            <label style="color: white;">Add New Role</label>
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body" style="overflow: hidden;">
            <div class="row">
                <div class="col-md-6">
                    <asp:TextBox ID="txtRoleNamePop" runat="server" CssClass="form-control" placeholder="Add New Role"></asp:TextBox>
                    <asp:Label ID="Label22" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnsaveRolePop" runat="server" CssClass="buttonhyperlink222" Text="Save"  OnClick="btnsaveRolePop_Click"/>
                </div>
            </div>
            <asp:Label Visible="false" Font-Bold="true" ForeColor="Red" Text="For Custom Role, Please donot select all entering individual timesheet permissions for default role." runat="server" />
                
            <div class="row" style="overflow: auto;">
                <div class="col-md-12" style="height: 350px;">
                    <asp:GridView ID="GridView_User_Role_Permission_Pop" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" AutoGenerateColumns="False" ShowHeader="False" OnRowDataBound="GridView_User_Role_Permission_Pop_RowDataBound"
                        GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found" Font-Size="Small">
                        <%--OnRowDataBound="GridView_User_Role_Permission_Pop_RowDataBound"--%>

                        <%--OnRowCommand="GridView_User_Role_Permission_RowCommand"
            OnPreRender="GridView_User_Role_Permission_PreRender"
             OnRowDataBound="GridView_User_Role_Permission_RowDataBound" --%>
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <%--<AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />--%>
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>
                            <asp:TemplateField HeaderText="User Permission Role Name" ControlStyle-Font-Size="Small" ControlStyle-ForeColor="#808080">
                                <ItemTemplate>
                                    <%--<asp:LinkButton ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />--%>
                                    <asp:Label ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />
                                    <asp:HiddenField ID="HiddenField_Role_Permission_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Id")%>' />
                                    <asp:HiddenField ID="hfdUserTypeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserTypeId")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="User Permission Role Id">
                                <ItemTemplate>
                                    <asp:DropDownList Class="form-control" ID="DropDownList_User_Role_Permission_Access_Status" runat="server">
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
                </div>
            </div>
        </div>
    </asp:Panel>


    <cc1:ModalPopupExtender ID="ModallEditRolePop" runat="server"
        PopupControlID="pnlEditRolePop" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnEditHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlEditRolePop" runat="server" ClientIDMode="Static" CssClass="modalPopup" Style="display: none; max-height: 550px;">
        <div class="header">
            <label style="color: white;" runat="server" id="lblBname">Add New Role</label>
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnEditHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body" style="overflow: hidden;">
            <div class="row">
                <asp:UpdatePanel ID="updpop" runat="server" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlEditRolePer" />
                        <asp:PostBackTrigger ControlID="btnSaveEditRolePop" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="col-md-6">

                            <asp:DropDownList ID="ddlEditRolePer" runat="server" CssClass="form-control" AutoPostBack="true" ClientIDMode="Static" Width="100%" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlEditRolePer_SelectedIndexChanged">
                                <asp:ListItem Selected="True" Text="Select Role" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:Label ID="Label33" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>

                        </div>
                           <div class="col-md-6">
                    <asp:Button ID="btnSaveEditRolePop" runat="server" CssClass="buttonhyperlink222" Text="Save" OnClick="btnSaveEditRolePop_Click" />
                </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                   
            </div>
            <asp:Label Visible="false" ID="Label_Timesheet_Message" Font-Bold="true" ForeColor="Red" Text="For Custom Role, Please donot select all entering individual timesheet permissions for default role." runat="server" />
                
            <div class="row" style="overflow: auto;">
                <div class="col-md-12" style="height: 350px;">
                    <asp:UpdatePanel ID="upd1" runat="server" UpdateMode="Always">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Gv_EditRolePop" />
                        </Triggers>
                        <ContentTemplate>
                            <asp:GridView ID="Gv_EditRolePop" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" AutoGenerateColumns="False" ShowHeader="False"
                                GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found" Font-Size="Small" OnRowDataBound="Gv_EditRolePop_RowDataBound">
                                <%--OnRowDataBound="GridView_User_Role_Permission_Pop_RowDataBound"--%>

                                <%--OnRowCommand="GridView_User_Role_Permission_RowCommand"
            OnPreRender="GridView_User_Role_Permission_PreRender"
             OnRowDataBound="GridView_User_Role_Permission_RowDataBound" --%>
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <%--<AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />--%>
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>
                                    <asp:TemplateField HeaderText="User Permission Role Name" ControlStyle-Font-Size="Small" ControlStyle-ForeColor="#808080">
                                        <ItemTemplate>
                                            <%--<asp:LinkButton ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />--%>
                                            <asp:Label ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />
                                            <asp:HiddenField ID="HiddenField_Role_Permission_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Id")%>' />
                                            <asp:HiddenField ID="hfdUserTypeId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserTypeId")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Permission Role Id">
                                        <ItemTemplate>
                                            <asp:DropDownList Class="form-control" ID="DropDownList_User_Role_Permission_Access_Status_Edit" runat="server">
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
        </div>
    </asp:Panel>


    <cc1:ModalPopupExtender ID="ModelAddNewProject" runat="server"
        PopupControlID="pnlAddProjectPop" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnAddProjectHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlAddProjectPop" runat="server" ClientIDMode="Static" CssClass="modalPopup" Style="display: none; max-height: 450px;">
        <div class="header">
            <label style="color: white;" runat="server" id="lblAddProject">Add New Project</label>
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnAddProjectHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body" style="overflow: hidden;">
            <div class="row">
                <%--<div class="col-md-6">
                    <p class="am-project-context" aria-live="polite"></p>
                        <asp:TextBox ID="txtAddProject" runat="server" CssClass="form-control" Placeholder="Add New Project"></asp:TextBox>

                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnSaveProject" runat="server" CssClass="btn2 btn-primary" Text="Save" OnClick="btnSaveProject_Click"/>
                </div>--%>
            </div>
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <label>Project Name</label>
                        <asp:TextBox ID="txtAddProject" runat="server" CssClass="form-control" Placeholder="Add New Project"></asp:TextBox>
                        <asp:Label ID="lblerror" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                        <asp:Label ID="Labelsameproject" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                    </div>
                </div>
                <div class="col-md-12">
                    <div class="form-group">
                        <label>Start Date</label>
                        <asp:TextBox ID="txtProjectStDate" TextMode="Date" Max="9999-12-31" runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:Label ID="lblerror1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                    </div>
                </div>
                <div class="col-md-12">
                    <div class="form-group">
                        <label>End Date</label>
                        <asp:TextBox ID="txtProjectEndDate" TextMode="Date" Max="9999-12-31" runat="server" CssClass="form-control"></asp:TextBox>                        
                    </div>
                </div>
                <div class="col-md-12">
                    <center>
                    <asp:Button ID="btnSaveProject" runat="server" CssClass="btn2 btn-primary" Text="Save" OnClick="btnSaveProject_Click" />
                    </center>
                </div>
            </div>

        </div>
    </asp:Panel>
    <%-- PopUp End --%>

    <%--pop up save data confirmation start--%>
    <link href="css/confirmation-pop-up.css" rel="stylesheet" />
    <cc1:ModalPopupExtender ID="Modelconfirmmessagepopup" runat="server"
        PopupControlID="pnlconfirmmessagepopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground_popup"
        CancelControlID="btnconfirmmessagepopupHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlconfirmmessagepopup" runat="server" ClientIDMode="Static" CssClass="modalPopup_popup" Style="display: none; max-height: 450px;">
        <div class="header">
            <label style="color: white;" runat="server" >Confirmation Message</label>
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnconfirmmessagepopupHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body">            
            <div class="row">
                <center><p style="font-weight:bold;">User has been added successfully</p></center>
                <%--<div class="col-md-2"></div>
                <div class="col-md-4">
                    <div class="form-group">
                        <center>
                            <asp:Button ID="btn_Okay" runat="server" CssClass="buttonhyperlink_popup" Text="Okay"/>
                        </center>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <center>
                            <asp:Button ID="btn_Cancel" runat="server" CssClass="buttonhyperlink_popup" Text="Cancel"/>
                        </center>
                    </div>
                </div>
                <div class="col-md-2"></div>--%>
                <div class="col-md-12">                    
                    <center>
                        <table>
                            <tr>
                                <td><asp:Button ID="btn_Okay" runat="server" OnClick="btn_Okay_Click" CssClass="buttonhyperlink_popup" Text="Ok"/></td>
                                <%--<td>&nbsp;&nbsp;</td>--%>
                                <td><asp:Button ID="btn_Cancel" runat="server" Visible="false" OnClick="btn_Cancel_Click" CssClass="buttonhyperlink_popup" Text="Cancel"/></td>
                            </tr>
                        </table>
                    </center>                    
                </div>
            </div>
        </div>
    </asp:Panel>
    <%--pop up save data confirmation end--%>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Add User" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A2" runat="server">
                    <%--<button id="btn_close" class="btn-sm btn-danger">--%>
                    <%--<i class="fa fa-window-close btn-sm btn-danger" aria-hidden="true"></i>--%>
                    <%--</button>--%>
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>
    <div style="position: relative; display:none;">
        <p>
            <%--<a href="Home_Page.aspx"
                id="btnback"
                runat="server"
                style="font-size: 14px">
                <i class="fa fa-angle-left" aria-hidden="true"></i>
                Home
            </a>--%>
            <a href="Home_Page.aspx"
                id="btnback"
                runat="server"
                style="font-size: 14px">
                Home
            </a>
            &nbsp;&nbsp;<asp:Label ID="Label_Maximum_Limit" Font-Bold="true" Font-Size="Larger" ForeColor="Red" runat="server" />
        </p>
        <%--<hr style="margin: 0" />--%>
        <h4>Add User
            <asp:Label ID="lblEmpNameForAddressCont"
                CssClass="text-muted"
                Style="font-size: 12px; margin-left: 16px"
                runat="server"></asp:Label><label style="font-size:small;color:red;display:none;">(First Click on Personal Details for Adding Employee)</label>
        </h4>
    </div>

    <ul class="nav nav-tabs">
        <li id="Lihomemid" class="active" runat="server" clientidmode="Static">
            <a data-toggle="tab" id="homemenu" href="#home" clientidmode="Static" class="tab-sm-fs" style="text-transform: uppercase;">Personal Details</a>
        </li>
        <%--<li id="Lirolemid" runat="server" clientidmode="Static">
            <a data-toggle="tab"
                href="#permission"
                id="permissionmenu" runat="server" clientidmode="Static" class="tab-sm-fs" style="text-transform: uppercase;">Assign Business And Role
            </a>
        </li>--%>
        <li id="LiRateDetails" runat="server" clientidmode="Static">
            <a
                data-toggle="tab"
                href="#ratedetils"
                id="ratedetailsmenu" style="text-transform: uppercase;" runat="server" class="tab-sm-fs" clientidmode="Static">Pay Detail
            </a>
        </li>
        <li id="LiProject" runat="server" clientidmode="Static">
            <a data-toggle="tab"
                href="#project"
                id="projectmenu" runat="server" clientidmode="Static" class="tab-sm-fs" style="text-transform: uppercase;">Assign Project
            </a>
        </li>

    </ul>
    <div id="container">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="tab-content">
                    <div id="home" class="tab-pane fade in active show" runat="server" clientidmode="Static">
                        <div class="row">
                            <div class="col-md-2">
                                <%--<br /><br /><br /><br />--%>
                                <div>
                                    <center>
                                        <div id="uploadArea" class="upload-area">
                                            <!-- Drop Zoon -->
                                            <div id="dropZoon" class="upload-area__drop-zoon drop-zoon">
                                                <span class="drop-zoon__icon">
                                                    <%--<i class='fa fa-image'></i>--%>
                                                </span>
                                                <p style="font-weight: bolder; font-size: medium;margin-top:12px;" class="drop-zoon__paragraph">Click Here to Upload Image</p>
                                                <span id="loadingText" class="drop-zoon__loading-text"></span>
                                                <asp:Image ID="previewImage" runat="server" ClientIDMode="Static" class="drop-zoon__preview-image" Height="180px" ImageUrl="" />
                                                <asp:FileUpload
                                                    runat="server"
                                                    ID="fupd"
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
                            <div class="col-md-8">




                                <%--<div class="row">
                                    <h5><span>Profile</span></h5>
                                </div>--%>
                                <%--<div class="row">
                                    <div class="col-md-3">
                                        

                                    </div>
                                    <div class="col-md-9">
                                        

                                    </div>
                                </div>--%>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Assign Business</label>
                                            <asp:DropDownList ID="ddlBusiness" runat="server" CssClass="form-control js-example-basic-single" ClientIDMode="Static" Width="100%" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBusiness_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="Select Business" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <label for="Name">Assign Role</label>
                                            <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control js-example-basic-single" AutoPostBack="true" ClientIDMode="Static" Width="100%" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">
                                                <asp:ListItem Selected="True" Text="Select Role" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Add New Role" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <%--<div class="col-md-2">
                                        <div style="height: 25px"></div>
                                        <asp:Button ID="btnAdd" Text="Add" runat="server" CssClass="buttonhyperlink444" ClientIDMode="Static" OnClick="btnAdd_Click" />
                                    </div>--%>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <asp:GridView ID="Gv_RolePermission" runat="server" ClientIDMode="Static"
                                            CssClass="table nowrap" AutoGenerateColumns="False" ForeColor="#333333"
                                            
                                             Font-Size="Small"
                                            OnRowEditing="Gv_RolePermission_RowEditing" OnRowCancelingEdit="Gv_RolePermission_RowCancelingEdit" OnRowDataBound="Gv_RolePermission_RowDataBound"
                                            OnPreRender="Gv_RolePermission_PreRender" OnRowCommand="Gv_RolePermission_RowCommand" OnRowUpdating="Gv_RolePermission_RowUpdating"> 
                                            <%--EmptyDataText="No Records Found"--%>
                                                <%--GridLines="None"--%>

                                            <Columns>
                                                <asp:TemplateField HeaderText="Business Name" ControlStyle-Font-Size="Small" ControlStyle-ForeColor="#808080">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_BusinessName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>' />
                                                        <asp:HiddenField ID="hfd_BussinessId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BussinessId")%>' />

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="User Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_EmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeName")%>' />
                                                        <asp:HiddenField ID="hdf_EmpId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="User Role">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lbl_UserRole" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserRole")%>' />
                                                        <asp:HiddenField ID="hfd_UserRoleId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserRoleId")%>' />
                                                        <asp:HiddenField ID="hfd_RoleId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RoleId")%>' />
                                                    </ItemTemplate>

                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txt_UserRole" runat="server" Text='<%#Eval("UserRole") %>'></asp:TextBox>
                                                        <asp:HiddenField ID="hfd_UserRoleIdEdit" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserRoleId")%>' />
                                                        <asp:LinkButton ID="btn_Update" runat="server" Text="Save" CommandName="Update">
                                                                   
                                                        </asp:LinkButton> <b>|</b>
                                                        <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel">
                                                                   
                                                        </asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnk_ChangeRoleName" runat="server" Text="Change Role Name" CommandName="Edit"></asp:LinkButton>
                                                        <asp:Label ID="lbl_separate" ForeColor="Black" Font-Bold="true" runat="server" Text=" | " />
                                                        <asp:LinkButton ID="lnk_EditPermission" runat="server" Text="Edit Permission" CommandName="EditPermission" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>
                                                        <asp:Label ID="lbl_separate2" ForeColor="Black" Font-Bold="true" runat="server" Text=" | " />
                                                        <asp:LinkButton ID="lnk_delete"  OnClientClick="Confirm()" runat="server" Text="Delete" CommandName="DeletePermission" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <%--<EmptyDataTemplate>
                                                <div>No records found.</div>
                                            </EmptyDataTemplate>--%>
                                        </asp:GridView>
                                    </div>
                                </div>


                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">First Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-md-6" style="display:none;">
                                                <div class="form-group">
                                                    <label for="Name">Middle Name</label>
                                                    <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Last Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

                                                </div>
                                            </div>
                                            <div class="col-md-3" style="display: none;">
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

                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="txtdob">Date of Birth</label>
                                                    <asp:TextBox ID="txtdob" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                                        type="date" Max="9999-12-31" placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
                                             
                                            
                                                    <asp:HiddenField ID="hfddboEncrpty" runat="server" />
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
                                            
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="txtdob">Hire Date</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtHiredate" ValidationGroup="persnoldetails" Max="9999-12-31" runat="server" class="form-control"
                                                        type="date" OnTextChanged="txtHiredate_TextChanged" AutoPostBack="false" 
                                                        placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
                                                   
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group"><br /><br />
                                                <asp:Label ForeColor="Red" Style="float: left;" Font-Bold="true" Text="*  Note - Hire Date cannot be changed or edited." runat="server" />
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

                                </asp:UpdatePanel>

                                <div class="row" style="display: none;">
                                    <h5><span>Password Mangement</span></h5>
                                </div>
                                <div class="row" style="display: none;">

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

                                <%--<div class="row">
                                    <h5><span>Notification</span></h5>
                                </div>--%>
                              <%--  <asp:UpdatePanel ID="up11" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>--%>
                                        <div class="row">
                                            <div class="col-md-6">

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
                                                <div style="margin-left: 20px; display: none;">
                                                    <asp:CheckBox ID="chkCofimNotificatin" runat="server" CssClass="checkbox" Text="Recive Email Notification" />
                                                </div>
                                            </div>
                                            <div class="col-md-6">
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
                                                    <div class="col-md-1" style="display: none;">
                                                        <div id="divtick" runat="server" visible="false" style="margin-top: 20px;">
                                                            <span class="fa fa-check fa-3x" aria-hidden="true" style="color: green"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    <%--</ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="txtMainEmail" EventName="TextChanged" />
                                        <asp:AsyncPostBackTrigger ControlID="txtConLoginEmail" />
                                    </Triggers>
                                </asp:UpdatePanel>--%>



                                












                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Contact Phone</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                        </div>

                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Mobile</label>
                                            <asp:TextBox ID="txtMobile" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <%--<div class="row">
                                    <h5><span>Address</span></h5>
                                </div>--%>
                             <%--   <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>--%>

                                        <div class="row">

                                            <div class="col-md-6">
                                                <label for="Name">Enter Postal Code</label>&nbsp;
                                        <%--<span style="color: black; font-weight: bolder;">(Required)</span>--%>
                                                <div class="row">
                                                    <div class="col-md-3">
                                                        <div class="form-group">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:TextBox ID="txtSearchPostalCode" Width="150px" MaxLength="6" Style="text-transform: uppercase"
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
                                                                            ID="lnkSearch" ClientIDMode="Static" runat="server" OnClientClick="return validatefields();"
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
                                            <%--<div class="col-md-1"></div>--%>
                                            <div class="col-md-12">
                                                <div id="resultdiv" style="border: 0px solid #349fce; border-radius: 0px; padding: 0px;"
                                                    runat="server" visible="false">
                                                    <div>
                                                        <span style="color: gray;">SELECT THE ADDRESS FROM THE LIST BELOW OR ADD NEW</span>
                                                        <%--<span style="color: black; font-weight: bolder">(Required)</span>--%>
                                                    </div>
                                                    <%--<br />--%>
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
                                            <%--<div class="col-md-1"></div>--%>
                                        </div>
                                        <div class="clearfix"></div>

                                        <div id="divAddres" runat="server" style="display: none">
                                            <div class="row">
                                                <%--<div class="col-md-1"></div>--%>
                                                <div class="col-md-12">
                                                    <%--<h4 style="text-decoration: none;">Insert New Address</h4>--%>
                                                    <center>
                                                        <asp:Label Font-Bold="true" Font-Size="Large" Text="Insert New Address" runat="server" />
                                                    </center>
                                                    <div class="row">
                                                        <div class="col-md-4">
                                                            <div class="form-group">
                                                                <label for="Name">Street Number</label>&nbsp;<%--<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-md-8">
                                                            <div class="form-group">
                                                                <label for="Name">Address - Line 1</label>&nbsp;<%--<span style="font-weight: bold">(Required)</span>--%>
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
                                                                <label for="Name">City</label>&nbsp;<%--<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
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
                                                                <label for="Name">Province/State</label>&nbsp;<%--<span style="font-weight: bold">(Required)</span--%>>
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
                                                                <label for="Name">Postal Code</label>&nbsp;<%--<span style="font-weight: bold">(Required)</span>--%>
                                                                <asp:TextBox ID="txtPostalCode" MaxLength="6" runat="server" CssClass="form-control"
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
                                                <%--<div class="col-md-1"></div>--%>
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


                                        </div>

                                   <%-- </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                                         <asp:AsyncPostBackTrigger ControlID="txtMainEmail" />

                                    </Triggers>
                                </asp:UpdatePanel>--%>



                            </div>
                            <div class="col-md-2"></div>
                        </div>
                        <div class="row">
                            <center>
                                <asp:Button ID="btnAdd" Text="Add" runat="server" CssClass="buttonhyperlink444" ClientIDMode="Static" OnClick="btnAdd_Click" />
                            </center>
                        </div>
                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8" style="text-align: right;">
                               <center>
                                    <asp:Button ID="btnNext" runat="server" Text="Next >>" CssClass="buttonlooklikelinkbutton" onClientclick="return addPerVal()"></asp:Button>
                               </center> 
                            </div>
                            <div class="col-md-2"></div>
                        </div>
                    </div>

                    <%--<div id="permission" class="tab-pane fade in" runat="server" clientidmode="Static">
                        <div class="row">
                            <div class="col-md-1"></div>
                            <div class="col-md-10">
                                
                                

                            </div>
                            <div class="col-md-1"></div>
                        </div>
                        <div class="row">
                            <div class="col-md-12">
                                <center>
                                   
                                    <button class="btn2 btn-primary" onclick="return addRoleVal()">Next >></button>
                                </center>
                             
                            </div>
                        </div>
                    </div>--%>

                    <div id="project" class="tab-pane fade in" runat="server" clientidmode="Static">
                        <div class="row">
                            <div class="col-md-2"></div>
                            <div class="col-md-8">
                                <%--<div class="row">
                                    <h5><span>Assign Project</span></h5>
                                </div>--%>
                                <div class="row">
                                    <div class="col-md-4">
                                        <label for="Name">Select Business</label>
                                        <asp:DropDownList ID="ddlBusAssProject" runat="server" CssClass="form-control js-example-basic-single" ClientIDMode="Static" Width="100%" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBusAssProject_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-md-4">
                                        <label for="Name">Assign Project</label>
                                        <asp:DropDownList ID="ddlAssProjectName" runat="server" CssClass="form-control js-example-basic-single" ClientIDMode="Static" Width="100%" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlAssProjectName_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>

                                    <div class="col-md-4">
                                        <div style="height: 25px"></div>
                                        <asp:Button ID="btnAssProject" runat="server" CssClass="buttonhyperlink444" Text="Assign Project" ClientIDMode="Static" OnClick="btnAssProject_Click" />
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <%--<div class="col-md-12">--%>
                                            <asp:GridView ID="GvProject" runat="server" ClientIDMode="Static"
                                                CssClass="table nowrap" AutoGenerateColumns="False" ForeColor="#333333"
                                                
                                                 Font-Size="Small"
                                                OnRowEditing="GvProject_RowEditing" OnRowCancelingEdit="GvProject_RowCancelingEdit" OnRowDataBound="GvProject_RowDataBound"
                                                OnPreRender="GvProject_PreRender" OnRowCommand="GvProject_RowCommand" OnRowUpdating="GvProject_RowUpdating"> <%--EmptyDataText="No Records Found"--%>
                                                <%--GridLines="None"--%>


                                                <Columns>
                                                    <asp:TemplateField HeaderText="Business Name" ControlStyle-Font-Size="Small" ControlStyle-ForeColor="#808080">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_BusinessName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>' />
                                                            <asp:HiddenField ID="hfd_BussinessId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BussinessId")%>' />

                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="User Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_EmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeName")%>' />
                                                            <asp:HiddenField ID="hfd_EmployeeIDpro" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Project Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lbl_ProjectName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>' />
                                                            <asp:HiddenField ID="hfd_ProjectNameId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>' />
                                                             <asp:HiddenField ID="hfd_roleId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserRoleId")%>' />
                                                        </ItemTemplate>

                                                        <EditItemTemplate>
                                                            <asp:TextBox ID="txt_ProjectName" runat="server" Text='<%#Eval("ProjectName") %>'></asp:TextBox>
                                                            <asp:HiddenField ID="hfd_ProjectNameIdEdit" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>' />
                                                            <asp:HiddenField ID="hfd_roleId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserRoleId")%>' />
                                                            <asp:LinkButton ID="btn_Update" runat="server" Text="Save" CommandName="Update">
                                                                   
                                                            </asp:LinkButton> <b>|</b>
                                                            <asp:LinkButton ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel">
                                                                   
                                                            </asp:LinkButton>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnk_ChangeRoleName" runat="server" Text="Edit Project Name" CommandName="Edit"></asp:LinkButton> 
                                                            <asp:Label ID="lbl_separate3" ForeColor="Black" Font-Bold="true" runat="server" Text=" | " />
                                                            <asp:LinkButton ID="lnk_delete"  OnClientClick="Confirm()" runat="server" Text="Delete" CommandName="DeleteProject" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <%--<EmptyDataTemplate>
                                                    <div>No records found.</div>
                                                </EmptyDataTemplate>--%>
                                            </asp:GridView>
                                        <%--</div>--%>
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-2"></div>
                        </div>

                        <div class="row">
                        <%--firoz 02--%>
                            <div class="col-md-2"></div>
                            <div class="col-md-4" style="text-align: left;">
                                <%--<center>--%>                                   
                                    <button class="buttonlooklikelinkbutton" onclick="return addProjectValPrev()"><< Previous</button>
                                <%--</center>--%>                             
                            </div>
                            <div class="col-md-4" style="text-align: right;">
                                <%--<center>--%>                                   
                                    <button class="buttonlooklikelinkbutton" onclick="return addProjectVal()">Next >></button>
                                <%--</center>--%>                             
                            </div>
                            <div class="col-md-2"></div>
                        </div>

                        <%--<div class="row">
                            <div class="col-md-12">
                                <center>
                                   
                                    <button class="btn2 btn-primary" onclick="return addProjectVal()">Next >></button>
                                </center>
                             
                            </div>
                        </div>--%>
                        </div>

                    <div id="ratedetils" class="tab-pane fade in" runat="server" clientidmode="Static">
                        <div class="row">
                            <div class="col-md-1"></div>
                            <div class="col-md-10">
                                <%--<div class="row">
                                    <h5><span>Rate Details</span></h5>
                                </div>--%>
                                <div class="row">
                                    <div class="col-md-12" style="overflow: auto">
                                        <asp:GridView ID="GvRateDetails" runat="server" ClientIDMode="Static" Font-Size="Small"
                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                            OnRowDataBound="GvRateDetails_RowDataBound">
                                            <EmptyDataTemplate>
                                                <p>Select a business and role in Personal Details, then load the pay fields.</p>
                                                <asp:Button ID="btnPreparePay" runat="server" Text="Load Pay Details"
                                                    CssClass="btn2 btn-primary" CausesValidation="false" OnClick="btnPreparePay_Click" />
                                            </EmptyDataTemplate>
                                            <%--<AlternatingRowStyle BackColor="#f5f5f5" />--%> <%--EmptyDataText="No Records Found"--%> <%--GridLines="None"--%>
                                            <Columns>

                                                <asp:TemplateField ControlStyle-Font-Size="Small" Visible="false" ControlStyle-ForeColor="#808080" HeaderText="Select">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkbussname" runat="server" CssClass="chkSelect"></asp:CheckBox>
                                                       
                                                       
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Business Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="bussname" runat="server" Text='<%# Eval("BusinessName") %>'></asp:Label>
                                                         <asp:HiddenField ID="hfdRate" runat="server" Value='<%# Eval("RateId") %>' />
                                                         <asp:HiddenField ID="hfdRateBusinessID" runat="server" Value='<%# Eval("BussinessId") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="User Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="employeeee_name" runat="server" Text='<%# Eval("EmployeeName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Start/Hire Date">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtstdate" Text='<%# Eval("StartDate") %>' Width="160px" runat="server" Max="9999-12-31" TextMode="Date" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="End Date" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtEndDate" Text='<%# Eval("EndDate") %>' Width="160px" runat="server" Max="9999-12-31" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Rate">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRate" onkeypress="return IsNumberWithOneDecimal(this,event);" ondrop="return false;" onpaste="return false;" MaxLength="6"  Width="90px" Text='<%# Eval("Rate") %>' runat="server" CssClass="form-control allownumericwithdecimal xs-mselect"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Type">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlPaytpe" Width="120px" runat="server" CssClass="form-control xs-mselect" SelectedValue='<%# Eval("RateType") %>'>
                                                            <asp:ListItem Value="" Selected="True">Select</asp:ListItem>
                                                            <asp:ListItem Value="Hourly">Hourly</asp:ListItem>
                                                            <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                                            <asp:ListItem Value="Bi-Weekly">Bi-Weekly</asp:ListItem>
                                                            <asp:ListItem Value="Fort Nightly">Fort Nightly</asp:ListItem>
                                                            <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                                            <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="EI Exempted">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlEmpEI11" runat="server" CssClass="form-control xs-mselect" SelectedValue='<%# Eval("IsEmployeeEIExempty") %>'>
                                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="CPP Exempted">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlEmpEI22" runat="server" CssClass="form-control xs-mselect" SelectedValue='<%# Eval("IsEmployeeCPPExempty") %>'>
                                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Retain Vacation?">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlEmpEI33" runat="server" CssClass="form-control xs-mselect" SelectedValue='<%# Eval("RetainVacation") %>'>
                                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                                            <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                                            <asp:ListItem Value="No">No</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Action" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary"></asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                                <div style="display:none;">
                                 <div class="row">
                                    <h5><span>Rate History</span></h5>
                                </div>

                                <%--<div class="row">
                                    <div class="col-md-1"></div>
                                    <div class="col-md-10">--%>
                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label>Select Business</label>
                                                    <asp:DropDownList ID="ddlBussRateHistory" runat="server" Width="100%" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBussRateHistory_SelectedIndexChanged">
                                                        <asp:ListItem Text="Please Select" Value="" Selected="True"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                            </div>

                                        </div>
                                    <%--</div>
                                    <div class="col-md-1"></div>
                                </div>--%>
                            </div>
                                   <div class="row">
                                    <div class="col-md-12">


                                        <asp:GridView ID="gvRateHistory" runat="server" ClientIDMode="Static" Font-Size="Small"
                                            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                            GridLines="None" OnPreRender="gvRateHistory_PreRender">
                                            <%--<AlternatingRowStyle BackColor="White" ForeColor="#284775" />--%> <%--EmptyDataText="No Records Found"--%>
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl. No.">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex + 1%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Business Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBusinessName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Start Date" >
                                                    <ItemTemplate>
                                                        <asp:Label ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <%--<asp:TemplateField HeaderText="Effective Date">--%>
                                                <asp:TemplateField HeaderText="Start Date" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="effectivedate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "effectivedate")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="End Date">
                                                    <ItemTemplate>
                                                        <%--<asp:Label ID="EndDate" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>' />--%>
                                                        <asp:Label ID="EndDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Rate">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Rate" runat="server" Text='<%# "$" + DataBinder.Eval(Container.DataItem, "Rate")+"/"+DataBinder.Eval(Container.DataItem, "RateType") %>'></asp:Label>
                                                        <asp:HiddenField ID="hfdRateId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RateId")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Rate Type" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="RateType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "RateType")%>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                            </Columns>
                                            <%--<EmptyDataTemplate>
                                                <div>No records found.</div>
                                            </EmptyDataTemplate>--%>
                                        </asp:GridView>

                                    </div>
                                </div>
                                <%--<div class="row">
                                    <div class="col-md-12">
                                        <center>
                                            <asp:Button ID="btnFinalSave" runat="server" CssClass="btn2 btn-primary" Text="Save" OnClick="btnFinalSave_Click" />
                                        </center>
                                    </div>
                                </div>--%>
                                <div class="row">
                                <%--firoz 02--%>
                                    <%--<div class="col-md-1"></div>--%>
                                    <div class="col-md-6">
                                        <%--<center>--%>
                                            <button class="buttonlooklikelinkbutton" onclick="return addDetailsValPrev()"><< Previous</button>
                                        <%--</center>--%>
                                    </div>
                                    <div class="col-md-6">
                                        <%--<center>--%>
                                            <asp:Button ID="btnFinalSave" runat="server" CssClass="buttonhyperlink444" Text="Save" OnClick="btnFinalSave_Click" />
                                        <%--</center>--%>
                                    </div>
                                </div>

                            </div>
                            <div class="col-md-1"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
        
</asp:Content>

