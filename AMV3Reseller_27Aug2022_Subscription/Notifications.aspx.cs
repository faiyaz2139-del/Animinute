using System;
using System.Web.UI.WebControls;
public partial class AnyminuteNotifications : System.Web.UI.Page
{
 protected void Page_Load(object s,EventArgs e){if(!IsPostBack)Bind();}
 void Bind(){Rows.DataSource=AnyminuteWeekService.Notifications();Rows.DataBind();Empty.Visible=Rows.Items.Count==0;}
 protected void Command(object s,RepeaterCommandEventArgs e){AnyminuteWeekService.ReadNotification(Int64.Parse(Convert.ToString(e.CommandArgument)));Bind();}
}
