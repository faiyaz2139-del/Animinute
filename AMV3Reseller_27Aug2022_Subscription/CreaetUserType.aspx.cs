using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CreaetUserType : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    Cls_UserType CUserType = new Cls_UserType();
    int User_TypeId = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            GetUserTypeList();
        }
    }
    public void GetUserTypeList()
    {
        comm.FillGridView(GvUserTypeList, "UserType", "User_TypeId", "UserType", "", "");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {   
        if (btnSave.Text == "Save")
        {
            DataTable dtusertype = comm.GetAllDataFromTable("UserType", "UserType", "UserType='" + txtusetype.Text + "'", "");
            if (dtusertype.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'User Type already exists', 'warning');", true);
                return;
            }

            CUserType.InsertUserType(txtusetype.Text, "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved!', 'success');", true);
            clear();
        }
        else
        {
            CUserType.UpdateUserType(int.Parse(hdfeditid.Value), txtusetype.Text);
            btnSave.Text = "Save";
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Updated!', 'success');", true);
            clear();
        }
        GetUserTypeList();
    }

    protected void GvUserTypeList_PreRender(object sender, EventArgs e)
    {
        if (GvUserTypeList.Rows.Count > 0)
        {
            GvUserTypeList.UseAccessibleHeader = true;
            GvUserTypeList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnedit_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdf_User_TypeId = (HiddenField)GvUserTypeList.Rows[gvr.RowIndex].FindControl("hdf_User_TypeId");
        Label _lbl_UserType = (Label)GvUserTypeList.Rows[gvr.RowIndex].FindControl("lbl_UserType");
        txtusetype.Text = _lbl_UserType.Text;
        hdfeditid.Value = hdf_User_TypeId.Value;
        btnSave.Text = "Update";
    }

    protected void btnyes_Click(object sender, EventArgs e)
    {

        CUserType.DeleteUserType(int.Parse(hdfeditid.Value));
        GetUserTypeList();
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);
        clear();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string confirmValue = Request.Form["confirm_value"];
        if (confirmValue == "Yes")
        {
            LinkButton lnkbtn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
            HiddenField hdf_User_TypeId = (HiddenField)GvUserTypeList.Rows[gvr.RowIndex].FindControl("hdf_User_TypeId");
            Label _lbl_UserType = (Label)GvUserTypeList.Rows[gvr.RowIndex].FindControl("lbl_UserType");

            CUserType.DeleteUserType(int.Parse(hdf_User_TypeId.Value));
            GetUserTypeList();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);
            clear();
        }
    }

    public void clear()
    {
        txtusetype.Text = string.Empty;
        hdfeditid.Value = string.Empty;
        btnSave.Text = "Save";
    }
}