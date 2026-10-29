using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-10-29 v28 (Shamsu deck 2 slides 3-4): the 8 Reseller reports. Empty with a notice until REQUIRED_09 is run.
public static partial class AnyminuteSupportReports
{
    static SrData NotReady(SrData d) { d.Note = RS.Missing; return d; }
    static string RStatus(DataRow r) { return SA.S(r["Status"]).Length == 0 ? "Active" : SA.S(r["Status"]); }
    static string RLogin(DataRow r) { return Convert.ToBoolean(r["LoginDisabled"]) || RStatus(r) == "Inactive" ? "Disabled" : "Enabled"; }
    static SrData ResellerMaster(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller ID", "Reseller Name", "Contact Person", "Email", "Phone", "Plan", "Status", "Subscribers", "Subscriber Limit", "Limit Status", "Businesses", "Max Businesses / Subscriber", "Permissions", "Created Date", "Created By");
        var t = RS.List(c); if (t == null) return NotReady(d);
        foreach (DataRow r in t.Rows)
        {
            int s = Convert.ToInt32(r["Subscribers"]); int? lim = Int(r["MaxSubscribers"]);
            d.Add(RS.Rsl(r["ResellerID"]), r["ResellerName"], r["ContactPerson"], r["Email"], r["Phone"], r["PlanName"], RStatus(r), s, lim == null ? "Unlimited" : lim.Value.ToString(Inv), LimitStatus(s, lim), r["Businesses"],
                Int(r["MaxBusinessesPerSubscriber"]) == null ? "Unlimited" : SA.S(r["MaxBusinessesPerSubscriber"]), RS.PermissionText(SA.S(r["Permissions"])), Day(Utc2(r["CreatedAtUTC"])), r["CreatedBy"]);
        }
        d.Opt("contact", "Contact Person", "Email", "Phone").Opt("perms", "Permissions");
        return d;
    }
    static SrData ResellerSubscriber(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller", "Reseller ID", "Subscriber ID", "Subscriber Name", "Email", "Plan", "Status", "Businesses", "Assigned Date", "Created Date");
        var t = RS.Subscribers(c, -1); if (t == null) return NotReady(d);
        foreach (DataRow r in t.Rows)
            d.Add(r["ResellerName"], RS.Rsl(r["ResellerID"]), RS.Sub(r["UserId"]), Name(r), r["Email"], r["PlanName"], Off(r) ? "Inactive" : First(r) ? "Pending Activation" : "Active", r["Businesses"], Day(Utc2(r["AssignedAtUTC"])), Day(r["Since"]));
        return d;
    }
    static SrData ResellerBusiness(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller", "Subscriber", "Subscriber ID", "Business ID", "Business Name", "City", "Country", "Status", "Agreement Date");
        if (!RS.Ready(c)) return NotReady(d);
        var t = SA.Q(c, @"SELECT r.ResellerName,u.UserId,u.Email,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name,b.BusinessID,b.CompanyName,b.City,b.PostalCode,b.Status BStatus,b.BusinessAgreementDate
            FROM AnyminuteResellerSubscriber s JOIN AnyminuteReseller r ON r.ResellerID=s.ResellerID JOIN UserAcc u ON u.UserId=s.UserId LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId
            JOIN (SELECT DISTINCT EmployeeID,BusinessID FROM AssginBussiness WHERE UserTypeId=1) a ON a.EmployeeID=u.EmployeeId JOIN AddBusiness b ON b.BusinessID=a.BusinessID ORDER BY r.ResellerName,u.UserId,b.CompanyName");
        foreach (DataRow r in t.Rows)
            d.Add(r["ResellerName"], Name(r), RS.Sub(r["UserId"]), "BIZ-" + SA.S(r["BusinessID"]), r["CompanyName"], r["City"], SA.GuessCountry(SA.S(r["PostalCode"])), SA.S(r["BStatus"]) == "2" ? "Inactive" : "Active", Day(r["BusinessAgreementDate"]));
        d.Note = "Country is derived from the business postal code format (Canada / USA / UK / India).";
        return d;
    }
    static SrData ResellerUsage(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller", "Reseller ID", "Limit Type", "Used", "Limit", "Usage %", "Limit Status");
        var t = RS.List(c); if (t == null) return NotReady(d);
        var subs = RS.Subscribers(c, -1).Rows.Cast<DataRow>().ToList();
        foreach (DataRow r in t.Rows)
        {
            int id = Convert.ToInt32(r["ResellerID"]), used = Convert.ToInt32(r["Subscribers"]); int? lim = Int(r["MaxSubscribers"]), bl = Int(r["MaxBusinessesPerSubscriber"]);
            int maxBiz = subs.Where(s => Convert.ToInt32(s["ResellerID"]) == id).Select(s => Convert.ToInt32(s["Businesses"])).DefaultIfEmpty(0).Max();
            d.Add(r["ResellerName"], RS.Rsl(id), "Subscribers", used, lim == null ? "Unlimited" : lim.Value.ToString(Inv), Pct(used, lim), LimitStatus(used, lim));
            d.Add(r["ResellerName"], RS.Rsl(id), "Businesses per Subscriber (highest)", maxBiz, bl == null ? "Unlimited" : bl.Value.ToString(Inv), Pct(maxBiz, bl), LimitStatus(maxBiz, bl));
        }
        d.Note = "Businesses per subscriber uses the reseller's busiest subscriber. A blank limit means unlimited.";
        return d;
    }
    static SrData ResellerAudit(SqlConnection c, bool values)
    {
        var d = values ? new SrData("Audit ID", "Reseller", "Reseller ID", "Action", "Changed By", "Date", "Previous Status", "New Status", "Previous Value", "New Value", "Comments")
                       : new SrData("Activity ID", "Reseller", "Reseller ID", "Activity Type", "Performed By", "Date", "Details");
        if (!RS.Ready(c)) return NotReady(d);
        var names = SA.Q(c, "SELECT ResellerID,ResellerName FROM AnyminuteReseller").Rows.Cast<DataRow>().ToDictionary(r => SA.S(r["ResellerID"]), r => SA.S(r["ResellerName"]));
        var a = Audit(c, "Entity='Reseller'"); if (a == null) { d.Note = SA.MissingTables; return d; }
        foreach (DataRow r in a.Rows)
        {
            string k = SA.S(r["EntityKey"]), n; names.TryGetValue(k, out n); int id; Int32.TryParse(k, out id);
            if (values) d.Add("AUD-" + SA.S(r["AuditID"]), n ?? "", id > 0 ? RS.Rsl(id) : k, r["Action"], r["ChangedByName"], Utc(r["ChangedAtUTC"]), r["PrevStatus"], r["NewStatus"], r["PrevValue"], r["NewValue"], r["Comments"]);
            else d.Add("ACT-" + SA.S(r["AuditID"]), n ?? "", id > 0 ? RS.Rsl(id) : k, r["Action"], r["ChangedByName"], Utc(r["ChangedAtUTC"]), SA.S(r["Comments"]).Length > 0 ? r["Comments"] : r["NewValue"]);
        }
        if (values) d.Opt("values", "Previous Value", "New Value");
        return d;
    }
    public static readonly string[] Periods = { "This Month", "Last Month", "Last 90 Days", "This Year", "All Time" };
    static void Period(string p, out DateTime? from, out DateTime? to)
    {
        var t = DateTime.Today; var m = new DateTime(t.Year, t.Month, 1); from = null; to = null;
        if (p == "This Month") { from = m; to = m.AddMonths(1); } else if (p == "Last Month") { from = m.AddMonths(-1); to = m; }
        else if (p == "Last 90 Days") { from = t.AddDays(-89); to = t.AddDays(1); } else if (p == "This Year") { from = new DateTime(t.Year, 1, 1); to = from.Value.AddYears(1); }
    }
    static SrData ResellerPerformance(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller", "Reseller ID", "Period", "Total Subscribers", "New Subscribers", "Active Subscribers", "Paid Subscribers (new)", "Conversion %", "Businesses", "Revenue (CAD)");
        var t = RS.List(c); if (t == null) return NotReady(d);
        string per = q["f_period"]; if (!Periods.Contains(per)) per = "This Month"; DateTime? from, to; Period(per, out from, out to);
        var subs = RS.Subscribers(c, -1).Rows.Cast<DataRow>().ToList();
        var pay = SA.Q(c, "SELECT EmailId,PlanName,StartDate,AmountPaid,Is_Cancelled FROM PlanHistory WHERE StartDate IS NOT NULL").Rows.Cast<DataRow>().ToList();
        Func<DateTime?, bool> inP = x => x != null && (from == null || x >= from) && (to == null || x < to);
        foreach (DataRow r in t.Rows)
        {
            int id = Convert.ToInt32(r["ResellerID"]); var mine = subs.Where(s => Convert.ToInt32(s["ResellerID"]) == id).ToList();
            var fresh = mine.Where(s => inP(Utc2(s["AssignedAtUTC"]))).ToList();
            var emails = new HashSet<string>(mine.Select(s => SA.S(s["Email"])), StringComparer.OrdinalIgnoreCase);
            Func<DataRow, bool> paid = s => pay.Any(p => String.Equals(SA.S(p["EmailId"]), SA.S(s["Email"]), StringComparison.OrdinalIgnoreCase) && !Bit(p["Is_Cancelled"]) && p["AmountPaid"] != DBNull.Value && Convert.ToDecimal(p["AmountPaid"]) > 0);
            int conv = fresh.Count(paid);
            decimal rev = pay.Where(p => emails.Contains(SA.S(p["EmailId"])) && inP(Dt(p["StartDate"])) && p["AmountPaid"] != DBNull.Value).Sum(p => Convert.ToDecimal(p["AmountPaid"]));
            d.Add(r["ResellerName"], RS.Rsl(id), per, mine.Count, fresh.Count, mine.Count(s => !Off(s)), conv, fresh.Count == 0 ? "" : Math.Round(conv * 100.0 / fresh.Count, 1).ToString("0.#", Inv), r["Businesses"], rev.ToString("0.00", Inv));
        }
        d.Note = "New = subscribers assigned to the reseller in the period. Conversion % = new subscribers with a paid (non-zero, not cancelled) plan / new subscribers. Revenue = PlanHistory amounts paid in the period by the reseller's subscribers.";
        return d;
    }
    static SrData ResellerLogin(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Reseller", "Reseller ID", "Login Email", "Login Status", "Invitation Status", "Welcome Email Sent", "Last Login", "Status", "Created Date");
        var t = RS.List(c); if (t == null) return NotReady(d);
        foreach (DataRow r in t.Rows)
            d.Add(r["ResellerName"], RS.Rsl(r["ResellerID"]), r["LoginEmail"], RLogin(r), SA.S(r["InvitationStatus"]).Length == 0 ? "Pending" : r["InvitationStatus"], Utc(r["WelcomeEmailSentAtUTC"]), Utc(r["LastLoginAtUTC"]), RStatus(r), Day(Utc2(r["CreatedAtUTC"])));
        d.Note = "Reseller sign-in is not yet wired to Login.aspx, so Last Login stays blank and invitations stay Pending until it is.";
        return d;
    }
}
