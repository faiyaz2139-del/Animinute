<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="MotherOrg.aspx.cs" Inherits="MotherOrg" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" Runat="Server">
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
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
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
            background-color: #607DF2;
            border-radius: 1px;
            color: white;
            padding: 10px 5px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
        .buttonhyperlink2 {
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

        .buttonhyperlink2:hover {
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
    </style>
    
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

    <script lang="Javascript" type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 189)
                return false;

            return true;
        }
    </script>
    <script lang="Javascript" type="text/javascript">
        $(document).ready(function () {
            $('#txtContactNumber').keyup(function (event) {
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

     <div class="panel panel-default">
        <div class="panel-body">
            <div class="col-md-6">
                    <h2 style="float: left; margin-top: 10px;">Add Accountant</h2>
                </div>
                <div class="col-md-6">
                    <%--<a href="SuperAdmin.aspx" id="btnback" runat="server" class="buttonhyperlink2" style="margin-top: 10px;
                        float: right;"><i class="fa fa-angle-left" aria-hidden="true"></i>Home</a>--%>
                    <a href="SuperAdmin.aspx" id="btnback" runat="server" class="buttonhyperlink2" style="margin-top: 10px;
                        float: right;">Home</a>
                </div>
        </div>
      </div>
 
    <div class="panel panel-default">
        <div class="row">
            <div class="col-md-3"></div>
            <div class="col-md-6">
                 <div class="panel-body">

                     <div class="row">
                         <div class="col-md-6">
                            <div class="form-group">
                               <label>Organisation Name</label>
                               <asp:TextBox ID="txtOrgName" runat="server" CssClass="form-control"></asp:TextBox>
                               <asp:HiddenField ID="hfdEditId" runat="server" />
                            </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Street No.</label>
                                 <asp:TextBox ID="txtStreetNo" runat="server" CssClass="form-control"></asp:TextBox>
                             </div>
                         </div>
                     </div>

                      <div class="row">
                         
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Street Name</label>
                                 <asp:TextBox ID="txtStreetName" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Apartment</label>
                                 <asp:TextBox ID="txtApartment" CssClass="form-control" runat="server"></asp:TextBox>
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
                                 <label>City</label>
                                 <asp:TextBox ID="txtCity" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                    </div>
                      <div class="row">

                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Postal Code</label>
                                 <asp:TextBox ID="txtPostalCode" MaxLength="7" CssClass="form-control" runat="server" ClientIDMode="Static" onkeyup="addHyphenn(this)" ></asp:TextBox>
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
                            <div class="col-md-6">
                             <div class="form-group">
                                 <label>Enroll Date</label><span>*</span>
                                 <asp:TextBox ID="txtEnroll" ClientIDMode="Static" TextMode="Date" CssClass="form-control" runat="server" max='31/12/9999'></asp:TextBox>
                             </div>
                                <script>
                                </script>
                         </div>
                         <div class="col-md-6" style="display:none;">
                             <div class="form-group">
                                 <label>Status</label>
                                 <asp:DropDownList ID="ddlStaus" CssClass="form-control" runat="server">
                                     <asp:ListItem Value="">Please Select</asp:ListItem>
                                     <asp:ListItem Value="1">Active</asp:ListItem>
                                     <asp:ListItem Value="0">In-Active</asp:ListItem>
                                 </asp:DropDownList>
                             </div>
                         </div>
                        </div>
                    
                       <div class="row">
                        
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Contact Name</label>
                                 <asp:TextBox ID="txtContactName" CssClass="form-control" runat="server"></asp:TextBox>
                             </div>
                         </div>
                         <div class="col-md-6">
                             <div class="form-group">
                                 <label>Contact Number</label>
                                 <asp:TextBox ID="txtContactNumber" ClientIDMode="Static" CssClass="form-control" runat="server" MaxLength="12" onkeypress="return isNumberKey(event)"></asp:TextBox>
                             </div>
                         </div>
                     </div>
                      <div class="row">
                        <asp:UpdatePanel ID="up11" runat="server" >
                                                    <ContentTemplate>
                      <div class="col-md-6">
                             <div class="form-group">
                               <label>Login Email</label><span>*</span>
                                <asp:TextBox ID="txtEmail" ValidationGroup="motherorg" TextMode="Email" runat="server" CssClass="form-control" OnTextChanged="txtEmail_TextChanged" AutoPostBack="true"></asp:TextBox>
                               <asp:HiddenField ID="hfdEmail" runat="server" />
                                  <div id="divValid" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild. Please try again
                                                </span>
                                            </div>
                                 <asp:RequiredFieldValidator ID="ReqEmail" ValidationGroup="AddressBusinessDetails"  runat="server" ControlToValidate="txtEmail" Display="None" ErrorMessage="This Field Is Requried"/>
                                  <cc1:ValidatorCalloutExtender ID="ValidatorCalloutExtender4" runat="server"  TargetControlID="ReqEmail"
                                      HighlightCssClass="validatorCalloutHighlight" ></cc1:ValidatorCalloutExtender>
                              </div>
                         </div>
                           <div class="col-md-6" id="divconfirm" runat="server" visible="false">
                                        <div class="row">
                                            <div class="col-md-10">
                                                 <div class="form-group">
                                            <label for="Name">Confirm Email Address</label>
                                            <asp:TextBox ID="txtComfirm" ClientIDMode="Static" runat="server" CssClass="form-control"  OnTextChanged="txtComfirm_TextChanged" AutoPostBack="true"></asp:TextBox>
                                            <div id="div1msg" runat="server">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                Please re-enter the Contact's email address</span>
                                            </div>
                                            <div id="div2stmsg" runat="server" visible="false">
                                                <span class="fa fa-exclamation" aria-hidden="true" style="color: red; font-size: 14px;">
                                                    The email address you've entered is not vaild. Please try again
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
                                                        <asp:AsyncPostBackTrigger ControlID="txtEmail" />
                                                         <asp:AsyncPostBackTrigger ControlID="txtComfirm" />
                                                    </Triggers>
                                                        </asp:UpdatePanel>
                      </div>
                     <div class="row">
                         <div class="col-md-8">
    <asp:CheckBox ID="CheckBox1" runat="server" Text="Act as a default accountant" /></div>
                          
                         
                     </div>
                     <div class="row">
                         <div class="col-md-4">
                             
                                 <asp:Button runat="server"  ID="btnsave" ValidationGroup="motherorg" Width="100%" Text="Save" CssClass="buttonhyperlink" OnClick="btnsave_Click"/>
                                 
                            
                         </div>
                     </div>
                         
                     <div style="height:30px;"></div>

                      <div class="row" style="display:none;">
                         <div class="col-lg-12 col-md-12">
                             <div style="overflow-x:scroll;">
                             <asp:GridView ID="GVMother" ClientIDMode="Static" runat="server" CssClass="table nowrap" CellPadding="1" ForeColor="#333333" GridLines="None" 
                                                AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender ="GVMother_PreRender">

                                 <AlternatingRowStyle BackColor="White" ForeColor="#284775" />

                                                                    <Columns>

                                                                        <asp:TemplateField HeaderText="Sl. No.">
                                                                            <ItemTemplate>
                                                                                <%#Container.DataItemIndex + 1%>
                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>





                                                                        <asp:TemplateField HeaderText="Mother Org Name">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MotherOrgName")%>'></asp:Label>
                                                                                <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MotherOrgID")%>' />


                                                                            </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="EnrollDate">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblEnrlDt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EnrollDate")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Status">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblStats" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>


                                                                        <asp:TemplateField HeaderText="Email">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Password">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="LblPswrd" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Password")%>' />
                                                                                </ItemTemplate>
                                                                        </asp:TemplateField>





                                                                        <asp:TemplateField HeaderText="Action">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton  ID="btnedit" runat="server" OnClick="btnedit_Click"  CssClass="btn" data-original-title="Edit" data-toggle="tooltip" >
                                                                                <i class="fa fa-edit"></i>
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


                </div>
        </div>
            <div class="col-md-3"></div>
        </div>
    </div>

</asp:Content>