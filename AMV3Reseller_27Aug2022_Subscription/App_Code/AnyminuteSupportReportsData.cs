using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-10-29 v28: loaders for the 12 Support Admin reports (deck 1). Columns follow Pandey's sample PDFs / XLSX.
public static partial class AnyminuteSupportReports
{
    // 1 Subscriber Master
    static SrData SubscriberMaster(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Subscriber ID", "Subscriber Name", "Email", "Phone", "Plan", "Account Status", "Login Access", "Invitation Status", "Registration Date", "Activation Date",
            "Active Businesses", "Business Limit", "Business Usage %", "Active Employees", "Employee Limit", "Employee Usage %", "Active Projects", "Project Limit", "Project Usage %", "Last Login");
        foreach (DataRow r in SubscriberTable(c).Rows)
        {
            int b = Convert.ToInt32(r["Businesses"]), e = Convert.ToInt32(r["Employees"]), p = Convert.ToInt32(r["Projects"]); int? bl = Int(r["Business"]), el = Int(r["No_Of_Users"]), pl = Int(r["ProjectLimit"]);
            d.Add(RS.Sub(r["UserId"]), Name(r), r["Email"], r["MobilePhone"], r["PlanName"], AccountStatus(r), LoginAccess(r), Invitation(r), Day(r["Since"]), Day(r["PlanActivationDate"] == DBNull.Value ? r["Since"] : r["PlanActivationDate"]),
                b, bl, Pct(b, bl), e, el, Pct(e, el), p, pl, Pct(p, pl), Stamp(r["LastLoginDate"]));
        }
        d.Opt("contact", "Email", "Phone").Opt("usage", "Active Businesses", "Business Limit", "Business Usage %", "Active Employees", "Employee Limit", "Employee Usage %", "Active Projects", "Project Limit", "Project Usage %").Opt("access", "Login Access", "Invitation Status", "Last Login");
        d.Note = "Counts are active resources: businesses the subscriber owns, distinct active employees across those businesses (no double counting) and open projects. Limits are the subscriber's active PlanHistory entitlement (project limit from the plan catalogue).";
        return d;
    }
    // 2 Subscriber Activity
    static SrData SubscriberActivity(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Event ID", "Activity Date / Time", "Subscriber ID", "Subscriber Name", "Event Category", "Action / Event", "Field Changed", "Previous Value", "New Value", "Changed By", "Actor Role", "Source", "Reason / Comments", "Result", "Reference ID");
        var names = SubscriberNames(c); var ev = new List<KeyValuePair<DateTime, object[]>>();
        foreach (DataRow r in SubscriberTable(c).Rows)
            if (r["Since"] != DBNull.Value)
                ev.Add(new KeyValuePair<DateTime, object[]>(Convert.ToDateTime(r["Since"]), new object[] { "REG-" + SA.S(r["UserId"]), Stamp(r["Since"]), RS.Sub(r["UserId"]), Name(r), "Registration", "Subscriber Registered", "Account Status", "Not Created", "Pending Activation", Name(r), "Subscriber", "Web Portal", "First subscription record", "Success", "" }));
        foreach (var ch in PlanChanges(c))
            ev.Add(new KeyValuePair<DateTime, object[]>(ch.When, new object[] { "PH-" + ch.Id, Stamp(ch.When), RS.Sub(ch.User), ch.Name, "Subscription", ch.Type == "New Subscription" ? "Plan Started" : "Plan " + ch.Type, "Plan", ch.Prev, ch.Next, ch.Name, "Subscriber", "Billing", ch.Cancelled ? "Cancelled" : "", "Success", SA.S(ch.Trans) }));
        var a = Audit(c, "Entity='Login' AND Action NOT IN('Sign In','Failed Sign In')");
        if (a != null)
            foreach (DataRow r in a.Rows)
            {
                string act = SA.S(r["Action"]); int u; Int32.TryParse(SA.S(r["EntityKey"]), out u); string n; names.TryGetValue(u, out n);
                string cat = act.IndexOf("Disable", StringComparison.OrdinalIgnoreCase) >= 0 || act.IndexOf("Suspend", StringComparison.OrdinalIgnoreCase) >= 0 ? "Account Status"
                    : act.IndexOf("Invit", StringComparison.OrdinalIgnoreCase) >= 0 ? "Activation" : act.IndexOf("Profile", StringComparison.OrdinalIgnoreCase) >= 0 ? "Profile Update" : "Access";
                ev.Add(new KeyValuePair<DateTime, object[]>(Utc2(r["ChangedAtUTC"]).Value, new object[] { "AUD-" + SA.S(r["AuditID"]), Utc(r["ChangedAtUTC"]), u > 0 ? RS.Sub(u) : "", n ?? "", cat, act, "Login Access", r["PrevValue"], r["NewValue"], r["ChangedByName"], "Support Admin", "Support Console", r["Comments"], r["Success"] == DBNull.Value || Convert.ToBoolean(r["Success"]) ? "Success" : "Failure", "AUD-" + SA.S(r["AuditID"]) }));
            }
        foreach (var e in ev.OrderByDescending(x => x.Key)) d.Add(e.Value);
        d.Opt("values", "Field Changed", "Previous Value", "New Value").Opt("source", "Actor Role", "Source");
        d.Note = "Events come from PlanHistory (registration and plan changes) and the Support Admin audit log (access changes, invitations, password resets). Passwords, tokens and reset links are never exported.";
        return d;
    }
    sealed class PlanChange { public int Id, User; public DateTime When; public string Name, Prev, Next, Type, PrevLimits, NextLimits, Status; public object Trans, Amount, Refund; public bool Cancelled; }
    static bool Bit(object o) { return o != null && o != DBNull.Value && Convert.ToBoolean(o); }
    static List<PlanChange> PlanChanges(SqlConnection c)
    {
        var list = new List<PlanChange>();
        var t = SA.Q(c, @"SELECT p.Id,p.PlanName,p.StartDate,p.AmountPaid,p.TransId,p.EmailId,p.No_Of_Users,p.Business,p.IsActive,p.Is_Cancelled,p.Is_Requested_Plan,p.RefundAmt,u.UserId,u.Email,
            LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name FROM PlanHistory p
            CROSS APPLY (SELECT TOP 1 x.UserId,x.EmployeeId,x.Email FROM UserAcc x WHERE x.Email=p.EmailId AND EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=x.UserId AND m.UserTypeId=1) ORDER BY x.UserId) u
            LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId WHERE p.StartDate IS NOT NULL ORDER BY p.EmailId,p.StartDate,p.Id");
        DataRow prev = null;
        foreach (DataRow r in t.Rows)
        {
            if (prev != null && SA.S(prev["EmailId"]) != SA.S(r["EmailId"])) prev = null;
            string from = prev == null ? "" : SA.S(prev["PlanName"]), to = SA.S(r["PlanName"]);
            int a = PlanRank(from), b = PlanRank(to);
            string type = prev == null ? "New Subscription" : SA.PlanKey(from) == SA.PlanKey(to) ? "Renewal" : a >= 0 && b >= 0 ? (b > a ? "Upgrade" : b < a ? "Downgrade" : "Change") : "Change";
            bool cancelled = Bit(r["Is_Cancelled"]);
            list.Add(new PlanChange { Id = Convert.ToInt32(r["Id"]), User = Convert.ToInt32(r["UserId"]), When = Convert.ToDateTime(r["StartDate"]), Name = Name(r), Prev = from.Length == 0 ? "\u2014" : from, Next = to, Type = type,
                PrevLimits = prev == null ? "" : SA.S(prev["Business"]) + " biz / " + SA.S(prev["No_Of_Users"]) + " emp", NextLimits = SA.S(r["Business"]) + " biz / " + SA.S(r["No_Of_Users"]) + " emp",
                Status = cancelled ? "Cancelled" : Bit(r["Is_Requested_Plan"]) ? "Requested" : Bit(r["IsActive"]) ? "Active" : "Ended",
                Trans = r["TransId"], Amount = r["AmountPaid"], Refund = r["RefundAmt"], Cancelled = cancelled });
            prev = r;
        }
        return list;
    }
    // 3 Subscription Plan
    static SrData SubscriptionPlan(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Plan ID", "Plan Name", "Description", "Billing Frequency", "Monthly Price (CAD)", "Annual Price (CAD)", "Max Businesses", "Max Employees", "Max Projects", "Trial Days", "Active Subscribers", "Status", "Last Updated");
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); var raw = new Dictionary<string, string>();
        foreach (DataRow r in SA.Q(c, "SELECT PlanName,COUNT(DISTINCT EmailId) C FROM PlanHistory WHERE ISNULL(IsActive,0)=1 AND ISNULL(Is_Cancelled,0)=0 AND PlanName IS NOT NULL GROUP BY PlanName").Rows)
        { string k = SA.PlanKey(SA.S(r["PlanName"])); int n; counts.TryGetValue(k, out n); counts[k] = n + Convert.ToInt32(r["C"]); if (!raw.ContainsKey(k)) raw[k] = SA.S(r["PlanName"]); }
        Func<object, string> money = o => o == DBNull.Value ? "Custom" : Convert.ToDecimal(o).ToString("0.00", Inv);
        Func<object, string> lim = o => o == DBNull.Value ? "Custom" : SA.S(o);
        if (SA.Has(c, "AnyminuteSubscriptionPlan"))
            foreach (DataRow r in SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan ORDER BY PlanID").Rows)
            {
                string k = SA.PlanKey(SA.S(r["PlanName"])); int n; counts.TryGetValue(k, out n); counts.Remove(k);
                d.Add("PLN-" + Convert.ToInt32(r["PlanID"]).ToString("000", Inv), r["PlanName"], r["Description"], r["BillingFrequency"], money(r["MonthlyPrice"]), money(r["AnnualPrice"]), lim(r["MaxBusinesses"]), lim(r["MaxEmployees"]), lim(r["MaxProjects"]), r["TrialDays"], n, Convert.ToBoolean(r["IsActive"]) ? "Active" : "Inactive", Utc(r["UpdatedAtUTC"]));
            }
        d.Opt("pricing", "Monthly Price (CAD)", "Annual Price (CAD)").Opt("limits", "Max Businesses", "Max Employees", "Max Projects");
        if (!SA.Has(c, "AnyminuteSubscriptionPlan")) d.Note = "The plan catalogue (DB_REQUIRED/REQUIRED_07) is not installed; only billing plans found in PlanHistory are listed.";
        foreach (var k in counts) d.Add("", raw[k.Key], "Billing plan (not in catalogue)", "", "", "", "", "", "", "", k.Value, "Active", "");
        return d;
    }
    // 4 Usage & Limits
    static SrData UsageLimits(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Subscriber ID", "Subscriber Name", "Plan", "Limit Type", "Used", "Limit", "Usage %", "Limit Status");
        foreach (DataRow r in SubscriberTable(c).Rows)
        {
            string id = RS.Sub(r["UserId"]), n = Name(r), plan = SA.S(r["PlanName"]);
            int b = Convert.ToInt32(r["Businesses"]), e = Convert.ToInt32(r["Employees"]), p = Convert.ToInt32(r["Projects"]); int? bl = Int(r["Business"]), el = Int(r["No_Of_Users"]), pl = Int(r["ProjectLimit"]);
            d.Add(id, n, plan, "Businesses", b, bl == null ? "Unlimited" : bl.Value.ToString(Inv), Pct(b, bl), LimitStatus(b, bl));
            d.Add(id, n, plan, "Employees", e, el == null ? "Unlimited" : el.Value.ToString(Inv), Pct(e, el), LimitStatus(e, el));
            d.Add(id, n, plan, "Projects", p, pl == null ? "Unlimited" : pl.Value.ToString(Inv), Pct(p, pl), LimitStatus(p, pl));
        }
        d.Note = "Usage % = active count / effective limit. Near Limit = 80% or more. A blank limit is treated as unlimited (no division).";
        return d;
    }
    // 5 Upgrade & Change
    static SrData UpgradeChange(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Change ID", "Change Date", "Subscriber ID", "Subscriber Name", "Previous Plan", "New Plan", "Change Type", "Previous Limits", "New Limits", "Amount Paid (CAD)", "Refund (CAD)", "Status", "Reference ID");
        foreach (var ch in PlanChanges(c).OrderByDescending(x => x.When))
            d.Add("PH-" + ch.Id, Day(ch.When), RS.Sub(ch.User), ch.Name, ch.Prev, ch.Next, ch.Type, ch.PrevLimits, ch.NextLimits, ch.Amount == DBNull.Value ? "" : Convert.ToDecimal(ch.Amount).ToString("0.00", Inv), ch.Refund == DBNull.Value ? "" : Convert.ToDecimal(ch.Refund).ToString("0.00", Inv), ch.Status, ch.Trans);
        d.Opt("limits", "Previous Limits", "New Limits").Opt("billing", "Amount Paid (CAD)", "Refund (CAD)", "Reference ID");
        d.Note = "One row per PlanHistory record compared with the subscriber's previous record. Upgrade / Downgrade follow the plan order Trial < Basic < Standard < Professional < Enterprise.";
        return d;
    }
}
