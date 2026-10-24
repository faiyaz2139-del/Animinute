<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="BusinessOwnerlist.aspx.cs" Inherits="BusinessOwnerlist" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://gitcdn.github.io/bootstrap-toggle/2.2.2/css/bootstrap-toggle.min.css"
        rel="stylesheet">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>
    <%--<script src="https://gitcdn.github.io/bootstrap-toggle/2.2.2/js/bootstrap-toggle.min.js"></script>--%>
    <script src="https://oss.maxcdn.com/html5shiv/3.7.3/html5shiv.min.js"></script>
    <script src="https://oss.maxcdn.com/respond/1.4.2/respond.min.js"></script>

    <script >
		$(document).ready(function() {
		    $('#GVBusiness').DataTable({
		    }); 
		    $('#gvDuplicate').DataTable({
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
    <%--<script>
        $(document).ready(function () {
            $('#ddlBusiness').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
            });
        });
    </script>--%>
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
     <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>
</asp:Content>  
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="panel panel-default" id="cardbus" runat="server" style="margin-top:28px">
        <div class="panel-body">
			<h2 style="text-align:left;margin-top:5px;">Business Owner Report</h2>
		</div>
	</div>
    <div class="panel panel-default" runat="server">
        <div class="panel-body">
            <div class="row">
            <div class="col-md-3">
                    <div class="form-group">
                        <label>Business Owner Name</label>
                        <asp:DropDownList ID="ddlBusinessOwner" runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" OnSelectedIndexChanged="ddlBusinessOwner_SelectedIndexChanged" AutoPostBack="true">
                            <asp:ListItem Text="Select" Value=""></asp:ListItem>
                        </asp:DropDownList>
                        
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="row" id="aa" runat="server" visible="false">
                 <div class="col-md-2 pull-right"  style="margin-right:5px;">
                    <div class="form-group">
                        <label>Export To</label>
                       <asp:DropDownList ID="ddlExportType"  runat="server" CssClass="form-control" Width="95%">
                           <asp:ListItem Text="Select" Value="Select"></asp:ListItem>
                           <asp:ListItem Text="Excel" Value="Excel"></asp:ListItem>
                           <asp:ListItem Text="Word" Value="Word"></asp:ListItem>
                           <asp:ListItem Text="PDF" Value="PDF"></asp:ListItem>
                       </asp:DropDownList>
                        
                    </div>
                </div>
                    <div class="col-md-2" style="margin-left:15px;">
                    <div class="form-group">
                        <label style="margin-top:15px;"></label>
                       <asp:Button ID="btnExport" runat="server" formnovalidate="formnovalidate" Text="Export" OnClick="btnExport_Click" CssClass="btn btn-primary" 
                            Width="100%" />
                        
                    </div>
                </div>
            </div>

             <asp:GridView ID="GVBusiness" ClientIDMode="Static" runat="server" CssClass="table table-striped wrap dataTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GVBusiness_PreRender"
                 OnRowCommand="GVBusiness_RowCommand"
                 >

                                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                    <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Sl. No.">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Business Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblBusiness" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BussName")%>'></asp:Label>
                                                                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>



                                                                        <asp:TemplateField HeaderText="Employee Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblempname" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                                                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="User Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUserType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Contact No.">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblMainPhone" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainPhone")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Hire Date">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblHireDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Login Email">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblCCEmail" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CCEmail")%>' />
                                                                                <asp:LinkButton ID="lnkBusinessOwnerName"   runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CCEmail")%>' CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="BusinessOwnerName"></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Rate Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblRateType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "RateType")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Rate">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblRate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Rate")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Status">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                    <EmptyDataTemplate>
                                                                        <div>No records found.</div>
                                                                    </EmptyDataTemplate>


                                                                </asp:GridView>
</div>
            <div style="display:none">
             <asp:GridView ID="gvDuplicate"  ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="gvDuplicate_PreRender">

                                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                    <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Sl. No.">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Business Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblBusiness" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BussName")%>'></asp:Label>
                                                                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>



                                                                        <asp:TemplateField HeaderText="Employee Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblempname" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                                                                
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="User Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUserType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Contact No.">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblMainPhone" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainPhone")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Hire Date">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblHireDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Login Email">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblCCEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CCEmail")%>' />
                                                                                
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Rate Type">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblRateType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "RateType")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Rate">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblRate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Rate")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Status">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
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
   
</asp:Content>

