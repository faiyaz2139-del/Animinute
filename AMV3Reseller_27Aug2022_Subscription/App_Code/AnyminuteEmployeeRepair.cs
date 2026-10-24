using System;
using System.Data;
using System.Data.SqlClient;

public static class AnyminuteEmployeeRepair
{
    public static void RemoveBusinessAssignment(int actor,int role,int business,int employee)
    {
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,role,business,employee,false);
            var assignments=AnyminuteTimesheetRepair.Query(c,tx,
                "SELECT UserTypeId FROM AssginBussiness WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e",
                "@b",business,"@e",employee);
            if(assignments.Rows.Count==0)throw new InvalidOperationException("Assignment no longer exists. Refresh the page.");
            foreach(DataRow assignment in assignments.Rows)
                if(Convert.ToInt32(assignment[0])==1)throw new InvalidOperationException("Subscriber ownership cannot be removed here.");
            int userId=Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
            if(actor==employee || AnyminuteTimesheetRepair.Query(c,tx,
                "SELECT 1 FROM MappingTable WHERE USERID=@u AND EmployeeID=@e AND BusinessID=@b",
                "@u",userId,"@e",employee,"@b",business).Rows.Count>0)
                throw new InvalidOperationException("You cannot remove your own access here.");
            AnyminuteTimesheetRepair.Execute(c,tx,
                "DELETE FROM AssgineManager WHERE BusinessID=@b AND (ManagerID=@e OR EmpId=@e); " +
                "DELETE FROM ASSIGNPROJECT WHERE BUSINESSID=@b AND EMPLOYEEID=@e; " +
                "DELETE FROM User_Role_Permission WHERE BusinessID=@b AND Employee_Id=@e; " +
                "DELETE FROM MappingTable WHERE BusinessID=@b AND EmployeeID=@e; " +
                "DELETE FROM AssginBussiness WHERE BusinessID=@b AND EmployeeID=@e;",
                "@b",business,"@e",employee);
            tx.Commit();
        }
    }
    public static void UpdateRole(int actor,int actorRole,int business,int employee,int role)
    {
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            int verifiedActor=actor, verifiedRole=actorRole;
            AnyminuteTimesheetRepair.ResolveBusinessActor(c,tx,ref verifiedActor,ref verifiedRole,business);
            if(verifiedRole!=1)throw new InvalidOperationException("Only a subscriber can change business roles here.");
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,actorRole,business,employee,false);
            if(actor==employee)throw new InvalidOperationException("You cannot change your own role here.");
            if(AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM UserType u WHERE u.User_TypeId=@r AND (u.User_TypeId IN (2,4,5,12) OR (u.User_TypeId NOT IN (1,2,3,4,5,6,12) AND EXISTS (SELECT 1 FROM AssginBussiness owner WHERE owner.BusinessID=@b AND owner.UserTypeId=1 AND owner.EmployeeID=u.BownerID)))","@r",role,"@b",business).Rows.Count!=1)throw new InvalidOperationException("Select a valid role.");

            // Refuse conflicting tenant metadata rather than discarding an arbitrary row.
            if(AnyminuteTimesheetRepair.Query(c,tx,
                "SELECT DISTINCT ISNULL(MotherOrgID,0) Org FROM AssginBussiness WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e",
                "@b",business,"@e",employee).Rows.Count>1)
                throw new InvalidOperationException("Assignment organization metadata is inconsistent. Review it before changing roles.");
            AnyminuteTimesheetRepair.Execute(c,tx,@"
                ;WITH d AS (
                    SELECT AssginBussinessId, ROW_NUMBER() OVER (PARTITION BY BusinessID,EmployeeID,UserTypeId ORDER BY AssginBussinessId) rn
                    FROM AssginBussiness WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e
                ) DELETE FROM d WHERE rn>1;", "@b",business,"@e",employee);

            var current=AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT UserTypeId FROM AssginBussiness WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e","@b",business,"@e",employee);
            if(current.Rows.Count!=1)throw new InvalidOperationException("Business role assignments are missing or inconsistent. Review them first.");
            if(Convert.ToInt32(current.Rows[0][0])==1)throw new InvalidOperationException("Subscriber ownership cannot be changed here.");
            // Also repair exact mapping duplicates on an idempotent same-role update.
            AnyminuteTimesheetRepair.Execute(c,tx,@"
                ;WITH d AS (
                    SELECT MappingId, ROW_NUMBER() OVER (PARTITION BY USERID,BusinessID,EmployeeID,UserTypeId ORDER BY MappingId) rn
                    FROM MappingTable WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e
                ) DELETE FROM d WHERE rn>1;", "@b",business,"@e",employee);
            if(AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WITH (UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e AND BusinessID=@b","@e",employee,"@b",business).Rows.Count==0)throw new InvalidOperationException("Login mapping is missing. Contact support.");
            if(Convert.ToInt32(current.Rows[0][0])==role){tx.Commit();return;}
            AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AssginBussiness SET UserTypeId=@r WHERE EmployeeID=@e AND BusinessID=@b; UPDATE MappingTable SET UserTypeId=@r WHERE EmployeeID=@e AND BusinessID=@b; UPDATE AddEmployee SET UserType=@r WHERE EmployeeID=@e AND NOT EXISTS (SELECT 1 FROM AssginBussiness WHERE EmployeeID=@e AND BusinessID<>@b)","@r",role,"@e",employee,"@b",business);
            // Updating inconsistent old mapping roles can collapse them to the same key.
            AnyminuteTimesheetRepair.Execute(c,tx,@"
                ;WITH d AS (
                    SELECT MappingId, ROW_NUMBER() OVER (PARTITION BY USERID,BusinessID,EmployeeID,UserTypeId ORDER BY MappingId) rn
                    FROM MappingTable WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b AND EmployeeID=@e
                ) DELETE FROM d WHERE rn>1;", "@b",business,"@e",employee);
            // Retain existing permissions for the new role if configured; never transfer old-role privileges.
            AnyminuteTimesheetRepair.Execute(c,tx,"IF NOT EXISTS (SELECT 1 FROM User_Role_Permission WHERE Employee_Id=@e AND UserTypeId=@r AND BusinessId=@b) INSERT User_Role_Permission (Employee_Id,UserTypeId,BusinessId,Role_Permission_Id,Status) VALUES (@e,@r,@b,'',1)","@e",employee.ToString(),"@r",role.ToString(),"@b",business);

            // Persist role history in the same transaction as the role change.
            // The DB trigger remains a second line of defence, but the application no longer depends on it.
            DateTime roleChangedAt = DateTime.UtcNow;
            AnyminuteTimesheetRepair.Execute(c,tx,@"
                UPDATE dbo.AnyminuteEmployeeRoleHistory
                   SET EffectiveTo=@now, Note=LEFT(Note + N'; closed by application role change',200)
                 WHERE EmployeeId=@e AND BusinessId=@b AND EffectiveTo IS NULL
                   AND ISNULL(UserTypeId,-1)<>@r;

                IF NOT EXISTS (SELECT 1 FROM dbo.AnyminuteEmployeeRoleHistory
                               WHERE EmployeeId=@e AND BusinessId=@b AND EffectiveTo IS NULL
                                 AND ISNULL(UserTypeId,-1)=@r)
                INSERT dbo.AnyminuteEmployeeRoleHistory
                    (EmployeeId,BusinessId,UserTypeId,EffectiveFrom,RecordedAt,Note)
                VALUES (@e,@b,@r,@now,@now,N'Role change recorded by application');",
                "@e",employee,"@b",business,"@r",role,"@now",roleChangedAt);
            tx.Commit();
        }
    }
    public static void UpdateStatus(int actor,int role,int business,int employee,string status,DateTime effective)
    {
        if(status!="Active" && status!="Not Active" && status!="Terminated")throw new InvalidOperationException("Select a valid employee status.");
        if(effective.Date>DateTime.Today)throw new InvalidOperationException("Future status scheduling is not supported. Select today or a historical effective date.");
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteTimesheetRepair.Authorize(c,tx,actor,role,business,employee,false);
            AnyminuteTimesheetRepair.Query(c,tx,"SELECT EmployeeID FROM AddEmployee WITH (UPDLOCK,HOLDLOCK) WHERE EmployeeID=@e","@e",employee);
            AnyminuteTimesheetRepair.Execute(c,tx,"INSERT Emp_Active_Inactive_Status (EmpId,mp_Stauts,Effective_Date,Datetime_Stamp) VALUES (@e,@s,@d,@now)","@e",employee,"@s",status,"@d",effective.Date,"@now",DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
            // An old history entry must not overwrite a newer effective state. Original hire date is retained.
            AnyminuteTimesheetRepair.Execute(c,tx,@"UPDATE a SET Employee_Status=h.mp_Stauts,Effective_Date=h.Effective_Date,
                IsActive=CASE WHEN h.mp_Stauts='Active' THEN 'YES' ELSE 'NO' END,
                TerminationDate=CASE WHEN h.mp_Stauts='Terminated' THEN h.Effective_Date ELSE NULL END
                FROM AddEmployee a CROSS APPLY (SELECT TOP(1) mp_Stauts,Effective_Date FROM Emp_Active_Inactive_Status
                  WHERE EmpId=a.EmployeeID AND Effective_Date<=CONVERT(date,GETDATE())
                  ORDER BY Effective_Date DESC,TRY_CONVERT(datetime2,Datetime_Stamp) DESC,Datetime_Stamp DESC) h
                WHERE a.EmployeeID=@e","@e",employee);
            tx.Commit();
        }
    }
    public static DataTable StatusHistory(int employee)
    {
        using(var c=AnyminuteTimesheetRepair.Open()) return AnyminuteTimesheetRepair.Query(c,null,@"SELECT mp_Stauts,Effective_Date,
            LEAD(Effective_Date) OVER (ORDER BY Effective_Date,TRY_CONVERT(datetime2,Datetime_Stamp),Datetime_Stamp) AS End_Date,
            Datetime_Stamp AS DateTime_Stamp FROM Emp_Active_Inactive_Status WHERE EmpId=@e
            ORDER BY Effective_Date DESC,TRY_CONVERT(datetime2,Datetime_Stamp) DESC,Datetime_Stamp DESC","@e",employee);
    }
    public static DataTable RoleHistory(int employee,int business)
    {
        using(var c=AnyminuteTimesheetRepair.Open())return AnyminuteTimesheetRepair.Query(c,null,@"SELECT b.CompanyName AS Business,
            u.UserType AS Role,h.EffectiveFrom AS Start_Date_UTC,h.EffectiveTo AS End_Date_UTC,
            CASE WHEN h.EffectiveTo IS NULL THEN 'Current' ELSE 'Previous' END AS Role_Status,h.RecordedAt AS Recorded_UTC,h.Note
            FROM AnyminuteEmployeeRoleHistory h LEFT JOIN AddBusiness b ON b.BusinessID=h.BusinessId
            LEFT JOIN UserType u ON u.User_TypeId=h.UserTypeId
            WHERE h.EmployeeId=@e AND h.BusinessId=@b ORDER BY h.RecordedAt DESC,h.HistoryId DESC","@e",employee,"@b",business);
    }
    public static bool Active(object status,object flag)
    {
        string s=Convert.ToString(status).Trim(); return s.Length>0 ? s=="Active" : Convert.ToString(flag).Trim().Equals("YES",StringComparison.OrdinalIgnoreCase);
    }
    public static DataTable Enrich(DataTable data)
    {
        if(!data.Columns.Contains("EmployeeID"))return data;
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            var states=AnyminuteTimesheetRepair.Query(c,null,@"SELECT a.EmployeeID,
                COALESCE(h.mp_Stauts,NULLIF(a.Employee_Status,''),CASE WHEN LTRIM(RTRIM(a.IsActive))='YES' THEN 'Active' ELSE 'Not Active' END) Employee_Status,
                COALESCE(h.Effective_Date,a.Effective_Date) Effective_Date
                FROM AddEmployee a OUTER APPLY (SELECT TOP(1) mp_Stauts,Effective_Date FROM Emp_Active_Inactive_Status
                 WHERE EmpId=a.EmployeeID AND Effective_Date<=CONVERT(date,GETDATE()) ORDER BY Effective_Date DESC,
                 TRY_CONVERT(datetime2,Datetime_Stamp) DESC,Datetime_Stamp DESC) h");
            var roleNames=AnyminuteTimesheetRepair.Query(c,null,"SELECT User_TypeId,UserType FROM UserType");
            roleNames.PrimaryKey=new DataColumn[]{roleNames.Columns["User_TypeId"]};
            states.PrimaryKey=new DataColumn[]{states.Columns["EmployeeID"]};
            if(!data.Columns.Contains("Employee_Status"))data.Columns.Add("Employee_Status",typeof(string));
            if(!data.Columns.Contains("Effective_Date"))data.Columns.Add("Effective_Date",typeof(DateTime));
            foreach(DataRow row in data.Rows)
            {
                var state=states.Rows.Find(row["EmployeeID"]);
                if(state!=null){row["Employee_Status"]=state["Employee_Status"];row["Effective_Date"]=state["Effective_Date"];}
                if(data.Columns.Contains("UserTypeId") && row["UserTypeId"]!=DBNull.Value)
                {
                    var role=roleNames.Rows.Find(row["UserTypeId"]);
                    if(role!=null && data.Columns.Contains("UserRoleName"))row["UserRoleName"]=role["UserType"];
                    if(data.Columns.Contains("UserType"))row["UserType"]=row["UserTypeId"];
                }
            }
        }
        return data;
    }
    public static int ActiveCount(int business)
    {
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            var data=AnyminuteTimesheetRepair.Query(c,null,@"SELECT COUNT(DISTINCT a.EmployeeID) FROM AddEmployee a
                JOIN AssginBussiness x ON x.EmployeeID=a.EmployeeID
                OUTER APPLY (SELECT TOP(1) mp_Stauts FROM Emp_Active_Inactive_Status WHERE EmpId=a.EmployeeID
                  AND Effective_Date<=CONVERT(date,GETDATE()) ORDER BY Effective_Date DESC,TRY_CONVERT(datetime2,Datetime_Stamp) DESC,Datetime_Stamp DESC) h
                WHERE x.BusinessID=@b AND x.UserTypeId<>1 AND COALESCE(h.mp_Stauts,NULLIF(a.Employee_Status,''),
                  CASE WHEN LTRIM(RTRIM(a.IsActive))='YES' THEN 'Active' ELSE 'Not Active' END)='Active'","@b",business);
            return Convert.ToInt32(data.Rows[0][0]);
        }
    }
}
