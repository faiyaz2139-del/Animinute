using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AddEditRole_Add_Employee : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    DataTable dtSelectedBuss = new DataTable();
    DataTable dtSelectProject = new DataTable();
    DataTable dtSelectRate = new DataTable();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddProjectController apc = new AddProjectController();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    RateDetailsController RateCtrl = new RateDetailsController();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();
    AssignProjectEntity ape = new AssignProjectEntity();
    Cls_UserType ut = new Cls_UserType();

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    private SqlConnection con;
    string MainEmailGVRole;
    string strUserTypeID;
    private void EmployeeRepairMessage(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "EmployeeRepair",
            "alert('" + System.Web.HttpUtility.JavaScriptStringEncode(message) + "');", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {


        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        // Clear the cookie with the specified name
        HttpCookie CookieName = Request.Cookies["tabid"];
        if (CookieName != null)
        {
            var cookvaluename = CookieName.Value;
            //if (cookvaluename == "#permission")
            //{
            //    CookieName.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(CookieName);
            //}
            if (cookvaluename == "#ratedetils")
            {
                CookieName.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(CookieName);
            }
            if (cookvaluename == "#project")
            {
                CookieName.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(CookieName);
            }
        }

        if (!IsPostBack)
        {
            #region show message if maximum limit is reached. This region code is same as of in Save button.
            
            #endregion

            #region show message if maximum limit is reached. This region code is same as of in Save button.
            if (Session["UserTypeid"].ToString() == "1")
            {
                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_subscriber.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnFinalSave.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnFinalSave.Visible = false;
                            DisableAllControlsInPage(Page, false);
                            return;
                        }
                    }
                    if (check_old_subscriber.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnFinalSave.Visible = false;
                        DisableAllControlsInPage(Page, false);
                        return;
                    }
                }
                if (check_reseller_or_subscriber_email)
                {
                    var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_reseller.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnFinalSave.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }

                            DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory where IsActive=0 and " +
                                "EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and " +
                                "MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_active_status.Rows.Count > 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnFinalSave.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnFinalSave.Visible = false;
                            DisableAllControlsInPage(Page, false);
                            return;
                        }
                    }
                    if (check_old_reseller.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnFinalSave.Visible = false;
                        DisableAllControlsInPage(Page, false);
                        return;
                    }
                }
            }
            #endregion

            Div_Personaldetails_Info.Attributes.Add("style", "display:none;");
            Div_Personaldetails_Nav_Button.Attributes.Add("style", "display:none;");

            ViewState["BGridEdit"] = false;
            ViewState["PGvProjectEdit"] = false;
            ViewState["QueryStringRole"] = "";
            ViewState["RoleId_PopAdd"] = "";
            bool Check_Subscriber_Active_Status = comm.IfExistsInTable("AddEmployee", "IsActive", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and IsActive='NO        ' and UserType='1' ");
            if (Check_Subscriber_Active_Status)
            {
                DisableAllControlsInPage(Page, false);
                btnFinalSave.Visible = false;
                return;
            }
            InitialSelectBuss();
            ViewState["Event"] = false;
            if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
            {
                ViewState["bid"] = Request.QueryString["BusinessID"].ToString();
            }
            else
            {
                var dt = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Session["EmployeeId"].ToString() + " and BusinessID is not null and BusinessID<>''", "BusinessID");
                if (dt.Rows.Count > 0)
                {
                    ViewState["bid"] = dt.Rows[0][0].ToString();
                }
                else
                {
                    ViewState["bid"] = 0;
                }
            }
            var empid = Session["EmployeeId"].ToString();
            // comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");
            // comm.FillDropDownList(ddlBusiness, "AssginBussiness", "BusinessID", "BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");
            Bindbusiness();
            BindAllUserType();
            if (Request.QueryString["EmployeeIDEdit"] != null && Request.QueryString["BussEditId"] != null)
            {
                #region amit code for edit employee
                //ddl_Business.SelectedValue = Request.QueryString["BussEditId"].ToString();
                //ddl_Business_SelectedIndexChanged(sender, e);
                //ddl_Employee.SelectedValue = Request.QueryString["EmployeeIDEdit"].ToString();
                //ddl_Employee_SelectedIndexChanged(sender, e);

                //ddl_Business.Enabled = false;
                //ddl_Employee.Enabled = false;
                #endregion


                ddl_Business.SelectedValue = Request.QueryString["BussEditId"].ToString();
                ddl_Employee.SelectedValue = Request.QueryString["EmployeeIDEdit"].ToString();
                DataTable Business_Owner_Pop_up_Employee_List_ = new DataTable();
                var empid_edit = Session["EmployeeId"].ToString();
                if (Request.QueryString["BussEditId"] != null)
                {
                    Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(Request.QueryString["BussEditId"].ToString()), int.Parse(empid_edit));
                }

                if (Business_Owner_Pop_up_Employee_List_.Rows.Count > 0)
                {
                    // DataRow[] drselect = Business_Owner_Pop_up_Employee_List_.Select("[IsActive]='YES'");


                    DataTable dtselect = Business_Owner_Pop_up_Employee_List_;
                    if (dtselect.Rows.Count > 0)
                    {
                        // DataTable dtselect = drselect.CopyToDataTable();
                        //ddl_Employee.Items.Add(new ListItem("Select Employee", ""));
                        ddl_Employee.Items.Add(new ListItem("Please Select", ""));
                        ddl_Employee.DataSource = dtselect;
                        ddl_Employee.DataValueField = dtselect.Columns[0].ToString();
                        ddl_Employee.DataTextField = dtselect.Columns[3].ToString();
                        ddl_Employee.DataBind();
                    }
                }

                Div_Personaldetails_Info.Attributes.Add("style", "display:block;");
                Div_Personaldetails_Nav_Button.Attributes.Add("style", "display:block;");

                GetChangeRateRole(int.Parse(ddl_Employee.SelectedValue));
                GetPersonalDetailsForEdit(int.Parse(ddl_Employee.SelectedValue));
                //GetAssgineBuss(txtMainEmail.Text, Session["UserTypeid"].ToString());
                //GetAssgineProject(txtMainEmail.Text, Session["UserTypeid"].ToString());
                //GetRateDetails(txtMainEmail.Text, Session["UserTypeid"].ToString());
                DataTable dt_GetEmailId = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = " + int.Parse(ddl_Employee.SelectedValue) + ")", "");
                GetAssgineBuss(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());
                MainEmailGVRole = dt_GetEmailId.Rows[0]["MainEmail"].ToString() ;
                strUserTypeID = Session["UserTypeid"].ToString();
                

                GetAssgineProject(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());
                GetRateDetails(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());

                var profile = comm.GetAllDataFromTable("UserAcc", "Profile_Image", "email='" + txtMainEmail.Text + "'", "");
                if (profile.Rows[0][0].ToString() != string.Empty)
                {
                    previewImage.ImageUrl = "~/Uploads/" + profile.Rows[0][0].ToString();
                    ViewState["profile"] = profile.Rows[0][0].ToString();
                }
                else
                {
                    //previewImage.ImageUrl = "~/Uploads/profile.jpg";
                }
                //txtHiredate.ReadOnly = true;

                #region disable the edit address if edit address permission is not given
                if (Session["UserTypeid"].ToString() != "1")
                {
                    string[] role;
                    string Str_Check_Edit_Address_Permission = string.Empty;
                    DataTable Dt_Check_User_Role_In_Particular_Business = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BussEditId"].ToString() + "' ", "UserTypeId");
                    DataTable Dt_Check_Edit_Address_Permission = comm.GetAllDataFromTable("User_Role_Permission with (Nolock)", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_Particular_Business.Rows[0]["EmployeeId"].ToString() + "'", "");

                    if (Dt_Check_Edit_Address_Permission.Rows.Count > 0)
                    {
                        if (Dt_Check_Edit_Address_Permission.Rows[0]["Role_Permission_Id"].ToString().Length > 1)
                        {
                            Str_Check_Edit_Address_Permission = Dt_Check_Edit_Address_Permission.Rows[0]["Role_Permission_Id"].ToString().Remove(Dt_Check_Edit_Address_Permission.Rows[0]["Role_Permission_Id"].ToString().Length - 1);
                        }

                        if (Dt_Check_Edit_Address_Permission.Rows[0]["Role_Permission_Id"].ToString().Length > 1)
                        {
                            string role_check_id_for_edit_address = string.Empty;
                            if (Dt_Check_Edit_Address_Permission.Rows.Count > 0)
                            {
                                role = Str_Check_Edit_Address_Permission.Split(',');
                                foreach (string role_check_for_edit_address in role)
                                {
                                    if (role_check_for_edit_address == "11" || role_check_for_edit_address == "22")
                                    {
                                        txtSearchPostalCode.ReadOnly = true;
                                        txtStreetNo.ReadOnly = true;
                                        txtStreetName.ReadOnly = true;
                                        txtStreetType.ReadOnly = true;
                                        txtSuiteNumber.ReadOnly = true;
                                        DropDownList2.Enabled = false;
                                        txtCity.ReadOnly = true;
                                        txtProvince.ReadOnly = true;
                                        txtPostalCode.ReadOnly = true;
                                        role_check_id_for_edit_address = "31";
                                    }
                                    else
                                    {
                                        role_check_id_for_edit_address = role_check_for_edit_address;
                                    }
                                }
                                if (role_check_id_for_edit_address != "31")
                                {
                                    txtSearchPostalCode.ReadOnly = false;
                                    txtStreetNo.ReadOnly = false;
                                    txtStreetName.ReadOnly = false;
                                    txtStreetType.ReadOnly = false;
                                    txtSuiteNumber.ReadOnly = false;
                                    DropDownList2.Enabled = true;
                                    txtCity.ReadOnly = false;
                                    txtProvince.ReadOnly = false;
                                    txtPostalCode.ReadOnly = false;
                                }
                            }
                        }
                        if (Dt_Check_Edit_Address_Permission.Rows[0]["Role_Permission_Id"].ToString() == "")
                        {
                            txtSearchPostalCode.ReadOnly = true;
                            txtStreetNo.ReadOnly = true;
                            txtStreetName.ReadOnly = true;
                            txtStreetType.ReadOnly = true;
                            txtSuiteNumber.ReadOnly = true;
                            DropDownList2.Enabled = false;
                            txtCity.ReadOnly = true;
                            txtProvince.ReadOnly = true;
                            txtPostalCode.ReadOnly = true;
                        }
                    }
                }
                #endregion
                ddl_Business.Enabled = false;
                ddl_Employee.Enabled = false;
                // Session["BusinessId"] = null;
                // Session["EmployeeId"] = null;

            }
        }
        if (ViewState["profile"] != null && !string.IsNullOrEmpty(ViewState["profile"].ToString()))
        {
            previewImage.ImageUrl = "~/Uploads/" + ViewState["profile"].ToString();
        }
        txtHiredate.Attributes.Add("max", System.DateTime.Now.Date.ToString("yyyy-MM-dd"));
        HttpCookie aCookie = Request.Cookies["tabid"];
        if (aCookie != null)
        {
            var cookvalue = aCookie.Value;
            if (cookvalue == "#home")
            {
                home.Attributes.Add("class", "tab-pane fade in active show");
                //permission.Attributes.Add("class", "tab-pane fade");
                ratedetils.Attributes.Add("class", "tab-pane fade");
                project.Attributes.Add("class", "tab-pane fade");
                // $("#home").attr("class", "tab-pane fade in active show");
            }
            //else if (cookvalue == "#permission")
            //{
            //    permission.Attributes.Add("class", "tab-pane fade in active show");
            //    project.Attributes.Add("class", "tab-pane fade in ");
            //    home.Attributes.Add("class", "tab-pane fade in");
            //    ratedetils.Attributes.Add("class", "tab-pane fade in");
            //    LiRateDetails.Attributes.Add("class", "");
            //    Lirolemid.Attributes.Add("class", "active");
            //    Lihomemid.Attributes.Add("class", "");
            //    LiProject.Attributes.Add("class", "");
            //}
            else if (cookvalue == "#ratedetils")
            {
                //permission.Attributes.Add("class", "tab-pane fade in");
                project.Attributes.Add("class", "tab-pane fade in");
                home.Attributes.Add("class", "tab-pane fade in");
                ratedetils.Attributes.Add("class", "tab-pane fade in active show");
                LiRateDetails.Attributes.Add("class", "");
                //Lirolemid.Attributes.Add("class", "");
                Lihomemid.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "");
            }
            else if (cookvalue == "#project")
            {
                project.Attributes.Add("class", "tab-pane fade in active show");
                //permission.Attributes.Add("class", "tab-pane fade in");
                home.Attributes.Add("class", "tab-pane fade in");
                //ratedetils.Attributes.Add("class", "tab-pane fade in");
                LiRateDetails.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "active");
                Lihomemid.Attributes.Add("class", "");
                //Lirolemid.Attributes.Add("class", "");
            }
        }

    }

    public void InitialSelectBuss()
    {
        dtSelectedBuss.Columns.AddRange(new DataColumn[7] { new DataColumn("EmployeeID"), new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("UserRole"), new DataColumn("UserRoleId"), new DataColumn("RoleId") });
        ViewState["RolePermission"] = dtSelectedBuss;
        ViewState["PrvRolePermission"] = dtSelectedBuss;
        this.BindGrid();

        dtSelectProject.Columns.AddRange(new DataColumn[6] { new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("EmployeeID"), new DataColumn("ProjectName"), new DataColumn("ProjectId") });
        ViewState["ProjetName"] = dtSelectProject;
        BindProjectGrid();

        dtSelectRate.Columns.AddRange(new DataColumn[12] { new DataColumn("RateId"), new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("EmployeeID"), new DataColumn("Rate"), new DataColumn("RateType"), new DataColumn("StartDate"), new DataColumn("EndDate"), new DataColumn("IsEmployeeEIExempty"), new DataColumn("RetainVacation"), new DataColumn("IsEmployeeCPPExempty") });
        ViewState["rateDetails"] = dtSelectRate;
        BindRate();
    }
    protected void BindProjectGrid()
    {
        GvProject.DataSource = (DataTable)ViewState["ProjetName"];
        GvProject.DataBind();
        //Gv_RolePermission.DataSource = (DataTable)ViewState["ProjetName"];
        //Gv_RolePermission.DataBind();
    }
    protected void BindGrid()
    {
        Gv_RolePermission.DataSource = null;
        Gv_RolePermission.DataBind();
        Gv_RolePermission.DataSource = (DataTable)ViewState["RolePermission"];
        Gv_RolePermission.DataBind();
    }
    protected void BindRate()
    {
        GvRateDetails.DataSource = (DataTable)ViewState["rateDetails"];
        GvRateDetails.DataBind();
    }
    public void Bindbusiness()
    {
        if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
        {
            var empid = Session["EmployeeId"].ToString();
            DataTable dt = AddBeCtrl.GetBussForSelectBUSS(int.Parse(empid), int.Parse(Session["UserTypeid"].ToString()));
            if (dt.Rows.Count > 0)
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                ddlBusiness.DataSource = ds;

                ddlBusiness.DataValueField = ds.Tables[0].Columns[1].ToString();
                ddlBusiness.DataTextField = ds.Tables[0].Columns[2].ToString();
                ddlBusiness.DataBind();
                ddlBusiness.SelectedIndex = 0;

                ddl_Business.DataSource = ds;
                ddl_Business.DataValueField = ds.Tables[0].Columns[1].ToString();
                ddl_Business.DataTextField = ds.Tables[0].Columns[2].ToString();
                ddl_Business.DataBind();
                ddl_Business.SelectedIndex = 0;
            }
        }
    }

    //protected void BindAllUserType()
    //{
    //    var uid = Session["UserId"].ToString();
    //    var chekMang = comm.IfExistsInTable("MappingTable", "*", "userid=" + uid + " and UserTypeId=2");
    //    if (chekMang == false)
    //    {
    //        if (ViewState["bid"].ToString() != "0")
    //        {
    //            var dt = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));

    //            ddlRole.Items.Clear();
    //            ddlRole.Items.Add(new ListItem("Select Role", ""));
    //            ddlRole.Items.Add(new ListItem("Add New Role", "0"));
    //            ddlRole.DataSource = dt;
    //            ddlRole.DataValueField = dt.Columns[0].ToString();
    //            ddlRole.DataTextField = dt.Columns[1].ToString();
    //            ddlRole.DataBind();
    //            ddlRole.SelectedValue = "";

    //            ddlEditRolePer.Items.Clear();
    //            ddlEditRolePer.DataSource = dt;
    //            ddlEditRolePer.DataValueField = dt.Columns[0].ToString();
    //            ddlEditRolePer.DataTextField = dt.Columns[1].ToString();
    //            ddlEditRolePer.DataBind();
    //        }
    //        else
    //        {
    //            var dt = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));

    //            ddlRole.Items.Clear();
    //            ddlRole.Items.Add(new ListItem("Select Role", ""));
    //            ddlRole.Items.Add(new ListItem("Add New Role", "0"));
    //            ddlRole.DataSource = dt;
    //            ddlRole.DataValueField = dt.Columns[0].ToString();
    //            ddlRole.DataTextField = dt.Columns[1].ToString();
    //            ddlRole.DataBind();
    //            ddlRole.SelectedValue = "";

    //            ddlEditRolePer.Items.Clear();
    //            ddlEditRolePer.DataSource = dt;
    //            ddlEditRolePer.DataValueField = dt.Columns[0].ToString();
    //            ddlEditRolePer.DataTextField = dt.Columns[1].ToString();
    //            ddlEditRolePer.DataBind();
    //        }
    //    }
    //    else
    //    {
    //        DataTable dtmgr = new DataTable();
    //        if (ViewState["bid"].ToString() != "0")
    //        {
    //            DataTable dtMuid = new DataTable();
    //            dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
    //            DataRow[] selectRow = dtmgr.Select("[User_TypeId]=4");
    //            DataTable dtmanuType = selectRow.CopyToDataTable();

    //            ddlRole.DataSource = dtmgr;
    //            ddlRole.Items.Clear();
    //            ddlRole.Items.Add(new ListItem("Select Role", ""));
    //            ddlRole.Items.Add(new ListItem("Add New Role", "0"));
    //            ddlRole.DataSource = dtmgr;
    //            ddlRole.DataValueField = dtmgr.Columns[0].ToString();
    //            ddlRole.DataTextField = dtmgr.Columns[1].ToString();
    //            ddlRole.DataBind();
    //            ddlRole.SelectedValue = "";

    //            ddlEditRolePer.Items.Clear();
    //            ddlEditRolePer.DataSource = dtmgr;
    //            ddlEditRolePer.DataValueField = dtmgr.Columns[0].ToString();
    //            ddlEditRolePer.DataTextField = dtmgr.Columns[1].ToString();
    //            ddlEditRolePer.DataBind();
    //        }
    //        else
    //        {
    //            DataTable dtMuid = new DataTable();
    //            dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
    //            DataRow[] selectRow = dtmgr.Select("[User_TypeId]=4");
    //            DataTable dtmanuType = selectRow.CopyToDataTable();

    //            dtMuid = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));
    //            ddlRole.DataSource = dtMuid;
    //            ddlRole.Items.Clear();
    //            ddlRole.Items.Add(new ListItem("Select Role", ""));
    //            ddlRole.Items.Add(new ListItem("Add New Role", "0"));
    //            ddlRole.DataSource = dtMuid;
    //            ddlRole.DataValueField = dtMuid.Columns[0].ToString();
    //            ddlRole.DataTextField = dtMuid.Columns[1].ToString();
    //            ddlRole.DataBind();
    //            ddlRole.SelectedValue = "";

    //            ddlEditRolePer.Items.Clear();
    //            ddlEditRolePer.DataSource = dtMuid;
    //            ddlEditRolePer.DataValueField = dtMuid.Columns[0].ToString();
    //            ddlEditRolePer.DataTextField = dtMuid.Columns[1].ToString();
    //            ddlEditRolePer.DataBind();
    //        }
    //    }

    //    DataTable Utpe = new DataTable();
    //    Utpe = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
    //    if (chekMang == false)
    //    {
    //        //ddlRoleNamepop.Items.Clear();
    //        //ddlRoleNamepop.DataSource = Utpe;
    //        //ddlRoleNamepop.DataValueField = Utpe.Columns[0].ToString();
    //        //ddlRoleNamepop.DataTextField = Utpe.Columns[1].ToString();
    //        //ddlRoleNamepop.DataBind();


    //        //ddlCatagoryRole.Items.Add("Please Select");
    //        //ddlCatagoryRole.SelectedItem.Value = "";
    //        //ddlCatagoryRole.DataSource = Utpe;
    //        //ddlCatagoryRole.DataValueField = Utpe.Columns[0].ToString();
    //        //ddlCatagoryRole.DataTextField = Utpe.Columns[1].ToString();
    //        //ddlCatagoryRole.DataBind();
    //        //ddlCatagoryRole.SelectedIndex = 0;
    //    }
    //    else
    //    {
    //        //foreach (ListItem itm in rdoBtnUserTypeList.Items)
    //        //{
    //        //    if(!itm.Value.ToString().Equals("4"))
    //        //    {
    //        //        //itm.Attributes.Add("style", "visibility: hidden");
    //        //        itm.Enabled = false;
    //        //    }
    //        //    else
    //        //    {
    //        //        itm.Selected = true;
    //        //    }
    //        //}
    //        DataRow[] selectRow = Utpe.Select("[User_TypeId]=4");
    //        DataTable dtmanuType = selectRow.CopyToDataTable();
    //        //ddlRoleNamepop.Items.Clear();
    //        //ddlRoleNamepop.DataSource = dtmanuType;
    //        //ddlRoleNamepop.DataValueField = dtmanuType.Columns[0].ToString();
    //        //ddlRoleNamepop.DataTextField = dtmanuType.Columns[1].ToString();
    //        //ddlRoleNamepop.DataBind();

    //        //ddlCatagoryRole.Items.Add("Please Select");
    //        //ddlCatagoryRole.SelectedItem.Value = "";
    //        //ddlCatagoryRole.DataSource = dtmanuType;
    //        //ddlCatagoryRole.DataValueField = dtmanuType.Columns[0].ToString();
    //        //ddlCatagoryRole.DataTextField = dtmanuType.Columns[1].ToString();
    //        //ddlCatagoryRole.DataBind();
    //        //ddlCatagoryRole.SelectedIndex = 0;
    //    }
    //}

    protected void BindAllUserType()
    {
        DataTable dtr = new DataTable();
        var uid = Session["UserId"].ToString();
        var chekMang = comm.IfExistsInTable("MappingTable", "*", "userid=" + uid + " and UserTypeId=2");
        if (chekMang == false)
        {
            if (ViewState["bid"].ToString() != "0")
            {
                var dt = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
                if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Administrator")
                {
                    DataRow[] drAdm = dt.Select("[User_TypeId] in ('2','4')");
                    if (drAdm.Count() > 0)
                    {
                        dtr = drAdm.CopyToDataTable();
                    }
                }
                else if (ViewState["QueryStringRole"].ToString() != null && ViewState["QueryStringRole"].ToString() == "Manager")
                {
                    DataRow[] drMan = dt.Select("[User_TypeId] in ('4')");
                    if (drMan.Count() > 0)
                    {
                        dtr = drMan.CopyToDataTable();
                    }
                }
                else
                {
                    dtr = dt;
                    ViewState["QueryStringRole"] = "";
                }
                ddlRole.Items.Clear();
                ddlRole.Items.Add(new ListItem("Select Role", ""));
                if (ViewState["QueryStringRole"].ToString() == "")
                {
                    ddlRole.Items.Add(new ListItem("Add New Role", "0"));
                }
                ddlRole.DataSource = dtr;
                ddlRole.DataValueField = dtr.Columns[0].ToString();
                ddlRole.DataTextField = dtr.Columns[1].ToString();
                ddlRole.DataBind();
                ddlRole.SelectedValue = "";


                ddlEditRolePer.Items.Clear();
                ddlEditRolePer.DataSource = dtr;
                ddlEditRolePer.DataValueField = dtr.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dtr.Columns[1].ToString();
                ddlEditRolePer.DataBind();
            }
            else
            {
                var dt = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));
                if (Request.QueryString["Role"] != null && Request.QueryString["Role"].ToString() == "Administrator")
                {
                    DataRow[] drAdm = dt.Select("[User_TypeId] in ('2','4')");
                    if (drAdm.Count() > 0)
                    {
                        dtr = drAdm.CopyToDataTable();
                    }
                }
                else if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Manager")
                {
                    DataRow[] drMan = dt.Select("[User_TypeId] in ('4')");
                    if (drMan.Count() > 0)
                    {
                        dtr = drMan.CopyToDataTable();
                    }
                }
                else
                {
                    dtr = dt;
                    ViewState["QueryStringRole"] = "";
                }
                ddlRole.Items.Clear();
                ddlRole.Items.Add(new ListItem("Select Role", ""));
                if (ViewState["QueryStringRole"] == "")
                {
                    ddlRole.Items.Add(new ListItem("Add New Role", "0"));
                }
                ddlRole.DataSource = dtr;
                ddlRole.DataValueField = dtr.Columns[0].ToString();
                ddlRole.DataTextField = dtr.Columns[1].ToString();
                ddlRole.DataBind();
                ddlRole.SelectedValue = "";

                ddlEditRolePer.Items.Clear();
                ddlEditRolePer.DataSource = dtr;
                ddlEditRolePer.DataValueField = dtr.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dtr.Columns[1].ToString();
                ddlEditRolePer.DataBind();
            }
        }
        else
        {
            DataTable dtmgr = new DataTable();
            if (ViewState["bid"].ToString() != "0")
            {
                DataTable dtMuid = new DataTable();
                dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
                if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Administrator")
                {
                    DataRow[] drAdm = dtmgr.Select("[User_TypeId] in ('2','4')");
                    if (drAdm.Count() > 0)
                    {
                        dtr = drAdm.CopyToDataTable();
                    }
                }
                else if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Manager")
                {
                    DataRow[] drMan = dtmgr.Select("[User_TypeId] in ('4')");
                    if (drMan.Count() > 0)
                    {
                        dtr = drMan.CopyToDataTable();
                    }
                }
                else
                {
                    dtr = dtmgr;
                    ViewState["QueryStringRole"] = "";
                }
                //  dtMuid= ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));

                ddlRole.DataSource = dtr;
                ddlRole.Items.Clear();
                ddlRole.Items.Add(new ListItem("Select Role", ""));
                if (ViewState["QueryStringRole"] == "")
                {
                    ddlRole.Items.Add(new ListItem("Add New Role", "0"));
                }
                ddlRole.DataSource = dtr;
                ddlRole.DataValueField = dtr.Columns[0].ToString();
                ddlRole.DataTextField = dtr.Columns[1].ToString();
                ddlRole.DataBind();
                ddlRole.SelectedValue = "";

                ddlEditRolePer.Items.Clear();
                ddlEditRolePer.DataSource = dtr;
                ddlEditRolePer.DataValueField = dtr.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dtmgr.Columns[1].ToString();
                ddlEditRolePer.DataBind();
            }
            else
            {
                DataTable dtMuid = new DataTable();
                dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
                DataRow[] selectRow = dtmgr.Select("[User_TypeId]=4");
                DataTable dtmanuType = selectRow.CopyToDataTable();

                dtMuid = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));
                if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Administrator")
                {
                    DataRow[] drAdm = dtMuid.Select("[User_TypeId] in ('2','4')");
                    if (drAdm.Count() > 0)
                    {
                        dtr = drAdm.CopyToDataTable();
                    }
                }
                else if (ViewState["QueryStringRole"].ToString() != "" && ViewState["QueryStringRole"].ToString() == "Manager")
                {
                    DataRow[] drMan = dtMuid.Select("[User_TypeId] in ('4')");
                    if (drMan.Count() > 0)
                    {
                        dtr = drMan.CopyToDataTable();

                    }
                }
                else
                {
                    dtr = dtmgr;
                    ViewState["QueryStringRole"] = "";

                }
                ddlRole.DataSource = dtr;
                ddlRole.Items.Clear();
                ddlRole.Items.Add(new ListItem("Select Role", ""));
                if (ViewState["QueryStringRole"] == null)
                {
                    ddlRole.Items.Add(new ListItem("Add New Role", "0"));
                }
                ddlRole.DataSource = dtr;
                ddlRole.DataValueField = dtr.Columns[0].ToString();
                ddlRole.DataTextField = dtMuid.Columns[1].ToString();
                ddlRole.DataBind();
                ddlRole.SelectedValue = "";

                ddlEditRolePer.Items.Clear();
                ddlEditRolePer.DataSource = dtMuid;
                ddlEditRolePer.DataValueField = dtMuid.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dtMuid.Columns[1].ToString();
                ddlEditRolePer.DataBind();
            }
        }

        DataTable Utpe = new DataTable();
        Utpe = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
        if (chekMang == false)
        {
            //ddlRoleNamepop.Items.Clear();
            //ddlRoleNamepop.DataSource = Utpe;
            //ddlRoleNamepop.DataValueField = Utpe.Columns[0].ToString();
            //ddlRoleNamepop.DataTextField = Utpe.Columns[1].ToString();
            //ddlRoleNamepop.DataBind();

            //ddlCatagoryRole.Items.Add("Please Select");
            //ddlCatagoryRole.SelectedItem.Value = "";
            //ddlCatagoryRole.DataSource = Utpe;
            //ddlCatagoryRole.DataValueField = Utpe.Columns[0].ToString();
            //ddlCatagoryRole.DataTextField = Utpe.Columns[1].ToString();
            //ddlCatagoryRole.DataBind();
            //ddlCatagoryRole.SelectedIndex = 0;
        }
        else
        {
            //foreach (ListItem itm in rdoBtnUserTypeList.Items)
            //{
            //    if(!itm.Value.ToString().Equals("4"))
            //    {
            //        //itm.Attributes.Add("style", "visibility: hidden");
            //        itm.Enabled = false;
            //    }
            //    else
            //    {
            //        itm.Selected = true;
            //    }
            //}
            DataRow[] selectRow = Utpe.Select("[User_TypeId]=4");
            DataTable dtmanuType = selectRow.CopyToDataTable();
            //ddlRoleNamepop.Items.Clear();
            //ddlRoleNamepop.DataSource = dtmanuType;
            //ddlRoleNamepop.DataValueField = dtmanuType.Columns[0].ToString();
            //ddlRoleNamepop.DataTextField = dtmanuType.Columns[1].ToString();
            //ddlRoleNamepop.DataBind();

            //ddlCatagoryRole.Items.Add("Please Select");
            //ddlCatagoryRole.SelectedItem.Value = "";
            //ddlCatagoryRole.DataSource = dtmanuType;
            //ddlCatagoryRole.DataValueField = dtmanuType.Columns[0].ToString();
            //ddlCatagoryRole.DataTextField = dtmanuType.Columns[1].ToString();
            //ddlCatagoryRole.DataBind();
            //ddlCatagoryRole.SelectedIndex = 0;
        }
    }

    protected void ddl_Business_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        if (ddl_Business.SelectedValue != "")
        {
            ddl_Employee.Items.Clear();// = null;
                                       // ddl_Employee.DataBind();
            DataTable Business_Owner_Pop_up_Employee_List_ = new DataTable();
            var bussid = ddl_Business.SelectedValue;
            ViewState["bid"] = ddl_Business.SelectedValue.ToString();
            lblActiveEmployeeCount.Text = "Active employees: " + AnyminuteEmployeeRepair.ActiveCount(int.Parse(ddl_Business.SelectedValue));
            var empid = Session["EmployeeId"].ToString();
            if (Request.QueryString["BussEditId"] != null)
            {
                Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(Request.QueryString["BussEditId"].ToString()), int.Parse(empid));
            }
            else
            {
                Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(ddl_Business.SelectedValue), int.Parse(empid));
            }

            if (Business_Owner_Pop_up_Employee_List_.Rows.Count > 0)
            {
                // DataRow[] drselect = Business_Owner_Pop_up_Employee_List_.Select("[IsActive]='YES'");
                DataTable dtselect = Business_Owner_Pop_up_Employee_List_;


                //
                //if (drselect.Length > 0)
                if (dtselect.Rows.Count > 0)
                {
                    //DataTable dtselect = drselect.CopyToDataTable();
                    //ddl_Employee.Items.Add(new ListItem("Select Employee", ""));
                    ddl_Employee.Items.Add(new ListItem("Please Select", ""));
                    ddl_Employee.DataSource = dtselect;
                    //if (dtselect.Rows.Count > 0)
                    //{
                    ddl_Employee.DataValueField = dtselect.Columns[0].ToString();
                    ddl_Employee.DataTextField = dtselect.Columns[3].ToString();
                    ddl_Employee.DataBind();
                    //}
                }
            }
        }
    }

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

    public DataTable Business_Owner_Pop_up_Employee_List(int BusinessId, int EmployeeId)
    {
        try
        {
            loginfo = "Business_Owner_Pop_up_Employee_List(" + BusinessId + "," + EmployeeId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "GetBusinessOwner_Pop_Up_Emp_List_With_Role";
            cmd.Parameters.AddWithValue("@BusinessId", BusinessId);
            cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Business_Owner_Pop_up_Employee_List(" + BusinessId + "," + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Business_Owner_Pop_up_Employee_List" + ex.Message);
            throw ex;
        }
    }

    protected void ddl_Employee_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        if (ddl_Employee.SelectedValue != "")
        {
            Div_Personaldetails_Info.Attributes.Add("style", "display:block;");
            Div_Personaldetails_Nav_Button.Attributes.Add("style", "display:block;");

            GetChangeRateRole(int.Parse(ddl_Employee.SelectedValue));
            GetPersonalDetailsForEdit(int.Parse(ddl_Employee.SelectedValue));
            //GetAssgineBuss(txtMainEmail.Text, Session["UserTypeid"].ToString());
            //GetAssgineProject(txtMainEmail.Text, Session["UserTypeid"].ToString());
            //GetRateDetails(txtMainEmail.Text, Session["UserTypeid"].ToString());
            DataTable dt_GetEmailId = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = " + int.Parse(ddl_Employee.SelectedValue) + ")", "");
            GetAssgineBuss(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());
            GetAssgineProject(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());
            GetRateDetails(dt_GetEmailId.Rows[0]["MainEmail"].ToString(), Session["UserTypeid"].ToString());

            //var profile = comm.GetAllDataFromTable("UserAcc", "Profile_Image", "email='" + txtMainEmail.Text + "'", "");
            //previewImage.ImageUrl = "~/Uploads/" + profile.Rows[0][0].ToString();
            //ViewState["profile"] = profile.Rows[0][0].ToString();

            var profile = comm.GetAllDataFromTable("UserAcc", "Profile_Image", "email='" + txtMainEmail.Text + "'", "");
            if (profile.Rows[0][0].ToString() != string.Empty)
            {
                previewImage.ImageUrl = "~/Uploads/" + profile.Rows[0][0].ToString();
                ViewState["profile"] = profile.Rows[0][0].ToString();
            }
            else
            {
                //previewImage.ImageUrl = "~/Uploads/defaultprofile.png";
                //previewImage.ImageUrl = "~/Uploads/profile.jpg";
                //ViewState["profile"] = profile.Rows[0][0].ToString();
            }

            ddl_Business.Enabled = false;
            ddl_Employee.Enabled = false;
        }
        //txtHiredate.ReadOnly = true;
    }

    public void GetRateDetails(string email, string UserTypeid)
    {
        DataTable dtAssgin = new DataTable();
        DataTable dtSelect = new DataTable();
        DataTable dtrateDetails = RateCtrl.Sp_GetSubsBussRate(email, int.Parse(Session["EmployeeId"].ToString()), UserTypeid);

        //if (Session["UserTypeid"].ToString() != "1")
        //{
        //    if (Request.QueryString["BussEditId"] != null)
        //    {
        //        DataRow[] row = dtrateDetails.Select("BussinessId=" + Request.QueryString["BussEditId"].ToString());
        //        if (row.Count() > 0)
        //        {
        //            dtSelect = row.CopyToDataTable();
        //        }
        //    }
        //    else
        //    {
        //        dtSelect = dtrateDetails.Copy();
        //    }
        //}
        //else
        //{
        //    dtSelect = dtrateDetails.Copy();
        //}

        ViewState["rateDetails"] = dtrateDetails;
        BindRate();
    }

    public void GetRate(string email)
    {
        DataTable dt = (DataTable)ViewState["rateDetails"];
        DataTable dtSelect = new DataTable();

        DataTable dtRateHistory = new DataTable();
        dtRateHistory.Columns.AddRange(new DataColumn[7] { new DataColumn("BusinessName"), new DataColumn("RateId"), new DataColumn("Rate"), new DataColumn("RateType"), new DataColumn("StartDate"), new DataColumn("EndDate"), new DataColumn("effectivedate") });
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            DataTable dt1 = new DataTable();
            dt1 = RateCtrl.GetRateHistory(Convert.ToInt32(dt.Rows[i]["EmployeeID"].ToString()));
            foreach (DataRow dr in dt1.Rows)
            {
                dtRateHistory.Rows.Add(dr.ItemArray);
            }
        }
        dtSelect = dtRateHistory.Select("BusinessName='" + ddlBussRateHistory.SelectedItem.Text + "'").CopyToDataTable();
        gvRateHistory.DataSource = dtSelect;
        gvRateHistory.DataBind();
    }

    public void GetAssgineBuss(string email, string usertypeid)
    {
        DataTable dtAssgin = new DataTable();
        DataTable dtSelect = new DataTable();
        dtAssgin = AddBeCtrl.Sp_GetSubsBuss(email, int.Parse(Session["EmployeeId"].ToString()), Session["UserTypeid"].ToString());
        //if (Session["UserTypeid"].ToString() != "1")
        //{
        //    if (Request.QueryString["BussEditId"] != null)
        //    {
        //        DataRow[] row = dtAssgin.Select("BussinessId=" + Request.QueryString["BussEditId"].ToString());
        //        if (row.Count() > 0)
        //        {
        //            dtSelect = row.CopyToDataTable();
        //        }
        //    }
        //    else
        //    {
        //        dtSelect = dtAssgin.Copy();
        //    }
        //}
        //else
        //{
        //    dtSelect = dtAssgin.Copy();
        //}

        ViewState["RolePermission"] = dtAssgin;
        ViewState["PrvRolePermission"] = dtAssgin;
        BindGrid();
        ddlBusAssProject.Items.Clear();
        ddlBusAssProject.Items.Add(new ListItem("Please Select", ""));
        DataTable projectBusinesses = dtAssgin.Clone();
        var seenProjectBusinesses = new System.Collections.Generic.HashSet<string>();
        foreach (DataRow sourceRow in dtAssgin.Rows)
        {
            string businessKey = Convert.ToString(sourceRow[2]);
            if (seenProjectBusinesses.Add(businessKey)) projectBusinesses.ImportRow(sourceRow);
        }
        ddlBusAssProject.DataSource = projectBusinesses;
        ddlBusAssProject.DataValueField = dtAssgin.Columns[2].ToString();
        ddlBusAssProject.DataTextField = dtAssgin.Columns[1].ToString();
        ddlBusAssProject.DataBind();

        ddlBussRateHistory.DataSource = dtAssgin;
        ddlBussRateHistory.Items.Clear();
        ddlBussRateHistory.Items.Add("Please Select");
        ddlBussRateHistory.DataValueField = dtAssgin.Columns[2].ToString();
        ddlBussRateHistory.DataTextField = dtAssgin.Columns[1].ToString();
        ddlBussRateHistory.DataBind();
        ddlBussRateHistory.SelectedIndex = 0;
    }

    public void GetAssgineProject(string email, string UserTypeId)
    {
        DataTable dtAssgin = new DataTable();
        DataTable dtSelect = new DataTable();
        dtAssgin = apc.Sp_GetSubsBussProject(email, int.Parse(Session["EmployeeId"].ToString()), Session["UserTypeid"].ToString());

        // Some legacy installs return an empty project list even when ASSIGNPROJECT contains rows.
        // Fall back to the persisted assignments for the employee being edited.
        if (dtAssgin.Rows.Count == 0)
        {
            DataTable employeeLookup = comm.ExecuteQueryDataTable(
                "SELECT TOP 1 ad.EmployeeID FROM AddEmployee ad JOIN EmpAddress ea ON ea.EmpAddressId=ad.ADDRESSID " +
                "WHERE ea.MainEmail='" + email.Replace("'", "''") + "'");
            if (employeeLookup.Rows.Count > 0)
            {
                int projectEmployee = Convert.ToInt32(employeeLookup.Rows[0]["EmployeeID"]);
                dtAssgin = comm.ExecuteQueryDataTable(
                    "SELECT DISTINCT dbo.FN_DESC('AddBusiness',ap.BUSINESSID) AS BusinessName, " +
                    "ap.BUSINESSID AS BussinessId, dbo.FN_DESC('AddEmployee',ap.EMPLOYEEID) AS EmployeeName, " +
                    "ap.EMPLOYEEID AS EmployeeID, 0 AS RoleId, 0 AS UserRoleId, " +
                    "ap.PROJECTID AS ProjectId, dbo.FN_DESC('Project',ap.PROJECTID) AS ProjectName " +
                    "FROM ASSIGNPROJECT ap WHERE ap.EMPLOYEEID=" + projectEmployee + " ORDER BY BussinessId,ProjectName");
            }
        }

        //if (Session["UserTypeid"].ToString() != "1")
        //{
        //    if (Request.QueryString["BussEditId"] != null)
        //    {
        //        DataRow[] row = dtAssgin.Select("BussinessId=" + Request.QueryString["BussEditId"].ToString());
        //        if(row.Count()>0)
        //        {
        //            dtSelect = row.CopyToDataTable();
        //        }
        //    }
        //    else
        //    {
        //        dtSelect = dtAssgin.Copy();
        //    }
        //}
        //else
        //{
        //    dtSelect = dtAssgin.Copy();
        //}
        ViewState["ProjetName"] = dtAssgin;
        BindProjectGrid();
    }
    protected void txtMainEmail_TextChanged(object sender, EventArgs e)
    {
        ImageSave();

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
                #region added by kumar after bug raised by tester
                var ceckSub1 = comm.IfExistsInTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", "*", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1");
                if (ceckSub1 == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckSupportAdmin1 = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
                if (ceckSupportAdmin1 == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckRegister_Partner1 = comm.IfExistsInTable("[Register_Partner]", "*", "[EMail_primary]='" + txtMainEmail.Text + "'");
                if (ceckRegister_Partner1 == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckMotherOrg1 = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
                if (ceckMotherOrg1 == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                #endregion

                divValid.Visible = false;
                bool ceck = false;
                ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
                if (ceck == false)
                {
                    divconfirm.Visible = true;
                }
                else
                {

                }
                if (txtMainEmail.Text.Trim() == "")
                {
                    divconfirm.Visible = false;
                    DataTable dtUserId = comm.GetAllDataFromTable("UserAcc", "UserId", "Email='" + txtMainEmail.Text + "'", "");
                    ViewState["ViweStateUserID"] = dtUserId.Rows[0]["UserId"].ToString();
                }
                var ceckSub = comm.IfExistsInTable("UserAcc ua join AddEmployee ad on ad.EmployeeID=ua.EmployeeId join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID", "*", "ua.Email='" + txtMainEmail.Text + "' and ad.UserType=1");
                if (ceckSub == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckSupportAdmin = comm.IfExistsInTable("[SupportAdmin]", "*", "[Email]='" + txtMainEmail.Text + "'");
                if (ceckSupportAdmin == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckRegister_Partner = comm.IfExistsInTable("[Register_Partner]", "*", "[EMail_primary]='" + txtMainEmail.Text + "'");
                if (ceckRegister_Partner == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
                }
                var ceckMotherOrg = comm.IfExistsInTable("[MotherOrg]", "*", "[Email]='" + txtMainEmail.Text + "'");
                if (ceckMotherOrg == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Subscriber ID Cannot Be Used For Other Role. Use Another Id!', 'info');", true);
                    txtMainEmail.Text = "";
                    return;
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

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
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

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        ImageSave();
        if (txtSearchPostalCode.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter valid postal code in format A#A #A# to search', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
            txtSearchPostalCode.Focus();
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        int search_postalcode_length = int.Parse(_serachpostcode.Length.ToString());
        //if (_serachpostcode.Length.ToString() != "6")
        if (search_postalcode_length < 5)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
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

    protected void StartDate_Click(object sender, EventArgs e)
    {
        ImageSave();
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

            txtCity.Text = Split[1].ToString().ToUpper();
            txtCity.ReadOnly = true;
            txtProvince.Text = Split[2].ToString().ToUpper();
            txtProvince.ReadOnly = true;
        }
        else
        {
            txtSuiteNumber.Text = "";
            txtStreetNo.Text = "";
            //firoz 08
            //DropDownList1.SelectedValue = "";
            //end
            DropDownList2.SelectedIndex = 0;
            txtSuiteNumber.Text = "";
            txtPostalCode.Text = "";
            txtPostalCode.ReadOnly = false;
            txtPostalCode.Enabled = true;
            txtStreetName.Text = "";
            txtStreetName.ReadOnly = false;
            txtStreetName.Enabled = true;
            txtStreetType.Text = "";

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

    public void ClearAddress()
    {
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        //firoz 08
        // DropDownList1.SelectedValue = "";
        //end
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";
        txtStreetNo.Text = "";
    }

    protected void rdobtnlist_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();

        if (ddlRole.SelectedValue == "0")
        {
            Label22.Text = "";
            ModalAddRolePop.Show();
            GetUserPermissionRoleList(0, GridView_User_Role_Permission_Pop);
        }
        if (ddlRole.SelectedValue != "2" && ddlRole.SelectedValue != "4" && ddlRole.SelectedValue != "5" && ddlRole.SelectedValue != "12" && ddlRole.SelectedValue != "" && ddlRole.SelectedValue != "0")
        {
            ////ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Click Edit Permission And Give Permission', 'info');", true);
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Click Add then Click on Edit Permission to Assign Custom Role Permission', 'info');", true);

            if (ViewState["RoleId_PopAdd"].ToString() == "")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Click Add then Click on Edit Permission to Assign Custom Role Permission', 'info');", true);
            }
            else
            {
                string[] ArrRoleId_PopAdd = ViewState["RoleId_PopAdd"].ToString().Split(',');
                if (ArrRoleId_PopAdd[0].ToString() != ddlRole.SelectedItem.Text)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Click Add then Click on Edit Permission to Assign Custom Role Permission', 'info');", true);
                }
            }

        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        ImageSave();
        int cnt = 0;
        if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Either First or Last Name is warning', 'error');", true);
            return;
        }
        //if (ddlBusiness.SelectedValue == "" || ddlRole.SelectedValue == "" || ddlRole.SelectedValue == "0")
        //{
        //    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either Business Name or Role is Not Select', 'error');", true);
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either Business Name or Role is Not Select', 'error');", true);
        //    return;
        //}

        if (ddlBusiness.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Assign Business', 'warning');", true);
            return;
        }
        if (ddlRole.SelectedValue == "" || ddlRole.SelectedValue == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Assign Role', 'warning');", true);
            return;
        }

        #region show message if maximum limit is reached. This region code is same as of in Save button.
        if (Session["UserTypeid"].ToString() == "1")
        {
            int empcounttt = 0;
            var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
            bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
            if (!check_reseller_or_subscriber_email)
            {
                var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' ");
                if (check_old_subscriber.Rows.Count > 0)
                {
                    #region old logic commented
                    //var empidddd = Session["EmployeeId"].ToString();
                    //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empidddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empidddd + "' and IsActive=1)");
                    //DataTable dtCommaUser = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join UserAcc ua on ua.Email=ea.MainEmail", "ad.EmployeeID,ua.UserId", "ua.Email='" + Session["email"].ToString() + "'", "");
                    //DataTable dt_getactualregistereduserscount = new DataTable();
                    //string CommaUserid = string.Empty;
                    //for (int i = 0; i < dtCommaUser.Rows.Count; i++)
                    //{
                    //    CommaUserid = CommaUserid + dtCommaUser.Rows[i]["UserId"].ToString();
                    //    CommaUserid += (i < dtCommaUser.Rows.Count) ? "," : string.Empty;
                    //}
                    //CommaUserid = CommaUserid.TrimEnd(',');
                    //DataTable dtMapping = new DataTable(); dtMapping = comm.GetAllDataFromTable("MappingTable", "*", "USERID in(" + CommaUserid + ")", "BusinessID,UserTypeId");

                    //int cnt_bo = dtMapping.Rows.Count;
                    //foreach (DataRow row in dtMapping.Rows)
                    //{
                    //    if (row["UserTypeId"].ToString() == "1")
                    //    {
                    //        int rCount = 0;
                    //        int rDeactiveCount = 0;
                    //        DataTable dtIsEdit = new DataTable();

                    //        try
                    //        {
                    //            if (Session["Redirect"] != null)
                    //            {
                    //                if (Session["MotherOrgid_acc"].ToString() != "")
                    //                {
                    //                    dt_getactualregistereduserscount = AddBeCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                    //                }
                    //                else
                    //                {
                    //                    dt_getactualregistereduserscount = AddBeCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                    //                }
                    //            }
                    //            if (Session["Redirect"] == null)
                    //            {
                    //                dt_getactualregistereduserscount = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                    //            }
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            dt_getactualregistereduserscount = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                    //        }
                    //        rCount = dt_getactualregistereduserscount.Rows.Count;
                    //        dtIsEdit = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + int.Parse(row["EmployeeID"].ToString().ToString()) + "", "");
                    //        DataRow[] rowsFiltered = dt_getactualregistereduserscount.Select("Status=2");
                    //        rDeactiveCount = int.Parse(rowsFiltered.Length.ToString());
                    //    }
                    //}

                    //for (int m = 0; m < dt_getactualregistereduserscount.Rows.Count; m++)
                    //    empcounttt = empcounttt + Convert.ToInt32(dt_getactualregistereduserscount.Rows[m]["CNT"].ToString());
                    #endregion

                    var empiddddone = Session["EmployeeId"].ToString();
                    DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users,MotherOrgID having EmpId='" + empiddddone + "' " +
                        "and Id=(select max(id) from PlanHistory where EmpId='" + empiddddone + "' " +
                        "and IsActive=1)");
                    //int empcounttt = 0; //commented because declared above only
                    //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                    //DataTable dtBussListtt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                    DataTable dtBussListtt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                        "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                        "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                        "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                        "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                        "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
                    for (int m = 0; m < dtBussListtt.Rows.Count; m++)
                        empcounttt = empcounttt + Convert.ToInt32(dtBussListtt.Rows[m]["CNT"].ToString());

                    foreach (GridViewRow row in Gv_RolePermission.Rows)
                    {
                        HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                        if (hdf_EmpId.Value == "0")
                            empcounttt = empcounttt + 1;
                    }

                    if (empcounttt >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                        return;
                    }
                }
            }
            if (check_reseller_or_subscriber_email)
            {
                var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
                if (check_old_reseller.Rows.Count > 0)
                {
                    //old logic commented for reseller
                    //int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
                    //DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
                    //for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
                    //{
                    //    DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
                    //    total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
                    //}

                    //foreach (GridViewRow row in Gv_RolePermission.Rows)
                    //{
                    //    HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                    //    if (hdf_EmpId.Value == "0")
                    //        total_no_of_user = total_no_of_user + 1;
                    //}

                    //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                    //int EMPCNT_RESELLER = 0;
                    //EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                    //if (total_no_of_user >= EMPCNT_RESELLER)
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                    //    return;
                    //}

                    DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory where IsActive=0 and " +
                        "EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and " +
                        "MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                    if (dt_check_active_status.Rows.Count == 0)
                    {
                        var empidddd = Session["EmployeeId"].ToString();
                        DataTable dtBussCnttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users,MotherOrgID " +
                            "having EmpId='" + empidddd + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " " +
                            "and Id=(select max(id) from PlanHistory where EmpId='" + empidddd + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " and IsActive=1)");
                        //int empcounttt = 0; //commented because declared above only
                        //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                        //DataTable dtBussListtt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                        DataTable dtBussListtt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                            "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                            "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                            "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                            "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                            "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' " +
                            "and ASB.MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'");
                        for (int m = 0; m < dtBussListtt.Rows.Count; m++)
                            empcounttt = empcounttt + Convert.ToInt32(dtBussListtt.Rows[m]["CNT"].ToString());

                        foreach (GridViewRow row in Gv_RolePermission.Rows)
                        {
                            HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                            if (hdf_EmpId.Value == "0")
                                empcounttt = empcounttt + 1;
                        }

                        if (empcounttt >= int.Parse(dtBussCnttt.Rows[0][0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                            return;
                        }
                    }
                    if (dt_check_active_status.Rows.Count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Plan is Cancelled, Please Contact Admin', 'error');", true);
                        return;
                    }
                }
            }
        }
        #endregion

        #region show message if maximum limit is reached. This is for Mgr, Adm, Cust Role User Login. This region code is same as of in Save button.
        if (Session["UserTypeid"].ToString() != "1")
        {
            DataTable Get_Reseller_Subscriber_Details = comm.GetAllDataFromTable("AssginBussiness", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(ddlBusiness.SelectedValue) + "' and UserTypeId=1", "");
            DataTable Get_MotherOrg_Details = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ", "");
            bool Check_Register_Partner_Details = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + Get_MotherOrg_Details.Rows[0]["Email"].ToString() + "' ");
            if (!Check_Register_Partner_Details)
            {
                var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' ");
                if (check_old_subscriber.Rows.Count > 0)
                {
                    var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and Is_Cancelled=1 ");
                    if (check_cancelled.Rows.Count == 0)
                    {
                        DataTable dtBussCntttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users " +
                            "having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                            "and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=1)");
                        int empppppcount = 0;
                        //DataTable dtBussListttt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()), int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()), int.Parse("1"));
                        DataTable dtBussListttt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                        "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                        "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                        "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                        "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                        "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                        "and ASB.MotherOrgID='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "'");
                        for (int m = 0; m < dtBussListttt.Rows.Count; m++)
                            empppppcount = empppppcount + Convert.ToInt32(dtBussListttt.Rows[m]["CNT"].ToString());

                        foreach (GridViewRow row in Gv_RolePermission.Rows)
                        {
                            HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                            HiddenField hfd_BussinessId_ = row.FindControl("hfd_BussinessId") as HiddenField;
                            DataTable Get_Gv_RolePermission_BO = comm.GetAllDataFromTable("AssginBussiness", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hfd_BussinessId_.Value) + "' and UserTypeId=1", "");
                            if (Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString() == Get_Gv_RolePermission_BO.Rows[0]["EmployeeID"].ToString())
                            {
                                if (hdf_EmpId.Value == "0")
                                    empppppcount = empppppcount + 1;
                            }
                        }

                        if (empppppcount >= int.Parse(dtBussCntttt.Rows[0][0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                            return;
                        }
                    }
                    if (check_cancelled.Rows.Count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as the Plan is Cancelled, Please Contact Admin', 'error');", true);
                        return;
                    }
                }
            }
            if (Check_Register_Partner_Details)
            {
                var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ");
                if (check_old_reseller.Rows.Count > 0)
                {
                    var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and Is_Cancelled=1 ");
                    if (check_cancelled.Rows.Count == 0)
                    {
                        //old logic commented for overall reseller
                        //int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
                        //DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
                        //for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
                        //{
                        //    DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
                        //    total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
                        //}

                        //foreach (GridViewRow row in Gv_RolePermission.Rows)
                        //{
                        //    HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                        //    if (hdf_EmpId.Value == "0")
                        //        total_no_of_user = total_no_of_user + 1;
                        //}

                        //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and IsActive=1)");
                        //int EMPCNT_RESELLER = 0;
                        //EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                        //if (total_no_of_user >= EMPCNT_RESELLER)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                        //    return;
                        //}


                        DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory where IsActive=0 and " +
                        "EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and " +
                        "MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " ");
                        if (dt_check_active_status.Rows.Count == 0)
                        {
                            int total_no_of_user = 0;
                            DataTable dtBussListttt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and ASB.MotherOrgID='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "'");

                            for (int m = 0; m < dtBussListttt.Rows.Count; m++)
                                total_no_of_user = total_no_of_user + Convert.ToInt32(dtBussListttt.Rows[m]["CNT"].ToString());

                            foreach (GridViewRow row in Gv_RolePermission.Rows)
                            {
                                HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
                                HiddenField hfd_BussinessId_ = row.FindControl("hfd_BussinessId") as HiddenField;
                                DataTable Get_Gv_RolePermission_Business_MotherOrgId = comm.GetAllDataFromTable("AssginBussiness", "MotherOrgID", "BusinessID = '" + int.Parse(hfd_BussinessId_.Value) + "' and UserTypeId=1", "");
                                DataTable Get_Gv_RolePermission_BO = comm.GetAllDataFromTable("AssginBussiness", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hfd_BussinessId_.Value) + "' " +
                                    "and MotherOrgID = '" + int.Parse(Get_Gv_RolePermission_Business_MotherOrgId.Rows[0]["MotherOrgID"].ToString()) + "' and UserTypeId=1", "");
                                if (Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString() == Get_Gv_RolePermission_BO.Rows[0]["EmployeeID"].ToString())
                                {
                                    if (hdf_EmpId.Value == "0")
                                        total_no_of_user = total_no_of_user + 1;
                                }
                            }

                            DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users,MotherOrgID " +
                                "having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " " +
                                "and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " and IsActive=1)");
                            int EMPCNT_RESELLER = 0;
                            EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                            if (total_no_of_user >= EMPCNT_RESELLER)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                                return;
                            }
                        }
                        if (dt_check_active_status.Rows.Count == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as the Plan is Cancelled, Please Contact Admin', 'error');", true);
                            return;
                        }
                    }
                    if (check_cancelled.Rows.Count > 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as the Plan is Cancelled, Please Contact Admin', 'error');", true);
                        return;
                    }
                }
            }
        }
        #endregion

        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            if (hfd_BussinessId.Value == ddlBusiness.SelectedValue)
            {
                cnt++;
            }
        }
        if (cnt > 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Business Already Addedd', 'error');", true);
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'User has been assigned role, Please add more users to add a different role', 'error');", true);
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'User is already added in " + ddlBusiness.SelectedItem.Text + ", <br> Please assign other business', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'User has already been added in " + ddlBusiness.SelectedItem.Text + ", <br> Please select another business', 'error');", true);
            return;
        }

        DataTable dt = (DataTable)ViewState["RolePermission"];
        DataTable dtrate = (DataTable)ViewState["rateDetails"];
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusiness.SelectedItem.Text;
        var rolename = ddlRole.SelectedItem.Text;
        var StartDate_RateGrid = txtHiredate.Text;
        var empid = 0;

        StringBuilder StringBuilder__ = new StringBuilder();
        var AllRole = comm.ExecuteQueryDataTable("select Role_Permission_Id FROM Role_Permission_Master WHERE UserTypeId='" + ddlRole.SelectedValue + "'");
        if (AllRole.Rows.Count > 0)
        {
            for (int i = 0; i < AllRole.Rows.Count; i++)
            {
                StringBuilder__.Append(AllRole.Rows[i][0].ToString() + ",");
            }

        }
        else
        {
            if (ViewState["RoleId_PopAdd"].ToString() != "")
            {
                string[] ArrRoleId_PopAdd = ViewState["RoleId_PopAdd"].ToString().Split(',');
                if (ArrRoleId_PopAdd[0].ToString() == ddlRole.SelectedItem.Text)
                {
                    for (int j = 1; j < ArrRoleId_PopAdd.Length - 1; j++)
                    {
                        StringBuilder__.Append(ArrRoleId_PopAdd[j].ToString() + ",");
                    }
                }
            }

        }

        dt.Rows.Add(empid, bussname, ddlBusiness.SelectedValue, EmpName, rolename, ddlRole.SelectedValue, StringBuilder__.ToString());
        ViewState["RolePermission"] = dt;
        BindGrid();
        //dtrate.Rows.Add(0, bussname, ddlBusiness.SelectedValue, EmpName, empid, 0, "", "", "", "No", "No", "No");
        dtrate.Rows.Add(0, bussname, ddlBusiness.SelectedValue, EmpName, empid, 0, "", StartDate_RateGrid, "", "No", "No", "No");
        ViewState["rateDetails"] = dtrate;
        BindRate();
        ddlBusAssProject.Items.Clear();
        ddlBusAssProject.Items.Add("Please Select");
        ddlBusAssProject.SelectedItem.Value = "";
        ddlBusAssProject.DataSource = dt;
        ddlBusAssProject.DataValueField = dt.Columns[2].ToString();
        ddlBusAssProject.DataTextField = dt.Columns[1].ToString();
        ddlBusAssProject.DataBind();
        string str = "";
        if (fupd.HasFile)
        {
            var fileName = Path.GetFileName(fupd.PostedFile.FileName);
            var fileExt = Path.GetExtension(fileName);
            str = string.Format("{0}{1}",
                                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                        fileExt);
            fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
        }
        ViewState["profile"] = str;
    }

    protected void Gv_RolePermission_RowEditing(object sender, GridViewEditEventArgs e)
    {
        ImageSave();
        Gv_RolePermission.EditIndex = e.NewEditIndex;
        ViewState["BGridEdit"] = true;
        BindGrid();
    }

    protected void Gv_RolePermission_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        ImageSave();
        Gv_RolePermission.EditIndex = -1;
        ViewState["BGridEdit"] = false;
        BindGrid();
    }

    protected void Gv_RolePermission_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        ImageSave();
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfd_UserRoleIdEdit = (e.Row.FindControl("hfd_UserRoleIdEdit") as HiddenField);
            HiddenField hfd_UserRoleId = (e.Row.FindControl("hfd_UserRoleId") as HiddenField);
            LinkButton lnk_ChangeRoleName = (e.Row.FindControl("lnk_ChangeRoleName") as LinkButton);
            Label lbl_separate = (e.Row.FindControl("lbl_separate") as Label);
            LinkButton lnk_EditPermission = (e.Row.FindControl("lnk_EditPermission") as LinkButton);
            LinkButton lnk_delete = (e.Row.FindControl("lnk_delete") as LinkButton);
            Label lbl_separate2 = (e.Row.FindControl("lbl_separate2") as Label);
            if (hfd_UserRoleIdEdit != null)
            {
                if (hfd_UserRoleIdEdit.Value == "2" || hfd_UserRoleIdEdit.Value == "4" || hfd_UserRoleIdEdit.Value == "5" || hfd_UserRoleIdEdit.Value == "12")
                {
                    lnk_ChangeRoleName.Visible = false;
                    lbl_separate.Visible = false;
                }
            }
            else
            {
                if (hfd_UserRoleId.Value == "2" || hfd_UserRoleId.Value == "4" || hfd_UserRoleId.Value == "5" || hfd_UserRoleId.Value == "12")
                {
                    lnk_ChangeRoleName.Visible = false;
                    lbl_separate.Visible = false;
                }
            }
            bool BGridEdit = (bool)ViewState["BGridEdit"];
            if (BGridEdit == true)
            {
                lnk_ChangeRoleName.Enabled = false;
                lnk_EditPermission.Enabled = false;
                lnk_delete.Enabled = false;
            }
            else
            {
                lnk_ChangeRoleName.Enabled = true;
                lnk_EditPermission.Enabled = true;
                lnk_delete.Enabled = true;
            }


            #region Kumar code for role permissions, cancel plan, deactivate subscriber reseller by support admin
            HiddenField hfd_BussinessId = (e.Row.FindControl("hfd_BussinessId") as HiddenField);
            HiddenField hdf_EmpId = (e.Row.FindControl("hdf_EmpId") as HiddenField);
            if (Session["UserTypeid"].ToString() == "1")
            {
                if (hdf_EmpId.Value != "0")
                {
                    //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                    //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                    //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                    //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                    var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hdf_EmpId.Value) + "'", "");
                    if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        e.Row.Enabled = false;
                    }
                }
            }
            if (Session["UserTypeid"].ToString() != "1")
            {
                if (hdf_EmpId.Value != "0")
                {
                    string[] role;
                    //DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hfd_BussinessId.Value + "'", "UserTypeId");
                    DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hfd_BussinessId.Value + "'", "UserTypeId");
                    DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,UserTypeId", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");

                    if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
                    {
                        if (Dt_Check_Custom_Role_Permission.Rows[0]["UserTypeId"].ToString() == "5")
                        {
                            role = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                            for (int i = 0; i < role.Length; i++)
                            {
                                if (role[i].ToString() == "16")
                                {
                                    lnk_EditPermission.Visible = true;
                                    lbl_separate2.Visible = true;
                                    break;
                                }
                                else
                                {
                                    lnk_EditPermission.Visible = false;
                                    lbl_separate2.Visible = false;
                                }
                            }
                        }
                        if (Dt_Check_Custom_Role_Permission.Rows[0]["UserTypeId"].ToString() != "2" && Dt_Check_Custom_Role_Permission.Rows[0]["UserTypeId"].ToString() != "4"
                            && Dt_Check_Custom_Role_Permission.Rows[0]["UserTypeId"].ToString() != "5" && Dt_Check_Custom_Role_Permission.Rows[0]["UserTypeId"].ToString() != "12")
                        {
                            role = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                            for (int i = 0; i < role.Length; i++)
                            {
                                if (role[i].ToString() == "16")
                                {
                                    lnk_EditPermission.Visible = true;
                                    lbl_separate2.Visible = true;
                                    break;
                                }
                                else
                                {
                                    lnk_EditPermission.Visible = false;
                                    lbl_separate2.Visible = false;
                                }
                            }
                        }
                    }
                }

                DataTable Get_Reseller_Subscriber_ID = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and UserTypeId=1", "");

                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "'", "");

                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=0 ");

                    var check_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    //if (check_subscriber_cancelled.Rows.Count > 0 || check_subscriber_plan_deactivated.Rows.Count > 0
                    //    || check_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                    //    || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    if (check_subscriber_cancelled.Rows.Count > 0
                        || check_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        if (hdf_EmpId.Value != "0")
                        {
                            e.Row.Enabled = false;
                        }
                    }

                    if (hdf_EmpId.Value != "0")
                    {
                        //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                        //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                        //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                        //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hdf_EmpId.Value) + "'", "");
                        if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                        {
                            e.Row.Enabled = false;
                        }
                    }
                }

                if (check_reseller_or_subscriber_email)
                {
                    var check_reseller_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and Is_Cancelled=1 ");

                    var check_reseller_fully_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_reseller_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' " +
                        "and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and IsActive=0 ");

                    var check_reseller_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    if (check_reseller_subscriber_cancelled.Rows.Count > 0 || check_reseller_fully_cancelled.Rows.Count > 0
                        || check_reseller_subscriber_plan_deactivated.Rows.Count > 0 || check_reseller_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        if (hdf_EmpId.Value != "0")
                        {
                            e.Row.Enabled = false;
                        }
                    }

                    if (hdf_EmpId.Value != "0")
                    {
                        //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                        //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                        //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                        //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hdf_EmpId.Value) + "'", "");
                        if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                        {
                            e.Row.Enabled = false;
                        }
                    }
                }
            }
            #endregion
        }
    }

    protected void Gv_RolePermission_PreRender(object sender, EventArgs e)
    {
        ImageSave();
        if (Gv_RolePermission.Rows.Count > 0)
        {
            Gv_RolePermission.UseAccessibleHeader = true;
            Gv_RolePermission.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    protected void Gv_RolePermission_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ImageSave();
        if (e.CommandName == "EditPermission")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = Gv_RolePermission.Rows[rowIndex];
            HiddenField hfd_UserRoleId = (row.FindControl("hfd_UserRoleId") as HiddenField);
            HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
            HiddenField hfd_RoleId = (row.FindControl("hfd_RoleId") as HiddenField);
            HiddenField hdf_EmpId = (row.FindControl("hdf_EmpId") as HiddenField);

            ViewState["BussIdEditRole"] = hfd_BussinessId.Value;
            ViewState["hfd_RoleId"] = hfd_RoleId.Value;
            ViewState["hfd_UserRoleId"] = hfd_UserRoleId.Value;
            ViewState["hdf_EmpId"] = hdf_EmpId.Value;
            ddlEditRolePer.SelectedValue = hfd_UserRoleId.Value;
            GetUserPermissionRoleList(0, Gv_EditRolePop);
            ModallEditRolePop.Show();
            Label33.Text = "";
        }
        if (e.CommandName == "DeletePermission")
        {
            var control = (Control)e.CommandSource;
            if (Request.Form["anyminute_assignment_delete"] != control.ClientID) return;
            try
            {
                var row = (GridViewRow)control.NamingContainer;
                int business = int.Parse(((HiddenField)row.FindControl("hfd_BussinessId")).Value);
                int employee = int.Parse(((HiddenField)row.FindControl("hdf_EmpId")).Value);
                if (employee <= 0) throw new InvalidOperationException("Save the user before removing a business assignment.");
                AnyminuteEmployeeRepair.RemoveBusinessAssignment(Convert.ToInt32(Session["EmployeeId"]),
                    Convert.ToInt32(Session["UserTypeid"]), business, employee);
                // Reflect success only after the transaction commits.
                foreach (string key in new string[] { "RolePermission", "ProjetName", "rateDetails" })
                {
                    var data = ViewState[key] as DataTable;
                    if (data != null && data.Columns.Contains("BussinessId"))
                        foreach (DataRow item in data.Select("BussinessId=" + business)) data.Rows.Remove(item);
                }
                BindGrid(); BindProjectGrid(); BindRate();
                ddlBusAssProject.Items.Clear();
                ddlBusAssProject.Items.Add(new ListItem("Please Select", ""));
                ddlBusAssProject.DataSource = ViewState["RolePermission"];
                ddlBusAssProject.DataValueField = "BussinessId";
                ddlBusAssProject.DataTextField = "BusinessName";
                ddlBusAssProject.DataBind();
                EmployeeRepairMessage("Business assignment removed. Attendance history, address and pay records were retained.");
            }
            catch (InvalidOperationException ex) { EmployeeRepairMessage(ex.Message); }
            catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("Remove business assignment failed", ex);
                EmployeeRepairMessage("The assignment was not removed. Refresh and try again or contact support."); }
            return;
        }
        if (e.CommandName == "EmployeeHistory")
        {

            /*int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = Gv_RolePermission.Rows[rowIndex];
            HiddenField hdf_EmpId = (row.FindControl("hdf_EmpId") as HiddenField);
            //this.BindGrid();*/
            var historyRow = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            showEmployeeHistory(int.Parse(e.CommandArgument.ToString()), int.Parse(((HiddenField)historyRow.FindControl("hfd_BussinessId")).Value));
        }


    }

    protected void Gv_RolePermission_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        ImageSave();
        GridViewRow row = Gv_RolePermission.Rows[e.RowIndex];
        HiddenField hfd_UserRoleIdEdit = (HiddenField)row.FindControl("hfd_UserRoleIdEdit");
        TextBox txt_UserRole = (TextBox)row.FindControl("txt_UserRole");

        if (txt_UserRole.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Role Name', 'warning');", true);
            return;
        }

        bool chk_userrolename = comm.IfExistsInTable("UserType", "UserType", "UserType='" + txt_UserRole.Text + "' and BownerID=" + int.Parse(Session["EmployeeId"].ToString()) + " ");
        if (chk_userrolename)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'This Role Name Already Exists', 'warning');", true);
            return;
        }

        comm.ExecuteQuery("update UserType set UserType='" + txt_UserRole.Text + "' where User_TypeId=" + hfd_UserRoleIdEdit.Value + "");

        DataTable dt = (DataTable)ViewState["RolePermission"];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["UserRoleId"].ToString() == hfd_UserRoleIdEdit.Value)
            {
                dt.Rows[i]["UserRole"] = txt_UserRole.Text;
            }
        }
        dt.AcceptChanges();

        ViewState["RolePermission"] = dt;
        Gv_RolePermission.EditIndex = -1;
        ViewState["BGridEdit"] = false;
        this.BindGrid();
        BindAllUserType();
        // ddlRole.SelectedValue = hfd_UserRoleIdEdit.Value;

        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Role Updated Successfull', 'success');", true);
    }

    protected void ddlBusAssProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        if (!string.IsNullOrEmpty(ddlBusAssProject.SelectedValue))
        {
            DisableProject();

            GetProject();
        }
    }
    public void GetProject()
    {
        DataTable dtProject = comm.GetAllDataFromTable("AddProject", "ProjectID,ProjectName", "BusinessID=" + ddlBusAssProject.SelectedValue + " and Status='A'", "ProjectID");
        ddlAssProjectName.Items.Clear();
        ddlAssProjectName.DataSource = dtProject;
        ddlAssProjectName.Items.Insert(0, new ListItem("Select Project ", ""));
        ddlAssProjectName.Items.Insert(1, new ListItem("Add New Project ", "0"));

        ddlAssProjectName.DataValueField = dtProject.Columns[0].ToString();
        ddlAssProjectName.DataTextField = dtProject.Columns[1].ToString();
        ddlAssProjectName.DataBind();
        ddlAssProjectName.SelectedValue = "";
        if (dtProject.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'No Project Found', 'warning');", true);
        }
    }
    protected void ddlAssProjectName_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        if (ddlAssProjectName.SelectedValue == "0")
        {
            lblerror.Text = "";
            lblerror1.Text = "";
            ModelAddNewProject.Show();
        }
    }

    protected void btnAssProject_Click(object sender, EventArgs e)
    {
        ImageSave();
        if (ddlBusAssProject.SelectedValue == "" || ddlAssProjectName.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Select Business And Assign Project', 'info');", true);
            return;
        }

        DisableProject();

        int cnt = 0;
        DataTable dt = (DataTable)ViewState["ProjetName"];
        DataTable dtRolePermission = (DataTable)ViewState["RolePermission"];
        var result =
         dtRolePermission.AsEnumerable().Where(dr => dr.Field<int>("BussinessId") == int.Parse(ddlBusAssProject.SelectedValue)).CopyToDataTable();
        var empid = result.Rows[0]["EmployeeID"].ToString();
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusAssProject.SelectedItem.Text;
        var projectName = ddlAssProjectName.SelectedItem.Text;
        var bussId = ddlBusAssProject.SelectedValue;
        var cntNoBUSS = 0;
        foreach (GridViewRow row in GvProject.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            if (bussId == hfd_BussinessId.Value)
            {
                cntNoBUSS++;
            }
        }

        if (empid != "0")
        {
            bool checkemptype = comm.IfExistsInTable("addemployee", "*", "EMPLOYEEID=" + empid + " and EMPLOYEETYPE='R'");
            if (checkemptype == true)
            {
                bool dtcheck = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + empid + " and TimeSheetDate='" + System.DateTime.Now.Date.AddDays(-1) + "' " +
                    "and BussID=" + bussId + "");
                if (dtcheck == false)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Time Sheet in Not Up To Date', 'info');", true);
                    return;
                }
            }

            if (cntNoBUSS == 0)
            {
                bool checkproject = comm.IfExistsInTable("ASSIGNPROJECT", "*", "EMPLOYEEID=" + empid + " and BUSINESSID=" + bussId + " " +
                    "and PROJECTID=" + ddlAssProjectName.SelectedValue + "");
                if (checkproject == true)
                {
                    bool dtcheck = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + empid + " and TimeSheetDate='" + System.DateTime.Now.Date.AddDays(-1) + "' " +
                        "and BussID=" + bussId + " and ProjectId='" + ddlAssProjectName.SelectedValue + "'");
                    if (dtcheck == false)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Time Sheet in Not Up To Date', 'info');", true);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["BussinessId"].ToString().Contains(ddlBusAssProject.SelectedValue) && dt.Rows[i]["ProjectId"].ToString().Contains(ddlAssProjectName.SelectedValue))
            {
                cnt++;
            }
        }
        if (cnt > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Duplicate Project and Business', 'warning');", true);
            return;
        }


        DateTime ctdate = DateTime.Now.Date;
        bool dtchecktime = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + empid + "  and BussID ='" + ddlBusAssProject.SelectedValue + "' " +
            "and TimeSheetDate>='" + ctdate + "'");
        ViewState["AdvEmpid"] = empid;
        ViewState["AdvBussid"] = ddlBusAssProject.SelectedValue.ToString();
        if (dtchecktime == true)
        {
            ViewState["Projectid"] = ddlBusAssProject.SelectedValue;
            dt.Rows.Add(bussname, ddlBusAssProject.SelectedValue, EmpName, 0, 0, 0, Convert.ToInt32(ddlAssProjectName.SelectedValue), projectName);
            ViewState["ProjetName"] = dt;

            bool check_project_assigned = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + empid + "  and BussID ='" + ddlBusAssProject.SelectedValue + "' " +
                "and TimeSheetDate>='" + ctdate + "' and ProjectId='" + ddlAssProjectName.SelectedValue + "'");
            if (check_project_assigned)
            {
                lblConfirm.Text = "Advance Time Sheet from " + ctdate.ToString("dd-MMM-yyyy") + " will be Deleted, Do you Want to Continue?";
                ModelConfirm.Show();
                return;
            }

            bool check_reg_emp_type = comm.IfExistsInTable("addemployee with (Nolock)", "EMPLOYEEID", "EMPLOYEEID=" + empid + " and EMPLOYEETYPE='R'");
            if (check_reg_emp_type)
            {
                lblConfirm.Text = "Advance Time Sheet from " + ctdate.ToString("dd-MMM-yyyy") + " will be Deleted, Do you Want to Continue?";
                ModelConfirm.Show();
                return;
            }
        }
        else
        {
            ViewState["Projectid"] = ddlBusAssProject.SelectedValue;
            dt.Rows.Add(bussname, ddlBusAssProject.SelectedValue, EmpName, 0, 0, 0, Convert.ToInt32(ddlAssProjectName.SelectedValue), projectName);
            ViewState["ProjetName"] = dt;
        }

        BindProjectGrid();
    }

    protected void GvProject_RowEditing(object sender, GridViewEditEventArgs e)
    {
        ImageSave();
        GvProject.EditIndex = e.NewEditIndex;
        ViewState["PGvProjectEdit"] = true;
        BindProjectGrid();
    }

    protected void GvProject_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        ImageSave();
        GvProject.EditIndex = -1;
        ViewState["PGvProjectEdit"] = false;
        BindProjectGrid();
    }

    protected void GvProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfd_ProjectNameId = (e.Row.FindControl("hfd_ProjectNameId") as HiddenField);
            HiddenField hfd_roleId = (e.Row.FindControl("hfd_roleId") as HiddenField);
            LinkButton lnk_ChangeRoleName = (e.Row.FindControl("lnk_ChangeRoleName") as LinkButton);
            LinkButton lnk_deleteProject = (e.Row.FindControl("lnk_deleteProject") as LinkButton);
            Label lbl_separate3 = (e.Row.FindControl("lbl_separate3") as Label);

            bool PGvProjectEdit = (bool)ViewState["PGvProjectEdit"];
            if (PGvProjectEdit == true)
            {
                lnk_ChangeRoleName.Enabled = false;
                if (lnk_deleteProject != null)
                {
                    lnk_deleteProject.Enabled = false;
                }
                if (lbl_separate3 != null)
                {
                    lbl_separate3.Enabled = false;
                }
            }
            else
            {
                lnk_ChangeRoleName.Enabled = true;
                if (lnk_deleteProject != null)
                {
                    lnk_deleteProject.Enabled = true;
                }
                if (lbl_separate3 != null)
                {
                    lbl_separate3.Enabled = true;
                }
            }


            #region Kumar code for role permissions, cancel plan, deactivate subscriber reseller by support admin
            HiddenField hfd_EmployeeIDpro = (e.Row.FindControl("hfd_EmployeeIDpro") as HiddenField);
            HiddenField hfd_BussinessId = (e.Row.FindControl("hfd_BussinessId") as HiddenField);
            if (Session["UserTypeid"].ToString() == "1")
            {
                if (hfd_EmployeeIDpro.Value != "0")
                {
                    //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                    //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                    //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                    //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                    var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hfd_EmployeeIDpro.Value) + "'", "");
                    if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        e.Row.Enabled = false;
                    }
                }
            }
            if (Session["UserTypeid"].ToString() != "1")
            {
                DataTable Get_Reseller_Subscriber_ID = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and UserTypeId=1", "");

                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "'", "");

                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=0 ");

                    var check_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    if (check_subscriber_cancelled.Rows.Count > 0 || check_subscriber_plan_deactivated.Rows.Count > 0
                        || check_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        if (hfd_EmployeeIDpro.Value != "0")
                        {
                            e.Row.Enabled = false;
                        }
                    }

                    if (hfd_EmployeeIDpro.Value != "0")
                    {
                        //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                        //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                        //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                        //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hfd_EmployeeIDpro.Value) + "'", "");
                        if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                        {
                            e.Row.Enabled = false;
                        }
                    }
                }

                if (check_reseller_or_subscriber_email)
                {
                    var check_reseller_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and Is_Cancelled=1 ");

                    var check_reseller_fully_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_reseller_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' " +
                        "and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and IsActive=0 ");

                    var check_reseller_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    if (check_reseller_subscriber_cancelled.Rows.Count > 0 || check_reseller_fully_cancelled.Rows.Count > 0
                        || check_reseller_subscriber_plan_deactivated.Rows.Count > 0 || check_reseller_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        if (hfd_EmployeeIDpro.Value != "0")
                        {
                            e.Row.Enabled = false;
                        }
                    }

                    if (hfd_EmployeeIDpro.Value != "0")
                    {
                        //var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                        //    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                        //var Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfd_BussinessId.Value) + "' " +
                        //    "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(hfd_EmployeeIDpro.Value) + "'", "");
                        if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                        {
                            e.Row.Enabled = false;
                        }
                    }
                }
            }
            #endregion
        }
    }

    protected void GvProject_PreRender(object sender, EventArgs e)
    {

    }

    protected void GvProject_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        ImageSave();
        if (e.CommandName == "DeleteProject")
        {
            string confirmValue = Request.Form["confirm_value"];
            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = GvProject.Rows[rowIndex];
            HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
            HiddenField hfd_ProjectNameId = (row.FindControl("hfd_ProjectNameId") as HiddenField);
            HiddenField hfd_EmployeeIDpro = (row.FindControl("hfd_EmployeeIDpro") as HiddenField);
            DataTable dt = (DataTable)ViewState["ProjetName"];
            ViewState["Projectid"] = hfd_ProjectNameId.Value;
            StringBuilder proId = new StringBuilder();
            if (confirmValue == "Yes")
            {
                DateTime ctdate = DateTime.Now.Date;
                bool dtchecktime = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + hfd_EmployeeIDpro.Value + "  and BussID ='" + hfd_BussinessId.Value.ToString() + "' " +
                    "and TimeSheetDate>='" + ctdate + "' and ProjectId=" + hfd_ProjectNameId.Value + "");
                ViewState["AdvEmpid"] = hfd_EmployeeIDpro.Value.ToString();
                ViewState["AdvBussid"] = hfd_BussinessId.Value.ToString();
                if (dtchecktime == true)
                {
                    lblConfirm.Text = "Advance Time Sheet from " + ctdate.ToString("dd-MMM-yyyy") + " will be Deleted, Want to Continue?";
                    ModelConfirm.Show();
                    return;
                }
                if (hfd_ProjectNameId != null)
                {
                    DataTable dt_check_if_admin_acc = comm.GetAllDataFromTable("MappingTable with (Nolock)", "UserTypeId", "BusinessID=" + hfd_BussinessId.Value + " and " +
                            "EmployeeID='" + hfd_EmployeeIDpro.Value + "'", "");
                    if (dt_check_if_admin_acc.Rows.Count > 0)
                    {
                        if (dt_check_if_admin_acc.Rows[0]["UserTypeId"].ToString() != "5" && dt_check_if_admin_acc.Rows[0]["UserTypeId"].ToString() != "12")
                        {
                            bool check = comm.IfExistsInTable("ASSIGNPROJECT", "*", "BUSINESSID=" + hfd_BussinessId.Value + " and PROJECTID in (" + hfd_ProjectNameId.Value + ") " +
                        "and EMPLOYEEID='" + hfd_EmployeeIDpro.Value + "'");
                            if (check)
                            {
                                DataTable dt_check_if_project_timesheet_entered_or_not = comm.GetAllDataFromTable("TimeSheetEntry", "distinct ProjectId," +
                                    "dbo.FN_DESC('Project'," + hfd_ProjectNameId.Value + ")projname", "BussID=" + hfd_BussinessId.Value + " " +
                                    "and ProjectId in (" + hfd_ProjectNameId.Value + ") and TimeSheetDate='" + DateTime.Now.Date.AddDays(-1) + "' " +
                                    "and EmpId='" + hfd_EmployeeIDpro.Value + "'", "");
                                if (dt_check_if_project_timesheet_entered_or_not.Rows.Count == 0)
                                {
                                    var project_name = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectID in (" + hfd_ProjectNameId.Value + ")", "");
                                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Cannot Remove Project, Time Sheet not Up to Date for Project " + project_name.Rows[0]["ProjectName"].ToString() + "', 'error');", true);
                                    return;
                                }
                            }
                        }
                    }

                    #region amit code is commented as not staisfying all conditions
                    //string StrproId = proId.ToString().TrimEnd(',');
                    //bool check = comm.IfExistsInTable("ASSIGNPROJECT", "*", "BUSINESSID=" + hfd_BussinessId.Value + " and PROJECTID in (" + hfd_ProjectNameId.Value + ") " +
                    //    "and EMPLOYEEID='" + hfd_EmployeeIDpro.Value + "'");
                    //if (check == true)
                    //{

                    //    bool TimeSheetCheck = false;
                    //    for (int i = 0; i < dt.Rows.Count; i++)
                    //    {
                    //        DataTable dtprojchk = comm.GetAllDataFromTable("TimeSheetEntry", "distinct ProjectId," +
                    //            "dbo.FN_DESC('Project'," + dt.Rows[i]["ProjectId"].ToString() + ")projname", "BussID=" + hfd_BussinessId.Value + " " +
                    //            "and ProjectId in (" + dt.Rows[i]["ProjectId"].ToString() + ") and TimeSheetDate='" + DateTime.Now.Date.AddDays(-1) + "' " +
                    //            "and EmpId='" + hfd_EmployeeIDpro.Value + "'", "");
                    //        if (dtprojchk.Rows.Count > 0)
                    //        {
                    //            TimeSheetCheck = true;
                    //            break;
                    //        }
                    //        else
                    //        {
                    //            proId.Append(dt.Rows[i]["ProjectName"].ToString());
                    //            proId.Append(",");
                    //            TimeSheetCheck = false;
                    //        }
                    //    }

                    //    if (TimeSheetCheck == false)
                    //    {
                    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Cannot Remove Project, Time Sheet not Up to Date for Project " + proId.ToString() + "', 'error');", true);
                    //        return;
                    //    }
                    //}
                    #endregion

                    DataRow[] rowsToRemove = dt.Select("BussinessId=" + hfd_BussinessId.Value + " and ProjectId=" + hfd_ProjectNameId.Value + "and EmployeeID=" + hfd_EmployeeIDpro.Value);
                    foreach (var rowToDelete in rowsToRemove)
                        dt.Rows.Remove(rowToDelete);
                    ViewState["ProjetName"] = dt;
                    comm.ExecuteQuery("delete from ASSIGNPROJECT where BUSINESSID=" + hfd_BussinessId.Value + " and PROJECTID=" + hfd_ProjectNameId.Value + " " +
                        "and EMPLOYEEID='" + hfd_EmployeeIDpro.Value + "'");

                    int result = comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + hfd_EmployeeIDpro.Value + "  and BussID = '" + hfd_BussinessId.Value + "' " +
                        "and TimeSheetDate>= '" + ctdate + "' and ProjectId=" + hfd_ProjectNameId.Value);
                    this.BindProjectGrid();
                    bool cchkAssproject = comm.IfExistsInTable("ASSIGNPROJECT", "PROJECTID", "EMPLOYEEID='" + hfd_EmployeeIDpro.Value + "' " +
                        "and BUSINESSID=" + hfd_BussinessId.Value + "");
                    if (cchkAssproject == false)
                    {
                        comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE ='R' WHERE EmployeeID='" + hfd_EmployeeIDpro.Value + "'");

                        //added by kumar to delete all the timesheet that is entered for today and future dates even for the regular hours that is entered from project timesheet screen
                        comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + hfd_EmployeeIDpro.Value + "  and BussID = '" + hfd_BussinessId.Value + "' " +
                        "and TimeSheetDate>= '" + ctdate + "'");
                    }
                }
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Cannot Remove Project, Please Cancel Row Editing', 'error');", true);
                }
            }
        }
    }

    protected void GvProject_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //ImageSave();
        GridViewRow row = GvProject.Rows[e.RowIndex];
        HiddenField hfd_ProjectNameIdEdit = (HiddenField)row.FindControl("hfd_ProjectNameIdEdit");
        HiddenField hfd_BussinessId = (HiddenField)row.FindControl("hfd_BussinessId");
        TextBox txt_ProjectName = (TextBox)row.FindControl("txt_ProjectName");

        if (txt_ProjectName.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Enter Project Name', 'error');", true);
            return;
        }
        //DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject", "ProjectName", "ProjectName='" + txt_ProjectName.Text + "' and Status='A' ", "");
        DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject", "ProjectName", "ProjectName='" + txt_ProjectName.Text + "' and Status='A' " +
            "and BusinessID=" + hfd_BussinessId.Value + " ", "");
        if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Project Already Exists', 'error');", true);
            return;
        }
        comm.ExecuteQuery("update AddProject set ProjectName='" + txt_ProjectName.Text + "' where ProjectID=" + hfd_ProjectNameIdEdit.Value + "");
        if (!string.IsNullOrEmpty(ddlBusAssProject.SelectedValue))
        {
            GetProject();
        }
        DataTable dt = (DataTable)ViewState["ProjetName"];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["ProjectID"].ToString() == hfd_ProjectNameIdEdit.Value)
            {
                dt.Rows[i]["ProjectName"] = txt_ProjectName.Text;
            }
        }
        dt.AcceptChanges();

        GvProject.EditIndex = -1;
        ViewState["ProjetName"] = dt;
        ViewState["PGvProjectEdit"] = false;
        this.BindProjectGrid();

        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Project Updated Successfully', 'success');", true);
    }

    #region employee edit region
    public void GetChangeRateRole(int EmployeeID)
    {//Venk - 08/29/2025
     ////This will check the login user has been assigned in which business in what roles with the employee id of same login user
        /*  DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and UserTypeId='5' and BusinessId = '" + Request.QueryString["BussEditId"].ToString() + "'", "UserTypeId");

          if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
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
                              //btnrateHistoryChangeRate.Enabled = false;
                //              ddlBillingType.Enabled = false;
                //              txtBillRate.Enabled = false;
                          }
                          else
                          {
                              //btnrateHistoryChangeRate.Enabled = true;
              //                ddlBillingType.Enabled = true;
              //                txtBillRate.Enabled = true;
                          }
                          string IsExistsCheckAssignPermissionRole = Convert.ToString(dtPersonalDetailsForEdit.Rows[0]["IsExistsCheckAssignPermissionRole"]);
                          if (IsExistsCheckAssignPermissionRole == "N")
                          {
                              //DropDownList1122.Enabled = false;
            //                  GridView_User_Role_Permission.Enabled = false;
                          }
                          else
                          {
                              //DropDownList1122.Enabled = true;
          //                    GridView_User_Role_Permission.Enabled = true;
                          }
                      }
                  }
              }
          }*/
    }

    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        DataTable dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*,convert(nvarchar(10),HireDate,120)HireDated", "EmployeeID=" + EmployeeID + "", "");
        //foreach (ListItem itm in rdoBtnUserTypeList.Items)
        //{
        //    if (itm.Value == dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString())
        //    {
        //        itm.Selected = true;
        //    }
        //}

        if (dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString() != "")
            txtSINExpiry.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString()).ToString("yyyy-MM-dd");

        txtFirstName.Text = dtPersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dtPersonalDetailsForEdit.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dtPersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        hfdSocialEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["SocialInsuranceNumber"].ToString();
        //ddlEmpEI.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        //ddlEmpCPP.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        //ddlRTVA.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["RetainVacation"].ToString().Trim();

        if (dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString() != "")
        {
            hfddboEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString();
        }
        txtSocialNumer.Text = comm.Decrypt(hfdSocialEncrpty.Value);
        string dbo = comm.Decrypt(hfddboEncrpty.Value);
        if (dbo != "")
        {
            txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
        }

        ddlgender.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["Gender"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString() != "")
        {
            DateTime HireDate = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["HireDated"].ToString());
            txtHiredate.Text = HireDate.ToString("yyyy-MM-dd");
        }

        //if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
        //txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        //ddlEmpStatus.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsActive"].ToString().Trim();
        if (ddlEmpStatus.SelectedValue == "NO")
        {
            //if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
            //txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        }
        if (dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            divAddres.Attributes.Add("style", "display:block;");
            GetAddressDetails(int.Parse(dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        }
        if (dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString() != "")
        {
            GetPaymentForEdit(int.Parse(dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString()));
        }

        //GetRateInformation(hfdEmpID.Value);
    }
    public void ImageSave()
    {
        string str = "";
        if (fupd.HasFile)
        {
            var fileName = Path.GetFileName(fupd.PostedFile.FileName);
            var fileExt = Path.GetExtension(fileName);
            str = string.Format("{0}{1}",
                                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                        fileExt);
            fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
            previewImage.ImageUrl = "~/Uploads/" + str;
        }
        ViewState["profile"] = str;
    }
    public void GetPaymentForEdit(int Rateid)
    {
        DataTable dtRateDetailsForEdit = new DataTable();
        dtRateDetailsForEdit = comm.GetAllDataFromTable("RateDetails", "*", "RateId=" + Rateid + " and EmployeeId=" + ddl_Employee.SelectedValue + "", "");
        //txtBillRate.Text = dtRateDetailsForEdit.Rows[0]["Rate"].ToString();
        //ddlBillingType.SelectedValue = dtRateDetailsForEdit.Rows[0]["RateType"].ToString();
    }

    public void Get_User_Role_Permission_While_Editing_Employee(int EmployeeID)
    {
        //DataTable Dt_Check_Role_Permission = new DataTable();
        //Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["EmployeeIDEdit"].ToString() + "' ", "");
        ////Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
        //if (Dt_Check_Role_Permission.Rows.Count > 0)
        //{
        //    foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
        //    {
        //        string string_Check_Role_Permission = Dt_Check_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //        string[] splittedArray = string_Check_Role_Permission.Split(',');
        //        HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
        //        DropDownList ddl_Role_Permission_Access_Status_ = (DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status");
        //        foreach (string s in splittedArray)
        //        {
        //            if (Role_Permission_Id_.Value == s)
        //            {
        //                ddl_Role_Permission_Access_Status_.SelectedValue = "YES";
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    //Div_User_Role_Permission.Visible = false;
        //}
    }

    protected void GetRateInformation(string empidd)
    {
        //DataTable dtname = new DataTable();
        //int empid = 0;
        //if (hfdEmpID.Value == "")
        //{
        //    empid = 0;
        //}
        //else
        //{
        //    empid = int.Parse(hfdEmpID.Value);
        //}
        //hfdchangerateid.Value = empid.ToString();
        //dtname = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + empid + "", "");
        //if (dtname.Rows.Count > 0)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("<b>Employee Name: </b>" + lblEmpNameForAddressCont.Text);
        //    sb.AppendLine("<b>Employee Status: </b>" + dtname.Rows[0]["IsActive"].ToString());
        //    // lblNameemp.Text = "Employee Name: " + lblEmpNameForAddressCont.Text + "\n" + "Is Active :" + dtname.Rows[0]["IsActive"].ToString() + "";
        //    lblNameempRateHistory.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
        //    DataTable dtratede = comm.GetAllDataFromTable("RateDetails", "*,convert(Nvarchar(10),effectivedate,120)effectivedateconv", "RateId='" + dtname.Rows[0]["Rateid"].ToString() + "'", "");
        //    if (dtratede.Rows.Count > 0)
        //    {
        //        DateTime mindate = DateTime.ParseExact(dtratede.Rows[0]["effectivedateconv"].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture);
        //        txtrateHistoryEffectiveDate.Attributes.Add("min", mindate.ToString());
        //    }
        //}
        //DataTable dtminrate = comm.GetAllDataFromTable("RateDetails", "MIN(rateid) rateid,Rate,RateType", "EmployeeId=" + empid + " group by Rate,RateType", "");
        //if (dtminrate.Rows.Count > 0)
        //{
        //    ddlBillingType.SelectedValue = dtminrate.Rows[0]["RateType"].ToString();
        //    txtBillRate.Text = dtminrate.Rows[0]["Rate"].ToString();
        //}

        //gvRateHistory.DataSource = RateCtrl.GetRateHistory(empid);
        //gvRateHistory.DataBind();

        //if (gvRateHistory.Rows.Count == 1)
        //{
        //    gvRateHistory.Rows[0].Cells[6].Visible = false;
        //}

        //ModalPopupEmpList.Show();
    }

    public void GetAddressDetails(int EmpAddressId)
    {
        DataTable dtAddressDetailForEdit = new DataTable();
        dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + EmpAddressId + "", "");

        txtSearchPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        txtStreetNo.Text = dtAddressDetailForEdit.Rows[0]["StreetNo"].ToString();
        txtStreetName.Text = dtAddressDetailForEdit.Rows[0]["StreetName"].ToString().ToUpper();

        txtStreetType.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtSuiteNumber.Text = dtAddressDetailForEdit.Rows[0]["SuiteNumber"].ToString().ToUpper();
        txtStreetDirection.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();

        DropDownList2.SelectedIndex = DropDownList2.Items.IndexOf(DropDownList2.Items.FindByValue(dtAddressDetailForEdit.Rows[0]["SuitType"].ToString()));

        txtCity.Text = dtAddressDetailForEdit.Rows[0]["City"].ToString().ToUpper();
        txtProvince.Text = dtAddressDetailForEdit.Rows[0]["Province"].ToString().ToUpper();
        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();

        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtMobile.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();

        //txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        //string trimeedvalue = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString().Remove(0, 15);
        string trimeedvalue = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        string substremail = trimeedvalue.Substring(trimeedvalue.Length - 5);
        if (substremail != "blank")
        {
            txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
            txtMainEmail.ReadOnly = true;
            txtConLoginEmail.ReadOnly = true;
        }
        if (substremail == "blank")
        {
            txtMainEmail.Text = "";
            txtConLoginEmail.Text = "";
        }

        //txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        //txtMainEmail.ReadOnly = true;
        //txtConLoginEmail.ReadOnly = true;
    }
    #endregion

    public void GetUserPermissionRoleList(int uid, GridView GvDataGridView)
    {
        if (uid != 0)
        {
            comm.FillGridView(GvDataGridView, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status,UserTypeId", "UserTypeId=" + uid + "", "Role_Permission_Id asc");
        }
        else
        {
            comm.FillGridView(GvDataGridView, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status,UserTypeId", "", "Role_Permission_Id asc");
        }
    }

    protected void btnSaveProject_Click(object sender, EventArgs e)
    {
        ImageSave();
        if (this.txtAddProject.Text.Trim() == string.Empty)
        {
            lblerror.Text = "Project Name is Requried";
            lblerror.Visible = true;
            txtAddProject.Focus();
            ModelAddNewProject.Show();
        }
        if (this.txtAddProject.Text.Trim() != string.Empty)
        {
            lblerror.Text = "";
            lblerror.Visible = false;
            ModelAddNewProject.Show();
        }
        if (this.txtProjectStDate.Text.Trim() == string.Empty)
        {
            lblerror1.Text = "Start Date is Requried";
            lblerror1.Visible = true;
            ModelAddNewProject.Show();
        }
        if (this.txtProjectStDate.Text.Trim() != string.Empty)
        {
            lblerror1.Text = "";
            lblerror1.Visible = false;
            ModelAddNewProject.Show();
        }
        if (txtProjectStDate.Text.Trim().Length > 0 && txtProjectEndDate.Text.Trim().Length > 0)
        {
            DateTime dtfrom = DateTime.Parse(txtProjectStDate.Text);
            DateTime dtto = DateTime.Parse(txtProjectEndDate.Text);
            if (dtfrom > dtto)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Start date is greater than End date!')", true);
                ModelAddNewProject.Show();
                return;
            }
        }

        //DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject", "ProjectName", "ProjectName='" + txtAddProject.Text + "' and Status='A' ", "");
        DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject", "ProjectName", "ProjectName='" + txtAddProject.Text + "' and Status='A' " +
            "and BusinessID=" + ddlBusAssProject.SelectedValue + " ", "");
        if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Project Already Exists', 'error');", true);
            Labelsameproject.Text = "Project Already Exists";
            Labelsameproject.Visible = true;
            txtAddProject.Focus();
            ModelAddNewProject.Show();
            return;
        }

        //if (this.txtAddProject.Text.Trim() == string.Empty)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Project Name', 'warning');", true);
        //    return;
        //}
        //if (this.txtProjectStDate.Text.Trim() == string.Empty)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Start Date', 'warning');", true);
        //    return;
        //}
        //if (txtProjectStDate.Text.Trim().Length > 0 && txtProjectEndDate.Text.Trim().Length > 0)
        //{
        //    DateTime dtfrom = DateTime.Parse(txtProjectStDate.Text);
        //    DateTime dtto = DateTime.Parse(txtProjectEndDate.Text);
        //    if (dtfrom > dtto)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Start date is greater than End date!')", true);
        //        return;
        //    }
        //}
        if (ddlBusAssProject.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business', 'warning');", true);
            return;
        }
        if (txtAddProject.Text != string.Empty && this.txtProjectStDate.Text.Trim() != string.Empty
            && ddlBusAssProject.SelectedValue != "")
        {
            comm.ExecuteQuery("insert into AddProject(ProjectName,startDate,EndDate,BusinessID,Status)values('" + txtAddProject.Text + "','" + txtProjectStDate.Text + "','" + txtProjectEndDate.Text + "'," + ddlBusAssProject.SelectedValue + ",'A')");
            GetProject();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Project Added Successfully', 'success');", true);
            txtAddProject.Text = "";
            txtProjectStDate.Text = "";
            txtProjectEndDate.Text = "";
            lblerror.Visible = false;
            Labelsameproject.Visible = false;
            lblerror1.Visible = false;
            ModelAddNewProject.Hide();
        }
    }

    protected void btnsaveRolePop_Click(object sender, EventArgs e)
    {
        ImageSave();
        if (ddlBusiness.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business', 'warning');", true);
            ModalAddRolePop.Show();
            return;
        }
        if (txtRoleNamePop.Text == "")
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Role', 'warning');", true);
            Label22.Text = "Enter Role";
            Label22.Visible = true;
            ModalAddRolePop.Show();
            return;
        }
        if (this.txtRoleNamePop.Text.Trim() != string.Empty)
        {
            Label22.Text = "";
            Label22.Visible = false;
            //ModalAddRolePop.Hide();
        }
        StringBuilder stringbuilder__ = new StringBuilder();
        stringbuilder__.Append(txtRoleNamePop.Text + ",");
        foreach (GridViewRow row in GridView_User_Role_Permission_Pop.Rows)
        {
            DropDownList DropDownList_User_Role_Permission_Access_Status = row.FindControl("DropDownList_User_Role_Permission_Access_Status") as DropDownList;
            HiddenField HiddenField_Role_Permission_Id = row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
            if (DropDownList_User_Role_Permission_Access_Status.SelectedValue == "YES")
            {

                stringbuilder__.Append(HiddenField_Role_Permission_Id.Value + ",");
                ViewState["RoleId_PopAdd"] = stringbuilder__.ToString();
            }
        }
        if (Convert.ToString(stringbuilder__) == "")
        {
            Label22.Text = "Select Permissions";
            Label22.Visible = true;
            ModalAddRolePop.Show();
            return;
        }

        bool chk_userrolename = comm.IfExistsInTable("UserType", "UserType", "UserType='" + txtRoleNamePop.Text + "' and BownerID=" + int.Parse(Session["EmployeeId"].ToString()) + " ");
        if (!chk_userrolename)
        {
            DataTable dt = comm.GetAllDataFromTable("AssginBussiness", "EmployeeID", "BusinessID='" + ddlBusiness.SelectedValue + "' and UserTypeId=1", "");
            comm.ExecuteQuery("insert into UserType(UserType,MotherOrgId,BownerID)values('" + txtRoleNamePop.Text + "',0," + int.Parse(dt.Rows[0][0].ToString()) + ")");
            BindAllUserType();
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Role Added Successfully', 'success');", true);
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'New Role has been Added Successfully, Please select the new role from the Assign Role dropdown and click Add button to Assign New Role', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'New Role has been Added Successfully to assign the user in business', 'success');", true);
            txtRoleNamePop.Text = "";
            Label22.Visible = false;
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'This Role Name Already Exists', 'warning');", true);
            Label22.Text = "This Role Name Already Exists";
            Label22.Visible = true;
            ModalAddRolePop.Show();
        }
    }

    protected void ddlEditRolePer_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        Label33.Text = "";
        Label33.Visible = false;
        ViewState["Event"] = true;
        GetUserPermissionRoleList(0, Gv_EditRolePop);
    }

    protected void btnSaveEditRolePop_Click(object sender, EventArgs e)
    {
        ImageSave();
        StringBuilder StringBuilder__ = new StringBuilder();
        foreach (GridViewRow row in Gv_EditRolePop.Rows)
        {
            DropDownList DropDownList_User_Role_Permission_Access_Status_Edit = row.FindControl("DropDownList_User_Role_Permission_Access_Status_Edit") as DropDownList;
            HiddenField HiddenField_Role_Permission_Id = row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
            if (DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue == "YES")
            {
                StringBuilder__.Append(HiddenField_Role_Permission_Id.Value + ",");
            }
        }
        if (ViewState["hfd_UserRoleId"].ToString() != ddlEditRolePer.SelectedValue)
        {
            if (ViewState["hfd_UserRoleId"].ToString() != "5" && ViewState["hfd_UserRoleId"].ToString() != "12")
            {
                if (ViewState["hdf_EmpId"].ToString() != "0")
                {
                    //  bool dtchecktime= comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ViewState["hdf_EmpId"].ToString() + "  and BussID ='" + ViewState["BussIdEditRole"].ToString() + "'");
                    bool dtcheck = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ViewState["hdf_EmpId"].ToString() + " and TimeSheetDate='" + System.DateTime.Now.Date.AddDays(-1) + "'  and BussID ='" + ViewState["BussIdEditRole"].ToString() + "'");
                    if (dtcheck == false)//&& dtchecktime==true
                    {
                        Label33.Text = "Time Sheet is not Update to date";
                        Label33.Visible = true;
                        ModallEditRolePop.Show();
                        return;
                    }
                    DateTime ctdate = DateTime.Now.Date;
                    bool dtchecktime = comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ViewState["hdf_EmpId"].ToString() + "  and BussID ='" + ViewState["BussIdEditRole"].ToString() + "' and TimeSheetDate>='" + ctdate + "'");
                    if (dtchecktime == true)
                    {

                        comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + ViewState["hdf_EmpId"].ToString() + "  and BussID = '" + ViewState["BussIdEditRole"].ToString() + "' and TimeSheetDate>= '" + ctdate + "'");
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Advance Time Sheet from " + ctdate.ToString("dd-MMM-yyyy") + " is Deleted', 'info');", true);
                    }
                }
            }
        }

        Label33.Text = "";
        Label33.Visible = false;

        if (string.IsNullOrEmpty(StringBuilder__.ToString()))
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Select Atleast One Role', 'error');", true);
            Label33.Text = "Select Atleast One Role";
            Label33.Visible = true;
            ModallEditRolePop.Show();
            return;
        }
        DataTable dt = (DataTable)ViewState["RolePermission"];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["BussinessId"].ToString() == ViewState["BussIdEditRole"].ToString())
            {
                dt.Rows[i]["RoleId"] = StringBuilder__.ToString();
                dt.Rows[i]["UserRole"] = ddlEditRolePer.SelectedItem.Text;
                dt.Rows[i]["UserRoleId"] = ddlEditRolePer.SelectedValue;
            }
        }
        dt.AcceptChanges();
        ViewState["RolePermission"] = dt;
        this.BindGrid();
        Label33.Text = "";
        Label33.Visible = false;

        DisableAdmAcc();
    }

    protected void Gv_EditRolePop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        ImageSave();
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdUserTypeId = e.Row.FindControl("hfdUserTypeId") as HiddenField;
            HiddenField HiddenField_Role_Permission_Id = e.Row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
            DropDownList DropDownList_User_Role_Permission_Access_Status_Edit = e.Row.FindControl("DropDownList_User_Role_Permission_Access_Status_Edit") as DropDownList;
            var selectUserTypid = ddlEditRolePer.SelectedValue;
            var ddlselect = ddlEditRolePer.SelectedValue;
            if (selectUserTypid == hfdUserTypeId.Value)
            {

                DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "YES";
            }
            if (ddlselect == "1" || ddlselect == "2" || ddlselect == "4" || ddlselect == "5" || ddlselect == "12")
            {
                DropDownList_User_Role_Permission_Access_Status_Edit.Enabled = false;

                if (HiddenField_Role_Permission_Id.Value == "31") //&& hfdUserTypeId.Value == "12")
                {
                    DropDownList_User_Role_Permission_Access_Status_Edit.Enabled = true;
                    DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "NO";
                }
            }
            else
            {

                DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "NO";
            }
            var ddevent = Convert.ToBoolean(ViewState["Event"]);
            if (ddevent == false)
            {
                var selectRole = ViewState["hfd_RoleId"].ToString();
                string[] r = selectRole.Split(',');
                for (int i = 0; i < r.Count(); i++)
                {
                    if (r[i] == HiddenField_Role_Permission_Id.Value)
                    {
                        DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "YES";
                    }
                }
            }
            string[] defaultuid = { "1", "2", "4", "5", "12" };
            bool isExistdefault = Array.Exists(defaultuid, x => x == ddlselect);
            if (isExistdefault == false)
            {
                string[] split = ViewState["RoleId_PopAdd"].ToString().Split(',');

                if (split[0].ToString() != ddlEditRolePer.SelectedItem.Text)
                {
                    string[] time = { "1", "2", "7", "8", "9", "18", "19", "20", "25", "26" };
                    bool isExist = Array.Exists(time, x => x == HiddenField_Role_Permission_Id.Value.ToString());
                    if (isExist)
                    {
                        DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "YES";
                    }
                }
            }
        }
    }

    protected void gvRateHistory_PreRender(object sender, EventArgs e)
    {

    }

    //protected void btnSavePay_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (txtBillRate.Text == "")
    //        {
    //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Billing Rate', 'warning');", true);
    //            return;
    //        }
    //        if (btnSavePay.Text != "Update Rate")
    //        {
    //            RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
    //            RateEntity.Rate = float.Parse(txtBillRate.Text);
    //            RateEntity.RateType = ddlBillingType.SelectedItem.Text;
    //            RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
    //            RateEntity.EndDate = System.DateTime.Now.Date;
    //            RateEntity.JobTitle = ddlJobTitle.SelectedValue;
    //            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
    //            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
    //            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
    //            RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
    //            RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));

    //            GetRateInformation(hfdEmpID.Value);
    //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);
    //        }
    //        else
    //        {
    //            comm.ExecuteQuery("update RateDetails set Rate =" + txtBillRate.Text + ", RateType='" + ddlBillingType.SelectedValue + "', StartDate='" + txteffectivedate.Text + "',effectivedate='" + txteffectivedate.Text + "' where RateId=" + hfdEditRateId.Value + "");
    //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Updated successfully', 'success');", true);
    //            btnSavePay.Text = "Save";
    //            hfdEditRateId.Value = "";
    //            GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
    //        }
    //        //ClearPaymentDetails();
    //    }
    //    catch
    //    {
    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Save Rate Details', 'error');", true);
    //    }
    //}

    //protected void btnrateHistoryChangeRate_Click(object sender, EventArgs e)
    //{
    //    if (gvRateHistory.Rows.Count == 0)
    //    {
    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'First Add Employee. After That Change Rate', 'warning');", true);
    //        return;
    //    }

    //    DateTime textdate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
    //    //if (textdate < DateTime.Now.Date)
    //    //{
    //    //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Back Date Not Allowed', 'error');", true);
    //    //    return;
    //    //}

    //    if (float.Parse(txtrateHistoryBillRate.Text) > 10000)
    //    {
    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Rate Must Less Than 10000', 'warning');", true);
    //        return;
    //    }
    //    if (btnrateHistoryChangeRate.Text != "Update Rate")
    //    {
    //        if (txtrateHistoryBillRate.Text == "")
    //        {
    //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Enter Rate', 'warning');", true);
    //            return;
    //        }
    //        RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
    //        RateEntity.Rate = float.Parse(txtrateHistoryBillRate.Text);
    //        RateEntity.RateType = ddlrateHistoryBillType.SelectedItem.Text;
    //        RateEntity.StartDate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
    //        RateEntity.EndDate = System.DateTime.Now.Date;
    //        RateEntity.JobTitle = ddlJobTitle.SelectedValue;
    //        RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
    //        RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
    //        RateEntity.RetainVacation = ddlRTVA.SelectedValue;
    //        RateEntity.effectivedate = DateTime.Parse(txtrateHistoryEffectiveDate.Text);
    //        RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));

    //        GetRateInformation(hfdEmpID.Value);
    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);
    //        GetRateInformation(hfdEmpID.Value);

    //        txtrateHistoryEffectiveDate.Text = "";
    //    }
    //    else
    //    {
    //        if (txtrateHistoryBillRate.Text == "")
    //        {
    //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Enter Rate', 'warning');", true);
    //            return;
    //        }

    //        comm.ExecuteQuery("update RateDetails set Rate =" + float.Parse(txtrateHistoryBillRate.Text) + ", RateType='" + ddlrateHistoryBillType.SelectedValue + "', StartDate='" + txtrateHistoryEffectiveDate.Text + "', effectivedate='" + txtrateHistoryEffectiveDate.Text + "' where RateId=" + hfdEditRateId.Value + "");
    //        bool chk = comm.IfExistsInTable("RateDetails", "*", "EmployeeId=" + int.Parse(hfdEmpID.Value) + "");
    //        if (chk == true)
    //        {
    //            DateTime previousdat = DateTime.Parse(txtrateHistoryEffectiveDate.Text).AddDays(-1);
    //            DataTable currentrateid = comm.GetAllDataFromTable("RateDetails", "*", "EmployeeId=" + int.Parse(hfdEmpID.Value) + "", "RateId");
    //            int colcount = currentrateid.Rows.Count;
    //            string currid = currentrateid.Rows[colcount - 2]["RateId"].ToString();
    //            comm.ExecuteQuery("update RateDetails set EndDate='" + previousdat.Date.ToString("yyyy-MM-dd") + "' where RateId=" + currid + " ");
    //        }

    //        //comm.ExecuteQuery("update RateDetails set Rate =" + txtrateHistoryBillRate.Text + ", RateType='" + ddlrateHistoryBillType.SelectedValue + "' where RateId=" + hfdEditRateId.Value + "");  effectivedate='" + txteffectivedate.Text
    //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Updated successfully', 'success');", true);
    //        btnSavePay.Text = "Save";
    //        hfdEditRateId.Value = "";
    //        if (Request.QueryString["EmployeeIDEdit"] != null)
    //        {
    //            GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
    //        }
    //        else
    //        {
    //            GetRateInformation(hfdEmpID.Value);
    //        }

    //        btnSavePay.Enabled = true;
    //        btnrateHistoryChangeRate.Text = "Change Rate";
    //        txtrateHistoryEffectiveDate.Text = "";
    //        txtrateHistoryEffectiveDate_adding_newrate.Text = "";
    //    }
    //    txtEffectiveDateRateHistory.Text = "";
    //    ddlrateHistoryBillType.SelectedValue = "";
    //    txtrateHistoryBillRate.Text = "";

    //    Response.Redirect("Home_Page.aspx");
    //}

    protected void ddlBussRateHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        ImageSave();
        if (ddlBussRateHistory.SelectedIndex != 0)
        {
            GetRate(txtMainEmail.Text);
        }
        else
        {
            gvRateHistory.DataSource = null;
            gvRateHistory.DataBind();
        }
    }
    public int SavePersonalDetails(int UserType, string userTypetext)
    {
        int ResultSavePersonalDetails = 0;
        try
        {
            EmpEntity.SINExpiry = txtSINExpiry.Text;
            EmpEntity.UserType = UserType;
            string userTypeUserACC = userTypetext;
            EmpEntity.LegalName = "";
            EmpEntity.Title = "";
            EmpEntity.FirstName = txtFirstName.Text;
            EmpEntity.MiddleName = txtMiddleName.Text;
            EmpEntity.LastName = txtLastName.Text;
            EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
            EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
            EmpEntity.Gender = ddlgender.SelectedItem.Text;
            EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
            //commented because NO is saved when we add the deactivated employee in another business
            //EmpEntity.IsActive = ddlEmpStatus.SelectedValue;
            //everytime we have to save YES when we add the deactivated employee in another business
            if (ddlEmpStatus.SelectedValue == "NO")
                EmpEntity.IsActive = "NO";
            else
                EmpEntity.IsActive = "YES";

            if (ddlEmpStatus.SelectedValue == "NO")
            {
                EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            }
            else
            {
                EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            }

            EmpEntity.TerminationDate = DateTime.Now.Date;

            EmpEntity.MotherOrgID = int.Parse(ViewState["bussWiseMotherOrgId"].ToString());

            EmpEntity.Occupation = "";

            int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);

            ResultSavePersonalDetails = Empid;


            if (string.IsNullOrEmpty(Convert.ToString(ViewState["profile"])))
            {
                ViewState["profile"] = "";
            }


            return ResultSavePersonalDetails;
        }
        catch (Exception ex)
        {
            return ResultSavePersonalDetails;
            throw ex;
        }
    }
    public int UpdatePersonalDetails(int EmpId, int UserType, string userTypetext, int BusinessId)
    {
        int ReturnEmpId = 0;
        try
        {
            if (int.Parse(ddl_Business.SelectedValue) == BusinessId && int.Parse(ddl_Employee.SelectedValue) == EmpId && txtMainEmail.Text == "")
            {
                EmpEntity.City = txtCity.Text;
                EmpEntity.Province = txtProvince.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                EmpEntity.UserType = UserType;
                string userTypeUserACC = "";
                EmpEntity.LegalName = "";
                EmpEntity.Title = "";
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;

                RateEntity.IsEmployeeEIExempty = "";
                RateEntity.IsEmployeeCPPExempty = "";
                RateEntity.RetainVacation = "";

                EmpEntity.EmployeeID = EmpId;
                EmpEntity.Occupation = "";

                var ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);
            }
            else
            {
                EmpEntity.City = txtCity.Text;
                EmpEntity.Province = txtProvince.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                EmpEntity.UserType = UserType;
                string userTypeUserACC = "";
                EmpEntity.LegalName = "";
                EmpEntity.Title = "";
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;

                RateEntity.IsEmployeeEIExempty = "";
                RateEntity.IsEmployeeCPPExempty = "";
                RateEntity.RetainVacation = "";

                EmpEntity.EmployeeID = EmpId;
                EmpEntity.Occupation = "";

                var ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);
            }

            if (txtMainEmail.Text != "")
            {
                EmpEntity.City = txtCity.Text;
                EmpEntity.Province = txtProvince.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                EmpEntity.MobilePhone = txtMobile.Text;
                //EmpEntity.UserType = int.Parse(ddlusertype11.SelectedValue);
                EmpEntity.UserType = UserType;
                string userTypeUserACC = "";
                EmpEntity.LegalName = "";
                EmpEntity.Title = "";
                EmpEntity.FirstName = txtFirstName.Text;
                EmpEntity.MiddleName = txtMiddleName.Text;
                EmpEntity.LastName = txtLastName.Text;
                EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
                EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
                EmpEntity.Gender = ddlgender.SelectedItem.Text;
                EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
                //EmpEntity.TerminationDate = DateTime.Parse(txtReleaseDate.Text);
                EmpEntity.TerminationDate = DateTime.Now.Date;

                RateEntity.IsEmployeeEIExempty = "";
                RateEntity.IsEmployeeCPPExempty = "";
                RateEntity.RetainVacation = "";

                EmpEntity.EmployeeID = EmpId;
                EmpEntity.Occupation = "";

                var ResultSavePersonalDetails = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);
            }

            ReturnEmpId = EmpId;


            if (string.IsNullOrEmpty(Convert.ToString(ViewState["profile"])))
            {
                ViewState["profile"] = "";
            }

            return ReturnEmpId;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public int SavePay(int _empid, int Buss)
    {
        int ResultSavePay = 0;
        try
        {
            foreach (GridViewRow row in GvRateDetails.Rows)
            {
                HiddenField hfdBusinessID = row.FindControl("hfdBusinessID") as HiddenField;
                HiddenField hfdRate = row.FindControl("hfdRate") as HiddenField;
                TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
                TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
                TextBox txtRate = row.FindControl("txtRate") as TextBox;
                DropDownList ddlPaytpe = row.FindControl("ddlPaytpe") as DropDownList;
                DropDownList ddlEmpEI11 = row.FindControl("ddlEmpEI11") as DropDownList;
                DropDownList ddlEmpEI22 = row.FindControl("ddlEmpEI22") as DropDownList;
                DropDownList ddlEmpEI33 = row.FindControl("ddlEmpEI33") as DropDownList;

                if (int.Parse(hfdBusinessID.Value) == Buss)
                {
                    RateEntity.EmployeeId = _empid;


                    // RateEntity.Rate = float.Parse(txtRate.Text);

                    //RateEntity.RateType = ddlPaytpe.SelectedItem.Text;
                    RateEntity.RateType = ddlPaytpe.SelectedValue;
                    //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
                    RateEntity.StartDate = DateTime.Parse(txtstdate.Text);
                    //RateEntity.EndDate = System.DateTime.Now.Date;
                    if (string.IsNullOrEmpty(txtEndDate.Text))
                    {
                        RateEntity.EndDate = System.DateTime.Now.Date;
                    }
                    else
                    {
                        RateEntity.EndDate = DateTime.Parse(txtEndDate.Text);
                    }

                    //string strRate;
                    RateEntity.JobTitle = "";
                    RateEntity.IsEmployeeEIExempty = ddlEmpEI11.SelectedValue;
                    RateEntity.IsEmployeeCPPExempty = ddlEmpEI22.SelectedValue;
                    RateEntity.RetainVacation = ddlEmpEI33.SelectedValue;
                    //  if (RateEntity.Rate.ToString() == "")
                    // {
                    //   RateEntity.Rate = null;                    }
                    //ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);

                    if (txtRate.Text.Trim() == "")
                    {

                        comm.ExecuteQuery("update RateDetails set Rate=null" + ", RateType='" + RateEntity.RateType + "', " +
                                                           "IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "', IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "', " +
                                                           "RetainVacation='" + RateEntity.RetainVacation + "', StartDate='" + RateEntity.StartDate + "', effectivedate = '" + RateEntity.StartDate + "'  where EmployeeId='" + _empid + "' and " +
                                                           "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");
                    }

                    else
                    {

                        comm.ExecuteQuery("update RateDetails set Rate=" + float.Parse(txtRate.Text) + ", RateType='" + RateEntity.RateType + "', " +
                                    "IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "', IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "', " +
                                    "RetainVacation='" + RateEntity.RetainVacation + "', StartDate='" + RateEntity.StartDate + "', effectivedate = '" + RateEntity.StartDate + "'  where EmployeeId='" + _empid + "' and " +
                                    "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");
                    }

                    #region end date logic commented
                    ////if (Convert.ToInt32(ViewState["NewGenEmp"]) == 0)
                    //if (ViewState["NewGenEmp"] == "Yes")
                    //{
                    //    //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                    //    RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
                    //    bool check_rate = comm.IfExistsInTable("RateDetails", "Rate", "EmployeeId='" + _empid + "' and " +
                    //        "Rate='" + RateEntity.Rate + "' and RateType='" + RateEntity.RateType + "' and StartDate='" + RateEntity.StartDate + "' and " +
                    //        "EndDate='" + RateEntity.EndDate + "' and IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "' and " +
                    //        "IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "' and RetainVacation='" + RateEntity.RetainVacation + "'");
                    //    if (check_rate == false)
                    //    {
                    //        ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);
                    //    }
                    //    //ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);

                    //    if (string.IsNullOrEmpty(txtEndDate.Text))
                    //    {

                    //    }
                    //    else
                    //    {
                    //        comm.ExecuteQuery("update RateDetails set EndDate='" + RateEntity.EndDate + "' where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");

                    //        comm.ExecuteQuery("update RateDetails set Is_Having_EndDate=1 where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");
                    //    }
                    //}

                    ////if (_empid != 0 && ViewState["NewGenEmp"] == null)
                    //if (_empid != 0 && ViewState["NewGenEmp"] == "No")
                    //{
                    //    ////RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                    //    //RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
                    //    //copmmented because the start date was saving wrong
                    //    RateEntity.effectivedate = DateTime.Parse(txtstdate.Text);

                    //    if (string.IsNullOrEmpty(txtEndDate.Text))
                    //    {
                    //        comm.ExecuteQuery("update RateDetails set Rate='" + RateEntity.Rate + "', RateType='" + RateEntity.RateType + "', " +
                    //            "IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "', IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "', " +
                    //            "RetainVacation='" + RateEntity.RetainVacation + "', EndDate='" + RateEntity.EndDate + "' where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");

                    //        comm.ExecuteQuery("update RateDetails set Is_Having_EndDate=0 where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");
                    //    }
                    //    else
                    //    {
                    //        comm.ExecuteQuery("update RateDetails set Rate='" + RateEntity.Rate + "', RateType='" + RateEntity.RateType + "', " +
                    //            "IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "', IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "', " +
                    //            "RetainVacation='" + RateEntity.RetainVacation + "', EndDate='" + RateEntity.EndDate + "' where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");

                    //        comm.ExecuteQuery("update RateDetails set Is_Having_EndDate=1 where EmployeeId='" + _empid + "' and " +
                    //            "RateId=(select MAX(RateId) from RateDetails where EmployeeId='" + _empid + "')");
                    //    }
                    //}
                    #endregion
                }
                //else
                //{
                //    if (Convert.ToInt32(ViewState["NewGenEmp"]) == 0)
                //    {
                //        //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                //        RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
                //        bool check_rate = comm.IfExistsInTable("RateDetails", "Rate", "EmployeeId='" + _empid + "' and " +
                //            "Rate='" + RateEntity.Rate + "' and RateType='" + RateEntity.RateType + "' and StartDate='" + RateEntity.StartDate + "' and " +
                //            "EndDate='" + RateEntity.EndDate + "' and IsEmployeeEIExempty='" + RateEntity.IsEmployeeEIExempty + "' and " +
                //            "IsEmployeeCPPExempty='" + RateEntity.IsEmployeeCPPExempty + "' and RetainVacation='" + RateEntity.RetainVacation + "'");
                //        if (check_rate == false)
                //        {
                //            ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);
                //        }
                //        //ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);
                //    }
                //}
            }


            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);
            return ResultSavePay;
            //ClearPaymentDetails();
        }
        catch (Exception ex)
        {
            return ResultSavePay;
            throw ex;
        }
    }

    public int ValidateRate()
    {
        int cnt = 0;
        foreach (GridViewRow row in GvRateDetails.Rows)
        {
            TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
            //TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
            TextBox txtRate = row.FindControl("txtRate") as TextBox;
            DropDownList ddlPaytpe = row.FindControl("ddlPaytpe") as DropDownList;
            if (string.IsNullOrEmpty(txtstdate.Text) || txtRate.Text == "0" || string.IsNullOrEmpty(ddlPaytpe.SelectedValue))
            {
                cnt++;
            }
        }
        return cnt;
    }

    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        //AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
        ImageSave();
        #region textbox validations
        if (txtFirstName.Text.Trim() == "")
        {
            txtFirstName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name in personal details', 'warning');", true);
            return;
        }
        if (txtLastName.Text.Trim() == "")
        {
            txtLastName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name in personal details', 'warning');", true);
            return;
        }
        if (txtHiredate.Text == "")
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter hire date in personal details', 'warning');", true);
            return;
        }
        //DateTime hdate = DateTime.Parse(txtHiredate.Text).Date;
        //DateTime currdate = DateTime.Now.Date;
        //if (hdate > currdate)
        //{
        //    txtHiredate.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be greater than current date in personal details', 'warning');", true);
        //    return;
        //}

        if (!txtMainEmail.ReadOnly)
        {
            //if (txtMainEmail.Text == "")
            //{
            //    txtMainEmail.Focus();
            //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter email address in personal details', 'warning');", true);
            //    return;
            //}

            if (txtMainEmail.Text.Trim() != string.Empty)
            {
                if (IsValidEmail(txtMainEmail.Text) == false)
                {
                    txtMainEmail.Focus();
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter vaild email address in personal details', 'warning');", true);
                    return;
                }
            }

            if (txtConLoginEmail.Text == "" && txtConLoginEmail.Visible == true)
            {
                txtConLoginEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please confirm email address in personal details', 'warning');", true);
                return;
            }
            if (txtMainEmail.Text != txtConLoginEmail.Text && txtConLoginEmail.Visible == true)
            {
                txtMainEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter same email address in confirm email address in personal details', 'warning');", true);
                return;
            }
            if (txtConLoginEmail.Text.Trim() != string.Empty)
            {
                if (IsValidEmail(txtConLoginEmail.Text) == false)
                {
                    txtConLoginEmail.Focus();
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter vaild confirm email address in personal details', 'warning');", true);
                    return;
                }
            }
        }

        if (txtMainPhone.Text == "")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter contact phone in personal details', 'warning');", true);
            return;
        }
        string txtMainPhone_ = txtMainPhone.Text.Replace("-", string.Empty).Trim();
        if (txtMainPhone_.Length.ToString() != "10")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter contact phone number properly', 'warning');", true);
            return;
        }
        #endregion

        #region textbox validations for address part
        //if (txtSearchPostalCode.Text.Trim() == "")
        //{
        //    txtSearchPostalCode.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid postal code in format A#A #A# and click on search in personal details', 'warning');", true);
        //    return;
        //}
        //if (gvAddressList.Rows.Count == 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid postal code in format A#A #A# and click on search in personal details', 'warning');", true);
        //    return;
        //}
        //if (txtStreetNo.Text.Trim() == "")
        //{
        //    txtStreetNo.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter street number in personal details', 'warning');", true);
        //    return;
        //}
        //if (txtStreetName.Text.Trim() == "")
        //{
        //    txtStreetName.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter address line 1', 'warning');", true);
        //    return;
        //}
        //if (txtCity.Text.Trim() == "")
        //{
        //    txtCity.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter city', 'warning');", true);
        //    return;
        //}
        //if (txtProvince.Text.Trim() == "")
        //{
        //    txtProvince.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter province', 'warning');", true);
        //    return;
        //}
        //if (txtPostalCode.Text.Trim() == "")
        //{
        //    txtPostalCode.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please postal code', 'warning');", true);
        //    return;
        //}
        #endregion

        #region duplicate entry validations
        if (!txtMainEmail.ReadOnly)
        {
            if (txtMainEmail.Text != "")
            {
                StringBuilder checkbussid_ = new StringBuilder();
                foreach (GridViewRow row in Gv_RolePermission.Rows)
                {
                    HiddenField hfd_BussinessId_ = row.FindControl("hfd_BussinessId") as HiddenField;
                    //HiddenField hdf_EmpId_ = row.FindControl("hdf_EmpId") as HiddenField;
                    Label lbl_BusinessName_ = row.FindControl("lbl_BusinessName") as Label;
                    checkbussid_.Append(hfd_BussinessId_.Value + ",");
                    //if (hdf_EmpId_.Value == "0")
                    //{
                    bool check_email_in_business = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID =" + int.Parse(hfd_BussinessId_.Value) + " and ea.MainEmail='" + txtMainEmail.Text + "' ");
                    if (check_email_in_business)
                    {
                        if (txtMainEmail.Text != "")
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Email Id is Already Registered in " + lbl_BusinessName_.Text + " business', 'warning');", true);
                        //if (txtMainEmail.Text == "")
                        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Blank Email Id is Already Registered in " + lbl_BusinessName_.Text + " business', 'warning');", true);
                        return;
                    }
                    //}
                }
            }
            //string businessesid = checkbussid_.Remove(checkbussid_.Length - 1);
            //bool Chk2 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID in (+ BussNo2 +) and ea.MainEmail='" + txtMainEmail.Text + "' ");
            //if (Chk2 == true)
            //{
            //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
            //    return;
            //}
        }
        #endregion

        foreach (GridViewRow row11 in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
            HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");

            if (__hfd_UserRoleId.Value != "5" && __hfd_UserRoleId.Value != "12")
            {
                foreach (GridViewRow row22 in GvRateDetails.Rows)
                {
                    HiddenField hfdBusinessID_ = (HiddenField)row22.FindControl("hfdBusinessID");
                    TextBox txtstdate = (TextBox)row22.FindControl("txtstdate");
                    TextBox txtEndDate = (TextBox)row22.FindControl("txtEndDate");
                    TextBox txtRate = (TextBox)row22.FindControl("txtRate");
                    DropDownList ddlPaytpe = row22.FindControl("ddlPaytpe") as DropDownList;

                    if (hfdBusinessID_.Value == hfd_BussinessId_.Value)
                    {
                        if (txtstdate.Text.Trim().Length == 0)
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Start Date', 'warning');", true);
                            return;
                        }
                        //if (txtEndDate.Text.Trim().Length == 0)
                        //{
                        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter End Date', 'warning');", true);
                        //    return;
                        //}
                        if (txtstdate.Text.Trim().Length > 0 && txtEndDate.Text.Trim().Length > 0)
                        {
                            DateTime dtfrom = DateTime.Parse(txtstdate.Text);
                            DateTime dtto = DateTime.Parse(txtEndDate.Text);
                            if (dtfrom > dtto)
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Start Date is Greater than End Date', 'warning');", true);
                                return;
                            }
                            if (dtfrom == dtto)
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'End Date should be Greater than Start Date', 'warning');", true);
                                return;
                            }
                        }
                        /*    if (txtRate.Text == "" || txtRate.Text == "0")
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Bill Rate Properly', 'warning');", true);
                                return;
                            }*/
                        if (string.IsNullOrEmpty(ddlPaytpe.SelectedValue))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Select Bill Type Properly', 'warning');", true);
                            return;
                        }
                    }
                }
            }
        }

        int cnt = 0;
        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
            if (hfd_BussinessId.Value == ddlBusiness.SelectedValue && hdf_EmpId.Value != "0")
            {
                cnt++;
            }
        }
        if (cnt > 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Business Already Addedd', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'User has been assigned role, Please add more users to add a different role', 'error');", true);
            return;
        }

        #region validation for pay details in 4th tab grid view
        //foreach (GridViewRow row in GvRateDetails.Rows)
        //{
        //    TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
        //    TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
        //    TextBox txtRate = row.FindControl("txtRate") as TextBox;
        //    if (txtstdate.Text.Trim().Length == 0)
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Start Date', 'warning');", true);
        //        return;
        //    }
        //    //if (txtEndDate.Text.Trim().Length == 0)
        //    //{
        //    //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter End Date', 'warning');", true);
        //    //    return;
        //    //}
        //    if (txtstdate.Text.Trim().Length > 0 && txtEndDate.Text.Trim().Length > 0)
        //    {
        //        DateTime dtfrom = DateTime.Parse(txtstdate.Text);
        //        DateTime dtto = DateTime.Parse(txtEndDate.Text);
        //        if (dtfrom > dtto)
        //        {
        //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Start Date is Greater than End Date', 'warning');", true);
        //            return;
        //        }
        //        if (dtfrom == dtto)
        //        {
        //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'End Date should be Greater than Start Date', 'warning');", true);
        //            return;
        //        }
        //    }
        //    if (txtRate.Text == "" || txtRate.Text == "0")
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Bill Rate Properly', 'warning');", true);
        //        return;
        //    }
        //}
        #endregion

        #region validation to select the assign business and assign role in Gv_RolePermission grid
        if (Gv_RolePermission.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Assign Business And Assign Role And Then Click on Add Button', 'warning');", true);
            return;
        }
        #endregion

        //if (ValidateRate() > 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Enter Rate Details Properly', 'warning');", true);
        //    return;
        //}

        int _Empid = 0, _AddressID, _bussid;
        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_RoleId = row.FindControl("hfd_RoleId") as HiddenField;

            Label lbl_BusinessName = row.FindControl("lbl_BusinessName") as Label;
            HiddenField hfd_UserRoleId = row.FindControl("hfd_UserRoleId") as HiddenField;
            HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
            Label lbl_UserRole = row.FindControl("lbl_UserRole") as Label;
            _bussid = int.Parse(hfd_BussinessId.Value);
            if (hdf_EmpId.Value == "0")
            {
                ViewState["NewGenEmp"] = "Yes";
                _bussid = int.Parse(hfd_BussinessId.Value);
                DataTable dtbussWiseMotherOrgId = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + _bussid + "", "");
                if (dtbussWiseMotherOrgId.Rows.Count > 0)
                {
                    ViewState["bussWiseMotherOrgId"] = dtbussWiseMotherOrgId.Rows[0][0].ToString();
                }

                _Empid = SavePersonalDetails(Convert.ToInt32(hfd_UserRoleId.Value), lbl_UserRole.Text);
                SavePay(_Empid, _bussid);
                var UserId = SaveProfilePic(_Empid, ViewState["profile"].ToString(), int.Parse(hfd_UserRoleId.Value), lbl_UserRole.Text);
                _AddressID = SaveAddress(_Empid);
                string EmployeeName = comm.GetNameFromId("AddEmployee", _Empid);
                AddBeCtrl.AssgineBusiness(Convert.ToInt32(hfd_BussinessId.Value), _Empid, lbl_BusinessName.Text, EmployeeName, Convert.ToInt32(hfd_UserRoleId.Value), Convert.ToInt32(ViewState["bussWiseMotherOrgId"].ToString()));
                SaveUpdateRolePermission(_Empid, int.Parse(hfd_BussinessId.Value), int.Parse(hfd_UserRoleId.Value), hfd_RoleId.Value);
                SaveAssgineProject(int.Parse(hfd_BussinessId.Value), _Empid);
                comm.ExecuteQuery("insert into MappingTable(USERID,UserTypeId,EmployeeID,BusinessID)values(" + UserId + "," + int.Parse(hfd_UserRoleId.Value) + "," + _Empid + "," + hfd_BussinessId.Value + ")");
                comm.ExecuteQuery("UPDATE AssginBussiness SET UserTypeId=" + hfd_UserRoleId.Value + " WHERE EmployeeID=" + _Empid + " AND BusinessID=" + hfd_BussinessId.Value + "");
                if (hfd_UserRoleId.Value != "4")
                {
                    bool chkm = comm.IfExistsInTable("AssgineManager", "*", "ManagerID='" + _Empid + "'");
                    if (chkm == true)
                    {
                        comm.ExecuteQuery("delete from AssgineManager where ManagerID='" + _Empid + "'");
                    }
                }
            }
            else
            {
                ViewState["NewGenEmp"] = "No";
                DataTable dtbussWiseMotherOrgId = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + _bussid + "", "");
                if (dtbussWiseMotherOrgId.Rows.Count > 0)
                {
                    ViewState["bussWiseMotherOrgId"] = dtbussWiseMotherOrgId.Rows[0][0].ToString();
                }
                //commented for the issue creating while editing the blank employee. So now editing only the user selected in business and employee ddl
                //_Empid = UpdatePersonalDetails(int.Parse(hdf_EmpId.Value), Convert.ToInt32(hfd_UserRoleId.Value), lbl_UserRole.Text);
                _Empid = UpdatePersonalDetails(int.Parse(hdf_EmpId.Value), Convert.ToInt32(hfd_UserRoleId.Value), lbl_UserRole.Text, int.Parse(hfd_BussinessId.Value));
                SavePay(_Empid, _bussid);
                SaveAddress(_Empid);
                SaveUpdateRolePermission(_Empid, int.Parse(hfd_BussinessId.Value), int.Parse(hfd_UserRoleId.Value), hfd_RoleId.Value);
                string EmployeeName = comm.GetNameFromId("AddEmployee", _Empid);
                bool businessAssignmentExists = comm.IfExistsInTable(
                    "AssginBussiness",
                    "*",
                    "EmployeeID=" + _Empid + " AND BusinessID=" + hfd_BussinessId.Value);

                if (businessAssignmentExists)
                {
                    comm.ExecuteQuery(
                        "UPDATE AssginBussiness SET UserTypeId=" + hfd_UserRoleId.Value +
                        " WHERE EmployeeID=" + _Empid +
                        " AND BusinessID=" + hfd_BussinessId.Value);
                }
                else
                {
                    AddBeCtrl.AssgineBusiness(
                        Convert.ToInt32(hfd_BussinessId.Value),
                        _Empid,
                        lbl_BusinessName.Text,
                        EmployeeName,
                        Convert.ToInt32(hfd_UserRoleId.Value),
                        Convert.ToInt32(ViewState["bussWiseMotherOrgId"].ToString()));
                }

                // Normalize historical duplicate assignment rows for this same employee/business.
                comm.ExecuteQuery(
                    "WITH d AS (" +
                    " SELECT AssginBussinessId, ROW_NUMBER() OVER (ORDER BY AssginBussinessId) AS rn" +
                    " FROM AssginBussiness" +
                    " WHERE EmployeeID=" + _Empid +
                    " AND BusinessID=" + hfd_BussinessId.Value +
                    ") DELETE FROM d WHERE rn>1");
                SaveAssgineProject(int.Parse(hfd_BussinessId.Value), _Empid);
                var UserId = comm.ExecuteQueryDataTable("select UserId from UserAcc where Email='" + txtMainEmail.Text + "'").Rows[0][0].ToString();
                comm.ExecuteQuery("delete from MappingTable where EmployeeID=" + _Empid + " and BusinessID=" + hfd_BussinessId.Value + "");
                comm.ExecuteQuery("insert into MappingTable(USERID,UserTypeId,EmployeeID,BusinessID)values(" + UserId + "," + int.Parse(hfd_UserRoleId.Value) + "," + _Empid + "," + hfd_BussinessId.Value + ")");
                comm.ExecuteQuery("UPDATE AssginBussiness SET UserTypeId=" + hfd_UserRoleId.Value + " WHERE EmployeeID=" + _Empid + " AND BusinessID=" + hfd_BussinessId.Value + "");
            }
            if (GvProject.Rows.Count == 0)
            {
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='R' WHERE EmployeeID=" + _Empid + "");
            }
            comm.ExecuteQuery("update useracc set Profile_Image='" + ViewState["profile"].ToString() + "' WHERE EmployeeID=" + _Empid + "");
            if (hfd_UserRoleId.Value != "2")
            {
                DataTable dt222 = (DataTable)ViewState["PrvRolePermission"];
                DataRow[] results = dt222.Select("BussinessId = '" + hfd_BussinessId.Value + "' AND UserRoleId = '" + hfd_UserRoleId.Value + "'");
                if (results.Count() == 0)
                {
                    bool chkm = comm.IfExistsInTable("AssgineManager", "*", "ManagerID='" + _Empid + "'");
                    if (chkm == true)
                    {
                        comm.ExecuteQuery("delete from AssgineManager where ManagerID='" + _Empid + "'");
                    }
                }
            }
        }
        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved', 'success');", true);

        // Clear the cookie with the specified name
        HttpCookie CookieName = Request.Cookies["tabid"];
        if (CookieName != null)
        {
            var cookvaluename = CookieName.Value;
            //if (cookvaluename == "#permission")
            //{
            //    CookieName.Expires = DateTime.Now.AddDays(-1);
            //    Response.Cookies.Add(CookieName);
            //}
            if (cookvaluename == "#ratedetils")
            {
                CookieName.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(CookieName);
            }
            if (cookvaluename == "#project")
            {
                CookieName.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(CookieName);
            }
        }

        //Response.Redirect("Home_Page.aspx");
        Modelconfirmmessagepopup.Show();
    }

    public void Updaterate(int buss)
    {

    }

    public int SaveProfilePic(int empid, string str, int UserTypeid, string UserType)
    {
        Uae.EmployeeId = empid;

        //Uae.Email = txtMainEmail.Text.Trim();
        DataTable dt_GetEmailId = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = " + int.Parse(ddl_Employee.SelectedValue) + ")", "");
        Uae.Email = dt_GetEmailId.Rows[0]["MainEmail"].ToString();

        Uae.MotherOrgid = Convert.ToInt32(Session["MotherOrgid"].ToString());
        Uae.UserTypeid = UserTypeid;
        Uae.UserType = UserType;
        Uae.ProfileImageName = ViewState["profile"].ToString();

        return Convert.ToInt32(Uac.Sp_InsertUserAccFromAddress(Uae));
    }
    public int SaveAddress(int _Empid)
    {
        int ResultSaveAddres = 0;
        DataTable cityid = new DataTable();
        DataTable province = new DataTable();
        try
        {
            bool chk = comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtCity.Text + "'");
            if (chk == false)
            {
                comm.ExecuteQuery("insert into CityMaster (CityName,Duplicate)values('" + txtCity.Text + "','False')");
            }

            bool chkProvinceMaster = comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtProvince.Text + "'");
            if (chkProvinceMaster == false)
            {
                comm.ExecuteQuery("insert into ProvinceMaster (ProvinceName,Duplicate)values('" + txtProvince.Text + "','False')");
                province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            }
            cityid = comm.GetAllDataFromTable("CityMaster", "CityId", "CityName='" + txtCity.Text + "'", "");
            province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            txtPostalCode.Text = txtPostalCode.Text.Replace(" ", string.Empty).Trim();
            EmpAddEnity.Postalcode = txtPostalCode.Text;
            EmpAddEnity.NumberSuffix = "";
            EmpAddEnity.StreetName = txtStreetName.Text.ToUpper();
            EmpAddEnity.StreetType = txtStreetType.Text.ToUpper();
            EmpAddEnity.StreetDirection = txtStreetDirection.Text;
            //EmpAddEnity.SuitType = ddlSuitType.SelectedItem.Text;
            EmpAddEnity.SuitType = DropDownList2.SelectedValue;//"";
            EmpAddEnity.SuiteNumber = txtSuiteNumber.Text;
            //firoz 07
            EmpAddEnity.SuitSuffix = ddlsuitesuffix.SelectedItem.Text;
            //end
            EmpAddEnity.City = txtCity.Text.ToUpper();
            EmpAddEnity.Province = txtProvince.Text.ToUpper();
            EmpAddEnity.MainPhone = txtMainPhone.Text;
            EmpAddEnity.WorkPhone = "";
            EmpAddEnity.Mobile = txtMobile.Text;
            EmpAddEnity.Fax = "";
            //EmpAddEnity.MainEmail = txtMainEmail.Text.Trim(); ;
            //Uae.Email = txtMainEmail.Text.Trim();
            DataTable dt_GetEmailId = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID = " + int.Parse(ddl_Employee.SelectedValue) + ")", "");
            EmpAddEnity.MainEmail = dt_GetEmailId.Rows[0]["MainEmail"].ToString();

            if (txtMainEmail.Text.Trim() == "" || txtMainEmail.Text.Trim() == string.Empty)
            {
                EmpAddEnity.MainEmail = dt_GetEmailId.Rows[0]["MainEmail"].ToString();
                //comm.ExecuteQuery("UPDATE [UserAcc] SET [Email]='" + txtMainEmail.Text + "' WHERE [Email]='" + dt_GetEmailId.Rows[0]["MainEmail"].ToString() + "' ");
            }
            if (txtMainEmail.Text != "")
            {
                EmpAddEnity.MainEmail = txtMainEmail.Text.ToString();
                comm.ExecuteQuery("UPDATE [UserAcc] SET [Email]='" + txtMainEmail.Text + "' WHERE [Email]='" + dt_GetEmailId.Rows[0]["MainEmail"].ToString() + "' ");
                comm.ExecuteQuery("UPDATE [EmpAddress] SET [MainEmail]='" + txtMainEmail.Text + "', [CCEmail]='" + txtMainEmail.Text + "' WHERE [MainEmail]='" + dt_GetEmailId.Rows[0]["MainEmail"].ToString() + "' ");
            }

            EmpAddEnity.CCEmail = "";
            EmpAddEnity.Website = "";
            EmpAddEnity.Other1 = "";
            EmpAddEnity.StreetNo = txtStreetNo.Text.ToUpper();
            RateEntity.IsEmployeeEIExempty = "";
            RateEntity.IsEmployeeCPPExempty = "";
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
            RateEntity.RetainVacation = "";
            EmpAddEnity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString()); //int.Parse(hfdMotherId.Value);

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, _Empid);

            return ResultSaveAddres;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SaveUpdateRolePermission(int empid, int bussid, int usertype, string permission)
    {
        bool check_emp_id_in_user_Role_permission = comm.IfExistsInTable("User_Role_Permission", "Employee_Id", "Employee_Id='" + empid + "'");

        if (check_emp_id_in_user_Role_permission == true)
        {
            comm.ExecuteQuery("update User_Role_Permission set Role_Permission_Id='" + permission + "', BusinessId='" + bussid + "' , UserTypeId = '" + usertype + "' " +
                "where Employee_Id ='" + empid + "'  and Status='True' ");
        }
        else
        {
            comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) " +
                "Values ('" + permission + "', '" + empid + "', '" + usertype + "', 'True', '" + bussid + "')");
        }
    }

    public void SaveAssgineProject(int bussid, int EmpId)
    {
        foreach (GridViewRow row in GvProject.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_ProjectNameId = row.FindControl("hfd_ProjectNameId") as HiddenField;

            if (int.Parse(hfd_BussinessId.Value) == bussid)
            {
                ape.BusinessID = Convert.ToInt32(hfd_BussinessId.Value);
                ape.EmployeeID = Convert.ToInt32(EmpId);
                ape.ProjectID = Convert.ToInt32(hfd_ProjectNameId.Value);
                apc.InsertAssgineProject(ape);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='P' WHERE EmployeeID=" + EmpId + "");
            }
        }

        comm.ExecuteQuery("insert into EmpType_Change_History values (" + EmpId + ", " + bussid + ", NULL, NULL, 'P', '" + DateTime.Now.Date.ToString() + "') ");
    }

    protected void ddlBusiness_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBusiness.SelectedItem.Text != "Select Business")
        {
            ImageSave();
            ViewState["bid"] = ddlBusiness.SelectedValue;
            DataTable dt = comm.GetAllDataFromTable("MappingTable", "[dbo].[FN_DESC]('UserType',UserTypeId)", "userid='" + Session["UserId"].ToString() + "' and BusinessID='" + ViewState["bid"] + "'", "");
            if (dt.Rows.Count > 0)
            {
                ViewState["QueryStringRole"] = dt.Rows[0][0].ToString();
            }
            BindAllUserType();
        }
    }

    protected void GridView_User_Role_Permission_Pop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField HiddenField_Role_Permission_Id = e.Row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
            DropDownList DropDownList_User_Role_Permission_Access_Status = e.Row.FindControl("DropDownList_User_Role_Permission_Access_Status") as DropDownList;
            string[] time = { "1", "2", "7", "8", "9", "18", "19", "20", "25", "26" };
            bool isExist = Array.Exists(time, x => x == HiddenField_Role_Permission_Id.Value.ToString());
            if (isExist)
            {
                DropDownList_User_Role_Permission_Access_Status.SelectedValue = "YES";
            }

        }
    }

    public void DeleteAdvanceTimeSheet(int empid, int bussid, DateTime ctdate, int prrojectID)
    {
        int result = 0;
        bool check_reg_emp_type = comm.IfExistsInTable("addemployee with (Nolock)", "EMPLOYEEID", "EMPLOYEEID=" + empid + " and EMPLOYEETYPE='R'");
        if (check_reg_emp_type)
        {
            result = comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + empid + "  and BussID = '" + bussid + "' and TimeSheetDate>= '" + ctdate + "' ");
        }

        bool check_proj_emp_type = comm.IfExistsInTable("addemployee with (Nolock)", "EMPLOYEEID", "EMPLOYEEID=" + empid + " and EMPLOYEETYPE='P'");
        if (check_proj_emp_type)
        {
            result = comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + empid + "  and BussID = '" + bussid + "' and TimeSheetDate>= '" + ctdate + "' " +
                "and ProjectId=" + prrojectID);
            comm.ExecuteQuery("delete from ASSIGNPROJECT where BUSINESSID=" + bussid + " and PROJECTID=" + prrojectID + " and EMPLOYEEID='" + empid + "'");
        }

        ModelConfirm.Hide();

        #region to convert the user into regular after checking if all the future timesheet is deleted for all projects and converting ointo regular employee
        //added by kumar
        bool cchkAssprojectforfuturetimesheetdelete = comm.IfExistsInTable("ASSIGNPROJECT", "PROJECTID", "EMPLOYEEID='" + empid + "' " +
                        "and BUSINESSID=" + bussid + "");
        if (cchkAssprojectforfuturetimesheetdelete == false)
        {
            comm.ExecuteQuery("delete from TimeSheetEntry where EmpId = " + empid + "  and BussID = '" + bussid + "' " +
            "and TimeSheetDate>= '" + ctdate + "'");
            comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE ='R' WHERE EmployeeID='" + empid + "'");
        }
        #endregion

        if (result > 0)
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Advance Time Sheet from " + ctdate.ToString("dd-MMM-yyyy") + " is Deleted', 'info');", true);
        BindProjectGrid();
    }

    protected void btnYesConfirm_Click(object sender, EventArgs e)
    {
        DateTime ctdate = DateTime.Now.Date;
        int empid = Convert.ToInt32(ViewState["AdvEmpid"].ToString());
        int bussid = Convert.ToInt32(ViewState["AdvBussid"].ToString());
        int prrojectID = Convert.ToInt32(ViewState["Projectid"].ToString());
        DeleteAdvanceTimeSheet(empid, bussid, ctdate, prrojectID);
        DataTable dt = (DataTable)ViewState["ProjetName"];
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            DataRow dr = dt.Rows[i];
            if (dr["ProjectId"].ToString() == prrojectID.ToString())
                dr.Delete();
        }
        dt.AcceptChanges();
        this.BindProjectGrid();
    }

    protected void btnNoConfirm_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)ViewState["ProjetName"];
        for (int i = dt.Rows.Count - 1; i >= 0; i--)
        {
            DataRow dr = dt.Rows[i];
            if (dr["ProjectId"].ToString() == ddlAssProjectName.SelectedValue)
                dr.Delete();
        }
        dt.AcceptChanges();
        ViewState["ProjetName"] = dt;
        ModelConfirm.Hide();
    }

    public DataTable GetEmplist_SuppAdmin(string type, int empid, int select)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
            //SqlCommand cmd = new SqlCommand("GetEmplist_SuppAdmin", con);
            SqlCommand cmd = new SqlCommand("GetEmplist_IsActive_SuppAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Type", type);
            cmd.Parameters.AddWithValue("@empid", empid);
            cmd.Parameters.AddWithValue("@selectempid", select);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    protected void DisableAllControlsInPage(Control parent, bool State)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is DropDownList)
            {
                ((DropDownList)(c)).Enabled = State;
            }
            if (c is TextBox)
            {
                ((TextBox)(c)).Enabled = State;
            }
            //if (c is LinkButton)
            //{
            //    ((LinkButton)(c)).Enabled = State;
            //}
            if (c is FileUpload)
            {
                ((FileUpload)(c)).Enabled = State;
            }
            if (c is Button)
            {
                ((Button)(c)).Enabled = State;
            }
            DisableAllControlsInPage(c, State);
        }
    }

    protected void GvRateDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            #region Kumar code for role permissions, cancel plan, deactivate subscriber reseller by support admin
            HiddenField hfdBusinessID = (e.Row.FindControl("hfdBusinessID") as HiddenField);
            DataTable Get_EmployeeID = new DataTable();
            if (Session["UserTypeid"].ToString() == "1")
            {
                if (Request.QueryString["EmployeeIDEdit"] != null && Request.QueryString["BussEditId"] != null)
                {
                    var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                        "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                    Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' " +
                        "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");

                    if (Get_EmployeeID.Rows.Count > 0)
                    {
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(Get_EmployeeID.Rows[0]["EmployeeID"].ToString()) + "'", "");
                        if (check_EmployeeID_activestatus.Rows.Count > 0)
                        {
                            if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                            {
                                e.Row.Enabled = false;
                            }
                        }
                    }
                }
                if (Request.QueryString["EmployeeIDEdit"] == null && Request.QueryString["BussEditId"] == null)
                {
                    if (ddl_Business.SelectedValue != "" && ddl_Employee.SelectedValue != "")
                    {
                        var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(ddl_Business.SelectedValue) + "' " +
                            "and EmployeeID = '" + int.Parse(ddl_Employee.SelectedValue) + "'", "");
                        Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' " +
                            "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");

                        if (Get_EmployeeID.Rows.Count > 0)
                        {
                            var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(Get_EmployeeID.Rows[0]["EmployeeID"].ToString()) + "'", "");
                            if (check_EmployeeID_activestatus.Rows.Count > 0)
                            {
                                if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                                {
                                    e.Row.Enabled = false;
                                }
                            }
                        }
                    }
                }
            }

            if (Session["UserTypeid"].ToString() != "1")
            {
                //var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                //bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");

                DataTable Get_Reseller_Subscriber_ID = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' and UserTypeId=1", "");

                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "'", "");

                //DataTable Get_Reseller_Subscriber_AddressId = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "ADDRESSID", "EmployeeID = '" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "'", "");

                //DataTable Get_Reseller_Subscriber_EmailId = comm.GetAllDataFromTable("EmpAddress with (Nolock)", "MainEmail", "EmpAddressId = '" + int.Parse(Get_Reseller_Subscriber_AddressId.Rows[0]["ADDRESSID"].ToString()) + "'", "");

                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=0 ");

                    var check_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    if (check_subscriber_cancelled.Rows.Count > 0 || check_subscriber_plan_deactivated.Rows.Count > 0
                        || check_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        e.Row.Enabled = false;
                    }

                    var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                    Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' " +
                        "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                    if (Get_EmployeeID.Rows.Count > 0)
                    {
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(Get_EmployeeID.Rows[0]["EmployeeID"].ToString()) + "'", "");
                        if (check_EmployeeID_activestatus.Rows.Count > 0)
                        {
                            if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                            {
                                e.Row.Enabled = false;
                            }
                        }
                    }
                }

                if (check_reseller_or_subscriber_email)
                {
                    var check_reseller_subscriber_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and Is_Cancelled=1 ");

                    var check_reseller_fully_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + "' and Is_Cancelled=1 ");

                    var check_reseller_subscriber_plan_deactivated = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' " +
                        "and MotherOrgID = " + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["MotherOrgID"].ToString()) + " and IsActive=0 ");

                    var check_reseller_subscriber_deactivated = comm.ExecuteQueryDataTable("select IsActive from AddEmployee with (Nolock) where EmployeeID='" + int.Parse(Get_Reseller_Subscriber_ID.Rows[0]["EmployeeID"].ToString()) + "' ");

                    var check_deactivated_user_role_employeeid = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeId", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' and USERID = '" + Session["UserId"].ToString() + "'", "");
                    var check_deactivated_user_role_employeeid_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(check_deactivated_user_role_employeeid.Rows[0]["EmployeeId"].ToString()) + "'", "");

                    if (check_reseller_subscriber_cancelled.Rows.Count > 0 || check_reseller_fully_cancelled.Rows.Count > 0
                        || check_reseller_subscriber_plan_deactivated.Rows.Count > 0 || check_reseller_subscriber_deactivated.Rows[0]["IsActive"].ToString() == "NO        "
                        || check_deactivated_user_role_employeeid_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                    {
                        e.Row.Enabled = false;
                    }

                    var Get_USERID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "USERID", "BusinessID = '" + int.Parse(Request.QueryString["BussEditId"].ToString()) + "' " +
                    "and EmployeeID = '" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "'", "");
                    Get_EmployeeID = comm.GetAllDataFromTable("MappingTable with (Nolock)", "EmployeeID", "BusinessID = '" + int.Parse(hfdBusinessID.Value) + "' " +
                        "and USERID = '" + int.Parse(Get_USERID.Rows[0]["USERID"].ToString()) + "'", "");
                    if (Get_EmployeeID.Rows.Count > 0)
                    {
                        var check_EmployeeID_activestatus = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID = '" + int.Parse(Get_EmployeeID.Rows[0]["EmployeeID"].ToString()) + "'", "");
                        if (check_EmployeeID_activestatus.Rows.Count > 0)
                        {
                            if (check_EmployeeID_activestatus.Rows[0]["IsActive"].ToString() == "NO        ")
                            {
                                e.Row.Enabled = false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region if the end date exists it will show in the end date text box in gridview
            HiddenField hfdRate = (e.Row.FindControl("hfdRate") as HiddenField);
            if (hfdRate.Value != "" || hfdRate.Value != "0")
            {
                TextBox txtEndDate = (e.Row.FindControl("txtEndDate") as TextBox);
                var Check_If_Rate_EndDate_Exists = comm.ExecuteQueryDataTable("select Is_Having_EndDate from RateDetails with (Nolock) where RateId='" + hfdRate.Value + "' ");
                if (Check_If_Rate_EndDate_Exists.Rows.Count > 0)
                {
                    if (Check_If_Rate_EndDate_Exists.Rows[0]["Is_Having_EndDate"].ToString() == "False")
                        txtEndDate.Text = "";
                }
            }
            #endregion
        }

        DisableAdmAcc();
    }

    public void DisableAdmAcc()
    {
        foreach (GridViewRow row11 in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
            HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");
            foreach (GridViewRow row22 in GvRateDetails.Rows)
            {
                HiddenField hfdBusinessID_ = (HiddenField)row22.FindControl("hfdBusinessID");
                if (hfdBusinessID_.Value != "" || hfdBusinessID_.Value != "0" || hfdBusinessID_.Value != "NULL")
                {
                    if (hfdBusinessID_.Value == hfd_BussinessId_.Value)
                    {
                        if (__hfd_UserRoleId.Value == "5")
                            row22.Enabled = false;
                        else if (__hfd_UserRoleId.Value == "12")
                            row22.Enabled = false;
                        else
                            row22.Enabled = true;
                    }
                }
            }
        }
    }

    public void DisableProject()
    {
        foreach (GridViewRow row11 in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
            HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");

            if (__hfd_UserRoleId.Value == "5" || __hfd_UserRoleId.Value == "12")
            {
                if (hfd_BussinessId_.Value == ddlBusAssProject.SelectedValue)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Another Business as User is selected as Administrator or Accountant User Role in Selected Business', 'warning');", true);
                    return;
                }
            }
        }
    }

    protected void btn_Okay_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home_Page.aspx");
    }

    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Add_EmployeeV2.aspx");
    }

    protected void txtHiredate_TextChanged(object sender, EventArgs e)
    {
        if (txtHiredate.Text != "")
        {
            DateTime Min_Date = DateTime.Parse("01/01/1753");
            DateTime Selected_Date = DateTime.Parse(txtHiredate.Text);

            if (Selected_Date < Min_Date)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Hire Date Correctly!', 'warning');", true);
                return;
            }

            DataTable dt_get_hiredate = comm.GetAllDataFromTable("RateDetails with (Nolock)", "StartDate, cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MIN(RateId)from RateDetails where EmployeeId=" + ddl_Employee.SelectedValue + " and effectivedate <= GETDATE())", "");

            bool check_if_timesheet_exits = comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "EmpId='" + ddl_Employee.SelectedValue + "'");

            if (check_if_timesheet_exits)
            {
                DateTime HireDate = DateTime.Parse(dt_get_hiredate.Rows[0]["StartDate"].ToString());
                txtHiredate.Text = HireDate.ToString("yyyy-MM-dd");
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be edited as timesheet is already entered', 'warning');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be edited as timesheet has already been entered', 'warning');", true);
                return;
            }

            foreach (GridViewRow row123 in GvRateDetails.Rows)
            {
                TextBox txtstdate = (TextBox)row123.FindControl("txtstdate");

                txtstdate.Text = txtHiredate.Text;
            }
        }
    }
    private void showEmployeeHistory(int employee, int business)
    {
        try
        {
            using(var c=AnyminuteTimesheetRepair.Open())
                AnyminuteTimesheetRepair.Authorize(c,null,Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),business,employee,false);
            gridViewEmployeeHistory.DataSource = AnyminuteEmployeeRepair.StatusHistory(employee);
            gridViewEmployeeHistory.DataBind();
            gridViewRoleHistory.DataSource = AnyminuteEmployeeRepair.RoleHistory(employee,business);
            gridViewRoleHistory.DataBind();
            modalEmployeeHistory.Show();
        }
        catch(InvalidOperationException ex) { EmployeeRepairMessage(ex.Message); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Employee history load failed",ex); EmployeeRepairMessage("Unable to load history. Contact support."); }
    }
    protected void lnk_EmployeeHistory_Click(object sender, EventArgs e)
    {
        var row = (GridViewRow)((LinkButton)sender).NamingContainer;
        showEmployeeHistory(int.Parse(((HiddenField)row.FindControl("hdf_EmpId")).Value), int.Parse(((HiddenField)row.FindControl("hfd_BussinessId")).Value));
    }

    protected void btnDeleteUser_Click(object sender, EventArgs e)
    {
        if (ddl_Employee.SelectedIndex ==0)
        { return; }

        int EmployeeID = Int32.Parse(ddl_Employee.SelectedValue);
        bool UserDeleted =false;
        UserDeleted = comm.DeleteUser(EmployeeID);
        if (UserDeleted==true)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'User deleted Successfully!', 'success');", true);
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'User deleted Successfully!', 'success');", true);  // this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Address not recorded previously', 'warning');", true);

            //ScriptManager.RegisterStartupScript(this, GetType(), "ReloadPage", "window.location.reload();", true);
           // Response.Redirect(Request.RawUrl);
            Div_Personaldetails_Info.Visible = false;
            Div_Personaldetails_Nav_Button.Visible = false;
            ddl_Employee.SelectedIndex = 0;
            ddl_Employee.Enabled = true;
            ddl_Business.Enabled = true;
            ddl_Employee.Items.RemoveAt(ddl_Employee.SelectedIndex);

            ddl_Employee.Items.Insert(0, new ListItem("Please Select"));

            ddl_Employee.SelectedIndex = 0;
        }



    }

    protected void btnUpdateRole_Click(object sender, EventArgs e)
    {
        try
        {
            int employee,business,role;
            if(!int.TryParse(ddl_Employee.SelectedValue,out employee) || !int.TryParse(ddl_Business.SelectedValue,out business) || !int.TryParse(ddlRole.SelectedValue,out role))
                throw new InvalidOperationException("Select the employee, business, and new role first.");
            AnyminuteEmployeeRepair.UpdateRole(
                Convert.ToInt32(Session["EmployeeId"]),
                Convert.ToInt32(Session["UserTypeid"]),
                business,
                employee,
                role);

            // Assignment, mapping de-duplication and history are committed together by the service.
            dtSelectedBuss = new DataTable();
            dtSelectedBuss.Columns.AddRange(new DataColumn[7] { new DataColumn("EmployeeID"),new DataColumn("BusinessName"),new DataColumn("BussinessId"),new DataColumn("EmployeeName"),new DataColumn("UserRole"),new DataColumn("UserRoleId"),new DataColumn("RoleId") });
            DataTable email = comm.GetAllDataFromTable("EmpAddress", "MainEmail", "EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID=" + employee + ")", "");
            if(email.Rows.Count>0) GetAssgineBuss(Convert.ToString(email.Rows[0]["MainEmail"]), Convert.ToString(Session["UserTypeid"]));
            Gv_RolePermission.DataSource = ViewState["RolePermission"];
            Gv_RolePermission.DataBind();
            EmployeeRepairMessage("Business role updated. Review permissions for the new role. The employee should sign in again.");
        }
        catch(InvalidOperationException ex) { EmployeeRepairMessage(ex.Message); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Employee role update failed",ex); EmployeeRepairMessage("Unable to update the role. Refresh and try again."); }
    }
}