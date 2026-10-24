using System;

/// <summary>
/// Summary description for AddTicketEntityClass
/// </summary>
public class AddTicketEntityClass
{
    public AddTicketEntityClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int TicketID { get; set; }
    public string MotherOrgName { get; set; }
    public int MotherOrgId { get; set; }
    public string BusinessName { get; set; }
    public int BusinessId { get; set; }
    public string ManagerName { get; set; }
    public int ManagerID { get; set; }
    public string EmpName { get; set; }
    public int EmpId { get; set; }
    public string Name { get; set; }
    public string EmailId { get; set; }
    public string PhoneNO { get; set; }
    public string ProblemType { get; set; }
    public string Module { get; set; }
    public string FileUploadName { get; set; }
    public string Description { get; set; }
    public string TicketNo { get; set; }
    public string DeveloperComment { get; set; }
    public string StatusType { get; set; }
    public string TicketSeverity { get; set; }
    public DateTime TicketRaiseDate { get; set; }
    public DateTime TicketResolveDate { get; set; }

}