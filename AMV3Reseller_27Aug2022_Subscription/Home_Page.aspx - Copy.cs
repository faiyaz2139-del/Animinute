using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using log4net;


public partial class Home_Page : System.Web.UI.Page
{

    public static string strEmpStatus;
    public static string strEmpId;

    CommanClass comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    AddProject AddProject = new AddProject();
    int mangercheck = 0;
    int customroleusercheck = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            bool Check_Subscriber_Active_Status = comm.IfExistsInTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and IsActive='NO        ' and UserType='1' ");
            if (Check_Subscriber_Active_Status)
            {
                btnSaveProjectDetails.Visible = false;
                return;
            }
            if (Session["MotherOrgid"] != null)
            {
                if (Session["UserType"].ToString() != "DELIGATES")
                {
                    GetEmplist();
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
            //if(Linkbutt )
            //{

            //}
        }
    }

    public void GetEmplist()
    {
        DataTable dtBussList = new DataTable();

        if (Session["EmployeeId"].ToString() == "0")
        {

        }
        else
        {
            string empid = "0";
            DataTable dtCommaUser = comm.GetAllDataFromTable("AddEmployee ad with (nolock) join EmpAddress ea with (nolock) on ad.ADDRESSID=ea.EmpAddressId join UserAcc ua with (nolock) on ua.Email=ea.MainEmail", "ad.EmployeeID,ua.UserId", "ua.Email='" + Session["email"].ToString() + "'", "");
            string CommaUserid = string.Empty;
            for (int i = 0; i < dtCommaUser.Rows.Count; i++)
            {
                CommaUserid = CommaUserid + dtCommaUser.Rows[i]["UserId"].ToString();
                CommaUserid += (i < dtCommaUser.Rows.Count) ? "," : string.Empty;
            }
            CommaUserid = CommaUserid.TrimEnd(',');
            DataTable dtMapping = new DataTable(); dtMapping = comm.GetAllDataFromTable("MappingTable with (Nolock)", "*", "USERID in(" + CommaUserid + ")", "BusinessID,UserTypeId");

            int cnt = dtMapping.Rows.Count;
            foreach (DataRow row in dtMapping.Rows)
            {
                if (row["UserTypeId"].ToString() == "1")
                {
                    int rCount = 0;
                    int rDeactiveCount = 0;
                    DataTable dtIsEdit = new DataTable();
                    dividBusinessOwner.Attributes.Add("style", "display:block");
                    try
                    {
                        if (Session["Redirect"] != null)
                        {
                            if (Session["MotherOrgid_acc"].ToString() != "")
                            {
                                dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            }
                            else
                            {
                                dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            }
                        }
                        if (Session["Redirect"] == null)
                        {
                            dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                        }
                    }
                    catch (Exception ex)
                    {
                        dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                    }
                    rCount = dtBussList.Rows.Count;
                    dtIsEdit = comm.GetAllDataFromTable("AddEmployee with (nolock)", "*", "EmployeeID=" + int.Parse(row["EmployeeID"].ToString().ToString()) + "", "");
                    DataRow[] rowsFiltered = dtBussList.Select("Status=2");
                    rDeactiveCount = int.Parse(rowsFiltered.Length.ToString());
                    if (rCount == rDeactiveCount)
                    {
                        GvEmpListForBusOwner.DataSource = null;
                        if (rCount != 0)
                        {
                            Label_Business_Owner_Deactive_Message.Visible = true;
                        }
                        else
                        {
                            DataTable chkboisActive = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + row["EmployeeID"].ToString() + " and IsActive='YES'", "");
                            if (chkboisActive.Rows.Count > 0)
                            {
                                Label_Business_Owner_Grid.Visible = false;
                            }
                            else
                            {
                                Label_Business_Owner_Deactive_Message.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        Label_Business_Owner_Grid.Visible = false;
                        GvEmpListForBusOwner.DataSource = dtBussList;
                    }
                    GvEmpListForBusOwner.DataBind();

                    var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                    bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                    if (!check_reseller_or_subscriber_email)
                    {
                        var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' ");
                        if (check_old_subscriber.Rows.Count > 0)
                        {
                            var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");

                            if (check_cancelled.Rows.Count > 0)
                            {
                                GvEmpListForBusOwner.Enabled = false;
                                Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                            }
                        }
                    }
                    if (check_reseller_or_subscriber_email)
                    {
                        var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
                        if (check_old_reseller.Rows.Count > 0)
                        {
                            var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                            if (check_cancelled.Rows.Count == 0)
                            {
                                DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                                DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                                if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                                {
                                    GvEmpListForBusOwner.Enabled = false;
                                    Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                                }


                                DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=0 and " +
                                    "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                                if (dt_check_active_status.Rows.Count > 0)
                                {
                                    GvEmpListForBusOwner.Enabled = false;
                                    Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                                }
                            }
                            if (check_cancelled.Rows.Count > 0)
                            {
                                GvEmpListForBusOwner.Enabled = false;
                                Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                            }
                        }
                    }
                }
                else if (row["UserTypeId"].ToString() == "4")
                {
                    dividEmp.Attributes.Add("style", "display:block;");
                    if (cnt > 1)
                    {
                        dividEmp.Attributes.Add("style", "display:block;");
                    }

                    empid = row["EmployeeID"].ToString() + "," + empid;
                    if (empid.Length > 0)
                    {
                        Label_Employee_Grid.Visible = false;
                        comm.FillGridView(gvBusinessListEmp, "AddEmployee Ae with (nolock) join AssginBussiness Assb with (nolock) on ae.EmployeeID=Assb.EmployeeID join EmpAddress ea with (nolock) on ea.EmpAddressId=ae.ADDRESSID and ea.MainEmail<>''", "ae.EmployeeID,assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID)BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName,(select PrimaryContactNumber from AddBusiness where BusinessID=Assb.BusinessID)PrimaryContactNumber,(select OperatingName from AddBusiness where BusinessID=Assb.BusinessID)OperatingName", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp,AE.EMPLOYEETYPE", "ae.EmployeeID in (" + empid + ")  and assb.UserTypeId=" + row["UserTypeid"].ToString() + " and ae.IsActive='YES'  AND assb.BusinessID  IN (SELECT  BusinessID from AddBusiness where AddBusiness.Status=1 ) ", "ae.EmployeeID");

                        //commented after creating issue in parmeswari login issue to not see option to enter timesheet.
                        //DataTable Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission with (Nolock)", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' ", "");
                        ////Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
                        //if (Dt_Check_Role_Permission.Rows.Count == 0)
                        //    this.gvBusinessListEmp.Columns[6].Visible = false;
                    }
                }
                else if (row["UserTypeId"].ToString() == "2")
                {
                    dividManager.Attributes.Add("style", "display:block;");
                    if (cnt > 1)
                    {

                    }

                    DataTable dtManagerBussniess = new DataTable();
                    if (Session["UserTypeid"].ToString() == "4")
                    {
                        DataTable dt_cnt = new DataTable();

                        dtManagerBussniess = AddEmpCtrl.GetBussList_ManagerForDashBord_Manager_Emp_to_Manager(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeId"].ToString()), int.Parse(row["UserTypeid"].ToString()), int.Parse(Session["UserId"].ToString()), Session["email"].ToString());
                        dtManagerBussniess.Columns.Add("Count", typeof(System.Int32));
                        foreach (DataRow row1 in dtManagerBussniess.Rows)
                        {
                            dt_cnt = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(row1["ManagerID"].ToString()), int.Parse(row1["BusinessID"].ToString()), int.Parse(Session["MotherOrgid"].ToString()), "P,R");
                            row1["Count"] = dt_cnt.Rows.Count;
                        }
                    }
                    else
                    {
                        DataTable dt_cnt = new DataTable();
                        dtManagerBussniess = AddEmpCtrl.GetBussList_ManagerForDashBord(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeId"].ToString()), int.Parse(row["UserTypeid"].ToString()), int.Parse(Session["UserId"].ToString()), Session["email"].ToString());
                        dtManagerBussniess.Columns.Add("Count", typeof(System.Int32));
                        foreach (DataRow row1 in dtManagerBussniess.Rows)
                        {
                            dt_cnt = AddEmpCtrl.GetEmpListForDefaultFromManager_FOR_POPUP(int.Parse(row1["ManagerID"].ToString()), int.Parse(row1["BusinessID"].ToString()), int.Parse(Session["MotherOrgid"].ToString()));
                            row1["Count"] = dt_cnt.Rows.Count;
                        }
                    }

                    Label_Manager_Grid.Visible = false;
                    gvbusinessListForManager.DataSource = dtManagerBussniess;
                    gvbusinessListForManager.DataBind();
                }
                else if (row["UserTypeId"].ToString() == "5")
                {
                    DataTable dtownr = new DataTable();
                    DataTable dtEmpType = new DataTable();
                    dtEmpType = comm.GetAllDataFromTable("AddEmployee ad with (Nolock) join EmpAddress ea with (Nolock) on ad.ADDRESSID=ea.EmpAddressId", "ad.EMPLOYEETYPE", "ea.MainEmail='" + Session["email"].ToString() + "' and ad.UserType='" + row["UserTypeId"].ToString() + "' and ad.EmployeeID='" + row["EmployeeID"].ToString() + "'", "");
                    string strEmpType = dtEmpType.Rows[0]["EMPLOYEETYPE"].ToString();
                    ViewState["strEmpType"] = strEmpType;
                    //Uncommented we have added new user type accountant whose employeetype column in database is N
                    dtownr = AddEmpCtrl.GetOwnerDetails(Session["email"].ToString(), int.Parse(Session["MotherOrgid"].ToString()), strEmpType);

                    //if (strEmpType == "P")
                    //    dtownr = AddEmpCtrl.GetOwnerDetails(Session["email"].ToString(), int.Parse(Session["MotherOrgid"].ToString()), strEmpType);
                    //if (strEmpType == "R")
                    //    dtownr = Regular_Owner_Gridview_Binding(Session["email"].ToString(), int.Parse(Session["MotherOrgid"].ToString()), strEmpType);

                    if (dtownr.Rows.Count > 0)
                    {
                        Owner.Attributes.Add("style", "display:block");
                        Label_Owner_Grid.Visible = false;
                        GvEmpListOwner.DataSource = dtownr;
                        GvEmpListOwner.DataBind();
                    }
                }
                else if (row["UserTypeId"].ToString() == "12")
                {
                    div_Accountant_User.Attributes.Add("style", "display:block;");
                    if (cnt > 1)
                    {
                        div_Accountant_User.Attributes.Add("style", "display:block;");
                    }

                    empid = row["EmployeeID"].ToString() + "," + empid;
                    if (empid.Length > 0)
                    {
                        Label_Accountant_Grid.Visible = false;
                        comm.FillGridView(GridView_Accountant_User, "AddEmployee Ae with (Nolock) join AssginBussiness Assb with (Nolock) on ae.EmployeeID=Assb.EmployeeID join EmpAddress ea with (Nolock) on ea.EmpAddressId=ae.ADDRESSID and ea.MainEmail<>''", "ae.EmployeeID,assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID)BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName,(select PrimaryContactNumber from AddBusiness where BusinessID=Assb.BusinessID)PrimaryContactNumber,(select OperatingName from AddBusiness where BusinessID=Assb.BusinessID)OperatingName", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp,AE.EMPLOYEETYPE", "ae.EmployeeID in (" + empid + ")  and assb.UserTypeId=" + row["UserTypeid"].ToString() + " and ae.IsActive='YES'  AND assb.BusinessID  IN (SELECT  BusinessID from AddBusiness where AddBusiness.Status=1 ) ", "ae.EmployeeID");
                    }
                }
                #region for custom role we are adding a new gridview
                else if ((row["UserTypeId"].ToString() != "1") && (row["UserTypeId"].ToString() != "2") && (row["UserTypeId"].ToString() != "4")
                        && (row["UserTypeId"].ToString() != "5") && (row["UserTypeId"].ToString() != "12"))
                {
                    //DataTable dtEmpType = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId", "ad.EMPLOYEETYPE", "ea.MainEmail='" + Session["email"].ToString() + "' and ad.UserType='" + row["UserTypeId"].ToString() + "' and ad.EmployeeID='" + row["EmployeeID"].ToString() + "'", "");
                    //string strEmpType = dtEmpType.Rows[0]["EMPLOYEETYPE"].ToString();
                    //ViewState["strEmpType"] = strEmpType;

                    //there is no need of employee type for this to fetch data as all user will see both regular and project employees
                    //R is mentioned so that procedure do not give any error
                    //There is no use of MotherOrgid. It should not give error so we have left it.
                    DataTable dtcustom_user_role = Additional_Role_Gridview_Binding(Session["email"].ToString(), int.Parse(Session["MotherOrgid"].ToString()), "R", int.Parse(row["UserTypeId"].ToString()), int.Parse(row["EmployeeID"].ToString())); // strEmpType);

                    if (dtcustom_user_role.Rows.Count > 0)
                    {
                        div_Custome_User_Role.Attributes.Add("style", "display:block");
                        Label_Custome_User_Role_Grid.Visible = false;
                        GridView_Custome_User_Role.DataSource = dtcustom_user_role;
                        GridView_Custome_User_Role.DataBind();
                    }
                }
                #endregion
            }
        }
    }

    protected void GvEmpListForBusOwner_PreRender(object sender, EventArgs e)
    {
        if (GvEmpListForBusOwner.Rows.Count > 0)
        {
            //GvEmpListForBusOwner.UseAccessibleHeader = true;
            //GvEmpListForBusOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvEmpListForBusOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdStatus = (e.Row.FindControl("hfdStatus") as HiddenField);
            //ImageButton togBtnForBus = (e.Row.FindControl("togBtnForBus") as ImageButton);
            HiddenField hdfBusinessID = (e.Row.FindControl("hdfBusinessID") as HiddenField);
            HyperLink hyeditBuss = (e.Row.FindControl("hyeditBuss") as HyperLink);
            HyperLink hyeditManger = (e.Row.FindControl("hyeditManger") as HyperLink);
            LinkButton lnkBusOnwerEmpList = (e.Row.FindControl("lnkBusOnwerEmpList") as LinkButton);
            HyperLink hyAddEmp = (e.Row.FindControl("hyAddEmp") as HyperLink);
            HyperLink hyAssbus = (e.Row.FindControl("hyAssbus") as HyperLink);
            HyperLink hyViewTimeSheet = (e.Row.FindControl("hyViewTimeSheet") as HyperLink);
            HyperLink hyAssManger = (e.Row.FindControl("hyAssManger") as HyperLink);
            //HyperLink hyProjectTimeSheet = (e.Row.FindControl("hyProjectTimeSheet") as HyperLink);
            //DropDownList ddlTimeSheetEntry = (e.Row.FindControl("ddlTimeSheetEntry") as DropDownList);
            DropDownList Ddl_Add_Project = (e.Row.FindControl("ddcreateproject") as DropDownList);
            LinkButton hyViewProj = (e.Row.FindControl("hyViewProj") as LinkButton);
            LinkButton hyAddProj = (e.Row.FindControl("hyAddProj") as LinkButton);

            //DataTable dtEmptype = comm.GetAllDataFromTable("AddEmployee ad with (Nolock) join AssginBussiness assb with (Nolock) lnkBusOnwerEmpListon ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + hdfBusinessID.Value + " and assb.UserTypeId in (2,4,5)", "");
            DataTable dtEmptype = comm.GetAllDataFromTable("AddEmployee ad with (Nolock) join AssginBussiness assb with (Nolock) on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + hdfBusinessID.Value + " and assb.UserTypeId not in (1)", "");
            if (dtEmptype.Rows.Count != 0)
            {
                if (dtEmptype.Rows.Count > 1)
                {
                    hyViewTimeSheet.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + hdfBusinessID.Value;
                }
                else
                {
                    if (dtEmptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P")
                    {
                        hyViewTimeSheet.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + hdfBusinessID.Value;
                    }
                    else if (dtEmptype.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                    {
                        hyViewTimeSheet.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + hdfBusinessID.Value;
                    }
                }
            }
            else
            {
                hyViewTimeSheet.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + hdfBusinessID.Value;
            }
            if (hfdStatus.Value.Trim() == "1")
            {

            }
            else
            {
                hyeditBuss.Enabled = false;
                hyeditManger.Enabled = false;
                lnkBusOnwerEmpList.Enabled = false;
                hyAddEmp.Enabled = false;
                hyAssbus.Enabled = false;
                hyViewTimeSheet.Enabled = false;
                hyAssManger.Enabled = false;
                e.Row.BackColor = System.Drawing.Color.LightGray;
                Ddl_Add_Project.Enabled = false;
                hyViewProj.Enabled = false;
                hyAddProj.Enabled = false;
            }
        }
    }

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {
        if (GvPopUpEmpList.Rows.Count > 0)
        {
            //GvPopUpEmpList.UseAccessibleHeader = true;
            //GvPopUpEmpList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvPopUpEmpList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (Session["UserTypeid"].ToString() == "1")
            {
                this.GvPopUpEmpList.Columns[5].Visible = false;
            }
            HyperLink hyeditEmp = (e.Row.FindControl("hyeditEmp") as HyperLink);
            HyperLink hyeditEmprate = (e.Row.FindControl("hyeditEmprate") as HyperLink);
            HyperLink hyEnterTimeSheet = (e.Row.FindControl("hyEnterTimeSheet") as HyperLink);
            HiddenField hfdStatusForBussOwner = (e.Row.FindControl("hfdStatusForBussOwner") as HiddenField);
            HiddenField hdfEmpNameid = (e.Row.FindControl("hdfEmpNameid") as HiddenField);
            ImageButton togBtnForBusOwner = (e.Row.FindControl("togBtnForBusOwner") as ImageButton);
            HyperLink HyperLink_Change_Address_By_Manager = (e.Row.FindControl("HyperLink_Change_Address_By_Manager") as HyperLink);
            string aa = Session["EmployeeId"].ToString();
            DataTable dtGv = comm.GetAllDataFromTable("AddEmployee ad with (Nolock) join EmpAddress ea with (Nolock) on ad.ADDRESSID=ea.EmpAddressId", "ea.MainEmail", "ad.EmployeeID=" + hdfEmpNameid.Value + "", "");
            DataTable dtlogin = comm.GetAllDataFromTable("AddEmployee ad with (Nolock) join EmpAddress ea with (Nolock) on ad.ADDRESSID=ea.EmpAddressId", "ea.MainEmail", "ad.EmployeeID=" + aa + "", "");

            if (dtGv.Rows.Count > 0 && dtlogin.Rows.Count > 0)
            {
                if (dtGv.Rows[0]["MainEmail"].ToString() == dtlogin.Rows[0]["MainEmail"].ToString() && mangercheck > 0)
                {
                    hyeditEmp.Visible = false;
                    hyeditEmprate.Visible = false;
                    togBtnForBusOwner.Enabled = false;
                }

                if (dtGv.Rows[0]["MainEmail"].ToString() == dtlogin.Rows[0]["MainEmail"].ToString() && customroleusercheck > 0)
                {
                    hyeditEmp.Visible = false;
                    hyeditEmprate.Visible = false;
                    togBtnForBusOwner.Enabled = false;

                    string[] role;
                    DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hfdPopUpBusid.Value + "'", "UserTypeId");
                    DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
                    role = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                    for (int i = 0; i < role.Length; i++)
                    {
                        if (role[i].ToString() == "11" || role[i].ToString() == "22")
                        {
                            HyperLink_Change_Address_By_Manager.Visible = true;
                            break;
                        }
                        else
                        {
                            HyperLink_Change_Address_By_Manager.Visible = false;
                        }
                    }
                    for (int i = 0; i < role.Length; i++)
                    {
                        if (role[i].ToString() == "10" || role[i].ToString() == "21")
                        {
                            togBtnForBusOwner.Visible = true;
                            togBtnForBusOwner.Enabled = false;
                            break;
                        }
                        else
                        {
                            togBtnForBusOwner.Visible = false;
                            togBtnForBusOwner.Enabled = false;
                        }
                    }
                    if (role.Contains("3"))
                    {
                        HyperLink_Change_Address_By_Manager.Visible = true;
                    }
                }

                if ((dtGv.Rows[0]["MainEmail"].ToString() != dtlogin.Rows[0]["MainEmail"].ToString()) && customroleusercheck > 0)
                {
                    string[] role;
                    DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hfdPopUpBusid.Value + "'", "UserTypeId");
                    DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
                    role = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                    for (int i = 0; i < role.Length; i++)
                    {
                        if (role[i].ToString() == "15")
                        {
                            hyeditEmprate.Visible = true;
                            break;
                        }
                        else
                        {
                            hyeditEmprate.Visible = false;
                        }
                    }
                    for (int i = 0; i < role.Length; i++)
                    {
                        if (role[i].ToString() == "11" || role[i].ToString() == "22")
                        {
                            HyperLink_Change_Address_By_Manager.Visible = true;
                            break;
                        }
                        else
                        {
                            HyperLink_Change_Address_By_Manager.Visible = false;
                        }
                    }
                    for (int i = 0; i < role.Length; i++)
                    {
                        if (role[i].ToString() == "10" || role[i].ToString() == "21")
                        {
                            togBtnForBusOwner.Visible = true;
                            break;
                        }
                        else
                        {
                            togBtnForBusOwner.Visible = false;
                        }
                    }
                    #region check single value from array c# backend
                    if (role.Contains("3"))
                    {
                        HyperLink_Change_Address_By_Manager.Visible = false;
                    }
                    if (role.Contains("11"))
                    {
                        HyperLink_Change_Address_By_Manager.Visible = true;
                    }
                    if (role.Contains("22"))
                    {
                        HyperLink_Change_Address_By_Manager.Visible = true;
                    }
                    #endregion

                    if (role.Contains("6") || role.Contains("14"))
                    {
                        hyeditEmp.Visible = true;
                    }
                    else
                        hyeditEmp.Visible = false;
                }
            }

            if (hfdStatusForBussOwner.Value.Trim() == "YES")
            {
                togBtnForBusOwner.ImageUrl = "~/images/active.png";
            }
            else
            {
                togBtnForBusOwner.ImageUrl = "~/images/deactive.png";
                e.Row.BackColor = System.Drawing.Color.LightGray;
                hyeditEmp.Enabled = false;
                hyeditEmprate.Enabled = false;
                hyEnterTimeSheet.Enabled = false;
            }

            #region to disabble edit and activate/deactivate the owner when owner log in
            if (Session["UserTypeid"].ToString() != "1")
            {
                DataTable Get_Owner_EmployeeID_UserType_In_Grid = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "UserType", "EmployeeID='" + int.Parse(hdfEmpNameid.Value) + "'", "");
                if (Get_Owner_EmployeeID_UserType_In_Grid.Rows[0]["UserType"].ToString() == "5")
                {
                    hyeditEmp.Visible = false;
                    hyeditEmprate.Visible = false;
                    togBtnForBusOwner.Enabled = false;
                }
            }
            #endregion
        }
    }

    protected void gvbusinessListForManager_PreRender(object sender, EventArgs e)
    {
        if (gvbusinessListForManager.Rows.Count > 0)
        {
            gvbusinessListForManager.UseAccessibleHeader = true;
            gvbusinessListForManager.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void gvBusinessListEmp_PreRender(object sender, EventArgs e)
    {
        if (gvBusinessListEmp.Rows.Count > 0)
        {
            gvBusinessListEmp.UseAccessibleHeader = true;
            gvBusinessListEmp.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    [System.Web.Services.WebMethod]
    protected void GvEmpListOwner_PreRender(object sender, EventArgs e)
    {
        if (GvEmpListOwner.Rows.Count > 0)
        {
            GvEmpListOwner.UseAccessibleHeader = true;
            GvEmpListOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvEmpListOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdStatus1 = (e.Row.FindControl("hfdStatus1") as HiddenField);
            ImageButton togBtnForBus1 = (e.Row.FindControl("togBtnForBus1") as ImageButton);

            HyperLink hyeditBuss1 = (e.Row.FindControl("hyeditBuss1") as HyperLink);
            HyperLink hyeditManger1 = (e.Row.FindControl("hyeditManger1") as HyperLink);
            LinkButton lnkBusOnwerEmpList1 = (e.Row.FindControl("lnkBusOnwerEmpList1") as LinkButton);
            HyperLink hyAddEmp1 = (e.Row.FindControl("hyAddEmp1") as HyperLink);
            HyperLink hyAssbus1 = (e.Row.FindControl("hyAssbus1") as HyperLink);
            HyperLink hyViewTimeSheet1 = (e.Row.FindControl("hyViewTimeSheet1") as HyperLink);
            HyperLink hyAssManger1 = (e.Row.FindControl("hyAssManger1") as HyperLink);

            if (hfdStatus1.Value.Trim() == "1")
            {
                togBtnForBus1.ImageUrl = "~/images/active.png";
            }
            else
            {
                togBtnForBus1.ImageUrl = "~/images/deactive.png";
                //all deactive
                hyeditBuss1.Enabled = false;
                hyeditManger1.Enabled = false;
                lnkBusOnwerEmpList1.Enabled = false;
                hyAddEmp1.Enabled = false;
                hyAssbus1.Enabled = false;
                hyViewTimeSheet1.Enabled = false;
                hyAssManger1.Enabled = false;
                e.Row.BackColor = System.Drawing.Color.LightGray;
            }

            HiddenField hdfBusinessID1 = (e.Row.FindControl("hdfBusinessID1") as HiddenField);
            HyperLink HyperLink_Schedular_Owner = (e.Row.FindControl("HyperLink_Schedular_Owner") as HyperLink);
            if (check_cancelled_reseller_suscriber(int.Parse(hdfBusinessID1.Value)) > 0)
            {
                e.Row.Enabled = false;
            }

            string[] roles;
            DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hdfBusinessID1.Value + "'", "UserTypeId");
            DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
            if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
            {
                roles = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                if (roles.Contains("31"))
                {
                    HyperLink_Schedular_Owner.Visible = true;
                }
            }
        }
    }

    protected void GvEmpListForBusOwner_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvEmpListForBusOwner.PageIndex = e.NewPageIndex;
        GetEmplist();
    }

    protected void GvPopUpEmpList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvPopUpEmpList.PageIndex = e.NewPageIndex;
        comm.FillGridViewDataNull(GvPopUpEmpList, "AddEmployee AE with (Nolock) left JOIN AssginBussiness Assb with (Nolock) on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD with (Nolock) ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID and effectivedate <= GETDATE())", "AE.EmployeeID,ASSB.BusinessID,AE.EMPLOYEETYPE", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " and AE.UserType!=1  ", "AE.EmployeeID");//AE.UserType!=1 after and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "
        ModalPopupEmpList.Show();
    }

    protected void gvbusinessListForManager_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvbusinessListForManager.PageIndex = e.NewPageIndex;
        GetEmplist();
    }

    protected void gvBusinessListEmp_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvbusinessListForManager.PageIndex = e.NewPageIndex;
        GetEmplist();
    }

    protected void GvEmpListOwner_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvEmpListOwner.PageIndex = e.NewPageIndex;
        GetEmplist();
    }

    protected void GvEmpListForBusOwner_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "BusOnwerEmpList")
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Response.Cookies["tabid"].Value = "#home";
            hfdPopUpBusid.Value = "0";

            Label CompanyName = (Label)GvEmpListForBusOwner.Rows[index].FindControl("CompanyName");
            lblbusName.Text = CompanyName.Text;
            hfdPopUpBusid.Value = commandArguments[1].ToString();

            DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(hfdPopUpBusid.Value), int.Parse(Session["EmployeeId"].ToString()));
            GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
            GvPopUpEmpList.DataBind();
            ModalPopupEmpList.Show();

