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
   Bind();
  }
 }
 protected void FilterChanged(object s,EventArgs e) {Bind();}
 void Bind()
 {
  var rows=AnyminuteModernScope.Businesses();rows.Columns.Add("Manage",typeof(bool));rows.Columns.Add("Users",typeof(int));rows.Columns.Add("DisplayStatus");
  int employees=0,projects=0;bool owner=false;foreach(DataRow row in rows.Rows)
  {int b=Convert.ToInt32(row["BusinessID"]);row["Manage"]=AnyminuteModernScope.Owns(b);owner|=Convert.ToBoolean(row["Manage"]);
   var users=AnyminuteModernScope.Users(b);row["Users"]=users.Rows.Count;employees+=users.Rows.Count;projects+=AnyminuteModernScope.Projects(b).Rows.Count;
   row["DisplayStatus"]=Convert.ToString(row["Status"])=="1"?"Active":"Not Active";}
  BusinessCount.Text=rows.Rows.Count.ToString();EmployeeCount.Text=employees.ToString();ProjectCount.Text=projects.ToString();ApprovalCount.Text=AnyminuteModernScope.Pending().ToString();
  AddUser.Visible=owner;AddBusiness.Visible=Convert.ToString(Session["UserTypeid"])=="1";
  string report=AnyminuteUiNavigation.ReportHref(AnyminuteUiNavigation.Load());ReportsQuick.Visible=report!=null;if(report!=null)ReportsQuick.NavigateUrl=report;
  ViewApprovals.Visible=AnyminuteDirectoryScope.CanReadDirectory();
  // v16: role-based quick actions - every user with a business gets Work Schedule; employees also get their weekly time card.
  WorkScheduleQuick.Visible=rows.Rows.Count>0;TimeCardQuick.Visible=rows.Rows.Count>0&&!owner&&!ViewApprovals.Visible;
  BusinessRows.DataSource=StatusFilter.SelectedValue=="All"?rows:rows.Select("DisplayStatus='"+(StatusFilter.SelectedValue=="Inactive"?"Not Active":"Active")+"'").CopyToDataTableOrEmpty(rows);
  BusinessRows.DataBind();Empty.Visible=BusinessRows.Items.Count==0;
 }
 protected void BusinessRows_ItemCommand(object sender,RepeaterCommandEventArgs e)
 {
  if(e.CommandName!="SetBusinessStatus")return;
  StatusMessage.Visible=false;
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
