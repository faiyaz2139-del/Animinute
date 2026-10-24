using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterWithoutMenu : System.Web.UI.MasterPage
{
    string _usertype;
    CommanClass comm = new CommanClass();
    DataTable MenusDomain = new DataTable();
    DataTable AllMenu = new DataTable();
    CreateLinkClass cl = new CreateLinkClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.AppendHeader("Refresh", Convert.ToString(Session.Timeout * 60) + "; URL=https://anyminute.net/");
    }
}