using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteEmployeeAssignmentPage : System.Web.UI.Page
{
    private int employee;
    protected void Page_Init(object sender,EventArgs e)
    {
        AnyminuteModernScope.Login();AnyminuteFormAccess.OwnerSomewhere();ViewStateUserKey=Session.SessionID;
        if(!Int32.TryParse(Request.QueryString["EmployeeId"],out employee)||employee<=0)throw new HttpException(400,"Select an employee.");
        var card=AnyminuteEmployeesView.RequireEmployee(employee);if(!card.Assignments.All(a=>a.Manage))throw new HttpException(403,"Subscriber ownership is required.");
        Person.Text=HttpUtility.HtmlEncode(card.Name);PersonInitials.Text=HttpUtility.HtmlEncode(card.Initials);PersonEmail.Text=HttpUtility.HtmlEncode(card.Email);PersonPhone.Text=HttpUtility.HtmlEncode(card.Phone);
        PersonStatus.Text=HttpUtility.HtmlEncode(card.Status);PersonStatus.CssClass="am-badge "+card.StatusCss;
        Existing.DataSource=card.Assignments;Existing.DataBind();Cancel.NavigateUrl="EmployeeDetails.aspx?EmployeeId="+employee+"&tab=assignments";
    }
    protected void Page_Load(object sender,EventArgs e)
    {
        if(IsPostBack)return;
        Business.Items.Add(new ListItem("Select a business",""));
        foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)if(AnyminuteModernScope.Owns(Convert.ToInt32(b["BusinessID"])))Business.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]),Convert.ToString(b["BusinessID"])));
        string selected=Request.QueryString["BusinessId"];
        if(selected!=null){if(Business.Items.FindByValue(selected)==null)throw new HttpException(403,"Business outside ownership.");Business.SelectedValue=selected;}
        TitleText.Text=selected==null?"Add Business Assignment":"Edit Business Assignment";BindRoles();
    }
    protected void BusinessChanged(object sender,EventArgs e){Message.Text="";BindRoles();}
    private void BindRoles()
    {
        Role.Items.Clear();Role.Items.Add(new ListItem("Select a role",""));int b;
        if(!Int32.TryParse(Business.SelectedValue,out b))return;
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            AnyminuteApprovalAssignments.RequireOwner(c,null,b);
            foreach(DataRow row in AnyminuteTimesheetRepair.Query(c,null,"SELECT User_TypeId,UserType FROM UserType u WHERE u.User_TypeId IN(2,4,5,12) OR (u.User_TypeId NOT IN(1,2,3,4,5,6,12) AND EXISTS(SELECT 1 FROM AssginBussiness owner WHERE owner.BusinessID=@b AND owner.UserTypeId=1 AND owner.EmployeeID=u.BownerID)) ORDER BY UserType","@b",b).Rows)Role.Items.Add(new ListItem(Convert.ToString(row["UserType"]),Convert.ToString(row["User_TypeId"])));
            var current=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT UserTypeId FROM AssginBussiness WHERE BusinessID=@b AND EmployeeID=@e","@b",b,"@e",employee);
            if(current.Rows.Count==1&&Role.Items.FindByValue(Convert.ToString(current.Rows[0][0]))!=null)Role.SelectedValue=Convert.ToString(current.Rows[0][0]);
        }
    }
    protected void Save_Click(object sender,EventArgs e)
    {
        int b,r;if(!Int32.TryParse(Business.SelectedValue,out b)||!Int32.TryParse(Role.SelectedValue,out r)){Message.Text="Select a business and role.";return;}
        try{AnyminuteEmployeeAssignmentService.SaveRole(employee,b,r);Response.Redirect("EmployeeDetails.aspx?EmployeeId="+employee+"&tab=assignments",false);Context.ApplicationInstance.CompleteRequest();}
        catch(InvalidOperationException error){Message.Text=HttpUtility.HtmlEncode(error.Message);}
    }
}
