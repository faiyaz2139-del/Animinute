using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-26 v19: HTML for Reports.aspx (slides 1-2), ReportAccess.aspx (slide 6) and LoginAccess.aspx (t77u), plus exports.
public static partial class AnyminuteReports
{
    static string E(string s) { return HttpUtility.HtmlEncode(s ?? ""); }
    static string A(string s) { return HttpUtility.HtmlAttributeEncode(s ?? ""); }
    const int PreviewRows = 50;
    static readonly Dictionary<string, string> Icons = new Dictionary<string, string> {
        { "doc", "M6 2h9l5 5v15H6zM14 2v6h6M9 13h8M9 17h8" }, { "cal", "M4 5h16v16H4zM4 10h16M8 3v4M16 3v4" }, { "dollar", "M12 2v20M17 6H10a3 3 0 0 0 0 6h4a3 3 0 0 1 0 6H6" },
        { "shield", "M12 2l8 3v6c0 5-3.5 9-8 11-4.5-2-8-6-8-11V5z" }, { "users", "M9 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM2 21c0-4 3-6 7-6s7 2 7 6M17 11a3 3 0 1 0 0-6M22 21c0-3-2-5-5-5.5" },
        { "chart", "M4 20V10M10 20V4M16 20v-7M22 20H2" }, { "building", "M4 21V3h10v18M14 8h6v13M7 7h2M7 11h2M7 15h2" }, { "person", "M12 12a5 5 0 1 0 0-10 5 5 0 0 0 0 10zM3 22c0-5 4-8 9-8s9 3 9 8" },
        { "rate", "M3 17l6-6 4 4 8-8M15 7h6v6" }, { "key", "M14 10a5 5 0 1 0-4 4l2 2h3v3h3v3h3v-4z" } };
    static string Icon(string k) { return "<svg viewBox=\"0 0 24 24\" aria-hidden=\"true\"><path d=\"" + Icons[k] + "\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\"/></svg>"; }
    static string Href(AmRptQuery q, string key, string export)
    {
        var p = new List<string> { "r=" + key };
        if (q.Business != "") p.Add("b=" + HttpUtility.UrlEncode(q.Business));
        if (key == q.Key)
        {
            if (q.Emp > 0) p.Add("emp=" + q.Emp); if (q.Proj > 0) p.Add("proj=" + q.Proj); if (q.Inactive) p.Add("inactive=1");
            if (q.From.HasValue) p.Add("from=" + q.From.Value.ToString("yyyy-MM-dd", Inv)); if (q.To.HasValue) p.Add("to=" + q.To.Value.ToString("yyyy-MM-dd", Inv));
            foreach (var c in q.Choice) p.Add("c_" + HttpUtility.UrlEncode(c.Key) + "=" + HttpUtility.UrlEncode(c.Value));
        }
        if (export != null) p.Add("export=" + export);
        return VirtualPathUtility.ToAbsolute("~/Reports.aspx") + "?" + String.Join("&", p);
    }

