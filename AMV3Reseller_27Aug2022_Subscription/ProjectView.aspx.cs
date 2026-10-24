using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class ProjectView : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    AddProject AddProject = new AddProject();
    ActiveDeactiveController ADCtrl = new ActiveDeactiveController();
    LoginClass LogClass = new LoginClass();
    int mangercheck = 0;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["MotherOrgid"] != null)
            {
                if (Session["UserType"].ToString() != "DELIGATES")
                {
                    GetBusiness();
                    GetEmplist();
                }
                else
                {

                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    public void GetBusiness()
    {
        if (Session["EmployeeId"].ToString() == "0" || Session["UserType"].ToString() == "BUSINESS OWNER" || Session["UserType"].ToString() == "LEVEL 2 (SUPERVISOR)" || Session["UserType"].ToString() == "LEVEL 1 (EMPLOYEE)" || Session["UserType"].ToString() == "LEVEL 3 (OWNER)")
        {
            DataTable dtBussList = new DataTable();
            if (Session["EmployeeId"].ToString() == "0")
            {

            }
            else
            {
                string empid = "0";
                DataTable dtCommaUser = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join UserAcc ua on ua.Email=ea.MainEmail", "ad.EmployeeID,ua.UserId", "ua.Email='" + Session["email"].ToString() + "'", "");
                string CommaUserid = string.Empty;
                for (int i = 0; i < dtCommaUser.Rows.Count; i++)
                {
                    CommaUserid = CommaUserid + dtCommaUser.Rows[i]["UserId"].ToString();
                    CommaUserid += (i < dtCommaUser.Rows.Count) ? "," : string.Empty;
                }
                CommaUserid = CommaUserid.TrimEnd(',');
                DataTable dtMapping = new DataTable();
                dtMapping = comm.GetAllDataFromTable("MappingTable", "*", "USERID in(" + CommaUserid + ")", "BusinessID,UserTypeId");

                int cnt = dtMapping.Rows.Count;
                DataTable dt = new DataTable();
                foreach (DataRow row in dtMapping.Rows)
                {
                    if (row["UserTypeId"].ToString() == "1")
                    {

                    }
                    else if (row["UserTypeId"].ToString() == "2")
                    {

                    }
                    else if (row["UserTypeId"].ToString() == "4")
                    {
                        dividEmp.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            dividEmp.Attributes.Add("style", "display:block;");
                        }

                        empid = row["EmployeeID"].ToString() + "," + empid;

                        if (empid.Length > 0)
                        {string firstcolumn = "  ae.EmployeeID,assb.BusinessID,asp.PROJECTID,AddProject.ProjectName,CONVERT(NVARCHAR,AddProject.startDate, 106)startDate ";
                            string secondcolumn = "  dbo.FN_DESC('AddEmployee', ae.EmployeeID)emp ";
                            string tablename = " AddEmployee Ae join ASSIGNPROJECT asp on asp.EMPLOYEEID = ae.EmployeeID join AddProject ";
                            tablename += " on addproject.ProjectID = asp.PROJECTID and AddProject.Status = 'A' ";
                            tablename += " join AssginBussiness Assb";
                            tablename += " on ae.EmployeeID = Assb.EmployeeID join EmpAddress ea on ea.EmpAddressId = ae.ADDRESSID and ea.MainEmail <> ''";
                            string whereclause = "  ae.EmployeeID in (" + empid + ") and assb.UserTypeId = " + row["UserTypeid"].ToString() + "";
                            whereclause += " and ae.IsActive = 'YES'  AND Assb.BusinessID = asp.BUSINESSID";
                            whereclause += " and assb.BusinessID = asp.BUSINESSID";
                            whereclause += " AND assb.BusinessID IN(SELECT BusinessID from AddBusiness where AddBusiness.Status= 1) ";
                            dt = comm.GetAllDataFromTable(tablename, " distinct assb.BusinessID,   dbo.FN_DESC('AddBusiness',assb.BusinessID)BusinessName ", whereclause, "");
                        }
                    }
                    else if (row["UserTypeId"].ToString() == "5")
                    {

                    }                   
                }

                ddlBusiness.Items.Clear();
                ddlBusiness.DataSource = dt;
                ddlBusiness.DataTextField = "BusinessName";
                ddlBusiness.DataValueField = "BusinessID";
                ddlBusiness.DataBind();
                if (dt.Rows.Count > 0)
                {
                    ddlBusiness.Visible = true;
                }
                else
                {
                    ddlBusiness.Visible = false;
                }
            }
        }
        else
        {

        }      
    }

    public void GetEmplist()
    {
        if (Session["EmployeeId"].ToString() == "0" || Session["UserType"].ToString() == "BUSINESS OWNER" || Session["UserType"].ToString() == "LEVEL 2 (SUPERVISOR)" || Session["UserType"].ToString() == "LEVEL 1 (EMPLOYEE)" || Session["UserType"].ToString() == "LEVEL 3 (OWNER)")
        {
            DataTable dtBussList = new DataTable();
            if (Session["EmployeeId"].ToString() == "0")
            {
                
            }
            else
            {
                string empid = "0";
                DataTable dtCommaUser = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join UserAcc ua on ua.Email=ea.MainEmail", "ad.EmployeeID,ua.UserId", "ua.Email='" + Session["email"].ToString() + "'", "");
                string CommaUserid = string.Empty;
                for (int i = 0; i < dtCommaUser.Rows.Count; i++)
                {
                    CommaUserid = CommaUserid + dtCommaUser.Rows[i]["UserId"].ToString();
                    CommaUserid += (i < dtCommaUser.Rows.Count) ? "," : string.Empty;
                }
                CommaUserid = CommaUserid.TrimEnd(',');
                DataTable dtMapping = new DataTable();
                dtMapping = comm.GetAllDataFromTable("MappingTable", "*", "USERID in(" + CommaUserid + ")", "BusinessID,UserTypeId");

                int cnt = dtMapping.Rows.Count;
                DataTable dt = new DataTable();
                foreach (DataRow row in dtMapping.Rows)
                {
                    if (row["UserTypeId"].ToString() == "1")
                    {

                    }
                    else if (row["UserTypeId"].ToString() == "2")
                    {

                    }
                    else if (row["UserTypeId"].ToString() == "4")
                    {
                        dividEmp.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            dividEmp.Attributes.Add("style", "display:block;");
                        }

                        empid = row["EmployeeID"].ToString() + "," + empid;
                        if (empid.Length > 0)
                        {
                            string firstcolumn = "  ae.EmployeeID,assb.BusinessID,asp.PROJECTID,AddProject.ProjectName,CONVERT(NVARCHAR,AddProject.startDate, 106)startDate,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName ";
                            string secondcolumn = "  dbo.FN_DESC('AddEmployee', ae.EmployeeID)emp ";
                            string tablename = " AddEmployee Ae join ASSIGNPROJECT asp on asp.EMPLOYEEID = ae.EmployeeID join AddProject ";
                            tablename += " on addproject.ProjectID = asp.PROJECTID and AddProject.Status = 'A' ";
                            tablename += " join AssginBussiness Assb";
                            tablename += " on ae.EmployeeID = Assb.EmployeeID join EmpAddress ea on ea.EmpAddressId = ae.ADDRESSID and ea.MainEmail <> ''";
                            string whereclause = "  ae.EmployeeID in (" + empid + ") and assb.UserTypeId = " + row["UserTypeid"].ToString() + "";
                            whereclause += " and ae.IsActive = 'YES'  AND Assb.BusinessID = asp.BUSINESSID";
                            whereclause += " and assb.BusinessID = asp.BUSINESSID  AND Assb.BusinessID = '"+ddlBusiness.SelectedValue.ToString()+"'";
                            whereclause += " AND assb.BusinessID IN(SELECT BusinessID from AddBusiness where AddBusiness.Status= 1) ";
                            comm.FillGridView(gvBusinessListEmp, tablename, firstcolumn, secondcolumn, whereclause, "ae.EmployeeID");
                        }
                    }
                    else if (row["UserTypeId"].ToString() == "5")
                    {

                    }                    
                }
            }
        }
        else
        {
           
        }      
    }

    protected void gvBusinessListEmp_PreRender(object sender, EventArgs e)
    {
        if (gvBusinessListEmp.Rows.Count > 0)
        {
            gvBusinessListEmp.UseAccessibleHeader = true;
            gvBusinessListEmp.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void ddlBusiness_SelectedIndexChanged(object sender, EventArgs e)
    {       
        GetEmplist();
    }
}