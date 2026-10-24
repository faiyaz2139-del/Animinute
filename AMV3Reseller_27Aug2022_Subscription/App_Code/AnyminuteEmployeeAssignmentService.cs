using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
public static class AnyminuteEmployeeAssignmentService
{
    public static void Deactivate(int employee)
    {
        var card=AnyminuteEmployeesView.RequireEmployee(employee);
        if(!card.Manage)throw new HttpException(403,"Subscriber ownership is required.");
        var records=card.Assignments.Where(a=>a.Status=="Active").Select(a=>a.EmployeeId).Distinct().OrderBy(x=>x).ToArray();
        if(records.Length==0)return;
        int actor=Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]),user=Convert.ToInt32(HttpContext.Current.Session["UserId"]);
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            // The linked login can expose other employee records; every such business must be owned.
            foreach(var assignment in card.Assignments)
                AnyminuteFormAccess.TargetAccount(assignment.BusinessId,assignment.EmployeeId);
            foreach(int record in records)
            {
                if(record==actor || AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WHERE EmployeeID=@e AND USERID=@u","@e",record,"@u",user).Rows.Count>0)
                    throw new InvalidOperationException("You cannot deactivate yourself here.");
                AnyminuteTimesheetRepair.Query(c,tx,"SELECT EmployeeID FROM AddEmployee WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e","@e",record);
                var memberships=AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT BusinessID,UserTypeId FROM AssginBussiness WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e","@e",record);
                if(memberships.Rows.Count==0)throw new InvalidOperationException("Employee assignments changed. Refresh and retry.");
                foreach(DataRow membership in memberships.Rows)
                {
                    if(Convert.ToInt32(membership["UserTypeId"])==1)throw new InvalidOperationException("Subscriber ownership cannot be deactivated here.");
                    int business=Convert.ToInt32(membership["BusinessID"]);
                    AnyminuteApprovalAssignments.RequireOwner(c,tx,business);
                    AnyminuteTimesheetRepair.Execute(c,tx,"INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,'DeactivateEmployee','Not Active',SYSUTCDATETIME())","@b",business,"@a",actor,"@e",record);
                }
                AnyminuteTimesheetRepair.Execute(c,tx,@"INSERT Emp_Active_Inactive_Status(EmpId,mp_Stauts,Effective_Date,Datetime_Stamp) VALUES(@e,'Not Active',@d,@stamp);
                    UPDATE AddEmployee SET Employee_Status='Not Active',IsActive='NO',Effective_Date=@d WHERE EmployeeID=@e", "@e",record,"@d",DateTime.Today,"@stamp",DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            }
            tx.Commit();
        }
    }
    public static void SaveRole(int employee,int business,int role)
    {
        var card=AnyminuteEmployeesView.RequireEmployee(employee);
        if(!card.Assignments.All(a=>a.Manage))throw new HttpException(403,"Subscriber ownership is required.");
        int actor=Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]);
        int user=Convert.ToInt32(HttpContext.Current.Session["UserId"]);
        using(var c=AnyminuteTimesheetRepair.Open())
        using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteApprovalAssignments.RequireOwner(c,tx,business);
            // Role changes are account/record-wide effects: all current business owners must agree.
            foreach(DataRow row in AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT BusinessID FROM AssginBussiness WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e AND BusinessID IS NOT NULL","@e",employee).Rows)
                AnyminuteApprovalAssignments.RequireOwner(c,tx,Convert.ToInt32(row[0]));
            if(employee==actor || AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WHERE EmployeeID=@e AND USERID=@u","@e",employee,"@u",user).Rows.Count>0)
                throw new InvalidOperationException("You cannot change your own business roles here.");
            var roles=AnyminuteTimesheetRepair.Query(c,tx,"SELECT u.User_TypeId FROM UserType u WHERE u.User_TypeId=@r AND (u.User_TypeId IN(2,4,5,12) OR (u.User_TypeId NOT IN(1,2,3,4,5,6,12) AND EXISTS(SELECT 1 FROM AssginBussiness owner WHERE owner.BusinessID=@b AND owner.UserTypeId=1 AND owner.EmployeeID=u.BownerID)))","@r",role,"@b",business);
            if(roles.Rows.Count!=1)throw new InvalidOperationException("Select a valid business role.");
            var current=AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT UserTypeId FROM AssginBussiness WITH(UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e","@b",business,"@e",employee);
            if(current.Rows.Count>0)
            {
                // Keep the established update/history path; never append a second active role row.
                tx.Commit();
                AnyminuteEmployeeRepair.UpdateRole(actor,1,business,employee,role);
                return;
            }
            if(AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 WHERE OBJECT_ID('dbo.AnyminuteEmployeeRoleHistory','U') IS NOT NULL AND OBJECT_ID('dbo.AnyminuteApprovalAudit','U') IS NOT NULL").Rows.Count!=1)
                throw new InvalidOperationException("Required role history/audit migrations are missing. No assignment was saved.");
            var linked=AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT USERID FROM MappingTable WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e AND USERID IS NOT NULL","@e",employee);
            if(linked.Rows.Count>1)throw new InvalidOperationException("Employee login identity is ambiguous. Resolve it before assigning a business.");
            if(AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WHERE EmployeeID=@e AND BusinessID=@b","@e",employee,"@b",business).Rows.Count>0)
                throw new InvalidOperationException("A login mapping exists without a matching assignment. Reconcile it before saving.");
            if(linked.Rows.Count==1 && AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WHERE USERID=@u AND BusinessID=@b AND EmployeeID<>@e","@u",linked.Rows[0][0],"@b",business,"@e",employee).Rows.Count>0)
                throw new InvalidOperationException("This login already has another employee record in the selected business. Edit that existing assignment instead.");
            // A shared login can span multiple employee records. Recheck every affected membership.
            if(linked.Rows.Count==1)
                foreach(DataRow b in AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT BusinessID FROM MappingTable WHERE USERID=@u AND BusinessID IS NOT NULL","@u",linked.Rows[0][0]).Rows)
                    AnyminuteApprovalAssignments.RequireOwner(c,tx,Convert.ToInt32(b[0]));
            var businessRow=AnyminuteTimesheetRepair.Query(c,tx,"SELECT CompanyName,MotherOrgId FROM AddBusiness WHERE BusinessID=@b","@b",business);
            if(businessRow.Rows.Count!=1)throw new InvalidOperationException("Business not found.");
            using(var cmd=new SqlCommand("Sp_InsertAssginBussiness",c,tx))
            {
                cmd.CommandType=CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@BusinessID",business);cmd.Parameters.AddWithValue("@EmployeeID",employee);
                cmd.Parameters.AddWithValue("@BusinessName",businessRow.Rows[0]["CompanyName"]);
                var person=AnyminuteTimesheetRepair.Query(c,tx,"SELECT dbo.FN_DESC('AddEmployee',@e) AS Name","@e",employee);
                cmd.Parameters.AddWithValue("@EmployeeName",person.Rows[0][0]);cmd.Parameters.AddWithValue("@UserTypeId",role);cmd.Parameters.AddWithValue("@MotherOrgID",businessRow.Rows[0]["MotherOrgId"]);cmd.ExecuteNonQuery();
            }
            if(Convert.ToInt32(AnyminuteTimesheetRepair.Query(c,tx,"SELECT COUNT(*) FROM AssginBussiness WHERE BusinessID=@b AND EmployeeID=@e AND UserTypeId=@r","@b",business,"@e",employee,"@r",role).Rows[0][0])!=1)
                throw new InvalidOperationException("The assignment procedure did not produce one canonical membership. No changes were saved.");
            if(linked.Rows.Count==1)AnyminuteTimesheetRepair.Execute(c,tx,"IF NOT EXISTS(SELECT 1 FROM MappingTable WHERE USERID=@u AND BusinessID=@b AND EmployeeID=@e AND UserTypeId=@r) INSERT MappingTable(USERID,BusinessID,EmployeeID,UserTypeId) VALUES(@u,@b,@e,@r)","@u",linked.Rows[0][0],"@b",business,"@e",employee,"@r",role);
            AnyminuteTimesheetRepair.Execute(c,tx,@"IF NOT EXISTS(SELECT 1 FROM AnyminuteEmployeeRoleHistory WHERE EmployeeId=@e AND BusinessId=@b AND EffectiveTo IS NULL AND UserTypeId=@r)
                INSERT AnyminuteEmployeeRoleHistory(EmployeeId,BusinessId,UserTypeId,EffectiveFrom,RecordedAt,Note) VALUES(@e,@b,@r,SYSUTCDATETIME(),SYSUTCDATETIME(),'Business assignment added');
                INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,'AddEmployeeAssignment',CONVERT(nvarchar(20),@r),SYSUTCDATETIME())", "@e",employee,"@b",business,"@r",role,"@a",actor);
            tx.Commit();
        }
    }
}
