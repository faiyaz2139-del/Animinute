using System;
using System.Globalization;
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
        Bind();
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
