<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="GridviewAssignAccountant.aspx.cs" Inherits="GridviewAssignAccountant" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <%-- <script >
         $(document).ready(function () {
             try {
                 $('#GridView_EmployeeList').DataTable({
                 });
             } catch (e) {

             }
             try {
                 $('#GridViewMgr').DataTable({
                 });
             } catch (e) {

             }
            
         });
     </script>--%>

    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script>
        $(document).ready(function () {
            $('#ddlmanager').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
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
    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
     <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>
   <%--<script>
       function pageLoad(sender, e) {
           try {

          
           //$('.table1').dataTable({
           //    retrieve: 'true',
           //    responsive:'true',
           //    paging: 'false',
           //    ordering: 'false',
           //    info: 'false'

           //});
           } catch (e) {

           }
       }
       
   </script>--%>
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
                <h2 style="text-align:left;margin-top:12px;">Assign Employee To Custom Role</h2>
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
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Assign Accountant" runat="server" />
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
            <div class="row">
         <div class="col-md-4"></div>
        <div class="col-md-4">
                    <div class="form-group">
                        <label>SELECT ACCOUNTANT</label>
                        <asp:DropDownList ID="ddlmanager" AppendDataBoundItems="true" runat="server" CssClass="form-control js-example-basic-single" Width="100%" OnSelectedIndexChanged="ddlmanager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                    </div>
                </div>
    </div>
     <div class="row">
         <div class="col-md-2"></div>
                <div class="col-md-3">
                    <asp:HiddenField ID="hfSelected" runat="server" />
                    <div style="overflow-x: auto;">
                        <asp:GridView ID="GridView_EmployeeList" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                            AutoGenerateColumns="False" EmptyDataText="No records Found" 
                            AllowPaging="false" OnPageIndexChanging="GridView_EmployeeList_PageIndexChanging" OnPreRender="GridView_EmployeeList_PreRender">

                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                               <Columns>
                                    <asp:TemplateField HeaderStyle-Width="30px">
                                        <HeaderTemplate>
                                            <asp:Label Text="Select" runat="server" />
                                            <%--<asp:CheckBox
                                                runat="server"
                                                class="chkSelectAll2" />--%>
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
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                
                            <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />

                                                                            </asp:GridView>
                    </div>
                </div>
            <%--</div>
            </div>
        </div>--%>

    <%--<div class="row">--%>
       
        <%-- <div class="col-md-5">
             <asp:HiddenField ID="hfSelected" runat="server" />

             <div style=" width:400px; height:400px; overflow-x:auto;">
            <asp:GridView ID="GridView_EmployeeList" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" GridLines="None" EmptyDataText="No Records Found" AllowPaging="true" OnPageIndexChanging="GridView_EmployeeList_PageIndexChanging" >
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox
                                                runat="server"
                                                class="chkSelectAll2" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                runat="server"
                                                ID="chkemp" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Employee Name">
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
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
                 </div>
        </div>--%>
         <div class="col-md-1">

             <table runat="server" border="1" id="table_button" Visible="false">
                 <tr>
                     <td style="padding:5px;">
                        <asp:Button ID="Button_LeftMove" Visible="false" runat="server" Text="Assign >" CssClass="buttonlooklikelinkbutton" OnClick="Button_LeftMove_Click"/>
                     </td>
                 </tr>
                 <tr>
                     <td style="padding:5px;">
                        <asp:Button ID="Button_LeftMoveAll" Visible="false" OnClick="Button_LeftMoveAll_Click" runat="server" Text="Assign All >>" CssClass="buttonlooklikelinkbutton"/>
                     </td>
                 </tr>
                 <tr>
                     <td style="padding:5px;">
                        <asp:Button ID="Button_RightMove" Visible="false" OnClick="Button_RightMove_Click" runat="server" Text="< Un Assign" CssClass="buttonlooklikelinkbutton"/>
                     </td>
                 </tr>
                 <tr>
                     <td style="padding:5px;">
                        <asp:Button ID="Button_RightMoveAll" Visible="false" OnClick="Button_RightMoveAll_Click" runat="server" Text="<< Un Assign All" CssClass="buttonlooklikelinkbutton"/>
                     </td>
                 </tr>
                 <tr>
                     <td style="padding:5px;">
                        <asp:Button ID="btnSave" runat="server" Visible="false" Text="Save" CssClass="btn2" OnClick="btnSave_Click"/>
                     </td>
                 </tr>
             </table>



            <%--<center>--%>
                    
                    
                <%--</center


                <center>--%>
                    <br /><br />
                    
                <%--</center>

            <center>--%>
                    <br /><br />
                    
                <%--</center>

            <center>--%>
                    <br /><br />
                    
                <%--</center>
              <center>--%>
                    <br /><br />
                    
                <%--</center>--%>
             <br /><br />
        </div>

        <%-- <div class="col-md-5">
              <div style=" width:300px; height:300px; overflow-x:auto;">
            <asp:GridView ID="GridViewMgr" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1"
                                ForeColor="#333333" GridLines="None" EmptyDataText="No Records Found" >
                                <AlternatingRowStyle BackColor="#f5f5f5" />
                                <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:CheckBox
                                                runat="server"
                                                class="chkSelectAll2" />
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:CheckBox
                                                runat="server"
                                                ID="chkemp" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Assign">
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
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />
                            </asp:GridView>
              </div>
        </div>--%>

          <div class="col-md-3">
                    <asp:HiddenField ID="HiddenField1" runat="server" />
                    <div style="overflow-x: auto;">
                        <asp:GridView ID="GridViewMgr" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                            AutoGenerateColumns="False" EmptyDataText="No records Found" 
                            AllowPaging="false" OnPageIndexChanging="GridViewMgr_PageIndexChanging" OnPreRender="GridViewMgr_PreRender">

                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                              <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label Text="Select" runat="server" />
                                            <%--<asp:CheckBox
                                                runat="server"
                                                class="chkSelectAll2" />--%>
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
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                                                                            </asp:GridView>

                        <asp:GridView ID="GridView_UnAssigned_AccountantList" Visible="false" ClientIDMode="Static" runat="server" CssClass="table nowrap divTable" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                            AutoGenerateColumns="False" EmptyDataText="No records Found" 
                            AllowPaging="false" OnPageIndexChanging="GridViewMgr_PageIndexChanging" OnPreRender="GridViewMgr_PreRender">

                                             <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>

                                                                              <Columns>
                                    <asp:TemplateField>
                                        <HeaderTemplate>
                                            <asp:Label Text="Select" runat="server" />
                                            <%--<asp:CheckBox
                                                runat="server"
                                                class="chkSelectAll2" />--%>
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
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                                <PagerStyle HorizontalAlign="Right" CssClass="GridPager" />

                                                                            </asp:GridView>
                    </div>
                </div>
         <div class="col-md-2"></div>
        </div>
            </div>
        </div>
   
</asp:Content>

