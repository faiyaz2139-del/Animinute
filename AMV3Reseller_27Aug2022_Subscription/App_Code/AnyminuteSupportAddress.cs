using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// 2026-11-01 v46 (Address deck s1 "Require Address Module" in Add Reseller, s2 "Address module in Subscriber - Remove the bottom"):
// the Business "Address Lookup" module for Support Admin forms. Same markup / classes as Add_Business (styled by
// css/AnyminuteAddressLookup.css in the .am-emp-al scope): Country with flag, Postal / ZIP Code with clear (x), Search Address,
// Enter Address Manually, hint, "Address Not Found" card (Paused deck s1 wording), "Select Address from Search Results",
// then Address Details (Street Number, Address Line 1, Address Line 2, Suite Type, Suite Number, City, Province / State,
// Postal / ZIP Code, Country). Behaviour: Scripts/AnyminuteSupport.js ([data-sa-addrmod]) with a JSON lookup endpoint
// (?lookup=1&postal=...) on the page, backed by AnyminuteAddressLookup.Search.
public static class AnyminuteSupportAddress
{
    public class Addr
    {
        public string StreetNo = "", Line1 = "", Line2 = "", SuiteType = "", SuiteNo = "", City = "", Province = "", Postal = "", Country = "Canada";
        public bool Any { get { return (StreetNo + Line1 + Line2 + SuiteNo + City + Postal).Trim().Length > 0; } }
    }
    public static readonly string[] Canada = { "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland and Labrador", "Northwest Territories", "Nova Scotia", "Nunavut", "Ontario", "Prince Edward Island", "Quebec", "Saskatchewan", "Yukon" };
    public static readonly string[] States = { "Alabama", "Alaska", "Arizona", "Arkansas", "California", "Colorado", "Connecticut", "Delaware", "District of Columbia", "Florida", "Georgia", "Hawaii", "Idaho", "Illinois", "Indiana", "Iowa", "Kansas", "Kentucky", "Louisiana", "Maine", "Maryland", "Massachusetts", "Michigan", "Minnesota", "Mississippi", "Missouri", "Montana", "Nebraska", "Nevada", "New Hampshire", "New Jersey", "New Mexico", "New York", "North Carolina", "North Dakota", "Ohio", "Oklahoma", "Oregon", "Pennsylvania", "Rhode Island", "South Carolina", "South Dakota", "Tennessee", "Texas", "Utah", "Vermont", "Virginia", "Washington", "West Virginia", "Wisconsin", "Wyoming" };
    public static readonly string[] SuiteTypes = { "Apartment", "Suite", "Unit" };
    static readonly Regex CaPostal = new Regex(@"^[A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d$"), UsZip = new Regex(@"^\d{5}(-\d{4})?$");

