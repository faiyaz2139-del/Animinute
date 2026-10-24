using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BusinessOwnerDashboard2 : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    GraphClass gp = new GraphClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"]==null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            // comm.FillDropDownList(ddlBussName, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId in (1,2,4)", "BusinessID");
            // comm.FillDropDownList(ddlBussName, "AssginBussiness assb join AddEmployee ad on ad.EmployeeID=assb.EmployeeID join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join UserAcc ua on ua.Email=ea.MainEmail ", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", "ua.EmployeeID= " + Session["EmployeeId"].ToString() + " and ad.UserType in (1,2,4,5) and assb.BusinessID is not null", "assb.BusinessID");
            // DataTable dt= comm.GetAllDataFromTable("AssginBussiness assb join AddEmployee ad on ad.EmployeeID=assb.EmployeeID join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join UserAcc ua on ua.Email=ea.MainEmail join  AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID) as BussName", "ua.EmployeeID= " + Session["EmployeeId"].ToString() + " and ad.UserType in (1,2,4,5) and assb.BusinessID is not null and ab.Status<>2", "assb.BusinessID");
            DataTable dt = comm.GetAllDataFromTable("AssginBussiness assb join AddEmployee ad on ad.EmployeeID=assb.EmployeeID join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join UserAcc ua on ua.Email=ea.MainEmail join  AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID,dbo.FN_DESC('AddBusiness',assb.BusinessID) as BussName", "ua.EmployeeID= " + Session["EmployeeId"].ToString() + " and assb.BusinessID is not null and ab.Status<>2", "assb.BusinessID");

            DataTable dtfilter = RemoveDuplicateRows(dt, "BusinessID");
            DataSet ds = new DataSet();
            ds.Tables.Add(dtfilter);
            ddlBussName.Items.Add("Please Select");
            ddlBussName.SelectedItem.Value = "";
            ddlBussName.DataSource = ds;
            ddlBussName.DataValueField = ds.Tables[0].Columns[0].ToString();
            ddlBussName.DataTextField = ds.Tables[0].Columns[1].ToString();
            ddlBussName.DataBind();
            ddlBussName.SelectedIndex = 0;
            if (dt.Rows.Count > 0)
            {
                DataTable dt1 = ds.Tables[0];
                string s = string.Join(", ", dt1.Rows.OfType<DataRow>().Select(r => r[0].ToString()));
                comm.FillDropDownList(ddlfig4, "TimeSheetEntry", "DISTINCT Year", " Year, Year as Yr", "BussID in (" + s + ")", "Yr desc");
            }
            Fig1(0);
            Fig2(0);
            Fig3(0);
            Fig4(0, DateTime.Now.Year);
            Fig5(0, DateTime.Now.Date);
        }
    }
    public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
    {
        Hashtable hTable = new Hashtable();
        ArrayList duplicateList = new ArrayList();

        //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
        //And add duplicate item value in arraylist.
        foreach (DataRow drow in dTable.Rows)
        {
            if (hTable.Contains(drow[colName]))
                duplicateList.Add(drow);
            else
                hTable.Add(drow[colName], string.Empty);
        }

        //Removing a list of duplicate items from datatable.
        foreach (DataRow dRow in duplicateList)
            dTable.Rows.Remove(dRow);

        //Datatable which contains unique records will be return as output.
        return dTable;
    }
    public void Fig1(int BusinessID)
    {
        DataTable dtBussOwnerNofBuss = new DataTable();
        //dtBussOwnerNofBuss = comm.GetAllDataFromTable("AssginBussiness", "top 10 count(EmployeeID) empcount,BusinessName,BusinessID", " UserTypeId not in(1,2) and BusinessID in (select BusinessID from AssginBussiness where UserTypeId=1 and EmployeeID=" + Session["EmployeeId"].ToString() + ") and (BusinessID=" + BusinessID + " or 0=" + BusinessID + ") group by BusinessName,BusinessID", "BusinessID");
        dtBussOwnerNofBuss = comm.GetAllDataFromTable("AssginBussiness", "count(EmployeeID) empcount,BusinessName,BusinessID", 
            " UserTypeId not in(1) and BusinessID in (select BusinessID from AssginBussiness where UserTypeId=1 and EmployeeID=" + Session["EmployeeId"].ToString() + ") and (BusinessID=" + BusinessID + " or 0=" + BusinessID + ") group by BusinessName,BusinessID", "BusinessID");
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
        //     

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
            DataTable dtUType = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ea.EmpAddressId=ad.ADDRESSID join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID ", "ad.UserType,ad.EmployeeID", "assb.BusinessID=" + ddlBussName.SelectedValue + " and ea.MainEmail ='" + Session["email"].ToString() + "'", "");

            if (dtUType.Rows[0]["UserType"].ToString().Trim() == "1")
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
            else if(dtUType.Rows[0]["UserType"].ToString().Trim() == "5")
            {
                //Fig1(int.Parse(ddlBussName.SelectedValue));
                int empid = int.Parse(dtUType.Rows[0]["EmployeeID"].ToString().Trim());
                Fig2owner(int.Parse(ddlBussName.SelectedValue), empid);
                Fig3owner(int.Parse(ddlBussName.SelectedValue), empid);
                Fig5owner(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Date, empid);


                if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
                {
                    Fig4owner(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year, empid);
                }
                else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
                {
                    Fig4owner(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue), empid);
                }
            }
            else if (dtUType.Rows[0]["UserType"].ToString().Trim() == "4" || dtUType.Rows[0]["UserType"].ToString().Trim()=="2")
            {
                int empid = int.Parse(dtUType.Rows[0]["EmployeeID"].ToString().Trim());
                Fig2Emp_Manager(int.Parse(ddlBussName.SelectedValue), empid, dtUType.Rows[0]["UserType"].ToString().Trim());
                Fig3Emp_Manager(int.Parse(ddlBussName.SelectedValue), empid, dtUType.Rows[0]["UserType"].ToString().Trim());
                Fig5Emp_Manager(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Date, empid, dtUType.Rows[0]["UserType"].ToString().Trim());


                if (ddlfig4.SelectedValue == "" && ddlBussName.SelectedValue != "")
                {
                    Fig4Emp_Manager(int.Parse(ddlBussName.SelectedValue), DateTime.Now.Year, empid, dtUType.Rows[0]["UserType"].ToString().Trim());
                }
                else if (ddlfig4.SelectedValue != "" && ddlBussName.SelectedValue != "")
                {
                    Fig4Emp_Manager(int.Parse(ddlBussName.SelectedValue), int.Parse(ddlfig4.SelectedValue), empid, dtUType.Rows[0]["UserType"].ToString().Trim());
                }
            }
        }
    }
    #region owner
    public void Fig2owner(int BusinessID, int EmpId)
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
        // int EmpId = int.Parse(Session["EmployeeId"].ToString());
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


    public void Fig3owner(int BusinessID, int EmpId)
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

    public void Fig4owner(int BusinessID, int year, int EmpId)
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
    public void Fig5owner(int BusinessID, DateTime year, int EmpId)
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

    #region Emp_Manager
    public void Fig2Emp_Manager(int BusinessID,int EmpId,string Userid)
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
        if (Userid == "4")
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


    public void Fig3Emp_Manager(int BusinessID, int EmpId, string Userid)
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
        //  int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());

        if (Userid == "4")
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

    public void Fig4Emp_Manager(int BusinessID, int year, int EmpId, string Userid)
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
        if (Userid == "4")
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

    public void Fig5Emp_Manager(int BusinessID, DateTime year, int EmpId, string Userid)
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
        //  int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());

        if (Userid == "4")
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
    #endregion

    public void Fig2(int BusinessID)
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
        dtBussOwnerNofBuss = gp.BoWeekly(d, BusinessID, EmpId, MotherId);

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
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.BoMonthly(d, BusinessID, EmpId, MotherId);

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
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.BoQuarterly(year, BusinessID, EmpId, MotherId);

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
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        //DateTime d = DateTime.Now.Date;
        // int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        //DateTime d = DateTime.Now.Date;
        int EmpId = int.Parse(Session["EmployeeId"].ToString());
        int MotherId = int.Parse(Session["MotherOrgid"].ToString());
        dtBussOwnerNofBuss = gp.BoYearly(year, BusinessID, EmpId, MotherId);

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

    protected void radiored_CheckedChanged(object sender, EventArgs e)
    {
        ddlBussName.SelectedValue = "";
        VarticalGraph.Visible = false;
        HorizontalGraph.Visible = true;
        lineGraph.Visible = false;
    }

    protected void radioblue_CheckedChanged(object sender, EventArgs e)
    {
        ddlBussName.SelectedValue = "";
        VarticalGraph.Visible = true;
        HorizontalGraph.Visible = false;
        lineGraph.Visible = false;
    }

    protected void radioLine_CheckedChanged(object sender, EventArgs e)
    {
        ddlBussName.SelectedValue = "";
        lineGraph.Visible = true;
        VarticalGraph.Visible = false;
        HorizontalGraph.Visible = false;
    }

    protected void DropDownList_GraphName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropDownList_GraphName.SelectedIndex == 0)
        {
            //ddlBussName.SelectedValue = "";
            VarticalGraph.Visible = true;
            HorizontalGraph.Visible = false;
            lineGraph.Visible = false;
        }
        if (DropDownList_GraphName.SelectedIndex == 1)
        {
            //ddlBussName.SelectedValue = "";
            VarticalGraph.Visible = false;
            HorizontalGraph.Visible = true;
            lineGraph.Visible = false;
        }
        if (DropDownList_GraphName.SelectedIndex == 2)
        {
            //ddlBussName.SelectedValue = "";
            lineGraph.Visible = true;
            VarticalGraph.Visible = false;
            HorizontalGraph.Visible = false;
        }
    }
}