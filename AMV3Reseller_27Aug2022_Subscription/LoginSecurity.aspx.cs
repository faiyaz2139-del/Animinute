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
 }
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
