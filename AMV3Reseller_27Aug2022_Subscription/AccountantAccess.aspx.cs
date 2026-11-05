using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

// 2026-10-24 v9 Deck G: Accountant & Payroll Access list.
public partial class AnyminuteAccountantAccessPage : System.Web.UI.Page
{
    protected void Page_Init(object sender, EventArgs e) { AnyminuteModernScope.Login(); ViewStateUserKey = Session.SessionID; AnyminuteFormAccess.OwnerSomewhere(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) { string f = AnyminuteFlash.Take("access"); if (!String.IsNullOrEmpty(f)) { Flash.Visible = true; FlashText.Text = HttpUtility.HtmlEncode(f); } }
        Bind(); BindInvite();
    }
    // v64 (PPT s4/s5): inline invite form (same AnyminuteAccessService.Invite as AccountantInvite.aspx).
    HashSet<int> picked = new HashSet<int>();
    protected bool Picked(int id) { return picked.Contains(id); }
    List<int> Posted()
    {
        var list = new List<int>(); var v = Request.Form.GetValues("biz");
        if (v != null) foreach (var x in v) { int id; if (Int32.TryParse(x, out id) && id > 0 && !list.Contains(id)) list.Add(id); }
        return list;
    }
    void BindInvite()
    {
        if (IsPostBack) picked = new HashSet<int>(Posted());
        else { int pre; if (Int32.TryParse(Request.QueryString["BusinessId"], out pre)) picked.Add(pre); InvTypeAccountant.Checked = true; }
        var owned = AnyminuteAccessService.Owned();
        if (owned.Count == 1 && !IsPostBack) picked.Add(owned[0].Key);
        InvBusinesses.DataSource = owned; InvBusinesses.DataBind();
    }
    protected void InvSend_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime? expiry = null;
            if (InvExpiry.Text.Trim().Length > 0)
            {
                DateTime d; if (!DateTime.TryParseExact(InvExpiry.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) throw new InvalidOperationException("Enter the access expiry as yyyy-mm-dd.");
                expiry = d;
            }
            Done(AnyminuteAccessService.Invite(new AnyminuteAccessService.Input
            {
                Name = InvFullName.Text, Firm = InvFirmName.Text, Email = InvEmail.Text, Phone = InvPhone.Text,
                Accountant = InvTypeAccountant.Checked, Payroll = InvTypePayroll.Checked,
                PayRates = InvPermPayRates.Checked, Export = InvPermExport.Checked, ApprovePayroll = InvPermApprove.Checked, ManageAccess = InvPermManage.Checked,
                Businesses = Posted(), Expires = expiry
            }));
        }
        catch (InvalidOperationException error) { InvError.Text = HttpUtility.HtmlEncode(error.Message); InvError.Visible = true; }
    }
    protected bool Live(object status) { string s = Convert.ToString(status); return s == "Active" || s == "Invited"; }
    void Bind()
    {
        ModuleNote.Visible = !AnyminuteAccessService.Installed();
        var owned = AnyminuteAccessService.Owned();
        var rows = AnyminuteAccessService.List();
        var live = rows.Where(r => Live(r.Status)).ToList();
        TotalCount.Text = live.Count.ToString(CultureInfo.InvariantCulture);
        ActiveCount.Text = live.Count(r => r.Status == "Active").ToString(CultureInfo.InvariantCulture);
        PendingCount.Text = live.Count(r => r.Status == "Invited").ToString(CultureInfo.InvariantCulture);
        BusinessCount.Text = live.SelectMany(r => r.BusinessIds).Distinct().Count().ToString(CultureInfo.InvariantCulture);
        var sb = new StringBuilder();
        foreach (var b in owned) sb.Append("<option value=\"").Append(b.Key.ToString(CultureInfo.InvariantCulture)).Append("\">").Append(HttpUtility.HtmlEncode(b.Value)).Append("</option>");
        BusinessOptions.Text = sb.ToString();
        TabAll.Text = TotalCount.Text; TabPending.Text = PendingCount.Text; TabActive.Text = ActiveCount.Text;
        Rows.DataSource = rows; Rows.DataBind();
        Showing.Text = live.Count == 0 ? "No users yet." : "Showing 1 to " + live.Count + " of " + live.Count + " users";
    }
    void Done(string message) { AnyminuteFlash.Set("access", message); Response.Redirect("~/AccountantAccess.aspx", false); Context.ApplicationInstance.CompleteRequest(); }
    protected void Rows_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        try
        {
            string arg = Convert.ToString(e.CommandArgument);
            if (e.CommandName == "Resend") { int id; if (!Int32.TryParse(arg, out id) || id <= 0) return; AnyminuteAccessService.Find(id, 0); Done(AnyminuteAccessService.SendEmail(id)); return; }
            if (e.CommandName == "Revoke")
            {
                int invite = 0, user = 0; var parts = arg.Split('=');
                if (parts.Length != 2) return;
                if (parts[0] == "InviteId") Int32.TryParse(parts[1], out invite); else if (parts[0] == "UserId") Int32.TryParse(parts[1], out user);
                if (invite <= 0 && user <= 0) return;
                Done(AnyminuteAccessService.Revoke(AnyminuteAccessService.Find(invite, user)));
            }
        }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; Bind(); }
    }
}
