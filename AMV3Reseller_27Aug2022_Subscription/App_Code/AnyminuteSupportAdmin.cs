using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// 2026-10-28 v25 (Shamsu Support Admin decks 1-4): shared helpers for the Support Admin console pages
// (SupportAddresses, SupportTickets, SupportPlans, SupportSubscribers). Data lives in existing tables
// (AddressMaster, Ticket, Ticket_Comments, UserAcc, PlanHistory, AnyminuteLoginSecurity) plus the additive
// DB_REQUIRED/REQUIRED_07_SupportAdmin.sql tables; without them the pages are read-only with a notice.
public static class AnyminuteSupportAdmin
{
    public const string MissingTables = "Database update needed: run DB_REQUIRED/REQUIRED_07_SupportAdmin.sql to save Support Admin changes and activity history.";
    static HttpContext Ctx { get { return HttpContext.Current; } }
    // v26: the exact SupportAdmin role (not SuperAdmin) gets the trimmed navigation.
    public static bool IsSupportAdminOnly()
    {
        var h = Ctx; if (h == null || h.Session == null) return false;
        string t = Convert.ToString(h.Session["UserType"]).Trim();
        return t.Equals("SupportAdmin", StringComparison.OrdinalIgnoreCase) || t.Equals("Support Admin", StringComparison.OrdinalIgnoreCase);
    }
    public static bool IsSupport()
    {
        var h = Ctx; if (h == null || h.Session == null) return false;
        string t = Convert.ToString(h.Session["UserType"]).Trim();
        return t.Equals("SupportAdmin", StringComparison.OrdinalIgnoreCase) || t.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || t.Equals("Support Admin", StringComparison.OrdinalIgnoreCase);
    }
    public static void Require()
    {
        if (Ctx.Session == null || Ctx.Session["UserId"] == null) { Ctx.Response.Redirect("Login.aspx", true); return; }
        if (!IsSupport()) throw new HttpException(403, "Only Support Admin can open this page.");
    }
    public static int ActorId() { int u; Int32.TryParse(Convert.ToString(Ctx.Session["UserId"]), out u); return u; }
    public static string ActorName()
    {
        string n = Convert.ToString(Ctx.Session["user"]).Trim();
        while (n.StartsWith("Welcome!", StringComparison.OrdinalIgnoreCase)) n = n.Substring(8).TrimStart();
        if (n.Length == 0) n = Convert.ToString(Ctx.Session["email"]).Trim();
        return n.Length == 0 ? "Support Admin" : n;
    }
    public static SqlConnection Open() { return AnyminuteTimesheetRepair.Open(); }
    public static DataTable Q(SqlConnection c, string sql, params object[] args) { return AnyminuteTimesheetRepair.Query(c, null, sql, args); }
    public static void X(SqlConnection c, SqlTransaction tx, string sql, params object[] args) { AnyminuteTimesheetRepair.Execute(c, tx, sql, args); }
    public static bool Has(SqlConnection c, string table) { return AnyminuteEmployeeEditor.HasTable(c, null, table); }
    public static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o, CultureInfo.InvariantCulture).Trim(); }
    public static string H(object o) { return HttpUtility.HtmlEncode(S(o)); }
    public static string A(object o) { return HttpUtility.HtmlAttributeEncode(S(o)); }
    public static string When(object utc)
    {
        if (utc == null || utc == DBNull.Value) return "&mdash;";
        var d = DateTime.SpecifyKind(Convert.ToDateTime(utc, CultureInfo.InvariantCulture), DateTimeKind.Utc).ToLocalTime();
        return d.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) + "<br /><small>" + d.ToString("h:mm tt", CultureInfo.InvariantCulture) + "</small>";
    }
    public static string Day(object d) { return d == null || d == DBNull.Value ? "&mdash;" : Convert.ToDateTime(d, CultureInfo.InvariantCulture).ToString("MMM d, yyyy", CultureInfo.InvariantCulture); }
    public static string Form(string key) { return (Ctx.Request.Form[key] ?? "").Trim(); }
    public static string Get(string key) { return (Ctx.Request.QueryString[key] ?? "").Trim(); }
    public static string Options(IEnumerable<string> values, string selected, string allLabel)
    {
        var sb = new StringBuilder();
        if (allLabel != null) sb.Append("<option value=\"\">").Append(H(allLabel)).Append("</option>");
        foreach (var v in values) sb.Append("<option value=\"").Append(A(v)).Append("\"").Append(String.Equals(v, selected, StringComparison.OrdinalIgnoreCase) ? " selected" : "").Append(">").Append(H(v)).Append("</option>");
        return sb.ToString();
    }
    public static string Pill(string text, string tone) { return "<span class='sa-pill sa-" + tone + "'>" + H(text) + "</span>"; }
    public static void Flash(string message) { Ctx.Session["sa_flash"] = message; }
    public static string TakeFlash()
    {
        string m = Convert.ToString(Ctx.Session["sa_flash"]); Ctx.Session.Remove("sa_flash");
        return String.IsNullOrEmpty(m) ? "" : "<div class='sa-flash' role='status'>" + H(m) + "</div>";
    }
    public static string Notice(SqlConnection c, params string[] tables)
    {
        return tables.All(t => Has(c, t)) ? "" : "<div class='sa-flash sa-warn' role='alert'>" + H(MissingTables) + "</div>";
    }
    public static string Pager(int total, int page, int size, string baseQuery)
    {
        int pages = Math.Max(1, (total + size - 1) / size);
        var sb = new StringBuilder("<div class='sa-pager'><span>Showing " + (total == 0 ? 0 : (page - 1) * size + 1) + " to " + Math.Min(total, page * size) + " of " + total + "</span><nav>");
        for (int i = 1; i <= Math.Min(pages, 10); i++)
            sb.Append("<a href='?").Append(A(baseQuery)).Append("&amp;page=").Append(i).Append("'").Append(i == page ? " class='on' aria-current='page'" : "").Append(">").Append(i).Append("</a>");
        return sb.Append("</nav></div>").ToString();
    }

    // ---- Activity history (AnyminuteSupportAudit) ----
    public static void Audit(SqlConnection c, SqlTransaction tx, string entity, string key, string action, string prevStatus, string newStatus, string prevValue, string newValue, string comments)
    {
        X(c, tx, @"INSERT AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,PrevStatus,NewStatus,PrevValue,NewValue,Comments,IpAddress,Success)
            VALUES(@en,@k,@a,@by,@n,@ps,@ns,@pv,@nv,@cm,@ip,1)", "@en", entity, "@k", key, "@a", action, "@by", ActorId(), "@n", ActorName(),
            "@ps", Null(prevStatus), "@ns", Null(newStatus), "@pv", Null(prevValue), "@nv", Null(newValue), "@cm", Null(comments), "@ip", Ctx.Request.UserHostAddress);
    }
    static object Null(string s) { return String.IsNullOrEmpty(s) ? (object)DBNull.Value : s; }
    public static DataTable History(SqlConnection c, string entity, string key)
    {
        if (!Has(c, "AnyminuteSupportAudit")) return null;
        return Q(c, "SELECT TOP 200 * FROM AnyminuteSupportAudit WHERE Entity=@e AND EntityKey=@k ORDER BY ChangedAtUTC DESC,AuditID DESC", "@e", entity, "@k", key);
    }
    public static string HistoryTable(DataTable t, string emptyText)
    {
        if (t == null) return "<p class='sa-muted'>" + H(MissingTables) + "</p>";
        if (t.Rows.Count == 0) return "<p class='sa-muted'>" + H(emptyText) + "</p>";
        var sb = new StringBuilder("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Changed On</th><th>Action</th><th>Changed By</th><th>Previous Status</th><th>New Status</th><th>Previous / New Value</th><th>Comments</th></tr></thead><tbody>");
        foreach (DataRow r in t.Rows)
        {
            string pv = S(r["PrevValue"]), nv = S(r["NewValue"]);
            sb.Append("<tr><td>").Append(When(r["ChangedAtUTC"])).Append("</td><td><strong>").Append(H(r["Action"])).Append("</strong></td><td>").Append(H(r["ChangedByName"]))
              .Append("</td><td>").Append(H(r["PrevStatus"])).Append("</td><td>").Append(H(r["NewStatus"])).Append("</td><td>")
              .Append(pv.Length + nv.Length == 0 ? "&mdash;" : "<span class='sa-old'>" + H(pv) + "</span> &rarr; " + H(nv)).Append("</td><td>").Append(H(r["Comments"])).Append("</td></tr>");
        }
        return sb.Append("</tbody></table></div>").ToString();
    }

    // ---- Login events (Login.aspx hook; never throws) ----
    public static void LoginEvent(string email, string userId, bool success, string description)
    {
        try
        {
            using (var c = Open())
            {
                if (!Has(c, "AnyminuteSupportAudit")) return;
                int u;
                if (!Int32.TryParse(userId, out u))
                {
                    var t = Q(c, "SELECT TOP 1 UserId FROM UserAcc WHERE Email=@m ORDER BY UserId", "@m", email ?? "");
                    if (t.Rows.Count == 0) return; u = Convert.ToInt32(t.Rows[0][0]);
                }
                var req = Ctx.Request; string device = "";
                try { device = req.Browser.Browser + " " + req.Browser.MajorVersion + " / " + Platform(req.UserAgent); } catch (Exception) { }
                X(c, null, @"INSERT AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,Comments,IpAddress,Device,Success)
                    VALUES('Login',@k,@a,@u,'Subscriber',@d,@ip,@dev,@s)", "@k", u.ToString(CultureInfo.InvariantCulture), "@a", success ? "Sign In" : "Failed Sign In",
                    "@u", u, "@d", description, "@ip", req.UserHostAddress, "@dev", device.Length > 200 ? device.Substring(0, 200) : device, "@s", success);
            }
        }
        catch (Exception) { }
    }
    public static string Platform(string ua)
    {
        ua = ua ?? "";
        if (ua.Contains("Windows NT 10")) return "Windows 10/11"; if (ua.Contains("Windows")) return "Windows";
        if (ua.Contains("iPhone") || ua.Contains("iPad")) return "iOS"; if (ua.Contains("Android")) return "Android";
        if (ua.Contains("Mac OS")) return "macOS"; if (ua.Contains("Linux")) return "Linux"; return "Other";
    }

    // ---- Ticket status / address helpers ----
    public static readonly string[] TicketStatuses = { "Open", "In Progress", "Waiting", "Resolved" };
    public static readonly string[] Priorities = { "High", "Medium", "Low" };
    public static readonly string[] Categories = { "Employees", "Subscription & Billing", "Projects", "Login & Access", "Businesses", "Address Verification", "Reports", "Time Cards", "Other" };
    // Legacy Ticket.StatusType codes (kept so Ticket_List.aspx still reads them): ''=Open, 1=Resolved, 2=In Progress, 3=Pending/Waiting.
    public static string StatusLabel(object code) { string s = S(code); return s == "1" ? "Resolved" : s == "2" ? "In Progress" : s == "3" ? "Waiting" : "Open"; }
    public static string StatusCode(string label) { return label == "Resolved" ? "1" : label == "In Progress" ? "2" : label == "Waiting" ? "3" : ""; }
    public static string StatusTone(string label) { return label == "Resolved" ? "green" : label == "In Progress" ? "blue" : label == "Waiting" ? "amber" : "red"; }
    public static string LegacyCategory(object problemType, object module)
    {
        switch (S(problemType)) { case "1": case "2": return "Login & Access"; case "3": case "4": return "Other"; }
        return S(module) == "1" ? "Time Cards" : "Other";
    }
    public static readonly string[] Countries = { "Canada", "USA", "UK", "India" };
    public static string GuessCountry(string postal)
    {
        postal = (postal ?? "").Trim();
        if (Regex.IsMatch(postal, @"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$")) return "Canada";
        if (Regex.IsMatch(postal, @"^\d{5}(-\d{4})?$")) return "USA";
        if (Regex.IsMatch(postal, @"^\d{6}$")) return "India";
        if (Regex.IsMatch(postal, @"^[A-Za-z]{1,2}\d[A-Za-z\d]?\s?\d[A-Za-z]{2}$")) return "UK";
        return "Canada";
    }
    // Same rules as the client-side Verify Address button (Scripts/AnyminuteSupport.js); the server re-checks on Approve.
    public static string VerifyAddress(string street, string city, string province, string country, string postal)
    {
        if (street.Length < 3) return "Street address is required.";
        if (city.Length < 2) return "City is required.";
        if (province.Length < 2) return "Province / State is required.";
        string p = postal.Trim().ToUpperInvariant();
        string rule = country == "Canada" ? @"^[A-Z]\d[A-Z][ -]?\d[A-Z]\d$" : country == "USA" ? @"^\d{5}(-\d{4})?$" : country == "India" ? @"^\d{6}$" : country == "UK" ? @"^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$" : null;
        if (rule == null) return "Choose a country.";
        if (!Regex.IsMatch(p, rule)) return "Postal / ZIP / PIN code does not match the " + country + " format.";
        return null;
    }
    public static string NormalizePostal(string country, string postal)
    {
        string p = postal.Trim().ToUpperInvariant();
        if (country == "Canada") { p = p.Replace(" ", "").Replace("-", ""); if (p.Length == 6) p = p.Substring(0, 3) + " " + p.Substring(3); }
        return p;
    }

    // ---- Badges for the Support Admin navigation ----
    public static int PendingAddresses()
    {
        try
        {
            using (var c = Open())
                return Convert.ToInt32(Q(c, Has(c, "AnyminuteAddressReview")
                    ? "SELECT COUNT(*) FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE ISNULL(a.Verifed,0)<>1 AND ISNULL(r.Status,'Pending')='Pending'"
                    : "SELECT COUNT(*) FROM AddressMaster WHERE ISNULL(Verifed,0)<>1").Rows[0][0]);
        }
        catch (SqlException) { return 0; }
    }
    public static int OpenTickets()
    {
        try
        {
            using (var c = Open())
                return Convert.ToInt32(Q(c, Has(c, "AnyminuteTicketMeta")
                    ? "SELECT COUNT(*) FROM Ticket t LEFT JOIN AnyminuteTicketMeta m ON m.TicketId=t.TicketId WHERE ISNULL(t.StatusType,'') IN('','0') AND ISNULL(m.Archived,0)=0"
                    : "SELECT COUNT(*) FROM Ticket WHERE ISNULL(StatusType,'') IN('','0')").Rows[0][0]);
        }
        catch (SqlException) { return 0; }
    }
    // Deck nav (decks 1-4): Dashboard, Subscribers, Subscription Plans, Usage & Limits, Support Tickets (badge),
    // Address Verification (badge), Reports, Settings. Legacy links the role already had are kept below these.
    public static string NavHtml(string reportsHref, string settingsHref)
    {
        var items = new List<string[]> {
            new[] { "Dashboard", "~/SuperAdmin.aspx", "" },
            new[] { "Resellers", "~/SupportResellers.aspx", "" }, // v28 deck 2
            new[] { "Subscribers", "~/SupportSubscribers.aspx", "" },
            new[] { "Businesses", "~/SupportBusinesses.aspx", "" }, // v30 deck 2 nav
            new[] { "Subscription Plans", "~/SupportPlans.aspx", "" },
            new[] { "Usage & Limits", "~/SupportPlans.aspx?view=usage", "" },
            new[] { "Support Tickets", "~/SupportTickets.aspx", OpenTickets().ToString(CultureInfo.InvariantCulture) },
            new[] { "Address Verification", "~/SupportAddresses.aspx", PendingAddresses().ToString(CultureInfo.InvariantCulture) } };
        items.Add(new[] { "Account Recovery", "~/SupportAccountRecovery.aspx", "" }); // v39 Change Login Email DOC s8: recovery via verification only
        items.Add(new[] { "Reports", "~/SupportReports.aspx", "" }); // v28: Support Admin Reports hub (12 support + 8 reseller reports); reportsHref kept for the signature
        items.Add(new[] { "Settings", settingsHref ?? "~/SupportAdminProfile.aspx", "" }); // v30: deck nav always ends with Settings (profile when no email-settings permission)
        var html = new StringBuilder();
        foreach (var i in items)
        {
            string href = i[1].StartsWith("~/") ? VirtualPathUtility.ToAbsolute(i[1]) : i[1];
            html.Append("<li class='am-sa-nav'><a href='").Append(HttpUtility.HtmlAttributeEncode(href)).Append("' data-am-primary='true'><i class='' aria-hidden='true'></i><span>").Append(HttpUtility.HtmlEncode(i[0])).Append("</span>");
            if (i[2].Length > 0 && i[2] != "0") html.Append("<span class='am-nav-badge' data-sa-badge='").Append(i[0] == "Support Tickets" ? "tickets" : "addresses").Append("' style='margin-left:auto;min-width:20px;height:20px;padding:0 6px;border-radius:10px;background:#e5484d;color:#fff;font-size:11px;font-weight:700;display:inline-flex;align-items:center;justify-content:center'>").Append(i[2]).Append("</span>");
            html.Append("</a></li>");
        }
        return html.ToString();
    }

    // ---- v26: Address Verify against the existing address lookup (same sources as AnyminuteAddressLookup / SearchAddress.aspx) ----
    public static string CompactPostal(string p) { return (p ?? "").ToUpperInvariant().Replace(" ", "").Replace("-", "").Trim(); }
    static string Norm(string v) { return Regex.Replace((v ?? "").ToUpperInvariant(), @"[^A-Z0-9]", ""); }
    // Known addresses for a postal code: verified AddressMaster rows (city/province ids resolved to names) and employee addresses.
    public static DataTable KnownAddresses(SqlConnection c, string postal, int excludeId)
    {
        string p = CompactPostal(postal);
        return Q(c, @"SELECT DISTINCT TOP 12 Street, City, Province, PostalCode, Source FROM (
            SELECT UPPER(LTRIM(RTRIM(a.Street_Name))) Street, UPPER(LTRIM(RTRIM(COALESCE(ci.CityName,a.City)))) City, UPPER(LTRIM(RTRIM(COALESCE(pr.ProvinceName,a.Province)))) Province, UPPER(LTRIM(RTRIM(a.PostalCode))) PostalCode, 'Verified address' Source
              FROM AddressMaster a LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(a.City AS nvarchar(50)) LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=CAST(a.Province AS nvarchar(50))
             WHERE a.Verifed=1 AND a.AddressMasterId<>@id AND REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p
            UNION SELECT UPPER(LTRIM(RTRIM(e.StreetName))), UPPER(LTRIM(RTRIM(e.City))), UPPER(LTRIM(RTRIM(e.Province))), UPPER(LTRIM(RTRIM(e.Postalcode))), 'Employee address'
              FROM EmpAddress e WHERE REPLACE(REPLACE(UPPER(e.Postalcode),' ',''),'-','')=@p) x
            WHERE ISNULL(Street,'')<>'' AND ISNULL(City,'')<>'' AND ISNULL(Province,'')<>'' ORDER BY Source DESC, Street", "@p", p, "@id", excludeId);
    }
    // level: "error" (format), "match" (same address already known), "partial" (postal known, address differs), "new" (valid format, postal not known yet).
    public static string LookupVerify(SqlConnection c, int id, string street, string city, string province, string country, string postal, out string level, out DataTable known)
    {
        known = null;
        string err = VerifyAddress(street ?? "", city ?? "", province ?? "", country ?? "", postal ?? "");
        if (err != null) { level = "error"; return err; }
        known = KnownAddresses(c, postal, id);
        foreach (DataRow r in known.Rows)
            if (Norm(S(r["Street"])) == Norm(street) && Norm(S(r["City"])) == Norm(city) && (Norm(S(r["Province"])) == Norm(province) || Norm(S(r["Province"])).StartsWith(Norm(province)) || Norm(province).StartsWith(Norm(S(r["Province"])))))
            { level = "match"; return "Matches a known " + S(r["Source"]).ToLowerInvariant() + " for this postal code."; }
        if (known.Rows.Count > 0) { level = "partial"; return "Postal code is known, but the street / city differ from " + known.Rows.Count + " known address(es). Pick a suggestion or approve as entered."; }
        level = "new"; return "Format is valid. No existing address uses this postal code yet (new address).";
    }

    // ---- v26: plan catalogue -> PlanHistory limits ----
    // 'BASIC PLAN' (PlanHistory) == 'Basic' / 'Basic Plan' (catalogue).
    public static string PlanKey(string name)
    {
        string k = Regex.Replace((name ?? "").Trim().ToUpperInvariant(), @"\s+", " ");
        return k.EndsWith(" PLAN") ? k.Substring(0, k.Length - 5) : k;
    }
    public const string PlanKeySql = "CASE WHEN UPPER(LTRIM(RTRIM({0}))) LIKE '% PLAN' THEN LEFT(UPPER(LTRIM(RTRIM({0}))),LEN(LTRIM(RTRIM({0})))-5) ELSE UPPER(LTRIM(RTRIM({0}))) END";
    // Catalogue limits for a plan name; falls back to the defaults when the catalogue or the plan is missing or the limit is blank.
    public static void CatalogueLimits(string planName, int defaultEmployees, int defaultBusinesses, out int employees, out int businesses)
    {
        employees = defaultEmployees; businesses = defaultBusinesses;
        try
        {
            using (var c = Open())
            {
                if (!Has(c, "AnyminuteSubscriptionPlan")) return;
                var t = Q(c, "SELECT TOP 1 MaxEmployees,MaxBusinesses FROM AnyminuteSubscriptionPlan WHERE IsActive=1 AND " + String.Format(PlanKeySql, "PlanName") + "=@k ORDER BY PlanID", "@k", PlanKey(planName));
                if (t.Rows.Count == 0) return;
                if (t.Rows[0]["MaxEmployees"] != DBNull.Value) employees = Convert.ToInt32(t.Rows[0]["MaxEmployees"]);
                if (t.Rows[0]["MaxBusinesses"] != DBNull.Value) businesses = Convert.ToInt32(t.Rows[0]["MaxBusinesses"]);
            }
        }
        catch (Exception) { }
    }
}
