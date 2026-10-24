<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CreateLink.aspx.cs" Inherits="CreateLink" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script >
        $(document).ready(function () {
            $('#GVCreateLinkList').DataTable({
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

    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.11.2/css/all.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

     <div class="panel panel-default" style="margin-top: 28px">
        <div class="panel-body">
            <h2 style="text-align:left;margin:10px;">Create Link</h2>
        </div>
      </div>

            <div class="panel panel-default">
                 <div class="panel-body" style="margin-left:20px;">

                       <div class="row">
                         <div class="col-lg-4 col-md-4" style="display:none;">
                           <div class="form-group">
                              <label>Create Link For</label>
                                <asp:DropDownList ID="ddlLinkFor" runat="server" required="true" CssClass="form-control">
                                                    <asp:ListItem Value="" Text="Please Select">Please Select</asp:ListItem>
                                                    <asp:ListItem Value="Admin" Text="Admin">Admin</asp:ListItem>
                                                    <asp:ListItem Value="Teacher" Text="Teacher">Teacher</asp:ListItem>
                                                    <asp:ListItem Value="Student" Text="Student">Student</asp:ListItem>
                                 </asp:DropDownList>
                            </div>
                         </div>
                         <div class="col-md-4 col-lg-4">
                              <div class="form-group">
                                 <label>Domain Name</label>
                                 <asp:DropDownList ID="ddlDomainName" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlDomainName_SelectedIndexChanged" AppendDataBoundItems="true" AutoPostBack="true"></asp:DropDownList>
                                 <asp:HiddenField ID="hdfeditid" ClientIDMode="Static" runat="server" />
                                 <asp:Label ID="lbl" runat="server" ClientIDMode="Static"></asp:Label>
                              </div>
                         </div>
                         <div class="col-md-4 col-lg-4">
                          <div class="form-group">
                              <label>Parent Menu</label>
                              <asp:DropDownList ID="ddlParent" CssClass="form-control" runat="server">
                                <asp:ListItem Value="0" Text="Please Select">Please Select</asp:ListItem>
                              </asp:DropDownList>
                              <asp:HiddenField ID="hfdMenuidEdit" runat="server" />
                              <asp:HiddenField ID="hfdMastno" runat="server" />
                         </div>
                         </div>
                          <div class="col-md-4 col-lg-4">
                              <div class="form-group">
                                 <label>Menu Name</label>
                                 <asp:TextBox ID="txtname" required="true" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                    </div>

                     <div class="row">
                         <div class="col-md-4 col-lg-4">
                            <div class="form-group">
                                 <label>Url</label>
                                 <asp:TextBox ID="txtUrl" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                         </div>
                          <div class="col-lg-4 col-md-4" style="display:none;">
                             <div class="form-group">
                                  <label>Icon</label>
                                  <asp:TextBox ID="txticon" CssClass="form-control" runat="server"></asp:TextBox>
                              </div>
                          </div>
                          <div class="col-md-4 col-lg-4">
                              <div class="form-group">
                                 <label>Menu SlNo</label>
                                  <asp:TextBox ID="txtmenuslno" type="number"  step="0.01" required="true" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-4 col-lg-4">
                              <div class="form-group ">
                                 <label style="height:17px;"></label>
                                 <asp:Button ID="btnSave" runat="server" Text="Save" Width="100%" CssClass="btn btn-success" Font-Bold="True"
                                      ValidationGroup="add" OnClick="btnSave_Click" />
                                  <br />
                              </div>
                         </div>
                     </div>

                     <div style="height:25px;"></div>

                      <div class="row">

                               <asp:GridView ID="GVCreateLinkList" runat="server" ClientIDMode="Static"
                                                                   CssClass="table table-striped table-bordered nowrap"
                                                                    EmptyDataText="No Records Found"
                                                                    AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333"
                                                                    GridLines="None" OnPreRender="GVCreateLinkList_PreRender"
                                                                    >
                                                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                    <HeaderStyle CssClass="bg-clouds" />

                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Sl. No.">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Domain Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDomain" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DomainName")%>'></asp:Label>
                                                                                <asp:HiddenField ID="hdfDomainID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "DomainID")%>' />

                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Titile">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblTitle" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Title")%>'></asp:Label>
                                                                                <asp:HiddenField ID="hdfMenuID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MenuID")%>' />
                                                                                <asp:HiddenField ID="hfdParentMenuID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ParentMenuID")%>' />
                                                                                <asp:HiddenField ID="hfdMast" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "mastno")%>' />
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Url">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblUrl" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Url")%>'
                                                                                    Font-Bold="true"></asp:Label>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>



                                                                        <asp:TemplateField HeaderText="Link For" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblLinkType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "LinkFor")%>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle CssClass="text-left" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Menu Sl No">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblmenuslno" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "menuSlno")%>'></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle CssClass="text-left" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Action">
                                                                            <ItemTemplate>

                                                                                <asp:LinkButton ClientIDMode="Static"  ID="btnDelete" runat="server" OnClick="btnDelete_Click" data-id='<%# Eval("MenuID") %>' CssClass="btn"><%--data-original-title="Delete" data-target="#DeleteConfirm" data-toggle="modal"--%>
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

                                 <script>
                                     $(document).ready(function () {
                                         $("#btnDelete").click(function () {
                                             var a = $(this).attr("data-id")
                                             $("#hdfeditid").val(a);
                                             $("#lbl").text(a);

                                         });
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
													<asp:Button ID="btnyes" OnClick="btnyes_Click" formnovalidate="true"  runat="server" Text="Yes" class="btn btn-success"/>
													<button type="button" class="btn btn-danger" data-dismiss="modal">Close</button>
												</div>
											</div>
										</div>
									</div>

                    </div>

          </div>
    </div>


</asp:Content>

