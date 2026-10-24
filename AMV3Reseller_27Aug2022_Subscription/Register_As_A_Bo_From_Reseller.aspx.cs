using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Register_As_A_Bo_From_Reseller : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    EmailSettingController Esctrl = new EmailSettingController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            var check_reseller_subscriber_registered_or_not_in_Planhistory = comm.ExecuteQueryDataTable("select PlanName,Business,No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                    "and IsActive = 1 ");
            if (check_reseller_subscriber_registered_or_not_in_Planhistory.Rows.Count == 0)
            {
                Disable_SubscriberRegistration();
                Label_Cancelled_Message.Text = "The Plan is not Active. Please contact Admin.";
                return;
            }

            var check_reseller_subscriber_registered_details = comm.ExecuteQueryDataTable("select PlanName,Business,No_Of_Users from PlanHistory with (Nolock) where MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                    "and IsActive = 1 ");            
            int total_assigned_businesses = 0;
            int total_assigned_users = 0;
            if (check_reseller_subscriber_registered_details.Rows.Count > 0)
            {
                Label_Assigned_Subscriber.Text = "Total Assigned Subscribers - " + check_reseller_subscriber_registered_details.Rows.Count.ToString();
                
                for (int h = 0; h < check_reseller_subscriber_registered_details.Rows.Count; h++)
                    total_assigned_businesses = total_assigned_businesses + int.Parse(check_reseller_subscriber_registered_details.Rows[h]["Business"].ToString());
                Label_Assigned_Business.Text = "Total Assigned Businesses - " + total_assigned_businesses.ToString();
                
                for (int h = 0; h < check_reseller_subscriber_registered_details.Rows.Count; h++)
                    total_assigned_users = total_assigned_users + int.Parse(check_reseller_subscriber_registered_details.Rows[h]["No_Of_Users"].ToString());
                Label_Assigned_User.Text = "Total Assigned Users - " + total_assigned_users.ToString();
            }

            var check_reseller_Plan_details = comm.ExecuteQueryDataTable("select PlanName,Business,No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                "and IsActive = 1 ");
            if (check_reseller_Plan_details.Rows.Count > 0)
            {
                if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
                {
                    Label_Plan_Name.Text = "Basic";//check_reseller_Plan_details.Rows[0]["PlanName"].ToString();
                    Label_Allowed_Subscriber.Text = "Total Allowed Subscribers - 1";
                    Label_Allowed_Business.Text = "Total Allowed Businesses - " + check_reseller_Plan_details.Rows[0]["Business"].ToString();
                    Label_Allowed_User.Text = "Total Allowed Users - " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString();
                }
                if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
                {
                    Label_Plan_Name.Text = "Standard";//check_reseller_Plan_details.Rows[0]["PlanName"].ToString();
                    Label_Allowed_Subscriber.Text = "Total Allowed Subscribers - " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString();
                    Label_Allowed_Business.Text = "Total Allowed Businesses - Unlimited"; //" + check_reseller_Plan_details.Rows[0]["Business"].ToString();
                    Label_Allowed_User.Text = "Total Allowed Users - " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString();
                }
                if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
                {
                    Label_Plan_Name.Text = "Enterprise";//check_reseller_Plan_details.Rows[0]["PlanName"].ToString();
                    Label_Allowed_Subscriber.Text = "Total Allowed Subscribers - " + check_reseller_Plan_details.Rows[0]["Business"].ToString(); ;
                    Label_Allowed_Business.Text = "Total Allowed Businesses - " + check_reseller_Plan_details.Rows[0]["Business"].ToString();
                    Label_Allowed_User.Text = "Total Allowed Users - " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString();
                }
            }

            if (total_assigned_businesses >= int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
            {
                Disable_SubscriberRegistration();
            }
            if (total_assigned_users >= int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
            {
                Disable_SubscriberRegistration();
            }

            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                GetPersonalDetailsForEdit(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                txtMainEmail.ReadOnly = true;
                Button_Register_As_A_BO.Visible = false;
                UpdateButton_Register_As_A_BO.Visible = true;
            }

            var check_reseller_Cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                "and Is_Cancelled = 1 ");
            if (check_reseller_Cancelled.Rows.Count > 0)
            {
                Disable_SubscriberRegistration();
                Label_Cancelled_Message.Text = "The Plan Is Cancelled.";
            }
        }
    }

    public void Disable_SubscriberRegistration()
    {
        txtFirstName.ReadOnly = true;
        txtLastName.ReadOnly = true;
        txtMainEmail.ReadOnly = true;
        txtConLoginEmail.ReadOnly = true;
        txtMainPhone.ReadOnly = true;
        txtExtPh.ReadOnly = true;
        txt_mobilenumber.ReadOnly = true;
        txtExtMob.ReadOnly = true;
        //TextBox_Total_Assigned_Businesses.ReadOnly = true;
        //TextBox_Total_Assigned_Users.ReadOnly = true;
        Button_Register_As_A_BO.Visible = false;
        UpdateButton_Register_As_A_BO.Visible = false;
    }

    public void GetList()
    {
        string empid;
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            empid = Request.QueryString["EmployeeIDEdit"].ToString();
        }
        else
        {
            empid = hfdEmpID.Value;
        }
    }

    public int SavePersonalDetails()
    {
        int ResultSavePersonalDetails = 0;
        try
        {
            if (btnsaveRole.Text != "Update")
            {
                EmpEntity.City = txt_city.Text;
                EmpEntity.Province = txt_province.Text;
                EmpEntity.MobilePhone = txt_mobilenumber.Text;

                //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                EmpEntity.UserType = 0;
                string userTypeUserACC = "1";//ddlusertype11.SelectedItem.Text;
                EmpEntity.LegalName = txtLegalName.Text;
                EmpEntity.Title = "";
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
                EmpEntity.DateOfBirth = txtdob.Text;
                //EmpEntity.MobilePhone = txtMobile.Text;txtMainPhone.Text;
                //EmpEntity.MobilePhone = txt_mobilenumber.Text;
                EmpEntity.MobilePhone = txtMainPhone.Text;
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(DateTime.Now.Date.ToString());
                EmpEntity.IsActive = "YES";                

                EmpEntity.Apartment = "";
                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;
                EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString()); // it is not 4 because here the motherorg id will be that id.
                EmpEntity.Occupation = txtOccupation.Text;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='R' where EmployeeID='" + Empid + "'");
                ResultSavePersonalDetails = Empid;                

                hfdEmpID.Value = Empid.ToString();
            }
            return ResultSavePersonalDetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void clearPersonalDetails()
    {
        txtLegalName.Text = "";
        txtFirstName.Text = "";
        txtMiddleName.Text = "";
        txtLastName.Text = "";
        txtSocialNumer.Text = "";
        txtdob.Text = "";
        ddlgender.SelectedValue = "";
        txtHiredate.Text = "";
        txtReleaseDate.Text = "";
        txtOccupation.Text = "";
    }

    public void ClearAddressContactDetails()
    {
        txtSearchPostalCode.Text = "";
        txtStreetNo.Text = "";
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtStreetDirection.Text = "";
        txtStreetType1.Text = "";
        txtStreetDirection1.Text = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtProvince.Text = "";
        //txtPostalCode.Visible = false;

        txtMainPhone.Text = "";
        txtWorkPhone.Text = "";
        txtMobile.Text = "";
        txtFax.Text = "";
        txtMainEmail.Text = "";
        txtCCEmail.Text = "";
        txtWebsite.Text = "";
        txtOther1.Text = "";


        txtPostalCode.Text = "";
        txtPostalCode.ReadOnly = false;
        txtStreetName.Text = "";
        txtStreetName.ReadOnly = false;
        txtStreetType.Text = "";
        txtStreetType1.Text = "";
        txtStreetType.ReadOnly = false;
        txtCity.Text = "";
        txtProvince.Text = "";

        btnSaveAddres.Text = "Save";
    }

    public void ClearPaymentDetails()
    {
        ddlBillingType.SelectedIndex = 0;
        txtBillRate.Text = "";
        ddlEmpEI.SelectedIndex = 0;
        ddlEmpCPP.SelectedIndex = 0;
        ddlRTVA.SelectedIndex = 0;
    }

    public void ClearAllFields()
    {
        clearPersonalDetails();
        ClearAddressContactDetails();
        ClearPaymentDetails();
    }

    public int SaveAddress()
    {
        int ResultSaveAddres = 0;
        try
        {
            EmpAddEnity.Postalcode = "";
            EmpAddEnity.NumberSuffix = ddlNumberSuffix.SelectedItem.Text;
            EmpAddEnity.StreetName = txtStreetName.Text;
            EmpAddEnity.StreetType = txtStreetType.Text;
            EmpAddEnity.StreetDirection = txtStreetDirection.Text;
            EmpAddEnity.SuitType = ddlSuitType.SelectedItem.Text;
            EmpAddEnity.SuiteNumber = txtSuiteNumber.Text;
            EmpAddEnity.SuitSuffix = ddlsuitesuffix.SelectedItem.Text;
            EmpAddEnity.City = txt_city.Text;
            EmpAddEnity.Province = txt_province.Text;
            EmpAddEnity.MainPhone = txtMainPhone.Text;
            EmpAddEnity.WorkPhone = txt_mobilenumber.Text; // txtWorkPhone.Text;
            EmpAddEnity.Mobile = txtExtPh.Text; //txtMobile.Text;
            EmpAddEnity.Fax = txtFax.Text;
            EmpAddEnity.MainEmail = txtMainEmail.Text.Trim();
            EmpAddEnity.CCEmail = txtCCEmail.Text;
            EmpAddEnity.Website = txtWebsite.Text;
            EmpAddEnity.Other1 = txtOther1.Text;
            EmpAddEnity.StreetNo = txtStreetNo.Text;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            //kumar changed
            EmpAddEnity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString()); // it is not 4 because here the motherorg id will be that id.

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));
            GetList();
            return ResultSaveAddres;
            //comm.ExecuteQuery("update EmpAddress set Mobile = '" + txtExtPh.Text + "' where EmpAddressId = " + ResultSaveAddres + "");
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }
    protected void btnsaveRole_Click(object sender, EventArgs e)
    {
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim() && txtConLoginEmail.Text.Trim() != string.Empty)
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
            return;
        }

        //bool chk = false;
        //DataTable dtCheckAcc = comm.GetAllDataFromTable("EmpAddress empadd with (Nolock) join AddEmployee ae with (Nolock) on empadd.EmpAddressId=ae.ADDRESSID", "*", "empadd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID");
        //if (dtCheckAcc.Rows.Count > 0)
        //{
        //    foreach (DataRow r in dtCheckAcc.Rows)
        //    {
        //        if (r["MotherOrgID"].ToString() != Session["MotherOrgid"].ToString())
        //        {
        //            chk = true;
        //        }
        //    }
        //}

        int FinalSaveSavePersonalDetails = 0, FinalAddressSave = 0, FinalSaveRate = 0;
        FinalSaveSavePersonalDetails = SavePersonalDetails();
        if (FinalSaveSavePersonalDetails > 0)
        {
            FinalAddressSave = SaveAddress();
            if (FinalAddressSave > 0)
            {
                if (btnsaveRole.Text == "Save")
                {
                    //FinalSaveRate = SavePay();
                    FinalSaveRate = 1;
                }
                if (FinalSaveRate > 0)
                {
                    if (txtMainEmail.Text.Trim() == "")
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Fill Address First', 'warning');", true);
                        return;
                    }
                    int UserType = 1;
                    string UserTypeName = "BUSINESS OWNER";
                    comm.ExecuteQuery("update AddEmployee set UserType=" + 1 + " where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                    Uae.Email = txtMainEmail.Text.Trim();
                    Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                    Uae.MotherOrgid = int.Parse(Session["MotherOrgid"].ToString()); // it is not 4 because here the motherorg id will be that id.
                    Uae.UserType = UserTypeName;
                    Uae.UserTypeid = UserType;
                    Uae.ProfileImageName = GenerateUniqueFileName(); // added by kumar
                    hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                    
                    //save attached file by firoz and copied by kumar
                    UploadAndSaveAttachment(Uae.ProfileImageName);

                    comm.ExecuteQuery("Insert into AssginBussiness (EmployeeID,EmployeeName,UserTypeId)values('" + int.Parse(hfdEmpID.Value) + "','" + txtFirstName.Text + "',1)");
                    
                    Uae.UserId = int.Parse(hfdUserId.Value);
                    Uae.UserTypeid = UserType;
                    Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                    Uae.BusinessID = 0;
                    Uac.Sp_InsertMapping(Uae);

                    comm.ExecuteQuery("insert into PlanHistory(StartDate,EndDate,AmountPaid,TransId,EmpId," +
                        "EmailId,No_Of_Users,Business,IsActive,MotherOrgID)" +
                        "values('" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
                        "'0','BASIC','" + int.Parse(hfdEmpID.Value) + "','" + txtMainEmail.Text.Trim() + "'," +
                        "'" + double.Parse(TextBox_Total_Assigned_Users.Text) + "','" + double.Parse(TextBox_Total_Assigned_Businesses.Text) + "','1'," +
                        "'" + int.Parse(Session["MotherOrgid"].ToString()) + "')");

                    GetList();
                    if (txtMainEmail.Text != string.Empty)
                    {
                        DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd with (Nolock) join AddEmployee ae with (Nolock) on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "ae.EmployeeID desc");
                        if (dtemp.Rows.Count > 0)
                        {
                            string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
                            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=3", "");
                            if (dtEmailTemp.Rows.Count > 0)
                            {
                                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                                if (IsSend)
                                    Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
                            }
                            else
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
                            }

                            //DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=4", "");
                            //if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
                            //{
                                //DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = 'supportadmin@gmail.com'", "");
                                //string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "New BO Registration Notification", url, dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com" + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                //Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
                                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
                            //}
                            //else
                            //{
                                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
                            //}
                        }
                    }

                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All Details Saved', 'success');", true);
                }
            }
        }
        btnsaveRole.Enabled = true;
        ClearAllFields();
        txt_mobilenumber.Text = "";
        txtConLoginEmail.Text = "";
        divconfirm.Visible = false;
        divtick.Visible = false;
        txtExtPh.Text = "";
        txtExtMob.Text = "";
        TextBox_Total_Assigned_Businesses.Text = "";
        TextBox_Total_Assigned_Users.Text = "";
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
    }

    protected void Button_Register_As_A_BO_Click(object sender, EventArgs e)
    {
        if (txtFirstName.Text == "")
        {
            txtFirstName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name', 'warning');", true);
            return;
        }

        if (txtMainEmail.Text == "")
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter email id', 'warning');", true);
            return;
        }
        if (IsValidEmail(txtMainEmail.Text) == false)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid email id', 'warning');", true);
            return;
        }
        if (txtConLoginEmail.Text == "" && txtConLoginEmail.Visible == true)
        {
            txtConLoginEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter confirm email id', 'warning');", true);
            return;
        }
        if (IsValidEmail(txtConLoginEmail.Text) == false && txtConLoginEmail.Visible == true)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid confirm email id', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text != txtConLoginEmail.Text && txtConLoginEmail.Visible == true)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter same confirm email id', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            DataTable dtchekmail = comm.GetAllDataFromTable("UserAcc ua with (Nolock) join AddEmployee ad with (Nolock) on ad.EmployeeID=ua.EmployeeId join EmpAddress ea with (Nolock) on ea.EmpAddressId=ad.ADDRESSID", " *", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1", "");
            if (dtchekmail.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkSameBO = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary='" + txtMainEmail.Text + "'");
            if (ChkSameBO == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }

            var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin] with (Nolock)", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckSupportAdmin == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg] with (Nolock)", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckMotherOrg == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckLowerUser = comm.IfExistsInTable("[UserAcc] with (Nolock)", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckLowerUser == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'This Id Cannot be Used for Subscriber', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
        }
        if (txtMainPhone.Text == "")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter phone number', 'warning');", true);
            return;
        }
        string txtMainPhone_ = txtMainPhone.Text.Replace("-", string.Empty).Trim();
        if (txtMainPhone_.Length.ToString() != "10")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter phone number properly', 'warning');", true);
            return;
        }

        if (TextBox_Total_Assigned_Businesses.Text == "")
        {
            TextBox_Total_Assigned_Businesses.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter number of total assigned businesses', 'warning');", true);
            return;
        }
        if (TextBox_Total_Assigned_Users.Text == "")
        {
            TextBox_Total_Assigned_Users.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter number of total assigned users', 'warning');", true);
            return;
        }

        btnsaveRole_Click(sender, e);
    }

    protected void UpdateButton_Register_As_A_BO_Click(object sender, EventArgs e)
    {
        if (TextBox_Total_Assigned_Businesses.Text == "")
        {
            TextBox_Total_Assigned_Businesses.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please enter number of total assigned businesses', 'info');", true);
            return;
        }
        if (TextBox_Total_Assigned_Users.Text == "")
        {
            TextBox_Total_Assigned_Users.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please enter number of total assigned users', 'info');", true);
            return;
        }
        if (TextBox_Total_Assigned_Businesses.Text.Trim() == "0")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please Enter Assigned Businesses more than 0', 'info');", true);
            TextBox_Total_Assigned_Businesses.Text = "";
            return;
        }
        if (TextBox_Total_Assigned_Users.Text.Trim() == "0")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please Enter Assigned Users more than 0', 'info');", true);
            TextBox_Total_Assigned_Users.Text = "";
            return;
        }

        if (FileUpload1.HasFile)
        {
            string ProfileImageName = GenerateUniqueFileName();
            UploadAndSaveAttachment(ProfileImageName);
            comm.ExecuteQuery("update UserAcc set Profile_Image='" + ProfileImageName.ToString() + "' " +
                "where EmployeeId=" + Request.QueryString["EmployeeIDEdit"].ToString() + " and MotherOrgid=" + Session["MotherOrgid"].ToString() + "");
        } 

        comm.ExecuteQuery("update AddEmployee set FirstName='"+ txtFirstName.Text + "',LastName='"+ txtLastName.Text + "',MobilePhone='"+ txtMainPhone.Text + "'" +
            "where EmployeeID = " + Request.QueryString["EmployeeIDEdit"].ToString() + "");

        comm.ExecuteQuery("update EmpAddress set MainPhone='" + txtMainPhone.Text + "',WorkPhone='" + txt_mobilenumber.Text + "',Mobile = '" + txtExtPh.Text + "' " +
            "where EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = " + Request.QueryString["EmployeeIDEdit"].ToString() + ")");

        comm.ExecuteQuery("update PlanHistory set Business='" + TextBox_Total_Assigned_Businesses.Text + "',No_Of_Users='" + TextBox_Total_Assigned_Users.Text + "'" +
            "where EmpId=" + Request.QueryString["EmployeeIDEdit"].ToString() + " and MotherOrgID=" + Session["MotherOrgid"].ToString() + "");

        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Plan updated for the Subscriber Successfully.', 'success');", true);
    }

    //[System.Web.Script.Services.ScriptMethod()]
    //[System.Web.Services.WebMethod]
    //public static List<string> GetCompletionList(string prefixText)
    //{
    //    List<string> result = new List<string>();
    //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
    //    {
    //        int len = prefixText.Length;
    //        if (len > 2)
    //        {
    //            string querytxt = "select distinct ea.MainEmail from AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join MappingTable mt on ad.EmployeeID=mt.EmployeeID where ea.MainEmail<>'' and ea.MainEmail LIKE '%'+@SearchText+'%'";
    //            using (SqlCommand cmd = new SqlCommand(querytxt, con))
    //            {
    //                con.Open();
    //                cmd.Parameters.AddWithValue("@SearchText", prefixText);
    //                SqlDataReader dr = cmd.ExecuteReader();
    //                while (dr.Read())
    //                {
    //                    result.Add(dr["MainEmail"].ToString());
    //                }
    //                return result;
    //            }
    //        }
    //        else
    //        {
    //            return result;
    //        }
    //    }
    //}

    protected void txtMainEmail_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
        {
            div1msg.Visible = true;
            divtick.Visible = false;
        }
        else
        {
            div2stmsg.Visible = false;
            divtick.Visible = true;
        }

        if (txtMainEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtMainEmail.Text) == true)
            {
                divValid.Visible = false;
                bool ceck = false;
                ceck = comm.IfExistsInTable("UserAcc with (Nolock)", "*", "Email='" + txtMainEmail.Text + "'");
                if (ceck == false)
                {
                    divconfirm.Visible = true;
                }
                if (txtMainEmail.Text.Trim() == "")
                {
                    divconfirm.Visible = false;
                }
            }
            else
            {
                divValid.Visible = true;
            }
        }
        else
        {
            txtConLoginEmail.Text = "";
            div2stmsg.Visible = false;
            divconfirm.Visible = false;
        }
    }

    protected void txtConLoginEmail_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
        }
        else
        {
            div2stmsg.Visible = false;
            divtick.Visible = true;
        }
    }

    private bool ValidateAttachment()
    {
        return FileUpload1.HasFile && FileUpload1.PostedFile.ContentType.ToLower().Contains("image");
    }

    private string GenerateUniqueFileName()
    {
        var uniqueFileName = string.Empty;
        if (FileUpload1.HasFile)
        {
            var fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
            var fileExt = Path.GetExtension(fileName);
            uniqueFileName = string.Format("{0}{1}",
                                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                        fileExt);
        }
        else
        {
            uniqueFileName = "";
        }

        return uniqueFileName;
    }

    private void UploadAndSaveAttachment(string fileName)
    {
        try
        {
            var fileDirectory = "~/Uploads/";
            if (!Directory.Exists(Server.MapPath(fileDirectory)))
                Directory.CreateDirectory(Server.MapPath(fileDirectory));

            FileUpload1.PostedFile.SaveAs(Server.MapPath(fileDirectory) + fileName);
        }
        catch
        {
            //Ignore
        }
    }

    protected void TextBox_Total_Assigned_Businesses_TextChanged(object sender, EventArgs e)
    {
        if (TextBox_Total_Assigned_Businesses.Text.Trim() == "0")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please Enter Assigned Businesses more than 0', 'info');", true);
            TextBox_Total_Assigned_Businesses.Text = "";
            return;
        }

        int total_assigned_businesses = 0;
        DataTable check_reseller_subscriber_Plan_details = new DataTable();
        if (Request.QueryString["EmployeeIDEdit"] == null)
        {
            check_reseller_subscriber_Plan_details = comm.ExecuteQueryDataTable("select PlanName,Business from PlanHistory with (Nolock) where MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                " and IsActive = 1 ");
        }
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            check_reseller_subscriber_Plan_details = comm.ExecuteQueryDataTable("select PlanName,Business from PlanHistory with (Nolock) where MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                "and EmpId!= '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "' and IsActive = 1 ");
        }

        if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
        {
            for (int h = 0; h < check_reseller_subscriber_Plan_details.Rows.Count; h++)
                total_assigned_businesses = total_assigned_businesses + int.Parse(check_reseller_subscriber_Plan_details.Rows[h]["Business"].ToString());
        }

        var check_reseller_Plan_details = comm.ExecuteQueryDataTable("select PlanName,Business from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                "and IsActive = 1 ");
        if (check_reseller_Plan_details.Rows.Count > 0)
        {
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
            {
                if (TextBox_Total_Assigned_Businesses.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Businesses.Text) + total_assigned_businesses) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is 1. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Businesses.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is 1. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                }
            }
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
            {
                if (TextBox_Total_Assigned_Businesses.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Businesses.Text) + total_assigned_businesses) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is Unlimited. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Businesses.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is Unlimited. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                }
            }
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
            {
                if (TextBox_Total_Assigned_Businesses.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Businesses.Text) + total_assigned_businesses) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is " + check_reseller_Plan_details.Rows[0]["Business"].ToString() + ". Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Businesses.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["Business"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Businesses is " + check_reseller_Plan_details.Rows[0]["Business"].ToString() + ". Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                        }
                    }
                }
            }
        }
    }

    protected void TextBox_Total_Assigned_Users_TextChanged(object sender, EventArgs e)
    {
        if (TextBox_Total_Assigned_Users.Text.Trim() == "0")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Please Enter Assigned Users more than 0', 'info');", true);
            TextBox_Total_Assigned_Users.Text = "";
            return;
        }

        int total_assigned_users = 0;
        DataTable check_reseller_subscriber_Plan_details = new DataTable();
        if (Request.QueryString["EmployeeIDEdit"] == null)
        {
            check_reseller_subscriber_Plan_details = comm.ExecuteQueryDataTable("select PlanName,No_Of_Users from PlanHistory with (Nolock) where MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
            " and IsActive = 1 ");
        }
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            check_reseller_subscriber_Plan_details = comm.ExecuteQueryDataTable("select PlanName,No_Of_Users from PlanHistory with (Nolock) where MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
            "and EmpId!= '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "' and IsActive = 1 ");
        }
        
        if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
        {
            for (int h = 0; h < check_reseller_subscriber_Plan_details.Rows.Count; h++)
                total_assigned_users = total_assigned_users + int.Parse(check_reseller_subscriber_Plan_details.Rows[h]["No_Of_Users"].ToString());
        }

        var check_reseller_Plan_details = comm.ExecuteQueryDataTable("select PlanName,No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                "and IsActive = 1 ");
        if (check_reseller_Plan_details.Rows.Count > 0)
        {
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "BASIC PLAN")
            {
                if (TextBox_Total_Assigned_Users.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Users.Text) + total_assigned_users) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Maximum Allowed Users is 5. Please Upgrade the Plan to Assign More Businesses', 'success');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Users.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Users is 5. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }                    
                }
            }
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "STANDARD PLAN")
            {
                if (TextBox_Total_Assigned_Users.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Users.Text) + total_assigned_users) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Users is 10. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Users.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Users is 10. Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                            return;
                        }
                    }
                }
            }
            if (check_reseller_Plan_details.Rows[0]["PlanName"].ToString() == "ENTERPRISE PLAN")
            {
                if (TextBox_Total_Assigned_Users.Text != string.Empty)
                {
                    if (check_reseller_subscriber_Plan_details.Rows.Count > 0)
                    {
                        if ((int.Parse(TextBox_Total_Assigned_Users.Text) + total_assigned_users) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Users is " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString() + ". Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                        }
                    }
                    else
                    {
                        if (int.Parse(TextBox_Total_Assigned_Users.Text) > int.Parse(check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Maximum Allowed Users is " + check_reseller_Plan_details.Rows[0]["No_Of_Users"].ToString() + ". Please Upgrade the Plan to Assign More Businesses', 'info');", true);
                            clear_business_and_user();
                        }
                    }
                }
            }
        }
    }

    protected void clear_business_and_user()
    {
        TextBox_Total_Assigned_Businesses.Text = "";
        TextBox_Total_Assigned_Users.Text = "";
    }

    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        DataTable DataTable_PersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "FirstName,LastName,ADDRESSID", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        txtFirstName.Text = DataTable_PersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        txtLastName.Text = DataTable_PersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        if (DataTable_PersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            GetAddressDetails(int.Parse(DataTable_PersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        }

        var Get_Profilepic = comm.GetAllDataFromTable("UserAcc with (Nolock)", "Profile_Image", "EmployeeId=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        if (File.Exists(Server.MapPath("~/Uploads/" + Get_Profilepic.Rows[0]["Profile_Image"].ToString())))
        {
            previewImage.ImageUrl = "~/Uploads/" + Get_Profilepic.Rows[0]["Profile_Image"].ToString();
        }
    }

    public void GetAddressDetails(int EmpAddressId)
    {
        DataTable dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail,MainPhone,WorkPhone,Mobile", "EmpAddressId=" + EmpAddressId + "", "");
        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtExtPh.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();
        txt_mobilenumber.Text = dtAddressDetailForEdit.Rows[0]["WorkPhone"].ToString();

        DataTable dtBusinessUserNosForEdit = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "Business,No_Of_Users", "EmpId=" + Request.QueryString["EmployeeIDEdit"].ToString() + "" +
            "and MotherOrgID="+ Session["MotherOrgid"].ToString() + "", "");
        if (dtBusinessUserNosForEdit.Rows.Count > 0)
        {
            TextBox_Total_Assigned_Businesses.Text = dtBusinessUserNosForEdit.Rows[0]["Business"].ToString();
            TextBox_Total_Assigned_Users.Text = dtBusinessUserNosForEdit.Rows[0]["No_Of_Users"].ToString();
        }
    }
}