using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Report2 : System.Web.UI.Page
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
            GetBusiness();
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
        //if(UserType_acc==null)
        //{
        //    if (Session["UserType"].ToString() == "BUSINESS OWNER")
        //    {
        //        Comm.FillDropDownList_message(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID", "Business"); //Company
        //    }
        //    else if (Session["UserType"].ToString() == "Admin")
        //    { 
        //        Comm.FillDropDownList_message(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "BusinessID", "Business"); //Company
        //    }
        //    else
        //    {
        //        Comm.FillDropDownList(ddlBusiness, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
        //    }
        //}
        //else
        //{
        //    Comm.FillDropDownList_message(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + " and Status=1", "ab.BusinessID", "Business"); //Company
        //}

        ddlBusiness.Items.Clear();
        var UserType_acc = Session["UserType_acc"];
        if (UserType_acc == null)
        {
            if (Session["UserType"].ToString() == "BUSINESS OWNER")
            {
                Comm.FillDropDownList(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and Status=1", "ab.BusinessID"); //Company
            }
            else if (Session["UserType"].ToString() == "Admin")
            {
                Comm.FillDropDownList(ddlBusiness, "AddBusiness", "BusinessID", "dbo.FN_DESC('AddBusiness',AddBusiness.BusinessID)CompanyName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and Status=1", "BusinessID"); //Company
            }
            else
            {
                Comm.FillDropDownList(ddlBusiness, "AssginBussiness assb join AddBusiness ab on ab.BusinessID=assb.BusinessID", "assb.BusinessID", "dbo.FN_DESC('AddBusiness',assb.BusinessID)", " EmployeeID in (select ad.EmployeeID from  AddEmployee ad join EmpAddress empAdd on ad.ADDRESSID=empAdd.EmpAddressId where empAdd.MainEmail='" + Session["email"].ToString() + "' and ad.UserType in (5,12)) and ab.Status<>2", "BusinessID");
            }
        }
        else
        {
            Comm.FillDropDownList(ddlBusiness, "AddBusiness ab join AssginBussiness assb on assb.BusinessID=ab.BusinessID", "ab.BusinessID", "dbo.FN_DESC('AddBusiness',ab.BusinessID)CompanyName", " assb.EmployeeID=" + Session["EmployeeId"].ToString() + " and UserTypeId=1 and ab.MotherOrgID=" + Session["MotherOrgid_acc"].ToString() + " and Status=1", "ab.BusinessID"); //Company
        }
    }
    //Amit on 28_01_2021 end

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
        if (txtFromDate.Text.ToString().Length == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Select Date Range', 'warning');", true);
            return;
        }
        DateTime baseDate = DateTime.Today;
        var today = baseDate;
        var thisWeekStart = Convert.ToDateTime(txtFromDate.Text);
        var thisWeekEnd = thisWeekStart.AddDays(1).AddSeconds(-1);

        DataTable dt = new DataTable();
        hdnfrmdt.Value = thisWeekStart.ToString();
        //dt = Comm.ReportAcc_Report2(hfSelected.Value, Convert.ToDateTime(thisWeekStart), Convert.ToDateTime(thisWeekEnd));
        dt = Comm.ReportAcc_Report2(hfSelected.Value, Convert.ToDateTime(thisWeekStart), Convert.ToDateTime(thisWeekEnd), int.Parse(DropDownList_Report_Status.SelectedValue));
        GVBusiness.DataSource = dt;
        GVBusiness.DataBind();
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




    protected void lnkExcel_Click(object sender, EventArgs e)
    {
        if (ddlBusiness.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select Business', 'error');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }

        DateTime baseDate = DateTime.Today;
        var today = baseDate;
        var thisWeekStart = Convert.ToDateTime(txtFromDate.Text);
        var thisWeekEnd = thisWeekStart.AddDays(1).AddSeconds(-1);


        DateTime dtfrom = DateTime.Parse(thisWeekStart.ToString("yyyy-MM-dd"));
        DateTime dtto = DateTime.Parse(thisWeekEnd.ToString("yyyy-MM-dd"));

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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }
        // ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + Convert.ToString(ddlBusiness.SelectedItem.Text) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        //DataTable dt = Comm.ReportAcc_Report2_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto));
        DataTable dt = Comm.ReportAcc_Report2_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(DropDownList_Report_Status.SelectedValue));


        TimeSpan gross_hours = new TimeSpan();
        TimeSpan break_total = new TimeSpan();
        TimeSpan net = new TimeSpan();

        if (dt.Rows.Count > 0)
        {
            GvExport.DataSource = dt;
            GvExport.DataBind();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][5].ToString().Trim() != "")
                {
                    string gross_hoursTime = DateTime.Parse(dt.Rows[i][3].ToString()).ToString("HH:mm");
                    var split = Convert.ToString(gross_hoursTime).Split(':');
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
        string filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

        Response.Clear();
        Response.BufferOutput = false;
        Response.ContentType = "application/excel";
        Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
        StringWriter stringWriter = new StringWriter();
        HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
        //GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
        GvExport.RenderControl(htw);
        string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + ddlBusiness.SelectedItem.Text + "</td></tr>" +
                                 //"<tr><td>" + "Business Name: " + ddlBusiness.SelectedItem.Text + "</td></tr>" +
                                 "<tr><td align='left'>" + "Date: " + dtfrom.ToString("yyyy/MM/dd") + "</td></tr>" +
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
        //string headerTables = @"<Table><tr><td></td>" +

        //                   "<td></td>" +
        //                    "<td></td>" +
        //                   "<td align='center'>" + totalDisplay.ToString() + "</td>" +
        //                   "<td align='center'>" + totalDisplay1.ToString() + "</td>" +
        //                   "<td align='center'>" + totalDisplay2.ToString() + "</td></tr>" +
        //                   "</Table>";
        //Response.Write(headerTables);

        Response.End();
    }

    protected void lnkDirectPdf_Click(object sender, EventArgs e)
    {
        if (ddlBusiness.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select Business', 'error');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }

        DateTime baseDate = DateTime.Today;
        var today = baseDate;
        var thisWeekStart = Convert.ToDateTime(txtFromDate.Text);
        var thisWeekEnd = thisWeekStart.AddDays(1).AddSeconds(-1);


        DateTime dtfrom = DateTime.Parse(thisWeekStart.ToString("yyyy-MM-dd"));
        DateTime dtto = DateTime.Parse(thisWeekEnd.ToString("yyyy-MM-dd"));

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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }
        // ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + Convert.ToString(ddlBusiness.SelectedItem.Text) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        //DataTable dt = Comm.ReportAcc_Report_PDF(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto));
        DataTable dt = Comm.ReportAcc_Report_PDF(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(DropDownList_Report_Status.SelectedValue));


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
                    string gross_hoursTime = DateTime.Parse(dt.Rows[j][2].ToString()).ToString("HH:mm");
                    var split = Convert.ToString(gross_hoursTime).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                    var split1 = Convert.ToString(dt.Rows[j][3]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                    var split2 = Convert.ToString(dt.Rows[j][4]).Split(':');
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
        string filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

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
        Chunk chunk = new Chunk("Custom Report for " + ddlBusiness.SelectedItem.Text);
        pdfDoc.Open();

        byte[] file2;
        file2 = System.IO.File.ReadAllBytes(Server.MapPath("~/Assets/assets/img/logo1.jpg"));//ImagePath    
        iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(file2);
        pdfDoc.Open();
        pdfDoc.Add(jpg2);

        //if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
        //{


        //    byte[] file;
        //    file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
        //    iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
        //    jpg.ScaleToFit(140F, 30F);//Set width and height in float    
        //    pdfDoc.Add(jpg);
        //}
        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CareOf"])))
        {
            byte[] file;
            file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
            jpg.ScaleToFit(30F, 30F);//Set width and height in float    
            pdfDoc.Add(jpg);
            pdfDoc.Add(new Chunk((" " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + " " + chunk)));
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


        //byte[] file;
        //file = System.IO.File.ReadAllBytes(Server.MapPath("~/Uploads/" + dt.Rows[0]["CareOf"].ToString()));//ImagePath    
        //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
        //jpg.ScaleToFit(150F, 50F);//Set width and height in float    
        //pdfDoc.Add(jpg);

        //byte[] file;
        //file = System.IO.File.ReadAllBytes(Server.MapPath("~/images/Anyminuteslogo.jpg"));//ImagePath    
        //iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(file);
        //jpg.ScaleToFit(150F, 50F);//Set width and height in float    
        //pdfDoc.Add(jpg);

        //Chunk chunk = new Chunk("Custom Report for " + ddlBusiness.SelectedItem.Text);

        //Paragraph preface = new Paragraph(chunk);
        //// preface.setAlignment(Element.ALIGN_CENTER);
        //preface.Alignment = Element.ALIGN_CENTER;
        //pdfDoc.Add(preface);

        //chunk = new Chunk(ddlBusiness.SelectedItem.Text);
        //pdfDoc.Add(new Paragraph(chunk));

        chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd"));
        pdfDoc.Add(new Paragraph("Date: " + " " + chunk));

        chunk = new Chunk(Environment.NewLine);
        pdfDoc.Add(new Paragraph(" "));
        PdfPTable pdfTable = new PdfPTable(dt.Columns.Count - 1);
        pdfTable.DefaultCell.Padding = 3;
        pdfTable.WidthPercentage = 100;
        pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

        for (int c = 0; c < dt.Columns.Count - 1; c++)
        {
            var redListTextFont = FontFactory.GetFont("Arial", 12, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
            var titleChunk = new Chunk(dt.Columns[c].ColumnName.ToString(), redListTextFont);
            PdfPCell cellq = new PdfPCell(new Phrase(titleChunk));
            cellq.Border = Rectangle.BOX;
            cellq.HorizontalAlignment = Element.ALIGN_CENTER;
            //cellq.BackgroundColor = iTextSharp.text.BaseColor.BLUE;

            pdfTable.AddCell(cellq);


        }
        int i = 1;
        foreach (DataRow row in dt.Rows)
        {
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
            i = i + 1;
        }
        PdfPCell cell = new PdfPCell(new Phrase(""));
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


    protected void lnkCsvFile_Click(object sender, EventArgs e)
    {
        if (ddlBusiness.SelectedValue == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Select Business', 'error');", true);
            return;
        }

        if (GVBusiness.Rows.Count == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }

        DateTime baseDate = DateTime.Today;
        var today = baseDate;
        var thisWeekStart = Convert.ToDateTime(txtFromDate.Text);
        var thisWeekEnd = thisWeekStart.AddDays(1).AddSeconds(-1);


        DateTime dtfrom = DateTime.Parse(thisWeekStart.ToString("yyyy-MM-dd"));
        DateTime dtto = DateTime.Parse(thisWeekEnd.ToString("yyyy-MM-dd"));

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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'No Record Found', 'error');", true);
            return;
        }
        // ScriptManager.RegisterStartupScript(this.Page, GetType(), "download", "ShowDocument('" + Convert.ToString(hfSelected.Value) + "','" + Convert.ToDateTime(dtfrom) + "','" + Convert.ToDateTime(dtto) + "','" + Convert.ToString(ddlBusiness.SelectedItem.Text) + "','" + ddlExportType.SelectedIndex + "',0,'NA','Parent');", true);
        //DataTable dt = Comm.ReportAcc_Report2_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto));
        DataTable dt = Comm.ReportAcc_Report2_Excel(ddlBusiness.SelectedValue, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), int.Parse(DropDownList_Report_Status.SelectedValue));


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
                    string gross_hoursTime = DateTime.Parse(dt.Rows[i][2].ToString()).ToString("HH:mm");
                    var split = Convert.ToString(gross_hoursTime).Split(':');
                    gross_hours = gross_hours.Add(new TimeSpan(int.Parse(split[0]), int.Parse(split[1]), int.Parse("0")));
                    var split1 = Convert.ToString(dt.Rows[i][3]).Split(':');
                    break_total = break_total.Add(new TimeSpan(int.Parse(split1[0]), int.Parse(split1[1]), int.Parse("0")));
                    var split2 = Convert.ToString(dt.Rows[i][4]).Split(':');
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
        string filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

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