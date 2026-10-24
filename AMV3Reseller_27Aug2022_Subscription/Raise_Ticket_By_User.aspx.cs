using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Raise_Ticket_By_User : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddTicketEntityClass AddTicketEntity = new AddTicketEntityClass();
    AddTicketControllerClass AddTicket = new AddTicketControllerClass();
    EmailSettingController emailctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                A1.HRef = "Accountant.aspx";
                A3.HRef = "Accountant.aspx";
            }
            else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
            {
                A1.HRef = "SuperAdmin_Approve_Subscriber.aspx";
                A3.HRef = "SuperAdmin_Approve_Subscriber.aspx";
            }
            else
            {
                A1.HRef = "Home_Page.aspx";
                A3.HRef = "Home_Page.aspx";
            }

            if (Session["UserType"].ToString() == "Admin")
            {
                DataTable dt1 = comm.GetAllDataFromTable("userACC UA JOIN MotherOrg Mo on ua.Email=mo.Email", "*", "mo.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and ua.UserType='ADMIN' AND UA.UserId=" + Session["UserId"].ToString() + "", "");
                if (dt1.Rows.Count > 0)
                {
                    string MotherOrgName_Seperate = dt1.Rows[0]["MotherOrgName"].ToString();
                    string[] MotherOrgName_Seperate_Label = MotherOrgName_Seperate.Split('-');
                    txt_fullname.Text = MotherOrgName_Seperate_Label[0].ToString();
                    txt_fullname.ReadOnly = true;
                }
            }
            if (Session["UserType"].ToString() != "Admin")
            {
                txt_fullname.Text = Session["NameOfLoggedInUser"].ToString();
                txt_fullname.ReadOnly = true;
            }
                
            txt_Email.Text = Session["email"].ToString();
            txt_Email.ReadOnly = true;
        }
    }

    DataTable GenNoticeID()
    {
        return comm.GetAllDataFromTable("Ticket", "isnull(max(TicketId),0) + 1 as tid", "", "");
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (txt_Phone.Text.Replace("-", string.Empty).Trim().Length < 10)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Phone Number Properly', 'warning');", true);
            return;
        }

        string photofilename = string.Empty;
        if (FileUpload.HasFile)
        {
            DataTable tkid = GenNoticeID();
            int TktIdId = Convert.ToInt32(tkid.Rows[0]["tid"].ToString());
            string fileName = Path.GetFileName(FileUpload.PostedFile.FileName);
            FileUpload.PostedFile.SaveAs(Server.MapPath("~/images/" + TktIdId) + fileName);
            photofilename = TktIdId + fileName;
        }
        if (!FileUpload.HasFile)
        {
            photofilename = "No_attachment.png";
        }

        DataSet dsticketno = new DataSet();
        int TicketId1;
        DataTable TicketNo = new DataTable();
        if (btn_save.Text == "Save")
        {
            AddTicketEntity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString());
            AddTicketEntity.EmpId = int.Parse(Session["EmployeeId"].ToString());
            AddTicketEntity.Name = txt_fullname.Text;
            AddTicketEntity.EmailId = txt_Email.Text;
            AddTicketEntity.PhoneNO = txt_Phone.Text;
            AddTicketEntity.ProblemType = ddl_problemtype.SelectedValue;
            AddTicketEntity.Module = ddl_module.SelectedValue;
            AddTicketEntity.Description = txt_Describe.Text;
            AddTicketEntity.FileUploadName = photofilename;

            TicketId1 = AddTicket.InsertTicket(AddTicketEntity);
            comm.ExecuteQuery(" Insert Into Ticket_Comments (TicketId,EmpId,Name,EmailId,PhoneNO,FileUploadName,CommentsBy,Comments,CommentsDate) " +
                " Values (" + TicketId1 + "," + AddTicketEntity.EmpId + ",'" + AddTicketEntity.Name + "','" + AddTicketEntity.EmailId + "','" + AddTicketEntity.PhoneNO + "'," +
                " '" + AddTicketEntity.FileUploadName + "','" + AddTicketEntity.Name + "','" + AddTicketEntity.Description + "','" + DateTime.Now.ToString() + "')");
            ClearTicketDetails();

            bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
            if (IsSend)
            {
                emailctrl.SendTicketMail(AddTicketEntity, Session["email"].ToString(), TicketId1.ToString());
                emailctrl.SendTicketDetailsToUser(AddTicketEntity, Session["email"].ToString(), TicketId1.ToString());
            }

            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Ticket Details Added Successfully, Thanks for your Valuable Feedback', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Thank you for creating the Ticket, One of our staff member will look into this issue and will get back to you as soon as possible.', 'success');", true);
        }
    }

    protected void btn_Reset_Click(object sender, EventArgs e)
    {
        ClearTicketDetails();
    }

    public void ClearTicketDetails()
    {
        //txt_fullname.Text = "";
        //txt_Email.Text = "";
        txt_Phone.Text = "";
        ddl_problemtype.SelectedValue = "";
        //ddl_module.SelectedValue = "";
        txt_Describe.Text = "";
    }
}