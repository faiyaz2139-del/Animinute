using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// 2026-10-24 v8 Deck F slide 1/2: Projects list grouped by business, row actions menu, Delete side panel.
public partial class AnyminuteProjects : System.Web.UI.Page
{
    protected int SelectedDelete;
    List<AnyminuteProjectRow> all;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            BusinessFilter.Items.Add(new ListItem("All Businesses", ""));
            foreach (System.Data.DataRow b in AnyminuteModernScope.Businesses().Rows)
                BusinessFilter.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]), Convert.ToString(b["BusinessID"])));
            string sel = Request.QueryString["BusinessId"];
            if (sel != null && BusinessFilter.Items.FindByValue(sel) != null) BusinessFilter.SelectedValue = sel;
            string flash = AnyminuteFlash.Take("projects");
            if (!String.IsNullOrEmpty(flash)) { Flash.Visible = true; FlashText.Text = HttpUtility.HtmlEncode(flash); }
        }
        Int32.TryParse(Request.QueryString["delete"], out SelectedDelete);
        Bind();
    }
    protected void Filter_Changed(object sender, EventArgs e) { Bind(); }

    void Bind()
    {
        all = AnyminuteProjectService.List();
        TotalCount.Text = all.Count.ToString(CultureInfo.InvariantCulture);
        AssignedCount.Text = all.Count(p => p.Employees > 0).ToString(CultureInfo.InvariantCulture);
        NotAssignedCount.Text = all.Count(p => p.Employees == 0).ToString(CultureInfo.InvariantCulture);
        CompletedCount.Text = all.Count(p => p.Status == "Completed").ToString(CultureInfo.InvariantCulture);
        AddProject.Visible = all.Any(p => p.CanManage) || AnyminuteProjectService.OwnedBusinesses().Rows.Count > 0;

        IEnumerable<AnyminuteProjectRow> rows = all;
        string q = (Search.Text ?? "").Trim();
        if (q.Length > 0) rows = rows.Where(p => p.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0 || p.Business.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0);
        if (BusinessFilter.SelectedValue.Length > 0) rows = rows.Where(p => p.BusinessId.ToString(CultureInfo.InvariantCulture) == BusinessFilter.SelectedValue);
        if (StatusFilter.SelectedValue.Length > 0) rows = rows.Where(p => p.Status == StatusFilter.SelectedValue);
        if (AssignFilter.SelectedValue.Length > 0) rows = rows.Where(p => p.AssignmentStatus == AssignFilter.SelectedValue);
        DateTime? from = AnyminuteProjectService.ParseDate(FromDate.Text), to = AnyminuteProjectService.ParseDate(ToDate.Text);
        // Date range = projects whose run overlaps the range (open-ended projects run "Ongoing").
        if (from.HasValue) rows = rows.Where(p => !p.End.HasValue || p.End.Value >= from.Value);
        if (to.HasValue) rows = rows.Where(p => p.Start <= to.Value);
        var groups = rows.GroupBy(p => new { p.BusinessId, p.Business }).OrderBy(g => g.Key.Business)
            .Select(g => new { BusinessId = g.Key.BusinessId, Business = g.Key.Business, Count = g.Count(), Items = g.OrderBy(p => p.Name).ToList() }).ToList();
        Groups.DataSource = groups; Groups.DataBind();
        Empty.Visible = groups.Count == 0;
        BindDelete();
    }
    protected void Groups_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
        var rows = (Repeater)e.Item.FindControl("Rows");
        rows.DataSource = DataBinder.Eval(e.Item.DataItem, "Items"); rows.DataBind();
    }
    protected string DeleteUrl(int project)
    {
        var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString()); qs["delete"] = project.ToString(CultureInfo.InvariantCulture);
        return ResolveUrl("~/Projects.aspx") + "?" + qs.ToString();
    }
    string CancelUrl()
    {
        var qs = HttpUtility.ParseQueryString(Request.QueryString.ToString()); qs.Remove("delete");
        return ResolveUrl("~/Projects.aspx") + (qs.Count > 0 ? "?" + qs.ToString() : "");
    }
    static string Check(bool ok, string text)
    {
        return "<li class=\"" + (ok ? "am-pj-yes" : "am-pj-not") + "\"><span class=\"am-pj-dot\">" + (ok ? "&#10003;" : "&#10005;") + "</span>" + HttpUtility.HtmlEncode(text) + "</li>";
    }
    void BindDelete()
    {
        DeletePanel.Visible = false;
        if (SelectedDelete <= 0 || all == null) return;
        var p = all.FirstOrDefault(x => x.ProjectId == SelectedDelete && x.CanManage);
        if (p == null) return;
        var el = AnyminuteProjectService.Eligibility(p.ProjectId);
        DeletePanel.Visible = true; Root.Attributes["class"] = "am-projects am-pj-with-side";
        DelName.Text = HttpUtility.HtmlEncode(p.Name); DelBusiness.Text = HttpUtility.HtmlEncode(p.Business);
        DelAssign.Text = p.AssignmentStatus; DelAssign.CssClass = "am-pj-pill " + (p.Employees > 0 ? "am-pj-assigned" : "am-pj-unassigned");
        DelEmployees.Text = p.Employees.ToString(CultureInfo.InvariantCulture); DelHours.Text = p.HoursText;
        DelChecks.Text = Check(el.Assigned == 0, el.Assigned == 0 ? "No assigned employees" : el.Assigned + " assigned employee(s)")
            + Check(el.Approved == 0, el.Approved == 0 ? "No approved time cards" : el.Approved + " approved time entr" + (el.Approved == 1 ? "y" : "ies"))
            + Check(el.Draft == 0, el.Draft == 0 ? "No draft time entries" : el.Draft + " draft / pending time entr" + (el.Draft == 1 ? "y" : "ies"))
            + Check(el.Payroll == 0, el.Payroll == 0 ? "No payroll linked records" : el.Payroll + " payroll linked record(s)");
        DeleteProject.Enabled = el.CanDelete;
        if (!el.CanDelete) { DeleteProject.CssClass += " am-pj-disabled"; DeleteProject.OnClientClick = ""; DeleteProject.ToolTip = "This project has records. Use Archive Instead."; }
        ArchiveInstead.Visible = p.Status != "Archived";
        DeleteClose.NavigateUrl = DeleteCancel.NavigateUrl = CancelUrl();
    }
    void Done(string message, bool keepDelete)
    {
        AnyminuteFlash.Set("projects", message);
        Response.Redirect(keepDelete ? Request.RawUrl : CancelUrl(), false); Context.ApplicationInstance.CompleteRequest();
    }
    void Fail(string message) { PageError.Text = HttpUtility.HtmlEncode(message); PageError.Visible = true; }
    protected void Rows_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int id; if (!Int32.TryParse(Convert.ToString(e.CommandArgument), out id)) return;
        try { if (e.CommandName == "Archive") { AnyminuteProjectService.Archive(id); Done("Project archived. Assignments ended today; time entries and history were kept.", false); } }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    protected void ArchiveInstead_Click(object sender, EventArgs e)
    {
        try { AnyminuteProjectService.Archive(SelectedDelete); Done("Project archived instead of deleted. Time entries and history were kept.", false); }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
    protected void DeleteProject_Click(object sender, EventArgs e)
    {
        try { AnyminuteProjectService.Delete(SelectedDelete); Done("Project deleted.", false); }
        catch (InvalidOperationException error) { Fail(error.Message); }
    }
}
