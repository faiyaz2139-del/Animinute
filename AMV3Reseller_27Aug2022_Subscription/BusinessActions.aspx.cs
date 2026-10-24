using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteBusinessActions : System.Web.UI.Page
{
    private bool Accountant { get { return Request.QueryString["action"] == "accountant"; } }
    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); ViewStateUserKey=Session.SessionID;
        AnyminuteFormAccess.OwnerSomewhere();
        if(Request.QueryString["action"]!="edit" && !Accountant)throw new HttpException(400,"Unknown business action.");
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        ActionTitle.Text=Accountant?"Assign Accountant":"Edit Business";
        if(IsPostBack)return;
        Business.Items.Add(new ListItem("Select a business", ""));
        foreach(DataRow row in AnyminuteModernScope.Businesses().Rows)
        {
            int id=Convert.ToInt32(row["BusinessID"]);
            if(AnyminuteModernScope.Owns(id))Business.Items.Add(new ListItem(Convert.ToString(row["CompanyName"]),id.ToString()));
        }
    }
    protected void Continue_Click(object sender, EventArgs e)
    {
        int business;
        if(!Int32.TryParse(Business.SelectedValue,out business)||business<=0){Message.Text="Select a business.";return;}
        using(var c=AnyminuteTimesheetRepair.Open())AnyminuteApprovalAssignments.RequireOwner(c,null,business);
        string target=Accountant?"BusinessAccountant.aspx?BusinessId=":"Add_Business.aspx?BusinessIDEdit=";
        Response.Redirect(target+business.ToString(System.Globalization.CultureInfo.InvariantCulture),false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
