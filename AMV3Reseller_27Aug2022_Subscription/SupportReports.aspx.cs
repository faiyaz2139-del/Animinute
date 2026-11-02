using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;
using SR = AnyminuteSupportReports;
using RL = AnyminuteReportLayout;

// 2026-11-01 v46: Reports deck - shared AnyminuteReportLayout (breadcrumb, searchable nav, Advanced Filters & Options, Report Preview card with exports).
// 2026-10-29 v31: all 20 reports share the Support Ticket filter layout (date filters first as Date Range / From / To cells).
// 2026-10-29 v30: Report Filters card per the Support Ticket mock (range3 = Date Range / From / To cells).
// 2026-10-29 v28 (Shamsu deck 1 slides 1-2, deck 2 slides 3-4): Support Admin Reports hub.
// Left list of the 12 Support reports and 8 Reseller reports; each report has the deck layout: title + instruction,
// Export PDF / Export Excel (top right), Filters card with Clear All, Additional Options, Generate Report, then KPIs + preview.
// Filters travel in the query string (?r=key&f_x=..&o_y=1&gen=1) so a generated report can be bookmarked and exported.
public partial class AnyminuteSupportReportsPage : System.Web.UI.Page
{
    // v49 (Pandey notes): Support Admin = all 20 reports; a reseller login with report permission = the 8 Reseller reports (own rows).
    protected void Page_Init(object sender, EventArgs e) { if (Session["UserId"] == null || !SR.CanOpen()) SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        var q = Request.QueryString; var scope = SR.ResellerScope(); var def = SR.DefFor(scope, q["r"]);
        if (q["export"] == "pdf" || q["export"] == "excel") { SR.Export(Context, SR.Execute(def, q), q["export"] == "pdf"); return; }
        Title = def.Name + " Report";
        // v46 (Reports deck): shared report layout (AnyminuteReportLayout) - same design as Reports.aspx in the other modules.
        Head1.Text = RL.Head(def.Group == "Reseller" ? "Reseller Reports" : "Support Admin Reports", "SupportReports.aspx", def.Name, def.Purpose.TrimEnd('.') + ". Filter the report to view and export the results.", null);
        Nav.Text = NavList(def, scope);
        bool gen = q["gen"] == "1"; var run = SR.Execute(def, gen ? q : new NameValueCollection { { "r", def.Key } });
        var sb = new StringBuilder();
        sb.Append(FilterCard(def, gen ? q : new NameValueCollection(), run));
        if (run.Error != null) sb.Append("<div class='am-rl-msg'>").Append(SA.H(run.Error)).Append("</div>");
        sb.Append(RL.Preview("data-sa-form='sa-rpt-form' data-sa-extra='export=pdf'", "data-sa-form='sa-rpt-form' data-sa-extra='export=excel'", gen, gen ? Summary(run) : null, gen ? Result(run) : null));
        Body1.Text = sb.ToString();
    }
    // v46: Report Filters card - date range + the first filters on the card, the rest with the options under
    // "Advanced Filters & Options" (Support Ticket mock). Also used by the offline layout check.
    public static string FilterCard(SrDef def, NameValueCollection q, SrRun run)
    {
        var main = new StringBuilder(); var adv = new StringBuilder(); int n = 0;
        foreach (var f in def.Filters.OrderBy(x => x.Type.StartsWith("range") ? 0 : 1))
        {
            bool range = f.Type.StartsWith("range");
            if (range || n++ < 4) main.Append(Filter(f, q, run.Base)); else adv.Append(Filter(f, q, run.Base));
        }
        var opts = new StringBuilder();
        foreach (var o in def.Options) opts.Append("<label><input type='checkbox' name='o_").Append(o[0]).Append("' value='1'").Append(run.On.Contains(o[0]) ? " checked" : "").Append(" /> ").Append(SA.H(o[1])).Append("</label>");
        return RL.Filters("id='sa-rpt-form' data-sa-get='SupportReports.aspx' data-sa-noauto='1'",
            "<input type='hidden' name='r' value='" + SA.A(def.Key) + "' /><input type='hidden' name='gen' value='1' />",
            main.ToString(), adv.ToString(), opts.ToString(), "SupportReports.aspx?r=" + HttpUtility.UrlEncode(def.Key), "data-sa-apply='1'");
    }
    static string NavList(SrDef cur, SrScope scope)
    {
        var groups = new List<RL.Group>();
        foreach (var g in new[] { new[] { "Support", "Support Admin Reports" }, new[] { "Reseller", "Reseller Reports" } })
        {
            if (scope != null && g[0] != "Reseller") continue;
            var grp = new RL.Group { Title = g[1] };
            foreach (var d in SR.DefsFor(scope).Where(x => x.Group == g[0])) grp.Items.Add(new RL.Item { Name = d.Name, Href = "SupportReports.aspx?r=" + d.Key, On = d == cur });
            groups.Add(grp);
        }
        return RL.Nav(groups);
    }
    static string Summary(SrRun run)
    {
        return run.Data.Rows.Count + " record(s)" + (run.Applied.Count == 0 ? " &middot; no filters" : " &middot; " + SA.H(String.Join("; ", run.Applied.ToArray())));
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
              .Append("<label class='sa-field'><span>To Date").Append(star).Append("</span><input type='date' name='").Append(n).Append("_to' value='").Append(SA.A(b)).Append("' />");
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
            sb.Append("<div class='am-rl-kpis'>");
            foreach (var k in run.Kpis) sb.Append("<div><span>").Append(SA.H(k.Key)).Append("</span><strong>").Append(SA.H(k.Value)).Append("</strong></div>");
            sb.Append("</div>");
        }
        var vis = run.Visible(); var d = run.Data;
        if (!String.IsNullOrEmpty(d.Note)) sb.Append("<p class='am-rl-small'>").Append(SA.H(d.Note)).Append("</p>");
        if (d.Rows.Count == 0) return sb.Append(RL.Empty("No records found", "No records match these filters. Change the filters and click Generate Report again.")).ToString();
        sb.Append("<div class='am-rl-twrap'><table class='am-rl-table'><thead><tr>");
        foreach (int i in vis) sb.Append("<th>").Append(SA.H(d.Cols[i])).Append("</th>");
        sb.Append("</tr></thead><tbody>");
        foreach (var r in d.Rows.Take(SR.PreviewRows)) { sb.Append("<tr>"); foreach (int i in vis) sb.Append("<td>").Append(SA.H(r[i])).Append("</td>"); sb.Append("</tr>"); }
        sb.Append("</tbody></table></div>");
        if (d.Rows.Count > SR.PreviewRows) sb.Append("<p class='am-rl-small'>Showing the first ").Append(SR.PreviewRows).Append(" rows. Export PDF or Excel for all ").Append(d.Rows.Count).Append(".</p>");
        return sb.ToString();
    }
}
