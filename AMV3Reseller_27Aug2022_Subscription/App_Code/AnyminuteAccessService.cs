using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

// 2026-10-24 v9 Deck G: Accountant & Payroll Access. Business access itself stays on the legacy Accountant role
// (UserTypeId 12) through AnyminuteAccountantAssignments; invitation details live in DB_REQUIRED/REQUIRED_04_AccountantAccess.sql.
public sealed class AnyminuteAccessRow
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public int InviteId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Firm { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public bool Accountant { get; set; }
    public bool Payroll { get; set; }
    public bool PayRates, Export, ApprovePayroll, ManageAccess;
    public string Status { get; set; }
    public DateTime? Invited { get; set; }
    public DateTime? Expires { get; set; }
    public List<int> BusinessIds = new List<int>();
    public List<string> BusinessNames = new List<string>();
    public bool AllBusinesses { get; set; }
    public string BusinessText { get { return AllBusinesses ? "All Businesses" : BusinessNames.Count == 0 ? "\u2014" : String.Join(", ", BusinessNames); } }
    public string Initials
    {
        get
        {
            var parts = (Name ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            return (parts[0].Substring(0, 1) + (parts.Length > 1 ? parts[parts.Length - 1].Substring(0, 1) : "")).ToUpperInvariant();
        }
    }
    public int Tone { get { int h = 0; foreach (char ch in (Name ?? "")) h = (h * 31 + ch) & 0xffff; return h % 6; } }
    public string FirmText { get { return String.IsNullOrWhiteSpace(Firm) ? "\u2014" : Firm; } }
    public string InvitedText { get { return Invited.HasValue ? Invited.Value.ToString("MMM d, yyyy", Inv) : "\u2014"; } }
    public string ExpiryText { get { return Expires.HasValue ? Expires.Value.ToString("MMM d, yyyy", Inv) : "\u2014"; } }
    public string Key { get { return InviteId > 0 ? "InviteId=" + InviteId.ToString(Inv) : "UserId=" + UserId.ToString(Inv); } }
    public string TypeKey { get { return (Accountant ? "accountant " : "") + (Payroll ? "payroll" : ""); } }
}

public static class AnyminuteAccessService
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    static DataTable Q(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, tx, sql, a); }
    static void X(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { AnyminuteTimesheetRepair.Execute(c, tx, sql, a); }
    static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o).Trim(); }
    static int Actor() { return Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]); }
    public static bool Installed(SqlConnection c, SqlTransaction tx) { return AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAccessInvite") && AnyminuteEmployeeEditor.HasTable(c, tx, "AnyminuteAccessInviteBusiness"); }
    public static bool Installed() { using (var c = AnyminuteTimesheetRepair.Open()) return Installed(c, null); }

    // Businesses the signed-in subscriber / owner manages (id -> name), alphabetical.
    public static List<KeyValuePair<int, string>> Owned()
    {
        var list = new List<KeyValuePair<int, string>>();
        foreach (DataRow b in AnyminuteModernScope.Businesses().Rows)
        {
            int id = Convert.ToInt32(b["BusinessID"]);
            if (AnyminuteModernScope.Owns(id)) list.Add(new KeyValuePair<int, string>(id, S(b["CompanyName"])));
        }
        return list;
    }
    static string InList(IEnumerable<int> ids) { var s = String.Join(",", ids.Select(i => i.ToString(Inv))); return s.Length == 0 ? "0" : s; }

    sealed class Legacy { public int User; public string Name, Email, Phone; public List<int> Businesses = new List<int>(); }
    static Dictionary<int, Legacy> LegacyAccountants(SqlConnection c, SqlTransaction tx, IEnumerable<int> owned)
    {
        var map = new Dictionary<int, Legacy>();
        foreach (DataRow r in Q(c, tx, @"SELECT m.USERID,m.BusinessID,m.EmployeeID,dbo.FN_DESC('AddEmployee',m.EmployeeID) AS Name,ea.MainEmail,ea.MainPhone
            FROM MappingTable m JOIN AssginBussiness a ON a.EmployeeID=m.EmployeeID AND a.BusinessID=m.BusinessID AND a.UserTypeId=m.UserTypeId
            JOIN AddEmployee e ON e.EmployeeID=m.EmployeeID LEFT JOIN EmpAddress ea ON ea.EmpAddressId=e.ADDRESSID
            WHERE m.UserTypeId=12 AND m.BusinessID IN (" + InList(owned) + ") ORDER BY m.USERID,m.EmployeeID").Rows)
        {
            int u = Convert.ToInt32(r["USERID"]); Legacy l;
            if (!map.TryGetValue(u, out l)) { l = new Legacy { User = u, Name = S(r["Name"]), Email = S(r["MainEmail"]), Phone = S(r["MainPhone"]) }; map[u] = l; }
            int b = Convert.ToInt32(r["BusinessID"]); if (!l.Businesses.Contains(b)) l.Businesses.Add(b);
            if (l.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) l.Email = S(r["MainEmail"]);
        }
        foreach (var l in map.Values) if (l.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) l.Email = "";
        return map;
    }
    static AnyminuteAccessRow FromInvite(DataRow r)
    {
        return new AnyminuteAccessRow
        {
            InviteId = Convert.ToInt32(r["InviteID"]), UserId = r["UserID"] == DBNull.Value ? 0 : Convert.ToInt32(r["UserID"]),
            Name = S(r["FullName"]), Firm = S(r["FirmName"]), Email = S(r["Email"]), Phone = S(r["Phone"]),
            Accountant = Convert.ToBoolean(r["IsAccountant"]), Payroll = Convert.ToBoolean(r["IsPayrollAdmin"]),
            PayRates = Convert.ToBoolean(r["PermPayRates"]), Export = Convert.ToBoolean(r["PermExport"]), ApprovePayroll = Convert.ToBoolean(r["PermApprovePayroll"]), ManageAccess = Convert.ToBoolean(r["PermManageAccess"]),
            Status = S(r["Status"]), Invited = Convert.ToDateTime(r["InvitedAtUTC"]).ToLocalTime().Date, Expires = r["ExpiresOn"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["ExpiresOn"]).Date
        };
    }
    static Dictionary<int, List<int>> InviteBusinesses(SqlConnection c, SqlTransaction tx, string ids)
    {
        var d = new Dictionary<int, List<int>>();
        foreach (DataRow r in Q(c, tx, "SELECT InviteID,BusinessID FROM AnyminuteAccessInviteBusiness WHERE InviteID IN (" + ids + ")").Rows)
        {
            int i = Convert.ToInt32(r["InviteID"]); List<int> l; if (!d.TryGetValue(i, out l)) d[i] = l = new List<int>(); l.Add(Convert.ToInt32(r["BusinessID"]));
        }
        return d;
    }

    // All rows the owner may manage: invitation records plus legacy accountants not yet covered by one.
    // Also activates invitations whose email now has an accountant login, and expires access past its expiry date.
    public static List<AnyminuteAccessRow> List() { return List(0); }
    static List<AnyminuteAccessRow> List(int pass)
    {
        var owned = Owned(); var ownedIds = owned.Select(o => o.Key).ToList();
        var names = owned.ToDictionary(o => o.Key, o => o.Value);
        var rows = new List<AnyminuteAccessRow>();
        if (ownedIds.Count == 0) return rows;
        Dictionary<int, Legacy> legacy; DataTable invites = null; Dictionary<int, List<int>> ib = new Dictionary<int, List<int>>();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            legacy = LegacyAccountants(c, null, ownedIds);
            if (Installed(c, null))
            {
                invites = Q(c, null, @"SELECT i.* FROM AnyminuteAccessInvite i WHERE EXISTS(SELECT 1 FROM AnyminuteAccessInviteBusiness b WHERE b.InviteID=i.InviteID AND b.BusinessID IN (" + InList(ownedIds) + @"))
                    OR (i.UserID IS NOT NULL AND EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=i.UserID AND m.UserTypeId=12 AND m.BusinessID IN (" + InList(ownedIds) + "))) ORDER BY i.InviteID DESC");
                ib = InviteBusinesses(c, null, InList(invites.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r["InviteID"]))));
            }
        }
        var covered = new HashSet<int>(); bool changed = false;
        if (invites != null)
            foreach (DataRow r in invites.Rows)
            {
                var row = FromInvite(r); List<int> bs; ib.TryGetValue(row.InviteId, out bs); bs = bs ?? new List<int>();
                if (row.Status == "Revoked" || row.Status == "Expired") { row.BusinessIds = bs; rows.Add(row); continue; }
                Legacy l = null;
                if (row.UserId > 0) legacy.TryGetValue(row.UserId, out l);
                if (l == null && row.Email.Length > 0) l = legacy.Values.FirstOrDefault(x => String.Equals(x.Email, row.Email, StringComparison.OrdinalIgnoreCase));
                if (l != null && covered.Contains(l.User)) l = null;
                if (row.Expires.HasValue && row.Expires.Value < DateTime.Today) { Expire(row, l, ownedIds); changed = true; continue; }
                if (l != null)
                {
                    covered.Add(l.User);
                    if (row.Status != "Active" || row.UserId != l.User) { Activate(row.InviteId, l.User, bs.Where(b => ownedIds.Contains(b) && !l.Businesses.Contains(b))); changed = true; continue; }
                    row.BusinessIds = l.Businesses.ToList(); if (row.Name.Length == 0) row.Name = l.Name;
                }
                else if (row.Status == "Active") row.Status = "Invited"; // the linked login lost every accountant assignment
                if (l == null) row.BusinessIds = bs;
                rows.Add(row);
            }
        if (changed && pass < 2) return List(pass + 1); // re-read after activation / expiry so the page shows the real state
        foreach (var l in legacy.Values.Where(x => !covered.Contains(x.User)))
            rows.Add(new AnyminuteAccessRow { UserId = l.User, Name = l.Name, Email = l.Email, Phone = l.Phone, Firm = "", Accountant = true, Status = "Active", BusinessIds = l.Businesses.ToList() });
        foreach (var row in rows)
        {
            row.BusinessIds = row.BusinessIds.Where(b => names.ContainsKey(b)).Distinct().ToList();
            row.BusinessNames = row.BusinessIds.Select(b => names[b]).OrderBy(n => n).ToList();
            row.AllBusinesses = owned.Count > 1 && row.BusinessIds.Count == owned.Count;
        }
        return rows.OrderBy(r => r.Status == "Revoked" || r.Status == "Expired" ? 1 : 0).ThenBy(r => r.Name).ToList();
    }
    public static AnyminuteAccessRow Find(int invite, int user)
    {
        var row = List().FirstOrDefault(r => invite > 0 ? r.InviteId == invite : r.InviteId == 0 && r.UserId == user);
        if (row == null) throw new HttpException(404, "This access record was not found or is not in a business you manage.");
        return row;
    }

    static void Activate(int invite, int user, IEnumerable<int> missing)
    {
        foreach (int b in missing.ToList()) { try { AnyminuteAccountantAssignments.Save(b, user, false); } catch (InvalidOperationException) { } }
        using (var c = AnyminuteTimesheetRepair.Open())
            X(c, null, "UPDATE AnyminuteAccessInvite SET Status='Active',UserID=@u,ActivatedAtUTC=COALESCE(ActivatedAtUTC,SYSUTCDATETIME()),UpdatedAtUTC=SYSUTCDATETIME() WHERE InviteID=@i", "@u", user, "@i", invite);
    }
    static void Expire(AnyminuteAccessRow row, Legacy l, List<int> owned)
    {
        if (l != null) foreach (int b in l.Businesses.Where(owned.Contains).ToList()) { try { AnyminuteAccountantAssignments.Save(b, l.User, true); } catch (InvalidOperationException) { } }
        using (var c = AnyminuteTimesheetRepair.Open())
            X(c, null, "UPDATE AnyminuteAccessInvite SET Status='Expired',UpdatedAtUTC=SYSUTCDATETIME() WHERE InviteID=@i", "@i", row.InviteId);
    }

    public sealed class Input
    {
        public string Name, Firm, Email, Phone; public bool Accountant, Payroll, PayRates, Export, ApprovePayroll, ManageAccess;
        public List<int> Businesses = new List<int>(); public DateTime? Expires;
    }
    static void Validate(Input p, List<int> owned)
    {
        if (String.IsNullOrWhiteSpace(p.Name)) throw new InvalidOperationException("Full Name is required.");
        if (p.Name.Trim().Length > 150) throw new InvalidOperationException("Full Name can be up to 150 characters.");
        if (String.IsNullOrWhiteSpace(p.Firm)) throw new InvalidOperationException("Firm Name is required.");
        if (p.Firm.Trim().Length > 150) throw new InvalidOperationException("Firm Name can be up to 150 characters.");
        if (String.IsNullOrWhiteSpace(p.Email) || !Regex.IsMatch(p.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$") || p.Email.Trim().Length > 200) throw new InvalidOperationException("Enter a valid Email Address.");
        if ((p.Phone ?? "").Trim().Length > 40) throw new InvalidOperationException("Phone Number can be up to 40 characters.");
        if (!p.Accountant && !p.Payroll) throw new InvalidOperationException("Select at least one access type.");
        if (p.Businesses.Count == 0) throw new InvalidOperationException("Select at least one business.");
        if (p.Businesses.Any(b => !owned.Contains(b))) throw new InvalidOperationException("Select businesses you manage.");
        if (p.Expires.HasValue && p.Expires.Value.Date < DateTime.Today) throw new InvalidOperationException("Access expiry cannot be in the past.");
    }
    static void RequireModule(SqlConnection c)
    {
        if (!Installed(c, null)) throw new InvalidOperationException("Run DB_REQUIRED/REQUIRED_04_AccountantAccess.sql first: invitations need the new access tables.");
    }

    // Creates the invitation. If the email already belongs to an accountant login in a business you manage,
    // access is granted at once (Active); otherwise the record stays Invited and an invitation email is sent when mail is enabled.
    public static string Invite(Input p)
    {
        var owned = Owned().Select(o => o.Key).ToList(); Validate(p, owned);
        string email = p.Email.Trim(); int id;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            RequireModule(c);
            foreach (int b in p.Businesses) AnyminuteApprovalAssignments.RequireOwner(c, null, b);
            if (Q(c, null, "SELECT 1 FROM UserAcc WHERE Email=@m AND UserTypeid=1", "@m", email).Rows.Count > 0) throw new InvalidOperationException("This email belongs to a subscriber account and cannot be invited.");
            if (Q(c, null, "SELECT 1 FROM AnyminuteAccessInvite i JOIN AnyminuteAccessInviteBusiness b ON b.InviteID=i.InviteID WHERE i.Email=@m AND i.Status IN('Invited','Active') AND b.BusinessID IN (" + InList(p.Businesses) + ")", "@m", email).Rows.Count > 0)
                throw new InvalidOperationException("This email already has access or a pending invitation for one of these businesses. Edit that entry instead.");
            using (var tx = c.BeginTransaction())
            {
                using (var cmd = AnyminuteTimesheetRepair.Command(c, tx, @"INSERT AnyminuteAccessInvite(FullName,FirmName,Email,Phone,IsAccountant,IsPayrollAdmin,PermPayRates,PermExport,PermApprovePayroll,PermManageAccess,ExpiresOn,Status,InvitedBy)
                    OUTPUT inserted.InviteID VALUES(@n,@f,@m,@p,@a,@y,@p1,@p2,@p3,@p4,@x,'Invited',@by)",
                    "@n", p.Name.Trim(), "@f", p.Firm.Trim(), "@m", email, "@p", (p.Phone ?? "").Trim(), "@a", p.Accountant, "@y", p.Payroll, "@p1", p.PayRates, "@p2", p.Export, "@p3", p.ApprovePayroll, "@p4", p.ManageAccess,
                    "@x", p.Expires.HasValue ? (object)p.Expires.Value.Date : DBNull.Value, "@by", Actor())) id = Convert.ToInt32(cmd.ExecuteScalar());
                foreach (int b in p.Businesses.Distinct()) X(c, tx, "INSERT AnyminuteAccessInviteBusiness(InviteID,BusinessID) VALUES(@i,@b)", "@i", id, "@b", b);
                tx.Commit();
            }
        }
        var match = MatchLogin(email);
        if (match > 0)
        {
            var failed = new List<string>();
            foreach (int b in p.Businesses) { try { AnyminuteAccountantAssignments.Save(b, match, false); } catch (InvalidOperationException e) { failed.Add(e.Message); } }
            using (var c = AnyminuteTimesheetRepair.Open()) X(c, null, "UPDATE AnyminuteAccessInvite SET Status='Active',UserID=@u,ActivatedAtUTC=SYSUTCDATETIME() WHERE InviteID=@i", "@u", match, "@i", id);
            return "Access granted to " + email + " (existing accountant login)." + (failed.Count > 0 ? " Some businesses were skipped: " + String.Join(" ", failed.Distinct()) : "");
        }
        return "Invitation recorded for " + email + ". " + SendEmail(id) + " Access turns Active automatically once an Accountant login with this email exists (create it in Add Employee with the Accountant role).";
    }
    // An accountant login (UserTypeId 12) with this email in a business the owner manages.
    static int MatchLogin(string email)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var available = AnyminuteAccountantAssignments.Available(c, null);
            if (available.Rows.Count == 0) return 0;
            var ids = available.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r["EmployeeID"]));
            var hit = Q(c, null, "SELECT TOP 1 m.USERID FROM MappingTable m JOIN AddEmployee e ON e.EmployeeID=m.EmployeeID JOIN EmpAddress a ON a.EmpAddressId=e.ADDRESSID WHERE m.UserTypeId=12 AND a.MainEmail=@m AND m.USERID IN (SELECT USERID FROM MappingTable WHERE EmployeeID IN (" + InList(ids) + "))", "@m", email);
            return hit.Rows.Count == 1 ? Convert.ToInt32(hit.Rows[0][0]) : 0;
        }
    }
    public static string SendEmail(int invite)
    {
        bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
        if (!send) return "Email sending is turned off for this site (IsSendMail), so no email was sent.";
        string to, name, type, businesses, from;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var r = Q(c, null, "SELECT FullName,Email,IsAccountant,IsPayrollAdmin,ExpiresOn FROM AnyminuteAccessInvite WHERE InviteID=@i AND Status='Invited'", "@i", invite);
            if (r.Rows.Count == 0) throw new InvalidOperationException("Only pending invitations can be re-sent.");
            to = S(r.Rows[0]["Email"]); name = S(r.Rows[0]["FullName"]);
            type = Convert.ToBoolean(r.Rows[0]["IsAccountant"]) && Convert.ToBoolean(r.Rows[0]["IsPayrollAdmin"]) ? "Accountant and Payroll Administrator" : Convert.ToBoolean(r.Rows[0]["IsAccountant"]) ? "Accountant" : "Payroll Administrator";
            businesses = String.Join(", ", Q(c, null, "SELECT dbo.FN_DESC('AddBusiness',BusinessID) FROM AnyminuteAccessInviteBusiness WHERE InviteID=@i", "@i", invite).Rows.Cast<DataRow>().Select(x => S(x[0])));
            from = S(Q(c, null, "SELECT dbo.FN_DESC('AddEmployee',@e)", "@e", Actor()).Rows[0][0]);
        }
        var body = new StringBuilder();
        body.Append("<p>Hello ").Append(HttpUtility.HtmlEncode(name)).Append(",</p>");
        body.Append("<p>").Append(HttpUtility.HtmlEncode(from.Length > 0 ? from : "A business owner")).Append(" has invited you to AnyMinute as <b>").Append(HttpUtility.HtmlEncode(type)).Append("</b> for: ").Append(HttpUtility.HtmlEncode(businesses)).Append(".</p>");
        body.Append("<p>You will receive your sign-in details as soon as your account is set up.</p>");
        try { new EmailSettingController().SendHtmlFormattedEmail(to, "AnyMinute invitation: " + type + " access", body.ToString()); }
        catch (Exception e) { log4net.LogManager.GetLogger("Log").Error("Access invitation email failed", e); return "The invitation email could not be sent."; }
        return "Invitation email sent to " + to + ".";
    }

    public static string Update(AnyminuteAccessRow row, Input p)
    {
        var owned = Owned().Select(o => o.Key).ToList();
        p.Email = row.Email.Length > 0 && row.UserId > 0 ? row.Email : p.Email;
        Validate(p, owned);
        var notes = new List<string>();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            RequireModule(c);
            foreach (int b in p.Businesses.Union(row.BusinessIds).Where(owned.Contains)) AnyminuteApprovalAssignments.RequireOwner(c, null, b);
            int id = row.InviteId;
            using (var tx = c.BeginTransaction())
            {
                if (id == 0)
                    using (var cmd = AnyminuteTimesheetRepair.Command(c, tx, "INSERT AnyminuteAccessInvite(FullName,FirmName,Email,Status,UserID,InvitedBy,ActivatedAtUTC) OUTPUT inserted.InviteID VALUES(@n,@f,@m,'Active',@u,@by,SYSUTCDATETIME())",
                        "@n", p.Name.Trim(), "@f", p.Firm.Trim(), "@m", p.Email.Trim(), "@u", row.UserId, "@by", Actor())) id = Convert.ToInt32(cmd.ExecuteScalar());
                X(c, tx, @"UPDATE AnyminuteAccessInvite SET FullName=@n,FirmName=@f,Email=@m,Phone=@p,IsAccountant=@a,IsPayrollAdmin=@y,PermPayRates=@p1,PermExport=@p2,PermApprovePayroll=@p3,PermManageAccess=@p4,ExpiresOn=@x,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@by WHERE InviteID=@i",
                    "@n", p.Name.Trim(), "@f", p.Firm.Trim(), "@m", p.Email.Trim(), "@p", (p.Phone ?? "").Trim(), "@a", p.Accountant, "@y", p.Payroll, "@p1", p.PayRates, "@p2", p.Export, "@p3", p.ApprovePayroll, "@p4", p.ManageAccess,
                    "@x", p.Expires.HasValue ? (object)p.Expires.Value.Date : DBNull.Value, "@by", Actor(), "@i", id);
                X(c, tx, "DELETE AnyminuteAccessInviteBusiness WHERE InviteID=@i AND BusinessID IN (" + InList(owned) + ")", "@i", id);
                foreach (int b in p.Businesses.Distinct()) X(c, tx, "INSERT AnyminuteAccessInviteBusiness(InviteID,BusinessID) VALUES(@i,@b)", "@i", id, "@b", b);
                tx.Commit();
            }
        }
        if (row.UserId > 0 && row.Status == "Active")
        {
            foreach (int b in p.Businesses.Where(b => !row.BusinessIds.Contains(b))) { try { AnyminuteAccountantAssignments.Save(b, row.UserId, false); } catch (InvalidOperationException e) { notes.Add(e.Message); } }
            foreach (int b in row.BusinessIds.Where(b => !p.Businesses.Contains(b))) { try { AnyminuteAccountantAssignments.Save(b, row.UserId, true); } catch (InvalidOperationException e) { notes.Add(e.Message); } }
        }
        return "Access for " + p.Name.Trim() + " was updated." + (notes.Count > 0 ? " " + String.Join(" ", notes.Distinct()) : "");
    }

    // Revoke: removes the accountant role from every business you manage (legacy unassign; the person's records stay),
    // and marks the invitation Revoked. Nothing is hard-deleted.
    public static string Revoke(AnyminuteAccessRow row)
    {
        var owned = Owned().Select(o => o.Key).ToList(); var notes = new List<string>();
        if (row.UserId > 0)
            foreach (int b in row.BusinessIds.Where(owned.Contains).ToList()) { try { AnyminuteAccountantAssignments.Save(b, row.UserId, true); } catch (InvalidOperationException e) { notes.Add(e.Message); } }
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (Installed(c, null))
            {
                if (row.InviteId > 0) X(c, null, "UPDATE AnyminuteAccessInvite SET Status='Revoked',RevokedAtUTC=SYSUTCDATETIME(),RevokedBy=@by WHERE InviteID=@i", "@by", Actor(), "@i", row.InviteId);
                else
                {
                    int id; using (var cmd = AnyminuteTimesheetRepair.Command(c, null, "INSERT AnyminuteAccessInvite(FullName,FirmName,Email,IsAccountant,Status,UserID,RevokedAtUTC,RevokedBy) OUTPUT inserted.InviteID VALUES(@n,'',@m,1,'Revoked',@u,SYSUTCDATETIME(),@by)",
                        "@n", row.Name ?? "", "@m", row.Email ?? "", "@u", row.UserId, "@by", Actor())) id = Convert.ToInt32(cmd.ExecuteScalar());
                    foreach (int b in row.BusinessIds) X(c, null, "INSERT AnyminuteAccessInviteBusiness(InviteID,BusinessID) VALUES(@i,@b)", "@i", id, "@b", b);
                }
            }
            else if (row.InviteId > 0) throw new InvalidOperationException("Run DB_REQUIRED/REQUIRED_04_AccountantAccess.sql first.");
        }
        return "Access for " + row.Name + " was revoked. Their existing records are kept." + (notes.Count > 0 ? " " + String.Join(" ", notes.Distinct()) : "");
    }
}
