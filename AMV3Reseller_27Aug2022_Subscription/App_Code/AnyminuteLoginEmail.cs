using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// 2026-10-31 v39 (Change Login Email DOC + PPT "One Login - Multiple Businesses"): self-service Change Login Email for every
// role. The permanent User ID is UserAcc.UserId; business assignments (MappingTable / AssginBussiness), roles, pay rates and
// timesheets are keyed by UserId / EmployeeID and are never touched. Only UserAcc.Email (the global login email) changes,
// and only after the one-time link sent to the NEW address is opened. Business contact emails (EmpAddress, MotherOrg,
// SupportAdmin) are never written here, and nothing here writes UserAcc.Email for another user.
// Tables: DB_OPTIONAL/OPTIONAL_04_ChangeLoginEmail.sql (EmailChangeRequests, EmailChangeAudit, UserAcc.EmailVerified/UpdatedOn).
public static class AnyminuteLoginEmail
{
    public const int Hours = 24;
    public const int MaxRequestsPerHour = 3;
    public const int MaxPasswordFailures = 5; // per 15 minutes
    public const string MissingTables = "Database update needed: run DB_OPTIONAL/OPTIONAL_04_ChangeLoginEmail.sql to enable Change Login Email.";
    static readonly Regex EmailFormat = new Regex(@"^[A-Za-z0-9._%+\-']+@[A-Za-z0-9.\-]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

    public sealed class Info { public int UserId; public string Name, Email, Status, PendingEmail; public DateTime? PendingExpiresUtc; public bool Verified, Ready; }

    public static bool Ready(SqlConnection c)
    {
        return AnyminuteEmployeeEditor.HasTable(c, null, "EmailChangeRequests") && AnyminuteEmployeeEditor.HasTable(c, null, "EmailChangeAudit")
            && AnyminuteTimesheetRepair.Query(c, null, "SELECT COL_LENGTH('dbo.UserAcc','EmailVerified')").Rows[0][0] != DBNull.Value;
    }
    static string Hash(string token)
    {
        using (var sha = SHA256.Create())
        {
            var sb = new StringBuilder(); foreach (var b in sha.ComputeHash(Encoding.UTF8.GetBytes(token ?? ""))) sb.Append(b.ToString("x2")); return sb.ToString();
        }
    }
    static string NewToken()
    {
        var bytes = new byte[32]; using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }
    static string Ip() { var h = HttpContext.Current; return h == null ? "" : (h.Request.UserHostAddress ?? ""); }
    public static string Normalize(string email) { return (email ?? "").Trim(); }
    public static bool ValidFormat(string email) { email = Normalize(email); return email.Length > 0 && email.Length <= 50 && EmailFormat.IsMatch(email); }

    static void Audit(SqlConnection c, SqlTransaction tx, int user, string prev, string next, int by, string method, string result)
    {
        // Never stores passwords or raw tokens.
        AnyminuteTimesheetRepair.Execute(c, tx, @"INSERT dbo.EmailChangeAudit(UserID,PreviousEmail,NewEmail,ChangedBy,ChangedOn,VerificationMethod,IPAddress,Result)
            VALUES(@u,@p,@n,@b,SYSUTCDATETIME(),@m,@ip,@r)", "@u", user, "@p", prev ?? "", "@n", next ?? "", "@b", by, "@m", Cut(method, 40), "@ip", Cut(Ip(), 45), "@r", Cut(result, 60));
    }
    static string Cut(string s, int n) { s = s ?? ""; return s.Length > n ? s.Substring(0, n) : s; }

    public static Info Load(int user)
    {
        var i = new Info { UserId = user, Name = "", Email = "", Status = "Not verified" };
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            i.Ready = Ready(c);
            var r = AnyminuteTimesheetRepair.Query(c, null, "SELECT Email,EmployeeId" + (i.Ready ? ",ISNULL(EmailVerified,0) AS V" : ",CAST(0 AS bit) AS V") + " FROM UserAcc WHERE UserId=@u", "@u", user);
            if (r.Rows.Count != 1) return i;
            i.Email = Convert.ToString(r.Rows[0]["Email"]).Trim(); i.Verified = Convert.ToBoolean(r.Rows[0]["V"]);
            int emp; Int32.TryParse(Convert.ToString(r.Rows[0]["EmployeeId"]), out emp);
            try { if (emp > 0) i.Name = Convert.ToString(AnyminuteTimesheetRepair.Query(c, null, "SELECT dbo.FN_DESC('AddEmployee',@e)", "@e", emp).Rows[0][0]).Trim(); }
            catch (SqlException) { }
            if (i.Name.Length == 0) i.Name = AnyminuteDisplayText.Tidy(Convert.ToString(HttpContext.Current.Session["user"]).Replace("Welcome!", "").Trim());
            i.Status = i.Verified ? "Verified" : "Not verified";
            if (i.Ready)
            {
                Expire(c);
                var p = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 NewEmail,ExpiresOn FROM dbo.EmailChangeRequests WHERE UserID=@u AND Status='Pending' ORDER BY RequestID DESC", "@u", user);
                if (p.Rows.Count == 1) { i.PendingEmail = Convert.ToString(p.Rows[0][0]); i.PendingExpiresUtc = Convert.ToDateTime(p.Rows[0][1]); i.Status = "Pending Email Verification"; }
            }
        }
        return i;
    }
    static void Expire(SqlConnection c) { AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Expired' WHERE Status='Pending' AND ExpiresOn<=SYSUTCDATETIME()"); }

    static bool PasswordOk(SqlConnection c, int user, string password)
    {
        return AnyminuteTimesheetRepair.Query(c, null, @"SELECT 1 FROM UserAcc WHERE UserId=@u AND
            convert(nvarchar(max),DecryptByPassPhrase('MD2',convert(varbinary(200),Password,2),2))=@p COLLATE SQL_Latin1_General_CP1_CS_AS", "@u", user, "@p", password ?? "").Rows.Count == 1;
    }
    static bool TakenByOther(SqlConnection c, SqlTransaction tx, int user, string email)
    {
        return AnyminuteTimesheetRepair.Query(c, tx, "SELECT 1 FROM UserAcc WHERE LTRIM(RTRIM(Email))=@n AND UserId<>@u", "@n", email, "@u", user).Rows.Count > 0;
    }

    // Self-service request (signed-in user, own UserId only). Returns null on success, else the message to show.
    public static string Request(int user, string newEmail, string confirm, string password)
    {
        newEmail = Normalize(newEmail); confirm = Normalize(confirm);
        if (newEmail.Length == 0 || confirm.Length == 0) return "Enter the new email address and confirm it.";
        if (!ValidFormat(newEmail)) return "Enter a valid email address (up to 50 characters).";
        if (!String.Equals(newEmail, confirm, StringComparison.OrdinalIgnoreCase)) return "The new email addresses do not match.";
        if (String.IsNullOrEmpty(password)) return "Enter your current password to confirm it is you.";
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return MissingTables;
            var cur = AnyminuteTimesheetRepair.Query(c, null, "SELECT Email FROM UserAcc WHERE UserId=@u", "@u", user);
            if (cur.Rows.Count != 1) return "Your login could not be found. Sign in again.";
            string prev = Convert.ToString(cur.Rows[0][0]).Trim();
            int fails = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(*) FROM dbo.EmailChangeAudit WHERE UserID=@u AND Result='Reauthentication failed' AND ChangedOn>DATEADD(minute,-15,SYSUTCDATETIME())", "@u", user).Rows[0][0]);
            if (fails >= MaxPasswordFailures) return "Too many incorrect password attempts. Try again in 15 minutes.";
            if (!PasswordOk(c, user, password)) { Audit(c, null, user, prev, newEmail, user, "Password", "Reauthentication failed"); return "Your current password is incorrect."; }
            int recent = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(*) FROM dbo.EmailChangeRequests WHERE UserID=@u AND RequestedOn>DATEADD(hour,-1,SYSUTCDATETIME())", "@u", user).Rows[0][0]);
            if (recent >= MaxRequestsPerHour) return "Too many email change requests. Try again in an hour.";
            if (String.Equals(prev, newEmail, StringComparison.OrdinalIgnoreCase)) return "This is already your login email.";
            if (TakenByOther(c, null, user, newEmail)) { Audit(c, null, user, prev, newEmail, user, "Password", "Rejected: email in use"); return "This email address is already registered to another AnyMinute account."; }
            return Issue(c, user, prev, newEmail, user, "Password");
        }
    }

    // Support Admin account recovery: starts the SAME verification (link to the new address). It never writes UserAcc.Email.
    public static string Recovery(int supportActor, string currentEmail, string newEmail, string identityNote)
    {
        if (!AnyminuteSupportAdmin.IsSupport()) return "Only Support Admin can start an account recovery.";
        currentEmail = Normalize(currentEmail); newEmail = Normalize(newEmail); identityNote = (identityNote ?? "").Trim();
        if (identityNote.Length < 10) return "Record how the user's identity was verified (at least 10 characters).";
        if (!ValidFormat(newEmail)) return "Enter a valid new email address (up to 50 characters).";
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return MissingTables;
            var u = AnyminuteTimesheetRepair.Query(c, null, "SELECT UserId,Email FROM UserAcc WHERE LTRIM(RTRIM(Email))=@m", "@m", currentEmail);
            if (u.Rows.Count == 0) return "No AnyMinute login uses that current email.";
            if (u.Rows.Count > 1) return "More than one login uses that email; resolve the duplicate login before recovery.";
            int user = Convert.ToInt32(u.Rows[0][0]); string prev = Convert.ToString(u.Rows[0][1]).Trim();
            if (String.Equals(prev, newEmail, StringComparison.OrdinalIgnoreCase)) return "The new email is the same as the current login email.";
            if (TakenByOther(c, null, user, newEmail)) return "This email address is already registered to another AnyMinute account.";
            int recent = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, "SELECT COUNT(*) FROM dbo.EmailChangeRequests WHERE UserID=@u AND RequestedOn>DATEADD(hour,-1,SYSUTCDATETIME())", "@u", user).Rows[0][0]);
            if (recent >= MaxRequestsPerHour) return "Too many email change requests for this user. Try again in an hour.";
            Audit(c, null, user, prev, newEmail, supportActor, "Support recovery", Cut("Recovery started: " + identityNote, 60));
            string err = Issue(c, user, prev, newEmail, supportActor, "Support recovery");
            if (err == null) Notify(prev, "AnyMinute account recovery started", "<p>AnyMinute Support started an account recovery for your login. A verification link was sent to the new email address you gave Support. Your login email changes only after that link is opened.</p><p>If you did not ask for this, contact AnyMinute Support right away.</p>");
            return err;
        }
    }

    static string Issue(SqlConnection c, int user, string prev, string next, int by, string method)
    {
        string token = NewToken();
        AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Cancelled' WHERE UserID=@u AND Status='Pending'", "@u", user);
        int id = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, @"INSERT dbo.EmailChangeRequests(UserID,PreviousEmail,NewEmail,VerificationTokenHash,RequestedOn,ExpiresOn,Status,RequestedBy,Method)
            VALUES(@u,@p,@n,@h,SYSUTCDATETIME(),DATEADD(hour,@x,SYSUTCDATETIME()),'Pending',@b,@m); SELECT CAST(SCOPE_IDENTITY() AS int)",
            "@u", user, "@p", prev, "@n", next, "@h", Hash(token), "@x", Hours, "@b", by, "@m", method).Rows[0][0]);
        string link = VerifyBase() + "?t=" + token;
        string body = "<p>Hello,</p><p>We received a request to change your AnyMinute login email to this address.</p>"
            + "<p><a href='" + HttpUtility.HtmlAttributeEncode(link) + "'>Verify my new login email</a></p>"
            + "<p>This link expires in " + Hours + " hours and can be used once. Until it is used, your current login email keeps working.</p>"
            + "<p>If you did not request this change, ignore this email. Your login will not change.</p><p>AnyMinute</p>";
        try
        {
            if (!SendEnabled()) throw new InvalidOperationException("IsSendMail is off");
            new EmailSettingController().SendHtmlFormattedEmail(next, "Verify your new AnyMinute login email", body);
        }
        catch (Exception ex)
        {
            log4net.LogManager.GetLogger("Log").Warn("Change Login Email: verification mail not sent for user " + user, ex);
            AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Cancelled' WHERE RequestID=@r", "@r", id);
            Audit(c, null, user, prev, next, by, method, "Verification email failed");
            return "The verification email could not be sent. Your login email has not changed. Please try again later.";
        }
        Audit(c, null, user, prev, next, by, method, "Verification email sent");
        return null;
    }
    static bool SendEnabled() { bool on; return !Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out on) || on; }
    static string VerifyBase()
    {
        string cfg = ConfigurationManager.AppSettings["LoginEmailVerifyUrl"];
        if (!String.IsNullOrEmpty(cfg)) return cfg.Trim();
        var r = HttpContext.Current.Request;
        return r.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/VerifyLoginEmail.aspx");
    }
    static void Notify(string to, string subject, string html)
    {
        try { if (SendEnabled() && ValidFormat(to)) new EmailSettingController().SendHtmlFormattedEmail(to, subject, html); }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("Change Login Email: notice to previous email not sent", ex); }
    }

    public static string Cancel(int user)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return MissingTables;
            var p = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 PreviousEmail,NewEmail FROM dbo.EmailChangeRequests WHERE UserID=@u AND Status='Pending' ORDER BY RequestID DESC", "@u", user);
            AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Cancelled' WHERE UserID=@u AND Status='Pending'", "@u", user);
            if (p.Rows.Count == 1) Audit(c, null, user, Convert.ToString(p.Rows[0][0]), Convert.ToString(p.Rows[0][1]), user, "Password", "Cancelled by user");
        }
        return null;
    }

    // Verification link. Returns null on success (newEmail set), else the message to show.
    public static string Verify(string token, out string newEmail)
    {
        newEmail = null;
        if (String.IsNullOrEmpty(token) || token.Length != 64) return "This verification link is not valid.";
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return MissingTables;
            var r = AnyminuteTimesheetRepair.Query(c, null, "SELECT RequestID,UserID,PreviousEmail,NewEmail,Status,ExpiresOn,Method FROM dbo.EmailChangeRequests WHERE VerificationTokenHash=@h", "@h", Hash(token));
            if (r.Rows.Count != 1) return "This verification link is not valid.";
            var row = r.Rows[0]; int id = Convert.ToInt32(row["RequestID"]), user = Convert.ToInt32(row["UserID"]);
            string prev = Convert.ToString(row["PreviousEmail"]), next = Convert.ToString(row["NewEmail"]), status = Convert.ToString(row["Status"]), method = Convert.ToString(row["Method"]);
            if (status == "Verified") return "This verification link has already been used.";
            if (status != "Pending") return "This verification link is no longer valid. Request a new one from My Profile \u203a Login & Security.";
            if (Convert.ToDateTime(row["ExpiresOn"]) <= DateTime.UtcNow)
            {
                AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Expired' WHERE RequestID=@r AND Status='Pending'", "@r", id);
                Audit(c, null, user, prev, next, user, method, "Link expired");
                return "This verification link has expired. Request a new one from My Profile \u203a Login & Security.";
            }
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                if (TakenByOther(c, tx, user, next))
                {
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE dbo.EmailChangeRequests SET Status='Cancelled' WHERE RequestID=@r", "@r", id);
                    Audit(c, tx, user, prev, next, user, method, "Rejected: email in use");
                    tx.Commit(); return "This email address is already registered to another AnyMinute account. Your login email has not changed.";
                }
                // Same UserId; only the login email changes. Guard: the login email must still be the one the request started from.
                int n = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "UPDATE UserAcc SET Email=@n,EmailVerified=1,UpdatedOn=SYSUTCDATETIME() WHERE UserId=@u AND LTRIM(RTRIM(ISNULL(Email,'')))=@p; SELECT @@ROWCOUNT", "@n", next, "@u", user, "@p", prev.Trim()).Rows[0][0]);
                if (n != 1)
                {
                    tx.Rollback();
                    AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.EmailChangeRequests SET Status='Cancelled' WHERE RequestID=@r", "@r", id);
                    return "Your login email changed since this link was sent. Request a new one from My Profile \u203a Login & Security.";
                }
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE dbo.EmailChangeRequests SET Status='Verified',VerifiedOn=SYSUTCDATETIME() WHERE RequestID=@r", "@r", id);
                Audit(c, tx, user, prev, next, user, method + " + email link", "Success");
                tx.Commit();
            }
            newEmail = next;
            var h = HttpContext.Current;
            if (h != null && h.Session != null && Convert.ToString(h.Session["UserId"]) == user.ToString(CultureInfo.InvariantCulture)) h.Session["email"] = next;
            Notify(prev, "Your AnyMinute login email was changed",
                "<p>Hello,</p><p>The login email for your AnyMinute account was changed from this address to <strong>" + HttpUtility.HtmlEncode(Mask(next)) + "</strong> on "
                + DateTime.UtcNow.ToString("MMM d, yyyy HH:mm", CultureInfo.InvariantCulture) + " UTC.</p><p>Your businesses, roles, timesheets and data are unchanged.</p>"
                + "<p>If you did not make this change, contact AnyMinute Support right away for account recovery.</p><p>AnyMinute</p>");
            return null;
        }
    }
    public static string Mask(string email)
    {
        email = email ?? ""; int at = email.IndexOf('@'); if (at <= 1) return email;
        return email.Substring(0, 1) + new string('*', Math.Min(at - 1, 6)) + email.Substring(at);
    }

    // Recent security history for the signed-in user (own rows only).
    public static DataTable History(int user)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return new DataTable();
            return AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 10 ChangedOn,PreviousEmail,NewEmail,VerificationMethod,Result FROM dbo.EmailChangeAudit WHERE UserID=@u ORDER BY AuditID DESC", "@u", user);
        }
    }
}
