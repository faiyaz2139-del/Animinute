using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

public partial class Approve_Plan : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    string subscriber_email;
    string subscriber_name;
    string Subscriber_Or_Reseller;
    EmailSettingController Esctrl = new EmailSettingController();

    
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            txtnewplanactivationdate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            Find_Email_Name();
            Label_Subscriber_Name.Text = subscriber_name;
            HiddenField_Subscriber_Reseller_Email.Value = subscriber_email;
            Get_ActivePlanDetails(int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()));
            var check_Cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' " +
                "and Is_Cancelled = 1 ");
            if (check_Cancelled.Rows.Count > 0)
            {
                Label_Cancelled_Message.Text = "This Subscriber Is Cancelled";
                txtcurrentplancost.Text = "0";
            }
        }
    }

    protected void btnSavePlanProjectDetails_Click(object sender, EventArgs e)
    {
        if (chk_Activate_Plan.Checked)
        {
            comm.ExecuteQuery("update PlanHistory set Is_Cancelled=NULL where EmailId='" + HiddenField_Subscriber_Reseller_Email.Value + "' " +
                "and EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The Plan has been Activated Successfully', 'success');", true);

            var check_Cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' " +
                "and Is_Cancelled = 1 ");
            if (check_Cancelled.Rows.Count == 0)
            {
                Label_Cancelled_Message.Text = "";
            }


            //After activating the cancelled plan user has to enter all values once again.
            if (Txtnoofbusinessinnewplan.Text == "" && Txtnewplancost.Text.Trim() == "0")
            {
                Txtnoofbusinessinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter free no of business', 'warning');", true);
                return;
            }
            if (Txtnoofbusinessinnewplan.Text == "")
            {
                Txtnoofbusinessinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of business', 'warning');", true);
                return;
            }

            if (Txtnoofuserinnewplan.Text == "" && Txtnewplancost.Text.Trim() == "0")
            {
                Txtnoofuserinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter free no of user', 'warning');", true);
                return;
            }
            if (Txtnoofuserinnewplan.Text == "")
            {
                Txtnoofuserinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of user', 'warning');", true);
                return;
            }

            if (txtnewplanactivationdate.Text == "")
            {
                txtnewplanactivationdate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter activation date', 'warning');", true);
                return;
            }

            if (Txtnewplanstartdate.Text == "")
            {
                Txtnewplanstartdate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter start date', 'warning');", true);
                return;
            }

            if (Txtnewplanenddate.Text == "")
            {
                Txtnewplanenddate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter end date', 'warning');", true);
                return;
            }

            if (Txtnewplanstartdate.Text.Trim().Length > 0 && Txtnewplanenddate.Text.Trim().Length > 0)
            {
                DateTime From_Date = DateTime.Parse(Txtnewplanstartdate.Text);
                DateTime To_Date = DateTime.Parse(Txtnewplanenddate.Text);
                if (From_Date > To_Date)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date is greater than End date!', 'warning');", true);
                    return;
                }
            }

            if (Txtnewplancost.Text == "")
            {
                Txtnewplancost.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter new plan cost', 'warning');", true);
                return;
            }

            if (Txttotalpayablecost.Text == "")
            {
                Txtnewplancost.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter new plan cost again', 'warning');", true);
                return;
            }

            Find_Email_Name();

            if (Txtnewplancost.Text != "0" && Txtnewplancost.Text != string.Empty)
            {
                var totalCost = Math.Round(Convert.ToDouble(Txttotalpayablecost.Text), 2);
                var paymentDto = new PaymentRequestDto
                {
                    EmpId = Request.QueryString["SubscriberFromBillingPage"].ToString(),
                    EmailId = subscriber_email,
                    NoOfBusiness = Txtnoofbusinessinnewplan.Text,
                    NoOfUsers = Txtnoofuserinnewplan.Text,
                    PlanName = "Name: " + Label_Subscriber_Name.Text + ", Email Id: " + subscriber_email,
                    PlanStartDate = Txtnewplanstartdate.Text,
                    PlanEndDate = Txtnewplanenddate.Text,
                    Amount = totalCost.ToString(),
                    //Currency = "usd"
                    Currency = "CAD"
                };
                string paymentURL = new PaymentRequest().CreatePaymentLink(paymentDto);

                comm.ExecuteQuery("insert into Email_Payment_Link (EmployeeID,Name,Email,Payment_Link,Payment_Amount,Reseller_Or_Subscriber) " +
                    "values('" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "','" + Label_Subscriber_Name.Text + "'," +
                    "'" + subscriber_email + "','" + paymentURL + "','" + totalCost.ToString() + "','" + Subscriber_Or_Reseller + "')");

                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    SendEmail(subscriber_email, Request.QueryString["SubscriberFromBillingPage"].ToString(), subscriber_name, paymentURL, totalCost.ToString());
            }

            if (Txtnewplancost.Text.Trim() == "0")
            {
                Txttotalpayablecost.Text = "0";

                comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmailId='" + HiddenField_Subscriber_Reseller_Email.Value + "' " +
                    "and EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");

                comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive,PlanActivationDate) " +
                    "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Parse(Txtnewplanstartdate.Text.ToString()) + "', '" + DateTime.Parse(Txtnewplanenddate.Text.ToString()) + "','" + Decimal.Parse(txtcurrentplancost.Text) + "'" +
                    ",'ENTERPRISE','" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "','" + subscriber_email + "','" + double.Parse(Txtnoofuserinnewplan.Text) + "'" +
                    ",'" + double.Parse(Txtnoofbusinessinnewplan.Text) + "','1','" + DateTime.Parse(txtnewplanactivationdate.Text.ToString()) + "')");
            }

            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Plan Updated Successfully', 'success');", true);

            Txtnoofbusinessinnewplan.Text = "";
            Txtnoofuserinnewplan.Text = "";
            Txtnewplanstartdate.Text = "";
            Txtnewplanenddate.Text = "";
            Txtnewplancost.Text = "";
            Txttotalpayablecost.Text = "";

            return;
        }

        if (!chk_Cancel_Plan.Checked)
        {
            if (Txtnoofbusinessinnewplan.Text == "" && Txtnewplancost.Text.Trim() == "0")
            {
                Txtnoofbusinessinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter free no of business', 'warning');", true);
                return;
            }
            if (Txtnoofbusinessinnewplan.Text == "")
            {
                Txtnoofbusinessinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of business', 'warning');", true);
                return;
            }

            if (Txtnoofuserinnewplan.Text == "" && Txtnewplancost.Text.Trim() == "0")
            {
                Txtnoofuserinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter free no of user', 'warning');", true);
                return;
            }
            if (Txtnoofuserinnewplan.Text == "")
            {
                Txtnoofuserinnewplan.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of user', 'warning');", true);
                return;
            }

            if (txtnewplanactivationdate.Text == "")
            {
                txtnewplanactivationdate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter activation date', 'warning');", true);
                return;
            }

            if (Txtnewplanstartdate.Text == "")
            {
                Txtnewplanstartdate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter start date', 'warning');", true);
                return;
            }

            if (Txtnewplanenddate.Text == "")
            {
                Txtnewplanenddate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter end date', 'warning');", true);
                return;
            }

            if (Txtnewplanstartdate.Text.Trim().Length > 0 && Txtnewplanenddate.Text.Trim().Length > 0)
            {
                DateTime From_Date = DateTime.Parse(Txtnewplanstartdate.Text);
                DateTime To_Date = DateTime.Parse(Txtnewplanenddate.Text);
                if (From_Date > To_Date)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date is greater than End date!', 'warning');", true);
                    return;
                }
            }

            if (Txtnewplancost.Text == "")
            {
                Txtnewplancost.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter new plan cost', 'warning');", true);
                return;
            }

            if (Txttotalpayablecost.Text == "")
            {
                Txtnewplancost.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter new plan cost again', 'warning');", true);
                return;
            }

            Find_Email_Name();

            if (Txtnewplancost.Text != "0" && Txtnewplancost.Text != string.Empty)
            {
                var totalCost = Math.Round(Convert.ToDouble(Txttotalpayablecost.Text), 2);
                var paymentDto = new PaymentRequestDto
                {
                    EmpId = Request.QueryString["SubscriberFromBillingPage"].ToString(),
                    EmailId = subscriber_email,
                    NoOfBusiness = Txtnoofbusinessinnewplan.Text,
                    NoOfUsers = Txtnoofuserinnewplan.Text,
                    PlanName = "Name: " + Label_Subscriber_Name.Text + ", Email Id: " + subscriber_email,
                    PlanStartDate = Txtnewplanstartdate.Text,
                    PlanEndDate = Txtnewplanenddate.Text,
                    Amount = totalCost.ToString(),
                    //Currency = "usd"
                    Currency = "CAD"
                };
                string paymentURL = new PaymentRequest().CreatePaymentLink(paymentDto);

                DataTable dt_checkifdirectactivatedbysupportadmin = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "No_Of_Users", "EmpId = " +
                    " '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and No_Of_Users IS NULL ", "");
                if (dt_checkifdirectactivatedbysupportadmin.Rows.Count == 1)
                {
                    comm.ExecuteQuery("update PlanHistory set No_Of_Users='" + Decimal.Parse(Txtnoofuserinnewplan.Text) + "',Business='" + Decimal.Parse(Txtnoofbusinessinnewplan.Text) + "', " +
                    "Is_Requested_Plan = 1 where EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");
                }

                comm.ExecuteQuery("insert into Email_Payment_Link (EmployeeID,Name,Email,Payment_Link,Payment_Amount,Reseller_Or_Subscriber) " +
                    "values('" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "','" + Label_Subscriber_Name.Text + "'," +
                    "'" + subscriber_email + "','" + paymentURL + "','" + totalCost.ToString() + "','" + Subscriber_Or_Reseller + "')");

                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    SendEmail(subscriber_email, Request.QueryString["SubscriberFromBillingPage"].ToString(), subscriber_name, paymentURL, totalCost.ToString());
            }

            if (Txtnewplancost.Text.Trim() == "0")
            {
                Txttotalpayablecost.Text = "0";

                DataTable dt_checkifdirectactivatedbysupportadmin = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "No_Of_Users", "EmpId = " +
                    " '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and No_Of_Users IS NULL ", "");
                if (dt_checkifdirectactivatedbysupportadmin.Rows.Count == 1)
                {
                    comm.ExecuteQuery("update PlanHistory set No_Of_Users='" + Decimal.Parse(Txtnoofuserinnewplan.Text) + "',Business='" + Decimal.Parse(Txtnoofbusinessinnewplan.Text) + "', " +
                    "Is_Requested_Plan = 1 where EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");
                }

                comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmailId='" + HiddenField_Subscriber_Reseller_Email.Value + "' " +
                    "and EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");

                comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive,PlanActivationDate) " +
                    "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Parse(Txtnewplanstartdate.Text.ToString()) + "', '" + DateTime.Parse(Txtnewplanenddate.Text.ToString()) + "','" + Decimal.Parse(txtcurrentplancost.Text) + "'" +
                    ",'ENTERPRISE','" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "','" + subscriber_email + "','" + double.Parse(Txtnoofuserinnewplan.Text) + "'" +
                    ",'" + double.Parse(Txtnoofbusinessinnewplan.Text) + "','1','" + DateTime.Parse(txtnewplanactivationdate.Text.ToString()) + "')");
            }

            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Plan Updated Successfully', 'success');", true);

            Txtnoofbusinessinnewplan.Text = "";
            Txtnoofuserinnewplan.Text = "";
            Txtnewplanstartdate.Text = "";
            Txtnewplanenddate.Text = "";
            Txtnewplancost.Text = "";
            Txttotalpayablecost.Text = "";

            return;
        }

        if (chk_Cancel_Plan.Checked)
        {
            #region To deactivate all users who are working under the Subscriber and the reseller
            //var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'", "");
            //bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
            //if (!check_reseller_or_subscriber_email)
            //{
            //    //comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'");

            //    DataTable DataTable_GetSubscriber_UserList = supportAdminCtrl.GetEmplist_SuppAdmin("Subscriber", int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()), 0);
            //    if (DataTable_GetSubscriber_UserList.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < DataTable_GetSubscriber_UserList.Rows.Count; i++)
            //        {
            //            comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + DataTable_GetSubscriber_UserList.Rows[i]["EmployeeID"].ToString() + "'");
            //        }
            //    }

            //    //DataTable DataTable_GetSubscriber_BusinessList = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID='" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and UserTypeId=1", "");
            //    //foreach (DataRow rowBuss in DataTable_GetSubscriber_BusinessList.Rows)
            //    //{
            //    //    if (rowBuss["BusinessID"].ToString() != string.Empty)
            //    //    {
            //    //        comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
            //    //    }
            //    //}
            //}
            //if (check_reseller_or_subscriber_email)
            //{
            //    //var check_reseller_emailid = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'", "");
            //    //var check_reseller_id = comm.GetAllDataFromTable("Register_Partner with (Nolock)", "PartnerID,EMail_primary", "EMail_primary = '" + check_reseller_emailid.Rows[0]["Email"].ToString() + "'", "");
            //    //comm.ExecuteQuery("update Register_Partner set IsActive='NO' where PartnerID='" + int.Parse(check_reseller_id.Rows[0]["PartnerID"].ToString()) + "'");
                
            //    DataTable dtres = comm.GetAllDataFromTable("AddBusiness ad with (Nolock) join AssginBussiness asb with (Nolock) on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
            //    if (dtres.Rows.Count > 0)
            //    {
            //        for (int i = 0; i < dtres.Rows.Count; i++)
            //        {
            //            //comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + dtres.Rows[i]["EmployeeID"].ToString() + "'");

            //            DataTable dt = supportAdminCtrl.GetEmplist_SuppAdmin("Reseller", int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()), int.Parse(dtres.Rows[i]["EmployeeID"].ToString()));
            //            if (dt.Rows.Count > 0)
            //            {
            //                for (int j = 0; j < dt.Rows.Count; j++)
            //                {
            //                    comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + dt.Rows[j]["EmployeeID"].ToString() + "'");
            //                }
            //            }
            //        }
            //    }

            //    ////added by kumar because the subscribers having no business was not getting deactivated
            //    //DataTable check_subscribers = comm.GetAllDataFromTable("AddEmployee ae with (Nolock)", "distinct ae.EmployeeID, dbo.FN_DESC('AddEmployee',ae.EmployeeID)empname ", "ae.MotherOrgId=" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "and ae.UserType=1 ", "");
            //    //if (check_subscribers.Rows.Count > 0)
            //    //{
            //    //    for (int i = 0; i < check_subscribers.Rows.Count; i++)
            //    //    {
            //    //        comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + check_subscribers.Rows[i]["EmployeeID"].ToString() + "'");
            //    //    }
            //    //}

            //    //DataTable dtResellerAllBuss = comm.GetAllDataFromTable("AddBusiness AsB with (Nolock)", "AsB.BusinessID, AsB.CompanyName, AsB.Status, (select count(EmployeeID) from AssginBussiness where UserTypeId <> 1 AND BusinessID = AsB.BusinessID)empcnt", "MotherOrgId=" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "", "");
            //    //foreach (DataRow rowBuss in dtResellerAllBuss.Rows)
            //    //{
            //    //    if (rowBuss["BusinessID"].ToString() != string.Empty)
            //    //    {
            //    //        comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
            //    //    }
            //    //}
            //}
            #endregion

            comm.ExecuteQuery("update PlanHistory set Is_Cancelled=1, RefundAmt='" + Label_Refund_Amount.Text + "', RefundDate = '" + DateTime.Today + "' " +
                "where EmailId='" + HiddenField_Subscriber_Reseller_Email.Value + "' " +
                "and EmpId = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' ");
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The Plan has been Cancelled Successfully', 'success');", true);

            var check_Cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' " +
                "and Is_Cancelled = 1 ");
            if (check_Cancelled.Rows.Count > 0)
            {
                Label_Cancelled_Message.Text = "This Subscriber Is Cancelled";
                txtcurrentplancost.Text = "0";
            }
        }
    }

    public void Get_ActivePlanDetails(int EmployeeID)
    {
        DataTable dt_ActivePlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + EmployeeID + " and IsActive=1", "");
        if (dt_ActivePlanDetails.Rows.Count > 0)
        {
            if (dt_ActivePlanDetails.Rows[0]["Business"].ToString() == "10000")
                txtnoofbusiness.Text = "Unlimited Business";
            else
                txtnoofbusiness.Text = dt_ActivePlanDetails.Rows[0]["Business"].ToString();

            txtnoofuser.Text = dt_ActivePlanDetails.Rows[0]["No_Of_Users"].ToString();
            txtplanstartdate.Text = DateTime.Parse(dt_ActivePlanDetails.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
            txtplanenddate.Text = DateTime.Parse(dt_ActivePlanDetails.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
            txtcurrentplancost.Text = dt_ActivePlanDetails.Rows[0]["AmountPaid"].ToString();
        }

        DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + EmployeeID + "", "");
        if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_FindIf1stTimeRegisteredUserDetails.Rows.Count > 0)
        {
            if (dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["Business"].ToString() == "10000")
                txtnoofbusiness.Text = "Unlimited Business";
            else
                txtnoofbusiness.Text = dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["Business"].ToString();

            txtnoofuser.Text = dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["No_Of_Users"].ToString();
            txtplanstartdate.Text = DateTime.Parse(dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
            txtplanenddate.Text = DateTime.Parse(dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
            txtcurrentplancost.Text = dt_FindIf1stTimeRegisteredUserDetails.Rows[0]["AmountPaid"].ToString();
        }

        DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + EmployeeID + " and Is_Requested_Plan=1", "");
        if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count > 0)
        {
            Txtnoofbusinessinnewplan.Text = dt_RequestedPlanDetails.Rows[0]["Business"].ToString();
            Txtnoofuserinnewplan.Text = dt_RequestedPlanDetails.Rows[0]["No_Of_Users"].ToString();
            Txtnewplanstartdate.Text = DateTime.Parse(dt_RequestedPlanDetails.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
            Txtnewplanenddate.Text = DateTime.Parse(dt_RequestedPlanDetails.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
        }
        if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count > 1 && dt_RequestedPlanDetails.Rows.Count > 0)
        {
            Txtnoofbusinessinnewplan.Text = dt_RequestedPlanDetails.Rows[0]["Business"].ToString();
            Txtnoofuserinnewplan.Text = dt_RequestedPlanDetails.Rows[0]["No_Of_Users"].ToString();
            Txtnewplanstartdate.Text = DateTime.Parse(dt_RequestedPlanDetails.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
            Txtnewplanenddate.Text = DateTime.Parse(dt_RequestedPlanDetails.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
        }

        DataTable dt_FindIfUserNotRegisteredProperly = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + EmployeeID + " and No_Of_Users IS NULL " +
            "and Business IS NULL", "");
        if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_FindIfUserNotRegisteredProperly.Rows.Count == 1)
        {
            Txtnoofbusinessinnewplan.Text = dt_FindIfUserNotRegisteredProperly.Rows[0]["Business"].ToString();
            Txtnoofuserinnewplan.Text = dt_FindIfUserNotRegisteredProperly.Rows[0]["No_Of_Users"].ToString();
            Txtnewplanstartdate.Text = DateTime.Parse(dt_FindIfUserNotRegisteredProperly.Rows[0]["StartDate"].ToString()).ToString("yyyy-MM-dd");
            Txtnewplanenddate.Text = DateTime.Parse(dt_FindIfUserNotRegisteredProperly.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
        }
    }

    protected void Txtnewplancost_TextChanged(object sender, EventArgs e)
    {
        if (Txtnewplancost.Text == string.Empty)
        {
            Txttotalpayablecost.Text = "";

            Txtnewplanstartdate.Text = "";
            Txtnewplanenddate.Text = "";

            Txtnoofbusinessinnewplan.Text = "";
            Txtnoofuserinnewplan.Text = "";
        }
        if (Txtnewplancost.Text.Trim() == "0")
        {
            Txtnewplanstartdate.Text = txtplanstartdate.Text;
            Txtnewplanenddate.Text = txtplanenddate.Text;
            Txttotalpayablecost.Text = "0";
        }
        if (Txtnewplancost.Text != string.Empty && Txtnewplancost.Text.Trim() != "0")
        {
            if (double.Parse(Txtnewplancost.Text) < double.Parse(txtcurrentplancost.Text))
            {
                Txtnewplancost.Text = "";
                Txttotalpayablecost.Text = "";
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter New Plan Cost more than Current Plan cost', 'warning');", true);
                return;
            }

            if (Txtnewplancost.Text.Trim() == "0")
                Txttotalpayablecost.Text = "0";

            DateTime d1 = DateTime.Parse(txtplanstartdate.Text.ToString());
            DateTime d2 = DateTime.Now;

            TimeSpan t = d2 - d1;
            double No_Of_Days = t.TotalDays;

            double Used_Amount = ((double.Parse(txtcurrentplancost.Text)) / 365) * No_Of_Days;

            double Remaining_Amount = double.Parse(txtcurrentplancost.Text) - Math.Round(Used_Amount, 2);//Used_Amount;

            double Actual_Payable_Amount = double.Parse(Txtnewplancost.Text) - Math.Round(Remaining_Amount, 2);//Remaining_Amount;

            Txttotalpayablecost.Text = Math.Round(Actual_Payable_Amount, 2).ToString(); //Actual_Payable_Amount.ToString();
        }
    }

    public void SendEmail(string email, string Subid, string subscriber_name, string paymentURL, string PayableAmount)
    {
        var dtEmail_Send = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=7", "");
        if (dtEmail_Send.Rows.Count > 0)
        {
            string body_new = Esctrl.PopulateBodyWithPaymentUrl(subscriber_name, "New Payment Request", "", dtEmail_Send.Rows[0]["description"].ToString(), dtEmail_Send.Rows[0]["TempName"].ToString(), paymentURL, PayableAmount);
            try
            {
                Esctrl.SendHtmlFormattedEmail(email, "New Payment Request", body_new);
            }
            catch
            {
                //ignore
            }
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
        }
    }

    public string Find_Email_Name()
    {
        DataTable Dt_GetEmailId = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'", "");
        if (Dt_GetEmailId.Rows.Count > 0)
        {
            DataTable Dt_Check_If_Reseller = comm.GetAllDataFromTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + Dt_GetEmailId.Rows[0]["Email"].ToString() + "'", "");
            if (Dt_Check_If_Reseller.Rows.Count > 0)
            {
                DataTable Dt_GetActualResellerEmailName = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "ContactName,Email", "MotherOrgID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'", "");
                if (Dt_GetActualResellerEmailName.Rows.Count > 0)
                {
                    subscriber_email = Dt_GetActualResellerEmailName.Rows[0]["Email"].ToString();
                    subscriber_name = Dt_GetActualResellerEmailName.Rows[0]["ContactName"].ToString();
                    HiddenField_Subscriber_Reseller_Email.Value = Dt_GetActualResellerEmailName.Rows[0]["Email"].ToString();
                    Subscriber_Or_Reseller = "Reseller";
                }
            }
            if (Dt_Check_If_Reseller.Rows.Count == 0)
            {
                DataTable Dt_GetEmailId2 = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and UserType = 1)", "");
                DataTable Dt_GetSubscriberName2 = comm.GetAllDataFromTable("AddEmployee", "(FirstName +' '+ MiddleName +' '+LastName) as SubName", "EmployeeID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and UserType = 1", "");
                subscriber_email = Dt_GetEmailId2.Rows[0]["MainEmail"].ToString();
                subscriber_name = Dt_GetSubscriberName2.Rows[0]["SubName"].ToString();
                HiddenField_Subscriber_Reseller_Email.Value = Dt_GetEmailId2.Rows[0]["MainEmail"].ToString();
                Subscriber_Or_Reseller = "Subscriber";
            }
        }
        if (Dt_GetEmailId.Rows.Count == 0)
        {
            DataTable Dt_GetEmailId3 = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "' and UserType = 1)", "");
            DataTable Dt_GetSubscriberName3 = comm.GetAllDataFromTable("AddEmployee", "(FirstName +' '+ MiddleName +' '+LastName) as SubName", "EmployeeID = '" + int.Parse(Request.QueryString["SubscriberFromBillingPage"].ToString()) + "'  and UserType = 1", "");
            subscriber_email = Dt_GetEmailId3.Rows[0]["MainEmail"].ToString();
            subscriber_name = Dt_GetSubscriberName3.Rows[0]["SubName"].ToString();
            HiddenField_Subscriber_Reseller_Email.Value = Dt_GetEmailId3.Rows[0]["MainEmail"].ToString();
            Subscriber_Or_Reseller = "Subscriber";
        }
        return subscriber_email;
        return subscriber_name;
        return Subscriber_Or_Reseller;
    }

    protected void chk_Cancel_Plan_CheckedChanged(object sender, EventArgs e)
    {
        if (!chk_Cancel_Plan.Checked)
        {
            Label_Refund_Amount.Text = "";

            div_refund.Visible = false;
        }

        if (chk_Cancel_Plan.Checked)
        {
            if (txtcurrentplancost.Text != string.Empty && txtcurrentplancost.Text != "" && txtcurrentplancost.Text != "0")
            {
                DateTime d1 = DateTime.Parse(txtplanstartdate.Text.ToString());
                DateTime d2 = DateTime.Now;

                TimeSpan t = d2 - d1;
                double No_Of_Days = t.TotalDays;

                double Used_Amount = ((double.Parse(txtcurrentplancost.Text)) / 365) * No_Of_Days;

                double Remaining_Amount = double.Parse(txtcurrentplancost.Text) - Math.Round(Used_Amount, 2);//Used_Amount;

                Label_Refund_Amount.Text = Math.Round(Remaining_Amount, 2).ToString(); //Remaining_Amount.ToString();

                div_refund.Visible = true;
            }
            if (txtcurrentplancost.Text == string.Empty)
            {
                txtcurrentplancost.Text = "";
            }
            if (txtcurrentplancost.Text.Trim() == "0")
            {
                txtcurrentplancost.Text = "0";
                Label_Refund_Amount.Text = "0";
            }
        }
    }
}