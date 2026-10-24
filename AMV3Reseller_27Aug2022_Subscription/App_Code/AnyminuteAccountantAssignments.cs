using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
public static class AnyminuteAccountantAssignments
{
    public static DataTable Available(SqlConnection c,SqlTransaction tx)
    {
        int user=Convert.ToInt32(HttpContext.Current.Session["UserId"]);
        int actor=Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]);
        return AnyminuteTimesheetRepair.Query(c,tx,@"SELECT m.USERID,MIN(m.EmployeeID) AS EmployeeID,
            dbo.FN_DESC('AddEmployee',MIN(m.EmployeeID)) AS empname
            FROM MappingTable m JOIN AssginBussiness a ON a.EmployeeID=m.EmployeeID AND a.BusinessID=m.BusinessID AND a.UserTypeId=m.UserTypeId
            JOIN AddEmployee e ON e.EmployeeID=m.EmployeeID
            WHERE m.UserTypeId=12 AND UPPER(LTRIM(RTRIM(e.IsActive)))='YES'
              AND EXISTS (SELECT 1 FROM AssginBussiness o JOIN AddBusiness b ON b.BusinessID=o.BusinessID
                WHERE o.BusinessID=m.BusinessID AND o.UserTypeId IN(1,5) AND b.Status<>2
                  AND (o.EmployeeID=@a OR EXISTS (SELECT 1 FROM MappingTable own
                    WHERE own.USERID=@u AND own.EmployeeID=o.EmployeeID AND own.BusinessID=o.BusinessID AND own.UserTypeId=o.UserTypeId)))
            GROUP BY m.USERID ORDER BY empname", "@u",user,"@a",actor);
    }
    public static DataTable Current(SqlConnection c,SqlTransaction tx,int business)
    { return AnyminuteTimesheetRepair.Query(c,tx,@"SELECT DISTINCT m.USERID,m.EmployeeID,dbo.FN_DESC('AddEmployee',m.EmployeeID) AS empname
        FROM MappingTable m JOIN AssginBussiness a ON a.EmployeeID=m.EmployeeID AND a.BusinessID=m.BusinessID AND a.UserTypeId=m.UserTypeId
        WHERE m.BusinessID=@b AND m.UserTypeId=12", "@b",business); }
    public static void Save(int business,int user,bool remove)
    {
        using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,business);
            var existing=Current(c,tx,business).Select("USERID="+user);
            if(existing.Length>1) throw new InvalidOperationException("Accountant mapping is ambiguous. Resolve duplicate assignments first.");
            if(remove)
            {
                if(existing.Length!=1) throw new InvalidOperationException("Accountant assignment changed. Refresh first.");
                int employee=Convert.ToInt32(existing[0]["EmployeeID"]);
                AnyminuteTimesheetRepair.Execute(c,tx,@"DELETE User_Role_Permission WHERE Employee_Id=@e AND BusinessId=@b AND UserTypeId=12;
                    DELETE MappingTable WHERE USERID=@u AND EmployeeID=@e AND BusinessID=@b AND UserTypeId=12;
                    DELETE AssginBussiness WHERE EmployeeID=@e AND BusinessID=@b AND UserTypeId=12;", "@u",user,"@e",employee,"@b",business);
                Audit(c,tx,business,actor,employee,"AccountantUnassigned");tx.Commit();return;
            }
            var sources=Available(c,tx).Select("USERID="+user);
            if(sources.Length!=1) throw new InvalidOperationException("Select an accountant in a business you manage.");
            if(existing.Length==1) { tx.Commit();return; }
            if(AnyminuteTimesheetRepair.Query(c,tx,"SELECT 1 FROM MappingTable WHERE USERID=@u AND BusinessID=@b","@u",user,"@b",business).Rows.Count>0)
                throw new InvalidOperationException("This account already has another role in this business. Edit its role instead.");
            int source=Convert.ToInt32(sources[0]["EmployeeID"]);
            var permissions=AnyminuteTimesheetRepair.Query(c,tx,"SELECT DISTINCT Role_Permission_Id FROM User_Role_Permission WHERE Employee_Id=@e AND UserTypeId=12 AND Status=1","@e",source);
            if(permissions.Rows.Count!=1) throw new InvalidOperationException("Source accountant permissions are missing or ambiguous.");
            // OUTPUT retrieves this transaction's IDs; never use TOP 1/MAX to find another request's insertion.
            int employeeId;
            using(var cmd=AnyminuteTimesheetRepair.Command(c,tx,@"INSERT AddEmployee
                (UserType,Title,FirstName,MiddleName,LastName,DateOfBirth,Gender,SocialInsuranceNumber,MobilePhone,HomePhone,EMail_primary,EMail_secondary,
                StreetNumber,StreetName,Apartment,City,Province,PostalCode,HireDate,BillRate,BillType,JobTitle,TerminationDate,IsEmployeeEIExempt,IsEmployeeCPPExempt,
                RetainVacation,MotherOrgID,BusinessID,MangerId,BusinessOwnerId,IsActive,LegalName,Occupation,ADDRESSID,Rateid,SINExpiry,IsEdit,EMPLOYEETYPE)
                OUTPUT inserted.EmployeeID
                SELECT 12,Title,FirstName,MiddleName,LastName,DateOfBirth,Gender,SocialInsuranceNumber,MobilePhone,HomePhone,EMail_primary,EMail_secondary,
                StreetNumber,StreetName,Apartment,City,Province,PostalCode,HireDate,BillRate,BillType,JobTitle,TerminationDate,IsEmployeeEIExempt,IsEmployeeCPPExempt,
                RetainVacation,MotherOrgID,@b,MangerId,BusinessOwnerId,'YES',LegalName,Occupation,ADDRESSID,NULL,SINExpiry,IsEdit,EMPLOYEETYPE
                FROM AddEmployee WHERE EmployeeID=@source", "@b",business,"@source",source)) employeeId=Convert.ToInt32(cmd.ExecuteScalar());
            AnyminuteTimesheetRepair.Execute(c,tx,@"INSERT EmpAddress (Postalcode,NumberSuffix,StreetName,StreetType,StreetDirection,SuitType,SuiteNumber,SuitSuffix,City,Province,MainPhone,WorkPhone,Mobile,Fax,MainEmail,CCEmail,Website,Other1,StreetNo)
                SELECT Postalcode,NumberSuffix,StreetName,StreetType,StreetDirection,SuitType,SuiteNumber,SuitSuffix,City,Province,MainPhone,WorkPhone,Mobile,Fax,MainEmail,CCEmail,Website,Other1,StreetNo
                FROM EmpAddress WHERE EmpAddressId=(SELECT ADDRESSID FROM AddEmployee WHERE EmployeeID=@source);
                IF @@ROWCOUNT<>1 THROW 51037,'Source accountant address is missing.',1;
                UPDATE AddEmployee SET ADDRESSID=CONVERT(int,SCOPE_IDENTITY()) WHERE EmployeeID=@e;
                INSERT RateDetails (EmployeeId,Rate,RateType,StartDate,EndDate,JobTitle,IsEmployeeEIExempty,IsEmployeeCPPExempty,RetainVacation,effectivedate)
                SELECT @e,Rate,RateType,StartDate,EndDate,JobTitle,IsEmployeeEIExempty,IsEmployeeCPPExempty,RetainVacation,effectivedate
                FROM RateDetails WHERE RateId=(SELECT Rateid FROM AddEmployee WHERE EmployeeID=@source);
                IF @@ROWCOUNT=1 UPDATE AddEmployee SET Rateid=CONVERT(int,SCOPE_IDENTITY()) WHERE EmployeeID=@e;
                INSERT AssginBussiness (BusinessID,EmployeeID,BusinessName,EmployeeName,UserTypeId,MotherOrgID)
                SELECT @b,@e,dbo.FN_DESC('AddBusiness',@b),dbo.FN_DESC('AddEmployee',@e),12,MotherOrgID FROM AddEmployee WHERE EmployeeID=@e;
                INSERT MappingTable (USERID,UserTypeId,EmployeeID,BusinessID) VALUES (@u,12,@e,@b);
                INSERT User_Role_Permission (Role_Permission_Id,Employee_Id,UserTypeId,Status,BusinessId) VALUES (@permissions,@e,12,1,@b);",
                "@source",source,"@e",employeeId,"@b",business,"@u",user,"@permissions",permissions.Rows[0]["Role_Permission_Id"]);
            Audit(c,tx,business,actor,employeeId,"AccountantAssigned");tx.Commit();
        }
    }
    static void Audit(SqlConnection c,SqlTransaction tx,int business,int actor,int employee,string action)
    { AnyminuteTimesheetRepair.Execute(c,tx,"INSERT AnyminuteApprovalAudit (BusinessID,ActorID,EmployeeID,Action,RecordedAtUTC) VALUES (@b,@a,@e,@action,SYSUTCDATETIME())","@b",business,"@a",actor,"@e",employee,"@action",action); }
}
