using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-28 v25 deck 1 (Address Verification): Pending / Verified / Rejected tabs over the existing AddressMaster
// lookup table (Verifed=1 is the verified address database). Edit & Verify keeps the original submitted address in
// AnyminuteAddressReview (REQUIRED_07) and records corrected address, admin, date/time and reason in AnyminuteSupportAudit.
// Verify Address (format/province check) is separate from Approve & Add to Database, which re-verifies on the server.
// 2026-10-28 v26 strict pass: Verify Address now also checks the existing address lookup (verified AddressMaster rows +
// employee addresses for the same postal code, as SearchAddress / AnyminuteAddressLookup use) and offers suggestions;
// city / province ids are shown as names and kept as ids on approve; Submitted By comes from the review row or the
// matching employee address, Submitted Date from the review row (REQUIRED_08 trigger records it for new addresses).
public partial class AnyminuteSupportAddressesPage : System.Web.UI.Page
{
    const int PageSize = 8;
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (SA.Get("verify") == "1") { VerifyJson(); return; }
        if (SA.Get("template") == "1") { Template(); return; }
        if (Form != null) Form.Enctype = "multipart/form-data"; // v71: Import Address file upload
        if (IsPostBack && (SA.Form("sa_op") == "addaddr" || SA.Form("sa_op") == "importaddr")) { if (SA.Form("sa_op") == "addaddr") AddAddress(); else ImportAddress(); return; }
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(); return; } // v78: Act also handles restore/delete/bulk_*
        string tab = SA.Get("tab"); if (tab != "verified" && tab != "rejected") tab = "pending";
        string q = SA.Get("q"), country = SA.Get("country"), prov = SA.Get("prov"), date = SA.Get("date");
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        int id; Int32.TryParse(SA.Get("id"), out id);
        Flash.Text = SA.TakeFlash() + TakeImportSummary();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteAddressReview") && SA.Has(c, "AnyminuteSupportAudit");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteAddressReview", "AnyminuteSupportAudit");
            string rj = ready ? " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId" : "";
            string status = ready ? "CASE WHEN a.Verifed=1 THEN 'Verified' WHEN r.Status='Rejected' THEN 'Rejected' ELSE 'Pending' END" : "CASE WHEN a.Verifed=1 THEN 'Verified' ELSE 'Pending' END";
            string cols = ready ? ",r.SubmittedBy,r.SubmittedAtUTC,r.Country,r.ReviewedAtUTC,r.ReviewedByName,r.Notes" : ",CAST(NULL AS nvarchar(150)) SubmittedBy,CAST(NULL AS datetime2) SubmittedAtUTC,CAST(NULL AS nvarchar(60)) Country,CAST(NULL AS datetime2) ReviewedAtUTC,CAST(NULL AS nvarchar(150)) ReviewedByName,CAST(NULL AS nvarchar(500)) Notes";
            var all = SA.Q(c, "SELECT a.AddressMasterId,a.Street_Name," + Names + ",a.PostalCode," + status + " St" + cols + " FROM AddressMaster a" + NameJoins + rj);
            Func<string, int> count = s => all.Rows.Cast<DataRow>().Count(r => SA.S(r["St"]) == s);
            string wanted = tab == "verified" ? "Verified" : tab == "rejected" ? "Rejected" : "Pending";
            var rows = all.Rows.Cast<DataRow>().Where(r => SA.S(r["St"]) == wanted).Where(r =>
            {
                string ctry = SA.S(r["Country"]); if (ctry.Length == 0) ctry = SA.GuessCountry(SA.S(r["PostalCode"]));
                if (country.Length > 0 && ctry != country) return false;
                if (prov.Length > 0 && !SA.S(r["Province"]).Equals(prov, StringComparison.OrdinalIgnoreCase)) return false;
                if (q.Length > 0 && !(SA.S(r["Street_Name"]) + " " + SA.S(r["City"]) + " " + SA.S(r["PostalCode"]) + " " + SA.S(r["SubmittedBy"])).ToLowerInvariant().Contains(q.ToLowerInvariant())) return false;
                if (date.Length > 0 && r["SubmittedAtUTC"] != DBNull.Value)
                {
                    int days = date == "7" ? 7 : date == "30" ? 30 : 0;
                    if (days > 0 && Convert.ToDateTime(r["SubmittedAtUTC"]) < DateTime.UtcNow.AddDays(-days)) return false;
                }
                return true;
            }).OrderByDescending(r => r["SubmittedAtUTC"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["SubmittedAtUTC"])).ThenByDescending(r => Convert.ToInt32(r["AddressMasterId"])).ToList();

            Func<string, string, string, int, string> tabLink = (key, label, tone, n) => "<a href='SupportAddresses.aspx?tab=" + key + "' class='" + (tab == key ? "on" : "") + "'" + (tab == key ? " aria-current='page'" : "") + ">" + label + " <span class='sa-count sa-" + tone + "'>" + n.ToString("#,0", CultureInfo.InvariantCulture) + "</span></a>";
            Tabs.Text = "<nav class='sa-tabs'>" + tabLink("pending", "Pending Verification", "red", count("Pending")) + tabLink("verified", "Verified Addresses", "green", count("Verified")) + tabLink("rejected", "Rejected Addresses", "grey", count("Rejected")) + "</nav>";
            var provinces = all.Rows.Cast<DataRow>().Select(r => SA.S(r["Province"])).Where(p => p.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(p => p).Take(80);
            Filters.Text = SelBar(tab) + "<div class='sa-filters' data-sa-get='SupportAddresses.aspx'>"
                + "<label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='" + SA.A(q) + "' placeholder='Search address, city, or subscriber...' /></label>"
                + "<label><span>Country</span><select name='country'>" + SA.Options(SA.Countries, country, "All") + "</select></label>"
                + "<label><span>Province/State</span><select name='prov'>" + SA.Options(provinces, prov, "All") + "</select></label>"
                + "<label><span>Status</span><select name='tab'>" + "<option value='pending'" + (tab == "pending" ? " selected" : "") + ">Pending Verification</option><option value='verified'" + (tab == "verified" ? " selected" : "") + ">Verified</option><option value='rejected'" + (tab == "rejected" ? " selected" : "") + ">Rejected</option></select></label>"
                + "<label><span>Submitted Date</span><select name='date'>" + "<option value=''>All Dates</option><option value='7'" + (date == "7" ? " selected" : "") + ">Last 7 days</option><option value='30'" + (date == "30" ? " selected" : "") + ">Last 30 days</option></select></label>"
                + "<button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>";

            string baseQ = "tab=" + tab + "&q=" + HttpUtility.UrlEncode(q) + "&country=" + HttpUtility.UrlEncode(country) + "&prov=" + HttpUtility.UrlEncode(prov) + "&date=" + date;
            var sb = new StringBuilder("<input type='hidden' name='sa_id' value='0' data-sa-id='1' /><div class='sa-table-wrap sa-av-wrap'><table class='sa-table' data-sa-avtable='1'><thead><tr><th><input type='checkbox' aria-label='Select all' data-sa-all='sa_pick' /></th><th>Address (Submitted)</th><th>City</th><th>Prov/State</th><th>Country</th><th>Postal Code</th><th>Submitted By</th><th>Date</th><th>Actions</th></tr></thead><tbody>");
            foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
            {
                int rid = Convert.ToInt32(r["AddressMasterId"]); string ctry = SA.S(r["Country"]); if (ctry.Length == 0) ctry = SA.GuessCountry(SA.S(r["PostalCode"]));
                string who = Submitter(c, r);
                sb.Append("<tr").Append(rid == id ? " class='sa-selected'" : "").Append("><td><input type='checkbox' name='sa_pick' value='").Append(rid).Append("' aria-label='Select' /></td><td>").Append(SA.H(r["Street_Name"])).Append("</td><td>").Append(SA.H(r["City"]))
                  .Append("</td><td>").Append(SA.H(r["Province"])).Append("</td><td>").Append(SA.H(ctry)).Append("</td><td>").Append(SA.H(r["PostalCode"])).Append("</td><td>")
                  .Append(who != null ? SA.H(who) : "<span class='sa-muted' title='No subscriber, employee, business or reseller address uses this address'>Not linked</span>").Append("</td><td>").Append(r["SubmittedAtUTC"] != DBNull.Value ? SA.Day(r["SubmittedAtUTC"]) : "<span class='sa-muted' title='Added before submission dates were recorded'>&mdash;</span>").Append("</td><td class='sa-actions sa-nowrap'>")
                  .Append(RowActions(tab, rid, baseQ, page)).Append("</td></tr>");
            }
            if (rows.Count == 0) sb.Append("<tr><td colspan='9' class='sa-empty'>No addresses in this list.</td></tr>");
            Table.Text = sb.Append("</tbody></table></div>").ToString() + SA.Pager(rows.Count, page, PageSize, baseQ);

            var pick = id > 0 ? all.Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["AddressMasterId"]) == id) : null;
            Panel.Text = pick == null ? "" : EditPanel(c, pick, ready, baseQ + "&page=" + page);
                        if (SA.Get("add") == "1") Panel.Text += AddModal(ready);
            else if (SA.Get("import") == "1") Panel.Text += ImportModal(ready);
        }
    }
    // City / Province hold CityMaster / ProvinceMaster ids in most rows (and names in a few): show names, keep raw values.
    const string Names = "COALESCE(ci.CityName,a.City) City,COALESCE(pr.ProvinceName,a.Province) Province,a.City CityRaw,a.Province ProvRaw";
    const string NameJoins = " LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(a.City AS nvarchar(50)) LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=CAST(a.Province AS nvarchar(50))";
    // Recorded submitter, else the subscriber / employee whose own address is this address (same postal code and street).
    static string Submitter(SqlConnection c, DataRow r) { return Submitter(c, null, r); }
    // v42 (PPT 05 s2-3, "When I try to approve I got below message"): Approve / Reject run inside a transaction, so every
    // lookup on that connection must enlist in it (a non-enlisted command threw InvalidOperationException -> error page).
    static string Submitter(SqlConnection c, SqlTransaction tx, DataRow r)
    {
        if (SA.S(r["SubmittedBy"]).Length > 0) return SA.S(r["SubmittedBy"]);
        try
        {
            var t = AnyminuteTimesheetRepair.Query(c, tx, @"SELECT TOP 1 LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name,ae.UserType FROM EmpAddress e JOIN AddEmployee ae ON ae.ADDRESSID=e.EmpAddressId
                WHERE REPLACE(REPLACE(UPPER(e.Postalcode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(e.StreetName)))=UPPER(LTRIM(RTRIM(@s))) ORDER BY CASE WHEN ae.UserType=1 THEN 0 ELSE 1 END,ae.EmployeeID",
                "@p", SA.CompactPostal(SA.S(r["PostalCode"])), "@s", SA.S(r["Street_Name"]));
            if (t.Rows.Count == 1 && SA.S(t.Rows[0]["Name"]).Length > 0) return SA.S(t.Rows[0]["Name"]) + (SA.S(t.Rows[0]["UserType"]) == "1" ? " (Subscriber)" : " (Employee)");
        }
        catch (SqlException) { }
        catch (InvalidOperationException) { }
        // v48 (NOTES_v47 deferred "Not linked"): also link addresses used by a saved business (BusinessAddress via AddBusiness.ADDRESSID)
        // or a reseller (AnyminuteReseller.Address, composed text) - existing tables only, no schema change.
        try
        {
            var b = AnyminuteTimesheetRepair.Query(c, tx, @"SELECT TOP 1 LTRIM(RTRIM(ab.CompanyName)) Name FROM BusinessAddress e JOIN AddBusiness ab ON ab.ADDRESSID=e.BusinessAddressId
                WHERE REPLACE(REPLACE(UPPER(e.Postalcode),' ',''),'-','')=@p AND UPPER(LTRIM(RTRIM(e.StreetName)))=UPPER(LTRIM(RTRIM(@s))) ORDER BY ab.BusinessID",
                "@p", SA.CompactPostal(SA.S(r["PostalCode"])), "@s", SA.S(r["Street_Name"]));
            if (b.Rows.Count == 1 && SA.S(b.Rows[0]["Name"]).Length > 0) return SA.S(b.Rows[0]["Name"]) + " (Business)";
            string street = SA.S(r["Street_Name"]).Trim(), postal = SA.CompactPostal(SA.S(r["PostalCode"]));
            if (street.Length > 0 && postal.Length > 0)
            {
                var rs = AnyminuteTimesheetRepair.Query(c, tx, @"IF OBJECT_ID('dbo.AnyminuteReseller','U') IS NOT NULL
                    SELECT TOP 1 ResellerName Name FROM dbo.AnyminuteReseller WHERE REPLACE(REPLACE(UPPER(ISNULL(Address,'')),' ',''),'-','') LIKE '%'+@p+'%' AND UPPER(ISNULL(Address,'')) LIKE '%'+UPPER(@s)+'%' ORDER BY ResellerID
                    ELSE SELECT CAST(NULL AS nvarchar(150)) Name WHERE 1=0", "@p", postal, "@s", street);
                if (rs.Rows.Count == 1 && SA.S(rs.Rows[0]["Name"]).Length > 0) return SA.S(rs.Rows[0]["Name"]) + " (Reseller)";
            }
        }
        catch (SqlException) { }
        catch (InvalidOperationException) { }
        return null;
    }
    // GET SupportAddresses.aspx?verify=1&id=&street=&city=&prov=&country=&postal= -> {level,message,suggestions[]} for the Verify Address button.
    void VerifyJson()
    {
        int id; Int32.TryParse(SA.Get("id"), out id);
        string level, message; DataTable known;
        using (var c = SA.Open()) message = SA.LookupVerify(c, id, SA.Get("street"), SA.Get("city"), SA.Get("prov"), SA.Get("country"), SA.Get("postal"), out level, out known);
        var sugg = new List<Dictionary<string, string>>();
        if (known != null) foreach (DataRow k in known.Rows) sugg.Add(new Dictionary<string, string> { { "street", SA.S(k["Street"]) }, { "city", SA.S(k["City"]) }, { "province", SA.S(k["Province"]) }, { "postal", SA.S(k["PostalCode"]) }, { "source", SA.S(k["Source"]) } });
        Response.Clear(); Response.ContentType = "application/json"; Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Write(new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(new Dictionary<string, object> { { "level", level }, { "message", message }, { "suggestions", level == "match" ? new List<Dictionary<string, string>>() : sugg } }));
        Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest();
    }
    // Keep CityMaster / ProvinceMaster ids when the row used ids (as AnyminuteAddressLookup joins on them); text rows stay text.
    static string MasterValue(SqlConnection c, SqlTransaction tx, string raw, string table, string nameCol, string idCol, string name)
    {
        int n; if (!Int32.TryParse(raw, out n)) return name;
        var t = AnyminuteTimesheetRepair.Query(c, tx, "IF NOT EXISTS(SELECT 1 FROM dbo." + table + " WITH(UPDLOCK,HOLDLOCK) WHERE " + nameCol + "=@n) INSERT dbo." + table + "(" + nameCol + ",Duplicate) VALUES(@n,0); SELECT TOP(1) " + idCol + " FROM dbo." + table + " WHERE " + nameCol + "=@n ORDER BY " + idCol, "@n", name.Trim().ToUpperInvariant());
        return Convert.ToString(t.Rows[0][0], CultureInfo.InvariantCulture);
    }
    string EditPanel(SqlConnection c, DataRow r, bool ready, string back)
    {
        int id = Convert.ToInt32(r["AddressMasterId"]);
        DataRow review = null;
        if (ready) { var t = SA.Q(c, "SELECT * FROM AnyminuteAddressReview WHERE AddressMasterId=@i", "@i", id); if (t.Rows.Count == 1) review = t.Rows[0]; }
        string os = review != null ? SA.S(review["OrigStreet"]) : SA.S(r["Street_Name"]), oc = review != null ? SA.S(review["OrigCity"]) : SA.S(r["City"]),
               op = review != null ? SA.S(review["OrigProvince"]) : SA.S(r["Province"]), opc = review != null ? SA.S(review["OrigPostal"]) : SA.S(r["PostalCode"]);
        // Original values were captured as stored (ids); show names.
        if (oc == SA.S(r["CityRaw"])) oc = SA.S(r["City"]); else { int x; if (Int32.TryParse(oc, out x)) { var n = SA.Q(c, "SELECT CityName FROM CityMaster WHERE CityId=@i", "@i", x); if (n.Rows.Count == 1) oc = SA.S(n.Rows[0][0]); } }
        if (op == SA.S(r["ProvRaw"])) op = SA.S(r["Province"]); else { int x; if (Int32.TryParse(op, out x)) { var n = SA.Q(c, "SELECT ProvinceName FROM ProvinceMaster WHERE ProvinceId=@i", "@i", x); if (n.Rows.Count == 1) op = SA.S(n.Rows[0][0]); } }
        string who = Submitter(c, r);
        string octry = review != null && SA.S(review["OrigCountry"]).Length > 0 ? SA.S(review["OrigCountry"]) : SA.GuessCountry(opc);
        string ctry = SA.S(r["Country"]).Length > 0 ? SA.S(r["Country"]) : SA.GuessCountry(SA.S(r["PostalCode"]));
        bool pending = SA.S(r["St"]) == "Pending", verified = SA.S(r["St"]) == "Verified";
        bool rejectMode = SA.Get("reject") == "1" && (pending || verified);
        string dis = (pending || rejectMode) && ready ? "" : " disabled";
        var sb = new StringBuilder("<aside class='sa-card sa-panel' id='sa-edit' aria-label='Edit and verify address'><div class='sa-panel-head'><h2>Edit &amp; Verify Address</h2><a href='SupportAddresses.aspx?" + SA.A(back) + "' class='sa-x' aria-label='Close'>&times;</a></div>");
        sb.Append("<h3>Original Submitted Address</h3><div class='sa-original'><p>").Append(SA.H(os + ", " + oc + ", " + op + ", " + octry + ", " + opc)).Append("</p><dl><dt>Submitted By:</dt><dd>")
          .Append(who != null ? SA.H(who) : "Not linked to a subscriber, employee, business or reseller address").Append("</dd><dt>Submitted Date:</dt><dd>")
          .Append(review != null && review["SubmittedAtUTC"] != DBNull.Value ? SA.When(review["SubmittedAtUTC"]).Replace("<br />", " ") : "Not recorded (added before v26)").Append("</dd></dl></div>");
        sb.Append("<div data-sa-address='1'><input type='hidden' name='sa_id' value='").Append(id).Append("' /><h3>Corrected Address (Edit if needed)</h3>")
          .Append("<label class='sa-field'><span>Street Address *</span><input name='sa_street' maxlength='200' value='").Append(SA.A(r["Street_Name"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<div class='sa-row2'><label class='sa-field'><span>City *</span><input name='sa_city' maxlength='200' value='").Append(SA.A(r["City"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<label class='sa-field'><span>Province / State *</span><input name='sa_prov' maxlength='200' list='sa-prov-list' value='").Append(SA.A(r["Province"])).Append("'").Append(dis).Append(" /></label></div>")
          .Append("<label class='sa-field'><span>Country *</span><select name='sa_country'").Append(dis).Append(">").Append(SA.Options(SA.Countries, ctry, null)).Append("</select></label>")
          .Append("<div class='sa-row2 sa-verify-row'><label class='sa-field'><span>Postal / ZIP / PIN Code *</span><input name='sa_postal' maxlength='20' value='").Append(SA.A(r["PostalCode"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<button type='button' class='sa-btn sa-btn-outline' data-sa-verify='1'").Append(dis).Append(">&#9679; Verify Address</button></div>")
          .Append("<p class='sa-verify-result' data-sa-verify-result='1' role='status'></p>")
          ;
        // v42/v78: Reject opens the panel in reject mode (reject=1) for Pending or Verified so a reason can be entered.
        // Approve stays pending-only. Verified View/Edit stays read-only unless reject mode.
        if (rejectMode)
            sb.Append("<div class='sa-panel-foot sa-one-line'><button type='submit' id='sa-reject' class='sa-btn sa-btn-danger' name='sa_op' value='reject' data-sa-need-notes='Enter the reason for rejecting this address in Note.'").Append(dis).Append(">&#10005; Reject</button>")
              .Append("<a class='sa-btn sa-btn-outline' href='SupportAddresses.aspx?").Append(SA.A(back)).Append("'>Cancel</a></div>");
        else if (pending)
            sb.Append("<div class='sa-panel-foot sa-one-line'><button type='submit' class='sa-btn sa-btn-success' name='sa_op' value='approve' data-sa-approve='1' disabled>&#10003; Approve &amp; Add to Database</button>")
              .Append("<a class='sa-btn sa-btn-outline' href='SupportAddresses.aspx?").Append(SA.A(back)).Append("'>Cancel</a></div>");
        else
            sb.Append("<div class='sa-panel-foot sa-one-line'><a class='sa-btn sa-btn-outline' href='SupportAddresses.aspx?").Append(SA.A(back)).Append("'>Close</a></div>");
        sb.Append("<label class='sa-field'><span>Note</span><textarea name='sa_notes' maxlength='500' rows='3' data-sa-counter='500'").Append(dis).Append(">").Append(review != null ? SA.H(review["Notes"]) : "").Append("</textarea><small data-sa-count='1'>0/500</small></label>");
        if (pending || rejectMode) { }
        else if (review != null)
            sb.Append("<p class='sa-muted'>").Append(SA.H(SA.S(r["St"]) + " by " + SA.S(review["ReviewedByName"]))).Append(" &middot; ").Append(SA.When(review["ReviewedAtUTC"]).Replace("<br />", " ")).Append("</p>");
        sb.Append("</div><datalist id='sa-prov-list'>");
        foreach (var p in new[] { "ON", "QC", "BC", "AB", "MB", "SK", "NS", "NB", "NL", "PE", "NT", "NU", "YT", "NY", "CA", "TX", "FL", "IL", "WA", "MA", "NJ", "England", "Scotland", "Wales" }) sb.Append("<option value='").Append(p).Append("'></option>");
        sb.Append("</datalist>");
        return sb.Append("</aside>").ToString();
    }
    // v78: row View + ⋮ actions and bulk bar (no Edit Selected).
    static string SelBar(string tab)
    {
        var sb = new StringBuilder("<div class='sa-selbar' data-sa-selbar='1' hidden><div class='sa-selbar-left'><span data-sa-selcount='1'>0 addresses selected</span></div><div class='sa-selbar-btns'>");
        if (tab == "rejected")
        {
            sb.Append("<button type='submit' class='sa-btn sa-btn-outline' name='sa_op' value='bulk_restore' data-sa-confirm='Restore the selected addresses to Pending Verification?'>&#8634; Restore Selected</button>");
            sb.Append("<button type='submit' class='sa-btn sa-btn-solid-danger' name='sa_op' value='bulk_delete' data-sa-confirm='Permanently delete the selected addresses? This cannot be undone.'>&#128465; Delete Selected</button>");
        }
        else
        {
            // Pending + Verified: Reject Selected only. Edit Selected removed (red X in held image6).
            sb.Append("<button type='submit' class='sa-btn sa-btn-danger' name='sa_op' value='bulk_reject' data-sa-confirm='Reject the selected addresses?'>&#10005; Reject Selected</button>");
        }
        return sb.Append("</div><button type='button' class='sa-selbar-clear' data-sa-clear='1'>&#10005; Clear Selection</button></div>").ToString();
    }
    static string RowActions(string tab, int rid, string baseQ, int page)
    {
        string href = "SupportAddresses.aspx?" + SA.A(baseQ) + "&amp;page=" + page + "&amp;id=" + rid;
        var sb = new StringBuilder("<a class='sa-btn sa-btn-sm sa-btn-outline' href='").Append(href).Append("'>View</a> ");
        sb.Append("<details class='sa-menu'><summary class='sa-kebab' aria-label='Address actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Address Actions</h4>");
        if (tab == "rejected")
        {
            // Notes: keep View inline; Restore + Delete under ⋮ (images 2/3 showed them inline — move them).
            sb.Append("<button type='submit' class='sa-menu-item' name='sa_op' value='restore' formnovalidate='formnovalidate' data-sa-set='sa_id=").Append(rid).Append("' data-sa-confirm='Restore this address to Pending Verification?'><i aria-hidden='true'>&#8634;</i><span><b>Restore</b></span></button>");
            sb.Append("<button type='submit' class='sa-menu-item sa-menu-danger' name='sa_op' value='delete' formnovalidate='formnovalidate' data-sa-set='sa_id=").Append(rid).Append("' data-sa-confirm='Permanently delete this address? This cannot be undone.'><i aria-hidden='true'>&#128465;</i><span><b>Delete</b></span></button>");
        }
        else if (tab == "verified")
        {
            // Images 1 & 6: ⋮ = Edit, Reject. No Edit Selected in bulk bar.
            sb.Append("<a class='sa-menu-item' href='").Append(href).Append("'><i aria-hidden='true'>&#9998;</i><span><b>Edit</b></span></a>");
            sb.Append("<a class='sa-menu-item sa-menu-danger' href='").Append(href).Append("&amp;reject=1#sa-edit'><i aria-hidden='true'>&#128683;</i><span><b>Reject</b></span></a>");
        }
        else
        {
            // Notes: Pending front=View; ⋮ = Reject and Delete.
            sb.Append("<a class='sa-menu-item sa-menu-danger' href='").Append(href).Append("&amp;reject=1#sa-edit'><i aria-hidden='true'>&#128683;</i><span><b>Reject</b></span></a>");
            sb.Append("<button type='submit' class='sa-menu-item sa-menu-danger' name='sa_op' value='delete' formnovalidate='formnovalidate' data-sa-set='sa_id=").Append(rid).Append("' data-sa-confirm='Permanently delete this address? This cannot be undone.'><i aria-hidden='true'>&#128465;</i><span><b>Delete</b></span></button>");
        }
        return sb.Append("</div></details>").ToString();
    }
    static int[] PickedIds()
    {
        var vals = System.Web.HttpContext.Current.Request.Form.GetValues("sa_pick");
        var ids = new List<int>();
        if (vals != null) foreach (var v in vals) { int n; if (Int32.TryParse((v ?? "").Trim(), out n) && n > 0 && !ids.Contains(n)) ids.Add(n); }
        return ids.ToArray();
    }
    void Act()
    {
        string op = SA.Form("sa_op");
        if (op == "bulk_reject" || op == "bulk_restore" || op == "bulk_delete") { BulkAct(op); return; }
        int id = SA.FormId("sa_id");
        if (op == "restore") { RestoreOne(id); return; }
        if (op == "delete") { DeleteOne(id); return; }
        string street = SA.Form("sa_street"), city = SA.Form("sa_city"), prov = SA.Form("sa_prov"), country = SA.Form("sa_country"), postal = SA.Form("sa_postal"), notes = SA.Form("sa_notes");
        if (notes.Length > 500) notes = notes.Substring(0, 500);
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { SA.Flash(SA.MissingTables); Back(id); return; }
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                var cur = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.AddressMasterId,a.Street_Name,a.PostalCode,a.Verifed," + Names + ",r.Status RStatus,r.SubmittedBy FROM AddressMaster a WITH (UPDLOCK)" + NameJoins + " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE a.AddressMasterId=@i", "@i", id);
                if (cur.Rows.Count != 1) { SA.Flash("That address was not found."); tx.Rollback(); Back(0); return; }
                var a = cur.Rows[0];
                string st = SA.S(a["Verifed"]) == "1" ? "Verified" : (SA.S(a["RStatus"]) == "Rejected" ? "Rejected" : "Pending");
                if (op == "approve" && st != "Pending") { SA.Flash("This address was already reviewed by another Support Admin."); tx.Rollback(); Back(id); return; }
                if (op == "reject" && st == "Rejected") { SA.Flash("This address was already rejected."); tx.Rollback(); Back(id); return; }
                string before = SA.S(a["Street_Name"]) + ", " + SA.S(a["City"]) + ", " + SA.S(a["Province"]) + ", " + SA.S(a["PostalCode"]);
                AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteAddressReview WHERE AddressMasterId=@i)
                    INSERT AnyminuteAddressReview(AddressMasterId,OrigStreet,OrigCity,OrigProvince,OrigCountry,OrigPostal,Status) VALUES(@i,@s,@c,@p,@co,@pc,'Pending')",
                    "@i", id, "@s", SA.S(a["Street_Name"]), "@c", SA.S(a["City"]), "@p", SA.S(a["Province"]), "@co", SA.GuessCountry(SA.S(a["PostalCode"])), "@pc", SA.S(a["PostalCode"]));
                string who = Submitter(c, tx, a);
                if (who != null && SA.S(a["SubmittedBy"]).Length == 0)
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET SubmittedBy=@w WHERE AddressMasterId=@i AND SubmittedBy IS NULL", "@w", who.Length > 150 ? who.Substring(0, 150) : who, "@i", id);
                if (op == "approve")
                {
                    string level; DataTable known;
                    string verdict = SA.LookupVerify(c, tx, id, street, city, prov, country, postal, out level, out known);
                    if (level == "error") { tx.Rollback(); SA.Flash("Not approved: " + verdict); Back(id); return; }
                    postal = SA.NormalizePostal(country, postal);
                    string cityValue = MasterValue(c, tx, SA.S(a["CityRaw"]), "CityMaster", "CityName", "CityId", city), provValue = MasterValue(c, tx, SA.S(a["ProvRaw"]), "ProvinceMaster", "ProvinceName", "ProvinceId", prov);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Street_Name=@s,City=@c,Province=@p,PostalCode=@pc,Verifed=1 WHERE AddressMasterId=@i", "@s", street, "@c", cityValue, "@p", provValue, "@pc", postal, "@i", id);
                    notes = (notes + " [Verify: " + verdict + "]").Trim(); if (notes.Length > 500) notes = notes.Substring(0, 500);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Verified',Country=@co,Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
                        "@co", country, "@n", notes, "@b", SA.ActorId(), "@bn", SA.ActorName(), "@i", id);
                    string after = street + ", " + city + ", " + prov + ", " + country + ", " + postal;
                    SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), before == after ? "Approved" : "Corrected & Approved", "Pending", "Verified", before, after, notes);
                    tx.Commit(); SA.Flash("Address verified and added to the Verified Address Database.");
                }
                else if (op == "reject")
                {
                    if (notes.Length == 0) { tx.Rollback(); SA.Flash("Enter the reason for rejecting this address in Note."); Back(id); return; }
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Verifed=0 WHERE AddressMasterId=@i", "@i", id);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Rejected',Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
                        "@n", notes, "@b", SA.ActorId(), "@bn", SA.ActorName(), "@i", id);
                    SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Rejected", st, "Rejected", before, null, notes);
                    tx.Commit(); SA.Flash("Address rejected and returned for correction.");
                }
                else tx.Rollback();
            }
        }
        Back(0);
    }
    void BulkAct(string op)
    {
        var ids = PickedIds();
        if (ids.Length == 0) { SA.Flash("Select at least one address."); Go("SupportAddresses.aspx?tab=" + (op == "bulk_restore" || op == "bulk_delete" ? "rejected" : SA.Get("tab"))); return; }
        int ok = 0; var errors = new List<string>();
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { SA.Flash(SA.MissingTables); Go("SupportAddresses.aspx"); return; }
            foreach (int id in ids)
            {
                try
                {
                    if (op == "bulk_restore") { if (RestoreOneCore(c, id, false)) ok++; else errors.Add("#" + id + " not restored"); }
                    else if (op == "bulk_delete") { string err; if (DeleteOneCore(c, id, out err)) ok++; else errors.Add("#" + id + ": " + err); }
                    else if (op == "bulk_reject") { if (RejectOneCore(c, id, "Bulk rejected from Address Verification list.")) ok++; else errors.Add("#" + id + " not rejected"); }
                }
                catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Error("Bulk address op failed " + id, ex); errors.Add("#" + id + ": database error"); }
            }
        }
        string msg = ok + " address" + (ok == 1 ? "" : "es") + " " + (op == "bulk_restore" ? "restored" : op == "bulk_delete" ? "deleted" : "rejected") + ".";
        if (errors.Count > 0) msg += " " + errors.Count + " skipped (" + string.Join("; ", errors.Take(5)) + (errors.Count > 5 ? "…" : "") + ").";
        SA.Flash(msg);
        string tab = op == "bulk_restore" || op == "bulk_delete" ? "rejected" : (op == "bulk_reject" && SA.Get("tab") == "verified" ? "verified" : "pending");
        // Prefer the tab of the page that posted (hidden via filters select name=tab may be present)
        string postedTab = SA.Form("tab"); if (postedTab == "pending" || postedTab == "verified" || postedTab == "rejected") tab = postedTab;
        if (op == "bulk_restore") tab = "pending";
        if (op == "bulk_delete") tab = "rejected";
        Go("SupportAddresses.aspx?tab=" + tab);
    }
    void RestoreOne(int id) { using (var c = SA.Open()) { if (!RestoreOneCore(c, id, true)) SA.Flash("That address could not be restored."); } Go("SupportAddresses.aspx?tab=pending"); }
    bool RestoreOneCore(SqlConnection c, int id, bool flash)
    {
        if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { if (flash) SA.Flash(SA.MissingTables); return false; }
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var cur = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.AddressMasterId,a.Street_Name,a.PostalCode,a.Verifed,r.Status RStatus FROM AddressMaster a WITH (UPDLOCK) LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE a.AddressMasterId=@i", "@i", id);
            if (cur.Rows.Count != 1) { tx.Rollback(); return false; }
            if (SA.S(cur.Rows[0]["RStatus"]) != "Rejected") { tx.Rollback(); return false; }
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Verifed=0 WHERE AddressMasterId=@i", "@i", id);
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Pending',Notes=NULL,ReviewedBy=NULL,ReviewedByName=NULL,ReviewedAtUTC=NULL WHERE AddressMasterId=@i", "@i", id);
            SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Restored", "Rejected", "Pending", null, null, "Restored to Pending Verification");
            tx.Commit();
        }
        if (flash) SA.Flash("Address restored to Pending Verification.");
        return true;
    }
    void DeleteOne(int id) { string err; using (var c = SA.Open()) { if (!DeleteOneCore(c, id, out err)) SA.Flash(err); else SA.Flash("Address deleted."); } Go("SupportAddresses.aspx?tab=" + (SA.Form("tab").Length > 0 ? SA.Form("tab") : "rejected")); }
    bool DeleteOneCore(SqlConnection c, int id, out string err)
    {
        err = "That address could not be deleted.";
        if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { err = SA.MissingTables; return false; }
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var cur = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.AddressMasterId,a.Street_Name," + Names + ",a.PostalCode,a.Verifed,r.Status RStatus,r.SubmittedBy,r.Country FROM AddressMaster a WITH (UPDLOCK)" + NameJoins + " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE a.AddressMasterId=@i", "@i", id);
            if (cur.Rows.Count != 1) { tx.Rollback(); err = "Address not found."; return false; }
            var a = cur.Rows[0];
            string st = SA.S(a["Verifed"]) == "1" ? "Verified" : (SA.S(a["RStatus"]) == "Rejected" ? "Rejected" : "Pending");
            if (st == "Verified") { tx.Rollback(); err = "Verified addresses cannot be deleted here. Reject them first."; return false; }
            string who = Submitter(c, tx, a);
            if (who != null && !who.StartsWith("Added by Support", StringComparison.OrdinalIgnoreCase) && !who.StartsWith("Imported by Support", StringComparison.OrdinalIgnoreCase))
            { tx.Rollback(); err = "Address is linked (" + who + ") and was not deleted."; return false; }
            string before = SA.S(a["Street_Name"]) + ", " + SA.S(a["City"]) + ", " + SA.S(a["Province"]) + ", " + SA.S(a["PostalCode"]);
            AnyminuteTimesheetRepair.Execute(c, tx, "DELETE FROM AnyminuteAddressReview WHERE AddressMasterId=@i", "@i", id);
            AnyminuteTimesheetRepair.Execute(c, tx, "DELETE FROM AddressMaster WHERE AddressMasterId=@i", "@i", id);
            SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Deleted", st, null, before, null, "Deleted from Address Verification");
            tx.Commit();
        }
        err = null; return true;
    }
    bool RejectOneCore(SqlConnection c, int id, string notes)
    {
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var cur = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.AddressMasterId,a.Street_Name,a.PostalCode,a.Verifed,a.City,a.Province,r.Status RStatus,r.SubmittedBy FROM AddressMaster a WITH (UPDLOCK) LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE a.AddressMasterId=@i", "@i", id);
            if (cur.Rows.Count != 1) { tx.Rollback(); return false; }
            var a = cur.Rows[0];
            string st = SA.S(a["Verifed"]) == "1" ? "Verified" : (SA.S(a["RStatus"]) == "Rejected" ? "Rejected" : "Pending");
            if (st == "Rejected") { tx.Rollback(); return false; }
            AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteAddressReview WHERE AddressMasterId=@i)
                INSERT AnyminuteAddressReview(AddressMasterId,OrigStreet,OrigCity,OrigProvince,OrigCountry,OrigPostal,Status) VALUES(@i,@s,@c,@p,@co,@pc,'Pending')",
                "@i", id, "@s", SA.S(a["Street_Name"]), "@c", SA.S(a["City"]), "@p", SA.S(a["Province"]), "@co", SA.GuessCountry(SA.S(a["PostalCode"])), "@pc", SA.S(a["PostalCode"]));
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Verifed=0 WHERE AddressMasterId=@i", "@i", id);
            AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Rejected',Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
                "@n", notes, "@b", SA.ActorId(), "@bn", SA.ActorName(), "@i", id);
            SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Rejected", st, "Rejected", null, null, notes);
            tx.Commit();
        }
        return true;
    }
        // ---- v71 (t206u/t207ua0): Verified Addresses > Add Address / Import Address (AnyminuteAddressImport) ----
    const string Back71 = "SupportAddresses.aspx?tab=verified";
    static string Inp(string label, string name, string val, string extra) { return "<label class='sa-field'><span>" + label + "</span><input name='" + name + "' value='" + SA.A(val) + "'" + extra + " /></label>"; }
    string[] kept;
    string V(int i) { if (kept == null) { kept = Session["sa_av_vals"] as string[] ?? new string[5]; Session.Remove("sa_av_vals"); } return kept[i] ?? ""; }
    string AddModal(bool ready)
    {
        string dis = ready ? "" : " disabled";
        var sb = new StringBuilder("<div class='sa-modal sa-modal45' data-sa-modal='1'><div class='sa-modal-box sa-av71' role='dialog' aria-modal='true' aria-labelledby='sa-av-h'><a class='sa-x sa-x-abs' href='" + Back71 + "' aria-label='Close'>&times;</a>")
            .Append("<h2 id='sa-av-h'>Add Address</h2>" + TakeModalMessage() + "<p class='sa-muted'>The address is added to the Verified Address Database.</p>")
            .Append(Inp("Street Name (Address) *", "av_street", V(0), " maxlength='200' required" + dis))
            .Append("<div class='sa-row2'>").Append(Inp("City *", "av_city", V(1), " maxlength='200' required" + dis))
            .Append(Inp("Prov/State *", "av_prov", V(2), " maxlength='200' list='sa-av-prov' required" + dis)).Append("</div>")
            .Append("<div class='sa-row2'><label class='sa-field'><span>Country *</span><select name='av_country' required").Append(dis).Append(">").Append(SA.Options(SA.Countries, V(3).Length > 0 ? V(3) : "Canada", null)).Append("</select></label>")
            .Append(Inp("Postal Code/PIN *", "av_postal", V(4), " maxlength='20' required" + dis)).Append("</div>")
            .Append("<datalist id='sa-av-prov'>");
        foreach (var p in AnyminuteSupportAddress.Canada.Concat(AnyminuteSupportAddress.States)) sb.Append("<option value='").Append(SA.A(p)).Append("'></option>");
        return sb.Append("</datalist><div class='sa-panel-foot sa-one-line'><button type='submit' class='sa-btn' name='sa_op' value='addaddr'").Append(dis).Append(">&#10003; Save Address</button><a class='sa-btn sa-btn-outline' href='").Append(Back71).Append("'>Cancel</a></div></div></div>").ToString();
    }
    string ImportModal(bool ready)
    {
        string dis = ready ? "" : " disabled";
        return "<div class='sa-modal sa-modal45' data-sa-modal='1'><div class='sa-modal-box sa-av71' role='dialog' aria-modal='true' aria-labelledby='sa-ai-h'><a class='sa-x sa-x-abs' href='" + Back71 + "' aria-label='Close'>&times;</a>"
            + "<h2 id='sa-ai-h'>Import Address</h2>" + TakeModalMessage() + "<p class='sa-muted'>Upload a CSV or Excel (.xlsx) file with the columns <b>Address, City, Prov/State, Country, Postal Code/PIN</b> (first row = header; all worksheets are read). "
            + "Valid rows are added to the Verified Address Database; blank rows and addresses already verified (same address, city and postal code) are skipped. Max 2 MB / " + AnyminuteAddressImport.MaxRows.ToString("#,0", CultureInfo.InvariantCulture) + " rows.</p>"
            + "<p><a class='sa-link' href='SupportAddresses.aspx?template=1'>&#8595; Download template (CSV)</a></p>"
            + "<label class='sa-field'><span>File *</span><input type='file' name='av_file' accept='.csv,.xlsx,text/csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' required" + dis + " /></label>"
            + "<div class='sa-panel-foot sa-one-line'><button type='submit' class='sa-btn' name='sa_op' value='importaddr'" + dis + ">&#8593; Import</button><a class='sa-btn sa-btn-outline' href='" + Back71 + "'>Cancel</a></div></div></div>";
    }
    void Template()
    {
        Response.Clear(); Response.ContentType = "text/csv"; Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition", "attachment; filename=\"AnyMinute_Address_Import_Template.csv\"");
        Response.Write("\uFEFF" + String.Join(",", AnyminuteAddressImport.Header) + "\r\n");
        Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest();
    }
    void Go(string url) { Response.Redirect(url, false); Context.ApplicationInstance.CompleteRequest(); }
    string TakeModalMessage() { string m = Session["sa_av_message"] as string; Session.Remove("sa_av_message"); return String.IsNullOrEmpty(m) ? "" : "<div class='sa-flash sa-warn' role='alert'>" + SA.H(m) + "</div>"; }
    void ModalError(string message, string modal) { Session["sa_av_message"] = message; Go(Back71 + "&" + modal + "=1"); }
    void AddAddress()
    {
        string street = SA.Form("av_street"), city = SA.Form("av_city"), prov = SA.Form("av_prov"), country = SA.Form("av_country"), postal = SA.Form("av_postal");
        Session["sa_av_vals"] = new[] { street, city, prov, country, postal };
        try
        {
            using (var c = SA.Open())
            {
                if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { ModalError(SA.MissingTables, "add"); return; }
                using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
                {
                    int id; string err = AnyminuteAddressImport.AddVerified(c, tx, street, city, prov, country, postal, "Added by Support (" + SA.ActorName() + ")", "Added by Support Admin", out id);
                    if (err != null) { tx.Rollback(); ModalError(err == "duplicate" ? "This address already exists (same street, city, province/state, country and postal code)." : "Address not added: " + err, "add"); return; }
                    string after = street + ", " + city + ", " + prov + ", " + AnyminuteAddressImport.Country(country) + ", " + postal;
                    SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Added (Verified)", null, "Verified", null, after, "Added by Support Admin");
                    tx.Commit();
                }
            }
        }
        catch (SqlException ex) { log4net.LogManager.GetLogger("Log").Error("Add address failed", ex); ModalError("Address not added: database error. Nothing was saved; please retry.", "add"); return; }
        Session.Remove("sa_av_vals"); Session.Remove("sa_av_message");
        SA.Flash("Address added to the Verified Address Database.");
        Go(Back71);
    }
    void ImportAddress()
    {
        var f = Request.Files["av_file"];
        if (f == null || f.ContentLength == 0) { ModalError("Choose a CSV or .xlsx file to import.", "import"); return; }
        if (f.ContentLength > AnyminuteAddressImport.MaxBytes) { ModalError("The file is larger than 2 MB. Nothing was imported.", "import"); return; }
        var data = new byte[f.ContentLength]; int read = 0;
        while (read < data.Length) { int n = f.InputStream.Read(data, read, data.Length - read); if (n <= 0) break; read += n; }
        if (read != data.Length) { ModalError("The upload was incomplete. Nothing was imported.", "import"); return; }
        List<string[]> rows; string sheets;
        try { rows = AnyminuteAddressImport.Read(f.FileName, data, out sheets); }
        catch (System.IO.InvalidDataException ex) { ModalError(ex.Message, "import"); return; }
        catch (System.Xml.XmlException) { ModalError("The .xlsx file could not be read. Nothing was imported.", "import"); return; }
        catch (IndexOutOfRangeException) { ModalError("The .xlsx file could not be read. Nothing was imported.", "import"); return; }
        catch (ArgumentException) { ModalError("The .xlsx file could not be read. Nothing was imported.", "import"); return; }
        if (rows.Count == 0) { ModalError("The file has no data rows.", "import"); return; }
        int added = 0, verified = 0, fileDups = 0, blanks = 0, failed = 0;
        bool rolledBack = false;
        var errors = new List<string>(); var reasons = new Dictionary<string, int>(); var seen = new HashSet<string>();
        Func<string[], int, string> cell = (r, i) => i >= 0 && i < r.Length ? (r[i] ?? "").Trim() : "";
        try
        {
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { ModalError(SA.MissingTables, "import"); return; }
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    string by = "Imported by Support (" + SA.ActorName() + ")", note = "Imported from " + System.IO.Path.GetFileName(f.FileName ?? "");
                    for (int i = 0; i < rows.Count; i++)
                    {
                        var r = rows[i]; string street = cell(r, 0), city = cell(r, 1), prov = cell(r, 2), country = cell(r, 3), postal = cell(r, 4);
                        if ((street + city + prov + country + postal).Length == 0) { blanks++; continue; }
                        string key = SA.CompactPostal(postal) + "|" + System.Text.RegularExpressions.Regex.Replace(street, @"\s+", " ").ToUpperInvariant() + "|" + System.Text.RegularExpressions.Regex.Replace(city, @"\s+", " ").ToUpperInvariant();
                        if (!seen.Add(key)) { fileDups++; continue; }
                        int id; string err = AnyminuteAddressImport.AddVerified(c, tx, street, city, prov, country, postal, by, note, out id);
                        if (err == null) added++;
                        else if (err == "duplicate") verified++;
                        else { errors.Add("Data row " + (i + 1) + ": " + err); reasons[err] = (reasons.ContainsKey(err) ? reasons[err] : 0) + 1; }
                    }
                    SA.Audit(c, tx, "Address", "import", "Imported (Verified)", null, "Verified", null,
                        rows.Count + " read, " + added + " added, " + verified + " already verified, " + fileDups + " within-file duplicates, " + blanks + " blank, " + errors.Count + " invalid, 0 failed", note);
                    tx.Commit();
                }
                catch (SqlException ex)
                {
                    try { tx.Rollback(); } catch (InvalidOperationException) { }
                    log4net.LogManager.GetLogger("Log").Error("Address import failed", ex);
                    rolledBack = true; added = 0; failed = rows.Count - verified - fileDups - blanks - errors.Count;
                }
            }
        }
        }
        catch (SqlException ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Address import connection/transaction failed", ex);
            rolledBack = true; added = 0; failed = rows.Count - verified - fileDups - blanks - errors.Count;
        }
        var sb = new StringBuilder("<div class='sa-flash' role='status'><strong>" + (rolledBack ? "Import rolled back: nothing added." : "Import complete.") + "</strong>");
        sb.Append("<p>Source: ").Append(SA.H(sheets)).Append(" (data rows, excluding headers; Excel row-number gaps are not records).</p><ul>")
            .Append("<li>Total rows read: ").Append(rows.Count).Append("</li><li>Added: ").Append(added)
            .Append("</li><li>Skipped — already verified: ").Append(verified).Append("</li><li>Skipped — duplicates within file: ").Append(fileDups)
            .Append("</li><li>Skipped — blank/invalid: ").Append(blanks + errors.Count).Append(" (blank: ").Append(blanks).Append(", invalid: ").Append(errors.Count)
            .Append(")</li><li>Failed: ").Append(failed).Append(rolledBack ? " (database failure: includes rolled-back additions and unprocessed rows)" : "").Append("</li></ul>");
        if (reasons.Count > 0) { sb.Append("<p>Invalid reason counts:</p><ul>"); foreach (var reason in reasons) sb.Append("<li>").Append(SA.H(reason.Key)).Append(": ").Append(reason.Value).Append("</li>"); sb.Append("</ul>"); }
        if (errors.Count > 0) { sb.Append("<ul class='sa-av-errs'>"); foreach (var e in errors.Take(50)) sb.Append("<li>").Append(SA.H(e)).Append("</li>"); if (errors.Count > 50) sb.Append("<li>... and ").Append(errors.Count - 50).Append(" more.</li>"); sb.Append("</ul>"); }
        Session["sa_av_import"] = sb.Append("</div>").ToString();
        Go(Back71);
    }
    string TakeImportSummary() { string s = Convert.ToString(Session["sa_av_import"]); Session.Remove("sa_av_import"); return s ?? ""; }
    void Back(int id) { Response.Redirect("SupportAddresses.aspx" + (id > 0 ? "?id=" + id : ""), false); Context.ApplicationInstance.CompleteRequest(); }
}
