using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GridviewAssignAccountant : System.Web.UI.Page
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
            GetMangerList();
        }
    }
    public void GetMangerList()
    {
        //comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       '", "ab.EmployeeID");

        //commented because same name was showing multiple times if user is registered multiple times
        //comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid IN (12) and asb.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID where AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' AND AsB.BusinessID IS NOT NULL) and ab.IsActive='YES       ' ", "ab.EmployeeID");

        comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID = asb.EmployeeID", "max(EmployeeID) as EmployeeID", "temp.empname from (Select ab.EmployeeID, dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid IN (12) and asb.BusinessID IN (Select asb.BusinessID from AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID where AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' AND AsB.BusinessID IS NOT NULL) and ab.IsActive = 'YES')temp group by temp.empname", "EmployeeID");

        ddlmanager.Items.Add(new ListItem() { Value = "0", Text = "Add New Accountant" });
    }

    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlmanager.SelectedValue == "0")
        {
            Response.Redirect("Add_EmployeeV2_Accountant.aspx");
        }

        table_button.Visible = true;
        Button_LeftMove.Visible = true;
        Button_LeftMoveAll.Visible = true;
        Button_RightMove.Visible = true;
        Button_RightMoveAll.Visible = true;
        btnSave.Visible = true;
        int managerid = 0;
        if (ddlmanager.SelectedValue != "")
        {
            managerid = int.Parse(ddlmanager.SelectedValue);
        }

        if (ddlmanager.SelectedValue == "")
        {
            return;
        }

        DataTable dt1 = comm.GetAllDataFromTable("AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID", "asb.BusinessID,ab.CompanyName,ab.Status", "AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' and ab.Status=1 AND AsB.BusinessID IS NOT NULL", "asb.BusinessID");

        GridView_EmployeeList.DataSource = dt1;
        GridView_EmployeeList.DataBind();

        if (dt1.Rows.Count > 0)
        {
            DataTable dt = comm.GetAllDataFromTable("AssginBussiness AsB join AddBusiness ab on ab.BusinessID = asb.BusinessID", "asb.BusinessID,ab.CompanyName,ab.Status", "AsB.UserTypeId=1 and AsB.EmployeeID='" + Session["EmployeeId"] + "' and ab.Status=1 AND AsB.BusinessID IS NOT NULL", "asb.BusinessID");
            GridView_EmployeeList.DataSource = dt;
            GridView_EmployeeList.DataBind();

            DataTable dtlist = dt.Clone();
            DataTable dtemp = dt.Clone();
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlmanager.SelectedValue + "", "");

                var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID,EmployeeId", "USERID=" + Check_USERID.Rows[0]["USERID"].ToString() + " " +
                    "and BusinessID=" + dt.Rows[i]["BusinessID"].ToString() + " and UserTypeId=12 ", "");
                
                if (Get_Business_Id.Rows.Count > 0)
                {
                    var Check_Active_Inactive = comm.GetAllDataFromTable("AddEmployee", "IsActive", "EmployeeID=" + Get_Business_Id.Rows[0]["EmployeeId"].ToString() + " " +
                    "and UserType=12 and IsActive='YES       ' ", "");
                    //"and UserType=12 ", "");

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

            GridView_EmployeeList.DataSource = dtlist;
            GridView_EmployeeList.DataBind();

            GridViewMgr.DataSource = dtemp;
            GridViewMgr.DataBind();
        }
    }

    protected void Button_LeftMove_Click(object sender, EventArgs e) //assign one by one code
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
        for (int i = 0; i < GridView_EmployeeList.Rows.Count; i++)
        {
            CheckBox chk = GridView_EmployeeList.Rows[i].FindControl("chkemp") as CheckBox;
            Label Label_EmployeeID_emp = GridView_EmployeeList.Rows[i].FindControl("Label_EmployeeID_emp") as Label;
            if (chk.Checked)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlmanager.SelectedValue + "", "");
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

        GridViewMgr.DataSource = dtemp;
        GridViewMgr.DataBind();

        GridView_EmployeeList.DataSource = dtlist;
        GridView_EmployeeList.DataBind();
    }

    protected void Button_LeftMoveAll_Click(object sender, EventArgs e) //assign all code
    {
        try
        {
            for (int i = 0; i < GridView_EmployeeList.Rows.Count; i++)
            {
                CheckBox chk = GridView_EmployeeList.Rows[i].FindControl("chkemp") as CheckBox;
                chk.Checked = true;
            }
        }
        catch (Exception)
        {

        }
        Button_LeftMove_Click(sender, e);
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (ddlmanager.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Accountant First', 'warning');", true);
            return;
        }

        //if (dtemp.Rows.Count == 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'If Employee is Assigned then Add or Remove Employee. <br/> Otherwise Select Employee to Assign Under Manager if No Employee is Selected', 'warning');", true);
        //    return;
        //}

        DataTable dtemp = ViewState["dtemp"] as DataTable;
        for (int i = 0; i < dtemp.Rows.Count; i++)
        {
            int BusId = int.Parse(dtemp.Rows[i]["BusinessID"].ToString());
            string BusName = dtemp.Rows[i]["CompanyName"].ToString();

            var Get_User_Id = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeID=" + ddlmanager.SelectedValue + "", "");

            var Get_Business_Id = comm.GetAllDataFromTable("MappingTable", "BusinessID", "USERID=" + Get_User_Id.Rows[0]["USERID"].ToString() + " and BusinessID=" + BusId + "", "");

            if (Get_Business_Id.Rows.Count == 0)
            {
                int GetEmpId = Comm.ExecuteQuery("INSERT INTO AddEmployee (UserType, Title, FirstName, MiddleName, LastName, DateOfBirth, Gender, SocialInsuranceNumber, MobilePhone, HomePhone, EMail_primary, " +
                        " EMail_secondary, StreetNumber, StreetName, Apartment, City, Province, PostalCode, HireDate, BillRate, BillType, JobTitle, TerminationDate, IsEmployeeEIExempt, IsEmployeeCPPExempt, RetainVacation, " +
                        " MotherOrgID, BusinessID, MangerId, BusinessOwnerId, IsActive, LegalName, Occupation, ADDRESSID, Rateid, SINExpiry, IsEdit, EMPLOYEETYPE) " +
                        " Output Inserted.EmployeeID" +
                        " SELECT UserType, Title, FirstName, MiddleName, LastName, DateOfBirth, Gender, SocialInsuranceNumber, MobilePhone, HomePhone, EMail_primary, " +
                        " EMail_secondary, StreetNumber, StreetName, Apartment, City, Province, PostalCode, HireDate, BillRate, BillType, JobTitle, TerminationDate, IsEmployeeEIExempt, IsEmployeeCPPExempt, RetainVacation, " +
                        " MotherOrgID, BusinessID, MangerId, BusinessOwnerId, IsActive, LegalName, Occupation, ADDRESSID, Rateid, SINExpiry, IsEdit, EMPLOYEETYPE FROM AddEmployee where EmployeeID = " + ddlmanager.SelectedValue + " ");

                var Get_EmpId = comm.GetAllDataFromTable("AddEmployee", " top 1 EmployeeID", "", "1 desc");

                int getRateId = Comm.ExecuteQuery("INSERT INTO [RateDetails] ([EmployeeId], [Rate], [RateType], [StartDate], [EndDate], [JobTitle], " +
                    " [IsEmployeeEIExempty], [IsEmployeeCPPExempty], [RetainVacation], [effectivedate]) " +
                    " Output Inserted.RateId" +
                    " SELECT " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", [Rate], [RateType], [StartDate], [EndDate], [JobTitle], [IsEmployeeEIExempty], [IsEmployeeCPPExempty], [RetainVacation], [effectivedate] " +
                    " FROM [RateDetails] where EmployeeId = " + ddlmanager.SelectedValue + " ");

                var get_RateId = comm.GetAllDataFromTable("RateDetails", " top 1 RateId", "", "1 desc");

                Comm.ExecuteQuery("update AddEmployee set Rateid=" + get_RateId.Rows[0]["RateId"].ToString() + " where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                int GetAddressId = Comm.ExecuteQuery("INSERT INTO [EmpAddress] ([Postalcode], [NumberSuffix], [StreetName], [StreetType], [StreetDirection], [SuitType], [SuiteNumber], [SuitSuffix], " +
                    " [City], [Province], [MainPhone], [WorkPhone], [Mobile], [Fax], [MainEmail], [CCEmail], [Website], [Other1], [StreetNo]) " +
                    " Output Inserted.EmpAddressId" +
                    " SELECT [Postalcode], [NumberSuffix], [StreetName], [StreetType], [StreetDirection], [SuitType], [SuiteNumber], [SuitSuffix], " +
                    " [City], [Province], [MainPhone], [WorkPhone], [Mobile], [Fax], [MainEmail], [CCEmail], [Website], [Other1], [StreetNo] " +
                    " FROM [EmpAddress] where EmpAddressId in (select ADDRESSID from AddEmployee where EmployeeId=" + ddlmanager.SelectedValue + ") ");

                var Get_AddressId = comm.GetAllDataFromTable("EmpAddress", " top 1 EmpAddressId", "", "1 desc");

                Comm.ExecuteQuery("update AddEmployee set ADDRESSID=" + Get_AddressId.Rows[0]["EmpAddressId"].ToString() + " where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                Comm.ExecuteQuery("INSERT INTO [AssginBussiness] ([BusinessID], [EmployeeID], [BusinessName], [EmployeeName], [UserTypeId]) " +
                    " Values (" + BusId + ", " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", '" + BusName + "', '" + ddlmanager.SelectedItem.Text + "', 12) ");

                //Comm.ExecuteQuery("INSERT INTO [AssginBussiness] ([BusinessID], [EmployeeID], [BusinessName], [EmployeeName], [UserTypeId]) " +
                //    " Values (" + BusId + ", " + ddlmanager.SelectedValue + ", '" + BusName + "', '" + ddlmanager.SelectedItem.Text + "', 12) ");

                Comm.ExecuteQuery("update AssginBussiness set MotherOrgID=(select MotherOrgID from AddEmployee where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ") " +
                    "where EmployeeID=" + Get_EmpId.Rows[0]["EmployeeID"].ToString() + " ");

                var RolePermission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id", "Employee_Id=" + ddlmanager.SelectedValue + "", "");

                Comm.ExecuteQuery("INSERT INTO User_Role_Permission ([Role_Permission_Id], [Employee_Id], [UserTypeId], [Status], [BusinessId] " + " ) " +
                    "values ('" + RolePermission.Rows[0]["Role_Permission_Id"].ToString() + "', " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + ", 12, 1, '" + BusId + "')");

                var GetUserID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlmanager.SelectedValue + "", "");

                comm.ExecuteQuery("insert into MappingTable(USERID, UserTypeId, EmployeeID, BusinessID) " +
                    " values('" + GetUserID.Rows[0]["USERID"].ToString() + "', 12, " + Get_EmpId.Rows[0]["EmployeeID"].ToString() + "," + BusId + ")");
            }
        }

        for (int i = 0; i < GridView_UnAssigned_AccountantList.Rows.Count; i++)
        {
            //int BusId_unassign = int.Parse(dt_unassigned_emp.Rows[i]["BusinessID"].ToString());
            Label BusId_unassign = GridView_UnAssigned_AccountantList.Rows[i].FindControl("Label_EmployeeID_emp") as Label;

            var Get_User_Id_unassign = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlmanager.SelectedValue + "", "");

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

                    //if (Get_User_Id_unassign.Rows.Count != GridViewMgr.Rows.Count)
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

    //DataTable dt_unassigned_emp = new DataTable();
    protected void Button_RightMove_Click(object sender, EventArgs e) //unassign one by one code
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
        for (int i = 0; i < GridViewMgr.Rows.Count; i++)
        {
            CheckBox chk = GridViewMgr.Rows[i].FindControl("chkemp") as CheckBox;
            if (chk.Checked)
            {
                dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
                //dt_unassigned_emp.Rows.Add(dtemp.Rows[i].ItemArray);//Datatable coode did not worked, so commeneted this for unassigning and taken a hidden gridview to hold data
            }
            else
            {
                dtemplst.Rows.Add(dtemp.Rows[i].ItemArray);
            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;

        GridViewMgr.DataSource = dtemplst;
        GridViewMgr.DataBind();

        GridView_EmployeeList.DataSource = dtlist;
        GridView_EmployeeList.DataBind();

        GridView_UnAssigned_AccountantList.DataSource = dtlist;
        GridView_UnAssigned_AccountantList.DataBind();
    }    

    protected void Button_RightMoveAll_Click(object sender, EventArgs e) //unassign all code
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
        for (int i = 0; i < GridViewMgr.Rows.Count; i++)
        {
            dtlist.Rows.Add(dtemp.Rows[i].ItemArray);
        }
       
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;

        GridViewMgr.DataSource = dtemplst;
        GridViewMgr.DataBind();

        GridView_EmployeeList.DataSource = dtlist;
        GridView_EmployeeList.DataBind();

        GridView_UnAssigned_AccountantList.DataSource = dtlist;
        GridView_UnAssigned_AccountantList.DataBind();
    }

    protected void GridView_EmployeeList_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_EmployeeList.PageIndex = e.NewPageIndex;
    }

    protected void GridView_EmployeeList_PreRender(object sender, EventArgs e)
    {
        if (GridView_EmployeeList.Rows.Count > 0)
        {
            GridView_EmployeeList.UseAccessibleHeader = true;
            GridView_EmployeeList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridViewMgr_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridViewMgr.PageIndex = e.NewPageIndex;
    }

    protected void GridViewMgr_PreRender(object sender, EventArgs e)
    {
        if (GridViewMgr.Rows.Count > 0)
        {
            GridViewMgr.UseAccessibleHeader = true;
            GridViewMgr.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void chkemp_CheckedChanged(object sender, EventArgs e)
    {
        for (int i = 0; i < GridView_EmployeeList.Rows.Count; i++)
        {
            CheckBox chkemp = GridView_EmployeeList.Rows[i].FindControl("chkemp") as CheckBox;
            Label Label_EmployeeID_emp = GridView_EmployeeList.Rows[i].FindControl("Label_EmployeeID_emp") as Label;
            if (chkemp.Checked)
            {
                var Check_USERID = comm.GetAllDataFromTable("MappingTable", "USERID", "EmployeeId=" + ddlmanager.SelectedValue + "", "");
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
}