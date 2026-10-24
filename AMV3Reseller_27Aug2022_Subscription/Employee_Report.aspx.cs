using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Report3 : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ReportController RptCtrl = new ReportController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            this.pnlhideandshow.Visible = false;
            //GetBusiness();
            divrange2.Attributes.Add("style", "display:none");
            GVBusiness.Visible = false;
            GVCompare1.Visible = false;
            GVCompare2.Visible = false;
            var empid = Session["EmployeeId"].ToString();
            //kumar commented as business name was showing wrong if business name is edited
            //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");
            //kumar commented because business name was not showing in reseller login
            //Comm.FillDropDownList(ddl_Business, "AssginBussiness", "BusinessID", "dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " UserTypeId=1 AND EmployeeID=" + empid + " and  BusinessID is not null", "BusinessID");

            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                Comm.FillDropDownList(ddl_Business, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID");
            }
            else if (Session["UserType"].ToString() == "Admin")
            {
                Comm.FillDropDownList(ddl_Business, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "CompanyName");
            }
            else
            {
                Comm.FillDropDownList(ddl_Business, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
            }
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
        this.pnlhideandshow.Visible = true;
        submit();
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);

        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        DateTime dtfrom3 = DateTime.Parse(txtFromDate.Text);
        DateTime dtto3 = DateTime.Parse(txtToDate.Text);

        if (dtfrom3 > dtto3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'From date is greater than To date for second date range', 'error');", true);
            return;
        }
        TimeSpan difference2 = dtto3 - dtfrom3;
        int diffday2 = difference2.Days;
        if (diffday2 > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'More than One year date selection not allowed for second date range', 'error');", true);
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
                    //int Status = 2;
                    //dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()),a,Status);
                    dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a, int.Parse(DropDownList_Report_Status.SelectedValue));
                }
            }
            else

                dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }


        GVBusiness.DataSource = dt;
        GVBusiness.DataBind();
        GVBusiness.Visible = true;
        GVCompare1.Visible = false;
        GVCompare2.Visible = false;


        if (hdnvisible.Value == "YES")
        {
            dt = Comm.ReportAcc_AllCustomerWise3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), DropDownList_Report_Status.SelectedValue.ToString());
            GVCompare1.DataSource = dt;
            GVCompare1.DataBind();
            dt = Comm.ReportAcc_AllCustomerWise3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom3), Convert.ToDateTime(dtto3), DropDownList_Report_Status.SelectedValue.ToString());
            GVCompare2.DataSource = dt;
            GVCompare2.DataBind();
            GVBusiness.Visible = false;
            GVCompare1.Visible = true;
            GVCompare2.Visible = true;
        }
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();

        DateTime dtfrom2 = DateTime.Parse(txtFromDate.Text);
        DateTime dtto2 = DateTime.Parse(txtToDate.Text);
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
            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month - 1).ToString() + "," + thisWeekEnd.Day.ToString();
            //txtToDate.Enabled = false;
        }
        if (ddrange.SelectedIndex == 2)
        {
            txtFromDate.Enabled = true;
            DateTime baseDate = DateTime.Today;
            var today = baseDate;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            thisWeekStart = thisWeekStart.AddDays(-20).AddSeconds(-1);
            var thisWeekStart1 = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart1.AddDays(7).AddSeconds(-1);
            //var thisWeekEnd1 = thisWeekStart.AddDays(-28).AddSeconds(-1);

            hdnDate.Value = thisWeekStart.Year.ToString() + "," + (thisWeekStart.Month - 1).ToString() + "," + thisWeekStart.Day.ToString();
            hdnDate1.Value = thisWeekEnd.Year.ToString() + "," + (thisWeekEnd.Month - 1).ToString() + "," + thisWeekEnd.Day.ToString();
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
        DateTime dtto2 = DateTime.Parse(txtToDate2.Text);
        hdndate3.Value = dtfrom2.Year.ToString() + "," + (dtfrom2.Month - 1).ToString() + "," + dtfrom2.Day.ToString();
        hdndate4.Value = dtto2.Year.ToString() + "," + (dtto2.Month - 1).ToString() + "," + dtto2.Day.ToString();

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
            //DataTable dt = Comm.ReportAcc_CustomerWise_Emp_Excels((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value)); ;
            DataTable dt = Comm.ReportAcc_CustomerWise_Emp_Excels((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value), int.Parse(DropDownList_Report_Status.SelectedValue));
            TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        

            string t1 = AnyminuteReportDurations.Format(gross_hours);
            string t2 = AnyminuteReportDurations.Format(break_total);
            string t3 = AnyminuteReportDurations.Format(net);
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
                                         //"<tr><td>" + ddlemployee.SelectedItem.Text + "</td></tr>" +
                                         "<tr><td colspan=2>" + "Date: " + " " + dtfrom.ToString("yyyy/MM/dd") + "-" + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
               "</Table>";

            Response.Write(headerTable);
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
            Response.Write(stringWriter.ToString());
            //headerTable = @"<Table><tr><td></td>" +
            //               "<td></td>" +
            //                "<td></td>" +
            //                 "<td></td>" +
            //                "<td align='center'>" + totalDisplay.ToString() + "</td>" +
            //                "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
            //                "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
            //                "</Table>";

            ////submit();
            //Response.Write(headerTable);

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
            int Status = 2;
            //DataTable dt = Comm.ReportAcc_CustomerWise_EmpApproves((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value),Status); ;
            DataTable dt = Comm.ReportAcc_CustomerWise_EmpApproves((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value), int.Parse(DropDownList_Report_Status.SelectedValue));
            TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        

            string t1 = AnyminuteReportDurations.Format(gross_hours);
            string t2 = AnyminuteReportDurations.Format(break_total);
            string t3 = AnyminuteReportDurations.Format(net);
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
            Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
#pragma warning disable CS0612 // Type or member is obsolete  
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete  
            PdfWriter.GetInstance(pdfDoc, Response.OutputStream);


            /* //Code to Add Header table to PDF document
            string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for Time Sheet For  " + lblEmployeeName.Text + "</td></tr>" +
                                             //"<tr><td>" + ddlemployee.SelectedItem.Text + "</td></tr>" +
                                             "<tr><td colspan=2>" + "Date: " + " " + dtfrom.ToString("yyyy/MM/dd") + "-" + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
                   "</Table>";*/

            //Table ph = new Table();
            //ph = returnPDFHeader(lblEmployeeName.Text);


            Chunk chunk = new Chunk("Report for Time Sheet For  " + lblEmployeeName.Text);


            byte[] file1;
            file1 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
            iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(file1);


            //jpg1.Width = jpg1.Width / 2;
            //jpg1.ScaleAbsolute(10F, 10F);

            jpg1.SetAbsolutePosition(0, 0);


            pdfDoc.Open();
            pdfDoc.Add(jpg1);
            // pdfDoc.Add(chunk);

            /*System.Drawing.Image imgLogo = System.Drawing.Image.FromFile("~\\Assets\\assets\\logo\\logo.jpg");

            System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image()
            {
                ImageUrl = "~\\Assets\\assets\\logo\\logo.jpg"
            };


             Table Tabel1 = new Table();
            TableRow tr = new TableRow();
            TableCell newCell = new TableCell();
            newCell.Controls.Add(img) ;
            tr.Cells.Add(newCell);
            Tabel1.Rows.Add(tr);



           // ImageStamp imageStamp = new ImageStamp(@"~:\Assets\assets\logo\logo.jpg");

            // Set properties of the stamp
            imageStamp.BottomMargin = 10;
            imageStamp.HorizontalAlignment = HorizontalAlignment.Center;
            imageStamp.VerticalAlignment = VerticalAlignment.Bottom;*/

            // Add footer on all pages
            // foreach (Page page in pdfDocument.Pages)
            //{
            // pdfDoc.AddStamp(imageStamp);
            //}








            pdfDoc.Open();
            // pdfDoc.AddHeader(Tabel1);

            //if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
            //{

            //    byte[] file;
            //    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
            //    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
            //    jpg.ScaleToFit(140F, 30F);//Set width and height in float    
            //    pdfDoc.Add(jpg);
            //}
            Chunk chunk1 = new Chunk("Report for Time Sheet For Employee: " + ddlemployee.SelectedItem.ToString() +" for "+ lblEmployeeName.Text);
            Paragraph preface1 = new Paragraph(chunk1);
            //preface.setAlignment(Element.ALIGN_CENTER);
            preface1.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(preface1);
            pdfDoc.Open();

            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
                {
                    byte[] file;
                    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
                    jpg1.ScaleToFit(1F, 1F);//Set width and height in float

                    pdfDoc.Add(jpg);
                    pdfDoc.Add(new Chunk((" " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + chunk)));
                }
            }
            else
            {
                Paragraph preface = new Paragraph(chunk);
                preface.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(preface);
            }
            //Paragraph preface = new Paragraph(chunk);
            //preface.Alignment = Element.ALIGN_CENTER;
            //pdfDoc.Add(preface);



            byte[] file2;
            file2 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(file1);
            pdfDoc.Open();
            pdfDoc.Add(jpg2);
            // pdfDoc.Add(new Paragraph(chunk));


            //chunk = new Chunk(ddlemployee.SelectedItem.Text);
            //pdfDoc.Add(new Paragraph(chunk));

            //chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd") + "-" + dtto.ToString("yyyy/MM/dd"));
            //pdfDoc.Add(new Paragraph("Date: "+" "+chunk));
            chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd"));
            pdfDoc.Add(new Paragraph("Date From: " + chunk));
            chunk = new Chunk(dtto.ToString("yyyy/MM/dd"));
            pdfDoc.Add(new Paragraph("Date To: " + chunk));

            chunk = new Chunk(Environment.NewLine);
            pdfDoc.Add(new Paragraph(chunk));
            PdfPTable pdfTable = new PdfPTable(dt.Columns.Count - 1);
            pdfTable.DefaultCell.Padding = 3;
            pdfTable.WidthPercentage = 100;
            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

            //foreach (DataColumn column in dt.Columns)
            //{
            //    var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            //    var titleChunk = new Chunk(column.ColumnName.ToString(), redListTextFont);
            //    PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            //    cellq.Border = Rectangle.NO_BORDER;
            //    // cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            //    pdfTable.AddCell(cellq);


            //}
            for (int c = 0; c < dt.Columns.Count - 1; c++)
            {
                var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont);
                PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
                cellq.Border = Rectangle.BOX;
                cellq.HorizontalAlignment = Element.ALIGN_CENTER;
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

                for (var item = 0; item < row1.ItemArray.Length - 1; item++)
                {
                    string val = row1.ItemArray[item].ToString();
                    if (val.ToString() == "0:0")
                    { val = "00:00"; }
                    PdfPCell cellitem = new PdfPCell(new Phrase(val.ToString()));
                    cellitem.Border = Rectangle.BOX;
                    cellitem.HorizontalAlignment = Element.ALIGN_CENTER;

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

            //foreach (DataRow row1 in dt.Rows)
            //{

            //    //foreach (itema in row.ItemArray)
            //    //{
            //    //    pdfTable.AddCell(cell.Text.ToString());
            //    //}



            //    foreach (var item in row1.ItemArray)
            //    {
            //        PdfPCell cellitem = new PdfPCell(new Phrase(item.ToString()));
            //        cellitem.Border = Rectangle.NO_BORDER;

            //        if (i % 2 != 0)
            //        {
            //            cellitem.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //        }
            //        else
            //        {
            //            cellitem.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;//EFF3FB
            //        }
            //        pdfTable.AddCell(cellitem);

            //    }
            //    i = i + 1;
            //}
            PdfPCell cell = new PdfPCell(new Phrase("Totals"));
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);

            cell = new PdfPCell(new Phrase(totalDisplay.ToString()));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalDisplay1.ToString()));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(totalDisplay2.ToString()));
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.BOX;
            pdfTable.AddCell(cell);

            // pdfDoc.Add(pdfTable);

            //htmlparser.Parse(sr);



            //*******************




            pdfDoc.Add(pdfTable);

            //htmlparser.Parse(sr);
            chunk = new Chunk(" \n");
            pdfDoc.Add(new Paragraph(chunk));
            chunk = new Chunk(" \n");
            pdfDoc.Add(new Paragraph(chunk));

            Paragraph portfolioText = new Paragraph("Powered By AnyMinute.Net", new Font(Font.FontFamily.HELVETICA, 15, Font.NORMAL, iTextSharp.text.BaseColor.BLUE));
            portfolioText.Alignment = Element.ALIGN_CENTER;
            portfolioText.SetLeading(12.1f, 12.1f);
            portfolioText.IndentationLeft = 90;


            Anchor portAnch = new Anchor(portfolioText);
            portAnch.Reference = "https://anyminute.net/";
            //pdfDoc.Add(portfolioText);
            pdfDoc.Add(portAnch);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            GvExport1.AllowPaging = true;
            GvExport1.DataBind();
        }

        else if (e.CommandName == "CSV")
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
            //DataTable dt = Comm.ReportAcc_CustomerWise_Emp_Excels((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value)); ;
            DataTable dt = Comm.ReportAcc_CustomerWise_Emp_Excels((hdncomid.Value), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hfdEmpid.Value), int.Parse(DropDownList_Report_Status.SelectedValue));
            TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        

            string t1 = AnyminuteReportDurations.Format(gross_hours);
            string t2 = AnyminuteReportDurations.Format(break_total);
            string t3 = AnyminuteReportDurations.Format(net);
            var totalDisplay = t1;
            var totalDisplay1 = t2;
            var totalDisplay2 = t3;
            GvExport1.DataSource = dt;
            GvExport1.DataBind();

            Response.ClearContent();
            string filename = ddlemployee.SelectedItem.Text + dtfrom.ToString("yyyy/MM/dd") + "_" + dtto.ToString("yyyy/MM/dd");

            //filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".csv");

            Response.Charset = "";
            Response.ContentType = "text/csv";

            //To Export all pages.
            GvExport1.AllowPaging = false;
            //this.GvExport();

            StringBuilder sb = new StringBuilder();
            sb.Append(ddlemployee.SelectedItem.Text);
            sb.Append("\r\n");
            sb.Append("Date:" + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
            sb.Append("\r\n");
            sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);
            sb.Append("\r\n");
            foreach (TableCell cell in GvExport1.HeaderRow.Cells)
            {
                //Append data with separator.
                sb.Append(cell.Text + ',');
            }
            //Append new line character.
            sb.Append("\r\n");

            foreach (GridViewRow row1 in GvExport1.Rows)
            {
                foreach (TableCell cell in row1.Cells)
                {
                    //Append data with separator.
                    sb.Append(cell.Text + ',');
                }
                //Append new line character.
                sb.Append("\r\n");

            }
            sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);


            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();


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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        if (GVBusiness.Rows.Count == 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Pleasce click on update report ', 'error');", true);
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

                //dt = Comm.ReportAcc_Report3_Excel(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a);
                dt = Comm.ReportAcc_Report3_Excel(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a, int.Parse(DropDownList_Report_Status.SelectedValue));

            }
        }
        else
        {
            dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }

        TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        GvExport.DataSource = dt;
            GvExport.DataBind();

        string t1 = AnyminuteReportDurations.Format(gross_hours);
        string t2 = AnyminuteReportDurations.Format(break_total);
        string t3 = AnyminuteReportDurations.Format(net);
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

        string headerTable = @"<Table><tr><TD><link rel=" + "icon" + " href=\"Assets/assets/img/logo.jpg\" type=\"image/x-icon\"></TD><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + ddlemployee.SelectedItem.Text.ToString() + "</td></tr>" +
                                  //"<tr><td>" + ddlemployee.SelectedItem.Text.ToString() + "</td></tr>" +
                                  "<tr><td></TD><td colspan=2>" + "Date: " + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
        "</Table>";
        Response.Write(headerTable);
        headerTable = @"<Table><tr><td></td>" +
                           "<td></td>" +
                           "<td></td>" +
                            "<td align='center'>" + totalDisplay.ToString() + "</td>" +
                            "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
                            "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
                            "</Table>";
        Response.Write(headerTable);
        Response.Write(stringWriter.ToString());
        //headerTable = @"<Table><tr><td></td>" +
        //               "<td></td>" +
        //               "<td></td>" +
        //                "<td align='center'>" + totalDisplay.ToString() + "</td>" +
        //                "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
        //                "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
        //                "</Table>";
        //Response.Write(headerTable);


        Response.End();
    }

    protected void lnkDirectPdf_Click(object sender, EventArgs e)
    {
        if (ddrange.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        if (GVBusiness.Rows.Count == 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Pleasce click on update report ', 'error');", true);
            return;
        }
        //ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(ddlemployee.SelectedValue.ToString()) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','"+ Convert.ToString(ddlemployee.SelectedItem.Text.ToString()) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
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
                //int Status = 2;
                //dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a,Status);
                dt = Comm.ReportAcc_Report3(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a, int.Parse(DropDownList_Report_Status.SelectedValue));
            }
        }
        else
        {
            dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }

        TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        GvExport.DataSource = dt;
            GvExport.DataBind();

        string t1 = AnyminuteReportDurations.Format(gross_hours);
        string t2 = AnyminuteReportDurations.Format(break_total);
        string t3 = AnyminuteReportDurations.Format(net);
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
        Document pdfDoc = new Document(PageSize.A2, 10f, 10f, 10f, 0f);
