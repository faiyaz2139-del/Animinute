using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class EmployeeProfile : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddEmployeeEntityClass AddempEntity= new AddEmployeeEntityClass();
    EmpAddressEntity EmpAddress = new EmpAddressEntity();
    MyProfileController profile = new MyProfileController(); 
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!Page.IsPostBack)
        {
            if (Request.QueryString["ProfileEmployeeId"] != null)
            {
                string aa = Request.QueryString["ProfileEmployeeId"].ToString();
                GetProfileDetails(int.Parse(aa));
            }

            if (Request.QueryString["ProfileEmployeeId_ForAddressEdit"] != null)
            {
                string aa = Request.QueryString["ProfileEmployeeId_ForAddressEdit"].ToString();
                GetProfileDetails_ForAddressEdit(int.Parse(aa));
            }
        }            
    }

    public void GetProfileDetails(int ProfileEmployeeId)
    {
        DataTable dt = new DataTable();
        dt = comm.GetAllDataFromTable("AddEmployee ae join EmpAddress ea on ea.EmpAddressId=ae.ADDRESSID join RateDetails rd on rd.RateId=ae.Rateid", "*", "ae.EmployeeID=" + ProfileEmployeeId + "", "");
        txtLegalName.SelectedValue = dt.Rows[0]["Title"].ToString();
        txtFirstName.Text = dt.Rows[0]["FirstName"].ToString();

        txtMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dt.Rows[0]["LastName"].ToString();
        if (dt.Rows[0]["DateOfBirth"].ToString() != "")
        {
            string dbo = dt.Rows[0]["DateOfBirth"].ToString();
            if (string.IsNullOrEmpty(dbo) || dbo.Length % 4 != 0
       || dbo.Contains(" ") || dbo.Contains("\t") || dbo.Contains("\r") || dbo.Contains("\n"))
            {
                if (dbo != "")
                {
                    txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                string ddbo = comm.Decrypt(dt.Rows[0]["DateOfBirth"].ToString());
                if (ddbo != "")
                {
                    txtdob.Text = DateTime.Parse(ddbo).ToString("yyyy-MM-dd");
                }
            }
        }
            ddlgender.SelectedValue = dt.Rows[0]["Gender"].ToString();
            txtMainPhone.Text = dt.Rows[0]["MainPhone"].ToString();
            txtMobile.Text = dt.Rows[0]["Mobile"].ToString();
            txtOccupation.Text = dt.Rows[0]["Occupation"].ToString();

        if (dt.Rows[0]["SocialInsuranceNumber"].ToString() != "")
        {
            string sin = dt.Rows[0]["SocialInsuranceNumber"].ToString();
            if (string.IsNullOrEmpty(sin) || sin.Length % 4 != 0
       || sin.Contains(" ") || sin.Contains("\t") || sin.Contains("\r") || sin.Contains("\n"))
            {
                if (sin != "")
                {
                    txtSocialNumer.Text = sin;
                }
            }
            else
            {
                string ssin = comm.Decrypt(dt.Rows[0]["SocialInsuranceNumber"].ToString());
                if (ssin != "")
                {
                    txtSocialNumer.Text = ssin;
                }
            }
        }


        txtSINExpiry.Text = dt.Rows[0]["SINExpiry"].ToString();
        txtMainEmail.Text = dt.Rows[0]["MainEmail"].ToString();
        ddlEmpEI.SelectedValue = dt.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        ddlEmpCPP.SelectedValue = dt.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        ddlRTVA.SelectedValue = dt.Rows[0]["RetainVacation"].ToString().Trim();
        txtBillRate.Text = dt.Rows[0]["Rate"].ToString();
        ddlBillingType.SelectedValue = dt.Rows[0]["RateType"].ToString();
        txtConLoginEmail.Text = dt.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dt.Rows[0]["CCEmail"].ToString();
        txtHiredate.Text = DateTime.Parse(dt.Rows[0]["HireDate"].ToString()).ToString("yyyy-MM-dd");
        ddlempStatus.SelectedValue = dt.Rows[0]["IsActive"].ToString().Trim();
        if (ddlempStatus.SelectedValue == "NO")
        {
            if (dt.Rows[0]["Apartment"].ToString() != "")
                txtInActiveDate.Text = DateTime.Parse(dt.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        }
        divAddres.Attributes.Add("style", "display:block;");
        DataTable dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + dt.Rows[0]["ADDRESSID"].ToString() + "", "");

        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        ddlNumberSuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["NumberSuffix"].ToString();
        txtStreetName.Text = dtAddressDetailForEdit.Rows[0]["StreetName"].ToString().ToUpper();

        txtStreetType.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetType1.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetDirection.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();
        txtStreetDirection1.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();

        DropDownList1.SelectedValue = dtAddressDetailForEdit.Rows[0]["SuitType"].ToString();
        txtSuiteNumber.Text = dtAddressDetailForEdit.Rows[0]["SuiteNumber"].ToString();
        ddlsuitesuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["SuitSuffix"].ToString();
        txtCity.Text = dtAddressDetailForEdit.Rows[0]["City"].ToString().ToUpper();
        txtProvince.Text = dtAddressDetailForEdit.Rows[0]["Province"].ToString().ToUpper();
        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtWorkPhone.Text = dtAddressDetailForEdit.Rows[0]["WorkPhone"].ToString();
        txtMobile.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();
        txtFax.Text = dtAddressDetailForEdit.Rows[0]["Fax"].ToString();
        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        hfdCCEmail.Value = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        txtWebsite.Text = dtAddressDetailForEdit.Rows[0]["Website"].ToString();
        txtOther1.Text = dtAddressDetailForEdit.Rows[0]["Other1"].ToString();
        txtStreetNo.Text = dtAddressDetailForEdit.Rows[0]["StreetNo"].ToString();

        txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();        
    }

    protected void txtSearchPostalCode_TextChanged(object sender, EventArgs e)
    {

    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
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
        ClearAddress();
        if (txtSearchPostalCode.Text.Length.ToString() != "") //== "7")
        {
            string strsearch = txtSearchPostalCode.Text;
            strsearch = strsearch.Replace(" ", String.Empty);

            resultdiv.Visible = true;
            DataTable dtSearchAddress = new DataTable();
            rdobtnlist.Visible = true;
            //dtSearchAddress = comm.GetAllDataFromTable("EmpAddress", "EmpAddressId,StreetName+', '+ StreetType+', '+StreetDirection+', '+City+', '+Province as SEARCHADDRESS", "Postalcode='" + txtSearchPostalCode.Text + "'", "EmpAddressId");
            //dtSearchAddress = comm.GetSearchAddress(txtSearchPostalCode.Text.Trim());
            dtSearchAddress = comm.GetSearchAddress(strsearch);
            BindAddressChoices(dtSearchAddress); // v17: new "Select Address from List or Add New" dropdown
            DataRow row1 = dtSearchAddress.NewRow();
            row1["id"] = 0;
            row1["SEARCHADDRESS"] = " ADD NEW ADDRESS"; //"None of the Above";
            dtSearchAddress.Rows.Add(row1);
            gvAddressList.DataSource = dtSearchAddress;
            gvAddressList.DataBind();
            DataRow row = dtSearchAddress.NewRow();
            if (dtSearchAddress.Rows.Count == 0)
            {
                //rdobtnlist.ClearSelection();
                row["id"] = 0;
                row["SEARCHADDRESS"] = " Add New Address"; //"None of the Above";
                dtSearchAddress.Rows.Add(row);
                rdobtnlist.DataBind();
                rdobtnlist.DataSource = dtSearchAddress;
                rdobtnlist.DataTextField = "SEARCHADDRESS";
                rdobtnlist.DataValueField = "SEARCHADDRESS";
                rdobtnlist.DataBind();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Address not recorded previously', 'warning');", true);
                return;
            }
            row["id"] = 0;
            row["SEARCHADDRESS"] = " Add New Address";
            dtSearchAddress.Rows.Add(row);
            rdobtnlist.DataSource = dtSearchAddress;
            rdobtnlist.DataTextField = "SEARCHADDRESS";
            rdobtnlist.DataValueField = "SEARCHADDRESS";
            foreach (ListItem itm in rdobtnlist.Items)
            {
                for (int i = 0; i < dtSearchAddress.Rows.Count; i++)
                {
                    itm.Attributes["title"] = dtSearchAddress.Rows[i]["tab"].ToString();
                    itm.Text = dtSearchAddress.Rows[i]["SEARCHADDRESS"].ToString();
                    itm.Value = dtSearchAddress.Rows[i]["id"].ToString();
                }

            }
            rdobtnlist.DataBind();
        }
    }

    // 2026-10-26 v17 (Pandey image5): the Address and Contacts tab lists the postal-code matches in a dropdown
    // (plus "+ Add New Address"); choosing one fills the Employee Address fields, which btnSaveAddress saves as before.
    private void BindAddressChoices(DataTable matches)
    {
        DataTable copy = matches.Copy(); if (String.IsNullOrEmpty(copy.TableName)) copy.TableName = "ProfileMatches";
        ViewState["ProfileMatches"] = copy;
        ddlAddressChoices.Items.Clear();
        ddlAddressChoices.Items.Add(new ListItem(copy.Rows.Count > 0 ? "Select an address" : "No matching addresses found", ""));
        for (int i = 0; i < copy.Rows.Count; i++)
        {
            string label = Convert.ToString(copy.Rows[i]["SEARCHADDRESS"]).Trim();
            if (label.Length > 0 && !String.Equals(label, "ADD NEW ADDRESS", StringComparison.OrdinalIgnoreCase)) ddlAddressChoices.Items.Add(new ListItem(label, i.ToString()));
        }
        ddlAddressChoices.Items.Add(new ListItem("+ Add New Address", "new"));
    }

    protected void ddlAddressChoices_Changed(object sender, EventArgs e)
    {
        string postal = txtSearchPostalCode.Text.Trim().ToUpper();
        txtStreetNo.ReadOnly = txtStreetName.ReadOnly = txtStreetType.ReadOnly = txtCity.ReadOnly = txtProvince.ReadOnly = txtPostalCode.ReadOnly = false;
        if (ddlAddressChoices.SelectedValue == "new")
        {
            txtStreetNo.Text = txtStreetName.Text = txtStreetType.Text = txtStreetType1.Text = txtSuiteNumber.Text = txtCity.Text = txtProvince.Text = "";
            DropDownList1.SelectedValue = ""; txtPostalCode.Text = postal; txtStreetNo.Focus();
            return;
        }
        var matches = ViewState["ProfileMatches"] as DataTable; int i;
        if (matches == null || !Int32.TryParse(ddlAddressChoices.SelectedValue, out i) || i < 0 || i >= matches.Rows.Count) return;
        var a = AnyminuteBusinessAddressInput.FromRow(matches.Rows[i], postal);
        txtStreetNo.Text = a.Number; txtStreetName.Text = (a.Street ?? "").ToUpper(); txtStreetType.Text = (a.Line2 ?? "").ToUpper(); txtStreetType1.Text = txtStreetName.Text;
        txtCity.Text = (a.City ?? "").ToUpper(); txtProvince.Text = (a.Province ?? "").Trim().ToUpper();
        txtPostalCode.Text = postal.Length > 0 ? postal : a.PostalCode;
    }

    public void ClearAddress()
    {
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        DropDownList1.SelectedValue = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";

    }

    protected void StartDate_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("StartDate");
        divAddres.Attributes.Add("style", "display:block;");
        if (StartDate.Text != " ADD NEW ADDRESS")
        {
            String[] Split = StartDate.Text.Split(',');
            txtPostalCode.Text = txtSearchPostalCode.Text;//dtselectresult.Rows[0]["Postalcode"].ToString();
            txtPostalCode.ReadOnly = true;
            txtPostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            //txtSuiteNumber.Text = dtselectresult.Rows[0]["SuiteNumber"].ToString();

            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.ReadOnly = true;
            txtStreetName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            //txtStreetType.Text = Split[0].ToString();//dtselectresult.Rows[0]["StreetType"].ToString();
            txtStreetType1.Text = Split[0].ToString().ToUpper();
            // txtStreetType.ReadOnly = true;
            // txtStreetType.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");
            txtCity.Text = Split[1].ToString().ToUpper();//dtselectresult.Rows[0]["City"].ToString();
            txtCity.ReadOnly = true;
            txtProvince.Text = Split[2].ToString().ToUpper();//dtselectresult.Rows[0]["Province"].ToString();
            txtProvince.ReadOnly = true;
        }
        else
        {
            txtSuiteNumber.Text = "";
            txtStreetNo.Text = "";
            DropDownList1.SelectedValue = "";

            txtSuiteNumber.Text = "";
            txtPostalCode.Text = "";
            txtPostalCode.ReadOnly = false;
            txtStreetName.Text = "";
            txtStreetName.ReadOnly = false;
            txtStreetType.Text = "";
            txtStreetType1.Text = "";
            txtStreetType.ReadOnly = false;
            txtCity.Text = "";
            txtProvince.Text = "";
            txtCity.ReadOnly = false;
            txtProvince.ReadOnly = false;
            //ClearAddressContactDetails();
        }
    }

    protected void rdobtnlist_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    [WebMethod]
    public static List<string> GetAutoCompleteDataCity(string city)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("select distinct City,City from AddressMaster where City LIKE '%'+@SearchText+'%'", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", city);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}/{1}", dr["City"], dr["City"]));
                }
                return result;
            }
        }
    }


    [WebMethod]
    public static List<string> GetAutoCompleteDataProvince(string Province)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("select distinct Province,Province from AddressMaster where Province LIKE '%'+@SearchText+'%'", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", Province);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}/{1}", dr["Province"], dr["Province"]));
                }
                return result;
            }
        }
    }


    [WebMethod]
    public static List<string> GetAutoCompleteDataOccupation(string Occupation)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            using (SqlCommand cmd = new SqlCommand("select distinct Occupation,Occupation from AddEmployee where Occupation LIKE '%'+@SearchText+'%'", con))
            {
                con.Open();
                cmd.Parameters.AddWithValue("@SearchText", Occupation);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    result.Add(string.Format("{0}/{1}", dr["Occupation"], dr["Occupation"]));
                }
                return result;
            }
        }
    }

    protected void btnsavePresnol_Click(object sender, EventArgs e)
    {
        if (Request.QueryString["ProfileEmployeeId"] == null)
            AddempEntity.EmployeeID = int.Parse(Request.QueryString["ProfileEmployeeId_ForAddressEdit"].ToString());
        else
            AddempEntity.EmployeeID = int.Parse(Request.QueryString["ProfileEmployeeId"].ToString());
        AddempEntity.FirstName = txtFirstName.Text;
        AddempEntity.MiddleName = txtMiddleName.Text;
        AddempEntity.Title = txtLegalName.SelectedValue;
        AddempEntity.LastName = txtLastName.Text;
        AddempEntity.Gender = ddlgender.SelectedValue;
        AddempEntity.DateOfBirth = comm.encrypt(txtdob.Text);
        AddempEntity.Occupation = txtOccupation.Text;
        
        AddempEntity.SocialInsuranceNumber = txtSocialNumer.Text;
        AddempEntity.SINExpiry = txtSINExpiry.Text;

        EmpAddress.MainPhone = txtMainPhone.Text;
        EmpAddress.Mobile = txtMainPhone.Text;
        EmpAddress.City = txtCity.Text;
        EmpAddress.Province = txtProvince.Text;
        EmpAddress.MainEmail = txtMainEmail.Text;
        EmpAddress.CCEmail = txtCCEmail.Text;

        EmpAddress.StreetName = txtStreetName.Text;
        EmpAddress.StreetNo = txtStreetNo.Text;
        EmpAddress.StreetType = txtStreetType.Text;
        EmpAddress.SuiteNumber = txtSuiteNumber.Text;
        EmpAddress.SuitType = DropDownList1.SelectedValue;
        EmpAddress.Postalcode = txtPostalCode.Text;
        int result = 0;
        result = profile.UpdateEmployeeProfile(AddempEntity, EmpAddress);
        if (result > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Profile Updated Successfully', 'success');", true);
        }
    }

    protected void btnSaveAddress_Click(object sender, EventArgs e)
    {
        btnsavePresnol_Click(sender, e);
    }

    public void GetProfileDetails_ForAddressEdit(int ProfileEmployeeId)
    {
        DataTable dt = new DataTable();
        dt = comm.GetAllDataFromTable("AddEmployee ae join EmpAddress ea on ea.EmpAddressId=ae.ADDRESSID join RateDetails rd on rd.RateId=ae.Rateid", "*", "ae.EmployeeID=" + ProfileEmployeeId + "", "");
        txtLegalName.SelectedValue = dt.Rows[0]["Title"].ToString();
        txtFirstName.Text = dt.Rows[0]["FirstName"].ToString();
        

        txtMiddleName.Text = dt.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dt.Rows[0]["LastName"].ToString();
        if (dt.Rows[0]["DateOfBirth"].ToString() != "")
        {
            string dbo = dt.Rows[0]["DateOfBirth"].ToString();
            if (string.IsNullOrEmpty(dbo) || dbo.Length % 4 != 0
       || dbo.Contains(" ") || dbo.Contains("\t") || dbo.Contains("\r") || dbo.Contains("\n"))
            {
                if (dbo != "")
                {
                    txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
                }
            }
            else
            {
                string ddbo = comm.Decrypt(dt.Rows[0]["DateOfBirth"].ToString());
                if (ddbo != "")
                {
                    txtdob.Text = DateTime.Parse(ddbo).ToString("yyyy-MM-dd");
                }
            }
        }
        ddlgender.SelectedValue = dt.Rows[0]["Gender"].ToString(); ddlgender.Enabled = false;
        txtMainPhone.Text = dt.Rows[0]["MainPhone"].ToString();
        txtMobile.Text = dt.Rows[0]["Mobile"].ToString(); txtMobile.ReadOnly = true;
        txtOccupation.Text = dt.Rows[0]["Occupation"].ToString(); txtOccupation.ReadOnly = true;

        if (dt.Rows[0]["SocialInsuranceNumber"].ToString() != "")
        {
            string sin = dt.Rows[0]["SocialInsuranceNumber"].ToString();
            if (string.IsNullOrEmpty(sin) || sin.Length % 4 != 0
       || sin.Contains(" ") || sin.Contains("\t") || sin.Contains("\r") || sin.Contains("\n"))
            {
                if (sin != "")
                {
                    txtSocialNumer.Text = sin;
                }
            }
            else
            {
                string ssin = comm.Decrypt(dt.Rows[0]["SocialInsuranceNumber"].ToString());
                if (ssin != "")
                {
                    txtSocialNumer.Text = ssin;
                }
            }
        }

        txtSINExpiry.Text = dt.Rows[0]["SINExpiry"].ToString(); txtSINExpiry.ReadOnly = true;
        txtMainEmail.Text = dt.Rows[0]["MainEmail"].ToString();
        ddlEmpEI.SelectedValue = dt.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        ddlEmpCPP.SelectedValue = dt.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        ddlRTVA.SelectedValue = dt.Rows[0]["RetainVacation"].ToString().Trim();
        txtBillRate.Text = dt.Rows[0]["Rate"].ToString();
        ddlBillingType.SelectedValue = dt.Rows[0]["RateType"].ToString();
        txtConLoginEmail.Text = dt.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dt.Rows[0]["CCEmail"].ToString();
        txtHiredate.Text = DateTime.Parse(dt.Rows[0]["HireDate"].ToString()).ToString("yyyy-MM-dd");
        ddlempStatus.SelectedValue = dt.Rows[0]["IsActive"].ToString().Trim();
        if (ddlempStatus.SelectedValue == "NO")
        {
            if (dt.Rows[0]["Apartment"].ToString() != "")
                txtInActiveDate.Text = DateTime.Parse(dt.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        }
        divAddres.Attributes.Add("style", "display:block;");
        DataTable dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + dt.Rows[0]["ADDRESSID"].ToString() + "", "");

        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        ddlNumberSuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["NumberSuffix"].ToString();
        txtStreetName.Text = dtAddressDetailForEdit.Rows[0]["StreetName"].ToString().ToUpper();

        txtStreetType.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetType1.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtStreetDirection.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();
        txtStreetDirection1.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();

        DropDownList1.SelectedValue = dtAddressDetailForEdit.Rows[0]["SuitType"].ToString();
        txtSuiteNumber.Text = dtAddressDetailForEdit.Rows[0]["SuiteNumber"].ToString();
        ddlsuitesuffix.SelectedValue = dtAddressDetailForEdit.Rows[0]["SuitSuffix"].ToString();
        txtCity.Text = dtAddressDetailForEdit.Rows[0]["City"].ToString().ToUpper();
        txtProvince.Text = dtAddressDetailForEdit.Rows[0]["Province"].ToString().ToUpper();
        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtWorkPhone.Text = dtAddressDetailForEdit.Rows[0]["WorkPhone"].ToString();
        txtMobile.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();
        txtFax.Text = dtAddressDetailForEdit.Rows[0]["Fax"].ToString();
        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        hfdCCEmail.Value = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
        txtWebsite.Text = dtAddressDetailForEdit.Rows[0]["Website"].ToString();
        txtOther1.Text = dtAddressDetailForEdit.Rows[0]["Other1"].ToString();
        txtStreetNo.Text = dtAddressDetailForEdit.Rows[0]["StreetNo"].ToString();

        txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
    }
}