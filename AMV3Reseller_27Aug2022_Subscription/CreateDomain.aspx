<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CreateDomain.aspx.cs" Inherits="CreateDomain" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script >
        $(document).ready(function () {
            $('#GVDomainList').DataTable({
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

    <div class="panel panel-default" style="margin-top:28px">
        <div class="panel-body">
            <h2 style="text-align:left;margin-top:10px;">Create Domain</h2>
        </div>
      </div>

    <div class="panel panel-default">
                 <div class="panel-body" style="margin-left:20px;">

                        <div class="row">
						 <div class="col-lg-3 col-md-3" style="display:none;">
                             <div class="form-group">
                                <label>Create Domain For</label>
                                <asp:DropDownList ID="ddlLinkFor"  CssClass="form-control" AppendDataBoundItems="true" runat="server" AutoPostBack="true">
                                </asp:DropDownList>
                              </div>
                          </div>
                         <div class="col-md-3">
                             <div class="form-group">
                                 <label for="DomainName">Domain Name</label>
                                 <asp:TextBox ID="txtDomainName" runat="server" CssClass="form-control"></asp:TextBox>
                                 <asp:HiddenField ID="hfdEditId" runat="server" />
                             </div>
                         </div>
                          <div class="col-md-3">
                              <div class="form-group">
                                 <label for="Icon">Icon</label>
                               <asp:TextBox ID="txticon" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
						  <div class="col-md-3">
                              <div class="form-group">
                                 <label for="Orderid">Order Id</label>
                                 <asp:TextBox ID="txtOrder_Id" runat="server" type="Number" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
                         </div>

                       <div class="row">
                         <div class="col-md-3">
                            <asp:Button ID="btnsave" runat="server" CssClass="btn btn-success" Text="Save" Width="100%" OnClick="btnsave_Click"/>
                         </div>
                       </div>
                        <br />

                        <div class="row" style="align-items:center;">
                          <div class="col-md-12">
                            <asp:GridView ID="GVDomainList" ClientIDMode="Static" runat="server" CssClass="table table-striped table-bordered nowrap"
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GVDomainList_PreRender"

                                                                    >
                                                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                    <HeaderStyle CssClass="bg-clouds" />

                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Sl. No.">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Domain For" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDomainFor" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DomainFor")%>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>



                                                                        <asp:TemplateField HeaderText="Domain Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DomainName")%>'></asp:Label>
                                                                                <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "DomainID")%>' />
                                                                                <asp:HiddenField ID="hdfOrderId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "OrderID")%>' />

                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>
                                                                         <asp:TemplateField HeaderText="Icon">
                                                                            <ItemTemplate>
                                                                                <i class='<%#DataBinder.Eval(Container.DataItem, "Icon")%>'></i>
                                                                                <asp:HiddenField ID="hdfIcon" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Icon")%>' />
                                                                            </ItemTemplate>

                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Action">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton  ID="btnedit" runat="server" OnClick="btnedit_Click"  CssClass="btn" data-original-title="Edit" data-toggle="tooltip" >
                                                                                <i class="fa fa-edit"></i>
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

</asp:Content>

