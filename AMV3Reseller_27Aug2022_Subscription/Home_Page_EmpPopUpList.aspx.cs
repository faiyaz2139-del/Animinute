using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Home_Page_EmpPopUpList : System.Web.UI.Page

{
    public Label lblStatus;
    public bool boolStatusUpdated = false;
    private string strEmpStatus
    {
        get { return Convert.ToString(ViewState["DirectoryStatus"]); }
        set { ViewState["DirectoryStatus"] = value; }
    }
    private string strEmpId
    {
        get { return Convert.ToString(ViewState["DirectoryEmployee"]); }
        set { ViewState["DirectoryEmployee"] = value; }
    }
    SqlConnection con;
    static CommanClass comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    AddProject AddProject = new AddProject();
    int mangercheck = 0;
    int customroleusercheck = 0;
    int intBusinessID = 0;

    protected override void OnInit(EventArgs e)
    {
        ViewStateUserKey = Session.SessionID;
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        rvEffectiveDate.MaximumValue = System.DateTime.Now.AddDays(365).ToShortDateString();
        rvEffectiveDate.MinimumValue = System.DateTime.Now.AddDays(-365).ToShortDateString();

        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        var businesses = AnyminuteDirectoryScope.Businesses();
        string requested = Request.QueryString["BusinessIdFromDefaultPage"];
        if (String.IsNullOrEmpty(requested)) requested = Request.QueryString["BusinessId"];
        if (String.IsNullOrEmpty(requested)) requested = Convert.ToString(ViewState["DirectoryBusiness"]);
        if (String.IsNullOrEmpty(requested)) requested = Convert.ToString(Session["BusinessID"]);
        if (String.IsNullOrEmpty(requested) && businesses.Rows.Count == 1)
            requested = Convert.ToString(businesses.Rows[0]["BusinessID"]);
        if (String.IsNullOrEmpty(requested))
        {
            Response.Redirect("Users.aspx");
            Context.ApplicationInstance.CompleteRequest();
            return;
        }
        if (!Int32.TryParse(requested, out intBusinessID) || intBusinessID <= 0 ||
            businesses.Select("BusinessID=" + intBusinessID).Length != 1)
            throw new System.Web.HttpException(403, "This business is outside your access.");
        ViewState["DirectoryBusiness"] = intBusinessID;
        hfdPopUpBusid.Value = intBusinessID.ToString();
        // Binding on every Page_Load replaced the submitted row controls before handlers ran.
        if (!IsPostBack) GetEmpList();
    }

    public void GetEmpList()
    {
        DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(intBusinessID, int.Parse(Session["EmployeeId"].ToString()));
        //DataView dv = new DataView(Business_Owner_Pop_up_Employee_List_);
        GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
        GvPopUpEmpList.DataBind();
    }

    public void GetEmplist1()
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
                    //  dividBusinessOwner.Attributes.Add("style", "display:block");
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
                        /* GvEmpListForBusOwner.DataSource = null;
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
                     GvEmpListForBusOwner.DataBind();*/

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
                                    //GvEmpListForBusOwner.Enabled = false;
                                    //Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
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
                                        //  GvEmpListForBusOwner.Enabled = false;
                                        // Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                                    }

                                    DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=0 and " +
                                        "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                                    if (dt_check_active_status.Rows.Count > 0)
                                    {
                                        // GvEmpListForBusOwner.Enabled = false;
                                        // Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                                    }
                                }
                                if (check_cancelled.Rows.Count > 0)
                                {
                                    //GvEmpListForBusOwner.Enabled = false;
                                    //Label_Subscriber_Cancelled_Message.Text = "The Plan has been Cancelled. Please Contact Admin.";
                                }
                            }
                        }
                    }
                    else if (row["UserTypeId"].ToString() == "4")
                    {
                        // dividEmp.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            //   dividEmp.Attributes.Add("style", "display:block;");
                        }

                        empid = row["EmployeeID"].ToString() + "," + empid;
                        if (empid.Length > 0)
                        {
                            // Label_Employee_Grid.Visible = false;
                            // comm.FillGridView(gvBusinessListEmp, "AddEmployee Ae with (nolock) join AssginBussiness Assb with (nolock) on ae.EmployeeID=Assb.EmployeeID join EmpAddress ea with (nolock) on ea.EmpAddressId=ae.ADDRESSID and ea.MainEmail<>''", "ae.EmployeeID,assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID)BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName,(select PrimaryContactNumber from AddBusiness where BusinessID=Assb.BusinessID)PrimaryContactNumber,(select OperatingName from AddBusiness where BusinessID=Assb.BusinessID)OperatingName", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp,AE.EMPLOYEETYPE", "ae.EmployeeID in (" + empid + ")  and assb.UserTypeId=" + row["UserTypeid"].ToString() + " and ae.IsActive='YES'  AND assb.BusinessID  IN (SELECT  BusinessID from AddBusiness where AddBusiness.Status=1 ) ", "ae.EmployeeID");

                            //commented after creating issue in parmeswari login issue to not see option to enter timesheet.
                            //DataTable Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission with (Nolock)", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' ", "");
                            ////Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
                            //if (Dt_Check_Role_Permission.Rows.Count == 0)
                            //    this.gvBusinessListEmp.Columns[6].Visible = false;
                        }
                    }
                    else if (row["UserTypeId"].ToString() == "2")
                    {
                        // dividManager.Attributes.Add("style", "display:block;");
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

                        /* Label_Manager_Grid.Visible = false;
                         gvbusinessListForManager.DataSource = dtManagerBussniess;
                         gvbusinessListForManager.DataBind();*/
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
                            //Owner.Attributes.Add("style", "display:block");
                            //Label_Owner_Grid.Visible = false;
                            // GvEmpListOwner.DataSource = dtownr;
                            // GvEmpListOwner.DataBind();
                        }
                    }
                    else if (row["UserTypeId"].ToString() == "12")
                    {
                        // div_Accountant_User.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            //   div_Accountant_User.Attributes.Add("style", "display:block;");
                        }

                        empid = row["EmployeeID"].ToString() + "," + empid;
                        if (empid.Length > 0)
                        {
                            // Label_Accountant_Grid.Visible = false;
                            //comm.FillGridView(GridView_Accountant_User, "AddEmployee Ae with (Nolock) join AssginBussiness Assb with (Nolock) on ae.EmployeeID=Assb.EmployeeID join EmpAddress ea with (Nolock) on ea.EmpAddressId=ae.ADDRESSID and ea.MainEmail<>''", "ae.EmployeeID,assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID)BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName,(select PrimaryContactNumber from AddBusiness where BusinessID=Assb.BusinessID)PrimaryContactNumber,(select OperatingName from AddBusiness where BusinessID=Assb.BusinessID)OperatingName", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp,AE.EMPLOYEETYPE", "ae.EmployeeID in (" + empid + ")  and assb.UserTypeId=" + row["UserTypeid"].ToString() + " and ae.IsActive='YES'  AND assb.BusinessID  IN (SELECT  BusinessID from AddBusiness where AddBusiness.Status=1 ) ", "ae.EmployeeID");
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
                            /* div_Custome_User_Role.Attributes.Add("style", "display:block");
                             Label_Custome_User_Role_Grid.Visible = false;
                             GridView_Custome_User_Role.DataSource = dtcustom_user_role;
                             GridView_Custome_User_Role.DataBind();*/
                        }
                    }
                    #endregion
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

    public DataTable Additional_Role_Gridview_Binding(string Email, int MotherOrgId, string employee_type, int UserTypeId, int EmployeeID)
    {
        try
        {


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

            // loginfo = "Additional_Role_Gridview_Binding(" + Email + "," + MotherOrgId + ", " + employee_type + ", " + UserTypeId + ", " + EmployeeID + ") end";
            // Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            // Logger.Error("Additional_Role_Gridview_Binding" + ex.Message);
            throw ex;
        }
    }
    protected void ddl_Action_Status_SelectedIndexChanged(object sender, EventArgs e)
    {
        // DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(int.Parse(hfdPopUpBusid.Value), int.Parse(Session["EmployeeId"].ToString()));

        DataTable Business_Owner_Pop_up_Employee_List_ = Business_Owner_Pop_up_Employee_List(intBusinessID, int.Parse(Session["EmployeeId"].ToString()));
        DataView dv = new DataView(Business_Owner_Pop_up_Employee_List_);
        GvPopUpEmpList.DataSource = null;
        GvPopUpEmpList.DataBind();


        if (ddl_Action_Status.SelectedIndex == 1)
        {

            GvPopUpEmpList.DataSource = dv;//Business_Owner_Pop_up_Employee_List_;
            GvPopUpEmpList.DataBind();
            //  ModalPopupEmpList.Show();
            return;
        }
        string strQuery;
        if (ddl_Action_Status.SelectedIndex == 2)
        {
            strQuery = "Employee_Status='Active'";
            dv.RowFilter = strQuery;
            GvPopUpEmpList.DataSource = dv;
            GvPopUpEmpList.DataBind();
            // ModalPopupEmpList.Show()hdn;
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
            // ModalPopupEmpList.Show();
            return;
        }
        if (ddl_Action_Status.SelectedIndex == 4)
        {

            strQuery = "Employee_Status='Terminated'";
            dv.RowFilter = strQuery;
            //gvBusinessListEmp.Visible = false;
            GvPopUpEmpList.DataSource = dv;
            GvPopUpEmpList.DataBind();
            GvPopUpEmpList.Dispose();
            // gvBusinessListEmp.Visible = true;
            // ModalPopupEmpList.Show();
            return;
        }
        return;

        /* GvPopUpEmpList.DataSource = Business_Owner_Pop_up_Employee_List_;
         GvPopUpEmpList.DataBind();

         //ModalPopupEmpList.Show();

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
                    if (hyeditEmp != null) hyeditEmp.Visible = false;
                    hyeditEmprate.Visible = false;
                    togBtnForBusOwner.Enabled = false;
                }

                if (dtGv.Rows[0]["MainEmail"].ToString() == dtlogin.Rows[0]["MainEmail"].ToString() && customroleusercheck > 0)
                {
                    if (hyeditEmp != null) hyeditEmp.Visible = false;
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
                        if (hyeditEmp != null) hyeditEmp.Visible = true;
                    }
                    else
                        if (hyeditEmp != null) hyeditEmp.Visible = false;
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
                ///hyeditEmp.Enabled = false;
                // hyeditEmprate.Enabled = false;
                // hyEnterTimeSheet.Enabled = false;
                // hyEnterTimeSheet.Enabled = false;
            }

            #region to disabble edit and activate/deactivate the owner when owner log in
            if (Session["UserTypeid"].ToString() != "1")
            {
                DataTable Get_Owner_EmployeeID_UserType_In_Grid = comm.GetAllDataFromTable("AddEmployee with (Nolock)", "UserType", "EmployeeID='" + int.Parse(hdfEmpNameid.Value) + "'", "");
                if (Get_Owner_EmployeeID_UserType_In_Grid.Rows[0]["UserType"].ToString() == "5")
                {
                    if (hyeditEmp != null) hyeditEmp.Visible = false;
                    hyeditEmprate.Visible = false;
                    togBtnForBusOwner.Enabled = false;
                }
            }
            #endregion
        }
    }

    public DataTable Business_Owner_Pop_up_Employee_List(int BusinessId, int EmployeeId)
    {
        return AnyminuteDirectoryScope.Users(BusinessId);
    }

    protected void GvPopUpEmpList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvPopUpEmpList.PageIndex = e.NewPageIndex;
        GetEmpList();
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


    protected void LinkButton_NotActive_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameid");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Not Active";
        lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");
        //lblStatus.Text = "Not Active";
        ModalPopupExtender1.Show();

        //  ModalPopupEmpList.Show();
    }
    protected void LinkButton_Active_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameid");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Active";
        lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");
        // lblStatus.Text = "Active";
        ModalPopupExtender1.Show();


    }

    protected void LinkButton_Terminate_Click(object sender, EventArgs e)
    {
        LinkButton lnkNotActive = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkNotActive.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameid");
        strEmpId = hdnField.Value.ToString();
        strEmpStatus = "Terminated";
        lblStatus = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("lblStatus");


        // lblStatus.Text = "Terminated";

        ModalPopupExtender1.Show();


    }

    private bool validateEffectiveDate()
    {

        DateTime parsedDate;

        if (TextEffectiveDate.Text == "")
        {
            return false;
        }
        else if (DateTime.TryParse(TextEffectiveDate.Text, out parsedDate) == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /* protected void rvEffectiveDate_Init(object sender, EventArgs e)
     {
         rvEffectiveDate.MaximumValue = System.DateTime.Now.AddDays(365).ToShortDateString();
         rvEffectiveDate.MinimumValue = System.DateTime.Now.AddDays(-365).ToShortDateString();
         // Do any related intialization work.
     }*/
    // [System.Web.Services.WebMethod]
    public void btnSaveEffectiveDate_Click(object sender, EventArgs e)
    // public  void btnSaveEffectiveDate_Click(string dtEffectiveDate)
    {
        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter valid EffectiveDate', 'error');", true);
        updateEffectiveDate(TextEffectiveDate.Text);

    }
    private void updateEffectiveDate(string dtEffectuveDate)
    {
        try
        {
            DateTime effective;
            if (!DateTime.TryParse(dtEffectuveDate, out effective) || effective < new DateTime(1753, 1, 1))
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter a valid Effective Date', 'error');", true);
                return;
            }

            int businessId = intBusinessID;
            if (businessId <= 0 && Session["BusinessID"] != null)
                int.TryParse(Session["BusinessID"].ToString(), out businessId);
            if (businessId <= 0)
                throw new InvalidOperationException("Business context is missing. Refresh the employee list and try again.");

            AnyminuteEmployeeRepair.UpdateStatus(
                Convert.ToInt32(Session["EmployeeId"]),
                Convert.ToInt32(Session["UserTypeid"]),
                businessId,
                int.Parse(strEmpId),
                strEmpStatus,
                effective);

            DataTable employees = Business_Owner_Pop_up_Employee_List(businessId, int.Parse(Session["EmployeeId"].ToString()));
            GvPopUpEmpList.DataSource = employees;
            GvPopUpEmpList.DataBind();
            ModalPopupExtender1.Hide();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Employee status history saved successfully.', 'success');", true);
        }
        catch (InvalidOperationException ex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + System.Web.HttpUtility.JavaScriptStringEncode(ex.Message) + "', 'error');", true);
        }
        catch (Exception ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Employee status history update failed", ex);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Unable to save employee status history. Please contact support.', 'error');", true);
        }
    }
    protected void lnkBusOnwerEmpList_click(object sender, EventArgs e)
    {    /* string url = "EmpPopUpList.aspx";
    string s = "window.open('" + url + "', 'popup_window', 'width=300,height=100,left=100,top=100,resizable=yes');";
    ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);*/

        // ModalPopupEmpList.Show();

    }

    protected void GvPopUpEmpList_Init(object sender, EventArgs e)
    {
        Response.CacheControl = "no-cache";
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
                // ModalPopupEmpList.Show();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Termination Date Added Successfully!', 'success');", true);


                //RegisterPostBackControl();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

            }
        }
    }

    protected void lnbUpdateProfile_Click(object sender, EventArgs e)
    {
        LinkButton lnkUpdProfile = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkUpdProfile.NamingContainer;
        HiddenField hdnField = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("hdfEmpNameid");
        strEmpId = hdnField.Value.ToString();

        //Session["EmployeeId"] =strEmpId;
        //Session["BusinessId"] = intBusinessID;

        Response.Redirect("~/AddEditRole_Add_Employee.aspx?EmployeeIDEdit=" + strEmpId + "&BussEditId=" + intBusinessID.ToString());
    }
}
