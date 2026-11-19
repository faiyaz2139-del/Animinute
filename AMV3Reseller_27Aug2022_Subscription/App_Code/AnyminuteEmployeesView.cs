using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

public sealed class AnyminuteEmployeeAssignmentView
{
    public int BusinessId {get;set;}
    public int EmployeeId {get;set;}
    public int RoleId {get;set;}
    public string Business {get;set;}
    public string Role {get;set;}
    public string Type {get;set;}
    public string Rate {get;set;}
    public string Status {get;set;}
    public string Name {get;set;}
    public string Email {get;set;}
    public string Phone {get;set;}
    public string Mobile {get;set;}
    public int UserId {get;set;}
    public bool Manage {get;set;}
}
public sealed class AnyminuteEmployeeCardView
{
    public string Key {get;set;}
    public List<AnyminuteEmployeeAssignmentView> Assignments {get;set;}
    public AnyminuteEmployeeAssignmentView First {get{return Assignments.OrderBy(a=>a.BusinessId).ThenBy(a=>a.EmployeeId).First();}}
    public int EmployeeId {get{return First.EmployeeId;}}
    public string Name {get{return First.Name;}}
    public string Email {get{return First.Email;}}
    public string Phone {get{return First.Phone;}}
    public string Initials {get {var words=(Name??"").Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);return words.Length==0?"?":String.Concat(words[0][0],words.Length>1?words[words.Length-1][0]:' ').Trim().ToUpperInvariant();}}
    public int BusinessCount {get{return Assignments.Select(a=>a.BusinessId).Distinct().Count();}}
    public string Roles {get{return String.Join(", ",RoleNames);}}
    public string[] RoleNames {get{return Assignments.Select(a=>a.Role).Where(x=>!String.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x=>x).ToArray();}}
    public string BusinessLabel {get{int count=BusinessCount;return count+(count==1?" Business":" Businesses");}}
    // CSS-safe status token for the badge colour (display text stays in Status).
    public string StatusCss {get{string s=Status;return s=="Active"?"Active":s=="On Leave"?"OnLeave":s=="Mixed"?"Mixed":"Inactive";}}
    public string Type {get{return String.Join(", ",Assignments.Select(a=>a.Type).Distinct());}}
    public bool Active {get{return Assignments.Any(a=>a.Status=="Active");}}
    public string Status {get{var values=Assignments.Select(a=>a.Status).Distinct().ToArray();return values.Length==1?values[0]:"Mixed";}}
    public string Rate {get{var values=Assignments.Select(a=>a.Rate).Where(x=>!String.IsNullOrWhiteSpace(x)).Distinct().ToArray();return values.Length==0?"Not recorded":values.Length==1?values[0]:"Varies by assignment";}}
    public bool Manage {get{return Assignments.All(a=>a.Manage);}}
    public string ProfileUrl {get{return "EmployeeDetails.aspx?EmployeeId="+EmployeeId;}}
    public string AssignmentUrl {get{return ProfileUrl+"&tab=assignments";}}
}
// Read model only. It never grants roles, changes login state or rewrites legacy assignments.
public static class AnyminuteEmployeesView
{
    public static string Status(object status, object active)
    {
        string value=Convert.ToString(status).Trim();
        if(value.Equals("Active",StringComparison.OrdinalIgnoreCase))return "Active";
        if(value.Equals("On Leave",StringComparison.OrdinalIgnoreCase))return "On Leave";
        if(value.Equals("Terminated",StringComparison.OrdinalIgnoreCase))return "Terminated";
        return AnyminuteEmployeeRepair.Active(status,active)?"Active":"Inactive";
    }
    public static List<AnyminuteEmployeeCardView> Load()
    {
        AnyminuteDirectoryScope.RequireDirectoryRole();
        var assignments=new List<AnyminuteEmployeeAssignmentView>();
        using(var c=AnyminuteTimesheetRepair.Open())
        foreach(DataRow b in AnyminuteDirectoryScope.Businesses().Rows)
        {
            int business=Convert.ToInt32(b["BusinessID"]),actor;
            int role=AnyminuteDirectoryScope.BusinessRole(c,business,out actor);
            foreach(DataRow row in AnyminuteDirectoryScope.Users(business).Rows)
            {
                int employee=Convert.ToInt32(row["EmployeeID"]);
                var mappings=AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT USERID FROM MappingTable WHERE BusinessID=@b AND EmployeeID=@e AND USERID IS NOT NULL","@b",business,"@e",employee);
                int user=mappings.Rows.Count==1?Convert.ToInt32(mappings.Rows[0][0]):0;
                assignments.Add(new AnyminuteEmployeeAssignmentView {
                    BusinessId=business, EmployeeId=employee, RoleId=Convert.ToInt32(row["UserTypeId"]), UserId=user,
                    Business=Convert.ToString(b["CompanyName"]), Role=AnyminuteEmployeeEditor.RoleDisplay(row["UserRoleName"]),
                    Type=Convert.ToString(row["Employee_Type"]), Rate=Convert.ToString(row["Rate"]),
                    Status=Status(row["Employee_Status"],row["IsActive"]), Name=Convert.ToString(row["EmpName"]),
                    Email=Convert.ToString(row["Email"]),Phone=Convert.ToString(row["Phone"]),Mobile=Convert.ToString(row["Mobile"]),Manage=role==1
                });
            }
        }
        return Group(assignments);
    }
    public static List<AnyminuteEmployeeCardView> Group(IEnumerable<AnyminuteEmployeeAssignmentView> assignments)
    {
        // Account identity only; matching email/name must NEVER merge unrelated employees.
        return assignments.GroupBy(a=>a.UserId>0?"U"+a.UserId:"E"+a.EmployeeId)
            .Select(g=>new AnyminuteEmployeeCardView {Key=g.Key,Assignments=g.GroupBy(a=>new{a.BusinessId,a.EmployeeId,a.RoleId}).Select(x=>x.First()).ToList()})
            .OrderBy(e=>e.Name,StringComparer.OrdinalIgnoreCase).ToList();
    }
    public static AnyminuteEmployeeCardView RequireEmployee(int employee)
    {
        var cards=Load().Where(e=>e.Assignments.Any(a=>a.EmployeeId==employee)).ToArray();
        if(cards.Length!=1)throw new HttpException(403,"This employee is outside your directory access or has an ambiguous account identity.");return cards[0];
    }
    public static List<AnyminuteEmployeeCardView> Filter(IEnumerable<AnyminuteEmployeeCardView> source,int business,int role,string status,string search)
    {
        string term=(search??"").Trim();
        return source.Where(card=>card.Assignments.Any(a=>
            (business==0||a.BusinessId==business)&&(role==0||a.RoleId==role||(AnyminuteEmployeeEditor.IsManagerRole(role)&&AnyminuteEmployeeEditor.IsManagerRole(a.RoleId)))&&(status=="All"||a.Status==status)&&
            (term.Length==0||new[]{a.Name,a.Email,a.Phone,a.Mobile}.Any(value=>(value??"").IndexOf(term,StringComparison.OrdinalIgnoreCase)>=0)))).ToList();
    }
    public static string EditUrl(AnyminuteEmployeeAssignmentView a,string tab)
    {
        if(!a.Manage)throw new HttpException(403,"Owner access is required to edit this assignment.");
        using(var c=AnyminuteTimesheetRepair.Open())
        {
            AnyminuteApprovalAssignments.RequireOwner(c,null,a.BusinessId);
            AnyminuteFormAccess.TargetAccount(a.BusinessId,a.EmployeeId);
            foreach(DataRow b in AnyminuteTimesheetRepair.Query(c,null,"SELECT DISTINCT BusinessID FROM AssginBussiness WHERE EmployeeID=@e AND BusinessID IS NOT NULL","@e",a.EmployeeId).Rows)
                AnyminuteApprovalAssignments.RequireOwner(c,null,Convert.ToInt32(b[0]));
        }
        string section=tab=="pay"?"#ratedetils":tab=="projects"?"#project":"#home";
        return "AddEditRole_Add_Employee.aspx?BusinessScoped=1&BussEditId="+a.BusinessId+"&EmployeeIDEdit="+a.EmployeeId+"&EmployeeSection="+(tab=="pay"?"pay":tab=="projects"?"projects":"profile")+section;
    }
}
