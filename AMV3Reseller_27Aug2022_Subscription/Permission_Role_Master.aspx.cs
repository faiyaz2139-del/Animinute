using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


public partial class Permission_Role_Master : System.Web.UI.Page
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
        comm.FillGridView(GvUserTypeList, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status", "", "Role_Permission_Id asc");
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {   
        if (btnSave.Text == "Save")
        {
            DataTable dtusertype = comm.GetAllDataFromTable("Role_Permission_Master", "Role_Permission_Name", "Role_Permission_Name='" + txtusetype.Text + "'", "");
            if (dtusertype.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'User Role Permission Name Already Exists!', 'warning');", true);
                return;
            }
            comm.ExecuteQuery("insert into Role_Permission_Master (Role_Permission_Name, Status) Values ('" + txtusetype.Text + "', 'True')");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved!', 'success');", true);
            clear();
        }
        else
        {
            DataTable dtusertype = comm.GetAllDataFromTable("Role_Permission_Master", "Role_Permission_Name", "Role_Permission_Name='" + txtusetype.Text + "'", "");
            if (dtusertype.Rows.Count > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'User Role Permission Name Already Exists!', 'warning');", true);
                return;
            }
            comm.ExecuteQuery("update Role_Permission_Master set Role_Permission_Name = '" + txtusetype.Text + "' where [Role_Permission_Id] = '" + int.Parse(hdfeditid.Value) + "'");
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
        //CUserType.DeleteUserType(int.Parse(hdfeditid.Value));
        comm.ExecuteQuery("delete from Role_Permission_Master where [Role_Permission_Id] = '" + int.Parse(hdfeditid.Value) + "'");
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

            //CUserType.DeleteUserType(int.Parse(hdf_User_TypeId.Value));
            comm.ExecuteQuery("delete from Role_Permission_Master where [Role_Permission_Id] = '" + int.Parse(hdf_User_TypeId.Value) + "'");
            GetUserTypeList();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);
            clear();
            GetUserTypeList();
        }
    }

    public void clear()
    {
        txtusetype.Text = string.Empty;
        hdfeditid.Value = string.Empty;
        btnSave.Text = "Save";
    }
}