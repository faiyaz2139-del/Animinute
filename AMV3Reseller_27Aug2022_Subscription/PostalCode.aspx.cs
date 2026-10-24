using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//21-05-22
using System.Configuration;
using log4net;
using System.Data.SqlClient;

public partial class PostalCode : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    AddressMasterController AddMasterCtrl = new AddressMasterController();
    AddressMasterEntity AddMasterEntity = new AddressMasterEntity();

    SearchAddressController objSA = new SearchAddressController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
            Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");

            Get_New_Account_Business_Owner(); //21-5-22
        }
    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        Clear();
        hfdAddressMasterId.Value = "";
        if (txtSearchPostalCode.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
            txtSearchPostalCode.Focus();
            resultdiv.Visible = false;
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        int search_postalcode_length = int.Parse(_serachpostcode.Length.ToString());
        //if (_serachpostcode.Length.ToString() != "6")
        if (search_postalcode_length < 5)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter at least 5 digits or letters postal code to search', 'error');", true);
            txtSearchPostalCode.Focus();
            resultdiv.Visible = false;
            return;
        }        
        populateaddress();
        resultdiv.Visible = true;        
    }

    public void populateaddress()
    {
        string strsearch = txtSearchPostalCode.Text;
        strsearch = strsearch.Replace(" ", String.Empty);
        DataTable dtSearchAddress = Comm.GetSearchPostalCode(strsearch);
        DataRow row1 = dtSearchAddress.NewRow();
        row1["id"] = 0;
        row1["SEARCHADDRESS"] = " ADD NEW ADDRESS"; //"None of the Above";
        dtSearchAddress.Rows.Add(row1);
        gvAddressList.DataSource = dtSearchAddress;
        gvAddressList.DataBind();
    }

    protected void StartDate_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("StartDate"); //
        HiddenField hfdid = (HiddenField)gvAddressList.Rows[gvr.RowIndex].FindControl("hfdid");
        divAddres.Attributes.Add("style", "display:block;");
        if (StartDate.Text != " ADD NEW ADDRESS")
        {
            String[] Split = StartDate.Text.Split(',');
            txtPostalCode.Text = txtSearchPostalCode.Text;
            hfdAddressMasterId.Value = hfdid.Value;
            DataTable dt = Comm.GetAllDataFromTable("AddressMaster", "*", "AddressMasterId=" + hfdid.Value + "","");
            txtPostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            if(dt.Rows[0]["Verifed"].ToString()=="1")
            {
                chkbxVerifed.Checked = true;
            }
            txtStreetName.Text = dt.Rows[0]["Street_Name"].ToString().ToUpper();
            txtStreetName.Text = dt.Rows[0]["Street_Name"].ToString().ToUpper();
           
            txtStreetName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            txtCity.SelectedValue = dt.Rows[0]["City"].ToString().ToUpper();
            txtProvince.Text = dt.Rows[0]["Province"].ToString().ToUpper();
            chkbxVerifed.Checked = true;
        }
        else
        {            
            txtPostalCode.Text = "";            
            txtStreetName.Text = "";
            hfdAddressMasterId.Value = "";

            Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
            Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
            chkbxVerifed.Checked = false;
            txtPostalCode.Text = txtSearchPostalCode.Text;
        }
    }

    public void Clear()
    {
        txtPostalCode.Text = "";
        txtStreetName.Text = "";
        hfdAddressMasterId.Value = "";
        txtCity.SelectedValue = "";
        txtProvince.SelectedValue = "";
        chkbxVerifed.Checked = false;
        txtPostalCode.Text = "";
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {        
        if(hfdAddressMasterId.Value=="")
        {
            if (chkbxVerifed.Checked != true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Please Check Verified Checkbox', 'error');", true);
                chkbxVerifed.Focus();
                return;
            }
            AddMasterEntity.City = txtCity.Text.Trim();
            AddMasterEntity.Street_Name = txtStreetName.Text.Trim();
            AddMasterEntity.Province = txtProvince.Text.Trim();
            AddMasterEntity.PostalCode = txtPostalCode.Text.Replace(" ", string.Empty).Trim(); 
            if(chkbxVerifed.Checked==true)
            {
                AddMasterEntity.Verifed = 1;
            }
            else
            {
                AddMasterEntity.Verifed = 2;
            }
            int result = AddMasterCtrl.Sp_InsertAddressMaster(AddMasterEntity);
            Clear();
            //added by kumar to remove confusion for user
            txtSearchPostalCode.Text = "";
            resultdiv.Visible = false;
            divAddres.Attributes.Add("style", "display:none;");
            if (result>0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Saved', 'success');", true);
            }
        }
        else
        {
            AddMasterEntity.AddressMasterId = int.Parse(hfdAddressMasterId.Value);
            AddMasterEntity.City = txtCity.Text.Trim();
            AddMasterEntity.Street_Name = txtStreetName.Text.Trim();
            AddMasterEntity.Province = txtProvince.Text.Trim();
            AddMasterEntity.PostalCode = txtPostalCode.Text.Replace(" ", string.Empty).Trim();
            if (chkbxVerifed.Checked == true)
            {
                AddMasterEntity.Verifed = 1;
            }
            else
            {
                AddMasterEntity.Verifed = 2;
            }
            int result = AddMasterCtrl.Sp_UpdateAddressMaster(AddMasterEntity);
            Clear();
            //added by kumar to remove confusion for user
            txtSearchPostalCode.Text = "";
            resultdiv.Visible = false;
            divAddres.Attributes.Add("style", "display:none;");
            if (result > 0)
            {
                populateaddress();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Updated', 'success');", true);
            }
        }
    }

    //21-5-22
    public void Get_New_Account_Business_Owner()
    {
        try
        {
            DataTable DataTable_Subscriber_Reseller_List = objSA.GetSearchData(txtstartdate.Text, txtenddate.Text);
            GridView_PostalCode.DataSource = DataTable_Subscriber_Reseller_List;
            GridView_PostalCode.DataBind();
        }
        catch (Exception ex)
        {

        }
    }

    private SqlConnection con;

    public void getconnection()
    {
        string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        con = new SqlConnection(strcon);
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Get_New_Account_Business_Owner();
    }
    protected void btndSave_Click(object sender, EventArgs e)
    {
        LinkButton btn = sender as LinkButton;
        int id = int.Parse(btn.CommandArgument);
        CheckBox newchk = new CheckBox();
        newchk.Checked = (GridView_PostalCode.Rows[id].FindControl("chkCheck") as CheckBox).Checked;
        if (!newchk.Checked)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Please check the checkbox in the row', 'error');", true);
            return;
        }
        newchk.Checked = false;
        //LinkButton btn = sender as LinkButton;
        //int id = int.Parse(btn.CommandArgument);
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Address Has Been Verified successfully', 'success');", true);

        CheckBox chkCheck = (GridView_PostalCode.Rows[id].FindControl("chkCheck") as CheckBox);
        chkCheck.Checked = false;
    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Label_Checkbox_Warning.Text = "";
        Label_All_Warning.Text = "";
        TextBox_Postalcode.Text = "";
        TextBox_Postalcode.ReadOnly = true;
        City.Text = "";
        StreetName.Text = "";
        Province.Text = "";
        LinkButton btn = sender as LinkButton;
        int id = int.Parse(btn.CommandArgument);
        TextBox_Postalcode.Text = (GridView_PostalCode.Rows[id].FindControl("Postalcode") as Label).Text;
        City.Text = (GridView_PostalCode.Rows[id].FindControl("City") as Label).Text;
        StreetName.Text = (GridView_PostalCode.Rows[id].FindControl("StreetName") as Label).Text;
        Province.Text = (GridView_PostalCode.Rows[id].FindControl("Province") as Label).Text;
        chkFinal.Checked = (GridView_PostalCode.Rows[id].FindControl("chkCheck") as CheckBox).Checked;
        ModalPopupEmpList.Show();
    }
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        if (!chkFinal.Checked)
        {
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Please check the checkbox in the Pop Up', 'error');", true);
            Label_Checkbox_Warning.Text = "Please Check the Confirm checkbox";
            ModalPopupEmpList.Show();
            return;
        }
        if (chkFinal.Checked)
        {
            Label_Checkbox_Warning.Text = "";
        }

        if (chkFinal.Checked && TextBox_Postalcode.Text.Trim() != "" && StreetName.Text.Trim() != "" && City.Text.Trim() != "" && Province.Text.Trim() != "")
        {
            int rw = objSA.InsertAddressDetails(TextBox_Postalcode.Text, StreetName.Text, City.Text, Province.Text);
            if (rw > 0)
            {
                Label_Checkbox_Warning.Text = "";
                Label_All_Warning.Text = "";
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Address Has been changed', 'success');", true);
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Some error found while updating', 'error');", true);
            }
            Get_New_Account_Business_Owner();
        }
        else
        {
            Label_All_Warning.Text = "Please fill all fields";
            ModalPopupEmpList.Show();
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please fill manadatory fields', 'success');", true);
        }
    }
    protected void GridView_PostalCode_PreRender(object sender, EventArgs e)
    {
        if (GridView_PostalCode.Rows.Count > 0)
        {
            GridView_PostalCode.UseAccessibleHeader = true;
            GridView_PostalCode.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    //end


    //city master page code starts here
    protected void lnkSearch_city_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode_city.Text.Trim().Length < 3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter atlest 3 Letter to search', 'error');", true);
            return;
        }
        DataTable dt = new DataTable();
        resultdiv_city.Visible = true;
        dt = Comm.GetAllDataFromTable("CityMaster", "CityName,CityId", "CityName like '%" + txtSearchPostalCode_city.Text + "%' and Duplicate='False'", "CityId");
        DataRow row1 = dt.NewRow();
        row1["CityId"] = 0;
        row1["CityName"] = " ADD NEW CITY"; //"None of the Above";
        dt.Rows.Add(row1);
        gvAddressList_city.DataSource = dt;
        gvAddressList_city.DataBind();
        gvAddressList_city.Visible = true;
    }

    protected void StartDate_city_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList_city.Rows[gvr.RowIndex].FindControl("StartDate_city"); //
        HiddenField hfdid = (HiddenField)gvAddressList_city.Rows[gvr.RowIndex].FindControl("hfdid_city");
        divAddres_city.Attributes.Add("style", "display:block;");
        if (StartDate.Text.Trim() != "ADD NEW CITY")
        {
            DataTable dt = new DataTable();
            dt = Comm.GetAllDataFromTable("CityMaster", "*", "CityId='" + hfdid.Value + "'", "CityId");

            txtStreetName_city.Text = dt.Rows[0]["CityName"].ToString();
            hfdStreetName.Value = dt.Rows[0]["CityName"].ToString().Trim();
            hfdAddressMasterId_city.Value = dt.Rows[0]["CityId"].ToString();
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
            txtStreetName_city.Text = string.Empty;
            hfdAddressMasterId_city.Value = string.Empty;
        }
    }

    protected void txtSave_Click(object sender, EventArgs e)
    {
        if (txtStreetName_city.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter city', 'error');", true);
            return;
        }
        if (hfdAddressMasterId_city.Value == string.Empty)
        {
            if (chkDup.Checked == false)
            {
                bool chk = Comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtStreetName_city.Text + "' and Duplicate='False'");
                if (chk == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtStreetName_city.Text + " is aready exists ', 'error');", true);
                    return;
                }
            }
            AddMasterEntity.CityName = txtStreetName_city.Text.ToUpper();
            if (chkDup.Checked == true)
            {
                AddMasterEntity.Duplicate = true;
            }
            else
            {
                AddMasterEntity.Duplicate = false;
            }
            int result = AddMasterCtrl.Sp_InsertCity(AddMasterEntity);
            Clear_city();
            if (result > 0)
            {
                Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
                Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'New City Saved', 'success');", true);
                return;
            }
        }
        else
        {
            AddMasterEntity.CityId = int.Parse(hfdAddressMasterId_city.Value);
            AddMasterEntity.CityName = txtStreetName_city.Text.ToUpper();
            if (chkDup.Checked == true)
            {
                AddMasterEntity.Duplicate = true;
            }
            else
            {
                AddMasterEntity.Duplicate = false;
            }
            int result = AddMasterCtrl.Sp_UpdateCity(AddMasterEntity);
            Comm.ExecuteQuery("update EmpAddress set City='" + txtStreetName_city.Text + "' where City='" + hfdStreetName.Value + "'");
            Comm.ExecuteQuery("update BusinessAddress set City='" + txtStreetName_city.Text + "' where City='" + hfdStreetName.Value + "'");

            Clear_city();
            if (result > 0)
            {
                Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
                Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'City Updated', 'success');", true);
                return;
            }
        }
        Clear_city();
    }
    public void Clear_city()
    {
        txtSearchPostalCode_city.Text = "";
        txtStreetName_city.Text = "";
        chkDup.Checked = false;
        gvAddressList_city.Visible = false;
        //added by kumar to remove confusion for user
        resultdiv_city.Visible = false;
        divAddres_city.Attributes.Add("style", "display:none;");
    }





    //province master code starts here
    protected void lnkSearch_province_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode_province.Text.Trim().Length < 3)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter atlest 3 Letter to search', 'error');", true);
            return;
        }
        resultdiv_province.Visible = true;
        DataTable dt = Comm.GetAllDataFromTable("ProvinceMaster", "ProvinceName,ProvinceId", "ProvinceName like '%" + txtSearchPostalCode_province.Text + "%' and Duplicate='False'", "ProvinceId");
        DataRow row1 = dt.NewRow();
        row1["ProvinceId"] = 0;
        row1["ProvinceName"] = " ADD NEW PROVINCE"; //"None of the Above";
        dt.Rows.Add(row1);
        gvAddressList_province.DataSource = dt;
        gvAddressList_province.DataBind();
        gvAddressList_province.Visible = true;
    }

    protected void StartDate_province_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList_province.Rows[gvr.RowIndex].FindControl("StartDate_province"); //
        HiddenField hfdid = (HiddenField)gvAddressList_province.Rows[gvr.RowIndex].FindControl("hfdid_province");
        divAddres_province.Attributes.Add("style", "display:block;");
        if (StartDate.Text.Trim() != "ADD NEW PROVINCE")
        {
            DataTable dt = new DataTable();
            dt = Comm.GetAllDataFromTable("ProvinceMaster", "*", "ProvinceId='" + hfdid.Value + "'", "ProvinceId");

            txtStreetName_province.Text = dt.Rows[0]["ProvinceName"].ToString();
            hfdAddressMasterId_province.Value = dt.Rows[0]["ProvinceId"].ToString();
            hfdStreetName_province.Value = dt.Rows[0]["ProvinceName"].ToString();
            if (dt.Rows[0]["Duplicate"].ToString() == "True")
            {
                chkDup_province.Checked = true;
            }
            else
            {
                chkDup_province.Checked = false;
            }
        }
        else
        {
            txtStreetName_province.Text = string.Empty;
            hfdAddressMasterId_province.Value = string.Empty;
        }
    }

    protected void txtSave_province_Click(object sender, EventArgs e)
    {
        if (txtStreetName_province.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter province/state', 'error');", true);
            return;
        }
        if (hfdAddressMasterId_province.Value == string.Empty)
        {
            if (chkDup_province.Checked == false)
            {
                bool chk = Comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtStreetName_province.Text + "' and Duplicate='False'");
                if (chk == true)
                {
                    ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtStreetName_province.Text + " is aready exists ', 'error');", true);
                    return;
                }
            }
            AddMasterEntity.ProvinceName = txtStreetName_province.Text.ToUpper();
            if (chkDup_province.Checked == true)
            {
                AddMasterEntity.DuplicateProvince = true;
            }
            else
            {
                AddMasterEntity.DuplicateProvince = false;
            }
            int result = AddMasterCtrl.Sp_InsertProvince(AddMasterEntity);
            Clear_province();
            if (result > 0)
            {
                Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
                Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'New Province Saved', 'success');", true);
                return;
            }
        }
        else
        {
            AddMasterEntity.ProvinceId = int.Parse(hfdAddressMasterId_province.Value);
            AddMasterEntity.ProvinceName = txtStreetName_province.Text.ToUpper();
            if (chkDup_province.Checked == true)
            {
                AddMasterEntity.DuplicateProvince = true;
            }
            else
            {
                AddMasterEntity.DuplicateProvince = false;
            }
            int result = AddMasterCtrl.Sp_UpdateProvince(AddMasterEntity);
            Comm.ExecuteQuery("update EmpAddress set Province='" + txtStreetName_province.Text + "' where Province='" + hfdStreetName_province.Value + "'");
            Comm.ExecuteQuery("update BusinessAddress set Province='" + txtStreetName_province.Text + "' where Province='" + hfdStreetName_province.Value + "'");
            Clear_province();
            if (result > 0)
            {
                Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
                Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
                ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('success!', 'City Updated', 'success');", true);
                return;
            }
        }
        Clear_province();
    }

    public void Clear_province()
    {
        txtSearchPostalCode_province.Text = "";
        txtStreetName_province.Text = "";
        chkDup_province.Checked = false;
        gvAddressList_province.Visible = false;
        //added by kumar to remove confusion for user
        resultdiv_province.Visible = false;
        divAddres_province.Attributes.Add("style", "display:none;");
    }
}