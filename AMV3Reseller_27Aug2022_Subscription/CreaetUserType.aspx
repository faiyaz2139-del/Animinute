<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CreaetUserType.aspx.cs" Inherits="CreaetUserType" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
   <script >
       $(document).ready(function () {
           $('#GvUserTypeList').DataTable({
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
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

     <div class="panel panel-default" style="margin-top: 32px">
        <div class="panel-body">
            <h2 style="text-align:left;margin-top:5px;">Create User</h2>
        </div>
      </div>

    <div class="panel panel-default">
        <div class="panel-body">

			<div class="row">
				<div class="col-md-4">
					
                     <div class="form-group">
                       <h4 style="text-decoration:none;">User Type</h4>
                       <asp:TextBox ID="txtusetype" required="true" runat="server" CssClass="form-control"></asp:TextBox>
                       <asp:HiddenField ID="hdfeditid" ClientIDMode="Static" runat="server" />
                       <asp:Label ID="lbl" Visible="false" ClientIDMode="Static" runat="server"></asp:Label>
                    </div>
					  
                    <div class="form-group">
                        <asp:Button ID="btnSave" OnClick="btnSave_Click" Width="50%" runat="server" Text="Save" CssClass="btn btn-success" />
                    </div>

				</div>


				<div class="col-lg-6 col-md-6">
                                   <div style="overflow-x:scroll;">     
                                            <h4 style="text-decoration:none;">User Type List</h4><br />
                                                <asp:GridView ID="GvUserTypeList" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GvUserTypeList_PreRender"  >
                                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                <Columns>

                                                    <asp:TemplateField >
                                                                        <ItemTemplate>
                                                                            <%#Container.DataItemIndex + 1%>
                                                                        </ItemTemplate>
                                                        <HeaderTemplate> Sl/no</HeaderTemplate>
                                                                    </asp:TemplateField>

                                                    <asp:TemplateField >
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdf_User_TypeId" runat="server" Value='<%# Eval("User_TypeId") %>' />
                                                                            <asp:Label ID="lbl_UserType" runat="server" Text='<%# Eval("UserType") %>'></asp:Label>
                                                                        </ItemTemplate>
                                                        <HeaderTemplate>User Type</HeaderTemplate>
                                                                    </asp:TemplateField>
                                                    <asp:TemplateField  >
                                                                        <ItemTemplate>

                                                                            <asp:LinkButton  ID="btnedit" runat="server" OnClick="btnedit_Click"  CssClass="btn btn-success" data-original-title="Edit" data-toggle="tooltip" >
                                                                                <i class="fa fa-edit"></i>
                                                                            </asp:LinkButton>

                                                                            <asp:LinkButton ClientIDMode="Static"  ID="btnDelete" runat="server" OnClick="btnDelete_Click" data-id='<%# Eval("User_TypeId") %>' CssClass="btn btn-danger"  OnClientClick="Confirm()"><%--data-target="#addRowModal" data-toggle="modal"--%>
                                                                                <i class="fa fa-trash"></i>
                                                                            </asp:LinkButton>

                                                                        </ItemTemplate>
                                                        <HeaderTemplate>Action</HeaderTemplate>
                                                                    </asp:TemplateField>

                                                </Columns>
                                                <EmptyDataTemplate>No Record Available</EmptyDataTemplate>
                                            </asp:GridView>
                                      </div>  
                                    </div>

             </div>

							<!-- Modal -->
                               <script>
                                   $("#btnDelete").click(function () {
                                       var a = $(this).attr("data-id")
                                       $("#hdfeditid").val(a);
                                       $("#lbl").text(a);
                                   });

                               </script>
									<div class="modal fade" id="DeleteConfirm" tabindex="-1" role="dialog" aria-hidden="true">
										<div class="modal-dialog" role="document">
											<div class="modal-content">
												<div class="modal-header no-bd">
													<h5 class="modal-title">
														<span class="fw-mediumbold">
														Confirm
                                                        </span>

													</h5>
													<button type="button" class="close" data-dismiss="modal" aria-label="Close">
														<span aria-hidden="true">&times;</span>
													</button>
												</div>
												<div class="modal-body">
													<p class="small">Do You Want to Delete</p>

												</div>
												<div class="modal-footer no-bd">
													<asp:Button ID="btnyes" OnClick="btnyes_Click" formnovalidate  runat="server" Text="Yes" class="btn btn-success"/>
													<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
												</div>
											</div>
										</div>
									</div>
                                <div class="modal fade" id="Save" tabindex="-1" role="dialog" aria-hidden="true">
										<div class="modal-dialog" role="document">
											<div class="modal-content">
												<div class="modal-header no-bd">
													<h5 class="modal-title">
														<span class="fw-mediumbold">
														Information
                                                        </span>

													</h5>
													<button type="button" class="close" data-dismiss="modal" aria-label="Close">
														<span aria-hidden="true">&times;</span>
													</button>
												</div>
												<div class="modal-body">
													<p class="small">Record Saved</p>

												</div>
												<div class="modal-footer no-bd">

													<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
												</div>
											</div>
										</div>
									</div>

                                 <div class="modal fade" id="Update" tabindex="-1" role="dialog" aria-hidden="true">
										<div class="modal-dialog" role="document">
											<div class="modal-content">
												<div class="modal-header no-bd">
													<h5 class="modal-title">
														<span class="fw-mediumbold">
														Information
                                                        </span>

													</h5>
													<button type="button" class="close" data-dismiss="modal" aria-label="Close">
														<span aria-hidden="true">&times;</span>
													</button>
												</div>
												<div class="modal-body">
													<p class="small">Record Updated</p>

												</div>
												<div class="modal-footer no-bd">

													<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
												</div>
											</div>
										</div>
									</div>


		</div>
    </div>

</asp:Content>