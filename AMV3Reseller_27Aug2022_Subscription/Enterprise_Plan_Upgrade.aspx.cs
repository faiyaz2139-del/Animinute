using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Enterprise_Plan_Upgrade : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            if (Session["UserType"].ToString() == "Admin")
                A1.HRef = "Accountant.aspx";
            else
                A1.HRef = "Home_Page.aspx";
        }
    }

    protected void btnSaveEnterprisePlan_Click(object sender, EventArgs e)
    {
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

        string nameofregistereduser = "";
        #region upgrade plans code 
        var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email,ContactName", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
        bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
        if (!check_reseller_or_subscriber_email)
        {
            //comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "'");
            comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
            "EmailId,No_Of_Users,Business,Is_Requested_Plan)" +
            "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
            "'0','ENTERPRISE','" + int.Parse(Session["EmployeeId"].ToString()) + "','" + Session["email"].ToString() + "','" + Decimal.Parse(txtnoofuser.Text) + "'," +
            "'" + Decimal.Parse(txtnoofbusiness.Text) + "','1')");

            var GetSubscriberEmployeeID = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "FirstName+' '+MiddleName+' '+LastName as SubName,ADDRESSID", "EmployeeID='" + int.Parse(Session["EmployeeId"].ToString()) + "' ", "1 desc");
            nameofregistereduser = GetSubscriberEmployeeID.Rows[0]["SubName"].ToString();
        }
        if (check_reseller_or_subscriber_email)
        {
            //comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "'");
            comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
            "EmailId,No_Of_Users,Business,Is_Requested_Plan)" +
            "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
            "'0','ENTERPRISE','" + int.Parse(Session["MotherOrgid"].ToString()) + "','" + Session["email"].ToString() + "','" + Decimal.Parse(txtnoofuser.Text) + "'," +
            "'" + Decimal.Parse(txtnoofbusiness.Text) + "','1')");

            nameofregistereduser = check_reseller_email.Rows[0]["ContactName"].ToString();
        }
        #endregion

        DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=6", "");
        if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        {
            string body = Esctrl.PopulateBodyNoUrl("Support Admin", "New Enterprise Upgrade Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), Session["email"].ToString(), nameofregistereduser.ToString());
            try
            {
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    //Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
                    Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", "New Enterprise Upgrade Plan Request", body);
            }
            catch
            {
                //ignore
            }
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Thank you for choosing Enterprise Plan. We will get back to you shortly.', 'success');", true);

            if (Session["UserType"].ToString() == "Admin")
                Response.Redirect("Accountant.aspx");
            else
                Response.Redirect("Home_Page.aspx");
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Email Template Not Found In DataBase', 'warning');", true);
        }

        txtnoofbusiness.Text = "";
        txtnoofuser.Text = "";
        //txtplanstartdate.Text = "";
        //txtplanenddate.Text = "";
    }
}