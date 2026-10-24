using System;

/// <summary>
/// Summary description for RateDetailsEntity
/// </summary>
public class RateDetailsEntity
{
    public RateDetailsEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int RateId { get; set; }
    public int EmployeeId { get; set; }
    public float Rate { get; set; }
    public String RateType { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
    public string JobTitle { get; set; }
    public string IsEmployeeEIExempty { get; set; }
    public string IsEmployeeCPPExempty { get; set; }
    public string RetainVacation { get; set; }
    public DateTime effectivedate { get; set; }



}