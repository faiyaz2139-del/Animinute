using System;
using System.Data;
using System.Web.UI.WebControls;
public partial class AnyminuteProjects : System.Web.UI.Page
{
 protected void Page_Load(object s,EventArgs e){if(IsPostBack)return;foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)Business.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]),Convert.ToString(b["BusinessID"])));string selected=Request.QueryString["BusinessId"];if(selected!=null&&Business.Items.FindByValue(selected)!=null)Business.SelectedValue=selected;Bind();}
 protected void Changed(object s,EventArgs e){Bind();}
 void Bind(){if(Business.Items.Count==0){Add.Visible=false;return;}int b=Int32.Parse(Business.SelectedValue);Rows.DataSource=AnyminuteModernScope.Projects(b);Rows.DataBind();Add.Visible=AnyminuteModernScope.Owns(b);}
}
