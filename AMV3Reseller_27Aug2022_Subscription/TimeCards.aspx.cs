using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Linq;
public partial class AnyminuteTimeCards : System.Web.UI.Page
{
 bool ApprovalQueue { get { return String.Equals(Request.QueryString["status"],"Submitted",StringComparison.OrdinalIgnoreCase); } }
 protected void Page_Load(object s,EventArgs e)
 {
  AnyminuteModernScope.Login();
  if (!IsPostBack)
  {
   DateTime chosen;string selected=Request.QueryString["Week"]??Convert.ToString(Session["AnyminuteSelectedWeek"]);
   Week.Text=AnyminuteWeekService.Monday(DateTime.TryParseExact(selected,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out chosen)?chosen:DateTime.Today).ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
   string initial=Request.QueryString["status"];
   State.SelectedValue=State.Items.FindByValue(initial ?? "All")!=null ? (initial ?? "All") : "All";
  }
  if(!IsPostBack&&!ApprovalQueue&&EmployeeOnly()){Heading.Text="My Time Cards";Intro.Text="Your weekly time cards with daily details and approval status.";} // v29 (employee PPT)
  Page.Title=ApprovalQueue?"Approvals":Heading.Text;
  if (ApprovalQueue) { State.SelectedValue="Submitted";State.Enabled=false; }
  if(!IsPostBack)Bind();
 }
 // v29: employee-only = every business role is plain employee (Tier 0: not owner/admin/manager/accountant, supervises nobody).
 static bool EmployeeOnly()
 {
  try{var scope=AnyminuteReports.Scope();return scope.Count>0&&scope.All(b=>b.Tier==0);}
  catch(Exception ex){log4net.LogManager.GetLogger("Log").Warn("TimeCards role check failed",ex);return false;}
 }
 protected void FilterChanged(object s,EventArgs e){Bind();}
 void Bind()
 {
  Notice.Visible=false;Notice.Text="";DateTime date;
  if(!DateTime.TryParseExact(Week.Text,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out date))
  {Notice.Visible=true;Notice.Text="Enter a valid week date.";Rows.DataSource=null;Rows.DataBind();Empty.Visible=false;return;}
  DateTime week=AnyminuteWeekService.Monday(date);Week.Text=week.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
  var result=new DataTable();foreach(string n in new[]{"EmpName","BusinessName","NetDisplay","State"})result.Columns.Add(n);
  foreach(string n in new[]{"EmployeeID","BusinessID"})result.Columns.Add(n,typeof(int));result.Columns.Add("WeekStart",typeof(DateTime));
  string wanted=ApprovalQueue?"Submitted":State.SelectedValue;var notSubmitted=new System.Collections.Generic.List<string>();
  int onlyBusiness;bool filterBusiness=int.TryParse(Request.QueryString["BusinessId"],out onlyBusiness)&&onlyBusiness>0;
  foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)
  {
   int business=Convert.ToInt32(b["BusinessID"]);
   if(filterBusiness&&business!=onlyBusiness)continue; // v34: honor ?BusinessId= (still limited to the user's scope + Authorize)
   using(var c=AnyminuteTimesheetRepair.Open())
   {
    int role,actor=AnyminuteModernScope.Actor(c,null,business,out role);
    foreach(DataRow employeeRow in AnyminuteModernScope.Users(business).Rows)
    {
     int employee=Convert.ToInt32(employeeRow["EmployeeID"]);
     try
     {
      if(ApprovalQueue)AnyminuteTimesheetRepair.Authorize(c,null,Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),business,employee,false);
      else AnyminuteTimesheetRepair.AuthorizeRead(c,null,Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),business,employee);
     }
     catch(InvalidOperationException) {continue;}
     // Listing cards must not create Draft headers as a side effect of browsing.
     var header=AnyminuteTimesheetRepair.Query(c,null,
      "SELECT State FROM dbo.AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w",
      "@b",business,"@e",employee,"@w",week);
     string state=header.Rows.Count==0?"Draft":Convert.ToString(header.Rows[0][0]);
     if(ApprovalQueue && state!="Submitted")
     {
      // v22 (Pandey #5): explain an empty queue - time entered but never submitted stays Draft and is not an approval.
      if(Convert.ToInt32(AnyminuteTimesheetRepair.Query(c,null,"SELECT COUNT(*) FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0","@b",business,"@e",employee,"@w",week).Rows[0][0])>0)
       notSubmitted.Add(Convert.ToString(employeeRow["EmpName"])+" ("+Convert.ToString(b["CompanyName"])+", "+state+")");
      continue;
     }
     if(wanted!="All" && state!=wanted)continue;
     TimeSpan total=TimeSpan.Zero;
     foreach(DataRow row in AnyminuteTimesheetRepair.Query(c,null,
      "SELECT Total FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0",
      "@b",business,"@e",employee,"@w",week).Rows)total+=AnyminuteReportDurations.Parse(row[0]);
     result.Rows.Add(employeeRow["EmpName"],b["CompanyName"],AnyminuteReportDurations.Format(total),state,employee,business,week);
    }
   }
  }
  Rows.DataSource=result;Rows.DataBind();Empty.Visible=result.Rows.Count==0;
  Empty.Text=ApprovalQueue?"No submitted weeks awaiting your approval for this week. Draft cards appear under Time Cards.":"No timecards match the selected week and status.";
  if(ApprovalQueue && notSubmitted.Count>0)
  {Notice.Visible=true;Notice.Text=HttpUtility.HtmlEncode("Time is entered but the week is not submitted yet for: "+String.Join("; ",notSubmitted.Distinct().ToArray())+
    ". Open the Weekly Time Card and click Submit Week - only submitted weeks appear here.");}
 }
}
