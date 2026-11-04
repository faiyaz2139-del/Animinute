using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-11-03 v55 (Reseller PPT collage, 9 screens): the Reseller login portal. A reseller signs in with the UserAcc login that
// AnyminuteResellers.EnsureLogin created for AnyminuteReseller.LoginEmail. Current() identifies that login (Active reseller, any
// permissions); every reseller screen is scoped to AnyminuteResellerSubscriber rows of that ResellerID and gated by Permissions CSV.
public sealed class RslCtx
{
    public int ResellerId; public string Code = "", Name = ""; public int? MaxSubscribers;
    public HashSet<string> Perms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public bool Can(string p) { return Perms.Contains(p); }
}
public static class AnyminuteResellerPortal
{
    // Subscribers (UserAcc.UserId) owned by reseller @rsl.
    public const string Scope = "(SELECT rs.UserId FROM AnyminuteResellerSubscriber rs WHERE rs.ResellerID=@rsl)";
    // Signed-in pure reseller, or null (Support Admin / SuperAdmin, any other login, REQUIRED_09 not run). Cached 2 minutes per session.
    public static RslCtx Current()
    {
        var h = HttpContext.Current; if (h == null || h.Session == null || h.Session["UserId"] == null) return null;
        if (SA.IsSupport()) return null;
        var cached = h.Session["AmRslPortal"] as Tuple<DateTime, string, RslCtx>;
        string uid = Convert.ToString(h.Session["UserId"]);
        if (cached != null && cached.Item1 > DateTime.UtcNow && cached.Item2 == uid) return cached.Item3;
        RslCtx s = null; int user; Int32.TryParse(uid, out user);
        string email = Convert.ToString(h.Session["email"]).Trim();
        if (user > 0 && email.Length > 0)
        {
            try
            {
                using (var c = SA.Open())
                    if (RS.Ready(c) && RS.LoginUser(c, email) == user)
                    {
                        var t = SA.Q(c, "SELECT TOP 1 ResellerID,ResellerName,Status,Permissions,MaxSubscribers FROM AnyminuteReseller WHERE LTRIM(RTRIM(LoginEmail))=@e", "@e", email);
                        if (t.Rows.Count == 1 && (SA.S(t.Rows[0]["Status"]).Length == 0 || SA.S(t.Rows[0]["Status"]) == "Active"))
                        {
                            var r = t.Rows[0];
                            s = new RslCtx { ResellerId = Convert.ToInt32(r["ResellerID"]), Code = RS.Rsl(r["ResellerID"]), Name = SA.S(r["ResellerName"]), MaxSubscribers = r["MaxSubscribers"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaxSubscribers"]) };
                            foreach (var p in SA.S(r["Permissions"]).Split(',')) if (p.Trim().Length > 0) s.Perms.Add(p.Trim());
                        }
                    }
            }
            catch (SqlException) { s = null; }
            catch (InvalidOperationException) { s = null; }
        }
        h.Session["AmRslPortal"] = Tuple.Create(DateTime.UtcNow.AddMinutes(2), uid, s); return s;
    }
    public static bool Is() { return Current() != null; }
    // v60 (Reseller PPT s9 + Add Plan): AnyminuteSubscriptionPlan.ResellerID exists (DB_OPTIONAL/OPTIONAL_06_ResellerPlans.sql).
    public static bool OwnPlans(SqlConnection c) { return SA.Has(c, "AnyminuteSubscriptionPlan") && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteSubscriptionPlan", "ResellerID"); }
    // Pages under Modern.master: signed-in reseller required (others get 403, not the reseller screens).
    public static RslCtx Require()
    {
        var h = HttpContext.Current;
        if (h.Session == null || h.Session["UserId"] == null) { h.Response.Redirect("Login.aspx", true); return null; }
        var r = Current(); if (r == null) throw new HttpException(403, "Only reseller logins can open this page.");
        return r;
    }
    // PPT sidebar: Dashboard, Subscribers, Businesses, Subscription Plans, Usage & Limits, Reports, Settings (permission-gated).
    // li.am-sa-nav reuses the pinned Support drawer (Modern.master pin script + AnyminuteNavDrawer) so content never sits under it.
    public static string NavHtml(RslCtx r)
    {
        var req = HttpContext.Current.Request; string path = req.Url.AbsolutePath, view = req.QueryString["view"] ?? "";
        string page = System.IO.Path.GetFileName(path);
        // v56 (PPT s1 strict): the full 7-item menu always shows; permission gates stay server-side on each page / action.
        var items = new List<string[]> {
            new[] { "Dashboard", "~/ResellerDashboard.aspx", "dash" },
            new[] { "Subscribers", "~/SupportSubscribers.aspx", "subs" },
            new[] { "Businesses", "~/ResellerDashboard.aspx?view=businesses", "businesses" },
            new[] { "Subscription Plans", "~/ResellerDashboard.aspx?view=plans", "plans" },
            new[] { "Usage & Limits", "~/ResellerDashboard.aspx?view=usage", "usage" },
            new[] { "Reports", "~/SupportReports.aspx", "reports" },
            new[] { "Settings", "~/ResellerDashboard.aspx?view=settings", "settings" } };
        string on = page.Equals("SupportSubscribers.aspx", StringComparison.OrdinalIgnoreCase) ? "subs" : page.Equals("SupportReports.aspx", StringComparison.OrdinalIgnoreCase) ? "reports"
            : page.Equals("ResellerDashboard.aspx", StringComparison.OrdinalIgnoreCase) ? (view.Length == 0 ? "dash" : view) : "";
        var html = new StringBuilder();
        foreach (var i in items)
            html.Append("<li class='am-sa-nav am-rsl-nav'><a href='").Append(HttpUtility.HtmlAttributeEncode(VirtualPathUtility.ToAbsolute(i[1]))).Append("' data-am-primary='true'")
                .Append(i[2] == on ? " aria-current='page' class='active'" : "").Append(">").Append(NavIcon(i[2])).Append("<span>").Append(HttpUtility.HtmlEncode(i[0])).Append("</span></a></li>");
        return html.ToString();
    }
    // v56 (PPT s1): line icons per nav item - home, users, building, paperclip, bar chart, document, gear.
    public static string NavIcon(string key)
    {
        string d = key == "dash" ? "<path d='M3 11l9-8 9 8'/><path d='M5 10v10h5v-6h4v6h5V10'/>"
            : key == "subs" ? "<path d='M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2'/><circle cx='9' cy='7' r='4'/><path d='M23 21v-2a4 4 0 0 0-3-3.9M16 3.1a4 4 0 0 1 0 7.8'/>"
            : key == "businesses" ? "<path d='M6 21V3h12v18M3 21h18M9 7h1m4 0h1M9 11h1m4 0h1M9 15h1m4 0h1M10 21v-3h4v3'/>"
            : key == "plans" ? "<path d='M21.4 11.1l-9.2 9.2a6 6 0 0 1-8.5-8.5l9.2-9.2a4 4 0 0 1 5.7 5.7l-9.2 9.2a2 2 0 0 1-2.8-2.8l8.5-8.5'/>"
            : key == "usage" ? "<path d='M4 20h16'/><path d='M7 16v-4M12 16V6M17 16v-7'/>"
            : key == "reports" ? "<path d='M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z'/><path d='M14 2v6h6M8 13h8M8 17h5'/>"
            : "<circle cx='12' cy='12' r='3'/><path d='M19.4 15a1.7 1.7 0 0 0 .3 1.8l.1.1a2 2 0 1 1-2.8 2.8l-.1-.1a1.7 1.7 0 0 0-1.8-.3 1.7 1.7 0 0 0-1 1.5V21a2 2 0 1 1-4 0v-.1a1.7 1.7 0 0 0-1.1-1.5 1.7 1.7 0 0 0-1.8.3l-.1.1a2 2 0 1 1-2.8-2.8l.1-.1a1.7 1.7 0 0 0 .3-1.8 1.7 1.7 0 0 0-1.5-1H3a2 2 0 1 1 0-4h.1a1.7 1.7 0 0 0 1.5-1.1 1.7 1.7 0 0 0-.3-1.8l-.1-.1a2 2 0 1 1 2.8-2.8l.1.1a1.7 1.7 0 0 0 1.8.3H9a1.7 1.7 0 0 0 1-1.5V3a2 2 0 1 1 4 0v.1a1.7 1.7 0 0 0 1 1.5 1.7 1.7 0 0 0 1.8-.3l.1-.1a2 2 0 1 1 2.8 2.8l-.1.1a1.7 1.7 0 0 0-.3 1.8V9a1.7 1.7 0 0 0 1.5 1H21a2 2 0 1 1 0 4h-.1a1.7 1.7 0 0 0-1.5 1z'/>";
        return "<svg class='am-ap-navico am-rsl-navico' viewBox='0 0 24 24' width='20' height='20' style='flex:0 0 20px;width:20px;height:20px' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true' focusable='false'>" + d + "</svg>";
    }
    // Top bar brand: AnyMinute mark + wordmark and the "Reseller" label (PPT header).
    public const string Brand = "<span class='am-ap-brand am-rsl-brand'><svg viewBox='0 0 40 40' width='34' height='34' aria-hidden='true' focusable='false'><circle cx='20' cy='21' r='15' fill='none' stroke='#1f6fe5' stroke-width='4'/><path d='M20 12v9.5l6 3.5' fill='none' stroke='#1f6fe5' stroke-width='3.6' stroke-linecap='round' stroke-linejoin='round'/><path d='M15 3.5h10' stroke='#1f6fe5' stroke-width='3.6' stroke-linecap='round'/></svg><span class='am-ap-brand-text'>AnyMinute</span><span class='am-rsl-brand-role' style='margin-left:18px;padding-left:18px;border-left:1px solid #dfe5ee;font-weight:700;font-size:20px;color:#14233c'>Reseller</span></span>";

    // ---- data (all scoped to AnyminuteResellerSubscriber.ResellerID = @rsl; same owner / business rules as SupportSubscribers) ----
    // s = the reseller's subscribers, biz = (subscriber, business) pairs the subscriber owns or is mapped to.
    static string Cte(bool sec)
    {
        return @"WITH s AS (SELECT u.UserId,u.EmployeeId,u.Email,u.Status,u.Is_First_Time_Login,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name," + (sec ? "ISNULL(ls.LoginDisabled,0)" : "CAST(0 AS bit)") + @" LoginDisabled
            FROM AnyminuteResellerSubscriber rs JOIN UserAcc u ON u.UserId=rs.UserId LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId" + (sec ? " LEFT JOIN AnyminuteLoginSecurity ls ON ls.UserId=u.UserId" : "") + @"
            WHERE rs.ResellerID=@rsl AND EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)),
            biz AS (SELECT s.UserId,a.BusinessID FROM s JOIN AssginBussiness a ON a.EmployeeID=s.EmployeeId AND a.UserTypeId=1
              UNION SELECT s.UserId,m.BusinessID FROM s JOIN MappingTable m ON m.USERID=s.UserId JOIN AssginBussiness a ON a.BusinessID=m.BusinessID AND a.EmployeeID=m.EmployeeID AND a.UserTypeId=m.UserTypeId) ";
    }
    // One row per subscriber: status inputs, active plan + limits and Businesses / Employees / Projects used.
    public static DataTable Subscribers(SqlConnection c, RslCtx r)
    {
        bool sec = SA.Has(c, "AnyminuteLoginSecurity");
        return SA.Q(c, Cte(sec) + @"SELECT s.*,pl.PlanName,pl.Business PlanBiz,pl.No_Of_Users PlanUsers,
            (SELECT COUNT(*) FROM biz JOIN AddBusiness b ON b.BusinessID=biz.BusinessID WHERE biz.UserId=s.UserId AND ISNULL(b.Status,'')<>'2') Businesses,
            (SELECT COUNT(DISTINCT a.EmployeeID) FROM biz JOIN AssginBussiness a ON a.BusinessID=biz.BusinessID JOIN AddEmployee ae ON ae.EmployeeID=a.EmployeeID WHERE biz.UserId=s.UserId AND a.EmployeeID<>s.EmployeeId AND ae.TerminationDate IS NULL) Employees,
            (SELECT COUNT(*) FROM biz JOIN AddProject p ON p.BusinessID=biz.BusinessID WHERE biz.UserId=s.UserId) Projects
            FROM s OUTER APPLY (SELECT TOP 1 p.PlanName,p.Business,p.No_Of_Users FROM PlanHistory p WHERE p.EmailId=s.Email AND ISNULL(p.IsActive,0)=1 ORDER BY p.StartDate DESC) pl ORDER BY s.Name", "@rsl", r.ResellerId);
    }
    // Businesses under the reseller's subscribers (deleted businesses shown as Deleted).
    public static DataTable Businesses(SqlConnection c, RslCtx r)
    {
        return SA.Q(c, Cte(false) + @"SELECT b.BusinessID,b.CompanyName,biz.UserId,s.Name,s.Email,CASE WHEN ISNULL(b.Status,'')='2' THEN 'Deleted' ELSE 'Active' END St,
            (SELECT COUNT(DISTINCT a.EmployeeID) FROM AssginBussiness a WHERE a.BusinessID=b.BusinessID AND a.EmployeeID<>s.EmployeeId) Employees,
            (SELECT COUNT(*) FROM AddProject p WHERE p.BusinessID=b.BusinessID) Projects
            FROM biz JOIN s ON s.UserId=biz.UserId JOIN AddBusiness b ON b.BusinessID=biz.BusinessID ORDER BY b.CompanyName", "@rsl", r.ResellerId);
    }
    // Same four states as SupportSubscribers.StatusOf.
    public static string StatusOf(DataRow r)
    {
        bool on = r["Status"] == DBNull.Value || Convert.ToBoolean(r["Status"]);
        return !on ? "Inactive" : Convert.ToBoolean(r["LoginDisabled"]) ? "Suspended" : r["Is_First_Time_Login"] != DBNull.Value && Convert.ToBoolean(r["Is_First_Time_Login"]) ? "Pending" : "Active";
    }
}