    // ---------- Reports hub ----------
    // v46 (Reports deck "same across all module"): the hub uses the shared AnyminuteReportLayout - breadcrumb + icon title,
    // left "Search reports..." list in collapsible groups, Report Filters card with Advanced Filters & Options,
    // Clear Filters / Generate Report, and a Report Preview card (Export PDF / Excel, "No report generated" until generated).
    static readonly string[][] HubGroups = {
        new[] { "Time & Payroll Reports", "time-card", "work-schedule", "payroll-hours", "approval-audit", "project-cost" },
        new[] { "People & Access Reports", "employee-master", "business-assignment", "pay-rate", "user-access", "accountant-access" } };
    public static string Hub(AmRptQuery q)
    {
        var scope = Scope(); var sb = new StringBuilder();
        if (scope.Count == 0) return "<p class=\"am-rp-empty\">You are not assigned to an active business yet, so there are no reports to show.</p>";
        if (q.Business != "all" && q.Business != "" && !scope.Any(b => b.Id.ToString(Inv) == q.Business)) q.Business = "";
        if (q.Business == "") q.Business = scope.Count == 1 ? scope[0].Id.ToString(Inv) : "all";
        var inScope = scope.Where(b => q.Business == "all" || b.Id.ToString(Inv) == q.Business).ToList();
        var defs = Available(q.Business);
        var def = defs.FirstOrDefault(x => x.Key == q.Key) ?? defs.FirstOrDefault(x => inScope.Any(b => Perm(x, b).View)) ?? defs.FirstOrDefault();
        string home = VirtualPathUtility.ToAbsolute("~/Reports.aspx");
        sb.Append("<div class=\"am-rl\">");
        sb.Append(AnyminuteReportLayout.Head("Reports", home, def == null ? "Reports" : def.Name, def == null ? "View, filter, preview and export reports based on your access." : def.Desc.TrimEnd('.') + ". Filter the report to view and export the results.",
            null));
        var groups = new List<AnyminuteReportLayout.Group>();
        foreach (var g in HubGroups)
        {
            var grp = new AnyminuteReportLayout.Group { Title = g[0] };
            foreach (var d in defs.Where(x => Array.IndexOf(g, x.Key, 1) > 0)) grp.Items.Add(new AnyminuteReportLayout.Item { Name = d.Name, Href = Href(q, d.Key, null), On = def != null && d.Key == def.Key });
            if (grp.Items.Count > 0) groups.Add(grp);
        }
        var rest = new AnyminuteReportLayout.Group { Title = "Other Reports" };
        foreach (var d in defs.Where(x => !HubGroups.Any(g => Array.IndexOf(g, x.Key, 1) > 0))) rest.Items.Add(new AnyminuteReportLayout.Item { Name = d.Name, Href = Href(q, d.Key, null), On = def != null && d.Key == def.Key });
        if (rest.Items.Count > 0) groups.Add(rest);
        sb.Append("<div class=\"am-rl-body\">").Append(AnyminuteReportLayout.Nav(groups));
        sb.Append("<div class=\"am-rl-main\" data-rp-form=\"1\" data-action=\"").Append(A(home)).Append("\">");
        if (def == null) sb.Append(AnyminuteReportLayout.Empty("No reports available", "No reports are enabled for your role in this business. A business owner can change this in Login & Access > Report Access."));
        else Preview(sb, def, q, inScope);
        sb.Append("</div></div></div>");
        return sb.ToString();
    }
    static void Preview(StringBuilder sb, AmRptDef d, AmRptQuery q, List<AmRptBiz> inScope)
    {
        if (q.Key != d.Key) { q.Key = d.Key; q.Choice.Clear(); }
        q.Action = "view"; AmRptResult r = null; string failure = null;
        bool gen = HttpContext.Current != null && HttpContext.Current.Request.QueryString["gen"] == "1";
        // v20 (Pandey #5): a report whose preview cannot run (export-only role, data error) still shows its
        // Filters + Export panel; the reason is shown where the preview would be.
        bool canView = inScope.Any(b => Perm(d, b).View);
        if (!canView) failure = "Preview is not enabled for your role on this report. Use Export PDF or Export Excel.";
        else
        {
            try { r = Run(d, q); }
            catch (HttpException ex) { failure = ex.Message; }
            catch (InvalidOperationException ex) { failure = ex.Message; }
            catch (System.Data.SqlClient.SqlException) { failure = "The report could not be loaded from the database. Try again or narrow the filters."; }
        }
        if (d.DateMode == "range")
        {
            DateTime t = Today(); if (!q.From.HasValue) q.From = new DateTime(t.Year, t.Month, 1); if (!q.To.HasValue) q.To = q.From.Value.AddMonths(1).AddDays(-1);
        }
        bool pdf = inScope.Any(b => Perm(d, b).Pdf), xls = inScope.Any(b => Perm(d, b).Excel);
        var main = new StringBuilder(); var adv = new StringBuilder(); var opts = new StringBuilder();
        string hidden = "<input type=\"hidden\" name=\"r\" value=\"" + A(d.Key) + "\" /><input type=\"hidden\" name=\"gen\" value=\"1\" />";
        string star = d.DateMode == "range" ? " <b class=\"am-rl-req\">*</b>" : "";
        if (d.DateMode != "none")
        {
            main.Append("<label><span>").Append(E(d.DateLabel)).Append(star).Append("</span><select data-rp-preset=\"1\"><option value=\"\">Custom Range</option><option value=\"this-month\">This Month</option><option value=\"last-month\">Last Month</option><option value=\"this-week\">This Week</option><option value=\"last-week\">Last Week</option><option value=\"ytd\">Year to Date</option>").Append(d.DateMode == "optional" ? "<option value=\"any\">Any date</option>" : "").Append("</select></label>");
            main.Append("<label><span>From Date").Append(star).Append("</span><input type=\"date\" name=\"from\" value=\"").Append(q.From.HasValue ? q.From.Value.ToString("yyyy-MM-dd", Inv) : "").Append("\"").Append(d.DateMode == "range" ? " required" : "").Append(" /></label>");
            main.Append("<label><span>To Date").Append(star).Append("</span><input type=\"date\" name=\"to\" value=\"").Append(q.To.HasValue ? q.To.Value.ToString("yyyy-MM-dd", Inv) : "").Append("\"").Append(d.DateMode == "range" ? " required" : "").Append(" /></label>");
        }
        // v20 (image 8): every report panel has its Business filter.
        var all = Scope();
        if (all.Count > 1)
        {
            main.Append("<label><span>Business</span><select name=\"b\"><option value=\"all\"").Append(q.Business == "all" ? " selected" : "").Append(">All Businesses</option>");
            foreach (var b in all) main.Append("<option value=\"").Append(b.Id).Append("\"").Append(q.Business == b.Id.ToString(Inv) ? " selected" : "").Append(">").Append(E(b.Name)).Append("</option>");
            main.Append("</select></label>");
        }
        else hidden += "<input type=\"hidden\" name=\"b\" value=\"" + A(q.Business) + "\" />";
        if (d.Emp)
        {
            var people = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using (var c = AnyminuteTimesheetRepair.Open()) foreach (var b in inScope) foreach (DataRow p in People(c, b).Rows) { int id = I(p["EmployeeID"]); if (Sees(b, id)) people[S(p["Name"]) + " (#" + id + ")"] = id; }
            main.Append("<label><span>Employee</span><select name=\"emp\"><option value=\"\">All Employees</option>");
            foreach (var p in people) main.Append("<option value=\"").Append(p.Value).Append("\"").Append(q.Emp == p.Value ? " selected" : "").Append(">").Append(E(p.Key)).Append("</option>");
            main.Append("</select></label>");
        }
        if (d.Proj)
        {
            main.Append("<label><span>Project</span><select name=\"proj\"><option value=\"\">All Projects</option>");
            using (var c = AnyminuteTimesheetRepair.Open()) foreach (var b in inScope)
                    foreach (DataRow p in Q(c, "SELECT ProjectID,ProjectName FROM AddProject WHERE BusinessID=@b AND ISNULL(Status,'')<>'D' ORDER BY ProjectName", "@b", b.Id).Rows)
                        main.Append("<option value=\"").Append(I(p["ProjectID"])).Append("\"").Append(q.Proj == I(p["ProjectID"]) ? " selected" : "").Append(">").Append(E(S(p["ProjectName"]) + (inScope.Count > 1 ? " \u2014 " + b.Name : ""))).Append("</option>");
            main.Append("</select></label>");
        }
        foreach (var ch in d.Choices)
        {
            List<string> o = null; if (r != null) r.Options.TryGetValue(ch[0], out o); o = o ?? new List<string>(); string cur; q.Choice.TryGetValue(ch[0], out cur);
            if (cur != null && !o.Contains(cur)) o.Add(cur);
            adv.Append("<label><span>").Append(E(ch[1])).Append("</span><select name=\"c_").Append(A(ch[0])).Append("\"><option value=\"\">All</option>");
            foreach (var x in o) adv.Append("<option").Append(x == cur ? " selected" : "").Append(">").Append(E(x)).Append("</option>");
            adv.Append("</select></label>");
        }
        if (d.Inactive) opts.Append("<label><input type=\"checkbox\" name=\"inactive\" value=\"1\"").Append(q.Inactive ? " checked" : "").Append(" /> Include inactive employees</label>");
        string clear = Href(new AmRptQuery { Key = d.Key, Business = q.Business }, d.Key, null);
        sb.Append(AnyminuteReportLayout.Filters("", hidden, main.ToString(), adv.ToString(), opts.ToString(), clear, "data-rp-go=\"1\""));
        var body = new StringBuilder();
        if (gen)
        {
            if (r == null) body.Append("<p class=\"am-rl-msg\">").Append(E(failure)).Append("</p>");
            else
            {
                body.Append("<div class=\"am-rl-kpis\">");
                foreach (var k in r.Kpis) body.Append("<div><span>").Append(E(k.Key)).Append("</span><strong>").Append(E(k.Value)).Append("</strong></div>");
                body.Append("</div>");
                foreach (var s in r.Sections)
                {
                    body.Append("<h3>").Append(E(s.Title)).Append(" <small>").Append(s.Rows.Count).Append(s.Rows.Count == 1 ? " row" : " rows").Append(s.Rows.Count > PreviewRows ? " \u2014 first " + PreviewRows + " shown; export for all" : "").Append("</small></h3>");
                    if (s.Rows.Count == 0) { body.Append(AnyminuteReportLayout.Empty("No records found", "No records match these filters.")); continue; }
                    body.Append("<div class=\"am-rl-twrap\"><table class=\"am-rl-table am-rp-table\"><thead><tr>");
                    foreach (var h in s.Cols) body.Append("<th>").Append(E(h)).Append("</th>");
                    body.Append("</tr></thead><tbody>");
                    foreach (var row in s.Rows.Take(PreviewRows))
                    {
                        body.Append("<tr>");
                        for (int i = 0; i < row.Length; i++) { string v = row[i] ?? ""; string cls = Pill(s.Cols[i], v); body.Append("<td>").Append(cls == null ? E(v) : "<span class=\"am-rp-pill " + cls + "\">" + E(v) + "</span>").Append("</td>"); }
                        body.Append("</tr>");
                    }
                    body.Append("</tbody></table></div>");
                }
                if (!String.IsNullOrEmpty(r.Note)) body.Append("<p class=\"am-rl-note\">").Append(E(r.Note)).Append("</p>");
            }
        }
        sb.Append(AnyminuteReportLayout.Preview(pdf ? "data-rp-go=\"1\" data-rp-export=\"pdf\"" : null, xls ? "data-rp-go=\"1\" data-rp-export=\"excel\"" : null, gen,
            r != null ? E(r.Subtitle) : E(d.Desc), body.ToString()));
    }
    static string Pill(string col, string v)
    {
        if (v == "" || !(col.EndsWith("Status", StringComparison.Ordinal) || col == "Attendance" || col == "Action" || col == "Login Access")) return null;
        string l = v.ToLowerInvariant();
        return l.Contains("approved") || l == "active" || l == "enabled" || l == "worked" || l == "working" || l == "current" || l.Contains("confirmed") ? "is-ok"
            : l.Contains("reject") || l.Contains("disabled") || l.Contains("revoked") || l.Contains("expired") || l == "inactive" || l.Contains("cancel") ? "is-bad"
            : l.Contains("submitted") || l.Contains("pending") || l.Contains("invited") || l.Contains("reopen") || l == "future" ? "is-warn" : "is-mute";
    }

