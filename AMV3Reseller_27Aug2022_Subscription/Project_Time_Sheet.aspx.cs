using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Project_Time_Sheet : System.Web.UI.Page
{
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
            lblWeek.Text = DateTime.Parse(txtmonth.Text).ToString("dddd");

            if (Request.QueryString["BidfromBusOwner"] != null)
            {
                lblEmpName.Text = Comm.GetNameFromId("AddEmployee", int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()));
                GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString());
                HyperLink_Project_Employee_Regular_Hours.NavigateUrl = "~/Project_Time_Sheet_Regular_Hours.aspx?EmpidfromBusOwner=" + Request.QueryString["EmpidfromBusOwner"].ToString() + "&BidfromBusOwner=" + Request.QueryString["BidfromBusOwner"].ToString();
            }
            //bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + Request.QueryString["EmpidfromBusOwner"] + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'");

            getPageLoadData();

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

    public void getPageLoadData()
    {
        DataTable dt = new DataTable();
        StringBuilder projid = new StringBuilder();
        string strempid = Request.QueryString["EmpidfromBusOwner"].ToString();
        string bid = Request.QueryString["BidfromBusOwner"].ToString();
        if (txtmonth.Text != "")
        {
            dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + strempid + " and effectivedate <= GETDATE())", "");

            DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + strempid + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
            if (dtsaveddata.Rows.Count > 0)
            {
                for (int i = 0; i < dtsaveddata.Rows.Count; i++)
                {

                    projid.Append(dtsaveddata.Rows[i]["ProjectId"].ToString());
                    projid.Append(",");
                }
                string proid = projid.ToString();
                string pronocoma = proid.Remove(proid.Length - 1, 1);
                int Bid = int.Parse(bid);
                DataTable dtchild = new DataTable();
                int y = DateTime.Parse(txtmonth.Text).Year;
                int m = DateTime.Parse(txtmonth.Text).Month;
                var d = DateTime.Parse(txtmonth.Text.Trim());
                int empid = int.Parse(strempid);
                CultureInfo cul = CultureInfo.CurrentCulture;
                int DateNo = d.Date.Day;

                dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp_ALL(y, m, DateNo, Bid, empid, pronocoma);
                ViewState["CurrentTable"] = dtchild;
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();
            }
            else
            {
                SetInitialRow();
            }
        }
    }
    
    public void GetAllDetails(string empid, string Bid)
    {
        Comm.FillDropDownList_NoPleaseSelect(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        ddlemp.SelectedValue = empid;

        DataTable dt = new DataTable();
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " )", "");
        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
        }

        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
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
    
    public void GetGridFill(int ProjectId)
    {
        int Bid = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());

        DataTable dtchild = new DataTable();
        int y = DateTime.Parse(txtmonth.Text).Year;
        int m = DateTime.Parse(txtmonth.Text).Month;
        var d = DateTime.Parse(txtmonth.Text.Trim());
        int empid = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
        CultureInfo cul = CultureInfo.CurrentCulture;
        int DateNo = d.Date.Day;
        dtchild = TSC.GetTimeSheetEntryChildGv_Project_Emp(y, m, DateNo, Bid, empid, ProjectId);
        ViewState["CurrentTable"] = dtchild;
        GVChildlist.DataSource = dtchild;
        GVChildlist.DataBind();
    }

    protected void lnkdelete_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField brkid = (HiddenField)GvPopUpEmpList.Rows[gvr.RowIndex].FindControl("brkid");
        Comm.ExecuteQuery("delete from BreakTime where BreakID=" + brkid.Value + "");
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time deleted', 'success');", true);
    }

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {

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

        Abe.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
        Abe.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
        Abe.Month = DateTime.Parse(txtmonth.Text).Month;
        Abe.Year = DateTime.Parse(txtmonth.Text).Year;
        string a = ViewState["datebreak"].ToString();
        string[] weekno = ViewState["weekno"].ToString().Split(',');

        Abe.WeekNo = int.Parse(weekno[0]);
        Abe.TimeSheetDate = DateTime.Parse(a);
        Abe.ProjectId = int.Parse(ViewState["ProjectName"].ToString());
        Abe.Day = DateTime.Parse(a).DayOfWeek.ToString();
        Abe.BreakStart = txtBreakStartTime.Text;
        Abe.BreakEnd = txtBreakEndTime.Text;
        TimeSpan TotalBreakHr = DateTime.Parse(txtBreakEndTime.Text) - DateTime.Parse(txtBreakStartTime.Text);
        Abe.BreakTotal = TotalBreakHr.ToString();
        Abc.InsertAddBreak_Project(Abe);
        DataTable dt;
        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(a).ToString("yyyy-MM-dd") + "' and ProjectId=" + ViewState["ProjectName"].ToString() + "", "BreakID");
        ViewState["dt"] = dt;
        GvPopUpEmpList.DataSource = ViewState["dt"];
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();

        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

        txtBreakStartTime.Text = "";
        txtBreakEndTime.Text = "";
    }

    protected void lnkAddEmp_Click(object sender, EventArgs e)
    {

    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        DateTime d = DateTime.Parse(txtmonth.Text);
        DateTime p = d.AddDays(-1);
        txtmonth.Text = p.ToString("yyyy-MM-dd");
        getPageLoadData();
        GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtmonth.Text));
    }

    protected void btnnext_Click(object sender, EventArgs e)
    {
        DateTime d = DateTime.Parse(txtmonth.Text);
        DateTime n = d.AddDays(1);
        txtmonth.Text = n.ToString("yyyy-MM-dd");
        getPageLoadData();
        GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtmonth.Text));
    }

    protected void txtmonth_TextChanged(object sender, EventArgs e)
    {
        //To reflect the rate as per the date choosen from calendar control
        GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtmonth.Text));

        getPageLoadData();
    }

    string role_check_for_add_and_edit_time_sheet_message_throw;
    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //For business owner
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

                    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                        return;
                    }
                    if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                        return;
                    }
                    //if (chkworked.Checked == false)
                    if (ddlWorked.SelectedValue == "No")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        return;
                    }
                    //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                    lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                    ViewState["datebreak"] = lblDate.Text;
                    ViewState["weekno"] = hfdweekno.Value;
                    ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                    string regMINst = lblDate.Text + " 00:00";
                    string regMAXst = lblDate.Text + " 23:59";
                    txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                    string regMinEnd = lblDate.Text + " 00:00";
                    //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                    string regMAXend = lblDate.Text + " 23:59";

                    txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                    // hfddatebreak.Value = lblDate.Text;
                    // hfdweeknopop.Value = hfdweekno.Value;
                    //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                    ViewState["dt"] = dt;
                    GvPopUpEmpList.DataSource = ViewState["dt"];
                    GvPopUpEmpList.DataBind();
                    up2.Update();
                    //GetGridFill();

                    ModalPopupEmpList.Show();

                    return;
                }

                if (e.CommandName == "Save")
                {
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
                        return;
                    }

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
                    DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                        return;
                    }

                    //if (chkworked.Checked == false)
                    if (ddlWorked.SelectedValue == "No")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        return;
                    }
                    string checkstartdate = txtRegularStartTime.Text;
                    if (checkstartdate == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                    bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                    DateTime d = DateTime.Parse(txtmonth.Text);
                    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


                    if (ToltalRegulahr < TotalBreakHr)
                    {
                        GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                        return;
                    }
                    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                    Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                    Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                    Tsee.WeekNo = int.Parse(weekno[0]);
                    Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                    Tsee.Day = lblWeek.Text;
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

                    //if (chkworked.Checked == true)
                    if (ddlWorked.SelectedValue == "Yes")
                    {
                        Tsee.Worked = "Yes";
                    }
                    else
                    {
                        Tsee.Worked = "No";
                    }
                    Tsee.BreakTotal = TotalBreakHr.ToString();
                    Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                    TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                    GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                    DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                    ddlProjectName_.Enabled = false;

                    return;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please check project details, it may be conflict with others', 'error');", true);
                return;
            }
        }

        //This will check the login user has been assigned in which business in what roles with the employee id of same login user
        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' ", "UserTypeId");

        //To check the old user who are registered before the role development
        DataTable Dt_Check_If_User_Role_Permission = new DataTable();
        if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' ", "");
        }

        if (Dt_Check_If_User_Role_Permission.Rows.Count == 0)
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

                    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                        return;
                    }
                    if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                        return;
                    }
                    //if (chkworked.Checked == false)
                    if (ddlWorked.SelectedValue == "Yes")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                        return;
                    }
                    //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                    lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy"); //lblDate.Text;
                    ViewState["datebreak"] = lblDate.Text;
                    ViewState["weekno"] = hfdweekno.Value;
                    ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                    string regMINst = lblDate.Text + " 00:00";
                    string regMAXst = lblDate.Text + " 23:59";
                    txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                    string regMinEnd = lblDate.Text + " 00:00";
                    //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                    string regMAXend = lblDate.Text + " 23:59";

                    txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                    txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                    // hfddatebreak.Value = lblDate.Text;
                    // hfdweeknopop.Value = hfdweekno.Value;
                    //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                    ViewState["dt"] = dt;
                    GvPopUpEmpList.DataSource = ViewState["dt"];
                    GvPopUpEmpList.DataBind();
                    up2.Update();
                    //GetGridFill();

                    ModalPopupEmpList.Show();

                    return;
                }

                if (e.CommandName == "Save")
                {
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
                        return;
                    }

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
                    DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                    //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                    DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                    if (ddlProjectName.SelectedValue == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                        return;
                    }

                    //if (chkworked.Checked == false)
                    if (ddlWorked.SelectedValue == "No")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                        return;
                    }
                    string checkstartdate = txtRegularStartTime.Text;
                    if (checkstartdate == "")
                    {
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                    bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                    DateTime d = DateTime.Parse(txtmonth.Text);
                    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                    if (ToltalRegulahr < TotalBreakHr)
                    {
                        GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                        return;
                    }
                    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                    Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                    Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                    Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                    Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                    Tsee.WeekNo = int.Parse(weekno[0]);
                    Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                    Tsee.Day = lblWeek.Text;
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
                    //if (chkworked.Checked == true)
                    if (ddlWorked.SelectedValue == "Yes")
                    {
                        Tsee.Worked = "Yes";
                    }
                    else
                    {
                        Tsee.Worked = "No";
                    }
                    Tsee.BreakTotal = TotalBreakHr.ToString();
                    Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                    TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                    GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                    DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                    ddlProjectName_.Enabled = false;

                    return;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please check project details, it may be conflict with others', 'error');", true);
                return;
            }
        }

        //For manager
        DataTable Dt_Check_Manager_Role_Permission = new DataTable();
        if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='2'", "");
        }

        if (Dt_Check_Manager_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "7" || role_check_for_add_and_edit_time_sheet == "8")
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

                            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                                return;
                            }
                            if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                                return;
                            }
                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                            ViewState["datebreak"] = lblDate.Text;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Text + " 00:00";
                            string regMAXst = lblDate.Text + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Text + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Text + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();

                            return;
                        }

                        if (e.CommandName == "Save")
                        {
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
                                return;
                            }

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
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }

                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                                return;
                            }
                            string checkstartdate = txtRegularStartTime.Text;
                            if (checkstartdate == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                            Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                            Tsee.Day = lblWeek.Text;
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
                            //if (chkworked.Checked == true)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                Tsee.Worked = "Yes";
                            }
                            else
                            {
                                Tsee.Worked = "No";
                            }
                            Tsee.BreakTotal = TotalBreakHr.ToString();
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "7" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "8")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For owner
        DataTable Dt_Check_Owner_Role_Permission = new DataTable();
        if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='5'", "");
        }

        if (Dt_Check_Owner_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
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

                            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                                return;
                            }
                            if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                                return;
                            }
                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                            ViewState["datebreak"] = lblDate.Text;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Text + " 00:00";
                            string regMAXst = lblDate.Text + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Text + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Text + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();

                            return;
                        }

                        if (e.CommandName == "Save")
                        {
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
                                return;
                            }

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
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }

                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                                return;
                            }
                            string checkstartdate = txtRegularStartTime.Text;
                            if (checkstartdate == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                            Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                            Tsee.Day = lblWeek.Text;
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
                            //if (chkworked.Checked == true)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                Tsee.Worked = "Yes";
                            }
                            else
                            {
                                Tsee.Worked = "No";
                            }
                            Tsee.BreakTotal = TotalBreakHr.ToString();
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "18" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "19")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For accountant
        DataTable Dt_Check_Accountant_Role_Permission = new DataTable();
        if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            Dt_Check_Accountant_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='12'", "");
        }

        if (Dt_Check_Accountant_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Accountant_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "25" || role_check_for_add_and_edit_time_sheet == "26")
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

                            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                                return;
                            }
                            if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                                return;
                            }
                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                            ViewState["datebreak"] = lblDate.Text;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Text + " 00:00";
                            string regMAXst = lblDate.Text + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Text + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Text + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();

                            return;
                        }

                        if (e.CommandName == "Save")
                        {
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
                                return;
                            }

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
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }

                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                                return;
                            }
                            string checkstartdate = txtRegularStartTime.Text;
                            if (checkstartdate == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                            Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                            Tsee.Day = lblWeek.Text;
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
                            //if (chkworked.Checked == true)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                Tsee.Worked = "Yes";
                            }
                            else
                            {
                                Tsee.Worked = "No";
                            }
                            Tsee.BreakTotal = TotalBreakHr.ToString();
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "25" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "26")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For employee
        DataTable Dt_Check_Employee_Role_Permission = new DataTable();
        if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        {
            Dt_Check_Employee_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='4'", "");
        }

        if (Dt_Check_Employee_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Employee_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2")
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

                            Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                            HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                            //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                            TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                            ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                                return;
                            }
                            if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                                return;
                            }
                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                return;
                            }
                            //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                            lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                            ViewState["datebreak"] = lblDate.Text;
                            ViewState["weekno"] = hfdweekno.Value;
                            ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                            string regMINst = lblDate.Text + " 00:00";
                            string regMAXst = lblDate.Text + " 23:59";
                            txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                            string regMinEnd = lblDate.Text + " 00:00";
                            //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                            string regMAXend = lblDate.Text + " 23:59";

                            txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                            txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                            // hfddatebreak.Value = lblDate.Text;
                            // hfdweeknopop.Value = hfdweekno.Value;
                            //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                            dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                            ViewState["dt"] = dt;
                            GvPopUpEmpList.DataSource = ViewState["dt"];
                            GvPopUpEmpList.DataBind();
                            up2.Update();
                            //GetGridFill();

                            ModalPopupEmpList.Show();

                            return;
                        }

                        if (e.CommandName == "Save")
                        {
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
                                return;
                            }

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
                            DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                            TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                            TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                            DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                            if (ddlProjectName.SelectedValue == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                return;
                            }

                            //if (chkworked.Checked == false)
                            if (ddlWorked.SelectedValue == "No")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                                return;
                            }
                            string checkstartdate = txtRegularStartTime.Text;
                            if (checkstartdate == "")
                            {
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                            //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                            //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                            DateTime d = DateTime.Parse(txtmonth.Text);
                            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                            if (ToltalRegulahr < TotalBreakHr)
                            {
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                return;
                            }
                            TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                            Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                            Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                            Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                            Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                            Tsee.WeekNo = int.Parse(weekno[0]);
                            Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                            Tsee.Day = lblWeek.Text;
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
                            //if (chkworked.Checked == true)
                            if (ddlWorked.SelectedValue == "Yes")
                            {
                                Tsee.Worked = "Yes";
                            }
                            else
                            {
                                Tsee.Worked = "No";
                            }
                            Tsee.BreakTotal = TotalBreakHr.ToString();
                            Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                            TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                            DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                            ddlProjectName_.Enabled = false;

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
            }
        }


        //This will check the login user has been assigned in which business in what roles with the employee id of same login user
        DataTable Dt_Check_User_Role_In_All_Businesses = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' ", "UserTypeId");
        if (Dt_Check_User_Role_In_All_Businesses.Rows.Count > 0)
        {
            //For the custom user type
            DataTable Dt_Check_Custome_USerType_Role_Permissions = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Businesses.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId not in ('4','1','2','5','12')", "");
            if (Dt_Check_Custome_USerType_Role_Permissions.Rows.Count > 0)
            {
                string string_Check_Role_Permission = Dt_Check_Custome_USerType_Role_Permissions.Rows[0]["Role_Permission_Id"].ToString();
                string[] splittedArray = string_Check_Role_Permission.Split(',');
                foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
                {
                    if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2" ||
                        role_check_for_add_and_edit_time_sheet == "6" || role_check_for_add_and_edit_time_sheet == "8" ||
                        role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
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

                                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                                HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
                                //CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
                                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                                DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                                if (ddlProjectName.SelectedValue == "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
                                    return;
                                }
                                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                                    return;
                                }
                                //if (chkworked.Checked == false)
                                if (ddlWorked.SelectedValue == "No")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
                                    return;
                                }
                                //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
                                lblbusname.Text = "Add Break on Date: " + DateTime.Parse(lblDate.Text).ToString("dd-MMM-yyyy");//lblDate.Text;
                                ViewState["datebreak"] = lblDate.Text;
                                ViewState["weekno"] = hfdweekno.Value;
                                ViewState["ProjectName"] = ddlProjectName.SelectedValue;

                                string regMINst = lblDate.Text + " 00:00";
                                string regMAXst = lblDate.Text + " 23:59";
                                txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
                                txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

                                string regMinEnd = lblDate.Text + " 00:00";
                                //string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";
                                string regMAXend = lblDate.Text + " 23:59";

                                txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
                                txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
                                // hfddatebreak.Value = lblDate.Text;
                                // hfdweeknopop.Value = hfdweekno.Value;
                                //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
                                ViewState["dt"] = dt;
                                GvPopUpEmpList.DataSource = ViewState["dt"];
                                GvPopUpEmpList.DataBind();
                                up2.Update();
                                //GetGridFill();

                                ModalPopupEmpList.Show();

                                return;
                            }

                            if (e.CommandName == "Save")
                            {
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
                                    return;
                                }

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
                                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
                                DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

                                if (ddlProjectName.SelectedValue == "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
                                    return;
                                }

                                //if (chkworked.Checked == false)
                                if (ddlWorked.SelectedValue == "No")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                                    return;
                                }
                                string checkstartdate = txtRegularStartTime.Text;
                                if (checkstartdate == "")
                                {
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
                                bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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

                                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
                                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
                                DateTime d = DateTime.Parse(txtmonth.Text);
                                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

                                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

                                if (ToltalRegulahr < TotalBreakHr)
                                {
                                    GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
                                    return;
                                }
                                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

                                Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
                                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
                                Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
                                Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
                                Tsee.WeekNo = int.Parse(weekno[0]);
                                Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
                                Tsee.Day = lblWeek.Text;
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
                                //if (chkworked.Checked == true)
                                if (ddlWorked.SelectedValue == "Yes")
                                {
                                    Tsee.Worked = "Yes";
                                }
                                else
                                {
                                    Tsee.Worked = "No";
                                }
                                Tsee.BreakTotal = TotalBreakHr.ToString();
                                Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
                                TSC.Sp_InsertTimeSheetEntryProject(Tsee);

                                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
                                GetGridFill(int.Parse(ddlProjectName.SelectedValue));
                                DropDownList ddlProjectName_ = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));
                                ddlProjectName_.Enabled = false;

                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else
                        role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                }
                if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                    return;
                }
            }
        }









        //Old code commented because of role logic
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

        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //        CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

        //        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //        if (ddlProjectName.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Select Project', 'error');", true);
        //            return;
        //        }
        //        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //            return;
        //        }
        //        if (chkworked.Checked == false)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //            return;
        //        }
        //        //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //        lblbusname.Text = "Add Break on Date: " + lblDate.Text;
        //        ViewState["datebreak"] = lblDate.Text;
        //        ViewState["weekno"] = hfdweekno.Value;
        //        ViewState["ProjectName"] = ddlProjectName.SelectedValue;



        //        string regMINst = lblDate.Text + " 00:00";
        //        string regMAXst = lblDate.Text + " 23:59";
        //        txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //        string regMinEnd = lblDate.Text + " 00:00";
        //        string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

        //        txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //        txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //        // hfddatebreak.Value = lblDate.Text;
        //        // hfdweeknopop.Value = hfdweekno.Value;
        //        //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + " ", "BreakID");
        //        ViewState["dt"] = dt;
        //        GvPopUpEmpList.DataSource = ViewState["dt"];
        //        GvPopUpEmpList.DataBind();
        //        up2.Update();
        //        //GetGridFill();

        //        ModalPopupEmpList.Show();
        //    }

        //    if (e.CommandName == "Save")
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
        //        CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));
        //        DropDownList ddlProjectName = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlProjectName"));

        //        if (ddlProjectName.SelectedValue == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select Project', 'error');", true);
        //            return;
        //        }

        //        if (chkworked.Checked == false)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
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
        //        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(txtmonth.Text).Date.ToString("yyyy-MM-dd") + "'");
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
        //        DataTable prvdate = Comm.GetAllDataFromTable("TimeSheetEntry", "Total", "EmpId='" + Request.QueryString["EmpidfromBusOwner"].ToString() + "' and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and total <> ''", "");

        //        TimeSpan tt = new TimeSpan();
        //        if (prvdate.Rows.Count > 0)
        //        {
        //            for (int i = 0; i < prvdate.Rows.Count; i++)
        //            {
        //                TimeSpan prvtotTime = TimeSpan.Parse(prvdate.Rows[i][0].ToString());
        //                tt += prvtotTime;
        //            }

        //            tt += ToltalRegulahr;
        //            if (tt.TotalHours >= 24)
        //            {
        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Cannot work more than 24 Hr in one Day', 'error');", true);
        //                return;
        //            }
        //        }


        //        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //        DateTime d = DateTime.Parse(txtmonth.Text);
        //        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString()) + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + d.Month + " and Year=" + d.Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "", "");

        //        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //        if (ToltalRegulahr < TotalBreakHr)
        //        {
        //            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //            return;
        //        }
        //        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //        Tsee.EmpId = int.Parse(Request.QueryString["EmpidfromBusOwner"].ToString());
        //        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
        //        Tsee.Month = DateTime.Parse(txtmonth.Text).Month;
        //        Tsee.Year = DateTime.Parse(txtmonth.Text).Year;
        //        Tsee.WeekNo = int.Parse(weekno[0]);
        //        Tsee.TimeSheetDate = DateTime.Parse(txtmonth.Text);
        //        Tsee.Day = lblWeek.Text;
        //        Tsee.Regular = ToltalRegulahr.ToString();
        //        Tsee.Over1_5x = Over15x[0];
        //        Tsee.Over2x = Over2x[0];
        //        Tsee.Holiday = holiday[0];
        //        Tsee.Total = TotalHr.ToString();
        //        Tsee.ReqularStart = DateTime.Parse(RegularStartTime[0].ToString());
        //        Tsee.Regularend = DateTime.Parse(RegularEndTime[0].ToString());
        //        Tsee.BreakStart = BreakStartTime[0].ToString();
        //        Tsee.BreakEnd = BreakEndTime[0].ToString();
        //        Tsee.Status = "1";
        //        if (chkworked.Checked == true)
        //        {
        //            Tsee.Worked = "Yes";
        //        }
        //        else
        //        {
        //            Tsee.Worked = "No";
        //        }
        //        Tsee.BreakTotal = TotalBreakHr.ToString();
        //        Tsee.ProjectId = int.Parse(ddlProjectName.SelectedValue);
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

    protected void GVChildlist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList ddlProjectName = (DropDownList)e.Row.FindControl("ddlProjectName");
            HiddenField hfdWorked = (HiddenField)e.Row.FindControl("hfdWorked");
            HiddenField hfdprojId = (HiddenField)e.Row.FindControl("hfdprojId");
            //CheckBox chkworked = (CheckBox)e.Row.FindControl("chkworked");
            DropDownList ddlWorked = e.Row.FindControl("ddlWorked") as DropDownList;
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
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            //Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT", "PROJECTID", "dbo.FN_DESC('Project',PROJECTID)Pname", "EMPLOYEEID=" + Request.QueryString["EmpidfromBusOwner"].ToString() + "", "PROJECTID");

            //Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= GETDATE() OR B.EndDate IS NULL OR B.EndDate = '')", "A.PROJECTID", "dbo.FN_DESC('Project',A.PROJECTID)Pname", "EMPLOYEEID=" + Request.QueryString["EmpidfromBusOwner"].ToString() + "", "A.PROJECTID");

            Comm.FillDropDownList(ddlProjectName, "ASSIGNPROJECT as A INNER JOIN AddProject AS B ON A.ProjectId = B.ProjectID  and(B.EndDate >= '" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' OR B.EndDate IS NULL OR B.EndDate = '')", "A.PROJECTID", "dbo.FN_DESC('Project',A.PROJECTID)Pname", "EMPLOYEEID=" + Request.QueryString["EmpidfromBusOwner"].ToString() + "", "A.PROJECTID");

            if (ddlProjectName.Items.Count == 1)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please extend the Project End Date as the project has been expired', 'error');", true);
            }

            if (hfdprojId.Value == "0")
            {
                ddlProjectName.Items.Add(new ListItem() { Value = "0", Text = "Regular Hours", Selected = true });
                ddlProjectName.Enabled = false;
            }

            if (hfdWorked.Value.Trim() == "Yes")
            {
                ddlWorked.SelectedValue = "Yes";
                //chkworked.Checked = true;
            }
            ddlProjectName.SelectedValue = hfdprojId.Value;

            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";

                ddlProjectName.Enabled = false;
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                //txtRegularStartTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;

                //ddlProjectName.Enabled = false;
                //ddlWorked.Enabled = false;
                ddlProjectName.Enabled = false;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                lnkSave.Enabled = true;
                lnkAddBreak.Enabled = true;
                //txtRegularStartTime.ReadOnly = false;
                ddlProjectName.Enabled = false;

            }
            else if (hfdStatus.Value == "4")
            {
                lblStatus.Text = "Absent";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                //txtRegularStartTime.ReadOnly = true;
                //txtBreakEndTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                //txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                //e.Row.BackColor = System.Drawing.Color.LightGray;
                //ddlProjectName.Enabled = false;
                //ddlWorked.Enabled = false;
                //lnkGvApp.Enabled = false;
                //lnkGvReject.Enabled = false;
                //chkworked.Enabled = false;
                //chkApproved.Enabled = false;
                //chkworked.Enabled = false;
                ddlProjectName.Enabled = false;
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

            DataTable DataTable_chk_enddate = Comm.GetAllDataFromTable("RateDetails with (Nolock)", "EndDate,Is_Having_EndDate", "RateId=(select MAX(RateId)from RateDetails " +
                "where EmployeeId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + ")", "");
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
                    Label lblDate = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblDate");
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
                        drCurrentRow["SelDate"] = lblDate.Text;
                        drCurrentRow["weekno"] = hfdweekno.Value;
                        drCurrentRow["weekdayno"] = hfdweekdayNo.Value;
                        drCurrentRow["ProjectId"] = isRegular ? "0" : ddlProjectName.SelectedValue;
                        drCurrentRow["ProjectName"] = isRegular ? "0" : ddlProjectName.SelectedValue;

                        dtCurrentTable.Rows[i - 1]["ProjectName"] = ddlProjectName.SelectedValue;
                        dtCurrentTable.Rows[i - 1]["ProjectId"] = ddlProjectName.SelectedValue;
                        dtCurrentTable.Rows[i - 1]["SelDate"] = lblDate.Text;
                        dtCurrentTable.Rows[i - 1]["weekno"] = hfdweekno.Value;
                        dtCurrentTable.Rows[i - 1]["weekdayno"] = hfdweekdayNo.Value;
                        dtCurrentTable.Rows[i - 1]["Worked"] = hfdWorked.Value;

                        if (txtRegularStartTime.Text != "" || txtRegularEndTime.Text != "")
                        {
                            dtCurrentTable.Rows[i - 1]["ReqularStart"] = txtRegularStartTime.Text;
                            dtCurrentTable.Rows[i - 1]["Regularend"] = txtRegularEndTime.Text;
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
                    Label lblDate = (Label)GVChildlist.Rows[rowIndex].Cells[1].FindControl("lblDate");
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

                    ddlProjectName.SelectedValue = hfdprojId.Value == "0" ? "0" : dt.Rows[i]["ProjectName"].ToString();
                    hfdprojId.Value = dt.Rows[i]["ProjectId"].ToString();
                    lblDate.Text = dt.Rows[i]["SelDate"].ToString();
                    hfdweekno.Value = dt.Rows[i]["weekno"].ToString();
                    hfdweekdayNo.Value = dt.Rows[i]["weekdayno"].ToString();
                    hfdWorked.Value = dt.Rows[i]["Worked"].ToString();
                    if (dt.Rows[i]["ReqularStart"].ToString() != "")
                    {
                        txtRegularStartTime.Text = DateTime.Parse(dt.Rows[i]["ReqularStart"].ToString()).ToString("yyyy-MM-ddTHH:mm");
                    }

                    if (dt.Rows[i]["Regularend"].ToString() != "")
                    {
                        txtRegularEndTime.Text = DateTime.Parse(dt.Rows[i]["Regularend"].ToString()).ToString("yyyy-MM-ddTHH:mm");
                    }

                    lblTotal.Text = dt.Rows[i]["Regular"].ToString();
                    lblBreakTotal.Text = dt.Rows[i]["BreakTotal"].ToString();
                    lblnetTotal.Text = dt.Rows[i]["Total"].ToString();

                    rowIndex++;
                }
            }
        }
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
        bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "EmpId=" + Request.QueryString["EmpidfromBusOwner"] + " and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "' and ProjectId=" + ddlProjectName.SelectedValue + "");
        if (check == true)
        {
            GetGridFill(int.Parse(ddlProjectName.SelectedValue));
        }

        if (ddlProjectName.SelectedValue != "")
        {
            DataTable DataTable_chk_project_enddate = Comm.GetAllDataFromTable("AddProject with (Nolock)", "EndDate", "ProjectId=" + ddlProjectName.SelectedValue + " " +
            "and BusinessID=" + Request.QueryString["BidfromBusOwner"] + "", "");
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

        DataTable dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
            GVChildlist.Enabled = true;
        }
        if (dt.Rows.Count == 0)
        {
            GVChildlist.Enabled = false;
            lblCurrentRate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
        }

        //EmployeeType Change date check and disable previous timesheetentry validation
        DataTable DataTable_chk_current_emptype = Comm.GetAllDataFromTable("AddEmployee with (Nolock)", "EMPLOYEETYPE", "EmployeeID=" + empid + "", "");
        DataTable DataTable_chk_emptypechangedate = Comm.GetAllDataFromTable("EmpType_Change_History with (Nolock)", "top 1 ChangeDate, EMPLOYEETYPE",
            "EmployeeID=" + empid + "", "1 desc");
        if (DataTable_chk_emptypechangedate.Rows.Count > 0 && DataTable_chk_current_emptype.Rows[0]["EMPLOYEETYPE"].ToString() == "P"
            && (DataTable_chk_current_emptype.Rows[0]["EMPLOYEETYPE"].ToString() == DataTable_chk_emptypechangedate.Rows[0]["EMPLOYEETYPE"].ToString()))
        {
            if (DateTime.Parse(txtmonth.Text) < DateTime.Parse(DataTable_chk_emptypechangedate.Rows[0]["ChangeDate"].ToString()))
            {
                GVChildlist.Enabled = false;
            }
        }
    }

    protected void lnkAddRegularHours_Click(object sender, EventArgs e)
    {
        var regularHoursExists = false;
        DataTable dt = new DataTable();
        StringBuilder projid = new StringBuilder();
        DataTable dtsaveddata = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + ddlemp.SelectedValue + " and ProjectId is not null and TimeSheetDate='" + DateTime.Parse(txtmonth.Text).ToString("yyyy-MM-dd") + "'", "ProjectId");
        if (dtsaveddata.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Info!', 'First Enter Project Time Sheet Then Regular Time Sheet', 'info');", true);
            return;
        }
        for (var i = 0; i < GVChildlist.Rows.Count; i++)
        {
            var projectId = (HiddenField)GVChildlist.Rows[i].FindControl("hfdprojId");
            regularHoursExists = projectId.Value == "0" ? true : false;
            if (projectId.Value == "0") break;
        }

        if (regularHoursExists)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No more regular hours to be added', 'error');", true);
            return;
        }
        else
            AddNewRowToGrid(true);
    }
    protected void LinkButton_CopyNextWeek_Click(object sender, EventArgs e)
    {
        int check_timesheet_data = 0;
        int check_Current_Week_timesheet_data = 0;

        foreach (GridViewRow row in GVChildlist.Rows)
        {
            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

            TextBox txtOver15x = (TextBox)row.FindControl("txtOver15x");
            TextBox txtOver2x = (TextBox)row.FindControl("txtOver2x");
            TextBox txtholiday = (TextBox)row.FindControl("txtholiday");
            //Label lblTotal = (Label)row.FindControl("lblTotal");
            Label lblDate = (Label)row.FindControl("lblDate");
            Label lblDay = (Label)row.FindControl("lblDay");
            HiddenField hfdweekno = (HiddenField)row.FindControl("hfdweekno");
            TextBox txtRegularStartTime = (TextBox)row.FindControl("txtRegularStartTime");
            TextBox txtRegularEndTime = (TextBox)row.FindControl("txtRegularEndTime");
            //TextBox txtBreakStartTime = (TextBox)row.FindControl("txtBreakStartTime");
            //TextBox txtBreakEndTime = (TextBox)row.FindControl("txtBreakEndTime");

            DropDownList ddlProjectName = (DropDownList)row.FindControl("ddlProjectName");

            HiddenField hfdprojId = (HiddenField)row.FindControl("hfdprojId");

            bool check = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "'");
            if (check)
            {
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
                //Over15x[0] = txtOver15x.Text.Substring(1);
                //Over2x[0] = txtOver2x.Text.Substring(1);
                //holiday[0] = txtholiday.Text.Substring(1);

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

            DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

            TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

            //TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

            Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
            Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
            Tsee.Month = DateTime.Parse(txtmonth.Text).AddDays(1).Month;
            Tsee.Year = DateTime.Parse(txtmonth.Text).AddDays(1).Year;

            //Tsee.WeekNo = int.Parse(weekno[0]);
            DateTime d = DateTime.Parse(lblDate.Text).AddDays(1);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            Tsee.WeekNo = weekNum;

            Tsee.TimeSheetDate = DateTime.Parse(lblDate.Text);
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
                bool check_if_timesheet_exits = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");

                bool check_if_timesheet_exits_nextWeek = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");

                if (!check_if_timesheet_exits)
                    check_Current_Week_timesheet_data = 10;

                if (check_if_timesheet_exits && !check_if_timesheet_exits_nextWeek)
                {
                    var check_project_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ", "");

                    if (check_project_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_project_absent_time.Rows[0]["Regularend"].ToString() == "")
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                            " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");
                    }
                    else
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                            " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");
                    }

                    check_timesheet_data = 1;

                    check_Current_Week_timesheet_data = 0;

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                }

                if (check_if_timesheet_exits && check_if_timesheet_exits_nextWeek)
                {
                    int a = Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                        "and (ProjectId='0' or ProjectId IS NULL) and BussID='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and Status in (1,3,4)");

                    if (a > 0)
                    {
                        var check_project_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ", "");

                        if (check_project_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_project_absent_time.Rows[0]["Regularend"].ToString() == "")
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                                " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");
                        }
                        else
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                                " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId IS NULL ");
                        }

                        check_timesheet_data = 1;

                        check_Current_Week_timesheet_data = 0;

                        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                    }
                }
            }
            else
            {
                bool check_if_timesheet_exits = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");

                bool check_if_timesheet_exits_nextWeek = Comm.IfExistsInTable("TimeSheetEntry with (Nolock)", "TimeSheetEntryId", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                    "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");

                if (!check_if_timesheet_exits)
                    check_Current_Week_timesheet_data = 10;

                if (check_if_timesheet_exits && !check_if_timesheet_exits_nextWeek)
                {
                    var check_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ", "");

                    if (check_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_absent_time.Rows[0]["Regularend"].ToString() == "")
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                            " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                    }
                    else
                    {
                        Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                        " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                        " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                        " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                        " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                        " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                    }

                    check_timesheet_data = 1;

                    check_Current_Week_timesheet_data = 0;

                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is copied successfully.', 'success');", true);
                }

                if (check_if_timesheet_exits && check_if_timesheet_exits_nextWeek)
                {
                    int a = Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.AddDays(1).ToString("yyyy-MM-dd") + "' " +
                        "and (ProjectId='" + hfdprojId.Value + "') and BussID='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and Status in (1,3,4)");

                    if (a > 0)
                    {
                        var check_absent_time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ReqularStart,Regularend", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
                        "and empid='" + ddlemp.SelectedValue + "' and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ", "");

                        if (check_absent_time.Rows[0]["ReqularStart"].ToString() == "" && check_absent_time.Rows[0]["Regularend"].ToString() == "")
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                                " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                                " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', NULL, " +
                                " NULL, Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                                " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                                " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
                        }
                        else
                        {
                            Comm.ExecuteQuery("INSERT INTO TimeSheetEntry (EmpId, BussID, Month, Year, WeekNo, TimeSheetDate, Day, ReqularStart, Regularend, Regular, BreakStart, BreakEnd, " +
                            " BreakTotal, Over1_5x, Over2x, Holiday, Total, Status, Worked, ProjectId) " +
                            " SELECT EmpId, BussID, '" + Tsee.Month + "', '" + Tsee.Year + "', '" + Tsee.WeekNo + "', '" + DateTime.Parse(lblDate.Text).AddDays(1) + "', '" + Tsee.Day + "', '" + Tsee.ReqularStart + "', " +
                            " '" + Tsee.Regularend + "', Regular, BreakStart, BreakEnd, BreakTotal, Over1_5x, Over2x, Holiday, Total, '1', Worked, ProjectId FROM TimeSheetEntry " +
                            " where TimeSheetDate = '" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' and empid='" + ddlemp.SelectedValue + "' " +
                            " and bussId='" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' and ProjectId = '" + hfdprojId.Value + "' ");
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
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Hours before clicking Copy Timesheet', 'warning');", true);
            return;
        }

        if (check_timesheet_data == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Timesheet already exists, so timesheet is not Copied', 'warning');", true);
        }
    }
}