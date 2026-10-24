using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class BusinessOwnerProfile : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddEmployeeEntityClass AddempEntity = new AddEmployeeEntityClass();
    EmpAddressEntity EmpAddress = new EmpAddressEntity();
    MyProfileController MyProfileCtrl = new MyProfileController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            if (Request.QueryString["ProfileEmployeeId"] != null)
            {
                string aa = Request.QueryString["ProfileEmployeeId"].ToString();
                GetProfileDetails(int.Parse(aa));
            }
        }
    }
    public void GetProfileDetails(int ProfileEmployeeId)
    {
        DataTable dt = new DataTable();
        dt = comm.GetAllDataFromTable("AddEmployee ae join EmpAddress ea on ea.EmpAddressId=ae.ADDRESSID", "*", "ae.EmployeeID=" + ProfileEmployeeId + "", "");
        txtLegalName.SelectedValue = dt.Rows[0]["Title"].ToString();
        txtFirstName.Text = dt.Rows[0]["FirstName"].ToString();

        txtMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dt.Rows[0]["LastName"].ToString();
        if (dt.Rows[0]["DateOfBirth"].ToString() != "")
        {
            string dbo = dt.Rows[0]["DateOfBirth"].ToString();
            if (string.IsNullOrEmpty(dbo) || dbo.Length % 4 != 0
       || dbo.Contains(" ") || dbo.Contains("\t") || dbo.Contains("\r") || dbo.Contains("\n"))
                {
                if (dbo != "")
                {
                    txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                string ddbo = comm.Decrypt(dt.Rows[0]["DateOfBirth"].ToString());
                if (ddbo != "")
                {
                    txtdob.Text = DateTime.Parse(ddbo).ToString("yyyy-MM-dd");
                }
            }
            

        }
        ddlgender.SelectedValue = dt.Rows[0]["Gender"].ToString();
        txtMainPhone.Text = dt.Rows[0]["MainPhone"].ToString();
        txt_mobilenumber.Text = dt.Rows[0]["Mobile"].ToString();
        txt_city.Text = dt.Rows[0]["City1"].ToString();
        txt_province.Text = dt.Rows[0]["Province1"].ToString();
        txtMainEmail.Text = dt.Rows[0]["MainEmail"].ToString();

        txtConLoginEmail.Text = dt.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dt.Rows[0]["CCEmail"].ToString();

    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        AddempEntity.EmployeeID = int.Parse(Request.QueryString["ProfileEmployeeId"].ToString());
        AddempEntity.FirstName = txtFirstName.Text;
        AddempEntity.MiddleName = txtMiddleName.Text;
        AddempEntity.Title = txtLegalName.SelectedValue;
        AddempEntity.LastName = txtLastName.Text;

        AddempEntity.DateOfBirth = comm.encrypt(txtdob.Text);

        AddempEntity.Gender = ddlgender.SelectedValue;

        EmpAddress.MainPhone = txtMainPhone.Text;
        EmpAddress.Mobile = txt_mobilenumber.Text;
        EmpAddress.City = txt_city.Text;
        EmpAddress.Province = txt_province.Text;
        EmpAddress.MainEmail = txtMainEmail.Text;

        //EmpAddress.MainEmail = txtConLoginEmail.Text ;
        EmpAddress.CCEmail = txtCCEmail.Text;
        int result = 0;
        result = MyProfileCtrl.UpdateBusinessOwnerProfile(AddempEntity, EmpAddress);
        if (result > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Profile Updated Successfully', 'success');", true);
        }

    }
}