    static string F(string prefix, string k) { return (HttpContext.Current.Request.Form[prefix + k] ?? "").Trim(); }
    public static Addr FromForm(string prefix)
    {
        var a = new Addr { StreetNo = F(prefix, "_no"), Line1 = F(prefix, "_line1"), Line2 = F(prefix, "_line2"), SuiteType = F(prefix, "_suitetype"), SuiteNo = F(prefix, "_suiteno"),
            City = F(prefix, "_city"), Province = F(prefix, "_prov"), Postal = F(prefix, "_postal"), Country = F(prefix, "_country") };
        if (a.Country != "United States") a.Country = "Canada";
        if (!SuiteTypes.Contains(a.SuiteType)) a.SuiteType = "";
        a.Postal = NormalPostal(a.Postal, a.Country);
        return a;
    }
    public static string NormalPostal(string p, string country)
    {
        p = (p ?? "").Trim().ToUpperInvariant();
        string compact = Regex.Replace(p, @"[\s-]", "");
        if (country != "United States" && Regex.IsMatch(compact, @"^[A-Z]\d[A-Z]\d[A-Z]\d$")) return compact.Substring(0, 3) + " " + compact.Substring(3);
        return p;
    }
    // required: Reseller (always). Optional (Subscriber): nothing entered = no address; anything entered = complete address.
    public static List<string> Validate(Addr a, bool required)
    {
        var e = new List<string>();
        if (!required && !a.Any) return e;
        if (a.StreetNo.Length == 0) e.Add("Street Number is required.");
        if (a.Line1.Length == 0) e.Add("Address Line 1 is required.");
        if (a.City.Length == 0) e.Add("City is required.");
        if (a.Province.Length == 0) e.Add("Province / State is required.");
        if (a.Postal.Length == 0) e.Add("Postal / ZIP Code is required.");
        else if (a.Country == "United States" ? !UsZip.IsMatch(a.Postal) : !CaPostal.IsMatch(a.Postal)) e.Add(a.Country == "United States" ? "Enter a valid US ZIP code (e.g. 33185)." : "Enter a valid Canadian postal code (e.g. M1B 2W1).");
        if (a.StreetNo.Length + a.Line1.Length + a.Line2.Length + a.SuiteNo.Length + a.City.Length > 330) e.Add("The address is too long.");
        return e;
    }
    // One display line: "3112 Krome Ave, Floor 2, Suite 200, Mississauga, Ontario L5W 0A2, Canada".
    public static string Compose(Addr a)
    {
        if (!a.Any) return "";
        var parts = new List<string>();
        string first = (a.StreetNo + " " + a.Line1).Trim(); if (first.Length > 0) parts.Add(first);
        if (a.Line2.Length > 0) parts.Add(a.Line2);
        if (a.SuiteNo.Length > 0) parts.Add(((a.SuiteType.Length > 0 ? a.SuiteType : "Suite") + " " + a.SuiteNo).Trim());
        if (a.City.Length > 0) parts.Add(a.City);
        string pp = (a.Province + " " + a.Postal).Trim(); if (pp.Length > 0) parts.Add(pp);
        parts.Add(a.Country);
        return String.Join(", ", parts.Select(x => x.Replace(",", " ")).ToArray());
    }
    // Best-effort reverse of Compose for the Edit form; anything that does not fit goes to Address Line 1.
    public static Addr Parse(string s)
    {
        var a = new Addr(); s = (s ?? "").Trim(); if (s.Length == 0) return a;
        var parts = s.Replace("\r", "").Replace("\n", ", ").Split(',').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
        if (parts.Count >= 4 && (parts[parts.Count - 1] == "Canada" || parts[parts.Count - 1] == "United States"))
        {
            a.Country = parts[parts.Count - 1];
            var m = Regex.Match(parts[parts.Count - 2], @"^(.*?)\s+([A-Za-z]\d[A-Za-z][ -]?\d[A-Za-z]\d|\d{5}(?:-\d{4})?)$");
            if (m.Success)
            {
                a.Province = m.Groups[1].Value.Trim(); a.Postal = m.Groups[2].Value.ToUpperInvariant(); a.City = parts[parts.Count - 3];
                var head = parts.Take(parts.Count - 3).ToList();
                var n = Regex.Match(head[0], @"^(\S*\d\S*)\s+(.+)$");
                if (n.Success) { a.StreetNo = n.Groups[1].Value; a.Line1 = n.Groups[2].Value; } else a.Line1 = head[0];
                foreach (var h in head.Skip(1))
                {
                    var su = Regex.Match(h, @"^(Apartment|Suite|Unit)\s+(.+)$", RegexOptions.IgnoreCase);
                    if (su.Success && a.SuiteNo.Length == 0) { a.SuiteType = SuiteTypes.First(t => t.Equals(su.Groups[1].Value, StringComparison.OrdinalIgnoreCase)); a.SuiteNo = su.Groups[2].Value; }
                    else a.Line2 = (a.Line2 + " " + h).Trim();
                }
                return a;
            }
        }
        a.Line1 = s.Length > 50 ? s.Substring(0, 50) : s; if (s.Length > 50) a.Line2 = s.Substring(50, Math.Min(50, s.Length - 50));
        return a;
    }
    static string H(string s) { return HttpUtility.HtmlEncode(s ?? ""); }
    static string A(string s) { return HttpUtility.HtmlAttributeEncode(s ?? ""); }
    static string Opts(IEnumerable<string> list, string sel, string blank)
    {
        var sb = new StringBuilder(); if (blank != null) sb.Append("<option value=''>").Append(H(blank)).Append("</option>");
        bool hit = false;
        foreach (var o in list) { bool on = String.Equals(o, sel, StringComparison.OrdinalIgnoreCase); hit |= on; sb.Append("<option value='").Append(A(o)).Append("'").Append(on ? " selected" : "").Append(">").Append(H(o)).Append("</option>"); }
        if (!hit && !String.IsNullOrEmpty(sel)) sb.Append("<option value='").Append(A(sel)).Append("' selected>").Append(H(sel)).Append("</option>");
        return sb.ToString();
    }
    const string FlagCa = "<svg class='am-flag-ca' viewBox='0 0 24 16' width='24' height='16' aria-hidden='true' focusable='false'><rect width='24' height='16' fill='#fff'/><rect width='6' height='16' fill='#d52b1e'/><rect x='18' width='6' height='16' fill='#d52b1e'/><path d='M12 3.2l1 2.1 1.3-.6-.4 2.6 1.4-1.3.4 1-1.2 1.1h1.1L15 9.5l.9.5-3.4.6V13h-1V10.6l-3.4-.6.9-.5-.6-1.4h1.1L8.3 7l.4-1 1.4 1.3-.4-2.6 1.3.6z' fill='#d52b1e'/></svg>";
    const string FlagUs = "<svg class='am-flag-us' viewBox='0 0 24 16' width='24' height='16' aria-hidden='true' focusable='false'><rect width='24' height='16' fill='#fff'/><g fill='#b22234'><rect width='24' height='1.23'/><rect y='2.46' width='24' height='1.23'/><rect y='4.92' width='24' height='1.23'/><rect y='7.38' width='24' height='1.23'/><rect y='9.84' width='24' height='1.23'/><rect y='12.3' width='24' height='1.23'/><rect y='14.77' width='24' height='1.23'/></g><rect width='10' height='8.6' fill='#3c3b6e'/></svg>";
    const string IcoSearch = "<svg viewBox='0 0 24 24' width='18' height='18' aria-hidden='true' focusable='false'><circle cx='11' cy='11' r='7' fill='none' stroke='currentColor' stroke-width='2.4'/><path d='M16.5 16.5L21 21' stroke='currentColor' stroke-width='2.4' stroke-linecap='round'/></svg>";
    const string IcoPen = "<svg viewBox='0 0 24 24' width='18' height='18' aria-hidden='true' focusable='false'><path fill='currentColor' d='M3 17.25V21h3.75L17.8 9.94l-3.75-3.75L3 17.25zm17.71-10.21a1 1 0 000-1.41l-2.34-2.34a1 1 0 00-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z'/></svg>";

