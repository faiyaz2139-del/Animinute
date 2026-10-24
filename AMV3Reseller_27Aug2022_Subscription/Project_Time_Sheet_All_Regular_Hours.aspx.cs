using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Project_Time_Sheet_All_Regular_Hours : System.Web.UI.Page
{
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    DataTable Dt_Check_Role_Permission = new DataTable(); //To_Check_The_Role_Assigned
    string user_type_id; // Checking user type id of the user
    string string_Check_Role_Permission; //to get the role assigned for user in array string
    string role_check_for_add_employee_message_throw; //to hide and unhide the add employee hyperlink
    //Page load event
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblsms.Text = Session["user"].ToString();

            #region To_Check_The_Role_Assigned definition //it is written in page load but showing any value in datatable in button click event
            user_type_id = Session["UserTypeid"].ToString();
            Dt_Check_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' and UserTypeId='" + user_type_id.ToString() + "'", "");
            #endregion

            if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
            {
                GetAllDetails(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");

                //DataTable dtEmptype = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5)", "");
                DataTable dtEmptype = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId not in (1)", "");
                if (dtEmptype.Rows.Count != 0)
                {
                    if (dtEmptype.Rows.Count > 1)
                    {
                        if (Request.QueryString["MangerId"] != null)
                        {
                            DataTable dt1 = Comm.GetAllDataFromTable("AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.EmpId", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                            if (dt1.Rows.Count > 0)
                            {
                                if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                                {
                                    hypProjectTime.Visible = false;
                                }
                                else
                                {
                                    hypProjectTime.Visible = true;
                                }
                            }
                        }
                        else
                        {
                            //string a = Session["EmployeeId"].ToString();
                            //DataTable dt1 = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5) and assb.EmployeeID='" + a + "'", "");
                            //if (dt1.Rows.Count > 0)
                            //{
                            //    if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                            //    {
                            //        hypProjectTime.Visible = false;
                            //    }
                            //    else
                            //    {
                            //        hypProjectTime.Visible = true;
                            //    }
                            //}
                            //else
                            {
                                //DataTable dtEmptype1 = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5)", "");
                                DataTable dtEmptype1 = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId not in (1)", "");
                                if (dtEmptype1.Rows.Count > 1)
                                {
                                    hypProjectTime.Visible = true;
                                }
                            }
                        }

                        //if (Request.QueryString["MangerId"] != null)
                        //{
                        //    DataTable dt1 = Comm.GetAllDataFromTable(" ad.EMPLOYEETYPE from AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.EmpId", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                        //    if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                        //    {
                        //        hypProjectTime.Visible = false;
                        //    }
                        //}
                        //    hypProjectTime.Visible = true;
                        hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                        // hyProjectTimeSheet.Visible = false;
                        // hyViewTimeSheet.Visible = false;
                    }
                }
            }

            #region For_Add_Employee_Hyperlink_According_To_Role definition
            //string user_type_id_for_role_permission = Session["UserTypeid"].ToString();
            //if (user_type_id_for_role_permission == "1")
            //{
            //    lnkAddEmp.Visible = true;
            //    return;
            //}

            ////To check the old user who are registered before the role development
            //DataTable Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' ", "");
            //if (Dt_Check_If_User_Role_Permission.Rows.Count == 0)
            //{
            //    lnkAddEmp.Visible = true;
            //    return;
            //}

            ////From manager gridview to hide and unhide add employee hyperlink
            //if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
            //{
            //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "UserTypeId");
            //    DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='2'", "");
            //    string string_Check_Manager_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            //    string[] splittedArray = string_Check_Manager_Role_Permission.Split(',');
            //    foreach (string role_check_for_add_employee_role in splittedArray)
            //    {
            //        if (role_check_for_add_employee_role == "6")
            //        {
            //            lnkAddEmp.Visible = true;
            //            return;
            //        }
            //        else
            //            role_check_for_add_employee_message_throw = role_check_for_add_employee_role;
            //    }
            //    if (role_check_for_add_employee_message_throw != "6")
            //    {
            //        lnkAddEmp.Visible = false;
            //        return;
            //    }
            //}

            ////From owner gridview to hide and unhide add employee hyperlink // also for the additional custyom roles
            //if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
            //{
            //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "UserTypeId");
            //    //DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");
            //    DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='" + Session["UserTypeid"].ToString() + "'", "");
            //    if (Dt_Check_Owner_Role_Permission.Rows.Count > 0)
            //    {
            //        string string_Check_Owner_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            //        string[] splittedArray2 = string_Check_Owner_Role_Permission.Split(',');
            //        foreach (string role_check_for_add_employee_role in splittedArray2)
            //        {
            //            if (role_check_for_add_employee_role == "14")
            //            {
            //                lnkAddEmp.Visible = true;
            //                return;
            //            }
            //            else
            //                role_check_for_add_employee_message_throw = role_check_for_add_employee_role;
            //        }
            //        if (role_check_for_add_employee_message_throw != "14")
            //        {
            //            lnkAddEmp.Visible = false;
            //            return;
            //        }
            //    }
            //    else
            //    {
            //        lnkAddEmp.Visible = false;
            //        return;
            //    }
            //}
            #endregion
        }
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
                //Commented because the new EmployeeType N is introduced for Accountant user type
                //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL)", "ad.EmployeeID");
                //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");
                //to show all project and all regular employees
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='P')", "ad.EmployeeID");
            }

        }
        else if (Session["UserTypeid"].ToString() == "2")
        {
            if (Request.QueryString["MangerId"] != null)
            {

                // Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "EmpId");

                DataTable dtMangerEmpList = new DataTable();
                DataTable dtMangerEmpListFilter = new DataTable();
                dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "R");
                DataRow[] dtFilter = dtMangerEmpList.Select("[IsActive]='YES'");
                if (dtFilter.Length > 0)
                {
                    dtMangerEmpListFilter = dtFilter.CopyToDataTable();
                }
                ddlemp.Items.Clear();
                ddlemp.Items.Add("Please Select");
                ddlemp.SelectedItem.Value = "";

                if (dtMangerEmpList.Rows.Count > 0)
                {
                    ddlemp.DataSource = dtMangerEmpListFilter;
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
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
                }
                else
                {
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
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
                //    Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "EmpId");
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
                    dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " AND AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");

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
                //Commented because the new EmployeeType N is introduced for Accountant user type
                //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and ad.EMPLOYEETYPE='R'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "





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
            else
            {
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
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
        if (txtmonth.Text != "")
        {
            int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            dtchild = TSC.GetTimeSheetEntryChildGv(y, m, weekNum, Bid, empid);

            if (dtchild.Rows.Count > 0)
            {
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();
            }
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Enter Reqular Hr First', 'error');", true);
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
        Abe.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        Abe.Month = DateTime.Parse(txtmonth.Text).Month;
        Abe.Year = DateTime.Parse(txtmonth.Text).Year;
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
        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(a).ToString("yyyy-MM-dd") + "'", "BreakID");
        ViewState["dt"] = dt;
        GvPopUpEmpList.DataSource = ViewState["dt"];
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();
        RegisterPostBackControl();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

        txtBreakStartTime.Text = "";
        txtBreakEndTime.Text = "";
    }

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {

    }

    protected void lnkAddEmp_Click(object sender, EventArgs e)
    {
        int bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //Response.Redirect("AddNewEmployee_TAB.aspx?BusinessIdFromDefaultPage=" + bid + "&EmpType=R", false); //Commented because of new page developed in role development for add employee
        //Response.Redirect("Add_Employee.aspx?BusinessIdFromDefaultPage=" + bid + "&EmpType=R", false); //Commented because now emp will be added and then assigned
        Response.Redirect("Add_EmployeeV2.aspx", false);
    }

    protected void ddlemp_SelectedIndexChanged(object sender, EventArgs e)
    {
        lblsms.Text = Session["user"].ToString();

        if ("Welcome! " + ddlemp.SelectedItem.Text == lblsms.Text)
        {
            ddlAction.Enabled = false;
            //lnkApproved.Enabled = false;

        }
        else
        {
            ddlAction.Enabled = true;
            //lnkApproved.Enabled = true;
        }
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select employee', 'error');", true);
            return;
        }
        DataTable dt = new DataTable();
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
        GetGridFill();
    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtmonth.Text);
        DateTime nxtdate = CurrentDate.AddMonths(-1);
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        txtmonth_TextChanged(sender, e);
    }

    protected void txtmonth_TextChanged(object sender, EventArgs e)
    {

        GetGridFill();


    }

    protected void btnnext_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtmonth.Text);
        DateTime nxtdate = CurrentDate.AddMonths(1);
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        txtmonth_TextChanged(sender, e);
    }

    string role_check_for_approve_and_reject_time_sheet_message_throw;
    //12-08
    //protected void lnkApproved_Click(object sender, EventArgs e)
    //{
    //    string user_type_id = Session["UserTypeid"].ToString();
    //    if (ddlemp.SelectedValue == "")
    //    {
    //        ScriptManager.RegisterClientScriptBlock(
    //                 this.Page,
    //                 this.GetType(),
    //                 "SweetAlert",
    //                 "swal('Error!', 'Please select employee', 'error');",
    //                 true);
    //        return;
    //    }

    //    if (user_type_id == "1")
    //    {
    //        ApproveTimesheet();
    //        return;
    //    }

    //    //For manager gridview Enter/View Time Sheet hyperlink
    //    if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
    //    {
    //        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
    //        DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

    //        //When users are old who are registered before the role permission development
    //        if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
    //        {
    //            ApproveTimesheet();
    //            return;
    //        }

    //        //When users are new who are registered after the role permission development
    //        var string_Check_Role_Permission = Dt_Check_Manager_Role_Permission
    //            .Rows[0]["Role_Permission_Id"].ToString();
    //        var splittedArray = string_Check_Role_Permission.Split(',');
    //        var roles = new string[] { "7", "8", "9" };

    //        if (splittedArray.Any(role => roles.Contains(role)))
    //        {
    //            ApproveTimesheet();
    //            return;
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock(
    //                this.Page,
    //                this.GetType(),
    //                "SweetAlert",
    //                "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');",
    //                true);
    //            return;
    //        }
    //    }

    //    //For owner gridview Enter/View Time Sheet hyperlink
    //    if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
    //    {
    //        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
    //        DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

    //        //When users are old who are registered before the role permission development
    //        if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
    //        {
    //            ApproveTimesheet();
    //            return;
    //        }

    //        //When users are new who are registered after the role permission development
    //        var string_Check_Role_Permission = Dt_Check_Owner_Role_Permission
    //            .Rows[0]["Role_Permission_Id"].ToString();
    //        var splittedArray = string_Check_Role_Permission.Split(',');
    //        var roles = new string[] { "18", "19", "20" };
    //        if (splittedArray.Any(role => roles.Contains(role)))
    //        {
    //            ApproveTimesheet();
    //            return;
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock(
    //                this.Page,
    //                this.GetType(),
    //                "SweetAlert",
    //                "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');",
    //                true);
    //            return;
    //        }
    //    }
    //}

    private int SaveTimesheet(GridViewRow row, bool isRejected = false)
    {
        var txtRegular = (TextBox)row.FindControl("txtRegular");
        var txtOver15x = (TextBox)row.FindControl("txtOver15x");
        var txtOver2x = (TextBox)row.FindControl("txtOver2x");
        var txtholiday = (TextBox)row.FindControl("txtholiday");
        var lblTotal = (Label)row.FindControl("lblTotal");
        var lblDate = (Label)row.FindControl("lblDate");
        var lblDay = (Label)row.FindControl("lblDay");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var txtBreakStartTime = (TextBox)row.FindControl("txtBreakStartTime");
        var txtBreakEndTime = (TextBox)row.FindControl("txtBreakEndTime");
        var ddstatus = (DropDownList)row.FindControl("ddstatus");
        var hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        var hfdStatus = (HiddenField)row.FindControl("hfdStatus");

        if (ddstatus.SelectedIndex == 1 && !isRejected)
        {
            absent(row.RowIndex);
            return -1;
        }

        var computedItems = CalculatedTimingHours(row);
        string Over15x = "0", Over2x = "0", holiday = "0", RegularStartTime = "0", RegularEndTime = "0";
        if (computedItems.Item4)
        {
            Over15x = txtOver15x.Text.Split(',')[0] == "" ? Over15x : txtOver15x.Text.Split(',')[0];
            Over2x = txtOver2x.Text.Split(',')[0] == "" ? Over2x : txtOver2x.Text.Split(',')[0];
            holiday = txtholiday.Text.Split(',')[0] == "" ? holiday : txtholiday.Text.Split(',')[0];
            RegularStartTime = txtRegularStartTime.Text.Split(',')[0] == ""
                ? RegularStartTime
                : txtRegularStartTime.Text.Split(',')[0];
            RegularEndTime = txtRegularEndTime.Text.Split(',')[0] == ""
                ? RegularEndTime
                : txtRegularEndTime.Text.Split(',')[0];
        }
        else
        {
            RegularStartTime = txtRegularStartTime.Text;
            RegularEndTime = txtRegularEndTime.Text;
        }

        var weekno = hfdweekno.Value.Split(',');
        var ToltalRegulahr = computedItems.Item2 - computedItems.Item1;
        var TotalHr = ToltalRegulahr - computedItems.Item3;

        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        Tsee.WeekNo = int.Parse(weekno[0]);
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        Tsee.Day = lblDay.Text;
        Tsee.Regular = ToltalRegulahr.ToString();
        Tsee.Over1_5x = Over15x;
        Tsee.Over2x = Over2x;
        Tsee.Holiday = holiday;
        Tsee.Total = TotalHr.ToString();
        Tsee.ReqularStart = DateTime.Parse(RegularStartTime.ToString());
        Tsee.Regularend = DateTime.Parse(RegularEndTime.ToString());
        Tsee.BreakStart = "";
        Tsee.BreakEnd = "";
        Tsee.Status = hfdStatus.Value == "2" ? "2" : "1";
        Tsee.Worked = ddstatus.SelectedIndex == 2 ? "Yes" : "No";
        Tsee.BreakTotal = computedItems.Item3.ToString();
        TSC.Sp_InsertTimeSheetEntry(Tsee);
        return 1;
    }

    private Tuple<DateTime, DateTime, TimeSpan, bool> CalculatedTimingHours(GridViewRow row)
    {
        var lblDate = (Label)row.FindControl("lblDate");
        var hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        var check = Comm.IfExistsInTable("TimeSheetEntry", "*",
            "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        var weekno = hfdweekno.Value.Split(',');
        string regularStartTime = "0", regularEndTime = "0";

        if (check == true)
        {
            regularStartTime = txtRegularStartTime.Text.Split(',')[0] == ""
                ? regularStartTime
                : txtRegularStartTime.Text.Split(',')[0];
            regularEndTime = txtRegularEndTime.Text.Split(',')[0] == ""
                ? regularEndTime
                : txtRegularEndTime.Text.Split(',')[0];
        }
        else
        {
            regularStartTime = txtRegularStartTime.Text;
            regularEndTime = txtRegularEndTime.Text;
        }

        var endDateTime = DateTime.Parse(regularEndTime);
        var startDateTime = DateTime.Parse(regularStartTime);

        var dtBreakTime = Comm.GetAllDataFromTable("BreakTime",
            "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" +
            ddlemp.SelectedValue + " and BussID=" +
            int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" +
            DateTime.Parse(txtmonth.Text).Month + " and Year=" +
            DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" +
            int.Parse(weekno[0]) + " and TimeSheetDate='" +
            DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        var totalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

        return new Tuple<DateTime, DateTime, TimeSpan, bool>(startDateTime, endDateTime, totalBreakHr, check);
    }

    private void ApproveTimesheet()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Approval failed status date wise:");
        int rowUpdateCount = 0;
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            var chkApproved = (CheckBox)row.FindControl("chkApproved");
            var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");

            if (chkApproved.Checked == true)
            {
                var validate = ValidateSelectedGridItems(row);
                if (!validate.Item1) { sb.Append("<br/>" + validate.Item2); }
                else
                {
                    var result = SaveTimesheet(row);
                    rowUpdateCount = result == -1 ? rowUpdateCount + 1 : rowUpdateCount;
                    if (result > 0)
                    {
                        var weekNo = (HiddenField)row.FindControl("hfdweekno");
                        var lblDate = (Label)row.FindControl("lblDate");
                        var parseDate = DateTime.Parse(lblDate.Text);

                        Comm.ExecuteQuery(String.Format(@"Update 
                                                    TimeSheetEntry 
                                                    set     Status=2 
                                                    where   WeekNo={0} 
                                                    and     EmpId='{1}' 
                                                    and     TimeSheetDate='{2}'",
                                                        weekNo.Value,
                                                        ddlemp.SelectedValue,
                                                        parseDate.Date.ToString("yyyy-MM-dd")
                                                        )
                                        );

                        rowUpdateCount++;
                    }
                }
            }
        }
        DisplayActionMessage(sb.ToString(), rowUpdateCount);
    }

    private Tuple<bool, string> ValidateSelectedGridItems(GridViewRow row)
    {
        StringBuilder sbInfo = new StringBuilder();
        var lblDate = (Label)row.FindControl("lblDate");
        var ddlStatus = (DropDownList)row.FindControl("ddstatus");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        sbInfo.Append("Date: " + lblDate.Text);
        if (ddlStatus.SelectedIndex == 0)
        {
            sbInfo.Append("<br/>- Status selection is required");
            return new Tuple<bool, string>(false, sbInfo.ToString());
        }
        if (ddlStatus.SelectedIndex == 1)
        {
            return new Tuple<bool, string>(true, "");
        }
        if (txtRegularStartTime.Text == "" ||
            txtRegularStartTime.Text == "," ||
            txtRegularEndTime.Text == "")
        {
            sbInfo.Append("<br/>- Start or End time should not be blank");
            return new Tuple<bool, string>(false, sbInfo.ToString());
        }
        else
        {
            var calculatedTiming = CalculatedTimingHours(row);
            if (calculatedTiming.Item2 < calculatedTiming.Item1)
            {
                sbInfo.Append("<br/>- End time should not be smaller than start time");
                return new Tuple<bool, string>(false, sbInfo.ToString());
            }

            if ((calculatedTiming.Item2 - calculatedTiming.Item1).TotalHours >= 24)
            {
                sbInfo.Append("<br/>- Start Time and End Time Must be within 24Hrs");
                return new Tuple<bool, string>(false, sbInfo.ToString());
            }

            if ((calculatedTiming.Item2 - calculatedTiming.Item1) < calculatedTiming.Item3)
            {
                sbInfo.Append("<br/>- Total break time should not be more than total working hour");

            }
            return new Tuple<bool, string>(true, "");
        }
    }
    //firoz 10
    private void DisplayActionMessage(string status, int updateCount)
    {
        if (updateCount > 0)
        {
            GetGridFill();
        }

        var statusMessage = status == "Approval failed status date wise:" ? "" : status;

        ScriptManager
            .RegisterClientScriptBlock(
                        this.Page,
                        this.GetType(),
                        "SweetAlert",
                        "swal({html: '<div>" + updateCount + " Time sheet got approved<br/>" + statusMessage + "</div>'});",
                    true);

        return;
    }

    //08-12

    //protected void lnkReject_Click(object sender, EventArgs e)
    //{
    //    string user_type_id = Session["UserTypeid"].ToString();
    //    if (user_type_id == "1")
    //    {
    //        if (ddlemp.SelectedValue == "")
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
    //            return;
    //        }
    //        int a = 0;
    //        foreach (GridViewRow row in GVChildlist.Rows)
    //        {
    //            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
    //            if (chkApproved.Checked == true)
    //            {
    //                HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
    //                Label lblDate = (Label)row.FindControl("lblDate");
    //                DateTime d = DateTime.Parse(lblDate.Text);

    //                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
    //            }
    //        }
    //        if (a > 0)
    //        {
    //            GetGridFill();
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
    //        }
    //        else
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
    //        }

    //        return;
    //    }

    //    //For manager gridview Enter/View Time Sheet hyperlink
    //    if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
    //    {
    //        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
    //        DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

    //        //When users are old who are registered before the role permission development
    //        if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
    //        {
    //            if (ddlemp.SelectedValue == "")
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
    //                return;
    //            }
    //            int a = 0;
    //            foreach (GridViewRow row in GVChildlist.Rows)
    //            {
    //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
    //                if (chkApproved.Checked == true)
    //                {
    //                    HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
    //                    Label lblDate = (Label)row.FindControl("lblDate");
    //                    DateTime d = DateTime.Parse(lblDate.Text);

    //                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
    //                }
    //            }
    //            if (a > 0)
    //            {
    //                GetGridFill();
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
    //            }
    //            else
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
    //            }
    //            return;
    //        }

    //        //When users are new who are registered after the role permission development
    //        string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
    //        string[] splittedArray = string_Check_Role_Permission.Split(',');
    //        foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
    //        {
    //            #region For_Reject_Time_Sheet definition
    //            if (role_check_for_approve_and_reject_time_sheet == "9")
    //            {
    //                if (ddlemp.SelectedValue == "")
    //                {
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
    //                    return;
    //                }
    //                int a = 0;
    //                foreach (GridViewRow row in GVChildlist.Rows)
    //                {
    //                    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
    //                    if (chkApproved.Checked == true)
    //                    {
    //                        HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
    //                        Label lblDate = (Label)row.FindControl("lblDate");
    //                        DateTime d = DateTime.Parse(lblDate.Text);

    //                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
    //                    }
    //                }
    //                if (a > 0)
    //                {
    //                    GetGridFill();
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
    //                }
    //                else
    //                {
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
    //                }
    //            }
    //            else
    //                role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
    //            #endregion
    //        }
    //        if (role_check_for_approve_and_reject_time_sheet_message_throw != "9")
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
    //            return;
    //        }
    //    }

    //    //For owner gridview Enter/View Time Sheet hyperlink
    //    if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
    //    {
    //        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
    //        DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

    //        //When users are old who are registered before the role permission development
    //        if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
    //        {
    //            if (ddlemp.SelectedValue == "")
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
    //                return;
    //            }
    //            int a = 0;
    //            foreach (GridViewRow row in GVChildlist.Rows)
    //            {
    //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
    //                if (chkApproved.Checked == true)
    //                {
    //                    HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
    //                    Label lblDate = (Label)row.FindControl("lblDate");
    //                    DateTime d = DateTime.Parse(lblDate.Text);

    //                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
    //                }
    //            }
    //            if (a > 0)
    //            {
    //                GetGridFill();
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
    //            }
    //            else
    //            {
    //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
    //            }
    //            return;
    //        }

    //        //When users are new who are registered after the role permission development
    //        string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
    //        string[] splittedArray = string_Check_Role_Permission.Split(',');
    //        foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
    //        {
    //            #region For_Reject_Time_Sheet definition
    //            if (role_check_for_approve_and_reject_time_sheet == "20")
    //            {
    //                if (ddlemp.SelectedValue == "")
    //                {
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
    //                    return;
    //                }
    //                int a = 0;
    //                foreach (GridViewRow row in GVChildlist.Rows)
    //                {
    //                    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
    //                    if (chkApproved.Checked == true)
    //                    {
    //                        HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
    //                        Label lblDate = (Label)row.FindControl("lblDate");
    //                        DateTime d = DateTime.Parse(lblDate.Text);

    //                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
    //                    }
    //                }
    //                if (a > 0)
    //                {
    //                    GetGridFill();
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
    //                }
    //                else
    //                {
    //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
    //                }
    //            }
    //            else
    //                role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
    //            #endregion
    //        }
    //        if (role_check_for_approve_and_reject_time_sheet_message_throw != "20")
    //        {
    //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
    //            return;
    //        }
    //    }
    //}

    string role_check_for_add_and_edit_time_sheet_message_throw;
    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //For the business owners
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
                //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

                if (ddstatus.SelectedIndex == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    // absent(row.RowIndex);
                    return;
                }
                if (ddstatus.SelectedIndex == 1)
                {
                    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    absent(row.RowIndex);
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
                if (ToltalRegulahr.TotalHours >= 24)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                    return;
                }
                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                if (ToltalRegulahr < TotalBreakHr)
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                    return;
                }
                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
                if (hfdStatus.Value == "2")
                {
                    Tsee.Status = "2";
                }
                else
                {
                    Tsee.Status = "1";
                }


                //if (chkworked.Checked == true)
                //{
                //    Tsee.Worked = "Yes";
                //}
                //else
                //{
                //    Tsee.Worked = "No";
                //}
                if (ddstatus.SelectedIndex == 2)
                {
                    Tsee.Worked = "Yes";
                }
                if (ddstatus.SelectedIndex == 1)
                {
                    Tsee.Worked = "No";
                }
                Tsee.BreakTotal = TotalBreakHr.ToString();
                TSC.Sp_InsertTimeSheetEntry(Tsee);

                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                GetGridFill();


            }

            if (e.CommandName == "Submit")//6070415004624658
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                if (ddstatus.SelectedIndex == 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    // absent(row.RowIndex);
                    return;
                }
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                //if (chkworked.Checked == false)
                //{
                //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                //    return;
                //}
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
                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                if (CheckSaved == true)
                {
                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
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
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                //if (chkworked.Checked == false)
                //{
                //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                //    return;
                //}
                if (ddstatus.SelectedIndex == 0)
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
                //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                ViewState["dt"] = dt;
                GvPopUpEmpList.DataSource = ViewState["dt"];
                GvPopUpEmpList.DataBind();
                //up2.Update();
                //GetGridFill();

                ModalPopupEmpList.Show();

                return;
            }

            return;
        }

        //For manager gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

            //When users are old who are registered before the role permission development
            if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
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
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                    HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        // absent(row.RowIndex);
                        return;
                    }
                    if (ddstatus.SelectedIndex == 1)
                    {
                        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        absent(row.RowIndex);
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
                    if (ToltalRegulahr.TotalHours >= 24)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                        return;
                    }
                    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                    if (ToltalRegulahr < TotalBreakHr)
                    {
                        GetGridFill();
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                        return;
                    }
                    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
                    if (hfdStatus.Value == "2")
                    {
                        Tsee.Status = "2";
                    }
                    else
                    {
                        Tsee.Status = "1";
                    }
                    if (ddstatus.SelectedIndex == 2)
                    {
                        Tsee.Worked = "Yes";
                    }
                    if (ddstatus.SelectedIndex == 1)
                    {
                        Tsee.Worked = "No";
                    }
                    Tsee.BreakTotal = TotalBreakHr.ToString();
                    TSC.Sp_InsertTimeSheetEntry(Tsee);

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                    GetGridFill();


                }
                if (e.CommandName == "Submit")//6070415004624658
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = GVChildlist.Rows[index];

                    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        // absent(row.RowIndex);
                        return;
                    }
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
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
                    bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    if (CheckSaved == true)
                    {
                        Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
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
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                    if (ddstatus.SelectedIndex == 0)
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
                    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                    ViewState["dt"] = dt;
                    GvPopUpEmpList.DataSource = ViewState["dt"];
                    GvPopUpEmpList.DataBind();
                    //up2.Update();
                    //GetGridFill();

                    ModalPopupEmpList.Show();
                }
                return;
            }

            //When users are new who are registered after the role permission development
            string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');

            foreach (string role_check_for_add_edit_time_sheet in splittedArray)
            {
                #region For_Enter_And_Edit_Time_Sheet definition
                if (role_check_for_add_edit_time_sheet == "7" || role_check_for_add_edit_time_sheet == "8")
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
                        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

                        if (ddstatus.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            // absent(row.RowIndex);
                            return;
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            absent(row.RowIndex);
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
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
                        if (hfdStatus.Value == "2")
                        {
                            Tsee.Status = "2";
                        }
                        else
                        {
                            Tsee.Status = "1";
                        }


                        //if (chkworked.Checked == true)
                        //{
                        //    Tsee.Worked = "Yes";
                        //}
                        //else
                        //{
                        //    Tsee.Worked = "No";
                        //}
                        if (ddstatus.SelectedIndex == 2)
                        {
                            Tsee.Worked = "Yes";
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")//6070415004624658
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        if (ddstatus.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            // absent(row.RowIndex);
                            return;
                        }
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        //if (chkworked.Checked == false)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
                        //    return;
                        //}
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
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
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
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                        // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                        //if (chkworked.Checked == false)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        //    return;
                        //}
                        if (ddstatus.SelectedIndex == 0)
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
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        //up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();

                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw != "7" || role_check_for_add_and_edit_time_sheet_message_throw != "8")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
        }

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

            //When users are old who are registered before the role permission development
            if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
            {
                string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                GridView GVChildlist = (GridView)sender;

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
                    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                    HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        return;
                    }
                    if (ddstatus.SelectedIndex == 1)
                    {
                        absent(row.RowIndex);
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
                    }
                    else
                    {
                        RegularStartTime[0] = txtRegularStartTime.Text;
                        RegularEndTime[0] = txtRegularEndTime.Text;
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
                    if (ToltalRegulahr.TotalHours >= 24)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                        return;
                    }
                    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                    if (ToltalRegulahr < TotalBreakHr)
                    {
                        GetGridFill();
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                        return;
                    }
                    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
                    if (hfdStatus.Value == "2")
                    {
                        Tsee.Status = "2";
                    }
                    else
                    {
                        Tsee.Status = "1";
                    }
                    if (ddstatus.SelectedIndex == 2)
                    {
                        Tsee.Worked = "Yes";
                    }
                    if (ddstatus.SelectedIndex == 1)
                    {
                        Tsee.Worked = "No";
                    }
                    Tsee.BreakTotal = TotalBreakHr.ToString();
                    TSC.Sp_InsertTimeSheetEntry(Tsee);

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                    GetGridFill();


                }
                if (e.CommandName == "Submit")//6070415004624658
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = GVChildlist.Rows[index];

                    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        return;
                    }
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
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
                    bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    if (CheckSaved == true)
                    {
                        Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
                        GetGridFill();
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                    }
                    else
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                    }
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
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        return;
                    }
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
                    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                    ViewState["dt"] = dt;
                    GvPopUpEmpList.DataSource = ViewState["dt"];
                    GvPopUpEmpList.DataBind();

                    ModalPopupEmpList.Show();
                }
                return;
            }

            //When users are new who are registered after the role permission development
            string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');

            foreach (string role_check_for_add_edit_time_sheet in splittedArray)
            {
                #region For_Enter_And_Edit_Time_Sheet definition
                if (role_check_for_add_edit_time_sheet == "18" || role_check_for_add_edit_time_sheet == "19")
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;

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
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

                        if (ddstatus.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            absent(row.RowIndex);
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
                        }
                        else
                        {
                            RegularStartTime[0] = txtRegularStartTime.Text;
                            RegularEndTime[0] = txtRegularEndTime.Text;
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
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
                            return;
                        }
                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
                        if (hfdStatus.Value == "2")
                        {
                            Tsee.Status = "2";
                        }
                        else
                        {
                            Tsee.Status = "1";
                        }
                        if (ddstatus.SelectedIndex == 2)
                        {
                            Tsee.Worked = "Yes";
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            Tsee.Worked = "No";
                        }
                        Tsee.BreakTotal = TotalBreakHr.ToString();
                        TSC.Sp_InsertTimeSheetEntry(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill();


                    }
                    if (e.CommandName == "Submit")//6070415004624658
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        if (ddstatus.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                            return;
                        }
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
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
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
                        }
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
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
                        if (ddstatus.SelectedIndex == 0)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
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
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw != "18" || role_check_for_add_and_edit_time_sheet_message_throw != "19")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
        }
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
            //CheckBox chkworked = e.Row.FindControl("chkworked") as CheckBox;
            CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;

            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            DropDownList ddstatus = e.Row.FindControl("ddstatus") as DropDownList;
            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";
                lnlGvAbsent.Enabled = false;
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                lnlGvAbsent.Enabled = false;
                e.Row.BackColor = System.Drawing.Color.LightGray;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                lnlGvAbsent.Enabled = false;
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
                //chkworked.Enabled = false;
                chkApproved.Enabled = false;
                ddstatus.Enabled = false;
                ddstatus.SelectedIndex = 1;
                //e.Row.ForeColor = System.Drawing.Color.White;
                //chkworked.Enabled = false;
            }

            if (hfdWorked.Value.Trim() == "Yes")
            {
                //chkworked.Checked = true;
                ddstatus.SelectedIndex = 2;
            }
            else
            {
                ddstatus.SelectedIndex = 1;
            }
            string regMINst = lblDate.Text + " 00:00";
            string regMAXst = lblDate.Text + " 23:59";
            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            string regMinEnd = lblDate.Text + " 00:00";
            string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));

        }
    }

    protected void GVChildlist_PreRender(object sender, EventArgs e)
    {
        if (GVChildlist.Rows.Count > 0)
        {
            //GVChildlist.UseAccessibleHeader = true;
            //GVChildlist.HeaderRow.TableSection = TableRowSection.TableHeader;
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
            currdate = DateTime.Parse(txtmonth.Text);
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




        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        txtmonth_TextChanged(sender, e);
    }

    protected void lnkweekNxt_Click(object sender, EventArgs e)
    {
        DateTime nxtdate;
        DateTime currdate = DateTime.Parse(txtmonth.Text);
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
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        txtmonth_TextChanged(sender, e);
    }

    protected void lnkGvApp_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        //CheckBox chkworked = (CheckBox)GVChildlist.Rows[gvr.RowIndex].FindControl("chkworked") as CheckBox;

        //if (chkworked.Checked == false)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
        //    return;
        //}
        DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
            return;
        }
        if (txtRegularStartTime.Text == string.Empty && txtRegularEndTime.Text == string.Empty)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Enter Start time and End Time', 'error');", true);
            return;
        }
        bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (chkdata == false)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
            return;
        }
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
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
            return;
        }
        //  CheckBox chkworked = (CheckBox)GVChildlist.Rows[gvr.RowIndex].FindControl("chkworked") as CheckBox;
        //if (chkworked.Checked == false)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
        //    return;
        //}
        if (txtRegularStartTime.Text == string.Empty && txtRegularEndTime.Text == string.Empty)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Enter Start time and End Time', 'error');", true);
            return;
        }
        bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (chkdata == false)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
            return;
        }

        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
        }
    }

    protected void lnlGvAbsent_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        Label lblDay = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        Tsee.WeekNo = int.Parse(weekno[0]);
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        Tsee.Day = lblDay.Text;
        Tsee.Regular = "";
        Tsee.Over1_5x = "";
        Tsee.Over2x = "";
        Tsee.Holiday = "";
        Tsee.Total = "";
        Tsee.ReqularStart = null;
        Tsee.Regularend = null;
        Tsee.BreakStart = "";
        Tsee.BreakEnd = "";

        Tsee.Worked = "No";
        Tsee.BreakTotal = "";
        DataTable dtcheck = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4", "");
        if (dtcheck.Rows.Count == 0)
        {
            Tsee.Status = "4";
            TSC.Sp_InsertTimeSheetEntry(Tsee);
        }
        else
        {

            //Tsee.Status = string.Empty;
            //TSC.Sp_InsertTimeSheetEntry(Tsee);
            Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4");
        }

        GetGridFill();
    }

    protected void absent(int RowIndex)
    {
        //LinkButton lnkdelete = (LinkButton)sender;
        //GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RowIndex].FindControl("lblDate");
        Label lblDay = (Label)GVChildlist.Rows[RowIndex].FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        Tsee.WeekNo = int.Parse(weekno[0]);
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        Tsee.Day = lblDay.Text;
        Tsee.Regular = "";
        Tsee.Over1_5x = "";
        Tsee.Over2x = "";
        Tsee.Holiday = "";
        Tsee.Total = "";
        Tsee.ReqularStart = null;
        Tsee.Regularend = null;
        Tsee.BreakStart = "";
        Tsee.BreakEnd = "";

        Tsee.Worked = "No";
        Tsee.BreakTotal = "";
        DataTable dtcheck = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4", "");
        if (dtcheck.Rows.Count == 0)
        {
            Tsee.Status = "4";
            TSC.Sp_InsertTimeSheetEntry(Tsee);
        }
        else
        {

            //Tsee.Status = string.Empty;
            //TSC.Sp_InsertTimeSheetEntry(Tsee);
            Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4");
        }

        GetGridFill();
    }

    protected void lnkEdit_Click(object sender, EventArgs e)
    {
        LinkButton lnkEdit = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkEdit.NamingContainer;
        HiddenField brkid = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("brkid");
        Label BreakStart = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("BreakStart");
        Label BreakEnd = (Label)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("BreakEnd");
        hfdBreakId.Value = brkid.Value;
        txtBreakStartTime.Text = BreakStart.Text;
        txtBreakEndTime.Text = BreakEnd.Text;

        //Comm.ExecuteQuery("delete from BreakTime where BreakID=" + brkid.Value + "");
        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time deleted', 'success');", true);
    }

    protected void GvPopUpEmpList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "DeleteRow")
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            HiddenField brkid = (HiddenField)GvPopUpEmpList.Rows[index].FindControl("brkid");
            Comm.ExecuteQuery("delete from BreakTime where BreakID=" + commandArguments[1].ToString() + "");
            //up2.Update();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time deleted', 'success');", true);
        }
        else if (e.CommandName == "EditRow")
        {
            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());

            Label BreakStart = (Label)GvPopUpEmpList.Rows[index].FindControl("BreakStart");
            Label BreakEnd = (Label)GvPopUpEmpList.Rows[index].FindControl("BreakEnd");
            hfdBreakId.Value = commandArguments[1].ToString();
            DataTable dt = Comm.GetAllDataFromTable("BreakTime", "CAST(BreakStart as time)as BreakStart,CAST(BreakEnd as time)as BreakEnd", "BreakID=" + hfdBreakId.Value + "", "");
            txtBreakStartTime.Text = dt.Rows[0]["BreakStart"].ToString();
            txtBreakEndTime.Text = dt.Rows[0]["BreakEnd"].ToString();
            //up2.Update();
        }
    }

    private void RegisterPostBackControl()
    {
        foreach (GridViewRow row in GvPopUpEmpList.Rows)
        {
            LinkButton lnkEdit = row.FindControl("lnkEdit") as LinkButton;
            ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkEdit);
        }
    }

    private void RegisterPostBackControl_NEw()
    {
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            ImageButton lnkEdit = row.FindControl("Worked") as ImageButton;
            ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkEdit);
        }
    }

    protected void lnkdelete_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        //int index = Convert.ToInt32(commandArguments[0].ToString());
        HiddenField brkid = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("brkid");
        Comm.ExecuteQuery("delete from BreakTime where BreakID=" + brkid.Value + "");
        //up2.Update();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time deleted', 'success');", true);
    }

    protected void ddstatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddstatus = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddstatus.NamingContainer;
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;

        if (ddstatus.SelectedIndex == 1)
        {
            txtRegularStartTime.Text = ",";
            txtRegularEndTime.Text = "";
        }
    }

    protected void ddapproval_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddapproval = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddapproval.NamingContainer;
        if (ddapproval.SelectedIndex == 1)

        {

            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Text);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            //CheckBox chkworked = (CheckBox)GVChildlist.Rows[gvr.RowIndex].FindControl("chkworked") as CheckBox;

            //if (chkworked.Checked == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
            //    return;
            //}
            DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
            if (ddstatus.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
                return;
            }
            if (txtRegularStartTime.Text == "00:00:00" && txtRegularEndTime.Text == "00:00:00")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Absent!', 'Already Marked Absent', 'error');", true);
                return;
            }
            if (txtRegularStartTime.Text == string.Empty && txtRegularEndTime.Text == string.Empty)
            {
                string msg = "For these " + lblDate.Text + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
                Label3.Text = msg;
                hfdRIndex.Value = gvr.RowIndex.ToString();
                ModalPopupConfirmSingal.Show();
                return;

            }
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }
            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            }

        }

        if (ddapproval.SelectedIndex == 2)
        {

            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Text);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
            if (ddstatus.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
                return;
            }
            //  CheckBox chkworked = (CheckBox)GVChildlist.Rows[gvr.RowIndex].FindControl("chkworked") as CheckBox;
            //if (chkworked.Checked == false)
            //{
            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes.', 'error');", true);
            //    return;
            //}

            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }

            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
            }
        }
    }

    protected void btnYes_Click(object sender, EventArgs e)
    {
        int a = 0;
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");

            if (chkApproved.Checked == true)
            {

                HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
                Label lblDate = (Label)row.FindControl("lblDate");
                DateTime d = DateTime.Parse(lblDate.Text);
                if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
                {
                    HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");

                    Label lblDay = (Label)row.FindControl("lblDay");
                    string[] weekno = hfdweekno.Value.Split(',');
                    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                    Tsee.WeekNo = int.Parse(weekno[0]);
                    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
                    Tsee.Day = lblDay.Text;
                    Tsee.Regular = "";
                    Tsee.Over1_5x = "";
                    Tsee.Over2x = "";
                    Tsee.Holiday = "";
                    Tsee.Total = "";
                    Tsee.ReqularStart = null;
                    Tsee.Regularend = null;
                    Tsee.BreakStart = "";
                    Tsee.BreakEnd = "";

                    Tsee.Worked = "No";
                    Tsee.BreakTotal = "";
                    Tsee.Status = "4";
                    TSC.Sp_InsertTimeSheetEntry(Tsee);
                    a = 1;
                }
                else
                {
                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                }


            }

        }

        if (a > 0)
        {
            GetGridFill();

            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        }
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        ModalPopupConfirm.Hide();
    }

    protected void lnkbtnYesSingal_Click(object sender, EventArgs e)
    {
        int RIndex = int.Parse(hfdRIndex.Value);
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkSave") as LinkButton;
        int a;

        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and TimeSheetDate=" + d.ToString("yyyy-MM-dd") + "");
        if (check == true)
        {
            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();

                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            }
        }
        else
        {

            HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");

            Label lblDay = (Label)GVChildlist.Rows[RIndex].FindControl("lblDay");
            string[] weekno = hfdweekno.Value.Split(',');
            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
            Tsee.WeekNo = int.Parse(weekno[0]);
            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
            Tsee.Day = lblDay.Text;
            Tsee.Regular = "";
            Tsee.Over1_5x = "";
            Tsee.Over2x = "";
            Tsee.Holiday = "";
            Tsee.Total = "";
            Tsee.ReqularStart = null;
            Tsee.Regularend = null;
            Tsee.BreakStart = "";
            Tsee.BreakEnd = "";

            Tsee.Worked = "No";
            Tsee.BreakTotal = "";
            Tsee.Status = "4";
            TSC.Sp_InsertTimeSheetEntry(Tsee);
            a = 1;
            if (a > 0)
            {
                GetGridFill();

                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            }
        }
    }

    protected void lnkbtnNoSingal_Click(object sender, EventArgs e)
    {
        ModalPopupConfirmSingal.Hide();
    }



    protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlAction.SelectedValue == "A")
        {
            approvedTimeSheet();
        }
        else if (ddlAction.SelectedValue == "R")
        {
            RejectTimeSheet();
            //lnkReject_Click(sender, e);
        }

        ddlAction.SelectedValue = "";
    }
    protected void lnkApproved_Click(object sender, EventArgs e)
    {
    }

    //12-08
    public void approvedTimeSheet()
    {
        string user_type_id = Session["UserTypeid"].ToString();
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(
                     this.Page,
                     this.GetType(),
                     "SweetAlert",
                     "swal('Error!', 'Please select employee', 'error');",
                     true);
            return;
        }
        if (user_type_id == "1")
        {
            ApproveTimesheet();
            return;
        }
        //For manager gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

            //When users are old who are registered before the role permission development
            if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
            {
                ApproveTimesheet();
                return;
            }

            //When users are new who are registered after the role permission development
            var string_Check_Role_Permission = Dt_Check_Manager_Role_Permission
                .Rows[0]["Role_Permission_Id"].ToString();
            var splittedArray = string_Check_Role_Permission.Split(',');
            var roles = new string[] { "7", "8", "9" };

            if (splittedArray.Any(role => roles.Contains(role)))
            {
                ApproveTimesheet();
                return;
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(
                    this.Page,
                    this.GetType(),
                    "SweetAlert",
                    "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');",
                    true);
                return;
            }
        }

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

            if (Dt_Check_User_Role_In_All_Business.Rows[0]["UserTypeId"].ToString() == "5")
            {
                //When users are old who are registered before the role permission development
                if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
                {
                    ApproveTimesheet();
                    return;
                }

                //When users are new who are registered after the role permission development
                var string_Check_Role_Permission = Dt_Check_Owner_Role_Permission
                    .Rows[0]["Role_Permission_Id"].ToString();
                var splittedArray = string_Check_Role_Permission.Split(',');
                var roles = new string[] { "18", "19", "20" };
                if (splittedArray.Any(role => roles.Contains(role)))
                {
                    ApproveTimesheet();
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(
                        this.Page,
                        this.GetType(),
                        "SweetAlert",
                        "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');",
                        true);
                    return;
                }
                return;
            }


            DataTable Dt_Check_Customer_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId not in (1,2,4,5,12)", "");
            //When users are old who are registered before the role permission development
            if (Dt_Check_Customer_Role_Permission.Rows.Count == 0)
            {
                ApproveTimesheet();
                return;
            }
            else
            {
                //When users are new who are registered after the role permission development
                var string_Check_Role_Permission = Dt_Check_Customer_Role_Permission
                    .Rows[0]["Role_Permission_Id"].ToString();
                var splittedArray = string_Check_Role_Permission.Split(',');
                var roles = new string[] { "7", "8", "9", "18", "19", "20" };
                if (splittedArray.Any(role => roles.Contains(role)))
                {
                    ApproveTimesheet();
                    return;
                }
                else
                {
                    ScriptManager.RegisterClientScriptBlock(
                        this.Page,
                        this.GetType(),
                        "SweetAlert",
                        "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');",
                        true);
                    return;
                }
            }
        }
    }

    //12-08

    public void RejectTimeSheet()
    {
        string user_type_id = Session["UserTypeid"].ToString();
        if (user_type_id == "1")
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
                    var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
                    var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");

                    //Firoz 27-12
                    int saveFlag = 0;
                    if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
                        saveFlag = SaveDefaultValueInTimeSheet(row);
                    else
                        saveFlag = SaveTimesheet(row, isRejected: true);

                    if (saveFlag > 0)
                        Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");

                    a++;
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

            return;
        }

        //For manager gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

            //When users are old who are registered before the role permission development
            if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
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

                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                return;
            }

            //When users are new who are registered after the role permission development
            string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
            {
                #region For_Reject_Time_Sheet definition
                if (role_check_for_approve_and_reject_time_sheet == "9")
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

                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                else
                    role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
                #endregion
            }
            if (role_check_for_approve_and_reject_time_sheet_message_throw != "9")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
                return;
            }
        }

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

            if (Dt_Check_User_Role_In_All_Business.Rows[0]["UserTypeId"].ToString() == "5")
            {
                //When users are old who are registered before the role permission development
                if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
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

                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                    return;
                }

                //When users are new who are registered after the role permission development
                string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
                string[] splittedArray = string_Check_Role_Permission.Split(',');
                foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
                {
                    #region For_Reject_Time_Sheet definition
                    if (role_check_for_approve_and_reject_time_sheet == "20")
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

                                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                    else
                        role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
                    #endregion
                }
                if (role_check_for_approve_and_reject_time_sheet_message_throw != "20")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
                    return;
                }
            }


            DataTable Dt_Check_Customer_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId not in (1,2,4,5,12)", "");
            //When users are old who are registered before the role permission development
            if (Dt_Check_Customer_Role_Permission.Rows.Count == 0)
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

                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                return;
            }
            else
            {
                //When users are new who are registered after the role permission development
                string string_Check_Role_Permission = Dt_Check_Customer_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
                string[] splittedArray = string_Check_Role_Permission.Split(',');
                foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
                {
                    #region For_Reject_Time_Sheet definition
                    if (role_check_for_approve_and_reject_time_sheet == "7" || role_check_for_approve_and_reject_time_sheet == "8" || role_check_for_approve_and_reject_time_sheet == "9"
                        || role_check_for_approve_and_reject_time_sheet == "18" || role_check_for_approve_and_reject_time_sheet == "19" || role_check_for_approve_and_reject_time_sheet == "20")
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

                                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
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
                    else
                        role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
                    #endregion
                }
                if (role_check_for_approve_and_reject_time_sheet_message_throw != "7" && role_check_for_approve_and_reject_time_sheet_message_throw != "8" && role_check_for_approve_and_reject_time_sheet_message_throw != "9"
                    && role_check_for_approve_and_reject_time_sheet_message_throw != "18" && role_check_for_approve_and_reject_time_sheet_message_throw != "19" && role_check_for_approve_and_reject_time_sheet_message_throw != "20")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
                    return;
                }
            }
        }
    }

    protected void lnkReject_Click(object sender, EventArgs e)
    {
    }

    //Firoz 27-12
    private int SaveDefaultValueInTimeSheet(GridViewRow row)
    {
        HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        Label lblDate = (Label)row.FindControl("lblDate");
        Label lblDay = (Label)row.FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        Tsee.WeekNo = int.Parse(weekno[0]);
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        Tsee.Day = lblDay.Text;
        Tsee.Regular = "";
        Tsee.Over1_5x = "";
        Tsee.Over2x = "";
        Tsee.Holiday = "";
        Tsee.Total = "";
        Tsee.ReqularStart = null;
        Tsee.Regularend = null;
        Tsee.BreakStart = "";
        Tsee.BreakEnd = "";

        Tsee.Worked = "No";
        Tsee.BreakTotal = "";
        Tsee.Status = "4";
        TSC.Sp_InsertTimeSheetEntry(Tsee);
        return 1;
    }
}