    // ---------- Export (PDF / Excel) ----------
    public static void Export(HttpContext h, AmRptQuery q)
    {
        var d = Def(q.Key); if (d == null) throw new HttpException(404, "Unknown report.");
        var r = Run(d, q); var ms = new MemoryStream(); bool pdf = q.Action == "pdf";
        if (pdf) AnyminuteReportFiles.Pdf(r, ms); else AnyminuteReportFiles.Xlsx(r, ms);
        string file = "AnyMinute_" + d.Name.Replace("&", "and").Replace(" ", "_") + "_" + Today().ToString("yyyyMMdd", Inv) + (pdf ? ".pdf" : ".xlsx");
        h.Response.Clear(); h.Response.ContentType = pdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        h.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + file + "\""); h.Response.AddHeader("X-Content-Type-Options", "nosniff");
        h.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        h.Response.BinaryWrite(ms.ToArray()); h.Response.Flush(); h.Response.SuppressContent = true; h.ApplicationInstance.CompleteRequest();
    }

    // ---------- Report Access matrix (slide 6) ----------
    public static string MatrixHtml(Dictionary<string, AmRptPerm[]> m, bool disabled, string prefix)
    {
        var sb = new StringBuilder("<table class=\"am-rp-matrix\"><thead><tr><th rowspan=\"2\">Report Name</th>");
        foreach (var t in Tiers) sb.Append("<th colspan=\"3\" class=\"am-rp-mt\">").Append(t).Append("</th>");
        sb.Append("</tr><tr>"); for (int i = 0; i < 3; i++) sb.Append("<th>View</th><th>PDF</th><th>Excel</th>"); sb.Append("</tr></thead><tbody>");
        foreach (var d in Defs.OrderBy(x => MatrixOrder.IndexOf(x.Key)))
        {
            sb.Append("<tr><th scope=\"row\">").Append(E(d.Name)).Append("</th>");
            for (int i = 0; i < 3; i++) foreach (var a in new[] { "v", "p", "x" })
                {
                    var p = m[d.Key][i]; bool on = a == "v" ? p.View : a == "p" ? p.Pdf : p.Excel;
                    sb.Append("<td><input type=\"checkbox\" name=\"").Append(prefix).Append(d.Key).Append("_").Append(i).Append("_").Append(a).Append("\" value=\"1\"").Append(on ? " checked" : "").Append(disabled ? " disabled" : "")
                      .Append(" aria-label=\"").Append(A(d.Name + " " + Tiers[i] + " " + (a == "v" ? "View" : a == "p" ? "PDF" : "Excel"))).Append("\" /></td>");
                }
            sb.Append("</tr>");
        }
        return sb.Append("</tbody></table>").ToString();
    }
    static readonly List<string> MatrixOrder = new List<string> { "employee-master", "business-assignment", "time-card", "payroll-hours", "work-schedule", "project-cost", "approval-audit", "pay-rate", "user-access", "accountant-access" };
    public static Dictionary<string, AmRptPerm[]> ReadMatrix(System.Collections.Specialized.NameValueCollection f, string prefix)
    {
        var m = new Dictionary<string, AmRptPerm[]>();
        foreach (var d in Defs) m[d.Key] = Enumerable.Range(0, 3).Select(i => new AmRptPerm { View = f[prefix + d.Key + "_" + i + "_v"] == "1", Pdf = f[prefix + d.Key + "_" + i + "_p"] == "1", Excel = f[prefix + d.Key + "_" + i + "_x"] == "1" }).ToArray();
        return m;
    }

    // ---------- Login & Access list (t77u): employees of businesses where the user is owner/administrator ----------
    public static List<string[]> LoginRows(int business)
    {
        var rows = new List<string[]>();
        using (var c = AnyminuteTimesheetRepair.Open())
            foreach (var b in Scope().Where(x => x.Tier == 2 && (business <= 0 || x.Id == business)))
            {
                var logins = Logins(c, b);
                foreach (DataRow p in People(c, b).Rows)
                {
                    int e = I(p["EmployeeID"]); Login l; if (!logins.TryGetValue(e, out l)) l = new Login();
                    rows.Add(new[] { e.ToString(Inv), S(p["Name"]), l.Email != "" ? l.Email : S(p["Email"]), b.Name, Tier(I(p["UserTypeId"]), I(p["Manages"]) == 1), S(p["IsActive"]).ToUpperInvariant() == "NO" ? "Inactive" : "Active", l.Status, l.LastLogin == "" ? "Never" : l.LastLogin, l.Mfa == "" ? "\u2014" : l.Mfa, b.Id.ToString(Inv) });
                }
            }
        return rows.OrderBy(x => x[1]).ThenBy(x => x[3]).ToList();
    }
    public static string LoginHtml(List<string[]> rows)
    {
        var sb = new StringBuilder("<table class=\"am-rp-table am-la-table\" id=\"LoginAccessTable\"><thead><tr><th>Employee</th><th>Login Email</th><th>Business</th><th>Role</th><th>Employment</th><th>Login Status</th><th>Last Login</th><th>MFA</th><th>Action</th></tr></thead><tbody>");
        if (rows.Count == 0) sb.Append("<tr><td colspan=\"9\" class=\"am-rp-empty\">No employees found.</td></tr>");
        foreach (var r in rows)
            sb.Append("<tr data-rp-text=\"").Append(A((r[1] + " " + r[2] + " " + r[3]).ToLowerInvariant())).Append("\"><td><strong>").Append(E(r[1])).Append("</strong><small> #").Append(r[0]).Append("</small></td><td>").Append(E(r[2])).Append("</td><td>").Append(E(r[3])).Append("</td><td>").Append(E(r[4]))
              .Append("</td><td><span class=\"am-rp-pill ").Append(Pill("Status", r[5])).Append("\">").Append(r[5]).Append("</span></td><td><span class=\"am-rp-pill ").Append(Pill("Status", r[6])).Append("\">").Append(E(r[6])).Append("</span></td><td>").Append(E(r[7])).Append("</td><td>").Append(E(r[8]))
              .Append("</td><td><a class=\"am-rp-btn am-rp-small\" href=\"").Append(A(VirtualPathUtility.ToAbsolute("~/EmployeeEditor.aspx") + "?EmployeeId=" + r[0] + "&BusinessId=" + r[9] + "&tab=access")).Append("\">Manage Login &amp; Access</a></td></tr>");
        return sb.Append("</tbody></table>").ToString();
    }
}
