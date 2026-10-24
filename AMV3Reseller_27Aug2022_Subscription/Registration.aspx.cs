using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Data.SqlClient;
using log4net;

public partial class Login : System.Web.UI.Page
{
    DataTable dt = new DataTable();
    LoginClass Loginc = new LoginClass();
    CommanClass comm = new CommanClass();
    CommonUtils com = new CommonUtils();
    EmailSettingController Esctrl = new EmailSettingController();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
        }       
    }
  
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {            
            dt = Loginc.GetLoginDetails(txtemail.Text, txtpassword.Text, txtemail.Text);

            if (dt.Rows.Count > 0)
            {
                //To check if the user is logging first time. If he is logging first time, we will not allow to login.
                if (dt.Rows[0]["Is_First_Time_Login"].ToString() == "True")
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Reset Password From Link Sent in Mail.<br/>Then Login Here.', 'warning');", true);
                    return;
                }

                DataTable dtdetails = new DataTable();
                Session["password"] = dt.Rows[0]["Password"].ToString();
                Session["UID"] = dt.Rows[0]["UID"].ToString();
                Session["UserId"] = dt.Rows[0]["UserId"].ToString();
                Session["EmployeeId"] = dt.Rows[0]["EmployeeId"].ToString();
                Session["email"] = dt.Rows[0]["Email"].ToString();
                Session["MotherOrgid"] = dt.Rows[0]["MotherOrgid"].ToString();
                DataTable dataTable = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + Session["EmployeeId"].ToString() + "", "");
                if (dataTable.Rows.Count > 0)
                {
                    Session["MotherOrgid"] = dataTable.Rows[0]["MotherOrgid"].ToString();
                }
                else
                {
                    Session["MotherOrgid"] = dt.Rows[0]["MotherOrgid"].ToString();
                }
                if (dt.Rows[0]["UserType"].ToString() != "SupportAdmin")
                {
                    dtdetails = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + Session["EmployeeId"].ToString() + "", "");
                }

                if (dtdetails.Rows.Count > 0)
                {
                    Session["UserTypeid"] = dtdetails.Rows[0]["UserType"].ToString();
                    Session["UserType"] = comm.GetNameFromId("UserType", int.Parse(Session["UserTypeid"].ToString()));
                }
                else
                {
                    Session["UserTypeid"] = dt.Rows[0]["UserTypeid"].ToString();
                    Session["UserType"] = dt.Rows[0]["UserType"].ToString();
                }

                if (dt.Rows[0]["UserTypeid"].ToString() == "0" && dt.Rows[0]["UserType"].ToString() == "Admin")
                {
                    Session["email_aac1"] = Session["email"];
                    Response.Redirect("Accountant_Dashboard.aspx", false); //kumar cpmmented Accountant.aspx
                }
                //Added this email to approve the newly registered account for business owner only
                else if (dt.Rows[0]["Email"].ToString() == "support@anyminute.net" && dt.Rows[0]["UserTypeid"].ToString() == "1" && dt.Rows[0]["UserType"].ToString() == "SupportAdmin")
                {
                    //Response.Redirect("SuperAdmin_Approve_New_Account.aspx", false);
                    Response.Redirect("SuperAdmin_Approve_Subscriber.aspx", false);
                }
                else if (dt.Rows[0]["UserTypeid"].ToString() == "0" && dt.Rows[0]["UserType"].ToString() == "SuperAdmin")
                {
                    Response.Redirect("SuperAdmin.aspx", false);
                }
                else if (dt.Rows[0]["UserTypeid"].ToString() == "1" && dt.Rows[0]["UserType"].ToString() == "SupportAdmin")
                {
                    Response.Redirect("SuperAdmin.aspx", false);
                }
                //kumar added for new login
                else if (dtdetails.Rows[0]["UserType"].ToString() == "1")
                {
                    Response.Redirect("Business_Owner_Dashboard.aspx", false);
                }
                else if (dtdetails.Rows[0]["UserType"].ToString() == "2")
                {
                    Response.Redirect("Employee_Dashboard.aspx", false);
                }
                else if ((dtdetails.Rows[0]["UserType"].ToString() == "4") || (dtdetails.Rows[0]["UserType"].ToString() == "2"))
                {
                    Response.Redirect("Employee_Dashboard.aspx", false);
                }
                else if (dtdetails.Rows[0]["UserType"].ToString() == "5")
                {
                    Response.Redirect("Owner_Dashboard.aspx", false);
                }
                else if (dtdetails.Rows[0]["UserType"].ToString() == "12")
                {
                    Response.Redirect("Accountant_Newuser_Dashboard.aspx", false);
                }
                else
                {
                    Response.Redirect("Employee_Dashboard.aspx", false);
                }
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Invalid Email or Password', 'warning');", true);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnForgetPasssword_Click(object sender, EventArgs e)
    {
        LoginPanel.Visible = false;
        ForgetPasswordPanel.Visible = true;
    }

    protected void btnForgetPassword_SendMail_Click(object sender, EventArgs e)
    {
        if (txtLoginEmail_ForgetPasswordReset.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Enter Login Email', 'warning');", true);
            return;
        }
        if (txtLoginEmail_ForgetPasswordReset.Text != "")
        {
            DataTable _chkregisteredemail = comm.GetAllDataFromTable("EmpAddress, UserAcc", "UserAcc.Email, EmpAddress.MainEmail, EmpAddress.CCEmail, EmpAddress.EmpAddressId", "EmpAddress.MainEmail=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "EmpAddress.EmpAddressId desc");
            DataTable _chkAccdemail = comm.GetAllDataFromTable("MotherOrg, UserAcc", "UserAcc.Email, MotherOrg.Email", "MotherOrg.Email=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "MotherOrg.MotherOrgID desc");
            DataTable _chkSupportdemail = comm.GetAllDataFromTable("SupportAdmin, UserAcc", "UserAcc.Email, SupportAdmin.Email", "SupportAdmin.Email=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "SupportAdmin.SupportAdminId desc");

            if (_chkregisteredemail.Rows.Count > 0)
            {
                string Host = ConfigurationManager.AppSettings["ResetUrl"];
                string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                if (dtEmailTemp.Rows.Count > 0)
                {
                    DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkregisteredemail.Rows[0]["MainEmail"].ToString() + "' ", "");
                    string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forget Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                    Esctrl.SendForgetPasswordEmail(_chkregisteredemail.Rows[0]["MainEmail"].ToString(), "Forgot Password", _Old_Password.Rows[0]["Password"].ToString());
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                    LoginPanel.Visible = true;
                    ForgetPasswordPanel.Visible = false;
                    txtLoginEmail_ForgetPasswordReset.Text = "";
                }
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Not Found', 'warning');", true);
                }
            }
            else if (_chkAccdemail.Rows.Count > 0)
            {
                string Host = ConfigurationManager.AppSettings["ResetUrl"];
                string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkAccdemail.Rows[0]["Email"].ToString() + "' ", "");
                string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forgot Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                Esctrl.SendForgetPasswordEmail(_chkAccdemail.Rows[0]["Email"].ToString(), "Forget Password", _Old_Password.Rows[0]["Password"].ToString());
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                LoginPanel.Visible = true;
                ForgetPasswordPanel.Visible = false;
                txtLoginEmail_ForgetPasswordReset.Text = "";

            }
            else if (_chkSupportdemail.Rows.Count > 0)
            {
                string Host = ConfigurationManager.AppSettings["ResetUrl"];
                string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkSupportdemail.Rows[0]["Email"].ToString() + "' ", "");
                string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forgot Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                Esctrl.SendForgetPasswordEmail(_chkSupportdemail.Rows[0]["Email"].ToString(), "Forget Password", _Old_Password.Rows[0]["Password"].ToString());
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                LoginPanel.Visible = true;
                ForgetPasswordPanel.Visible = false;
                txtLoginEmail_ForgetPasswordReset.Text = "";
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Is Not Register With us', 'warning');", true);
            }
        }
    }

    protected void btnBack_ForgetPassword_Click(object sender, EventArgs e)
    {
        ForgetPasswordPanel.Visible = false;
        LoginPanel.Visible = true;
    }

    protected void LinkButton_Sign_Up_Subscriber_Click(object sender, EventArgs e)
    {
        Response.Redirect("Register_As_A_Bo.aspx");
    }

    protected void LinkButton_Sign_Up_Reseller_Click(object sender, EventArgs e)
    {
        Response.Redirect("Register_As_A_Partner.aspx");
    }
}