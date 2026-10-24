using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using log4net;
using System.Text.RegularExpressions;

public partial class AddSuppAdmin : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    SupportAdminCtrl SuppCtrl = new SupportAdminCtrl();
    SupportAdminEntity SuppEntity = new SupportAdminEntity();
    UserAccEntity UaEntity = new UserAccEntity();
    EmailSettingController Esctrl = new EmailSettingController();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Request.QueryString["SupportAdminId"] != null)
            {
                GetRecordForEdit(int.Parse(Request.QueryString["SupportAdminId"].ToString()));
            }
        }
    }
    public void GetRecordForEdit(int SupportAdminId)
    {
        DataTable dt = new DataTable();
        dt = Comm.GetAllDataFromTable("SupportAdmin", "*", "SupportAdminId=" + SupportAdminId + "", "");
        ddlLegalName.SelectedValue = dt.Rows[0]["Title"].ToString();
        txtFirstName.Text = dt.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dt.Rows[0]["LastName"].ToString();
        txtdob.Text = DateTime.Parse(dt.Rows[0]["DOB"].ToString()).ToString("yyyy-MM-dd");
        ddlgender.SelectedValue = dt.Rows[0]["Gender"].ToString();
        txtMainPhone.Text = dt.Rows[0]["Contact"].ToString();
        txt_mobilenumber.Text = dt.Rows[0]["Mobile"].ToString();
        txt_city.Text = dt.Rows[0]["CITY"].ToString();
        txt_province.Text = dt.Rows[0]["Province"].ToString();
        txtMainEmail.Text = dt.Rows[0]["Email"].ToString();
        //txtMainEmail.ReadOnly = true;
        hfdemail.Value = dt.Rows[0]["Email"].ToString();
    }
    protected void btnsave_Click(object sender, EventArgs e)
    {
        if(txtMainEmail.Text!=hfdemail.Value)
        {
            if (txtConLoginEmail.Text.Trim() == string.Empty)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('error!', 'Enter Confirm Mail !', 'error');", true);
                return;
            }
        }        

        try
        {
            if (Request.QueryString["SupportAdminId"] == null)
            {

                bool Check = Comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
                if (Check == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email " + txtMainEmail.Text + " Already exist ', 'warning');", true);
                    return;
                }
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
                SuppEntity.Status = "True";

                int empid = SuppCtrl.Sp_InsertSupport(SuppEntity);

                DataTable dtUserType = Comm.GetAllDataFromTable("AdminUserType", "*", "", "AdminUserId");
                UaEntity.UserType = dtUserType.Rows[0]["AdminUserType"].ToString();
                UaEntity.UserTypeid = int.Parse(dtUserType.Rows[0]["AdminUserId"].ToString());
                UaEntity.Email = txtMainEmail.Text;
                UaEntity.EmployeeId = empid;
                int result = SuppCtrl.Sp_SP_InsertUserAccSupportAdmin(UaEntity);
                SendEmail(txtMainEmail.Text, empid.ToString());
            }
            else
            {                
                SuppEntity.SupportAdminId = int.Parse(Request.QueryString["SupportAdminId"].ToString());
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

                int result = SuppCtrl.Sp_UpdateSupport(SuppEntity);

                DataTable dtChkMail = Comm.GetAllDataFromTable("SupportAdmin", "Email", "SupportAdminId='" + Request.QueryString["SupportAdminId"].ToString() + "'", "");
                if (hfdemail.Value.Trim() != dtChkMail.Rows[0]["Email"].ToString().Trim())
                {
                    Comm.ExecuteQuery("update UserAcc set Email='" + txtMainEmail.Text + "' where EmployeeId=" + Request.QueryString["SupportAdminId"].ToString() + "and UserType='SupportAdmin'");
                    SendEmail(txtMainEmail.Text, Request.QueryString["SupportAdminId"].ToString());
                }
                if (result > 0)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Updated', 'success');", true);
                }
            }
            Response.Redirect("SupportAdminList.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
        catch (Exception ex)
        {
            loginfo = "btnsave_Click " + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
    }

    public void SendEmail(string Email, string Empid)
    {
        try
        {
            loginfo = "SendEmail(Select " + Email + ", " + Empid + ") Start";
            Logger.Info(loginfo);
            string Host = ConfigurationManager.AppSettings["ResetUrl"];
            string url = Host + Empid.ToString() + "&LoginType=Support"; ;
            DataTable dtEmailTemp = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
            DataTable dtemp = Comm.GetAllDataFromTable("SupportAdmin", "*", "SupportAdminId=" + Empid + "", "");
            DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["Email"].ToString() + "' ", "");
            string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["Email"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
            Esctrl.SendHtmlFormattedEmail(txtMainEmail.Text, dtEmailTemp.Rows[0]["Subject"].ToString(), body);

            loginfo = "SendEmail(Select " + Email + ", " + Empid + ") End";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "SendEmail(Select " + Email + ", " + Empid + ") " + ex.Message;
            Logger.Error(loginfo);
            throw ex;

        }
    }
    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }
    protected void txtMainEmail_TextChanged(object sender, EventArgs e)
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
}