using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;
using SR = AnyminuteSupportReports;

// 2026-10-29 v31: all 20 reports share the Support Ticket filter layout (date filters first as Date Range / From / To cells).
// 2026-10-29 v30: Report Filters card per the Support Ticket mock (range3 = Date Range / From / To cells).
// 2026-10-29 v28 (Shamsu deck 1 slides 1-2, deck 2 slides 3-4): Support Admin Reports hub.
// Left list of the 12 Support reports and 8 Reseller reports; each report has the deck layout: title + instruction,
// Export PDF / Export Excel (top right), Filters card with Clear All, Additional Options, Generate Report, then KPIs + preview.
// Filters travel in the query string (?r=key&f_x=..&o_y=1&gen=1) so a generated report can be bookmarked and exported.
public partial class AnyminuteSupportReportsPage : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        var q = Request.QueryString; var def = SR.Def(q["r"]);
        if (q["export"] == "pdf" || q["export"] == "excel") { SR.Export(Context, SR.Execute(def, q), q["export"] == "pdf"); return; }
        Title = def.Name + " Report";
        Nav.Text = NavList(def);
        bool gen = q["gen"] == "1"; var run = SR.Execute(def, gen ? q : new NameValueCollection { { "r", def.Key } });
        var sb = new StringBuilder();
        sb.Append("<header class='sa-head'><div><h1>").Append(SA.H(def.Name)).Append(" Report</h1><p>").Append(SA.H(def.Purpose.TrimEnd('.') + ".")).Append(" Select filters to generate the report.</p></div>")
          .Append("<div class='sa-rpt-export'><button type='button' class='sa-btn sa-btn-outline' data-sa-apply='1' data-sa-extra='export=pdf' data-sa-form='sa-rpt-form'>&#128196; Export PDF</button>")
          .Append("<button type='button' class='sa-btn sa-btn-outline' data-sa-apply='1' data-sa-extra='export=excel' data-sa-form='sa-rpt-form'>&#128200; Export Excel</button></div></header>");
        // v30 (Pandey post-v29, Support Ticket mock): "Report Filters" card (funnel icon, Clear All button), 3-column filter grid,
        // Additional Options band with gear icon and Generate Report bottom-left. Same layout for every report in the hub.
        sb.Append(FilterCard(def, gen ? q : new NameValueCollection(), run));
        if (run.Error != null) sb.Append("<div class='sa-flash sa-warn'>").Append(SA.H(run.Error)).Append("</div>");
        if (!gen) sb.Append("<div class='sa-card sa-rpt-hint'><p class='sa-muted'>Choose filters and click <strong>Generate Report</strong> to preview the results. Exports use the same filters.</p></div>");
        else sb.Append(Result(run));
        Body1.Text = sb.ToString();
    }
    // v31: one shared Report Filters card for all 20 reports (also used by the offline layout check).
    public static string FilterCard(SrDef def, NameValueCollection q, SrRun run)
    {
        var sb = new StringBuilder();
        sb.Append("<div class='sa-card sa-rpt-card' id='sa-rpt-form' data-sa-get='SupportReports.aspx' data-sa-noauto='1'><input type='hidden' name='r' value='").Append(SA.A(def.Key)).Append("' /><input type='hidden' name='gen' value='1' />")
          .Append("<div class='sa-rpt-cardhead'><h2><i class='sa-rpt-ico' aria-hidden='true'>&#9660;</i> Report Filters</h2><a class='sa-btn sa-btn-outline sa-btn-sm' href='SupportReports.aspx?r=").Append(HttpUtility.UrlEncode(def.Key)).Append("'>&#8635; Clear All</a></div><div class='sa-rpt-grid'>");
        foreach (var f in def.Filters.OrderBy(x => x.Type.StartsWith("range") ? 0 : 1)) sb.Append(Filter(f, q, run.Base));
        sb.Append("</div>");
        if (def.Options.Length > 0)
        {
            sb.Append("<div class='sa-rpt-optband'><h3><i class='sa-rpt-ico' aria-hidden='true'>&#9881;</i> Additional Options</h3><div class='sa-rpt-opts'>");
            foreach (var o in def.Options) sb.Append("<label class='sa-check'><input type='checkbox' name='o_").Append(o[0]).Append("' value='1'").Append(run.On.Contains(o[0]) ? " checked" : "").Append(" /> ").Append(SA.H(o[1])).Append("</label>");
            sb.Append("</div></div>");
        }
        sb.Append("<div class='sa-rpt-go'><button type='button' class='sa-btn' data-sa-apply='1'>&#128269; Generate Report</button></div></div>");
        return sb.ToString();
    }
    static string NavList(SrDef cur)
    {
        var sb = new StringBuilder("<aside class='sa-rpt-nav'><nav aria-label='Reports'>");
        foreach (var g in new[] { new[] { "Support", "Support Admin Reports" }, new[] { "Reseller", "Reseller Reports" } })
        {
            sb.Append("<h4>").Append(g[1]).Append("</h4><ol>");
            foreach (var d in SR.Defs.Where(x => x.Group == g[0]))
                sb.Append("<li><a href='SupportReports.aspx?r=").Append(d.Key).Append("'").Append(d == cur ? " class='on' aria-current='page'" : "").Append(">").Append(SA.H(d.Name)).Append("</a></li>");
            sb.Append("</ol>");
        }
        return sb.Append("</nav></aside>").ToString();
    }
    static string Filter(SrFilter f, NameValueCollection q, SrData all)
    {
        string n = "f_" + f.Key, v = q[n] ?? "";
        var sb = new StringBuilder("<label class='sa-field'><span>").Append(SA.H(f.Label)).Append("</span>");
        if (f.Type == "range3" || f.Type == "range")
        {
            // v31 (Pandey post-v30): every date filter uses the Support Ticket layout - three grid cells (Date Range / From Date / To Date).
            // range3 (Support Ticket) is required and defaults to Custom Range, this year to today; range (other reports) defaults to All Dates.
            var t = DateTime.Today; bool req = f.Type == "range3", fresh = q[n + "_preset"] == null;
            string p = fresh ? (req ? "Custom Range" : "All Dates") : q[n + "_preset"];
            string a = fresh ? (req ? new DateTime(t.Year, 1, 1).ToString("yyyy-MM-dd") : "") : q[n + "_from"], b = fresh ? (req ? t.ToString("yyyy-MM-dd") : "") : q[n + "_to"];
            string star = req ? " <b class='sa-req'>*</b>" : "";
            sb = new StringBuilder("<label class='sa-field'><span>").Append(SA.H(f.Label)).Append(star).Append("</span><select name='").Append(n).Append("_preset'>");
            foreach (var r in SR.Ranges) sb.Append("<option").Append(r == p ? " selected" : "").Append(">").Append(r).Append("</option>");
            sb.Append("</select></label><label class='sa-field'><span>From Date").Append(star).Append("</span><input type='date' name='").Append(n).Append("_from' value='").Append(SA.A(a)).Append("' /></label>")
              .Append("<label class='sa-field'><span>To Date").Append(star).Append("</span><input type='date' name='").Append(n).Append("_to' value='").Append(SA.A(b)).Append("' /><small class='sa-muted'>From / To apply with Custom Range.</small>");
            return sb.Append("</label>").ToString();
        }
        else if (f.Type == "text") sb.Append("<input type='search' name='").Append(n).Append("' value='").Append(SA.A(v)).Append("' placeholder='Search ").Append(SA.A(f.Label.ToLowerInvariant())).Append("...' />");
        else if (f.Type == "param")
        {
            if (v.Length == 0) v = f.Fixed[0];
            sb.Append("<select name='").Append(n).Append("'>");
            foreach (var o in f.Fixed) sb.Append("<option").Append(o == v ? " selected" : "").Append(">").Append(SA.H(o)).Append("</option>");
            sb.Append("</select>");
        }
        else if (f.Type == "min")
        {
            sb.Append("<select name='").Append(n).Append("'><option value=''>Any usage</option>");
            foreach (var o in f.Fixed) sb.Append("<option value='").Append(o).Append("'").Append(o == v ? " selected" : "").Append(">").Append(o).Append("% or more</option>");
            sb.Append("</select>");
        }
        else sb.Append("<select name='").Append(n).Append("'>").Append(SA.Options(SR.OptionValues(f, all), v, "All")).Append("</select>");
        return sb.Append("</label>").ToString();
    }
    static string Result(SrRun run)
    {
        var sb = new StringBuilder();
        if (run.Kpis.Count > 0)
        {
            sb.Append("<div class='sa-kpis sa-rpt-kpis'>");
            foreach (var k in run.Kpis) sb.Append("<div class='sa-kpi'><div><span>").Append(SA.H(k.Key)).Append("</span><strong>").Append(SA.H(k.Value)).Append("</strong></div></div>");
            sb.Append("</div>");
        }
        var vis = run.Visible(); var d = run.Data;
        sb.Append("<div class='sa-card'><div class='sa-rpt-cardhead'><h2>Report Preview</h2><span class='sa-muted'>").Append(d.Rows.Count).Append(" record(s)")
          .Append(run.Applied.Count == 0 ? " &middot; no filters" : " &middot; " + SA.H(String.Join("; ", run.Applied.ToArray()))).Append("</span></div>");
        if (!String.IsNullOrEmpty(d.Note)) sb.Append("<p class='sa-muted sa-small'>").Append(SA.H(d.Note)).Append("</p>");
        if (d.Rows.Count == 0) return sb.Append("<p class='sa-rpt-empty'>No records match these filters.</p></div>").ToString();
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr>");
        foreach (int i in vis) sb.Append("<th>").Append(SA.H(d.Cols[i])).Append("</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var r in d.Rows.Take(SR.PreviewRows)) { sb.Append("<tr>"); foreach (int i in vis) sb.Append("<td>").Append(SA.H(r[i])).Append("</td>"); sb.Append("</tr>"); }
        sb.Append("</tbody></table></div>");
        if (d.Rows.Count > SR.PreviewRows) sb.Append("<p class='sa-muted sa-small'>Showing the first ").Append(SR.PreviewRows).Append(" rows. Export PDF or Excel for all ").Append(d.Rows.Count).Append(".</p>");
        return sb.Append("</div>").ToString();
    }
}
