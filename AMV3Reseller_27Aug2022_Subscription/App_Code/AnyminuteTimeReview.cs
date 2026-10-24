using System;
using System.Data;
using System.Linq;
using System.Web;

public static class AnyminuteTimeReview
{
    public static void ReopenDay(int actor,int role,int business,int employee,DateTime date)
    {
        throw new InvalidOperationException("Use Reopen Approved Week on the weekly timecard and enter a correction reason.");
    }
    public static void DeleteDay(int actor,int role,int business,int employee,DateTime date)
    {
        using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,role,business,employee,true);
            var rows=AnyminuteTimesheetRepair.Day(c,tx,business,employee,date);
            AnyminuteTimesheetRepair.EnsureUnlocked(c,tx,business,employee,date,rows);
            AnyminuteTimesheetRepair.Execute(c,tx,@"DELETE BreakTime WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0;
                DELETE TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0;
                INSERT AnyminuteApprovalAudit (BusinessID,ActorID,EmployeeID,TimeSheetDate,Action,RecordedAtUTC)
                VALUES (@b,@a,@e,@d,'DeleteDay',SYSUTCDATETIME())", "@b",business,"@e",employee,"@d",date.Date,"@a",actor);
            tx.Commit();
        }
    }
    public static void SaveEntry(int actor,int role,TimeSheetEntryEntity entry)
    {
        if(entry.Status=="2" || entry.Status=="3") throw new InvalidOperationException("Save attendance first, then use Approve or Reject.");
        using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            int b=entry.BussID,e=entry.EmpId;DateTime d=entry.TimeSheetDate.Date;
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,role,b,e,true);
            var day=AnyminuteTimesheetRepair.Day(c,tx,b,e,d);
            AnyminuteTimesheetRepair.EnsureUnlocked(c,tx,b,e,d,day);
            var breaks=AnyminuteTimesheetRepair.Breaks(c,tx,b,e,d);
            TimeSpan gross=TimeSpan.Zero,totalBreak=TimeSpan.Zero;
            bool worked=Convert.ToString(entry.Worked).Trim()=="Yes";
            if(worked)
            {
                if(!entry.ReqularStart.HasValue || !entry.Regularend.HasValue || entry.ReqularStart.Value.Date!=d || entry.Regularend.Value<=entry.ReqularStart.Value || entry.Regularend.Value-entry.ReqularStart.Value>=TimeSpan.FromDays(1))
                    throw new InvalidOperationException("Clock-in must belong to this day and clock-out must follow it within 24 hours.");
                gross=entry.Regularend.Value-entry.ReqularStart.Value;
                totalBreak=AnyminuteTimesheetRepair.ValidateSavedBreaks(breaks,entry.ReqularStart.Value,entry.Regularend.Value);
            }
            else if(breaks.Rows.Count>0) throw new InvalidOperationException("Remove existing breaks before marking this day as not worked.");
            var culture=System.Globalization.CultureInfo.CurrentCulture;
            int week=culture.Calendar.GetWeekOfYear(d,System.Globalization.CalendarWeekRule.FirstDay,DayOfWeek.Sunday);
            string sql=day.Rows.Count==0?@"INSERT TimeSheetEntry
                (EmpId,BussID,Month,Year,WeekNo,TimeSheetDate,Day,ReqularStart,Regularend,Regular,BreakStart,BreakEnd,BreakTotal,Over1_5x,Over2x,Holiday,Total,Status,Worked,ProjectId)
                VALUES (@e,@b,@m,@y,@w,@d,@day,@s,@f,@g,'','',@break,@o1,@o2,@h,@n,@status,@worked,0)":
                @"UPDATE TimeSheetEntry SET Month=@m,Year=@y,WeekNo=@w,Day=@day,ReqularStart=@s,Regularend=@f,Regular=@g,
                BreakStart='',BreakEnd='',BreakTotal=@break,Over1_5x=@o1,Over2x=@o2,Holiday=@h,Total=@n,Status=@status,Worked=@worked
                WHERE TimeSheetEntryId=@id AND BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0";
            AnyminuteTimesheetRepair.Execute(c,tx,sql,"@id",day.Rows.Count==0?(object)DBNull.Value:day.Rows[0]["TimeSheetEntryId"],
                "@e",e,"@b",b,"@m",d.Month,"@y",d.Year,"@w",week,"@d",d,"@day",d.DayOfWeek.ToString(),
                "@s",worked?(object)entry.ReqularStart.Value:DBNull.Value,"@f",worked?(object)entry.Regularend.Value:DBNull.Value,
                "@g",gross.ToString(),"@break",totalBreak.ToString(),"@n",(gross-totalBreak).ToString(),
                "@o1",worked?AnyminuteReportDurations.Parse(entry.Over1_5x).ToString():"00:00:00",
                "@o2",worked?AnyminuteReportDurations.Parse(entry.Over2x).ToString():"00:00:00",
                "@h",worked?AnyminuteReportDurations.Parse(entry.Holiday).ToString():"00:00:00","@status",worked?1:4,"@worked",worked?"Yes":"No");
            tx.Commit();
        }
    }
    public static int Review(int actor,int role,int business,int employee,DateTime[] dates,int status)
    {
        if(dates.Length==0) throw new InvalidOperationException("Select a day.");
        DateTime week=AnyminuteWeekService.Monday(dates[0]);
        if(dates.Any(d=>AnyminuteWeekService.Monday(d)!=week)) throw new InvalidOperationException("Use the Monday–Sunday weekly timecard to review this range.");
        int revision=Convert.ToInt32(AnyminuteWeekService.Header(business,employee,week).Rows[0]["Revision"]);
        if(status==3) throw new InvalidOperationException("Use the weekly timecard and enter a comment to return this week for correction.");
        if(status!=2) throw new InvalidOperationException("Invalid review status.");
        if(dates.Length==1) AnyminuteWeekService.ApproveDay(business,employee,dates[0],revision);
        else AnyminuteWeekService.Transition(business,employee,week,revision,"Approve","");
        return dates.Length;
    }
    public static void SetLock(int actor,int role,int business,int employee,DateTime date,bool locked)
    {
        using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,role,business,employee,false);
            int resolved=actor,r=role;AnyminuteTimesheetRepair.ResolveBusinessActor(c,tx,ref resolved,ref r,business);
            if(!locked && r!=1 && r!=5) throw new InvalidOperationException("Only the subscriber or business owner can unlock a timecard.");
            var culture=System.Globalization.CultureInfo.CurrentCulture;
            int week=culture.Calendar.GetWeekOfYear(date,culture.DateTimeFormat.CalendarWeekRule,culture.DateTimeFormat.FirstDayOfWeek);
            AnyminuteTimesheetRepair.Execute(c,tx,@"UPDATE TimeCardStatus WITH (UPDLOCK,HOLDLOCK) SET TimeSheetStatus=@s WHERE BusinessID=@b AND EmployeeID=@e AND Year=@y AND weekNum=@w;
                IF @@ROWCOUNT=0 INSERT TimeCardStatus (EmployeeID,BusinessID,weekNum,Year,TimeSheetStatus) VALUES (@e,@b,@w,@y,@s);
                INSERT AnyminuteApprovalAudit (BusinessID,ActorID,EmployeeID,TimeSheetDate,Action,AfterValue,RecordedAtUTC)
                VALUES (@b,@a,@e,@d,'TimecardLock',@s,SYSUTCDATETIME())", "@b",business,"@e",employee,"@y",date.Year,"@w",week,"@s",locked?1:0,"@a",resolved,"@d",date.Date);
            tx.Commit();
        }
    }
}
