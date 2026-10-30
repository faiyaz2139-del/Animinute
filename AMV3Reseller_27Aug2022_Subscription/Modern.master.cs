using System;
using System.Linq;
using System.Data;
using System.Web;
public partial class AnyminuteModernMaster : System.Web.UI.MasterPage
{
 private bool signingOut;
 protected void Page_Init(object s,EventArgs e)
 {Response.ContentEncoding=System.Text.Encoding.UTF8;Response.HeaderEncoding=System.Text.Encoding.UTF8;Response.Charset="utf-8";if(Session["UserId"]==null) {Response.Redirect("Login.aspx");return;}AnyminuteModernScope.Login();Page.ViewStateUserKey=Session.SessionID;}
 protected void Page_Load(object s,EventArgs e)
 {
  var menu=AnyminuteUiNavigation.Load().ToList();
  Navigation.Text=AnyminuteUiNavigation.Render(menu);HeaderHelp.Text=AnyminuteUiNavigation.HeaderHelp(menu);AccountUpgrade.Text=AnyminuteUiNavigation.AccountUpgrade(menu);
  Payroll.Visible=AnyminuteModernScope.Businesses().Rows.Cast<DataRow>().Any(r=>AnyminuteModernScope.Owns(Convert.ToInt32(r["BusinessID"])));
  string displayName=Convert.ToString(Session["user"]).Trim();
  while(displayName.StartsWith("Welcome!",StringComparison.OrdinalIgnoreCase))displayName=displayName.Substring(8).TrimStart();
  // v22: some logins (Pandey images 4/9/10) have no Session["user"]; fall back to the signed-in person's name.
  if(displayName.Length==0){int me;if(Int32.TryParse(Convert.ToString(Session["EmployeeId"]),out me))try{using(var c=AnyminuteTimesheetRepair.Open())displayName=Convert.ToString(AnyminuteTimesheetRepair.Query(c,null,"SELECT dbo.FN_DESC('AddEmployee',@e)","@e",me).Rows[0][0]).Trim();}catch(System.Data.SqlClient.SqlException){}}
  Welcome.Text=HttpUtility.HtmlEncode(AnyminuteDisplayText.Tidy(displayName)); // v27: no double spaces
  // v20 (Pandey images 1/3/4): employee / supervisor role and business under the welcome line.
  string context=AnyminuteRoleDashboard.HeaderContext();
  if(String.IsNullOrEmpty(context)&&AnyminuteAccountantDashboard.Applies())context="Accountant & Payroll Admin"; // v33 deck header
  if(!String.IsNullOrEmpty(context))WelcomeContext.Text="<small class='am-welcome-context' style='display:block;font-size:12px;font-weight:500;color:#5b6778;line-height:1.3'>"+HttpUtility.HtmlEncode(context)+"</small>";

  int business;
  if(Int32.TryParse(Request.QueryString["BusinessId"]??Request.QueryString["BusinessIDEdit"]??Request.QueryString["BusinessIdFromDefaultPapge"],out business)) AnyminuteClockEvidence.Record(business,"PageAction");

 }
 protected void SignOut_Click(object sender,EventArgs e)
 {
  signingOut=true;
  AnyminuteSessionGuard.SignedOut();System.Web.Security.FormsAuthentication.SignOut();Session.Clear();Session.Abandon();
  Response.Redirect("Login.aspx",false);Context.ApplicationInstance.CompleteRequest();
 }
 protected void Page_PreRender(object sender,EventArgs e)
 {
  if(signingOut || Session["UserId"]==null)return;
  int count=AnyminuteModernScope.Pending();Pending.Text=count>0?"<span class='am-count'>"+count+"</span>":"";
  int unread=AnyminuteWeekService.Notifications().Rows.Cast<DataRow>().Count(r=>r["ReadAtUTC"]==DBNull.Value);
  Unread.Text=unread>0?"<span class='am-count'>"+unread+"</span>":"";
 }
}
