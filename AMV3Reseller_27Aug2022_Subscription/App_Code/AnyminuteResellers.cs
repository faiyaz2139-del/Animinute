using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SA = AnyminuteSupportAdmin;

// 2026-10-29 v28 (Shamsu deck 2): reseller data for Support Admin > Resellers and the 8 Reseller reports.
// Tables come from DB_REQUIRED/REQUIRED_09_Resellers.sql (AnyminuteReseller, AnyminuteResellerSubscriber); without them
// everything here returns empty results and Ready() is false. Subscriber = UserAcc row mapped as owner (MappingTable UserTypeId 1),
// businesses = active AddBusiness rows the subscriber owns (AssginBussiness UserTypeId 1).
public static class AnyminuteResellers
{
    public const string Missing = "Database update needed: run DB_REQUIRED/REQUIRED_09_Resellers.sql to add and manage resellers.";
    public static readonly string[] Plans = { "Basic", "Standard", "Professional", "Premium" };
    public static readonly string[][] PermissionList = {
        new[] { "subscribers", "Create and manage subscribers", "1" }, new[] { "businesses", "Create and manage businesses (under their subscribers)", "1" },
        new[] { "plans", "View subscription plans", "1" }, new[] { "usage", "View usage and limits (for their subscribers)", "1" },
        new[] { "reports", "View reports (for their subscribers)", "1" }, new[] { "upgrades", "Allow subscription upgrades/downgrades", "0" },
        new[] { "api", "Allow API access (if applicable)", "0" } };
    public static bool Ready(SqlConnection c) { return SA.Has(c, "AnyminuteReseller") && SA.Has(c, "AnyminuteResellerSubscriber"); }
    public static string Rsl(object id) { return "RSL-" + Convert.ToInt32(id).ToString("000", CultureInfo.InvariantCulture); }
    public static string Sub(object id) { return "SUB-" + Convert.ToInt32(id).ToString("000", CultureInfo.InvariantCulture); }
    // Businesses owned by subscriber u (alias of UserAcc); same rule as SupportSubscribers.
    public const string OwnedBiz = "(SELECT COUNT(DISTINCT b.BusinessID) FROM AssginBussiness a JOIN AddBusiness b ON b.BusinessID=a.BusinessID WHERE a.EmployeeID=u.EmployeeId AND a.UserTypeId=1 AND ISNULL(b.Status,'')<>'2')";
    public static DataTable List(SqlConnection c)
    {
        if (!Ready(c)) return null;
        return SA.Q(c, @"SELECT r.*,(SELECT COUNT(*) FROM AnyminuteResellerSubscriber s WHERE s.ResellerID=r.ResellerID) Subscribers,
            (SELECT COUNT(DISTINCT b.BusinessID) FROM AnyminuteResellerSubscriber s JOIN UserAcc u ON u.UserId=s.UserId JOIN AssginBussiness a ON a.EmployeeID=u.EmployeeId AND a.UserTypeId=1
              JOIN AddBusiness b ON b.BusinessID=a.BusinessID AND ISNULL(b.Status,'')<>'2' WHERE s.ResellerID=r.ResellerID) Businesses
            FROM AnyminuteReseller r ORDER BY r.CreatedAtUTC DESC,r.ResellerID DESC");
    }
    // Subscribers with their reseller (if any) - used by View and the reseller subscriber/business/performance reports.
    public static DataTable Subscribers(SqlConnection c, int reseller)
    {
        if (!Ready(c)) return null;
        return SA.Q(c, @"SELECT u.UserId,u.EmployeeId,u.Email,u.Status,u.Is_First_Time_Login,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name,
            (SELECT TOP 1 p.PlanName FROM PlanHistory p WHERE p.EmailId=u.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) PlanName,
            (SELECT MIN(p.StartDate) FROM PlanHistory p WHERE p.EmailId=u.Email) Since," + OwnedBiz + @" Businesses,
            s.ResellerID,s.AssignedAtUTC,r.ResellerName
            FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId LEFT JOIN AnyminuteResellerSubscriber s ON s.UserId=u.UserId LEFT JOIN AnyminuteReseller r ON r.ResellerID=s.ResellerID
            WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)" + (reseller > 0 ? " AND s.ResellerID=@r" : reseller < 0 ? " AND s.ResellerID IS NOT NULL" : "") + " ORDER BY u.UserId DESC", "@r", reseller);
    }
    public static string NewTempPassword()
    {
        const string up = "ABCDEFGHJKLMNPQRSTUVWXYZ", lo = "abcdefghijkmnpqrstuvwxyz", dg = "23456789", sy = "@#$%!";
        var b = new byte[12]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
        var sb = new StringBuilder(); sb.Append(up[b[0] % up.Length]).Append(lo[b[1] % lo.Length]).Append(sy[b[2] % sy.Length]);
        for (int i = 3; i < 10; i++) { string all = up + lo + dg; sb.Append(all[b[i] % all.Length]); }
        sb.Append(dg[b[10] % dg.Length]).Append(dg[b[11] % dg.Length]);
        return sb.ToString();
    }
    public static string Salt() { var b = new byte[16]; using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b); return BitConverter.ToString(b).Replace("-", "").ToLowerInvariant(); }
    public static string Hash(string salt, string password)
    {
        using (var sha = SHA256.Create()) { var sb = new StringBuilder(); foreach (var x in sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password))) sb.Append(x.ToString("x2")); return sb.ToString(); }
    }
    public static string PermissionText(string csv)
    {
        var on = new HashSet<string>((csv ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()), StringComparer.OrdinalIgnoreCase);
        return String.Join("; ", PermissionList.Where(p => on.Contains(p[0])).Select(p => p[1]).ToArray());
    }
}
