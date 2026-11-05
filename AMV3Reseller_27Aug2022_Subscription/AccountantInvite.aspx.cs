using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

// 2026-10-24 v9 Deck G: Invite Accountant or Payroll Administrator (new) / edit access (?InviteId= or ?UserId= for legacy accountants).
public partial class AnyminuteAccountantInvitePage : System.Web.UI.Page
{
    int invite, user; HashSet<int> picked = new HashSet<int>(); AnyminuteAccessRow row;
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); ViewStateUserKey = Session.SessionID; AnyminuteFormAccess.OwnerSomewhere(); }
    protected bool Picked(int id) { return picked.Contains(id); }
    protected bool ViewOnly;
    bool Editing { get { return invite > 0 || user > 0; } }
    protected void Page_Load(object sender, EventArgs e)
    {
        Int32.TryParse(Request.QueryString["InviteId"], out invite); Int32.TryParse(Request.QueryString["UserId"], out user);
        if (Editing)
        {
            row = AnyminuteAccessService.Find(invite, user);
            if (row.Status != "Active" && row.Status != "Invited") throw new HttpException(400, "Revoked or expired access cannot be edited. Send a new invitation instead.");
            PageTitle.Text = "Edit Accountant or Payroll Access"; PageSubtitle.Text = "Update access type, permissions, businesses and expiry.";
            SaveButton.Text = "Save Changes"; RevokeButton.Visible = true;
            if (row.UserId > 0 && row.Email.Length > 0) { Email.ReadOnly = true; Email.ToolTip = "The email is linked to an existing login and cannot be changed here."; }
        }
        ModuleNote.Visible = !AnyminuteAccessService.Installed();
        if (!IsPostBack)
        {
            if (row != null)
            {
                FullName.Text = row.Name; FirmName.Text = row.Firm; Email.Text = row.Email; Phone.Text = row.Phone;
                TypeAccountant.Checked = row.Accountant; TypePayroll.Checked = row.Payroll;
                PermPayRates.Checked = row.PayRates; PermExport.Checked = row.Export; PermApprove.Checked = row.ApprovePayroll; PermManage.Checked = row.ManageAccess;
                Expiry.Text = row.Expires.HasValue ? row.Expires.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "";
                foreach (int b in row.BusinessIds) picked.Add(b);
            }
            else
            {
                int pre; if (Int32.TryParse(Request.QueryString["BusinessId"], out pre)) picked.Add(pre);
                TypeAccountant.Checked = true;
            }
        }
        else picked = new HashSet<int>(Posted());
        if (Editing && Request.QueryString["view"] == "1") // v64 (PPT s6): read-only View from the Action column
        {
            PageTitle.Text = "View Accountant or Payroll Access"; PageSubtitle.Text = "Access details. Use Edit Access from the menu to change them.";
            SaveButton.Visible = false; RevokeButton.Visible = false; CancelLink.Text = "Back";
            foreach (var t in new System.Web.UI.WebControls.WebControl[] { FullName, FirmName, Email, Phone, TypeAccountant, TypePayroll, PermPayRates, PermApprove, PermExport, PermManage, Expiry }) t.Enabled = false;
            ViewOnly = true;
        }
        var owned = AnyminuteAccessService.Owned();
        if (owned.Count == 1 && !Editing && !IsPostBack) picked.Add(owned[0].Key);
        Businesses.DataSource = owned; Businesses.DataBind();
    }
    List<int> Posted()
    {
        var list = new List<int>(); var v = Request.Form.GetValues("biz");
        if (v != null) foreach (var s in v) { int id; if (Int32.TryParse(s, out id) && id > 0 && !list.Contains(id)) list.Add(id); }
        return list;
    }
    protected void Save_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime? expiry = null;
            if (Expiry.Text.Trim().Length > 0)
            {
                DateTime d; if (!DateTime.TryParseExact(Expiry.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) throw new InvalidOperationException("Enter the access expiry as yyyy-mm-dd.");
                expiry = d;
            }
            var p = new AnyminuteAccessService.Input
            {
                Name = FullName.Text, Firm = FirmName.Text, Email = Email.Text, Phone = Phone.Text,
                Accountant = TypeAccountant.Checked, Payroll = TypePayroll.Checked,
                PayRates = PermPayRates.Checked, Export = PermExport.Checked, ApprovePayroll = PermApprove.Checked, ManageAccess = PermManage.Checked,
                Businesses = Posted(), Expires = expiry
            };
            string message = Editing ? AnyminuteAccessService.Update(row, p) : AnyminuteAccessService.Invite(p);
            AnyminuteFlash.Set("access", message);
            Response.Redirect("~/AccountantAccess.aspx", false); Context.ApplicationInstance.CompleteRequest();
        }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; }
    }
    protected void Revoke_Click(object sender, EventArgs e)
    {
        try { AnyminuteFlash.Set("access", AnyminuteAccessService.Revoke(row)); Response.Redirect("~/AccountantAccess.aspx", false); Context.ApplicationInstance.CompleteRequest(); }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; }
    }
}
