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
    table.Rows.Add(date,date.ToString("dddd"),day==null?"":AttendanceValue(day["Worked"]),day==null||day["ReqularStart"]==DBNull.Value?"":Convert.ToDateTime(day["ReqularStart"]).ToString("HH:mm"),day==null||day["Regularend"]==DBNull.Value?"":Convert.ToDateTime(day["Regularend"]).ToString("HH:mm"),
     AnyminuteReportDurations.Format(dayUnpaid)+" / "+AnyminuteReportDurations.Format(dayPaid),day==null?"-":AnyminuteReportDurations.Format(hours),status,
     estimated?"<small class='am-muted'>Estimated clock-out "+Convert.ToDateTime(estimates.Rows[0][0]).ToString("HH:mm")+" \u00b7 Needs review</small>":"",
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
  Save.Visible=edit&&(state=="Draft"||state=="Rejected");SaveTop.Visible=Save.Visible;Copy.Visible=Save.Visible;Submit.Visible=own&&(state=="Draft"||state=="Rejected");SubmitTop.Visible=Submit.Visible;Approve.Visible=Reject.Visible=review&&state=="Submitted";Reopen.Visible=AnyminuteModernScope.Owns(Business)&&state=="Approved";
  Comment.ReadOnly=!review&&!Reopen.Visible;Comment.Text=Convert.ToString(h["ManagerComment"]);ApprovalInfo.Text=h["ApprovedAtUTC"]==DBNull.Value?"":"<p class='am-muted'>Approved at "+Convert.ToDateTime(h["ApprovedAtUTC"]).ToString("yyyy-MM-dd HH:mm")+" UTC \u00b7 reviewer ID "+Convert.ToString(h["ApprovedBy"])+"</p>";
 }
 protected void Save_Click(object s,EventArgs e)
 {
  Run(()=>{var input=new List<AnyminuteDraftDay>();foreach(RepeaterItem row in Days.Items)
   {DateTime date=DateTime.ParseExact(((HiddenField)row.FindControl("DateKey")).Value,"yyyy-MM-dd",CultureInfo.InvariantCulture);string worked=((DropDownList)row.FindControl("Worked")).SelectedValue;string start=((TextBox)row.FindControl("Start")).Text,end=((TextBox)row.FindControl("End")).Text;
    var d=new AnyminuteDraftDay {Date=date,Worked=worked==""?(bool?)null:worked=="Yes"};if(d.Worked==true){TimeSpan t;if(start!=""){if(!TryTime(start,out t))throw new FormatException();d.Start=date+t;}if(end!=""){if(!TryTime(end,out t))throw new FormatException();d.End=date+t+(((CheckBox)row.FindControl("NextDay")).Checked?TimeSpan.FromDays(1):TimeSpan.Zero);}}input.Add(d);}
   AnyminuteWeekService.SaveDraft(Business,Target,StartWeek,Revision,input);}, "Draft saved. Use Submit Week to send it for approval.");
 }
 void Transition(string action)
 {
  string feedback=action=="Submit"?"Week submitted. Editing is locked until it is returned. Another authorized reviewer must approve your own timecard.":action=="Approve"?"Week approved and locked.":action=="Reject"?"Week returned for correction.":"Week reopened for correction.";
  Run(()=>AnyminuteWeekService.Transition(Business,Target,StartWeek,Revision,action,Comment.Text),feedback);
 }
 protected void Submit_Click(object s,EventArgs e){Transition("Submit");}protected void Approve_Click(object s,EventArgs e){Transition("Approve");}protected void Reject_Click(object s,EventArgs e){Transition("Reject");}protected void Reopen_Click(object s,EventArgs e){Transition("Reopen");}
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
