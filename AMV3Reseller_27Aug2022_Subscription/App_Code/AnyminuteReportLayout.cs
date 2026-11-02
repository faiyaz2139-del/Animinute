using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

// 2026-11-01 v46 (Reports deck "Please change all report similar - across all module", Support Ticket mock):
// ONE report page layout shared by Support Admin Reports (SupportReports.aspx) and the Business Owner / Manager /
// Employee / Accountant Reports hub (Reports.aspx): breadcrumb + icon title, left "Search reports..." list with
// collapsible groups, "Report Filters" card (funnel band) with an "Advanced Filters & Options" collapsible band,
// Clear Filters + Generate Report, then a separate "Report Preview" card with Export PDF / Export Excel and the
// "No report generated" empty state. Styles: css/AnyminuteReportLayout.css (scoped to .am-rl). Each page keeps its
// own GET/filter JavaScript; this class only builds the markup.
public static class AnyminuteReportLayout
{
    public sealed class Item { public string Name, Href; public bool On; }
    public sealed class Group { public string Title; public List<Item> Items = new List<Item>(); }
    static string E(string s) { return HttpUtility.HtmlEncode(s ?? ""); }
    static string A(string s) { return HttpUtility.HtmlAttributeEncode(s ?? ""); }
    const string DocIcon = "<svg viewBox='0 0 24 24' aria-hidden='true'><path d='M6 2h9l5 5v15H6zM14 2v6h6M9 13h8M9 17h6' fill='none' stroke='currentColor' stroke-width='1.8' stroke-linejoin='round'/></svg>";
    const string Funnel = "<svg viewBox='0 0 24 24' aria-hidden='true'><path d='M3 4h18l-7 8v6l-4 2v-8z' fill='currentColor'/></svg>";
    const string Sliders = "<svg viewBox='0 0 24 24' aria-hidden='true'><path d='M4 7h10M18 7h2M4 17h4M12 17h8' stroke='currentColor' stroke-width='2' stroke-linecap='round'/><circle cx='16' cy='7' r='2' fill='none' stroke='currentColor' stroke-width='2'/><circle cx='10' cy='17' r='2' fill='none' stroke='currentColor' stroke-width='2'/></svg>";
    const string Chevron = "<svg class='am-rl-chev' viewBox='0 0 24 24' aria-hidden='true'><path d='M6 15l6-6 6 6' fill='none' stroke='currentColor' stroke-width='2.2' stroke-linecap='round' stroke-linejoin='round'/></svg>";
    const string Ticket = "<svg viewBox='0 0 24 24' aria-hidden='true'><path d='M5 4h11l3 3v13H5z' fill='none' stroke='currentColor' stroke-width='2' stroke-linejoin='round'/><path d='M8 11h8M8 15h5M15 4v4h4' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round'/></svg>";

