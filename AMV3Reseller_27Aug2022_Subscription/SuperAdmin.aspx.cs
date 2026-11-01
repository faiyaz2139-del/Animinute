using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class SuperAdmin : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        // v42 PPT 03 s1 "Remove this screen - it's not required": Support Admin goes to Subscriber Management instead.
        if (AnyminuteSupportAdmin.IsSupportAdminOnly()) { Response.Redirect("SupportSubscribers.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
        if (!Page.IsPostBack)
        {
            GetMotherOrgList();
        }
    }

    public void GetMotherOrgList()
    {
        string email = Session["email"].ToString();
        string UserType = Session["UserType"].ToString();
        string bussOwnerCnt="", busscnt="", empcnt = "";
        bussOwnerCnt = "(select count(DISTINCT AD.EmployeeID) from AddEmployee Ad  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID  and ad.UserType=1 join UserAcc ua on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB ON ASSB.EmployeeID = ad.EmployeeId where(ad.MotherOrgID = m.MotherOrgID or ASSB.MotherOrgID = m.MotherOrgID)) bussOwnerCnt,";
        busscnt = "(select count(BusinessID) from AddBusiness where MotherOrgid=m.MotherOrgID) as busscnt,";
        empcnt = "(select COUNT(EmployeeID) from AssginBussiness where MotherOrgid=m.MotherOrgID and UserTypeId=4)empcnt";
        //Comm.FillGridView(GVMother, "MotherOrg m join UserAcc ua on ua.MotherOrgid=m.MotherOrgid and UserType='Admin'", "m.*,CONVERT(nvarchar(10), EnrollDate,120)as EnrollDate1", "ua.Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password, "+ bussOwnerCnt + ""+ busscnt + ""+ empcnt + "", "", "MotherOrgID desc");
        //Comm.FillGridView(GVMother, "MotherOrg m join UserAcc ua on ua.MotherOrgid=m.MotherOrgid and UserType='Admin'", "m.*,CONVERT(nvarchar(10), EnrollDate,120)as EnrollDate1", "ua.Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password , " + bussOwnerCnt + "" + busscnt + "" + empcnt + "", "", "MotherOrgID desc");
        Comm.FillGridView(GVMother, "MotherOrg m join UserAcc ua on ua.MotherOrgid=m.MotherOrgid and UserType='Admin' and m.Email not in (select EMail_primary from Register_Partner)", "m.*,CONVERT(nvarchar(10), EnrollDate,120)as EnrollDate1", "ua.Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password , " + bussOwnerCnt + "" + busscnt + "" + empcnt + "", "", "MotherOrgID desc");
    }

    protected void GVMother_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVMother.PageIndex = e.NewPageIndex;
        GetMotherOrgList();
    }

    protected void GVMother_PreRender(object sender, EventArgs e)
    {
        if (GVMother.Rows.Count > 0)
        {
            GVMother.UseAccessibleHeader = true;
            GVMother.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GVMother_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // v36 (DOCX image7): status pill (Active / Inactive) instead of the active.png / deactive.png image toggle.
            LinkButton btnactive = (e.Row.FindControl("btnactive") as LinkButton);
            HiddenField hfdStatusForAdminBussOwner = (e.Row.FindControl("hfdStatusForAdminBussOwner") as HiddenField);
            bool active = hfdStatusForAdminBussOwner.Value == "True";
            btnactive.Text = active ? "Active" : "Inactive";
            btnactive.CssClass = "sa-pill am-sa-status " + (active ? "sa-green" : "sa-red");
            btnactive.OnClientClick = "return confirm('" + (active ? "Deactivate this accountant and its businesses?" : "Activate this accountant and its businesses?") + "');";
        }
    }

    protected void GVMother_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "status")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField hfdStatusForAdminBussOwner = (HiddenField)GVMother.Rows[index].FindControl("hfdStatusForAdminBussOwner");
            HiddenField hfdid = (HiddenField)GVMother.Rows[index].FindControl("hfdid");

            if (hfdid.Value != string.Empty)
            {
                if (hfdStatusForAdminBussOwner.Value == "True")
                {
                    DataTable dtBuss = new DataTable();
                    DataTable dtEmp = new DataTable();
                    dtBuss = Comm.GetAllDataFromTable("AddBusiness", "*", "MotherOrgID=" + hfdid.Value + "", "");
                    dtEmp= Comm.GetAllDataFromTable("AddEmployee", "*", "MotherOrgID=" + hfdid.Value + "", "");
                    foreach(DataRow rBuss in dtBuss.Rows)
                    {
                        Comm.ExecuteQuery("update AddBusiness set Status=2 where BusinessID='"+ rBuss["BusinessID"].ToString() + "'");
                    }
                    foreach (DataRow rEmp in dtEmp.Rows)
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + rEmp["EmployeeID"].ToString() + "'");
                    }
                    Comm.ExecuteQuery("Update MotherOrg set Status='False' where MotherOrgID=" + hfdid.Value + "");
                }
                else
                {
                    DataTable dtBuss = new DataTable();
                    DataTable dtEmp = new DataTable();
                    dtBuss = Comm.GetAllDataFromTable("AddBusiness", "*", "MotherOrgID=" + hfdid.Value + "", "");
                    dtEmp = Comm.GetAllDataFromTable("AddEmployee", "*", "MotherOrgID=" + hfdid.Value + "", "");
                    foreach (DataRow rBuss in dtBuss.Rows)
                    {
                        Comm.ExecuteQuery("update AddBusiness set Status=1 where BusinessID='" + rBuss["BusinessID"].ToString() + "'");
                    }
                    foreach (DataRow rEmp in dtEmp.Rows)
                    {
                        Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + rEmp["EmployeeID"].ToString() + "'");
                    }
                    Comm.ExecuteQuery("Update MotherOrg set Status='True' where MotherOrgID=" + hfdid.Value + "");
                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed !', 'success');", true);
                GetMotherOrgList();
            }
        }
    }
}