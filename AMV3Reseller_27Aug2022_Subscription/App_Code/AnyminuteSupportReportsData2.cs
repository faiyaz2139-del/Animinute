using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using SA = AnyminuteSupportAdmin;
using RS = AnyminuteResellers;

// 2026-10-29 v28: Support Ticket, Staff Performance, Ticket Audit, Address, Login & Access and Admin Activity reports.
public static partial class AnyminuteSupportReports
{
    static DataTable Tickets(SqlConnection c)
    {
        bool meta = SA.Has(c, "AnyminuteTicketMeta"), audit = SA.Has(c, "AnyminuteSupportAudit");
        return SA.Q(c, "SELECT t.TicketId,t.TicketNo,t.Name,t.BusinessName,t.ProblemType,t.Module,t.TicketSeverity,t.StatusType,t.TicketRaiseDate,t.TicketResolveDate,t.Description," +
            (meta ? "m.Subject,m.Category,m.Priority,m.AssignedTo,ISNULL(m.Archived,0) Archived" : "CAST(NULL AS nvarchar(200)) Subject,CAST(NULL AS nvarchar(60)) Category,CAST(NULL AS varchar(10)) Priority,CAST(NULL AS nvarchar(150)) AssignedTo,CAST(0 AS bit) Archived") +
            ",(SELECT MIN(k.CommentsDate) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) FirstComment,(SELECT MAX(k.CommentsDate) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) LastComment" +
            ",(SELECT TOP 1 k.Comments FROM Ticket_Comments k WHERE k.TicketId=t.TicketId ORDER BY k.CommentsDate DESC,k.TicketCommentId DESC) LastText" +
            ",(SELECT COUNT(*) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) Replies" +
            // v30: Created By (the audit "Created" row when Support raised it, else the requester) and the latest internal (audit) note.
            (audit ? ",(SELECT TOP 1 a.ChangedByName FROM AnyminuteSupportAudit a WHERE a.Entity='Ticket' AND a.EntityKey=CAST(t.TicketId AS nvarchar(50)) AND a.Action='Created' ORDER BY a.AuditID) CreatedBy" +
                ",(SELECT TOP 1 a.Comments FROM AnyminuteSupportAudit a WHERE a.Entity='Ticket' AND a.EntityKey=CAST(t.TicketId AS nvarchar(50)) AND ISNULL(a.Comments,'')<>'' ORDER BY a.AuditID DESC) InternalNote"
                : ",CAST(NULL AS nvarchar(150)) CreatedBy,CAST(NULL AS nvarchar(max)) InternalNote") +
            " FROM Ticket t" + (meta ? " LEFT JOIN AnyminuteTicketMeta m ON m.TicketId=t.TicketId" : "") + " ORDER BY t.TicketRaiseDate DESC,t.TicketId DESC");
    }
    static string Prio(DataRow r) { string p = SA.S(r["Priority"]); if (p.Length > 0) return p; string s = SA.S(r["TicketSeverity"]); return s == "1" ? "High" : s == "3" ? "Low" : "Medium"; }
    static double? Hours(object from, object to) { var a = Dt(from); var b = Dt(to); return a == null || b == null || b < a ? (double?)null : (b.Value - a.Value).TotalHours; }
    // v29 (live v28 Staff Performance: Avg Resolution 0.0 h / SLA 100% next to Avg First Response 128 h): the legacy
    // insert writes TicketResolveDate = TicketRaiseDate (both date-only) when the ticket is raised, so it is not a real
    // resolution time. Use it only when it is later than the raise date; otherwise the last reply; otherwise unknown.
    static object ResolvedAt(DataRow r)
    {
        var raised = Dt(r["TicketRaiseDate"]); var res = Dt(r["TicketResolveDate"]); var last = Dt(r["LastComment"]);
        if (res != null && (raised == null || res.Value.Date > raised.Value.Date)) return r["TicketResolveDate"];
        if (last != null && (raised == null || last.Value >= raised.Value)) return r["LastComment"];
        return DBNull.Value;
    }
    public static readonly string[] ResolutionStatuses = { "Resolved within SLA", "Resolved after SLA", "Resolved (time unknown)", "Unresolved" };
    // 6 Support Ticket
    static SrData SupportTicket(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Ticket ID", "Subject", "Subscriber", "Business", "Ticket Type", "Category", "Priority", "Status", "Assigned To", "Created By", "Created Date", "Resolution Status", "First Response (hrs)", "Resolution (hrs)", "Resolved Date", "Replies", "Latest Update", "Description", "Archived", "Internal Notes");
        foreach (DataRow r in Tickets(c).Rows)
        {
            string st = SA.StatusLabel(r["StatusType"]); string cat = SA.S(r["Category"]); if (cat.Length == 0) cat = SA.LegacyCategory(r["ProblemType"], r["Module"]);
            string subj = SA.S(r["Subject"]); if (subj.Length == 0) subj = TicketType(r["ProblemType"]);
            double? rh = st == "Resolved" ? Hours(r["TicketRaiseDate"], ResolvedAt(r)) : null; string by = SA.S(r["CreatedBy"]); if (by.Length == 0) by = SA.S(r["Name"]);
            string rs = st != "Resolved" ? "Unresolved" : rh == null ? "Resolved (time unknown)" : rh.Value <= 24 ? "Resolved within SLA" : "Resolved after SLA";
            d.Add(Ticket(r["TicketNo"], r["TicketId"]), subj, r["Name"], r["BusinessName"], TicketType(r["ProblemType"]), cat, Prio(r), st, SA.S(r["AssignedTo"]).Length == 0 ? "Unassigned" : r["AssignedTo"], by, Stamp(r["TicketRaiseDate"]), rs,
                Hrs(Hours(r["TicketRaiseDate"], r["FirstComment"])), Hrs(rh),
                st == "Resolved" ? Stamp(ResolvedAt(r)) : "", r["Replies"], r["LastText"], r["Description"], Convert.ToBoolean(r["Archived"]) ? "Yes" : "No", r["InternalNote"]);
        }
        d.Opt("details", "Description", "Ticket Type", "Business", "Archived").Opt("latest", "Latest Update", "Replies").Opt("time", "First Response (hrs)", "Resolution (hrs)").Opt("notes", "Internal Notes");
        d.Note = "First response = first reply in Ticket_Comments; resolution = raised to resolved (legacy resolve dates equal to the raise date are placeholders, so the last reply is used). Closed is not a separate legacy status, so resolved tickets count as closed.";
        return d;
    }
    // 7 Staff Performance (aggregate of the filtered tickets)
    static SrData StaffPost(SrData t, NameValueCollection q)
    {
        var d = new SrData("Staff Member", "Tickets Assigned", "Open", "In Progress", "Waiting", "Resolved", "Resolution Rate %", "Avg First Response (hrs)", "Avg Resolution (hrs)", "SLA Met % (24h)");
        int ia = t.Ix("Assigned To"), ist = t.Ix("Status"), ifr = t.Ix("First Response (hrs)"), irs = t.Ix("Resolution (hrs)");
        foreach (var g in t.Rows.GroupBy(r => r[ia]).OrderByDescending(g => g.Count()))
        {
            Func<string, int> n = s => g.Count(r => r[ist] == s); int res = n("Resolved");
            var fr = g.Select(r => r[ifr]).Where(x => x.Length > 0).Select(x => Double.Parse(x, Inv)).ToList(); var rs = g.Select(r => r[irs]).Where(x => x.Length > 0).Select(x => Double.Parse(x, Inv)).ToList();
            d.Add(g.Key, g.Count(), n("Open"), n("In Progress"), n("Waiting"), res, Math.Round(res * 100.0 / g.Count(), 1).ToString("0.#", Inv),
                fr.Count == 0 ? "" : Math.Round(fr.Average(), 1).ToString("0.0", Inv), rs.Count == 0 ? "" : Math.Round(rs.Average(), 1).ToString("0.0", Inv),
                rs.Count == 0 ? "" : Math.Round(rs.Count(x => x <= 24) * 100.0 / rs.Count, 1).ToString("0.#", Inv));
        }
        d.Note = "Grouped by the ticket's current assignee. SLA met = resolved within 24 hours of being raised (resolved tickets with a known resolution time only). Legacy tickets store the raise date without a time, so hours are counted from midnight of that day.";
        return d;
    }
    // 8 Ticket Audit
    static SrData TicketAudit(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Audit ID", "Date / Time", "Ticket ID", "Action", "Changed By", "Previous Status", "New Status", "Previous Value", "New Value", "Comments");
        var a = Audit(c, "Entity='Ticket'");
        if (a == null) { d.Note = SA.MissingTables; return d; }
        var nos = SA.Q(c, "SELECT TicketId,TicketNo FROM Ticket").Rows.Cast<DataRow>().ToDictionary(r => SA.S(r["TicketId"]), r => Ticket(r["TicketNo"], r["TicketId"]));
        foreach (DataRow r in a.Rows) { string k = SA.S(r["EntityKey"]), no; nos.TryGetValue(k, out no); d.Add("AUD-" + SA.S(r["AuditID"]), Utc(r["ChangedAtUTC"]), no ?? "TK-" + k, r["Action"], r["ChangedByName"], r["PrevStatus"], r["NewStatus"], r["PrevValue"], r["NewValue"], r["Comments"]); }
        d.Opt("values", "Previous Value", "New Value");
        return d;
    }
    // 9 / 10 Address Verification and Address Correction
    const string AddrNames = "COALESCE(ci.CityName,a.City) City,COALESCE(pr.ProvinceName,a.Province) Province";
    const string AddrJoins = " LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50))=CAST(a.City AS nvarchar(50)) LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50))=CAST(a.Province AS nvarchar(50))";
    static SrData AddressVerification(SqlConnection c, NameValueCollection q)
    {
        bool rv = SA.Has(c, "AnyminuteAddressReview");
        var d = new SrData("Address ID", "Submitted By", "Submitted Date", "Street", "City", "Province / State", "Country", "Postal Code", "Verification Status", "Reviewed By", "Reviewed Date", "Notes");
        var t = SA.Q(c, "SELECT a.AddressMasterId,a.Street_Name,a.PostalCode,a.Verifed," + AddrNames + (rv ? ",r.Status RStatus,r.SubmittedBy,r.SubmittedAtUTC,r.Country,r.ReviewedByName,r.ReviewedAtUTC,r.Notes" : "") + " FROM AddressMaster a" + AddrJoins + (rv ? " LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId=a.AddressMasterId" : "") + " ORDER BY a.AddressMasterId DESC");
        foreach (DataRow r in t.Rows)
        {
            string st = SA.S(r["Verifed"]) == "1" || SA.S(r["Verifed"]).Equals("True", StringComparison.OrdinalIgnoreCase) ? "Verified" : rv && SA.S(r["RStatus"]) == "Rejected" ? "Rejected" : "Pending";
            string country = rv ? SA.S(r["Country"]) : ""; if (country.Length == 0) country = SA.GuessCountry(SA.S(r["PostalCode"]));
            d.Add("ADR-" + SA.S(r["AddressMasterId"]), rv ? r["SubmittedBy"] : "", rv ? Utc(r["SubmittedAtUTC"]) : "", r["Street_Name"], r["City"], r["Province"], country, r["PostalCode"], st, rv ? r["ReviewedByName"] : "", rv ? Utc(r["ReviewedAtUTC"]) : "", rv ? r["Notes"] : "");
        }
        if (!rv) d.Note = SA.MissingTables;
        return d;
    }
    static SrData AddressCorrection(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Audit ID", "Correction Date", "Address ID", "Submitted By", "Original Address", "Corrected Address", "Decision", "Corrected By", "Country", "Reason / Notes");
        var a = Audit(c, "Entity='Address'");
        if (a == null) { d.Note = SA.MissingTables; return d; }
        var by = SA.Q(c, "SELECT AddressMasterId,SubmittedBy FROM AnyminuteAddressReview").Rows.Cast<DataRow>().ToDictionary(r => SA.S(r["AddressMasterId"]), r => SA.S(r["SubmittedBy"]));
        foreach (DataRow r in a.Rows)
        {
            string nv = SA.S(r["NewValue"]), country = ""; foreach (var k in SA.Countries) if (nv.EndsWith(k, StringComparison.OrdinalIgnoreCase) || nv.Contains(", " + k + ",")) country = k;
            string sb; by.TryGetValue(SA.S(r["EntityKey"]), out sb);
            d.Add("AUD-" + SA.S(r["AuditID"]), Utc(r["ChangedAtUTC"]), "ADR-" + SA.S(r["EntityKey"]), sb ?? "", r["PrevValue"], nv, r["Action"], r["ChangedByName"], country, r["Comments"]);
        }
        return d;
    }
    // 11 Login & Access
    static SrData LoginAccessReport(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Event ID", "Date / Time", "Subscriber ID", "Subscriber Name", "Email", "Event", "Result", "IP Address", "Device", "Login Access", "Invitation Status", "Two-Factor", "Details");
        var a = Audit(c, "Entity='Login'");
        if (a == null) { d.Note = SA.MissingTables; return d; }
        var subs = SubscriberTable(c).Rows.Cast<DataRow>().ToDictionary(r => SA.S(r["UserId"]));
        foreach (DataRow r in a.Rows)
        {
            DataRow s; subs.TryGetValue(SA.S(r["EntityKey"]), out s);
            d.Add("AUD-" + SA.S(r["AuditID"]), Utc(r["ChangedAtUTC"]), RS.Sub(r["EntityKey"]), s == null ? "" : Name(s), s == null ? "" : s["Email"], r["Action"], r["Success"] == DBNull.Value || Convert.ToBoolean(r["Success"]) ? "Success" : "Failure",
                r["IpAddress"], r["Device"], s == null ? "" : LoginAccess(s), s == null ? "" : Invitation(s), s == null ? "" : Convert.ToBoolean(s["TwoFactor"]) ? "On" : "Off", r["Comments"]);
        }
        d.Opt("device", "IP Address", "Device");
        d.Note = "Sign-in events are logged from Login.aspx since v25; earlier sign-ins are not in the log.";
        return d;
    }
    // 12 Admin Activity
    static SrData AdminActivity(SqlConnection c, NameValueCollection q)
    {
        var d = new SrData("Audit ID", "Date / Time", "Admin", "Module", "Action", "Record", "Subscriber", "Previous Status", "New Status", "Comments", "IP Address");
        var a = Audit(c, "ChangedByName<>'Subscriber' AND ISNULL(ChangedBy,0)>0");
        if (a == null) { d.Note = SA.MissingTables; return d; }
        var names = SubscriberNames(c); var tk = SA.Q(c, "SELECT TicketId,Name FROM Ticket").Rows.Cast<DataRow>().ToDictionary(r => SA.S(r["TicketId"]), r => SA.S(r["Name"]));
        foreach (DataRow r in a.Rows)
        {
            string en = SA.S(r["Entity"]), k = SA.S(r["EntityKey"]);
            string rec = en == "Ticket" ? "TK-" + k : en == "Address" ? "ADR-" + k : en == "Login" ? RS.Sub(k) : en == "Reseller" ? RS.Rsl(k) : en == "Plan" ? "PLN-" + k : k;
            string who = ""; int u; if (en == "Login" && Int32.TryParse(k, out u) && names.ContainsKey(u)) who = names[u]; else if (en == "Ticket" && tk.ContainsKey(k)) who = tk[k];
            d.Add("AUD-" + SA.S(r["AuditID"]), Utc(r["ChangedAtUTC"]), r["ChangedByName"], en == "Login" ? "Subscriber Access" : en, r["Action"], rec, who, r["PrevStatus"], r["NewStatus"], r["Comments"], r["IpAddress"]);
        }
        d.Opt("ip", "IP Address");
        return d;
    }
}
