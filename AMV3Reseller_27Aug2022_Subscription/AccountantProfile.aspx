<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="AccountantProfile.aspx.cs" Inherits="AccountantProfile" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script >
        $(document).ready(function () {
            $('#GVMother').DataTable({
            });

            $('#multi-filter-select').DataTable({
                "pageLength": 5,
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select class="form-control"><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }
            });

            // Add Row
            $('#add-row').DataTable({
                "pageLength": 5,
            });

            var action = '<td> <div class="form-button-action"> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-primary btn-lg" data-original-title="Edit Task"> <i class="fa fa-edit"></i> </button> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-danger" data-original-title="Remove"> <i class="fa fa-times"></i> </button> </div> </td>';

            $('#addRowButton').click(function () {
                $('#add-row').dataTable().fnAddData([
                    $("#addName").val(),
                    $("#addPosition").val(),
                    $("#addOffice").val(),
                    action
                ]);
                $('#addRowModal').modal('hide');

            });
        });
	</script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
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

            .panelnew {
                /*margin-top:30px;*/
                /*border-radius: 20px;*/
                /*border: 1px solid #0275d8;*/
                width:1000px;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
 <div class="panel panel-default" style="display:none;">
     <div class="panel-body">
        <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="text-align: left; margin-top: 12px;">Profile</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <a href="Accountant.aspx" id="btnback" runat="server" class="buttonhyperlink" style="margin-top: 12px;
                        float: right;">Home</a>
                </div>
            </div>
    </div>
      </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Accountant.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Profile" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Accountant.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>


   <div class="panel panel-default">
        <div class="panel-body">
            <div class="tab-content">
                
                    <div class="col-md-2">
                    </div>

                    <div class="col-md-8">

                     <div class="row" style="display:none;">
                         <div class="col-md-12">
                            <div class="form-group">
                               <label> Name</label>
                               <asp:TextBox ID="txtOrgName" runat="server" CssClass="form-control" required></asp:TextBox>
                               <asp:HiddenField ID="hfdEditId" runat="server" />
                            </div>
                         </div>
                     </div>

                       <div class="row">
                      <div class="col-md-6">
                             <div class="form-group">
                                 <label>Contact Name</label>
                                 <asp:TextBox ID="txtContactName" CssClass="form-control" runat="server" required="true" ></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Contact Number</label>
                                 <asp:TextBox ID="txtContactNumber" CssClass="form-control" runat="server" required="true"></asp:TextBox>
                             </div>
                         </div>
                          </div>

                      <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Street No.</label>
                                 <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Street Name</label>
                                 <asp:TextBox ID="txtStreetName" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                          </div>

                     <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Apartment</label>
                                 <asp:TextBox ID="txtApartment" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>City</label>
                                 <asp:TextBox ID="txtCity" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                     </div>

                   <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Province</label>
                                 <asp:TextBox ID="txtProvince" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Postal Code</label>
                                 <asp:TextBox ID="txtPostalCode" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                       </div>

                      <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Status</label>
                                 <asp:DropDownList ID="ddlStaus" CssClass="form-control" runat="server" Enabled="false">
                                     <asp:ListItem Value="">Please Select</asp:ListItem>
                                     <asp:ListItem Value="1">Active</asp:ListItem>
                                     <asp:ListItem Value="0">In-Active</asp:ListItem>
                                 </asp:DropDownList>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Enroll Date</label><span>*</span>
                                 <asp:TextBox ID="txtEnroll" TextMode="Date" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                     </div>
                    
                       <div class="row">
                         <div class="col-md-6">
                             <div class="form-group">
                               <label>Login Email</label><span>*</span>
                                <asp:TextBox ID="txtEmail" ValidationGroup="motherorg" TextMode="Email" required runat="server" CssClass="form-control" Enabled="False"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ReqEmail" ValidationGroup="AddressBusinessDetails"  runat="server" ControlToValidate="txtEmail" Display="None" ErrorMessage="This Field Is Requried"/>
                                  <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server"  TargetControlID="ReqEmail"
                                      HighlightCssClass="validatorCalloutHighlight" ></cc1:ValidatorCalloutExtender>
                              </div>
                         </div>
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <center>
                                            <asp:Button runat="server" ID="btnsave" ValidationGroup="motherorg"  Text="Save" CssClass="btn2 btn-primary" OnClick="btnsave_Click"/>
                                            <asp:HiddenField ID="hdnemailid" runat="server" />
                                    </center>
                                </div>
                            </div>
                        </div>


                     </div>

                    <div class="col-md-2">
                    </div>
                

            </div>               
       </div>
    </div>

</asp:Content>

