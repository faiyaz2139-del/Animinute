<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SupportAdminList.aspx.cs" Inherits="SupportAdminList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script>
        $(document).ready(function () {
            $('#GVMother').DataTable({
               
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
        .GridPager a, .GridPager span {
            display: block;
            height: 35px;
            width: 35px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            padding: 10px;
        }

        .GridPager a {
            background-color: #fff;
            color: #969696;
            border: 1px solid #dedede;
        }

        .GridPager a:hover {
            color: #337ab7;
        }

        .GridPager span {
            background-color: #337ab7;
            color: #fff;
            border: 1px solid #dedede;
        }

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
            border-radius: 0px;
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
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            
            border: 1px solid #607DF2;
            display: block;
            background-color:#607DF2 ;
            border-radius: 0px;
            color: white ;
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
            background-color:#607DF2 ;
            border-radius: 0px;
            color: white ;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6">
                    <h2 style="text-align: left; margin-top: 12px;">Support Admin List</h2>
                </div>
                <div class="col-md-6">
                    <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Add Support Admin" NavigateUrl="~/AddSuppAdmin.aspx"
                        class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                </div>
            </div>

        </div>
    </div>


     <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-lg-12 col-md-12">
                    <div style="overflow-x: scroll;">
                        <asp:GridView ID="GVMother" ClientIDMode="Static" runat="server" CssClass="table nowrap"
                            CellPadding="1" ForeColor="#333333" GridLines="None"
                            AutoGenerateColumns="False" OnRowCommand="GVMother_RowCommand"
                            EmptyDataText="No records Found" OnPreRender="GVMother_PreRender" OnRowDataBound="GVMother_RowDataBound" >
                            <AlternatingRowStyle BackColor="#f5f5f5" />

                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:Label>
                                        <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "SupportAdminId")%>' />
                                        <br />
                                        <asp:HyperLink ID="btnedit" runat="server" NavigateUrl='<%# "~/AddSuppAdmin.aspx?SupportAdminId=" + Eval("SupportAdminId")%>'
                                            Text="Upadate" data-original-title="Edit" data-toggle="tooltip">
                                                                                
                                        </asp:HyperLink>

                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContactName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Contact")%>'></asp:Label>


                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Email Id">
                                    <ItemTemplate>
                                        <asp:Label ID="LblEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <%--<label class="switch">--%>
                                        
                                        <%--<asp:CheckBox ID="togBtnForAdminBussOwner" ClientIDMode="Static" runat="server" />--%>
                                        <asp:ImageButton ID="btnactive" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="status" />
     
                                        <%--<div class="slider round"></div>--%>
                                   <%-- </label>--%>
                                        <asp:HiddenField ID="hfdStatusForAdminBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
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
    </div>
</asp:Content>

