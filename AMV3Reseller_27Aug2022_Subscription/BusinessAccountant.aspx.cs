using System;
using System.Web;
using System.Web.UI;
// v65 (PPT s4/s5): old Assign Accountant UI removed; every visit lands on the redesigned Accountant & Payroll Access page.
public partial class BusinessAccountant : Page
{
    protected void Page_Init(object s,EventArgs e)
    {
        if(Session["EmployeeId"]==null) { Response.Redirect("Login.aspx",false);Context.ApplicationInstance.CompleteRequest();return; }
        int business;
        string target=Int32.TryParse(Request.QueryString["BusinessId"],out business)&&business>0?"AccountantAccess.aspx?BusinessId="+business+"#am-ac-inviteform":"AccountantAccess.aspx";
        Response.Redirect(target,true);
    }
}
