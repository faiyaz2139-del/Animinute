using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupportAdminProfile : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    SupportAdminCtrl SuppCtrl = new SupportAdminCtrl();
    SupportAdminEntity SuppEntity = new SupportAdminEntity();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            GetDetails(int.Parse(Session["EmployeeId"].ToString()));
        }
    }

    public void GetDetails(int AccId)
    {
        DataTable dt = Comm.GetAllDataFromTable("SupportAdmin", "*", "SupportAdminId='" + Session["EmployeeId"].ToString() + "'", "");
        ddlLegalName.SelectedValue = dt.Rows[0]["Title"].ToString();
        txtFirstName.Text = dt.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
        txtLastName.Text= dt.Rows[0]["LastName"].ToString();
        if(dt.Rows[0]["DOB"].ToString()!="")
        {
            txtdob.Text = DateTime.Parse(dt.Rows[0]["DOB"].ToString()).ToString("yyyy-MM-dd");
        }
       
        ddlgender.SelectedValue = dt.Rows[0]["Gender"].ToString();
        txtMainPhone.Text= dt.Rows[0]["Contact"].ToString();
        txt_mobilenumber.Text= dt.Rows[0]["Mobile"].ToString();
        txt_city.Text= dt.Rows[0]["CITY"].ToString();
        txt_province.Text = dt.Rows[0]["Province"].ToString();
        txtMainEmail.Text= dt.Rows[0]["Email"].ToString();
        hfdemail.Value= dt.Rows[0]["Email"].ToString();
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        if(txtMainEmail.Text!= hfdemail.Value)
        {
            if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm Email Not Match', 'warning');", true);
                return;

            }
        }
        
        int result = 0;

        SuppEntity.SupportAdminId = int.Parse(Session["EmployeeId"].ToString());
        SuppEntity.Title = ddlLegalName.SelectedValue;
        SuppEntity.FirstName = txtFirstName.Text;
        SuppEntity.MiddleName = txtMiddleName.Text;
        SuppEntity.LastName = txtLastName.Text;
        SuppEntity.DOB = txtdob.Text;
        SuppEntity.Gender = ddlgender.SelectedValue;
        SuppEntity.Contact = txtMainPhone.Text;
        SuppEntity.Mobile = txt_mobilenumber.Text;
        SuppEntity.CITY = txt_city.Text;
        SuppEntity.Province = txt_province.Text;
        SuppEntity.Email = txtMainEmail.Text;

        result = SuppCtrl.Sp_UpdateSupport(SuppEntity);
        DataTable dtChkMail = Comm.GetAllDataFromTable("SupportAdmin", "Email", "SupportAdminId='" + int.Parse(Session["EmployeeId"].ToString()) + "'", "");
        if (hfdemail.Value.Trim() != dtChkMail.Rows[0]["Email"].ToString().Trim())
        {
            Comm.ExecuteQuery("update UserAcc set Email='" + txtMainEmail.Text + "' where EmployeeId=" + int.Parse(Session["EmployeeId"].ToString()) + "and UserType='SupportAdmin'");           
        }

        if (result > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Profile Updated Successfully', 'success');", true);
        }
    }

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    protected void txtMainEmail_TextChanged(object sender, EventArgs e)
    {
        if (txtMainEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtMainEmail.Text) == true)
            {
                divValid.Visible = false;
                bool ceck = false;
                ceck = Comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
                if (ceck == false)
                {
                    divconfirm.Visible = true;
                }
                if (txtMainEmail.Text.Trim() == "")
                {
                    divconfirm.Visible = false;
                }
            }
            else
            {
                divValid.Visible = true;
            }
        }
        else
        {
            divconfirm.Visible = false;
        }
    }

    protected void txtConLoginEmail_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
        }
        else
        {
            div2stmsg.Visible = false;
            divtick.Visible = true;
        }
    }

    protected void LinkButton_Back_To_Home_Page_Click(object sender, EventArgs e)
    {
        if (Session["email"].ToString() == "support@anyminute.net")
            Response.Redirect("SuperAdmin_Approve_Subscriber.aspx"); //SuperAdmin_Approve_New_Account.aspx changed by kumar
        else
            Response.Redirect("SuperAdmin.aspx");              
    }
}