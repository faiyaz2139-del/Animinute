using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ManagerDashboard : System.Web.UI.Page
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
            ManagerEmpCountGraph();
           
            comm.FillDropDownList_NoPleaseSelect(ddlfig1, "AssgineManager", "distinct BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)bussname", " ManagerID=" + Session["EmployeeId"].ToString() + "", "BusinessID");
            comm.FillDropDownList_NoPleaseSelect(ddlfig3, "AssgineManager", "distinct BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)bussname", " ManagerID=" + Session["EmployeeId"].ToString() + "", "BusinessID");
            ddlfig1_SelectedIndexChanged(sender, e);
            ddlfig3_SelectedIndexChanged(sender, e);
        }
    }

    public void ManagerEmpCountGraph()
    {
        DataTable dtBussOwnerNofBuss = new DataTable();
        dtBussOwnerNofBuss = gp.ManagerEmpCountGraph(int.Parse(Session["EmployeeId"].ToString()), int.Parse(Session["MotherOrgid"].ToString()));
        string categories = "";
        string series = "";
        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["bussname"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            series = series + dtBussOwnerNofBuss.Rows[i]["cnt"].ToString();
            series += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;
        }
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

    protected void ddlfig3_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtBussOwnerNofBuss = new DataTable();
        DateTime d = DateTime.Now.Date;
        CultureInfo cul = CultureInfo.CurrentCulture;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        dtBussOwnerNofBuss = gp.MangerEmpWeekGraph(int.Parse(Session["EmployeeId"].ToString()),52 ,int.Parse(ddlfig3.SelectedValue));
        string categories = "";
        string currentweek = "", Prevweek = "";

        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {
            currentweek = dtBussOwnerNofBuss.Rows[0]["allweek"].ToString();
            var Arrayseries = currentweek.Split(',');

            int[] asIntegers = Arrayseries.Select(s => int.Parse(s)).ToArray();
            List<int> lstcategories = asIntegers.OfType<int>().ToList();
            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();



            hfdcurrentweekfig3.Value = jsoncategories;

        }
        else
        {

            hfdcurrentweekfig3.Value = "";

        }
    }

    protected void ddlfig1_SelectedIndexChanged(object sender, EventArgs e)
    {
        DataTable dtBussOwnerNofBuss = new DataTable();
        DateTime d = DateTime.Now.Date;
        CultureInfo cul = CultureInfo.CurrentCulture;
        int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        dtBussOwnerNofBuss = gp.MangerHrWeekGraph(int.Parse(Session["EmployeeId"].ToString()), 52, int.Parse(ddlfig3.SelectedValue));
        string categories = "";
        string currentweek = "", Prevweek = "";
        for (int i = 0; i < dtBussOwnerNofBuss.Rows.Count; i++)
        {
            categories = categories + dtBussOwnerNofBuss.Rows[i]["Empname"].ToString();
            categories += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            currentweek = currentweek + dtBussOwnerNofBuss.Rows[i]["currentweek"].ToString();
            currentweek += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;

            Prevweek = Prevweek + dtBussOwnerNofBuss.Rows[i]["Prevweek"].ToString();
            Prevweek += (i < dtBussOwnerNofBuss.Rows.Count) ? "," : string.Empty;
        }
        if (dtBussOwnerNofBuss.Rows.Count > 0)
        {
            currentweek = currentweek.Remove(currentweek.Length - 1, 1);
            categories = categories.Remove(categories.Length - 1, 1);
            Prevweek = Prevweek.Remove(Prevweek.Length - 1, 1);
            var Arraycategories = categories.Split(',');
            var Arrayseries = currentweek.Split(',');
            var ArrayPrevweek = Prevweek.Split(',');
            List<string> lstcategories = Arraycategories.OfType<string>().ToList();
            List<string> lstseries = Arrayseries.OfType<string>().ToList();
            List<string> lstPrevweek = ArrayPrevweek.OfType<string>().ToList();
            string jsoncategories = JsonConvert.SerializeObject(lstcategories).ToString();
            string jsonseries = JsonConvert.SerializeObject(lstseries).ToString();
            string jsonPrevweek = JsonConvert.SerializeObject(lstPrevweek).ToString();

            hfdCatagoriesfig3.Value = jsoncategories;
            hfdcurrentweek.Value = jsonseries;
            hfdpPrevweek.Value = jsonPrevweek;
        }
        else
        {
            hfdCatagoriesfig3.Value = "";
            hfdcurrentweek.Value = "";
            hfdpPrevweek.Value = "";
        }
    }
}