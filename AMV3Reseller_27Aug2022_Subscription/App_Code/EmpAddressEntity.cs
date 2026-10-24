/// <summary>
/// Summary description for EmpAddressEntity
/// </summary>
public class EmpAddressEntity
{
    public EmpAddressEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int EmpAddressId { get; set; }
    public string Postalcode { get; set; }
    public string NumberSuffix { get; set; }
    public string StreetName { get; set; }
    public string StreetType { get; set; }
    public string StreetDirection { get; set; }
    public string SuitType { get; set; }
    public string SuiteNumber { get; set; }
    public string SuitSuffix { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string MainPhone { get; set; }
    public string Mobile { get; set; }
    public string WorkPhone { get; set; }
    public string Fax { get; set; }
    public string MainEmail { get; set; }
    public string CCEmail { get; set; }
    public string Website { get; set; }
    public string Other1 { get; set; }
    public string StreetNo { get; set; }
    public int MotherOrgId { get; set; }
    public int CityId { get; set; }
    public int ProvinceId { get; set; }
}