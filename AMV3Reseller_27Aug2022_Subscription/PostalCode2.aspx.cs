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

public partial class PostalCode2 : System.Web.UI.Page
{
    SearchAddressController objSA = new SearchAddressController();

    CommanClass Comm = new CommanClass();
    AddressMasterController AddMasterCtrl = new AddressMasterController();
    AddressMasterEntity AddMasterEntity = new AddressMasterEntity();

    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            Get_New_Account_Business_Owner(); //21-5-22
            Comm.FillDropDownList(txtCity, "CityMaster", "CityId", "CityName", "Duplicate='False'", "CityId");
            Comm.FillDropDownList(txtProvince, "ProvinceMaster", "ProvinceId", "ProvinceName", "Duplicate='False'", "ProvinceId");
        }
    }

    //21-5-22
    public void Get_New_Account_Business_Owner()
    {
        try
        {




            DataTable DataTable_Subscriber_Reseller_List = objSA.GetSearchData(txtstartdate.Text, txtenddate.Text);
            GridView_New_Registered_BO.DataSource = DataTable_Subscriber_Reseller_List;
            GridView_New_Registered_BO.DataBind();

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
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Address Has been changed', 'success');", true);
        
        

    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Postalcode.Text = "";
        City.Text = "";
        StreetName.Text = "";
        Province.Text = "";
        LinkButton btn = sender as LinkButton;
        int id = int.Parse(btn.CommandArgument);
        Postalcode.Text = (GridView_New_Registered_BO.Rows[id].FindControl("Postalcode") as Label).Text;
        City.Text = (GridView_New_Registered_BO.Rows[id].FindControl("City") as Label).Text;
        StreetName.Text = (GridView_New_Registered_BO.Rows[id].FindControl("StreetName") as Label).Text;
        Province.Text = (GridView_New_Registered_BO.Rows[id].FindControl("Province") as Label).Text;
        chkFinal.Checked = (GridView_New_Registered_BO.Rows[id].FindControl("ChkCheck") as CheckBox).Checked;
        ModalPopupEmpList.Show();
    }
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        if (chkFinal.Checked && Postalcode.Text.Trim() != "" && StreetName.Text.Trim() != "" && City.Text.Trim() != "" && Province.Text.Trim() != "")
        {
            int rw = objSA.InsertAddressDetails(Postalcode.Text, StreetName.Text, City.Text, Province.Text);
            if (rw > 0)
            {
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
            ModalPopupEmpList.Show();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please fill manadatory field', 'success');", true);

        }
    }
    protected void GridView_New_Registered_BO_PreRender(object sender, EventArgs e)
    {
        if (GridView_New_Registered_BO.Rows.Count > 0)
        {
            GridView_New_Registered_BO.UseAccessibleHeader = true;
            GridView_New_Registered_BO.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    //end
    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        Clear();
        hfdAddressMasterId.Value = "";
        if (txtSearchPostalCode.Text == "")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        if (_serachpostcode.Length.ToString() != "6")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            txtSearchPostalCode.Focus();
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
        }
        else
        {            
            txtPostalCode.Text = "";            
            txtStreetName.Text = "";
            hfdAddressMasterId.Value = "";          
           
            txtCity.Text = "";
            txtProvince.Text = "";
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
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Please Verified Address', 'error');", true);
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
            if (result > 0)
            {
                populateaddress();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Updated', 'success');", true);
            }
        }
    }
}