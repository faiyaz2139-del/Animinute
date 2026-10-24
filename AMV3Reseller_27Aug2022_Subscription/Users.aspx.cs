using System;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteEmployeesDirectory : System.Web.UI.Page
{
    private int Index {get{return Convert.ToInt32(ViewState["Index"]??0);}set{ViewState["Index"]=Math.Max(0,value);}}
    private string SortKey {get{return Convert.ToString(ViewState["Sort"]??"Name");}set{ViewState["Sort"]=value;}}
    private bool SortDescending {get{return Convert.ToBoolean(ViewState["SortDesc"]??false);}set{ViewState["SortDesc"]=value;}}
    protected int CurrentPage {get{return Index+1;}}
    protected void Sort_Command(object sender,CommandEventArgs e){string key=Convert.ToString(e.CommandArgument);SortDescending=key==SortKey&&!SortDescending;SortKey=key;Index=0;Bind();}
    protected void Page_Command(object sender,CommandEventArgs e){int page;if(!Int32.TryParse(Convert.ToString(e.CommandArgument),out page))throw new HttpException(400,"Invalid page.");Index=page-1;Bind();}
    protected static string RateText(object rate){string r=Convert.ToString(rate);return r.Replace("/"," / ");}
    protected static string AvatarCss(object name){string n=Convert.ToString(name);int h=0;foreach(char ch in n)h=(h*31+ch)&0xffff;return "am-av-"+(h%6);}
    public static string RoleCss(object role){string r=Convert.ToString(role).ToLowerInvariant();
        if(r.Contains("admin"))return "am-role-admin";if(r.Contains("manager")||r.Contains("supervisor"))return "am-role-manager";
        if(r.Contains("accountant"))return "am-role-accountant";if(r.Contains("employee")||r.Contains("staff"))return "am-role-employee";return "am-role-other";}
    protected void Page_Init(object sender,EventArgs e){AnyminuteDirectoryScope.RequireDirectoryRole();ViewStateUserKey=Session.SessionID;}
    protected void Page_Load(object sender,EventArgs e)
    {
        if(!IsPostBack)
        {
            Business.Items.Add(new ListItem("All businesses","0"));Role.Items.Add(new ListItem("All roles","0"));
            var cards=AnyminuteEmployeesView.Load();
            foreach(DataRow business in AnyminuteDirectoryScope.Businesses().Rows)Business.Items.Add(new ListItem(Convert.ToString(business["CompanyName"]),Convert.ToString(business["BusinessID"])));
            foreach(var group in cards.SelectMany(x=>x.Assignments).GroupBy(x=>x.RoleId).OrderBy(x=>x.First().Role))Role.Items.Add(new ListItem(group.First().Role,group.Key.ToString()));
            string selected=Request.QueryString["BusinessId"];
            if(selected!=null){if(Business.Items.FindByValue(selected)==null)throw new HttpException(403,"Business outside directory access.");Business.SelectedValue=selected;}
            string flash=AnyminuteFlash.Take("employees");
            if(flash!=null){FlashText.Text=HttpUtility.HtmlEncode(flash);Flash.Visible=true;}
            Bind();
        }
    }
    protected void Filter_Click(object sender,EventArgs e){Index=0;Bind();}
    protected void Clear_Click(object sender,EventArgs e){Search.Text="";Business.SelectedIndex=Role.SelectedIndex=Status.SelectedIndex=0;SortKey="Name";SortDescending=false;Index=0;Bind();}
    protected void Previous_Click(object sender,EventArgs e){Index--;Bind();}
    protected void Next_Click(object sender,EventArgs e){Index++;Bind();}
    private void Bind()
    {
        var all=AnyminuteEmployeesView.Load();
        TotalEmployees.Text=all.Count.ToString();TotalAssignments.Text=all.Sum(x=>x.BusinessCount).ToString();
        ActiveEmployees.Text=all.Count(x=>x.Active).ToString();InactiveEmployees.Text=all.Count(x=>!x.Active).ToString();
        AddEmployee.Visible=all.Any(x=>x.Manage);
        if(!AddEmployee.Visible)foreach(DataRow b in AnyminuteModernScope.Businesses().Rows)if(AnyminuteModernScope.Owns(Convert.ToInt32(b["BusinessID"])))AddEmployee.Visible=true;
        int business,role,size;
        if(!Int32.TryParse(Business.SelectedValue,out business)||!Int32.TryParse(Role.SelectedValue,out role)||!Int32.TryParse(PageSize.SelectedValue,out size)||!(size==10||size==25||size==50))throw new HttpException(400,"Invalid directory filters.");
        if(business!=0&&!AnyminuteDirectoryScope.Businesses().Rows.Cast<DataRow>().Any(b=>Convert.ToInt32(b["BusinessID"])==business))throw new HttpException(403,"Business outside current access.");
        var filtered=AnyminuteEmployeesView.Filter(all,business,role,Status.SelectedValue,Search.Text);
        IOrderedEnumerable<AnyminuteEmployeeCardView> ordered;
        switch(SortKey)
        {
            case "Name": ordered=SortDescending?filtered.OrderByDescending(x=>x.Name):filtered.OrderBy(x=>x.Name);break;
            case "Businesses": ordered=SortDescending?filtered.OrderBy(x=>x.BusinessCount):filtered.OrderByDescending(x=>x.BusinessCount);break;
            case "Roles": ordered=SortDescending?filtered.OrderByDescending(x=>x.Roles):filtered.OrderBy(x=>x.Roles);break;
            case "Rate": ordered=SortDescending?filtered.OrderByDescending(x=>x.Rate):filtered.OrderBy(x=>x.Rate);break;
            case "Status": ordered=SortDescending?filtered.OrderByDescending(x=>x.Status):filtered.OrderBy(x=>x.Status);break;
            default: throw new HttpException(400,"Invalid sort.");
        }
        filtered=ordered.ThenBy(x=>x.Name).ToList();
        int pages=Math.Max(1,(filtered.Count+size-1)/size);if(Index>=pages)Index=pages-1;
        Rows.DataSource=filtered.Skip(Index*size).Take(size).ToList();Rows.DataBind();Empty.Visible=filtered.Count==0;
        int first=filtered.Count==0?0:Index*size+1,last=Math.Min((Index+1)*size,filtered.Count);
        Count.Text=String.Format(CultureInfo.InvariantCulture,"Showing {0} to {1} of {2} employees",first,last,filtered.Count);
        PageNumber.Text=String.Format(CultureInfo.InvariantCulture,"Page {0} of {1}",Index+1,pages);Previous.Enabled=Index>0;Next.Enabled=Index+1<pages;
        int start=Math.Max(1,Math.Min(Index+1-2,pages-4)),end=Math.Min(pages,start+4);
        Pages.DataSource=Enumerable.Range(start,end-start+1).ToList();Pages.DataBind();
    }
}
