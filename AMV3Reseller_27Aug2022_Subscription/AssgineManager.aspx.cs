using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// 2026-10-28 v27 (live: old UI, empty Manager/Employee pickers when opened without ?BusinessIdFromDefaultPapge, no circular messaging).
// Owner/admin businesses come from AnyminuteModernScope; reads/saves use AnyminuteApprovalAssignments (RequireOwner + circular guard).
public partial class AssgineManager : System.Web.UI.Page
{
    int Business { get { int b; Int32.TryParse(ddlbusiness.SelectedValue, out b); return b; } }
    protected void Page_Init(object sender, EventArgs e) { if (Session["EmployeeId"] == null) { Response.Redirect("Login.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; } ViewStateUserKey = Session.SessionID; }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack || Session["EmployeeId"] == null) return;
        try
        {
            foreach (DataRow r in AnyminuteModernScope.Businesses().Rows)
            {
                int b = Convert.ToInt32(r["BusinessID"]);
                if (AnyminuteModernScope.Owns(b)) ddlbusiness.Items.Add(new ListItem(AnyminuteDisplayText.Tidy(Convert.ToString(r["CompanyName"])), b.ToString()));
            }
        }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("AssgineManager business list failed", ex); }
        if (ddlbusiness.Items.Count == 0) { Editor.Visible = false; Show("Access denied. Only the subscriber or business owner can assign users to managers.", true); return; }
        string q = Request.QueryString["BusinessIdFromDefaultPapge"] ?? Request.QueryString["BusinessId"];
        if (q != null && ddlbusiness.Items.FindByValue(q) != null) ddlbusiness.SelectedValue = q;
        LoadManagers();
    }
    void Show(string text, bool error) { Notice.Text = HttpUtility.HtmlEncode(text); Notice.CssClass = error ? "am-notice am-error" : "am-notice"; Notice.Visible = true; }
    void LoadManagers()
    {
        ddlmanager.Items.Clear(); ddlmanager.Items.Add(new ListItem("Select a manager", ""));
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var people = AnyminuteApprovalAssignments.People(c, null, Business);
            foreach (DataRow r in people.Select("UserTypeId=2", "empname")) ddlmanager.Items.Add(new ListItem(Convert.ToString(r["empname"]), Convert.ToString(r["EmployeeID"])));
        }
        if (ddlmanager.Items.Count == 1) ddlmanager.Items[0].Text = "No active managers in this business";
        else if (ddlmanager.Items.Count == 2) ddlmanager.SelectedIndex = 1;
        LoadPeople();
    }
    void LoadPeople()
    {
        int manager; ddlemp.Items.Clear(); Stale.Text = ""; ViewState.Remove("Fingerprint");
        People.Visible = Int32.TryParse(ddlmanager.SelectedValue, out manager);
        if (!People.Visible) return;
        ManagerName.Text = HttpUtility.HtmlEncode(ddlmanager.SelectedItem.Text);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var people = AnyminuteApprovalAssignments.People(c, null, Business);
            var edges = AnyminuteApprovalAssignments.Edges(c, null, Business);
            var blocked = AnyminuteApprovalAssignments.Blocked(edges, manager, people);
            foreach (DataRow r in people.Rows)
            {
                int id = Convert.ToInt32(r["EmployeeID"]); if (id == manager) continue;
                bool has = edges.Select("ManagerID=" + manager + " AND EmpId=" + id).Length > 0;
                bool other = Convert.ToInt32(r["UserTypeId"]) == 2 && edges.Select("ManagerID<>" + manager + " AND EmpId=" + id).Length > 0;
                var item = new ListItem(Convert.ToString(r["empname"]), id.ToString()) { Selected = has };
                if (!has && blocked.Contains(id)) { item.Enabled = false; item.Text += " (already approves this manager - circular chain) [AM-" + AnyminuteApprovalAssignments.Build + "]"; }
                else if (!has && other) { item.Enabled = false; item.Text += " (already has an approver)"; }
                ddlemp.Items.Add(item);
            }
            int stale = edges.Select("ManagerID=" + manager).Count(x => people.Select("EmployeeID=" + x["EmpId"]).Length == 0);
            if (stale > 0) Stale.Text = "<p class='am-notice'>" + stale + " inactive or unlisted user(s) are still assigned to this manager. Saving keeps only the active users ticked above.</p>";
            ViewState["Fingerprint"] = AnyminuteApprovalAssignments.Fingerprint(edges);
        }
        Empty.Visible = ddlemp.Items.Count == 0;
    }
    protected void ddlbusiness_SelectedIndexChanged(object sender, EventArgs e) { Notice.Visible = false; Guard(LoadManagers); }
    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e) { Notice.Visible = false; Guard(LoadPeople); }
    void Guard(Action a)
    {
        try { a(); }
        catch (InvalidOperationException ex) { Show(ex.Message, true); }
        catch (HttpException ex) { Show(ex.Message, true); }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        int manager;
        if (!Int32.TryParse(ddlmanager.SelectedValue, out manager)) { Show("Select a manager first.", true); return; }
        var ids = ddlemp.Items.Cast<ListItem>().Where(i => i.Selected && i.Enabled).Select(i => Int32.Parse(i.Value)).ToList();
        try
        {
            AnyminuteApprovalAssignments.Save(Business, manager, ids, Convert.ToString(ViewState["Fingerprint"]));
            LoadPeople(); Show("Approval assignments saved for " + ddlmanager.SelectedItem.Text + ".", false);
        }
        catch (InvalidOperationException ex) { Show(ex.Message, true); }
        catch (Exception ex) { log4net.LogManager.GetLogger("Log").Error("AssgineManager save failed", ex); Show("Assignments were not saved. Please refresh and try again. [AM-" + AnyminuteApprovalAssignments.Build + "]", true); }
    }
}
