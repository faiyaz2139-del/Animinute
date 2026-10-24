using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class AdminUserType : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            Comm.FillGridView(GvDivAdminBussOwner, "AdminUserType", "AdminUserId", "AdminUserType", "", "");
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
    

    protected void lblTitledivAdmin_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdAdminUserId = (HiddenField)GvDivAdminBussOwner.Rows[gvr.RowIndex].FindControl("hfdAdminUserId");
        LinkButton lblTitledivAdmin = (LinkButton)GvDivAdminBussOwner.Rows[gvr.RowIndex].FindControl("lblTitledivAdmin");
        hfdEditId.Value = hfdAdminUserId.Value;
        txtUserType.Text = lblTitledivAdmin.Text;
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        bool chk = Comm.IfExistsInTable("AdminUserType", "*", "AdminUserType='" + txtUserType.Text + "'");
        if(chk==true)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', '"+ txtUserType.Text + " Already Exists ', 'warning');", true);
            return;
        }
        int result = 0;
        if (hfdEditId.Value==string.Empty)
        {
            result = Comm.ExecuteQuery("Insert into AdminUserType (AdminUserType)values('" + txtUserType.Text + "')");
        }
        else
        {
            result = Comm.ExecuteQuery("Update AdminUserType set AdminUserType='" + txtUserType.Text + "' where AdminUserId="+ hfdEditId.Value +"");
        }
         



        Clear();
        Comm.FillGridView(GvDivAdminBussOwner, "AdminUserType", "AdminUserId", "AdminUserType", "", "");
        if (result>0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved', 'success');", true);
            return;
        }
    }
    public void Clear()
    {
        hfdEditId.Value = string.Empty;
        txtUserType.Text = string.Empty;
    }


}