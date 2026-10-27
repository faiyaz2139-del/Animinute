using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-26 v19 (Pandey reports deck, 6 slides): one Reports module for Employee / Supervisor / Administrator.
// Role per business (same rule as v18): Administrator = owner (1) or Administrator (5); Supervisor = any other
// non-Employee membership (Manager 2, Team Leader 13, Accountant 12 ...) or an Employee listed as someone's manager
// (AssgineManager); Employee = Employee (4). Which reports each role may View / PDF / Excel comes from Report Access
// (dbo.AnyminuteReportAccess, DB_REQUIRED/REQUIRED_06_ReportAccess.sql); without that table the slide 6 defaults apply.
// Data scope: Administrator (and Accountant) = whole business; Supervisor = self + assigned employees; Employee = self.
// Every number comes from existing tables. Nothing is invented; empty results show as empty.
public sealed class AmRptDef
{
    public string Key, Name, Desc, Icon; public bool[] View, Pdf, Excel; // index 0 Employee, 1 Supervisor, 2 Administrator
    public bool Emp, Proj, Inactive; public string DateMode = "range", DateLabel = "Date Range"; // range | optional | none
    public string[][] Choices = new string[0][]; // {param, label, column}
}
public sealed class AmRptPerm { public bool View, Pdf, Excel; public bool Any { get { return View || Pdf || Excel; } } }
public sealed class AmRptBiz { public int Id, Tier, Actor, Role, Owner; public string Name; public HashSet<int> Team; public bool Costs; }
public sealed class AmRptSection { public string Title; public string[] Cols; public List<string[]> Rows = new List<string[]>(); }
public sealed class AmRptResult
{
    public AmRptDef Def; public string Subtitle, FilterText, Generated, Note;
    public List<KeyValuePair<string, string>> Kpis = new List<KeyValuePair<string, string>>();
    public List<AmRptSection> Sections = new List<AmRptSection>();
    public Dictionary<string, List<string>> Options = new Dictionary<string, List<string>>();
}
public sealed class AmRptQuery
{
    public string Key = "", Business = "", Action = "view"; public int Emp, Proj; public bool Inactive; public DateTime? From, To;
    public Dictionary<string, string> Choice = new Dictionary<string, string>();
    public static AmRptQuery Parse(System.Collections.Specialized.NameValueCollection q)
    {
        var r = new AmRptQuery(); r.Key = (q["r"] ?? "").Trim().ToLowerInvariant(); r.Business = (q["b"] ?? "").Trim().ToLowerInvariant();
        int i; if (Int32.TryParse(q["emp"], out i)) r.Emp = i; if (Int32.TryParse(q["proj"], out i)) r.Proj = i;
        r.Inactive = q["inactive"] == "1"; DateTime d;
        if (DateTime.TryParseExact(q["from"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) r.From = d;
        if (DateTime.TryParseExact(q["to"], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) r.To = d;
        foreach (string k in q.AllKeys) if (k != null && k.StartsWith("c_", StringComparison.Ordinal) && !String.IsNullOrWhiteSpace(q[k])) r.Choice[k.Substring(2)] = q[k].Trim();
        string a = (q["export"] ?? "").ToLowerInvariant(); r.Action = a == "pdf" ? "pdf" : a == "excel" ? "excel" : "view";
        return r;
    }
}

public static partial class AnyminuteReports
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public static readonly string[] Tiers = { "Employee", "Supervisor", "Administrator" };
    public const string Table = "AnyminuteReportAccess";
    static bool[] T(bool e, bool s, bool a) { return new[] { e, s, a }; }
    static AmRptDef D(string key, string name, string desc, string icon, bool[] who) { return new AmRptDef { Key = key, Name = name, Desc = desc, Icon = icon, View = who, Pdf = (bool[])who.Clone(), Excel = (bool[])who.Clone() }; }
    static string[] C(string p, string label, string col) { return new[] { p, label, col }; }
    // Order: slide 1 list, then the remaining slide 3 cards. Defaults: slide 6 matrix.
    public static readonly List<AmRptDef> Defs = new List<AmRptDef> {
        new Func<AmRptDef>(() => { var d = D("time-card", "Time Card Detail Report", "Detailed view of employee time cards including clock in/out, breaks and total hours.", "doc", T(true, true, true)); d.Emp = d.Proj = true; d.DateLabel = "Work Date Range"; d.Choices = new[] { C("att", "Attendance Status", "Attendance"), C("appr", "Approval Status", "Approval Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("work-schedule", "Work Schedule Report", "Shows scheduled shifts, days off and planned hours for a selected period.", "cal", T(true, true, true)); d.Emp = true; d.DateLabel = "Schedule Date Range"; d.Choices = new[] { C("day", "Working / Day Off", "Day Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("payroll-hours", "Payroll Hours Report", "Summary of approved hours, regular, overtime and payroll information.", "dollar", T(false, true, true)); d.Emp = true; d.DateLabel = "Pay Period"; d.Choices = new[] { C("appr", "Approval Status", "Approval Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("approval-audit", "Approval Audit Report", "Audit trail of time card approvals including who, when and what changed.", "shield", T(false, true, true)); d.Emp = true; d.DateLabel = "Action Date Range"; d.Choices = new[] { C("by", "Approver", "Action By"), C("scope", "Daily / Weekly", "Approval Scope"), C("act", "Approval Action", "Action") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("user-access", "User Access Report", "List of users, roles, login status and last login for the selected business.", "users", T(false, false, true)); d.Emp = true; d.Inactive = true; d.DateMode = "none"; d.Choices = new[] { C("role", "Role", "Role"), C("login", "Login Status", "Login Status"), C("mfa", "MFA Status", "MFA Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("project-cost", "Project & Cost Report", "Hours and labour cost by project and employee.", "chart", T(false, true, true)); d.Emp = d.Proj = true; d.Choices = new[] { C("appr", "Approval Status", "Approval Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("business-assignment", "Business Assignment Report", "Employee and supervisor assignments across businesses.", "building", T(false, true, true)); d.Emp = true; d.Inactive = true; d.DateMode = "optional"; d.DateLabel = "Effective Date Range"; d.Choices = new[] { C("role", "Role", "Role"), C("st", "Assignment Status", "Assignment Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("employee-master", "Employee Master Report", "Employee profiles, contact details and statuses.", "person", T(false, true, true)); d.Emp = true; d.Inactive = true; d.DateMode = "optional"; d.DateLabel = "Hire Date Range"; d.Choices = new[] { C("st", "Employee Status", "Employment Status"), C("role", "Role", "Role"), C("login", "Login Status", "Login Access") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("pay-rate", "Pay Rate History Report", "Effective-dated pay rates by employee and business.", "rate", T(false, false, true)); d.Emp = true; d.Inactive = true; d.DateMode = "optional"; d.DateLabel = "Effective Date Range"; d.Choices = new[] { C("type", "Rate Type", "Pay Type"), C("rs", "Current / Historical", "Rate Status") }; return d; })(),
        new Func<AmRptDef>(() => { var d = D("accountant-access", "Accountant & Payroll Access Report", "External accountants and payroll administrators and their permissions.", "key", T(false, false, true)); d.DateMode = "none"; d.Choices = new[] { C("firm", "Firm Name", "Firm Name"), C("type", "Access Type", "Access Type"), C("st", "Access Status", "Access Status") }; return d; })(),
    };
    public static AmRptDef Def(string key) { return Defs.FirstOrDefault(d => d.Key == key); }

    // ---------- Report Access matrix ----------
    public static Dictionary<string, AmRptPerm[]> Defaults()
    {
        var m = new Dictionary<string, AmRptPerm[]>();
        foreach (var d in Defs) m[d.Key] = Enumerable.Range(0, 3).Select(i => new AmRptPerm { View = d.View[i], Pdf = d.Pdf[i], Excel = d.Excel[i] }).ToArray();
        return m;
    }
    public static bool Installed(SqlConnection c) { return AnyminuteEmployeeEditor.HasTable(c, null, Table); }
    public static bool Installed() { using (var c = AnyminuteTimesheetRepair.Open()) return Installed(c); }
    static Dictionary<string, AmRptPerm[]> Rows(SqlConnection c, int owner, int business)
    {
        var dt = AnyminuteTimesheetRepair.Query(c, null, "SELECT ReportKey,Tier,CanView,CanPdf,CanExcel FROM dbo.AnyminuteReportAccess WHERE OwnerID=@o AND BusinessID=@b", "@o", owner, "@b", business);
        if (dt.Rows.Count == 0) return null;
        var m = Defaults();
        foreach (DataRow r in dt.Rows)
        {
            AmRptPerm[] p; int tier = Array.IndexOf(Tiers, Convert.ToString(r["Tier"]));
            if (tier < 0 || !m.TryGetValue(Convert.ToString(r["ReportKey"]), out p)) continue;
            p[tier] = new AmRptPerm { View = Convert.ToBoolean(r["CanView"]), Pdf = Convert.ToBoolean(r["CanPdf"]), Excel = Convert.ToBoolean(r["CanExcel"]) };
        }
        return m;
    }
    // business = 0: the owner's global defaults. A business override (rows for that business) replaces the defaults.
    public static Dictionary<string, AmRptPerm[]> Matrix(SqlConnection c, int owner, int business, out bool overridden)
    {
        overridden = false;
        if (owner <= 0 || !Installed(c)) return Defaults();
        if (business > 0) { var o = Rows(c, owner, business); if (o != null) { overridden = true; return o; } }
        return Rows(c, owner, 0) ?? Defaults();
    }
    public static void Save(int owner, int business, Dictionary<string, AmRptPerm[]> m)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Installed(c)) throw new InvalidOperationException("Run DB_REQUIRED/REQUIRED_06_ReportAccess.sql to save report access.");
            if (business > 0) AnyminuteApprovalAssignments.RequireOwner(c, null, business);
            int actor = Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]);
            using (var tx = c.BeginTransaction())
            {
                AnyminuteTimesheetRepair.Execute(c, tx, "DELETE dbo.AnyminuteReportAccess WHERE OwnerID=@o AND BusinessID=@b", "@o", owner, "@b", business);
                foreach (var d in Defs) for (int i = 0; i < 3; i++)
                    {
                        var p = m[d.Key][i];
                        AnyminuteTimesheetRepair.Execute(c, tx, "INSERT dbo.AnyminuteReportAccess(OwnerID,BusinessID,ReportKey,Tier,CanView,CanPdf,CanExcel,UpdatedAtUTC,UpdatedBy) VALUES(@o,@b,@k,@t,@v,@p,@x,SYSUTCDATETIME(),@a)",
                            "@o", owner, "@b", business, "@k", d.Key, "@t", Tiers[i], "@v", p.View, "@p", p.Pdf, "@x", p.Excel, "@a", actor);
                    }
                tx.Commit();
            }
        }
    }
    public static void ClearOverride(int owner, int business)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!Installed(c)) return;
            AnyminuteApprovalAssignments.RequireOwner(c, null, business);
            AnyminuteTimesheetRepair.Execute(c, null, "DELETE dbo.AnyminuteReportAccess WHERE OwnerID=@o AND BusinessID=@b", "@o", owner, "@b", business);
        }
    }
    public static int OwnerOf(SqlConnection c, int business)
    {
        var dt = AnyminuteTimesheetRepair.Query(c, null, "SELECT MIN(EmployeeID) FROM AssginBussiness WHERE BusinessID=@b AND UserTypeId=1", "@b", business);
        return dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0][0]);
    }

    // ---------- Scope (role per business), cached per request ----------
    public static List<AmRptBiz> Scope()
    {
        var h = HttpContext.Current; var cached = h.Items["AmRptScope"] as List<AmRptBiz>; if (cached != null) return cached;
        var list = new List<AmRptBiz>();
        using (var c = AnyminuteTimesheetRepair.Open())
            foreach (DataRow b in AnyminuteModernScope.Businesses().Rows)
            {
                int id = Convert.ToInt32(b["BusinessID"]), role, actor;
                try { actor = AnyminuteModernScope.Actor(c, null, id, out role); } catch (InvalidOperationException) { continue; }
                var managed = AnyminuteTimesheetRepair.Query(c, null, "SELECT DISTINCT EmpId FROM AssgineManager WHERE BusinessID=@b AND ManagerID=@a AND EmpId<>@a", "@b", id, "@a", actor);
                int tier = role == 1 || role == 5 ? 2 : role == 4 ? (managed.Rows.Count > 0 ? 1 : 0) : 1;
                var z = new AmRptBiz { Id = id, Name = Convert.ToString(b["CompanyName"]), Tier = tier, Actor = actor, Role = role, Owner = OwnerOf(c, id), Costs = tier == 2 };
                if (tier < 2 && role != 12) { z.Team = new HashSet<int> { actor }; foreach (DataRow r in managed.Rows) z.Team.Add(Convert.ToInt32(r[0])); }
                list.Add(z);
            }
        h.Items["AmRptScope"] = list; return list;
    }
    static Dictionary<string, AmRptPerm[]> MatrixFor(AmRptBiz b)
    {
        var h = HttpContext.Current; string k = "AmRptM" + b.Id; var m = h.Items[k] as Dictionary<string, AmRptPerm[]>;
        if (m == null) { bool o; using (var c = AnyminuteTimesheetRepair.Open()) m = Matrix(c, b.Owner, b.Id, out o); h.Items[k] = m; }
        return m;
    }
    public static AmRptPerm Perm(AmRptDef d, AmRptBiz b) { return MatrixFor(b)[d.Key][b.Tier]; }
    static bool Allowed(AmRptPerm p, string action) { return action == "pdf" ? p.Pdf : action == "excel" ? p.Excel : p.View; }
    public static List<AmRptDef> Available(string business)
    {
        var scope = Scope().Where(b => business == "" || business == "all" || b.Id.ToString(Inv) == business).ToList();
        return Defs.Where(d => scope.Any(b => Perm(d, b).Any)).ToList();
    }
    // True when the signed-in account has at least one report somewhere / is owner or administrator somewhere
    // (both cached in the session for 2 minutes so the left navigation stays cheap).
    static bool[] Flags()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null || h.Session["EmployeeId"] == null) return new[] { false, false };
        var c = h.Session["AmRptNav"] as Tuple<DateTime, bool[]>;
        if (c != null && c.Item1 > DateTime.UtcNow) return c.Item2;
        bool[] f = { false, false };
        // v20: the two probes are independent, so a report-matrix failure can no longer hide User Access > Login & Access.
        try { f[1] = Scope().Any(b => b.Tier == 2); } catch (HttpException) { } catch (InvalidOperationException) { } catch (SqlException) { }
        try { f[0] = Available("").Count > 0; } catch (HttpException) { } catch (InvalidOperationException) { } catch (SqlException) { }
        h.Session["AmRptNav"] = Tuple.Create(DateTime.UtcNow.AddMinutes(2), f); return f;
    }
    public static bool AnyAvailable() { return Flags()[0]; }
    public static bool AdminSomewhere() { return Flags()[1]; }
    public static void Forget() { var h = HttpContext.Current; if (h != null && h.Session != null) h.Session.Remove("AmRptNav"); }
    public static string HubHref(IEnumerable<MenuViewModel> permitted)
    {
        if (AnyAvailable()) return VirtualPathUtility.ToAbsolute("~/Reports.aspx");
        return AnyminuteUiNavigation.ReportHref(permitted);
    }
}
