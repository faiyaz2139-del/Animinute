<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="DatabaseBackup.aspx.cs" Inherits="DatabaseBackup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
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
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <div class="panel panel-default" style="margin-top: 32px">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6">
                        <h2 style="text-align: left; margin-top: 12px;">Data Backup</h2>
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
                        <asp:Button ID="btnBackup" runat="server" Width="100%" Text="Take Database Backup" CssClass="btn btn-primary "  OnClick="btnBackup_Click"/>
                    </div>
                    </div>
                </div>
         </div>
</asp:Content>

