using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Collections.Generic;
public static class AnyminuteModernScope
{
 public static void Login()
 {
  var h=HttpContext.Current;int u,a;
  if(h.Session==null||!Int32.TryParse(Convert.ToString(h.Session["UserId"]),out u)||!Int32.TryParse(Convert.ToString(h.Session["EmployeeId"]),out a))
   throw new HttpException(401,"Sign in to continue.");
  using(var c=AnyminuteTimesheetRepair.Open())
   if(AnyminuteTimesheetRepair.Query(c,null,"SELECT 1 FROM UserAcc WHERE UserId=@u AND EmployeeId=@a","@u",u,"@a",a).Rows.Count!=1)
    throw new HttpException(403,"Login identity could not be verified.");
 }
 public static DataTable Businesses()
 {
  Login();var h=HttpContext.Current;
  using(var c=AnyminuteTimesheetRepair.Open()) return AnyminuteTimesheetRepair.Query(c,null,@"SELECT DISTINCT b.BusinessID,b.CompanyName,b.PrimaryContactName,b.Status
   FROM AddBusiness b WHERE b.Status<>2 AND EXISTS (SELECT 1 FROM AssginBussiness a
    WHERE a.BusinessID=b.BusinessID AND ((a.EmployeeID=@a AND a.UserTypeId=1) OR EXISTS
     (SELECT 1 FROM MappingTable m WHERE m.USERID=@u AND m.BusinessID=a.BusinessID AND m.EmployeeID=a.EmployeeID AND m.UserTypeId=a.UserTypeId)))
   ORDER BY b.CompanyName","@u",Convert.ToInt32(h.Session["UserId"]),"@a",Convert.ToInt32(h.Session["EmployeeId"]));
 }
 public static int Actor(SqlConnection c,SqlTransaction tx,int business,out int role)
 {int actor=Convert.ToInt32(HttpContext.Current.Session["EmployeeId"]);role=Convert.ToInt32(HttpContext.Current.Session["UserTypeid"]);
  AnyminuteTimesheetRepair.ResolveBusinessActor(c,tx,ref actor,ref role,business);return actor;}
 public static bool Owns(int business)
 {using(var c=AnyminuteTimesheetRepair.Open()) {try {AnyminuteApprovalAssignments.RequireOwner(c,null,business);return true;}catch(InvalidOperationException){return false;}}}
 public static DataTable Users(int business)
 {
  using(var c=AnyminuteTimesheetRepair.Open())
  {
   int role,actor=Actor(c,null,business,out role);
   var dt=AnyminuteTimesheetRepair.Query(c,null,@"SELECT DISTINCT a.EmployeeID,a.BusinessID,a.UserTypeId,
    dbo.FN_DESC('AddEmployee',a.EmployeeID) AS EmpName,u.UserType AS Role,e.EMPLOYEETYPE,e.IsActive,
    CASE WHEN @role IN(1,5) THEN TRY_CONVERT(decimal(18,4),r.Rate) ELSE NULL END AS Rate,
    CASE WHEN @role IN(1,5) THEN r.RateType ELSE NULL END AS RateType,e.Effective_Date
    FROM AssginBussiness a JOIN AddEmployee e ON e.EmployeeID=a.EmployeeID LEFT JOIN UserType u ON u.User_TypeId=a.UserTypeId
    OUTER APPLY (SELECT TOP(1) Rate,RateType FROM RateDetails WHERE EmployeeId=a.EmployeeID AND effectivedate<=GETDATE() ORDER BY effectivedate DESC,RateId DESC) r
    WHERE a.BusinessID=@b AND (@role IN(1,5) OR a.EmployeeID=@actor OR EXISTS
     (SELECT 1 FROM AssgineManager m WHERE m.BusinessID=@b AND m.ManagerID=@actor AND m.EmpId=a.EmployeeID))
    ORDER BY EmpName","@b",business,"@actor",actor,"@role",role);
   return AnyminuteEmployeeRepair.Enrich(dt);
  }
 }
 public static DataTable Projects(int business)
 {
  using(var c=AnyminuteTimesheetRepair.Open())
  {int role,actor=Actor(c,null,business,out role);
   return AnyminuteTimesheetRepair.Query(c,null,@"SELECT ProjectID,ProjectName,StartDate,EndDate FROM AddProject p WHERE BusinessID=@b AND ISNULL(Status,'')<>'D'
    AND (@r IN(1,5) OR EXISTS(SELECT 1 FROM ASSIGNPROJECT x WHERE x.PROJECTID=p.ProjectID AND x.BUSINESSID=@b AND
     (x.EMPLOYEEID=@a OR EXISTS(SELECT 1 FROM AssgineManager m WHERE m.BusinessID=@b AND m.ManagerID=@a AND m.EmpId=x.EMPLOYEEID)))) ORDER BY ProjectName","@b",business,"@a",actor,"@r",role);}
 }
 public static int Pending()
 {
  int total=0;foreach(DataRow business in Businesses().Rows)
   using(var c=AnyminuteTimesheetRepair.Open())
   {int role,b=Convert.ToInt32(business["BusinessID"]),actor=Actor(c,null,b,out role);
    var dt=AnyminuteTimesheetRepair.Query(c,null,@"SELECT COUNT(*) FROM AnyminuteWeek w WHERE BusinessID=@b AND State='Submitted'
      AND (@r IN(1,5) OR EXISTS (SELECT 1 FROM AssgineManager m WHERE m.BusinessID=w.BusinessID AND m.ManagerID=@a AND m.EmpId=w.EmployeeID))","@b",b,"@a",actor,"@r",role);total+=Convert.ToInt32(dt.Rows[0][0]);}
  return total;
 }
 public static string Url(string page,int business,int employee)
 {return page+"?BusinessId="+business+"&EmployeeId="+employee;}
}
