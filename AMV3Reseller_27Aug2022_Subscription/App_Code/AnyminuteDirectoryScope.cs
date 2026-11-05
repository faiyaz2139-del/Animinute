using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

// Read scope for the user directory. No email-based identity joins and no writes.
public static class AnyminuteDirectoryScope
{
    static int SessionNumber(string key)
    {
        int value;
        var context = HttpContext.Current;
        if (context == null || context.Session == null ||
            !Int32.TryParse(Convert.ToString(context.Session[key]), out value) || value <= 0)
            throw new HttpException(401, "Sign in again to continue.");
        return value;
    }

    public static bool CanReadDirectory()
    {
        // A login may hold an employee role in one business and a manager role in
        // another. Use persisted account memberships rather than a display/session role.
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            int user = SessionNumber("UserId"), employee = SessionNumber("EmployeeId");
            var identity = AnyminuteTimesheetRepair.Query(c, null,
                "SELECT UserTypeid FROM UserAcc WHERE UserId=@u AND EmployeeId=@e", "@u", user, "@e", employee);
            if (identity.Rows.Count != 1) throw new HttpException(401, "Your account could not be verified.");
            int role;
            if (Int32.TryParse(Convert.ToString(identity.Rows[0][0]), out role) &&
                (role == 1 || role == 2 || role == 5 || role == 12)) return true;
            // v64 (PPT s2/s3 User List): an owner recorded in AssginBussiness (UserTypeId=1) can always read the directory, even
            // when the login's UserAcc/session role is not 1 (subscriber after password reset).
            if (AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 1 FROM AssginBussiness WHERE EmployeeID=@e AND UserTypeId=1", "@e", employee).Rows.Count > 0) return true;
            return AnyminuteTimesheetRepair.Query(c, null, @"
                SELECT DISTINCT m.BusinessID FROM MappingTable m
                JOIN AssginBussiness a ON a.BusinessID=m.BusinessID AND a.EmployeeID=m.EmployeeID AND a.UserTypeId=m.UserTypeId
                JOIN AddBusiness b ON b.BusinessID=m.BusinessID
                WHERE m.USERID=@u AND m.UserTypeId IN (1,2,5,12) AND b.Status='1'", "@u", user).Rows.Count > 0;
        }
    }

    public static void RequireDirectoryRole()
    {
        if (!CanReadDirectory()) throw new HttpException(403, "Your business roles cannot open the user directory.");
    }

    public static DataTable Businesses()
    {
        RequireDirectoryRole();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var candidates = AnyminuteTimesheetRepair.Query(c, null, @"
                SELECT DISTINCT b.BusinessID,b.CompanyName FROM AddBusiness b
                WHERE b.Status='1' AND (
                    EXISTS (SELECT 1 FROM MappingTable m JOIN AssginBussiness a
                        ON a.BusinessID=m.BusinessID AND a.EmployeeID=m.EmployeeID AND a.UserTypeId=m.UserTypeId
                        WHERE m.USERID=@u AND m.BusinessID=b.BusinessID AND m.UserTypeId IN (1,2,5,12))
                    OR (@r=@r AND EXISTS (SELECT 1 FROM AssginBussiness a
                        WHERE a.BusinessID=b.BusinessID AND a.EmployeeID=@e AND a.UserTypeId=1)))
                ORDER BY b.CompanyName", "@u", SessionNumber("UserId"),
                "@e", SessionNumber("EmployeeId"), "@r", SessionNumber("UserTypeid"));
            // Resolve again per business, using the same persisted identity checks as mutations.
            foreach (DataRow row in candidates.Rows)
            {
                int actor = SessionNumber("EmployeeId"), role = SessionNumber("UserTypeid");
                AnyminuteTimesheetRepair.ResolveBusinessActor(c, null, ref actor, ref role, Convert.ToInt32(row["BusinessID"]));
                if (role != 1 && role != 2 && role != 5 && role != 12) row.Delete(); // v64: skip, don't fail the whole page
            }
            candidates.AcceptChanges();
            return candidates;
        }
    }

