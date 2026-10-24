using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class AccountantDashBoard2 : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    GraphClass gp = new GraphClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            CardView();
            Weekly();
            Monthly();
            comm.FillDropDownList(ddlfig3, "AddBusiness", "distinct DATEPART(year,BusinessAgreementDate)", " DATEPART(year,BusinessAgreementDate), DATEPART(year,BusinessAgreementDate) as Yr", "MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "Yr desc");
            Quatarly(int.Parse(DateTime.Now.Year.ToString()));
            Yearly();
        }
    }
    public void CardView()
    {
        DataTable dt = new DataTable();
        dt = gp.AccountanCard(int.Parse(Session["MotherOrgid"].ToString()));
        if (dt.Rows.Count > 0)
        {
            lblbuss.InnerText = "No. of Company " + dt.Rows[0]["Business"].ToString();
            lblbussOwner.InnerText = "No. of Subscriber " + dt.Rows[0]["BusinessOwner"].ToString();
            lblep.InnerText = "No. of Employee " + dt.Rows[0]["Employee"].ToString();
            lblsuper.InnerText = "No. of Manager " + dt.Rows[0]["SUPERVISOR"].ToString();
        }

    }
    public void Weekly()
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
        dtBussOwnerNofBuss = gp.AccountantWeekly(MotherId, d);

        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["BussCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["BussOwnerCnt"].ToString();
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

    public void Monthly()
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        dtBussOwnerNofBuss = gp.AccountantMonthly(int.Parse(Session["MotherOrgid"].ToString()), d);
        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["date1"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["BussCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["BussOwnerCnt"].ToString();
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

            hfdCatagoriesYR.Value = jsoncategories;
            hfdsubmitedfiYR.Value = jsonsubmited;
            hfdpendingYR.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesYR.Value = "";
            hfdsubmitedfiYR.Value = "";
            hfdpendingYR.Value = "";

        }
    }


    public void Quatarly(int Year)
    {
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        dtBussOwnerNofBuss = gp.AccountantQutar(int.Parse(Session["MotherOrgid"].ToString()), Year);
        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["BussCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["BussOwnerCnt"].ToString();
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

            hfdCatagoriesQR.Value = jsoncategories;
            hfdsubmitedfiQR.Value = jsonsubmited;
            hfdpendingQR.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesQR.Value = "";
            hfdsubmitedfiQR.Value = "";
            hfdpendingQR.Value = "";

        }
    }

    protected void ddlfig3_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(ddlfig3.SelectedValue!="")
        {
            Quatarly(int.Parse(ddlfig3.SelectedValue));
        }
        else
        {
            Quatarly(int.Parse(DateTime.Now.Year.ToString()));
        }
        
    }


    public void Yearly()
    {
        int Year = DateTime.Now.Year;
        string categories = "";
        string submited = "";
        string pending = "";
        //string Approved = "";
        DataTable dtBussOwnerNofBuss = new DataTable();
        CultureInfo cul = CultureInfo.CurrentCulture;
        DateTime d = DateTime.Now.Date;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        dtBussOwnerNofBuss = gp.AccountantYearly(int.Parse(Session["MotherOrgid"].ToString()), Year);
        string JsonString = string.Empty;

        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["WeekName"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            submited = submited + dtBussOwnerNofBuss.Rows[i]["BussCnt"].ToString();
            submited += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            pending = pending + dtBussOwnerNofBuss.Rows[i]["BussOwnerCnt"].ToString();
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

            hfdCatagoriesYer.Value = jsoncategories;
            hfdsubmitedfiYer.Value = jsonsubmited;
            hfdpendingYer.Value = jsonpending;

        }
        else
        {
            hfdCatagoriesYer.Value = "";
            hfdsubmitedfiYer.Value = "";
            hfdpendingYer.Value = "";

        }
    }
}