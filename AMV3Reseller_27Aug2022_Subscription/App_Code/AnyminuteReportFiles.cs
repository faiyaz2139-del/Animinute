using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using iTextSharp.text;
using iTextSharp.text.pdf;

// 2026-10-26 v19 Reports: PDF (iTextSharp 5, already in Bin) and real .xlsx (minimal SpreadsheetML in a stored zip,
// no new assembly / web.config reference needed). Layout follows Pandey's sample templates: title band
// "ANYMINUTE | REPORT", subtitle line, KPI strip, then one table per section (PDF) / one sheet per section (Excel,
// title in row 1, subtitle row 2, headers row 4, data from row 5).
public static class AnyminuteReportFiles
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static readonly BaseColor Navy = new BaseColor(0x1B, 0x3A, 0x6B), Blue = new BaseColor(0x1F, 0x6F, 0xEB), Zebra = new BaseColor(0xF4, 0xF7, 0xFB), Line = new BaseColor(0xD9, 0xE0, 0xEA), Muted = new BaseColor(0x5B, 0x6B, 0x80);

    sealed class Footer : PdfPageEventHelper
    {
        public string Text;
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            var f = FontFactory.GetFont(FontFactory.HELVETICA, 7, Muted);
            ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_LEFT, new Phrase(Text, f), doc.LeftMargin, doc.BottomMargin - 14, 0);
            ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_RIGHT, new Phrase("Page " + writer.PageNumber, f), doc.PageSize.Width - doc.RightMargin, doc.BottomMargin - 14, 0);
        }
    }

    // v51 (held note B+): the title band / footer carry the company the report is about, not a hard-coded "ANYMINUTE".
    static string Brand(AmRptResult r) { string k = (r.Company ?? "").Trim(); return k.Length == 0 ? "" : k.ToUpperInvariant() + "  |  "; }
    static string FootBrand(AmRptResult r) { string k = (r.Company ?? "").Trim(); return k.Length == 0 ? "" : k + " \u2022 "; }
    // Signed-in user's own organisation (MotherOrg) - the fallback when the rows are not about one single company.
    public static string ActorCompany()
    {
        try
        {
            var h = System.Web.HttpContext.Current; int m;
            if (h == null || h.Session == null || !Int32.TryParse(Convert.ToString(h.Session["MotherOrgid"]), out m) || m <= 0) return "";
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                var t = AnyminuteTimesheetRepair.Query(c, null, "SELECT MotherOrgName FROM MotherOrg WHERE MotherOrgID=@m", "@m", m);
                return t.Rows.Count == 0 ? "" : Convert.ToString(t.Rows[0][0]).Trim();
            }
        }
        catch (Exception) { return ""; }
    }
    public static void Pdf(AmRptResult r, Stream output)
    {
        var doc = new Document(PageSize.LETTER.Rotate(), 28, 28, 28, 36);
        var writer = PdfWriter.GetInstance(doc, output);
        writer.CloseStream = false;
        writer.PageEvent = new Footer { Text = FootBrand(r) + r.Def.Name + " \u2022 Confidential \u2022 Generated " + r.Generated };
        doc.Open();
        var band = new PdfPTable(1) { WidthPercentage = 100 };
        var title = new PdfPCell(new Phrase(Brand(r) + r.Def.Name.ToUpperInvariant(), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 15, BaseColor.WHITE)));
        title.BackgroundColor = Navy; title.Border = 0; title.Padding = 10; title.PaddingBottom = 12; band.AddCell(title);
        doc.Add(band);
        doc.Add(new Paragraph(r.Subtitle, FontFactory.GetFont(FontFactory.HELVETICA, 8.5f, Muted)) { SpacingBefore = 6, SpacingAfter = 4 });
        if (!String.IsNullOrEmpty(r.FilterText)) doc.Add(new Paragraph("Filters: " + r.FilterText, FontFactory.GetFont(FontFactory.HELVETICA, 8, Muted)) { SpacingAfter = 6 });
        if (r.Kpis.Count > 0)
        {
            var k = new PdfPTable(r.Kpis.Count) { WidthPercentage = 100, SpacingAfter = 8 };
            foreach (var kpi in r.Kpis)
            {
                var p = new Phrase();
                p.Add(new Chunk(kpi.Key.ToUpperInvariant() + "\n", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 7, Muted)));
                p.Add(new Chunk(kpi.Value, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, Navy)));
                var cell = new PdfPCell(p) { Padding = 7, BorderColor = Line, BackgroundColor = Zebra };
                k.AddCell(cell);
            }
            doc.Add(k);
        }
        foreach (var s in r.Sections)
        {
            doc.Add(new Paragraph(s.Title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10.5f, Navy)) { SpacingBefore = 6, SpacingAfter = 4 });
            if (s.Rows.Count == 0) { doc.Add(new Paragraph("No records match the selected filters.", FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8.5f, Muted))); continue; }
            var t = new PdfPTable(s.Cols.Length) { WidthPercentage = 100, HeaderRows = 1 };
            float size = s.Cols.Length > 12 ? 6f : s.Cols.Length > 9 ? 6.8f : 7.8f;
            var hf = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, size, BaseColor.WHITE);
            var bf = FontFactory.GetFont(FontFactory.HELVETICA, size, BaseColor.BLACK);
            foreach (var col in s.Cols) t.AddCell(new PdfPCell(new Phrase(col, hf)) { BackgroundColor = Blue, BorderColor = Blue, Padding = 4 });
            for (int i = 0; i < s.Rows.Count; i++)
                for (int j = 0; j < s.Cols.Length; j++)
                {
                    string v = j < s.Rows[i].Length ? (s.Rows[i][j] ?? "") : "";
                    var cell = new PdfPCell(new Phrase(v, bf)) { Padding = 3.5f, BorderColor = Line, BackgroundColor = i % 2 == 1 ? Zebra : BaseColor.WHITE };
                    if (Numeric(v)) cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    t.AddCell(cell);
                }
            doc.Add(t);
        }
        if (!String.IsNullOrEmpty(r.Note)) doc.Add(new Paragraph(r.Note, FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 7.5f, Muted)) { SpacingBefore = 8 });
        doc.Close();
    }

    static bool Numeric(string v) { return Regex.IsMatch(v ?? "", @"^-?\d{1,12}(\.\d+)?$"); }
    static string X(string v) { return System.Security.SecurityElement.Escape(v ?? "") ?? ""; }
    static string Col(int i) { string s = ""; i++; while (i > 0) { int m = (i - 1) % 26; s = (char)('A' + m) + s; i = (i - 1) / 26; } return s; }
    static string Cell(int c, int r, string v, int style)
    {
        string a = Col(c) + r.ToString(Inv);
        if (style == 0 && Numeric(v) && !(v.Length > 1 && v[0] == '0' && v[1] != '.')) return "<c r=\"" + a + "\"><v>" + v + "</v></c>";
        return "<c r=\"" + a + "\" t=\"inlineStr\"" + (style > 0 ? " s=\"" + style + "\"" : "") + "><is><t xml:space=\"preserve\">" + X(v) + "</t></is></c>";
    }
    static string Sheet(AmRptResult r, AmRptSection s)
    {
        var sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\">");
        sb.Append("<sheetViews><sheetView workbookViewId=\"0\"><pane ySplit=\"4\" topLeftCell=\"A5\" activePane=\"bottomLeft\" state=\"frozen\"/></sheetView></sheetViews><cols>");
        for (int j = 0; j < s.Cols.Length; j++)
        {
            int w = s.Cols[j].Length + 2; foreach (var row in s.Rows.Take(300)) if (j < row.Length && row[j] != null) w = Math.Max(w, Math.Min(row[j].Length + 2, 48));
            sb.Append("<col min=\"").Append(j + 1).Append("\" max=\"").Append(j + 1).Append("\" width=\"").Append(Math.Max(w, 10)).Append("\" customWidth=\"1\"/>");
        }
        sb.Append("</cols><sheetData>");
        sb.Append("<row r=\"1\">").Append(Cell(0, 1, Brand(r) + r.Def.Name.ToUpperInvariant() + (s.Title == r.Def.Name ? "" : "  /  " + s.Title.ToUpperInvariant()), 1)).Append("</row>");
        sb.Append("<row r=\"2\">").Append(Cell(0, 2, r.Subtitle + (String.IsNullOrEmpty(r.FilterText) ? "" : "  \u2022  Filters: " + r.FilterText), 3)).Append("</row>");
        sb.Append("<row r=\"4\">"); for (int j = 0; j < s.Cols.Length; j++) sb.Append(Cell(j, 4, s.Cols[j], 2)); sb.Append("</row>");
        for (int i = 0; i < s.Rows.Count; i++)
        {
            int rn = i + 5; sb.Append("<row r=\"").Append(rn).Append("\">");
            for (int j = 0; j < s.Cols.Length; j++) sb.Append(Cell(j, rn, j < s.Rows[i].Length ? s.Rows[i][j] : "", 0));
            sb.Append("</row>");
        }
        if (s.Rows.Count == 0) sb.Append("<row r=\"5\">").Append(Cell(0, 5, "No records match the selected filters.", 3)).Append("</row>");
        sb.Append("</sheetData>");
        if (s.Rows.Count > 0) sb.Append("<autoFilter ref=\"A4:").Append(Col(s.Cols.Length - 1)).Append(s.Rows.Count + 4).Append("\"/>");
        return sb.Append("</worksheet>").ToString();
    }
    static string SheetName(string t, HashSet<string> used)
    {
        string n = Regex.Replace(t ?? "Sheet", @"[\[\]\:\*\?\/\\]", " ").Trim(); if (n.Length > 31) n = n.Substring(0, 31).Trim(); if (n == "") n = "Sheet";
        string b = n; int k = 2; while (!used.Add(n.ToLowerInvariant())) { n = (b.Length > 28 ? b.Substring(0, 28) : b) + " " + k++; }
        return n;
    }
    public static void Xlsx(AmRptResult r, Stream output)
    {
        var files = new List<KeyValuePair<string, string>>();
        var used = new HashSet<string>(); var names = new List<string>();
        var sections = r.Sections.Count == 0 ? new List<AmRptSection> { new AmRptSection { Title = r.Def.Name, Cols = new[] { "Result" } } } : r.Sections;
        for (int i = 0; i < sections.Count; i++) { names.Add(SheetName(sections[i].Title, used)); files.Add(new KeyValuePair<string, string>("xl/worksheets/sheet" + (i + 1) + ".xml", Sheet(r, sections[i]))); }
        var ct = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\"><Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/><Default Extension=\"xml\" ContentType=\"application/xml\"/><Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/><Override PartName=\"/xl/styles.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml\"/>");
        var wb = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\"><sheets>");
        var rel = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">");
        for (int i = 0; i < sections.Count; i++)
        {
            ct.Append("<Override PartName=\"/xl/worksheets/sheet").Append(i + 1).Append(".xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>");
            wb.Append("<sheet name=\"").Append(X(names[i])).Append("\" sheetId=\"").Append(i + 1).Append("\" r:id=\"rId").Append(i + 1).Append("\"/>");
            rel.Append("<Relationship Id=\"rId").Append(i + 1).Append("\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet").Append(i + 1).Append(".xml\"/>");
        }
        rel.Append("<Relationship Id=\"rId").Append(sections.Count + 1).Append("\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles\" Target=\"styles.xml\"/></Relationships>");
        files.Add(new KeyValuePair<string, string>("[Content_Types].xml", ct.Append("</Types>").ToString()));
        files.Add(new KeyValuePair<string, string>("_rels/.rels", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\"><Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/></Relationships>"));
        files.Add(new KeyValuePair<string, string>("xl/workbook.xml", wb.Append("</sheets></workbook>").ToString()));
        files.Add(new KeyValuePair<string, string>("xl/_rels/workbook.xml.rels", rel.ToString()));
        files.Add(new KeyValuePair<string, string>("xl/styles.xml", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><styleSheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><fonts count=\"4\"><font><sz val=\"10\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"14\"/><color rgb=\"FF1B3A6B\"/><name val=\"Calibri\"/></font><font><b/><sz val=\"10\"/><color rgb=\"FFFFFFFF\"/><name val=\"Calibri\"/></font><font><i/><sz val=\"9\"/><color rgb=\"FF5B6B80\"/><name val=\"Calibri\"/></font></fonts><fills count=\"3\"><fill><patternFill patternType=\"none\"/></fill><fill><patternFill patternType=\"gray125\"/></fill><fill><patternFill patternType=\"solid\"><fgColor rgb=\"FF1F6FEB\"/><bgColor indexed=\"64\"/></patternFill></fill></fills><borders count=\"1\"><border><left/><right/><top/><bottom/><diagonal/></border></borders><cellStyleXfs count=\"1\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\"/></cellStyleXfs><cellXfs count=\"4\"><xf numFmtId=\"0\" fontId=\"0\" fillId=\"0\" borderId=\"0\" xfId=\"0\"/><xf numFmtId=\"0\" fontId=\"1\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/><xf numFmtId=\"0\" fontId=\"2\" fillId=\"2\" borderId=\"0\" xfId=\"0\" applyFont=\"1\" applyFill=\"1\"/><xf numFmtId=\"0\" fontId=\"3\" fillId=\"0\" borderId=\"0\" xfId=\"0\" applyFont=\"1\"/></cellXfs><cellStyles count=\"1\"><cellStyle name=\"Normal\" xfId=\"0\" builtinId=\"0\"/></cellStyles></styleSheet>"));
        Zip(files, output);
    }

    // Minimal "stored" (uncompressed) zip writer; valid for Excel / LibreOffice / Numbers.
    static uint[] crcTable;
    static uint Crc(byte[] data)
    {
        if (crcTable == null) { var t = new uint[256]; for (uint n = 0; n < 256; n++) { uint c = n; for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1; t[n] = c; } crcTable = t; }
        uint crc = 0xFFFFFFFFu; foreach (byte b in data) crc = crcTable[(crc ^ b) & 0xFF] ^ (crc >> 8); return crc ^ 0xFFFFFFFFu;
    }
    static void Zip(List<KeyValuePair<string, string>> files, Stream output)
    {
        var ms = new MemoryStream(); var w = new BinaryWriter(ms); var central = new MemoryStream(); var cw = new BinaryWriter(central);
        DateTime now = DateTime.Now; ushort time = (ushort)((now.Hour << 11) | (now.Minute << 5) | (now.Second / 2)), date = (ushort)(((now.Year - 1980) << 9) | (now.Month << 5) | now.Day);
        foreach (var f in files)
        {
            byte[] name = Encoding.UTF8.GetBytes(f.Key), data = new UTF8Encoding(false).GetBytes(f.Value); uint crc = Crc(data); uint offset = (uint)ms.Position;
            w.Write(0x04034b50u); w.Write((ushort)20); w.Write((ushort)0x0800); w.Write((ushort)0); w.Write(time); w.Write(date); w.Write(crc); w.Write((uint)data.Length); w.Write((uint)data.Length); w.Write((ushort)name.Length); w.Write((ushort)0); w.Write(name); w.Write(data);
            cw.Write(0x02014b50u); cw.Write((ushort)20); cw.Write((ushort)20); cw.Write((ushort)0x0800); cw.Write((ushort)0); cw.Write(time); cw.Write(date); cw.Write(crc); cw.Write((uint)data.Length); cw.Write((uint)data.Length); cw.Write((ushort)name.Length); cw.Write((ushort)0); cw.Write((ushort)0); cw.Write((ushort)0); cw.Write((ushort)0); cw.Write(0u); cw.Write(offset); cw.Write(name);
        }
        cw.Flush(); uint cdOffset = (uint)ms.Position; w.Write(central.ToArray());
        w.Write(0x06054b50u); w.Write((ushort)0); w.Write((ushort)0); w.Write((ushort)files.Count); w.Write((ushort)files.Count); w.Write((uint)central.Length); w.Write(cdOffset); w.Write((ushort)0);
        w.Flush(); ms.Position = 0; ms.CopyTo(output);
    }
}
