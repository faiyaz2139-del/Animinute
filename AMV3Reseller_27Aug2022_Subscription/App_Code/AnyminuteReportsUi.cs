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
    public static string Hub(AmRptQuery q)
    {
        var scope = Scope(); var sb = new StringBuilder();
        if (scope.Count == 0) return "<p class=\"am-rp-empty\">You are not assigned to an active business yet, so there are no reports to show.</p>";
        if (q.Business != "all" && q.Business != "" && !scope.Any(b => b.Id.ToString(Inv) == q.Business)) q.Business = "";
        if (q.Business == "") q.Business = scope.Count == 1 ? scope[0].Id.ToString(Inv) : "all";
        var inScope = scope.Where(b => q.Business == "all" || b.Id.ToString(Inv) == q.Business).ToList();
        var defs = Available(q.Business);
        var tiers = inScope.Select(b => b.Tier).Distinct().OrderByDescending(t => t).Select(t => Tiers[t]).ToList();
        // Header: title, Selected Business, Role Context (slide 1).
        sb.Append("<header class=\"am-rp-head\"><div><h1>Reports</h1><p>View, filter, preview and export reports based on your access.</p></div><div data-rp-form=\"1\" class=\"am-rp-context\" data-action=\"").Append(A(VirtualPathUtility.ToAbsolute("~/Reports.aspx"))).Append("\">");
        if (q.Key != "") sb.Append("<input type=\"hidden\" name=\"r\" value=\"").Append(A(q.Key)).Append("\" />");
        sb.Append("<label><span>Selected Business</span><select name=\"b\" data-rp-autosubmit=\"1\">");
        if (scope.Count > 1) sb.Append("<option value=\"all\"").Append(q.Business == "all" ? " selected" : "").Append(">All my businesses</option>");
        foreach (var b in scope) sb.Append("<option value=\"").Append(b.Id).Append("\"").Append(q.Business == b.Id.ToString(Inv) ? " selected" : "").Append(">").Append(E(b.Name)).Append("</option>");
        sb.Append("</select></label><label><span>Role Context</span><span class=\"am-rp-role\">").Append(E(String.Join(" / ", tiers))).Append("</span></label></div></header>");
        sb.Append("<div class=\"am-rp-banner\" role=\"note\"><strong>i</strong><div>The reports shown below update automatically based on the selected business and your user role permissions.<br/><small>Switch business to see the reports available to you there.</small></div></div>");
        sb.Append("<div class=\"am-rp-grid\"><section class=\"am-rp-card am-rp-list\"><div class=\"am-rp-card-head\"><h2>Available Reports (").Append(defs.Count).Append(")</h2><input type=\"search\" placeholder=\"Search reports...\" data-rp-search=\"1\" aria-label=\"Search reports\" /></div>");
        if (defs.Count == 0) sb.Append("<p class=\"am-rp-empty\">No reports are enabled for your role in this business. A business owner can change this in Login &amp; Access &rsaquo; Report Access.</p>");
        foreach (var d in defs)
        {
            var biz = inScope.Where(b => Perm(d, b).Any).ToList();
            bool pdf = biz.Any(b => Perm(d, b).Pdf), xls = biz.Any(b => Perm(d, b).Excel);
            var m = inScope.Count > 0 ? MatrixFor(inScope[0])[d.Key] : Defaults()[d.Key];
            string pick = Href(q, d.Key, null) + "#rp-preview";
            sb.Append("<article class=\"am-rp-item").Append(d.Key == q.Key ? " is-active" : "").Append("\" data-rp-href=\"").Append(A(pick)).Append("\" tabindex=\"0\" role=\"button\" aria-pressed=\"").Append(d.Key == q.Key ? "true" : "false").Append("\" data-rp-text=\"").Append(A((d.Name + " " + d.Desc).ToLowerInvariant())).Append("\">");
            sb.Append("<span class=\"am-rp-ic am-rp-ic-").Append(d.Icon).Append("\">").Append(Icon(d.Icon)).Append("</span>");
            sb.Append("<a class=\"am-rp-name\" href=\"" + A(pick) + "\">" + E(d.Name) + "</a>"); // v20: export-only reports open their filters too
            sb.Append("<p>").Append(E(d.Desc)).Append("</p><span class=\"am-rp-roles\">");
            for (int i = 0; i < 3; i++) if (m[i].Any) sb.Append("<span class=\"am-rp-badge am-rp-t").Append(i).Append("\">").Append(Tiers[i]).Append("</span>");
            sb.Append("</span><span class=\"am-rp-acts\">");
            sb.Append(pdf ? "<a class=\"am-rp-pdf\" href=\"" + A(Href(q, d.Key, "pdf")) + "\">PDF</a>" : "<span class=\"am-rp-pdf is-off\" title=\"PDF export is not enabled for your role\">PDF</span>");
            sb.Append(xls ? "<a class=\"am-rp-xls\" href=\"" + A(Href(q, d.Key, "excel")) + "\">Excel</a>" : "<span class=\"am-rp-xls is-off\" title=\"Excel export is not enabled for your role\">Excel</span>");
            sb.Append("</span></article>");
        }
        var def = defs.FirstOrDefault(x => x.Key == q.Key) ?? defs.FirstOrDefault(x => inScope.Any(b => Perm(x, b).View)) ?? defs.FirstOrDefault();
        sb.Append("</section><section class=\"am-rp-card am-rp-preview\" id=\"rp-preview\" tabindex=\"-1\" aria-live=\"polite\" data-rp-key=\"").Append(def == null ? "" : A(def.Key)).Append("\">");
        if (def == null) sb.Append("<h2>Report Filters &amp; Preview</h2><p class=\"am-rp-empty\">Select a report to preview it.</p>");
        else Preview(sb, def, q, inScope);
        sb.Append("</section></div>");
        sb.Append("<footer class=\"am-rp-principles\"><div><strong>Single Reports Module</strong><span>One unified place for all your reporting needs.</span></div><div><strong>Reports depend on selected business</strong><span>Only reports for the chosen business are shown.</span></div><div><strong>Reports depend on user role</strong><span>Each user sees only the reports they have permission to access.</span></div><div><strong>One export system for PDF and Excel</strong><span>All reports use the same export system.</span></div><div><strong>Supports multi-business</strong><span>Works across multiple businesses in one system.</span></div></footer>");
        return sb.ToString();
    }
    static void Preview(StringBuilder sb, AmRptDef d, AmRptQuery q, List<AmRptBiz> inScope)
    {
        if (q.Key != d.Key) { q.Key = d.Key; q.Choice.Clear(); }
        q.Action = "view"; AmRptResult r = null; string failure = null;
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
        if (r == null && d.DateMode == "range")
        {
            DateTime t = Today(); if (!q.From.HasValue) q.From = new DateTime(t.Year, t.Month, 1); if (!q.To.HasValue) q.To = q.From.Value.AddMonths(1).AddDays(-1);
        }
        bool pdf = inScope.Any(b => Perm(d, b).Pdf), xls = inScope.Any(b => Perm(d, b).Excel);
        sb.Append("<div class=\"am-rp-card-head\"><h2>Report Filters &amp; Preview</h2><span class=\"am-rp-chip\">").Append(E(d.Name)).Append("</span></div>");
        sb.Append("<p class=\"am-rp-desc\">").Append(E(d.Desc)).Append("</p>"); // v21: which report these filters belong to
        sb.Append("<div data-rp-form=\"1\" class=\"am-rp-filters\" data-action=\"").Append(A(VirtualPathUtility.ToAbsolute("~/Reports.aspx"))).Append("\"><input type=\"hidden\" name=\"r\" value=\"").Append(A(d.Key)).Append("\" />");
        // v20 (image 8): every report panel starts with its Business filter.
        var all = Scope();
        if (all.Count > 1)
        {
            sb.Append("<label><span>Business</span><select name=\"b\"><option value=\"all\"").Append(q.Business == "all" ? " selected" : "").Append(">All Businesses</option>");
            foreach (var b in all) sb.Append("<option value=\"").Append(b.Id).Append("\"").Append(q.Business == b.Id.ToString(Inv) ? " selected" : "").Append(">").Append(E(b.Name)).Append("</option>");
            sb.Append("</select></label>");
        }
        else sb.Append("<input type=\"hidden\" name=\"b\" value=\"").Append(A(q.Business)).Append("\" />");
        if (d.DateMode != "none")
        {
            sb.Append("<label><span>").Append(E(d.DateLabel)).Append(" from</span><input type=\"date\" name=\"from\" value=\"").Append(q.From.HasValue ? q.From.Value.ToString("yyyy-MM-dd", Inv) : "").Append("\"").Append(d.DateMode == "range" ? " required" : "").Append(" /></label>");
            sb.Append("<label><span>to</span><input type=\"date\" name=\"to\" value=\"").Append(q.To.HasValue ? q.To.Value.ToString("yyyy-MM-dd", Inv) : "").Append("\"").Append(d.DateMode == "range" ? " required" : "").Append(" /></label>");
            sb.Append("<label><span>Reporting Period</span><select data-rp-preset=\"1\"><option value=\"\">Custom</option><option value=\"this-month\">This Month</option><option value=\"last-month\">Last Month</option><option value=\"this-week\">This Week</option><option value=\"last-week\">Last Week</option><option value=\"ytd\">Year to Date</option>").Append(d.DateMode == "optional" ? "<option value=\"any\">Any date</option>" : "").Append("</select></label>");
        }
        if (d.Emp)
        {
            var people = new SortedDictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using (var c = AnyminuteTimesheetRepair.Open()) foreach (var b in inScope) foreach (DataRow p in People(c, b).Rows) { int id = I(p["EmployeeID"]); if (Sees(b, id)) people[S(p["Name"]) + " (#" + id + ")"] = id; }
            sb.Append("<label><span>Employee</span><select name=\"emp\"><option value=\"\">All Employees</option>");
            foreach (var p in people) sb.Append("<option value=\"").Append(p.Value).Append("\"").Append(q.Emp == p.Value ? " selected" : "").Append(">").Append(E(p.Key)).Append("</option>");
            sb.Append("</select></label>");
        }
        if (d.Proj)
        {
            sb.Append("<label><span>Project</span><select name=\"proj\"><option value=\"\">All Projects</option>");
            using (var c = AnyminuteTimesheetRepair.Open()) foreach (var b in inScope)
                    foreach (DataRow p in Q(c, "SELECT ProjectID,ProjectName FROM AddProject WHERE BusinessID=@b AND ISNULL(Status,'')<>'D' ORDER BY ProjectName", "@b", b.Id).Rows)
                        sb.Append("<option value=\"").Append(I(p["ProjectID"])).Append("\"").Append(q.Proj == I(p["ProjectID"]) ? " selected" : "").Append(">").Append(E(S(p["ProjectName"]) + (inScope.Count > 1 ? " \u2014 " + b.Name : ""))).Append("</option>");
            sb.Append("</select></label>");
        }
        foreach (var ch in d.Choices)
        {
            List<string> opts = null; if (r != null) r.Options.TryGetValue(ch[0], out opts); opts = opts ?? new List<string>(); string cur; q.Choice.TryGetValue(ch[0], out cur);
            if (cur != null && !opts.Contains(cur)) opts.Add(cur);
            sb.Append("<label><span>").Append(E(ch[1])).Append("</span><select name=\"c_").Append(A(ch[0])).Append("\"><option value=\"\">All</option>");
            foreach (var o in opts) sb.Append("<option").Append(o == cur ? " selected" : "").Append(">").Append(E(o)).Append("</option>");
            sb.Append("</select></label>");
        }
        if (d.Inactive) sb.Append("<label class=\"am-rp-check\"><input type=\"checkbox\" name=\"inactive\" value=\"1\"").Append(q.Inactive ? " checked" : "").Append(" /> Include inactive employees</label>");
        sb.Append("<div class=\"am-rp-buttons\"><button type=\"button\" data-rp-go=\"1\" class=\"am-rp-btn am-rp-primary\">Preview Report</button>");
        if (pdf) sb.Append("<button type=\"button\" data-rp-go=\"1\" data-rp-export=\"pdf\" class=\"am-rp-btn am-rp-btn-pdf\">Export PDF</button>");
        if (xls) sb.Append("<button type=\"button\" data-rp-go=\"1\" data-rp-export=\"excel\" class=\"am-rp-btn am-rp-btn-xls\">Export Excel</button>");
        sb.Append("</div></div>");
        sb.Append("<div class=\"am-rp-card-head am-rp-preview-head\"><h3>Report Preview</h3><small>Live data for the filters above</small></div>");
        if (r == null) { sb.Append("<p class=\"am-rp-empty\">").Append(E(failure)).Append("</p>"); return; }
        sb.Append("<p class=\"am-rp-sub\">").Append(E(r.Subtitle)).Append("</p><div class=\"am-rp-kpis\">");
        foreach (var k in r.Kpis) sb.Append("<div><span>").Append(E(k.Key)).Append("</span><strong>").Append(E(k.Value)).Append("</strong></div>");
        sb.Append("</div>");
        foreach (var s in r.Sections)
        {
            sb.Append("<h3>").Append(E(s.Title)).Append(" <small>").Append(s.Rows.Count).Append(s.Rows.Count == 1 ? " row" : " rows").Append(s.Rows.Count > PreviewRows ? " \u2014 first " + PreviewRows + " shown; export for all" : "").Append("</small></h3>");
            if (s.Rows.Count == 0) { sb.Append("<p class=\"am-rp-empty\">No records match these filters.</p>"); continue; }
            sb.Append("<div class=\"am-rp-table-wrap\"><table class=\"am-rp-table\"><thead><tr>");
            foreach (var h in s.Cols) sb.Append("<th>").Append(E(h)).Append("</th>");
            sb.Append("</tr></thead><tbody>");
            foreach (var row in s.Rows.Take(PreviewRows))
            {
                sb.Append("<tr>");
                for (int i = 0; i < row.Length; i++) { string v = row[i] ?? ""; string cls = Pill(s.Cols[i], v); sb.Append("<td>").Append(cls == null ? E(v) : "<span class=\"am-rp-pill " + cls + "\">" + E(v) + "</span>").Append("</td>"); }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table></div>");
        }
        if (!String.IsNullOrEmpty(r.Note)) sb.Append("<p class=\"am-rp-note\">").Append(E(r.Note)).Append("</p>");
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
