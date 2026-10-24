using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Time_Sheet_Entry : System.Web.UI.Page
{
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        AnyminuteCalendarNavigation.NormalizeManager(this);
        if (!IsPostBack)
        {
            // Manager direct-navigation fix: old code assumed BusinessIdFromDefaultPage/MangerId
            // query strings always existed and dereferenced them unconditionally.
            if (string.Equals(Convert.ToString(Session["UserType"]).Trim(), "Manager", StringComparison.OrdinalIgnoreCase) &&
                string.IsNullOrEmpty(Request.QueryString["BusinessIdFromDefaultPage"]))
            {
                int managerId = Convert.ToInt32(Session["EmployeeId"]);
                DataTable managerBusiness = Comm.GetAllDataFromTable("AssgineManager", "TOP 1 BusinessID",
                    "ManagerID=" + managerId, "BusinessID");
                if (managerBusiness.Rows.Count == 0)
                    managerBusiness = Comm.GetAllDataFromTable("AssginBussiness", "TOP 1 BusinessID",
                        "EmployeeID=" + managerId + " and BusinessID is not null", "BusinessID");

                if (managerBusiness.Rows.Count > 0)
                {
                    string calendarUrl = "Schedular.aspx?BusinessIdFromDefaultPage=" +
                        Convert.ToString(managerBusiness.Rows[0]["BusinessID"]) +
                        "&UserRole=Manager&MangerId=" + managerId;
                    Response.Redirect(calendarUrl, false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                Label_Maximum_Limit.Text = "No business assignment is available for this manager.";
                Label_Maximum_Limit.Visible = true;
                return;
            }

            #region show message if maximum limit is reached. This region code is same as of in Save button.
            if (Session["UserTypeid"].ToString() == "1")
            {
                var check_reseller_email = Comm.GetAllDataFromTable("MotherOrg", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                bool check_reseller_or_subscriber_email = Comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_old_subscriber = Comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_subscriber.Rows.Count > 0)
                    {
                        var check_cancelled = Comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = Comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = Comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnprv.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }

                            var empid = Session["EmployeeId"].ToString();
                            DataTable dtBussCnt = Comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
                            int empcount = 0;
                            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            //DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                            DataTable dtBussList = Comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
                            for (int m = 0; m < dtBussList.Rows.Count; m++)
                                empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
                            if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            {
                                //Label_Maximum_Limit.Text = "Please contact admin to schedule timesheet. You have reached the maximum limit.";
                                //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                //btnprv.Visible = false;
                                //DisableAllControlsInPage(Page, false);
                                //return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to schedule timesheet. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnprv.Visible = false;
                            DisableAllControlsInPage(Page, false);
                            return;
                        }
                    }
                    if (check_old_subscriber.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnprv.Visible = false;
                        DisableAllControlsInPage(Page, false);
                        return;
                    }
                }
                if (check_reseller_or_subscriber_email)
                {
                    var check_old_reseller = Comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_reseller.Rows.Count > 0)
                    {
                        var check_cancelled = Comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = Comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = Comm.GetAllDataFromTable("PlanHistory", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnprv.Visible = false;
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

                            DataTable dt_check_if_res_sub_regsitered_status = Comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=1 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_if_res_sub_regsitered_status.Rows.Count == 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact reseller. Your plan is not activated.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnprv.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }

                            DataTable dt_check_active_status = Comm.ExecuteQueryDataTable("select IsActive from PlanHistory where IsActive=0 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_active_status.Rows.Count == 0)
                            {
                                var empid = Session["EmployeeId"].ToString();
                                DataTable dtBussCnt = Comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
                                int empcount = 0;
                                DataTable dtBussList = Comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
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
                                    //Label_Maximum_Limit.Text = "Please contact admin to schedule timesheet. You have reached the maximum limit.";
                                    //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                    //btnprv.Visible = false;
                                    //DisableAllControlsInPage(Page, false);
                                    //return;
                                }
                            }
                            if (dt_check_active_status.Rows.Count > 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact admin toschedule timesheet. Your plan has been cancelled.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnprv.Visible = false;
                                DisableAllControlsInPage(Page, false);
                                return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to schedule timesheet. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnprv.Visible = false;
                            DisableAllControlsInPage(Page, false);
                            return;
                        }
                    }
                    if (check_old_reseller.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnprv.Visible = false;
                        DisableAllControlsInPage(Page, false);
                        return;
                    }
                }
            }
            #endregion

            //var empid = Session["EmployeeId"].ToString();
            //kumar commented as business name was showing wrong if business name is edited
            //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");
            Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " UserTypeId='"+ Session["UserTypeid"].ToString()+"'  AND EmployeeID=" + Session["EmployeeId"].ToString() + " and  BusinessID is not null", "BusinessID");

            //if (Request.QueryString["EmpidfromBusOwner"] != null && Request.QueryString["BidfromBusOwner"] != null)
            //{
            //    GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString());
            txtFromDate.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
            //txtmonthTo.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");

            //    GetGridFill();
            //}

            if (Session["UserTypeid"].ToString() != "1")
            {
                ddl_Business.Items.Clear();
                //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName",
                //    "UserTypeId not in (1,4,2,5,12) and EmployeeID in (select EmployeeID from MappingTable where USERID = '" + Session["UserId"].ToString() + "' and UserTypeId not in (1, 4, 2, 5, 12))", "BusinessID");

                Comm.FillDropDownList_NoPleaseSelect(ddl_Business, "AssginBussiness", "distinct BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", "BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                ddl_Business.SelectedValue = Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                GetAllDetails_Employee_List(ddl_Business.SelectedValue);
            }

            if (Request.QueryString["UserRole"] == null)
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx";
            }

            if (Request.QueryString["UserRole"] == "CustomRole")
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=CustomRole" + "&MangerId=" + Request.QueryString["MangerId"].ToString();
            }

            if (Request.QueryString["UserRole"] == "Administrator")
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Administrator";
            }

            if (Request.QueryString["UserRole"] == "Manager")
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Manager" + "&MangerId=" + Request.QueryString["MangerId"].ToString();
            }

            if (Request.QueryString["UserRole"] == "Employee")
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Employee";
            }

            if (Request.QueryString["UserRole"] == "Accountant")
            {
                HyperLink1.NavigateUrl = "~/Dashboard_Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Accountant";
            }
        }
    }

    protected void ddl_Business_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_Business.SelectedIndex != 0)
        {
            var bussid = ddl_Business.SelectedValue;
            var empid = Session["EmployeeId"].ToString();
            //commented by kumar
            //Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId!=1   and Assb.UserTypeId=4 and ae.EmployeeID<>" + empid + "", "AE.EmployeeID ");
            //added by kumar as all user roles will be shown here. Vishnu sir said.
            //Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId!=1 and ae.EmployeeID<>" + empid + " and AE.ISActive='YES'", "AE.EmployeeID ");
            //added by kumar as all admininstrator and accountant will not have his timesheet entered.
            Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId not in (1,5,12) and ae.EmployeeID<>" + empid + " and AE.ISActive='YES'", "AE.EmployeeID ");

            DataTable MotherOrgiD = Comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + bussid + "", "");
            string MotherOrgID = MotherOrgiD.Rows[0]["MotherOrgId"].ToString();
        }
    }

    public void GetAllDetails(string empid, string Bid)
    {
        DataTable dt = new DataTable();
        //Comm.FillDropDownList(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        Comm.FillDropDownList_NoPleaseSelect(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        ddlemp.SelectedValue = empid;
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " )", "");
        lblCurrentRate.Text = "$" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();

        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
    }

    public void GetAllDetails(string Bid)
    {
        if (Session["UserTypeid"].ToString() == "1")
        {
            if (Request.QueryString["MangerId"] != null)
            {
                DataTable dtMangerEmpList = new DataTable();
                dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "R");

                ddlemp.Items.Clear();
                ddlemp.Items.Add("Please Select");
                ddlemp.SelectedItem.Value = "";
                if (dtMangerEmpList.Rows.Count > 0)
                {
                    ddlemp.DataSource = dtMangerEmpList;
                    ddlemp.DataValueField = "EmployeeID";
                    ddlemp.DataTextField = "EmpName";
                    ddlemp.DataBind();
                    ddlemp.SelectedIndex = 0;
                }
            }
            else
            {
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "");
            }

        }
        else if (Session["UserTypeid"].ToString() == "2")
        {
            if (Request.QueryString["MangerId"] != null)
            {

                // Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BidfromBusOwner"].ToString() + "'", "EmpId");

                DataTable dtMangerEmpList = new DataTable();
                dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "R");

                ddlemp.Items.Clear();
                ddlemp.Items.Add("Please Select");
                ddlemp.SelectedItem.Value = "";
                if (dtMangerEmpList.Rows.Count > 0)
                {
                    ddlemp.DataSource = dtMangerEmpList;
                    ddlemp.DataValueField = "EmployeeID";
                    ddlemp.DataTextField = "EmpName";
                    ddlemp.DataBind();
                    ddlemp.SelectedIndex = 0;
                }
            }
            else
            {
                DataTable dtMapping = new DataTable();
                int ChkOwner = 0;
                dtMapping = Comm.GetAllDataFromTable("MappingTable", "*", "USERID=" + Session["UserId"].ToString() + "", "BusinessID");

                foreach (DataRow row in dtMapping.Rows)
                {
                    if (row["UserTypeId"].ToString() == "5")
                    {
                        ChkOwner = 1;
                    }
                }

                if (ChkOwner == 1)
                {
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
                }
                else
                {
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
                }

            }
            //Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
        }
        else
        {
            //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + " and ad.UserType<>1", "");
            if (Session["UserTypeid"].ToString() == "4")
            {
                //if (Request.QueryString["MangerId"] != null)
                //{
                //    Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BidfromBusOwner"].ToString() + "'", "EmpId");
                //}
                //else
                //{
                //Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
                //}
                if (Request.QueryString["MangerId"] != null)
                {
                    DataTable dtMangerEmpList = new DataTable();
                    dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "R");

                    ddlemp.Items.Clear();
                    ddlemp.Items.Add("Please Select");
                    ddlemp.SelectedItem.Value = "";
                    if (dtMangerEmpList.Rows.Count > 0)
                    {
                        ddlemp.DataSource = dtMangerEmpList;
                        ddlemp.DataValueField = "EmployeeID";
                        ddlemp.DataTextField = "EmpName";
                        ddlemp.DataBind();
                        ddlemp.SelectedIndex = 0;
                    }
                }
                else
                {
                    DataTable dtMangerEmpList = new DataTable();
                    dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BidfromBusOwner"].ToString() + " AND AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");

                    ddlemp.Items.Clear();
                    ddlemp.Items.Add("Please Select");
                    ddlemp.SelectedItem.Value = "";
                    if (dtMangerEmpList.Rows.Count > 0)
                    {
                        ddlemp.DataSource = dtMangerEmpList;
                        ddlemp.DataValueField = "EmployeeID";
                        ddlemp.DataTextField = "EmpName";
                        ddlemp.DataBind();
                        ddlemp.SelectedIndex = 0;
                    }
                }
            }
            else if (Session["UserTypeid"].ToString() == "5")
            {
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 ", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                //DataTable dtMangerEmpList = new DataTable();
                //dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Session["EmployeeId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()));

                //ddlemp.Items.Clear();
                //ddlemp.Items.Add("Please Select");
                //ddlemp.SelectedItem.Value = "";
                //if (dtMangerEmpList.Rows.Count > 0)
                //{
                //    ddlemp.DataSource = dtMangerEmpList;
                //    ddlemp.DataValueField = "EmployeeID";
                //    ddlemp.DataTextField = "EmpName";
                //    ddlemp.DataBind();
                //    ddlemp.SelectedIndex = 0;
                //}
            }
        }
        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
    }

    public void GetGridFill()
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select employee first', 'error');", true);
            return;
        }
        if (txtFromDate.Text != "")
        {


            int Bid = int.Parse(ddl_Business.SelectedValue);
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtFromDate.Text).Year;
            int m = DateTime.Parse(txtFromDate.Text).Month;
            var d = DateTime.Parse(txtFromDate.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            dtchild = TSC.GetTimeSheetEntryChildGv(y, m, weekNum, Bid, empid);
            if (dtchild.Rows.Count > 0)
            {
                //GVChildlist.Enabled = true;
                //lblCurrentRate.Visible = true;
                //lblCurrentErrorSms.Visible = false;
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();
            }
            //firoz 06
            //if (y < 2021)
            //{
            //    GVChildlist.Enabled = false;
            //    lblCurrentRate.Visible = false;
            //    lblCurrentErrorSms.Visible = true;
            //    lblCurrentErrorSms.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
            //}
            //end  


        }
    }
    protected void btnAddBreak_Click(object sender, EventArgs e)
    {
        if (txtBreakStartTime.Text == "" || txtBreakEndTime.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Break Time or End Time Cannot be Empty', 'error');", true);
            return;
        }

        if (DateTime.Parse(txtBreakEndTime.Text) < DateTime.Parse(txtBreakStartTime.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
            return;
        }
        if (ViewState["txtRegularStartTime"].ToString() == string.Empty || ViewState["txtRegularEndTime"].ToString() == string.Empty)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
            return;
        }
        DateTime tstart = DateTime.Parse(ViewState["txtRegularStartTime"].ToString());
        DateTime tend = DateTime.Parse(ViewState["txtRegularEndTime"].ToString());

        DateTime bstart = DateTime.Parse(txtBreakStartTime.Text);
        DateTime bend = DateTime.Parse(txtBreakEndTime.Text);

        if (bstart < tstart)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Break Start time should Between Woking Hr', 'error');", true);
            return;
        }
        if (bend > tend)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Break End time should Between Woking Hr', 'error');", true);
            return;
        }
        Abe.EmpId = int.Parse(ddlemp.SelectedValue);
        Abe.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
        Abe.Month = DateTime.Parse(txtFromDate.Text).Month;
        Abe.Year = DateTime.Parse(txtFromDate.Text).Year;
        string a = ViewState["datebreak"].ToString();
        string[] weekno = ViewState["weekno"].ToString().Split(',');

        Abe.WeekNo = int.Parse(weekno[0]);
        Abe.TimeSheetDate = DateTime.Parse(a);

        Abe.Day = DateTime.Parse(a).DayOfWeek.ToString();
        Abe.BreakStart = txtBreakStartTime.Text;
        Abe.BreakEnd = txtBreakEndTime.Text;
        TimeSpan TotalBreakHr = DateTime.Parse(txtBreakEndTime.Text) - DateTime.Parse(txtBreakStartTime.Text);
        Abe.BreakTotal = TotalBreakHr.ToString();
        Abc.InsertAddBreak(Abe);
        DataTable dt;
        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(a).ToString("yyyy-MM-dd") + "'", "BreakID");
        ViewState["dt"] = dt;
        GvPopUpEmpList.DataSource = ViewState["dt"];
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

        txtBreakStartTime.Text = "";
        txtBreakEndTime.Text = "";
    }

    protected void lnkdelete_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField brkid = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("brkid");
        Comm.ExecuteQuery("delete from BreakTime where BreakID=" + brkid.Value + "");
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time deleted', 'success');", true);
    }

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {

    }

    protected void lnkAddEmp_Click(object sender, EventArgs e)
    {

    }

    protected void ddlemp_SelectedIndexChanged(object sender, EventArgs e)
    {
        divPrj.Visible = false;
        ddlProjectName.Items.Clear();
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select employee', 'error');", true);
            return;
        }
        Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT", "PROJECTID", "dbo.FN_DESC('Project',PROJECTID)Pname", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "PROJECTID");
        
        if (ddlProjectName.Items.Count > 1)
        {
            ddlProjectName.Items.Add(new ListItem() { Value = "0", Text = "Regular Hours" });
            divPrj.Visible = true;
        }

        txtFromDate.Text = "";
        txtToDate.Text = "";
    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        if(ddl_Business.SelectedValue=="")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Business', 'warning');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please selct employee and please enter date range ', 'warning');", true);
            return;
        }

        if (ddlemp.SelectedValue == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Employee', 'warning');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select User', 'warning');", true);
            return;
        }

        if (txtFromDate.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Start Date and Time', 'warning');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Enter timings properly or Start time is empty', 'warning');", true);
            return;
        }
        if (txtToDate.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select End Date and Time', 'warning');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Enter timings properly or End time is empty', 'warning');", true);
            return;
        }

        else if (txtFromDate.Text == "" && txtToDate.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date and Time', 'warning');", true);
            return;
        }
        else if (ddlProjectName.SelectedValue == "" && divPrj.Visible)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Project', 'warning');", true);
            return;
        }
        else if (ddl_Business.SelectedValue != "" && ddlemp.SelectedValue=="")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Employee', 'warning');", true);
            return;
        }

        var dateAndTime = DateTime.Now;
        var todaydate = dateAndTime.Date;
        if (DateTime.Parse(txtFromDate.Text).Date == DateTime.Parse(txtToDate.Text).Date && DateTime.Parse(txtFromDate.Text).Date == todaydate && DateTime.Parse(txtToDate.Text).Date == todaydate)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Current Date Cannot be Scheduled', 'warning');", true);
            return;
        }

        DataTable DataTable_chk_hiredate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "Rate,RateType", "RateId=(select MIN(RateId)from RateDetails " +
                "where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + DateTime.Parse(txtFromDate.Text) + "')", "");
        if (DataTable_chk_hiredate.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select From Date On or After Hire Date', 'warning');", true);
            return;
        }

        DataTable DataTable_chk_enddate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "EndDate,Is_Having_EndDate", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + ddlemp.SelectedValue + ")", "");
        if (DataTable_chk_enddate.Rows.Count > 0 && DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"].ToString() == "True")
        {
            if (DateTime.Parse(txtToDate.Text) > DateTime.Parse(DataTable_chk_enddate.Rows[0]["EndDate"].ToString()))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Time Sheet can not be Scheduled after End Date', 'warning');", true);
                return;
            }
        }

        if (ddlProjectName.Items.Count > 1 && ddlProjectName.SelectedValue != "")
        {
            DataTable DataTable_chk_project_enddate = Comm.GetAllDataFromTable("AddProject with (Nolock)", "EndDate", "ProjectId=" + ddlProjectName.SelectedValue + " " +
                "and BusinessID=" + ddl_Business.SelectedValue + "", "");
            if (DataTable_chk_project_enddate.Rows.Count > 0 && DataTable_chk_project_enddate.Rows[0]["EndDate"].ToString() != "")
            {
                if (DateTime.Parse(txtToDate.Text) > DateTime.Parse(DataTable_chk_project_enddate.Rows[0]["EndDate"].ToString()))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Project has Ended so Time Sheet can not be Scheduled', 'warning');", true);
                    return;
                }
            }
        }

        try
        {
            DateTime t1 = DateTime.Parse(txtFromDate.Text);
            DateTime t2 = DateTime.Parse(txtToDate.Text);
            var lsttime = DateTime.Parse(txtToDate.Text).ToString("HH:mm");

            string query = @"a.EmpId = '" + ddlemp.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "' and a.ProjectId <> '" + ddlProjectName.SelectedValue + "'  and a.TimeSheetDate between '" + t1.ToString("dd-MMM-yyyy") + "' and '" + t2.ToString("dd-MMM-yyyy") + "'";
            DataTable dthr = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");

            if (t1>t2)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please fill the correct date', 'warning');", true);
            }
           
            else if (t1.Hour >= t2.Hour)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please fill the correct time', 'warning');", true);
            }
            else if (dthr.Rows.Count > 0)
            {

                int rwcnt = 0;
                while (t1 <= t2)
                {
                    try
                    {
                        DataTable dtflt = dthr.Select("DT='" + t1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                        TimeSpan tt1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                        TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                        TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                        for (int i = 1; i < dtflt.Rows.Count; i++)
                        {
                            tt1 = tt1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                            tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                            brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                        }
                        DateTime dtlast1 = DateTime.Parse(t1.ToString("MM-dd-yyyy " + lsttime));
                        TimeSpan thr1 = dtlast1 - t1;
                        tot = tot + thr1;
                        if (tot.TotalHours >= 24)
                        {
                            rwcnt = 1;
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Time is greater then 24 hours on " + t1.ToString("dd-MMM-yyyy")+"', 'warning');", true);
                            return;

                        }
                        

                    }
                    catch (Exception ex)
                    {
                       
                        //throw;
                    }
                    DateTime dtlast = DateTime.Parse(t1.ToString("MM-dd-yyyy " + lsttime));
                    TimeSpan thr = dtlast - t1;
                   
                    if (thr.TotalHours >= 24)
                    {
                        
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Time is greater then 24 hours on " + t1.ToString("dd-MMM-yyyy") + "', 'warning');", true);
                        return;

                    }


                    t1 = t1.AddDays(1);  
                }
                if (rwcnt == 0)
                {
                    SaveList();
                }
            }
            else {
               
                SaveList();
            }            
            //GetGridFill();
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('info!', 'Select all value', 'info');", true);
        }
    }
    public void SaveList()
    {
        DateTime t1 = DateTime.Parse(txtFromDate.Text);
        DateTime t2 = DateTime.Parse(txtToDate.Text);
        var lsttime = DateTime.Parse(txtToDate.Text).ToString("HH:mm");
        string Msg = "";
        int rwexe = 0;
        while (t1 <= t2)
        {
            DateTime t3 = DateTime.Now;

            if (t1.Date == t3.Date)
            {
                t1 = t1.AddDays(1);
                continue;
            }

            DateTime dtlast = DateTime.Parse(t1.ToString("MM-dd-yyyy " + lsttime));
            TimeSpan thr = dtlast - t1;

            int rw = SaveData(t1, dtlast);
            if (rw == 0)
            {
                rwexe = rwexe + 1;
                Msg = Msg + "<br/>Data already exist on " + t1.ToString("dd-MMM-yyy") + " ";
            }
            else if (rw == 2)
            {
                rwexe = rwexe + 1;
                //Msg = Msg + "<br/>Data updated on " + t1.ToString("dd-MMM-yyy") + " ";
                Msg = Msg + "<br/>Data already exist " + " "+ " ";
                //break;
            }
            t1 = t1.AddDays(1);
        }

        if (rwexe == 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet is added successfully.', 'success');", true);

            var dateAndTime = DateTime.Now;
            var todaydate = dateAndTime.Date;
            if (DateTime.Parse(txtFromDate.Text).Date == todaydate || DateTime.Parse(txtToDate.Text).Date == todaydate)
            {
                resetallfields();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet is added successfully except for today date.', 'success');", true);
            }
            else
            {
                resetallfields();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet is added successfully.', 'success');", true);
            } 
        }
        else
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('info!', '" + Msg + ". Timesheet is scheduled for the rest of the days.', 'info');", true);

            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', '" + Msg + ". Time sheet updated successfully.', 'success');", true);

            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet already exists. Timesheet is edited successfully.', 'success');", true);

            var dateAndTime = DateTime.Now;
            var todaydate = dateAndTime.Date;
            if (DateTime.Parse(txtFromDate.Text).Date == todaydate || DateTime.Parse(txtToDate.Text).Date == todaydate)
            {
                resetallfields();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet already exists. Timesheet is edited successfully except for today date.', 'success');", true);
            }
            else
            {
                resetallfields();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet already exists. Timesheet is edited successfully.', 'success');", true);
            }
        }
    }

    protected void resetallfields()
    {
        divPrj.Visible = false;
        ddlProjectName.Items.Clear();
        ddl_Business.SelectedIndex = 0;
        ddlemp.Items.Clear();
        ddlemp.Items.Add("Please Select");
        ddlemp.SelectedItem.Value = "";
        txtFromDate.Text = "";
        txtToDate.Text = "";
    }

    //firoz 15-22
    public void GetAllDetails(string empid, string Bid, DateTime Calender_Selected_Date)
    {
        //To reflect the day of date choosen from calendar control
        string get_month_from_date_control = txtFromDate.Text;
        DateTime get_date_from_date_control;
        if (DateTime.TryParse(get_month_from_date_control, out get_date_from_date_control))
        {
            //lblWeek.Text = get_date_from_date_control.DayOfWeek.ToString();
        }
        // lblWeek.Text = DateTime.Parse(txtFromDate.Text).DayOfWeek.ToString();

        DataTable dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
            //GVChildlist.Enabled = true;
        }
        if (dt.Rows.Count == 0)
        {
            //GVChildlist.Enabled = false;
            lblCurrentRate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
        }
    }
    //end
    protected void txtFromDate_TextChanged(object sender, EventArgs e)
    {
        //firoz 15-22
        //GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtFromDate.Text));
        //end
        GetGridFill();
    }

    protected void btnnext_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtFromDate.Text);
        DateTime nxtdate = CurrentDate.AddMonths(1);
        txtFromDate.Text = nxtdate.ToString("yyyy-MM-dd");
        txtFromDate_TextChanged(sender, e);
    }

    protected void lnkApproved_Click(object sender, EventArgs e)
    {

        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
            return;
        }
        int a = 0;
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            if (chkApproved.Checked == true)
            {

                HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
                Label lblDate = (Label)row.FindControl("lblDate");
                DateTime d = DateTime.Parse(lblDate.Text);

                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
            }
        }
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        }
    }

    protected void lnkReject_Click(object sender, EventArgs e)
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
            return;
        }
        int a = 0;
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            if (chkApproved.Checked == true)
            {
                HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
                Label lblDate = (Label)row.FindControl("lblDate");
                DateTime d = DateTime.Parse(lblDate.Text);

                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
            }
        }
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        }
    }

    string role_check_for_add_and_edit_time_sheet_message_throw;
    public int  SaveData(DateTime Date, DateTime dttlast)
    {
        TextBox txtRegular = new TextBox();
        txtRegular.Text = "";
        TextBox txtOver15x = new TextBox(); txtOver15x.Text = "";
        TextBox txtOver2x = new TextBox(); txtOver2x.Text = "";
        TextBox txtholiday = new TextBox(); txtholiday.Text = "";
        Label lblTotal = new Label(); lblTotal.Text = "";
        Label lblDate = new Label(); lblDate.Text = Date.ToString("yyyy-MM-dd");
        Label lblDay = new Label(); lblDay.Text = Date.ToString("dddd");
        CultureInfo cul = CultureInfo.CurrentCulture;
        int weekNum = cul.Calendar.GetWeekOfYear(Date, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        HiddenField hfdweekno = new HiddenField(); hfdweekno.Value = weekNum.ToString();
        TextBox txtRegularStartTime = new TextBox(); txtRegularStartTime.Text = Date.ToString("yyyy-MM-dd HH:mm");
        TextBox txtRegularEndTime = new TextBox(); txtRegularEndTime.Text = dttlast.ToString("yyyy-MM-dd HH:mm");
        DropDownList ddlWorked = new DropDownList(); ddlWorked.Text = "Yes";
        TextBox txtBreakStartTime = new TextBox(); txtBreakStartTime.Text = "";
        TextBox txtBreakEndTime = new TextBox(); txtBreakEndTime.Text = "";

        bool check = false;
        if (divPrj.Visible)
        {
            if (ddlProjectName.SelectedValue == "0")
            {
                check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                "and empid='" + ddlemp.SelectedValue + "' and bussId='" + ddl_Business.SelectedValue + "' and (ProjectId = '" + ddlProjectName.SelectedValue + "' or ProjectId IS NULL) ");
            }
            if (ddlProjectName.SelectedValue != "0")
            {
                check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                "and empid='" + ddlemp.SelectedValue + "' and bussId='" + ddl_Business.SelectedValue + "' and ProjectId = '" + ddlProjectName.SelectedValue + "' ");
            }
        }
        if (!divPrj.Visible)
        {
            check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                "and empid='" + ddlemp.SelectedValue + "' and bussId='" + ddl_Business.SelectedValue + "' and ProjectId = '" + ddlProjectName.SelectedValue + "' ");
        }

        TimeSpan thr = dttlast - Date;

        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(ddl_Business.SelectedValue);
        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
        Tsee.WeekNo = weekNum;
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        Tsee.Day = lblDay.Text;
        Tsee.Regular = thr.ToString();
        Tsee.Over1_5x = "0";
        Tsee.Over2x = "0";
        Tsee.Holiday = "0";
        Tsee.Total = thr.ToString();
        Tsee.ReqularStart = DateTime.Parse(txtRegularStartTime.Text);
        Tsee.Regularend = DateTime.Parse(txtRegularEndTime.Text);
        Tsee.BreakStart = txtBreakStartTime.Text.ToString();
        Tsee.BreakEnd = txtBreakEndTime.Text.ToString();
        Tsee.Status = "1";

        Tsee.Worked = "Yes";
        if (ddlProjectName.SelectedValue != "")
        {
            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
        }
        Tsee.BreakTotal = "00:00:00";

        //TSC.Sp_InsertTimeSheetEntry(Tsee);

        if (check == false)
        {
            TSC.Sp_InsertTimeSheetEntry(Tsee);

            return 1;
        }
        else
        {
            int a = Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + Tsee.TimeSheetDate + "' " +
                "and (ProjectId=" + Tsee.ProjectId + " or ProjectId IS NULL) and BussID='" + Tsee.BussID + "' and Status in (1,3,4)");
            if (a > 0)
            {
                TSC.Sp_InsertTimeSheetEntry(Tsee);
                return 2;
            }
        }
        return 0;
    }

    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string user_type_id = Session["UserTypeid"].ToString();
        if (user_type_id == "1")
        {
            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
            GridView GVChildlist = (GridView)sender;


            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
            if (e.CommandName == "Save")
            {


                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];
                TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    return;
                }

                string checkstartdate;
                checkstartdate = txtRegularStartTime.Text;
                if (checkstartdate == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }
                if (txtRegularStartTime.Text == ",")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }

                if (txtRegularEndTime.Text == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                if (check == true)
                {

                    Over15x = txtOver15x.Text.Split(',');
                    Over2x = txtOver2x.Text.Split(',');
                    holiday = txtholiday.Text.Split(',');
                    RegularStartTime = txtRegularStartTime.Text.Split(',');
                    RegularEndTime = txtRegularEndTime.Text.Split(',');

                    // BreakStartTime = txtBreakStartTime.Text.Split(',');
                    //BreakEndTime = txtBreakEndTime.Text.Split(',');

                    //Regular[0] = Regular[1];
                    //Over15x[0] = Over15x[1];
                    //Over2x[0] = Over2x[1];
                    // holiday[0] = holiday[1];
                    //RegularStartTime[0] = RegularStartTime[1];
                    //RegularEndTime[0] = RegularEndTime[1];

                    //BreakStartTime[0] = BreakStartTime[1];
                    //BreakEndTime[0] = BreakEndTime[1];
                }
                else
                {
                    //Regular[0] = txtRegular.Text.Substring(1);
                    // Over15x[0] = txtOver15x.Text.Substring(1);
                    // Over2x[0] = txtOver2x.Text.Substring(1);
                    // holiday[0] = txtholiday.Text.Substring(1);

                    RegularStartTime[0] = txtRegularStartTime.Text;
                    RegularEndTime[0] = txtRegularEndTime.Text;

                    //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                    //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                }



                string[] weekno = hfdweekno.Value.Split(',');
                if (Regular[0] == "")
                {
                    Regular[0] = "0";
                }
                if (Over15x[0] == "")
                {
                    Over15x[0] = "0";
                }
                if (Over2x[0] == "")
                {
                    Over2x[0] = "0";
                }
                if (holiday[0] == "")
                {
                    holiday[0] = "0";
                }


                if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                    return;
                }
                TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                if (ToltalRegulahr.TotalHours >= 24)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                    return;
                }

                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                if (ToltalRegulahr < TotalBreakHr)
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                    return;
                }
                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                Tsee.WeekNo = int.Parse(weekno[0]);
                Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                Tsee.Day = lblDay.Text;
                Tsee.Regular = ToltalRegulahr.ToString();
                Tsee.Over1_5x = Over15x[0];
                Tsee.Over2x = Over2x[0];
                Tsee.Holiday = holiday[0];
                Tsee.Total = TotalHr.ToString();
                Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                Tsee.BreakStart = BreakStartTime[0].ToString();
                Tsee.BreakEnd = BreakEndTime[0].ToString();
                Tsee.Status = "1";
                if (ddlWorked.SelectedValue == "Yes")
                {
                    Tsee.Worked = "Yes";
                }
                else
                {
                    Tsee.Worked = "No";
                }
                Tsee.BreakTotal = TotalBreakHr.ToString();
                TSC.Sp_InsertTimeSheetEntry(Tsee);

                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                GetGridFill();


            }
            if (e.CommandName == "Submit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                    return;
                }
                string checkstartdate = txtRegularStartTime.Text;
                if (checkstartdate == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }
                if (txtRegularStartTime.Text == ",")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }

                if (txtRegularEndTime.Text == "")
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                if (CheckSaved == true)
                {
                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                }


                //Response.Redirect(Request.RawUrl);
            }

            if (e.CommandName == "AddBreak")
            {
                DataTable dt;
                GvPopUpEmpList.DataSource = null;
                GvPopUpEmpList.DataBind();
                txtBreakEndTime.Text = string.Empty;
                txtBreakStartTime.Text = string.Empty;
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                    return;
                }
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                    return;
                }
                //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                ViewState["datebreak"] = lblDate.Text;
                ViewState["weekno"] = hfdweekno.Value;

                string regMINst = lblDate.Text + " 00:00";
                string regMAXst = lblDate.Text + " 23:59";
                txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                string regMinEnd = lblDate.Text + " 00:00";
                string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                // hfddatebreak.Value = lblDate.Text;
                // hfdweeknopop.Value = hfdweekno.Value;
                //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                ViewState["dt"] = dt;
                GvPopUpEmpList.DataSource = ViewState["dt"];
                GvPopUpEmpList.DataBind();
                up2.Update();
                //GetGridFill();

                ModalPopupEmpList.Show();
                return;
            }

            return;
        }

        //This will check the login user has been assigned in which business in what roles with the employee id of same login user
        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' ", "UserTypeId");

        //To check the old user who are registered before the role development
        DataTable Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' ", "");
        if (Dt_Check_If_User_Role_Permission.Rows.Count == 0)
        {
            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
            GridView GVChildlist = (GridView)sender;

            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
            if (e.CommandName == "Save")
            {


                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];
                TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    return;
                }

                string checkstartdate = txtRegularStartTime.Text;
                if (checkstartdate == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }
                if (txtRegularStartTime.Text == ",")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }

                if (txtRegularEndTime.Text == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                if (check == true)
                {

                    Over15x = txtOver15x.Text.Split(',');
                    Over2x = txtOver2x.Text.Split(',');
                    holiday = txtholiday.Text.Split(',');
                    RegularStartTime = txtRegularStartTime.Text.Split(',');
                    RegularEndTime = txtRegularEndTime.Text.Split(',');

                    // BreakStartTime = txtBreakStartTime.Text.Split(',');
                    //BreakEndTime = txtBreakEndTime.Text.Split(',');

                    //Regular[0] = Regular[1];
                    //Over15x[0] = Over15x[1];
                    //Over2x[0] = Over2x[1];
                    // holiday[0] = holiday[1];
                    //RegularStartTime[0] = RegularStartTime[1];
                    //RegularEndTime[0] = RegularEndTime[1];

                    //BreakStartTime[0] = BreakStartTime[1];
                    //BreakEndTime[0] = BreakEndTime[1];
                }
                else
                {
                    //Regular[0] = txtRegular.Text.Substring(1);
                    // Over15x[0] = txtOver15x.Text.Substring(1);
                    // Over2x[0] = txtOver2x.Text.Substring(1);
                    // holiday[0] = txtholiday.Text.Substring(1);

                    RegularStartTime[0] = txtRegularStartTime.Text;
                    RegularEndTime[0] = txtRegularEndTime.Text;

                    //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                    //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                }



                string[] weekno = hfdweekno.Value.Split(',');
                if (Regular[0] == "")
                {
                    Regular[0] = "0";
                }
                if (Over15x[0] == "")
                {
                    Over15x[0] = "0";
                }
                if (Over2x[0] == "")
                {
                    Over2x[0] = "0";
                }
                if (holiday[0] == "")
                {
                    holiday[0] = "0";
                }


                if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                    return;
                }
                TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                if (ToltalRegulahr.TotalHours >= 24)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                    return;
                }

                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                if (ToltalRegulahr < TotalBreakHr)
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                    return;
                }
                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                Tsee.WeekNo = int.Parse(weekno[0]);
                Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                Tsee.Day = lblDay.Text;
                Tsee.Regular = ToltalRegulahr.ToString();
                Tsee.Over1_5x = Over15x[0];
                Tsee.Over2x = Over2x[0];
                Tsee.Holiday = holiday[0];
                Tsee.Total = TotalHr.ToString();
                Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                Tsee.BreakStart = BreakStartTime[0].ToString();
                Tsee.BreakEnd = BreakEndTime[0].ToString();
                Tsee.Status = "1";
                if (ddlWorked.SelectedValue == "Yes")
                {
                    Tsee.Worked = "Yes";
                }
                else
                {
                    Tsee.Worked = "No";
                }
                Tsee.BreakTotal = TotalBreakHr.ToString();
                TSC.Sp_InsertTimeSheetEntry(Tsee);

                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                GetGridFill();


            }
            if (e.CommandName == "Submit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                    return;
                }
                string checkstartdate = txtRegularStartTime.Text;
                if (checkstartdate == "")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }
                if (txtRegularStartTime.Text == ",")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                    return;
                }

                if (txtRegularEndTime.Text == "")
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                if (CheckSaved == true)
                {
                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                }


                //Response.Redirect(Request.RawUrl);
            }

            if (e.CommandName == "AddBreak")
            {
                DataTable dt;
                GvPopUpEmpList.DataSource = null;
                GvPopUpEmpList.DataBind();
                txtBreakEndTime.Text = string.Empty;
                txtBreakStartTime.Text = string.Empty;
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                    return;
                }
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                    return;
                }
                //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                ViewState["datebreak"] = lblDate.Text;
                ViewState["weekno"] = hfdweekno.Value;

                string regMINst = lblDate.Text + " 00:00";
                string regMAXst = lblDate.Text + " 23:59";
                txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                string regMinEnd = lblDate.Text + " 00:00";
                string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                // hfddatebreak.Value = lblDate.Text;
                // hfdweeknopop.Value = hfdweekno.Value;
                //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                ViewState["dt"] = dt;
                GvPopUpEmpList.DataSource = ViewState["dt"];
                GvPopUpEmpList.DataBind();
                up2.Update();
                //GetGridFill();

                ModalPopupEmpList.Show();
            }
            return;
        }

        //For manager
        DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='2'", "");
        if (Dt_Check_Manager_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "7" || role_check_for_add_and_edit_time_sheet == "8")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

                    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
                    if (e.CommandName == "Save")
                    {


                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }

                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {

                            Over15x = txtOver15x.Text.Split(',');
                            Over2x = txtOver2x.Text.Split(',');
                            holiday = txtholiday.Text.Split(',');
                            RegularStartTime = txtRegularStartTime.Text.Split(',');
                            RegularEndTime = txtRegularEndTime.Text.Split(',');

                            // BreakStartTime = txtBreakStartTime.Text.Split(',');
                            //BreakEndTime = txtBreakEndTime.Text.Split(',');

                            //Regular[0] = Regular[1];
                            //Over15x[0] = Over15x[1];
                            //Over2x[0] = Over2x[1];
                            // holiday[0] = holiday[1];
                            //RegularStartTime[0] = RegularStartTime[1];
                            //RegularEndTime[0] = RegularEndTime[1];

                            //BreakStartTime[0] = BreakStartTime[1];
                            //BreakEndTime[0] = BreakEndTime[1];
                        }
                        else
                        {
                            //Regular[0] = txtRegular.Text.Substring(1);
                            // Over15x[0] = txtOver15x.Text.Substring(1);
                            // Over2x[0] = txtOver2x.Text.Substring(1);
                            // holiday[0] = txtholiday.Text.Substring(1);

                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;

                            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                        }



                        string[] weekno = hfdweekno.Value.Split(',');
                        if (Regular[0] == "")
                        {
                            Regular[0] = "0";
                        }
                        if (Over15x[0] == "")
                        {
                            Over15x[0] = "0";
                        }
                        if (Over2x[0] == "")
                        {
                            Over2x[0] = "0";
                        }
                        if (holiday[0] == "")
                        {
                            holiday[0] = "0";
                        }


                        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                            return;
                        }
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }

                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                        Tsee.Day = lblDay.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        Tsee.Status = "1";
                        if (ddlWorked.SelectedValue == "Yes")
                        {
                            Tsee.Worked = "Yes";
                        }
                        else
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }


                        //Response.Redirect(Request.RawUrl);
                    }

                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                        ViewState["datebreak"] = lblDate.Text;
                        ViewState["weekno"] = hfdweekno.Value;

                        string regMINst = lblDate.Text + " 00:00";
                        string regMAXst = lblDate.Text + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Text + " 00:00";
                        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "7" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "8")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For owner
        DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='5'", "");
        if (Dt_Check_Owner_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

                    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
                    if (e.CommandName == "Save")
                    {


                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }

                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {

                            Over15x = txtOver15x.Text.Split(',');
                            Over2x = txtOver2x.Text.Split(',');
                            holiday = txtholiday.Text.Split(',');
                            RegularStartTime = txtRegularStartTime.Text.Split(',');
                            RegularEndTime = txtRegularEndTime.Text.Split(',');

                            // BreakStartTime = txtBreakStartTime.Text.Split(',');
                            //BreakEndTime = txtBreakEndTime.Text.Split(',');

                            //Regular[0] = Regular[1];
                            //Over15x[0] = Over15x[1];
                            //Over2x[0] = Over2x[1];
                            // holiday[0] = holiday[1];
                            //RegularStartTime[0] = RegularStartTime[1];
                            //RegularEndTime[0] = RegularEndTime[1];

                            //BreakStartTime[0] = BreakStartTime[1];
                            //BreakEndTime[0] = BreakEndTime[1];
                        }
                        else
                        {
                            //Regular[0] = txtRegular.Text.Substring(1);
                            // Over15x[0] = txtOver15x.Text.Substring(1);
                            // Over2x[0] = txtOver2x.Text.Substring(1);
                            // holiday[0] = txtholiday.Text.Substring(1);

                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;

                            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                        }



                        string[] weekno = hfdweekno.Value.Split(',');
                        if (Regular[0] == "")
                        {
                            Regular[0] = "0";
                        }
                        if (Over15x[0] == "")
                        {
                            Over15x[0] = "0";
                        }
                        if (Over2x[0] == "")
                        {
                            Over2x[0] = "0";
                        }
                        if (holiday[0] == "")
                        {
                            holiday[0] = "0";
                        }


                        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                            return;
                        }
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }

                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                        Tsee.Day = lblDay.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        Tsee.Status = "1";
                        if (ddlWorked.SelectedValue == "Yes")
                        {
                            Tsee.Worked = "Yes";
                        }
                        else
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }


                        //Response.Redirect(Request.RawUrl);
                    }

                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                        ViewState["datebreak"] = lblDate.Text;
                        ViewState["weekno"] = hfdweekno.Value;

                        string regMINst = lblDate.Text + " 00:00";
                        string regMAXst = lblDate.Text + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Text + " 00:00";
                        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "18" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "19")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For accountant
        DataTable Dt_Check_AccountantRole_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='12'", "");
        if (Dt_Check_AccountantRole_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_AccountantRole_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "25" || role_check_for_add_and_edit_time_sheet == "26")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

                    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
                    if (e.CommandName == "Save")
                    {


                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }

                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {

                            Over15x = txtOver15x.Text.Split(',');
                            Over2x = txtOver2x.Text.Split(',');
                            holiday = txtholiday.Text.Split(',');
                            RegularStartTime = txtRegularStartTime.Text.Split(',');
                            RegularEndTime = txtRegularEndTime.Text.Split(',');

                            // BreakStartTime = txtBreakStartTime.Text.Split(',');
                            //BreakEndTime = txtBreakEndTime.Text.Split(',');

                            //Regular[0] = Regular[1];
                            //Over15x[0] = Over15x[1];
                            //Over2x[0] = Over2x[1];
                            // holiday[0] = holiday[1];
                            //RegularStartTime[0] = RegularStartTime[1];
                            //RegularEndTime[0] = RegularEndTime[1];

                            //BreakStartTime[0] = BreakStartTime[1];
                            //BreakEndTime[0] = BreakEndTime[1];
                        }
                        else
                        {
                            //Regular[0] = txtRegular.Text.Substring(1);
                            // Over15x[0] = txtOver15x.Text.Substring(1);
                            // Over2x[0] = txtOver2x.Text.Substring(1);
                            // holiday[0] = txtholiday.Text.Substring(1);

                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;

                            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                        }



                        string[] weekno = hfdweekno.Value.Split(',');
                        if (Regular[0] == "")
                        {
                            Regular[0] = "0";
                        }
                        if (Over15x[0] == "")
                        {
                            Over15x[0] = "0";
                        }
                        if (Over2x[0] == "")
                        {
                            Over2x[0] = "0";
                        }
                        if (holiday[0] == "")
                        {
                            holiday[0] = "0";
                        }


                        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                            return;
                        }
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }

                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                        Tsee.Day = lblDay.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        Tsee.Status = "1";
                        if (ddlWorked.SelectedValue == "Yes")
                        {
                            Tsee.Worked = "Yes";
                        }
                        else
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }


                        //Response.Redirect(Request.RawUrl);
                    }

                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                        ViewState["datebreak"] = lblDate.Text;
                        ViewState["weekno"] = hfdweekno.Value;

                        string regMINst = lblDate.Text + " 00:00";
                        string regMAXst = lblDate.Text + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Text + " 00:00";
                        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "25" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "26")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For employee
        DataTable Dt_Check_Employee_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='4'", "");
        if (Dt_Check_Employee_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Employee_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

                    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
                    if (e.CommandName == "Save")
                    {


                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {

                            Over15x = txtOver15x.Text.Split(',');
                            Over2x = txtOver2x.Text.Split(',');
                            holiday = txtholiday.Text.Split(',');
                            RegularStartTime = txtRegularStartTime.Text.Split(',');
                            RegularEndTime = txtRegularEndTime.Text.Split(',');

                            // BreakStartTime = txtBreakStartTime.Text.Split(',');
                            //BreakEndTime = txtBreakEndTime.Text.Split(',');

                            //Regular[0] = Regular[1];
                            //Over15x[0] = Over15x[1];
                            //Over2x[0] = Over2x[1];
                            // holiday[0] = holiday[1];
                            //RegularStartTime[0] = RegularStartTime[1];
                            //RegularEndTime[0] = RegularEndTime[1];

                            //BreakStartTime[0] = BreakStartTime[1];
                            //BreakEndTime[0] = BreakEndTime[1];
                        }
                        else
                        {
                            //Regular[0] = txtRegular.Text.Substring(1);
                            // Over15x[0] = txtOver15x.Text.Substring(1);
                            // Over2x[0] = txtOver2x.Text.Substring(1);
                            // holiday[0] = txtholiday.Text.Substring(1);

                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;

                            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                        }



                        string[] weekno = hfdweekno.Value.Split(',');
                        if (Regular[0] == "")
                        {
                            Regular[0] = "0";
                        }
                        if (Over15x[0] == "")
                        {
                            Over15x[0] = "0";
                        }
                        if (Over2x[0] == "")
                        {
                            Over2x[0] = "0";
                        }
                        if (holiday[0] == "")
                        {
                            holiday[0] = "0";
                        }


                        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                            return;
                        }
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }

                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                        Tsee.Day = lblDay.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        Tsee.Status = "1";
                        if (ddlWorked.SelectedValue == "Yes")
                        {
                            Tsee.Worked = "Yes";
                        }
                        else
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }


                        //Response.Redirect(Request.RawUrl);
                    }

                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                        ViewState["datebreak"] = lblDate.Text;
                        ViewState["weekno"] = hfdweekno.Value;

                        string regMINst = lblDate.Text + " 00:00";
                        string regMAXst = lblDate.Text + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Text + " 00:00";
                        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }


        //For the custom user type
        //For employee
        DataTable Dt_Check_Custome_USerType_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId not in ('4','1','2','5','12')", "");
        if (Dt_Check_Custome_USerType_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Custome_USerType_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2" ||
                    role_check_for_add_and_edit_time_sheet == "6" || role_check_for_add_and_edit_time_sheet == "8" ||
                    role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

                    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
                    if (e.CommandName == "Save")
                    {


                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
                        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
                        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
                        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {

                            Over15x = txtOver15x.Text.Split(',');
                            Over2x = txtOver2x.Text.Split(',');
                            holiday = txtholiday.Text.Split(',');
                            RegularStartTime = txtRegularStartTime.Text.Split(',');
                            RegularEndTime = txtRegularEndTime.Text.Split(',');

                            // BreakStartTime = txtBreakStartTime.Text.Split(',');
                            //BreakEndTime = txtBreakEndTime.Text.Split(',');

                            //Regular[0] = Regular[1];
                            //Over15x[0] = Over15x[1];
                            //Over2x[0] = Over2x[1];
                            // holiday[0] = holiday[1];
                            //RegularStartTime[0] = RegularStartTime[1];
                            //RegularEndTime[0] = RegularEndTime[1];

                            //BreakStartTime[0] = BreakStartTime[1];
                            //BreakEndTime[0] = BreakEndTime[1];
                        }
                        else
                        {
                            //Regular[0] = txtRegular.Text.Substring(1);
                            // Over15x[0] = txtOver15x.Text.Substring(1);
                            // Over2x[0] = txtOver2x.Text.Substring(1);
                            // holiday[0] = txtholiday.Text.Substring(1);

                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;

                            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
                            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
                        }



                        string[] weekno = hfdweekno.Value.Split(',');
                        if (Regular[0] == "")
                        {
                            Regular[0] = "0";
                        }
                        if (Over15x[0] == "")
                        {
                            Over15x[0] = "0";
                        }
                        if (Over2x[0] == "")
                        {
                            Over2x[0] = "0";
                        }
                        if (holiday[0] == "")
                        {
                            holiday[0] = "0";
                        }


                        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
                            return;
                        }
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }

                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                        Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
                        Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                        Tsee.Day = lblDay.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        Tsee.Status = "1";
                        if (ddlWorked.SelectedValue == "Yes")
                        {
                            Tsee.Worked = "Yes";
                        }
                        else
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                            return;
                        }
                        string checkstartdate = txtRegularStartTime.Text;
                        if (checkstartdate == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }
                        if (txtRegularStartTime.Text == ",")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }


                        //Response.Redirect(Request.RawUrl);
                    }

                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
                        ViewState["datebreak"] = lblDate.Text;
                        ViewState["weekno"] = hfdweekno.Value;

                        string regMINst = lblDate.Text + " 00:00";
                        string regMAXst = lblDate.Text + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Text + " 00:00";
                        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }


        //commented for new role base logic
        //string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //GridView GVChildlist = (GridView)sender;

        //// GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //if (e.CommandName == "Save")
        //{


        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];
        //    TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //    TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //    TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //    TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //    Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //        return;
        //    }

        //    if (txtRegularStartTime.Text == ",")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //        return;
        //    }

        //    if (txtRegularEndTime.Text == "")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //        return;
        //    }
        //    bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //    if (check == true)
        //    {

        //        Over15x = txtOver15x.Text.Split(',');
        //        Over2x = txtOver2x.Text.Split(',');
        //        holiday = txtholiday.Text.Split(',');
        //        RegularStartTime = txtRegularStartTime.Text.Split(',');
        //        RegularEndTime = txtRegularEndTime.Text.Split(',');

        //        // BreakStartTime = txtBreakStartTime.Text.Split(',');
        //        //BreakEndTime = txtBreakEndTime.Text.Split(',');

        //        //Regular[0] = Regular[1];
        //        //Over15x[0] = Over15x[1];
        //        //Over2x[0] = Over2x[1];
        //        // holiday[0] = holiday[1];
        //        //RegularStartTime[0] = RegularStartTime[1];
        //        //RegularEndTime[0] = RegularEndTime[1];

        //        //BreakStartTime[0] = BreakStartTime[1];
        //        //BreakEndTime[0] = BreakEndTime[1];
        //    }
        //    else
        //    {
        //        //Regular[0] = txtRegular.Text.Substring(1);
        //        // Over15x[0] = txtOver15x.Text.Substring(1);
        //        // Over2x[0] = txtOver2x.Text.Substring(1);
        //        // holiday[0] = txtholiday.Text.Substring(1);

        //        RegularStartTime[0] = txtRegularStartTime.Text;
        //        RegularEndTime[0] = txtRegularEndTime.Text;

        //        //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
        //        //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
        //    }



        //    string[] weekno = hfdweekno.Value.Split(',');
        //    if (Regular[0] == "")
        //    {
        //        Regular[0] = "0";
        //    }
        //    if (Over15x[0] == "")
        //    {
        //        Over15x[0] = "0";
        //    }
        //    if (Over2x[0] == "")
        //    {
        //        Over2x[0] = "0";
        //    }
        //    if (holiday[0] == "")
        //    {
        //        holiday[0] = "0";
        //    }


        //    if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //        return;
        //    }
        //    TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //    if (ToltalRegulahr.TotalHours >= 24)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //        return;
        //    }

        //    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //    if (ToltalRegulahr < TotalBreakHr)
        //    {
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //        return;
        //    }
        //    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //    Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
        //    Tsee.Month = DateTime.Parse(txtFromDate.Text).Month;
        //    Tsee.Year = DateTime.Parse(txtFromDate.Text).Year;
        //    Tsee.WeekNo = int.Parse(weekno[0]);
        //    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //    Tsee.Day = lblDay.Text;
        //    Tsee.Regular = ToltalRegulahr.ToString();
        //    Tsee.Over1_5x = Over15x[0];
        //    Tsee.Over2x = Over2x[0];
        //    Tsee.Holiday = holiday[0];
        //    Tsee.Total = TotalHr.ToString();
        //    Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //    Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //    Tsee.BreakStart = BreakStartTime[0].ToString();
        //    Tsee.BreakEnd = BreakEndTime[0].ToString();
        //    Tsee.Status = "1";
        //    if (chkworked.Checked == true)
        //    {
        //        Tsee.Worked = "Yes";
        //    }
        //    else
        //    {
        //        Tsee.Worked = "No";
        //    }
        //    Tsee.BreakTotal = TotalBreakHr.ToString();
        //    TSC.Sp_InsertTimeSheetEntry(Tsee);

        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //    GetGridFill();


        //}
        //if (e.CommandName == "Submit")
        //{
        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];

        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //        return;
        //    }
        //    if (txtRegularStartTime.Text == ",")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //        return;
        //    }

        //    if (txtRegularEndTime.Text == "")
        //    {
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //        return;
        //    }
        //    bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //    if (CheckSaved == true)
        //    {
        //        Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //    }


        //    //Response.Redirect(Request.RawUrl);
        //}

        //if (e.CommandName == "AddBreak")
        //{
        //    DataTable dt;
        //    GvPopUpEmpList.DataSource = null;
        //    GvPopUpEmpList.DataBind();
        //    txtBreakEndTime.Text = string.Empty;
        //    txtBreakStartTime.Text = string.Empty;
        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];

        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //    if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //        return;
        //    }
        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //        return;
        //    }
        //    //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //    lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //    ViewState["datebreak"] = lblDate.Text;
        //    ViewState["weekno"] = hfdweekno.Value;

        //    string regMINst = lblDate.Text + " 00:00";
        //    string regMAXst = lblDate.Text + " 23:59";
        //    txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //    txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //    string regMinEnd = lblDate.Text + " 00:00";
        //    string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

        //    txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //    txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //    // hfddatebreak.Value = lblDate.Text;
        //    // hfdweeknopop.Value = hfdweekno.Value;
        //    //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtFromDate.Text).Month + " and Year=" + DateTime.Parse(txtFromDate.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //    ViewState["dt"] = dt;
        //    GvPopUpEmpList.DataSource = ViewState["dt"];
        //    GvPopUpEmpList.DataBind();
        //    up2.Update();
        //    //GetGridFill();

        //    ModalPopupEmpList.Show();
        //}
    }


    protected void GVChildlist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdStatus = e.Row.FindControl("hfdStatus") as HiddenField;
            UpdatePanel up1 = e.Row.FindControl("up1") as UpdatePanel;
            Label lblStatus = e.Row.FindControl("lblStatus") as Label;
            Label lblDate = e.Row.FindControl("lblDate") as Label;
            HiddenField hfdWorked = e.Row.FindControl("hfdWorked") as HiddenField;
            DropDownList ddlWorked = e.Row.FindControl("ddlWorked") as DropDownList;
            CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;


            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-ddTHH:mm"));


            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                txtBreakEndTime.ReadOnly = true;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                lnkSave.Enabled = true;
                lnkAddBreak.Enabled = true;
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
            }
            else if (hfdStatus.Value == "4")
            {
                lblStatus.Text = "Absent";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                txtRegularStartTime.ReadOnly = true;
                //txtBreakEndTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                e.Row.BackColor = System.Drawing.Color.LightGray;
                lnkGvApp.Enabled = false;
                lnkGvReject.Enabled = false;
                ddlWorked.Enabled = false;
                chkApproved.Enabled = false;
                //chkworked.Enabled = false;
            }
            if (hfdWorked.Value.Trim() == "Yes")
            {
                ddlWorked.SelectedValue = "Yes";
            }
            //if(txtRegularEndTime.Text!=string.Empty && txtRegularStartTime.Text!=string.Empty)
            //{
            //    lnkSave.Enabled = false;
            //    lnkAddBreak.Enabled = false;
            //    txtRegularStartTime.ReadOnly = true;
            //    txtRegularStartTime.ReadOnly = true;
            //    //chkworked.Enabled = false;
            //}

            string regMINst = lblDate.Text + " 00:00";
            string regMAXst = lblDate.Text + " 23:59";
            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            string regMinEnd = lblDate.Text + " 00:00";
            string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));

            DataTable DataTable_chk_hiredate = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and effectivedate <= '" + DateTime.Parse(lblDate.Text) + "')", "");
            if (DataTable_chk_hiredate.Rows.Count > 0)
            {
                //GVChildlist.Enabled = true;
                e.Row.Enabled = true;
            }
            if (DataTable_chk_hiredate.Rows.Count == 0)
            {
                //GVChildlist.Enabled = false;
                e.Row.Enabled = false;
            }
        }
    }

    protected void GVChildlist_PreRender(object sender, EventArgs e)
    {
        if (GVChildlist.Rows.Count > 0)
        {
            GVChildlist.UseAccessibleHeader = true;
            GVChildlist.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }



    protected void lnkweekPre_Click(object sender, EventArgs e)
    {
        DateTime nxtdate;
        string lbldate = string.Empty;
        DateTime currdate;
        CultureInfo cul = CultureInfo.CurrentCulture;
        int currweekNum = 0, prvweekNum = 0;

        for (int i = 0; i < GVChildlist.Rows.Count; i++)
        {
            lbldate = ((Label)GVChildlist.Rows[0].FindControl("lblDate")).Text.Trim();
        }
        int currday = DateTime.Parse(lbldate).Day;
        if (currday != 1)
        {
            currdate = DateTime.Parse(txtFromDate.Text);
        }
        else
        {
            currdate = DateTime.Parse(lbldate);
        }
        if (currday == 1)
        {
            currweekNum = cul.Calendar.GetWeekOfYear(currdate, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            prvweekNum = cul.Calendar.GetWeekOfYear(currdate.AddDays(-currday), CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        if (currweekNum == prvweekNum && currweekNum != 0 && prvweekNum != 0)
        {
            nxtdate = currdate.AddDays(-1);
        }
        else if (prvweekNum == 52 || prvweekNum == 53)
        {
            nxtdate = currdate.AddDays(-1);
        }
        else
        {
            nxtdate = currdate.AddDays(-7);
        }




        txtFromDate.Text = nxtdate.ToString("yyyy-MM-dd");
        txtFromDate_TextChanged(sender, e);
    }

    protected void lnkweekNxt_Click(object sender, EventArgs e)
    {
        DateTime nxtdate;
        DateTime currdate = DateTime.Parse(txtFromDate.Text);
        string lbldate = string.Empty;
        int totmonthday = DateTime.DaysInMonth(currdate.Year, currdate.Month);
        int currday = currdate.Day;
        int gridlastday;
        DateTime gridlastdt;
        int diff = totmonthday - currday;
        for (int i = 0; i < GVChildlist.Rows.Count; i++)
        {
            lbldate = ((Label)GVChildlist.Rows[GVChildlist.Rows.Count - 1].FindControl("lblDate")).Text.Trim();
        }
        gridlastdt = DateTime.Parse(lbldate);
        gridlastday = gridlastdt.Day;
        if (diff < 7 && diff != 0)
        {

            if (gridlastday == 28 || gridlastday == 29 || gridlastday == 30 || gridlastday == 31)
            {
                nxtdate = gridlastdt.AddDays(1);
            }
            else
            {
                nxtdate = currdate.AddDays(diff);
            }
        }
        else
        {
            if (diff == 0)
            {
                if (gridlastday == 28 || gridlastday == 29 || gridlastday == 30 || gridlastday == 31)
                {
                    nxtdate = gridlastdt.AddDays(1);
                }
                else
                {
                    nxtdate = currdate.AddDays(1);
                }

            }
            else
            {
                nxtdate = currdate.AddDays(7);
            }

        }
        int currtday = nxtdate.Day;
        //if(currtday==28|| currtday==29|| currtday==30|| currtday==31)
        //{
        //    nxtdate = currdate.AddDays(1);
        //}
        txtFromDate.Text = nxtdate.ToString("yyyy-MM-dd");
        txtFromDate_TextChanged(sender, e);
    }

    protected void lnkGvApp_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);

        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        }
    }

    protected void lnkGvReject_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);

        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
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

    public void GetAllDetails_Employee_List(string Bid)
    {
        if (Request.QueryString["UserRole"] == "CustomRole")
        {
            string[] role;
            ddlemp.Items.Clear();
            ddlemp.Items.Add("Please Select");
            ddlemp.SelectedItem.Value = "";
            string str_Check_Role_Permission = string.Empty;
            DataTable dt_Custome_User_Role_Popup_Emp_List = CustomRole_Pop_up_Employee_List(int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()), "R", int.Parse(Request.QueryString["MangerId"].ToString()));
            DataTable dtROLE = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id=" + int.Parse(Request.QueryString["MangerId"].ToString()) + "", "");
            if (dtROLE.Rows[0]["Role_Permission_Id"].ToString().Length > 1)
            {
                str_Check_Role_Permission = dtROLE.Rows[0]["Role_Permission_Id"].ToString().Remove(dtROLE.Rows[0]["Role_Permission_Id"].ToString().Length - 1);
            }
            DataRow[] dr;
            DataTable dtr;
            if (dtROLE.Rows[0]["Role_Permission_Id"].ToString().Length > 1)
            {
                string role_check_employee_time_sheet_permission = string.Empty;
                if (dtROLE.Rows.Count > 0)
                {
                    //bool ch = false;
                    role = str_Check_Role_Permission.Split(',');
                    foreach (string role_check_for_add_edit_time_sheet in role)
                    //for(int i=0;i<role.Length;i++)
                    {
                        //if (role[i].ToString()=="1" || role[i].ToString() == "2")
                        if (role_check_for_add_edit_time_sheet == "1" || role_check_for_add_edit_time_sheet == "2")
                        {
                            ddlemp.Items.Clear();
                            ddlemp.DataSource = null;
                            ddlemp.DataBind();
                            ddlemp.Items.Add("Please Select");
                            ddlemp.SelectedItem.Value = "";
                            dr = null;
                            dtr = null;
                            dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES' and [EmployeeID]='" + Request.QueryString["MangerId"].ToString() + "'");
                            if (dr.Count() > 0)
                            {
                                dtr = dr.CopyToDataTable();
                                ddlemp.DataSource = dtr;
                                ddlemp.DataValueField = "EmployeeID";
                                ddlemp.DataTextField = "EmpName";
                                ddlemp.DataBind();
                                ddlemp.SelectedIndex = 0;
                                //break;
                            }

                            role_check_employee_time_sheet_permission = "32";
                        }
                        //else if (role[i].ToString() != "1" && role[i].ToString() != "2" && role[i].ToString() != "")
                        else
                        {
                            role_check_employee_time_sheet_permission = role_check_for_add_edit_time_sheet;
                            ddlemp.Items.Clear();
                            ddlemp.DataSource = null;
                            ddlemp.DataBind();
                            ddlemp.Items.Add("Please Select");
                            ddlemp.SelectedItem.Value = "";
                            dr = null;
                            dtr = null;
                            //ch = true;
                        }
                    }
                    //if(ch == true)
                    if (role_check_employee_time_sheet_permission != "32")
                    {
                        ddlemp.Items.Clear();
                        ddlemp.DataSource = null;
                        ddlemp.DataBind();
                        ddlemp.Items.Add("Please Select");
                        ddlemp.SelectedItem.Value = "";
                        dr = null;
                        dtr = null;
                        dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES'");
                        if (dr.Count() > 0)
                        {
                            dtr = dr.CopyToDataTable();
                            ddlemp.DataSource = dtr;
                            ddlemp.DataValueField = "EmployeeID";
                            ddlemp.DataTextField = "EmpName";
                            ddlemp.DataBind();
                            ddlemp.SelectedIndex = 0;
                        }
                    }
                }
            }
            else
            {
                ddlemp.Items.Clear();
                ddlemp.DataSource = null;
                ddlemp.DataBind();
                ddlemp.Items.Add("Please Select");
                ddlemp.SelectedItem.Value = "";
                dr = null;
                dtr = null;
                dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES'");
                if (dr.Count() > 0)
                {
                    dtr = dr.CopyToDataTable();
                    ddlemp.DataSource = dtr;
                    ddlemp.DataValueField = "EmployeeID";
                    ddlemp.DataTextField = "EmpName";
                    ddlemp.DataBind();
                    ddlemp.SelectedIndex = 0;
                }
            }
        }

        if (Request.QueryString["UserRole"] == "Administrator")
        {
            Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType Not in (1,5,12) and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE IN ('R','P')", "AE.EmployeeID");
        }

        if (Request.QueryString["UserRole"] == "Manager")
        {
            DataTable dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager_FOR_POPUP(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()));

            ddlemp.Items.Clear();
            ddlemp.Items.Add("Please Select");
            ddlemp.SelectedItem.Value = "";
            if (dtMangerEmpList.Rows.Count > 0)
            {
                ddlemp.DataSource = dtMangerEmpList;
                ddlemp.DataValueField = "EmployeeID";
                ddlemp.DataTextField = "EmpName";
                ddlemp.DataBind();
                ddlemp.SelectedIndex = 0;
            }
        }

        if (Request.QueryString["UserRole"] == "Employee")
        {
            Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID=AE.EmployeeID) ", "AE.EmployeeID ", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName ", "ASSB.BusinessID=" + Bid + " and AE.UserType=4 and ae.EmployeeID in (select EmployeeId from MappingTable where USERID='" + Session["UserId"].ToString() + "' and BusinessID=" + Bid + ") and AE.IsActive='YES' ", "AE.EmployeeID");
        }

        if (Request.QueryString["UserRole"] == "Accountant")
        {
            Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID=AE.EmployeeID) ", "AE.EmployeeID ", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName ", "ASSB.BusinessID=" + Bid + " and AE.UserType=12 and ae.EmployeeID in (select EmployeeId from MappingTable where USERID='" + Session["UserId"].ToString() + "' and BusinessID=" + Bid + ") and AE.IsActive='YES' ", "AE.EmployeeID");
        }
    }

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

    public DataTable CustomRole_Pop_up_Employee_List(int BusinessId, string Employee_Type, int EmployeeId)
    {
        //Below two datatable to check the owner is Regular Owner or Project Owner
        //DataTable Dt_Check_EmployeeId_For_Owner = comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + BusinessId + "' and UserTypeId='5' ", "UserTypeId");
        //DataTable Dt_Check_Owner_EMPLOYEETYPE_Column_Value = comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Dt_Check_EmployeeId_For_Owner.Rows[0]["EmployeeID"].ToString() + "' ", "");

        try
        {
            loginfo = "CustomRole_Pop_up_Employee_List(" + BusinessId + "," + Employee_Type + "," + EmployeeId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            //if ((Dt_Check_Owner_EMPLOYEETYPE_Column_Value.Rows[0]["EMPLOYEETYPE"].ToString()) == "R")
            //    cmd.CommandText = "getOwnerRole_For_Accountant_User";
            //if ((Dt_Check_Owner_EMPLOYEETYPE_Column_Value.Rows[0]["EMPLOYEETYPE"].ToString()) == "P")
            //    cmd.CommandText = "getOwnerRole";
            //firoz 0405
            //cmd.CommandText = "Get_Custom_Role_User_Popup_Emp_List"; //commented because not giving proper data
            //cmd.CommandText = "GetCustomRoleEmp"; //commented because kumar added new procedure
            cmd.CommandText = "Get_Cutom_Role_User_PopUp_Emp_List";
            cmd.Parameters.AddWithValue("@BusinessId", BusinessId);
            //cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmd.Parameters.AddWithValue("@CustomRoleUser_EmployeeId", EmployeeId);
            //cmd.Parameters.AddWithValue("@Ownerid", EmployeeId);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "CustomRole_Pop_up_Employee_List(" + BusinessId + "," + Employee_Type + "," + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("CustomRole_Pop_up_Employee_List" + ex.Message);
            throw ex;
        }
    }
}