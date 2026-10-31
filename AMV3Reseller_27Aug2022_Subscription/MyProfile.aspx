<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="MyProfile.aspx.cs" Inherits="MyProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
      <style>
        .buttonlink {
            text-align: center;
            /*margin-top:10px;*/
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonlink:hover {
            background-color: #607DF2;
            border-radius: 1px;
            border: none;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink {
            text-align: center;
            margin-top: 10px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink:hover {
            background-color: #607DF2;
            border-radius: 1px;
            border: none;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            
            border: 1px solid #607DF2;
            display: block;
            background-color:white ;
            border-radius: 1px;
            color: #607DF2;
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
            background-color:white ;
            border-radius: 1px;
            color: #607DF2;
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
        $(document).ready(function () {
            $('#GvBusiness').DataTable(
                {
                });            
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="text-align: left; margin-top: 12px;">My Profile</h2>
                    <%-- v39: My Profile > Login & Security > Change Login Email --%>
                    <p style="margin:4px 0 12px"><a href="LoginSecurity.aspx" class="btn btn-primary" style="border-radius:7px">Login &amp; Security &rsaquo; Change Login Email</a></p>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <a href="Home_Page.aspx" id="btnback" runat="server" visible="true" class="buttonhyperlink"
                        style="margin-top: 12px; float: right;">Home</a>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center;margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="My Profile" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default" id="pnlbody">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <div style="overflow-x: scroll;">

                    <asp:GridView ID="GvBusiness" runat="server" ClientIDMode="Static" Width="100%"
                        CssClass="table nowrap FS14" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                        GridLines="None" EmptyDataText="No Records Found" OnRowDataBound="GvBusiness_RowDataBound"
                        OnPreRender="GvBusiness_PreRender" 
                        RowStyle-VerticalAlign="Bottom">
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <AlternatingRowStyle BackColor="#f5f5f5" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblBusinessName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BussinessName")%>'
                                        ToolTip='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>'></asp:Label>
                                    <asp:HiddenField ID="hdfEmployeeID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Owner">
                                <ItemTemplate>
                                    <asp:Label ID="lblDomaindivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BussinessOwner")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblTitledivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainEmail")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblUrldivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainPhone")%>'
                                        Font-Bold="true"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="User Type">
                                <ItemTemplate>
                                    <asp:Label ID="lblUserType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
                                    <asp:HiddenField ID="hfdUserType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "UserTypeid")%>' />
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hypditdivAdmin" ToolTip="Edit" runat="server" NavigateUrl='<%# "~/AddBusinessOwner.aspx?EmployeeIDEdit=" + Eval("EmployeeID")%>'>
                                                           View/Edit Profile
                                    </asp:HyperLink>
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