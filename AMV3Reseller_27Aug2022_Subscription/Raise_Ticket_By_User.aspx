<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Raise_Ticket_By_User.aspx.cs" Inherits="Raise_Ticket_By_User" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />

    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

     <style>
        .btn2 {
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
        .btn2:hover {
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
    </style>

    <script type="text/javascript">
        function DisableButton() {
            document.getElementById("<%=btn_save.ClientID %>").disabled = true;
        }
        window.onbeforeunload = DisableButton;
    </script>

    <script lang="Javascript" type="text/javascript">
        function NumbersOnly(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)
                return false;

            return true;
        }
    </script>

    <script>
        $(document).ready(function () {
            $('#txt_Phone').keyup(function (event) {
                addHyphen(this);
            });
        });

        function addHyphen(element) {
            let val = $(element).val().split('-').join('');   // Remove dash (-) if mistakenly entered.
            let n = val.length;

            if (n <= 9) {
                let finalVal = val.match(/.{1,3}/g).join('-');    // Add (-) after 3rd every char.
                $(element).val(finalVal);		// Update the input box.
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div class="panel panel-default" id="cardbus" style="display:none;">
        <div class="panel-body">
            <h2 style="text-align:left;margin-top:5px;">Create Ticket</h2>
        </div>
    </div>


    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
                <br /><br />
                <a href="Ticket_List.aspx" id="A2" runat="server">
                    View Ticket
                </a>
            </div>
            <div class="col-md-4" style="text-align: center;margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Create Ticket" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A3" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
                 <div class="panel-body">

                     <%--<div class="row text-center" style="font-size:20px;height:40px;">
                         Please provide the details of the issue
                     </div>--%>
                    
                         <div class="col-md-1"></div>
                         <div class="col-md-4">
                             <div class="row">
                                 <div class="form-group">
                                 <label>Full Name</label><span>*</span>
                                 <asp:TextBox ID="txt_fullname" required="true" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                             </div>
                              <div class="row">
                                 <div class="form-group">
                                 <label>Phone Number</label><span>*</span>
                                 <asp:TextBox ID="txt_Phone" required="true" runat="server" MaxLength="12" ClientIDMode="Static" onkeypress="return NumbersOnly(event)" CssClass="form-control"></asp:TextBox>
                                </div>
                             </div>
                               <div class="row" style="display:none;">
                                 <div class="form-group">
                                 <label>Module</label><span>*</span>
                                  <asp:DropDownList ID="ddl_module" required="true" AppendDataBoundItems="true" CssClass="form-control" runat="server">
                                    <asp:ListItem Value="">Please Select</asp:ListItem>
                                    <asp:ListItem Value="1" Selected="True">Time Sheet</asp:ListItem>
                                  </asp:DropDownList >
                                </div>
                             </div>
                               <div class="row">
                                 <div class="form-group">
                                 <label>Description of the Issue</label><span>*</span>
                                      <asp:TextBox ID="txt_Describe" TextMode="MultiLine" Rows="5" required="true" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                             </div>
                         </div>
                         <div class="col-md-1"></div>
                         <div class="col-md-4">
                            <div class="row">
                                <div class="form-group">
                                    <label>Email Address</label><span>*</span>
                                    <asp:TextBox ID="txt_Email" required="true" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                    <label>Issue Type</label><span>*</span>
                                    <%--<asp:DropDownList ID="ddl_problemtype" required="true" AppendDataBoundItems="true" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="">Please Select</asp:ListItem>
                                        <asp:ListItem Value="1">Load Issue</asp:ListItem>
                                        <asp:ListItem Value="2">Functional Issue</asp:ListItem>
                                        <asp:ListItem Value="3">Crash Issue</asp:ListItem>
                                        <asp:ListItem Value="4">Design Issue</asp:ListItem>
                                    </asp:DropDownList >--%>
                                    <asp:DropDownList ID="ddl_problemtype" required="true" AppendDataBoundItems="true" CssClass="form-control" runat="server">
                                        <asp:ListItem Value="">Please Select</asp:ListItem>
                                        <asp:ListItem Value="1">Login Issue</asp:ListItem>
                                        <asp:ListItem Value="2">Password Issue</asp:ListItem>
                                        <asp:ListItem Value="3">Application Crash Issue</asp:ListItem>
                                        <asp:ListItem Value="4">Functionality Issue</asp:ListItem>
                                        <asp:ListItem Value="5">Other</asp:ListItem>
                                    </asp:DropDownList >
                                </div>
                            </div>
                            <div class="row">
                                <div class="form-group">
                                <label>File Upload</label>
                                    <asp:FileUpload id="FileUpload" CssClass="form-control" accept=".png,.jpg,.jpeg,.pdf,.doc,.docx,.xls,.xlsx" formnovalidate="true" runat="server" />
                                </div>
                             </div>
                            <div class="row">
                                <div class="col-md-3" style="margin-top:12px;"></div>
                                <div class="col-md-3" style="margin-top:12px;">
                                    <center>
                                        <asp:Button ID="btn_save" style="margin-bottom:-12px;text-align:right;" runat="server" OnClick="btn_save_Click" Text="Save" CssClass="btn2 btn-primary" />
                                    </center>
                                </div>
                                <div class="col-md-3" style="margin-top:12px;">
                                    <center>
                                        <asp:Button ID="btn_Reset" style="margin-bottom:-12px;" runat="server" OnClick="btn_Reset_Click" Text="Reset" CssClass="btn2 btn-primary" />
                                    </center>
                                </div>
                                <div class="col-md-3" style="margin-top:12px;"></div>
                            </div>
                        </div>
                        <div class="col-md-2"></div>

                 </div>
    </div>

</asp:Content>