<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ProfilesPage.aspx.cs" Inherits="ProfilesPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <%--for making the label control font size less in small screens--%>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/label_control_font_small.js"></script>
    <%--SweetAlert Javascript--%>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">

     <link href="css/upgrade.css" rel="stylesheet" />


    <script type="text/javascript">
        $(function () {
            var plans = JSON.parse($('[id*="hdf_stripePlans"]').val());
            var templatePlan = "";
            var plansWithPrice = plans.data.filter(function (plan) {
                return plan.default_price !== null && ["BASIC", "STANDARD LAN", "STANDARD PLAN"].includes(plan.name.toUpperCase());
            });
            plansWithPrice.forEach(function (plan) {
                var metaTemplate = "";
                Object.values(plan.metadata).forEach(function (meta) {
                    metaTemplate = metaTemplate + `<li>${meta}</li>`;
                });
                var metadataTemplate = `<ul class="deal-item">
                ${metaTemplate}
            </ul>`
                var priceInfo = "";
                if (plan.default_price) {
                    priceInfo = `<h4>${plan.default_price.unit_amount/100} ${plan.default_price.currency.toUpperCase()}</h4>`;
                }
                var template = `<div class="col-md-4 col-sm-6 col-xs-12">
                <div class="single-price">
                    <div class="deal-top">
                        <h3>${plan.name}</h3>
                                                ${priceInfo}
                    </div>
                    <div class="deal-bottom">
                        ${metadataTemplate}
                        <div class="btn-area">
                            <a class="signUp" href="javascript:void(0)"
                                data-pr="${plan.default_price.id}"
                                data-pl="${plan.id}">Select Plan</a>
                        </div>
                    </div>
                </div>
            </div>`;
                templatePlan = templatePlan + template;
            });

            $(".pricing-area").append(templatePlan);

            $(".signUp").on("click", function () {
                $('[id*="hdf_selectedPlanPriceId"]').val($(this).data("pr"));
                $('[id*="hdf_selectedPlanId"]').val($(this).data("pl"));
                document.getElementById("<%=lnkCheckout.ClientID %>").click();
            });
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <%--firoz--%>
    <asp:HiddenField ID="hdf_stripePlans" runat="server" />
    <asp:HiddenField ID="hdf_selectedPlanPriceId" runat="server" />
    <asp:HiddenField ID="hdf_selectedPlanId" runat="server" />
    <asp:LinkButton ID="lnkCheckout" runat="server" style="display:none" OnClick="lnkCheckout_Click"></asp:LinkButton>
    <div class="row text-center" style="display:none;">
        <h1>Plan Upgrade</h1>
        <%--<h4>You reached the limits of your plan!</h4>--%>
        <%--<h5>Please upgrade now to get more stuff</h5>--%>
        <%--<asp:Label ID="Label_Warning_Reseller_Subscriber" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />--%>
        <asp:Label ID="Label_Plan_From_To_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><%--<br />--%>
        <asp:Label ID="Label_No_Of_User_Business" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><%--<br />--%>
        <%--<asp:Label ID="Label_Information" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />--%>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-4">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a>
            </div>
            <div class="col-md-4" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="Plan Upgrade" runat="server" />
            </div>
            <div class="col-md-4" style="text-align: right; margin-top:-5px;">
                <a href="Home_Page.aspx" id="A2" runat="server">
                    <i class="fa fa-times fa-2x" aria-hidden="true"></i>
                </a>
            </div>
        </div>
    </div>

    <div class="container">
        <div class="row">
            <%--<div class="col-md-3">
                <br /><br /><br /><br /><br /><br />
                <div class="form-group">
                    <asp:Label ID="Label_Plan_Details" Text="Plan Details:" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />
                    <asp:Label ID="Label_Warning_Reseller_Subscriber" Font-Bold="true" Font-Size="Larger" ForeColor="Red" runat="server" /><br />
                    <asp:Label ID="Label_Plan_Start_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                    <asp:Label ID="Label_Plan_End_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                    <asp:Label ID="Label_No_Of_Business" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                    <asp:Label ID="Label_No_Of_Users" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                    <asp:Label ID="Label_Information" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />
                </div>
            </div>--%>
            <div class="col-md-12">
                <div class="row text-center">
                    <%--<h2>Plan Upgrade</h2>--%>
                    <div class="form-group" runat="server" id="div_plan_info">                                               
                        <%--<asp:Label ID="Label_Plan_Start_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                        <asp:Label ID="Label_Plan_End_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                        <asp:Label ID="Label_No_Of_Business" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />
                        <asp:Label ID="Label_No_Of_Users" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" /><br />--%>
                        <asp:Label ID="Label_Information" Visible="false" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />
                    </div>
                    <asp:Label ID="Label_Warning_Reseller_Subscriber" Font-Bold="true" Font-Size="Larger" ForeColor="Red" runat="server" />
                </div>

                <div class="pricing-area">
                <%--firoz--%>

                <div class="col-md-4 col-sm-6 col-xs-12">
                    <div class="single-price">
                      <div class="deal-top">
                        <h3>Basic Plan</h3>
                        <h4> FREE <span class="sup"></span> </h4>
                      </div>
                      <div class="deal-bottom">
                        <ul class="deal-item">
                            <li>1 Business</li>
                            <li>5 Users</li>
                            <li>24 Hours Support</li>
                        </ul>
                        <div class="btn-area">
                            <%--<a href="#">Select Plan</a>--%>  
                            <asp:LinkButton ID="LinkButton_BasicPlan" runat="server" ClientIDMode="Static" Text="Select Plan"/>
                        </div>
                      </div>
                    </div>
                </div>
                     
                <%--end--%>
                </div>

                <div class="col-md-4 col-sm-6 col-xs-12">
                    <div class="single-price">
                        <div class="deal-top">
                            <h3>Enterprise Plan</h3>
                            <h4  style="color: #16A086;"><span class="sup">.</span></h4>
                        </div>
                        <div class="deal-bottom">
                            <ul class="deal-item">
                                <li>Contact Us</li>
                                    <li>Customize as per Requirement</li>
                                    <li>24 Hours Support</li>
                            </ul>
                            <div class="btn-area">
                                <%--<a href="#">Select Plan</a>--%>
                                <asp:LinkButton ID="LinkButton_Enterprise_Plan" runat="server" ClientIDMode="Static" OnClick="LinkButton_Enterprise_Plan_Click" Text="Select Plan"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <br /><br />
        <div class="row" runat="server" id="table_plan_information">
            <%--<table border="1" style="width:350px">
                    <tr>
                        <td width="33%">
                            <asp:Label ID="Label_Plan_Start_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_Plan_End_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="Label_No_Of_Business" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="Label_No_Of_Users" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                        </td>
                    </tr>
                </table>--%>
            <div class="col-md-12">
            <div class="col-md-4 col-sm-6 col-xs-12">
            <!-- Current User -->
            <%--<center>--%>
                <center><asp:Label Text="Current Plan Details" Font-Bold="true" Font-Size="Large" runat="server" /></center>
                
                <table border="1" style="width: 100%;">
                    <tr>
                        <td style="padding:3px;">
                            <asp:Label id="Plan_Start_Date" Text="Plan Start Date" runat="server"></asp:Label>
                        </td>
                        <td style="padding:3px;">
                            <asp:Label id="Label_Plan_Start_Date" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding:3px;">
                            <asp:Label id="Plan_End_Date" Text="Plan End Date" runat="server"></asp:Label>
                        </td>
                        <td style="padding:3px;">
                            <asp:Label id="Label_Plan_End_Date" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding:3px;">
                            <asp:Label ID="No_Of_Busines" Text="No Of Business" runat="server" />
                        </td>
                        <td style="padding:3px;">
                            <asp:Label ID="Label_No_Of_Business" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding:3px;">
                            <asp:Label ID="No_Of_Users" Text="No Of Users" runat="server" />
                        </td>
                        <td style="padding:3px;">
                            <asp:Label ID="Label_No_Of_Users" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding:3px;">
                            <asp:Label ID="Label1" Text="Comments" runat="server" />
                        </td>
                        <td style="padding:3px;">
                            <asp:Label ID="Label2" Text="User cannot downgrade the plan" runat="server" />
                        </td>
                    </tr>
                </table>
            <%--</center>--%>
            <!-- End Current User -->
            </div>
            </div>
            <%--<center>
                <asp:Label ID="Label_Plan_Start_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label_Plan_End_Date" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label_No_Of_Business" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label_No_Of_Users" Font-Bold="true" Font-Size="Larger" ForeColor="Black" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;
            </center>--%>
        </div>
    </div>
    
    <%--end--%>
</asp:Content>

