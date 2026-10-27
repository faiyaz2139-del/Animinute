using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteWeekTimeCard : System.Web.UI.Page
{
 int Business {get{return Int32.Parse(Request.QueryString["BusinessId"]);}}
 int Target {get{return Int32.Parse(Employee.SelectedValue);}}
 // v16: a cleared/invalid week box falls back to the current week instead of a server error.
 DateTime StartWeek {get{DateTime d;return AnyminuteWeekService.Monday(DateTime.TryParseExact(Week.Text,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out d)?d:DateTime.Today);}}
 static bool TryTime(string text,out TimeSpan t){return TimeSpan.TryParseExact((text??"").Trim(),new[]{@"hh\:mm",@"hh\:mm\:ss",@"h\:mm"},CultureInfo.InvariantCulture,out t);}
 static bool PlanTime(object v,out TimeSpan t)
 {
  t=TimeSpan.Zero;if(v==null||v==DBNull.Value)return false;if(v is TimeSpan){t=(TimeSpan)v;return true;}if(v is DateTime){t=((DateTime)v).TimeOfDay;return true;}
  DateTime d;if(DateTime.TryParse(Convert.ToString(v),CultureInfo.InvariantCulture,DateTimeStyles.None,out d)){t=d.TimeOfDay;return true;}return TimeSpan.TryParse(Convert.ToString(v),out t);
 }
 int Revision {get{return Convert.ToInt32(ViewState["Revision"]);}}
 protected void Page_Init(object s,EventArgs e)
 {
  AnyminuteModernScope.Login();ViewStateUserKey=Session.SessionID;
  // v16: opened without ?BusinessId (nav, bookmarks, quick actions) -> first accessible business instead of HTTP 400.
  int b;if(!Int32.TryParse(Request.QueryString["BusinessId"],out b))
  {
   var list=AnyminuteModernScope.Businesses();
   if(list.Rows.Count==0)throw new HttpException(403,"No business is available for your account.");
   string extra=Request.QueryString["EmployeeId"]!=null?"&EmployeeId="+HttpUtility.UrlEncode(Request.QueryString["EmployeeId"]):"";
   if(Request.QueryString["Week"]!=null)extra+="&Week="+HttpUtility.UrlEncode(Request.QueryString["Week"]);
   Response.Redirect("~/WeekTimeCard.aspx?BusinessId="+Convert.ToInt32(list.Rows[0]["BusinessID"])+extra,false);Context.ApplicationInstance.CompleteRequest();return;
  }
 }
 protected void Page_Load(object s,EventArgs e)
 {
  int qb;if(!Int32.TryParse(Request.QueryString["BusinessId"],out qb))return; // redirecting (Page_Init)
  // v16: the Manage Breaks panel only stays open for break actions; it was re-opening on Save/Submit after being closed.
  if(IsPostBack){BreakModal.Visible=false;return;}
  var businesses=AnyminuteModernScope.Businesses();
  foreach(DataRow r in businesses.Rows)BusinessPick.Items.Add(new ListItem(Convert.ToString(r["CompanyName"]),Convert.ToString(r["BusinessID"])));
  if(BusinessPick.Items.FindByValue(qb.ToString())==null)throw new HttpException(403,"This business is outside your access.");
  BusinessPick.SelectedValue=qb.ToString();BusinessField.Visible=BusinessPick.Items.Count>1;
  var users=AnyminuteModernScope.Users(Business);Employee.DataSource=users;Employee.DataTextField="EmpName";Employee.DataValueField="EmployeeID";Employee.DataBind();
  if(Employee.Items.Count==0)throw new HttpException(403,"No accessible timecard in this business.");
  string selected=Request.QueryString["EmployeeId"];if(selected!=null){if(Employee.Items.FindByValue(selected)==null)throw new HttpException(403,"Employee is outside your access.");Employee.SelectedValue=selected;}
  else {using(var c=AnyminuteTimesheetRepair.Open()){int role,a=AnyminuteModernScope.Actor(c,null,Business,out role);if(Employee.Items.FindByValue(a.ToString())!=null)Employee.SelectedValue=a.ToString();}}
  DateTime week;Week.Text=DateTime.TryParseExact(Request.QueryString["Week"],"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out week)?AnyminuteWeekService.Monday(week).ToString("yyyy-MM-dd"):AnyminuteWeekService.Monday(AnyminuteClockEvidence.Today(Business)).ToString("yyyy-MM-dd");Bind();AnyminuteClockEvidence.Record(Business,"OpenTimeCard");
 }
 protected void SelectionChanged(object s,EventArgs e){BreakModal.Visible=false;Bind();}
 protected void BusinessChanged(object s,EventArgs e)
 {
  int b;if(!Int32.TryParse(BusinessPick.SelectedValue,out b)||b<=0)return;
  Response.Redirect("~/WeekTimeCard.aspx?BusinessId="+b+"&Week="+StartWeek.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture),false);Context.ApplicationInstance.CompleteRequest();
 }
 void FreshSelection(){if(Convert.ToString(ViewState["WeekKey"])!=Target+":"+StartWeek.ToString("yyyy-MM-dd"))throw new InvalidOperationException("Load the selected employee and week before applying an action.");}
 void Run(Action action, string successMessage = null)
 {
  Message.Visible=false;BreakMessage.Visible=false;
  try{FreshSelection();action();Bind();AnyminuteClockEvidence.Record(Business,"TimecardAction");if(successMessage!=null){Message.Text=successMessage;Message.Visible=true;}}
  catch(InvalidOperationException ex){Message.Text=HttpUtility.HtmlEncode(ex.Message);Message.Visible=true;}
  catch(FormatException){Message.Text="Enter valid dates and times.";Message.Visible=true;}
  catch(Exception ex){log4net.LogManager.GetLogger("Log").Error("Weekly timecard action",ex);Message.Text="The action did not complete. Refresh and contact support if it persists.";Message.Visible=true;}
  if(BreakModal.Visible&&Message.Visible){BreakMessage.Text=Message.Text;BreakMessage.Visible=true;}
 }
 static string AttendanceValue(object value)
 {
  string text=Convert.ToString(value).Trim();
  if(text.Length==0)return "";
  if(String.Equals(text,"Yes",StringComparison.OrdinalIgnoreCase))return "Yes";
  if(String.Equals(text,"No",StringComparison.OrdinalIgnoreCase))return "No";
  throw new InvalidOperationException("Unrecognized attendance value. Ask an administrator to review this day.");
 }
 void Bind()
 {
  Week.Text=StartWeek.ToString("yyyy-MM-dd");Session["AnyminuteSelectedWeek"]=Week.Text;AnyminuteClockEvidence.Estimate(Business,Target,StartWeek);
  var h=AnyminuteWeekService.Header(Business,Target,StartWeek).Rows[0];string state=Convert.ToString(h["State"]);ViewState["Revision"]=h["Revision"];ViewState["WeekKey"]=Target+":"+StartWeek.ToString("yyyy-MM-dd");
  Badge.Text="<span class='am-badge "+state+"'>"+state+"</span>";
  bool own,edit,review=false;decimal price=0;bool canPrice=false,completeRates=true;
  using(var c=AnyminuteTimesheetRepair.Open())
  {
   int role,actor=AnyminuteModernScope.Actor(c,null,Business,out role);own=actor==Target;
   edit=own||AnyminuteModernScope.Owns(Business);if(!edit)try{AnyminuteTimesheetRepair.Authorize(c,null,Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,true);edit=true;}catch(InvalidOperationException){}
   try{AnyminuteTimesheetRepair.Authorize(c,null,Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,false);review=true;}catch(InvalidOperationException){}
   canPrice=own||AnyminuteModernScope.Owns(Business);
   var table=new DataTable();table.Columns.Add("Date",typeof(DateTime));foreach(string n in new[]{"Day","Worked","Start","End","BreakDisplay","Net","Status","EstimateNote"})table.Columns.Add(n);
   foreach(string n in new[]{"Editable","NextDay","Reviewable","EstimateAvailable"})table.Columns.Add(n,typeof(bool));TimeSpan net=TimeSpan.Zero,unpaid=TimeSpan.Zero,paid=TimeSpan.Zero,overtime=TimeSpan.Zero;int working=0,approved=0;
   // v21 (Pandey #7): the saved Work Schedule (dbo.AnyminuteWeeklySchedule, same BusinessID + EmployeeID + Mon-Sun week) drives
   // the time card. A day with no time entry yet is pre-filled from the schedule (Worked / Day Off + start/end) and marked
   // "Scheduled"; Save Draft stores it as the employee's time. Days already entered keep their recorded time; the planned
   // shift is shown under the net hours. Nothing is written until the user saves.
   // v22 (Pandey #4): v21 matched the schedule only on the exact EmployeeID. The schedule is now read for the same person
   // (any EmployeeID of the same login in this business, exact ID preferred), dates compared as DATE, and times parsed from
   // time / datetime / text columns. When nothing is found the card says so (with a link) instead of staying silent.
   var plan=new Dictionary<DateTime,DataRow>();bool planTable=AnyminuteEmployeeEditor.HasTable(c,null,AnyminuteWeeklySchedule.Table);
   if(planTable)
    foreach(DataRow p in AnyminuteTimesheetRepair.Query(c,null,@"SELECT s.WorkDate,s.ScheduleType,s.StartTime,s.EndTime,s.BreakMinutes,CASE WHEN s.EmployeeID=@e THEN 0 ELSE 1 END AS Pref
      FROM dbo.AnyminuteWeeklySchedule s WHERE s.BusinessID=@b AND s.WorkDate>=CONVERT(date,@f) AND s.WorkDate<DATEADD(day,7,CONVERT(date,@f))
       AND (s.EmployeeID=@e OR s.EmployeeID IN (SELECT m2.EmployeeID FROM MappingTable m1 JOIN MappingTable m2 ON m2.USERID=m1.USERID AND m2.BusinessID=m1.BusinessID
         WHERE m1.EmployeeID=@e AND m1.BusinessID=@b AND m1.USERID IS NOT NULL))
      ORDER BY Pref DESC,s.WorkDate","@b",Business,"@e",Target,"@f",StartWeek.Date).Rows)
     plan[Convert.ToDateTime(p["WorkDate"]).Date]=p;
   string scheduleUrl="WeeklyEmployeeSchedule.aspx?BusinessId="+Business+"&EmployeeId="+Target+"&week="+StartWeek.ToString("yyyy-MM-dd",CultureInfo.InvariantCulture);
   PlanInfo.Text=!planTable?"<p class='am-notice am-plan-info'>Work Schedule is not installed on this database (run DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql), so planned shifts cannot be shown.</p>"
    :plan.Count==0?"<p class='am-muted am-plan-info'>No saved Work Schedule for this employee and week. <a href='"+HttpUtility.HtmlAttributeEncode(scheduleUrl)+"'>Open Work Schedule</a> and click Save Weekly Schedule to plan it.</p>"
    :"<p class='am-muted am-plan-info'>Work Schedule found for "+plan.Count+" day"+(plan.Count==1?"":"s")+" of this week. Empty days are pre-filled and marked <span class='am-badge Scheduled'>Scheduled</span>; Save changes or Submit Week stores them.</p>";
   for(int i=0;i<7;i++)
   {
    DateTime date=StartWeek.AddDays(i);var rows=AnyminuteTimesheetRepair.Day(c,null,Business,Target,date);if(rows.Rows.Count>1)throw new InvalidOperationException("Duplicate daily entries need administrator review.");
    DataRow day=rows.Rows.Count==0?null:rows.Rows[0];var breaks=AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,date);
    TimeSpan dayUnpaid=TimeSpan.Zero,dayPaid=TimeSpan.Zero;foreach(DataRow br in breaks.Rows){if(Convert.ToBoolean(br["IsPaid"]))dayPaid+=AnyminuteReportDurations.Parse(br["BreakTotal"]);else dayUnpaid+=AnyminuteReportDurations.Parse(br["BreakTotal"]);}unpaid+=dayUnpaid;paid+=dayPaid;
    bool isApproved=day!=null&&Convert.ToString(day["Status"])=="2",isWorked=day!=null&&AttendanceValue(day["Worked"])=="Yes";
    if(isWorked){working++;if(isApproved)approved++;}
    TimeSpan hours=day==null?TimeSpan.Zero:AnyminuteReportDurations.Parse(day["Total"]);net+=hours;
    if(canPrice&&isWorked)
    {var rates=AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP(1) TRY_CONVERT(decimal(18,4),Rate) AS Rate,RateType FROM RateDetails WHERE EmployeeId=@e AND effectivedate<DATEADD(day,1,@d) ORDER BY effectivedate DESC,RateId DESC","@e",Target,"@d",date);
     if(rates.Rows.Count==0||rates.Rows[0]["Rate"]==DBNull.Value||Convert.ToString(rates.Rows[0]["RateType"]).IndexOf("hour",StringComparison.OrdinalIgnoreCase)<0)completeRates=false;
     else price+=(decimal)hours.TotalHours*Convert.ToDecimal(rates.Rows[0]["Rate"]);}

    if(day!=null){overtime+=AnyminuteReportDurations.Parse(day["Over1_5x"])+AnyminuteReportDurations.Parse(day["Over2x"]);}
    var estimates=AnyminuteTimesheetRepair.Query(c,null,"SELECT EstimatedEnd FROM AnyminuteClockOutEstimate WHERE BusinessID=@b AND EmployeeID=@e AND ShiftDate=@d AND State='NeedsReview'","@b",Business,"@e",Target,"@d",date);
    bool estimated=estimates.Rows.Count>0&&day!=null&&day["Regularend"]==DBNull.Value;
    string status=day!=null&&!isWorked&&AttendanceValue(day["Worked"])=="No"?"Day Off":isApproved?"Approved":day!=null&&Convert.ToString(day["Status"])=="3"?"Rejected":state=="Submitted"?"Submitted":day==null?"Not entered":"Draft";
    string worked=day==null?"":AttendanceValue(day["Worked"]),start=day==null||day["ReqularStart"]==DBNull.Value?"":Convert.ToDateTime(day["ReqularStart"]).ToString("HH:mm"),finish=day==null||day["Regularend"]==DBNull.Value?"":Convert.ToDateTime(day["Regularend"]).ToString("HH:mm"),planNote="";
    DataRow sp;if(plan.TryGetValue(date,out sp))
    {
     TimeSpan pst=TimeSpan.Zero,pet=TimeSpan.Zero;bool planWork=String.Equals(Convert.ToString(sp["ScheduleType"]).Trim(),"Working",StringComparison.OrdinalIgnoreCase)&&PlanTime(sp["StartTime"],out pst)&PlanTime(sp["EndTime"],out pet);
     string ps=planWork?DateTime.Today.Add(pst).ToString("HH:mm"):"",pe=planWork?DateTime.Today.Add(pet).ToString("HH:mm"):"";
     int pb=sp["BreakMinutes"]==DBNull.Value?0:Convert.ToInt32(sp["BreakMinutes"]);
     planNote="<small class='am-muted am-plan-note'>Scheduled: "+(planWork?HttpUtility.HtmlEncode(ps+" \u2013 "+pe)+(pb>0?" \u00b7 "+pb+" min break":""):"Day Off")+"</small>";
     if(day==null&&(state=="Draft"||state=="Rejected")){worked=planWork?"Yes":"No";start=ps;finish=pe;status="Scheduled";}
    }
    table.Rows.Add(date,date.ToString("dddd"),worked,start,finish,
     AnyminuteReportDurations.Format(dayUnpaid)+" / "+AnyminuteReportDurations.Format(dayPaid),day==null?"-":AnyminuteReportDurations.Format(hours),status,
     (estimated?"<small class='am-muted'>Estimated clock-out "+Convert.ToDateTime(estimates.Rows[0][0]).ToString("HH:mm")+" \u00b7 Needs review</small>":"")+planNote,
     edit&&(state=="Draft"||state=="Rejected")&&!isApproved,
     day!=null&&day["Regularend"]!=DBNull.Value&&Convert.ToDateTime(day["Regularend"]).Date>date,
     review&&state=="Submitted"&&isWorked&&!isApproved,estimated&&edit&&(state=="Draft"||state=="Rejected"));
   }
   Days.DataSource=table;Days.DataBind();NetHours.Text=AnyminuteReportDurations.Format(net);BreakHours.Text=AnyminuteReportDurations.Format(unpaid)+" / "+AnyminuteReportDurations.Format(paid);Overtime.Text=AnyminuteReportDurations.Format(overtime);
   BasePay.Text=canPrice&&completeRates?"$"+price.ToString("N2",CultureInfo.InvariantCulture):"\u2014";
   ProgressLabel.Text="Working days approved: "+approved+" / "+working;ProgressBar.Text="<div class='am-progress'><span style='width:"+(working==0?0:approved*100/working)+"%'></span></div>";
   Approve.Enabled=approved==working;Approve.ToolTip=Approve.Enabled?"Approve the reviewed week":"Approve each worked day first";
  }
  ViewState["EditableWeek"]=edit&&(state=="Draft"||state=="Rejected");
  Save.Visible=edit&&(state=="Draft"||state=="Rejected");SaveTop.Visible=Save.Visible;Copy.Visible=Save.Visible;Submit.Visible=edit&&(state=="Draft"||state=="Rejected"); // v22: owner/manager entering time can submit it tooSubmitTop.Visible=Submit.Visible;Approve.Visible=Reject.Visible=review&&state=="Submitted";Reopen.Visible=AnyminuteModernScope.Owns(Business)&&state=="Approved";
  Comment.ReadOnly=!review&&!Reopen.Visible;Comment.Text=Convert.ToString(h["ManagerComment"]);ApprovalInfo.Text=h["ApprovedAtUTC"]==DBNull.Value?"":"<p class='am-muted'>Approved at "+Convert.ToDateTime(h["ApprovedAtUTC"]).ToString("yyyy-MM-dd HH:mm")+" UTC \u00b7 reviewer ID "+Convert.ToString(h["ApprovedBy"])+"</p>";
 }
 void SaveGrid()
 {
  var input=new List<AnyminuteDraftDay>();foreach(RepeaterItem row in Days.Items)
   {DateTime date=DateTime.ParseExact(((HiddenField)row.FindControl("DateKey")).Value,"yyyy-MM-dd",CultureInfo.InvariantCulture);string worked=((DropDownList)row.FindControl("Worked")).SelectedValue;string start=((TextBox)row.FindControl("Start")).Text,end=((TextBox)row.FindControl("End")).Text;
    var d=new AnyminuteDraftDay {Date=date,Worked=worked==""?(bool?)null:worked=="Yes"};if(d.Worked==true){TimeSpan t;if(start!=""){if(!TryTime(start,out t))throw new FormatException();d.Start=date+t;}if(end!=""){if(!TryTime(end,out t))throw new FormatException();d.End=date+t+(((CheckBox)row.FindControl("NextDay")).Checked?TimeSpan.FromDays(1):TimeSpan.Zero);}}input.Add(d);}
  AnyminuteWeekService.SaveDraft(Business,Target,StartWeek,Revision,input);
 }
 protected void Save_Click(object s,EventArgs e)
 {
  Run(SaveGrid,"Draft saved. Use Submit Week to send it for approval.");
 }
 void Transition(string action)
 {
  string feedback=action=="Submit"?"Week submitted. Editing is locked until it is returned. Another authorized reviewer must approve your own timecard.":action=="Approve"?"Week approved and locked.":action=="Reject"?"Week returned for correction.":"Week reopened for correction.";
  Run(()=>AnyminuteWeekService.Transition(Business,Target,StartWeek,Revision,action,Comment.Text),feedback);
 }
 // v22 (Pandey #5): Submit Week first saves what is on screen (times typed but not saved were lost before, so the week
 // stayed Draft and never reached Approvals), then submits with the fresh revision.
 protected void Submit_Click(object s,EventArgs e)
 {
  Run(()=>{if(Convert.ToBoolean(ViewState["EditableWeek"]))SaveGrid();
   int fresh=Convert.ToInt32(AnyminuteWeekService.Header(Business,Target,StartWeek).Rows[0]["Revision"]);
   AnyminuteWeekService.Transition(Business,Target,StartWeek,fresh,"Submit","");},
   "Week saved and submitted. It is now listed under Approvals (Team Timesheets \u203a Submitted) for the authorized reviewer.");
 }protected void Approve_Click(object s,EventArgs e){Transition("Approve");}protected void Reject_Click(object s,EventArgs e){Transition("Reject");}protected void Reopen_Click(object s,EventArgs e){Transition("Reopen");}
 protected void Copy_Click(object s,EventArgs e)
 {Run(()=>{var dates=Enumerable.Range(1,4).Select(i=>StartWeek.AddDays(i)).ToArray();AnyminuteTimesheetRepair.Copy(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,dates.Select(d=>StartWeek).ToArray(),dates,false);},"Monday copied to the empty weekdays. Review the times, then Save changes.");}
 protected void DayCommand(object s,RepeaterCommandEventArgs e)
 {
  DateTime day=DateTime.ParseExact(Convert.ToString(e.CommandArgument),"yyyy-MM-dd",CultureInfo.InvariantCulture);if(day<StartWeek||day>=StartWeek.AddDays(7))throw new HttpException(400,"Invalid day.");
  if(e.CommandName=="ApproveDay")Run(()=>AnyminuteWeekService.ApproveDay(Business,Target,day,Revision));
  if(e.CommandName=="ConfirmEstimate")Run(()=>AnyminuteClockEvidence.Confirm(Business,Target,day));
  if(e.CommandName=="Breaks")Run(()=>{ViewState["BreakDay"]=day;ClearBreakEditor();OpenBreaks();});
 }
 protected bool CanEditBreaks {get{return Convert.ToBoolean(ViewState["EditableWeek"]);}}
 void OpenBreaks()
 {
  DateTime day=Convert.ToDateTime(ViewState["BreakDay"]);BreakDay.Text=day.ToString("dddd, MMMM d, yyyy");BreakModal.Visible=true;
  SaveBreak.Visible=CanEditBreaks;CancelBreakEdit.Visible=CanEditBreaks&&ViewState["BreakID"]!=null;
  SaveBreak.Text=ViewState["BreakID"]==null?"\uff0b Add Break":"Save Break Changes";BreakStart.Enabled=BreakEnd.Enabled=Paid.Enabled=CanEditBreaks;
  var breaks=AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,day);
  BreakRows.DataSource=breaks;BreakRows.DataBind();TimeSpan unpaid=TimeSpan.Zero,paid=TimeSpan.Zero;
  foreach(DataRow br in breaks.Rows){var duration=AnyminuteReportDurations.Parse(br["BreakTotal"]);if(Convert.ToBoolean(br["IsPaid"]))paid+=duration;else unpaid+=duration;}
  BreakUnpaid.Text=TotalUnpaidBreak.Text=AnyminuteReportDurations.Format(unpaid);BreakPaid.Text=TotalPaidBreak.Text=AnyminuteReportDurations.Format(paid);
  BreakWorkTime.Text=BreakGross.Text=BreakNet.Text="\u2014";
  using(var c=AnyminuteTimesheetRepair.Open())
  {AnyminuteWeekService.ReadAccess(c,null,Business,Target);var rows=AnyminuteTimesheetRepair.Day(c,null,Business,Target,day);
   if(rows.Rows.Count==1&&rows.Rows[0]["ReqularStart"]!=DBNull.Value&&rows.Rows[0]["Regularend"]!=DBNull.Value)
   {DateTime a=Convert.ToDateTime(rows.Rows[0]["ReqularStart"]),z=Convert.ToDateTime(rows.Rows[0]["Regularend"]);
    BreakWorkTime.Text=a.ToString("HH:mm")+" \u2013 "+z.ToString("HH:mm")+(z.Date>a.Date?" (+1 day)":"");
    BreakGross.Text=AnyminuteReportDurations.Format(z-a);BreakNet.Text=AnyminuteReportDurations.Format(z-a-unpaid);}}
 }
 void ClearBreakEditor(){ViewState.Remove("BreakID");BreakStart.Text=BreakEnd.Text="";Paid.SelectedValue="0";}
 protected void CancelBreakEdit_Click(object s,EventArgs e){Run(()=>{ClearBreakEditor();OpenBreaks();});}
 protected void SaveBreak_Click(object s,EventArgs e)
 {Run(()=>{DateTime day=Convert.ToDateTime(ViewState["BreakDay"]);int? id=ViewState["BreakID"]==null?(int?)null:Convert.ToInt32(ViewState["BreakID"]);
  TimeSpan a,z;if(!TryTime(BreakStart.Text,out a)||!TryTime(BreakEnd.Text,out z))throw new FormatException();
  DateTime start=day.Date+a,end=day.Date+z;
  using(var c=AnyminuteTimesheetRepair.Open())
  {AnyminuteWeekService.ReadAccess(c,null,Business,Target);var rows=AnyminuteTimesheetRepair.Day(c,null,Business,Target,day);
   if(rows.Rows.Count!=1||rows.Rows[0]["ReqularStart"]==DBNull.Value||rows.Rows[0]["Regularend"]==DBNull.Value)throw new InvalidOperationException("Save a complete shift before adding breaks.");
   DateTime shiftStart=Convert.ToDateTime(rows.Rows[0]["ReqularStart"]);if(a<shiftStart.TimeOfDay)start=start.AddDays(1);if(z<shiftStart.TimeOfDay)end=end.AddDays(1);if(end<start)end=end.AddDays(1);}
  AnyminuteTimesheetRepair.ChangeBreak(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,day,id,start,end,false,Paid.SelectedValue=="1");ClearBreakEditor();OpenBreaks();});}
 protected void BreakCommand(object s,RepeaterCommandEventArgs e)
 {Run(()=>{int id=Int32.Parse(Convert.ToString(e.CommandArgument));DateTime day=Convert.ToDateTime(ViewState["BreakDay"]);
  if(e.CommandName=="Delete"){AnyminuteTimesheetRepair.ChangeBreak(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,day,id,null,null,true);ClearBreakEditor();}
  else {var rows=AnyminuteTimesheetRepair.ReadBreaks(Convert.ToInt32(Session["EmployeeId"]),Convert.ToInt32(Session["UserTypeid"]),Business,Target,day).Select("BreakID="+id);if(rows.Length!=1)throw new InvalidOperationException("Break changed. Refresh first.");ViewState["BreakID"]=id;BreakStart.Text=Convert.ToDateTime(rows[0]["BreakStart"]).ToString("HH:mm");BreakEnd.Text=Convert.ToDateTime(rows[0]["BreakEnd"]).ToString("HH:mm");Paid.SelectedValue=Convert.ToBoolean(rows[0]["IsPaid"])?"1":"0";}
  OpenBreaks();});}
}
