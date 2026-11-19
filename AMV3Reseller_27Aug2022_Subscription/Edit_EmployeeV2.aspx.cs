using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Add_EmployeeV2 : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    DataTable dtSelectedBuss = new DataTable();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddProjectController apc = new AddProjectController();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    RateDetailsController RateCtrl = new RateDetailsController();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();
    AssignProjectEntity ape = new AssignProjectEntity();
    AssgineManagerEntity asaMangEntity = new AssgineManagerEntity();
    AssgineManagerController asMangCtrl = new AssgineManagerController();
    Cls_UserType ut = new Cls_UserType();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Bindbusiness();
            InitialSelectBuss();
            //comm.FillDropDownList(ddlRoleNamepop, "UserType", "User_TypeId", "UserType", "", "User_TypeId");
            comm.FillDropDownList(ddlBillingType, "CodeType", "Code_Value", "Code_Type", "Code_Id in (1,2,3,4,5,6)", "Code_Id");
            if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
            {
                ViewState["bid"] = Request.QueryString["BusinessID"].ToString();
            }
            else
            {
                var dt = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Session["EmployeeId"].ToString() + " and BusinessID is not null and BusinessID<>''", "");
                ViewState["bid"] = dt.Rows[0][0].ToString();
            }
            BindAllUserType();

            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                txtMainEmail.ReadOnly = true;
                hfdEmpID.Value = Request.QueryString["EmployeeIDEdit"].ToString();
                LinkButton_EditEmployee.Visible = true;
                GetChangeRateRole(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                GetPersonalDetailsForEdit(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                Get_User_Role_Permission_While_Editing_Employee(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
                btnFinalSave.Visible = false;
                btnSaveNewRole.Visible = false;
                btnSendBuss.Visible = false;
                btnBackBuss.Visible = false;
            }
        }
    }
    public void InitialSelectBuss()
    {
        dtSelectedBuss.Columns.AddRange(new DataColumn[7] { new DataColumn("BusinessName"), new DataColumn("BusinessID"), new DataColumn("Role"), new DataColumn("Roleid"), new DataColumn("Projectid"), new DataColumn("ProjectType"), new DataColumn("Permission") });
        ViewState["SelectBuss"] = dtSelectedBuss;
        this.BindGrid();
    }
    protected void BindGrid()
    {
        GvSelectBuss.DataSource = (DataTable)ViewState["SelectBuss"];
        GvSelectBuss.DataBind();
    }
    public void Bindbusiness()
    {
        if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
        {
            var empid = Session["EmployeeId"].ToString();
            comm.FillGridView(gvAvBu, "AssginBussiness assb join AddBusiness ab on assb.BusinessID = ab.BusinessID", "ab.BusinessID", "assb.BusinessName", "EmployeeID=" + empid + "", "assb.BusinessID");
        }
    }
    public void GetUserPermissionRoleList(int uid)
    {
        if (uid != 0)
        {
            comm.FillGridView(GridView_User_Role_Permission, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status", "UserTypeId=" + uid + "", "Role_Permission_Id asc");
        }
        else
        {
            comm.FillGridView(GridView_User_Role_Permission_Pop, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status,UserTypeId", "", "Role_Permission_Id asc");
        }


    }
    protected void BindAllUserType()
    {

        rdoBtnUserTypeList.DataSource = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));//comm.GetAllDataFromTable("UserType", "UserType, User_TypeId", "User_TypeId != 1", "User_TypeId");
        rdoBtnUserTypeList.DataValueField = "User_TypeId";
        rdoBtnUserTypeList.DataTextField = "UserType";
        rdoBtnUserTypeList.DataBind();
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (Session["UserType"].ToString() != "Admin")
            {
                if (Request.QueryString["EmployeeIDEdit"] == null)
                {
                    if (itm.Value == "4")
                    {
                        //itm.Selected = true;
                    }
                }
            }
            else
            {
                if (itm.Value == "1")
                {
                    itm.Selected = true;
                }
            }

        }
        var Utpe = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
        ddlRoleNamepop.DataSource = Utpe;
        ddlRoleNamepop.DataValueField = Utpe.Columns[0].ToString();
        ddlRoleNamepop.DataTextField = Utpe.Columns[1].ToString();
        ddlRoleNamepop.DataBind();


        ddlCatagoryRole.Items.Add("Please Select");
        ddlCatagoryRole.SelectedItem.Value = "";
        ddlCatagoryRole.DataSource = Utpe;
        ddlCatagoryRole.DataValueField = Utpe.Columns[0].ToString();
        ddlCatagoryRole.DataTextField = Utpe.Columns[1].ToString();
        ddlCatagoryRole.DataBind();
        ddlCatagoryRole.SelectedIndex = 0;
        AnyminuteEmployeeEditor.OfferManagerChoices(rdoBtnUserTypeList, 0);
        AnyminuteEmployeeEditor.OfferManagerChoices(ddlRoleNamepop, 0);
        AnyminuteEmployeeEditor.OfferManagerChoices(ddlCatagoryRole, 0);

        //comm.FillDropDownList(ddlCatagoryRole, "UserType", "User_TypeId", "UserType", "User_TypeId != 1", "User_TypeId");
    }
    protected void btnSaveAddres_Click(object sender, EventArgs e)
    {

    }

    protected void txtSearchPostalCode_TextChanged(object sender, EventArgs e)
    {
        int searchtextlenghth = int.Parse(txtSearchPostalCode.Text.Length.ToString());

        if (searchtextlenghth > 3)
        {
            string serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
            string padleft = serachpostcode.Insert(3, " ");
            txtSearchPostalCode.Text = padleft;
        }
    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        if (txtSearchPostalCode.Text == "")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter valid postal code in format A#A #A# to search', 'error');", true);
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        if (_serachpostcode.Length.ToString() != "6")
        {
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter 6 digit postal code to search', 'error');", true);
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter valid postal code in format A#A #A# to search', 'error');", true);
            txtSearchPostalCode.Focus();
            return;
        }
        // ClearAddress();
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

    protected void StartDate_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        LinkButton StartDate = (LinkButton)gvAddressList.Rows[gvr.RowIndex].FindControl("StartDate");
        divAddres.Attributes.Add("style", "display:block;");
        if (StartDate.Text != " ADD NEW ADDRESS")
        {
            ClearAddress();
            String[] Split = StartDate.Text.Split(',');
            txtPostalCode.Text = txtSearchPostalCode.Text;
            txtPostalCode.ReadOnly = true;
            txtPostalCode.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.Text = Split[0].ToString().ToUpper();
            txtStreetName.ReadOnly = true;
            txtStreetName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#1a2035");

            txtCity.Text = Split[1].ToString().ToUpper();
            txtCity.ReadOnly = true;
            txtProvince.Text = Split[2].ToString().ToUpper();
            txtProvince.ReadOnly = true;
        }
        else
        {
            txtSuiteNumber.Text = "";
            txtStreetNo.Text = "";
            //firoz 08
            //DropDownList1.SelectedValue = "";
            //end
            DropDownList2.SelectedIndex = 0;
            txtSuiteNumber.Text = "";
            txtPostalCode.Text = "";
            txtPostalCode.ReadOnly = false;
            txtPostalCode.Enabled = true;
            txtStreetName.Text = "";
            txtStreetName.ReadOnly = false;
            txtStreetName.Enabled = true;
            txtStreetType.Text = "";

            txtStreetType.ReadOnly = false;
            txtStreetType.Enabled = true;
            txtCity.Text = "";
            txtProvince.Text = "";
            txtCity.ReadOnly = false;
            txtCity.Enabled = true;
            txtProvince.ReadOnly = false;
            txtProvince.Enabled = true;
            txtPostalCode.Text = txtSearchPostalCode.Text;
            //ClearAddressContactDetails();
        }
    }
    public void ClearAddress()
    {
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        //firoz 08
        // DropDownList1.SelectedValue = "";
        //end
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";
        txtStreetNo.Text = "";
    }
    protected void rdobtnlist_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void lnkView_Click(object sender, EventArgs e)
    {

    }

    protected void txtMainEmail_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
        {
            div1msg.Visible = true;
            divtick.Visible = false;
        }
        else
        {
            div2stmsg.Visible = false;
            divtick.Visible = true;
        }

        if (txtMainEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtMainEmail.Text) == true)
            {
                divValid.Visible = false;
                bool ceck = false;
                ceck = comm.IfExistsInTable("UserAcc", "*", "Email='" + txtMainEmail.Text + "'");
                if (ceck == false)
                {
                    divconfirm.Visible = true;
                }
                if (txtMainEmail.Text.Trim() == "")
                {
                    divconfirm.Visible = false;
                }
            }
            else
            {
                divValid.Visible = true;
            }
        }
        else
        {
            txtConLoginEmail.Text = "";
            div2stmsg.Visible = false;
            divconfirm.Visible = false;
        }
    }
    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    protected void txtConLoginEmail_TextChanged(object sender, EventArgs e)
    {
        div1msg.Visible = false;
        if (txtMainEmail.Text.Trim() != txtConLoginEmail.Text.Trim())
        {
            div2stmsg.Visible = true;
            divtick.Visible = false;
        }
        else
        {
            div2stmsg.Visible = false;
            divtick.Visible = true;
        }
    }

    protected void btnSaveNewRole_Click(object sender, EventArgs e)
    {
        var empid = Session["EmployeeId"].ToString();
        DataTable dt = comm.GetAllDataFromTable("AssginBussiness assb join AddBusiness ab on assb.BusinessID = ab.BusinessID", "ab.BusinessID, assb.BusinessName", "EmployeeID=" + empid + " and assb.UserTypeId=1", "assb.BusinessID");
        if (String.IsNullOrEmpty(txtRole.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter role', 'error');", true);
        }
        else
        {
            bool chk = comm.IfExistsInTable("UserType", "UserType", "UserType='" + txtRole.Text + "'");
            if (chk == false)
            {
                var BownerId = comm.GetAllDataFromTable("AssginBussiness", "EmployeeID", "BusinessID=" + ViewState["bid"].ToString() + " AND UserTypeId=1", "");
                comm.ExecuteQuery("insert into UserType(UserType,MotherOrgId,BownerID)values('" + txtRole.Text + "','0'," + BownerId.Rows[0][0].ToString() + ")");
            }
            txtRole.Text = "";
            BindAllUserType();
        }
    }


    protected void rdoBtnUserTypeList_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnSendBuss_Click(object sender, EventArgs e)
    {
        bool boolCheckBox = false;
        StringBuilder sbBuss = new StringBuilder();
        int cnt = 0;
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (itm.Selected == true)
            {
                boolCheckBox = true;
            }
        }
        if (boolCheckBox == false)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select User Role', 'warning');", true);
            return;
        }

        var check_available_business = 0;
        foreach (GridViewRow row in gvAvBu.Rows)
        {
            var chk_bussname = (CheckBox)row.FindControl("chkbussname");
            {
                if (chk_bussname.Checked)
                {
                    check_available_business = 1;
                }
            }
        }
        if (check_available_business == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select available business names in change business', 'warning');", true);
            return;
        }

        DataTable dt = (DataTable)ViewState["SelectBuss"];
        foreach (GridViewRow row in gvAvBu.Rows)
        {
            var chkbussname = (CheckBox)row.FindControl("chkbussname");
            var hfdBusinessID = (HiddenField)row.FindControl("hfdBusinessID");
            var bussname = (Label)row.FindControl("bussname");
            var UserTypeid = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0")).ToString();
            var userTypeText = AnyminuteEmployeeEditor.StoredName(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
            if (chkbussname.Checked)
            {

                foreach (GridViewRow row1 in GvSelectBuss.Rows)
                {
                    var hfdBusinessIDslect = (HiddenField)row1.FindControl("hfdBusinessID");
                    var bussnameSelect = (Label)row.FindControl("bussname");
                    if (hfdBusinessID.Value.ToString().Trim() == hfdBusinessIDslect.Value.ToString().Trim())
                    {
                        sbBuss.Append(bussnameSelect.Text);
                        sbBuss.Append(",");
                        cnt++;
                    }


                }

                if (cnt == 0)
                {
                    dt.Rows.Add(bussname.Text, hfdBusinessID.Value, userTypeText, UserTypeid, "", "R");
                    ViewState["SelectBuss"] = dt;
                    chkbussname.Checked = false;
                }
                else
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + sbBuss + " Already Selected', 'warning');", true);
                    return;
                }

            }
            this.BindGrid();
        }
        //string str = "";
        //str = fupd.FileName;
        //if (fupd.HasFile)
        //{
        //    fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
        //    ViewState["profile"] = str;
        //}

        string str = "";
        if (fupd.HasFile)
        {
            var fileName = Path.GetFileName(fupd.PostedFile.FileName);
            var fileExt = Path.GetExtension(fileName);
            str = string.Format("{0}{1}",
                                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                        fileExt);
            fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
        }
        ViewState["profile"] = str;
    }

    protected void btnBackBuss_Click(object sender, EventArgs e)
    {
        //var check_role_selection = 0;
        //foreach (ListItem itm in rdoBtnUserTypeList.Items)
        //{
        //    if (itm.Selected == true)
        //    {
        //        check_role_selection = 1;
        //    }
        //}
        //if (check_role_selection == 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select user role in permission role', 'warning');", true);
        //    return;
        //}

        //var check_available_business = 0;
        //foreach (GridViewRow row in gvAvBu.Rows)
        //{
        //    var chk_bussname = (CheckBox)row.FindControl("chkbussname");
        //    {
        //        if (chk_bussname.Checked)
        //        {
        //            check_available_business = 1;
        //        }
        //    }
        //}
        //if (check_available_business == 0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select available business names in change business', 'warning');", true);
        //    return;
        //}

        var uncheckcheck_selected_business = 0;
        foreach (GridViewRow row in GvSelectBuss.Rows)
        {
            var chk_bussname = (CheckBox)row.FindControl("chkbussname");
            {
                if (chk_bussname.Checked)
                {
                    uncheckcheck_selected_business = 1;
                }
            }
        }
        if (uncheckcheck_selected_business == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please check selected business names in change business', 'warning');", true);
            return;
        }

        DataTable dt = (DataTable)ViewState["SelectBuss"];
        DataTable dtSelect = new DataTable();
        foreach (GridViewRow row in GvSelectBuss.Rows)
        {
            var chkbussname = (CheckBox)row.FindControl("chkbussname");
            var hfdBusinessID = (HiddenField)row.FindControl("hfdBusinessID");
            var bussname = (Label)row.FindControl("bussname");
            if (chkbussname.Checked)
            {
                DataRow[] rowsToRemove = dt.Select("BusinessID=" + hfdBusinessID.Value + "");
                foreach (var rowToDelete in rowsToRemove)
                    dt.Rows.Remove(rowToDelete);

            }

        }
        GvSelectBuss.DataSource = dt;
        GvSelectBuss.DataBind();
        //string str = "";
        //str=fupd.FileName;
        //if(fupd.HasFile)
        //{
        //    fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
        //    ViewState["profile"] = str;
        //}

        string str = "";
        if (fupd.HasFile)
        {
            var fileName = Path.GetFileName(fupd.PostedFile.FileName);
            var fileExt = Path.GetExtension(fileName);
            str = string.Format("{0}{1}",
                                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds(),
                                        fileExt);
            fupd.PostedFile.SaveAs(Server.MapPath("~/Uploads/" + str));
        }
        ViewState["profile"] = str;
    }

    //protected void GvSelectBuss_RowCommand(object sender, GridViewCommandEventArgs e)
    //{
    //    if(e.CommandName== "assgin")
    //    {
    //        int index =Convert.ToInt32(e.CommandArgument.ToString());
    //        HiddenField hfdBusinessID = (HiddenField)GvSelectBuss.Rows[index].FindControl("hfdBusinessID");
    //        HiddenField hfdRoleid = (HiddenField)GvSelectBuss.Rows[index].FindControl("hfdRoleid");
    //        Label bussname = (Label)GvSelectBuss.Rows[index].FindControl("bussname");
    //        lblBuss.Text = bussname.Text;
    //        ModalPopupEmailConfirm.Show();

    //    }
    //}

    protected void btnassignee_Click(object sender, EventArgs e)
    {
        //if(ddlRoleNamepop.SelectedIndex==0)
        //{
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select role name', 'warning');", true);
        //    return;
        //}

        DataTable dt = (DataTable)ViewState["SelectBuss"];
        StringBuilder _ProjectSb = new StringBuilder();
        StringBuilder StringBuilder__ = new StringBuilder();
        foreach (GridViewRow row in GridView_User_Role_Permission_Pop.Rows)
        {
            string ddl_Role_Permission_Access_Status_ = ((DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status")).SelectedValue;
            if (ddl_Role_Permission_Access_Status_ == "YES")
            {
                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                StringBuilder__.Append(Role_Permission_Id_.Value + ",");
            }
        }
        foreach (DataRow dr in dt.Rows) // search whole table
        {
            if (dr["BusinessID"].ToString().Trim() == hfdbussidpop.Value.ToString().Trim()) // if id==2
            {
                dr["Role"] = AnyminuteEmployeeEditor.StoredName(ddlRoleNamepop.SelectedItem);
                dr["Roleid"] = ddlRoleNamepop.SelectedValue;//change the name

                dr["ProjectType"] = ddlUserType.SelectedValue;//break; break or not depending on you
                foreach (ListItem itm in ddlProject.Items)
                {
                    if (itm.Selected == true)
                    {
                        _ProjectSb.Append(itm.Value);
                        _ProjectSb.Append(",");
                    }
                }
                dr["Projectid"] = _ProjectSb.ToString();
                dr["Permission"] = StringBuilder__.ToString();
            }
        }
      
        ViewState["AddinationRole"] = StringBuilder__.ToString();
        GvSelectBuss.DataSource = dt;
        GvSelectBuss.DataBind();
    }

    protected void lnlRole_Click(object sender, EventArgs e)
    {
        ddlUserType.Items.Clear();
        ddlRoleNamepop.SelectedValue = "";

        LinkButton btn = (LinkButton)sender;
        GridViewRow row = (GridViewRow)btn.NamingContainer;
        HiddenField hfdBusinessID = (HiddenField)row.FindControl("hfdBusinessID");
        HiddenField hfdRoleid = (HiddenField)row.FindControl("hfdRoleid");
        Label bussname = (Label)row.FindControl("bussname");
        hfdbussidpop.Value = hfdBusinessID.Value;
        lblBuss.Text = bussname.Text;
        bool chkProject = comm.IfExistsInTable("AddProject", "*", "BusinessID=" + hfdBusinessID.Value + "");
        DataTable dtEmpType = new DataTable();
        dtEmpType.Columns.Add("value", typeof(string));
        dtEmpType.Columns.Add("text", typeof(string));
        if (chkProject == true)
        {
            dtEmpType.Rows.Add(new Object[]{
                "R",
                "Regular"
           });
            dtEmpType.Rows.Add(new Object[]{
                "P",
                "Project"
           });
            ddlProject.Items.Clear();
            DataTable dtProject = comm.GetAllDataFromTable("AddProject", "ProjectID,ProjectName", "BusinessID=" + hfdBusinessID.Value + "", "ProjectID");
            ddlProject.DataSource = dtProject;
            ddlProject.DataValueField = dtProject.Columns[0].ToString();
            ddlProject.DataTextField = dtProject.Columns[1].ToString();
            ddlProject.DataBind();
        }
        else
        {
            dtEmpType.Rows.Add(new Object[]{
                "R",
                "Regular"
           });
        }
        ViewState["Popuprole"] = rdoBtnUserTypeList.SelectedValue;
        ddlUserType.DataSource = dtEmpType;
        ddlUserType.DataValueField = dtEmpType.Columns[0].ToString();
        ddlUserType.DataTextField = dtEmpType.Columns[1].ToString();
        ddlUserType.DataBind();
        ddlRoleNamepop.SelectedValue = rdoBtnUserTypeList.SelectedValue;
        GetUserPermissionRoleList(0);
        ModalPopupEmailConfirm.Show();
    }

    public int SavePersonalDetails()
    {
        int ResultSavePersonalDetails = 0;
        try
        {

            EmpEntity.SINExpiry = txtSINExpiry.Text;
            EmpEntity.UserType = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
            string userTypeUserACC = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0")).ToString();
            EmpEntity.LegalName = "";
            EmpEntity.Title = "";
            EmpEntity.FirstName = txtFirstName.Text;
            EmpEntity.MiddleName = txtMiddleName.Text;
            EmpEntity.LastName = txtLastName.Text;
            EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
            EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
            EmpEntity.Gender = ddlgender.SelectedItem.Text;
            EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
            EmpEntity.IsActive = ddlEmpStatus.SelectedValue;
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            //if (ddlEmpStatus.SelectedValue == "NO")
            //{
            //    EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            //}
            //else
            //{
            //    EmpEntity.Apartment = "";
            //}
            if (ddlEmpStatus.SelectedValue == "NO")
            {
                EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            }
            else
            {
                if (txtInActiveDate.Text != "")
                    EmpEntity.Apartment = DateTime.Parse(txtInActiveDate.Text.ToString()).ToString();//Inactivedate
                else
                    EmpEntity.Apartment = "";
            }


            EmpEntity.TerminationDate = DateTime.Now.Date;

            EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString());

            EmpEntity.Occupation = "";

            int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);

            ResultSavePersonalDetails = Empid;


            hfdEmpID.Value = Empid.ToString();
            SavePay();
            SaveAddress();
            if (string.IsNullOrEmpty(Convert.ToString(ViewState["profile"])))
            {
                ViewState["profile"] = "";
            }

            int userid = SaveProfilePic(Empid, ViewState["profile"].ToString());
            ViewState["ViweStateUserID"] = userid;

            return ResultSavePersonalDetails;
        }
        catch (Exception ex)
        {
            return ResultSavePersonalDetails;
            throw ex;

        }

    }
    public int SavePay()
    {
        int ResultSavePay = 0;
        try
        {
            if (txtBillRate.Text == "")
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please Enter Billing Rate', 'warning');", true);
                return 0;
            }

            RateEntity.EmployeeId = int.Parse(hfdEmpID.Value);
            RateEntity.Rate = float.Parse(txtBillRate.Text);
            RateEntity.RateType = ddlBillingType.SelectedItem.Text;
            //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
            RateEntity.StartDate = DateTime.Parse(txtHiredate.Text);
            RateEntity.EndDate = System.DateTime.Now.Date;
            RateEntity.JobTitle = "";
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
            RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
            ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, int.Parse(hfdEmpID.Value));


            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Rate Details Added successfully', 'success');", true);

            return ResultSavePay;
            //ClearPaymentDetails();
        }
        catch (Exception ex)
        {
            return ResultSavePay;
            throw ex;
        }
    }

    public int SaveAddress()
    {
        int ResultSaveAddres = 0;
        DataTable cityid = new DataTable();
        DataTable province = new DataTable();
        try
        {
            bool chk = comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtCity.Text + "'");
            if (chk == false)
            {
                // ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtCity.Text + " not Exists, Contact Support Admin ', 'error');", true);
                //return;
                comm.ExecuteQuery("insert into CityMaster (CityName,Duplicate)values('" + txtCity.Text + "','False')");
            }

            bool chkProvinceMaster = comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtProvince.Text + "'");
            if (chkProvinceMaster == false)
            {
                //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', '" + txtProvince.Text + " not Exists, Contact Support Admin ', 'error');", true);
                // return;
                comm.ExecuteQuery("insert into ProvinceMaster (ProvinceName,Duplicate)values('" + txtProvince.Text + "','False')");
                province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            }
            cityid = comm.GetAllDataFromTable("CityMaster", "CityId", "CityName='" + txtCity.Text + "'", "");
            province = comm.GetAllDataFromTable("ProvinceMaster", "ProvinceId", "ProvinceName='" + txtProvince.Text + "'", "");
            txtPostalCode.Text = txtPostalCode.Text.Replace(" ", string.Empty).Trim();
            EmpAddEnity.Postalcode = txtPostalCode.Text;
            EmpAddEnity.NumberSuffix = "";
            EmpAddEnity.StreetName = txtStreetName.Text.ToUpper();
            EmpAddEnity.StreetType = txtStreetType.Text.ToUpper();
            EmpAddEnity.StreetDirection = txtStreetDirection.Text;
            //EmpAddEnity.SuitType = ddlSuitType.SelectedItem.Text;
            EmpAddEnity.SuitType = DropDownList2.SelectedItem.Text;
            EmpAddEnity.SuiteNumber = txtSuiteNumber.Text;
            //firoz 07
            EmpAddEnity.SuitSuffix = ddlsuitesuffix.SelectedItem.Text;
            //end
            EmpAddEnity.City = txtCity.Text.ToUpper();
            EmpAddEnity.Province = txtProvince.Text.ToUpper();
            EmpAddEnity.MainPhone = txtMainPhone.Text;
            EmpAddEnity.WorkPhone = "";
            EmpAddEnity.Mobile = txtMobile.Text;
            EmpAddEnity.Fax = "";
            EmpAddEnity.MainEmail = txtMainEmail.Text.Trim(); ;
            EmpAddEnity.CCEmail = "";
            EmpAddEnity.Website = "";
            EmpAddEnity.Other1 = "";
            EmpAddEnity.StreetNo = txtStreetNo.Text.ToUpper();
            RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
            RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
            if (cityid.Rows.Count > 0)
            {
                EmpAddEnity.CityId = int.Parse(cityid.Rows[0]["CityId"].ToString());
            }
            else
            {
                EmpAddEnity.CityId = 0;
            }

            if (province.Rows.Count > 0)
            {
                EmpAddEnity.ProvinceId = int.Parse(province.Rows[0]["ProvinceId"].ToString());
            }
            else
            {
                EmpAddEnity.ProvinceId = 0;
            }
            RateEntity.RetainVacation = ddlRTVA.SelectedValue;
            EmpAddEnity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString()); //int.Parse(hfdMotherId.Value);

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, int.Parse(hfdEmpID.Value));

            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Address Saved Successfully', 'success');", true);

            return ResultSaveAddres;

            //ClearAddressContactDetails();
        }
        catch (Exception ex)
        {
            throw ex;
            //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'First Save Personal Details, then Save Contact Details', 'error');", true);
            //return;
        }
    }

    public int SaveProfilePic(int empid, string str)
    {


        Uae.EmployeeId = empid;
        Uae.Email = txtMainEmail.Text.Trim();
        Uae.MotherOrgid = Convert.ToInt32(Session["MotherOrgid"].ToString());
        Uae.UserTypeid = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
        Uae.UserType = AnyminuteEmployeeEditor.StoredName(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
        Uae.ProfileImageName = str;

        return Convert.ToInt32(Uac.Sp_InsertUserAccFromAddress(Uae));

    }

    public void SaveUpdateRolePermission(int empid, int bussid, int usertype,string permission)
    {
        bool check_emp_id_in_user_Role_permission = comm.IfExistsInTable("User_Role_Permission", "Employee_Id", "Employee_Id='" + hfdEmpID.Value + "'");
       
        if (check_emp_id_in_user_Role_permission == true)
        {
           
            comm.ExecuteQuery("update User_Role_Permission set Role_Permission_Id='" + permission + "', BusinessId='" + bussid + "'  where Employee_Id ='" + hfdEmpID.Value + "' and UserTypeId = '" + usertype + "' and Status='True' ");
        }
        else
        {
            
            comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) Values ('" + permission + "', '" + empid + "', '" + usertype + "', 'True', '" + bussid + "')");
        }
    }
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        #region textbox validations
        if (txtFirstName.Text.Trim() == "")
        {
            txtFirstName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter first name in personal details', 'warning');", true);
            return;
        }
        if (txtLastName.Text.Trim() == "")
        {
            txtLastName.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter last name in personal details', 'warning');", true);
            return;
        }
        if (txtHiredate.Text == "")
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter hire date in personal details', 'warning');", true);
            return;
        }
        DateTime hdate = DateTime.Parse(txtHiredate.Text).Date;
        DateTime currdate = DateTime.Now.Date;
        if (hdate > currdate)
        {
            txtHiredate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Hire date cannot be greater than current date in personal details', 'warning');", true);
            return;
        }
        //if (txtTempPassword.Text == "")
        //{
        //    txtTempPassword.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter temporary password in personal details', 'warning');", true);
        //    return;
        //}
        //if (txtcnfirmTempPassword.Text == "")
        //{
        //    txtcnfirmTempPassword.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter confirm temporary password in personal details', 'warning');", true);
        //    return;
        //}
        //if (txtTempPassword.Text != txtcnfirmTempPassword.Text)
        //{
        //    txtcnfirmTempPassword.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter same password in confirm temporary password in personal details', 'warning');", true);
        //    return;
        //}
        if (txtMainEmail.Text == "")
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter email address in personal details', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtMainEmail.Text) == false)
            {
                txtMainEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter vaild email address in personal details', 'warning');", true);
                return;
            }
        }
        if (txtConLoginEmail.Text == "")
        {
            txtConLoginEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please confirm email address in personal details', 'warning');", true);
            return;
        }
        if (txtMainEmail.Text != txtConLoginEmail.Text)
        {
            txtMainEmail.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter same email address in confirm email address in personal details', 'warning');", true);
            return;
        }
        if (txtConLoginEmail.Text.Trim() != string.Empty)
        {
            if (IsValidEmail(txtConLoginEmail.Text) == false)
            {
                txtConLoginEmail.Focus();
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter vaild confirm email address in personal details', 'warning');", true);
                return;
            }
        }
        if (txtMainPhone.Text == "")
        {
            txtMainPhone.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter contact phone in personal details', 'warning');", true);
            return;
        }
        if (txtSearchPostalCode.Text.Trim() == "")
        {
            txtSearchPostalCode.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter valid postal code in format A#A #A# and click on search in personal details', 'warning');", true);
            return;
        }
        if (txtStreetNo.Text.Trim() == "")
        {
            txtStreetNo.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter street number in personal details', 'warning');", true);
            return;
        }
        //if (ddlBillingType.SelectedIndex == 0)
        //{
        //    ddlBillingType.Focus();
        //    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select pay type in personal details', 'warning');", true);
        //    return;
        //}
        if (txtBillRate.Text.Trim() == "")
        {
            txtBillRate.Focus();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please enter pay rate in personal details', 'warning');", true);
            return;
        }
        #endregion

        #region user role and gridview validation
        var check_role_selection = 0;
        foreach (ListItem itm in rdoBtnUserTypeList.Items)
        {
            if (itm.Selected == true)
            {
                check_role_selection = 1;
            }
        }
        if (check_role_selection == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select user role in permission role', 'warning');", true);
            return;
        }
        if (GvSelectBuss.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please select available business names in change business', 'warning');", true);
            return;
        }
        #endregion

        #region Validation to check employee and manager cannot become owner or accountant and vice versa
        //Added by Kumar. Employee and Manager can not become the Owner or Accountant
        DataTable dt_Check_User_Registrered_As_Employee_Or_Manager = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (2,4)", "");
        if (dt_Check_User_Registrered_As_Employee_Or_Manager.Rows.Count > 0)
        {
            int Check_UserType = 0;
            foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
            {
                if (item_Check_UserType.Selected == true)
                {
                    Check_UserType = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
                }
            }
            if (Check_UserType == 5 || Check_UserType == 12)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Employee or Manager. This user can not become Owner or Accountant.', 'warning');", true);
                return;
            }
        }
        //Added by Kumar. Owner can not become Employee or Manager
        DataTable dt_Check_User_Registrered_As_Owner = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (5)", "");
        if (dt_Check_User_Registrered_As_Owner.Rows.Count > 0)
        {
            int Check_UserType = 0;
            foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
            {
                if (item_Check_UserType.Selected == true)
                {
                    Check_UserType = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
                }
            }
            if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 12)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Owner. This user can not become Employee or Manager.', 'warning');", true);
                return;
            }
        }
        //Added by Kumar. Accountant can not become Employee or Manager
        DataTable dt_Check_User_Registrered_As_Accountant = comm.GetAllDataFromTable("EmpAddress ea join AddEmployee ae on ae.ADDRESSID=ea.EmpAddressId ", "ae.MotherOrgID,ae.UserType", "ea.MainEmail='" + txtMainEmail.Text + "' and ae.UserType in (12)", "");
        if (dt_Check_User_Registrered_As_Accountant.Rows.Count > 0)
        {
            int Check_UserType = 0;
            foreach (ListItem item_Check_UserType in rdoBtnUserTypeList.Items)
            {
                if (item_Check_UserType.Selected == true)
                {
                    Check_UserType = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
                }
            }
            if (Check_UserType == 2 || Check_UserType == 4 || Check_UserType == 5)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', '" + txtMainEmail.Text + " is regsitered as Accountant. This user can not become Employee or Manager.', 'warning');", true);
                return;
            }
        }
        #endregion

        #region Validation to check if the user is alreday registered in same business
        foreach (GridViewRow row_check_user_role_in_business in GvSelectBuss.Rows)
        {
            if (txtMainEmail.Text.Trim() != string.Empty)
            {
                HiddenField hfdBusinessID_check = row_check_user_role_in_business.FindControl("hfdBusinessID") as HiddenField;
                int BussNo1 = int.Parse(hfdBusinessID_check.Value);
                bool Check_employee_manager = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (2,4)");
                if (Check_employee_manager == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current Business', 'warning');", true);
                    return;
                }
                bool Check_Owner = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (5)");
                if (Check_Owner == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current Business', 'warning');", true);
                    return;
                }
                bool Check_Accountant = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType in (12)");
                if (Check_Accountant == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current Business', 'warning');", true);
                    return;
                }
                bool Check_Other_UserType_Role = comm.IfExistsInTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ad.EmployeeID", "*", "assb.BusinessID=" + BussNo1 + " and ea.MainEmail='" + txtMainEmail.Text + "' and ad.UserType not in (2,4,5,12)");
                if (Check_Other_UserType_Role == true)
                {
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', ' User Email Id already registered in Current Business', 'warning');", true);
                    return;
                }
            }
        }
        #endregion


        foreach (GridViewRow row in GvSelectBuss.Rows)
        {

            hfdEmpID.Value = SavePersonalDetails().ToString();
            HiddenField hfdBusinessID = row.FindControl("hfdBusinessID") as HiddenField;
            Label bussname = row.FindControl("bussname") as Label;
            Label lnlRole = row.FindControl("lnlRole") as Label;
            HiddenField hfdRoleid = row.FindControl("hfdRoleid") as HiddenField;
            HiddenField hfdProjectid = row.FindControl("hfdProjectid") as HiddenField;
            HiddenField hfdProjectType = row.FindControl("hfdProjectType") as HiddenField;
            HiddenField hfdPermissionId = (HiddenField)row.FindControl("hfdPermissionId");
            string EmployeeName = comm.GetNameFromId("AddEmployee", Convert.ToInt32(hfdEmpID.Value));
            AddBeCtrl.AssgineBusiness(Convert.ToInt32(hfdBusinessID.Value), Convert.ToInt32(hfdEmpID.Value), bussname.Text, EmployeeName, AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0")), Convert.ToInt32(Session["MotherOrgid"].ToString()));
            SaveUpdateRolePermission(Convert.ToInt32(hfdEmpID.Value), Convert.ToInt32(hfdBusinessID.Value), Convert.ToInt32(hfdRoleid.Value), hfdPermissionId.Value);
            comm.ExecuteQuery("insert into MappingTable(USERID,UserTypeId,EmployeeID,BusinessID)values(" + ViewState["ViweStateUserID"] + "," + hfdRoleid.Value + "," + hfdEmpID.Value + "," + hfdBusinessID.Value + ")");
            comm.ExecuteQuery("UPDATE AddEmployee SET UserType=" + hfdRoleid.Value + ",EMPLOYEETYPE='" + hfdProjectType.Value + "' WHERE EmployeeID=" + hfdEmpID.Value + "");
            comm.ExecuteQuery("UPDATE AssginBussiness SET UserTypeId=" + hfdRoleid.Value + " WHERE EmployeeID=" + hfdEmpID.Value + " AND BusinessID=" + hfdBusinessID.Value + "");

            if (!string.IsNullOrEmpty(hfdProjectid.Value))
            {
                string founderMinus1 = hfdProjectid.Value.Remove(hfdProjectid.Value.Length - 1, 1);
                string[] pieces = founderMinus1.Split(new string[] { "," },
                                   StringSplitOptions.None);
                for (int i = 0; i < pieces.Length; i++)
                {
                    ape.BusinessID = Convert.ToInt32(hfdBusinessID.Value);
                    ape.EmployeeID = Convert.ToInt32(hfdEmpID.Value);
                    ape.ProjectID = Convert.ToInt32(pieces[i].ToString());
                    apc.InsertAssgineProject(ape);
                }

            }
            var mangerid = int.Parse(Session["EmployeeId"].ToString());
            AssgineManger(int.Parse(hfdEmpID.Value), int.Parse(Session["UserTypeid"].ToString()), int.Parse(hfdBusinessID.Value), int.Parse(hfdRoleid.Value), mangerid, mangerid);

        }
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved', 'success');", true);
    }
    public void AssgineManger(int empid, int UserType, int BusinessID, int Roleid, int ManagerId, int oldempid)
    {
        switch (UserType)
        {
            case 2:
                var MangerName = comm.GetNameFromId("AddEmployee", ManagerId);
                var EmpName = comm.GetNameFromId("AddEmployee", empid);
                var BussName = comm.GetNameFromId("AddBusiness", BusinessID);
                asaMangEntity.BusinessID = BusinessID;
                asaMangEntity.BusinessName = BussName;
                asaMangEntity.ManagerID = ManagerId;
                asaMangEntity.MangerName = MangerName;
                asaMangEntity.EmpId = empid;
                asaMangEntity.EmpName = EmpName;
                asMangCtrl.InsertAssgineManger(asaMangEntity);

                break;
            case 4:
                string MangerName_emp = "", EmpName_emp = "", BussName_emp = "", MangerID_emp = "";
                DataTable dtMangerDts = comm.GetAllDataFromTable("AssgineManager", "*", "EmpId=" + oldempid + " and BusinessID=" + BusinessID + "", "");
                if (dtMangerDts.Rows.Count > 0)
                {
                    MangerName_emp = dtMangerDts.Rows[0]["MangerName"].ToString();
                    EmpName_emp = comm.GetNameFromId("AddEmployee", empid);
                    BussName_emp = dtMangerDts.Rows[0]["BusinessName"].ToString();
                    MangerID_emp = dtMangerDts.Rows[0]["ManagerID"].ToString();
                }
                else
                {
                    MangerName_emp = comm.GetNameFromId("AddEmployee", ManagerId);
                    EmpName_emp = comm.GetNameFromId("AddEmployee", empid);
                    BussName_emp = comm.GetNameFromId("AddBusiness", BusinessID);
                    MangerID_emp = ManagerId.ToString();
                }

                asaMangEntity.BusinessID = BusinessID;
                asaMangEntity.BusinessName = BussName_emp;
                asaMangEntity.ManagerID = int.Parse(MangerID_emp);
                asaMangEntity.MangerName = MangerName_emp;
                asaMangEntity.EmpId = empid;
                asaMangEntity.EmpName = EmpName_emp;
                asMangCtrl.InsertAssgineManger(asaMangEntity);
                break;
            case 5:
                var BussName_owner = comm.GetNameFromId("AddBusiness", BusinessID);
                var EmpName_owner = comm.GetNameFromId("AddEmployee", empid);
                AddBeCtrl.AssgineBusiness(BusinessID, empid, BussName_owner, EmpName_owner, Roleid, Convert.ToInt32(Session["MotherOrgid"].ToString()));
                break;
        }
    }
    protected void ddlCatagoryRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetUserPermissionRoleList(int.Parse(ddlCatagoryRole.SelectedValue));
    }

    protected void GridView_User_Role_Permission_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList DropDownList_User_Role_Permission_Access_Status = e.Row.FindControl("DropDownList_User_Role_Permission_Access_Status") as DropDownList;
            DropDownList_User_Role_Permission_Access_Status.SelectedValue = "YES";
        }
    }



    #region employee edit region
    public void GetChangeRateRole(int EmployeeID)
    {
        ////This will check the login user has been assigned in which business in what roles with the employee id of same login user
        //DataTable Dt_Check_User_Role_In_All_Business = comm.GetAllDataFromTable("MappingTable", "*", "USERID = '" + Session["UserId"].ToString() + "' and UserTypeId='5' and BusinessId = '" + Request.QueryString["BussEditId"].ToString() + "'", "UserTypeId");

        //if (Dt_Check_User_Role_In_All_Business.Rows.Count > 0)
        //{
        //    DataTable Dt_Check_Manager_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "' and UserTypeId='5'", "");
        //    if (Dt_Check_Manager_Role_Permission.Rows.Count > 0)
        //    {
        //        DataTable dtPersonalDetailsForEdit = new DataTable();
        //        dtPersonalDetailsForEdit = comm.GetAllDataFromTable("Role_Permission_Master", "dbo.udf_CheckRolesByEmployeeid('" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "',15) as 'IsExistsCheckUpdatePay',dbo.udf_CheckRolesByEmployeeid('" + Dt_Check_User_Role_In_All_Business.Rows[0]["EmployeeId"].ToString() + "',16) as 'IsExistsCheckAssignPermissionRole'", "", "");
        //        if (dtPersonalDetailsForEdit != null)
        //        {
        //            if (dtPersonalDetailsForEdit.Rows.Count > 0)
        //            {
        //                string IsExistsCheckUpdatePay = Convert.ToString(dtPersonalDetailsForEdit.Rows[0]["IsExistsCheckUpdatePay"]);
        //                if (IsExistsCheckUpdatePay == "N")
        //                {
        //                    //btnrateHistoryChangeRate.Enabled = false;
        //                    ddlBillingType.Enabled = false;
        //                    txtBillRate.Enabled = false;
        //                }
        //                else
        //                {
        //                    //btnrateHistoryChangeRate.Enabled = true;
        //                    ddlBillingType.Enabled = true;
        //                    txtBillRate.Enabled = true;
        //                }
        //                string IsExistsCheckAssignPermissionRole = Convert.ToString(dtPersonalDetailsForEdit.Rows[0]["IsExistsCheckAssignPermissionRole"]);
        //                if (IsExistsCheckAssignPermissionRole == "N")
        //                {
        //                    //DropDownList1122.Enabled = false;
        //                    GridView_User_Role_Permission.Enabled = false;
        //                }
        //                else
        //                {
        //                    //DropDownList1122.Enabled = true;
        //                    GridView_User_Role_Permission.Enabled = true;
        //                }
        //            }
        //        }
        //    }
        //}
    }

    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        DataTable dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*,convert(nvarchar(10),HireDate,120)HireDated", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        string originalType = dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString();
        ViewState["OriginalUserTypeId"] = originalType;
        AnyminuteEmployeeEditor.SelectStoredRole(rdoBtnUserTypeList, originalType);

        if (dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString() != "")
            txtSINExpiry.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString()).ToString("yyyy-MM-dd");

        txtFirstName.Text = dtPersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        txtMiddleName.Text = dtPersonalDetailsForEdit.Rows[0]["MiddleName"].ToString();
        txtLastName.Text = dtPersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        hfdSocialEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["SocialInsuranceNumber"].ToString();
        ddlEmpEI.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        ddlEmpCPP.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        ddlRTVA.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["RetainVacation"].ToString().Trim();

        if (dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString() != "")
        {
            hfddboEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString();
        }
        txtSocialNumer.Text = comm.Decrypt(hfdSocialEncrpty.Value);
        string dbo = comm.Decrypt(hfddboEncrpty.Value);
        if (dbo != "")
        {
            txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
        }

        ddlgender.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["Gender"].ToString();
        if (dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString() != "")
        {
            DateTime HireDate = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["HireDated"].ToString());
            txtHiredate.Text = HireDate.ToString("yyyy-MM-dd");
        }

        if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
            txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        ddlEmpStatus.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsActive"].ToString().Trim();
        if (ddlEmpStatus.SelectedValue == "NO")
        {
            if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
                txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        }
        if (dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            divAddres.Attributes.Add("style", "display:block;");
            GetAddressDetails(int.Parse(dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        }
        if (dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString() != "")
        {
            GetPaymentForEdit(int.Parse(dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString()));
        }
        GetRateInformation(hfdEmpID.Value);
    }

    public void GetPaymentForEdit(int Rateid)
    {
        DataTable dtRateDetailsForEdit = new DataTable();
        dtRateDetailsForEdit = comm.GetAllDataFromTable("RateDetails", "*", "RateId=" + Rateid + " and EmployeeId=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        txtBillRate.Text = dtRateDetailsForEdit.Rows[0]["Rate"].ToString();
        ddlBillingType.SelectedValue = dtRateDetailsForEdit.Rows[0]["RateType"].ToString();
    }

    public void Get_User_Role_Permission_While_Editing_Employee(int EmployeeID)
    {
        DataTable Dt_Check_Role_Permission = new DataTable();
        Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["EmployeeIDEdit"].ToString() + "' ", "");
        //Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
        if (Dt_Check_Role_Permission.Rows.Count > 0)
        {
            foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
            {
                string string_Check_Role_Permission = Dt_Check_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
                string[] splittedArray = string_Check_Role_Permission.Split(',');
                HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
                DropDownList ddl_Role_Permission_Access_Status_ = (DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status");
                foreach (string s in splittedArray)
                {
                    if (Role_Permission_Id_.Value == s)
                    {
                        ddl_Role_Permission_Access_Status_.SelectedValue = "YES";
                    }
                }
            }
        }
        else
        {
            //Div_User_Role_Permission.Visible = false;
        }
    }

    protected void GetRateInformation(string empidd)
    {
        //DataTable dtname = new DataTable();
        //int empid = 0;
        //if (hfdEmpID.Value == "")
        //{
        //    empid = 0;
        //}
        //else
        //{
        //    empid = int.Parse(hfdEmpID.Value);
        //}
        //hfdchangerateid.Value = empid.ToString();
        //dtname = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + empid + "", "");
        //if (dtname.Rows.Count > 0)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("<b>Employee Name: </b>" + lblEmpNameForAddressCont.Text);
        //    sb.AppendLine("<b>Employee Status: </b>" + dtname.Rows[0]["IsActive"].ToString());
        //    // lblNameemp.Text = "Employee Name: " + lblEmpNameForAddressCont.Text + "\n" + "Is Active :" + dtname.Rows[0]["IsActive"].ToString() + "";
        //    lblNameempRateHistory.Text = sb.ToString().Replace(Environment.NewLine, "<br />");
        //    DataTable dtratede = comm.GetAllDataFromTable("RateDetails", "*,convert(Nvarchar(10),effectivedate,120)effectivedateconv", "RateId='" + dtname.Rows[0]["Rateid"].ToString() + "'", "");
        //    if (dtratede.Rows.Count > 0)
        //    {
        //        DateTime mindate = DateTime.ParseExact(dtratede.Rows[0]["effectivedateconv"].ToString(), "yyyy-MM-dd", CultureInfo.CurrentCulture);
        //        txtrateHistoryEffectiveDate.Attributes.Add("min", mindate.ToString());
        //    }
        //}
        //DataTable dtminrate = comm.GetAllDataFromTable("RateDetails", "MIN(rateid) rateid,Rate,RateType", "EmployeeId=" + empid + " group by Rate,RateType", "");
        //if (dtminrate.Rows.Count > 0)
        //{
        //    ddlBillingType.SelectedValue = dtminrate.Rows[0]["RateType"].ToString();
        //    txtBillRate.Text = dtminrate.Rows[0]["Rate"].ToString();
        //}

        //gvRateHistory.DataSource = RateCtrl.GetRateHistory(empid);
        //gvRateHistory.DataBind();

        //if (gvRateHistory.Rows.Count == 1)
        //{
        //    gvRateHistory.Rows[0].Cells[6].Visible = false;
        //}

        //ModalPopupEmpList.Show();
    }

    public void GetAddressDetails(int EmpAddressId)
    {
        DataTable dtAddressDetailForEdit = new DataTable();
        dtAddressDetailForEdit = comm.GetAllDataFromTable("EmpAddress", "*", "EmpAddressId=" + EmpAddressId + "", "");

        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();
        txtStreetNo.Text = dtAddressDetailForEdit.Rows[0]["StreetNo"].ToString();
        txtStreetName.Text = dtAddressDetailForEdit.Rows[0]["StreetName"].ToString().ToUpper();

        txtStreetType.Text = dtAddressDetailForEdit.Rows[0]["StreetType"].ToString().ToUpper();
        txtSuiteNumber.Text = dtAddressDetailForEdit.Rows[0]["SuiteNumber"].ToString().ToUpper();
        txtStreetDirection.Text = dtAddressDetailForEdit.Rows[0]["StreetDirection"].ToString();

        DropDownList2.SelectedIndex = DropDownList2.Items.IndexOf(DropDownList2.Items.FindByValue(dtAddressDetailForEdit.Rows[0]["SuitType"].ToString()));

        txtCity.Text = dtAddressDetailForEdit.Rows[0]["City"].ToString().ToUpper();
        txtProvince.Text = dtAddressDetailForEdit.Rows[0]["Province"].ToString().ToUpper();
        txtPostalCode.Text = dtAddressDetailForEdit.Rows[0]["Postalcode"].ToString();

        txtMainPhone.Text = dtAddressDetailForEdit.Rows[0]["MainPhone"].ToString();
        txtMobile.Text = dtAddressDetailForEdit.Rows[0]["Mobile"].ToString();

        txtMainEmail.Text = dtAddressDetailForEdit.Rows[0]["MainEmail"].ToString();
        txtConLoginEmail.Text = dtAddressDetailForEdit.Rows[0]["CCEmail"].ToString();
    }
    #endregion

    protected void LinkButton_EditEmployee_Click(object sender, EventArgs e)
    {
        EmpEntity.EmployeeID = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
        EmpEntity.SINExpiry = txtSINExpiry.Text;
        EmpEntity.UserType = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0"));
        string userTypeUserACC = AnyminuteEmployeeEditor.StoredRoleId(rdoBtnUserTypeList, Convert.ToInt32(ViewState["OriginalUserTypeId"] ?? "0")).ToString();
        EmpEntity.LegalName = "";
        EmpEntity.Title = "";
        EmpEntity.FirstName = txtFirstName.Text;
        EmpEntity.MiddleName = txtMiddleName.Text;
        EmpEntity.LastName = txtLastName.Text;
        EmpEntity.SocialInsuranceNumber = comm.encrypt(txtSocialNumer.Text);
        EmpEntity.DateOfBirth = comm.encrypt(txtdob.Text);
        EmpEntity.Gender = ddlgender.SelectedItem.Text;
        EmpEntity.HireDate = DateTime.Parse(txtHiredate.Text);
        EmpEntity.IsActive = ddlEmpStatus.SelectedValue;
        RateEntity.IsEmployeeEIExempty = ddlEmpEI.SelectedValue;
        RateEntity.IsEmployeeCPPExempty = ddlEmpCPP.SelectedValue;
        RateEntity.RetainVacation = ddlRTVA.SelectedValue;
        if (ddlEmpStatus.SelectedValue == "NO")
        {
            EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
        }
        else
        {
            EmpEntity.Apartment = "";
        }

        EmpEntity.TerminationDate = DateTime.Now.Date;

        EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString());

        EmpEntity.Occupation = "";

        int Empid = EmpCtrl.UpdatePersonalDetails(EmpEntity, RateEntity);

        if (txtInActiveDate.Text == "")
            EmpEntity.Apartment = ""; // DateTime.Parse(DateTime.Now.ToShortDateString()).ToString();//Inactivedate

        if (txtInActiveDate.Text != "")
            EmpEntity.Apartment = DateTime.Parse(txtInActiveDate.Text.ToString()).ToString();//Inactivedate
        
        EmpEntity.EmployeeID = int.Parse(Request.QueryString["EmployeeIDEdit"].ToString());
        comm.ExecuteQuery("update AddEmployee set Apartment='" + EmpEntity.Apartment + "',IsActive='" + EmpEntity.IsActive + "' where EmployeeID=" + EmpEntity.EmployeeID + "");

        DataTable dtcntrate = comm.GetAllDataFromTable("RateDetails", "count(*)cnt", "EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "", "");
        if (int.Parse(dtcntrate.Rows[0]["cnt"].ToString()) == 1)
        {
            DataTable dtdetails = comm.GetAllDataFromTable("AddEmployee", "Rateid", "EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "", "");
            comm.ExecuteQuery("update RateDetails set RateType='" + ddlBillingType.SelectedValue + "',Rate=CONVERT(DECIMAL(10,2)," + txtBillRate.Text + "),StartDate='" + txtHiredate.Text + "',effectivedate='" + txtHiredate.Text + "' where EmployeeId=" + int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()) + "");
        }

        DataTable dtgetemprateid = comm.GetAllDataFromTable("RateDetails", "top 1 RateId", "EmployeeId='" + EmpEntity.EmployeeID + "'", "RateId asc");
        comm.ExecuteQuery("update RateDetails set StartDate='" + EmpEntity.HireDate.ToString("yyyy-MM-dd") + "', effectivedate='" + EmpEntity.HireDate.ToString("yyyy-MM-dd") + "' where RateId = '" + int.Parse(dtgetemprateid.Rows[0][0].ToString()) + "' and EmployeeId='" + EmpEntity.EmployeeID + "'");

        SaveAddress();
        Response.Redirect("Home_Page.aspx");
    }

    protected void GridView_User_Role_Permission_Pop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            DropDownList DropDownList_User_Role_Permission_Access_Status = e.Row.FindControl("DropDownList_User_Role_Permission_Access_Status") as DropDownList;
            HiddenField hfdUserTypeId = e.Row.FindControl("hfdUserTypeId") as HiddenField;
           var utype = ViewState["Popuprole"].ToString();
            if (hfdUserTypeId.Value == utype)
            {
                DropDownList_User_Role_Permission_Access_Status.SelectedValue = "YES";
            }
           
        }
    }

    protected void ddlRoleNamepop_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridView_User_Role_Permission_Pop.DataSource = new DataTable();
        GridView_User_Role_Permission_Pop.DataBind();

        ViewState["Popuprole"] = ddlRoleNamepop.SelectedValue;
        GetUserPermissionRoleList(0);
    }
}