<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ProjectView.aspx.cs" Inherits="ProjectView" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">

    <script>
        $(document).ready(function () {
            $('#GVEmployeeList').DataTable(
                {
                });
            $('#SupportAdminList').DataTable({
                
            });
            $('#GvEmpListForBusOwner').DataTable({
                
            });
            $('#GvDivAdminBussOwner').DataTable({
                
            });

            $('#gvbusinessListForManager').DataTable({
                
            });

            $('#GvEmpListOwner').DataTable({

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
    <script type = "text/javascript">
        function Confirm() {
           
                var confirm_value = document.createElement("INPUT");
                confirm_value.type = "hidden";
                confirm_value.name = "confirm_value";
            if (confirm("Do you want to delete this project?")) {
                    confirm_value.value = "Yes";
                } else {
                    confirm_value.value = "No";
                }
                document.forms[0].appendChild(confirm_value);
            
        }
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
            overflow:auto;
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
                height: 400px;
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
                border-radius: 30px;
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
                border-radius: 30px;
                color: white ;
                padding: 10px 25px;
                text-align: center;
                text-decoration: none;
                display: block;
                margin-top: -1px;
                font-family: sans-serif;
                font-size: medium;
            }
            .GridPager a, .GridPager span
    {
        display: block;
        height: 35px;
        width: 35px;
        font-weight: bold;
        text-align: center;
        text-decoration: none;
        padding:10px;
        
    }
    .GridPager a
    {
        background-color: #fff;
        color: #969696;
        border: 1px solid #dedede;
        
    }
    .GridPager a:hover
    {
        color: #337ab7;
    }
    .GridPager span
    {
        background-color: #337ab7;
        color: #fff;
        border: 1px solid #dedede;
        
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
            /*border-bottom: 2px solid black;*/
            /*text-align: center !important;*/
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
            //if (dismiss == 'cancel') {
            //    swal('Cancelled', 'No record de-actived', 'error');
            //}
        });
                return false;
            });
        });
    </script>
     
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:Label ID="hfdconfirm" ClientIDMode="Static" runat="server" />
    <div class="panel panel-default" style="display: none;">
        <div class="panel-body">
            <h2 class="text-primary" style="text-align: left; margin-top: -2px;"><i class="fa fa-address-card-o">
            </i>DashBoard</h2>
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
                        <div class="form-group" >
                            <div class="row">
                                <div class="col-md-5" style="margin-top:10px;padding:0;">
                                    <label  style="float:right;" for="Name">Business Name:</label>
                                </div>
                                <div class="col-md-4" >
                                    <asp:DropDownList ID="ddlBusiness" runat="server" CssClass="form-control" Width="60%" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlBusiness_SelectedIndexChanged">
                                        
                                    </asp:DropDownList>
                                </div>
                               <div class="col-md-3" >
                                   </div>
                            </div>


                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: scroll;">

                            <asp:GridView ID="gvBusinessListEmp" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" 
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvBusinessListEmp_PreRender" >
                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Project Name">
                                        <ItemTemplate>
                                            <asp:Label ID="ProjectName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <asp:HiddenField ID="dhdfprojectid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectID")%>' />
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
                                    <asp:TemplateField HeaderText="Start Date">
                                        <ItemTemplate>
                                            <asp:Label ID="startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startdate")%>'></asp:Label><b>
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

                                 


                                    <asp:TemplateField HeaderText="Time Sheet">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Enter Time Sheet" NavigateUrl='<%# "~/TimeSheetEntry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID")%>'></asp:HyperLink>

                                        </ItemTemplate>
                                    </asp:TemplateField>

                                   
                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>


   
</asp:Content>