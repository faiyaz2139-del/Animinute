<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" 
    CodeFile="SuperAdmin_Approve_Partner.aspx.cs" Inherits="SuperAdmin_Approve_Partner" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            $('#GridView_Partner_List').DataTable({
                
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

        .buttonhyperlink {
            /*text-align: center;
                padding-top: 10px;
                font-family: sans-serif;
                font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 30px;
            color: white;
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
            background-color: #607DF2;
            border-radius: 30px;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>

     <style>
        .switch {
            position: relative;
            display: inline-block;
            width: 125px;
            height: 34px;
        }

        .switch input {
            display: none;
        }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            /*background-color: #ca2222;*/
            background-color: #949494;
            -webkit-transition: .4s;
            transition: .4s;
            border-radius: 34px;
        }

        .slider:before {
            position: absolute;
            content: "";
            height: 26px;
            width: 26px;
            left: 4px;
            bottom: 4px;
            background-color: white;
            -webkit-transition: .4s;
            transition: .4s;
            border-radius: 50%;
        }

        input:checked + .slider {
            /*background-color: #2ab934;*/
            background-color: #607DF2;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(125px);
            -ms-transform: translateX(125px);
            transform: translateX(91px);
        }

        /*------ ADDED CSS ---------*/
        .slider:after {
            content: 'Not Active';
            color: white;
            display: block;
            position: absolute;
            transform: translate(-50%,-50%);
            top: 50%;
            left: 50%;
            font-size: 10px;
            font-family: Verdana, sans-serif;
        }

        input:checked + .slider:after {
            content: 'Active';
        }

        .modalBackground {
            background-color: Black;
            filter: alpha(opacity=60);
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
            /*font-weight: bold;*/
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            overflow-y: scroll;
            height: 350px;
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

        /*.HeaderTemplate {
            border-bottom: 2px solid black;
        }*/
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <script type = "text/javascript">
        function ConfirmStatusChangeBox() {
            var Toggle_activate_deactivate = document.createElement("INPUT");
            Toggle_activate_deactivate.type = "hidden";
            Toggle_activate_deactivate.name = "Toggle_activate_deactivate";
            if (confirm("Do you want to Activate?")) {
                Toggle_activate_deactivate.value = "Yes";
            } else {
                Toggle_activate_deactivate.value = "No";
            }
            document.forms[0].appendChild(Toggle_activate_deactivate);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" id="divPartner" runat="server">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6">
                    <h2 style="text-align: left; margin-top: 12px;">Reseller List</h2>
                </div>
                <div class="col-md-6">
                    <asp:HyperLink ID="HyperLink2" runat="server" Text="Company List" NavigateUrl="~/SuperAdmin_Approve_New_Account.aspx"
                        class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                </div>
            </div>

        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body" id="div3" runat="server">
            <div class="row">
                <asp:Panel ID ="Panel2" runat="server">
                <div class="col-lg-12 col-md-12">
                    <div style="overflow-x: scroll;">
                        <asp:GridView ID="GridView_Partner_List" ClientIDMode="Static" runat="server" CssClass="table nowrap"
                            CellPadding="1" ForeColor="#333333" GridLines="None" AllowSorting="true"
                              OnPageIndexChanging="GridView_Partner_List_PageIndexChanging" AutoGenerateColumns="False"
                            EmptyDataText="No records Found" OnPreRender="GridView_Partner_List_PreRender"
                            OnRowCommand="GridView_Partner_List_RowCommand" OnRowDataBound="GridView_Partner_List_RowDataBound" >
                            <AlternatingRowStyle BackColor="#f5f5f5" />
                            <%--<HeaderStyle CssClass="bg-clouds" />--%>
                            <Columns>
                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reseller Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Partner_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PartnerName")%>'></asp:Label>
                                        <asp:HiddenField ID="HiddenField_Partner_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PartnerID")%>' />
                                        <asp:HiddenField ID="HiddenField_HireDate" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>' />
                                        <asp:HiddenField ID="HiddenField_EMail_primary" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EMail_primary")%>' />
                                        <asp:HiddenField ID="HiddenField_MobilePhone" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MobilePhone")%>' />
                                        <asp:HiddenField ID="HiddenField_ProfilePic" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "StreetNumber")%>' />
                                        <asp:HiddenField ID="HiddenField_CompanyName" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Title")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Mobile__Phone" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MobilePhone")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Email ID">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_EMail_Primary" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EMail_primary")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Enrolled Date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Hire__Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HireDate")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton_Active_Deactive_Partner" runat="server" OnClientClick="ConfirmStatusChangeBox()" ImageUrl="~/images/deactive.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Active_Deactive_Partner" />
                                        <asp:HiddenField ID="HiddenField_Partner_IsActive_Status" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
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