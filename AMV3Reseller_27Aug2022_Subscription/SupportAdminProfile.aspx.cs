using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-11-03 v58 (Support Admin PPT s2/s3): editable Support Admin profile on Modern.master. Reads SupportAdmin by the signed-in
// SupportAdminId (Session["EmployeeId"]) with a parameterised query and saves through the existing Sp_UpdateSupport. The login
// email is read-only here (v39: changes go through Login & security verification) and is passed back unchanged.
public partial class SupportAdminProfile : System.Web.UI.Page
{
    readonly SupportAdminCtrl SuppCtrl = new SupportAdminCtrl();
    int Me { get { int id; Int32.TryParse(Convert.ToString(Session["EmployeeId"]), out id); return id; } }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["UserId"] == null || Session["EmployeeId"] == null) { Response.Redirect("Login.aspx", true); return; }
        SA.Require();
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) Bind();
    }

    DataRow Row()
    {
        using (var c = SA.Open())
        {
            var t = SA.Q(c, "SELECT TOP 1 * FROM SupportAdmin WHERE SupportAdminId=@s", "@s", Me);
            return t.Rows.Count == 1 ? t.Rows[0] : null;
        }
    }

    static void Pick(System.Web.UI.WebControls.DropDownList d, string v)
    {
        var item = d.Items.FindByValue(v ?? "");
        if (item == null && !String.IsNullOrEmpty(v)) { item = new System.Web.UI.WebControls.ListItem(v, v); d.Items.Add(item); }
        d.ClearSelection(); if (item != null) item.Selected = true;
    }

    void Bind()
    {
        var r = Row();
        if (r == null) { ProfileForm.Visible = false; Flash.Text = "<div class='sa-flash sa-warn' role='alert'>No Support Admin profile was found for this sign-in.</div>"; return; }
        Pick(ddlLegalName, SA.S(r["Title"]));
        txtFirstName.Text = SA.S(r["FirstName"]); txtMiddleName.Text = SA.S(r["MiddleName"]); txtLastName.Text = SA.S(r["LastName"]);
        DateTime dob; txtdob.Text = r["DOB"] != DBNull.Value && DateTime.TryParse(SA.S(r["DOB"]), out dob) ? dob.ToString("yyyy-MM-dd") : "";
        Pick(ddlgender, SA.S(r["Gender"]));
        txtMainPhone.Text = SA.S(r["Contact"]); txt_mobilenumber.Text = SA.S(r["Mobile"]);
        txt_city.Text = SA.S(r["CITY"]); txt_province.Text = SA.S(r["Province"]);
        txtMainEmail.Text = SA.S(r["Email"]);
    }

    static readonly Regex Phone = new Regex(@"^[0-9+()\-\s.]{7,15}$");

    protected void btnsave_Click(object sender, EventArgs e)
    {
        var r = Row();
        if (r == null) { Bind(); return; }
        string first = txtFirstName.Text.Trim(), last = txtLastName.Text.Trim(), phone = txtMainPhone.Text.Trim(), mobile = txt_mobilenumber.Text.Trim(), dob = txtdob.Text.Trim();
        string err = first.Length == 0 ? "First Name is required." : last.Length == 0 ? "Last Name is required." : phone.Length == 0 ? "Main Phone is required."
            : !Phone.IsMatch(phone) ? "Enter a valid Main Phone number." : mobile.Length > 0 && !Phone.IsMatch(mobile) ? "Enter a valid Mobile number." : null;
        DateTime d;
        if (err == null && dob.Length > 0 && (!DateTime.TryParseExact(dob, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d) || d > DateTime.Today || d.Year < 1900)) err = "Enter a valid Date of Birth.";
        if (err != null) { Flash.Text = "<div class='sa-flash sa-warn' role='alert'>" + HttpUtility.HtmlEncode(err) + "</div>"; return; }

        var x = new SupportAdminEntity
        {
            SupportAdminId = Me, Title = ddlLegalName.SelectedValue, FirstName = first, MiddleName = txtMiddleName.Text.Trim(), LastName = last, DOB = dob,
            Gender = ddlgender.SelectedValue, Contact = phone, Mobile = mobile, CITY = txt_city.Text.Trim(), Province = txt_province.Text.Trim(),
            Email = SA.S(r["Email"]) // login email unchanged here
        };
        SuppCtrl.Sp_UpdateSupport(x);
        string name = AnyminuteDisplayText.Tidy(first + " " + x.MiddleName + " " + last);
        Session["user"] = "Welcome! " + name; Session["NameOfLoggedInUser"] = name;
        try { using (var c = SA.Open()) if (SA.Has(c, "AnyminuteSupportAudit")) SA.Audit(c, null, "SupportAdmin", Me.ToString(System.Globalization.CultureInfo.InvariantCulture), "Profile Updated", null, null, null, null, "Support Admin updated their own profile."); }
        catch (System.Data.SqlClient.SqlException) { }
        SA.Flash("Profile updated successfully.");
        Response.Redirect("SupportAdminProfile.aspx", false); Context.ApplicationInstance.CompleteRequest();
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Flash.Text.Length == 0) Flash.Text = SA.TakeFlash();
    }
}
