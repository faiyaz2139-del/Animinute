using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-10-29 v28 (Shamsu deck 1 slides 1-2, deck 2 slides 3-4, Pandey sample PDFs/XLSX): Support Admin Reports hub.
// 12 support reports + 8 reseller reports. Every report reads live data (Ticket, Ticket_Comments, AddressMaster, UserAcc,
// PlanHistory, AddBusiness, AnyminuteSupportAudit, AnyminuteSubscriptionPlan, AnyminuteReseller...) and shows an honest
// empty state when a table is missing or no rows match. No sample rows are ever generated.
// Flow: Load (SQL -> base rows incl. every filter column) -> column filters -> optional Post (aggregate) -> KPIs -> option-gated columns.
public sealed class SrFilter
{
    public string Key, Label, Col, Type; public string[] Fixed; // Type: select | range | min | text | param
    public SrFilter(string key, string label, string col, string type, params string[] fixedOptions) { Key = key; Label = label; Col = col; Type = type; Fixed = fixedOptions.Length == 0 ? null : fixedOptions; }
}
public sealed class SrData
{
    public string[] Cols; public string[] Gate; public List<string[]> Rows = new List<string[]>(); public string Note;
    public SrData(params string[] cols) { Cols = cols; Gate = new string[cols.Length]; }
    public int Ix(string col) { return Array.IndexOf(Cols, col); }
    public void Add(params object[] v) { Rows.Add(v.Select(x => x == null || x == DBNull.Value ? "" : Convert.ToString(x, CultureInfo.InvariantCulture).Trim()).ToArray()); }
    public SrData Opt(string option, params string[] cols) { foreach (var c in cols) { int i = Ix(c); if (i >= 0) Gate[i] = option; } return this; }
}
public sealed class SrDef
{
    public string Key, Name, Group, Purpose; public SrFilter[] Filters = new SrFilter[0]; public string[][] Options = new string[0][]; // {key,label,defaultOn}
    public Func<SqlConnection, NameValueCollection, SrData> Load; public Func<SrData, NameValueCollection, SrData> Post;
    public Func<SrData, List<KeyValuePair<string, string>>> Kpis;
}
public static partial class AnyminuteSupportReports
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public const string Build = "v28";
    public const int PreviewRows = 500;
    // ---------- formatting ----------
    static string Day(object o) { if (o == null || o == DBNull.Value) return ""; DateTime d; return DateTime.TryParse(Convert.ToString(o, Inv), Inv, DateTimeStyles.None, out d) ? d.ToString("yyyy-MM-dd", Inv) : SA.S(o); }
    static string Stamp(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToDateTime(o, Inv).ToString("yyyy-MM-dd HH:mm", Inv); }
    static string Utc(object o) { return o == null || o == DBNull.Value ? "" : DateTime.SpecifyKind(Convert.ToDateTime(o, Inv), DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm", Inv); }
    static DateTime? Utc2(object o) { return o == null || o == DBNull.Value ? (DateTime?)null : DateTime.SpecifyKind(Convert.ToDateTime(o, Inv), DateTimeKind.Utc).ToLocalTime(); }
    static DateTime? Dt(object o) { return o == null || o == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(o, Inv); }
    static string Hrs(double? h) { return h == null ? "" : Math.Round(h.Value, 1).ToString("0.0", Inv); }
    static int? Int(object o) { int i; return Int32.TryParse(SA.S(o), out i) ? i : (int?)null; }
    static string Pct(int used, int? limit) { return limit == null || limit.Value <= 0 ? "" : Math.Round(used * 100.0 / limit.Value, 1).ToString("0.#", Inv); }
    static string LimitStatus(int used, int? limit) { if (limit == null || limit.Value <= 0) return "Unlimited"; double p = used * 100.0 / limit.Value; return p > 100 ? "Over Limit" : p >= 100 ? "At Limit" : p >= 80 ? "Near Limit" : "Within Limit"; }
    static string Avg(IEnumerable<double> v) { var l = v.ToList(); return l.Count == 0 ? "\u2014" : Math.Round(l.Average(), 1).ToString("0.0", Inv) + " h"; }
    static KeyValuePair<string, string> K(string k, object v) { return new KeyValuePair<string, string>(k, Convert.ToString(v, Inv)); }
    static int Count(SrData d, string col, params string[] values) { int i = d.Ix(col); return i < 0 ? 0 : d.Rows.Count(r => values.Any(v => String.Equals(r[i], v, StringComparison.OrdinalIgnoreCase))); }
    static int Sum(SrData d, string col) { int i = d.Ix(col); return i < 0 ? 0 : d.Rows.Sum(r => Int(r[i]) ?? 0); }
    static string Ticket(object no, object id) { string n = SA.S(no); return n.Length > 0 ? n : "TK-" + Convert.ToInt32(id).ToString(Inv); }
    static string TicketType(object p) { switch (SA.S(p)) { case "1": return "Login Issue"; case "2": return "Password Issue"; case "3": return "Application Crash"; case "4": return "Functionality Issue"; default: return "Other"; } }
    public static readonly string[] Ranges = { "All Dates", "Today", "Last 7 Days", "Last 30 Days", "This Month", "Last Month", "This Year", "Custom Range" };
    static readonly string[] Pcts = { "50", "80", "90", "100" };

    // ---------- shared loaders ----------
    static bool Sec(SqlConnection c) { return SA.Has(c, "AnyminuteLoginSecurity"); }
    static DataTable SubscriberTable(SqlConnection c)
    {
        bool sec = Sec(c), last = sec && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteLoginSecurity", "LastLoginDate"), cat = SA.Has(c, "AnyminuteSubscriptionPlan");
        const string own = "(SELECT a.BusinessID FROM AssginBussiness a WHERE a.EmployeeID=u.EmployeeId AND a.UserTypeId=1)";
        return SA.Q(c, @"SELECT u.UserId,u.EmployeeId,u.Email,u.Status,u.Is_First_Time_Login,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.MiddleName,'')+' '+ISNULL(ae.LastName,''))) Name,
            ae.MobilePhone,ph.PlanName,ph.No_Of_Users,ph.Business,ph.PlanActivationDate,
            (SELECT MIN(p.StartDate) FROM PlanHistory p WHERE p.EmailId=u.Email) Since," + RS.OwnedBiz + @" Businesses,
            (SELECT COUNT(DISTINCT a2.EmployeeID) FROM AssginBussiness a2 JOIN AddEmployee e2 ON e2.EmployeeID=a2.EmployeeID WHERE a2.EmployeeID<>u.EmployeeId AND e2.TerminationDate IS NULL AND a2.BusinessID IN " + own + @") Employees,
            (SELECT COUNT(*) FROM AddProject pj WHERE pj.BusinessID IN " + own + @" AND ISNULL(pj.Status,'') NOT IN('0','2','Inactive','InActive','Closed','Completed')) Projects," +
            (cat ? "(SELECT TOP 1 sp.MaxProjects FROM AnyminuteSubscriptionPlan sp WHERE " + String.Format(SA.PlanKeySql, "sp.PlanName") + "=" + String.Format(SA.PlanKeySql, "ph.PlanName") + ") ProjectLimit," : "CAST(NULL AS int) ProjectLimit,") +
            (sec ? "ISNULL(s.LoginDisabled,0) LoginDisabled,ISNULL(s.TwoFactorEnabled,0) TwoFactor,s.InvitationCancelledAtUTC," : "CAST(0 AS bit) LoginDisabled,CAST(0 AS bit) TwoFactor,CAST(NULL AS datetime2) InvitationCancelledAtUTC,") +
            (last ? "s.LastLoginDate" : "CAST(NULL AS datetime2) LastLoginDate") +
            " FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId OUTER APPLY (SELECT TOP 1 p.PlanName,p.No_Of_Users,p.Business,p.PlanActivationDate FROM PlanHistory p WHERE p.EmailId=u.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) ph" +
            (sec ? " LEFT JOIN AnyminuteLoginSecurity s ON s.UserId=u.UserId" : "") +
            " WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1) ORDER BY u.UserId");
    }
    static string Name(DataRow r) { string n = AnyminuteDisplayText.Tidy(SA.S(r["Name"])); return n.Length > 0 ? n : SA.S(r["Email"]); }
    static bool Off(DataRow r) { return r["Status"] != DBNull.Value && !Convert.ToBoolean(r["Status"]); }
    static bool First(DataRow r) { return r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]); }
    static string AccountStatus(DataRow r) { return Off(r) ? "Inactive" : Convert.ToBoolean(r["LoginDisabled"]) ? "Suspended" : First(r) ? "Pending Activation" : "Active"; }
    static string LoginAccess(DataRow r) { return Off(r) || Convert.ToBoolean(r["LoginDisabled"]) ? "Disabled" : "Enabled"; }
    static string Invitation(DataRow r) { return r["InvitationCancelledAtUTC"] != DBNull.Value ? "Cancelled" : First(r) ? "Pending" : "Accepted"; }
    static Dictionary<int, string> SubscriberNames(SqlConnection c)
    {
        var d = new Dictionary<int, string>();
        foreach (DataRow r in SA.Q(c, "SELECT u.UserId,u.Email,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId").Rows)
            d[Convert.ToInt32(r["UserId"])] = Name(r);
        return d;
    }
    static DataTable Audit(SqlConnection c, string where)
    {
        if (!SA.Has(c, "AnyminuteSupportAudit")) return null;
        return SA.Q(c, "SELECT TOP 5000 * FROM AnyminuteSupportAudit WHERE " + where + " ORDER BY ChangedAtUTC DESC,AuditID DESC");
    }
    static int PlanRank(string plan)
    {
        string k = SA.PlanKey(plan);
        if (k.Contains("TRIAL")) return 0; if (k.Contains("BASIC")) return 1; if (k.Contains("STANDARD")) return 2;
        if (k.Contains("PROFESSIONAL")) return 3; if (k.Contains("PREMIUM") || k.Contains("ENTERPRISE")) return 4; return -1;
    }
}
