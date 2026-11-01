using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

// 2026-10-31 v41: "Continue with Google" / "Continue with Microsoft" on the main sign-in screen for every role.
// OpenID Connect authorization-code flow with state + nonce + PKCE (S256). The id_token is taken straight from the provider's
// token endpoint over TLS (OIDC Core 3.1.3.7) and its iss / aud / exp / nonce are checked.
// Permanent User ID model: a provider account is linked to the EXISTING UserAcc.UserId whose login email equals the provider's
// VERIFIED email. No account is ever created here (AnyMinute sign-up needs the subscriber/reseller registration form and Support
// approval), so an unknown email is refused with a pointer to Sign Up / the administrator's invite. Once linked, the link is keyed
// by provider + subject (stored only as a SHA-256 hash) and follows the UserId even if the login email changes later.
// No provider access/refresh tokens are stored. Secrets come from appSettings only. Table: DB_OPTIONAL/OPTIONAL_05_ExternalLogins.sql.
public static class AnyminuteExternalLogin
{
    public const string Google = "Google", Microsoft = "Microsoft";
    public const string MissingTable = "Google / Microsoft sign-in is not enabled yet (database update DB_OPTIONAL/OPTIONAL_05_ExternalLogins.sql has not been run). Please sign in with your email and password.";
    const string MsaTenant = "9188040d-6c67-4c5b-b112-36a304b66dad"; // personal Microsoft accounts (emails are verified by Microsoft)
    const string StateKey = "am_ext_state";
    public const string TicketKey = "am_ext_signin", ErrorKey = "am_ext_error";

    static string Cfg(string k) { return (ConfigurationManager.AppSettings[k] ?? "").Trim(); }
    public static string Normalize(string p)
    {
        p = (p ?? "").Trim().ToLowerInvariant();
        return p == "google" ? Google : (p == "microsoft" ? Microsoft : null);
    }
    public static bool Configured(string provider)
    {
        provider = Normalize(provider);
        return provider != null && Cfg(provider + ":ClientId").Length > 0 && Cfg(provider + ":ClientSecret").Length > 0;
    }
    static string Tenant() { string t = Cfg("Microsoft:Tenant"); return t.Length == 0 ? "common" : t; }
    static string AuthorizeUrl(string p) { return p == Google ? "https://accounts.google.com/o/oauth2/v2/auth" : "https://login.microsoftonline.com/" + Uri.EscapeDataString(Tenant()) + "/oauth2/v2.0/authorize"; }
    static string TokenUrl(string p) { return p == Google ? "https://oauth2.googleapis.com/token" : "https://login.microsoftonline.com/" + Uri.EscapeDataString(Tenant()) + "/oauth2/v2.0/token"; }
    public static string RedirectUri(string p)
    {
        string r = Cfg(p + ":RedirectUri");
        if (r.Length > 0) return r;
        var req = HttpContext.Current.Request;
        return req.Url.GetLeftPart(UriPartial.Authority) + VirtualPathUtility.ToAbsolute("~/ExternalLogin.aspx");
    }