    // v53 (NOTES_v53 #3/#4, img_02 -> img_01): Edit Reseller / Edit Subscriber collapsed Address card. Pin + "Address",
    // current composed address or "Please Select", module note and an outline "Manage Address" button that reveals the
    // Address Lookup module (panel) in place. The panel stays in the DOM (hidden) so its fields still post; {prefix}_open
    // tells the save path whether the module was opened (not opened = keep the stored address untouched).
    public static bool Opened(string prefix) { return F(prefix, "_open") == "1"; }
    const string IcoPin = "<svg class='sa-i' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true' focusable='false'><path d='M21 10c0 7-9 13-9 13S3 17 3 10a9 9 0 0 1 18 0z'/><circle cx='12' cy='10' r='3'/></svg>";
    public static string Gate(string prefix, string summary, bool open, string panel) { return Gate(prefix, summary, open, panel, null); }
    public static string Gate(string prefix, string summary, bool open, string panel, string badge)
    {
        string sum = (summary ?? "").Trim();
        return "<div class='sa-addr-wrap' id='address'><div class='sa-card sa-addr-gate' id='" + prefix + "_gate'" + (open ? " hidden" : "") + "><span class='sa-addr-gate-ico'>" + IcoPin + "</span><div class='sa-addr-gate-txt'><h2>Address</h2>"
            + "<div class='sa-addr-gate-sum'>" + (sum.Length > 0 ? H(sum) + (badge ?? "") : "Please Select") + "</div><small class='sa-muted'>Address changes are managed through the separate Address module.</small></div>"
            + "<button type='button' class='sa-btn sa-btn-outline sa-addr-gate-btn' data-sa-reveal='" + prefix + "_panel' data-sa-gate='" + prefix + "'>" + IcoPin + " Manage Address</button></div>"
            + "<input type='hidden' name='" + prefix + "_open' id='" + prefix + "_open' value='" + (open ? "1" : "0") + "' />"
            + "<div id='" + prefix + "_panel' class='sa-addr-panel' data-sa-addrpanel='1'" + (open ? "" : " hidden") + ">" + panel + "</div></div>";
    }
    // prefix: form field prefix (rs_addr / ad_addr). lookupUrl: page URL ending in "postal=" that returns [{street,city,province}].
    public static string Render(string prefix, Addr a, bool required, string lookupUrl, string sub) { return Render(prefix, a, required, lookupUrl, sub, null, false); }
    // v57 (PPT s4): verify = this form queues unverified addresses for Address Verification on save; status = the stored
    // address status (Verified / Pending / Rejected / "" = not in the verification table) shown in the Address Details pill.
    public static string Render(string prefix, Addr a, bool required, string lookupUrl, string sub, string status, bool verify) { return Render(prefix, a, required, lookupUrl, sub, status, verify, null); }
    // v70 (Pandey docx s1/s2): std = title of a separate Address Details card (the standard Employee Editor / Employee Profile
    // layout, .am-emp-al46): Address Lookup card, then "<std>" card with the "Address Details" pill, 3-column grid and the
    // "City, State/Province and Country" band. Same field names / JS hooks, so posting and validation are unchanged.
    public static string Render(string prefix, Addr a, bool required, string lookupUrl, string sub, string status, bool verify, string std)
    {
        bool sd = std != null;
        if (a == null) a = new Addr();
        bool us = a.Country == "United States";
        string req = "<b class='am-al-req'>*</b>", opt = "<span class='am-al-opt'>(Optional)</span>", rq = required ? " required" : "";
        Func<string, string, string, string, string, string, string> fld = (label, key, val, ph, extra, cls) =>
            "<div class='am-al-f" + cls + "'><label for='" + prefix + key + "'>" + label + "</label><input type='text' id='" + prefix + key + "' name='" + prefix + key + "' value='" + A(val) + "' placeholder='" + A(ph) + "'" + extra + " /></div>";
        var sb = new StringBuilder(sd ? "<div class='am-emp-al am-emp-al46 sa-addrmod sa-addr-std' data-sa-addrmod='" : "<div class='am-emp-al sa-addrmod' data-sa-addrmod='").Append(A(lookupUrl)).Append("' data-sa-addr-prefix='").Append(prefix).Append("'").Append(verify ? " data-al-verify='1'" : "").Append(">")
            .Append("<input type='hidden' name='").Append(prefix).Append("_src' id='").Append(prefix).Append("_src' value='").Append(A(F(prefix, "_src"))).Append("' data-al-lsrc='1' />")
            .Append("<div class='am-al-layout am-al-single'><section class='am-al-main' aria-labelledby='").Append(prefix).Append("_h'>")
            .Append("<h2 id='").Append(prefix).Append("_h'>Address Lookup</h2><p class='am-al-sub'>").Append(H(sub ?? "Search for an address using a postal or ZIP code, select a result, or enter the address manually.")).Append("</p>")
            .Append("<div class='am-al-search'><div class='am-al-f am-al-country'><label for='").Append(prefix).Append("_lc'>Country ").Append(req).Append("</label><span class='am-al-flagsel'><span class='am-al-flag").Append(us ? " is-us" : "").Append("' data-al-flag='1'>").Append(FlagCa).Append(FlagUs).Append("</span>")
            .Append("<select id='").Append(prefix).Append("_lc' data-al-lcountry='1'>").Append(Opts(new[] { "Canada", "United States" }, a.Country, null)).Append("</select></span></div>")
            .Append("<div class='am-al-f am-al-postal'><label for='").Append(prefix).Append("_lp'>Postal / ZIP Code ").Append(req).Append("</label><span class='am-al-clearwrap'><input type='text' id='").Append(prefix).Append("_lp' data-al-lpostal='1' maxlength='10' autocomplete='off' placeholder='").Append(us ? "e.g. 33185" : "e.g. M1B 2W1").Append("' value='").Append(A(a.Postal)).Append("' /><button type='button' class='am-al-clear' data-al-lclear='1' aria-label='Clear postal code'>&times;</button></span></div>")
            .Append("<button type='button' class='am-al-btn am-al-primary' data-al-lsearch='1'>").Append(IcoSearch).Append(" Search Address</button>")
            .Append("<button type='button' class='am-al-btn am-al-outline' data-al-lmanual='1'>").Append(IcoPen).Append(" Enter Address Manually</button></div>")
            .Append("<div class='am-al-info' data-al-lhint='1'><span class='am-al-i' aria-hidden='true'>i</span><span data-al-lhinttext='1'>").Append(us ? "Enter a valid US ZIP code (e.g. 33185) or Canadian postal code (e.g. M1B 2W1)." : "Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).").Append("</span></div>")
            .Append("<div class='am-al-nf' data-al-lnf='1' role='alert' hidden><div class='am-al-nf-head'><span class='am-al-nf-i' aria-hidden='true'>!</span><strong>Address Not Found</strong></div><p>No matching address was found for postal code <b data-al-lnfcode='1'></b>. Please verify the postal code and search again, or enter the address manually.</p></div>")
            .Append("<div class='am-al-results'><label for='").Append(prefix).Append("_hits'>Select Address from Search Results</label><select id='").Append(prefix).Append("_hits' data-al-lhits='1' disabled><option value=''>Select an address from the list below</option></select></div>")
            .Append(sd ? "</section></div>" : "").Append("<div class='am-al-details'><div class='am-al-details-head'><h3>").Append(sd ? H(std) : "Address Details").Append("</h3>").Append(sd && !verify ? "<span class='am-al-pill'>Address Details</span>" : verify ? Pill(a.Any ? (F(prefix, "_src") == "manual" ? "Pending" : status ?? "") : null, true) : "<span class='am-al-pill' data-al-lmode='1'>" + (a.Any ? "Selected Address" : "Manual Entry") + "</span>").Append("</div><div class='am-al-grid'>")
            .Append(fld("Street Number " + req, "_no", a.StreetNo, sd ? "Enter street number" : "e.g. 3112", " maxlength='20'" + rq, ""))
            .Append(fld("Address Line 1 " + req, "_line1", a.Line1, sd ? "Enter address line 1" : "e.g. Krome Ave", " maxlength='100' autocomplete='address-line1'" + rq, " am-al-span2"))
            .Append(fld("Address Line 2 " + opt, "_line2", a.Line2, sd ? "Enter address line 2 (e.g. Unit, Floor, etc.)" : "e.g. Unit, Floor, Building, etc.", " maxlength='100' autocomplete='address-line2'", ""))
            .Append("<div class='am-al-f'><label for='").Append(prefix).Append("_suitetype'>Suite Type ").Append(opt).Append("</label><select id='").Append(prefix).Append("_suitetype' name='").Append(prefix).Append("_suitetype'>").Append(Opts(SuiteTypes, a.SuiteType, sd ? "Select" : "Please Select")).Append("</select></div>")
            .Append(fld("Suite Number " + opt, "_suiteno", a.SuiteNo, sd ? "Enter suite number" : "e.g. 200", " maxlength='20'", ""))
            .Append("<div class='am-al-subhead' role='heading' aria-level='4'>City, State/Province and Country</div>")
            .Append(fld("City " + req, "_city", a.City, sd ? "Enter city" : us ? "e.g. Miami" : "e.g. Mississauga", " maxlength='50' autocomplete='address-level2'" + rq, ""))
            .Append("<div class='am-al-f'><label for='").Append(prefix).Append("_prov'>Province / State ").Append(req).Append("</label><select id='").Append(prefix).Append("_prov' name='").Append(prefix).Append("_prov' data-al-lprov='1'").Append(rq).Append(">")
            .Append(Opts(us ? States : Canada, a.Province, sd ? "Select" : us ? "Select state" : "Select province")).Append("</select></div>")
            .Append(fld("Postal / ZIP Code " + req, "_postal", a.Postal, sd ? "Enter postal code" : us ? "e.g. 33185" : "e.g. L5W 0A2", " maxlength='10' autocomplete='postal-code'" + rq, ""))
            .Append("<div class='am-al-f am-al-span2 am-al-countryf'><label for='").Append(prefix).Append("_country'>Country ").Append(req).Append("</label><select id='").Append(prefix).Append("_country' name='").Append(prefix).Append("_country' data-al-dcountry='1'>").Append(Opts(new[] { "Canada", "United States" }, a.Country, null)).Append("</select></div>")
            .Append(sd ? "</div></div></div>" : "</div></div></section></div></div>");
        return sb.ToString();
    }
    // ---- v57 (PPT s4): Address Verification status + pending queue (AddressMaster Verifed=0 + AnyminuteAddressReview Pending) ----
    // status: "Verified" | "Pending" | "Rejected" | "" (not in the verification table) | null (no address). mode=true adds data-al-lmode.
    public static string Pill(string status, bool mode)
    {
        string t = status == null ? "Manual Entry" : status == "Verified" ? "Verified Address" : status == "Pending" ? "Pending Verification" : status == "Rejected" ? "Address Rejected" : "Not Verified";
        string tone = status == "Verified" ? " am-al-pill-ok" : status == "Pending" ? " am-al-pill-pend" : status == "Rejected" ? " am-al-pill-bad" : status == null ? "" : " am-al-pill-none";
        return "<span class='am-al-pill" + tone + "'" + (mode ? " data-al-lmode='1'" : "") + ">" + t + "</span>";
    }
    public static string Source(string prefix) { return F(prefix, "_src"); }
    static string Full(Addr a) { return (a.StreetNo + " " + a.Line1).Trim(); }
    static string Cp(string p) { return (p ?? "").ToUpperInvariant().Replace(" ", "").Replace("-", "").Trim(); }
    public static bool ReviewReady(SqlConnection c) { return AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteAddressReview"); }
    // Matches on postal code + street (Address Line 1, or Street Number + Line 1), on the stored row or the original submitted values.
    public static string Status(SqlConnection c, SqlTransaction tx, Addr a, bool rv)
    {
        if (a == null || !a.Any) return null;
        try
        {
            var t = AnyminuteTimesheetRepair.Query(c, tx, rv
                ? @"SELECT TOP 1 CASE WHEN a.Verifed=1 THEN 'Verified' WHEN r.Status='Rejected' THEN 'Rejected' ELSE 'Pending' END St FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId
                    WHERE (REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(a.Street_Name))) IN (UPPER(@l1),UPPER(@f),UPPER(@b)))
                       OR (REPLACE(REPLACE(UPPER(ISNULL(r.OrigPostal,'')),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(ISNULL(r.OrigStreet,'')))) IN (UPPER(@l1),UPPER(@f),UPPER(@o),UPPER(@b)))
                    ORDER BY CASE WHEN a.Verifed=1 THEN 0 WHEN ISNULL(r.Status,'Pending')='Pending' THEN 1 ELSE 2 END, a.AddressMasterId DESC"
                : @"SELECT TOP 1 CASE WHEN a.Verifed=1 THEN 'Verified' ELSE 'Pending' END St FROM AddressMaster a
                    WHERE REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(a.Street_Name))) IN (UPPER(@l1),UPPER(@f),UPPER(@b)) ORDER BY CASE WHEN a.Verifed=1 THEN 0 ELSE 1 END, a.AddressMasterId DESC",
                "@p", Cp(a.Postal), "@l1", a.Line1.Trim(), "@f", Full(a), "@o", Orig(a), "@b", NoNum(a.Line1));
            return t.Rows.Count == 1 ? Convert.ToString(t.Rows[0][0]) : "";
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Warn("Address status lookup failed", ex); return ""; }
    }
    // Original submitted street line kept on the review row: "168 Bloor St W Unit 4" (Street_Name keeps Address Line 1, as the lookup rows do).
    static string Orig(Addr a) { return (Full(a) + (a.Line2.Length > 0 ? " " + a.Line2 : "") + (a.SuiteNo.Length > 0 ? " " + (a.SuiteType.Length > 0 ? a.SuiteType : "Suite") + " " + a.SuiteNo : "")).Trim(); }
    // A saved address that is not in the Verified Address Database becomes a Pending Verification row (an existing pending row is
    // reused; a rejected one gets a fresh submission). Returns the resulting status ("Verified" / "Pending").
    public static string QueuePending(SqlConnection c, SqlTransaction tx, Addr a, bool rv, string submittedBy, string note)
    {
        if (a == null || !a.Any) return null;
        string st = Status(c, tx, a, rv);
        if (st == "Verified") return st;
        string who = submittedBy ?? ""; if (who.Length > 150) who = who.Substring(0, 150);
        string n = note ?? ""; if (n.Length > 500) n = n.Substring(0, 500);
        if (st == "Pending")
        {
            if (rv) AnyminuteTimesheetRepair.Execute(c, tx, @"UPDATE r SET SubmittedBy=@w FROM AnyminuteAddressReview r JOIN AddressMaster a ON a.AddressMasterId=r.AddressMasterId
                WHERE ISNULL(a.Verifed,0)<>1 AND r.Status='Pending' AND ISNULL(r.SubmittedBy,'')='' AND REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(a.Street_Name))) IN (UPPER(@l1),UPPER(@f),UPPER(@b))",
                "@w", who, "@p", Cp(a.Postal), "@l1", a.Line1.Trim(), "@f", Full(a), "@b", NoNum(a.Line1));
            return st;
        }
        object id = AnyminuteTimesheetRepair.Query(c, tx, "INSERT AddressMaster(Street_Name,City,Province,PostalCode,Verifed) VALUES(@s,@c,@pr,@pc,0); SELECT CAST(SCOPE_IDENTITY() AS int)",
            "@s", a.Line1.Trim(), "@c", a.City, "@pr", a.Province, "@pc", a.Postal).Rows[0][0];
        // TR_AnyminuteAddressMaster_Submitted (REQUIRED_08) may already have written the review row; either way it ends Pending with the submitter.
        if (rv) AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteAddressReview WHERE AddressMasterId=@i)
                INSERT AnyminuteAddressReview(AddressMasterId,SubmittedBy,SubmittedAtUTC,OrigStreet,OrigCity,OrigProvince,OrigCountry,OrigPostal,Status,Country,Notes) VALUES(@i,@w,SYSUTCDATETIME(),@o,@c,@pr,@co,@pc,'Pending',@co,@n)
            ELSE UPDATE AnyminuteAddressReview SET SubmittedBy=@w,SubmittedAtUTC=ISNULL(SubmittedAtUTC,SYSUTCDATETIME()),OrigStreet=@o,OrigCountry=@co,Country=@co,Status='Pending',Notes=@n WHERE AddressMasterId=@i",
            "@i", id, "@w", who, "@o", Orig(a), "@c", a.City, "@pr", a.Province, "@co", a.Country, "@pc", a.Postal, "@n", n);
        return "Pending";
    }
    // v80: "19 Neelands Cres" typed in Address Line 1 (no separate Street Number) still matches the street row "NEELANDS CRES".
    static string NoNum(string s) { s = (s ?? "").Trim(); var m = Regex.Match(s, @"^\d+[A-Za-z]?(-\d+)?\s+(.+)$"); return m.Success ? m.Groups[2].Value.Trim() : s; }
    // City / Province name for a legacy value that may be a CityMaster / ProvinceMaster id (or 0 = none).
    static string MasterName(SqlConnection c, SqlTransaction tx, string value, int fallbackId, bool city)
    {
        value = (value ?? "").Trim(); int id;
        if (value.Length > 0 && !Int32.TryParse(value, out id)) return value;
        if (!Int32.TryParse(value, out id) || id <= 0) id = fallbackId;
        if (id <= 0) return "";
        var t = AnyminuteTimesheetRepair.Query(c, tx, city ? "SELECT CityName FROM CityMaster WHERE CityId=@i" : "SELECT ProvinceName FROM ProvinceMaster WHERE ProvinceId=@i", "@i", id);
        return t.Rows.Count == 1 ? Convert.ToString(t.Rows[0][0]).Trim() : "";
    }
    public static Addr FromLegacy(SqlConnection c, SqlTransaction tx, string streetNo, string street, string line2, string suiteType, string suiteNo, string city, int cityId, string prov, int provId, string postal)
    {
        var a = new Addr { StreetNo = (streetNo ?? "").Trim(), Line1 = (street ?? "").Trim(), Line2 = (line2 ?? "").Trim(), SuiteNo = (suiteNo ?? "").Trim(),
            SuiteType = SuiteTypes.FirstOrDefault(t => t.Equals((suiteType ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) ?? "" };
        a.Country = Regex.IsMatch(Cp(postal), @"^\d{5}(\d{4})?$") ? "United States" : "Canada";
        a.Postal = NormalPostal(postal, a.Country);
        a.City = MasterName(c, tx, city, cityId, true); a.Province = MasterName(c, tx, prov, provId, false);
        if (a.StreetNo.Length == 0 && NoNum(a.Line1) != a.Line1) { a.StreetNo = a.Line1.Substring(0, a.Line1.Length - NoNum(a.Line1).Length).Trim(); a.Line1 = NoNum(a.Line1); }
        return a;
    }
    public static int MaxMasterId(SqlConnection c, SqlTransaction tx)
    {
        try { return Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "SELECT ISNULL(MAX(AddressMasterId),0) FROM AddressMaster").Rows[0][0]); }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("AddressMaster max id read failed", ex); return -1; }
    }
    // v80 root cause fix (Fawaz Thakor, "19 Neelands Cres ... M1E 0B6" already Verified but listed Pending with City 0 / Prov 0):
    // the legacy Sp_InsertEmpAddress / Sp_InsertBusinessAddress add an AddressMaster row whenever no row has the EXACT postal
    // text (verified rows hold "M1E0B6", forms send "M1E 0B6") or the exact street, storing @CityId/@ProvinceId (0 when the caller
    // did not set them) and Street_Name without the number; TR_AnyminuteAddressMaster_Submitted then makes it Pending.
    // Call right after those SPs with the MAX(AddressMasterId) read before: rows they just added for this address are removed when
    // the address is already Verified (or already waiting in Pending), else kept once with real names + the full submitted street.
    public static void AfterLegacyInsert(SqlConnection c, SqlTransaction tx, int afterId, Addr a)
    {
        if (afterId < 0 || a == null || !a.Any || Cp(a.Postal).Length == 0) return;
        try
        {
            const string Same = "REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(a.Street_Name))) IN (UPPER(@l1),UPPER(@f),UPPER(@b))";
            object[] args = { "@m", afterId, "@p", Cp(a.Postal), "@l1", a.Line1.Trim(), "@f", Full(a), "@b", NoNum(a.Line1) };
            var fresh = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.AddressMasterId FROM AddressMaster a WHERE a.AddressMasterId>@m AND ISNULL(a.Verifed,0)<>1 AND " + Same + " ORDER BY a.AddressMasterId", args)
                .Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r[0])).ToList();
            if (fresh.Count == 0) return;
            bool rv = AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAddressReview");
            string ids = String.Join(",", fresh.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
            bool known = AnyminuteTimesheetRepair.Query(c, tx, "SELECT TOP 1 1 FROM AddressMaster a" + (rv ? " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId" : "")
                + " WHERE a.AddressMasterId NOT IN (" + ids + ") AND " + Same + " AND (a.Verifed=1" + (rv ? " OR ISNULL(r.Status,'Pending')='Pending'" : " OR 1=1") + ")", args).Rows.Count == 1;
            var drop = known ? fresh : fresh.Skip(1).ToList();
            if (drop.Count > 0)
            {
                string d = String.Join(",", drop.Select(x => x.ToString(CultureInfo.InvariantCulture)).ToArray());
                if (rv) AnyminuteTimesheetRepair.Execute(c, tx, "DELETE FROM AnyminuteAddressReview WHERE AddressMasterId IN (" + d + ")");
                AnyminuteTimesheetRepair.Execute(c, tx, "DELETE FROM AddressMaster WHERE AddressMasterId IN (" + d + ") AND ISNULL(Verifed,0)<>1");
            }
            if (known) return;
            int keep = fresh[0];
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Street_Name=@s,City=@c,Province=@pr,PostalCode=@pc WHERE AddressMasterId=@i",
                "@s", a.Line1.Trim(), "@c", a.City, "@pr", a.Province, "@pc", a.Postal, "@i", keep);
            if (rv) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET OrigStreet=@o,OrigCity=@c,OrigProvince=@pr,OrigPostal=@pc,OrigCountry=@co,Country=@co WHERE AddressMasterId=@i",
                "@o", Orig(a), "@c", a.City, "@pr", a.Province, "@pc", a.Postal, "@co", a.Country, "@i", keep);
        }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("Legacy address reconcile failed", ex); }
    }
    // JSON for the lookup endpoint: [{street,city,province}] (title case), up to 20 rows.
    public static string LookupJson(string postal)
    {
        var sb = new StringBuilder("[");
        try
        {
            var t = AnyminuteAddressLookup.Search(postal);
            var ti = CultureInfo.InvariantCulture.TextInfo;
            Func<object, string> tc = o => { string s = Convert.ToString(o ?? "").Trim(); return HttpUtility.JavaScriptStringEncode(s == s.ToUpperInvariant() ? ti.ToTitleCase(s.ToLowerInvariant()) : s); };
            if (t != null) foreach (DataRow r in t.Rows.Cast<DataRow>().Take(20))
                sb.Append(sb.Length > 1 ? "," : "").Append("{\"street\":\"").Append(tc(r["StreetName"])).Append("\",\"city\":\"").Append(tc(r["City"])).Append("\",\"province\":\"").Append(tc(r["Province"])).Append("\"}");
        }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("Support address lookup failed", ex); }
        return sb.Append("]").ToString();
    }
}
