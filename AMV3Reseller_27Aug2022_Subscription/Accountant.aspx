<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Accountant.aspx.cs" Inherits="Accountant" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            $('#GvDivAdminBussOwner').DataTable({
                
            });
            $('#GridView_PopUp_Employee_List').DataTable({

            });
            $('#GridView_PopUp_Business_List').DataTable({

            });
        });
    </script>
   
        <style>

            .modalBackground {
                background-color: Black;
                /*filter: alpha(opacity=60);*/
                opacity: 0.6;
                overflow: auto;
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
                /*font-weight: bold;*/
                margin-bottom: 5px;
                color: black !important;
                padding: 10px;
                overflow-y: scroll;
                height: 400px;
                width: auto;
            }

            .buttonhyperlink {
                /*text-align: center;
                padding-top: 10px;
                font-family: sans-serif;
                font-size: medium;*/
            
                border: 1px solid #607DF2;
                display: block;
                background-color:#607DF2 ;
                border-radius: 0px;
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
                border-radius: 0px;
                color: white ;
                padding: 10px 25px;
                text-align: center;
                text-decoration: none;
                display: block;
                margin-top: -1px;
                font-family: sans-serif;
                font-size: medium;
            }

            /*.HeaderTemplate {
                border-bottom: 2px solid black;
            }*/
        </style>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    
    <%--<script type="text/javascript" src='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.min.js'></script>
    <link media="screen" rel="stylesheet" href='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.min.css' />
    <link media="screen" rel="stylesheet" href='https://cdn.jsdelivr.net/sweetalert2/6.3.8/sweetalert2.css' />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.0/jquery.min.js" type="text/javascript"></script>--%>

    
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
    </style>
     
    <style>
        .swal2-container {
        display: -webkit-box;
        display: flex;
        position: fixed;
        z-index: 300000;
        }
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
    </style>
    <script type = "text/javascript">
        function ConfirmBox() {
            var Toggle_activate_deactivate = document.createElement("INPUT");
            Toggle_activate_deactivate.type = "hidden";
            Toggle_activate_deactivate.name = "Toggle_activate_deactivate";
            if (confirm("Do you want to Activate/Deactivate the subscriber?")) {
                Toggle_activate_deactivate.value = "Yes";
            } else {
                Toggle_activate_deactivate.value = "No";
            }
            document.forms[0].appendChild(Toggle_activate_deactivate);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupBussList" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            <asp:Label ID="lblbusName" runat="server"></asp:Label>
            <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>

        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GridView_PopUp_Business_List" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" Width="110%" AutoGenerateColumns="False" CellPadding="1"
                        ForeColor="#333333" OnRowCommand="GridView_PopUp_Business_List_RowCommand" 
                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_PopUp_Business_List_PreRender" 
                        OnRowDataBound="GridView_PopUp_Business_List_RowDataBound">
                        <AlternatingRowStyle BackColor="#f5f5f5" />

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name">
                                <ItemTemplate>
                                <asp:Label ID="LBLBusinessName" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                <asp:HiddenField ID="Bid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="Employee Count">
                                <ItemTemplate>
                                <asp:Label ID="LBLempcnt" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "empcnt")%>'></asp:Label>
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
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modelpopemp" BehaviorID="mpe1" runat="server"
        PopupControlID="pnlPopupemp" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide1">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopupemp" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            <asp:Label ID="lblempna" runat="server"></asp:Label>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide1" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>

        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GridView_PopUp_Employee_List" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" Width="110%" AutoGenerateColumns="False" CellPadding="1"
                        ForeColor="#333333" OnRowCommand="GridView_PopUp_Employee_List_RowCommand" 
                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_PopUp_Employee_List_PreRender" 
                        OnRowDataBound="GridView_PopUp_Employee_List_RowDataBound">
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
                             <asp:TemplateField HeaderText="User Role">
                                <ItemTemplate>
                                <asp:Label ID="LBLUserType" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
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
    </asp:Panel>


        <%--<div class="panel panel-default" >
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-6 col-sm-6 col-xs-6">
                        <h2 style="text-align: left; margin-top: 12px;">Subscriber List</h2>
                    </div>
                    <div class="col-md-6 col-sm-6 col-xs-6">
                        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Add Subscriber" NavigateUrl="~/Register_As_A_Bo_From_Reseller.aspx"
                            class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                    </div>
                </div>
            </div>
        </div>--%>

     <div style="position: relative">
        <div class="row">
            <div class="col-md-4 col-sm-4 col-xs-4">
                <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Add Subscriber" style="font-size:large;" NavigateUrl="~/Register_As_A_Bo_From_Reseller.aspx"></asp:HyperLink>
            </div>
            <div class="col-md-4 col-sm-4 col-xs-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Subscriber List" runat="server"/>
            </div>
        </div>
    </div>

        <div class="panel panel-default">
            <div class="panel-body">
                <div runat="server" id="Panel_Check_If_User_As_A_Partner_Exists">
                <div class="row">
                    <div class="col-md-12">
                        <div class="form-group" >
                            <div class="row">
                                <div class="col-md-1"></div>
                                <div class="col-md-1" style="margin-top:10px;">
                                    <label  for="Name">Status:</label>
                                </div>
                                <div class="col-md-4" >
                                    <asp:DropDownList ID="ddl_status" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_status_SelectedIndexChanged"
                                        AutoPostBack="true">
                                        <asp:ListItem Value="" Selected="True">Select</asp:ListItem>
                                        <asp:ListItem Value="YES">Active</asp:ListItem>
                                        <asp:ListItem Value="NO">Not Active</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                </div>

                <div style="overflow-x: scroll;">
                    <asp:GridView ID="GvDivAdminBussOwner" runat="server" ClientIDMode="Static" Width="100%"
                        CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                        GridLines="None" EmptyDataText="No Records Found"  OnRowCommand="GvDivAdminBussOwner_RowCommand"
                        OnPreRender="GvDivAdminBussOwner_PreRender" OnRowDataBound="GvDivAdminBussOwner_RowDataBound"
                        RowStyle-VerticalAlign="Bottom" AllowSorting="true" >
                        <AlternatingRowStyle BackColor="#f5f5f5" />

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Subscriber Name" >
                                <ItemTemplate>
                                    <asp:Label ID="lblTitledivAdmin" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "FirstName")%>'></asp:Label><br />
                                    <asp:HyperLink ID="HyperLink_Edit_Subscriber" ToolTip="Edit" runat="server" NavigateUrl='<%# "~/Register_As_A_Bo_From_Reseller.aspx?EmployeeIDEdit=" + Eval("EmployeeID")%>'>
                                        Edit Businesses & Users
                                    </asp:HyperLink>
                                    </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name" Visible="false"> <%--Hidden after new requirement in July 2023 for Reseller--%>
                                <ItemTemplate>
                                    <asp:DropDownList ID="DropDownList_Business_Name" runat="server" AppendDataBoundItems="true" CssClass="form-control xs-mmmselect">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="HiddenField_EmployeeID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="200px" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="lblCompanyName" style="word-wrap:break-word;" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName").ToString().Replace("," , ",</br>")%>' ></asp:Label> <%--Font-Bold="true"--%>
                                    <asp:HiddenField ID="hfdCompanyName" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'/>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business & User Details">
                                <ItemTemplate>
                                    <asp:Label ID="Label_Business_Count" runat="server" Font-Bold="true" Text='<%#DataBinder.Eval(Container.DataItem, "bussinessCount")%>'></asp:Label> |
                                    <asp:LinkButton ID="lnkBusinessList" Visible="true" runat="server" Text="Business List" CommandName="bussList" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("EmployeeID")%>'></asp:LinkButton>
                                    <br />
                                    <%--<asp:LinkButton ID="LinkButton_Employee_Count" runat="server" Text="Count" CommandName="empList_activeonly" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:LinkButton>--%>
                                    <asp:Label ID="LinkButton_Employee_Count" runat="server" Font-Bold="true" Text="Count"></asp:Label> |
                                    <asp:LinkButton ID="lnkEmployeeList" Visible="true" runat="server" Text="Employee List" CommandName="empList" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("EmployeeID")%>'></asp:LinkButton>
                                    <asp:HiddenField ID="HiddenField_EmployeeID_UserList" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'/>
                                </ItemTemplate>
                                
                               <%-- <ItemTemplate>
                                    
                                </ItemTemplate>--%>
                            </asp:TemplateField>

                            <%--<asp:TemplateField HeaderText="User List"> 
                                
                            </asp:TemplateField>--%>

                            <asp:TemplateField HeaderText="Contact Number">
                                <ItemTemplate>
                                    <asp:Label ID="lblUrldivAdmin" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainPhone")%>'></asp:Label> <%--Font-Bold="true"--%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Email Id">
                                <ItemTemplate>
                                    <asp:Label ID="EMail_primarydivAdmin"  runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainEmail")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Create Date" Visible="false">
                                <ItemTemplate>
                                    <asp:Label ID="Created_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle CssClass="text-left" />
                            </asp:TemplateField>

                           <%--<asp:TemplateField HeaderText="Owner Status" Visible="true">--%>
                            <asp:TemplateField HeaderText="Status" Visible="true">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnactive" runat="server" OnClientClick="ConfirmBox()" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+ Eval("EmployeeID")%>' CommandName="status" />
                                    <asp:HiddenField ID="hfdStatusForAdminBussOwner" runat="server"  Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
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
</asp:Content>

