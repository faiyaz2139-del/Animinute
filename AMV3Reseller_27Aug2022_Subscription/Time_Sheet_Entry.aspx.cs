using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Time_Sheet_Entry : System.Web.UI.Page
{

    private bool boolCopyNextWeek { get { return Convert.ToBoolean(ViewState["CopyNextWeek"] ?? false); } set { ViewState["CopyNextWeek"] = value; } }
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    protected void Page_Error(object sender, EventArgs e)
    {
        // Retain the normal error page, but record the exception needed to diagnose deployment/data failures.
        log4net.LogManager.GetLogger("Log").Error("Regular employee Time Card failed", Server.GetLastError());
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        lblLocked.Visible = false;
        if (!IsPostBack)
            boolCopyNextWeek = false;
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            if (Request.QueryString["EmpidfromBusOwner"] != null && Request.QueryString["BidfromBusOwner"] != null)
            {
                //ddlemp.SelectedValue = Request.QueryString["EmpidfromBusOwner"].ToString();
                txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
                GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString());

                GetGridFill();
            }
        }
    }
   /* protected void txtmonth_TextChanged(object sender, EventArgs e)
    {
        GetAllDetails(ddlemp.SelectedValue, Request.QueryString["BidfromBusOwner"].ToString());
    }*/
    public void GetAllDetails(string empid, string Bid)
    {
        DataTable dt = new DataTable();
        //Comm.FillDropDownList(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        Comm.FillDropDownList_NoPleaseSelect(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        ddlemp.SelectedValue = empid;


        DateTime TimeCardDate;
        if (!DateTime.TryParseExact(txtmonth.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
            DateTimeStyles.None, out TimeCardDate)) TimeCardDate = DateTime.Today;
        CultureInfo currentCulture = CultureInfo.CurrentCulture;
        System.Globalization.Calendar calendar = currentCulture.Calendar;
        CalendarWeekRule weekRule = currentCulture.DateTimeFormat.CalendarWeekRule;
        DayOfWeek firstDayOfWeek = currentCulture.DateTimeFormat.FirstDayOfWeek;
        int weekNum = calendar.GetWeekOfYear(TimeCardDate, weekRule, firstDayOfWeek);
        // Get the week number using the culture-specific rules
        DataTable dtTimeSheetStatus = Comm.GetAllDataFromTable("TimeCardStatus", "TimeSheetStatus", "EmployeeID="+empid +  " and BusinessID="+Bid +" and WeekNum="+weekNum+ " and Year="+TimeCardDate.Year, "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
        dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
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
                lblLocked.Visible = false;
            }
            else
            { GVChildlist.Enabled = false;
                lblLocked.Visible = true;
            }
        }
        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
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
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "");
            }

        }
        else if (Session["UserTypeid"].ToString() == "2")
        {
            if (Request.QueryString["MangerId"] != null)
            {

                // Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BidfromBusOwner"].ToString() + "'", "EmpId");

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
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
                }
                else
                {
                    Comm.FillDropDownList(ddlemp, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " AND AE.UserType<>5 and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");
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
                //    Comm.FillDropDownList(ddlemp, "AssgineManager", "EmpId", "EmpName", "ManagerID=" + Request.QueryString["MangerId"].ToString() + " and BusinessID='" + Request.QueryString["BidfromBusOwner"].ToString() + "'", "EmpId");
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
                    dtMangerEmpList = Comm.GetAllDataFromTable("AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID,DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + Request.QueryString["BidfromBusOwner"].ToString() + " AND AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "", "AE.EmployeeID");

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
                Comm.FillDropDownList(ddlemp, "AddEmployee ad join AssginBussiness assb on ad.EmployeeID=assb.EmployeeID ", "ad.EmployeeID", "dbo.FN_DESC('AddEmployee',ad.EmployeeID)as Name", "assb.BusinessID=" + Bid + "and ad.UserType<>1 ", "");//and ad.EmployeeID<>" + Session["EmployeeId"].ToString() + "
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


            int Bid = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
            DataTable dtchild = new DataTable();
            int y = DateTime.Parse(txtmonth.Text).Year;
            int m = DateTime.Parse(txtmonth.Text).Month;
            var d = DateTime.Parse(txtmonth.Text.Trim());
            int empid = int.Parse(ddlemp.SelectedValue);
            CultureInfo cul = CultureInfo.CurrentCulture;
            int weekNum = cul.Calendar.GetWeekOfYear(d, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
            dtchild = TSC.GetTimeSheetEntryChildGv(y, m, weekNum, Bid, empid);
            //Chart2.Visible = true;
            if (dtchild.Rows.Count > 0)
            {
                //GVChildlist.Enabled = true;
                //lblCurrentRate.Visible = true;
                //lblCurrentErrorSms.Visible = false;
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();

                //Firoz 06/23



                BindHoursChart(dtchild);
            }
            else
            {
                GVChildlist.DataSource = dtchild;
                GVChildlist.DataBind();
                Chart2.Series[0].Points.Clear();
                Chart2.Visible = false;
            }
            //firoz 06
            //if (y < 2021)
            //{
            //    GVChildlist.Enabled = false;
            //    lblCurrentRate.Visible = false;
            //    lblCurrentErrorSms.Visible = true;
            //    lblCurrentErrorSms.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
            //}
            //end  


        }
    }
    private void BindHoursChart(DataTable rows)
    {
        // Durations are hours/minutes, not DateTime values. Ignore invalid legacy totals in the chart only.
        decimal[] minutes = new decimal[3];
        foreach (DataRow row in rows.Rows)
        {
            int status;
            if (!Int32.TryParse(Convert.ToString(row[16]), out status) || status < 1 || status > 3) continue;
            string[] parts = Convert.ToString(row[15]).Trim().Split(':');
            int hours, mins;
            if (parts.Length != 2 || !Int32.TryParse(parts[0], out hours) ||
                !Int32.TryParse(parts[1], out mins) || hours < 0 || mins < 0 || mins > 59) continue;
            minutes[status - 1] += (decimal)hours * 60 + mins;
        }
        Chart2.Series[0].Points.Clear();
        string[] labels = { "Pending", "Approved", "Rejected" };
        for (int i = 0; i < minutes.Length; i++)
            if (minutes[i] > 0) Chart2.Series[0].Points.AddXY(labels[i], (double)(minutes[i] / 60));
        Chart2.Visible = Chart2.Series[0].Points.Count > 0;
        Chart2.Series[0].ChartType = System.Web.UI.DataVisualization.Charting.SeriesChartType.Pie;
    }

    private int RepairBusinessId { get { return int.Parse(Request.QueryString["BidfromBusOwner"]); } }
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

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {

    }

    protected void lnkAddEmp_Click(object sender, EventArgs e)
    {

    }

    protected void ddlemp_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlemp.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select employee', 'error');", true);
            return;
        }
        DataTable dt = new DataTable();
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
        GetGridFill();
    }

    protected void btnprv_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtmonth.Text);
        DateTime nxtdate = CurrentDate.AddMonths(-1);
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        GetAllDetails(ddlemp.SelectedValue, Request.QueryString["BidfromBusOwner"].ToString());
        // txtmonth_TextChanged(sender, e);
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

        //DataTable dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        DataTable dt = Comm.GetAllDataFromTable("RateDetails", "cast((convert(decimal(18, 2), Rate)) as varchar(20)) as Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= '" + Calender_Selected_Date + "')", "");
        if (dt.Rows.Count > 0)
        {
            lblCurrentRate.Text = "Rate : $" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
            //GVChildlist.Enabled = true;
        }
        if (dt.Rows.Count == 0)
        {
            //GVChildlist.Enabled = false;
            lblCurrentRate.Text = "The user is hired after this date. Please choose hire date if the back date timesheet is entered.";
        }
    }
    //end
    protected void txtmonth_TextChanged(object sender, EventArgs e)
    {
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

        //firoz 15-22
        GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString(), DateTime.Parse(txtmonth.Text));
        //end
        GetGridFill();
    }

    protected void btnnext_Click(object sender, EventArgs e)
    {
        DateTime CurrentDate = DateTime.Parse(txtmonth.Text);
        DateTime nxtdate = CurrentDate.AddMonths(1);
        txtmonth.Text = nxtdate.ToString("yyyy-MM-dd");
        GetAllDetails(ddlemp.SelectedValue, Request.QueryString["BidfromBusOwner"].ToString());
        //  txtmonth_TextChanged(sender, e);
    }

    protected void lnkApproved_Click(object sender, EventArgs e)
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

                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=2 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
            }
        }
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Date is not selected', 'error');", true);
        }
    }

    protected void lnkReject_Click(object sender, EventArgs e)
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

                a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
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

    string role_check_for_add_and_edit_time_sheet_message_throw;
    protected void GVChildlist_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        #region COMMENTED BECAUSE OF SAVE ROCOMMAND ERROR
        //string user_type_id = Session["UserTypeid"].ToString();
        //if (user_type_id == "1")
        //{
        //    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //    GridView GVChildlist = (GridView)sender;

        //    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
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
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //        if (ddlWorked.SelectedValue == "No")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //            return;
        //        }

        //        string checkstartdate;
        //        checkstartdate = txtRegularStartTime.Text;
        //        if (checkstartdate == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time is empty', 'error');", true);
        //            return;
        //        }
        //        if (txtRegularStartTime.Text == ",")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            return;
        //        }

        //        if (txtRegularEndTime.Text == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time is empty', 'error');", true);
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
        //        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //        if (ToltalRegulahr.TotalHours >= 24)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //            return;
        //        }

        //        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //        if (ToltalRegulahr < TotalBreakHr)
        //        {
        //            GetGridFill();
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //            return;
        //        }
        //        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //        Tsee.Status = "1";
        //        if (ddlWorked.SelectedValue == "Yes")
        //        {
        //            Tsee.Worked = "Yes";
        //        }
        //        else
        //        {
        //            Tsee.Worked = "No";
        //        }
        //        Tsee.BreakTotal = TotalBreakHr.ToString();
        //        TSC.Sp_InsertTimeSheetEntry(Tsee);

        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //        GetGridFill();


        //    }
        //    if (e.CommandName == "Submit")
        //    {
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];

        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        if (ddlWorked.SelectedValue == "No")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //            return;
        //        }
        //        string checkstartdate = txtRegularStartTime.Text;
        //        if (checkstartdate == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            return;
        //        }
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
        //        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //        if (CheckSaved == true)
        //        {
        //            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //            return;
        //        }
        //        if (ddlWorked.SelectedValue == "No")
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
        //        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        ViewState["dt"] = dt;
        //        GvPopUpEmpList.DataSource = ViewState["dt"];
        //        GvPopUpEmpList.DataBind();
        //        up2.Update();
        //        //GetGridFill();

        //        ModalPopupEmpList.Show();
        //        return;
        //    }

        //    return;
        //}

        ////This will check the login user has been assigned in which business in what roles with the employee id of same login user
        //DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' ", "UserTypeId");

        ////To check the old user who are registered before the role development
        //DataTable Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' ", "");
        //if(Dt_Check_If_User_Role_Permission.Rows.Count==0)
        //{
        //    string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //    GridView GVChildlist = (GridView)sender;

        //    // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
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
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //        if (ddlWorked.SelectedValue == "No")
        //        {
        //            //It is commented because tester said employee user can also make his timesheet absent
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
        //            //hfdRIndex.Value = row.RowIndex.ToString();
        //            ViewState["RIndex"] = row.RowIndex.ToString();
        //            //ModalPopupConfirmSingal.Show();
        //            //absent(row.RowIndex);
        //            return;
        //        }

        //        string checkstartdate = txtRegularStartTime.Text;
        //        if (checkstartdate == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
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
        //        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //        if (ToltalRegulahr.TotalHours >= 24)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //            return;
        //        }

        //        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //        if (ToltalRegulahr < TotalBreakHr)
        //        {
        //            GetGridFill();
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //            return;
        //        }
        //        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //        Tsee.Status = "1";
        //        if (ddlWorked.SelectedValue == "Yes")
        //        {
        //            Tsee.Worked = "Yes";
        //        }
        //        else
        //        {
        //            Tsee.Worked = "No";
        //        }
        //        Tsee.BreakTotal = TotalBreakHr.ToString();
        //        TSC.Sp_InsertTimeSheetEntry(Tsee);

        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //        GetGridFill();


        //    }
        //    if (e.CommandName == "Submit")
        //    {
        //        int index = Convert.ToInt32(e.CommandArgument);
        //        GridViewRow row = GVChildlist.Rows[index];

        //        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //        if (ddlWorked.SelectedValue == "No")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //            return;
        //        }
        //        string checkstartdate = txtRegularStartTime.Text;
        //        if (checkstartdate == "")
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //            return;
        //        }
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
        //        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //        if (CheckSaved == true)
        //        {
        //            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //        {
        //            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //            return;
        //        }
        //        if (ddlWorked.SelectedValue == "No")
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
        //        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //        ViewState["dt"] = dt;
        //        GvPopUpEmpList.DataSource = ViewState["dt"];
        //        GvPopUpEmpList.DataBind();
        //        up2.Update();
        //        //GetGridFill();

        //        ModalPopupEmpList.Show();
        //    }
        //    return;
        //}

        ////For manager
        //DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='2'", "");
        //if (Dt_Check_Manager_Role_Permission.Rows.Count>0)
        //{
        //    string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
        //    {
        //        #region For_role_check_for_update_address definition
        //        if (role_check_for_add_and_edit_time_sheet == "7" || role_check_for_add_and_edit_time_sheet == "8")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Save")
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    return;
        //                }

        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == ",")
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
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }

        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //                Tsee.Status = "1";
        //                if (ddlWorked.SelectedValue == "Yes")
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                else
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }
        //            if (e.CommandName == "Submit")
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
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
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //                    return;
        //                }
        //                if (ddlWorked.SelectedValue == "No")
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
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                up2.Update();
        //                //GetGridFill();

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
        //        #endregion
        //        //else
        //        //{
        //        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
        //        //    return;
        //        //}
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "7" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "8")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}

        ////For owner
        //DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='5'", "");
        //if (Dt_Check_Owner_Role_Permission.Rows.Count > 0)
        //{
        //    string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
        //    {
        //        #region For_role_check_for_update_address definition
        //        if (role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Save")
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    return;
        //                }

        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularStartTime.Text == ",")
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
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }

        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //                Tsee.Status = "1";
        //                if (ddlWorked.SelectedValue == "Yes")
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                else
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }
        //            if (e.CommandName == "Submit")
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
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
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //                    return;
        //                }
        //                if (ddlWorked.SelectedValue == "No")
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
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                up2.Update();
        //                //GetGridFill();

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
        //        #endregion
        //        //else
        //        //{
        //        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
        //        //    return;
        //        //}
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "18" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "19")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}

        ////For accountant
        //DataTable Dt_Check_AccountantRole_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='12'", "");
        //if (Dt_Check_AccountantRole_Permission.Rows.Count > 0)
        //{
        //    string string_Check_Role_Permission = Dt_Check_AccountantRole_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
        //    {
        //        #region For_role_check_for_update_address definition
        //        if (role_check_for_add_and_edit_time_sheet == "25" || role_check_for_add_and_edit_time_sheet == "26")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Save")
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //                    return;
        //                }

        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }

        //                if (txtRegularStartTime.Text == ",")
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
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }

        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //                Tsee.Status = "1";
        //                if (ddlWorked.SelectedValue == "Yes")
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                else
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }
        //            if (e.CommandName == "Submit")
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
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
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //                    return;
        //                }
        //                if (ddlWorked.SelectedValue == "No")
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
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                up2.Update();
        //                //GetGridFill();

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
        //        #endregion
        //        //else
        //        //{
        //        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
        //        //    return;
        //        //}
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "25" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "26")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}

        ////For employee
        //DataTable Dt_Check_Employee_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='4'", "");
        //if (Dt_Check_Employee_Role_Permission.Rows.Count > 0)
        //{
        //    string string_Check_Role_Permission = Dt_Check_Employee_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
        //    {
        //        #region For_role_check_for_update_address definition
        //        if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Save")
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    //It is commented because tester said employee user can also make his timesheet absent
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
        //                    //hfdRIndex.Value = row.RowIndex.ToString();
        //                    ViewState["RIndex"] = row.RowIndex.ToString();
        //                    //ModalPopupConfirmSingal.Show();
        //                    //absent(row.RowIndex);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == ",")
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
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be lower than start time', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan ToltalRegulahr = DateTime.Parse(RegularEndTime[0].ToString()) - DateTime.Parse(RegularStartTime[0].ToString());
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }

        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //                Tsee.Status = "1";
        //                if (ddlWorked.SelectedValue == "Yes")
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                else
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }
        //            if (e.CommandName == "Submit")
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
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
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //                    return;
        //                }
        //                if (ddlWorked.SelectedValue == "No")
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
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                up2.Update();
        //                //GetGridFill();

        //                txtBreakEndTime.ReadOnly = false;
        //                txtBreakEndTime.Enabled = true;

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
        //        #endregion
        //        //else
        //        //{
        //        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
        //        //    return;
        //        //}
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}

        ////For the custom user type
        //DataTable Dt_Check_Custome_USerType_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId not in ('4','1','2','5','12')", "");
        //if (Dt_Check_Custome_USerType_Role_Permission.Rows.Count > 0)
        //{
        //    string string_Check_Role_Permission = Dt_Check_Custome_USerType_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //    string[] splittedArray = string_Check_Role_Permission.Split(',');
        //    foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
        //    {
        //        #region For_role_check_for_update_address definition
        //        if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2" ||
        //            role_check_for_add_and_edit_time_sheet == "6" || role_check_for_add_and_edit_time_sheet == "8" ||
        //            role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
        //        {
        //            string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //            GridView GVChildlist = (GridView)sender;

        //            // GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //            if (e.CommandName == "Save")
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    //It is commented because tester said employee user can also make his timesheet absent
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
        //                    //hfdRIndex.Value = row.RowIndex.ToString();
        //                    ViewState["RIndex"] = row.RowIndex.ToString();
        //                    //ModalPopupConfirmSingal.Show();
        //                    //absent(row.RowIndex);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
        //                if (txtRegularStartTime.Text == ",")
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
        //                //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //                //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //                if (ToltalRegulahr.TotalHours >= 24)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //                    return;
        //                }

        //                DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //                TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //                if (ToltalRegulahr < TotalBreakHr)
        //                {
        //                    GetGridFill();
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //                    return;
        //                }
        //                TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //                Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //                Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //                Tsee.Status = "1";
        //                if (ddlWorked.SelectedValue == "Yes")
        //                {
        //                    Tsee.Worked = "Yes";
        //                }
        //                else
        //                {
        //                    Tsee.Worked = "No";
        //                }
        //                Tsee.BreakTotal = TotalBreakHr.ToString();
        //                TSC.Sp_InsertTimeSheetEntry(Tsee);

        //                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //                GetGridFill();


        //            }
        //            if (e.CommandName == "Submit")
        //            {
        //                int index = Convert.ToInt32(e.CommandArgument);
        //                GridViewRow row = GVChildlist.Rows[index];

        //                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //                if (ddlWorked.SelectedValue == "No")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //                    return;
        //                }
        //                string checkstartdate = txtRegularStartTime.Text;
        //                if (checkstartdate == "")
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //                    return;
        //                }
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
        //                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //                if (CheckSaved == true)
        //                {
        //                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
        //                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
        //                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //                {
        //                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //                    return;
        //                }
        //                if (ddlWorked.SelectedValue == "No")
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
        //                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //                ViewState["dt"] = dt;
        //                GvPopUpEmpList.DataSource = ViewState["dt"];
        //                GvPopUpEmpList.DataBind();
        //                up2.Update();
        //                //GetGridFill();

        //                ModalPopupEmpList.Show();
        //                return;
        //            }
        //        }
        //        else
        //            role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
        //        #endregion
        //        //else
        //        //{
        //        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
        //        //    return;
        //        //}
        //    }
        //    if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
        //        return;
        //    }
        //    return;
        //}
        #endregion


        //commented for new role base logic
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        GridView GVChildlist = (GridView)sender;

        //// GridViewRow chrow = (GridViewRow)GVChildlist.Parent;
        //if (e.CommandName == "Save")
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
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
        //    TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
        //        return;
        //    }

        //    if (txtRegularStartTime.Text == ",")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start time should not be blank', 'error');", true);
        //        return;
        //    }

        //    if (txtRegularEndTime.Text == "")
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
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
        //    //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //    //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        //    if (ToltalRegulahr.TotalHours >= 24)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
        //        return;
        //    }

        //    DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        //    TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        //    if (ToltalRegulahr < TotalBreakHr)
        //    {
        //        GetGridFill();
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
        //        return;
        //    }
        //    TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        //    Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        //    Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //    Tsee.Status = "1";
        //    if (chkworked.Checked == true)
        //    {
        //        Tsee.Worked = "Yes";
        //    }
        //    else
        //    {
        //        Tsee.Worked = "No";
        //    }
        //    Tsee.BreakTotal = TotalBreakHr.ToString();
        //    TSC.Sp_InsertTimeSheetEntry(Tsee);

        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //    GetGridFill();


        //}
        //if (e.CommandName == "Submit")
        //{
        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];

        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
        //        return;
        //    }
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
        //    bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
        //    if (CheckSaved == true)
        //    {
        //        Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
            if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
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
            dt = AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]), Convert.ToInt32(Session["UserTypeid"]), RepairBusinessId, int.Parse(ddlemp.SelectedValue), DateTime.Parse(lblDate.Text));
            ViewState["dt"] = dt;
            GvPopUpEmpList.DataSource = ViewState["dt"];
            GvPopUpEmpList.DataBind();
            up2.Update();
            //GetGridFill();

            ModalPopupEmpList.Show();
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
            DropDownList ddlWorked = e.Row.FindControl("ddlWorked") as DropDownList;
            CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            //LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;


            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-ddTHH:mm"));

            LinkButton dropdownMenu1 = e.Row.FindControl("dropdownMenu1") as LinkButton;
            LinkButton LinkButton_Save = e.Row.FindControl("LinkButton_Save") as LinkButton;
            LinkButton LinkButton_Modify = e.Row.FindControl("LinkButton_Modify") as LinkButton;
            LinkButton LinkButton_Delete = e.Row.FindControl("LinkButton_Delete") as LinkButton;

            //lblDate.Text = DateTime.Parse(lblDate.Text).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
            lblDate.Text = DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
                dropdownMenu1.Text = "Pending";
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                ////lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                txtBreakEndTime.ReadOnly = true;
                ddlWorked.Enabled = false;
                dropdownMenu1.Text = hfdWorked.Value.Trim() == "No" ? "Approved (Absent)" : "Approved";

                dropdownMenu1.Enabled = false;
                LinkButton_Save.Enabled = false;
                LinkButton_Modify.Enabled = false;
                LinkButton_Delete.Enabled = false;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                ////lnkSave.Enabled = true;
                lnkAddBreak.Enabled = true;
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
                dropdownMenu1.Text = "Rejected";
            }
            else if (hfdStatus.Value == "4")
            {
                lblStatus.Text = "Absent";
                ////lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                //txtRegularStartTime.ReadOnly = true;
                //txtBreakEndTime.ReadOnly = true;
                //txtRegularEndTime.ReadOnly = true;
                //txtRegularStartTime.BackColor = System.Drawing.Color.LightGray;
                //txtRegularEndTime.BackColor = System.Drawing.Color.LightGray;
                //e.Row.BackColor = System.Drawing.Color.LightGray;
                lnkGvApp.Enabled = false;
                lnkGvReject.Enabled = false;
                //ddlWorked.Enabled = false;
                //chkApproved.Enabled = false;
                //chkworked.Enabled = false;
                dropdownMenu1.Text = "Absent";
            }
            if (hfdWorked.Value.Trim() == "Yes")
            {
                ddlWorked.SelectedValue = "Yes";
            }


            //if(txtRegularEndTime.Text!=string.Empty && txtRegularStartTime.Text!=string.Empty)
            //{
            //    lnkSave.Enabled = false;
            //    lnkAddBreak.Enabled = false;
            //    txtRegularStartTime.ReadOnly = true;
            //    txtRegularStartTime.ReadOnly = true;
            //    //chkworked.Enabled = false;
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
                "where EmployeeId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + " and effectivedate <= '" + DateTime.Parse(lblDate.Text) + "')", "");
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
                "where EmployeeId=" + Request.QueryString["EmpidfromBusOwner"].ToString() + ")", "");
            // Missing pay settings must not crash the page or enable dates before hiring.
            e.Row.Enabled = DataTable_chk_hiredate.Rows.Count > 0;
            if (DataTable_chk_enddate.Rows.Count > 0)
            {
                bool hasEnd;
                DateTime endDate;
                if (Boolean.TryParse(Convert.ToString(DataTable_chk_enddate.Rows[0]["Is_Having_EndDate"]), out hasEnd) && hasEnd)
                {
                    if (!DateTime.TryParse(Convert.ToString(DataTable_chk_enddate.Rows[0]["EndDate"]), out endDate)
                        || DateTime.Parse(lblDate.Text).Date > endDate.Date) e.Row.Enabled = false;
                }
            }
        }
    }

    protected void GVChildlist_PreRender(object sender, EventArgs e)
    {
        if (GVChildlist.Rows.Count > 0)
        {
            GVChildlist.UseAccessibleHeader = true;
            GVChildlist.HeaderRow.TableSection = TableRowSection.TableHeader;
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
        GetAllDetails(ddlemp.SelectedValue, Request.QueryString["BidfromBusOwner"].ToString());
        // txtmonth_TextChanged(sender, e);
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
        GetAllDetails(ddlemp.SelectedValue, Request.QueryString["BidfromBusOwner"].ToString());
        //txtmonth_TextChanged(sender, e);
    }

    protected void lnkGvApp_Click(object sender, EventArgs e)
    {
        LinkButton lnkdelete = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField HiddenField = (HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");
        DateTime d = DateTime.Parse(lblDate.Text);

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

        int a = Comm.ExecuteQuery("Update TimeSheetEntry set Status=3 where WeekNo=" + HiddenField.Value + " and EmpId='" + ddlemp.SelectedValue + "' and TimeSheetDate='" + d + "'");
        if (a > 0)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got Rejected', 'success');", true);
        }
    }
    protected void LinkButton_CopyNextWeek_Click(object sender, EventArgs e)
    { CopyRepairWeek(true); }

    protected void absent(int RowIndex)
    {
        //LinkButton lnkdelete = (LinkButton)sender;
        //GridViewRow gvr = (GridViewRow)lnkdelete.NamingContainer;
        HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RowIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RowIndex].FindControl("lblDate");
        Label lblDay = (Label)GVChildlist.Rows[RowIndex].FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        }
        else
        {
            //Tsee.Status = string.Empty;
            //TSC.Sp_InsertTimeSheetEntry(Tsee);
            //Comm.ExecuteQuery("delete from TimeSheetEntry where EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4");
        }

        GetGridFill();
    }

    protected void LinkButton_CopyCurrentWeek_Click(object sender, EventArgs e)
    { CopyRepairWeek(false); }

    protected void lnkbtnYesSingal_Click(object sender, EventArgs e)
    {
        int RIndex = int.Parse(ViewState["RIndex"].ToString());
        //int RIndex = int.Parse(hfdRIndex.Value);
        HiddenField hfdweekno = (HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno");
        Label lblDate = (Label)GVChildlist.Rows[RIndex].FindControl("lblDate");
        Label lblDay = (Label)GVChildlist.Rows[RIndex].FindControl("lblDay");
        string[] weekno = hfdweekno.Value.Split(',');
        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        //Tsee.Status = "4";
        //TSC.Sp_InsertTimeSheetEntry(Tsee);
        ////ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet got approved', 'success');", true);
        ////ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'The time sheet is marked as Absent.', 'success');", true);
        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        //GetGridFill();
        DataTable dtcheck = Comm.GetAllDataFromTable("TimeSheetEntry", "*", "EmpId=" + Tsee.EmpId + " and BussID=" + Tsee.BussID + " and Month=" + Tsee.Month + " and Year=" + Tsee.Year + " and WeekNo=" + Tsee.WeekNo + " and TimeSheetDate='" + Tsee.TimeSheetDate.ToShortDateString() + "' and Status=4", "");
        if (dtcheck.Rows.Count == 0)
        {
            Tsee.Status = "4";
            TSC.Sp_InsertTimeSheetEntry(Tsee);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'The time sheet is marked as Absent.', 'success');", true);
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'The time sheet is marked as Absent.', 'success');", true);
        }

        GetGridFill();
        up1.Update();
    }

    protected void lnkbtnNoSingal_Click(object sender, EventArgs e)
    {
        ModalPopupConfirmSingal.Hide();
    }

    protected void LinkButton_Save_Click(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        LinkButton LinkButton_Save = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Save.NamingContainer;
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblTotal"));
        Label lblDate = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate"));
        Label lblDay = ((Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[gvr.RowIndex].FindControl("hfdweekno"));
        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularStartTime"));
        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtRegularEndTime"));
        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[gvr.RowIndex].FindControl("ddlWorked"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[gvr.RowIndex].FindControl("txtBreakEndTime"));
        if (boolCopyNextWeek == false)
        {
            if (ddlWorked.SelectedValue == "No")
            {
                //It is commented because tester said employee user can also make his timesheet absent
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present to save time sheet', 'error');", true);
                //hfdRIndex.Value = row.RowIndex.ToString();
                ViewState["RIndex"] = gvr.RowIndex.ToString();
                //ModalPopupConfirmSingal.Show();
                //absent(row.RowIndex);
                return;
            }
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
        //TimeSpan TotalBreakHr = DateTime.Parse(BreakEndTime[0].ToString()) - DateTime.Parse(BreakStartTime[0].ToString());
        //float totalovertime = float.Parse(Over15x[0]) + float.Parse(Over2x[0]) + float.Parse(holiday[0]);
        if (ToltalRegulahr.TotalHours >= 24)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Start Time and End Time Must be within 24Hrs', 'error');", true);
            return;
        }

        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());


        if (ToltalRegulahr < TotalBreakHr)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
            return;
        }
        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;





        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        if (ddlWorked.SelectedValue == "Yes")
        {
            Tsee.Worked = "Yes";
        }
        else
        {
            Tsee.Worked = "No";
        }
        Tsee.BreakTotal = TotalBreakHr.ToString();
        TSC.Sp_InsertTimeSheetEntry(Tsee);

        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Time sheet added successfully', 'success');", true);
        GetGridFill();
    }

    protected void LinkButton_Modify_Click(object sender, EventArgs e)
    {
        TextBox1.Text = "";
        TextBox2.Text = "";

        LinkButton LinkButton_Modify = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)LinkButton_Modify.NamingContainer;
        Label lblDate = (Label)GVChildlist.Rows[gvr.RowIndex].FindControl("lblDate");

        DataTable Dt_check_Time = Comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text).Date.ToString("yyyy-MM-dd") + "' " +
            "and EmpId='" + ddlemp.SelectedValue + "' and BussID = '" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + "' ", "");
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
        up2.Update();
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
        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
    }

    protected void Button_Save_Action_popup_Click(object sender, EventArgs e)
    {
        string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };

        int RIndex = int.Parse(hfdRIndex.Value);
        TextBox txtRegular = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegular"));
        TextBox txtOver15x = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtOver15x"));
        TextBox txtOver2x = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtOver2x"));
        TextBox txtholiday = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtholiday"));
        Label lblTotal = ((Label)GVChildlist.Rows[RIndex].FindControl("lblTotal"));
        Label lblDate = ((Label)GVChildlist.Rows[RIndex].FindControl("lblDate"));
        Label lblDay = ((Label)GVChildlist.Rows[RIndex].FindControl("lblDay"));
        HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[RIndex].FindControl("hfdweekno"));
        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularStartTime"));
        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtRegularEndTime"));
        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[RIndex].FindControl("ddlWorked"));
        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtBreakStartTime"));
        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[RIndex].FindControl("txtBreakEndTime"));

        txtRegularStartTime.ReadOnly = false;
        txtRegularEndTime.ReadOnly = false;
        ddlWorked.Enabled = true;

        if (ddlWorked.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Present.', 'error');", true);
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
        DataTable dtBreakTime = Comm.GetAllDataFromTable("BreakTime", "isnull(convert (nvarchar,DATEADD(ms, SUM(DATEDIFF(ms, '00:00:00.000', BreakTotal)), '00:00:00.000'),108),'00:00:00') as totalbreak", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and WeekNo=" + int.Parse(weekno[0]) + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "");

        TimeSpan TotalBreakHr = TimeSpan.Parse(dtBreakTime.Rows[0]["totalbreak"].ToString());

        if (ToltalRegulahr < TotalBreakHr)
        {
            GetGridFill();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Total break time should not be more than total working hour', 'error');", true);
            return;
        }
        TimeSpan TotalHr = ToltalRegulahr - TotalBreakHr;

        Tsee.EmpId = int.Parse(ddlemp.SelectedValue);
        Tsee.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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

        if (ddlWorked.SelectedValue == "Yes")
        {
            Tsee.Worked = "Yes";
        }
        else
        {
            Tsee.Worked = "No";
        }
        Tsee.BreakTotal = TotalBreakHr.ToString();
        TSC.Sp_InsertTimeSheetEntry(Tsee);
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Time sheet is modified successfully.', 'success');", true);
        GetGridFill();
        up1.Update();

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
    protected void page_unload()
    {
        //page_loadboolCopyNextWeek = false;
    }
}