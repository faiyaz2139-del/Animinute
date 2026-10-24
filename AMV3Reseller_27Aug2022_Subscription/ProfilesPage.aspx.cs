using Newtonsoft.Json;
using System;
using System.Data;
using System.Web.UI;

public partial class ProfilesPage : Page
{
    CommanClass comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                A1.HRef = "Accountant.aspx";
                A2.HRef = "Accountant.aspx";
            }  
            else
            {
                A1.HRef = "Home_Page.aspx";
                A2.HRef = "Home_Page.aspx";
            }

            var products = new PaymentRequest().FetchStripPlans(200);
            hdf_stripePlans.Value = JsonConvert.SerializeObject(products);

            #region disable plans that is not applicable. 
            var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
            bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
            if (!check_reseller_or_subscriber_email)
            {
                DataTable dt_getplanname = comm.ExecuteQueryDataTable("select PlanName,StartDate,EndDate,No_Of_Users,Business from PlanHistory with (Nolock) where EmailId='" + Session["email"].ToString() + "' and IsActive=1 ");

                if (dt_getplanname.Rows.Count > 0)
                {
                    Label_Plan_From_To_Date.Text = "Plan Start Date is " + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd") +
                        " and Plan End Date is " + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    //Label_Plan_Start_Date.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    //Label_Plan_End_Date.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    Label_Plan_Start_Date.Text = DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    Label_Plan_End_Date.Text = DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    if (dt_getplanname.Rows[0]["Business"].ToString() == "10000")
                    {
                        Label_No_Of_User_Business.Text = "No of Business is Unlimited" +
                            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                        //Label_No_Of_Business.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "Unlimited";
                        //Label_No_Of_Users.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                        Label_No_Of_Business.Text = "Unlimited";
                        Label_No_Of_Users.Text = dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                    }
                    else
                    {
                        Label_No_Of_User_Business.Text = "No of Business is " + dt_getplanname.Rows[0]["Business"].ToString() +
                            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                        //Label_No_Of_Business.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["Business"].ToString();
                        //Label_No_Of_Users.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                        Label_No_Of_Business.Text = dt_getplanname.Rows[0]["Business"].ToString();
                        Label_No_Of_Users.Text = dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                    }

                    Label_Information.Text = "User cannot downgrade the plan";
                    Label_Warning_Reseller_Subscriber.Attributes.Add("style", "display:none");

                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                    }
                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                        lnkCheckout.Enabled = false;
                    }
                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                        lnkCheckout.Enabled = false;
                    }
                }

                //if (dt_getplanname.Rows.Count > 0 )
                //{
                //    Label_Plan_From_To_Date.Text = "Plan Start Date is " + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd") + 
                //        " and Plan End Date is " + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                //    Label_Plan_Start_Date.Text = "Plan Start Date: " + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                //    Label_Plan_End_Date.Text = "Plan End Date: " + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                //    if (dt_getplanname.Rows[0]["Business"].ToString() == "10000")
                //    {
                //        Label_No_Of_User_Business.Text = "No of Business is Unlimited" +
                //            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                //        Label_No_Of_Business.Text = "No. Of Business: Unlimited";
                //        Label_No_Of_Users.Text = "No. Of Users: " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                //    }
                //    else
                //    {
                //        Label_No_Of_User_Business.Text = "No of Business is " + dt_getplanname.Rows[0]["Business"].ToString() +
                //            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                //        Label_No_Of_Business.Text = "No. Of Business: " + dt_getplanname.Rows[0]["Business"].ToString();
                //        Label_No_Of_Users.Text = "No. Of Users: " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                //    }

                //    Label_Information.Text = "User cannot downgrade the Plan";
                //    Label_Warning_Reseller_Subscriber.Attributes.Add("style", "display:none");

                //    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
                //    {
                //        LinkButton_BasicPlan.Enabled = false;
                //    }
                //    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
                //    {
                //        LinkButton_BasicPlan.Enabled = false;
                //        lnkCheckout.Enabled = false;
                //    }
                //    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
                //    {
                //        LinkButton_BasicPlan.Enabled = false;
                //        lnkCheckout.Enabled = false;
                //    }
                //}
            }
            if (check_reseller_or_subscriber_email)
            {
                if (Session["MotherOrgid"].ToString() != "1" && Session["MotherOrgid"].ToString() != "2" && Session["MotherOrgid"].ToString() != "3"
                   && Session["MotherOrgid"].ToString() != "4" && Session["MotherOrgid"].ToString() != "5" && Session["MotherOrgid"].ToString() != "1005"
                   && Session["UserType"].ToString() != "Admin")
                {
                    LinkButton_BasicPlan.Enabled = false;
                    lnkCheckout.Enabled = false;
                    LinkButton_Enterprise_Plan.Enabled = false;
                    Label_Warning_Reseller_Subscriber.Text = "Please contact Reseller";//"Please contact admin to upgrade the plan";
                    //Label_Plan_Details.Visible = false;
                    div_plan_info.Attributes.Add("style", "display:none");
                    table_plan_information.Attributes.Add("style", "display:none");
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please contact admin to upgrade the plan', 'info');", true);
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please contact Reseller', 'info');", true);
                    return;
                }

                DataTable dt_getplanname = comm.ExecuteQueryDataTable("select PlanName,StartDate,EndDate,No_Of_Users,Business from PlanHistory with (Nolock) where EmailId='" + Session["email"].ToString() + "' and IsActive=1 ");
                if (dt_getplanname.Rows.Count > 0)
                {
                    Label_Plan_From_To_Date.Text = "Plan Start Date is " + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd") +
                        " and Plan End Date is " + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    //Label_Plan_Start_Date.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    //Label_Plan_End_Date.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    Label_Plan_Start_Date.Text = DateTime.Parse(dt_getplanname.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
                    Label_Plan_End_Date.Text = DateTime.Parse(dt_getplanname.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
                    if (dt_getplanname.Rows[0]["Business"].ToString() == "10000")
                    {
                        Label_No_Of_User_Business.Text = "No of Business is Unlimited" +
                            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                        //Label_No_Of_Business.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + "Unlimited";
                        //Label_No_Of_Users.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                        Label_No_Of_Business.Text = "Unlimited";
                        Label_No_Of_Users.Text = dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                    }
                    else
                    {
                        Label_No_Of_User_Business.Text = "No of Business is " + dt_getplanname.Rows[0]["Business"].ToString() +
                            " and No of Users is " + dt_getplanname.Rows[0]["No_Of_Users"].ToString();

                        //Label_No_Of_Business.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["Business"].ToString();
                        //Label_No_Of_Users.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;" + dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                        Label_No_Of_Business.Text = dt_getplanname.Rows[0]["Business"].ToString();
                        Label_No_Of_Users.Text = dt_getplanname.Rows[0]["No_Of_Users"].ToString();
                    }
                        
                    Label_Information.Text = "User cannot downgrade the plan";

                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                    }
                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                        lnkCheckout.Enabled = false;
                    }
                    if (dt_getplanname.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
                    {
                        LinkButton_BasicPlan.Enabled = false;
                        lnkCheckout.Enabled = false;
                    }
                }
            }
            #endregion
        }
    }

    protected void lnkCheckout_Click(object sender, EventArgs e)
    {
        var paymentRequestDto = new PaymentRequestDto
        {
            EmpId = Session["EmployeeId"].ToString(),
            EmailId = Session["email"].ToString(),
            PriceId = hdf_selectedPlanPriceId.Value,
            ProductId = hdf_selectedPlanId.Value
        };

        var paymentRequestUrl = new PaymentRequest().GetPaymentUrl(paymentRequestDto);
        Response.Redirect(paymentRequestUrl);
    }

    protected void LinkButton_Enterprise_Plan_Click(object sender, EventArgs e)
    {
        Response.Redirect("Enterprise_Plan_Upgrade.aspx");
    }
}