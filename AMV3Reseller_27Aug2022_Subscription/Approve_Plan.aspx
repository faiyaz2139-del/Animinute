<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Approve_Plan.aspx.cs" Inherits="Approve_Plan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>        

    <script lang="Javascript" type="text/javascript">
        function UseNumberAndDecimalOnly(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            /*if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)*/
            if (charCode != 46 && charCode > 31
                && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
    </script>

    <%--To turn autocomplete off in all textboxes except the textboxes which are under the update panel--%>
    <script type="text/javascript">
        $(document).ready(function () { $("input").attr("autocomplete", "off"); });
    </script>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <style>
        .buttonhyperlink {            
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: large;
        }

        .buttonhyperlink:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: large;
        }
        .buttonhyperlink22 {            
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: large;
        }

        .buttonhyperlink22:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: large;
        }
    </style>

    <style type="text/css">
        input[type=checkbox] + label {
            display: block;
            margin: 0.2em;
            cursor: pointer;
            padding: 0.2em;
        }

        input[type=checkbox] {
            display: none;
        }

        input[type=checkbox] + label:before{
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
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6">
                    <h2 style="text-align: left; margin-top: 12px;">Upgrade Plan</h2>
                </div>
                <div class="col-md-6">
                    <asp:HyperLink ID="hyperback" runat="server" NavigateUrl="~/Billing_Info.aspx" class="buttonhyperlink22" style="float:right;margin-top:12px;">
                        Home</asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-2">
                <a href="SuperAdmin_Approve_Subscriber.aspx" runat="server" style="font-size:large;">
                    Home
                </a>
                <br /><br />
                <a href="Billing_Info.aspx" id="A2" runat="server">
                    Back
                </a>
            </div>
            <div class="col-md-8" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Upgrade Plan" runat="server" />
            </div>
            <div class="col-md-2" style="text-align: right; margin-top:-5px;">
                <a href="SuperAdmin_Approve_Subscriber.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">

            <div class="row">

                <div class="col-md-2">

                </div>

                <div class="col-md-8">
                    <asp:Label ID="Label_Cancelled_Message" Font-Bold="true" Font-Size="XX-Large" ForeColor="Red" runat="server" />
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Subscriber Name</label>
                                <asp:DropDownList ID="ddl_Subscriber_Name" Visible="false" runat="server" CssClass="form-control js-example-basic-single" Width="100%" 
                                AppendDataBoundItems="true" AutoPostBack="true">
                                </asp:DropDownList>
                                <asp:Label ID="Label_Subscriber_Name" runat="server" Font-Bold="true" CssClass="form-control"></asp:Label>
                                <asp:HiddenField ID="HiddenField_Subscriber_Reseller_Email" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>No of Business in Current Plan</label>
                                <asp:TextBox ID="txtnoofbusiness" ClientIDMode="Static" ReadOnly="true" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>No of Users in Current Plan</label>
                                <asp:TextBox ID="txtnoofuser" ClientIDMode="Static" ReadOnly="true" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Current Plan Start Date</label>
                                <asp:TextBox ID="txtplanstartdate" Max="9999-12-31" placeholder="DD/MM/YYYY" ReadOnly="true" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Current Plan End Date</label>
                                <asp:TextBox ID="txtplanenddate" Max="9999-12-31" placeholder="DD/MM/YYYY" ReadOnly="true" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Current Plan Cost</label>
                                <asp:TextBox ID="txtcurrentplancost" ClientIDMode="Static" ReadOnly="true" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                    </div>

                    
                    <div class="row">

                        <div class="col-md-6">
                            <div class="form-group">
                                <label>No of Business in New Plan</label>
                                <asp:TextBox ID="Txtnoofbusinessinnewplan" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>No of Users in New Plan</label>
                                <asp:TextBox ID="Txtnoofuserinnewplan" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>New Plan Activation Date</label>
                                <asp:TextBox ID="txtnewplanactivationdate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>New Plan Start Date</label>
                                <asp:TextBox ID="Txtnewplanstartdate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>New Plan End Date</label>
                                <asp:TextBox ID="Txtnewplanenddate" Max="9999-12-31" placeholder="DD/MM/YYYY" type="date" CssClass="form-control" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>New Plan Cost</label>
                                <asp:TextBox ID="Txtnewplancost" ClientIDMode="Static" onkeypress="return UseNumberAndDecimalOnly(event)" 
                                    OnTextChanged="Txtnewplancost_TextChanged" AutoPostBack="true" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>

                    </div>

                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Total Payable Cost</label>
                                <asp:Label ID="Txttotalpayablecost" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:Label>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label></label>
                                <asp:Button runat="server" ID="btnSavePlanProjectDetails" OnClick="btnSavePlanProjectDetails_Click" Text="Save" CssClass="buttonhyperlink"/>
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label></label>
                                <asp:CheckBox ID="chk_Cancel_Plan" Text="Cancel Plan" OnCheckedChanged="chk_Cancel_Plan_CheckedChanged" AutoPostBack="true" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-4">
                            <div class="form-group">
                                <label></label>
                                <asp:CheckBox ID="chk_Activate_Plan" Text="Activate Cancelled Plan" runat="server" />
                            </div>
                        </div>
                        <div class="col-md-4" runat="server" id="div_refund" visible="false">
                            <div class="form-group">
                                <label>Refund Amount</label>
                                <asp:Label ID="Label_Refund_Amount" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:Label>
                            </div>
                        </div>
                        
                    </div>

                </div>

                <div class="col-md-2">

                </div>

            </div>

            

                    <div class="row">
                        
                    </div>

                    

                
        </div>
    </div>
</asp:Content>