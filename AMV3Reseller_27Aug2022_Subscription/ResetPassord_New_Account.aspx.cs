using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ResetPassord_New_Account : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    UserAccController UActrl = new UserAccController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["empid"] != null)
            {
                if (Request.QueryString["LoginType"] == null)
                {
                    string name;
                    name = comm.GetNameFromId("AddEmployee", int.Parse(Request.QueryString["empid"].ToString()));
                    lblname.Text = name;
                    Label1.Text = name;
                }
                else
                {
                    if (Request.QueryString["LoginType"].ToString() == "Acc")
                    {
                        DataTable name;
                        name = comm.GetAllDataFromTable("MotherOrg", "MotherOrgName,ContactName", "MotherOrgID='" + int.Parse(Request.QueryString["empid"].ToString()) + "'", "");
                        //lblname.Text = name.Rows[0]["MotherOrgName"].ToString();
                        lblname.Text = name.Rows[0]["ContactName"].ToString();
                        Label1.Text = name.Rows[0]["ContactName"].ToString();
                    }
                    if (Request.QueryString["LoginType"].ToString() == "Support")
                    {
                        DataTable name;
                        name = comm.GetAllDataFromTable("SupportAdmin", "Title+' '+FirstName+' '+MiddleName+' '+LastName as name", "SupportAdminId='" + int.Parse(Request.QueryString["empid"].ToString()) + "'", "");
                        lblname.Text = name.Rows[0]["name"].ToString();
                        Label1.Text = name.Rows[0]["name"].ToString();
                    }
                }
            }
        }
    }

    // v58 (Support Admin PPT s5/s6 "If I press Welcome, Don't know how to get Temp PW"): invitation and Support password-reset
    // emails now carry the temporary password. This page checks it with a parameterised query (the old check concatenated the
    // typed text into SQL), remembers the matched UserAcc row, and the new password is written to that one row only
    // (Updatepass 'EMP' updated every UserAcc row with the same EmployeeId, which can include a SupportAdmin row). First-time
    // login is cleared so the subscriber can sign in straight away.
    const string Decrypt = "CONVERT(nvarchar(max),DecryptByPassPhrase('MD2',CONVERT(varbinary(200),Password,2),2))";
    string LoginType { get { return Request.QueryString["LoginType"] ?? "EMP"; } }
    int VerifiedUser { get { return ViewState["am_vu"] == null ? 0 : (int)ViewState["am_vu"]; } set { ViewState["am_vu"] = value; } }
    void Alert(string title, string text, string icon) { ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SweetAlert", "swal(" + HttpUtility.JavaScriptStringEncode(title, true) + ", " + HttpUtility.JavaScriptStringEncode(text, true) + ", '" + icon + "');", true); }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        int user = VerifiedUser;
        if (user <= 0) { LoginPanel.Visible = false; OldPasswordPanel.Visible = true; Alert("warning!", "Please enter the temporary password from your email first.", "error"); return; }
        string pw = txtNewPass.Text.Trim();
        if (pw != txtConfirmPass.Text.Trim()) { Alert("warning!", "Confirm password does not match with password", "error"); return; }
        if (pw.Length < 8) { Alert("warning!", "Password Must Contain atleast 8 characters", "error"); return; }
        if (pw.Length > 50) { Alert("warning!", "Password must be 50 characters or fewer", "error"); return; }
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            AnyminuteTimesheetRepair.Execute(c, null, "UPDATE UserAcc SET Password=CONVERT(nvarchar(max),EncryptByPassPhrase('MD2',CAST(@p AS nvarchar(50))),2),Is_First_Time_Login='False' WHERE UserId=@u", "@p", pw, "@u", user);
            if (AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteSupportAudit"))
                AnyminuteTimesheetRepair.Execute(c, null, @"INSERT AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,Comments,IpAddress,Success)
                    VALUES('Login',@k,'Password Changed',@u,'Subscriber','New password set on the Welcome page with the emailed temporary password.',@ip,1)",
                    "@k", user.ToString(System.Globalization.CultureInfo.InvariantCulture), "@u", user, "@ip", Request.UserHostAddress ?? "");
        }
        VerifiedUser = 0;
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Password Changed Successfully. Please sign in with your new password.', 'success').then((value) => {window.location = 'Login.aspx';});", true);
    }

    protected void Submit_Click(object sender, EventArgs e)
    {
        string temp = txtOldPassword.Text.Trim();
        int id;
        if (temp.Length == 0) { Alert("warning!", "Enter the temporary password from your email", "error"); return; }
        if (!Int32.TryParse(Request.QueryString["empid"], out id)) { Alert("warning!", "This link is not valid. Please use the link from your latest AnyMinute email.", "error"); return; }
        string where = LoginType == "Acc" ? "MotherOrgid=@e" : LoginType == "Support" ? "EmployeeId=@e AND UserType='SupportAdmin'" : "EmployeeId=@e AND ISNULL(UserType,'')<>'SupportAdmin'";
        DataTable match;
        using (var c = AnyminuteTimesheetRepair.Open())
            match = AnyminuteTimesheetRepair.Query(c, null, "SELECT TOP 1 UserId FROM UserAcc WHERE " + where + " AND " + Decrypt + "=@p ORDER BY UserId DESC", "@e", id, "@p", temp);
        if (match.Rows.Count == 0)
        {
            Alert("warning!", "The temporary password does not match. Use the temporary password from your most recent AnyMinute invitation or password reset email (each new email replaces the previous password).", "error");
            return;
        }
        VerifiedUser = Convert.ToInt32(match.Rows[0][0]);
        LoginPanel.Visible = true;
        OldPasswordPanel.Visible = false;
    }
}