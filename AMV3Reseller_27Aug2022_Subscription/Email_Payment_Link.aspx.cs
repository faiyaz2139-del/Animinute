using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class Email_Payment_Link : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();

    protected void Page_Load(object sender, EventArgs e)
    {       
        if (!Page.IsPostBack)
        {
            GetAllEmailPaymentLink();
        }
    }

    public void GetAllEmailPaymentLink()
    {
        DataTable dt_Payment_Link = comm.GetAllDataFromTable("Email_Payment_Link with (Nolock)", "Name,Email,Payment_Link,Payment_Amount,Reseller_Or_Subscriber", "", "1 desc");
        GridView_OnlinePaymentList.DataSource = dt_Payment_Link;
        GridView_OnlinePaymentList.DataBind();
    }

    protected void GridView_OnlinePaymentList_PreRender(object sender, EventArgs e)
    {
        if (GridView_OnlinePaymentList.Rows.Count > 0)
        {
            GridView_OnlinePaymentList.UseAccessibleHeader = true;
            GridView_OnlinePaymentList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}