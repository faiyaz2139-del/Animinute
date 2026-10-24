using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Change_Password : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    ChangePasswordControllerClass cpctrl = new ChangePasswordControllerClass();
    ChangePasswordClass cpCls = new ChangePasswordClass();
    string role_check_for_change_pssword_role_message_throw = "0";

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
                A2.HRef = "Accountant.aspx";
            }               
            else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
            {
                A1.HRef = "SuperAdmin_Approve_Subscriber.aspx";
                A2.HRef = "SuperAdmin_Approve_Subscriber.aspx";
            }  
            else
            {
                A1.HRef = "Home_Page.aspx";
                A2.HRef = "Home_Page.aspx";
            }

            if (Request.QueryString["SubscriberIDEdit"] != null || Request.QueryString["ResellerIDEdit"] != null)
            {
                TxtCrntPswrd.Text = "Not Required to Enter";
                //TxtCrntPswrd.ReadOnly = true;
                TxtCrntPswrd.Enabled = false;
            }
        }
    }

    protected void BtnUpdtPswrd_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["SubscriberIDEdit"] != null)
        {
            int len = TxtNewPswrd.Text.Trim().Length;
            if (len < 8)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Password Must Contain atleast 8 characters', 'error');", true);
                return;
            }

            if (TxtNewPswrd.Text != TxtCnfrmPswrd.Text)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm password does not match', 'warning');", true);
                return;
            }

            var Get_UserID = Comm.GetAllDataFromTable("UserAcc with (Nolock)", "UserId", "EmployeeID='" + Request.QueryString["SubscriberIDEdit"].ToString() + "' ", "");
            cpCls.UserId = int.Parse(Get_UserID.Rows[0]["UserId"].ToString());
            cpCls.Password = TxtNewPswrd.Text;
            cpctrl.UpdateChangePassword(cpCls);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Password has been updated successfully', 'success');", true);
            //if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
            //{
            //    Response.Redirect("SuperAdmin_Approve_Subscriber.aspx");
            //}
            return;
        }

        if (Request.QueryString["ResellerIDEdit"] != null)
        {
            int len = TxtNewPswrd.Text.Trim().Length;
            if (len < 8)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Password Must Contain atleast 8 characters', 'error');", true);
                return;
            }

            if (TxtNewPswrd.Text != TxtCnfrmPswrd.Text)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm password does not match', 'warning');", true);
                return;
            }

            var Get_EmailID = Comm.GetAllDataFromTable("Register_Partner with (Nolock)", "EMail_primary", "PartnerID='" + Request.QueryString["ResellerIDEdit"].ToString() + "' ", "");
            if (Get_EmailID.Rows.Count > 0)
            {
                var Get_UserID = Comm.GetAllDataFromTable("UserAcc with (Nolock)", "UserId", "Email='" + Get_EmailID.Rows[0]["EMail_primary"].ToString() + "' ", "");
                cpCls.UserId = int.Parse(Get_UserID.Rows[0]["UserId"].ToString());
                cpCls.Password = TxtNewPswrd.Text;
                cpctrl.UpdateChangePassword(cpCls);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Password has been updated successfully', 'success');", true);
                //if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
                //{
                //    Response.Redirect("SuperAdmin_Approve_Subscriber.aspx");
                //}
            }
            if (Get_EmailID.Rows.Count == 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Password cannot be updated as user is not activated.', 'warning');", true);
            }
            return;
        }

        if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() != "support@anyminute.net")
        {
            Change_The_Password();
            return;
        }

        if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
        {
            Change_The_Password();
            return;
        }

        if (Session["UserType"].ToString() == "Admin")
        {
            Change_The_Password();
            return;
        }

        //For business owners
        if (Session["UserTypeid"].ToString() == "1")
        {
            var Dt_Check_Deactive_Subscriber = Comm.GetAllDataFromTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and UserType='1'", "");
            if (Dt_Check_Deactive_Subscriber.Rows[0]["IsActive"].ToString() == "NO        ")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Subscriber is Deactivated, So Password cannot be Changed', 'error');", true);
                return;
            }
            Change_The_Password();
            return;
        }               

        //To check the old user who are registered before the role development
        DataTable Dt_Check_If_User_Role_Permission = Comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Session["EmployeeId"].ToString() + "' ", "");
        if (Dt_Check_If_User_Role_Permission.Rows.Count == 0)
        {
            Change_The_Password();
            return;
        }

        if (Dt_Check_If_User_Role_Permission.Rows.Count > 0)
        {
            string string_Check_Role_Permission = Dt_Check_If_User_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
            string[] splittedArray = string_Check_Role_Permission.Split(',');
            foreach (string role_check_for_password_change in splittedArray)
            {
                #region For_role_check_for_password_chnage definition
                //For employee user type
                if(Session["UserTypeid"].ToString() == "4")
                {
                    if (role_check_for_password_change == "4")
                    {
                        Change_The_Password();
                        role_check_for_change_pssword_role_message_throw = "31";
                    }
                    else
                        role_check_for_change_pssword_role_message_throw = role_check_for_password_change;
                }

                //For manager user type
                if (Session["UserTypeid"].ToString() == "2")
                {
                    if (role_check_for_password_change == "12")
                    {
                        Change_The_Password();
                        role_check_for_change_pssword_role_message_throw = "31";
                    }
                    else
                        role_check_for_change_pssword_role_message_throw = role_check_for_password_change;
                }

                //For owner user type
                if (Session["UserTypeid"].ToString() == "5")
                {
                    if (role_check_for_password_change == "23")
                    {
                        Change_The_Password();
                        role_check_for_change_pssword_role_message_throw = "31";
                    }
                    else
                        role_check_for_change_pssword_role_message_throw = role_check_for_password_change;
                }

                //For accountant user type
                if (Session["UserTypeid"].ToString() == "12")
                {
                    if (role_check_for_password_change == "28")
                    {
                        Change_The_Password();
                        role_check_for_change_pssword_role_message_throw = "31";
                    }
                    else
                        role_check_for_change_pssword_role_message_throw = role_check_for_password_change;
                }

                //For custom role user type
                if (Session["UserTypeid"].ToString() != "4" && Session["UserTypeid"].ToString() != "2" && Session["UserTypeid"].ToString() != "5" && Session["UserTypeid"].ToString() != "12")
                {
                    if (role_check_for_password_change == "4" || role_check_for_password_change == "12" || role_check_for_password_change == "23" || role_check_for_password_change == "28")
                    {
                        Change_The_Password();
                        role_check_for_change_pssword_role_message_throw = "31";
                    }
                    else
                        role_check_for_change_pssword_role_message_throw = role_check_for_password_change;
                }
                #endregion
            }

            if (role_check_for_change_pssword_role_message_throw == "0")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Change The Password', 'error');", true);
                return;
            }
            if (role_check_for_change_pssword_role_message_throw != "31")
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Role Is Not Assigned To Change The Password', 'error');", true);
            }
            
        }
    }

    public void Change_The_Password()
    {
        bool check;
        check = Comm.IfExistsInTable("UserAcc", "*", "UserId=" + Session["UserId"].ToString() + " and convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))='" + TxtCrntPswrd.Text + "' COLLATE SQL_Latin1_General_CP1_CS_AS ");
        if (check == false)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Old password is not correct', 'warning');", true);
            return;
        }

        if (TxtNewPswrd.Text != TxtCnfrmPswrd.Text)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Confirm password does not match', 'warning');", true);
            return;
        }
        int len = TxtNewPswrd.Text.Trim().Length;
        if (len < 8)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Password Must Contain atleast 8 characters', 'error');", true);
            return;
        }
        
        cpCls.UserId = int.Parse(Session["UserId"].ToString());
        cpCls.Password = TxtNewPswrd.Text;
        cpctrl.UpdateChangePassword(cpCls);
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Password has been updated successfully', 'success');", true);

        if (Session["UserType"].ToString() == "Admin")
        {
            Response.Redirect("Accountant.aspx");
        }
        else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() != "support@anyminute.net")
        {
            Response.Redirect("SuperAdmin.aspx");
        }
        else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
        {
            Response.Redirect("SuperAdmin_Approve_Subscriber.aspx");
        }
        else if (Session["UserType"].ToString() == "SuperAdmin")
        {
            Response.Redirect("SuperAdmin.aspx");
        }
        else
        {
            Response.Redirect("Home_Page.aspx");
        }
    }
}