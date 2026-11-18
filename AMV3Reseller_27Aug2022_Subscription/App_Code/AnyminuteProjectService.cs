using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

// 2026-10-24 v8 Projects module (Deck F): list + delete panel, add/edit, manage assignments.
// Data: AddProject (Status 'A' live / 'D' deleted, unchanged legacy meaning), ASSIGNPROJECT (who is on a project),
// TimeSheetEntry.ProjectId (time; Status 2 = approved). Optional AnyminuteProject* tables (REQUIRED_03) add
// description, display status, assignment dates and history. Every mutation re-checks business ownership.
public class AnyminuteProjectRow
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public int ProjectId { get; set; }
    public int BusinessId { get; set; }
    public string Business { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    public string Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }
    public int Employees { get; set; }
    public double Hours { get; set; }
    public bool CanManage { get; set; }
    public bool HasTime { get; set; }
    public bool Locked { get { return Employees > 0 || HasTime; } }
    public string AssignmentStatus { get { return Employees > 0 ? "Assigned" : "Not Assigned"; } }
    public string StartText { get { return Start.ToString("MMM d, yyyy", Inv); } }
    public string EndText { get { return End.HasValue ? End.Value.ToString("MMM d, yyyy", Inv) : "Ongoing"; } }
    public string StartIso { get { return Start.ToString("yyyy-MM-dd", Inv); } }
    public string EndIso { get { return End.HasValue ? End.Value.ToString("yyyy-MM-dd", Inv) : ""; } }
    public string HoursText { get { return Math.Round(Hours, 1).ToString("0.#", Inv); } }
}
public class AnyminuteProjectEligibility
{
    public int Assigned { get; set; }
    public int Approved { get; set; }
    public int Draft { get; set; }
    public int Payroll { get; set; }
    public bool CanDelete { get { return Assigned == 0 && Approved == 0 && Draft == 0 && Payroll == 0; } }
}
public class AnyminuteProjectMember
{
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    public string Department { get; set; }
    public DateTime? Assigned { get; set; }
    public DateTime? EndDate { get; set; }
    public bool Active { get; set; }
    public string AssignedText { get { return Assigned.HasValue ? Assigned.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string EndText { get { return EndDate.HasValue ? EndDate.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) : "\u2014"; } }
    public string Status { get { return EndDate.HasValue ? "Ending" : "Active"; } }
}

public static class AnyminuteProjectService
{
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    public static readonly string[] StatusValues = { "Active", "Completed", "Inactive" };
    const string ActiveExpr = @"UPPER(LTRIM(RTRIM(COALESCE(h.mp_Stauts,NULLIF(a.Employee_Status,''),
        CASE WHEN UPPER(LTRIM(RTRIM(a.IsActive)))='YES' THEN 'Active' ELSE 'Not Active' END))))";
    const string StatusApply = @"OUTER APPLY (SELECT TOP(1) s.mp_Stauts FROM Emp_Active_Inactive_Status s
        WHERE s.EmpId=a.EmployeeID AND s.Effective_Date<=CONVERT(date,GETDATE())
        ORDER BY s.Effective_Date DESC,s.Datetime_Stamp DESC) h";
    const string RoleExpr = @"STUFF((SELECT DISTINCT ', '+ut.UserType FROM AssginBussiness m2 JOIN UserType ut ON ut.User_TypeId=m2.UserTypeId
        WHERE m2.BusinessID=@b AND m2.EmployeeID=a.EmployeeID FOR XML PATH('')),1,2,'')";

    static DataTable Q(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { return AnyminuteTimesheetRepair.Query(c, tx, sql, a); }
    static void X(SqlConnection c, SqlTransaction tx, string sql, params object[] a) { AnyminuteTimesheetRepair.Execute(c, tx, sql, a); }
    static bool HasTable(SqlConnection c, SqlTransaction tx, string n) { return AnyminuteEmployeeEditor.HasTable(c, tx, n); }
    static int Actor() { return Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]); }
    static string S(object o) { return o == null || o == DBNull.Value ? "" : Convert.ToString(o).Trim(); }
    public static bool Installed(SqlConnection c, SqlTransaction tx) { return HasTable(c, tx, "AnyminuteProjectDetail") && HasTable(c, tx, "AnyminuteProjectAssignment") && HasTable(c, tx, "AnyminuteProjectAssignmentLog"); }
    public static bool Installed() { using (var c = AnyminuteTimesheetRepair.Open()) return Installed(c, null); }
    public static DateTime? ParseDate(string s)
    {
        DateTime d; s = (s ?? "").Trim(); if (s.Length == 0) return null;
        if (DateTime.TryParseExact(s, new[] { "yyyy-MM-dd", "yyyy-M-d", "dd-MMM-yyyy", "dd MMM yyyy", "MM/dd/yyyy", "M/d/yyyy" }, Inv, DateTimeStyles.None, out d)) return d.Date;
        if (DateTime.TryParse(s, Inv, DateTimeStyles.None, out d)) return d.Date;
        return null;
    }
    public static string Title(string s)
    {
        s = (s ?? "").Trim(); if (s.Length == 0) return "\u2014";
        return AnyminuteEmployeeEditor.RolesDisplay(String.Join(", ", s.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).Select(p => p == p.ToUpperInvariant() ? Inv.TextInfo.ToTitleCase(p.ToLowerInvariant()) : p).Distinct().ToArray()));
    }

    // ---------- reads ----------
    static string Visibility { get { return @"(@r IN(1,5) OR EXISTS(SELECT 1 FROM ASSIGNPROJECT x WHERE x.PROJECTID=p.ProjectID AND x.BUSINESSID=@b AND
        (x.EMPLOYEEID=@a OR EXISTS(SELECT 1 FROM AssgineManager m WHERE m.BusinessID=@b AND m.ManagerID=@a AND m.EmpId=x.EMPLOYEEID))))"; } }
    static string Select(bool installed)
    {
        return @"SELECT p.ProjectID,p.ProjectName,p.startDate,p.EndDate,p.BusinessID,
            (SELECT COUNT(DISTINCT x.EMPLOYEEID) FROM ASSIGNPROJECT x WHERE x.PROJECTID=p.ProjectID AND x.BUSINESSID=p.BusinessID) AS Employees,
            (SELECT COUNT(*) FROM TimeSheetEntry t WHERE t.ProjectId=p.ProjectID) AS Entries,
            (SELECT ISNULL(SUM(CAST(DATEDIFF(second,CAST('00:00' AS time),TRY_CONVERT(time,LTRIM(RTRIM(t.Total)))) AS bigint)),0) FROM TimeSheetEntry t WHERE t.ProjectId=p.ProjectID) AS Seconds,"
            + (installed ? "d.ProjectStatus,d.Description" : "CAST(NULL AS nvarchar(20)) AS ProjectStatus,CAST(NULL AS nvarchar(1000)) AS Description")
            + " FROM AddProject p" + (installed ? " LEFT JOIN AnyminuteProjectDetail d ON d.ProjectID=p.ProjectID" : "");
    }
    static AnyminuteProjectRow Row(DataRow r, string business, bool manage)
    {
        string st = S(r["ProjectStatus"]);
        return new AnyminuteProjectRow
        {
            ProjectId = Convert.ToInt32(r["ProjectID"]), BusinessId = Convert.ToInt32(r["BusinessID"]), Business = business, Name = S(r["ProjectName"]),
            Start = r["startDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["startDate"]).Date, End = ParseDate(S(r["EndDate"])),
            Employees = Convert.ToInt32(r["Employees"]), HasTime = Convert.ToInt32(r["Entries"]) > 0, Hours = Convert.ToInt64(r["Seconds"]) / 3600.0,
            Status = st.Length == 0 ? "Active" : st, Description = S(r["Description"]), CanManage = manage
        };
    }
    public static List<AnyminuteProjectRow> List()
    {
        var list = new List<AnyminuteProjectRow>();
        var businesses = AnyminuteModernScope.Businesses();
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool installed = Installed(c, null);
            foreach (DataRow b in businesses.Rows)
            {
                int id = Convert.ToInt32(b["BusinessID"]), role, actor;
                try { actor = AnyminuteModernScope.Actor(c, null, id, out role); } catch (InvalidOperationException) { continue; }
                bool manage = role == 1 || role == 5;
                foreach (DataRow r in Q(c, null, Select(installed) + " WHERE p.BusinessID=@b AND ISNULL(p.Status,'')<>'D' AND " + Visibility + " ORDER BY p.ProjectName", "@b", id, "@a", actor, "@r", role).Rows)
                    list.Add(Row(r, S(b["CompanyName"]), manage));
            }
        }
        return list;
    }
    public static DataTable OwnedBusinesses()
    {
        var all = AnyminuteModernScope.Businesses(); var owned = all.Clone();
        foreach (DataRow b in all.Rows) if (AnyminuteModernScope.Owns(Convert.ToInt32(b["BusinessID"]))) owned.ImportRow(b);
        return owned;
    }
    // Read access: the project's business must be in the caller's scope and the project visible to their role.
    public static AnyminuteProjectRow Load(int project)
    {
        var row = List().FirstOrDefault(p => p.ProjectId == project);
        if (row == null) throw new HttpException(404, "Project not found or not available to your account.");
        return row;
    }
    static AnyminuteProjectRow RequireManage(SqlConnection c, SqlTransaction tx, int project)
    {
        bool installed = Installed(c, tx);
        var t = Q(c, tx, Select(installed) + " WHERE p.ProjectID=@p AND ISNULL(p.Status,'')<>'D'", "@p", project);
        if (t.Rows.Count == 0) throw new InvalidOperationException("Project not found.");
        int business = Convert.ToInt32(t.Rows[0]["BusinessID"]);
        AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
        var name = Q(c, tx, "SELECT CompanyName FROM AddBusiness WHERE BusinessID=@b", "@b", business);
        return Row(t.Rows[0], name.Rows.Count > 0 ? S(name.Rows[0][0]) : "", true);
    }
    static void Log(SqlConnection c, SqlTransaction tx, AnyminuteProjectRow p, int employee, string action, DateTime? effective, string note)
    {
        if (HasTable(c, tx, "AnyminuteProjectAssignmentLog"))
            X(c, tx, "INSERT AnyminuteProjectAssignmentLog(ProjectID,BusinessID,EmployeeID,Action,EffectiveDate,Note,ActorID) VALUES(@p,@b,@e,@x,@d,@n,@a)",
              "@p", p.ProjectId, "@b", p.BusinessId, "@e", employee > 0 ? (object)employee : DBNull.Value, "@x", action,
              "@d", effective.HasValue ? (object)effective.Value : DBNull.Value, "@n", (object)note ?? DBNull.Value, "@a", Actor());
        if (HasTable(c, tx, "AnyminuteApprovalAudit"))
            X(c, tx, "INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC) VALUES(@b,@a,@e,@x,@bv,@av,SYSUTCDATETIME())",
              "@b", p.BusinessId, "@a", Actor(), "@e", employee > 0 ? employee : 0, "@x", "Project" + action, "@bv", "Project " + p.ProjectId, "@av", (object)note ?? DBNull.Value);
    }

    // ---------- add / edit ----------
    public static int Save(int project, int business, string name, DateTime? start, DateTime? end, string status, string description)
    {
        name = (name ?? "").Trim(); description = (description ?? "").Trim(); status = (status ?? "").Trim();
        if (name.Length == 0) throw new InvalidOperationException("Project Name is required.");
        if (name.Length > 100) throw new InvalidOperationException("Project Name must be 100 characters or fewer.");
        if (!start.HasValue) throw new InvalidOperationException("Start Date is required (yyyy-mm-dd).");
        if (end.HasValue && end.Value < start.Value) throw new InvalidOperationException("End Date cannot be before Start Date.");
        if (!StatusValues.Contains(status)) throw new InvalidOperationException("Select a valid Project Status.");
        if (description.Length > 1000) throw new InvalidOperationException("Description must be 1000 characters or fewer.");
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            bool installed = Installed(c, tx);
            AnyminuteProjectRow old = null;
            if (project > 0)
            {
                old = RequireManage(c, tx, project);
                if (business != old.BusinessId && old.Locked) throw new InvalidOperationException("Business name cannot be changed because this project has employee assignments or time entries.");
            }
            AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
            if (Q(c, tx, "SELECT 1 FROM AddProject WHERE BusinessID=@b AND ProjectName=@n AND ISNULL(Status,'')<>'D' AND ProjectID<>@p", "@b", business, "@n", name, "@p", project).Rows.Count > 0)
                throw new InvalidOperationException("A project with this name already exists in the selected business.");
            object endValue = end.HasValue ? end.Value.ToString("yyyy-MM-dd", Inv) : "";
            if (project > 0)
                X(c, tx, "UPDATE AddProject SET ProjectName=@n,startDate=@s,EndDate=@e,BusinessID=@b WHERE ProjectID=@p", "@n", name, "@s", start.Value, "@e", endValue, "@b", business, "@p", project);
            else
                project = Convert.ToInt32(Q(c, tx, "INSERT AddProject(ProjectName,startDate,EndDate,BusinessID,Status) VALUES(@n,@s,@e,@b,'A'); SELECT CAST(SCOPE_IDENTITY() AS int);", "@n", name, "@s", start.Value, "@e", endValue, "@b", business).Rows[0][0]);
            if (installed)
                X(c, tx, @"IF EXISTS(SELECT 1 FROM AnyminuteProjectDetail WHERE ProjectID=@p)
                    UPDATE AnyminuteProjectDetail SET Description=@d,ProjectStatus=CASE WHEN ProjectStatus='Archived' THEN ProjectStatus ELSE @st END,UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE ProjectID=@p
                    ELSE INSERT AnyminuteProjectDetail(ProjectID,Description,ProjectStatus,UpdatedBy) VALUES(@p,@d,@st,@a)",
                  "@p", project, "@d", description.Length == 0 ? (object)DBNull.Value : description, "@st", status, "@a", Actor());
            var saved = new AnyminuteProjectRow { ProjectId = project, BusinessId = business };
            Log(c, tx, saved, 0, old == null ? "Created" : "Updated", start, name + " (" + status + ")");
            tx.Commit();
        }
        return project;
    }

    // ---------- delete / archive (slide 1 rules) ----------
    public static AnyminuteProjectEligibility Eligibility(int project)
    {
        using (var c = AnyminuteTimesheetRepair.Open()) return Eligibility(c, null, project);
    }
    static AnyminuteProjectEligibility Eligibility(SqlConnection c, SqlTransaction tx, int project)
    {
        var r = Q(c, tx, @"SELECT (SELECT COUNT(*) FROM ASSIGNPROJECT WHERE PROJECTID=@p),
            (SELECT COUNT(*) FROM TimeSheetEntry WHERE ProjectId=@p AND ISNULL(Status,0)=2),
            (SELECT COUNT(*) FROM TimeSheetEntry WHERE ProjectId=@p AND ISNULL(Status,0)<>2)+(SELECT COUNT(*) FROM BreakTime WHERE ProjectId=@p)", "@p", project).Rows[0];
        int payroll = 0;
        // Payroll is linked through any payroll/paystub table that carries a ProjectId column (none in the base schema).
        foreach (DataRow t in Q(c, tx, @"SELECT t.TABLE_NAME FROM INFORMATION_SCHEMA.COLUMNS t WHERE t.TABLE_SCHEMA='dbo' AND t.COLUMN_NAME IN('ProjectId','ProjectID')
            AND (t.TABLE_NAME LIKE '%payroll%' OR t.TABLE_NAME LIKE '%paystub%' OR t.TABLE_NAME LIKE '%payrun%')").Rows)
            payroll += Convert.ToInt32(Q(c, tx, "SELECT COUNT(*) FROM dbo.[" + S(t[0]).Replace("]", "]]") + "] WHERE ProjectId=@p", "@p", project).Rows[0][0]);
        return new AnyminuteProjectEligibility { Assigned = Convert.ToInt32(r[0]), Approved = Convert.ToInt32(r[1]), Draft = Convert.ToInt32(r[2]), Payroll = payroll };
    }
    public static void Delete(int project)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var p = RequireManage(c, tx, project);
            if (!Eligibility(c, tx, project).CanDelete) throw new InvalidOperationException("This project has assigned employees or time records, so it cannot be deleted. Archive it instead.");
            // Legacy soft delete (same as Add_Project.aspx): the row is kept with Status='D'.
            X(c, tx, "UPDATE AddProject SET Status='D' WHERE ProjectID=@p", "@p", project);
            Log(c, tx, p, 0, "Deleted", DateTime.Today, p.Name);
            tx.Commit();
        }
    }
    public static void Archive(int project)
    {
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            if (!Installed(c, tx)) throw new InvalidOperationException("Archiving needs the Projects database update (DB_REQUIRED/REQUIRED_03_ProjectModule.sql).");
            var p = RequireManage(c, tx, project);
            if (p.Status == "Archived") throw new InvalidOperationException("This project is already archived.");
            // Stop future use: current assignments end today (history kept); time entries and the project row are retained.
            foreach (DataRow r in Q(c, tx, "SELECT DISTINCT EMPLOYEEID FROM ASSIGNPROJECT WHERE PROJECTID=@p AND EMPLOYEEID IS NOT NULL", "@p", project).Rows)
                EndOne(c, tx, p, Convert.ToInt32(r[0]), DateTime.Today, "Archived");
            X(c, tx, @"IF EXISTS(SELECT 1 FROM AnyminuteProjectDetail WHERE ProjectID=@p) UPDATE AnyminuteProjectDetail SET ProjectStatus='Archived',ArchivedAtUTC=SYSUTCDATETIME(),UpdatedAtUTC=SYSUTCDATETIME(),UpdatedBy=@a WHERE ProjectID=@p
                ELSE INSERT AnyminuteProjectDetail(ProjectID,ProjectStatus,ArchivedAtUTC,UpdatedBy) VALUES(@p,'Archived',SYSUTCDATETIME(),@a)", "@p", project, "@a", Actor());
            Log(c, tx, p, 0, "Archived", DateTime.Today, p.Name);
            tx.Commit();
        }
    }

    // ---------- assignments (slide 4) ----------
    static void Sweep(SqlConnection c, SqlTransaction tx, int project)
    {
        if (!HasTable(c, tx, "AnyminuteProjectAssignment")) return;
        X(c, tx, @"DELETE x FROM ASSIGNPROJECT x JOIN AnyminuteProjectAssignment a ON a.ProjectID=x.PROJECTID AND a.EmployeeID=x.EMPLOYEEID
            WHERE x.PROJECTID=@p AND a.EffectiveEndDate IS NOT NULL AND a.EffectiveEndDate<CAST(GETDATE() AS date)", "@p", project);
    }
    public static List<AnyminuteProjectMember> Members(int project)
    {
        var p = Load(project);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            bool installed = Installed(c, null);
            if (installed && p.CanManage) Sweep(c, null, project);
            var t = Q(c, null, @"SELECT a.EmployeeID,dbo.FN_DESC('AddEmployee',a.EmployeeID) AS Name,a.JobTitle," + RoleExpr + " AS Roles," + ActiveExpr + " AS St,"
                + (installed ? "pa.AssignedDate,pa.EffectiveEndDate" : "CAST(NULL AS date) AS AssignedDate,CAST(NULL AS date) AS EffectiveEndDate")
                + " FROM (SELECT DISTINCT EMPLOYEEID FROM ASSIGNPROJECT WHERE PROJECTID=@p AND BUSINESSID=@b) x JOIN AddEmployee a ON a.EmployeeID=x.EMPLOYEEID " + StatusApply
                + (installed ? " LEFT JOIN AnyminuteProjectAssignment pa ON pa.ProjectID=@p AND pa.EmployeeID=a.EmployeeID" : "") + " ORDER BY 2", "@p", project, "@b", p.BusinessId);
            return t.Rows.Cast<DataRow>().Select(r => new AnyminuteProjectMember
            {
                EmployeeId = Convert.ToInt32(r["EmployeeID"]), Name = S(r["Name"]), Role = Title(S(r["Roles"])), Department = S(r["JobTitle"]).Length > 0 ? S(r["JobTitle"]) : "\u2014",
                Assigned = r["AssignedDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["AssignedDate"]),
                EndDate = r["EffectiveEndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["EffectiveEndDate"]), Active = S(r["St"]) == "ACTIVE"
            }).ToList();
        }
    }
    public static List<AnyminuteProjectMember> Candidates(int project)
    {
        var p = Load(project);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            var t = Q(c, null, @"SELECT a.EmployeeID,dbo.FN_DESC('AddEmployee',a.EmployeeID) AS Name,a.JobTitle," + RoleExpr + " AS Roles," + ActiveExpr + @" AS St
                FROM AddEmployee a " + StatusApply + @"
                WHERE EXISTS(SELECT 1 FROM AssginBussiness m WHERE m.BusinessID=@b AND m.EmployeeID=a.EmployeeID AND m.UserTypeId NOT IN(1,5,12))
                  AND NOT EXISTS(SELECT 1 FROM ASSIGNPROJECT x WHERE x.PROJECTID=@p AND x.EMPLOYEEID=a.EmployeeID) ORDER BY 2", "@p", project, "@b", p.BusinessId);
            return t.Rows.Cast<DataRow>().Select(r => new AnyminuteProjectMember
            {
                EmployeeId = Convert.ToInt32(r["EmployeeID"]), Name = S(r["Name"]), Role = Title(S(r["Roles"])), Department = S(r["JobTitle"]).Length > 0 ? S(r["JobTitle"]) : "\u2014", Active = S(r["St"]) == "ACTIVE"
            }).ToList();
        }
    }
    public static DataTable History(int project)
    {
        Load(project);
        using (var c = AnyminuteTimesheetRepair.Open())
        {
            if (!HasTable(c, null, "AnyminuteProjectAssignmentLog")) return new DataTable();
            return Q(c, null, @"SELECT TOP(500) l.RecordedAtUTC,l.Action,l.EffectiveDate,l.Note,
                CASE WHEN l.EmployeeID IS NULL THEN '' ELSE dbo.FN_DESC('AddEmployee',l.EmployeeID) END AS Employee,
                CASE WHEN l.ActorID IS NULL THEN '' ELSE dbo.FN_DESC('AddEmployee',l.ActorID) END AS ActorName
                FROM AnyminuteProjectAssignmentLog l WHERE l.ProjectID=@p ORDER BY l.RecordedAtUTC DESC,l.LogID DESC", "@p", project);
        }
    }
    public static int Assign(int project, IEnumerable<int> employees)
    {
        int n = 0;
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var p = RequireManage(c, tx, project);
            if (p.Status == "Archived") throw new InvalidOperationException("Archived projects cannot take new assignments.");
            bool installed = Installed(c, tx);
            foreach (int e in employees.Distinct())
            {
                if (Q(c, tx, "SELECT 1 FROM AssginBussiness WHERE BusinessID=@b AND EmployeeID=@e AND UserTypeId NOT IN(1,5,12)", "@b", p.BusinessId, "@e", e).Rows.Count == 0)
                    throw new InvalidOperationException("One of the selected employees does not belong to " + p.Business + ".");
                if (Q(c, tx, "SELECT 1 FROM ASSIGNPROJECT WHERE PROJECTID=@p AND EMPLOYEEID=@e", "@p", project, "@e", e).Rows.Count > 0) continue;
                X(c, tx, "INSERT ASSIGNPROJECT(BUSINESSID,PROJECTID,EMPLOYEEID) VALUES(@b,@p,@e)", "@b", p.BusinessId, "@p", project, "@e", e);
                if (installed)
                    X(c, tx, @"IF EXISTS(SELECT 1 FROM AnyminuteProjectAssignment WHERE ProjectID=@p AND EmployeeID=@e)
                        UPDATE AnyminuteProjectAssignment SET BusinessID=@b,AssignedDate=CAST(GETDATE() AS date),EffectiveEndDate=NULL,AssignedBy=@a,UpdatedAtUTC=SYSUTCDATETIME() WHERE ProjectID=@p AND EmployeeID=@e
                        ELSE INSERT AnyminuteProjectAssignment(ProjectID,EmployeeID,BusinessID,AssignedDate,AssignedBy) VALUES(@p,@e,@b,CAST(GETDATE() AS date),@a)", "@p", project, "@e", e, "@b", p.BusinessId, "@a", Actor());
                Log(c, tx, p, e, "Assigned", DateTime.Today, null); n++;
            }
            tx.Commit();
        }
        return n;
    }
    static void EndOne(SqlConnection c, SqlTransaction tx, AnyminuteProjectRow p, int e, DateTime date, string action)
    {
        X(c, tx, "DELETE FROM ASSIGNPROJECT WHERE PROJECTID=@p AND EMPLOYEEID=@e", "@p", p.ProjectId, "@e", e);
        if (HasTable(c, tx, "AnyminuteProjectAssignment"))
            X(c, tx, "UPDATE AnyminuteProjectAssignment SET EffectiveEndDate=@d,UpdatedAtUTC=SYSUTCDATETIME() WHERE ProjectID=@p AND EmployeeID=@e", "@d", date, "@p", p.ProjectId, "@e", e);
        Log(c, tx, p, e, action == "Archived" ? "Unassigned (archive)" : "Unassigned", date, null);
    }
    // Unassign removes employees from the project only; the project, its time entries and history stay.
    public static int Unassign(int project, IEnumerable<int> employees)
    {
        return End(project, employees, DateTime.Today);
    }
    // Effective end date: past/today ends now; a future date is stored and applied automatically after that day.
    public static int End(int project, IEnumerable<int> employees, DateTime date)
    {
        int n = 0;
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            var p = RequireManage(c, tx, project);
            bool installed = Installed(c, tx);
            if (date.Date > DateTime.Today && !installed) throw new InvalidOperationException("Future end dates need the Projects database update (DB_REQUIRED/REQUIRED_03_ProjectModule.sql).");
            foreach (int e in employees.Distinct())
            {
                if (Q(c, tx, "SELECT 1 FROM ASSIGNPROJECT WHERE PROJECTID=@p AND EMPLOYEEID=@e", "@p", project, "@e", e).Rows.Count == 0) continue;
                if (date.Date <= DateTime.Today) EndOne(c, tx, p, e, date.Date, "Unassigned");
                else
                {
                    X(c, tx, @"IF EXISTS(SELECT 1 FROM AnyminuteProjectAssignment WHERE ProjectID=@p AND EmployeeID=@e)
                        UPDATE AnyminuteProjectAssignment SET EffectiveEndDate=@d,UpdatedAtUTC=SYSUTCDATETIME() WHERE ProjectID=@p AND EmployeeID=@e
                        ELSE INSERT AnyminuteProjectAssignment(ProjectID,EmployeeID,BusinessID,AssignedDate,EffectiveEndDate,AssignedBy) VALUES(@p,@e,@b,CAST(GETDATE() AS date),@d,@a)",
                      "@p", project, "@e", e, "@d", date.Date, "@b", p.BusinessId, "@a", Actor());
                    Log(c, tx, p, e, "End date set", date.Date, null);
                }
                n++;
            }
            tx.Commit();
        }
        return n;
    }
}
