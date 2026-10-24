using System;

public partial class AnyminuteHelp : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    {
        // Verify the current account on every request, including postbacks.
        AnyminuteModernScope.Login();
    }
}
