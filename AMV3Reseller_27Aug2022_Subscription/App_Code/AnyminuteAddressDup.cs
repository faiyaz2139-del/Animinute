using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SA = AnyminuteSupportAdmin;

// 2026-11-14 v81: Duplicate Addresses tab (PPT slides 1-3). A Potential Duplicate is 2+ AddressMaster
// rows in the same country whose standardized Street Address and Postal/ZIP Code both match. Decisions
// (Original / Duplicate / Dismissed / Deleted) are stored in AnyminuteAddressDup when that table exists
// (DB_REQUIRED/REQUIRED_10_AddressDuplicates.sql, also created on first use if the login can CREATE TABLE).
public static class AnyminuteAddressDup
{
    public const string Table = "AnyminuteAddressDup";
    public const string NeedTable = "Database update needed: run DB_REQUIRED/REQUIRED_10_AddressDuplicates.sql to save Duplicate Addresses decisions.";
    public class Group
    {
        public string Code, Key;
        public List<DataRow> Rows = new List<DataRow>();
        public int OriginalId;
    }

    public static string NormStreet(string s)
    {
        s = Regex.Replace((s ?? "").ToUpperInvariant(), @"[^A-Z0-9]+", " ");
        return Regex.Replace(s, @"\s+", " ").Trim();
    }
    public static string NormCountry(string country, string postal)
    {
        string k = AnyminuteAddressImport.Country(country);
        return k.Length > 0 ? k : SA.GuessCountry(postal);
    }
    public static string StreetOf(DataRow r)
    {
        string orig = r.Table.Columns.Contains("OrigStreet") ? SA.S(r["OrigStreet"]).Trim() : "";
        string st = SA.S(r["Street_Name"]).Trim();
        if (orig.Length > 0 && (st.Length == 0 || (char.IsDigit(orig[0]) && orig.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0))) return orig;
        return st.Length > 0 ? st : orig;
    }
    public static string CountryOf(DataRow r)
    {
        string ctry = r.Table.Columns.Contains("Country") ? SA.S(r["Country"]) : "";
        return NormCountry(ctry, SA.S(r["PostalCode"]));
    }
    public static string KeyOf(DataRow r)
    {
        return CountryOf(r) + "|" + SA.CompactPostal(SA.S(r["PostalCode"])) + "|" + NormStreet(StreetOf(r));
    }
    public static string PostalOf(DataRow r) { return SA.NormalizePostal(CountryOf(r), SA.S(r["PostalCode"])); }
    public static string StatusOf(DataRow r)
    {
        string st = r.Table.Columns.Contains("St") ? SA.S(r["St"]) : "";
        if (st.Length == 0) st = SA.S(r["Verifed"]) == "1" ? "Verified" : (r.Table.Columns.Contains("RStatus") && SA.S(r["RStatus"]) == "Rejected" ? "Rejected" : "Pending");
        return st;
    }
    public static string StatusLabel(string st) { return st == "Pending" ? "Pending Review" : st; }

