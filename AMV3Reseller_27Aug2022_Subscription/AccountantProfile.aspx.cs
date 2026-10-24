using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class AccountantProfile : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    int employeeid = 0;
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            txtEmail.Enabled = false;
            GetMotherOrgList();
        }
    }
    public void GetMotherOrgList()
    {
        DataTable dt = new DataTable();
        dt= comm.GetAllDataFromTable("MotherOrg m join UserAcc ua on ua.MotherOrgid=m.MotherOrgid and UserType='Admin'", "*", "ua.Email='"+ Session["email"].ToString() + "'", "ua.MotherOrgid");
        if (dt.Rows.Count > 0)
        {
            hfdEditId.Value = dt.Rows[0]["MotherOrgID"].ToString();
            //txtOrgName.Text = dt.Rows[0]["MotherOrgName"].ToString();
            txtOrgName.Text = dt.Rows[0]["ContactName"].ToString();
            txtStreetNo.Text = dt.Rows[0]["StreetNumber"].ToString();
            txtStreetName.Text = dt.Rows[0]["StreetName"].ToString();
            txtApartment.Text = dt.Rows[0]["Apartment"].ToString();
            txtCity.Text = dt.Rows[0]["City"].ToString();
            txtProvince.Text = dt.Rows[0]["Province"].ToString();
            txtPostalCode.Text = dt.Rows[0]["PostalCode"].ToString();
            txtEnroll.Text = DateTime.Parse(dt.Rows[0]["EnrollDate"].ToString()).ToString("yyyy-MM-dd");

            if (bool.Parse(dt.Rows[0]["Status"].ToString()) == true)
            {
                ddlStaus.SelectedValue = "1";
            }
            else
            {
                ddlStaus.SelectedValue = "0";
            }

            txtEmail.Text = dt.Rows[0]["Email"].ToString();
            hdnemailid.Value = dt.Rows[0]["Email"].ToString();
            txtContactNumber.Text = dt.Rows[0]["ContactNumber"].ToString();
            txtContactName.Text = dt.Rows[0]["ContactName"].ToString();
            employeeid = Convert.ToInt32(dt.Rows[0]["EmployeeID"].ToString());
        }
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        if (txtEmail.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Email!', 'error');", true);
            return;
        }
        if (txtEnroll.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Enroll Date!', 'error');", true);
            return;
        }
        MotherEc.MotherOrgID = int.Parse(hfdEditId.Value);
        MotherEc.MotherOrgName = txtOrgName.Text;
        MotherEc.StreetNumber = txtStreetNo.Text;
        MotherEc.StreetName = txtStreetName.Text;
        MotherEc.Apartment = txtApartment.Text;
        MotherEc.City = txtCity.Text;
        MotherEc.Checked = "N";
        MotherEc.Province = txtProvince.Text;
        MotherEc.PostalCode = txtPostalCode.Text;
        MotherEc.EnrollDate = DateTime.Parse(txtEnroll.Text);
        if (ddlStaus.SelectedValue == "1")
        {
            MotherEc.Status = true;
        }
        else
        {
            MotherEc.Status = false;
        }

        MotherEc.Email = txtEmail.Text;
        MotherEc.ContactName = txtContactName.Text;
        MotherEc.ContactNumber = txtContactNumber.Text;
        MotherEc.OldEmail = hdnemailid.Value;
        MotherEc.EmployeeID = employeeid.ToString();
        if(txtEmail.Text.Trim() == hdnemailid.Value.Trim())
        {
            MotherC.UpdateMotherOrg(MotherEc);
        }
       else
        {
            DataTable dtCheck;
            dtCheck = comm.GetAllDataFromTable("UserAcc", "*", "Email='" + txtEmail.Text.Trim() + "'", "");
            if (dtCheck.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Email " + txtEmail.Text + " Already exist ', 'error');", true);
                return;
            }
            bool Chk2 = comm.IfExistsInTable("MotherOrg", "*",  "Email= '" + txtEmail.Text + "'");
            if (Chk2 == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' Accountant Email Id already registered', 'warning');", true);
                return;
            }
            MotherC.UpdateMotherOrgWithNewEmail(MotherEc);
        }
        GetMotherOrgList();      
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Profile Updated Successfully', 'success');", true);
    }
}