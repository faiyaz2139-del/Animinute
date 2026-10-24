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

public partial class BO_Individual : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ReportController RptCtrl = new ReportController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetBusiness();
        }
    }
    public void GetBusiness()
    {
        DataTable dt = new DataTable();
        ddlBusiness.Items.Clear();
        dt = Comm.GetAllDataFromTable("MappingTable mt join AssginBussiness ab on ab.EmployeeID=mt.EmployeeID", "ab.BusinessID,dbo.FN_DESC('AddBusiness',ab.BusinessID)BusinessName", "mt.USERID='" + Session["UserId"].ToString() + "' and ab.UserTypeId=1", "BusinessID");
        ddlBusiness.DataSource = dt;
        ddlBusiness.DataTextField = "BusinessName";
        ddlBusiness.DataValueField = "BusinessID";
        ddlBusiness.DataBind();
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
        DataTable dt = new DataTable();
        dt = Comm.ReportAcc_BusinessOwner(hfSelected.Value, Convert.ToDateTime(txtFromDate.Text), Convert.ToDateTime(txtToDate.Text));
        GVBusiness.DataSource = dt;
        GVBusiness.DataBind();
    }

    protected void lnkExcel_Click(object sender, EventArgs e)
    {
        DataTable dtReport = new DataTable();
        LinkButton lnkExcel = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkExcel.NamingContainer;
        HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdEmpid");
        HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdBussID");
        Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName");

        dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

        GvExport.DataSource = dtReport;
        GvExport.DataBind();

        Response.ClearContent();
        string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
        Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
        Response.ContentType = "application/excel";
        StringWriter stringWriter = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
        GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
        GvExport.RenderControl(htw);
        Response.Write(stringWriter.ToString());
        Response.End();



    }
    public override void VerifyRenderingInServerForm(Control control)
    {
    }
    protected void lnkPdf_Click(object sender, EventArgs e)
    {
        DataTable dtReport = new DataTable();
        LinkButton lnkExcel = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkExcel.NamingContainer;
        HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdEmpid");
        HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdBussID");
        Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName");

        dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

        GvExport.DataSource = dtReport;
        GvExport.DataBind();

        string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
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
        htmlparser.Parse(sr);
        pdfDoc.Close();
        Response.Write(pdfDoc);
        Response.End();
        GvExport.AllowPaging = true;
        GvExport.DataBind();
    }

    protected void lnkDoc_Click(object sender, EventArgs e)
    {
        DataTable dtReport = new DataTable();
        LinkButton lnkExcel = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkExcel.NamingContainer;
        HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdEmpid");
        HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[gvr.RowIndex].FindControl("hfdBussID");
        Label lblEmployeeName = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("lblEmployeeName");

        dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

        GvExport.DataSource = dtReport;
        GvExport.DataBind();

        Response.ClearContent();
        string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
        Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".doc");
        Response.ContentType = "application/word";
        StringWriter stringWriter = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
        GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
        GvExport.RenderControl(htw);
        Response.Write(stringWriter.ToString());
        Response.End();
    }

    protected void GVBusiness_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVBusiness.PageIndex = e.NewPageIndex;
        btnshow_Click(sender,e);
    }

    protected void GVBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName=="Excel")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();
            
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid");
            HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[index].FindControl("hfdBussID");
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName");

            dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

            GvExport.DataSource = dtReport;
            GvExport.DataBind();

            Response.ClearContent();
            string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GvExport.RenderControl(htw);
            Response.Write(stringWriter.ToString());
            Response.End();

        }
        else if(e.CommandName=="PDF")
        {
            DataTable dtReport = new DataTable();
            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid");
            HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[index].FindControl("hfdBussID");
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName");

            dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

            GvExport.DataSource = dtReport;
            GvExport.DataBind();

            string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
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
            htmlparser.Parse(sr);
            pdfDoc.Close();
            Response.Write(pdfDoc);
            Response.End();
            GvExport.AllowPaging = true;
            GvExport.DataBind();
        }
        else if(e.CommandName== "Word")
        {
            DataTable dtReport = new DataTable();
            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid");
            HiddenField hfdBussID = (HiddenField)GVBusiness.Rows[index].FindControl("hfdBussID");
            Label lblEmployeeName = (Label)GVBusiness.Rows[index].FindControl("lblEmployeeName");

            dtReport = RptCtrl.GetReportAccindividual(hfdBussID.Value, hfdEmpid.Value, DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));

            GvExport.DataSource = dtReport;
            GvExport.DataBind();

            Response.ClearContent();
            string filename = lblEmployeeName.Text + "_" + txtFromDate.Text + "_" + txtToDate.Text;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".doc");
            Response.ContentType = "application/word";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GvExport.RenderControl(htw);
            Response.Write(stringWriter.ToString());
            Response.End();
        }
    }
}