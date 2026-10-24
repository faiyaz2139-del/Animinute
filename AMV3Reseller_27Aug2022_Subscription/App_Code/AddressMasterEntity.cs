/// <summary>
/// Summary description for AddressMasterEntity
/// </summary>
public class AddressMasterEntity
{
    public AddressMasterEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int AddressMasterId { get; set; }
    public string Street_Name { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string PostalCode { get; set; }
    public int Verifed { get; set; }


    public int CityId { get; set; }
    public string CityName { get; set; }
    public bool Duplicate { get; set; }

    public int ProvinceId { get; set; }
    public string ProvinceName { get; set; }
    public bool DuplicateProvince { get; set; }
}