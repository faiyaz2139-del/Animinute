using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-29 v28: filter engine, option gating, KPIs and PDF / Excel export for the Support Admin Reports hub.
public sealed class SrRun
{
    public SrDef Def; public SrData Base, Data; public List<KeyValuePair<string, string>> Kpis = new List<KeyValuePair<string, string>>();
    public List<string> Applied = new List<string>(); public HashSet<string> On = new HashSet<string>(); public string Error;
    public int[] Visible() { var d = Data; return Enumerable.Range(0, d.Cols.Length).Where(i => d.Gate[i] == null || On.Contains(d.Gate[i])).ToArray(); }
}
public static partial class AnyminuteSupportReports
{
    public static bool OptionOn(SrDef def, NameValueCollection q, string key)
    {
        var o = def.Options.FirstOrDefault(x => x[0] == key); if (o == null) return false;
        return q["gen"] == "1" || q["export"] != null ? q["o_" + key] == "1" : o[2] == "1";
    }
    public static void RangeBounds(string preset, string from, string to, out DateTime? a, out DateTime? b)
    {
        var t = DateTime.Today; var m = new DateTime(t.Year, t.Month, 1); a = null; b = null; DateTime x;
        switch (preset)
        {
            case "Today": a = t; b = t.AddDays(1); break;
            case "Last 7 Days": a = t.AddDays(-6); b = t.AddDays(1); break;
            case "Last 30 Days": a = t.AddDays(-29); b = t.AddDays(1); break;
            case "This Month": a = m; b = m.AddMonths(1); break;
            case "Last Month": a = m.AddMonths(-1); b = m; break;
            case "This Year": a = new DateTime(t.Year, 1, 1); b = a.Value.AddYears(1); break;
            case "Custom Range":
                if (DateTime.TryParseExact(from ?? "", "yyyy-MM-dd", Inv, DateTimeStyles.None, out x)) a = x;
                if (DateTime.TryParseExact(to ?? "", "yyyy-MM-dd", Inv, DateTimeStyles.None, out x)) b = x.AddDays(1);
                break;
        }
    }
    static DateTime? CellDate(string v) { DateTime d; return v.Length >= 10 && DateTime.TryParseExact(v.Substring(0, 10), "yyyy-MM-dd", Inv, DateTimeStyles.None, out d) ? d : (DateTime?)null; }
    public static SrRun Execute(SrDef def, NameValueCollection q)
    {
        var run = new SrRun { Def = def };
        var scope = ResellerScope(); // v49: reseller logins only get their own rows (Support Admin: null = everything)
        if (scope != null && def.Group != "Reseller") { run.Error = "This report is only available to Support Admin."; run.Base = new SrData("Result"); return Shape(run, q); }
        try { using (var c = SA.Open()) run.Base = Limit(def.Load(c, q), scope); }
        catch (SqlException ex) { run.Error = "The report could not read the database: " + ex.Message; run.Base = new SrData("Result"); }
        return Shape(run, q);
    }
    // Filters, Post aggregate and KPIs on already-loaded rows (separate so it can be checked without a database).
    public static SrRun Shape(SrRun run, NameValueCollection q)
    {
        var def = run.Def;
        foreach (var o in def.Options) if (OptionOn(def, q, o[0])) run.On.Add(o[0]);
        var rows = run.Base.Rows.AsEnumerable();
        foreach (var f in def.Filters)
        {
            int i = run.Base.Ix(f.Col); if (i < 0 && f.Type != "param") continue;
            if (f.Type == "range" || f.Type == "range3")
            {
                string p = q["f_" + f.Key + "_preset"] ?? ""; DateTime? a, b; RangeBounds(p, q["f_" + f.Key + "_from"], q["f_" + f.Key + "_to"], out a, out b);
                if (a == null && b == null) continue; int ix = i;
                rows = rows.Where(r => { var d = CellDate(r[ix]); return d != null && (a == null || d >= a) && (b == null || d < b); }).ToList();
                run.Applied.Add(f.Label + ": " + (p == "Custom Range" ? (a == null ? "\u2026" : a.Value.ToString("yyyy-MM-dd", Inv)) + " to " + (b == null ? "\u2026" : b.Value.AddDays(-1).ToString("yyyy-MM-dd", Inv)) : p));
                continue;
            }
            string v = (q["f_" + f.Key] ?? "").Trim(); if (v.Length == 0) continue;
            if (f.Type == "param") { run.Applied.Add(f.Label + ": " + v); continue; }
            int k = i;
            if (f.Type == "min") { double min; if (!Double.TryParse(v, NumberStyles.Float, Inv, out min)) continue; rows = rows.Where(r => { double x; return Double.TryParse(r[k], NumberStyles.Float, Inv, out x) && x >= min; }).ToList(); run.Applied.Add(f.Label + ": " + v + "%+"); }
            else if (f.Type == "text") { rows = rows.Where(r => r[k].IndexOf(v, StringComparison.OrdinalIgnoreCase) >= 0).ToList(); run.Applied.Add(f.Label + " contains " + v); }
            else { rows = rows.Where(r => String.Equals(r[k], v, StringComparison.OrdinalIgnoreCase)).ToList(); run.Applied.Add(f.Label + ": " + v); }
        }
        var data = new SrData(run.Base.Cols) { Gate = run.Base.Gate, Note = run.Base.Note }; data.Rows.AddRange(rows);
        run.Data = def.Post != null ? def.Post(data, q) : data;
        if (run.Data.Note == null) run.Data.Note = run.Base.Note;
        try { run.Kpis = def.Kpis != null ? def.Kpis(run.Data) : new List<KeyValuePair<string, string>> { K("Records", run.Data.Rows.Count) }; }
        catch (FormatException) { run.Kpis = new List<KeyValuePair<string, string>> { K("Records", run.Data.Rows.Count) }; }
        return run;
    }
    // Choices for a select filter: the fixed list, or the distinct values present in the unfiltered data.
    public static IEnumerable<string> OptionValues(SrFilter f, SrData all)
    {
        if (f.Fixed != null) return f.Fixed;
        int i = all == null ? -1 : all.Ix(f.Col); if (i < 0) return new string[0];
        return all.Rows.Select(r => r[i]).Where(v => v.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(v => v, StringComparer.OrdinalIgnoreCase);
    }
    public static AmRptResult ToResult(SrRun run)
    {
        var vis = run.Visible();
        var r = new AmRptResult { Def = new AmRptDef { Key = "sa-" + run.Def.Key, Name = run.Def.Name + " Report", Desc = run.Def.Purpose }, Subtitle = (run.Def.Group == "Reseller" ? "Reseller Reports" : "Support Admin Reports") + " \u2022 " + run.Data.Rows.Count + " record(s)",
            FilterText = run.Applied.Count == 0 ? "No filters (all records)" : String.Join("; ", run.Applied.ToArray()), Generated = DateTime.Now.ToString("yyyy-MM-dd HH:mm", Inv) + " by " + SA.ActorName(),
            Note = run.Error ?? run.Data.Note, Kpis = run.Kpis };
        var s = new AmRptSection { Title = run.Def.Name + " Report", Cols = vis.Select(i => run.Data.Cols[i]).ToArray() };
        foreach (var row in run.Data.Rows) s.Rows.Add(vis.Select(i => row[i]).ToArray());
        r.Sections.Add(s);
        return r;
    }
    public static void Export(HttpContext h, SrRun run, bool pdf)
    {
        try { using (var c = SA.Open()) if (SA.Has(c, "AnyminuteSupportAudit")) SA.Audit(c, null, "Report", run.Def.Key, pdf ? "Exported PDF" : "Exported Excel", null, null, null, run.Data.Rows.Count + " rows", run.Applied.Count == 0 ? null : String.Join("; ", run.Applied.ToArray())); }
        catch (SqlException) { }
        var res = ToResult(run); string file = "AnyMinute_" + run.Def.Name.Replace(" & ", "_").Replace(" ", "_") + "_Report_" + DateTime.Now.ToString("yyyyMMdd", Inv) + (pdf ? ".pdf" : ".xlsx");
        var resp = h.Response; resp.Clear(); resp.Buffer = true;
        resp.ContentType = pdf ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        resp.AddHeader("Content-Disposition", "attachment; filename=\"" + file + "\"");
        using (var ms = new System.IO.MemoryStream()) { if (pdf) AnyminuteReportFiles.Pdf(res, ms); else AnyminuteReportFiles.Xlsx(res, ms); ms.WriteTo(resp.OutputStream); }
        resp.Flush(); h.ApplicationInstance.CompleteRequest();
    }
}
