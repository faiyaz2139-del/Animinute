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
    // v44 reseller sign-in fix. Login.aspx signs in only through SP_LOGIN, which reads UserAcc and compares
    // DecryptByPassPhrase('MD2', UserAcc.Password). The reseller row (AnyminuteReseller) only held a SHA-256 hash, so the
    // emailed email + password could never sign in. A reseller now gets the same login a legacy approved reseller gets
    // (SuperAdmin_Approve_Reseller: Sp_InsertMotherOrg -> MotherOrg + UserAcc UserTypeid 0 'Admin'), and the temporary
    // password is written to UserAcc.Password in the SP_LOGIN format. Sign-in lands on Dashboard.aspx like other MotherOrg logins.
    public const int MaxLoginEmail = 50, MaxPassword = 50; // SP_LOGIN / Sp_InsertMotherOrg take nvarchar(50)
    public static int LoginUser(SqlConnection c, string login)
    {
        var t = SA.Q(c, "SELECT TOP 1 u.UserId FROM UserAcc u JOIN MotherOrg m ON m.MotherOrgID=u.MotherOrgid AND LTRIM(RTRIM(m.Email))=LTRIM(RTRIM(u.Email)) WHERE LTRIM(RTRIM(u.Email))=@l AND u.UserTypeid=0 AND u.UserType='Admin' ORDER BY u.UserId", "@l", (login ?? "").Trim());
        return t.Rows.Count == 1 ? Convert.ToInt32(t.Rows[0][0]) : 0;
    }
    // True when the login email already belongs to a different AnyMinute login (not this reseller's own).
    public static bool LoginTaken(SqlConnection c, string login)
    {
        return LoginUser(c, login) == 0 && SA.Q(c, "SELECT 1 FROM UserAcc WHERE LTRIM(RTRIM(Email))=@l", "@l", (login ?? "").Trim()).Rows.Count > 0;
    }
    // Creates the reseller's UserAcc login when missing and sets its password (null password = keep it, only sync access).
    // Returns null on success, otherwise why the sign-in could not be set up.
    public static string EnsureLogin(SqlConnection c, DataRow r, string password)
    {
        string login = SA.S(r["LoginEmail"]).Trim();
        if (login.Length == 0 || login.Length > MaxLoginEmail) return "The login email must be 1 to " + MaxLoginEmail + " characters to sign in.";
        if (password != null && (password.Length < 8 || password.Length > MaxPassword)) return "The temporary password must be 8 to " + MaxPassword + " characters.";
        int user = LoginUser(c, login);
        if (user == 0)
        {
            if (LoginTaken(c, login)) return "This login email is already registered to another AnyMinute account.";
            if (password == null) return "This reseller has no sign-in yet. Use Send Password Reset to create it.";
            string name = SA.S(r["ResellerName"]) + "-" + Rsl(r["ResellerID"]);
            new MotherOrgControllerClass().InsertMotherOrg(new MotherOrgEntityClass
            {
                MotherOrgName = name.Length > 100 ? name.Substring(0, 100) : name, StreetNumber = "profile.jpg", StreetName = "", Apartment = "", City = "", Province = "", PostalCode = "",
                EnrollDate = DateTime.Today, Status = true, Email = login, ContactName = Cut(SA.S(r["ContactPerson"]), 50), ContactNumber = Cut(SA.S(r["Phone"]), 50), Checked = "N"
            });
            user = LoginUser(c, login);
            if (user == 0) return "The reseller sign-in could not be created.";
        }
        if (password != null)
            SA.X(c, null, "UPDATE UserAcc SET Password=CONVERT(nvarchar(max),EncryptByPassPhrase('MD2',CAST(@p AS nvarchar(50))),2),Is_First_Time_Login=0 WHERE UserId=@u", "@p", password, "@u", user);
        SyncAccess(c, r, user);
        return null;
    }
    // Inactive reseller or Login Disabled = cannot sign in (Login.aspx checks AnyminuteLoginSecurity.LoginDisabled).
    public static void SyncAccess(SqlConnection c, DataRow r, int user)
    {
        if (user <= 0) user = LoginUser(c, SA.S(r["LoginEmail"]));
        if (user <= 0 || !SA.Has(c, "AnyminuteLoginSecurity")) return;
        bool off = Convert.ToBoolean(r["LoginDisabled"]) || SA.S(r["Status"]) != "Active";
        SA.X(c, null, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u) INSERT AnyminuteLoginSecurity(UserId,LoginDisabled,RequirePasswordChange,TwoFactorEnabled,UpdatedAtUTC,UpdatedBy) VALUES(@u,@d,0,0,SYSUTCDATETIME(),@a)
            ELSE UPDATE AnyminuteLoginSecurity SET LoginDisabled=@d,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE UserId=@u", "@u", user, "@d", off, "@a", SA.ActorId());
    }
    static string Cut(string s, int n) { return s.Length > n ? s.Substring(0, n) : s; }
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
