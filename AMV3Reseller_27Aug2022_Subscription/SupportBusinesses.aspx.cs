using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-11-01 v46 (t148u-t150u): Subscribers table pattern (10 per page, kebab-only Actions, per-page selector + pager).
// 2026-10-29 v30 (Shamsu deck 2 Support nav: Dashboard, Resellers, Subscribers, Businesses, ...): read-only list of every business
// with its owning subscriber (AssginBussiness UserTypeId 1), users and status. Manage links go to the subscriber's Businesses tab.
public partial class AnyminuteSupportBusinessesPage : System.Web.UI.Page
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    int PageSize = 10; // v46 (t148u): 10 rows per page by default, 10 / 20 / 50 selector
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        string q = SA.Get("q"), st = SA.Get("status"); int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1; PageSize = SA.PageSizeArg(10);
        var sb = new StringBuilder("<header class='sa-head'><div><h1>Businesses</h1><p>All businesses under every subscriber (read-only). Open the owner to see employees and projects.</p></div></header>");
        List<DataRow> all;
        try
        {
            using (var c = SA.Open())
                all = SA.Q(c, @"SELECT b.BusinessID,b.CompanyName,COALESCE(ci.CityName,CAST(b.City AS nvarchar(200))) City,COALESCE(pr.ProvinceName,CAST(b.Province AS nvarchar(200))) Province,b.Status,b.BusinessAgreementDate,o.UserId,o.Email,o.Name,
                    (SELECT COUNT(DISTINCT a2.EmployeeID) FROM AssginBussiness a2 WHERE a2.BusinessID=b.BusinessID) Users
                    FROM AddBusiness b LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(b.City AS nvarchar(50)) LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=CAST(b.Province AS nvarchar(50))
                    OUTER APPLY (SELECT TOP 1 u.UserId,u.Email,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name FROM AssginBussiness a JOIN UserAcc u ON u.EmployeeId=a.EmployeeID
                        LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId WHERE a.BusinessID=b.BusinessID AND a.UserTypeId=1 ORDER BY u.UserId) o
                    ORDER BY b.CompanyName,b.BusinessID").Rows.Cast<DataRow>().ToList();
        }
        catch (SqlException ex)
        {
            log4net.LogManager.GetLogger("Log").Warn("SupportBusinesses load failed", ex);
            Body1.Text = sb.Append("<div class='sa-flash sa-warn'>Businesses could not be loaded from the database.</div>").ToString(); return;
        }
        Func<DataRow, string> stat = r => SA.S(r["Status"]) == "2" ? "Inactive" : "Active";
        IEnumerable<DataRow> rows = all;
        if (q.Length > 0) rows = rows.Where(r => new[] { "CompanyName", "City", "Province", "Email", "Name" }.Any(k => SA.S(r[k]).IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
        if (st.Length > 0) rows = rows.Where(r => stat(r) == st);
        var list = rows.ToList();
        sb.Append("<div class='sa-kpis'><div class='sa-kpi'><i class='sa-kpi-icon sa-blue' aria-hidden='true'>&#127970;</i><div><span>Total Businesses</span><strong>").Append(all.Count).Append("</strong></div></div>")
          .Append("<div class='sa-kpi'><i class='sa-kpi-icon sa-green' aria-hidden='true'>&#10004;</i><div><span>Active</span><strong>").Append(all.Count(r => stat(r) == "Active")).Append("</strong></div></div>")
          .Append("<div class='sa-kpi'><i class='sa-kpi-icon sa-red' aria-hidden='true'>&#10006;</i><div><span>Inactive</span><strong>").Append(all.Count(r => stat(r) == "Inactive")).Append("</strong></div></div>")
          .Append("<div class='sa-kpi'><i class='sa-kpi-icon sa-purple' aria-hidden='true'>&#128100;</i><div><span>Subscribers</span><strong>").Append(all.Where(r => r["UserId"] != DBNull.Value).Select(r => SA.S(r["UserId"])).Distinct().Count()).Append("</strong></div></div></div>");
        sb.Append("<div class='sa-card sa-rsl-bar'><div class='sa-filters' data-sa-get='SupportBusinesses.aspx'><input type='hidden' name='size' value='").Append(PageSize).Append("' /><label class='sa-grow sa-search'><span class='sr-only'>Search</span><i aria-hidden='true'>&#128269;</i><input type='search' name='q' value='").Append(SA.A(q))
          .Append("' placeholder='Search by business, city or subscriber...' /></label><label><span>Status</span><select name='status'>").Append(SA.Options(new[] { "Active", "Inactive" }, st, "All")).Append("</select></label><a class='sa-btn sa-btn-outline' href='SupportBusinesses.aspx'>Clear Filters</a></div></div>");
        // v46 (t148u/t150u): same table pattern as Subscribers / Resellers - Sl. No., name link, status pill, kebab-only Actions
        // (full menu shown as a fixed pop-over), "Showing x to y of z" + per-page selector + pager.
        int pages = Math.Max(1, (list.Count + PageSize - 1) / PageSize); if (page > pages) page = pages;
        sb.Append("<div class='sa-card'><div class='sa-rpt-cardhead'><h2>Business List</h2></div><div class='sa-table-wrap'><table class='sa-table sa-list46'><thead><tr><th>Sl. No.</th><th>Business Name</th><th>Subscriber (Owner)</th><th>City</th><th>Province / State</th><th class='sa-c'>Users</th><th>Agreement Date</th><th>Status</th><th class='sa-c'>Actions</th></tr></thead><tbody>");
        if (list.Count == 0) sb.Append("<tr><td colspan='9' class='sa-rpt-empty'>").Append(all.Count == 0 ? "No businesses yet." : "No businesses match these filters.").Append("</td></tr>");
        int n = (page - 1) * PageSize;
        Func<string, string, string, string, string> mi = (href, ico, t, sm) => "<a class='sa-menu-item' href='" + href + "'><i aria-hidden='true'>" + ico + "</i><span><b>" + t + "</b><small>" + sm + "</small></span></a>";
        foreach (var r in list.Skip((page - 1) * PageSize).Take(PageSize))
        {
            string s = stat(r), owner = AnyminuteDisplayText.Tidy(SA.S(r["Name"])); if (owner.Length == 0) owner = SA.S(r["Email"]);
            string u = SA.S(r["UserId"]), sub = "SupportSubscribers.aspx?user=" + u, bid = SA.S(r["BusinessID"]);
            sb.Append("<tr><td>").Append(++n).Append("</td><td class='sa-wrap'>").Append(u.Length > 0 ? "<a class='sa-link' href='" + sub + "&amp;view=businesses'>" + SA.H(r["CompanyName"]) + "</a>" : "<strong>" + SA.H(r["CompanyName"]) + "</strong>")
              .Append("<small class='sa-muted'>BIZ-").Append(bid).Append("</small></td><td class='sa-wrap'>").Append(owner.Length == 0 ? "&mdash;" : SA.H(owner))
              .Append(SA.S(r["Email"]).Length > 0 ? "<small class='sa-muted'>" + SA.H(r["Email"]) + "</small>" : "").Append("</td><td>").Append(SA.H(r["City"])).Append("</td><td>").Append(SA.H(r["Province"])).Append("</td><td class='sa-c'>").Append(r["Users"])
              .Append("</td><td class='sa-nowrap'>").Append(r["BusinessAgreementDate"] == DBNull.Value ? "&mdash;" : Convert.ToDateTime(r["BusinessAgreementDate"], Inv).ToString("MMM dd, yyyy", Inv)).Append("</td><td>").Append(SA.Pill(s, s == "Active" ? "green" : "red")).Append("</td><td class='sa-c'>");
            if (u.Length > 0)
                sb.Append("<details class='sa-menu'><summary class='sa-kebab' aria-label='Business actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Business Actions</h4><small>").Append(SA.H(r["CompanyName"])).Append(" &middot; ").Append(s).Append("</small>")
                  .Append(mi(sub + "&amp;view=businesses", "&#128065;", "View Business Details", "Business, employees and projects under the owner"))
                  .Append(mi(sub + "&amp;view=bizedit&amp;biz=" + bid, "&#9998;", "Edit Business", "Update business information and address"))
                  .Append(mi(sub, "&#128100;", "View Subscriber", "Open the owning subscriber"))
                  .Append("</div></details>");
            else sb.Append("&mdash;");
            sb.Append("</td></tr>");
        }
        sb.Append("</tbody></table></div>").Append(SA.Pager46(list.Count, page, PageSize, "SupportBusinesses.aspx?q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(st), "business")).Append("</div>");
        Body1.Text = sb.ToString();
    }
}
