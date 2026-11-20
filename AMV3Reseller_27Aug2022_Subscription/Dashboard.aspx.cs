using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteDashboard : System.Web.UI.Page
{
 protected void Page_Load(object s,EventArgs e)
 {
  if(!IsPostBack)
  {
   // Confirmation set by Add_Business after a successful save (shown once).
   string flash=AnyminuteFlash.Take("dashboard");
   if(flash!=null){SavedFlashText.Text=HttpUtility.HtmlEncode(flash);SavedFlash.Visible=true;}
   // v18 (deck slides 1-3): employees / supervisors get the role dashboard; owners, admins, accountants and
   // "My Businesses" (?view=businesses) keep the existing businesses dashboard below.
   if(Request.QueryString["view"]!="businesses" && AnyminuteRoleDashboard.Applies())
   {
    var model=AnyminuteRoleDashboard.Load(Request.QueryString["b"],Request.QueryString["mode"],Request.QueryString["w"]);
    RoleDashboard.Text=AnyminuteRoleDashboard.Render(model,null);RoleDashboard.Visible=true;OwnerView.Visible=false;return;
   }
   // v33 (Shamsu deck "Accountant & Payroll Agency Dashboard"): accountants / payroll agencies get the deck dashboard;
   // "Assigned Businesses" (?view=businesses) keeps the businesses list below.
   // v37 (PPT s1 Access History / View All Activity): full activity list for accountants.
   if(Request.QueryString["view"]=="activity" && AnyminuteAccountantDashboard.Applies())
   {RoleDashboard.Text=AnyminuteAccountantDashboard.RenderActivity();RoleDashboard.Visible=true;OwnerView.Visible=false;return;}
   // v38 (PPT "Open vs View"): View = read-only business details screen.
  if(Request.QueryString["view"]=="business" && AnyminuteAccountantDashboard.Applies())
  {RoleDashboard.Text=AnyminuteAccountantDashboard.RenderBusinessView(Request.QueryString["b"]);RoleDashboard.Visible=true;OwnerView.Visible=false;return;}
  if(Request.QueryString["view"]!="businesses" && AnyminuteAccountantDashboard.Applies())
   {RoleDashboard.Text=AnyminuteAccountantDashboard.Render(Request.QueryString["b"]);RoleDashboard.Visible=true;OwnerView.Visible=false;return;}
   // v86: subscribers (Business Administrator) get the redesigned Dashboard; "Businesses" (?view=businesses) keeps the list below.
   if(Request.QueryString["view"]!="businesses" && AnyminuteSubscriberDashboard.Applies())
   {RoleDashboard.Text=AnyminuteSubscriberDashboard.Render(Request.QueryString["b"],Request.QueryString["w"],Request.QueryString["view"]=="activity");RoleDashboard.Visible=true;OwnerView.Visible=false;return;}
   Bind();
  }
 }
 protected void FilterChanged(object s,EventArgs e) {Bind();}
 void Bind()
 {
  var rows=AnyminuteAccountantDashboard.Applies()?AnyminuteAccountantDashboard.Businesses():AnyminuteModernScope.Businesses();rows.Columns.Add("Manage",typeof(bool));rows.Columns.Add("Users",typeof(int));rows.Columns.Add("DisplayStatus");
  int projects=0;bool owner=false;var people=new System.Collections.Generic.HashSet<int>();foreach(DataRow row in rows.Rows)
  {int b=Convert.ToInt32(row["BusinessID"]);row["Manage"]=AnyminuteModernScope.Owns(b);owner|=Convert.ToBoolean(row["Manage"]);
   row["Users"]=0;
   try{var users=AnyminuteModernScope.Users(b);row["Users"]=users.Rows.Count;foreach(DataRow u in users.Rows)people.Add(Convert.ToInt32(u["EmployeeID"]));projects+=AnyminuteModernScope.Projects(b).Rows.Count;}
   catch(InvalidOperationException){} // v27: one ambiguous membership (e.g. accountant) no longer breaks the whole dashboard
   row["DisplayStatus"]=Convert.ToString(row["Status"])=="1"?"Active":"Not Active";}
  // v27 (live: Total Employees 77 vs Users 43): the card summed per-business assignments, so a person in several businesses
  // counted several times. It now counts people the same way the Users directory does (one card per person), else distinct IDs.
  int employees=people.Count;
  if(AnyminuteDirectoryScope.CanReadDirectory())try{employees=AnyminuteEmployeesView.Load().Count;}catch(HttpException){}catch(InvalidOperationException){}
  BusinessCount.Text=rows.Rows.Count.ToString();EmployeeCount.Text=employees.ToString();ProjectCount.Text=projects.ToString();ApprovalCount.Text=AnyminuteModernScope.Pending().ToString();
  AddUser.Visible=owner;AddBusiness.Visible=Convert.ToString(Session["UserTypeid"])=="1";
  string report=AnyminuteReports.HubHref(AnyminuteUiNavigation.Load());ReportsQuick.Visible=report!=null;if(report!=null)ReportsQuick.NavigateUrl=report;
  ViewApprovals.Visible=AnyminuteDirectoryScope.CanReadDirectory();
  // v16: role-based quick actions - every user with a business gets Work Schedule; employees also get their weekly time card.
  WorkScheduleQuick.Visible=rows.Rows.Count>0;TimeCardQuick.Visible=rows.Rows.Count>0&&!owner&&!ViewApprovals.Visible;
  // v27: role-aware copy - accountants and employee-only users no longer see manager wording / a Pending Approvals card.
  string typeId=Convert.ToString(Session["UserTypeid"]).Trim();bool accountant=typeId=="12"||(typeId=="0"&&Convert.ToString(Session["UserType"])=="Admin");
  if(accountant)
  {
   TimeCardQuick.Visible=false;BusinessesIntro.Text="Businesses you have accountant access to. Use Reports for timesheet and payroll exports; Work Schedule is read-only.";
   ApprovalTitle.Text="Reports";ApprovalCount.Text=report!=null?"Open":"\u2014";ApprovalHint.Text=report!=null?"Timesheet and payroll reports":"No reports granted yet";ApprovalCard.HRef=report??"Dashboard.aspx";
  }
  else if(!owner&&!ViewApprovals.Visible)
  {
   BusinessesIntro.Text="Businesses you work for. Use Quick Actions to open your time card or work schedule.";
   ApprovalTitle.Text="My Time Card";ApprovalCount.Text="Open";ApprovalHint.Text="Enter and submit your hours";ApprovalCard.HRef="WeekTimeCard.aspx";
  }
  if(rows.Rows.Count==0)Empty.Text=accountant?"No businesses have shared accountant access with you yet. Ask the business owner to invite you from Accountant & Payroll Access.":"No accessible businesses found.";
  BusinessRows.DataSource=StatusFilter.SelectedValue=="All"?rows:rows.Select("DisplayStatus='"+(StatusFilter.SelectedValue=="Inactive"?"Not Active":"Active")+"'").CopyToDataTableOrEmpty(rows);
  BusinessRows.DataBind();Empty.Visible=BusinessRows.Items.Count==0;
 }
 protected void BusinessRows_ItemCommand(object sender,RepeaterCommandEventArgs e)
 {
  StatusMessage.Visible=false;
  if(e.CommandName=="DeleteBusiness")
  {
   int delId;
   if(!Int32.TryParse(Convert.ToString(e.CommandArgument),out delId)||delId<=0)throw new HttpException(400,"Invalid business.");
   try
   {
    using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction())
    {
     int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,delId);
     var current=AnyminuteTimesheetRepair.Query(c,tx,"SELECT Status FROM AddBusiness WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b","@b",delId);
     if(current.Rows.Count!=1)throw new InvalidOperationException("Business was not found. Refresh the dashboard before trying again.");
     int before=Convert.ToInt32(current.Rows[0][0]);
     if(before==2)
     {tx.Commit();Bind();StatusMessage.Text="Business already removed from the list.";StatusMessage.Visible=true;return;}
     // Soft delete: Status=2 is already hidden by Businesses list queries (Status<>2). Rows and related data stay.
     AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AddBusiness SET Status=2 WHERE BusinessID=@b AND Status<>2; INSERT INTO AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@a,'DeleteBusiness',@before,'2',SYSUTCDATETIME())",
      "@b",delId,"@a",actor,"@before",before.ToString());
     tx.Commit();
    }
    Bind();StatusMessage.Text="Business removed from the list. Existing records were kept.";StatusMessage.Visible=true;
   }
   catch(InvalidOperationException ex){StatusMessage.Text=HttpUtility.HtmlEncode(ex.Message);StatusMessage.Visible=true;}
   return;
  }
  if(e.CommandName!="SetBusinessStatus")return;
  string[] values=Convert.ToString(e.CommandArgument).Split('|');int business,expected,next;
  if(values.Length!=3 || !Int32.TryParse(values[0],out business) || !Int32.TryParse(values[1],out expected) || !Int32.TryParse(values[2],out next) ||
     business<=0 || (expected!=0&&expected!=1) || (next!=0&&next!=1))throw new HttpException(400,"Invalid business status action.");
  try
  {
   using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction())
   {
    int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,business);
    var current=AnyminuteTimesheetRepair.Query(c,tx,"SELECT Status FROM AddBusiness WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b","@b",business);
    if(current.Rows.Count!=1 || Convert.ToInt32(current.Rows[0][0])!=expected)
     throw new InvalidOperationException("Business status changed. Refresh the dashboard before trying again.");
    if(expected!=next)
    {
     AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AddBusiness SET Status=@next WHERE BusinessID=@b AND Status=@expected; INSERT INTO AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@a,'BusinessStatus',@before,@after,SYSUTCDATETIME())",
      "@b",business,"@a",actor,"@next",next,"@expected",expected,"@before",expected.ToString(),"@after",next.ToString());
    }
    tx.Commit();
   }
   Bind();StatusMessage.Text="Business status saved as "+(next==1?"Active":"Not Active")+".";StatusMessage.Visible=true;
  }
  catch(InvalidOperationException ex){StatusMessage.Text=HttpUtility.HtmlEncode(ex.Message);StatusMessage.Visible=true;}
 }

}
