using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using log4net;

public partial class Report3 : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ReportController RptCtrl = new ReportController();
    protected void Page_Load(object sender, EventArgs e)
    {
        grdHeaderDays.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdHeaderDays.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdHeaderDays.HeaderStyle.Font.Bold = true;
        grdHalfMonth.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdHalfMonth.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdHalfMonth.HeaderStyle.Font.Bold = true;
        grdTotal.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdTotal.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdTotal.HeaderStyle.Font.Bold = true;

        grdQterly.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdQterly.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdQterly.HeaderStyle.Font.Bold = true;

        grdFourWeek.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdFourWeek.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdFourWeek.HeaderStyle.Font.Bold = true;
        grdFive.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdFive.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdFive.HeaderStyle.Font.Bold = true;
        grdsix.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdsix.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdsix.HeaderStyle.Font.Bold = true;
        grdSeven.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdSeven.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdSeven.HeaderStyle.Font.Bold = true;
        grdEight.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
        grdEight.HeaderStyle.ForeColor = System.Drawing.Color.White;
        grdEight.HeaderStyle.Font.Bold = true;

        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        AnyminuteCalendarNavigation.NormalizeManager(this);
        if (!IsPostBack)
        {
            grdHeaderDays.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdHeaderDays.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdHeaderDays.HeaderStyle.Font.Bold = true;
            grdHalfMonth.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdHalfMonth.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdHalfMonth.HeaderStyle.Font.Bold = true;
            grdTotal.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdTotal.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdTotal.HeaderStyle.Font.Bold = true;

            grdQterly.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdQterly.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdQterly.HeaderStyle.Font.Bold = true;

            grdFourWeek.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdFourWeek.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdFourWeek.HeaderStyle.Font.Bold = true;
            grdFive.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdFive.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdFive.HeaderStyle.Font.Bold = true;
            grdsix.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdsix.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdsix.HeaderStyle.Font.Bold = true;
            grdSeven.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdSeven.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdSeven.HeaderStyle.Font.Bold = true;
            grdEight.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#607DF2");
            grdEight.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grdEight.HeaderStyle.Font.Bold = true;
            //GetBusiness();
            divrange2.Attributes.Add("style", "display:none");
            GVBusiness.Visible = false;
            GVCompare1.Visible = false;
            GVCompare2.Visible = false;
            var empid = Session["EmployeeId"].ToString();
            //kumar commented as business name was showing wrong if business name is edited
            //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");
            Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");

            if (Session["UserTypeid"].ToString() != "1")
            {
                ddl_Business.Items.Clear();
                //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName",
                //    "UserTypeId not in (1,4,2,5,12) and EmployeeID in (select EmployeeID from MappingTable where USERID = '" + Session["UserId"].ToString() + "' and UserTypeId not in (1, 4, 2, 5, 12))", "BusinessID");

                Comm.FillDropDownList_NoPleaseSelect(ddl_Business, "AssginBussiness", "distinct BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", "BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPage"].ToString()) + "", "");
                ddl_Business.SelectedValue = Request.QueryString["BusinessIdFromDefaultPage"].ToString();

                GetAllDetails_Employee_List(ddl_Business.SelectedValue);
            }

            if (Request.QueryString["UserRole"] == null)
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx";
            }

            if (Request.QueryString["UserRole"] == "CustomRole")
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=CustomRole" + "&MangerId=" + Request.QueryString["MangerId"].ToString();
            }

            if (Request.QueryString["UserRole"] == "Administrator")
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Administrator";
            }

            if (Request.QueryString["UserRole"] == "Manager")
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Manager" + "&MangerId=" + Request.QueryString["MangerId"].ToString();
            }

            if (Request.QueryString["UserRole"] == "Employee")
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Employee";
            }

            if (Request.QueryString["UserRole"] == "Accountant")
            {
                HyperLink1.NavigateUrl = "~/Schedular.aspx?BusinessIdFromDefaultPage=" + Request.QueryString["BusinessIdFromDefaultPage"].ToString() + "&UserRole=Accountant";
            }
        }
    }

    public void GetTotalHeader()
    {
        grdHeaderDays.Visible = false;
        //tbldataheader.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtF = new DataTable();

        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {
            var col = dt1.ToString("dddd");
            //var col = dt1.ToString("dd");

            try
            {
                DataTable dtflt = dthr.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n"; //+ t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                dtF.Columns.Add(col);
                int datavalues = dtF.Columns.Count;

                if (datavalues == 7)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                dtF.Columns.Add(col + "\n");
                int datavalues = dtF.Columns.Count;
                if (datavalues == 7)
                {
                    break;
                }

                //throw;
            }


            dt1 = dt1.AddDays(1);
        }


        DataRow dr = dtF.NewRow();
        //dr[0] = "PLANNED";
       dtF.Rows.Add(dr);


        for (int i = 0; i < dtF.Columns.Count; i++)
        {
            if (i >= 0 && i < 7)
            {
                goto stops;
            }




        stops:
            //if (i < 7)
            //{
            //for (int j = 0; j < k; c++)
            //{
            var colto = dtF.Columns[i].ColumnName.Split('\n');
            var col = colto[1].Split('*');
            //var colto  = dtF.Columns[i].ColumnName.Split(' ');
            //dtF.Columns[i].ColumnName = col[0]+"     ";

            //if (col.Length > 2)
            //{
            //    if (col[2].ToString() == "")
            //    {
            //        //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]  +"     @Break  Hours -" + col[2] + "             @Net Hours-" + col[1];
            //        //dtF.Rows[0][i] = "Total Scheduled Hours-" + col[0] + "     @Net Hours -" + col[1] + "             @Break  Hours-" + col[2];//vishnu sir said to change sequence
            //        //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

            //        // dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "     @Break  Hours - 0.0 " + "             @Net Hours - " + col[1];
            //        //dtF.Rows[1][i] = "BREAK-" + col[2];

            //        dtF.Rows[0][i] = " " + col[0] + " 0.0 " + "   " + col[1];
            //        dtF.Rows[1][i] = " " + col[2];
            //    }
            //    else
            //    {
            //        //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]  +"     @Break  Hours -" + col[2] + "             @Net Hours-" + col[1];
            //        //dtF.Rows[0][i] = "Total Scheduled Hours-" + col[0] + "     @Net Hours -" + col[1] + "             @Break  Hours-" + col[2];//vishnu sir said to change sequence
            //        // dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

            //        //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "     @Break  Hours - " + col[2] + "             @Net Hours - " + col[1];
            //        // dtF.Rows[1][i] = "BREAK-" + col[2];
            //        dtF.Rows[0][i] = " " + col[0] + "     " + col[2] + "      " + col[1];
            //        dtF.Rows[1][i] = " " + col[2];
            //    }
            //}
            //else
            //{
            //    //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]   +"       @Net Hours -" + col[1] + " " +
            //    //"@Break  Hours - 0.0";//vishnu sir said to change sequence

            //    //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

            //    //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "       @Break  Hours - 0.0 " +
            //    //    "@Net Hours - " + col[1] + " ";

            //    dtF.Rows[0][i] = "  " + col[0] + "     @  0.0 " +
            //        " @ " + col[1] + " ";
            //}
            //dtF.Columns[i].ColumnName = colto[0];
            // }
            // }


        }


        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;

        if (ddrange.SelectedIndex == 1)
        {
            grdHeaderDays.Attributes.Add("style", "Width:100%;");
            grdHeaderDays.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            grdTotal.Attributes.Add("style", "Width:1000%;");
           // grdHeaderDays.Attributes.Add("style", "Width:84%;");
            grdHeaderDays.Attributes.Add("style", "text-align:center;");
            //tbldataheader.Attributes.Add("style", "Width: 16%;");
        }
        //tbldataheader.Visible = true;
        grdHeaderDays.Visible = true;
        grdHeaderDays.DataSource = dtF;
        grdHeaderDays.DataBind();


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
    }


    public void GetTotalHours()
    {
        grdTotal.Visible = false;
        grdTotals.Visible = false;
        //tblTotal.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtF = new DataTable();
        
        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {
            //var col1 = dt1.ToString("dddd");
            //var col = dt1.ToString("dd");
            var col = dt1.ToString(" dd-MMM-yyyy");

            try
            {
                DataTable dtflt = dthr.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n" + t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                dtF.Columns.Add(col);
               int datavalues = dtF.Columns.Count;

                if (datavalues == 7)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                dtF.Columns.Add(col + "\n0.0*0.0");
                int datavalues = dtF.Columns.Count;
                if(datavalues == 7)
                {
                    break;
                }

                //throw;
            }


            dt1 = dt1.AddDays(1);
        }

        
        DataRow dr = dtF.NewRow();
        //dr[0] = "PLANNED";
        dtF.Rows.Add(dr);


        for (int i = 0; i < dtF.Columns.Count; i++)
        {
            if (i >= 0 && i < 7)
            {
                goto stops;
            }
           
           
            

        stops:
            //if (i < 7)
            //{
                //for (int j = 0; j < k; c++)
                //{
                var colto = dtF.Columns[i].ColumnName.Split('\n');
                var col = colto[1].Split('*');
                //var colto  = dtF.Columns[i].ColumnName.Split(' ');
                //dtF.Columns[i].ColumnName = col[0]+"     ";

                if (col.Length > 2)
                {
                    if (col[2].ToString() == "")
                    {
                    //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]  +"     @Break  Hours -" + col[2] + "             @Net Hours-" + col[1];
                    //dtF.Rows[0][i] = "Total Scheduled Hours-" + col[0] + "     @Net Hours -" + col[1] + "             @Break  Hours-" + col[2];//vishnu sir said to change sequence
                    //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

                    // dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "     @Break  Hours - 0.0 " + "             @Net Hours - " + col[1];
                    //dtF.Rows[1][i] = "BREAK-" + col[2];

                    dtF.Rows[0][i] = " " + col[0];// + " 0.0 " + "   " + col[1];
                    //dtF.Rows[1][i] = " " + col[2];
                }
                    else
                    {
                    //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]  +"     @Break  Hours -" + col[2] + "             @Net Hours-" + col[1];
                    //dtF.Rows[0][i] = "Total Scheduled Hours-" + col[0] + "     @Net Hours -" + col[1] + "             @Break  Hours-" + col[2];//vishnu sir said to change sequence
                    // dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

                    //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "     @Break  Hours - " + col[2] + "             @Net Hours - " + col[1];
                    // dtF.Rows[1][i] = "BREAK-" + col[2];
                    dtF.Rows[0][i] = " " + col[0];// + "     " + col[2] + "      " + col[1];
                    dtF.Rows[1][i] = " " + col[2];
                }
                }
                else
                {
                //dtF.Rows[0][i] ="Total Scheduled Hours-" + col[0]   +"       @Net Hours -" + col[1] + " " +
                //"@Break  Hours - 0.0";//vishnu sir said to change sequence

                //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] ;

                //dtF.Rows[0][i] = "Scheduled Hours - " + col[0] + "       @Break  Hours - 0.0 " +
                //    "@Net Hours - " + col[1] + " ";

                dtF.Rows[0][i] = "  " + col[0];// + "     @  0.0 " +        " @ " + col[1] + " ";
            }
                dtF.Columns[i].ColumnName = colto[0];
                // }
           // }

            
        }
       
        
        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;
       
        if(ddrange.SelectedIndex==1)
        {
            grdTotal.Attributes.Add("style", "Width:100%;");
            grdTotal.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            //grdTotal.Attributes.Add("style", "Width:1000%;");
            grdFive.Attributes.Add("style", "Width:84%;");
            grdTotal.Attributes.Add("style", "text-align:center;");
            //tblTotal.Attributes.Add("style", "Width: 16%;");
            //tblTotal.Attributes.Add("style", "text-align:center;");
           // tblTotal.Attributes.Add("style", "margin-top: 2%;");
           // tblTotal.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#3d4c51");
            //tblTotal.Attributes.Add("style", "border:medium;");
        }
       // tblTotal.Visible = true;
        grdTotal.Visible = true;
        grdTotals.Visible = true;
        grdTotal.DataSource = dtF;
        grdTotal.DataBind();


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
    }

    public void GetTotalHour()
    {
        grdHalfMonth.Visible = false;
        //tblMonth.Visible = false;
        grdHalfMonths.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr1 = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtFs = new DataTable();

        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {

            //var col1 = dt1.ToString("dddd");
            //var col = dt1.ToString("dddd-MMM-yyyy");
           // var col = dt1.ToString("dd");
            var col = dt1.ToString(" dd-MMM-yyyy");

            try
            {
                DataTable dtflt = dthr1.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n" + t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                dtFs.Columns.Add(col);
                int datavalues = dtFs.Columns.Count;

                if (datavalues == 14)
                {
                    break;
                }
               
                //if (datavalues >= 1 && datavalues < 8)
                //{
                //    continue;
                //}
                //if (datavalues < 8)
                //{
                //    dtFs.Columns.RemoveAt(col);
                //}
            }
            catch (Exception ex)
            {


                //int datavalues = dtFs.Columns.Count;

                //if (datavalues < 7)

                //{
                //    dtFs.Columns.Add(col);
                //    //dtFs.Columns.Remove(col);
                //    goto stop;
                //}

              

                dtFs.Columns.Add(col + "\n0.0*0.0");
                int datavalues = dtFs.Columns.Count;
                if (datavalues == 14)
                {
                    break;
                }

                //if (datavalues == 13)
                //{
                //    break;
                //}
                
               

                
                //throw;
            }
            // int datav = dtFs.Columns.Count;
            //if(datav<8)
            //{
            //    continue;
            //}

            
                dt1 = dt1.AddDays(1);
            

            
       // stop:
        }


        DataRow dr = dtFs.NewRow();
        //dr[0] = "PLANNED";
        dtFs.Rows.Add(dr);

       
        



        for (int i = 0; i < dtFs.Columns.Count; i++)
        {
            if (i > 0 && i < 7 )
            {
                goto stops;
            }
            if(i<1)
            {
                dtFs.Columns.RemoveAt(i);

               // goto stops;
                //continue;
            }
            if (i < 2)
            {
                dtFs.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 3)
            {
                dtFs.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 4)
            {
                dtFs.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 5)
            {
                dtFs.Columns.RemoveAt(i);
               // continue;
            }
            if (i < 6)
            {
                dtFs.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 7)
            {
                dtFs.Columns.RemoveAt(i);
               // continue;
            }
       
          
        stops:
            
        

            if (i < 14)
            {
                
                var colto = dtFs.Columns[i].ColumnName.Split('\n');
               
                var col = colto[1].Split('*');
                
                if (col.Length > 2)
                {
                   
                    if (col[2].ToString() == "")
                    {
                        // dtFs.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFs.Rows[0][i] = " " + col[0];// + "     @ 0.0 " + "             @ " + col[1];
                    }
                    else
                    {
                        //dtFs.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFs.Rows[0][i] = " " + col[0];// + "     @ " + col[2] + "             @" + col[1];

                    }
                }
                else
                {
                    // dtFs.Rows[0][i] = " Scheduled Hours  - " + col[0] ;
                    dtFs.Rows[0][i] = "  " + col[0];// + "  @ 0.0 "  + "                        @ " + col[1] + " ";
                }
              
                dtFs.Columns[i].ColumnName = colto[0];
            }
           // stops;
         }
       

        

       

        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;

        if (ddrange.SelectedIndex == 1)
        {
            grdHalfMonth.Attributes.Add("style", "Width:100%;");
            //grdHalfMonth.Attributes.Add("style", "text-align:center;");
            grdHalfMonth.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            //grdHalfMonth.Attributes.Add("style", "Width:1000%;");
            //grdHalfMonth.Attributes.Add("style", "text-align:center;");
            grdHalfMonth.Attributes.Add("style", "Width:1000%;");
            grdHalfMonth.Attributes.Add("style", "text-align:center;");
        }

        grdHalfMonth.Visible = true;
        grdHalfMonths.Visible = true;
       // tblMonth.Visible = true;
        //grdHalfMonth.CellPadding = 50;
        grdHalfMonth.DataSource = dtFs;
        grdHalfMonth.DataBind();


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
    }

    public void GetTotalHourQty()
    {
        grdQterly.Visible = false;
        grdQterlys.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr1 = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtFq = new DataTable();

        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {

            //var col1 = dt1.ToString("dddd");
            //var col = dt1.ToString("dddd-MMM-yyyy");
            //var col = dt1.ToString("dd");
            var col = dt1.ToString(" dd-MMM-yyyy");


            try
            {
                DataTable dtflt = dthr1.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n" + t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                //dtFq.Columns.Add(col);
                int dataval = dtFq.Columns.Count;
                dtFq.Columns.Add(col + "\n0.0*0.0");
                if (dataval == 20)
                {
                   break;
                }
            }
            catch (Exception ex)
            {
                int datavalues = dtFq.Columns.Count;
                dtFq.Columns.Add(col + "\n0.0*0.0");
                if (datavalues == 20)
                {
                    break;
                }
            }
                dt1 = dt1.AddDays(1);
        }


        DataRow dr = dtFq.NewRow();
        //dr[0] = "PLANNED";
        dtFq.Rows.Add(dr);






        for (int i = 0; i < dtFq.Columns.Count; i++)
        {
            if (i > 0 && i < 14)
            {
                goto stops;
            }
            if (i < 1)
            {
                dtFq.Columns.RemoveAt(i);

                // goto stops;
                //continue;
            }
            if (i < 2)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 3)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 4)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 5)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 6)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 7)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 8)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 9)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 10)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 11)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 12)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 13)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 14)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }

        stops:

            //int numCols = dt.Columns.Count;

            //foreach (String str in A)
            //{//loop all strings in arrays

            //    for (int i = 0; i < numCols; i++)
            //    {//loop all columnames in dataTable B
            //        string columnName = dt.Columns[i].ColumnName.ToString();//get column Name
            //                                                                //check if columnNames in B are equal to 
            //        if (!str.Equals(columnName){
            //            //remove column
            //            dt.Columns.RemoveAt(i);
            //        }
            //        else
            //        {
            //            //column name is equal to the string array
            //        }
            //    }
            //}

            if (i < 22)
            {

                var colto = dtFq.Columns[i].ColumnName.Split('\n');
                //if (i < 8)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                //if (i < 7)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                var col = colto[1].Split('*');

                if (col.Length > 2)
                {
                    if (col[2].ToString() == "")
                    {
                        //dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFq.Rows[0][i] = " " + col[0];// + "     @ 0.0 " + "             @ " + col[1];
                    }
                    else
                    {
                        //dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFq.Rows[0][i] = " " + col[0];// + "     @ " + col[2] + "             @ " + col[1];

                    }
                }
                else
                {
                    // dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                    dtFq.Rows[0][i] = "" + col[0];// +  "       @ 0.0 " + "                  @ " + col[1] + " ";
                }
                dtFq.Columns[i].ColumnName = colto[0];
            }
            // stops;
        }


        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;

        if (ddrange.SelectedIndex == 1)
        {
            grdQterly.Attributes.Add("style", "Width:100%;");
            grdQterly.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            grdQterly.Attributes.Add("style", "Width:1000%;");
            grdQterly.Attributes.Add("style", "text-align:center;");
        }

        grdQterly.Visible = true;
        grdQterlys.Visible = true;
        grdQterly.DataSource = dtFq;
        grdQterly.DataBind();


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
    }

    public void GetTotalHourFour()
    {
        grdFourWeek.Visible = false;
        grdFourWeeks.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr1 = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtFq = new DataTable();

        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {

            //var col1 = dt1.ToString("dddd");
            //var col = dt1.ToString("dddd-MMM-yyyy");
            //var col = dt1.ToString("dd");
            var col = dt1.ToString(" dd-MMM-yyyy");

            try
            {
                DataTable dtflt = dthr1.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n" + t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                //dtFq.Columns.Add(col);
                int datavalues = dtFq.Columns.Count;
                dtFq.Columns.Add(col + "\n0.0*0.0");
                if (datavalues == 27)
                {
                    break;
                }
                //if (datavalues >= 1 && datavalues < 8)
                //{
                //    continue;
                //}
                //if (datavalues < 8)
                //{
                //    dtFs.Columns.RemoveAt(col);
                //}
            }
            catch (Exception ex)
            {
                int datavalues = dtFq.Columns.Count;

                //if (datavalues < 21)

                //{
                //    dtFq.Columns.Add(col);
                //    //dtFs.Columns.Remove(col);
                //    goto stop;
                //}
                dtFq.Columns.Add(col + "\n0.0*0.0");
                if (datavalues == 27)
                {
                    break;
                }




                //throw;
            }
        // int datav = dtFs.Columns.Count;
        //if(datav<8)
        //{
        //    continue;
        //}
        //stop:

            dt1 = dt1.AddDays(1);
        }


        DataRow dr = dtFq.NewRow();
        //dr[0] = "PLANNED";
        dtFq.Rows.Add(dr);






        for (int i = 0; i < dtFq.Columns.Count; i++)
        {
            if (i > 0 && i < 21)
            {
                goto stops;
            }
            if (i < 1)
            {
                dtFq.Columns.RemoveAt(i);

                // goto stops;
                //continue;
            }
            if (i < 2)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 3)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 4)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 5)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 6)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 7)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 8)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 9)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 10)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 11)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 12)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 13)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 14)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }

            if (i < 15)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 16)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 17)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 18)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 19)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 20)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 21)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
           


        stops:

            //int numCols = dt.Columns.Count;

            //foreach (String str in A)
            //{//loop all strings in arrays

            //    for (int i = 0; i < numCols; i++)
            //    {//loop all columnames in dataTable B
            //        string columnName = dt.Columns[i].ColumnName.ToString();//get column Name
            //                                                                //check if columnNames in B are equal to 
            //        if (!str.Equals(columnName){
            //            //remove column
            //            dt.Columns.RemoveAt(i);
            //        }
            //        else
            //        {
            //            //column name is equal to the string array
            //        }
            //    }
            //}

            if (i < 28)
            {

                var colto = dtFq.Columns[i].ColumnName.Split('\n');
                //if (i < 8)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                //if (i < 7)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                var col = colto[1].Split('*');

                if (col.Length > 2)
                {
                    if (col[2].ToString() == "")
                    {
                        // dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFq.Rows[0][i] = " " + col[0];// + "     @ 0.0 " + "             @ " + col[1];
                    }
                    else
                    {
                        //dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFq.Rows[0][i] = " " + col[0];//; + "     @ " + col[2] + "             @ " + col[1];

                    }
                }
                else
                {
                    // dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                    dtFq.Rows[0][i] = " " + col[0];// + "       @ 0.0 " + "                        @ " + col[1] + " ";
                }
                dtFq.Columns[i].ColumnName = colto[0];
            }
            // stops;
        }


        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;

        if (ddrange.SelectedIndex == 1)
        {
            grdFourWeek.Attributes.Add("style", "Width:100%;");
            grdFourWeek.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            grdFourWeek.Attributes.Add("style", "Width:1000%;");
            grdFourWeek.Attributes.Add("style", "text-align:center;");
        }

        grdFourWeek.Visible = true;
        grdFourWeeks.Visible = true;
        grdFourWeek.DataSource = dtFq;
        grdFourWeek.DataBind();


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
    }

    public void GetTotalHourFive()
    {
        grdFive.Visible = false;
        grdFives.Visible = false;
        string query = @"a.EmpId = '" + ddlemployee.SelectedValue + "' and a.BussID = '" + ddl_Business.SelectedValue + "'  and a.TimeSheetDate between '" + txtFromDate.Text + "' and '" + txtToDate.Text + "'";
        DataTable dthr1 = Comm.GetAllDataFromTable("timesheetentry  as a left join breakTime as b on isnull(a.ProjectId,0)=b.ProjectId and a.EmpId = b.EmpId and a.TimeSheetDate=b.TimeSheetDate ", "cast(replace(substring(b.BreakStart,len(b.BreakStart)-4,5),':','') as numeric(18,0)) BRKSTART,cast(replace(substring(b.BreakEnd,len(b.BreakEnd)-4,5),':','') as numeric(18,0)) BRKEND, a.Regular, a.Total,a.BreakTotal,format(a.ReqularStart, 'hh:mm tt') STARTTIME, format(a.Regularend, 'hh:mm tt')ENDTIME, format(a.TimeSheetDate, 'dd-MMM-yyyy')DT,cast(format(a.ReqularStart, 'HHmm') as numeric(18,0)) STARTATT, cast(format(a.Regularend, 'HHmm') as numeric(18,0))ENDATT", query, "");
        DateTime dt1 = DateTime.Parse(txtFromDate.Text);
        DateTime dt2 = DateTime.Parse(txtToDate.Text);
        hdnDate1.Value = dt2.Year.ToString() + "," + (dt2.Month).ToString() + "," + dt2.Day.ToString();

        hdnDate.Value = dt1.Year.ToString() + "," + (dt1.Month).ToString() + "," + dt1.Day.ToString();

        DateTime dttimes = DateTime.Parse(txtFromDate.Text + " 12:00 AM");
        DateTime dttimee = DateTime.Parse(txtFromDate.Text + " 11:59 PM");
        DataTable dtFq = new DataTable();

        //dtF.Columns.Add("Head");
        //dtF.Columns.Add("TmColumnName");
        while (dt1 <= dt2)
        {

            //var col1 = dt1.ToString("dddd");
            //var col = dt1.ToString("dddd-MMM-yyyy");
            //var col = dt1.ToString("dd");
            var col = dt1.ToString(" dd-MMM-yyyy");
            // grdFive.Attributes.Add("style", "Width:57.30%;");


            try
            {
                DataTable dtflt = dthr1.Select("DT='" + dt1.ToString("dd-MMM-yyyy") + "'").CopyToDataTable();
                TimeSpan t1 = TimeSpan.Parse(dtflt.Rows[0]["REGULAR"].ToString());
                TimeSpan tot = TimeSpan.Parse(dtflt.Rows[0]["TOTAL"].ToString());
                TimeSpan brk = TimeSpan.Parse(dtflt.Rows[0]["BreakTotal"].ToString());
                for (int i = 1; i < dtflt.Rows.Count; i++)
                {
                    t1 = t1 + TimeSpan.Parse(dtflt.Rows[i]["REGULAR"].ToString());
                    tot = tot + TimeSpan.Parse(dtflt.Rows[i]["TOTAL"].ToString());
                    brk = brk + TimeSpan.Parse(dtflt.Rows[i]["BreakTotal"].ToString());
                }

                string brkhd = brk.TotalMinutes > 0 ? brk.Hours.ToString() + "." + brk.Minutes.ToString() : "";
                col = col + "\n" + t1.Hours.ToString() + "." + t1.Minutes.ToString() + "*" + tot.Hours.ToString() + "." + tot.Minutes.ToString() + "*" + brkhd;
                dtFq.Columns.Add(col);
                int datavalues = dtFq.Columns.Count;
                //if (datavalues >= 1 && datavalues < 8)
                //{
                //    continue;
                //}
                //if (datavalues < 8)
                //{
                //    dtFs.Columns.RemoveAt(col);
                //}
            }
            catch (Exception ex)
            {
                int datavalues = dtFq.Columns.Count;

                if (datavalues < 28)

                {
                    //col.ItemStyle.Width = Unit.Pixel(150);
                    dtFq.Columns.Add(col);
                    //col.ItemStyle.Width = Unit.Pixel(150);
                    //dtFs.Columns.Remove(col);
                    goto stop;
                }
               // col.ItemStyle.Width = Unit.Pixel(150);
                dtFq.Columns.Add(col + "\n0.0*0.0");
                if (datavalues == 32)
                {
                    break;
                }




                //throw;
            }
        // int datav = dtFs.Columns.Count;
        //if(datav<8)
        //{
        //    continue;
        //}
        stop:

            dt1 = dt1.AddDays(1);
        }


        DataRow dr = dtFq.NewRow();
        //dr[0] = "PLANNED";
        dtFq.Rows.Add(dr);






        for (int i = 0; i < dtFq.Columns.Count; i++)
        {
            if (i > 0 && i < 28)
            {
                goto stops;
            }
            if (i < 1)
            {
                dtFq.Columns.RemoveAt(i);

                // goto stops;
                //continue;
            }
            if (i < 2)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 3)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 4)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 5)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 6)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 7)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 8)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 9)
            {
                dtFq.Columns.RemoveAt(i);
                //continue;
            }
            if (i < 10)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 11)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 12)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 13)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 14)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }

            if (i < 15)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 16)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 17)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 18)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 19)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 20)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 21)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 22)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 23)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 24)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 25)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 26)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 27)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }
            if (i < 28)
            {
                dtFq.Columns.RemoveAt(i);
                // continue;
            }




        stops:

            
            if (i < 32)
            {

                var colto = dtFq.Columns[i].ColumnName.Split('\n');
                //if (i < 8)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                //if (i < 7)
                //{
                //    dtFs.Columns.RemoveAt(i);
                //    continue;
                //}
                var col = colto[1].Split('*');

                if (col.Length > 2)
                {
                    if (col[2].ToString() == "")
                    {
                        //dtFq.Rows[0][i] = "Scheduled Hours - " + col[0];
                        dtFq.Rows[0][i] = " " + col[0];// + "     @ 0.0 " + "             @ " + col[1];
                    }
                    else
                    {
                        //dtFq.Rows[0][i] = "Scheduled Hours - " + col[0] ;
                        dtFq.Rows[0][i] = " " + col[0];// + "     @ " + col[2] + "             @ " + col[1];

                    }
                }
                else
                {
                    // dtFq.Rows[0][i] = "              " + "Scheduled Hours - " + col[0] + " ";

                    dtFq.Rows[0][i] = "              " + " " + col[0];// +"                " + "       @ 0.0 " +    "              @ " + col[1] + " ";
                }
                dtFq.Columns[i].ColumnName = colto[0];
            }
            // stops;
        }


        dt1 = DateTime.Parse(txtFromDate.Text);
        int colind = 2;

        
        if (ddrange.SelectedIndex == 1)
        {
            grdFive.Attributes.Add("style", "Width:20%;");
            //grdFive.Attributes.Add("style", "text-align:center;");
        }
        else if (ddrange.SelectedIndex == 2)
        {
            grdFive.Attributes.Add("style", "Width:57.30%;");
           // grdFive.HorizontalAlign = HorizontalAlign.Center;
            //grdFive.Attributes.Add("style", "text-align:center;");

            grdsix.Attributes.Add("style", "Width:28.45%;");

            grdEight.Attributes.Add("style", "Width:14.45%;");

            // grdSeven.Attributes.Add("style", "Width:57.30%;");
            // grdFive.HorizontalAlign = HorizontalAlign.Center;
            //grdFive.Attributes.Add("style", "text-align:center;");

            grdSeven.Attributes.Add("style", "Width:43%;");

            // grdsix.Attributes.Add("style", "text-align:center;");
        }

        if(dtFq.Columns.Count == 1)
        {
            grdSeven.Visible = false;
            grdSevens.Visible = false;
            grdFive.Visible = false;
            grdFives.Visible = false;
            grdsix.Visible = false;
            grdEight.Visible = true;
            grdEight.DataSource = dtFq;
            grdEight.DataBind();

        }
       else if (dtFq.Columns.Count == 2)
        {
            grdSeven.Visible = false;
            grdSevens.Visible = false;
            grdFive.Visible = false;
            grdFives.Visible = false;
            grdEight.Visible = false;
            grdsix.Visible = true;
            grdsixs.Visible = true;
            grdsix.DataSource = dtFq;
            grdsix.DataBind();
        }
        else if(dtFq.Columns.Count == 3)
        {
            grdSeven.Visible = true;
            grdSevens.Visible = true;
            grdFive.Visible = false;
            grdFives.Visible = false;
            grdsix.Visible = false;
            grdsixs.Visible = false;
            grdEight.Visible = false;
            grdSeven.DataSource = dtFq;
            grdSeven.DataBind();
        }
        else if(dtFq.Columns.Count == 4)
        {
            grdSeven.Visible = false;
            grdSevens.Visible = false;
            grdsix.Visible = false;
            grdsixs.Visible = false;
            grdEight.Visible = false;
            grdFive.Visible = true;
            grdFives.Visible = true;
            grdFive.DataSource = dtFq;

            grdFive.DataBind();
        }
        


        //select REGULAR, TOTAL, BreakTotal  from TimeSheetEntry where 
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
    public void GetBusiness()
    {
        DataTable dt = new DataTable();
        ddlemployee.Items.Clear();
        var UserType_acc = Session["UserType_acc"];
        if (UserType_acc == null)
        {
            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                //Comm.FillDropDownList(ddlemployee, "AddEmployee AE JOIN EmpAddress EAD ON EAD.EmpAddressId=AE.ADDRESSID JOIN UserAcc UA ON UA.Email=EAD.MainEmail  join AssginBussiness assb on assb.EmployeeID=AE.EmployeeID", "DISTINCT AE.EmployeeId", "dbo.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "AE.UserType=4 and assb.BusinessID IN (SELECT BusinessID FROM AssginBussiness WHERE EmployeeId=" + Session["EmployeeId"].ToString() + ") and IsActive='YES'", "EmpName");
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                dt1 = Comm.GetAllDataFromTable("AddEmployee AE JOIN EmpAddress EAD ON EAD.EmpAddressId=AE.ADDRESSID JOIN UserAcc UA ON UA.Email=EAD.MainEmail  join AssginBussiness assb on assb.EmployeeID=AE.EmployeeID", "DISTINCT AE.EmployeeId, dbo.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "AE.UserType=4 and assb.BusinessID IN (SELECT BusinessID FROM AssginBussiness WHERE EmployeeId=" + Session["EmployeeId"].ToString() + ") and IsActive='YES'", "EmpName");
                dt2 = RemoveDuplicateRows(dt1, "EmpName");
                ddlemployee.Items.Clear();
                ddlemployee.Items.Add("Please Select");
                ddlemployee.SelectedItem.Value = "";

                if (dt2.Rows.Count > 0)
                {
                    ddlemployee.DataSource = dt2;
                    ddlemployee.DataValueField = dt2.Columns[0].ToString();
                    ddlemployee.DataTextField = dt2.Columns[1].ToString();
                    ddlemployee.DataBind();
                    ddlemployee.SelectedIndex = 0;
                }
            }
            else
            {
                // Comm.FillDropDownList(ddlemployee, "AssginBussiness assb  join UserAcc ua on ua.EmployeeId=assb.EmployeeID", "DISTINCT assb.EmployeeId", "dbo.FN_DESC('AddEmployee',assb.EmployeeID)EmpName", "BusinessID in(select BusinessID from AddBusiness where MotherOrgId=" + Session["MotherOrgid"].ToString() + ") and assb.UserTypeId =4", "EmpName");

                // Comm.FillDropDownList(ddlemployee, "AssginBussiness assb  join UserAcc ua on ua.EmployeeId=assb.EmployeeID JOIN AddEmployee AD ON AD.EmployeeID=assb.EmployeeID", "DISTINCT assb.EmployeeId", "dbo.FN_DESC('AddEmployee',assb.EmployeeID)EmpName", "assb.BusinessID in(select BusinessID from AddBusiness where MotherOrgId=" + Session["MotherOrgid"].ToString() + ") and assb.UserTypeId =4 and IsActive='YES'", "EmpName");
                Comm.Sp_Report3DropList(ddlemployee, int.Parse(Session["MotherOrgid"].ToString()));
            }
        }
        else
        {
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE JOIN EmpAddress EAD ON EAD.EmpAddressId=AE.ADDRESSID JOIN UserAcc UA ON UA.Email=EAD.MainEmail join AssginBussiness assb on assb.EmployeeID=AE.EmployeeID", "DISTINCT AE.EmployeeId", "dbo.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "AE.UserType=4 and assb.BusinessID IN (SELECT BusinessID FROM AssginBussiness WHERE EmployeeId=" + Session["EmployeeId"].ToString() + ") and assb.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + " and IsActive='YES'", "EmpName");
        }

        //dt = Comm.GetAllDataFromTable("AddBusiness", "BusinessID,CompanyName", "MotherOrgId=" + Session["MotherOrgid"].ToString() + "", "BusinessID");
        //ddlBusiness.DataSource = dt;
        //ddlBusiness.DataTextField = "CompanyName";
        //ddlBusiness.DataValueField = "BusinessID";
        //ddlBusiness.DataBind();
    }

    protected void GVBusiness_PreRender(object sender, EventArgs e)
    {
        if (GVBusiness.Rows.Count > 0)
        {
            GVBusiness.UseAccessibleHeader = true;
            GVBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    protected void btnshow_Click(object sender, EventArgs e)
    {
        DateTime frdate = DateTime.Parse(txtFromDate.Text);
        DateTime todate = DateTime.Parse(txtToDate.Text);
        var daysdiff = (todate - frdate).TotalDays;

        if (ddrange.SelectedIndex == 1)
        {
            if (daysdiff > 7)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date Range of One Week Only', 'warning');", true);
                return;
            }
        }
        if (ddrange.SelectedIndex == 2)
        {
            if (daysdiff > 31)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date Range of One Month Only', 'warning');", true);
                return;
            }
        }


        if (ddrange.SelectedValue == "Select")
        {
            grdTotal.DataSource = null;
            grdTotal.DataBind();
            grdTotal.Visible = false;
            grdTotals.Visible = false;
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Date Range', 'warning');", true);
        }
        else
        {
            if (ddrange.SelectedIndex == 1)
            {
                GetTotalHours();
                GetTotalHeader();
                grdTotal.Visible = true;
                grdHeaderDays.Visible = true;
                lblHeader.Visible = true;
                lblHeaderMonthly.Visible = false;

                grdHalfMonth.Visible = false;
                grdQterly.Visible = false;
                grdFourWeek.Visible = false;
                grdSeven.Visible = false;
                //grdSevens.Visible = false;
                grdFive.Visible = false;
                //grdFives.Visible = false;
                grdsix.Visible = false;
                grdEight.Visible = false;
            }
            else
            {
                grdTotal.Visible = false;
                lblHeader.Visible = false;
                lblHeaderMonthly.Visible = true;
                GetTotalHeader();
                GetTotalHours();
                GetTotalHour();
                GetTotalHourQty();
                GetTotalHourFour();
                GetTotalHourFive();
            }
            //  submit();
        }
    }

    public void submit()
    {
        //if (this.ddlBusiness.SelectedIndex == 0)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business', 'error');", true);
        //    return;
        //}

        if (ddrange.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'error');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);

        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        DateTime dtfrom3 = DateTime.Parse(txtFromDate2.Text);
        DateTime dtto3 = DateTime.Parse(txtToDate2.Text);

        if (dtfrom3 > dtto3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'From date is greater than To date for second date range', 'error');", true);
            return;
        }
        TimeSpan difference2 = dtto3 - dtfrom3;
        int diffday2 = difference2.Days;
        if (diffday2 > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'More than One year date selection not allowed for second date range', 'error');", true);
            return;
        }
        DataTable dt = new DataTable();
        string a = Session["EmployeeId"].ToString();

        if (hdnvisible.Value == "NO")
        {
            if (Session["UserType_acc"] == null)
            {
                if (Session["UserType"].ToString() == "Admin")
                {
                    dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a);
                }
                else
                {
                    int Status = 2;
                    dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a,Status);
                }
            }
            else
            {
                dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
            }


            GVBusiness.DataSource = dt;
            GVBusiness.DataBind();
            GVBusiness.Visible = true;
            GVCompare1.Visible = false;
            GVCompare2.Visible = false;
        }
        else if (hdnvisible.Value == "YES")
        {
            dt = Comm.ReportAcc_AllCustomerWise3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), "0");
            GVCompare1.DataSource = dt;
            GVCompare1.DataBind();
            dt = Comm.ReportAcc_AllCustomerWise3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom3), Convert.ToDateTime(dtto3),"0");
            GVCompare2.DataSource = dt;
            GVCompare2.DataBind();
            GVBusiness.Visible = false;
            GVCompare1.Visible = true;
            GVCompare2.Visible = true;
        }
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();

        DateTime dtfrom2 = DateTime.Parse(txtFromDate2.Text);
        DateTime dtto2 = DateTime.Parse(txtToDate2.Text);
        hdndate3.Value = dtfrom2.Year.ToString() + "," + (dtfrom2.Month - 1).ToString() + "," + dtfrom2.Day.ToString();
        hdndate4.Value = dtto2.Year.ToString() + "," + (dtto2.Month - 1).ToString() + "," + dtto2.Day.ToString();
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
    }

    protected void GVBusiness_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVBusiness.PageIndex = e.NewPageIndex;
        btnshow_Click(sender, e);
    }

    protected void ddrange_SelectedIndexChanged(object sender, EventArgs e)
    {

        //if (ddrange.SelectedIndex == 1)
        //{
        //    grdHeaderDay.Visible = true;
        //    //grdTotal.DataSource = null;
        //    //grdTotal.DataBind();
        //    //grdTotal.Visible = false;
        //}

        hdnrange.Value = ddrange.SelectedIndex.ToString();
        if (ddrange.SelectedIndex == 1)
        {
            txtFromDate.Enabled = true;

            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

            if (ddrange.SelectedIndex == 1)
            {
                thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            }
            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month).ToString() + "," + thisWeekEnd.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 2)
        {
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate;
            thisWeekStart = thisWeekStart;
            var thisWeekStart1 = baseDate;
            var thisWeekEnd = thisWeekStart1.AddMonths(1);
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month).ToString() + "," + thisWeekEnd.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 3)
        {
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-30).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            txtToDate.Enabled = true;
        }
        if (ddrange.SelectedIndex == 4)
        {
            
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-60).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 5)
        {
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-90).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 6)
        {
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var firstDayOfMonth = new DateTime(baseDate.Year, baseDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            hdnDate.Value = firstDayOfMonth.Year.ToString() + "," + (firstDayOfMonth.Month - 1).ToString() + "," + "01";
            hdnDate1.Value = lastDayOfMonth.Year.ToString() + "," + (lastDayOfMonth.Month - 1).ToString() + "," + lastDayOfMonth.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 7)
        {
            txtFromDate.Enabled = true;
            txtToDate.Enabled = true;
        }
        DateTime dtfrom2 = DateTime.Parse(txtFromDate2.Text);
        //DateTime dtto2 = DateTime.Parse(txtToDate2.Text);
        hdndate3.Value = dtfrom2.Year.ToString() + "," + (dtfrom2.Month - 1).ToString() + "," + dtfrom2.Day.ToString();
        //hdndate4.Value = dtto2.Year.ToString() + "," + (dtto2.Month - 1).ToString() + "," + dtto2.Day.ToString();
    }

    protected void ddapproval_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList ddapproval = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)ddapproval.NamingContainer;
        DropDownList ddstatus = (DropDownList)GVBusiness.Rows[gvr.RowIndex].FindControl("ddapproval") as DropDownList;
        HiddenField hdncomid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hdncomid") as HiddenField;
        Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName") as Label;
        DataTable dt = new DataTable();
        // DropDownList ddlapproval = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hdncomid");
        if (ddstatus.SelectedIndex == 1)
        {
            string query = " [USERID] = (SELECT USERID from[dbo].[MappingTable]";
            query += " where[EmployeeID] = " + Convert.ToInt32(ddlemployee.SelectedValue) + ")";
            query += " AND [BusinessID] = " + Convert.ToInt32(hdncomid.Value) + "";
            DataTable dtAccDetails = Comm.GetAllDataFromTable("MappingTable", "EmployeeID", query, "");
            if (dtAccDetails.Rows.Count == 0)
            {
                return;
            }
            dt = Comm.ReportAcc_CustomerWise_Emp((hdncomid.Value), Convert.ToDateTime(hdnfrmdt.Value), Convert.ToDateTime(hdntodt.Value), Convert.ToInt32(dtAccDetails.Rows[0][0].ToString()));
            GvExport1.DataSource = dt;
            GvExport1.DataBind();
            ddstatus.SelectedIndex = 0;
            Response.ClearContent();
            string filename = lblEmployeeName.Text + "_" + hdnfrmdt.Value + "_" + hdntodt.Value;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GvExport1.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GvExport1.RenderControl(htw);
            //submit();
            string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Custom Report for Time Sheet For  " + lblEmployeeName.Text + "</td></tr>" +
                                        "<tr><td>" + lblEmployeeName.Text + "</td></tr>" +
                                         "<tr><td colspan=2>" + hdnfrmdt.Value.ToString() + "-" + hdntodt.Value.ToString() + "</td></tr>" +
               "</Table>";
            Response.Write(headerTable);
            Response.Write(stringWriter.ToString());
            ddapproval.SelectedIndex = 0;

            Response.End();
            ddstatus.SelectedIndex = 0;
            submit();
        }
        if (ddapproval.SelectedIndex == 2)
        {

            ddstatus.SelectedIndex = 0;
            submit();
        }
        ddstatus.SelectedIndex = 0;
        submit();
    }

    protected void GVBusiness_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GVBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Excel")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();

            GridViewRow row = GVBusiness.Rows[index];
            DropDownList ddstatus = (DropDownList)GVBusiness.Rows[index].FindControl("ddapproval") as DropDownList;
            HiddenField hdncomid = (HiddenField)GVBusiness.Rows[index].FindControl("hdncomid") as HiddenField;
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid") as HiddenField;
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName") as Label;

            DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
            DateTime dtto = DateTime.Parse(txtToDate.Text);
            DataTable dt = Comm.ReportAcc_CustomerWise_Emp((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value)); ;
            TimeSpan gross_hours = new TimeSpan();
            TimeSpan break_total = new TimeSpan();
            TimeSpan net = new TimeSpan();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][4].ToString().Trim() != "")
                {
                    var split = Convert.ToString(dt.Rows[i][4]).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                }
                if (dt.Rows[i][5].ToString().Trim() != "")
                {
                    var split1 = Convert.ToString(dt.Rows[i][5]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                }
                if (dt.Rows[i][6].ToString().Trim() != "")
                {
                    var split2 = Convert.ToString(dt.Rows[i][6]).Split(':');
                    net = net.Add(new TimeSpan(int.Parse(split2[0]), int.Parse(split2[1]), 0));
                }

            }
            string t1 = string.Format("{0}:{1}", gross_hours.Hours + (gross_hours.Days * 24), gross_hours.Minutes, gross_hours.Seconds);
            string t2 = string.Format("{0}:{1}", break_total.Hours + (break_total.Days * 24), break_total.Minutes, break_total.Seconds);
            string t3 = string.Format("{0}:{1}", net.Hours + (net.Days * 24), net.Minutes, net.Seconds);
            var totalDisplay = t1;
            var totalDisplay1 = t2;
            var totalDisplay2 = t3;
            GvExport1.DataSource = dt;
            GvExport1.DataBind();

            Response.ClearContent();
            string filename = ddlemployee.SelectedItem.Text + dtfrom.ToString("yyyy/MM/dd") + "_" + dtto.ToString("yyyy/MM/dd");
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            // GvExport1.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GvExport1.RenderControl(htw);
            string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for Time Sheet For  " + lblEmployeeName.Text + "</td></tr>" +
                                        "<tr><td>" + ddlemployee.SelectedItem.Text + "</td></tr>" +
                                         "<tr><td colspan=2>" + dtfrom.ToString("yyyy/MM/dd") + "-" + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
               "</Table>";

            Response.Write(headerTable);
            Response.Write(stringWriter.ToString());
            headerTable = @"<Table><tr><td></td>" +
                           "<td></td>" +
                            "<td></td>" +
                             "<td></td>" +
                            "<td align='center'>" + totalDisplay.ToString() + "</td>" +
                            "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
                            "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
                            "</Table>";

            //submit();
            Response.Write(headerTable);

            Response.End();
        }
        else if (e.CommandName == "PDF")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();

            GridViewRow row = GVBusiness.Rows[index];
            DropDownList ddstatus = (DropDownList)GVBusiness.Rows[index].FindControl("ddapproval") as DropDownList;
            HiddenField hdncomid = (HiddenField)GVBusiness.Rows[index].FindControl("hdncomid") as HiddenField;
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName") as Label;
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid") as HiddenField;
            DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
            DateTime dtto = DateTime.Parse(txtToDate.Text);
            DataTable dt = Comm.ReportAcc_CustomerWise_Emp((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value)); ;
            TimeSpan gross_hours = new TimeSpan();
            TimeSpan break_total = new TimeSpan();
            TimeSpan net = new TimeSpan();

            for (int J = 0; J < dt.Rows.Count; J++)
            {
                if (dt.Rows[J][4].ToString().Trim() != "")
                {
                    var split = Convert.ToString(dt.Rows[J][4]).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                }
                if (dt.Rows[J][5].ToString().Trim() != "")
                {
                    var split1 = Convert.ToString(dt.Rows[J][5]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                }
                if (dt.Rows[J][6].ToString().Trim() != "")
                {
                    var split2 = Convert.ToString(dt.Rows[J][6]).Split(':');
                    net = net.Add(new TimeSpan(int.Parse(split2[0]), int.Parse(split2[1]), 0));
                }

            }
            string t1 = string.Format("{0}:{1}", gross_hours.Hours + (gross_hours.Days * 24), gross_hours.Minutes, gross_hours.Seconds);
            string t2 = string.Format("{0}:{1}", break_total.Hours + (break_total.Days * 24), break_total.Minutes, break_total.Seconds);
            string t3 = string.Format("{0}:{1}", net.Hours + (net.Days * 24), net.Minutes, net.Seconds);
            var totalDisplay = t1;
            var totalDisplay1 = t2;
            var totalDisplay2 = t3;
            GvExport1.DataSource = dt;
            GvExport1.DataBind();


            string filename = ddlemployee.SelectedItem.Text + "_" + dtfrom.ToString("yyyy/MM/dd") + "_" + dtto.ToString("yyyy/MM/dd");
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            GvExport1.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
#pragma warning disable CS0612 // Type or member is obsolete  
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete  
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();
            Chunk chunk = new Chunk("Report for Time Sheet For  " + lblEmployeeName.Text);
            Paragraph preface = new Paragraph(chunk);
            // preface.setAlignment(Element.ALIGN_CENTER);
            preface.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(preface);

            chunk = new Chunk(ddlemployee.SelectedItem.Text);
            pdfDoc.Add(new Paragraph(chunk));

            chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd") + "-" + dtto.ToString("yyyy/MM/dd"));
            pdfDoc.Add(new Paragraph(chunk));

            chunk = new Chunk(Environment.NewLine);
            pdfDoc.Add(new Paragraph(chunk));
            PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            foreach (DataColumn column in dt.Columns)
            {
                var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var titleChunk = new Chunk(column.ColumnName.ToString(), redListTextFont);
                PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
                cellq.Border = Rectangle.NO_BORDER;
                // cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

                pdfTable.AddCell(cellq);
            }
            int i = 1;
            foreach (DataRow row1 in dt.Rows)
            {
                //foreach (itema in row.ItemArray)
                //{
                //    pdfTable.AddCell(cell.Text.ToString());
                //}

                foreach (var item in row1.ItemArray)
                {
                    PdfPCell cellitem = new PdfPCell(new Phrase(item.ToString()));
                    cellitem.Border = Rectangle.NO_BORDER;

                    if (i % 2 != 0)
                    {
                        cellitem.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                    }
                    else
                    {
                        cellitem.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;//EFF3FB
                    }
                    pdfTable.AddCell(cellitem);

                }
                i = i + 1;
            }
            PdfPCell cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);

            cell = new PdfPCell(new Phrase(totalDisplay.ToString()));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalDisplay1.ToString()));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalDisplay2.ToString()));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
            pdfTable.AddCell(cell);

            // pdfDoc.Add(pdfTable);

            //htmlparser.Parse(sr);

            //*******************
            pdfDoc.Add(pdfTable);

            //htmlparser.Parse(sr);

            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            GvExport1.AllowPaging = true;
            GvExport1.DataBind();
        }
    }


    protected void ddrange2_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdnrange2.Value = ddrange2.SelectedIndex.ToString();
        if (ddrange2.SelectedIndex == 1)
        {
            txtFromDate2.Enabled = true;

            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

            if (ddrange2.SelectedIndex == 1)
            {
                thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
                thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);
            }
            hdndate3.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdndate4.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month - 1).ToString() + "," + thisWeekEnd.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange2.SelectedIndex == 2)
        {
            txtFromDate2.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            thisWeekStart = thisWeekStart.AddDays(-20).AddSeconds(-1);
            var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart1.AddDays(7).AddSeconds(-1);
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdndate3.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdndate4.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month - 1).ToString() + "," + thisWeekEnd.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange2.SelectedIndex == 3)
        {
            txtFromDate2.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-30).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdndate3.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdndate4.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            txtToDate.Enabled = true;
        }
        if (ddrange2.SelectedIndex == 4)
        {
            txtFromDate2.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-60).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdndate3.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdndate4.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange2.SelectedIndex == 5)
        {
            txtFromDate2.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-90).AddSeconds(-1);
            //thisWeekStart = thisWeekStart.AddMonths(-1).AddSeconds(-1);
            // var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            //var thisWeekEnd = baseDate;
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdndate3.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdndate4.Value = baseDate.Year.ToString() + "," + (baseDate.Month - 1).ToString() + "," + baseDate.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange2.SelectedIndex == 6)
        {
            txtFromDate2.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var firstDayOfMonth = new DateTime(baseDate.Year, baseDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            hdndate3.Value = firstDayOfMonth.Year.ToString() + "," + (firstDayOfMonth.Month - 1).ToString() + "," + "01";
            hdndate4.Value = lastDayOfMonth.Year.ToString() + "," + (lastDayOfMonth.Month - 1).ToString() + "," + lastDayOfMonth.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange2.SelectedIndex == 7)
        {
            txtFromDate2.Enabled = true;
            txtToDate2.Enabled = true;
        }
        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();
    }

    protected void lnkbtn_Click(object sender, EventArgs e)
    {
        hdnvisible.Value = "YES";
        divrange2.Attributes.Add("style", "display:block");
        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();
        GVBusiness.Visible = false;
        GVCompare1.Visible = false;
        GVCompare2.Visible = false;
        lnkbtn.Visible = false;
    }

    protected void ImageButton2_Click(object sender, ImageClickEventArgs e)
    {
        divrange2.Attributes.Add("style", "display:none");
        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();
        GVBusiness.Visible = false;
        GVCompare1.Visible = false;
        GVCompare2.Visible = false;
        lnkbtn.Visible = true;
        hdnvisible.Value = "NO";
    }

    protected void GVCompare1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVCompare1.PageIndex = e.NewPageIndex;
        btnshow_Click(sender, e);
    }

    protected void GVCompare2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVCompare2.PageIndex = e.NewPageIndex;
        btnshow_Click(sender, e);
    }

    protected void GVCompare1_PreRender(object sender, EventArgs e)
    {
        if (GVCompare1.Rows.Count > 0)
        {
            GVCompare1.UseAccessibleHeader = true;
            GVCompare1.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GVCompare2_PreRender(object sender, EventArgs e)
    {
        if (GVCompare2.Rows.Count > 0)
        {
            GVCompare2.UseAccessibleHeader = true;
            GVCompare2.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void lnkExcel_Click(object sender, EventArgs e)
    {
        if (ddrange.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'error');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'No Record Found', 'error');", true);
            return;
        }
        //    ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(ddlemployee.SelectedValue.ToString()) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','"+ Convert.ToString(ddlemployee.SelectedItem.Text.ToString()) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        string a = Session["EmployeeId"].ToString();
        DataTable dt = new DataTable();
        if (Session["UserType_acc"] == null)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a);
            }
            else
            {
                int Status = 2;
                dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a,Status);
            }
        }
        else
        {
            dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }

        TimeSpan gross_hours = new TimeSpan();
        TimeSpan break_total = new TimeSpan();
        TimeSpan net = new TimeSpan();

        if (dt.Rows.Count > 0)
        {
            GvExport.DataSource = dt;
            GvExport.DataBind();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][4].ToString().Trim() != "")
                {
                    var split = Convert.ToString(dt.Rows[i][3]).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                    var split1 = Convert.ToString(dt.Rows[i][4]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                    var split2 = Convert.ToString(dt.Rows[i][5]).Split(':');
                    net = net.Add(new TimeSpan(int.Parse(split2[0]), int.Parse(split2[1]), 0));
                }
            }
        }
        string t1 = string.Format("{0}:{1}", (gross_hours.Hours + (gross_hours.Days * 24)).ToString().PadLeft(2, '0'), gross_hours.Minutes.ToString().PadLeft(2, '0'));
        string t2 = string.Format("{0}:{1}", (break_total.Hours + (break_total.Days * 24)).ToString().PadLeft(2, '0'), break_total.Minutes.ToString().PadLeft(2, '0'));
        string t3 = string.Format("{0}:{1}", (net.Hours + (net.Days * 24)).ToString().PadLeft(2, '0'), net.Minutes.ToString().PadLeft(2, '0'), net.Seconds.ToString());
        var totalDisplay = t1;
        var totalDisplay1 = t2;
        var totalDisplay2 = t3;
        string filename = ddlemployee.SelectedItem.Text.ToString() + "_" + dtfrom.ToString() + "_" + dtto.ToString();
        Response.ClearContent();
        Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
        Response.ContentType = "application/excel";
        StringWriter stringWriter = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
        //GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
        GvExport.RenderControl(htw);
        string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + ddlemployee.SelectedItem.Text.ToString() + "</td></tr>" +
                                     "<tr><td>" + ddlemployee.SelectedItem.Text.ToString() + "</td></tr>" +
                                      "<tr><td colspan=2>" + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
            "</Table>";
        Response.Write(headerTable);
        Response.Write(stringWriter.ToString());
        headerTable = @"<Table><tr><td></td>" +
                       "<td></td>" +
                       "<td></td>" +
                        "<td align='center'>" + totalDisplay.ToString() + "</td>" +
                        "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
                        "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
                        "</Table>";
        Response.Write(headerTable);

        Response.End();
    }

    protected void lnkDirectPdf_Click(object sender, EventArgs e)
    {
        if (ddrange.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'error');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'No Record Found', 'error');", true);
            return;
        }
        //    ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(ddlemployee.SelectedValue.ToString()) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','"+ Convert.ToString(ddlemployee.SelectedItem.Text.ToString()) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        string a = Session["EmployeeId"].ToString();
        DataTable dt = new DataTable();
        if (Session["UserType_acc"] == null)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a);
            }
            else
            {
                int Status = 2;
                dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a,Status);
            }
        }
        else
        {
            dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }

        TimeSpan gross_hours = new TimeSpan();
        TimeSpan break_total = new TimeSpan();
        TimeSpan net = new TimeSpan();

        if (dt.Rows.Count > 0)
        {
            GvExport.DataSource = dt;
            GvExport.DataBind();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                if (dt.Rows[j][4].ToString().Trim() != "")
                {
                    var split = Convert.ToString(dt.Rows[j][3]).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                    var split1 = Convert.ToString(dt.Rows[j][4]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                    var split2 = Convert.ToString(dt.Rows[j][5]).Split(':');
                    net = net.Add(new TimeSpan(int.Parse(split2[0]), int.Parse(split2[1]), 0));
                }
            }
        }
        string t1 = string.Format("{0}:{1}", (gross_hours.Hours + (gross_hours.Days * 24)).ToString().PadLeft(2, '0'), gross_hours.Minutes.ToString().PadLeft(2, '0'));
        string t2 = string.Format("{0}:{1}", (break_total.Hours + (break_total.Days * 24)).ToString().PadLeft(2, '0'), break_total.Minutes.ToString().PadLeft(2, '0'));
        string t3 = string.Format("{0}:{1}", (net.Hours + (net.Days * 24)).ToString().PadLeft(2, '0'), net.Minutes.ToString().PadLeft(2, '0'), net.Seconds.ToString());
        var totalDisplay = t1;
        var totalDisplay1 = t2;
        var totalDisplay2 = t3;
        string filename = ddlemployee.SelectedItem.Text.ToString() + "_" + dtfrom.ToString() + "_" + dtto.ToString();
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        GvExport.RenderControl(hw);
        StringReader sr = new StringReader(sw.ToString());
        Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
