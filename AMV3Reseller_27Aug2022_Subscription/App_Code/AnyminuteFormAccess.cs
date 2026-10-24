using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public static class AnyminuteFormAccess
{
 public static void OwnerSomewhere()
 {AnyminuteModernScope.Login();foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)if(AnyminuteModernScope.Owns(Convert.ToInt32(b["BusinessID"])))return;throw new HttpException(403,"Business owner access is required.");}
 public static void Subscriber()
 {AnyminuteModernScope.Login();using(var c=AnyminuteTimesheetRepair.Open())if(AnyminuteTimesheetRepair.Query(c,null,"SELECT 1 FROM AddEmployee WHERE EmployeeID=@e AND UserType=1","@e",Convert.ToInt32(HttpContext.Current.Session["EmployeeId"])).Rows.Count!=1)throw new HttpException(403,"Only a subscriber can create a business.");}
 public static int TargetAccount(int business,int employee)
 {
  using(var c=AnyminuteTimesheetRepair.Open())
  {
   AnyminuteApprovalAssignments.RequireOwner(c,null,business);
   var accounts=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT USERID FROM MappingTable WHERE BusinessID=@b AND EmployeeID=@e","@b",business,"@e",employee);
   if(accounts.Rows.Count==0)return 0;if(accounts.Rows.Count!=1)throw new HttpException(403,"Employee account is ambiguous.");int user=Convert.ToInt32(accounts.Rows[0][0]);
   foreach(DataRow row in AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT BusinessID FROM MappingTable WHERE USERID=@u AND BusinessID IS NOT NULL","@u",user).Rows)AnyminuteApprovalAssignments.RequireOwner(c,null,Convert.ToInt32(row[0]));
   return user;
  }
 }
 public static void OwnedSelections(Control parent)
 {
  foreach(Control control in parent.Controls)
  {
   var hidden=control as HiddenField;var drop=control as DropDownList;string value=null;
   if(hidden!=null && (hidden.ID=="hfd_BussinessId"||hidden.ID=="hfdBusinessID"||hidden.ID=="hdbusid"||hidden.ID=="hfdBusinessId"))value=hidden.Value;
   if(drop!=null && (drop.ID=="ddlBusiness"||drop.ID=="ddl_Business"||drop.ID=="ddlBusAssProject"||drop.ID=="ddlBussRateHistory"||drop.ID=="ddl_Business_Name"))value=drop.SelectedValue;
   int business;if(value!=null&&Int32.TryParse(value,out business)&&business>0)
    using(var c=AnyminuteTimesheetRepair.Open())AnyminuteApprovalAssignments.RequireOwner(c,null,business);
   OwnedSelections(control);
  }
 }
}
