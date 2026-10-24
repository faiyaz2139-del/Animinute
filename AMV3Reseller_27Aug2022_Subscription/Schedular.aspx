<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Schedular.aspx.cs" Inherits="Time_Sheet_Entry" %>
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

        .buttonhyperlink222:hover {
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

     <script type="text/javascript">
         $(document).ready(function () {
             $("#btnMsg").click(function () {
                 swal({
                     title: 'Are you sure?',
                     text: "The timesheet will be scheduled for the selected date range!",
                     type: 'warning',
                     showCancelButton: true,
                     confirmButtonColor: '#607DF2',
                     cancelButtonColor: '#607DF2',
                     confirmButtonText: 'Yes !',
                     cancelButtonText: 'No !',
                     confirmButtonClass: 'btn2 btn-primary',
                     cancelButtonClass: 'btn2 btn-primary',
                     buttonsStyling: false
                 }).then(function (result) {
                     if (result.value !== undefined) {
                         document.getElementById("btnprv").click();
                     }
                     // $("#btnprv").click();
                 }, function (dismiss) {
                     // dismiss can be 'cancel', 'overlay',
                     // 'close', and 'timer'
                     if (dismiss === 'cancel') {
                         swal(
                             'Cancelled',
                             'Your imaginary file is safe :)',
                             'error'
                         )
                     }
                 });
             });
         });
     </script>

    <script type="text/javascript">

        $(document).ready(function () {
            //$('#GvPopUpEmpList').DataTable(
            //    {
            //    });
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
    <script type="text/javascript" src="https://code.jquery.com/ui/1.12.1/jquery-ui.js" defer></script>

    <link type="text/css" rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" /><script type="text/javascript">
</script>
    <script>
        //$(function () {
        //    $('#txtRegularStartTime').timepicker();
        //});
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
                        <button id="btnHide" class="btn-sm btn-danger">
                            <i class="fa fa-window-close" aria-hidden="true"></i>
                        </button>
                    </div>
                </div>

                <div class="body">
                    <div class="row">
                        <div class="col-md-4">
                            <label>Break Start Time</label>
                            <asp:TextBox ID="txtBreakStartTime" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>

                        </div>
                        <div class="col-md-4">
                            <label>Break End Time</label>
                            <asp:TextBox ID="txtBreakEndTime" TextMode="DateTimeLocal" required="true" CssClass="form-control"
                                ClientIDMode="Static" Width="250px" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-md-4" style="margin-top: 0px;">
                            <label>&nbsp;</label>
                            <label style="visibility: hidden;">Break End Time</label>
                            <asp:Button ID="btnAddBreak" runat="server" CssClass="btn btn-primary" formnovalidate Text="Add"
                                OnClick="btnAddBreak_Click" Width="100%" />

                        </div>
                    </div>
                    <br />
                    <br />
                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found"
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
                                            <asp:LinkButton ID="lnkdelete" runat="server" Text="Delete" OnClick="lnkdelete_Click"></asp:LinkButton>

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
                    <h2 style="margin-top: 12px; text-align: left;">Time Sheet Schedule</h2>
                </div>
                <div class="col-md-4">
                    <div style="width: 100%; text-align: center; margin-top: 20px;">
                        <h4>
                            <asp:Label ID="lblbid" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
                        </h4>
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
                        Style="margin-top: 12px; float: right;display:none;">Home
                    </asp:HyperLink>
                    <%--<asp:LinkButton ID="LinkButton_ViewSchedule" ClientIDMode="Static" runat="server" CssClass="buttonhyperlink"
                            Text="View Schedule" Style="margin-top: 12px; float: right;" OnClick="LinkButton_ViewSchedule_Click"></asp:LinkButton>--%>
                    <asp:HyperLink ID="HyperLink1___" runat="server" CssClass="buttonhyperlink"
                        Style="margin-top: 12px; float: right;">View Schedule
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
                </a><br /><br />
                <asp:HyperLink ID="HyperLink1" runat="server" Text="View Schedule"></asp:HyperLink>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Time Sheet Schedule" runat="server" />
            </div>
            <div class="col-md-3" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>
    

    <div class="panel panel-default" id="Div1" runat="server">
        <div class="panel-body">
            
            <div class="row">
                <div class="col-md-12">
                    <asp:Label ID="Label_Maximum_Limit" Font-Bold="true" Font-Size="Larger" ForeColor="Red" runat="server" />
                    <div class="row">
                        <div class="col-md-4">
                            <div class="form-group">
                                <label for="Name">Business</label>
                                    <asp:DropDownList ID="ddl_Business" runat="server" CssClass="form-control" AppendDataBoundItems="true" Width="100%" ClientIDMode="Static" OnSelectedIndexChanged="ddl_Business_SelectedIndexChanged" AutoPostBack="true">
                                                
                                    </asp:DropDownList>
                                <asp:HiddenField ID="hfdMotherId" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <%--<label style="font-weight: bold; font-size: 16px; color: black">Employee</label>--%>
                            <label>User</label>
                            <asp:DropDownList ID="ddlemp" Width="100%" ClientIDMode="Static" required runat="server" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlemp_SelectedIndexChanged">
                                <asp:ListItem Text="Select" Value=""></asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfSelected" runat="server" />
                            <asp:HiddenField ID="hdnDate" runat="server" />
                            <asp:HiddenField ID="hdnDate1" runat="server" />
                        </div>
                        <div class="col-md-4" runat="server" id="divPrj" visible="false">
                            <label >Project</label>
                            <asp:DropDownList
                                ID="ddlProjectName"
                                runat="server"
                                Width="100%"
                                AppendDataBoundItems="true"
                                CssClass="form-control mselect">
                                <asp:ListItem Text="Please Select Project" Value="">
                                </asp:ListItem>
                            </asp:DropDownList>
                            </div>
                    </div>

                    <div class="row">
                            
                        <div class="col-md-4">
                            <label>From Date</label>
                            <asp:TextBox
                                ID="txtFromDate"
                                TextMode="DateTimeLocal"
                                CssClass="form-control"
                                ClientIDMode="Static"
                                Width="100%"
                                runat="server"
                                Text='<%#Bind("ReqularStart","{0:yyyy-MM-ddTHH:mm}")%>'>
                                            </asp:TextBox>
                        </div>                   

                        <div class="col-md-4">
                            <label>To Date</label>
                            <asp:TextBox
                                ID="txtToDate"
                                TextMode="DateTimeLocal"
                                CssClass="form-control"
                                ClientIDMode="Static"
                                Width="100%"
                                runat="server"
                                Text='<%#Bind("Regularend","{0:yyyy-MM-ddTHH:mm}")%>' />
                        </div>


                            
                    </div>
                    <div class="row">
                        <div class="col-md-3"></div>
                                                <div class="col-md-1">
                            <label>&nbsp;</label>
                                <a class="buttonhyperlink222" id="btnMsg">
                                    <%--<i class="" aria-hidden="true">Save</i>--%>
                                    Save
                                </a>
                            <%--<asp:LinkButton ID="btnMsg" Text="Save" runat="server" CssClass="buttonhyperlink222">
                            </asp:LinkButton>--%>
                            <asp:LinkButton ID="btnprv" style="display:none" runat="server" CssClass="buttonhyperlink222"
                                ClientIDMode="Static"
                                OnClick="btnprv_Click">
                                <i class="" aria-hidden="true">Save</i> 
                            </asp:LinkButton>
                   
                        </div>
                    </div>

                </div>
            </div>
            
        </div>
    </div>


    <div class="panel panel-default">
        <div class="panel-body" style="display:none">
            <div class="row">
               <%-- <div class="col-md-3">
                    <div class="form-group">
                        <label>Select Employee</label>
                        <asp:DropDownList ID="ddlemp" runat="server" AppendDataBoundItems="true" CssClass="form-control"
                            OnSelectedIndexChanged="ddlemp_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>--%>

                <div class="col-md-3" style="display:none">
                    <div class="form-group">
                        <label>Rate</label>
                        <asp:TextBox ID="lblCurrentRate" Font-Bold="true" ReadOnly="true" CssClass="form-control" runat="server" />
                        <asp:Label ID="lblCurrentErrorSms" Font-Bold="true" ReadOnly="true"  runat="server" />
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
                   }
               </style>
                
               

               

                <div class="col-md-1">
                    <div class="form-group">
                        <label></label>
                        <asp:LinkButton ID="btnnext" runat="server" CssClass="buttonhyperlink"
                            OnClick="btnnext_Click"><i class="fa fa-angle-right" aria-hidden="true"></i>
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
                                                <asp:Label ID="lblDate" Width="100px" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SelDate")%>'></asp:Label>
                                                <asp:HiddenField ID="hfdweekno" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "weekno")%>' />
                                                <asp:HiddenField ID="hfdweekdayNo" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "weekdayno")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Day">
                                            <ItemTemplate>
                                                <asp:Label ID="lblDay" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "weekname")%>'></asp:Label>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Worked">
                                            <ItemTemplate>
                                                <asp:DropDownList ID="ddlWorked" CssClass="form-control xs-mselect" runat="server">
                                                    <asp:ListItem Selected Value="No">No</asp:ListItem>
                                                    <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                </asp:DropDownList>

                                                <asp:HiddenField ID="hfdWorked" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Worked")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Start Time">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtRegularStartTime" TextMode="DateTimeLocal" CssClass="form-control" ClientIDMode="Static"
                                                    Width="230px" runat="server" Text='<%# Bind("ReqularStart","{0:yyyy-MM-ddTHH:mm}") %>'></asp:TextBox>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="End Time">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtRegularEndTime" TextMode="DateTimeLocal" CssClass="form-control" ClientIDMode="Static"
                                                    Width="230px" runat="server" Text='<%# Bind("Regularend","{0:yyyy-MM-ddTHH:mm}")%>'></asp:TextBox>
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
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>

                                                <asp:Label ID="lblStatus" runat="server"></asp:Label>
                                                <asp:HiddenField ID="hfdStatus" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>

                                                <asp:LinkButton ID="lnkSave"
                                                    runat="server"
                                                    CommandName="Save"
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

                                    </Columns>
                                </asp:GridView>

                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <div class="col-md-12">
                    <div class="row">
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <asp:LinkButton ID="lnkweekPre" runat="server" CssClass="buttonhyperlink" Width="125px" OnClick="lnkweekPre_Click">
                                    <i class="fa fa-angle-left" aria-hidden="true"></i>
                                    Previous
                            </asp:LinkButton>
                        </div>
                        <div class="col-md-6 col-sm-6 col-xs-6">
                            <asp:LinkButton ID="lnkweekNxt" Style="float: right;" runat="server" CssClass="buttonhyperlink" Width="100px" OnClick="lnkweekNxt_Click">
                                    Next
                                    <i class="fa fa-angle-right" aria-hidden="true"></i>
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

