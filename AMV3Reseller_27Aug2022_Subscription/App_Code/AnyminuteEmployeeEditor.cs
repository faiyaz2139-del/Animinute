using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

// 2026-10-24 v2: services behind the redesigned Employees screens (Add/Edit Employee, assignment
// details, rate history and Login & Access). Legacy tables and procedures are reused; the optional
// tables created by DB_REQUIRED/REQUIRED_01_EmployeeModule.sql are detected at run time and every
// feature that needs them degrades with a clear message when they are missing.
public sealed class AnyminuteEmployeeProfile
{
    public string FirstName, MiddleName, LastName, Email, Phone, Mobile, DateOfBirth, Gender, Sin, SinExpiry, Status;
    public string StreetNo, Line1, Line2, SuiteNumber, SuiteType, City, Province, PostalCode, Country;
    public DateTime? HireDate;
}
public sealed class AnyminuteAssignmentInput
{
    public int BusinessId; public int[] Roles = new int[0]; public string EmployeeType = "Regular", PayType = "Hourly", Status = "Active", Notes = "";
    public decimal PayRate; public decimal? BillingRate; public DateTime EffectiveDate = DateTime.Today; public DateTime? EndDate; public int ProjectId;
}
public sealed class AnyminuteAssignmentRow
{
    public int BusinessId { get; set; } public int EmployeeId { get; set; } public int RoleId { get; set; }
    public string Business { get; set; } public string[] Roles { get; set; } public string EmployeeType { get; set; } public string PayType { get; set; }
    public decimal? PayRate { get; set; } public decimal? BillingRate { get; set; } public DateTime? EffectiveDate { get; set; } public DateTime? EndDate { get; set; }
    public string Status { get; set; } public int ProjectId { get; set; } public string Notes { get; set; } public int[] RoleIds { get; set; }
    public string PayRateText { get { return PayRate.HasValue ? "$" + PayRate.Value.ToString("N2", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string EffectiveText { get { return EffectiveDate.HasValue ? EffectiveDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string EffectiveIso { get { return EffectiveDate.HasValue ? EffectiveDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string EndIso { get { return EndDate.HasValue ? EndDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string EndText { get { return EndDate.HasValue ? EndDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) : "\u2014"; } }
}
public static class AnyminuteEmployeeEditor
{
    public const string MissingTables = "This option needs the database update in DB_REQUIRED/REQUIRED_01_EmployeeModule.sql. Nothing was changed.";
    static readonly string[] AccessActions = { "CreateEmployee", "SendInvitation", "CancelInvitation", "LoginEnabled", "LoginDisabled", "RequirePasswordChange", "TwoFactorChanged", "CopyInvitationLink", "ResetPassword" };

    // ---------- helpers ----------
    static DataTable Q(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, tx, sql, a); }
    static void X(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { AnyminuteTimesheetRepair.Execute(c, tx, sql, a); }
    public static bool HasTable(SqlConnection c, SqlTransaction tx, string name) { return Q(c, tx, "SELECT 1 WHERE OBJECT_ID(@n,'U') IS NOT NULL", "@n", "dbo." + name).Rows.Count == 1; }
    public static bool HasColumn(SqlConnection c, SqlTransaction tx, string table, string column) { return Q(c, tx, "SELECT 1 WHERE COL_LENGTH(@t,@c) IS NOT NULL", "@t", "dbo." + table, "@c", column).Rows.Count == 1; }
    public static bool ModuleInstalled() { using (var c = AnyminuteTimesheetRepair.Open()) return HasTable(c, null, "AnyminuteAssignmentDetail") && HasTable(c, null, "AnyminuteLoginSecurity"); }
    static int Actor() { return Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]); }
    static object Db(string s) { return String.IsNullOrWhiteSpace(s) ? (object)DBNull.Value : s.Trim(); }
    static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o).Trim(); }
    static void Audit(SqlConnection c, SqlTransaction tx, int business, int employee, string action, string before, string after)
    {
        X(c, tx, "INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,@x,@bv,@av,SYSUTCDATETIME())",
          "@b", business, "@a", Actor(), "@e", employee, "@x", action, "@bv", Db(before), "@av", Db(after));
    }
    public static string[] StatusValues { get { return new[] { "Active", "Not Active", "Terminated" }; } }
    public static string[] PayTypes { get { return new[] { "Hourly", "Weekly", "Bi-Weekly", "Fort Nightly", "Monthly", "Salary" }; } }

    // Owner of every business the employee belongs to (same rule as the existing edit pages).
    public static AnyminuteEmployeeCardView RequireEditable(int employee)
    {
        var card = AnyminuteEmployeesView.RequireEmployee(employee);
        if (!card.Assignments.All(a => a.Manage)) throw new HttpException(403, "Subscriber ownership is required.");
        using (var c = AnyminuteTimesheetRepair.Open())
            foreach (var b in card.Assignments.Select(a => a.BusinessId).Distinct()) AnyminuteApprovalAssignments.RequireOwner(c, null, b);
        return card;
    }
    static int[] Records(AnyminuteEmployeeCardView card) { return card.Assignments.Select(a => a.EmployeeId).Distinct().ToArray(); }
    public static int RecordFor(AnyminuteEmployeeCardView card, int business)
    {
        var a = card.Assignments.FirstOrDefault(x => x.BusinessId == business);
        return a == null ? card.EmployeeId : a.EmployeeId;
    }
    public static int UserFor(AnyminuteEmployeeCardView card) { var u = card.Assignments.Select(a => a.UserId).Where(x => x > 0).Distinct().ToArray(); return u.Length == 1 ? u[0] : 0; }

    // ---------- profile ----------
    public static AnyminuteEmployeeProfile LoadProfile(int employee)
    {
        var p = new AnyminuteEmployeeProfile();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool country = HasColumn(c, null, "EmpAddress", "Country");
            var t = Q(c, null, "SELECT e.*,a.StreetNo,a.StreetName,a.Other1,a.SuiteNumber,a.SuitType,a.City,a.Province,a.Postalcode,a.MainPhone,a.Mobile AddrMobile,a.MainEmail" + (country ? ",a.Country" : "") + " FROM AddEmployee e LEFT JOIN EmpAddress a ON a.EmpAddressId=e.ADDRESSID WHERE e.EmployeeID=@e", "@e", employee);
            if (t.Rows.Count != 1) throw new HttpException(404, "Employee not found.");
            var r = t.Rows[0]; var comm = new CommanClass();
            p.FirstName = S(r["FirstName"]); p.MiddleName = S(r["MiddleName"]); p.LastName = S(r["LastName"]);
            p.Gender = S(r["Gender"]); p.SinExpiry = S(r["SINExpiry"]);
            p.DateOfBirth = SafeDecrypt(comm, S(r["DateOfBirth"])); p.Sin = SafeDecrypt(comm, S(r["SocialInsuranceNumber"]));
            DateTime hire; if (DateTime.TryParse(S(r["HireDate"]), out hire)) p.HireDate = hire;
            p.StreetNo = S(r["StreetNo"]); p.Line1 = S(r["StreetName"]); p.Line2 = S(r["Other1"]); p.SuiteNumber = S(r["SuiteNumber"]); p.SuiteType = S(r["SuitType"]);
            p.City = S(r["City"]); p.Province = S(r["Province"]); p.PostalCode = S(r["Postalcode"]); p.Phone = S(r["MainPhone"]); p.Mobile = S(r["AddrMobile"]);
            p.Email = S(r["MainEmail"]); if (p.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) p.Email = "";
            p.Country = country ? S(r["Country"]) : "";
            string status = S(r["Employee_Status"]); p.Status = StatusValues.Contains(status) ? status : (S(r["IsActive"]).ToUpperInvariant() == "NO" ? "Not Active" : "Active");
        }
        return p;
    }
    static string SafeDecrypt(CommanClass comm, string value) { if (value.Length == 0) return ""; try { return comm.Decrypt(value); } catch (Exception) { return ""; } }

    static void Validate(AnyminuteEmployeeProfile p, bool creating)
    {
        if (String.IsNullOrWhiteSpace(p.FirstName) || String.IsNullOrWhiteSpace(p.LastName)) throw new InvalidOperationException("First name and last name are required.");
        if (String.IsNullOrWhiteSpace(p.Phone)) throw new InvalidOperationException("Contact phone is required.");
        if (!String.IsNullOrWhiteSpace(p.Email) && !System.Text.RegularExpressions.Regex.IsMatch(p.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) throw new InvalidOperationException("Enter a valid email address.");
        if (String.IsNullOrWhiteSpace(p.StreetNo) || String.IsNullOrWhiteSpace(p.Line1) || String.IsNullOrWhiteSpace(p.City) || String.IsNullOrWhiteSpace(p.Province) || String.IsNullOrWhiteSpace(p.PostalCode))
            throw new InvalidOperationException("Street number, address line 1, city, province/state and postal code are required.");
        if (creating && !p.HireDate.HasValue) throw new InvalidOperationException("Hire date is required.");
        DateTime dob; if (!String.IsNullOrWhiteSpace(p.DateOfBirth) && (!DateTime.TryParse(p.DateOfBirth, out dob) || dob.Date >= DateTime.Today)) throw new InvalidOperationException("Enter a valid date of birth.");
        if (!StatusValues.Contains(p.Status)) throw new InvalidOperationException("Select a valid employee status.");
    }

    public static void SaveProfile(int employee, AnyminuteEmployeeProfile p)
    {
        Validate(p, false);
        var card = RequireEditable(employee); var comm = new CommanClass(); int actor = Actor();
        var old = LoadProfile(employee);
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            bool country = HasColumn(c, tx, "EmpAddress", "Country");
            foreach (int record in Records(card))
            {
                // Hire date is intentionally not editable (design: "Hire Date cannot be changed or edited").
                X(c, tx, "UPDATE AddEmployee SET FirstName=@f,MiddleName=@m,LastName=@l,DateOfBirth=@d,Gender=@g,SocialInsuranceNumber=@s,SINExpiry=@x,MobilePhone=@mo WHERE EmployeeID=@e",
                  "@f", p.FirstName.Trim(), "@m", (p.MiddleName ?? "").Trim(), "@l", p.LastName.Trim(), "@d", comm.encrypt((p.DateOfBirth ?? "").Trim()), "@g", (p.Gender ?? "").Trim(),
                  "@s", comm.encrypt((p.Sin ?? "").Trim()), "@x", (p.SinExpiry ?? "").Trim(), "@mo", (p.Mobile ?? "").Trim(), "@e", record);
                X(c, tx, "UPDATE a SET StreetNo=@no,StreetName=@l1,Other1=@l2,SuiteNumber=@sn,SuitType=@st,City=@ci,Province=@pr,Postalcode=@pc,MainPhone=@ph,Mobile=@mo" + (String.IsNullOrWhiteSpace(p.Email) ? "" : ",MainEmail=@em") + (country ? ",Country=@co" : "") + " FROM EmpAddress a JOIN AddEmployee e ON e.ADDRESSID=a.EmpAddressId WHERE e.EmployeeID=@e",
                  "@no", p.StreetNo.Trim().ToUpperInvariant(), "@l1", p.Line1.Trim().ToUpperInvariant(), "@l2", (p.Line2 ?? "").Trim(), "@sn", (p.SuiteNumber ?? "").Trim(), "@st", (p.SuiteType ?? "").Trim(),
                  "@ci", p.City.Trim().ToUpperInvariant(), "@pr", p.Province.Trim().ToUpperInvariant(), "@pc", p.PostalCode.Replace(" ", "").Trim(), "@ph", p.Phone.Trim(), "@mo", (p.Mobile ?? "").Trim(),
                  "@em", (p.Email ?? "").Trim(), "@co", (p.Country ?? "").Trim(), "@e", record);
            }
            // Login email follows the profile email only when no other account already uses it.
            int user = UserFor(card);
            if (user > 0 && !String.IsNullOrWhiteSpace(p.Email) && !String.Equals(p.Email.Trim(), old.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (Q(c, tx, "SELECT 1 FROM UserAcc WHERE Email=@m AND UserId<>@u", "@m", p.Email.Trim(), "@u", user).Rows.Count > 0) throw new InvalidOperationException("That email address is already used by another login.");
                X(c, tx, "UPDATE UserAcc SET Email=@m WHERE UserId=@u", "@m", p.Email.Trim(), "@u", user);
            }
            foreach (var a in card.Assignments.GroupBy(x => x.BusinessId)) Audit(c, tx, a.Key, a.First().EmployeeId, "EditEmployeeProfile", null, p.FirstName + " " + p.LastName);
            tx.Commit();
        }
        if (p.Status != old.Status)
            foreach (var a in card.Assignments.GroupBy(x => x.BusinessId)) AnyminuteEmployeeRepair.UpdateStatus(actor, 1, a.Key, a.First().EmployeeId, p.Status, DateTime.Today);
    }

    // ---------- create (mirrors Add_EmployeeV2 btnFinalSave_Click order) ----------
    public static int Create(AnyminuteEmployeeProfile p, IList<AnyminuteAssignmentInput> rows, bool loginEnabled, bool sendInvite, out string notice)
    { return Create(p, rows, loginEnabled, sendInvite, false, out notice); }

    // 2026-10-31 v37 (DOCX images 7-9: invited accountant could not sign in, email had no password). The Accountant & Payroll
    // Access invitation now creates the accountant login itself (role 12 in every invited business) through the same Create
    // path as Add Employee. Only name + email are known at invite time, so address / phone / hire date / pay rate are not required.
    public static int CreateAccountant(string fullName, string email, string phone, IEnumerable<int> businesses, out int user)
    {
        var parts = (fullName ?? "").Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
        var p = new AnyminuteEmployeeProfile
        {
            FirstName = parts.Length > 0 ? parts[0] : "", LastName = parts.Length > 1 ? parts[1].Trim() : "", MiddleName = "", Email = (email ?? "").Trim(), Phone = (phone ?? "").Trim(), Mobile = "",
            DateOfBirth = "", Gender = "", Sin = "", SinExpiry = "", Status = "Active", StreetNo = "", Line1 = "", Line2 = "", SuiteNumber = "", SuiteType = "", City = "", Province = "", PostalCode = "", Country = "",
            HireDate = DateTime.Today
        };
        if (p.FirstName.Length == 0 || p.Email.Length == 0) throw new InvalidOperationException("Full Name and Email Address are required.");
        var rows = businesses.Distinct().Select(b => new AnyminuteAssignmentInput { BusinessId = b, Roles = new[] { 12 }, EmployeeType = "Regular", PayType = "Hourly", Status = "Active", PayRate = 0m, EffectiveDate = DateTime.Today }).ToList();
        string notice; int first = Create(p, rows, true, false, true, out notice);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var u = Q(c, null, "SELECT TOP 1 USERID FROM MappingTable WHERE EmployeeID=@e AND UserTypeId=12", "@e", first);
            user = u.Rows.Count == 1 ? Convert.ToInt32(u.Rows[0][0]) : 0;
        }
        return first;
    }

    static int Create(AnyminuteEmployeeProfile p, IList<AnyminuteAssignmentInput> rows, bool loginEnabled, bool sendInvite, bool accountantInvite, out string notice)
    {
        if (!accountantInvite) Validate(p, true); notice = "";
        if (rows == null || rows.Count == 0) throw new InvalidOperationException("Add at least one business assignment.");
        if (rows.Select(r => r.BusinessId).Distinct().Count() != rows.Count) throw new InvalidOperationException("Each business can be assigned only once.");
        var comm = new CommanClass(); string email = (p.Email ?? "").Trim();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            foreach (var r in rows)
            {
                AnyminuteApprovalAssignments.RequireOwner(c, null, r.BusinessId); ValidateAssignment(c, null, r, accountantInvite);
                if (email.Length > 0 && Q(c, null, "SELECT 1 FROM AddEmployee ad JOIN EmpAddress ea ON ad.ADDRESSID=ea.EmpAddressId JOIN AssginBussiness ab ON ab.EmployeeID=ad.EmployeeID WHERE ab.BusinessID=@b AND ea.MainEmail=@m", "@b", r.BusinessId, "@m", email).Rows.Count > 0)
                    throw new InvalidOperationException("This email is already registered in the selected business.");
            }
            if (email.Length > 0 && Q(c, null, "SELECT 1 FROM UserAcc WHERE Email=@m AND UserTypeid=1", "@m", email).Rows.Count > 0) throw new InvalidOperationException("This email belongs to a subscriber account and cannot be added as an employee.");
        }
        string blank = email.Length > 0 ? email : DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture).Substring(0, 15) + "blank";
        // v86: a login that already exists for this email (same person in another business) is shared; never change its access here.
        bool existingLogin = false;
        if (email.Length > 0) using (var c = AnyminuteTimesheetRepair.Open()) existingLogin = Q(c, null, "SELECT 1 FROM UserAcc WHERE Email=@m", "@m", email).Rows.Count > 0;
        int first = 0; int user = 0;
        foreach (var r in rows)
        {
            int role = r.Roles[0]; string roleName;
            DataTable business, perms;
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                business = Q(c, null, "SELECT CompanyName,MotherOrgId FROM AddBusiness WHERE BusinessID=@b", "@b", r.BusinessId);
                roleName = S(Q(c, null, "SELECT UserType FROM UserType WHERE User_TypeId=@r", "@r", role).Rows[0][0]);
                perms = Q(c, null, "SELECT Role_Permission_Id FROM Role_Permission_Master WHERE UserTypeId=@r", "@r", role);
            }
            int mother = Convert.ToInt32(business.Rows[0]["MotherOrgId"]);
            var emp = new AddEmployeeEntityClass();
            emp.SINExpiry = (p.SinExpiry ?? "").Trim(); emp.UserType = role; emp.LegalName = ""; emp.Title = ""; emp.Occupation = "";
            emp.FirstName = p.FirstName.Trim(); emp.MiddleName = (p.MiddleName ?? "").Trim(); emp.LastName = p.LastName.Trim();
            emp.SocialInsuranceNumber = comm.encrypt((p.Sin ?? "").Trim()); emp.DateOfBirth = comm.encrypt((p.DateOfBirth ?? "").Trim());
            emp.Gender = (p.Gender ?? "").Trim(); emp.HireDate = p.HireDate.Value; emp.IsActive = p.Status == "Active" ? "YES" : "NO";
            emp.Apartment = DateTime.Now.ToString(); emp.TerminationDate = DateTime.Now.Date; emp.MotherOrgID = mother;
            var rate = new RateDetailsEntity();
            int empId = new AddEmployeeControlerClass().InsertPersonalDetails(emp, roleName, rate);
            if (first == 0) first = empId;
            rate.EmployeeId = empId; rate.Rate = (float)r.PayRate; rate.RateType = r.PayType; rate.JobTitle = "";
            rate.IsEmployeeEIExempty = "No"; rate.IsEmployeeCPPExempty = "No"; rate.RetainVacation = "No";
            if (role == 12 || role == 5) { rate.StartDate = DateTime.Today.AddYears(100); rate.EndDate = DateTime.Today.AddYears(100); }
            else { rate.StartDate = r.EffectiveDate; rate.EndDate = r.EndDate.HasValue ? r.EndDate.Value : DateTime.Today; }
            rate.effectivedate = r.EffectiveDate;
            new RateDetailsController().InsertRateDetails(rate, empId);
            var ua = new UserAccEntity { EmployeeId = empId, Email = blank, MotherOrgid = Convert.ToInt32(HttpContext.Current.Session["MotherOrgid"]), UserTypeid = role, UserType = roleName, ProfileImageName = "" };
            user = Convert.ToInt32(new UserAccController().Sp_InsertUserAccFromAddress(ua));
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                // v37: an accountant invitation has no address yet; never add blank city / province master rows.
                if (p.City.Trim().Length > 0) X(c, null, "IF NOT EXISTS(SELECT 1 FROM CityMaster WHERE CityName=@n) INSERT CityMaster(CityName,Duplicate) VALUES(@n,'False')", "@n", p.City.Trim());
                if (p.Province.Trim().Length > 0) X(c, null, "IF NOT EXISTS(SELECT 1 FROM ProvinceMaster WHERE ProvinceName=@n) INSERT ProvinceMaster(ProvinceName,Duplicate) VALUES(@n,'False')", "@n", p.Province.Trim());
                var city = Q(c, null, "SELECT TOP 1 CityId FROM CityMaster WHERE CityName=@n", "@n", p.City.Trim());
                var prov = Q(c, null, "SELECT TOP 1 ProvinceId FROM ProvinceMaster WHERE ProvinceName=@n", "@n", p.Province.Trim());
                var ad = new EmpAddressEntity
                {
                    Postalcode = p.PostalCode.Replace(" ", "").Trim(), NumberSuffix = "", StreetName = p.Line1.Trim().ToUpperInvariant(), StreetType = "", StreetDirection = "",
                    SuitType = (p.SuiteType ?? "").Trim(), SuiteNumber = (p.SuiteNumber ?? "").Trim(), SuitSuffix = "", City = p.City.Trim().ToUpperInvariant(), Province = p.Province.Trim().ToUpperInvariant(),
                    MainPhone = p.Phone.Trim(), WorkPhone = "", Mobile = (p.Mobile ?? "").Trim(), Fax = "", MainEmail = blank, CCEmail = "", Website = "", Other1 = (p.Line2 ?? "").Trim(),
                    StreetNo = p.StreetNo.Trim().ToUpperInvariant(), CityId = city.Rows.Count > 0 ? Convert.ToInt32(city.Rows[0][0]) : 0, ProvinceId = prov.Rows.Count > 0 ? Convert.ToInt32(prov.Rows[0][0]) : 0,
                    MotherOrgId = Convert.ToInt32(HttpContext.Current.Session["MotherOrgid"])
                };
                new EmpAddressController().InsertEmpAddress(ad, empId);
            }
            string name; using (var c = AnyminuteTimesheetRepair.Open()) name = S(Q(c, null, "SELECT dbo.FN_DESC('AddEmployee',@e)", "@e", empId).Rows[0][0]);
            new AddBusinessControllerClass().AssgineBusiness(r.BusinessId, empId, S(business.Rows[0]["CompanyName"]), name, role, mother);
            string permission = String.Concat(perms.Rows.Cast<DataRow>().Select(x => S(x[0]) + ","));
            using (var c = AnyminuteTimesheetRepair.Open())
            using (var tx = c.BeginTransaction())
            {
                X(c, tx, "IF EXISTS(SELECT 1 FROM User_Role_Permission WHERE Employee_Id=@e) UPDATE User_Role_Permission SET Role_Permission_Id=@p,BusinessId=@b WHERE Employee_Id=@e AND UserTypeId=@r AND Status='True' ELSE INSERT User_Role_Permission(Role_Permission_Id,Employee_Id,UserTypeId,Status,BusinessId) VALUES(@p,@e,@r,'True',@b)", "@e", empId, "@p", permission, "@b", r.BusinessId, "@r", role);
                X(c, tx, "INSERT MappingTable(USERID,UserTypeId,EmployeeID,BusinessID) VALUES(@u,@r,@e,@b)", "@u", user, "@r", role, "@e", empId, "@b", r.BusinessId);
                X(c, tx, "UPDATE AssginBussiness SET UserTypeId=@r WHERE EmployeeID=@e AND BusinessID=@b", "@r", role, "@e", empId, "@b", r.BusinessId);
                if (r.EmployeeType == "Project") X(c, tx, "UPDATE AddEmployee SET EMPLOYEETYPE='P' WHERE EmployeeID=@e", "@e", empId);
                if (r.ProjectId > 0) X(c, tx, "IF NOT EXISTS(SELECT 1 FROM ASSIGNPROJECT WHERE BUSINESSID=@b AND EMPLOYEEID=@e AND PROJECTID=@p) INSERT ASSIGNPROJECT(BUSINESSID,EMPLOYEEID,PROJECTID) VALUES(@b,@e,@p)", "@b", r.BusinessId, "@e", empId, "@p", r.ProjectId);
                SaveDetail(c, tx, r, empId);
                if (p.Status != "Active") X(c, tx, "UPDATE AddEmployee SET Employee_Status=@s,Effective_Date=CAST(GETDATE() AS date) WHERE EmployeeID=@e", "@s", p.Status, "@e", empId);
                Audit(c, tx, r.BusinessId, empId, "CreateEmployee", null, loginEnabled ? "Login enabled" : "Login disabled");
                tx.Commit();
            }
        }
        if (!loginEnabled && !existingLogin) { if (!SetSecurity(user, true, null, null, false, first, rows[0].BusinessId)) notice = "Employee saved. Login could not be marked disabled until the database update is installed; no invitation or password was sent."; }
        if (sendInvite && loginEnabled && email.Length > 0)
        {
            string result = SendInvitation(first, false); notice = (notice + " " + result).Trim();
        }
        return first;
    }

    // ---------- assignments ----------
    static void ValidateAssignment(SqlConnection c, SqlTransaction tx, AnyminuteAssignmentInput r, bool accountantInvite = false)
    {
        if (r.BusinessId <= 0) throw new InvalidOperationException("Select a business.");
        if (r.Roles == null || r.Roles.Length == 0) throw new InvalidOperationException("Select at least one role.");
        foreach (int role in r.Roles)
            if (Q(c, tx, "SELECT 1 FROM UserType u WHERE u.User_TypeId=@r AND (u.User_TypeId IN(2,4,5,12) OR (u.User_TypeId NOT IN(1,2,3,4,5,6,12) AND EXISTS(SELECT 1 FROM AssginBussiness o WHERE o.BusinessID=@b AND o.UserTypeId=1 AND o.EmployeeID=u.BownerID)))", "@r", role, "@b", r.BusinessId).Rows.Count != 1)
                throw new InvalidOperationException("Select valid roles for this business.");
        if (r.EmployeeType != "Regular" && r.EmployeeType != "Project") throw new InvalidOperationException("Select an employee type.");
        if (!PayTypes.Contains(r.PayType)) throw new InvalidOperationException("Select a pay type.");
        if (!(accountantInvite && r.Roles.All(x => x == 12)) && (r.PayRate <= 0 || r.PayRate > 1000000)) throw new InvalidOperationException("Enter a pay rate greater than zero.");
        if (r.BillingRate.HasValue && (r.BillingRate < 0 || r.BillingRate > 1000000)) throw new InvalidOperationException("Enter a valid billing rate.");
        if (r.EndDate.HasValue && r.EndDate.Value.Date < r.EffectiveDate.Date) throw new InvalidOperationException("End date cannot be before the effective date.");
        if (!StatusValues.Contains(r.Status)) throw new InvalidOperationException("Select an assignment status.");
        if ((r.Notes ?? "").Length > 500) throw new InvalidOperationException("Notes can be up to 500 characters.");
        if (r.ProjectId > 0 && Q(c, tx, "SELECT 1 FROM AddProject WHERE ProjectID=@p AND BusinessID=@b", "@p", r.ProjectId, "@b", r.BusinessId).Rows.Count != 1) throw new InvalidOperationException("Select a project from the chosen business.");
    }
    static void SaveDetail(SqlConnection c, SqlTransaction tx, AnyminuteAssignmentInput r, int record)
    {
        if (HasTable(c, tx, "AnyminuteAssignmentDetail"))
            X(c, tx, @"MERGE AnyminuteAssignmentDetail AS t USING (SELECT @b BusinessID,@e EmployeeID) s ON t.BusinessID=s.BusinessID AND t.EmployeeID=s.EmployeeID
                WHEN MATCHED THEN UPDATE SET BillingRate=@br,EffectiveDate=@ef,EndDate=@en,DefaultProjectID=@p,Notes=@n,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a
                WHEN NOT MATCHED THEN INSERT(BusinessID,EmployeeID,BillingRate,EffectiveDate,EndDate,DefaultProjectID,Notes,UpdatedAtUTC,UpdatedBy) VALUES(@b,@e,@br,@ef,@en,@p,@n,SYSUTCDATETIME(),@a);",
                "@b", r.BusinessId, "@e", record, "@br", r.BillingRate.HasValue ? (object)r.BillingRate.Value : DBNull.Value, "@ef", r.EffectiveDate.Date,
                "@en", r.EndDate.HasValue ? (object)r.EndDate.Value.Date : DBNull.Value, "@p", r.ProjectId > 0 ? (object)r.ProjectId : DBNull.Value, "@n", Db(r.Notes), "@a", Actor());
        if (HasTable(c, tx, "AnyminuteAssignmentRole"))
        {
            X(c, tx, "DELETE AnyminuteAssignmentRole WHERE BusinessID=@b AND EmployeeID=@e", "@b", r.BusinessId, "@e", record);
            foreach (int extra in r.Roles.Skip(1).Distinct())
                X(c, tx, "INSERT AnyminuteAssignmentRole(BusinessID,EmployeeID,UserTypeId,AddedAtUTC,AddedBy) VALUES(@b,@e,@r,SYSUTCDATETIME(),@a)", "@b", r.BusinessId, "@e", record, "@r", extra, "@a", Actor());
        }
    }

    public static List<AnyminuteAssignmentRow> Assignments(AnyminuteEmployeeCardView card)
    {
        var list = new List<AnyminuteAssignmentRow>();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool detail = HasTable(c, null, "AnyminuteAssignmentDetail"), extra = HasTable(c, null, "AnyminuteAssignmentRole");
            foreach (var a in card.Assignments.OrderBy(x => x.Business))
            {
                var row = new AnyminuteAssignmentRow { BusinessId = a.BusinessId, EmployeeId = a.EmployeeId, RoleId = a.RoleId, Business = a.Business, EmployeeType = a.Type, Status = a.Status == "Inactive" ? "Not Active" : a.Status, Notes = "" };
                var roles = new List<string> { a.Role }; var ids = new List<int> { a.RoleId };
                if (extra) foreach (DataRow r in Q(c, null, "SELECT u.User_TypeId,u.UserType FROM AnyminuteAssignmentRole x JOIN UserType u ON u.User_TypeId=x.UserTypeId WHERE x.BusinessID=@b AND x.EmployeeID=@e AND x.UserTypeId<>@r ORDER BY u.UserType", "@b", a.BusinessId, "@e", a.EmployeeId, "@r", a.RoleId).Rows) { roles.Add(S(r[1])); ids.Add(Convert.ToInt32(r[0])); }
                row.Roles = roles.Where(x => x.Length > 0).ToArray(); row.RoleIds = ids.ToArray();
                var rate = Q(c, null, "SELECT TOP 1 Rate,RateType,effectivedate FROM RateDetails WHERE EmployeeId=@e AND effectivedate<=GETDATE() ORDER BY effectivedate DESC,RateId DESC", "@e", a.EmployeeId);
                if (rate.Rows.Count == 1) { row.PayRate = Convert.ToDecimal(rate.Rows[0]["Rate"]); row.PayType = S(rate.Rows[0]["RateType"]); if (rate.Rows[0]["effectivedate"] != DBNull.Value) row.EffectiveDate = Convert.ToDateTime(rate.Rows[0]["effectivedate"]); }
                if (detail)
                {
                    var d = Q(c, null, "SELECT BillingRate,EffectiveDate,EndDate,DefaultProjectID,Notes FROM AnyminuteAssignmentDetail WHERE BusinessID=@b AND EmployeeID=@e", "@b", a.BusinessId, "@e", a.EmployeeId);
                    if (d.Rows.Count == 1)
                    {
                        var r = d.Rows[0]; if (r["BillingRate"] != DBNull.Value) row.BillingRate = Convert.ToDecimal(r["BillingRate"]);
                        if (r["EffectiveDate"] != DBNull.Value) row.EffectiveDate = Convert.ToDateTime(r["EffectiveDate"]);
                        if (r["EndDate"] != DBNull.Value) row.EndDate = Convert.ToDateTime(r["EndDate"]);
                        if (r["DefaultProjectID"] != DBNull.Value) row.ProjectId = Convert.ToInt32(r["DefaultProjectID"]); row.Notes = S(r["Notes"]);
                    }
                }
                list.Add(row);
            }
        }
        return list;
    }

    // v7 slide 9 trash icon: soft-ends one business assignment (status Not Active + end date today), never deletes rows.
    public static void EndAssignment(int employee, int business)
    {
        var card = RequireEditable(employee);
        var a = card.Assignments.FirstOrDefault(x => x.BusinessId == business);
        if (a == null) throw new InvalidOperationException("That assignment was not found.");
        if (a.Status != "Active") throw new InvalidOperationException("That assignment is already not active.");
        if (!card.Assignments.Any(x => x.BusinessId != business && x.Status == "Active")) throw new InvalidOperationException("The last active assignment cannot be removed. Use Deactivate Employee instead.");
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
            if (HasTable(c, tx, "AnyminuteAssignmentDetail"))
                X(c, tx, @"IF EXISTS(SELECT 1 FROM AnyminuteAssignmentDetail WHERE BusinessID=@b AND EmployeeID=@e)
                    UPDATE AnyminuteAssignmentDetail SET EndDate=CAST(GETDATE() AS date),UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE BusinessID=@b AND EmployeeID=@e
                    ELSE INSERT AnyminuteAssignmentDetail(BusinessID,EmployeeID,EndDate,UpdatedBy) VALUES(@b,@e,CAST(GETDATE() AS date),@a);", "@b", business, "@e", a.EmployeeId, "@a", Actor());
            Audit(c, tx, business, a.EmployeeId, "EndAssignment", "Active", "Not Active (ended " + DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ")");
            tx.Commit();
        }
        AnyminuteEmployeeRepair.UpdateStatus(Actor(), 1, business, a.EmployeeId, "Not Active", DateTime.Today);
    }

    // Add a business to an existing employee, or edit one assignment.
    public static void SaveAssignment(int employee, AnyminuteAssignmentInput r)
    {
        var card = RequireEditable(employee); int actor = Actor();
        using (var c = AnyminuteTimesheetRepair.Open()) { AnyminuteApprovalAssignments.RequireOwner(c, null, r.BusinessId); ValidateAssignment(c, null, r); }
        bool existing = card.Assignments.Any(a => a.BusinessId == r.BusinessId);
        var current = existing ? Assignments(card).First(a => a.BusinessId == r.BusinessId) : null;
        if (!existing || current.RoleId != r.Roles[0]) AnyminuteEmployeeAssignmentService.SaveRole(existing ? RecordFor(card, r.BusinessId) : card.EmployeeId, r.BusinessId, r.Roles[0]);
        int record = existing ? RecordFor(card, r.BusinessId) : card.EmployeeId;
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            X(c, tx, "UPDATE AddEmployee SET EMPLOYEETYPE=@t WHERE EmployeeID=@e", "@t", r.EmployeeType == "Project" ? "P" : "R", "@e", record);
            if (current == null || current.PayRate != r.PayRate || current.PayType != r.PayType)
                InsertRate(c, tx, record, r.PayType, r.PayRate, current == null ? r.EffectiveDate : (r.EffectiveDate.Date < DateTime.Today && current.PayRate.HasValue ? DateTime.Today : r.EffectiveDate), existing ? "Changed from business assignment" : "Business assignment added", r.EndDate);
            if (r.ProjectId > 0) X(c, tx, "IF NOT EXISTS(SELECT 1 FROM ASSIGNPROJECT WHERE BUSINESSID=@b AND EMPLOYEEID=@e AND PROJECTID=@p) INSERT ASSIGNPROJECT(BUSINESSID,EMPLOYEEID,PROJECTID) VALUES(@b,@e,@p)", "@b", r.BusinessId, "@e", record, "@p", r.ProjectId);
            SaveDetail(c, tx, r, record);
            Audit(c, tx, r.BusinessId, record, existing ? "EditAssignmentDetail" : "AddAssignmentDetail", null, String.Join(",", r.Roles) + " " + r.PayType + " " + r.PayRate.ToString(CultureInfo.InvariantCulture));
            tx.Commit();
        }
        string status = current == null ? "Active" : current.Status;
        if (r.Status != status) AnyminuteEmployeeRepair.UpdateStatus(actor, 1, r.BusinessId, record, r.Status, DateTime.Today);
    }
    public static void RemoveAssignment(int employee, int business)
    {
        var card = RequireEditable(employee);
        if (!card.Assignments.Any(a => a.BusinessId == business)) throw new InvalidOperationException("That assignment no longer exists.");
        AnyminuteEmployeeRepair.RemoveBusinessAssignment(Actor(), 1, business, RecordFor(card, business));
    }

    // ---------- rates ----------
    static void InsertRate(SqlConnection c, SqlTransaction tx, int record, string type, decimal rate, DateTime effective, string reason, DateTime? end)
    {
        var last = Q(c, tx, "SELECT TOP 1 RateId,effectivedate,JobTitle,IsEmployeeEIExempty,IsEmployeeCPPExempty,RetainVacation FROM RateDetails WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeId=@e ORDER BY effectivedate DESC,RateId DESC", "@e", record);
        if (last.Rows.Count == 1 && last.Rows[0]["effectivedate"] != DBNull.Value && effective.Date < Convert.ToDateTime(last.Rows[0]["effectivedate"]).Date)
            throw new InvalidOperationException("The new rate must start on or after the current rate's effective date. Historical rates are locked.");
        if (last.Rows.Count == 1)
            X(c, tx, "UPDATE RateDetails SET EndDate=@d,Is_Having_EndDate=1 WHERE RateId=@id", "@d", effective.Date.AddDays(-1) < Convert.ToDateTime(last.Rows[0]["effectivedate"] == DBNull.Value ? (object)effective : last.Rows[0]["effectivedate"]).Date ? effective.Date : effective.Date.AddDays(-1), "@id", last.Rows[0]["RateId"]);
        string job = last.Rows.Count == 1 ? S(last.Rows[0]["JobTitle"]) : "", ei = last.Rows.Count == 1 ? S(last.Rows[0]["IsEmployeeEIExempty"]) : "No", cpp = last.Rows.Count == 1 ? S(last.Rows[0]["IsEmployeeCPPExempty"]) : "No", vac = last.Rows.Count == 1 ? S(last.Rows[0]["RetainVacation"]) : "No";
        var id = Q(c, tx, "INSERT RateDetails(EmployeeId,Rate,RateType,StartDate,EndDate,JobTitle,IsEmployeeEIExempty,IsEmployeeCPPExempty,RetainVacation,effectivedate,Is_Having_EndDate) OUTPUT INSERTED.RateId VALUES(@e,@r,@t,@s,@en,@j,@ei,@cpp,@v,@s,@h)",
            "@e", record, "@r", (double)rate, "@t", type, "@s", effective.Date, "@en", end.HasValue ? end.Value.Date : effective.Date, "@j", job, "@ei", ei, "@cpp", cpp, "@v", vac, "@h", end.HasValue);
        if (HasTable(c, tx, "AnyminuteRateChangeReason") && !String.IsNullOrWhiteSpace(reason))
            X(c, tx, "INSERT AnyminuteRateChangeReason(RateId,Reason,ChangedBy,ChangedAtUTC) VALUES(@id,@r,@a,SYSUTCDATETIME())", "@id", id.Rows[0][0], "@r", reason.Trim(), "@a", Actor());
    }
    public static void ChangeRate(int employee, int business, string type, decimal rate, DateTime effective, string reason)
    {
        var card = RequireEditable(employee);
        if (!card.Assignments.Any(a => a.BusinessId == business)) throw new InvalidOperationException("Select one of this employee's businesses.");
        if (!PayTypes.Contains(type)) throw new InvalidOperationException("Select a rate type.");
        if (rate <= 0 || rate > 1000000) throw new InvalidOperationException("Enter a pay rate greater than zero.");
        if (String.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 200) throw new InvalidOperationException("Enter a reason for the change (up to 200 characters).");
        int record = RecordFor(card, business);
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
            InsertRate(c, tx, record, type, rate, effective, reason, null);
            Audit(c, tx, business, record, "ChangePayRate", null, type + " " + rate.ToString(CultureInfo.InvariantCulture) + " from " + effective.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ": " + reason.Trim());
            tx.Commit();
        }
    }
    // v3 (deck slide 10): "Update Current Rate" corrects the current rate row in place; "Add New Rate" uses ChangeRate.
    public static void UpdateCurrentRate(int employee, int business, string type, decimal rate, DateTime effective, string reason)
    {
        var card = RequireEditable(employee);
        if (!card.Assignments.Any(a => a.BusinessId == business)) throw new InvalidOperationException("Select one of this employee's businesses.");
        if (!PayTypes.Contains(type)) throw new InvalidOperationException("Select a rate type.");
        if (rate <= 0 || rate > 1000000) throw new InvalidOperationException("Enter a pay rate greater than zero.");
        if (String.IsNullOrWhiteSpace(reason) || reason.Trim().Length > 200) throw new InvalidOperationException("Enter a reason for the change (up to 200 characters).");
        int record = RecordFor(card, business);
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
            var all = Q(c, tx, "SELECT RateId,effectivedate,Rate,RateType FROM RateDetails WITH(UPDLOCK,HOLDLOCK) WHERE EmployeeId=@e ORDER BY effectivedate DESC,RateId DESC", "@e", record);
            int cur = -1;
            for (int i = 0; i < all.Rows.Count; i++) if (all.Rows[i]["effectivedate"] != DBNull.Value && Convert.ToDateTime(all.Rows[i]["effectivedate"]) <= DateTime.Now) { cur = i; break; }
            if (cur < 0) { InsertRate(c, tx, record, type, rate, effective, reason, null); tx.Commit(); return; }
            if (cur > 0) throw new InvalidOperationException("A future rate is already scheduled, so the current rate cannot be edited. Use Add New Rate instead.");
            var row = all.Rows[cur];
            if (cur + 1 < all.Rows.Count && all.Rows[cur + 1]["effectivedate"] != DBNull.Value && effective.Date <= Convert.ToDateTime(all.Rows[cur + 1]["effectivedate"]).Date)
                throw new InvalidOperationException("The effective date must be after the previous rate's start date. Historical rates are locked.");
            X(c, tx, "UPDATE RateDetails SET Rate=@r,RateType=@t,StartDate=@s,effectivedate=@s WHERE RateId=@id", "@r", (double)rate, "@t", type, "@s", effective.Date, "@id", row["RateId"]);
            if (cur + 1 < all.Rows.Count) X(c, tx, "UPDATE RateDetails SET EndDate=@d,Is_Having_EndDate=1 WHERE RateId=@id", "@d", effective.Date.AddDays(-1), "@id", all.Rows[cur + 1]["RateId"]);
            if (HasTable(c, tx, "AnyminuteRateChangeReason"))
                X(c, tx, "MERGE AnyminuteRateChangeReason t USING(SELECT @id RateId) s ON t.RateId=s.RateId WHEN MATCHED THEN UPDATE SET Reason=@why,ChangedBy=@a,ChangedAtUTC=SYSUTCDATETIME() WHEN NOT MATCHED THEN INSERT(RateId,Reason,ChangedBy,ChangedAtUTC) VALUES(@id,@why,@a,SYSUTCDATETIME());", "@id", row["RateId"], "@why", reason.Trim(), "@a", Actor());
            Audit(c, tx, business, record, "EditCurrentRate", S(row["RateType"]) + " " + S(row["Rate"]), type + " " + rate.ToString(CultureInfo.InvariantCulture) + " from " + effective.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ": " + reason.Trim());
            tx.Commit();
        }
    }
    public static DataTable RateHistory(int record)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool reasons = HasTable(c, null, "AnyminuteRateChangeReason");
            var t = Q(c, null, "SELECT r.RateId,r.RateType,r.Rate,r.effectivedate,r.EndDate,r.Is_Having_EndDate," + (reasons ? "x.Reason" : "CAST(NULL AS nvarchar(200)) Reason") + " FROM RateDetails r " + (reasons ? "LEFT JOIN AnyminuteRateChangeReason x ON x.RateId=r.RateId " : "") + "WHERE r.EmployeeId=@e ORDER BY r.effectivedate DESC,r.RateId DESC", "@e", record);
            t.Columns.Add("No", typeof(int)); t.Columns.Add("Current", typeof(bool)); t.Columns.Add("StartText"); t.Columns.Add("EndText"); t.Columns.Add("RateText");
            bool currentFound = false; int i = 0;
            foreach (DataRow r in t.Rows)
            {
                r["No"] = ++i; DateTime eff = r["effectivedate"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["effectivedate"]);
                bool current = !currentFound && eff <= DateTime.Now; if (current) currentFound = true; r["Current"] = current;
                r["StartText"] = eff == DateTime.MinValue ? "\u2014" : eff.ToString("MMM d, yyyy", CultureInfo.InvariantCulture);
                bool hasEnd = S(r["Is_Having_EndDate"]) == "True" && r["EndDate"] != DBNull.Value;
                r["EndText"] = hasEnd ? Convert.ToDateTime(r["EndDate"]).ToString("MMM d, yyyy", CultureInfo.InvariantCulture) : (current ? "Present" : "\u2014");
                r["RateText"] = "$" + Convert.ToDecimal(r["Rate"]).ToString("N2", CultureInfo.InvariantCulture) + " / " + S(r["RateType"]);
            }
            return t;
        }
    }

    // ---------- login & access ----------
    public sealed class AccessState
    {
        public int UserId; public string Email = "", Username = "", Role = "", State = "No login", SentText = "\u2014", LastChange = "\u2014", LastLogin = "Never";
        public bool Enabled, Pending, RequireChange, TwoFactor, TablesReady;
    }
    public static AccessState Access(AnyminuteEmployeeCardView card)
    {
        var s = new AccessState(); s.UserId = UserFor(card);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            s.TablesReady = HasTable(c, null, "AnyminuteLoginSecurity");
            if (s.UserId == 0) return s;
            var u = Q(c, null, "SELECT Email,uid,UserType,Status,Is_First_Time_Login FROM UserAcc WHERE UserId=@u", "@u", s.UserId);
            if (u.Rows.Count != 1) return s;
            s.Email = S(u.Rows[0]["Email"]); if (s.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) s.Email = "";
            s.Username = S(u.Rows[0]["uid"]); s.Role = String.Join(", ", card.RoleNames);
            s.Pending = S(u.Rows[0]["Is_First_Time_Login"]) == "True"; s.Enabled = true;
            if (s.TablesReady)
            {
                var sec = Q(c, null, "SELECT LoginDisabled,RequirePasswordChange,TwoFactorEnabled FROM AnyminuteLoginSecurity WHERE UserId=@u", "@u", s.UserId);
                if (sec.Rows.Count == 1) { s.Enabled = !Convert.ToBoolean(sec.Rows[0][0]); s.RequireChange = Convert.ToBoolean(sec.Rows[0][1]); s.TwoFactor = Convert.ToBoolean(sec.Rows[0][2]); }
                if (HasColumn(c, null, "AnyminuteLoginSecurity", "LastLoginDate"))
                {
                    var last = Q(c, null, "SELECT LastLoginDate FROM AnyminuteLoginSecurity WHERE UserId=@u AND LastLoginDate IS NOT NULL", "@u", s.UserId);
                    if (last.Rows.Count == 1) s.LastLogin = DateTime.SpecifyKind(Convert.ToDateTime(last.Rows[0][0]), DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd hh:mm tt", CultureInfo.InvariantCulture);
                }
            }
            s.State = !s.Enabled ? "Disabled" : s.Pending ? "Invitation Pending" : "Enabled";
            var sent = Q(c, null, "SELECT MAX(RecordedAtUTC) FROM AnyminuteApprovalAudit WHERE EmployeeID IN(SELECT EmployeeID FROM MappingTable WHERE USERID=@u) AND Action='SendInvitation'", "@u", s.UserId); // v86: adding an employee no longer sends an invitation
            if (sent.Rows[0][0] != DBNull.Value) s.SentText = DateTime.SpecifyKind(Convert.ToDateTime(sent.Rows[0][0]), DateTimeKind.Utc).ToLocalTime().ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture);
        }
        return s;
    }
    public static DataTable AccessHistory(AnyminuteEmployeeCardView card)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var ids = Records(card); var sql = new StringBuilder("SELECT TOP 50 a.RecordedAtUTC,a.Action,a.AfterValue,dbo.FN_DESC('AddEmployee',a.ActorID) ChangedBy FROM AnyminuteApprovalAudit a WHERE a.EmployeeID IN(");
            var args = new List<object>();
            for (int i = 0; i < ids.Length; i++) { sql.Append(i == 0 ? "" : ",").Append("@e" + i); args.Add("@e" + i); args.Add(ids[i]); }
            sql.Append(") AND a.Action IN('" + String.Join("','", AccessActions) + "','EditEmployeeProfile') ORDER BY a.RecordedAtUTC DESC");
            var t = Q(c, null, sql.ToString(), args.ToArray()); t.Columns.Add("When");
            foreach (DataRow r in t.Rows) r["When"] = DateTime.SpecifyKind(Convert.ToDateTime(r["RecordedAtUTC"]), DateTimeKind.Utc).ToLocalTime().ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture);
            return t;
        }
    }
    static void RequireUserOwner(SqlConnection c, SqlTransaction tx, int user)
    {
        foreach (DataRow b in Q(c, tx, "SELECT DISTINCT BusinessID FROM MappingTable WHERE USERID=@u AND BusinessID IS NOT NULL", "@u", user).Rows) AnyminuteApprovalAssignments.RequireOwner(c, tx, Convert.ToInt32(b[0]));
        if (Q(c, tx, "SELECT 1 FROM MappingTable WHERE USERID=@u AND UserTypeId=1", "@u", user).Rows.Count > 0) throw new InvalidOperationException("Subscriber logins cannot be changed here.");
        if (user == Convert.ToInt32(HttpContext.Current.Session["UserId"])) throw new InvalidOperationException("You cannot change your own login access here.");
    }
    // Returns false when the security table is missing (nothing stored).
    static bool SetSecurity(int user, bool? disabled, bool? requireChange, bool? twoFactor, bool cancelInvite, int employee, int business)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            RequireUserOwner(c, tx, user);
            if (!HasTable(c, tx, "AnyminuteLoginSecurity")) return false;
            X(c, tx, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u) INSERT AnyminuteLoginSecurity(UserId,LoginDisabled,RequirePasswordChange,TwoFactorEnabled,UpdatedAtUTC,UpdatedBy) VALUES(@u,0,0,0,SYSUTCDATETIME(),@a);
                UPDATE AnyminuteLoginSecurity SET LoginDisabled=COALESCE(@d,LoginDisabled),RequirePasswordChange=COALESCE(@r,RequirePasswordChange),TwoFactorEnabled=COALESCE(@t,TwoFactorEnabled),
                InvitationCancelledAtUTC=CASE WHEN @c=1 THEN SYSUTCDATETIME() WHEN @d=0 THEN NULL ELSE InvitationCancelledAtUTC END,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE UserId=@u",
                "@u", user, "@a", Actor(), "@d", disabled.HasValue ? (object)disabled.Value : DBNull.Value, "@r", requireChange.HasValue ? (object)requireChange.Value : DBNull.Value,
                "@t", twoFactor.HasValue ? (object)twoFactor.Value : DBNull.Value, "@c", cancelInvite ? 1 : 0);
            // "Require password change at next login" uses the existing first-login reset flow.
            if (requireChange == true) X(c, tx, "UPDATE UserAcc SET Is_First_Time_Login=1 WHERE UserId=@u", "@u", user);
            tx.Commit();
        }
        return true;
    }
    public static string SaveAccess(int employee, bool enabled, bool requireChange, bool twoFactor)
    {
        var card = RequireEditable(employee); int user = UserFor(card);
        if (user == 0) throw new InvalidOperationException("This employee has no single login account to manage.");
        var before = Access(card);
        if (!SetSecurity(user, !enabled, requireChange, twoFactor, false, employee, card.Assignments[0].BusinessId)) return MissingTables;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var a = card.First;
            if (before.Enabled != enabled) Audit(c, null, a.BusinessId, a.EmployeeId, enabled ? "LoginEnabled" : "LoginDisabled", null, null);
            if (requireChange && !before.RequireChange) Audit(c, null, a.BusinessId, a.EmployeeId, "RequirePasswordChange", null, "Reset at next login");
            if (before.TwoFactor != twoFactor) Audit(c, null, a.BusinessId, a.EmployeeId, "TwoFactorChanged", null, twoFactor ? "On (stored setting)" : "Off");
        }
        return "Access settings saved.";
    }
    public static string CancelInvitation(int employee)
    {
        var card = RequireEditable(employee); int user = UserFor(card);
        if (user == 0) throw new InvalidOperationException("This employee has no single login account to manage.");
        if (!SetSecurity(user, true, null, null, true, employee, card.First.BusinessId)) return MissingTables;
        using (var c = AnyminuteTimesheetRepair.Open()) Audit(c, null, card.First.BusinessId, card.First.EmployeeId, "CancelInvitation", null, "Login disabled");
        return "Invitation cancelled. Login access is now disabled.";
    }
    public static string InvitationLink(int employee)
    {
        var card = RequireEditable(employee);
        using (var c = AnyminuteTimesheetRepair.Open()) Audit(c, null, card.First.BusinessId, card.First.EmployeeId, "CopyInvitationLink", null, null);
        return ConfigurationManager.AppSettings["ResetUrl"] + LinkEmployee(card);
    }
    // The legacy reset link uses the EmployeeID of the first record that owns the login email.
    static int LinkEmployee(AnyminuteEmployeeCardView card)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var t = Q(c, null, "SELECT TOP 1 ae.EmployeeID FROM EmpAddress a JOIN AddEmployee ae ON a.EmpAddressId=ae.ADDRESSID WHERE a.MainEmail=@m ORDER BY ae.EmployeeID", "@m", card.Email);
            return t.Rows.Count == 1 ? Convert.ToInt32(t.Rows[0][0]) : card.EmployeeId;
        }
    }
    // v86: Login & Access "Send Invitation": grants login access (clears the "disabled" flag set when the employee was added)
    // and sends the existing invitation email. Password reset (SendInvitation(employee,false)) does not change access.
    public static string GrantAccessAndInvite(int employee, bool welcome)
    {
        var card = RequireEditable(employee); int user = UserFor(card);
        if (user == 0) throw new InvalidOperationException("This employee has no single login account to manage.");
        if (String.IsNullOrWhiteSpace(card.Email) || card.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Add an email address before sending an invitation.");
        var before = Access(card); string granted = "";
        if (before.TablesReady && !before.Enabled && SetSecurity(user, false, null, null, false, employee, card.First.BusinessId))
        {
            using (var c = AnyminuteTimesheetRepair.Open()) Audit(c, null, card.First.BusinessId, card.First.EmployeeId, "LoginEnabled", null, "Access granted (invitation)");
            granted = "Login access granted. ";
        }
        return granted + SendInvitation(employee, welcome);
    }
    // Sent only when an owner explicitly asks; honours the existing IsSendMail switch and template.
    public static string SendInvitation(int employee, bool welcome)
    {
        var card = RequireEditable(employee);
        if (String.IsNullOrWhiteSpace(card.Email) || card.Email.EndsWith("blank", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Add an email address before sending an invitation.");
        bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
        if (!send) return "Email sending is turned off for this site (IsSendMail), so no invitation was sent.";
        int user = UserFor(card);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (user > 0) RequireUserOwner(c, null, user);
            var tpl = Q(c, null, "SELECT TOP 1 * FROM TemplateMaster WHERE TempType=1");
            if (tpl.Rows.Count == 0) return "The invitation email template (TemplateMaster TempType 1) is missing, so no invitation was sent.";
            int linkEmployee = LinkEmployee(card);
            var person = Q(c, null, "SELECT TOP 1 ae.FirstName,a.MainEmail FROM EmpAddress a JOIN AddEmployee ae ON a.EmpAddressId=ae.ADDRESSID WHERE ae.EmployeeID=@e", "@e", linkEmployee);
            var acc = Q(c, null, "SELECT convert(nvarchar(max),DecryptByPassPhrase('MD2',convert(varbinary(200),Password,2),2)) Password,uid FROM UserAcc WHERE Email=@m", "@m", card.Email);
            if (person.Rows.Count == 0 || acc.Rows.Count == 0) throw new InvalidOperationException("The login account for this email was not found.");
            string url = ConfigurationManager.AppSettings["ResetUrl"] + linkEmployee;
            var mail = new EmailSettingController();
            string body = mail.PopulateBodyWithUrl(S(person.Rows[0]["FirstName"]), welcome ? "Welcome" : "Reset Password", url, S(tpl.Rows[0]["description"]), S(tpl.Rows[0]["TempName"]), card.Email + " <br />User Id - " + S(acc.Rows[0]["uid"]), S(acc.Rows[0]["Password"]));
            mail.SendHtmlFormattedEmail(card.Email, S(tpl.Rows[0]["Subject"]), body);
            Audit(c, null, card.First.BusinessId, card.First.EmployeeId, "SendInvitation", null, card.Email);
        }
        return "Invitation email sent to " + card.Email + ".";
    }
    // Used by Login.aspx: true only when an owner disabled this login in the new security table.
    public static bool LoginDisabled(string userId)
    {
        int user; if (!Int32.TryParse(userId, out user)) return false;
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
                return HasTable(c, null, "AnyminuteLoginSecurity") && Q(c, null, "SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u AND LoginDisabled=1", "@u", user).Rows.Count > 0;
        }
        catch (SqlException) { return false; }
    }
    // v7: remembers the last successful sign-in (DB_REQUIRED/REQUIRED_02_LastLogin.sql). Never throws: login must not depend on it.
    public static void RecordLogin(string userId)
    {
        int user; if (!Int32.TryParse(userId, out user) || user <= 0) return;
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
            {
                if (!HasColumn(c, null, "AnyminuteLoginSecurity", "LastLoginDate")) return;
                X(c, null, @"UPDATE AnyminuteLoginSecurity SET LastLoginDate=SYSUTCDATETIME() WHERE UserId=@u;
                    IF @@ROWCOUNT=0 INSERT AnyminuteLoginSecurity(UserId,LastLoginDate) VALUES(@u,SYSUTCDATETIME());", "@u", user);
            }
        }
        catch (Exception) { }
    }
    public static string RoleCss(object role)
    {
        string r = Convert.ToString(role).ToLowerInvariant();
        if (r.Contains("admin")) return "am-role-admin"; if (r.Contains("manager") || r.Contains("supervisor")) return "am-role-manager";
        if (r.Contains("accountant")) return "am-role-accountant"; if (r.Contains("employee") || r.Contains("staff")) return "am-role-employee"; return "am-role-other";
    }
    public static string StatusPill(object status) { string s = Convert.ToString(status); return s == "Active" ? "am-pill-green" : s == "On Leave" ? "am-pill-amber" : s == "Terminated" ? "am-pill-red" : "am-pill-grey"; }
    public static DataTable Projects(int business)
    {
        using (var c = AnyminuteTimesheetRepair.Open()) return Q(c, null, "SELECT ProjectID,ProjectName FROM AddProject WHERE BusinessID=@b AND (Status IS NULL OR Status<>'0') ORDER BY ProjectName", "@b", business);
    }
    public static DataTable Roles(int business)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
            return Q(c, null, "SELECT User_TypeId,UserType FROM UserType u WHERE u.User_TypeId IN(2,4,5,12) OR (u.User_TypeId NOT IN(1,2,3,4,5,6,12) AND EXISTS(SELECT 1 FROM AssginBussiness o WHERE o.BusinessID=@b AND o.UserTypeId=1 AND o.EmployeeID=u.BownerID)) ORDER BY CASE User_TypeId WHEN 2 THEN 0 ELSE 1 END,UserType", "@b", business);
    }
}
