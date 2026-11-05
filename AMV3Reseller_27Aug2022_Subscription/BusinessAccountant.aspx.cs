using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
public partial class BusinessAccountant : Page
{
    private int Business { get { return Int32.Parse(Request.QueryString["BusinessId"]); } }
    protected void Page_Init(object s,EventArgs e)
    {
        if(Session["EmployeeId"]==null) { Response.Redirect("Login.aspx");return; }
        ViewStateUserKey=Session.SessionID;
        int business;if(!Int32.TryParse(Request.QueryString["BusinessId"],out business)) throw new HttpException(400,"Select a business.");
        using(var c=AnyminuteTimesheetRepair.Open()) AnyminuteApprovalAssignments.RequireOwner(c,null,business);
        // v64 (PPT s4/s5): Assign Accountant is replaced by Accountant & Payroll Access (invite form + Invited & Active Users).
        Response.Redirect("AccountantAccess.aspx?BusinessId="+business+"#am-ac-inviteform",false);Context.ApplicationInstance.CompleteRequest();
    }
    protected void Page_Load(object s,EventArgs e) { if(!IsPostBack) Bind(); }
    void Bind()
    {
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            Accountants.Items.Clear();Accountants.Items.Add(new ListItem("Please Select",""));Accountants.AppendDataBoundItems=true;
            Accountants.DataSource=AnyminuteAccountantAssignments.Available(c,null);
            Accountants.DataValueField="USERID";Accountants.DataTextField="empname";Accountants.DataBind();
            Assigned.DataSource=AnyminuteAccountantAssignments.Current(c,null,Business);Assigned.DataBind();
        }
    }
    protected void Assign_Click(object s,EventArgs e) { int user;if(!Int32.TryParse(Accountants.SelectedValue,out user)) { Result.Text="Select an accountant.";return; } Change(user,false); }
    protected void Assigned_RowCommand(object s,GridViewCommandEventArgs e) { if(e.CommandName=="Unassign") Change(Int32.Parse(Convert.ToString(e.CommandArgument)),true); }
    void Change(int user,bool remove)
    {
        try { AnyminuteAccountantAssignments.Save(Business,user,remove);Bind();Result.Text=remove?"Accountant unassigned.":"Accountant assigned."; }
        catch(InvalidOperationException ex) { Result.Text=HttpUtility.HtmlEncode(ex.Message); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Accountant assignment failed",ex);Result.Text="Assignment was not changed. Contact support."; }
    }
}
