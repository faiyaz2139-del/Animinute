using System;
using System.Web;
// v41: one page is both the start link and the registered redirect URI for Google and Microsoft.
// The signed-in UserId is handed to Login.aspx through the server session only (never the URL), so Login.aspx runs the
// exact same checks (first-time login, disabled login access) and role routing as password sign-in.
public partial class AnyminuteExternalLoginPage : System.Web.UI.Page
{
 protected void Page_Load(object s, EventArgs e)
 {
  Response.Cache.SetCacheability(HttpCacheability.NoCache);
  if (!String.IsNullOrEmpty(Request.QueryString["p"]))
  {
   string url = AnyminuteExternalLogin.Start(Request.QueryString["p"]);
   if (url == null) Session[AnyminuteExternalLogin.ErrorKey] = "That sign-in option is not available. Please sign in with your email and password.";
   Go(url ?? ResolveUrl("~/Login.aspx"));
   return;
  }
  string provider, error;
  int user = 0;
  try { user = AnyminuteExternalLogin.Complete(Request, out provider, out error); }
  catch (Exception ex) { provider = null; error = "Could not complete sign-in. Please try again."; try { log4net.LogManager.GetLogger("Log").Error("ExternalLogin: " + ex); } catch { } }
  if (user > 0) Session[AnyminuteExternalLogin.TicketKey] = user + "|" + provider;
  else Session[AnyminuteExternalLogin.ErrorKey] = error ?? "Could not complete sign-in. Please try again.";
  Go(ResolveUrl("~/Login.aspx"));
 }
 void Go(string url) { Response.Redirect(url, false); Context.ApplicationInstance.CompleteRequest(); }
}
