using System;

/// <summary>
/// Summary description for AddBreakEntity
/// </summary>
public class AddBreakEntity
{
    public AddBreakEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int BreakID { get; set; }
    public int EmpId { get; set; }
    public int BussID { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int WeekNo { get; set; }
    public DateTime TimeSheetDate { get; set; }
    public string Day { get; set; }
    public string BreakStart { get; set; }
    public string BreakEnd { get; set; }
    public string BreakTotal { get; set; }
    public int ProjectId { get; set; }
}