using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class My_Time_Report : Page
{
    private int LoginUserId
    {
        get { int id; return Int32.TryParse(Convert.ToString(Session["UserId"]), out id) ? id : 0; }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (LoginUserId <= 0 || Session["EmployeeId"] == null) { Response.Redirect("Login.aspx"); return; }
        AnyminuteReportLayout.LegacyRedirect("time-card"); // v48: same report layout across all modules (deckR)
        if (!IsPostBack)
        {
            FromDate.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            ToDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            try
            {
                using (var c = AnyminuteTimesheetRepair.Open())
                {
                    // Stable authenticated USERID connects employee identities in different businesses.
                    var data = AnyminuteTimesheetRepair.Query(c, null,
                        @"SELECT DISTINCT b.BusinessID,b.CompanyName FROM AddBusiness b
                          JOIN MappingTable m ON m.BusinessID=b.BusinessID
                          JOIN AssginBussiness a ON a.BusinessID=m.BusinessID AND a.EmployeeID=m.EmployeeID
                          WHERE m.USERID=@u AND m.UserTypeId=4 AND a.UserTypeId=4 AND b.Status<>2
                          ORDER BY b.CompanyName", "@u", LoginUserId);
                    Businesses.DataSource = data; Businesses.DataTextField = "CompanyName";
                    Businesses.DataValueField = "BusinessID"; Businesses.DataBind();
                    if (data.Rows.Count == 0) Message.Text = "No employee business assignments were found. Ask your subscriber to check your business assignment.";
                }
            }
            catch (Exception ex) { ReportError(ex); }
        }
    }
    private DataTable ReadReport()
    {
        DateTime from, to; int business;
        if (LoginUserId <= 0 || !Int32.TryParse(Businesses.SelectedValue, out business))
            throw new InvalidOperationException("Select an assigned business.");
        if (!DateTime.TryParseExact(FromDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out from)
            || !DateTime.TryParseExact(ToDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out to)
            || to < from || (to-from).TotalDays > 366 || to == DateTime.MaxValue.Date)
            throw new InvalidOperationException("Choose a valid date range of up to 367 days.");
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            // Recheck scope on every view/export. Never trust business selection alone.
            return AnyminuteTimesheetRepair.Query(c, null,
                @"SELECT t.TimeSheetDate AS [Date], t.ProjectId AS [Project ID],
                    RTRIM(t.Worked) AS [Worked],t.ReqularStart AS [Start],t.Regularend AS [End],
                    t.Regular AS [Regular],t.BreakTotal AS [Break],RTRIM(t.Total) AS [Total],
                    CASE t.Status WHEN 1 THEN 'Pending' WHEN 2 THEN 'Approved'
                        WHEN 3 THEN 'Rejected' WHEN 4 THEN 'Absent' ELSE 'Unsubmitted' END AS [Status]
                  FROM TimeSheetEntry t
                  WHERE t.BussID=@b AND t.TimeSheetDate>=@f AND t.TimeSheetDate<@t
                    AND EXISTS (SELECT 1 FROM MappingTable m
                      JOIN AssginBussiness a ON a.BusinessID=m.BusinessID AND a.EmployeeID=m.EmployeeID
                      JOIN AddBusiness b ON b.BusinessID=m.BusinessID
                      WHERE m.USERID=@u AND m.UserTypeId=4 AND a.UserTypeId=4 AND b.Status<>2
                        AND m.BusinessID=t.BussID AND m.EmployeeID=t.EmpId)
                  ORDER BY t.TimeSheetDate,t.TimeSheetEntryId",
                  "@u",LoginUserId,"@b",business,"@f",from,"@t",to.AddDays(1));
        }
    }
    protected void ShowReport_Click(object sender, EventArgs e)
    {
        try { var data=ReadReport(); Results.DataSource=data; Results.DataBind(); Message.Text=data.Rows.Count+" saved timesheet(s)."; }
        catch (InvalidOperationException ex) { Message.Text=Server.HtmlEncode(ex.Message); }
        catch (Exception ex) { ReportError(ex); }
    }
    private static string Csv(object value)
    {
        string s = value is DateTime ? ((DateTime)value).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) : Convert.ToString(value);
        string trimmed=s.TrimStart();
        if (trimmed.Length>0 && "=+-@".IndexOf(trimmed[0])>=0) s="'"+s;
        return "\""+s.Replace("\"","\"\"")+"\"";
    }
    protected void DownloadReport_Click(object sender, EventArgs e)
    {
        DataTable data;
        try { data=ReadReport(); }
        catch (InvalidOperationException ex) { Message.Text=Server.HtmlEncode(ex.Message); return; }
        catch (Exception ex) { ReportError(ex); return; }
        var csv=new StringBuilder();
        for(int i=0;i<data.Columns.Count;i++) { if(i>0)csv.Append(','); csv.Append(Csv(data.Columns[i].ColumnName)); }
        csv.AppendLine();
        foreach(DataRow row in data.Rows)
        { for(int i=0;i<data.Columns.Count;i++) { if(i>0)csv.Append(','); csv.Append(Csv(row[i])); } csv.AppendLine(); }
        Response.Clear(); Response.ContentType="text/csv"; Response.ContentEncoding=Encoding.UTF8;
        Response.AddHeader("Content-Disposition","attachment; filename=My_Timesheets.csv");
        Response.BinaryWrite(Encoding.UTF8.GetPreamble()); Response.Write(csv.ToString()); Response.End();
    }
    private void ReportError(Exception ex)
    {
        log4net.LogManager.GetLogger("Log").Error("Personal timesheet report failed",ex);
        Message.Text="The report could not be loaded. Please contact support.";
    }
}
