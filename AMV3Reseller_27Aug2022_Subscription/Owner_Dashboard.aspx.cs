using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class OwnerDashBoard : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    GraphClass gp = new GraphClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        // SECURITY: Owner Dashboard is restricted to authenticated Owner users.
        // This check runs before any dashboard data is queried or rendered.
        if (Session["UserId"] == null ||
            Session["EmployeeId"] == null ||
            Session["UserTypeid"] == null ||
            Session["UserType"] == null ||
            Session["MotherOrgid"] == null)
        {
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        // Authorize Owner Dashboard from the persisted business-owner assignment.
        // Existing Owner dashboard queries already use AssginBussiness.UserTypeId = 5.
        DataTable dtOwnerAccess = comm.GetAllDataFromTable(
            "AssginBussiness",
            "TOP 1 EmployeeID",
            "EmployeeID=" + Convert.ToInt32(Session["EmployeeId"]) +
            " and MotherOrgID=" + Convert.ToInt32(Session["MotherOrgid"]) +
            " and UserTypeId=5",
            "");

        if (dtOwnerAccess.Rows.Count == 0)
        {
            Response.Redirect("Employee_Dashboard.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
            return;
        }

        if (!IsPostBack)
        {
            //comm.FillDropDownList(ddlBussName, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='"+ Session["email"].ToString() + "') and UserTypeId in (1,2,4,5)", "BusinessID");
            //comm.FillDropDownList(ddlBussName, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') ", "BusinessID");
            
            //commented by kumar because there is problem in shopwing the business because of motherorgid
            comm.FillDropDownList(ddlBussName, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)", "EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') ", "BusinessID");
            comm.FillDropDownList(ddlfig4, "TimeSheetEntry", "DISTINCT Year", " Year, Year as Yr", "", "Yr desc");
            Fig1(0);
            Fig2(0);
            Fig3(0);
            Fig4(0, DateTime.Now.Year);
            Fig5(0, DateTime.Now.Date);
        }
    }
    public void Fig1(int BusinessID)
    {
        DataTable dtBussOwnerNofBuss = new DataTable();
        dtBussOwnerNofBuss = comm.GetAllDataFromTable("AssginBussiness", "top 10 count(EmployeeID) empcount,BusinessName,BusinessID", "MotherOrgID='"+ Session["MotherOrgid"].ToString() + "' and UserTypeId not in(1,2) and BusinessID in (select BusinessID from AssginBussiness where UserTypeId=5 and EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "') ) and (BusinessID=" + BusinessID + " or 0=" + BusinessID + ") group by BusinessName,BusinessID", "BusinessID");
        string categories = "";
        string series = "";
        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["BusinessName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            series = series + dtBussOwnerNofBuss.Rows[i]["empcount"].ToString();
            series += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;
        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {


            series = series.Remove(series.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);

            var Arraycategories = categories.Split(',');
            var Arrayseries = series.Split(',');
            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstseries = Arrayseries.OfType<string>().ToList();
            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonseries = JsonConvert.SerializeObject(lstseries).ToString();
            fig1Series.Value = jsonseries;
            figcategories.Value = jsoncategories;
        }
        else
        {
            fig1Series.Value = "";
            figcategories.Value = "";
        }
    }

    protected void ddlBussName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlBussName.SelectedValue == "")
        {
            Fig1(0);
            Fig2(0);
            Fig3(0);
            Fig5(0, DateTime.Now.Date);
            if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue == "")
            {
                Fig4(0, DateTime.Now.Year);
            }
            else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue == "")
            {
                Fig4(0, int.Parse(ddlfig4.SelectedValue));
            }

        }
        else
        {
            Fig1(int.Parse(ddlBussName.SelectedValue));
            Fig2(int.Parse(ddlBussName.SelectedValue));
            Fig3(int.Parse(ddlBussName.SelectedValue));
            Fig5(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Date);

            if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
            {
                Fig4(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year);
            }
            else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
            {
                Fig4(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue));
            }
        }
       
        
    }

    public void Fig2(int BusinessID)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        string Approved = "";
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


    public void Fig3(int BusinessID)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        string Approved = "";
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

    public void Fig4(int BusinessID, int year)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        string Approved = "";
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

    protected void ddlfig4_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue == "")
        {
            Fig4(0, DateTime.Now.Year);
        }
        else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue == "")
        {
            Fig4(0, int.Parse(ddlfig4.SelectedValue));
        }
        else if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
        {
            Fig4(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year);
        }
        else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
        {
            Fig4(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue));
        }
    }

    public void Fig5(int BusinessID, DateTime year)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        string Approved = "";
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
}