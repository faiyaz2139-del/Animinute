using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
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
        public string StreetNo, Line2, SuiteType, SuiteNo; // v46 Address module (Street Number, Address Line 2, Suite Type / Number)
        public int Businesses, Employees; public DateTime Start, NextBilling; public bool Invite;
        public string TempPassword; // v45: set by Create (shown once on screen, sent in the welcome email; never stored in clear)
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
                Postalcode = x.Postal ?? "", NumberSuffix = "", StreetName = x.Street ?? "", StreetType = x.Line2 ?? "", StreetDirection = "", SuitType = x.SuiteType ?? "", SuiteNumber = x.SuiteNo ?? "", SuitSuffix = "",
                City = x.City ?? "", Province = x.Province ?? "", MainPhone = x.Phone, WorkPhone = "", Mobile = x.Mobile ?? "", Fax = "", MainEmail = x.Email, CCEmail = "",
                Website = "", Other1 = "", StreetNo = x.StreetNo ?? "", MotherOrgId = MotherOrg
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
            // v45: the legacy SP gives every new login the fixed password '12345678' and the welcome email had no password at all.
            // Each Support-created subscriber now gets its own temporary password (same generator as the v44 reseller fix).
            x.TempPassword = SetTempPassword(c, user, "Temporary password issued with the new subscriber account.");
            bool audit = SA.Has(c, "AnyminuteSupportAudit");
            if (x.Status == "Active" || (x.Status == "Pending" && !audit)) SA.X(c, null, "UPDATE UserAcc SET Status=1,Is_First_Time_Login=0 WHERE UserId=@u", "@u", user);
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
    // v45: writes a new temporary password to UserAcc.Password in the SP_LOGIN format (EncryptByPassPhrase 'MD2') and returns it.
    // The audit row ("Temporary Password Issued", no password in it) is what lets that password activate a Pending account at sign-in.
    public static string SetTempPassword(SqlConnection c, int user, string note)
    {
        string pw = AnyminuteResellers.NewTempPassword();
        SA.X(c, null, "UPDATE UserAcc SET Password=CONVERT(nvarchar(max),EncryptByPassPhrase('MD2',CAST(@p AS nvarchar(50))),2) WHERE UserId=@u", "@p", pw, "@u", user);
        if (SA.Has(c, "AnyminuteSupportAudit")) SA.Audit(c, null, "Login", user.ToString(CultureInfo.InvariantCulture), "Temporary Password Issued", null, null, null, null, note);
        return pw;
    }
    // v45 (Login.aspx password sign-in only): a Pending Activation subscriber who signs in with the temporary password Support emailed
    // is activated (Is_First_Time_Login cleared) instead of being sent back to the reset link. Only accounts with a
    // "Temporary Password Issued" audit row qualify. Never throws; false = leave the legacy first-time check as it was.
    public static bool ActivateOnTempPassword(DataTable dt)
    {
        try
        {
            if (dt == null || dt.Rows.Count == 0 || dt.Rows[0]["Is_First_Time_Login"].ToString() != "True") return false;
            int user; if (!Int32.TryParse(dt.Rows[0]["UserId"].ToString(), out user) || user <= 0) return false;
            using (var c = SA.Open())
            {
                if (!SA.Has(c, "AnyminuteSupportAudit")) return false;
                if (SA.Q(c, "SELECT COUNT(*) FROM AnyminuteSupportAudit WHERE Entity='Login' AND EntityKey=@k AND Action='Temporary Password Issued'", "@k", user.ToString(CultureInfo.InvariantCulture)).Rows[0][0].ToString() == "0") return false;
                SA.X(c, null, "UPDATE UserAcc SET Is_First_Time_Login=0 WHERE UserId=@u", "@u", user);
                SA.Audit(c, null, "Login", user.ToString(CultureInfo.InvariantCulture), "Account Activated", "Pending", "Active", null, null, "First sign-in with the emailed temporary password.");
            }
            var col = dt.Columns["Is_First_Time_Login"]; dt.Rows[0]["Is_First_Time_Login"] = col.DataType == typeof(bool) ? (object)false : "False";
            return true;
        }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("Temporary password activation failed", ex); return false; }
    }
    // v58 (Support Admin PPT s5/s6): the invitation / reset email always carries a real temporary password (no more
    // "(your current password; use Forgot Password...)" placeholder) plus the Welcome link (ResetUrl_New_Account + EmployeeId),
    // whose page accepts exactly that temporary password and then sets the new one. Own subject + body (TemplateMaster type 3 is
    // subject "Reset Password", which was confusing for an invitation). Returns null when sent, otherwise why it was not sent.
    public static string SendInvitation(SqlConnection c, int user, string password) { return SendCredentials(c, user, password, false, null); }
    public static string SendPasswordReset(SqlConnection c, int user, string password, string to) { return SendCredentials(c, user, password, true, to); }
    static string SendCredentials(SqlConnection c, int user, string password, bool reset, string to)
    {
        if (String.IsNullOrEmpty(password)) return "No temporary password was issued, so nothing was sent.";
        var r = SA.Q(c, "SELECT u.Email,u.uid,u.EmployeeId,ae.FirstName FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId WHERE u.UserId=@u", "@u", user);
        if (r.Rows.Count != 1) return "Subscriber not found.";
        string login = SA.S(r.Rows[0]["Email"]), email = String.IsNullOrEmpty(to) ? login : to.Trim(), first = SA.S(r.Rows[0]["FirstName"]);
        bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
        if (!send) return "Email sending is turned off for this site (IsSendMail), so nothing was sent.";
        string host = ConfigurationManager.AppSettings["ResetUrl_New_Account"] ?? "";
        string welcome = host.Length > 0 ? host + SA.S(r.Rows[0]["EmployeeId"]) : "";
        int cut = host.IndexOf("ResetPassord", StringComparison.OrdinalIgnoreCase);
        string signIn = cut > 0 ? host.Substring(0, cut) + "Login.aspx" : "https://anyminute.net/Login.aspx";
        Func<string, string> H = HttpUtility.HtmlEncode; Func<string, string> A = HttpUtility.HtmlAttributeEncode;
        string subject = reset ? "Your AnyMinute Password Has Been Reset" : "Welcome to AnyMinute - Your Account Invitation";
        var b = new StringBuilder("<div style=\"font-family:Arial,sans-serif;font-size:14px;color:#13234b;line-height:1.5\">");
        b.Append("<p>Hello <b>").Append(H(first.Length > 0 ? first : login)).Append("</b>,</p>");
        b.Append(reset ? "<p>AnyMinute Support has reset the password for your AnyMinute account. Your previous password no longer works.</p>"
                       : "<p>You have been invited to AnyMinute.net, the smart way to log your hours and minutes from anywhere, at any time.</p>");
        b.Append("<table style=\"border-collapse:collapse;margin:8px 0 12px\"><tr><td style=\"padding:4px 12px 4px 0\">Login email</td><td><b>").Append(H(login)).Append("</b></td></tr>")
         .Append("<tr><td style=\"padding:4px 12px 4px 0\">User ID</td><td><b>").Append(H(SA.S(r.Rows[0]["uid"]))).Append("</b></td></tr>")
         .Append("<tr><td style=\"padding:4px 12px 4px 0\">Temporary password</td><td><b style=\"font-family:Consolas,monospace;font-size:15px;background:#eef5ff;padding:2px 6px;border-radius:4px\">").Append(H(password)).Append("</b></td></tr></table>");
        if (welcome.Length > 0)
            b.Append("<p>Click <b>").Append(reset ? "Set New Password" : "Welcome").Append("</b> below, enter the temporary password above, then choose your own password (at least 8 characters).</p>")
             .Append("<p><a href=\"").Append(A(welcome)).Append("\" style=\"display:inline-block;background:#1366e8;color:#fff;text-decoration:none;padding:9px 18px;border-radius:6px;font-weight:bold\">").Append(reset ? "Set New Password" : "Welcome").Append("</a></p>");
        b.Append("<p>You can also sign in at <a href=\"").Append(A(signIn)).Append("\">").Append(H(signIn)).Append("</a> with your login email and the temporary password, then change it from <b>Change password</b>.</p>")
         .Append("<p>").Append(reset ? "If you did not ask for this reset, please contact AnyMinute Support." : "If you were not expecting this invitation, you can ignore this email.").Append("</p><p>Thank you,<br />AnyMinute's Support Team</p></div>");
        new EmailSettingController().SendHtmlFormattedEmail(email, subject, b.ToString());
        if (SA.Has(c, "AnyminuteSupportAudit"))
            SA.Audit(c, null, "Login", user.ToString(CultureInfo.InvariantCulture), reset ? "Password Reset" : "Activation Email", null, null, null, email,
                reset ? "Password reset by Support: new temporary password emailed (Welcome link to set a new password)." : "Activation invitation sent with a new temporary password.");
        return null;
    }
}
