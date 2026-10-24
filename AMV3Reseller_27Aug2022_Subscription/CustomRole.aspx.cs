using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CustomRole : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AssgineManagerEntity Ame = new AssgineManagerEntity();
    AssgineManagerController Amc = new AssgineManagerController();
    AssgienDelegateEntity Ade = new AssgienDelegateEntity();
    AssgienDelegateController ADC = new AssgienDelegateController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Request.QueryString["BusinessIdFromDefaultPapge"] != null)
            {
                GetMangerList();
            }
        }
    }

    public void GetMangerList()
    {
        //comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "", "ab.EmployeeID");
        comm.FillDropDownList(ddlmanager, "AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID", "dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       ' ", "ab.EmployeeID");
    }

    protected void ddlmanager_SelectedIndexChanged(object sender, EventArgs e)
    {
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
        DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid NOT IN (1,2,4,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");
        //DataTable dt1 = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID , dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid=2 and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "  and ab.EmployeeID='" + this.ddlmanager.SelectedValue.ToString() + "'", "ab.EmployeeID");
        if (dt1.Rows.Count > 0)
        {
            ddlemp.Items.Clear();
            //commented by kumar
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (2,1,5) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and AB.EMPLOYEETYPE ='" + dt1.Rows[0]["EMPLOYEETYPE"].ToString() + "'", "ab.EmployeeID");
            //added by kumar as we have to show only the user type who is employee
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid in (4) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and AB.EMPLOYEETYPE ='" + dt1.Rows[0]["EMPLOYEETYPE"].ToString() + "'", "ab.EmployeeID");
            //kumar commented
            //DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid in (2,4,5,12) and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " ", "ab.EmployeeID");
            //kumar added after firoz sir commented
            DataTable dt = comm.GetAllDataFromTable("AddEmployee ab join AssginBussiness asb on ab.EmployeeID=asb.EmployeeID", "ab.EmployeeID,dbo.FN_DESC('AddEmployee',ab.EmployeeID) as empname", "UserTypeid not in (1,5) and ab.EmployeeID<>'" + this.ddlmanager.SelectedValue.ToString() + "' and asb.BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and ab.IsActive='YES       ' ", "ab.EmployeeID");
            ddlemp.DataSource = dt;
            ddlemp.DataTextField = "empname";
            ddlemp.DataValueField = "EmployeeID";
            ddlemp.DataBind();
            foreach (ListItem item in ddlemp.Items)
            {
                bool check = comm.IfExistsInTable("AssgineManager", "*", "BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + " and EmpId=" + item.Value + " and ManagerID=" + managerid + "");
                if (check == true)
                {
                    item.Selected = true;
                }
            }
        }
        //changes end by ragini on 02.11.2021
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        string keyWords = hfSelected.Value;
        string[] arrayval = keyWords.Trim().Split(',');

        if (ddlmanager.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Custom Role User First', 'warning');", true);
            return;
        }

        if (hfSelected.Value == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'If Employee is Assigned then Add or Remove Employee. <br/> Otherwise Select Employee to Assign Under Custom Role User if No Employee is Selected', 'warning');", true);
            return;
        }

        string busname = comm.GetNameFromId("AddBusiness", int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()));
        comm.ExecuteQuery("delete from AssgineManager where BusinessID=" + int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString()) + " and ManagerID=" + ddlmanager.SelectedValue + "");

        for (int i = 0; i < arrayval.Length; i++)
        {
            if (!string.IsNullOrEmpty(arrayval[i]))
            {
                Ame.EmpId = int.Parse(arrayval[i]);
                Ame.ManagerID = int.Parse(ddlmanager.SelectedValue);
                Ame.MangerName = ddlmanager.SelectedItem.Text;
                Ame.BusinessID = int.Parse(Request.QueryString["BusinessIdFromDefaultPapge"].ToString());
                Ame.EmpName = comm.GetNameFromId("AddEmployee", int.Parse(arrayval[i].ToString()));
                Ame.BusinessName = busname;
                Amc.InsertAssgineManger(Ame);
            }
        }
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Manager Assigned Successfully', 'success');", true);
        Response.Redirect("Home_Page.aspx");
    }

    protected void ddlemp_SelectedIndexChanged(object sender, EventArgs e)
    {
        //if (ddlmanager.SelectedValue != "")
        //{
        //    int a = 0;
        //    foreach (ListItem itm in ddlemp.Items)
        //    {
        //        if (itm.Selected == false)
        //        {
        //            string empid = itm.Value;
        //            bool chk = comm.IfExistsInTable("AssgineManager", "*", "ManagerID=" + ddlmanager.SelectedValue + " and BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "");
        //            if (chk == true)
        //            {
        //                a = comm.ExecuteQuery("delete from AssgineManager where EmpId=" + empid + " and ManagerID=" + ddlmanager.SelectedValue + " and BusinessID=" + Request.QueryString["BusinessIdFromDefaultPapge"].ToString() + "");
        //            }
        //        }
        //    }
        //    if (a > 0)
        //    {
        //        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Employee Removed from Manger " + ddlmanager.SelectedItem.Text + "  Successfully', 'success');", true);
        //    }
        //}
    }
}