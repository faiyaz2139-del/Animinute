using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-29 v28: report catalogue (filters per Shamsu deck 1 slide 2 / deck 2 slide 3), filter engine, KPIs and export.
public static partial class AnyminuteSupportReports
{
    static SrFilter F(string key, string label, string col, string type, params string[] fixedOptions) { return new SrFilter(key, label, col, type, fixedOptions); }
    static string[][] O(params string[] v) { var l = new List<string[]>(); for (int i = 0; i + 2 < v.Length; i += 3) l.Add(new[] { v[i], v[i + 1], v[i + 2] }); return l.ToArray(); }
    static readonly string[] Yes = { "Active", "Inactive" };
    // Built lazily: the filter option arrays live in other partial files whose static initialisation order is not defined.
    static List<SrDef> defs;
    public static List<SrDef> Defs { get { return defs ?? (defs = BuildDefs()); } }
    static List<SrDef> BuildDefs() { return new List<SrDef>
    {
        new SrDef { Key = "subscriber-master", Group = "Support", Name = "Subscriber Master", Purpose = "Master list of subscribers with plan, account status and usage against limits.", Load = SubscriberMaster,
            Filters = new[] { F("sub", "Subscriber", "Subscriber Name", "select"), F("plan", "Plan", "Plan", "select"), F("status", "Status", "Account Status", "select", "Active", "Pending Activation", "Suspended", "Inactive"), F("act", "Activation Date", "Activation Date", "range") },
            Options = O("contact", "Include contact details", "1", "usage", "Include usage and limits", "1", "access", "Include login and invitation status", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Subscribers", d.Rows.Count), K("Active", Count(d, "Account Status", "Active")), K("Pending Activation", Count(d, "Account Status", "Pending Activation")), K("Suspended / Inactive", Count(d, "Account Status", "Suspended", "Inactive")) } },
        new SrDef { Key = "subscriber-activity", Group = "Support", Name = "Subscriber Activity", Purpose = "Timeline of registrations, plan changes and access changes per subscriber.", Load = SubscriberActivity,
            Filters = new[] { F("sub", "Subscriber", "Subscriber Name", "select"), F("type", "Event Type", "Event Category", "select"), F("date", "Date Range", "Activity Date / Time", "range") },
            Options = O("values", "Include previous / new values", "1", "source", "Include source and actor role", "0"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Events", d.Rows.Count), K("Subscribers", d.Rows.Select(r => r[d.Ix("Subscriber ID")]).Distinct().Count()), K("Plan Changes", Count(d, "Event Category", "Subscription")), K("Access Changes", d.Rows.Count(r => r[d.Ix("Event Category")] != "Subscription" && r[d.Ix("Event Category")] != "Registration")) } },
        new SrDef { Key = "subscription-plan", Group = "Support", Name = "Subscription Plan", Purpose = "Plan catalogue with prices, limits and active subscribers.", Load = SubscriptionPlan,
            Filters = new[] { F("plan", "Plan", "Plan Name", "select"), F("status", "Status", "Status", "select", Yes), F("freq", "Billing Frequency", "Billing Frequency", "select") },
            Options = O("pricing", "Include pricing", "1", "limits", "Include limits", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Plans", d.Rows.Count), K("Active Plans", Count(d, "Status", "Active")), K("Active Subscribers", Sum(d, "Active Subscribers")) } },
        new SrDef { Key = "usage-limits", Group = "Support", Name = "Usage & Limits", Purpose = "Businesses, employees and projects used against each subscriber's plan limits.", Load = UsageLimits,
            Filters = new[] { F("sub", "Subscriber", "Subscriber Name", "select"), F("plan", "Plan", "Plan", "select"), F("pct", "Usage Percentage (at least)", "Usage %", "min", Pcts), F("type", "Limit Type", "Limit Type", "select", "Businesses", "Employees", "Projects") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Subscribers", d.Rows.Select(r => r[0]).Distinct().Count()), K("Near Limit", Count(d, "Limit Status", "Near Limit")), K("At / Over Limit", Count(d, "Limit Status", "At Limit", "Over Limit")), K("Unlimited", Count(d, "Limit Status", "Unlimited")) } },
        new SrDef { Key = "upgrade-change", Group = "Support", Name = "Upgrade & Change", Purpose = "Plan upgrades, downgrades, renewals and new subscriptions.", Load = UpgradeChange,
            Filters = new[] { F("sub", "Subscriber", "Subscriber Name", "select"), F("prev", "Previous Plan", "Previous Plan", "select"), F("next", "New Plan", "New Plan", "select"), F("date", "Date Range", "Change Date", "range") },
            Options = O("limits", "Include limit changes", "1", "billing", "Include amounts and references", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Changes", d.Rows.Count), K("Upgrades", Count(d, "Change Type", "Upgrade")), K("Downgrades", Count(d, "Change Type", "Downgrade")), K("New Subscriptions", Count(d, "Change Type", "New Subscription")), K("Renewals", Count(d, "Change Type", "Renewal")) } },
        // v30 (Pandey post-v29, Shamsu deck 1 Support Ticket mock): Date Range (Custom) + From / To first, then Subscriber, Business,
        // Assigned Support Admin, Priority, Category, Status, Ticket Type, Created By, Resolution Status; four Additional Options.
        new SrDef { Key = "support-ticket", Group = "Support", Name = "Support Ticket", Purpose = "Filter the report to view and export support ticket information.", Load = SupportTicket,
            Filters = new[] { F("date", "Date Range", "Created Date", "range3"), F("sub", "Subscriber", "Subscriber", "select"), F("biz", "Business", "Business", "select"), F("assigned", "Assigned Support Admin", "Assigned To", "select"),
                F("prio", "Priority", "Priority", "select", SA.Priorities), F("cat", "Category", "Category", "select", SA.Categories), F("status", "Status", "Status", "select", SA.TicketStatuses),
                F("type", "Ticket Type", "Ticket Type", "select"), F("by", "Created By", "Created By", "select"), F("res", "Resolution Status", "Resolution Status", "select", ResolutionStatuses) },
            Options = O("details", "Include ticket details", "1", "latest", "Include latest activity", "0", "time", "Include time metrics", "0", "notes", "Include internal notes", "0"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Tickets", d.Rows.Count), K("Open", Count(d, "Status", "Open")), K("In Progress", Count(d, "Status", "In Progress")), K("Waiting", Count(d, "Status", "Waiting")), K("Resolved / Closed", Count(d, "Status", "Resolved")) } },
        new SrDef { Key = "staff-performance", Group = "Support", Name = "Support Staff Performance", Purpose = "Tickets handled per Support Admin with resolution rate, response times and SLA.", Load = SupportTicket, Post = StaffPost,
            Filters = new[] { F("assigned", "Support Admin", "Assigned To", "select"), F("date", "Date Range", "Created Date", "range"), F("status", "Ticket Status", "Status", "select", SA.TicketStatuses) },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Staff", d.Rows.Count), K("Tickets", Sum(d, "Tickets Assigned")), K("Resolved", Sum(d, "Resolved")), K("Open", Sum(d, "Open")) } },
        new SrDef { Key = "ticket-audit", Group = "Support", Name = "Ticket Audit", Purpose = "Every change made to support tickets: who, what and when.", Load = TicketAudit,
            Filters = new[] { F("ticket", "Ticket Number", "Ticket ID", "text"), F("action", "Action Type", "Action", "select"), F("by", "Changed By", "Changed By", "select"), F("date", "Date", "Date / Time", "range") },
            Options = O("values", "Include previous / new values", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Audit Entries", d.Rows.Count), K("Tickets", d.Rows.Select(r => r[2]).Distinct().Count()), K("Status Changes", d.Rows.Count(r => r[5] != r[6] && r[6].Length > 0)) } },
        new SrDef { Key = "address-verification", Group = "Support", Name = "Address Verification", Purpose = "Submitted addresses with verification status and reviewer.", Load = AddressVerification,
            Filters = new[] { F("country", "Country", "Country", "select", SA.Countries), F("prov", "Province / State", "Province / State", "select"), F("status", "Status", "Verification Status", "select", "Pending", "Verified", "Rejected"), F("date", "Submitted Date", "Submitted Date", "range"), F("rev", "Reviewer", "Reviewed By", "select") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Addresses", d.Rows.Count), K("Pending", Count(d, "Verification Status", "Pending")), K("Verified", Count(d, "Verification Status", "Verified")), K("Rejected", Count(d, "Verification Status", "Rejected")) } },
        new SrDef { Key = "address-correction", Group = "Support", Name = "Address Correction Audit", Purpose = "Original vs corrected addresses with the reviewer's decision and reason.", Load = AddressCorrection,
            Filters = new[] { F("subby", "Submitted By", "Submitted By", "select"), F("by", "Corrected By", "Corrected By", "select"), F("date", "Date", "Correction Date", "range"), F("dec", "Decision", "Decision", "select") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Reviews", d.Rows.Count), K("Approved", Count(d, "Decision", "Approved")), K("Corrected & Approved", Count(d, "Decision", "Corrected & Approved")), K("Rejected", Count(d, "Decision", "Rejected")) } },
        new SrDef { Key = "login-access", Group = "Support", Name = "Subscriber Login & Access", Purpose = "Sign-ins, failed attempts, access changes and invitation status per subscriber.", Load = LoginAccessReport,
            Filters = new[] { F("sub", "Subscriber", "Subscriber Name", "select"), F("login", "Login Status", "Login Access", "select", "Enabled", "Disabled"), F("inv", "Invitation Status", "Invitation Status", "select", "Pending", "Accepted", "Cancelled"), F("date", "Last Login", "Date / Time", "range") },
            Options = O("device", "Include IP address and device", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Events", d.Rows.Count), K("Successful Sign-ins", d.Rows.Count(r => r[5] == "Sign In" && r[6] == "Success")), K("Failed Sign-ins", Count(d, "Result", "Failure")), K("Subscribers", d.Rows.Select(r => r[2]).Distinct().Count()) } },
        new SrDef { Key = "admin-activity", Group = "Support", Name = "Support Admin Activity", Purpose = "Everything Support Admins changed across tickets, addresses, plans, access and resellers.", Load = AdminActivity,
            Filters = new[] { F("admin", "Support Admin", "Admin", "select"), F("sub", "Subscriber", "Subscriber", "select"), F("action", "Action Type", "Action", "select"), F("date", "Date", "Date / Time", "range") },
            Options = O("ip", "Include IP address", "0"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Actions", d.Rows.Count), K("Admins", d.Rows.Select(r => r[2]).Distinct().Count()), K("Ticket Actions", Count(d, "Module", "Ticket")), K("Address Actions", Count(d, "Module", "Address")) } },

        new SrDef { Key = "reseller-master", Group = "Reseller", Name = "Reseller Master", Purpose = "All resellers with contact, plan, status and subscriber limit.", Load = ResellerMaster,
            Filters = new[] { F("rsl", "Reseller Name", "Reseller Name", "select"), F("status", "Status", "Status", "select", Yes), F("date", "Created Date", "Created Date", "range"), F("limit", "Subscriber Limit", "Limit Status", "select", "Within Limit", "Near Limit", "At Limit", "Over Limit", "Unlimited") },
            Options = O("contact", "Include contact details", "1", "perms", "Include permissions", "0"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Total Resellers", d.Rows.Count), K("Active", Count(d, "Status", "Active")), K("Inactive", Count(d, "Status", "Inactive")), K("Total Subscribers", Sum(d, "Subscribers")) } },
        new SrDef { Key = "reseller-subscriber", Group = "Reseller", Name = "Reseller Subscriber", Purpose = "Subscribers under each reseller with plan and status.", Load = ResellerSubscriber,
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("plan", "Plan", "Plan", "select"), F("status", "Status", "Status", "select", "Active", "Pending Activation", "Inactive"), F("date", "Created Date", "Created Date", "range") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Subscribers", d.Rows.Count), K("Active", Count(d, "Status", "Active")), K("Resellers", d.Rows.Select(r => r[0]).Distinct().Count()), K("Businesses", Sum(d, "Businesses")) } },
        new SrDef { Key = "reseller-business", Group = "Reseller", Name = "Reseller Business", Purpose = "Businesses created under each reseller's subscribers.", Load = ResellerBusiness,
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("sub", "Subscriber", "Subscriber", "select"), F("country", "Country", "Country", "select", SA.Countries), F("status", "Status", "Status", "select", Yes) },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Businesses", d.Rows.Count), K("Active", Count(d, "Status", "Active")), K("Subscribers", d.Rows.Select(r => r[2]).Distinct().Count()) } },
        new SrDef { Key = "reseller-usage", Group = "Reseller", Name = "Reseller Usage & Limits", Purpose = "Subscribers and businesses used against each reseller's limits.", Load = ResellerUsage,
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("limit", "Limit Status", "Limit Status", "select", "Within Limit", "Near Limit", "At Limit", "Over Limit", "Unlimited"), F("pct", "Usage % (at least)", "Usage %", "min", Pcts) },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Resellers", d.Rows.Select(r => r[1]).Distinct().Count()), K("Near Limit", Count(d, "Limit Status", "Near Limit")), K("At / Over Limit", Count(d, "Limit Status", "At Limit", "Over Limit")) } },
        new SrDef { Key = "reseller-activity", Group = "Reseller", Name = "Reseller Activity", Purpose = "What happened on each reseller account and who did it.", Load = (c, q) => ResellerAudit(c, false),
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("type", "Activity Type", "Activity Type", "select"), F("by", "Performed By", "Performed By", "select"), F("date", "Date", "Date", "range") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Activities", d.Rows.Count), K("Resellers", d.Rows.Select(r => r[2]).Distinct().Count()) } },
        new SrDef { Key = "reseller-performance", Group = "Reseller", Name = "Reseller Performance", Purpose = "New subscribers, conversion and revenue per reseller for a period.", Load = ResellerPerformance,
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("period", "Period", "Period", "param", Periods) },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Resellers", d.Rows.Count), K("New Subscribers", Sum(d, "New Subscribers")), K("Paid (new)", Sum(d, "Paid Subscribers (new)")), K("Revenue (CAD)", d.Rows.Sum(r => Decimal.Parse(r[d.Ix("Revenue (CAD)")], Inv)).ToString("0.00", Inv)) } },
        new SrDef { Key = "reseller-login", Group = "Reseller", Name = "Reseller Login & Access", Purpose = "Reseller login status, invitation and welcome email.", Load = ResellerLogin,
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("login", "Login Status", "Login Status", "select", "Enabled", "Disabled"), F("inv", "Invitation Status", "Invitation Status", "select", "Pending", "Accepted", "Cancelled") },
            Kpis = d => new List<KeyValuePair<string, string>> { K("Resellers", d.Rows.Count), K("Login Enabled", Count(d, "Login Status", "Enabled")), K("Invitations Pending", Count(d, "Invitation Status", "Pending")) } },
        new SrDef { Key = "reseller-audit", Group = "Reseller", Name = "Reseller Audit", Purpose = "Full change log for resellers with previous and new values.", Load = (c, q) => ResellerAudit(c, true),
            Filters = new[] { F("rsl", "Reseller", "Reseller", "select"), F("action", "Action", "Action", "select"), F("by", "Changed By", "Changed By", "select"), F("date", "Date", "Date", "range") },
            Options = O("values", "Include previous / new values", "1"),
            Kpis = d => new List<KeyValuePair<string, string>> { K("Audit Entries", d.Rows.Count), K("Resellers", d.Rows.Select(r => r[2]).Distinct().Count()) } },
    }; }
    public static SrDef Def(string key) { return Defs.FirstOrDefault(d => d.Key == key) ?? Defs[0]; }
}
