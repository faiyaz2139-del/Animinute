<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="GridviewNewDesign_AssignUser.aspx.cs" Inherits="GridviewNewDesign_AssignUser" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script>
        $(document).ready(function () {
            $('#ddlmanager').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $('#ddlcustomrole').on('change', function () {
                $('#<%=hfSelected_Customrole.ClientID%>').val($(this).val());
            });
        });
    </script>
    <script>
        $(document).ready(function () {
            $('#ddlaccountant').on('change', function () {
                $('#<%=hfSelected_accountant.ClientID%>').val($(this).val());
            });
        });
    </script>
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
    .buttonhyperlink {
        /*text-align: center;
        padding-top: 10px;
        font-family: sans-serif;
        font-size: medium;*/
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
    .dropdownlist {
        /*text-align: center;
        padding-top: 10px;
        font-family: sans-serif;
        font-size: medium;*/
            
        border: 1px solid #607DF2;
        display: block;
        background-color:#ECF0F3 ;
        border-radius: 30px;
        color: black ;
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
          function ShowDocument(hdfselected, dtfrom, dtto, businessname, counter, empid, empname, type) {
              var url = "document2.aspx?hfSelected=" + hdfselected + "&dtfrom=  " + dtfrom + "&dtto=  " + dtto + "&businessname=  " + businessname + "&counter=  " + counter + "&empid=  " + empid + " &empname=  " + empname + "&type=  " + type;
              document.location.href = url;
              return false;

          }
      </script>

    <script>
        function ShowDocument(hdfselected, dtfrom, dtto, businessname, counter, empid, empname, type) {
            var url = "document2.aspx?hfSelected_Customrole=" + hdfselected + "&dtfrom=  " + dtfrom + "&dtto=  " + dtto + "&businessname=  " + businessname + "&counter=  " + counter + "&empid=  " + empid + " &empname=  " + empname + "&type=  " + type;
            document.location.href = url;
            return false;

        }
    </script>
    <script>
        function ShowDocument(hdfselected, dtfrom, dtto, businessname, counter, empid, empname, type) {
            var url = "document2.aspx?hfSelected_accountant=" + hdfselected + "&dtfrom=  " + dtfrom + "&dtto=  " + dtto + "&businessname=  " + businessname + "&counter=  " + counter + "&empid=  " + empid + " &empname=  " + empname + "&type=  " + type;
            document.location.href = url;
            return false;

        }
    </script>
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
    <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>

    <style type="text/css">
    .divTable
    {
        display:  table-bordered;
        width:100%;
        color:white;
        background-color:#ffffff;
        border:1px solid  #666666;
        border-spacing:5px;/*cellspacing:poor IE support for  this*/
       /* border-collapse:separate;*/
    }

    .divTables
    {
        display:  table-bordered;
        width:16%;
        background-color:#ffffff;
        border:1px solid  #ffffff;
        border-spacing:5px;/*cellspacing:poor IE support for  this*/
       /* border-collapse:separate;*/
    }

    .divRow
    {
       display:table-row;
       width:auto;
    }

    .divCell
    {
        float:left;/*fix for  buggy browsers*/
        display:table-column;
        width:auto;
        background-color:#ccc;
    }
    </style>
    <link href="css/ButtonAsLinkbutton.css" rel="stylesheet" />

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
            <div class="col-md-6">
                <h2 style="text-align:left;margin-top:12px;">Assign Employee To Manager</h2>
            </div>
            
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A2" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Assign User" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>
    
    <div class="panel panel-default" runat="server">
        <div class="panel-body">

            <div class="row am-assignment-selectors">
<div class="form-group"><label>Business</label><asp:DropDownList
                            ID="Ddl_Business_Assign_User"
                            runat="server"
                            AppendDataBoundItems="true"
                            CssClass="form-control js-example-basic-single"                             
                            AutoPostBack="true" OnSelectedIndexChanged="Ddl_Business_Assign_User_SelectedIndexChanged">
                        </asp:DropDownList></div>
<div class="form-group"><label>Role</label><asp:DropDownList
                            ID="DropDownList_Select_Role"
                            runat="server"
                            AppendDataBoundItems="true"
                            CssClass="form-control js-example-basic-single"                             
                            AutoPostBack="true" OnSelectedIndexChanged="DropDownList_Select_Role_SelectedIndexChanged">
                            <asp:ListItem>Select Role</asp:ListItem>
                            <asp:ListItem>Manager</asp:ListItem>
                            <asp:ListItem>Custom Role</asp:ListItem>
                            <asp:ListItem>Accountant</asp:ListItem>
                        </asp:DropDownList></div>
<div class="form-group" runat="server" id="Div_Assign_Manager_ddl" visible="false"><label>Manager</label><asp:DropDownList ID="ddlmanager" AppendDataBoundItems="true" runat="server" CssClass="form-control js-example-basic-single" Width="100%" OnSelectedIndexChanged="ddlmanager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
<div class="form-group" runat="server" id="Div_Assign_CustomRole_ddl" visible="false"><label>Custom Role</label><asp:DropDownList ID="ddlcustomrole" AppendDataBoundItems="true" runat="server" CssClass="form-control js-example-basic-single" Width="100%" OnSelectedIndexChanged="ddlcustomrole_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
<div class="form-group" runat="server" id="Div_Assign_Accountant_ddl" visible="false"><label>Accountant</label><asp:DropDownList ID="ddlaccountant" AppendDataBoundItems="true" runat="server" CssClass="form-control js-example-basic-single" Width="100%" OnSelectedIndexChanged="ddlaccountant_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></div>
</div>
<div runat="server" id="Div_Assign_Manager" visible="false">        

            <div class="row">
                <div class="col-md-4"></div>
                
            </div>

            <div class="row">
                <div class="col-md-2"></div>
                <div class="col-md-3">
                    <asp:HiddenField ID="hfSelected" runat="server" />
                    <div style="overflow-x: auto;">
                        <asp:GridView ID="GridView_UnAssigned_Employee_List" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                        AutoGenerateColumns="False"  
                        AllowPaging="false" OnPageIndexChanging="GridView_UnAssigned_Employee_List_PageIndexChanging" OnPreRender="GridView_UnAssigned_Employee_List_PreRender">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <Columns>
                                <asp:TemplateField HeaderStyle-Width="30px">
                                    <HeaderTemplate>
                                        <asp:Label Text="Select" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox
                                            runat="server"
                                            ID="chkemp" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="User Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label><%--Width="100px"--%>
                                            
                                    </ItemTemplate>
                                </asp:TemplateField>
                                    
                                <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                    
                            </Columns>
                            <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />
                        </asp:GridView>
                    </div>
                </div>

                <div class="col-md-1">

                     <table runat="server" border="1" id="table_button_Manager" Visible="false">
                         <tr>
                             <td style="padding:5px;">
                                <asp:Button ID="Button_LeftMove_Manager" Visible="false" runat="server" Text="Assign >" CssClass="buttonlooklikelinkbutton" OnClick="Button_LeftMove_Manager_Click"/>
                             </td>
                         </tr>
                         <tr>
                             <td style="padding:5px;">
                                <asp:Button ID="Button_LeftMoveAll_Manager" Visible="false" OnClick="Button_LeftMoveAll_Manager_Click" runat="server" Text="Assign All >>" CssClass="buttonlooklikelinkbutton"/>
                             </td>
                         </tr>
                         <tr>
                             <td style="padding:5px;">
                                <asp:Button ID="Button_RightMove_Manager" Visible="false" OnClick="Button_RightMove_Manager_Click" runat="server" Text="< Un Assign" CssClass="buttonlooklikelinkbutton"/>
                             </td>
                         </tr>
                         <tr>
                             <td style="padding:5px;">
                                <asp:Button ID="Button_RightMoveAll_Manager" Visible="false" OnClick="Button_RightMoveAll_Manager_Click" runat="server" Text="<< Un Assign All" CssClass="buttonlooklikelinkbutton"/>
                             </td>
                         </tr>
                         <tr>
                             <td style="padding:5px;">
                                <asp:Button ID="btnSave_Manager" runat="server" Visible="false" Text="Save" CssClass="btn2" OnClick="btnSave_Manager_Click"/>
                             </td>
                         </tr>
                     </table>

                </div>

                <div class="col-md-3">
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                    <div style="overflow-x: auto;">
                        <asp:GridView ID="GridView_Assigned_Employee_List" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                            AutoGenerateColumns="False"  
                            AllowPaging="false" OnPageIndexChanging="GridView_Assigned_Employee_List_PageIndexChanging" OnPreRender="GridView_Assigned_Employee_List_PreRender">
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label Text="Select" runat="server" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                runat="server"
                                                ID="chkemp" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Assigned User Name">
                                        <ItemTemplate>
                                            <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                            
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    
                                    <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                    
                                </Columns>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                        </asp:GridView>
                        </div>
                    </div>

                <div class="col-md-2"></div>

            </div>

        </div>





        <div runat="server" id="Div_Assign_CustomRole" visible="false">   
            
                    <div class="row">
                 <div class="col-md-4"></div>
                
            </div>
                    <div class="row">
                    <div class="col-md-2"></div>
                        <div class="col-md-3">
                            <asp:HiddenField ID="hfSelected_Customrole" runat="server" />
                            <div style="overflow-x: auto;">
                                <asp:GridView ID="GridView_UnAssigned_Employee_List_To_Customrole" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                                    AutoGenerateColumns="False"  
                                    AllowPaging="false" OnPageIndexChanging="GridView_UnAssigned_Employee_List_To_Customrole_PageIndexChanging" OnPreRender="GridView_UnAssigned_Employee_List_To_Customrole_PreRender">

                                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                                        <Columns>
                                            <asp:TemplateField HeaderStyle-Width="30px">
                                                <HeaderTemplate>
                                                    <asp:Label Text="Select" runat="server" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox
                                                        runat="server"
                                                        ID="chkemp" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="User Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label><%--Width="100px"--%>
                                            
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                    
                                            <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                    
                                        </Columns>
                                
                                    <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />

                                                                                    </asp:GridView>
                            </div>
                        </div>

                    <div class="col-md-1">

                        <table runat="server" border="1" id="table_button_Customrole" Visible="false">
                            <tr>
                                <td style="padding:5px;">
                                <asp:Button ID="Button_LeftMove_Customrole" Visible="false" runat="server" Text="Assign >" CssClass="buttonlooklikelinkbutton" OnClick="Button_LeftMove_Customrole_Click"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:5px;">
                                <asp:Button ID="Button_LeftMoveAll_Customrole" Visible="false" OnClick="Button_LeftMoveAll_Customrole_Click" runat="server" Text="Assign All >>" CssClass="buttonlooklikelinkbutton"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:5px;">
                                <asp:Button ID="Button_RightMove_Customrole" Visible="false" OnClick="Button_RightMove_Customrole_Click" runat="server" Text="< Un Assign" CssClass="buttonlooklikelinkbutton"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:5px;">
                                <asp:Button ID="Button_RightMoveAll_Customrole" Visible="false" OnClick="Button_RightMoveAll_Customrole_Click" runat="server" Text="<< Un Assign All" CssClass="buttonlooklikelinkbutton"/>
                                </td>
                            </tr>
                            <tr>
                                <td style="padding:5px;">
                                <asp:Button ID="btnSave_Customrole" runat="server" Visible="false" Text="Save" CssClass="btn2" OnClick="btnSave_Customrole_Click"/>
                                </td>
                            </tr>
                        </table>

            

                </div>

        

                    <div class="col-md-3">
                            <asp:HiddenField ID="HiddenField1_Customrole" runat="server" />
                            <div style="overflow-x: auto;">
                                <asp:GridView ID="GridView_Assigned_Employee_List_To_Customrole" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                                    AutoGenerateColumns="False"  
                                    AllowPaging="false" OnPageIndexChanging="GridView_Assigned_Employee_List_To_Customrole_PageIndexChanging" OnPreRender="GridView_Assigned_Employee_List_To_Customrole_PreRender">

                                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                                                                        <Columns>
                                            <asp:TemplateField>                                        
                                                <HeaderTemplate>
                                                    <asp:Label Text="Select" runat="server" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox
                                                        runat="server"
                                                        ID="chkemp" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Assigned User Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                            
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                    
                                            <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                    
                                        </Columns>
                                        <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                                                                                    </asp:GridView>
                            </div>
                        </div>
                    <div class="col-md-2"></div>
            </div>

        </div>









        <div runat="server" id="Div_Assign_Accountant" visible="false"> 
            
                    <div class="row">
                        <div class="col-md-4"></div>
                
            </div>
                    <div class="row">
                        <div class="col-md-2"></div>
                            <div class="col-md-3">
                                    <asp:HiddenField ID="hfSelected_accountant" runat="server" />
                                    <div style="overflow-x: auto;">
                                        <asp:GridView ID="GridView_UnAssigned_Business_List_To_Accountant" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                                            AutoGenerateColumns="False"  
                                            AllowPaging="false" OnPageIndexChanging="GridView_UnAssigned_Business_List_To_Accountant_PageIndexChanging" OnPreRender="GridView_UnAssigned_Business_List_To_Accountant_PreRender">

                                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                                                                               <Columns>
                                                    <asp:TemplateField HeaderStyle-Width="30px">
                                                        <HeaderTemplate>
                                                            <asp:Label Text="Select" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox
                                                                runat="server"
                                                                ID="chkemp" OnCheckedChanged="chkemp_CheckedChanged" AutoPostBack="true" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Business Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label><%--Width="100px"--%>
                                            
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                </Columns>
                                
                                            <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />

                                                                                            </asp:GridView>
                                    </div>
                                </div>

                         <div class="col-md-1">

                             <table runat="server" border="1" id="table_button_Accountant" Visible="false">
                                 <tr>
                                     <td style="padding:5px;">
                                        <asp:Button ID="Button_LeftMove_Accountant" Visible="false" runat="server" Text="Assign >" CssClass="buttonlooklikelinkbutton" OnClick="Button_LeftMove_Accountant_Click"/>
                                     </td>
                                 </tr>
                                 <tr>
                                     <td style="padding:5px;">
                                        <asp:Button ID="Button_LeftMoveAll_Accountant" Visible="false" OnClick="Button_LeftMoveAll_Accountant_Click" runat="server" Text="Assign All >>" CssClass="buttonlooklikelinkbutton"/>
                                     </td>
                                 </tr>
                                 <tr>
                                     <td style="padding:5px;">
                                        <asp:Button ID="Button_RightMove_Accountant" Visible="false" OnClick="Button_RightMove_Accountant_Click" runat="server" Text="< Un Assign" CssClass="buttonlooklikelinkbutton"/>
                                     </td>
                                 </tr>
                                 <tr>
                                     <td style="padding:5px;">
                                        <asp:Button ID="Button_RightMoveAll_Accountant" Visible="false" OnClick="Button_RightMoveAll_Accountant_Click" runat="server" Text="<< Un Assign All" CssClass="buttonlooklikelinkbutton"/>
                                     </td>
                                 </tr>
                                 <tr>
                                     <td style="padding:5px;">
                                        <asp:Button ID="btnSave_Accountant" runat="server" Visible="false" Text="Save" CssClass="btn2" OnClick="btnSave_Accountant_Click"/>
                                     </td>
                                 </tr>
                             </table>



            
                        </div>

        

                          <div class="col-md-3">
                                    <asp:HiddenField ID="HiddenField1_accountant" runat="server" />
                                    <div style="overflow-x: auto;">
                                        <asp:GridView ID="GridView_Assigned_Business_List_To_Accountant" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                                            AutoGenerateColumns="False"  
                                            AllowPaging="false" OnPageIndexChanging="GridView_Assigned_Business_List_To_Accountant_PageIndexChanging" OnPreRender="GridView_Assigned_Business_List_To_Accountant_PreRender">

                                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                                              <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label Text="Select" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox
                                                                runat="server"
                                                                ID="chkemp" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Assigned Business Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                </Columns>
                                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                                                                                            </asp:GridView>

                                        <asp:GridView ID="GridView_Assigned_Business_List_To_Accountant_2" Visible="false" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                                            AutoGenerateColumns="False"  
                                            AllowPaging="false" OnPageIndexChanging="GridView_Assigned_Business_List_To_Accountant_PageIndexChanging" OnPreRender="GridView_Assigned_Business_List_To_Accountant_PreRender">

                                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                                                                              <Columns>
                                                    <asp:TemplateField>
                                                        <HeaderTemplate>
                                                            <asp:Label Text="Select" runat="server" />
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <asp:CheckBox
                                                                runat="server"
                                                                ID="chkemp" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Assigned Business Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_empname_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                            
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                    <asp:TemplateField HeaderText="Employee Id" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_EmployeeID_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                    
                                                </Columns>
                                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                                                                                            </asp:GridView>
                                    </div>
                                </div>
                         <div class="col-md-2"></div>
                        </div>

        </div>








        </div>
    </div>
</asp:Content>

