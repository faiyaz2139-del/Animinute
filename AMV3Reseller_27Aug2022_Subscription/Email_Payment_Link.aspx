<%@ Page Title="" Language="C#" MasterPageFile="~/MasterWithoutMenu.master" AutoEventWireup="true" CodeFile="Email_Payment_Link.aspx.cs" Inherits="Email_Payment_Link" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <script>
        $(document).ready(function () {
            $('#GridView_OnlinePaymentList').DataTable({
            });
        });
    </script>

    <style>
        .buttonhyperlink {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
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
            background-color: white;
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
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <asp:Image ID="Image1" runat="server" Height="50px" Width="50px" ImageUrl="~/Assets/assets/img/logo.jpg" Style="padding-top: 10px;" />
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6 text-right">
                    <a href="Login.aspx" id="btnback" runat="server" class="buttonhyperlink" style="margin-top: 10px; float: right;">Home</a>
                </div>
            </div>
        </div>
    </div>    

    <div class="panel panel-default">
        <div class="panel-body">

            <asp:GridView ID="GridView_OnlinePaymentList" runat="server" ClientIDMode="Static"
                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_OnlinePaymentList_PreRender">
                <AlternatingRowStyle BackColor="#f5f5f5" />
                <Columns>
                    <asp:TemplateField HeaderText="Sl. No.">
                        <ItemTemplate>
                            <%#Container.DataItemIndex + 1%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Email Id">
                        <ItemTemplate>
                            <asp:Label ID="Label_Email_Id" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Name">
                        <ItemTemplate>
                            <asp:Label ID="Label_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Name")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Payment Link In Mail">
                        <ItemTemplate>
                            <asp:HyperLink ID="HyperLink_Payment_Link_In_Mail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Payment_Link")%>'
                                NavigateUrl='<%#Eval("Payment_Link")%>' Target="_blank" ></asp:HyperLink>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Payment Amount">
                        <ItemTemplate>
                            <asp:Label ID="Label_Payment_Amount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Payment_Amount")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Reseller/Subscriber">
                        <ItemTemplate>
                            <asp:Label ID="Label_Reseller_Or_Subscriber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Reseller_Or_Subscriber")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataTemplate>
                    <div>No records found.</div>
                </EmptyDataTemplate>
            </asp:GridView>
            
        </div>
    </div>

</asp:Content>