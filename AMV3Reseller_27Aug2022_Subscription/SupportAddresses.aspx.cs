using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-28 v25 deck 1 (Address Verification): Pending / Verified / Rejected tabs over the existing AddressMaster
// lookup table (Verifed=1 is the verified address database). Edit & Verify keeps the original submitted address in
// AnyminuteAddressReview (REQUIRED_07) and records corrected address, admin, date/time and reason in AnyminuteSupportAudit.
// Verify Address (format/province check) is separate from Approve & Add to Database, which re-verifies on the server.
public partial class AnyminuteSupportAddressesPage : System.Web.UI.Page
{
    const int PageSize = 8;
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(); return; }
        string tab = SA.Get("tab"); if (tab != "verified" && tab != "rejected") tab = "pending";
        string q = SA.Get("q"), country = SA.Get("country"), prov = SA.Get("prov"), date = SA.Get("date");
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        int id; Int32.TryParse(SA.Get("id"), out id);
        Flash.Text = SA.TakeFlash();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteAddressReview") && SA.Has(c, "AnyminuteSupportAudit");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteAddressReview", "AnyminuteSupportAudit");
            string rj = ready ? " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId" : "";
            string status = ready ? "CASE WHEN a.Verifed=1 THEN 'Verified' WHEN r.Status='Rejected' THEN 'Rejected' ELSE 'Pending' END" : "CASE WHEN a.Verifed=1 THEN 'Verified' ELSE 'Pending' END";
            string cols = ready ? ",r.SubmittedBy,r.SubmittedAtUTC,r.Country,r.ReviewedAtUTC,r.ReviewedByName,r.Notes" : ",CAST(NULL AS nvarchar(150)) SubmittedBy,CAST(NULL AS datetime2) SubmittedAtUTC,CAST(NULL AS nvarchar(60)) Country,CAST(NULL AS datetime2) ReviewedAtUTC,CAST(NULL AS nvarchar(150)) ReviewedByName,CAST(NULL AS nvarchar(500)) Notes";
            var all = SA.Q(c, "SELECT a.AddressMasterId,a.Street_Name,a.City,a.Province,a.PostalCode," + status + " St" + cols + " FROM AddressMaster a" + rj);
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
            Filters.Text = "<div class='sa-filters' data-sa-get='SupportAddresses.aspx'>"
                + "<label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='" + SA.A(q) + "' placeholder='Search address, city, or subscriber...' /></label>"
                + "<label><span>Country</span><select name='country'>" + SA.Options(SA.Countries, country, "All") + "</select></label>"
                + "<label><span>Province/State</span><select name='prov'>" + SA.Options(provinces, prov, "All") + "</select></label>"
                + "<label><span>Status</span><select name='tab'>" + "<option value='pending'" + (tab == "pending" ? " selected" : "") + ">Pending Verification</option><option value='verified'" + (tab == "verified" ? " selected" : "") + ">Verified</option><option value='rejected'" + (tab == "rejected" ? " selected" : "") + ">Rejected</option></select></label>"
                + "<label><span>Submitted Date</span><select name='date'>" + "<option value=''>All Dates</option><option value='7'" + (date == "7" ? " selected" : "") + ">Last 7 days</option><option value='30'" + (date == "30" ? " selected" : "") + ">Last 30 days</option></select></label>"
                + "<button type='button' class='sa-btn' data-sa-apply='1'>Search</button></div>";

            string baseQ = "tab=" + tab + "&q=" + HttpUtility.UrlEncode(q) + "&country=" + HttpUtility.UrlEncode(country) + "&prov=" + HttpUtility.UrlEncode(prov) + "&date=" + date;
            var sb = new StringBuilder("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th><input type='checkbox' aria-label='Select all' data-sa-all='1' /></th><th>Address (Submitted)</th><th>City</th><th>Prov/State</th><th>Country</th><th>Postal Code</th><th>Submitted By</th><th>Date</th><th>Actions</th></tr></thead><tbody>");
            foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
            {
                int rid = Convert.ToInt32(r["AddressMasterId"]); string ctry = SA.S(r["Country"]); if (ctry.Length == 0) ctry = SA.GuessCountry(SA.S(r["PostalCode"]));
                sb.Append("<tr").Append(rid == id ? " class='sa-selected'" : "").Append("><td><input type='checkbox' aria-label='Select' /></td><td>").Append(SA.H(r["Street_Name"])).Append("</td><td>").Append(SA.H(r["City"]))
                  .Append("</td><td>").Append(SA.H(r["Province"])).Append("</td><td>").Append(SA.H(ctry)).Append("</td><td>").Append(SA.H(r["PostalCode"])).Append("</td><td>")
                  .Append(SA.S(r["SubmittedBy"]).Length > 0 ? SA.H(r["SubmittedBy"]) : "<span class='sa-muted'>Manual entry</span>").Append("</td><td>").Append(SA.Day(r["SubmittedAtUTC"])).Append("</td><td class='sa-actions'>")
                  .Append("<a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportAddresses.aspx?").Append(SA.A(baseQ)).Append("&amp;page=").Append(page).Append("&amp;id=").Append(rid).Append("'>").Append(tab == "pending" ? "Edit" : "View").Append("</a>");
                if (tab == "pending") sb.Append(" <a class='sa-btn sa-btn-sm sa-btn-danger' href='SupportAddresses.aspx?").Append(SA.A(baseQ)).Append("&amp;page=").Append(page).Append("&amp;id=").Append(rid).Append("#sa-reject'>Reject</a>");
                sb.Append("</td></tr>");
            }
            if (rows.Count == 0) sb.Append("<tr><td colspan='9' class='sa-empty'>No addresses in this list.</td></tr>");
            Table.Text = sb.Append("</tbody></table></div>").ToString() + SA.Pager(rows.Count, page, PageSize, baseQ);

            var pick = id > 0 ? all.Rows.Cast<DataRow>().FirstOrDefault(r => Convert.ToInt32(r["AddressMasterId"]) == id) : null;
            Panel.Text = pick == null ? "" : EditPanel(c, pick, ready, baseQ + "&page=" + page);
        }
    }
    string EditPanel(System.Data.SqlClient.SqlConnection c, DataRow r, bool ready, string back)
    {
        int id = Convert.ToInt32(r["AddressMasterId"]);
        DataRow review = null;
        if (ready) { var t = SA.Q(c, "SELECT * FROM AnyminuteAddressReview WHERE AddressMasterId=@i", "@i", id); if (t.Rows.Count == 1) review = t.Rows[0]; }
        string os = review != null ? SA.S(review["OrigStreet"]) : SA.S(r["Street_Name"]), oc = review != null ? SA.S(review["OrigCity"]) : SA.S(r["City"]),
               op = review != null ? SA.S(review["OrigProvince"]) : SA.S(r["Province"]), opc = review != null ? SA.S(review["OrigPostal"]) : SA.S(r["PostalCode"]);
        string octry = review != null && SA.S(review["OrigCountry"]).Length > 0 ? SA.S(review["OrigCountry"]) : SA.GuessCountry(opc);
        string ctry = SA.S(r["Country"]).Length > 0 ? SA.S(r["Country"]) : SA.GuessCountry(SA.S(r["PostalCode"]));
        bool pending = SA.S(r["St"]) == "Pending";
        string dis = pending && ready ? "" : " disabled";
        var sb = new StringBuilder("<aside class='sa-card sa-panel' id='sa-edit' aria-label='Edit and verify address'><div class='sa-panel-head'><h2>Edit &amp; Verify Address</h2><a href='SupportAddresses.aspx?" + SA.A(back) + "' class='sa-x' aria-label='Close'>&times;</a></div>");
        sb.Append("<h3>Original Submitted Address</h3><div class='sa-original'><p>").Append(SA.H(os + ", " + oc + ", " + op + ", " + octry + ", " + opc)).Append("</p><dl><dt>Submitted By:</dt><dd>")
          .Append(review != null && SA.S(review["SubmittedBy"]).Length > 0 ? SA.H(review["SubmittedBy"]) : "Manual entry").Append("</dd><dt>Submitted Date:</dt><dd>")
          .Append(review != null && review["SubmittedAtUTC"] != DBNull.Value ? SA.When(review["SubmittedAtUTC"]).Replace("<br />", " ") : "&mdash;").Append("</dd></dl></div>");
        sb.Append("<div data-sa-address='1'><input type='hidden' name='sa_id' value='").Append(id).Append("' /><h3>Corrected Address (Edit if needed)</h3>")
          .Append("<label class='sa-field'><span>Street Address *</span><input name='sa_street' maxlength='200' value='").Append(SA.A(r["Street_Name"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<div class='sa-row2'><label class='sa-field'><span>City *</span><input name='sa_city' maxlength='200' value='").Append(SA.A(r["City"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<label class='sa-field'><span>Province / State *</span><input name='sa_prov' maxlength='200' list='sa-prov-list' value='").Append(SA.A(r["Province"])).Append("'").Append(dis).Append(" /></label></div>")
          .Append("<label class='sa-field'><span>Country *</span><select name='sa_country'").Append(dis).Append(">").Append(SA.Options(SA.Countries, ctry, null)).Append("</select></label>")
          .Append("<div class='sa-row2 sa-verify-row'><label class='sa-field'><span>Postal / ZIP / PIN Code *</span><input name='sa_postal' maxlength='20' value='").Append(SA.A(r["PostalCode"])).Append("'").Append(dis).Append(" /></label>")
          .Append("<button type='button' class='sa-btn sa-btn-outline' data-sa-verify='1'").Append(dis).Append(">&#9679; Verify Address</button></div>")
          .Append("<p class='sa-verify-result' data-sa-verify-result='1' role='status'></p>")
          .Append("<label class='sa-field'><span>Support Notes (correction reason)</span><textarea name='sa_notes' maxlength='500' rows='3' data-sa-counter='500'").Append(dis).Append(">").Append(review != null ? SA.H(review["Notes"]) : "").Append("</textarea><small data-sa-count='1'>0/500</small></label>");
        if (pending)
            sb.Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportAddresses.aspx?").Append(SA.A(back)).Append("'>Cancel</a>")
              .Append("<button type='submit' id='sa-reject' class='sa-btn sa-btn-danger' name='sa_op' value='reject' data-sa-need-notes='Enter the reason for rejecting this address in Support Notes.'").Append(dis).Append(">&#10005; Reject</button>")
              .Append("<button type='submit' class='sa-btn sa-btn-success' name='sa_op' value='approve' data-sa-approve='1' disabled>&#10003; Approve &amp; Add to Database</button></div><p class='sa-muted sa-small'>Verify the address first; Approve &amp; Add to Database stays off until the current values pass Verify Address.</p>");
        else if (review != null)
            sb.Append("<p class='sa-muted'>").Append(SA.H(SA.S(r["St"]) + " by " + SA.S(review["ReviewedByName"]))).Append(" &middot; ").Append(SA.When(review["ReviewedAtUTC"]).Replace("<br />", " ")).Append("</p>");
        sb.Append("</div><datalist id='sa-prov-list'>");
        foreach (var p in new[] { "ON", "QC", "BC", "AB", "MB", "SK", "NS", "NB", "NL", "PE", "NT", "NU", "YT", "NY", "CA", "TX", "FL", "IL", "WA", "MA", "NJ", "England", "Scotland", "Wales" }) sb.Append("<option value='").Append(p).Append("'></option>");
        sb.Append("</datalist><h3>Activity History</h3>").Append(SA.HistoryTable(ready ? SA.History(c, "Address", id.ToString(CultureInfo.InvariantCulture)) : null, "No review actions yet."));
        return sb.Append("</aside>").ToString();
    }
    void Act()
    {
        string op = SA.Form("sa_op"); int id; Int32.TryParse(SA.Form("sa_id"), out id);
        string street = SA.Form("sa_street"), city = SA.Form("sa_city"), prov = SA.Form("sa_prov"), country = SA.Form("sa_country"), postal = SA.Form("sa_postal"), notes = SA.Form("sa_notes");
        if (notes.Length > 500) notes = notes.Substring(0, 500);
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteAddressReview") || !SA.Has(c, "AnyminuteSupportAudit")) { SA.Flash(SA.MissingTables); Back(id); return; }
            using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
            {
                var cur = AnyminuteTimesheetRepair.Query(c, tx, "SELECT a.*,r.Status RStatus FROM AddressMaster a WITH (UPDLOCK) LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId WHERE a.AddressMasterId=@i", "@i", id);
                if (cur.Rows.Count != 1) { SA.Flash("That address was not found."); tx.Rollback(); Back(0); return; }
                var a = cur.Rows[0];
                if (SA.S(a["Verifed"]) == "1" || SA.S(a["RStatus"]) == "Rejected") { SA.Flash("This address was already reviewed by another Support Admin."); tx.Rollback(); Back(id); return; }
                string before = SA.S(a["Street_Name"]) + ", " + SA.S(a["City"]) + ", " + SA.S(a["Province"]) + ", " + SA.S(a["PostalCode"]);
                // The original submitted address is captured once and never overwritten (audit requirement).
                AnyminuteTimesheetRepair.Execute(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteAddressReview WHERE AddressMasterId=@i)
                    INSERT AnyminuteAddressReview(AddressMasterId,OrigStreet,OrigCity,OrigProvince,OrigCountry,OrigPostal,Status) VALUES(@i,@s,@c,@p,@co,@pc,'Pending')",
                    "@i", id, "@s", SA.S(a["Street_Name"]), "@c", SA.S(a["City"]), "@p", SA.S(a["Province"]), "@co", SA.GuessCountry(SA.S(a["PostalCode"])), "@pc", SA.S(a["PostalCode"]));
                if (op == "approve")
                {
                    string problem = SA.VerifyAddress(street, city, prov, country, postal);
                    if (problem != null) { tx.Rollback(); SA.Flash("Not approved: " + problem); Back(id); return; }
                    postal = SA.NormalizePostal(country, postal);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AddressMaster SET Street_Name=@s,City=@c,Province=@p,PostalCode=@pc,Verifed=1 WHERE AddressMasterId=@i", "@s", street, "@c", city, "@p", prov, "@pc", postal, "@i", id);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Verified',Country=@co,Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
                        "@co", country, "@n", notes, "@b", SA.ActorId(), "@bn", SA.ActorName(), "@i", id);
                    string after = street + ", " + city + ", " + prov + ", " + country + ", " + postal;
                    SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), before == after ? "Approved" : "Corrected & Approved", "Pending", "Verified", before, after, notes);
                    tx.Commit(); SA.Flash("Address verified and added to the Verified Address Database.");
                }
                else if (op == "reject")
                {
                    if (notes.Length == 0) { tx.Rollback(); SA.Flash("Enter the reason for rejecting this address in Support Notes."); Back(id); return; }
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteAddressReview SET Status='Rejected',Notes=@n,ReviewedBy=@b,ReviewedByName=@bn,ReviewedAtUTC=SYSUTCDATETIME() WHERE AddressMasterId=@i",
                        "@n", notes, "@b", SA.ActorId(), "@bn", SA.ActorName(), "@i", id);
                    SA.Audit(c, tx, "Address", id.ToString(CultureInfo.InvariantCulture), "Rejected", "Pending", "Rejected", before, null, notes);
                    tx.Commit(); SA.Flash("Address rejected and returned for correction.");
                }
                else tx.Rollback();
            }
        }
        Back(0);
    }
    void Back(int id) { Response.Redirect("SupportAddresses.aspx" + (id > 0 ? "?id=" + id : ""), false); Context.ApplicationInstance.CompleteRequest(); }
}
