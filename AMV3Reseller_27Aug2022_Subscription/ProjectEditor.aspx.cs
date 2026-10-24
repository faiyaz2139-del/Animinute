using System;
using System.Data;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;

// 2026-10-24 v8 Deck F slide 3: Add Project / Edit Project.
public partial class AnyminuteProjectEditorPage : System.Web.UI.Page
{
    int project;
    protected void Page_Load(object sender, EventArgs e)
    {
        Int32.TryParse(Request.QueryString["ProjectId"], out project);
        if (IsPostBack) return;
        var owned = AnyminuteProjectService.OwnedBusinesses();
        Business.Items.Add(new ListItem("Select business", ""));
        foreach (DataRow b in owned.Rows) Business.Items.Add(new ListItem(Convert.ToString(b["CompanyName"]), Convert.ToString(b["BusinessID"])));
        ModuleNote.Visible = !AnyminuteProjectService.Installed();
        if (project > 0)
        {
            var p = AnyminuteProjectService.Load(project);
            if (!p.CanManage) throw new HttpException(403, "Only the subscriber or business owner can edit this project.");
            PageTitle.Text = "Edit Project"; PageSubtitle.Text = "Update the project details."; SaveButton.Text = "Save Changes";
            ProjectName.Text = p.Name;
            if (Business.Items.FindByValue(p.BusinessId.ToString(CultureInfo.InvariantCulture)) == null) Business.Items.Add(new ListItem(p.Business, p.BusinessId.ToString(CultureInfo.InvariantCulture)));
            Business.SelectedValue = p.BusinessId.ToString(CultureInfo.InvariantCulture);
            if (p.Locked) { Business.Enabled = false; Business.CssClass = "am-pj-locked"; BusinessLocked.Visible = true; }
            StartDate.Text = p.StartIso; EndDate.Text = p.EndIso;
            if (Status.Items.FindByValue(p.Status) == null) Status.Items.Add(new ListItem(p.Status));
            Status.SelectedValue = p.Status; Description.Text = p.Description;
            DeleteLink.Visible = true; DeleteLink.NavigateUrl = "~/Projects.aspx?delete=" + project;
        }
        else
        {
            string pre = Request.QueryString["BusinessId"];
            if (pre != null && Business.Items.FindByValue(pre) != null) Business.SelectedValue = pre;
            else if (owned.Rows.Count == 1) Business.SelectedIndex = 1;
        }
    }
    protected void Save_Click(object sender, EventArgs e)
    {
        try
        {
            int business;
            if (!Int32.TryParse(Business.SelectedValue, out business) || business <= 0) throw new InvalidOperationException("Business Name is required.");
            DateTime? start = AnyminuteProjectService.ParseDate(StartDate.Text), end = AnyminuteProjectService.ParseDate(EndDate.Text);
            if (StartDate.Text.Trim().Length > 0 && !start.HasValue) throw new InvalidOperationException("Enter Start Date as yyyy-mm-dd.");
            if (EndDate.Text.Trim().Length > 0 && !end.HasValue) throw new InvalidOperationException("Enter End Date as yyyy-mm-dd.");
            string status = Status.SelectedValue == "Archived" ? "Active" : Status.SelectedValue;
            int id = AnyminuteProjectService.Save(project, business, ProjectName.Text, start, end, status, Description.Text);
            AnyminuteFlash.Set("projects", project > 0 ? "Project \u201c" + ProjectName.Text.Trim() + "\u201d was updated." : "Project \u201c" + ProjectName.Text.Trim() + "\u201d was added.");
            Response.Redirect("~/Projects.aspx", false); Context.ApplicationInstance.CompleteRequest();
        }
        catch (InvalidOperationException error) { PageError.Text = HttpUtility.HtmlEncode(error.Message); PageError.Visible = true; }
    }
}
