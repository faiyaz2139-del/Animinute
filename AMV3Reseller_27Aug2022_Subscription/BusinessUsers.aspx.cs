using System;
using System.Web;
using System.Web.UI;
public partial class BusinessUsers : Page
{
    private int business;
    protected void Page_Init(object s,EventArgs e)
    {
        if(Session["EmployeeId"]==null) { Response.Redirect("Login.aspx");return; }
        ViewStateUserKey=Session.SessionID;
        if(!Int32.TryParse(Request.QueryString["BusinessId"],out business)) throw new HttpException(400,"Select a business.");
        using(var c=AnyminuteTimesheetRepair.Open()) AnyminuteApprovalAssignments.RequireOwner(c,null,business);
    }
    protected void Page_Load(object s,EventArgs e)
    {
        if(IsPostBack) return;
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            Users.DataSource=AnyminuteTimesheetRepair.Query(c,null,@"SELECT DISTINCT a.EmployeeID,b.BusinessID,
                dbo.FN_DESC('AddEmployee',a.EmployeeID) AS empname FROM AddEmployee a
                JOIN AssginBussiness b ON b.EmployeeID=a.EmployeeID WHERE b.BusinessID=@b ORDER BY empname", "@b",business);
            Users.DataBind();
        }
    }
}
