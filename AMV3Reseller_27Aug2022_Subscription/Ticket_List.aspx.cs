using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class Ticket_List : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddTicketControllerClass AddTicket = new AddTicketControllerClass();
    AddTicketEntityClass AddTicketEntity = new AddTicketEntityClass();
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

            if (Session["UserType"].ToString() == "SuperAdmin" || Session["UserType"].ToString()== "SupportAdmin")
            {
                //mother.Attributes.Add("Style", "display:block;"); //commented because we will show all the list to support admin
                mother.Attributes.Add("Style", "display:none;");
                comm.FillDropDownList(ddlmother, "MotherOrg", "MotherOrgID", "MotherOrgName", "", "");
            }
            else
            {
                mother.Attributes.Add("Style", "display:none;");
                comm.FillDropDownList(ddlmother, "MotherOrg", "MotherOrgID", "MotherOrgName", "", "");
                ddlmother.SelectedValue = Session["MotherOrgid"].ToString();
                ddlmother.Enabled = false;
            }
        }
    }

    protected void btnShow_Click(object sender, EventArgs e)
    {
        if (Session["UserType"].ToString() == "SuperAdmin")
        {
            if (ddlmother.SelectedItem.Text == "Please Select")
            {
                GVTicketList.DataSource = AddTicket.GetTimeSheetDetailsForAdmin_All(DateTime.Parse(txtfromDate.Text), DateTime.Parse(txttodate.Text)); //, int.Parse(Session["EmployeeId"].ToString()));
                GVTicketList.DataBind();
            }
            else
            {
                GVTicketList.DataSource = AddTicket.GetTimeSheetDetailsForAdmin(DateTime.Parse(txtfromDate.Text), DateTime.Parse(txttodate.Text), int.Parse(ddlmother.SelectedValue)); //, int.Parse(Session["EmployeeId"].ToString()));
                GVTicketList.DataBind();
            }
        }
        else if (Session["UserType"].ToString() == "SupportAdmin")
        {
            GVTicketList.DataSource = AddTicket.GetTimeSheetDetailsForAdmin_All(DateTime.Parse(txtfromDate.Text), DateTime.Parse(txttodate.Text)); //, int.Parse(Session["EmployeeId"].ToString()));
            GVTicketList.DataBind();
        }
        //Session["UserType"].ToString() == "Admin" added by Kumar after Gayathri's bug
        else if (Session["UserType"].ToString() == "Admin")
        {
            GVTicketList.DataSource = AddTicket.GetTimeSheetDetailsForAdmin(DateTime.Parse(txtfromDate.Text), DateTime.Parse(txttodate.Text), int.Parse(Session["MotherOrgid"].ToString()));
            GVTicketList.DataBind();
        }
        //Session["UserType"].ToString() == "BUSINESS OWNER" added by Kumar after Gayathri's bug
        else if (Session["UserType"].ToString() == "BUSINESS OWNER")
        {
            DataTable DataTable_GetTicketList = comm.ExecuteQueryDataTable("select [TicketId] ,[MotherOrgName] ,[MotherOrgId] ,[BusinessName] ,[BusinessId] " +
                ",[ManagerName] ,[ManagerID] ,[EmpName] ,[EmpId] ,[Name] ,[EmailId] ,[FileUploadName] ,[PhoneNO] ,[StatusType] ,[ProblemType] ,[Module] " +
                ",[FileUploadName] ,[Description] ,[TicketNo], [DeveloperComment] from Ticket " +
                "where [EmpId] in (select distinct EmployeeID from AssginBussiness where BusinessID in " +
                "(select BusinessID from AssginBussiness where UserTypeId = 1 and EmployeeID = '" + int.Parse(Session["EmployeeId"].ToString()) + "' )) " +
                //"(select BusinessID from AssginBussiness where EmployeeID = '" + int.Parse(Session["EmployeeId"].ToString()) + "' )) " +
                "and [TicketRaiseDate] between '" + DateTime.Parse(txtfromDate.Text) + "' and '" + DateTime.Parse(txttodate.Text) + "' order by TicketId desc ");

            GVTicketList.DataSource = DataTable_GetTicketList;
            GVTicketList.DataBind();
        }
        else
        {
            GVTicketList.DataSource = AddTicket.GetTimeSheetDetailsForEdit(DateTime.Parse(txtfromDate.Text), DateTime.Parse(txttodate.Text), int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeId"].ToString()));
            GVTicketList.DataBind();
        }
    }

    protected void GVTicketList_PreRender(object sender, EventArgs e)
    {
        if (GVTicketList.Rows.Count > 0)
        {
            GVTicketList.UseAccessibleHeader = true;
            GVTicketList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void lnkbtn_Resolve_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[gvr.RowIndex].FindControl("hfd_TicketId");
        comm.ExecuteQuery("Update Ticket set StatusType='1' where TicketId=" + hfdTicketid.Value + "");
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Resolved', 'success');", true);
        btnShow_Click(new object(), new EventArgs());
    }

    protected void lnkbtn_On_Going_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[gvr.RowIndex].FindControl("hfd_TicketId");
        comm.ExecuteQuery("Update Ticket set StatusType='2' where TicketId=" + hfdTicketid.Value + "");
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Ongoing', 'success');", true);
        btnShow_Click(new object(), new EventArgs());
    }

    protected void lnkbtn_Pending_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[gvr.RowIndex].FindControl("hfd_TicketId");
        comm.ExecuteQuery("Update Ticket set StatusType='3' where TicketId=" + hfdTicketid.Value + "");
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Pending', 'success');", true);
        btnShow_Click(new object(), new EventArgs());
    }

    protected void GVTicketList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Find the TextBox control.
            Label _lbl_StatusType = (e.Row.FindControl("lbl_StatusType") as Label);
            if (_lbl_StatusType.Text == "")
            {
                _lbl_StatusType.Text = "";
            }
            else if (_lbl_StatusType.Text == "1")
            {
                _lbl_StatusType.Text = "Resolved";
            }
            else if (_lbl_StatusType.Text == "2")
            {
                _lbl_StatusType.Text = "In Progress";
            }
            else if (_lbl_StatusType.Text == "3")
            {
                _lbl_StatusType.Text = "Pending"; //"Rejected";
            }

            //if (Session["UserType"].ToString() == "SuperAdmin")
            //{
            //    e.Row.Cells[8].Visible = true;
            //    e.Row.Cells[11].Visible = true;
            //}
            //else
            //{
            //    e.Row.Cells[8].Visible = false;
            //    e.Row.Cells[11].Visible = false;
            //}

            //if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
            //{
            //    this.GVTicketList.Columns[12].Visible = true;
            //    this.GVTicketList.Columns[13].Visible = false;
            //    this.GVTicketList.Columns[14].Visible = true;
            //}
        }

        //if (e.Row.RowType == DataControlRowType.Header)
        //{
        //    if (Session["UserType"].ToString() == "SuperAdmin")
        //    {
        //        e.Row.Cells[8].Visible = true;
        //        e.Row.Cells[11].Visible = true;
        //    }
        //    else
        //    {
        //        e.Row.Cells[8].Visible = false;
        //        e.Row.Cells[11].Visible = false;
        //    }
        //}
    }

    protected void GVTicketList_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName == "Resolved")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            
            HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[index].FindControl("hfd_TicketId");
            comm.ExecuteQuery("Update Ticket set StatusType='1' where TicketId=" + hfdTicketid.Value + "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Resolved', 'success');", true);
            btnShow_Click(new object(), new EventArgs());
        }
        else if(e.CommandName == "On_Going")
        {
            int index = Convert.ToInt32(e.CommandArgument); 
            HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[index].FindControl("hfd_TicketId");
            comm.ExecuteQuery("Update Ticket set StatusType='2' where TicketId=" + hfdTicketid.Value + "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Ongoing', 'success');", true);
            btnShow_Click(new object(), new EventArgs());
        }
        else if(e.CommandName == "Pending")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[index].FindControl("hfd_TicketId");
            comm.ExecuteQuery("Update Ticket set StatusType='3' where TicketId=" + hfdTicketid.Value + "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Pending', 'success');", true);
            btnShow_Click(new object(), new EventArgs());
        }

        if (e.CommandName == "AddComment")
        {
            Label_Warning_Message.Visible = false;

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Label lbl_TicketNo = (Label)GVTicketList.Rows[index].FindControl("lbl_TicketNo");
            HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[index].FindControl("hfd_TicketId");
            lblticketid.Text = "Ticket No - " + lbl_TicketNo.Text;
            hfdticketautoid.Value = hfdTicketid.Value; //hfdTicketid.Value is sent in the email first time when ticket is created.

            Label lbl_Name = (Label)GVTicketList.Rows[index].FindControl("lbl_Name");
            Label Lbl_EmailId = (Label)GVTicketList.Rows[index].FindControl("Lbl_EmailId");
            Label lbl_PhoneNo = (Label)GVTicketList.Rows[index].FindControl("lbl_PhoneNo");
            HiddenField_Name.Value = lbl_Name.Text;
            HiddenField_Email.Value = Lbl_EmailId.Text;
            HiddenField_Phone.Value = lbl_PhoneNo.Text;
            var Ticket_EmpId = comm.GetAllDataFromTable("Ticket with (Nolock)", "EmpId,FileUploadName", "TicketId='" + hfdTicketid.Value + "' ", "");
            HiddenField_EmpId.Value = Ticket_EmpId.Rows[0]["EmpId"].ToString();
            HiddenField_FileUploadName.Value = Ticket_EmpId.Rows[0]["FileUploadName"].ToString();

            //var Dt_Check_Comment = comm.GetAllDataFromTable("Ticket with (Nolock)", "DeveloperComment", "TicketId='" + hfdTicketid.Value + "' ", "");
            //txtAddComment.Text = Dt_Check_Comment.Rows[0]["DeveloperComment"].ToString();
            modaladdcomment.Show();
        }

        if (e.CommandName == "ViewComment")
        {
            Label_Warning_Message.Visible = false;
            GridView_Comments_List.DataSource = null;
            GridView_Comments_List.DataBind();

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Label lbl_TicketNo = (Label)GVTicketList.Rows[index].FindControl("lbl_TicketNo");
            HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[index].FindControl("hfd_TicketId");
            lblticketid2.Text = "Ticket No - " + lbl_TicketNo.Text;
            hfdticketautoid2.Value = hfdTicketid.Value; //hfdTicketid.Value is sent in the email first time when ticket is created.

            var Dt_Comments_List = comm.GetAllDataFromTable("Ticket_Comments with (Nolock)", "*, CAST(CommentsDate AS DATE)CommentsDateOnly", "TicketId='" + hfdticketautoid2.Value + "' ", "CommentsDate desc");
            GridView_Comments_List.DataSource = Dt_Comments_List;
            GridView_Comments_List.DataBind();
            modalviewcomment.Show();
        }
    }

    protected void btnAddComment_Click(object sender, EventArgs e)
    {
        if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
        {
            if (txtAddComment.Text == "")
            {
                Label_Warning_Message.Visible = true;
                modaladdcomment.Show();
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please add comment', 'warning');", true);
                return;
            }

            AddTicketEntity.Name = HiddenField_Name.Value;
            AddTicketEntity.EmailId = HiddenField_Email.Value;
            AddTicketEntity.PhoneNO = HiddenField_Phone.Value;
            AddTicketEntity.DeveloperComment = txtAddComment.Text;
            AddTicketEntity.EmpId = int.Parse(HiddenField_EmpId.Value);

            comm.ExecuteQuery("Update Ticket set DeveloperComment='" + txtAddComment.Text + "' where TicketId=" + hfdticketautoid.Value + "");
            comm.ExecuteQuery("Insert Into Ticket_Comments (TicketId,EmpId,Name,EmailId,PhoneNO,FileUploadName,CommentsBy,Comments,CommentsDate) " +
                    " Values (" + hfdticketautoid.Value + "," + AddTicketEntity.EmpId + ",'" + AddTicketEntity.Name + "','" + AddTicketEntity.EmailId + "','" + AddTicketEntity.PhoneNO + "'," +
                    " '" + AddTicketEntity.FileUploadName + "','Support Team','" + AddTicketEntity.DeveloperComment + "','" + DateTime.Now.ToString() + "')");
            
            bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
            if (IsSend)
            {
                emailctrl.SendTicketDetailsToUserbySupportAdmin(AddTicketEntity, hfdticketautoid.Value);
            }

            txtAddComment.Text = "";
            modaladdcomment.Hide();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Comments added successfully', 'success');", true);
        }
        else
        {
            if (txtAddComment.Text == "")
            {
                Label_Warning_Message.Visible = true;
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Please add comment', 'warning');", true);
                modaladdcomment.Show();
                return;
            }

            var Ticket_Details = comm.GetAllDataFromTable("Ticket with (Nolock)", "*", "TicketId='" + hfdticketautoid.Value + "' ", "");
            comm.ExecuteQuery("Insert Into Ticket_Comments (TicketId,EmpId,Name,EmailId,PhoneNO,FileUploadName,CommentsBy,Comments,CommentsDate) " +
                " Values (" + hfdticketautoid.Value + "," + Ticket_Details.Rows[0]["EmpId"].ToString() + ",'" + Ticket_Details.Rows[0]["Name"].ToString() + "','" + Ticket_Details.Rows[0]["EmailId"].ToString() + "'," +
                " '" + Ticket_Details.Rows[0]["PhoneNO"].ToString() + "','" + Ticket_Details.Rows[0]["FileUploadName"].ToString() + "','" + Ticket_Details.Rows[0]["Name"].ToString() + "'," +
                " '" + txtAddComment.Text + "','" + DateTime.Now.ToString() + "')");
            comm.ExecuteQuery("Update Ticket set DeveloperComment='" + txtAddComment.Text + "' where TicketId=" + hfdticketautoid.Value + "");

            AddTicketEntity.Name = Ticket_Details.Rows[0]["Name"].ToString();
            AddTicketEntity.EmailId = Ticket_Details.Rows[0]["EmailId"].ToString();
            AddTicketEntity.PhoneNO = Ticket_Details.Rows[0]["PhoneNO"].ToString();
            AddTicketEntity.DeveloperComment = txtAddComment.Text;
            AddTicketEntity.EmpId = int.Parse(Ticket_Details.Rows[0]["EmpId"].ToString());

            bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
            if (IsSend)
            {
                emailctrl.SendTicketDetailsCommentsFromUserToSupportAdmin(AddTicketEntity, hfdticketautoid.Value);
            }

            txtAddComment.Text = "";
            modaladdcomment.Hide();
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Comments added successfully', 'success');", true);
        }
    }

    protected void DropDownList_Change_Status_SelectedIndexChanged(object sender, EventArgs e)
    {
        DropDownList DropDownList_Change_Status = (DropDownList)sender;
        GridViewRow gvr = (GridViewRow)DropDownList_Change_Status.NamingContainer;
        HiddenField hfdTicketid = (HiddenField)GVTicketList.Rows[gvr.RowIndex].FindControl("hfd_TicketId");
        if (DropDownList_Change_Status.SelectedIndex == 1)
        {
            int a = comm.ExecuteQuery("Update Ticket set StatusType='1' where TicketId=" + hfdTicketid.Value + "");
            if (a > 0)
            {
                btnShow_Click(sender, e);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Resolved', 'success');", true);
            }
        }
        if (DropDownList_Change_Status.SelectedIndex == 2)
        {
            int a = comm.ExecuteQuery("Update Ticket set StatusType='2' where TicketId=" + hfdTicketid.Value + "");
            if (a > 0)
            {
                btnShow_Click(sender, e);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to In Progress', 'success');", true);
            }
        }
        if (DropDownList_Change_Status.SelectedIndex == 3)
        {
            int a = comm.ExecuteQuery("Update Ticket set StatusType='3' where TicketId=" + hfdTicketid.Value + "");
            if (a > 0)
            {
                btnShow_Click(sender, e);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Ticket Status has been Changed to Pending', 'success');", true);
            }
        }
    }

    protected void GridView_Comments_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_Comments_List.Rows.Count > 0)
        {
            GridView_Comments_List.UseAccessibleHeader = true;
            GridView_Comments_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_Comments_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lbl_Comments_DateOnly = (e.Row.FindControl("lbl_Comments_DateOnly") as Label);
            string input = lbl_Comments_DateOnly.Text;
            DateTime dt;
            DateTime.TryParse(input, out dt);
            lbl_Comments_DateOnly.Text = dt.ToString("yyyy-MM-dd");
        }
    }
}