using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class SuperAdminProfile : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    SuperAdminController SActrl = new SuperAdminController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            GetDetails();
        }
    }

    public void GetDetails()
    {
        DataTable dt = Comm.GetAllDataFromTable("UserAcc", "Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password", "UserType='SuperAdmin'", "");
        txtemail.Text = dt.Rows[0]["Email"].ToString();
        txtPassword.Text= dt.Rows[0]["Password"].ToString();
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        if (txtemail.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Email!', 'error');", true);
            return;
        }
        if (txtPassword.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Passwords!', 'error');", true);
            return;
        }

        int result = 0;
        result = SActrl.UpdateSuperAdmin(txtemail.Text, txtPassword.Text);
        if(result>0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Profile Updated!', 'success');", true);
        }
    }
}