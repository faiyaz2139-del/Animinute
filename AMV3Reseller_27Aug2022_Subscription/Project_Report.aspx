<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Project_Report.aspx.cs" Inherits="ProjectWiseReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <style>
        /*.btn-rounded {
            border-radius: 30px;
        }

        .btn-rounded:hover {
            border-radius: 30px;
        }*/
        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
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
    <script>
        $(document).ready(function () {
            $('#GVBusiness').DataTable({
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
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" id="cardbus" runat="server" style="display:none;">
        <div class="panel-body" style="margin-top: 0px;">
            <h2 style="text-align: center; margin-top: 5px; font-weight: bold; font-size: 30px;">Custom Report By Project</h2>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a><br /><br />
                <a href="Company_Report.aspx" id="A2" runat="server">
                    Company Report
                </a>
                &nbsp; &nbsp; &nbsp;
                 <a href="Daily_Report.aspx" id="A3" runat="server">
                    Daily Report
                </a>
                &nbsp; &nbsp; &nbsp;
                <a href="Employee_Report.aspx" id="A4" runat="server">
                    Employee Report
                </a>
            </div>
            <div class="col-md-4" style="text-align: center;margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Project Report" runat="server" />
            </div>
        </div>
    </div>

    <div class="panel panel-default" id="Div1" runat="server" style="margin-top: 0px; height: 80%;">
        <div class="panel-body" style="margin-top: 0px; height: 80%;">
            <div class="row">

                <div class="col-md-3">                    
                    <div class="form-group">
                        <label>Business Name</label>
                        <asp:DropDownList ID="ddlProject" runat="server" CssClass="form-control" AppendDataBoundItems="true" Width="100%" required>
                            <asp:ListItem Text="Please Select" Value=""></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="panel panel-default" style="background-color: #ECF0F3;">

        <div class="panel-body">
            <div class="row">
                <div class="col-md-3">
                    <label>Date Range</label>
                    <div class="form-group">
                        <asp:DropDownList ID="ddlDateRange" runat="server" CssClass="form-control" required AppendDataBoundItems="true" OnSelectedIndexChanged="ddlDateRange_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="Please Select" Value=""></asp:ListItem>
                            <asp:ListItem Value="Weekly" Text="Weekly"></asp:ListItem>
                            <asp:ListItem Value="Last 4 Weeks" Text="Last 4 Weeks"></asp:ListItem>
                            <asp:ListItem Value="Last 30 Days" Text="Last 30 Days"></asp:ListItem>
                            <asp:ListItem Value="Last 60 Days" Text="Last 60 Days"></asp:ListItem>
                            <asp:ListItem Value="Last 90 Days" Text="Last 90 Days"></asp:ListItem>
                            <asp:ListItem Value="Monthly" Text="Monthly"></asp:ListItem>
                            <asp:ListItem Value="Custom" Selected="True" Text="Custom"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="col-md-3">
                    <label>Report Status</label>
                    <asp:DropDownList ID="DropDownList_Report_Status" Width="100%" ClientIDMode="Static" required runat="server" CssClass="form-control" AppendDataBoundItems="true">
                        <asp:ListItem Text="All" Value="0"></asp:ListItem>
                        <asp:ListItem Text="Approved" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Rejected" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Absent" Value="4"></asp:ListItem>
                        <asp:ListItem Text="Pending" Value="1"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-md-2">
                    <label>From Date</label>
                    <asp:TextBox ID="txtFromDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox> <%--ReadOnly="true"--%>

                </div>

                <div class="col-md-2">
                    <label>To Date</label>
                    <asp:TextBox ID="txtToDate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox> <%--ReadOnly="true"--%>

                </div>

                <div class="col-md-2">
                    <div style="margin-top: 25px;"></div>
                    <asp:Button ID="btnShow" runat="server" CssClass="buttonhyperlink" OnClick="btnShow_Click" Text="Update Report" /> <%--Width="100%"--%>

                </div>

                <div class="col-md-2">
                    <div style="margin-top: 25px;"></div>
                    <div class="form-group">

                        <div class="dropdown">
                            <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" style="background-color: #ECF0F3; border-radius: 30px; border: 1px solid #607DF2; color: black; font-family: sans-serif; font-size: medium;" aria-expanded="true">
                                Download
    <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
                                <li>
                                    <asp:LinkButton ID="lnkExcel" runat="server" Text="Excel" OnClick="lnkExcel_Click"></asp:LinkButton></li>
                                <li>
                                    <asp:LinkButton ID="lnkDirectPdf" runat="server" Text="PDF" OnClick="lnkDirectPdf_Click"></asp:LinkButton></li>

                                <li>
                                            <asp:LinkButton ID="lnkCsvFile" runat="server" Text="CSV" Visible="false" OnClick="lnkCsvFile_Click"></asp:LinkButton></li>
                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div class="panel panel-default" runat="server" id="pnlhideandshow">
        <div class="panel-body" style="margin-top: 0px;">
            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x: auto;">
                    <asp:GridView ID="GVBusiness" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None"
                        AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVBusiness_PreRender" OnRowCommand="GVBusiness_RowCommand"
                        AllowPaging="true">

                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Project Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "projectname")%>' />
                                    <asp:HiddenField ID="hfdEmpid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>' />
                                    <asp:HiddenField ID="hfdProjectid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectId")%>' />
                                    <asp:HiddenField ID="hfdProjectName" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "projectname")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Gross Hours">
                                <ItemTemplate>
                                    <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "GrossHours")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>


                            <asp:TemplateField HeaderText="Break Time">
                                <ItemTemplate>
                                    <asp:Label ID="LblBreakTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BreakTotal")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Net Hours">
                                <ItemTemplate>
                                    <asp:Label ID="LblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "NetHours")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="View Report">
                                <ItemTemplate>
                                   

                                    <div class="dropdown">
                                        <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" style="background-color: #ECF0F3; border-radius: 30px; border: 1px solid #607DF2; color: black; font-family: sans-serif; font-size: medium;" aria-expanded="true">
                                            Download
    <span class="caret"></span>
                                        </button>
                                        <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
                                            <li>
                                                <asp:LinkButton ID="lnkGvExcel" runat="server" Text="Excel" CommandName="Excel" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="lnkPdf" runat="server" Text="PDF" CommandName="PDF" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton></li>
                                            <li>
                                                <asp:LinkButton ID="lnkCSV" runat="server" Text="CSV" CommandName="CSV" Visible="false" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton></li>

                                        </ul>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>



                        </Columns>
                        <EmptyDataTemplate>
                            <div>No records found.</div>
                        </EmptyDataTemplate>
                        <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                    </asp:GridView>
                    </div>
                </div>                
            </div>
        </div>
    </div>

    <div>


        <asp:GridView ID="GvExport" runat="server"></asp:GridView>

    </div>
</asp:Content>