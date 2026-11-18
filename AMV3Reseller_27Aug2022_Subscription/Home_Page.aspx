<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Home_Page.aspx.cs" Inherits="Home_Page" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <script src="Scripts/AnyminuteTables.js"></script>
    <script type="text/javascript">
        function Confirm() {
            var confirm_value = document.createElement("INPUT");
            confirm_value.type = "hidden";
            confirm_value.name = "confirm_value";
            if (confirm("Do you want to delete this project?")) {
                confirm_value.value = "Yes";
            } else {
                confirm_value.value = "No";
            }
            document.forms[0].appendChild(confirm_value);
        }
          //  document.getElementById("LinkButton_Termination").onclick = function () { window.showModalDialog("ModalPopUpSelectEffectiveDate.aspx"); }
            function OpenEmpList() {
            //            window.showModalDialog("EmpPopUpList.aspx");
                alert('test');
            }
       // document.getElementById("clickMe").onclick = function () { alert('hello!'); };
           
    </script>
    <script>
            function showEmpList(hfdPopUpBusid1) {
                alert('test200');
                window.open("~/Home_Page_EmpPopUpList.aspx?hfdPopUpBusid=" +  hfdPopUpBusid1);
               // return false;
            }
    </script>
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

        
        .modalPopup_viewproject {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup_viewproject .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup_viewproject .body {
            min-height: 50px;
            line-height: 30px;
            /*font-weight: bold;*/
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            overflow-y: scroll;
            height: 400px;
            width: 800px;
        }

        .modalPopup_addproject {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup_addproject .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup_addproject .body {
            min-height: 50px;
            line-height: 30px;
            /*font-weight: bold;*/
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            /*overflow-y: scroll;*/
            height: 370px;
            width: auto;
        }

        .buttonhyperlink22 {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            
            border: 1px solid #607DF2;
            display: block;
            background-color:#607DF2 ;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink22:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color:#607DF2 ;
            border-radius: 1px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

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

        .btn-rounded {
            border-radius: 30px;
        }

        .btn-rounded:hover {
            border-radius: 30px;
        }

        /*.HeaderTemplate {
            border-bottom: 2px solid black;
        }*/
    </style>

    <style>
        .swal2-container:not(.swal2-top):not(.swal2-top-start):not(.swal2-top-end):not(.swal2-top-left):not(.swal2-top-right):not(.swal2-center-start):not(.swal2-center-end):not(.swal2-center-left):not(.swal2-center-right):not(.swal2-bottom):not(.swal2-bottom-start):not(.swal2-bottom-end):not(.swal2-bottom-left):not(.swal2-bottom-right):not(.swal2-grow-fullscreen) > .swal2-modal {
            margin: auto;
        }

         .swal2-container {
            overflow-y: auto;
            z-index: 300000 !important;
        }
    </style>

    
    <script src="Assets/assets/js/SweetAlert.js"></script>
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" >

    <div id="dividBusinessOwner" runat="server" style="display: none">        
        <div class="panel panel-default" id="divlist" runat="server">
            <div class="panel-body">
                <div class="row" >
                    <div class="col-md-12">
                        <div style="overflow-x: auto; overflow-y:hidden;">
                            <asp:Label ID="Label_Business_Owner_Deactive_Message" Text="This Business Owner Has Been Deactivated. Please Contact Admin." ForeColor="Red" Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:Label ID="Label_Business_Owner_Grid" Text="In below businesses you are assigned as a subscriber." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:Label ID="Label_Subscriber_Cancelled_Message" Text="" ForeColor="Red" Font-Size="X-Large" runat="server"/>
                            <asp:GridView ID="GvEmpListForBusOwner" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowCommand="GvEmpListForBusOwner_RowCommand"
                                GridLines="None" EmptyDataText="" OnPreRender="GvEmpListForBusOwner_PreRender"
                                OnRowDataBound="GvEmpListForBusOwner_RowDataBound">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <br />
                                            <b></b>
                                            <asp:HyperLink ID="hyeditBuss" ClientIDMode="Static" runat="server" Text="Update Profile"
                                                NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $(document).ready(function () {
                                                    $('#hyeditBuss').click(function () {
                                                        document.cookie = "busstabid=#home";
                                                    });
                                                });
                                            </script>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Create Project" Visible="false">
                                        <ItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddcreateproject" AutoPostBack="true">
                                                <asp:ListItem Value="No" Text="No"></asp:ListItem>
                                                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                            </asp:DropDownList>
                                            <br />
                                            <asp:LinkButton ID="hyAddProj" runat="server" Text="Add Project |" Visible="false"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("BusinessID") %>' CommandName="BusOnwerAddProjectList"></asp:LinkButton>
                                            <asp:LinkButton ID="hyViewProj" runat="server" Text="View Project" Visible="false"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("BusinessID") %>' CommandName="BusOnwerViewProjectList"></asp:LinkButton>
                                            <br />                               
                                        </ItemTemplate>
                                    </asp:TemplateField>--%>
                                    <asp:TemplateField HeaderText="Contact Name">
                                    <%--<asp:TemplateField HeaderText="Operating Name">--%>
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                            <asp:Label ID="Label1" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label>
                                            <%--<br />
                                            <b></b>--%>
                                            <asp:HyperLink ID="hyeditManger" Visible="false" runat="server" Text="Update Profile" NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $('#hyeditManger').click(function () {
                                                    document.cookie = "busstabid=#home";
                                                });
                                            </script>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Project List">
                                        <ItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddcreateproject" Visible="false" AutoPostBack="true">
                                                <asp:ListItem Value="No" Text="No"></asp:ListItem>
                                                <asp:ListItem Value="Yes" Text="Yes"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:LinkButton ID="hyViewProj" runat="server" Text="View Project" Visible="true"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("BusinessID") %>' CommandName="BusOnwerViewProjectList"></asp:LinkButton>
                                            <br />
                                            <asp:LinkButton ID="hyAddProj" runat="server" Text="Add Project" Visible="true"
                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex +","+Eval("BusinessID") %>' CommandName="BusOnwerAddProjectList"></asp:LinkButton>

                                        </ItemTemplate>

                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="PrimaryContNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Details" >
                                        <ItemTemplate>
                                            <asp:Label ID="CNT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNT")%>'
                                                Font-Bold="true"></asp:Label>
                                            <b></b>
                                           <asp:LinkButton ID="lnkBusOnwerEmpList" runat="server" Text="
                                                | User List" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' CommandName="ShowEmployeeList" /> 
                                            <b></b>
                                            <asp:HyperLink ID="hyAddEmp" CssClass="ckhhyAddEmp" ClientIDMode="Static" runat="server" Text=" | Add User"
                                                NavigateUrl='<%# "~/Add_EmployeeV2.aspx"%>'></asp:HyperLink>
                                            <asp:HyperLink ID="hyEditUserInline" runat="server" Text=" | Edit User" NavigateUrl='<%# "~/Home_Page_EmpPopUpList.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID") %>'></asp:HyperLink><b> </b>
                                            <asp:HyperLink  ID="HyperLink_AssUser" runat="server" Text=" | Assign User"
                                    NavigateUrl='<%# "~/GridviewNewDesign_AssignUser.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <br />
                                            <script>
                                                $('.ckhhyAddEmp').click(function () {

                                                    document.cookie = "tabid=#home";
                                                });
                                            </script>

                                <%----%>
                                            <%----%>
                                            
                                            
                                            <asp:HyperLink ID="hyAssManger" runat="server" Text=" | Assign User To Manager"
                                                NavigateUrl='<%# "~/GridviewNewDesign.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <asp:HyperLink ID="hyAssignAccountantInline" runat="server" Text=" | Accountant &amp; Payroll Access" NavigateUrl='<%# "~/AccountantAccess.aspx?BusinessId=" + Eval("BusinessID")%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Open Time Card" NavigateUrl='<%# "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <br />
                                            <asp:HyperLink ID="hyEditTimeSheet" Visible="false" runat="server" Text="Edit Time Sheet"
                                                NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                            <asp:HyperLink ID="hyWeekalyTimeSheet" Visible="false" runat="server" Text="Weekly Time Sheet"
                                                NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client Status" Visible="false">
                                        <ItemTemplate>
                                            <asp:HiddenField ID="hfdStatus" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status"><ItemTemplate><asp:Label runat="server" CssClass='<%# Convert.ToString(Eval("Status")) == "1" ? "am-status am-status-active" : "am-status am-status-inactive" %>' Text='<%# Convert.ToString(Eval("Status")) == "1" ? "Active" : "Inactive" %>' /></ItemTemplate></asp:TemplateField>
</Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="Owner" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body; swal2-container; swal2-confirm ">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: auto;">
                            <asp:Label ID="Label_Owner_Grid" Text="In below businesses you are assigned as a supervisor." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:GridView ID="GvEmpListOwner" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowCommand="GvEmpListOwner_RowCommand"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GvEmpListOwner_PreRender"
                                OnRowDataBound="GvEmpListOwner_RowDataBound">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessID1" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                            <asp:HyperLink ID="hyeditBuss1" ClientIDMode="Static" Visible="false" runat="server" Text="Update Profile"
                                                NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $(document).ready(function () {
                                                    $('#hyeditBuss').click(function () {
                                                        document.cookie = "busstabid=#home";
                                                    });
                                                });
                                            </script>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Role Name" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="Lbl_Administrator" runat="server" Text='Supervisor'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Contact Name">--%>
                                    <asp:TemplateField HeaderText="Operating Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName1" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                            <asp:Label ID="Label4" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                            <asp:HyperLink ID="hyeditManger1" runat="server" Visible="false" Text="Update Profile" NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <script>
                                                $('#hyeditManger').click(function () {
                                                    document.cookie = "busstabid=#home";
                                                });
                                            </script>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="PrimaryContNumber1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Details" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CNT1" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNT")%>'
                                                Font-Bold="true"></asp:Label>
                                            <b></b>
                                            <asp:HiddenField ID="hdnemptype" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EMPLOYEETYPE")%>'></asp:HiddenField>
                                            <asp:LinkButton ID="lnkBusOnwerEmpList1" runat="server" Text=" | User List"  CommandArgument='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' CommandName="ShowEmployeeList" /> 
                          
                                            <b></b>
                                            <asp:HyperLink ID="hyAddEmp1" ClientIDMode="Static" runat="server" Text=" | Add User"
                                                NavigateUrl='<%# "~/Add_EmployeeV2.aspx?Role=Administrator"%>'
                                                Visible='<%#Eval("ÍsExistAddEmployeeRole").ToString()=="Y"?true:false %>'></asp:HyperLink><b> </b>
                                            <script>
                                                $('#hyAddEmp').click(function () {
                                                    document.cookie = "tabid=#home";
                                                });
                                            </script>
                                            <br />
                                            <asp:HyperLink ID="hyAssbus1" Visible="false" runat="server" Text="Assign Staff to Business"
                                                NavigateUrl="~/AssignBusiness.aspx"></asp:HyperLink>
                                            <asp:HyperLink ID="hyAssManger1" runat="server" Text=" | Assign User To Manager"
                                                NavigateUrl='<%# "~/GridviewNewDesign.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'
                                                Visible='<%#Eval("ÍsExistAssignEmployeeToSuupervisorRole").ToString()=="Y"?true:false %>'>
                                            </asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet1" runat="server" Text="Open Time Card" NavigateUrl='<%#"~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Owner" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Owner" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                            <asp:HyperLink ID="HyperLink_Schedular_Owner" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&UserRole=Administrator"%>'></asp:HyperLink>
                                            <asp:HyperLink ID="hyEditTimeSheet1" Visible="false" runat="server" Text="Edit Time Sheet"
                                                NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                            <asp:HyperLink ID="hyWeekalyTimeSheet1" Visible="false" runat="server" Text="Weekly Time Sheet"
                                                NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Client Status" Visible="false">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="togBtnForBus1" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="togBtnForBus1" />
                                            <asp:HiddenField ID="hfdStatus1" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>
                                <asp:TemplateField HeaderText="Status"><ItemTemplate><asp:Label runat="server" CssClass='<%# Convert.ToString(Eval("Status")) == "1" ? "am-status am-status-active" : "am-status am-status-inactive" %>' Text='<%# Convert.ToString(Eval("Status")) == "1" ? "Active" : "Inactive" %>' /></ItemTemplate></asp:TemplateField>
</Columns>

                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="dividManager" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: auto;">
                            <asp:Label ID="Label_Manager_Grid" Text="In below businesses you are assigned as a manager." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:GridView ID="gvbusinessListForManager" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowCommand="gvbusinessListForManager_RowCommand" OnRowDataBound="gvbusinessListForManager_RowDataBound"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvbusinessListForManager_PreRender">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Role Name" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="Lbl_Manager" runat="server" Text='Manager'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Contact Name">--%>
                                    <asp:TemplateField HeaderText="Operating Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                            <asp:Label ID="Label3" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                            <asp:HiddenField ID="hfdMangerIDFormManger" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ManagerID")%>' />
                                           
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_PrimaryContactNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label><b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Details" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CNT" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "count")%>'
                                                Font-Bold="true"></asp:Label>
                                            <b></b>
                                            <asp:LinkButton ID="lnkMangerEmpList" runat="server" Text=" | User List" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="MangerEmpList"/> 
                          
                                            <b></b>
                                            <asp:HiddenField ID="hdfEmployeeType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EMPLOYEETYPE1")%>' />
                                            <asp:HyperLink ID="hyAddEmp2" ClientIDMode="Static" runat="server" Text=" | Add User"
                                                NavigateUrl='<%# "~/Add_EmployeeV2.aspx?Role=Manager"%>'
                                                Visible='<%#Eval("IsAddEmployeeRole").ToString()=="Y"?true:false %>'></asp:HyperLink><b> </b>
                                            <script>
                                                $('#hyAddEmp2').click(function () {
                                                    document.cookie = "tabid=#home";
                                                });
                                            </script>
                                            <br />
                                
                                <asp:HyperLink ID="hyAssMangerNew" runat="server" Text=" | Assign User To Manager"
                                    NavigateUrl='<%# "~/AssgineManager.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'
                                    Visible='<%#Eval("IsExistAssignEmployeeToSuupervisorRole").ToString()=="Y"?false:false %>'>
                                </asp:HyperLink>
                                
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Address" Visible="false">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HyperLink_Change_Address_Manager_Itself" runat="server" Text="Change Address" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("ManagerID")) : ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("ManagerID"))%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <%--<asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Enter/View Time Sheet" NavigateUrl='<%#(Eval("EMPLOYEETYPE1").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))  : ("~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))%>'></asp:HyperLink>--%>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Open Time Card" NavigateUrl='<%#(Eval("EMPLOYEETYPE1").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))  : ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))%>'></asp:HyperLink>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Manager" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Manager" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#(Eval("EMPLOYEETYPE1").ToString().Trim() =="R") ? ("~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))  : ("~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("ManagerID"))%>'></asp:HyperLink>--%>
                                            <asp:HyperLink ID="HyperLink_Schedular_Manager" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&UserRole=Manager"+"&MangerId=" + Eval("ManagerID")%>'></asp:HyperLink>
                                            <asp:HyperLink Visible="false" ID="hyEditTimeSheet" runat="server" Text="Edit Time Sheet"
                                                NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                            <asp:HyperLink Visible="false" ID="hyWeekalyTimeSheet" runat="server" Text="Weekly Time Sheet"
                                                NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="dividEmp" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: auto;">
                            <asp:Label ID="Label_Employee_Grid" Text="In below businesses you are assigned as an employee." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:GridView ID="gvBusinessListEmp" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowDataBound="gvBusinessListEmp_RowDataBound"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvBusinessListEmp_PreRender">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Role Name" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="Lbl_Employee" runat="server" Text='Employee'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Contact Name">--%>
                                    <asp:TemplateField HeaderText="Operating Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                            <asp:Label ID="Label2" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label><b><b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_PrimaryContactNumber" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                    
                                    <asp:TemplateField HeaderText="Edit Profile" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HyperLink_Change_Address_By_Employee" runat="server" Text="Edit Profile" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID")) : ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID"))%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet" runat="server" Text="Open Time Card" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))  : ("~/Project_Time_Sheet.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))%>'></asp:HyperLink>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Employee" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                            <%--<asp:HyperLink ID="HyperLink_Schedular_Employee" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Schedular.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))  : ("~/Schedular.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))%>'></asp:HyperLink>--%>
                                            <asp:HyperLink ID="HyperLink_Schedular_Employee" Visible="false" runat="server" Text="| Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&UserRole=Employee"%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="div_Accountant_User" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: auto;">
                            <asp:Label ID="Label_Accountant_Grid" Text="In below businesses you are assigned as an accountant." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:GridView ID="GridView_Accountant_User" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" OnRowDataBound="GridView_Accountant_User_RowDataBound"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_Accountant_User_PreRender">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="CompanyName__" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>'></asp:Label>
                                            <asp:HiddenField ID="hdfBusinessIDForManager__" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                            <b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="User Role Name" ItemStyle-Width="10%">
                                        <ItemTemplate>
                                            <asp:Label ID="Lbl_Accountant" runat="server" Text='Accountant'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <%--<asp:TemplateField HeaderText="Contact Name">--%>
                                    <asp:TemplateField HeaderText="Operating Name" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:Label ID="MangerName__" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                            <asp:Label ID="Label2__" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label><b><b>
                                                <br />
                                            </b>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Contact Number" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="_PrimaryContactNumber__" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Time Sheet" Visible="false">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hyViewTimeSheet__" runat="server" Visible="false" Text="Open Time Card" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))  : ("~/Project_Time_Sheet.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Schedule" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HyperLink_Schedular_Accountant" Visible="false" runat="server" Text="Schedular" NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&UserRole=Accountant"%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Edit Profile" Visible="true" ItemStyle-Width="20%">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="HyperLink_Change_Address_By_Employee__" runat="server" Text="Edit Profile" NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID")) : ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID"))%>'></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="div_Custome_User_Role" runat="server" style="display: none;">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <div style="overflow-x: auto;">
                            <asp:Label ID="Label_Custome_User_Role_Grid" Text="In below businesses you have been given additional roles." Font-Size="X-Large" runat="server" Visible="false" />
                            <asp:GridView ID="GridView_Custome_User_Role" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"  OnRowCommand="GridView_Custome_User_Role_RowCommand"
                                ForeColor="#333333" OnRowDataBound="GridView_Custome_User_Role_RowDataBound"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_Custome_User_Role_PreRender">
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                        <asp:TemplateField HeaderText="Sl. No." ItemStyle-Width="10%">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Business Name" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label ID="CompanyName1_Custome_User_Role" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                <asp:HiddenField ID="hdfBusinessID1_Custome_User_Role" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                <asp:HiddenField ID="HiddenField_EmployeeID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                <b>
                                    <br />
                                </b>
                                <asp:HyperLink ID="hyeditBuss1_Custome_User_Role" ClientIDMode="Static" Visible="false" runat="server" Text="Update Profile"
                                    NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                                <script>
                                    $(document).ready(function () {
                                        $('#hyeditBuss').click(function () {
                                            document.cookie = "busstabid=#home";
                                        });
                                    });
                                </script>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Role Name" ItemStyle-Width="10%" Visible="true">
                            <ItemTemplate>
                                <asp:Label ID="Custom_User_Role_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "User_Role_Name")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Operating Name" ItemStyle-Width="20%">
                            <ItemTemplate>
                                <asp:Label ID="MangerName1_Custome_User_Role" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MangerName")%>'></asp:Label>
                                <asp:Label ID="Label4_Custome_User_Role" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OperatingName")%>'></asp:Label><b>
                                    <br />
                                </b>
                                <asp:HyperLink ID="hyeditManger1_Custome_User_Role" runat="server" Visible="false" Text="Update Profile" NavigateUrl='<%# "~/Add_Business.aspx?BusinessIDEdit=" + Eval("BusinessID")%>'></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Contact Number" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="PrimaryContNumber1_Custome_User_Role" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PrimaryContactNumber")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Additional Roles">
                            <ItemTemplate>
                                <asp:Label ID="CNT1_Custome_User_Role" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNT")%>'
                                    Font-Bold="true"></asp:Label>
                                <asp:Label ID="CNTCustRole_Custome_User_Role" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CNTCustRole")%>'
                                    Font-Bold="true"></asp:Label>
                                <asp:HiddenField ID="hdnemptype_Custome_User_Role" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EMPLOYEETYPE")%>'></asp:HiddenField>

                                <asp:LinkButton ID="lnkBusOnwerEmpList1_Custome_User_Role" Visible="true" runat="server" Text=" | User List"  CommandArgument='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' CommandName="ShowEmployeeList"></asp:LinkButton>
                                
                                <asp:HyperLink ID="hyAddEmp1_Custome_User_Role" ClientIDMode="Static" runat="server" Text=" | Add User"
                                    NavigateUrl='<%# "~/Add_EmployeeV2.aspx"%>'
                                    Visible='<%#Eval("IsExistAddEmployeeRoleForManager").ToString()=="Y"?true:false %>'>
                                </asp:HyperLink><b> </b>
                                
                                <script>
                                    $('#hyAddEmp').click(function () {
                                        document.cookie = "tabid=#home";
                                    });
                                </script>

                                <asp:HyperLink ID="hyAssManger1_Custome_User_Role" runat="server" Text=" | Assign User To Manager"
                                    NavigateUrl='<%# "~/GridviewNewDesign.aspx?BusinessIdFromDefaultPapge=" + Eval("BusinessID")%>'
                                    Visible='<%#Eval("IsExistAssignEmployeeToSuupervisorRole").ToString()=="Y"?true:false %>'>
                                </asp:HyperLink>
                                
                                <%--<asp:HyperLink ID="hyViewTimeSheet1_Custome_User_Role" runat="server" Text="| Enter/View Time Sheet" 
                                    NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID"))  : ("~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID"))%>' 
                                    Visible='<%#Eval("IsEnterTimeSheetRoleExistsForEmployee").ToString()=="Y"?true:true %>'>
                                </asp:HyperLink>--%>

                                <asp:HyperLink ID="hyViewTimeSheet1_Custome_User_Role" runat="server" Text="| Open Time Card" 
                                    NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID")+"&CustomRole=YES")  : ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID")+"&CustomRole=YES")%>' 
                                    Visible='<%#Eval("IsEnterTimeSheetRoleExistsForEmployee").ToString()=="Y"?true:true %>'>
                                </asp:HyperLink>
                                
                                <%--<asp:HyperLink ID="hyViewTimeSheet1_Custome_User_Role" runat="server" Text="| Enter/View Time Sheet" 
                                    NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID"))  : ("~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID"))%>'
                                    Visible='<%#Eval("IsEnterTimeSheetRoleExistsForEmployee").ToString()=="Y"?true:false %>'>
                                </asp:HyperLink>--%>

                                <%--<asp:HyperLink ID="HyperLink_Schedular_Custom_User" runat="server" Visible="false" Text="| Schedular" 
                                    NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")%>'></asp:HyperLink>--%>
                                
                                <%--<asp:HyperLink ID="HyperLink_Schedular_Custom_User" runat="server" Visible="false" Text="| Schedular" 
                                    NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID")+"&CustomRole=YES")  : ("~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&MangerId=" + Eval("EmployeeID")+"&CustomRole=YES")%>'></asp:HyperLink>--%>

                                <asp:HyperLink ID="HyperLink_Schedular_Custom_User" runat="server" Visible="false" Text="| Schedular" 
                                    NavigateUrl='<%#"~/Schedular.aspx?BusinessIdFromDefaultPage=" + Eval("BusinessID")+"&UserRole=CustomRole"+"&MangerId=" + Eval("EmployeeID")%>'></asp:HyperLink>

                                <asp:HyperLink ID="hyAssbus1_Custome_User_Role" Visible="false" runat="server" Text="Assign Staff to Business"
                                    NavigateUrl="~/AssignBusiness.aspx"></asp:HyperLink>

                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Time Sheet" ItemStyle-Width="20%" Visible="false" >
                            <ItemTemplate>
                                <asp:HyperLink ID="hyEditTimeSheet1_Custome_User_Role" Visible="false" runat="server" Text="Edit Time Sheet"
                                    NavigateUrl="~/EditTimeSheet.aspx"></asp:HyperLink>
                                <asp:HyperLink ID="hyWeekalyTimeSheet1_Custome_User_Role" Visible="false" runat="server" Text="Weekly Time Sheet"
                                    NavigateUrl="~/WeekWiseTimeSheet.aspx"></asp:HyperLink>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Client Status" Visible="false">
                            <ItemTemplate>
                                <asp:ImageButton ID="togBtnForBus1_Custome_User_Role" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="togBtnForBus1" />
                                <asp:HiddenField ID="hfdStatus1_Custome_User_Role" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'></asp:HiddenField>
                            </ItemTemplate>
                            <ItemStyle CssClass="text-left" />
                        </asp:TemplateField>
                    </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%-- Model Pop Start for Pop Up Emp List --%>
    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmpList" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide" >
    </cc1:ModalPopupExtender>

    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none; height:400px; width:90%; overflow-y:scroll ">
       

        <div class="modal-content" >
            <div class="header">
                <asp:Label ID="lblbusName" runat="server"></asp:Label>
                <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
                <div class="pull-right" style="margin-right: 5px;">
                    <button type="button" id="btnHide" class="btn btn-danger">
                        <i class="fa fa-close" aria-hidden="true" ></i>
                    </button>
                </div>
            </div>
                           <!-- <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>-->
            <!--<div class="body">-->
                <div class="row" >
                    <div class="col-md-12">
                        <table style="width:100%">
                            <tr>
                                <td width="2%">
                                    <label>&nbsp;&nbsp;&nbsp;&nbsp;</label>
                                </td>
                                <td width="15%">
                                    <label>Select Status</label>
                                </td>
                                <td width="2%">
                                    <label>&nbsp;</label>
                                </td>
                                <td width="25%" align="left">
                             <!--       <asp:UpdatePanel ID="updatepanel5" runat="server" UpdateMode="conditional">
                                        <ContentTemplate>-->
                                    <asp:DropDownList runat="server" ID="ddl_Action_Status" Width="50%" CssClass="form-control xs-mselect" OnSelectedIndexChanged="ddl_Action_Status_SelectedIndexChanged" AutoPostBack="true">
                                        <asp:ListItem Value="" Text="Select"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="All"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Active"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Not Active"></asp:ListItem>
                                        <asp:ListItem Text="Terminated" Value="Terminated" />
