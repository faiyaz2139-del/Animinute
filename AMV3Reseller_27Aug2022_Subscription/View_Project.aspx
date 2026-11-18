<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="View_Project.aspx.cs" Inherits="View_Project" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <%--<link href="css/bootstrap.min.css" rel="stylesheet" />--%>
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>
        

    <%--To turn autocomplete off in all textboxes except the textboxes which are under the update panel--%>
    <script type="text/javascript">
        $(document).ready(function () { $("input").attr("autocomplete", "off"); });
    </script>

    <script type="text/javascript">
        function DisableButton() {
            document.getElementById("<%=btnSaveProjectDetails.ClientID %>").disabled = true;
        }
        window.onbeforeunload = DisableButton;
    </script>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script> 

   

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

        /*.buttonhyperlink {
            text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink:hover {
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
        }*/
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
            padding: 10px 5px;
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
            padding: 10px 5px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .btn2 {
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

        .btn2:hover {
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
    </style>

    <style type="text/css">
        .buttonClass
        {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
            border-radius: 3px;
            /*padding: 2px 20px;
            text-decoration: none;
            border: solid 1px gray;
            color:gray;
            background-color: white;*/
        }
        .buttonClass:hover
        {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color:white;
            background-color: #607DF2;
            border-radius: 3px;
            /*padding: 2px 20px;
            text-decoration: none;
            border: solid 1px gray;
            color:gray;
            background-color: white;*/
        }
    </style>

    <style>
        #search-text-input{
            border-top:thin solid  #e5e5e5;
            border-right:thin solid #e5e5e5;
            border-bottom:0;
            border-left:thin solid  #e5e5e5;
            box-shadow:0px 1px 1px 1px #e5e5e5;
            float:left;
            height:17px;
            margin:.8em 0 0 .5em; 
            outline:0;
            padding:.4em 0 .4em .6em; 
            width:183px; 
        }
  
        #button-holder{
            background-color:#f1f1f1;
            border-top:thin solid #e5e5e5;
            box-shadow:1px 1px 1px 1px #e5e5e5;
            cursor:pointer;
            float:left;
            height:27px;
            margin:11px 0 0 0;
            text-align:center;
            width:50px;
        }
  
        #button-holder img{
            margin:4px;
            width:20px; 
        }
    </style>

    <script>
        $(document).ready(function () {
            $("#<%=ddl_Business_Name.ClientID%>").select2({
                placeholder: "Select Business",
                allowClear: true
            });
            <%--$("#<%=ListBox_Business_Name.ClientID%>").select2({
            });--%>
        });
    </script>

    <style type="text/css">
        .modalBackground {
            background-color: Black;
            /*filter: alpha(opacity=60);*/
            opacity: 0.6;
            overflow:auto;
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
            padding: 9px;
            overflow-y: scroll;
            height: 400px;
            width: auto;
        }
    </style>

    <%--<script>
        
        function activeTab() {
            if ($("#hdnTab").val().length == 0) {
                $("#hdnTab").val("#add_project");
            }
            $("#add_project_menu_id").attr("class", "");
            $("#view_business_wise_project_menu_id").attr("class", "");
            $("#add_project_menu").attr("class", "tab-pane fade in tab-sm-fs");
            $("#view_business_wise_project_menu").attr("class", "tab-pane fade in tab-sm-fs");
            $("#add_project").attr("class", "tab-pane fade in tab-sm-fs");
            $("#view_business_wise_project").attr("class", "tab-pane fade in");
           
            $($("#hdnTab").val() + "_menu").attr("class", "tab-pane fade in active show tab-sm-fs");
            $($("#hdnTab").val() + "_menu_id").attr("class", "active");
            $($("#hdnTab").val()).attr("class", "tab-pane fade in active tab-sm-fs");
        }
        $(document).ready(function () {
           
            $("#add_project_menu").click(function () {
                document.cookie = "tabid=#add_project";
                $("#hdnTab").val("#add_project");
            });

            $("#view_business_wise_project_menu").click(function () {
                document.cookie = "tabid=#view_business_wise_project";
               
                $("#hdnTab").val("#view_business_wise_project");
            });
            activeTab();
        });
    </script>--%>

    <%--Below css and script is needed to show none selected in the multi select list box or dropdown--%>
    <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function () {
            $('[id*=ListBox_Ddl_Business_Name]').multiselect({
                includeSelectAllOption: true
            });
        });
        $(function () {
            $('[id*=ListBox_Business_Name]').multiselect({
                includeSelectAllOption: true
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-4">
                    <h2 style="text-align: left; margin-top: 12px;">Add Project</h2>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a><br /><br />
                <asp:HyperLink href="ProjectEditor.aspx" ID="A2" runat="server" Text="Add Project"></asp:HyperLink>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="View Project" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <%--<div class="row" id="header-wrap">
        <div class="col-md-12">--%>
    <asp:HiddenField ID="hdnTab" runat="server" ClientIDMode="Static" />
            <ul class="nav nav-tabs" style="display:none;">
                <li id="add_project_menu_id"  runat="server" ClientIDMode="Static">
                    <a data-toggle="tab" id="add_project_menu" href="#add_project" ClientIDMode="Static" class="tab-sm-fs" style="text-transform: uppercase;">Create Project</a>
                </li>
                <li id="view_business_wise_project_menu_id"  ClientIDMode="Static" runat="server">
                    <a data-toggle="tab" id="view_business_wise_project_menu" href="#view_business_wise_project" ClientIDMode="Static" class="tab-sm-fs" style="text-transform: uppercase;" runat="server">View Projects</a>
                </li>
            </ul>
        <%--</div>
    </div>--%>


    <div id="container">
        <div class="panel panel-default">
            <div class="panel-body">

                <div class="tab-content">            

                    <div id="add_project" class="tab-pane fade in active show" runat="server" ClientIDMode="Static" style="display:none;">
                        <div class="row" style="display:none;">
                            <%--<div class="col-md-1">
                                
                            </div>--%>

                            <div class="col-md-4">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h4>Create Project</h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label>Project Name</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:TextBox ID="txtProjectName" ClientIDMode="Static" runat="server" Max="9999-12-31" CssClass="form-control"></asp:TextBox>
                                            <asp:Label ID="lblerror" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label>Start Date</label>&nbsp;<span style="font-weight: bold">(Required)</span>                                            
                                            <asp:TextBox ID="txtstartdate" placeholder="DD/MM/YYYY" Max="9999-12-31" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                            <asp:Label ID="lblerror1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label>End Date</label>&nbsp;<span style="font-weight: bold"></span>
                                            <asp:TextBox ID="txtenddate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label>Business Name</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                            <asp:ListBox ID="ListBox_Ddl_Business_Name" ClientIDMode="Static" runat="server" SelectionMode="Multiple" AppendDataBoundItems="true" Width="100%" CssClass="form-control js-example-basic-multiple">
                                            </asp:ListBox>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <center>
                                                <asp:Button runat="server" ID="btnSaveProjectDetails" Text="Save" OnClick="btnSaveProjectDetails_Click" CssClass="btn2 btn-primary"/>
                                            </center>
                                        </div>
                                    </div>
                                </div>

                                <%--<div class="row">
                                    <div class="col-md-12">
                                        <h4>View Project</h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <label>Business Name</label>
                                            <asp:DropDownList ID="ddl_Business_Name" runat="server" CssClass="form-control" AppendDataBoundItems="true" Width="100%" OnSelectedIndexChanged="ddl_Business_Name_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <h4>Project List</h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div style="overflow-x: auto;">
                                            <asp:GridView ID="grdviewProject" runat="server" ClientIDMode="Static"  OnPreRender="grdviewProject_PreRender"
                                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="grdviewProject_RowCommand"
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
                                                            <asp:Label ID="Label_ProjectName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>'></asp:Label>
                                                            <asp:HiddenField ID="HiddenField_ProjectID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectID")%>' />
                                                            <asp:HiddenField ID="HiddenField_BusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_ProjName" CssClass="form-control" runat="server" Text='<%#Eval("ProjectName") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Start Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_startdate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("startDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="End Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_enddate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_enddate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("EndDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Edit">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgeditproject" runat="server" ImageUrl="~/images/edit.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject"/>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                                                            <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update"/>
                                                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel"/>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Delete">
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
                                </div>--%>
                            </div>

                            <div class="col-md-3" style="display:none;">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h4>Select Business</h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                <asp:GridView ID="GridView_Business_List" runat="server" ClientIDMode="Static"
                                    CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                    GridLines="None" EmptyDataText="No Records Found" ShowHeader="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Select" ControlStyle-ForeColor="#808080"> <%--ControlStyle-Font-Size="Medium"--%> 
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkbussname" Font-Size="XX-Small" runat="server"></asp:CheckBox> <%--CssClass="chkSelect"--%>
                                                <asp:HiddenField ID="hfdBusinessID" runat="server"  Value='<%# Eval("BusinessID") %>'/>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Business Name">
                                            <ItemTemplate>
                                                <asp:Label ID="bussname" runat="server" Text='<%# Eval("BusinessName") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                                    </div>
                                </div>
                            </div>

                            <div class="col-md-5" style="display:none;">
                                <div class="row">
                                    <div class="col-md-12">
                                        <h4>Project List</h4>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <div style="overflow-x: auto;">
                                        <asp:GridView ID="GridView_Project_List" runat="server" ClientIDMode="Static" OnPreRender="GridView_Project_List_PreRender"
                                            CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                            GridLines="None" EmptyDataText="No Records Found" ShowHeader="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sl. No.">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex + 1%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Select" Visible="false" ControlStyle-ForeColor="#808080">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="CheckBox_ProjectID" runat="server"></asp:CheckBox> /*CssClass="chkSelect"*/ /*ControlStyle-Font-Size="Medium"*/
                                                        <asp:HiddenField ID="HiddenField_ProjectID" runat="server" Value='<%# Eval("ProjectID") %>'/>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Business Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label_BusinessName" runat="server" Text='<%# Eval("BusinessName") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Project Name">
                                                    <ItemTemplate>
                                                        <asp:Label ID="Label_PrtojectName" runat="server" Text='<%# Eval("PROJECTNAME") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Edit">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgeditproject" runat="server" ImageUrl="~/images/edit.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject"/>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                                                        <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update"/>
                                                        <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel"/>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Delete">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="imgdeleteproject" runat="server" OnClientClick="Confirm()" ImageUrl="~/images/delete3.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="deleteproject" />   
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        </div>
                                        <%--<div style="overflow-x: auto;">
                                            <asp:GridView ID="GridView1" runat="server" ClientIDMode="Static"  OnPreRender="grdviewProject_PreRender"
                                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="grdviewProject_RowCommand"
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
                                                            <asp:Label ID="Label_ProjectName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>'></asp:Label>
                                                            <asp:HiddenField ID="HiddenField_ProjectID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectID")%>' />
                                                            <asp:HiddenField ID="HiddenField_BusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_ProjName" CssClass="form-control" runat="server" Text='<%#Eval("ProjectName") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Start Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_startdate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("startDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="End Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_enddate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_enddate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("EndDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Edit">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgeditproject" runat="server" ImageUrl="~/images/edit.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject"/>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                                                            <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update"/>
                                                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel"/>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Delete">
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
                                        </div>--%>
                                    </div>
                                </div>
                            </div>                            

                            <div class="col-md-1">
                                
                            </div>
                        </div>
                    </div>

                    <div id="view_business_wise_project" class="tab-pane fade in active show" runat="server" ClientIDMode="Static" >
                        <%--<asp:UpdatePanel ID="updpop" runat="server" UpdateMode="Always">
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddl_Business_Name" />
                        </Triggers>
                        <ContentTemplate>--%>
                        <div class="row">
                            <%--<div class="col-md-1">
                                
                            </div>--%>

                            <div class="col-md-12">
                                <div class="row">
                                    <div class="col-md-4">
                                        <h4>Business Name</h4>
                                        <asp:DropDownList ID="ddl_Business_Name" runat="server" Width="100%" CssClass="form-control" AppendDataBoundItems="true" AutoPostBack="true"
                                            OnSelectedIndexChanged="ddl_Business_Name_SelectedIndexChanged">
                                        </asp:DropDownList>
                                        <%--<asp:ListBox ID="ListBox_Business_Name" ClientIDMode="Static" Visible="false" runat="server" SelectionMode="Multiple" AppendDataBoundItems="true" Width="100%" OnSelectedIndexChanged="ddl_Business_Name_SelectedIndexChanged"
                                            CssClass="form-control js-example-basic-multiple">
                                        </asp:ListBox>--%>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-12">
                                        <%--<div style="overflow-x: auto;">
                                            <asp:GridView ID="grdviewProject" runat="server" ClientIDMode="Static"  OnPreRender="grdviewProject_PreRender"
                                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="grdviewProject_RowCommand"
                                                ForeColor="#333333" 
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
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Start Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="End Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="enddate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label><br />                                    
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Edit">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="imgeditproject" runat="server" ImageUrl="~/images/edit.png" width="30px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject" />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Delete">
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
                                        </div>--%>
                                        <h4 runat="server" id="label_Project_List" Text="Project List"></h4>
                                        <div style="overflow-x: auto;">
                                            <asp:GridView ID="grdviewProject" runat="server" ClientIDMode="Static"  OnPreRender="grdviewProject_PreRender"
                                                CssClass="table nowrap" Width="100%" AutoGenerateColumns="False" CellPadding="1" OnRowCommand="grdviewProject_RowCommand"
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
                                                            <asp:Label ID="Label_ProjectName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ProjectName")%>'></asp:Label>
                                                            <asp:HiddenField ID="HiddenField_ProjectID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ProjectID")%>' />
                                                            <asp:HiddenField ID="HiddenField_BusinessID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_ProjName" CssClass="form-control" runat="server" Text='<%#Eval("ProjectName") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Business Name">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_BusinessName_" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "BusinessName")%>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Start Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_startdate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "startDate")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_startdate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("startDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="End Date">
                                                        <ItemTemplate>
                                                            <asp:Label ID="Label_enddate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>'></asp:Label>
                                                        </ItemTemplate>
                                                        <EditItemTemplate>  
                                                            <asp:TextBox ID="TextBox_enddate" type="date" CssClass="form-control" Max="9999-12-31" runat="server" Text='<%#Eval("EndDate") %>'></asp:TextBox>  
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>

                                                    <%--<asp:TemplateField HeaderText="Edit">--%>
                                                    <asp:TemplateField HeaderText="Action">
                                                        <ItemTemplate>

                                                            <%--<asp:ImageButton ID="imgeditproject_" runat="server" ImageUrl="~/images/edit.png" width="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject"/>
                                                            &nbsp;&nbsp;<asp:ImageButton ID="imgdeleteproject_" runat="server" OnClientClick="Confirm()" ImageUrl="~/images/delete3.png" width="20px"  CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="deleteproject" />--%>
                                                            
                                                            <asp:LinkButton ID="imgeditproject" runat="server"
                                                                CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="editproject"
                                                                data-original-title="Edit" data-toggle="tooltip">
                                                                <li class="fa fa-edit fa-lg"></li>
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="imgdeleteproject" runat="server" 
                                                                OnClientClick="Confirm()" CommandArgument='<%# ((GridViewRow) Container).RowIndex+","+Eval("ProjectID")+","+Eval("BusinessID") %>' CommandName="deleteproject" 
                                                                data-original-title="Delete" data-toggle="tooltip">
                                                                <li class="fa fa-trash fa-lg"></li>
                                                            </asp:LinkButton>

                                                        </ItemTemplate>
                                                        <EditItemTemplate>
                                                            <asp:Button ID="btn_Edit" runat="server" Text="Edit" CommandName="Edit" />
                                                            <asp:Button ID="btn_Update" runat="server" Text="Update" CommandName="Update"/>
                                                            <asp:Button ID="btn_Cancel" runat="server" Text="Cancel" CommandName="Cancel"/>
                                                        </EditItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Delete" Visible="false">
                                                        <ItemTemplate>

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

                                <%--<div class="row" style="text-align:center;display:none;">
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <asp:Label ID="lblSelectedTech" runat="server" Text="" Visible="false"></asp:Label>
                                            <asp:Button runat="server" ID="Button_Show" Width="50%" Text="Save" OnClick="Button_Show_Click" CssClass="btn2 btn-primary"/>
                                        </div>
                                    </div>
                                </div>--%>
                            </div>

                            <div class="col-md-7">
                                

                                
                            </div>

                            <div class="col-md-1">
                                
                            </div>
                        </div>
                        <%--</ContentTemplate>
                        </asp:UpdatePanel>--%>
                    </div>

                </div>                

            </div>
        </div>
    </div>


    <asp:LinkButton ID="lnkDummy2" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="modaladdproject" BehaviorID="mpe2" runat="server"
        PopupControlID="pnladdproject" TargetControlID="lnkDummy2" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide12">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnladdproject" runat="server" Width="30%" CssClass="modalPopup" Style="display: none">
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
    
                <div class="row">
                    <div class="col-md-12">
    
                            <%--<div class="row">--%>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label>Project Name</label>&nbsp;<span style="font-weight:bold">(Required)</span>
                                        <asp:TextBox ID="TextBox_Project_Name_Edit"  ClientIDMode="Static" runat="server" CssClass="form-control" onkeypress="hideIt()"></asp:TextBox>
                                        <asp:Label ID="Label1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                        <asp:Label ID="Label_Message" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                        <asp:HiddenField ID="hdn_projectname" runat="server" />
                                    </div>
                                </div>         
                            <%--</div>--%>
                                    
                            <%--<div class="row">--%>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label>Start Date</label>&nbsp;<span style="font-weight:bold">(Required)</span>                                            
                                        <asp:TextBox ID="TextBox_Start_Date_Edit" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" onkeypress="hideIt1()" runat="server"></asp:TextBox>
                                        <asp:Label ID="Label2" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                    </div>
                                </div>
                            <%--</div>--%>

                            <%--<div class="row">--%>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <label>End Date</label>&nbsp;<span style="font-weight:bold"></span>
                                        <asp:TextBox ID="TextBox_End_Date_Edit" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            <%--</div>--%>

                            <%--<div class="row">--%>
                                <div class="col-md-12">
                                    <div class="form-group">
                                        <center>
                                            <asp:Button runat="server" ID="Button_Update_Project" Text="Save" CssClass="btn2 btn-primary" OnClick="Button_Update_Project_Click"/>
                                            <asp:HiddenField ID="HiddenField_ProjedtID_ForEdit" runat="server" />
                                            <asp:HiddenField ID="HiddenField_BusinessID_ForProjectEdit" runat="server" />
                                        </center>
                                    </div>
                                </div>
                            <%--</div>--%>

                    </div>
                </div>
                               
            </ContentTemplate>
            <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="Button_Update_Project" EventName="Click"/>
            </Triggers>
        </asp:UpdatePanel>                
    </asp:Panel>

    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <%--pop up save data confirmation start--%>
    <link href="css/confirmation-pop-up.css" rel="stylesheet" />
    <cc1:ModalPopupExtender ID="Modelconfirmmessagepopup" runat="server"
        PopupControlID="pnlconfirmmessagepopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground_popup"
        CancelControlID="btnconfirmmessagepopupHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlconfirmmessagepopup" runat="server" ClientIDMode="Static" CssClass="modalPopup_popup" Style="display: none; max-height: 450px;">
        <div class="header">
            <label style="color: white;" runat="server" >Confirmation Message</label>
            <div class="pull-right" style="margin-right: 5px;">
                <button type="button" id="btnconfirmmessagepopupHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body">            
            <div class="row">
                <center>
                    <asp:label Id="Label_PopUP_Message" text="Project Updated Successfully" runat="server" />
                </center>
                <%--<div class="col-md-2"></div>
                <div class="col-md-4">
                    <div class="form-group">
                        <center>
                            <asp:Button ID="btn_Okay" runat="server" CssClass="buttonhyperlink_popup" Text="Okay"/>
                        </center>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group">
                        <center>
                            <asp:Button ID="btn_Cancel" runat="server" CssClass="buttonhyperlink_popup" Text="Cancel"/>
                        </center>
                    </div>
                </div>
                <div class="col-md-2"></div>--%>
                <div class="col-md-12">                    
                    <center>
                        <table>
                            <tr>
                                <td><asp:Button ID="btn_Okay" runat="server" OnClick="btn_Okay_Click" CssClass="buttonhyperlink_popup" Text="Ok"/></td>
                                <%--<td>&nbsp;&nbsp;</td>--%>
                                <td><asp:Button ID="btn_Cancel" runat="server" Visible="false" OnClick="btn_Cancel_Click" CssClass="buttonhyperlink_popup" Text="Cancel"/></td>
                            </tr>
                        </table>
                    </center>                    
                </div>
            </div>
        </div>
    </asp:Panel>
    <%--pop up save data confirmation end--%>

</asp:Content>