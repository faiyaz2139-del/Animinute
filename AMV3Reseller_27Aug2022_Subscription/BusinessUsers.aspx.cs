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
        // 2026-10-26 v19 (t78u): the intermediate "Edit User" picker is retired. Old links land on User Access > Login & Access
        // for this business, whose rows open the real employee editor (Login & Access tab).
        Response.Redirect("~/LoginAccess.aspx?BusinessId="+business,false);Context.ApplicationInstance.CompleteRequest();
    }
    protected void Page_Load(object s,EventArgs e)
    {
        if(IsPostBack || Response.IsRequestBeingRedirected) return;
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            Users.DataSource=AnyminuteTimesheetRepair.Query(c,null,@"SELECT DISTINCT a.EmployeeID,b.BusinessID,
                dbo.FN_DESC('AddEmployee',a.EmployeeID) AS empname FROM AddEmployee a
                JOIN AssginBussiness b ON b.EmployeeID=a.EmployeeID WHERE b.BusinessID=@b ORDER BY empname", "@b",business);
            Users.DataBind();
        }
    }
}
