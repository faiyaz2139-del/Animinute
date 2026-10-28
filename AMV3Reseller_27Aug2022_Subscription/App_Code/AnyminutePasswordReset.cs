using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

// 2026-10-28 v26 (Send Password Reset deck, strict): one-time reset links that expire after 24 hours.
// The link is ResetUrl + EmployeeId + "&rt=" + token; only the SHA-256 hash of the token is stored
// (AnyminutePasswordResetToken, REQUIRED_08). Issuing a new link cancels the user's earlier unused links.
// ResetPassord.aspx validates the token, skips the temporary-password step, and consumes the token on success.
public static class AnyminutePasswordReset
{
    public const int Hours = 24;
    public static bool Ready(SqlConnection c) { return AnyminuteEmployeeEditor.HasTable(c, null, "AnyminutePasswordResetToken"); }
    static string Hash(string token)
    {
        using (var sha = SHA256.Create())
        {
            var sb = new StringBuilder(); foreach (var b in sha.ComputeHash(Encoding.UTF8.GetBytes(token ?? ""))) sb.Append(b.ToString("x2")); return sb.ToString();
        }
    }
    public static string Issue(SqlConnection c, SqlTransaction tx, int userId, int employeeId, string email, int createdBy)
    {
        var bytes = new byte[32]; using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(bytes);
        string token = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminutePasswordResetToken SET UsedAtUTC=SYSUTCDATETIME() WHERE UserId=@u AND UsedAtUTC IS NULL", "@u", userId);
        AnyminuteTimesheetRepair.Execute(c, tx, "INSERT AnyminutePasswordResetToken(TokenHash,UserId,EmployeeId,Email,ExpiresAtUTC,CreatedBy) VALUES(@h,@u,@e,@m,DATEADD(hour,@x,SYSUTCDATETIME()),@b)",
            "@h", Hash(token), "@u", userId, "@e", employeeId, "@m", email ?? "", "@x", Hours, "@b", createdBy);
        return token;
    }
    // userId of a live (unused, unexpired) token for this employee id; 0 when invalid.
    public static int Check(string empid, string token)
    {
        int e; if (String.IsNullOrEmpty(token) || token.Length != 64 || !Int32.TryParse(empid, out e)) return 0;
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (!Ready(c)) return 0;
                var t = AnyminuteTimesheetRepair.Query(c, null, "SELECT UserId FROM AnyminutePasswordResetToken WHERE TokenHash=@h AND EmployeeId=@e AND UsedAtUTC IS NULL AND ExpiresAtUTC>SYSUTCDATETIME()", "@h", Hash(token), "@e", e);
                return t.Rows.Count == 1 ? Convert.ToInt32(t.Rows[0][0]) : 0;
            }
        }
        catch (Exception) { return 0; }
    }
    public static void Consume(string token, int userId)
    {
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                AnyminuteTimesheetRepair.Execute(c, null, "UPDATE AnyminutePasswordResetToken SET UsedAtUTC=SYSUTCDATETIME() WHERE TokenHash=@h AND UsedAtUTC IS NULL", "@h", Hash(token));
                if (AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteSupportAudit"))
                    AnyminuteTimesheetRepair.Execute(c, null, @"INSERT AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,Comments,IpAddress,Success)
                        VALUES('Login',@k,'Password Changed',@u,'Subscriber','New password set from the one-time reset link.',@ip,1)",
                        "@k", userId.ToString(System.Globalization.CultureInfo.InvariantCulture), "@u", userId, "@ip", System.Web.HttpContext.Current.Request.UserHostAddress ?? "");
            }
        }
        catch (Exception) { }
    }
}
