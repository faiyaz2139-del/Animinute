<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AssgineManager.aspx.cs" Inherits="AssgineManager" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <link href="Assets/assets/css/checkliststyle.css" rel="stylesheet" />    

    <style>
        .search {
            position: relative;
            color: #aaa;
            font-size: 16px;
        }

        .search input {
            width: 250px;
            height: 32px;
            background: #fcfcfc;
            border: 1px solid #aaa;
            border-radius: 5px;
        }

        .search input {
            text-indent: 32px;
        }

        .search .fa-search {
            position: absolute;
            top: 12px;
            left: 10px;
        }
    </style>

    <script>
        $(document).ready(function () {
            $('#ddlemp').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
            });
        });
    </script>

    <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
    <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>

    <script type="text/javascript">
        $(function () {
            var a = ['', 'One', 'Two', 'Three', 'Four', 'Five', 'Six', 'Seven', 'Eight', 'Nine'];
            $('[id*=ddlemp]').multiselect({
                includeSelectAllOption: true,
                nonSelectedText: "Select Employee",
                buttonText: function (options,select) {
                    if (options.length === 0) {
                        return "Select Employee"
                    }
                    else {
                        return `${a[options.length]} Selected`;
                    }
               }
            });
       });
    </script>


    <style>
        .multiselect-container button{
            width:100%;
            border:0px;
            padding:0px;
        }
        .multiselect-container button label{
            padding:5px 5px 5px 28px !important;    
        }
        .multiselect-container button .form-check{
            float: left;
            padding:0px  !important;
            /*padding-left :5px  !important;*/
            height: auto;
            padding-top: 5px;
        }
        .multiselect-container button .form-check .form-check-label{
            font-size:12px;
        }

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

    <link rel="Stylesheet" href="https://twitter.github.io/typeahead.js/css/examples.css"/>
    <%--<script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">


      <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
            <div class="col-md-6 col-sm-6 col-xs-6">
                <h2 style="text-align:left;margin-top:12px;">Assign Employee to Manager</h2>
            </div>
            <div class="col-md-6 col-sm-6 col-xs-6">
                 <asp:HyperLink ID="hyperback" runat="server" NavigateUrl="~/Home_Page.aspx" class="buttonhyperlink" style="float:right;margin-top:12px;">
                        Home</asp:HyperLink>
            </div>
            </div>
        </div>
      </div>

    <div class="panel panel-default">
        <div class="panel-body"><br />


                <div class="row">
                    <div class="col-md-3">

                    </div>
               

                    <div class="col-md-3">
                        <div class="form-group">
                            <label>Manager Name</label>
                            <asp:DropDownList ID="ddlmanager" AppendDataBoundItems="true" runat="server" CssClass="form-control js-example-basic-single" Width="100%" OnSelectedIndexChanged="ddlmanager_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                        </div>
                        <br /><br />
                        <div class="form-group">
                            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn2 btn-primary" OnClick="btnSave_Click"/>
                        </div>
                    </div>
                    
                   
                   
                    <div class="form-group">
                        <div class="col-md-3">                        
                            <label>Employee Name</label>
                            <asp:HiddenField ID="hfSelected" runat="server" />
                            
                            <asp:ListBox ID="ddlemp" ClientIDMode="Static" runat="server" SelectionMode="Multiple" AppendDataBoundItems="true" Width="100%" CssClass="form-control js-example-basic-multiple" OnSelectedIndexChanged="ddlemp_SelectedIndexChanged" AutoPostBack="true"></asp:ListBox>
                        </div>
                    </div>

               
                </div>


                
               

                <div class="row">

                    <div class="col-md-3">

                    </div>
                    
                    <div class="col-md-3">
                        
                    </div>
                           
                 </div>
        </div>
   </div>

</asp:Content>