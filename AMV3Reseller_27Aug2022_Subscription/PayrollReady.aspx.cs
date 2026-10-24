using System;
using System.Data;
using System.Text;
using System.Web;
public partial class AnyminutePayrollReady : System.Web.UI.Page
{
 protected void Page_Load(object s,EventArgs e){if(!IsPostBack){Rows.DataSource=GetRows();Rows.DataBind();}}
 DataTable GetRows()
 {
  var result=new DataTable();foreach(string n in new[]{"Business","Employee","EmployeeID","WeekStart","NetHours","DecimalHours","ApprovedBy","ApprovedAtUTC"})result.Columns.Add(n);
  foreach(DataRow business in AnyminuteModernScope.Businesses().Rows)
  {int b=Convert.ToInt32(business["BusinessID"]);if(!AnyminuteModernScope.Owns(b))continue;
   using(var c=AnyminuteTimesheetRepair.Open())foreach(DataRow h in AnyminuteTimesheetRepair.Query(c,null,"SELECT * FROM AnyminuteWeek WHERE BusinessID=@b AND State='Approved' AND ApprovedAtUTC IS NOT NULL ORDER BY WeekStart DESC","@b",b).Rows)
   {int employee=Convert.ToInt32(h["EmployeeID"]);TimeSpan total=TimeSpan.Zero;foreach(DataRow day in AnyminuteTimesheetRepair.Query(c,null,"SELECT Total FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND Status=2 AND ISNULL(ProjectId,0)=0","@b",b,"@e",employee,"@w",h["WeekStart"]).Rows)total+=AnyminuteReportDurations.Parse(day[0]);
    var names=AnyminuteTimesheetRepair.Query(c,null,"SELECT dbo.FN_DESC('AddEmployee',@e)","@e",employee);
    result.Rows.Add(business["CompanyName"],names.Rows[0][0],employee,Convert.ToDateTime(h["WeekStart"]).ToString("yyyy-MM-dd"),AnyminuteReportDurations.Format(total),total.TotalHours.ToString("0.00",System.Globalization.CultureInfo.InvariantCulture),h["ApprovedBy"],Convert.ToDateTime(h["ApprovedAtUTC"]).ToString("yyyy-MM-ddTHH:mm:ss"));}}
  return result;
 }
 static string Cell(object value){string s=Convert.ToString(value);if(s.Length>0&&"=+-@\t\r".IndexOf(s[0])>=0)s="'"+s;return "\""+s.Replace("\"","\"\"")+"\"";}
 protected void Export_Click(object s,EventArgs e)
 {var rows=GetRows();var text=new StringBuilder();for(int i=0;i<rows.Columns.Count;i++){if(i>0)text.Append(',');text.Append(Cell(rows.Columns[i].ColumnName));}text.AppendLine();foreach(DataRow row in rows.Rows){for(int i=0;i<rows.Columns.Count;i++){if(i>0)text.Append(',');text.Append(Cell(row[i]));}text.AppendLine();}Response.Clear();Response.ContentType="text/csv; charset=utf-8";Response.AddHeader("Content-Disposition","attachment; filename=AnyMinute_Approved_Hours.csv");Response.Write(text.ToString());Response.End();}
}
