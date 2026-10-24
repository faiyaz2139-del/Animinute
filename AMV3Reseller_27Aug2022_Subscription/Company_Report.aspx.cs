using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using System.Globalization;

using System.Text;
//using CsvHelper;

public partial class customerWiseReport : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    CommanClass comm = new CommanClass();
    ReportController RptCtrl = new ReportController();
    protected void Page_Load(object sender, EventArgs e)
    {
        
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        // Employee reports must not run company-wide queries or exports.
        if (Convert.ToString(Session["UserTypeid"]) == "4" && Session["UserType_acc"] == null)
        {
            Response.Redirect("My_Time_Report.aspx", true);
            return;
        }
        if (!IsPostBack)
        {
            this.pnlhideandshow.Visible = false;
            GetBusiness();
            divrange2.Attributes.Add("style", "display:none");
            GVBusiness.Visible = false;
            GV.Visible = false;
            GVCompare2.Visible = false;
            txtFromDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
            txtFromDate2.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");
            txtToDate2.Text = DateTime.Now.Date.ToString("yyyy-MM-dd");

            //GetDataforEditBusinessDetails(int.Parse(Request.QueryString["BusinessIDEdit"]));
        }
    }
    //Amit on 28_01_2021 start
    public void GetBusiness()
    {
        //DataTable dt = new DataTable();
        //ddlBusiness.Items.Clear();
        //dt = Comm.GetAllDataFromTable("AddBusiness", "BusinessID,CompanyName", "MotherOrgId=" + Session["MotherOrgid"].ToString() + "", "BusinessID");
        //ddlBusiness.DataSource = dt;
        //ddlBusiness.DataTextField = "CompanyName";
        //ddlBusiness.DataValueField = "BusinessID";
        //ddlBusiness.DataBind();

        //DataTable dt = new DataTable();
        //ddlBusiness.Items.Clear();
        //var UserType_acc = Session["UserType_acc"];
        //if (UserType_acc == null)
        //{
        //    if (Session["UserType"].ToString() == "BUSINESS OWNER")
        //    {
        //        Comm.FillDropDownList_message(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID", "");
        //    }
        //    else if (Session["UserType"].ToString() == "Admin")
        //    {
        //        Comm.FillDropDownList_message(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "CompanyName", "BusinessID");
        //    }
        //    else
        //    {
        //        Comm.FillDropDownList(ddlBusiness, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
        //    }
        //}
        //else
        //{
        //    Comm.FillDropDownList_message(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "CompanyName", "Company");
        //    // Comm.FillDropDownList_message(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "CompanyName", "Company");
        //}

        ddlBusiness.Items.Clear();
        var UserType_acc = Session["UserType_acc"];
        if (UserType_acc == null)
        {
            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                Comm.FillDropDownList(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID");
            }
            else if (Session["UserType"].ToString() == "Admin")
            {
                Comm.FillDropDownList(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "CompanyName");
            }
            else
            {
                Comm.FillDropDownList(ddlBusiness, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
            }
        }
        else
        {
            Comm.FillDropDownList(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "CompanyName");
            // Comm.FillDropDownList_message(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "CompanyName", "Company");
        }
    }
    //Amit 28_01_2021 End

    protected void GVBusiness_PreRender(object sender, EventArgs e)
    {
        if (GVBusiness.Rows.Count > 0)
        {
            GVBusiness.UseAccessibleHeader = true;
            GVBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    protected void GV_PreRender(object sender, EventArgs e)
    {
        if (GV.Rows.Count > 0)
        {
            GV.UseAccessibleHeader = true;
            GV.HeaderRow.TableSection = TableRowSection.TableHeader;
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
    protected void btnshow_Click(object sender, EventArgs e)
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'From date is greater than To date', 'warning');", true);
            return;
        }
        TimeSpan difference = dtto - dtfrom;
        int diffday = difference.Days;
        if (diffday > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'More than One year date selection not allowed', 'warning');", true);
            return;
        }
        DateTime dtfrom3 = DateTime.Parse(txtFromDate.Text);
        DateTime dtto3 = DateTime.Parse(txtToDate.Text);

        if (dtfrom3 > dtto3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'From date is greater than To date for second date range', 'warning');", true);
            return;
        }
        TimeSpan difference2 = dtto3 - dtfrom3;
        int diffday2 = difference2.Days;
        if (diffday2 > 366)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'More than One year date selection not allowed for second date range', 'warning');", true);
            return;
        }
        DataTable dt = new DataTable();
        if (hdnvisible.Value == "NO")
        {
            //int Status = 2;
            //dt = Comm.ReportAcc_CustomerWise(hfSelected.Value, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Status);
            dt = Comm.ReportAcc_CustomerWise(hfSelected.Value, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(DropDownList_Report_Status.SelectedValue));
            GVBusiness.DataSource = dt;
            GVBusiness.DataBind();

            GVBusiness.Visible = true;
            GV.Visible = false;
            GVCompare2.Visible = false;
        }
        else if (hdnvisible.Value == "YES")
        {
            dt = Comm.ReportAcc_AllCustomerWise(hfSelected.Value, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), DropDownList_Report_Status.SelectedValue.ToString());
            GV.DataSource = dt;
            GV.DataBind();
            dt = Comm.ReportAcc_AllCustomerWise(hfSelected.Value, Convert.ToDateTime(dtfrom3), Convert.ToDateTime(dtto3), DropDownList_Report_Status.SelectedValue.ToString());
            GVCompare2.DataSource = dt;
            GVCompare2.DataBind();
            GVBusiness.Visible = false;
            GV.Visible = true;
            GVCompare2.Visible = true;
        }
        hdnDate.Value = dtfrom.Year.ToString() + "," + (dtfrom.Month - 1).ToString() + "," + dtfrom.Day.ToString();
        hdnDate1.Value = dtto.Year.ToString() + "," + (dtto.Month - 1).ToString() + "," + dtto.Day.ToString();

        DateTime dtfrom2 = DateTime.Parse(txtFromDate2.Text);
        DateTime dtto2 = DateTime.Parse(txtToDate2.Text);
        hdndate3.Value = dtfrom2.Year.ToString() + "," + (dtfrom2.Month - 1).ToString() + "," + dtfrom2.Day.ToString();
        hdndate4.Value = dtto2.Year.ToString() + "," + (dtto2.Month - 1).ToString() + "," + dtto2.Day.ToString();

        //hdndone.Value = "Yes";
        this.pnlhideandshow.Visible = true;
    }


    public override void VerifyRenderingInServerForm(Control control)
    {
    }

    protected void GVBusiness_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVBusiness.PageIndex = e.NewPageIndex;
        btnshow_Click(sender, e);
    }
    protected void GV_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GV.PageIndex = e.NewPageIndex;
        btnshow_Click(sender, e);
    }
    protected void GVCompare2_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVCompare2.PageIndex = e.NewPageIndex;
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
        DataTable dt = new DataTable();
        if (ddapproval.SelectedIndex == 1)
        {
            HiddenField hdnempid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hdnempid");
            Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName");
            dt = Comm.ReportAcc_CustomerWise_Emp(hfSelected.Value, Convert.ToDateTime(hdnfrmdt.Value), Convert.ToDateTime(hdntodt.Value), Convert.ToInt32(hdnempid.Value));
            GvExport1.DataSource = dt;
            GvExport1.DataBind();
            Response.ClearContent();
            string filename = lblEmployeeName.Text + "_" + hdnfrmdt.Value + "_" + hdntodt.Value;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GvExport1.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GvExport1.RenderControl(htw);

            string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Custom Report for Employees working Under " + ddlBusiness.SelectedItem.Text + "</td></tr>" +
                                        "<tr><td>" + lblEmployeeName.Text + "</td></tr>" +
                                         "<tr><td colspan=2>" + hdnfrmdt.Value.ToString() + "-" + hdntodt.Value.ToString() + "</td></tr>" +
               "</Table>";
            Response.Write(headerTable);
            Response.Write(stringWriter.ToString());
            Response.End();
        }
        if (ddapproval.SelectedIndex == 2)
        {
            HiddenField hdnempid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hdnempid");
            Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName");
            dt = Comm.ReportAcc_CustomerWise_Emp(hfSelected.Value, Convert.ToDateTime(hdnfrmdt.Value), Convert.ToDateTime(hdntodt.Value), Convert.ToInt32(hdnempid.Value));
            GvExport1.DataSource = dt;
            GvExport1.DataBind();
            string filename = lblEmployeeName.Text + "_" + hdnfrmdt.Value + "_" + hdntodt.Value;
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
            Chunk chunk = new Chunk("Custom Report for Employees working Under " + ddlBusiness.SelectedItem.Text);
            Paragraph preface = new Paragraph(chunk);
            // preface.setAlignment(Element.ALIGN_CENTER);
            preface.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(preface);

            chunk = new Chunk(lblEmployeeName.Text);
            pdfDoc.Add(new Paragraph(chunk));

            chunk = new Chunk(hdnfrmdt.Value.ToString() + "-" + hdntodt.Value.ToString());
            pdfDoc.Add(new Paragraph(chunk));

            chunk = new Chunk(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            pdfDoc.Add(new Paragraph(" "));
            htmlparser.Parse(sr);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            GvExport1.AllowPaging = true;
            GvExport1.DataBind();
        }
    }

  
    public static string ConvertDataTableToHTML(DataTable dt)
    {
        string html = "<table style='border: 1px solid black;'>";
        //add header row
        html += "<tr style='border: 1px solid black;'>";
        for (int i = 0; i < dt.Columns.Count; i++)
            html += "<td style='border: 1px solid black;'>" + dt.Columns[i].ColumnName + "</td>";
        html += "</tr>";
        //add rows
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            html += "<tr style='border: 1px solid black;'>";
            for (int j = 0; j < dt.Columns.Count; j++)
                html += "<td style='border: 1px solid black;'>" + dt.Rows[i][j].ToString() + "</td>";
            html += "</tr>";
        }
        html += "</table>";
        return html;
    }
    protected void GVBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Excel")
        {
            int index;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out index) ||
                index < 0 || index >= GVBusiness.Rows.Count) return;
            GridViewRow row = GVBusiness.Rows[index];
            int employeeId;
            if (!int.TryParse(((HiddenField)row.FindControl("hdnempid")).Value, out employeeId)) return;
            ExportCompanyExcel(employeeId, ((Label)row.FindControl("lblEmployeeName")).Text);
        }

        else if (e.CommandName == "PDF")
        {

            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GVBusiness.Rows[index];

            DataTable dtReport = new DataTable();
            DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
            DateTime dtto = DateTime.Parse(txtToDate.Text);

            DropDownList ddstatus = (DropDownList)GVBusiness.Rows[index].FindControl("ddlExportType1") as DropDownList;
            HiddenField hdnempid = (HiddenField)GVBusiness.Rows[index].FindControl("hdnempid") as HiddenField;
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName") as Label;

            //DataTable dt = new DataTable();
            //dt = Comm.ReportAcc_CustomerWise_Emp(hfSelected.Value, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hdnempid.Value));
            //ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + ddlBusiness.SelectedItem.Text + "','" + ddstatus.SelectedIndex + "','" + Convert.ToString(hdnempid.Value) + "','" + lblEmployeeName.Text + "', 'Child');", true);

            DataTable dt = new DataTable();
            //int Status = 2;
            dt = Comm.ReportAcc_CustomerWise_Emp(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hdnempid.Value));
            //dt = Comm.ReportAcc_CustomerWise_EmpApprove(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hdnempid.Value), int.Parse(DropDownList_Report_Status.SelectedValue));
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

            string filename = "Employee_Report_" + Convert.ToInt32(hdnempid.Value) + "_" + dtfrom.ToString("yyyyMMdd") + "_" + dtto.ToString("yyyyMMdd");
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
            pdfDoc.Open();

            //if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
            //{
            //    byte[] file;
            //    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
            //    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
            //    jpg.ScaleToFit(140F, 30F);//Set width and height in float    
            //    pdfDoc.Add(jpg);
            //}

            Chunk chunk = new Chunk("Report for Employees working for " + ddlBusiness.SelectedItem.Text);
            Paragraph preface1 = new Paragraph(chunk);
            preface1.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(preface1);
           // Chunk chunk1 = new Chunk(" ");
            pdfDoc.Open();
            if (dt.Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
                {
                    byte[] file;
                    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
                    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
                    jpg.ScaleToFit(30F, 30F);//Set width and height in float    
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
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(file2);
            pdfDoc.Open();
            pdfDoc.Add(jpg2);


           // chunk1 = new Chunk("\n");
            //    pdfDoc.Add((chunk1));


                //chunk = new Chunk(lblEmployeeName.Text);
                //pdfDoc.Add(new Paragraph(chunk));

                chunk = new Chunk(Convert.ToDateTime(dtfrom).ToString("yyyy/MM/dd") + " to " + Convert.ToDateTime(dtto).ToString("yyyy/MM/dd"));
                pdfDoc.Add(new Paragraph("Date: " + " " + chunk));

            chunk = new Chunk(Environment.NewLine);// + Environment.NewLine + Environment.NewLine + Environment.NewLine);
                pdfDoc.Add(chunk);
            

                PdfPTable pdfTable = new PdfPTable(dt.Columns.Count - 1);
                pdfTable.DefaultCell.Padding = 3;
                pdfTable.WidthPercentage = 100;
                pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;
            
            for (int c = 0; c < dt.Columns.Count - 1; c++)
            {
                var redListTextFont1 = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont1);
                PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
                cellq.Border = Rectangle.BOX;
                cellq.HorizontalAlignment = Element.ALIGN_CENTER;
                // cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

                pdfTable.AddCell(cellq);


            }

            //foreach (DataColumn column in dt.Columns)
            //{
            //    var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            //    var titleChunk = new Chunk(column.ColumnName.ToString(), redListTextFont);
            //    PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            //    cellq.Border = Rectangle.NO_BORDER;
            //    //cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            //    pdfTable.AddCell(cellq);


            //}
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
            cell = new PdfPCell(new Phrase(""));
            cell.Border = Rectangle.NO_BORDER;
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
            //*********************
            GvExport1.AllowPaging = true;
            GvExport1.DataBind();
        }

       else if (e.CommandName == "CSV")
        {

            int index = Convert.ToInt32(e.CommandArgument);
            GridViewRow row = GVBusiness.Rows[index];

            DataTable dtReport = new DataTable();
            DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
            DateTime dtto = DateTime.Parse(txtToDate.Text);

            DropDownList ddstatus = (DropDownList)GVBusiness.Rows[index].FindControl("ddlExportType1") as DropDownList;
            HiddenField hdnempid = (HiddenField)GVBusiness.Rows[index].FindControl("hdnempid") as HiddenField;
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName") as Label;

            //DataTable dt = new DataTable();
            //dt = Comm.ReportAcc_CustomerWise_Emp(hfSelected.Value, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hdnempid.Value));
            //ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + ddlBusiness.SelectedItem.Text + "','" + ddstatus.SelectedIndex + "','" + Convert.ToString(hdnempid.Value) + "','" + lblEmployeeName.Text + "', 'Child');", true);

            DataTable dt = new DataTable();

            dt = Comm.ReportAcc_CustomerWise_Emp_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(hdnempid.Value), int.Parse(DropDownList_Report_Status.SelectedValue));
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

            string filename = "Company_Report_" + dtfrom.ToString("yyyyMMdd") + "_" + dtto.ToString("yyyyMMdd");

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".csv");

            Response.Charset = "";
            Response.ContentType = "text/csv";

            //To Export all pages.
            GvExport1.AllowPaging = false;
            //this.GvExport();

            StringBuilder sb = new StringBuilder();
            sb.Append(ddlBusiness.SelectedItem.Text);
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
           // sb.Append("\r\n");
            sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);


            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();

            //Response.ClearContent();
            //Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            //Response.ContentType = "application/excel";
            //StringWriter stringWriter = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            //// GvExport1.HeaderRow.Style.Add("background-color", "#FFFFFF");
            //GvExport1.RenderControl(htw);

            //string headerTable = @"<Table><tr><TD align='center' colspan='8' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for Employees working Under " + ddlBusiness.SelectedItem.Text + "</td></tr>" +
            //                             //"<tr><td style='font-size:12.0pt;text-weight:bold;'>" + lblEmployeeName.Text + "</td></tr>" +
            //                             "<tr><td colspan=2>" + "Date: " + " " + Convert.ToDateTime(dtfrom).ToString("yyyy/MM/dd") + " to " + Convert.ToDateTime(dtto).ToString("yyyy/MM/dd") + "</td></tr>" +
            //   "</Table>";
            //Response.Write(headerTable);
            //Response.Write(stringWriter.ToString());
            //headerTable = @"<Table><tr><td></td>" +
            //               "<td></td>" +
            //                "<td></td>" +
            //                 "<td></td>" +
            //                "<td align='center'>" + totalDisplay.ToString() + "</td>" +
            //                "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
            //                "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
            //                "</Table>";
            //Response.Write(headerTable);
            //Response.End();

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
        GV.Visible = false;
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
        GV.Visible = false;
        GVCompare2.Visible = false;
        lnkbtn.Visible = true;
        hdnvisible.Value = "NO";
    }

    protected void GV_RowCommand(object sender, GridViewCommandEventArgs e)
    {

    }



    //private void ExportGridToCSV()
    //{
    //    Bindgrid();
    //    Response.Clear();
    //    Response.Buffer = true;
    //    Response.AddHeader("content-disposition", "attachment;filename=Vithal_Wadje.csv");
    //    Response.Charset = "";
    //    Response.ContentType = "application/text";
    //    GridView1.AllowPaging = false;
    //    GridView1.DataBind();
    //    StringBuilder columnbind = new StringBuilder();
    //    for (int k = 0; k < GridView1.Columns.Count; k++)
    //    {
    //        columnbind.Append(GridView1.Columns[k].HeaderText + ',');
    //    }
    //    columnbind.Append("\r\n");
    //    for (int i = 0; i < GridView1.Rows.Count; i++)
    //    {
    //        for (int k = 0; k < GridView1.Columns.Count; k++)
    //        {
    //            columnbind.Append(GridView1.Rows[i].Cells[k].Text + ',');
    //        }
    //        columnbind.Append("\r\n");
    //    }
    //    Response.Output.Write(columnbind.ToString());
    //    Response.Flush();
    //    Response.End();
    //}

    //protected void lnkExcel_Click(object sender, EventArgs e)
    //{
    //    Response.Clear();
    //    Response.Buffer = true;
    //    Response.ClearContent();
    //    Response.ClearHeaders();
    //    Response.Charset = "";

    //    if (ddrange.SelectedIndex == 0)
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'error');", true);
    //        return;
    //    }

    //    DateTime dtfrom = DateTime.Parse(txtFromDate.Text);
    //    DateTime dtto = DateTime.Parse(txtToDate.Text);
    //    if (dtfrom > dtto)
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'From date is greater than To date', 'error');", true);
    //        return;
    //    }
    //    TimeSpan difference = dtto - dtfrom;
    //    int diffday = difference.Days;
    //    if (diffday > 366)
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'More than One year date selection not allowed', 'error');", true);
    //        return;
    //    }
    //    if (GVBusiness.Rows.Count == 0)
    //    {
    //        ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'No Record Found', 'error');", true);
    //        return;
    //    }
    //    //ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + Convert.ToString(ddlBusiness.SelectedItem.Text) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);

    //    string filename = "";
    //    DataTable dt = Comm.ReportAcc_CustomerWise_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto));
    //    GvExport.DataSource = dt;
    //    GvExport.DataBind();

    //    TimeSpan gross_hours = AnyminuteReportDurations.Sum(dt, "gross");
    // TimeSpan break_total = AnyminuteReportDurations.Sum(dt, "break");
    // TimeSpan net = AnyminuteReportDurations.Sum(dt, "net");
    // AnyminuteReportDurations.FormatCells(dt);
        
    // string t1 = AnyminuteReportDurations.Format(gross_hours);
    //    string t2 = AnyminuteReportDurations.Format(break_total);
    //    string t3 = AnyminuteReportDurations.Format(net);
    //    var totalDisplay = t1;
    //    var totalDisplay1 = t2;
    //    var totalDisplay2 = t3;

    //    filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

    //    Response.Clear();
    //    Response.Buffer = true;
    //    Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.csv");
    //    Response.Charset = "";
    //    Response.ContentType = "text/csv";

    //    //To Export all pages.
    //    GvExport.AllowPaging = false;
    //    //this.GvExport();

    //    StringBuilder sb = new StringBuilder();
    //    if(GvExport.HeaderRow!=null) foreach (TableCell cell in GvExport.HeaderRow.Cells)
    //    {
    //        //Append data with separator.
    //        sb.Append(cell.Text + ',');
    //    }
    //    //Append new line character.
    //    sb.Append("\r\n");

    //    foreach (GridViewRow row in GvExport.Rows)
    //    {
    //        foreach (TableCell cell in row.Cells)
    //        {
    //            //Append data with separator.
    //            sb.Append(cell.Text + ',');
    //        }
    //        //Append new line character.
    //        sb.Append("\r\n");
    //    }

    //    Response.Output.Write(sb.ToString());
    //    Response.Flush();
    //    Response.End();

    //    //Response.ClearContent();
    //    //Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", "Customers.csv"));
    //    //Response.ContentType = "application/text";
    //    //GvExport.AllowPaging = false;
    //    //GvExport.DataBind();
    //    //StringBuilder strbldr = new StringBuilder();
    //    //for (int i = 0; i < GvExport.Columns.Count; i++)
    //    //{
    //    //    //separting header columns text with comma operator
    //    //    strbldr.Append(GvExport.Columns[i].HeaderText + ',');
    //    //}
    //    ////appending new line for gridview header row
    //    //strbldr.Append("\n");
    //    //for (int j = 0; j < GvExport.Rows.Count; j++)
    //    //{
    //    //    for (int k = 0; k < GvExport.Columns.Count; k++)
    //    //    {
    //    //        //separating gridview columns with comma
    //    //        strbldr.Append(GvExport.Rows[j].Cells[k].Text + ',');
    //    //    }
    //    //    //appending new line for gridview rows
    //    //    strbldr.Append("\n");
    //    //}
    //    //Response.Write(strbldr.ToString());
    //    //Response.End();



    //    //Response.Clear();
    //    //Response.Buffer = true;
    //    //Response.AddHeader("content-disposition", "attachment;filename=Vithal_Wadje.csv");
    //    //Response.Charset = "";
    //    //Response.ContentType = "application/text";
    //    //GvExport.AllowPaging = false;
    //    //GvExport.DataBind();
    //    //StringBuilder columnbind = new StringBuilder();

    //    //for (int k = 0; k < GvExport.Columns.Count; k++)
    //    //{
    //    //    columnbind.Append(GvExport.Columns[k].HeaderText + ',');
    //    //}
    //    //columnbind.Append("\r\n");
    //    //for (int i = 0; i < GvExport.Rows.Count; i++)
    //    //{
    //    //    for (int k = 0; k < GvExport.Columns.Count; k++)
    //    //    {
    //    //        columnbind.Append(GvExport.Rows[i].Cells[k].Text + ',');
    //    //    }
    //    //    columnbind.Append("\r\n");
    //    //}
    //    //Response.Output.Write(columnbind.ToString());
    //    //Response.Flush();
    //    //Response.End();


    //    //---------------------------------------------
    //    //Response.BufferOutput = false;
    //    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
    //    //Response.ContentType = "application/excel";
    //    //Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
    //    //StringWriter stringWriter = new StringWriter();
    //    //HtmlTextWriter htw = new HtmlTextWriter(stringWriter);




    //    //GvExport.RenderControl(htw);
    //    //GvExport.Visible = true;

    //    //string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Custom Report for " + ddlBusiness.SelectedItem.Text + "</td></tr>" +

    //    //                          "<tr><td colspan=2>" + "Date: " + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd") + "</td></tr>" +
    //    //"</Table>";
    //    //Response.Write(headerTable);
    //    //Response.Write(stringWriter.ToString());
    //    //headerTable = @"<Table><tr><td></td>" +
    //    //               "<td></td>" +
    //    //                "<td align='center'>" + totalDisplay.ToString() + "</td>" +
    //    //                "<td align='center'>" + totalDisplay1.ToString().PadLeft(2, '0') + "</td>" +
    //    //                "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
    //    //                "</Table>";

    //    //Response.Write(headerTable);

    //    //Response.Flush();
    //    //Response.Close();
    //    //GvExport.AllowPaging = true;
    //    //GvExport.DataBind();
    //}



    private bool excelResponseWritten;

    protected override void Render(HtmlTextWriter writer)
    {
        if (!excelResponseWritten) base.Render(writer);
    }

    private void ExportWarning(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this.Page, GetType(), "ExcelWarning",
            "swal('Report unavailable', '" + message + "', 'error');", true);
    }

    private void ExportCompanyExcel(int? employeeId, string employeeName)
    {
        DateTime from, to;
        if (ddrange.SelectedIndex == 0 ||
            !DateTime.TryParse(txtFromDate.Text, out from) || !DateTime.TryParse(txtToDate.Text, out to))
        {
            ExportWarning("Select a valid date range.");
            return;
        }
        if (from > to || (to - from).TotalDays > 366)
        {
            ExportWarning("Select an ordered date range of no more than one year.");
            return;
        }
        try
        {
            int status = int.Parse(DropDownList_Report_Status.SelectedValue);
            DataTable data = employeeId.HasValue
                ? Comm.ReportAcc_CustomerWise_Emp_Excel(ddlBusiness.SelectedValue, from, to, employeeId.Value, status)
                : Comm.ReportAcc_CustomerWise_Excel(ddlBusiness.SelectedValue, from, to, status);
            if (data.Rows.Count == 0)
            {
                ExportWarning("No records match this date range and status.");
                return;
            }
            // Calculate by column name. Blank legacy absence durations have become 00:00 in SQL.
            string[] durationColumns = { "Gross Hours", "Break Time", "Net Hours & Minutes" };
            long[] totals = { 0, 0, 0 };
            foreach (DataRow row in data.Rows)
                for (int i = 0; i < durationColumns.Length; i++)
                {
                    string value = Convert.ToString(row[durationColumns[i]]).Trim();
                    string[] parts = value.Split(':');
                    long hours; int minutes;
                    if (parts.Length != 2 || !long.TryParse(parts[0], out hours) ||
                        !int.TryParse(parts[1], out minutes) || hours < 0 || minutes < 0 || minutes > 59)
                        throw new FormatException("Invalid report duration in " + durationColumns[i]);
                    totals[i] = checked(totals[i] + checked(hours * 60) + minutes);
                }
            var grid = new GridView { AutoGenerateColumns = true, AllowPaging = false };
            grid.DataSource = data;
            grid.DataBind();
            string table;
            using (var buffer = new StringWriter())
            using (var html = new HtmlTextWriter(buffer))
            {
                grid.RenderControl(html);
                table = buffer.ToString();
            }
            string title = System.Web.HttpUtility.HtmlEncode(ddlBusiness.SelectedItem.Text);
            if (employeeId.HasValue) title += " - " + System.Web.HttpUtility.HtmlEncode(employeeName);
            var output = new System.Text.StringBuilder();
            output.Append("<html><head><meta charset='utf-8'></head><body><h3>").Append(title).Append("</h3>");
            output.Append("<p>").Append(from.ToString("yyyy-MM-dd")).Append(" to ")
                .Append(to.ToString("yyyy-MM-dd")).Append("</p><p>");
            for (int i = 0; i < totals.Length; i++)
                output.Append(durationColumns[i]).Append(": ")
                    .Append((totals[i] / 60).ToString("00")).Append(":")
                    .Append((totals[i] % 60).ToString("00")).Append(" &nbsp; ");
            output.Append("</p>").Append(table).Append("</body></html>");
            // Preserve existing HTML-based XLS output; only write the response after it is complete.
            string filename = "Company_Report_" + from.ToString("yyyyMMdd") + "_" + to.ToString("yyyyMMdd")
                + (employeeId.HasValue ? "_Employee_" + employeeId.Value : "") + ".xls";
            Response.Clear();
            Response.BufferOutput = true;
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.Charset = "utf-8";
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(output.ToString());
            excelResponseWritten = true;
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (System.Data.SqlClient.SqlException ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Company Excel report failed", ex);
            ExportWarning(ex.Number == 51020
                ? "This report contains an invalid time entry. Correct the entry identified in the server log, then retry."
                : "The report could not be generated. Please contact support with this date range.");
        }
        catch (FormatException ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Company Excel duration is invalid", ex);
            ExportWarning("The report contains an invalid duration. Please contact support.");
        }
    }

    protected void lnkExcel_Click(object sender, EventArgs e)
    {
        ExportCompanyExcel(null, null);
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
       /* if (GVBusiness.Rows.Count == 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'No Record Found', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please click on update report ', 'error');", true);
            return;
        }*/
        //ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + Convert.ToString(ddlBusiness.SelectedItem.Text) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        string filename = "";
        int Status = 2;
        //DataTable dt = Comm.ReportAcc_CustomerWise(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto),Status);
        DataTable dt = Comm.ReportAcc_CustomerWise(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(DropDownList_Report_Status.SelectedValue));
        GvExport.DataSource = dt;
        GvExport.DataBind();

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
        

        filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

        Response.Clear();
        Response.BufferOutput = false;
        Response.ContentType = "application/pdf";
        Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        GvExport.RenderControl(hw);
        StringReader sr = new StringReader(sw.ToString());
        Document pdfDoc = new Document(PageSize.A2, 30f, 10f, 30f, 30f);
#pragma warning disable CS0612 // Type or member is obsolete  
        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete  
        PdfWriter.GetInstance(pdfDoc, Response.OutputStream);

       
        Chunk chunk = new Chunk("Custom Report for " + ddlBusiness.SelectedItem.Text);
        Chunk chunk1 = new Chunk(" ");
        pdfDoc.Open();
        if (!string.IsNullOrEmpty(Convert.ToString( dt.Rows[0]["CareOf"])))
        {
            byte[] file;
            file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
            jpg.ScaleToFit(30F, 30F);//Set width and height in float    
            pdfDoc.Add(jpg);
            pdfDoc.Add(new Chunk(("            " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + chunk)));
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


        byte[] file1;
        file1 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
        iTextSharp.text.Image jpg1 = iTextSharp.text.Image.GetInstance(file1);
        jpg1.ScaleToFit(45F, 45F);
        pdfDoc.Open();
        pdfDoc.Add(jpg1);

        //chunk1 = new Chunk("\n");
        //pdfDoc.Add(chunk1);

        //chunk = new Chunk(ddlBusiness.SelectedItem.Text);
        //pdfDoc.Add(new Paragraph(chunk));

        

        chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
        pdfDoc.Add(new Paragraph("Date: "+" " + chunk));

        chunk = new Chunk(Environment.NewLine);
        pdfDoc.Add(chunk);
        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count-1);
        //pdfTable.DefaultCell.Padding = 6;
        pdfTable.WidthPercentage = 100f;
        pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;

        

        for (int c = 0; c< dt.Columns.Count -1; c++)
        {
            var redListTextFont1 = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont1);
            PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            cellq.Border = Rectangle.BOX;
             cellq.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellq.Border = Rectangle.ALIGN_CENTER;
            //pdfTable.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellq.Headers = Element.ALIGN_CENTER;
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
            
            for (var item = 0; item< row.ItemArray.Length-1; item ++)
            {
                string val = row.ItemArray[item].ToString();
                if(val.ToString()== "0:0")
                { val = "00:00"; }
                PdfPCell cellitem = new PdfPCell(new Phrase(val.ToString()));
                //cellitem.Border = Rectangle.NO_BORDER;
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
       
        PdfPCell cell = new PdfPCell(new Phrase("Totals"));
        cell.Border = Rectangle.NO_BORDER;
        //cell.Colspan = 1;

        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(""));

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.NO_BORDER;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay.ToString()));

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border =  Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell  =  new PdfPCell(new Phrase(totalDisplay1.ToString()));

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        cell = new PdfPCell(new Phrase(totalDisplay2.ToString()));

        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        cell.Border = Rectangle.BOX;
        pdfTable.AddCell(cell);
        // Create new footer with formatted text.

        pdfDoc.Add(pdfTable);

        //htmlparser.Parse(sr);



        chunk = new Chunk(" \n");
        pdfDoc.Add(new Paragraph(chunk));
        chunk = new Chunk(" \n");
        pdfDoc.Add(new Paragraph(chunk));

        Paragraph portfolioText = new Paragraph("Powered By AnyMinute.Net" , new Font(Font.FontFamily.HELVETICA, 15, Font.NORMAL, iTextSharp.text.BaseColor.BLUE ));
        portfolioText.Alignment = Element.ALIGN_CENTER;
        portfolioText.SetLeading(12.1f, 12.1f);
        portfolioText.IndentationLeft = 90;

        
        Anchor portAnch = new Anchor(portfolioText);
        portAnch.Reference = "https://anyminute.net/";
        //pdfDoc.Add(portfolioText);
        pdfDoc.Add(portAnch);
      
        pdfDoc.Close();
        Response.Write(pdfDoc);
        Response.Flush();
        Response.Close();
        GvExport.AllowPaging = true;
        GvExport.DataBind();
        
    }





    protected void lnkCsvFile_Click(object sender, EventArgs e)
    {

        Response.Clear();
        Response.Buffer = true;
        Response.ClearContent();
        Response.ClearHeaders();
        Response.Charset = "";

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

        string filename = "";
        int Status = 2;
        DataTable dt = Comm.ReportAcc_CustomerWise_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Status);
        GvExport.DataSource = dt;
        GvExport.DataBind();

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

        filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

         Response.Clear();
        Response.Buffer = true;
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".csv");







        Response.Charset = "";
        Response.ContentType = "text/csv";

        //To Export all pages.
        GvExport.AllowPaging = false;
        //this.GvExport();


        StringBuilder sb = new StringBuilder();
        sb.Append(ddlBusiness.SelectedItem.Text);
        sb.Append("\r\n");
        sb.Append("Date:" + dtfrom.ToString("yyyy/MM/dd") + " to " + dtto.ToString("yyyy/MM/dd"));
        sb.Append("\r\n");
        sb.Append("                            " + "Total: " + "      " + totalDisplay + "   " + "        " + totalDisplay1 + " " + "      " + totalDisplay2);

        sb.Append("\r\n");

        if(GvExport.HeaderRow!=null) foreach (TableCell cell in GvExport.HeaderRow.Cells)
        {
            //sb.Append(cell.ToString().PadRight(20)); // Adjust the padding size as needed
            //sb.Append(","); // Add the CSV delimiter

            sb.Append(cell.Text + ',');
            
        }
        //Append new line character.
        sb.Append("\r\n");

        foreach (GridViewRow row in GvExport.Rows)
        {

            foreach (TableCell cell in row.Cells)
            {
                
                //Append data with separator.
                sb.Append(cell.Text + "\t" + "\t" + "\t" + ','.ToString().PadLeft(2));

            }
            //Append new line character.

            sb.Append("\r\n");
        }

        sb.Append("                                 " + "Total: " + "      " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "     " + totalDisplay2);

        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();
        //Response.Close();
        //GvExport.AllowPaging = true;

        //GvExport.DataBind();

    }
   

}