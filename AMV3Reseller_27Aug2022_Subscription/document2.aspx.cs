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

public partial class document2 : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ReportController RptCtrl = new ReportController();
    protected void Page_Load(object sender, EventArgs e)
    {
        string hfSelected = Request.QueryString["hfSelected"];
        DateTime dtfrom = Convert.ToDateTime(Request.QueryString["dtfrom"]);
        DateTime dtto = Convert.ToDateTime(Request.QueryString["dtto"]);
        string businessname = Request.QueryString["businessname"];
        string counter = Request.QueryString["counter"].Trim();
        string type = Request.QueryString["type"].Trim();
        string empid = Request.QueryString["empid"].Trim();
        string empname = Request.QueryString["empname"].Trim();
        DataTable dt = new DataTable();
        if (type == "Parent")
        {
            //dt = Comm.ReportAcc_Report2(hfSelected, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto));

            int Status = 2;
            dt = Comm.ReportAcc_Report2(hfSelected, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Status);

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
                        var split = Convert.ToString(dt.Rows[i][2]).Split(':');
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
            string filename = businessname + "_" + dtfrom.ToString() + "_" + dtto.ToString();

            //string filename = ddlBusiness.SelectedItem.Text + "_" + dtfrom.ToString() + "_" + dtto.ToString();

            if (counter == "1")
            {
                Response.Clear();
                Response.BufferOutput = false;
                Response.ContentType = "application/excel";
                Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
                //GvExport.HeaderRow.Style.Add("background-color", "#FFFFFF");
                GvExport.RenderControl(htw);
                string headerTable = @"<Table><tr><TD align='center' colspan='5' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for " + businessname + "</td></tr>" +
                                         "<tr><td>" + businessname + "</td></tr>" +
                                         "<tr><td align='left'>" + dtfrom.ToString("yyyy/MM/dd") + "</td></tr>" +
                "</Table>";
                Response.Write(headerTable);
                Response.Write(stringWriter.ToString());
                Response.End();

            }
            if (counter == "2")
            {
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
                Chunk chunk = new Chunk("Custom Report for " +businessname);

                Paragraph preface = new Paragraph(chunk);
                // preface.setAlignment(Element.ALIGN_CENTER);
                preface.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(preface);

                chunk = new Chunk(businessname);
                pdfDoc.Add(new Paragraph(chunk));

                chunk = new Chunk(dtfrom.ToString("yyyy/MM/dd"));
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
        }

        if (type == "Child")
        {
            DataTable dtReport = new DataTable();

            dt = Comm.ReportAcc_CustomerWise_Emp(hfSelected, Convert.ToDateTime(dtfrom), Convert.ToDateTime(dtto), Convert.ToInt32(empid));
            TimeSpan gross_hours = new TimeSpan();
            TimeSpan break_total = new TimeSpan();
            TimeSpan net = new TimeSpan();
            string filename = empname + "_" + dtfrom.ToString("yyyy/MM/dd") + "_" + dtto.ToString("yyyy/MM/dd");

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
            if (counter == "1")
            {
                Response.ClearContent();
                Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                Response.ContentType = "application/excel";
                StringWriter stringWriter = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(stringWriter);
                // GvExport1.HeaderRow.Style.Add("background-color", "#FFFFFF");
                GvExport1.RenderControl(htw);

                string headerTable = @"<Table><tr><TD align='center' colspan='8' style='font-size:14.0pt;text-weight:bold;text-decoration:underline;'>Report for Employees working Under " + businessname + "</td></tr>" +
                                            "<tr><td style='font-size:12.0pt;text-weight:bold;'>" + empname + "</td></tr>" +
                                             "<tr><td colspan=2>" + Convert.ToDateTime(dtfrom).ToString("yyyy/MM/dd") + " to " + Convert.ToDateTime(dtto).ToString("yyyy/MM/dd") + "</td></tr>" +
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
                Response.Write(headerTable);
                Response.End();
            }
            if (counter == "2")
            {
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
                Chunk chunk = new Chunk("Report for Employees working Under " + businessname);
                Paragraph preface = new Paragraph(chunk);
                // preface.setAlignment(Element.ALIGN_CENTER);
                preface.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(preface);

                chunk = new Chunk(empname);
                pdfDoc.Add(new Paragraph(chunk));

                chunk = new Chunk(Convert.ToDateTime(dtfrom).ToString("yyyy/MM/dd") + " to " + Convert.ToDateTime(dtto).ToString("yyyy/MM/dd"));
                pdfDoc.Add(new Paragraph(chunk));

                chunk = new Chunk(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine);
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
                //*********************
                GvExport1.AllowPaging = true;
                GvExport1.DataBind();

            }
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
    }

}