#pragma warning disable CS0612 // Type or member is obsolete  
        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete  
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
        Chunk chunk = new Chunk("Report for " + ddlemployee.SelectedItem.Text);
        pdfDoc.Open();

        for (int k = 0; k < dt.Rows.Count; k++)
        {
            if (dt.Rows.Count > 0)
            {

                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[k]["CareOf"])) && ddlemployee.SelectedValue == Convert.ToString(dt.Rows[k]["EmployeeID"]))
                {
                    byte[] file;
                    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[k]["CareOf"].ToString()));//ImagePath    
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
                    jpg.ScaleToFit(140F, 30F);//Set width and height in float    
                    pdfDoc.Add(jpg);

                    pdfDoc.Add(new Chunk(("            " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + chunk)));

                }
            }

        }
        Paragraph preface = new Paragraph(chunk);
        preface.Alignment = Element.ALIGN_CENTER;
        pdfDoc.Add(preface);

        //chunk = new Chunk("\n");
        //pdfDoc.Add(new Paragraph(chunk));


        byte[] file1;
        file1 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
        iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(file1);
        // pdfDoc.Open();
        pdfDoc.Add(jpg1);


        //Paragraph preface = new Paragraph(chunk);
        //// preface.setAlignment(Element.ALIGN_CENTER);
        //preface.Alignment = Element.ALIGN_CENTER;
        //pdfDoc.Add(preface);

        //chunk = new Chunk(ddlemployee.SelectedItem.Text);
        //pdfDoc.Add(new Paragraph(chunk));

        chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
        pdfDoc.Add(new Paragraph("Date: " + " " + chunk));

        //chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd"));
        //pdfDoc.Add(new Paragraph("Date From: " + chunk));
        //chunk = new Chunk(dtto.ToString("yyyy/MM/dd"));
        //pdfDoc.Add(new Paragraph("Date To: " + chunk));

        chunk = new Chunk(Environment.NewLine);
        pdfDoc.Add(new Paragraph(" "));
        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count - 1);
        pdfTable.DefaultCell.Padding = 3;
        pdfTable.WidthPercentage = 100;
        pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

        //foreach (DataColumn column in dt.Columns)
        //{
        //    var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
        //    var titleChunk = new Chunk(column.ColumnName.ToString(), redListTextFont);
        //    PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
        //    cellq.Border = Rectangle.NO_BORDER;
        //    //cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

        //    pdfTable.AddCell(cellq);


        //}
        for (int c = 0; c < dt.Columns.Count - 1; c++)
        {
            var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont);
            PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            cellq.Border = Rectangle.BOX;
            cellq.HorizontalAlignment = Element.ALIGN_CENTER;
            // cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            pdfTable.AddCell(cellq);


        }

        int i = 1;
        foreach (DataRow row in dt.Rows)
        {

            //foreach (itema in row.ItemArray)
            //{
            //    pdfTable.AddCell(cell.Text.ToString());
            //}

            for (var item = 0; item < row.ItemArray.Length - 1; item++)
            {
                //for (int j = 0; j < dt.Rows.Count; j++)
                //{
                string val = row.ItemArray[item].ToString();
                if (val.ToString() == "0:0")
                { val = "00:00"; }

                //string secondsvalues = "00";
                //var split = Convert.ToString(dt.Rows[j][4]).Split(':');
                //if (split[1] == "0")
                //{
                //    split[1] = "00";
                //}




                PdfPCell cellitem = new PdfPCell(new Phrase(val.ToString()));

                cellitem.Border = Rectangle.BOX;
                cellitem.HorizontalAlignment = Element.ALIGN_CENTER;

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
        //foreach (DataRow row in dt.Rows)
        //{

        //    //foreach (itema in row.ItemArray)
        //    //{
        //    //    pdfTable.AddCell(cell.Text.ToString());
        //    //}

        //    foreach (var item in row.ItemArray)
        //    {
        //        PdfPCell cellitem = new PdfPCell(new Phrase(item.ToString()));
        //        cellitem.Border = Rectangle.NO_BORDER;

        //        if (i % 2 != 0)
        //        {
        //            cellitem.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
        //        }
        //        else
        //        {
        //            cellitem.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
        //        }
        //        pdfTable.AddCell(cellitem);

        //    }
        //    i = i + 1;
        //}
        PdfPCell cell = new PdfPCell(new Phrase("Totals"));
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(""));
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(""));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay.ToString()));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay1.ToString()));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay2.ToString()));
        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);


        pdfDoc.Add(pdfTable);

        //htmlparser.Parse(sr);
        chunk = new Chunk(" \n");
        pdfDoc.Add(new Paragraph(chunk));
        chunk = new Chunk(" \n");
        pdfDoc.Add(new Paragraph(chunk));

        Paragraph portfolioText = new Paragraph("Powered By AnyMinute.Net", new Font(Font.FontFamily.HELVETICA, 15, Font.NORMAL, iTextSharp.text.BaseColor.BLUE));
        portfolioText.Alignment = Element.ALIGN_CENTER;
        portfolioText.SetLeading(12.1f, 12.1f);
        portfolioText.IndentationLeft = 90;


        Anchor portAnch = new Anchor(portfolioText);
        portAnch.Reference = "https://anyminute.net/";
        //pdfDoc.Add(portfolioText);
        pdfDoc.Add(portAnch);

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
            //above is commented because administrator and accountant will not be shown in employee list as they are not seen in timesheet screen
            Comm.FillDropDownList(ddlemployee, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID", "AE.EmployeeID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", " ASSB.BusinessID=" + bussid + " and Assb.UserTypeId NOT IN (1,5,12) and ae.EmployeeID<>" + empid + "", "AE.EmployeeID ");
            DataTable MotherOrgiD = Comm.GetAllDataFromTable("AddBusiness", "MotherOrgId", "BusinessID=" + bussid + "", "");
            //string MotherOrgID = MotherOrgiD.Rows[0]["MotherOrgId"].ToString();
        }
        if (ddl_Business.SelectedIndex == 0)
        {
            ddlemployee.Items.Clear();
            ddlemployee.Items.Add("Please Select");
            ddlemployee.SelectedItem.Value = "";
            ddlemployee.DataBind();
        }
    }

    protected void lnkCsvFile_Click(object sender, EventArgs e)
    {
        if (ddrange.SelectedIndex == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
        DateTime dtto = DateTime.Parse(txtToDate.Text);
        if (dtfrom > dtto)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'From date is greater than To date', 'error');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'More than One year date selection not allowed', 'error');", true);
            return;
        }
        if (GVBusiness.Rows.Count == 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Pleasce click on update report ', 'error');", true);
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

                dt = Comm.ReportAcc_Report3_Excel(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid"].ToString()), a, int.Parse(DropDownList_Report_Status.SelectedValue));

            }
        }
        else
        {
            dt = Comm.ReportAcc_Report3_Acc(ddlemployee.SelectedValue.ToString(), Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(Session["MotherOrgid_acc"].ToString()), a);
        }

        TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
        TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
        TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
        AnyminuteReportDurations.FormatCells(dt);
        GvExport.DataSource = dt;
            GvExport.DataBind();

        string t1 = AnyminuteReportDurations.Format(gross_hours);
        string t2 = AnyminuteReportDurations.Format(break_total);
        string t3 = AnyminuteReportDurations.Format(net);
        var totalDisplay = t1;
        var totalDisplay1 = t2;
        var totalDisplay2 = t3;
        string filename = ddlemployee.SelectedItem.Text.ToString() + "_" + dtfrom.ToString() + "_" + dtto.ToString();
        //filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

        Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".csv");

        Response.Charset = "";
        Response.ContentType = "text/csv";

        //To Export all pages.
        GvExport.AllowPaging = false;
        //this.GvExport();

        StringBuilder sb = new StringBuilder();
        sb.Append(ddlemployee.SelectedItem.Text);
        sb.Append("\r\n");
        sb.Append("Date:" + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
        sb.Append("\r\n");
        sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);
        sb.Append("\r\n");
        if(GvExport.HeaderRow!=null) foreach (TableCell cell in GvExport.HeaderRow.Cells)
        {
            //Append data with separator.
            sb.Append(cell.Text + ',');
        }
        //Append new line character.
        sb.Append("\r\n");

        foreach (GridViewRow row in GvExport.Rows)
        {
            foreach (TableCell cell in row.Cells)
            {
                //Append data with separator.
                // sb.Append(cell.Text + ',');
                sb.Append(cell.Text + "\t" + "\t" + "\t" + ','.ToString().PadLeft(2));
            }
            //Append new line character.
            sb.Append("\r\n");

        }


        sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);

        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();

    }

    public Table returnPDFHeader(string employeeName)
    {

        /* System.Drawing.Image imgLogo = System.Drawing.Image.FromFile("~\\Assets\\assets\\logo\\logo.jpg", true);

         PdfPCell pdfcellImage = new PdfPCell(imgLogo, true);
         pdfcellImage.FixedHeight = 40f;
         pdfcellImage.HorizontalAlignment = Element.ALIGN_CENTER;
         pdfcellImage.VerticalAlignment = Element.ALIGN_CENTER;
         pdfcellImage.Border = Rectangle.NO_BORDER;
         pdfcellImage.Border = Rectangle.NO_BORDER;
         PdfPTable pdftblImage = new PdfPTable(1);
         pdftblImage.AddCell(pdfcellImage);

         PdfGridCellContentList lst = new PdfGridCellContentList();
         PdfGridCellContent textAndStyle = new PdfGridCellContent();
         textAndStyle.Image = PdfImage.FromFile("..\\..\\image1.jpg");*/

        Table table = new Table();
        TableRow tr = new TableRow();
        TableCell tc = new TableCell();
        tc.Text = "Report for Time Sheet For:  " + employeeName;
        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image()
        {
            ImageUrl = "~/Assets/assets/logo/logo.jpg"
        };

        tr.Cells.Add(new TableCell()
        {
            Controls = { img }
        });

        tr.Cells.Add(tc);


        table.Rows.Add(tr);


        //Chunk chunk = new Chunk("Report for Time Sheet For  " + lblEmployeeName.Text);
        //Paragraph phHeader = new Paragraph();

        //phHeader.Add(table);
        //phHeader.Add(chunk);
        return table;
        // return PDFHeaderString;
    }

}