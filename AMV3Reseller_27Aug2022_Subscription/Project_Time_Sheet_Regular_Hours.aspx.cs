using System;
using System.Data;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Project_Time_Sheet_Regular_Hours : System.Web.UI.Page
{
    TimeSheetController TSC = new TimeSheetController();
    TimeSheetEntryEntity Tsee = new TimeSheetEntryEntity();
    AddBreakController Abc = new AddBreakController();
    AddBreakEntity Abe = new AddBreakEntity();
    CommanClass Comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["EmpidfromBusOwner"] != null && Request.QueryString["BidfromBusOwner"] != null)
            {
                GetAllDetails(Request.QueryString["EmpidfromBusOwner"].ToString(), Request.QueryString["BidfromBusOwner"].ToString());
                txtmonth.Text = System.DateTime.Now.Date.ToString("yyyy-MM-dd");
                GetGridFill();
            }
        }
    }
    public void GetAllDetails(string empid, string Bid)
    {
        DataTable dt = new DataTable();
        //Comm.FillDropDownList(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        Comm.FillDropDownList_NoPleaseSelect(ddlemp, "AddEmployee", "EmployeeID", "dbo.FN_DESC('AddEmployee'," + empid + ")as Name", "EmployeeID=" + empid + "", "");
        ddlemp.SelectedValue = empid;
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " and effectivedate <= GETDATE())", "");
        //dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + empid + " )", "");
        lblCurrentRate.Text = "$" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();

        lblbid.Text = Comm.GetNameFromId("AddBusiness", int.Parse(Bid));
    }
    public void GetAllDetails(string Bid)
    {
        if (Session["UserTypeid"].ToString() == "1")
        {
            if (Request.QueryString["MangerId"] != null)
            {
                DataTable dtMangerEmpList = new DataTable();
                dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()),"R");

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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
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
        Abe.BussID = int.Parse(Request.QueryString["BidfromBusOwner"].ToString());
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
        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(a).ToString("yyyy-MM-dd") + "'", "BreakID");
        ViewState["dt"] = dt;
        GvPopUpEmpList.DataSource = ViewState["dt"];
        GvPopUpEmpList.DataBind();
        ModalPopupEmpList.Show();
        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'Break time added', 'success');", true);

        txtBreakStartTime.Text = "";
        txtBreakEndTime.Text = "";
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
        dt = Comm.GetAllDataFromTable("RateDetails", "Rate,RateType", "RateId=(select MAX(RateId)from RateDetails where EmployeeId=" + ddlemp.SelectedValue + " and effectivedate <= GETDATE())", "");
        lblCurrentRate.Text = "$" + dt.Rows[0]["Rate"].ToString() + "/" + dt.Rows[0]["RateType"].ToString();
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
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to save time sheet', 'error');", true);
                    return;
                }

                string checkstartdate;
                checkstartdate = txtRegularStartTime.Text;
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
            if (e.CommandName == "Submit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                if (CheckSaved == true)
                {
                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                    return;
                }
                if (ddlWorked.SelectedValue == "No")
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
                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                ViewState["dt"] = dt;
                GvPopUpEmpList.DataSource = ViewState["dt"];
                GvPopUpEmpList.DataBind();
                up2.Update();
                //GetGridFill();

                ModalPopupEmpList.Show();
                return;
            }

            return;
        }

        //This will check the login user has been assigned in which business in what roles with the employee id of same login user
        DataTable Dt_Check_User_Role_In_All_Business = Comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' ", "UserTypeId");

        //To check the old user who are registered before the role development
        DataTable Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' ", "");
        if(Dt_Check_If_User_Role_Permission.Rows.Count==0)
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
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

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
            if (e.CommandName == "Submit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GVChildlist.Rows[index];

                Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                if (ddlWorked.SelectedValue == "No")
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                    GetGridFill();
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                    return;
                }
                bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                if (CheckSaved == true)
                {
                    Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                    return;
                }
                if (ddlWorked.SelectedValue == "No")
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
                dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                ViewState["dt"] = dt;
                GvPopUpEmpList.DataSource = ViewState["dt"];
                GvPopUpEmpList.DataBind();
                up2.Update();
                //GetGridFill();

                ModalPopupEmpList.Show();
            }
            return;
        }

        //For manager
        DataTable Dt_Check_Manager_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='2'", "");
        if (Dt_Check_Manager_Role_Permission.Rows.Count>0)
        {
            string string_Check_Role_Permission = Dt_Check_Manager_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "7" || role_check_for_add_and_edit_time_sheet == "8")
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

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
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
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
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "7" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "8")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For owner
        DataTable Dt_Check_Owner_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='5'", "");
        if (Dt_Check_Owner_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Owner_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

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
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
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
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "18" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "19")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }

        //For employee
        DataTable Dt_Check_Employee_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId='4'", "");
        if (Dt_Check_Employee_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Employee_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2")
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

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
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
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
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }


        //For the custom user type
        //For employee
        DataTable Dt_Check_Custome_USerType_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and BusinessId = '" + Request.QueryString["BidfromBusOwner"].ToString() + "' and UserTypeId not in ('4','1','2','5','12')", "");
        if (Dt_Check_Custome_USerType_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_Custome_USerType_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_add_and_edit_time_sheet in splittedArray)
            {
                #region For_role_check_for_update_address definition
                if (role_check_for_add_and_edit_time_sheet == "1" || role_check_for_add_and_edit_time_sheet == "2" ||
                    role_check_for_add_and_edit_time_sheet == "6" || role_check_for_add_and_edit_time_sheet == "8" ||
                    role_check_for_add_and_edit_time_sheet == "18" || role_check_for_add_and_edit_time_sheet == "19")
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtBreakStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakStartTime"));
                        TextBox txtBreakEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtBreakEndTime"));

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
                    if (e.CommandName == "Submit")
                    {
                        int index = Convert.ToInt32(e.CommandArgument);
                        GridViewRow row = GVChildlist.Rows[index];

                        Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));
                        if (ddlWorked.SelectedValue == "No")
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to submit time sheet', 'error');", true);
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
                            GetGridFill();
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'End time should not be blank', 'error');", true);
                            return;
                        }
                        bool CheckSaved = Comm.IfExistsInTable("TimeSheetEntry", "*", "TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString());
                        if (CheckSaved == true)
                        {
                            Comm.ExecuteQuery("Update TimeSheetEntry set Status=1 where TimeSheetDate='" + DateTime.Parse(lblDate.Text) + "' and EmpId=" + ddlemp.SelectedValue + " and BussID=" + Request.QueryString["BidfromBusOwner"].ToString() + "");
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
                        DropDownList ddlWorked = ((DropDownList)GVChildlist.Rows[index].FindControl("ddlWorked"));
                        TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
                        TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

                        ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
                        ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
                        if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
                        {
                            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
                            return;
                        }
                        if (ddlWorked.SelectedValue == "No")
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
                        dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
                        ViewState["dt"] = dt;
                        GvPopUpEmpList.DataSource = ViewState["dt"];
                        GvPopUpEmpList.DataBind();
                        up2.Update();
                        //GetGridFill();

                        ModalPopupEmpList.Show();
                        return;
                    }
                }
                else
                    role_check_for_add_and_edit_time_sheet_message_throw = role_check_for_add_and_edit_time_sheet;
                #endregion
                //else
                //{
                //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Role Is Not Assigned To Update Address', 'warning');", true);
                //    return;
                //}
            }
            if (role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "1" || role_check_for_add_and_edit_time_sheet_message_throw.ToString() != "2")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Add And Edit Time Sheet', 'error');", true);
                return;
            }
            return;
        }


        //commented for new role base logic
        //string[] Regular = { "" }, Over15x = { "" }, Over2x = { "" }, holiday = { "" }, RegularStartTime = { "" }, RegularEndTime = { "" }, BreakStartTime = { "" }, BreakEndTime = { "" };
        //GridView GVChildlist = (GridView)sender;

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

        //if (e.CommandName == "AddBreak")
        //{
        //    DataTable dt;
        //    GvPopUpEmpList.DataSource = null;
        //    GvPopUpEmpList.DataBind();
        //    txtBreakEndTime.Text = string.Empty;
        //    txtBreakStartTime.Text = string.Empty;
        //    int index = Convert.ToInt32(e.CommandArgument);
        //    GridViewRow row = GVChildlist.Rows[index];

        //    Label lblDate = ((Label)GVChildlist.Rows[index].FindControl("lblDate"));
        //    HiddenField hfdweekno = ((HiddenField)GVChildlist.Rows[index].FindControl("hfdweekno"));
        //    CheckBox chkworked = ((CheckBox)GVChildlist.Rows[index].FindControl("chkworked"));
        //    TextBox txtRegularStartTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularStartTime"));
        //    TextBox txtRegularEndTime = ((TextBox)GVChildlist.Rows[index].FindControl("txtRegularEndTime"));

        //    ViewState["txtRegularStartTime"] = txtRegularStartTime.Text;
        //    ViewState["txtRegularEndTime"] = txtRegularEndTime.Text;
        //    if (ViewState["txtRegularStartTime"] == null || ViewState["txtRegularEndTime"] == null)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please Enter Regular Hr First', 'error');", true);
        //        return;
        //    }
        //    if (chkworked.Checked == false)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select worked status as Yes to add break', 'error');", true);
        //        return;
        //    }
        //    //UpdatePanel up1 = ((UpdatePanel)GvMainList.Rows[index].FindControl("up1"));
        //    lblbusName.Text = "Add Break on Date: " + lblDate.Text;
        //    ViewState["datebreak"] = lblDate.Text;
        //    ViewState["weekno"] = hfdweekno.Value;

        //    string regMINst = lblDate.Text + " 00:00";
        //    string regMAXst = lblDate.Text + " 23:59";
        //    txtBreakStartTime.Attributes.Add("max", DateTime.Parse(regMAXst).ToString("yyyy-MM-ddTHH:mm"));
        //    txtBreakStartTime.Attributes.Add("min", DateTime.Parse(regMINst).ToString("yyyy-MM-ddTHH:mm"));

        //    string regMinEnd = lblDate.Text + " 00:00";
        //    string regMAXend = DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-dd") + " 23:59";

        //    txtBreakEndTime.Attributes.Add("max", DateTime.Parse(regMAXend).ToString("yyyy-MM-ddTHH:mm"));
        //    txtBreakEndTime.Attributes.Add("min", DateTime.Parse(regMinEnd).ToString("yyyy-MM-ddTHH:mm"));
        //    // hfddatebreak.Value = lblDate.Text;
        //    // hfdweeknopop.Value = hfdweekno.Value;
        //    //Comm.FillGridView(GvPopUpEmpList, "BreakTime", "BreakStart,BreakEnd,BreakTotal", "BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //    dt = Comm.GetAllDataFromTable("BreakTime", "BreakStart,BreakEnd,BreakTotal,BreakID", "EmpId=" + ddlemp.SelectedValue + " and BussID=" + int.Parse(Request.QueryString["BidfromBusOwner"].ToString()) + " and Month=" + DateTime.Parse(txtmonth.Text).Month + " and Year=" + DateTime.Parse(txtmonth.Text).Year + " and TimeSheetDate='" + DateTime.Parse(lblDate.Text).ToString("yyyy-MM-dd") + "'", "BreakID");
        //    ViewState["dt"] = dt;
        //    GvPopUpEmpList.DataSource = ViewState["dt"];
        //    GvPopUpEmpList.DataBind();
        //    up2.Update();
        //    //GetGridFill();

        //    ModalPopupEmpList.Show();
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
            DropDownList ddlWorked = e.Row.FindControl("ddlWorked") as DropDownList;
            CheckBox chkApproved = e.Row.FindControl("chkApproved") as CheckBox;
            TextBox txtRegularStartTime = e.Row.FindControl("txtRegularStartTime") as TextBox;
            TextBox txtRegularEndTime = e.Row.FindControl("txtRegularEndTime") as TextBox;
            LinkButton lnkAddBreak = e.Row.FindControl("lnkAddBreak") as LinkButton;
            LinkButton lnkSave = e.Row.FindControl("lnkSave") as LinkButton;


            LinkButton lnkGvApp = e.Row.FindControl("lnkGvApp") as LinkButton;
            LinkButton lnkGvReject = e.Row.FindControl("lnkGvReject") as LinkButton;
            LinkButton lnlGvAbsent = e.Row.FindControl("lnlGvAbsent") as LinkButton;

            txtRegularStartTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).ToString("yyyy-MM-ddTHH:mm"));
            txtRegularEndTime.Attributes.Add("max", DateTime.Parse(lblDate.Text).AddDays(1).ToString("yyyy-MM-ddTHH:mm"));


            if (hfdStatus.Value == "1")
            {
                lblStatus.Text = "Pending";
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
            }
            else if (hfdStatus.Value == "2")
            {
                lblStatus.Text = "Approved";
                lnkSave.Enabled = false;
                lnkAddBreak.Enabled = false;
                txtRegularStartTime.ReadOnly = true;
                txtRegularEndTime.ReadOnly = true;
                txtBreakEndTime.ReadOnly = true;
            }
            else if (hfdStatus.Value == "3")
            {
                lblStatus.Text = "Rejected";
                lnkSave.Enabled = true;
                lnkAddBreak.Enabled = true;
                txtRegularStartTime.ReadOnly = false;
                txtBreakEndTime.ReadOnly = false;
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
                ddlWorked.Enabled = false;
                chkApproved.Enabled = false;
                //chkworked.Enabled = false;
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
}