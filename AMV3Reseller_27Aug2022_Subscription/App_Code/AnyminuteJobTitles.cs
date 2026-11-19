using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;

// 2026-11-18 v90: per-business job title list used on Add Business Assignment. Display and
// storage only; additive table dbo.AnyminuteJobTitle and optional JobTitleId on AnyminuteAssignmentDetail.
public static class AnyminuteJobTitles
{
    static DataTable Q(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, tx, sql, a); }
    static void X(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { AnyminuteTimesheetRepair.Execute(c, tx, sql, a); }
    static int Actor()
    {
        object v = HttpContext.Current == null ? null : HttpContext.Current.Session["EmployeeId"];
        int id; return v != null && Int32.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), out id) ? id : 0;
    }
    public static bool Installed()
    {
        using (var c = AnyminuteTimesheetRepair.Open()) return AnyminuteEmployeeEditor.HasTable(c, null, "AnyminuteJobTitle");
    }
    public static bool AssignmentColumn()
    {
        using (var c = AnyminuteTimesheetRepair.Open()) return AnyminuteEmployeeEditor.HasColumn(c, null, "AnyminuteAssignmentDetail", "JobTitleId");
    }
    static string Clean(string title)
    {
        title = (title ?? "").Replace("\u00a0", " ").Trim();
        if (title.Length == 0) throw new InvalidOperationException("Enter a job title.");
        if (title.Length > 150) throw new InvalidOperationException("Job title can be up to 150 characters.");
        return title;
    }
    public static DataTable List(int business)
    {
        if (business <= 0 || !Installed()) return new DataTable();
        using (var c = AnyminuteTimesheetRepair.Open())
            return Q(c, null, "SELECT JobTitleId,Title FROM AnyminuteJobTitle WHERE BusinessID=@b AND IsDeleted=0 ORDER BY Title", "@b", business);
    }
    public static void Bind(DropDownList list, int business, int selected)
    {
        if (list == null) return;
        list.Items.Clear();
        list.Items.Add(new ListItem("Select Job Title", "0"));
        if (business <= 0) return;
        foreach (DataRow r in List(business).Rows)
        {
            string id = Convert.ToString(r["JobTitleId"], CultureInfo.InvariantCulture);
            list.Items.Add(new ListItem(Convert.ToString(r["Title"]), id));
        }
        if (selected > 0 && list.Items.FindByValue(selected.ToString(CultureInfo.InvariantCulture)) != null)
            list.SelectedValue = selected.ToString(CultureInfo.InvariantCulture);
        else list.SelectedIndex = 0;
    }
    public static int Selected(DropDownList list)
    {
        int id; return list != null && Int32.TryParse(list.SelectedValue, out id) ? id : 0;
    }
    public static int Add(int business, string title)
    {
        if (business <= 0) throw new InvalidOperationException("Select a business first.");
        title = Clean(title);
        if (!Installed()) throw new InvalidOperationException("Job titles are saved after the database update is installed.");
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            AnyminuteApprovalAssignments.RequireOwner(c, null, business);
            if (Q(c, null, "SELECT 1 FROM AnyminuteJobTitle WHERE BusinessID=@b AND IsDeleted=0 AND LOWER(Title)=LOWER(@t)", "@b", business, "@t", title).Rows.Count > 0)
                throw new InvalidOperationException("That job title already exists for this business.");
            var t = Q(c, null, "INSERT AnyminuteJobTitle(BusinessID,Title,IsDeleted,CreatedAtUTC,UpdatedAtUTC,UpdatedBy) OUTPUT INSERTED.JobTitleId VALUES(@b,@t,0,SYSUTCDATETIME(),SYSUTCDATETIME(),@a)",
                "@b", business, "@t", title, "@a", Actor());
            return Convert.ToInt32(t.Rows[0][0]);
        }
    }
    public static void Rename(int business, int id, string title)
    {
        if (business <= 0 || id <= 0) throw new InvalidOperationException("Select a job title to edit.");
        title = Clean(title);
        if (!Installed()) throw new InvalidOperationException("Job titles are saved after the database update is installed.");
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            AnyminuteApprovalAssignments.RequireOwner(c, null, business);
            if (Q(c, null, "SELECT 1 FROM AnyminuteJobTitle WHERE JobTitleId=@i AND BusinessID=@b AND IsDeleted=0", "@i", id, "@b", business).Rows.Count != 1)
                throw new InvalidOperationException("Select a job title to edit.");
            if (Q(c, null, "SELECT 1 FROM AnyminuteJobTitle WHERE BusinessID=@b AND IsDeleted=0 AND LOWER(Title)=LOWER(@t) AND JobTitleId<>@i", "@b", business, "@t", title, "@i", id).Rows.Count > 0)
                throw new InvalidOperationException("That job title already exists for this business.");
            X(c, null, "UPDATE AnyminuteJobTitle SET Title=@t,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE JobTitleId=@i AND BusinessID=@b",
                "@t", title, "@a", Actor(), "@i", id, "@b", business);
        }
    }
    public static void Delete(int business, int id)
    {
        if (business <= 0 || id <= 0) throw new InvalidOperationException("Select a job title to delete.");
        if (!Installed()) throw new InvalidOperationException("Job titles are saved after the database update is installed.");
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            AnyminuteApprovalAssignments.RequireOwner(c, null, business);
            if (Q(c, null, "SELECT 1 FROM AnyminuteJobTitle WHERE JobTitleId=@i AND BusinessID=@b AND IsDeleted=0", "@i", id, "@b", business).Rows.Count != 1)
                throw new InvalidOperationException("Select a job title to delete.");
            X(c, null, "UPDATE AnyminuteJobTitle SET IsDeleted=1,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE JobTitleId=@i AND BusinessID=@b",
                "@a", Actor(), "@i", id, "@b", business);
        }
    }
}
