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
    static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(AnyminuteResellerPortal));
    // v61: build stamp shown in the reseller sidebar (owner asked to see the version on reseller pages).
    public const string Build = "v63-20261104";
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
        if (user > 0)
        {
            try
            {
                using (var c = SA.Open())
                    if (RS.Ready(c))
                    {
                        // v61 (pt192u "Fix all", Bug 2): forgiving detection. v55-v60 needed RS.LoginUser(email)==user (MotherOrg join with an
                        // exact e-mail, UserTypeid 0 + 'Admin') and Status exactly '' / 'Active', so a reseller whose login row differed in case /
                        // spaces / MotherOrg e-mail fell back to the owner screens. Now: the session login's own UserAcc e-mail (or the session
                        // e-mail) matches AnyminuteReseller.LoginEmail case- and space-insensitively, Status blank or Active (any case), and the
                        // login is not a subscriber owner (MappingTable UserTypeId 1) or a Support / Super Admin login.
                        var ua = SA.Q(c, "SELECT TOP 1 LTRIM(RTRIM(ISNULL(Email,''))) Email,ISNULL(UserType,'') UserType,CASE WHEN EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1) THEN 1 ELSE 0 END Owner FROM UserAcc u WHERE u.UserId=@u", "@u", user);
                        string ue = ua.Rows.Count == 1 ? SA.S(ua.Rows[0]["Email"]) : "", ut = ua.Rows.Count == 1 ? SA.S(ua.Rows[0]["UserType"]).Trim() : "";
                        bool owner = ua.Rows.Count == 1 && Convert.ToInt32(ua.Rows[0]["Owner"]) == 1;
                        bool staff = ut.Equals("SupportAdmin", StringComparison.OrdinalIgnoreCase) || ut.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || ut.Equals("Support Admin", StringComparison.OrdinalIgnoreCase);
                        if (ue.Length == 0) ue = email;
                        if (ue.Length > 0 && !staff)
                        {
                            var t = SA.Q(c, @"SELECT TOP 1 ResellerID,ResellerName,Status,Permissions,MaxSubscribers FROM AnyminuteReseller
                                WHERE LOWER(LTRIM(RTRIM(ISNULL(LoginEmail,''))))=LOWER(@e) OR (@s<>'' AND LOWER(LTRIM(RTRIM(ISNULL(LoginEmail,''))))=LOWER(@s))
                                ORDER BY CASE WHEN LOWER(LTRIM(RTRIM(ISNULL(Status,'')))) IN ('','active') THEN 0 ELSE 1 END,ResellerID", "@e", ue, "@s", email);
                            string st = t.Rows.Count == 1 ? SA.S(t.Rows[0]["Status"]).Trim() : "";
                            bool active = t.Rows.Count == 1 && (st.Length == 0 || st.Equals("Active", StringComparison.OrdinalIgnoreCase));
                            // A subscriber owner is only a reseller when it is the reseller's own EnsureLogin login (legacy rule).
                            if (active && owner && RS.LoginUser(c, ue) != user) active = false;
                            if (active)
                            {
                                var r = t.Rows[0];
                                s = new RslCtx { ResellerId = Convert.ToInt32(r["ResellerID"]), Code = RS.Rsl(r["ResellerID"]), Name = SA.S(r["ResellerName"]), MaxSubscribers = r["MaxSubscribers"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaxSubscribers"]) };
                                foreach (var p in SA.S(r["Permissions"]).Split(',')) if (p.Trim().Length > 0) s.Perms.Add(p.Trim().ToLowerInvariant());
                            }
                            else if (t.Rows.Count == 1) Log.Info("Reseller portal: login " + user + " matches reseller " + SA.S(t.Rows[0]["ResellerID"]) + " but was not admitted (status '" + st + "', owner " + owner + ").");
                        }
                    }
            }
            catch (SqlException ex) { s = null; Log.Warn("Reseller portal detection failed", ex); }
            catch (InvalidOperationException ex) { s = null; Log.Warn("Reseller portal detection failed", ex); }
        }
        h.Session["AmRslPortal"] = Tuple.Create(DateTime.UtcNow.AddMinutes(2), uid, s); return s;
    }
    public static bool Is() { return Current() != null; }
    // v60 (Reseller PPT s9 + Add Plan): AnyminuteSubscriptionPlan.ResellerID exists (DB_OPTIONAL/OPTIONAL_06_ResellerPlans.sql).
    public static bool OwnPlans(SqlConnection c) { return SA.Has(c, "AnyminuteSubscriptionPlan") && AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteSubscriptionPlan", "ResellerID"); }
    // v61 (Bug 2, PPT s9 "+ Add Plan" must work): apply OPTIONAL_06 on demand - the same additive, idempotent statement (one NULLable
    // column; existing rows stay NULL = AnyMinute catalogue). Returns OwnPlans(c); false when the app login lacks ALTER permission
    // (the form then keeps the "run OPTIONAL_06" notice).
    public static bool EnsureOwnPlans(SqlConnection c)
    {
        if (OwnPlans(c)) return true;
        if (!SA.Has(c, "AnyminuteSubscriptionPlan")) return false;
        try
        {
            using (var cmd = new SqlCommand("IF COL_LENGTH('dbo.AnyminuteSubscriptionPlan','ResellerID') IS NULL ALTER TABLE dbo.AnyminuteSubscriptionPlan ADD ResellerID int NULL;", c)) cmd.ExecuteNonQuery();
            Log.Info("Reseller portal: applied OPTIONAL_06 (AnyminuteSubscriptionPlan.ResellerID) on first + Add Plan.");
        }
        catch (SqlException ex) { Log.Warn("Reseller portal: could not add AnyminuteSubscriptionPlan.ResellerID (run DB_OPTIONAL/OPTIONAL_06_ResellerPlans.sql)", ex); }
        return OwnPlans(c);
    }
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
        // v61 (Bug 2 "every reseller page"): the active item is highlighted on every page a reseller can reach - Reports, Usage, Plans,
        // Settings and the Settings sub-pages (My Profile / Login & Security / Change Password) included.
        Func<string, bool> pg = n => page.Equals(n, StringComparison.OrdinalIgnoreCase);
        string on = pg("SupportSubscribers.aspx") ? "subs" : pg("SupportReports.aspx") ? "reports" : pg("SupportPlans.aspx") ? (view == "usage" ? "usage" : "plans")
            : pg("SupportBusinesses.aspx") ? "businesses" : pg("MyProfile.aspx") || pg("LoginSecurity.aspx") || pg("Change Password.aspx") || pg("ChangePassword.aspx") ? "settings"
            : pg("ResellerDashboard.aspx") ? (view.Length == 0 ? "dash" : view == "plan" || view == "planadd" || view == "planedit" ? "plans" : view) : "";
        var html = new StringBuilder();
        foreach (var i in items)
            html.Append("<li class='am-sa-nav am-rsl-nav'><a href='").Append(HttpUtility.HtmlAttributeEncode(VirtualPathUtility.ToAbsolute(i[1]))).Append("' data-am-primary='true'")
                .Append(i[2] == on ? " aria-current='page' class='active'" : "").Append(">").Append(NavIcon(i[2])).Append("<span>").Append(HttpUtility.HtmlEncode(i[0])).Append("</span></a></li>");
        html.Append("<li class='am-sa-nav am-rsl-build' style='list-style:none;margin-top:18px;padding:6px 18px;font-size:11px;color:#8a97ab' title='AnyMinute build'>Build ").Append(Build).Append("</li>");
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
    public const string Brand = "<span class='am-ap-brand am-rsl-brand'><svg viewBox='0 0 40 40' width='34' height='34' aria-hidden='true' focusable='false'><circle cx='20' cy='21' r='15' fill='none' stroke='#1f6fe5' stroke-width='4'/><path d='M20 12v9.5l6 3.5' fill='none' stroke='#1f6fe5' stroke-width='3.6' stroke-linecap='round' stroke-linejoin='round'/><path d='M15 3.5h10' stroke='#1f6fe5' stroke-width='3.6' stroke-linecap='round'/></svg><span class='am-ap-brand-text'>AnyMinute</span><span class='am-rsl-brand-role' style='padding-left:18px;border-left:1px solid #dfe5ee;font-weight:700;font-size:20px;color:#14233c'>Reseller</span></span>";

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
