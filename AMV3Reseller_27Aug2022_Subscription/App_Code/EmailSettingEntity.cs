/// <summary>
/// Summary description for EmailSettingEntity
/// </summary>
public class EmailSettingEntity
{
    public EmailSettingEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int EID { get; set; }
    public string SenderEmail { get; set; }
    public string Password { get; set; }
    public string HostName { get; set; }
    public int PortNo { get; set; }


    public int TempID { get; set; }
    public int TempType { get; set; }
    public string TempName { get; set; }
    public string Subject { get; set; }
    public string description { get; set; }

}