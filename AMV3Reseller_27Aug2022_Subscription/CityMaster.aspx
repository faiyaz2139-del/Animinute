<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="CityMaster.aspx.cs" Inherits="CityMaster" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
     <style>
        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
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
            background-color: white;
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
        .buttonClass {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }

        .buttonClass:hover {
            /*padding: 2px 20px;
            text-decoration: none;
            border: solid 1px gray;
            color:gray;
            background-color: white;*/
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }
        input[type=checkbox] + label {
            display: block;
            margin: 0.2em;
            cursor: pointer;
            padding: 0.2em;
        }

        input[type=checkbox] {
            display: none;
        }

        input[type=checkbox] + label:before {
            content: "\2714";
            border: 0.1em solid #000;
            border-radius: 0.2em;
            display: inline-block;
            width: 30px;
            height: 30px;
            padding-left: 0.2em;
            padding-bottom: 0.1em;
            margin-right: 0.2em;
            vertical-align: bottom;
            color: transparent;
            transition: .2s;
            font-size:20px;
        }

        input[type=checkbox] + label:active:before {
            transform: scale(0);
        }

        input[type=checkbox]:checked + label:before {
            background-color: #607df2;
            border-color: #607df2;
            color: #fff;  
        }

        input[type=checkbox]:disabled + label:before {
            transform: scale(1);
            border-color: #607df2;
        }

        input[type=checkbox]:checked:disabled + label:before {
            transform: scale(1);
            background-color: #607df2;
            border-color: #607df2;
        }
    </style>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <div class="panel panel-default">
        <div class="panel-body">
           <div class="row">
                    <div class="col-md-6 col-sm-6 col-xs-6">
                        <h2 style="text-align: left; margin-top: 12px;">City Master </h2>
                    </div>
                    <div class="col-md-6 col-sm-6 col-xs-6">
                        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="Home" NavigateUrl="~/SuperAdmin.aspx"
                            class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                    </div>
                </div>
        </div>
        </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-3" style="margin-left:5%"></div>
                <div class="col-md-6">
                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <label for="Name">Search City</label>
                                                <table style="width:85%">
                                                    <tr>
                                                        <td>
                                                            
                                                            <asp:TextBox ID="txtSearchPostalCode" Width="100%"  Style="text-transform: uppercase"
                                                                runat="server" CssClass="form-control" placeholder="Type City Name Press Search Button"
                                                                 ClientIDMode="Static"
                                                              ></asp:TextBox>
                                                        </td>
                                                        <td>
                                                            <asp:LinkButton Style="font-size: x-large;" Height="40.5px" CssClass="buttonClass"
                                                                ID="lnkSearch" runat="server" OnClientClick="return validatefields();"
                                                                Text="" OnClick="lnkSearch_Click"><i class="fa fa-search" aria-hidden="true"></i>
                                                            </asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <div id="resultdiv" style="border: 0px solid #349fce; border-radius: 0px; padding: 0px; width:85%"
                                        runat="server" visible="false">
                                        <div>
                                            <span style="color: gray;">SELECT THE CITY FROM THE LIST BELOW OR ADD NEW</span>
                                            <span style="color: black; font-weight: bolder">(Required)</span>
                                        </div>
                                        <br />
                                        <div>
                                            <asp:GridView ID="gvAddressList" runat="server" ClientIDMode="Static"
                                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False"
                                                GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found" Width="85%">
                                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                                <AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />
                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Address">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CityName")%>'
                                                                OnClick="StartDate_Click" />
                                                            <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CityId")%>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                                <EmptyDataTemplate>
                                                    <div>No records found.</div>
                                                </EmptyDataTemplate>
                                            </asp:GridView>
                                        </div>
                                        
                                    </div>
                            </div>
                    </div>

                    
                            <div class="clearfix"></div>

                            <div id="divAddres" runat="server" style="display: none">
                                <div class="row">
                                    
                                    <div class="col-md-12">
                                        <h4 style="text-decoration: none;">Insert New City</h4>
                                        <div class="row">
                                            
                                            <div class="col-md-5">
                                                <div class="form-group">
                                                    <label for="Name">City</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtStreetName" Style="text-transform: uppercase" required runat="server"   CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                    <asp:HiddenField ID="hfdAddressMasterId" runat="server" />
                                                    <asp:HiddenField ID="hfdStreetName" runat="server" />
                                                </div>
                                            </div>
                                            <div class="col-md-5">
                                                <div class="form-group">
                                                    <label for="Name"></label>
                                                    <asp:CheckBox ID="chkDup" Text="Marked Dulplicate" runat="server" />
                                                    
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row">

                                            <div class="col-md-3"></div>
                                            <div class="col-md-3">
                                                <asp:Button ID="txtSave" Width="100%" runat="server" OnClick="txtSave_Click" Text="Save" CssClass="btn btn-primary" />

                                            </div>
                                            <div class="col-md-6"></div>
                                        </div>
                                       

                                    </div>
                                   
                                </div>                                

                            </div>
                </div>
                <div class="col-md-3"></div>
            </div>
            </div>
        </div>
</asp:Content>