    public static bool Ready(SqlConnection c, SqlTransaction tx)
    {
        try { return AnyminuteEmployeeEditor.HasTable(c, tx, Table); }
        catch (SqlException) { return false; }
    }
    public static void Ensure(SqlConnection c)
    {
        try
        {
            if (Ready(c, null)) return;
            AnyminuteTimesheetRepair.Execute(c, null, @"IF OBJECT_ID('dbo.AnyminuteAddressDup','U') IS NULL
CREATE TABLE dbo.AnyminuteAddressDup(
    DupId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteAddressDup PRIMARY KEY,
    AddressMasterId int NOT NULL,
    PeerId int NOT NULL CONSTRAINT DF_AnyminuteAddressDup_Peer DEFAULT 0,
    Action varchar(20) NOT NULL,
    ActorId int NULL,
    ActorName nvarchar(150) NULL,
    ActedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteAddressDup_At DEFAULT SYSUTCDATETIME(),
    Notes nvarchar(500) NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteAddressDup_Addr')
CREATE INDEX IX_AnyminuteAddressDup_Addr ON dbo.AnyminuteAddressDup(AddressMasterId,Action,ActedAtUTC);");
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("AnyminuteAddressDup.Ensure failed", ex); }
    }

    public static HashSet<int> DeletedIds(SqlConnection c) { return DeletedIds(c, null); }
    public static HashSet<int> DeletedIds(SqlConnection c, SqlTransaction tx)
    {
        var set = new HashSet<int>();
        if (!Ready(c, tx)) return set;
        try
        {
            var t = AnyminuteTimesheetRepair.Query(c, tx, @"SELECT AddressMasterId FROM (
                SELECT AddressMasterId, Action, ROW_NUMBER() OVER (PARTITION BY AddressMasterId ORDER BY ActedAtUTC DESC, DupId DESC) n
                FROM dbo.AnyminuteAddressDup WHERE Action='Deleted' AND PeerId=0) x WHERE n=1");
            foreach (DataRow r in t.Rows) set.Add(Convert.ToInt32(r[0]));
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("AnyminuteAddressDup.DeletedIds failed", ex); }
        return set;
    }

    class Pair
    {
        public string Action;
        public int Peer;
        public DateTime At;
        public int Id;
    }

    static List<Pair> Load(SqlConnection c, SqlTransaction tx)
    {
        var list = new List<Pair>();
        if (!Ready(c, tx)) return list;
        try
        {
            var t = AnyminuteTimesheetRepair.Query(c, tx, "SELECT DupId,AddressMasterId,PeerId,Action,ActedAtUTC FROM dbo.AnyminuteAddressDup");
            foreach (DataRow r in t.Rows)
                list.Add(new Pair { Id = Convert.ToInt32(r["AddressMasterId"]), Peer = Convert.ToInt32(r["PeerId"]), Action = SA.S(r["Action"]), At = r["ActedAtUTC"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["ActedAtUTC"]) });
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("AnyminuteAddressDup.Load failed", ex); }
        return list;
    }
    static bool PairDismissed(List<Pair> ev, int a, int b)
    {
        var hits = ev.Where(x => x.Action == "Dismissed" && ((x.Id == a && x.Peer == b) || (x.Id == b && x.Peer == a) || (x.Id == a && x.Peer == 0) || (x.Id == b && x.Peer == 0))).OrderByDescending(x => x.At).ToList();
        return hits.Count > 0;
    }
    static int OriginalOf(List<Pair> ev, IEnumerable<int> ids)
    {
        var set = new HashSet<int>(ids);
        var hit = ev.Where(x => x.Action == "Original" && x.Peer == 0 && set.Contains(x.Id)).OrderByDescending(x => x.At).FirstOrDefault();
        return hit != null ? hit.Id : 0;
    }

    public static List<Group> Groups(SqlConnection c, IList<DataRow> rows) { return Groups(c, null, rows); }
    public static List<Group> Groups(SqlConnection c, SqlTransaction tx, IList<DataRow> rows)
    {
        var deleted = DeletedIds(c, tx);
        var ev = Load(c, tx);
        var buckets = new Dictionary<string, List<DataRow>>(StringComparer.Ordinal);
        foreach (DataRow r in rows)
        {
            int id = Convert.ToInt32(r["AddressMasterId"]);
            if (deleted.Contains(id)) continue;
            string key = KeyOf(r);
            if (key.EndsWith("|") || SA.CompactPostal(SA.S(r["PostalCode"])).Length == 0 || NormStreet(StreetOf(r)).Length == 0) continue;
            List<DataRow> list;
            if (!buckets.TryGetValue(key, out list)) { list = new List<DataRow>(); buckets[key] = list; }
            list.Add(r);
        }
        var groups = new List<Group>();
        foreach (var kv in buckets.OrderBy(x => x.Value.Min(r => Convert.ToInt32(r["AddressMasterId"]))))
        {
            var members = kv.Value.OrderBy(r => !r.Table.Columns.Contains("SubmittedAtUTC") || r["SubmittedAtUTC"] == DBNull.Value ? DateTime.MaxValue : Convert.ToDateTime(r["SubmittedAtUTC"])).ThenBy(r => Convert.ToInt32(r["AddressMasterId"])).ToList();
            if (members.Count < 2) continue;
            var keep = new List<DataRow>();
            for (int i = 0; i < members.Count; i++)
            {
                int a = Convert.ToInt32(members[i]["AddressMasterId"]);
                bool any = false;
                for (int j = 0; j < members.Count; j++)
                {
                    if (i == j) continue;
                    int b = Convert.ToInt32(members[j]["AddressMasterId"]);
                    if (!PairDismissed(ev, a, b)) { any = true; break; }
                }
                if (any) keep.Add(members[i]);
            }
            if (keep.Count < 2) continue;
            groups.Add(new Group { Key = kv.Key, Rows = keep, OriginalId = OriginalOf(ev, keep.Select(r => Convert.ToInt32(r["AddressMasterId"]))) });
        }
        for (int i = 0; i < groups.Count; i++)
            groups[i].Code = "DUP-" + (i + 1).ToString("000", CultureInfo.InvariantCulture);
        return groups;
    }
    public static Group GroupOf(List<Group> groups, int id)
    {
        return groups.FirstOrDefault(g => g.Rows.Any(r => Convert.ToInt32(r["AddressMasterId"]) == id));
    }
    public static int CountRows(List<Group> groups) { return groups.Sum(g => g.Rows.Count); }

    public static bool Matches(SqlConnection c, SqlTransaction tx, int excludeId, string street, string postal, string country)
    {
        string keyC = NormCountry(country, postal), keyP = SA.CompactPostal(postal), keyS = NormStreet(street);
        if (keyP.Length == 0 || keyS.Length == 0) return false;
        try
        {
            var deleted = DeletedIds(c, tx);
            var t = AnyminuteTimesheetRepair.Query(c, tx, @"SELECT a.AddressMasterId,a.Street_Name,a.PostalCode,r.Country,r.OrigStreet
                FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId
                WHERE a.AddressMasterId<>@i AND REPLACE(REPLACE(UPPER(ISNULL(a.PostalCode,'')),' ',''),'-','')=@p",
                "@i", excludeId, "@p", keyP);
            foreach (DataRow r in t.Rows)
            {
                if (deleted.Contains(Convert.ToInt32(r["AddressMasterId"]))) continue;
                if (CountryOf(r) == keyC && NormStreet(StreetOf(r)) == keyS) return true;
            }
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("AnyminuteAddressDup.Matches failed", ex); }
        return false;
    }

    public static string Record(SqlConnection c, SqlTransaction tx, int id, int peer, string action, string notes)
    {
        if (!Ready(c, tx)) return NeedTable;
        AnyminuteTimesheetRepair.Execute(c, tx, "INSERT dbo.AnyminuteAddressDup(AddressMasterId,PeerId,Action,ActorId,ActorName,ActedAtUTC,Notes) VALUES(@i,@p,@a,@b,@n,SYSUTCDATETIME(),@c)",
            "@i", id, "@p", peer, "@a", action, "@b", SA.ActorId(), "@n", Cut(SA.ActorName(), 150), "@c", Cut(notes ?? "", 500));
        return null;
    }
    static string Cut(string s, int n) { s = s ?? ""; return s.Length > n ? s.Substring(0, n) : s; }

    public static string Card(Group g, int focusId, Func<DataRow, string> who)
    {
        if (g == null || g.Rows.Count == 0) return "";
        var sb = new StringBuilder();
        sb.Append("<div class='sa-dup-card'><div class='sa-dup-card-head'><strong>Duplicate Group #").Append(SA.H(g.Code)).Append("</strong>")
          .Append("<span class='sa-pill sa-purple'>").Append(g.Rows.Count.ToString(CultureInfo.InvariantCulture)).Append(" Matching Records</span></div>");
        sb.Append("<table class='sa-dup-cmp'><thead><tr><th>Field</th>");
        for (int i = 0; i < g.Rows.Count && i < 4; i++)
        {
            string label = "Record " + (char)('A' + i);
            int rid = Convert.ToInt32(g.Rows[i]["AddressMasterId"]);
            if (rid == focusId) label += " (this)";
            if (g.OriginalId == rid) label += " — Original";
            sb.Append("<th>").Append(SA.H(label)).Append("</th>");
        }
        if (g.Rows.Count > 4) sb.Append("<th>+").Append(g.Rows.Count - 4).Append(" more</th>");
        sb.Append("</tr></thead><tbody>");
        Action<string, Func<DataRow, string>> row = (field, val) =>
        {
            sb.Append("<tr><th>").Append(SA.H(field)).Append("</th>");
            for (int i = 0; i < g.Rows.Count && i < 4; i++) sb.Append("<td>").Append(val(g.Rows[i])).Append("</td>");
            if (g.Rows.Count > 4) sb.Append("<td></td>");
            sb.Append("</tr>");
        };
        row("Address", r => SA.H(StreetOf(r)));
        row("Postal Code", r => SA.H(PostalOf(r)));
        row("Status", r =>
        {
            string st = StatusOf(r);
            string tone = st == "Verified" ? "sa-green" : st == "Rejected" ? "sa-grey" : "sa-amber";
            return "<span class='sa-pill " + tone + "'>" + SA.H(StatusLabel(st)) + "</span>";
        });
        row("Submitted By", r => SA.H(who(r) ?? "Not linked"));
        sb.Append("</tbody></table></div>");
        return sb.ToString();
    }
}
