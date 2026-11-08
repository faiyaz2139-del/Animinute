using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SA = AnyminuteSupportAdmin;

// 2026-11-03 v59 (t188u/t189u "erase the data from all modules"): Support Admin > Subscriber Details > Erase All Module Data.
// Default is per subscriber; v69 global wrapper uses keepLogins=true. Hard-deletes the module records under the subscriber's businesses - businesses,
// employees, projects, timesheets / breaks / weeks, schedules, approvals, notifications, role and permission links, tickets,
// business / employee addresses and their pending address-verification rows, and the logins of erased employees.
// Kept: the subscriber account shell (UserAcc, owner AddEmployee + EmpAddress, one owner AssginBussiness + MappingTable row so the
// subscriber still lists and can sign in), PlanHistory / billing / Stripe, AnyminuteSupportAudit (the erase itself is audited),
// archive tables, and shared master data. No table is dropped or truncated.
// Mirrors the per-employee SP DeleteAllDetails (RateDetails, EmpAddress, AssginBussiness, MappingTable, ASSIGNPROJECT,
// User_Role_Permission) inside one transaction for every employee who works only for this subscriber; employees that also
// work for another subscriber's business are only unlinked from these businesses (their record and login stay).
// Businesses that another owner also owns (UserTypeId 1 for a different employee) are skipped and reported.
public static class AnyminuteSubscriberErase
{
    public const string Word = "ERASE";
    public class Result { public List<KeyValuePair<string, int>> Rows = new List<KeyValuePair<string, int>>(); public int Skipped; public string Summary() { return String.Join(", ", Rows.Where(x => x.Value > 0).Select(x => x.Key + " " + x.Value).ToArray()); } }

    static int Run(SqlConnection c, SqlTransaction tx, string sql, params object[] args)
    {
        using (var cmd = AnyminuteTimesheetRepair.Command(c, tx, sql, args)) { cmd.CommandTimeout = 300; return cmd.ExecuteNonQuery(); }
    }
    static int Num(SqlConnection c, SqlTransaction tx, string sql, params object[] args)
    {
        using (var cmd = AnyminuteTimesheetRepair.Command(c, tx, sql, args)) { cmd.CommandTimeout = 300; object o = cmd.ExecuteScalar(); return o == null || o == DBNull.Value ? 0 : Convert.ToInt32(o, CultureInfo.InvariantCulture); }
    }
    static bool Col(SqlConnection c, SqlTransaction tx, string table, string col) { return AnyminuteEmployeeEditor.HasColumn(c, tx, table, col); }

