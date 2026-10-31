using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Threading;

public partial class Accountant : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    ActiveDeactiveController ACCtrl = new ActiveDeactiveController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (AnyminuteAccountantDashboard.Applies()) { Response.Redirect("Dashboard.aspx", true); return; } // v37: PPT Accountant & Payroll Dashboard
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            GetEmplist();

            // To check if user is partner or not and deactivate the status dropdown
            DataTable Dt_Check_If_User_As_A_Partner_Exists = comm.GetAllDataFromTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary='" + Session["email"].ToString() + "' ", "");
            if (Dt_Check_If_User_As_A_Partner_Exists.Rows.Count > 0)
            {
                Panel_Check_If_User_As_A_Partner_Exists.Visible = false;
            }
            else
                Panel_Check_If_User_As_A_Partner_Exists.Visible = false;

            var check_deactive_reseller = comm.GetAllDataFromTable("Register_Partner with (Nolock)", "IsActive", "EMail_primary='" + Session["email"].ToString() + "' and EMail_secondary='" + Session["email"].ToString() + "'", "");
            if (check_deactive_reseller.Rows.Count > 0)
            {
                if (check_deactive_reseller.Rows[0]["IsActive"].ToString() == "NO        ")
                {
                    GvDivAdminBussOwner.Enabled = false;
                    hyperAdminAddEmp.Visible = false;
                }
            }
        }
    }
    public void GetEmplist()
    {
        DataTable dtAccDetails = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Status", "MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
        if (dtAccDetails.Rows.Count > 0)
        {
            if (dtAccDetails.Rows[0]["Status"].ToString() != "False")
            {
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad with (Nolock) join EmpAddress EmpAdd with (Nolock) on EmpAdd.EmpAddressId=AD.ADDRESSID and ad.UserType=1 join UserAcc ua with (Nolock) on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB with (Nolock) ON ASSB.EmployeeID=ad.EmployeeId", "DISTINCT AD.EmployeeID as useraccempid,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, EmpAdd.MainPhone, EmpAdd.MainEmail,CONVERT(NVARCHAR,Ad.HireDate, 106) as HireDate,IsActive", "(select COUNT(*) from AssginBussiness asb with (Nolock) where asb.EmployeeID = ad.EmployeeId and asb.UserTypeId=1  and Asb.MotherOrgID=" + Session["MotherOrgid"].ToString() + "  and BusinessID is not null)bussinessCount,dbo.getCompanyNameByMotherOrgid(cast (Ad.EmployeeId as int)) as'CompanyName' ", "ad.UserType=1 and (Ad.MotherOrgID=" + Session["MotherOrgid"].ToString() + " or Assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + ")", "AD.EmployeeID desc");
            }
        }
    }

    protected void ddl_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Session["EmployeeId"].ToString() == "0" && ddl_status.SelectedValue == "")
        {
            comm.FillGridView(GvDivAdminBussOwner, "AddEmployee Ad with (Nolock) join EmpAddress EmpAdd with (Nolock) on EmpAdd.EmpAddressId=AD.ADDRESSID and ad.UserType=1 join UserAcc ua on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB with (Nolock) ON ASSB.EmployeeID=ad.EmployeeId", "DISTINCT AD.EmployeeID as useraccempid,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, EmpAdd.MainPhone, EmpAdd.MainEmail,CONVERT(NVARCHAR,Ad.HireDate, 106) as HireDate,IsActive", "(select COUNT(*) from AssginBussiness asb where asb.EmployeeID = ad.EmployeeId and asb.UserTypeId=1   and Asb.MotherOrgID=" + Session["MotherOrgid"].ToString() + "  and BusinessID is not null)bussinessCount", "ad.UserType=1 and (Ad.MotherOrgID=" + Session["MotherOrgid"].ToString() + " or Assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + ")", "AD.EmployeeID desc");
        }
        if (Session["EmployeeId"].ToString() == "0" && ddl_status.SelectedValue == "YES")
        {
            comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad with (Nolock) join EmpAddress EmpAdd with (Nolock) on EmpAdd.EmpAddressId=AD.ADDRESSID and ad.UserType=1 join UserAcc ua on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB with (Nolock) ON ASSB.EmployeeID=ad.EmployeeId", "DISTINCT ad.EmployeeId as useraccempid,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, EmpAdd.MainPhone, EmpAdd.MainEmail,CONVERT(NVARCHAR,Ad.HireDate, 106) as HireDate,IsActive", "(select COUNT(*) from AssginBussiness asb where asb.EmployeeID = ad.EmployeeId and asb.UserTypeId=1   and Asb.MotherOrgID=" + Session["MotherOrgid"].ToString() + "   and BusinessID is not null)bussinessCount", " ad.UserType=1 and ad.IsActive='YES' and  ad.UserType = 1 and(Ad.MotherOrgID = " + Session["MotherOrgid"].ToString() + " or Assb.MotherOrgID = " + Session["MotherOrgid"].ToString() + ")", "AD.EmployeeID desc");
        }
        if (Session["EmployeeId"].ToString() == "0" && ddl_status.SelectedValue == "NO")
        {
            comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad with (Nolock) join EmpAddress EmpAdd with (Nolock) on EmpAdd.EmpAddressId=AD.ADDRESSID and ad.UserType=1 join UserAcc ua on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB with (Nolock) ON ASSB.EmployeeID=ad.EmployeeId", "DISTINCT ad.EmployeeId as useraccempid,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, EmpAdd.MainPhone, EmpAdd.MainEmail,CONVERT(NVARCHAR,Ad.HireDate, 106) as HireDate,IsActive", "(select COUNT(*) from AssginBussiness asb where asb.EmployeeID = ad.EmployeeId and asb.UserTypeId=1  and Asb.MotherOrgID=" + Session["MotherOrgid"].ToString() + "  and BusinessID is not null)bussinessCount", " ad.UserType=1 and ad.IsActive='NO'  and(Ad.MotherOrgID = " + Session["MotherOrgid"].ToString() + " or Assb.MotherOrgID = " + Session["MotherOrgid"].ToString() + ")", "AD.EmployeeID desc");
        }
    }

    protected void GvDivAdminBussOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField HiddenField_EmployeeID = (e.Row.FindControl("HiddenField_EmployeeID") as HiddenField);
            DropDownList DropDownList_Business_Name = (e.Row.FindControl("DropDownList_Business_Name") as DropDownList);

            DataTable Get_DropDownList_Business_Name_List = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "BusinessID,BusinessName", "MotherOrgID=" + Session["MotherOrgid"].ToString() + " and EmployeeID = " + HiddenField_EmployeeID.Value + " and UserTypeId=1 and BusinessID IS NOT NULL", "BusinessID");
            if (Get_DropDownList_Business_Name_List.Rows.Count != 0)
            {
                DropDownList_Business_Name.Items.Clear();
                DropDownList_Business_Name.Items.Add("Registered Business List");
                DropDownList_Business_Name.SelectedItem.Value = "";
                DropDownList_Business_Name.DataSource = Get_DropDownList_Business_Name_List;
                DropDownList_Business_Name.DataValueField = Get_DropDownList_Business_Name_List.Columns[0].ToString();
                DropDownList_Business_Name.DataTextField = Get_DropDownList_Business_Name_List.Columns[1].ToString();
                DropDownList_Business_Name.DataBind();
            }
            else
            {
                DropDownList_Business_Name.Items.Clear();
                DropDownList_Business_Name.Items.Add("No Business Registered");
                DropDownList_Business_Name.SelectedItem.Value = "";
                DropDownList_Business_Name.DataBind();
            }

            Label LinkButton_Employee_Count_ = (e.Row.FindControl("LinkButton_Employee_Count") as Label);
            DataTable Employee_Count = comm.GetAllDataFromTable("AssginBussiness with (Nolock) join AddEmployee with (Nolock) on AssginBussiness.EmployeeID=AddEmployee.EmployeeID", "AddEmployee.EmployeeID",
                "AssginBussiness.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and UserTypeId not in (1) and AddEmployee.IsActive='YES' " +
                "and AssginBussiness.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID=asb.BusinessID " +
                "where AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID.Value + " AND AsB.BusinessID IS NOT NULL)", "");
            LinkButton_Employee_Count_.Text = Employee_Count.Rows.Count.ToString();

            HyperLink HyperLink_Edit_Subscriber = (e.Row.FindControl("HyperLink_Edit_Subscriber") as HyperLink);
            var check_if_deactivated_by_support_admin = comm.ExecuteQueryDataTable("select BusinessOwnerId from AddEmployee where EmployeeID=" + HiddenField_EmployeeID.Value + "" +
                    "and (BusinessOwnerId='2' or IsActive='NO        ') ");
            if (check_if_deactivated_by_support_admin.Rows.Count > 0)
            {
                HyperLink_Edit_Subscriber.Enabled = false;
            }

            ImageButton btnactive = (e.Row.FindControl("btnactive") as ImageButton);
            HiddenField hfdStatusForAdminBussOwner = (e.Row.FindControl("hfdStatusForAdminBussOwner") as HiddenField);

            if (hfdStatusForAdminBussOwner.Value.Trim() == "YES")
            {
                btnactive.ImageUrl = "~/images/active.png";
            }
            else
            {
                btnactive.ImageUrl = "~/images/deactive.png";
                e.Row.BackColor = System.Drawing.Color.LightGray;
            }
        }
    }

    protected void GvDivAdminBussOwner_PreRender(object sender, EventArgs e)
    {
        if (GvDivAdminBussOwner.Rows.Count > 0)
        {
            GvDivAdminBussOwner.UseAccessibleHeader = true;
            GvDivAdminBussOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvDivAdminBussOwner_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GvDivAdminBussOwner.PageIndex = e.NewPageIndex;
        GetEmplist();
    }

    protected void GvDivAdminBussOwner_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "status")
        {
            string Toggle_button_confirm_value = Request.Form["Toggle_activate_deactivate"];
            if (Toggle_button_confirm_value == "Yes" || Toggle_button_confirm_value == "null" || Toggle_button_confirm_value == null)
            {

            }
            else
            {
                return;
            }

            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            //int i = 0;
            int id = 0;
            //GridViewRow row = GvDivAdminBussOwner.Rows[index];
            HiddenField hfdStatusForAdminBussOwner = (HiddenField)GvDivAdminBussOwner.Rows[index].FindControl("hfdStatusForAdminBussOwner");
            
            id = int.Parse(commandArguments[1].ToString());
            DataTable dtAllemp = ACCtrl.GetEmpIDFromAccountant(id);
            DataTable dtAllBuss = comm.GetAllDataFromTable("AssginBussiness with (Nolock)", "BusinessID", "EmployeeID=" + id + " and UserTypeId=1", "");
            if (hfdStatusForAdminBussOwner.Value.Trim() == "YES")
            {
                //i = comm.ExecuteQuery("Update AddEmployee set IsActive='NO' where EmployeeID=" + id + "");
                comm.ExecuteQuery("Update AddEmployee set IsActive='NO' where EmployeeID=" + id + "");
                foreach (DataRow r in dtAllemp.Rows)
                {
                    comm.ExecuteQuery("Update AddEmployee set IsActive='NO'  where EmployeeID=" + r["EmployeeID"].ToString() + "");
                }

                foreach (DataRow rBuss in dtAllBuss.Rows)
                {
                    if (rBuss["BusinessID"].ToString() != string.Empty)
                    {
                        comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rBuss["BusinessID"].ToString() + "");
                    }
                }

                comm.ExecuteQuery("update PlanHistory set IsActive=0 " +
                    "where EmpId=" + id + " and MotherOrgID=" + Session["MotherOrgid"].ToString() + "");

                string BoName = comm.GetNameFromId("AddEmployee", id);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + BoName + " Has Been Deactivated', 'success');", true);
            }
            else
            {
                var check_if_deactivated_by_support_admin = comm.ExecuteQueryDataTable("select BusinessOwnerId from AddEmployee where EmployeeID=" + id + "" +
                    "and BusinessOwnerId='2' ");
                if (check_if_deactivated_by_support_admin.Rows.Count > 0)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'This Reseller Subscriber has been DeActivated by Support Admin. So could not activate it. Please Contact Support Admin.', 'warning');", true);
                    return;
                }

                //i = comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID=" + id + "");
                comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID=" + id + "");
                foreach (DataRow r in dtAllemp.Rows)
                {
                    comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID=" + r["EmployeeID"].ToString() + "");
                }
                foreach (DataRow rBuss in dtAllBuss.Rows)
                {
                    if (rBuss["BusinessID"].ToString() != string.Empty)
                    {
                        comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID=" + rBuss["BusinessID"].ToString() + "");
                    }
                }

                comm.ExecuteQuery("update PlanHistory set IsActive=1 " +
                    "where EmpId=" + id + " and MotherOrgID=" + Session["MotherOrgid"].ToString() + "");

                string BoName = comm.GetNameFromId("AddEmployee", id);
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', '" + BoName + " Has Been Activated', 'success');", true);
            }
            GetEmplist();
        }

        if (e.CommandName == "bussList")
        {
            //Control ctrl = e.CommandSource as Control;
            //if (ctrl != null)
            //{
            //    GridViewRow gvRow = ctrl.Parent.NamingContainer as GridViewRow;
            //    int empid = Convert.ToInt32(e.CommandArgument);

            //    HiddenField HiddenField_EmployeeID = (HiddenField)gvRow.FindControl("HiddenField_EmployeeID");
            //    HiddenField HiddenField_EmployeeID_UserList = (HiddenField)gvRow.FindControl("HiddenField_EmployeeID_UserList");

            //    //if (HiddenField_EmployeeID.Value != "")
            //    //{
            //    //    //lblbusName.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID.Value));
            //    //    lblbusName.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));
            //    //}

            //    lblbusName.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));

            //    GridView_PopUp_Business_List.DataSource = null;
            //    GridView_PopUp_Business_List.DataBind();

            //    //comm.FillGridView(GridView_PopUp_Business_List, "AssginBussiness AsB join AddBusiness ab  on ab.BusinessID = asb.BusinessID", "asb.BusinessID", "ab.CompanyName, ab.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "AsB.UserTypeId=1 and AsB.EmployeeID=" + empid + " AND AsB.BusinessID IS NOT NULL", "");
            //    comm.FillGridView(GridView_PopUp_Business_List, "AssginBussiness AsB join AddBusiness ab  on ab.BusinessID = asb.BusinessID", "asb.BusinessID", "ab.CompanyName, ab.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID_UserList.Value + " AND AsB.BusinessID IS NOT NULL", "");
            //    ModalPopupBussList.Show();
            //}


            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            int id = 0;
            id = int.Parse(commandArguments[1].ToString());
            //HiddenField HiddenField_EmployeeID_UserList = (HiddenField)GvDivAdminBussOwner.Rows[index].FindControl("HiddenField_EmployeeID_UserList");

            //lblbusName.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));
            lblbusName.Text = comm.GetNameFromId("AddEmployee", id);

            GridView_PopUp_Business_List.DataSource = null;
            GridView_PopUp_Business_List.DataBind();
            //comm.FillGridView(GridView_PopUp_Business_List, "AssginBussiness AsB join AddBusiness ab  on ab.BusinessID = asb.BusinessID", "asb.BusinessID", "ab.CompanyName, ab.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID_UserList.Value + " AND AsB.BusinessID IS NOT NULL", "");
            comm.FillGridView(GridView_PopUp_Business_List, "AssginBussiness AsB join AddBusiness ab  on ab.BusinessID = asb.BusinessID", "asb.BusinessID", "ab.CompanyName, ab.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "AsB.UserTypeId=1 and AsB.EmployeeID=" + id + " AND AsB.BusinessID IS NOT NULL", "");
            ModalPopupBussList.Show();
        }

        if (e.CommandName == "empList")
        {
            //Control ctrlempList = e.CommandSource as Control;
            //if (ctrlempList != null)
            //{
            //    //below commented code shows list of all employees whether it is active ot in active
            //    //GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
            //    //int empid = Convert.ToInt32(e.CommandArgument);

            //    //HiddenField HiddenField_EmployeeID = (HiddenField)gvRow.FindControl("HiddenField_EmployeeID");
            //    ////lblempna.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID.Value));

            //    //SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
            //    //DataTable dt = supportAdminCtrl.GetEmplist_SuppAdmin("Subscriber", empid, 0);
            //    //GridView_PopUp_Employee_List.DataSource = dt;
            //    //GridView_PopUp_Employee_List.DataBind();
            //    //modelpopemp.Show();


            //    GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
            //    int empid = Convert.ToInt32(e.CommandArgument);

            //    HiddenField HiddenField_EmployeeID_UserList = (HiddenField)gvRow.FindControl("HiddenField_EmployeeID_UserList");
            //    lblempna.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));

            //    DataTable EmployeeCount = comm.GetAllDataFromTable("AssginBussiness with (Nolock) join AddEmployee with (Nolock) on AssginBussiness.EmployeeID=AddEmployee.EmployeeID",
            //        "AssginBussiness.BusinessName as bussname, AssginBussiness.BusinessID, AddEmployee.FirstName+' '+AddEmployee.MiddleName+' '+AddEmployee.LastName as empname, " +
            //        "AddEmployee.EmployeeID, dbo.FN_DESC('UserType',AssginBussiness.UserTypeId)UserType",
            //        "AssginBussiness.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and UserTypeId not in (1) and AddEmployee.IsActive='YES' " +
            //        "and AssginBussiness.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID=asb.BusinessID " +
            //        "where AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID_UserList.Value + " AND AsB.BusinessID IS NOT NULL)", "1");
            //    GridView_PopUp_Employee_List.DataSource = EmployeeCount;
            //    GridView_PopUp_Employee_List.DataBind();
            //    modelpopemp.Show();
            //}


            string[] commandArguments = e.CommandArgument.ToString().Split(',');
            int index = Convert.ToInt32(commandArguments[0].ToString());
            int id = 0;
            id = int.Parse(commandArguments[1].ToString());
            //HiddenField HiddenField_EmployeeID_UserList = (HiddenField)GvDivAdminBussOwner.Rows[index].FindControl("HiddenField_EmployeeID_UserList");

            //lblempna.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));
            lblempna.Text = comm.GetNameFromId("AddEmployee", id);

            GridView_PopUp_Employee_List.DataSource = null;
            GridView_PopUp_Employee_List.DataBind();
            //DataTable EmployeeCount = comm.GetAllDataFromTable("AssginBussiness with (Nolock) join AddEmployee with (Nolock) on AssginBussiness.EmployeeID=AddEmployee.EmployeeID",
            //        "AssginBussiness.BusinessName as bussname, AssginBussiness.BusinessID, AddEmployee.FirstName+' '+AddEmployee.MiddleName+' '+AddEmployee.LastName as empname, " +
            //        "AddEmployee.EmployeeID, dbo.FN_DESC('UserType',AssginBussiness.UserTypeId)UserType",
            //        "AssginBussiness.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and UserTypeId not in (1) and AddEmployee.IsActive='YES' " +
            //        "and AssginBussiness.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID=asb.BusinessID " +
            //        "where AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID_UserList.Value + " AND AsB.BusinessID IS NOT NULL)", "1");
            DataTable EmployeeCount = comm.GetAllDataFromTable("AssginBussiness with (Nolock) join AddEmployee with (Nolock) on AssginBussiness.EmployeeID=AddEmployee.EmployeeID",
                    "AssginBussiness.BusinessName as bussname, AssginBussiness.BusinessID, AddEmployee.FirstName+' '+AddEmployee.MiddleName+' '+AddEmployee.LastName as empname, " +
                    "AddEmployee.EmployeeID, dbo.FN_DESC('UserType',AssginBussiness.UserTypeId)UserType",
                    "AssginBussiness.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and UserTypeId not in (1) and AddEmployee.IsActive='YES' " +
                    "and AssginBussiness.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID=asb.BusinessID " +
                    "where AsB.UserTypeId=1 and AsB.EmployeeID=" + id + " AND AsB.BusinessID IS NOT NULL)", "1");
            GridView_PopUp_Employee_List.DataSource = EmployeeCount;
            GridView_PopUp_Employee_List.DataBind();
            modelpopemp.Show();
        }

        if (e.CommandName == "empList_activeonly")
        {
            Control ctrlempList = e.CommandSource as Control;
            if (ctrlempList != null)
            {
                GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
                int empid = Convert.ToInt32(e.CommandArgument);

                HiddenField HiddenField_EmployeeID_UserList = (HiddenField)gvRow.FindControl("HiddenField_EmployeeID_UserList");
                lblempna.Text = comm.GetNameFromId("AddEmployee", int.Parse(HiddenField_EmployeeID_UserList.Value));

                DataTable EmployeeCount = comm.GetAllDataFromTable("AssginBussiness with (Nolock) join AddEmployee with (Nolock) on AssginBussiness.EmployeeID=AddEmployee.EmployeeID",
                    "AssginBussiness.BusinessName as bussname, AssginBussiness.BusinessID, AddEmployee.FirstName+' '+AddEmployee.MiddleName+' '+AddEmployee.LastName as empname, " +
                    "AddEmployee.EmployeeID, dbo.FN_DESC('UserType',AssginBussiness.UserTypeId)UserType",
                    "AssginBussiness.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and UserTypeId not in (1) and AddEmployee.IsActive='YES' " +
                    "and AssginBussiness.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID=asb.BusinessID " +
                    "where AsB.UserTypeId=1 and AsB.EmployeeID=" + HiddenField_EmployeeID_UserList.Value + " AND AsB.BusinessID IS NOT NULL)", "1");
                GridView_PopUp_Employee_List.DataSource = EmployeeCount;
                GridView_PopUp_Employee_List.DataBind();
                modelpopemp.Show();
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

    protected void GridView_PopUp_Business_List_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName == "Businessstatus")
        //{
        //    Control ctrlempList = e.CommandSource as Control;
        //    if (ctrlempList != null)
        //    {
        //        GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
        //        int BusinessID = Convert.ToInt32(e.CommandArgument);
        //        ImageButton ImageButton_Active_Deactive = (ImageButton)gvRow.FindControl("btnactivebus");
        //        var sour = ImageButton_Active_Deactive.ImageUrl;
        //        if (sour.Contains("~/images/active.png"))
        //        {
        //            Comm.ExecuteQuery("update AddBusiness set Status=2 where BusinessID=" + BusinessID + "");
        //            DataTable dt = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID", "BusinessID=" + BusinessID + " and BusinessID is not null and UserTypeId <>1", "");
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
        //                //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
        //                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
        //            }
        //        }
        //        else
        //        {
        //            Comm.ExecuteQuery("update AddBusiness set Status=1 where BusinessID=" + BusinessID + "");
        //            DataTable dt = Comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID", "BusinessID=" + BusinessID + " and BusinessID is not null ", "");
        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
        //                //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
        //                Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
        //            }
        //        }
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
        //    }
        //}
    }

    protected void GridView_PopUp_Business_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    ImageButton ImageButton_Active_Deactive = (e.Row.FindControl("btnactivebus") as ImageButton);
        //    HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("hfdStatusForBussOwner") as HiddenField);
        //    if (HiddenField_Business_Owner_IsActive_Status.Value.ToString().Trim() == "1")
        //    {
        //        ImageButton_Active_Deactive.ImageUrl = "~/images/active.png";
        //    }
        //    else
        //    {
        //        ImageButton_Active_Deactive.ImageUrl = "~/images/deactive.png";
        //    }
        //}
    }

    protected void GridView_PopUp_Employee_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_PopUp_Employee_List.Rows.Count > 0)
        {
            GridView_PopUp_Employee_List.UseAccessibleHeader = true;
            GridView_PopUp_Employee_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_PopUp_Employee_List_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        //if (e.CommandName == "EMPSTUS")
        //{
        //    Control ctrlempList = e.CommandSource as Control;
        //    if (ctrlempList != null)
        //    {
        //        GridViewRow gvRow = ctrlempList.Parent.NamingContainer as GridViewRow;
        //        int empid = Convert.ToInt32(e.CommandArgument);
        //        ImageButton ImageButton_Active_Deactive = (ImageButton)gvRow.FindControl("btnactiveEMP");
        //        var sour = ImageButton_Active_Deactive.ImageUrl;
        //        if (sour.Contains("~/images/active.png"))
        //        {
        //            Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + empid + "");
        //            //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
        //            Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID=" + empid + "");
        //        }
        //        else
        //        {
        //            Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + empid + "");
        //            //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
        //            Comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID=" + empid + "");
        //        }
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
        //    }
        //}
    }

    protected void GridView_PopUp_Employee_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    ImageButton ImageButton_Active_Deactive = (e.Row.FindControl("btnactiveEMP") as ImageButton);
        //    HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("hdbID") as HiddenField);
        //    if (HiddenField_Business_Owner_IsActive_Status.Value.ToString().Trim() == "YES")
        //    {
        //        ImageButton_Active_Deactive.ImageUrl = "~/images/active.png";
        //    }
        //    else
        //    {
        //        ImageButton_Active_Deactive.ImageUrl = "~/images/deactive.png";
        //    }
        //}
    }
}