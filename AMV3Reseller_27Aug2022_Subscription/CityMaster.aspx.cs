using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class CityMaster : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    AddressMasterController AddMastCtrl = new AddressMasterController();
    AddressMasterEntity AddMastEntity = new AddressMasterEntity();
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode.Text.Trim().Length<3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter atlest 3 Letter to search', 'error');", true);
            return;
        }
        DataTable dt = new DataTable();
        resultdiv.Visible = true;
        dt = Comm.GetAllDataFromTable("CityMaster", "CityName,CityId", "CityName like '%"+ txtSearchPostalCode.Text + "%' and Duplicate='False'", "CityId");
        DataRow row1 = dt.NewRow();
        row1["CityId"] = 0;
        row1["CityName"] = " ADD NEW CITY"; //"None of the Above";
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
        if (StartDate.Text.Trim() != "ADD NEW CITY")
        {
            DataTable dt = new DataTable();
            dt = Comm.GetAllDataFromTable("CityMaster", "*", "CityId='" + hfdid.Value + "'", "CityId");

            txtStreetName.Text =dt.Rows[0]["CityName"].ToString() ;
            hfdStreetName.Value = dt.Rows[0]["CityName"].ToString().Trim();
            hfdAddressMasterId.Value = dt.Rows[0]["CityId"].ToString(); 
            if(dt.Rows[0]["Duplicate"].ToString()=="True")
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
        if(hfdAddressMasterId.Value==string.Empty)
        {
            if (chkDup.Checked == false)
            {
                bool chk = Comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtStreetName.Text + "' and Duplicate='False'");
                if (chk == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtStreetName.Text + " is aready exists ', 'error');", true);
                    return;
                }
            }
            AddMastEntity.CityName = txtStreetName.Text.ToUpper();
            if(chkDup.Checked==true)
            {
                AddMastEntity.Duplicate = true;
            }
            else
            {
                AddMastEntity.Duplicate = false;
            }
            int result=AddMastCtrl.Sp_InsertCity(AddMastEntity);
            Clear();
            if (result>0)
            {
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'New City Saved', 'success');", true);
                return;
            }
        }
        else
        {
            AddMastEntity.CityId = int.Parse(hfdAddressMasterId.Value);
            AddMastEntity.CityName = txtStreetName.Text.ToUpper();
            if (chkDup.Checked == true)
            {
                AddMastEntity.Duplicate = true;
            }
            else
            {
                AddMastEntity.Duplicate = false;
            }
            int result = AddMastCtrl.Sp_UpdateCity(AddMastEntity);
            Comm.ExecuteQuery("update EmpAddress set City='" + txtStreetName.Text + "' where City='" + hfdStreetName.Value + "'");
            Comm.ExecuteQuery("update BusinessAddress set City='" + txtStreetName.Text + "' where City='" + hfdStreetName.Value + "'");

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