using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SA = AnyminuteSupportAdmin;

// 2026-11-07 v71 (t206u/t207ua0, Address Verification > Verified Addresses): Support Admin "Add Address" and "Import Address".
// Both add rows straight to the Verified Address Database (AddressMaster Verifed=1) with an AnyminuteAddressReview row
// (Status=Verified, SubmittedBy = "Added by Support (<name>)" / "Imported by Support (<name>)", submitted/reviewed now), so they
// show in the Verified Addresses list. Same validation as Approve (SA.VerifyAddress: street, city, prov/state, country,
// postal format per country). Duplicate = an already verified row with the same street + city + postal code.
// Import reads CSV or .xlsx (first worksheet; minimal zip reader on DeflateStream from System.dll - no new assembly / package).
public static class AnyminuteAddressImport
{
    public const int MaxBytes = 2 * 1024 * 1024, MaxRows = 5000;
    public static readonly string[] Header = { "Address", "City", "Prov/State", "Country", "Postal Code/PIN" };

    public static string Country(string v)
    {
        string k = Regex.Replace((v ?? "").ToUpperInvariant(), @"[^A-Z]", "");
        switch (k)
        {
            case "CANADA": case "CA": case "CAN": return "Canada";
            case "USA": case "US": case "UNITEDSTATES": case "UNITEDSTATESOFAMERICA": case "AMERICA": return "USA";
            case "UK": case "GB": case "GBR": case "UNITEDKINGDOM": case "GREATBRITAIN": case "ENGLAND": case "SCOTLAND": case "WALES": return "UK";
            case "INDIA": case "IN": case "IND": return "India";
        }
        return "";
    }
    static string Cut(string s, int n) { s = (s ?? "").Trim(); return s.Length > n ? s.Substring(0, n) : s; }

