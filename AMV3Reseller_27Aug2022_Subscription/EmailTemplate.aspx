<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="EmailTemplate.aspx.cs" Inherits="EmailTemplate" %>
<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
     <script >
		$(document).ready(function() {
		    $('#GVEmailTemp').DataTable({
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	 <div class="panel panel-default" id="cardbus" runat="server" style="margin-top:28px">
        <div class="panel-body">
			<h2 style="text-align:left;margin-top:5px;">Email Template</h2>
		</div>
	</div>


	 <div class="panel panel-default">
        <div class="panel-body">

			  <div class="row">
                    <div class="col-md-3">
                    <div class="form-group">
                        <label>Template Type</label>
                        <asp:DropDownList ID="ddlTemplateType"  required="true"  runat="server" CssClass="form-control" AppendDataBoundItems="true"></asp:DropDownList>

                    </div>
                </div>
                    <div class="col-md-6">
                        <div class="form-group">
                        <label>Template Page Name</label>
                        <asp:TextBox ID="txtpageName" runat="server" CssClass="form-control"></asp:TextBox>

                    </div>
                    </div>

                 <div class="col-md-3">
                        <div class="form-group">
                        <label style="visibility:hidden;">Template Page Name</label>
                        <asp:Button ID="btnsave" runat="server" CssClass="btn  btn-success btn-block" OnClick="btnsave_Click"
                                                    Text="Save"  Font-Bold="true" />

                    </div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-md-12">
                    <div class="form-group">
                        <label>Subject</label>
                        <asp:TextBox ID="txtSubject" runat="server" CssClass="form-control"></asp:TextBox>

                    </div>
                </div>
                </div>
               <div class="row">
                   <div class="col-md-12">
                    <div class="form-group">
                        <label>Description </label>
                        <CKEditor:CKEditorControl  required="true" ID="txtDescription" CssClass="form-control" runat="server" Height="200" >
		                        </CKEditor:CKEditorControl>

                    </div>
                </div>
               </div>

		</div>
	</div>

	 <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
			<div class="col-lg-12 col-md-12">
                <div style="overflow-x:scroll;">
                    <asp:GridView ID="GVEmailTemp" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVEmailTemp_PreRender">

                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                    <asp:HiddenField ID="hfd_TicketId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TempID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Template Type">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_TemplateType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TemplateType")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Template Name">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_TemplateName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TempName")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>



                            <asp:TemplateField HeaderText="Action" Visible="true" >
                                <ItemTemplate>
                                    <asp:LinkButton ID="btneditaction" runat="server" CssClass="btn btn-danger"
                                        data-original-title="Delete" data-toggle="tooltip" OnClick="btneditaction_Click">
                                                                                <i class="fa fa-trash"></i>
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

</asp:Content>

