<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true"
    CodeFile="Change_Rate_EmployeeV2.aspx.cs" Inherits="Change_Rate_EmployeeV2" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <%--for making the label control font size less in small screens--%>
    <script src="js/label_control_font_small.js"></script>

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <script lang="Javascript" type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)
                return false;

            return true;
        }
    </script> 
    
    <script type="text/javascript">
        function IsNumberWithOneDecimal(txt, evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57) && !(charCode == 46 || charCode == 8)) {
                return false;
            }//} else {
            //    var len = txt.value.length;
            //    var index = txt.value.indexOf('.');
            //    if (index > 0 && charCode == 46) {
            //        return false;
            //    }
            //    if (index > 0) {
            //        if ((len + 0) - index > 2) {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }
    </script>
    
    <script>
        var date = new Date();
        var today = new Date(date.getFullYear(), date.getMonth(), date.getDate());
        $('#txtrateHistoryEffectiveDate').attr("min", "2020-03-30");
    </script>       

    <style>

        .buttonlink {
            text-align: center;
            margin-top: 10px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonlink:hover {
            background-color: #607DF2;
            border-radius: 30px;
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

        .buttonsquarelink2 {
            text-align: center;
            padding-top: 200px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink2:hover {
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

        .buttonhyperlink {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 5px 5px;
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
            border-radius: 1px;
            color: white;
            padding: 5px 5px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink222 {
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
            font-size: medium;
        }

        .buttonhyperlink222:hover {
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
            font-size: medium;
        }

        .btnblank {
            border: 1px solid #607DF2;
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            height: 40px;
            font-family: sans-serif;
            font-size: large;
        }

        .btnblank:hover {
            border: 1px solid #607DF2;
            background-color: #607DF2;
            border-radius: 1px;
            height: 40px;
            color: white;
            font-family: sans-serif;
            font-size: large;
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
        .centerlebel{
            text-align:center;
            margin-top: 12px;
        }
        @media (max-width:767px){
            .centerlebel{
                text-align:left;
                margin-top: -10px !important;
            }
        }
    </style>

    <style type="text/css">
        .buttonClass {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }

        .buttonClass:hover {
            padding: 2px 20px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
            border-radius: 3px;
        }
    </style>

    <style type="text/css">
        .buttonClassEye {
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
        }

        .buttonClassEye:hover {
            padding: 2px 15px;
            text-decoration: none;
            border: solid 1px #607DF2;
            color: white;
            background-color: #607DF2;
        }
    </style>    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    

    <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>

    <asp:HiddenField ID="hfdMotherId" runat="server" />
   

    <cc1:ModalPopupExtender ID="ModalPopupEmailConfirm" BehaviorID="mpe" runat="server"
        PopupControlID="pnlEmailConfirm" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>


    <asp:Panel ID="pnlEmailConfirm" runat="server" ClientIDMode="Static" CssClass="modalPopup" Style="display: none;">
        <div class="header">
            Confirm Login Email
        <div class="pull-right" style="margin-right: 5px;">
            <button id="btnHide" class="btn-sm btn-danger">
               <i class="fa fa-window-close" aria-hidden="true" onclick="window.close(); window.parent.close();" ></i>
           </button>
        </div>
        </div>
        <div class="body">
             <div class="row">
            <div class="col-md-8">
                <div class="form-group">
                   <asp:TextBox ID="txtconfirmMainEmail" runat="server" TextMode="Email" CssClass="form-control" placeholder="Confirm Login Email"></asp:TextBox>
                </div>
                </div>
            <div class="col-md-4">
                <asp:Button ID="btnconfirm" runat="server" Text="Confirm Email" OnClick="btnconfirm_Click" CssClass="btn btn-primary" />
            </div>
            </div>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            Rate History
        <div class="pull-right" style="margin-right: 5px;">
            <button id="btnHide22" class="btn-sm btn-danger">
                <i class="fa fa-window-close" aria-hidden="true"></i>
            </button>
        </div>
        </div>
        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">
                        <asp:Label ID="lblNameemp" runat="server"></asp:Label>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-md-3">
                    <div class="form-group">
                        <asp:DropDownList ID="ratetype" runat="server" CssClass="form-control">
                            <asp:ListItem Value="" Selected="True"> Select Rate Type</asp:ListItem>
                            <asp:ListItem Value="1">Hourly</asp:ListItem>
                            <asp:ListItem Value="2">Weekly</asp:ListItem>
                            <asp:ListItem Value="3">Bi-Weekly</asp:ListItem>
                            <asp:ListItem Value="4">Fort Nightly</asp:ListItem>
                            <asp:ListItem Value="5">Monthly</asp:ListItem>
                            <asp:ListItem Value="6">Annually</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="Reqratetype" ValidationGroup="RateHistory" runat="server"
                            ControlToValidate="ratetype" Display="None" ErrorMessage="This Field Is Requried" />
                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender2" runat="server" TargetControlID="Reqratetype"
                            HighlightCssClass="validatorCalloutHighlight">
                        </cc1:ValidatorCalloutExtender>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <asp:TextBox ID="txtrate" runat="server" TextMode="Number" CssClass="form-control"
                            placeholder="Enter Rate"></asp:TextBox>
                        <asp:HiddenField ID="hfdchangerateid" runat="server" />
                        <asp:RequiredFieldValidator ID="Reqtxtrate" ValidationGroup="RateHistory" runat="server"
                            ControlToValidate="txtrate" Display="None" ErrorMessage="This Field Is Requried" />
                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender5" runat="server" TargetControlID="Reqtxtrate"
                            HighlightCssClass="validatorCalloutHighlight">
                        </cc1:ValidatorCalloutExtender>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <asp:TextBox ID="txtEffectiveDateRateHistory" placeholder="Effective Date" runat="server"
                            CssClass="form-control" onfocus="(this.type='date')" onfocusout="(this.type='text')"></asp:TextBox>
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <asp:Button ID="btnSaveRate" runat="server" ValidationGroup="RateHistory" CssClass="btn btn-primary"
                            Text="Change Rate" OnClick="btnSaveRate_Click" />
                    </div>
                </div>
            </div>
            <div class="row">
            </div>
            <div class="row">
            </div>
            <br />
            <div style="text-align: center;">
            </div>
        </div>
    </asp:Panel>

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
                <button id="btnconfirmmessagepopupHide" class="btn-sm btn-danger">
                    <i class="fa fa-window-close" aria-hidden="true"></i>
                </button>
            </div>
        </div>
        <div class="body">            
            <div class="row">
                <div class="col-md-12">
                    <center>
                        <asp:label Id="Label_PopUP_Message" text="" runat="server" />
                    </center>
                </div>                
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

    <asp:Panel ID="pnlload" runat="server" CssClass="modalPopup" Style="display: none;">
        <img src="assets/img/load.gif" />
    </asp:Panel>

    <div class="panel panel-default" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-8 col-sm-8 col-xs-8">
                    <div class="row">
                        <div class="col-md-6 col-sm-12 col-xs-12">
                            <h2 style="margin-top: 12px; text-align: left;">Update rate</h2>
                        </div>
                        <div class="col-md-6 col-sm-12 col-xs-12 centerlebel">
                            <h4><asp:Label ID="lblEmpNameForAddressCont____" runat="server"></asp:Label></h4>
                        </div>
                    </div>
                </div>
                <div class="col-md-4 col-sm-4 col-xs-4">
                    <a href="Home_Page.aspx" id="btnback" runat="server" class="buttonhyperlink222" style="margin-top: 12px; float: right;">
                       Home</a>
                     <%--<i class="fa fa-angle-left" aria-hidden="true"></i>--%>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-3">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <%--<asp:Label Font-Bold="true" Font-Size="X-Large" Text="Update Rate - " runat="server" /><asp:Label ID="lblEmpNameForAddressCont" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>--%>
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Payment History" runat="server" /><asp:Label ID="lblEmpNameForAddressCont" Font-Bold="true" Font-Size="X-Large" runat="server"></asp:Label>
            </div>
            <div class="col-md-3" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>
   
    <div class="panel panel-default">
        <div class="panel-body">


            <asp:HiddenField ID="hfdEmpID" runat="server" />
            <asp:HiddenField ID="hfdUserId" runat="server" />


            <div class="row" style="display:none;">
                <div class="col-md-12">
                    <ul class="nav nav-tabs">
                        <li id="homemid" class="active"><a data-toggle="tab" id="homemenu" href="#home" style="visibility:hidden;font-size: 15px; 
                            text-transform: uppercase;">Personal Details</a></li>
                        <li id="addmid"><a data-toggle="tab" href="#menu1" id="menu1menu" style="visibility:hidden;font-size: 15px;
                            text-transform: uppercase;">Address and Contacts</a></li>
                        <li id="rolemid"><a data-toggle="tab" href="#menu4" id="menu4menu" style="visibility:hidden;font-size: 15px;
                            text-transform: uppercase;">Permission Role</a></li>
                        <li id="paymeid"><a data-toggle="tab" href="#menu3" id="menu3menu" style="font-size: 15px;
                            text-transform: uppercase;">Payment History</a></li>
                        <li><a data-toggle="tab" href="#menu2" id="menu2menu" style="display: none;">Assign
                            Menu</a></li>
                    </ul>
                </div>
            </div>

            <div id="menu3" class="tab-pane fade in active show">

                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-11">
                                    <center><h4 style="text-decoration: none;">Payment Info</h4></center>
                                </div>
                                <div class="col-md-6">
                                    <div class="pull-right">
                                        <h4>
                                            <asp:Label ID="lblEmpNameForPaymentInfo" Visible="false" runat="server" Style="margin-right: 10px;"></asp:Label>
                                        </h4>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="display: none;">
                                <div class="form-group">
                                    <asp:Label ID="lblNameempRateHistory" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="col-md-12">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <asp:Label ID="Label1" runat="server"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Select Rate Type</label>
                                        <asp:DropDownList ID="ddlrateHistoryBillType" runat="server" CssClass="form-control"
                                            ValidationGroup="AddRateHistory">
                                            <asp:ListItem Value="" Selected="True"> Select Rate Type</asp:ListItem>
                                            <asp:ListItem Value="Hourly">Hourly</asp:ListItem>
                                            <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                            <asp:ListItem Value="Bi-Weekly">Bi-Weekly</asp:ListItem>
                                            <asp:ListItem Value="Fort Nightly">Fort Nightly</asp:ListItem>
                                            <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                            <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="AddRateHistory"
                                            runat="server" ControlToValidate="ddlrateHistoryBillType" Display="None" ErrorMessage="This Field Is Requried" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender8" runat="server" TargetControlID="RequiredFieldValidator1"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Bill Rate</label>
                                        <asp:TextBox ID="txtrateHistoryBillRate" runat="server" onkeypress="return IsNumberWithOneDecimal(this,event);" MaxLength="7" ValidationGroup="AddRateHistory"
                                            CssClass="form-control allownumericwithdecimal" placeholder="Enter Rate"></asp:TextBox>
                                        <asp:HiddenField ID="HiddenField1" runat="server" />
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="AddRateHistory"
                                            runat="server" ControlToValidate="txtrateHistoryBillRate" Display="None" ErrorMessage="This Field Is Requried" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender9" runat="server" TargetControlID="RequiredFieldValidator2"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>Start Date</label>
                                        <asp:TextBox ID="txtrateHistoryEffectiveDate" type="date" Max="9999-12-31" ClientIDMode="Static"
                                            placeholder="Start Date"
                                            ValidationGroup="AddRateHistory" runat="server" CssClass="form-control" onfocus="(this.type='date')"
                                            onfocusout="(this.type='text')"></asp:TextBox>
                                        <asp:TextBox ID="txtrateHistoryEffectiveDate_adding_newrate" Visible="false" placeholder="Start Date"
                                            ValidationGroup="AddRateHistory" runat="server" CssClass="form-control" onfocus="(this.type='date')"
                                            onfocusout="(this.type='text')"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="AddRateHistory"
                                            runat="server" ControlToValidate="txtrateHistoryEffectiveDate" Display="None"
                                            ErrorMessage="This Field Is Requried" />
                                        <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender10" runat="server" TargetControlID="RequiredFieldValidator3"
                                            HighlightCssClass="validatorCalloutHighlight">
                                        </cc1:ValidatorCalloutExtender>
                                        <asp:HiddenField ID="HiddenField_PreviousDate" runat="server" />
                                    </div>
                                </div>
                                <div class="col-md-3" runat="server" id="div_rate_enddate" style="display:none;">
                                    <div class="form-group">
                                        <label>End Date</label>
                                        <asp:TextBox ID="TextBox_EndDate" type="date" Max="9999-12-31" ClientIDMode="Static"
                                            placeholder="End Date"
                                            ValidationGroup="AddRateHistory" runat="server" CssClass="form-control" onfocus="(this.type='date')"
                                            onfocusout="(this.type='text')">
                                        </asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label>&nbsp;</label><br />
                                        <asp:LinkButton ID="btnrateHistoryChangeRate" runat="server" ValidationGroup="AddRateHistory"
                                            CssClass="buttonhyperlink" Width="60%" Text="Change Rate" OnClick="btnrateHistoryChangeRate_Click" />
                                    </div>
                                    <script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/1.11.2/jquery.min.js"></script>
                                    <script type="text/javascript">
             var isSubmitted = false;
             function preventMultipleSubmissions() {
                 if (!isSubmitted) {
                     $('#<%=btnrateHistoryChangeRate.ClientID %>').val('Submitting.. Plz Wait..');
                     isSubmitted = true;
                     return true;
                 }
                 else {
                     return false;
                 }
             }
                                    </script>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>



                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-7">
                            <asp:GridView ID="gvRateHistory" runat="server" ClientIDMode="Static"
                                CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvRateHistory_PreRender"
                                OnRowDataBound="gvRateHistory_RowDataBound">
                                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sl. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex + 1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Start Date" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="StartDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <%--<asp:TemplateField HeaderText="Effective Date">--%>
                                    <asp:TemplateField HeaderText="Start Date">
                                        <ItemTemplate>
                                            <asp:Label ID="effectivedate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "effectivedate")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="End Date">
                                        <ItemTemplate>
                                            <%--<asp:Label ID="EndDate" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "StartDate")%>' />--%>
                                            <asp:Label ID="EndDate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EndDate")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Bill Rate">
                                        <ItemTemplate>
                                            <asp:Label ID="Rate" runat="server" Text='<%# "$" + DataBinder.Eval(Container.DataItem, "Rate")+"/"+DataBinder.Eval(Container.DataItem, "RateType") %>'></asp:Label>
                                            <asp:HiddenField ID="hfdRateId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RateId")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Rate Type" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="RateType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "RateType")%>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Edit">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnrateEdit" runat="server" OnClick="btnrateEdit_Click" CssClass=""
                                                data-original-title="Edit" data-toggle="tooltip">
                                                                <li class="fa fa-edit fa-lg"></li>
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="text-left" />
                                    </asp:TemplateField>

                                </Columns>
                                <EmptyDataTemplate>
                                    <div>No records found.</div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                        <div class="col-md-2"></div>
                    </div>

                    <div class="row" style="display: none;">
                        <div class="col-md-3">
                            <div class="form-group">
                                <label for="txtdob">Hire Date</label>
                                <asp:TextBox ID="txthdate" runat="server" type="date" Max="9999-12-31" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="form-group">
                                <label for="txtdob">Termination Date</label>
                                <asp:TextBox ID="TxtTermDT" runat="server" class="form-control" Max="9999-12-31"
                                    type="date" placeholder="DD/MM/YYYY"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div style="height: 20px;"></div>
                    <div class="row">
                        <div class="col-md-1"></div>
                        <div class="col-md-10">
                            <div class="row">
                                <div class="col-md-5"></div>
                                <div class="col-md-2">
                                    <div class="form-group">
                                        <a data-toggle="tab" href="#menu4" id="Previoustabmenu3" class="buttonhyperlink"
                                            style="float: left;display:none;">
                                            <i class="fa fa-angle-left"></i>Previous
                                        </a>

                                        
                                    </div>
                                </div>
                                <div class="col-md-5"></div>
                            </div>
                        </div>
                        <div class="col-md-1"></div>
                    </div>
                </div>

            <div class="tab-content" style="display:none;">

                <div id="home" class="tab-pane fade in" style="display:none;">
                    <br />

                    <div class="row" style="display: none;">
                        <div class="col-md-2">
                            <div class="form-group">
                                <label for="title">User Type</label>
                                <asp:DropDownList ID="ddlusertype11" runat="server" CssClass="form-control js-example-basic-single"
                                    AppendDataBoundItems="true">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                            <div class="row" id="divsearch" runat="server">
                                <div class="col-md-11" style="padding: 0;">
                                    <asp:TextBox ID="txtsearch" Visible="false" Placeholder="Search User" runat="server" CssClass="form-control"
                                        ClientIDMode="Static">
                                    </asp:TextBox>
                                        
                                </div>
                                <div class="col-md-1" style="padding: 0;">
                                    <asp:LinkButton Style="font-size: x-large;" Height="40.5px" CssClass="buttonClass"
                                        ID="lnkSearchUser" runat="server"
                                        Text="" OnClick="lnkSearchUser_Click"><i class="fa fa-search" aria-hidden="true"></i>
                                    </asp:LinkButton>
                                </div>
                            </div>
                            <h4 style="text-decoration: none; margin-bottom: 20px;">Personal Details</h4>
                            <asp:HiddenField ID="hfdbid" runat="server" />
                            <div class="row">
                                <div class="col-md-6" style="display:none;">
                                    <div class="form-group">
                                        <label for="Name">Title</label>
                                        <asp:TextBox ID="txtLegalName1" runat="server" CssClass="form-control" Visible="false"></asp:TextBox>
                                        <asp:DropDownList ID="txtLegalName" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Male">Mr.</asp:ListItem>
                                            <asp:ListItem Value="Female">Miss.</asp:ListItem>
                                            <asp:ListItem Value="Others">Ms.</asp:ListItem>
                                            <asp:ListItem Value="Others">Mx.</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                
                            </div>

                          
                              <asp:UpdatePanel ID="up21" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>

                                    <div class="row">
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="Name">First Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                                <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="Name">Middle Name</label>
                                                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-4">
                                            <div class="form-group">
                                                <label for="Name">Last Name</label>&nbsp;&nbsp;<span style="font-weight: bold">(Required)</span>
                                                <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>

                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="txtdob">Date of Birth</label>
                                                <asp:TextBox ID="txtdob" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                                    type="date" Max="9999-12-31" placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="Reqtxtdob" ValidationGroup="persnoldetails" runat="server"
                                                    ControlToValidate="txtdob" Display="None" ErrorMessage="This Field Is Requried" />
                                                <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender6" runat="server" TargetControlID="Reqtxtdob"
                                                    PopupPosition="Right"
                                                    HighlightCssClass="validatorCalloutHighlight">
                                                    <Animations>
                                            <OnShow>
                                                <Sequence>
                                                    <HideAction Visible="true" />
                                                    <FadeIn Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                                </Sequence>
                                            </OnShow>

                                            <OnHide>
                                                <Sequence>
                                                    <FadeOut Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                                    <HideAction Visible="false" />
                                                 </Sequence>
                                            </OnHide>
                                                    </Animations>
                                                </cc1:ValidatorCalloutExtender>
                                                <asp:HiddenField ID="hfddboEncrpty" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="title">Gender</label>
                                                <asp:DropDownList ID="ddlgender" runat="server" CssClass="form-control"
                                                    Width="100%">
                                                    <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                                    <asp:ListItem Value="Male">Male</asp:ListItem>
                                                    <asp:ListItem Value="Female">Female</asp:ListItem>
                                                    <asp:ListItem Value="Others">Others</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="Name">Contact Phone</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                <asp:TextBox ID="txtMainPhone" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                    runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                               
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="Name">Mobile</label>
                                                <asp:TextBox ID="txtMobile" MaxLength="12" onkeypress="return isNumberKey(event)"
                                                    runat="server" ClientIDMode="Static" CssClass="form-control"></asp:TextBox>

                                              
                                            </div>
                                        </div>
                                       <asp:UpdatePanel ID="up11" runat="server">
                                <ContentTemplate>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="Name">Email</label>&nbsp;<span style="font-weight: bold">(To grant online access )</span>

                                            <asp:TextBox ID="txtMainEmail" runat="server" ClientIDMode="Static" CssClass="form-control"
                                                OnTextChanged="txtMainEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <div id="divValid" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild. Please try again.
                                                </span>
                                            </div>
                                            <asp:HiddenField ID="hfdMainEmail" runat="server" />
                                            <cc1:AutoCompleteExtender ServiceMethod="GetCompletionList"
                                                MinimumPrefixLength="1"
                                                CompletionInterval="10"
                                                EnableCaching="false"
                                                CompletionSetCount="1"
                                                TargetControlID="txtMainEmail"
                                                ID="AutoCompleteExtender1"
                                                runat="server"
                                                FirstRowSelected="false">
                                            </cc1:AutoCompleteExtender>

                                        </div>
                                    </div>

                                    <div class="col-md-6" id="divconfirm" runat="server" visible="false">
                                        <div class="row">
                                            <div class="col-md-10">
                                                 <div class="form-group">
                                            <label for="Name">Confirm Email Address</label>
                                            <asp:TextBox ID="txtConLoginEmail" ClientIDMode="Static" runat="server" CssClass="form-control"  OnTextChanged="txtConLoginEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <div id="div1msg" runat="server">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                Please re-enter the Confirm email address.</span>
                                            </div>
                                            <div id="div2stmsg" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild as it is not matching. Please try again.
                                                </span>
                                            </div>
                                            

                                        </div>
                                            </div>
                                            <div class="col-md-1">
                                                <div id="divtick" runat="server" visible="false" style="margin-top:20px;">
                                                <span class="fa fa-check fa-3x" aria-hidden="true" style="color:green"></span>
                                                    </div>
                                            </div>
                                        </div>
                                       
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="txtMainEmail" />
                                    <asp:AsyncPostBackTrigger ControlID="txtConLoginEmail" />
                                </Triggers>
                            </asp:UpdatePanel>

                                        <div class="col-md-12">
                                            <div class="row">
                                                <div class="col-md-6" style="display:none;">
                                                    <label id="labelsameasloginemail">Contact Email Same as Login Email?</label>
                                                    <asp:DropDownList ID="ddlCopyLoginEmail" runat="server" CssClass="form-control" ClientIDMode="Static"
                                                        onchange="Copymail(this)">
                                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                                        <asp:ListItem Value="No">No</asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                                <div class="col-md-6" style="display:none;">
                                                    <div class="form-group">
                                                        <label for="Name" style="font-weight: bold">Contact Email</label>
                                                        &nbsp;<asp:CheckBox Text="Same as Login Email" OnClick="copyvalue1(this)" runat="server"
                                                            Visible="false" />
                                                        <asp:TextBox ID="txtCCEmail" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                        <asp:HiddenField ID="hfdCCEmail" runat="server" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="txtdob">Hire Date</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                <asp:TextBox ID="txtHiredate" ValidationGroup="persnoldetails" runat="server" class="form-control"
                                                    type="date"
                                                    placeholder="DD/MM/YYYY" ClientIDMode="Static"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="ReqtxtHiredate" ValidationGroup="persnoldetails"
                                                    runat="server" ControlToValidate="txtHiredate" Display="None" ErrorMessage="This Field Is Requried" />
                                                <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender7" runat="server" TargetControlID="ReqtxtHiredate"
                                                    PopupPosition="Right"
                                                    HighlightCssClass="validatorCalloutHighlight">
                                                    <Animations>
                                                <OnShow>
                                                 <Sequence>
                                                 <HideAction Visible="true" />
                                                 <FadeIn Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                                 </Sequence>
                                                 </OnShow>

                                                <OnHide>
                                                 <Sequence>
                                                 <FadeOut Duration="1" MinimumOpacity="0" MaximumOpacity="1" />
                                                <HideAction Visible="false" />
                                                 </Sequence>
                                                 </OnHide>
                                                    </Animations>
                                                </cc1:ValidatorCalloutExtender>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="Name">Occupation</label>
                                                <asp:TextBox ID="txtOccupation" runat="server" CssClass="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="row">
                                                <div class="col-md-9">
                                                    <div class="form-group">
                                                        <label for="Name">SIN Number</label>
                                                        <asp:TextBox ID="txtSocialNumer" MaxLength="9" runat="server" placeholder="### ### ###"
                                                            CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                        <asp:HiddenField ID="hfdSocialEncrpty" runat="server" />
                                                    </div>
                                                </div>

                                                <div class="col-md-2">
                                                    <div class="form-group">
                                                        <div style="height: 25px;"></div>
                                                        <asp:LinkButton ID="lnkView" CausesValidation="false" runat="server" Font-Size="30px"
                                                            CssClass="buttonClassEye" OnClick="lnkView_Click" Visible="false">
                                                            <i id="eye" runat="server" style="font-size: 30px;" class="fa fa-eye" aria-hidden="true">
                                                            </i>
                                                        </asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <label for="txtdob">SIN Expiry Date (if any)</label>
                                                <asp:TextBox ID="txtSINExpiry" runat="server" class="form-control" type="date" Max="9999-12-31"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="lnkView" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>

                            <div class="row">
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">EI Exemptedpted</label>
                                        <asp:DropDownList ID="ddlEmpEI" runat="server" CssClass="form-control">
                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">CPP Exempted</label>
                                        <asp:DropDownList ID="ddlEmpCPP" runat="server" CssClass="form-control">
                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                            <asp:ListItem Value="No" Selected="True">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group">
                                        <label for="title">Retain Vacation?</label>
                                        <asp:DropDownList ID="ddlRTVA" runat="server" CssClass="form-control">
                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                            <asp:ListItem Value="Yes" Selected="True">Yes</asp:ListItem>
                                            <asp:ListItem Value="No">No</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                            </div>
                        </div>
                        <div class="col-md-3"></div>
                    </div>

                    <br />
                    <div class="row" style="display: none;">
                        <div class="col-md-12">
                            <h4 style="text-decoration: none;">Employment Details</h4>
                        </div>
                    </div>
                    <div class="row" style="display: none;">
                        <div class="col-md-8">
                            <div class="row">

                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtdob">Release Date</label>
                                        <asp:TextBox ID="txtReleaseDate" runat="server" class="form-control" type="date"
                                            Max="9999-12-31"></asp:TextBox>
                                    </div>
                                </div>
                            </div>



                            <div class="row">
                            </div>

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <asp:Button ID="btnSavePersonalDetails" Visible="false" runat="server" formnovalidate
                                            ValidationGroup="persnoldetails" OnClick="btnSavePersonalDetails_Click"
                                            Width="100%" Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                            CssClass="btn btn-primary" Text="Save" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>



                    <div class="row" style="margin-top:-20px">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                            <h4 style="text-decoration: none;">Payment Details</h4>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">Pay Type</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                        <asp:DropDownList ID="ddlBillingType" runat="server" CssClass="form-control" ClientIDMode="Static">
                                            <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                            <asp:ListItem Value="Hourly">Hourly</asp:ListItem>
                                            <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                            <asp:ListItem Value="Bi-Weekly">Bi-Weekly</asp:ListItem>
                                            <asp:ListItem Value="Fort Nightly">Fort Nightly</asp:ListItem>
                                            <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                            <asp:ListItem Value="Annually">Annually</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="txtCity1">Pay Rate</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                        <asp:TextBox ID="txtBillRate" runat="server" CssClass="form-control allownumericwithdecimal" ClientIDMode="Static"></asp:TextBox>
                                        
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">Employee Status</label>
                                        <asp:DropDownList ID="ddlempStatus" runat="server" ClientIDMode="Static" CssClass="form-control">
                                            <asp:ListItem Value=""> Select</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="YES">Active</asp:ListItem>
                                            <asp:ListItem Value="NO">InActive</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="title">InActive Date</label>
                                        <asp:TextBox ID="txtInActiveDate" runat="server" Max="9999-12-31" TextMode="Date"
                                            ClientIDMode="Static" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
        <asp:UpdatePanel ID="UpdatePanel_Reporting_Type_Ddl" runat="server">
        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-6">
                            <h4 style="text-decoration: none; margin-bottom: 20px;">Reporting Type</h4>
                                <asp:Label ID="Label2" Visible="false" runat="server"></asp:Label>
                                <asp:DropDownList ID="ddlusertyperegular" Width="100%" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlusertyperegular_SelectedIndexChanged"
                                            ValidationGroup="AddUserType">
                                            <asp:ListItem Value="NO" Selected="True">Select Reporting Type</asp:ListItem>
                                            <asp:ListItem Value="P">Project</asp:ListItem>
                                            <asp:ListItem Value="R">Regular</asp:ListItem>
                                            <asp:ListItem Value="N">None</asp:ListItem>
                                        </asp:DropDownList>
                                 

                        </div>
                                <div class="col-md-6">
                                <h4 style="text-decoration: none; margin-bottom: 20px;">&nbsp;</h4>
                                <asp:Label ID="Label28" Visible="false" runat="server"></asp:Label>
                                <asp:ListBox ID="ddlproject" Width="100%"   ClientIDMode="Static" runat="server" Visible="false"
                                     SelectionMode="Multiple" OnSelectedIndexChanged="ddlproject_SelectedIndexChanged"
                                     AppendDataBoundItems="true" CssClass="form-control js-example-basic-multiple"  AutoPostBack="true"></asp:ListBox>
                                    <asp:HiddenField ID="hdnproject" runat="server" />
                           
                                </div>
                            </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ddlusertyperegular" EventName="SelectedIndexChanged"/>
            </Triggers>
        </asp:UpdatePanel> 
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-md-8">
                            <div class="row">

                                <div style="display: none;">

                                    <div class="col-md-12">
                                        <div class="form-group" style="display: none;">
                                            <label for="effectiv">Start Date</label>
                                            <asp:TextBox ID="txteffectivedate" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-md-12" style="display: none;">
                                        <div class="form-group">
                                            <label for="title">Job Title</label>
                                            <asp:DropDownList ID="ddlJobTitle" runat="server" CssClass="form-control">
                                                <asp:ListItem Value="" Selected="True"> Select</asp:ListItem>
                                                <asp:ListItem Value="Cashier">Cashier</asp:ListItem>
                                                <asp:ListItem Value="Food Handler">Food Handler</asp:ListItem>
                                                <asp:ListItem Value="Cook">Cook</asp:ListItem>
                                                <asp:ListItem Value="Manager">Manager</asp:ListItem>
                                                <asp:ListItem Value="Contact Center Manager">Contact Center Manager</asp:ListItem>
                                                <asp:ListItem Value="Others">Others</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <asp:Button ID="btnSavePay" Visible="false" runat="server" Style="margin-bottom: -12px;
                                                font-family: 'Times New Roman'; font-size: large"
                                                formnovalidate Text="Save"
                                                CssClass="btn btn-primary" OnClick="btnSavePay_Click" />
                                            <asp:HiddenField ID="hfdEditRateId" runat="server" />
                                        </div>
                                    </div>
                                    <div class="col-md-12">
                                        <div class="form-group">
                                            <asp:LinkButton ID="lnkratechange" Visible="false" CssClass="btn btn-primary" runat="server"
                                                Text="Change Rate" OnClick="lnkratechange_Click"></asp:LinkButton>
                                        </div>
                                    </div>


                                </div>
                            </div>
                            <div class="row">
                            </div>
                        </div>
                    </div>


                    <div class="row" style="display: none;">
                        <div class="col-md-12">
                            <h4 style="text-decoration: none;">Add Contacts</h4>
                        </div>
                    </div>
                    <div class="row" style="display: none;">
                        <div class="col-md-8">
                            <div class="row">
                            </div>
                            <div class="row">
                            </div>
                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Website</label>
                                        <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-md-6" style="display: none;">
                                    <div class="form-group">
                                        <label for="Name">Other1</label>
                                        <asp:TextBox ID="txtOther1" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                            </div>

                            <div class="row">
                                <div class="col-md-6">
                                    <div class="form-group">
                                        <label for="Name">Work Phone</label>
                                        <asp:TextBox ID="txtWorkPhone" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                            <div class="col-md-12" style="display: none;">
                                <div class="form-group">
                                    <label for="Name">Fax</label>
                                    <asp:TextBox ID="txtFax" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-5"></div>
                                <div class="col-md-2">
                                    <a href="#" id="NextPer" class="buttonhyperlink">Next <i class="fa fa-angle-right"></i>
                                    </a>
                                </div>
                                <div class="col-md-5"></div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>
                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-5"></div>
                                <div class="col-md-2">
                                    <asp:LinkButton ID="btnUpdatePersonalDetails" runat="server" Text="Save" CssClass="btn2 btn-primary"
                                        OnClick="btnUpdatePersonalDetails_Click" ClientIDMode="Static" Visible="false" />
                                </div>
                                <div class="col-md-5"></div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>

                </div>

                <div id="menu1" class="tab-pane fade in" style="margin-top: 40px;display:none;">

                    <div class="col-md-12">
                        <div class="pull-right">
                        </div>
                    </div>


                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-md-3"></div>
                                <div class="col-md-6">
                                    <label for="Name">Enter Postal Code</label>&nbsp;<span style="color: black; font-weight: bolder;">(Required)</span>
                                    <div class="row">
                                        <div class="col-md-3">
                                            <div class="form-group">
                                                <table>
                                                    <tr>
                                                        <td>
                                                            <asp:TextBox ID="txtSearchPostalCode" Width="150px" MaxLength="7" Style="text-transform: uppercase"
                                                                runat="server" CssClass="form-control"
                                                                OnTextChanged="txtSearchPostalCode_TextChanged" AutoPostBack="false" ClientIDMode="Static"
                                                                onkeyup="addHyphenn(this)"></asp:TextBox>
                                                            <script>
             $(document).ready(function () {
                 $('#txtSearchPostalCode').keyup(function (event) {
                     addHyphenn(this);
                 });
             });

             function addHyphenn(element) {
                 let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                 let n = val.length;

                 if (n <= 6) {
                     let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                     $(element).val(finalVal);		// Update the input box.
                 }
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

                                        <div class="col-md-1" style="margin-left: -40px; margin-top: 0px;">
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3"></div>
                            </div>
                            <div class="row">
                                <div class="col-md-3"></div>
                                <div class="col-md-6">
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
                                                <AlternatingRowStyle BackColor="#f4f4f4" ForeColor="#284775" />
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
                                        <div style="display: none">
                                            <asp:RadioButtonList ID="rdobtnlist" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rdobtnlist_SelectedIndexChanged"
                                                AutoPostBack="true" Visible="false" Width="500px">
                                                <asp:ListItem Text=" Add New Address" Value="0"></asp:ListItem>
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-2">
                                        </div>
                                        <div class="col-md-2"></div>
                                    </div>
                                </div>
                                <div class="col-md-3"></div>
                            </div>



                            <div class="clearfix"></div>

                            <div id="divAddres" runat="server" style="display: none">
                                <div class="row">
                                    <div class="col-md-3"></div>
                                    <div class="col-md-6">
                                        <h4 style="text-decoration: none;">Insert New Address</h4>
                                        <div class="row">
                                            <div class="col-md-4">
                                                <div class="form-group">
                                                    <label for="Name">Street Number</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-md-8">
                                                <div class="form-group">
                                                    <label for="Name">Address - Line 1</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtStreetName" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-12">
                                                <div class="form-group">
                                                    <label for="Name">Address - Line 2</label>
                                                    <asp:TextBox ID="txtStreetType" runat="server" CssClass="form-control"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Suite Number</label>
                                                    <asp:TextBox ID="txtSuiteNumber" CssClass="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <div style="display: none;">
                                                        <label for="Name">Street Direction</label>
                                                        <asp:TextBox ID="txtStreetDirection" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </div>
                                                    <label>Suite Type</label><br />
                                                    <asp:DropDownList ID="DropDownList1"
                                                        CssClass="form-control" AppendDataBoundItems="true" runat="server" Width="100%">
                                                        <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                                        <asp:ListItem Value="Apartment" Text="Apartment"></asp:ListItem>
                                                        <asp:ListItem Value="Suite" Text="Suite"></asp:ListItem>
                                                        <asp:ListItem Value="Unit" Text="Unit"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>

                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">City</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtCity" required runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                    <cc1:AutoCompleteExtender ServiceMethod="GetAutoCompleteDataCity"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="10"
                                        EnableCaching="false"
                                        CompletionSetCount="3"
                                        TargetControlID="txtCity"
                                        ID="AutoCompleteExtender2"
                                        runat="server"
                                        FirstRowSelected="false">
                                    </cc1:AutoCompleteExtender>
                                                </div>
                                            </div>
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Province</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtProvince" runat="server" CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                    <cc1:AutoCompleteExtender ServiceMethod="GetAutoCompleteDataProvince"
                                        MinimumPrefixLength="1"
                                        CompletionInterval="10"
                                        EnableCaching="false"
                                        CompletionSetCount="3"
                                        TargetControlID="txtProvince"
                                        ID="AutoCompleteExtender3"
                                        runat="server"
                                        FirstRowSelected="false">
                                    </cc1:AutoCompleteExtender>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-md-6">
                                                <div class="form-group">
                                                    <label for="Name">Postal Code</label>&nbsp;<span style="font-weight: bold">(Required)</span>
                                                    <asp:TextBox ID="txtPostalCode" MaxLength="7" runat="server" CssClass="form-control"
                                                        ClientIDMode="Static" onkeyup="addHyphenn(this)" Style="text-transform: uppercase"></asp:TextBox>
                                                        <script>
             $(document).ready(function () {
                 $('#txtPostalCode').keyup(function (event) {
                     addHyphenn(this);
                 });
             });

             function addHyphenn(element) {
                 let val = $(element).val().split(' ').join('');   // Remove dash (-) if mistakenly entered.
                 let n = val.length;

                 if (n <= 6) {
                     let finalVal = val.match(/.{1,3}/g).join(' ');    // Add (-) after 3rd every char.
                     $(element).val(finalVal);		// Update the input box.
                 }
             }
                                                        </script>

                                                </div>
                                            </div>
                                        </div>


                                    </div>
                                    <div class="col-md-3"></div>
                                </div>



                                <div class="form-group" style="display: none;">
                                    <div class="col-md-6">
                                        <label for="ddlnumbersuffix">Number Suffix</label><span>*</span>
                                        <asp:DropDownList ID="ddlNumberSuffix"
                                            CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                            Width="100%">
                                            <asp:ListItem Text="Please Select" Value=""></asp:ListItem>
                                            <asp:ListItem Text="1" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="2" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="3" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="4" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="6" Value="6"></asp:ListItem>
                                            <asp:ListItem Text="7" Value="7"></asp:ListItem>
                                            <asp:ListItem Text="8" Value="8"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <label>SuiteType</label><br />
                                        <asp:DropDownList ID="ddlSuitType"
                                            CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                            Width="100%">
                                            <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                            <asp:ListItem Value="Duplex" Text="Duplex"></asp:ListItem>
                                            <asp:ListItem Value="Simplex" Text="Simplex"></asp:ListItem>
                                            <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                            <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                            <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <div class="col-md-10">
                                            <label for="Name">Street Type</label>
                                            <asp:TextBox ID="txtStreetType1" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <div class="col-md-8">
                                            <label for="Name">Street Direction</label>
                                            <asp:TextBox ID="txtStreetDirection1" ClientIDMode="Static" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <label for="ddlsuitetype2">SuiteType</label><br />
                                        <asp:DropDownList ID="ddlSuitType1"
                                            CssClass="form-control" AppendDataBoundItems="true" runat="server" Width="100%">
                                            <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                            <asp:ListItem Value="Duplex" Text="Duplex"></asp:ListItem>
                                            <asp:ListItem Value="Simplex" Text="Simplex"></asp:ListItem>
                                            <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                            <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                            <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-md-12">
                                    <div class="form-group" style="display: none;">
                                        <label>SuiteSuffix</label><br />
                                        <asp:DropDownList ID="ddlsuitesuffix"
                                            CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" runat="server"
                                            Width="100%">
                                            <asp:ListItem Value="" Text="Please Select"></asp:ListItem>
                                            <asp:ListItem Value="Du" Text="Du"></asp:ListItem>
                                            <asp:ListItem Value="Si" Text="Si"></asp:ListItem>
                                            <asp:ListItem Value="1BHK" Text="1BHK"></asp:ListItem>
                                            <asp:ListItem Value="2BHK" Text="2BHK"></asp:ListItem>
                                            <asp:ListItem Value="3BHK" Text="3BHK"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:Button ID="btnSaveAddres" Width="100%" Visible="false" Style="margin-bottom: -12px;
                                                    font-family: 'Times New Roman'; font-size: large"
                                                    ValidationGroup="AddressBusinessDetails"
                                                    OnClick="btnSaveAddres_Click" CssClass="btn btn-primary" Text="Save"
                                                    runat="server"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="lnkSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>






                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-md-3">
                                    <a data-toggle="tab" id="PreviousAdd" href="#home" class="buttonhyperlink" style="float: left;">
                                        <i class="fa fa-angle-left"></i>Previous
                                    </a>
                                </div>
                                <div class="col-md-6">
                                </div>
                                <div class="col-md-3">
                                    <a data-toggle="tab" href="#menu4"   id="NextAdd" class="buttonhyperlink" style="float: right;">
                                        Next <i class="fa fa-angle-right"></i></a>
                                </div>

                                
                            </div>
                        </div>
                        <div class="col-md-3"></div>
                    </div>

                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">
                                <div class="col-md-5"></div>
                                <div class="col-md-2" style="margin-top: -40px;">
                                    <asp:LinkButton ID="btnUpdateAddress" runat="server" Text="Save" OnClick="btnUpdateAddress_Click"
                                        CssClass="btn2 btn-primary" Visible="false" />
                                </div>
                                <div class="col-md-5"></div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>
                    </div>



                </div>


                <div id="menu4" class="tab-pane fade in" style="display:none;">
                    
                    
                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                            <h4 style="text-decoration: none; margin-bottom: 20px;">User Role</h4>
                            <h4 style="float: right; margin-right: 20px;">
                                <asp:Label ID="lblEmpNameForUserCredentials" Visible="false" runat="server"></asp:Label>
                            </h4>
                        </div>
                        <div class="col-md-3"></div>
                    </div>

                    <asp:UpdatePanel ID="UpdatePanel_User_Role" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                    <div class="row" style="">
                        <div class="col-md-3"></div>
                        <div class="col-md-6" style="margin-top: -50px; border: 0px solid #349fce; border-radius: 10px;">
                            <div class="form-group">
                                <asp:RadioButtonList ID="rdoBtnUserTypeList" runat="server" RepeatDirection="Vertical" OnSelectedIndexChanged="rdoBtnUserTypeList_SelectedIndexChanged" AutoPostBack="true"
                                    CellPadding="10" CssClass="radioButtonList" CellSpacing="50" RepeatColumns="1">
                                </asp:RadioButtonList>
                            </div>
                        </div>
                    </div>
                    </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlusertyperegular" />
                            <asp:AsyncPostBackTrigger ControlID="rdoBtnUserTypeList" />
                        </Triggers>
                    </asp:UpdatePanel>

                    <div class="row" runat="server" id="Div_Hide_PERMISSION_ROLE_SELECTION" style="">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
        
        <h4 runat="server" id="H4_Hide_PERMISSION_ROLE_SELECTION" style="text-align:center">
            Permission Role Selection
        </h4>
        
        <asp:UpdatePanel ID="UpdatePanel_User_Role_Permission" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
        <asp:GridView ID="GridView_User_Role_Permission" runat="server" ClientIDMode="Static"
            CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ShowHeader="False"
            GridLines="Vertical" BorderWidth="1" EmptyDataText="No Records Found"  >

            <Columns>
                <asp:TemplateField HeaderText="User Permission Role Name" ControlStyle-Font-Size="Large" ControlStyle-ForeColor="#808080">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton_Role_Permission_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Name")%>' />
                        <asp:HiddenField ID="HiddenField_Role_Permission_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Role_Permission_Id")%>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="User Permission Role Id">
                    <ItemTemplate>
                        <asp:DropDownList Class="form-control" ID="DropDownList_User_Role_Permission_Access_Status" runat="server" AutoPostBack="true">
                            <asp:ListItem Value="NO" Text="NO" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="YES" Text="YES"></asp:ListItem>                            
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div>No records found.</div>
            </EmptyDataTemplate>
        </asp:GridView>                   
        </ContentTemplate>
        </asp:UpdatePanel>                  
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-2"></div>
                        <div class="col-md-8">
                            <div class="row">

                                <div class="col-md-5"></div>
                                <div class="col-md-2" style="float: left;">
                                    <asp:LinkButton ID="btnsaveRole" runat="server" OnClientClick="return preventMultipleSubmissions();"
                                        OnClick="btnsaveRole_Click" ClientIDMode="Static" Text="Save" CssClass="btn2 btn-primary"
                                        Style="margin-left: 0px;" />

                                    <asp:LinkButton ID="btnupdateRole" runat="server" Text="Save" CssClass="btn2 btn-primary"
                                        OnClick="btnupdateRole_Click" ClientIDMode="Static" Visible="false" />
                                </div>
                                <div class="col-md-5"></div>

                            </div>
                        </div>
                        <div class="col-md-2"></div>




                        <div class="col-md-2" style="display: none;">
                            <asp:Button ID="Button1" runat="server" Width="100" OnClick="Button1_Click" ClientIDMode="Static"
                                Text="Save" CssClass="btn btn-primary" Style="margin-left: 0px;" />
                        </div>
                    </div>

                    <div style="height: 30px;"></div>

                    <div class="row">
                        <div class="col-md-3"></div>
                        <div class="col-md-6">
                            <div class="row">
                                <div class="col-md-12">
                                    <asp:GridView ID="GVDomainList" runat="server" ClientIDMode="Static"
                                        CssClass="table nowrap" AutoGenerateColumns="False" CellPadding="1" ForeColor="#333333"
                                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="GVDomainList_PreRender">
                                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                        <Columns>

                                            <asp:TemplateField HeaderText="Sl. No.">
                                                <ItemTemplate>
                                                    <%#Container.DataItemIndex + 1%>
                                                </ItemTemplate>
                                            </asp:TemplateField>





                                            <asp:TemplateField HeaderText="Name">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FirstName")%>'></asp:Label>

                                                </ItemTemplate>

                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="UserType">
                                                <ItemTemplate>
                                                    <asp:Label ID="lbluserType" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
                                                </ItemTemplate>

                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Email">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MainEmail")%>'></asp:Label>
                                                </ItemTemplate>

                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="Password" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPassword" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Password")%>'></asp:Label>
                                                </ItemTemplate>

                                            </asp:TemplateField>


                                            <asp:TemplateField HeaderText="Action" Visible="false">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnedit" runat="server" ClientIDMode="Static" CssClass="weeeeeeee"
                                                        data-original-title="Reset Password" data-toggle="tooltip">
                                                        <i class="fa fa-refresh"></i>
                                                    </asp:LinkButton>

                                                </ItemTemplate>
                                                <ItemStyle CssClass="text-left" />
                                            </asp:TemplateField>

                                        </Columns>
                                        <EmptyDataTemplate>
                                            <div>No records found.</div>
                                        </EmptyDataTemplate>


                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                        <div class="col-md-3"></div>
                    </div>

                    <div class="row">
                        <div class="col-md-1"></div>
                        <div class="col-md-10">
                            <div class="row">

                                <div class="col-md-5"></div>
                                <div class="col-md-2">
                                    <a data-toggle="tab" href="#menu1" id="PreviousUser" class="buttonhyperlink">
                                        <i class="fa fa-angle-left"></i>Previous
                                    </a>
                                    
                                </div>
                                <div class="col-md-5"></div>

                            </div>
                        </div>
                        <div class="col-md-1"></div>

                    </div>
                   
                    
                    

                </div>

                

                <div id="menu2" class="tab-pane fade in" style="display:none;">
                    <div class="row" style="border-bottom: 2px solid #349fce">
                        <div class="col-md-6">
                            <h4>Assign Menu</h4>
                        </div>
                        <div class="col-md-6">
                            <div class="pull-right">
                                <h4>
                                    <asp:Label ID="lblEmpNameAssignMenu" Visible="false" runat="server"></asp:Label>
                                </h4>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-6">



                            <div class="row">
                                <div class="col-lg-4 col-md-2   ">
                                    <div class="form-group ">
                                        <asp:Button ID="btnAssginelinSave" runat="server" formnovalidate Text="Assign Menu"
                                            OnClick="btnAssginelinSave_Click" Width="100%"
                                            Style="margin-bottom: -12px; font-family: 'Times New Roman'; font-size: large"
                                            CssClass="btn btn-primary"
                                            ValidationGroup="NumericValidate" />
                                        <br />
                                    </div>
                                </div>




                            </div>
                        </div>

                        <div class="col-lg-6 col-md-6 col-sm-6 col-xs-6">

                            <div class="row">
                                <div class="col-lg-6 col-md-6">
                                    <div class="form-group">

                                        <asp:TreeView ID="tvLinks" ClientIDMode="Static" runat="server" ImageSet="Simple2">
                                            <HoverNodeStyle Font-Underline="True" ForeColor="#5555DD" />
                                            <NodeStyle Font-Names="Verdana" Font-Size="14pt" ForeColor="Black"
                                                HorizontalPadding="5px" NodeSpacing="0px" VerticalPadding="0px" />
                                            <ParentNodeStyle Font-Bold="False" />
                                            <SelectedNodeStyle Font-Underline="True" ForeColor="#5555DD"
                                                HorizontalPadding="0px" VerticalPadding="0px" />
                                        </asp:TreeView>
                                    </div>
                                </div>
                            </div>


                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
        
</asp:Content>
