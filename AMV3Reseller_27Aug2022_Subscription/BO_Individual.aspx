<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="BO_Individual.aspx.cs" Inherits="BO_Individual" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script >
		$(document).ready(function() {
		    $('#GVBusiness').DataTable({
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
    <style>
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
    </style>
	<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script>
        $(document).ready(function () {
            $('#ddlBusiness').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
            });
        });
    </script>
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
     <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" id="cardbus" runat="server">
        <div class="panel-body">
			<h2 style="text-align:left;margin-top:5px;">Employee Wise Report</h2>
		</div>
	</div>

    <div class="panel panel-default" runat="server">
        <div class="panel-body">
                 <div class="col-md-6">
                        <div class="form-group">
                            <label>Business Name</label>
                            <asp:HiddenField ID="hfSelected" runat="server" />
                                <asp:ListBox ID="ddlBusiness" ClientIDMode="Static" required runat="server" SelectionMode="Multiple" AppendDataBoundItems="true" CssClass="form-control js-example-basic-multiple"></asp:ListBox>
                        
                        </div>
                    </div>

            <div class="col-md-2">
                    <div class="form-group">
                        <label>From Date</label>
                       <asp:TextBox ID="txtFromDate" TextMode="Date" required runat="server" CssClass="form-control" Max="9999-12-31"></asp:TextBox>
                        
                    </div>
                </div>

            <div class="col-md-2">
                    <div class="form-group">
                        <label>To Date</label>
                       <asp:TextBox ID="txtToDate" TextMode="Date" required  runat="server" CssClass="form-control" Max="9999-12-31"></asp:TextBox>
                        
                    </div>
                </div>

            <div class="col-md-2">
                    <div class="form-group">
                        <label style="margin-top:15px;"></label>
                       <asp:Button ID="btnshow" runat="server" Text="Show" CssClass="btn btn-primary" 
                            Width="100%" OnClick="btnshow_Click"/>
                        
                    </div>
                </div>
            </div>
        </div>

    <div class="panel panel-default" runat="server">
        <div class="panel-body">
            <div class="row">
                
               <%-- <div class="col-md-2" style="display:none;">
                    <div class="form-group">
                        <label style="margin-top:15px;"></label>
                       <asp:Button ID="btnExport" runat="server" formnovalidate="formnovalidate" Text="Export" OnClick="btnExport_Click" CssClass="btn btn-primary" 
                            Width="100%" />
                        
                    </div>
                </div>--%>
            </div>

            <asp:GridView ID="GVBusiness" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GVBusiness_PreRender" AllowPaging="true" OnPageIndexChanging="GVBusiness_PageIndexChanging"
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

                                                                         <asp:TemplateField HeaderText="Business Owner">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblBusinessOwner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessOwner")%>'></asp:Label>
                                                                                <asp:HiddenField ID="hfdEmpid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmpId")%>' />
                                                                                <asp:HiddenField ID="hfdBussID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BussID")%>' />

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>



                                                                        <asp:TemplateField HeaderText="Business Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblBusinessName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>'></asp:Label>
                                                                                


                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Employee Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblEmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeName")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="UserType">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblUserType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Total Working Hr">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Total")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Status">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Export To">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton ID="lnkExcel" runat="server" Text="Excel" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Excel"></asp:LinkButton>/
                                                                                <asp:LinkButton ID="lnkPdf" runat="server" Text="PDF" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="PDF"></asp:LinkButton>/
                                                                                <asp:LinkButton ID="lnkDoc" runat="server" Text="Word" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Word"></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                    <EmptyDataTemplate>
                                                                        <div>No records found.</div>
                                                                    </EmptyDataTemplate>
                <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />

                                                                </asp:GridView>

         <asp:GridView runat="server" ID="GvExport"></asp:GridView>
            </div>
            
            </div>
       
</asp:Content>