    // Returns null when added (id set), "duplicate" when already verified, otherwise the validation error.
    public static string AddVerified(SqlConnection c, SqlTransaction tx, string street, string city, string prov, string country, string postal, string by, string note, out int id)
    {
        id = 0;
        street = Regex.Replace((street ?? "").Trim(), @"\s+", " "); city = Regex.Replace((city ?? "").Trim(), @"\s+", " "); prov = (prov ?? "").Trim();
        string ctry = Country(country);
        if (street.Length == 0) return "Street Name (Address) is required.";
        if (city.Length == 0) return "City is required.";
        if ((country ?? "").Trim().Length == 0) return "Country is required.";
        if (ctry.Length == 0) return "Country must be Canada, USA, UK or India.";
        if (street.Length > 200 || city.Length > 200 || prov.Length > 200) return "A value is longer than 200 characters.";
        string err = SA.VerifyAddress(street, city, prov, ctry, postal ?? "");
        if (err != null) return err;
        postal = SA.NormalizePostal(ctry, postal);
        var dup = AnyminuteTimesheetRepair.Query(c, tx, @"SELECT TOP 1 a.AddressMasterId FROM AddressMaster a WITH (UPDLOCK,HOLDLOCK)
            LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(a.City AS nvarchar(50))
            WHERE a.Verifed=1 AND REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(a.Street_Name)))=UPPER(@s) AND UPPER(LTRIM(RTRIM(COALESCE(ci.CityName,a.City))))=UPPER(@c)",
            "@p", SA.CompactPostal(postal), "@s", street, "@c", city);
        if (dup.Rows.Count > 0) return "duplicate";
        id = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "INSERT AddressMaster(Street_Name,City,Province,PostalCode,Verifed) VALUES(@s,@c,@pr,@pc,1); SELECT CAST(SCOPE_IDENTITY() AS int)",
            "@s", street, "@c", city, "@pr", prov, "@pc", postal).Rows[0][0], CultureInfo.InvariantCulture);
        // TR_AnyminuteAddressMaster_Submitted (REQUIRED_08) may already have written the review row; either way it ends Verified.
        AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteAddressReview WHERE AddressMasterId=@i)
                INSERT AnyminuteAddressReview(AddressMasterId,SubmittedBy,SubmittedAtUTC,OrigStreet,OrigCity,OrigProvince,OrigCountry,OrigPostal,Status,Country,Notes,ReviewedBy,ReviewedByName,ReviewedAtUTC)
                VALUES(@i,@w,SYSUTCDATETIME(),@s,@c,@pr,@co,@pc,'Verified',@co,@n,@b,@bn,SYSUTCDATETIME())
            ELSE UPDATE AnyminuteAddressReview SET SubmittedBy=@w,SubmittedAtUTC=SYSUTCDATETIME(),OrigCountry=@co,Country=@co,Status='Verified',Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
            "@i", id, "@w", Cut(by, 150), "@s", street, "@c", city, "@pr", prov, "@co", ctry, "@pc", postal, "@n", Cut(note, 500), "@b", SA.ActorId(), "@bn", Cut(SA.ActorName(), 150));
        return null;
    }

    // ---- file reading: rows of cells (header row included) ----
    public static List<string[]> Read(string fileName, byte[] data)
    {
        string ext = Path.GetExtension(fileName ?? "").ToLowerInvariant();
        if (ext == ".xlsx") return Xlsx(data);
        if (ext == ".csv" || ext == ".txt") return Csv(Decode(data));
        throw new InvalidDataException("Upload a .csv or .xlsx file.");
    }
    static string Decode(byte[] d)
    {
        if (d.Length >= 3 && d[0] == 0xEF && d[1] == 0xBB && d[2] == 0xBF) return Encoding.UTF8.GetString(d, 3, d.Length - 3);
        try { return new UTF8Encoding(false, true).GetString(d); } catch (DecoderFallbackException) { return Encoding.GetEncoding(1252).GetString(d); }
    }
    public static List<string[]> Csv(string text)
    {
        var rows = new List<string[]>(); var cur = new List<string>(); var cell = new StringBuilder(); bool q = false;
        char sep = ',';
        int nl = text.IndexOf('\n'); string first = nl >= 0 ? text.Substring(0, nl) : text;
        if (first.Count(ch => ch == ';') > first.Count(ch => ch == ',')) sep = ';';
        for (int i = 0; i < text.Length; i++)
        {
            char ch = text[i];
            if (q) { if (ch == '"') { if (i + 1 < text.Length && text[i + 1] == '"') { cell.Append('"'); i++; } else q = false; } else cell.Append(ch); continue; }
            if (ch == '"') q = true;
            else if (ch == sep) { cur.Add(cell.ToString()); cell.Clear(); }
            else if (ch == '\r') { }
            else if (ch == '\n') { cur.Add(cell.ToString()); cell.Clear(); rows.Add(cur.ToArray()); cur.Clear(); }
            else cell.Append(ch);
        }
        if (cell.Length > 0 || cur.Count > 0) { cur.Add(cell.ToString()); rows.Add(cur.ToArray()); }
        return rows;
    }
    // Minimal .xlsx reader: zip central directory -> sharedStrings + first worksheet.
    static Dictionary<string, byte[]> Unzip(byte[] z)
    {
        var files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
        int eocd = -1;
        for (int i = z.Length - 22; i >= Math.Max(0, z.Length - 65557); i--) if (BitConverter.ToUInt32(z, i) == 0x06054b50) { eocd = i; break; }
        if (eocd < 0) throw new InvalidDataException("The .xlsx file could not be read.");
        int n = BitConverter.ToUInt16(z, eocd + 10), p = (int)BitConverter.ToUInt32(z, eocd + 16);
        for (int k = 0; k < n; k++)
        {
            if (BitConverter.ToUInt32(z, p) != 0x02014b50) throw new InvalidDataException("The .xlsx file could not be read.");
            int method = BitConverter.ToUInt16(z, p + 10), csize = (int)BitConverter.ToUInt32(z, p + 20), usize = (int)BitConverter.ToUInt32(z, p + 24);
            int fl = BitConverter.ToUInt16(z, p + 28), el = BitConverter.ToUInt16(z, p + 30), cl = BitConverter.ToUInt16(z, p + 32), off = (int)BitConverter.ToUInt32(z, p + 42);
            string name = Encoding.UTF8.GetString(z, p + 46, fl); p += 46 + fl + el + cl;
            bool want = name.Equals("xl/sharedStrings.xml", StringComparison.OrdinalIgnoreCase) || name.Equals("xl/workbook.xml", StringComparison.OrdinalIgnoreCase) || name.Equals("xl/_rels/workbook.xml.rels", StringComparison.OrdinalIgnoreCase) || name.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase);
            if (!want) continue;
            if (usize > 50 * 1024 * 1024) throw new InvalidDataException("The .xlsx file is too large.");
            int data = off + 30 + BitConverter.ToUInt16(z, off + 26) + BitConverter.ToUInt16(z, off + 28);
            if (method == 0) { var b = new byte[csize]; Buffer.BlockCopy(z, data, b, 0, csize); files[name] = b; }
            else if (method == 8) using (var ds = new DeflateStream(new MemoryStream(z, data, csize), CompressionMode.Decompress)) using (var ms = new MemoryStream()) { ds.CopyTo(ms); files[name] = ms.ToArray(); }
            else throw new InvalidDataException("The .xlsx file uses an unsupported format.");
        }
        return files;
    }
    static XmlDocument Xml(byte[] b) { var d = new XmlDocument { XmlResolver = null }; using (var r = XmlReader.Create(new MemoryStream(b), new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null })) d.Load(r); return d; }
    static List<string[]> Xlsx(byte[] data)
    {
        Dictionary<string, byte[]> f;
        try { f = Unzip(data); } catch (ArgumentException) { throw new InvalidDataException("The .xlsx file could not be read."); }
        const string M = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        var ss = new List<string>();
        if (f.ContainsKey("xl/sharedStrings.xml"))
        {
            var d = Xml(f["xl/sharedStrings.xml"]); var ns = new XmlNamespaceManager(d.NameTable); ns.AddNamespace("m", M);
            foreach (XmlNode si in d.SelectNodes("//m:si", ns)) ss.Add(String.Concat(si.SelectNodes(".//m:t", ns).Cast<XmlNode>().Select(t => t.InnerText)));
        }
        // First sheet in workbook order (via rels), else the lowest sheetN.xml.
        string sheet = null;
        try
        {
            var wb = Xml(f["xl/workbook.xml"]); var ns = new XmlNamespaceManager(wb.NameTable); ns.AddNamespace("m", M);
            var s1 = wb.SelectSingleNode("//m:sheets/m:sheet", ns) as XmlElement;
            string rid = s1 == null ? null : s1.GetAttribute("id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            var rels = Xml(f["xl/_rels/workbook.xml.rels"]);
            foreach (XmlElement r in rels.GetElementsByTagName("Relationship")) if (r.GetAttribute("Id") == rid) { string t = r.GetAttribute("Target").TrimStart('/'); sheet = t.StartsWith("xl/") ? t : "xl/" + t; }
        }
        catch (Exception) { sheet = null; }
        if (sheet == null || !f.ContainsKey(sheet)) sheet = f.Keys.Where(k => k.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase)).OrderBy(k => k.Length).ThenBy(k => k).FirstOrDefault();
        if (sheet == null) throw new InvalidDataException("The .xlsx file has no worksheet.");
        var doc = Xml(f[sheet]); var nm = new XmlNamespaceManager(doc.NameTable); nm.AddNamespace("m", M);
        var rows = new List<string[]>();
        foreach (XmlElement row in doc.SelectNodes("//m:sheetData/m:row", nm))
        {
            int rn; if (Int32.TryParse(row.GetAttribute("r"), out rn)) while (rows.Count < rn - 1) rows.Add(new string[0]);
            var cells = new List<string>();
            foreach (XmlElement cel in row.SelectNodes("m:c", nm))
            {
                int col = cells.Count; string rf = cel.GetAttribute("r");
                var m = Regex.Match(rf, @"^([A-Z]+)"); if (m.Success) { col = 0; foreach (char ch in m.Groups[1].Value) col = col * 26 + (ch - 'A' + 1); col--; }
                while (cells.Count < col) cells.Add("");
                string t = cel.GetAttribute("t"), v;
                var vn = cel.SelectSingleNode("m:v", nm);
                if (t == "s") { int i; v = vn != null && Int32.TryParse(vn.InnerText, out i) && i >= 0 && i < ss.Count ? ss[i] : ""; }
                else if (t == "inlineStr") v = String.Concat(cel.SelectNodes(".//m:t", nm).Cast<XmlNode>().Select(x => x.InnerText));
                else v = vn != null ? vn.InnerText : "";
                if (cells.Count == col) cells.Add(v); else cells[col] = v;
            }
            rows.Add(cells.ToArray());
            if (rows.Count > MaxRows + 1) break;
        }
        return rows;
    }

    // Column positions from the header row (Address/Street, City, Prov/State/Province, Country, Postal/ZIP/PIN); null = no header.
    public static int[] Map(string[] h)
    {
        var idx = new[] { -1, -1, -1, -1, -1 };
        for (int i = 0; i < h.Length; i++)
        {
            string k = Regex.Replace((h[i] ?? "").ToUpperInvariant(), @"[^A-Z]", "");
            if (k.Length == 0) continue;
            int slot = k.StartsWith("POSTAL") || k.StartsWith("ZIP") || k == "PIN" || k.StartsWith("PINCODE") || k == "POSTCODE" ? 4
                : k.StartsWith("PROV") || k.StartsWith("STATE") ? 2 : k.StartsWith("COUNTRY") ? 3 : k.StartsWith("CITY") ? 1
                : k.StartsWith("ADDRESS") || k.StartsWith("STREET") ? 0 : -1;
            if (slot >= 0 && idx[slot] < 0) idx[slot] = i;
        }
        return idx.Count(x => x >= 0) >= 3 ? idx : null;
    }
}