    // v48 (deckR "Please change all report similar - across all module"; NOTES_v46 deferred): the old report pages
    // (Company_Report, Daily_Report, Employee_Report, Project_Report, Report3, My_Time_Report) are still reachable from
    // DB menu links. When the signed-in user has the Reports hub (Reports.aspx, built on this shared layout), a GET to an
    // old page opens the matching hub report instead, so every report screen has the same layout. Users without hub
    // reports keep the old page; ?legacy=1 keeps the old page reachable. Postbacks are never redirected.
    public static void LegacyRedirect(string key)
    {
        var h = HttpContext.Current;
        if (h == null || h.Session == null || h.Session["EmployeeId"] == null) return;
        if (!String.Equals(h.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) || h.Request.QueryString["legacy"] == "1") return;
        bool hub = false;
        try { hub = AnyminuteReports.AnyAvailable(); } catch (Exception) { hub = false; }
        if (!hub) return;
        h.Response.Redirect("~/Reports.aspx" + (AnyminuteReports.Def(key) != null ? "?r=" + HttpUtility.UrlEncode(key) : ""), true);
    }
    public static string Head(string crumbRoot, string crumbHref, string reportName, string subtitle, string extraHtml)
    {
        var sb = new StringBuilder("<header class='am-rl-head'><span class='am-rl-tile'>").Append(Ticket).Append("</span><div class='am-rl-titles'>");
        sb.Append("<nav class='am-rl-crumb' aria-label='Breadcrumb'><a href='").Append(A(crumbHref)).Append("'>").Append(E(crumbRoot)).Append("</a><span aria-hidden='true'>/</span><span>").Append(E(reportName)).Append("</span></nav>");
        string title = reportName.EndsWith(" Report", StringComparison.OrdinalIgnoreCase) ? reportName : reportName + " Report";
        sb.Append("<h1>").Append(E(title)).Append("</h1><p>").Append(E(subtitle)).Append("</p></div>");
        if (!String.IsNullOrEmpty(extraHtml)) sb.Append("<div class='am-rl-headx'>").Append(extraHtml).Append("</div>");
        return sb.Append("</header>").ToString();
    }
    public static string Nav(IEnumerable<Group> groups)
    {
        var sb = new StringBuilder("<aside class='am-rl-nav'><label class='am-rl-search'><svg viewBox='0 0 24 24' aria-hidden='true'><circle cx='11' cy='11' r='7' fill='none' stroke='currentColor' stroke-width='2'/><path d='M16.5 16.5L21 21' stroke='currentColor' stroke-width='2' stroke-linecap='round'/></svg><input type='search' placeholder='Search reports...' aria-label='Search reports' data-rl-search='1' /></label><nav aria-label='Reports'>");
        foreach (var g in groups)
        {
            sb.Append("<details class='am-rl-group' open><summary><span>").Append(E(g.Title)).Append("</span>").Append(Chevron).Append("</summary><ul>");
            foreach (var i in g.Items)
                sb.Append("<li data-rl-text='").Append(A(i.Name.ToLowerInvariant())).Append("'><a href='").Append(A(i.Href)).Append("'").Append(i.On ? " class='on' aria-current='page'" : "").Append(">").Append(DocIcon).Append("<span>").Append(E(i.Name)).Append("</span></a></li>");
            sb.Append("</ul></details>");
        }
        return sb.Append("<p class='am-rl-nohit' hidden>No reports match your search.</p></nav></aside>").ToString();
    }
    // mainFields / advancedFields / options: ready-made <label> blocks. boxAttrs: the page's GET-form attributes.
    public static string Filters(string boxAttrs, string hiddenInputs, string mainFields, string advancedFields, string options, string clearHref, string generateAttrs)
    {
        var sb = new StringBuilder("<div class='am-rl-card am-rl-filters' ").Append(boxAttrs).Append(">").Append(hiddenInputs ?? "");
        sb.Append("<div class='am-rl-band'><h2>").Append(Funnel).Append(" Report Filters</h2></div><div class='am-rl-grid'>").Append(mainFields).Append("</div>");
        if (!String.IsNullOrEmpty(advancedFields) || !String.IsNullOrEmpty(options))
        {
            sb.Append("<details class='am-rl-adv' open><summary><span>").Append(Sliders).Append(" Advanced Filters &amp; Options</span>").Append(Chevron).Append("</summary><div class='am-rl-advbody'>");
            if (!String.IsNullOrEmpty(advancedFields)) sb.Append("<div class='am-rl-grid'>").Append(advancedFields).Append("</div>");
            if (!String.IsNullOrEmpty(options)) sb.Append("<div class='am-rl-opts'>").Append(options).Append("</div>");
            sb.Append("</div></details>");
        }
        sb.Append("<div class='am-rl-go'><a class='am-rl-btn am-rl-outline' href='").Append(A(clearHref)).Append("'>&#8635; Clear Filters</a>")
          .Append("<button type='button' class='am-rl-btn am-rl-primary' ").Append(generateAttrs).Append("><svg viewBox='0 0 24 24' aria-hidden='true'><circle cx='11' cy='11' r='7' fill='none' stroke='currentColor' stroke-width='2.4'/><path d='M16.5 16.5L21 21' stroke='currentColor' stroke-width='2.4' stroke-linecap='round'/></svg> Generate Report</button></div></div>");
        return sb.ToString();
    }
    // pdfAttrs / excelAttrs == null: that export is not allowed for this user. generated == false: empty state, exports disabled.
    public static string Preview(string pdfAttrs, string excelAttrs, bool generated, string summary, string bodyHtml)
    {
        var sb = new StringBuilder("<section class='am-rl-card am-rl-preview' id='rl-preview'><div class='am-rl-prevhead'><div><h2>").Append(DocIcon).Append(" Report Preview</h2><p>")
          .Append(generated ? summary : E("Generate the report to display matching records.")).Append("</p></div><div class='am-rl-exports'>");
        if (pdfAttrs != null) sb.Append("<button type='button' class='am-rl-btn am-rl-ghost' ").Append(generated ? pdfAttrs : "disabled").Append(">&#128196; Export PDF</button>");
        if (excelAttrs != null) sb.Append("<button type='button' class='am-rl-btn am-rl-ghost' ").Append(generated ? excelAttrs : "disabled").Append(">&#128200; Export Excel</button>");
        sb.Append("</div></div>");
        if (!generated) sb.Append(Empty("No report generated", "Select your filters and click Generate Report to view the results here."));
        else sb.Append(bodyHtml);
        return sb.Append("</section>").ToString();
    }
    public static string Empty(string title, string text)
    {
        return "<div class='am-rl-empty'><span class='am-rl-emptyart' aria-hidden='true'><svg viewBox='0 0 64 64'><rect x='14' y='8' width='32' height='42' rx='3' fill='#eaf1fd' stroke='#9db8e8' stroke-width='2'/><path d='M20 18h20M20 25h20M20 32h12' stroke='#9db8e8' stroke-width='2' stroke-linecap='round'/><rect x='36' y='34' width='4' height='14' fill='#7aa2e3'/><rect x='42' y='28' width='4' height='20' fill='#5b8bdc'/><rect x='48' y='38' width='4' height='10' fill='#9db8e8'/></svg></span><div><strong>"
            + E(title) + "</strong><p>" + E(text) + "</p></div></div>";
    }
}
