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
public partial class Bo_BusinessWise : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
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
        dt = Comm.GetAllDataFromTable("MappingTable mt join AssginBussiness ab on ab.EmployeeID=mt.EmployeeID", "ab.BusinessID,dbo.FN_DESC('AddBusiness',ab.BusinessID)BusinessName", "mt.USERID='"+ Session["UserId"].ToString() + "' and ab.UserTypeId=1", "BusinessID");
        ddlBusiness.DataSource = dt;
        ddlBusiness.DataTextField = "BusinessName";
        ddlBusiness.DataValueField = "BusinessID";
        ddlBusiness.DataBind();
    }
    protected void btnshow_Click(object sender, EventArgs e)
    {
        GVBusiness.DataSource = null;
        GVBusiness.DataBind();
        ddlExportType.SelectedValue = "Select";
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
        dt = Comm.ReportBusinessWise(hfSelected.Value, Convert.ToDateTime(txtFromDate.Text), Convert.ToDateTime(txtToDate.Text));
        GVBusiness.DataSource = dt;
        GVBusiness.DataBind();
    }


    protected void GVBusiness_PreRender(object sender, EventArgs e)
    {

        if (GVBusiness.Rows.Count > 0)
        {
            GVBusiness.UseAccessibleHeader = true;
            GVBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
    }

  

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'No Data To Export', 'error');", true);
            return;
        }


        if (ddlExportType.SelectedValue == "Word")
        {
            Response.ClearContent();
            string filename = txtFromDate.Text + "_" + txtToDate.Text;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".doc");
            Response.ContentType = "application/word";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GVBusiness.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GVBusiness.RenderControl(htw);
            Response.Write(stringWriter.ToString());
            Response.End();
        }
        else if (ddlExportType.SelectedValue == "Excel")
        {
            Response.ClearContent();
            string filename = txtFromDate.Text + "_" + txtToDate.Text;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GVBusiness.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GVBusiness.RenderControl(htw);
            Response.Write(stringWriter.ToString());
            Response.End();
            Response.ClearContent();
        }
        else if (ddlExportType.SelectedValue == "PDF")
        {
            string filename = txtFromDate.Text + "_" + txtToDate.Text;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            GVBusiness.RenderControl(hw);
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
            GVBusiness.AllowPaging = true;
            GVBusiness.DataBind();
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('error!', 'Select Export File Format', 'error');", true);
        }
        ddlExportType.SelectedValue = "Select";
    }
}