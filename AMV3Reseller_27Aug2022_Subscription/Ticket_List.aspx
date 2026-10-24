<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Ticket_List.aspx.cs" Inherits="Ticket_List" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">

    <style>
        .btn2 {
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
        .btn2:hover {
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
    </style>
    <script>
const { Script } = require("vm");

        $(document).ready(function () {
            $('#GVTicketList').DataTable({
            });
            $('#GridView_Comments_List').DataTable({
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



    <style>

        .modalBackground {
            background-color: Black;
            /*filter: alpha(opacity=60);*/
            opacity: 0.6;
            overflow: auto;
        }

        .modalPopup_viewcomment {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup_viewcomment .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup_viewcomment .body {
            min-height: 50px;
            line-height: 30px;
            /*font-weight: bold;*/
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            /*overflow-y: scroll;*/
            /*height: 370px;*/
            overflow-y: scroll;
            height: 400px;
            width: auto;
            overflow-x: auto;
        }
    </style>

    <script type="text/javascript">
        var submit_click_count = 0;
        function Prevent_Double_Click() {
            if (++submit_click_count > 1) {
                alert('Already procesing. Please be patient.');
                return false;
            }
        }
    </script>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

     <div class="panel panel-default" id="cardbus" style="display:none;">
        <div class="panel-body">
            <%--<h2 style="text-align:left;margin-top:5px;">Ticket List</h2>--%>
            <h2 style="text-align:left;margin-top:5px;">Ticket Details</h2>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
                <br /><br />
                <a href="Raise_Ticket_By_User.aspx" id="A2" runat="server">
                    Create Ticket
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Ticket Details" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
                 <div class="panel-body">

                     <div class="row">
                         <div class="col-md-3">
                             <div class="form-group">
                                 <label>From Date</label><span>*</span>
                                 <asp:TextBox ID="txtfromDate" required="true" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-3">
                             <div class="form-group">
                                 <label>To Date</label><span>*</span>
                                 <asp:TextBox ID="txttodate" required="true" TextMode="Date" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-3">
                             <div class="form-group">
                                 <label>&nbsp;</label><%--<br />--%>
                                 <asp:Button ID="btnShow" runat="server" Text="View" OnClick="btnShow_Click" CssClass="btn2 btn-primary" />
                             </div>
                         </div>
                         <div class="col-md-3" id="mother" runat="server">
                              <div class="form-group">
                                 <label>Mother Organization</label>
                                 <asp:DropDownList ID="ddlmother" runat="server"  AppendDataBoundItems="true" CssClass="form-control"></asp:DropDownList>
                             </div>
                         </div>
                         
                    </div>

                    <div class="row" style="display:none;">
                         
                    </div>
                     <div class="row">
                         
                          <div class="col-md-4">
                             <div class="form-group">
                                <asp:Button ID="btnAdd" Visible="false" runat="server" style="margin-bottom:-12px;"
                                    Text="Add Time Sheet" formnovalidate="true" CssClass="btn btn-primary" />
                              </div>
                         </div>
                     </div>
    </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x:scroll;">
                    <asp:GridView ID="GVTicketList" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                    AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVTicketList_PreRender" OnRowCommand="GVTicketList_RowCommand"
                    OnRowDataBound="GVTicketList_RowDataBound">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>

                        <asp:TemplateField HeaderText="Sl. No.">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                                <asp:HiddenField ID="hfd_TicketId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TicketId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Ticket No">
                            <ItemTemplate>
                                <%--<asp:Label ID="lbl_TicketNo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TicketNo")%>'></asp:Label>--%>
                                <asp:Label ID="lbl_TicketNo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "TicketId")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>                        

                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email Id">
                            <ItemTemplate>
                                <asp:Label ID="Lbl_EmailId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Phone No">
                            <ItemTemplate>
                                <asp:Label ID="lbl_PhoneNo" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PhoneNO")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Problem Type" Visible="false" >
                            <ItemTemplate>
                                <asp:Label ID="lbl_ProblemType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProblemType")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Module" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Module" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Module")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Attachment" >
                            <ItemTemplate>
                                <a href='<%# Eval("FileUploadName","../images/{0}") %>' target="_blank"><i class="fa fa-download" style="font-size:20px;"></i></a>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Issue Description" ItemStyle-Width="10%" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Description" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Description")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Action" Visible="false" >
                            <ItemTemplate>
                                <asp:LinkButton ID="btneditaction" runat="server" CssClass="btn"
                                    data-original-title="Edit" data-toggle="tooltip" >
                                                                            <i class="fa fa-edit"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="btnSendAction" runat="server" CssClass="btn"
                                    data-original-title="Send For Approval" data-toggle="tooltip" >
                                                                            <i class="fa fa-paper-plane" aria-hidden="true"></i>
                                </asp:LinkButton>

                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkbtn_Resolve_" runat="server" ForeColor="Green" CssClass="btn"
                                    data-original-title="Click to Change to Resolved Status" data-toggle="tooltip" ToolTip="Click to Change to Resolved Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Resolved">
                                                                            <i class="fa fa-thumbs-up fa-lg"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="lnkbtn_On_Going_" runat="server" ForeColor="Yellow" CssClass="btn"
                                    data-original-title="Click to Change to Ongoing Status" data-toggle="tooltip" ToolTip="Click to Change to In Progress Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="On_Going">
                                                                            <i class="fa fa-thumbs-up fa-lg"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="lnkbtn_Pending_" runat="server" ForeColor="Red" CssClass="btn"
                                    data-original-title="Click to Change to Pending Status" data-toggle="tooltip" ToolTip="Click to Change to Recjeted Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Pending">
                                                                            <i class="fa fa-thumbs-up fa-lg" aria-hidden="true"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Comments">
                            <ItemTemplate>
                                <asp:LinkButton ID="LinkButton_AddComment" runat="server" Text="Add" 
                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("TicketId") %>' CommandName="AddComment"></asp:LinkButton> <b>|</b>
                                <asp:LinkButton ID="LinkButton_ViewComment" runat="server" Text="View" 
                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("TicketId") %>' CommandName="ViewComment"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Comments" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Comments" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "DeveloperComment")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Ticket Status" Visible="false">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkbtn_Resolve" runat="server" ForeColor="Green" CssClass="btn"
                                    data-original-title="Click to Change to Resolved Status" data-toggle="tooltip" ToolTip="Click to Change to Resolved Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Resolved">
                                                                            <i class="fa fa-thumbs-up fa-lg"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="lnkbtn_On_Going" runat="server" ForeColor="Yellow" CssClass="btn"
                                    data-original-title="Click to Change to Ongoing Status" data-toggle="tooltip" ToolTip="Click to Change to In Progress Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="On_Going">
                                                                            <i class="fa fa-thumbs-up fa-lg"></i>
                                </asp:LinkButton>

                                <asp:LinkButton ID="lnkbtn_Pending" runat="server" ForeColor="Red" CssClass="btn"
                                    data-original-title="Click to Change to Pending Status" data-toggle="tooltip" ToolTip="Click to Change to Recjeted Status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Pending">
                                                                            <i class="fa fa-thumbs-up fa-lg" aria-hidden="true"></i>
                                </asp:LinkButton>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label ID="lbl_StatusType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StatusType")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Change Status">
                            <ItemTemplate>
                                <asp:DropDownList
                                    runat="server" ID="DropDownList_Change_Status" Width="140px"
                                    CssClass="form-control xs-mselect" AutoPostBack="true"
                                    OnSelectedIndexChanged="DropDownList_Change_Status_SelectedIndexChanged">
                                    <asp:ListItem Value="0" Text="Please Select"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Resolved"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="In Progress"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="Pending"></asp:ListItem>
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <%--<EmptyDataTemplate>
                        <div>No records found.</div>
                    </EmptyDataTemplate>--%>

                </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>



    <%-- Model Pop Start for Add Comment --%>
    <div id="MyPopup" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">
                    &times;</button>
                <h4 class="modal-title">
                </h4>
            </div>
            <div class="modal-body">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-danger" data-dismiss="modal">
                Close</button>
            </div>
        </div>
    </div>
    </div>
    <asp:LinkButton ID="lnkDummy2" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modaladdcomment" BehaviorID="mpe2" runat="server"
        PopupControlID="pnladdproject" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide12">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnladdproject" runat="server" CssClass="modalPopup_viewcomment" Style="display: none;">
        <div class="header">
            <asp:Label ID="lblticketid" runat="server"></asp:Label>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide12" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
            <div class="body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label>Comment</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtAddComment" MaxLength="4000" Width="400px" Height="250px" ClientIDMode="Static"
                                    runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                                <asp:HiddenField ID="hfdticketautoid" runat="server" />
                                <asp:HiddenField ID="HiddenField_Name" runat="server" />
                                <asp:HiddenField ID="HiddenField_Email" runat="server" />
                                <asp:HiddenField ID="HiddenField_Phone" runat="server" />
                                <asp:HiddenField ID="HiddenField_EmpId" runat="server" />
                                <asp:HiddenField ID="HiddenField_FileUploadName" runat="server" />
                                <asp:Label Text="Please Enter Comments" ForeColor="Red" Visible="false" ID="Label_Warning_Message" runat="server" />
                            </div>
                        </div>          
                    </div>
                    
                    <div class="row">
                        <div class="col-md-12">                         
                            <div class="form-group">
                                <center>
                                    <asp:Button runat="server" ID="btnAddComment" ClientIDMode="Static" Text="Save" CssClass="btn2" OnClick="btnAddComment_Click" OnClientClick="return Prevent_Double_Click();" />
                                </center>
                            </div>
                        </div>
                    </div>
                 
            </div>
                                
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAddComment" EventName="Click"/>                                        
                </Triggers>
        </asp:UpdatePanel>                
    </asp:Panel>
    <%-- Model Pop End for Add Comment --%>



    <%-- Model Pop Start for View Comment --%>
    <div id="MyPopup_viewcomment" class="modal fade" role="dialog">
    <div class="modal-dialog">
        <!-- Modal content-->
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">
                    &times;</button>
                <h4 class="modal-title">
                </h4>
            </div>
            <div class="modal-body">
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-danger" data-dismiss="modal">
                Close</button>
            </div>
        </div>
    </div>
    </div>
    <asp:LinkButton ID="lnkDummy3" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modalviewcomment" BehaviorID="mpe3" runat="server"
        PopupControlID="pnlviewproject" TargetControlID="lnkDummy3" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide13">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlviewproject" runat="server" CssClass="modalPopup_viewcomment" Style="display: none;">
        <div class="header">
            <asp:Label ID="lblticketid2" runat="server"></asp:Label>
            <asp:HiddenField ID="HiddenField2" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide13" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    
            <div class="body">

                    <div class="row">
                        <div class="col-md-12">
                            <asp:HiddenField ID="hfdticketautoid2" runat="server" />
                            <asp:GridView ID="GridView_Comments_List" Width="700px" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                AutoGenerateColumns="False" OnPreRender="GridView_Comments_List_PreRender" OnRowDataBound="GridView_Comments_List_RowDataBound">
                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                    <Columns>

                        <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Date" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Comments_Date" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CommentsDate")%>' />
                                <asp:Label ID="lbl_Comments_DateOnly" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CommentsDateOnly")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Description" ItemStyle-Width="40%">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Comments_Comments" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Comments")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Comments By" ItemStyle-Width="30%">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Comments_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CommentsBy")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Email Id" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="Lbl_Comments_EmailId" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Phone No" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbl_Comments_PhoneNO" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PhoneNO")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                    <%--<EmptyDataTemplate>
                        <div>No records found.</div>
                    </EmptyDataTemplate>--%>

                </asp:GridView>
                        </div>          
                    </div>
                 
            </div>
                                
                </ContentTemplate>
        </asp:UpdatePanel>                
    </asp:Panel>
    <%-- Model Pop End for View Comment --%>

</asp:Content>