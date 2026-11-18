using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
// v86 (Employee Details deck): every panel rebuilt to the deck. Reads only; the only writes are the existing remove-assignment /
// deactivate actions, owner-requested invitation / password-reset emails (existing SendInvitation), and document upload/delete.
public partial class AnyminuteEmployeeDetails : System.Web.UI.Page
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private AnyminuteEmployeeCardView card;
    private int employee;
    protected bool CanManage;
    private bool manage;
    private AnyminuteEmployeeAssignmentView context;
    private Dictionary<int, string> jobTitles = new Dictionary<int, string>();
    private string Tab {get {string value=Request.QueryString["tab"]??"profile";return new[]{"profile","address","assignments","pay","projects","time","documents","access","activity"}.Contains(value)?value:"profile";}}
    protected void Page_Init(object sender,EventArgs e)
    {
        AnyminuteDirectoryScope.RequireDirectoryRole();ViewStateUserKey=Session.SessionID;
        if(!Int32.TryParse(Request.QueryString["EmployeeId"],out employee)||employee<=0)throw new HttpException(400,"Select an employee.");
        card=AnyminuteEmployeesView.RequireEmployee(employee);
    }
    protected void Page_Load(object sender,EventArgs e)
    {
        string flash=AnyminuteFlash.Take("details");if(flash!=null){FlashText.Text=HttpUtility.HtmlEncode(flash);Flash.Visible=true;}
        if(!IsPostBack)
        {
            foreach(var a in card.Assignments.GroupBy(x=>new{x.BusinessId,x.EmployeeId}).Select(x=>x.First()))ContextAssignment.Items.Add(new ListItem(a.Business,a.BusinessId+":"+a.EmployeeId));
            if(Request.QueryString["BusinessId"]!=null || Request.QueryString["RecordId"]!=null)
            {
                int b,record; // "record" (not "e"): a local named e clashes with the EventArgs parameter (CS0136) and stops the page compiling.
                if(!Int32.TryParse(Request.QueryString["BusinessId"],out b)||!Int32.TryParse(Request.QueryString["RecordId"],out record)||ContextAssignment.Items.FindByValue(b+":"+record)==null)throw new HttpException(403,"Assignment outside your access.");
                ContextAssignment.SelectedValue=b+":"+record;
            }
        }
        if(Request.QueryString["doc"]!=null&&!IsPostBack){SendDocument();return;}
        Bind();
    }
    protected void ContextChanged(object sender,EventArgs e){ /* Page_Load already binds posted, reauthorized selection. */ }
    protected void AssignmentCommand(object sender,RepeaterCommandEventArgs e)
    {
        int b;if(e.CommandName!="RemoveAssignment"||!Int32.TryParse(Convert.ToString(e.CommandArgument),out b))return;
        try{AnyminuteEmployeeEditor.RemoveAssignment(employee,b);AnyminuteFlash.Set("details","Business assignment removed.");
            bool exists=true;try{AnyminuteEmployeesView.RequireEmployee(employee);}catch(HttpException){exists=false;}
            Response.Redirect(exists?"EmployeeDetails.aspx?EmployeeId="+employee+"&tab=assignments":"Users.aspx",false);Context.ApplicationInstance.CompleteRequest();}
        catch(InvalidOperationException error){ShowNotice(error.Message);}
    }
    protected void Deactivate_Click(object sender,EventArgs e)
    {
        if(Request.QueryString["action"]!="deactivate")throw new HttpException(400,"Open the Deactivate Employee action first.");
        try{AnyminuteEmployeeAssignmentService.Deactivate(employee);card=AnyminuteEmployeesView.RequireEmployee(employee);ShowNotice("Employee status updated. Timecards and pay history were retained. Login access was not changed.");Bind();}
        catch(InvalidOperationException error){ShowNotice(error.Message);}
    }
    private void ShowNotice(string text){Notice.Text=HttpUtility.HtmlEncode(text);Notice.Visible=true;}
    private string Url(string tab){var parts=ContextAssignment.SelectedValue.Split(':');return "EmployeeDetails.aspx?EmployeeId="+employee+"&BusinessId="+parts[0]+"&RecordId="+parts[1]+"&tab="+tab;}
    private IEnumerable<int> ScopeBusinesses{get{return card.Assignments.Select(a=>a.BusinessId).Distinct();}}

    // ---------- small view helpers ----------
    static string S(object o){return o==null||o==DBNull.Value?"":Convert.ToString(o).Trim();}
    protected static string Money(object v){if(v==null||v==DBNull.Value)return "\u2014";return "$"+Convert.ToDecimal(v).ToString("N2",Inv);}
    protected static string LongDate(object v){if(v==null||v==DBNull.Value)return "\u2014";return Convert.ToDateTime(v).ToString("MMM dd, yyyy",Inv);}
    protected static string Pill(string status)
    {
        switch(status){case "Active":case "Approved":case "Enabled":case "Accepted":case "Verified":case "Completed":return "am-ed-pill am-pill-ok";
            case "Pending":case "Scheduled":case "Invitation Pending":return "am-ed-pill am-pill-warn";
            case "On Hold":return "am-ed-pill am-pill-info";
            case "Rejected":case "Disabled":case "Expired":case "Terminated":return "am-ed-pill am-pill-bad";
            default:return "am-ed-pill am-pill-grey";}
    }
    protected static string StatusIcon(object status)
    {
        switch(Convert.ToString(status)){case "Approved":return I("check");case "Pending":return I("clock");case "On Hold":return I("pause");case "Completed":return I("check");default:return "";}
    }
    protected static string AssignStatus(object status,object end){string s=Convert.ToString(status);if(s=="Active")return "Active";if(end!=null&&end!=DBNull.Value&&Convert.ToDateTime(end).Date<=DateTime.Today)return "Ended";return s.Length>0?s:"\u2014";}
    protected static string RoleChip(object role){string r=AnyminuteEmployeeEditor.RoleDisplay(role).ToLowerInvariant();return r.Contains("manager")||r.Contains("supervisor")?"am-chip-purple":r.Contains("admin")?"am-chip-amber":r.Contains("account")?"am-chip-green":"am-chip-blue";}
    protected string JobTitle(object recordId){string t;return jobTitles.TryGetValue(Convert.ToInt32(recordId),out t)&&t.Length>0?t:"\u2014";}
    protected static string FileBadge(object name){string ext=System.IO.Path.GetExtension(Convert.ToString(name)).TrimStart('.').ToUpperInvariant();return ext=="JPEG"?"JPG":ext;}
    protected string DocUrl(object id,bool download){return Url("documents")+"&doc="+HttpUtility.UrlEncode(Convert.ToString(id))+(download?"&dl=1":"");}
    static string Hours(double h){return h.ToString("0.00",Inv)+" h";}
    static string Local(object utc,string format){if(utc==null||utc==DBNull.Value)return "\u2014";return DateTime.SpecifyKind(Convert.ToDateTime(utc),DateTimeKind.Utc).ToLocalTime().ToString(format,Inv);}
    static double Span(object v){TimeSpan t;string s=S(v);return s.Length>0&&TimeSpan.TryParse(s,Inv,out t)?t.TotalHours:0;}
    static string RateUnit(string type){return type.Equals("Hourly",StringComparison.OrdinalIgnoreCase)?"/hr":type.Length>0?" / "+type:"";}

    // Line icons used by the deck (stroke = currentColor).
    static readonly Dictionary<string,string> Icons=new Dictionary<string,string>{
        {"user","<circle cx='12' cy='8' r='4'/><path d='M4 21c0-4 4-6 8-6s8 2 8 6'/>"},
        {"users","<circle cx='9' cy='8' r='3.5'/><path d='M2.5 20c0-3.5 3-5.5 6.5-5.5s6.5 2 6.5 5.5'/><path d='M16 4.5a3.5 3.5 0 0 1 0 7'/><path d='M18 14.6c2.2.6 3.5 2.4 3.5 5.4'/>"},
        {"briefcase","<rect x='3' y='7' width='18' height='13' rx='2'/><path d='M8 7V5a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2M3 12h18'/>"},
        {"dollar","<path d='M12 2v20M17 6.5c-1-1.5-2.8-2-5-2-2.8 0-4.5 1.3-4.5 3.3 0 4.7 9.5 2.4 9.5 7.2 0 2-1.9 3.5-5 3.5-2.4 0-4.2-.8-5.2-2.3'/>"},
        {"clock","<circle cx='12' cy='12' r='9'/><path d='M12 7v5l3 2'/>"},
        {"file","<path d='M14 3H7a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h10a2 2 0 0 0 2-2V8z'/><path d='M14 3v5h5M9 13h6M9 17h6'/>"},
        {"pin","<path d='M12 21s-7-6.2-7-12a7 7 0 0 1 14 0c0 5.8-7 12-7 12z'/><circle cx='12' cy='9' r='2.5'/>"},
        {"layers","<path d='M12 3 2 8l10 5 10-5z'/><path d='m2 13 10 5 10-5M2 17.5l10 5 10-5'/>"},
        {"lock","<rect x='4' y='10' width='16' height='11' rx='2'/><path d='M8 10V7a4 4 0 0 1 8 0v3'/>"},
        {"history","<path d='M3 12a9 9 0 1 0 3-6.7L3 8'/><path d='M3 3v5h5M12 7v5l3 2'/>"},
        {"mail","<rect x='3' y='5' width='18' height='14' rx='2'/><path d='m3 7 9 6 9-6'/>"},
        {"phone","<path d='M5 3h4l2 5-2.5 1.5a11 11 0 0 0 6 6L16 13l5 2v4a2 2 0 0 1-2 2A16 16 0 0 1 3 5a2 2 0 0 1 2-2z'/>"},
        {"building","<rect x='4' y='3' width='16' height='18' rx='1'/><path d='M9 7h1M14 7h1M9 11h1M14 11h1M9 15h1M14 15h1M10 21v-3h4v3'/>"},
        {"pencil","<path d='M4 20h4L19 9l-4-4L4 16z'/><path d='m13.5 6.5 4 4'/>"},
        {"eye","<path d='M2 12s3.6-7 10-7 10 7 10 7-3.6 7-10 7S2 12 2 12z'/><circle cx='12' cy='12' r='3'/>"},
        {"plus","<circle cx='12' cy='12' r='9'/><path d='M12 8v8M8 12h8'/>"},
        {"download","<path d='M12 4v11M7 10l5 5 5-5M5 20h14'/>"},
        {"upload","<path d='M12 16V5M7 10l5-5 5 5M5 20h14'/>"},
        {"search","<circle cx='11' cy='11' r='7'/><path d='m20 20-4-4'/>"},
        {"refresh","<path d='M4 12a8 8 0 1 0 2.4-5.7L4 8.5'/><path d='M4 4v4.5h4.5'/>"},
        {"calendar","<rect x='3' y='5' width='18' height='16' rx='2'/><path d='M3 10h18M8 3v4M16 3v4'/>"},
        {"check","<path d='m5 12.5 4.5 4.5L19 7.5'/>"},
        {"checkcircle","<circle cx='12' cy='12' r='9'/><path d='m8 12.5 3 3 5-6'/>"},
        {"home","<path d='M3 11 12 4l9 7'/><path d='M5 10v10h14V10M10 20v-5h4v5'/>"},
        {"shield","<path d='M12 3 4 6v6c0 5 3.5 8 8 9 4.5-1 8-4 8-9V6z'/><path d='m9 12 2 2 4-4'/>"},
        {"key","<circle cx='8' cy='15' r='4'/><path d='m11 12 9-9M17 6l3 3M15 8l2 2'/>"},
        {"send","<path d='M21 3 3 10.5l7 2.5 2.5 7z'/><path d='m10 13 5-5'/>"},
        {"smartphone","<rect x='7' y='2' width='10' height='20' rx='2'/><path d='M11 18h2'/>"},
        {"info","<circle cx='12' cy='12' r='9'/><path d='M12 11v5M12 8h.01'/>"},
        {"folder","<path d='M3 6a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v10a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z'/>"},
        {"tag","<path d='M3 12V4h8l10 10-8 8z'/><circle cx='7.5' cy='8.5' r='1.5'/>"},
        {"coins","<ellipse cx='12' cy='6' rx='7' ry='3'/><path d='M5 6v6c0 1.7 3.1 3 7 3s7-1.3 7-3V6M5 12v6c0 1.7 3.1 3 7 3s7-1.3 7-3v-6'/>"},
        {"activity","<path d='M3 12h4l3-7 4 14 3-7h4'/>"},
        {"dots","<circle cx='12' cy='5' r='1.6' fill='currentColor'/><circle cx='12' cy='12' r='1.6' fill='currentColor'/><circle cx='12' cy='19' r='1.6' fill='currentColor'/>"},
        {"chevron","<path d='m6 9 6 6 6-6'/>"},
        {"play","<circle cx='12' cy='12' r='9'/><path d='m10 8.5 5 3.5-5 3.5z'/>"},
        {"pause","<path d='M9 6v12M15 6v12'/>"},
        {"add","<path d='M12 5v14M5 12h14'/>"},
        {"gear","<circle cx='12' cy='12' r='3'/><path d='M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1a1.7 1.7 0 0 0-1.1-1.5 1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1a1.7 1.7 0 0 0 1.5-1.1 1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1z'/>"}
    };
    protected static string I(string name){string p;if(!Icons.TryGetValue(name??"",out p))return "";return "<svg class='am-ed-ico' viewBox='0 0 24 24' width='18' height='18' fill='none' stroke='currentColor' stroke-width='1.8' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'>"+p+"</svg>";}

    private void Bind()
    {
        context=card.Assignments.FirstOrDefault(a=>ContextAssignment.SelectedValue==a.BusinessId+":"+a.EmployeeId);
        if(context==null)throw new HttpException(403,"This assignment is outside your current access.");
        Initials.Text=HttpUtility.HtmlEncode(card.Initials);EmployeeName.Text=HttpUtility.HtmlEncode(card.Name);Email.Text=HttpUtility.HtmlEncode(card.Email);Phone.Text=HttpUtility.HtmlEncode(card.Phone);
        EmployeeStatus.Text="<i></i>"+HttpUtility.HtmlEncode(card.Status);EmployeeStatus.CssClass=Pill(card.Status)+" am-ed-dot";
        var tabs=new[]{new{L=ProfileTab,K="profile",T="Profile",Ic="user"},new{L=AssignmentsTab,K="assignments",T="Business Assignments",Ic="briefcase"},new{L=PayTab,K="pay",T="Pay Rate History",Ic="dollar"},new{L=TimeTab,K="time",T="Time Cards",Ic="clock"},new{L=DocumentsTab,K="documents",T="Documents",Ic="file"},
            new{L=AddressTab,K="address",T="Address",Ic="pin"},new{L=ProjectsTab,K="projects",T="Projects",Ic="layers"},new{L=AccessTab,K="access",T="Login &amp; Access",Ic="lock"},new{L=ActivityTab,K="activity",T="Activity History",Ic="history"}};
        foreach(var t in tabs){t.L.NavigateUrl=Url(t.K);t.L.Text=I(t.Ic)+"<span>"+t.T+"</span>";t.L.CssClass=t.K==Tab?"is-active":"";}
        ProfilePane.Visible=Tab=="profile";AddressPane.Visible=Tab=="address";AssignmentsPane.Visible=Tab=="assignments";PayPane.Visible=Tab=="pay";ProjectsPane.Visible=Tab=="projects";TimePane.Visible=Tab=="time";DocumentsPane.Visible=Tab=="documents";AccessPane.Visible=Tab=="access";ActivityPane.Visible=Tab=="activity";
        manage=false;string editor=null;
        if(context.Manage)
            try{editor=AnyminuteEmployeesView.EditUrl(context,"profile");manage=true;}
            catch(HttpException denied){if(denied.GetHttpCode()!=403)throw;}
            catch(InvalidOperationException){manage=false;}
        EditBasic.Visible=ProjectEditor.Visible=ManageAssignments.Visible=manage;
        Deactivate.Visible=manage && card.Active && Request.QueryString["action"]=="deactivate";DeactivateBox.Visible=Deactivate.Visible;
        EditBasic.Text=I("pencil")+"Edit";
        if(manage){EditBasic.NavigateUrl=editor;ProjectEditor.NavigateUrl=AnyminuteEmployeesView.EditUrl(context,"projects");}
        ProjectEditor.Text=I("add")+"Assign Project";ManageAssignments.Text=I("gear")+"Manage Assignments";ManageAssignments.NavigateUrl="Projects.aspx";
        if(Request.QueryString["edit"]=="1"&&!IsPostBack)
        {
            if(!manage)throw new HttpException(403,"You cannot edit the account-wide details of this employee.");
            Response.Redirect(editor,false);Context.ApplicationInstance.CompleteRequest();return;
        }
        // 2026-10-24 v2 (new ppt slide 7): Edit opens the Edit Employee screen; assignment rows show roles/pay and edit/delete.
        CanManage=card.Assignments.All(a=>a.Manage);if(CanManage){EditBasic.Visible=true;EditBasic.NavigateUrl="EmployeeEditor.aspx?EmployeeId="+employee+"&tab=profile";}
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            if(ProfilePane.Visible)BindProfile(c);
            if(AssignmentsPane.Visible)BindAssignments(c);
            if(PayPane.Visible)BindPay(c);
            if(TimePane.Visible)BindTime(c);
            if(DocumentsPane.Visible)BindDocuments();
            if(AddressPane.Visible)BindAddress(c);
            if(ProjectsPane.Visible)BindProjects(c);
            if(AccessPane.Visible)BindAccess(c);
            if(ActivityPane.Visible)BindActivity(c);
        }
    }

    // ---------- slide 1: Profile ----------
    private void BindProfile(System.Data.SqlClient.SqlConnection c)
    {
        InfoName.Text=HttpUtility.HtmlEncode(card.Name);InfoEmail.Text=HttpUtility.HtmlEncode(card.Email);InfoPhone.Text=HttpUtility.HtmlEncode(card.Phone);
        InfoStatus.Text=HttpUtility.HtmlEncode(card.Status);InfoStatus.CssClass=Pill(card.Status);
        InfoRole.Text=HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(context.Role)?"\u2014":context.Role);InfoType.Text=HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(context.Type)?"\u2014":context.Type);
        InfoBusiness.Text=HttpUtility.HtmlEncode(context.Business);
        var hire=AnyminuteTimesheetRepair.Query(c,null,"SELECT HireDate FROM AddEmployee WHERE EmployeeID=@e","@e",context.EmployeeId);DateTime h;
        InfoJoin.Text=hire.Rows.Count==1&&DateTime.TryParse(S(hire.Rows[0][0]),out h)?h.ToString("MMM dd, yyyy",Inv):"\u2014";
        // "This Period" = the current calendar month: weekly time cards (weeks with entries) for this assignment.
        var first=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
        var weeks=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT TimeSheetDate FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND ISNULL(ProjectId,0)=0 AND TimeSheetDate>=@f AND TimeSheetDate<@t","@b",context.BusinessId,"@e",context.EmployeeId,"@f",first,"@t",first.AddMonths(1));
        StatTime.Text=weeks.Rows.Cast<DataRow>().Select(r=>AnyminuteWeekService.Monday(Convert.ToDateTime(r[0]))).Distinct().Count().ToString(Inv);
        StatProjects.Text=Convert.ToString(AnyminuteTimesheetRepair.Query(c,null,"SELECT COUNT(DISTINCT a.PROJECTID) FROM ASSIGNPROJECT a JOIN AddProject p ON p.ProjectID=a.PROJECTID AND p.BusinessID=a.BUSINESSID WHERE a.BUSINESSID=@b AND a.EMPLOYEEID=@e AND ISNULL(p.Status,'')<>'D'","@b",context.BusinessId,"@e",context.EmployeeId).Rows[0][0],Inv);
        StatDocs.Text=AnyminuteEmployeeDocuments.List(card.EmployeeId,ScopeBusinesses).Count.ToString(Inv);
    }

    // ---------- slide 2: Business Assignments ----------
    private void BindAssignments(System.Data.SqlClient.SqlConnection c)
    {
        var rows=AnyminuteEmployeeEditor.Assignments(card);
        foreach(var r in rows){var t=AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP 1 JobTitle FROM RateDetails WHERE EmployeeId=@e ORDER BY effectivedate DESC,RateId DESC","@e",r.EmployeeId);jobTitles[r.EmployeeId]=t.Rows.Count==1?S(t.Rows[0][0]):"";}
        Assignments.DataSource=rows;Assignments.DataBind();
        AddAssignment.Visible=CanManage;AddAssignment.NavigateUrl="EmployeeAssignment.aspx?EmployeeId="+employee+"&mode=add";AddAssignment.Text=I("plus")+"Add Assignment";
    }

    // ---------- slide 3: Pay Rate History ----------
    private sealed class RateView{public string Eff{get;set;}public string EffSort{get;set;}public string End{get;set;}public string EndSort{get;set;}public string Type{get;set;}public string Rate{get;set;}public string RateSort{get;set;}public string Billing{get;set;}public string By{get;set;}public string Reason{get;set;}public string Status{get;set;}public decimal Amount;public DateTime? Date;}
    private List<RateView> LoadRates(System.Data.SqlClient.SqlConnection c)
    {
        var t=AnyminuteEmployeeEditor.RateHistory(context.EmployeeId);
        var by=new Dictionary<string,string>();
        if(AnyminuteEmployeeEditor.HasTable(c,null,"AnyminuteRateChangeReason"))
            foreach(DataRow r in AnyminuteTimesheetRepair.Query(c,null,"SELECT x.RateId,dbo.FN_DESC('AddEmployee',x.ChangedBy) Name FROM AnyminuteRateChangeReason x JOIN RateDetails r ON r.RateId=x.RateId WHERE r.EmployeeId=@e","@e",context.EmployeeId).Rows)by[S(r["RateId"])]=S(r["Name"]);
        var current=AnyminuteEmployeeEditor.Assignments(card).FirstOrDefault(a=>a.BusinessId==context.BusinessId&&a.EmployeeId==context.EmployeeId);
        var list=new List<RateView>();
        foreach(DataRow r in t.Rows)
        {
            DateTime? eff=r["effectivedate"]==DBNull.Value?(DateTime?)null:Convert.ToDateTime(r["effectivedate"]);
            bool isCurrent=Convert.ToBoolean(r["Current"]);string type=S(r["RateType"]);decimal rate=Convert.ToDecimal(r["Rate"]);
            bool hasEnd=S(r["Is_Having_EndDate"])=="True"&&r["EndDate"]!=DBNull.Value;DateTime? end=hasEnd?Convert.ToDateTime(r["EndDate"]):(DateTime?)null;
            string name;by.TryGetValue(S(r["RateId"]),out name);
            list.Add(new RateView{Eff=eff.HasValue?eff.Value.ToString("MMM d, yyyy",Inv):"\u2014",EffSort=eff.HasValue?eff.Value.ToString("yyyyMMdd",Inv):"",End=end.HasValue?end.Value.ToString("MMM d, yyyy",Inv):"\u2014",EndSort=end.HasValue?end.Value.ToString("yyyyMMdd",Inv):"",
                Type=type.Length>0?type:"\u2014",Rate="$"+rate.ToString("N2",Inv)+RateUnit(type),RateSort=rate.ToString("0.00",Inv),Billing=isCurrent&&current!=null&&current.BillingRate.HasValue?"$"+current.BillingRate.Value.ToString("N2",Inv)+RateUnit(type):"\u2014",
                By=String.IsNullOrWhiteSpace(name)?"\u2014":name,Reason=S(r["Reason"]).Length>0?S(r["Reason"]):"\u2014",Status=isCurrent?"Active":eff.HasValue&&eff.Value>DateTime.Now?"Scheduled":"Ended",Amount=rate,Date=eff});
        }
        return list;
    }
    private void BindPay(System.Data.SqlClient.SqlConnection c)
    {
        var list=LoadRates(c);
        Rates.DataSource=list;Rates.DataBind();RatesEmpty.Visible=list.Count==0;ExportRates.Text=I("download")+"Export";
        var cur=list.FirstOrDefault(r=>r.Status=="Active");
        string type=cur==null?"":cur.Type;
        CurrentRateLabel.Text=type=="Hourly"||type.Length==0?"Current Hourly Rate (Pay Rate)":"Current "+HttpUtility.HtmlEncode(type)+" Rate (Pay Rate)";
        CurrentRate.Text=HttpUtility.HtmlEncode(cur==null?"\u2014":cur.Rate);CurrentBilling.Text=HttpUtility.HtmlEncode(cur==null?"\u2014":cur.Billing);
        CurrentEffective.Text=HttpUtility.HtmlEncode(cur==null?"\u2014":cur.Eff);CurrentType.Text=HttpUtility.HtmlEncode(cur==null?"\u2014":cur.Type);
        CurrentStatus.Text=cur==null?"\u2014":"Active";CurrentStatus.CssClass=cur==null?"":Pill("Active");
        var sb=new StringBuilder("<ol class='am-ed-timeline'>");
        for(int i=0;i<list.Count;i++)
        {
            var r=list[i];bool initial=i==list.Count-1;
            sb.Append("<li class='").Append(i==0?"is-current":"").Append("'><span class='am-ed-tl-dot'></span><strong class='am-ed-tl-date'>").Append(HttpUtility.HtmlEncode(r.Eff)).Append("</strong><div class='am-ed-tl-card'><b>")
              .Append(initial?I("plus")+"Initial rate set":I("pencil")+"Pay rate updated").Append("</b><span>")
              .Append(HttpUtility.HtmlEncode(initial?r.Rate:list[i+1].Rate+" \u2192 "+r.Rate)).Append("</span><span>By ").Append(HttpUtility.HtmlEncode(r.By)).Append("</span><span>Reason: ").Append(HttpUtility.HtmlEncode(r.Reason)).Append("</span></div></li>");
        }
        if(list.Count==0)sb.Append("<li class='am-ed-empty'>No pay rate changes recorded.</li>");
        RateAudit.Text=sb.Append("</ol>").ToString();
    }
    protected void ExportRates_Click(object sender,EventArgs e)
    {
        context=card.Assignments.First(a=>ContextAssignment.SelectedValue==a.BusinessId+":"+a.EmployeeId);
        List<RateView> list;using(var c=AnyminuteTimesheetRepair.Open())list=LoadRates(c);
        Csv("pay-rate-history.csv",new[]{"Effective Date","End Date","Pay Type","Pay Rate","Billing Rate","Changed By","Reason / Notes","Status"},list.Select(r=>new[]{r.Eff,r.End,r.Type,r.Rate,r.Billing,r.By,r.Reason,r.Status}));
    }
    private void Csv(string name,string[] head,IEnumerable<string[]> rows)
    {
        Func<string,string> q=v=>{v=v??"";if(v.Length>0&&"=+-@".IndexOf(v[0])>=0)v="'"+v;return "\""+v.Replace("\"","\"\"")+"\"";};
        var sb=new StringBuilder();sb.AppendLine(String.Join(",",head.Select(q)));foreach(var r in rows)sb.AppendLine(String.Join(",",r.Select(q)));
        Response.Clear();Response.ContentType="text/csv";Response.ContentEncoding=Encoding.UTF8;Response.AddHeader("Content-Disposition","attachment; filename="+name);Response.Write("\uFEFF"+sb);
        Response.Flush();Response.SuppressContent=true;Context.ApplicationInstance.CompleteRequest();
    }

    // ---------- slide 4: Time Cards ----------
    private sealed class WeekView{public DateTime Week{get;set;}public string WeekText{get;set;}public string WeekSort{get;set;}public double Total;public double Regular{get;set;}public double Overtime{get;set;}public string RegularText{get;set;}public string OvertimeText{get;set;}public string Status{get;set;}public string SubmittedText{get;set;}public string SubmittedSort{get;set;}public string ApprovedBy{get;set;}public string Url{get;set;}}
    private List<WeekView> LoadWeeks(System.Data.SqlClient.SqlConnection c,DateTime from)
    {
        var map=new Dictionary<DateTime,WeekView>();
        foreach(DataRow r in AnyminuteTimesheetRepair.Query(c,null,"SELECT TimeSheetDate,Total,Over1_5x,Over2x,Status FROM TimeSheetEntry WHERE BussID=@b AND EmpId=@e AND ISNULL(ProjectId,0)=0 AND TimeSheetDate>=@f","@b",context.BusinessId,"@e",context.EmployeeId,"@f",from).Rows)
        {
            var w=AnyminuteWeekService.Monday(Convert.ToDateTime(r["TimeSheetDate"]));WeekView v;if(!map.TryGetValue(w,out v))map[w]=v=new WeekView{Week=w,Status=""};
            double ot=Span(r["Over1_5x"])+Span(r["Over2x"]),total=Span(r["Total"]);v.Total+=total;v.Overtime+=ot;v.Regular+=Math.Max(0,total-ot);
            if(S(r["Status"])!="2")v.Status="Draft";else if(v.Status=="")v.Status="Approved";
        }
        if(AnyminuteEmployeeEditor.HasTable(c,null,"AnyminuteWeek"))
            foreach(DataRow r in AnyminuteTimesheetRepair.Query(c,null,"SELECT WeekStart,State,SubmittedAtUTC,ApprovedBy,dbo.FN_DESC('AddEmployee',ApprovedBy) ApprovedName FROM AnyminuteWeek WHERE BusinessID=@b AND EmployeeID=@e AND WeekStart>=@f","@b",context.BusinessId,"@e",context.EmployeeId,"@f",AnyminuteWeekService.Monday(from)).Rows)
            {
                var w=Convert.ToDateTime(r["WeekStart"]).Date;WeekView v;if(!map.TryGetValue(w,out v)){if(S(r["State"])=="Draft")continue;map[w]=v=new WeekView{Week=w};}
                string st=S(r["State"]);v.Status=st=="Submitted"?"Pending":st.Length>0?st:v.Status;
                if(r["SubmittedAtUTC"]!=DBNull.Value){v.SubmittedText=Local(r["SubmittedAtUTC"],"MMM d, yyyy h:mm tt");v.SubmittedSort=Convert.ToDateTime(r["SubmittedAtUTC"]).ToString("yyyyMMddHHmmss",Inv);}
                if(st=="Approved"&&r["ApprovedBy"]!=DBNull.Value)v.ApprovedBy=S(r["ApprovedName"]);
            }
        var cal=CultureInfo.InvariantCulture.Calendar;
        foreach(var v in map.Values)
        {
            var end=v.Week.AddDays(6);v.WeekText=v.Week.ToString("MMM d",Inv)+(v.Week.Year!=end.Year?", "+v.Week.Year:"")+" \u2013 "+end.ToString(v.Week.Month==end.Month&&v.Week.Year==end.Year?"MMM d, yyyy":"MMM d, yyyy",Inv);
            v.WeekSort=v.Week.ToString("yyyyMMdd",Inv);v.RegularText=Hours(v.Regular);v.OvertimeText=Hours(v.Overtime);
            if(String.IsNullOrEmpty(v.Status))v.Status="Draft";if(String.IsNullOrEmpty(v.SubmittedText)){v.SubmittedText="\u2014";v.SubmittedSort="";}if(String.IsNullOrEmpty(v.ApprovedBy))v.ApprovedBy="\u2014";
            v.Url="WeekTimeCard.aspx?BusinessId="+context.BusinessId+"&EmployeeId="+context.EmployeeId+"&Week="+v.Week.ToString("yyyy-MM-dd",Inv);
        }
        return map.Values.OrderByDescending(v=>v.Week).ToList();
    }
    static string WeekLabel(WeekView v){int n=CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(v.Week,CalendarWeekRule.FirstFourDayWeek,DayOfWeek.Monday);return v.WeekText+" (Week "+n+")";}
    private void BindTime(System.Data.SqlClient.SqlConnection c)
    {
        try{AnyminuteWeekService.ReadAccess(c,null,context.BusinessId,context.EmployeeId);}
        catch(Exception denied){if(!(denied is HttpException)&&!(denied is InvalidOperationException)&&!(denied is UnauthorizedAccessException))throw;TimeEmpty.Text="Time cards for this assignment are restricted to authorized reviewers.";TimeEmpty.Visible=true;TimeRows.DataSource=null;TimeRows.DataBind();KpiTotal.Text=KpiApproved.Text=KpiPending.Text=KpiOvertime.Text="\u2014";TimeClear.NavigateUrl=Url("time");return;}
        var all=LoadWeeks(c,AnyminuteWeekService.Monday(DateTime.Today).AddDays(-7*52));
        string pickedWeek=TimeWeek.SelectedValue;if(!IsPostBack&&Request.QueryString["week"]!=null)pickedWeek=Request.QueryString["week"];
        TimeWeek.Items.Clear();TimeWeek.Items.Add(new ListItem("All weeks (last 52)",""));foreach(var v in all)TimeWeek.Items.Add(new ListItem(WeekLabel(v),v.WeekSort));
        if(pickedWeek==null&&!IsPostBack&&all.Count>0)pickedWeek=all[0].WeekSort;
        if(!IsPostBack&&String.IsNullOrEmpty(Request.QueryString["week"])&&all.Count>0)pickedWeek=all[0].WeekSort;
        if(TimeWeek.Items.FindByValue(pickedWeek??"")!=null)TimeWeek.SelectedValue=pickedWeek;
        if(!IsPostBack&&Request.QueryString["status"]!=null&&TimeStatus.Items.FindByValue(Request.QueryString["status"])!=null)TimeStatus.SelectedValue=Request.QueryString["status"];
        var kpi=TimeWeek.SelectedValue==""?all:all.Where(v=>v.WeekSort==TimeWeek.SelectedValue).ToList();
        KpiTotal.Text=Hours(kpi.Sum(v=>v.Total));KpiApproved.Text=Hours(kpi.Where(v=>v.Status=="Approved").Sum(v=>v.Total));KpiPending.Text=Hours(kpi.Where(v=>v.Status=="Pending").Sum(v=>v.Total));KpiOvertime.Text=Hours(kpi.Sum(v=>v.Overtime));
        var rows=all.Where(v=>TimeStatus.SelectedValue==""||v.Status==TimeStatus.SelectedValue).ToList();
        bool viewAll=Request.QueryString["all"]=="1";TimeRows.DataSource=viewAll?rows:rows.Take(5).ToList();TimeRows.DataBind();TimeEmpty.Visible=rows.Count==0;
        TimeViewAll.Text=viewAll?"Show Recent":"View All "+I("chevron");TimeViewAll.NavigateUrl=Url("time")+(viewAll?"":"&all=1");TimeClear.NavigateUrl=Url("time");
        int n;if(!Int32.TryParse(TrendRange.SelectedValue,out n))n=6;
        var thisWeek=AnyminuteWeekService.Monday(DateTime.Today);var byWeek=all.ToDictionary(v=>v.Week);
        var bars=Enumerable.Range(0,n).Select(i=>thisWeek.AddDays(-7*(n-1-i))).Select(w=>{WeekView v;byWeek.TryGetValue(w,out v);return new{W=w,R=v==null?0:v.Regular,O=v==null?0:v.Overtime};}).ToList();
        double max=Math.Max(10,Math.Ceiling(bars.Max(b=>b.R+b.O)/10.0)*10);
        var sb=new StringBuilder("<div class='am-ed-chart' role='img' aria-label='Hours per week'><div class='am-ed-chart-y'>");
        for(int i=5;i>=0;i--)sb.Append("<span>").Append((max*i/5).ToString("0.#",Inv)).Append(" h</span>");
        sb.Append("</div><div class='am-ed-chart-bars'>");
        foreach(var b in bars)sb.Append("<div class='am-ed-bar' title='").Append(HttpUtility.HtmlAttributeEncode(b.W.ToString("MMM d",Inv)+": "+Hours(b.R)+" regular, "+Hours(b.O)+" overtime")).Append("'><div class='am-ed-bar-stack'><i class='am-l-ot' style='height:").Append((b.O/max*100).ToString("0.##",Inv)).Append("%'></i><i class='am-l-reg' style='height:").Append((b.R/max*100).ToString("0.##",Inv)).Append("%'></i></div><span>").Append(b.W.ToString("MMM d",Inv)).Append("</span></div>");
        TrendChart.Text=sb.Append("</div></div>").ToString();
    }
    protected void ExportTime_Click(object sender,EventArgs e)
    {
        context=card.Assignments.First(a=>ContextAssignment.SelectedValue==a.BusinessId+":"+a.EmployeeId);
        List<WeekView> list;using(var c=AnyminuteTimesheetRepair.Open()){AnyminuteWeekService.ReadAccess(c,null,context.BusinessId,context.EmployeeId);list=LoadWeeks(c,AnyminuteWeekService.Monday(DateTime.Today).AddDays(-7*52));}
        list=list.Where(v=>TimeStatus.SelectedValue==""||v.Status==TimeStatus.SelectedValue).ToList();
        Csv("time-cards.csv",new[]{"Week","Regular Hours","Overtime Hours","Total Hours","Status","Submitted On","Approved By"},list.Select(v=>new[]{v.WeekText,v.Regular.ToString("0.00",Inv),v.Overtime.ToString("0.00",Inv),v.Total.ToString("0.00",Inv),v.Status,v.SubmittedText,v.ApprovedBy}));
    }

    // ---------- slide 5: Documents ----------
    private void BindDocuments()
    {
        UploadButton.Visible=UploadForm.Visible=CanManage;
        if(DocType.Items.Count==0){foreach(var t in AnyminuteEmployeeDocuments.Types)DocType.Items.Add(t);foreach(var a in card.Assignments.GroupBy(x=>x.BusinessId).Select(g=>g.First()))DocBusiness.Items.Add(new ListItem(a.Business,a.BusinessId.ToString(Inv)));
            if(DocBusiness.Items.FindByValue(context.BusinessId.ToString(Inv))!=null)DocBusiness.SelectedValue=context.BusinessId.ToString(Inv);}
        DocTypeOptions.Text=String.Concat(AnyminuteEmployeeDocuments.Types.Select(t=>"<option>"+HttpUtility.HtmlEncode(t)+"</option>"));
        var list=AnyminuteEmployeeDocuments.List(card.EmployeeId,ScopeBusinesses);Docs.DataSource=list;Docs.DataBind();DocsEmpty.Visible=list.Count==0;
    }
    protected void DocUpload_Click(object sender,EventArgs e)
    {
        try
        {
            if(!card.Assignments.All(a=>a.Manage))throw new HttpException(403,"Owner access is required to upload documents.");
            int b;if(!Int32.TryParse(DocBusiness.SelectedValue,out b)||!card.Assignments.Any(a=>a.BusinessId==b))throw new InvalidOperationException("Select a related business.");
            DateTime? expiry=null;DateTime d;if(DocExpiry.Text.Trim().Length>0){if(!DateTime.TryParseExact(DocExpiry.Text.Trim(),"yyyy-MM-dd",Inv,DateTimeStyles.None,out d))throw new InvalidOperationException("Enter a valid expiry date.");expiry=d;}
            AnyminuteEmployeeDocuments.Save(card.EmployeeId,DocFile.PostedFile,DocType.SelectedValue,b,card.Assignments.First(a=>a.BusinessId==b).Business,expiry,S(Session["email"]));
            AnyminuteFlash.Set("details","Document uploaded.");Response.Redirect(Url("documents"),false);Context.ApplicationInstance.CompleteRequest();
        }
        catch(InvalidOperationException error){ShowNotice(error.Message);UploadForm.CssClass="am-ed-upload is-open";}
    }
    protected void DocCommand(object sender,RepeaterCommandEventArgs e)
    {
        if(e.CommandName!="DeleteDoc")return;
        try{if(!card.Assignments.All(a=>a.Manage))throw new HttpException(403,"Owner access is required to delete documents.");
            AnyminuteEmployeeDocuments.Delete(card.EmployeeId,ScopeBusinesses,Convert.ToString(e.CommandArgument));AnyminuteFlash.Set("details","Document deleted.");Response.Redirect(Url("documents"),false);Context.ApplicationInstance.CompleteRequest();}
        catch(InvalidOperationException error){ShowNotice(error.Message);}
    }
    private void SendDocument()
    {
        var d=AnyminuteEmployeeDocuments.Find(card.EmployeeId,ScopeBusinesses,Request.QueryString["doc"]);
        if(d==null)throw new HttpException(404,"Document not found.");
        string path=AnyminuteEmployeeDocuments.PathOf(card.EmployeeId,d);if(!System.IO.File.Exists(path))throw new HttpException(404,"Document file is missing.");
        string type=AnyminuteEmployeeDocuments.ContentType(d.FileName);bool inline=Request.QueryString["dl"]!="1"&&(type=="application/pdf"||type.StartsWith("image/"));
        Response.Clear();Response.ContentType=type;Response.AddHeader("X-Content-Type-Options","nosniff");
        Response.AddHeader("Content-Disposition",(inline?"inline":"attachment")+"; filename=\""+d.FileName.Replace("\"","")+"\"; filename*=UTF-8''"+Uri.EscapeDataString(d.FileName));
        Response.TransmitFile(path);Response.Flush();Response.SuppressContent=true;Context.ApplicationInstance.CompleteRequest();
    }

    // ---------- slide 6: Address ----------
    private void BindAddress(System.Data.SqlClient.SqlConnection c)
    {
        AddressBody.Visible=manage;AddressScope.Visible=!manage;AddressHistoryLink.Visible=EditAddress.Visible=manage;
        if(!manage){AddressScope.Text="Address is restricted to owners who can manage the employee's shared record.";return;}
        EditAddress.Text=I("pencil")+"Edit Address";EditAddress.NavigateUrl="EmployeeEditor.aspx?EmployeeId="+employee+"&tab=address";
        bool history=Request.QueryString["view"]=="history";AddressHistoryLink.Text=I("history")+(history?"Hide Address History":"View Address History");AddressHistoryLink.NavigateUrl=Url("address")+(history?"":"&view=history");
        var p=AnyminuteEmployeeEditor.LoadProfile(context.EmployeeId);
        string line=String.Join(" ",new[]{p.StreetNo,p.Line1}.Where(x=>!String.IsNullOrWhiteSpace(x)));if(!String.IsNullOrWhiteSpace(p.Line2))line+=(line.Length>0?", ":"")+p.Line2;
        if(!String.IsNullOrWhiteSpace(p.SuiteNumber))line+=(line.Length>0?", ":"")+(String.IsNullOrWhiteSpace(p.SuiteType)?"Unit":p.SuiteType)+" "+p.SuiteNumber;
        Func<string,string> v=x=>HttpUtility.HtmlEncode(String.IsNullOrWhiteSpace(x)?"\u2014":x);
        ResLine.Text=MailLine.Text=v(line);ResCity.Text=MailCity.Text=v(p.City);ResProvince.Text=MailProvince.Text=v(p.Province);ResPostal.Text=MailPostal.Text=v(p.PostalCode);ResCountry.Text=MailCountry.Text=v(p.Country.Length>0?p.Country:"Canada");
        var ids=card.Assignments.Select(a=>a.EmployeeId).Distinct().ToArray();
        var edits=AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP 50 a.RecordedAtUTC,a.AfterValue,dbo.FN_DESC('AddEmployee',a.ActorID) ChangedBy FROM AnyminuteApprovalAudit a WHERE a.EmployeeID IN("+String.Join(",",ids.Select(i=>i.ToString(Inv)))+") AND a.Action='EditEmployeeProfile' ORDER BY a.RecordedAtUTC DESC");
        AddressUpdated.Text=edits.Rows.Count>0?HttpUtility.HtmlEncode(Local(edits.Rows[0]["RecordedAtUTC"],"MMM d, yyyy 'at' h:mm tt")):"\u2014";
        string compact=AnyminuteBusinessAddressInput.Postal(p.PostalCode??""),spaced=compact.Length==6?compact.Substring(0,3)+" "+compact.Substring(3):compact;
        bool verified=compact.Length>0&&AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP 1 1 FROM AddressMaster WHERE PostalCode IN(@p,@s) AND ISNULL(Verifed,0)=1","@p",compact,"@s",spaced).Rows.Count>0;
        AddressVerified.Text=verified?I("check")+"Verified":"Not Verified";AddressVerified.CssClass=Pill(verified?"Verified":"Not Verified");
        AddressHistoryPanel.Visible=history;
        if(history){edits.Columns.Add("When");foreach(DataRow r in edits.Rows)r["When"]=Local(r["RecordedAtUTC"],"MMM d, yyyy h:mm tt");AddressHistory.DataSource=edits;AddressHistory.DataBind();AddressHistoryEmpty.Visible=edits.Rows.Count==0;}
    }

    // ---------- slide 7: Projects ----------
    private void BindProjects(System.Data.SqlClient.SqlConnection c)
    {
        bool detail=AnyminuteEmployeeEditor.HasTable(c,null,"AnyminuteProjectDetail");
        var t=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT p.ProjectID,p.ProjectName,p.startDate,p.EndDate,"+(detail?"d.ProjectStatus":"CAST(NULL AS nvarchar(20)) ProjectStatus")+
            ",(SELECT ISNULL(SUM(CAST(DATEDIFF(second,CAST('00:00' AS time),TRY_CONVERT(time,LTRIM(RTRIM(x.Total)))) AS bigint)),0) FROM TimeSheetEntry x WHERE x.ProjectId=p.ProjectID AND x.EmpId=@e AND x.BussID=@b) Seconds"+
            " FROM ASSIGNPROJECT a JOIN AddProject p ON p.ProjectID=a.PROJECTID AND p.BusinessID=a.BUSINESSID"+(detail?" LEFT JOIN AnyminuteProjectDetail d ON d.ProjectID=p.ProjectID":"")+" WHERE a.BUSINESSID=@b AND a.EMPLOYEEID=@e AND ISNULL(p.Status,'')<>'D' ORDER BY p.ProjectName","@b",context.BusinessId,"@e",context.EmployeeId);
        var rows=t.Rows.Cast<DataRow>().Select(r=>{DateTime? s=r["startDate"]==DBNull.Value?(DateTime?)null:Convert.ToDateTime(r["startDate"]);DateTime? en=AnyminuteProjectService.ParseDate(S(r["EndDate"]));string st=S(r["ProjectStatus"]);double h=Convert.ToInt64(r["Seconds"])/3600.0;
            return new{ProjectId=Convert.ToInt32(r["ProjectID"]),Name=S(r["ProjectName"]),Business=context.Business,Role=String.IsNullOrWhiteSpace(context.Role)?"\u2014":context.Role,
                StartText=s.HasValue?s.Value.ToString("MMM dd, yyyy",Inv):"\u2014",StartSort=s.HasValue?s.Value.ToString("yyyyMMdd",Inv):"",EndText=en.HasValue?en.Value.ToString("MMM dd, yyyy",Inv):"\u2014",EndSort=en.HasValue?en.Value.ToString("yyyyMMdd",Inv):"",
                Hours=h.ToString("0.0",Inv),HoursText=h.ToString("0.0",Inv),Status=st=="Inactive"?"On Hold":st.Length==0?"Active":st};}).ToList();
        Projects.DataSource=rows;Projects.DataBind();ProjectsEmpty.Visible=rows.Count==0;
        PrjAssigned.Text=rows.Count.ToString(Inv);PrjHours.Text=rows.Sum(r=>Double.Parse(r.Hours,Inv)).ToString("0.0",Inv);
        PrjActive.Text=rows.Count(r=>r.Status=="Active").ToString(Inv);PrjHold.Text=rows.Count(r=>r.Status=="On Hold").ToString(Inv);PrjDone.Text=rows.Count(r=>r.Status=="Completed").ToString(Inv);
    }

    // ---------- slide 8: Login & Access ----------
    private void BindAccess(System.Data.SqlClient.SqlConnection c)
    {
        AccessBody.Visible=manage;AccessMessage.Visible=!manage;
        if(!manage){AccessMessage.Text="Account details are restricted to authorized owners.";return;}
        var s=AnyminuteEmployeeEditor.Access(card);
        AccAccount.Text=HttpUtility.HtmlEncode(card.Status);AccAccount.CssClass=Pill(card.Status);
        Action<Label,string> pill=(l,v)=>{l.Text="<i></i>"+HttpUtility.HtmlEncode(v);l.CssClass=Pill(v)+" am-ed-dot";};
        pill(AccLogin,s.UserId==0?"No login":s.Enabled?"Enabled":"Disabled");
        bool notSent=s.UserId==0||(!s.Enabled&&s.SentText=="\u2014"); // v86: employees are added without access; nothing is sent until Send Invitation
        AccInvite.Text=notSent?"Not sent":s.Pending?"Pending":I("check")+"Accepted";AccInvite.CssClass=Pill(notSent?"":s.Pending?"Pending":"Accepted");
        pill(AccMfa,s.TwoFactor?"Enabled":"Disabled");
        AccLastLogin.Text=HttpUtility.HtmlEncode(s.LastLogin=="Never"?"Never":FormatLogin(s.LastLogin));
        ResendInvite.Text=I("send")+(s.Enabled?"Resend Invitation":"Send Invitation");ResendInvite.ToolTip=s.Enabled?"Send the invitation email again.":"Grant login access and send the invitation email.";SendReset.Text=I("shield")+"Send Password Reset";ResendInvite.Visible=SendReset.Visible=s.UserId>0;
        var ids=card.Assignments.Select(a=>a.EmployeeId).Distinct().ToArray();
        var reset=AnyminuteTimesheetRepair.Query(c,null,"SELECT MAX(RecordedAtUTC) FROM AnyminuteApprovalAudit WHERE EmployeeID IN("+String.Join(",",ids.Select(i=>i.ToString(Inv)))+") AND Action IN('ResetPassword','RequirePasswordChange')");
        AccLastReset.Text=HttpUtility.HtmlEncode(reset.Rows[0][0]==DBNull.Value?"\u2014":Local(reset.Rows[0][0],"MMM d, yyyy 'at' h:mm tt"));
        bool sessions=s.UserId>0&&AnyminuteEmployeeEditor.HasTable(c,null,"AnyminuteActiveSession");int active=0;
        if(sessions)active=Convert.ToInt32(AnyminuteTimesheetRepair.Query(c,null,"SELECT COUNT(*) FROM AnyminuteActiveSession WHERE UserId=@u AND EndedAtUTC IS NULL AND LastSeenAtUTC>DATEADD(minute,-@t,SYSUTCDATETIME())","@u",s.UserId,"@t",Session.Timeout).Rows[0][0]);
        AccSessions.Text=active+(active==1?" Active Session":" Active Sessions");
        bool history=Request.QueryString["view"]=="history";LoginHistoryLink.Text=I("history")+(history?"Hide Login History":"View Login History");LoginHistoryLink.NavigateUrl=Url("access")+(history?"":"&view=history");
        LoginHistoryPanel.Visible=history;
        if(history)
        {
            var rows=new List<object>();
            if(sessions)foreach(DataRow r in AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP 20 StartedAtUTC,LastSeenAtUTC,Device,EndedAtUTC,EndReason FROM AnyminuteActiveSession WHERE UserId=@u ORDER BY StartedAtUTC DESC","@u",s.UserId).Rows)
                rows.Add(new{Started=Local(r["StartedAtUTC"],"MMM d, yyyy h:mm tt"),Seen=Local(r["LastSeenAtUTC"],"MMM d, yyyy h:mm tt"),Device=S(r["Device"]).Length>0?S(r["Device"]):"\u2014",Ended=r["EndedAtUTC"]==DBNull.Value?"Active":Local(r["EndedAtUTC"],"MMM d, yyyy h:mm tt")+(S(r["EndReason"]).Length>0?" ("+S(r["EndReason"])+")":"")});
            LoginHistory.DataSource=rows;LoginHistory.DataBind();LoginHistoryEmpty.Visible=rows.Count==0;
        }
    }
    static string FormatLogin(string value){DateTime d;return DateTime.TryParseExact(value,"yyyy-MM-dd hh:mm tt",Inv,DateTimeStyles.None,out d)?d.ToString("MMM d, yyyy 'at' h:mm tt",Inv):value;}
    protected void ResendInvite_Click(object sender,EventArgs e){AccessAction(()=>AnyminuteEmployeeEditor.GrantAccessAndInvite(employee,true),null);}
    protected void SendReset_Click(object sender,EventArgs e){AccessAction(()=>AnyminuteEmployeeEditor.SendInvitation(employee,false),"ResetPassword");}
    private void AccessAction(Func<string> action,string audit)
    {
        try
        {
            string m=action();
            if(audit!=null&&m.StartsWith("Invitation email sent",StringComparison.Ordinal))
            {
                m="Password reset email sent to "+card.Email+".";var a=card.First;
                using(var c=AnyminuteTimesheetRepair.Open())AnyminuteTimesheetRepair.Execute(c,null,"INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,@x,NULL,@av,SYSUTCDATETIME())","@b",a.BusinessId,"@a",Convert.ToInt32(Session["EmployeeId"]),"@e",a.EmployeeId,"@x",audit,"@av",card.Email);
            }
            AnyminuteFlash.Set("details",m);Response.Redirect(Url("access"),false);Context.ApplicationInstance.CompleteRequest();
        }
        catch(InvalidOperationException error){ShowNotice(error.Message);}
    }

    // ---------- slide 9: Activity History ----------
    static readonly Dictionary<string,string[]> Events=new Dictionary<string,string[]>{
        {"EditEmployeeProfile",new[]{"Profile Update","user","Employee profile updated."}},{"SendInvitation",new[]{"Invitation Sent","mail","Invitation email sent to employee."}},
        {"CreateEmployee",new[]{"Employee Created","user","Employee record created."}},{"ResetPassword",new[]{"Password Reset","lock","Password reset email sent."}},
        {"RequirePasswordChange",new[]{"Password Reset","lock","Password change required at next login."}},{"LoginEnabled",new[]{"Login Enabled","key","Login access enabled."}},
        {"LoginDisabled",new[]{"Login Disabled","key","Login access disabled."}},{"CancelInvitation",new[]{"Invitation Cancelled","mail","Invitation cancelled."}},
        {"TwoFactorChanged",new[]{"MFA Changed","smartphone","Two-factor setting changed."}},{"CopyInvitationLink",new[]{"Invitation Link Copied","mail","Invitation link copied."}},
        {"ChangeRate",new[]{"Pay Rate Change","dollar","Pay rate changed."}},{"UpdateRate",new[]{"Pay Rate Change","dollar","Current pay rate updated."}},
        {"SaveAssignment",new[]{"Business Assignment Edited","layers","Modified business assignment."}},{"AddAssignment",new[]{"Business Assignment Added","layers","Business assignment added."}},
        {"EndAssignment",new[]{"Business Assignment Ended","layers","Business assignment ended."}},{"RemoveAssignment",new[]{"Business Assignment Removed","layers","Business assignment removed."}},
        {"Submit",new[]{"Time Card Submitted","clock","Weekly time card submitted."}},{"Approve",new[]{"Time Card Approved","clock","Weekly time card approved."}},
        {"Reject",new[]{"Time Card Rejected","clock","Weekly time card rejected."}},{"SaveDraft",new[]{"Time Card Saved","clock","Weekly time card saved as draft."}}};
    static string[] EventOf(string action)
    {
        string[] e;if(Events.TryGetValue(action,out e))return e;
        string words=System.Text.RegularExpressions.Regex.Replace(action,"(?<=[a-z0-9])(?=[A-Z])"," ").Trim();
        string icon=action.StartsWith("Project",StringComparison.Ordinal)?"layers":action.IndexOf("Rate",StringComparison.Ordinal)>=0?"dollar":action.IndexOf("Day",StringComparison.Ordinal)>=0||action.IndexOf("Week",StringComparison.Ordinal)>=0?"clock":"activity";
        return new[]{words.Length>0?words:"Event",icon,words.Length>0?words+".":"\u2014"};
    }
    private void BindActivity(System.Data.SqlClient.SqlConnection c)
    {
        var records=card.Assignments.Select(a=>a.BusinessId+":"+a.EmployeeId).Distinct().ToArray();
        var ids=card.Assignments.Select(a=>a.EmployeeId).Distinct().ToArray();var bizIds=card.Assignments.Select(a=>a.BusinessId).Distinct().ToArray();
        var t=AnyminuteTimesheetRepair.Query(c,null,"SELECT TOP 1000 a.BusinessID,a.EmployeeID,a.Action,a.BeforeValue,a.AfterValue,a.TimeSheetDate,a.RecordedAtUTC,dbo.FN_DESC('AddEmployee',a.ActorID) ChangedBy,b.CompanyName FROM AnyminuteApprovalAudit a LEFT JOIN AddBusiness b ON b.BusinessID=a.BusinessID WHERE a.EmployeeID IN("+String.Join(",",ids.Select(i=>i.ToString(Inv)))+") AND a.BusinessID IN("+String.Join(",",bizIds.Select(i=>i.ToString(Inv)))+") ORDER BY a.RecordedAtUTC DESC");
        var all=t.Rows.Cast<DataRow>().Where(r=>records.Contains(S(r["BusinessID"])+":"+S(r["EmployeeID"]))).Select(r=>{var ev=EventOf(S(r["Action"]));var when=DateTime.SpecifyKind(Convert.ToDateTime(r["RecordedAtUTC"]),DateTimeKind.Utc).ToLocalTime();
            string desc=ev[2];if(r["TimeSheetDate"]!=DBNull.Value)desc+=" ("+Convert.ToDateTime(r["TimeSheetDate"]).ToString("MMM d, yyyy",Inv)+")";
            return new{Local=when,When=when.ToString("MMM d, yyyy, h:mm tt",Inv),Sort=when.ToString("yyyyMMddHHmmss",Inv),Event=ev[0],Icon=ev[1],Description=desc,Before=S(r["BeforeValue"]).Length>0?S(r["BeforeValue"]):"\u2014",After=S(r["AfterValue"]).Length>0?S(r["AfterValue"]):"\u2014",By=S(r["ChangedBy"]).Length>0?S(r["ChangedBy"]):"System",Business=S(r["CompanyName"])};}).ToList();
        string range=ActRange.SelectedValue,type=ActType.SelectedValue,user=ActUser.SelectedValue;
        if(!IsPostBack){range=Request.QueryString["range"]??"";type="";user="";}
        ActRange.Items.Clear();ActRange.Items.Add(new ListItem("All dates",""));
        var month=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1);
        for(int i=0;i<12;i++){var m=month.AddMonths(-i);ActRange.Items.Add(new ListItem(m.ToString("MMM d, yyyy",Inv)+" \u2013 "+m.AddMonths(1).AddDays(-1).ToString("MMM d, yyyy",Inv),m.ToString("yyyy-MM",Inv)));}
        ActType.Items.Clear();ActType.Items.Add(new ListItem("All Event Types",""));foreach(var x in all.Select(r=>r.Event).Distinct().OrderBy(x=>x))ActType.Items.Add(x);
        ActUser.Items.Clear();ActUser.Items.Add(new ListItem("All Users",""));foreach(var x in all.Select(r=>r.By).Distinct().OrderBy(x=>x))ActUser.Items.Add(x);
        if(ActRange.Items.FindByValue(range??"")!=null)ActRange.SelectedValue=range;if(ActType.Items.FindByValue(type??"")!=null)ActType.SelectedValue=type;if(ActUser.Items.FindByValue(user??"")!=null)ActUser.SelectedValue=user;
        var rows=all.Where(r=>(ActRange.SelectedValue==""||r.Local.ToString("yyyy-MM",Inv)==ActRange.SelectedValue)&&(ActType.SelectedValue==""||r.Event==ActType.SelectedValue)&&(ActUser.SelectedValue==""||r.By==ActUser.SelectedValue)).Take(200).ToList();
        Activity.DataSource=rows;Activity.DataBind();ActivityEmpty.Visible=rows.Count==0;
        ActReset.Text=I("refresh")+"Reset Filters";ActReset.NavigateUrl=Url("activity");
    }
}
