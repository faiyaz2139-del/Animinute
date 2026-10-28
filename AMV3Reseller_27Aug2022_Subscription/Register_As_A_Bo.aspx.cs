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
using Stripe;
using Stripe.Checkout;
using Newtonsoft.Json;

public partial class Register_As_A_Bo : System.Web.UI.Page
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
        if (!Page.IsPostBack)
        {
            ViewState["Enterprise_Plan"] = "";
            var products = new PaymentRequest().FetchStripPlans(200);
            hdf_stripePlans.Value = JsonConvert.SerializeObject(products);
        }
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
                //EmpEntity.IsActive = "NO";
                EmpEntity.IsActive = "YES";

                EmpEntity.Apartment = "";
                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;
                EmpEntity.MotherOrgID = 4; //Right now the default accountant mother org id is 4.
                EmpEntity.Occupation = txtOccupation.Text;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='NEW_AC_BO' where EmployeeID='" + Empid + "'");
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
        resultdiv.Visible = false;
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
            EmpAddEnity.WorkPhone = txtWorkPhone.Text;
            EmpAddEnity.Mobile = txtMobile.Text;
            EmpAddEnity.Fax = txtFax.Text;
            EmpAddEnity.MainEmail = txtMainEmail.Text.Trim();
            EmpAddEnity.CCEmail = txtCCEmail.Text;
            EmpAddEnity.Website = txtWebsite.Text;
            EmpAddEnity.Other1 = txtOther1.Text;
            EmpAddEnity.StreetNo = txtStreetNo.Text;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            EmpAddEnity.MotherOrgId = 4; // int.Parse(Session["MotherOrgid"].ToString());

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));
            GetList();
            return ResultSaveAddres;
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

        bool chk = false;
        DataTable dtCheckAcc = comm.GetAllDataFromTable("EmpAddress empadd join AddEmployee ae on empadd.EmpAddressId=ae.ADDRESSID", "*", "empadd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID");
        if (dtCheckAcc.Rows.Count > 0)
        {
            foreach (DataRow r in dtCheckAcc.Rows)
            {
                if (r["MotherOrgID"].ToString() != Convert.ToString(Session["MotherOrgid"]))
                {
                    chk = true;
                }
            }
        }

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
                    Uae.MotherOrgid = 4; // int.Parse(Session["MotherOrgid"].ToString());
                    Uae.UserType = UserTypeName;
                    Uae.UserTypeid = UserType;
                    Uae.ProfileImageName = GenerateUniqueFileName();//added by firoz
                    hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                    //save attached file
                    UploadAndSaveAttachment(Uae.ProfileImageName);

                    comm.ExecuteQuery("Insert into AssginBussiness (EmployeeID,EmployeeName,UserTypeId)values('" + int.Parse(hfdEmpID.Value) + "','" + txtFirstName.Text + "',1)");

                    Uae.UserId = int.Parse(hfdUserId.Value);
                    Uae.UserTypeid = UserType;
                    Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                    Uae.BusinessID = 0;
                    Uac.Sp_InsertMapping(Uae);

                    if (ViewState["Enterprise_Plan"].ToString() == "")
                    {
                        // 2026-10-28 v26: Basic limits come from the Support Admin plan catalogue (falls back to the old 5 users / 1 business).
                        int basicUsers, basicBusinesses;
                        AnyminuteSupportAdmin.CatalogueLimits("BASIC PLAN", 5, 1, out basicUsers, out basicBusinesses);
                        comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
                        "EmailId,No_Of_Users,Business,IsActive)" +
                        "values('prod_basic','prod_basic_priceid','BASIC PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
                        "'0','BASIC','" + int.Parse(hfdEmpID.Value) + "','" + txtMainEmail.Text.Trim() + "','" + basicUsers + "','" + basicBusinesses + "','1')");
                    }

                    GetList();
                    if (txtMainEmail.Text != string.Empty)
                    {
                        DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID desc");
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
                            }
                            else
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
                            }

                            if (ViewState["Enterprise_Plan"].ToString() == "")
                            {
                                DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=6", "");
                                if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
                                {
                                    string body = Esctrl.PopulateBodyNoUrl("Support Admin", "New Basic Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), "New_Basic_Plan_Request.htm", txtMainEmail.Text, txtFirstName.Text+' '+txtLastName.Text);
                                    try
                                    {
                                        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                                        if (IsSend)
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

                            //DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=4", "");
                            //if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
                            //{
                            //    DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = 'supportadmin@gmail.com'", "");
                            //    string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "New BO Registration Notification", url, dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com" + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                            //    try
                            //    {
                            //        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                            //        if (IsSend)
                            //            Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
                            //    }
                            //    catch
                            //    {
                            //        //ignore
                            //    }
                            //    //if (HiddenField_CheckEnterprise_Plan.Value == "Enterprise Plan")
                            //    //    Response.Redirect("Enterprise_Plan.aspx");

                            //    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
                            //}
                            //else
                            //{
                            //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
                            //}
                        }
                    }

                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All Details Saved', 'success');", true);
                }
            }
        }

        if (ViewState["Enterprise_Plan"].ToString() != "")
        {
            comm.ExecuteQuery("insert into PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId," +
                "EmailId)" +
                "values('prod_enterprise','prod_enterprise_priceid','ENTERPRISE PLAN','" + DateTime.Today + "','" + DateTime.Today.AddDays(365) + "'," +
                "'0','ENTERPRISE','" + int.Parse(hfdEmpID.Value) + "','" + txtMainEmail.Text.Trim() + "')");
            Response.Redirect("Enterprise_Plan.aspx?SubscriberId=" + hfdEmpID.Value);
        } 
        else
        {
            btnsaveRole.Enabled = true;
            ClearAllFields();
            txt_mobilenumber.Text = "";
            txtConLoginEmail.Text = "";
            txtExtPh.Text = "";
            txtExtMob.Text = "";
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
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Registration Completed Successfully. Please check your email for more details.', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'User created succeefully.', 'success');", true);
            

        }
    }

    protected void Button_Register_As_A_BO_Click(object sender, EventArgs e)
    {
        //if (!ValidateAttachment())
        //{
        //    this.ClientScript
        //        .RegisterStartupScript(this.GetType(),
        //        "SweetAlert",
        //        "swal('Warning!', 'Attached file is not a valid image', 'warning');",
        //        true);
        //    return;
        //}
        if (txtFirstName.Text == "")
        {
            txtFirstName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name', 'warning');", true);
            return;
        }
        //if (txtLastName.Text == "")
        //{
        //    txtLastName.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name', 'warning');", true);
        //    return;
        //}

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
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkSameBO = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary='" + txtMainEmail.Text + "'");
            if (ChkSameBO == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }

            var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckSupportAdmin == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckMotherOrg == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckLowerUser = comm.IfExistsInTable("[UserAcc]", "*", "[Email]='" + txtMainEmail.Text + "'");
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
                Source = "B"
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
        //if (!ValidateAttachment())
        //{
        //    this.ClientScript
        //        .RegisterStartupScript(this.GetType(),
        //        "SweetAlert",
        //        "swal('Warning!', 'Attached file is not a valid image', 'warning');",
        //        true);
        //    return;
        //}
        if (txtFirstName.Text == "")
        {
            txtFirstName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name', 'warning');", true);
            return;
        }
        if (txtLastName.Text == "")
        {
            txtLastName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name', 'warning');", true);
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
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkSameBO = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary='" + txtMainEmail.Text + "'");
            if (ChkSameBO == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Subscriber already exists', 'warning');", true);
                return;
            }

            var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckSupportAdmin == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
            if (ceckMotherOrg == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber already exists', 'info');", true);
                txtMainEmail.Text = "";
                return;
            }
            var ceckLowerUser = comm.IfExistsInTable("[UserAcc]", "*", "[Email]='" + txtMainEmail.Text + "'");
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

        //HiddenField_CheckEnterprise_Plan.Value = "Enterprise Plan";
        ViewState["Enterprise_Plan"] = "EnterprisePlan";
        btnsaveRole_Click(sender, e);
        //Response.Redirect("Enterprise_Plan.aspx");
        //if (HiddenField_CheckEnterprise_Plan.Value == "Enterprise Plan")
        //    Response.Redirect("Enterprise_Plan.aspx");
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