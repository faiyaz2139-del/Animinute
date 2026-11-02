using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-11-02 v49 (Pandey notes "report types not seen in all logins"): a reseller login (UserAcc Admin row created by
// AnyminuteResellers.EnsureLogin) with the "View reports (for their subscribers)" permission opens the 8 Reseller reports
// on SupportReports.aspx, limited to its own reseller rows. Support Admin keeps all 20 reports. No new tables.
public sealed class SrScope { public int ResellerId; public string Code, Name; }
public static partial class AnyminuteSupportReports
{
    // Signed-in reseller allowed to view reports, or null (Support Admin, other logins, REQUIRED_09 not run). Cached 2 min per session.
    public static SrScope ResellerScope()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null || h.Session["UserId"] == null) return null;
        if (SA.IsSupport()) return null;
        var cached = h.Session["AmSrRsl"] as Tuple<DateTime, SrScope>;
        if (cached != null && cached.Item1 > DateTime.UtcNow) return cached.Item2;
        SrScope s = null; int user; Int32.TryParse(Convert.ToString(h.Session["UserId"]), out user);
        string email = Convert.ToString(h.Session["email"]).Trim();
        if (user > 0 && email.Length > 0)
        {
            try
            {
                using (var c = SA.Open())
                    if (RS.Ready(c) && RS.LoginUser(c, email) == user)
                    {
                        var t = SA.Q(c, "SELECT TOP 1 ResellerID,ResellerName,Status,Permissions FROM AnyminuteReseller WHERE LTRIM(RTRIM(LoginEmail))=@e", "@e", email);
                        if (t.Rows.Count == 1 && (SA.S(t.Rows[0]["Status"]).Length == 0 || SA.S(t.Rows[0]["Status"]) == "Active")
                            && SA.S(t.Rows[0]["Permissions"]).Split(',').Any(p => p.Trim().Equals("reports", StringComparison.OrdinalIgnoreCase)))
                            s = new SrScope { ResellerId = Convert.ToInt32(t.Rows[0]["ResellerID"]), Code = RS.Rsl(t.Rows[0]["ResellerID"]), Name = SA.S(t.Rows[0]["ResellerName"]) };
                    }
            }
            catch (SqlException) { s = null; }
            catch (InvalidOperationException) { s = null; }
        }
        h.Session["AmSrRsl"] = Tuple.Create(DateTime.UtcNow.AddMinutes(2), s); return s;
    }
    // Support Admin, or a reseller with report permission (Reseller reports only).
    public static bool CanOpen() { return SA.IsSupport() || ResellerScope() != null; }
    public static List<SrDef> DefsFor(SrScope s) { return s == null ? Defs : Defs.Where(d => d.Group == "Reseller").ToList(); }
    public static SrDef DefFor(SrScope s, string key) { var l = DefsFor(s); return l.FirstOrDefault(d => d.Key == key) ?? l[0]; }
    // Keeps only the reseller's own rows: by Reseller ID (RSL-xxx) when the report has it, otherwise by reseller name.
    public static SrData Limit(SrData d, SrScope s)
    {
        if (s == null || d == null) return d;
        int id = d.Ix("Reseller ID"), name = id >= 0 ? -1 : Math.Max(d.Ix("Reseller"), d.Ix("Reseller Name"));
        var keep = new SrData(d.Cols) { Gate = d.Gate, Note = d.Note };
        if (id < 0 && name < 0) return keep;
        keep.Rows.AddRange(d.Rows.Where(r => id >= 0 ? r[id] == s.Code : String.Equals(r[name], s.Name, StringComparison.OrdinalIgnoreCase)));
        return keep;
    }
}
