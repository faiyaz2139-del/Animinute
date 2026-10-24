using System;
using System.Data;
using System.Web.UI.WebControls;
public partial class AnyminuteBusinessSettings : System.Web.UI.Page
{
 protected void Page_Load(object s,EventArgs e)
 {
  if(IsPostBack)return;foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)if(AnyminuteModernScope.Owns(Convert.ToInt32(b["BusinessID"])))Business.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]),Convert.ToString(b["BusinessID"])));
  foreach(var zone in TimeZoneInfo.GetSystemTimeZones())Zone.Items.Add(new ListItem(zone.DisplayName,zone.Id));
  Save.Visible=Business.Items.Count>0;if(Save.Visible)LoadPolicy();else Message.Text="Only a subscriber or business owner can change these settings.";
 }
 protected void Changed(object s,EventArgs e){LoadPolicy();}
 void LoadPolicy()
 {using(var c=AnyminuteTimesheetRepair.Open()){int b=Int32.Parse(Business.SelectedValue);AnyminuteApprovalAssignments.RequireOwner(c,null,b);var p=AnyminuteTimesheetRepair.Query(c,null,"SELECT * FROM AnyminuteBusinessPolicy WHERE BusinessID=@b","@b",b);
  string zone=p.Rows.Count==0?"Eastern Standard Time":Convert.ToString(p.Rows[0]["TimeZoneID"]);if(Zone.Items.FindByValue(zone)!=null)Zone.SelectedValue=zone;Estimates.Checked=p.Rows.Count==0||Convert.ToBoolean(p.Rows[0]["AllowClockOutEstimates"]);}}
 protected void Save_Click(object s,EventArgs e)
 {int b=Int32.Parse(Business.SelectedValue);TimeZoneInfo.FindSystemTimeZoneById(Zone.SelectedValue);using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(System.Data.IsolationLevel.Serializable))
  {int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,b);AnyminuteTimesheetRepair.Execute(c,tx,@"UPDATE AnyminuteBusinessPolicy WITH(UPDLOCK,HOLDLOCK) SET TimeZoneID=@z,AllowClockOutEstimates=@allow WHERE BusinessID=@b;
   IF @@ROWCOUNT=0 INSERT AnyminuteBusinessPolicy(BusinessID,TimeZoneID,AllowClockOutEstimates) VALUES(@b,@z,@allow);
   INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,AfterValue,RecordedAtUTC) VALUES(@b,@a,@a,'BusinessPolicy',@z,SYSUTCDATETIME())","@b",b,"@a",actor,"@z",Zone.SelectedValue,"@allow",Estimates.Checked);tx.Commit();}Message.Text="Settings saved.";}
}
