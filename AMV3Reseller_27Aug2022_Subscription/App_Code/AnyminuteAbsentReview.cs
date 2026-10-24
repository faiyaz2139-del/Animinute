using System;
// Absences must be explicitly saved before review; no database-only procedure dependency.
public static class AnyminuteAbsentReview
{
    public static void Review(int actorId,int actorRole,int businessId,int employeeId,DateTime date,int weekNo,int reviewStatus)
    {
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            AnyminuteTimesheetRepair.Authorize(c,null,actorId,actorRole,businessId,employeeId,false);
            var rows=AnyminuteTimesheetRepair.Day(c,null,businessId,employeeId,date);
            if(rows.Rows.Count!=1 || Convert.ToString(rows.Rows[0]["Worked"]).Trim()!="No")
                throw new InvalidOperationException("Save this day as not worked before reviewing its absence.");
        }
        AnyminuteTimeReview.Review(actorId,actorRole,businessId,employeeId,new[]{date.Date},reviewStatus);
    }
}
