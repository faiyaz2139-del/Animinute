using System;
using System.Web;
public partial class AnyminuteSupportAccountRecoveryPage : System.Web.UI.Page
{
 protected void Page_Load(object s, EventArgs e) { AnyminuteSupportAdmin.Require(); }
 protected void Start_Click(object s, EventArgs e)
 {
  AnyminuteSupportAdmin.Require();
  string err = AnyminuteLoginEmail.Recovery(AnyminuteSupportAdmin.ActorId(), CurrentEmail.Text, NewEmail.Text, IdentityNote.Text);
  Notice.Visible = true; Notice.CssClass = err == null ? "am-notice" : "am-notice am-error";
  Notice.Text = HttpUtility.HtmlEncode(err ?? "Recovery started. A verification link was sent to the new email and the current email was notified. The login email changes only after the user opens the link.");
  if (err == null) { CurrentEmail.Text = ""; NewEmail.Text = ""; IdentityNote.Text = ""; }
 }
}
