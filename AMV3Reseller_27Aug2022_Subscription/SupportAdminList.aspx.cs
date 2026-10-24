using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class SupportAdminList : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            GetAllSupportAdmin();
        }
    }
    public void GetAllSupportAdmin()
    {
        Comm.FillGridView(GVMother, "SupportAdmin", "Title+' '+FirstName+' '+MiddleName+' '+LastName as Name", "*", "", "SupportAdminId desc");
    }

    protected void togBtnForAdminBussOwner_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox togBtnForAdminBussOwner = (CheckBox)sender;
        GridViewRow gvr = (GridViewRow)togBtnForAdminBussOwner.NamingContainer;
        HiddenField hfdStatusForAdminBussOwner = (HiddenField)GVMother.Rows[gvr.RowIndex].FindControl("hfdStatusForAdminBussOwner");
        HiddenField hdID = (HiddenField)GVMother.Rows[gvr.RowIndex].FindControl("hdID");

        if (hfdStatusForAdminBussOwner.Value == "True")
        {
            Comm.ExecuteQuery("Update SupportAdmin set Status='False' where SupportAdminId=" + hdID.Value + "");
        }
        else
        {
            Comm.ExecuteQuery("Update SupportAdmin set Status='True' where SupportAdminId=" + hdID.Value + "");
        }

        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed !', 'success');", true);
    }

    protected void GVMother_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GVMother.PageIndex = e.NewPageIndex;
        GetAllSupportAdmin();
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
            ImageButton btnactive = (e.Row.FindControl("btnactive") as ImageButton);
            HiddenField hfdStatusForAdminBussOwner = (e.Row.FindControl("hfdStatusForAdminBussOwner") as HiddenField);
            if (hfdStatusForAdminBussOwner.Value == "True")
            {
                btnactive.ImageUrl = "~/images/active.png";
            }
            else
            {
                btnactive.ImageUrl = "~/images/deactive.png";
            }
        }
    }

    protected void GVMother_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName== "status")
        {
            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField hfdStatusForAdminBussOwner = (HiddenField)GVMother.Rows[index].FindControl("hfdStatusForAdminBussOwner");
            HiddenField hfdid = (HiddenField)GVMother.Rows[index].FindControl("hfdid");

            if (hfdStatusForAdminBussOwner.Value == "True")
            {
                Comm.ExecuteQuery("Update SupportAdmin set Status='False' where SupportAdminId=" + hfdid.Value + "");
            }
            else
            {
                Comm.ExecuteQuery("Update SupportAdmin set Status='True' where SupportAdminId=" + hfdid.Value + "");
            }
            GetAllSupportAdmin();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed !', 'success');", true);
        }
    }
}