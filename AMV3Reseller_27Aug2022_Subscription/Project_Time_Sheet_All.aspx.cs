using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;

public partial class Project_Time_Sheet_All : System.Web.UI.Page
{
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    string role_check_for_add_employee_message_throw; //to hide and unhide the add employee hyperlink

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            Get_Business_Name_List();
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
            {
                GetAllDetails(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
                lblweekDayName.Text = DateTime.Parse(txtmonth.Text).ToString("dddd");
                HyperLink_Project_Employee_Regular_Hours.NavigateUrl = "~/Project_Time_Sheet_All_Regular_Hours.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();
            }

            #region To show the viewtimesheet for regular reporting type and accountnt user type
            //DataTable dtEmptype = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5,12)", "");
            //if (dtEmptype.Rows.Count != 0)
            //{
            //    if (dtEmptype.Rows.Count > 1)
            //    {
            //        if (Request.QueryString["MangerId"] != null)
            //        {
            //            DataTable dt1 = Comm.GetAllDataFromTable("AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.EmpId", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
            //            if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "N")
            //            {
            //                hypProjectTime.Visible = false;
            //            }
            //            else
            //            {
            //                hypProjectTime.Visible = true;
            //            }
            //        }
            //        else
            //        {
            //            string a = Session["EmployeeId"].ToString();
            //            DataTable dt1 = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5,12) and assb.EmployeeID='" + a + "'", "");
            //            if (dt1.Rows.Count > 0)
            //            {
            //                if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "N")
            //                {
            //                    hypProjectTime.Visible = false;
            //                }
            //                else
            //                {
            //                    hypProjectTime.Visible = true;
            //                }
            //            }
            //            else
            //            {
            //                DataTable dtEmptype1 = Comm.GetAllDataFromTable("AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID", "distinct ad.EMPLOYEETYPE", "assb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " and assb.UserTypeId in (2,4,5,12)", "");

            //                if (dtEmptype1.Rows.Count > 1)
            //                {
            //                    hypProjectTime.Visible = true;
            //                }
            //            }
            //        }

            //        //if (Request.QueryString["MangerId"] != null)
            //        //{
            //        //    DataTable dt1 = Comm.GetAllDataFromTable(" ad.EMPLOYEETYPE from AssgineManager assm join AddEmployee ad on ad.EmployeeID=assm.EmpId", "distinct ad.EMPLOYEETYPE", "assm.ManagerID='" + Request.QueryString["MangerId"].ToString() + "'", "");
            //        //    if (dt1.Rows[0]["EMPLOYEETYPE"].ToString() == "R")
            //        //    {
            //        //        hypProjectTime.Visible = false;
            //        //    }
            //        //}

            //        //hypProjectTime.Visible = true;
            //        hypProjectTime.NavigateUrl = "~/ViewTimeSheetEntry.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

            //        // hyProjectTimeSheet.Visible = false;
            //        // hyViewTimeSheet.Visible = false;
            //    }
            //}
            #endregion

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

            ////From owner gridview to hide and unhide add employee hyperlink
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
            //        }
            //    }
            //    else
            //    {
            //        lnkAddEmp.Visible = false;
            //        return;
            //    }
            //}
            #endregion

            GridView1.DataSource = new List<Item>
            {
                new Item {Text1 = "11/01/2021", Text2 = "Tuesday", Text3 = "11/01/2021 09:11AM", Text4 = "11/01/2021 09:11PM",Text5 = "12:00 Hours", Text6 = "01:00 Hours", Text7 = "11:00 Hours", Text8 = "Pending"}
            };
            GridView1.DataBind();
        }
    }

    public class Item
    {
        public string Text1 { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Text5 { get; set; }
        public string Text6 { get; set; }
        public string Text7 { get; set; }
        public string Text8 { get; set; }
    }

    public void GetAllDetails(string Bid)
    {
        if (Request.QueryString["CustomRole"] != null)
        {
            HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString() + "&CustomRole=YES";
            
            string[] role;
            ddlemp.Items.Clear();
            ddlemp.Items.Add("Please Select");
            ddlemp.SelectedItem.Value = "";
            DataTable dt_Custome_User_Role_Popup_Emp_List = CustomRole_Pop_up_Employee_List(int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()), "P", int.Parse(Request.QueryString["MangerId"].ToString()));
            DataTable dtROLE = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id=" + int.Parse(Request.QueryString["MangerId"].ToString()) + "", "");
            DataRow[] dr;
            DataTable dtr;
            if (dtROLE.Rows.Count > 0)
            {
                role = dtROLE.Rows[0][0].ToString().Split(',');
                if (role.Contains("1") || role.Contains("2"))
                {
                    ddlemp.Items.Clear();
                    ddlemp.DataSource = null;
                    ddlemp.DataBind();
                    ddlemp.Items.Add("Please Select");
                    ddlemp.SelectedItem.Value = "";
                    dr = null;
                    dtr = null;
                    dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'P' AND [IsActive]='YES' and [EmployeeID]='" + Request.QueryString["MangerId"].ToString() + "'");
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
                if (role.Contains("7") || role.Contains("8") || role.Contains("9") || role.Contains("18") || role.Contains("19") || role.Contains("20"))
                {
                    ddlemp.Items.Clear();
                    ddlemp.DataSource = null;
                    ddlemp.DataBind();
                    ddlemp.Items.Add("Please Select");
                    ddlemp.SelectedItem.Value = "";
                    dr = null;
                    dtr = null;
                    dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'P' AND [IsActive]='YES'");
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

                if (dtROLE.Rows[0]["Role_Permission_Id"].ToString() == "")
                {
                    ddlemp.Items.Clear();
                    ddlemp.DataSource = null;
                    ddlemp.DataBind();
                    ddlemp.Items.Add("Please Select");
                    ddlemp.SelectedItem.Value = "";
                    dr = null;
                    dtr = null;
                    dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'P' AND [IsActive]='YES'");
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
                ddlemp.Items.Clear();
                ddlemp.DataSource = null;
                ddlemp.DataBind();
                ddlemp.Items.Add("Please Select");
                ddlemp.SelectedItem.Value = "";
                dr = null;
                dtr = null;
                dr = dt_Custome_User_Role_Popup_Emp_List.Select("[EMPLOYEETYPE] = 'P' AND [IsActive]='YES'");
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
                HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                if (Request.QueryString["MangerId"] != null)
                {
                    DataTable dtMangerEmpList = new DataTable();
                    dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "P");

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
                    //commented because administrator will approve and reject timesheet for others only
                    //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and ad.EMPLOYEETYPE='P'", "ad.EmployeeID");
                    Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType not in (1,5,12) and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + " and ad.IsActive='YES' and ad.EMPLOYEETYPE='P'", "ad.EmployeeID");
                }
            }
            else if (Session["UserTypeid"].ToString() == "2")
            {
                if (Request.QueryString["MangerId"] != null)
                {
                    HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();

                    // Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "EmpId");

                    DataTable dtMangerEmpList = new DataTable();
                    DataTable dtMangerEmpListFilter = new DataTable();
                    dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "P");
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
                    HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

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
                        //Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='P'", "AE.EmployeeID");
                        Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType Not in (1,5,12) and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='P'", "AE.EmployeeID");
                    }
                    else
                    {
                        Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE='P'", "AE.EmployeeID");
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
                        HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();

                        DataTable dtMangerEmpList = new DataTable();
                        dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "P");

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
                        HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                        DataTable dtMangerEmpList = new DataTable();
                        //commented because administrator will approve and reject timesheet for others only
                        //dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " AND AE.UserType!=1 and ae.EMPLOYEETYPE='P' and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
                        dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + " AND AE.UserType Not in (1,5,12) and ae.EMPLOYEETYPE='P' and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");

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
                        HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&MangerId=" + Request.QueryString["MangerId"].ToString();

                        // Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "'", "EmpId");

                        DataTable dtMangerEmpList = new DataTable();
                        DataTable dtMangerEmpListFilter = new DataTable();
                        dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()), "P");
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
                        HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                        //commented because administrator will approve and reject timesheet for others only
                        //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and ad.EMPLOYEETYPE='P'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                        Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId Not in (1,5,12) and IsActive='YES' and ad.EMPLOYEETYPE='P'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                    }

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
                    HyperLink_Back.NavigateUrl = "~/Time_Sheet_Entry_All.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                    //commented because administrator will approve and reject timesheet for others only
                    //Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId<>1 and IsActive='YES' and ad.EMPLOYEETYPE='P'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                    Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and assb.UserTypeId Not in (1,5,12) and IsActive='YES' and ad.EMPLOYEETYPE='P'", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
                }
            }
        }
        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
    }

    public void GetGridFill(int ProjectId)
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select employee first', 'error');", true);
            return;
        }
        int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        DataTable dtchild = new DataTable();
        int y = DateTime.Parse(txtmonth.Text).Year;
        int m = DateTime.Parse(txtmonth.Text).Month;
        var d = DateTime.Parse(txtmonth.Text.Trim());
        int empid = int.Parse(ddlemp.SelectedValue);
        CultureInfo cul = CultureInfo.CurrentCulture;
        int DateNo = d.Date.Day;
        dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp(y, m, DateNo, Bid, empid, ProjectId);
        ViewState["CurrentTable"] = dtchild;
        GVChildlist.DataSource = dtchild;
        GVChildlist.DataBind();
    }
    public void GetGridFill_All()
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select employee first', 'error');", true);
            return;
        }
        StringBuilder projid = new StringBuilder();
        int cnt = GVChildlist.Rows.Count;
        for (int i = 0; i < cnt; i++)
        {
            DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[i].FindControl("ddlProjectName");
            projid.Append(ddlProjectName.SelectedValue);
            projid.Append(",");
        }
        string proid = projid.ToString();
        string pronocoma = proid.Remove(proid.Length - 1, 1);
        int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        DataTable dtchild = new DataTable();
        int y = DateTime.Parse(txtmonth.Text).Year;
        int m = DateTime.Parse(txtmonth.Text).Month;
        var d = DateTime.Parse(txtmonth.Text.Trim());
        int empid = int.Parse(ddlemp.SelectedValue);
        CultureInfo cul = CultureInfo.CurrentCulture;
        int DateNo = d.Date.Day;

        dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
        ViewState["CurrentTable"] = dtchild;
        GVChildlist.DataSource = dtchild;
        GVChildlist.DataBind();
    }
    public void SetInitialRow()
    {
        DataTable dt = new DataTable();
        DataRow dr = null;
        DateTime d = DateTime.Parse(txtmonth.Text);//DateTime.Parse("2021-02-13");//;
        CultureInfo cul = CultureInfo.CurrentCulture;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        int weekdayno = (int)d.DayOfWeek + 1;
        dt.Columns.Add(new DataColumn("RowNumber", typeof(string)));
        dt.Columns.Add(new DataColumn("ProjectName", typeof(string)));
        dt.Columns.Add(new DataColumn("ProjectId", typeof(string)));
        dt.Columns.Add(new DataColumn("SelDate", typeof(string)));
        dt.Columns.Add(new DataColumn("weekno", typeof(string)));
        dt.Columns.Add(new DataColumn("weekdayno", typeof(string)));
        dt.Columns.Add(new DataColumn("Worked", typeof(string)));
        dt.Columns.Add(new DataColumn("ReqularStart", typeof(string)));
        dt.Columns.Add(new DataColumn("Regularend", typeof(string)));
        dt.Columns.Add(new DataColumn("Regular", typeof(string)));
        dt.Columns.Add(new DataColumn("BreakTotal", typeof(string)));
        dt.Columns.Add(new DataColumn("Total", typeof(string)));
        dt.Columns.Add(new DataColumn("Status", typeof(string)));
        dr = dt.NewRow();
        dr["RowNumber"] = 1;
        dr["ProjectName"] = string.Empty;
        dr["ProjectId"] = string.Empty;
        dr["SelDate"] = DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd");
        dr["weekno"] = weekNum.ToString();
        dr["weekdayno"] = weekdayno.ToString();
        dr["Worked"] = "No";
        dr["ReqularStart"] = string.Empty;
        dr["Regularend"] = string.Empty;
        dr["Regular"] = string.Empty;
        dr["BreakTotal"] = string.Empty;
        dr["Total"] = string.Empty;
        dr["Status"] = string.Empty;
        dt.Rows.Add(dr);
        ViewState["CurrentTable"] = dt;
        GVChildlist.DataSource = dt;
        GVChildlist.DataBind();
    }
    protected void lnkAddEmp_Click(object sender, EventArgs e)
    {
        int bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //Response.Redirect("AddNewEmployee_TAB.aspx?BusinessIdFromDefaultPage=" + bid + "&EmpType=P", false); //Commented because of new page developed in role development for add employee
        //Response.Redirect("Add_Employee.aspx?BusinessIdFromDefaultPage=" + bid + "&EmpType=P", false); //Commented because now emp will be added and then assigned
        Response.Redirect("Add_EmployeeV2.aspx", false);
    }

    string role_check_for_approve_and_reject_time_sheet_message_throw;
    protected void lnkReject_Click(object sender, EventArgs e)
    {
        //For business owner
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

                    DateTime d = DateTime.Parse(txtmonth.Text);
                    DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
                    if (string.IsNullOrEmpty(ddlProjectName.SelectedValue.ToString()))
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Project is not selected', 'error');", true);
                        return;
                    }
                    
                    RejectTimesheet();

                    var checkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    if (checkdata)
                    {
                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    }
                    else
                    {

                    }
                }
            }
            if (a > 0)
            {
                GetGridFill_All();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
            }
            else
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select Date Check Box From Table', 'error');", true);
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

                        DateTime d = DateTime.Parse(txtmonth.Text);
                        DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        ////Firoz 27 - 12
                        //if (a == 0)
                        //{

                        //    int saveFlag = 0;
                        //    if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
                        //        saveFlag = SaveDefaultValueInTimeSheet(row);
                        //    else
                        //        saveFlag = SaveTimesheet(row, isRejected: true);

                        //    if (saveFlag > 0)
                        //        Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        //}
                        //a++;
                    }
                }
                if (a > 0)
                {
                    GetGridFill_All();
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

                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        }
                    }
                    if (a > 0)
                    {
                        GetGridFill_All();
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
            return;
        }

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

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

                        DateTime d = DateTime.Parse(txtmonth.Text);
                        DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                    }
                }
                if (a > 0)
                {
                    GetGridFill_All();
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

                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

                            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
                        }
                    }
                    if (a > 0)
                    {
                        GetGridFill_All();
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






        //Old code commented because of role development
        //if (ddlemp.SelectedValue == "")
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
        //    return;
        //}
        //int a = 0;
        //foreach (GridViewRow row in GVChildlist.Rows)
        //{
        //    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //    if (chkApproved.Checked == true)
        //    {

        //        DateTime d = DateTime.Parse(txtmonth.Text);
        //        DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

        //        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where ProjectId=" + ddlProjectName.SelectedValue + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
        //    }
        //}
        //if (a > 0)
        //{
        //    GetGridFill_All();
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got rejected', 'success');", true);
        //}
        //else
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        //}
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

        bool duplicate_project = false;
        for (int ii = 0; ii < GVChildlist.Rows.Count - 1; ii++)
        {
            DropDownList ddlProjectName1 = (DropDownList)GVChildlist.Rows[ii].FindControl("ddlProjectName");
            for (int jj = ii + 1; jj < GVChildlist.Rows.Count; jj++)
            {
                DropDownList ddlProjectName2 = (DropDownList)GVChildlist.Rows[jj].FindControl("ddlProjectName");
                if (ddlProjectName1.SelectedValue == ddlProjectName2.SelectedValue)
                {
                    duplicate_project = true;
                    break;
                }
            }
            if (duplicate_project)
            {
                break;
            }
        }
        if (duplicate_project)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Duplicate Project Name', 'warning');", true);
            ddlAction.SelectedValue = "";
            return;
        }

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
            DropDownList ddstatus = (DropDownList)row.FindControl("ddstatus");

            if (chkApproved.Checked)
            {
                if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length == 0)
                { }

                if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length != 0)
                {
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
                    ModalPopupStartDate.Show();
                    ddlAction.SelectedValue = "";
                    
                    return;
                }

                ////firoz
                //DataTable samedate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId ='" + ddlProjectName.SelectedValue + "'", "");
                //if (samedate.Rows.Count > 0 && ddlProjectName.Enabled == true)
                //{
                //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Time Sheet already exist for this porject', 'error');", true);
                //    ddlAction.SelectedValue = "";
                //    return;
                //}

                if (txtRegularStartTime.Text.Trim().Length != 0 && txtRegularEndTime.Text.Trim().Length == 0)
                {
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
                    ModalPopupEndDate.Show();
                    ddlAction.SelectedValue = "";
                    return;
                }

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

                if (ddlProjectName.SelectedItem.Text == "Please Select")
                {
                    //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Project Name', 'warning');", true);
                    ModalPopupSelectProject.Show();
                    ddlAction.SelectedValue = "";
                    return;
                }

                if (ddlAction.SelectedValue == "A")
                {
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
                }

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

                    DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                    TimeSpan tt = new TimeSpan();
                    if (prvdate.Rows.Count > 0)
                    {
                        for (int i = 0; i < prvdate.Rows.Count; i++)
                        {
                            TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                            tt += prvtotTime;
                        }

                        tt += ToltalRegulahr;
                        if (tt.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                            ddlAction.SelectedValue = "";
                            return;
                        }
                    }
                }
            }
        }

        if (ddlAction.SelectedValue == "A")
        {
            lnkApproved_Click(sender, e);
            //approvedTimeSheet();
        }
        else if (ddlAction.SelectedValue == "R")
        {
            //foreach (GridViewRow row in GVChildlist.Rows)
            //{
            //    TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            //    TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            //    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            //    DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
            //    DropDownList ddstatus = (DropDownList)row.FindControl("ddstatus");

            //    if (chkApproved.Checked)
            //    {
            //        if (ddstatus.SelectedValue == "No")
            //        {
            //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Worked Status As Yes', 'warning');", true);
            //            ddlAction.SelectedValue = "";
            //            return;
            //        }

            //        if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length == 0)
            //        { }

            //        if (txtRegularStartTime.Text.Trim().Length == 0 && txtRegularEndTime.Text.Trim().Length != 0)
            //        {
            //            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
            //            ModalPopupStartDate.Show();
            //            ddlAction.SelectedValue = "";

            //            return;
            //        }

            //        if (txtRegularStartTime.Text.Trim().Length != 0 && txtRegularEndTime.Text.Trim().Length == 0)
            //        {
            //            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
            //            ModalPopupEndDate.Show();
            //            ddlAction.SelectedValue = "";
            //            return;
            //        }

            //        if (ddstatus.SelectedValue == "Yes")
            //        {
            //            if (txtRegularStartTime.Text.Trim().Length == 0)
            //            {
            //                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Start Date', 'warning');", true);
            //                ModalPopupStartDate.Show();
            //                ddlAction.SelectedValue = "";
            //                return;
            //            }

            //            if (txtRegularEndTime.Text.Trim().Length == 0)
            //            {
            //                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter End Date', 'warning');", true);
            //                ModalPopupEndDate.Show();
            //                ddlAction.SelectedValue = "";
            //                return;
            //            }
            //        }

            //        if (ddlProjectName.SelectedItem.Text == "Please Select")
            //        {
            //            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Projuect Name', 'warning');", true);
            //            ddlAction.SelectedValue = "";
            //            return;
            //        }

            //        if (txtRegularStartTime.Text.Trim().Length > 0 && txtRegularEndTime.Text.Trim().Length > 0)
            //        {
            //            DateTime dtfrom = DateTime.Parse(txtRegularStartTime.Text);
            //            DateTime dtto = DateTime.Parse(txtRegularEndTime.Text);
            //            if (dtfrom > dtto)
            //            {
            //                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Start Date is Greater than End Date', 'warning');", true);
            //                ModalPopupStarDateGreaterEnd.Show();
            //                ddlAction.SelectedValue = "";
            //                return;
            //            }

            //            string[] RegularStartTime = { "" }, RegularEndTime = { "" };
            //            RegularStartTime[0] = txtRegularStartTime.Text;
            //            RegularEndTime[0] = txtRegularEndTime.Text;
            //            TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
            //            if (ToltalRegulahr.TotalHours >= 24)
            //            {
            //                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Start Time and End Time Must be within 24Hrs', 'warning');", true);
            //                ModalPopupStartTimeMust.Show();
            //                ddlAction.SelectedValue = "";
            //                return;
            //            }

            //            DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

            //            TimeSpan tt = new TimeSpan();
            //            if (prvdate.Rows.Count > 0)
            //            {
            //                for (int i = 0; i < prvdate.Rows.Count; i++)
            //                {
            //                    TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
            //                    tt += prvtotTime;
            //                }

            //                tt += ToltalRegulahr;
            //                if (tt.TotalHours >= 24)
            //                {
            //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
            //                    ddlAction.SelectedValue = "";
            //                    return;
            //                }
            //            }
            //        }
            //    }
            //}

            lnkReject_Click(sender, e);

            //lnkReject_Click(sender, e);
        }

        ddlAction.SelectedValue = "";
    }

    protected void lnkApproved_Click(object sender, EventArgs e)
    {
        //For business owner
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
            #region:comment code
            //StringBuilder sb = new StringBuilder();
            //int a = 0;
            //foreach (GridViewRow row in GVChildlist.Rows)
            //{
            //    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            //    TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            //    TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            //    DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
            //    if (chkApproved.Checked == true)
            //    {
            //        if (ddlProjectName.SelectedValue == "")
            //        {
            //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Please Select Project', 'error');", true);
            //            return;
            //        }
            //        if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
            //        {
            //            HiddenField lblDate1 = (HiddenField)row.FindControl("lblDate");
            //            sb.Append(lblDate1.Value + ",");
            //        }
            //    }
            //}
            //string blankdate = sb.ToString();
            //if (blankdate != "")
            //{
            //    string msg = "For these " + blankdate + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
            //    lblmsg.Text = msg;
            //    ModalPopupConfirm.Show();
            //    return;
            //}
            //else
            //{
            //    foreach (GridViewRow row in GVChildlist.Rows)
            //    {
            //        CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
            //        TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            //        TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            //        DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
            //        if (chkApproved.Checked == true)
            //        {
            //            HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
            //            HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
            //            DateTime d = DateTime.Parse(lblDate.Value);

            //            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.Date.ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
            //        }
            //    }
            //    if (a > 0)
            //    {
            //        GetGridFill_All();
            //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            //    }
            //    else
            //    {
            //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
            //    }
            //}
            //return;
            #endregion
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
        //if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] != null)
        //{
        //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' ", "UserTypeId");
        //    DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["MangerId"].ToString() + "' and UserTypeId='2'", "");

        //    //When users are old who are registered before the role permission development
        //    if (Dt_Check_Manager_Role_Permission.Rows.Count == 0)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //ModalPopupConfirm.Show();
        //        //return;
        //        if (ddlemp.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
        //            return;
        //        }
        //        int a = 0;
        //        foreach (GridViewRow row in GVChildlist.Rows)
        //        {
        //            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //            if (chkApproved.Checked == true)
        //            {
        //                if (ddlProjectName.SelectedValue == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Please Select Project', 'error');", true);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
        //                {
        //                    HiddenField lblDate1 = (HiddenField)row.FindControl("lblDate");
        //                    sb.Append(lblDate1.Value + ",");
        //                }
        //            }
        //        }
        //        string blankdate = sb.ToString();
        //        if (blankdate != "")
        //        {
        //            string msg = "For these " + blankdate + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
        //            lblmsg.Text = msg;
        //            ModalPopupConfirm.Show();
        //            return;
        //        }
        //        else
        //        {
        //            foreach (GridViewRow row in GVChildlist.Rows)
        //            {
        //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                if (chkApproved.Checked == true)
        //                {
        //                    HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
        //                    HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
        //                    DateTime d = DateTime.Parse(lblDate.Value);

        //                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.Date.ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
        //                }
        //            }
        //            if (a > 0)
        //            {
        //                GetGridFill_All();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        //            }
        //        }
        //        return;
        //    }

        //    //When users are new who are registered after the role permission development
        //    string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
        //    {
        //        #region For_Approve_Time_Sheet definition
        //        if (role_check_for_approve_and_reject_time_sheet == "9")
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            //ModalPopupConfirm.Show();
        //            //return;
        //            if (ddlemp.SelectedValue == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
        //                return;
        //            }
        //            int a = 0;
        //            foreach (GridViewRow row in GVChildlist.Rows)
        //            {
        //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                if (chkApproved.Checked == true)
        //                {
        //                    if (ddlProjectName.SelectedValue == "")
        //                    {
        //                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Please Select Project', 'error');", true);
        //                        return;
        //                    }
        //                    if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
        //                    {
        //                        HiddenField lblDate1 = (HiddenField)row.FindControl("lblDate");
        //                        sb.Append(lblDate1.Value + ",");
        //                    }
        //                }
        //            }
        //            string blankdate = sb.ToString();
        //            if (blankdate != "")
        //            {
        //                string msg = "For these " + blankdate + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
        //                lblmsg.Text = msg;
        //                ModalPopupConfirm.Show();
        //                return;
        //            }
        //            else
        //            {
        //                foreach (GridViewRow row in GVChildlist.Rows)
        //                {
        //                    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                    TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                    TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                    DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                    if (chkApproved.Checked == true)
        //                    {
        //                        HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
        //                        HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
        //                        DateTime d = DateTime.Parse(lblDate.Value);

        //                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.Date.ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
        //                    }
        //                }
        //                if (a > 0)
        //                {
        //                    GetGridFill_All();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //                }
        //                else
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        //                }
        //            }
        //        }
        //        else
        //            role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
        //        #endregion
        //    }
        //    if (role_check_for_approve_and_reject_time_sheet_message_throw != "9")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

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
        //if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        //{
        //    DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
        //    DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

        //    //When users are old who are registered before the role permission development
        //    if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        //ModalPopupConfirm.Show();
        //        //return;
        //        if (ddlemp.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
        //            return;
        //        }
        //        int a = 0;
        //        foreach (GridViewRow row in GVChildlist.Rows)
        //        {
        //            CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //            if (chkApproved.Checked == true)
        //            {
        //                if (ddlProjectName.SelectedValue == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Please Select Project', 'error');", true);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
        //                {
        //                    HiddenField lblDate1 = (HiddenField)row.FindControl("lblDate");
        //                    sb.Append(lblDate1.Value + ",");
        //                }
        //            }
        //        }
        //        string blankdate = sb.ToString();
        //        if (blankdate != "")
        //        {
        //            string msg = "For these " + blankdate + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
        //            lblmsg.Text = msg;
        //            ModalPopupConfirm.Show();
        //            return;
        //        }
        //        else
        //        {
        //            foreach (GridViewRow row in GVChildlist.Rows)
        //            {
        //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                if (chkApproved.Checked == true)
        //                {
        //                    HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
        //                    HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
        //                    DateTime d = DateTime.Parse(lblDate.Value);

        //                    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.Date.ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
        //                }
        //            }
        //            if (a > 0)
        //            {
        //                GetGridFill_All();
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //            }
        //            else
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        //            }
        //        }
        //        return;
        //    }

        //    //When users are new who are registered after the role permission development
        //    string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_approve_and_reject_time_sheet in splittedArray)
        //    {
        //        #region For_Approve_Time_Sheet definition
        //        if (role_check_for_approve_and_reject_time_sheet == "20")
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            //ModalPopupConfirm.Show();
        //            //return;
        //            if (ddlemp.SelectedValue == "")
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Employee is not selected', 'error');", true);
        //                return;
        //            }
        //            int a = 0;
        //            foreach (GridViewRow row in GVChildlist.Rows)
        //            {
        //                CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                if (chkApproved.Checked == true)
        //                {
        //                    if (ddlProjectName.SelectedValue == "")
        //                    {
        //                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Please Select Project', 'error');", true);
        //                        return;
        //                    }
        //                    if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
        //                    {
        //                        HiddenField lblDate1 = (HiddenField)row.FindControl("lblDate");
        //                        sb.Append(lblDate1.Value + ",");
        //                    }
        //                }
        //            }
        //            string blankdate = sb.ToString();
        //            if (blankdate != "")
        //            {
        //                string msg = "For these " + blankdate + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
        //                lblmsg.Text = msg;
        //                ModalPopupConfirm.Show();
        //                return;
        //            }
        //            else
        //            {
        //                foreach (GridViewRow row in GVChildlist.Rows)
        //                {
        //                    CheckBox chkApproved = (CheckBox)row.FindControl("chkApproved");
        //                    TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        //                    TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        //                    DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        //                    if (chkApproved.Checked == true)
        //                    {
        //                        HiddenField HiddenField = (HiddenField)row.FindControl("hfdweekno");
        //                        HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
        //                        DateTime d = DateTime.Parse(lblDate.Value);

        //                        a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.Date.ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
        //                    }
        //                }
        //                if (a > 0)
        //                {
        //                    GetGridFill_All();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //                }
        //                else
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        //                }
        //            }
        //        }
        //        else
        //            role_check_for_approve_and_reject_time_sheet_message_throw = role_check_for_approve_and_reject_time_sheet;
        //        #endregion
        //    }
        //    if (role_check_for_approve_and_reject_time_sheet_message_throw != "20")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Approve And Reject Time Sheet', 'error');", true);
        //    }
        //}
        GetGridFill_All();
    }

    protected void btnnext_Click(object sender, EventArgs e)
    {
        DateTime d = DateTime.Parse(txtmonth.Text);
        DateTime next = d.AddDays(1);
        txtmonth.Text = next.ToString("yyyy-MM-dd");
        StringBuilder projid = new StringBuilder();
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Employee', 'error');", true);
            return;
        }
        DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");

        if (dtsaveddata.Rows.Count > 0)
        {
            for (int i = 0; i < dtsaveddata.Rows.Count; i++)
            {

                projid.Append(dtsaveddata.Rows[i]["ProjectId"].ToString());
                projid.Append(",");
            }
            string proid = projid.ToString();
            string pronocoma = proid.Remove(proid.Length - 1, 1);
            int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d1 = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int DateNo = d1.Date.Day;

            dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
            ViewState["CurrentTable"] = dtchild;
            GVChildlist.DataSource = dtchild;
            GVChildlist.DataBind();

            Get_Selected_Calendar_Day();
        }
        else
        {
            SetInitialRow();

            Get_Selected_Calendar_Day();
        }
    }

    protected void txtmonth_TextChanged(object sender, EventArgs e)
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Employee', 'error');", true);
            return;
        }
        if (txtmonth.Text != "")
        {
            StringBuilder projid = new StringBuilder();
            DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
            if (dtsaveddata.Rows.Count > 0)
            {
                for (int i = 0; i < dtsaveddata.Rows.Count; i++)
                {
                    projid.Append(dtsaveddata.Rows[i]["ProjectId"].ToString());
                    projid.Append(",");
                }
                string proid = projid.ToString();
                string pronocoma = proid.Remove(proid.Length - 1, 1);
                int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                DataTable dtchild = new DataTable();
                int y = DateTime.Parse(txtmonth.Text).Year;
                int m = DateTime.Parse(txtmonth.Text).Month;
                var d1 = DateTime.Parse(txtmonth.Text.Trim());
                int empid = int.Parse(ddlemp.SelectedValue);
                CultureInfo cul = CultureInfo.CurrentCulture;
                int DateNo = d1.Date.Day;

                dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
                ViewState["CurrentTable"] = dtchild;
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();

                Get_Selected_Calendar_Day();
            }
            else
            {
                SetInitialRow();
                Get_Selected_Calendar_Day();
            }
        }
    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        DateTime d = DateTime.Parse(txtmonth.Text);
        DateTime prv = d.AddDays(-1);
        txtmonth.Text = prv.ToString("yyyy-MM-dd");
        StringBuilder projid = new StringBuilder();

        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Employee', 'error');", true);
            return;
        }
        DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
        if (dtsaveddata.Rows.Count > 0)
        {
            for (int i = 0; i < dtsaveddata.Rows.Count; i++)
            {

                projid.Append(dtsaveddata.Rows[i]["ProjectId"].ToString());
                projid.Append(",");
            }
            string proid = projid.ToString();
            string pronocoma = proid.Remove(proid.Length - 1, 1);
            int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d1 = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int DateNo = d1.Date.Day;

            dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
            ViewState["CurrentTable"] = dtchild;
            GVChildlist.DataSource = dtchild;
            GVChildlist.DataBind();

            Get_Selected_Calendar_Day();
        }
        else
        {
            SetInitialRow();

            Get_Selected_Calendar_Day();
        }
    }

    protected void ddlemp_SelectedIndexChanged(object sender, EventArgs e)
    {
        ////firoz
        //lblsms.Text = Session["user"].ToString();
        //if ("Welcome! " + ddlemp.SelectedItem.Text == lblsms.Text)
        //{
        //    ddlAction.Enabled = false;
        //    this.GVChildlist.Columns[12].Visible = true;
        //    this.GVChildlist.Columns[13].Visible = false;
        //    //lnkApproved.Enabled = false;
        //}
        //else
        //{
        //    ddlAction.Enabled = true;
        //    this.GVChildlist.Columns[12].Visible = false;
        //    this.GVChildlist.Columns[13].Visible = true;
        //    //lnkApproved.Enabled = true;
        //}
        ////end

        //if (ddlemp.SelectedValue != "")
        //{
        //    DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
        //        "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
        //    if (Dt_Get_UserType.Rows.Count > 0)
        //    {
        //        if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "2" && Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "5"
        //            && Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "4" && Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "12"
        //            && Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "1")
        //        {
        //            ddlAction.Enabled = false;
        //            this.GVChildlist.Columns[12].Visible = true;
        //            this.GVChildlist.Columns[13].Visible = false;
        //            //lnkApproved.Enabled = false;
        //        }
        //        if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "2" || Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "5"
        //            && Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "1")
        //        {
        //            ddlAction.Enabled = false;
        //            this.GVChildlist.Columns[12].Visible = true;
        //            this.GVChildlist.Columns[13].Visible = false;
        //            //lnkApproved.Enabled = false;
        //        }
        //        if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "4" || Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "12")
        //        {
        //            ddlAction.Enabled = true;
        //            this.GVChildlist.Columns[12].Visible = false;
        //            this.GVChildlist.Columns[13].Visible = true;
        //            //lnkApproved.Enabled = true;
        //        }
        //    }
        //}

        if (ddlemp.SelectedValue != "")
        {
            if (Request.QueryString["BusinessIdFromDefaultPage"] != null && Request.QueryString["MangerId"] == null && Request.QueryString["CustomRole"] == null)
            {
                if (Session["UserTypeid"].ToString() != "1")
                {
                    DataTable Dt_Get_UserType = Comm.ExecuteQueryDataTable("select UserTypeId from MappingTable where EmployeeID = '" + int.Parse(ddlemp.SelectedValue) + "' " +
                    "AND BusinessID = '" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' ");
                    if (Dt_Get_UserType.Rows.Count > 0)
                    {
                        if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() == "5")
                        {
                            ddlAction.Enabled = false;
                            this.GVChildlist.Columns[12].Visible = true;
                            this.GVChildlist.Columns[14].Visible = false;
                            //lnkApproved.Enabled = false;
                        }
                        if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "5")
                        {
                            ddlAction.Enabled = true;
                            this.GVChildlist.Columns[12].Visible = false;
                            this.GVChildlist.Columns[14].Visible = true;
                            //lnkApproved.Enabled = true;
                        }
                    }
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
                        ddlAction.Enabled = false;
                        this.GVChildlist.Columns[12].Visible = true;
                        this.GVChildlist.Columns[14].Visible = false;
                        //lnkApproved.Enabled = false;
                    }
                    if (Dt_Get_UserType.Rows[0]["UserTypeId"].ToString() != "2")
                    {
                        ddlAction.Enabled = true;
                        this.GVChildlist.Columns[12].Visible = false;
                        this.GVChildlist.Columns[14].Visible = true;
                        //lnkApproved.Enabled = true;
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
                        ddlAction.Enabled = false;
                        this.GVChildlist.Columns[12].Visible = true;
                        this.GVChildlist.Columns[14].Visible = false;
                        //lnkApproved.Enabled = false;
                    }
                    else
                    {
                        ddlAction.Enabled = true;
                        this.GVChildlist.Columns[12].Visible = false;
                        this.GVChildlist.Columns[14].Visible = true;
                        //lnkApproved.Enabled = true;
                    }
                }
            }
        }

        this.GVChildlist.Columns[12].Visible = true;//Status column
        this.GVChildlist.Columns[13].Visible = false;//Edit Status column

        if (ddlemp.SelectedValue == "")
        {
            //GVChildlist.Visible = false;
            //GridView1.Visible = false;
            //h3_regular_hours_heading.Visible = false;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select employee', 'error');", true);
            return;
        }

        //GVChildlist.Visible = true;
        //GVChildlist.DataSource = null;
        //GVChildlist.DataBind();
        //GridView1.Visible = true;
        //h3_regular_hours_heading.Visible = true;
        DataTable dt = new DataTable();
        StringBuilder projid = new StringBuilder();
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        lblrate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
        //DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
        //DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry AS A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectId and (B.EndDate >= GETDATE() OR B.EndDate IS NULL OR B.EndDate = '' )", "*", "EmpId=" + ddlemp.SelectedValue + " and A.ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "A.ProjectId");
        DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry AS A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectId and (B.EndDate >= '" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' OR B.EndDate IS NULL OR B.EndDate = '' )", "*", "EmpId=" + ddlemp.SelectedValue + " and A.ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "A.ProjectId");

        //GetAllDataFromTable(Select * from TimeSheetEntry AS A
        //INNER JOIN AddProject AS B
        // ON A.ProjectId = B.ProjectID
        // and(B.EndDate >= GETDATE() OR B.EndDate IS NULL OR B.EndDate = '')
        // WHERE EmpId = 22020 and A.ProjectId is not null and
        // TimeSheetDate = '2023-03-03' order by A.ProjectId)
        if (dtsaveddata.Rows.Count > 0)
        {
            for (int i = 0; i < dtsaveddata.Rows.Count; i++)
            {

                projid.Append(dtsaveddata.Rows[i]["ProjectId"].ToString());
                projid.Append(",");
            }
            string proid = projid.ToString();
            string pronocoma = proid.Remove(proid.Length - 1, 1);
            int Bid = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int DateNo = d.Date.Day;

            dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
            ViewState["CurrentTable"] = dtchild;
            GVChildlist.DataSource = dtchild;
            GVChildlist.DataBind();

            Get_Selected_Calendar_Day();
        }
        else
        {
            SetInitialRow();

            Get_Selected_Calendar_Day();
        }
    }

    string role_check_for_add_and_edit_time_sheet_message_throw;
    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //For business owners
        string user_type_id = Session["UserTypeid"].ToString();
        if (user_type_id == "1")
        {
            try
            {
                string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                GridView GVChildlist = (GridView)sender;
                if (e.CommandName == "AddBreak")
                {
                    DataTable dt;
                    GvPopUpEmpList.DataSource = null;
                    GvPopUpEmpList.DataBind();
                    txtBreakEndTime.Text = string.Empty;
                    txtBreakStartTime.Text = string.Empty;
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = GVChildlist.Rows[index];

                    HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                    // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                    //if (chkworked.Checked == false)
                    //{
                    //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                    //    return;
                    //}
                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                        return;
                    }
                    if (ddstatus.SelectedIndex == 1)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        return;
                    }
                    //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                    lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
                    ViewState["datebreak"] = lblDate.Value;
                    ViewState["weekno"] = hfdweekno.Value;
                    ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                    string regMINst = lblDate.Value + " 00:00";
                    string regMAXst = lblDate.Value + " 23:59";
                    txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                    string regMinEnd = lblDate.Value + " 00:00";
                    //string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                    string regMAXend = lblDate.Value + " 23:59";

                    txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                    // hfddatebreak.Value = lblDate.Text;
                    // hfdweeknopop.Value = hfdweekno.Value;
                    //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
                    ViewState["dt"] = dt;
                    GvPopUpEmpList.DataSource = ViewState["dt"];
                    GvPopUpEmpList.DataBind();
                    //up2.Update();
                    //GetGridFill();

                    ModalPopupEmpList.Show();
                }

                if (e.CommandName == "Save")
                {
                    int index = Convert.ToInt32(e.CommandArgument);
                    GridViewRow row = GVChildlist.Rows[index];
                    TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));

                    Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                    HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                    HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
                    Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                    DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                        // absent(row.RowIndex);
                        return;
                    }


                    if (ddstatus.SelectedIndex == 0)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        // absent(row.RowIndex);
                        return;
                    }
                    //firoz
                    DataTable samedate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId ='" + ddlProjectName.SelectedValue + "'", "");



                    if (samedate.Rows.Count > 0 && ddlProjectName.Enabled == true)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Time Sheet already exist for this porject', 'error');", true);
                        return;
                    }

                    if (ddstatus.SelectedIndex == 1)
                    {
                        //firoz 14
                        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        absent(row.RowIndex);
                        return;
                    }
                    if (txtRegularStartTime.Text == ",")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                        return;
                    }
                    string startingtime = txtRegularStartTime.Text;
                    if (startingtime == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                        return;
                    }

                    if (txtRegularEndTime.Text == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                        return;
                    }
                    bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
                    if (check == true)
                    {
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

                    //DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

                    DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                    TimeSpan tt = new TimeSpan();
                    if (prvdate.Rows.Count > 0)
                    {
                        for (int i = 0; i < prvdate.Rows.Count; i++)
                        {
                            TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                            tt += prvtotTime;
                        }

                        tt += ToltalRegulahr;
                        if (tt.TotalHours >= 24)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                            return;
                        }
                    }

                    //return;
                    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                    if (ToltalRegulahr < TotalBreakHr)
                    {
                        GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                        return;
                    }
                    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                    Tsee.WeekNo = int.Parse(weekno[0]);
                    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                    Tsee.Day = lblweekDayName.Text;
                    Tsee.Regular = ToltalRegulahr.ToString();
                    Tsee.Over1_5x = Over15x[0];
                    Tsee.Over2x = Over2x[0];
                    Tsee.Holiday = holiday[0];
                    Tsee.Total = TotalHr.ToString();
                    Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                    Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                    Tsee.BreakStart = BreakStartTime[0].ToString();
                    Tsee.BreakEnd = BreakEndTime[0].ToString();
                    if (hfdStatus.Value != "2")
                    {
                        Tsee.Status = "1";
                    }
                    else
                    {
                        Tsee.Status = "1"; // "2";
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
                    Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                    //TSC.Sp_InsertTimeSheetEntry(Tsee);
                    TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                    GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                    //firoz
                    //DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                    ddlProjectName.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
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
                try
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;
                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                        //if (chkworked.Checked == false)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        //    return;
                        //}
                        if (ddlProjectName.SelectedValue == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                            return;
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
                        ViewState["datebreak"] = lblDate.Value;
                        ViewState["weekno"] = hfdweekno.Value;
                        ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                        string regMINst = lblDate.Value + " 00:00";
                        string regMAXst = lblDate.Value + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Value + " 00:00";
                        //string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                        string regMAXend = lblDate.Value + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        //up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                    }

                    if (e.CommandName == "Save")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));

                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        if (ddlProjectName.SelectedValue == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                            // absent(row.RowIndex);
                            return;
                        }

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
                        string startingtime = txtRegularStartTime.Text;
                        if (startingtime == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {
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

                        //DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

                        DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                        TimeSpan tt = new TimeSpan();
                        if (prvdate.Rows.Count > 0)
                        {
                            for (int i = 0; i < prvdate.Rows.Count; i++)
                            {
                                TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                                tt += prvtotTime;
                            }

                            tt += ToltalRegulahr;
                            if (tt.TotalHours >= 24)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                                return;
                            }
                        }

                        //return;
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                        Tsee.Day = lblweekDayName.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        if (hfdStatus.Value != "2")
                        {
                            Tsee.Status = "1";
                        }
                        else
                        {
                            Tsee.Status = "1"; // "2";
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
                        Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                        //TSC.Sp_InsertTimeSheetEntry(Tsee);
                        TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                        DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        ddlProjectName_.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
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
                    try
                    {
                        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                        GridView GVChildlist = (GridView)sender;
                        if (e.CommandName == "AddBreak")
                        {
                            DataTable dt;
                            GvPopUpEmpList.DataSource = null;
                            GvPopUpEmpList.DataBind();
                            txtBreakEndTime.Text = string.Empty;
                            txtBreakStartTime.Text = string.Empty;
                            int index = Convert.ToInt32(e.CommandArgument);
                            GridViewRow row = GVChildlist.Rows[index];

                            HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                            //if (chkworked.Checked == false)
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            //    return;
                            //}
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }
                            if (ddstatus.SelectedIndex == 1)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
                            ViewState["datebreak"] = lblDate.Value;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Value + " 00:00";
                            string regMAXst = lblDate.Value + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Value + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Value + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            //up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();
                        }

                        if (e.CommandName == "Save")
                        {
                            int index = Convert.ToInt32(e.CommandArgument);
                            GridViewRow row = GVChildlist.Rows[index];
                            TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));

                            Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                            HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
                            Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                // absent(row.RowIndex);
                                return;
                            }

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
                            string startingtime = txtRegularStartTime.Text;
                            if (startingtime == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                                return;
                            }

                            if (txtRegularEndTime.Text == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                                return;
                            }
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
                            if (check == true)
                            {
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

                            //DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

                            DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                            TimeSpan tt = new TimeSpan();
                            if (prvdate.Rows.Count > 0)
                            {
                                for (int i = 0; i < prvdate.Rows.Count; i++)
                                {
                                    TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                                    tt += prvtotTime;
                                }

                                tt += ToltalRegulahr;
                                if (tt.TotalHours >= 24)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                                    return;
                                }
                            }

                            //return;
                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                            Tsee.Day = lblweekDayName.Text;
                            Tsee.Regular = ToltalRegulahr.ToString();
                            Tsee.Over1_5x = Over15x[0];
                            Tsee.Over2x = Over2x[0];
                            Tsee.Holiday = holiday[0];
                            Tsee.Total = TotalHr.ToString();
                            Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                            Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                            Tsee.BreakStart = BreakStartTime[0].ToString();
                            Tsee.BreakEnd = BreakEndTime[0].ToString();
                            if (hfdStatus.Value != "2")
                            {
                                Tsee.Status = "1";
                            }
                            else
                            {
                                Tsee.Status = "1"; // "2";
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
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            //TSC.Sp_InsertTimeSheetEntry(Tsee);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
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
            return;
        }

        //For owner gridview Enter/View Time Sheet hyperlink
        if (Request.QueryString["BusinessIdFromDefaultPage"] != null)
        {
            DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessID = '" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "' ", "UserTypeId");
            DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");

            //When users are old who are registered before the role permission development
            if (Dt_Check_Owner_Role_Permission.Rows.Count == 0)
            {
                try
                {
                    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                    GridView GVChildlist = (GridView)sender;
                    if (e.CommandName == "AddBreak")
                    {
                        DataTable dt;
                        GvPopUpEmpList.DataSource = null;
                        GvPopUpEmpList.DataBind();
                        txtBreakEndTime.Text = string.Empty;
                        txtBreakStartTime.Text = string.Empty;
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                        //if (chkworked.Checked == false)
                        //{
                        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        //    return;
                        //}
                        if (ddlProjectName.SelectedValue == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                            return;
                        }
                        if (ddstatus.SelectedIndex == 1)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            return;
                        }
                        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                        lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
                        ViewState["datebreak"] = lblDate.Value;
                        ViewState["weekno"] = hfdweekno.Value;
                        ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                        string regMINst = lblDate.Value + " 00:00";
                        string regMAXst = lblDate.Value + " 23:59";
                        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                        string regMinEnd = lblDate.Value + " 00:00";
                        //string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                        string regMAXend = lblDate.Value + " 23:59";

                        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                        // hfddatebreak.Value = lblDate.Text;
                        // hfdweeknopop.Value = hfdweekno.Value;
                        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        //up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                    }

                    if (e.CommandName == "Save")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];
                        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));

                        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
                        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        if (ddlProjectName.SelectedValue == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                            // absent(row.RowIndex);
                            return;
                        }

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
                        string startingtime = txtRegularStartTime.Text;
                        if (startingtime == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                            return;
                        }

                        if (txtRegularEndTime.Text == "")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
                        if (check == true)
                        {
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

                        //DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

                        DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                        TimeSpan tt = new TimeSpan();
                        if (prvdate.Rows.Count > 0)
                        {
                            for (int i = 0; i < prvdate.Rows.Count; i++)
                            {
                                TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                                tt += prvtotTime;
                            }

                            tt += ToltalRegulahr;
                            if (tt.TotalHours >= 24)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                                return;
                            }
                        }

                        //return;
                        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                        if (ToltalRegulahr < TotalBreakHr)
                        {
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                            return;
                        }
                        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                        Tsee.WeekNo = int.Parse(weekno[0]);
                        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                        Tsee.Day = lblweekDayName.Text;
                        Tsee.Regular = ToltalRegulahr.ToString();
                        Tsee.Over1_5x = Over15x[0];
                        Tsee.Over2x = Over2x[0];
                        Tsee.Holiday = holiday[0];
                        Tsee.Total = TotalHr.ToString();
                        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                        Tsee.BreakStart = BreakStartTime[0].ToString();
                        Tsee.BreakEnd = BreakEndTime[0].ToString();
                        if (hfdStatus.Value != "2")
                        {
                            Tsee.Status = "1";
                        }
                        else
                        {
                            Tsee.Status = "1"; // "2";
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
                        Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                        //TSC.Sp_InsertTimeSheetEntry(Tsee);
                        TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                        GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                        DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                        ddlProjectName_.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
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
                    try
                    {
                        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
                        GridView GVChildlist = (GridView)sender;
                        if (e.CommandName == "AddBreak")
                        {
                            DataTable dt;
                            GvPopUpEmpList.DataSource = null;
                            GvPopUpEmpList.DataBind();
                            txtBreakEndTime.Text = string.Empty;
                            txtBreakStartTime.Text = string.Empty;
                            int index = Convert.ToInt32(e.CommandArgument);
                            GridViewRow row = GVChildlist.Rows[index];

                            HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

                            //if (chkworked.Checked == false)
                            //{
                            //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                            //    return;
                            //}
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }
                            if (ddstatus.SelectedIndex == 1)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
                            ViewState["datebreak"] = lblDate.Value;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Value + " 00:00";
                            string regMAXst = lblDate.Value + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Value + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Value + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            //up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();
                        }

                        if (e.CommandName == "Save")
                        {
                            int index = Convert.ToInt32(e.CommandArgument);
                            GridViewRow row = GVChildlist.Rows[index];
                            TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));

                            Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
                            HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
                            Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                // absent(row.RowIndex);
                                return;
                            }

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
                            string startingtime = txtRegularStartTime.Text;
                            if (startingtime == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
                                return;
                            }

                            if (txtRegularEndTime.Text == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                                return;
                            }
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
                            if (check == true)
                            {
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

                            //DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

                            DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId <>'" + ddlProjectName.SelectedValue + "' and total <> ''", "");

                            TimeSpan tt = new TimeSpan();
                            if (prvdate.Rows.Count > 0)
                            {
                                for (int i = 0; i < prvdate.Rows.Count; i++)
                                {
                                    TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
                                    tt += prvtotTime;
                                }

                                tt += ToltalRegulahr;
                                if (tt.TotalHours >= 24)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
                                    return;
                                }
                            }

                            //return;
                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                            Tsee.Day = lblweekDayName.Text;
                            Tsee.Regular = ToltalRegulahr.ToString();
                            Tsee.Over1_5x = Over15x[0];
                            Tsee.Over2x = Over2x[0];
                            Tsee.Holiday = holiday[0];
                            Tsee.Total = TotalHr.ToString();
                            Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
                            Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
                            Tsee.BreakStart = BreakStartTime[0].ToString();
                            Tsee.BreakEnd = BreakEndTime[0].ToString();
                            if (hfdStatus.Value != "2")
                            {
                                Tsee.Status = "1";
                            }
                            else
                            {
                                Tsee.Status = "1"; // "2";
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
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            //TSC.Sp_InsertTimeSheetEntry(Tsee);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw != "18" || role_check_for_add_and_edit_time_sheet_message_throw != "19")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
            }
        }









        //Commented because of role development
        //try
        //{
        //    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //    GridView GVChildlist = (GridView)sender;
        //    if (e.CommandName == "AddBreak")
        //    {
        //        DataTable dt;
        //        GvPopUpEmpList.DataSource = null;
        //        GvPopUpEmpList.DataBind();
        //        txtBreakEndTime.Text = string.Empty;
        //        txtBreakStartTime.Text = string.Empty;
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];

        //        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
        //        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
        //        // CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));

        //        //if (chkworked.Checked == false)
        //        //{
        //        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //        //    return;
        //        //}
        //        if (ddlProjectName.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
        //            return;
        //        }
        //        if (ddstatus.SelectedIndex == 1)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //            return;
        //        }
        //        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //        lblbusName.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Value).ToString("dd-MMM-yyyy");
        //        ViewState["datebreak"] = lblDate.Value;
        //        ViewState["weekno"] = hfdweekno.Value;
        //        ViewState["ProjectName"] = ddlProjectName.SelectedValue;

        //        string regMINst = lblDate.Value + " 00:00";
        //        string regMAXst = lblDate.Value + " 23:59";
        //        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //        string regMinEnd = lblDate.Value + " 00:00";
        //        string regMAXend = DateTime.Parse(lblDate.Value).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

        //        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //        // hfddatebreak.Value = lblDate.Text;
        //        // hfdweeknopop.Value = hfdweekno.Value;
        //        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "BreakID");
        //        ViewState["dt"] = dt;
        //        GvPopUpEmpList.DataSource = ViewState["dt"];
        //        GvPopUpEmpList.DataBind();
        //        //up2.Update();
        //        //GetGridFill();

        //        ModalPopupEmpList.Show();
        //    }

        //    if (e.CommandName == "Save")
        //    {


        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];
        //        TextBox txtRegular = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegular"));


        //        Label lblTotal = ((Label)GVChildlist.Rows[index].FindControl("lblTotal"));
        //        HiddenField lblDate = ((HiddenField)GVChildlist.Rows[index].FindControl("lblDate"));
        //        HiddenField hfdStatus = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdStatus"));
        //        Label lblDay = ((Label)GVChildlist.Rows[index].FindControl("lblDay"));
        //        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //        DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[index].FindControl("ddstatus"));

        //        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
        //        if (ddlProjectName.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
        //            // absent(row.RowIndex);
        //            return;
        //        }

        //        if (ddstatus.SelectedIndex == 0)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            // absent(row.RowIndex);
        //            return;
        //        }
        //        if (ddstatus.SelectedIndex == 1)
        //        {
        //            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            absent(row.RowIndex);
        //            return;
        //        }
        //        if (txtRegularStartTime.Text == ",")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            return;
        //        }

        //        if (txtRegularEndTime.Text == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
        //            return;
        //        }
        //        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
        //        if (check == true)
        //        {


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

        //        DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='"+ ddlemp.SelectedValue + "' and TimeSheetDate='"+ DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

        //        TimeSpan tt = new TimeSpan();
        //        if(prvdate.Rows.Count>0)
        //        {
        //            for(int i =0;i<prvdate.Rows.Count;i++)
        //            {
        //                TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
        //                tt += prvtotTime;
        //            }

        //            tt += ToltalRegulahr;
        //            if(tt.TotalHours>=24)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
        //                return;
        //            }
        //        }

        //        //return;
        //        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

        //        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //        if (ToltalRegulahr < TotalBreakHr)
        //        {
        //            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //            return;
        //        }
        //        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //        Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
        //        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //        Tsee.WeekNo = int.Parse(weekno[0]);
        //        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
        //        Tsee.Day = lblweekDayName.Text;
        //        Tsee.Regular = ToltalRegulahr.ToString();
        //        Tsee.Over1_5x = Over15x[0];
        //        Tsee.Over2x = Over2x[0];
        //        Tsee.Holiday = holiday[0];
        //        Tsee.Total = TotalHr.ToString();
        //        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //        Tsee.BreakStart = BreakStartTime[0].ToString();
        //        Tsee.BreakEnd = BreakEndTime[0].ToString();
        //        if (hfdStatus.Value != "2")
        //        {
        //            Tsee.Status = "1";
        //        }
        //        else
        //        {
        //            Tsee.Status = "2";
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
        //        Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
        //        //TSC.Sp_InsertTimeSheetEntry(Tsee);
        //        TSC.Sp_InsertTimeSheetEntryProject(Tsee);

        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //        GetGridFill(int.Parse(ddlProjectName.SelectedValue));


        //    }
        //}
        //catch (Exception ex)
        //{
        //    throw ex;
        //}
    }
    protected void absent(int RowIndex)
    {
        try
        {
            //LinkButton lnkdelete = (LinkButton)sender;
            //GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
            HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RowIndex].FindControl("hfdweekno");

            Label lblTotal = ((Label)GVChildlist.Rows[RowIndex].FindControl("lblTotal"));
            HiddenField lblDate = ((HiddenField)GVChildlist.Rows[RowIndex].FindControl("lblDate"));
            Label lblDay = ((Label)GVChildlist.Rows[RowIndex].FindControl("lblDay"));

            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[RowIndex].FindControl("txtRegularStartTime"));
            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[RowIndex].FindControl("txtRegularEndTime"));
            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[RowIndex].FindControl("txtBreakStartTime"));
            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[RowIndex].FindControl("txtBreakEndTime"));
            DropDownList ddstatus = ((DropDownList)GVChildlist.Rows[RowIndex].FindControl("ddstatus"));

            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[RowIndex].FindControl("ddlProjectName"));
            string[] weekno = hfdweekno.Value.Split(',');
            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
            Tsee.WeekNo = int.Parse(weekno[0]);
            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
            Tsee.Day = lblweekDayName.Text;
            Tsee.Regular = "";
            Tsee.Over1_5x = "";
            Tsee.Over2x = "";
            Tsee.Holiday = "";
            Tsee.Total = "";
            Tsee.ReqularStart = null;
            Tsee.Regularend = null;
            Tsee.BreakStart = "";
            Tsee.BreakEnd = "";
            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
            Tsee.Worked = "No";
            Tsee.BreakTotal = "";
            DataTable dtcheck = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4 and ProjectId="+ Tsee.ProjectId + "", "");
            if (dtcheck.Rows.Count == 0)
            {
                Tsee.Status = "4";
                TSC.Sp_InsertTimeSheetEntryProject(Tsee);
            }
            else
            {
                //Tsee.Status = string.Empty;
                //TSC.Sp_InsertTimeSheetEntry(Tsee);
                //Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4");
            }

            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    protected void GVChildlist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddlProjectName = (DropDownList)e.Row.FindControl("ddlProjectName");
            HiddenField hfdWorked = (HiddenField)e.Row.FindControl("hfdWorked");
            HiddenField hfdprojId = (HiddenField)e.Row.FindControl("hfdprojId");
            CheckBox chkworked = (CheckBox)e.Row.FindControl("chkworked");
            HiddenField hfdStatus = e.Row.FindControl("hfdStatus") as HiddenField;
            UpdatePanel up1 = e.Row.FindControl("up1") as UpdatePanel;
            Label lblStatus = e.Row.FindControl("lblStatus") as Label;

            CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;

            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            DropDownList ddstatus = e.Row.FindControl("ddstatus") as DropDownList;


            //Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT", "PROJECTID", "dbo.FN_DESC('Project',PROJECTID)Pname", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "PROJECTID");

            //Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= GETDATE() OR B.EndDate IS NULL OR B.EndDate = '')", "A.PROJECTID", "dbo.FN_DESC('Project',A.PROJECTID)Pname", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "A.PROJECTID");

            Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= '" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' OR B.EndDate IS NULL OR B.EndDate = '')", "A.PROJECTID", "dbo.FN_DESC('Project',A.PROJECTID)Pname", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "A.PROJECTID");

            if (ddlProjectName.Items.Count == 1)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please extend the Project End Date as the project has been expired', 'error');", true);
            }
            
            if (hfdprojId.Value == "0")
            {
                ddlProjectName.Items.Add(new ListItem() { Value = "0", Text = "Regular Hours", Selected = true });
                ddlProjectName.Enabled = false;
            }
            ddlProjectName.SelectedValue = hfdprojId.Value;
            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";

                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
                ddlProjectName.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                ddstatus.Enabled = false;

                lnkSave.Enabled = false;
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                //e.Row.BackColor = System.Drawing.Color.DarkGray;
                ddlProjectName.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                ddstatus.Enabled = false;

                lnkSave.Enabled = false;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                ddlProjectName.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;

                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                ddstatus.Enabled = false;

                lnkSave.Enabled = false;
            }
            else if (hfdStatus.Value == "4")
            {
                lblStatus.Text = "Absent";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                //txtRegularStartTime.ReadOnly = true;
                ddlProjectName.Enabled = false;
                //txtBreakEndTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                //txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                //e.Row.BackColor = System.Drawing.Color.LightGray;
                //lnkGvApp.Enabled = false;
                //lnkGvReject.Enabled = false;
                //chkworked.Enabled = false;
                //chkApproved.Enabled = false;
                //ddstatus.Enabled = false;
                ddstatus.SelectedIndex = 1;
                //e.Row.ForeColor = System.Drawing.Color.White;
                //chkworked.Enabled = false;

                LinkButton LinkButton_Edit_Timesheet = e.Row.FindControl("LinkButton_Edit_Timesheet") as LinkButton;
                LinkButton_Edit_Timesheet.Visible = true;
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

            string regMINst = txtmonth.Text + " 00:00";
            string regMAXst = txtmonth.Text + " 23:59";
            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

            string regMinEnd = txtmonth.Text + " 00:00";
            //string regMAXend = DateTime.Parse(txtmonth.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";//commented to remove dd from end date
            string regMAXend = txtmonth.Text + " 23:59";

            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));

            //End date validation
            DataTable DataTable_chk_enddate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "EndDate,Is_Having_EndDate", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + ddlemp.SelectedValue + ")", "");
            if (DataTable_chk_enddate.Rows.Count > 0 && DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"].ToString() == "True")
            {
                if (DateTime.Parse(txtmonth.Text) > DateTime.Parse(DataTable_chk_enddate.Rows[0]["EndDate"].ToString()))
                {
                    e.Row.Enabled = false;
                }
            }
            if (DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"].ToString() == "False")
            {
                e.Row.Enabled = true;
            }
        }
    }

    protected void GVChildlist_PreRender(object sender, EventArgs e)
    {

    }

    protected void ddapproval_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddapproval = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddapproval.NamingContainer;
        int rIndex = gvr.RowIndex;
        if (ddapproval.SelectedIndex == 1)

        {
            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            HiddenField lblDate = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Value);
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
            DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddlProjectName") as DropDownList;
            DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
            if (ddlProjectName.SelectedValue == "")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                return;
            }

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
                //string msg = "For these " + lblDate.Value + " <br /> days timesheet has not been entered,<br /> do you want to approve as absent with“Yes”";
                //string msg = "For the selected date, time sheet has not been entered, If approved then status will be changed to Absent. Please confirm?";
                //Label3.Text = msg;
                hfdRIndex.Value = gvr.RowIndex.ToString();
                ModalPopupConfirmSingal.Show();
                return;

            }
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
            if (chkdata == false)
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please select Approve from Action Drop Down', 'warning');", true);
                return;
            }
            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "' and (ProjectId=" + ddlProjectName.SelectedValue + " or ProjectId is null)");
            if (a > 0)
            //if (a > 0)
            {
                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);

            }

        }

        if (ddapproval.SelectedIndex == 2)
        {
            HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
            HiddenField lblDate = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
            DateTime d = DateTime.Parse(lblDate.Value);
            TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
            DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
            DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddlProjectName") as DropDownList;
            if (ddlProjectName.SelectedValue == "")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                return;
            }

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
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please select Reject from Action Drop Down', 'warning');", true);
                return;
            }

            int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "' and ISNULL(ProjectId, 0)=" + ddlProjectName.SelectedValue + "");
            if (a > 0)
            {
                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
            }
        }

    }

    protected void ddlProjectName_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddlProjectName = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddlProjectName.NamingContainer;
        bool duplicate = false;
        if (GVChildlist.Rows.Count > 1)
        {
            for (int i = 0; i < GVChildlist.Rows.Count - 1; i++)
            {
                DropDownList ddlProjectName1 = (DropDownList)GVChildlist.Rows[i].FindControl("ddlProjectName");
                for (int j = i + 1; j < GVChildlist.Rows.Count; j++)
                {
                    DropDownList ddlProjectName2 = (DropDownList)GVChildlist.Rows[j].FindControl("ddlProjectName");
                    if (ddlProjectName1.SelectedValue == ddlProjectName2.SelectedValue)
                    {
                        duplicate = true;
                        break;
                    }
                }
                if (duplicate == true)
                {
                    break;
                }
            }
            if (duplicate == true)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Duplicate Project Name', 'error');", true);
                return;
            }
        }
        bool check = ddlProjectName.SelectedIndex != 0
            ? Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "")
            : false;
        if (check == true)
        {
            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
        }

        if (ddlProjectName.SelectedValue != "")
        {
            DataTable DataTable_chk_project_enddate = Comm.GetAllDataFromTable("AddProject with (Nolock)", "EndDate", "ProjectId=" + ddlProjectName.SelectedValue + " " +
            "and BusinessID=" + Request.QueryString["BusinessIdFromDefaultPage"] + "", "");
            if (DataTable_chk_project_enddate.Rows.Count > 0 && DataTable_chk_project_enddate.Rows[0]["EndDate"].ToString() != "")
            {
                if (DateTime.Parse(txtmonth.Text) > DateTime.Parse(DataTable_chk_project_enddate.Rows[0]["EndDate"].ToString()))
                {
                    //ddlProjectName.Enabled = false;
                    ddlProjectName.SelectedIndex = 0;
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Project has Ended so Time Sheet can not be Entered', 'error');", true);
                    //return;
                }
            }
            if (DataTable_chk_project_enddate.Rows[0]["EndDate"].ToString() == "")
            {
                ddlProjectName.Enabled = true;
            }
        }
    }

    protected void lnkAddRegularHours_Click(object sender, EventArgs e)
    {
        var regularHoursExists = false;
        for (var i = 0; i < GVChildlist.Rows.Count; i++)
        {
            var projectId = (HiddenField)GVChildlist.Rows[i].FindControl("hfdprojId");
            regularHoursExists = projectId.Value == "0" ? true : false;
            if (projectId.Value == "0") break;
        }
        DataTable dt = new DataTable();
        StringBuilder projid = new StringBuilder();
        DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
        if (dtsaveddata.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'First Enter Project Time Sheet Then Regular Time Sheet', 'info');", true);
            return;
        }
        if (regularHoursExists)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No more regular hours to be added', 'error');", true);
            return;
        }
        else
            AddNewRowToGrid(true);
    }

    protected void lnkAddMore_Click(object sender, EventArgs e)
    {
        //DataTable dtcnt = Comm.GetAllDataFromTable("ASSIGNPROJECT", "count(*)cnt", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "");

        //DataTable dtcnt = Comm.GetAllDataFromTable("ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= GETDATE() OR B.EndDate IS NULL OR B.EndDate = '')", "count(*)cnt", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "");
        
        DataTable dtcnt = Comm.GetAllDataFromTable("ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= '" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' OR B.EndDate IS NULL OR B.EndDate = '')", "count(*)cnt", "EMPLOYEEID=" + ddlemp.SelectedValue + "", "");

        int cnt = int.Parse(dtcnt.Rows[0]["cnt"].ToString());

        var regularHoursExists = false;
        for (var i = 0; i < GVChildlist.Rows.Count; i++)
        {
            var projectId = (HiddenField)GVChildlist.Rows[i].FindControl("hfdprojId");
            regularHoursExists = projectId.Value == "0" ? true : false;
            if (projectId.Value == "0") break;
        }
        if (cnt == GVChildlist.Rows.Count - (regularHoursExists ? 1 : 0))
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No More Project To Add', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'There are no additional projects for the selected employee', 'warning');", true);
            return;
        }
        AddNewRowToGrid();
    }

    private void AddNewRowToGrid(bool isRegular = false)
    {
        int rowIndex = 0;
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
            DataRow drCurrentRow = null;
            if (dtCurrentTable.Rows.Count > 0)
            {
                for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)
                {
                    DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[rowIndex].Cells[1].FindControl("ddlProjectName");
                    HiddenField lblDate = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblDate");
                    HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdweekno");
                    HiddenField hfdweekdayNo = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdweekdayNo");
                    HiddenField hfdWorked = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdWorked");
                    TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[rowIndex].Cells[1].FindControl("txtRegularStartTime");
                    TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[rowIndex].Cells[1].FindControl("txtRegularEndTime");
                    Label lblTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblTotal");
                    Label lblBreakTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblBreakTotal");
                    Label lblnetTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblnetTotal");
                    HiddenField hfdStatus = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdStatus");
                    HiddenField hfdprojId = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdprojId");
                    if (hfdprojId.Value != "0")
                    {
                        drCurrentRow = dtCurrentTable.NewRow();
                        drCurrentRow["RowNumber"] = i + 1;
                        drCurrentRow["SelDate"] = lblDate.Value;
                        drCurrentRow["weekno"] = hfdweekno.Value;
                        drCurrentRow["weekdayno"] = hfdweekdayNo.Value;
                        drCurrentRow["ProjectId"] = isRegular ? "0" : ddlProjectName.SelectedValue;
                        drCurrentRow["ProjectName"] = isRegular ? "0" : ddlProjectName.SelectedValue;

                        dtCurrentTable.Rows[i - 1]["ProjectName"] = ddlProjectName.SelectedValue;
                        dtCurrentTable.Rows[i - 1]["ProjectId"] = ddlProjectName.SelectedValue;
                        dtCurrentTable.Rows[i - 1]["SelDate"] = lblDate.Value;
                        dtCurrentTable.Rows[i - 1]["weekno"] = hfdweekno.Value;
                        dtCurrentTable.Rows[i - 1]["weekdayno"] = hfdweekdayNo.Value;
                        dtCurrentTable.Rows[i - 1]["Worked"] = hfdWorked.Value;
                        if (txtRegularStartTime.Text != "")
                        {
                            dtCurrentTable.Rows[i - 1]["ReqularStart"] = DateTime.Parse(txtRegularStartTime.Text).ToString("yyyy-MM-ddTHH:mm");
                            dtCurrentTable.Rows[i - 1]["Regularend"] = DateTime.Parse(txtRegularEndTime.Text).ToString("yyyy-MM-ddTHH:mm");//;// txtRegularEndTime.Text;
                        }
                        else
                        {
                            dtCurrentTable.Rows[i - 1]["ReqularStart"] = DBNull.Value;
                            dtCurrentTable.Rows[i - 1]["Regularend"] = DBNull.Value;
                        }
                        //firoz
                        if (lblTotal.Text.Length > 0)
                        {
                            dtCurrentTable.Rows[i - 1]["Regular"] = lblTotal.Text;
                            dtCurrentTable.Rows[i - 1]["BreakTotal"] = lblBreakTotal.Text;
                            dtCurrentTable.Rows[i - 1]["Total"] = lblnetTotal.Text;
                            dtCurrentTable.Rows[i - 1]["Status"] = hfdStatus.Value;
                        }
                        else
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please save the open project', 'warning');", true);
                            return;
                        }
                    }
                    rowIndex++;
                }

                dtCurrentTable.Rows.Add(drCurrentRow);
                ViewState["CurrentTable"] = dtCurrentTable;
                GVChildlist.DataSource = dtCurrentTable;
                GVChildlist.DataBind();
            }
        }
        SetPreviousData();
    }

    private void SetPreviousData()
    {
        int rowIndex = 0;
        if (ViewState["CurrentTable"] != null)
        {
            DataTable dt = (DataTable)ViewState["CurrentTable"];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[rowIndex].Cells[1].FindControl("ddlProjectName");
                    HiddenField lblDate = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblDate");
                    HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdweekno");
                    HiddenField hfdweekdayNo = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdweekdayNo");
                    HiddenField hfdWorked = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdWorked");
                    TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[rowIndex].Cells[1].FindControl("txtRegularStartTime");
                    TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[rowIndex].Cells[1].FindControl("txtRegularEndTime");
                    Label lblTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblTotal");
                    Label lblBreakTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblBreakTotal");
                    Label lblnetTotal = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblnetTotal");
                    HiddenField hfdStatus = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdStatus");
                    HiddenField hfdprojId = (HiddenField)GVChildlist.Rows[rowIndex].Cells[1].FindControl("hfdprojId");

                    ddlProjectName.SelectedValue = dt.Rows[i]["ProjectName"].ToString();
                    hfdprojId.Value = dt.Rows[i]["ProjectId"].ToString();
                    lblDate.Value = dt.Rows[i]["SelDate"].ToString();
                    hfdweekno.Value = dt.Rows[i]["weekno"].ToString();
                    hfdweekdayNo.Value = dt.Rows[i]["weekdayno"].ToString();
                    hfdWorked.Value = dt.Rows[i]["Worked"].ToString();
                    if (dt.Rows[i]["ReqularStart"].ToString() != "")
                    {
                        String regstr = dt.Rows[i]["ReqularStart"].ToString(); //.Substring(0,16);
                        txtRegularStartTime.Text = DateTime.Parse(regstr).ToString("yyyy-MM-ddTHH:mm"); //DateTime.Parse(dt.Rows[i]["ReqularStart"].ToString()).ToString("yyyy-MM-ddTHH:mm");
                    }

                    if (dt.Rows[i]["Regularend"].ToString() != "")
                    {
                        String regEnd = dt.Rows[i]["Regularend"].ToString();//.Substring(0, 16);
                        txtRegularEndTime.Text = DateTime.Parse(regEnd).ToString("yyyy-MM-ddTHH:mm");//DateTime.Parse(dt.Rows[i]["Regularend"].ToString()).ToString("yyyy-MM-ddTHH:mm");
                    }

                    lblTotal.Text = dt.Rows[i]["Regular"].ToString();
                    lblBreakTotal.Text = dt.Rows[i]["BreakTotal"].ToString();
                    lblnetTotal.Text = dt.Rows[i]["Total"].ToString();

                    rowIndex++;
                }
            }
        }
    }

    protected void btnAddBreak_Click(object sender, EventArgs e)
    {
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
        Abe.ProjectId = int.Parse(ViewState["ProjectName"].ToString());
        Abe.WeekNo = int.Parse(weekno[0]);
        Abe.TimeSheetDate = DateTime.Parse(a);

        Abe.Day = DateTime.Parse(a).DayOfWeek.ToString();
        Abe.BreakStart = txtBreakStartTime.Text;
        Abe.BreakEnd = txtBreakEndTime.Text;
        TimeSpan TotalBreakHr = DateTime.Parse(txtBreakEndTime.Text) - DateTime.Parse(txtBreakStartTime.Text);
        Abe.BreakTotal = TotalBreakHr.ToString();
        Abc.InsertAddBreak_Project(Abe);
        DataTable dt;
        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(a).ToString("yyyy-MM-dd") + "' and ProjectId=" + ViewState["ProjectName"].ToString() + "", "BreakID");
        ViewState["dt"] = dt;
        GvPopUpEmpList.DataSource = ViewState["dt"];
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();
        RegisterPostBackControl();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

        txtBreakStartTime.Text = "";
        txtBreakEndTime.Text = "";
    }
    
    private void RegisterPostBackControl()
    {
        foreach (GridViewRow row in GvPopUpEmpList.Rows)
        {
            LinkButton lnkEdit = row.FindControl("lnkEdit") as LinkButton;
            ScriptManager.GetCurrent(this).RegisterPostBackControl(lnkEdit);
        }
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

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {

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
                HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
                DateTime d = DateTime.Parse(lblDate.Value);
                DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
                if (txtRegularStartTime.Text == "" || txtRegularEndTime.Text == "")
                {
                    HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");

                    HiddenField lblDay = (HiddenField)row.FindControl("lblDay");
                    string[] weekno = hfdweekno.Value.Split(',');
                    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
                    Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                    Tsee.WeekNo = int.Parse(weekno[0]);
                    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
                    Tsee.Day = lblweekDayName.Text;
                    Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
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
                    TSC.Sp_InsertTimeSheetEntryProject(Tsee);
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
            GetGridFill_All();

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
        lnkApproved_Click(sender, e);

        //int RIndex = int.Parse(hfdRIndex.Value);
        //HiddenField HiddenField = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        //HiddenField lblDate = (HiddenField)GVChildlist.Rows[RIndex].FindControl("lblDate");
        //DateTime d = DateTime.Parse(lblDate.Value);
        //TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularStartTime") as TextBox;
        //TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularEndTime") as TextBox;
        //LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkAddBreak") as LinkButton;
        //LinkButton lnkSave = (LinkButton)GVChildlist.Rows[RIndex].FindControl("lnkSave") as LinkButton;
        //DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[RIndex].FindControl("ddlProjectName") as DropDownList;
        //int a;

        //bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and TimeSheetDate=" + d.ToString("yyyy-MM-dd") + "");
        //if (check == true)
        //{
        //    a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d.ToString("yyyy-MM-dd") + "'");
        //    if (a > 0)
        //    {
        //        GetGridFill(int.Parse(ddlProjectName.SelectedValue));

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
        //    Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
        //    Tsee.Day = lblweekDayName.Text;
        //    Tsee.Regular = "";
        //    Tsee.Over1_5x = "";
        //    Tsee.Over2x = "";
        //    Tsee.Holiday = "";
        //    Tsee.Total = "";
        //    Tsee.ReqularStart = null;
        //    Tsee.Regularend = null;
        //    Tsee.BreakStart = "";
        //    Tsee.BreakEnd = "";
        //    Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
        //    Tsee.Worked = "No";
        //    Tsee.BreakTotal = "";
        //    Tsee.Status = "4";
        //    TSC.Sp_InsertTimeSheetEntryProject(Tsee);
        //    a = 1;
        //    if (a > 0)
        //    {
        //        GetGridFill(int.Parse(ddlProjectName.SelectedValue));

        //        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        //    }
        //}
    }

    protected void lnkbtnNoSingal_Click(object sender, EventArgs e)
    {
        ModalPopupConfirmSingal.Hide();
    }

    public void Get_Selected_Calendar_Day()
    {
        //To reflect the day of date choosen from calendar control
        string get_month_from_date_control = txtmonth.Text;
        DateTime get_date_from_date_control;
        if (DateTime.TryParse(get_month_from_date_control, out get_date_from_date_control))
        {
            lblweekDayName.Text = get_date_from_date_control.DayOfWeek.ToString();
        }
        //To reflect the day of date choosen from calendar control
        lblweekDayName.Text = DateTime.Parse(txtmonth.Text).DayOfWeek.ToString();

        DataTable User_Pay_Rate_Display = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= '" + DateTime.Parse(txtmonth.Text) + "')", "");
        if (User_Pay_Rate_Display.Rows.Count > 0)
        {
            lblrate.Text = "Rate : $" + User_Pay_Rate_Display.Rows[0]["Rate"].ToString() + "/" + User_Pay_Rate_Display.Rows[0]["RateType"].ToString();
            GVChildlist.Enabled = true;
        }
        if (User_Pay_Rate_Display.Rows.Count == 0)
        {
            GVChildlist.Enabled = false;
            lblrate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
        }

        //EmployeeType Change date check and disable previous timesheetentry validation
        DataTable DataTable_chk_current_emptype = Comm.GetAllDataFromTable("AddEmployee with (Nolock)", "EMPLOYEETYPE", "EmployeeID=" + ddlemp.SelectedValue + "", "");
        DataTable DataTable_chk_emptypechangedate = Comm.GetAllDataFromTable("EmpType_Change_History with (Nolock)", "top 1 ChangeDate, EMPLOYEETYPE",
            "EmployeeID=" + ddlemp.SelectedValue + "", "1 desc");
        if (DataTable_chk_emptypechangedate.Rows.Count > 0 && DataTable_chk_current_emptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P"
            && (DataTable_chk_current_emptype.Rows[0]["EMPLOYEETYPE"].ToString() == DataTable_chk_emptypechangedate.Rows[0]["EMPLOYEETYPE"].ToString()))
        {
            if (DateTime.Parse(txtmonth.Text) < DateTime.Parse(DataTable_chk_emptypechangedate.Rows[0]["ChangeDate"].ToString()))
            {
                GVChildlist.Enabled = false;
            }
        }
    }

    //firoz 10
    private void ApproveTimesheet()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(" ");
        int rowUpdateCount = 0;
        double Hr = 0;

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            var hfdStatus = (HiddenField)row.FindControl("hfdStatus");
            var chkApproved = (CheckBox)row.FindControl("chkApproved");
            var ddstatus = (DropDownList)row.FindControl("ddstatus");
            if ((chkApproved.Checked || hfdStatus.Value.Length > 0)&& ddstatus.SelectedValue=="Yes")
            {
                var calculatedTiming = CalculatedTimingHours(row);
                Hr = Hr + (calculatedTiming.Item2 - calculatedTiming.Item1).TotalHours;
                if (Hr > 24)
                {
                    sb.Append("<br/>- Start Time and End Time Must be within 24Hrs");

                }
            }
        }
        if (Hr <= 24)
        {
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
                            Comm.ExecuteQuery(string.Format(@"Update 
                                                    TimeSheetEntry 
                                                    set     Status=2
                                                    where   TimeSheetEntryId={0}",
                                                        result
                                                            )
                                            );

                            rowUpdateCount++;
                        }
                    }
                }
            }
        }
        DisplayActionMessage(sb.ToString(), rowUpdateCount);
    }

    private void RejectTimesheet()
    {
        StringBuilder sb = new StringBuilder();
        //sb.Append("::Approval failed status::");
        sb.Append(" ");
        int rowUpdateCount = 0;
        double Hr = 0;

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            var hfdStatus = (HiddenField)row.FindControl("hfdStatus");
            var chkApproved = (CheckBox)row.FindControl("chkApproved");
            var ddstatus = (DropDownList)row.FindControl("ddstatus");
              if ((chkApproved.Checked || hfdStatus.Value.Length > 0) && ddstatus.SelectedValue == "Yes")
            {
                var calculatedTiming = CalculatedTimingHours(row);
                Hr = Hr + (calculatedTiming.Item2 - calculatedTiming.Item1).TotalHours;
                if (Hr > 24)
                {
                    sb.Append("<br/>- Start Time and End Time Must be within 24Hrs");

                }
            }
        }
        if (Hr <= 24)
        {
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
                            Comm.ExecuteQuery(string.Format(@"Update 
                                                    TimeSheetEntry 
                                                    set     Status=3
                                                    where   TimeSheetEntryId={0}",
                                                        result
                                                            )
                                            );

                            rowUpdateCount++;
                        }
                    }
                }
            }
        }
        DisplayActionMessage2(sb.ToString(), rowUpdateCount);
    }

    private Tuple<bool, string> ValidateSelectedGridItems(GridViewRow row)
    {
        StringBuilder sbInfo = new StringBuilder();
        var ddlStatus = (DropDownList)row.FindControl("ddstatus");
        var ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var hfdprojId = (HiddenField)row.FindControl("hfdprojId");
        sbInfo.Append("Row Index: " + row.RowIndex + 1);
        if (ddlProjectName.SelectedIndex == 0 && hfdprojId.Value != "0")
        {
            sbInfo.Append("<br/>- Project selection is required");
            return new Tuple<bool, string>(false, sbInfo.ToString());
        }
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

    private void DisplayActionMessage(string status, int updateCount)
    {
        if (updateCount > 0)
        {
            GetGridFill_All();
        }

        var statusMessage = status == "Approval failed status:" ? "" : status;

        //ScriptManager
        //    .RegisterClientScriptBlock(
        //                this.Page,
        //                this.GetType(),
        //                "SweetAlert",
        //                "swal({html: '<div>" + updateCount + " Time sheet got rejected<br/>" + statusMessage + "</div>'});",
        //            true);
        //ScriptManager
        //    .RegisterClientScriptBlock(
        //                this.Page,
        //                this.GetType(),
        //                "SweetAlert",
        //                "swal({html: '<div>" + updateCount + " Time sheet got rejected<br/>" + statusMessage + "</div>'});",
        //            true);
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);

        return;
    }

    private void DisplayActionMessage2(string status, int updateCount)
    {
        if (updateCount > 0)
        {
            GetGridFill_All();
        }

        var statusMessage = status == "Approval failed status:" ? "" : status;

        //ScriptManager
        //    .RegisterClientScriptBlock(
        //                this.Page,
        //                this.GetType(),
        //                "SweetAlert",
        //                "swal({html: '<div>" + updateCount + " Time sheet got rejected<br/>" + statusMessage + "</div>'});",
        //            true);
        //ScriptManager
        //    .RegisterClientScriptBlock(
        //                this.Page,
        //                this.GetType(),
        //                "SweetAlert",
        //                "swal({html: '<div>" + updateCount + " Time sheet got rejected<br/>" + statusMessage + "</div>'});",
        //            true);
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);

        return;
    }

    private int SaveTimesheet(GridViewRow row)
    {
        var lblDate = (HiddenField)row.FindControl("lblDate");
        var lblDay = (HiddenField)row.FindControl("lblDay");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var txtBreakStartTime = (TextBox)row.FindControl("txtBreakStartTime");
        var txtBreakEndTime = (TextBox)row.FindControl("txtBreakEndTime");
        var ddstatus = (DropDownList)row.FindControl("ddstatus");
        var ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");
        var hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        var hfdStatus = (HiddenField)row.FindControl("hfdStatus");
        var hfdprojId = (HiddenField)row.FindControl("hfdprojId");
        if (ddstatus.SelectedIndex == 1)
        {
            absent(row.RowIndex);
            return -1;
        }

        var computedItems = CalculatedTimingHours(row);
        string RegularStartTime = "0", RegularEndTime = "0";
        if (computedItems.Item4)
        {
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
        Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
        Tsee.Day = DateTime.Parse(txtmonth.Text).DayOfWeek.ToString();
        Tsee.Regular = ToltalRegulahr.ToString();
        Tsee.Over1_5x = "";
        Tsee.Over2x = "";
        Tsee.Holiday = "";
        Tsee.Total = TotalHr.ToString();
        Tsee.ReqularStart = DateTime.Parse(RegularStartTime.ToString());
        Tsee.Regularend = DateTime.Parse(RegularEndTime.ToString());
        Tsee.BreakStart = "";
        Tsee.BreakEnd = "";
        Tsee.Status = hfdStatus.Value == "2" ? "2" : "1";
        Tsee.Worked = ddstatus.SelectedIndex == 2 ? "Yes" : "No";
        Tsee.BreakTotal = computedItems.Item3.ToString();
        Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
        return TSC.Sp_InsertTimeSheetEntryProject(Tsee);
    }

    private Tuple<DateTime, DateTime, TimeSpan, bool> CalculatedTimingHours(GridViewRow row)
    {
        var lblDate = (HiddenField)row.FindControl("lblDate");
        var hfdweekno = (HiddenField)row.FindControl("hfdweekno");
        var hfdprojId = (HiddenField)row.FindControl("hfdprojId");
        var txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
        var txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
        var check = Comm.IfExistsInTable("TimeSheetEntry", "*",
            "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
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
            DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "' and projectId='" + hfdprojId.Value + "'", "");

        var totalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

        return new Tuple<DateTime, DateTime, TimeSpan, bool>(startDateTime, endDateTime, totalBreakHr, check);
    }
    //Firoz 27-12
    //private int SaveDefaultValueInTimeSheet(GridViewRow row)
    //{
    //    HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");
    //    Label lblDate = (Label)row.FindControl("lblDate");
    //    Label lblDay = (Label)row.FindControl("lblDay");
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
    //    return 1;
    //}
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
    {
        if (ddlemp.SelectedValue == "")
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Employee', 'warning');", true);
            ModalPopupEmployee.Show();
            return;
        }

        int check_timesheet_data = 0;
        int check_Current_Week_timesheet_data = 0;

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

            TextBox txtOver15x = (TextBox)row.FindControl("txtOver15x");
            TextBox txtOver2x = (TextBox)row.FindControl("txtOver2x");
            TextBox txtholiday = (TextBox)row.FindControl("txtholiday");
            //Label lblTotal = (Label)row.FindControl("lblTotal");
            HiddenField lblDate = (HiddenField)row.FindControl("lblDate");
            Label lblDay = (Label)row.FindControl("lblDay");
            HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");
            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            //TextBox txtBreakStartTime = (TextBox)row.FindControl("txtBreakStartTime");
            //TextBox txtBreakEndTime = (TextBox)row.FindControl("txtBreakEndTime");

            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

            HiddenField hfdprojId = (HiddenField)row.FindControl("hfdprojId");

            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "'");
            if (check)
            {
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

            //TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());

            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Value).ToString("yyyy-MM-dd") + "'", "");

            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

            //TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
            Tsee.BussID = int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString());
            Tsee.Month = DateTime.Parse(txtmonth.Text).AddDays(1).Month;
            Tsee.Year = DateTime.Parse(txtmonth.Text).AddDays(1).Year;

            //Tsee.WeekNo = int.Parse(weekno[0]);
            DateTime d = DateTime.Parse(lblDate.Value).AddDays(1);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            Tsee.WeekNo = weekNum;

            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Value);
            //Tsee.Day = lblweekDayName.Text;
            Tsee.Day = DateTime.Parse(txtmonth.Text).AddDays(1).DayOfWeek.ToString();
            //Tsee.Regular = ToltalRegulahr.ToString();
            Tsee.Over1_5x = Over15x[0];
            Tsee.Over2x = Over2x[0];
            Tsee.Holiday = holiday[0];
            //Tsee.Total = TotalHr.ToString();
            if (RegularStartTime[0].ToString() != string.Empty)
            {
                Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString()).AddDays(1);
                Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString()).AddDays(1);
            }
            Tsee.BreakStart = BreakStartTime[0].ToString();
            Tsee.BreakEnd = BreakEndTime[0].ToString();


            if (hfdprojId.Value == "0")
            {
                bool check_if_timesheet_exits = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");

                bool check_if_timesheet_exits_nextWeek = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");

                if (!check_if_timesheet_exits)
                    check_Current_Week_timesheet_data = 10;

                if (check_if_timesheet_exits && !check_if_timesheet_exits_nextWeek)
                {
                    var check_project_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ", "");

                    if (check_project_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_project_absent_time.Rows[0]["Regularend"].ToString() == "")
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                            " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");
                    }
                    else
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                            " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");
                    }

                    check_timesheet_data = 1;

                    check_Current_Week_timesheet_data = 0;

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                }

                if (check_if_timesheet_exits && check_if_timesheet_exits_nextWeek)
                {
                    int a = Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                        "and (ProjectId='0' or ProjectId IS NULL) and BussID='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and Status in (1,3,4)");

                    if (a > 0)
                    {
                        var check_project_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ", "");

                        if (check_project_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_project_absent_time.Rows[0]["Regularend"].ToString() == "")
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                                " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");
                        }
                        else
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                                " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId IS NULL ");
                        }

                        check_timesheet_data = 1;

                        check_Current_Week_timesheet_data = 0;

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                    }
                }
            }
            else
            {
                bool check_if_timesheet_exits = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");

                bool check_if_timesheet_exits_nextWeek = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");

                if (!check_if_timesheet_exits)
                    check_Current_Week_timesheet_data = 10;

                if (check_if_timesheet_exits && !check_if_timesheet_exits_nextWeek)
                {
                    var check_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ", "");

                    if (check_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_absent_time.Rows[0]["Regularend"].ToString() == "")
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                            " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                    }
                    else
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                            " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                    }

                    check_timesheet_data = 1;

                    check_Current_Week_timesheet_data = 0;

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                }

                if (check_if_timesheet_exits && check_if_timesheet_exits_nextWeek)
                {
                    int a = Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                        "and (ProjectId='" + hfdprojId.Value + "') and BussID='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and Status in (1,3,4)");

                    if (a > 0)
                    {
                        var check_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ", "");

                        if (check_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_absent_time.Rows[0]["Regularend"].ToString() == "")
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                                " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                        }
                        else
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Value).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                                " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Value).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                        }

                        check_timesheet_data = 1;

                        check_Current_Week_timesheet_data = 0;

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                    }
                }
            }
        }

        if (check_Current_Week_timesheet_data == 10)
        {
            ModalPopupTimesheetEnter.Show();
            return;
        }

        if (check_timesheet_data == 0)
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Timesheet already exists, so timesheet is not Copied', 'success');", true);
            ModalPopupTimesheetalready.Show();
        }
    }

    protected void lnkGvApp_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        HiddenField lblDate = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Value);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        //CheckBox chkworked = (CheckBox)GVChildlist.Rows[gvr.RowIndex].FindControl("chkworked") as CheckBox;

        DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddlProjectName") as DropDownList;

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
        ////bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        //bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
        //    "and (ProjectId IS NULL or ProjectId = 0 or ProjectId = '" + ddlProjectName.SelectedValue + "') ");
        //if (chkdata == false)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
        //    return;
        //}

        int a = 0;
        if (ddlProjectName.SelectedValue == "0")
        {
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId IS NULL or ProjectId = 0) ");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }
            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and ProjectId IS NULL");
        }
        else
        {
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId = '" + ddlProjectName.SelectedValue + "') ");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }
            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId IS NOT NULL or ProjectId != 0) ");
        }

        if (a > 0)
        {
            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is approved.', 'success');", true);
        }
    }

    protected void lnkGvReject_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        HiddenField lblDate = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Value);
        TextBox txtRegularStartTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime") as TextBox;
        TextBox txtRegularEndTime = (TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime") as TextBox;
        LinkButton lnkAddBreak = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkAddBreak") as LinkButton;
        LinkButton lnkSave = (LinkButton)GVChildlist.Rows[gvr.RowIndex].FindControl("lnkSave") as LinkButton;
        DropDownList ddstatus = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddstatus") as DropDownList;
        DropDownList ddlProjectName = (DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddlProjectName") as DropDownList;
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
        ////bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        //bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
        //    "and (ProjectId IS NULL or ProjectId = 0 or ProjectId = '" + ddlProjectName.SelectedValue + "') ");
        //if (chkdata == false)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
        //    return;
        //}

        int a = 0;
        if (ddlProjectName.SelectedValue == "0")
        {
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId IS NULL or ProjectId = 0) ");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }
            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and ProjectId IS NULL");
        }
        else
        {
            bool chkdata = Comm.IfExistsInTable("TimeSheetEntry", "*", "WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId = '" + ddlProjectName.SelectedValue + "') ");
            if (chkdata == false)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Save data first', 'error');", true);
                return;
            }
            a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "' " +
                "and (ProjectId IS NOT NULL or ProjectId != 0) ");
        }

        if (a > 0)
        {
            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The selected date time sheet is rejected.', 'success');", true);
        }
    }

    protected void Ddl_Business_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    public void Get_Business_Name_List()
    {
        DataTable Get_Business_Name_In_Ddl = Comm.GetAllDataFromTable("AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID,(ab.CompanyName)BusinessName", "EmployeeID=" + Session["EmployeeId"].ToString() + " and ab.Status=1", "ab.BusinessID");
        Ddl_Business.Items.Add("Select Business");
        Ddl_Business.SelectedItem.Value = "";
        Ddl_Business.DataSource = Get_Business_Name_In_Ddl;
        Ddl_Business.DataValueField = Get_Business_Name_In_Ddl.Columns[0].ToString();
        Ddl_Business.DataTextField = Get_Business_Name_In_Ddl.Columns[1].ToString();
        Ddl_Business.DataBind();
    }
}