using System;
using System.Linq;
using System.Data;
using System.Web;
public partial class AnyminuteModernMaster : System.Web.UI.MasterPage
{
 private bool signingOut,accountantHeader;
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
  if(AnyminuteAccountantDashboard.Applies())context="Accountant & Payroll Admin"; // v33 deck header; v37: always, as in PPT s1
  // v37 (PPT s1 header, strict): accountants see "Welcome! <firm>" / "Accountant & Payroll Admin", a notification bell and the
  // initials circle + firm name in the account menu. Everyone else keeps the profile picture.
  AccountAvatar.Text="<img src='Assets/assets/img/profile2.jpg' alt='' class='am-account-avatar' />";
  if(AnyminuteAccountantDashboard.Applies())
  {
   string firm=AnyminuteAccountantDashboard.Firm();string who=firm.Length>0?firm:AnyminuteDisplayText.Tidy(displayName);
   if(firm.Length>0)Welcome.Text=HttpUtility.HtmlEncode(firm);
   AccountAvatar.Text="<span class='am-ap-avatar' aria-hidden='true'>"+HttpUtility.HtmlEncode(AnyminuteAccountantDashboard.Initials(who))+"</span><span class='am-ap-avatar-name'>"+HttpUtility.HtmlEncode(who)+"</span>";
   accountantHeader=true;
  }
  if(!String.IsNullOrEmpty(context))WelcomeContext.Text="<small class='am-welcome-context' style='display:block;font-size:12px;font-weight:500;color:#5b6778;line-height:1.3'>"+HttpUtility.HtmlEncode(context)+"</small>";

  int business;
  if(Int32.TryParse(Request.QueryString["BusinessId"]??Request.QueryString["BusinessIDEdit"]??Request.QueryString["BusinessIdFromDefaultPapge"],out business)) {
   // v37 (DOCX images 5/6): page-action evidence must never turn a page into ErrorPages (e.g. LoginAccess.aspx?BusinessId=).
   try{AnyminuteClockEvidence.Record(business,"PageAction");}
   catch(System.Threading.ThreadAbortException){throw;}
   catch(Exception ex){log4net.LogManager.GetLogger("Log").Warn("PageAction evidence skipped for business "+business,ex);}
  }

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
  if(accountantHeader)HeaderBell.Text="<a class='am-ap-bell' href='"+HttpUtility.HtmlAttributeEncode(VirtualPathUtility.ToAbsolute("~/Notifications.aspx"))+"' aria-label='Notifications"+(unread>0?" ("+unread+" unread)":"")+"'><svg viewBox='0 0 24 24' width='22' height='22' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' aria-hidden='true'><path d='M18 8a6 6 0 10-12 0c0 7-3 9-3 9h18s-3-2-3-9M13.7 21a2 2 0 01-3.4 0'/></svg>"+(unread>0?"<span class='am-ap-bell-dot'></span>":"")+"</a>";
 }
}
