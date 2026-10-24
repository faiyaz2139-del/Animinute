using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class ProvinceMaster : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    AddressMasterController AddMastCtrl = new AddressMasterController();
    AddressMasterEntity AddMastEntity = new AddressMasterEntity();

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode.Text.Trim().Length < 3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter atlest 3 Letter to search', 'error');", true);
            return;
        }
        resultdiv.Visible = true;
        DataTable dt = Comm.GetAllDataFromTable("ProvinceMaster", "ProvinceName,ProvinceId", "ProvinceName like '%" + txtSearchPostalCode.Text + "%' and Duplicate='False'", "ProvinceId");
        DataRow row1 = dt.NewRow();
        row1["ProvinceId"] = 0;
        row1["ProvinceName"] = " ADD NEW PROVINCE"; //"None of the Above";
        dt.Rows.Add(row1);
        gvAddressList.DataSource = dt;
        gvAddressList.DataBind();
        gvAddressList.Visible = true;
    }

    protected void StartDate_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("StartDate"); //
        HiddenField hfdid = (HiddenField)gvAddressList.Rows[gvr.RowIndex].FindControl("hfdid");
        divAddres.Attributes.Add("style", "display:block;");
        if (StartDate.Text.Trim() != "ADD NEW PROVINCE")
        {
            DataTable dt = new DataTable();
            dt = Comm.GetAllDataFromTable("ProvinceMaster", "*", "ProvinceId='" + hfdid.Value + "'", "ProvinceId");

            txtStreetName.Text = dt.Rows[0]["ProvinceName"].ToString();
            hfdAddressMasterId.Value = dt.Rows[0]["ProvinceId"].ToString();
            hfdStreetName.Value = dt.Rows[0]["ProvinceName"].ToString();
            if (dt.Rows[0]["Duplicate"].ToString() == "True")
            {
                chkDup.Checked = true;
            }
            else
            {
                chkDup.Checked = false;
            }
        }
        else
        {
            txtStreetName.Text = string.Empty;
            hfdAddressMasterId.Value = string.Empty;
        }
    }

    protected void txtSave_Click(object sender, EventArgs e)
    {
        if (hfdAddressMasterId.Value == string.Empty)
        {
            if (chkDup.Checked == false)
            {
                bool chk = Comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtStreetName.Text + "' and Duplicate='False'");
                if (chk == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtStreetName.Text + " is aready exists ', 'error');", true);
                    return;
                }
            }
            AddMastEntity.ProvinceName = txtStreetName.Text.ToUpper();
            if (chkDup.Checked == true)
            {
                AddMastEntity.DuplicateProvince = true;
            }
            else
            {
                AddMastEntity.DuplicateProvince = false;
            }
            int result = AddMastCtrl.Sp_InsertProvince(AddMastEntity);
            Clear();
            if (result > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'New Province Saved', 'success');", true);
                return;
            }
        }
        else
        {
            AddMastEntity.ProvinceId = int.Parse(hfdAddressMasterId.Value);
            AddMastEntity.ProvinceName = txtStreetName.Text.ToUpper();
            if (chkDup.Checked == true)
            {
                AddMastEntity.DuplicateProvince = true;
            }
            else
            {
                AddMastEntity.DuplicateProvince = false;
            }
            int result = AddMastCtrl.Sp_UpdateProvince(AddMastEntity);
            Comm.ExecuteQuery("update EmpAddress set Province='" + txtStreetName.Text + "' where Province='" + hfdStreetName.Value + "'");
            Comm.ExecuteQuery("update BusinessAddress set Province='" + txtStreetName.Text + "' where Province='" + hfdStreetName.Value + "'");
            Clear();
            if (result > 0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'City Updated', 'success');", true);
                return;
            }
        }
        Clear();
    }

    public void Clear()
    {
        txtSearchPostalCode.Text = "";
        txtStreetName.Text = "";
        chkDup.Checked = false;
        gvAddressList.Visible = false;
    }
}