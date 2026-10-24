<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="PostalCode2.aspx.cs" Inherits="PostalCode2" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    
    <%--<script>
        $('.js-example-basic-single').select2({
            allowClear: true
        });
        $(document).ready(function () {
            $('#GridView_New_Registered_BO').DataTable({

            });
        });
    </script>--%>
    
   <%-- <style>
        /*22-05-22*/
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
       
        .Req{
            float:right !important;
            color:red;
        }

        /*end*/

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
    <script>
        $('.js-example-basic-single').select2({
            allowClear: true
        });
        $(document).ready(function () {
            $('#GridView_New_Registered_BO').DataTable({

            });
        });
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>--%>


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
        input[type=checkbox] + label {
            display: block;
            margin: 0.2em;
            cursor: pointer;
            padding: 0.2em;
        }

        input[type=checkbox] {
           /* display: none;*/
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
        .Req{
            float:right !important;
            color:red;
        }
    </style>
    <script>
        $('.js-example-basic-single').select2({
            allowClear: true
        });
        $(document).ready(function () {
            $('#GridView_New_Registered_BO').DataTable({

            });
        });
    </script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="float: left; margin-top: 10px;">Add/View Postal Code</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <a href="SuperAdmin.aspx" id="btnback" runat="server" class="buttonhyperlink" style="margin-top: 10px;
                        float: right;"><i class="fa fa-angle-left" aria-hidden="true"></i>Home</a>
                </div>
            </div>
        </div>
    </div>
    
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="col-md-3"></div>
                     <div class="col-md-6">
                         <label for="Name">Enter Postal Code</label>&nbsp;<span style="color: black; font-weight: bolder;">(Required)</span>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <div class="form-group">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtSearchPostalCode" Width="150px" MaxLength="7" onkeyup="addHyphenn(this)" 
                                                                runat="server" CssClass="form-control" Style="text-transform:uppercase;"></asp:TextBox>
                                                            <script>
                                                                //$(document).ready(function () {
                                                                //    $('#txtSearchPostalCode').keyup(function (event) {
                                                                //        addHyphenn(this);
                                                                //    });
                                                                //});

                                                                //function addHyphenn(element) {
                                                                //    let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                //    let n = val.length;

                                                                //    if (n <= 6) {
                                                                //        let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                //        $(element).val(finalVal);		// Update the input box.
                                                                //    }
                                                                //}

                                                                function addHyphenn(element) {
                                                                    let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                                    let n = val.length;
                                                                    let finalVal = val;
                                                                    var number_regex = /^[1-9]\d*$/;


                                                                    let c = val.charAt(n - 1);
                                                                    if (n == 2 || n == 4 || n == 6) { //num    
                                                                        if (!number_regex.test(c)) {
                                                                            finalVal = val.slice(0, -1);
                                                                        }
                                                                    }
                                                                    else if (n == 1 || n == 3 || n == 5) {
                                                                        if (number_regex.test(c)) {
                                                                            finalVal = val.slice(0, -1);
                                                                        }
                                                                    }
                                                                    if (n <= 6) {
                                                                        try {
                                                                            finalVal = finalVal.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                        }
                                                                        catch (err) { }
                                                                        // Update the input box.
                                                                    }
                                                                    $(element).val(finalVal);
                                                                }
                                                            </script>
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
                         </div>
                    
                     <%--21-05-22--%>
                                             <div class="row">
                        <div class="col-md-5">
                            <div class="form-group">
                                <label>Start Date</label>&nbsp;<span style="font-weight:bold"></span>
                                    <asp:Label ID="lblerror1" runat="server" Text="" ForeColor="Red" Visible="false"></asp:Label>
                                <asp:TextBox ID="txtstartdate"  placeholder="DD/MMM/YYYY"
                                        type="date" CssClass="form-control"  onkeypress="hideIt1()" runat="server"></asp:TextBox>                                            
                            </div>
                        </div>
                    <%--</div>

                    <div class="row">--%>
                        <div class="col-md-5">
                            <div class="form-group">
                                <label>End Date</label>&nbsp;<span style="font-weight:bold"></span>
                                <asp:TextBox ID="txtenddate"  placeholder="DD/MMM/YYYY"
                                    type="date" CssClass="form-control" runat="server"></asp:TextBox>                                            
                            </div>
                        </div>
                          <div class="col-md-2">
                              <br />
                              
                              <asp:LinkButton ID="btnSearch" OnClick="btnSearch_Click" runat="server"><span class="btn btn-primary fa fa-search">&nbsp;Search</span></asp:LinkButton>
                          </div>                       
                    </div>

                          <div class="col-lg-12 col-md-12">
                    <div">
                        <asp:GridView ID="GridView_New_Registered_BO" ClientIDMode="Static" runat="server" CssClass="table nowrap"
                            CellPadding="1" ForeColor="#333333" OnPreRender="GridView_New_Registered_BO_PreRender" GridLines="None" AllowSorting="true"
                             AutoGenerateColumns="False"
                            EmptyDataText="No records Found">
                            <AlternatingRowStyle BackColor="#f5f5f5" />

                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                

                                <asp:TemplateField HeaderText="Postal Code">
                                    <ItemTemplate>
                                        <asp:Label ID="Postalcode" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Postalcode")%>'></asp:Label>
                                                                             
                                    </ItemTemplate>
                                </asp:TemplateField>
                               
                                <asp:TemplateField HeaderText="Address Line 1">
                                    <ItemTemplate>
                                        <asp:Label ID="StreetName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StreetName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                  <asp:TemplateField HeaderText="City">
                                    <ItemTemplate>
                                        <asp:Label ID="City" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "City")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                  <asp:TemplateField HeaderText="Province">
                                    <ItemTemplate>
                                        <asp:Label ID="Province" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Province")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                        
                                       
                                   

                                <asp:TemplateField HeaderText="Action">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkCheck" runat="server" />
                                         <asp:LinkButton OnClick="btndSave_Click" ID="btndSave" runat="server" Width="50px" Height="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Save_Partner" ><span class="btn btn-primary">Save</span></asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;
                                             <asp:LinkButton OnClick="btnEdit_Click" ID="btnEdit" runat="server" Width="50px" Height="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Save_Partner">
                                                 <span class="btn btn-primary">Edit</span>
                                             </asp:LinkButton> 
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            
                             <PagerStyle   HorizontalAlign = "Right" CssClass = "GridPager" />

                        </asp:GridView>
                    </div>
                </div>
                        


                     <%--</div>--%>

                          <div class="row">
                               
                                <div class="col-md-12">
                                    <div id="resultdiv" style="border: 0px solid #349fce; border-radius: 0px; padding: 0px;"
                                        runat="server" visible="false">
                                        <div>
                                            <span style="color: gray;">SELECT THE ADDRESS FROM THE LIST BELOW OR ADD NEW</span>
                                            <span style="color: black; font-weight: bolder">(Required)</span>
                                        </div>
                                        <br />
                                        <div>
                                            <asp:GridView ID="gvAddressList" runat="server" ClientIDMode="Static"
                                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False"
                                                GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found">
                                                <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                                                <AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />
                                                <%--<HeaderStyle CssClass="bg-clouds" />--%>
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Address">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SEARCHADDRESS")%>'
                                                                OnClick="StartDate_Click" />
                                                            <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
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
                                        <h4 style="text-decoration: none;">Insert New Address</h4>
                                        <div class="row">
                                            
                                            <div class="col-md-12">
                                                <div class="form-group">
                                                    <label for="Name">Address - Line 1</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtStreetName"  required runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                    <asp:HiddenField ID="hfdAddressMasterId" runat="server" />
                                                </div>
                                            </div>
                                        </div>

                                       
                                      

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">City</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:DropDownList ID="txtCity" required runat="server" CssClass="form-control js-example-basic-single" ClientIDMode="Static" AppendDataBoundItems="true">   </asp:DropDownList>
                                                    <%--<asp:TextBox ID="txtCity" required runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>--%>
                                                    <div id="divautocompleteCity" class="tt-menu" style="display: none"></div>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Province</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:DropDownList ID="txtProvince" required runat="server" CssClass="form-control js-example-basic-single" ClientIDMode="Static" AppendDataBoundItems="true">   </asp:DropDownList>
                                                    <%--<asp:TextBox ID="txtProvince" required runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>--%>
                                                    <div id="divautocompleteProvince" class="tt-menu" style="display: none"></div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Postal Code</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtPostalCode" MaxLength="7" runat="server" CssClass="form-control"
                                                        ClientIDMode="Static" onkeyup="addHyphennew(this)" Style="text-transform: uppercase"></asp:TextBox>
                                                    <script>
                                                        //$(document).ready(function () {
                                                        //    $('#txtPostalCode').keyup(function (event) {
                                                        //        addHyphenn(this);
                                                        //    });
                                                        //});

                                                        //function addHyphenn(element) {
                                                        //    let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                        //    let n = val.length;

                                                        //    if (n <= 6) {
                                                        //        let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                        //        $(element).val(finalVal);		// Update the input box.
                                                        //    }
                                                        //}

                                                        function addHyphennew(element) {
                                                            let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                                                            let n = val.length;
                                                            let finalVal = val;
                                                            var number_regex = /^[1-9]\d*$/;


                                                            let c = val.charAt(n - 1);
                                                            if (n == 2 || n == 4 || n == 6) { //num    
                                                                if (!number_regex.test(c)) {
                                                                    finalVal = val.slice(0, -1);
                                                                }
                                                            }
                                                            else if (n == 1 || n == 3 || n == 5) {
                                                                if (number_regex.test(c)) {
                                                                    finalVal = val.slice(0, -1);
                                                                }
                                                            }
                                                            if (n <= 6) {
                                                                try {
                                                                    finalVal = finalVal.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                                                                }
                                                                catch (err) { }
                                                                // Update the input box.
                                                            }
                                                            $(element).val(finalVal);
                                                        }
                                                    </script>
                                                    <%--<asp:RegularExpressionValidator Display = "Dynamic" ControlToValidate = "txtPostalCode" ID="RegularExpressionValidator3" ValidationExpression = "^[a-zA-Z0-9'@&#.\s]{6,6}$" runat="server" ErrorMessage="Postal Code length must be 6 characters"></asp:RegularExpressionValidator>--%>
                                                </div>
                                            </div>
                                            <div class="Col-md-6">
                                                <div class="form-group" style="margin-top:30px;">
                                                    <asp:CheckBox ID="chkbxVerifed" required CssClass="checkbox-inline" runat="server" Text="Verified" />
                                                    </div>
                                                
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-md-4"></div>
                                             <div class="col-md-4">
                                                 <asp:Button ID="btnsave" Width="100%" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnsave_Click"/>
                                             </div>
                                             <div class="col-md-4"></div>
                                        </div>

                                    </div>
                                   
                                </div>



                                
                              
                                
                               

                                

                            </div>


                     <%--</div>--%>
                    <div class="col-md-3"></div>
                    </div>
                </div>

    <%--22/05/22--%>

     <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupEmpList" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="modal-content" style="width:500px !important">
            <div class="header">
                <asp:Label ID="lblbusName" runat="server"></asp:Label>
                <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
                <div class="pull-right" style="margin-right: 5px;">
                    <button id="btnHide" class="btn btn-danger">
                        <i class="fa fa-close"></i>
                    </button>
                </div>
            </div>
            <div class="body" style="overflow:hidden !important">
                <div class="row">
                    <div class="col-md-12">
                       <label>Postal Code<sup style="color:red;font-weight:bold">*</sup></label>
                                  
                                        <asp:TextBox ID="Postalcode" CssClass="form-control" runat="server"></asp:TextBox>
                                                                             
                                   <asp:RequiredFieldValidator ID="Reqtxtdob" ValidationGroup="persnoldetails" runat="server"
                                                        ControlToValidate="Postalcode" CssClass="Req"  ErrorMessage="This Field Is Requried" />
                       
                        <label>Address<sup style="color:red;font-weight:bold">*</sup></label>
                                 
                                        <asp:TextBox ID="StreetName" runat="server" CssClass="form-control" ></asp:TextBox>
                                               <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="persnoldetails" runat="server"
                                                        ControlToValidate="StreetName" CssClass="Req"  ErrorMessage="This Field Is Requried" />                              
                                  
                        <label>City<sup style="color:red;font-weight:bold">*</sup></label>
                                   
                                        <asp:TextBox ID="City" runat="server" CssClass="form-control"></asp:TextBox>
                                               <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="persnoldetails" runat="server"
                                                        ControlToValidate="City" CssClass="Req"  ErrorMessage="This Field Is Requried" />                           
                                  
                        <label>Province<sup style="color:red;font-weight:bold">*</sup></label>
                                  
                                        <asp:TextBox ID="Province" runat="server" CssClass="form-control"></asp:TextBox>
                         <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="persnoldetails" runat="server"
                                                        ControlToValidate="Province" CssClass="Req"  ErrorMessage="This Field Is Requried" />       
                        <br />
                         <asp:LinkButton ID="btnFinalSave" ValidationGroup="persnoldetails" OnClick="btnFinalSave_Click" runat="server" Width="50px" Height="20px"><span class="btn btn-primary">Verify</span></asp:LinkButton>&nbsp; &nbsp;&nbsp;<asp:CheckBox ID="chkFinal" runat="server" /> Confirm<sup style="color:red;font-weight:bold">*</sup>
                    </div>
                </div>
            </div>
            
        </div>
    </asp:Panel>
           <%--end--%>
</asp:Content>

