<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="EmailSetting.aspx.cs" Inherits="EmailSetting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script >
		$(document).ready(function() {
		    $('#GVDomainList').DataTable({
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
			<h2 style="text-align:left;margin-top:5px;">Email Setting</h2>
		</div>
	</div>

	 <div class="panel panel-default" runat="server">
        <div class="panel-body">

			<div class="row">
                    <div class="col-md-3">
                    <div class="form-group">
                        <label>Sender Email</label>
                        <asp:TextBox ID="txtSenderEmail" textmode="Email" required="true"  runat="server" CssClass="form-control"></asp:TextBox>
                        <asp:HiddenField runat="server" ID="hfdEId" />
                    </div>
                </div>
                    <div class="col-md-2">
                    <div class="form-group">
                        <label>Password</label>
                        <asp:TextBox ID="txtPassword"  required="true"  runat="server" CssClass="form-control"></asp:TextBox>

                    </div>
                </div>

                    <div class="col-md-3">
                    <div class="form-group">
                        <label>Host Name</label>
                        <asp:TextBox ID="txtHost"  required="true" placeholder="smtp.HostName.com"  runat="server" CssClass="form-control"></asp:TextBox>

                    </div>
                </div>
                    <div class="col-md-2">
                    <div class="form-group">
                        <label>Port No</label>
                        <asp:TextBox ID="txtPortNo"  required="true" TextMode="Number"  runat="server" CssClass="form-control"></asp:TextBox>

                    </div>
                </div>
                    <div class="col-lg-2 col-md-2">
                        <div class="form-group">
                            <label style="visibility:hidden;">Port No</label>
							<div style="height:-5px;"></div>
                            <asp:Button ID="btnsave" runat="server" Width="100%" CssClass="btn btn-primary" OnClick="btnsave_Click" Text="Save" Font-Bold="true" />
                        </div>
                    </div>
                </div>

		</div>
	</div>

</asp:Content>