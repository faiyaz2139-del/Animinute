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

public partial class BusinessOwnerlist : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    LoginClass LClass = new LoginClass();
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
        ddlBusinessOwner.Items.Clear();
        Comm.FillDropDownList (ddlBusinessOwner, "AddEmployee AE JOIN EmpAddress EAD ON EAD.EmpAddressId=AE.ADDRESSID JOIN UserAcc UA ON UA.Email=EAD.MainEmail", "UA.EmployeeId", "dbo.FN_DESC('AddEmployee',AE.EmployeeID)EmpName", "AE.UserType=1 and AE.MotherOrgID=" + Session["MotherOrgid"].ToString() + "", "BusinessID");
        if(Session["Boid"]!=null)
        {
            string boid = Session["Boid"].ToString();
            ddlBusinessOwner.SelectedValue = boid;
            ddlBusinessOwner_SelectedIndexChanged(null, null);
        }
        
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
    }
    protected void ddlBusinessOwner_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(ddlBusinessOwner.SelectedValue!="")
        {
            GVBusiness.DataSource = null;
            GVBusiness.DataBind();
            aa.Visible = true;
            DataTable dt = new DataTable();
            dt = Comm.ReportBusinessOwner(ddlBusinessOwner.SelectedValue);
            GVBusiness.DataSource = dt;
            GVBusiness.DataBind();
            gvDuplicate.DataSource = dt;
            gvDuplicate.DataBind();
        }
        
    }

    protected void GVBusiness_PreRender(object sender, EventArgs e)
    {
        if (GVBusiness.Rows.Count > 0)
        {
            GVBusiness.UseAccessibleHeader = true;
            GVBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
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
            string filename = ddlBusinessOwner.SelectedItem.Text;
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
            string filename = ddlBusinessOwner.SelectedItem.Text;
            Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
            Response.ContentType = "application/excel";
            StringWriter stringWriter = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            GVBusiness.HeaderRow.Style.Add("background-color", "#FFFFFF");
            GVBusiness.RenderControl(htw);
            Response.Write(stringWriter.ToString());
            Response.End();
        }
        else if (ddlExportType.SelectedValue == "PDF")
        {
            string filename = ddlBusinessOwner.SelectedItem.Text;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename + ".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            gvDuplicate.AllowSorting = false;
            gvDuplicate.RenderControl(hw);
            StringReader sr = new StringReader(sw.ToString());
            Document pdfDoc = new Document(PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);
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
    }

    protected void lnkBusinessOwnerName_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        Label LblCCEmail = (Label)GVBusiness.Rows[gvr.RowIndex].FindControl("LblCCEmail");
        DataTable dtAccToDefault = new DataTable();
        DataTable dtdetails = new DataTable();
        dtAccToDefault = LClass.GetDetilsForAcc(LblCCEmail.Text);
        Session["UserId_acc"] = dtAccToDefault.Rows[0]["UserId"].ToString();
        Session["EmployeeId_acc"] = dtAccToDefault.Rows[0]["EmployeeId"].ToString();
        Session["email_acc"] = dtAccToDefault.Rows[0]["Email"].ToString();
        Session["MotherOrgid_acc"] = dtAccToDefault.Rows[0]["MotherOrgid"].ToString();
        dtdetails = Comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + Session["EmployeeId_acc"].ToString() + "", "");

        Session["UserTypeid_acc"] = dtdetails.Rows[0]["UserType"].ToString();
        Session["UserType_acc"] = Comm.GetNameFromId("UserType", int.Parse(Session["UserTypeid_acc"].ToString()));
        Response.Redirect("Acc_To_Default.aspx");
    }

    protected void gvDuplicate_PreRender(object sender, EventArgs e)
    {
        if (gvDuplicate.Rows.Count > 0)
        {
            gvDuplicate.UseAccessibleHeader = true;
            gvDuplicate.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GVBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName=="BusinessOwnerName")
        {
            int index = Convert.ToInt32(e.CommandArgument);

            GridViewRow row = GVBusiness.Rows[index];
            HiddenField hfdempid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdempid");
            Label LblCCEmail = (Label)GVBusiness.Rows[index].FindControl("LblCCEmail");
            DataTable dtAccToDefault = new DataTable();
            DataTable dtdetails = new DataTable();
            dtAccToDefault = LClass.GetDetilsForAcc(LblCCEmail.Text);

            Session["UserId_acc"] = Session["UserId"];
            Session["EmployeeId_acc"] = Session["EmployeeId"];
            Session["email_acc"] = Session["email"];
            Session["MotherOrgid_acc"] = Session["MotherOrgid"];


            Session["UserTypeid_acc"] = Session["UserTypeid"];
            Session["UserType_acc"] = Session["UserType"];


            //--------------------
            Session["UserId"] = dtAccToDefault.Rows[0]["UserId"].ToString();
            Session["EmployeeId"] = dtAccToDefault.Rows[0]["EmployeeId"].ToString();
            Session["email"] = dtAccToDefault.Rows[0]["Email"].ToString();
            Session["MotherOrgid"] = dtAccToDefault.Rows[0]["MotherOrgid"].ToString();
            dtdetails = Comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + Session["EmployeeId"].ToString() + "", "");

            Session["UserTypeid"] = dtdetails.Rows[0]["UserType"].ToString();
            Session["UserType"] = Comm.GetNameFromId("UserType", int.Parse(Session["UserTypeid"].ToString()));
            Session["Page"] = "BusinessOwnerlist";
            Session["Boid"] = ddlBusinessOwner.SelectedValue;
            Response.Redirect("Default.aspx", false);

        }
    }
}