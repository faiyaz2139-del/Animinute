using System;
using System.Data;
using System.Web.UI.WebControls;

public partial class GridviewNewDesign_AssignUser : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    CommanClass comm = new CommanClass();
    AssgineManagerEntity Ame = new AssgineManagerEntity();
    AssgineManagerController Amc = new AssgineManagerController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            //GetMangerList();
            //GetCustomRoleList();
            //GetAccountantList();

            DataTable Get_Business_Name_In_Ddl = Comm.GetAllDataFromTable("AssginBussiness assb with (Nolock) join AddBusiness ab with (Nolock) on assb.BusinessID = ab.BusinessID", "ab.BusinessID,(ab.CompanyName)BusinessName", "EmployeeID=" + Session["EmployeeId"].ToString() + " and ab.Status=1", "ab.BusinessID");
            Ddl_Business_Assign_User.Items.Add("Please Select");
            Ddl_Business_Assign_User.SelectedItem.Value = "";
            Ddl_Business_Assign_User.DataSource = Get_Business_Name_In_Ddl;
            Ddl_Business_Assign_User.DataValueField = Get_Business_Name_In_Ddl.Columns[0].ToString();
            Ddl_Business_Assign_User.DataTextField = Get_Business_Name_In_Ddl.Columns[1].ToString();
            Ddl_Business_Assign_User.DataBind();

            //Ddl_Business_Assign_User.SelectedValue = Request.QueryString["BusinessIdFromDefaultPapge"].ToString();
        }
    }

    protected void Ddl_Business_Assign_User_SelectedIndexChanged(object sender, EventArgs e)
    {
       DropDownList_Select_Role.SelectedIndex = 0;

        Div_Assign_Manager.Visible = false;
        Div_Assign_CustomRole.Visible = false;
        Div_Assign_Accountant.Visible = false;
        Div_Assign_Manager_ddl.Visible = false;
        Div_Assign_CustomRole_ddl.Visible = false;
        Div_Assign_Accountant_ddl.Visible = false;

        ddlmanager.DataSource = null;
        ddlmanager.DataBind();
        GridView_UnAssigned_Employee_List.DataSource = null;
        GridView_UnAssigned_Employee_List.DataBind();
        GridView_Assigned_Employee_List.DataSource = null;
        GridView_Assigned_Employee_List.DataBind();

        ddlcustomrole.DataSource = null;
        ddlcustomrole.DataBind();
        GridView_UnAssigned_Employee_List_To_Customrole.DataSource = null;
        GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
        GridView_Assigned_Employee_List_To_Customrole.DataSource = null;
        GridView_Assigned_Employee_List_To_Customrole.DataBind();

        ddlaccountant.DataSource = null;
        ddlaccountant.DataBind();
        GridView_UnAssigned_Business_List_To_Accountant.DataSource = null;
        GridView_UnAssigned_Business_List_To_Accountant.DataBind();
        GridView_Assigned_Business_List_To_Accountant.DataSource = null;
        GridView_Assigned_Business_List_To_Accountant.DataBind();
        GridView_Assigned_Business_List_To_Accountant_2.DataSource = null;
        GridView_Assigned_Business_List_To_Accountant_2.DataBind();
    }

    protected void DropDownList_Select_Role_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DropDownList_Select_Role.SelectedIndex == 0)
        {
            Div_Assign_Manager.Visible = false;
            Div_Assign_CustomRole.Visible = false;
            Div_Assign_Accountant.Visible = false;
            Div_Assign_Manager_ddl.Visible = false;
            Div_Assign_CustomRole_ddl.Visible = false;
            Div_Assign_Accountant_ddl.Visible = false;

            ddlmanager.DataSource = null;
            ddlmanager.DataBind();
            GridView_UnAssigned_Employee_List.DataSource = null;
            GridView_UnAssigned_Employee_List.DataBind();
            GridView_Assigned_Employee_List.DataSource = null;
            GridView_Assigned_Employee_List.DataBind();

            ddlcustomrole.DataSource = null;
            ddlcustomrole.DataBind();
            GridView_UnAssigned_Employee_List_To_Customrole.DataSource = null;
            GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
            GridView_Assigned_Employee_List_To_Customrole.DataSource = null;
            GridView_Assigned_Employee_List_To_Customrole.DataBind();

            ddlaccountant.DataSource = null;
            ddlaccountant.DataBind();
            GridView_UnAssigned_Business_List_To_Accountant.DataSource = null;
            GridView_UnAssigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant_2.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant_2.DataBind();

            table_button_Manager.Visible = false;
            table_button_Customrole.Visible = false;
            table_button_Accountant.Visible = false;
        }

        if (DropDownList_Select_Role.SelectedIndex == 1)
        {
            Div_Assign_Manager.Visible = true;
            Div_Assign_CustomRole.Visible = false;
            Div_Assign_Accountant.Visible = false;
            Div_Assign_Manager_ddl.Visible = true;
            Div_Assign_CustomRole_ddl.Visible = false;
            Div_Assign_Accountant_ddl.Visible = false;

            GetMangerList();

            ddlcustomrole.DataSource = null;
            ddlcustomrole.DataBind();
            GridView_UnAssigned_Employee_List_To_Customrole.DataSource = null;
            GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
            GridView_Assigned_Employee_List_To_Customrole.DataSource = null;
            GridView_Assigned_Employee_List_To_Customrole.DataBind();

            ddlaccountant.DataSource = null;
            ddlaccountant.DataBind();
            GridView_UnAssigned_Business_List_To_Accountant.DataSource = null;
            GridView_UnAssigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant_2.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant_2.DataBind();

            table_button_Manager.Visible = false;
            table_button_Customrole.Visible = false;
            table_button_Accountant.Visible = false;
        }

        if (DropDownList_Select_Role.SelectedIndex == 2)
        {
            Div_Assign_Manager.Visible = false;
            Div_Assign_CustomRole.Visible = true;
            Div_Assign_Accountant.Visible = false;
            Div_Assign_Manager_ddl.Visible = false;
            Div_Assign_CustomRole_ddl.Visible = true;
            Div_Assign_Accountant_ddl.Visible = false;

            GetCustomRoleList();

            ddlmanager.DataSource = null;
            ddlmanager.DataBind();
            GridView_UnAssigned_Employee_List.DataSource = null;
            GridView_UnAssigned_Employee_List.DataBind();
            GridView_Assigned_Employee_List.DataSource = null;
            GridView_Assigned_Employee_List.DataBind();

            ddlaccountant.DataSource = null;
            ddlaccountant.DataBind();
            GridView_UnAssigned_Business_List_To_Accountant.DataSource = null;
            GridView_UnAssigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant.DataBind();
            GridView_Assigned_Business_List_To_Accountant_2.DataSource = null;
            GridView_Assigned_Business_List_To_Accountant_2.DataBind();

            table_button_Manager.Visible = false;
            table_button_Customrole.Visible = false;
            table_button_Accountant.Visible = false;
        }

        if (DropDownList_Select_Role.SelectedIndex == 3)
        {
            Div_Assign_Manager.Visible = false;
            Div_Assign_CustomRole.Visible = false;
            Div_Assign_Accountant.Visible = true;
            Div_Assign_Manager_ddl.Visible = false;
            Div_Assign_CustomRole_ddl.Visible = false;
            Div_Assign_Accountant_ddl.Visible = true;

            GetAccountantList();

            ddlmanager.DataSource = null;
            ddlmanager.DataBind();
            GridView_UnAssigned_Employee_List.DataSource = null;
            GridView_UnAssigned_Employee_List.DataBind();
            GridView_Assigned_Employee_List.DataSource = null;
            GridView_Assigned_Employee_List.DataBind();

            ddlcustomrole.DataSource = null;
            ddlcustomrole.DataBind();
            GridView_UnAssigned_Employee_List_To_Customrole.DataSource = null;
            GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
            GridView_Assigned_Employee_List_To_Customrole.DataSource = null;
            GridView_Assigned_Employee_List_To_Customrole.DataBind();

            table_button_Manager.Visible = false;
            table_button_Customrole.Visible = false;
            table_button_Accountant.Visible = false;
        }
    }




    #region manager code starts here
    public void GetMangerList()
    {
        if (DropDownList_Select_Role.SelectedIndex != 0)
        {
            comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ab.IsActive='YES       '", "ab.EmployeeID");
        }
    }

    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e)
    {
        table_button_Manager.Visible = true;
        Button_LeftMove_Manager.Visible = true;
        Button_LeftMoveAll_Manager.Visible = true;
        Button_RightMove_Manager.Visible = true;
        Button_RightMoveAll_Manager.Visible = true;
        btnSave_Manager.Visible = true;
        int managerid = 0;
        if (ddlmanager.SelectedValue != "")
        {
            managerid = int.Parse(ddlmanager.SelectedValue);
        }

        if (ddlmanager.SelectedValue == "")
        {
            return;
        }

        DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");
        if (dt1.Rows.Count > 0)
        {
            DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (1,2,5,12) and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ab.IsActive='YES       ' ", "ab.EmployeeID");
            GridView_UnAssigned_Employee_List.DataSource = dt;
            GridView_UnAssigned_Employee_List.DataBind();

            DataTable dtemp = dt.Clone();
            DataTable dtlist = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool check = comm.IfExistsInTable("AssgineManager", "*", "BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and EmpId=" + dt.Rows[i]["EmployeeID"].ToString() + " and ManagerID=" + managerid + "");
                if (check == true)
                {
                    dtemp.Rows.Add(dt.Rows[i].ItemArray);
                }
                else
                {
                    dtlist.Rows.Add(dt.Rows[i].ItemArray);
                }
            }
            ViewState["dtemp"] = dtemp;
            ViewState["dt"] = dtlist;
            GridView_UnAssigned_Employee_List.DataSource = dtlist;

            GridView_UnAssigned_Employee_List.DataBind();
            GridView_Assigned_Employee_List.DataSource = dtemp;

            GridView_Assigned_Employee_List.DataBind();
        }
    }

    protected void Button_LeftMove_Manager_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt.Clone();
        DataTable dtemp = new DataTable();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_UnAssigned_Employee_List.Rows.Count; i++)
        {
            CheckBox chk = GridView_UnAssigned_Employee_List.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtemp.Rows.Add(dt.Rows[i].ItemArray);
            }
            else
            {
                dtlist.Rows.Add(dt.Rows[i].ItemArray);

            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemp;
        GridView_Assigned_Employee_List.DataSource = dtemp;
        GridView_Assigned_Employee_List.DataBind();
        GridView_UnAssigned_Employee_List.DataSource = dtlist;
        GridView_UnAssigned_Employee_List.DataBind();
    }

    protected void btnSave_Manager_Click(object sender, EventArgs e)
    {
        string keyWords = hfSelected.Value;
        DataTable dtemp = ViewState["dtemp"] as DataTable;
        string[] arrayval = keyWords.Trim().Split(',');

        if (ddlmanager.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Manager First', 'warning');", true);
            return;
        }

        string busname = comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business_Assign_User.SelectedValue));
        comm.ExecuteQuery("delete from AssgineManager where BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ManagerID=" + ddlmanager.SelectedValue + "");

        for (int i = 0; i < dtemp.Rows.Count; i++)
        {
            Ame.EmpId = int.Parse(dtemp.Rows[i]["EmployeeID"].ToString());
            Ame.ManagerID = int.Parse(ddlmanager.SelectedValue);
            Ame.MangerName = ddlmanager.SelectedItem.Text;
            Ame.BusinessID = int.Parse(Ddl_Business_Assign_User.SelectedValue);
            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(dtemp.Rows[i]["EmployeeID"].ToString()));
            Ame.BusinessName = busname;
            Amc.InsertAssgineManger(Ame);
        }

        if (dtemp.Rows.Count == 0)
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Manager UnAssigned Successfully', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All employees deassigned successfully', 'success');", true);
        else
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Manager Assigned Successfully', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Selected employee assigned successfully', 'success');", true);
    }

    protected void Button_RightMove_Manager_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Employee_List.Rows.Count; i++)
        {
            CheckBox chk = GridView_Assigned_Employee_List.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
            }
            else
            {
                dtemplst.Rows.Add(dtemp.Rows[i].ItemArray);

            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;
        GridView_Assigned_Employee_List.DataSource = dtemplst;
        GridView_Assigned_Employee_List.DataBind();
        GridView_UnAssigned_Employee_List.DataSource = dtlist;
        GridView_UnAssigned_Employee_List.DataBind();
    }

    protected void Button_LeftMoveAll_Manager_Click(object sender, EventArgs e)
    {
        try
        {
            for (int i = 0; i < GridView_UnAssigned_Employee_List.Rows.Count; i++)
            {
                CheckBox chk = GridView_UnAssigned_Employee_List.Rows[i].FindControl("chkemp") as CheckBox;
                chk.Checked = true;
            }
        }
        catch (Exception)
        {


        }
        Button_LeftMove_Manager_Click(sender, e);
    }

    protected void Button_RightMoveAll_Manager_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Employee_List.Rows.Count; i++)
        {

            dtlist.Rows.Add(dtemp.Rows[i].ItemArray);

        }

        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;
        GridView_Assigned_Employee_List.DataSource = dtemplst;
        GridView_Assigned_Employee_List.DataBind();
        GridView_UnAssigned_Employee_List.DataSource = dtlist;
        GridView_UnAssigned_Employee_List.DataBind();
    }

    protected void GridView_UnAssigned_Employee_List_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_UnAssigned_Employee_List.PageIndex = e.NewPageIndex;
    }

    protected void GridView_UnAssigned_Employee_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_UnAssigned_Employee_List.Rows.Count > 0)
        {
            GridView_UnAssigned_Employee_List.UseAccessibleHeader = true;
            GridView_UnAssigned_Employee_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_Assigned_Employee_List_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_Assigned_Employee_List.PageIndex = e.NewPageIndex;
    }

    protected void GridView_Assigned_Employee_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_Assigned_Employee_List.Rows.Count > 0)
        {
            GridView_Assigned_Employee_List.UseAccessibleHeader = true;
            GridView_Assigned_Employee_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    #endregion manager code end here











    #region custom role code starts here
    public void GetCustomRoleList()
    {
        if (DropDownList_Select_Role.SelectedIndex != 0)
        {
            comm.FillDropDownList(ddlcustomrole, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ab.IsActive='YES       ' ", "ab.EmployeeID");
        }
    }

    protected void ddlcustomrole_SelectedIndexChanged(object sender, EventArgs e)
    {
        table_button_Customrole.Visible = true;
        Button_LeftMove_Customrole.Visible = true;
        Button_LeftMoveAll_Customrole.Visible = true;
        Button_RightMove_Customrole.Visible = true;
        Button_RightMoveAll_Customrole.Visible = true;
        btnSave_Customrole.Visible = true;
        int managerid = 0;
        if (ddlcustomrole.SelectedValue != "")
        {
            managerid = int.Parse(ddlcustomrole.SelectedValue);
        }

        if (ddlcustomrole.SelectedValue == "")
        {
            return;
        }

        DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + "  and ab.EmployeeID='" + this.ddlcustomrole.SelectedValue.ToString() + "'", "ab.EmployeeID");


        if (dt1.Rows.Count > 0)
        {
            DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (1,2,5,12) and asb.BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ab.IsActive='YES       ' and ab.EmployeeID!='" + this.ddlcustomrole.SelectedValue.ToString() + "'", "ab.EmployeeID");
            GridView_UnAssigned_Employee_List_To_Customrole.DataSource = dt;
            GridView_UnAssigned_Employee_List_To_Customrole.DataBind();

            DataTable dtemp = dt.Clone();
            DataTable dtlist = dt.Clone();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool check = comm.IfExistsInTable("AssgineManager", "*", "BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and EmpId=" + dt.Rows[i]["EmployeeID"].ToString() + " and ManagerID=" + managerid + "");
                if (check == true)
                {
                    dtemp.Rows.Add(dt.Rows[i].ItemArray);
                }
                else
                {
                    dtlist.Rows.Add(dt.Rows[i].ItemArray);
                }
            }
            ViewState["dtemp"] = dtemp;
            ViewState["dt"] = dtlist;
            GridView_UnAssigned_Employee_List_To_Customrole.DataSource = dtlist;

            GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
            GridView_Assigned_Employee_List_To_Customrole.DataSource = dtemp;

            GridView_Assigned_Employee_List_To_Customrole.DataBind();
        }
    }

    protected void Button_LeftMove_Customrole_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt.Clone();
        DataTable dtemp = new DataTable();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_UnAssigned_Employee_List_To_Customrole.Rows.Count; i++)
        {
            CheckBox chk = GridView_UnAssigned_Employee_List_To_Customrole.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtemp.Rows.Add(dt.Rows[i].ItemArray);
            }
            else
            {
                dtlist.Rows.Add(dt.Rows[i].ItemArray);

            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemp;
        GridView_Assigned_Employee_List_To_Customrole.DataSource = dtemp;
        GridView_Assigned_Employee_List_To_Customrole.DataBind();
        GridView_UnAssigned_Employee_List_To_Customrole.DataSource = dtlist;
        GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
    }

    protected void btnSave_Customrole_Click(object sender, EventArgs e)
    {
        string keyWords = hfSelected_Customrole.Value;
        DataTable dtemp = ViewState["dtemp"] as DataTable;
        string[] arrayval = keyWords.Trim().Split(',');

        if (ddlcustomrole.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Custom Role First', 'warning');", true);
            return;
        }

        string busname = comm.GetNameFromId("AddBusiness", int.Parse(Ddl_Business_Assign_User.SelectedValue));
        comm.ExecuteQuery("delete from AssgineManager where BusinessID=" + int.Parse(Ddl_Business_Assign_User.SelectedValue) + " and ManagerID=" + ddlcustomrole.SelectedValue + "");

        for (int i = 0; i < dtemp.Rows.Count; i++)
        {


            Ame.EmpId = int.Parse(dtemp.Rows[i]["EmployeeID"].ToString());
            Ame.ManagerID = int.Parse(ddlcustomrole.SelectedValue);
            Ame.MangerName = ddlcustomrole.SelectedItem.Text;
            Ame.BusinessID = int.Parse(Ddl_Business_Assign_User.SelectedValue);
            Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(dtemp.Rows[i]["EmployeeID"].ToString()));
            Ame.BusinessName = busname;
            Amc.InsertAssgineManger(Ame);

        }
        if (dtemp.Rows.Count == 0)
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Custom Role UnAssigned Successfully', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'All employees deassigned successfully', 'success');", true);
        else
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Custom Role Assigned Successfully', 'success');", true);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Selected employee assigned successfully', 'success');", true);
    }

    protected void Button_RightMove_Customrole_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Employee_List_To_Customrole.Rows.Count; i++)
        {
            CheckBox chk = GridView_Assigned_Employee_List_To_Customrole.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
            }
            else
            {
                dtemplst.Rows.Add(dtemp.Rows[i].ItemArray);

            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;
        GridView_Assigned_Employee_List_To_Customrole.DataSource = dtemplst;
        GridView_Assigned_Employee_List_To_Customrole.DataBind();
        GridView_UnAssigned_Employee_List_To_Customrole.DataSource = dtlist;
        GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
    }

    protected void Button_LeftMoveAll_Customrole_Click(object sender, EventArgs e)
    {
        try
        {
            for (int i = 0; i < GridView_UnAssigned_Employee_List_To_Customrole.Rows.Count; i++)
            {
                CheckBox chk = GridView_UnAssigned_Employee_List_To_Customrole.Rows[i].FindControl("chkemp") as CheckBox;
                chk.Checked = true;
            }
        }
        catch (Exception)
        {


        }
        Button_LeftMove_Customrole_Click(sender, e);
    }

    protected void Button_RightMoveAll_Customrole_Click(object sender, EventArgs e)
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {


        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Employee_List_To_Customrole.Rows.Count; i++)
        {

            dtlist.Rows.Add(dtemp.Rows[i].ItemArray);

        }

        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;
        GridView_Assigned_Employee_List_To_Customrole.DataSource = dtemplst;
        GridView_Assigned_Employee_List_To_Customrole.DataBind();
        GridView_UnAssigned_Employee_List_To_Customrole.DataSource = dtlist;
        GridView_UnAssigned_Employee_List_To_Customrole.DataBind();
    }

    protected void GridView_UnAssigned_Employee_List_To_Customrole_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_UnAssigned_Employee_List_To_Customrole.PageIndex = e.NewPageIndex;
    }

    protected void GridView_UnAssigned_Employee_List_To_Customrole_PreRender(object sender, EventArgs e)
    {
        if (GridView_UnAssigned_Employee_List_To_Customrole.Rows.Count > 0)
        {
            GridView_UnAssigned_Employee_List_To_Customrole.UseAccessibleHeader = true;
            GridView_UnAssigned_Employee_List_To_Customrole.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_Assigned_Employee_List_To_Customrole_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_Assigned_Employee_List_To_Customrole.PageIndex = e.NewPageIndex;
    }

    protected void GridView_Assigned_Employee_List_To_Customrole_PreRender(object sender, EventArgs e)
    {
        if (GridView_Assigned_Employee_List_To_Customrole.Rows.Count > 0)
        {
            GridView_Assigned_Employee_List_To_Customrole.UseAccessibleHeader = true;
            GridView_Assigned_Employee_List_To_Customrole.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    #endregion custom role code ends here












    #region assign accountant code starts here
    public void GetAccountantList()
    {
        if (DropDownList_Select_Role.SelectedIndex != 0)
        {
            comm.FillDropDownList(ddlaccountant, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID = asb.EmployeeID", "max(EmployeeID) as EmployeeID", "temp.empname from (Select ab.EmployeeID, dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid IN (12) and asb.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID where AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' AND AsB.BusinessID IS NOT NULL) and ab.IsActive = 'YES')temp group by temp.empname", "EmployeeID");

            ddlaccountant.Items.Add(new ListItem() { Value = "0", Text = "Add New Accountant" });
        }
    }

    protected void ddlaccountant_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlaccountant.SelectedValue == "0")
        {
            Response.Redirect("Add_EmployeeV2_Accountant.aspx");
        }

        table_button_Accountant.Visible = true;
        Button_LeftMove_Accountant.Visible = true;
        Button_LeftMoveAll_Accountant.Visible = true;
        Button_RightMove_Accountant.Visible = true;
        Button_RightMoveAll_Accountant.Visible = true;
        btnSave_Accountant.Visible = true;
        int managerid = 0;
        if (ddlaccountant.SelectedValue != "")
        {
            managerid = int.Parse(ddlaccountant.SelectedValue);
        }

        if (ddlaccountant.SelectedValue == "")
        {
            return;
        }

        DataTable dt1 = comm.GetAllDataFromTable("AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID", "asb.BusinessID,ab.CompanyName,ab.Status", "AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' and ab.Status=1 AND AsB.BusinessID IS NOT NULL", "asb.BusinessID");

        GridView_UnAssigned_Business_List_To_Accountant.DataSource = dt1;
        GridView_UnAssigned_Business_List_To_Accountant.DataBind();

        if (dt1.Rows.Count > 0)
        {
            DataTable dt = comm.GetAllDataFromTable("AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID", "asb.BusinessID,ab.CompanyName,ab.Status", "AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' and ab.Status=1 AND AsB.BusinessID IS NOT NULL", "asb.BusinessID");
            GridView_UnAssigned_Business_List_To_Accountant.DataSource = dt;
            GridView_UnAssigned_Business_List_To_Accountant.DataBind();

            DataTable dtlist = dt.Clone();
            DataTable dtemp = dt.Clone();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlaccountant.SelectedValue + "", "");

                var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID,EmployeeId", "USERID=" + Check_USERID.Rows[0]["USERID"].ToString() + " " +
                    "and BusinessID=" + dt.Rows[i]["BusinessID"].ToString() + " and UserTypeId=12 ", "");

                if (Get_Business_Id.Rows.Count > 0)
                {
                    var Check_Active_Inactive = comm.GetAllDataFromTable("AddEmployee", "IsActive", "EmployeeID=" + Get_Business_Id.Rows[0]["EmployeeId"].ToString() + " " +
                    "and UserType=12 and IsActive='YES       ' ", "");

                    if (Check_Active_Inactive.Rows.Count > 0)
                        dtemp.Rows.Add(dt.Rows[i].ItemArray);
                    else
                        dtlist.Rows.Add(dt.Rows[i].ItemArray);
                }
                else
                {
                    dtlist.Rows.Add(dt.Rows[i].ItemArray);
                }
            }

            ViewState["dt"] = dtlist;
            ViewState["dtemp"] = dtemp;

            GridView_UnAssigned_Business_List_To_Accountant.DataSource = dtlist;
            GridView_UnAssigned_Business_List_To_Accountant.DataBind();

            GridView_Assigned_Business_List_To_Accountant.DataSource = dtemp;
            GridView_Assigned_Business_List_To_Accountant.DataBind();
        }
    }

    protected void Button_LeftMove_Accountant_Click(object sender, EventArgs e) //assign one by one code
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt.Clone();
        DataTable dtemp = new DataTable();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {

        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_UnAssigned_Business_List_To_Accountant.Rows.Count; i++)
        {
            CheckBox chk = GridView_UnAssigned_Business_List_To_Accountant.Rows[i].FindControl("chkemp") as CheckBox;
            Label Label_EmployeeID_emp = GridView_UnAssigned_Business_List_To_Accountant.Rows[i].FindControl("Label_EmployeeID_emp") as Label;
            if (chk.Checked)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlaccountant.SelectedValue + "", "");
                var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID", "USERID=" + Check_USERID.Rows[0]["USERID"].ToString() + " and " +
                    "BusinessID=" + Label_EmployeeID_emp.Text.ToString() + "", "");
                if (Get_Business_Id.Rows.Count == 0)
                    dtemp.Rows.Add(dt.Rows[i].ItemArray);
                if (Get_Business_Id.Rows.Count > 0)
                    dtlist.Rows.Add(dt.Rows[i].ItemArray);
            }
            else
            {
                dtlist.Rows.Add(dt.Rows[i].ItemArray);
            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemp;

        GridView_Assigned_Business_List_To_Accountant.DataSource = dtemp;
        GridView_Assigned_Business_List_To_Accountant.DataBind();

        GridView_UnAssigned_Business_List_To_Accountant.DataSource = dtlist;
        GridView_UnAssigned_Business_List_To_Accountant.DataBind();
    }

    protected void Button_LeftMoveAll_Accountant_Click(object sender, EventArgs e) //assign all code
    {
        try
        {
            for (int i = 0; i < GridView_UnAssigned_Business_List_To_Accountant.Rows.Count; i++)
            {
                CheckBox chk = GridView_UnAssigned_Business_List_To_Accountant.Rows[i].FindControl("chkemp") as CheckBox;
                chk.Checked = true;
            }
        }
        catch (Exception)
        {

        }
        Button_LeftMove_Accountant_Click(sender, e);
    }

    protected void btnSave_Accountant_Click(object sender, EventArgs e)
    {
        if (ddlaccountant.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Accountant First', 'warning');", true);
            return;
        }

        DataTable dtemp = ViewState["dtemp"] as DataTable;
        for (int i = 0; i < dtemp.Rows.Count; i++)
        {
            int BusId = int.Parse(dtemp.Rows[i]["BusinessID"].ToString());
            string BusName = dtemp.Rows[i]["CompanyName"].ToString();

            var Get_User_Id = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeID=" + ddlaccountant.SelectedValue + "", "");

            var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID", "USERID=" + Get_User_Id.Rows[0]["USERID"].ToString() + " and BusinessID=" + BusId + "", "");

            if (Get_Business_Id.Rows.Count == 0)
            {
                int GetEmpId = Comm.ExecuteQuery("INSERT INTO AddEmployee (UserType, Title, FirstName, MiddleName, LastName, DateOfBirth, Gender, SocialInsuranceNumber, MobilePhone, HomePhone, EMail_primary, " +
                        " EMail_secondary, StreetNumber, StreetName, Apartment, City, Province, PostalCode, HireDate, BillRate, BillType, JobTitle, TerminationDate, IsEmployeeEIExempt, IsEmployeeCPPExempt, RetainVacation, " +
                        " MotherOrgID, BusinessID, MangerId, BusinessOwnerId, IsActive, LegalName, Occupation, ADDRESSID, Rateid, SINExpiry, IsEdit, EMPLOYEETYPE) " +
                        " Output Inserted.EmployeeID" +
                        " SELECT UserType, Title, FirstName, MiddleName, LastName, DateOfBirth, Gender, SocialInsuranceNumber, MobilePhone, HomePhone, EMail_primary, " +
                        " EMail_secondary, StreetNumber, StreetName, Apartment, City, Province, PostalCode, HireDate, BillRate, BillType, JobTitle, TerminationDate, IsEmployeeEIExempt, IsEmployeeCPPExempt, RetainVacation, " +
                        " MotherOrgID, BusinessID, MangerId, BusinessOwnerId, IsActive, LegalName, Occupation, ADDRESSID, Rateid, SINExpiry, IsEdit, EMPLOYEETYPE FROM AddEmployee where EmployeeID = " + ddlaccountant.SelectedValue + " ");

                var Get_EmpId = comm.GetAllDataFromTable("AddEmployee", " top 1 EmployeeID", "", "1 desc");

                int getRateId = Comm.ExecuteQuery("INSERT INTO [RateDetails] ([EmployeeId], [Rate], [RateType], [StartDate], [EndDate], [JobTitle], " +
                    " [IsEmployeeEIExempty], [IsEmployeeCPPExempty], [RetainVacation], [effectivedate]) " +
                    " Output Inserted.RateId" +
                    " SELECT " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", [Rate], [RateType], [StartDate], [EndDate], [JobTitle], [IsEmployeeEIExempty], [IsEmployeeCPPExempty], [RetainVacation], [effectivedate] " +
                    " FROM [RateDetails] where EmployeeId = " + ddlaccountant.SelectedValue + " ");

                var get_RateId = comm.GetAllDataFromTable("RateDetails", " top 1 RateId", "", "1 desc");

                Comm.ExecuteQuery("update AddEmployee set Rateid=" + get_RateId.Rows[0]["RateId"].ToString() + " where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                int GetAddressId = Comm.ExecuteQuery("INSERT INTO [EmpAddress] ([Postalcode], [NumberSuffix], [StreetName], [StreetType], [StreetDirection], [SuitType], [SuiteNumber], [SuitSuffix], " +
                    " [City], [Province], [MainPhone], [WorkPhone], [Mobile], [Fax], [MainEmail], [CCEmail], [Website], [Other1], [StreetNo]) " +
                    " Output Inserted.EmpAddressId" +
                    " SELECT [Postalcode], [NumberSuffix], [StreetName], [StreetType], [StreetDirection], [SuitType], [SuiteNumber], [SuitSuffix], " +
                    " [City], [Province], [MainPhone], [WorkPhone], [Mobile], [Fax], [MainEmail], [CCEmail], [Website], [Other1], [StreetNo] " +
                    " FROM [EmpAddress] where EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeId=" + ddlaccountant.SelectedValue + ") ");

                var Get_AddressId = comm.GetAllDataFromTable("EmpAddress", " top 1 EmpAddressId", "", "1 desc");

                Comm.ExecuteQuery("update AddEmployee set ADDRESSID=" + Get_AddressId.Rows[0]["EmpAddressId"].ToString() + " where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                Comm.ExecuteQuery("INSERT INTO [AssginBussiness] ([BusinessID], [EmployeeID], [BusinessName], [EmployeeName], [UserTypeId]) " +
                    " Values (" + BusId + ", " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", '" + BusName + "', '" + ddlaccountant.SelectedItem.Text + "', 12) ");

                Comm.ExecuteQuery("update AssginBussiness set MotherOrgID=(select MotherOrgID from AddEmployee where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ") " +
                    "where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                var RolePermission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id=" + ddlaccountant.SelectedValue + "", "");

                Comm.ExecuteQuery("INSERT INTO User_Role_Permission ([Role_Permission_Id], [Employee_Id], [UserTypeId], [Status], [BusinessId] " + " ) " +
                    "values ('" + RolePermission.Rows[0]["Role_Permission_Id"].ToString() + "', " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", 12, 1, '" + BusId + "')");

                var GetUserID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlaccountant.SelectedValue + "", "");

                comm.ExecuteQuery("insert into MappingTable(USERID, UserTypeId, EmployeeID, BusinessID) " +
                    " values('" + GetUserID.Rows[0]["USERID"].ToString() + "', 12, " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + "," + BusId + ")");
            }
        }

        for (int i = 0; i < GridView_Assigned_Business_List_To_Accountant_2.Rows.Count; i++)
        {
            Label BusId_unassign = GridView_Assigned_Business_List_To_Accountant_2.Rows[i].FindControl("Label_EmployeeID_emp") as Label;

            var Get_User_Id_unassign = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlaccountant.SelectedValue + "", "");

            if (Get_User_Id_unassign.Rows.Count > 0)
            {
                var Get_Business_Id_unassign = comm.GetAllDataFromTable("MappingTable", "BusinessID,EmployeeID", "USERID=" + Get_User_Id_unassign.Rows[0]["USERID"].ToString() + " " +
                "and BusinessID=" + BusId_unassign.Text.ToString() + " and UserTypeId=12", "");

                if (Get_Business_Id_unassign.Rows.Count > 0)
                {
                    //comm.ExecuteQuery("delete from [RateDetails] where RateId in (select Rateid from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + ") ");

                    //comm.ExecuteQuery("delete from [EmpAddress] where EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + ") ");

                    //comm.ExecuteQuery("delete from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " ");

                    //comm.ExecuteQuery("delete from AssginBussiness where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessID=" + BusId_unassign.Text.ToString() + " ");
                    //comm.ExecuteQuery("delete from User_Role_Permission where Employee_Id=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessId=" + BusId_unassign.Text.ToString() + " ");
                    //comm.ExecuteQuery("delete from MappingTable where EmployeeId=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessID=" + BusId_unassign.Text.ToString() + " ");


                    comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " ");

                    //if (Get_User_Id_unassign.Rows.Count != GridView_Assigned_Business_List_To_Accountant.Rows.Count)
                    //{
                    //    comm.ExecuteQuery("delete from [RateDetails] where RateId in (select Rateid from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + ") ");

                    //    comm.ExecuteQuery("delete from [EmpAddress] where EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + ") ");

                    //    comm.ExecuteQuery("delete from AddEmployee where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " ");

                    //    comm.ExecuteQuery("delete from AssginBussiness where EmployeeID=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessID=" + BusId_unassign.Text.ToString() + " ");
                    //    comm.ExecuteQuery("delete from User_Role_Permission where Employee_Id=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessId=" + BusId_unassign.Text.ToString() + " ");
                    //    comm.ExecuteQuery("delete from MappingTable where EmployeeId=" + Get_Business_Id_unassign.Rows[0]["EmployeeID"].ToString() + " and BusinessID=" + BusId_unassign.Text.ToString() + " ");
                    //}
                }
            }
        }


        if (dtemp.Rows.Count == 0)
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Accountant UnAssigned Successfully', 'success');", true);
        else
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Accountant Assigned Successfully', 'success');", true);
    }

    protected void Button_RightMove_Accountant_Click(object sender, EventArgs e) //unassign one by one code
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();

        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {

        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Business_List_To_Accountant.Rows.Count; i++)
        {
            CheckBox chk = GridView_Assigned_Business_List_To_Accountant.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
            }
            else
            {
                dtemplst.Rows.Add(dtemp.Rows[i].ItemArray);
            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;

        GridView_Assigned_Business_List_To_Accountant.DataSource = dtemplst;
        GridView_Assigned_Business_List_To_Accountant.DataBind();

        GridView_UnAssigned_Business_List_To_Accountant.DataSource = dtlist;
        GridView_UnAssigned_Business_List_To_Accountant.DataBind();

        GridView_Assigned_Business_List_To_Accountant_2.DataSource = dtlist;
        GridView_Assigned_Business_List_To_Accountant_2.DataBind();
    }

    protected void Button_RightMoveAll_Accountant_Click(object sender, EventArgs e) //unassign all code
    {
        DataTable dt = ViewState["dt"] as DataTable;
        DataTable dtlist = dt;
        DataTable dtemp = new DataTable();
        DataTable dtemplst = dt.Clone();
        try
        {
            dtemp = ViewState["dtemp"] as DataTable;
        }
        catch (Exception)
        {

        }
        if (dtemp == null)
        {
            dtemp = dt.Clone();
        }
        for (int i = 0; i < GridView_Assigned_Business_List_To_Accountant.Rows.Count; i++)
        {
            dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
        }

        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;

        GridView_Assigned_Business_List_To_Accountant.DataSource = dtemplst;
        GridView_Assigned_Business_List_To_Accountant.DataBind();

        GridView_UnAssigned_Business_List_To_Accountant.DataSource = dtlist;
        GridView_UnAssigned_Business_List_To_Accountant.DataBind();

        GridView_Assigned_Business_List_To_Accountant_2.DataSource = dtlist;
        GridView_Assigned_Business_List_To_Accountant_2.DataBind();
    }

    protected void GridView_UnAssigned_Business_List_To_Accountant_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_UnAssigned_Business_List_To_Accountant.PageIndex = e.NewPageIndex;
    }

    protected void GridView_UnAssigned_Business_List_To_Accountant_PreRender(object sender, EventArgs e)
    {
        if (GridView_UnAssigned_Business_List_To_Accountant.Rows.Count > 0)
        {
            GridView_UnAssigned_Business_List_To_Accountant.UseAccessibleHeader = true;
            GridView_UnAssigned_Business_List_To_Accountant.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_Assigned_Business_List_To_Accountant_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_Assigned_Business_List_To_Accountant.PageIndex = e.NewPageIndex;
    }

    protected void GridView_Assigned_Business_List_To_Accountant_PreRender(object sender, EventArgs e)
    {
        if (GridView_Assigned_Business_List_To_Accountant.Rows.Count > 0)
        {
            GridView_Assigned_Business_List_To_Accountant.UseAccessibleHeader = true;
            GridView_Assigned_Business_List_To_Accountant.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void chkemp_CheckedChanged(object sender, EventArgs e)
    {
        for (int i = 0; i < GridView_UnAssigned_Business_List_To_Accountant.Rows.Count; i++)
        {
            CheckBox chkemp = GridView_UnAssigned_Business_List_To_Accountant.Rows[i].FindControl("chkemp") as CheckBox;
            Label Label_EmployeeID_emp = GridView_UnAssigned_Business_List_To_Accountant.Rows[i].FindControl("Label_EmployeeID_emp") as Label;
            if (chkemp.Checked)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlaccountant.SelectedValue + "", "");
                var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID", "USERID=" + Check_USERID.Rows[0]["USERID"].ToString() + " and " +
                    "BusinessID=" + Label_EmployeeID_emp.Text.ToString() + "", "");

                if (Get_Business_Id.Rows.Count > 0)
                {
                    chkemp.Checked = false;
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Accountant is already Assigned in this Business, Please Select another Business', 'warning');", true);
                }
            }
        }
    }
    #endregion assign accountant code starts here






}