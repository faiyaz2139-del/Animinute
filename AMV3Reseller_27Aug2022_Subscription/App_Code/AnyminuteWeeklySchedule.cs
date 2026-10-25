using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

// 2026-10-25 v11 Deck I: Weekly Employee Schedule (planned hours). Stored in dbo.AnyminuteWeeklySchedule
// (DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql). Never writes TimeSheetEntry / BreakTime (those are actual punches).
public sealed class AnyminuteScheduleDay
{
    public DateTime Date { get; set; }
    public bool Working { get; set; }
    public TimeSpan Start { get; set; }
    public TimeSpan End { get; set; }
    public int BreakMinutes { get; set; }
    public string Notes { get; set; }
    public bool Saved { get; set; }
    public decimal Hours
    {
        get
        {
            if (!Working || End <= Start) return 0m;
            double minutes = (End - Start).TotalMinutes - BreakMinutes;
            return minutes <= 0 ? 0m : Math.Round((decimal)minutes / 60m, 2);
        }
    }
}

public static class AnyminuteWeeklySchedule
{
    public const string Table = "AnyminuteWeeklySchedule";
    public const string MissingTable = "Weekly schedules need the database update in DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql. Nothing was saved.";
    public static readonly int[] BreakOptions = { 0, 15, 30, 45, 60, 90, 120 };

    public static DateTime Monday(DateTime d)
    {
        int diff = ((int)d.DayOfWeek + 6) % 7;
        return d.Date.AddDays(-diff);
    }

    public static bool Ready()
    {
        using (var c = AnyminuteTimesheetRepair.Open()) return AnyminuteEmployeeEditor.HasTable(c, null, Table);
    }

    // Default week per the deck: Mon-Fri Working 09:00-17:00 with a 30 minute break, Sat/Sun Day Off.
    public static List<AnyminuteScheduleDay> DefaultWeek(DateTime monday)
    {
        var list = new List<AnyminuteScheduleDay>();
        for (int i = 0; i < 7; i++)
            list.Add(new AnyminuteScheduleDay { Date = monday.AddDays(i), Working = i < 5, Start = new TimeSpan(9, 0, 0), End = new TimeSpan(17, 0, 0), BreakMinutes = i < 5 ? 30 : 0, Notes = "" });
        return list;
    }

    // Business + employee the signed-in user may schedule: owners for anyone in the business,
    // managers/admins per the existing timesheet rule (AnyminuteTimesheetRepair.Authorize).
    public static void Authorize(SqlConnection c, SqlTransaction tx, int business, int employee)
    {
        int role, actor = AnyminuteModernScope.Actor(c, tx, business, out role);
        if (role == 1)
        {
            if (AnyminuteTimesheetRepair.Query(c, tx, "SELECT 1 FROM AssginBussiness WHERE EmployeeID=@e AND BusinessID=@b", "@e", employee, "@b", business).Rows.Count == 0)
                throw new InvalidOperationException("Employee is not assigned to this business.");
            return;
        }
        AnyminuteTimesheetRepair.Authorize(c, tx, actor, role, business, employee, false);
    }

