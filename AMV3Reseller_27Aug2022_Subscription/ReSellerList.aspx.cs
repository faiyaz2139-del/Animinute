using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ReSellerList : System.Web.UI.Page
{
    SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
    CommanClass comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            if(Request.QueryString["name"]!=null)
            {
                lblTitle.Text = "Reseller Name : " + Request.QueryString["name"].ToString();
            }
            if(Request.QueryString["empid"] != null)
            {
                var empid = Request.QueryString["empid"].ToString();
                ////comm.FillDropDownList(ddlReseller,"AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct ad.BusinessID", "ad.CompanyName", "ad.MotherOrgId=" + empid + "and AsB.UserTypeId<>1", "ad.BusinessID");
                //comm.FillDropDownList(ddlReseller, "AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID", "dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + empid + "and AsB.UserTypeId=1", "asb.EmployeeID");
                //comm.FillDropDownList(ddlReseller, "AddBusiness ad join AssginBussiness asb on ad.BusinessID=asb.BusinessID", "distinct asb.EmployeeID", "dbo.FN_DESC('AddEmployee',asb.EmployeeID)empname  ", "ad.MotherOrgId=" + empid + "and AsB.UserTypeId=1", "asb.EmployeeID");
                comm.FillDropDownList(ddlReseller, "AddEmployee Ad join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID and ad.UserType=1 join UserAcc ua on ua.Email = EmpAdd.MainEmail JOIN AssginBussiness ASSB ON ASSB.EmployeeID=ad.EmployeeId", "DISTINCT AD.EmployeeID,dbo.FN_DESC('AddEmployee',AD.EmployeeID)empname", "dbo.FN_DESC('AddEmployee',AD.EmployeeID)empname", "ad.UserType=1 and (Ad.MotherOrgID=" + empid + " or Assb.MotherOrgID=" + empid + ")", "AD.EmployeeID desc");
                //select EmployeeID from AddEmployee where MotherOrgID=7061 and UserType=1 order by EmployeeID desc //simple query to fetch all the subscribers of a reseller.
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
    protected void gvEmplistPop_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        string status_change_confirm_value = Request.Form["Toggle_activate_deactivate"];
        if (status_change_confirm_value == "Yes" || status_change_confirm_value == "null" || status_change_confirm_value == null)
        {

        }
        else
        {
            return;
        }

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
                    comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + empid + "");
                    //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                    comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + empid + "'");

                }
                else
                {
                    comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + empid + "");
                    //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                    comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + empid + "'");
                }
                if (Request.QueryString["type"] != null)
                {
                    var type = Request.QueryString["type"].ToString();
                    var empidQuery = Request.QueryString["empid"].ToString();
                    var seletbuss = ddlReseller.SelectedValue;
                    DataTable dt = supportAdminCtrl.GetEmplist_SuppAdmin(type, int.Parse(empidQuery), int.Parse(seletbuss));
                    if (dt.Rows.Count > 0)
                    {
                        gvEmplistPop.DataSource = dt;
                        gvEmplistPop.DataBind();
                    }
                }
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
            }
        }
    }

    protected void ddlReseller_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(Request.QueryString["type"] != null)
        {
            if (ddlReseller.SelectedValue == "")
            {
                gvEmplistPop.DataSource = null;
                gvEmplistPop.DataBind();
                return;
            }

            var type = Request.QueryString["type"].ToString();
            var empid= Request.QueryString["empid"].ToString();
            var seletbuss = ddlReseller.SelectedValue;
            DataTable dt = supportAdminCtrl.GetEmplist_SuppAdmin(type,int.Parse(empid),int.Parse(seletbuss));
            DataTable dtact= comm.ExecuteQueryDataTable("select IsActive FROM AddEmployee where EmployeeID='"+ seletbuss + "'");
            if(dtact.Rows.Count>0)
            {
                if (dtact.Rows[0]["IsActive"].ToString().Trim() != "YES")
                {
                    btnall.Text = "Active Subscriber";
                }
            }
            
            if (dt.Rows.Count > 0)
            {               
                gvEmplistPop.DataSource = dt;
                gvEmplistPop.DataBind();
            }
            if (dt.Rows.Count == 0)
            {
                gvEmplistPop.DataSource = null;
                gvEmplistPop.DataBind();
            }
        }        
    }

    protected void btnall_Click(object sender, EventArgs e)
    {
        string status_change_confirm_value = Request.Form["Toggle_activate_deactivate"];
        if (status_change_confirm_value == "Yes" || status_change_confirm_value == "null" || status_change_confirm_value == null)
        {

        }
        else
        {
            return;
        }

        if (ddlReseller.SelectedValue != "")
        {
            var type = Request.QueryString["type"].ToString();
            var empid = Request.QueryString["empid"].ToString();
            var seletbuss = ddlReseller.SelectedValue;
            DataTable dt = supportAdminCtrl.GetEmplist_SuppAdmin(type, int.Parse(empid), int.Parse(seletbuss));

            if (btnall.Text == "Deactive Subscriber")
            {
                comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + ddlReseller.SelectedValue + "'");
                //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + ddlReseller.SelectedValue + "'");
                btnall.Text = "Active Subscriber";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                        //BusinessOwnerId='2' to deactivate the employee,manager,administrator,accountant,custom role user.
                        comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='2' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                    }
                }

                DataTable dtSubAllBuss = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID='" + ddlReseller.SelectedValue + "' and UserTypeId=1", "");
                foreach (DataRow rowBuss in dtSubAllBuss.Rows)
                {
                    if (rowBuss["BusinessID"].ToString() != string.Empty)
                    {
                        comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                    }
                }
            }
            else
            {
                comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + ddlReseller.SelectedValue + "'");
                //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + ddlReseller.SelectedValue + "'");
                btnall.Text = "Deactive Subscriber";
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID=" + dt.Rows[i]["EmployeeID"].ToString() + "");
                        //BusinessOwnerId='1' to activate the employee,manager,administrator,accountant,custom role user.
                        comm.ExecuteQuery("update AddEmployee set BusinessOwnerId='1' where EmployeeID='" + dt.Rows[i]["EmployeeID"].ToString() + "'");
                    }
                }

                DataTable dtSubAllBuss = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID='" + ddlReseller.SelectedValue + "' and UserTypeId=1", "");
                foreach (DataRow rowBuss in dtSubAllBuss.Rows)
                {
                    if (rowBuss["BusinessID"].ToString() != string.Empty)
                    {
                        comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID=" + rowBuss["BusinessID"].ToString() + "");
                    }
                }
            }

            DataTable dtNew = supportAdminCtrl.GetEmplist_SuppAdmin(type, int.Parse(empid), int.Parse(seletbuss));
            if (dtNew.Rows.Count > 0)
            {
                gvEmplistPop.DataSource = null;
                gvEmplistPop.DataSource = dtNew;
                gvEmplistPop.DataBind();
            }
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Status Changed ', 'success');", true);
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Select Subscriber to Activate/Deactivate', 'warning');", true);
        }
    }
}