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
            dbo.FN_DESC('AddEmployee',a.EmployeeID) AS empname, m.UserTypeId
            FROM AddEmployee a JOIN AssginBussiness m ON m.EmployeeID=a.EmployeeID
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
    public static void ValidateGraph(IEnumerable<Tuple<int,int>> edges)
    {
        var graph = new Dictionary<int,List<int>>();
        foreach (var edge in edges)
        {
            if (edge.Item1==edge.Item2) throw new InvalidOperationException("A manager cannot approve their own time.");
            if (!graph.ContainsKey(edge.Item1)) graph[edge.Item1]=new List<int>();
            graph[edge.Item1].Add(edge.Item2);
        }
        var visited=new HashSet<int>(); var path=new HashSet<int>();
        foreach (int node in graph.Keys) Visit(node,graph,visited,path);
    }
    static void Visit(int node, Dictionary<int,List<int>> graph, HashSet<int> visited, HashSet<int> path)
    {
        if (path.Contains(node)) throw new InvalidOperationException("This assignment creates a circular approval chain.");
        if (visited.Contains(node)) return;
        path.Add(node);
        if (graph.ContainsKey(node)) foreach(int child in graph[node]) Visit(child,graph,visited,path);
        path.Remove(node); visited.Add(node);
    }
    // A chain is circular only when one of the manager's assigned users can reach the manager again through current,
    // active managers' assignments (UserTypeId 2 in this business). Stale rows of former managers do not approve anything.
    public static void ValidateManager(IEnumerable<Tuple<int,int>> edges, int manager, DataTable people)
    {
        var managers = new HashSet<int>(people.Select("UserTypeId=2").Select(r => Convert.ToInt32(r["EmployeeID"])));
        var graph = new Dictionary<int,List<int>>();
        foreach (var edge in edges)
        {
            if (edge.Item1 == manager && edge.Item2 == manager) throw new InvalidOperationException("A manager cannot approve their own time.");
            if (edge.Item1 == edge.Item2 || !managers.Contains(edge.Item1)) continue;
            List<int> list; if (!graph.TryGetValue(edge.Item1, out list)) graph[edge.Item1] = list = new List<int>();
            list.Add(edge.Item2);
        }
        List<int> start; if (!graph.TryGetValue(manager, out start)) return;
        var seen = new HashSet<int>(); var stack = new Stack<int>(start);
        while (stack.Count > 0)
        {
            int node = stack.Pop();
            if (node == manager)
                throw new InvalidOperationException("This assignment creates a circular approval chain: one of the selected users already approves (directly or through another manager) the time of this manager. Remove that assignment first.");
            if (!seen.Add(node)) continue;
            List<int> next; if (graph.TryGetValue(node, out next)) foreach (int n in next) stack.Push(n);
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
            // A manager has one designated approver per business. Existing employee mappings are preserved.
            foreach(int id in ids.Where(id=>people.Select("EmployeeID="+id+" AND UserTypeId=2").Length==1))
                if(next.Count(edge=>edge.Item2==id)>1) throw new InvalidOperationException("This manager already has an approver. Remove that assignment before reassigning.");
            // v21 (Pandey #6): only a cycle through THIS manager blocks the save. Before, any legacy loop or self-row anywhere
            // in the business (or rows left by users who are no longer managers) rejected every assignment.
            ValidateManager(next, manager, people);
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
