using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Enterprise_Plan : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSaveEnterprisePlan_Click(object sender, EventArgs e)
    {
        DataTable GetSubscriberEmployeeID = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "EmployeeID,FirstName+' '+MiddleName+' '+LastName as SubName,ADDRESSID", "EmployeeID='"+ int.Parse(Request.QueryString["SubscriberId"].ToString()) + "' ", "1 desc");
        DataTable GetSubscriberEmailID = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId = '"+ int.Parse(GetSubscriberEmployeeID.Rows[0]["ADDRESSID"].ToString()) +"'", "");

        if (txtnoofbusiness.Text == "")
        {
            txtnoofbusiness.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of business', 'warning');", true);
            return;
        }

        if (txtnoofuser.Text == "")
        {
            txtnoofuser.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter no of user', 'warning');", true);
            return;
        }

        //if (txtplanstartdate.Text == "")
        //{
        //    txtplanstartdate.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter start date', 'warning');", true);
        //    return;
        //}

        //if (txtplanenddate.Text == "")
        //{
        //    txtplanenddate.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter end date', 'warning');", true);
        //    return;
        //}

        //comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
        //    "EmailId,No_Of_Users,Business,Is_Requested_Plan)" +
        //    "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
        //    "'0','ENTERPRISE','" + int.Parse(GetSubscriberEmployeeID.Rows[0]["EmployeeID"].ToString()) + "','" + GetSubscriberEmailID.Rows[0]["MainEmail"].ToString() + "'," +
        //    "'" + Decimal.Parse(txtnoofuser.Text) + "','" + Decimal.Parse(txtnoofbusiness.Text) + "','1')");

        comm.ExecuteQuery("update PlanHistory set No_Of_Users='" + Decimal.Parse(txtnoofuser.Text) + "',Business='" + Decimal.Parse(txtnoofbusiness.Text) + "', " +
            "Is_Requested_Plan = 1 where EmpId = '" + int.Parse(Request.QueryString["SubscriberId"].ToString()) + "' ");

        DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster  with (Nolock)", "description,TempName,Subject", "TempType=6", "");
        if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        {
            string body = Esctrl.PopulateBodyNoUrl("Support Admin", "New Enterprise Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), GetSubscriberEmailID.Rows[0]["MainEmail"].ToString(), GetSubscriberEmployeeID.Rows[0]["SubName"].ToString());
            try
            {
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
            }
            catch
            {
                //ignore
            }
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Thank you for choosing enterprise plan. We will get back to you shortly.', 'success');", true);
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
        }

        txtnoofbusiness.Text = "";
        txtnoofuser.Text = "";
        //txtplanstartdate.Text = "";
        //txtplanenddate.Text = "";
    }
}