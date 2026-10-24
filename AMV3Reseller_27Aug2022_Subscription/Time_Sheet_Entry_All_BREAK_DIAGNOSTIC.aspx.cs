using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Time_Sheet_Entry_All : System.Web.UI.Page
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
        if (!IsPostBack) { 
       // lblLocked.Visible = false;
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            //lblLocked.Visible = false;
            //Get_Business_Name_List();
            DataTable Get_Business_Name_In_Ddl = Comm.GetAllDataFromTable("AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID,(ab.CompanyName)BusinessName", "EmployeeID=" + Session["EmployeeId"].ToString() + " and ab.Status=1", "ab.BusinessID");
            Ddl_Business.Items.Add("Please Select");
            Ddl_Business.SelectedItem.Value = "";
            Ddl_Business.DataSource = Get_Business_Name_In_Ddl;
            Ddl_Business.DataValueField = Get_Business_Name_In_Ddl.Columns[0].ToString();
            Ddl_Business.DataTextField = Get_Business_Name_In_Ddl.Columns[1].ToString();
            Ddl_Business.DataBind();

            Ddl_Business.SelectedValue = Request.QueryString["BusinessIdFromDefaultPage"].ToString();
            //lblbid.Text = "Business Name - " + Comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business.SelectedValue));
            lblbid.Text = "  ";
            HiddenField_confirm_pop_up_count.Value = "0";

            lblsms.Text = Session["user"].ToString();

            #region To_Check_The_Role_Assigned definition //it is written in page load but showing any value in datatable in button click event
            user_type_id = Session["UserTypeid"].ToString();
            Dt_Check_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' and UserTypeId='" + user_type_id.ToString() + "'", "");
            #endregion

            if (Request.QueryString["CustomRole"] != null)
            {
                //commented by kumar
                DataTable dt1 = Comm.GetAllDataFromTable("AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.EmpId", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                //DataTable dt1 = Comm.GetAllDataFromTable("AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.ManagerID", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                if (dt1.Rows.Count > 0)
                {
                    if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                    {
                        hypProjectTime.Visible = false;
                    }
                    DataTable dt_checmgremptype = Comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                    if (dt_checmgremptype.Rows.Count > 0)
                    {
                        if (dt_checmgremptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P")
                        {
                            hypProjectTime.Visible = true;
                            hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString() + "&CustomRole=YES";
                            Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                        }
                    }
                    else
                    {
                        hypProjectTime.Visible = true;
                        hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString() + "&CustomRole=YES";
                        Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                    }
                }
                else
                {
                    DataTable dt2 = Comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                    if (dt2.Rows.Count > 0)
                    {

                        if (dt2.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                        {
                            hypProjectTime.Visible = false;
                        }
                        else
                        {
                            hypProjectTime.Visible = true;
                            hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString() + "&CustomRole=YES";
                            Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                        }
                    }
                }

                GetAllDetails(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
            }
            else
            {
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
                                    DataTable dt_checmgremptype = Comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                                    if (dt_checmgremptype.Rows.Count > 0)
                                    {
                                        if (dt_checmgremptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P")
                                        {
                                            hypProjectTime.Visible = true;
                                            hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();
                                            Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                                        }
                                    }
                                    else
                                    {
                                        hypProjectTime.Visible = true;
                                        hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();
                                        Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                                    }
                                }
                                else
                                {
                                    DataTable dt2 = Comm.GetAllDataFromTable("AddEmployee", "EMPLOYEETYPE", "EmployeeID='" + Request.QueryString["MangerId"].ToString() + "'", "");
                                    if (dt2.Rows.Count > 0)
                                    {
                                        if (dt2.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                                        {
                                            hypProjectTime.Visible = false;
                                        }
                                        else
                                        {
                                            hypProjectTime.Visible = true;
                                            hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString() + "&CustomRole=YES";
                                            Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";

                                        }
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
                                        hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();
                                        Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
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
                            //hypProjectTime.Visible = true;

                            //commented by kumar because custom role can also have R and P employees assigned to him.
                            //hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                            // hyProjectTimeSheet.Visible = false;
                            // hyViewTimeSheet.Visible = false;
                        }
                        //added by kumar to show if only project employees are under any business in any subscriber
                        else
                        {
                            //DataTable dtEmptype = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5)", "");
                            DataTable DtEmptype_Sub = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId not in (1)", "");
                            if (DtEmptype_Sub.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
                            {
                                hypProjectTime.Visible = false;
                            }
                            else
                            {
                                hypProjectTime.Visible = true;
                                hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();
                                Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                            }

                            if (Request.QueryString["MangerId"] != null)
                            {
                                if (dtEmptype.Rows.Count == 1 && dtEmptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P")
                                {
                                    hypProjectTime.Visible = true;
                                    hypProjectTime.NavigateUrl = "~/Project_Time_Sheet_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();
                                    Label_Select_Employee.ToolTip = "Click on Enter/View Project Time Sheet for Project Employee";
                                }
                            }
                        }
                    }
                }
            }

            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && user_type_id.ToString() == "1")
            {
                Div_Select_Business.Attributes.Add("style", "display:block;");
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
        if (ddlemp.SelectedIndex != 0)
        {
            GetGridFill();
        }
        }
        if (!IsPostBack)
        setGVShildListLockStatus();
    }

    public void GetAllDetails(string Bid)
    {
        if (Request.QueryString["CustomRole"] != null)
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
                            // dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R' AND [IsActive]='YES' and [EmployeeID]='" + Request.QueryString["MangerId"].ToString() + "'");

                            dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R'  and [EmployeeID]='" + Request.QueryString["MangerId"].ToString() + "'");
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
                        // dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R' AND [IsActive]='YES'");
                        dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R' ");
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
                // dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R' AND [IsActive]='YES'");
                dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'R' ");

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
        else
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
                    ////Commented because the new EmployeeType N is introduced for Accountant user type
                    ////Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL)", "ad.EmployeeID");
                    //commented because administrator will approve and reject timesheet for others only
                    //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");
                    Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType not in (1,5,12) and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "  and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");
                    //to show all project and all regular employees
                    //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE IS NULL OR ad.EMPLOYEETYPE='N' or ad.EMPLOYEETYPE='P')", "ad.EmployeeID");
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
                     // DataRow[] dtFilter = dtMangerEmpList.Select("[EmployeeID]<>"+ int.Parse(Request.QueryString["MangerId"].ToString()));
                       /*    if (dtFilter.Length > 0)
                           {
                               dtMangerEmpListFilter = dtFilter.CopyToDataTable();
                           }*/
                           ddlemp.Items.Clear();

                    ddlemp.DataSource = dtMangerEmpList;
                    ddlemp.DataValueField = "EmployeeID";
                    ddlemp.DataTextField = "EmpName";
                    
                    ddlemp.SelectedIndex = 0;
                    ddlemp.Items.Add("Please Select");
                           ddlemp.SelectedItem.Value = "";

                    ddlemp.DataBind();

                    /* if (dtMangerEmpList.Rows.Count > 0)
                     {
                         ddlemp.DataSource = dtMangerEmpList;
                         ddlemp.DataValueField = "EmployeeID";
                         ddlemp.DataTextField = "EmpName";
                         ddlemp.DataBind();
                         ddlemp.SelectedIndex = 0;
                     }*/
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
                        //commented because administrator will approve and reject timesheet for others only
                        //Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
                        Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType Not in (1,5,12) and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "  and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
                    }
                    else
                    {
                        Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "  and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
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
                        //commented because administrator will approve and reject timesheet for others only
                        //dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " AND AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");
                        dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " AND AE.UserType Not in (1,5,12) and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ae.EMPLOYEETYPE='R'", "AE.EmployeeID");

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
                        //Commented because the new EmployeeType N is introduced for Accountant user type
                        //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and ad.EMPLOYEETYPE='R'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                        //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                        //commented because administrator will approve and reject timesheet for others only
                        Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId Not in (1,5,12) and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                    }



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
                    //commented because administrator will approve and reject timesheet for others only
                    //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                    Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId Not in (1,5,12) and IsActive='YES' and (ad.EMPLOYEETYPE='R' OR ad.EMPLOYEETYPE='N')", "ad.EmployeeID");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                }
            }

        }


        //lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
    }
    //public void GetGridFill()
    //{
    //    if (ddlemp.SelectedValue == "")
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select employee first', 'error');", true);
    //        return;
    //    }
    //    if (txtmonth.Text != "")
    //    {
    //        int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
    //        DataTable dtchild = new DataTable();
    //        int y = DateTime.Parse(txtmonth.Text).Year;
    //        int m = DateTime.Parse(txtmonth.Text).Month;
    //        var d = DateTime.Parse(txtmonth.Text.Trim());
    //        int empid = int.Parse(ddlemp.SelectedValue);
    //        CultureInfo cul = CultureInfo.CurrentCulture;
    //        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
    //        dtchild = TSC.GetTimeSheetEntryChildGv(y, m, weekNum, Bid, empid);

    //        if (dtchild.Rows.Count > 0)
    //        {
    //            //GVChildlist.Enabled = true;
    //            //lblCurrentRate.Visible = true;
    //            //lblCurrentErrorSms.Visible = false;
    //            GVChildlist.DataSource = dtchild;
    //            GVChildlist.DataBind();
    //        }
    //        //firoz 06
    //        //if (y < 2021)
    //        //{
    //        //    GVChildlist.Enabled = false;
    //        //    lblCurrentRate.Visible = false;
    //        //    lblCurrentErrorSms.Visible = true;
    //        //    lblCurrentErrorSms.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
    //        //}
    //        //end  
    //    }
    //}
    public void GetGridFill()
    {
        Label_Approved_Hours.Text = "";
        Label_Rejected_Hours.Text = "";


        if (ddlemp.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select employee first', 'error');", true);
            return;
        }

        
         
        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()));
         
        if (txtmonth.Text != "")
        {
            int Bid = 0;
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
            {
                Bid = int.Parse(Ddl_Business.SelectedValue);
            }
            else
            {
                Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            }

            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            dtchild = TSC.GetTimeSheetEntryChildGv(y, m, weekNum, Bid, empid);
            

            if (dtchild.Rows.Count == 0)
            {
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();

            }

            if (dtchild.Rows.Count > 0)
            {
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();

                int[] Y = new int[dtchild.Rows.Count];
                string[] X = new string[dtchild.Rows.Count];

                int val2 = 0;
                int approved = 0;
                int absent = 0;
                int blank = 0;
                int Reject = 0;

                for (int i = 0; i < dtchild.Rows.Count; i++)
                {
                    if (dtchild.Rows[i][16].ToString() == "1")
                    {
                        if (val2 != 0)
                        {
                            continue;
                        }
                        for (int j = 0; j < dtchild.Rows.Count; j++)
                        {
                            if (dtchild.Rows[j][16].ToString() == "1")
                            {
                                var gross_hoursTime = DateTime.Parse(dtchild.Rows[j][15].ToString()).ToString("HH:mm");
                                var split = Convert.ToString(gross_hoursTime).Split(':');
                                if (split[0] != null)
                                {
                                    val2 = (Convert.ToInt32(val2) + Convert.ToInt32(split[0].ToString()));
                                    split[0] = null;
                                }
                            }

                            X[i] = "Total Pending Hours   - " + Convert.ToInt32(val2).ToString() + ":00";
                            Y[i] = Convert.ToInt32(val2);

                            Label_Pending_Hours.Text = " | Total Pending Hours - " + Convert.ToInt32(val2).ToString() + ":00";
                        }

                    }

                    if (dtchild.Rows[i][16].ToString() == "2")
                    {
                        if (approved != 0)
                        {
                            continue;
                        }
                        for (int j = 0; j < dtchild.Rows.Count; j++)
                        {
                            if (dtchild.Rows[j][16].ToString() == "2")
                            {
                                var gross_hoursTime = DateTime.Parse(dtchild.Rows[j][15].ToString()).ToString("HH:mm");
                                var split = Convert.ToString(gross_hoursTime).Split(':');
                                if (split[0] != null)
                                {
                                    approved = (Convert.ToInt32(approved) + Convert.ToInt32(split[0].ToString()));
                                    split[0] = null;
                                }

                            }

                        }
                        X[i] = "Total Approved Hours  - " + Convert.ToInt32(approved).ToString() + ":00";
                        //X[i] = Convert.ToInt32(approved).ToString();
                        Y[i] = Convert.ToInt32(approved); //Convert.ToInt32(dtchild.Rows[i][3]);
                                                          //if(X[i] == 0)
                                                          //{

                        //}   !
                        if (Convert.ToInt32(approved) != 0)
                        {

                            Label_Approved_Hours.Text = "Total Approved Hours - " + Convert.ToInt32(approved).ToString() + ":00";
                            Label_Approved_Hours.Visible = true;
                        }
                        else { Label_Approved_Hours.Visible = false; }
                    }

                    if (dtchild.Rows[i][16].ToString() == "3")
                    {
                        if (Reject != 0)
                        {
                            continue;
                        }
                        for (int k = 0; k < dtchild.Rows.Count; k++)
                        {
                            if (dtchild.Rows[k][16].ToString() == "3")
                            {
                                var gross_hoursTime = DateTime.Parse(dtchild.Rows[k][15].ToString()).ToString("HH:mm");
                                var split = Convert.ToString(gross_hoursTime).Split(':');
                                if (split[0] != null)
                                {
                                    Reject = (Convert.ToInt32(Reject) + Convert.ToInt32(split[0].ToString()));
                                }
                            }
                        }
                        X[i] = "Total Rejected Hours  - " + Convert.ToInt32(Reject).ToString() + ":00";
                        //X[i] = Convert.ToInt32(Reject).ToString();
                        Y[i] = Convert.ToInt32(Reject);
                        if (Reject != 0)
                        {
                            Label_Rejected_Hours.Text = "Total Rejected Hours - " + Convert.ToInt32(Reject).ToString() + ":00";
                            Label_Rejected_Hours.Visible = true;
                        }
                        else
                        { Label_Rejected_Hours.Visible = false; }
                    }

                    //if (dtchild.Rows[i][16].ToString() == null)
                    //{
                    //    var gross_hoursTime = DateTime.Parse(dtchild.Rows[i][15].ToString()).ToString("HH:mm");
                    //    var split = Convert.ToString(gross_hoursTime).Split(':');
                    //    blank = (Convert.ToInt32(blank) + Convert.ToInt32(split[0].ToString()));

                    //    X[i] = Convert.ToInt32(blank);// dtchild.Rows[i][15].ToString();
                    //    Y[i] = Convert.ToInt32(dtchild.Rows[i][3]);
                    //}
                }


                //for (int k = 0; k < dtchild.Rows.Count; k++)
                //{
                Chart2.Series[0].Points.DataBindXY(X, Y);

                //Chart2.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;

                // Chart2.Series[0].Points.DataBindXY(dtchild.Rows[X][15].ToString(), dtchild.Rows[i][16].ToString());
                //Chart2.Series[0].Points.DataBindXY(X[k], Y[k]);
                Chart2.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
                Chart2.Series[0].IsValueShownAsLabel = true;
                Chart2.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                Chart2.ChartAreas[0].AxisY.MajorGrid.Enabled = false;
                // Chart2.ChartAreas["Series1"].Label = "#PERCENT{P0}"; 
                Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                Chart2.Legends[0].Enabled = true;
                // Chart2.Series["Series1"].Label = "#PERCENT{P0}";
                // Chart2.Series[0].LegendText = "#VALX";
                //Chart2.Legends[0].LegendStyle = LegendStyle.Column;
                //Chart2.Legends[0].Docking = Docking.Right;
                //Chart2.Legends[0].Alignment = System.Drawing.StringAlignment.Center;
                // }
                //rdr.Close();



                //Chart2.Series[0];
                //Chart2.Series[0].Label = "#PERCENT{P0}\n#VALX";
                //Chart2.Series[0].Label = "#PERCENT{X}";

                // ["PieLabelStyle"] = "Disabled"; Chart2.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
                //Chart2.Legends[0].Enabled = true;

            }
        }
        DateTime TimeCardDate = new DateTime();
        if (txtmonth.Text != "" || txtmonth.Text == null)
        {
            TimeCardDate = DateTime.Parse(txtmonth.Text);
        }
        else { TimeCardDate = DateTime.Now; }
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        System.Globalization.Calendar calendar = currentCulture.Calendar;
        CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
        int WeekNum = calendar.GetWeekOfYear(TimeCardDate, weekRule, firstDayOfWeek);
        // Get the week number using the culture-specific rules
        DataTable dtTimeSheetStatus = Comm.GetAllDataFromTable("TimeCardStatus", "TimeSheetStatus", "EmployeeID=" + ddlemp.SelectedValue + " and BusinessID=" + Int32.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and WeekNum=" + WeekNum + " and Year=" + TimeCardDate.Year, "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
        DataTable dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " )", "");

        if (dt.Rows.Count != 0)
        {
            lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
        }

        if (dtTimeSheetStatus.Rows.Count != 0)
        {
            int TimeCardStatus;
            TimeCardStatus = Int32.Parse(dtTimeSheetStatus.Rows[0][0].ToString());

            if (TimeCardStatus == 0)
            {
                GVChildlist.Enabled = true;
                //lblLocked.Visible = false;
            }
            else
            {
                GVChildlist.Enabled = false;
               // lblLocked.Visible = true;
            }
        }

       
    }
    private int RepairBusinessId { get { return (Convert.ToString(Session["UserTypeid"]) == "1" && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null ? int.Parse(Ddl_Business.SelectedValue) : int.Parse(Request.QueryString["BusinessIdFromDefaultPage"])); } }
    private void RepairMessage(string message, bool error)
    {
        string text = System.Web.HttpUtility.JavaScriptStringEncode(message);
        ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "RepairResult",
            "swal('" + (error ? "Action not completed" : "Success") + "', '" + text + "', '" + (error ? "error" : "success") + "');", true);
    }
    private void RefreshRepairBreaks()
    {
        DateTime day = DateTime.Parse(Convert.ToString(ViewState["datebreak"]));
        GvPopUpEmpList.DataSource = AnyminuteTimesheetRepair.ReadBreaks(
            Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]),
            RepairBusinessId, int.Parse(ddlemp.SelectedValue), day);
        GvPopUpEmpList.DataBind();
        GetGridFill();
        ModalPopupEmpList.Show();
    }
    protected void lnkEditBreak_Click(object sender, EventArgs e)
    {
        try
        {
            var row = (GridViewRow)((LinkButton)sender).NamingContainer;
            int id = int.Parse(((HiddenField)row.FindControl("brkid")).Value);
            var data = AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]),
                Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue),
                DateTime.Parse(Convert.ToString(ViewState["datebreak"])));
            DataRow[] matches = data.Select("BreakID=" + id);
            if(matches.Length != 1) throw new InvalidOperationException("Break no longer exists. Refresh and retry.");
            txtBreakStartTime.Text = DateTime.Parse(Convert.ToString(matches[0]["BreakStart"])).ToString("yyyy-MM-ddTHH:mm");
            txtBreakEndTime.Text = DateTime.Parse(Convert.ToString(matches[0]["BreakEnd"])).ToString("yyyy-MM-ddTHH:mm");
            ViewState["RepairBreakId"] = id;
            btnAddBreak.Text = "Save Break";
            ModalPopupEmpList.Show();
        }
        catch(InvalidOperationException ex) { RepairMessage(ex.Message, true); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Edit break failed", ex); RepairMessage("Unable to load this break. Refresh and try again.", true); }
    }
    private void CopyRepairWeek(bool nextWeek)
    {
        try
        {
            if(GVChildlist.Rows.Count == 0 || String.IsNullOrWhiteSpace(ddlemp.SelectedValue))
                throw new InvalidOperationException("Select an employee and load the week first.");
            var sources = new System.Collections.Generic.List<DateTime>();
            var targets = new System.Collections.Generic.List<DateTime>();
            if(nextWeek)
            {
                foreach(GridViewRow row in GVChildlist.Rows)
                { DateTime date = DateTime.Parse(((Label)row.FindControl("lblDate")).Text).Date;
                  sources.Add(date); targets.Add(date.AddDays(7)); }
            }
            else
            {
                // Copy Entire Week repeats the first saved present day into unentered days of this displayed week.
                DateTime? source = null;
                using(var c = AnyminuteTimesheetRepair.Open())
                {
                    AnyminuteTimesheetRepair.Authorize(c, null, Convert.ToInt32(Session["EmployeeId"]),
                        Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue), true);
                    foreach(GridViewRow row in GVChildlist.Rows)
                    {
                        DateTime date = DateTime.Parse(((Label)row.FindControl("lblDate")).Text).Date;
                        var data = AnyminuteTimesheetRepair.Day(c, null, RepairBusinessId, int.Parse(ddlemp.SelectedValue), date);
                        if(data.Rows.Count == 1 && Convert.ToString(data.Rows[0]["Worked"]).Trim() == "Yes") { source=date; break; }
                    }
                }
                if(!source.HasValue) throw new InvalidOperationException("Save at least one present day before using Copy Entire Week.");
                foreach(GridViewRow row in GVChildlist.Rows)
                { sources.Add(source.Value); targets.Add(DateTime.Parse(((Label)row.FindControl("lblDate")).Text).Date); }
            }
            int count = AnyminuteTimesheetRepair.Copy(Convert.ToInt32(Session["EmployeeId"]),
                Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue),
                sources.ToArray(), targets.ToArray(), nextWeek);
            if(nextWeek && count > 0) txtmonth.Text = DateTime.Parse(txtmonth.Text).AddDays(7).ToString("yyyy-MM-dd");
            GetGridFill();
            RepairMessage(count > 0 ? count + " day(s) copied with editable break records." : "No eligible days were copied. Existing days were preserved.", false);
        }
        catch(InvalidOperationException ex) { RepairMessage(ex.Message, true); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Copy week failed", ex); RepairMessage("Nothing was copied. Check the source records and destination locks, or contact support.", true); }
    }

    protected void btnAddBreak_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime start, end, day;
            if(!DateTime.TryParse(txtBreakStartTime.Text, out start) || !DateTime.TryParse(txtBreakEndTime.Text, out end) ||
                !DateTime.TryParse(Convert.ToString(ViewState["datebreak"]), out day))
                throw new InvalidOperationException("Enter valid break dates and times.");
            int? id = ViewState["RepairBreakId"] == null ? (int?)null : Convert.ToInt32(ViewState["RepairBreakId"]);
            AnyminuteTimesheetRepair.ChangeBreak(Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]),
                RepairBusinessId, int.Parse(ddlemp.SelectedValue), day, id, start, end, false);
            ViewState.Remove("RepairBreakId"); btnAddBreak.Text = "Add";
            txtBreakStartTime.Text = ""; txtBreakEndTime.Text = "";
            RefreshRepairBreaks();
        }
        catch(InvalidOperationException ex) { RepairMessage(ex.Message, true); ModalPopupEmpList.Show(); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Save break failed", ex); RepairMessage("Break was not saved. Refresh and check the timecard lock or contact support.", true); }
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

        if (ddlemp.SelectedValue != "")
        {
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] == null)
            {
                DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
                "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
                if (Dt_Get_UserType.Rows.Count > 0)
                {
                    if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "2")
                    {
                        //this.GVChildlist.Columns[18].Visible = false;
                        //this.GVChildlist.Columns[19].Visible = true;
                        Response.Redirect("~/Time_Sheet_Entry.aspx?EmpidfromBusOwner="+Request.QueryString["MangerId"] + "&BidfromBusOwner="+ Request.QueryString["BusinessIdFromDefaultPage"]);
                    }
                    if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "2")
                    {
                        this.GVChildlist.Columns[18].Visible = true;
                        this.GVChildlist.Columns[19].Visible = false;
                    }


                    if (Request.QueryString["ManagerID"] == (ddlemp.SelectedValue))
                    {

                        this.GVChildlist.Columns[18].Visible = true;
                        this.GVChildlist.Columns[19].Visible = false;
                    }
                }
            }
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] != null)
            {
                DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId,EmployeeID from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
                "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
                if (Dt_Get_UserType.Rows.Count > 0)
                {
                    if (Dt_Get_UserType.Rows[0]["EmployeeID"].ToString() == Request.QueryString["MangerId"].ToString())
                    {
                        this.GVChildlist.Columns[18].Visible = false;
                        this.GVChildlist.Columns[19].Visible = true;
                    }
                    else
                    {
                        this.GVChildlist.Columns[18].Visible = true;
                        this.GVChildlist.Columns[19].Visible = false;
                    }
                }
            }
        }

        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select employee', 'error');", true);
            return;
        }

        label_ConfirmEmpSelection.Text = ddlemp.SelectedItem.Text + " Employee is Selected, <br> Please Confirm?";

        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business.SelectedValue));
        }
        else
        {
            lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()));
        }

        Label_Selected_EmpName.Text = ddlemp.SelectedItem.Text;

        Label_Approved_Hours.Text = "";
        Label_Rejected_Hours.Text = "";
        Label_Pending_Hours.Text = "";

        if (HiddenField_confirm_pop_up_count.Value != "0")
            ModalPopupPnlConfirmEmpSelection.Show();
        if (HiddenField_confirm_pop_up_count.Value == "0")
            GetGridFill();

        DataTable dt = new DataTable();
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + DateTime.Parse(txtmonth.Text) + "' )", "");
       
        //DataTable dt_get_hiredate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "StartDate", "RateId=(select MIN(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        DataTable dt_get_hiredate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "convert(varchar, StartDate, 101) as StartDate", "RateId=(select MIN(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " )", "");
        Label_HireDate.Text = dt_get_hiredate.Rows[0]["StartDate"].ToString();
        Label_Seperation_.Visible = true;
        Label_Seperation_1.Visible = true;

        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "$" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
            
            GetGridFill();
            GVChildlist.Enabled = true;
        }
        if (dt.Rows.Count == 0)
        {
            lblCurrentRate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
            GVChildlist.Enabled = false;
        }
        HiddenField_confirm_pop_up_count.Value = "1";
        GetGridFill();
    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtmonth.Text);
        DateTime nxtdate = CurrentDate.AddMonths(-1);
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        txtmonth_TextChanged(sender, e);
    }

    //firoz 15-22
    public void GetAllDetails(string empid, string Bid, DateTime Calender_Selected_Date)
    {
        //To reflect the day of date choosen from calendar control
        string get_month_from_date_control = txtmonth.Text;
        DateTime get_date_from_date_control;
        if (DateTime.TryParse(get_month_from_date_control, out get_date_from_date_control))
        {
            lblWeek.Text = get_date_from_date_control.DayOfWeek.ToString();
        }
        lblWeek.Text = DateTime.Parse(txtmonth.Text).DayOfWeek.ToString();

        DataTable dt = new DataTable();
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        //lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();

        //DataTable dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "$" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
            //GVChildlist.Enabled = true;
        }
        if (dt.Rows.Count == 0)
        {
            lblCurrentRate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
            //GVChildlist.Enabled = false;
        }
    }
    //end
    protected void txtmonth_TextChanged(object sender, EventArgs e)
    {
        Label_Approved_Hours.Text = "";
        Label_Rejected_Hours.Text = "";
        Label_Pending_Hours.Text = "";

        if (txtmonth.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date', 'warning');", true);
            return;
        }

        DateTime Min_Date = DateTime.Parse("01/01/1753");
        DateTime Selected_Date = DateTime.Parse(txtmonth.Text);
        if (Selected_Date < Min_Date)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date Properly!', 'warning');", true);
            txtmonth.Text = DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            return;
        }

        if (ddlemp.SelectedIndex != 0)
        {
            //firoz 15-22
            GetAllDetails(Request.QueryString["BusinessIdFromDefaultPage"].ToString(), Request.QueryString["BusinessIdFromDefaultPage"].ToString(), DateTime.Parse(txtmonth.Text));
            //txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");

            //GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtmonth.Text));
            //end
            GetGridFill();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'Employee is not selected', 'info');", true);
        }
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

    private int ReviewAbsentRow(GridViewRow row, int reviewStatus)
    {
        int businessId = (Request.QueryString["MangerId"] == null &&
            Request.QueryString["CustomRole"] == null && Convert.ToString(Session["UserTypeid"]) == "1")
            ? int.Parse(Ddl_Business.SelectedValue)
            : int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
        AnyminuteAbsentReview.Review(
            Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]),
            businessId, int.Parse(ddlemp.SelectedValue),
            DateTime.Parse(((Label)row.FindControl("lblDate")).Text).Date,
            int.Parse(((HiddenField)row.FindControl("hfdweekno")).Value.Split(',')[0]),
            reviewStatus);
        return -1; // Already persisted, do not execute legacy unscoped UPDATE.
    }

    private void ReviewAbsentAndRefresh(GridViewRow row, int reviewStatus)
    {
        try
        {
            ReviewAbsentRow(row, reviewStatus);
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "SweetAlert",
                reviewStatus == 2
                ? "swal('Success!', 'Absent day approved and locked.', 'success');"
                : "swal('Success!', 'Absent day rejected and reopened for correction.', 'success');", true);
        }
        catch (SqlException ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Absent day review failed", ex);
            ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "SweetAlert",
                "swal('Error!', 'Review could not be completed. Refresh the page and check your permissions or contact support.', 'error');", true);
        }
    }

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

        if (ddstatus.SelectedIndex == 1)
        {
            return ReviewAbsentRow(row, isRejected ? 3 : 2);
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

        // Persist first, then verify the exact business/employee/date row exists before
        // reporting success. The legacy controller method returns void, so a normal
        // stored-procedure return does not prove that a row was actually written.
        TSC.Sp_InsertTimeSheetEntry(Tsee);

        string persistedWhere =
            "EmpId=" + Tsee.EmpId +
            " and BussID=" + Tsee.BussID +
            " and TimeSheetDate='" + Tsee.TimeSheetDate.Date.ToString("yyyy-MM-dd") + "'";

        bool persisted = Comm.IfExistsInTable("TimeSheetEntry", "*", persistedWhere);
        if (!persisted)
        {
            log4net.LogManager.GetLogger("Log").Error(
                "Time sheet save returned without persistence. EmpId=" + Tsee.EmpId +
                ", BussID=" + Tsee.BussID +
                ", TimeSheetDate=" + Tsee.TimeSheetDate.Date.ToString("yyyy-MM-dd"));
            return 0;
        }

        return 1;
    }

    private Tuple<DateTime, DateTime, TimeSpan, bool> CalculatedTimingHours(GridViewRow row)
    {
        var lblDate = (Label)row.FindControl("lblDate");
        var hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        int businessId = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        int employeeId = int.Parse(ddlemp.SelectedValue);
        var check = Comm.IfExistsInTable("TimeSheetEntry", "*",
            "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") +
            "' and EmpId=" + employeeId +
            " and BussID=" + businessId);
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
            var chkApproved = (CheckBox)row.FindControl("hdnCheckBox");
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
                    if (result == 0)
                    {
                        var failedDate = (Label)row.FindControl("lblDate");
                        sb.Append("<br/>- " + failedDate.Text + ": save returned without a persisted TimeSheetEntry row.");
                    }
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
                                                    and     BussID='{2}'
                                                    and     TimeSheetDate='{3}'",
                                                        weekNo.Value,
                                                        ddlemp.SelectedValue,
                                                        Request.QueryString["BusinessIdFromDefaultPage"],
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

        //ScriptManager
        //    .RegisterClientScriptBlock(
        //                this.Page,
        //                this.GetType(),
        //                "SweetAlert",
        //                "swal({html: '<div>" + updateCount + " Time sheet got approved<br/>" + statusMessage + "</div>'});",
        //            true);

        if (updateCount > 0 && String.IsNullOrWhiteSpace(statusMessage))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert",
                "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        }
        else if (updateCount > 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert",
                "swal({title: 'Completed with warnings', html: '" + statusMessage.Replace("'", "\\'") + "', icon: 'warning'});", true);
        }
        else
        {
            var safeStatus = String.IsNullOrWhiteSpace(statusMessage)
                ? "Time sheet was not saved. Please refresh and try again or contact support."
                : statusMessage;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert",
                "swal({title: 'Save failed', html: '" + safeStatus.Replace("'", "\\'") + "', icon: 'error'});", true);
        }

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
    protected void lnkUnLock_Click(object sender, EventArgs e)
    {
        DateTime TimeCardDate = new DateTime();
        TimeCardDate = DateTime.Parse(txtmonth.Text);

        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        System.Globalization.Calendar calendar = currentCulture.Calendar;
        CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;

        // Get the week number using the culture-specific rules
        int weekNum = calendar.GetWeekOfYear(TimeCardDate, weekRule, firstDayOfWeek);
        int BussId = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
        int EmployeeID = int.Parse(ddlemp.SelectedValue);
        Comm.ExecuteQuery("UPDATE TimeCardStatus SET TimeSheetStatus=0 where BusinessID="+BussId+" and EmployeeID="+EmployeeID
            + " AND weekNum="+weekNum + " AND Year="+TimeCardDate.Year);
        
       // lblLocked.Visible = false;
        // Get the ISO 8601 week number

        //Responspage_e.Write(txtmonth.Text);

                GetGridFill();
        GVChildlist.Enabled = true;


    }

    public void setGVShildListLockStatus()
    {
        DateTime TimeCardDate = new DateTime();
        TimeCardDate = DateTime.Parse(txtmonth.Text);

        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        System.Globalization.Calendar calendar = currentCulture.Calendar;
        CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;

        // Get the week number using the culture-specific rules
        int weekNum = calendar.GetWeekOfYear(TimeCardDate, weekRule, firstDayOfWeek);
        int BussId = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
        int EmployeeID = 0;
        if ( ddlemp.SelectedValue.ToString()!="")
        {
             EmployeeID = int.Parse(ddlemp.SelectedValue);
        }
        DataTable dt = new DataTable();
        dt = Comm.GetAllDataFromTable("TimeCardStatus", "TimeSheetStatus", "BusinessID = " + BussId + " and EmployeeID = " + EmployeeID
            + " AND weekNum=" + weekNum + " AND Year=" + TimeCardDate.Year,"");
        // dt =Comm.ExecuteQuery("Select TimeSheetStatus from TimeCardStatus  where BusinessID=" + BussId + " and EmployeeID=" + EmployeeID
        //     + " AND weekNum=" + weekNum + " AND Year=" + TimeCardDate.Year);

        // lblLocked.Visible = false;
        // Get the ISO 8601 week number

        //Responspage_e.Write(txtmonth.Text);
        int TimeSheetStatus;
        if (dt.Rows.Count != 0)
        {
            TimeSheetStatus = int.Parse(dt.Rows[0][0].ToString());
            if (TimeSheetStatus == 0)
            {
                GVChildlist.Enabled = true; }

            if (TimeSheetStatus == 1)
            {
                GVChildlist.Enabled = false;
            }
        }
        else
        { GVChildlist.Enabled = true; }


    }

    protected void LinkButton15_Click(object sender, EventArgs e)
    { 
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "alert('"+GVChildlist.Enabled+"')",  true); 
    }


        protected void lnkLock_Click(object sender, EventArgs e)
    {
        DateTime TimeCardDate = new DateTime();
        TimeCardDate = DateTime.Parse(txtmonth.Text);

        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        System.Globalization.Calendar calendar = currentCulture.Calendar;
        CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;

        // Get the week number using the culture-specific rules
        int weekNum = calendar.GetWeekOfYear(TimeCardDate, weekRule, firstDayOfWeek);
        int BussId = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]);
        int EmployeeID = int.Parse(ddlemp.SelectedValue);
        Comm.ExecuteQuery("UPDATE TimeCardStatus SET TimeSheetStatus=1 where BusinessID=" + BussId + " and EmployeeID=" + EmployeeID
            + " AND weekNum=" + weekNum + " AND Year=" + TimeCardDate.Year + " IF  @@ROWCOUNT=0 BEGIN " +
            " INSERT INTO TIMECARDSTATUS VALUES ("+ EmployeeID + ", "+ BussId + ", " +weekNum + ", "+ TimeCardDate.Year +
            ",1) END "  );
        GVChildlist.Enabled = false;
       // lblLocked.Visible = true;

    }


 

    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        #region commented code because giving error of save command
        ////For the business owners
        //string user_type_id = Session["UserTypeid"].ToString();
        //if (user_type_id == "1")
        //{
        //    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //    GridView GVChildlist = (GridView)sender;

        //    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //    if (e.CommandName == "Savee")
        //    {
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];
        //        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //        TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
        //        Label lblError = ((Label)GVChildlist.Rows[index].FindControl("lblError"));
        //        Label lblErrorend = ((Label)GVChildlist.Rows[index].FindControl("lblErrorend"));

        //        if (ddstatus.SelectedIndex == 0)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            // absent(row.RowIndex);
        //            return;
        //        }
        //        if (ddstatus.SelectedIndex == 1)
        //        {
        //            //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it. 
        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            hfdRIndex.Value = row.RowIndex.ToString();
        //            ModalPopupConfirmSingal.Show();
        //            //absent(row.RowIndex);
        //            return;
        //        }
        //        if (txtRegularStartTime.Text == ",")
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            lblError.Visible = true;
        //            return;
        //        }
        //        string startingtime = txtRegularStartTime.Text;
        //        if (startingtime == "")
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            lblError.Visible = true;
        //            return;
        //        }

        //        if (txtRegularEndTime.Text == "")
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //            lblErrorend.Visible = true;
        //            return;
        //        }
        //        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //        if (check == true)
        //        {
        //            Over15x = txtOver15x.Text.Split(',');
        //            Over2x = txtOver2x.Text.Split(',');
        //            holiday = txtholiday.Text.Split(',');
        //            RegularStartTime = txtRegularStartTime.Text.Split(',');
        //            RegularEndTime = txtRegularEndTime.Text.Split(',');

        //            // BreakStartTime = txtBreakStartTime.Text.Split(',');
        //            //BreakEndTime = txtBreakEndTime.Text.Split(',');

        //            //Regular[0] = Regular[1];
        //            //Over15x[0] = Over15x[1];
        //            //Over2x[0] = Over2x[1];
        //            // holiday[0] = holiday[1];
        //            //RegularStartTime[0] = RegularStartTime[1];
        //            //RegularEndTime[0] = RegularEndTime[1];

        //            //BreakStartTime[0] = BreakStartTime[1];
        //            //BreakEndTime[0] = BreakEndTime[1];
        //        }
        //        else
        //        {
        //            //Regular[0] = txtRegular.Text.Substring(1);
        //            // Over15x[0] = txtOver15x.Text.Substring(1);
        //            // Over2x[0] = txtOver2x.Text.Substring(1);
        //            // holiday[0] = txtholiday.Text.Substring(1);

        //            RegularStartTime[0] = txtRegularStartTime.Text;
        //            RegularEndTime[0] = txtRegularEndTime.Text;

        //            //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
        //            //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
        //        }

        //        string[] weekno = hfdweekno.Value.Split(',');
        //        if (Regular[0] == "")
        //        {
        //            Regular[0] = "0";
        //        }
        //        if (Over15x[0] == "")
        //        {
        //            Over15x[0] = "0";
        //        }
        //        if (Over2x[0] == "")
        //        {
        //            Over2x[0] = "0";
        //        }
        //        if (holiday[0] == "")
        //        {
        //            holiday[0] = "0";
        //        }


        //        if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //            return;
        //        }
        //        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //        if (ToltalRegulahr.TotalHours >= 24)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //            return;
        //        }
        //        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //        if (ToltalRegulahr < TotalBreakHr)
        //        {
        //            GetGridFill();
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //            return;
        //        }
        //        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //        Tsee.WeekNo = int.Parse(weekno[0]);
        //        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //        Tsee.Day = lblDay.Text;
        //        Tsee.Regular = ToltalRegulahr.ToString();
        //        Tsee.Over1_5x = Over15x[0];
        //        Tsee.Over2x = Over2x[0];
        //        Tsee.Holiday = holiday[0];
        //        Tsee.Total = TotalHr.ToString();
        //        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //        Tsee.BreakStart = BreakStartTime[0].ToString();
        //        Tsee.BreakEnd = BreakEndTime[0].ToString();
        //        if (hfdStatus.Value == "2")
        //        {
        //            //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //            //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //            //Tsee.Status = "2";

        //            //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //            Tsee.Status = "1";
        //        }
        //        else
        //        {
        //            Tsee.Status = "1";
        //        }


        //        //if (chkworked.Checked == true)
        //        //{
        //        //    Tsee.Worked = "Yes";
        //        //}
        //        //else
        //        //{
        //        //    Tsee.Worked = "No";
        //        //}
        //        if (ddstatus.SelectedIndex == 2)
        //        {
        //            Tsee.Worked = "Yes";
        //        }
        //        if (ddstatus.SelectedIndex == 1)
        //        {
        //            Tsee.Worked = "No";
        //        }
        //        Tsee.BreakTotal = TotalBreakHr.ToString();
        //        TSC.Sp_InsertTimeSheetEntry(Tsee);

        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //        GetGridFill();


        //    }

        //    if (e.CommandName == "Submit")//6070415004624658
        //    {
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];

        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //        if (ddstatus.SelectedIndex == 0)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            // absent(row.RowIndex);
        //            return;
        //        }
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        //if (chkworked.Checked == false)
        //        //{
        //        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //        //    return;
        //        //}
        //        if (txtRegularStartTime.Text == ",")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            return;
        //        }

        //        if (txtRegularEndTime.Text == "")
        //        {
        //            GetGridFill();
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //            return;
        //        }
        //        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //        if (CheckSaved == true)
        //        {
        //            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //            GetGridFill();
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //        }
        //        else
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //        }


        //        //Response.Redirect(Request.RawUrl);
        //    }

        //    if (e.CommandName == "AddBreak")
        //    {
        //        DataTable dt;
        //        GvPopUpEmpList.DataSource = null;
        //        GvPopUpEmpList.DataBind();
        //        txtBreakEndTime.Text = string.Empty;
        //        txtBreakStartTime.Text = string.Empty;
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];

        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //        // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

        //        //if (chkworked.Checked == false)
        //        //{
        //        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //        //    return;
        //        //}
        //        if (ddstatus.SelectedIndex == 0)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //            return;
        //        }
        //        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //        lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //        ViewState["datebreak"] = lblDate.Text;
        //        ViewState["weekno"] = hfdweekno.Value;
        //        string regMINst = lblDate.Text + " 00:00";
        //        string regMAXst = lblDate.Text + " 23:59";
        //        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //        string regMinEnd = lblDate.Text + " 00:00";
        //        //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        //        string regMAXend = lblDate.Text + " 23:59";

        //        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //        // hfddatebreak.Value = lblDate.Text;
        //        // hfdweeknopop.Value = hfdweekno.Value;
        //        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        ViewState["dt"] = dt;
        //        GvPopUpEmpList.DataSource = ViewState["dt"];
        //        GvPopUpEmpList.DataBind();
        //        //up2.Update();
        //        //GetGridFill();

        //        ModalPopupEmpList.Show();

        //        return;
        //    }

        //    return;
        //}

        ////For manager gridview Enter/View Time Sheet hyperlink
        //if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
        //{
        //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
        //    DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

        //    //When users are old who are registered before the role permission development
        //    if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
        //    {
        //        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //        GridView GVChildlist = (GridView)sender;

        //        // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //        if (e.CommandName == "Savee")
        //        {
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];
        //            TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //            TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //            TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //            TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //            Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                // absent(row.RowIndex);
        //                return;
        //            }
        //            if (ddstatus.SelectedIndex == 1)
        //            {
        //                //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it.
        //                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                absent(row.RowIndex);
        //                return;
        //            }
        //            if (txtRegularStartTime.Text == ",")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }
        //            string startingtime = txtRegularStartTime.Text;
        //            if (startingtime == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }


        //            if (txtRegularEndTime.Text == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                return;
        //            }
        //            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //            if (check == true)
        //            {

        //                Over15x = txtOver15x.Text.Split(',');
        //                Over2x = txtOver2x.Text.Split(',');
        //                holiday = txtholiday.Text.Split(',');
        //                RegularStartTime = txtRegularStartTime.Text.Split(',');
        //                RegularEndTime = txtRegularEndTime.Text.Split(',');

        //                // BreakStartTime = txtBreakStartTime.Text.Split(',');
        //                //BreakEndTime = txtBreakEndTime.Text.Split(',');

        //                //Regular[0] = Regular[1];
        //                //Over15x[0] = Over15x[1];
        //                //Over2x[0] = Over2x[1];
        //                // holiday[0] = holiday[1];
        //                //RegularStartTime[0] = RegularStartTime[1];
        //                //RegularEndTime[0] = RegularEndTime[1];

        //                //BreakStartTime[0] = BreakStartTime[1];
        //                //BreakEndTime[0] = BreakEndTime[1];
        //            }
        //            else
        //            {
        //                //Regular[0] = txtRegular.Text.Substring(1);
        //                // Over15x[0] = txtOver15x.Text.Substring(1);
        //                // Over2x[0] = txtOver2x.Text.Substring(1);
        //                // holiday[0] = txtholiday.Text.Substring(1);

        //                RegularStartTime[0] = txtRegularStartTime.Text;
        //                RegularEndTime[0] = txtRegularEndTime.Text;

        //                //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
        //                //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
        //            }



        //            string[] weekno = hfdweekno.Value.Split(',');
        //            if (Regular[0] == "")
        //            {
        //                Regular[0] = "0";
        //            }
        //            if (Over15x[0] == "")
        //            {
        //                Over15x[0] = "0";
        //            }
        //            if (Over2x[0] == "")
        //            {
        //                Over2x[0] = "0";
        //            }
        //            if (holiday[0] == "")
        //            {
        //                holiday[0] = "0";
        //            }


        //            if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //                return;
        //            }
        //            TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //            if (ToltalRegulahr.TotalHours >= 24)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                return;
        //            }
        //            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //            if (ToltalRegulahr < TotalBreakHr)
        //            {
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                return;
        //            }
        //            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        //            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //            Tsee.WeekNo = int.Parse(weekno[0]);
        //            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //            Tsee.Day = lblDay.Text;
        //            Tsee.Regular = ToltalRegulahr.ToString();
        //            Tsee.Over1_5x = Over15x[0];
        //            Tsee.Over2x = Over2x[0];
        //            Tsee.Holiday = holiday[0];
        //            Tsee.Total = TotalHr.ToString();
        //            Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //            Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //            Tsee.BreakStart = BreakStartTime[0].ToString();
        //            Tsee.BreakEnd = BreakEndTime[0].ToString();
        //            if (hfdStatus.Value == "2")
        //            {
        //                //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //                //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //                //Tsee.Status = "2";

        //                //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //                Tsee.Status = "1";
        //            }
        //            else
        //            {
        //                Tsee.Status = "1";
        //            }
        //            if (ddstatus.SelectedIndex == 2)
        //            {
        //                Tsee.Worked = "Yes";
        //            }
        //            if (ddstatus.SelectedIndex == 1)
        //            {
        //                Tsee.Worked = "No";
        //            }
        //            Tsee.BreakTotal = TotalBreakHr.ToString();
        //            TSC.Sp_InsertTimeSheetEntry(Tsee);

        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //            GetGridFill();


        //        }

        //        if (e.CommandName == "Submit")//6070415004624658
        //        {
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];

        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                // absent(row.RowIndex);
        //                return;
        //            }
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            if (txtRegularStartTime.Text == ",")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }

        //            if (txtRegularEndTime.Text == "")
        //            {
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                return;
        //            }
        //            bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //            if (CheckSaved == true)
        //            {
        //                Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //            }


        //            //Response.Redirect(Request.RawUrl);
        //        }

        //        if (e.CommandName == "AddBreak")
        //        {
        //            DataTable dt;
        //            GvPopUpEmpList.DataSource = null;
        //            GvPopUpEmpList.DataBind();
        //            txtBreakEndTime.Text = string.Empty;
        //            txtBreakStartTime.Text = string.Empty;
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];

        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //                return;
        //            }
        //            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //            lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //            ViewState["datebreak"] = lblDate.Text;
        //            ViewState["weekno"] = hfdweekno.Value;
        //            string regMINst = lblDate.Text + " 00:00";
        //            string regMAXst = lblDate.Text + " 23:59";
        //            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //            string regMinEnd = lblDate.Text + " 00:00";
        //            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        //            string regMAXend = lblDate.Text + " 23:59";

        //            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //            ViewState["dt"] = dt;
        //            GvPopUpEmpList.DataSource = ViewState["dt"];
        //            GvPopUpEmpList.DataBind();
        //            //up2.Update();
        //            //GetGridFill();

        //            ModalPopupEmpList.Show();
        //        }
        //        return;
        //    }

        //    //When users are new who are registered after the role permission development
        //    string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');

        //    foreach (string role_check_for_add_edit_time_sheet in splittedArray)
        //    {
        //        #region For_Enter_And_Edit_Time_Sheet definition
        //        if (role_check_for_add_edit_time_sheet == "7" || role_check_for_add_edit_time_sheet == "8")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Savee")
        //            {


        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];
        //                TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //                TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //                TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //                TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //                Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //                HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    // absent(row.RowIndex);
        //                    return;
        //                }
        //                if (ddstatus.SelectedIndex == 1)
        //                {
        //                    //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it.
        //                    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    absent(row.RowIndex);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == ",")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                string startingtime = txtRegularStartTime.Text;
        //                if (startingtime == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularEndTime.Text == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //                if (check == true)
        //                {

        //                    Over15x = txtOver15x.Text.Split(',');
        //                    Over2x = txtOver2x.Text.Split(',');
        //                    holiday = txtholiday.Text.Split(',');
        //                    RegularStartTime = txtRegularStartTime.Text.Split(',');
        //                    RegularEndTime = txtRegularEndTime.Text.Split(',');

        //                    // BreakStartTime = txtBreakStartTime.Text.Split(',');
        //                    //BreakEndTime = txtBreakEndTime.Text.Split(',');

        //                    //Regular[0] = Regular[1];
        //                    //Over15x[0] = Over15x[1];
        //                    //Over2x[0] = Over2x[1];
        //                    // holiday[0] = holiday[1];
        //                    //RegularStartTime[0] = RegularStartTime[1];
        //                    //RegularEndTime[0] = RegularEndTime[1];

        //                    //BreakStartTime[0] = BreakStartTime[1];
        //                    //BreakEndTime[0] = BreakEndTime[1];
        //                }
        //                else
        //                {
        //                    //Regular[0] = txtRegular.Text.Substring(1);
        //                    // Over15x[0] = txtOver15x.Text.Substring(1);
        //                    // Over2x[0] = txtOver2x.Text.Substring(1);
        //                    // holiday[0] = txtholiday.Text.Substring(1);

        //                    RegularStartTime[0] = txtRegularStartTime.Text;
        //                    RegularEndTime[0] = txtRegularEndTime.Text;

        //                    //BreakStartTime[0] = txtBreakStartTime.Text.Substring(1);
        //                    //BreakEndTime[0] = txtBreakEndTime.Text.Substring(1);
        //                }



        //                string[] weekno = hfdweekno.Value.Split(',');
        //                if (Regular[0] == "")
        //                {
        //                    Regular[0] = "0";
        //                }
        //                if (Over15x[0] == "")
        //                {
        //                    Over15x[0] = "0";
        //                }
        //                if (Over2x[0] == "")
        //                {
        //                    Over2x[0] = "0";
        //                }
        //                if (holiday[0] == "")
        //                {
        //                    holiday[0] = "0";
        //                }


        //                if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //                Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //                Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //                Tsee.WeekNo = int.Parse(weekno[0]);
        //                Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //                Tsee.Day = lblDay.Text;
        //                Tsee.Regular = ToltalRegulahr.ToString();
        //                Tsee.Over1_5x = Over15x[0];
        //                Tsee.Over2x = Over2x[0];
        //                Tsee.Holiday = holiday[0];
        //                Tsee.Total = TotalHr.ToString();
        //                Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //                Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //                Tsee.BreakStart = BreakStartTime[0].ToString();
        //                Tsee.BreakEnd = BreakEndTime[0].ToString();
        //                if (hfdStatus.Value == "2")
        //                {
        //                    //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //                    //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //                    //Tsee.Status = "2";

        //                    //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //                    Tsee.Status = "1";
        //                }
        //                else
        //                {
        //                    Tsee.Status = "1";
        //                }


        //                //if (chkworked.Checked == true)
        //                //{
        //                //    Tsee.Worked = "Yes";
        //                //}
        //                //else
        //                //{
        //                //    Tsee.Worked = "No";
        //                //}
        //                if (ddstatus.SelectedIndex == 2)
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                if (ddstatus.SelectedIndex == 1)
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }

        //            if (e.CommandName == "Submit")//6070415004624658
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    // absent(row.RowIndex);
        //                    return;
        //                }
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                //if (chkworked.Checked == false)
        //                //{
        //                //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                //    return;
        //                //}
        //                if (txtRegularStartTime.Text == ",")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularEndTime.Text == "")
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //                }
        //                else
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //                }


        //                //Response.Redirect(Request.RawUrl);
        //            }

        //            if (e.CommandName == "AddBreak")
        //            {
        //                DataTable dt;
        //                GvPopUpEmpList.DataSource = null;
        //                GvPopUpEmpList.DataBind();
        //                txtBreakEndTime.Text = string.Empty;
        //                txtBreakStartTime.Text = string.Empty;
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //                // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

        //                //if (chkworked.Checked == false)
        //                //{
        //                //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //                //    return;
        //                //}
        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //                    return;
        //                }
        //                //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //                lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //                ViewState["datebreak"] = lblDate.Text;
        //                ViewState["weekno"] = hfdweekno.Value;
        //                string regMINst = lblDate.Text + " 00:00";
        //                string regMAXst = lblDate.Text + " 23:59";
        //                txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //                txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //                string regMinEnd = lblDate.Text + " 00:00";
        //                //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        //                string regMAXend = lblDate.Text + " 23:59";

        //                txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //                txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //                // hfddatebreak.Value = lblDate.Text;
        //                // hfdweeknopop.Value = hfdweekno.Value;
        //                //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                //up2.Update();
        //                //GetGridFill();

        //                ModalPopupEmpList.Show();

        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_edit_time_sheet;
        //        #endregion
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw != "7" || role_check_for_add_and_edit_time_sheet_message_throw != "8")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //}

        ////For owner gridview Enter/View Time Sheet hyperlink
        //if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        //{
        //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
        //    DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

        //    //When users are old who are registered before the role permission development
        //    if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
        //    {
        //        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //        GridView GVChildlist = (GridView)sender;

        //        if (e.CommandName == "Savee")
        //        {
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];
        //            TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //            TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //            TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //            TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //            Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                //absent(row.RowIndex);
        //                return;
        //            }
        //            if (ddstatus.SelectedIndex == 1)
        //            {
        //                //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it.
        //                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                absent(row.RowIndex);
        //                return;
        //            }
        //            if (txtRegularStartTime.Text == ",")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }
        //            string startingtime = txtRegularStartTime.Text;
        //            if (startingtime == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }

        //            if (txtRegularEndTime.Text == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                return;
        //            }
        //            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //            if (check == true)
        //            {

        //                Over15x = txtOver15x.Text.Split(',');
        //                Over2x = txtOver2x.Text.Split(',');
        //                holiday = txtholiday.Text.Split(',');
        //                RegularStartTime = txtRegularStartTime.Text.Split(',');
        //                RegularEndTime = txtRegularEndTime.Text.Split(',');
        //            }
        //            else
        //            {
        //                RegularStartTime[0] = txtRegularStartTime.Text;
        //                RegularEndTime[0] = txtRegularEndTime.Text;
        //            }



        //            string[] weekno = hfdweekno.Value.Split(',');
        //            if (Regular[0] == "")
        //            {
        //                Regular[0] = "0";
        //            }
        //            if (Over15x[0] == "")
        //            {
        //                Over15x[0] = "0";
        //            }
        //            if (Over2x[0] == "")
        //            {
        //                Over2x[0] = "0";
        //            }
        //            if (holiday[0] == "")
        //            {
        //                holiday[0] = "0";
        //            }


        //            if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //                return;
        //            }
        //            TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //            if (ToltalRegulahr.TotalHours >= 24)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                return;
        //            }
        //            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //            if (ToltalRegulahr < TotalBreakHr)
        //            {
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                return;
        //            }
        //            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        //            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //            Tsee.WeekNo = int.Parse(weekno[0]);
        //            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //            Tsee.Day = lblDay.Text;
        //            Tsee.Regular = ToltalRegulahr.ToString();
        //            Tsee.Over1_5x = Over15x[0];
        //            Tsee.Over2x = Over2x[0];
        //            Tsee.Holiday = holiday[0];
        //            Tsee.Total = TotalHr.ToString();
        //            Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //            Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //            Tsee.BreakStart = BreakStartTime[0].ToString();
        //            Tsee.BreakEnd = BreakEndTime[0].ToString();
        //            if (hfdStatus.Value == "2")
        //            {
        //                //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //                //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //                //Tsee.Status = "2";

        //                //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //                Tsee.Status = "1";
        //            }
        //            else
        //            {
        //                Tsee.Status = "1";
        //            }
        //            if (ddstatus.SelectedIndex == 2)
        //            {
        //                Tsee.Worked = "Yes";
        //            }
        //            if (ddstatus.SelectedIndex == 1)
        //            {
        //                Tsee.Worked = "No";
        //            }
        //            Tsee.BreakTotal = TotalBreakHr.ToString();
        //            TSC.Sp_InsertTimeSheetEntry(Tsee);

        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //            GetGridFill();


        //        }

        //        if (e.CommandName == "Submit")//6070415004624658
        //        {
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];

        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                return;
        //            }
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            if (txtRegularStartTime.Text == ",")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                return;
        //            }

        //            if (txtRegularEndTime.Text == "")
        //            {
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                return;
        //            }
        //            bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //            if (CheckSaved == true)
        //            {
        //                Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //                GetGridFill();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //            }
        //        }

        //        if (e.CommandName == "AddBreak")
        //        {
        //            DataTable dt;
        //            GvPopUpEmpList.DataSource = null;
        //            GvPopUpEmpList.DataBind();
        //            txtBreakEndTime.Text = string.Empty;
        //            txtBreakStartTime.Text = string.Empty;
        //            int index = Convert.ToInt32(e.CommandArgument);
        //            GridViewRow row = GVChildlist.Rows[index];

        //            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //            if (ddstatus.SelectedIndex == 0)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //                return;
        //            }
        //            lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //            ViewState["datebreak"] = lblDate.Text;
        //            ViewState["weekno"] = hfdweekno.Value;
        //            string regMINst = lblDate.Text + " 00:00";
        //            string regMAXst = lblDate.Text + " 23:59";
        //            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //            string regMinEnd = lblDate.Text + " 00:00";
        //            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        //            string regMAXend = lblDate.Text + " 23:59";

        //            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //            ViewState["dt"] = dt;
        //            GvPopUpEmpList.DataSource = ViewState["dt"];
        //            GvPopUpEmpList.DataBind();

        //            ModalPopupEmpList.Show();
        //        }
        //        return;
        //    }

        //    //When users are new who are registered after the role permission development
        //    string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');

        //    foreach (string role_check_for_add_edit_time_sheet in splittedArray)
        //    {
        //        #region For_Enter_And_Edit_Time_Sheet definition
        //        if (role_check_for_add_edit_time_sheet == "18" || role_check_for_add_edit_time_sheet == "19")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            if (e.CommandName == "Savee")
        //            {


        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];
        //                TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));
        //                TextBox txtOver15x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver15x"));
        //                TextBox txtOver2x = ((TextBox)GVChildlist.Rows[index].FindControl("txtOver2x"));
        //                TextBox txtholiday = ((TextBox)GVChildlist.Rows[index].FindControl("txtholiday"));
        //                Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //                HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));

        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    return;
        //                }
        //                if (ddstatus.SelectedIndex == 1)
        //                {
        //                    //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it.
        //                    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    absent(row.RowIndex);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == ",")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                string startingtime = txtRegularStartTime.Text;
        //                if (startingtime == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularEndTime.Text == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
        //                if (check == true)
        //                {
        //                    Over15x = txtOver15x.Text.Split(',');
        //                    Over2x = txtOver2x.Text.Split(',');
        //                    holiday = txtholiday.Text.Split(',');
        //                    RegularStartTime = txtRegularStartTime.Text.Split(',');
        //                    RegularEndTime = txtRegularEndTime.Text.Split(',');
        //                }
        //                else
        //                {
        //                    RegularStartTime[0] = txtRegularStartTime.Text;
        //                    RegularEndTime[0] = txtRegularEndTime.Text;
        //                }

        //                string[] weekno = hfdweekno.Value.Split(',');
        //                if (Regular[0] == "")
        //                {
        //                    Regular[0] = "0";
        //                }
        //                if (Over15x[0] == "")
        //                {
        //                    Over15x[0] = "0";
        //                }
        //                if (Over2x[0] == "")
        //                {
        //                    Over2x[0] = "0";
        //                }
        //                if (holiday[0] == "")
        //                {
        //                    holiday[0] = "0";
        //                }


        //                if (DateTime.Parse(RegularEndTime[0].ToString()) < DateTime.Parse(RegularStartTime[0].ToString()))
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be smaller than start time', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }
        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //                Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //                Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //                Tsee.WeekNo = int.Parse(weekno[0]);
        //                Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //                Tsee.Day = lblDay.Text;
        //                Tsee.Regular = ToltalRegulahr.ToString();
        //                Tsee.Over1_5x = Over15x[0];
        //                Tsee.Over2x = Over2x[0];
        //                Tsee.Holiday = holiday[0];
        //                Tsee.Total = TotalHr.ToString();
        //                Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //                Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //                Tsee.BreakStart = BreakStartTime[0].ToString();
        //                Tsee.BreakEnd = BreakEndTime[0].ToString();
        //                if (hfdStatus.Value == "2")
        //                {
        //                    //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //                    //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //                    //Tsee.Status = "2";

        //                    //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //                    Tsee.Status = "1";
        //                }
        //                else
        //                {
        //                    Tsee.Status = "1";
        //                }
        //                if (ddstatus.SelectedIndex == 2)
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                if (ddstatus.SelectedIndex == 1)
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }

        //            if (e.CommandName == "Submit")//6070415004624658
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    return;
        //                }
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (txtRegularStartTime.Text == ",")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularEndTime.Text == "")
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //                }
        //                else
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //                }
        //            }

        //            if (e.CommandName == "AddBreak")
        //            {
        //                DataTable dt;
        //                GvPopUpEmpList.DataSource = null;
        //                GvPopUpEmpList.DataBind();
        //                txtBreakEndTime.Text = string.Empty;
        //                txtBreakStartTime.Text = string.Empty;
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //                if (ddstatus.SelectedIndex == 0)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //                    return;
        //                }
        //                lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //                ViewState["datebreak"] = lblDate.Text;
        //                ViewState["weekno"] = hfdweekno.Value;
        //                string regMINst = lblDate.Text + " 00:00";
        //                string regMAXst = lblDate.Text + " 23:59";
        //                txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //                txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //                string regMinEnd = lblDate.Text + " 00:00";
        //                //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        //                string regMAXend = lblDate.Text + " 23:59";

        //                txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //                txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_edit_time_sheet;
        //        #endregion
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw != "18" || role_check_for_add_and_edit_time_sheet_message_throw != "19")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //}
        #endregion




        //string user_type_id = Session["UserTypeid"].ToString();
        //if (user_type_id == "1")
        //{
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        GridView GVChildlist = (GridView)sender;

        //// GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //if (e.CommandName == "Savee")
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
        //    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));
        //    HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
        //    Label lblError = ((Label)GVChildlist.Rows[index].FindControl("lblError"));
        //    Label lblErrorend = ((Label)GVChildlist.Rows[index].FindControl("lblErrorend"));

        //    if (ddstatus.SelectedIndex == 0)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //        // absent(row.RowIndex);
        //        return;
        //    }
        //    if (ddstatus.SelectedIndex == 1)
        //    {
        //        //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it. 
        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //        hfdRIndex.Value = row.RowIndex.ToString();
        //        ModalPopupConfirmSingal.Show();
        //        //absent(row.RowIndex);
        //        return;
        //    }
        //    if (txtRegularStartTime.Text == ",")
        //    {
        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //        lblError.Visible = true;
        //        return;
        //    }
        //    string startingtime = txtRegularStartTime.Text;
        //    if (startingtime == "")
        //    {
        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //        lblError.Visible = true;
        //        return;
        //    }

        //    if (txtRegularEndTime.Text == "")
        //    {
        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //        lblErrorend.Visible = true;
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
        //    if (ToltalRegulahr.TotalHours >= 24)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //        return;
        //    }
        //    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //    if (ToltalRegulahr < TotalBreakHr)
        //    {
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //        return;
        //    }
        //    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
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
        //    if (hfdStatus.Value == "2")
        //    {
        //        //if the timesheet is approved. now we have changed the date and time. now we click on save it was going to approved status.
        //        //for pending and rejected status it will go to pending. for absent will go to absent. earlier it was like that that. 2 is for approved status                    
        //        //Tsee.Status = "2";

        //        //vishnu sir now said, on approved status it will go to pending if we are changing date and time.
        //        Tsee.Status = "1";
        //    }
        //    else
        //    {
        //        Tsee.Status = "1";
        //    }


        //    //if (chkworked.Checked == true)
        //    //{
        //    //    Tsee.Worked = "Yes";
        //    //}
        //    //else
        //    //{
        //    //    Tsee.Worked = "No";
        //    //}
        //    if (ddstatus.SelectedIndex == 2)
        //    {
        //        Tsee.Worked = "Yes";
        //    }
        //    if (ddstatus.SelectedIndex == 1)
        //    {
        //        Tsee.Worked = "No";
        //    }
        //    Tsee.BreakTotal = TotalBreakHr.ToString();
        //    TSC.Sp_InsertTimeSheetEntry(Tsee);

        //    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully, Please Select Approve from the drop down menu', 'success');", true);
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet added successfully', 'success');", true);
        //    GetGridFill();


        //}

        //if (e.CommandName == "Submit")//6070415004624658
        //{
        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];

        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //    if (ddstatus.SelectedIndex == 0)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //        // absent(row.RowIndex);
        //        return;
        //    }
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //    //if (chkworked.Checked == false)
        //    //{
        //    //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //    //    return;
        //    //}
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
        //    bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //    if (CheckSaved == true)
        //    {
        //        Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "");
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet submitted successfully', 'success');", true);
        //    }
        //    else
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'First Save Data Then Click Submit', 'error');", true);
        //    }


        //    //Response.Redirect(Request.RawUrl);
        //}

        if (e.CommandName == "AddBreak")
        {
            try
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
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to add break', 'error');", true);
                return;
            }
            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
            lblbusName.Text = "Add Break on Date: " + lblDate.Text;
            ViewState.Remove("RepairBreakId"); btnAddBreak.Text = "Add";
            ViewState["datebreak"] = lblDate.Text;
            ViewState["weekno"] = hfdweekno.Value;
            string regMINst = lblDate.Text + " 00:00";
            string regMAXst = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            string regMinEnd = lblDate.Text + " 00:00";
            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
            string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
            // hfddatebreak.Value = lblDate.Text;
            // hfdweeknopop.Value = hfdweekno.Value;
            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
            {
                dt = AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue), DateTime.Parse(lblDate.Text));
            }
            else
            {
                dt = AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue), DateTime.Parse(lblDate.Text));
            }

            ViewState["dt"] = dt;
            GvPopUpEmpList.DataSource = ViewState["dt"];
            GvPopUpEmpList.DataBind();
            //up2.Update();
            //GetGridFill();

            ModalPopupEmpList.Show();

            return;
            }
            catch (InvalidOperationException ex)
            {
                log4net.LogManager.GetLogger("Log").Warn("Open break dialog blocked: " + ex.Message);
                RepairMessage(ex.Message, true);
                return;
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger("Log").Error("Open break dialog failed", ex);
                RepairMessage("Break entry could not be opened. Refresh the week and try again. If it still fails, contact support.", true);
                return;
            }
        }

        //    return;
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
            //CheckBox chkworked = e.Row.FindControl("chkworked") as CheckBox;
            //CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;

            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            LinkButton dropdownMenu1 = e.Row.FindControl("dropdownMenu1") as LinkButton;
            LinkButton dropdownMenu2 = e.Row.FindControl("dropdownMenu2") as LinkButton;

            //lblDate.Text = DateTime.Parse(lblDate.Text).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            lblDate.Text = DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            DropDownList ddstatus = e.Row.FindControl("ddstatus") as DropDownList;
            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";
                lnlGvAbsent.Enabled = false;
                txtRegularStartTime.ReadOnly = false;
                txtRegularEndTime.ReadOnly = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                //txtRegularStartTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //ddstatus.Enabled = false;

                lnkSave.Enabled = false;
                lnkSave.Visible = false;

                dropdownMenu1.Text = "Pending";
                dropdownMenu2.Text = "Pending";
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                lnlGvAbsent.Enabled = false;
                lnkAddBreak.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                ddstatus.Enabled = false;

                //e.Row.BackColor = System.Drawing.Color.LightGray;
                //txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                //txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                //ddstatus.BackColor = System.Drawing.Color.LightGray;
                e.Row.BackColor = System.Drawing.Color.FromName("#f5f5f5");

                e.Row.Attributes.Add("BackColor", "#f5f5f5;");

                lnkSave.Enabled = false;
                lnkSave.Visible = false;

                dropdownMenu1.Text = hfdWorked.Value.Trim() == "No" ? "Approved (Absent)" : "Approved";
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                lnlGvAbsent.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                //txtRegularStartTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //ddstatus.Enabled = false;

                lnkSave.Enabled = false;
                lnkSave.Visible = false;

                dropdownMenu1.Text = "Rejected";
            }
            else if (hfdStatus.Value == "4")
            {
                lblStatus.Text = "Absent";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;

                //txtRegularStartTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //ddstatus.Enabled = false;
                txtRegularStartTime.ReadOnly = false;
                txtRegularEndTime.ReadOnly = false;
                ddstatus.Enabled = true;

                //e.Row.BackColor = System.Drawing.Color.LightGray;
                //txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                //txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                //ddstatus.BackColor = System.Drawing.Color.LightGray;
                e.Row.BackColor = System.Drawing.Color.FromName("#f5f5f5");

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                lnkGvApp.Enabled = false;
                lnkGvReject.Enabled = false;

                ddstatus.SelectedIndex = 1;

                dropdownMenu1.Text = "Absent";
                dropdownMenu2.Text = "Absent";
            }

            if (hfdWorked.Value.Trim() == "Yes")
            {
                //chkworked.Checked = true;
                ddstatus.SelectedIndex = 2;
            }
            else
            {
                ddstatus.SelectedIndex = 1;
                //ddstatus.SelectedIndex = 0;
            }
            //if (hfdWorked.Value.Trim() == "No")
            //{
            //    ddstatus.SelectedIndex = 1;
            //}

            string regMINst = lblDate.Text + " 00:00";
            string regMAXst = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            string regMinEnd = lblDate.Text + " 00:00";
            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";//commented to remove dd from end date
            string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));



            DataTable DataTable_chk_hiredate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + DateTime.Parse(lblDate.Text) + "')", "");
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


            DataTable DataTable_chk_enddate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "EndDate,Is_Having_EndDate", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + ddlemp.SelectedValue + ")", "");
            if (DataTable_chk_enddate.Rows.Count > 0 && DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"].ToString() == "True")
            {
                if (DateTime.Parse(lblDate.Text) > DateTime.Parse(DataTable_chk_enddate.Rows[0]["EndDate"].ToString()))
                {
                    e.Row.Enabled = false;
                }
            }
            if (DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"].ToString() == "False")
            {
                e.Row.Enabled = true;
            }
        }

        int absentdays = 0;
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            HiddenField hfdStatus = row.FindControl("hfdStatus") as HiddenField;
            Label lblnetTotal = row.FindControl("lblnetTotal") as Label;
            if (hfdStatus.Value == "1")
                absentdays = absentdays + 1;
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
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
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
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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
            //Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4");
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
        if(e.CommandName == "DeleteRow") lnkdelete_Click(e.CommandSource, EventArgs.Empty);
        else if(e.CommandName == "EditRow") lnkEditBreak_Click(e.CommandSource, EventArgs.Empty);
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
        try
        {
            var row = (GridViewRow)((LinkButton)sender).NamingContainer;
            int id = int.Parse(((HiddenField)row.FindControl("brkid")).Value);
            AnyminuteTimesheetRepair.ChangeBreak(Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]),
                RepairBusinessId, int.Parse(ddlemp.SelectedValue), DateTime.Parse(Convert.ToString(ViewState["datebreak"])),
                id, null, null, true);
            ViewState.Remove("RepairBreakId"); btnAddBreak.Text = "Add";
            RefreshRepairBreaks();
        }
        catch(InvalidOperationException ex) { RepairMessage(ex.Message, true); ModalPopupEmpList.Show(); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Delete break failed", ex); RepairMessage("Break was not deleted. Refresh and check the timecard lock or contact support.", true); }
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
                //string msg = "For these " + lblDate.Text + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
                //string msg = "For the selected date, time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?";
                //Label3.Text = msg;
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
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet got approved', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
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
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        }
    }

    protected void btnNo_Click(object sender, EventArgs e)
    {
        ModalPopupConfirm.Hide();
    }

    protected void lnkbtnYesSingal_Click(object sender, EventArgs e)
    {
        //copmmented after giving message for absent in dec 2023 after telling by Vishnu Sir
        //approvedTimeSheet();

        //int RIndex = int.Parse(hfdRIndex.Value);
        //HiddenField HiddenField = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        //Label lblDate = (Label)GVChildlist.Rows[RIndex].FindControl("lblDate");
        //DateTime d = DateTime.Parse(lblDate.Text);
        //TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularStartTime") as TextBox;
        //TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularEndTime") as TextBox;
        //LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkAddBreak") as LinkButton;
        //LinkButton lnkSave = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkSave") as LinkButton;
        //int a;

        //bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and TimeSheetDate=" + d.ToString("yyyy-MM-dd") + "");
        //if (check == true)
        //{
        //    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
        //    if (a > 0)
        //    {
        //        GetGridFill();

        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        //    }
        //}
        //else
        //{

        //    HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");

        //    Label lblDay = (Label)GVChildlist.Rows[RIndex].FindControl("lblDay");
        //    string[] weekno = hfdweekno.Value.Split(',');
        //    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //    Tsee.WeekNo = int.Parse(weekno[0]);
        //    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
        //    Tsee.Day = lblDay.Text;
        //    Tsee.Regular = "";
        //    Tsee.Over1_5x = "";
        //    Tsee.Over2x = "";
        //    Tsee.Holiday = "";
        //    Tsee.Total = "";
        //    Tsee.ReqularStart = null;
        //    Tsee.Regularend = null;
        //    Tsee.BreakStart = "";
        //    Tsee.BreakEnd = "";

        //    Tsee.Worked = "No";
        //    Tsee.BreakTotal = "";
        //    Tsee.Status = "4";
        //    TSC.Sp_InsertTimeSheetEntry(Tsee);
        //    a = 1;
        //    if (a > 0)
        //    {
        //        GetGridFill();

        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        //    }
        //}


        int RIndex = int.Parse(hfdRIndex.Value);
        HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RIndex].FindControl("lblDate");
        Label lblDay = (Label)GVChildlist.Rows[RIndex].FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        GetGridFill();
        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is marked as Absent.', 'success');", true);
    }

    protected void lnkbtnNoSingal_Click(object sender, EventArgs e)
    {
        ModalPopupConfirmSingal.Hide();
    }



    protected void ddlAction_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlemp.SelectedValue == "")
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Employee', 'warning');", true);
            ModalPopupEmployee.Show();
            ddlAction.SelectedValue = "";
            return;
        }

        int chk_checkboxselected = 0;

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            if (chkApproved.Checked)
            {
                chk_checkboxselected = 1;
                break;
            }
        }

        if (chk_checkboxselected == 0)
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please check the Checkbox in Grid', 'warning');", true);
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select the Date to Approve or Reject time sheet', 'warning');", true);
            ModalPopupApproveOrReject.Show();
            ddlAction.SelectedValue = "";
            return;
        }

        if (ddlAction.SelectedValue == "A")
        {
            //if (ddlemp.SelectedValue == "")
            //{
            //    ScriptManager.RegisterClientScriptBlock(
            //             this.Page,
            //             this.GetType(),
            //             "SweetAlert",
            //             "swal('Error!', 'Please select employee', 'error');",
            //             true);
            //    return;
            //}

            foreach (GridViewRow row in GVChildlist.Rows)
            {
                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
                DropDownList ddstatus = (DropDownList)row.FindControl("ddstatus");
                // if (chkApproved.Checked)
                {
                    //if (ddstatus.SelectedValue == "SELECT" || ddstatus.SelectedValue == "No")
                    //{
                    //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Worked as Yes', 'warning');", true);
                    //    ddlAction.SelectedValue = "";
                    //    return;
                    //}
                    //if (txtRegularStartTime.Text.Trim().Length == 0)
                    //{
                    //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
                    //    ddlAction.SelectedValue = "";
                    //    return;
                    //}
                    //if (txtRegularEndTime.Text.Trim().Length == 0)
                    //{
                    //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
                    //    ddlAction.SelectedValue = "";
                    //    return;
                    //}

                    if (ddstatus.SelectedValue == "Yes")
                    {
                        if (txtRegularStartTime.Text.Trim().Length == 0)
                        {
                            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
                            ModalPopupStartDate.Show();
                            ddlAction.SelectedValue = "";
                            return;
                        }

                        if (txtRegularEndTime.Text.Trim().Length == 0)
                        {
                            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
                            ModalPopupEndDate.Show();
                            ddlAction.SelectedValue = "";
                            return;
                        }
                    }

                    if (txtRegularStartTime.Text == string.Empty && txtRegularEndTime.Text == string.Empty && ddstatus.SelectedValue == "No")
                    {
                        //string msg = "For these " + lblDate.Text + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
                        //string msg = "For the selected date, time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?";
                        //Label3.Text = msg;
                        ddlAction.SelectedValue = "";
                        //hfdRIndex.Value = gvr.RowIndex.ToString();
                        ModalPopupConfirmSingal.Show();
                        return;
                    }

                    if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length == 0)
                    { }

                    if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length != 0)
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
                        ModalPopupStartDate.Show();
                        ddlAction.SelectedValue = "";
                        return;
                    }

                    if (txtRegularStartTime.Text.Trim().Length != 0 && txtRegularEndTime.Text.Trim().Length == 0)
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
                        ModalPopupEndDate.Show();
                        ddlAction.SelectedValue = "";
                        return;
                    }


                    if (txtRegularStartTime.Text.Trim().Length > 0 && txtRegularEndTime.Text.Trim().Length > 0)
                    {
                        DateTime dtfrom = DateTime.Parse(txtRegularStartTime.Text);
                        DateTime dtto = DateTime.Parse(txtRegularEndTime.Text);
                        if (dtfrom > dtto)
                        {
                            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start Date is Greater than End Date', 'warning');", true);
                            ModalPopupStarDateGreaterEnd.Show();
                            ddlAction.SelectedValue = "";
                            return;
                        }
                        string[] RegularStartTime = { "" }, RegularEndTime = { "" };
                        RegularStartTime[0] = txtRegularStartTime.Text;
                        RegularEndTime[0] = txtRegularEndTime.Text;
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Start Time and End Time Must be within 24Hrs', 'warning');", true);
                            ModalPopupStartTimeMust.Show();
                            ddlAction.SelectedValue = "";
                            return;
                        }
                    }
                }
            }

            approvedTimeSheet();
        }
        else if (ddlAction.SelectedValue == "R")
        {
            //if (ddlemp.SelectedValue == "")
            //{
            //    ScriptManager.RegisterClientScriptBlock(
            //             this.Page,
            //             this.GetType(),
            //             "SweetAlert",
            //             "swal('Error!', 'Please select employee', 'error');",
            //             true);
            //    return;
            //}

            foreach (GridViewRow row in GVChildlist.Rows)
            {
                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
                DropDownList ddstatus = (DropDownList)row.FindControl("ddstatus");
                if (chkApproved.Checked)
                {
                    if (ddstatus.SelectedValue == "SELECT" || ddstatus.SelectedValue == "No")
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Worked as Yes', 'warning');", true);
                        ModalPopupWorked.Show();
                        ddlAction.SelectedValue = "";
                        return;
                    }
                    if (txtRegularStartTime.Text.Trim().Length == 0)
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
                        ModalPopupStartDate.Show();
                        ddlAction.SelectedValue = "";
                        return;
                    }
                    if (txtRegularEndTime.Text.Trim().Length == 0)
                    {
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
                        ModalPopupEndDate.Show();
                        ddlAction.SelectedValue = "";
                        return;
                    }
                    if (txtRegularStartTime.Text.Trim().Length > 0 && txtRegularEndTime.Text.Trim().Length > 0)
                    {
                        DateTime dtfrom = DateTime.Parse(txtRegularStartTime.Text);
                        DateTime dtto = DateTime.Parse(txtRegularEndTime.Text);
                        if (dtfrom > dtto)
                        {
                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start Date is Greater than End Date', 'warning');", true);
                            ddlAction.SelectedValue = "";
                            return;
                        }
                        string[] RegularStartTime = { "" }, RegularEndTime = { "" };
                        RegularStartTime[0] = txtRegularStartTime.Text;
                        RegularEndTime[0] = txtRegularEndTime.Text;
                        TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
                        if (ToltalRegulahr.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Start Time and End Time Must be within 24Hrs', 'warning');", true);
                            ddlAction.SelectedValue = "";
                            return;
                        }
                    }
                }
            }

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

                    if (saveFlag == -1) a++;

                    else if (saveFlag > 0)
                        Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");

                    a++;
                }
            }
            if (a > 0)
            {
                GetGridFill();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                        int saveFlag = 0;
                        saveFlag = SaveTimesheet(row, isRejected: true);

                        if (saveFlag == -1) a++;

                        else if (saveFlag > 0)
                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    }
                }
                if (a > 0)
                {
                    GetGridFill();
                    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                            int saveFlag = 0;
                            saveFlag = SaveTimesheet(row, isRejected: true);

                            if (saveFlag == -1) a++;

                            else if (saveFlag > 0)
                                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        }
                    }
                    if (a > 0)
                    {
                        GetGridFill();
                        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                            int saveFlag = 0;
                            saveFlag = SaveTimesheet(row, isRejected: true);

                            if (saveFlag == -1) a++;

                            else if (saveFlag > 0)
                                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        }
                    }
                    if (a > 0)
                    {
                        GetGridFill();
                        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                                int saveFlag = 0;
                                saveFlag = SaveTimesheet(row, isRejected: true);

                                if (saveFlag == -1) a++;

                                else if (saveFlag > 0)
                                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                            }
                        }
                        if (a > 0)
                        {
                            GetGridFill();
                            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                        int saveFlag = 0;
                        saveFlag = SaveTimesheet(row, isRejected: true);

                        if (saveFlag == -1) a++;

                        else if (saveFlag > 0)
                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    }
                }
                if (a > 0)
                {
                    GetGridFill();
                    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

                                int saveFlag = 0;
                                saveFlag = SaveTimesheet(row, isRejected: true);

                                if (saveFlag == -1) a++;

                                else if (saveFlag > 0)
                                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                            }
                        }
                        if (a > 0)
                        {
                            GetGridFill();
                            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
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

    protected void LinkButton_Edit_Timesheet_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdStatus = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus");
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime");
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime");
        DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus");
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave");
        if (hfdStatus.Value == "1" || hfdStatus.Value == "2" || hfdStatus.Value == "3" || hfdStatus.Value == "4")
        {
            txtRegularStartTime.ReadOnly = false;
            txtRegularEndTime.ReadOnly = false;
            ddstatus.Enabled = true;

            lnkSave.Enabled = true;
        }
    }

    protected void LinkButton_CopyNextWeek_Click(object sender, EventArgs e)
    { CopyRepairWeek(true); }

    protected void LinkButton_CopyCurrentWeek_Click(object sender, EventArgs e)
    { CopyRepairWeek(false); }

    protected void Ddl_Business_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Ddl_Business.SelectedValue != "")
        {
            lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business.SelectedValue));

            ddlemp.Items.Clear();
            DataTable Get_Employee_Name_In_Ddl = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID ",
                "AE.EmployeeID, DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName, ASSB.BusinessID, AE.EMPLOYEETYPE, case when AE.EMPLOYEETYPE='R' then 'Regular' else 'Project' end as Employee_Type ",
                "ASSB.BusinessID=" + Ddl_Business.SelectedValue + " and Assb.UserTypeId not in (1,5,12) and AE.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and AE.EMPLOYEETYPE='R' ",
                "AE.EmployeeID");
            ddlemp.Items.Add("Please Select");
            ddlemp.SelectedItem.Value = "";
            ddlemp.DataSource = Get_Employee_Name_In_Ddl;
            ddlemp.DataValueField = Get_Employee_Name_In_Ddl.Columns[0].ToString();
            ddlemp.DataTextField = Get_Employee_Name_In_Ddl.Columns[1].ToString();
            ddlemp.DataBind();

            GVChildlist.DataSource = null;
            GVChildlist.DataBind();

            Label_Selected_EmpName.Text = "";
            Label_HireDate.Text = "";
            Label_Seperation_.Visible = false;
            Label_Seperation_1.Visible = false;
            Label_Seperation_2.Visible = false;
            lblCurrentRate.Text = "";
        }
        if (Ddl_Business.SelectedValue == "")
        {
            lblbid.Text = "";
            GVChildlist.DataSource = null;
            GVChildlist.DataBind();
            ddlemp.Items.Clear();
            ddlemp.Items.Add("Please Select");
            ddlemp.SelectedItem.Value = "";

            Label_Selected_EmpName.Text = "";
            Label_HireDate.Text = "";
            Label_Seperation_.Visible = false;
            Label_Seperation_1.Visible = false;
            Label_Seperation_2.Visible = false;
            lblCurrentRate.Text = "";

            Label_Approved_Hours.Text = "";
            Label_Rejected_Hours.Text = "";
            Label_Pending_Hours.Text = "";
        }
    }

    public void Get_Business_Name_List()
    {
        //DataTable Get_Business_Name_In_Ddl = Comm.GetAllDataFromTable("AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID,(ab.CompanyName)BusinessName", "EmployeeID=" + Session["EmployeeId"].ToString() + " and ab.Status=1", "ab.BusinessID");
        //Ddl_Business.Items.Add("Please Select");
        //Ddl_Business.SelectedItem.Value = "";
        //Ddl_Business.DataSource = Get_Business_Name_In_Ddl;
        //Ddl_Business.DataValueField = Get_Business_Name_In_Ddl.Columns[0].ToString();
        //Ddl_Business.DataTextField = Get_Business_Name_In_Ddl.Columns[1].ToString();
        //Ddl_Business.DataBind();

        //Ddl_Business.SelectedValue = Request.QueryString["BusinessIdFromDefaultPage"].ToString();
        //lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business.SelectedValue));
    }

    protected void ddl_Action_SelectedIndexChanged(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        DropDownList ddl_Action = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddl_Action.NamingContainer;
        if (ddl_Action.SelectedIndex == 1) //Approve
        {
            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Text);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
            TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
            TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
            TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
            Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
            Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
            Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
            Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));

            if (ddstatus.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
                ddl_Action.SelectedIndex = 0;
                return;
            }
            if (ddstatus.SelectedIndex == 1)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
                ddl_Action.SelectedIndex = 0;
                return;
            }
            if (txtRegularStartTime.Text == ",")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                lblError.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }
            string startingtime = txtRegularStartTime.Text;
            if (startingtime == "")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                lblError.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }

            if (txtRegularEndTime.Text == "")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                lblErrorend.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }

            bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
            if (check)
            {
                Over15x = txtOver15x.Text.Split(',');
                Over2x = txtOver2x.Text.Split(',');
                holiday = txtholiday.Text.Split(',');
                RegularStartTime = txtRegularStartTime.Text.Split(',');
                RegularEndTime = txtRegularEndTime.Text.Split(',');

                //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
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
            Tsee.Status = "1";

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

            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
            }
        }
        if (ddl_Action.SelectedIndex == 2) //Reject
        {
            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Text);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
            TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
            TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
            TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
            Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
            Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
            Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
            Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));

            txtRegularStartTime.ReadOnly = false;
            txtRegularEndTime.ReadOnly = false;
            ddstatus.Enabled = true;

            if (ddstatus.SelectedIndex == 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
                ddl_Action.SelectedIndex = 0;
                return;
            }
            if (ddstatus.SelectedIndex == 1)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
                ddl_Action.SelectedIndex = 0;
                return;
            }
            if (txtRegularStartTime.Text == ",")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                lblError.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }
            string startingtime = txtRegularStartTime.Text;
            if (startingtime == "")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                lblError.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }

            if (txtRegularEndTime.Text == "")
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                lblErrorend.Visible = true;
                ddl_Action.SelectedIndex = 0;
                return;
            }

            bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
            if (check)
            {
                Over15x = txtOver15x.Text.Split(',');
                Over2x = txtOver2x.Text.Split(',');
                holiday = txtholiday.Text.Split(',');
                RegularStartTime = txtRegularStartTime.Text.Split(',');
                RegularEndTime = txtRegularEndTime.Text.Split(',');

                //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
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
            Tsee.Status = "1";

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

            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is rejected successfully.', 'success');", true);
            }
        }
        if (ddl_Action.SelectedIndex == 3) //Absent
        {
            hfdRIndex.Value = gvr.RowIndex.ToString();
            ModalPopupConfirmSingal.Show();
            ddl_Action.SelectedIndex = 0;
        }
        if (ddl_Action.SelectedIndex == 4) //Modify
        {
            Label_Selected_Emp_Name_Popup.Text = "Modify Time sheet for " + ddlemp.SelectedItem.Text;

            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");

            DataTable Dt_check_Time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                "and EmpId='" + ddlemp.SelectedValue + "' and BussID = '" + int.Parse(Ddl_Business.SelectedValue) + "' ", "");
            if (Dt_check_Time.Rows.Count > 0)
            {
                TextBox1.Text = DateTime.Parse(Dt_check_Time.Rows[0]["ReqularStart"].ToString()).ToString("yyyy-MM-ddTHH:mm");
                TextBox2.Text = DateTime.Parse(Dt_check_Time.Rows[0]["Regularend"].ToString()).ToString("yyyy-MM-ddTHH:mm");
            }
            hfdRIndex.Value = gvr.RowIndex.ToString();
            ModalPopup_NewAction.Show();
            ddl_Action.SelectedIndex = 0;
        }
        if (ddl_Action.SelectedIndex == 5) //Delete
        {
            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Text);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
            TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
            TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
            TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
            Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
            Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
            Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
            Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));
            string[] weekno = hfdweekno.Value.Split(',');
            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
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

            Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' ");
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is deleted successfully.', 'success');", true);
            ddl_Action.SelectedIndex = 0;
        }
    }

    protected void LinkButton_btnYesSingal_Click(object sender, EventArgs e)
    {
        GetGridFill();
    }

    protected void LinkButton_btnNoSingal_Click(object sender, EventArgs e)
    {
        ModalPopupPnlConfirmEmpSelection.Hide();
    }

    protected void Button_Save_Action_popup_Click(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        int RIndex = int.Parse(hfdRIndex.Value);
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[RIndex].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[RIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[RIndex].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[RIndex].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[RIndex].FindControl("lblErrorend"));

        txtRegularStartTime.ReadOnly = false;
        txtRegularEndTime.ReadOnly = false;
        ddstatus.Enabled = true;

        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
            return;
        }
        if (ddstatus.SelectedIndex == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
            return;
        }
        //if (txtRegularStartTime.Text == ",")
        //{
        //    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //    lblError.Visible = true;
        //    return;
        //}
        //string startingtime = txtRegularStartTime.Text;
        //if (startingtime == "")
        //{
        //    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //    lblError.Visible = true;
        //    return;
        //}

        //if (txtRegularEndTime.Text == "")
        //{
        //    //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //    lblErrorend.Visible = true;
        //    return;
        //}

        if (TextBox1.Text == ",")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            //lblError.Visible = true;
            return;
        }
        string startingtime = TextBox1.Text;
        if (startingtime == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            //lblError.Visible = true;
            return;
        }

        if (TextBox2.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
            //lblErrorend.Visible = true;
            return;
        }

        bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
        if (check)
        {
            Over15x = txtOver15x.Text.Split(',');
            Over2x = txtOver2x.Text.Split(',');
            holiday = txtholiday.Text.Split(',');
            RegularStartTime = TextBox1.Text.Split(',');
            RegularEndTime = TextBox2.Text.Split(',');

            //BreakStartTime = txtBreakStartTime.Text.Split(',');
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

            RegularStartTime[0] = TextBox1.Text;
            RegularEndTime[0] = TextBox2.Text;

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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        Tsee.Status = "1";

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
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is modified successfully. Please approve it.', 'success');", true);
        GetGridFill();

        TextBox1.Text = "";
        TextBox2.Text = "";

        string startingtime_popup = txtRegularStartTime.Text;
        if (startingtime_popup != "")
        {
            TextBox1.Text = "";
        }

        if (txtRegularEndTime.Text != "")
        {
            TextBox2.Text = "";
        }
    }

    protected void LinkButton_Approve_Click(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        LinkButton LinkButton_Approve = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Approve.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));

        //txtRegularStartTime.ReadOnly = false;
        //txtRegularEndTime.ReadOnly = false;
        //ddstatus.Enabled = true;

        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
            return;
        }
        if (ddstatus.SelectedIndex == 1)
        {
            ReviewAbsentAndRefresh(gvr, 2);
            return;
        }
        if (txtRegularStartTime.Text == ",")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }
        string startingtime = txtRegularStartTime.Text;
        if (startingtime == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }

        if (txtRegularEndTime.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
            lblErrorend.Visible = true;
            return;
        }

        bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
        if (check)
        {
            Over15x = txtOver15x.Text.Split(',');
            Over2x = txtOver2x.Text.Split(',');
            holiday = txtholiday.Text.Split(',');
            RegularStartTime = txtRegularStartTime.Text.Split(',');
            RegularEndTime = txtRegularEndTime.Text.Split(',');

            //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        Tsee.Status = "1";

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

        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null)
        {
            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                    "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                GetGridFill();
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
            }
        }

        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] == null)
        {
            DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
            "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
            if (Dt_Get_UserType.Rows.Count > 0)
            {
                if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "2")
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is added successfully.', 'success');", true);
                }
                if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "2")
                {
                    int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                        "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    if (a > 0)
                    {
                        GetGridFill();
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
                    }
                }
            }
        }
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] != null)
        {
            DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId,EmployeeID from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
            "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
            if (Dt_Get_UserType.Rows.Count > 0)
            {
                if (Dt_Get_UserType.Rows[0]["EmployeeID"].ToString() == Request.QueryString["MangerId"].ToString())
                {
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is added successfully.', 'success');", true);
                }
                else
                {
                    int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                        "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    if (a > 0)
                    {
                        GetGridFill();
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
                    }
                }
            }
        }

        //int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
        //if (a > 0)
        //{
        //    GetGridFill();
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
        //}
    }

    protected void LinkButton_Reject_Click(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        LinkButton LinkButton_Reject = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Reject.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));

        //txtRegularStartTime.ReadOnly = false;
        //txtRegularEndTime.ReadOnly = false;
        //ddstatus.Enabled = true;

        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
            return;
        }
        if (ddstatus.SelectedIndex == 1)
        {
            ReviewAbsentAndRefresh(gvr, 3);
            return;
        }
        if (txtRegularStartTime.Text == ",")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }
        string startingtime = txtRegularStartTime.Text;
        if (startingtime == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }

        if (txtRegularEndTime.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
            lblErrorend.Visible = true;
            return;
        }

        bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
        if (check)
        {
            Over15x = txtOver15x.Text.Split(',');
            Over2x = txtOver2x.Text.Split(',');
            holiday = txtholiday.Text.Split(',');
            RegularStartTime = txtRegularStartTime.Text.Split(',');
            RegularEndTime = txtRegularEndTime.Text.Split(',');

            //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        Tsee.Status = "1";

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

        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is rejected successfully.', 'success');", true);
        }
    }

    protected void LinkButton_Absent_Click(object sender, EventArgs e)
    {
        LinkButton LinkButton_Absent = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Absent.NamingContainer;
        hfdRIndex.Value = gvr.RowIndex.ToString();
        ModalPopupConfirmSingal.Show();
    }

    protected void LinkButton_Modify_Click(object sender, EventArgs e)
    {
        TextBox1.Text = "";
        TextBox2.Text = "";

        LinkButton LinkButton_Modify = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Modify.NamingContainer;
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");

        DataTable Dt_check_Time = new DataTable();
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Dt_check_Time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
            "and EmpId='" + ddlemp.SelectedValue + "' and BussID = '" + int.Parse(Ddl_Business.SelectedValue) + "' ", "");
        }
        else
        {
            Dt_check_Time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
            "and EmpId='" + ddlemp.SelectedValue + "' and BussID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"]) + "' ", "");
        }

        //DataTable Dt_check_Time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
        //    "and EmpId='" + ddlemp.SelectedValue + "' and BussID = '" + int.Parse(Ddl_Business.SelectedValue) + "' ", "");
        if (Dt_check_Time.Rows.Count > 0 && Dt_check_Time.Rows[0]["ReqularStart"].ToString() != "")
        {
            TextBox1.Text = DateTime.Parse(Dt_check_Time.Rows[0]["ReqularStart"].ToString()).ToString("yyyy-MM-ddTHH:mm");
            TextBox2.Text = DateTime.Parse(Dt_check_Time.Rows[0]["Regularend"].ToString()).ToString("yyyy-MM-ddTHH:mm");

            //string regMINst = lblDate.Text + " 00:00";
            //string regMAXst = lblDate.Text + " 23:59";
            //TextBox1.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            //TextBox1.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            //string regMinEnd = lblDate.Text + " 00:00";
            ////string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
            //string regMAXend = lblDate.Text + " 23:59";
            //TextBox2.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            //TextBox2.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        }

        string regMINst = lblDate.Text + " 00:00";
        string regMAXst = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        TextBox1.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        TextBox1.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        string regMinEnd = lblDate.Text + " 00:00";
        //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
        TextBox2.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        TextBox2.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));

        hfdRIndex.Value = gvr.RowIndex.ToString();
        Label_Selected_Emp_Name_Popup.Text = "Modify Time sheet for " + ddlemp.SelectedItem.Text;
        ModalPopup_NewAction.Show();
    }

    protected void LinkButton_Delete_Click(object sender, EventArgs e)
    {
        LinkButton LinkButton_Delete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Delete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblErrorend"));
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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

        Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' ");
        Comm.ExecuteQuery("delete from BreakTime where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' ");
        Label_Approved_Hours.Text = "";
        Label_Rejected_Hours.Text = "";
        Label_Pending_Hours.Text = "";
        GetGridFill();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is deleted successfully.', 'success');", true);
    }

    protected void CheckAll(object sender, EventArgs e)
    {
        CheckBox chckheader = (CheckBox)GVChildlist.HeaderRow.FindControl("chkSelectAll1");
        foreach (GridViewRow row in GVChildlist.Rows)
        {
            System.Web.UI.HtmlControls.HtmlInputCheckBox chckrw = (System.Web.UI.HtmlControls.HtmlInputCheckBox)row.Cells[0].FindControl("chkSelect1");
            if (chckheader.Checked == true)
            {
                chckrw.Checked = true;

            }
            else
            {
                chckrw.Checked = false;
            }

        }
    }

    protected void checkSelect1_OnClick(object sender, EventArgs e)
    {
        GridViewRow row = ((GridViewRow)((CheckBox)sender).NamingContainer);
        int index = row.RowIndex;
        CheckBox cb1 = (CheckBox)GVChildlist.Rows[index].FindControl("CheckBox1");

        if (cb1.Checked == false)
        {
            CheckBox chckheader = (CheckBox)GVChildlist.HeaderRow.FindControl("chkSelectAll1");
            chckheader.Checked = false;
        }
    }
    protected void approveSelectedTimesheetRows(object sender, EventArgs e)
    {
        //int i=0;
       //System.Web.UI.HtmlControls.HtmlInputCheckBox cb1; 
        int j = 0;
        //System.Web.UI.HtmlControls.HtmlInputCheckBox cb1;
        //foreach (GridViewRow row in GVChildlist.Rows)
        //System.Web.UI.HtmlControls.HtmlInputCheckBox cb1;


        for (int i=0; i< GVChildlist.Rows.Count; i++)
        {
          CheckBox  cb1 = GVChildlist.Rows[i].FindControl("chkSelect1") as CheckBox;
            HiddenField hdn = GVChildlist.Rows[i].Cells[0].FindControl("Hidden2") as HiddenField;
            // cb1 = (System.Web.UI.HtmlControls.HtmlInputCheckBox)GVChildlist.Rows[i].Cells[0].FindControl("chkSelect1");
            //string val1= GVChildlist.Rows[i].Cells[0].te
            //int intChecked = int.Parse(cb1.Value);
            //LinkButton btn = (LinkButton)sender;
            string Val = Request.Form[hdn.ClientID];
            //bool isChecked = Request.Form[cb1] != null;
                if ( cb1.Checked)

             {
                //  ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);

                approveTimesheet(i);
               
                j = j + 1;
            }
            
        }
        if (j > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);

        }
    }
    protected void rejectSelectedTimesheetRows(object sender, EventArgs e)
    {
        //int i=0;
        int j = 0;
        CheckBox cb1;
        //foreach (GridViewRow row in GVChildlist.Rows)
         for (int i=0; i< GVChildlist.Rows.Count; i++)
        {

            cb1 = (CheckBox)GVChildlist.Rows[i].Cells[0].FindControl("chkSelect1");
            //string val1= GVChildlist.Rows[i].Cells[0].te
            //int intChecked = int.Parse(cb1.Value);
            //LinkButton btn = (LinkButton)sender;
            string val = cb1.Attributes["value"];
            if (cb1.Checked )

            { 
                rejectTimesheet(i);
               
                j = j + 1;
            }
        }
        if (j > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is rejected successfully.', 'success');", true);

        }
    }
    protected void approveTimesheet(int i)
    {



        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        //LinkButton LinkButton_Approve = (LinkButton)sender;
        //GridViewRow gvr = (GridViewRow)LinkButton_Approve.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[i].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[i].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[i].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[i].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[i].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[i].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[i].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[i].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[i].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[i].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[i].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[i].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[i].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[i].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[i].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[i].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[i].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[i].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[i].FindControl("lblErrorend"));
        Label lblStaus = ((Label)GVChildlist.Rows[i].FindControl("lblStatus"));
        //txtRegularStartTime.ReadOnly = false;
        //txtRegularEndTime.ReadOnly = false;
        //ddstatus.Enabled = true;

        if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
            return;
        }
        if (ddstatus.SelectedIndex == 1)
        {
            //it will be absent now for No status. removed approve reject dropdown from top. vishnu sir said it. 
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
            hfdRIndex.Value = i.ToString();// gvr.RowIndex.ToString();
                                           //  ModalPopupConfirmSingal.Show();
                                           //absent(row.RowIndex);
            //return;
        }
        if (txtRegularStartTime.Text == ",")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }
        string startingtime = txtRegularStartTime.Text;
        if (startingtime == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }

        if (txtRegularEndTime.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
            lblErrorend.Visible = true;
            return;
        }

        bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
        if (check)
        {
            Over15x = txtOver15x.Text.Split(',');
            Over2x = txtOver2x.Text.Split(',');
            holiday = txtholiday.Text.Split(',');
            RegularStartTime = txtRegularStartTime.Text.Split(',');
            RegularEndTime = txtRegularEndTime.Text.Split(',');

            //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
            // GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
            return;
        }
        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        Tsee.Status = "2";

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

        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null)
        {
            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                    "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (a > 0)
            {
                //   GetGridFill();
                // ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
            }
        }

        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] == null)
        {
            DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
            "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
            if (Dt_Get_UserType.Rows.Count > 0)
            {
                if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "2")
                {
                    GetGridFill();
                    lblStaus.Text = "Approved";
                    lblStaus.Visible = true;
                    //  ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is added successfully.', 'success');", true);
                }
                if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "2")
                {
                    int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                        "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    if (a > 0)
                    {
                        // GetGridFill();
                        lblStaus.Text = "Approved";
                        lblStaus.Visible = true;
                        // ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);
                    }
                }
            }
        }
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null && Request.QueryString["CustomRole"] != null)
        {
            DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId,EmployeeID from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
            "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
            if (Dt_Get_UserType.Rows.Count > 0)
            {
                if (Dt_Get_UserType.Rows[0]["EmployeeID"].ToString() == Request.QueryString["MangerId"].ToString())
                {
                    //GetGridFill();
                    // ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is added successfully.', 'success');", true);
                }
                else
                {
                    int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' " +
                        "and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    if (a > 0)
                    {
                        //  GetGridFill();
                        lblStaus.Text = "Approved";
                        lblStaus.Visible = true;
                        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is approved successfully.', 'success');", true);

                    }
                }
            }
        }

    }
    public void rejectTimesheet(int i)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        // LinkButton LinkButton_Reject = (LinkButton)sender;
        //  GridViewRow gvr = (GridViewRow)LinkButton_Reject.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[i].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[i].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[i].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[i].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[i].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[i].FindControl("lnkSave") as LinkButton;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[i].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[i].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[i].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[i].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[i].FindControl("lblTotal"));
        Label lblDay = ((Label)GVChildlist.Rows[i].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[i].FindControl("hfdweekno"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[i].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[i].FindControl("txtBreakEndTime"));
        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[i].FindControl("ddstatus"));
        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[i].FindControl("hfdStatus"));
        Label lblError = ((Label)GVChildlist.Rows[i].FindControl("lblError"));
        Label lblErrorend = ((Label)GVChildlist.Rows[i].FindControl("lblErrorend"));

        //txtRegularStartTime.ReadOnly = false;
        //txtRegularEndTime.ReadOnly = false;
        //ddstatus.Enableq1d = true;

       /* if (ddstatus.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
            return;
        }
        if (ddstatus.SelectedIndex == 1)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
            return;
        }*/
        if (txtRegularStartTime.Text == ",")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }
        string startingtime = txtRegularStartTime.Text;
        if (startingtime == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
            lblError.Visible = true;
            return;
        }

        if (txtRegularEndTime.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
            lblErrorend.Visible = true;
            return;
        }

        bool check = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and EmpId='" + ddlemp.SelectedValue + "' ");
        if (check)
        {
            Over15x = txtOver15x.Text.Split(',');
            Over2x = txtOver2x.Text.Split(',');
            holiday = txtholiday.Text.Split(',');
            RegularStartTime = txtRegularStartTime.Text.Split(',');
            RegularEndTime = txtRegularEndTime.Text.Split(',');

            //BreakStartTime = txtBreakStartTime.Text.Split(',');
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
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
            //GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
            return;
        }
        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null && Session["UserTypeid"].ToString() == "1")
        {
            Tsee.BussID = int.Parse(Ddl_Business.SelectedValue);
        }
        else
        {
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        }
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
        Tsee.Status = ddstatus.SelectedIndex.ToString();

        if (ddstatus.SelectedIndex == 2)
        {
            Tsee.Worked = "Yes";
        }
        if (ddstatus.SelectedIndex == 1)
        {
            Tsee.Worked = "No";
        }
        else Tsee.Worked = "No";
        Tsee.BreakTotal = TotalBreakHr.ToString();
        TSC.Sp_InsertTimeSheetEntry(Tsee);
        ddstatus.SelectedIndex = 0;
        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
       if (a > 0)
        {

            ddstatus.SelectedIndex = 0;
            // GetGridFill();
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is rejected successfully.', 'success');", true);



        }
    }
}