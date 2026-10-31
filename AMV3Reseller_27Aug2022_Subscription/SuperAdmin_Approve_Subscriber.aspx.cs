using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using log4net;
using System.Data.SqlClient;
using System.Globalization;

public partial class SuperAdmin_Approve_Subscriber : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            Get_New_Account_Business_Owner();
        }
    }

    public void Get_New_Account_Business_Owner()
    {
        DataTable DataTable_Subscriber_Reseller_List = Get_Subscriber_Reseller_List();
        GridView_New_Registered_BO.DataSource = DataTable_Subscriber_Reseller_List;
        GridView_New_Registered_BO.DataBind();
    }

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    private SqlConnection con;
    public void getconnection()
    {
        string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        con = new SqlConnection(strcon);
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
    }
    public DataTable Get_Subscriber_Reseller_List()
    {
        try
        {
            loginfo = "Get_Subscriber_Reseller_List() Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SP_Get_Subscriber_Reseller_List";
            //cmd.CommandText = "SP_Get_Subscriber_List";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Get_Subscriber_Reseller_List() end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Get_Subscriber_Reseller_List" + ex.Message);
            throw ex;
        }
    }

    //protected void GridView_New_Registered_BO_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    GridView_New_Registered_BO.PageIndex = e.NewPageIndex;
    //    Get_New_Account_Business_Owner();
    //}

    protected void GridView_New_Registered_BO_PreRender(object sender, EventArgs e)
    {
        if (GridView_New_Registered_BO.Rows.Count > 0)
        {
            GridView_New_Registered_BO.UseAccessibleHeader = true;
            GridView_New_Registered_BO.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_New_Registered_BO_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Activate_Deactivate_New_Account_Business_Owner")
        {
            string status_change_confirm_value = Request.Form["Toggle_activate_deactivate"];
            if (status_change_confirm_value == "Yes" || status_change_confirm_value == "null" || status_change_confirm_value == null)
            {

            }
            else
            {
                return;
            }

            int index = Convert.ToInt32(e.CommandArgument);
            Label Label_Check_Subscriber_Type = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Subscriber_Type");

            if (Label_Check_Subscriber_Type.Text == "Subscriber")
            {
                HiddenField HiddenField_Business_Owner_IsActive_Status = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Business_Owner_IsActive_Status");
                HiddenField HiddenField_Employee_Id = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Employee_Id");
                Label Label_Partner_Name_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Employee_Name");
                Label HiddenField_EMail_primary_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Email_Id");

                if (HiddenField_Business_Owner_IsActive_Status.Value != string.Empty)
                {
                    if (HiddenField_Business_Owner_IsActive_Status.Value == "YES       ")
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");

                        if (Label_Check_Subscriber_Type.Text == "Subscriber")
                        {
                            int empid = Convert.ToInt32(HiddenField_Employee_Id.Value);
                            
                            //SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
                            DataTable dt = new DataTable();
                            dt = supportAdminCtrl.GetEmplist_SuppAdmin(Label_Check_Subscriber_Type.Text, empid, 0);
                            if (dt.Rows.Count > 0)
                            {
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                                    //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                                    Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                                }
                            }

                            DataTable dtSubAllBuss = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID='" + HiddenField_Employee_Id.Value + "' and UserTypeId=1", "");
                            foreach (DataRow rowBuss in dtSubAllBuss.Rows)
                            {
                                if (rowBuss["BusinessID"].ToString() != string.Empty)
                                {
                                    Comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                                }
                            }

                            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Subscriber is deactivated!', 'success');", true);
                            Get_New_Account_Business_Owner();
                        }

                        //Code for mail sending on deactivating subscriber
                        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                        if (IsSend)
                        {
                            Send_Email_Id = HiddenField_EMail_primary_.Text;
                            string Host = ConfigurationManager.AppSettings["ResetUrl"];
                            string url = Host + Session["email"].ToString();
                            DataTable dtEmail_Send_Deactivation = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=5", "");
                            //DataTable dtemp = Comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + Send_Email_Id + "'", "ae.EmployeeID desc");
                            //DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email='" + Send_Email_Id + "' ", "");
                            string body = Esctrl.PopulateBodyWithUrl(Label_Partner_Name_.Text, "BO Deactivation Notification", url, dtEmail_Send_Deactivation.Rows[0]["description"].ToString(), dtEmail_Send_Deactivation.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com" + " <br />User Id - No_User_Id", "No_Password");
                            Esctrl.SendHtmlFormattedEmail(Send_Email_Id, dtEmail_Send_Deactivation.Rows[0]["Subject"].ToString(), body);
                        }
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Deactivation Mail Sent To Registered Email Id', 'success');", true);
                    }

                    if (HiddenField_Business_Owner_IsActive_Status.Value == "NO        ")
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");                        

                        int empid = Convert.ToInt32(HiddenField_Employee_Id.Value);

                        //SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
                        DataTable dt = new DataTable();
                        dt = supportAdminCtrl.GetEmplist_SuppAdmin(Label_Check_Subscriber_Type.Text, empid, 0);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                                //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                            }
                        }

                        DataTable dtSubAllBuss = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID='" + HiddenField_Employee_Id.Value + "' and UserTypeId=1", "");
                        foreach (DataRow rowBuss in dtSubAllBuss.Rows)
                        {
                            if (rowBuss["BusinessID"].ToString() != string.Empty)
                            {
                                Comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                            }
                        }

                        Get_New_Account_Business_Owner();
                        //Comm.ExecuteQuery("update UserAcc set Is_First_Time_Login='True' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");

                        DataTable dtemp = Comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "ae.EmployeeID='" + HiddenField_Employee_Id.Value + "'", "ae.EmployeeID desc");
                        if (dtemp.Rows.Count > 0)
                        {
                            string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
                            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                            DataTable dtEmailTemp = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=3", "");
                            if (dtEmailTemp.Rows.Count > 0)
                            {
                                DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                                if (IsSend)
                                    Esctrl.SendHtmlFormattedEmail(dtemp.Rows[0]["MainEmail"].ToString(), dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Subscriber is activated and email has been sent successfully', 'success');", true);
                                //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Business Owner Has Been Activated Successfully', 'success');", true);
                            }
                            else
                            {
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email template is not found in dataBase', 'warning');", true);
                            }
                        }

                    }
                }
            }

            if (Label_Check_Subscriber_Type.Text == "Reseller")
            {                
                HiddenField HiddenField_Partner_IsActive_Status_ = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Business_Owner_IsActive_Status");
                HiddenField HiddenField_Partner_Id_ = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Partner_Id");
                Label Label_Partner_Name_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Employee_Name");
                Label HiddenField_HireDate_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Enrolled_Date");
                Label HiddenField_EMail_primary_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Email_Id");
                Label HiddenField_MobilePhone_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Contact_Phone");
                Label HiddenField_ProfilePic_ = (Label)GridView_New_Registered_BO.Rows[index].FindControl("Label_Accountant_MotherOrg_Name");
                //HiddenField HiddenField_CompanyName_ = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_MotherOrg_Id");
                
                HiddenField HiddenField_Employee_Id = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Employee_Id");
                int empid = 0;
                if (HiddenField_Employee_Id.Value != "")
                {
                    empid = Convert.ToInt32(HiddenField_Employee_Id.Value);
                }
                
                int MotherOrgId = 0;
                if (HiddenField_Partner_IsActive_Status_.Value != string.Empty)
                {
                    //MotherEc.MotherOrgName = Label_Partner_Name_.Text+'-'+HiddenField_Partner_Id_.Value;   // kumar commented
                    var chk = Comm.IfExistsInTable("MotherOrg", "*", "Email='" + HiddenField_EMail_primary_.Text + "'");
                    if(chk==false)
                    {
                        MotherEc.MotherOrgName = Label_Partner_Name_.Text + '-' + HiddenField_Partner_Id_.Value;
                        MotherEc.StreetNumber = "";
                        MotherEc.StreetName = "";
                        MotherEc.Apartment = "";
                        MotherEc.City = "";
                        MotherEc.Province = "";
                        MotherEc.PostalCode = "";
                        MotherEc.EnrollDate = DateTime.Parse(HiddenField_HireDate_.Text);
                        MotherEc.Status = true;
                        MotherEc.Email = HiddenField_EMail_primary_.Text;
                        Send_Email_Id = HiddenField_EMail_primary_.Text;
                        MotherEc.ContactName = Label_Partner_Name_.Text;
                        MotherEc.ContactNumber = HiddenField_MobilePhone_.Text;
                        MotherEc.StreetNumber = HiddenField_ProfilePic_.Text;
                        MotherEc.Checked = "N";
                        MotherOrgId = MotherC.InsertMotherOrg(MotherEc);
                    }
                    
                    if (HiddenField_Partner_IsActive_Status_.Value == "NO        ")
                    {
                        DataTable dt = new DataTable();
                        Comm.ExecuteQuery("update Register_Partner set IsActive='YES' where PartnerID='" + int.Parse(HiddenField_Partner_Id_.Value) + "'");
                        
                        DataTable dtres = Comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + empid + "and AsB.UserTypeId=1", "asb.EmployeeID");
                        if (dtres.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtres.Rows.Count; i++)
                            {
                                Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + dtres.Rows[i]["EmployeeID"].ToString() + "'");
                                //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + dtres.Rows[i]["EmployeeID"].ToString() + "'");

                                dt = supportAdminCtrl.GetEmplist_SuppAdmin("Reseller", empid, int.Parse(dtres.Rows[i]["EmployeeID"].ToString()));
                                if (dt.Rows.Count > 0)
                                {
                                    for (int j = 0; j < dt.Rows.Count; j++)
                                    {
                                        Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + dt.Rows[j]["EmployeeID"].ToString() + "'");
                                        //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                                        Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + dt.Rows[j]["EmployeeID"].ToString() + "'");
                                    }
                                }
                                
                            }
                        }

                        //added by kumar because the subscribers having no business was not getting activated
                        DataTable check_subscribers = Comm.GetAllDataFromTable("AddEmployee ae", "distinct ae.EmployeeID, dbo.FN_DESC('AddEmployee',ae.EmployeeID)empname ", "ae.MotherOrgId=" + empid + "and ae.UserType=1 ", "");
                        if (check_subscribers.Rows.Count > 0)
                        {
                            for (int i = 0; i < check_subscribers.Rows.Count; i++)
                            {
                                Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + check_subscribers.Rows[i]["EmployeeID"].ToString() + "'");
                                //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + check_subscribers.Rows[i]["EmployeeID"].ToString() + "'");
                            }
                        }

                        DataTable dtResellerAllBuss = Comm.GetAllDataFromTable("AddBusiness AsB", "AsB.BusinessID, AsB.CompanyName, AsB.Status, (select count(EmployeeID) from AssginBussiness where UserTypeId <> 1 AND BusinessID = AsB.BusinessID)empcnt", "MotherOrgId=" + empid + "", "");
                        foreach (DataRow rowBuss in dtResellerAllBuss.Rows)
                        {
                            if (rowBuss["BusinessID"].ToString() != string.Empty)
                            {
                                Comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                            }
                        }

                        Send_Email_Id = HiddenField_EMail_primary_.Text;
                        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                        if (IsSend)
                            SendEmail(HiddenField_EMail_primary_.Text, MotherOrgId.ToString());

                        Get_New_Account_Business_Owner();
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Reseller is activated and email has been sent successfully!', 'success');", true);
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        Comm.ExecuteQuery("update Register_Partner set IsActive='NO' where PartnerID='" + int.Parse(HiddenField_Partner_Id_.Value) + "'");
                        DataTable dtres= Comm.GetAllDataFromTable("AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID, dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + empid + "and AsB.UserTypeId=1", "asb.EmployeeID");
                        if (dtres.Rows.Count > 0)
                        {
                            for (int i = 0; i < dtres.Rows.Count; i++)
                            {
                                Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + dtres.Rows[i]["EmployeeID"].ToString() + "'");
                                //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + dtres.Rows[i]["EmployeeID"].ToString() + "'");

                                dt = supportAdminCtrl.GetEmplist_SuppAdmin("Reseller", empid, int.Parse(dtres.Rows[i]["EmployeeID"].ToString()));
                                if (dt.Rows.Count > 0)
                                {
                                    for (int j = 0; j < dt.Rows.Count; j++)
                                    {
                                        Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + dt.Rows[j]["EmployeeID"].ToString() + "'");
                                        //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                                        Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + dt.Rows[j]["EmployeeID"].ToString() + "'");
                                    }
                                }                                
                            }
                        }

                        //added by kumar because the subscribers having no business was not getting deactivated
                        DataTable check_subscribers = Comm.GetAllDataFromTable("AddEmployee ae", "distinct ae.EmployeeID, dbo.FN_DESC('AddEmployee',ae.EmployeeID)empname ", "ae.MotherOrgId=" + empid + "and ae.UserType=1 ", "");
                        if (check_subscribers.Rows.Count > 0)
                        {
                            for (int i = 0; i < check_subscribers.Rows.Count; i++)
                            {
                                Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + check_subscribers.Rows[i]["EmployeeID"].ToString() + "'");
                                //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + check_subscribers.Rows[i]["EmployeeID"].ToString() + "'");
                            }
                        }

                        DataTable dtResellerAllBuss = Comm.GetAllDataFromTable("AddBusiness AsB", "AsB.BusinessID, AsB.CompanyName, AsB.Status, (select count(EmployeeID) from AssginBussiness where UserTypeId <> 1 AND BusinessID = AsB.BusinessID)empcnt", "MotherOrgId=" + empid + "", "");
                        foreach (DataRow rowBuss in dtResellerAllBuss.Rows)
                        {
                            if (rowBuss["BusinessID"].ToString() != string.Empty)
                            {
                                Comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                            }
                        }

                        //Code for mail sending on deactivating reseller
                        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                        if (IsSend)
                        {
                            Send_Email_Id = HiddenField_EMail_primary_.Text;
                            //To check if user is partner or not. If user is partner, then the mail will be sent to user.
                            DataTable Dt_Check_If_User_As_A_Partner_Exists = Comm.GetAllDataFromTable("Register_Partner", "EMail_primary", "EMail_primary='" + Send_Email_Id + "' ", "");
                            if (Dt_Check_If_User_As_A_Partner_Exists.Rows.Count > 0)
                            {
                                string Host = ConfigurationManager.AppSettings["ResetUrl"];
                                string url = Host + Session["email"].ToString();
                                DataTable dtEmail_Send_Deactivation = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=5", "");

                                //DataTable dtemp = Comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.CCEmail='" + Send_Email_Id + "'", "ae.EmployeeID desc");
                                //DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email='" + Send_Email_Id + "' ", "");
                                string body = Esctrl.PopulateBodyWithUrl(Label_Partner_Name_.Text, "BO Deactivation Notification", url, dtEmail_Send_Deactivation.Rows[0]["description"].ToString(), dtEmail_Send_Deactivation.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com" + " <br />User Id - No_User_Id", "No_Password");
                                Esctrl.SendHtmlFormattedEmail(Send_Email_Id, dtEmail_Send_Deactivation.Rows[0]["Subject"].ToString(), body);
                                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Deactivation Mail Sent To Registered Email Id', 'success');", true);
                            }
                        }                            

                        Get_New_Account_Business_Owner();
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Reseller is deactivated!', 'success');", true);
                    }
                }
            }
        }

        if (e.CommandName == "bussList")
        {
            Control ctrl = e.CommandSource as Control;
            if (ctrl != null)
            {
                GridViewRow gvRow = ctrl.Parent.NamingContainer as GridViewRow;
                int empid = Convert.ToInt32(e.CommandArgument);
                Label Label_Check_Subscriber_Type = (Label)gvRow.FindControl("Label_Subscriber_Type");
                Label Label_Employee_Name = (Label)gvRow.FindControl("Label_Employee_Name");

                lblbusName.Text = Label_Employee_Name.Text;
                if (Label_Check_Subscriber_Type.Text == "Subscriber")
                {
                    GridView_PopUp_Business_List.DataSource = null;
                    GridView_PopUp_Business_List.DataBind();

                    Comm.FillGridView(GridView_PopUp_Business_List, "AssginBussiness AsB join AddBusiness ab  on ab.BusinessID = asb.BusinessID", "asb.BusinessID", "ab.CompanyName, ab.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "AsB.UserTypeId=1 and AsB.EmployeeID=" + empid + " AND AsB.BusinessID IS NOT NULL", "");
                    ModalPopupBussList.Show();
                }
                else
                {
                    GridView_PopUp_Business_List.DataSource = null;
                    GridView_PopUp_Business_List.DataBind();

                    Comm.FillGridView(GridView_PopUp_Business_List, "AddBusiness AsB", "AsB.BusinessID", "AsB.CompanyName, AsB.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "MotherOrgId=" + empid + "", "");
                    ModalPopupBussList.Show();
                }
            }
        }

        if (e.CommandName == "empList")
        {
            Control ctrlempList = e.CommandSource as Control;
            if (ctrlempList != null)
            {
                GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
                int empid = Convert.ToInt32(e.CommandArgument);
                Label Label_Check_Subscriber_Type = (Label)gvRow.FindControl("Label_Subscriber_Type");
                Label Label_Employee_Name = (Label)gvRow.FindControl("Label_Employee_Name");
                if(Label_Check_Subscriber_Type.Text=="Subscriber")
                {
                    lblempna.Text = Label_Employee_Name.Text;
                    //SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
                    DataTable dt = new DataTable();
                    dt = supportAdminCtrl.GetEmplist_SuppAdmin(Label_Check_Subscriber_Type.Text, empid,0);
                    gvEmplistPop.DataSource = dt;
                    gvEmplistPop.DataBind();
                    modelpopemp.Show();
                }
                else
                {
                    Response.Redirect("~/ReSellerList.aspx?type="+ Label_Check_Subscriber_Type.Text+"&empid="+ empid+"&name="+ Label_Employee_Name.Text, false);
                }                
            }
        }
    }

    protected void GridView_New_Registered_BO_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton ImageButton_Active_Deactive = (e.Row.FindControl("ImageButton_Active_Deactive") as LinkButton); // v37 (DOCX image4): status pill
            HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("HiddenField_Business_Owner_IsActive_Status") as HiddenField);
            HiddenField HiddenField_Employee_Id = (e.Row.FindControl("HiddenField_Employee_Id") as HiddenField);
            LinkButton lnkBusinessList = (e.Row.FindControl("lnkBusinessList") as LinkButton);
            LinkButton lnkEmployeeList = (e.Row.FindControl("lnkEmployeeList") as LinkButton);
            Label Label_Plan_Name_ = (e.Row.FindControl("Label_Plan_Name") as Label);

            if (HiddenField_Employee_Id.Value != "")
            {
                DataTable Dt_PlanName = Comm.ExecuteQueryDataTable("select PlanName from PlanHistory with (Nolock) where EmpId='" + int.Parse(HiddenField_Employee_Id.Value) + "' " +
                "and IsActive=1");
                if (Dt_PlanName.Rows.Count > 0)
                    Label_Plan_Name_.Text = Dt_PlanName.Rows[0]["PlanName"].ToString();

                DataTable Dt_PlansCount = Comm.ExecuteQueryDataTable("select PlanName from PlanHistory with (Nolock) where EmpId='" + int.Parse(HiddenField_Employee_Id.Value) + "' ");

                DataTable Dt_PlanNameEnterprise = Comm.ExecuteQueryDataTable("select PlanName from PlanHistory with (Nolock) where EmpId='" + int.Parse(HiddenField_Employee_Id.Value) + "' " +
                "and IsActive IS NULL");
                if (Dt_PlanNameEnterprise.Rows.Count > 0 && Dt_PlansCount.Rows.Count == 1)
                    Label_Plan_Name_.Text = Dt_PlanNameEnterprise.Rows[0]["PlanName"].ToString();
            }

            if (HiddenField_Employee_Id.Value == "")
            {
                Label_Plan_Name_.Text = "";
                lnkBusinessList.Enabled = false;
                lnkEmployeeList.Enabled = false;
            }

            if (HiddenField_Business_Owner_IsActive_Status.Value.ToString().Trim() == "YES")
            {
                ImageButton_Active_Deactive.Text = "Active";
                ImageButton_Active_Deactive.CssClass = "am-sa-pill is-active";
            }
            else
            {
                ImageButton_Active_Deactive.Text = "Inactive";
                ImageButton_Active_Deactive.CssClass = "am-sa-pill is-inactive";
            }

            Label Label_Enrolled_Date_ = (e.Row.FindControl("Label_Enrolled_Date") as Label);
            string input = Label_Enrolled_Date_.Text;
            DateTime dt;
            //if (DateTime.TryParse(input, out dt))
            //{
            //    string output = dt.ToString("yyyy-MM-dd");
            //}
            DateTime.TryParse(input, out dt);
            Label_Enrolled_Date_.Text = dt.ToString("yyyy-MM-dd");
        }
    }

    string Send_Email_Id;
    public void SendEmail(string email, string Motherid)
    {
        DataTable Dt_motherOrgId = Comm.GetAllDataFromTable("MotherOrg", "MotherOrgID", "Email='" + email + "'", "");
        string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        //string url = Host + Motherid.ToString() + "&LoginType=Acc"; //commented because motherorgid is coming 0 when activating 3rd or 4th time.
        string url = Host + Dt_motherOrgId.Rows[0]["MotherOrgID"].ToString() + "&LoginType=Acc";
        DataTable dtEmailTemp = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
        //DataTable dtemp = Comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Motherid + "", ""); //commented because giving error as motherorgid is coming 0 when activating 3rd or 4th time.
        DataTable dtemp = Comm.GetAllDataFromTable("MotherOrg", "*", "Email='" + email + "'", "");
        DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + Send_Email_Id.ToString() + "' ", "");
        string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["ContactName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), Send_Email_Id.ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
        Esctrl.SendHtmlFormattedEmail(email, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
    }

    protected void GridView_PopUp_Business_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ImageButton_Active_Deactive = (e.Row.FindControl("btnactivebus") as ImageButton);
            HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("hfdStatusForBussOwner") as HiddenField);
            if (HiddenField_Business_Owner_IsActive_Status.Value.ToString().Trim() == "1")
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/active.png";
            }
            else
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/deactive.png";
            }
        }
    }

    protected void GridView_PopUp_Business_List_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Businessstatus")
        {
            Control ctrlempList = e.CommandSource as Control;
            if (ctrlempList != null)
            {
                GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
                int BusinessID = Convert.ToInt32(e.CommandArgument);
                ImageButton ImageButton_Active_Deactive = (ImageButton)gvRow.FindControl("btnactivebus");
                var sour = ImageButton_Active_Deactive.ImageUrl;
                if(sour.Contains("~/images/active.png"))
                {
                    Comm.ExecuteQuery("update AddBusiness set Status=2 where BusinessID=" + BusinessID + "");
                    DataTable dt = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID", "BusinessID=" + BusinessID + " and BusinessID is not null and UserTypeId <>1", "");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                        //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                        Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                    }
                }
                else
                {
                    Comm.ExecuteQuery("update AddBusiness set Status=1 where BusinessID=" + BusinessID + "");
                    DataTable dt = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID", "BusinessID=" + BusinessID + " and BusinessID is not null ", "");
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                        //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                        Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                    }
                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
            }
        }
    }

    protected void GridView_PopUp_Business_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_PopUp_Business_List.Rows.Count > 0)
        {
            GridView_PopUp_Business_List.UseAccessibleHeader = true;
            GridView_PopUp_Business_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void gvEmplistPop_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EMPSTUS")
        {
            Control ctrlempList = e.CommandSource as Control;
            if (ctrlempList != null)
            {
                GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
                int empid = Convert.ToInt32(e.CommandArgument);
                ImageButton ImageButton_Active_Deactive = (ImageButton)gvRow.FindControl("btnactiveEMP");
                var sour = ImageButton_Active_Deactive.ImageUrl;
                if (sour.Contains("~/images/active.png"))
                {
                    Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + empid + "");
                    //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                    Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID=" + empid + "");
                }
                else
                {
                    Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + empid + "");
                    //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                    Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID=" + empid + "");
                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
            }
        }
    }
    protected void gvEmplistPop_PreRender(object sender, EventArgs e)
    {
        if (gvEmplistPop.Rows.Count > 0)
        {
            gvEmplistPop.UseAccessibleHeader = true;
            gvEmplistPop.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void gvEmplistPop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ImageButton_Active_Deactive = (e.Row.FindControl("btnactiveEMP") as ImageButton);
            HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("hdbID") as HiddenField);
            if (HiddenField_Business_Owner_IsActive_Status.Value.ToString().Trim() == "YES")
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/active.png";
            }
            else
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/deactive.png";
            }
        }
    }
}
