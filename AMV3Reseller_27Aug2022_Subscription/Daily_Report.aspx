<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Daily_Report.aspx.cs" Inherits="Report2" %>
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
	<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script>
        $(document).ready(function () {
            $('#ddlBusiness').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
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
    .dropdownlist {
                    /*text-align: center;
                    padding-top: 10px;
                    font-family: sans-serif;
                    font-size: medium;*/
            
                    border: 1px solid #607DF2;
                    display: block;
                    background-color:#ECF0F3 ;
                    border-radius: 30px;
                    color: black ;
                    padding: 10px 25px;
                    text-align: center;
                    text-decoration: none;
                    display: block;
                    margin-top: -1px;
                    font-family: sans-serif;
                    font-size: medium;
                }
    </style>
      <script>
          function ShowDocument(hdfselected, dtfrom, dtto, businessname, counter, empid, empname, type) {
              var url = "document2.aspx?hfSelected=" + hdfselected + "&dtfrom=  " + dtfrom + "&dtto=  " + dtto + "&businessname=  " + businessname + "&counter=  " + counter + "&empid=  " + empid + " &empname=  " + empname + "&type=  " + type;
              document.location.href = url;
              return false;

          }
    </script>
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
     <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" id="cardbus" runat="server" style="display:none;">
        <%--<div class="panel-body">
			<h2 style="text-align:left;margin-top:5px;">Report by Each Day</h2>
		</div>--%>
        <div class="panel-body" style="margin-top: 0px;">
            <h2 style="text-align: left; margin-top: 5px; font-weight: bold; font-size: 30px;">Daily Report</h2>
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
                <a href="Employee_Report.aspx" id="A3" runat="server">
                    Employee Report
                </a>
                &nbsp; &nbsp; &nbsp;
                <a href="Project_Report.aspx" id="A4" runat="server">
                    Project Report
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Daily Report" runat="server" />
            </div>
        </div>
    </div>

    <div class="panel panel-default" runat="server" >
        <div class="panel-body">
            <div class="row">
                 <div class="col-md-3">
                    <div class="form-group">
                        <label>Business Name</label>
                        <asp:HiddenField ID="hfSelected" runat="server" />
                                <asp:DropDownList ID="ddlBusiness" Width="100%" ClientIDMode="Static" required  runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true">
                        <asp:ListItem Text="Select" Value=""></asp:ListItem>
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
                    <div class="form-group">
                        <label>Date</label> 
                        <asp:TextBox ID="txtFromDate" TextMode="Date" required runat="server" CssClass="form-control" Max="9999-12-31" ></asp:TextBox>
                    </div>
                </div>
                <asp:HiddenField ID="hdnfrmdt" runat="server" />
           

                <div class="col-md-2">
                    <div class="form-group">
                        <label>&nbsp;</label>
                        <asp:Button ID="btnshow" runat="server" Text="Update Report" class="buttonhyperlink"  OnClick="btnshow_Click" Width="100%"/> <%--Width="100%"--%>
                    </div>
                </div>

      
                <div class="col-md-2">
                    <div class="form-group">
                        <label>&nbsp;</label>
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
                                            <asp:LinkButton ID="lnkCsvFile" runat="server" Visible="false" Text="CSV" OnClick="lnkCsvFile_Click"></asp:LinkButton></li>
                            </ul>
                        </div>
                    </div>
                </div>

            </div> 
        </div>
    </div>


    <div class="panel panel-default" runat="server" id="pnlhideandshow">
        <div class="panel-body">
            <%--<div class="row">                
            <div class="col-md-2" style="display:none;">
                    <div class="form-group">
                        <label style="margin-top:15px;"></label>
                       <asp:Button ID="btnExport" runat="server" formnovalidate="formnovalidate" Text="Export" OnClick="btnExport_Click" CssClass="btn btn-primary" 
                            Width="100%" />                        
                    </div>
                </div
            </div>>--%>

            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x: auto;">
                        <asp:GridView ID="GVBusiness" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                            AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GVBusiness_PreRender"
                            AllowPaging="true" OnPageIndexChanging="GVBusiness_PageIndexChanging">

                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                                <Columns>

                                                                                    <asp:TemplateField HeaderText="Sl. No.">
                                                                                        <ItemTemplate>
                                                                                            <%#Container.DataItemIndex + 1%>
                                                                                        </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                              <asp:TemplateField HeaderText="User Name">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="lblEmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Employee Name")%>' />
                           
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                     <asp:TemplateField HeaderText="Start Time">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Start Time")%>' />
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                     <asp:TemplateField HeaderText="End Time">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "End Time")%>' />
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>
                                                                                    <asp:TemplateField HeaderText="Gross Hours">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Gross Hours")%>' />
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>


                                                                                    <asp:TemplateField HeaderText="Break Time">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="LblBreakTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Break Time")%>' />
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>

                                                                                    <asp:TemplateField HeaderText="Net Hours">
                                                                                        <ItemTemplate>
                                                                                            <asp:Label ID="LblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Net Hours")%>' />
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


         <asp:GridView runat="server" ID="GvExport"></asp:GridView>
         <asp:GridView runat="server" ID="GvExport1"></asp:GridView>
            </div>
            
            </div>
       
</asp:Content>