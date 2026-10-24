using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
public static class AnyminuteClockEvidence
{
 public static TimeZoneInfo Zone(SqlConnection c,SqlTransaction tx,int b)
 {
  var p=AnyminuteTimesheetRepair.Query(c,tx,"SELECT TimeZoneID FROM AnyminuteBusinessPolicy WHERE BusinessID=@b","@b",b);
  return TimeZoneInfo.FindSystemTimeZoneById(p.Rows.Count==0?"Eastern Standard Time":Convert.ToString(p.Rows[0][0]));
 }
 public static DateTime Today(int business)
 {using(var c=AnyminuteTimesheetRepair.Open())return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,Zone(c,null,business)).Date;}
 public static void Record(int business,string kind)
 {
  using(var c=AnyminuteTimesheetRepair.Open())
  {
   int role,actor=AnyminuteModernScope.Actor(c,null,business,out role);
   var policy=AnyminuteTimesheetRepair.Query(c,null,"SELECT AllowClockOutEstimates FROM AnyminuteBusinessPolicy WHERE BusinessID=@b","@b",business);
   if(policy.Rows.Count>0&&!Convert.ToBoolean(policy.Rows[0][0]))return;
   DateTime local=TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,Zone(c,null,business));
   // Only track useful attendance evidence for this authenticated account's own open shift.
   if(AnyminuteTimesheetRepair.Query(c,null,"SELECT 1 FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ReqularStart IS NOT NULL AND Regularend IS NULL AND ISNULL(ProjectId,0)=0","@b",business,"@e",actor,"@d",local.Date).Rows.Count==0)return;
   AnyminuteTimesheetRepair.Execute(c,null,"INSERT AnyminuteActivity(UserID,BusinessID,EmployeeID,ShiftDate,ActivityAtUTC,Kind) VALUES(@u,@b,@e,@d,SYSUTCDATETIME(),@kind)","@u",Convert.ToInt32(HttpContext.Current.Session["UserId"]),"@b",business,"@e",actor,"@d",local.Date,"@kind",kind);
  }
 }
 public static void Estimate(int b,int e,DateTime week)
 {
  using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
  {
   AnyminuteWeekService.ReadAccess(c,tx,b,e);var policy=AnyminuteTimesheetRepair.Query(c,tx,"SELECT AllowClockOutEstimates FROM AnyminuteBusinessPolicy WHERE BusinessID=@b","@b",b);
   if(policy.Rows.Count>0&&!Convert.ToBoolean(policy.Rows[0][0])) {tx.Commit();return;}
   var zone=Zone(c,tx,b);var today=TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,zone).Date;
   for(int i=0;i<7;i++)
   {
    DateTime day=AnyminuteWeekService.Monday(week).AddDays(i);if(day>=today)continue;
    var row=AnyminuteTimesheetRepair.Day(c,tx,b,e,day);
    if(row.Rows.Count!=1||row.Rows[0]["ReqularStart"]==DBNull.Value||row.Rows[0]["Regularend"]!=DBNull.Value)continue;
    var last=AnyminuteTimesheetRepair.Query(c,tx,"SELECT TOP(1) ActivityID,ActivityAtUTC FROM AnyminuteActivity WHERE BusinessID=@b AND EmployeeID=@e AND ShiftDate=@d ORDER BY ActivityAtUTC DESC,ActivityID DESC","@b",b,"@e",e,"@d",day);
    if(last.Rows.Count==0)continue;
    DateTime end=TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(Convert.ToDateTime(last.Rows[0]["ActivityAtUTC"]),DateTimeKind.Utc),zone);
    if(end.Date!=day||end<=Convert.ToDateTime(row.Rows[0]["ReqularStart"]))continue;
    AnyminuteTimesheetRepair.Execute(c,tx,@"IF NOT EXISTS(SELECT 1 FROM AnyminuteClockOutEstimate WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e AND ShiftDate=@d)
      INSERT AnyminuteClockOutEstimate(BusinessID,EmployeeID,ShiftDate,EstimatedEnd,EvidenceActivityID) VALUES(@b,@e,@d,@end,@activity)","@b",b,"@e",e,"@d",day,"@end",end,"@activity",last.Rows[0]["ActivityID"]);
   }
   tx.Commit();
  }
 }
 public static void Confirm(int b,int e,DateTime day)
 {
  using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
  {
   AnyminuteTimesheetRepair.Authorize(c,tx,Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]),Convert.ToInt32(HttpContext.Current.Session["UserTypeid"]),b,e,true);
   AnyminuteWeekService.EnsureEditable(c,tx,b,e,day);
   var rows=AnyminuteTimesheetRepair.Day(c,tx,b,e,day);AnyminuteTimesheetRepair.EnsureUnlocked(c,tx,b,e,day,rows);
   var estimate=AnyminuteTimesheetRepair.Query(c,tx,"SELECT * FROM AnyminuteClockOutEstimate WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e AND ShiftDate=@d AND State='NeedsReview'","@b",b,"@e",e,"@d",day.Date);
   if(rows.Rows.Count!=1||estimate.Rows.Count!=1||rows.Rows[0]["Regularend"]!=DBNull.Value)throw new InvalidOperationException("Estimate is no longer available.");
   DateTime start=Convert.ToDateTime(rows.Rows[0]["ReqularStart"]),end=Convert.ToDateTime(estimate.Rows[0]["EstimatedEnd"]);
   var unpaid=AnyminuteTimesheetRepair.ValidateSavedBreaks(AnyminuteTimesheetRepair.Breaks(c,tx,b,e,day),start,end);
   int role,actor=AnyminuteModernScope.Actor(c,tx,b,out role);
   AnyminuteTimesheetRepair.Execute(c,tx,@"UPDATE TimeSheetEntry SET Regularend=@end,Regular=@gross,BreakTotal=@break,Total=@net,Status=1 WHERE TimeSheetEntryId=@id;
    UPDATE AnyminuteClockOutEstimate SET State='Confirmed',ConfirmedBy=@a,ConfirmedAtUTC=SYSUTCDATETIME() WHERE BusinessID=@b AND EmployeeID=@e AND ShiftDate=@d;
    INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,TimeSheetDate,Action,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,@d,'ConfirmEstimate',CONVERT(nvarchar(30),@end,126),SYSUTCDATETIME())",
    "@end",end,"@gross",(end-start).ToString(),"@break",unpaid.ToString(),"@net",(end-start-unpaid).ToString(),"@id",rows.Rows[0]["TimeSheetEntryId"],"@b",b,"@e",e,"@d",day.Date,"@a",actor);
   tx.Commit();
  }
 }
}
