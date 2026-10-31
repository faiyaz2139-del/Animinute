using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;

public partial class MotherOrg : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    EmailSettingController Esctrl = new EmailSettingController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlStaus.SelectedValue = "1";
            GetMotherOrgList();
            if(Request.QueryString["MotherOrgId"]!=null)
            {
                GetDetailsForEdit(int.Parse(Request.QueryString["MotherOrgId"].ToString()));
            }
        }
    }

    public void GetMotherOrgList()
    {
        comm.FillGridView(GVMother, "MotherOrg m join UserAcc ua on ua.MotherOrgid=m.MotherOrgid and UserType='Admin'", "m.MotherOrgID,m.MotherOrgName,m.EnrollDate,m.Status", "ua.Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password", "", "MotherOrgID");   
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        int MotherOrgId = 0;

        if (txtEmail.Text.Trim() == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Email!', 'error');", true);
            return;
        }
        if (txtEnroll.Text == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Enroll Date!', 'error');", true);
            return;
        }
        if(txtEmail.Text.Trim()!=txtComfirm.Text.Trim())
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Confirm Mail Not Match!', 'error');", true);
            return;
        }
        if (txtComfirm.Text.Trim()==string .Empty)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Confirm Mail !', 'error');", true);
            return;
        }
        string[] y = txtEnroll.Text.Split('-');
        if (y[0].Length > 4)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Vaild Enroll Date!', 'error');", true);
            return;
        }
        if(txtEmail.Text.Trim()!= hfdEmail.Value)
        {
            DataTable dtCheck;
            dtCheck = comm.GetAllDataFromTable("UserAcc", "*", "Email='" + txtEmail.Text.Trim() + "'", "");
            if (dtCheck.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Email " + txtEmail.Text + " Already exist ', 'error');", true);
                return;
            }
        }        

        if (btnsave.Text == "Save")
        {
            MotherEc.MotherOrgName = txtOrgName.Text;
            MotherEc.StreetNumber = txtStreetNo.Text;
            MotherEc.StreetName = txtStreetName.Text;
            MotherEc.Apartment = txtApartment.Text;
            MotherEc.City = txtCity.Text;
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
            if(CheckBox1.Checked == true)
            {
                MotherEc.Checked = "Y";
            }
            if (CheckBox1.Checked == false)
            {
                MotherEc.Checked = "N";
            }
            MotherOrgId =MotherC.InsertMotherOrg(MotherEc);
            SendEmail(txtEmail.Text.Trim(), MotherOrgId.ToString());
            clear();
            GetMotherOrgList();
            Response.Redirect("SuperAdmin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        else
        {
            MotherOrgId= int.Parse(Request.QueryString["MotherOrgId"].ToString());
            MotherEc.MotherOrgID = int.Parse(Request.QueryString["MotherOrgId"].ToString());
            MotherEc.MotherOrgName = txtOrgName.Text;
            MotherEc.StreetNumber = txtStreetNo.Text;
            MotherEc.StreetName = txtStreetName.Text;
            MotherEc.Apartment = txtApartment.Text;
            MotherEc.City = txtCity.Text;
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
            if (CheckBox1.Checked == true)
            {
                MotherEc.Checked = "Y";
            }
            if (CheckBox1.Checked == false)
            {
                MotherEc.Checked = "N";
            }
            MotherC.UpdateMotherOrg(MotherEc);
            // v39 (Change Login Email DOC s6): the firm's contact email is business information and no longer overwrites the
            // firm admin's global login email (UserAcc.Email). The login owner changes it in My Profile > Login & Security.
            
            clear();
            GetMotherOrgList();
            Response.Redirect("SuperAdmin.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }       
    }

    public void clear()
    {
        txtOrgName.Text = string.Empty;
        txtStreetNo.Text = string.Empty;
        txtStreetName.Text = string.Empty;
        txtApartment.Text = string.Empty;
        txtCity.Text = string.Empty;
        txtProvince.Text = string.Empty;
        txtPostalCode.Text = string.Empty;
        txtEnroll.Text = string.Empty;
        ddlStaus.SelectedValue = "";
        txtEmail.Text = string.Empty;
    }

    public void SendEmail(string email,string Motherid)
    {
        string Host = ConfigurationManager.AppSettings["ResetUrl"];
        string url = Host + Motherid.ToString()+"&LoginType=Acc";
        DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
        DataTable dtemp = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Motherid + "", "");
        DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["Email"].ToString() + "' ", "");
        string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["MotherOrgName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["Email"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
        Esctrl.SendHtmlFormattedEmail(txtEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
    }

    public void GetDetailsForEdit(int MotherId)
    {
        DataTable dt = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + MotherId + "", "");
        DataTable dt1 = comm.GetAllDataFromTable("DefaultMotherOrg", "*", "MotherOrgID=" + MotherId + "", "");
        txtOrgName.Text = dt.Rows[0]["MotherOrgName"].ToString();
        txtStreetNo.Text = dt.Rows[0]["StreetNumber"].ToString();
        txtStreetName.Text = dt.Rows[0]["StreetName"].ToString();
        txtApartment.Text = dt.Rows[0]["Apartment"].ToString();
        txtCity.Text = dt.Rows[0]["City"].ToString();
        txtProvince.Text = dt.Rows[0]["Province"].ToString();
        txtPostalCode.Text = dt.Rows[0]["PostalCode"].ToString();
        txtEnroll.Text = DateTime.Parse(dt.Rows[0]["EnrollDate"].ToString()).ToString("yyyy-MM-dd");
        txtComfirm.Text= dt.Rows[0]["Email"].ToString();
        if (bool.Parse(dt.Rows[0]["Status"].ToString()) == true)
        {
            ddlStaus.SelectedValue = "1";
        }
        else
        {
            ddlStaus.SelectedValue = "0";
        }

        txtEmail.Text = dt.Rows[0]["Email"].ToString();
        hfdEmail.Value = dt.Rows[0]["Email"].ToString();
        txtContactName.Text= dt.Rows[0]["ContactName"].ToString();
        txtContactNumber.Text= dt.Rows[0]["ContactNumber"].ToString();
        btnsave.Text = "Update";
        if (dt1.Rows.Count > 0)
        {
            CheckBox1.Checked = true;
        }
        else
        {
            CheckBox1.Checked = false;
        }
    }

    protected void GVMother_PreRender(object sender, EventArgs e)
    {
        if (GVMother.Rows.Count > 0)
        {
            GVMother.UseAccessibleHeader = true;
            GVMother.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnedit_Click(object sender, EventArgs e)
    {
        DataTable dt = new DataTable();
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdid = (HiddenField)GVMother.Rows[gvr.RowIndex].FindControl("hfdid");
        hfdEditId.Value = hfdid.Value;
        dt = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + hfdid.Value + "", "");
        txtOrgName.Text = dt.Rows[0]["MotherOrgName"].ToString();
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
        btnsave.Text = "Update";
    }

    protected void txtComfirm_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtEmail.Text.Trim() != txtComfirm.Text.Trim())
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
    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }
    protected void txtEmail_TextChanged(object sender, EventArgs e)
    {
        if (txtEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtEmail.Text) == true)
            {
                divValid.Visible = false;
                bool ceck = false;
                ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtEmail.Text + "'");
                if (ceck == false)
                {
                    divconfirm.Visible = true;
                }
                if (txtEmail.Text.Trim() == "")
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
}