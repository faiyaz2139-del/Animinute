<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SearchAddress.aspx.cs" Inherits="SearchAddress" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

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
                    <h2 style="float: left; margin-top: 10px;">Search Address</h2>
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
                               
                                <asp:TemplateField HeaderText="Address">
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
                                         <asp:LinkButton OnClick="btndSave_Click" ID="btndSave" runat="server" Width="50px" Height="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Save_Partner" ><span class="btn btn-success">Save</span></asp:LinkButton> &nbsp;&nbsp;&nbsp;&nbsp;
                                             <asp:LinkButton OnClick="btnEdit_Click" ID="btnEdit" runat="server" Width="50px" Height="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Save_Partner">
                                                 <span class="btn btn-warning">Edit</span>
                                             </asp:LinkButton> 
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            
                             <PagerStyle   HorizontalAlign = "Right" CssClass = "GridPager" />

                        </asp:GridView>
                    </div>
                </div>
                        


                     <%--</div>--%>
                   
                </div>
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
                         <asp:LinkButton ID="btnFinalSave" ValidationGroup="persnoldetails" OnClick="btnFinalSave_Click" runat="server" Width="50px" Height="20px"><span class="btn btn-success">Save</span></asp:LinkButton>&nbsp; &nbsp;&nbsp;<asp:CheckBox ID="chkFinal" runat="server" /> Confirm<sup style="color:red;font-weight:bold">*</sup>
                    </div>
                </div>
            </div>
            
        </div>
    </asp:Panel>
           
</asp:Content>

