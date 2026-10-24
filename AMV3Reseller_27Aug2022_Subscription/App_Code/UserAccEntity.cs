/// <summary>
/// Summary description for UserAccEntity
/// </summary>
public class UserAccEntity
{
    public UserAccEntity()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int UserId { get; set; }
    public int EmployeeId { get; set; }
    public string Email { get; set; }
    public int UserTypeid { get; set; }
    public string UserType { get; set; }
    public string Password { get; set; }
    public string LinkAcc { get; set; }
    public bool Status { get; set; }
    public int MotherOrgid { get; set; }
    public int BusinessID { get; set; }
    public string ProfileImageName { get; set; }


}