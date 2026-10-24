<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ReSellerList.aspx.cs" Inherits="ReSellerList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            $('#gvEmplistPop').DataTable({

            });

        });
    </script>
    <style>
        .GridPager a, .GridPager span {
            display: block;
            height: 35px;
            width: 35px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            padding: 10px;
        }

        .GridPager a {
            background-color: #fff;
            color: #969696;
            border: 1px solid #dedede;
        }

        .GridPager a:hover {
            color: #337ab7;
        }

        .GridPager span {
            background-color: #337ab7;
            color: #fff;
            border: 1px solid #dedede;
        }
    </style>

    <style>
        .buttonhyperlink2 {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 0px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink2:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 0px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 0px;
            color: white;
            padding: 5px 19px;
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
            background-color: #607DF2;
            border-radius: 0px;
            color: white;
            padding: 5px 19px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>
    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <script type="text/javascript">
        function ConfirmStatusChangeBox() {
            var Toggle_activate_deactivate = document.createElement("INPUT");
            Toggle_activate_deactivate.type = "hidden";
            Toggle_activate_deactivate.name = "Toggle_activate_deactivate";
            if (confirm("Would you like to activate/deactivate this user?")) {
                Toggle_activate_deactivate.value = "Yes";
            } else {
                Toggle_activate_deactivate.value = "No";
            }
            document.forms[0].appendChild(Toggle_activate_deactivate);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6" style="text-align: left; margin-top: 17px;">
                    <asp:Label ID="lblTitle_" runat="server" Font-Size="X-Large"></asp:Label>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <asp:HyperLink ID="HyperLink1" runat="server" Text="Home" NavigateUrl="~/SuperAdmin_Approve_Subscriber.aspx"
                        class="buttonhyperlink2" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-2">
                <a href="SuperAdmin_Approve_Subscriber.aspx" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-8" style="text-align: center; margin-top:-7px;">
                <asp:Label ID="lblTitle" Font-Bold="true" Font-Size="X-Large" runat="server" />
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body" id="div_New_Registered_BO" runat="server">
            <div class="row">
                <div class="col-md-2"></div>
                <div class="col-md-8">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Subscriber Name</label>
                                <asp:DropDownList ID="ddlReseller" runat="server" Width="100%" AppendDataBoundItems="true" CssClass="form-control" OnSelectedIndexChanged="ddlReseller_SelectedIndexChanged" AutoPostBack="true">
                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>&nbsp;</label>
                                <asp:Button ID="btnall" runat="server" OnClientClick="ConfirmStatusChangeBox()" Text="Deactive Subscriber" CssClass="buttonhyperlink" OnClick="btnall_Click" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <asp:GridView ID="gvEmplistPop" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="110%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowCommand="gvEmplistPop_RowCommand" 
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvEmplistPop_PreRender" 
                                OnRowDataBound="gvEmplistPop_RowDataBound">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>

                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                  <asp:TemplateField HeaderText="Business Name">
                                        <ItemTemplate>
                                        <asp:Label ID="LBLBusinessName" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "bussname")%>'></asp:Label>
                                        <asp:HiddenField ID="Bid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="Employee Name">
                                        <ItemTemplate>
                                        <asp:Label ID="LBLempnamepop" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                        <asp:HiddenField ID="eid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                     <asp:TemplateField HeaderText="User Type">
                                        <ItemTemplate>
                                        <asp:Label ID="LBLUserType" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Status">
                                    <%--<asp:TemplateField HeaderText="Business Status">--%>
                                        <ItemTemplate>
                                                <asp:HiddenField ID="hdbID" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' runat="server" />
                                                <asp:ImageButton ID="btnactiveEMP" OnClientClick="ConfirmStatusChangeBox()" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' CommandName="EMPSTUS" />
                                                <asp:HiddenField ID="hfdStatusForBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
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
                <div class="col-md-2"></div>
            </div>
        </div>
    </div>
</asp:Content>

