using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ProjectWiseReport : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ProjectReportClass prc = new ProjectReportClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        AnyminuteReportLayout.LegacyRedirect("project-cost"); // v48: same report layout across all modules (deckR)
        if (!IsPostBack)
        {
            this.pnlhideandshow.Visible = false;
            GetBusiness();
            txtFromDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

    }

    //Amit on 28_01_2021 start
    public void GetBusiness()
    {
        //DataTable dt = new DataTable();
        //ddlProject.Items.Clear();

        //var UserType_acc = Session["UserType_acc"];
        //if (UserType_acc == null)
        //{
        //    if (Session["UserType"].ToString() == "BUSINESS OWNER")
        //    {
        //        Comm.FillDropDownList_message(ddlProject, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID", "Business"); //Company
        //    }
        //    else if (Session["UserType"].ToString() == "Admin")
        //    {
        //        Comm.FillDropDownList_message(ddlProject, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "BusinessID", "Business"); //Company
        //    }
        //    else
        //    {
        //        Comm.FillDropDownList(ddlProject, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
        //    }
        //}
        //else
        //{
        //    Comm.FillDropDownList_message(ddlProject, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "ab.BusinessID", "Business"); //Company
        //}

        ddlProject.Items.Clear();

        var UserType_acc = Session["UserType_acc"];
        if (UserType_acc == null)
        {
            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                Comm.FillDropDownList(ddlProject, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID"); //Company
            }
            else if (Session["UserType"].ToString() == "Admin")
            {
                Comm.FillDropDownList(ddlProject, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "BusinessID"); //Company
            }
            else
            {
                Comm.FillDropDownList(ddlProject, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
            }
        }
        else
        {
            Comm.FillDropDownList(ddlProject, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + "", "ab.BusinessID"); //Company
        }
    }
    //Amit 28_01_2021 End

    protected void ddlDateRange_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (ddlDateRange.SelectedValue == "Weekly")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddDays(-7);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Last 4 Weeks")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddDays(-28);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Last 30 Days")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddDays(-30);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Last 60 Days")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddDays(-60);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Last 90 Days")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddDays(-90);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Monthly")
        {
            txtToDate.ReadOnly = true;
            txtFromDate.ReadOnly = true;

            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DateTime fromdate = DateTime.Now.AddMonths(-1);
            txtFromDate.Text = fromdate.ToString("yyyy-MM-dd");
        }
        else if (ddlDateRange.SelectedValue == "Custom")
        {
            txtToDate.ReadOnly = false;

            txtFromDate.ReadOnly = false;
        }
    }

    protected void btnShow_Click(object sender, EventArgs e)
    {

        DataTable dt = new DataTable();

        string TGrossHours;
        TimeSpan TBreakTotal;
        TimeSpan TTotal;
        //dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.ParseExact(txtFromDate.Text,"yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime.ParseExact(txtToDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture));
        dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.ParseExact(txtFromDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime.ParseExact(txtToDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture), int.Parse(DropDownList_Report_Status.SelectedValue));
        DataView dataView = dt.DefaultView;

        //dataView.RowFilter = "ProjectId <> '" + string.Empty + "'";
        dataView.RowFilter = "ProjectId <> '0' and ProjectId <> '" + string.Empty + "'";

        GVBusiness.DataSource = dataView;
        GVBusiness.DataBind();
        this.pnlhideandshow.Visible = true;
    }

    protected void GVBusiness_PreRender(object sender, EventArgs e)
    {
        if (GVBusiness.Rows.Count > 0)
        {
            GVBusiness.UseAccessibleHeader = true;
            GVBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
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

    public override void VerifyRenderingInServerForm(Control control)
    {
    }


    protected void lnkExcel_Click(object sender, EventArgs e)
    {
        if (ddlProject.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business ', 'warning');", true);
            return;
        }

        if (ddlDateRange.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'No Record Found', 'warning');", true);
            return;
        }

        DataTable dt = new DataTable();

        DataTable dt1 = new DataTable();
        dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

        string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
        //dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));//your datatable
        dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));//your datatable
        dt.Columns.Remove("ProjectId");
        dt.AcceptChanges();
        string table = ConvertDataTableToHTML(dt);
        // GvExport.DataSource = dt;
        // GvExport.DataBind();
        string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

        string attachment = "attachment; filename=" + filename + ".xls";
        Response.ClearContent();
        Response.AddHeader("content-disposition", attachment);
        Response.ContentType = "application/vnd.ms-excel";
        StringWriter stringWriter = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
        GvExport.RenderControl(htw);

        string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + businessname + "</td></tr>" +
                                              //"<tr><td>" + businessname + "</td></tr>" +
                                              "<tr><td colspan=2>" + "Date: " + " " + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd") + "</td></tr>" +
                    "</Table>";
        Response.Write(headerTable);

        Response.Write(stringWriter.ToString());
        Response.Write(table);

        Response.End();
    }

    protected void lnkDirectPdf_Click(object sender, EventArgs e)
    {
        if (ddlProject.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business ', 'warning');", true);
            return;
        }

        if (ddlDateRange.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'No Record Found', 'warning');", true);
            return;
        }

        DataTable dt = new DataTable();

        DataTable dt1 = new DataTable();
        dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

        string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
        //dt = prc.GetProjectReportGrids(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));//your datatable 
        dt = prc.GetProjectReportGrids(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));//your datatable 
        dt.Columns.Remove("ProjectId");
        dt.AcceptChanges();
        string table = ConvertDataTableToHTML(dt);
        // GvExport.DataSource = dt;
        // GvExport.DataBind();
        string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

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

        Chunk chunk = new Chunk(businessname);

        //Paragraph preface = new Paragraph(chunk);
        //// preface.setAlignment(Element.ALIGN_CENTER);
        //preface.Alignment = Element.ALIGN_CENTER;
        //pdfDoc.Add(preface);
        pdfDoc.Open();
        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
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


        byte[] file3;
        file3 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
        iTextSharp.text.Image jpg3 = iTextSharp.text.Image.GetInstance(file3);
        pdfDoc.Open();
        pdfDoc.Add(jpg3);

        chunk = new Chunk(DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd"));
        pdfDoc.Add(new Paragraph("Date: " + " " + chunk));
        var chunk1 = new Chunk(" \n");
        pdfDoc.Add(new Paragraph(chunk1));
        chunk = new Chunk(Environment.NewLine);
        pdfDoc.Add(new Paragraph(" "));
        for (int j = 0; j < dt.Columns.Count; j++)
        {
            if (j == 0)
            {
                dt.Columns.RemoveAt(j);
            }
            if (j == 1)
            {
                dt.Columns.RemoveAt(j);
            }
            //if (j == 2)
            //{
            //    dt.Columns.RemoveAt(j);
            //}
        }
        for (int k = 0; k < dt.Columns.Count; k++)
        {
            if (k == 0)
            {
                dt.Columns.RemoveAt(k);
            }

        }
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
            var redListTextFont1 = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont1);
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




            //foreach (var item in row.ItemArray)
            //{
            //    PdfPCell cellitem = new PdfPCell(new Phrase(item.ToString()));
            //    cellitem.Border = Rectangle.NO_BORDER;

            //    if (i % 2 != 0)
            //    {
            //        cellitem.BackgroundColor = iTextSharp.text.BaseColor.WHITE;
            //    }
            //    else
            //    {
            //        cellitem.BackgroundColor = iTextSharp.text.BaseColor.LIGHT_GRAY;
            //    }
            //    pdfTable.AddCell(cellitem);

            //}
            for (var item = 0; item < row.ItemArray.Length - 1; item++)
            {
                string val = row.ItemArray[item].ToString();
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

    protected void GVBusiness_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Excel")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();
            DataTable dt = new DataTable();
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid") as HiddenField;
            HiddenField hfdProjectid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdProjectid") as HiddenField;
            //dt = prc.GetProjectReportFromGrids(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));
            dt = prc.GetProjectReportFromGrids(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));

            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 8)
                {
                    dt.Columns.RemoveAt(j);
                }

            }
            for (int l = 0; l < dt.Columns.Count; l++)
            {
                if (l == 8)
                {
                    dt.Columns.RemoveAt(l);
                }

            }
            dt.Columns.Remove("EmpId");
            dt.Columns.Remove("ProjectId");
            dt.AcceptChanges();
            string table = ConvertDataTableToHTML(dt);
            DataTable dt1 = new DataTable();
            dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

            string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
            string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

            string attachment = "attachment; filename=" + filename + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter stringWriter = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
            //GvExport.RenderControl(htw);

            string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + businessname + "</td></tr>" +
                                                  //"<tr><td>" + businessname + "</td></tr>" +
                                                  "<tr><td colspan=2>" + "Date: " + " " + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd") + "</td></tr>" +
                        "</Table>";
            Response.Write(headerTable);
            Response.Write(stringWriter.ToString());
            Response.Write(table);
            Response.End();
        }

        else if (e.CommandName == "PDF")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();
            DataTable dt = new DataTable();
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid") as HiddenField;
            HiddenField hfdProjectid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdProjectid") as HiddenField;
            //dt = prc.GetProjectReportFromGrid(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));
            dt = prc.GetProjectReportFromGrid(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));
            dt.Columns.Remove("EmpId");
            dt.Columns.Remove("ProjectId");
            dt.AcceptChanges();
            string table = ConvertDataTableToHTML(dt);
            DataTable dt1 = new DataTable();
            dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

            string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
            string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

            string attachment = "attachment; filename=" + filename + ".xls";

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

            // pdfDoc.Open();
            //if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
            //{
            //    byte[] file;
            //    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
            //    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
            //    jpg.ScaleToFit(140F, 30F);//Set width and height in float    
            //    pdfDoc.Add(jpg);
            //}

            byte[] file2;
            file2 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(file2);
            pdfDoc.Open();
            pdfDoc.Add(jpg2);
            Chunk chunk = new Chunk(businessname);

            //Paragraph preface = new Paragraph(chunk);
            //// preface.setAlignment(Element.ALIGN_CENTER);
            //preface.Alignment = Element.ALIGN_CENTER;
            //pdfDoc.Add(preface);

            if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
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




            chunk = new Chunk("\n");
            pdfDoc.Add(new Paragraph(chunk));





            //chunk = new Chunk(businessname);
            //pdfDoc.Add(new Paragraph(chunk));

            chunk = new Chunk(DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd"));
            pdfDoc.Add(new Paragraph("Date: " + " " + chunk));

            chunk = new Chunk(Environment.NewLine);
            pdfDoc.Add(new Paragraph(" "));
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 7)
                {
                    dt.Columns.RemoveAt(j);
                }

            }
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 6)
                {
                    dt.Columns.RemoveAt(j);
                }


            }
            for (int j = 0; j < dt.Columns.Count; j++)
            {
                if (j == 7)
                {
                    dt.Columns.RemoveAt(j);
                }


            }
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
                var redListTextFont1 = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont1);
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
                    string val = row.ItemArray[item].ToString();
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

        else if (e.CommandName == "CSV")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            DataTable dtReport = new DataTable();
            DataTable dt = new DataTable();
            HiddenField hfdEmpid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdEmpid") as HiddenField;
            HiddenField hfdProjectid = (HiddenField)GVBusiness.Rows[index].FindControl("hfdProjectid") as HiddenField;
            //dt = prc.GetProjectReportFromGrids(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));
            dt = prc.GetProjectReportFromGrids(int.Parse(hfdProjectid.Value), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));
            dt.Columns.Remove("EmpId");
            dt.Columns.Remove("ProjectId");
            dt.AcceptChanges();
            string table = ConvertDataTableToHTML(dt);
            GvExport.DataSource = dt;
            GvExport.DataBind();
            DataTable dt1 = new DataTable();
            dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

            //string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
            //string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");


            // string filename = ddlemployee.SelectedItem.Text + dtfrom.ToString("yyyy/MM/dd") + "_" + dtto.ToString("yyyy/MM/dd");

            //filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

            string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
            string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".csv");

            Response.Charset = "";
            Response.ContentType = "text/csv";

            //To Export all pages.
            GvExport.AllowPaging = false;
            //this.GvExport();

            StringBuilder sb = new StringBuilder();
            sb.Append(ddlProject.SelectedItem.Text);
            sb.Append("\r\n");
            sb.Append("Date:" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd"));
            sb.Append("\r\n");
            foreach (TableCell cell in GvExport.HeaderRow.Cells)
            {
                //Append data with separator.
                sb.Append(cell.Text + ',');
            }
            //Append new line character.
            sb.Append("\r\n");

            foreach (GridViewRow row1 in GvExport.Rows)
            {
                foreach (TableCell cell in row1.Cells)
                {
                    //Append data with separator.
                    sb.Append(cell.Text + ',');
                }
                //Append new line character.
                sb.Append("\r\n");

            }


            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();

        }
    }
    protected void lnkCsvFile_Click(object sender, EventArgs e)
    {
        if (ddlProject.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business ', 'warning');", true);
            return;
        }

        if (ddlDateRange.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'No Record Found', 'warning');", true);
            return;
        }

        DataTable dt = new DataTable();

        DataTable dt1 = new DataTable();
        dt1 = Comm.GetAllDataFromTable("TimeSheetEntry", "distinct dbo.FN_DESC('AddBusiness',BussID)bussname", "BussID=" + ddlProject.SelectedValue + "", "");

        string businessname = "" + dt1.Rows[0]["bussname"].ToString() + " Hours Report by Project";
        //dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text));//your datatable
        dt = prc.GetProjectReportGrid(int.Parse(ddlProject.SelectedValue), DateTime.Parse(txtFromDate.Text), DateTime.Parse(txtToDate.Text), int.Parse(DropDownList_Report_Status.SelectedValue));//your datatable
        dt.Columns.Remove("ProjectId");
        dt.AcceptChanges();
        string table = ConvertDataTableToHTML(dt);
        GvExport.DataSource = dt;
        GvExport.DataBind();


        string filename = businessname + "_" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + "_" + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd");

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
        sb.Append(ddlProject.SelectedItem.Text);
        sb.Append("\r\n");
        sb.Append("Date:" + DateTime.Parse(txtFromDate.Text).ToString("yyyy/MM/dd") + " to " + DateTime.Parse(txtToDate.Text).ToString("yyyy/MM/dd"));
        sb.Append("\r\n");

        // sb.Append("                  " + "Total: " + "     " + totalDisplay + "   " + "       " + totalDisplay1 + " " + "    " + totalDisplay2);


        foreach (TableCell cell in GvExport.HeaderRow.Cells)
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
                //if (cell.Text == "&nbsp;")
                //{
                //    continue;
                //}
                //Append data with separator.
                sb.Append(cell.Text + ',');
                if (cell.Text == "&nbsp;")
                {
                    sb.Append(cell.Text.Replace("&nbsp;", "Total"));
                }
            }
            //Append new line character.
            sb.Append("\r\n");

        }


        Response.Output.Write(sb.ToString());
        Response.Flush();
        Response.End();

    }
}