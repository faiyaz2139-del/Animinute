using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
public class AnyminuteDraftDay
{
 public DateTime Date; public bool? Worked; public DateTime? Start,End;
}
public static class AnyminuteWeekService
{
 public static DateTime Monday(DateTime date) {return date.Date.AddDays(-(((int)date.DayOfWeek+6)%7));}
 static int SessionActor {get{return Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]);}}
 static int SessionRole {get{return Convert.ToInt32(HttpContext.Current.Session["UserTypeid"]);}}
 public static void ReadAccess(SqlConnection c,SqlTransaction tx,int b,int e) {AnyminuteTimesheetRepair.AuthorizeRead(c,tx,SessionActor,SessionRole,b,e);}
 public static DataRow Header(SqlConnection c,SqlTransaction tx,int b,int e,DateTime week)
 {
  week=Monday(week);
  var rows=AnyminuteTimesheetRepair.Query(c,tx,@"SELECT * FROM AnyminuteWeek WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@b",b,"@e",e,"@w",week);
  if(rows.Rows.Count==0)
  {
   var old=AnyminuteTimesheetRepair.Query(c,tx,"SELECT Status FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0","@b",b,"@e",e,"@w",week);
   bool approved=old.Rows.Count==7 && old.Rows.Cast<DataRow>().All(r=>Convert.ToString(r["Status"])=="2");
   AnyminuteTimesheetRepair.Execute(c,tx,"INSERT AnyminuteWeek(BusinessID,EmployeeID,WeekStart,State,ChangedAtUTC) VALUES(@b,@e,@w,@s,SYSUTCDATETIME())","@b",b,"@e",e,"@w",week,"@s",approved?"Approved":"Draft");
   rows=AnyminuteTimesheetRepair.Query(c,tx,"SELECT * FROM AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@b",b,"@e",e,"@w",week);
  }
  return rows.Rows[0];
 }
 public static DataTable Header(int b,int e,DateTime week)
 {using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable)) {ReadAccess(c,tx,b,e);var result=Header(c,tx,b,e,week).Table.Copy();tx.Commit();return result;}}
 public static void EnsureEditable(SqlConnection c,SqlTransaction tx,int b,int e,DateTime date)
 {
  var state=AnyminuteTimesheetRepair.Query(c,tx,"SELECT State FROM AnyminuteWeek WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@b",b,"@e",e,"@w",Monday(date));
  if(state.Rows.Count>0 && (Convert.ToString(state.Rows[0][0])=="Submitted"||Convert.ToString(state.Rows[0][0])=="Approved"))
   throw new InvalidOperationException("This week is submitted or approved. Return or reopen it before editing.");
 }
 static void Revision(DataRow h,int revision)
 {if(Convert.ToInt32(h["Revision"])!=revision)throw new InvalidOperationException("This card changed in another session. Refresh before saving.");}
 static void Audit(SqlConnection c,SqlTransaction tx,int b,int e,DateTime date,int actor,string action,string before,string after)
 {AnyminuteTimesheetRepair.Execute(c,tx,"INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,TimeSheetDate,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,@d,@action,@before,@after,SYSUTCDATETIME())","@b",b,"@a",actor,"@e",e,"@d",date.Date,"@action",action,"@before",before,"@after",after);}
 public static void SaveDraft(int b,int e,DateTime week,int revision,IEnumerable<AnyminuteDraftDay> input)
 {
  week=Monday(week);var days=input.ToArray();if(days.Length!=7||days.Select(d=>d.Date.Date).Distinct().Count()!=7||days.Any(d=>d.Date<week||d.Date>=week.AddDays(7)))throw new InvalidOperationException("Invalid weekly dates.");
  using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
  {
   AnyminuteTimesheetRepair.Authorize(c,tx,SessionActor,SessionRole,b,e,true);var h=Header(c,tx,b,e,week);Revision(h,revision);EnsureEditable(c,tx,b,e,week);
   foreach(var day in days)
   {
    if(!day.Worked.HasValue)continue;
    var rows=AnyminuteTimesheetRepair.Day(c,tx,b,e,day.Date);
    if(rows.Rows.Count==1&&Convert.ToString(rows.Rows[0]["Status"])=="2")continue;
    AnyminuteTimesheetRepair.EnsureUnlocked(c,tx,b,e,day.Date,rows);
    var breaks=AnyminuteTimesheetRepair.Breaks(c,tx,b,e,day.Date);TimeSpan gross=TimeSpan.Zero,unpaid=TimeSpan.Zero;
    bool complete=day.Worked.Value&&day.Start.HasValue&&day.End.HasValue;
    if(complete)
    {
     if(day.Start.Value.Date!=day.Date.Date||day.End.Value<=day.Start.Value||day.End.Value-day.Start.Value>=TimeSpan.FromDays(1))throw new InvalidOperationException("Invalid shift on "+day.Date.ToString("MMM d")+".");
     gross=day.End.Value-day.Start.Value;unpaid=AnyminuteTimesheetRepair.ValidateSavedBreaks(breaks,day.Start.Value,day.End.Value);
    }
    else if(breaks.Rows.Count>0)throw new InvalidOperationException("A day with breaks needs a complete shift.");
    string sql=rows.Rows.Count==0?@"INSERT TimeSheetEntry(EmpId,BussID,Month,Year,WeekNo,TimeSheetDate,Day,ReqularStart,Regularend,Regular,BreakStart,BreakEnd,BreakTotal,Over1_5x,Over2x,Holiday,Total,Status,Worked,ProjectId)
     VALUES(@e,@b,@m,@y,@wn,@d,@day,@s,@f,@gross,'','',@break,'00:00:00','00:00:00','00:00:00',@net,@status,@worked,0)":
     @"UPDATE TimeSheetEntry SET ReqularStart=@s,Regularend=@f,Regular=@gross,BreakTotal=@break,Total=@net,Status=@status,Worked=@worked
     WHERE TimeSheetEntryId=@id AND BussID=@b AND EmpId=@e AND ISNULL(ProjectId,0)=0";
    AnyminuteTimesheetRepair.Execute(c,tx,sql,"@id",rows.Rows.Count==0?(object)DBNull.Value:rows.Rows[0]["TimeSheetEntryId"],"@b",b,"@e",e,"@m",day.Date.Month,"@y",day.Date.Year,
     "@wn",System.Globalization.CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(day.Date,System.Globalization.CalendarWeekRule.FirstDay,DayOfWeek.Sunday),"@d",day.Date.Date,"@day",day.Date.DayOfWeek.ToString(),
     "@s",day.Worked.Value?(object)day.Start:DBNull.Value,"@f",day.Worked.Value?(object)day.End:DBNull.Value,
     "@gross",complete?gross.ToString():day.Worked.Value?"":"00:00:00","@break",unpaid.ToString(),"@net",complete?(gross-unpaid).ToString():day.Worked.Value?"":"00:00:00","@status",day.Worked.Value?1:4,"@worked",day.Worked.Value?"Yes":"No");
   }
   AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AnyminuteWeek SET State='Draft',Revision=Revision+1,ChangedAtUTC=SYSUTCDATETIME() WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@b",b,"@e",e,"@w",week);
   Audit(c,tx,b,e,week,SessionActor,"SaveDraft",Convert.ToString(h["State"]),"Draft");tx.Commit();
  }
 }
 public static void Validate(SqlConnection c,SqlTransaction tx,int b,int e,DateTime week)
 {
  for(int i=0;i<7;i++)
  {
   DateTime date=week.AddDays(i);var rows=AnyminuteTimesheetRepair.Day(c,tx,b,e,date);
   if(rows.Rows.Count!=1)throw new InvalidOperationException("Complete "+date.ToString("dddd, MMM d")+" or mark it Not worked before submitting.");
   var r=rows.Rows[0];var breaks=AnyminuteTimesheetRepair.Breaks(c,tx,b,e,date);
   if(Convert.ToString(r["Worked"])=="Yes")
   {
    if(r["ReqularStart"]==DBNull.Value||r["Regularend"]==DBNull.Value)throw new InvalidOperationException("Missing clock time on "+date.ToString("MMM d")+".");
    DateTime start=Convert.ToDateTime(r["ReqularStart"]),end=Convert.ToDateTime(r["Regularend"]);
    if(start.Date!=date.Date||end<=start||end-start>=TimeSpan.FromDays(1))throw new InvalidOperationException("Invalid saved shift.");
    var unpaid=AnyminuteTimesheetRepair.ValidateSavedBreaks(breaks,start,end);
    if(AnyminuteReportDurations.Parse(r["Regular"])!=end-start||AnyminuteReportDurations.Parse(r["BreakTotal"])!=unpaid||AnyminuteReportDurations.Parse(r["Total"])!=end-start-unpaid)throw new InvalidOperationException("Saved totals do not reconcile.");
   }
   else if(Convert.ToString(r["Worked"])!="No"||breaks.Rows.Count>0||AnyminuteReportDurations.Parse(r["Regular"])!=TimeSpan.Zero||AnyminuteReportDurations.Parse(r["BreakTotal"])!=TimeSpan.Zero||AnyminuteReportDurations.Parse(r["Total"])!=TimeSpan.Zero)
    throw new InvalidOperationException("Invalid absence record.");
  }
 }
 static void Notify(SqlConnection c,SqlTransaction tx,int b,int e,DateTime w,string kind,bool reviewers)
 {
  string recipients=reviewers?@"SELECT DISTINCT u.UserId FROM UserAcc u JOIN AssginBussiness a ON a.EmployeeID=u.EmployeeId WHERE a.BusinessID=@b AND a.UserTypeId IN(1,5)
   UNION SELECT DISTINCT m.USERID FROM MappingTable m JOIN AssginBussiness a ON a.EmployeeID=m.EmployeeID AND a.BusinessID=m.BusinessID AND a.UserTypeId=m.UserTypeId
    WHERE m.BusinessID=@b AND (m.UserTypeId IN(1,5) OR EXISTS(SELECT 1 FROM AssgineManager r WHERE r.BusinessID=@b AND r.ManagerID=m.EmployeeID AND r.EmpId=@e))":
   "SELECT DISTINCT USERID FROM MappingTable WHERE BusinessID=@b AND EmployeeID=@e";
  foreach(DataRow recipient in AnyminuteTimesheetRepair.Query(c,tx,recipients,"@b",b,"@e",e).Rows)
   AnyminuteTimesheetRepair.Execute(c,tx,"INSERT AnyminuteNotification(RecipientUserID,BusinessID,EmployeeID,WeekStart,Kind,Message,CreatedAtUTC) VALUES(@u,@b,@e,@w,@kind,@message,SYSUTCDATETIME())","@u",recipient[0],"@b",b,"@e",e,"@w",w,"@kind",kind,"@message",kind+" timecard for "+w.ToString("MMM d, yyyy")+".");
 }
 public static void Transition(int b,int e,DateTime week,int revision,string action,string comment)
 {
  week=Monday(week);comment=(comment??"").Trim();if(comment.Length>1000)throw new InvalidOperationException("Keep comments within 1,000 characters.");
  using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
  {
   var h=Header(c,tx,b,e,week);Revision(h,revision);string before=Convert.ToString(h["State"]),after;int resolved,role;
   if(action=="Submit")
   {
    resolved=AnyminuteModernScope.Actor(c,tx,b,out role);
    if(resolved!=e)throw new InvalidOperationException("Only the employee can submit their week.");
    if(before!="Draft"&&before!="Rejected")throw new InvalidOperationException("Only a draft or returned week can be submitted.");
    Validate(c,tx,b,e,week);after="Submitted";
   }
   else
   {
    if(action=="Reopen")AnyminuteApprovalAssignments.RequireOwner(c,tx,b);
    else AnyminuteTimesheetRepair.Authorize(c,tx,SessionActor,SessionRole,b,e,false);
    resolved=AnyminuteModernScope.Actor(c,tx,b,out role);
    if(action=="Reopen")
    {AnyminuteApprovalAssignments.RequireOwner(c,tx,b);if(before!="Approved")throw new InvalidOperationException("Only an approved week can be reopened.");after="Rejected";if(comment.Length==0)throw new InvalidOperationException("Explain why this week is being reopened.");}
    else
    {if(before!="Submitted")throw new InvalidOperationException("Only a submitted week can be reviewed.");after=action=="Approve"?"Approved":"Rejected";
     if(action!="Approve"&&action!="Reject")throw new InvalidOperationException("Invalid action.");
     if(after=="Rejected"&&comment.Length==0)throw new InvalidOperationException("A comment is required when returning a week.");
     if(after=="Approved")
     {Validate(c,tx,b,e,week);
      var pending=AnyminuteTimesheetRepair.Query(c,tx,"SELECT COUNT(*) FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0 AND Worked='Yes' AND ISNULL(Status,0)<>2","@b",b,"@e",e,"@w",week);
      if(Convert.ToInt32(pending.Rows[0][0])>0)throw new InvalidOperationException("Approve each worked day before approving the week.");}}
   }
   after=AnyminuteWeekRules.Next(before,action,resolved==e,action!="Submit"&&action!="Reopen",action=="Reopen",comment);
   // Trusted workflow operation owns this connection and clears the trigger marker before returning it to the pool.
   AnyminuteTimesheetRepair.Execute(c,tx,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=1");
   try
   {
    AnyminuteTimesheetRepair.Execute(c,tx,@"UPDATE AnyminuteWeek SET State=@state,Revision=Revision+1,ChangedAtUTC=SYSUTCDATETIME(),
      SubmittedAtUTC=CASE WHEN @state='Submitted' THEN SYSUTCDATETIME() ELSE SubmittedAtUTC END,
      ApprovedAtUTC=CASE WHEN @state='Approved' THEN SYSUTCDATETIME() WHEN @state='Rejected' THEN NULL ELSE ApprovedAtUTC END,
      ApprovedBy=CASE WHEN @state='Approved' THEN @actor WHEN @state='Rejected' THEN NULL ELSE ApprovedBy END,
      ManagerComment=CASE WHEN @state='Submitted' THEN ManagerComment ELSE @comment END
      WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@state",after,"@b",b,"@e",e,"@w",week,"@actor",resolved,"@comment",comment);
    if(after=="Approved"||after=="Rejected") AnyminuteTimesheetRepair.Execute(c,tx,
      "UPDATE TimeSheetEntry SET Status=@s WHERE BussID=@b AND EmpId=@e AND TimeSheetDate>=@w AND TimeSheetDate<DATEADD(day,7,@w) AND ISNULL(ProjectId,0)=0",
      "@s",after=="Approved"?2:3,"@b",b,"@e",e,"@w",week);
    Audit(c,tx,b,e,week,resolved,action+"Week",before,after);Notify(c,tx,b,e,week,after,after=="Submitted");
    AnyminuteTimesheetRepair.Execute(c,tx,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=NULL");tx.Commit();
   }
   catch {try{tx.Rollback();}catch{} try{AnyminuteTimesheetRepair.Execute(c,null,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=NULL");}catch{SqlConnection.ClearPool(c);}throw;}
  }
 }
 public static void ApproveDay(int b,int e,DateTime date,int revision)
 {
  var week=Monday(date);
  using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
  {
   AnyminuteTimesheetRepair.Authorize(c,tx,SessionActor,SessionRole,b,e,false);var h=Header(c,tx,b,e,week);Revision(h,revision);
   if(Convert.ToString(h["State"])!="Submitted")throw new InvalidOperationException("Only submitted weeks can be reviewed.");
   Validate(c,tx,b,e,week);var day=AnyminuteTimesheetRepair.Day(c,tx,b,e,date);
   if(Convert.ToString(day.Rows[0]["Status"])=="2") {tx.Commit();return;}
   int role,actor=AnyminuteModernScope.Actor(c,tx,b,out role);
   AnyminuteTimesheetRepair.Execute(c,tx,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=1");
   try
   {
    AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE TimeSheetEntry SET Status=2 WHERE TimeSheetEntryId=@id AND BussID=@b AND EmpId=@e; UPDATE AnyminuteWeek SET Revision=Revision+1,ChangedAtUTC=SYSUTCDATETIME() WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart=@w","@id",day.Rows[0]["TimeSheetEntryId"],"@b",b,"@e",e,"@w",week);
    Audit(c,tx,b,e,date,actor,"ApproveDay",Convert.ToString(day.Rows[0]["Status"]),"2");
    AnyminuteTimesheetRepair.Execute(c,tx,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=NULL");tx.Commit();
   }
   catch {try{tx.Rollback();}catch{}try{AnyminuteTimesheetRepair.Execute(c,null,"EXEC sys.sp_set_session_context @key=N'AnyminuteWorkflowWrite',@value=NULL");}catch{SqlConnection.ClearPool(c);}throw;}
  }
 }
 public static DataTable Notifications()
 {using(var c=AnyminuteTimesheetRepair.Open())return AnyminuteTimesheetRepair.Query(c,null,"SELECT * FROM AnyminuteNotification WHERE RecipientUserID=@u ORDER BY NotificationID DESC","@u",Convert.ToInt32(HttpContext.Current.Session["UserId"]));}
 public static void ReadNotification(long id)
 {using(var c=AnyminuteTimesheetRepair.Open())AnyminuteTimesheetRepair.Execute(c,null,"UPDATE AnyminuteNotification SET ReadAtUTC=SYSUTCDATETIME() WHERE NotificationID=@id AND RecipientUserID=@u","@id",id,"@u",Convert.ToInt32(HttpContext.Current.Session["UserId"]));}
}
