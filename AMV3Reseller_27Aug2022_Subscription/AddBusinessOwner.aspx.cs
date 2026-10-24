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

public partial class AddBusinessOwner : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    CreateLinkClass CL = new CreateLinkClass();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    RateDetailsController RateCtrl = new RateDetailsController();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();
    AssgineManagerEntity Ame = new AssgineManagerEntity();
    AssgineManagerController Amc = new AssgineManagerController();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    EmailSettingController Esctrl = new EmailSettingController();
    LoginClass logclass = new LoginClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
            {

            }
            txtdob.Attributes.Add("max", System.DateTime.Now.Date.ToString("yyyy-MM-dd"));

            txtLegalName.Focus();
            txteffectivedate.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
            if (Session["MotherOrgid"] != null)
            {
                this.Page.SetFocus(ddlusertype11);
                GetStreetNumber();
                if (Session["UserTypeid"].ToString() == "0" && Session["UserType"].ToString() == "Admin")
                {
                    btnback.HRef = "Accountant.aspx";
                }
                if (Request.QueryString["EmployeeIDEdit"] != null)
                {
                    GetPersonalDetailsForEdit(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                    btnsaveRole.Text = "Update";
                    Button1.Text = "Update";
                    Button1.Visible = false;
                    UpdateButton1.Visible = true;
                }
            }
            else
            {
                Response.Redirect("login.aspx", false);
            }

            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                btnback.Visible = true;
            }
        }
    }


    protected void DisableControls(Control parent, bool State)
    {

    }

    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        btnSavePersonalDetails.Text = "Update";

        hfdEmpID.Value = Request.QueryString["EmployeeIDEdit"].ToString();
        DataTable dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        ddlusertype11.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString();
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (itm.Value == dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString())
            {
                itm.Selected = true;
            }
        }

        txt_city.Text = dtPersonalDetailsForEdit.Rows[0]["City"].ToString();
        txt_province.Text = dtPersonalDetailsForEdit.Rows[0]["Province"].ToString();
        txt_mobilenumber.Text = dtPersonalDetailsForEdit.Rows[0]["MobilePhone"].ToString();

        txtLegalName.Text = dtPersonalDetailsForEdit.Rows[0]["LegalName"].ToString();
        txtFirstName.Text = dtPersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dtPersonalDetailsForEdit.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dtPersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        txtSocialNumer.Text = dtPersonalDetailsForEdit.Rows[0]["SocialInsuranceNumber"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString() != "")
        {
            //string dbo = comm.Decrypt(dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString()).ToString();
            //txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
            txtdob.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString()).ToString("yyyy-MM-dd");
        }

        ddlgender.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["Gender"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString() != "")
        {
            txtHiredate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString()).ToString("yyyy-MM-dd");
        }

        if (dtPersonalDetailsForEdit.Rows[0]["TerminationDate"].ToString() != "")
        {
            txtReleaseDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["TerminationDate"].ToString()).ToString("yyyy-MM-dd");
        }

        txtOccupation.Text = dtPersonalDetailsForEdit.Rows[0]["Occupation"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            GetAddressDetails(int.Parse(dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        }
        hfdUserId.Value = comm.GetNameFromId("UserId", int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
    }

    public void GetAddressDetails(int EmpAddressId)
    {
        DataTable dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + EmpAddressId + "", "");
        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        hfdMainEmail.Value = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        hfdCCEmail.Value = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
    }

    //Get User Type
    public void GetStreetNumber()
    {
        comm.FillDropDownList(ddlusertype11, "UserType", "User_TypeId", "UserType", "", "User_TypeId");
        DataTable dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "", "User_TypeId");
        rdoBtnUserTypeList.DataSource = dtUserTypeList;
        rdoBtnUserTypeList.DataValueField = "User_TypeId";
        rdoBtnUserTypeList.DataTextField = "UserType";
        rdoBtnUserTypeList.DataBind();
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (Session["UserType"].ToString() != "Admin")
            {
                if (Request.QueryString["EmployeeIDEdit"] == null)
                {
                    if (itm.Value == "4")
                    {
                        itm.Selected = true;
                    }
                }
            }
            else
            {
                if (itm.Value == "1")
                {
                    itm.Selected = true;
                }
            }
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {

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

                EmpEntity.UserType = 0;
                string userTypeUserACC = ddlusertype11.SelectedItem.Text;
                EmpEntity.LegalName = txtLegalName.Text;
                EmpEntity.Title = "";
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
                EmpEntity.DateOfBirth = txtdob.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txt_mobilenumber.Text;
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(DateTime.Now.Date.ToString());
                EmpEntity.IsActive = "YES";
                if (EmpEntity.IsActive == "NO")
                {
                    EmpEntity.Apartment = DateTime.Now.ToShortDateString();
                }
                else
                {
                    EmpEntity.Apartment = "";
                }

                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;
                EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString());
                EmpEntity.Occupation = txtOccupation.Text;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                ResultSavePersonalDetails = Empid;
                hfdEmpID.Value = Empid.ToString();
            }
            else
            {
                if (txtMainEmail.Text.Trim() == hfdMainEmail.Value.Trim())
                {
                    EmpEntity.City = txt_city.Text;
                    EmpEntity.Province = txt_province.Text;
                    EmpEntity.MobilePhone = txt_mobilenumber.Text;
                    EmpEntity.MobilePhone = txtMobile.Text;
                    EmpEntity.MobilePhone = txt_mobilenumber.Text;
                    //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                    EmpEntity.UserType = 0;
                    string userTypeUserACC = ddlusertype11.SelectedItem.Text;
                    EmpEntity.LegalName = txtLegalName.Text;
                    EmpEntity.Title = "";
                    EmpEntity.FirstName = txtFirstName.Text;
                    EmpEntity.MiddleName = txtMiddleName.Text;
                    EmpEntity.LastName = txtLastName.Text;
                    EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
                    EmpEntity.DateOfBirth = txtdob.Text;//comm.encrypt(txtdob.Text);
                    EmpEntity.Gender = ddlgender.SelectedItem.Text;
                    EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                    //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                    EmpEntity.TerminationDate = DateTime.Now.Date;

                    RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                    RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                    RateEntity.RetainVacation = ddlRTVA.SelectedValue;

                    EmpEntity.EmployeeID = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                    EmpEntity.Occupation = txtOccupation.Text;

                    ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);
                }
                else
                {
                    EmpEntity.City = txt_city.Text;
                    EmpEntity.Province = txt_province.Text;
                    EmpEntity.MobilePhone = txt_mobilenumber.Text;

                    //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                    EmpEntity.UserType = 0;
                    string userTypeUserACC = ddlusertype11.SelectedItem.Text;
                    EmpEntity.LegalName = txtLegalName.Text;
                    EmpEntity.Title = "";
                    EmpEntity.FirstName = txtFirstName.Text;
                    EmpEntity.MiddleName = txtMiddleName.Text;
                    EmpEntity.LastName = txtLastName.Text;
                    EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
                    EmpEntity.DateOfBirth = txtdob.Text;
                    EmpEntity.MobilePhone = txtMobile.Text;
                    EmpEntity.MobilePhone = txt_mobilenumber.Text;
                    EmpEntity.Gender = ddlgender.SelectedItem.Text;
                    EmpEntity.HireDate = DateTime.Parse(DateTime.Now.Date.ToString());
                    EmpEntity.IsActive = "YES";
                    if (EmpEntity.IsActive == "NO")
                    {
                        EmpEntity.Apartment = DateTime.Now.ToShortDateString();
                    }
                    else
                    {
                        EmpEntity.Apartment = "";
                    }

                    //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                    EmpEntity.TerminationDate = DateTime.Now.Date;
                    EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString());
                    EmpEntity.Occupation = txtOccupation.Text;
                    RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                    RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                    RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                    int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                    ResultSavePersonalDetails = Empid;
                    hfdEmpID.Value = Empid.ToString();
                }
            }
            return ResultSavePersonalDetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnSavePersonalDetails_Click(object sender, EventArgs e)
    {
        
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
            EmpAddEnity.MainEmail = txtMainEmail.Text.Trim(); ;
            EmpAddEnity.CCEmail = txtCCEmail.Text;
            EmpAddEnity.Website = txtWebsite.Text;
            EmpAddEnity.Other1 = txtOther1.Text;
            EmpAddEnity.StreetNo = txtStreetNo.Text;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            EmpAddEnity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString());

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));
            return ResultSaveAddres;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void btnSaveAddres_Click(object sender, EventArgs e)
    {

    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        
    }

    protected void rdobtnlist_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    public int SavePay()
    {
        int ResultSavePay = 0;
        //try
        //{
        //    if (txtBillRate.Text == "")
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Billing Rate', 'warning');", true);
        //        return 0;
        //    }

        //    RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
        //    RateEntity.Rate = float.Parse(txtBillRate.Text);
        //    RateEntity.RateType = ddlBillingType.SelectedItem.Text;
        //    //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
        //    RateEntity.StartDate = DateTime.Parse(txtHiredate.Text);
        //    RateEntity.EndDate = System.DateTime.Now.Date;
        //    RateEntity.JobTitle = ddlJobTitle.SelectedValue;
        //    RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
        //    RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
        //    RateEntity.RetainVacation = ddlRTVA.SelectedValue;
        //    RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
        //    ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));
        //    GetRateInformation(hfdEmpID.Value);
              return ResultSavePay;
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }

    protected void btnSavePay_Click(object sender, EventArgs e)
    {
        
    }

    protected void btnAssginelinSave_Click(object sender, EventArgs e)
    {
        
    }

    protected void lnkratechange_Click(object sender, EventArgs e)
    {
        
    }

    protected void btnSaveRate_Click(object sender, EventArgs e)
    {
        //try
        //{
        //    DataTable dt = new DataTable();
        //    dt = comm.GetAllDataFromTable("RateDetails", "*", "RateId=(select MAX(RateId) from RateDetails where EmployeeId=" + hfdchangerateid.Value + ")", "");
        //    if (dt.Rows.Count > 0)
        //    {

        //        RateEntity.EmployeeId = int.Parse(hfdchangerateid.Value);
        //        RateEntity.Rate = int.Parse(txtrate.Text);
        //        RateEntity.RateType = ratetype.SelectedItem.Text;
        //        RateEntity.StartDate = System.DateTime.Now.Date;
        //        RateEntity.EndDate = System.DateTime.Now.Date;
        //        RateEntity.JobTitle = dt.Rows[0]["JobTitle"].ToString();
        //        RateEntity.IsEmployeeEIExempty = dt.Rows[0]["IsEmployeeEIExempty"].ToString();
        //        RateEntity.IsEmployeeCPPExempty = dt.Rows[0]["IsEmployeeCPPExempty"].ToString();
        //        RateEntity.RetainVacation = dt.Rows[0]["RetainVacation"].ToString();
        //        RateEntity.JobTitle = dt.Rows[0]["RetainVacation"].ToString();
        //        RateEntity.effectivedate = DateTime.Parse(txtEffectiveDateRateHistory.Text).Date;
        //        RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdchangerateid.Value));
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Rate Details Changes', 'success');", true);
        //    }
        //    else
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'No Rate Details Found Try to Add Rate From Add Employee Page', 'error');", true);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }

    protected void btnrateDelete_Click(object sender, EventArgs e)
    {

    }

    protected void btnrateEdit_Click(object sender, EventArgs e)
    {
       
    }

    public int Validate()
    {
        int output = 0;
        if (txtdob.Text == string.Empty)
        {
            output = 1;
        }
        if (txtFirstName.Text == string.Empty)
        {
            output = 2;
        }

        return output;
    }

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    protected void btnsaveRole_Click(object sender, EventArgs e)
    {
        int output = Validate();
        if (output > 0)
        {
            if (output == 2)
            {
                txtFirstName.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter First Name', 'warning');", true);
                return;
            }
        }
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim() && txtConLoginEmail.Text.Trim() != string.Empty)
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
            return;
        }
        if (IsValidEmail(txtMainEmail.Text) == false)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Valid Login Email', 'warning');", true);
            return;
        }

        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            bool ChkBo = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness mt on ad.EmployeeID=mt.EmployeeID", "distinct ea.MainEmail", "mt.UserTypeId=1 and ea.MainEmail<>'' and ea.MainEmail='" + txtMainEmail.Text + "'");
            if (ChkBo == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business Owner is Already Exists with " + txtMainEmail.Text + " Email , Use Another Email', 'warning');", true);
                return;
            }
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

        //Commet on 24-04-2021 as disccude with  pk same email in default acc
        //if (chk == true)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'User is Already Exists with " + txtMainEmail.Text + " Email , Use Another Email', 'warning');", true);
        //    return;
        //}

        bool ceck = false;
        ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
        if (ceck == false)
        {
            if (txtConLoginEmail.Text.Trim() == string.Empty)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Confirm Email', 'warning');", true);
                return;
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
                    Uae.MotherOrgid = int.Parse(Session["MotherOrgid"].ToString());
                    Uae.UserType = UserTypeName;
                    Uae.UserTypeid = UserType;
                    hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                    comm.ExecuteQuery("Insert into AssginBussiness (EmployeeID,EmployeeName,UserTypeId)values('" + int.Parse(hfdEmpID.Value) + "','" + txtFirstName.Text + "',1)");
                    
                    Uae.UserId = int.Parse(hfdUserId.Value);
                    Uae.UserTypeid = UserType;
                    Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                    Uae.BusinessID = 0;
                    Uac.Sp_InsertMapping(Uae);

                    if (txtMainEmail.Text != string.Empty)
                    {
                        DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "ae.EmployeeID desc");
                        if (dtemp.Rows.Count > 0)
                        {
                            string Host = ConfigurationManager.AppSettings["ResetUrl"];
                            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                            if (dtEmailTemp.Rows.Count > 0)
                            {
                                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Email Send to Your Register Mail', 'success');", true);
                            }
                            else
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Not Found', 'warning');", true);
                            }
                        }
                    }
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All Details Saved', 'success');", true);
                }
                else
                {
                    if (btnsaveRole.Text != "Update")
                    {
                        comm.ExecuteQuery("Delete from AddEmployee where EmployeeID=" + hfdEmpID.Value + "");
                    }
                    else
                    {
                        if (txtMainEmail.Text.Trim() == "")
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Fill Address First', 'warning');", true);
                            return;
                        }

                        #region we_are_saving_only_business_owner so it is commented
                        //int UserType = 0;
                        //string UserTypeName = "";
                        //foreach (ListItem item in rdoBtnUserTypeList.Items)
                        //{
                        //    if (item.Selected == true)
                        //    {
                        //        UserType = int.Parse(item.Value);
                        //        UserTypeName = item.Text;
                        //    }
                        //}
                        //comm.ExecuteQuery("update AddEmployee set UserType=" + UserType + " where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                        #endregion

                        int UserType = 1;
                        string UserTypeName = "BUSINESS OWNER";
                        comm.ExecuteQuery("update AddEmployee set UserType=" + 1 + " where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");

                        Uae.Email = txtMainEmail.Text.Trim();
                        Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                        Uae.MotherOrgid = int.Parse(Session["MotherOrgid"].ToString());
                        Uae.UserType = UserTypeName;
                        Uae.UserTypeid = UserType;
                        if (Request.QueryString["EmployeeIDEdit"] == null)
                        {
                            hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                        }
                        else
                        {
                            if (txtMainEmail.Text.Trim() != string.Empty)
                            {
                                if (txtMainEmail.Text.Trim() != hfdMainEmail.Value)
                                {
                                    DataTable EmailChk = comm.GetAllDataFromTable("UserAcc", "distinct *", "Email='" + txtMainEmail.Text + "'", "");
                                    if (EmailChk.Rows.Count > 0)
                                    {
                                        int EmployeeIDEdit = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                        DataTable BussEditId = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID=" + EmployeeIDEdit + " and UserTypeId=1", "BusinessID");
                                        foreach (DataRow r in BussEditId.Rows)
                                        {
                                            if (r["BusinessID"].ToString() != string.Empty)
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where BusinessID=" + r["BusinessID"].ToString() + " and  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                            else
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                        }
                                        foreach (DataRow r1 in BussEditId.Rows)
                                        {
                                            if (r1["BusinessID"].ToString() != string.Empty)
                                            {
                                                comm.ExecuteQuery("Update MappingTable set EmployeeID=" + hfdEmpID.Value + " ,  USERID=" + EmailChk.Rows[0]["UserId"].ToString() + " where BusinessID=" + r1["BusinessID"].ToString() + " and UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                            else
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                        }
                                        comm.ExecuteQuery("update AddEmployee set IsEdit=1 where EmployeeID=" + EmployeeIDEdit + "");
                                        DataTable dt23 = comm.GetAllDataFromTable("UserAcc ua join AddEmployee ad on ad.EmployeeID = ua.EmployeeId", "ad.EmployeeID", "ua.Email='" + txtMainEmail.Text.Trim() + "' and ad.IsEdit=1", "ad.EmployeeID");
                                        if (dt23.Rows.Count > 0)
                                        {
                                            foreach (DataRow r23 in dt23.Rows)
                                            {
                                                comm.ExecuteQuery("update AddEmployee set IsEdit=0 where EmployeeID=" + r23["EmployeeID"] + "");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                                        int EmployeeIDEdit = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                        DataTable BussEditId = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID=" + EmployeeIDEdit + " and UserTypeId=1", "BusinessID");

                                        foreach (DataRow r in BussEditId.Rows)
                                        {
                                            if (r["BusinessID"].ToString() != string.Empty)
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where BusinessID=" + r["BusinessID"].ToString() + " and  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                            else
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                        }
                                        foreach (DataRow r1 in BussEditId.Rows)
                                        {
                                            if (r1["BusinessID"].ToString() != string.Empty)
                                            {
                                                comm.ExecuteQuery("Update MappingTable set EmployeeID=" + hfdEmpID.Value + " ,  USERID=" + hfdUserId.Value + " where BusinessID=" + r1["BusinessID"].ToString() + " and UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                            else
                                            {
                                                comm.ExecuteQuery("Update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where  UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                                comm.ExecuteQuery("Update MappingTable set EmployeeID=" + hfdEmpID.Value + " ,  USERID=" + hfdUserId.Value + " where UserTypeId=1 and EmployeeID=" + EmployeeIDEdit + "");
                                            }
                                        }
                                        comm.ExecuteQuery("update AddEmployee set IsEdit=1 where EmployeeID=" + EmployeeIDEdit + "");
                                    }
                                }
                            }
                            if (Request.QueryString["EmployeeIDEdit"] != null)
                            {
                                if (txtMainEmail.Text != string.Empty)
                                {
                                    DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID desc");
                                    if (dtemp.Rows[0]["MainEmail"].ToString() != hfdMainEmail.Value.Trim())
                                    {
                                        if (dtemp.Rows.Count > 0)
                                        {
                                            string Host = ConfigurationManager.AppSettings["ResetUrl"];
                                            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                                            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                                            if (dtEmailTemp.Rows.Count > 0)
                                            {
                                                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                                Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Email Send to Your Register Mail', 'success');", true);
                                            }
                                            else
                                            {
                                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Not Found', 'warning');", true);
                                            }
                                        }
                                    }
                                }

                                Uae.UserId = int.Parse(Session["UserId"].ToString());
                                Uae.UserTypeid = UserType;
                                Uae.EmployeeId = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                DataTable DTBusinessID = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "EmployeeID");
                                if (DTBusinessID.Rows.Count > 0)
                                {
                                    if (DTBusinessID.Rows[0]["BusinessID"].ToString() != string.Empty)
                                    {
                                        Uae.BusinessID = int.Parse(DTBusinessID.Rows[0]["BusinessID"].ToString());
                                        Uac.Sp_InsertMapping(Uae);
                                        comm.ExecuteQuery("Update AssginBussiness set UserTypeId=" + UserType + " where EmployeeID = " + Request.QueryString["EmployeeIDEdit"].ToString() + "");
                                    }
                                }
                            }
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All Details Saved', 'success');", true);
                        }
                    }
                }
            }
            else
            {
                if (btnsaveRole.Text != "Update")
                {
                    comm.ExecuteQuery("Delete from AddEmployee where EmployeeID=" + hfdEmpID.Value + "");
                }
            }
        }
        btnsaveRole.Enabled = true;
        if (Session["UserTypeid"].ToString() == "0" && Session["UserType"].ToString() == "Admin")
        {
            Response.Redirect("Accountant.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        else
        {
            Response.Redirect("Default.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }

    protected void btnrateHistoryChangeRate_Click(object sender, EventArgs e)
    {
        
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (txtFirstName.Text == "")
        {            
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name', 'warning');", true);
            txtFirstName.Focus();
            return;
        }
        if (txtLastName.Text == "")
        {
            txtLastName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name', 'warning');", true);
            return;
        }
        if (txtMainPhone.Text == "")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter contact phone', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text == "")
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter login email', 'warning');", true);
            return;
        }
        if (IsValidEmail(txtMainEmail.Text) == false)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid login email', 'warning');", true);
            return;
        }

        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            DataTable dtchekmail = comm.GetAllDataFromTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", " *", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1", "");
            if (dtchekmail.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business Owner already exist', 'warning');", true);
                return;
            }
        }

        btnsaveRole_Click(sender, e);
    }

    protected void btnconfirm_Click(object sender, EventArgs e)
    {
        
    }

    protected void UpdateButton1_Click(object sender, EventArgs e)
    {
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
        if (txtMainPhone.Text == "")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter contact phone', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text == "")
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter login email', 'warning');", true);
            return;
        }
        if (IsValidEmail(txtMainEmail.Text) == false)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid login email', 'warning');", true);
            return;
        }

        if (txtCCEmail.Text == "" && ddlCopyLoginEmail.SelectedValue == "Yes")
        {
            txtCCEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid contact email', 'warning');", true);
            return;
        }
        if (IsValidEmail(txtCCEmail.Text) == false && txtCCEmail.Text != "")
        {
            txtCCEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid contact email', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text.Trim() != hfdMainEmail.Value.Trim())
        {
            DataTable dtchekmail = comm.GetAllDataFromTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", " *", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1", "");
            if (dtchekmail.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business Owner already exist', 'warning');", true);
                return;
            }

            DataTable dtOldEmail = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID Join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "assb.BusinessID", "ea.MainEmail='" + hfdMainEmail.Value.Trim() + "' and assb.BusinessID is not null", "assb.BusinessID");
            DataTable dtNewEmail = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID Join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "assb.BusinessID", "ea.MainEmail='" + txtMainEmail.Text + "' and assb.BusinessID is not null", "assb.BusinessID");
            int cntchk = 0;
            foreach (DataRow oldr in dtOldEmail.Rows)
            {
                foreach (DataRow newr in dtNewEmail.Rows)
                {
                    if (oldr["BusinessID"].ToString() == newr["BusinessID"].ToString())
                    {
                        cntchk++;
                        break;
                    }
                }
            }
            if (cntchk > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'User is already registered under current business Owner as a Manager/Employee/Owner', 'warning');", true);
                return;
            }
        }
        btnsaveRole_Click(sender, e);
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCompletionList(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "select distinct ea.MainEmail from AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join MappingTable mt on ad.EmployeeID=mt.EmployeeID where ea.MainEmail<>'' and ea.MainEmail LIKE '%'+@SearchText+'%'";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["MainEmail"].ToString());
                    }
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
    }

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
}