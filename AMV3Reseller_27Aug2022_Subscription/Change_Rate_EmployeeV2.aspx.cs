using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class Change_Rate_EmployeeV2 : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    CreateLinkClass CL = new CreateLinkClass();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddProjectController apc = new AddProjectController();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    RateDetailsController RateCtrl = new RateDetailsController();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();
    AssgineManagerEntity Ame = new AssgineManagerEntity();
    AssignProjectEntity ape = new AssignProjectEntity();
    AssgineManagerController Amc = new AssgineManagerController();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    EmailSettingController Esctrl = new EmailSettingController();
    LoginClass logclass = new LoginClass();
    public static string name { get; set; }
    protected void Page_Load(object sender, EventArgs e)
    {
        // 2026-10-24 v10: the Rate History Module (RateHistory.aspx) replaces this legacy screen; ?legacy=1 keeps it reachable.
        if (Request.QueryString["legacy"] != "1") { Response.Redirect("RateHistory.aspx" + Request.Url.Query, true); return; }
        if (!IsPostBack)
        {
            divsearch.Attributes.Add("style", "display:None;");
           
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
            {
                name = Request.QueryString["BusinessIdFromDefaultPage"].ToString();
                DataTable MotherOrgiD = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"] + "", "");

                string MotherOrgID = MotherOrgiD.Rows[0]["MotherOrgId"].ToString();
                hfdMotherId.Value = MotherOrgID;

                //changes by ragini 02.11.2012
                if (Request.QueryString["EMPLOYEETYPE"] != null)
                {
                    String EMPLOYEETYPE = Request.QueryString["EMPLOYEETYPE"].ToString();
                    ddlusertyperegular.SelectedValue = EMPLOYEETYPE;
                    ddlusertyperegular.Enabled = false;
                    if(EMPLOYEETYPE=="P")
                    {
                        ddlusertyperegular_SelectedIndexChanged(sender, e);
                    }
                }
                //changes end by ragini 02.11.2012

                //DataTable chkboisActive = comm.GetAllDataFromTable("AddBusiness", "*", "BusinessID=" + name + " and APPROVED='A'", "");
                //if (chkboisActive.Rows.Count == 0)
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business is Locked.Please Contact your accountant', 'warning');", true);
                //    return;
                //}
            }
            if (Request.QueryString["BussEditId"] != null)
            {
                name = Request.QueryString["BussEditId"].ToString();
                DataTable MotherOrgiD = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + Request.QueryString["BussEditId"] + "", "");

                string MotherOrgID = MotherOrgiD.Rows[0]["MotherOrgId"].ToString();
                hfdMotherId.Value = MotherOrgID;
            }
            if (Request.QueryString["EmployeeIDEdit"] != null && Request.QueryString["BussEditId"] == null)
            {
               // name = Request.QueryString["BussEditId"].ToString();
                DataTable MotherOrgiDd = comm.GetAllDataFromTable("AddEmployee", "MotherOrgID", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"] + "", "");

                string MotherOrgID = MotherOrgiDd.Rows[0]["MotherOrgID"].ToString();
                hfdMotherId.Value = MotherOrgID;
            }
            txtLegalName.Focus();
            txtHiredate.Attributes.Add("max", System.DateTime.Now.Date.ToString("yyyy-MM-dd"));
            txtdob.Attributes.Add("max", System.DateTime.Now.Date.ToString("yyyy-MM-dd"));
            if (Session["MotherOrgid"] != null)
            {
                this.Page.SetFocus(ddlusertype11);
                GetStreetNumber();
                GetUserPermissionRoleList();
                Fill_TreeviewLink(tvLinks, string.Empty);
                if (Request.QueryString["EmployeeIDEdit"] != null)
                {
                    GetChangeRateRole(int.Parse(Session["EmployeeId"].ToString()));
                    GetPersonalDetailsForEdit(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                    GetPersonalDetails_While_Editing_Employee(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                    GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
                    btnUpdatePersonalDetails.Visible = true;
                    btnUpdateAddress.Visible = true;
                    btnsaveRole.Text = "Update";
                    Button1.Text = "Update";
                    lnkView.Visible = true;
                    divAddres.Attributes.Add("style", "display:block;");

                    btnupdateRole.Visible = true;
                    btnsaveRole.Visible = false;
                    txtCity.Enabled = false;
                    txtProvince.Enabled = false;
                    txtPostalCode.Enabled = false;
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }

            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                btnback.Visible = true;
            }
            if(Request.QueryString["EmpType"] !=null)
            {
                ddlusertyperegular.SelectedValue = Request.QueryString["EmpType"].ToString();
                ddlusertyperegular.Enabled = false;
                ddlusertyperegular_SelectedIndexChanged(sender, e);
            }
        }
    }
	
    public void GetChangeRateRole(int EmployeeID)
    {
        //This will check the login user has been assigned in which business in what roles with the employee id of same login user
        DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and UserTypeId='5' and BusinessId = '" + Request.QueryString["BussEditId"].ToString() + "'", "UserTypeId");

        if(Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            DataTable Dt_Check_Manager_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");
            if (Dt_Check_Manager_Role_Permission.Rows.Count > 0)
            {
                DataTable dtPersonalDetailsForEdit = new DataTable();
                dtPersonalDetailsForEdit = comm.GetAllDataFromTable("Role_Permission_Master", "dbo.udf_CheckRolesByEmployeeid('" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "',15) as 'IsExistsCheckUpdatePay',dbo.udf_CheckRolesByEmployeeid('" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "',16) as 'IsExistsCheckAssignPermissionRole'", "", "");
                if (dtPersonalDetailsForEdit != null)
                {
                    if (dtPersonalDetailsForEdit.Rows.Count > 0)
                    {
                        string IsExistsCheckUpdatePay = Convert.ToString(dtPersonalDetailsForEdit.Rows[0]["IsExistsCheckUpdatePay"]);
                        if (IsExistsCheckUpdatePay == "N")
                        {
                            btnrateHistoryChangeRate.Enabled = false;
                            ddlBillingType.Enabled = false;
                            txtBillRate.Enabled = false;
                        }
                        else
                        {
                            btnrateHistoryChangeRate.Enabled = true;
                            ddlBillingType.Enabled = true;
                            txtBillRate.Enabled = true;
                        }
                        string IsExistsCheckAssignPermissionRole = Convert.ToString(dtPersonalDetailsForEdit.Rows[0]["IsExistsCheckAssignPermissionRole"]);
                        if (IsExistsCheckAssignPermissionRole == "N")
                        {
                            GridView_User_Role_Permission.Enabled = false;
                        }
                        else
                        {
                            GridView_User_Role_Permission.Enabled = true;
                        }
                    }
                }
            }
        }
    }
	
    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            btnSavePersonalDetails.Text = "Update";
            hfdEmpID.Value = Request.QueryString["EmployeeIDEdit"].ToString();
        }
        else
        {
            hfdEmpID.Value = EmployeeID.ToString();
        }

        DataTable dtPersonalDetailsForEdit = new DataTable();
        dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*,convert(nvarchar(10),HireDate,120)HireDated", "EmployeeID=" + EmployeeID + "", "");
        ddlusertype11.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString();
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (itm.Value == dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString())
            {
                itm.Selected = true;
            }
        }
        if (dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString() == "5")
        {
            txtMainEmail.Enabled = false;
        }

        if (dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString() != "")
            txtSINExpiry.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString()).ToString("yyyy-MM-dd");

        txtLegalName.Text = dtPersonalDetailsForEdit.Rows[0]["LegalName"].ToString();
        txtFirstName.Text = dtPersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dtPersonalDetailsForEdit.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dtPersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        hfdSocialEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["SocialInsuranceNumber"].ToString();
        ddlEmpEI.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        ddlEmpCPP.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        ddlRTVA.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["RetainVacation"].ToString().Trim();
        ddlusertyperegular.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["EMPLOYEETYPE"].ToString().Trim();
        ddlproject.Visible = false;
        ddlusertyperegular.Enabled = false; ddlproject.Visible = false;
        if (dtPersonalDetailsForEdit.Rows[0]["EMPLOYEETYPE"].ToString().Trim() == "P")
        {
            ddlproject.Visible = true;
            ddlusertyperegular.Enabled = false; ddlproject.Visible = true;

            //comm.FillDropDownList(ddlemp, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserType not in (2,1) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "", "ab.EmployeeID");
            DataTable dt = new DataTable();
            ddlproject.Items.Clear();
            if (Request.QueryString["BussEditId"] != null)
            {
                //dt = comm.GetAllDataFromTable("AddProject ab  left join ASSIGNPROJECT asb on ab.ProjectID=asb.PROJECTID  ", "ab.ProjectID,ab.ProjectName", "Status ='A' and  AB.BUSINESSID =ASB.BUSINESSID", "ab.ProjectID");
                dt = comm.GetAllDataFromTable("AddProject ab  left join ASSIGNPROJECT asb on ab.ProjectID=asb.PROJECTID and  AB.BUSINESSID =ASB.BUSINESSID and EmployeeID=" + hfdEmpID.Value + "", "distinct ab.ProjectID,ab.ProjectName", "Status ='A'   and ab.BUSINESSID=" + Request.QueryString["BussEditId"].ToString() + "", "ab.ProjectID");
                ddlproject.DataSource = dt;
                ddlproject.DataTextField = "ProjectName";
                ddlproject.DataValueField = "ProjectID";
                ddlproject.DataBind();
                foreach (ListItem item in ddlproject.Items)
                {
                    bool check = comm.IfExistsInTable("ASSIGNPROJECT", "*", "BusinessID=" + Request.QueryString["BussEditId"].ToString() + " and PROJECTID=" + item.Value + " and EmployeeID=" + hfdEmpID.Value + "");
                    if (check == true)
                    {
                        item.Selected = true;
                    }
                }
                String result = dt.AsEnumerable()
                     .Select(row => row["ProjectId"].ToString())
                     .Aggregate((s1, s2) => String.Concat(s1, "," + s2));
                hdnproject.Value = result;
            }
        }
        if (dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString() != "")
        {
            //DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString()).ToString("yyyy-MM-dd");
            hfddboEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString();
        }

        ddlgender.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["Gender"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString() != "")
        {
            DateTime HireDate = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["HireDated"].ToString());
            txtHiredate.Text = HireDate.ToString("yyyy-MM-dd");
        }

        if (dtPersonalDetailsForEdit.Rows[0]["TerminationDate"].ToString() != "")
        {
            //txtReleaseDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["TerminationDate"].ToString()).ToString("yyyy-MM-dd");
        }
        ddlempStatus.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsActive"].ToString().Trim();
        if (ddlempStatus.SelectedValue == "NO")
        {
            if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
                txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        }
        txtOccupation.Text = dtPersonalDetailsForEdit.Rows[0]["Occupation"].ToString();
        //txtMainEmail.ReadOnly = true;
        if (dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            GetAddressDetails(int.Parse(dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        }
        if (dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString() != "")
        {
            //GetPaymentForEdit(int.Parse(dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString()));
        }
        hfdUserId.Value = comm.GetNameFromId("UserId", EmployeeID);

        GetList();
        GetAssgineLink();
        GetEmpNameForAllTab(EmployeeID);

        GetRateInformation(hfdEmpID.Value);
    }
	
    public void GetEmpNameForAllTab(int EmpId)
    {
        //lblEmpNameForAddressCont.Text = comm.GetNameFromId("AddEmployee", EmpId);
        lblEmpNameAssignMenu.Text = lblEmpNameForAddressCont.Text;
        lblEmpNameForPaymentInfo.Text = lblEmpNameForAddressCont.Text;
        lblEmpNameForUserCredentials.Text = lblEmpNameForAddressCont.Text;
    }
	
    public void GetAddressDetails(int EmpAddressId)
    {
        DataTable dtAddressDetailForEdit = new DataTable();
        dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + EmpAddressId + "", "");

        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        ddlNumberSuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["NumberSuffix"].ToString();
        txtStreetName.Text = dtAddressDetailForEdit.Rows[0]["StreetName"].ToString().ToUpper();

        txtStreetType.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetType1.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetDirection.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();
        txtStreetDirection1.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();

        DropDownList1.SelectedIndex = DropDownList1.Items.IndexOf(DropDownList1.Items.FindByValue(dtAddressDetailForEdit.Rows[0]["SuitType"].ToString()));
        txtSuiteNumber.Text = dtAddressDetailForEdit.Rows[0]["SuiteNumber"].ToString();
        ddlsuitesuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["SuitSuffix"].ToString();
        txtCity.Text = dtAddressDetailForEdit.Rows[0]["City"].ToString().ToUpper();
        txtProvince.Text = dtAddressDetailForEdit.Rows[0]["Province"].ToString().ToUpper();
        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtWorkPhone.Text = dtAddressDetailForEdit.Rows[0]["WorkPhone"].ToString();
        txtMobile.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();
        txtFax.Text = dtAddressDetailForEdit.Rows[0]["Fax"].ToString();
        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        hfdMainEmail.Value = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        if (Request.QueryString["BussEditId"] != null)
        {
            bool ChkUType = comm.IfExistsInTable("AddEmployee ad join EmpAddress ae on ad.ADDRESSID=ae.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "ad.UserType", "AE.MainEmail='" + dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString() + "' and ad.UserType=1 and assb.BusinessID='" + Request.QueryString["BussEditId"].ToString() + "'");
            if (ChkUType == true)
            {
                txtMainEmail.Enabled = false;
            }
            else
            {
                txtMainEmail.Enabled = true;
            }
        }

        txtCCEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        hfdCCEmail.Value = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        txtWebsite.Text = dtAddressDetailForEdit.Rows[0]["Website"].ToString();
        txtOther1.Text = dtAddressDetailForEdit.Rows[0]["Other1"].ToString();
        txtStreetNo.Text = dtAddressDetailForEdit.Rows[0]["StreetNo"].ToString();

        txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();

        if (dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString() == dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString())
            ddlCopyLoginEmail.SelectedValue = "Yes";
        if (dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString() != dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString())
            ddlCopyLoginEmail.SelectedValue = "No";
    }

    public void GetPaymentForEdit(int Rateid)
    {
        DataTable dtRateDetailsForEdit = new DataTable();
        dtRateDetailsForEdit = comm.GetAllDataFromTable("RateDetails", "*", "RateId=" + Rateid + " and EmployeeId=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        txtBillRate.Text = dtRateDetailsForEdit.Rows[0]["Rate"].ToString();
        ddlBillingType.SelectedValue = dtRateDetailsForEdit.Rows[0]["RateType"].ToString();
        ddlJobTitle.SelectedValue = dtRateDetailsForEdit.Rows[0]["JobTitle"].ToString();
        ddlEmpEI.SelectedValue = dtRateDetailsForEdit.Rows[0]["IsEmployeeEIExempty"].ToString();
        ddlEmpCPP.SelectedValue = dtRateDetailsForEdit.Rows[0]["IsEmployeeCPPExempty"].ToString();
        ddlRTVA.SelectedValue = dtRateDetailsForEdit.Rows[0]["RetainVacation"].ToString();
    }

    public void GetList()
    {
        DataTable dtlist = new DataTable();
        string empid;
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            empid = Request.QueryString["EmployeeIDEdit"].ToString();
        }
        else
        {
            empid = hfdEmpID.Value;
        }
        //dtlist = comm.GetAllDataFromTable("AddEmployee emp  left join EmpAddress ea on ea.EmpAddressId=emp.ADDRESSID join useracc ua on ua.Email=ea.MainEmail", "distinct emp.FirstName,ea.MainEmail,ua.Password,dbo.FN_DESC('UserType',emp.UserType)UserType", "emp.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and ua.Email='" + txtMainEmail.Text + "' and emp.EmployeeID=" + empid + "", "");
        //GVDomainList.DataSource = dtlist;
        //GVDomainList.DataBind();
        dtlist = comm.GetAllDataFromTable("AddEmployee emp  left join EmpAddress ea on ea.EmpAddressId=emp.ADDRESSID join useracc ua on ua.Email=ea.MainEmail", "distinct emp.FirstName,ea.MainEmail,ua.Password,dbo.FN_DESC('UserType',emp.UserType)UserType", "emp.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and ua.Email='" + txtMainEmail.Text + "' and emp.EmployeeID=" + empid + " and Password is not null", "");
        if (dtlist.Rows.Count > 0)
        {
            GVDomainList.DataSource = dtlist;
            GVDomainList.DataBind();
        }
        else
        {
            try
            {
                if (Session["MotherOrgid_acc"] != null)
                {
                    dtlist = comm.GetAllDataFromTable("AddEmployee emp  left join EmpAddress ea on ea.EmpAddressId=emp.ADDRESSID join useracc ua on ua.Email=ea.MainEmail", "distinct emp.FirstName,ea.MainEmail,ua.Password,dbo.FN_DESC('UserType',emp.UserType)UserType", "emp.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + " and ua.Email='" + txtMainEmail.Text + "' and emp.EmployeeID=" + empid + "", "");
                    GVDomainList.DataSource = dtlist;
                    GVDomainList.DataBind();
                }
            }
            catch (Exception ex)
            {
                dtlist = comm.GetAllDataFromTable("AddEmployee emp  left join EmpAddress ea on ea.EmpAddressId=emp.ADDRESSID join useracc ua on ua.Email=ea.MainEmail", "distinct emp.FirstName,ea.MainEmail,''as Password,dbo.FN_DESC('UserType',emp.UserType)UserType", "ua.Email='" + txtMainEmail.Text + "' and emp.EmployeeID=" + empid + "", "");
                GVDomainList.DataSource = dtlist;
                GVDomainList.DataBind();
            }
        }
    }

    //Get User Type
    public void GetStreetNumber()
    {
        //comm.FillDropDownList(ddlusertype11, "UserType", "User_TypeId", "UserType", "", "User_TypeId"); //commented because of new user role requirement in new user role
        comm.FillDropDownList(ddlusertype11, "UserType", "User_TypeId", "UserType", "", "User_TypeId asc");
        AnyminuteEmployeeEditor.Relabel(ddlusertype11);
        DataTable dtUserTypeList = new DataTable();
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            string bid = Request.QueryString["BusinessIdFromDefaultPage"].ToString();
            string emailid = Session["email"].ToString();
            DataTable dt = comm.GetAllDataFromTable(" EmpAddress ea join AddEmployee ae on ea.EmpAddressId=ae.ADDRESSID join AssginBussiness ab on ab.EmployeeID=ae.EmployeeID", "ab.UserTypeId", "ea.MainEmail='" + emailid + "' and ae.UserType=2 and ab.BusinessID=" + bid + "", "");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["UserTypeId"].ToString() == "1")
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType");
                }
                else
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1,2,5,12)", "UserType");
                }
            }
            else
            {
                int chk = 0;
                DataTable dtOwnerCheck = comm.GetAllDataFromTable(" EmpAddress ea join AddEmployee ae on ea.EmpAddressId=ae.ADDRESSID join AssginBussiness ab on ab.EmployeeID=ae.EmployeeID", "ab.UserTypeId", "ea.MainEmail='" + emailid + "' and ae.UserType in (1,5) and ab.BusinessID=" + bid + "", "ab.UserTypeId");
                if (dtOwnerCheck.Rows.Count == 1)
                {
                    if (dtOwnerCheck.Rows[0]["UserTypeId"].ToString() == "1")
                    {
                        dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType");
                    }
                    else if (dtOwnerCheck.Rows[0]["UserTypeId"].ToString() == "5")
                    {
                        dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1,5)", "UserType");
                    }
                }
                else
                {
                    for (int i = 0; i < dtOwnerCheck.Rows.Count - 1; i++)
                    {
                        if (dtOwnerCheck.Rows[i]["UserTypeId"].ToString() == "1")
                        {
                            if (dtOwnerCheck.Rows[i]["UserTypeId"].ToString() == "5")
                            {
                                chk = 1;
                            }
                        }
                    }
                    if (chk == 1)
                    {
                        dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType");
                    }
                    else
                    {
                        if (Session["UserTypeid"].ToString() == "4")
                        {
                            dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType");
                        }
                        else
                        {
                            dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType");
                        }
                    }
                }
            }
        }

        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            string bid = "0";
            DataTable dtbid = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
            if (dtbid.Rows.Count > 0)
            {
                bid = dtbid.Rows[0]["BusinessID"].ToString();
            }

            string emailid = Session["email"].ToString();
            DataTable dt = comm.GetAllDataFromTable(" EmpAddress ea join AddEmployee ae on ea.EmpAddressId=ae.ADDRESSID join AssginBussiness ab on ab.EmployeeID=ae.EmployeeID", "ab.UserTypeId", "ea.MainEmail='" + emailid + "' and ae.UserType=2 and ab.BusinessID=" + bid + "", "");
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["UserTypeId"].ToString() == "1")
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType ");
                }
                else
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1,2,5,12)", "UserType ");
                }
            }
            else
            {
                DataTable dtnn = comm.GetAllDataFromTable(" EmpAddress ea join AddEmployee ae on ea.EmpAddressId=ae.ADDRESSID join AssginBussiness ab on ab.EmployeeID=ae.EmployeeID", "ab.UserTypeId", "ea.MainEmail='" + emailid + "' and ae.UserType=1 and ab.BusinessID=" + bid + "", "");

                if (dtnn.Rows.Count > 0)
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1)", "UserType ");
                }
                else
                {
                    dtUserTypeList = comm.GetAllDataFromTable("UserType", "User_TypeId,UserType", "User_TypeId not in (1,5)", "UserType ");
                }
            }
        }

        rdoBtnUserTypeList.DataSource = dtUserTypeList;
        rdoBtnUserTypeList.DataValueField = "User_TypeId";
        rdoBtnUserTypeList.DataTextField = "UserType";
        rdoBtnUserTypeList.DataBind();
        AnyminuteEmployeeEditor.Relabel(rdoBtnUserTypeList);
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

    protected void GVDomainList_PreRender(object sender, EventArgs e)
    {
        if (GVDomainList.Rows.Count > 0)
        {
            GVDomainList.UseAccessibleHeader = true;
            GVDomainList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {

    }

    public void GetAssgineLink()
    {
        DataTable dtAssgineLink = new DataTable();
        string empid;
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            empid = Request.QueryString["EmployeeIDEdit"].ToString();
        }
        else
        {
            empid = hfdEmpID.Value;
        }
        dtAssgineLink = comm.GetAllDataFromTable("UserAcc", "*", "EmployeeId=" + empid + "", "");
        if (dtAssgineLink.Rows.Count > 0)
        {
            Fill_TreeviewLink(tvLinks, dtAssgineLink.Rows[0]["LinkAcc"].ToString());
        }
    }

    public void Fill_TreeviewLink(TreeView tvlink, string links)
    {
        SqlDataReader drLinks = null;
        int linkno = 0;
        int mastno = 0;
        int url_idno = 0;
        TreeNode xx = null;
        TreeNode yy = null;
        TreeNode zz = null;

        tvLinks.Nodes.Clear();
        try
        {
            drLinks = CL.GetLinkForAdminTree(int.Parse(Session["MotherOrgid"].ToString()));//GetLinkForAdminTree();

            while (drLinks.Read())
            {
                if (drLinks["Title"].ToString().Trim() != "")
                {
                    if (drLinks["id"] != null & drLinks["id"].ToString() != "" & int.Parse(drLinks["id"].ToString()) != linkno)
                    {
                        xx = new TreeNode();  // this is defination of the node.
                        xx.Text = drLinks["DomainName"].ToString();
                        xx.NavigateUrl = "";
                        xx.SelectAction = TreeNodeSelectAction.Expand;
                        tvLinks.Nodes.Add(xx);

                        if (drLinks["url"].ToString().Trim() == "" || drLinks["url"] != null)
                        {
                            zz = new TreeNode();    // this is defination of the node.
                            zz.Text = drLinks["Title"].ToString();
                            zz.Value = drLinks["MenuID"].ToString();
                            zz.ToolTip = drLinks["MenuID"].ToString();
                            zz.ShowCheckBox = true;
                            zz.NavigateUrl = "";
                            zz.SelectAction = TreeNodeSelectAction.Expand;

                            if (!links.Equals(string.Empty) && SearchLink(links, drLinks["MenuID"].ToString()))
                                zz.Checked = true;

                            mastno = int.Parse(drLinks["mastno"].ToString());
                            // adding node as child of node xx.
                            xx.ChildNodes.Add(zz);
                        }
                        else
                        {
                            yy = new TreeNode();
                            yy.Text = drLinks["Title"].ToString();
                            yy.Value = drLinks["MenuID"].ToString();
                            yy.ToolTip = drLinks["MenuID"].ToString();
                            yy.ShowCheckBox = true;

                            if (!links.Equals(string.Empty) && SearchLink(links, drLinks["al_no"].ToString()))
                                yy.Checked = true;
                            if ((drLinks["mastno"] != null & drLinks["mastno"].ToString() != ""))
                            {
                                if (int.Parse(drLinks["mastno"].ToString()) == mastno & int.Parse(drLinks["mastno"].ToString()) != 0)
                                {
                                    // adding node as child of node xx.
                                    zz.ChildNodes.Add(yy);
                                }
                                else
                                {
                                    // adding node as child of node xx.
                                    xx.ChildNodes.Add(yy);
                                }
                            }
                            else
                            {
                                // adding node as child of node xx.
                                xx.ChildNodes.Add(yy);
                            }
                        }
                    }
                    else
                    {
                        if (drLinks["url"].ToString() == "")
                        {
                            zz = new TreeNode();   // this is defination of the node.
                            zz.Text = drLinks["Title"].ToString();
                            zz.ShowCheckBox = true;
                            zz.Value = drLinks["MenuID"].ToString();
                            zz.ToolTip = drLinks["MenuID"].ToString();
                            zz.NavigateUrl = "";
                            zz.SelectAction = TreeNodeSelectAction.Expand;

                            if (!links.Equals(string.Empty) && SearchLink(links, drLinks["MenuID"].ToString()))
                                zz.Checked = true;

                            if (drLinks["mastno"] != null & drLinks["mastno"].ToString() != "")
                                mastno = int.Parse(drLinks["mastno"].ToString());

                            // adding node as child of node xx.
                            xx.ChildNodes.Add(zz);
                        }
                        else
                        {
                            yy = new TreeNode();
                            yy.Text = drLinks["Title"].ToString();
                            yy.Value = drLinks["MenuID"].ToString();
                            yy.ToolTip = drLinks["MenuID"].ToString();
                            yy.ShowCheckBox = true;
                            //if (links.Contains(drLinks["al_no"].ToString()))
                            if (!links.Equals(string.Empty) && SearchLink(links, drLinks["MenuID"].ToString()))
                                yy.Checked = true;
                            if ((drLinks["mastno"] != null & drLinks["mastno"].ToString() != ""))
                            {
                                if (int.Parse(drLinks["mastno"].ToString()) == mastno & int.Parse(drLinks["mastno"].ToString()) != 0)
                                {
                                    zz.ChildNodes.Add(yy);
                                }
                                else
                                {
                                    xx.ChildNodes.Add(yy);
                                }
                            }
                            else
                            {
                                xx.ChildNodes.Add(yy);
                            }
                        }
                    }
                }
                if (drLinks["id"] != null & drLinks["id"].ToString() != "")
                    linkno = int.Parse(drLinks["id"].ToString());
                if (drLinks["mastno"] != null & drLinks["mastno"].ToString() != "")
                    mastno = int.Parse(drLinks["mastno"].ToString());
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool SearchLink(string lnks, string linkno)
    {
        char sp = ',';  //separator
        string[] links = lnks.Split(sp);

        for (int i = 0; i < links.Length; i++)
        {
            if (links[i].Equals(linkno))
                return true;
        }
        return false;
    }

    public int SavePersonalDetails()
    {
        int ResultSavePersonalDetails = 0;
        try
        {
            if (btnsaveRole.Text != "Update")
            {
                EmpEntity.SINExpiry = txtSINExpiry.Text;
                EmpEntity.UserType = 0;
                string userTypeUserACC = AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem);
                EmpEntity.LegalName = txtLegalName.Text;
                EmpEntity.Title = txtLegalName.SelectedItem.Text;
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                EmpEntity.IsActive = ddlempStatus.SelectedValue;
                if (ddlempStatus.SelectedValue == "NO")
                {
                    EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
                }
                else
                {
                    EmpEntity.Apartment = "";
                }

                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;
                try
                {
                    if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
                    {
                        DataTable dataTable = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + name.ToString() + "", "BusinessID");
                        EmpEntity.MotherOrgID = int.Parse(dataTable.Rows[0][0].ToString());
                    }

                    else
                    {
                        EmpEntity.MotherOrgID = int.Parse(hfdMotherId.Value);
                    }
                }
                catch (Exception ex)
                {

                    EmpEntity.MotherOrgID = int.Parse(hfdMotherId.Value);
                }

                EmpEntity.Occupation = txtOccupation.Text;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                ResultSavePersonalDetails = Empid;
                //if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
                //{
                //    int BusinessIdFromDefaultPapge = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                //    string BusinessName = comm.GetNameFromId("AddBusiness", BusinessIdFromDefaultPapge);
                //    string EmployeeName = comm.GetNameFromId("AddEmployee", Empid);
                //    comm.ExecuteQuery("Insert AssginBussiness (BusinessID,EmployeeID,BusinessName,EmployeeName,MotherOrgID) values(" + BusinessIdFromDefaultPapge + "," + Empid + ",'" + BusinessName + "','" + EmployeeName + "'," + Session["MotherOrgid"].ToString() + ")");
                //}

                hfdEmpID.Value = Empid.ToString();
                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Personal Details Added Successfully, Now Add Contact Details', 'success');", true);
                // GetList();
                //GetAssgineLink();
                //clearPersonalDetails();
                // GetEmpNameForAllTab(Empid);
            }
            else
            {
                //if (txtSINExpiry.Text != "")
                //    EmpEntity.SINExpiry = DateTime.Parse(txtSINExpiry.Text);

                if (hfdMainEmail.Value.Trim() == txtMainEmail.Text.Trim())
                {
                    EmpEntity.SINExpiry = txtSINExpiry.Text;

                    //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                    EmpEntity.UserType = 0;
                    string userTypeUserACC = AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem);
                    EmpEntity.LegalName = txtLegalName.Text;
                    EmpEntity.Title = "";
                    EmpEntity.FirstName = txtFirstName.Text;
                    EmpEntity.MiddleName = txtMiddleName.Text;
                    EmpEntity.LastName = txtLastName.Text;
                    if (txtSocialNumer.Text != "")
                    {
                        EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                    }
                    if (txtdob.Text != "")
                    {
                        EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                    }

                    EmpEntity.Gender = ddlgender.SelectedItem.Text;
                    EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                    DataTable dtcntrate = comm.GetAllDataFromTable("RateDetails", "count(*)cnt", "EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "", "");
                    if (int.Parse(dtcntrate.Rows[0]["cnt"].ToString()) == 1)
                    {
                        DataTable dtdetails = comm.GetAllDataFromTable("AddEmployee", "Rateid", "EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "", "");
                        comm.ExecuteQuery("update RateDetails set RateType='" + ddlBillingType.SelectedValue + "',Rate=CONVERT(DECIMAL(10,2)," + txtBillRate.Text + "),StartDate='" + txtHiredate.Text + "',effectivedate='" + txtHiredate.Text + "' where EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "");
                    }

                    //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                    EmpEntity.TerminationDate = DateTime.Now.Date;

                    RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                    RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                    RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                    EmpEntity.IsActive = ddlempStatus.SelectedValue;
                    if (ddlempStatus.SelectedValue == "NO")
                    {
                        if (txtInActiveDate.Text == "")
                            EmpEntity.Apartment = ""; // DateTime.Parse(DateTime.Now.ToShortDateString()).ToString();//Inactivedate

                        if (txtInActiveDate.Text != "")
                            EmpEntity.Apartment = DateTime.Parse(txtInActiveDate.Text.ToString()).ToString();//Inactivedate
                    }
                    EmpEntity.EmployeeID = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                    EmpEntity.Occupation = txtOccupation.Text;
                    comm.ExecuteQuery("update AddEmployee set Apartment='" + EmpEntity.Apartment + "',IsActive='" + EmpEntity.IsActive + "' where EmployeeID=" + EmpEntity.EmployeeID + "");

                    if (txtSocialNumer.Text != "" && txtdob.Text == "")
                        ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails_WithSIN(EmpEntity, RateEntity);
                    if (txtSocialNumer.Text == "" && txtdob.Text != "")
                        ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails_WithDOB(EmpEntity, RateEntity);
                    if (txtSocialNumer.Text == "" && txtdob.Text == "")
                        ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails_WithoutSINDOD(EmpEntity, RateEntity);
                    if (txtSocialNumer.Text != "" && txtdob.Text != "")
                        ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);

                    DataTable dtgetemprateid = comm.GetAllDataFromTable("RateDetails", "top 1 RateId", "EmployeeId='" + EmpEntity.EmployeeID + "'", "RateId asc");
                    comm.ExecuteQuery("update RateDetails set StartDate='" + EmpEntity.HireDate.ToString("yyyy-MM-dd") + "', effectivedate='" + EmpEntity.HireDate.ToString("yyyy-MM-dd") + "' where RateId = '" + int.Parse(dtgetemprateid.Rows[0][0].ToString()) + "' and EmployeeId='" + EmpEntity.EmployeeID + "'");
                }
                else
                {
                    EmpEntity.SINExpiry = txtSINExpiry.Text;

                    //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                    EmpEntity.UserType = 0;
                    string userTypeUserACC = AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem);
                    EmpEntity.LegalName = txtLegalName.Text;
                    EmpEntity.Title = "";
                    EmpEntity.FirstName = txtFirstName.Text;
                    EmpEntity.MiddleName = txtMiddleName.Text;
                    EmpEntity.LastName = txtLastName.Text;
                    EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                    EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                    EmpEntity.Gender = ddlgender.SelectedItem.Text;
                    EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                    EmpEntity.IsActive = ddlempStatus.SelectedValue;
                    if (ddlempStatus.SelectedValue == "NO")
                    {
                        EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
                    }
                    else
                    {
                        EmpEntity.Apartment = "";
                    }

                    //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                    EmpEntity.TerminationDate = DateTime.Now.Date;
                    EmpEntity.MotherOrgID = int.Parse(hfdMotherId.Value);////int.Parse(Session["MotherOrgid"].ToString());
                    EmpEntity.Occupation = txtOccupation.Text;
                    RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                    RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                    RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                    int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
                        ResultSavePersonalDetails = Empid;
                    //if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
                    //{
                    //    int BusinessIdFromDefaultPapge = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                    //    string BusinessName = comm.GetNameFromId("AddBusiness", BusinessIdFromDefaultPapge);
                    //    string EmployeeName = comm.GetNameFromId("AddEmployee", Empid);
                    //    comm.ExecuteQuery("Insert AssginBussiness (BusinessID,EmployeeID,BusinessName,EmployeeName,MotherOrgID) values(" + BusinessIdFromDefaultPapge + "," + Empid + ",'" + BusinessName + "','" + EmployeeName + "'," + Session["MotherOrgid"].ToString() + ")");
                    //}

                    hfdEmpID.Value = Empid.ToString();
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Personal Details Added Successfully, Now Add Contact Details', 'success');", true);
                    // GetList();
                    //GetAssgineLink();
                    //clearPersonalDetails();
                    // GetEmpNameForAllTab(Empid);
                }

                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Personal Details Updated Successfully', 'success');", true);
                //GetList();
                //GetAssgineLink();
                //clearPersonalDetails();
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
        //if (AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem) == "Please Select")
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select User Type', 'warning');", true);
        //    return;
        //}

        if (txtReleaseDate.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Release', 'warning');", true);
            return;
        }

        if (btnSavePersonalDetails.Text != "Update")
        {
            //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
            EmpEntity.UserType = 0;
            string userTypeUserACC = AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem);
            EmpEntity.LegalName = txtLegalName.Text;
            EmpEntity.Title = "";
            EmpEntity.FirstName = txtFirstName.Text;
            EmpEntity.MiddleName = txtMiddleName.Text;
            EmpEntity.LastName = txtLastName.Text;
            EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
            EmpEntity.DateOfBirth = txtdob.Text;
            EmpEntity.Gender = ddlgender.SelectedItem.Text;
            EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
            //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
            EmpEntity.TerminationDate = DateTime.Now.Date;
            EmpEntity.MotherOrgID = int.Parse(hfdMotherId.Value); //int.Parse(Session["MotherOrgid"].ToString());
            EmpEntity.Occupation = txtOccupation.Text;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);
            //if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
            //{
            //    int BusinessIdFromDefaultPapge = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
            //    string BusinessName = comm.GetNameFromId("AddBusiness", BusinessIdFromDefaultPapge);
            //    string EmployeeName = comm.GetNameFromId("AddEmployee", Empid);
            //    comm.ExecuteQuery("Insert AssginBussiness (BusinessID,EmployeeID,BusinessName,EmployeeName,MotherOrgID) values(" + BusinessIdFromDefaultPapge + "," + Empid + ",'" + BusinessName + "','" + EmployeeName + "'," + Session["MotherOrgid"].ToString() + ")");
            //}

            hfdEmpID.Value = Empid.ToString();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Personal Details Added Successfully, Now Add Contact Details', 'success');", true);
            // GetList();
            //GetAssgineLink();
            //clearPersonalDetails();
            GetEmpNameForAllTab(Empid);
        }
        else
        {
            //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
            EmpEntity.UserType = 0;
            string userTypeUserACC = AnyminuteEmployeeEditor.StoredName(ddlusertype11.SelectedItem);
            EmpEntity.LegalName = txtLegalName.Text;
            EmpEntity.Title = "";
            EmpEntity.FirstName = txtFirstName.Text;
            EmpEntity.MiddleName = txtMiddleName.Text;
            EmpEntity.LastName = txtLastName.Text;
            EmpEntity.SocialInsuranceNumber = txtSocialNumer.Text;
            EmpEntity.DateOfBirth = txtdob.Text;
            EmpEntity.Gender = ddlgender.SelectedItem.Text;
            EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
            //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
            EmpEntity.TerminationDate = DateTime.Now.Date;

            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;

            EmpEntity.EmployeeID = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
            EmpEntity.Occupation = txtOccupation.Text;

            EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);

            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Personal Details Updated Successfully', 'success');", true);
            //GetList();
            //GetAssgineLink();
            //clearPersonalDetails();
        }
    }

    public void clearPersonalDetails()
    {
        ddlusertype11.SelectedValue = "";
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

    public void ClearUserCredentials()
    {
        GVDomainList.DataSource = null;
        GVDomainList.DataBind();
    }

    public void ClearAllFields()
    {
        clearPersonalDetails();
        ClearAddressContactDetails();
        ClearPaymentDetails();
        ClearUserCredentials();
    }

    public int SaveAddress()
    {
        int ResultSaveAddres = 0;
        DataTable cityid = new DataTable();
        DataTable province = new DataTable();
        try
        {
            bool chk = comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtCity.Text + "'");
            if (chk == false)
            {
                // ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtCity.Text + " not Exists, Contact Support Admin ', 'error');", true);
                //return;
                comm.ExecuteQuery("insert into CityMaster (CityName,Duplicate)values('" + txtCity.Text + "','False')");
            }

            bool chkProvinceMaster = comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtProvince.Text + "'");
            if (chkProvinceMaster == false)
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtProvince.Text + " not Exists, Contact Support Admin ', 'error');", true);
                // return;
                comm.ExecuteQuery("insert into ProvinceMaster (ProvinceName,Duplicate)values('" + txtProvince.Text + "','False')");
                province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            }
            cityid = comm.GetAllDataFromTable("CityMaster", "CityId", "CityName='" + txtCity.Text + "'", "");
            province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            txtPostalCode.Text = txtPostalCode.Text.Replace(" ", string.Empty).Trim();
            EmpAddEnity.Postalcode = txtPostalCode.Text;
            EmpAddEnity.NumberSuffix = ddlNumberSuffix.SelectedItem.Text;
            EmpAddEnity.StreetName = txtStreetName.Text.ToUpper();
            EmpAddEnity.StreetType = txtStreetType.Text.ToUpper();
            EmpAddEnity.StreetDirection = txtStreetDirection.Text;
            //EmpAddEnity.SuitType = ddlSuitType.SelectedItem.Text;
            EmpAddEnity.SuitType = DropDownList1.SelectedItem.Text;
            EmpAddEnity.SuiteNumber = txtSuiteNumber.Text;
            EmpAddEnity.SuitSuffix = ddlsuitesuffix.SelectedItem.Text;
            EmpAddEnity.City = txtCity.Text.ToUpper();
            EmpAddEnity.Province = txtProvince.Text.ToUpper();
            EmpAddEnity.MainPhone = txtMainPhone.Text;
            EmpAddEnity.WorkPhone = txtWorkPhone.Text;
            EmpAddEnity.Mobile = txtMobile.Text;
            EmpAddEnity.Fax = txtFax.Text;
            EmpAddEnity.MainEmail = txtMainEmail.Text.Trim(); ;
            EmpAddEnity.CCEmail = txtCCEmail.Text;
            EmpAddEnity.Website = txtWebsite.Text;
            EmpAddEnity.Other1 = txtOther1.Text;
            EmpAddEnity.StreetNo = txtStreetNo.Text.ToUpper();
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            if (cityid.Rows.Count > 0)
            {
                EmpAddEnity.CityId = int.Parse(cityid.Rows[0]["CityId"].ToString());
            }
            else
            {
                EmpAddEnity.CityId = 0;
            }

            if (province.Rows.Count > 0)
            {
                EmpAddEnity.ProvinceId = int.Parse(province.Rows[0]["ProvinceId"].ToString());
            }
            else
            {
                EmpAddEnity.ProvinceId = 0;
            }
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            EmpAddEnity.MotherOrgId = int.Parse(hfdMotherId.Value);//int.Parse(Session["MotherOrgid"].ToString());

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));

            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Saved Successfully', 'success');", true);
            GetList();
            return ResultSaveAddres;

            //ClearAddressContactDetails();
        }
        catch (Exception ex)
        {
            throw ex;
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Save Contact Details', 'error');", true);
            //return;
        }
    }
    protected void btnSaveAddres_Click(object sender, EventArgs e)
    {
        int ResultSaveAddres = 0;
        try
        {
            EmpAddEnity.Postalcode = txtPostalCode.Text;
            EmpAddEnity.NumberSuffix = ddlNumberSuffix.SelectedItem.Text;
            EmpAddEnity.StreetName = txtStreetName.Text;
            EmpAddEnity.StreetType = txtStreetType.Text;
            EmpAddEnity.StreetDirection = txtStreetDirection.Text;
            EmpAddEnity.SuitType = ddlSuitType.SelectedItem.Text;
            EmpAddEnity.SuiteNumber = txtSuiteNumber.Text;
            EmpAddEnity.SuitSuffix = ddlsuitesuffix.SelectedItem.Text;
            EmpAddEnity.City = txtCity.Text;
            EmpAddEnity.Province = txtProvince.Text;
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
            EmpAddEnity.MotherOrgId = int.Parse(hfdMotherId.Value);//int.Parse(Session["MotherOrgid"].ToString());

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));

            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Saved Successfully', 'success');", true);
            GetList();

            //ClearAddressContactDetails();
        }
        catch (Exception ex)
        {
            throw ex;
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Save Contact Details', 'error');", true);
            //return;
        }
    }

    public void ClearAddress()
    {
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        DropDownList1.SelectedValue = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";
        txtStreetNo.Text = "";
    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        if (_serachpostcode.Length.ToString() != "6")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            txtSearchPostalCode.Focus();
            return;
        }
        // ClearAddress();
        if (txtSearchPostalCode.Text.Length.ToString() != "") //== "7")
        {
            string strsearch = txtSearchPostalCode.Text;
            strsearch = strsearch.Replace(" ", String.Empty);

            resultdiv.Visible = true;
            DataTable dtSearchAddress = new DataTable();
            rdobtnlist.Visible = true;
            //dtSearchAddress = comm.GetAllDataFromTable("EmpAddress", "EmpAddressId,StreetName+', '+ StreetType+', '+StreetDirection+', '+City+', '+Province as SEARCHADDRESS", "Postalcode='" + txtSearchPostalCode.Text + "'", "EmpAddressId");
            //dtSearchAddress = comm.GetSearchAddress(txtSearchPostalCode.Text.Trim());
            dtSearchAddress = comm.GetSearchAddress(strsearch);
            DataRow row1 = dtSearchAddress.NewRow();
            row1["id"] = 0;
            row1["SEARCHADDRESS"] = " ADD NEW ADDRESS"; //"None of the Above";
            dtSearchAddress.Rows.Add(row1);
            gvAddressList.DataSource = dtSearchAddress;
            gvAddressList.DataBind();
            DataRow row = dtSearchAddress.NewRow();
            if (dtSearchAddress.Rows.Count == 0)
            {
                //rdobtnlist.ClearSelection();
                row["id"] = 0;
                row["SEARCHADDRESS"] = " Add New Address"; //"None of the Above";
                dtSearchAddress.Rows.Add(row);
                rdobtnlist.DataBind();
                rdobtnlist.DataSource = dtSearchAddress;
                rdobtnlist.DataTextField = "SEARCHADDRESS";
                rdobtnlist.DataValueField = "SEARCHADDRESS";
                rdobtnlist.DataBind();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Address not recorded previously', 'warning');", true);
                return;
            }
            row["id"] = 0;
            row["SEARCHADDRESS"] = " Add New Address";
            dtSearchAddress.Rows.Add(row);
            rdobtnlist.DataSource = dtSearchAddress;
            rdobtnlist.DataTextField = "SEARCHADDRESS";
            rdobtnlist.DataValueField = "SEARCHADDRESS";
            foreach (ListItem itm in rdobtnlist.Items)
            {
                for (int i = 0; i < dtSearchAddress.Rows.Count; i++)
                {
                    itm.Attributes["title"] = dtSearchAddress.Rows[i]["tab"].ToString();
                    itm.Text = dtSearchAddress.Rows[i]["SEARCHADDRESS"].ToString();
                    itm.Value = dtSearchAddress.Rows[i]["id"].ToString();
                }
            }
            rdobtnlist.DataBind();
        }
    }

    protected void rdobtnlist_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtselectresult = new DataTable();
        int AddressId = 0;
        string a = "";
        string address = "";
        divAddres.Attributes.Add("style", "display:block;");
        foreach (ListItem itm in rdobtnlist.Items)
        {
            if (itm.Selected == true)
            {
                //AddressId = int.Parse(itm.Value);
                address = itm.Text;

            }
        }

        dtselectresult = comm.GetAllDataFromTable("EmpAddress e,BusinessAddress b", "*", "e.Postalcode='" + txtSearchPostalCode.Text + "' or b.Postalcode='" + txtSearchPostalCode.Text + "'", "");
        if (address != " Add New Address")
        {
            String[] Split = address.Split(',');
            txtPostalCode.Text = txtSearchPostalCode.Text;//dtselectresult.Rows[0]["Postalcode"].ToString();
            txtPostalCode.ReadOnly = true;
            txtPostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            //txtSuiteNumber.Text = dtselectresult.Rows[0]["SuiteNumber"].ToString();

            txtStreetName.Text = Split[0].ToString();
            txtStreetName.Text = Split[0].ToString();
            txtStreetName.ReadOnly = true;
            txtStreetName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            //txtStreetType.Text = Split[0].ToString();//dtselectresult.Rows[0]["StreetType"].ToString();
            txtStreetType1.Text = Split[0].ToString();
            // txtStreetType.ReadOnly = true;
            // txtStreetType.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            txtCity.Text = Split[1].ToString();//dtselectresult.Rows[0]["City"].ToString();
            txtCity.ReadOnly = true;
            txtProvince.Text = Split[2].ToString();//dtselectresult.Rows[0]["Province"].ToString();
            txtProvince.ReadOnly = true;
        }
        else
        {
            txtSuiteNumber.Text = "";
            txtStreetNo.Text = "";
            DropDownList1.SelectedValue = "";

            txtSuiteNumber.Text = "";
            txtPostalCode.Text = "";
            txtPostalCode.ReadOnly = false;
            txtStreetName.Text = "";
            txtStreetName.ReadOnly = false;
            txtStreetType.Text = "";
            txtStreetType1.Text = "";
            txtStreetType.ReadOnly = false;
            txtCity.Text = "";
            txtProvince.Text = "";
            txtCity.ReadOnly = false;
            txtProvince.ReadOnly = false;
            //ClearAddressContactDetails();
        }
    }

    public int SavePay()
    {
        int ResultSavePay = 0;
        try
        {
            if (txtBillRate.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Billing Rate', 'warning');", true);
                return 0;
            }

            RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
            RateEntity.Rate = float.Parse(txtBillRate.Text);
            RateEntity.RateType = ddlBillingType.SelectedItem.Text;
            //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
            RateEntity.StartDate = DateTime.Parse(txtHiredate.Text);
            RateEntity.EndDate = System.DateTime.Now.Date;
            RateEntity.JobTitle = ddlJobTitle.SelectedValue;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
            RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
            ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));

            GetRateInformation(hfdEmpID.Value);
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);

            return ResultSavePay;
            //ClearPaymentDetails();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnSavePay_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtBillRate.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Billing Rate', 'warning');", true);
                return;
            }
            if (btnSavePay.Text != "Update Rate")
            {
                RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
                RateEntity.Rate = float.Parse(txtBillRate.Text);
                RateEntity.RateType = ddlBillingType.SelectedItem.Text;
                RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
                RateEntity.EndDate = System.DateTime.Now.Date;
                RateEntity.JobTitle = ddlJobTitle.SelectedValue;
                RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
                RateEntity.RetainVacation = ddlRTVA.SelectedValue;
                RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));

                GetRateInformation(hfdEmpID.Value);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);
            }
            else
            {
                comm.ExecuteQuery("update RateDetails set Rate =" + txtBillRate.Text + ", RateType='" + ddlBillingType.SelectedValue + "', StartDate='" + txteffectivedate.Text + "',effectivedate='" + txteffectivedate.Text + "' where RateId=" + hfdEditRateId.Value + "");
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Updated successfully', 'success');", true);
                btnSavePay.Text = "Save";
                hfdEditRateId.Value = "";
                GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
            }
            //ClearPaymentDetails();
        }
        catch
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Save Rate Details', 'error');", true);
        }
    }

    protected void btnAssginelinSave_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dtAssgBusUserType = new DataTable();
            dtAssgBusUserType = comm.GetAllDataFromTable("AddEmployee", "UserType", "EmployeeID=" + hfdEmpID.Value + "", "");

            if (CL.GetSelectedLinks(tvLinks) != "")
            {
                CL.UpdateEmpLink(int.Parse(hfdEmpID.Value), CL.GetSelectedLinks(tvLinks));

                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Menu Assigned Successfully', 'success');", true);

                ClearAllFields();
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Any Menu', 'warning');", true);
                return;
            }
        }
        catch (Exception ex)
        {
            throw (ex);
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Assign Menu', 'error');", true);
            //return;
        }
    }

    protected void lnkratechange_Click(object sender, EventArgs e)
    {
        DataTable dtname = new DataTable();
        int empid = 0;
        if (hfdEmpID.Value == "")
        {
            empid = 0;
        }
        else
        {
            empid = int.Parse(hfdEmpID.Value);
        }
        hfdchangerateid.Value = empid.ToString();
        dtname = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + empid + "", "");
        if (dtname.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Employee Name: " + lblEmpNameForAddressCont.Text);
            sb.AppendLine("Employee Status: " + dtname.Rows[0]["IsActive"].ToString());
            // lblNameemp.Text = "Employee Name: " + lblEmpNameForAddressCont.Text + "\n" + "Is Active :" + dtname.Rows[0]["IsActive"].ToString() + "";
            lblNameempRateHistory.Text = sb.ToString().Replace(Environment.NewLine, "<br />");

        }
        gvRateHistory.DataSource = RateCtrl.GetRateHistory(empid);
        gvRateHistory.DataBind();
        //ModalPopupEmpList.Show();
    }

    protected void btnSaveRate_Click(object sender, EventArgs e)
    {
        try
        {
            DataTable dt = new DataTable();
            dt = comm.GetAllDataFromTable("RateDetails", "*", "RateId=(select MAX(RateId) from RateDetails where EmployeeId=" + hfdchangerateid.Value + ")", "");
            if (dt.Rows.Count > 0)
            {
                RateEntity.EmployeeId = int.Parse(hfdchangerateid.Value);
                RateEntity.Rate = int.Parse(txtrate.Text);
                RateEntity.RateType = ratetype.SelectedItem.Text;
                RateEntity.StartDate = System.DateTime.Now.Date;
                RateEntity.EndDate = System.DateTime.Now.Date;
                RateEntity.JobTitle = dt.Rows[0]["JobTitle"].ToString();
                RateEntity.IsEmployeeEIExempty = dt.Rows[0]["IsEmployeeEIExempty"].ToString();
                RateEntity.IsEmployeeCPPExempty = dt.Rows[0]["IsEmployeeCPPExempty"].ToString();
                RateEntity.RetainVacation = dt.Rows[0]["RetainVacation"].ToString();
                RateEntity.JobTitle = dt.Rows[0]["RetainVacation"].ToString();
                RateEntity.effectivedate = DateTime.Parse(txtEffectiveDateRateHistory.Text).Date;
                RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdchangerateid.Value));

                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Rate Details Changes', 'success');", true);
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'No Rate Details Found Try to Add Rate From Add Employee Page', 'error');", true);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void btnrateDelete_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdRateId = (HiddenField)gvRateHistory.Rows[gvr.RowIndex].FindControl("hfdRateId");
        comm.ExecuteQuery("delete from RateDetails where RateId=" + hfdRateId.Value + "");
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Rate Details Deleted', 'success');", true);
    }

    protected void gvRateHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        LinkButton btnrateEdit = (e.Row.FindControl("btnrateEdit") as LinkButton);
        Label effectivedate = (e.Row.FindControl("effectivedate") as Label);
        Label EndDate = (e.Row.FindControl("EndDate") as Label);
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (e.Row.DataItemIndex == 0)
            {
                //effectivedate.Text = DateTime.ParseExact(effectivedate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                string input = effectivedate.Text;
                DateTime dt;
                if (DateTime.TryParse(input, out dt))
                {
                    string output = dt.ToString("yyyy-MM-dd");
                }
                effectivedate.Text = dt.ToString("yyyy-MM-dd");
                btnrateEdit.Enabled = true;                
            }
            else
            {
                //effectivedate.Text = DateTime.ParseExact(effectivedate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                //EndDate.Text = DateTime.ParseExact(EndDate.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                btnrateEdit.Visible = false;

                string input = effectivedate.Text;
                DateTime dt;
                if (DateTime.TryParse(input, out dt))
                {
                    string output = dt.ToString("yyyy-MM-dd");
                }
                effectivedate.Text = dt.ToString("yyyy-MM-dd");

                string input1 = EndDate.Text;
                DateTime dt1;
                if (DateTime.TryParse(input1, out dt1))
                {
                    string output1 = dt1.ToString("yyyy-MM-dd");
                }
                EndDate.Text = dt1.ToString("yyyy-MM-dd");
            }
        }
    }

    protected void gvRateHistory_PreRender(object sender, EventArgs e)
    {

    }

    protected void GetRateInformation(string empidd)
    {
        DataTable dtname = new DataTable();
        int empid = 0;
        if (hfdEmpID.Value == "")
        {
            empid = 0;
        }
        else
        {
            empid = int.Parse(hfdEmpID.Value);
        }
        hfdchangerateid.Value = empid.ToString();
        dtname = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + empid + "", "");
        if (dtname.Rows.Count > 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>Employee Name: </b>" + lblEmpNameForAddressCont.Text);
            sb.AppendLine("<b>Employee Status: </b>" + dtname.Rows[0]["IsActive"].ToString());
            // lblNameemp.Text = "Employee Name: " + lblEmpNameForAddressCont.Text + "\n" + "Is Active :" + dtname.Rows[0]["IsActive"].ToString() + "";
            lblNameempRateHistory.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
            DataTable dtratede = comm.GetAllDataFromTable("RateDetails", "*,convert(Nvarchar(10),effectivedate,120)effectivedateconv", "RateId='" + dtname.Rows[0]["Rateid"].ToString() + "'", "");
            if (dtratede.Rows.Count > 0)
            {
                DateTime mindate= DateTime.ParseExact(dtratede.Rows[0]["effectivedateconv"].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture); 
                txtrateHistoryEffectiveDate.Attributes.Add("min", mindate.ToString());
            }
        }
        //DataTable dtminrate = comm.GetAllDataFromTable("RateDetails", "MIN(rateid) rateid,Rate,RateType", "EmployeeId=" + empid + " group by Rate,RateType", "");
        DataTable dtminrate = comm.GetAllDataFromTable("RateDetails", "MIN(rateid) rateid,cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "EmployeeId=" + empid + " group by Rate,RateType", "");
        if (dtminrate.Rows.Count > 0)
        {
            ddlBillingType.SelectedValue = dtminrate.Rows[0]["RateType"].ToString();
            txtBillRate.Text = dtminrate.Rows[0]["Rate"].ToString();
        }

        gvRateHistory.DataSource = RateCtrl.GetRateHistory(empid);
        gvRateHistory.DataBind();

        //Old code when there is one entry in rate, edit option in grid will not show
        //if (gvRateHistory.Rows.Count == 1)
        //{
        //    gvRateHistory.Rows[0].Cells[6].Visible = false;
        //}

        var check_If_Rate_EndDateExists = comm.GetAllDataFromTable("RateDetails with (Nolock)", "top 1 rateid, Is_Having_EndDate", "EmployeeId=" + empid + "", "rateid desc");

        //New code when there is one entry in rate, edit option in grid will not show if that one entry has the end date
        if (gvRateHistory.Rows.Count == 1 && check_If_Rate_EndDateExists.Rows[0]["Is_Having_EndDate"].ToString() == "False")
        {
            //gvRateHistory.Rows[0].Cells[6].Visible = false;
        }

        //New code when there is one entry in rate, edit option in grid will show if that one entry has the end date
        if (gvRateHistory.Rows.Count == 1 && check_If_Rate_EndDateExists.Rows[0]["Is_Having_EndDate"].ToString() == "True")
        {
            //gvRateHistory.Rows[0].Cells[6].Visible = true;
        }
    }


    protected void btnrateEdit_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdRateId = (HiddenField)gvRateHistory.Rows[gvr.RowIndex].FindControl("hfdRateId");
        DataTable dtchangeRate = new DataTable();
        hfdEditRateId.Value = hfdRateId.Value;
        dtchangeRate = comm.GetAllDataFromTable("RateDetails", "*,cast((convert(decimal(18, 2), Rate)) as varchar(20)) as RateDecimal", "RateId=" + hfdRateId.Value + "", "");
        //txtrateHistoryBillRate.Text = dtchangeRate.Rows[0]["Rate"].ToString();
        txtrateHistoryBillRate.Text = dtchangeRate.Rows[0]["RateDecimal"].ToString();
        ddlrateHistoryBillType.SelectedValue = dtchangeRate.Rows[0]["RateType"].ToString();
        txtrateHistoryEffectiveDate.Text = DateTime.Parse(dtchangeRate.Rows[0]["effectivedate"].ToString()).ToString("yyyy-MM-dd");
        txtrateHistoryEffectiveDate_adding_newrate.Text = txtrateHistoryEffectiveDate.Text;
        //txtrateHistoryEffectiveDate.Enabled = false;
        btnrateHistoryChangeRate.Text = "Update Rate";
        btnSavePay.Enabled = false;
        HiddenField_PreviousDate.Value = txtrateHistoryEffectiveDate.Text;//this is added for changing message en edit rate

        var Check_If_Rate_EndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=1 order by RateId desc");
        if (Check_If_Rate_EndDate.Rows.Count > 0)
        {
            TextBox_EndDate.Text = DateTime.Parse(dtchangeRate.Rows[0]["EndDate"].ToString()).ToString("yyyy-MM-dd");
            txtrateHistoryEffectiveDate.ReadOnly = true;
            div_rate_enddate.Attributes.Add("style","display:block;");
        }
        else
        {
            var Check_Rate_EndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=0 order by RateId desc");
            if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
            {
                TextBox_EndDate.Text = "";
                txtrateHistoryEffectiveDate.ReadOnly = false;
                div_rate_enddate.Attributes.Add("style", "display:none;");
            }
        }
    }

    public int Validate()
    {
        int output = 0;
        if (txtdob.Text == string.Empty)
        {
            output = 1;
        }
        if (txtSocialNumer.Text.Length != 9)
        {
            output = 2;
        }
        if (txtHiredate.Text == string.Empty)
        {
            output = 3;
        }
        return output;
    }

    public bool IsValidEmailAddress(string email)
    {
        try
        {
            var emailChecked = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    protected void btnsaveRole_Click(object sender, EventArgs e)
    {
        if (btnsaveRole.Text == "Save")
        {
            if (txtMainEmail.Text.Trim() != string.Empty)
            {
                if (IsValidEmail(txtMainEmail.Text) == false)
                {
                    txtMainEmail.Focus();
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Vaild Email  ', 'warning');", true);
                    return;
                }
            }
            //DataTable chkboisActive = comm.GetAllDataFromTable("AddBusiness", "*", "BusinessID=" + name + " and APPROVED='A'", "");
            //if (chkboisActive.Rows.Count == 0)
            //{
            //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business is not Approved', 'warning');", true);
            //    return;
            //}
            if (txtFirstName.Text.Trim() == "")
            {
                txtFirstName.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name', 'warning');", true);
                return;
            }
            if (txtLastName.Text.Trim() == "")
            {
                txtLastName.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name', 'warning');", true);
                return;
            }

            if (float.Parse(txtBillRate.Text) > 10000)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Rate Must Less Than 10000', 'warning');", true);
                return;
            }
            if (txtHiredate.Text == "")
            {
                txtHiredate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter hire date', 'warning');", true);
                return;
            }

            DateTime hdate = DateTime.Parse(txtHiredate.Text).Date;
            DateTime currdate = DateTime.Now.Date;
            if (hdate > currdate)
            {
                txtHiredate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date Cannot be Greater Than Current Date', 'warning');", true);
                return;
            }
            if (ddlBillingType.SelectedItem.Text == " Select")
            {
                ddlBillingType.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select billing type', 'warning');", true);
                return;
            }
            if (txtBillRate.Text.Trim() == "")
            {
                txtBillRate.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter billing rate', 'warning');", true);
                return;
            }

            if (txtStreetNo.Text.Trim() == "")
            {
                txtStreetNo.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter street number', 'warning');", true);
                return;
            }
            if (txtStreetName.Text.Trim() == "")
            {
                txtStreetName.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter street 1', 'warning');", true);
                return;
            }

            if (txtCity.Text.Trim() == "")
            {
                txtCity.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter city', 'warning');", true);
                return;
            }
            if (txtProvince.Text.Trim() == "")
            {
                txtProvince.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter province', 'warning');", true);
                return;
            }
            if (txtPostalCode.Text == "")
            {
                txtPostalCode.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter postal code', 'warning');", true);
                return;
            }
            if (ddlusertyperegular.SelectedIndex == 0)
            {
                ddlusertyperegular.Focus();
                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select User Type', 'warning');", true);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Reporting Type', 'warning');", true);
                return;
            }
            if (ddlusertyperegular.SelectedIndex == 1)
            {
                if (ddlproject.SelectedIndex == -1)
                {
                    ddlproject.Focus();
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Project', 'warning');", true);
                    return;
                }
            }
        }

        #region Different onditions while adding and editing the users
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            if (txtMainEmail.Text.Trim() != string.Empty)
            {
                int BussNo1 = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                bool Chk1 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (2,4)");
                if (Chk1 == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
                    return;
                }
                bool Chk3 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (5)");
                if (Chk3 == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
                    return;
                }

                #region Owner not becoming other profile is commented
                //Added by Prashant. Employee and Manager can not become the Owner or Accountant
                DataTable dt_Check_User_Registrered_As_Employee_Or_Manager = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (2,4)", "");
                if (dt_Check_User_Registrered_As_Employee_Or_Manager.Rows.Count > 0)
                {
                    int Check_UserType = 0;
                    foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                    {
                        if (item_Check_UserType.Selected == true)
                        {
                            Check_UserType = int.Parse(item_Check_UserType.Value);
                        }
                    }
                    if (Check_UserType == 5 || Check_UserType == 12)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Employee or Manager. This user can not become Owner or Accountant.', 'warning');", true);
                        return;
                    }
                }
                //Added by Prashant. Owner can not become Employee or Manager
                DataTable dt_Check_User_Registrered_As_Owner = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (5)", "");
                if (dt_Check_User_Registrered_As_Owner.Rows.Count > 0)
                {
                    int Check_UserType = 0;
                    foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                    {
                        if (item_Check_UserType.Selected == true)
                        {
                            Check_UserType = int.Parse(item_Check_UserType.Value);
                        }
                    }
                    if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 12)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Owner or Accountant. This user can not become Employee or Manager.', 'warning');", true);
                        return;
                    }
                }
                //Added by Prashant. Accountant can not become Employee or Manager
                DataTable dt_Check_User_Registrered_As_Accountant = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (12)", "");
                if (dt_Check_User_Registrered_As_Accountant.Rows.Count > 0)
                {
                    int Check_UserType = 0;
                    foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                    {
                        if (item_Check_UserType.Selected == true)
                        {
                            Check_UserType = int.Parse(item_Check_UserType.Value);
                        }
                    }
                    if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 5)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Owner or Accountant. This user can not become Employee or Manager.', 'warning');", true);
                        return;
                    }
                }
                #endregion
            }

        }

        if (Request.QueryString["EmployeeIDEdit"] != null && Request.QueryString["BussEditId"] != null)
        {
            if(txtMainEmail.Text.Trim()!=string.Empty)
            {
                if (txtMainEmail.Text.Trim() != hfdMainEmail.Value)
                {
                    int BussNo2 = int.Parse(Request.QueryString["BussEditId"].ToString());
                    bool Chk2 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo2 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (2,4)");
                    if (Chk2 == true)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
                        return;
                    }
                    bool Chk3 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo2 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (5)");
                    if (Chk3 == true)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
                        return;
                    }

                    #region Owner not becoming other profile is commented
                    //Added by Prashant. Employee and Manager can not become the Owner or Accountant
                    DataTable dt_Check_User_Registrered_As_Employee_Or_Manager = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (2,4)", "");
                    if (dt_Check_User_Registrered_As_Employee_Or_Manager.Rows.Count > 0)
                    {
                        int Check_UserType = 0;
                        foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                        {
                            if (item_Check_UserType.Selected == true)
                            {
                                Check_UserType = int.Parse(item_Check_UserType.Value);
                            }
                        }
                        if (Check_UserType == 5 || Check_UserType == 12)
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Employee or Manager. This user can not become Owner or Accountant.', 'warning');", true);
                            return;
                        }
                    }
                    //Added by Prashant. Owner can not become Employee or Manager
                    DataTable dt_Check_User_Registrered_As_Owner = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (5)", "");
                    if (dt_Check_User_Registrered_As_Owner.Rows.Count > 0)
                    {
                        int Check_UserType = 0;
                        foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                        {
                            if (item_Check_UserType.Selected == true)
                            {
                                Check_UserType = int.Parse(item_Check_UserType.Value);
                            }
                        }
                        if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 12)
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Owner or Accountant. This user can not become Employee or Manager.', 'warning');", true);
                            return;
                        }
                    }
                    //Added by Prashant. Accountant can not become Employee or Manager
                    DataTable dt_Check_User_Registrered_As_Accountant = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (12)", "");
                    if (dt_Check_User_Registrered_As_Accountant.Rows.Count > 0)
                    {
                        int Check_UserType = 0;
                        foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
                        {
                            if (item_Check_UserType.Selected == true)
                            {
                                Check_UserType = int.Parse(item_Check_UserType.Value);
                            }
                        }
                        if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 5)
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Owner or Accountant. This user can not become Employee or Manager.', 'warning');", true);
                            return;
                        }
                    }
                    #endregion
                }
            }
        }

        int SameUser = 0;
        foreach (ListItem item in rdoBtnUserTypeList.Items)
        {
            if (item.Selected == true)
            {
                SameUser = int.Parse(item.Value);
            }
        }
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && txtMainEmail.Text.Trim() != string.Empty)
        {
            bool SameCheck = comm.IfExistsInTable("EmpAddress empadd join AddEmployee ea on ea.ADDRESSID=empadd.EmpAddressId join AssginBussiness ass on ass.EmployeeID=ea.EmployeeID", " *", "empadd.MainEmail='" + txtMainEmail.Text.Trim() + "' and ass.BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' and ea.UserType=" + SameUser + "");
            if (SameCheck == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is already in use, add another Login Email ', 'warning');", true);
                return;
            }

            #region User can become owner and accountant in different mother org also.
            //Commented because user can become owner and accountant in different motherorg also
            //DataTable dt_Check_If_User_Registrered_As_Owner = comm.GetAllDataFromTable("UserAcc", "*", "Email='" + Session["email"].ToString() + "'", "");
            //if (dt_Check_If_User_Registrered_As_Owner.Rows[0]["MotherOrgid"].ToString() == "4")
            //{
            //    DataTable dt_Check_User_Registrered__in_Other_MotherOrg = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=5", "");
            //    if (dt_Check_User_Registrered__in_Other_MotherOrg.Rows.Count > 0)
            //    {
            //        if (dt_Check_User_Registrered__in_Other_MotherOrg.Rows[0]["MotherOrgID"].ToString() != "4")
            //        {
            //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Owner Already Exists With Email " + txtMainEmail.Text + "', 'warning');", true);
            //            return;
            //        }
            //    }
            //}

            //if (dt_Check_If_User_Registrered_As_Owner.Rows[0]["MotherOrgid"].ToString() != "4")
            //{
            //    DataTable dt_Check_User_Registrered__in_Other_MotherOrg = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=5", "");
            //    if (dt_Check_User_Registrered__in_Other_MotherOrg.Rows.Count > 0)
            //    {
            //        if (dt_Check_User_Registrered__in_Other_MotherOrg.Rows[0]["MotherOrgID"].ToString() == "4")
            //        {
            //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Owner Already Exists With Email " + txtMainEmail.Text + "', 'warning');", true);
            //            return;
            //        }
            //    }
            //}
            #endregion
        }

        if (ddlusertyperegular.SelectedIndex == 0)
        {
            ddlusertyperegular.Focus();
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select User Type', 'warning');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Reporting Type', 'warning');", true);
            return;
        }
        if (ddlusertyperegular.SelectedIndex == 1)
        {
            if (ddlproject.SelectedIndex == -1)
            {
                ddlproject.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Project', 'warning');", true);
                return;
            }
        }
        // BO AS EMP MANG
        int bidtt = 0;

        DataTable dt = new DataTable();
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            bidtt = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
        else if (Request.QueryString["BussEditId"] != null)
        {
            bidtt = int.Parse(Request.QueryString["BussEditId"].ToString());
        }

        dt = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ad on ea.EmpAddressId=ad.ADDRESSID join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "distinct ad.EmployeeID,assb.BusinessID", "ea.MainEmail='" + txtMainEmail.Text + "' and assb.UserTypeId=1 and assb.BusinessID=" + bidtt + "", "BusinessID");
        if (dt.Rows.Count > 0)
        {
            if (SameUser != 5)
            {
                txtMainEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'User Can register only as a Owner in own business', 'warning');", true);
                return;
            }
        }
        int chkBoisBid = 0;
        bool boolchkBois = false;
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && txtMainEmail.Text != string.Empty)
        {
            chkBoisBid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
        DataTable chkBois = new DataTable();
        chkBois = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType=1 and  assb.BusinessID=" + chkBoisBid + "", "assb.BusinessID");
        if (chkBois.Rows.Count > 0)
        {
            if (SameUser != 5)
            {
                txtMainEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', '" + txtMainEmail.Text + " is Already Bussiness Owner', 'warning');", true);
                return;
            }
        }

        DataTable dtChkBoAccWise = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=1", "");
        bool boolBoAccWise = false;
        foreach (DataRow r in dtChkBoAccWise.Rows)
        {
            if (r["MotherOrgID"].ToString() != hfdMotherId.Value)
            {
                boolBoAccWise = true;
                break;
            }
        }
        if (boolBoAccWise == true)
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business Owner is Already Exists with " + txtMainEmail.Text + " Email , Use Another Email', 'warning');", true);
            //return;
        }

        bool ceck = false;
        ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
        if (ceck == false)
        {
            if (txtConLoginEmail.Text == string.Empty)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Confirm Email', 'warning');", true);
                return;
            }

        }
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            //bool ChkOwner = comm.IfExistsInTable("MappingTable", "*", "BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and UserTypeId=5");
            //if (ChkOwner == true)
            //{
            //    int UserType = 0;
            //    string UserTypeName = "";
            //    foreach (ListItem item in rdoBtnUserTypeList.Items)
            //    {
            //        if (item.Selected == true)
            //        {
            //            UserType = int.Parse(item.Value);
            //            UserTypeName = item.Text;
            //        }
            //    }
            //    if (UserType == 5)
            //    {
            //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Owner Already Exists', 'warning');", true);
            //        return;
            //    }

            //}
        }
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            bool ChkOwner = comm.IfExistsInTable("MappingTable", "*", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + " and UserTypeId=5");
            if (ChkOwner == true)
            {
                int UserType = 0;
                string UserTypeName = "";
                foreach (ListItem item in rdoBtnUserTypeList.Items)
                {
                    if (item.Selected == true)
                    {
                        UserType = int.Parse(item.Value);
                        UserTypeName = item.Text;
                    }
                }
                if (UserType == 2 || UserType == 4)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Owner Cannot be LEVEL 1 (EMPLOYEE) or LEVEL 2 (SUPERVISOR)', 'warning');", true);
                    return;
                }
            }
        }
        bool boolboChk;
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            boolboChk = comm.IfExistsInTable("AddEmployee ae join  EmpAddress empadd on ae.ADDRESSID=empadd.EmpAddressId", "*", "empadd.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=1 and ae.EmployeeID='" + Request.QueryString["EmployeeIDEdit"].ToString() + "'");
        }
        else
        {
            boolboChk = comm.IfExistsInTable("AddEmployee ae join  EmpAddress empadd on ae.ADDRESSID=empadd.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ae.EmployeeID", "*", "empadd.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=1 and assb.BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'");
        }

        // DataTable dtboChk = new DataTable();
        //dtboChk = comm.GetAllDataFromTable("AddEmployee ae join  EmpAddress empadd on ae.ADDRESSID=empadd.EmpAddressId join", "*", "empadd.MainEmail='" + txtMainEmail.Text + "' and ae.UserType=1");

        if (boolboChk == true)
        {
            int UserType = 0;
            string UserTypeName = "";
            foreach (ListItem item in rdoBtnUserTypeList.Items)
            {
                if (item.Selected == true)
                {
                    UserType = int.Parse(item.Value);
                    UserTypeName = item.Text;
                }
            }
            if (UserType == 2 || UserType == 4)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Business Owner Cannot be LEVEL 1 (EMPLOYEE) or LEVEL 2 (SUPERVISOR)', 'warning');", true);
                return;
            }
        }
        #endregion

        int FinalSaveSavePersonalDetails = 0, FinalAddressSave = 0, FinalSaveRate = 0;
        FinalSaveSavePersonalDetails = SavePersonalDetails();
        if (FinalSaveSavePersonalDetails > 0)
        {
            FinalAddressSave = SaveAddress();
            if (FinalAddressSave > 0)
            {
                if (btnsaveRole.Text == "Save")
                {
                    FinalSaveRate = SavePay();
                }
                //Add employee code is written here.
                #region Add_Employee_Code Add employee code is written here
                if (FinalSaveRate > 0)
                {
                    int UserType = 0;
                    string UserTypeName = "";
                    foreach (ListItem item in rdoBtnUserTypeList.Items)
                    {
                        if (item.Selected == true)
                        {
                            UserType = int.Parse(item.Value);
                            UserTypeName = item.Text;
                        }
                    }

                    #region Inserting_the_role_from_role_permission_dropdown
                    bool check_emp_id_in_user_Role_permission = comm.IfExistsInTable("User_Role_Permission", "Employee_Id", "Employee_Id='" + hfdEmpID.Value + "'");
                    if (check_emp_id_in_user_Role_permission==true)
                    {                    }
                    else
                    {
                        StringBuilder StringBuilder__ = new StringBuilder();
                        foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
                        {
                            string ddl_Role_Permission_Access_Status_ = ((DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status")).SelectedValue;
                            if (ddl_Role_Permission_Access_Status_ == "YES")
                            {
                                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                                StringBuilder__.Append(Role_Permission_Id_.Value + ",");
                            }
                        }
                        string UserType_ = string.Empty;
                        string UserTypeName_ = "";
                        foreach (ListItem item in rdoBtnUserTypeList.Items)
                        {
                            if (item.Selected == true)
                            {
                                UserType_ = item.Value;
                                UserTypeName_ = item.Text;
                            }
                        }
                        comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) Values ('" + StringBuilder__ + "', '" + hfdEmpID.Value + "', '" + UserType_ + "', 'True', '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]) + "')");
                    }
                    #endregion

                    comm.ExecuteQuery("update AddEmployee set UserType=" + UserType + " where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                    if (Request.QueryString["BussEditId"] != null)
                    {
                        string keyWords = hdnproject.Value;
                        string[] arrayval = keyWords.Trim().Split(',');
                        #region to_save_user_as_user_type_N_when_accountant_user_type_is_selected
                        if (ddlusertyperegular.SelectedIndex == 3)
                        {
                            comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'N' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                        }
                        #endregion
                        if (ddlusertyperegular.SelectedIndex == 2)
                        {
                            comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'R' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                        }
                        if (ddlusertyperegular.SelectedIndex == 1)
                        {
                            comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'P' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                        }
                        if (ddlusertyperegular.SelectedIndex == 1)
                        {

                            for (int i = 0; i < arrayval.Length; i++)
                            {
                                if (!string.IsNullOrEmpty(arrayval[i]))
                                {
                                    ape.ProjectID = int.Parse(arrayval[i]);
                                    ape.EmployeeID = int.Parse(hfdEmpID.Value);
                                    ape.BusinessID = int.Parse(Request.QueryString["BussEditId"]);

                                    apc.InsertAssgineProject(ape);
                                }
                            }
                        }
                    }
                    Uae.Email = txtMainEmail.Text.Trim();
                    Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                    Uae.MotherOrgid = int.Parse(hfdMotherId.Value);//int.Parse(Session["MotherOrgid"].ToString());
                    Uae.UserType = UserTypeName;
                    Uae.UserTypeid = UserType;
                    hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                    if (ddlusertyperegular.SelectedIndex == 1)
                    {
                        string keyWords = hdnproject.Value;
                        string[] arrayval = keyWords.Trim().Split(',');

                        comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'P' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");

                        for (int i = 0; i < arrayval.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(arrayval[i]))
                            {

                                ape.ProjectID = int.Parse(arrayval[i]);
                                ape.EmployeeID = int.Parse(hfdEmpID.Value);
                                ape.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);

                                apc.InsertAssgineProject(ape);
                            }
                        }
                    }
                    else if (ddlusertyperegular.SelectedIndex == 3)
                    {
                        comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'N' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                    }
                    else
                    {
                        comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'R' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                    }
                    if (Request.QueryString["BusinessIdFromDefaultPage"] != null && ddlusertyperegular.SelectedIndex == 2)
                    {
                        int BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        string BusinessName = comm.GetNameFromId("AddBusiness", BusinessID);
                        string EmployeeName = comm.GetNameFromId("AddEmployee", int.Parse(hfdEmpID.Value));

                        AddBeCtrl.AssgineBusiness(BusinessID, int.Parse(hfdEmpID.Value), BusinessName, EmployeeName, UserType, int.Parse(hfdMotherId.Value));
                        if (Session["UserTypeid"].ToString() == "2" && ddlusertyperegular.SelectedIndex == 2)
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId", "*", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'R' AND AD.UserType=2", "");
                            DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "'  AND ae.EMPLOYEETYPE= 'R' and ae.UserType=2 and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            // Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                            if (dtmanager.Rows.Count > 0)
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ab.UserTypeId=2  AND ae.EMPLOYEETYPE= 'R' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ")    AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'R')  AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(Session["EmployeeId"].ToString()));
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                        }
                        else if (Session["UserTypeid"].ToString() == "4")
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId join MappingTable mt on mt.EmployeeID=EMPADD.EmpAddressId", " *", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'R' AND AD.UserType=2 and mt.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");

                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            if (dtmanager.Rows.Count > 0)
                            {
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.ManagerID = int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString());
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                            else
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'R' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ")   AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'R')  AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'R' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            if (dtCheckManager.Rows.Count == 1)
                            {
                                Ame.EmpId = int.Parse(hfdEmpID.Value);
                                Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }

                                DataTable dtLeftEmp = new DataTable();
                                dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ")  AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'R')    AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                if (dtLeftEmp.Rows.Count > 0)
                                {
                                    foreach (DataRow r in dtLeftEmp.Rows)
                                    {
                                        Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                        Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                        Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                        Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                        Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                        Ame.BusinessName = BusinessName;
                                        if (UserType != 5)
                                        {
                                            Amc.InsertAssgineManger(Ame);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "' and ae.UserType=2   AND ae.EMPLOYEETYPE= 'R'  and  ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (MangerID.Rows.Count > 0)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(MangerID.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }
                                }
                            }
                        }

                        Uae.UserId = int.Parse(hfdUserId.Value);
                        Uae.UserTypeid = UserType;
                        Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                        Uae.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
                        Uac.Sp_InsertMapping(Uae);

                    }

                    else if (Request.QueryString["BusinessIdFromDefaultPage"] != null && ddlusertyperegular.SelectedIndex == 1)
                    {
                        int BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        string BusinessName = comm.GetNameFromId("AddBusiness", BusinessID);
                        string EmployeeName = comm.GetNameFromId("AddEmployee", int.Parse(hfdEmpID.Value));

                        AddBeCtrl.AssgineBusiness(BusinessID, int.Parse(hfdEmpID.Value), BusinessName, EmployeeName, UserType, int.Parse(hfdMotherId.Value));
                        if (Session["UserTypeid"].ToString() == "2" && ddlusertyperegular.SelectedIndex == 1)
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId", "*", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'P' AND AD.UserType=2", "");
                            DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "'  AND ae.EMPLOYEETYPE= 'P' and ae.UserType=2 and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            // Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                            if (dtmanager.Rows.Count > 0)
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ab.UserTypeId=2  AND ae.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ") AND UserTypeID<>1     AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P')   and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(Session["EmployeeId"].ToString()));
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                        }
                        else if (Session["UserTypeid"].ToString() == "4")
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId join MappingTable mt on mt.EmployeeID=EMPADD.EmpAddressId", " *", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'P' AND AD.UserType=2 and mt.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");

                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            if (dtmanager.Rows.Count > 0)
                            {
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.ManagerID = int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString());
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                            else
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ")   AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P')  AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            if (dtCheckManager.Rows.Count == 1)
                            {
                                Ame.EmpId = int.Parse(hfdEmpID.Value);
                                Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }

                                DataTable dtLeftEmp = new DataTable();
                                dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ") AND EmployeeID  IN(SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P') AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                if (dtLeftEmp.Rows.Count > 0)
                                {
                                    foreach (DataRow r in dtLeftEmp.Rows)
                                    {
                                        Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                        Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                        Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                        Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                        Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                        Ame.BusinessName = BusinessName;
                                        if (UserType != 5)
                                        {
                                            Amc.InsertAssgineManger(Ame);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "' and ae.UserType=2   AND ae.EMPLOYEETYPE= 'P'  and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (MangerID.Rows.Count > 0)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(MangerID.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }
                                }
                            }
                        }

                        Uae.UserId = int.Parse(hfdUserId.Value);
                        Uae.UserTypeid = UserType;
                        Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                        Uae.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
                        Uac.Sp_InsertMapping(Uae);
                    }

                    //Added by Prashant for saving the user type as N when accountant is created
                    #region to_save_user_as_user_type_N_when_accountant_user_type_is_selected
                    else if (Request.QueryString["BusinessIdFromDefaultPage"] != null && ddlusertyperegular.SelectedIndex == 3)
                    {
                        int BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        string BusinessName = comm.GetNameFromId("AddBusiness", BusinessID);
                        string EmployeeName = comm.GetNameFromId("AddEmployee", int.Parse(hfdEmpID.Value));

                        AddBeCtrl.AssgineBusiness(BusinessID, int.Parse(hfdEmpID.Value), BusinessName, EmployeeName, UserType, int.Parse(hfdMotherId.Value));
                        if (Session["UserTypeid"].ToString() == "2" && ddlusertyperegular.SelectedIndex == 1)
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId", "*", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'P' AND AD.UserType=2", "");
                            DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "'  AND ae.EMPLOYEETYPE= 'P' and ae.UserType=2 and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            // Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                            if (dtmanager.Rows.Count > 0)
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ab.UserTypeId=2  AND ae.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ") AND UserTypeID<>1     AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P')   and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(Session["EmployeeId"].ToString()));
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                        }
                        else if (Session["UserTypeid"].ToString() == "4")
                        {
                            DataTable dtmanager = comm.GetAllDataFromTable("AddEmployee AD JOIN EmpAddress EMPADD ON AD.ADDRESSID=EMPADD.EmpAddressId join MappingTable mt on mt.EmployeeID=EMPADD.EmpAddressId", " *", "EMPADD.MainEmail='" + Session["email"].ToString() + "' AND AD.EMPLOYEETYPE= 'P' AND AD.UserType=2 and mt.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");

                            Ame.EmpId = int.Parse(hfdEmpID.Value);
                            if (dtmanager.Rows.Count > 0)
                            {
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.ManagerID = int.Parse(dtmanager.Rows[0]["EmployeeID"].ToString());
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                            else
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }

                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ")   AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P')  AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = BusinessName;
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2  AND AE.EMPLOYEETYPE= 'P' and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                            if (dtCheckManager.Rows.Count == 1)
                            {
                                Ame.EmpId = int.Parse(hfdEmpID.Value);
                                Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }

                                DataTable dtLeftEmp = new DataTable();
                                dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + ") AND EmployeeID  IN(SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P') AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                                if (dtLeftEmp.Rows.Count > 0)
                                {
                                    foreach (DataRow r in dtLeftEmp.Rows)
                                    {
                                        Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                        Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                        Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                        Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                        Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                        Ame.BusinessName = BusinessName;
                                        if (UserType != 5)
                                        {
                                            Amc.InsertAssgineManger(Ame);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                DataTable MangerID = comm.GetAllDataFromTable("AddEmployee Ae join EmpAddress empad on ae.ADDRESSID=empad.EmpAddressId join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "empad.MainEmail='" + Session["email"].ToString() + "' and ae.UserType=2   AND ae.EMPLOYEETYPE= 'P'  and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "");
                                if (MangerID.Rows.Count > 0)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(MangerID.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }
                                }
                            }
                        }

                        Uae.UserId = int.Parse(hfdUserId.Value);
                        Uae.UserTypeid = UserType;
                        Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                        Uae.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
                        Uac.Sp_InsertMapping(Uae);
                    }
                    #endregion

                    else if (Request.QueryString["EmployeeIDEdit"] != null)
                    {
                        Uae.UserId = int.Parse(Session["UserId"].ToString());
                        Uae.UserTypeid = UserType;
                        Uae.EmployeeId = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                        DataTable DTBusinessID = new DataTable();
                        DTBusinessID = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "EmployeeID");
                        if (DTBusinessID.Rows.Count > 0)
                        {
                            Uae.BusinessID = int.Parse(DTBusinessID.Rows[0]["BusinessID"].ToString());
                            Uac.Sp_InsertMapping(Uae);
                            comm.ExecuteQuery("Update AssginBussiness set UserTypeId=" + UserType + " where EmployeeID = " + Request.QueryString["EmployeeIDEdit"].ToString() + "");
                        }
                    }

                    GetList();
                    if (txtCCEmail.Text != string.Empty)
                    {
                        DataTable dtemp = new DataTable();
                        //dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "");
                        dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "ae.EmployeeID desc");
                        if (dtemp.Rows.Count > 0)
                        {
                            string Host = ConfigurationManager.AppSettings["ResetUrl"];
                            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                            if (dtEmailTemp.Rows.Count > 0)
                            {
                                //DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "Password", "EmployeeId = '" + dtemp.Rows[0]["EmployeeID"].ToString() + "' ", "");
                                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                if(txtMainEmail.Text!="")
                                {
                                    //Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                                }
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
                #endregion

                //Update employee code is written here.
                #region Update_Employee_Code Update employee code is written here
                else
                {
                    #region Inserting_the_role_from_role_permission_dropdown
                    bool check_emp_id_in_user_Role_permission = comm.IfExistsInTable("User_Role_Permission", "Employee_Id", "Employee_Id='" + hfdEmpID.Value + "'");
                    if (check_emp_id_in_user_Role_permission == true)
                    {
                        StringBuilder StringBuilder__ = new StringBuilder();
                        foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
                        {
                            string ddl_Role_Permission_Access_Status_ = ((DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status")).SelectedValue;
                            if (ddl_Role_Permission_Access_Status_ == "YES")
                            {
                                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                                StringBuilder__.Append(Role_Permission_Id_.Value + ",");
                            }
                        }
                        string UserType_ = string.Empty;
                        string UserTypeName_ = "";
                        foreach (ListItem item in rdoBtnUserTypeList.Items)
                        {
                            if (item.Selected == true)
                            {
                                UserType_ = item.Value;
                                UserTypeName_ = item.Text;
                            }
                        }
                        comm.ExecuteQuery("update User_Role_Permission set Role_Permission_Id='" + StringBuilder__ + "', BusinessId='"+ int.Parse(Request.QueryString["BussEditId"]) + "'  where Employee_Id ='" + hfdEmpID.Value + "' and UserTypeId = '" + UserType_ + "' and Status='True' ");
                    }
                    else
                    {
                        StringBuilder StringBuilder__ = new StringBuilder();
                        foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
                        {
                            string ddl_Role_Permission_Access_Status_ = ((DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status")).SelectedValue;
                            if (ddl_Role_Permission_Access_Status_ == "YES")
                            {
                                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                                StringBuilder__.Append(Role_Permission_Id_.Value + ",");
                            }
                        }

                        string UserType_ = string.Empty;
                        string UserTypeName_ = "";
                        foreach (ListItem item in rdoBtnUserTypeList.Items)
                        {
                            if (item.Selected == true)
                            {
                                UserType_ = item.Value;
                                UserTypeName_ = item.Text;
                            }
                        }
                        comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) Values ('" + StringBuilder__ + "', '" + hfdEmpID.Value + "', '" + UserType_ + "', 'True', '" + int.Parse(Request.QueryString["BussEditId"]) + "')");
                    }
                    #endregion

                    if (btnsaveRole.Text != "Update")
                    {
                        comm.ExecuteQuery("Delete from AddEmployee where EmployeeID=" + hfdEmpID.Value + "");
                    }
                    else
                    {
                        int UserType = 0;
                        string UserTypeName = "";
                        foreach (ListItem item in rdoBtnUserTypeList.Items)
                        {
                            if (item.Selected == true)
                            {
                                UserType = int.Parse(item.Value);
                                UserTypeName = item.Text;
                            }
                        }
                        comm.ExecuteQuery("update AddEmployee set UserType=" + UserType + " where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                        // comm.ExecuteQuery("update UserAcc set UserTypeid=" + UserType + ", UserType='" + UserTypeName + "' where EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
                        if (Request.QueryString["BussEditId"] != null)
                        {
                            string keyWords = hdnproject.Value;
                            string[] arrayval = keyWords.Trim().Split(',');
                            #region to_save_user_as_user_type_N_when_accountant_user_type_is_selected
                            if (ddlusertyperegular.SelectedIndex == 3)
                            {
                                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'N' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                            }
                            #endregion
                            if (ddlusertyperegular.SelectedIndex == 2)
                            {
                                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'R' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                            }
                            if (ddlusertyperegular.SelectedIndex == 1)
                            {
                                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE= 'P' where EmployeeID=" + int.Parse(hfdEmpID.Value) + "");
                            }
                            if (ddlusertyperegular.SelectedIndex == 1)
                            {
                                for (int i = 0; i < arrayval.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(arrayval[i]))
                                    {

                                        ape.ProjectID = int.Parse(arrayval[i]);
                                        ape.EmployeeID = int.Parse(hfdEmpID.Value);
                                        ape.BusinessID = int.Parse(Request.QueryString["BussEditId"]);

                                        apc.InsertAssgineProject(ape);
                                    }
                                }
                            }
                        }
                        Uae.Email = txtMainEmail.Text.Trim();
                        Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                        Uae.MotherOrgid = int.Parse(hfdMotherId.Value);
                        Uae.UserType = UserTypeName;
                        Uae.UserTypeid = UserType;
                        if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
                        {
                            hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                        }
                        else
                        {
                            if (txtMainEmail.Text.Trim() != string.Empty)
                            {
                                //Commented for userrole Enhancement
                                //DataTable dtUID = new DataTable();
                                //bool EmailChk = comm.IfExistsInTable("AddEmployee AE JOIN EmpAddress EA ON AE.ADDRESSID=EA.EmpAddressId join UserAcc ua on ua.Email='" + txtMainEmail.Text + "'", "*", "EA.MainEmail='" + txtMainEmail.Text + "'");
                                //if (EmailChk == true)
                                //{
                                //    DataTable dtempid = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", "ad.EmployeeID", "ea.MainEmail='" + txtMainEmail.Text + "'", "ad.EmployeeID");
                                //    dtUID = comm.GetAllDataFromTable("UserAcc", "UID,Password", "EmployeeId='" + dtempid.Rows[0]["EmployeeID"].ToString() + "'", "EmployeeId");
                                //    comm.ExecuteQuery("update UserAcc set Email='" + Uae.Email + "',Password='" + dtUID.Rows[0]["Password"].ToString() + "',UID='" + dtUID.Rows[0]["UID"].ToString() + "' where EmployeeId='" + Uae.EmployeeId + "'");

                                //}
                                //else
                                //{
                                //    logclass.UpdateBlankUserAccMail(Uae.EmployeeId, txtMainEmail.Text,int.Parse(Session["MotherOrgid"].ToString()));
                                //    DataTable userid = comm.GetAllDataFromTable("UserAcc", "*", "EmployeeID=" + Uae.EmployeeId + "", "EmployeeID");
                                //    comm.ExecuteQuery("Update MappingTable set USERID=" + userid.Rows[0]["USERID"].ToString() + " where EmployeeID=" + Uae.EmployeeId + "");
                                //}
                                if (txtMainEmail.Text.Trim() != hfdMainEmail.Value)
                                {
                                    DataTable EmailChk = comm.GetAllDataFromTable("UserAcc", "distinct *", "Email='" + txtMainEmail.Text + "'", "");
                                    if (EmailChk.Rows.Count > 0 && EmailChk.Rows[0]["Email"].ToString() != string.Empty)
                                    {
                                        DataTable dtRateID = new DataTable();
                                        int EmployeeIDEdit = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                        int EmailChkEmpID = int.Parse(EmailChk.Rows[0]["EmployeeId"].ToString());
                                        int BussEditId = int.Parse(Request.QueryString["BussEditId"].ToString());
                                        int EmailChkUserId = int.Parse(EmailChk.Rows[0]["UserId"].ToString());
                                        dtRateID = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + EmployeeIDEdit + "", "");
                                        int RateId = int.Parse(dtRateID.Rows[0]["Rateid"].ToString());
                                        comm.ExecuteQuery("update RateDetails set EmployeeId=" + hfdEmpID.Value + " where RateId=" + RateId + "");
                                        comm.ExecuteQuery("update AddEmployee set Rateid=" + RateId + " where EmployeeID=" + hfdEmpID.Value + "");
                                        comm.ExecuteQuery("update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                        comm.ExecuteQuery("update MappingTable set USERID=" + EmailChkUserId + ", EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                        comm.ExecuteQuery("update TimeSheetEntry set EmpId=" + hfdEmpID.Value + "where EmpId=" + EmployeeIDEdit + "");

                                    }
                                    else
                                    {
                                        //hfdEmpID.Value

                                        hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);
                                        DataTable dtRateID = new DataTable();
                                        int EmployeeIDEdit = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                        // int EmailChkEmpID = int.Parse(EmailChk.Rows[0]["EmployeeId"].ToString());
                                        int BussEditId = int.Parse(Request.QueryString["BussEditId"].ToString());
                                        // int EmailChkUserId = int.Parse(EmailChk.Rows[0]["UserId"].ToString());
                                        dtRateID = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + EmployeeIDEdit + "", "");
                                        int RateId = int.Parse(dtRateID.Rows[0]["Rateid"].ToString());
                                        comm.ExecuteQuery("update RateDetails set EmployeeId=" + hfdEmpID.Value + " where RateId=" + RateId + "");
                                        comm.ExecuteQuery("update AddEmployee set Rateid=" + RateId + " where EmployeeID=" + hfdEmpID.Value + "");
                                        comm.ExecuteQuery("update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                        comm.ExecuteQuery("update MappingTable set USERID=" + hfdUserId.Value + ", EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                        comm.ExecuteQuery("update TimeSheetEntry set EmpId=" + hfdEmpID.Value + "where EmpId=" + EmployeeIDEdit + "");

                                    }
                                }
                            }
                            else
                            {
                                if (txtMainEmail.Text.Trim() != hfdMainEmail.Value)
                                {
                                    hfdUserId.Value = Uac.Sp_InsertUserAccFromAddress(Uae);

                                    DataTable dtRateID = new DataTable();
                                    int EmployeeIDEdit = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
                                    // int EmailChkEmpID = int.Parse(EmailChk.Rows[0]["EmployeeId"].ToString());
                                    int BussEditId = int.Parse(Request.QueryString["BussEditId"].ToString());
                                    // int EmailChkUserId = int.Parse(EmailChk.Rows[0]["UserId"].ToString());
                                    dtRateID = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + EmployeeIDEdit + "", "");
                                    int RateId = int.Parse(dtRateID.Rows[0]["Rateid"].ToString());
                                    comm.ExecuteQuery("update RateDetails set EmployeeId=" + hfdEmpID.Value + " where RateId=" + RateId + "");
                                    comm.ExecuteQuery("update AddEmployee set Rateid=" + RateId + " where EmployeeID=" + hfdEmpID.Value + "");
                                    comm.ExecuteQuery("update AssginBussiness set EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                    comm.ExecuteQuery("update MappingTable set USERID=" + hfdUserId.Value + ", EmployeeID=" + hfdEmpID.Value + " where EmployeeID=" + EmployeeIDEdit + " and BusinessID=" + BussEditId + "");
                                }
                            }
                        }

                        if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
                        {
                            int BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                            string BusinessName = comm.GetNameFromId("AddBusiness", BusinessID);
                            string EmployeeName = comm.GetNameFromId("AddEmployee", int.Parse(hfdEmpID.Value));

                            AddBeCtrl.AssgineBusiness(BusinessID, int.Parse(hfdEmpID.Value), BusinessName, EmployeeName, UserType, int.Parse(hfdMotherId.Value));
                            if (Session["UserTypeid"].ToString() == "2")
                            {
                                Ame.EmpId = int.Parse(hfdEmpID.Value);
                                Ame.ManagerID = int.Parse(Session["EmployeeId"].ToString());
                                Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(Session["EmployeeId"].ToString())); ;
                                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                                Ame.EmpName = EmployeeName;
                                Ame.BusinessName = BusinessName;
                                if (UserType != 5)
                                {
                                    Amc.InsertAssgineManger(Ame);
                                }
                            }
                            else
                            {
                                DataTable dtCheckManager = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ae.UserType=2 and ab.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "", "");
                                if (dtCheckManager.Rows.Count == 1)
                                {
                                    Ame.EmpId = int.Parse(hfdEmpID.Value);
                                    Ame.ManagerID = int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString());
                                    Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManager.Rows[0]["EmployeeID"].ToString())); ;
                                    Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                                    Ame.EmpName = EmployeeName;
                                    Ame.BusinessName = BusinessName;
                                    if (UserType != 5)
                                    {
                                        Amc.InsertAssgineManger(Ame);
                                    }
                                }
                            }

                            Uae.UserId = int.Parse(hfdUserId.Value);
                            Uae.UserTypeid = UserType;
                            Uae.EmployeeId = int.Parse(hfdEmpID.Value);
                            Uae.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"]);
                            Uac.Sp_InsertMapping(Uae);
                        }
                        else if (Request.QueryString["EmployeeIDEdit"] != null)
                        {
                            if (txtCCEmail.Text != string.Empty)
                            {
                                DataTable dtemp = new DataTable();
                                //dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "");
                                dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtCCEmail.Text + "'", "ae.EmployeeID desc");
                                if (dtemp.Rows[0]["CCEmail"].ToString() != hfdCCEmail.Value.Trim())
                                {
                                    if (dtemp.Rows.Count > 0)
                                    {
                                        string Host = ConfigurationManager.AppSettings["ResetUrl"];
                                        string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                                        DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
                                        if (dtEmailTemp.Rows.Count > 0)
                                        {
                                            //DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "Password", "EmployeeId = '"+dtemp.Rows[0]["EmployeeID"].ToString()+"' ", "");
                                            DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                            string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                            //Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
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
                            DataTable DTBusinessID = new DataTable();
                            DTBusinessID = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "EmployeeID");
                            if (DTBusinessID.Rows.Count > 0)
                            {
                                Uae.BusinessID = int.Parse(DTBusinessID.Rows[0]["BusinessID"].ToString());
                                Uac.Sp_InsertMapping(Uae);
                                comm.ExecuteQuery("Update AssginBussiness set UserTypeId=" + UserType + " where EmployeeID = " + Request.QueryString["EmployeeIDEdit"].ToString() + "");
                            }
                            if (UserType == 2   && ddlusertyperegular.SelectedIndex == 1)
                            {
                                DataTable dtCheckManagerOnUpdate = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ab.UserTypeId=2   AND ae.EMPLOYEETYPE= 'P' and  ab.BusinessID=" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "", "");
                                if (dtCheckManagerOnUpdate.Rows.Count == 1)
                                {
                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "and ManagerID=" + Request.QueryString["EmployeeIDEdit"].ToString() + ")   AND EmployeeID  IN (SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE= 'P')   AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManagerOnUpdate.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManagerOnUpdate.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BussEditId"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BussEditId"].ToString()));
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                            if (UserType == 2  && ddlusertyperegular.SelectedIndex == 2)
                            {
                                DataTable dtCheckManagerOnUpdate = comm.GetAllDataFromTable("AddEmployee ae join AssginBussiness ab on ae.EmployeeID=ab.EmployeeID", "*", "ab.UserTypeId=2  AND ae.EMPLOYEETYPE= 'R'  and ab.BusinessID=" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "", "");
                                if (dtCheckManagerOnUpdate.Rows.Count == 1)
                                {
                                    DataTable dtLeftEmp = new DataTable();
                                    dtLeftEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID  IN(SELECT ae.EmployeeID FROM AddEmployee ae  WHERE AE.EMPLOYEETYPE = 'R')  AND EmployeeID NOT IN(SELECT EmpId FROM AssgineManager WHERE BusinessID=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "and ManagerID=" + Request.QueryString["EmployeeIDEdit"].ToString() + ") AND UserTypeID<>1  and BusinessID=" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "", "");
                                    if (dtLeftEmp.Rows.Count > 0)
                                    {
                                        foreach (DataRow r in dtLeftEmp.Rows)
                                        {
                                            Ame.EmpId = int.Parse(r["EmployeeID"].ToString());
                                            Ame.ManagerID = int.Parse(dtCheckManagerOnUpdate.Rows[0]["EmployeeID"].ToString());
                                            Ame.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(dtCheckManagerOnUpdate.Rows[0]["EmployeeID"].ToString())); ;
                                            Ame.BusinessID = int.Parse(Request.QueryString["BussEditId"].ToString());
                                            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(r["EmployeeID"].ToString())); ; ;
                                            Ame.BusinessName = comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BussEditId"].ToString()));
                                            if (UserType != 5)
                                            {
                                                Amc.InsertAssgineManger(Ame);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        GetList();
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All Details Saved', 'success');", true);
                    }
                }
                #endregion
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
        Response.Redirect("Default.aspx");
    }

    protected void btnrateHistoryChangeRate_Click(object sender, EventArgs e)
    {
        if (gvRateHistory.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'First Add Employee. After That Change Rate', 'warning');", true);
            return;
        }

        //DateTime textdate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
        //if (textdate < DateTime.Now.Date)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Back Date Is Not Allowed', 'warning');", true);
        //    return;
        //}

        if (float.Parse(txtrateHistoryBillRate.Text) > 10000000)//10000
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Rate Must Be Less Than 10000000', 'warning');", true);
            return;
        }
        if (btnrateHistoryChangeRate.Text != "Update Rate")
        {
            if (txtrateHistoryBillRate.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Rate', 'warning');", true);
                return;
            }

            var Check_If_Rate_EndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=1 order by RateId desc");
            if (Check_If_Rate_EndDate.Rows.Count > 0)
            {
                if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_If_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than current rate start date', 'warning');", true);
                    return;
                }

                var chktotalentryofrate = comm.ExecuteQueryDataTable("select Is_Having_EndDate,EndDate from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
                if (chktotalentryofrate.Rows.Count == 1 && chktotalentryofrate.Rows[0]["Is_Having_EndDate"].ToString() == "True")
                {
                    if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(chktotalentryofrate.Rows[0]["EndDate"].ToString()))
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than current end date as it has the end date', 'warning');", true);
                        return;
                    }

                    comm.ExecuteQuery("update RateDetails set Is_Having_EndDate=0 where EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
                }
            }
            else
            {
                var Check_Rate_EndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                    "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=0 order by RateId desc");
                if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than current rate start date', 'warning');", true);
                    return;
                }
            }

            RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
            RateEntity.Rate = float.Parse(txtrateHistoryBillRate.Text);
            RateEntity.RateType = ddlrateHistoryBillType.SelectedItem.Text;
            RateEntity.StartDate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
            RateEntity.EndDate = System.DateTime.Now.Date;
            RateEntity.JobTitle = ddlJobTitle.SelectedValue;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            RateEntity.effectivedate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
            RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));

            GetRateInformation(hfdEmpID.Value);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);
            //Label_PopUP_Message.Text = "Rate Details Added successfully";
            //Modelconfirmmessagepopup.Show();
            GetRateInformation(hfdEmpID.Value);

            txtrateHistoryEffectiveDate.Text = "";
        }
        else
        {
            if (txtrateHistoryBillRate.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Rate', 'warning');", true);
                return;
            }

            var Check_If_Rate_EndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=1 order by RateId desc");
            if (Check_If_Rate_EndDate.Rows.Count > 0)
            {
                var chktotalentryofratedata = comm.ExecuteQueryDataTable("select RateId from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + "");

                if (chktotalentryofratedata.Rows.Count == 1)
                {
                    if (TextBox_EndDate.Text != "")
                    {
                        if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) < DateTime.Parse(Check_If_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than previous rate end date', 'warning');", true);
                            return;
                        }
                    }
                }
                else
                {
                    if (TextBox_EndDate.Text != "")
                    {
                        if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) < DateTime.Parse(Check_If_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than previous rate end date', 'warning');", true);
                            return;
                        }
                    }
                    else
                    {
                        if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_If_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than previous rate end date', 'warning');", true);
                            return;
                        }
                    }
                }
            }
            else
            {
                var Check_Rate_EndDate = comm.ExecuteQueryDataTable("select top 2 RateId, effectivedate, Is_Having_EndDate " +
                    "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " and Is_Having_EndDate=0 order by RateId desc");

                if (Check_Rate_EndDate.Rows.Count==1)
                {
                    if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_Rate_EndDate.Rows[0]["effectivedate"].ToString()))
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than previous rate start date', 'warning');", true);
                        return;
                    }
                }
                else if (Check_Rate_EndDate.Rows.Count > 1)
                {
                    if (DateTime.Parse(txtrateHistoryEffectiveDate.Text) <= DateTime.Parse(Check_Rate_EndDate.Rows[1]["effectivedate"].ToString()))
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start date should be greater than previous rate start date', 'warning');", true);
                        return;
                    }
                }
            }

            comm.ExecuteQuery("update RateDetails set Rate =" + float.Parse(txtrateHistoryBillRate.Text) + ", RateType='" + ddlrateHistoryBillType.SelectedValue + "', StartDate='" + txtrateHistoryEffectiveDate.Text + "', effectivedate='" + txtrateHistoryEffectiveDate.Text + "' where RateId=" + hfdEditRateId.Value + "");
            bool chk = comm.IfExistsInTable("RateDetails", "*", "EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
            var chktotalentryofrate = comm.ExecuteQueryDataTable("select RateId from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
            if (chk == true && chktotalentryofrate.Rows.Count > 1)
            {
                DateTime previousdat = DateTime.Parse(txtrateHistoryEffectiveDate.Text).AddDays(-1);
                DataTable currentrateid = comm.GetAllDataFromTable("RateDetails", "*", "EmployeeId=" + int.Parse(hfdEmpID.Value) + "", "RateId");
                int colcount = currentrateid.Rows.Count;
                string currid = currentrateid.Rows[colcount - 2]["RateId"].ToString();
                comm.ExecuteQuery("update RateDetails set EndDate='" + previousdat.Date.ToString("yyyy-MM-dd") + "' where RateId=" + currid + " ");
            }

            //comm.ExecuteQuery("update RateDetails set Rate =" + txtrateHistoryBillRate.Text + ", RateType='" + ddlrateHistoryBillType.SelectedValue + "' where RateId=" + hfdEditRateId.Value + "");  effectivedate='" + txteffectivedate.Text
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Previous rate change was made on " + HiddenField_PreviousDate.Value + ", </br> The new rate will be with effect from " + txtrateHistoryEffectiveDate.Text + " ', 'success');", true);
            //Label_PopUP_Message.Text = "Rate Details Updated successfully";
            //Label_PopUP_Message.Text = "Previous rate change was made on " + HiddenField_PreviousDate.Value + ", </br> The new rate will be changed as of " + txtrateHistoryEffectiveDate.Text;
            //Modelconfirmmessagepopup.Show();
            btnSavePay.Text = "Save";
            hfdEditRateId.Value = "";
            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
            }
            else
            {
                GetRateInformation(hfdEmpID.Value);
            }

            btnSavePay.Enabled = true;
            btnrateHistoryChangeRate.Text = "Change Rate";
            txtrateHistoryEffectiveDate.Text = "";
            txtrateHistoryEffectiveDate_adding_newrate.Text = "";

            //Kumar to edit rate if it has end date
            var Check_If_RateEndDate = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hfdEmpID.Value) + " order by RateId desc");
            if (Check_If_RateEndDate.Rows.Count > 0 && Check_If_RateEndDate.Rows[0]["Is_Having_EndDate"].ToString() == "True" && TextBox_EndDate.Text != "")
            {
                comm.ExecuteQuery("update RateDetails set EndDate='" + DateTime.Parse(TextBox_EndDate.Text).ToString("yyyy-MM-dd") + "' where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and " +
                    "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and Is_Having_EndDate=1)");
            }

            if (Check_If_RateEndDate.Rows.Count > 0 && Check_If_RateEndDate.Rows[0]["Is_Having_EndDate"].ToString() == "True" && TextBox_EndDate.Text == "")
            {
                comm.ExecuteQuery("update RateDetails set EndDate='" + System.DateTime.Now.Date + "' where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and " +
                    "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and Is_Having_EndDate=1)");

                comm.ExecuteQuery("update RateDetails set Is_Having_EndDate=0 where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and " +
                    "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + int.Parse(hfdEmpID.Value) + "' and Is_Having_EndDate=1)");
            }
        }
        txtEffectiveDateRateHistory.Text = "";
        ddlrateHistoryBillType.SelectedValue = "";
        txtrateHistoryBillRate.Text = "";

        //Response.Redirect("Home_Page.aspx");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetAutoCompleteDataCity(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            //HttpContext.Current.Request.QueryString["BusinessIdFromDefaultPage"];//GetQueryStringData();
            //For User Role 25_07_2020
            //string querytxt = "select ADDEMP.EmployeeID,ea.MainEmail from AddEmployee ADDEMP JOIN EmpAddress EA ON ADDEMP.ADDRESSID=EA.EmpAddressId JOIN AssginBussiness ASSB ON ASSB.EmployeeID=ADDEMP.EmployeeID WHERE ASSB.BusinessID="+ bid + "AND EA.MainEmail LIKE '%'+@SearchText+'%'";

            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "Select distinct CityName from CityMaster where CityName LIKE '%'+@SearchText+'%'";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["CityName"].ToString());
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


    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetAutoCompleteDataProvince(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            //HttpContext.Current.Request.QueryString["BusinessIdFromDefaultPage"];//GetQueryStringData();
            //For User Role 25_07_2020
            //string querytxt = "select ADDEMP.EmployeeID,ea.MainEmail from AddEmployee ADDEMP JOIN EmpAddress EA ON ADDEMP.ADDRESSID=EA.EmpAddressId JOIN AssginBussiness ASSB ON ASSB.EmployeeID=ADDEMP.EmployeeID WHERE ASSB.BusinessID="+ bid + "AND EA.MainEmail LIKE '%'+@SearchText+'%'";

            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "Select distinct ProvinceName from ProvinceMaster where ProvinceName LIKE '%'+@SearchText+'%'";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["ProvinceName"].ToString());
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

    [WebMethod]
    public static List<string> GetAutoCompleteDataOccupation(string Occupation)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("select distinct Occupation,Occupation from AddEmployee where Occupation LIKE '%'+@SearchText+'%'", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", Occupation);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}/{1}", dr["Occupation"], dr["Occupation"]));
                }
                return result;
            }
        }
    }

    protected void btnUpdatePersonalDetails_Click(object sender, EventArgs e)
    {
        btnsaveRole_Click(sender, e);
    }

    protected void btnUpdateAddress_Click(object sender, EventArgs e)
    {
        if (txtConLoginEmail.Text != txtMainEmail.Text)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm login email does not match with login email', 'warning');", true);
            return;
        }
        btnsaveRole_Click(sender, e);
    }

    protected void lnkView_Click(object sender, EventArgs e)
    {
        if (txtSocialNumer.Text == "" && txtdob.Text == "")
        {
            txtSocialNumer.Text = comm.Decrypt(hfdSocialEncrpty.Value);
            string dbo = comm.Decrypt(hfddboEncrpty.Value);
            if (dbo != "")
            {
                txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
            }
            eye.Attributes.Add("class", "fa fa-eye-slash");
        }
        else
        {
            txtdob.Text = string.Empty;
            txtSocialNumer.Text = string.Empty;
            eye.Attributes.Add("class", "fa fa-eye");
        }
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        if (Button1.Text == "Save")
        {
            ModalPopupEmailConfirm.Show();
        }
        else
        {
            btnsaveRole_Click(sender, e);
        }
    }

    protected void btnconfirm_Click(object sender, EventArgs e)
    {
        if (txtconfirmMainEmail.Text == string.Empty)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Watch Out!', 'Confirm Login Email Not be Blank', 'error');", true);
            return;
        }

        if (txtMainEmail.Text.Trim() == txtconfirmMainEmail.Text.Trim())
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Confirm Login Email Match', 'success');", true);
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Oops!', 'Login Email Not Match', 'error');", true);
            txtconfirmMainEmail.Text = "";
        }
    }

    protected void StartDate_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("StartDate");
        divAddres.Attributes.Add("style", "display:block;");
        if (StartDate.Text != " ADD NEW ADDRESS")
        {
            ClearAddress();
            String[] Split = StartDate.Text.Split(',');
            txtPostalCode.Text = txtSearchPostalCode.Text;
            txtPostalCode.ReadOnly = true;
            txtPostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.ReadOnly = true;
            txtStreetName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            txtStreetType1.Text = Split[0].ToString().ToUpper();
            txtCity.Text = Split[1].ToString().ToUpper();
            txtCity.ReadOnly = true;
            txtProvince.Text = Split[2].ToString().ToUpper();
            txtProvince.ReadOnly = true;
        }
        else
        {
            txtSuiteNumber.Text = "";
            txtStreetNo.Text = "";
            DropDownList1.SelectedValue = "";

            txtSuiteNumber.Text = "";
            txtPostalCode.Text = "";
            txtPostalCode.ReadOnly = false;
            txtPostalCode.Enabled = true;
            txtStreetName.Text = "";
            txtStreetName.ReadOnly = false;
            txtStreetName.Enabled = true;
            txtStreetType.Text = "";
            txtStreetType1.Text = "";
            txtStreetType.ReadOnly = false;
            txtStreetType.Enabled = true;
            txtCity.Text = "";
            txtProvince.Text = "";
            txtCity.ReadOnly = false;
            txtCity.Enabled = true;
            txtProvince.ReadOnly = false;
            txtProvince.Enabled = true;
            txtPostalCode.Text = txtSearchPostalCode.Text;
            //ClearAddressContactDetails();
        }
    }

    protected void btnupdateRole_Click(object sender, EventArgs e)
    {
        btnsaveRole_Click(sender, e);
    }

    protected void txtSearchPostalCode_TextChanged(object sender, EventArgs e)
    {
        int searchtextlenghth = int.Parse(txtSearchPostalCode.Text.Length.ToString());

        if (searchtextlenghth > 3)
        {
            string serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
            string padleft = serachpostcode.Insert(3, " ");
            txtSearchPostalCode.Text = padleft;
        }
    }

    protected void ddlCopyLoginEmail_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCopyLoginEmail.SelectedValue == "Yes")
            txtCCEmail.Text = txtMainEmail.Text;
        else
            txtCCEmail.Text = "";
    }

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    protected void lnkSearchUser_Click(object sender, EventArgs e)
    {
        DataTable dtbeid = new DataTable();
        dtbeid = comm.GetAllDataFromTable("AddEmployee ADDEMP JOIN EmpAddress EA ON ADDEMP.ADDRESSID=EA.EmpAddressId", "ADDEMP.EmployeeID,ea.MainEmail", "EA.MainEmail='" + txtsearch.Text + "'", "");
        GetPersonalDetailsForEdit(int.Parse(dtbeid.Rows[0]["EmployeeID"].ToString()));
        GetRateInformation(dtbeid.Rows[0]["EmployeeID"].ToString());
        divAddres.Attributes.Add("style", "display:block;");
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetCompletionList(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            string bid = name;
            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "Select distinct Email from UserAcc where email LIKE '%'+@SearchText+'%' and UserType is null";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["Email"].ToString());
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

    protected void ddlusertyperegular_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlusertyperegular.SelectedIndex == 1)
        {
            ddlproject.Visible = true;

            //comm.FillDropDownList(ddlemp, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserType not in (2,1) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "", "ab.EmployeeID");
            DataTable dt = new DataTable();
            ddlproject.Visible = true;
            ddlproject.Items.Clear();
            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                dt = comm.GetAllDataFromTable("AddProject ab  left join ASSIGNPROJECT asb on ab.ProjectID=asb.PROJECTID and  AB.BUSINESSID =ASB.BUSINESSID ", "distinct ab.ProjectID,ab.ProjectName", "Status ='A'   and asb.BUSINESSID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "ab.ProjectID");

            }
            else
            {
                dt = comm.GetAllDataFromTable("AddProject ab  left join ASSIGNPROJECT asb on ab.ProjectID=asb.PROJECTID and  AB.BUSINESSID =ASB.BUSINESSID ", "distinct ab.ProjectID,ab.ProjectName", "Status ='A'   and ab.BUSINESSID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "", "ab.ProjectID");

            }
            ddlproject.DataSource = dt;
            ddlproject.DataTextField = "ProjectName";
            ddlproject.DataValueField = "ProjectID";
            ddlproject.DataBind();
            string empid;
            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                empid = Request.QueryString["EmployeeIDEdit"].ToString();
            }
            else
            {
                empid = hfdEmpID.Value;
            }
            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                foreach (ListItem item in ddlproject.Items)
                {
                    bool check = comm.IfExistsInTable("ASSIGNPROJECT", "*", "BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and PROJECTID=" + item.Value + " and EMPLOYEEID=" + empid + "");
                    if (check == true)
                    {
                        item.Selected = true;
                    }
                }
            }
        }
        if (ddlusertyperegular.SelectedIndex == 2 || ddlusertyperegular.SelectedIndex == 0 || ddlusertyperegular.SelectedIndex == 3)
        {
            ddlproject.Visible = false;
        }
    }

    protected void ddlproject_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Request.QueryString["EmployeeIDEdit"] != null)
        {
            int a = 0;
            String projectnm = string.Empty;
            foreach (ListItem itm in ddlproject.Items)
            {
                if (itm.Selected == false)
                {
                    string proejctid = itm.Value;
                    projectnm = itm.Text;
                    projectnm = itm.Text;
                    bool chk = comm.IfExistsInTable("ASSIGNPROJECT", "*", "EmployeeId=" + hfdEmpID.Value + " and BusinessID=" + Request.QueryString["BussEditId"].ToString() + " and  PROJECTID=" + proejctid + "");
                    if (chk == true)
                    {
                        a = comm.ExecuteQuery("delete from ASSIGNPROJECT where PROJECTID=" + proejctid + " and EmployeeID=" + hfdEmpID.Value + " and BusinessID=" + Request.QueryString["BussEditId"].ToString() + "");
                    }
                }
            }
            if (a > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Employee Removed from Project " + projectnm + "  Successfully', 'success');", true);
            }
        }
    }

    public void GetUserPermissionRoleList()
    {
        comm.FillGridView(GridView_User_Role_Permission, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status", "", "Role_Permission_Id asc");
    }

    protected void GridView_User_Role_Permission_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        
    }

    protected void GridView_User_Role_Permission_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }

    protected void GridView_User_Role_Permission_PreRender(object sender, EventArgs e)
    {
        if (GridView_User_Role_Permission.Rows.Count > 0)
        {
            
        }
    }

    public void GetPersonalDetails_While_Editing_Employee(int EmployeeID)
    {
        DataTable Dt_Check_Role_Permission = new DataTable();
        Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["EmployeeIDEdit"].ToString() + "' ", "");
        //Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
        if (Dt_Check_Role_Permission.Rows.Count > 0)
        {
            foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
            {
                string string_Check_Role_Permission = Dt_Check_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
                string[] splittedArray = string_Check_Role_Permission.Split(',');
                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                DropDownList ddl_Role_Permission_Access_Status_ = (DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status");
                foreach (string s in splittedArray)
                {
                    if (Role_Permission_Id_.Value == s)
                    {
                        ddl_Role_Permission_Access_Status_.SelectedValue = "YES";
                    }
                }
            }
        }
        else
        {
            Div_Hide_PERMISSION_ROLE_SELECTION.Visible = false;
        }
    }

    protected void rdoBtnUserTypeList_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdoBtnUserTypeList.SelectedValue.ToString() == "12")
        {
            int UserType = int.Parse(rdoBtnUserTypeList.SelectedValue);
            string UserTypeName = AnyminuteEmployeeEditor.StoredName(rdoBtnUserTypeList.SelectedItem);
            ddlusertyperegular.ClearSelection();
            ddlusertyperegular.Items.FindByValue("N").Selected = true;
            ddlusertyperegular_SelectedIndexChanged(sender, e);
            ddlusertyperegular.Enabled = false;
        }
        //commented because creating issue while editing employee
        //else
        //{
        //    ddlusertyperegular.Enabled = true;
        //    ddlusertyperegular.ClearSelection();
        //    ddlusertyperegular.Items.FindByValue("NO").Selected = true;
        //    ddlusertyperegular_SelectedIndexChanged(sender, e); //It is to reset the reporting type dropdown
        //}
    }

    protected void btn_Okay_Click(object sender, EventArgs e)
    {
        //Response.Redirect("Home_Page.aspx");
    }

    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Add_EmployeeV2.aspx");
    }
}