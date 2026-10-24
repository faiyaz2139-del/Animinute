using System;

/// <summary>
/// Summary description for AddEmployeeEntityClass
/// </summary>
public class AddEmployeeEntityClass
{
    public AddEmployeeEntityClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int EmployeeID { get; set; }
    public int UserType { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string SocialInsuranceNumber { get; set; }
    public string MobilePhone { get; set; }
    public string HomePhone { get; set; }
    public string EMail_primary { get; set; }
    public string EMail_secondary { get; set; }
    public string StreetNumber { get; set; }
    public string StreetName { get; set; }
    public string Apartment { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
    public int BillRate { get; set; }
    public string BillType { get; set; }
    public string JobTitle { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime TerminationDate { get; set; }
    public bool IsEmployeeEIExempt { get; set; }
    public bool IsEmployeeCPPExempt { get; set; }
    public bool RetainVacation { get; set; }
    public int MotherOrgID { get; set; }
    public int BusinessID { get; set; }
    public int MangerId { get; set; }
    public int BusinessOwnerId { get; set; }
    public string Occupation { get; set; }
    public string LegalName { get; set; }
    public string IsActive { get; set; }

    public string SINExpiry { get; set; }
}