using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GridviewNewDesign : Page
{
    private int Business { get { return Int32.Parse(Request.QueryString["BusinessIdFromDefaultPapge"]); } }
    protected void Page_Init(object sender, EventArgs e)
    {
        if (Session["EmployeeId"]==null) { Response.Redirect("Login.aspx"); return; }
        ViewStateUserKey=Session.SessionID;
        int business;
        if (!Int32.TryParse(Request.QueryString["BusinessIdFromDefaultPapge"],out business)) throw new HttpException(400,"Select a valid business.");
        using(var c=AnyminuteTimesheetRepair.Open())
            try { AnyminuteApprovalAssignments.RequireOwner(c,null,business); }
            catch(InvalidOperationException) { throw new HttpException(403,"You cannot manage this business."); }
    }
    protected void Page_Load(object sender, EventArgs e) { if(!IsPostBack) GetMangerList(); }
    public void GetMangerList()
    {
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            var people=AnyminuteApprovalAssignments.People(c,null,Business);
            ddlmanager.Items.Clear(); ddlmanager.Items.Add(new ListItem("Please Select",""));
            ddlmanager.DataSource=people.Select("UserTypeId=2").CopyToDataTableOrEmpty(people);
            ddlmanager.DataValueField="EmployeeID"; ddlmanager.DataTextField="empname"; ddlmanager.DataBind();
        }
    }
    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e)
    {
        int manager;
        bool selected=Int32.TryParse(ddlmanager.SelectedValue,out manager);
        table_button.Visible=Button_LeftMove.Visible=Button_LeftMoveAll.Visible=Button_RightMove.Visible=Button_RightMoveAll.Visible=btnSave.Visible=selected;
        ViewState.Remove("dt"); ViewState.Remove("dtemp"); ViewState.Remove("AssignmentFingerprint");
        if(!selected) { GridView_EmployeeList.DataSource=null;GridView_EmployeeList.DataBind();GridViewMgr.DataSource=null;GridViewMgr.DataBind();GridView_ManagerRoleHistory.DataSource=null;GridView_ManagerRoleHistory.DataBind();return; }
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            var people=AnyminuteApprovalAssignments.People(c,null,Business);
            var edges=AnyminuteApprovalAssignments.Edges(c,null,Business);
            if(people.Select("EmployeeID="+manager+" AND UserTypeId=2").Length!=1) throw new HttpException(403,"Manager is not available.");
            var assigned=people.Clone(); var available=people.Clone();
            foreach(DataRow row in people.Rows)
            {
                int id=Convert.ToInt32(row["EmployeeID"]); if(id==manager) continue;
                bool has=edges.Select("ManagerID="+manager+" AND EmpId="+id).Length>0;
                // Keep current rows visible; disallow accidental reassignment of another manager's approver.
                bool other=Convert.ToInt32(row["UserTypeId"])==2 && edges.Select("ManagerID<>"+manager+" AND EmpId="+id).Length>0;
                if(has) assigned.ImportRow(row); else if(!other) available.ImportRow(row);
            }
            // Do not silently drop inactive/unlisted users from existing assignments.
            foreach(DataRow edge in edges.Select("ManagerID="+manager))
                if(assigned.Select("EmployeeID="+edge["EmpId"]).Length==0)
                {
                    var row=assigned.NewRow();row["EmployeeID"]=edge["EmpId"];row["empname"]="User "+edge["EmpId"]+" (review inactive/role assignment)";row["UserTypeId"]=0;assigned.Rows.Add(row);
                }
            ViewState["dt"]=available; ViewState["dtemp"]=assigned;
            ViewState["AssignmentFingerprint"]=AnyminuteApprovalAssignments.Fingerprint(edges);
        }
        GridView_ManagerRoleHistory.DataSource=AnyminuteEmployeeRepair.RoleHistory(manager,Business);GridView_ManagerRoleHistory.DataBind(); Bind();
    }
    void Bind()
    {
        GridView_EmployeeList.DataSource=ViewState["dt"];GridView_EmployeeList.DataBind();
        GridViewMgr.DataSource=ViewState["dtemp"];GridViewMgr.DataBind();
    }
    void Move(bool assign,bool all)
    {
        var source=ViewState[assign?"dt":"dtemp"] as DataTable;
        var dest=ViewState[assign?"dtemp":"dt"] as DataTable;
        if(source==null||dest==null) { Message("Select a manager first.");return; }
        var grid=assign?GridView_EmployeeList:GridViewMgr;
        var ids=grid.Rows.Cast<GridViewRow>().Where(r=>all||((CheckBox)r.FindControl("chkemp")).Checked)
            .Select(r=>Int32.Parse(((Label)r.FindControl("Label_EmployeeID_emp")).Text)).ToArray();
        foreach(int id in ids)
        {
            var rows=source.Select("EmployeeID="+id);if(rows.Length!=1)continue;
            if(dest.Select("EmployeeID="+id).Length==0)dest.ImportRow(rows[0]);source.Rows.Remove(rows[0]);
        }
        ViewState[assign?"dt":"dtemp"]=source;ViewState[assign?"dtemp":"dt"]=dest;Bind();
    }
    void Message(string value) { ScriptManager.RegisterStartupScript(this,GetType(),"AssignmentMessage","alert('"+HttpUtility.JavaScriptStringEncode(value)+"');",true); }
    protected void Button_LeftMove_Click(object s,EventArgs e) { Move(true,false); }
    protected void Button_LeftMoveAll_Click(object s,EventArgs e) { Move(true,true); }
    protected void Button_RightMove_Click(object s,EventArgs e) { Move(false,false); }
    protected void Button_RightMoveAll_Click(object s,EventArgs e) { Move(false,true); }
    protected void btnSave_Click(object s,EventArgs e)
    {
        try
        {
            int manager;var data=ViewState["dtemp"] as DataTable;
            if(!Int32.TryParse(ddlmanager.SelectedValue,out manager)||data==null) { Message("Select a manager first.");return; }
            AnyminuteApprovalAssignments.Save(Business,manager,data.Rows.Cast<DataRow>().Select(r=>Convert.ToInt32(r["EmployeeID"])),Convert.ToString(ViewState["AssignmentFingerprint"]));
            ddlmanager_SelectedIndexChanged(s,e);Message("Approval assignments saved.");
        }
        catch(InvalidOperationException ex) { Message(ex.Message); }
        catch(Exception ex) { log4net.LogManager.GetLogger("Log").Error("Approval assignment save failed",ex);Message("Assignments were not saved. Contact support."); }
    }
    protected void GridView_EmployeeList_PageIndexChanging(object s,GridViewPageEventArgs e) { GridView_EmployeeList.PageIndex=e.NewPageIndex;Bind(); }
    protected void GridViewMgr_PageIndexChanging(object s,GridViewPageEventArgs e) { GridViewMgr.PageIndex=e.NewPageIndex;Bind(); }
    protected void GridView_EmployeeList_PreRender(object s,EventArgs e) { Header(GridView_EmployeeList); }
    protected void GridViewMgr_PreRender(object s,EventArgs e) { Header(GridViewMgr); }
    static void Header(GridView g) { if(g.HeaderRow!=null) { g.UseAccessibleHeader=true;g.HeaderRow.TableSection=TableRowSection.TableHeader; } }
}
