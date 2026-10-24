using System;

/// <summary>
/// Summary description for TimeSheetEntryEntity
/// </summary>
public class TimeSheetEntryEntity
{
    public TimeSheetEntryEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int TimeSheetEntryId { get; set; }
    public int EmpId { get; set; }
    public int BussID { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int WeekNo { get; set; }
    public DateTime TimeSheetDate { get; set; }
    public string Day { get; set; }
    public string Regular { get; set; }
    public string Over1_5x { get; set; }
    public string Over2x { get; set; }
    public string Holiday { get; set; }
    public string Total { get; set; }
    public Nullable<DateTime> ReqularStart { get; set; }
    public Nullable<DateTime> Regularend { get; set; }
    public string BreakStart { get; set; }
    public string BreakEnd { get; set; }
    public string BreakTotal { get; set; }
    public string Worked { get; set; }
    public string Status { get; set; }
    public int ProjectId { get; set; }
}