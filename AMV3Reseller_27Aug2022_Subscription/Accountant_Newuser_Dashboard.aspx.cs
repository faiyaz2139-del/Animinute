using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Accountant_NewUser_DashBoard : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    GraphClass gp = new GraphClass();
    // v27 (live: accountant kk3 landed on this old Main.master page - empty charts, clipped sidebar, stale build meta).
    // Accountant logins (UserTypeId 12) now use the modern Dashboard.aspx with role-aware copy and an explicit empty state.
    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] != null) Response.Redirect("Dashboard.aspx", true);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            //comm.FillDropDownList(ddlBussName, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", "assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') and UserTypeId in (1,2,4,5,12) and ab.Status<>2", "BusinessID");
            //comm.FillDropDownList(ddlBussName, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", "assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') and ab.Status<>2", "BusinessID");

            //commented by kumar because there is problem in shopwing the business because of motherorgid
            comm.FillDropDownList(ddlBussName, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", "EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') and ab.Status<>2", "BusinessID");
            comm.FillDropDownList(ddlfig4, "TimeSheetEntry", "DISTINCT Year", " Year, Year as Yr", "", "Yr desc");
            int EmpId = int.Parse(Session["EmployeeId"].ToString());
            string UserID = Session["UserTypeid"].ToString();
            Fig2(0, EmpId, UserID);
            Fig3(0, EmpId, UserID);
            Fig4(0, DateTime.Now.Year, EmpId, UserID);
            Fig5(0, DateTime.Now.Date, EmpId, UserID);
        }
    }
   

    protected void ddlBussName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBussName.SelectedValue == "")
        {
            int EmpId = int.Parse(Session["EmployeeId"].ToString());
            string UserID = Session["UserTypeid"].ToString();
            Fig2(0, EmpId, UserID);
            Fig3(0, EmpId, UserID);
            Fig5(0, DateTime.Now.Date, EmpId, UserID);
            if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue == "")
            {
                Fig4(0, DateTime.Now.Year, EmpId, UserID);
            }
            else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue == "")
            {
                Fig4(0, int.Parse(ddlfig4.SelectedValue), EmpId, UserID);
            }

        }
        else
        {
            DataTable dtUType = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID ", "ad.UserType,ad.EmployeeID", "assb.BusinessID=" + ddlBussName.SelectedValue + " and ea.MainEmail ='" + Session["email"].ToString() + "'", "");
            int EmpId= int.Parse(dtUType.Rows[0]["EmployeeID"].ToString().Trim());
            
            if(dtUType.Rows[0]["UserType"].ToString().Trim()=="4" || dtUType.Rows[0]["UserType"].ToString().Trim() == "2")
            {
                Fig2(int.Parse(ddlBussName.SelectedValue), EmpId, dtUType.Rows[0]["UserType"].ToString().Trim());
                Fig3(int.Parse(ddlBussName.SelectedValue), EmpId, dtUType.Rows[0]["UserType"].ToString().Trim());
                Fig5(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Date, EmpId, dtUType.Rows[0]["UserType"].ToString().Trim());
                if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
                {
                    Fig4(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year, EmpId, dtUType.Rows[0]["UserType"].ToString().Trim());
                }
                else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
                {
                    Fig4(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue), EmpId, dtUType.Rows[0]["UserType"].ToString().Trim());
                }
            }
            if(dtUType.Rows[0]["UserType"].ToString().Trim() == "5")
            {
                Fig2Owner(int.Parse(ddlBussName.SelectedValue));
                Fig3Owner(int.Parse(ddlBussName.SelectedValue));
                Fig5Owner(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Date);

                 if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
                {
                    Fig4Owner(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year);
                }
                else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
                {
                    Fig4Owner(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue));
                }
            }



        }
        
        
    }

    public void Fig2(int BusinessID, int EmpId,string UserTypeid)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        //int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        if(UserTypeid=="12")
        {
            dtBussOwnerNofBuss = gp.EmpWeekly(d, BusinessID, EmpId, MotherId);
        }
        else

        {
            dtBussOwnerNofBuss = gp.SupWeekly(d, BusinessID, EmpId, MotherId);
        }

        

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig2.Value = jsoncategories;
            hfdsubmitedfig2.Value = jsonsubmited;
            hfdpendingfig2.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig2.Value = "";
            hfdsubmitedfig2.Value = "";
            hfdpendingfig2.Value = "";

        }
    }


    public void Fig3(int BusinessID, int EmpId, string UserTypeid)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        //int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());

        if (UserTypeid == "12")
        {
            dtBussOwnerNofBuss = gp.EmpMonthly(d, BusinessID, EmpId, MotherId);
        }
        else

        {
            dtBussOwnerNofBuss = gp.SupMonthly(d, BusinessID, EmpId, MotherId);
        }
        

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["MonthName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig3.Value = jsoncategories;
            hfdsubmitedfig3.Value = jsonsubmited;
            hfdpendingfig3.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig3.Value = "";
            hfdsubmitedfig3.Value = "";
            hfdpendingfig3.Value = "";

        }
    }

    public void Fig4(int BusinessID, int year, int EmpId, string UserTypeid)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        //int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        if (UserTypeid == "12")
        {
            dtBussOwnerNofBuss = gp.EmpQuarterly(year, BusinessID, EmpId, MotherId);
        }
        else

        {
            dtBussOwnerNofBuss = gp.SupQuarterly(year, BusinessID, EmpId, MotherId);
        }
       

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig4.Value = jsoncategories;
            hfdsubmitedfig4.Value = jsonsubmited;
            hfdpendingfig4.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig4.Value = "";
            hfdsubmitedfig4.Value = "";
            hfdpendingfig4.Value = "";

        }
    }

    protected void ddlfig4_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtUType = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID ", "ad.UserType,ad.EmployeeID", "assb.BusinessID=" + ddlBussName.SelectedValue + " and ea.MainEmail ='" + Session["email"].ToString() + "'", "");
        int EmpId = int.Parse(dtUType.Rows[0]["EmployeeID"].ToString().Trim());
        string UserID = dtUType.Rows[0]["UserType"].ToString().Trim();
        if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue == "")
        {
            Fig4(0, DateTime.Now.Year, EmpId, UserID);
        }
        else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue == "")
        {
            Fig4(0, int.Parse(ddlfig4.SelectedValue), EmpId, UserID);
        }
        else if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
        {
            Fig4(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year, EmpId, UserID);
        }
        else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
        {
            Fig4(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue), EmpId, UserID);
        }
    }

    public void Fig5(int BusinessID, DateTime year, int EmpId, string UserTypeid)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        // int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());

        if (UserTypeid == "12")
        {
            dtBussOwnerNofBuss = gp.EmpYearly(year, BusinessID, EmpId, MotherId);
        }
        else

        {
            dtBussOwnerNofBuss = gp.SupYearly(year, BusinessID, EmpId, MotherId);
        }
       

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["MonthName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig5.Value = jsoncategories;
            hfdsubmitedfig5.Value = jsonsubmited;
            hfdpendingfig5.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig5.Value = "";
            hfdsubmitedfig5.Value = "";
            hfdpendingfig5.Value = "";

        }
    }

    #region Owner
    public void Fig2Owner(int BusinessID)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.OwnerWeekly(d, BusinessID, EmpId, MotherId);

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig2.Value = jsoncategories;
            hfdsubmitedfig2.Value = jsonsubmited;
            hfdpendingfig2.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig2.Value = "";
            hfdsubmitedfig2.Value = "";
            hfdpendingfig2.Value = "";

        }
    }


    public void Fig3Owner(int BusinessID)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.OwnerMonthly(d, BusinessID, EmpId, MotherId);

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["MonthName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig3.Value = jsoncategories;
            hfdsubmitedfig3.Value = jsonsubmited;
            hfdpendingfig3.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig3.Value = "";
            hfdsubmitedfig3.Value = "";
            hfdpendingfig3.Value = "";

        }
    }

    public void Fig4Owner(int BusinessID, int year)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.OwnerQuarterly(year, BusinessID, EmpId, MotherId);

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig4.Value = jsoncategories;
            hfdsubmitedfig4.Value = jsonsubmited;
            hfdpendingfig4.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig4.Value = "";
            hfdsubmitedfig4.Value = "";
            hfdpendingfig4.Value = "";

        }
    }


    public void Fig5Owner(int BusinessID, DateTime year)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.OwnerYearly(year, BusinessID, EmpId, MotherId);

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["MonthName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["EmpCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["HrCnt"].ToString();
            pending += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            submited = submited.Remove(submited.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            pending = pending.Remove(pending.Length - 1, 1);


            var Arraycategories = categories.Split(',');
            var Arraysubmited = submited.Split(',');
            var Arraypending = pending.Split(',');

            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstsubmited = Arraysubmited.OfType<string>().ToList();
            List<string> lstpending = Arraypending.OfType<string>().ToList();


            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonsubmited = JsonConvert.SerializeObject(lstsubmited).ToString();
            string jsonpending = JsonConvert.SerializeObject(lstpending).ToString();

            hfdCatagoriesfig5.Value = jsoncategories;
            hfdsubmitedfig5.Value = jsonsubmited;
            hfdpendingfig5.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesfig5.Value = "";
            hfdsubmitedfig5.Value = "";
            hfdpendingfig5.Value = "";

        }
    }
    #endregion
}