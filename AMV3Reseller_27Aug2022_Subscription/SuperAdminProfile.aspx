<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SuperAdminProfile.aspx.cs" Inherits="SuperAdminProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/label_control_font_small.js"></script>
    
    <style>
        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 0px;
            color: #607DF2;
            padding: 10px 25px;
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
            border-radius: 0px;
            color: #607DF2 ;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
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
                        <h2 style="text-align: left; margin-top: 12px;">Super Admin Profile </h2>
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
               <div class="col-lg-4">
                    <div class="form-group">
                                 <label>Email</label>
                                 <asp:TextBox ID="txtemail" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
               </div>
               <div class="col-lg-4">
                    <div class="form-group">
                                 <label>Password</label>
                                 <asp:TextBox ID="txtPassword" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
               </div>
               <div class="col-md-4">
                   <div class="form-group">
                                    <label>&nbsp;</label>
                                 <asp:Button runat="server" ID="btnsave" ValidationGroup="motherorg" Width="100%" Height="35px" Text="Save" CssClass="btn2 btn-primary" OnClick="btnsave_Click"/>
                       </div>

               </div>
               </div>
            </div>
        </div>
</asp:Content>