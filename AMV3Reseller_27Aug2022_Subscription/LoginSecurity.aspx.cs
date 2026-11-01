using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
// v39 (Change Login Email DOC): own login only - the UserId always comes from the signed-in session, never from the request.
public partial class AnyminuteLoginSecurityPage : System.Web.UI.Page
{
 int Me { get { return Convert.ToInt32(Session["UserId"]); } }
 protected void Page_Load(object s, EventArgs e)
 {
  if (Session["UserId"] == null) { Response.Redirect("~/Login.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
  if (!IsPostBack) Show();
 }
 void Show()
 {
  var i = AnyminuteLoginEmail.Load(Me);
  UserName.Text = HttpUtility.HtmlEncode(i.Name.Length > 0 ? i.Name : "\u2014");
  CurrentEmail.Text = HttpUtility.HtmlEncode(i.Email);
  string cls = i.PendingEmail != null ? "Submitted" : i.Verified ? "Approved" : "";
  VerifyStatus.Text = "<span class='am-badge " + cls + "'>" + HttpUtility.HtmlEncode(i.Status) + "</span>";
  PendingView.Visible = i.PendingEmail != null;
  if (i.PendingEmail != null) { PendingEmail.Text = HttpUtility.HtmlEncode(i.PendingEmail); PendingUntil.Text = HttpUtility.HtmlEncode(i.PendingExpiresUtc.Value.ToString("MMM d, yyyy HH:mm", CultureInfo.InvariantCulture) + " UTC"); }
  if (!i.Ready) Say(AnyminuteLoginEmail.MissingTables, true);
  Send.Enabled = i.Ready;
  var h = AnyminuteLoginEmail.History(Me); HistoryView.Visible = h.Rows.Count > 0;
  var sb = new StringBuilder();
  foreach (DataRow r in h.Rows)
   sb.Append("<tr><td>").Append(HttpUtility.HtmlEncode(Convert.ToDateTime(r["ChangedOn"]).ToString("MMM d, yyyy HH:mm", CultureInfo.InvariantCulture))).Append("</td><td>")
     .Append(HttpUtility.HtmlEncode(r["PreviousEmail"])).Append("</td><td>").Append(HttpUtility.HtmlEncode(r["NewEmail"])).Append("</td><td>")
     .Append(HttpUtility.HtmlEncode(r["VerificationMethod"])).Append("</td><td>").Append(HttpUtility.HtmlEncode(r["Result"])).Append("</td></tr>");
  HistoryRows.Text = sb.ToString();
  ShowSocial();
 }
 void ShowSocial()
 {
  bool ready; var t = AnyminuteExternalLogin.Connected(Me, out ready);
  DisconnectGoogle.Visible = DisconnectMicrosoft.Visible = false;
  if (!ready) { SocialRows.Text = "<p class='am-muted am-ls-small'>" + HttpUtility.HtmlEncode(AnyminuteExternalLogin.MissingTable) + "</p>"; return; }
  if (t.Rows.Count == 0) { SocialRows.Text = "<p class='am-muted am-ls-small'>No Google or Microsoft account is connected yet.</p>"; return; }
  var sb = new StringBuilder("<div class='am-table-wrap'><table class='am-table'><thead><tr><th>Provider</th><th>Email when connected</th><th>Connected (UTC)</th><th>Last used (UTC)</th></tr></thead><tbody>");
  foreach (DataRow r in t.Rows)
  {
   string p = Convert.ToString(r["Provider"]);
   if (p == AnyminuteExternalLogin.Google) DisconnectGoogle.Visible = true; else if (p == AnyminuteExternalLogin.Microsoft) DisconnectMicrosoft.Visible = true;
   sb.Append("<tr><td>").Append(HttpUtility.HtmlEncode(p)).Append("</td><td>").Append(HttpUtility.HtmlEncode(r["EmailAtLink"])).Append("</td><td>")
     .Append(HttpUtility.HtmlEncode(Convert.ToDateTime(r["LinkedOn"]).ToString("MMM d, yyyy HH:mm", CultureInfo.InvariantCulture))).Append("</td><td>")
     .Append(r["LastUsedOn"] == DBNull.Value ? "" : HttpUtility.HtmlEncode(Convert.ToDateTime(r["LastUsedOn"]).ToString("MMM d, yyyy HH:mm", CultureInfo.InvariantCulture))).Append("</td></tr>");
  }
  SocialRows.Text = sb.Append("</tbody></table></div>").ToString();
 }
 protected void DisconnectGoogle_Click(object s, EventArgs e) { AnyminuteExternalLogin.Disconnect(Me, "google"); Show(); Say("Google sign-in disconnected. You can still sign in with your email and password.", false); }
 protected void DisconnectMicrosoft_Click(object s, EventArgs e) { AnyminuteExternalLogin.Disconnect(Me, "microsoft"); Show(); Say("Microsoft sign-in disconnected. You can still sign in with your email and password.", false); }
 void Say(string m, bool error) { Notice.Visible = true; Notice.Text = HttpUtility.HtmlEncode(m); Notice.CssClass = error ? "am-notice am-error" : "am-notice"; }
 protected void Send_Click(object s, EventArgs e)
 {
  string err = AnyminuteLoginEmail.Request(Me, NewEmail.Text, ConfirmEmail.Text, Password.Text);
  Show();
  if (err != null) { Say(err, true); return; }
  Say("Verification email sent to " + NewEmail.Text.Trim() + ". Open the link in that email to finish. Until then, keep signing in with your current login email.", false);
  NewEmail.Text = ""; ConfirmEmail.Text = "";
 }
 protected void CancelPending_Click(object s, EventArgs e)
 {
  string err = AnyminuteLoginEmail.Cancel(Me); Show();
  Say(err ?? "The pending email change was cancelled. Your login email has not changed.", err != null);
 }
}
