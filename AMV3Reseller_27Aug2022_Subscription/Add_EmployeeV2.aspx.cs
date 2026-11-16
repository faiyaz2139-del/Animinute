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

public partial class Add_EmployeeV2 : System.Web.UI.Page
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
    AssgineManagerEntity asaMangEntity = new AssgineManagerEntity();
    AssgineManagerController asMangCtrl = new AssgineManagerController();
    Cls_UserType ut = new Cls_UserType();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        // Preserve tabid across postbacks. The previous code expired #ratedetils/#project
        // before the tab-restoration logic ran, causing Add User/Edit User to jump tabs.

        if (!IsPostBack)
        {
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
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1) // v82: was ==0 (inverted), same fix as Add_Business
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnFinalSave.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }

                            var empid = Session["EmployeeId"].ToString();
                            DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
                            int empcount = 0;
                            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            //DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                            DataTable dtBussList = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
                            for (int m = 0; m < dtBussList.Rows.Count; m++)
                                empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
                            if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            {
                                Label_Maximum_Limit.Text = "Please contact admin to add more user. You have reached the maximum limit.";
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

                            //This logic commented is as per the reseller not the reseller subscriber
                            //int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
                            //DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
                            //for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
                            //{
                            //    DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
                            //    total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
                            //}

                            //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                            //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and IsActive=1)");
                            //int EMPCNT_RESELLER = 0;
                            //EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                            //if (total_no_of_user >= EMPCNT_RESELLER)
                            //{
                            //    Label_Maximum_Limit.Text = "Please contact admin to add more user. You have reached the maximum limit.";
                            //    Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //    btnFinalSave.Visible = false;
                            //    DisableAllControlsInPage(Page, false);
                            //    return;
                            //}

                            DataTable dt_check_if_res_sub_regsitered_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=1 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_if_res_sub_regsitered_status.Rows.Count == 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact reseller. Your plan is not activated.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnFinalSave.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }

                            DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory where IsActive=0 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_active_status.Rows.Count == 0)
                            {
                                var empid = Session["EmployeeId"].ToString();
                                DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
                                int empcount = 0;
                                DataTable dtBussList = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                    "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                    "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                    "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                    "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                    "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' " +
                                    "and ASB.MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
                                for (int m = 0; m < dtBussList.Rows.Count; m++)
                                    empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
                                if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                                {
                                    Label_Maximum_Limit.Text = "Please contact admin to add more user. You have reached the maximum limit.";
                                    Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                    btnFinalSave.Visible = false;
                                    DisableAllControlsInPage(Page, false);
                                    return;
                                }
                            }
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

            ViewState["BGridEdit"] = false;
            ViewState["PGvProjectEdit"] = false;
            ViewState["QueryStringRole"] = "";
            ViewState["RoleId_PopAdd"] = "";
            if (Request.QueryString["Role"] != null)
            {
                ViewState["QueryStringRole"] = Request.QueryString["Role"].ToString();
            }
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
                var dt = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Session["EmployeeId"].ToString() + " and BusinessID is not null and BusinessID<>''", "");
                if (dt.Rows.Count > 0)
                {
                    ViewState["bid"] = dt.Rows[0][0].ToString();
                }
                else
                {
                    ViewState["bid"] = 0;
                }
            }
            btnAdd.Visible = true;
            btnNext.Visible = false;
            Bindbusiness();
            BindAllUserType();
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
                Lihomemid.Attributes["class"] = "active";
                LiRateDetails.Attributes["class"] = "";
                LiProject.Attributes["class"] = "";
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
                LiRateDetails.Attributes.Add("class", "active");
                //Lirolemid.Attributes.Add("class", "");
                Lihomemid.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "");
            }
            else if (cookvalue == "#project")
            {
                project.Attributes.Add("class", "tab-pane fade in active show");
                //permission.Attributes.Add("class", "tab-pane fade in");
                home.Attributes.Add("class", "tab-pane fade in");
                ratedetils.Attributes.Add("class", "tab-pane fade in");
                LiRateDetails.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "active");
                Lihomemid.Attributes.Add("class", "");
                //Lirolemid.Attributes.Add("class", "");
            }
        }
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
            }
        }
    }

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
                    GetPersonalDetailsForEdit(txtMainEmail.Text);
                    GetAssgineBuss(txtMainEmail.Text);
                    GetRateDetails(txtMainEmail.Text);
                    GetAssgineProject(txtMainEmail.Text);
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

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
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

    protected void btnNextPerson_Click(object sender, EventArgs e)
    {

    }

    protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
    {
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



    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int cnt = 0;
        if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either First or Last Name is Blank', 'error');", true);
            return;
        }
        if (txtHiredate.Text == "")
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter hire date in personal details', 'warning');", true);
            return;
        }
        DateTime hdate;
        if (!TryHireDate(out hdate)) return;
        DateTime currdate = DateTime.Now.Date;
        if (hdate > currdate)
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be greater than current date in personal details', 'warning');", true);
            return;
        }
        if (ddlBusiness.SelectedValue == "" || ddlRole.SelectedValue == "" || ddlRole.SelectedValue == "0")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either Business Name or Role is Not Select', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Assign Role to Selected Business', 'error');", true);
            return;
        }

        Check_If_Email_registered_As_Subscriber();

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
                    var empidddd = Session["EmployeeId"].ToString();
                    DataTable dtBussCnttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empidddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empidddd + "' and IsActive=1)");
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

                    if (empcounttt >= int.Parse(dtBussCnttt.Rows[0][0].ToString()))
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                        return;
                    }

                    #region amit's commented code
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
                    ////empcounttt = Convert.ToInt32(dt.Rows[0]["CNT"].ToString());
                    //if (empcounttt >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add more Users in this Business as Maximum Limit for Number of Users has Reached, Please Contact Admin', 'error');", true);
                    //    return;
                    //}
                    #endregion
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
                        "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
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
            btnAdd.Visible = false;
            btnNext.Visible = true;
        }

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
                        DataTable dtBussCntttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=1)");
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
                        //old logic coommented
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
                        if (dt_check_active_status.Rows.Count > 0)
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'User has been assigned role, Please add more users to add a different role', 'error');", true);
            return;
        }

        DataTable dt = (DataTable)ViewState["RolePermission"];
        DataTable dtrate = (DataTable)ViewState["rateDetails"];
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusiness.SelectedItem.Text;
        var rolename = ddlRole.SelectedItem.Text;
        var StartDate_RateGrid = txtHiredate.Text;
        //StringBuilder StringBuilder__ = new StringBuilder();
        //var AllRole = comm.ExecuteQueryDataTable("select Role_Permission_Id FROM Role_Permission_Master WHERE UserTypeId='" + ddlRole.SelectedValue + "'");
        //for (int i = 0; i < AllRole.Rows.Count; i++)
        //{
        //    StringBuilder__.Append(AllRole.Rows[i][0].ToString() + ",");
        //}
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
        //commented because the the hire date will be the start date
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
        
    }

    public void InitialSelectBuss()
    {
        dtSelectedBuss.Columns.AddRange(new DataColumn[7] { new DataColumn("EmployeeID"), new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("UserRole"), new DataColumn("UserRoleId"), new DataColumn("RoleId") });
        ViewState["RolePermission"] = dtSelectedBuss;
        this.BindGrid();

        dtSelectProject.Columns.AddRange(new DataColumn[8] { new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("EmployeeID"), new DataColumn("UserRole"), new DataColumn("UserRoleId"), new DataColumn("ProjectId"), new DataColumn("ProjectName") });
        ViewState["ProjetName"] = dtSelectProject;
        BindProjectGrid();

        dtSelectRate.Columns.AddRange(new DataColumn[12] { new DataColumn("RateId"), new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("EmployeeID"), new DataColumn("Rate"), new DataColumn("RateType"), new DataColumn("StartDate"), new DataColumn("EndDate"), new DataColumn("IsEmployeeEIExempty"), new DataColumn("RetainVacation"), new DataColumn("IsEmployeeCPPExempty") });
        ViewState["rateDetails"] = dtSelectRate;
        BindRate();
    }
    protected void BindRate()
    {
        GvRateDetails.DataSource = (DataTable)ViewState["rateDetails"];
        GvRateDetails.DataBind();
    }
    protected void BindGrid()
    {
        Gv_RolePermission.DataSource = (DataTable)ViewState["RolePermission"];
        Gv_RolePermission.DataBind();

    }

    protected void BindProjectGrid()
    {
        GvProject.DataSource = (DataTable)ViewState["ProjetName"];
        GvProject.DataBind();
        //Gv_RolePermission.DataSource = (DataTable)ViewState["ProjetName"];
        //Gv_RolePermission.DataBind();
    }

    protected void Gv_RolePermission_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Gv_RolePermission.EditIndex = e.NewEditIndex;
        ViewState["BGridEdit"] = true;

        BindGrid();


    }

    protected void Gv_RolePermission_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        Gv_RolePermission.EditIndex = -1;
        ViewState["BGridEdit"] = false;
        BindGrid();
    }

    protected void Gv_RolePermission_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfd_UserRoleIdEdit = (e.Row.FindControl("hfd_UserRoleIdEdit") as HiddenField);
            HiddenField hfd_UserRoleId = (e.Row.FindControl("hfd_UserRoleId") as HiddenField);
            HiddenField hdf_EmpId = (e.Row.FindControl("hdf_EmpId") as HiddenField);
            LinkButton lnk_ChangeRoleName = (e.Row.FindControl("lnk_ChangeRoleName") as LinkButton);
            LinkButton lnk_EditPermission = (e.Row.FindControl("lnk_EditPermission") as LinkButton);
            LinkButton lnk_delete = (e.Row.FindControl("lnk_delete") as LinkButton);
            Label lbl_separate = (e.Row.FindControl("lbl_separate") as Label);
            Label lbl_separate2 = (e.Row.FindControl("lbl_separate2") as Label);
            if (hdf_EmpId.Value != "0")
            {
                lnk_ChangeRoleName.Enabled = false;
                lnk_EditPermission.Enabled = false;
                lnk_delete.Visible = false;
                lbl_separate2.Visible = false;
            }
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
        }
    }

    protected void Gv_RolePermission_PreRender(object sender, EventArgs e)
    {

        if (Gv_RolePermission.Rows.Count > 0)
        {
            Gv_RolePermission.UseAccessibleHeader = true;
            Gv_RolePermission.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }



    protected void Gv_RolePermission_RowCommand(object sender, GridViewCommandEventArgs e)
    {
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
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                var pid = "";
                GridViewRow row = Gv_RolePermission.Rows[rowIndex];
                HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
                HiddenField hdf_EmpId = (row.FindControl("hdf_EmpId") as HiddenField);
                DataTable dt = (DataTable)ViewState["RolePermission"];
                DataTable dtproject = (DataTable)ViewState["ProjetName"];
                DataTable dtarte = (DataTable)ViewState["rateDetails"];
                DataRow[] rowsToRemove = dt.Select("BussinessId=" + hfd_BussinessId.Value + "");
                foreach (var rowToDelete in rowsToRemove)
                    dt.Rows.Remove(rowToDelete);
                ViewState["RolePermission"] = dt;

                DataRow[] ProrowsToRemove = dtproject.Select("BussinessId=" + hfd_BussinessId.Value + "");
                foreach (var rowToDelete in ProrowsToRemove)
                    dtproject.Rows.Remove(rowToDelete);
                ViewState["ProjetName"] = dtproject;

                DataRow[] RaterowsToRemove = dtarte.Select("BussinessId=" + hfd_BussinessId.Value + "");
                foreach (var rowToDelete in RaterowsToRemove)
                    dtarte.Rows.Remove(rowToDelete);
                ViewState["rateDetails"] = dtarte;

                this.BindGrid();
                this.BindProjectGrid();
                this.BindRate();


                ddlBusAssProject.Items.Clear();
                ddlBusAssProject.Items.Add("Please Select");
                ddlBusAssProject.SelectedItem.Value = "";
                ddlBusAssProject.DataSource = dt;
                ddlBusAssProject.DataValueField = dt.Columns[2].ToString();
                ddlBusAssProject.DataTextField = dt.Columns[1].ToString();
                ddlBusAssProject.DataBind();
            }


        }
    }

    protected void Gv_EditRolePop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
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

    protected void ddlBusAssProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlBusAssProject.SelectedValue))
        {
            //DisableProject();

            foreach (GridViewRow row11 in Gv_RolePermission.Rows)
            {
                HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
                HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");

                if (__hfd_UserRoleId.Value == "5" || __hfd_UserRoleId.Value == "12")
                {
                    if (hfd_BussinessId_.Value == ddlBusAssProject.SelectedValue)
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Another Business as User is selected as Administrator or Accountant User Role in Selected Business', 'warning');", true);
                        ddlBusAssProject.SelectedIndex = 0;
                        return;
                    }
                }
            }

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
    protected void GvProject_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GvProject.EditIndex = e.NewEditIndex;
        ViewState["PGvProjectEdit"] = true;
        BindProjectGrid();
    }

    protected void GvProject_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
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
            LinkButton lnk_delete = (e.Row.FindControl("lnk_delete") as LinkButton);
            Label lbl_separate3 = (e.Row.FindControl("lbl_separate3") as Label);
            if (hfd_roleId.Value != "0")
            {
                lnk_ChangeRoleName.Enabled = false;
                lnk_delete.Visible = false;
                lbl_separate3.Visible = false;
            }
            bool PGvProjectEdit = (bool)ViewState["PGvProjectEdit"];
            if (PGvProjectEdit == true)
            {
                lnk_ChangeRoleName.Enabled = false;
                lnk_delete.Enabled = false;
                lbl_separate3.Enabled = false;
            }
            else
            {
                lnk_ChangeRoleName.Enabled = true;
                lnk_delete.Enabled = true;
                lbl_separate3.Enabled = true;
            }
        }
    }

    protected void GvProject_PreRender(object sender, EventArgs e)
    {

    }

    protected void GvProject_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteProject")
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
                GridViewRow row = GvProject.Rows[rowIndex];
                HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
                HiddenField hfd_ProjectNameId = (row.FindControl("hfd_ProjectNameId") as HiddenField);
                HiddenField hfd_EmployeeIDpro = (row.FindControl("hfd_EmployeeIDpro") as HiddenField);
                DataTable dt = (DataTable)ViewState["ProjetName"];
                DataRow[] rowsToRemove = dt.Select("BussinessId=" + hfd_BussinessId.Value + " and ProjectId=" + hfd_ProjectNameId.Value + " and EmployeeID=" + hfd_EmployeeIDpro.Value);
                foreach (var rowToDelete in rowsToRemove)
                    dt.Rows.Remove(rowToDelete);
                ViewState["ProjetName"] = dt;
                this.BindProjectGrid();
            }

        }
    }

    protected void btnAssProject_Click(object sender, EventArgs e)
    {
        if (ddlBusAssProject.SelectedValue == "" || ddlAssProjectName.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Info!', 'Select Business And Assign Project', 'info');", true);
            return;
        }

        //DisableProject();

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

        int cnt = 0;
        DataTable dt = (DataTable)ViewState["ProjetName"];
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusAssProject.SelectedItem.Text;
        var projectName = ddlAssProjectName.SelectedItem.Text;

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
        dt.Rows.Add(bussname, ddlBusAssProject.SelectedValue, EmpName, 0, 0, 0, Convert.ToInt32(ddlAssProjectName.SelectedValue), projectName);
        ViewState["ProjetName"] = dt;

        BindProjectGrid();

        //DataTable dt = (DataTable)ViewState["ProjetName"];
        //var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        //var bussname = ddlBusAssProject.SelectedItem.Text;
        //var projectName = ddlAssProjectName.SelectedItem.Text;
        //dt.Rows.Add(bussname, ddlBusAssProject.SelectedValue, EmpName, projectName, ddlAssProjectName.SelectedValue);
        //ViewState["ProjetName"] = dt;
        //BindProjectGrid();
    }

    protected void ddlAssProjectName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlAssProjectName.SelectedValue == "0")
        {
            lblerror.Text = "";
            lblerror1.Text = "";
            ModelAddNewProject.Show();
        }
    }

    public int SavePersonalDetails(int UserType, string userTypetext)
    {

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





            if (string.IsNullOrEmpty(Convert.ToString(ViewState["profile"])))
            {
                ViewState["profile"] = "";
            }



            return Empid;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int SaveProfilePic(int empid, string str, int UserTypeid, string UserType)
    {
        Uae.EmployeeId = empid;

        //Uae.Email = txtMainEmail.Text.Trim();
        if (txtMainEmail.Text != "")
            Uae.Email = txtMainEmail.Text.Trim(); ;
        if (txtMainEmail.Text == "")
            Uae.Email = blank_email_id;

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
            EmpAddEnity.SuitType = DropDownList2.SelectedValue;// "";
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

            if (txtMainEmail.Text != "")
                EmpAddEnity.MainEmail = txtMainEmail.Text.Trim(); ;
            if (txtMainEmail.Text == "")
                EmpAddEnity.MainEmail = blank_email_id;

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

            comm.ExecuteQuery("update User_Role_Permission set Role_Permission_Id='" + permission + "', BusinessId='" + bussid + "'  where Employee_Id ='" + empid + "' and UserTypeId = '" + usertype + "' and Status='True' ");
        }
        else
        {

            comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) Values ('" + permission + "', '" + empid + "', '" + usertype + "', 'True', '" + bussid + "')");
        }
    }

    public void SaveAssgineProject(int bussid, int EmpId)
    {
        foreach (GridViewRow row in GvProject.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_ProjectNameId = row.FindControl("hfd_ProjectNameId") as HiddenField;
            Label lbl_ProjectName = row.FindControl("lbl_ProjectName") as Label;

            if (int.Parse(hfd_BussinessId.Value) == bussid)
            {
                ape.BusinessID = Convert.ToInt32(hfd_BussinessId.Value);
                ape.EmployeeID = Convert.ToInt32(EmpId);
                ape.ProjectID = Convert.ToInt32(hfd_ProjectNameId.Value);
                apc.InsertAssgineProject(ape);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='P' WHERE EmployeeID=" + EmpId + "");
            }

        }
    }
    public int SavePay(int _empid, int Buss, int RoleId)
    {
        int ResultSavePay = 0;
        try
        {

            foreach (GridViewRow row in GvRateDetails.Rows)
            {

                HiddenField hfdRateBusinessID = row.FindControl("hfdRateBusinessID") as HiddenField;
                HiddenField hfdRate = row.FindControl("hfdRate") as HiddenField;
                TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
                TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
                TextBox txtRate = row.FindControl("txtRate") as TextBox;
                DropDownList ddlPaytpe = row.FindControl("ddlPaytpe") as DropDownList;
                DropDownList ddlEmpEI11 = row.FindControl("ddlEmpEI11") as DropDownList;
                DropDownList ddlEmpEI22 = row.FindControl("ddlEmpEI22") as DropDownList;
                DropDownList ddlEmpEI33 = row.FindControl("ddlEmpEI33") as DropDownList;
                if (int.Parse(hfdRateBusinessID.Value) == Buss)
                {
                    if (hfdRate.Value == "0")
                    {
                        RateEntity.EmployeeId = _empid;
                        RateEntity.Rate = float.Parse(txtRate.Text);
                        //RateEntity.RateType = ddlPaytpe.SelectedItem.Text;
                        RateEntity.RateType = ddlPaytpe.SelectedValue;
                        //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
                        if (RoleId == 12 || RoleId == 5)
                        {
                            RateEntity.StartDate = System.DateTime.Now.Date.AddYears(100);
                            //RateEntity.EndDate = System.DateTime.Now.Date;
                            if (string.IsNullOrEmpty(txtEndDate.Text))
                            {
                                RateEntity.EndDate = System.DateTime.Now.Date.AddYears(100);
                            }
                            else
                            {
                                RateEntity.EndDate = System.DateTime.Now.Date.AddYears(100);
                            }
                        }
                        else
                        {
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
                        }

                        RateEntity.JobTitle = "";
                        RateEntity.IsEmployeeEIExempty = ddlEmpEI11.SelectedValue;
                        RateEntity.IsEmployeeCPPExempty = ddlEmpEI22.SelectedValue;
                        RateEntity.RetainVacation = ddlEmpEI33.SelectedValue;
                        //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                        RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
                        ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);
                    }

                }



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

    public void check_rate_validation_not_for_admin_and_acc(HiddenField __hfd_BussinessId)
    {
        #region validation for pay details in 4th tab grid view
        foreach (GridViewRow row in GvRateDetails.Rows)
        {
            TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
            TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
            TextBox txtRate = row.FindControl("txtRate") as TextBox;
            HiddenField __hfdRateBusinessID = row.FindControl("hfdRateBusinessID") as HiddenField;

            if (__hfd_BussinessId.Value == __hfdRateBusinessID.Value)
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
                if (txtRate.Text == "" || txtRate.Text == "0")
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Bill Rate Properly', 'warning');", true);
                    return;
                }
            }
        }
        #endregion
    }

    public int ValidateRate()
    {
        int cnt = 0;
        foreach (GridViewRow row in GvRateDetails.Rows)
        {
            TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
            TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
            TextBox txtRate = row.FindControl("txtRate") as TextBox;
            DropDownList ddlPaytpe = row.FindControl("ddlPaytpe") as DropDownList;
            if (string.IsNullOrEmpty(txtstdate.Text) || string.IsNullOrEmpty(txtRate.Text) || string.IsNullOrEmpty(ddlPaytpe.SelectedValue))
            {
                cnt++;
            }
        }
        return cnt;
    }

    public string RandomDigits(int length)
    {
        var random = new Random();
        string s = string.Empty;
        for (int i = 0; i < length; i++)
            s = String.Concat(s, random.Next(10).ToString());
        return s;
    }

    string blank_email_id;
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        //AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
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
        DateTime hdate;
        if (!TryHireDate(out hdate)) return;
        DateTime currdate = DateTime.Now.Date;
        if (hdate > currdate)
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be greater than current date in personal details', 'warning');", true);
            return;
        }
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


        //int check_role_id_admin_acc = 0;
        //foreach (GridViewRow row in Gv_RolePermission.Rows)
        //{
        //    HiddenField __hfd_UserRoleId = row.FindControl("hfd_UserRoleId") as HiddenField;
        //    HiddenField __hfd_RoleId = row.FindControl("hfd_RoleId") as HiddenField;
        //    HiddenField __hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
        //    if (__hfd_UserRoleId.Value == "5" || __hfd_UserRoleId.Value == "12")
        //        check_role_id_admin_acc = 1;
        //    if (__hfd_RoleId.Value == "5" || __hfd_RoleId.Value == "12")
        //        check_role_id_admin_acc = 2;
        //    else
        //    {
        //        check_rate_validation_not_for_admin_and_acc(__hfd_BussinessId);
        //    }
        //}

        foreach (GridViewRow row123 in Gv_RolePermission.Rows)
        {
            HiddenField __hfd_UserRoleId = (HiddenField)row123.FindControl("hfd_UserRoleId");

            if (__hfd_UserRoleId.Value == "12")
            {
                if (txtMainEmail.Text == "")
                {
                    txtMainEmail.Focus();
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter Email Address as Accountant Role is Selected', 'warning');", true);
                    return;
                }
            }
        }

        foreach (GridViewRow row11 in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
            HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");

            if (__hfd_UserRoleId.Value != "5" && __hfd_UserRoleId.Value != "12")
            {
                foreach (GridViewRow row22 in GvRateDetails.Rows)
                {
                    HiddenField hfdRateBusinessID_ = (HiddenField)row22.FindControl("hfdRateBusinessID");
                    TextBox txtstdate = (TextBox)row22.FindControl("txtstdate");
                    TextBox txtEndDate = (TextBox)row22.FindControl("txtEndDate");
                    TextBox txtRate = (TextBox)row22.FindControl("txtRate");
                    DropDownList ddlPaytpe = row22.FindControl("ddlPaytpe") as DropDownList;

                    if (hfdRateBusinessID_.Value == hfd_BussinessId_.Value)
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
                        if (txtRate.Text == "" || txtRate.Text == "0")
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Enter Bill Rate Properly', 'warning');", true);
                            return;
                        }
                        if (string.IsNullOrEmpty(ddlPaytpe.SelectedValue))
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Please Select Bill Type Properly', 'warning');", true);
                            return;
                        }
                    }
                }
            }
        }


        string random_number = RandomDigits(15);
        blank_email_id = random_number + "blank";


        #region validation to select the assign business and assign role in Gv_RolePermission grid
        if (Gv_RolePermission.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Assign Business And Assign Role And Then Click on Add Button', 'warning');", true);
            return;
        }
        #endregion

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

        #region duplicate entry validations
        if (txtMainEmail.Text != "")
        {
            StringBuilder checkbussid_ = new StringBuilder();
            foreach (GridViewRow row in Gv_RolePermission.Rows)
            {
                HiddenField hfd_BussinessId_ = row.FindControl("hfd_BussinessId") as HiddenField;
                HiddenField hdf_EmpId_ = row.FindControl("hdf_EmpId") as HiddenField;
                Label lbl_BusinessName_ = row.FindControl("lbl_BusinessName") as Label;
                checkbussid_.Append(hfd_BussinessId_.Value + ",");
                if (hdf_EmpId_.Value == "0")
                {
                    bool check_email_in_business = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID =" + int.Parse(hfd_BussinessId_.Value) + " and ea.MainEmail='" + txtMainEmail.Text + "' ");
                    if (check_email_in_business)
                    {
                        if (txtMainEmail.Text != "")
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'User Email Id is Already Registered in " + lbl_BusinessName_.Text + " business', 'warning');", true);
                        //if (txtMainEmail.Text == "")
                        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Blank Email Id is Already Registered in " + lbl_BusinessName_.Text + " business', 'warning');", true);
                        return;
                    }
                }
            }
        }
        //string businessesid = checkbussid_.Remove(checkbussid_.Length - 1);
        //bool Chk2 = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID in (+ BussNo2 +) and ea.MainEmail='" + txtMainEmail.Text + "' ");
        //if (Chk2 == true)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current  Business', 'warning');", true);
        //    return;
        //}
        #endregion        

        //if (check_role_id_admin_acc == 0)
        //{
        //    if (ValidateRate() > 0)
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Enter Rate Details Properly', 'warning');", true);
        //        return;
        //    }
        //}

        #region no of users validation. Commented code
        //if (Session["UserTypeid"].ToString() == "1")
        //{
        //    var check_reseller_email = comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
        //    bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
        //    if (!check_reseller_or_subscriber_email)
        //    {
        //        var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' ");
        //        if (check_old_subscriber.Rows.Count > 0)
        //        {
        //            var empid = Session["EmployeeId"].ToString();
        //            DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
        //            int empcount = 0;
        //            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
        //            //DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
        //            DataTable dtBussList = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
        //                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
        //                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
        //                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
        //                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
        //                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
        //            for (int m = 0; m < dtBussList.Rows.Count; m++)
        //                empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
        //            if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add More Users, Contact Admin', 'error');", true);
        //                return;
        //            }
        //        }
        //    }
        //    if (check_reseller_or_subscriber_email)
        //    {
        //        var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
        //        if (check_old_reseller.Rows.Count > 0)
        //        {
        //            int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
        //            DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
        //            for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
        //            {
        //                DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
        //                total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
        //            }

        //            DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
        //            int EMPCNT_RESELLER = 0;
        //            EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

        //            if (total_no_of_user >= EMPCNT_RESELLER)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Add More Users, Contact Admin', 'error');", true);
        //                return;
        //            }
        //        } 
        //    }
        //}
        #endregion

        Check_If_Email_registered_As_Subscriber();

        int _Empid = 0, _AddressID, _bussid;
        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_RoleId = row.FindControl("hfd_RoleId") as HiddenField;
            Label lbl_BusinessName = row.FindControl("lbl_BusinessName") as Label;
            HiddenField hfd_UserRoleId = row.FindControl("hfd_UserRoleId") as HiddenField;
            HiddenField hdf_EmpId = row.FindControl("hdf_EmpId") as HiddenField;
            Label lbl_UserRole = row.FindControl("lbl_UserRole") as Label;

            if (hdf_EmpId.Value == "0")
            {
                _bussid = int.Parse(hfd_BussinessId.Value);
                DataTable dtbussWiseMotherOrgId = comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + _bussid + "", "");
                if (dtbussWiseMotherOrgId.Rows.Count > 0)
                {
                    ViewState["bussWiseMotherOrgId"] = dtbussWiseMotherOrgId.Rows[0][0].ToString();
                }

                _Empid = SavePersonalDetails(Convert.ToInt32(hfd_UserRoleId.Value), lbl_UserRole.Text);

                SavePay(_Empid, _bussid, Convert.ToInt32(hfd_UserRoleId.Value));
                var UserId = SaveProfilePic(_Empid, ViewState["profile"].ToString(), int.Parse(hfd_UserRoleId.Value), lbl_UserRole.Text);
                _AddressID = SaveAddress(_Empid);
                string EmployeeName = comm.GetNameFromId("AddEmployee", _Empid);
                AddBeCtrl.AssgineBusiness(Convert.ToInt32(hfd_BussinessId.Value), _Empid, lbl_BusinessName.Text, EmployeeName, Convert.ToInt32(hfd_UserRoleId.Value), Convert.ToInt32(ViewState["bussWiseMotherOrgId"].ToString()));
                SaveUpdateRolePermission(_Empid, int.Parse(hfd_BussinessId.Value), int.Parse(hfd_UserRoleId.Value), hfd_RoleId.Value);
                SaveAssgineProject(int.Parse(hfd_BussinessId.Value), _Empid);
                comm.ExecuteQuery("insert into MappingTable(USERID,UserTypeId,EmployeeID,BusinessID)values(" + UserId + "," + int.Parse(hfd_UserRoleId.Value) + "," + _Empid + "," + hfd_BussinessId.Value + ")");
                comm.ExecuteQuery("UPDATE AssginBussiness SET UserTypeId=" + hfd_UserRoleId.Value + " WHERE EmployeeID=" + _Empid + " AND BusinessID=" + hfd_BussinessId.Value + "");

                var getUserId = Session["UserId"].ToString();
                var MangerBid = comm.GetAllDataFromTable("mappingtable", "BusinessID", "userid=" + getUserId + " and UserTypeId=2 and BusinessID=" + hfd_BussinessId.Value + "", "");
                if (MangerBid.Rows.Count > 0)
                {
                    var AssgineMangerId = comm.GetAllDataFromTable("mappingtable", "EmployeeID", "userid=" + getUserId + " and UserTypeId=2 and BusinessID=" + hfd_BussinessId.Value + "", "").Rows[0][0].ToString();

                    if (hfd_BussinessId.Value == MangerBid.Rows[0][0].ToString())
                    {
                        asaMangEntity.BusinessID = int.Parse(hfd_BussinessId.Value);
                        asaMangEntity.BusinessName = lbl_BusinessName.Text;

                        asaMangEntity.ManagerID = int.Parse(AssgineMangerId);
                        asaMangEntity.MangerName = comm.GetNameFromId("AddEmployee", int.Parse(AssgineMangerId));
                        asaMangEntity.EmpId = _Empid;
                        asaMangEntity.EmpName = EmployeeName;
                        asMangCtrl.InsertAssgineManger(asaMangEntity);
                    }
                }
            }
        }

        #region added by kumar for mail sending
        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
        if (IsSend)
        {
            if (txtMainEmail.Text != string.Empty)
            {
                //DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID desc");
                DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.MainEmail='" + txtMainEmail.Text + "'", "ae.EmployeeID asc");
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
                        if (txtMainEmail.Text != "")
                        {
                            Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                        }
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Email Send to Your Register Mail', 'success');", true);
                    }
                    else
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Not Found', 'warning');", true);
                    }
                }
            }
        }
        #endregion

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

    AddEmployeeEntityClass EmpEc = new AddEmployeeEntityClass();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    private SqlConnection con;
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
    public void AssgineManger(int PassBid)
    {


    }

    protected void ddlEditRolePer_SelectedIndexChanged(object sender, EventArgs e)
    {
        Label33.Text = "";
        Label33.Visible = false;
        ViewState["Event"] = true;
        GetUserPermissionRoleList(0, Gv_EditRolePop);
    }



    protected void btnSaveEditRolePop_Click(object sender, EventArgs e)
    {
        var selectRole = ddlEditRolePer.SelectedValue;
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
        Label33.Visible = false;

        DisableAdmAcc();
    }

    protected void btnsaveRolePop_Click(object sender, EventArgs e)
    {
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

    protected void btnSaveProject_Click(object sender, EventArgs e)
    {
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

    protected void Gv_RolePermission_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
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
        BindAllUserType();
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
        ddlRole.SelectedValue = hfd_UserRoleIdEdit.Value;
        BindAllUserType();

        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Role Updated Successfull', 'success');", true);


    }

    protected void GvProject_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
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
    public void GetPersonalDetailsForEdit(string email)
    {
        DataTable dtEmpID = comm.GetAllDataFromTable("AddEmployee e join EmpAddress ad on e.ADDRESSID=ad.EmpAddressId", "e.EmployeeID", "ad.MainEmail='" + txtMainEmail.Text + "'", "EmployeeID ");
        // DataTable dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*,convert(nvarchar(10),HireDate,120)HireDated", "EmployeeID=" + dtEmpID.Rows[0][0].ToString() + "", "");
        DataTable dtPersonalDetailsForEdit = EmpCtrl.Sp_GetSubsPerDetails(txtMainEmail.Text, int.Parse(Session["EmployeeId"].ToString()));

        //foreach (ListItem itm in rdoBtnUserTypeList.Items)
        //{
        //    if (itm.Value == dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString())
        //    {
        //        itm.Selected = true;
        //    }
        //}
        if (dtPersonalDetailsForEdit.Rows.Count > 0)
        {
            var profile = comm.GetAllDataFromTable("UserAcc", "Profile_Image", "email='" + txtMainEmail.Text + "'", "");
            if (profile.Rows[0][0].ToString() != string.Empty)
            {
                previewImage.ImageUrl = "~/Uploads/" + profile.Rows[0][0].ToString();
                ViewState["profile"] = profile.Rows[0][0].ToString();
            }
            //else
            //{
            //    //previewImage.ImageUrl = "~/Uploads/defaultprofile.png";
            //    previewImage.ImageUrl = "~/Uploads/profile.jpg";
            //    ViewState["profile"] = profile.Rows[0][0].ToString();
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
        }
        else
        {
            divconfirm.Visible = true;
        }

        //GetRateInformation(hfdEmpID.Value);
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

        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
    }
    public void GetAssgineBuss(string email)
    {
        DataTable dtAssgin = AddBeCtrl.Sp_GetSubsBuss(email, int.Parse(Session["EmployeeId"].ToString()), Session["UserTypeid"].ToString());

        ViewState["RolePermission"] = dtAssgin;
        BindGrid();

        ddlBussRateHistory.DataSource = dtAssgin;
        ddlBussRateHistory.Items.Clear();
        ddlBussRateHistory.Items.Add("Please Select");
        ddlBussRateHistory.DataValueField = dtAssgin.Columns[2].ToString();
        ddlBussRateHistory.DataTextField = dtAssgin.Columns[1].ToString();
        ddlBussRateHistory.DataBind();
        ddlBussRateHistory.SelectedIndex = 0;
    }
    public void GetRateDetails(string email)
    {
        DataTable dtrateDetails = RateCtrl.Sp_GetSubsBussRate(email, int.Parse(Session["EmployeeId"].ToString()), Session["UserTypeid"].ToString());
        ViewState["rateDetails"] = dtrateDetails;
        BindRate();

    }
    public void GetAssgineProject(string email)
    {
        DataTable dtAssgin = apc.Sp_GetSubsBussProject(email, int.Parse(Session["EmployeeId"].ToString()), Session["UserTypeid"].ToString());

        ViewState["ProjetName"] = dtAssgin;
        BindProjectGrid();
    }

    protected void ddlBussRateHistory_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetRate(txtMainEmail.Text);
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

    protected void gvRateHistory_PreRender(object sender, EventArgs e)
    {

    }

    protected void ddlBusiness_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBusiness.SelectedItem.Text != "Select Business")
        {
            ViewState["bid"] = ddlBusiness.SelectedValue;
            DataTable dt = new DataTable();
            dt = comm.GetAllDataFromTable("MappingTable", "[dbo].[FN_DESC]('UserType',UserTypeId)", "userid='" + Session["UserId"].ToString() + "' and BusinessID='" + ViewState["bid"] + "'", "");
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
                txtdob.Enabled = true;
                txtHiredate.Enabled = true;
                txtSINExpiry.Enabled = true;
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

        txtdob.ReadOnly = true;
        txtHiredate.ReadOnly = true;
        txtSINExpiry.ReadOnly = true;
    }

    // Native date controls submit ISO dates. Validate only completed submissions,
    // not each keystroke; SQL datetime cannot store years before 1753.
    private bool TryHireDate(out DateTime date)
    {
        bool valid = DateTime.TryParseExact(txtHiredate.Text.Trim(), "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out date);
        if (valid && date >= new DateTime(1753, 1, 1) && date.Date <= DateTime.Today)
            return true;
        txtHiredate.Focus();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert",
            "swal('Warning!', 'Enter a valid hire date between January 1, 1753 and today.', 'warning');", true);
        return false;
    }

    protected void txtHiredate_TextChanged(object sender, EventArgs e)
    {
        DateTime date;
        if (!TryHireDate(out date)) return;
        foreach (GridViewRow row in GvRateDetails.Rows)
        {
            TextBox start = row.FindControl("txtstdate") as TextBox;
            HiddenField rate = row.FindControl("hfdRate") as HiddenField;
            // Do not rewrite the effective date of an existing persisted rate.
            if (start != null && rate != null && rate.Value == "0")
                start.Text = date.ToString("yyyy-MM-dd");
        }
    }

    protected void btnPreparePay_Click(object sender, EventArgs e)
    {
        // Reuse the existing assignment validation, quota checks and draft-row creation.
        // Nothing is written to the database until the existing final Save succeeds.
        if (GvRateDetails.Rows.Count == 0) btnAdd_Click(sender, e);
        bool ready = GvRateDetails.Rows.Count > 0;
        home.Attributes["class"] = ready ? "tab-pane fade in" : "tab-pane fade in active show";
        ratedetils.Attributes["class"] = ready ? "tab-pane fade in active show" : "tab-pane fade in";
        project.Attributes["class"] = "tab-pane fade in";
        Lihomemid.Attributes["class"] = ready ? "" : "active";
        LiRateDetails.Attributes["class"] = ready ? "active" : "";
        LiProject.Attributes["class"] = "";
        Response.Cookies["tabid"].Value = ready ? "#ratedetils" : "#home";
    }

    protected void GvRateDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DisableAdmAcc();
    }

    public void DisableAdmAcc()
    {
        foreach (GridViewRow row11 in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId_ = (HiddenField)row11.FindControl("hfd_BussinessId");
            HiddenField __hfd_UserRoleId = (HiddenField)row11.FindControl("hfd_UserRoleId");

            if (GvRateDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row22 in GvRateDetails.Rows)
                {
                    HiddenField hfdRateBusinessID_ = (HiddenField)row22.FindControl("hfdRateBusinessID");
                    if (hfdRateBusinessID_.Value != "" || hfdRateBusinessID_.Value != "0" || hfdRateBusinessID_.Value != "NULL")
                    {
                        if (hfdRateBusinessID_.Value == hfd_BussinessId_.Value)
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

    public void Check_If_Email_registered_As_Subscriber()
    {
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

    protected void btn_Okay_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home_Page.aspx");
    }

    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Add_EmployeeV2.aspx");
    }
}