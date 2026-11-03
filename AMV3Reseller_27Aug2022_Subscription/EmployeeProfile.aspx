    <%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="EmployeeProfile.aspx.cs" Inherits="EmployeeProfile" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="css/AnyminuteAddressLookup.css?v=20261102v51" rel="stylesheet" />
    <%-- v37 (DOCX image1, strict): Personal / Payment Details use the same full-width 3-column grid as Address Lookup and
         Employee Address below (they were a centred half-width column). Control IDs unchanged. --%>
    <style>
    @media(min-width:768px){
    .am-profile-wide{display:grid!important;grid-template-columns:repeat(3,minmax(0,1fr));column-gap:24px;float:none!important;width:auto!important;margin:0 16px!important;padding:0!important}
    .am-profile-wide .row{display:contents!important}
    .am-profile-wide .row::before,.am-profile-wide .row::after{display:none!important;content:none!important}
    .am-profile-wide [class*="col-"]{width:auto!important;max-width:none!important;flex:none!important;float:none!important;padding:0!important}
    .am-profile-wide>h4{grid-column:1/-1}
    .am-profile-wide .col-md-12{display:contents!important}
    }
    .am-profile-wide>h4{margin-left:0!important}
    </style>

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

    <script>
        $(document).ready(function () {
            $("#homemenu").click(function () {
                document.cookie = "tabidp=#home";
                $("#home").attr("class", "tab-pane fade in active show");
            });

            $("#menu1menu").click(function () {
                document.cookie = "tabidp=#menu1";
                $("#home").attr("class", "tab-pane fade in");
                $("#menu1").attr("class", "tab-pane fade in active show");
                
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

            var x = null; document.cookie = "tabidp=#home"; /* v36: single Personal Details pane; never hide it or the address block */

            //<li id="homemid" class="active"><a data-toggle="tab" id="homemenu" href="#home" style="font-size:15px;text-transform:uppercase;">Personal Details</a></li>
            //    <li id="addmid"><a data-toggle="tab" href="#menu1" id="menu1menu" style="font-size:15px;text-transform:uppercase;">Address and Contacts</a></li>
            //    <li id="rolemid"><a data-toggle="tab" href="#menu4" id="menu4menu" style="font-size:15px;text-transform:uppercase;">User Role and Credential</a></li>
            //    <li id="paymeid">

            //alert(arr[1]);
            if (x == '#home') {
                $("#home").attr("class", "tab-pane fade in active show");
                $("#menu1").attr("class", "tab-pane fade in");
            }
            else if (x == '#menu1') {
                $("#menu1").attr("class", "tab-pane fade in active show");
                $("#home").attr("class", "tab-pane fade in");
            }
        });
    </script>

    <script>
        $(document).ready(function () {
            //$('#GVDomainList').DataTable({
            //});
            $('#gvRateHistory').DataTable({
            });

            $('#multi-filter-select').DataTable({
                "pageLength": 5,
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select class="form-control"><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }
            });

            // Add Row
            $('#add-row').DataTable({
                "pageLength": 5,
            });

            var action = '<td> <div class="form-button-action"> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-primary btn-lg" data-original-title="Edit Task"> <i class="fa fa-edit"></i> </button> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-danger" data-original-title="Remove"> <i class="fa fa-times"></i> </button> </div> </td>';

            $('#addRowButton').click(function () {
                $('#add-row').dataTable().fnAddData([
                    $("#addName").val(),
                    $("#addPosition").val(),
                    $("#addOffice").val(),
                    action
                ]);
                $('#addRowModal').modal('hide');

            });
        });
    </script>
    <script>

        var date = new Date();
        var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());

        $('#txtrateHistoryEffectiveDate').attr("min", "2020-03-30");

    </script>

    <script type="text/javascript">

        function Copymail(val) {
            if (val.value == 'Yes') {
                var email = $('#txtMainEmail').val();
                $('#txtCCEmail').val(email);
            } else {
                $('#txtCCEmail').val('');
            }
        }

        function validatefields() {
            // v46: Canadian postal code (A1A 1A1) or US ZIP code (12345 / 12345-6789); no warning pop-ups.
            var v = ($("#txtSearchPostalCode").val() || "").replace(/\s/g, "").toUpperCase();
            var ok = /^[A-Z]\d[A-Z]\d[A-Z]\d$/.test(v) || /^\d{5}(-?\d{4})?$/.test(v);
            var box = document.getElementById("txtSearchPostalCode"); if (box) { if (ok) box.removeAttribute("aria-invalid"); else { box.setAttribute("aria-invalid", "true"); box.focus(); } }
            return ok;
        }
    </script>

    <script src="http://ajax.aspnetcdn.com/ajax/jQuery/jquery-1.10.0.min.js" type="text/javascript"></script>
    <script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/jquery-ui.min.js" type="text/javascript"></script>
    <link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.9.2/themes/blitzer/jquery-ui.css" rel="Stylesheet" type="text/css" />
    <link rel="Stylesheet" href="https://twitter.github.io/typeahead.js/css/examples.css" />

    <script type="text/javascript">
        $(function () {
            $('#txtCity').keyup(function () {
                var values = "{'city':'" + $('#txtCity').val() + "'}";
                $.ajax({
                    type: "POST",
                    url: "AddNewEmployee_TAB.aspx/GetAutoCompleteDataCity",
                    data: values,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        var val = '<ul id="userlist" style="list-style-type: none;">';
                        $.map(data.d, function (item) {
                            var itemval = item.split('/')[0];
                            val += '<li class=tt-suggestion>' + itemval + '</li>'
                        })
                        val += '</ul>'
                        $('#divautocompleteCity').show();
                        $('#divautocompleteCity').html(val);
                        $('#userlist li').click(function () {
                            $('#txtCity').val($(this).text());
                            $('#divautocompleteCity').hide();
                        })
                    },
                    error: function (response) {
                        alert(response.responseText);
                    }
                });
            })
            $(document).mouseup(function (e) {
                var closediv = $("#divautocompleteCity");
                if (closediv.has(e.target).length == 0) {
                    closediv.hide();
                }
            });
        });
    </script>

    <script type="text/javascript">
        $(function () {
            $('#txtProvince').keyup(function () {
                var values = "{'Province':'" + $('#txtProvince').val() + "'}";
                $.ajax({
                    type: "POST",
                    url: "AddNewEmployee_TAB.aspx/GetAutoCompleteDataProvince",
                    data: values,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        var val = '<ul id="userlist" style="list-style-type: none;">';
                        $.map(data.d, function (item) {
                            var itemval = item.split('/')[0];
                            val += '<li class=tt-suggestion>' + itemval + '</li>'
                        })
                        val += '</ul>'
                        $('#divautocompleteProvince').show();
                        $('#divautocompleteProvince').html(val);
                        $('#userlist li').click(function () {
                            $('#txtProvince').val($(this).text());
                            $('#divautocompleteProvince').hide();
                        })
                    },
                    error: function (response) {
                        alert(response.responseText);
                    }
                });
            })
            $(document).mouseup(function (e) {
                var closediv = $("#divautocompleteProvince");
                if (closediv.has(e.target).length == 0) {
                    closediv.hide();
                }
            });
        });
    </script>

    <script>
        function copyvalue1(Chk) {
            if (Chk.checked) {
                var txt1 = document.getElementById("<%= txtMainEmail.ClientID %>").value;
                document.getElementById("<%= txtCCEmail.ClientID %>").value = txt1;
            }
            else {
                document.getElementById("<%= txtCCEmail.ClientID %>").value = ""
            }
        }
    </script>

    <script>
        $(document).ready(function () {
            $("#txtMainEmail").focusout(function () {
                var a = $("#txtMainEmail").val();
                $("#txtCCEmail").val(a);
            });
        });
    </script>

    <script>
        $(document).ready(function () {
            // v48 (deckP s1/s2, NOTES_v46 #8): the "6 digit postal code" pop-up on leaving the field is gone; validatefields() flags the field (CA or US ZIP).
        });
    </script>
    <script>
        function getUrlVars() {
            var vars = [], hash;
            var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
            for (var i = 0; i < hashes.length; i++) {
                hash = hashes[i].split('=');
                vars.push(hash[0]);
                vars[hash[0]] = hash[1];
            }
            return vars;
        }
        $(document).ready(function () {
            var aa = getUrlVars()["EmployeeIDEdit"];
            if (aa != null) {

                $("#NextPer").attr("class", "btn btn-primary btn-round pull-right invisible");
                $("#PreviousAdd").attr("class", "btn btn-primary btn-round pull-right invisible");
                $("#NextAdd").attr("class", "btn btn-primary btn-round pull-right invisible");
                $("#PreviousUser").attr("class", "btn btn-primary btn-round pull-right invisible");
                $("#ContentPlaceHolder1_LinkButton6").attr("class", "btn btn-primary btn-round pull-right invisible");
            }
        });
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
            width: 550px;
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

        .buttonhyperlinkprevious {
            border: 1px solid #607DF2;
            display: block;
            background-color:white ;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 0px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlinkprevious:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color:white ;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 0px;
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
            border-bottom: 1px solid #fff !important;
        }

        .nav-tabs > li.active > a, .nav-tabs > li.active > a:focus, .nav-tabs > li.active > a:hover {
            color: #555;
            cursor: default;
            background-color: #fff;
            border-left: 1px solid #fff !important;
            border-right: 1px solid #fff !important;
            border-top: 1px solid #fff !important;
            border-bottom-color:#fff !important;
        }
        .nav-tabs >li>a:hover
        {
            border-bottom-color:#0275d8 !important;
        }
        .nav-tabs >li.active
        {
            border-bottom:1px solid #0275d8 !important;
        }
    </style>

    <style type="text/css">
        .buttonClass
        {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
            border-radius: 3px;
        }
        .buttonClass:hover
        {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
            border-radius: 3px;
        }
    </style>

    <style type="text/css">
        .buttonClassEye
        {
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
        }
        .buttonClassEye:hover
        {
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
    <%-- v35 (DOCX image4/5): only Personal Details remains; Personal Details and the address block always show together
         (legacy tab scripts that toggle #home/#menu1 classes can no longer hide either one). --%>
    <style>#home,#menu1{display:block!important;opacity:1!important;visibility:visible!important}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="float: left; margin-top: 10px;">Profile</h2>
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
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Profile" runat="server" />
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
            <div class="row">
                <div class="col-md-12">
                    <ul class="nav nav-tabs">
                        <li id="homemid" class="active"><a data-toggle="tab" id="homemenu" href="#home" class="tab-sm-fs" style="text-transform: uppercase;">Personal Details</a></li>
                        <%-- v35 (DOCX image5): Address and Contacts tab removed; the address block now sits at the end of Personal Details. --%>
                    </ul>
                    </div>
                </div>
            <div class="tab-content">
                <div id="home" class="tab-pane fade in active show">
                    <br />

                    <div class="row" style="display: none;">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="title">User Type</label>
                                <asp:DropDownList ID="ddlusertype11" runat="server" CssClass="form-control js-example-basic-single"
                                    AppendDataBoundItems="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12 am-profile-wide">
                            <h4 style="text-decoration: none; margin-bottom:20px;">Personal Details</h4>
                            <div class="row">
                                <div class="col-md-6" style="display:none;">
                                    <div class="form-group">
                                        <label for="Name">Title</label>
                                        <asp:TextBox ID="txtLegalName1" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:DropDownList ID="txtLegalName" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Male">Mr.</asp:ListItem>
                                            <asp:ListItem Value="Female">Miss.</asp:ListItem>
                                            <asp:ListItem Value="Others">Ms.</asp:ListItem>
                                            <asp:ListItem Value="Others">Mx.</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">First Name</label>&nbsp;&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Last Name</label>&nbsp;&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

                                    </div>
                                </div>
                            </div>

                            

                            <div class="row">
                                <div class="col-md-6" style="display:none;">
                                    <div class="form-group">
                                        <label for="Name">Middle Name</label>
                                        <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtdob">Date of Birth</label>
                                        <asp:TextBox ID="txtdob" ReadOnly="true" ValidationGroup="persnoldetails" runat="server" class="form-control"
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
                                        <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Mobile</label>
                                        <asp:TextBox ID="txtMobile" MaxLength="12" onkeypress="return isNumberKey(event)" runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name" style="font-weight:bold">Login Email</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtMainEmail"  Enabled="false" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                        <asp:RegularExpressionValidator ValidationGroup="AddressBusinessDetails" ID="RegEmail"
                                            runat="server" ControlToValidate="txtMainEmail"
                                            ForeColor="Red" ValidationExpression="^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"
                                            Display="None" ErrorMessage="Invalid email address" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender3" runat="server" TargetControlID="RegEmail"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                        <asp:RequiredFieldValidator ID="ReqEmail" ValidationGroup="AddressBusinessDetails"
                                            runat="server" ControlToValidate="txtMainEmail" Display="None" ErrorMessage="This Field Is Requried" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server" TargetControlID="ReqEmail"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                    </div>
                                </div>
                                <div class="col-md-6" style="display:none;">
                                    <div class="form-group">
                                        <label for="Name">Confirm Login Email</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtConLoginEmail" Enabled="false"  runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                    </div>
                                </div>
                                
                                <div class="col-md-12">
                                    <div class="row">
                                <%--<asp:UpdatePanel ID="UpdatePanel441" runat="server">
                                    <ContentTemplate>--%>
                                        <div class="col-md-6" style="display:none;">
                                            <%--<div class="form-group" style="margin-top:35px;text-align:left;">
                                                <asp:CheckBox Visible="true" Text="Same as Login Email"
                                                    OnClick="copyvalue1(this)" runat="server" />            
                                            </div>--%>
                                            <label id="labelsameasloginemail">Contact Email Same as Login Email?</label>
                                            <asp:DropDownList ID="ddlCopyLoginEmail" runat="server" CssClass="form-control" ClientIDMode="Static" onchange="Copymail(this)">
                                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                <asp:ListItem Value="No">No</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    <%--</ContentTemplate>
                                </asp:UpdatePanel>--%>
                                        <div class="col-md-6" style="display:none;">
                                            <div class="form-group">
                                                <label for="Name" style="font-weight:bold">Contact Email</label>
                                                &nbsp;<asp:CheckBox Text="Same as Login Email" OnClick="copyvalue1(this)" runat="server" Visible="false"/>
                                                <asp:TextBox ID="txtCCEmail" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                <asp:HiddenField ID="hfdCCEmail" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtdob">Hire Date</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtHiredate" ReadOnly="true" ValidationGroup="persnoldetails" runat="server" class="form-control"
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
                                <div class="col-md-6" style="display:none;">
                                    <div class="form-group">
                                        <label for="Name">Occupation</label>
                                        <asp:TextBox ID="txtOccupation" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>


                            <div class="row">
                                <div class="col-md-6">
                                    <div class="row">
                                        <div class="col-md-9">
                                            <div class="form-group">
                                                <label for="Name">SIN Number</label>
                                                <asp:TextBox ID="txtSocialNumer" ReadOnly="true" MaxLength="9" runat="server" placeholder="### ### ###"
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
                                    

                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">EI Exempted</label>
                                        <asp:DropDownList ID="ddlEmpEI" Enabled="false" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" > Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">CPP Exempted</label>
                                        <asp:DropDownList ID="ddlEmpCPP" Enabled="false" runat="server" CssClass="form-control">
                                            <asp:ListItem Value="" > Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">Retain Vacation?</label>
                                        <asp:DropDownList ID="ddlRTVA" Enabled="false" runat="server" CssClass="form-control">
                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                            <asp:ListItem Value="No">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                            </div>
                        </div>
                    </div>

                    <br />
                    <div class="row" style="display:none;">
                        <div class="col-md-12">
                            <h4 style="text-decoration: none;">Employment Details</h4>
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                        <div class="col-md-8">
                            <div class="row" >
                                
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtdob">Release Date</label>
                                        <asp:TextBox ID="txtReleaseDate" runat="server" class="form-control" type="date" Max="9999-12-31"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            

                            <div class="row">
                                
                            </div>

                            
                        </div>
                    </div>



                    <div class="row">
                        <div class="col-md-12 am-profile-wide">
                            <h4 style="text-decoration: none;">Payment Details</h4>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12 am-profile-wide">
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">Billing Type</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:DropDownList ID="ddlBillingType" Enabled="false" runat="server" CssClass="form-control" ClientIDMode="Static">
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
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtCity1">Billing Rate</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="txtBillRate" Enabled="false" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
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
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">Employee Status</label>
                                        <asp:DropDownList ID="ddlempStatus" Enabled="false" runat="server" ClientIDMode="Static" CssClass="form-control">
                                        <asp:ListItem Value=""> Select</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="YES">Active</asp:ListItem>
                                        <asp:ListItem Value="NO">InActive</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">InActive Date</label>
                                        <asp:TextBox ID="txtInActiveDate" Enabled="false" runat="server" Max="9999-12-31" TextMode="Date" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-md-8">
                            <div class="row">
                                
                                <div style="display:none;">

                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <%--<label for="effectiv">Effective Date</label>--%>
                                        <label for="effectiv">Start Date</label>
                                        <asp:TextBox ID="txteffectivedate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-12" style="display: none;">
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
                            </div>
                            <div class="row">
                                 
                            </div>
                        </div>
                    </div>


                    <div class="row" style="display:none;">
                        <div class="col-md-12">
                            <h4 style="text-decoration: none;">Add Contacts</h4>
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                        <div class="col-md-8">
                            <div class="row">
                                
                            </div>
                            <div class="row">
                                
                                 
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Website</label>
                                        <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6" style="display: none;">
                                    <div class="form-group">
                                        <label for="Name">Other1</label>
                                        <asp:TextBox ID="txtOther1" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                
                            </div>

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Work Phone</label>
                                        <asp:TextBox ID="txtWorkPhone" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                              
                            </div>
                            <div class="col-md-12" style="display: none;">
                                <div class="form-group">
                                    <label for="Name">Fax</label>
                                    <asp:TextBox ID="txtFax" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                   <div id="menu1" class="am-profile-address-under" style="margin-top:24px;display:block;">

                    <div class="col-md-12">
                        <div class="pull-right">
                        </div>
                    </div>


                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                    <%-- 2026-11-01 v46 (Paused deck s2/s4): Address and Contacts tab in the Business Address Lookup look (Country with
                         flag, Postal / ZIP Code with clear, Search Address, Enter Address Manually, hint, Address Not Found card,
                         Select Address from Search Results) and the Address Details card. Server IDs and btnSaveAddress unchanged. --%>
                    <div class="am-emp-al am-emp-al46 am-al2-profile" id="amEmpAddress">
                    <asp:Panel ID="pnlAddressLookup" runat="server" CssClass="am-al-layout am-al-single" DefaultButton="lnkSearch">
                       <section class="am-al-main" aria-labelledby="amEmpLookupHead">
                        <h2 id="amEmpLookupHead">Address Lookup</h2>
                        <p class="am-al-sub">Search for an address using a postal or ZIP code, select a result, or enter the address manually.</p>
                        <div class="am-al-search">
                            <div class="am-al-f am-al-country"><label for="amEmpLookupCountry">Country <b class="am-al-req">*</b></label><span class="am-al-flagsel"><span class="am-al-flag" id="amEmpLookupFlag"><svg class="am-flag-ca" viewBox="0 0 24 16" width="24" height="16" aria-hidden="true" focusable="false"><rect width="24" height="16" fill="#fff"/><rect width="6" height="16" fill="#d52b1e"/><rect x="18" width="6" height="16" fill="#d52b1e"/><path d="M12 3.2l1 2.1 1.3-.6-.4 2.6 1.4-1.3.4 1-1.2 1.1h1.1L15 9.5l.9.5-3.4.6V13h-1V10.6l-3.4-.6.9-.5-.6-1.4h1.1L8.3 7l.4-1 1.4 1.3-.4-2.6 1.3.6z" fill="#d52b1e"/></svg> <svg class="am-flag-us" viewBox="0 0 24 16" width="24" height="16" aria-hidden="true" focusable="false"><rect width="24" height="16" fill="#fff"/><g fill="#b22234"><rect width="24" height="1.23"/><rect y="2.46" width="24" height="1.23"/><rect y="4.92" width="24" height="1.23"/><rect y="7.38" width="24" height="1.23"/><rect y="9.84" width="24" height="1.23"/><rect y="12.3" width="24" height="1.23"/><rect y="14.77" width="24" height="1.23"/></g><rect width="10" height="8.6" fill="#3c3b6e"/></svg></span><select id="amEmpLookupCountry"><option value="Canada">Canada</option><option value="United States">United States</option></select></span></div>
                            <div class="am-al-f am-al-postal"><label for="txtSearchPostalCode">Postal / ZIP Code <b class="am-al-req">*</b></label><span class="am-al-clearwrap">
                                <asp:TextBox ID="txtSearchPostalCode" MaxLength="10" style="text-transform:uppercase" runat="server" placeholder="e.g. M1B 2W1" data-addr-field="LookupPostal"
                                    OnTextChanged="txtSearchPostalCode_TextChanged" AutoPostBack="false" ClientIDMode="Static" onkeyup="addHyphenn(this)"></asp:TextBox><button type="button" class="am-al-clear" data-al-clear="1" aria-label="Clear postal code">&times;</button></span></div>
                            <asp:LinkButton CssClass="am-al-btn am-al-primary" ID="lnkSearch" runat="server" OnClientClick="return validatefields();" OnClick="lnkSearch_Click"><svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false"><circle cx="11" cy="11" r="7" fill="none" stroke="currentColor" stroke-width="2.4"/><path d="M16.5 16.5L21 21" stroke="currentColor" stroke-width="2.4" stroke-linecap="round"/></svg> Search Address</asp:LinkButton>
                            <button type="button" class="am-al-btn am-al-outline" onclick="if(window.AnyminuteAddressLookup&amp;&amp;AnyminuteAddressLookup.newAddress)AnyminuteAddressLookup.newAddress(this);"><svg viewBox="0 0 24 24" width="18" height="18" aria-hidden="true" focusable="false"><path fill="currentColor" d="M3 17.25V21h3.75L17.8 9.94l-3.75-3.75L3 17.25zm17.71-10.21a1 1 0 000-1.41l-2.34-2.34a1 1 0 00-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg> Enter Address Manually</button>
                        </div>
                        <div id="LookupInfo" runat="server" class="am-al-info"><span class="am-al-i" aria-hidden="true">i</span><span id="amEmpLookupHint">Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).</span></div>
                        <asp:Literal ID="AddressNotFound" runat="server" Visible="false" />
                        <asp:Panel ID="LookupResults" runat="server" CssClass="am-al-results am-al2-choice"><label for="ddlAddressChoices">Select Address from Search Results</label>
                                <asp:DropDownList ID="ddlAddressChoices" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="ddlAddressChoices_Changed"><asp:ListItem Value="">Select an address from the list below</asp:ListItem></asp:DropDownList></asp:Panel>
                       </section>
                        <script>
                            function addHyphenn(element) {
                                // v46: "A1A 1A1" for Canadian codes only; US ZIP codes (digits) are left as typed.
                                let val = $(element).val().split(' ').join('');
                                if (/^[A-Za-z]/.test(val) && val.length > 0 && val.length <= 6) $(element).val(val.match(/.{1,3}/g).join(' '));
                            }
                        </script>
                        <%-- Legacy result list, kept hidden for the existing code-behind; the dropdown above replaces it. --%>
                        <div style="display:none">
                            <div id="resultdiv" runat="server" visible="false">
                                <asp:GridView ID="gvAddressList" runat="server" ClientIDMode="Static" AutoGenerateColumns="False" ShowHeader="False">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Address">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SEARCHADDRESS")%>' OnClick="StartDate_Click" />
                                                <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                <asp:RadioButtonList ID="rdobtnlist" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rdobtnlist_SelectedIndexChanged" AutoPostBack="true" Visible="false">
                                    <asp:ListItem Text=" Add New Address" Value="0"></asp:ListItem>
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </asp:Panel>

                    <div id="divAddres" runat="server" class="am-al-details am-emp-details">
                        <div class="am-al-details-head"><h3>Employee Address</h3><span class="am-al-pill">Address Details</span></div>
                        <div class="am-al-grid">
                            <div class="am-al-f"><label for="txtStreetNo">Street Number <b class="am-al-req">*</b></label><asp:TextBox ID="txtStreetNo" runat="server" ClientIDMode="Static" MaxLength="20" placeholder="Enter street number" data-addr-field="StreetNo"></asp:TextBox></div>
                            <div class="am-al-f am-al-span2"><label for="txtStreetName">Address Line 1 <b class="am-al-req">*</b></label><asp:TextBox ID="txtStreetName" runat="server" ClientIDMode="Static" placeholder="Enter address line 1" data-addr-field="Line1"></asp:TextBox></div>
                            <div class="am-al-f"><label for="txtStreetType">Address Line 2 <span class="am-al-opt">(Optional)</span></label><asp:TextBox ID="txtStreetType" runat="server" ClientIDMode="Static" placeholder="Enter address line 2 (e.g. Unit, Floor, etc.)" data-addr-field="Line2"></asp:TextBox></div>
                            <div class="am-al-f"><label for="txtSuiteNumber">Suite Number <span class="am-al-opt">(Optional)</span></label><asp:TextBox ID="txtSuiteNumber" runat="server" ClientIDMode="Static" placeholder="Enter suite number" data-addr-field="SuiteNumber"></asp:TextBox></div>
                            <div class="am-al-f"><label for="DropDownList1">Suite Type <span class="am-al-opt">(Optional)</span></label><asp:DropDownList ID="DropDownList1" runat="server" ClientIDMode="Static" AppendDataBoundItems="true" data-addr-field="SuiteType">
                                <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                <asp:ListItem Value="Apartment" Text="Apartment"></asp:ListItem>
                                <asp:ListItem Value="Suite" Text="Suite"></asp:ListItem>
                                <asp:ListItem Value="Unit" Text="Unit"></asp:ListItem>
                            </asp:DropDownList></div>
                            <div class="am-al-subhead" role="heading" aria-level="4">City, State/Province and Country</div>
                            <div class="am-al-f"><label for="txtCity">City <b class="am-al-req">*</b></label><asp:TextBox ID="txtCity" runat="server" ClientIDMode="Static" placeholder="Enter city" data-addr-field="City"></asp:TextBox><div id="divautocompleteCity" class="tt-menu" style="display: none"></div></div>
                            <div class="am-al-f"><label for="amProfileProvince">Province / State <b class="am-al-req">*</b></label><select id="amProfileProvince" data-addr-field="Province" data-al2-mirror="txtProvince"><option value="">Select</option><option>ALBERTA</option><option>BRITISH COLUMBIA</option><option>MANITOBA</option><option>NEW BRUNSWICK</option><option>NEWFOUNDLAND AND LABRADOR</option><option>NORTHWEST TERRITORIES</option><option>NOVA SCOTIA</option><option>NUNAVUT</option><option>ONTARIO</option><option>PRINCE EDWARD ISLAND</option><option>QUEBEC</option><option>SASKATCHEWAN</option><option>YUKON</option><option>ALABAMA</option><option>ALASKA</option><option>ARIZONA</option><option>ARKANSAS</option><option>CALIFORNIA</option><option>COLORADO</option><option>CONNECTICUT</option><option>DELAWARE</option><option>DISTRICT OF COLUMBIA</option><option>FLORIDA</option><option>GEORGIA</option><option>HAWAII</option><option>IDAHO</option><option>ILLINOIS</option><option>INDIANA</option><option>IOWA</option><option>KANSAS</option><option>KENTUCKY</option><option>LOUISIANA</option><option>MAINE</option><option>MARYLAND</option><option>MASSACHUSETTS</option><option>MICHIGAN</option><option>MINNESOTA</option><option>MISSISSIPPI</option><option>MISSOURI</option><option>MONTANA</option><option>NEBRASKA</option><option>NEVADA</option><option>NEW HAMPSHIRE</option><option>NEW JERSEY</option><option>NEW MEXICO</option><option>NEW YORK</option><option>NORTH CAROLINA</option><option>NORTH DAKOTA</option><option>OHIO</option><option>OKLAHOMA</option><option>OREGON</option><option>PENNSYLVANIA</option><option>RHODE ISLAND</option><option>SOUTH CAROLINA</option><option>SOUTH DAKOTA</option><option>TENNESSEE</option><option>TEXAS</option><option>UTAH</option><option>VERMONT</option><option>VIRGINIA</option><option>WASHINGTON</option><option>WEST VIRGINIA</option><option>WISCONSIN</option><option>WYOMING</option></select>
                                <asp:TextBox ID="txtProvince" runat="server" ClientIDMode="Static" style="display:none" aria-hidden="true" tabindex="-1"></asp:TextBox><div id="divautocompleteProvince" class="tt-menu" style="display: none"></div></div>
                            <div class="am-al-f"><label for="txtPostalCode">Postal / ZIP Code <b class="am-al-req">*</b></label><asp:TextBox ID="txtPostalCode" MaxLength="10" runat="server" ClientIDMode="Static" onkeyup="addHyphenn(this)" style="text-transform:uppercase" placeholder="Enter postal code" data-addr-field="PostalCode"></asp:TextBox></div>
                            <div class="am-al-f am-al-span2 am-al-countryf"><label for="amProfileCountry">Country <b class="am-al-req">*</b></label><select id="amProfileCountry" data-addr-field="Country"><option>Canada</option><option>United States</option></select></div>
                        </div>
                        <div style="display: none;"><asp:TextBox ID="txtStreetDirection" ClientIDMode="Static" runat="server"></asp:TextBox></div>
                        <div class="form-group" style="display: none;">
                            <div class="col-md-6">
                                <label for="ddlnumbersuffix">Number Suffix</label><span>*</span>
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
                        <div class="col-md-12">
                            <div class="form-group" style="display: none;">
                                <div class="col-md-10">
                                    <label for="Name">Street Type</label>
                                    <asp:TextBox ID="txtStreetType1" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group" style="display: none;">
                                <div class="col-md-8">
                                    <label for="Name">Street Direction</label>
                                    <asp:TextBox ID="txtStreetDirection1" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <div class="form-group" style="display: none;">
                                <label for="ddlsuitetype2">SuiteType</label><br />
                                <asp:DropDownList ID="ddlSuitType1"
                                    CssClass="form-control" AppendDataBoundItems="true" runat="server" Width="100%">
                                    <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                    <asp:ListItem Value="Duplex" Text="Duplex"></asp:ListItem>
                                    <asp:ListItem Value="Simplex" Text="Simplex"></asp:ListItem>
                                    <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                    <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                    <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
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

                        <div class="row">
                            <div class="col-md-4">
                                <div class="form-group">
                                   
                                </div>
                            </div>
                        </div>

                    </div>

                    </ContentTemplate>
                                <Triggers>
                                     <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click"/>
                                </Triggers>
                                </asp:UpdatePanel>

                       <div class="row">
                           <div class="col-md-4"></div>
                           <div class="col-md-2">
                               <asp:Button ID="btnSaveAddress" runat="server" Visible="false" Text="Save" CssClass="btn2 btn-primary" OnClick="btnSaveAddress_Click" />
                            </div>
                            <div class="col-md-1"></div>
                            <div class="col-md-1">
                                <%-- v32: Previous button removed from Address and Contacts (Save stays). --%>
                            <script>
                                $("#PreviousAdd").click(function () {
                                    document.cookie = "tabid=#home";
                                    $("#homemid").attr("class", "active");
                                    $("#addmid").attr("class", "");
                                    $("#rolemid").attr("class", "");
                                    $("#paymeid").attr("class", "");
                                    $("#menu1").attr("class", "tab-pane fade in");
                                    $("#menu3").attr("class", "tab-pane fade in");
                                    $("#menu4").attr("class", "tab-pane fade in");
                                    $("#menu2").attr("class", "tab-pane fade in");
                                });
                            </script>
                            </div>
                           
                           
                       </div>
                       <div class="row">
                           <div class="col-lg-5"></div>
                           
                           <div class="col-lg-5"></div>
                       </div>




                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                        <div class="row">
                        <div class="col-md-3">
                            
                        </div>
                        <div class="col-md-6">
                        </div>
                        <div class="col-md-3" style="visibility:hidden;">
                            <a data-toggle="tab" href="#menu4" id="NextAdd" class="buttonhyperlink" style="float: right;">
                                Next <i class="fa fa-angle-right"></i></a>
                            <%--<a href="#menu4" id="NextAdd" class="buttonsquarelink" data-toggle="modal" data-backdrop="static"
                                data-keyboard="false" data-target="#myModal" style="float: right;">Next <i class="fa fa-angle-right">
                                </i></a>--%>
                        </div>

                        <script>

                        $("#NextAdd").click(function () {

                            var isValidNext = true;

                            //var Suffixx = $("#txtLegalName option:selected").val();
                            //if (Suffixx == "") {
                            //    $("#txtLegalName").focus();
                            //    //alert("");
                            //    swal('Warning!', 'Please select suffix', 'warning');
                            //    //$("#txtFirstName").css("border", "1px solid red");
                            //    return false;
                            //}
                            var FName = $("#txtFirstName").val();
                            if (FName == "") {
                                $("#txtFirstName").focus();
                                //alert("Please enter first name");
                                swal('Warning!', 'Please enter first name in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var LName = $("#txtLastName").val();
                            if (LName == "") {
                                $("#txtLastName").focus();
                                //alert("Please enter last name");
                                swal('Warning!', 'Please enter last name in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var LName = $("#txtMainPhone").val();
                            if (LName == "") {
                                $("#txtMainPhone").focus();
                                //alert("Please enter last name");
                                swal('Warning!', 'Please enter contact phone in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }

                            //var _txtSocialNumer = $("#txtSocialNumer").val();
                            //var _txtSocialNumerlength = document.getElementById("txtSocialNumer").value.length;
                            //if (_txtSocialNumerlength != "9") {
                            //    $("#txtSocialNumer").focus();
                            //    swal('Warning!', 'Please enter 9 digit social number', 'warning');

                            //    //$("#txtSocialNumer").css("border", "1px solid red");
                            //    return false;
                            //}
                            //var DOB = $("#txtdob").val();
                            //if (DOB == "") {
                            //    $("#txtdob").focus();
                            //    //alert("Please enter date of birth");
                            //    swal('Warning!', 'Please enter date of birth', 'warning');
                            //    //$("#txtFirstName").css("border", "1px solid red");
                            //    return false;
                            //}

                            var MEmail = $("#txtMainEmail").val();
                            if (MEmail == "") {
                                $("#txtMainEmail").focus();
                                swal('Warning!', 'Please enter login email in personal details', 'warning');
                                // alert("");
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            //var emailExp = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([com\co\.\in])+$/; // to validate email id
                            var emailExp = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9\-])+$/; // to validate email id
                            if (MEmail != '') {
                                if (!MEmail.match(emailExp)) {
                                    $("#txtMainEmail").focus();
                                    swal('Warning!', 'Invalid login email format in personal details', 'warning');
                                    //alert("");
                                    return false;
                                }
                            }

                            var HDate = $("#txtHiredate").val();
                            if (HDate == "") {
                                $("#txtHiredate").focus();
                                //alert("");
                                swal('Warning!', 'Please enter hire date in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var BType = $("#ddlBillingType option:selected").val();
                            if (BType == "") {
                                $("#ddlBillingType").focus();
                                //alert("");
                                swal('Warning!', 'Please select billing type in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var Brate = $("#txtBillRate").val();
                            if (Brate == "") {
                                $("#txtBillRate").focus();
                                // alert("Please enter billing rate");
                                swal('Warning!', 'Please enter billing rate in personal details', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }


                            //var _btnsaveRole = $("#btnsaveRole").val();
                            //if (_btnsaveRole == "Save") {
                            //    var _txtSearchPostalCode = $("#txtSearchPostalCode").val();
                            //    if (_txtSearchPostalCode == "") {
                            //        $("#txtSearchPostalCode").focus();
                            //        swal('Warning!', 'Please enter 6 digit postal code and click on Search', 'warning');
                            //        // alert("Please enter street no");
                            //        //$("#txtFirstName").css("border", "1px solid red");
                            //        return false;
                            //    }
                            //}
                            // v48: Search Postal is not required to save (Enter Address Manually fills the Address Details directly).
                            var _txtStreetNo = $("#txtStreetNo").val();
                            if (_txtStreetNo == "") {
                                $("#txtStreetNo").focus();
                                swal('Warning!', 'Please enter street number', 'warning');
                                // alert("Please enter street no");
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var _txtStreetName = $("#txtStreetName").val();
                            if (_txtStreetName == "") {
                                $("#txtStreetName").focus();
                                swal('Warning!', 'Please enter address - line 1', 'warning');
                                //alert("");
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            // v48: Suite Number is optional (as labelled in the Business lookup look).
                            var _txtCity = $("#txtCity").val();
                            if (_txtCity == "") {
                                $("#txtCity").focus();
                                swal('Warning!', 'Please enter city', 'warning');

                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            var _txtProvince = $("#txtProvince").val();
                            if (_txtProvince == "") {
                                $("#txtProvince").focus();
                                // alert("Please enter province");
                                swal('Warning!', 'Please enter province', 'warning');
                                //$("#txtFirstName").css("border", "1px solid red");
                                return false;
                            }
                            // v48 (NOTES_v46 #8): Canadian postal code or US ZIP accepted; an invalid code is flagged on the field, no pop-up.
                            var _pc = ($("#txtPostalCode").val() || "").replace(/\s/g, "").toUpperCase();
                            var _pcBox = document.getElementById("txtPostalCode");
                            if (!(/^[A-Z]\d[A-Z]\d[A-Z]\d$/.test(_pc) || /^\d{5}(-?\d{4})?$/.test(_pc))) {
                                if (_pcBox) { _pcBox.setAttribute("aria-invalid", "true"); _pcBox.focus(); }
                                return false;
                            }
                            if (_pcBox) _pcBox.removeAttribute("aria-invalid");
                            

                            if (isValidNext == true) {
                                document.cookie = "tabid=#menu3";
                                $("#menu1").attr("class", "tab-pane fade in")
                                $("#menu4").attr("class", "tab-pane fade in active show")
                            }
                            document.cookie = "tabid=#menu3";
                            $("#homemid").attr("class", "");
                            $("#addmid").attr("class", "");
                            $("#rolemid").attr("class", "active");
                            $("#paymeid").attr("class", "");
                        });
                    </script>
                        </div>
                        </div>
                        <div class="col-md-3"></div>
                    </div>

                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-5"></div>
                              
                                <div class="col-md-5"></div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>

                    

                    <%-- v36 (DOCX image4, strict): address block is the last part of Personal Details; one Save below it (both old Saves ran the same update). Next link removed. --%>
                    <div class="row am-profile-save" style="margin-top:18px;">
                        <div class="col-md-12" style="text-align:center;">
                            <asp:Button ID="btnSavePersnol" runat="server" CssClass="btn2 btn-primary" Text="Save" OnClick="btnsavePresnol_Click" />
                        </div>
                    </div>
                </div>
                </div>
                </div>
            </div>
        </div>
<script src="Scripts/AnyminuteAddressLookup.js?v=20261102v49"></script>
</asp:Content>

