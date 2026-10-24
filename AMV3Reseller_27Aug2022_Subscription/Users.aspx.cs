using System;
using System.Data;
using System.Linq;
using System.Globalization;
using System.Web;
using System.Web.UI.WebControls;
public partial class AnyminuteEmployeesDirectory : System.Web.UI.Page
{
    private int Index {get{return Convert.ToInt32(ViewState["Index"]??0);}set{ViewState["Index"]=Math.Max(0,value);}}
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
            Bind();
        }
    }
    protected void Filter_Click(object sender,EventArgs e){Index=0;Bind();}
    protected void Clear_Click(object sender,EventArgs e){Search.Text="";Business.SelectedIndex=Role.SelectedIndex=Status.SelectedIndex=Sort.SelectedIndex=0;Index=0;Bind();}
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
        if(Sort.SelectedValue=="Businesses")filtered=filtered.OrderByDescending(x=>x.BusinessCount).ThenBy(x=>x.Name).ToList();
        else if(Sort.SelectedValue=="Status")filtered=filtered.OrderBy(x=>x.Status).ThenBy(x=>x.Name).ToList();
        else if(Sort.SelectedValue!="Name")throw new HttpException(400,"Invalid sort.");
        int pages=Math.Max(1,(filtered.Count+size-1)/size);if(Index>=pages)Index=pages-1;
        Rows.DataSource=filtered.Skip(Index*size).Take(size).ToList();Rows.DataBind();Empty.Visible=filtered.Count==0;
        int first=filtered.Count==0?0:Index*size+1,last=Math.Min((Index+1)*size,filtered.Count);
        Count.Text=String.Format(CultureInfo.InvariantCulture,"Showing {0} to {1} of {2} matching employees ({3} accessible)",first,last,filtered.Count,all.Count);
        PageNumber.Text=String.Format(CultureInfo.InvariantCulture,"Page {0} of {1}",Index+1,pages);Previous.Enabled=Index>0;Next.Enabled=Index+1<pages;
    }
}
