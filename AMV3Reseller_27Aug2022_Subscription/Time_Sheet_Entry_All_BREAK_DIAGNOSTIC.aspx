<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" ViewStateMode="Enabled" AutoEventWireup="true" CodeFile="Time_Sheet_Entry_All_BREAK_DIAGNOSTIC.aspx.cs" Inherits="Time_Sheet_Entry_All" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>

    <%--firoz--%>
    <link rel="stylesheet" href="Content/bootstrap.min.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.4.0/css/bootstrap-datepicker3.standalone.css" />
    <script type="text/javascript" src="Scripts/jquery-1.11.1.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="~/js/selectall.js"></script>
    <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datepicker/1.6.1/js/bootstrap-datepicker.min.js"></script>
    <link rel="stylesheet" type="text/css" href="Jquery/jquery.datetimepicker.css"/ >
    <script src="Jquery/jquery.js"></script>
    <script src="Jquery/build/jquery.datetimepicker.full.min.js"></script>
   
    <script type="text/javascript">
        function SelectAll(headerCheckBox) {

            
            var gridView = headerCheckBox.parentNode.parentNode.parentNode;
            
            // 2. Get all input elements inside the GridView
            var inputs = gridView.getElementsByTagName("input");

            // 3. Loop and set checked state for all row checkboxes
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox" && inputs[i] != headerCheckBox) {
                    inputs[i].checked = headerCheckBox.checked;
                }
            }
        }
        
        function checkUncheck(row) {
            var hiddenfield = $(row).closest('tr').find('input[type="hidden"]');
            hiddenfield.value = "1234";
            //alert(hiddenfield.value);
            //alert(row.checked);
            // Reference the GridView table (usually 3 levels up: cell -> row -> table)
            var grid = document.getElementById('<%= GVChildlist.ClientID %>');
            // Standard GridView renders the header in row index 0
            var headerCheckBox1 = grid.rows[0].cells[0].getElementsByTagName("input")[0];
            
            //var hiddenField = document.getElementById(controlId);
            
            // Find all elements within the target cell (e.g., cell 0)
           
            if (!row.checked) {

                if (headerCheckBox1.type == "checkbox") {
                    headerCheckBox1.checked = row.checked;
                      hiddenfield.value = null;
    //               alert(hiddenfield.value)
                }
              
            }
                else {
                    hiddenfield.value = "123";
                  //  alert(hiddenfield.value)
                   // $(this).closest('tr').find('input[type="hidden"]').val("123");
                }
         




            
            
        }
            
    </script>

    <script src="Scripts/AnyminuteTimeInputs.js"></script>
    


    <style>
        .swal2-container:not(.swal2-top):not(.swal2-top-start):not(.swal2-top-end):not(.swal2-top-left):not(.swal2-top-right):not(.swal2-center-start):not(.swal2-center-end):not(.swal2-center-left):not(.swal2-center-right):not(.swal2-bottom):not(.swal2-bottom-start):not(.swal2-bottom-end):not(.swal2-bottom-left):not(.swal2-bottom-right):not(.swal2-grow-fullscreen) > .swal2-modal {
            margin: auto;
        }
    </style>

    <style type="text/css">
        .error {
            color: Red;
            display: none;
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
            /*document .getElementById("myText").value = "Johnny Bravo";*/
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
            margin-top: 0px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink222:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            /*padding: 2px 25px;*/
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: 0px;
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

        .swal2-container {
            overflow-y: auto;
            z-index: 300000 !important;
        }
    </style>

    <style>
        .modalBackground_ConfirmEmpSelection {
            background-color: Black;
            /*filter: alpha(opacity=60);*/
            opacity: 0.6;
        }
        .modalPopup_ConfirmEmpSelection {
            background-color: #FFFFFF;
            width: auto;
            height: auto;
            border: 3px solid #1269db;
            padding: 5px;
            border-radius: 5px;
            position: fixed !important;
            z-index: 10002 !important;
        }

        /*.modalPopup_ConfirmEmpSelection .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 5px;
            margin-bottom: 10px;
        }*/

        .modalPopup_ConfirmEmpSelection .body {
            min-height: 50px;
            line-height: 30px;
            font-weight: bold;
            margin-bottom: 5px;
            color: black !important;
            /*overflow-y: scroll;*/
            height: inherit;
            /*width: auto;*/
            /*padding: 10px;*/
        }
    </style>
     
    
<script type="text/javascript">

    $(document).ready(function () {
        $("#birthdate").datepicker({
            autoclose: true,
            format: 'yyyy-mm-dd',
            todayHighlight: true,
            clearBtn: true,
            orientation: 'bottom'
        });
    });


</script>
   
<style type="text/css">
.ui-datepicker { font-size:8pt !important}
</style>

     <script>
         $(document).ready(function () {
             // Use the native date picker on txtmonth.
         });
     </script>

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
            $('#GVChildlist').DataTable({});
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
        });
            
    </script>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <link href="css/ButtonAsLinkbutton.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdatePanel runat="server" ID="up2" UpdateMode="Always" ChildrenAsTriggers="true">
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
                                GridLines="None"  OnRowCommand="GvPopUpEmpList_RowCommand"
                                OnPreRender="GvPopUpEmpList_PreRender"> <%--EmptyDataText="No Records Found"--%>
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <%--<HeaderStyle CssClass="" />--%>


                                <%--<asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                    CssClass="table table-striped table-bordered nowrap"
                    EmptyDataText="No Records Found"
                    AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333"
                    GridLines="None" OnPreRender="GvPopUpEmpList_PreRender"
                    HeaderStyle-BackColor="#007bff" HeaderStyle-ForeColor="White" >--%>


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
                                            <asp:LinkButton ID="lnkEditBreak" runat="server" Text="Edit" OnClick="lnkEditBreak_Click" CausesValidation="false" /> | <asp:LinkButton ID="lnkdelete" runat="server" Text="Delete" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("BreakID") % CausesValidation="false">' CommandName="DeleteRow"></asp:LinkButton>

                                            <%--OnClick="lnkdelete_Click"--%>
                                            <asp:LinkButton ID="lnkEdit" Visible="false" runat="server" Text="Edit" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("BreakID") %>' CommandName="EditRow"></asp:LinkButton>

                                            <%--OnClick="lnkEdit_Click"--%>
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
                            <asp:LinkButton ID="btnYes" runat="server" Text="Yes" CssClass="btn2 btn-primary" OnClick="btnYes_Click"  />
                        </div>
                        <div class="col-md-6" style="text-align: center">
                            <asp:LinkButton ID="btnNo" runat="server" Text="No" CssClass="btn2 btn-primary" OnClick="btnNo_Click" />
                        </div>
                    </div>
                </div>

            </asp:Panel>

            <asp:LinkButton ID="LinkButton2" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupConfirmSingal" runat="server"
                PopupControlID="pnlConfirmSingal" TargetControlID="LinkButton2" BackgroundCssClass="modalBackground"
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
                        <button id="btnHide2"></button>
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

            <asp:LinkButton ID="LinkButton14" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupPnlConfirmEmpSelection" runat="server"
                PopupControlID="PnlConfirmEmpSelection" TargetControlID="LinkButton14" BackgroundCssClass="modalBackground_ConfirmEmpSelection"
                CancelControlID="btnHide14">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="PnlConfirmEmpSelection" runat="server" CssClass="modalPopup_ConfirmEmpSelection" Style="display: none">
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
                        <img class="swal2-image" style="display: none;"><h2 class="swal2-title" id="swal2-title">Info!</h2>
                        <button type="button" class="swal2-close" style="display: none;">×</button></div>
                    <div class="swal2-content">
                        <%--<div id="swal2-content" style="display: block;">For the selected date, time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?</div>--%>
                        <div id="swal2-content" style="display: block;">
                                <asp:label ID="label_ConfirmEmpSelection" text="" runat="server" />
                        </div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button id="btnHide14"></button>
                        <div class="row" style="margin-top: 20px;">
                            <div class="col-md-6" style="text-align: center; background-color: white">
                                <asp:HiddenField ID="HiddenField_confirm_pop_up_count" runat="server" />
                                <asp:LinkButton ID="LinkButton_btnYesSingal" runat="server" Text="Yes" CssClass="btn btn-primary" OnClick="LinkButton_btnYesSingal_Click" />
                            </div>
                            <div class="col-md-6" style="text-align: center;">
                                <asp:LinkButton ID="LinkButton_btnNoSingal" runat="server" Text="No" CssClass="btn btn-primary" OnClick="LinkButton_btnNoSingal_Click" />
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
                        <button id="btnHide13" class="btn-sm btn-danger">
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
                                ClientIDMode="Static" Width="200px" runat="server"></asp:TextBox>
                            <asp:HiddenField ID="HiddenField13" runat="server" />
                        </div>
                        <div class="col-md-5">
                            <label>End Date / Time</label>
                            <asp:TextBox ID="TextBox2" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="200px" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-md-2" style="margin-left:-5px;">
                            <label>&nbsp;</label>
                            <asp:LinkButton ID="Button_Save_Action_popup" runat="server" CssClass="buttonhyperlink222" Text="Save" OnClick="Button_Save_Action_popup_Click"/>
                        </div>
                    </div>
                </div>

            </asp:Panel>

        </ContentTemplate>

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
                        <%--<h4><asp:Label ID="lblbid" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label></h4>
                        <asp:HyperLink ID="hypProjectTime" runat="server" Text="Enter/View Project Time Sheet" Visible="false"></asp:HyperLink>--%>
                    </div>
                </div>
                <div class="col-md-4">
                    <asp:HyperLink ID="hyBack" runat="server" NavigateUrl="~/Home_Page.aspx" CssClass="buttonhyperlink"
                        Style="margin-top: 12px; float: right;">Home
                    </asp:HyperLink>
                </div>
                <div class="col-md-3 col-lg-3 col-xs-12 col-sm-12" style="display:none;">
                    <asp:LinkButton ID="lnkAddEmp" ClientIDMode="Static" runat="server" CssClass="btn btn-primary"
                        Text="Add New Employee" OnClick="lnkAddEmp_Click"></asp:LinkButton>

                    <div style="margin-top: 20px;">
                        
                    </div>
                    <script>
            $('#lnkAddEmp').click(function () {
                document.cookie = "tabid=#home";
            });
                    </script>
                </div>
                <div style="width: 80%; text-align: center;">
                    <asp:Label ID="Label1" runat="server" Font-Size="Large"></asp:Label>
                </div>
            </div>
        </div>
    </div>


    <div style="position: relative;display:none;">
        <div class="row">
            <div class="col-md-3" style="margin-top: 9px;">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
                <br /><br />
                <p>
                    <asp:HyperLink ID="hypProjectTime_" runat="server" Text="Enter/View Project Time Sheet" Visible="false"></asp:HyperLink>
                </p>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:22px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Sheet Entry - " runat="server" /><asp:Label ID="lblbid_" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>
            </div>
            <div class="col-md-3" style="margin-top: 12px; text-align: right;">
                
            </div>
        </div>
    </div>

    <div style="position: relative; margin-top:-10px;">
        <div class="row">
            <div class="col-md-3">
                <a href="Home_Page.aspx" id="A2" runat="server" style="font-size:large;">
                    Home
                </a>
                <%--<br /><br />--%>
                <p>
                    <%--<asp:HyperLink ID="hypProjectTime" runat="server" Text="Enter/View Project Time Sheet" Visible="false"></asp:HyperLink>--%>
                </p>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <%--<asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Sheet Entry - " runat="server" /><asp:Label ID="lblbid" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>--%>
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Card" runat="server" />
            </div>
            <div class="col-md-3" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <%--<button id="btn_close" class="btn-sm btn-danger">--%>
                    <%--<i class="fa fa-window-close btn-sm btn-danger" aria-hidden="true"></i>--%>
                    <%--</button>--%>
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Always">
        <ContentTemplate>

            <asp:LinkButton ID="LinkButton3" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupEmployee" runat="server"
                PopupControlID="pnlEmployee" TargetControlID="LinkButton3" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideEmployee">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlEmployee" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Please Select Employee</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideEmployee"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton4" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupApproveOrReject" runat="server"
                PopupControlID="pnlApproveOrReject" TargetControlID="LinkButton4" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideApproveOrReject">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlApproveOrReject" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Please select the Date to Approve or Reject time sheet</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideApproveOrReject"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton5" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStartDate" runat="server"
                PopupControlID="pnlStartDate" TargetControlID="LinkButton5" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStartDate">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStartDate" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Start time is empty</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideStartDate"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton6" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupEndDate" runat="server" 
                PopupControlID="pnlEndDate" TargetControlID="LinkButton6" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideEndDate" >
            </cc1:ModalPopupExtender>
            
            <asp:Panel ID="pnlEndDate" runat="server" CssClass="modalPopup" Style="display:none;height:100%">
                <div>test</div>
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
                        <div id="swal2-content" style="display: block;">End time is empty</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideEndDate" visible="true"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                       
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton7" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStarDateGreaterEnd" runat="server"
                PopupControlID="pnlStarDateGreaterEnd" TargetControlID="LinkButton7" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStarDateGreaterEnd">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStarDateGreaterEnd" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Start Date is Greater than End Date</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideStarDateGreaterEnd"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton8" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupStartTimeMust" runat="server"
                PopupControlID="pnlStartTimeMust" TargetControlID="LinkButton8" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideStartTimeMust">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlStartTimeMust" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Start Time and End Time Must be within 24Hrs</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideStartTimeMust"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton9" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupWorked" runat="server"
                PopupControlID="pnlWorked" TargetControlID="LinkButton9" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideWorked">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlWorked" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Please Select Worked as Yes</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideWorked"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>
            <asp:LinkButton ID="LinkButton10" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupTimesheetalready" runat="server"
                PopupControlID="pnlTimesheetalready" TargetControlID="LinkButton10" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideTimesheetalready">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlTimesheetalready" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Timesheet already exists, so timesheet is not Copied.</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideTimesheetalready"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>


            <asp:LinkButton ID="LinkButton11" runat="server"></asp:LinkButton>
            <cc1:ModalPopupExtender ID="ModalPopupTimesheetEnter" runat="server"
                PopupControlID="pnlTimesheetEnter" TargetControlID="LinkButton11" BackgroundCssClass="modalBackground"
                CancelControlID="btnHideTimesheetenter">
            </cc1:ModalPopupExtender>
            <asp:Panel ID="pnlTimesheetEnter" runat="server" CssClass="modalPopup" Style="display: none">
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
                        <div id="swal2-content" style="display: block;">Please Enter Hours before clicking Copy Timesheet.</div>
                        <input class="swal2-input" style="display: none;"><input type="file" class="swal2-file" style="display: none;"><div class="swal2-range" style="display: none;">
                            <input type="range">
                            <output></output>
                        </div>
                        <select class="swal2-select" style="display: none;"></select><div class="swal2-radio" style="display: none;"></div>
                        <label for="swal2-checkbox" class="swal2-checkbox" style="display: none;">
                            <input type="checkbox"></label><textarea class="swal2-textarea" style="display: none;"></textarea><div class="swal2-validationerror" id="swal2-validationerror" style="display: none;"></div>
                    </div>
                    <div class="swal2-actions" style="display: flex;">
                        <button type="button" class="swal2-cancel swal2-styled" aria-label="" style="display: none;">Cancel</button>
                        <button id="btnHideTimesheetenter"
                            class="swal2-confirm swal2-styled" aria-label="" style="border-left-color: rgb(48, 133, 214); border-right-color: rgb(48, 133, 214);">
                            Ok</button>
                    </div>
                    <div class="swal2-footer" style="display: none;"></div>
                </div>
            </asp:Panel>

    <div class="panel panel-default" style="margin-top:-20px;">
        <div class="panel-body">
            <asp:Label ID="lblsms" Text="user" runat="server" Visible="false"></asp:Label>

            <div class="row">

                <div class="col-md-2" runat="server" id="Div_Select_Business" style="display:none;">
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

                <div class="col-md-2">
                    <div class="form-group">
                        <div><asp:Label ID="Label_Select_Employee" style="text-transform:uppercase" ForeColor="#808080" Font-Bold="true" Text="Select Employee" runat="server"></asp:Label></div>
                        <div style="margin-top:5px;"></div>
                        <div>
                            <%--<label>Select Employee</label>--%>
                            <asp:DropDownList style="padding-top:5px;"
                                ID="ddlemp" Width="100%"
                                runat="server"
                                AppendDataBoundItems="true"
                                CssClass="form-control js-example-basic-single"
                                OnSelectedIndexChanged="ddlemp_SelectedIndexChanged"
                                AutoPostBack="true">
                            </asp:DropDownList>
                            <%--<asp:Label ID="lblCurrentRate" runat="server"></asp:Label>
                            <asp:Label ID="lblCurrentErrorSms" runat="server"></asp:Label>--%>
                        </div>
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
                       #action_div_mobile_view {
                           margin-left: 0px;
                       }
                       .lnkbtncopytimesheet{
                            margin-left: -50px;
                            //margin-right: 5px;
                       }
                   }
               </style>
                
                
                <td style="width:60%">
                <div class="col-md-1 btnprvmargin">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton
                            ID="btnprv"
                            runat="server"
                            CssClass="buttonhyperlink"
                            BackColor=""
                            ClientIDMode="Static"
                            ForeColor="Black"
                            OnClick="btnprv_Click">
                                 <i class="fa fa-angle-left" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </div>
                </div>
                <div class="col-md-2">
                    <div class="form-group">
                        <label>Date Range</label>
                        <asp:TextBox
                            ID="txtmonth"
                            runat="server"
                            TextMode="Date"
                            OnTextChanged="txtmonth_TextChanged"
                            AutoPostBack="true"
                            required="required"
                            Max="9999-12-31"
                            CssClass="form-control" ClientIDMode="Static"></asp:TextBox><button type="button" aria-label="Open calendar" onclick="var d=document.getElementById('txtmonth');if(d.showPicker){d.showPicker();}else{d.focus();}">Calendar</button>
                        <%--firoz--%>
                        <asp:Label ID="lblWeek" runat="server" Visible="false" Font-Size="16"></asp:Label>
                        <asp:Label ID="lblweekDayName" Visible="false" runat="server" Font-Size="14"></asp:Label>
                    </div>
                </div>
                <div class="col-md-1">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton
                            ID="btnnext"
                            runat="server"
                            CssClass="buttonhyperlink"
                            BackColor=""
                            ForeColor="Black"
                            OnClick="btnnext_Click">
                                    <i class="fa fa-angle-right" aria-hidden="true"></i>
                        </asp:LinkButton>
                    </div>
                </div>
                    <br>
                <div class="col-md-4 lnkbtncopytimesheet" style="width:500px"><%--<div style="height:30px"></div>--%>
                   <div class="form-group" style="font-size:14px;">
                       <Table>
                           <tr >
                               <td style="border-style:None; ">
                                    <asp:HyperLink ID="hypProjectTime" runat="server" 
                                        Text="Enter/View Project Time Sheet |" ToolTip="Project time sheet can be entered by clicking this link" 
                                        Visible="false"></asp:HyperLink> 
                               </td>
                               <td style="border-style:None; ">
                                    <asp:LinkButton 
                                       ID="LinkButton12" ToolTip="Copy first day timesheet to entire week"
                                       OnClientClick="return confirm('Repeat the first saved present day into empty, pending days of this week? Entered and reviewed days will be kept.');" runat="server" CausesValidation="false" Text="Copy Entire Week |"
                                       OnClick="LinkButton_CopyCurrentWeek_Click">
                                    </asp:LinkButton>
                               </td>
                               <td style="border-style:None; ">
                                    <asp:LinkButton 
                                        ID="LinkButton_CopyNextWeek" OnClientClick="return confirm('Copy saved shifts and breaks to next week? Unapproved destination entries will be replaced. Locked or approved days stop the copy.');" Style="width:120px;" ToolTip="Copy current week timesheet to next week"
                                        runat="server" CausesValidation="false" AutoPostBack="false" Text="Copy Next Week"
                                        OnClick="LinkButton_CopyNextWeek_Click">
                                    </asp:LinkButton>
                               </td>
                               </tr>
                       </Table>
                      
                           <Table>
                           <tr>
                               <td style="border-style:None; ">
                            <asp:UpdatePanel ID="UPD2" runat="server">
                                <ContentTemplate>
                                   <asp:LinkButton 
                            ID="LinkButtonApproveTimesheetSelectedRows" ToolTip="Approve Selected Time sheets"
                            runat="server" Text="| Approve"
                            
                            OnClick="approveSelectedTimesheetRows" >
                        </asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp
                                    </ContentTemplate>
                          </asp:UpdatePanel>   
                                   </td>
                               <td style="border-style:None;">
                                    <asp:LinkButton 
                            ID="LinkButtonRejectTimesheetSelectedRows" ToolTip="Reject Selected Time Sheets"
                            runat="server" Text="| Reject"
                            OnClick="rejectSelectedTimesheetRows">
                        </asp:LinkButton>
                        <asp:LinkButton 
                            ID="LinkButtonLock" ToolTip="Lock Time Card"
                            runat="server" Text="| Lock"
                            OnClick="lnkLock_Click">
                        </asp:LinkButton>
                                   <asp:LinkButton 
                            ID="LinkButtonUnLock" ToolTip="Un Lock Time Card"
                            runat="server" Text="| Un Lock"
                            OnClick="lnkUnLock_Click">
                        </asp:LinkButton>
                                   </td>
                               </tr>
                          
                           </Table>
                            
                    </div>
                </div> 
              
               
                <div class="col-md-3" runat="server" id="action_div_mobile_view" ClientIDMode="Static" style="margin-left:-50px; display:none;">
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

                <div class="col-md-1" style="margin-right:0">
                    <div class="form-group">
                        <asp:LinkButton
                            ID="lnkApproved"
                            ToolTip="Approved"
                            runat="server"
                            ClientIDMode="Static"
                            Text="Approve"
                            BackColor="#1cff1c"
                            ForeColor="Black" Visible="false"
                            OnClick="lnkApproved_Click"
                            CssClass="buttonhyperlink" />
                    </div>
                </div>

                <div class="col-md-1">
                    <div class="form-group">
                        <asp:LinkButton
                            ID="lnkReject"
                            ToolTip="Reject"
                            runat="server"
                            Text="Reject"
                            BackColor="#ff4f22" Visible="false"
                            ForeColor="Black"
                            OnClick="lnkReject_Click"
                            CssClass="buttonhyperlink"></asp:LinkButton>

                    </div>
                </div>

            </div>
                
            <div class="row">
                <div class="col-md-6" style="margin-top:-10px;">
                <%--<center>--%>
                    <asp:Label ID="lblbid" Font-Bold="true" runat="server"></asp:Label><asp:Label Font-Bold="true" Font-Size="Large" ID="Label_Seperation_" Visible="false" runat="server" Text="   |   "></asp:Label>
                    <asp:Label ID="Label_Selected_EmpName" Font-Bold="true" runat="server" Text=""></asp:Label><asp:Label Font-Bold="true" Font-Size="Large" ID="Label_Seperation_1" Visible="false" runat="server" Text="   |   "></asp:Label>
                    <asp:Label ID="lblCurrentRate" Font-Bold="true" runat="server"></asp:Label><asp:Label runat="server" Font-Bold="true" Font-Size="Large" ID="Label_Seperation_2" Visible="false" Text="   |   "></asp:Label>
                    <asp:Label ID="lblCurrentErrorSms" Font-Bold="true" runat="server"></asp:Label>
                    <asp:Label ID="Label_HireDate" Visible="False" Font-Bold="true" runat="server" Text=""></asp:Label>
                <%--</center>--%>
                </div>
                
               
            </div>
            
                    
            <div class="row">
                <div>
                    <table Width="100%">
                        <tr>
                            <td width="90%">
                               <div class="col-md-3" style="text-align: left;">
                                <asp:Label ID="Label_Approved_Hours" Font-Bold="true" runat="server" Text=""></asp:Label>
                                <%--<asp:Label Font-Bold="true" Font-Size="Large" ID="Label_Seperation_3" Visible="false" runat="server" Text="   |   "></asp:Label>--%>
                                <asp:Label ID="Label_Rejected_Hours" Font-Bold="true" runat="server" Text=""></asp:Label>
                                <%--<asp:Label Font-Bold="true" Font-Size="Large" ID="Label_Seperation_4" Visible="false" runat="server" Text="   |   "></asp:Label>--%>
                                <asp:Label ID="Label_Pending_Hours" Font-Bold="true" runat="server" Text=""></asp:Label>
                            </div>
                            </td>
                            <td width="10%" align="left">
                   
                                </td>
                            
                        </tr>
                    </table>
                    
                    <div style="overflow-x: auto;">
                    
                        <asp:GridView
                                ID="GVChildlist"
                                runat="server"
                                EnableViewState ="true"
                                CssClass="table FS14"
                                AutoGenerateColumns="False"
                                CellPadding="1"
                                ForeColor="#333333"
                                GridLines="None"
                                
                                HeaderStyle-BackColor="#607DF2"
                                HeaderStyle-ForeColor="White"
                                OnRowCommand="GVChildlist_RowCommand"
                                OnRowDataBound="GVChildlist_RowDataBound"
                                OnPreRender="GVChildlist_PreRender"> <%--EmptyDataText="No Records Found"--%>
                                <Columns>

                                     <asp:TemplateField>
                                        <HeaderTemplate >
                                           <input type="checkbox" 
                                                ID="chkSelectAll1" 
                                                runat="server"
                                                onclick="SelectAll(this)"
                                                 />
                                        </HeaderTemplate>
                                        <ItemTemplate>

                                             
                                        <asp:HiddenField ID ="Hidden2" runat="server"  EnableViewState="true" />
                                            <asp:CheckBox id="chkSelect1"  EnableViewState="true" runat="server"  onclick="checkUncheck(this) "
                                            /> 
                                        
                                        
                                        </ItemTemplate>
                                         
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Day / Date">
                                        <ItemTemplate>
                                            <asp:Label
                                                ID="lblDay"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "weekname")%>' /><br />
                                            <asp:Label
                                                ID="lblDate"
                                                Width="100px"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "SelDate")%>'>
                                            </asp:Label>
                                            <asp:HiddenField
                                                ID="hfdweekno"
                                                runat="server"
                                                Value='<%#DataBinder.Eval(Container.DataItem, "weekno")%>' />
                                            <asp:HiddenField
                                                ID="hfdweekdayNo"
                                                runat="server"
                                                Value='<%#DataBinder.Eval(Container.DataItem, "weekdayno")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[1]--%>
                                    <%--<asp:TemplateField HeaderText="Day">
                                        <ItemTemplate>
                                            <asp:Label
                                                ID="lblDay"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "weekname")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>--%> <%--GVChildlist.Columns[2]--%>
                                    <asp:TemplateField HeaderText="Worked">
                                        <ItemTemplate>
                                            <asp:DropDownList
                                                runat="server" Width="95%"
                                                ID="ddstatus" CssClass="form-control xs-mselect"
                                                                                                
                                                 > <%--Width="120px"--%>
                                                <asp:ListItem Value="Select" Text="Select"></asp:ListItem>
                                                <%--<asp:ListItem Value="No" Text="No"></asp:ListItem>
                                                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>--%>
                                                <asp:ListItem Value="No" Enabled="true" Text="Absent"></asp:ListItem>
                                                <asp:ListItem Value="Yes" Enabled="true" Text="Present"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:HiddenField
                                                ID="hfdWorked"
                                                runat="server"
                                                Value='<%#DataBinder.Eval(Container.DataItem, "Worked")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[2]--%>
                                    <asp:TemplateField HeaderText="Start Time">
                                        <ItemTemplate>
                                            <asp:TextBox
                                                ID="txtRegularStartTime"
                                                TextMode="DateTime"
                                                CssClass="form-control"
                                                ClientIDMode="AutoID"                                                
                                                runat="server" Width="90%"
                                                Text='<%#Bind("ReqularStart")%>' onFocus="setStartDateTime(this);" onClick="setStartDateTime(this);" > <%--Width="190px"--%>
                                            
                                            
                                            </asp:TextBox>


                                            <%--<asp:Label ID="lblError" runat="server" Visible="false" Font-Size="Small" ForeColor="Red">Start time is empty</asp:Label>--%>
                                            <asp:Label ID="lblError" runat="server" Visible="false" Font-Size="Small" ForeColor="Red">Please enter the start time</asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[3]--%>
                                    <asp:TemplateField HeaderText="End Time">
                                        <ItemTemplate>
                                            <asp:TextBox
                                                ID="txtRegularEndTime"
                                                TextMode="DateTime"
                                                CssClass="form-control"
                                               ClientIDMode="AutoID"                                                
                                                runat="server" Width="90%"
                                                Text='<%#Bind("Regularend")%>' onFocus="setEndDateTime(this);" onClick= "setEndDateTime(this);" /> <%--Width="190px"--%>
                                            <%--<asp:Label ID="lblErrorend" runat="server" Visible="false" Font-Size="Small" ForeColor="Red">End time is empty</asp:Label>--%>
                                            <asp:Label ID="lblErrorend" runat="server" Visible="false" Font-Size="Small" ForeColor="Red">Please enter the end time</asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[4]--%>
                                    <asp:TemplateField HeaderText="Break Start" Visible="false">
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[5]--%>
                                    <asp:TemplateField HeaderText="Break End" Visible="false">
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[6]--%>
                                    <asp:TemplateField HeaderText="Over 1.5x" Visible="false">
                                        <ItemTemplate>
                                            <asp:TextBox
                                                ID="txtOver15x"
                                                Width="50px"
                                                TextMode="Number"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Over1_5x")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[7]--%>
                                    <asp:TemplateField HeaderText="Over 2x" Visible="false">
                                        <ItemTemplate>
                                            <asp:TextBox
                                                ID="txtOver2x"
                                                TextMode="Number"
                                                Width="50px"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Over2x")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[8]--%>
                                    <asp:TemplateField HeaderText="Holiday</br>Others" Visible="false">
                                        <ItemTemplate>
                                            <asp:TextBox
                                                ID="txtholiday"
                                                Width="50px"
                                                TextMode="Number"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Holiday")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[9]--%>
                                    <asp:TemplateField HeaderText="Total Hours" ItemStyle-HorizontalAlign="Justify"> <%--HeaderStyle-Width="100px"--%>
                                        <ItemTemplate>
                                            <asp:Label
                                                ID="lblTotal"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Regular")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[10]--%>
                                    <asp:TemplateField HeaderText="Add/View Break">
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkAddBreak"
                                                runat="server"
                                                CommandName="AddBreak"
                                                Text="Add Break"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[11]--%>
                                    <asp:TemplateField HeaderText="Break Hours" ItemStyle-HorizontalAlign="Justify"> <%--HeaderStyle-Width="100px"--%>
                                        <ItemTemplate>
                                            <asp:Label
                                                ID="lblBreakTotal"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "BreakTotal")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[12]--%>
                                    <asp:TemplateField HeaderText="Net Hours" ItemStyle-HorizontalAlign="Justify"> <%--HeaderStyle-Width="100px"--%>
                                        <ItemTemplate>
                                            <asp:Label
                                                ID="lblnetTotal"
                                                runat="server"
                                                Text='<%#DataBinder.Eval(Container.DataItem, "Total")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[13]--%>

                                    <asp:TemplateField HeaderText="Action" Visible="false"> <%--Visible="false"--%>
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkSave"
                                                runat="server"
                                                CommandName="Save"
                                                Text="Save"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                            <%--<asp:LinkButton
                                                ID="LinkButton_Edit_Timesheet"
                                                runat="server"
                                                Text=" / Edit"
                                                Visible="false"
                                                OnClick="LinkButton_Edit_Timesheet_Click" />--%>
                                            <asp:LinkButton
                                                ID="LinkButton_Edit_Timesheet"
                                                runat="server"
                                                Text="Edit"
                                                Visible="false"
                                                OnClick="LinkButton_Edit_Timesheet_Click" />
                                            <asp:LinkButton
                                                ID="lnksubmit"
                                                runat="server"
                                                CommandName="Submit"
                                                Text="Submit"
                                                Visible="true"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[14]--%>

                                    <asp:TemplateField HeaderText="Timesheet Status" Visible="false"> <%--HeaderText="Edit Status"--%> <%--Visible="true"--%>
                                        <ItemTemplate>
                                            <asp:LinkButton
                                                ID="lnkGvApp"
                                                runat="server"
                                                Text="Approve"
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
                                                OnClick="lnlGvAbsent_Click"
                                                Visible="false" />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[15]--%>

                                    <asp:TemplateField HeaderText="Edit Status" Visible="false">
                                        <ItemTemplate>
                                            <asp:DropDownList
                                                runat="server" Width="103px"
                                                ID="ddapproval" CssClass="form-control xs-mselect"
                                                OnSelectedIndexChanged="ddapproval_SelectedIndexChanged"
                                                AutoPostBack="true">
                                                <asp:ListItem Value="SELECT" Text="SELECT"></asp:ListItem>
                                                <asp:ListItem Value="Approved" Text="Approved"></asp:ListItem>
                                                <asp:ListItem Value="Reject" Text="Reject"></asp:ListItem>
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[16]--%>

                                    <asp:TemplateField HeaderText="Status" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                            <asp:HiddenField
                                                ID="hfdStatus"
                                                runat="server"
                                                Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[17]--%>
                                   
                                    <asp:TemplateField HeaderText="Action" Visible="true">
                                        <ItemTemplate>
                                            <asp:DropDownList Visible="false"
                                                runat="server"
                                                ID="ddl_Action" CssClass="buttonlooklikelinkbutton xs-mselect " 
                                                OnSelectedIndexChanged="ddl_Action_SelectedIndexChanged" AutoPostBack="true" >
                                                <asp:ListItem Value="Action" Text="Action"></asp:ListItem>
                                                <asp:ListItem Value="Approve" Text="Approve"></asp:ListItem>
                                                <asp:ListItem Value="Reject" Text="Reject"></asp:ListItem>
                                                <asp:ListItem Value="Absent" Text="Absent"></asp:ListItem>
                                                <asp:ListItem Value="Modify" Text="Modify"></asp:ListItem>
                                                <asp:ListItem Value="Delete" Text="Delete"></asp:ListItem>
                                            </asp:DropDownList>

                                            <div class="dropdown">
                                                <%--<button class="buttonlooklikelinkbutton" id="dropdownMenu1" 
                                                    data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                                                    Action&nbsp;<i class="fa fa-angle-down" aria-hidden="true"></i>
                                                </button>--%>
                                                <asp:LinkButton ID="dropdownMenu1" runat="server" Text="Action" data-toggle="dropdown" 
                                                    aria-haspopup="true" aria-expanded="true" />&nbsp;<i class="fa fa-angle-down" style="color:#607DF2;" aria-hidden="true"></i>
                                                <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
                                                    <li><asp:LinkButton ID="LinkButton_Approve" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Approve" OnClick="LinkButton_Approve_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Reject" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Reject" OnClick="LinkButton_Reject_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Absent" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Absent" Visible="false" OnClick="LinkButton_Absent_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Modify" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Modify" OnClick="LinkButton_Modify_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Delete" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Delete" OnClick="LinkButton_Delete_Click"></asp:LinkButton></li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[18]--%>

                                    <asp:TemplateField HeaderText="Action" Visible="false">
                                        <ItemTemplate>
                                            <div class="dropdown">
                                                <%--<button class="buttonlooklikelinkbutton" id="dropdownMenu1" 
                                                    data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">
                                                    Action&nbsp;<i class="fa fa-angle-down" aria-hidden="true"></i>
                                                </button>--%>
                                                <asp:LinkButton ID="dropdownMenu2" runat="server" Text="Action" data-toggle="dropdown" 
                                                    aria-haspopup="true" aria-expanded="true" />&nbsp;<i class="fa fa-angle-down" style="color:#607DF2;" aria-hidden="true"></i>
                                                <ul class="dropdown-menu" aria-labelledby="dropdownMenu2">
                                                    <li><asp:LinkButton ID="LinkButton_Approve2" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Save" OnClick="LinkButton_Approve_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Modify2" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Modify" OnClick="LinkButton_Modify_Click"></asp:LinkButton></li>
                                                    <li><asp:LinkButton ID="LinkButton_Delete2" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Delete" OnClick="LinkButton_Delete_Click"></asp:LinkButton></li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>

                                    </asp:TemplateField> <%--GVChildlist.Columns[19]--%>
                                   
                                </Columns>
                           
                         </asp:GridView>
   
                    </div>
                               
                </div>
            </div>

                        <%--<br />--%>
                        <%--<asp:Chart ID="Chart1" runat="server" Visible="false" Width="550px">
                            <Legends>
                                <asp:Legend Alignment="Center" Docking="Left"  IsTextAutoFit="false" TitleAlignment="Far" TableStyle="Tall" Name="Default" LegendStyle="Column"></asp:Legend>
                            </Legends>
                                <series>
                                    <asp:Series Name="Series1">
                                    </asp:Series>
                                </series>                            
                            <ChartAreas>
                                <asp:ChartArea Name="ChartArea1">
                                </asp:ChartArea>
                            </ChartAreas>
                        </asp:Chart>--%>

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

           <%-- <asp:Chart ID="Chart3" runat="server">
                <Series>
                    <asp:Series Name="Series1">
                    </asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1">
                    </asp:ChartArea>
                </ChartAreas>
            </asp:Chart>--%>


           <%-- <asp:Chart ID="Chart3" runat="server">
                <Series>
                    <asp:Series Name="Series1">
                    </asp:Series>
                </Series>
                <ChartAreas>
                    <asp:ChartArea Name="ChartArea1">
                    </asp:ChartArea>
                </ChartAreas>
            </asp:Chart>--%>


        </div>
    </div>
   </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlAction" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        

function GenerateCloseButton(sender, e) {
    if ($('#ajax__calendar_close_button').length == 0) {
        $(sender._footer).before("<div id='ajax__calendar_close_button'>OK</div>");
        $('#ajax__calendar_close_button').bind("click", sender, function(e) {
            e.data.hide();
        });
    }
}
</script>
</asp:Content>