    public static List<AnyminuteScheduleDay> Load(int business, int employee, DateTime monday, out bool anySaved)
    {
        var week = DefaultWeek(monday); anySaved = false;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            Authorize(c, null, business, employee);
            if (!AnyminuteEmployeeEditor.HasTable(c, null, Table)) return week;
            var rows = AnyminuteTimesheetRepair.Query(c, null, "SELECT WorkDate,ScheduleType,StartTime,EndTime,BreakMinutes,Notes FROM dbo.AnyminuteWeeklySchedule WHERE BusinessID=@b AND EmployeeID=@e AND WorkDate BETWEEN @f AND @t",
                "@b", business, "@e", employee, "@f", monday, "@t", monday.AddDays(6));
            foreach (DataRow r in rows.Rows)
            {
                DateTime d = Convert.ToDateTime(r["WorkDate"]).Date;
                var day = week.FirstOrDefault(x => x.Date == d); if (day == null) continue;
                day.Working = Convert.ToString(r["ScheduleType"]) == "Working";
                if (r["StartTime"] != DBNull.Value) day.Start = (TimeSpan)r["StartTime"];
                if (r["EndTime"] != DBNull.Value) day.End = (TimeSpan)r["EndTime"];
                day.BreakMinutes = Convert.ToInt32(r["BreakMinutes"]);
                day.Notes = r["Notes"] == DBNull.Value ? "" : Convert.ToString(r["Notes"]);
                day.Saved = true; anySaved = true;
            }
        }
        return week;
    }

    public static string Validate(IList<AnyminuteScheduleDay> week)
    {
        foreach (var d in week)
        {
            string name = d.Date.ToString("dddd", CultureInfo.InvariantCulture);
            if ((d.Notes ?? "").Length > 200) return name + ": notes can be at most 200 characters.";
            if (!d.Working) continue;
            if (d.End <= d.Start) return name + ": End Time must be after Start Time.";
            if (d.BreakMinutes < 0 || d.BreakMinutes >= (d.End - d.Start).TotalMinutes) return name + ": the break must be shorter than the working time.";
        }
        return null;
    }

    // Saves the week; when repeatUntil is set, copies the pattern to the following weeks up to that date.
    // apply: "all" = every day, "weekdays" = Mon-Fri only, "selected" = the chosen days only.
    public static int Save(int business, int employee, IList<AnyminuteScheduleDay> week, DateTime? repeatUntil, string apply, ICollection<DayOfWeek> selected)
    {
        string error = Validate(week); if (error != null) throw new InvalidOperationException(error);
        DateTime monday = Monday(week[0].Date);
        if (repeatUntil.HasValue)
        {
            if (repeatUntil.Value.Date <= monday.AddDays(6)) throw new InvalidOperationException("Repeat Until must be after this week.");
            if (repeatUntil.Value.Date > monday.AddDays(7 * 52)) throw new InvalidOperationException("Repeat Until can be at most 52 weeks ahead.");
            if (apply == "selected" && (selected == null || selected.Count == 0)) throw new InvalidOperationException("Select at least one day to repeat.");
        }
        int actor = Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]), written = 0;
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction())
        {
            if (!AnyminuteEmployeeEditor.HasTable(c, tx, Table)) throw new InvalidOperationException(MissingTable);
            Authorize(c, tx, business, employee);
            foreach (var d in week) { Upsert(c, tx, business, employee, d.Date, d, actor); written++; }
            if (repeatUntil.HasValue)
                for (DateTime w = monday.AddDays(7); w <= repeatUntil.Value.Date; w = w.AddDays(7))
                    foreach (var d in week)
                    {
                        DateTime target = w.AddDays((d.Date - monday).Days);
                        if (target > repeatUntil.Value.Date) continue;
                        bool weekday = target.DayOfWeek != DayOfWeek.Saturday && target.DayOfWeek != DayOfWeek.Sunday;
                        if (apply == "weekdays" && !weekday) continue;
                        if (apply == "selected" && !selected.Contains(target.DayOfWeek)) continue;
                        Upsert(c, tx, business, employee, target, d, actor); written++;
                    }
            tx.Commit();
        }
        return written;
    }

    static void Upsert(SqlConnection c, SqlTransaction tx, int business, int employee, DateTime date, AnyminuteScheduleDay d, int actor)
    {
        object start = d.Working ? (object)d.Start : DBNull.Value, end = d.Working ? (object)d.End : DBNull.Value;
        object notes = String.IsNullOrWhiteSpace(d.Notes) ? (object)DBNull.Value : d.Notes.Trim();
        AnyminuteTimesheetRepair.Execute(c, tx, @"UPDATE dbo.AnyminuteWeeklySchedule SET ScheduleType=@t,StartTime=@s,EndTime=@x,BreakMinutes=@br,Notes=@n,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a
 WHERE BusinessID=@b AND EmployeeID=@e AND WorkDate=@d;
IF @@ROWCOUNT=0 INSERT dbo.AnyminuteWeeklySchedule(BusinessID,EmployeeID,WorkDate,ScheduleType,StartTime,EndTime,BreakMinutes,Notes,UpdatedAtUTC,UpdatedBy)
 VALUES(@b,@e,@d,@t,@s,@x,@br,@n,SYSUTCDATETIME(),@a);",
            "@b", business, "@e", employee, "@d", date.Date, "@t", d.Working ? "Working" : "DayOff", "@s", start, "@x", end,
            "@br", d.Working ? d.BreakMinutes : 0, "@n", notes, "@a", actor);
    }
}
