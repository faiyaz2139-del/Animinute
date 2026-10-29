using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

// 2026-10-29 v29 (live v28 smoke: LoginAccess.aspx -> ErrorPages for owner ava and administrator stl3).
// Guarded versions of the Login & Access loaders: one failing business/query no longer fails the whole page.
public static partial class AnyminuteReports
{
    static readonly log4net.ILog SafeLog = log4net.LogManager.GetLogger("Log");

    // Used only when Scope() throws: businesses where the signed-in account is owner (1) or administrator (5).
    public static List<AmRptBiz> AdminScopeFallback()
    {
        var list = new List<AmRptBiz>();
        try
        {
            var h = HttpContext.Current; int u, a;
            if (h == null || h.Session == null || !Int32.TryParse(Convert.ToString(h.Session["UserId"]), out u) || !Int32.TryParse(Convert.ToString(h.Session["EmployeeId"]), out a)) return list;
            using (var c = AnyminuteTimesheetRepair.Open())
                foreach (DataRow r in Q(c, @"SELECT b.BusinessID,MAX(b.CompanyName) CompanyName,MAX(a.EmployeeID) Actor,MIN(a.UserTypeId) Role FROM AddBusiness b JOIN AssginBussiness a ON a.BusinessID=b.BusinessID
                    WHERE b.Status<>2 AND a.UserTypeId IN(1,5) AND ((a.EmployeeID=@a AND a.UserTypeId=1) OR EXISTS
                     (SELECT 1 FROM MappingTable m WHERE m.USERID=@u AND m.BusinessID=a.BusinessID AND m.EmployeeID=a.EmployeeID AND m.UserTypeId=a.UserTypeId))
                    GROUP BY b.BusinessID ORDER BY MAX(b.CompanyName)", "@a", a, "@u", u).Rows)
                    list.Add(new AmRptBiz { Id = I(r["BusinessID"]), Name = S(r["CompanyName"]), Tier = 2, Actor = I(r["Actor"]), Role = I(r["Role"]), Costs = true });
        }
        catch (Exception ex) { SafeLog.Warn("LoginAccess fallback scope failed", ex); }
        return list;
    }

    // Login & Access rows for the given admin businesses. Never throws; problem is a user-facing message or null.
    public static List<string[]> LoginRowsSafe(int business, List<AmRptBiz> admin, out string problem)
    {
        problem = null; var rows = new List<string[]>(); var failed = new List<string>();
        foreach (var b in admin.Where(x => business <= 0 || x.Id == business))
        {
            try
            {
                using (var c = AnyminuteTimesheetRepair.Open())
                {
                    Dictionary<int, Login> logins;
                    try { logins = Logins(c, b); }
                    catch (Exception ex) { SafeLog.Warn("LoginAccess logins query failed for business " + b.Id + "; using basic login status", ex); logins = BasicLogins(c, b); }
                    foreach (DataRow p in People(c, b).Rows)
                    {
                        int e = I(p["EmployeeID"]); Login l; if (!logins.TryGetValue(e, out l)) l = new Login();
                        rows.Add(new[] { e.ToString(Inv), S(p["Name"]), l.Email != "" ? l.Email : S(p["Email"]), b.Name, Tier(I(p["UserTypeId"]), I(p["Manages"]) == 1), S(p["IsActive"]).ToUpperInvariant() == "NO" ? "Inactive" : "Active", l.Status, l.LastLogin == "" ? "Never" : l.LastLogin, l.Mfa == "" ? "\u2014" : l.Mfa, b.Id.ToString(Inv) });
                    }
                }
            }
            catch (Exception ex) { SafeLog.Warn("LoginAccess rows failed for business " + b.Id, ex); failed.Add(b.Name); }
        }
        if (failed.Count > 0) problem = "Login details could not be loaded for: " + String.Join(", ", failed) + ".";
        return rows.OrderBy(x => x[1]).ThenBy(x => x[3]).ToList();
    }

    // Minimal login status from UserAcc/MappingTable only (no security/audit tables).
    static Dictionary<int, Login> BasicLogins(System.Data.SqlClient.SqlConnection c, AmRptBiz b)
    {
        var map = new Dictionary<int, Login>();
        try
        {
            foreach (DataRow r in Q(c, @"SELECT m.EmployeeID,MIN(u.UserId) UserId,MAX(u.Email) Email,MAX(CASE WHEN u.Is_First_Time_Login=1 THEN 1 ELSE 0 END) FirstLogin
                FROM MappingTable m JOIN UserAcc u ON u.UserId=m.USERID WHERE m.BusinessID=@b AND m.EmployeeID IS NOT NULL GROUP BY m.EmployeeID", "@b", b.Id).Rows)
                map[I(r["EmployeeID"])] = new Login { UserId = I(r["UserId"]), Email = S(r["Email"]), Status = I(r["FirstLogin"]) == 1 ? "Invitation Pending" : "Enabled" };
        }
        catch (Exception ex) { SafeLog.Warn("LoginAccess basic logins failed for business " + b.Id, ex); }
        return map;
    }
}
