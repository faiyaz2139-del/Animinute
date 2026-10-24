<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Time_Sheet_Entry.aspx.cs" Inherits="Time_Sheet_Entry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">

    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

    <style>
        .switch {
            position: relative;
            display: inline-block;
            width: 50px;
            height: 34px;
        }

        .switch input {
            display: none;
        }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #585958;
            -webkit-transition: .4s;
            transition: .4s;
            border-radius: 4px;
        }

        .slider:before {
            position: absolute;
            content: "";
            height: 26px;
            width: 10px;
            left: 4px;
            bottom: 4px;
            background-color: white;
            -webkit-transition: .4s;
            transition: .4s;
            border-radius: 4px;
        }

        input:checked + .slider {
            background-color: #607DF2;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(50px);
            -ms-transform: translateX(50px);
            transform: translateX(33px);
        }

        /*------ ADDED CSS ---------*/
        .slider:after {
            content: 'No';
            color: white;
            display: block;
            position: absolute;
            transform: translate(-50%,-50%);
            top: 50%;
            left: 50%;
            font-size: 10px;
            font-family: Verdana, sans-serif;
        }

        input:checked + .slider:after {
            content: 'Yes';
        }

        .modalBackground {
            background-color: Black;
            /*filter: alpha(opacity=60);*/
            opacity: 0.6;
        }

        .modalPopup {
            background-color: #FFFFFF;
            width: auto;
            border: 3px solid #1269db;
            padding: 5px;
            border-radius: 5px;
        }

        .modalPopup .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 5px;
            margin-bottom: 10px;
        }

        .modalPopup .body {
            min-height: 50px;
            line-height: 30px;
            font-weight: bold;
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            /*overflow-y: scroll;*/
            height: inherit;
            width: auto;
            padding: 10px;
        }

        .textboxAslabel {
            border: none;
            background-color: #FFF;
            border-color: #FFF;
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

        .buttonhyperlink222 {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            /*padding: 2px 25px;*/
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
            /*padding: 5px 25px;*/
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink333 {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
            width:82%;
        }

        .buttonhyperlink333:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
            width:82%;
        }

        .buttonhyperlink444 {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
            width:88%;
        }

        .buttonhyperlink444:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
            width:88%;
        }
    </style>

    <script type="text/javascript">

        $(document).ready(function () {
            $('#GvPopUpEmpList').DataTable(
                {
                });
        });

        $("[src*=plus]").live("click", function () {
            $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
            $(this).attr("src", "Assets/assets/img/minus.png");
            $(this).next().next().val(1);
        });

        $("[src*=minus]").live("click", function () {
            $(this).attr("src", "Assets/assets/img/plus.png");
            $(this).closest("tr").next().remove();
            $(this).next().next().val("");

        });

    </script>

    <script>
        $.noConflict();
    </script>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <link rel="stylesheet" type="text/css" href="Jquery/jquery.datetimepicker.css"/ >
    <script src="Jquery/jquery.js"></script>
    <script src="Jquery/build/jquery.datetimepicker.full.min.js"></script>
    <script>
       

        // Initialize once per input; never overwrite a time selected by the user.
        function initializeEntryDateTime(input, defaultHour) {
            var row = input.closest ? input.closest('tr') : $(input).closest('tr')[0];
            if (!row) return;
            var dateLabel = row.querySelector("[id$='lblDate']");
            var dateText = dateLabel ? $(dateLabel).text().trim() : '';
            var $input = $(input);
            if ($input.data('anyminutePickerReady')) return;
            $input.data('anyminutePickerReady', true);
            var settings = {
                format: 'Y-m-d h:i A',
                formatTime: 'h:i A',
                step: 15,
                validateOnBlur: false
            };
            // Only supply an initial value if the textbox is actually empty.
            // No UTC/local timezone conversion is applied to wall-clock entries.
            if (!$input.val().trim() && /^\d{4}-\d{2}-\d{2}$/.test(dateText)) {
                settings.value = dateText + (defaultHour === 8 ? ' 08:00 AM' : ' 06:00 PM');
            }
            $input.datetimepicker(settings);
        }
        function setEndDateTime(input) { initializeEntryDateTime(input, 18); }
        function setStartDateTime(input) { initializeEntryDateTime(input, 8); }
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupEmpList" BehaviorID="mpe" runat="server"
                PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
                CancelControlID="btnHide">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
                <div class="header">
                    <asp:Label ID="lblbusName" runat="server"></asp:Label>
                    <asp:HiddenField ID="hfdweeknopop" runat="server" />
                    <asp:HiddenField ID="hfddatebreak" runat="server" />
                    <div class="pull-right" style="margin-right: 5px;">
                        <button type="button" id="btnHide" class="btn-sm btn-danger">
                            <i class="fa fa-window-close" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>

                <div class="body">
                    <div class="row">
                        <div class="col-md-5">
                            <label>Break Start Time</label>
                            <asp:TextBox ID="txtBreakStartTime" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>

                        </div>
                        <div class="col-md-5">
                            <label>Break End Time</label>
                            <asp:TextBox ID="txtBreakEndTime" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <label>&nbsp;</label>
                            <%--<label style="visibility: hidden;">Break End Time</label>--%>
                            <%--<asp:Button ID="btnAddBreak" runat="server" CssClass="btn btn-primary" formnovalidate Text="Add"
                                OnClick="btnAddBreak_Click" Width="100%" />--%>
                            <%--<asp:Button ID="btnAddBreak" runat="server" CssClass="buttonhyperlink222" Text="Add"
                                OnClick="btnAddBreak_Click" />--%>
                            <asp:LinkButton ID="btnAddBreak" runat="server" CausesValidation="false" CssClass="buttonhyperlink222" Text="Add"
                                OnClick="btnAddBreak_Click" />
                        </div>
                    </div>
                    <%--<br />
                    <br />--%>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                GridLines="None" 
                                OnPreRender="GvPopUpEmpList_PreRender"> <%--EmptyDataText="No Records Found"--%>
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                            <asp:HiddenField ID="brkid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BreakID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Break Start">
                                        <ItemTemplate>
                                            <asp:Label ID="BreakStart" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BreakStart")%>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Break End">
                                        <ItemTemplate>
                                            <asp:Label ID="BreakEnd" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BreakEnd")%>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Total">
                                        <ItemTemplate>
                                            <asp:Label ID="Total" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BreakTotal")%>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                             <asp:LinkButton ID="lnkEditBreak" runat="server" Text="Edit" OnClick="lnkEditBreak_Click" CausesValidation="false" /> | <asp:LinkButton ID="lnkdelete" runat="server" Text="Delete" OnClick="lnkdelete_Click" CausesValidation="false"></asp:LinkButton>

                                           </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                                <%--<EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>--%>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>


            <asp:LinkButton ID="lnkDummy2" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupConfirmSingal" runat="server"
                PopupControlID="pnlConfirmSingal" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground"
                CancelControlID="btnHide2">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmSingal" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;">
                    <div class="swal2-header">
                        <ul class="swal2-progresssteps" style="display: none;"></ul>
                        <div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div>
                        <div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div>
                        <div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div>
                        <div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div>
                        <div class="swal2-icon swal2-success" style="display: none;">
                            <div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div>
                            <span class="swal2-success-line-tip"></span><span class="swal2-success-line-long"></span>
                            <div class="swal2-success-ring"></div>
                            <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div>
                            <div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div>
                        </div>
                        <img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2>
                        <button type="button" class="swal2-close" style="display: none;">×</button></div>
                    <div class="swal2-content">
                        <%--<div id="swal2-content" style="display: block;">For the selected date, time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?</div>--%>
                        <%--<div id="swal2-content" style="display: block;">Time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?</div>--%>
                        <div id="swal2-content" style="display: block;">Time sheet will be changed to Absent. Please confirm?</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" id="btnHide2"></button>
                        <div class="row" style="margin-top: 20px;">
                            <div class="col-md-6" style="text-align: center; background-color: white">
                                <asp:HiddenField ID="hfdRIndex" runat="server" />
                                <asp:LinkButton ID="lnkbtnYesSingal" runat="server" Text="Yes" CssClass="btn btn-primary" OnClick="lnkbtnYesSingal_Click" />
                            </div>
                            <div class="col-md-6" style="text-align: center;">
                                <asp:LinkButton ID="lnkbtnNoSingal" runat="server" Text="No" CssClass="btn btn-primary" OnClick="lnkbtnNoSingal_Click" />
                            </div>
                        </div>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>

            </asp:Panel>

            <asp:LinkButton ID="LinkButton13" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopup_NewAction" runat="server"
                PopupControlID="pnl_Action" TargetControlID="LinkButton13" BackgroundCssClass="modalBackground"
                CancelControlID="btnHide13">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnl_Action" runat="server" CssClass="modalPopup" Style="display: none">
                <div class="header">
                    <%--<label style="width: 300px; color: white">Enter Timesheet for </label>--%>
                    <asp:Label ID="Label_Selected_Emp_Name_Popup" style="width: 300px; color: white" Text="" runat="server" />
                    <div class="pull-right" style="margin-right: 5px;">
                        <button type="button" id="btnHide13" class="btn-sm btn-danger">
                            <i class="fa fa-window-close" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>

                <div class="body">
                    <div class="row">
                        <center></center>
                    </div>
                    <div class="row">
                        <div class="col-md-5">
                            <label>Start Date / Time</label>
                            <asp:TextBox ID="TextBox1" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>
                            <asp:HiddenField ID="HiddenField13" runat="server" />
                        </div>
                        <div class="col-md-5">
                            <label>End Date / Time</label>
                            <asp:TextBox ID="TextBox2" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-md-2" style="margin-left:-5px;">
                            <label>&nbsp;</label>
                            <asp:LinkButton ID="Button_Save_Action_popup" runat="server" CssClass="buttonhyperlink222" Text="Save" OnClick="Button_Save_Action_popup_Click"/>
                        </div>
                    </div>
                </div>

            </asp:Panel>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAddBreak" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <style>
                /*Added for MOBILE-ISSUE-SURYA*/
                @media (max-width:767px) {
                    .pheader, #ContentPlaceHolder1_hypProjectTime, #ContentPlaceHolder1_hyBack {
                        float: left !important;
                    }

                    #ContentPlaceHolder1_lblbid {
                        /*margin-left: -10px;*/
                        float: left !important;
                    }
                }
            </style>
            <div class="row pheader">
                <div class="col-md-4">
                    <h2 style="margin-top: 12px; text-align: left;">Time Sheet Entry</h2>
                </div>
                <div class="col-md-4">
                    <div style="width: 100%; text-align: center; margin-top: 20px;">
                        <%--<h4>
                            <asp:Label ID="lblbid" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label></h4>--%>
                        <asp:LinkButton ID="lnkAddEmp" ClientIDMode="Static" runat="server" Width="50%" CssClass="btn btn-primary"
                            Text="Add New Employee" Visible="false" OnClick="lnkAddEmp_Click"></asp:LinkButton>
                        <script>
                            $('#lnkAddEmp').click(function () {
                                document.cookie = "tabid=#home";
                            });
                        </script>
                    </div>
                </div>
                <div class="col-md-4">
                    <asp:HyperLink ID="hyBack" runat="server" NavigateUrl="~/Home_Page.aspx" CssClass="buttonhyperlink"
                        Style="margin-top: 12px; float: right;">Home
                    </asp:HyperLink>
                </div>
                <div style="width: 80%; text-align: center;">
                    <asp:Label ID="Label1" runat="server" Font-Size="Large"></asp:Label>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-3">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Card - " runat="server" /><asp:Label ID="lblbid" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>
                <center><asp:Label ID="lblLocked" Font-Bold="true"  Text="Locked" runat="server" ForeColor="Red" /></center>
            </div>
            <div class="col-md-3" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-2">
                    <div class="form-group">
                        <label>Select Employee</label>
                        <asp:DropDownList ID="ddlemp" runat="server" AppendDataBoundItems="true" CssClass="form-control"
                            OnSelectedIndexChanged="ddlemp_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <%--<asp:TextBox ID="lblCurrentRate" Font-Bold="true" ReadOnly="true" runat="server" />--%>
                        <asp:Label ID="lblCurrentRate" ReadOnly="true" runat="server" />
                        <asp:Label ID="lblCurrentErrorSms" ReadOnly="true" runat="server" />
                    </div>
                </div>

                <div class="col-md-1" style="display:none;">
                    <div class="form-group">
                        <label>Rate</label>
                        <asp:TextBox ID="lblCurrentRate___" Font-Bold="true" ReadOnly="true" CssClass="form-control" runat="server" />
                        <asp:Label ID="lblCurrentErrorSms___" Font-Bold="true" ReadOnly="true" runat="server" />
                    </div>
                </div>

                <style>
                   @media (min-width:767px) {
                       #btnprv {
                           margin-top: 5.5px;
                           padding: 4px;
                           margin-right: -29px;
                           margin-left: 45px;
                       }

                       #ContentPlaceHolder1_btnnext {
                           margin-top: 5.5px;
                           padding: 4px;
                           margin-left: -29.5px;
                           margin-right: 45px;
                       }
                       .btnprvmargin{
                            margin-left: -50px;
                       }
                       .lnkbtncopytimesheet{
                            margin-left: -50px;
                       }
                   }
               </style>
                
                <div class="col-md-1 btnprvmargin">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton ID="btnprv" runat="server" CssClass="buttonhyperlink"
                            ClientIDMode="Static"
                            OnClick="btnprv_Click">
                                <i class="fa fa-angle-left" aria-hidden="true"></i> 
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="col-md-2">
                    <div class="form-group">
                        <label>Date Range</label>
                        <asp:TextBox ID="txtmonth" runat="server" TextMode="Date" OnTextChanged="txtmonth_TextChanged"
                            AutoPostBack="true" required="required" CssClass="form-control" Max="9999-12-31"></asp:TextBox>
                        <asp:Label ID="lblWeek" runat="server" Visible="false" Font-Size="16"></asp:Label>
                    </div>
                </div>

                <div class="col-md-1">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton ID="btnnext" runat="server" CssClass="buttonhyperlink"
                            OnClick="btnnext_Click"><i class="fa fa-angle-right" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="col-md-4 lnkbtncopytimesheet"><%--<div style="height:30px;"></div>--%>
                    <div class="form-group">
                        <asp:LinkButton
                            ID="LinkButton1" ToolTip="Copy first day timesheet to entire week"
                            OnClientClick="return confirm('Repeat the first saved present day into empty, pending days of this week? Entered and reviewed days will be kept.');" runat="server" CausesValidation="false" Text="Copy Entire Week"
                            OnClick="LinkButton_CopyCurrentWeek_Click">
                        </asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton
                            ID="LinkButton_CopyNextWeek" OnClientClick="return confirm('Copy saved shifts and breaks to next week? Unapproved destination entries will be replaced. Locked or approved days stop the copy.');" ToolTip="Copy current week timesheet to next week"
                            runat="server" CausesValidation="false" Text="Copy Next Week"
                            OnClick="LinkButton_CopyNextWeek_Click">
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="col-md-3" style="display:none;">
                    <div class="form-group">
                        <label>&nbsp;</label>
                        <asp:LinkButton
                            ID="LinkButton_CopyCurrentWeek" OnClientClick="return confirm('Repeat the first saved present day into empty, pending days of this week? Entered and reviewed days will be kept.');"
                            runat="server" CausesValidation="false" Text="Copy Entire Week"
                            OnClick="LinkButton_CopyCurrentWeek_Click">
                        </asp:LinkButton>
                    </div>
                </div>                
                 
                <div class="col-md-1" style="display:none;">
                    <div class="form-group">
                        <asp:LinkButton ID="lnkApproved" Visible="false" ToolTip="Approved" Width="100%" runat="server" ClientIDMode="Static"
                            Text="Approved"
                            OnClick="lnkApproved_Click" CssClass="buttonhyperlink"> </asp:LinkButton>
                    </div>
                </div>
                <div class="col-md-1" style="display:none;">
                    <div class="form-group">
                        <asp:LinkButton ID="lnkReject" Visible="false" ToolTip="Reject" runat="server" Text="Reject" Width="100%"
                            OnClick="lnkReject_Click" CssClass="buttonhyperlink"></asp:LinkButton>
                    </div>
                </div>

            </div>

            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x: auto;">
                        <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>

                                <asp:GridView ID="GVChildlist" runat="server" ClientIDMode="Static"
                                    CssClass="table FS14" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                    GridLines="None" EmptyDataText="No Records Found" HeaderStyle-BackColor="#607DF2"
                                    HeaderStyle-ForeColor="White"
                                    OnRowCommand="GVChildlist_RowCommand" OnRowDataBound="GVChildlist_RowDataBound"
                                    OnPreRender="GVChildlist_PreRender"> <%--CssClass="table table-striped wrap"--%>
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="25px" Visible="false">
                                            <ItemTemplate>
                                                <asp:CheckBox runat="server" ID="chkApproved" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Date">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDay" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "weekname")%>'></asp:Label><br />
                                                <asp:Label ID="lblDate" Width="100px" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SelDate")%>'></asp:Label>
                                                <asp:HiddenField ID="hfdweekno" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "weekno")%>' />
                                                <asp:HiddenField ID="hfdweekdayNo" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "weekdayno")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <%--<asp:TemplateField HeaderText="Day">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDay" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "weekname")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Worked">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlWorked" Width="80%" CssClass="form-control xs-mselect" runat="server">
                                                    <asp:ListItem Selected="True" Value="No">Absent</asp:ListItem>
                                                    <asp:ListItem Value="Yes">Present</asp:ListItem>
                                                </asp:DropDownList><%--Width="75px"--%>
                                                <asp:HiddenField ID="hfdWorked" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Worked")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Start Time"><%--ItemStyle-HorizontalAlign="Center"--%>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtRegularStartTime" Width="85%" TextMode="DateTime"  CssClass="form-control" ClientIDMode="AutoID"
                                                    runat="server" Text='<%# Bind("ReqularStart") %>' OnFocus="setStartDateTime(this);" onClick="setStartDateTime(this);" ></asp:TextBox>
                                                    <%--Width="190px"--%>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="End Time"><%--ItemStyle-HorizontalAlign="Center"--%>
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtRegularEndTime" Width="190px" TextMode="DateTime" CssClass="form-control" ClientIDMode="AutoID"
                                                    runat="server"  Text='<%# Bind("Regularend")%>' OnFocus="setEndDateTime(this);" onClick="setEndDateTime(this);" ></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Break Start" Visible="false">
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Break End" Visible="false">
                                            <ItemTemplate>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Over 1.5x" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtOver15x" Width="50px" TextMode="Number" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Over1_5x")%>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Over 2x" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtOver2x" TextMode="Number" Width="50px" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Over2x")%>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Holiday</br>Others" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtholiday" Width="50px" TextMode="Number" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Holiday")%>'></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Total Hours">
                                            <ItemTemplate>
                                                <asp:Label ID="lblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Regular")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Add/View Break">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkAddBreak" runat="server" CommandName="AddBreak" Text="Add Break"
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Break Taken">
                                            <ItemTemplate>
                                                <asp:Label ID="lblBreakTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BreakTotal")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Net Hours">
                                            <ItemTemplate>
                                                <asp:Label ID="lblnetTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Total")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Status" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hfdStatus" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Action" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkSave" runat="server" CommandName="Save" Visible="false"
                                                    Text='<%#DataBinder.Eval(Container.DataItem, "Regular").ToString() == "" ? "Save": "Update"%>'
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'></asp:LinkButton>
                                                <asp:LinkButton ID="lnksubmit" runat="server" CommandName="Submit" Text="Submit" Visible="false"
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>'></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Change Status" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkGvApp" runat="server" Text="Approved/" OnClick="lnkGvApp_Click"></asp:LinkButton>
                                                <asp:LinkButton ID="lnkGvReject" runat="server" Text="Reject" OnClick="lnkGvReject_Click"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <div class="dropdown">
                                                    <%--<button class="buttonlooklikelinkbutton" id="dropdownMenu1" 
                                                        data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                                                        Action&nbsp;<i class="fa fa-angle-down" aria-hidden="true"></i>
                                                    </button>--%>
                                                    <asp:LinkButton ID="dropdownMenu1" runat="server" Text="Action" data-toggle="dropdown" 
                                                        aria-haspopup="true" aria-expanded="true" />&nbsp;<i class="fa fa-angle-down" style="color:#607DF2;" aria-hidden="true"></i>
                                                    <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
                                                        <li><asp:LinkButton ID="LinkButton_Save" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Save" OnClick="LinkButton_Save_Click"></asp:LinkButton></li>
                                                        <li><asp:LinkButton ID="LinkButton_Modify" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Modify" OnClick="LinkButton_Modify_Click"></asp:LinkButton></li>
                                                        <li><asp:LinkButton ID="LinkButton_Delete" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Delete" OnClick="LinkButton_Delete_Click"></asp:LinkButton></li>
                                                    </ul>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <asp:LinkButton ID="lnkweekPre" runat="server" OnClick="lnkweekPre_Click">
                                    <i class="fa fa-angle-left" aria-hidden="true"></i>
                                    Previous
                            </asp:LinkButton><%--CssClass="buttonhyperlink Width="125px""--%>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <asp:LinkButton ID="lnkweekNxt" Style="float: right;" runat="server" OnClick="lnkweekNxt_Click">
                                    Next
                                    <i class="fa fa-angle-right" aria-hidden="true"></i>
                            </asp:LinkButton><%--CssClass="buttonhyperlink Width="100px""--%>
                        </div>
                    </div>
                </div>
            </div>
            
                       <center>

                        <asp:Chart ID="Chart2" runat="server" Visible="false" Width="550px">
                             <Legends>
                                <asp:Legend Alignment="Center" Docking="Left"  IsTextAutoFit="false" TitleAlignment="Far" TableStyle="Tall" Name="Default" LegendStyle="Column"></asp:Legend>
                            </Legends>
                            <Series>
                                <asp:Series Name="Series1">
                                </asp:Series>
                            </Series>
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1">
                                </asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>
                        </center>
        </div>
    </div>
</asp:Content>

