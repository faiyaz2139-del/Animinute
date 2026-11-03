using log4net;
using System;
using System.Configuration;
using System.Data;

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
        // v41: Google / Microsoft buttons for everyone on the main sign-in screen (shown once the provider is configured).
        GoogleLogin.Visible = AnyminuteExternalLogin.Configured(AnyminuteExternalLogin.Google);
        MicrosoftLogin.Visible = AnyminuteExternalLogin.Configured(AnyminuteExternalLogin.Microsoft);
        SocialLogin.Visible = GoogleLogin.Visible || MicrosoftLogin.Visible;
        if (!IsPostBack)
        {
            string extError = Session[AnyminuteExternalLogin.ErrorKey] as string;
            if (extError != null)
            {
                Session.Remove(AnyminuteExternalLogin.ErrorKey);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Sign in', '" + System.Web.HttpUtility.JavaScriptStringEncode(extError) + "', 'warning');", true);
            }
            string ticket = Session[AnyminuteExternalLogin.TicketKey] as string;
            if (ticket != null)
            {
                Session.Remove(AnyminuteExternalLogin.TicketKey);
                var parts = ticket.Split('|');
                int extUser;
                if (parts.Length == 2 && Int32.TryParse(parts[0], out extUser))
                {
                    dt = AnyminuteExternalLogin.LoginRow(extUser);
                    if (dt.Rows.Count == 1) { SignIn(dt, dt.Rows[0]["Email"].ToString(), "Signed in with " + parts[1] + "."); return; }
                }
            }
            // v26: sent here by AnyminuteSessionGuard after Support disabled login access / revoked the session.
            if (Request.QueryString["revoked"] == "1")
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Signed out', 'Your session was ended by an administrator. Please sign in again or contact your administrator.', 'warning');", true);
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            dt = Loginc.GetLoginDetails(txtemail.Text, txtpassword.Text, txtemail.Text);

            if (dt.Rows.Count > 0)
            {
                // v45: a Support-created Pending subscriber signing in with the emailed temporary password is activated here (password sign-in only).
                AnyminuteSubscriberCreate.ActivateOnTempPassword(dt);
                SignIn(dt, txtemail.Text, "Signed in to AnyMinute.");
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Invalid Email or Password', 'warning');", true);
                AnyminuteSupportAdmin.LoginEvent(txtemail.Text, null, false, "Incorrect password entered.");
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    // v41: shared by password sign-in and Google / Microsoft sign-in (ExternalLogin.aspx), so both run the same
    // first-time-login and disabled-login checks, session setup and role routing for every role.
    void SignIn(DataTable dt, string loginName, string signedInNote)
    {
        //To check if the user is logging first time. If he is logging first time, we will not allow to login.
        if (dt.Rows[0]["Is_First_Time_Login"].ToString() == "True")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Reset Password From Link Sent in Mail.<br/>Then Login Here.', 'warning');", true);
            return;
        }


        // 2026-10-24 v2: an owner can switch off login access from Edit Employee > Login & Access.
        if (AnyminuteEmployeeEditor.LoginDisabled(dt.Rows[0]["UserId"].ToString()))
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Your login access is disabled. Please contact your administrator.', 'warning');", true);
            AnyminuteSupportAdmin.LoginEvent(loginName, dt.Rows[0]["UserId"].ToString(), false, "Login access is disabled.");
            return;
        }

        string strEmployeeID = dt.Rows[0]["EmployeeId"].ToString();
        DataTable dtdetails = new DataTable();
        Session["password"] = dt.Rows[0]["Password"].ToString();
        Session["UID"] = dt.Rows[0]["UID"].ToString();
        Session["UserId"] = dt.Rows[0]["UserId"].ToString();
        Session["EmployeeId"] = dt.Rows[0]["EmployeeId"].ToString();
        Session["email"] = dt.Rows[0]["Email"].ToString();
        AnyminuteEmployeeEditor.RecordLogin(dt.Rows[0]["UserId"].ToString()); // v7 last login, never throws
        AnyminuteSupportAdmin.LoginEvent(loginName, dt.Rows[0]["UserId"].ToString(), true, signedInNote); // v25 Login Activity History, never throws
        AnyminuteSessionGuard.SignedIn(dt.Rows[0]["UserId"].ToString()); // v26 active session (Disable Login Access revokes it), never throws
        Session["MotherOrgid"] = dt.Rows[0]["MotherOrgid"].ToString();
        DataTable dataTable = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "MotherOrgid", "EmployeeID=" + strEmployeeID + "", "");
        if (dataTable.Rows.Count > 0)
        {
            Session["MotherOrgid"] = dataTable.Rows[0]["MotherOrgid"].ToString();
        }
        else if (dt.Rows.Count>0)
        {
            if (dt.Rows[0]["MotherOrgid"]!=null)
            Session["MotherOrgid"] = dt.Rows[0]["MotherOrgid"].ToString();
        }
        if (dt.Rows.Count >0 && dt.Rows[0]["UserType"].ToString() != "SupportAdmin")
        {
            dtdetails = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "UserType", "EmployeeID=" + Session["EmployeeId"].ToString() + "", "");
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
            // v55 (Reseller PPT collage): a reseller login (AnyminuteReseller.LoginEmail -> UserAcc via EnsureLogin) lands on the Reseller portal.
            if (AnyminuteResellerPortal.Is()) { Response.Redirect("ResellerDashboard.aspx", false); return; }
            Response.Redirect("Dashboard.aspx", false); // v37: accountant / payroll-agency logins land on the PPT Accountant & Payroll Dashboard (was legacy Accountant_Dashboard.aspx)
        }
        //Added this email to approve the newly registered account for business owner only
        else if (dt.Rows[0]["Email"].ToString() == "support@anyminute.net" && dt.Rows[0]["UserTypeid"].ToString() == "1" && dt.Rows[0]["UserType"].ToString() == "SupportAdmin")
        {
            Response.Redirect("SupportSubscribers.aspx", false); // v47 (Nov 2 live smoke): modern Subscriber Management, same as every Support Admin (was legacy SuperAdmin_Approve_Subscriber.aspx)
        }
        else if (dt.Rows[0]["UserTypeid"].ToString() == "0" && dt.Rows[0]["UserType"].ToString() == "SuperAdmin")
        {
            Response.Redirect("SuperAdmin.aspx", false);
        }
        else if (dt.Rows[0]["UserTypeid"].ToString() == "1" && dt.Rows[0]["UserType"].ToString() == "SupportAdmin")
        {
            Response.Redirect("SupportSubscribers.aspx", false); // v42 PPT 03 s1: Accountant List removed; Subscriber Management is the landing
        }
        //kumar added for new login
        else if (dtdetails.Rows[0]["UserType"].ToString() == "1")
        {
            Response.Redirect("Dashboard.aspx", false); // v21 (Pandey #5): new role dashboard
        }
        else if (dtdetails.Rows[0]["UserType"].ToString() == "2")
        {
            Response.Redirect("Dashboard.aspx", false); // v21 (Pandey #5): new role dashboard
        }
        else if ((dtdetails.Rows[0]["UserType"].ToString() == "4") || (dtdetails.Rows[0]["UserType"].ToString() == "2"))
        {
            Response.Redirect("Dashboard.aspx", false); // v21 (Pandey #5): new role dashboard
        }
        else if (dtdetails.Rows[0]["UserType"].ToString() == "5")
        {
            Response.Redirect("Dashboard.aspx", false); // v21 (Pandey #5): new role dashboard
        }
        else if (dtdetails.Rows[0]["UserType"].ToString() == "12")
        {
            Response.Redirect("Dashboard.aspx", false); // v27: accountants use the modern dashboard (was old Accountant_Newuser_Dashboard)
        }
        else
        {
            Response.Redirect("Dashboard.aspx", false); // v21 (Pandey #5): new role dashboard
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
            DataTable _chkregisteredemail = comm.GetAllDataFromTable("EmpAddress with (Nolock), UserAcc with (Nolock)", "UserAcc.Email, EmpAddress.MainEmail, EmpAddress.CCEmail, EmpAddress.EmpAddressId", "EmpAddress.MainEmail=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "EmpAddress.EmpAddressId desc");
            DataTable _chkAccdemail = comm.GetAllDataFromTable("MotherOrg with (Nolock), UserAcc with (Nolock)", "UserAcc.Email, MotherOrg.Email", "MotherOrg.Email=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "MotherOrg.MotherOrgID desc");
            DataTable _chkSupportdemail = comm.GetAllDataFromTable("SupportAdmin with (Nolock), UserAcc with (Nolock)", "UserAcc.Email, SupportAdmin.Email", "SupportAdmin.Email=UserAcc.Email and UserAcc.Email='" + txtLoginEmail_ForgetPasswordReset.Text + "'", "SupportAdmin.SupportAdminId desc");

            // v39 (Change Login Email): a login email changed through verified self-service no longer matches the business contact
            // email (EmpAddress/MotherOrg/SupportAdmin are never overwritten), so fall back to the UserAcc login email itself (the mail goes to that same address).
            if (_chkregisteredemail.Rows.Count == 0 && _chkAccdemail.Rows.Count == 0 && _chkSupportdemail.Rows.Count == 0)
                try
                {
                    using (var c = AnyminuteTimesheetRepair.Open())
                        _chkregisteredemail = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 Email AS MainEmail FROM UserAcc WHERE Email=@m", "@m", txtLoginEmail_ForgetPasswordReset.Text.Trim());
                }
                catch (System.Data.SqlClient.SqlException) { }
            if (_chkregisteredemail.Rows.Count > 0)
            {
                //string Host = ConfigurationManager.AppSettings["ResetUrl"];
                //string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                //DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster with (Nolock)", "*", "TempType=1", "");
                //if (dtEmailTemp.Rows.Count > 0)
                //{
                //    DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkregisteredemail.Rows[0]["MainEmail"].ToString() + "' ", "");
                //    //string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forget Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                //    bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                //    if (IsSend)
                //        Esctrl.SendForgetPasswordEmail(_chkregisteredemail.Rows[0]["MainEmail"].ToString(), "Forget Password", _Old_Password.Rows[0]["Password"].ToString());
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                //    LoginPanel.Visible = true;
                //    ForgetPasswordPanel.Visible = false;
                //    txtLoginEmail_ForgetPasswordReset.Text = "";
                //}
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Not Found', 'warning');", true);
                //}

                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkregisteredemail.Rows[0]["MainEmail"].ToString() + "' ", "");
                //string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forget Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendForgetPasswordEmail(_chkregisteredemail.Rows[0]["MainEmail"].ToString(), "Forgot Password", _Old_Password.Rows[0]["Password"].ToString());
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                LoginPanel.Visible = true;
                ForgetPasswordPanel.Visible = false;
                txtLoginEmail_ForgetPasswordReset.Text = "";
            }
            else if (_chkAccdemail.Rows.Count > 0)
            {
                //string Host = ConfigurationManager.AppSettings["ResetUrl"];
                //string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                //DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkAccdemail.Rows[0]["Email"].ToString() + "' ", "");
                //string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forget Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendForgetPasswordEmail(_chkAccdemail.Rows[0]["Email"].ToString(), "Forgot Password", _Old_Password.Rows[0]["Password"].ToString());
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Forgotten Password has been sent Successfully to Your Contact Email', 'success');", true);
                LoginPanel.Visible = true;
                ForgetPasswordPanel.Visible = false;
                txtLoginEmail_ForgetPasswordReset.Text = "";

            }
            else if (_chkSupportdemail.Rows.Count > 0)
            {
                //string Host = ConfigurationManager.AppSettings["ResetUrl"];
                //string url = Host + txtLoginEmail_ForgetPasswordReset.Text;
                //DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc with (Nolock)", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 )) as Password", "Email = '" + _chkSupportdemail.Rows[0]["Email"].ToString() + "' ", "");
                //string body = Esctrl.PopulateBodyForForgetPassword(txtLoginEmail_ForgetPasswordReset.Text, "Forget Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendForgetPasswordEmail(_chkSupportdemail.Rows[0]["Email"].ToString(), "Forgot Password", _Old_Password.Rows[0]["Password"].ToString());
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

    protected void LinkButton_Sign_Up_New_Click(object sender, EventArgs e)
    {
        LinkButton_Sign_Up_Subscriber.Visible = true;
        LinkButton_Sign_Up_Reseller.Visible = true;
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