    static string Random(int n) { var b = new byte[n]; using (var rng = new RNGCryptoServiceProvider()) rng.GetBytes(b); return B64Url(b); }
    static string B64Url(byte[] b) { return Convert.ToBase64String(b).TrimEnd('=').Replace('+', '-').Replace('/', '_'); }
    static byte[] FromB64Url(string s)
    {
        s = (s ?? "").Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4) { case 2: s += "=="; break; case 3: s += "="; break; }
        return Convert.FromBase64String(s);
    }
    static string Sha256Hex(string s)
    {
        using (var sha = SHA256.Create()) { var sb = new StringBuilder(); foreach (var x in sha.ComputeHash(Encoding.UTF8.GetBytes(s))) sb.Append(x.ToString("x2")); return sb.ToString(); }
    }
    static string SubjectHash(string provider, string subject) { return Sha256Hex(provider + "|" + subject); }

    // Step 1: build the provider URL and remember state / nonce / PKCE verifier in the server session (single use).
    public static string Start(string provider)
    {
        provider = Normalize(provider);
        if (provider == null || !Configured(provider)) return null;
        string state = Random(32), nonce = Random(32), verifier = Random(48);
        string challenge; using (var sha = SHA256.Create()) challenge = B64Url(sha.ComputeHash(Encoding.ASCII.GetBytes(verifier)));
        HttpContext.Current.Session[StateKey] = provider + "|" + state + "|" + nonce + "|" + verifier + "|" + DateTime.UtcNow.Ticks;
        var q = new StringBuilder(AuthorizeUrl(provider));
        q.Append("?response_type=code&client_id=").Append(Uri.EscapeDataString(Cfg(provider + ":ClientId")))
         .Append("&redirect_uri=").Append(Uri.EscapeDataString(RedirectUri(provider)))
         .Append("&scope=").Append(Uri.EscapeDataString("openid email profile"))
         .Append("&state=").Append(state).Append("&nonce=").Append(nonce)
         .Append("&code_challenge=").Append(challenge).Append("&code_challenge_method=S256")
         .Append("&prompt=select_account");
        if (provider == Microsoft) q.Append("&response_mode=query");
        return q.ToString();
    }

    // Step 2: callback. Returns the UserId to sign in, or 0 with a user-facing message in error.
    public static int Complete(HttpRequest req, out string provider, out string error)
    {
        provider = null; error = null;
        var s = HttpContext.Current.Session;
        string saved = s[StateKey] as string; s.Remove(StateKey);
        string state = req.QueryString["state"] ?? "";
        if (saved == null) { error = "Your sign-in session expired. Please try again."; return 0; }
        var parts = saved.Split('|');
        provider = parts[0];
        if (parts.Length != 5 || state.Length == 0 || !FixedEquals(parts[1], state)) { error = "The sign-in request could not be verified. Please try again."; return 0; }
        if (new DateTime(Convert.ToInt64(parts[4]), DateTimeKind.Utc) < DateTime.UtcNow.AddMinutes(-15)) { error = "Your sign-in session expired. Please try again."; return 0; }
        if (!String.IsNullOrEmpty(req.QueryString["error"])) { error = provider + " sign-in was cancelled or not allowed."; return 0; }
        string code = req.QueryString["code"] ?? "";
        if (code.Length == 0) { error = provider + " sign-in did not return a code. Please try again."; return 0; }

        Dictionary<string, object> claims;
        try { claims = Exchange(provider, code, parts[3]); }
        catch (Exception ex) { Log("token exchange failed: " + ex.Message); error = "Could not complete " + provider + " sign-in. Please try again."; return 0; }
        string why = Validate(provider, claims, parts[2]);
        if (why != null) { Log("id_token rejected: " + why); error = "Could not verify your " + provider + " account. Please try again."; return 0; }

        string subject = Str(claims, "sub"), email; bool verified;
        ReadEmail(provider, claims, out email, out verified);
        return Resolve(provider, subject, email, verified, out error);
    }

    static bool FixedEquals(string a, string b)
    {
        if (a == null || b == null || a.Length != b.Length) return false;
        int d = 0; for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i]; return d == 0;
    }
    static string Str(Dictionary<string, object> c, string k) { object v; return c.TryGetValue(k, out v) && v != null ? Convert.ToString(v) : ""; }
    static bool Bool(Dictionary<string, object> c, string k) { object v; if (!c.TryGetValue(k, out v) || v == null) return false; if (v is bool) return (bool)v; return String.Equals(Convert.ToString(v), "true", StringComparison.OrdinalIgnoreCase); }
    static void Log(string m) { try { log4net.LogManager.GetLogger("Log").Warn("AnyminuteExternalLogin: " + m); } catch { } }

    static Dictionary<string, object> Exchange(string provider, string code, string verifier)
    {
        ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        string body = "grant_type=authorization_code&code=" + Uri.EscapeDataString(code)
            + "&redirect_uri=" + Uri.EscapeDataString(RedirectUri(provider))
            + "&client_id=" + Uri.EscapeDataString(Cfg(provider + ":ClientId"))
            + "&client_secret=" + Uri.EscapeDataString(Cfg(provider + ":ClientSecret"))
            + "&code_verifier=" + Uri.EscapeDataString(verifier);
        if (provider == Microsoft) body += "&scope=" + Uri.EscapeDataString("openid email profile");
        var w = (HttpWebRequest)WebRequest.Create(TokenUrl(provider));
        w.Method = "POST"; w.ContentType = "application/x-www-form-urlencoded"; w.Accept = "application/json"; w.Timeout = 15000;
        var bytes = Encoding.UTF8.GetBytes(body);
        using (var st = w.GetRequestStream()) st.Write(bytes, 0, bytes.Length);
        string json;
        try { using (var r = (HttpWebResponse)w.GetResponse()) using (var rd = new StreamReader(r.GetResponseStream())) json = rd.ReadToEnd(); }
        catch (WebException we)
        {
            string detail = "";
            if (we.Response != null) using (var rd = new StreamReader(we.Response.GetResponseStream())) detail = rd.ReadToEnd();
            throw new InvalidOperationException(we.Message + " " + (detail.Length > 300 ? detail.Substring(0, 300) : detail));
        }
        var js = new JavaScriptSerializer();
        var tok = js.Deserialize<Dictionary<string, object>>(json);
        string idt = Str(tok, "id_token");
        var seg = idt.Split('.');
        if (seg.Length != 3) throw new InvalidOperationException("no id_token");
        return js.Deserialize<Dictionary<string, object>>(Encoding.UTF8.GetString(FromB64Url(seg[1])));
    }

    static string Validate(string provider, Dictionary<string, object> c, string nonce)
    {
        string aud = Str(c, "aud"), iss = Str(c, "iss");
        if (aud != Cfg(provider + ":ClientId")) return "aud";
        if (!FixedEquals(Str(c, "nonce"), nonce)) return "nonce";
        long exp; if (!Int64.TryParse(Str(c, "exp"), out exp)) return "exp";
        if (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(exp) < DateTime.UtcNow.AddMinutes(-5)) return "expired";
        if (Str(c, "sub").Length == 0) return "sub";
        if (provider == Google) return iss == "https://accounts.google.com" || iss == "accounts.google.com" ? null : "iss";
        string tid = Str(c, "tid");
        if (tid.Length == 0 || iss != "https://login.microsoftonline.com/" + tid + "/v2.0") return "iss";
        string t = Tenant();
        if (t != "common" && t != "organizations" && t != "consumers" && !String.Equals(t, tid, StringComparison.OrdinalIgnoreCase)) return "tenant";
        return null;
    }

    static void ReadEmail(string provider, Dictionary<string, object> c, out string email, out bool verified)
    {
        email = Str(c, "email").Trim(); verified = false;
        if (provider == Google) { verified = email.Length > 0 && Bool(c, "email_verified"); return; }
        // Microsoft Entra: the email claim is only trusted when Microsoft vouches for it - personal accounts, the optional
        // xms_edov claim (email domain owner verified), or a tenant listed in Microsoft:TrustedTenantIds (admin-managed directory).
        string tid = Str(c, "tid").ToLowerInvariant();
        bool trustedTenant = false;
        foreach (var x in Cfg("Microsoft:TrustedTenantIds").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries))
            if (String.Equals(x.Trim(), tid, StringComparison.OrdinalIgnoreCase)) trustedTenant = true;
        if (email.Length == 0 && trustedTenant) email = Str(c, "preferred_username").Trim();
        verified = email.Length > 0 && email.Contains("@") && (tid == MsaTenant || Bool(c, "xms_edov") || trustedTenant);
    }

    public static bool Ready(SqlConnection c) { return AnyminuteEmployeeEditor.HasTable(c, null, "ExternalLogins"); }

    static DataTable Raw(SqlConnection c, string sql, params object[] args)
    {
        using (var cmd = AnyminuteTimesheetRepair.Command(c, null, sql, args)) using (var da = new SqlDataAdapter(cmd)) { var dt = new DataTable(); da.Fill(dt); return dt; }
    }

    static int Resolve(string provider, string subject, string email, bool verified, out string error)
    {
        error = null;
        string hash = SubjectHash(provider, subject);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) { error = MissingTable; return 0; }
            var linked = Raw(c, "SELECT L.UserId FROM dbo.ExternalLogins L JOIN dbo.UserAcc U ON U.UserId=L.UserId WHERE L.Provider=@p AND L.SubjectHash=@h", "@p", provider, "@h", hash);
            if (linked.Rows.Count == 1)
            {
                int u = Convert.ToInt32(linked.Rows[0][0]);
                AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.ExternalLogins SET LastUsedOn=SYSUTCDATETIME() WHERE Provider=@p AND SubjectHash=@h", "@p", provider, "@h", hash);
                return u;
            }
            if (!verified) { error = "Your " + provider + " account did not provide a verified email address, so it cannot be matched to an AnyMinute login. Please sign in with your email and password."; return 0; }
            var users = Raw(c, "SELECT UserId FROM dbo.UserAcc WITH (NOLOCK) WHERE LOWER(LTRIM(RTRIM(Email)))=LOWER(@e)", "@e", email);
            if (users.Rows.Count == 0) { error = "No AnyMinute account uses " + email + ". Sign in with your email and password, use Sign Up, or ask your administrator to invite you with this email."; return 0; }
            if (users.Rows.Count > 1) { error = "More than one AnyMinute login uses " + email + ". Please contact AnyMinute Support."; return 0; }
            int user = Convert.ToInt32(users.Rows[0][0]);
            if (Raw(c, "SELECT 1 FROM dbo.ExternalLogins WHERE UserId=@u AND Provider=@p", "@u", user, "@p", provider).Rows.Count > 0)
            { error = "A different " + provider + " account is already connected to this AnyMinute login. Sign in with your password and disconnect it under My Profile > Login & Security first."; return 0; }
            try
            {
                AnyminuteTimesheetRepair.Execute(c, null, "INSERT dbo.ExternalLogins(UserId,Provider,SubjectHash,EmailAtLink,LinkedOn,LastUsedOn) VALUES(@u,@p,@h,@e,SYSUTCDATETIME(),SYSUTCDATETIME())",
                    "@u", user, "@p", provider, "@h", hash, "@e", email.Length > 256 ? email.Substring(0, 256) : email);
            }
            catch (SqlException) { error = "Could not connect your " + provider + " account. Please try again."; return 0; }
            try
            {
                if (AnyminuteTimesheetRepair.Query(c, null, "SELECT COL_LENGTH('dbo.UserAcc','EmailVerified')").Rows[0][0] != DBNull.Value)
                    AnyminuteTimesheetRepair.Execute(c, null, "UPDATE dbo.UserAcc SET EmailVerified=1 WHERE UserId=@u AND ISNULL(EmailVerified,0)=0", "@u", user);
            }
            catch (SqlException) { }
            AnyminuteSupportAdmin.LoginEvent(email, user.ToString(), true, provider + " account connected (verified email matched login email).");
            return user;
        }
    }

    // The same columns SP_LOGIN returns, so Login.aspx runs the identical session / role-routing code as password sign-in.
    public static DataTable LoginRow(int user)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
            return Raw(c, "SELECT UserId,EmployeeId,Email,MotherOrgid,UserType,Password,UID,UserTypeid,Is_First_Time_Login FROM UserAcc WITH (NOLOCK) WHERE UserId=@u", "@u", user);
    }

    // Login & Security: own connected accounts only (UserId from the session).
    public static DataTable Connected(int user, out bool ready)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            ready = Ready(c);
            if (!ready) return new DataTable();
            return Raw(c, "SELECT Provider,EmailAtLink,LinkedOn,LastUsedOn FROM dbo.ExternalLogins WHERE UserId=@u ORDER BY Provider", "@u", user);
        }
    }
    public static void Disconnect(int user, string provider)
    {
        provider = Normalize(provider); if (provider == null) return;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Ready(c)) return;
            AnyminuteTimesheetRepair.Execute(c, null, "DELETE dbo.ExternalLogins WHERE UserId=@u AND Provider=@p", "@u", user, "@p", provider);
        }
        AnyminuteSupportAdmin.LoginEvent(Convert.ToString(HttpContext.Current.Session["email"]), user.ToString(), true, provider + " account disconnected from Login & Security.");
    }
}