    // Scope temp tables (session scoped): #eb businesses, #ae all non-owner employees there, #ee employees only working here,
    // #ep projects, #et tickets, #eu logins of #ee, #ea pending address-verification rows of these businesses / employees.
    // The CREATE batch has no parameters (plain batch, session scope); a parameterised batch would drop them at sp_executesql exit.
    static void Scope(SqlConnection c, SqlTransaction tx, int owner, int user, string email, string ownerName)
    {
        bool rv = AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAddressReview");
        Run(c, tx, @"IF OBJECT_ID('tempdb..#eb') IS NOT NULL DROP TABLE #eb; IF OBJECT_ID('tempdb..#ae') IS NOT NULL DROP TABLE #ae; IF OBJECT_ID('tempdb..#ee') IS NOT NULL DROP TABLE #ee;
            IF OBJECT_ID('tempdb..#ep') IS NOT NULL DROP TABLE #ep; IF OBJECT_ID('tempdb..#et') IS NOT NULL DROP TABLE #et; IF OBJECT_ID('tempdb..#eu') IS NOT NULL DROP TABLE #eu; IF OBJECT_ID('tempdb..#ea') IS NOT NULL DROP TABLE #ea; IF OBJECT_ID('tempdb..#sk') IS NOT NULL DROP TABLE #sk;
            CREATE TABLE #eb(BusinessID int PRIMARY KEY); CREATE TABLE #sk(BusinessID int PRIMARY KEY); CREATE TABLE #ae(EmployeeID int PRIMARY KEY); CREATE TABLE #ee(EmployeeID int PRIMARY KEY);
            CREATE TABLE #ep(ProjectID int PRIMARY KEY); CREATE TABLE #et(TicketId int PRIMARY KEY); CREATE TABLE #eu(UserId int PRIMARY KEY); CREATE TABLE #ea(AddressMasterId int PRIMARY KEY);");
        // Separate parameterised batch: it runs inside sp_executesql, so the temp tables must already exist at session level.
        Run(c, tx, @"INSERT #eb SELECT DISTINCT a.BusinessID FROM AssginBussiness a WHERE a.BusinessID IS NOT NULL AND ((a.EmployeeID=@a AND a.UserTypeId=1)
                OR EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=@u AND m.BusinessID=a.BusinessID AND m.EmployeeID=a.EmployeeID AND m.UserTypeId=a.UserTypeId));
            INSERT #sk SELECT b.BusinessID FROM #eb b WHERE EXISTS(SELECT 1 FROM AssginBussiness o WHERE o.BusinessID=b.BusinessID AND o.UserTypeId=1 AND o.EmployeeID<>@a);
            DELETE b FROM #eb b JOIN #sk s ON s.BusinessID=b.BusinessID;
            INSERT #ae SELECT DISTINCT a.EmployeeID FROM AssginBussiness a JOIN #eb b ON b.BusinessID=a.BusinessID WHERE a.EmployeeID IS NOT NULL AND a.EmployeeID<>@a;
            INSERT #ee SELECT e.EmployeeID FROM #ae e WHERE NOT EXISTS(SELECT 1 FROM AssginBussiness x WHERE x.EmployeeID=e.EmployeeID AND (x.BusinessID IS NULL OR x.BusinessID NOT IN (SELECT BusinessID FROM #eb)))
                AND NOT EXISTS(SELECT 1 FROM UserAcc ux WHERE ux.EmployeeId=e.EmployeeID AND LOWER(REPLACE(ISNULL(ux.UserType,''),' ','')) IN('supportadmin','superadmin','support'))
                AND NOT EXISTS(SELECT 1 FROM UserAcc ux JOIN MappingTable mx ON mx.USERID=ux.UserId WHERE ux.EmployeeId=e.EmployeeID AND (mx.UserTypeId=1 OR (ISNULL(mx.BusinessID,0)<>0 AND mx.BusinessID NOT IN(SELECT BusinessID FROM #eb))));
            INSERT #ep SELECT DISTINCT p.ProjectID FROM AddProject p JOIN #eb b ON b.BusinessID=p.BusinessID;
            INSERT #et SELECT DISTINCT t.TicketId FROM Ticket t WHERE t.BusinessId IN (SELECT BusinessID FROM #eb) OR t.EmpId IN (SELECT EmployeeID FROM #ee) OR (@m<>'' AND LTRIM(RTRIM(t.EmailId))=@m);
            INSERT #eu SELECT DISTINCT ux.UserId FROM UserAcc ux JOIN #ee e ON e.EmployeeID=ux.EmployeeId WHERE ux.UserId<>@u AND LOWER(REPLACE(ISNULL(ux.UserType,''),' ','')) NOT IN('supportadmin','superadmin','support')
                AND NOT EXISTS(SELECT 1 FROM MappingTable mx WHERE mx.USERID=ux.UserId AND (mx.UserTypeId=1 OR (ISNULL(mx.BusinessID,0)<>0 AND mx.BusinessID NOT IN (SELECT BusinessID FROM #eb))));" +
            (rv ? @" INSERT #ea SELECT DISTINCT r.AddressMasterId FROM AnyminuteAddressReview r WHERE ISNULL(r.Status,'Pending')<>'Verified' AND (r.SubmittedBy IN (SELECT LEFT(b.CompanyName+' (Business)',150) FROM AddBusiness b JOIN #eb e ON e.BusinessID=b.BusinessID)
                OR r.SubmittedBy IN (SELECT LEFT(LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,'')))+' (Employee)',150) FROM AddEmployee ae JOIN #ee e ON e.EmployeeID=ae.EmployeeID));" : ""),
            "@a", owner, "@u", user, "@m", (email ?? "").Trim());
    }
    static void Drop(SqlConnection c, SqlTransaction tx) { try { Run(c, tx, "DROP TABLE #eb; DROP TABLE #sk; DROP TABLE #ae; DROP TABLE #ee; DROP TABLE #ep; DROP TABLE #et; DROP TABLE #eu; DROP TABLE #ea;"); } catch (SqlException) { } }

    static int TimeRows(SqlConnection c, SqlTransaction tx)
    {
        int n = 0;
        foreach (var t in new[] { new[] { "TimeSheetEntry", "BussID" }, new[] { "TimeSheet", "BusinessID" }, new[] { "BreakTime", "BussID" }, new[] { "TimeCardStatus", "BusinessID" }, new[] { "AnyminuteWeek", "BusinessID" } })
            if (Col(c, tx, t[0], t[1])) n += Num(c, tx, "SELECT COUNT(*) FROM dbo." + t[0] + " WHERE " + t[1] + " IN (SELECT BusinessID FROM #eb)");
        return n;
    }
    // What the confirm modal shows before the wipe.
    public static Result Preview(SqlConnection c, int owner, int user, string email, string ownerName)
    {
        var r = new Result();
        try
        {
            Scope(c, null, owner, user, email, ownerName);
            r.Rows.Add(new KeyValuePair<string, int>("Businesses", Num(c, null, "SELECT COUNT(*) FROM #eb")));
            r.Rows.Add(new KeyValuePair<string, int>("Employees erased", Num(c, null, "SELECT COUNT(*) FROM #ee")));
            r.Rows.Add(new KeyValuePair<string, int>("Shared employees unlinked", Num(c, null, "SELECT COUNT(*) FROM #ae WHERE EmployeeID NOT IN (SELECT EmployeeID FROM #ee)")));
            r.Rows.Add(new KeyValuePair<string, int>("Projects", Num(c, null, "SELECT COUNT(*) FROM #ep")));
            r.Rows.Add(new KeyValuePair<string, int>("Timesheet / time-card rows", TimeRows(c, null)));
            r.Rows.Add(new KeyValuePair<string, int>("Support tickets", Num(c, null, "SELECT COUNT(*) FROM #et")));
            r.Rows.Add(new KeyValuePair<string, int>("Employee logins", Num(c, null, "SELECT COUNT(*) FROM #eu")));
            r.Rows.Add(new KeyValuePair<string, int>("Pending address verifications", Num(c, null, "SELECT COUNT(*) FROM #ea")));
            r.Skipped = Num(c, null, "SELECT COUNT(*) FROM #sk");
        }
        finally { Drop(c, null); }
        return r;
    }

    // Hard delete, one transaction; rolls back completely on any error. Returns per-module row counts (also written to the audit row).
    public static Result Erase(SqlConnection c, int owner, int user, string email, string ownerName, string ownerFirst, string reason, bool keepLogins = false)
    {
        var res = new Result(); Func<string, string, int> add = null;
        using (var tx = c.BeginTransaction())
        {
            Scope(c, tx, owner, user, email, ownerName);
            // Global run includes shared-owner businesses, preserves every login and its profile shell.
            if (keepLogins)
            {
                Run(c, tx, "INSERT #eb SELECT BusinessID FROM #sk; DELETE FROM #sk; DELETE FROM #eu;");
                // Rebuild dependent scope after shared businesses were included.
                Run(c, tx, @"INSERT #ae SELECT DISTINCT a.EmployeeID FROM AssginBussiness a JOIN #eb b ON b.BusinessID=a.BusinessID WHERE a.EmployeeID IS NOT NULL AND a.EmployeeID<>@a AND a.EmployeeID NOT IN (SELECT EmployeeID FROM #ae);
                    INSERT #ee SELECT e.EmployeeID FROM #ae e WHERE e.EmployeeID NOT IN (SELECT EmployeeID FROM #ee) AND NOT EXISTS(SELECT 1 FROM AssginBussiness x WHERE x.EmployeeID=e.EmployeeID AND (x.BusinessID IS NULL OR x.BusinessID NOT IN (SELECT BusinessID FROM #eb))) AND NOT EXISTS(SELECT 1 FROM UserAcc ux JOIN MappingTable m ON m.USERID=ux.UserId WHERE ux.EmployeeId=e.EmployeeID AND m.UserTypeId=1)
                    AND NOT EXISTS(SELECT 1 FROM UserAcc ux WHERE ux.EmployeeId=e.EmployeeID AND LOWER(REPLACE(ISNULL(ux.UserType,''),' ','')) IN('supportadmin','superadmin','support'));
                    INSERT #ep SELECT p.ProjectID FROM AddProject p JOIN #eb b ON b.BusinessID=p.BusinessID WHERE p.ProjectID NOT IN (SELECT ProjectID FROM #ep);
                    INSERT #et SELECT t.TicketId FROM Ticket t WHERE t.BusinessId IN (SELECT BusinessID FROM #eb) AND t.TicketId NOT IN (SELECT TicketId FROM #et);", "@a", owner);
            }
            if (keepLogins && AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAddressReview"))
                Run(c, tx, @"INSERT #ea SELECT DISTINCT r.AddressMasterId FROM AnyminuteAddressReview r WHERE ISNULL(r.Status,'Pending')<>'Verified' AND r.AddressMasterId NOT IN (SELECT AddressMasterId FROM #ea) AND
                    (r.SubmittedBy IN (SELECT LEFT(b.CompanyName+' (Business)',150) FROM AddBusiness b JOIN #eb e ON e.BusinessID=b.BusinessID)
                    OR r.SubmittedBy IN (SELECT LEFT(LTRIM(RTRIM(ISNULL(a.FirstName,'')+' '+ISNULL(a.LastName,'')))+' (Employee)',150) FROM AddEmployee a JOIN #ee e ON e.EmployeeID=a.EmployeeID));");
            // v72: snapshot parent-owned addresses/rates before deleting profiles/businesses.
            // Detach nullable rate references first; legacy databases may have both RateDetails.EmployeeId
            // and AddEmployee.Rateid foreign keys. Never delete a shared rate/address still in use.
            Run(c, tx, @"IF OBJECT_ID('tempdb..#erate') IS NOT NULL DROP TABLE #erate;
                IF OBJECT_ID('tempdb..#eaddr') IS NOT NULL DROP TABLE #eaddr;
                IF OBJECT_ID('tempdb..#baddr') IS NOT NULL DROP TABLE #baddr;
                SELECT DISTINCT Rateid INTO #erate FROM AddEmployee WHERE EmployeeID IN(SELECT EmployeeID FROM #ee) AND Rateid IS NOT NULL;
                SELECT DISTINCT ADDRESSID INTO #eaddr FROM AddEmployee WHERE EmployeeID IN(SELECT EmployeeID FROM #ee) AND ADDRESSID IS NOT NULL;
                SELECT DISTINCT ADDRESSID INTO #baddr FROM AddBusiness WHERE BusinessID IN(SELECT BusinessID FROM #eb) AND ADDRESSID IS NOT NULL;
                UPDATE AddEmployee SET Rateid=NULL WHERE EmployeeID IN(SELECT EmployeeID FROM #ee);");
            res.Skipped = Num(c, tx, "SELECT COUNT(*) FROM #sk");
            var bucket = new Dictionary<string, int>();
            add = (k, sql) => { int n = Run(c, tx, sql); bucket[k] = (bucket.ContainsKey(k) ? bucket[k] : 0) + Math.Max(0, n); return n; };
            Action<string, string, string, string> del = (k, table, col, set) => { if (Col(c, tx, table, col)) add(k, "DELETE FROM dbo." + table + " WHERE " + col + " IN (SELECT " + set + ")"); };
            string B = "BusinessID FROM #eb", E = "EmployeeID FROM #ee", P = "ProjectID FROM #ep", T = "TicketId FROM #et", U = "UserId FROM #eu";
            // Remove scoped accountant/payroll invitations, retaining invitations that still serve another business.
            if (keepLogins && Col(c, tx, "AnyminuteAccessInviteBusiness", "InviteID"))
            {
                Run(c, tx, "IF OBJECT_ID('tempdb..#ei') IS NOT NULL DROP TABLE #ei; CREATE TABLE #ei(InviteID int PRIMARY KEY);");
                Run(c, tx, @"INSERT #ei SELECT i.InviteID FROM dbo.AnyminuteAccessInvite i WHERE (EXISTS(SELECT 1 FROM dbo.AnyminuteAccessInviteBusiness b WHERE b.InviteID=i.InviteID AND b.BusinessID IN (SELECT BusinessID FROM #eb)) OR i.InvitedBy=" + user.ToString(CultureInfo.InvariantCulture) + @") AND NOT EXISTS(SELECT 1 FROM dbo.AnyminuteAccessInviteBusiness b WHERE b.InviteID=i.InviteID AND b.BusinessID NOT IN (SELECT BusinessID FROM #eb))");
                add("Accountant / payroll invitation links", "DELETE FROM dbo.AnyminuteAccessInviteBusiness WHERE InviteID IN (SELECT InviteID FROM #ei)");
                add("Accountant / payroll invitations deleted", "DELETE FROM dbo.AnyminuteAccessInvite WHERE InviteID IN (SELECT InviteID FROM #ei)");
                Run(c, tx, "DROP TABLE #ei;");
            }
            // Time & attendance / approvals / schedules / notifications (per business)
            foreach (var t in new[] { new[] { "TimeSheetEntry", "BussID" }, new[] { "TimeSheet", "BusinessID" }, new[] { "BreakTime", "BussID" }, new[] { "TimeCardStatus", "BusinessID" }, new[] { "AnyminuteWeek", "BusinessID" } })
                del("Timesheet / time-card rows", t[0], t[1], B);
            foreach (var t in new[] { new[] { "AnyminuteActivity", "BusinessID" }, new[] { "AnyminuteApprovalAudit", "BusinessID" }, new[] { "AnyminuteBusinessPolicy", "BusinessID" }, new[] { "AnyminuteClockOutEstimate", "BusinessID" },
                new[] { "AnyminuteNotification", "BusinessID" }, new[] { "AnyminuteWeeklySchedule", "BusinessID" }, new[] { "AnyminuteEmployeeRoleHistory", "BusinessId" }, new[] { "EmpType_Change_History", "BusinessID" },
                new[] { "AssgineDelegates", "BusinessID" }, new[] { "AssgineManager", "BusinessID" }, new[] { "AnyminuteAssignmentDetail", "BusinessID" }, new[] { "AnyminuteAssignmentRole", "BusinessID" },
                new[] { "AnyminuteReportAccess", "BusinessID" }, new[] { "AnyminuteAccessInviteBusiness", "BusinessID" }, new[] { "User_Role_Permission", "BusinessId" } })
                del("Activity, approvals, schedules and permissions", t[0], t[1], B);
            // Projects
            foreach (var t in new[] { new[] { "ASSIGNPROJECT", "BUSINESSID" }, new[] { "AnyminuteProjectAssignment", "BusinessID" }, new[] { "AnyminuteProjectAssignmentLog", "BusinessID" } }) del("Project assignments", t[0], t[1], B);
            foreach (var table in new[] { "AnyminuteProjectAssignmentLog", "AnyminuteProjectAssignment", "ASSIGNPROJECT" }) del("Project assignments", table, "ProjectID", P);
            del("Project assignments", "AnyminuteProjectDetail", "ProjectID", P);
            del("Projects", "AddProject", "ProjectID", P);
            // Tickets (business, erased employees, and the subscriber's own email)
            del("Support tickets", "Ticket_Comments", "TicketId", T); del("Support tickets", "AnyminuteTicketMeta", "TicketId", T); del("Support tickets", "Ticket", "TicketId", T);
            if (Col(c, tx, "AnyminuteAccessLog", "UserId")) del("Employee logins", "AnyminuteAccessLog", "UserId", U);
            if (Col(c, tx, "EmailChangeAudit", "UserId")) del("Employee logins", "EmailChangeAudit", "UserId", U);
            // Employee logins (only employees working solely for this subscriber; never SupportAdmin or subscriber logins)
            foreach (var t in new[] { "AnyminuteLoginSecurity", "AnyminuteActiveSession", "AnyminutePasswordResetToken", "ExternalLogins" }) del("Employee logins", t, "UserId", U);
            if (Col(c, tx, "EmailChangeRequests", "UserId")) del("Employee logins", "EmailChangeRequests", "UserId", U);
            add("Employee logins", "DELETE FROM dbo.MappingTable WHERE USERID IN (SELECT " + U + ")");
            add("Employee logins", "DELETE FROM dbo.UserAcc WHERE UserId IN (SELECT " + U + ")");
            // Employees: DeleteAllDetails pattern for #ee (record, rate, address, image, status); links for every employee in these businesses
            if (Col(c, tx, "AnyminuteRateChangeReason", "RateId")) add("Employees", "DELETE FROM dbo.AnyminuteRateChangeReason WHERE (RateId IN(SELECT Rateid FROM #erate) OR RateId IN(SELECT RateId FROM RateDetails WHERE EmployeeId IN(SELECT EmployeeID FROM #ee))) AND NOT EXISTS(SELECT 1 FROM AddEmployee a WHERE a.Rateid=AnyminuteRateChangeReason.RateId)");
            if (Col(c, tx, "RateDetails", "EmployeeId")) add("Employees", "DELETE FROM dbo.RateDetails WHERE EmployeeId IN(SELECT EmployeeID FROM #ee) AND NOT EXISTS(SELECT 1 FROM AddEmployee a WHERE a.Rateid=RateDetails.RateId)");
            add("Employees", "DELETE r FROM dbo.RateDetails r WHERE r.RateId IN(SELECT Rateid FROM #erate) AND NOT EXISTS(SELECT 1 FROM AddEmployee a WHERE a.Rateid=r.RateId)");
            foreach (var t in new[] { new[] { "EmpImage", "Empid" }, new[] { "Emp_Active_Inactive_Status", "EmpId" }, new[] { "AnyminuteEmployeeRoleHistory", "EmployeeId" } }) del("Employees", t[0], t[1], E);
            if (keepLogins) Run(c, tx, "UPDATE dbo.MappingTable SET BusinessID=0 WHERE UserTypeId=1 AND BusinessID IN (SELECT BusinessID FROM #eb)");
            add("Employees", "DELETE FROM dbo.MappingTable WHERE BusinessID IN (SELECT " + B + ") AND ISNULL(USERID,0)<>" + user.ToString(CultureInfo.InvariantCulture));
            // Owner shell: keep exactly one owner MappingTable row (BusinessID 0, as Add Subscriber writes it) and one owner AssginBussiness row.
            Run(c, tx, @"UPDATE dbo.MappingTable SET BusinessID=0 WHERE USERID=@u AND BusinessID IN (SELECT BusinessID FROM #eb);
                DELETE m FROM dbo.MappingTable m WHERE m.USERID=@u AND m.BusinessID=0 AND EXISTS(SELECT 1 FROM dbo.MappingTable k WHERE k.USERID=m.USERID AND k.UserTypeId=m.UserTypeId AND ISNULL(k.EmployeeID,0)=ISNULL(m.EmployeeID,0) AND k.BusinessID=0 AND k.MappingId<m.MappingId);", "@u", user);
            add("Employees", "DELETE FROM dbo.AssginBussiness WHERE BusinessID IN (SELECT " + B + ")");
            Run(c, tx, "IF NOT EXISTS(SELECT 1 FROM dbo.AssginBussiness WHERE EmployeeID=@a AND UserTypeId=1) INSERT INTO dbo.AssginBussiness(EmployeeID,EmployeeName,UserTypeId) VALUES(@a,@n,1)", "@a", owner, "@n", (ownerFirst ?? "").Length > 50 ? ownerFirst.Substring(0, 50) : (ownerFirst ?? ""));
            int emps = Num(c, tx, "SELECT COUNT(*) FROM #ee WHERE EmployeeID NOT IN (SELECT EmployeeId FROM UserAcc WHERE EmployeeId IS NOT NULL)");
            add("Employees", "DELETE FROM dbo.AddEmployee WHERE EmployeeID IN (SELECT " + E + ") AND EmployeeID<>" + owner.ToString(CultureInfo.InvariantCulture) + " AND EmployeeID NOT IN (SELECT EmployeeId FROM UserAcc WHERE EmployeeId IS NOT NULL)");
            add("Addresses", "DELETE a FROM dbo.EmpAddress a WHERE a.EmpAddressId IN(SELECT ADDRESSID FROM #eaddr) AND NOT EXISTS(SELECT 1 FROM AddEmployee e WHERE e.ADDRESSID=a.EmpAddressId)");
            // Address verification queue rows submitted for these businesses / employees (unverified only), then the businesses.
            if (AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAddressReview"))
            {
                add("Addresses", "DELETE FROM dbo.AnyminuteAddressReview WHERE AddressMasterId IN (SELECT AddressMasterId FROM #ea)");
                add("Addresses", "DELETE FROM dbo.AddressMaster WHERE ISNULL(Verifed,0)=0 AND AddressMasterId IN (SELECT AddressMasterId FROM #ea)");
            }
            int biz = add("Businesses", "DELETE FROM dbo.AddBusiness WHERE BusinessID IN (SELECT " + B + ")");
            add("Addresses", "DELETE a FROM dbo.BusinessAddress a WHERE a.BusinessAddressId IN(SELECT ADDRESSID FROM #baddr) AND NOT EXISTS(SELECT 1 FROM AddBusiness b WHERE b.ADDRESSID=a.BusinessAddressId)");
            Run(c, tx, "DROP TABLE #erate; DROP TABLE #eaddr; DROP TABLE #baddr;");
            int proj = bucket.ContainsKey("Projects") ? bucket["Projects"] : 0;
            res.Rows.Add(new KeyValuePair<string, int>("Businesses", biz)); res.Rows.Add(new KeyValuePair<string, int>("Employees erased", emps));
            res.Rows.Add(new KeyValuePair<string, int>("Projects", proj));
            foreach (var k in bucket.Keys.Where(k => k != "Businesses" && k != "Projects")) res.Rows.Add(new KeyValuePair<string, int>(k == "Employees" ? "Employee rows (records, rates, links)" : k, bucket[k]));
            SA.Audit(c, tx, "Login", user.ToString(CultureInfo.InvariantCulture), "All Module Data Erased", null, null, null, res.Summary() + (res.Skipped > 0 ? "; skipped shared-owner businesses " + res.Skipped : ""), reason);
            Drop(c, tx);
            tx.Commit();
        }
        return res;
    }
}
