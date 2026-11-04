using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SA = AnyminuteSupportAdmin;

// 2026-11-04 v61 (t191u "existing test data should be cleaned", pt192u "Fix all"): Support Admin > Subscriber Management >
// Clean Test Data. Bulk wrapper around the v59 per-subscriber AnyminuteSubscriberErase (one transaction per subscriber, so one
// failure never rolls back the others) plus an opt-in delete of the emptied subscriber account shell. Never a system wipe: only
// the selected subscribers, no DROP / TRUNCATE, never Support / Super Admin or reseller logins, never the signed-in admin, and
// subscribers with paid billing (PlanHistory.AmountPaid > 0) are skipped unless the admin ticks the explicit override.
public static class AnyminuteTestDataClean
{
    public const string Word = "CLEAN";
    public const int MaxBatch = 100;
    // Test-looking name / email: test, demo, dummy, fake, sample, qa / temp as a word, throw-away mail domains, example.* domains.
    static readonly Regex Like = new Regex(@"test|demo|dummy|fake|sample|(^|[^a-z])(qa|temp|tmp|asdf|xyz)([^a-z]|$)|@(example\.(com|org|net)|mailinator\.|yopmail\.|guerrillamail\.|tempmail|sharklasers\.|10minutemail\.)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    public static bool TestLike(string name, string email) { return Like.IsMatch(name ?? "") || Like.IsMatch(email ?? ""); }
    static bool Paid(object amount)
    {
        string s = Convert.ToString(amount, CultureInfo.InvariantCulture).Replace("$", "").Replace(",", "").Trim(); decimal d;
        return s.Length > 0 && Decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out d) && d > 0;
    }
    // Emails / employee ids with at least one paid PlanHistory row.
    public static HashSet<int> PaidUsers(SqlConnection c, IEnumerable<DataRow> subs)
    {
        var res = new HashSet<int>();
        foreach (var r in subs)
            foreach (DataRow p in SA.Q(c, "SELECT AmountPaid FROM PlanHistory WHERE EmailId=@m OR EmpId=@e", "@m", SA.S(r["Email"]), "@e", SA.S(r["EmployeeId"])).Rows)
                if (Paid(p[0])) { res.Add(Convert.ToInt32(r["UserId"])); break; }
        return res;
    }
    // Logins never cleaned here: Support / Super Admin accounts and reseller portal logins (AnyminuteReseller.LoginEmail).
    public static HashSet<int> Protected(SqlConnection c, IEnumerable<DataRow> subs)
    {
        var res = new HashSet<int>(); bool rsl = SA.Has(c, "AnyminuteReseller");
        foreach (var r in subs)
        {
            int u = Convert.ToInt32(r["UserId"]);
            var t = SA.Q(c, "SELECT ISNULL(UserType,'') UserType FROM UserAcc WHERE UserId=@u", "@u", u);
            string ut = t.Rows.Count == 1 ? SA.S(t.Rows[0][0]).Trim() : "";
            if (ut.Equals("SupportAdmin", StringComparison.OrdinalIgnoreCase) || ut.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || ut.Equals("Support Admin", StringComparison.OrdinalIgnoreCase)) { res.Add(u); continue; }
            if (rsl && SA.Q(c, "SELECT 1 FROM AnyminuteReseller WHERE LOWER(LTRIM(RTRIM(ISNULL(LoginEmail,''))))=LOWER(LTRIM(RTRIM(@m)))", "@m", SA.S(r["Email"])).Rows.Count > 0) res.Add(u);
        }
        return res;
    }
    static bool Col(SqlConnection c, SqlTransaction tx, string table, string col) { return AnyminuteEmployeeEditor.HasColumn(c, tx, table, col); }
    static int Run(SqlConnection c, SqlTransaction tx, string sql, params object[] args) { using (var cmd = AnyminuteTimesheetRepair.Command(c, tx, sql, args)) { cmd.CommandTimeout = 300; return cmd.ExecuteNonQuery(); } }
    // Opt-in "Also delete the subscriber account" (after the v59 erase): the login (UserAcc), its MappingTable / security / session /
    // reset-token / external-login rows, the reseller link, the owner AddEmployee + EmpAddress + rate / image / status rows, the owner
    // AssginBussiness shell row and the UNPAID PlanHistory rows. Paid PlanHistory rows and AnyminuteSupportAudit stay. One transaction;
    // returns null on success or the reason the account was kept (nothing deleted).
    public static string DeleteAccount(SqlConnection c, int owner, int user, string email, string reason)
    {
        if (Convert.ToInt32(SA.Q(c, "SELECT COUNT(*) FROM AssginBussiness WHERE EmployeeID=@a AND BusinessID IS NOT NULL", "@a", owner).Rows[0][0]) > 0)
            return "the owner is still linked to a business (shared with another subscriber), so the account was kept";
        var plan = SA.Q(c, "SELECT Id,AmountPaid FROM PlanHistory WHERE EmailId=@m OR EmpId=@e", "@m", email, "@e", owner.ToString(CultureInfo.InvariantCulture)).Rows.Cast<DataRow>().ToList();
        var unpaid = plan.Where(p => !Paid(p["AmountPaid"])).Select(p => Convert.ToInt32(p["Id"]).ToString(CultureInfo.InvariantCulture)).ToList();
        using (var tx = c.BeginTransaction())
        {
            try
            {
                var addr = AnyminuteTimesheetRepair.Query(c, tx, "SELECT ADDRESSID,Rateid FROM AddEmployee WHERE EmployeeID=@a", "@a", owner);
                object addrId = addr.Rows.Count == 1 ? addr.Rows[0]["ADDRESSID"] : DBNull.Value, rateId = addr.Rows.Count == 1 && addr.Columns.Contains("Rateid") ? addr.Rows[0]["Rateid"] : DBNull.Value;
                if (AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteResellerSubscriber")) Run(c, tx, "DELETE FROM dbo.AnyminuteResellerSubscriber WHERE UserId=@u", "@u", user);
                foreach (var t in new[] { "AnyminuteLoginSecurity", "AnyminuteActiveSession", "AnyminutePasswordResetToken", "ExternalLogins", "EmailChangeRequests" })
                    if (Col(c, tx, t, "UserId")) Run(c, tx, "DELETE FROM dbo." + t + " WHERE UserId=@u", "@u", user);
                if (unpaid.Count > 0) Run(c, tx, "DELETE FROM dbo.PlanHistory WHERE Id IN (" + String.Join(",", unpaid.ToArray()) + ")");
                Run(c, tx, "DELETE FROM dbo.MappingTable WHERE USERID=@u", "@u", user);
                Run(c, tx, "DELETE FROM dbo.AssginBussiness WHERE EmployeeID=@a AND BusinessID IS NULL", "@a", owner);
                Run(c, tx, "DELETE FROM dbo.UserAcc WHERE UserId=@u", "@u", user);
                if (Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "SELECT COUNT(*) FROM UserAcc WHERE EmployeeId=@a", "@a", owner).Rows[0][0]) == 0)
                {
                    foreach (var t in new[] { new[] { "EmpImage", "Empid" }, new[] { "Emp_Active_Inactive_Status", "EmpId" }, new[] { "RateDetails", "EmployeeId" }, new[] { "AnyminuteEmployeeRoleHistory", "EmployeeId" } })
                        if (Col(c, tx, t[0], t[1])) Run(c, tx, "DELETE FROM dbo." + t[0] + " WHERE " + t[1] + "=@a", "@a", owner);
                    Run(c, tx, "DELETE FROM dbo.AddEmployee WHERE EmployeeID=@a", "@a", owner);
                    if (rateId != DBNull.Value && Col(c, tx, "RateDetails", "RateId")) Run(c, tx, "DELETE FROM dbo.RateDetails WHERE RateId=@r AND NOT EXISTS(SELECT 1 FROM dbo.AddEmployee WHERE Rateid=@r)", "@r", rateId);
                    if (addrId != DBNull.Value) Run(c, tx, "DELETE FROM dbo.EmpAddress WHERE EmpAddressId=@x AND NOT EXISTS(SELECT 1 FROM dbo.AddEmployee WHERE ADDRESSID=@x)", "@x", addrId);
                }
                SA.Audit(c, tx, "Login", user.ToString(CultureInfo.InvariantCulture), "Subscriber Account Deleted", null, null, email, "Login, profile, mapping and " + unpaid.Count + " unpaid plan row(s) deleted" + (plan.Count > unpaid.Count ? "; " + (plan.Count - unpaid.Count) + " paid plan row(s) kept" : ""), "Clean Test Data: " + reason);
                tx.Commit(); return null;
            }
            catch (SqlException ex) { tx.Rollback(); log4net.LogManager.GetLogger("Log").Error("Clean Test Data: account delete failed for user " + user, ex); return "the account could not be deleted and was kept (" + ex.Message + ")"; }
        }
    }
}
