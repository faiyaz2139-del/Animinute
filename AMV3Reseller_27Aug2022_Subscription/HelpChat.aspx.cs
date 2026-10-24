using System;
public partial class AnyminuteHelpChat : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e)
    { AnyminuteModernScope.Login(); ViewStateUserKey=Session.SessionID; }
    protected void Page_Load(object sender, EventArgs e)
    {
        string href=AnyminuteUiNavigation.PermittedHref(AnyminuteUiNavigation.Load(),"Raise_Ticket_By_User.aspx","Ticket_List.aspx");
        Ticket.Visible=href!=null;if(href!=null)Ticket.NavigateUrl=href;
    }
}
