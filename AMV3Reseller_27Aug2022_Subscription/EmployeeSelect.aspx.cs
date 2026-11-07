using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
// 2026-11-07 v70 (docx s3): Employees > Edit Employee picker. Lists only employees the signed-in subscriber owns (every
// assignment manageable, the same rule EmployeeEditor enforces) and redirects to EmployeeEditor.aspx?EmployeeId=<id>.
public partial class AnyminuteEmployeeSelectPage : System.Web.UI.Page
{
    private List<AnyminuteEmployeeCardView> cards = new List<AnyminuteEmployeeCardView>();
    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); AnyminuteFormAccess.OwnerSomewhere(); ViewStateUserKey = Session.SessionID;
        try { cards = AnyminuteEmployeesView.Load().Where(c => c.Manage).ToList(); }
        catch (HttpException) { cards = new List<AnyminuteEmployeeCardView>(); }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        EmployeeList.Items.Clear();
        EmployeeList.Items.Add(new ListItem("Select an employee", ""));
        foreach (var c in cards)
        {
            string text = (c.Name ?? "").Trim() + (String.IsNullOrWhiteSpace(c.Email) ? "" : " - " + c.Email.Trim()) + " (" + c.BusinessLabel + ")";
            var li = new ListItem(text, c.EmployeeId.ToString(CultureInfo.InvariantCulture));
            li.Attributes["data-k"] = String.Join(" ", new[] { c.Name, c.Email, c.Phone, String.Join(" ", c.Assignments.Select(a => a.Mobile + " " + a.Business)) }).ToLowerInvariant();
            EmployeeList.Items.Add(li);
        }
        CountText.Text = cards.Count + (cards.Count == 1 ? " employee" : " employees");
        bool any = cards.Count > 0; PickPanel.Visible = any; EmptyPanel.Visible = !any; EditSelected.Visible = any;
    }
    protected void EditSelected_Click(object sender, EventArgs e)
    {
        int id;
        if (!Int32.TryParse(EmployeeList.SelectedValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out id) || id <= 0 || !cards.Any(c => c.EmployeeId == id))
        { PageError.Text = "Select an employee from the list."; PageError.Visible = true; return; }
        Response.Redirect("EmployeeEditor.aspx?EmployeeId=" + id.ToString(CultureInfo.InvariantCulture), false);
        Context.ApplicationInstance.CompleteRequest();
    }
}
