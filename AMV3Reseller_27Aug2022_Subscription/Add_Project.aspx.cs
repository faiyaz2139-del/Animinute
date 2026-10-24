using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Add_Project : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    //AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    AddProject AddProject = new AddProject();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            bool Check_Subscriber_Active_Status = comm.IfExistsInTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and IsActive='NO        ' and UserType='1' ");
            if (Check_Subscriber_Active_Status)
            {
                //Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                txtProjectName.Enabled = false;
                txtstartdate.Enabled = false;
                txtenddate.Enabled = false;
                ListBox_Ddl_Business_Name.Enabled = false;
                btnSaveProjectDetails.Visible = false;
                ddl_Business_Name.Enabled = false;
                //DisableAllControlsInPage(Page, false);
                return;
            }

            #region show message if maximum limit is reached. This region code is same as of in Save button.
            if (Session["UserTypeid"].ToString() == "1")
            {
                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner", "EMail_primary", "EMail_primary = '" + check_reseller_email.Rows[0]["Email"].ToString() + "' ");
                if (!check_reseller_or_subscriber_email)
                {
                    var check_old_subscriber = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_subscriber.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                //Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                txtProjectName.Enabled = false;
                                txtstartdate.Enabled = false;
                                txtenddate.Enabled = false;
                                ListBox_Ddl_Business_Name.Enabled = false;
                                btnSaveProjectDetails.Visible = false;
                                ddl_Business_Name.Enabled = false;
                                //DisableAllControlsInPage(Page, false);
                                return;
                            }

                            //var empidddddddd = Session["EmployeeId"].ToString();
                            //DataTable dtBussCnttt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + empidddddddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empidddddddd + "' and IsActive=1)");
                            //DataTable dt_NoOfBusiness = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeId"].ToString()), int.Parse("1"));
                            //if (dtBussCnttt.Rows.Count > 0 && dt_NoOfBusiness.Rows.Count > 0)
                            //{
                            //    if (dt_NoOfBusiness.Rows.Count >= int.Parse(dtBussCnttt.Rows[0][0].ToString()))
                            //    {
                            //        //Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                            //        //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //        txtProjectName.Enabled = false;
                            //        txtstartdate.Enabled = false;
                            //        txtenddate.Enabled = false;
                            //        ListBox_Ddl_Business_Name.Enabled = false;
                            //        btnSaveProjectDetails.Visible = false;
                            //        ddl_Business_Name.Enabled = false;
                            //        //DisableAllControlsInPage(Page, false);
                            //        return;
                            //    }
                            //}

                            //var empiddddddd = Session["EmployeeId"].ToString();
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  No_Of_Users from PlanHistory GROUP BY EmpId,id,No_Of_Users having EmpId='" + empiddddddd + "' and Id=(select max(id) from PlanHistory where EmpId='" + empiddddddd + "' and IsActive=1)");
                            //int empcount = 0;
                            ////commented because we are now counting only IsActive Yes Users Only for the total no of users validation
                            ////DataTable dtBussList = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                            //DataTable dtBussList = comm.ExecuteQueryDataTable("Select DISTINCT AB.BusinessID, (SELECT COUNT(AssginBussinessId) FROM AssginBussiness " +
                            //    "WHERE BusinessID=AB.BusinessID AND UserTypeId not in (1) " +
                            //    "AND EmployeeID in (select EmployeeID from AddEmployee where IsActive='YES') ) AS CNT from AddBusiness AB " +
                            //    "join AssginBussiness ASB ON AB.BusinessID=ASB.BusinessID " +
                            //    "LEFT JOIN AssgineManager ASSM ON AB.BusinessID=ASSM.BusinessID " +
                            //    "WHERE ASB.UserTypeId=1 AND ASB.EmployeeID = '" + int.Parse(Session["EmployeeID"].ToString()) + "' ");
                            //for (int m = 0; m < dtBussList.Rows.Count; m++)
                            //    empcount = empcount + Convert.ToInt32(dtBussList.Rows[m]["CNT"].ToString());
                            //if (empcount >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            //{
                            //    //Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                            //    //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //    txtProjectName.Enabled = false;
                            //    txtstartdate.Enabled = false;
                            //    txtenddate.Enabled = false;
                            //    ListBox_Ddl_Business_Name.Enabled = false;
                            //    btnSaveProjectDetails.Visible = false;
                            //    ddl_Business_Name.Enabled = false;
                            //    //DisableAllControlsInPage(Page, false);
                            //    return;
                            //}
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            //Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                            //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            txtProjectName.Enabled = false;
                            txtstartdate.Enabled = false;
                            txtenddate.Enabled = false;
                            ListBox_Ddl_Business_Name.Enabled = false;
                            btnSaveProjectDetails.Visible = false;
                            ddl_Business_Name.Enabled = false;
                            //DisableAllControlsInPage(Page, false);
                            //return;
                        }
                    }
                    if (check_old_subscriber.Rows.Count == 0)
                    {
                        txtProjectName.Enabled = false;
                        txtstartdate.Enabled = false;
                        txtenddate.Enabled = false;
                        ListBox_Ddl_Business_Name.Enabled = false;
                        btnSaveProjectDetails.Visible = false;
                        ddl_Business_Name.Enabled = false;
                        return;
                    }
                }
                if (check_reseller_or_subscriber_email)
                {
                    var check_old_reseller = comm.ExecuteQueryDataTable("select No_Of_Users from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_reseller.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                //Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                                //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                txtProjectName.Enabled = false;
                                txtstartdate.Enabled = false;
                                txtenddate.Enabled = false;
                                ListBox_Ddl_Business_Name.Enabled = false;
                                btnSaveProjectDetails.Visible = false;
                                ddl_Business_Name.Enabled = false;
                                //DisableAllControlsInPage(Page, false);
                                //return;
                            }

                            ////DataTable No_of_business = comm.GetAllDataFromTable("AddBusiness AsB", "AsB.BusinessID, AsB.CompanyName, AsB.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            //DataTable No_of_business = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeId"].ToString()), int.Parse("1"));
                            //int total_no_bussiness = 0;
                            //total_no_bussiness = int.Parse(No_of_business.Rows.Count.ToString());
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                            //int BUSSCNT_RESELLER = 0;
                            //BUSSCNT_RESELLER = int.Parse(dtBussCnt.Rows[0]["Business"].ToString());
                            //if (total_no_bussiness >= BUSSCNT_RESELLER)
                            //{
                            //    //Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                            //    //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //    txtProjectName.Enabled = false;
                            //    txtstartdate.Enabled = false;
                            //    txtenddate.Enabled = false;
                            //    ListBox_Ddl_Business_Name.Enabled = false;
                            //    btnSaveProjectDetails.Visible = false;
                            //    ddl_Business_Name.Enabled = false;
                            //    //DisableAllControlsInPage(Page, false);
                            //    //return;
                            //}
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            //Label_Maximum_Limit.Text = "Please contact admin to add more user. Your plan has been cancelled.";
                            //Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            txtProjectName.Enabled = false;
                            txtstartdate.Enabled = false;
                            txtenddate.Enabled = false;
                            ListBox_Ddl_Business_Name.Enabled = false;
                            btnSaveProjectDetails.Visible = false;
                            ddl_Business_Name.Enabled = false;
                            //DisableAllControlsInPage(Page, false);
                            //return;
                        }
                    }
                    if (check_old_reseller.Rows.Count == 0)
                    {
                        txtProjectName.Enabled = false;
                        txtstartdate.Enabled = false;
                        txtenddate.Enabled = false;
                        ListBox_Ddl_Business_Name.Enabled = false;
                        btnSaveProjectDetails.Visible = false;
                        ddl_Business_Name.Enabled = false;
                        return;
                    }
                }
            }
            #endregion

            Get_Business_Name_List();
        }
    }

    public void Get_Business_Name_List()
    {
        var empid = Session["EmployeeId"].ToString();
        if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
        {            
            comm.FillGridView(GridView_Business_List, "AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID", "(ab.CompanyName)BusinessName", "EmployeeID=" + empid + " and ab.Status=1", "ab.BusinessID");
        }

        DataTable Get_Business_Name_In_Ddl = comm.GetAllDataFromTable("AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID,(ab.CompanyName)BusinessName", "EmployeeID=" + empid + " and ab.Status=1", "ab.BusinessID");
        ddl_Business_Name.Items.Add("Select Business");
        ddl_Business_Name.SelectedItem.Value = "";
        ddl_Business_Name.DataSource = Get_Business_Name_In_Ddl;
        ddl_Business_Name.DataValueField = Get_Business_Name_In_Ddl.Columns[0].ToString();
        ddl_Business_Name.DataTextField = Get_Business_Name_In_Ddl.Columns[1].ToString();
        ddl_Business_Name.DataBind();
        ddl_Business_Name.SelectedIndex = 0;


        ListBox_Ddl_Business_Name.DataSource = Get_Business_Name_In_Ddl;
        ListBox_Ddl_Business_Name.DataTextField = "BusinessName";
        ListBox_Ddl_Business_Name.DataValueField = "BusinessID";
        ListBox_Ddl_Business_Name.DataBind();

        //ListBox_Business_Name.DataSource = Get_Business_Name_In_Ddl;
        //ListBox_Business_Name.DataTextField = "BusinessName";
        //ListBox_Business_Name.DataValueField = "BusinessID";
        //ListBox_Business_Name.DataBind();
    }

    protected void btnSaveProjectDetails_Click(object sender, EventArgs e)
    {
        if (this.txtProjectName.Text.Trim() == string.Empty)
        {
            lblerror.Text = "Project Name is Requried";
            lblerror.Visible = true;
            txtProjectName.Focus();
            return;
        }
        if (this.txtProjectName.Text.Trim() != string.Empty)
        {
            lblerror.Text = "";
            lblerror.Visible = false;
        }
        if (this.txtstartdate.Text.Trim() == string.Empty)
        {
            lblerror1.Text = "Start Date is Requried";
            lblerror1.Visible = true;
            txtstartdate.Focus();
            return;
        }
        if (this.txtstartdate.Text.Trim() != string.Empty)
        {
            lblerror1.Text = "";
            lblerror1.Visible = false;
        }
        if (txtstartdate.Text.Trim().Length > 0 && txtenddate.Text.Trim().Length > 0)
        {
            DateTime dtfrom = DateTime.Parse(txtstartdate.Text);
            DateTime dtto = DateTime.Parse(txtenddate.Text);
            if (dtfrom > dtto)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Start date is greater than End date!')", true);
                return;
            }
        }

        //DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectName='" + txtProjectName.Text + "' and Status='A' ", "");
        //if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
        //{
        //    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Project Name Already Exists', 'error');", true);
        //    return;
        //}

        var check_if_business_name_selected = 0;
        foreach (ListItem item_Ddl_Business_Name in ListBox_Ddl_Business_Name.Items)
        {
            if (!item_Ddl_Business_Name.Selected)
            {
                check_if_business_name_selected = 0;
            }
            if (item_Ddl_Business_Name.Selected)
            {
                check_if_business_name_selected = 1;
                break;
            }
        }
        if (check_if_business_name_selected == 0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please select business name to assign project', 'error');", true);
            return;
        }

        int ProjId = 0;
        foreach (ListItem item_Ddl_Business_Name in ListBox_Ddl_Business_Name.Items)
        {
            if (item_Ddl_Business_Name.Selected)
            {
                DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectName='" + txtProjectName.Text + "' and Status='A' " +
                    "and BusinessID = " + int.Parse(item_Ddl_Business_Name.Value) + " ", "");
                if (Dt_Check_If_Project_Name_Exists.Rows.Count == 0)
                {
                    AddProject.ProjectName = txtProjectName.Text.Trim().ToUpper();
                    AddProject.StartDate = DateTime.Parse(txtstartdate.Text);
                    AddProject.EndDate = txtenddate.Text;
                    AddProject.BusinessID = int.Parse(item_Ddl_Business_Name.Value);
                    ProjId = AddEmpCtrl.InsertProjectDetails(AddProject);
                }
                if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Warning!', 'Project Name Already Exists', 'warning');", true);
                }

                //AddProject.ProjectName = txtProjectName.Text.Trim().ToUpper();
                //AddProject.StartDate = DateTime.Parse(txtstartdate.Text);
                //AddProject.EndDate = txtenddate.Text;
                //AddProject.BusinessID = int.Parse(item_Ddl_Business_Name.Value);
                //ProjId = AddEmpCtrl.InsertProjectDetails(AddProject);
            }
        }

        if (ProjId > 0)
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Success!', 'Project Saved', 'success');", true);
            Label_PopUP_Message.Text = "Project Saved Successfully";
            Modelconfirmmessagepopup.Show();
            //Response.Redirect("Home_Page.aspx");
        }
    }

    protected void grdviewProject_PreRender(object sender, EventArgs e)
    {
        if (grdviewProject.Rows.Count > 0)
        {
            grdviewProject.UseAccessibleHeader = true;
            grdviewProject.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }    

    protected void grdviewProject_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "deleteproject")
        {
            string confirmValue = Request.Form["confirm_value"];
            if (confirmValue == "Yes" || confirmValue == "null" || confirmValue == null)
            {

            }
            else
            {
                return;
            }

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Label Label_ProjectName_ = (Label)grdviewProject.Rows[index].FindControl("Label_ProjectName");
            int hdfBusinessID, hdfProject = 0;
            hdfProject = Convert.ToInt32(commandArguments[1].ToString());
            hdfBusinessID = Convert.ToInt32(commandArguments[2].ToString());
            DataTable Check_Employee_Assigned_In_Project = comm.GetAllDataFromTable("TimeSheetEntry with (Nolock)", "ProjectId", "ProjectID='" + hdfProject.ToString() + "'", "");
            DataTable Check_Employee_Added_In_Project = comm.GetAllDataFromTable("[ASSIGNPROJECT] with (Nolock)", "[PROJECTID]", "[PROJECTID]='" + hdfProject.ToString() + "'", "");
            if (Check_Employee_Assigned_In_Project.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Employee has been Assigned in " + Label_ProjectName_.Text + " Project, So it can not be deleted.', 'warning');", true);
                return;
            }
            if (Check_Employee_Added_In_Project.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Employee has been Assigned in " + Label_ProjectName_.Text + " Project, So it can not be deleted.', 'warning');", true);
                return;
            }
            if (Check_Employee_Assigned_In_Project.Rows.Count == 0)
            {
                comm.ExecuteQuery("Update AddProject set Status='D' where ProjectID='" + hdfProject.ToString() + "' AND BusinessID ='" + hdfBusinessID.ToString() + "' ");
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + Label_ProjectName_.Text + " Has Been Deleted', 'success');", true);
                GetProjlist();
            }
        }

        if (e.CommandName == "editproject")
        {
            Label_Message.Text = "";
            Label_Message.Visible = false;
            TextBox_Project_Name_Edit.Text = String.Empty;
            TextBox_Start_Date_Edit.Text = String.Empty;
            TextBox_End_Date_Edit.Text = String.Empty;
            HiddenField_ProjedtID_ForEdit.Value = "";
            HiddenField_BusinessID_ForProjectEdit.Value = "";

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            Label Label_ProjectName_ = (Label)grdviewProject.Rows[index].FindControl("Label_ProjectName");
            HiddenField HiddenField_ProjectID_ = (HiddenField)grdviewProject.Rows[index].FindControl("HiddenField_ProjectID");
            HiddenField HiddenField_BusinessID_ = (HiddenField)grdviewProject.Rows[index].FindControl("HiddenField_BusinessID");
            Label Label_startdate_ = (Label)grdviewProject.Rows[index].FindControl("Label_startdate");
            Label Label_enddate_ = (Label)grdviewProject.Rows[index].FindControl("Label_enddate");
            hdn_projectname.Value = Label_ProjectName_.Text;
            TextBox_Project_Name_Edit.Text = Label_ProjectName_.Text;
            if (Label_startdate_.Text != "")
            {
                TextBox_Start_Date_Edit.Text = DateTime.Parse(Label_startdate_.Text).ToString("yyyy-MM-dd");
            }
            if (Label_enddate_.Text != "")
            {
                TextBox_End_Date_Edit.Text = DateTime.Parse(Label_enddate_.Text).ToString("yyyy-MM-dd");
            }
            HiddenField_ProjedtID_ForEdit.Value = HiddenField_ProjectID_.Value;
            HiddenField_BusinessID_ForProjectEdit.Value = HiddenField_BusinessID_.Value;
            modaladdproject.Show();
        }
    }

    protected void ddl_Business_Name_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_Business_Name.SelectedIndex == 0)
        {
            grdviewProject.DataSource = null;
            grdviewProject.DataBind();
        }
        if (ddl_Business_Name.SelectedIndex != 0)
        {
            GetProjlist();
        }
    }

    protected void GetProjlist()
    {
        grdviewProject.DataSource = null;
        grdviewProject.DataBind();
        comm.FillGridView(grdviewProject, "ADDPROJECT with (Nolock)", "BusinessID,ProjectID,PROJECTNAME,CONVERT(NVARCHAR,startDate, 106) as startDate", "CONVERT(NVARCHAR,EndDate, 106) as EndDate,dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " Status='A' and BusinessID ='" + ddl_Business_Name.SelectedValue + "'", "BusinessID,ProjectID asc");
    }

    protected void GridView_Project_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_Project_List.Rows.Count > 0)
        {
            GridView_Project_List.UseAccessibleHeader = true;
            GridView_Project_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void Button_Update_Project_Click(object sender, EventArgs e)
    {
        if (this.TextBox_Project_Name_Edit.Text.Trim() == string.Empty)
        {
            Label1.Text = "Project Name is Requried";
            Label1.Visible = true;
            txtProjectName.Focus();
            return;
        }
        if (this.TextBox_Start_Date_Edit.Text.Trim() == string.Empty)
        {
            Label2.Text = "Start Date is Requried";
            Label2.Visible = true;
            txtstartdate.Focus();
            return;
        }
        if (TextBox_Start_Date_Edit.Text.Trim().Length > 0 && TextBox_End_Date_Edit.Text.Trim().Length > 0)
        {
            DateTime dtfrom = DateTime.Parse(TextBox_Start_Date_Edit.Text);
            DateTime dtto = DateTime.Parse(TextBox_End_Date_Edit.Text);
            if (dtfrom > dtto)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Start date is greater than End date!')", true);
                return;
            }
        }

        if (hdn_projectname.Value != TextBox_Project_Name_Edit.Text)
        {
            DataTable Dt_Check_If_Project_Name_Exists = comm.GetAllDataFromTable("AddProject with (Nolock)", "ProjectName", "ProjectName='" + TextBox_Project_Name_Edit.Text + "' and Status='A' ", "");
            if (Dt_Check_If_Project_Name_Exists.Rows.Count > 0)
            {
                Label_Message.Text = "Project Name Already Exists";
                Label_Message.Visible = true;
                return;
            }
        }

        AddProject.ProjectName = TextBox_Project_Name_Edit.Text.Trim().ToUpper();
        AddProject.StartDate = DateTime.Parse(TextBox_Start_Date_Edit.Text);
        AddProject.EndDate = TextBox_End_Date_Edit.Text;
        AddProject.BusinessID = Convert.ToInt32(HiddenField_BusinessID_ForProjectEdit.Value);
        AddProject.ProjectID = Convert.ToInt32(HiddenField_ProjedtID_ForEdit.Value);        

        AddEmpCtrl.UpdateProjectDetails(AddProject);

        //Response.Redirect("Home_Page.aspx");
        Label_PopUP_Message.Text = "Project Updated Successfully";        
        Modelconfirmmessagepopup.Show();
        modaladdproject.Show();
    }

    //protected void Button_Show_Click(object sender, EventArgs e)
    //{
    //    var get_business_id = string.Empty;
    //    var get_all_selected_business_id = string.Empty;
    //    var check_if_business_selected = 0;
    //    StringBuilder StringBuilder__ = new StringBuilder();
    //    foreach (ListItem item_ListBox_Business_Name in ListBox_Business_Name.Items)
    //    {
    //        if (item_ListBox_Business_Name.Selected)
    //        {
    //            lblSelectedTech.Text += ListBox_Business_Name.SelectedValue + ",";
    //            StringBuilder__.Append(ListBox_Business_Name.SelectedValue + ",");
    //            //get_business_id = ListBox_Business_Name.SelectedValue + ",";
    //            check_if_business_selected = 1;
    //        }
    //    }

    //    if (check_if_business_selected == 1)
    //    {
    //        //get_business_id = get_business_id.Remove(get_business_id.Length - 1);
    //        get_all_selected_business_id = StringBuilder__.ToString().TrimEnd(',');
    //        lblSelectedTech.Text = lblSelectedTech.Text.ToString().TrimEnd(',');
    //        grdviewProject.DataSource = null;
    //        grdviewProject.DataBind();
    //        //comm.FillGridView(grdviewProject, "ADDPROJECT", "BusinessID,ProjectID,PROJECTNAME,CONVERT(NVARCHAR,startDate, 106) as startDate", "CONVERT(NVARCHAR,EndDate, 106) as EndDate,dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " Status='A' and BusinessID IN (" + get_business_id + ")", "BusinessID,ProjectID asc");
    //        comm.FillGridView(grdviewProject, "ADDPROJECT with (Nolock)", "BusinessID,ProjectID,PROJECTNAME,CONVERT(NVARCHAR,startDate, 106) as startDate", "CONVERT(NVARCHAR,EndDate, 106) as EndDate,dbo.FN_DESC('AddBusiness',BusinessID)BusinessName", " Status='A' and BusinessID IN (" + lblSelectedTech.Text + ")", "BusinessID,ProjectID asc");
    //    }
    //    if (check_if_business_selected == 0)
    //    {
    //        grdviewProject.DataSource = null;
    //        grdviewProject.DataBind();
    //    }
    //}

    protected void btn_Okay_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home_Page.aspx");
    }

    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Add_EmployeeV2.aspx");
    }
}