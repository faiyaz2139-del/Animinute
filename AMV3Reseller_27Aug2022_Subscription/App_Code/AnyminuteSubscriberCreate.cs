using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-11-01 v43 (PPT 01 "20261027A" s5 Add Subscriber): Support Admin creates a subscriber (business owner) account.
// Uses the SAME stored procedures and row set as self-registration (Register_As_A_Bo.aspx: Sp_InsertEmpPersonalDetails,
// Sp_InsertEmpAddress, Sp_InsertUserAccFromAddress, AssginBussiness owner row, Sp_InsertMapping, PlanHistory), with
// MotherOrgID 4 and EMPLOYEETYPE 'NEW_AC_BO' so the account behaves exactly like a registered subscriber. No payment is taken:
// PlanHistory.AmountPaid = 0, TransId = 'SUPPORT'. One login per email: an email already on UserAcc is refused (v41 rule).
// Status: Pending Activation = first-time login pending; Active = first-time flag cleared; Suspended = login disabled
// (AnyminuteLoginSecurity); Inactive = UserAcc.Status 0. The activation invitation uses the existing TemplateMaster mail.
public static class AnyminuteSubscriberCreate
{
    public const int MotherOrg = 4; // default subscriber mother org (same as Register_As_A_Bo)
    public sealed class Input
    {
        public string First, Middle, Last, Email, Phone, Mobile, PlanName, Status, Street, City, Province, Postal, Country;
        public int Businesses, Employees; public DateTime Start, NextBilling; public bool Invite;
    }
    public static bool EmailTaken(SqlConnection c, string email)
    {
        return SA.Q(c, "SELECT COUNT(*) FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email)))=LOWER(@m)", "@m", (email ?? "").Trim()).Rows[0][0].ToString() != "0";
    }
    static string PlanCode(string planName)
    {
        string k = SA.PlanKey(planName).ToLowerInvariant().Replace(" ", "_");
        return "prod_" + (k.Length == 0 ? "support" : k);
    }
    // Returns the new UserAcc.UserId. Throws on a database failure (the caller reports it).
    public static int Create(Input x)
    {
        string name = (x.First + " " + x.Last).Trim();
        var emp = new AddEmployeeEntityClass
        {
            UserType = 0, Title = "", Occupation = "", LegalName = "", FirstName = x.First, MiddleName = x.Middle ?? "", LastName = x.Last,
            SocialInsuranceNumber = "", DateOfBirth = "", Gender = "", HireDate = DateTime.Today, TerminationDate = DateTime.Today,
            MotherOrgID = MotherOrg, IsActive = "YES", MobilePhone = x.Mobile ?? "", Apartment = "", City = x.City ?? "", Province = x.Province ?? ""
        };
        var rate = new RateDetailsEntity { IsEmployeeEIExempty = "NO", IsEmployeeCPPExempty = "NO", RetainVacation = "NO" };
        int empId = new AddEmployeeControlerClass().InsertPersonalDetails(emp, "1", rate);
        if (empId <= 0) throw new InvalidOperationException("The subscriber profile could not be created.");
        using (var c = SA.Open())
        {
            SA.X(c, null, "UPDATE AddEmployee SET EMPLOYEETYPE='NEW_AC_BO',UserType=1 WHERE EmployeeID=@e", "@e", empId);
            var addr = new EmpAddressEntity
            {
                Postalcode = x.Postal ?? "", NumberSuffix = "", StreetName = x.Street ?? "", StreetType = "", StreetDirection = "", SuitType = "", SuiteNumber = "", SuitSuffix = "",
                City = x.City ?? "", Province = x.Province ?? "", MainPhone = x.Phone, WorkPhone = "", Mobile = x.Mobile ?? "", Fax = "", MainEmail = x.Email, CCEmail = "",
                Website = "", Other1 = "", StreetNo = "", MotherOrgId = MotherOrg
            };
            if (new EmpAddressController().InsertEmpAddress(addr, empId) <= 0) throw new InvalidOperationException("The subscriber contact details could not be saved (employee " + empId + ").");
            if (!String.IsNullOrEmpty(x.Country) && AnyminuteEmployeeEditor.HasColumn(c, null, "EmpAddress", "Country"))
                SA.X(c, null, "UPDATE ea SET Country=@k FROM EmpAddress ea JOIN AddEmployee ae ON ae.ADDRESSID=ea.EmpAddressId WHERE ae.EmployeeID=@e", "@k", x.Country, "@e", empId);
            var uac = new UserAccController();
            var ue = new UserAccEntity { Email = x.Email, EmployeeId = empId, MotherOrgid = MotherOrg, UserType = "BUSINESS OWNER", UserTypeid = 1, ProfileImageName = "" };
            int user = Int32.Parse(uac.Sp_InsertUserAccFromAddress(ue), CultureInfo.InvariantCulture);
            SA.X(c, null, "INSERT INTO AssginBussiness(EmployeeID,EmployeeName,UserTypeId) VALUES(@e,@n,1)", "@e", empId, "@n", x.First);
            ue.UserId = user; ue.BusinessID = 0; uac.Sp_InsertMapping(ue);
            string code = PlanCode(x.PlanName);
            SA.X(c, null, @"INSERT INTO PlanHistory(PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive)
                VALUES(@pid,@pr,@pn,@sd,@ed,'0','SUPPORT',@e,@m,@u,@b,'1')", "@pid", code, "@pr", code + "_priceid", "@pn", x.PlanName.ToUpperInvariant(),
                "@sd", x.Start, "@ed", x.NextBilling, "@e", empId, "@m", x.Email, "@u", x.Employees.ToString(CultureInfo.InvariantCulture), "@b", x.Businesses.ToString(CultureInfo.InvariantCulture));
            if (x.Status == "Active") SA.X(c, null, "UPDATE UserAcc SET Status=1,Is_First_Time_Login=0 WHERE UserId=@u", "@u", user);
            else if (x.Status == "Inactive") SA.X(c, null, "UPDATE UserAcc SET Status=0 WHERE UserId=@u", "@u", user);
            else SA.X(c, null, "UPDATE UserAcc SET Status=1,Is_First_Time_Login=1 WHERE UserId=@u", "@u", user);
            if (x.Status == "Suspended" && SA.Has(c, "AnyminuteLoginSecurity"))
                SA.X(c, null, @"IF NOT EXISTS(SELECT 1 FROM AnyminuteLoginSecurity WHERE UserId=@u) INSERT AnyminuteLoginSecurity(UserId,LoginDisabled,RequirePasswordChange,TwoFactorEnabled,UpdatedAtUTC,UpdatedBy) VALUES(@u,1,0,0,SYSUTCDATETIME(),@a)
                    ELSE UPDATE AnyminuteLoginSecurity SET LoginDisabled=1,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE UserId=@u", "@u", user, "@a", SA.ActorId());
            if (SA.Has(c, "AnyminuteSupportAudit"))
                SA.Audit(c, null, "Login", user.ToString(CultureInfo.InvariantCulture), "Subscriber Created", null, x.Status, null,
                    x.PlanName + " (" + x.Businesses + " businesses, " + x.Employees + " employees)", "New subscriber " + name + " created by Support Admin.");
            return user;
        }
    }
    // Activation invitation (existing TemplateMaster mail). Returns null when sent, otherwise the reason it was not sent.
    public static string SendInvitation(SqlConnection c, int user)
    {
        var r = SA.Q(c, "SELECT u.Email,u.uid,u.EmployeeId,ae.FirstName FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId WHERE u.UserId=@u", "@u", user);
        if (r.Rows.Count != 1) return "Subscriber not found.";
        string email = SA.S(r.Rows[0]["Email"]);
        bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
        if (!send) return "Email sending is turned off for this site (IsSendMail), so no invitation was sent.";
        var tpl = SA.Q(c, "SELECT TOP 1 * FROM TemplateMaster WHERE TempType=3");
        string host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        if (tpl.Rows.Count == 0 || String.IsNullOrEmpty(host)) { tpl = SA.Q(c, "SELECT TOP 1 * FROM TemplateMaster WHERE TempType=1"); host = ConfigurationManager.AppSettings["ResetUrl"]; }
        if (tpl.Rows.Count == 0) return "The invitation email template (TemplateMaster) is missing, so no invitation was sent.";
        var mail = new EmailSettingController();
        string body = mail.PopulateBodyWithUrl(HttpUtility.HtmlEncode(SA.S(r.Rows[0]["FirstName"])), "Welcome", host + SA.S(r.Rows[0]["EmployeeId"]), SA.S(tpl.Rows[0]["description"]), SA.S(tpl.Rows[0]["TempName"]), email + " <br />User Id - " + SA.S(r.Rows[0]["uid"]), "");
        mail.SendHtmlFormattedEmail(email, SA.S(tpl.Rows[0]["Subject"]), body);
        if (SA.Has(c, "AnyminuteSupportAudit")) SA.Audit(c, null, "Login", user.ToString(CultureInfo.InvariantCulture), "Activation Email", null, null, null, email, "Activation invitation sent.");
        return null;
    }
}
