<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AdminUserType.aspx.cs" Inherits="AdminUserType" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <script >
		$(document).ready(function() {
		    $('#GvDivAdminBussOwner').DataTable({
		        "paging": false,
		        "info": false
			});

			$('#multi-filter-select').DataTable( {
				"pageLength": 5,
				initComplete: function () {
					this.api().columns().every( function () {
						var column = this;
						var select = $('<select class="form-control"><option value=""></option></select>')
						.appendTo( $(column.footer()).empty() )
						.on( 'change', function () {
							var val = $.fn.dataTable.util.escapeRegex(
								$(this).val()
								);

							column
							.search( val ? '^'+val+'$' : '', true, false )
							.draw();
						} );

						column.data().unique().sort().each( function ( d, j ) {
							select.append( '<option value="'+d+'">'+d+'</option>' )
						} );
					} );
				}
			});

			// Add Row
			$('#add-row').DataTable({
				"pageLength": 5,
			});

			var action = '<td> <div class="form-button-action"> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-primary btn-lg" data-original-title="Edit Task"> <i class="fa fa-edit"></i> </button> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-danger" data-original-title="Remove"> <i class="fa fa-times"></i> </button> </div> </td>';

			$('#addRowButton').click(function() {
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
	<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
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

        /*.HeaderTemplate {
            border-bottom: 2px solid black;
        }*/
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <h2 style="text-align: left; margin-top: 12px;">Admin User Type</h2>
                    </div>
                    <div class="col-md-6">
                        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Home" NavigateUrl="~/SuperAdmin.aspx"  
                            class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>

 <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-3">
                            <div class="form-group">
                               <label> Admin User Type</label>
                               <asp:TextBox ID="txtUserType" runat="server" CssClass="form-control" required></asp:TextBox>
                               <asp:HiddenField ID="hfdEditId" runat="server" />
                            </div>
                         </div>
                    <div class="col-md-3">
                            <div class="form-group">
                              <div style="height:25px;"></div>
                               <asp:Button ID="btnAdd" runat="server" Text="Save" CssClass="btn btn-primary"  OnClick="btnAdd_Click"/>
                            </div>
                         </div>

                    <div class="col-md-3">
                            <div class="form-group">
                              <div style="height:25px;"></div>
                                <asp:GridView ID="GvDivAdminBussOwner" runat="server" ClientIDMode="Static" Width="100%"
                        CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                        GridLines="None" EmptyDataText="No Records Found" 
                        OnPreRender="GvDivAdminBussOwner_PreRender" AllowPaging="true"
                        RowStyle-VerticalAlign="Bottom" AllowSorting="true" PageSize="20">
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <AlternatingRowStyle BackColor="#f5f5f5" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Admin User Type">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lblTitledivAdmin" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "AdminUserType")%>' OnClick="lblTitledivAdmin_Click"></asp:LinkButton>
                                    
                               
                                    
                                    <asp:HiddenField ID="hfdAdminUserId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "AdminUserId")%>'/>
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
</asp:Content>