#pragma warning disable CS0612 // Type or member is obsolete  
        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete  
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        pdfDoc.Open();
        Chunk chunk = new Chunk("Report for " + ddlemployee.SelectedItem.Text);

        Paragraph preface = new Paragraph(chunk);
        // preface.setAlignment(Element.ALIGN_CENTER);
        preface.Alignment = Element.ALIGN_CENTER;
        pdfDoc.Add(preface);

        chunk = new Chunk(ddlemployee.SelectedItem.Text);
        pdfDoc.Add(new Paragraph(chunk));

        chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
        pdfDoc.Add(new Paragraph(chunk));

        chunk = new Chunk(Environment.NewLine);
        pdfDoc.Add(new Paragraph(" "));
        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count);
        pdfTable.DefaultCell.Padding = 3;
        pdfTable.WidthPercentage = 100;
        pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

        foreach (DataColumn column in dt.Columns)
        {
            var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var titleChunk = new Chunk(column.ColumnName.ToString(), redListTextFont);
            PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            cellq.Border = Rectangle.NO_BORDER;
            //cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            pdfTable.AddCell(cellq);
        }
        int i = 1;
        foreach (DataRow row in dt.Rows)
        {

            //foreach (itema in row.ItemArray)
            //{
            //    pdfTable.AddCell(cell.Text.ToString());
            //}

            foreach (var item in row.ItemArray)
            {
                PdfPCell cellitem = new PdfPCell(new Phrase(item.ToString()));
                cellitem.Border = Rectangle.NO_BORDER;

                if (i % 2 != 0)
                {
                    cellitem.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
                }
                else
                {
                    cellitem.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
                }
                pdfTable.AddCell(cellitem);

            }
            i = i + 1;
        }
        PdfPCell cell = new PdfPCell(new Phrase(""));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(""));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(""));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay.ToString()));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay1.ToString()));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay2.ToString()));
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);

        pdfDoc.Add(pdfTable);

        //htmlparser.Parse(sr);

        pdfDoc.Close();
        Response.Write(pdfDoc);

        Response.End();
        GvExport.AllowPaging = true;
        GvExport.DataBind();
    }

    protected void ddl_Business_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_Business.SelectedIndex != 0)
        {
            var bussid = ddl_Business.SelectedValue;
            var empid = Session["EmployeeId"].ToString();
            //commented by kumar
            //Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId!=1   and Assb.UserTypeId=4 and ae.EmployeeID<>" + empid + "", "AE.EmployeeID ");
            //added by kumar as all user roles will be shown here. Vishnu sir said.
            //Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId!=1 and ae.EmployeeID<>" + empid + "", "AE.EmployeeID ");
            //added by kumar as all admininstrator and accountant will not have his timesheet entered.
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId not in (1,5,12) and ae.EmployeeID<>" + empid + "", "AE.EmployeeID ");
            DataTable MotherOrgiD = Comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + bussid + "", "");
            string MotherOrgID = MotherOrgiD.Rows[0]["MotherOrgId"].ToString();
        }
    }

    protected void grdTotal_PreRender(object sender, EventArgs e)
    {
        if (grdTotal.Rows.Count > 0)
        {
            grdTotal.UseAccessibleHeader = true;
            grdTotal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdTotal_DataBound(object sender, EventArgs e)
    {
        
    }

    protected void grdTotal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if(e.Row.RowType== DataControlRowType.DataRow)
        {
            //e.Row.Cells[0].Width = Unit.Pixel(50); // Replace with your desired width for column 1
            //e.Row.Cells[1].Width = Unit.Pixel(50); // Replace with your desired width for column 2
            //e.Row.Cells[2].Width = Unit.Pixel(50); // Replace with your desired width for column 1
            //e.Row.Cells[3].Width = Unit.Pixel(50); // Replace with your desired width for column 2
            //e.Row.Cells[4].Width = Unit.Pixel(50); // Replace with your desired width for column 2
            //e.Row.Cells[5].Width = Unit.Pixel(200); // Replace with your desired width for column 1
            //e.Row.Cells[6].Width = Unit.Pixel(200); // Replace with your desired width for column 2
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
                //e.Row.Cells[1].BackColor = System.Drawing.Color.Red;
            }

            
        }
    }

    public void GetAllDetails_Employee_List(string Bid)
    {
        if (Request.QueryString["UserRole"] == "CustomRole")
        {
            string[] role;
            ddlemployee.Items.Clear();
            ddlemployee.Items.Add("Please Select");
            ddlemployee.SelectedItem.Value = "";
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
                            ddlemployee.Items.Clear();
                            ddlemployee.DataSource = null;
                            ddlemployee.DataBind();
                            ddlemployee.Items.Add("Please Select");
                            ddlemployee.SelectedItem.Value = "";
                            dr = null;
                            dtr = null;
                            dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES' and [EmployeeID]='" + Request.QueryString["MangerId"].ToString() + "'");
                            if (dr.Count() > 0)
                            {
                                dtr = dr.CopyToDataTable();
                                ddlemployee.DataSource = dtr;
                                ddlemployee.DataValueField = "EmployeeID";
                                ddlemployee.DataTextField = "EmpName";
                                ddlemployee.DataBind();
                                ddlemployee.SelectedIndex = 0;
                                //break;
                            }

                            role_check_employee_time_sheet_permission = "32";
                        }
                        //else if (role[i].ToString() != "1" && role[i].ToString() != "2" && role[i].ToString() != "")
                        else
                        {
                            role_check_employee_time_sheet_permission = role_check_for_add_edit_time_sheet;
                            ddlemployee.Items.Clear();
                            ddlemployee.DataSource = null;
                            ddlemployee.DataBind();
                            ddlemployee.Items.Add("Please Select");
                            ddlemployee.SelectedItem.Value = "";
                            dr = null;
                            dtr = null;
                            //ch = true;
                        }
                    }
                    //if(ch == true)
                    if (role_check_employee_time_sheet_permission != "32")
                    {
                        ddlemployee.Items.Clear();
                        ddlemployee.DataSource = null;
                        ddlemployee.DataBind();
                        ddlemployee.Items.Add("Please Select");
                        ddlemployee.SelectedItem.Value = "";
                        dr = null;
                        dtr = null;
                        dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES'");
                        if (dr.Count() > 0)
                        {
                            dtr = dr.CopyToDataTable();
                            ddlemployee.DataSource = dtr;
                            ddlemployee.DataValueField = "EmployeeID";
                            ddlemployee.DataTextField = "EmpName";
                            ddlemployee.DataBind();
                            ddlemployee.SelectedIndex = 0;
                        }
                    }
                }
            }
            else
            {
                ddlemployee.Items.Clear();
                ddlemployee.DataSource = null;
                ddlemployee.DataBind();
                ddlemployee.Items.Add("Please Select");
                ddlemployee.SelectedItem.Value = "";
                dr = null;
                dtr = null;
                dr = dt_Custome_User_Role_Popup_Emp_List.Select("[IsActive]='YES'");
                if (dr.Count() > 0)
                {
                    dtr = dr.CopyToDataTable();
                    ddlemployee.DataSource = dtr;
                    ddlemployee.DataValueField = "EmployeeID";
                    ddlemployee.DataTextField = "EmpName";
                    ddlemployee.DataBind();
                    ddlemployee.SelectedIndex = 0;
                }
            }
        }

        if (Request.QueryString["UserRole"] == "Administrator")
        {
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "ASSB.BusinessID=" + Bid + " and AE.UserType Not in (1,5,12) and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + " and AE.IsActive='YES' and ae.EMPLOYEETYPE IN ('R','P')", "AE.EmployeeID");
        }

        if (Request.QueryString["UserRole"] == "Manager")
        {
            AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
            DataTable dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager_FOR_POPUP(int.Parse(Request.QueryString["MangerId"].ToString()), int.Parse(Bid), int.Parse(Session["MotherOrgid"].ToString()));

            ddlemployee.Items.Clear();
            ddlemployee.Items.Add("Please Select");
            ddlemployee.SelectedItem.Value = "";
            if (dtMangerEmpList.Rows.Count > 0)
            {
                ddlemployee.DataSource = dtMangerEmpList;
                ddlemployee.DataValueField = "EmployeeID";
                ddlemployee.DataTextField = "EmpName";
                ddlemployee.DataBind();
                ddlemployee.SelectedIndex = 0;
            }
        }

        if (Request.QueryString["UserRole"] == "Employee")
        {
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID=AE.EmployeeID) ", "AE.EmployeeID ", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName ", "ASSB.BusinessID=" + Bid + " and AE.UserType=4 and ae.EmployeeID in (select EmployeeId from MappingTable where USERID='" + Session["UserId"].ToString() + "' and BusinessID=" + Bid + ") and AE.IsActive='YES' ", "AE.EmployeeID");
        }

        if (Request.QueryString["UserRole"] == "Accountant")
        {
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID=AE.EmployeeID) ", "AE.EmployeeID ", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName ", "ASSB.BusinessID=" + Bid + " and AE.UserType=12 and ae.EmployeeID in (select EmployeeId from MappingTable where USERID='" + Session["UserId"].ToString() + "' and BusinessID=" + Bid + ") and AE.IsActive='YES' ", "AE.EmployeeID");
        }
    }

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

    protected void grdHalfMonth_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }
    }

    protected void grdHalfMonth_PreRender(object sender, EventArgs e)
    {
        if (grdHalfMonth.Rows.Count > 0)
        {
            grdHalfMonth.UseAccessibleHeader = true;
            grdHalfMonth.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdQterly_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }
    }

    protected void grdQterly_PreRender(object sender, EventArgs e)
    {
        if (grdQterly.Rows.Count > 0)
        {
            grdQterly.UseAccessibleHeader = true;
            grdQterly.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdFourWeek_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }
    }

    protected void grdFourWeek_PreRender(object sender, EventArgs e)
    {
        if (grdFourWeek.Rows.Count > 0)
        {
            grdFourWeek.UseAccessibleHeader = true;
            grdFourWeek.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdFive_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //e.Row.Cells[0].Width = Unit.Percentage(7); // Replace with your desired width for column 1
            //e.Row.Cells[1].Width = Unit.Percentage(7); // Replace with your desired width for column 2
            //e.Row.Cells[2].Width = Unit.Percentage(7); // Replace with your desired width for column 1
            //e.Row.Cells[3].Width = Unit.Percentage(7); // Replace with your desired width for column 2
            //e.Row.Cells[5].Width = Unit.Pixel(50); // Replace with your desired width for column 1
            //e.Row.Cells[6].Width = Unit.Pixel(50); // Replace with your desired width for column 2
            //e.Row.Cells[7].Width = Unit.Pixel(50); // Replace with your desired width for column 1
            //e.Row.Cells[8].Width = Unit.Pixel(50); // Replace with your desired width for column 2

            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
                //e.Row.Cells[i].Wrap = true;
                
            }
        }

        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    // Set the column widths here
        //    e.Row.Cells[0].Width = Unit.Pixel(100); // Replace with your desired width for column 1
        //    e.Row.Cells[1].Width = Unit.Pixel(120); // Replace with your desired width for column 2
        //                                            // ... Repeat this for all columns by adjusting the cell index and width

        //    // Adjust the widths of all cells in the row
        //    foreach (TableCell cell in e.Row.Cells)
        //    {
        //        cell.Wrap = false; // Optionally, prevent wrapping text
        //    }
        //}
    }

    protected void grdFive_PreRender(object sender, EventArgs e)
    {
        if (grdFive.Rows.Count > 0)
        {
            grdFive.UseAccessibleHeader = true;
            grdFive.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }



    }

    protected void GridView1_PreRender(object sender, EventArgs e)
    {
        if (grdsix.Rows.Count > 0)
        {
            grdsix.UseAccessibleHeader = true;
            grdsix.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdHeaderDays_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void grdHeaderDays_PreRender(object sender, EventArgs e)
    {

    }

    protected void grdSeven_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }
    }

    protected void grdSeven_PreRender(object sender, EventArgs e)
    {
        if (grdSeven.Rows.Count > 0)
        {
            grdSeven.UseAccessibleHeader = true;
            grdSeven.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void grdEight_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = e.Row.Cells[i].Text.Replace("@", "<br>");
            }
        }
    }

    protected void grdEight_PreRender(object sender, EventArgs e)
    {
        if (grdEight.Rows.Count > 0)
        {
            grdEight.UseAccessibleHeader = true;
            grdEight.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}