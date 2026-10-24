using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class DatabaseBackup : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Email = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBackup_Click(object sender, EventArgs e)
    {
        string file = HttpContext.Current.Request.MapPath("~/"+ ConfigurationManager.AppSettings["FileLocation"].ToString()+ "");

        string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(strcon);

        string server = builder.DataSource;
        string database = builder.InitialCatalog;
        Comm.GetDatabaseBackup(file, database);
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Database Back up Döne with file name "+ DateTime.Now.ToString("yyyy-MM-dd") + ".bak', 'success');", true);
        Email.SendBackUpMail();
    }
}