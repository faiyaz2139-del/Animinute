using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

// Business identity comes from the authenticated account and persisted memberships.
public static class AnyminuteApprovalAssignments
{
    public static int RequireOwner(SqlConnection c, SqlTransaction tx, int business)
    {
        if (business <= 0) throw new InvalidOperationException("Select a valid business.");
        int actor, role;
        var s = HttpContext.Current.Session;
        if (s == null || !Int32.TryParse(Convert.ToString(s["EmployeeId"]), out actor) ||
            !Int32.TryParse(Convert.ToString(s["UserTypeid"]), out role))
            throw new InvalidOperationException("Sign in again.");
        AnyminuteTimesheetRepair.ResolveBusinessActor(c, tx, ref actor, ref role, business);
        if (role != 1 && role != 5) throw new InvalidOperationException("Only the subscriber or business owner can manage approval assignments.");
        return actor;
    }
    public static DataTable People(SqlConnection c, SqlTransaction tx, int business)
    {
        return AnyminuteTimesheetRepair.Query(c, tx, @"SELECT DISTINCT a.EmployeeID,
            dbo.FN_DESC('AddEmployee',a.EmployeeID) AS empname, m.UserTypeId, lg.USERID AS LoginId
            FROM AddEmployee a JOIN AssginBussiness m ON m.EmployeeID=a.EmployeeID
            OUTER APPLY (SELECT MIN(mt.USERID) AS USERID FROM MappingTable mt WHERE mt.EmployeeID=a.EmployeeID AND mt.BusinessID=m.BusinessID) lg
            OUTER APPLY (SELECT TOP(1) s.mp_Stauts FROM Emp_Active_Inactive_Status s
              WHERE s.EmpId=a.EmployeeID AND s.Effective_Date<=CONVERT(date,GETDATE())
              ORDER BY s.Effective_Date DESC,TRY_CONVERT(datetime2,s.Datetime_Stamp) DESC,s.Datetime_Stamp DESC) h
            WHERE m.BusinessID=@b AND m.UserTypeId NOT IN (1,5,12)
              AND UPPER(LTRIM(RTRIM(COALESCE(h.mp_Stauts,NULLIF(a.Employee_Status,''),
                CASE WHEN UPPER(LTRIM(RTRIM(a.IsActive)))='YES' THEN 'Active' ELSE 'Not Active' END))))='ACTIVE'
            ORDER BY empname", "@b", business);
    }
    public static DataTable Edges(SqlConnection c, SqlTransaction tx, int business)
    {
        return AnyminuteTimesheetRepair.Query(c, tx,
            "SELECT ManagerID,EmpId FROM AssgineManager WITH (UPDLOCK,HOLDLOCK) WHERE BusinessID=@b ORDER BY ManagerID,EmpId", "@b", business);
    }
    public static string Fingerprint(DataTable edges)
    {
        var lines = edges.Rows.Cast<DataRow>().Select(r => Convert.ToString(r["ManagerID"])+":"+Convert.ToString(r["EmpId"])).OrderBy(x=>x);
        using (var hash=SHA256.Create()) return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(String.Join(";",lines.ToArray()))));
    }
    // v22 (Pandey #3): the old ValidateGraph (any loop or self-row anywhere in the business blocked every save) is removed,
    // so its message can no longer appear. Rules now:
    //  - only assignments of CURRENT active managers (UserTypeId 2, member of this business) are graph edges; rows of former
    //    managers, self-rows and rows naming non-members are ignored as stale;
    //  - the same person (same login in this business) can never be their own approver;
    //  - only users NEWLY added by this save are checked for a loop back to the manager. Loops already in legacy data no
    //    longer block unrelated changes.
    public const string Build = "v27"; // v27: tag follows the shipped build (was v22)
    static Dictionary<int,List<int>> Graph(IEnumerable<Tuple<int,int>> edges, DataTable people)
    {
        var managers = new HashSet<int>(people.Select("UserTypeId=2").Select(r => Convert.ToInt32(r["EmployeeID"])));
        var members = new HashSet<int>(people.Rows.Cast<DataRow>().Select(r => Convert.ToInt32(r["EmployeeID"])));
        var graph = new Dictionary<int,List<int>>();
        foreach (var edge in edges)
        {
            if (edge.Item1 == edge.Item2 || !managers.Contains(edge.Item1) || !members.Contains(edge.Item2)) continue;
            List<int> list; if (!graph.TryGetValue(edge.Item1, out list)) graph[edge.Item1] = list = new List<int>();
            if (!list.Contains(edge.Item2)) list.Add(edge.Item2);
        }
        return graph;
    }
    static string Login(DataTable people, int id)
    {
        var rows = people.Select("EmployeeID=" + id);
        return rows.Length == 0 || rows[0]["LoginId"] == DBNull.Value ? null : Convert.ToString(rows[0]["LoginId"]);
    }
    static string Name(DataTable people, int id)
    {
        var rows = people.Select("EmployeeID=" + id); return rows.Length == 0 ? "User " + id : Convert.ToString(rows[0]["empname"]);
    }
    // Returns the path candidate -> ... -> manager through current managers' assignments, or null.
    static List<int> PathTo(Dictionary<int,List<int>> graph, int from, int manager)
    {
        var prev = new Dictionary<int,int>(); var queue = new Queue<int>(); queue.Enqueue(from); prev[from] = from;
        while (queue.Count > 0)
        {
            int node = queue.Dequeue(); List<int> next; if (!graph.TryGetValue(node, out next)) continue;
            foreach (int n in next)
            {
                if (prev.ContainsKey(n)) continue; prev[n] = node;
                if (n == manager) { var path = new List<int> { n }; int x = n; while (x != from) { x = prev[x]; path.Insert(0, x); } return path; }
                queue.Enqueue(n);
            }
        }
        return null;
    }
    // Users that cannot be added under this manager: the manager (same login) and anyone who already approves the manager.
    public static HashSet<int> Blocked(DataTable edges, int manager, DataTable people)
    {
        var list = edges.Rows.Cast<DataRow>().Select(r => Tuple.Create(Convert.ToInt32(r["ManagerID"]), Convert.ToInt32(r["EmpId"])))
            .Where(t => t.Item1 != manager).ToList();
        var graph = Graph(list, people); var blocked = new HashSet<int> { manager }; string me = Login(people, manager);
        foreach (DataRow r in people.Rows)
        {
            int id = Convert.ToInt32(r["EmployeeID"]);
            if (me != null && me == Login(people, id)) blocked.Add(id);
            else if (PathTo(graph, id, manager) != null) blocked.Add(id);
        }
        return blocked;
    }
    public static void ValidateManager(IEnumerable<Tuple<int,int>> edges, int manager, DataTable people, ICollection<int> added)
    {
        string me = Login(people, manager);
        foreach (int id in added)
            if (id == manager || (me != null && me == Login(people, id)))
                throw new InvalidOperationException(Name(people, id) + " is the same person as the manager and cannot approve their own time. [AM-" + Build + "]");
        var graph = Graph(edges, people);
        foreach (int id in added)
        {
            var path = PathTo(graph, id, manager);
            if (path == null) continue;
            throw new InvalidOperationException("Not saved: " + Name(people, id) + " already approves " + Name(people, manager) + "'s time (" +
                String.Join(" \u2192 ", path.Select(x => Name(people, x)).ToArray()) + "). Assigning them under " + Name(people, manager) +
                " would make a circular approval chain. Open Assign User to Manager for " + Name(people, id) + " and unassign " + Name(people, manager) + " first. [AM-" + Build + "]");
        }
    }
    public static void Save(int business,int manager,IEnumerable<int> selected,string expected)
    {
        var ids=selected.Distinct().OrderBy(x=>x).ToArray();
        using(var c=AnyminuteTimesheetRepair.Open()) using(var tx=c.BeginTransaction(IsolationLevel.Serializable))
        {
            int actor=RequireOwner(c,tx,business);
            var people=People(c,tx,business);
            if (people.Select("EmployeeID="+manager+" AND UserTypeId=2").Length!=1)
                throw new InvalidOperationException("Select an active manager in this business.");
            foreach(int id in ids)
                if(people.Select("EmployeeID="+id).Length!=1) throw new InvalidOperationException("A selected user is inactive, outside this business, or has an ambiguous role. Refresh the page.");
            var old=Edges(c,tx,business);
            if(String.IsNullOrEmpty(expected) || expected!=Fingerprint(old))
                throw new InvalidOperationException("Assignments changed since you opened this page. Refresh and try again.");
            var next=old.Rows.Cast<DataRow>().Where(r=>Convert.ToInt32(r["ManagerID"])!=manager)
                .Select(r=>Tuple.Create(Convert.ToInt32(r["ManagerID"]),Convert.ToInt32(r["EmpId"]))).ToList();
            next.AddRange(ids.Select(id=>Tuple.Create(manager,id)));
            var previousSet=new HashSet<int>(old.Rows.Cast<DataRow>().Where(r=>Convert.ToInt32(r["ManagerID"])==manager).Select(r=>Convert.ToInt32(r["EmpId"])));
            var added=ids.Where(id=>!previousSet.Contains(id)).ToList();
            // A manager has one designated approver per business (counting current managers' rows only; stale rows are ignored).
            var currentManagers=new HashSet<int>(people.Select("UserTypeId=2").Select(r=>Convert.ToInt32(r["EmployeeID"])));
            foreach(int id in added.Where(id=>currentManagers.Contains(id)))
            {
                var other=next.Where(edge=>edge.Item2==id&&edge.Item1!=manager&&edge.Item1!=id&&currentManagers.Contains(edge.Item1)).Select(edge=>edge.Item1).FirstOrDefault();
                if(other!=0) throw new InvalidOperationException(Convert.ToString(people.Select("EmployeeID="+id)[0]["empname"])+" already has an approver ("+Convert.ToString(people.Select("EmployeeID="+other)[0]["empname"])+"). Unassign them there first. [AM-"+Build+"]");
            }
            ValidateManager(next, manager, people, added);
            var previous=old.Rows.Cast<DataRow>().Where(r=>Convert.ToInt32(r["ManagerID"])==manager).Select(r=>Convert.ToInt32(r["EmpId"])).Distinct().OrderBy(x=>x).ToArray();
            if(previous.SequenceEqual(ids)) { tx.Commit(); return; }
            AnyminuteTimesheetRepair.Execute(c,tx,"DELETE AssgineManager WHERE BusinessID=@b AND ManagerID=@m","@b",business,"@m",manager);
            foreach(int id in ids) AnyminuteTimesheetRepair.Execute(c,tx,@"INSERT AssgineManager (BusinessID,BusinessName,ManagerID,MangerName,EmpId,EmpName)
                VALUES (@b,dbo.FN_DESC('AddBusiness',@b),@m,dbo.FN_DESC('AddEmployee',@m),@e,dbo.FN_DESC('AddEmployee',@e))", "@b",business,"@m",manager,"@e",id);
            AnyminuteTimesheetRepair.Execute(c,tx,@"INSERT AnyminuteApprovalAudit (BusinessID,ActorID,EmployeeID,Action,BeforeValue,AfterValue,RecordedAtUTC)
                VALUES (@b,@a,@m,'Assignments',@old,@new,SYSUTCDATETIME())", "@b",business,"@a",actor,"@m",manager,
                "@old",String.Join(",",previous),"@new",String.Join(",",ids));
            tx.Commit();
        }
    }
}
