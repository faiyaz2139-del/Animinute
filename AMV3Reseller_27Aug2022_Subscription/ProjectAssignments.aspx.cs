using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

// 2026-10-24 v8 Deck F slide 4: Manage Project Assignments (assigned / add / history / details).
public partial class AnyminuteProjectAssignmentsPage : System.Web.UI.Page
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    int project; string tab; AnyminuteProjectRow p;
    protected bool CanManage { get { return p != null && p.CanManage; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        Int32.TryParse(Request.QueryString["ProjectId"], out project);
        tab = (Request.QueryString["tab"] ?? "assigned").ToLowerInvariant();
        if (tab != "assigned" && tab != "add" && tab != "history" && tab != "details") tab = "assigned";
        if (!IsPostBack) { string f = AnyminuteFlash.Take("project-assign"); if (!String.IsNullOrEmpty(f)) { Flash.Visible = true; FlashText.Text = HttpUtility.HtmlEncode(f); } }
        Bind();
    }
    string Url(string t) { return ResolveUrl("~/ProjectAssignments.aspx") + "?ProjectId=" + project + "&tab=" + t; }
    void Bind()
    {
        p = AnyminuteProjectService.Load(project);
        if (!p.CanManage && (tab == "add")) tab = "assigned";
        ModuleNote.Visible = !AnyminuteProjectService.Installed();
        var members = AnyminuteProjectService.Members(project);
        p.Employees = members.Count;
        HeadProject.Text = HttpUtility.HtmlEncode(p.Name); HeadBusiness.Text = HttpUtility.HtmlEncode(p.Business);
        HeadStatus.Text = HttpUtility.HtmlEncode(p.Status); HeadStatus.CssClass = "am-pj-pill am-pj-st-" + p.Status.ToLowerInvariant();
        HeadStart.Text = p.StartText; HeadEnd.Text = p.EndText; HeadTotal.Text = members.Count.ToString(Inv);
        var tabs = new[] { new[] { "assigned", "Assigned Employees (" + members.Count + ")" }, new[] { "add", "Add / Assign Employees" }, new[] { "history", "Assignment History" }, new[] { "details", "Project Details" } };
        var sb = new StringBuilder();
        foreach (var t in tabs)
        {
            if (t[0] == "add" && !p.CanManage) continue;
            sb.Append("<a role=\"tab\" href=\"").Append(HttpUtility.HtmlAttributeEncode(Url(t[0]))).Append("\" class=\"").Append(t[0] == tab ? "active" : "").Append("\" aria-selected=\"").Append(t[0] == tab ? "true" : "false").Append("\">").Append(HttpUtility.HtmlEncode(t[1])).Append("</a>");
        }
        TabLinks.Text = sb.ToString();
        // Slide 4 shows the assigned list with the add/assign area underneath on the first tab.
        AssignedSection.Visible = tab == "assigned";
        AddSection.Visible = p.CanManage && (tab == "assigned" || tab == "add");
        HistorySection.Visible = tab == "history"; DetailsSection.Visible = tab == "details";
        AssignedActions.Visible = p.CanManage;
        AssignedCount.Text = members.Count.ToString(Inv);
        Members.DataSource = members; Members.DataBind(); NoMembers.Visible = members.Count == 0;
        if (AddSection.Visible)
        {
            var cands = AnyminuteProjectService.Candidates(project);
            Candidates.DataSource = cands; Candidates.DataBind(); NoCandidates.Visible = cands.Count == 0;
            RoleOptions.Text = Options(cands.SelectMany(c => c.Role.Split(',').Select(r => r.Trim())));
            DeptOptions.Text = Options(cands.Select(c => c.Department));
        }
        if (HistorySection.Visible) { var h = AnyminuteProjectService.History(project); History.DataSource = h; History.DataBind(); NoHistory.Visible = h.Rows.Count == 0; }
        if (DetailsSection.Visible)
        {
            DName.Text = HttpUtility.HtmlEncode(p.Name); DBusiness.Text = HttpUtility.HtmlEncode(p.Business); DStart.Text = p.StartText; DEnd.Text = p.EndText;
            DStatus.Text = HttpUtility.HtmlEncode(p.Status); DAssign.Text = p.AssignmentStatus; DEmployees.Text = members.Count.ToString(Inv); DHours.Text = p.HoursText;
            DDescription.Text = p.Description.Length > 0 ? HttpUtility.HtmlEncode(p.Description) : "&mdash;";
            EditProjectLink.Visible = p.CanManage; EditProjectLink.NavigateUrl = "~/ProjectEditor.aspx?ProjectId=" + project;
        }
    }
    static string Options(IEnumerable<string> values)
    {
        var sb = new StringBuilder();
        foreach (var v in values.Where(v => v.Length > 0 && v != "\u2014").Distinct().OrderBy(v => v)) sb.Append("<option>").Append(HttpUtility.HtmlEncode(v)).Append("</option>");
        return sb.ToString();
    }
    protected string LocalTime(object utc) { return utc == null || utc == DBNull.Value ? "" : DateTime.SpecifyKind(Convert.ToDateTime(utc), DateTimeKind.Utc).ToLocalTime().ToString("MMM d, yyyy h:mm tt", Inv); }
    protected string Day(object d) { return d == null || d == DBNull.Value ? "\u2014" : Convert.ToDateTime(d).ToString("MMM d, yyyy", Inv); }
    List<int> Picked(string name)
    {
        var list = new List<int>(); var values = Request.Form.GetValues(name);
        if (values != null) foreach (var v in values) { int id; if (Int32.TryParse(v, out id) && id > 0) list.Add(id); }
        return list;
    }
    void Done(string message, string t) { AnyminuteFlash.Set("project-assign", message); Response.Redirect(Url(t), false); Context.ApplicationInstance.CompleteRequest(); }
    void Fail(string message) { PageError.Text = HttpUtility.HtmlEncode(message); PageError.Visible = true; }
    protected void Members_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int id; if (e.CommandName != "Unassign" || !Int32.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
        try { AnyminuteProjectService.Unassign(project, new[] { id }); Done("Employee unassigned from the project.", tab); }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    protected void UnassignSelected_Click(object sender, EventArgs e)
    {
        try
        {
            var ids = Picked("sel"); if (ids.Count == 0) throw new InvalidOperationException("Select at least one assigned employee.");
            int n = AnyminuteProjectService.Unassign(project, ids); Done(n + " employee(s) unassigned from the project.", tab);
        }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    protected void ApplyEndDate_Click(object sender, EventArgs e)
    {
        try
        {
            var ids = Picked("sel"); if (ids.Count == 0) throw new InvalidOperationException("Select at least one assigned employee.");
            DateTime? d = AnyminuteProjectService.ParseDate(EndDateBox.Text); if (!d.HasValue) throw new InvalidOperationException("Enter the effective end date as yyyy-mm-dd.");
            int n = AnyminuteProjectService.End(project, ids, d.Value);
            Done(d.Value < DateTime.Today ? n + " employee(s) unassigned effective " + d.Value.ToString("yyyy-MM-dd", Inv) + "." : "Effective end date " + d.Value.ToString("yyyy-MM-dd", Inv) + " set for " + n + " employee(s).", tab);
        }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    protected void AssignSelected_Click(object sender, EventArgs e)
    {
        try
        {
            var ids = Picked("add"); if (ids.Count == 0) throw new InvalidOperationException("Select at least one employee to assign.");
            int n = AnyminuteProjectService.Assign(project, ids); Done(n + " employee(s) assigned to the project.", "assigned");
        }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    static string Csv(string v) { v = v ?? ""; if (v.Length > 0 && "=+-@".IndexOf(v[0]) >= 0) v = "'" + v; return "\"" + v.Replace("\"", "\"\"") + "\""; }
    protected void Export_Click(object sender, EventArgs e)
    {
        var sb = new StringBuilder("Employee Name,Role,Department,Assigned Date,Effective End Date,Status\r\n");
        foreach (var m in AnyminuteProjectService.Members(project))
            sb.Append(Csv(m.Name)).Append(',').Append(Csv(m.Role)).Append(',').Append(Csv(m.Department)).Append(',')
              .Append(Csv(m.Assigned.HasValue ? m.Assigned.Value.ToString("yyyy-MM-dd", Inv) : "")).Append(',').Append(Csv(m.EndDate.HasValue ? m.EndDate.Value.ToString("yyyy-MM-dd", Inv) : "")).Append(',').Append(Csv(m.Status)).Append("\r\n");
        string file = "Project_" + project + "_Assignments_" + DateTime.Today.ToString("yyyy-MM-dd", Inv) + ".csv";
        Response.Clear(); Response.ContentType = "text/csv"; Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition", "attachment; filename=" + file);
        Response.BinaryWrite(Encoding.UTF8.GetPreamble()); Response.Write(sb.ToString());
        Response.Flush(); Response.SuppressContent = true; Context.ApplicationInstance.CompleteRequest();
    }
}
