using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EmailSetting : System.Web.UI.Page
{
    EmailSettingEntity EsE = new EmailSettingEntity();
    EmailSettingController EsCtrl = new EmailSettingController();
    CommanClass Comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetDetail();
        }

    }

    public void GetDetail()
    {
        DataTable dt = new DataTable();
        dt = Comm.GetAllDataFromTable("EmailSeting", "*", "1=1", "");
        if (dt.Rows.Count > 0)
        {
            btnsave.Text = "Update";
            txtSenderEmail.Text = dt.Rows[0]["SenderEmail"].ToString();
            txtPassword.Text = dt.Rows[0]["Password"].ToString();
            txtHost.Text = dt.Rows[0]["HostName"].ToString();
            txtPortNo.Text = dt.Rows[0]["PortNo"].ToString();
            hfdEId.Value = dt.Rows[0]["EID"].ToString();
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        int a = 0;
        if (btnsave.Text != "Update")
        {
            EsE.SenderEmail = txtSenderEmail.Text;
            EsE.Password = txtPassword.Text;
            EsE.HostName = txtHost.Text;
            EsE.PortNo = int.Parse(txtPortNo.Text);
            a = EsCtrl.InsertEmailSetting(EsE);
            if (a > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Record Save Successfully', 'success');", true);
            }
        }
        else
        {
            EsE.EID = int.Parse(hfdEId.Value);
            EsE.SenderEmail = txtSenderEmail.Text;
            EsE.Password = txtPassword.Text;
            EsE.HostName = txtHost.Text;
            EsE.PortNo = int.Parse(txtPortNo.Text);
            a = EsCtrl.UpdateEmailSetting(EsE);
            if (a > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Record Save Successfully', 'success');", true);
            }
        }

    }
}