using System;
public partial class AnyminuteEmployeesEntry : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AnyminuteDirectoryScope.RequireDirectoryRole();
        Response.Redirect("Users.aspx", true);
    }
}
