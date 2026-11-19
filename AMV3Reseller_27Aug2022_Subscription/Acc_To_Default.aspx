<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Acc_To_Default.aspx.cs" Inherits="Acc_To_Default" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            $('#GVEmployeeList').DataTable(
                {
                });
            $('#GvEmpListOwner').DataTable({
            });
            $('#GvEmpListForBusOwner').DataTable({
            });
            $('#GvDivAdminBussOwner').DataTable({
            });

            $('#gvbusinessListForManager').DataTable({
            });

            $('#gvBusinessListEmp').DataTable({
            });
            $('#GvPopUpEmpList').DataTable({
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

    <style>
        .switch {
            position: relative;
            display: inline-block;
            width: 125px;
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
            background-color: #949494;
            -webkit-transition: .4s;
            transition: .4s;
            border-radius: 34px;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 26px;
                width: 26px;
                left: 4px;
                bottom: 4px;
                background-color: white;
                -webkit-transition: .4s;
                transition: .4s;
                border-radius: 50%;
            }

        input:checked + .slider {
            /*background-color: #2ab934;*/
            background-color: #607DF2;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(125px);
            -ms-transform: translateX(125px);
            transform: translateX(91px);
        }

        /*------ ADDED CSS ---------*/
        .slider:after {
            content: 'Not Active';
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
            content: 'Active';
        }

        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
            opacity: 0.6;
        }

        .modalPopup {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
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
                height: 350px;
                width: auto;
            }

        .buttonhyperlink {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 30px;
            color: white;
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
                background-color: #607DF2;
                border-radius: 30px;
                color: white;
                padding: 10px 25px;
                text-align: center;
                text-decoration: none;
                display: block;
                margin-top: -1px;
                font-family: sans-serif;
                font-size: medium;
            }

        /*.HeaderTemplate {
            border-bottom: 2px solid black;
        }*/
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <style type="text/css">
        .GridVieCss {
            vertical-align: central !important;
            width: 100%;
        }
    </style>
    <script type="text/javascript" src='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.min.js'></script>
    <link media="screen" rel="stylesheet" href='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.min.css' />
    <link media="screen" rel="stylesheet" href='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.css' />
    <script type="text/javascript">
        $(function () {
            $("[id*=togBtnForAdminBussOwner]").on("click", function () {
                var id = $(this).closest('tr').find('[id*=hdID]').val();
                swal({
                    title: 'Are you sure?',
                    text: "Do you want to proceed?",
                    type: 'warning',
                    showCancelButton: true,
                    //confirmButtonColor: '#3085d6',
                    //cancelButtonColor: '#d33',
                    confirmButtonColor: '#607DF2',
                    cancelButtonColor: '#607DF2',
                    confirmButtonText: 'Yes',
                    cancelButtonText: 'No',
                    confirmButtonClass: 'btn btn-success',
                    cancelButtonClass: 'btn btn-success',
                    buttonsStyling: true
                }).then(function (result) {
                    if (result) {
                        $.ajax({
                            type: "POST",
                            url: "Default.aspx/DeleteClick",
                            data: "{id:" + id + "}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (r) {
                                if (r.d == "Deleted") {
                                    location.reload();
                                }

                            }
                        });
                    }
                },
                    function (dismiss) {

                    });
                return false;
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" style="display: none;">
        <div class="panel-body">
            <h2 class="text-primary" style="text-align: left; margin-top: -2px;"><i class="fa fa-address-card-o"></i>DashBoard</h2>
        </div>
    </div>
    <div id="divAllHead" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <h2 class="text-primary" style="text-align: left; margin-top: -2px;"><i class="fa fa-address-card-o"></i>Business List</h2>
            </div>
        </div>
    </div>

    <div id="divAdmin" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <h2 style="text-align: left; margin-top: 12px;">Business Owner List</h2>
                    </div>
                    <div class="col-md-6">
                        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Add Business Owner" NavigateUrl="~/AddBusinessOwner.aspx"
                            class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

        <div class="panel panel-default" id="div1">
            <div class="panel-body">

                <div class="row">
                    <div class="col-md-3" style="text-align: center; position: absolute; z-index: 9; margin-left: 400px; margin-top: 10px;">
                        <div class="form-group">
                            <div class="row">
                                <div class="col-md-4" style="margin-top: 10px; padding: 0;">
                                    <label class="pull-right" style="float: right;" for="Name">Status:</label>
                                </div>
                                <div class="col-md-8">
                                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_status_SelectedIndexChanged"
                                        AutoPostBack="true">
                                        <asp:ListItem Value="" Selected="True">Select</asp:ListItem>
                                        <asp:ListItem Value="YES">Active</asp:ListItem>
                                        <asp:ListItem Value="NO">Not Active</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>


                        </div>
                    </div>
                </div>

                <div style="overflow-x: scroll;">

                    <asp:GridView ID="GvDivAdminBussOwner" runat="server" ClientIDMode="Static" Width="100%"
                        CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                        GridLines="None" EmptyDataText="No Records Found" OnRowDataBound="GvDivAdminBussOwner_RowDataBound"
                        OnPreRender="GvDivAdminBussOwner_PreRender"
                        RowStyle-VerticalAlign="Bottom">

                        <AlternatingRowStyle BackColor="#f5f5f5" />


                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblBusinessNamedivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "bussname")%>'
                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:Label>
                                    <asp:HiddenField ID="hdfEmployeeIDdivAdmin" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="User Type" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblDomaindivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Client Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblTitledivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FirstName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Contact Number">
                                <ItemTemplate>
                                    <asp:Label ID="lblUrldivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainPhone")%>'
                                        Font-Bold="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email Id">
                                <ItemTemplate>
                                    <asp:Label ID="EMail_primarydivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainEmail")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Create Date">
                                <ItemTemplate>
                                    <asp:Label ID="Created_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Password" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="BillingTypedivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Password")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Edit">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hypditdivAdmin" ToolTip="Edit" runat="server" NavigateUrl='<%# "~/AddBusinessOwner.aspx?EmployeeIDEdit=" + Eval("EmployeeID")%>'>
                                                            <i class="fa fa-edit fa-lg"></i>
                                    </asp:HyperLink>
                                    <asp:LinkButton Visible="false" ClientIDMode="Static" data-toggle="tooltip" ID="btneditdivAdmin"
                                        ToolTip="Edit" runat="server" data-id='<%# Eval("EmployeeID") %>' CssClass="btn"
                                        OnClick="btneditdivAdmin_Click"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                    </asp:LinkButton>
                                    <asp:LinkButton Visible="false" ClientIDMode="Static" data-toggle="tooltip" ID="btnDeletedivAdmin"
                                        ToolTip="Delete" runat="server" data-id='<%# Eval("EmployeeID") %>' OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                                        <i class="fa fa-trash fa-lg"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Owner Status">
                                <ItemTemplate>
                                    <label class="switch">
                                        <asp:HiddenField ID="hdID" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'
                                            runat="server" />
                                        <asp:CheckBox ID="togBtnForAdminBussOwner" ClientIDMode="Static" runat="server" OnCheckedChanged="togBtnForAdminBussOwner_CheckedChanged" />
                                        <div class="slider round"></div>
                                    </label>
                                    <asp:HiddenField ID="hfdStatusForAdminBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
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
    </div>
    <div id="cardbus" runat="server">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6">
                    <%--<h2 style="text-align: left; margin-top: 12px; visibility:hidden;">Business List</h2>--%>
                    <asp:HyperLink ID="btnback" runat="server" Text="Back" NavigateUrl="~/Accountant.aspx" Width="100px" CssClass="buttonhyperlink" ClientIDMode="Static" />

                </div>
                <div class="col-md-6">
                    <asp:Button ID="btnAddBusiness" CssClass="buttonhyperlink" ClientIDMode="Static" runat="server" Style="float: right; margin-top: 12px;"
                        OnClick="btnAddBusiness_Click" Text="Add New Business" />
                    <script>
                        $(document).ready(function () {
                            $('#btnAddBusiness').click(function () {
                                document.cookie = "busstabid=#home";
                            });
                        });
                    </script>
                </div>
            </div>
        </div>
    </div>
    <div id="dividBusinessOwner" runat="server" style="display: none">
        <%-- class="panel panel-default"--%>


        <div class="panel panel-default" id="divlist" runat="server">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: scroll;">
                            <%--GvEmpList Start--%>

                            <asp:GridView ID="GVEmployeeList" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GVEmployeeList_PreRender"
                                OnRowDataBound="GVEmployeeList_RowDataBound">
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Company Name">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Operating Name">
                                        <ItemTemplate>
                                            <asp:Label ID="OperatingName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Business Registration Date">
                                        <ItemTemplate>
                                            <asp:Label ID="BusinessRegistrationDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessRegistrationDate")%>'
                                                Font-Bold="true"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>




                                    <asp:TemplateField HeaderText="Contact Name">
                                        <ItemTemplate>
                                            <asp:Label ID="PrimaryContactName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactName")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Primary Contact Number">
                                        <ItemTemplate>
                                            <asp:Label ID="PrimaryContactNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Business Assignment Date">
                                        <ItemTemplate>
                                            <asp:Label ID="BusinessAgreementDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessAgreementDate")%>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Client Status">
                                        <ItemTemplate>
                                            <label class="switch">
                                                <asp:CheckBox ID="togBtn" runat="server" Checked="true" OnCheckedChanged="togBtn_CheckedChanged"
                                                    AutoPostBack="true" />
                                                <div class="slider round"></div>
                                            </label>


                                            <asp:HiddenField ID="hfdStatus" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>

                                            <asp:LinkButton ClientIDMode="Static" ToolTip="Edit" data-toggle="tooltip" ID="btnedit"
                                                runat="server" OnClick="btnedit_Click" data-id='<%# Eval("BusinessID") %>' CssClass="btn"
                                                Visible="false"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-edit fa-lg"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ClientIDMode="Static" ID="btnDelete" data-toggle="tooltip" runat="server"
                                                OnClick="btnDelete_Click" data-id='<%# Eval("BusinessID") %>' CssClass="btn"
                                                Visible="false" OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-trash fa-lg"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>


                            </asp:GridView>
                            <%--GvEmpList End--%>


                            <asp:GridView ID="GvEmpListForBusOwner" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GvEmpListForBusOwner_PreRender"
                                OnRowDataBound="GvEmpListForBusOwner_RowDataBound">
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Company Name">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                            <asp:HyperLink ID="hyeditBuss" ClientIDMode="Static" runat="server" Text="Update Profile"
                                                NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $(document).ready(function () {
                                                    $('#hyeditBuss').click(function () {
                                                        document.cookie = "busstabid=#home";
                                                    });
                                                });
                                            </script>
                                            <%--<script>
                                                $('#hyeditBuss').click(function () {
                                                    document.cookie = "busstabid=#home";
                                                });
                                            </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Name">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                            <asp:HyperLink ID="hyeditManger" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $('#hyeditManger').click(function () {
                                                    document.cookie = "busstabid=#home";
                                                });
                                            </script>
                                            <%--<script>
                                                $('#hyeditManger').click(function () {
                                                    document.cookie = "busstabid=#home";
                                                });
                                            </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Number">
                                        <ItemTemplate>
                                            <asp:Label ID="PrimaryContNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Employee Detail">
                                        <ItemTemplate>
                                            <asp:Label ID="CNT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNT")%>'
                                                Font-Bold="true"></asp:Label>
                                            <b>|</b>
                                            <asp:LinkButton ID="lnkBusOnwerEmpList" runat="server" Text="Employee List" OnClick="lnkBusOnwerEmpList_Click"></asp:LinkButton>
                                            <b>|</b>
                                            <asp:HyperLink ID="hyAddEmp" ClientIDMode="Static" runat="server" Text="Add Employee"
                                                NavigateUrl='<%# "~/AddNewEmployee_TAB.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink><b> </b>
                                            <script>
                                                $('#hyAddEmp').click(function () {
                                                    document.cookie = "tabid=#home";
                                                });
                                            </script>

                                            <br />
                                            </b>
                                <asp:HyperLink Visible="false" ID="hyAssbus" runat="server" Text="Assign Staff to Business"
                                    NavigateUrl="~/AssignBusiness.aspx"></asp:HyperLink><b>|</b>
                                            <asp:HyperLink ID="hyAssManger" runat="server" Text="Assign Employee to Manager"
                                                NavigateUrl='<%# "~/AssgineManager.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Enter/View Time Sheet" NavigateUrl='<%# "~/ViewTimeSheetEntry.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <asp:HyperLink ID="hyEditTimeSheet" Visible="false" runat="server" Text="Edit Time Sheet"
                                                NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                            <asp:HyperLink ID="hyWeekalyTimeSheet" Visible="false" runat="server" Text="Weekly Time Sheet"
                                                NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Client Status" Visible="true">
                                        <ItemTemplate>
                                            <label class="switch">
                                                <asp:CheckBox ID="togBtnForBus" runat="server" Checked="true" OnCheckedChanged="togBtnForBus_CheckedChanged"
                                                    AutoPostBack="true" />
                                                <div class="slider round"></div>
                                            </label>
                                            <asp:HiddenField ID="hfdStatus" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action" Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ClientIDMode="Static" ToolTip="Edit" data-toggle="tooltip" ID="btneditForBus"
                                                runat="server" OnClick="btneditForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                Visible="false" CssClass="btn"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-edit fa-lg"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ClientIDMode="Static" ID="btnDeleteForBus" data-toggle="tooltip"
                                                runat="server" OnClick="btnDeleteForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                CssClass="btn" OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-trash fa-lg"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>

                            </asp:GridView>


                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="Owner" runat="server" style="display: none;">

        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row" style="display: none;">

                    <div class="col-md-4">
                        <div class="form-group" style="display: none;">
                            <label>Business Name</label>

                        </div>
                    </div>

                    <div class="col-md-4">
                        <div class="form-group">
                            <label>Employee Name</label>

                        </div>
                    </div>
                </div>

                <asp:GridView ID="GvEmpListOwner" runat="server" ClientIDMode="Static"
                    CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                    ForeColor="#333333"
                    GridLines="None" EmptyDataText="No Records Found" OnPreRender="GvEmpListOwner_PreRender"
                    OnRowDataBound="GvEmpListOwner_RowDataBound">
                    <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                    <AlternatingRowStyle BackColor="#f5f5f5" />
                    <%--<HeaderStyle CssClass="bg-clouds" />--%>

                    <Columns>

                        <asp:TemplateField HeaderText="Sl. No.">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Company Name">
                            <ItemTemplate>
                                <asp:Label ID="CompanyName1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                <asp:HiddenField ID="hdfBusinessID1" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                <b>
                                    <br />
                                </b>
                                <asp:HyperLink ID="hyeditBuss1" ClientIDMode="Static" runat="server" Text="Update Profile"
                                    NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                <script>
                                    $(document).ready(function () {
                                        $('#hyeditBuss').click(function () {
                                            document.cookie = "busstabid=#home";
                                        });
                                    });
                                </script>
                               
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Contact Name">
                            <ItemTemplate>
                                <asp:Label ID="MangerName1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label><b>
                                    <br />
                                </b>
                                <asp:HyperLink ID="hyeditManger1" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                <script>
                                    $('#hyeditManger').click(function () {
                                        document.cookie = "busstabid=#home";
                                    });
                                </script>
                              
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Contact Number">
                            <ItemTemplate>
                                <asp:Label ID="PrimaryContNumber1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Employee Detail">
                            <ItemTemplate>
                                <asp:Label ID="CNT1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNT")%>'
                                    Font-Bold="true"></asp:Label>
                                <b>|</b>
                                <asp:LinkButton ID="lnkBusOnwerEmpList1" runat="server" Text="Employee List" OnClick="lnkBusOnwerEmpList1_Click"></asp:LinkButton>
                                <b>|</b>
                                <asp:HyperLink ID="hyAddEmp" ClientIDMode="Static" runat="server" Text="Add Employee"
                                    NavigateUrl='<%# "~/AddNewEmployee_TAB.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink><b> </b>
                                <script>
                                    $('#hyAddEmp').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>

                                <br />
                                </b>
                                <asp:HyperLink Visible="false" ID="hyAssbus1" runat="server" Text="Assign Staff to Business"
                                    NavigateUrl="~/AssignBusiness.aspx"></asp:HyperLink><b>|</b>
                                <asp:HyperLink ID="hyAssManger1" runat="server" Text="Assign Employee to Manager"
                                    NavigateUrl='<%# "~/AssgineManager.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Time Sheet">
                            <ItemTemplate>
                                <asp:HyperLink ID="hyViewTimeSheet1" runat="server" Text="Enter/View Time Sheet" NavigateUrl='<%# "~/ViewTimeSheetEntry.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>
                                <asp:HyperLink ID="hyEditTimeSheet1" Visible="false" runat="server" Text="Edit Time Sheet"
                                    NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                <asp:HyperLink ID="hyWeekalyTimeSheet1" Visible="false" runat="server" Text="Weekly Time Sheet"
                                    NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Client Status" Visible="false">
                            <ItemTemplate>
                                <label class="switch">
                                    <asp:CheckBox ID="togBtnForBus1" runat="server" Checked="true" OnCheckedChanged="togBtnForBus1_CheckedChanged"
                                        AutoPostBack="true" />
                                    <div class="slider round"></div>
                                </label>
                                <asp:HiddenField ID="hfdStatus1" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ClientIDMode="Static" ToolTip="Edit" data-toggle="tooltip" ID="btneditForBus"
                                    runat="server" OnClick="btneditForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                    Visible="false" CssClass="btn"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-edit fa-lg"></i>
                                </asp:LinkButton>
                                <asp:LinkButton ClientIDMode="Static" ID="btnDeleteForBus" data-toggle="tooltip"
                                    runat="server" OnClick="btnDeleteForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                    CssClass="btn" OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-trash fa-lg"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>

                    </Columns>
                    <EmptyDataTemplate>
                        <div>No records found.</div>
                    </EmptyDataTemplate>

                </asp:GridView>
            </div>
        </div>
    </div>

    <div id="dividManager" runat="server" style="display: none;">

        <div class="panel panel-default" id="dividManagerHead" runat="server" style="visibility: hidden;">
            <div class="panel-body">
                <h2 style="text-align: left; margin-top: 5px;">Business List</h2>
            </div>
        </div>

        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: scroll;">
                            <asp:GridView ID="gvbusinessListForManager" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvbusinessListForManager_PreRender">
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Company Name">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                            <%--<asp:HyperLink ID="hyeditBuss" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>' ></asp:HyperLink>
                                <script>
                                    $('#hyeditBuss').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Name">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                            <asp:HiddenField ID="hfdMangerIDFormManger" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ManagerID")%>' />
                                            <%--<asp:HyperLink ID="hyeditManger" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>' ></asp:HyperLink>
                                <script>
                                    $('#hyeditManger').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Number">
                                        <ItemTemplate>
                                            <asp:Label ID="_PrimaryContactNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Employee Detail">
                                        <ItemTemplate>
                                            <asp:Label ID="CNT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "count")%>'
                                                Font-Bold="true"></asp:Label>
                                            <b>|</b>
                                            <asp:LinkButton ID="lnkMangerEmpList" runat="server" Text="Employee List" OnClick="lnkMangerEmpList_Click"></asp:LinkButton>
                                            <b>|</b>
                                            <asp:HyperLink ID="hyAddEmp2" ClientIDMode="Static" runat="server" Text="Add Employee"
                                                NavigateUrl='<%# "~/AddNewEmployee_TAB.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink><b> </b>
                                            <script>
                                                $('#hyAddEmp2').click(function () {
                                                    document.cookie = "tabid=#home";
                                                });
                                            </script>
                                            <br />
                                            </b>
                                <asp:HyperLink Visible="false" ID="hyAssbus" runat="server" Text="Assign Staff to Business"
                                    NavigateUrl="~/AssignBusiness.aspx"></asp:HyperLink>
                                            <%--<asp:HyperLink ID="hyAssManger" runat="server" Text="Assign Employee to Manager" NavigateUrl='<%# "~/AssgineManager?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Enter/View Time Sheet" NavigateUrl='<%# "~/ViewTimeSheetEntry.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID")%>'></asp:HyperLink>
                                            <asp:HyperLink Visible="false" ID="hyEditTimeSheet" runat="server" Text="Edit Time Sheet"
                                                NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                            <asp:HyperLink Visible="false" ID="hyWeekalyTimeSheet" runat="server" Text="Weekly Time Sheet"
                                                NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action" Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ClientIDMode="Static" ToolTip="Edit" data-toggle="tooltip" ID="btneditForBus"
                                                runat="server" OnClick="btneditForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                Visible="false" CssClass="btn"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-edit fa-lg"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ClientIDMode="Static" ID="btnDeleteForBus" data-toggle="tooltip"
                                                runat="server" OnClick="btnDeleteForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                CssClass="btn" OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-trash fa-lg"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>

                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <div id="dividEmp" runat="server" style="display: none;">

        <div class="panel panel-default" id="dividEmpHead" style="display: none;">
            <div class="panel-body">
                <h2 style="text-align: left; margin-top: 5px;">Business List</h2>
            </div>
        </div>

        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: scroll;">

                            <asp:GridView ID="gvBusinessListEmp" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvBusinessListEmp_PreRender">
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Company Name">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                            <%--<asp:HyperLink ID="hyeditBuss" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>' ></asp:HyperLink>
                                <script>
                                    $('#hyeditBuss').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Name">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                            <%--<asp:HyperLink ID="hyeditManger" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddBusiness(TAB).aspx?BusinessIDEdit=" + Eval("BusinessID")%>' ></asp:HyperLink>
                                <script>
                                    $('#hyeditManger').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>--%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Contact Number">
                                        <ItemTemplate>
                                            <asp:Label ID="_PrimaryContactNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Time Sheet">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Enter Time Sheet" NavigateUrl='<%# "~/TimeSheetEntry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID")%>'></asp:HyperLink>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Action" Visible="false">
                                        <ItemTemplate>
                                            <asp:LinkButton ClientIDMode="Static" ToolTip="Edit" data-toggle="tooltip" ID="btneditForBus"
                                                runat="server" OnClick="btneditForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                Visible="false" CssClass="btn"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-edit fa-lg"></i>
                                            </asp:LinkButton>
                                            <asp:LinkButton ClientIDMode="Static" ID="btnDeleteForBus" data-toggle="tooltip"
                                                runat="server" OnClick="btnDeleteForBus_Click" data-id='<%# Eval("BusinessID") %>'
                                                CssClass="btn" OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                <i class="fa fa-trash fa-lg"></i>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>

                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
    <%-- Model Pop Start --%>
    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmpList" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            <asp:Label ID="lblbusName" runat="server"></asp:Label>
            <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>

        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                        ForeColor="#333333"
                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="GvPopUpEmpList_PreRender"
                        OnRowDataBound="GvPopUpEmpList_RowDataBound">
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <AlternatingRowStyle BackColor="#f5f5f5" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Employee Name">
                                <ItemTemplate>
                                    <asp:Label ID="EmpName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmpName")%>'></asp:Label>
                                    <asp:HiddenField ID="hdfEmpNameid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                    <b>
                                        <br />
                                    </b>
                                    <asp:HyperLink ID="hyeditEmp" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddNewEmployee_TAB.aspx?EmployeeIDEdit=" + Eval("EmployeeID")%>'></asp:HyperLink>
                                    <script>
                                        $('#hyeditEmp').click(function () {
                                            document.cookie = "tabid=#home";
                                        });
                                    </script>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Client Rate">
                                <ItemTemplate>
                                    <asp:Label ID="Rate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Rate")%>'></asp:Label><br />
                                    <%--<asp:HiddenField ID="hdfEmpNameid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' /><b></b>--%>
                                    <asp:HyperLink ID="hyeditEmprate" runat="server" ClientIDMode="Static" Text="Update Rate" NavigateUrl='<%# "~/AddNewEmployee_TAB.aspx?EmployeeIDEdit=" + Eval("EmployeeID")%>'></asp:HyperLink>
                                    <script>
                                        $('#hyeditEmprate').click(function () {
                                            document.cookie = "tabid=#menu3";
                                        });
                                    </script>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Time Sheet">
                                <ItemTemplate>
                                    <%--<asp:HyperLink ID="hyEnterTimeSheet" runat="server" Text="Enter Time Sheet" NavigateUrl='<%# "~/employeetimesheet.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID")%>' ></asp:HyperLink>--%>
                                    <asp:HyperLink ID="hyEnterTimeSheet" runat="server" Text="Enter Time Sheet" NavigateUrl='<%# "~/TimeSheetEntry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID")%>'></asp:HyperLink>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Employee Status">
                                <ItemTemplate>
                                    <label class="switch">
                                        <%--style="margin-top:15px;"--%>
                                        <asp:CheckBox ID="togBtnForBusOwner" runat="server" Checked="true" OnCheckedChanged="togBtnForBusOwner_CheckedChanged"
                                            AutoPostBack="true" />
                                        <div class="slider round"></div>
                                    </label>
                                    <asp:HiddenField ID="hfdStatusForBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
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

    <%-- Model Pop End--%>
</asp:Content>

