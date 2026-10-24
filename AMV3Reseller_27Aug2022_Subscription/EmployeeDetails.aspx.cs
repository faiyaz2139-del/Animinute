using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteEmployeeDetails : System.Web.UI.Page
{
    private AnyminuteEmployeeCardView card;
    private int employee;
    private string Tab {get {string value=Request.QueryString["tab"]??"profile";return new[]{"profile","address","assignments","pay","projects","time","access","activity"}.Contains(value)?value:"profile";}}
    protected void Page_Init(object sender,EventArgs e)
    {
        AnyminuteDirectoryScope.RequireDirectoryRole();ViewStateUserKey=Session.SessionID;
        if(!Int32.TryParse(Request.QueryString["EmployeeId"],out employee)||employee<=0)throw new HttpException(400,"Select an employee.");
        card=AnyminuteEmployeesView.RequireEmployee(employee);
    }
    protected void Page_Load(object sender,EventArgs e)
    {
        if(!IsPostBack)
        {
            foreach(var a in card.Assignments.GroupBy(x=>new{x.BusinessId,x.EmployeeId}).Select(x=>x.First()))ContextAssignment.Items.Add(new ListItem(a.Business,a.BusinessId+":"+a.EmployeeId));
            if(Request.QueryString["BusinessId"]!=null || Request.QueryString["RecordId"]!=null)
            {
                int b,e;
                if(!Int32.TryParse(Request.QueryString["BusinessId"],out b)||!Int32.TryParse(Request.QueryString["RecordId"],out e)||ContextAssignment.Items.FindByValue(b+":"+e)==null)throw new HttpException(403,"Assignment outside your access.");
                ContextAssignment.SelectedValue=b+":"+e;
            }
        }
        Bind();
    }
    protected void ContextChanged(object sender,EventArgs e){ /* Page_Load already binds posted, reauthorized selection. */ }
    protected void AssignmentCommand(object sender,GridViewCommandEventArgs e) { }
    protected void Deactivate_Click(object sender,EventArgs e)
    {
        if(Request.QueryString["action"]!="deactivate")throw new HttpException(400,"Open the Deactivate Employee action first.");
        try{AnyminuteEmployeeAssignmentService.Deactivate(employee);card=AnyminuteEmployeesView.RequireEmployee(employee);Notice.Text="Employee status updated. Timecards and pay history were retained. Login access was not changed.";Notice.Visible=true;Bind();}
        catch(InvalidOperationException error){Notice.Text=HttpUtility.HtmlEncode(error.Message);Notice.Visible=true;}
    }
    private string Url(string tab){var parts=ContextAssignment.SelectedValue.Split(':');return "EmployeeDetails.aspx?EmployeeId="+employee+"&BusinessId="+parts[0]+"&RecordId="+parts[1]+"&tab="+tab;}
    private void Bind()
    {
        var context=card.Assignments.FirstOrDefault(a=>ContextAssignment.SelectedValue==a.BusinessId+":"+a.EmployeeId);
        if(context==null)throw new HttpException(403,"This assignment is outside your current access.");
        Initials.Text=HttpUtility.HtmlEncode(card.Initials);EmployeeName.Text=HttpUtility.HtmlEncode(card.Name);Email.Text=HttpUtility.HtmlEncode(card.Email);Phone.Text=HttpUtility.HtmlEncode(card.Phone);EmployeeStatus.Text=HttpUtility.HtmlEncode(card.Status);
        ProfileTab.NavigateUrl=Url("profile");AddressTab.NavigateUrl=Url("address");AssignmentsTab.NavigateUrl=Url("assignments");PayTab.NavigateUrl=Url("pay");ProjectsTab.NavigateUrl=Url("projects");TimeTab.NavigateUrl=Url("time");AccessTab.NavigateUrl=Url("access");ActivityTab.NavigateUrl=Url("activity");
        ProfilePane.Visible=Tab=="profile";AddressPane.Visible=Tab=="address";AssignmentsPane.Visible=Tab=="assignments";PayPane.Visible=Tab=="pay";ProjectsPane.Visible=Tab=="projects";TimePane.Visible=Tab=="time";AccessPane.Visible=Tab=="access";ActivityPane.Visible=Tab=="activity";
        bool manage=false;string editor=null;
        if(context.Manage)
            try{editor=AnyminuteEmployeesView.EditUrl(context,"profile");manage=true;}
            catch(HttpException denied){if(denied.GetHttpCode()!=403)throw;}
            catch(InvalidOperationException){manage=false;}
        EditBasic.Visible=PayEditor.Visible=ProjectEditor.Visible=manage;
        Deactivate.Visible=manage && card.Active && Request.QueryString["action"]=="deactivate";
        AddressGrid.Visible=AccessGrid.Visible=manage;
        if(!manage){AddressGrid.DataSource=null;AddressGrid.DataBind();AccessGrid.DataSource=null;AccessGrid.DataBind();}
        if(manage){EditBasic.NavigateUrl=editor;PayEditor.NavigateUrl=AnyminuteEmployeesView.EditUrl(context,"pay");ProjectEditor.NavigateUrl=AnyminuteEmployeesView.EditUrl(context,"projects");}
        if(Request.QueryString["edit"]=="1"&&!IsPostBack)
        {
            if(!manage)throw new HttpException(403,"You cannot edit the account-wide details of this employee.");
            Response.Redirect(editor,false);Context.ApplicationInstance.CompleteRequest();return;
        }
        Assignments.DataSource=card.Assignments;Assignments.DataBind();AddAssignment.Visible=manage;AddAssignment.NavigateUrl="EmployeeAssignment.aspx?EmployeeId="+context.EmployeeId;
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            if(ProfilePane.Visible)
            {
                var profile=new DataTable();profile.Columns.Add("Field");profile.Columns.Add("Value");
                profile.Rows.Add("Name",card.Name);profile.Rows.Add("Email",card.Email);profile.Rows.Add("Phone",card.Phone);profile.Rows.Add("Status",card.Status);profile.Rows.Add("Roles",card.Roles);ProfileGrid.DataSource=profile;ProfileGrid.DataBind();
            }
            if(AddressPane.Visible)
            {
                AddressScope.Text=manage?"":"Address is restricted to owners who can manage the employee's shared record.";
                if(manage){AddressGrid.DataSource=AnyminuteTimesheetRepair.Query(c,null,"SELECT ad.StreetNo AS [Street Number],ad.StreetName AS [Address Line 1],ad.StreetType AS [Address Line 2],ad.SuiteNumber AS [Suite],ad.City,ad.Province,ad.Postalcode FROM EmpAddress ad JOIN AddEmployee e ON e.ADDRESSID=ad.EmpAddressId WHERE e.EmployeeID=@e","@e",context.EmployeeId);AddressGrid.DataBind();}
            }
            if(PayPane.Visible){Rates.DataSource=AnyminuteTimesheetRepair.Query(c,null,"SELECT RateId,Rate,RateType,StartDate,EndDate,effectivedate FROM RateDetails WHERE EmployeeId=@e ORDER BY effectivedate DESC,RateId DESC","@e",context.EmployeeId);Rates.DataBind();}
            if(ProjectsPane.Visible){Projects.DataSource=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT p.ProjectName,p.StartDate,p.EndDate FROM ASSIGNPROJECT a JOIN AddProject p ON p.ProjectID=a.PROJECTID AND p.BusinessID=a.BUSINESSID WHERE a.BUSINESSID=@b AND a.EMPLOYEEID=@e","@b",context.BusinessId,"@e",context.EmployeeId);Projects.DataBind();}
            if(TimePane.Visible)OpenTime.NavigateUrl="WeekTimeCard.aspx?BusinessId="+context.BusinessId+"&EmployeeId="+context.EmployeeId;
            if(AccessPane.Visible)
            {
                AccessMessage.Text=manage?"Use the employee sign-in email shown below. Passwords are never displayed.":"Account details are restricted to authorized owners.";
                if(manage){int user=AnyminuteFormAccess.TargetAccount(context.BusinessId,context.EmployeeId);if(user>0){AccessGrid.DataSource=AnyminuteTimesheetRepair.Query(c,null,"SELECT Email AS [Login Email],Status AS [Account Enabled] FROM UserAcc WHERE UserId=@u","@u",user);AccessGrid.DataBind();}else AccessMessage.Text="No linked login account is recorded for this assignment.";}
            }
            if(ActivityPane.Visible){Activity.DataSource=AnyminuteTimesheetRepair.Query(c,null,"SELECT Action,ActorID AS [Changed By Employee ID],RecordedAtUTC AS [Recorded UTC] FROM AnyminuteApprovalAudit WHERE BusinessID=@b AND EmployeeID=@e ORDER BY RecordedAtUTC DESC,AuditID DESC","@b",context.BusinessId,"@e",context.EmployeeId);Activity.DataBind();}
        }
    }
}