            //DataRow[] dr;
            //DataTable dtr;
            //dr = null;
            //dtr = null;
            //dr = Business_Owner_Pop_up_Employee_List_.Select("[IsActive]='YES'");
            //if (dr.Count() > 0)
            //{
            //    dtr = dr.CopyToDataTable();
            //    GvPopUpEmpList.DataSource = dtr;
            //    GvPopUpEmpList.DataBind();
            //}
        }
        else if (e.CommandName == "ClientStatus")
        {
            int a = 0;
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            int hdfBusinessID = 0;
            hdfBusinessID = Convert.ToInt32(commandArguments[1].ToString());
            DataTable dtAllEmp = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "*", "BusinessID=" + hdfBusinessID + "", "");

            HiddenField hfdStatus = (HiddenField)GvEmpListForBusOwner.Rows[index].FindControl("hfdStatus");
            if (hfdStatus.Value.Trim() == "1")
            {
                a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + hdfBusinessID + "'");
                foreach (DataRow r in dtAllEmp.Rows)
                {
                    comm.ExecuteQuery("Update AddEmployee set IsActive='No' where EmployeeID='" + r["EmployeeID"].ToString() + "'");
                }

                string BussName = comm.GetNameFromId("AddBusiness", hdfBusinessID);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + BussName + " Has Been Deactivated', 'success');", true);
            }
            if (hfdStatus.Value.Trim() == "2")
            {
                a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + hdfBusinessID + "'");
                foreach (DataRow r in dtAllEmp.Rows)
                {
                    comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID='" + r["EmployeeID"].ToString() + "'");
                }

                string BussName = comm.GetNameFromId("AddBusiness", hdfBusinessID);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + BussName + " Has Been Activated', 'success');", true);
            }
            GetEmplist();
        }
        else if (e.CommandName == "BusOnwerViewProjectList")
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            //int index = Convert.ToInt32(commandArguments[0].ToString());
            Response.Cookies["tabid"].Value = "#home";
            hdbusid.Value = "0";
            //Label CompanyName = (Label)GvEmpListForBusOwner.Rows[index].FindControl("CompanyName");
            lblbussnm.Text = "Project Details";
            hdbusid.Value = commandArguments[1].ToString();
            GetProjlist();
            modalviewproject.Show();
        }
        else if (e.CommandName == "ShowEmployeeList")
        {
            string strURL = "Home_Page_EmpPopUpList.aspx";
            Session["BusinessID"] = e.CommandArgument;

            string s = "window.open('" + strURL + "', 'popup_window', 'width=1200,height=400,left=200,top=150,resizable=no');";
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true); 
                     //this.ClientScript.RegisterStartupScript(this.GetType(), "test", "alert('" + (string)e.CommandArgument + "'); window.open('" + strURL + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');", true);
           // window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');
        }
        else if (e.CommandName == "BusOnwerAddProjectList")
        {
            #region show message if maximum limit is reached. This region code is same as of in Save button.
            if (Session["UserTypeid"].ToString() == "1")
            {
                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' ");
                    if (check_old_subscriber.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                txtProjectName.ReadOnly = true;
                                txtstartdate.ReadOnly = true;
                                txtenddate.ReadOnly = true;
                                btnSaveProjectDetails.Visible = false;
                            }

                            //var empidddddddd = Session["EmployeeId"].ToString();
                            //DataTable dtBussCnttt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + empidddddddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empidddddddd + "' and IsActive=1)");
                            //DataTable dt_NoOfBusiness = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeId"].ToString()), int.Parse("1"));
                            //if (dtBussCnttt.Rows.Count > 0 && dt_NoOfBusiness.Rows.Count > 0)
                            //{
                            //    if (dt_NoOfBusiness.Rows.Count >= int.Parse(dtBussCnttt.Rows[0][0].ToString()))
                            //    {
                            //        txtProjectName.ReadOnly = true;
                            //        txtstartdate.ReadOnly = true;
                            //        txtenddate.ReadOnly = true;
                            //        btnSaveProjectDetails.Visible = false;
                            //    }
                            //}

                            //var empiddddddd = Session["EmployeeId"].ToString();
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empiddddddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empiddddddd + "' and IsActive=1)");
                            //int empcount = 0;
                            ////commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            ////DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                            //DataTable dtBussList = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                            //    "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                            //    "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                            //    "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                            //    "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                            //    "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
                            //for (int m = 0; m < dtBussList.Rows.Count; m++)
                            //    empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
                            //if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            //{
                            //    txtProjectName.ReadOnly = true;
                            //    txtstartdate.ReadOnly = true;
                            //    txtenddate.ReadOnly = true;
                            //    btnSaveProjectDetails.Visible = false;
                            //}
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            txtProjectName.ReadOnly = true;
                            txtstartdate.ReadOnly = true;
                            txtenddate.ReadOnly = true;
                            btnSaveProjectDetails.Visible = false;
                        }
                    }
                }
               
             
                if (check_reseller_or_subscriber_email)
                {
                    var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
                    if (check_old_reseller.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                txtProjectName.ReadOnly = true;
                                txtstartdate.ReadOnly = true;
                                txtenddate.ReadOnly = true;
                                btnSaveProjectDetails.Visible = false;
                            }

                            //DataTable No_of_business = comm.GetAllDataFromTable("AddBusiness AsB", "AsB.BusinessID, AsB.CompanyName, AsB.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            //int total_no_bussiness = 0;
                            //total_no_bussiness = int.Parse(No_of_business.Rows.Count.ToString());
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                            //int BUSSCNT_RESELLER = 0;
                            //BUSSCNT_RESELLER = int.Parse(dtBussCnt.Rows[0]["Business"].ToString());
                            //if (total_no_bussiness >= BUSSCNT_RESELLER)
                            //{
                            //    txtProjectName.ReadOnly = true;
                            //    txtstartdate.ReadOnly = true;
                            //    txtenddate.ReadOnly = true;
                            //    btnSaveProjectDetails.Visible = false;
                            //}
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            txtProjectName.ReadOnly = true;
                            txtstartdate.ReadOnly = true;
                            txtenddate.ReadOnly = true;
                            btnSaveProjectDetails.Visible = false;
                        }
                    }
                   
                }

            }
          
            #endregion

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Response.Cookies["tabid"].Value = "#home";
            hfdPopUpBusid.Value = "0";
            //Label CompanyName = (Label)GvEmpListForBusOwner.Rows[index].FindControl("CompanyName");
            lblbusinessname.Text = "Add Project"; // + CompanyName.Text;
            hfdPopUpBusid.Value = commandArguments[1].ToString();
            txtProjectName.Text = String.Empty;
            txtstartdate.Text = String.Empty;
            txtenddate.Text = String.Empty;
            lblerror.Visible = false;
            lblerror1.Visible = false;
            modaladdproject.Show();
        }
    }

    private void GetProjlist()
    {
        grdviewProject.DataSource = null;
        grdviewProject.DataBind();
        comm.FillGridView(grdviewProject, "ADDPROJECT with (Nolock)", "BusinessID,ProjectID,PROJECTNAME,CONVERT(NVARCHAR,startDate, 106) as startDate", "CONVERT(NVARCHAR,EndDate, 106) as EndDate", " Status='A' and BusinessID ='" + hdbusid.Value + "'", "ProjectID asc");
    }

    protected void grdviewProject_PreRender(object sender, EventArgs e)
    {
        if (grdviewProject.Rows.Count > 0)
        {
            grdviewProject.UseAccessibleHeader = true;
            grdviewProject.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void gvbusinessListForManager_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "MangerEmpList")
        {
            mangercheck = 1;
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            DataTable dtMangerEmpList = new DataTable();

            HiddenField hdfBusinessIDForManager = (HiddenField)gvbusinessListForManager.Rows[index].FindControl("hdfBusinessIDForManager");
            HiddenField hfdMangerIDFormManger = (HiddenField)gvbusinessListForManager.Rows[index].FindControl("hfdMangerIDFormManger");
            Label CompanyName = (Label)gvbusinessListForManager.Rows[index].FindControl("CompanyName");
            lblbusName.Text = CompanyName.Text;
            hfdPopUpBusid.Value = hdfBusinessIDForManager.Value;

            DataTable Dt_Check_Role_Permission = new DataTable();
            Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission with (Nolock)", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + hfdMangerIDFormManger.Value + "' ", "");
            //Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
            if (Dt_Check_Role_Permission.Rows.Count == 0)
                this.GvPopUpEmpList.Columns[5].Visible = false;

            dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager_FOR_POPUP(int.Parse(hfdMangerIDFormManger.Value), int.Parse(hdfBusinessIDForManager.Value), int.Parse(Session["MotherOrgid"].ToString()));
            Session["BusinessID"] = int.Parse(hdfBusinessIDForManager.Value);
            Session["ManagerID"] = int.Parse(hdfBusinessIDForManager.Value);
            string strURL = "Home_Page_EmpPopUpList.aspx";
            

            string s = "window.open('" + strURL + "', 'popup_window', 'width=1200,height=400,left=300,top=100,resizable=no');";
            //ClientScript.RegisterStartupScript(this.GetType(), "script", "alert('test')", true);
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
           // GvPopUpEmpList.DataSource = dtMangerEmpList;
            //GvPopUpEmpList.DataBind();
           // ModalPopupEmpList.Show();
        }
    }

    protected void GvEmpListOwner_RowCommand(object sender, GridViewCommandEventArgs e)
    {
       if (e.CommandName == "ShowEmployeeList")
        {
            string strURL = "Home_Page_EmpPopUpList.aspx";
            Session["BusinessID"] = e.CommandArgument;

            string s = "window.open('" + strURL + "', 'popup_window', 'width=1200,height=500,left=300,top=200,resizable=no');";
            //ClientScript.RegisterStartupScript(this.GetType(), "script", "alert('test')", true);
            ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
            //this.ClientScript.RegisterStartupScript(this.GetType(), "test", "alert('" + (string)e.CommandArgument + "'); window.open('" + strURL + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');", true);
            // window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');
        }
        else if (e.CommandName == "BusOnwerEmpList")
        {
            hfdPopUpBusid.Value = "0";
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            //HiddenField hdfBusinessID = (HiddenField)GvEmpListOwner.Rows[index].FindControl("hdfBusinessID1");
            Label CompanyName = (Label)GvEmpListOwner.Rows[index].FindControl("CompanyName1");
            lblbusName.Text = CompanyName.Text;
            hfdPopUpBusid.Value = commandArguments[1].ToString();

            DataTable Dt_Check_EmployeeId_For_Owner = comm.GetAllDataFromTable("MappingTable with (Nolock)", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + hfdPopUpBusid.Value + "' ", "UserTypeId");

            DataTable Dt_Check_Role_Permission = new DataTable();
            Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission with (Nolock)", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_EmployeeId_For_Owner.Rows[0]["EmployeeID"].ToString() + "' ", "");
            //Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
            if (Dt_Check_Role_Permission.Rows.Count == 0)
                this.GvPopUpEmpList.Columns[5].Visible = false;

            DataTable Owner_Pop_up_Employee_List_ = Owner_Pop_up_Employee_List(int.Parse(hfdPopUpBusid.Value), ViewState["strEmpType"].ToString(), int.Parse(Dt_Check_EmployeeId_For_Owner.Rows[0]["EmployeeID"].ToString()));
            GvPopUpEmpList.DataSource = Owner_Pop_up_Employee_List_;
            GvPopUpEmpList.DataBind();
            ModalPopupEmpList.Show();
        }
        else if (e.CommandName == "togBtnForBus1")
        {
            int a = 0;
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            // HiddenField hdfBusinessID = (HiddenField)GvEmpListOwner.Rows[index].FindControl("hdfBusinessID1");
            int hdfEmpNameid = int.Parse(commandArguments[1].ToString());
            HiddenField hfdStatus = (HiddenField)GvEmpListOwner.Rows[index].FindControl("hfdStatus1");
            if (hfdStatus.Value.Trim() == "1")
            {
                a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + commandArguments[1].ToString() + "'");
                string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been DeActivated', 'success');", true);
            }
            if (hfdStatus.Value.Trim() == "2")
            {
                a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + commandArguments[1].ToString() + "'");
                string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been Activated', 'success');", true);
            }
        }

    }

    protected void GvPopUpEmpList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "togBtnForBusOwner")
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            //int a = 0;
            int hdfEmpNameid = 0;
            //HiddenField hdfEmpNameid = (HiddenField)GvPopUpEmpList.Rows[index].FindControl("hdfEmpNameid");
            hdfEmpNameid = Convert.ToInt32(commandArguments[1].ToString());

            string dt = DateTime.Now.ToShortDateString();
            HiddenField hfdStatusForBussOwner = (HiddenField)GvPopUpEmpList.Rows[index].FindControl("hfdStatusForBussOwner");
            HiddenField hdfbussinessid = (HiddenField)GvPopUpEmpList.Rows[index].FindControl("hdfbusinessId");

            if (hfdStatusForBussOwner.Value.Trim().ToUpper() == "YES")
            {
                //a = comm.ExecuteQuery("Update AddEmployee set IsActive='NO', Apartment='" + dt + "' where EmployeeID='" + hdfEmpNameid + "'");
                comm.ExecuteQuery("Update AddEmployee set IsActive='NO', Apartment='" + dt + "' where EmployeeID='" + hdfEmpNameid + "'");
                string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been DeActivated', 'success');", true);
            }
            if (hfdStatusForBussOwner.Value.Trim().ToUpper() == "NO")
            {
                var check_if_deactivated_by_support_admin = comm.ExecuteQueryDataTable("select BusinessOwnerId from AddEmployee where EmployeeID='" + hdfEmpNameid + "'" +
                    "and BusinessOwnerId='2'");
                if (check_if_deactivated_by_support_admin.Rows.Count > 0)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'User has been DeActivated by Support Admin. So could not activate it. Please Contact Support Admin.', 'warning');", true);
                    return;
                }

                if (Session["UserTypeid"].ToString() == "1")
                {
                    var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                    bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                    if (!check_reseller_or_subscriber_email)
                    {
                        var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' ");
                        if (check_old_subscriber.Rows.Count > 0)
                        {
                            var empid = Session["EmployeeId"].ToString();
                            DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
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
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                                return;
                            }
                        }
                    }
                    if (check_reseller_or_subscriber_email)
                    {
                        var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' ");
                        if (check_old_reseller.Rows.Count > 0)
                        {
                            //int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
                            //DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad with (Nolock) join AssginBussiness asb with (Nolock) on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
                            //for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
                            //{
                            //    DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
                            //    total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
                            //}

                            //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                            //int EMPCNT_RESELLER = 0;
                            //EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                            //if (total_no_of_user >= EMPCNT_RESELLER)
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                            //    return;
                            //}

                            var empidddd = Session["EmployeeId"].ToString();
                            DataTable dtBussCnt = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users,MotherOrgID " +
                            "having EmpId='" + empidddd + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " " +
                            "and Id=(select max(id) from PlanHistory where EmpId='" + empidddd + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " and IsActive=1)");
                            int empcount = 0;
                            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            //DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
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
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                                return;
                            }
                        }
                    }
                }
              
                if (Session["UserTypeid"].ToString() != "1")
                {
                    DataTable Get_Reseller_Subscriber_Details = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "EmployeeID,MotherOrgID", "BusinessID = '" + int.Parse(hdfbussinessid.Value) + "' and UserTypeId=1", "");
                    DataTable Get_MotherOrg_Details = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ", "");
                    bool Check_Register_Partner_Details = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + Get_MotherOrg_Details.Rows[0]["Email"].ToString() + "' ");
                    if (!Check_Register_Partner_Details)
                    {
                        var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' ");
                        if (check_old_subscriber.Rows.Count > 0)
                        {
                            DataTable dtBussCntttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=1)");
                            int empppppcount = 0;
                            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            //DataTable dtBussListttt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()), int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()), int.Parse("1"));
                            DataTable dtBussListttt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' ");
                            for (int m = 0; m < dtBussListttt.Rows.Count; m++)
                                empppppcount = empppppcount + Convert.ToInt32(dtBussListttt.Rows[m]["CNT"].ToString());
                            if (empppppcount >= int.Parse(dtBussCntttt.Rows[0][0].ToString()))
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                                return;
                            }
                        }
                    }
                    if (Check_Register_Partner_Details)
                    {
                        var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ");
                        if (check_old_reseller.Rows.Count > 0)
                        {
                            //int total_no_of_user = 0;//for counting the total no of employees in all subscribers of particular reseller
                            //DataTable Get_Subsriber_List_For_Reseller = comm.GetAllDataFromTable("AddBusiness ad with (Nolock) join AssginBussiness asb with (Nolock) on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "and AsB.UserTypeId=1", "asb.EmployeeID");
                            //for (int k = 0; k < Get_Subsriber_List_For_Reseller.Rows.Count; k++)
                            //{
                            //    DataTable Get_Subscriber_Emp_List = GetEmplist_SuppAdmin("Reseller", int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()), int.Parse(Get_Subsriber_List_For_Reseller.Rows[k]["EmployeeID"].ToString()));
                            //    total_no_of_user = total_no_of_user + int.Parse(Get_Subscriber_Emp_List.Rows.Count.ToString());
                            //}

                            //DataTable dt_Emp_Cnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and IsActive=1)");
                            //int EMPCNT_RESELLER = 0;
                            //EMPCNT_RESELLER = int.Parse(dt_Emp_Cnt.Rows[0]["No_Of_Users"].ToString());

                            //if (total_no_of_user >= EMPCNT_RESELLER)
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                            //    return;
                            //}


                            //DataTable dtBussCntttt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and IsActive=1)");
                            DataTable dtBussCntttt = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) GROUP BY EmpId,id,No_Of_Users,MotherOrgID " +
                                "having EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " " +
                                "and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " and IsActive=1)");
                            int empppppcount = 0;
                            //commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            //DataTable dtBussListttt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()), int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()), int.Parse("1"));
                            DataTable dtBussListttt = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                                "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                                "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                                "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                                "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                                "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' " +
                                "and ASB.MotherOrgID='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ");
                            for (int m = 0; m < dtBussListttt.Rows.Count; m++)
                                empppppcount = empppppcount + Convert.ToInt32(dtBussListttt.Rows[m]["CNT"].ToString());
                            if (empppppcount >= int.Parse(dtBussCntttt.Rows[0][0].ToString()))
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot Activate User as Limit is Reached, Contact Admin', 'error');", true);
                                return;
                            }
                        }
                    }
                }

                //a = comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID='" + hdfEmpNameid + "'");
                comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID='" + hdfEmpNameid + "'");
                string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been Activated', 'success');", true);
            }
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

    public DataTable Owner_Pop_up_Employee_List(int BusinessId, string Employee_Type, int EmployeeId)
    {
        //Below two datatable to check the owner is Regular Owner or Project Owner
        DataTable Dt_Check_EmployeeId_For_Owner = comm.GetAllDataFromTable("MappingTable with (Nolock)", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + BusinessId + "' and UserTypeId='5' ", "UserTypeId");
        DataTable Dt_Check_Owner_EMPLOYEETYPE_Column_Value = comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Dt_Check_EmployeeId_For_Owner.Rows[0]["EmployeeID"].ToString() + "' ", "");

        try
        {
            loginfo = "Owner_Pop_up_Employee_List(" + BusinessId + "," + Employee_Type + "," + EmployeeId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            //if ((Dt_Check_Owner_EMPLOYEETYPE_Column_Value.Rows[0]["EMPLOYEETYPE"].ToString()) == "R")
            //    cmd.CommandText = "getOwnerRole_For_Accountant_User";
            //if ((Dt_Check_Owner_EMPLOYEETYPE_Column_Value.Rows[0]["EMPLOYEETYPE"].ToString()) == "P")
            //    cmd.CommandText = "getOwnerRole";

            cmd.CommandText = "getOwnerRole";

            cmd.Parameters.AddWithValue("@BusinessId", BusinessId);
            cmd.Parameters.AddWithValue("@Employee_Type", Employee_Type);
            cmd.Parameters.AddWithValue("@Ownerid", EmployeeId);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Owner_Pop_up_Employee_List(" + BusinessId + "," + Employee_Type + "," + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Owner_Pop_up_Employee_List" + ex.Message);
            throw ex;
        }
    }

    protected void GridView_Accountant_User_PreRender(object sender, EventArgs e)
    {
        if (GridView_Accountant_User.Rows.Count > 0)
        {
            GridView_Accountant_User.UseAccessibleHeader = true;
            GridView_Accountant_User.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
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

    //public DataTable Regular_Owner_Gridview_Binding(string Email, int MotherOrgId, string employee_type)
    //{
    //    try
    //    {
    //        loginfo = "Regular_Owner_Gridview_Binding(" + Email + "," + MotherOrgId + ", "+ employee_type + ") Start";
    //        Logger.Info(loginfo);

    //        DataTable dt = new DataTable();
    //        SqlDataAdapter da = new SqlDataAdapter();
    //        getconnection();
    //        SqlCommand cmd = new SqlCommand();

    //        cmd.CommandText = "sp_GetOwnerDetails_For_Regular_Owner";
    //        cmd.Parameters.AddWithValue("@email", Email);
    //        cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
    //        cmd.Parameters.AddWithValue("@EmpType", employee_type);
    //        cmd.CommandType = CommandType.StoredProcedure;

    //        cmd.Connection = con;
    //        da.SelectCommand = cmd;
    //        da.Fill(dt);

    //        loginfo = "Regular_Owner_Gridview_Binding(" + Email + "," + MotherOrgId + ", " + employee_type + ") end";
    //        Logger.Info(loginfo);

    //        return dt;
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Error("Regular_Owner_Gridview_Binding" + ex.Message);
    //        throw ex;
    //    }
    //}

    protected void GridView_Custome_User_Role_PreRender(object sender, EventArgs e)
    {
        if (GridView_Custome_User_Role.Rows.Count > 0)
        {
            GridView_Custome_User_Role.UseAccessibleHeader = true;
            GridView_Custome_User_Role.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    public DataTable Additional_Role_Gridview_Binding(string Email, int MotherOrgId, string employee_type, int UserTypeId, int EmployeeID)
    {
        try
        {
            loginfo = "Additional_Role_Gridview_Binding(" + Email + "," + MotherOrgId + ", " + employee_type + ", " + UserTypeId + ", " + EmployeeID + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "sp_Get_Addition_Role_Details";
            cmd.Parameters.AddWithValue("@email", Email);
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmpType", employee_type);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID); //@EmployeeID is not working properly, used so that it should not give error
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Additional_Role_Gridview_Binding(" + Email + "," + MotherOrgId + ", " + employee_type + ", " + UserTypeId + ", " + EmployeeID + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Additional_Role_Gridview_Binding" + ex.Message);
            throw ex;
        }
    }

    protected void grdviewProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdfProjNameid_ = (e.Row.FindControl("hdfProjNameid") as HiddenField);
            HiddenField HiddenField_BusinessID_For_Project_ = (e.Row.FindControl("HiddenField_BusinessID_For_Project") as HiddenField);
            DropDownList DropDownList_Project_Employee_Name_ = (e.Row.FindControl("DropDownList_Project_Employee_Name") as DropDownList);

            DataTable Get_Project_Employee_List = comm.GetAllDataFromTable("ASSIGNPROJECT with (Nolock)", "EMPLOYEEID,dbo.FN_DESC('AddEmployee',EMPLOYEEID)EmployeeName", "[BUSINESSID]=" + HiddenField_BusinessID_For_Project_.Value + " and [PROJECTID]=" + hdfProjNameid_.Value + "", "EMPLOYEEID");

            if (Get_Project_Employee_List.Rows.Count != 0)
            {
                DropDownList_Project_Employee_Name_.Items.Clear();
                //DropDownList_Project_Employee_Name_.Items.Add("Please Select");
                DropDownList_Project_Employee_Name_.Items.Add("Select User");
                DropDownList_Project_Employee_Name_.SelectedItem.Value = "";
                DropDownList_Project_Employee_Name_.DataSource = Get_Project_Employee_List;
                DropDownList_Project_Employee_Name_.DataValueField = Get_Project_Employee_List.Columns[0].ToString();
                DropDownList_Project_Employee_Name_.DataTextField = Get_Project_Employee_List.Columns[1].ToString();
                DropDownList_Project_Employee_Name_.DataBind();
            }
            else
            {
                DropDownList_Project_Employee_Name_.Items.Clear();
                //DropDownList_Project_Employee_Name_.Items.Add("No Employee Assigned");
                DropDownList_Project_Employee_Name_.Items.Add("No User");
                DropDownList_Project_Employee_Name_.SelectedItem.Value = "";
                DropDownList_Project_Employee_Name_.DataBind();
            }
        }
    }

    protected void btnSaveProjectDetails_Click(object sender, EventArgs e)
    {
        if (this.txtProjectName.Text.Trim() == string.Empty)
        {
            lblerror.Text = "Project Name Requried";
            lblerror.Visible = true;
            txtProjectName.Focus();
            return;
        }
        if (this.txtProjectName.Text.Trim() != string.Empty)
        {
            lblerror.Text = "";
            lblerror.Visible = false;
        }
        if (this.txtstartdate.Text.Trim() == string.Empty)
        {
            lblerror1.Text = "Start Date is Requried";
            lblerror1.Visible = true;
            txtstartdate.Focus();
            return;
        }
        if (this.txtstartdate.Text.Trim() != string.Empty)
        {
            lblerror1.Text = "";
            lblerror1.Visible = false;
        }
        if (txtstartdate.Text.Trim().Length > 0 && txtenddate.Text.Trim().Length > 0)
        {
            DateTime dtfrom = DateTime.Parse(txtstartdate.Text);
            DateTime dtto = DateTime.Parse(txtenddate.Text);
            if (dtfrom > dtto)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Start date is greater than End date!')", true);
                return;
            }
        }

        if (hfdsaveorupdate.Value == "SAVE")
        {
            AddProject.ProjectName = txtProjectName.Text.Trim().ToUpper();
            AddProject.StartDate = DateTime.Parse(txtstartdate.Text);
            AddProject.EndDate = txtenddate.Text;
            AddProject.BusinessID = Convert.ToInt32(hfdPopUpBusid.Value);
            //Commented because user can enter the deleted project name again.
            //if (comm.checkIfExists("AddProject", "ProjectName", txtProjectName.Text.Trim().ToUpper(), "BusinessID", hfdPopUpBusid.Value) == true)
            //Added by Prashant. User can enter the deleted project name again.
            DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectName='" + txtProjectName.Text + "' and BusinessID='" + int.Parse(hfdPopUpBusid.Value) + "' and Status='A' ", "");
            if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Data Already Exist', 'error');", true);
                modaladdproject.Hide();
                return;
            }
            int ProjId = AddEmpCtrl.InsertProjectDetails(AddProject);
            if (ProjId > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Project Saved', 'success');", true);
                txtProjectName.Text = String.Empty;
                txtstartdate.Text = String.Empty;
                txtenddate.Text = String.Empty;
                modaladdproject.Hide();
                return;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Error Occured', 'error');", true);
                modaladdproject.Hide();
                return;
            }
        }
        else if (hfdsaveorupdate.Value == "UPDATE")
        {
            AddProject.ProjectName = txtProjectName.Text.Trim().ToUpper();
            AddProject.StartDate = DateTime.Parse(txtstartdate.Text);
            AddProject.EndDate = txtenddate.Text;
            AddProject.BusinessID = Convert.ToInt32(hfdPopUpBusid.Value);
            AddProject.ProjectID = Convert.ToInt32(hfdprojectId.Value);
            AddProject.Status = "A";
            if (hdfprojname.Value.Trim().ToUpper() != txtProjectName.Text.Trim().ToUpper())
            {
                //Commented because user can enter the deleted project name again.
                //if (comm.checkIfExists("AddProject", "ProjectName", txtProjectName.Text.Trim().ToUpper(), "BusinessID", hfdPopUpBusid.Value) == true)
                //Added by Prashant. User can enter the deleted project name again.
                DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectName='" + txtProjectName.Text + "' and BusinessID='" + int.Parse(hfdPopUpBusid.Value) + "' and Status='A' ", "");
                if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Data Already Exist', 'error');", true);
                    hfdsaveorupdate.Value = "SAVE";
                    txtProjectName.Text = String.Empty;
                    txtstartdate.Text = String.Empty;
                    txtenddate.Text = String.Empty;
                    modaladdproject.Hide();
                    return;
                }
            }
            AddEmpCtrl.UpdateProjectDetails(AddProject);
            hfdsaveorupdate.Value = "SAVE";
            txtProjectName.Text = String.Empty;
            txtstartdate.Text = String.Empty;
            txtenddate.Text = String.Empty;
            modaladdproject.Hide();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Project Details Updated Successfully', 'success');", true);
        }
    }

    protected void GridView_Custome_User_Role_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "CustomRoleUserPopUpEmpList")
        {
            customroleusercheck = 1;
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());

            HiddenField hdfBusinessID1_Custome_User_Role = (HiddenField)GridView_Custome_User_Role.Rows[index].FindControl("hdfBusinessID1_Custome_User_Role");
            HiddenField HiddenField_EmployeeID = (HiddenField)GridView_Custome_User_Role.Rows[index].FindControl("HiddenField_EmployeeID");
            Label CompanyName = (Label)GridView_Custome_User_Role.Rows[index].FindControl("CompanyName1_Custome_User_Role");
            lblbusName.Text = CompanyName.Text;
            hfdPopUpBusid.Value = hdfBusinessID1_Custome_User_Role.Value;

            DataTable dt_Custome_User_Role_Popup_Emp_List = CustomRole_Pop_up_Employee_List(int.Parse(hdfBusinessID1_Custome_User_Role.Value), "R", int.Parse(HiddenField_EmployeeID.Value.ToString()));
            GvPopUpEmpList.DataSource = dt_Custome_User_Role_Popup_Emp_List;
            GvPopUpEmpList.DataBind();
            ModalPopupEmpList.Show();
        }

        //logic for pop up employee list is same as that of manager so we have copied from the manager logic
        //if (e.CommandName == "MangerEmpList")
        //{
        //    mangercheck = 1;
        //    string[] commandArguments = e.CommandArgument.ToString().Split(',');
        //    int index = Convert.ToInt32(commandArguments[0].ToString());
        //    DataTable dtMangerEmpList = new DataTable();

        //    HiddenField hdfBusinessIDForManager = (HiddenField)gvbusinessListForManager.Rows[index].FindControl("hdfBusinessIDForManager");
        //    HiddenField hfdMangerIDFormManger = (HiddenField)gvbusinessListForManager.Rows[index].FindControl("hfdMangerIDFormManger");
        //    Label CompanyName = (Label)gvbusinessListForManager.Rows[index].FindControl("CompanyName");
        //    lblbusName.Text = CompanyName.Text;
        //    hfdPopUpBusid.Value = hdfBusinessIDForManager.Value;

        //    DataTable Dt_Check_Role_Permission = new DataTable();
        //    Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + hfdMangerIDFormManger.Value + "' ", "");
        //    //Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
        //    if (Dt_Check_Role_Permission.Rows.Count == 0)
        //        this.GvPopUpEmpList.Columns[5].Visible = false;

        //    dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager_FOR_POPUP(int.Parse(hfdMangerIDFormManger.Value), int.Parse(hdfBusinessIDForManager.Value), int.Parse(Session["MotherOrgid"].ToString()));
        //    GvPopUpEmpList.DataSource = dtMangerEmpList;
        //    GvPopUpEmpList.DataBind();
        //    ModalPopupEmpList.Show();
        //}

        //else if (e.CommandName == "togBtnForBus1")
        //{
        //    int a = 0;
        //    string[] commandArguments = e.CommandArgument.ToString().Split(',');
        //    int index = Convert.ToInt32(commandArguments[0].ToString());
        //    //HiddenField hdfBusinessID = (HiddenField)GvEmpListOwner.Rows[index].FindControl("hdfBusinessID1");
        //    int hdfEmpNameid = int.Parse(commandArguments[1].ToString());
        //    HiddenField hfdStatus = (HiddenField)GvEmpListOwner.Rows[index].FindControl("hfdStatus1");
        //    if (hfdStatus.Value.Trim() == "1")
        //    {
        //        a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + commandArguments[1].ToString() + "'");
        //        string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been DeActivated', 'success');", true);
        //    }
        //    if (hfdStatus.Value.Trim() == "2")
        //    {
        //        a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + commandArguments[1].ToString() + "'");
        //        string empname = comm.GetNameFromId("AddEmployee", hdfEmpNameid);
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + empname + " Has Been Activated', 'success');", true);
        //    }
        //}
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

    public int check_cancelled_reseller_suscriber(int particularbusinessid)
    {
        int int_cancelled_reseller_suscriber = 0;
        DataTable check_reseller_email = new DataTable();
        DataTable Get_Reseller_Subscriber_Details = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "EmployeeID,MotherOrgID", "BusinessID = '" + particularbusinessid + "' and UserTypeId=1", "");
        if (Get_Reseller_Subscriber_Details.Rows.Count > 0)
        {
            check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "'", "");
        }
        if (check_reseller_email.Rows.Count > 0)
        {
            bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");

            if (!check_reseller_or_subscriber_email)
            {
                var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' ");
                if (check_old_subscriber.Rows.Count > 0)
                {
                    var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + "' and Is_Cancelled=1 ");

                    if (check_cancelled.Rows.Count > 0)
                    {
                        int_cancelled_reseller_suscriber++;
                    }
                }
            }
            if (check_reseller_or_subscriber_email)
            {
                var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' ");
                if (check_old_reseller.Rows.Count > 0)
                {
                    var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and Is_Cancelled=1 ");
                    if (check_cancelled.Rows.Count == 0)
                    {
                        DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "", "");
                        DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + " and Is_Requested_Plan=1", "");
                        if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                        {
                            int_cancelled_reseller_suscriber++;
                        }

                        DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=0 and " +
                            "EmpId='" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["MotherOrgID"].ToString()) + "' and MotherOrgID=" + int.Parse(Get_Reseller_Subscriber_Details.Rows[0]["EmployeeID"].ToString()) + " ");
                        if (dt_check_active_status.Rows.Count > 0)
                        {
                            int_cancelled_reseller_suscriber++;
                        }
                    }
                    if (check_cancelled.Rows.Count > 0)
                    {
                        int_cancelled_reseller_suscriber++;
                    }
                }
            }
        }

        return int_cancelled_reseller_suscriber;
    }

    protected void gvbusinessListForManager_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdfBusinessIDForManager = (e.Row.FindControl("hdfBusinessIDForManager") as HiddenField);
            HyperLink HyperLink_Schedular_Manager = (e.Row.FindControl("HyperLink_Schedular_Manager") as HyperLink);
            if (check_cancelled_reseller_suscriber(int.Parse(hdfBusinessIDForManager.Value)) > 0)
            {
                e.Row.Enabled = false;
            }

            string[] roles;
            DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hdfBusinessIDForManager.Value + "'", "UserTypeId");
            DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
            if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
            {
                roles = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                if (roles.Contains("31"))
                {
                    HyperLink_Schedular_Manager.Visible = true;
                }
            }
        }
    }

    protected void gvBusinessListEmp_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdfBusinessIDForManager = (e.Row.FindControl("hdfBusinessIDForManager") as HiddenField);
            HyperLink HyperLink_Schedular_Employee = (e.Row.FindControl("HyperLink_Schedular_Employee") as HyperLink);
            if (check_cancelled_reseller_suscriber(int.Parse(hdfBusinessIDForManager.Value)) > 0)
            {
                e.Row.Enabled = false;
            }

            string[] roles;
            DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hdfBusinessIDForManager.Value + "'", "UserTypeId");
            DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
            if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
            {
                roles = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                if (roles.Contains("31"))
                {
                    HyperLink_Schedular_Employee.Visible = true;
                }
            }
        }
    }

    protected void GridView_Accountant_User_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdfBusinessIDForManager__ = (e.Row.FindControl("hdfBusinessIDForManager__") as HiddenField);
            HyperLink HyperLink_Schedular_Accountant = (e.Row.FindControl("HyperLink_Schedular_Accountant") as HyperLink);
            if (check_cancelled_reseller_suscriber(int.Parse(hdfBusinessIDForManager__.Value)) > 0)
            {
                e.Row.Enabled = false;
            }

            string[] roles;
            DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hdfBusinessIDForManager__.Value + "'", "UserTypeId");
            DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
            if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
            {
                roles = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                if (roles.Contains("31"))
                {
                    HyperLink_Schedular_Accountant.Visible = true;
                }
            }
        }
    }

    protected void GridView_Custome_User_Role_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdfBusinessID1_Custome_User_Role = (e.Row.FindControl("hdfBusinessID1_Custome_User_Role") as HiddenField);
            HyperLink HyperLink_Schedular_Custom_User = (e.Row.FindControl("HyperLink_Schedular_Custom_User") as HyperLink);
            HyperLink hyAssManger1_Custome_User_Role = (e.Row.FindControl("hyAssManger1_Custome_User_Role") as HyperLink);
            if (check_cancelled_reseller_suscriber(int.Parse(hdfBusinessID1_Custome_User_Role.Value)) > 0)
            {
                e.Row.Enabled = false;
            }

            string[] roles;
            DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "EmployeeId", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + hdfBusinessID1_Custome_User_Role.Value + "'", "UserTypeId");
            DataTable Dt_Check_Custom_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "'", "");
            if (Dt_Check_Custom_Role_Permission.Rows.Count > 0)
            {
                roles = Dt_Check_Custom_Role_Permission.Rows[0][0].ToString().Split(',');
                if (roles.Contains("31"))
                {
                    HyperLink_Schedular_Custom_User.Visible = true;
                }
                if (roles.Contains("17"))
                {
                    hyAssManger1_Custome_User_Role.Visible = true;
                }
            }
        }
    }

    protected void ddl_Action_Status_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(hfdPopUpBusid.Value), int.Parse(Session["EmployeeId"].ToString()));
        DataView dv = new DataView(Business_Owner_Pop_up_Employee_List_);
        GvPopUpEmpList.DataSource = null;
        GvPopUpEmpList.DataBind();


        if (ddl_Action_Status.SelectedIndex == 1)
        {

            GvPopUpEmpList.DataSource = dv;//Business_Owner_Pop_up_Employee_List_;
            GvPopUpEmpList.DataBind();
            ModalPopupEmpList.Show();
            return;
        }
        string strQuery;
        if (ddl_Action_Status.SelectedIndex == 2)
        {
            strQuery = "Employee_Status='Active'";
            dv.RowFilter = strQuery;
            GvPopUpEmpList.DataSource = dv;
            GvPopUpEmpList.DataBind();
            ModalPopupEmpList.Show();
            return;
        }
        if (ddl_Action_Status.SelectedIndex == 3)
        {
          
            strQuery = "Employee_Status='Not Active'";
            dv.RowFilter = strQuery;
            //gvBusinessListEmp.Visible = false;
            GvPopUpEmpList.DataSource = dv;
            GvPopUpEmpList.DataBind();
            GvPopUpEmpList.Dispose();
            // gvBusinessListEmp.Visible = true;
             ModalPopupEmpList.Show();
            return;
        }

        return;

        /* GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
         GvPopUpEmpList.DataBind();

         ModalPopupEmpList.Show();

         if (ddl_Action_Status.SelectedValue == "")
         {
             GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
             GvPopUpEmpList.DataBind();
         }

         DataRow[] dr;
         DataTable dtr;
         if (ddl_Action_Status.SelectedValue == "0")
         {
             GvPopUpEmpList.DataSource = null;
             GvPopUpEmpList.DataBind();
             dr = null;
             dtr = null;
             dr = Business_Owner_Pop_up_Employee_List_.Select("[IsActive]='YES'");
             if (dr.Count() > 0)
             {
                 dtr = dr.CopyToDataTable();
                 GvPopUpEmpList.DataSource = dtr;
                 GvPopUpEmpList.DataBind();
             }
         }
         if (ddl_Action_Status.SelectedValue == "1")
         {
             GvPopUpEmpList.DataSource = null;
             GvPopUpEmpList.DataBind();
             dr = null;
             dtr = null;
             dr = Business_Owner_Pop_up_Employee_List_.Select("[IsActive]='NO'");
             if (dr.Count() > 0)
             {
                 dtr = dr.CopyToDataTable();
                 GvPopUpEmpList.DataSource = dtr;
                 GvPopUpEmpList.DataBind();
             }
         }*/
    }


    protected void txtTerminationDate_TextChanged(object sender, EventArgs e)
    {
        TextBox txtTerminationDate = (TextBox)sender;
        GridViewRow gvr = (GridViewRow)txtTerminationDate.NamingContainer;
        HiddenField hdf_EmployeeID_ = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdf_EmployeeID_");
        TextBox txtTerminationDate_ = (TextBox)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("txtTerminationDate");

        if (txtTerminationDate_.Text != "")
        {
            DateTime Min_Date = DateTime.Parse("01/01/1753");
            DateTime Selected_Date = DateTime.Parse(txtTerminationDate_.Text);

            if (Selected_Date < Min_Date)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Termination Date Correctly!', 'warning');", true);
                return;
            }

            var Get_RateId = comm.ExecuteQueryDataTable("select top 1 RateId, effectivedate, Is_Having_EndDate " +
                "from RateDetails with (Nolock) where EmployeeId=" + int.Parse(hdf_EmployeeID_.Value) + "  order by RateId desc");
            if (Get_RateId.Rows.Count > 0)
            {
                comm.ExecuteQuery("update RateDetails set EndDate='" + DateTime.Parse(txtTerminationDate_.Text) + "',Is_Having_EndDate=1 where RateId=" + Get_RateId.Rows[0]["RateId"].ToString() + " ");
                comm.ExecuteQuery("update [AddEmployee] set [TerminationDate]='" + DateTime.Parse(txtTerminationDate_.Text) + "' where EmployeeID=" + int.Parse(hdf_EmployeeID_.Value) + " ");
                ModalPopupEmpList.Show();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Termination Date Added Successfully!', 'success');", true);


                //RegisterPostBackControl();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

            }
        }
    }

    protected void LinkButton_NotActive_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameId");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Not Active";
        Label lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");
        lblStatus.Text = "Not Active";
        ModalPopupExtender1.Show();

        //  ModalPopupEmpList.Show();
    }
    protected void LinkButton_Active_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameId");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Active";
        Label lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");
        lblStatus.Text = "Active";
        ModalPopupExtender1.Show();


    }

    protected void LinkButton_Terminate_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameId");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Terminated";
        Label lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");
        lblStatus.Text = "Terminated";
        
        ModalPopupExtender1.Show();
       

    }

    protected void btnSaveEffectiveDate_Click(object sender, EventArgs e)
    {

        string strQuery;
        if (strEmpStatus == "Active")
        {

            strQuery = "Update AddEmployee set Employee_Status ='" + strEmpStatus + "', HireDate='" + TextEffectiveDate.Text + "' where EmployeeId=" + int.Parse(strEmpId);
            comm.ExecuteQuery(strQuery);
        }
        else
        {
            strQuery = "Update AddEmployee set Employee_Status ='" + strEmpStatus + "' where EmployeeId=" + int.Parse(strEmpId);
            comm.ExecuteQuery(strQuery);
        }
        strQuery = "INSERT INTO  Emp_Active_Inactive_Status values (" + int.Parse(strEmpId) +
                        ", '" + strEmpStatus + "' ,'" + TextEffectiveDate.Text + "', '" + DateTime.Now + "')";
        comm.ExecuteQuery(strQuery);
        DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(hfdPopUpBusid.Value), int.Parse(Session["EmployeeId"].ToString()));
        GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();
        ModalPopupExtender1.Hide();
       

    }
    protected void lnkBusOnwerEmpList_click(object sender, EventArgs e)
    {    /* string url = "EmpPopUpList.aspx";
    string s = "window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');";
    ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);*/
  
        ModalPopupEmpList.Show();

    }

    protected void GvPopUpEmpList_Init(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
    }
}