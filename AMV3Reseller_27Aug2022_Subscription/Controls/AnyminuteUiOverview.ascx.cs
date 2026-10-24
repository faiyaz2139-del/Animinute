using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.UI.HtmlControls;

public partial class AnyminuteUiOverview : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int user, employee;
        // Existing reseller/support administrator sessions can legitimately use employee 0.
        // Do not change those legacy flows just to render optional dashboard cards.
        if (!Int32.TryParse(Convert.ToString(Session["UserId"]), out user) || user <= 0 ||
            !Int32.TryParse(Convert.ToString(Session["EmployeeId"]), out employee) || employee <= 0)
        {
            UiDirectoryAccess.Value = "false";
            amServerOverview.Visible = false;
            return;
        }
        bool access = AnyminuteDirectoryScope.CanReadDirectory();
        UiDirectoryAccess.Value = access ? "true" : "false";
        // A native Dashboard retains its own existing metrics and queries.
        if (!access || !String.Equals(System.IO.Path.GetFileName(Request.FilePath), "Home_Page.aspx", StringComparison.OrdinalIgnoreCase))
        { amServerOverview.Visible = false; return; }
        var businesses = AnyminuteDirectoryScope.Businesses();
        var employees = new HashSet<int>(); int projects = 0, pending = 0;
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool weeksExist = AnyminuteTimesheetRepair.Query(c, null,
                "SELECT OBJECT_ID('dbo.AnyminuteWeek','U') ObjectId").Rows[0][0] != DBNull.Value;
            PendingCard.Visible = weeksExist;
            foreach (DataRow business in businesses.Rows)
            {
                int id = Convert.ToInt32(business["BusinessID"]), actor;
                int role = AnyminuteDirectoryScope.BusinessRole(c, id, out actor);
                var members = AnyminuteTimesheetRepair.Query(c, null, @"
                    SELECT DISTINCT a.EmployeeID FROM AssginBussiness a
                    WHERE a.BusinessID=@b AND a.UserTypeId<>1 AND
                    (@r<>2 OR EXISTS (SELECT 1 FROM AssgineManager m WHERE m.BusinessID=@b AND m.ManagerID=@a AND m.EmpId=a.EmployeeID))",
                    "@b", id, "@r", role, "@a", actor);
                foreach (DataRow member in members.Rows) if (member[0] != DBNull.Value) employees.Add(Convert.ToInt32(member[0]));
                projects += Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null,
                    "SELECT COUNT(*) FROM AddProject WHERE BusinessID=@b AND Status='A'", "@b", id).Rows[0][0]);
                if (weeksExist)
                    pending += Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, null, @"
                        SELECT COUNT(*) FROM dbo.AnyminuteWeek w WHERE w.BusinessID=@b AND w.State='Submitted'
                        AND (@r IN (1,5) OR EXISTS (SELECT 1 FROM AssgineManager m WHERE m.BusinessID=@b AND m.ManagerID=@a AND m.EmpId=w.EmployeeID))",
                        "@b", id, "@r", role, "@a", actor).Rows[0][0]);
            }
        }
        BusinessesCount.Text = businesses.Rows.Count.ToString(CultureInfo.InvariantCulture);
        EmployeesCount.Text = employees.Count.ToString(CultureInfo.InvariantCulture);
        ProjectsCount.Text = projects.ToString(CultureInfo.InvariantCulture);
        PendingCount.Text = pending.ToString(CultureInfo.InvariantCulture);
    }
}
