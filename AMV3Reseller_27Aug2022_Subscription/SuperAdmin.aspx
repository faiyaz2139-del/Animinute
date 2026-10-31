<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="SuperAdmin.aspx.cs" Inherits="SuperAdmin" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        // v35 (DOCX image7): DataTables search/pager removed here; the shared list toolbar (Search this list + Status + Clear)
        // is the only filter, so the grid no longer shows two search boxes.
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
            /*filter: alpha(opacity=60);*/
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
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <link href="css/AnyminuteSupport.css?v=20261031v35" rel="stylesheet" />
    <style>.am-sa-list .am-grid-wrap{border:0}.am-sa-list .am-sa-grid{margin:0}.am-sa-list .am-sa-grid td a{font-size:12px}.am-sa-list .am-filter{margin-bottom:10px}.am-sa-list .sa-head h1{font-size:24px}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

    <%-- v35 (DOCX image7): Accountant List uses the Support Admin card/table design (sa-page / sa-head / sa-card / sa-table). --%>
    <div class="sa-page am-sa-list">
    <div class="sa-head" id="divAddAcc" runat="server">
        <div><h1>Accountant List</h1><p>Accountants and payroll agencies, their businesses and login status.</p></div>
        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="+ Add Accountant" NavigateUrl="~/MotherOrg.aspx" CssClass="sa-btn"></asp:HyperLink>
    </div>
    <div class="sa-card">
        <div id="divAddgrid" runat="server">
                    <div class="sa-table-wrap">
                        <asp:GridView ID="GVMother" ClientIDMode="Static" runat="server" CssClass="table sa-table am-sa-grid"
                            CellPadding="1" ForeColor="#333333" GridLines="None" AllowSorting="true"
                             OnPageIndexChanging="GVMother_PageIndexChanging" AutoGenerateColumns="False"
                            EmptyDataText="No records Found" OnPreRender="GVMother_PreRender" OnRowDataBound="GVMother_RowDataBound" OnRowCommand="GVMother_RowCommand">
                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Accountant Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MotherOrgName")%>'></asp:Label>
                                        <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MotherOrgID")%>' />
                                        <br />
                                        <asp:HyperLink ID="btnedit" runat="server" NavigateUrl='<%# "~/MotherOrg.aspx?MotherOrgId=" + Eval("MotherOrgID")%>'
                                            Text="Update" data-original-title="Edit" data-toggle="tooltip">                                                                                
                                        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Business Owner">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBusinessOwner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "bussOwnerCnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No. of Business">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBusinesscnt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "busscnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No. of Employee">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeescnt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empcnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                               
                                <asp:TemplateField HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContactName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ContactNumber")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Email ID">
                                    <ItemTemplate>
                                        <asp:Label ID="LblEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Enrolled Date">
                                    <ItemTemplate>
                                        <asp:Label ID="LblEnrlDt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EnrollDate1")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%-- v32: Password column removed; passwords are never shown in plain text in list UIs. --%>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <%--<label class="switch">--%>
                                        
                                        <%--<asp:CheckBox ID="togBtnForAdminBussOwner" ClientIDMode="Static" runat="server" />--%>
                                        <asp:ImageButton ID="btnactive" runat="server" ImageUrl="~/images/active.png" Width="96px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="status" />
     
                                        <%--<div class="slider round"></div>--%>
                                   <%-- </label>--%>
                                        <asp:HiddenField ID="hfdStatusForAdminBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                

                            </Columns>
                            
                             <PagerStyle   HorizontalAlign = "Right" CssClass = "GridPager" />

                        </asp:GridView>
                    </div>
        </div>
    </div>
    </div>

</asp:Content>
