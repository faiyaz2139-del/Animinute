using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.Caching;

// 2026-10-28 v26 (Support Admin deck 4, strict): active sessions + "Active sessions will be revoked immediately".
// Login.aspx calls SignedIn(); Global.asax calls Check() on every request after session state is loaded.
// A session is ended when its user has LoginDisabled=1 or SessionsRevokedAtUTC later than the session's sign-in.
// Same-server revocations are seen on the very next request (HttpRuntime.Cache); the database is re-checked at most
// once per minute per session (covers other servers / app pool recycles). Needs REQUIRED_08; without it nothing happens.
public static class AnyminuteSessionGuard
{
    const string LoginKey = "am_login_utc", NextKey = "am_sg_next", UserKey = "am_sg_uid";
    static bool ready; static DateTime checkedAt = DateTime.MinValue; static readonly object gate = new object();
    public static bool Ready(SqlConnection c)
    {
        lock (gate) { if (DateTime.UtcNow - checkedAt < TimeSpan.FromMinutes(10)) return ready; }
        bool ok = AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteActiveSession") && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteLoginSecurity", "SessionsRevokedAtUTC");
        lock (gate) { ready = ok; checkedAt = DateTime.UtcNow; }
        return ok;
    }
    static string Device(HttpRequest req)
    {
        string d = ""; try { d = req.Browser.Browser + " " + req.Browser.MajorVersion + " / " + AnyminuteSupportAdmin.Platform(req.UserAgent); } catch (Exception) { }
        return d.Length > 200 ? d.Substring(0, 200) : d;
    }
    public static void SignedIn(string userId)
    {
        var h = HttpContext.Current; if (h == null || h.Session == null) return;
        h.Session[LoginKey] = DateTime.UtcNow; h.Session[NextKey] = DateTime.UtcNow.AddSeconds(60);
        int u; if (!Int32.TryParse(userId, out u)) return;
        h.Session[UserKey] = u; // the account that signed in (Support "login as" switches Session["UserId"] but not this)
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (!Ready(c)) return;
                AnyminuteTimesheetRepair.Execute(c, null, @"IF EXISTS(SELECT 1 FROM AnyminuteActiveSession WHERE SessionId=@s)
                    UPDATE AnyminuteActiveSession SET UserId=@u,StartedAtUTC=SYSUTCDATETIME(),LastSeenAtUTC=SYSUTCDATETIME(),IpAddress=@ip,Device=@d,EndedAtUTC=NULL,EndReason=NULL WHERE SessionId=@s
                    ELSE INSERT AnyminuteActiveSession(SessionId,UserId,IpAddress,Device) VALUES(@s,@u,@ip,@d)",
                    "@s", h.Session.SessionID, "@u", u, "@ip", h.Request.UserHostAddress ?? "", "@d", Device(h.Request));
            }
        }
        catch (Exception) { }
    }
    // Sign out from the masters: mark this session ended (never throws).
    public static void SignedOut()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null) return;
        try { using (var c = AnyminuteTimesheetRepair.Open()) if (Ready(c)) AnyminuteTimesheetRepair.Execute(c, null, "UPDATE AnyminuteActiveSession SET EndedAtUTC=SYSUTCDATETIME(),EndReason='Signed out' WHERE SessionId=@s AND EndedAtUTC IS NULL", "@s", h.Session.SessionID); }
        catch (Exception) { }
    }
    public static void Check(HttpContext h)
    {
        if (h == null || h.Session == null) return;
        object who = h.Session[UserKey] ?? h.Session["UserId"]; if (who == null) return;
        int u; if (!Int32.TryParse(Convert.ToString(who), out u)) return;
        DateTime login = h.Session[LoginKey] is DateTime ? (DateTime)h.Session[LoginKey] : DateTime.MinValue;
        object rev = HttpRuntime.Cache["am_rev_" + u];
        bool revoked = rev is DateTime && (DateTime)rev > login;
        if (!revoked)
        {
            DateTime next = h.Session[NextKey] is DateTime ? (DateTime)h.Session[NextKey] : DateTime.MinValue;
            if (DateTime.UtcNow < next) return;
            h.Session[NextKey] = DateTime.UtcNow.AddSeconds(60);
            try
            {
                using (var c = AnyminuteTimesheetRepair.Open())
                {
                    if (!Ready(c)) return;
                    var t = AnyminuteTimesheetRepair.Query(c, null, "SELECT LoginDisabled,SessionsRevokedAtUTC FROM AnyminuteLoginSecurity WHERE UserId=@u", "@u", u);
                    if (t.Rows.Count == 1)
                        revoked = Convert.ToBoolean(t.Rows[0]["LoginDisabled"]) || (t.Rows[0]["SessionsRevokedAtUTC"] != DBNull.Value && DateTime.SpecifyKind(Convert.ToDateTime(t.Rows[0]["SessionsRevokedAtUTC"]), DateTimeKind.Utc) > login);
                    if (!revoked)
                        AnyminuteTimesheetRepair.Execute(c, null, @"IF EXISTS(SELECT 1 FROM AnyminuteActiveSession WHERE SessionId=@s)
                            UPDATE AnyminuteActiveSession SET LastSeenAtUTC=SYSUTCDATETIME() WHERE SessionId=@s AND EndedAtUTC IS NULL
                            ELSE INSERT AnyminuteActiveSession(SessionId,UserId,IpAddress,Device) VALUES(@s,@u,@ip,@d)",
                            "@s", h.Session.SessionID, "@u", u, "@ip", h.Request.UserHostAddress ?? "", "@d", Device(h.Request));
                }
            }
            catch (Exception) { return; }
        }
        if (!revoked) return;
        try { using (var c = AnyminuteTimesheetRepair.Open()) AnyminuteTimesheetRepair.Execute(c, null, "UPDATE AnyminuteActiveSession SET EndedAtUTC=ISNULL(EndedAtUTC,SYSUTCDATETIME()),EndReason=ISNULL(EndReason,'Revoked') WHERE SessionId=@s", "@s", h.Session.SessionID); }
        catch (Exception) { }
        h.Session.Clear(); h.Session.Abandon();
        if (h.Request.Path.EndsWith("/Login.aspx", StringComparison.OrdinalIgnoreCase)) return;
        h.Response.Redirect(VirtualPathUtility.ToAbsolute("~/Login.aspx") + "?revoked=1", false);
        h.ApplicationInstance.CompleteRequest();
    }
    // Support Admin "Disable Login Access": end every live session of the user (call inside the save transaction).
    public static int Revoke(SqlConnection c, SqlTransaction tx, int userId, string reason)
    {
        if (!Ready(c)) return -1;
        AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteLoginSecurity SET SessionsRevokedAtUTC=SYSUTCDATETIME() WHERE UserId=@u", "@u", userId);
        var n = AnyminuteTimesheetRepair.Query(c, tx, @"UPDATE AnyminuteActiveSession SET EndedAtUTC=SYSUTCDATETIME(),EndReason=@r WHERE UserId=@u AND EndedAtUTC IS NULL AND LastSeenAtUTC>DATEADD(minute,-@t,SYSUTCDATETIME()); SELECT @@ROWCOUNT",
            "@u", userId, "@r", reason.Length > 100 ? reason.Substring(0, 100) : reason, "@t", Timeout());
        return Convert.ToInt32(n.Rows[0][0]);
    }
    // After the transaction commits: same-server sessions see the revocation on their next request.
    public static void Revoked(int userId) { HttpRuntime.Cache.Insert("am_rev_" + userId, DateTime.UtcNow, null, DateTime.UtcNow.AddHours(12), Cache.NoSlidingExpiration); }
    public static int Timeout() { var h = HttpContext.Current; return h != null && h.Session != null ? h.Session.Timeout : 20; }
    // Live sessions = not ended and seen within the session timeout. Returns -1 when REQUIRED_08 is missing.
    public static DataTable Active(SqlConnection c, int userId)
    {
        if (!Ready(c)) return null;
        return AnyminuteTimesheetRepair.Query(c, null, "SELECT SessionId,StartedAtUTC,LastSeenAtUTC,IpAddress,Device FROM AnyminuteActiveSession WHERE UserId=@u AND EndedAtUTC IS NULL AND LastSeenAtUTC>DATEADD(minute,-@t,SYSUTCDATETIME()) ORDER BY LastSeenAtUTC DESC", "@u", userId, "@t", Timeout());
    }
}
