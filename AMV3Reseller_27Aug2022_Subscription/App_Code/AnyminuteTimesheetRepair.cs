using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

public static class AnyminuteTimesheetRepair
{
    public static SqlConnection Open()
    {
        var c = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString);
        c.Open(); return c;
    }
    public static SqlCommand Command(SqlConnection c, SqlTransaction tx, string sql, params object[] args)
    {
        var cmd = new SqlCommand(sql, c, tx);
        for (int i=0;i<args.Length;i+=2) cmd.Parameters.AddWithValue((string)args[i],args[i+1] ?? DBNull.Value);
        return cmd;
    }
    public static DataTable Query(SqlConnection c, SqlTransaction tx, string sql, params object[] args)
    {
        using(var cmd=Command(c,tx,sql,args)) using(var da=new SqlDataAdapter(cmd))
        { var dt=new DataTable(); da.Fill(dt); return AnyminuteDisplayText.Tidy(dt); } // v27: tidy display names/roles
    }
    public static void Execute(SqlConnection c, SqlTransaction tx, string sql, params object[] args)
    { using(var cmd=Command(c,tx,sql,args)) cmd.ExecuteNonQuery(); }
    public static void ResolveBusinessActor(SqlConnection c, SqlTransaction tx, ref int actor, ref int role, int business)
    {
        // Bind the supplied actor to the current authenticated account, then resolve
        // that account's business-specific employee identity. Never join by email.
        var context = System.Web.HttpContext.Current;
        int userId, sessionActor;
        if (context == null || context.Session == null ||
            !Int32.TryParse(Convert.ToString(context.Session["UserId"]), out userId) ||
            !Int32.TryParse(Convert.ToString(context.Session["EmployeeId"]), out sessionActor) ||
            actor <= 0 || actor != sessionActor ||
            Query(c,tx,"SELECT 1 FROM UserAcc WHERE UserId=@u AND EmployeeId=@a", "@u",userId,"@a",actor).Rows.Count != 1)
            throw new InvalidOperationException("Your login could not be verified. Sign in again.");
        var owners = Query(c,tx,@"SELECT 1 FROM AssginBussiness a JOIN AddBusiness b ON b.BusinessID=a.BusinessID
            WHERE a.EmployeeID=@a AND a.BusinessID=@b AND a.UserTypeId=1 AND b.Status<>2",
            "@a",actor,"@b",business);
        if (role==1 && owners.Rows.Count>0)
        {
            // The subscriber assignment is the ownership record used by the existing report page.
            role=1;
        }
        else
        {
            var memberships=Query(c,tx,@"SELECT DISTINCT m.EmployeeID,m.UserTypeId FROM MappingTable m
                JOIN AssginBussiness a ON a.EmployeeID=m.EmployeeID AND a.BusinessID=m.BusinessID AND a.UserTypeId=m.UserTypeId
                JOIN AddBusiness b ON b.BusinessID=m.BusinessID
                WHERE m.USERID=@u AND m.BusinessID=@b AND b.Status<>2",
                "@u",userId,"@b",business);
            if (memberships.Rows.Count!=1 || memberships.Rows[0]["EmployeeID"]==DBNull.Value || memberships.Rows[0]["UserTypeId"]==DBNull.Value)
                throw new InvalidOperationException("Your business assignment is missing or ambiguous. Ask your subscriber to review it.");
            actor=Convert.ToInt32(memberships.Rows[0]["EmployeeID"]);
            role=Convert.ToInt32(memberships.Rows[0]["UserTypeId"]);
        }
    }
    public static void Authorize(SqlConnection c, SqlTransaction tx, int actor, int role, int business, int employee, bool employeeAllowed)
    {
        ResolveBusinessActor(c,tx,ref actor,ref role,business);
        if(Query(c,tx,"SELECT 1 FROM AssginBussiness WHERE EmployeeID=@e AND BusinessID=@b","@e",employee,"@b",business).Rows.Count==0)
            throw new InvalidOperationException("Employee is not assigned to this business.");
        int userId=Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
        bool ownAccount=actor==employee || Query(c,tx,"SELECT 1 FROM MappingTable WHERE USERID=@u AND EmployeeID=@e AND BusinessID=@b","@u",userId,"@e",employee,"@b",business).Rows.Count>0;
        if(!employeeAllowed && ownAccount) throw new InvalidOperationException("You cannot review your own attendance.");
        if(employeeAllowed && ownAccount) return;
        if(role==1) return;
        var permissions=Query(c,tx,"SELECT Role_Permission_Id,Status FROM User_Role_Permission WHERE Employee_Id=@a AND UserTypeId=@r AND (BusinessId=@b OR BusinessId IS NULL)","@a",actor.ToString(),"@r",role.ToString(),"@b",business);
        // v22 (Pandey #5): a role change inserts a placeholder permission row (empty Role_Permission_Id, Status=1). That row
        // used to count as "explicit permissions = none", so a newly promoted Manager could not see or approve any time
        // card. Placeholder rows now keep the role default (Manager / Admin allowed); a disabled or non-empty row is explicit.
        bool placeholderOnly=permissions.Rows.Cast<DataRow>().All(p=>p["Status"]!=DBNull.Value && Convert.ToBoolean(p["Status"]) && Convert.ToString(p["Role_Permission_Id"]).Replace(",","").Trim().Length==0);
        bool allowed=placeholderOnly && (role==2 || role==5);
        foreach(DataRow p in permissions.Rows)
        {
            if(p["Status"]==DBNull.Value || !Convert.ToBoolean(p["Status"])) continue;
            foreach(string id in Convert.ToString(p["Role_Permission_Id"]).Split(','))
                if(employeeAllowed ? (id.Trim()=="7" || id.Trim()=="8" || id.Trim()=="18" || id.Trim()=="19") : (id.Trim()=="9" || id.Trim()=="20")) allowed=true;
        }
        if(!allowed) throw new InvalidOperationException("Your role does not permit this change.");
        if(role!=5 && Query(c,tx,"SELECT 1 FROM AssgineManager WHERE BusinessID=@b AND ManagerID=@a AND EmpId=@e","@b",business,"@a",actor,"@e",employee).Rows.Count==0)
            throw new InvalidOperationException("Employee is not assigned to you.");
    }
    public static void AuthorizeRead(SqlConnection c,SqlTransaction tx,int actor,int role,int business,int employee)
    {
        try { Authorize(c,tx,actor,role,business,employee,true); }
        catch(InvalidOperationException) { Authorize(c,tx,actor,role,business,employee,false); }
    }
    static object[] Scope(int b,int e,DateTime d) {return new object[]{"@b",b,"@e",e,"@d",d.Date};}
    public static DataTable Day(SqlConnection c,SqlTransaction tx,int b,int e,DateTime d)
    { return Query(c,tx,"SELECT * FROM TimeSheetEntry WITH (UPDLOCK,HOLDLOCK) WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0",Scope(b,e,d)); }
    static int Week(DateTime d) {return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d,CalendarWeekRule.FirstDay,DayOfWeek.Sunday);}
    public static void EnsureUnlocked(SqlConnection c,SqlTransaction tx,int b,int e,DateTime d,DataTable day, bool reviewing=false)
    {
        if(!reviewing) AnyminuteWeekService.EnsureEditable(c,tx,b,e,d);
        if(day.Rows.Count>1) throw new InvalidOperationException("Duplicate timesheets require review before this action.");
        if(day.Rows.Count==1 && Convert.ToString(day.Rows[0]["Status"])=="2") throw new InvalidOperationException("This day is approved. Ask a reviewer to reopen it first.");
        if(Query(c,tx,"SELECT 1 FROM TimeCardStatus WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e AND Year=@y AND weekNum=@w AND TimeSheetStatus=1","@b",b,"@e",e,"@y",d.Year,"@w",CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(d,CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)).Rows.Count>0)
            throw new InvalidOperationException("This timecard is locked.");
    }
    public static DataTable Breaks(SqlConnection c,SqlTransaction tx,int b,int e,DateTime d)
    { return Query(c,tx,"SELECT BreakID,BreakStart,BreakEnd,BreakTotal,IsPaid FROM BreakTime WITH (UPDLOCK,HOLDLOCK) WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0 ORDER BY BreakStart,BreakID",Scope(b,e,d)); }
    public static DataTable ReadBreaks(int actor,int role,int b,int e,DateTime d)
    { using(var c=Open()) { AuthorizeRead(c,null,actor,role,b,e); return Breaks(c,null,b,e,d); } }
    static TimeSpan Duration(object value)
    {
        string s=Convert.ToString(value).Trim(); if(s.Length==0)return TimeSpan.Zero;
        TimeSpan t; if(!TimeSpan.TryParse(s,out t) || t<TimeSpan.Zero)throw new InvalidOperationException("Invalid saved duration. Please contact support.");return t;
    }
    public static TimeSpan ValidateSavedBreaks(DataTable breaks,DateTime start,DateTime end)
    {
        TimeSpan total=TimeSpan.Zero; var periods=new List<Tuple<DateTime,DateTime>>();
        foreach(DataRow br in breaks.Rows)
        {
            DateTime bs,be;
            if(!DateTime.TryParse(Convert.ToString(br["BreakStart"]),out bs) || !DateTime.TryParse(Convert.ToString(br["BreakEnd"]),out be) || bs<start || be>end || be<=bs)
                throw new InvalidOperationException("A break is outside the saved shift. Correct its dates and times.");
            foreach(var other in periods) if(bs<other.Item2 && be>other.Item1)throw new InvalidOperationException("Breaks must not overlap.");
            periods.Add(Tuple.Create(bs,be)); if(!breaks.Columns.Contains("IsPaid") || !Convert.ToBoolean(br["IsPaid"])) total+=be-bs;
        }
        return total;
    }
    public static void ChangeBreak(int actor,int role,int b,int e,DateTime d,int? id,DateTime? start,DateTime? end,bool delete, bool? paid=null)
    {
        using(var c=Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            Authorize(c,tx,actor,role,b,e,true);var day=Day(c,tx,b,e,d);EnsureUnlocked(c,tx,b,e,d,day);
            if(day.Rows.Count!=1 || day.Rows[0]["ReqularStart"]==DBNull.Value || day.Rows[0]["Regularend"]==DBNull.Value)
                throw new InvalidOperationException("Save a present shift before changing breaks.");
            var shift=day.Rows[0];DateTime ss=Convert.ToDateTime(shift["ReqularStart"]),se=Convert.ToDateTime(shift["Regularend"]);
            if(se<=ss || se-ss>=TimeSpan.FromDays(1))throw new InvalidOperationException("Saved shift times are invalid.");
            var breaks=Breaks(c,tx,b,e,d);
            if(id.HasValue && breaks.Select("BreakID="+id.Value).Length!=1)throw new InvalidOperationException("Break no longer exists on this day. Refresh and try again.");
            if(delete && !id.HasValue)throw new InvalidOperationException("Select a break to delete.");
            if(delete) Execute(c,tx,"DELETE BreakTime WHERE BreakID=@id AND BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0","@id",id.Value,"@b",b,"@e",e,"@d",d.Date);
            else
            {
                if(!start.HasValue || !end.HasValue || start.Value<ss || end.Value>se || end.Value<=start.Value)throw new InvalidOperationException("Break start and end must be within the saved shift, with end after start.");
                var p=new object[]{"@id",id,"@b",b,"@e",e,"@d",d.Date,"@s",start.Value.ToString("yyyy-MM-ddTHH:mm:ss"),"@f",end.Value.ToString("yyyy-MM-ddTHH:mm:ss"),"@t",(end.Value-start.Value).ToString(),"@m",d.Month,"@y",d.Year,"@w",Week(d),"@day",d.DayOfWeek.ToString(),"@paid",paid.HasValue?(object)paid.Value:DBNull.Value};
                Execute(c,tx,id.HasValue ? "UPDATE BreakTime SET BreakStart=@s,BreakEnd=@f,BreakTotal=@t,IsPaid=COALESCE(@paid,IsPaid) WHERE BreakID=@id AND BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0" : "INSERT BreakTime (EmpId,BussID,Month,Year,WeekNo,TimeSheetDate,Day,BreakStart,BreakEnd,BreakTotal,ProjectId,IsPaid) VALUES (@e,@b,@m,@y,@w,@d,@day,@s,@f,@t,0,COALESCE(@paid,0))",p);
            }
            TimeSpan total=ValidateSavedBreaks(Breaks(c,tx,b,e,d),ss,se);
            Execute(c,tx,"UPDATE TimeSheetEntry SET Regular=@g,BreakTotal=@t,Total=@n WHERE TimeSheetEntryId=@id","@g",(se-ss).ToString(),"@t",total.ToString(),"@n",(se-ss-total).ToString(),"@id",shift["TimeSheetEntryId"]);
            tx.Commit();
        }
    }
    static bool EmptyPendingDay(DataTable data)
    {
        if(data.Rows.Count==0)return true;
        if(data.Rows.Count!=1)return false;
        var row=data.Rows[0];
        string status=Convert.ToString(row["Status"]).Trim();
        if(status!="1" || Convert.ToString(row["Worked"]).Trim()!="No" ||
            row["ReqularStart"]!=DBNull.Value || row["Regularend"]!=DBNull.Value)return false;
        foreach(string column in new string[]{"Regular","BreakTotal","Total","Over1_5x","Over2x","Holiday"})
        {
            string value=Convert.ToString(row[column]).Trim();
            if(value!="" && value!="0" && value!="00:00" && value!="00:00:00")return false;
        }
        return true;
    }
    public static int Copy(int actor,int role,int b,int e,DateTime[] sources,DateTime[] targets,bool overwrite)
    {
        if(sources.Length!=targets.Length)throw new ArgumentException("Copy dates do not match.");
        using(var c=Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            Authorize(c,tx,actor,role,b,e,true); int count=0;
            // Snapshot every source before changing any destination.
            var shifts=new List<DataTable>();var breakSets=new List<DataTable>();
            for(int i=0;i<sources.Length;i++){shifts.Add(Day(c,tx,b,e,sources[i]));breakSets.Add(Breaks(c,tx,b,e,sources[i]));}
            for(int i=0;i<sources.Length;i++)
            {
                if(sources[i].Date==targets[i].Date || shifts[i].Rows.Count==0)continue;
                if(shifts[i].Rows.Count!=1)throw new InvalidOperationException("Duplicate source timesheet. Nothing copied.");
                var dest=Day(c,tx,b,e,targets[i]);
                if(!overwrite && (!EmptyPendingDay(dest) || Breaks(c,tx,b,e,targets[i]).Rows.Count>0))continue;
                EnsureUnlocked(c,tx,b,e,targets[i],dest);
                var s=shifts[i].Rows[0];var brs=breakSets[i];int delta=(targets[i].Date-sources[i].Date).Days;
                DateTime? start=s["ReqularStart"]==DBNull.Value ? (DateTime?)null : Convert.ToDateTime(s["ReqularStart"]);
                DateTime? end=s["Regularend"]==DBNull.Value ? (DateTime?)null : Convert.ToDateTime(s["Regularend"]);
                TimeSpan total=TimeSpan.Zero,gross=TimeSpan.Zero;
                if(start.HasValue && end.HasValue)
                {
                    gross=end.Value-start.Value;if(gross<=TimeSpan.Zero || gross>=TimeSpan.FromDays(1))throw new InvalidOperationException("Invalid source shift. Nothing copied.");
                    total=ValidateSavedBreaks(brs,start.Value,end.Value);
                    if(Duration(s["BreakTotal"])!=total)throw new InvalidOperationException("Source break total does not match its break records. Review the source day before copying.");
                }
                else if(start.HasValue || end.HasValue || brs.Rows.Count>0 || Duration(s["BreakTotal"])!=TimeSpan.Zero || Convert.ToString(s["Worked"]).Trim()=="Yes")throw new InvalidOperationException("Incomplete source day. Nothing copied.");
                Execute(c,tx,"DELETE BreakTime WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0; DELETE TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND TimeSheetDate=@d AND ISNULL(ProjectId,0)=0",Scope(b,e,targets[i]));
                Execute(c,tx,"INSERT TimeSheetEntry (EmpId,BussID,Month,Year,WeekNo,TimeSheetDate,Day,ReqularStart,Regularend,Regular,BreakStart,BreakEnd,BreakTotal,Over1_5x,Over2x,Holiday,Total,Status,Worked,ProjectId) VALUES (@e,@b,@m,@y,@w,@d,@day,@s,@f,@g,'','',@t,@o1,@o2,@h,@n,1,@worked,0)",
                    "@e",e,"@b",b,"@m",targets[i].Month,"@y",targets[i].Year,"@w",Week(targets[i]),"@d",targets[i].Date,"@day",targets[i].DayOfWeek.ToString(),"@s",start.HasValue?(object)start.Value.AddDays(delta):DBNull.Value,"@f",end.HasValue?(object)end.Value.AddDays(delta):DBNull.Value,"@g",gross.ToString(),"@t",total.ToString(),"@n",(gross-total).ToString(),"@o1",s["Over1_5x"],"@o2",s["Over2x"],"@h",s["Holiday"],"@worked",s["Worked"]);
                foreach(DataRow br in brs.Rows)
                {
                    DateTime bs=DateTime.Parse(Convert.ToString(br["BreakStart"])).AddDays(delta),be=DateTime.Parse(Convert.ToString(br["BreakEnd"])).AddDays(delta);
                    Execute(c,tx,"INSERT BreakTime (EmpId,BussID,Month,Year,WeekNo,TimeSheetDate,Day,BreakStart,BreakEnd,BreakTotal,ProjectId,IsPaid) VALUES (@e,@b,@m,@y,@w,@d,@day,@s,@f,@t,0,COALESCE(@paid,0))","@e",e,"@b",b,"@m",targets[i].Month,"@y",targets[i].Year,"@w",Week(targets[i]),"@d",targets[i].Date,"@day",targets[i].DayOfWeek.ToString(),"@s",bs.ToString("yyyy-MM-ddTHH:mm:ss"),"@f",be.ToString("yyyy-MM-ddTHH:mm:ss"),"@t",(be-bs).ToString(),"@paid",br["IsPaid"]);
                }
                count++;
            }
            tx.Commit();return count;
        }
    }
}
