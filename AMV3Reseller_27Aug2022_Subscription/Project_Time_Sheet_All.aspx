<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Project_Time_Sheet_All.aspx.cs" Inherits="Project_Time_Sheet_All" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

     <style>
        .swal2-container:not(.swal2-top):not(.swal2-top-start):not(.swal2-top-end):not(.swal2-top-left):not(.swal2-top-right):not(.swal2-center-start):not(.swal2-center-end):not(.swal2-center-left):not(.swal2-center-right):not(.swal2-bottom):not(.swal2-bottom-start):not(.swal2-bottom-end):not(.swal2-bottom-left):not(.swal2-bottom-right):not(.swal2-grow-fullscreen)>.swal2-modal {
            margin: auto;
        }
    </style>

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
            /*background-color: #ca2222;*/
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
            /*background-color: #2ab934;*/
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
            /*document.getElementById("myText").value = "Johnny Bravo";*/
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
            position: fixed !important;
            z-index: 10002 !important;
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
            overflow-y: scroll;
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

        /*.buttonhyperlink {
            text-align: center;
            padding-top:10px;
            font-family: sans-serif;
            font-size: medium;
        }
        .buttonhyperlink:hover {            
            background-color: #607DF2;
            border-radius: 1px;
            border: none;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top:-1px;
            font-family: sans-serif;
            font-size: medium;
        }*/

        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
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

        .swal2-container {
            overflow-y: auto;
            z-index: 300000 !important;
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
            padding: 2px 25px;
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
            padding: 2px 25px;
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
            width:90%;
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
            width:90%;
        }
    </style>

    <script>
        $(document).ready(function () {
            $("#<%=ddlemp.ClientID%>").select2({
                placeholder: "Select Employee",
                allowClear: true
            });
        });
    </script>

    <script>
        $(document).ready(function () {
            $('#GVChildlist').DataTable(
                {
                });
        });
    </script>
    <script>
        $(document).ready(function () {
            $("#btnSubmit").click(function () {
                var AddBreakEntity = {};
                AddBreakEntity.BreakStart = $('#txtBreakStartTime').val();
                AddBreakEntity.BreakEnd = $('#txtBreakEndTime').val();
                $.ajax({
                    url: 'breakadd.asmx/AddBreak',
                    method: 'post',
                    data: '{Abe: ' + JSON.stringify(AddBreakEntity) + '}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function () {
                        swal('Error!', 'Select Employee', 'error');
                        console.log(data);
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            });

            $('.chkSelectAll').on('change', function () {
                $('.chkSelect')
                    .children()
                    .not(":disabled")
                    .prop('checked', $(this).prop('checked'));
            });
        });
    </script>
    <script>
        $("#chkworked").change(function () {
            var id = $('#chkworked').val();
            alert(id.to());
        });
    </script>
    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Always" ChildrenAsTriggers="true">
        <ContentTemplate>

            <asp:LinkButton ID="LinkButton2" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupConfirmSingal" runat="server"
                PopupControlID="pnlConfirmSingal" TargetControlID="LinkButton2" BackgroundCssClass="modalBackground"
                CancelControlID="btnHide2">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlConfirmSingal" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Confirm</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">For the selected date, time sheet has not been entered,If approved then status will be changed to Absent. Please confirm?</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button id="btnHide2"></button>
                    <div class="row" style="margin-top: 20px;">
                        <div class="col-md-6" style="text-align: center;background-color:white">
                            <asp:HiddenField ID="hfdRIndex" runat="server" />
                            <asp:LinkButton ID="lnkbtnYesSingal" runat="server" Text="Yes" CssClass="btn btn-primary" OnClick="lnkbtnYesSingal_Click" />
                        </div>
                        <div class="col-md-6" style="text-align: center">
                            <asp:LinkButton ID="lnkbtnNoSingal" runat="server" Text="No" CssClass="btn btn-primary" OnClick="lnkbtnNoSingal_Click" />
                        </div>
                    </div></div><div class="swal2-footer" style="display: none;"></div></div>
                <%--<div class="header">
                    <label style="width: 300px; color: white">Confirm</label>

                    <div class="pull-right" style="margin-right: 5px;">
                        <button id="btnHide2" class="btn-sm btn-danger">
                            <i class="fa fa-window-close" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>--%>
                 </asp:Panel>

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
                        <button id="btnHide" class="btn-sm btn-danger">
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
                            <asp:HiddenField ID="hfdBreakId" runat="server" />
                        </div>
                        <div class="col-md-5">
                            <label>Break End Time</label>
                            <asp:TextBox ID="txtBreakEndTime" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-md-2">
                            <label>&nbsp;</label>
                            <%--<label style="visibility: hidden;">Break End Time</label>--%>
                            <asp:Button ID="btnAddBreak" runat="server" CssClass="buttonhyperlink222" Text="Add"
                                OnClick="btnAddBreak_Click"  />

                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <div class="col-md-12">

                            <asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnRowCommand="GvPopUpEmpList_RowCommand"
                                OnPreRender="GvPopUpEmpList_PreRender">
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
                                            <asp:LinkButton ID="lnkdelete" runat="server" Text="Delete" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("BreakID") %>' CommandName="DeleteRow"></asp:LinkButton>

                                            <%--OnClick="lnkdelete_Click"--%>
                                            <asp:LinkButton ID="lnkEdit" Visible="false" runat="server" Text="Edit" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("BreakID") %>' CommandName="EditRow"></asp:LinkButton>

                                            <%--OnClick="lnkEdit_Click"--%>
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



            <asp:LinkButton ID="LinkButton1" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupConfirm" runat="server"
                PopupControlID="pnlConfirm" TargetControlID="LinkButton1" BackgroundCssClass="modalBackground"
                CancelControlID="btnHide1">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlConfirm" runat="server" CssClass="modalPopup" Style="display: none">
                <div class="header">
                    <label style="width: 300px; color: white">Please Confirm ?</label>

                    <div class="pull-right" style="margin-right: 5px;">
                        <button id="btnHide1" class="btn-sm btn-danger">
                            <i class="fa fa-window-close" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>
                <div class="body">
                    <div style="text-align: center;">
                        <asp:Label ID="lblmsg" runat="server"></asp:Label>
                    </div>


                    <div class="row" style="margin-top: 20px;">
                        <div class="col-md-6" style="text-align: center">
                            <asp:LinkButton ID="btnYes" runat="server" Text="Yes" CssClass="btn2 btn-primary" OnClick="btnYes_Click" />
                        </div>
                        <div class="col-md-6" style="text-align: center">
                            <asp:LinkButton ID="btnNo" runat="server" Text="No" CssClass="btn2 btn-primary" OnClick="btnNo_Click" />
                        </div>
                    </div>
                </div>

            </asp:Panel>


            
            <%--<asp:HiddenField ID="hfdRIndex" runat="server" />--%>
                <%--<div class="body">
                    <div style="text-align: center;">
                        <asp:Label ID="Label3" Visible="false" runat="server"></asp:Label>
                        <asp:Label Text="For the selected date, time sheet has not been entered," runat="server" /><br />
                        <asp:Label Text="If approved then status will be changed to Absent. Please confirm?" runat="server" />
                    </div>
                    <div class="row" style="margin-top: 20px;">
                        <div class="col-md-6" style="text-align: center">
                            <asp:HiddenField ID="hfdRIndex" runat="server" />
                            <asp:LinkButton ID="lnkbtnYesSingal" runat="server" Text="Yes" CssClass="btn btn-primary" OnClick="lnkbtnYesSingal_Click" />
                        </div>
                        <div class="col-md-6" style="text-align: center">
                            <asp:LinkButton ID="lnkbtnNoSingal" runat="server" Text="No" CssClass="btn btn-primary" OnClick="lnkbtnNoSingal_Click" />
                        </div>
                    </div>
                </div>--%>

           

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAddBreak" />
            <asp:AsyncPostBackTrigger ControlID="btnYes" />
            <asp:AsyncPostBackTrigger ControlID="btnNo" />
            <asp:AsyncPostBackTrigger ControlID="GvPopUpEmpList" EventName="RowCommand" />
            <%--<asp:PostBackTrigger ControlID="GvPopUpEmpList" />--%>
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
                        <asp:LinkButton ID="lnkAddEmp" Visible="false" ClientIDMode="Static" runat="server" Width="50%" CssClass="btn btn-primary"
                            Text="Add New Employee" OnClick="lnkAddEmp_Click"></asp:LinkButton>
                        <div style="margin-top: 20px;">
                            <asp:HyperLink ID="hypProjectTime" runat="server" Text="Enter/View Project Time Sheet" Visible="false"></asp:HyperLink>
                        </div>
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
                <br /><br />
                <p>
                    <asp:HyperLink ID="HyperLink_Back" runat="server" Text="Back"></asp:HyperLink>
                </p>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Card - " runat="server" /><asp:Label ID="lblbid" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>
            </div>
            <div class="col-md-3" style="margin-top: 12px; text-align: right;">
                
            </div>
        </div>
    </div>

    

                            <asp:LinkButton ID="LinkButton3" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupEmployee" runat="server"
                PopupControlID="pnlEmployee" TargetControlID="LinkButton3" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideEmployee">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlEmployee" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Select Employee</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideEmployee"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton4" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupApproveOrReject" runat="server"
                PopupControlID="pnlApproveOrReject" TargetControlID="LinkButton4" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideApproveOrReject">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlApproveOrReject" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please select the Date to Approve or Reject time sheet</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideApproveOrReject"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton5" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStartDate" runat="server"
                PopupControlID="pnlStartDate" TargetControlID="LinkButton5" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStartDate">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStartDate" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Enter Start Date</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideStartDate"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
             <asp:LinkButton ID="LinkButton6" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupEndDate" runat="server"
                PopupControlID="pnlEndDate" TargetControlID="LinkButton6" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideEndDate">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlEndDate" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Enter End Date</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideEndDate"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
                             <asp:LinkButton ID="LinkButton7" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStarDateGreaterEnd" runat="server"
                PopupControlID="pnlStarDateGreaterEnd" TargetControlID="LinkButton7" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStarDateGreaterEnd">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStarDateGreaterEnd" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Start Date is Greater than End Date</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideStarDateGreaterEnd"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
                            <asp:LinkButton ID="LinkButton8" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStartTimeMust" runat="server"
                PopupControlID="pnlStartTimeMust" TargetControlID="LinkButton8" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStartTimeMust">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStartTimeMust" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Start Time and End Time Must be within 24Hrs</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideStartTimeMust"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
                            <asp:LinkButton ID="LinkButton9" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupWorked" runat="server"
                PopupControlID="pnlWorked" TargetControlID="LinkButton9" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideWorked">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlWorked" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Select Worked as Yes</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideWorked"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>
                            <asp:LinkButton ID="LinkButton10" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupTimesheetalready" runat="server"
                PopupControlID="pnlTimesheetalready" TargetControlID="LinkButton10" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideTimesheetalready">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlTimesheetalready" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Timesheet already exists, so timesheet is not Copied</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideTimesheetalready"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>

                            <asp:LinkButton ID="LinkButton11" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupSelectProject" runat="server"
                PopupControlID="pnlSelectProject" TargetControlID="LinkButton11" BackgroundCssClass="modalBackground"
                CancelControlID="btnSelectProject">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlSelectProject" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Select Project Name</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnSelectProject"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>

                            <asp:LinkButton ID="LinkButton12" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupTimesheetEnter" runat="server"
                PopupControlID="pnlTimesheetEnter" TargetControlID="LinkButton12" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideTimesheetenter">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlTimesheetEnter" runat="server" CssClass="modalPopup" Style="display: none">
                <div aria-labelledby="swal2-title" aria-describedby="swal2-content" class="swal2-popup swal2-modal swal2-show" tabindex="-1" role="dialog" aria-live="assertive" aria-modal="true" style="display: flex;"><div class="swal2-header"><ul class="swal2-progresssteps" style="display: none;"></ul><div class="swal2-icon swal2-error" style="display: none;"><span class="swal2-x-mark"><span class="swal2-x-mark-line-left"></span><span class="swal2-x-mark-line-right"></span></span></div><div class="swal2-icon swal2-question" style="display: none;"><span class="swal2-icon-text">?</span></div><div class="swal2-icon swal2-warning swal2-animate-warning-icon" style="display: flex;"><span class="swal2-icon-text">!</span></div><div class="swal2-icon swal2-info" style="display: none;"><span class="swal2-icon-text">i</span></div><div class="swal2-icon swal2-success" style="display: none;"><div class="swal2-success-circular-line-left" style="background-color: rgb(255, 255, 255);"></div><span class="swal2-success-line-tip"></span> <span class="swal2-success-line-long"></span><div class="swal2-success-ring"></div> <div class="swal2-success-fix" style="background-color: rgb(255, 255, 255);"></div><div class="swal2-success-circular-line-right" style="background-color: rgb(255, 255, 255);"></div></div><img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Warning!</h2><button type="button" class="swal2-close" style="display: none;">×</button></div><div class="swal2-content"><div id="swal2-content" style="display: block;">Please Enter Hours before clicking Copy Timesheet</div><input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;"><input type="range"><output></output></div><select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div><label for="swal2-checkbox" class="swal2-checkbox" style="display: none;"><input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div></div><div class="swal2-actions" style="display: flex;"><button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button><button id="btnHideTimesheetenter"
                        class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">Ok</button></div><div class="swal2-footer" style="display: none;"></div></div>
            </asp:Panel>

    <div class="panel panel-default">
        <div class="panel-body">
            <asp:Label ID="lblsms" Text="user" runat="server" Visible="false"></asp:Label>
            <div class="row">

                <div class="col-md-2" style="display:none;">
                    <div class="form-group">
                        <label>Select Business</label>
                        <asp:DropDownList
                            ID="Ddl_Business"
                            runat="server"
                            AppendDataBoundItems="true"
                            CssClass="form-control js-example-basic-single" 
                            OnSelectedIndexChanged="Ddl_Business_SelectedIndexChanged"                            
                            AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-md-3">
                    <div class="form-group">
                        <label>Select User</label>
                        <asp:DropDownList ID="ddlemp" Width="100%" runat="server" AppendDataBoundItems="true" CssClass="form-control js-example-basic-single"
                            OnSelectedIndexChanged="ddlemp_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                        <asp:Label ID="lblrate" runat="server"></asp:Label>
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

                        .btnprvmargin {
                            margin-left: -50px;
                        }
                    }
                </style>


                <div class="col-md-1 btnprvmargin">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton ID="btnprv" ClientIDMode="Static" runat="server" CssClass="buttonhyperlink"
                            OnClick="btnprv_Click">
                            <i class="fa fa-angle-left" aria-hidden="true"></i> 
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="form-group">
                        <label>Date Range</label>
                        <asp:TextBox ID="txtmonth" runat="server" TextMode="Date" OnTextChanged="txtmonth_TextChanged"
                            AutoPostBack="true" required="required" CssClass="form-control"></asp:TextBox>
                        <asp:Label ID="lblweekDayName" Visible="false" runat="server" Font-Size="14"></asp:Label>
                    </div>
                </div>
                <div class="col-md-1">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton ID="btnnext" runat="server" CssClass="buttonhyperlink"
                            OnClick="btnnext_Click">
                                <i class="fa fa-angle-right" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </div>
                </div>

                <div class="col-md-3" style="margin-left:-50px;display:none;">
                    <div class="form-group">
                        <%--<label for="title">Action</label>--%>
                        <label for="title">Approve/Reject Time Sheet</label>
                        <asp:DropDownList
                            ID="ddlAction"
                            runat="server"
                            CssClass="form-control"
                            OnSelectedIndexChanged="ddlAction_SelectedIndexChanged"
                            AutoPostBack="True">
                            <asp:ListItem Selected="True" Value="">Select</asp:ListItem>
                            <asp:ListItem Value="A">Approve</asp:ListItem>
                            <asp:ListItem Value="R">Reject</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                 <div class="col-md-2">
                            <div class="form-group">
                                <%--<label>&nbsp;</label>--%>
                                <asp:LinkButton
                                    ID="LinkButton_CopyNextWeek"
                                    runat="server" Text="Copy Next Day"                                    
                                    OnClick="LinkButton_CopyNextWeek_Click"><%--CssClass="buttonhyperlink333"--%>
                                </asp:LinkButton>
                            </div>
                        </div>
            </div>


            <asp:LinkButton ID="lnkApproved" Visible="false" ToolTip="Approved" Width="100%" runat="server" ClientIDMode="Static"
                Text="Approve" OnClick="lnkApproved_Click" CssClass="buttonhyperlink"> </asp:LinkButton>
            <asp:LinkButton ID="lnkReject" Visible="false" ToolTip="Reject" runat="server" Text="Reject" Width="100%"
                OnClick="lnkReject_Click" CssClass="buttonhyperlink"></asp:LinkButton>


            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x: auto;">
                        <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <asp:GridView
                                    ID="GVChildlist"
                                    runat="server"
                                    ClientIDMode="Static"
                                    CssClass="table FS14"
                                    AutoGenerateColumns="False"
                                    CellPadding="1"
                                    ForeColor="#333333"
                                    GridLines="None"
                                    EmptyDataText="No Records Found"
                                    HeaderStyle-BackColor="#607DF2"
                                    HeaderStyle-ForeColor="White"
                                    ShowFooter="true"
                                    OnRowCommand="GVChildlist_RowCommand"
                                    OnRowDataBound="GVChildlist_RowDataBound"
                                    OnPreRender="GVChildlist_PreRender">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" HeaderStyle-Width="25px">
                                            <HeaderTemplate>
                                                <input
                                                    type="checkbox"
                                                    class="chkSelectAll" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox
                                                    runat="server"
                                                    ID="chkApproved"
                                                    CssClass="chkSelect" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Project Name">
                                            <ItemTemplate>
                                                <asp:DropDownList
                                                    ID="ddlProjectName"
                                                    runat="server"
                                                    Width="170px"
                                                    AppendDataBoundItems="true"
                                                    CssClass="form-control mselect"
                                                    OnSelectedIndexChanged="ddlProjectName_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Text="Please Select Project" Value="">
                                                    </asp:ListItem>
                                                </asp:DropDownList>
                                                
                                                <asp:HiddenField
                                                    ID="hfdprojId"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>' />
                                                <asp:HiddenField
                                                    ID="lblDate"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "SelDate")%>' />
                                                <asp:HiddenField
                                                    ID="hfdweekno"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "weekno")%>' />
                                                <asp:HiddenField
                                                    ID="hfdweekdayNo"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "weekdayno")%>' />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:LinkButton
                                                    ID="lnkAddMore"
                                                    runat="server"
                                                    OnClick="lnkAddMore_Click">
                                                    <i class="fa fa-plus-circle" 
                                                        aria-hidden="true"></i> Add More Project
                                                </asp:LinkButton>
                                                <br />
                                                <asp:LinkButton
                                                    ID="lnkAddRegularHours"
                                                    runat="server"
                                                    OnClick="lnkAddRegularHours_Click">
                                                    <i class="fa fa-plus-circle" 
                                                        aria-hidden="true"></i> Add Regular Hours
                                                </asp:LinkButton>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Worked">
                                            <ItemTemplate>
                                                <asp:DropDownList runat="server" Width="70px" CssClass="form-control xs-mselect"
                                                    ID="ddstatus">
                                                    <asp:ListItem Value="SELECT" Text="SELECT"></asp:ListItem>
                                                    <asp:ListItem Value="No" Text="No"></asp:ListItem>
                                                    <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:HiddenField
                                                    ID="hfdWorked"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "Worked")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Start Time">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtRegularStartTime"
                                                    TextMode="DateTimeLocal"
                                                    CssClass="form-control"
                                                    ClientIDMode="Static"
                                                    Width="230px"
                                                    runat="server"
                                                    Text='<%#Bind("ReqularStart","{0:yyyy-MM-ddTHH:mm}")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="End Time">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtRegularEndTime"
                                                    TextMode="DateTimeLocal"
                                                    CssClass="form-control"
                                                    ClientIDMode="Static"
                                                    Width="230px"
                                                    runat="server"
                                                    Text='<%#Bind("Regularend","{0:yyyy-MM-ddTHH:mm}")%>' />
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
                                        <asp:TemplateField HeaderText="Total Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblTotal"
                                                    runat="server"
                                                    Text='<%#DataBinder.Eval(Container.DataItem, "Regular")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Add/View Break">
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkAddBreak"
                                                    runat="server"
                                                    CommandName="AddBreak"
                                                    Text="Add Break"
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Break Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblBreakTotal"
                                                    runat="server"
                                                    Text='<%#DataBinder.Eval(Container.DataItem, "BreakTotal")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Net Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblnetTotal"
                                                    runat="server"
                                                    Text='<%#DataBinder.Eval(Container.DataItem, "Total")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblStatus"
                                                    runat="server"></asp:Label>
                                                <asp:HiddenField
                                                    ID="hfdStatus"
                                                    runat="server"
                                                    Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Action" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkSave"
                                                    runat="server"
                                                    CommandName="Save"
                                                    Text="Save"
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                                <asp:LinkButton
                                                    ID="LinkButton_Edit_Timesheet"
                                                    runat="server"
                                                    Text=" / Edit"
                                                    Visible="false" OnClick="LinkButton_Edit_Timesheet_Click"/>
                                                <asp:LinkButton
                                                    ID="lnksubmit"
                                                    runat="server"
                                                    CommandName="Submit"
                                                    Text="Submit"
                                                    Visible="false"
                                                    CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Edit Status">
                                            <ItemTemplate>
                                                <asp:DropDownList
                                                    runat="server"
                                                    ID="ddapproval"
                                                    Width="105px"
                                                    CssClass="form-control"
                                                    OnSelectedIndexChanged="ddapproval_SelectedIndexChanged"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Value="SELECT" Text="SELECT"></asp:ListItem>
                                                    <asp:ListItem Value="Approved" Text="Approved"></asp:ListItem>
                                                    <asp:ListItem Value="Reject" Text="Reject"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Timesheet Status" Visible="true"> <%--HeaderText="Edit Status"--%>
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkGvApp"
                                                    runat="server"
                                                    Text="Approve/"
                                                    OnClick="lnkGvApp_Click" /> <%--Text="Approved/"--%>
                                                <asp:LinkButton
                                                    ID="lnkGvReject"
                                                    runat="server"
                                                    Text="Reject"
                                                    OnClick="lnkGvReject_Click" />
                                                <asp:LinkButton
                                                    ID="lnlGvAbsent"
                                                    runat="server"
                                                    Text="Absent"                                                
                                                    Visible="false" /> <%--OnClick="lnlGvAbsent_Click"--%>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        
                                    </Columns>
                                </asp:GridView>

                                <asp:HyperLink ID="HyperLink_Project_Employee_Regular_Hours" runat="server" Visible="false" Text="Enter Regular Timesheet Hours"></asp:HyperLink>
                                <asp:LinkButton ID="LinkButton_Project_Employee_Regular_Hours" runat="server" ClientIDMode="Static" Text="Enter Regular Timesheet Hours" Visible="false"></asp:LinkButton>
                                <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Project_Time_Sheet_All_Regular_Hours.aspx" Text="Enter Regular Timesheet Hours" Visible="false"></asp:HyperLink>
                                <h5 runat="server" id="h3_regular_hours_heading" visible="false">Enter Regular Timesheet Hours</h5>
                                <asp:GridView
                                    ID="GridView1" Visible="false"
                                    runat="server" AutoGenerateColumns="False"
                                    ClientIDMode="Static"
                                    CssClass="table FS14"
                                    CellPadding="1"
                                    ForeColor="#333333"
                                    GridLines="None"
                                    EmptyDataText="No Records Found"
                                    HeaderStyle-BackColor="#607DF2"
                                    HeaderStyle-ForeColor="White">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Date" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblDate"
                                                    Width="100px"
                                                    runat="server" TextMode="Date" Text='<%# Eval("Text1") %>'>
                                                </asp:Label>
                                                <asp:HiddenField
                                                    ID="hfdweekno"
                                                    runat="server" />
                                                <asp:HiddenField
                                                    ID="hfdweekdayNo"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Day" Visible="false">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblDay"
                                                    runat="server" Text='<%# Eval("Text2") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Worked">
                                            <ItemTemplate>
                                                <asp:DropDownList
                                                    runat="server"
                                                    ID="ddstatus" CssClass="form-control xs-mselect"
                                                    AutoPostBack="true">
                                                    <asp:ListItem Value="SELECT" Text="SELECT"></asp:ListItem>
                                                    <asp:ListItem Value="No" Text="No"></asp:ListItem>
                                                    <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                                <asp:HiddenField
                                                    ID="hfdWorked"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Start Time">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtRegularStartTime"
                                                    TextMode="DateTimeLocal"
                                                    CssClass="form-control"
                                                    ClientIDMode="Static"
                                                    Width="220px"
                                                    runat="server" Text='<%# Eval("Text3") %>'>
                                                </asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="End Time">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtRegularEndTime"
                                                    TextMode="DateTimeLocal"
                                                    CssClass="form-control"
                                                    ClientIDMode="Static"
                                                    Width="220px"
                                                    runat="server" Text='<%# Eval("Text4") %>' />
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
                                                <asp:TextBox
                                                    ID="txtOver15x"
                                                    Width="50px"
                                                    TextMode="Number"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Over 2x" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtOver2x"
                                                    TextMode="Number"
                                                    Width="50px"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Holiday</br>Others" Visible="false">
                                            <ItemTemplate>
                                                <asp:TextBox
                                                    ID="txtholiday"
                                                    Width="50px"
                                                    TextMode="Number"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblTotal"
                                                    runat="server" Text='<%# Eval("Text5") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Add/View Break">
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkAddBreak"
                                                    runat="server"
                                                    CommandName="AddBreak"
                                                    Text="Add Break" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Break Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblBreakTotal"
                                                    runat="server" Text='<%# Eval("Text6") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Net Hours">
                                            <ItemTemplate>
                                                <asp:Label
                                                    ID="lblnetTotal"
                                                    runat="server" Text='<%# Eval("Text7") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" Text='<%# Eval("Text8") %>' runat="server"></asp:Label>
                                                <asp:HiddenField
                                                    ID="hfdStatus"
                                                    runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkSave"
                                                    runat="server"
                                                    CommandName="Save"
                                                    Text="Save" />
                                                <asp:LinkButton
                                                    ID="lnksubmit"
                                                    runat="server"
                                                    CommandName="Submit"
                                                    Text="Submit"
                                                    Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edit Status" Visible="false">
                                            <ItemTemplate>
                                                <asp:LinkButton
                                                    ID="lnkGvApp"
                                                    runat="server"
                                                    Text="Approved/" />
                                                <asp:LinkButton
                                                    ID="lnkGvReject"
                                                    runat="server"
                                                    Text="Reject" />
                                                <asp:LinkButton
                                                    ID="lnlGvAbsent"
                                                    runat="server"
                                                    Text="Absent"
                                                    Visible="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Edit Status">
                                            <ItemTemplate>
                                                <asp:DropDownList
                                                    runat="server" CssClass="form-control xs-mselect"
                                                    ID="ddapproval">
                                                    <asp:ListItem Value="SELECT" Text="SELECT"></asp:ListItem>
                                                    <asp:ListItem Value="Approved" Text="Approved"></asp:ListItem>
                                                    <asp:ListItem Value="Reject" Text="Reject"></asp:ListItem>
                                                </asp:DropDownList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                
                                
                            </ContentTemplate>
                            
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>

        </div>
    </div>

                            </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlAction" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>

