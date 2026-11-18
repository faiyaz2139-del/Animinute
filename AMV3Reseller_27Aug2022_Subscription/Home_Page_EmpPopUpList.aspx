<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Home_Page_EmpPopUpList.aspx.cs" Inherits="Home_Page_EmpPopUpList" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server" >
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
                                        function hideEffectiveDate() {
                                            document.getElementById("ModalPopUpExtender1").style.display = "hidden";
                                        }
                                        // document.getElementById("clickMe").onclick = function () { alert('hello!'); };
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
    <asp:HyperLink ID="btnback" runat="server" Text="<< Back" NavigateUrl="~/Home_Page.aspx" width="500px" CssClass="buttonhyperlink" ClientIDMode="Static" />
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style=" height:550px; width:100%; overflow-y:scroll ">
        
        <div class="modal-content" >
            <div class="header">
                <asp:Label ID="lblbusName" runat="server"></asp:Label>
                <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
                <div class="pull-right" style="margin-right: 5px;">
                </div>
            </div>
              
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
                                        <asp:ListItem Value="3" Text="Terminated"></asp:ListItem>
                                    </asp:DropDownList>
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
                            OnRowDataBound="GvPopUpEmpList_RowDataBound" AlternatingRowStyle-BackColor="White"  >
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
                                     
                                        <asp:LinkButton ID="lnbUpdateProfile" runat ="server" Text="Update Profile" OnClick="lnbUpdateProfile_Click" ></asp:LinkButton>
                                        
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
                                        <asp:Label ID="Rate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Rate")%>'></asp:Label><br></br>
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
                                        <asp:HiddenField ID="hdnredirect" runat="server" Value='<%#Eval("EMPLOYEETYPE")%>' />
                                        <asp:HyperLink ID="hyEnterTimeSheet" runat="server" Text="Time Card"
                                            NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/Time_Sheet_Entry.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))  : ("~/Project_Time_Sheet.aspx?EmpidfromBusOwner=" + Eval("EmployeeID")+"&BidfromBusOwner=" + Eval("BusinessID"))%>'
                                            Visible='<%#Eval("IsEnterTimeSheetRoleExists").ToString()=="Y"?true:false %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Address">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink_Change_Address_By_Manager" runat="server" Text="Edit Address"
                                            NavigateUrl='<%#(Eval("EMPLOYEETYPE").ToString().Trim() =="R") ? ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID")) : ("~/EmployeeProfile_Address_Edit.aspx?ProfileEmployeeId_ForAddressEdit=" + Eval("EmployeeID"))%>'
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
                                                 <li><asp:LinkButton ID="LinkButton_Termination" ForeColor="#607DF2" text-decoration="underline" runat="server" Text="Terminate" OnClick="LinkButton_Terminate_Click" ></asp:LinkButton></li>
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

                 
            </div>
                                
                </ContentTemplate>
              
        </asp:UpdatePanel>                
    </asp:Panel>
       <!-- effective date code  starts here-->
       <!--effective date code ends here -->
    <%-- Model Pop End for Add Project --%>

    <asp:LinkButton ID="LinkDummy1" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupExtender1" BehaviorID="mpe3" runat="server"
        PopupControlID="PnlEffectiveDate" TargetControlID="LinkDummy1" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide13" >
    </cc1:ModalPopupExtender>
    
    <asp:Panel ID="PnlEffectiveDate" runat="server" CssClass="modalPopup_addproject" Style="display: none;">
        <div class="header">
            <asp:Label ID="Label5" runat="server"></asp:Label>
            <asp:HiddenField ID="hdnEmpId" runat="server" />
            <asp:HiddenField ID="hdnEmpStatus" runat="server" />
            <asp:HiddenField ID="hdnEffectiveDate" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnHide13" class="btn btn-danger" aria-hidden="true">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>
                    
            <div class="body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="Label6" text="Effective Date: " runat="server"></asp:Label>
                                <asp:TextBox ID="TextEffectiveDate" Width="400px" clientIDMode ="Static" 
                                    type="date"                 
                                    runat="server" CssClass="form-control" >
                                </asp:TextBox>
                                <asp:RangeValidator ID="rvEffectiveDate" runat="server" ControlToValidate="TextEffectiveDate" 
                                    ErrorMessage="Date beyond one year is not valid!" 
                                    Forecolor="Red"
                                    Type="Date"/>


                            </div>
                        </div>          
                    </div>

                    <div class="row">
                        <div class="col-md-12">                         
                            <div class="form-group">
                                <center>
                                    <asp:Button runat="server" ID="BtnSaveEffectiveDate" type = "button" Text="Save" CssClass="buttonhyperlink22" onClick="btnSaveEffectiveDate_Click" />
                                 
                                </center>
                                
                            </div>
                        </div>
                    </div>
                 
            </div>
     </asp:Panel>

</asp:Content>
