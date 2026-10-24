using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ResetPassord : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    UserAccController UActrl = new UserAccController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["empid"] != null)
            {
                if (Request.QueryString["LoginType"] == null)
                {
                    string name;
                    name = comm.GetNameFromId("AddEmployee", int.Parse(Request.QueryString["empid"].ToString()));
                    lblname.Text = name;
                    Label1.Text = name;
                }
                else
                {
                    if (Request.QueryString["LoginType"].ToString() == "Acc")
                    {
                        DataTable name;
                        name = comm.GetAllDataFromTable("MotherOrg", "MotherOrgName", "MotherOrgID='" + int.Parse(Request.QueryString["empid"].ToString()) + "'", "");
                        lblname.Text = name.Rows[0]["MotherOrgName"].ToString();
                        Label1.Text = name.Rows[0]["MotherOrgName"].ToString(); ;
                    }
                    if (Request.QueryString["LoginType"].ToString() == "Support")
                    {
                        DataTable name;
                        name = comm.GetAllDataFromTable("SupportAdmin", "Title+' '+FirstName+' '+MiddleName+' '+LastName as name", "SupportAdminId='" + int.Parse(Request.QueryString["empid"].ToString()) + "'", "");
                        lblname.Text = name.Rows[0]["name"].ToString();
                        Label1.Text = name.Rows[0]["name"].ToString(); ;
                    }
                }

            }
        }

    }

    protected void btnReset_Click(object sender, EventArgs e)
    {
        if (txtNewPass.Text.Trim() != txtConfirmPass.Text.Trim())
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm password does not match with password', 'error');", true);
            return;
        }
        int len = txtNewPass.Text.Trim().Length;
        if (len < 8)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Password Must Contain atleast 8 characters', 'error');", true);
            return;
        }
        string LoginType;
        if (Request.QueryString["LoginType"] == null)
        {
            LoginType = "EMP";
        }
        else
        {
            LoginType = Request.QueryString["LoginType"].ToString();
        }

        UActrl.Sp_ReSetPassword(int.Parse(Request.QueryString["empid"].ToString()), txtConfirmPass.Text, LoginType);
        //Response.Redirect("Login.aspx", false);   //Commented after hemal code
        ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Password Changed Successfully', 'success').then((value) => {window.location = 'Login.aspx';});", true);
    }

    protected void Submit_Click(object sender, EventArgs e)
    {
        if (txtOldPassword.Text.Trim() == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Enter old password', 'error');", true);
            return;
        }
        string LoginType;
        DataTable oldcheck;
        if (Request.QueryString["LoginType"] == null)
        {
            LoginType = "EMP";
        }
        else
        {
            LoginType = Request.QueryString["LoginType"].ToString();
        }
        if (LoginType == "EMP")
        {
            oldcheck = comm.GetAllDataFromTable("UserAcc", "Password", "EmployeeId = '" + int.Parse(Request.QueryString["empid"].ToString()) + "' and convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))='" + txtOldPassword.Text + "'", "");
        }
        else if (LoginType == "Acc")
        {
            oldcheck = comm.GetAllDataFromTable("UserAcc", "Password", "MotherOrgid = '" + int.Parse(Request.QueryString["empid"].ToString()) + "' and convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))='" + txtOldPassword.Text + "'", "");
        }
        else
        {
            oldcheck = comm.GetAllDataFromTable("UserAcc", "Password", "EmployeeId = '" + int.Parse(Request.QueryString["empid"].ToString()) + "' and convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))='" + txtOldPassword.Text + "' and UserType='SupportAdmin'", "");
        }
        if (oldcheck.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Old password does not match', 'error');", true);
            return;
        }
        if (oldcheck.Rows.Count > 0)
        {
            LoginPanel.Visible = true;
            OldPasswordPanel.Visible = false;
        }
    }
}