<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Billing_Info.aspx.cs" Inherits="Billing_Info" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            $('#GridView_Billing_Info').DataTable({
                
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
        .modalBackground {
            background-color: Black;
            opacity: 0.6;
        }

        .modalPopup {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup .body {
            min-height: 50px;
            line-height: 30px;
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            overflow-y: scroll;
            height: 350px;
            width: auto;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="text-align: left; margin-top: 12px;">Billing Info</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="SuperAdmin_Approve_Subscriber.aspx" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Billing Info" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="SuperAdmin_Approve_Subscriber.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>
    
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-lg-2 col-md-2" style="text-align: right; margin-top: 08px;">
                    <div class="form-group">
                        <label for="Name">Subscriber Name</label>
                    </div>
                </div>
                <div class="col-lg-4 col-md-4">
                    <div class="form-group">
                        <%--<label for="Name">Subscriber Name</label>--%>
                        <asp:DropDownList ID="ddl_Subscriber_Name" runat="server" CssClass="form-control js-example-basic-single" Width="100%" 
                            AppendDataBoundItems="true" OnSelectedIndexChanged="ddl_Subscriber_Name_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-lg-4 col-md-4">
                </div>
            </div>
            <div class="row">
                <asp:Panel ID="Panel1" runat="server">
                <div class="col-lg-12 col-md-12">
                    <div style="overflow-x: scroll;">
                        <asp:GridView ID="GridView_Billing_Info" ClientIDMode="Static" runat="server" CssClass="table nowrap"
                            CellPadding="1" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" 
                            EmptyDataText="No records Found" OnPreRender="GridView_Billing_Info_PreRender"
                             OnRowDataBound="GridView_Billing_Info_RowDataBound">
                            <AlternatingRowStyle BackColor="#f5f5f5" />
                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Subscription Date">
                                    <ItemTemplate>                                        
                                        <asp:Label ID="Label_Subscription_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>'></asp:Label>
                                        <asp:HiddenField ID="Label_Row_UniqueId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Id")%>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Subscriber Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Subscriber_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubName")%>'></asp:Label>
                                        <asp:HiddenField ID="Label_Subscriber_EmpId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmpId")%>'/>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Email">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Email_Id" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmailId")%>' />
                                        <asp:HiddenField ID="HiddenField_PlanId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PlanId")%>'/>                                        
                                        <asp:HiddenField ID="HiddenField_Price_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PriceId")%>' />                                        
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Plan Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Plan_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PlanName")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No Of Business">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_No_Of_Business" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Business")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                 <asp:TemplateField HeaderText="No Of Users">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_No_Of_Users" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "No_Of_Users")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Amount Paid">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Amount_Paid" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "AmountPaid")%>' />
                                        <asp:HiddenField ID="HiddenField_Transactuon_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TransId")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>                             

                                <asp:TemplateField HeaderText="Subscription Start Date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Start_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Subscription Renewal Date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_End_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="HiddenField_IsActive_Status" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
                                        <asp:HyperLink ID="HyperLink_Edit_Plan" runat="server" Text="Edit" NavigateUrl='<%# "~/Approve_Plan.aspx?SubscriberFromBillingPage=" + Eval("EmpId")%>'></asp:HyperLink>

                                        <asp:HiddenField ID="HiddenField_Is_Requested_Plan" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Is_Requested_Plan")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            
                             <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                        </asp:GridView>
                    </div>
                </div>
                    </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>

