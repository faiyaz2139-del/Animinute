using System;
using System.Web;
public partial class AnyminuteVerifyLoginEmailPage : System.Web.UI.Page
{
 protected void Page_Load(object s, EventArgs e)
 {
  Response.Cache.SetCacheability(HttpCacheability.NoCache);
  if (IsPostBack) return;
  string t = Request.QueryString["t"];
  if (String.IsNullOrEmpty(t) || t.Length != 64) { Done("This verification link is not valid."); return; }
  Heading.Text = "Verify Your New Login Email";
  Message.Text = "Select Confirm to make this email address your AnyMinute login email. Your User ID, businesses, roles and timesheets stay the same.";
 }
 protected void Confirm_Click(object s, EventArgs e)
 {
  string next, err;
  try { err = AnyminuteLoginEmail.Verify(Request.QueryString["t"], out next); }
  catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("Verify login email failed", ex); err = "We could not verify this link right now. Please try again later."; }
  Done(err);
 }
 void Done(string err)
 {
  if (err == null) { Heading.Text = "Login Email Updated Successfully"; Message.Text = "Your login email has been updated. Please use your new email address the next time you sign in."; }
  else { Heading.Text = "Email Not Verified"; Message.Text = HttpUtility.HtmlEncode(err); }
  Confirm.Visible = false; Action.Visible = true;
  if (Session["UserId"] != null) { Action.Text = "Back to AnyMinute"; Action.NavigateUrl = "~/Dashboard.aspx"; }
 }
}