    public static int BusinessRole(SqlConnection c, int business, out int actor)
    {
        RequireDirectoryRole();
        actor = SessionNumber("EmployeeId");
        int role = SessionNumber("UserTypeid");
        AnyminuteTimesheetRepair.ResolveBusinessActor(c, null, ref actor, ref role, business);
        if (role != 1 && role != 2 && role != 5 && role != 12)
            throw new HttpException(403, "Your business role cannot open the user directory.");
        return role;
    }

    public static DataTable Users(int business)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            int actor; int role = BusinessRole(c, business, out actor);
            // Managers see only explicitly assigned employees in the selected business.
            return AnyminuteTimesheetRepair.Query(c, null, @"
                SELECT DISTINCT e.EmployeeID,a.BusinessID,a.UserTypeId,
                    LTRIM(RTRIM(ISNULL(e.FirstName,'')+' '+ISNULL(e.LastName,''))) EmpName,
                    ISNULL(ut.UserType,'') UserRoleName,ISNULL(e.EMPLOYEETYPE,'') EMPLOYEETYPE,
                    CASE WHEN e.EMPLOYEETYPE='P' THEN 'Project' ELSE 'Regular' END Employee_Type,
                    ISNULL(e.IsActive,'') IsActive,
                    COALESCE(NULLIF(h.mp_Stauts,''),NULLIF(e.Employee_Status,''),
                        CASE WHEN UPPER(LTRIM(RTRIM(e.IsActive)))='YES' THEN 'Active' ELSE 'Not Active' END) Employee_Status,
                    COALESCE(h.Effective_Date,e.Effective_Date) Effective_Date,
                    ISNULL(ad.MainEmail,ISNULL(e.EMail_primary,'')) Email,
                    ISNULL(ad.MainPhone,ISNULL(e.HomePhone,'')) Phone,
                    ISNULL(ad.Mobile,ISNULL(e.MobilePhone,'')) Mobile,
                    CASE WHEN rd.Rate IS NULL THEN '' ELSE '$ '+CONVERT(varchar(30),CONVERT(decimal(18,2),rd.Rate))+'/'+ISNULL(rd.RateType,'') END Rate,
                    'Y' IsEnterTimeSheetRoleExists,'Y' IsExistsUpdateAddress,'Y' IsExistsActivateDeactivateEmployee,
                    a.UserTypeId UserType
                FROM AddEmployee e JOIN AssginBussiness a ON a.EmployeeID=e.EmployeeID
                LEFT JOIN EmpAddress ad ON ad.EmpAddressId=e.ADDRESSID
                LEFT JOIN UserType ut ON ut.User_TypeId=a.UserTypeId
                OUTER APPLY (SELECT TOP(1) Rate,RateType FROM RateDetails r
                    WHERE r.EmployeeId=e.EmployeeID AND r.effectivedate<=GETDATE()
                    ORDER BY r.effectivedate DESC,r.RateId DESC) rd
                OUTER APPLY (SELECT TOP(1) mp_Stauts,Effective_Date FROM Emp_Active_Inactive_Status h
                    WHERE h.EmpId=e.EmployeeID AND h.Effective_Date<=CONVERT(date,GETDATE())
                    ORDER BY h.Effective_Date DESC,TRY_CONVERT(datetime2,h.Datetime_Stamp) DESC,h.Datetime_Stamp DESC) h
                WHERE a.BusinessID=@b AND a.UserTypeId<>1 AND
                    (@r<>2 OR EXISTS (SELECT 1 FROM AssgineManager m
                        WHERE m.BusinessID=@b AND m.ManagerID=@actor AND m.EmpId=e.EmployeeID))
                ORDER BY EmpName,e.EmployeeID", "@b",business,"@r",role,"@actor",actor);
        }
    }
}
