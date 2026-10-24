using System;

/// <summary>
/// Summary description for TimeSheetEntityClass
/// </summary>
public class TimeSheetEntityClass
{
    public TimeSheetEntityClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int TimeSheetID { get; set; }
    public int EmployeeId { get; set; }
    public int BusinessID { get; set; }
    public string BillType { get; set; }
    public int BillRate { get; set; }
    public DateTime Entrydate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string WorkingHr { get; set; }
    public string OverTimeHr { get; set; }
    public string BusinessName { get; set; }
    public string FirstName { get; set; }
    public string Description { get; set; }
    public string OverStartTime { get; set; }
    public string OverEndTime { get; set; }
    public string Day { get; set; }
    public bool WorkonHoliday { get; set; }
    public int MotherOrgId { get; set; }
}