</asp:DropDownList>
                                           <!-- </ContentTemplate>
                                        </asp:UpdatePanel>-->
                                </td>
                                <td style="width:56%"></td>
                            </tr>
                        </table>
                        </div>
                    </div>
                </div>
                  <div class="row">
                    <div class="col-md-12"   >
                
                        <asp:GridView ID="GvPopUpEmpList" runat="server" ClientIDMode="Static"
                            CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                            ForeColor="#333333" OnRowCommand="GvPopUpEmpList_RowCommand"
                            GridLines="None" EmptyDataText="No Records Found" OnPreRender="GvPopUpEmpList_PreRender"
                            OnRowDataBound="GvPopUpEmpList_RowDataBound" AlternatingRowStyle-BackColor="White" >
                            <AlternatingRowStyle BackColor="#f5f5f5" />
                            <Columns>
                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="User Name">
                                    <ItemTemplate>
                                        <asp:Label ID="EmpName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmpName")%>'></asp:Label>
                                        <asp:HiddenField ID="hdfEmpNameid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                        <b>
                                            <br />
                                        </b>
                                        <asp:HyperLink ID="hyeditEmp" runat="server" Text="Update Profile" NavigateUrl='<%# "~/AddEditRole_Add_Employee.aspx?EmployeeIDEdit=" + Eval("EmployeeID")+"&BussEditId="+ Eval("BusinessID")%>'></asp:HyperLink>
                                        <script>
                                            $('#hyeditEmp').click(function () {
                                                document.cookie = "tabid=#home";
                                            });
                                        </script>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Role">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_UserRole" runat="server" Text='<%# AnyminuteEmployeeEditor.RoleDisplay(DataBinder.Eval(Container.DataItem, "UserRoleName")) %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Employee Type">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Employee_Type" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Employee_Type")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Rate">
                                    <ItemTemplate>
                                        <asp:Label ID="Rate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Rate")%>'></asp:Label><br />
                                        <asp:HyperLink ID="hyeditEmprate" runat="server" CssClass="Emprate" ClientIDMode="Static" Text="Update Rate" NavigateUrl='<%# "~/Change_Rate_EmployeeV2.aspx?EmployeeIDEdit=" + Eval("EmployeeID")+"&BussEditId="+ Eval("BusinessID")%>' onClick="showDiv();"></asp:HyperLink>
                                        <script>
                                            function showDiv() {
                                                document.cookie = "tabid=#menu3";
                                            }
                                        </script>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Time Sheet" Visible="false">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnredirect" runat="server" Value='<%#Eval("EMPLOYEE_TYPE")%>' />
                                        <asp:HyperLink ID="hyEnterTimeSheet" runat="server" Text="Open Time Card"
                                            NavigateUrl='<%#(Eval("EMPLOYEE_TYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))  : ("~/Project_Time_Sheet.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))%>'
                                            Visible='<%#Eval("IsEnterTimeSheetRoleExists").ToString()=="Y"?true:false %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Edit Profile">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink_Change_Address_By_Manager" runat="server" Text="Edit Profile"
                                            NavigateUrl='<%#(Eval("EMPLOYEE_TYPE").ToString().Trim() =="R") ? ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID")) : ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID"))%>'
                                            Visible='<%#Eval("IsExistsUpdateAddress").ToString()=="Y"?true:false %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="User Status" Visible="false">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="togBtnForBusOwner" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("EmployeeID") %>' CommandName="togBtnForBusOwner"
                                            Visible='<%#Eval("IsExistsActivateDeactivateEmployee").ToString()=="Y"?true:false %>' />
                                        <asp:HiddenField ID="hfdStatusForBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive" )%>' />
                                        <asp:HiddenField ID="hdfbusinessId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <div class="dropdown">
                                            <asp:LinkButton ID="dropdownMenu1" runat="server" Text="Action" data-toggle="dropdown" 
                                                aria-haspopup="true" aria-expanded="true" />&nbsp;<i class="fa fa-angle-down" style="color:#607DF2;" aria-hidden="true"></i>
                                            <ul class="dropdown-menu" aria-labelledby="dropdownMenu1">
                                                <li><asp:LinkButton ID="LinkButton_Active" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Active" OnClick="LinkButton_Active_Click" ></asp:LinkButton></li>
                                                <li><asp:LinkButton ID="LinkButton_NotActive" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Not Active" OnClick="LinkButton_NotActive_Click"></asp:LinkButton></li>
                                                 <li><asp:LinkButton ID="LinkButton_Termination" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Terminate" OnClientClick="OpenEmpList()" ></asp:LinkButton></li>
                                                 <li><asp:LinkButton ID="LinkButton_Edit" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Edit"></asp:LinkButton></li>
                                            </ul>
                                        </div>
                                        <asp:HiddenField ID="hfdStatusForBussOwner____" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
                                        <asp:HiddenField ID="hdfbusinessId____" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                    </ItemTemplate>
                           </asp:TemplateField>

                                  <asp:TemplateField HeaderText="Status" Visible="true">
                                        <ItemTemplate>
                                            <asp:Label ID="lblStatus" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Employee_Status")%>'></asp:Label>
                                                                          
                                        </ItemTemplate>
                                    </asp:TemplateField> <%--GVChildlist.Columns[17]--%>
                                   

                                <asp:TemplateField HeaderText="Effective Date">
                                    <ItemTemplate>
                                         <asp:Label ID="lblEffectiveDate" runat="server" width="100px" Text='<%#DataBinder.Eval(Container.DataItem,
                                            "Effective_Date", "{0: MM/dd/yyyy}")%>'></asp:Label><br />
                                        <!--<asp:TextBox ID="txtTerminationDate" Max="9999-12-31" runat="server" class="form-control"
                                            type="date" OnTextChanged="txtTerminationDate_TextChanged" AutoPostBack="true" 
                                             ClientIDMode="Static"
                                            Text='<%#DataBinder.Eval(Container.DataItem, "Effective_Date")%'></asp:TextBox>-->
                                        <asp:HiddenField ID="hdf_EmployeeID_" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div>No records found.</div>
                            </EmptyDataTemplate>
                            <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                        </asp:GridView>
                    </div>
                       <!--</ContentTemplate>
                        </asp:UpdatePanel>-->
                </div>
         
            <!--</div>-->
        <!--</div>-->
                     
    </asp:Panel>
                   
    
    <%-- Model Pop End for Pop Up Emp List --%>
   

    <%-- Model Pop Start for View Project --%>

 
    <asp:LinkButton ID="lnkDummy1" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modalviewproject" BehaviorID="mpe1" runat="server"
        PopupControlID="pnlviewproject" TargetControlID="lnkDummy1" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide11">
    </cc1:ModalPopupExtender>
 
    <asp:Panel ID="pnlviewproject" runat="server" CssClass="modalPopup_viewproject" Style="display: none;">
    <div class="header">
        <asp:Label ID="lblbussnm" runat="server"></asp:Label>
        <asp:HiddenField ID="hdbusid" runat="server" />
        <asp:HiddenField ID="confirm_value" runat="server" />
        <div class="pull-right" style="margin-right: 5px;">
            <button type="button" id="btnHide11" class="btn btn-danger">
                <i class="fa fa-close"></i>
            </button>
        </div>
    </div>
    <div class="body">
        <div class="row">
            <div class="col-md-12">
                <asp:HiddenField ID="hdfprojname" runat="server" />
                <asp:GridView ID="grdviewProject" runat="server" ClientIDMode="Static"  OnPreRender="grdviewProject_PreRender"
                    CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                    ForeColor="#333333" OnRowDataBound="grdviewProject_RowDataBound"
                    GridLines="None" EmptyDataText="No Records Found">
                    <AlternatingRowStyle BackColor="#f5f5f5" />
                    <Columns>
                        <asp:TemplateField HeaderText="Sl. No.">
                            <ItemTemplate>
                                <%#Container.DataItemIndex + 1%>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Project Name">
                            <ItemTemplate>
                                <asp:Label ID="ProjName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>'></asp:Label>
                                <asp:HiddenField ID="hdfProjNameid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectID")%>' />
                                <asp:HiddenField ID="HiddenField_BusinessID_For_Project" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Project User List">
                            <ItemTemplate>
                                <asp:DropDownList ID="DropDownList_Project_Employee_Name" runat="server" AppendDataBoundItems="true" CssClass="form-control xs-mmmselect">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Start Date" Visible="true">
                            <ItemTemplate>
                                <asp:Label ID="startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startDate")%>'></asp:Label><br />                                    
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="End Date" Visible="true">
                            <ItemTemplate>
                                <asp:Label ID="enddate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label><br />                                    
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Edit" Visible="false">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgeditproject" runat="server" ImageUrl="~/images/edit.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Delete" Visible="false">
                            <ItemTemplate>
                                <asp:ImageButton ID="imgdeleteproject" runat="server" OnClientClick="Confirm()" ImageUrl="~/images/delete3.png" width="30px"  CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="deleteproject" />   
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
   
    <%-- Model Pop End for View Project --%>


    <%-- Model Pop Start for Add Project --%>
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
    <cc1:ModalPopupExtender ID="modaladdproject" BehaviorID="mpe2" runat="server"
        PopupControlID="pnladdproject" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide12">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnladdproject" runat="server" CssClass="modalPopup_addproject" Style="display: none;">
        <div class="header">
            <asp:Label ID="lblbusinessname" runat="server"></asp:Label>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnHide12" class="btn btn-danger">
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
                                <label>Project Name</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                <asp:TextBox ID="txtProjectName" Width="400px"  ClientIDMode="Static"
                                    runat="server" CssClass="form-control" onkeypress="hideIt()"></asp:TextBox>
                                <asp:Label ID="lblerror" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                <asp:HiddenField ID="hdnproject" runat="server" />
    
                                <asp:HiddenField ID="hfdsaveorupdate" runat="server" Value="SAVE"/>
                                <asp:HiddenField ID="hfdprojectId" runat="server" />
                            </div>
                        </div>          
                    </div>
                                    
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label>Start Date</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                    <asp:Label ID="lblerror1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                <asp:TextBox ID="txtstartdate" Width="400px" placeholder="DD/MM/YYYY"
                                        type="date" CssClass="form-control"  onkeypress="hideIt1()" runat="server"></asp:TextBox>                                            
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label>End Date</label>&nbsp;<span style="font-weight:bold"></span>
                                <asp:TextBox ID="txtenddate" Max="9999-12-31" Width="400px" placeholder="DD/MM/YYYY"
                                    type="date" CssClass="form-control" runat="server"></asp:TextBox>                                            
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">                         
                            <div class="form-group">
                                <center>
                                    <asp:Button runat="server" ID="btnSaveProjectDetails" Text="Save" CssClass="buttonhyperlink22" OnClick="btnSaveProjectDetails_Click"/>
                                    <asp:HiddenField ID="hdnemailid" runat="server" />
                                </center>
                            </div>
                        </div>
                    </div>
                 
            </div>
                                
                </ContentTemplate>
                <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSaveProjectDetails" EventName="Click"/>                                        
                </Triggers>
        </asp:UpdatePanel>                
    </asp:Panel>
       <!-- effective date code  starts here-->
       <!--effective date code ends here -->
    <%-- Model Pop End for Add Project --%>

    <asp:LinkButton ID="LinkDummy1" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="mpe3" runat="server"
        PopupControlID="PnlEffectiveDate" TargetControlID="LinkDummy1" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide12">
    </cc1:ModalPopupExtender>
    
    <asp:Panel ID="PnlEffectiveDate" runat="server" CssClass="modalPopup_addproject" Style="display: none;">
        <div class="header">
            <asp:Label ID="Label5" runat="server"></asp:Label>
            <asp:HiddenField ID="hdnEmpId" runat="server" />
            <asp:HiddenField ID="hdnEmpStatus" runat="server" />
            <asp:HiddenField ID="hdnEffectiveDate" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnHide13" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>
                    
            <div class="body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="Label6" text="Effective Date: " runat="server"></asp:Label>
                                <asp:TextBox ID="TextEffectiveDate" Width="400px"  ClientIDMode="Static"
                                    type="date"  AutoPostBack="true" 
                                                                                
                                    runat="server" CssClass="form-control" onkeypress="hideIt()"></asp:TextBox>
                                
                            </div>
                        </div>          
                    </div>
                                    
                    
                    
                    <div class="row">
                        <div class="col-md-12">                         
                            <div class="form-group">
                                <center>
                                    <asp:Button runat="server" ID="BtnSaveEffectiveDate" Text="Save" CssClass="buttonhyperlink22" OnClick="btnSaveEffectiveDate_Click"/>
                                 
                                </center>
                            </div>
                        </div>
                    </div>
                 
            </div>
     </asp:Panel>

</asp:Content>
