using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GridviewCustomRole: System.Web.UI.Page
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
            //DataTable DataTable_ManagerList = Comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID, dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=2078 and ab.IsActive='YES       '", "ab.EmployeeID");
            //GridView_ManagerList.DataSource = DataTable_ManagerList;
            //GridView_ManagerList.DataBind();

            //DataTable DataTable_EmployeeList = Comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (1,2,5,12) and asb.BusinessID=2078 and ab.IsActive='YES       ' ", "ab.EmployeeID");
            //GridView_EmployeeList.DataSource = DataTable_EmployeeList;
            //GridView_EmployeeList.DataBind();
        }
    }
    public void GetMangerList()
    {
        //comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       '", "ab.EmployeeID");

        comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       ' ", "ab.EmployeeID");
    }

    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e)
    {
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

        //comm.FillDropDownList(ddlemp, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserType not in (2,1) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "", "ab.EmployeeID");
        //changes by ragini on 02.11.2021
        //DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname,ab.EMPLOYEETYPE", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");
        //kumar commented

        //DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");

        DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");


        if (dt1.Rows.Count > 0)
        {
           // GridView_EmployeeList.Rows.Clear();
            //commented by kumar
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (2,1,5) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and AB.EMPLOYEETYPE ='" + dt1.Rows[0]["EMPLOYEETYPE"].ToString() + "'", "ab.EmployeeID");
            //added by kumar as we have to show only the user type who is employee
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid in (4) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and AB.EMPLOYEETYPE ='" + dt1.Rows[0]["EMPLOYEETYPE"].ToString() + "'", "ab.EmployeeID");
            //kumar commented
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid in (4) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " ", "ab.EmployeeID");
            //added by kumar because custom role as well as employee can be assigned under the manager
            DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (1,2,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       ' and ab.EmployeeID!='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");
            GridView_EmployeeList.DataSource = dt;
            //ddlemp.DataTextField = "empname";
            //ddlemp.DataValueField = "EmployeeID";
            GridView_EmployeeList.DataBind();
          
            DataTable dtemp = dt.Clone();
            DataTable dtlist = dt.Clone();
            for (int i=0;i<dt.Rows.Count;i++)
            {
                bool check = comm.IfExistsInTable("AssgineManager", "*", "BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and EmpId=" + dt.Rows[i]["EmployeeID"].ToString() + " and ManagerID=" + managerid + "");
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
            GridView_EmployeeList.DataSource = dtlist;

            GridView_EmployeeList.DataBind();
            GridViewMgr.DataSource = dtemp;
            
            GridViewMgr.DataBind();
        }
        //changes end by ragini on 02.11.2021
    }

    protected void Button_LeftMove_Click(object sender, EventArgs e)
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
        GridViewMgr.DataSource = dtemp;
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridViewMgr.DataBind();
        GridView_EmployeeList.DataSource = dtlist;
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridView_EmployeeList.DataBind();



        // run kariye
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string keyWords = hfSelected.Value;
        DataTable  dtemp = ViewState["dtemp"] as DataTable;
        string[] arrayval = keyWords.Trim().Split(',');

        if (ddlmanager.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Custom Role First', 'warning');", true);
            return;
        }

        //if (dtemp.Rows.Count == 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'If Employee is Assigned then Add or Remove Employee. <br/> Otherwise Select Employee to Assign Under Manager if No Employee is Selected', 'warning');", true);
        //    return;
        //}

        string busname = comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()));
        comm.ExecuteQuery("delete from AssgineManager where BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and ManagerID=" + ddlmanager.SelectedValue + "");

        for (int i = 0; i < dtemp.Rows.Count; i++)
        {
            
               
                Ame.EmpId = int.Parse(dtemp.Rows[i]["EmployeeID"].ToString());
                Ame.ManagerID = int.Parse(ddlmanager.SelectedValue);
                Ame.MangerName = ddlmanager.SelectedItem.Text;
                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
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

    protected void Button_RightMove_Click(object sender, EventArgs e)
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
            }
            else
            {
                dtemplst.Rows.Add(dtemp.Rows[i].ItemArray);

            }
        }
        ViewState["dt"] = dtlist;
        ViewState["dtemp"] = dtemplst;
        GridViewMgr.DataSource = dtemplst;
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridViewMgr.DataBind();
        GridView_EmployeeList.DataSource = dtlist;
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridView_EmployeeList.DataBind();
    }

    protected void Button_LeftMoveAll_Click(object sender, EventArgs e)
    {
        //DataTable dt = ViewState["dt"] as DataTable;
        //DataTable dtlist = dt;
        //DataTable dtemp = new DataTable();
        //DataTable dtemplst = dt.Clone();
        //try
        //{
        //    dtemp = ViewState["dtemp"] as DataTable;
        //}
        //catch (Exception)
        //{


        //}
        //if (dtemp == null)
        //{
        //    dtemp = dt.Clone();
        //}
        //dtemplst = dtemp;
        //dtemp = dt.Clone();
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
           
        //ViewState["dt"] = dtemp;
        //ViewState["dtemp"] = dtemplst;
        //GridViewMgr.DataSource = dtemplst;
        ////ddlemp.DataTextField = "empname";
        ////ddlemp.DataValueField = "EmployeeID";
        //GridViewMgr.DataBind();
        //GridView_EmployeeList.DataSource = dtemp;
        ////ddlemp.DataTextField = "empname";
        ////ddlemp.DataValueField = "EmployeeID";
        //GridView_EmployeeList.DataBind();

       

    }

    protected void Button_RightMoveAll_Click(object sender, EventArgs e)
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
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridViewMgr.DataBind();
        GridView_EmployeeList.DataSource = dtlist;
        //ddlemp.DataTextField = "empname";
        //ddlemp.DataValueField = "EmployeeID";
        GridView_EmployeeList.DataBind();
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
}