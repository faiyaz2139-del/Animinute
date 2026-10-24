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
//for log data
using log4net;
using System.IO;
using Newtonsoft.Json;

public partial class Register_As_A_Partner : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    EmailSettingController Esctrl = new EmailSettingController();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ViewState["Enterprise_Plan"] = "";
            var products = new PaymentRequest().FetchStripPlans(200);
            hdf_stripePlans.Value = JsonConvert.SerializeObject(products);
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

                EmpEntity.EMail_primary = txtMainEmail.Text;
                EmpEntity.EMail_secondary = txtConLoginEmail.Text;
                EmpEntity.UserType = 0;
                string userTypeUserACC = "0";//ddlusertype11.SelectedItem.Text;
                EmpEntity.LegalName = txtLegalName.Text;
                EmpEntity.Title = TextBox_Company_Name.Text;
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
                EmpEntity.DateOfBirth = txtdob.Text;
                EmpEntity.MobilePhone = txtMainPhone.Text;
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(DateTime.Now.Date.ToString());
                EmpEntity.IsActive = "YES";

                EmpEntity.Apartment = "";
                EmpEntity.TerminationDate = DateTime.Now.Date;
                //kumar written
                //Right now the default accountant mother org id is 4. but this MotherOrgID column is not being used in the reseller logic.
                //when reseler will be approved, this reseeler will have different motherorgID
                EmpEntity.MotherOrgID = 4;
                EmpEntity.Occupation = txtOccupation.Text;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                //save attached file by firoz and copied by kumar
                EmpEntity.StreetNumber = GenerateUniqueFileName();
                UploadAndSaveAttachment(EmpEntity.StreetNumber);
                InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
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

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    string Send_Email_Id;
    protected void btnsaveRole_Click(object sender, EventArgs e)
    {
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim() && txtConLoginEmail.Text.Trim() != string.Empty)
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
            return;
        }

        DataTable dtCheckAcc = new DataTable();
        bool chk = false;
        dtCheckAcc = comm.GetAllDataFromTable("EmpAddress empadd join AddEmployee ae on empadd.EmpAddressId=ae.ADDRESSID", "*", "empadd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID");
        if (dtCheckAcc.Rows.Count > 0)
        {
            foreach (DataRow r in dtCheckAcc.Rows)
            {
                if (r["MotherOrgID"].ToString() != Session["MotherOrgid"].ToString())
                {
                    chk = true;
                }
            }
        }

        int FinalSaveSavePersonalDetails = 0;
        FinalSaveSavePersonalDetails = SavePersonalDetails();

        DataTable Get_Latest_Reseller_Details = comm.GetAllDataFromTable("Register_Partner", "*", "EMail_primary='" + txtMainEmail.Text + "'", "");

        int MotherOrgId = 0;
        var check_mo = comm.IfExistsInTable("MotherOrg", "*", "Email='" + txtMainEmail.Text + "'");
        if (!check_mo)
        {
            MotherEc.MotherOrgName = Get_Latest_Reseller_Details.Rows[0]["FirstName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["MiddleName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["LastName"].ToString() + '-' + Get_Latest_Reseller_Details.Rows[0]["PartnerID"].ToString();
            MotherEc.StreetNumber = "";
            MotherEc.StreetName = "";
            MotherEc.Apartment = "";
            MotherEc.City = "";
            MotherEc.Province = "";
            MotherEc.PostalCode = "";
            MotherEc.EnrollDate = DateTime.Parse(Get_Latest_Reseller_Details.Rows[0]["HireDate"].ToString());
            MotherEc.Status = true;
            MotherEc.Email = txtMainEmail.Text;
            Send_Email_Id = txtMainEmail.Text;
            MotherEc.ContactName = Get_Latest_Reseller_Details.Rows[0]["FirstName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["MiddleName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["LastName"].ToString();
            MotherEc.ContactNumber = Get_Latest_Reseller_Details.Rows[0]["MobilePhone"].ToString();
            MotherEc.StreetNumber = Get_Latest_Reseller_Details.Rows[0]["StreetNumber"].ToString();
            MotherEc.Checked = "N";
            MotherOrgId = MotherC.InsertMotherOrg(MotherEc);
        }

        DataTable Get_MotherOrdeId = comm.GetAllDataFromTable("MotherOrg", "MotherOrgID", "Email='" + txtMainEmail.Text + "'", "");
        if (ViewState["Enterprise_Plan"].ToString() == "")
            comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
            "EmailId,No_Of_Users,Business,IsActive)" +
            "values('prod_basic','prod_basic_priceid','BASIC PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
            "'0','BASIC','" + int.Parse(Get_MotherOrdeId.Rows[0]["MotherOrgID"].ToString()) + "','" + txtMainEmail.Text.Trim() + "','5','1','1')");

        if (txtMainEmail.Text != string.Empty)
        {
            DataTable dtemp = comm.GetAllDataFromTable("Register_Partner", "*", "EMail_primary='" + txtMainEmail.Text + "'", "PartnerID");
            if (dtemp.Rows.Count > 0)
            {
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    SendEmail(txtMainEmail.Text, MotherOrgId.ToString());

                if (ViewState["Enterprise_Plan"].ToString() == "")
                {
                    DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=6", "");
                    if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
                    {
                        string body = Esctrl.PopulateBodyNoUrl("Support Admin", "New Basic Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), "New_Basic_Plan_Request.htm", txtMainEmail.Text, txtFirstName.Text + ' ' + txtLastName.Text);
                        try
                        {
                            bool IsSendSPAdminMail = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                            if (IsSendSPAdminMail)
                                Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", "New Basic Plan Request", body);
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
            }
        }
        string reselleremailid = txtMainEmail.Text;

        if (ViewState["Enterprise_Plan"].ToString() != "")
        {
            comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
                "EmailId)" +
                "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
                "'0','ENTERPRISE','" + int.Parse(Get_MotherOrdeId.Rows[0]["MotherOrgID"].ToString()) + "','" + txtMainEmail.Text.Trim() + "')");
            Session["ResellerEmail"] = reselleremailid;       
            Response.Redirect("Enterprise_Plan_Reseller.aspx?ResellerId=" + Get_MotherOrdeId.Rows[0]["MotherOrgID"].ToString());
        } 
        else
        {
            ClearAllFields();
            txt_mobilenumber.Text = "";
            txtConLoginEmail.Text = "";
            TextBox_Ext_Contact_Phone.Text = "";
            TextBox_Ext_Mobile.Text = "";
            TextBox_Company_Name.Text = "";
            divconfirm.Visible = false;
            divtick.Visible = false;

            TextBox_SearchPostalCode.Text = "";
            gvAddressList.DataSource = null;
            gvAddressList.DataBind();
            TextBox_Street_Number.Text = "";
            TextBox_AddressLine1.Text = "";
            TextBox_AddressLine2.Text = "";
            TextBox_SuiteNumber.Text = "";
            DropDownList_SuiteType.SelectedIndex = 0;
            TextBox_City.Text = "";
            TextBox_Province.Text = "";
            TextBox_PostalCode.Text = "";
            Div_Search_Address_List.Visible = false;
            divAddres.Visible = false;
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
        }
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
            DataTable dtchekmail = comm.GetAllDataFromTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", " *", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1", "");
            if (dtchekmail.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkBo = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness mt on ad.EmployeeID=mt.EmployeeID", "distinct ea.MainEmail", "mt.UserTypeId=1 and ea.MainEmail<>'' and ea.MainEmail='" + txtMainEmail.Text + "'");
            if (ChkBo == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists with " + txtMainEmail.Text + " email Id, use another email id', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkPartner = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary='" + txtMainEmail.Text + "'");
            if (ChkPartner == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists', 'warning');", true);
                return;
            }

            var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckSupportAdmin == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Reseller already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckMotherOrg == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Reseller already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckLowerUser = comm.IfExistsInTable("[UserAcc]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckLowerUser == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'This Id Cannot be Used for Reseller', 'info');", true);
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
        
        LinkButton lnkBtn = sender as LinkButton;
        if (string.IsNullOrEmpty(hdf_selectedPlanPriceId.Value))
        {
            btnsaveRole_Click(sender, e);
        }
        else
        {
            var fileName = GenerateUniqueFileName();
            var paymentRequestDto = new PaymentRequestDto
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                EmailId = txtMainEmail.Text,
                PhoneNo = txtMainPhone.Text,
                MobileNo = txt_mobilenumber.Text,
                PriceId = hdf_selectedPlanPriceId.Value,
                ProductId = hdf_selectedPlanId.Value,
                FileName = fileName,
                Source = "P"
            };

            var paymentRequestUrl = new PaymentRequest().GetPaymentUrl(paymentRequestDto);

            UploadAndSaveAttachment(fileName);
            Response.Redirect(paymentRequestUrl);
        }
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
                ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
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

    AddEmployeeEntityClass EmpEc = new AddEmployeeEntityClass();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    private SqlConnection con;
    public void getconnection()
    {
        string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        con = new SqlConnection(strcon);
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
    }

    public void InsertPersonalDetails(AddEmployeeEntityClass EmpEc, string userTypeUserACC, RateDetailsEntity RateEntity)
    {
        int Empid = 0;
        getconnection();
        try
        {
            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "Sp_InsertEmpPersonalDetails_Partner";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@EMail_primary", EmpEc.EMail_primary);
            cmdPersonalDetails.Parameters.AddWithValue("@EMail_secondary", EmpEc.EMail_secondary);
            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmdPersonalDetails.Parameters.AddWithValue("@userTypeUserACC", userTypeUserACC);
            cmdPersonalDetails.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmdPersonalDetails.Parameters.AddWithValue("@MotherOrgID", EmpEc.MotherOrgID);
            cmdPersonalDetails.Parameters.AddWithValue("@IsActive", EmpEc.IsActive);
            cmdPersonalDetails.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmdPersonalDetails.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);//Inactivedate
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);
            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);
            cmdPersonalDetails.Parameters.AddWithValue("@StreetNumber", EmpEc.StreetNumber);
            cmdPersonalDetails.Parameters.Add("@EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdPersonalDetails.ExecuteNonQuery();
            Empid = int.Parse(cmdPersonalDetails.Parameters["@EmployeeID"].Value.ToString());

            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") end";
            Logger.Info(loginfo);

            //return Empid;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertEmpPersonalDetails_Partner" + ex.Message);
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

    protected void Add_EnterprisePlan_Click(object sender, EventArgs e)
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
            DataTable dtchekmail = comm.GetAllDataFromTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", " *", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1", "");
            if (dtchekmail.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkBo = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness mt on ad.EmployeeID=mt.EmployeeID", "distinct ea.MainEmail", "mt.UserTypeId=1 and ea.MainEmail<>'' and ea.MainEmail='" + txtMainEmail.Text + "'");
            if (ChkBo == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists with " + txtMainEmail.Text + " email Id, use another email id', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkPartner = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary='" + txtMainEmail.Text + "'");
            if (ChkPartner == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Reseller already exists', 'warning');", true);
                return;
            }

            var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckSupportAdmin == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Reseller already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckMotherOrg == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Reseller already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckLowerUser = comm.IfExistsInTable("[UserAcc]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckLowerUser == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'This Id Cannot be Used for Reseller', 'info');", true);
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
        ViewState["Enterprise_Plan"] = "EnterprisePlan";
        btnsaveRole_Click(sender, e);
    }

    public void SendEmail(string email, string Motherid)
    {
        DataTable Dt_motherOrgId = comm.GetAllDataFromTable("MotherOrg", "MotherOrgID", "Email='" + email + "'", "");
        string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        //string url = Host + Motherid.ToString() + "&LoginType=Acc"; //commented because motherorgid is coming 0 when activating 3rd or 4th time.
        string url = Host + Dt_motherOrgId.Rows[0]["MotherOrgID"].ToString() + "&LoginType=Acc";
        DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
        //DataTable dtemp = Comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Motherid + "", ""); //commented because giving error as motherorgid is coming 0 when activating 3rd or 4th time.
        DataTable dtemp = comm.GetAllDataFromTable("MotherOrg", "*", "Email='" + email + "'", "");
        DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + email + "' ", "");
        string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["ContactName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), Send_Email_Id.ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
        Esctrl.SendHtmlFormattedEmail(email, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
    }

    protected void LinkButton_Search_Address_Click(object sender, EventArgs e)
    {
        if (TextBox_SearchPostalCode.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
            TextBox_SearchPostalCode.Focus();
            return;
        }
        string _serachpostcode = TextBox_SearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        int search_postalcode_length = int.Parse(_serachpostcode.Length.ToString());
        if (search_postalcode_length < 5)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
            TextBox_SearchPostalCode.Focus();
            return;
        }
        if (TextBox_SearchPostalCode.Text.Length.ToString() != "")
        {
            string strsearch = TextBox_SearchPostalCode.Text;
            strsearch = strsearch.Replace(" ", String.Empty);

            Div_Search_Address_List.Visible = true;
            DataTable dtSearchAddress = comm.GetSearchAddress(strsearch);
            DataRow row1 = dtSearchAddress.NewRow();
            row1["id"] = 0;
            row1["SEARCHADDRESS"] = " ADD NEW ADDRESS";
            dtSearchAddress.Rows.Add(row1);
            gvAddressList.DataSource = dtSearchAddress;
            gvAddressList.DataBind();
        }
    }

    protected void LinkButton_Copy_Searched_Address_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton LinkButton_Copy_Searched_Address = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("LinkButton_Copy_Searched_Address");
        divAddres.Attributes.Add("style", "display:block;");
        if (LinkButton_Copy_Searched_Address.Text != " ADD NEW ADDRESS")
        {
            ClearAddress();
            String[] Split = LinkButton_Copy_Searched_Address.Text.Split(',');

            TextBox_AddressLine1.Text = Split[0].ToString().ToUpper();
            TextBox_AddressLine1.ReadOnly = true;
            TextBox_AddressLine1.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            TextBox_City.Text = Split[1].ToString().ToUpper();
            TextBox_City.ReadOnly = true;
            TextBox_City.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            TextBox_Province.Text = Split[2].ToString().ToUpper();
            TextBox_Province.ReadOnly = true;
            TextBox_Province.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            TextBox_PostalCode.Text = TextBox_SearchPostalCode.Text;
            TextBox_PostalCode.ReadOnly = true;
            TextBox_PostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
        }
        else
        {
            TextBox_Street_Number.Text = "";

            TextBox_AddressLine1.Text = "";
            TextBox_AddressLine1.ReadOnly = false;
            TextBox_AddressLine1.Enabled = true;

            TextBox_AddressLine2.Text = "";

            TextBox_SuiteNumber.Text = "";

            DropDownList_SuiteType.SelectedIndex = 0;

            TextBox_Province.Text = "";
            TextBox_City.ReadOnly = false;
            TextBox_City.Enabled = true;

            TextBox_City.Text = "";
            TextBox_Province.ReadOnly = false;
            TextBox_Province.Enabled = true;

            TextBox_PostalCode.Text = "";
            TextBox_PostalCode.ReadOnly = false;
            TextBox_PostalCode.Enabled = true;
            TextBox_PostalCode.Text = TextBox_SearchPostalCode.Text;
            TextBox_PostalCode.ReadOnly = true;
            TextBox_PostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
        }
    }

    public void ClearAddress()
    {
        TextBox_Street_Number.Text = "";
        TextBox_AddressLine1.Text = "";
        TextBox_AddressLine2.Text = "";
        TextBox_SuiteNumber.Text = "";
        DropDownList_SuiteType.SelectedIndex = 0;
        TextBox_Province.Text = "";
        TextBox_City.Text = "";
        TextBox_PostalCode.Text = "";

        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";
        txtStreetNo.Text = "";
    }
}