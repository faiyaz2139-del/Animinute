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
    DataTable dtSelectProject = new DataTable();
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
            InitialSelectBuss();
            if (!string.IsNullOrEmpty(Request.QueryString["BusinessID"]))
            {
                ViewState["bid"] = Request.QueryString["BusinessID"].ToString();
            }
            else
            {
                var dt = comm.GetAllDataFromTable("AssginBussiness", "BusinessID", "EmployeeID=" + Session["EmployeeId"].ToString() + " and BusinessID is not null and BusinessID<>''", "");
                if (dt.Rows.Count > 0)
                {
                    ViewState["bid"] = dt.Rows[0][0].ToString();
                }
                else
                {
                    ViewState["bid"] = 0;
                }
            }
            Bindbusiness();
            BindAllUserType();

            if (Request.QueryString["EmployeeIDEdit"] != null)
            {
                txtMainEmail.ReadOnly = true;
                //hfdEmpID.Value = Request.QueryString["EmployeeIDEdit"].ToString();
                //LinkButton_EditEmployee.Visible = true;
                GetChangeRateRole(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                GetPersonalDetailsForEdit(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                Get_User_Role_Permission_While_Editing_Employee(int.Parse(Request.QueryString["EmployeeIDEdit"].ToString()));
                GetRateInformation(Request.QueryString["EmployeeIDEdit"].ToString());
                btnFinalSave.Visible = false;
                //btnSaveNewRole.Visible = false;
                //btnSendBuss.Visible = false;
                //btnBackBuss.Visible = false;
            }

        }
        HttpCookie aCookie = Request.Cookies["tabid"];
        if (aCookie != null)
        {
            var cookvalue = aCookie.Value;
            if (cookvalue == "#home")
            {
                home.Attributes.Add("class", "tab-pane fade in active show");
                permission.Attributes.Add("class", "tab-pane fade");
                ratedetils.Attributes.Add("class", "tab-pane fade");
                project.Attributes.Add("class", "tab-pane fade");
                // $("#home").attr("class", "tab-pane fade in active show");
            }
            else if (cookvalue == "#permission")
            {
                permission.Attributes.Add("class", "tab-pane fade in active show");
                project.Attributes.Add("class", "tab-pane fade in ");
                home.Attributes.Add("class", "tab-pane fade in");
                ratedetils.Attributes.Add("class", "tab-pane fade in");
                LiRateDetails.Attributes.Add("class", "");
                Lirolemid.Attributes.Add("class", "active");
                Lihomemid.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "");
            }
            else if (cookvalue == "#ratedetils")
            {
                permission.Attributes.Add("class", "tab-pane fade in");
                project.Attributes.Add("class", "tab-pane fade in");
                home.Attributes.Add("class", "tab-pane fade in");
                ratedetils.Attributes.Add("class", "tab-pane fade in active show");
                LiRateDetails.Attributes.Add("class", "");
                Lirolemid.Attributes.Add("class", "");
                Lihomemid.Attributes.Add("class", "");
            }
            else if (cookvalue == "#project")
            {
                project.Attributes.Add("class", "tab-pane fade in active show");
                permission.Attributes.Add("class", "tab-pane fade in");
                home.Attributes.Add("class", "tab-pane fade in");
                //ratedetils.Attributes.Add("class", "tab-pane fade in");
                LiRateDetails.Attributes.Add("class", "");
                LiProject.Attributes.Add("class", "active");
                Lihomemid.Attributes.Add("class", "");
                Lirolemid.Attributes.Add("class", "");
            }
        }

    }
    public void Bindbusiness()
    {
        if (!string.IsNullOrEmpty(Session["EmployeeId"].ToString()))
        {
            var empid = Session["EmployeeId"].ToString();
            DataTable dt = AddBeCtrl.GetBussForSelectBUSS(int.Parse(empid), int.Parse(Session["UserTypeid"].ToString()));
            if (dt.Rows.Count > 0)
            {

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);
                ddlBusiness.DataSource = ds;

                ddlBusiness.DataValueField = ds.Tables[0].Columns[1].ToString();
                ddlBusiness.DataTextField = ds.Tables[0].Columns[2].ToString();
                ddlBusiness.DataBind();
                ddlBusiness.SelectedIndex = 0;
            }


        }
    }
    protected void BindAllUserType()
    {
        var uid = Session["UserId"].ToString();
        var chekMang = comm.IfExistsInTable("MappingTable", "*", "userid=" + uid + " and UserTypeId=2");
        if (chekMang == false)
        {
            if (ViewState["bid"].ToString() != "0")
            {
                var dt = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));

              
                ddlRole.DataSource = dt;
                ddlRole.DataValueField = dt.Columns[0].ToString();
                ddlRole.DataTextField = dt.Columns[1].ToString();
                ddlRole.DataBind();

               
                ddlEditRolePer.DataSource = dt;
                ddlEditRolePer.DataValueField = dt.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dt.Columns[1].ToString();
                ddlEditRolePer.DataBind();

            }
            else
            {

                var dt = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));

                ddlRole.DataSource = dt;
                ddlRole.DataValueField = dt.Columns[0].ToString();
                ddlRole.DataTextField = dt.Columns[1].ToString();
                ddlRole.DataBind();

                ddlEditRolePer.DataSource = dt;
                ddlEditRolePer.DataValueField = dt.Columns[0].ToString();
                ddlEditRolePer.DataTextField = dt.Columns[1].ToString();
                ddlEditRolePer.DataBind();
            }
        }
        else
        {
            DataTable dtmgr = new DataTable();
            if (ViewState["bid"].ToString() != "0")
            {
                DataTable dtMuid = new DataTable();
                dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
                DataRow[] selectRow = dtmgr.Select("[User_TypeId]=4");
                DataTable dtmanuType = selectRow.CopyToDataTable();


                ddlRole.DataSource = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));
                ddlRole.DataBind();
            }
            else
            {
                DataTable dtMuid = new DataTable();
                dtmgr = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
                DataRow[] selectRow = dtmgr.Select("[User_TypeId]=4");
                DataTable dtmanuType = selectRow.CopyToDataTable();



                ddlRole.DataSource = ut.GetUserTypeCompanyWise(int.Parse(Session["EmployeeId"].ToString()));
                ddlRole.DataBind();
            }

        }


        DataTable Utpe = new DataTable();

        Utpe = ut.GetUserTypeCompanyWise(int.Parse(ViewState["bid"].ToString()));
        if (chekMang == false)
        {

            //ddlRoleNamepop.Items.Clear();
            //ddlRoleNamepop.DataSource = Utpe;
            //ddlRoleNamepop.DataValueField = Utpe.Columns[0].ToString();
            //ddlRoleNamepop.DataTextField = Utpe.Columns[1].ToString();
            //ddlRoleNamepop.DataBind();


            //ddlCatagoryRole.Items.Add("Please Select");
            //ddlCatagoryRole.SelectedItem.Value = "";
            //ddlCatagoryRole.DataSource = Utpe;
            //ddlCatagoryRole.DataValueField = Utpe.Columns[0].ToString();
            //ddlCatagoryRole.DataTextField = Utpe.Columns[1].ToString();
            //ddlCatagoryRole.DataBind();
            //ddlCatagoryRole.SelectedIndex = 0;
        }
        else
        {
            //foreach (ListItem itm in rdoBtnUserTypeList.Items)
            //{
            //    if(!itm.Value.ToString().Equals("4"))
            //    {
            //        //itm.Attributes.Add("style", "visibility: hidden");
            //        itm.Enabled = false;
            //    }
            //    else
            //    {
            //        itm.Selected = true;
            //    }
            //}
            DataRow[] selectRow = Utpe.Select("[User_TypeId]=4");
            DataTable dtmanuType = selectRow.CopyToDataTable();
            //ddlRoleNamepop.Items.Clear();
            //ddlRoleNamepop.DataSource = dtmanuType;
            //ddlRoleNamepop.DataValueField = dtmanuType.Columns[0].ToString();
            //ddlRoleNamepop.DataTextField = dtmanuType.Columns[1].ToString();
            //ddlRoleNamepop.DataBind();


            //ddlCatagoryRole.Items.Add("Please Select");
            //ddlCatagoryRole.SelectedItem.Value = "";
            //ddlCatagoryRole.DataSource = dtmanuType;
            //ddlCatagoryRole.DataValueField = dtmanuType.Columns[0].ToString();
            //ddlCatagoryRole.DataTextField = dtmanuType.Columns[1].ToString();
            //ddlCatagoryRole.DataBind();
            //ddlCatagoryRole.SelectedIndex = 0;
        }



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
                    DataTable dtUserId = comm.GetAllDataFromTable("UserAcc", "UserId", "Email='" + txtMainEmail.Text + "'", "");
                    ViewState["ViweStateUserID"] = dtUserId.Rows[0]["UserId"].ToString();
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
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Please enter valid postal code in format A#A #A# to search', 'error');", true);
            return;
        }
        string _serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        if (_serachpostcode.Length.ToString() != "6")
        {
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

    protected void btnNextPerson_Click(object sender, EventArgs e)
    {

    }

    protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlRole.SelectedValue == "0")
        {
            ModalAddRolePop.Show();
            GetUserPermissionRoleList(0, GridView_User_Role_Permission_Pop);
        }
    }

    public void GetUserPermissionRoleList(int uid, GridView GvDataGridView)
    {
        if (uid != 0)
        {
            comm.FillGridView(GvDataGridView, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status,UserTypeId", "UserTypeId=" + uid + "", "Role_Permission_Id asc");
        }
        else
        {
            comm.FillGridView(GvDataGridView, "Role_Permission_Master", "Role_Permission_Id", "Role_Permission_Name, Status,UserTypeId", "", "Role_Permission_Id asc");
        }
    }



    protected void btnAdd_Click(object sender, EventArgs e)
    {
        int cnt = 0;
        if (string.IsNullOrEmpty(txtFirstName.Text) || string.IsNullOrEmpty(txtLastName.Text))
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either First or Last Name is Blank', 'error');", true);
            return;
        }
        if (ddlBusiness.SelectedValue == "" || ddlRole.SelectedValue == "" || ddlRole.SelectedValue == "0")
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Either Business Name or Role is Not Select', 'error');", true);
            return;
        }
        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            if(hfd_BussinessId.Value== ddlBusiness.SelectedValue)
            {
                cnt++;
            }
        }
        if(cnt>0)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "SweetAlert", "swal('Error!', 'Business Already Addedd', 'error');", true);
            return;
        }

            DataTable dt = (DataTable)ViewState["RolePermission"];
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusiness.SelectedItem.Text;
        var rolename = ddlRole.SelectedItem.Text;
        StringBuilder StringBuilder__ = new StringBuilder();
        var AllRole = comm.ExecuteQueryDataTable("select Role_Permission_Id FROM Role_Permission_Master WHERE UserTypeId='" + ddlRole.SelectedValue + "'");
        for (int i = 0; i < AllRole.Rows.Count; i++)
        {
            StringBuilder__.Append(AllRole.Rows[i][0].ToString() + ",");
        }

        dt.Rows.Add(bussname, ddlBusiness.SelectedValue, EmpName, rolename, ddlRole.SelectedValue, StringBuilder__.ToString());
        ViewState["RolePermission"] = dt;
        BindGrid();

        ddlBusAssProject.Items.Clear();
        ddlBusAssProject.Items.Add("Please Select");
        ddlBusAssProject.SelectedItem.Value = "";
        ddlBusAssProject.DataSource = dt;
        ddlBusAssProject.DataValueField = dt.Columns[1].ToString();
        ddlBusAssProject.DataTextField = dt.Columns[0].ToString();
        ddlBusAssProject.DataBind();
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

    public void InitialSelectBuss()
    {
        dtSelectedBuss.Columns.AddRange(new DataColumn[6] { new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("UserRole"), new DataColumn("UserRoleId"), new DataColumn("RoleId") });
        ViewState["RolePermission"] = dtSelectedBuss;
        this.BindGrid();
        dtSelectProject.Columns.AddRange(new DataColumn[5] { new DataColumn("BusinessName"), new DataColumn("BussinessId"), new DataColumn("EmployeeName"), new DataColumn("ProjectName"), new DataColumn("ProjectId") });
        ViewState["ProjetName"] = dtSelectProject;
        BindProjectGrid();
    }
    protected void BindGrid()
    {
        Gv_RolePermission.DataSource = (DataTable)ViewState["RolePermission"];
        Gv_RolePermission.DataBind();
        GvRateDetails.DataSource = (DataTable)ViewState["RolePermission"];
        GvRateDetails.DataBind();
    }

    protected void BindProjectGrid()
    {
        GvProject.DataSource = (DataTable)ViewState["ProjetName"];
        GvProject.DataBind();
        //Gv_RolePermission.DataSource = (DataTable)ViewState["ProjetName"];
        //Gv_RolePermission.DataBind();
    }

    protected void Gv_RolePermission_RowEditing(object sender, GridViewEditEventArgs e)
    {
        Gv_RolePermission.EditIndex = e.NewEditIndex;
        BindGrid();
    }

    protected void Gv_RolePermission_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        Gv_RolePermission.EditIndex = -1;
        BindGrid();
    }

    protected void Gv_RolePermission_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfd_UserRoleIdEdit = (e.Row.FindControl("hfd_UserRoleIdEdit") as HiddenField);
            HiddenField hfd_UserRoleId = (e.Row.FindControl("hfd_UserRoleId") as HiddenField);
            LinkButton lnk_ChangeRoleName = (e.Row.FindControl("lnk_ChangeRoleName") as LinkButton);
            if (hfd_UserRoleIdEdit != null)
            {
                if (hfd_UserRoleIdEdit.Value == "2" || hfd_UserRoleIdEdit.Value == "4" || hfd_UserRoleIdEdit.Value == "5" || hfd_UserRoleIdEdit.Value == "12")
                {
                    lnk_ChangeRoleName.Visible = false;

                }
            }
            else
            {
                if (hfd_UserRoleId.Value == "2" || hfd_UserRoleId.Value == "4" || hfd_UserRoleId.Value == "5" || hfd_UserRoleId.Value == "12")
                {
                    lnk_ChangeRoleName.Visible = false;

                }
            }

        }
    }

    protected void Gv_RolePermission_PreRender(object sender, EventArgs e)
    {

        if (Gv_RolePermission.Rows.Count > 0)
        {
            Gv_RolePermission.UseAccessibleHeader = true;
            Gv_RolePermission.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }



    protected void Gv_RolePermission_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditPermission")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = Gv_RolePermission.Rows[rowIndex];
            HiddenField hfd_UserRoleId = (row.FindControl("hfd_UserRoleId") as HiddenField); 
            HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
            ViewState["BussIdEditRole"] = hfd_BussinessId.Value;
            ddlEditRolePer.SelectedValue = hfd_UserRoleId.Value;
            GetUserPermissionRoleList(0, Gv_EditRolePop);
            ModallEditRolePop.Show();


        }
        if(e.CommandName== "DeletePermission")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = Gv_RolePermission.Rows[rowIndex];
            HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
            DataTable dt = (DataTable)ViewState["RolePermission"];
            DataRow[] rowsToRemove = dt.Select("BussinessId=" + hfd_BussinessId.Value + "");
            foreach (var rowToDelete in rowsToRemove)
                dt.Rows.Remove(rowToDelete);
            ViewState["RolePermission"] = dt;
            this.BindGrid();
            ddlBusAssProject.Items.Clear();
            ddlBusAssProject.Items.Add("Please Select");
            ddlBusAssProject.SelectedItem.Value = "";
            ddlBusAssProject.DataSource = dt;
            ddlBusAssProject.DataValueField = dt.Columns[1].ToString();
            ddlBusAssProject.DataTextField = dt.Columns[0].ToString();
            ddlBusAssProject.DataBind();

        }
    }

    protected void Gv_EditRolePop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdUserTypeId = e.Row.FindControl("hfdUserTypeId") as HiddenField;
            DropDownList DropDownList_User_Role_Permission_Access_Status_Edit = e.Row.FindControl("DropDownList_User_Role_Permission_Access_Status_Edit") as DropDownList;
            var selectUserTypid = ddlEditRolePer.SelectedValue;
            var ddlselect = ddlEditRolePer.SelectedValue;
            if (selectUserTypid == hfdUserTypeId.Value)
            {
               
                DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue = "YES";
            }
            if(ddlselect == "1"|| ddlselect == "2"|| ddlselect == "4"|| ddlselect == "5"|| ddlselect == "12")
            {
                DropDownList_User_Role_Permission_Access_Status_Edit.Enabled = false;
            }

        }
    }

    protected void ddlBusAssProject_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(ddlBusAssProject.SelectedValue))
        {
            GetProject();
        }

    }
    public void GetProject()
    {
        DataTable dtProject = comm.GetAllDataFromTable("AddProject", "ProjectID,ProjectName", "BusinessID=" + ddlBusAssProject.SelectedValue + "", "ProjectID");
        ddlAssProjectName.Items.Clear();
        ddlAssProjectName.DataSource = dtProject;
        ddlAssProjectName.Items.Insert(0, new ListItem("Select Project ", ""));
        ddlAssProjectName.Items.Insert(1, new ListItem("Add New Project ", "0"));

        ddlAssProjectName.DataValueField = dtProject.Columns[0].ToString();
        ddlAssProjectName.DataTextField = dtProject.Columns[1].ToString();
        ddlAssProjectName.DataBind();
        ddlAssProjectName.SelectedValue = "";
        if (dtProject.Rows.Count == 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'No Project Found', 'warning');", true);
        }
    }
    protected void GvProject_RowEditing(object sender, GridViewEditEventArgs e)
    {
        GvProject.EditIndex = e.NewEditIndex;
        BindProjectGrid();
    }

    protected void GvProject_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        GvProject.EditIndex = -1;
        BindProjectGrid();
    }

    protected void GvProject_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GvProject_PreRender(object sender, EventArgs e)
    {

    }

    protected void GvProject_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if(e.CommandName== "DeleteProject")
        {
            int rowIndex = Convert.ToInt32(e.CommandArgument.ToString());
            GridViewRow row = GvProject.Rows[rowIndex];
            HiddenField hfd_BussinessId = (row.FindControl("hfd_BussinessId") as HiddenField);
            HiddenField hfd_ProjectNameId = (row.FindControl("hfd_ProjectNameId") as HiddenField);
            DataTable dt = (DataTable)ViewState["ProjetName"];
            DataRow[] rowsToRemove = dt.Select("BussinessId=" + hfd_BussinessId.Value + "and ProjectId="+ hfd_ProjectNameId.Value+ "");
            foreach (var rowToDelete in rowsToRemove)
                dt.Rows.Remove(rowToDelete);
            ViewState["ProjetName"] = dt;
            this.BindProjectGrid();
        }
    }

    protected void btnAssProject_Click(object sender, EventArgs e)
    {
        DataTable dt = (DataTable)ViewState["ProjetName"];
        var EmpName = txtFirstName.Text + " " + txtLastName.Text;
        var bussname = ddlBusAssProject.SelectedItem.Text;
        var projectName = ddlAssProjectName.SelectedItem.Text;
        dt.Rows.Add(bussname, ddlBusAssProject.SelectedValue, EmpName, projectName, ddlAssProjectName.SelectedValue);
        ViewState["ProjetName"] = dt;
        BindProjectGrid();
    }

    protected void ddlAssProjectName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlAssProjectName.SelectedValue == "0")
        {
            ModelAddNewProject.Show();

        }

    }
    public int SavePersonalDetails(int UserType, string userTypetext)
    {
        int ResultSavePersonalDetails = 0;
        try
        {
            EmpEntity.SINExpiry = txtSINExpiry.Text;
            EmpEntity.UserType = UserType;
            string userTypeUserACC = userTypetext;
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


            if (ddlEmpStatus.SelectedValue == "NO")
            {
                EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            }
            else
            {
                EmpEntity.Apartment = DateTime.Parse(DateTime.Now.ToString()).ToString();//Inactivedate
            }

            EmpEntity.TerminationDate = DateTime.Now.Date;

            EmpEntity.MotherOrgID = int.Parse(Session["MotherOrgid"].ToString());

            EmpEntity.Occupation = "";

            int Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, userTypeUserACC, RateEntity);

            ResultSavePersonalDetails = Empid;



            if (string.IsNullOrEmpty(Convert.ToString(ViewState["profile"])))
            {
                ViewState["profile"] = "";
            }



            return ResultSavePersonalDetails;
        }
        catch (Exception ex)
        {
            return ResultSavePersonalDetails;
            throw ex;
        }
    }
    public int SaveProfilePic(int empid, string str, int UserTypeid, string UserType)
    {
        Uae.EmployeeId = empid;
        Uae.Email = txtMainEmail.Text.Trim();
        Uae.MotherOrgid = Convert.ToInt32(Session["MotherOrgid"].ToString());
        Uae.UserTypeid = UserTypeid;
        Uae.UserType = UserType;
        Uae.ProfileImageName = ViewState["profile"].ToString();

        return Convert.ToInt32(Uac.Sp_InsertUserAccFromAddress(Uae));
    }
    public int SaveAddress(int _Empid)
    {
        int ResultSaveAddres = 0;
        DataTable cityid = new DataTable();
        DataTable province = new DataTable();
        try
        {
            bool chk = comm.IfExistsInTable("CityMaster", "CityName", "CityName='" + txtCity.Text + "'");
            if (chk == false)
            {
                comm.ExecuteQuery("insert into CityMaster (CityName,Duplicate)values('" + txtCity.Text + "','False')");
            }

            bool chkProvinceMaster = comm.IfExistsInTable("ProvinceMaster", "ProvinceName", "ProvinceName='" + txtProvince.Text + "'");
            if (chkProvinceMaster == false)
            {
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
            EmpAddEnity.SuitType = "";
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
            RateEntity.IsEmployeeEIExempty = "";
            RateEntity.IsEmployeeCPPExempty = "";
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
            RateEntity.RetainVacation = "";
            EmpAddEnity.MotherOrgId = int.Parse(Session["MotherOrgid"].ToString()); //int.Parse(hfdMotherId.Value);

            ResultSaveAddres = EmpAddCtrl.InsertEmpAddress(EmpAddEnity, _Empid);

            return ResultSaveAddres;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public void SaveUpdateRolePermission(int empid, int bussid, int usertype, string permission)
    {
        bool check_emp_id_in_user_Role_permission = comm.IfExistsInTable("User_Role_Permission", "Employee_Id", "Employee_Id='" + empid + "'");

        if (check_emp_id_in_user_Role_permission == true)
        {

            comm.ExecuteQuery("update User_Role_Permission set Role_Permission_Id='" + permission + "', BusinessId='" + bussid + "'  where Employee_Id ='" + empid + "' and UserTypeId = '" + usertype + "' and Status='True' ");
        }
        else
        {

            comm.ExecuteQuery("insert into User_Role_Permission (Role_Permission_Id, Employee_Id, UserTypeId, Status, BusinessId) Values ('" + permission + "', '" + empid + "', '" + usertype + "', 'True', '" + bussid + "')");
        }
    }

    public void SaveAssgineProject(int bussid, int EmpId)
    {
        foreach (GridViewRow row in GvProject.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_ProjectNameId = row.FindControl("hfd_ProjectNameId") as HiddenField;
            Label lbl_ProjectName = row.FindControl("lbl_ProjectName") as Label;

            if (int.Parse(hfd_BussinessId.Value) == bussid)
            {
                ape.BusinessID = Convert.ToInt32(hfd_BussinessId.Value);
                ape.EmployeeID = Convert.ToInt32(EmpId);
                ape.ProjectID = Convert.ToInt32(hfd_ProjectNameId.Value);
                apc.InsertAssgineProject(ape);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='P' WHERE EmployeeID=" + EmpId + "");
            }

        }
    }
    public int SavePay(int _empid)
    {
        int ResultSavePay = 0;
        try
        {

            foreach (GridViewRow row in GvRateDetails.Rows)
            {

                HiddenField hfd_BussinessId = row.FindControl("BussinessId") as HiddenField;
                TextBox txtstdate = row.FindControl("txtstdate") as TextBox;
                TextBox txtEndDate = row.FindControl("txtEndDate") as TextBox;
                TextBox txtRate = row.FindControl("txtRate") as TextBox;
                DropDownList ddlPaytpe = row.FindControl("ddlPaytpe") as DropDownList;
                DropDownList ddlEmpEI11 = row.FindControl("ddlEmpEI11") as DropDownList;
                DropDownList ddlEmpEI22 = row.FindControl("ddlEmpEI22") as DropDownList;
                DropDownList ddlEmpEI33 = row.FindControl("ddlEmpEI33") as DropDownList;

                RateEntity.EmployeeId = _empid;
                RateEntity.Rate = float.Parse(txtRate.Text);
                RateEntity.RateType = ddlPaytpe.SelectedItem.Text;
                //RateEntity.StartDate = DateTime.Parse(txteffectivedate.Text);
                RateEntity.StartDate = DateTime.Parse(txtstdate.Text);
                RateEntity.EndDate = System.DateTime.Now.Date;
                RateEntity.JobTitle = "";
                RateEntity.IsEmployeeEIExempty = ddlEmpEI11.SelectedValue;
                RateEntity.IsEmployeeCPPExempty = ddlEmpEI22.SelectedValue;
                RateEntity.RetainVacation = ddlEmpEI33.SelectedValue;
                //RateEntity.effectivedate = DateTime.Parse(txteffectivedate.Text);
                RateEntity.effectivedate = DateTime.Parse(txtHiredate.Text);
                ResultSavePay = RateCtrl.InsertRateDetails(RateEntity, _empid);
                comm.ExecuteQuery("update AddEmployee set Rateid=" + ResultSavePay + " where EmployeeID=" + _empid + "");

            }



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
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        int _Empid = 0, _AddressID;
        foreach (GridViewRow row in Gv_RolePermission.Rows)
        {
            HiddenField hfd_BussinessId = row.FindControl("hfd_BussinessId") as HiddenField;
            HiddenField hfd_RoleId = row.FindControl("hfd_RoleId") as HiddenField;
            Label lbl_BusinessName = row.FindControl("lbl_BusinessName") as Label;
            HiddenField hfd_UserRoleId = row.FindControl("hfd_UserRoleId") as HiddenField;
            Label lbl_UserRole = row.FindControl("lbl_UserRole") as Label;
            _Empid = SavePersonalDetails(Convert.ToInt32(hfd_UserRoleId.Value), lbl_UserRole.Text);
            SavePay(_Empid);
            var UserId = SaveProfilePic(_Empid, ViewState["profile"].ToString(), int.Parse(hfd_UserRoleId.Value), lbl_UserRole.Text);
            _AddressID = SaveAddress(_Empid);
            string EmployeeName = comm.GetNameFromId("AddEmployee", _Empid);
            AddBeCtrl.AssgineBusiness(Convert.ToInt32(hfd_BussinessId.Value), _Empid, lbl_BusinessName.Text, EmployeeName, Convert.ToInt32(hfd_UserRoleId.Value), Convert.ToInt32(Session["MotherOrgid"].ToString()));
            SaveUpdateRolePermission(_Empid, int.Parse(hfd_BussinessId.Value), int.Parse(hfd_UserRoleId.Value), hfd_RoleId.Value);
            SaveAssgineProject(int.Parse(hfd_BussinessId.Value), _Empid);
            comm.ExecuteQuery("insert into MappingTable(USERID,UserTypeId,EmployeeID,BusinessID)values(" + UserId + "," + int.Parse(hfd_UserRoleId.Value) + "," + _Empid + "," + hfd_BussinessId.Value + ")");
            comm.ExecuteQuery("UPDATE AssginBussiness SET UserTypeId=" + hfd_UserRoleId.Value + " WHERE EmployeeID=" + _Empid + " AND BusinessID=" + hfd_BussinessId.Value + "");

            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved', 'success');", true);
            Response.Redirect("Home_Page.aspx");
        }
    }

    protected void ddlEditRolePer_SelectedIndexChanged(object sender, EventArgs e)
    {
        GetUserPermissionRoleList(0, Gv_EditRolePop);
    }



    protected void btnSaveEditRolePop_Click(object sender, EventArgs e)
    {
        StringBuilder StringBuilder__ = new StringBuilder();
        foreach (GridViewRow row in Gv_EditRolePop.Rows)
        {
            DropDownList DropDownList_User_Role_Permission_Access_Status_Edit = row.FindControl("DropDownList_User_Role_Permission_Access_Status_Edit") as DropDownList;
            HiddenField HiddenField_Role_Permission_Id = row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
            if (DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue == "YES")
            {
                StringBuilder__.Append(HiddenField_Role_Permission_Id.Value + ",");
            }
        }
        if(string.IsNullOrEmpty(StringBuilder__.ToString()))
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Select Atleast One Role', 'error');", true);
            return;
        }
        DataTable dt = (DataTable)ViewState["RolePermission"];
        for(int i=0;i<dt.Rows.Count;i++)
        {
            if(dt.Rows[i]["BussinessId"].ToString()== ViewState["BussIdEditRole"].ToString())
            {
                dt.Rows[i]["RoleId"] = StringBuilder__.ToString();
                dt.Rows[i]["UserRole"] = ddlEditRolePer.SelectedItem.Text;
                dt.Rows[i]["UserRoleId"] = ddlEditRolePer.SelectedValue;
            }
        }
        dt.AcceptChanges();
        ViewState["RolePermission"] = dt;
        this.BindGrid();


    }

    protected void btnsaveRolePop_Click(object sender, EventArgs e)
    {
        if(ddlBusiness.SelectedValue=="")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business', 'warning');", true);
            return;
        }
        if(txtRoleNamePop.Text=="")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Enter Role', 'warning');", true);
            return;
        }
        //StringBuilder StringBuilder__ = new StringBuilder();
        //foreach (GridViewRow row in GridView_User_Role_Permission_Pop.Rows)
        //{
        //    DropDownList DropDownList_User_Role_Permission_Access_Status_Edit = row.FindControl("DropDownList_User_Role_Permission_Access_Status_Edit") as DropDownList;
        //    HiddenField HiddenField_Role_Permission_Id = row.FindControl("HiddenField_Role_Permission_Id") as HiddenField;
        //    if (DropDownList_User_Role_Permission_Access_Status_Edit.SelectedValue == "YES")
        //    {
        //        StringBuilder__.Append(HiddenField_Role_Permission_Id.Value + ",");
        //    }
        //}
        comm.ExecuteQuery("insert into UserType(UserType,MotherOrgId,BownerID)values('" + txtRoleNamePop.Text + "',0," + int.Parse(Session["EmployeeId"].ToString()) + ")");
        BindAllUserType();
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Role Added Successfull', 'success');", true);
    }

    protected void btnSaveProject_Click(object sender, EventArgs e)
    {
        if (ddlBusAssProject.SelectedValue == "")
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Select Business', 'warning');", true);
            return;
        }
        comm.ExecuteQuery("insert into AddProject(ProjectName,startDate,EndDate,BusinessID,Status)values('" + txtAddProject.Text + "','"+ txtProjectStDate.Text + "'," + txtProjectEndDate.Text + ","+ ddlBusAssProject.SelectedValue+ ",'A')");
        GetProject();
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Project Added Successfull', 'success');", true);
    }

    protected void Gv_RolePermission_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow row = Gv_RolePermission.Rows[e.RowIndex];
        HiddenField hfd_UserRoleIdEdit = (HiddenField)row.FindControl("hfd_UserRoleIdEdit");
        TextBox txt_UserRole = (TextBox)row.FindControl("txt_UserRole");
        comm.ExecuteQuery("update UserType set UserType='"+ txt_UserRole.Text + "' where User_TypeId=" + hfd_UserRoleIdEdit.Value + "");
        BindAllUserType();
        DataTable dt = (DataTable)ViewState["RolePermission"];
        for(int i=0;i<dt.Rows.Count;i++)
        {
            if(dt.Rows[i]["UserRoleId"].ToString()== hfd_UserRoleIdEdit.Value)
            {
                dt.Rows[i]["UserRole"] = txt_UserRole.Text;
            }
        }
        dt.AcceptChanges();
       
        ViewState["RolePermission"] = dt;
        this.BindGrid();
        ddlRole.SelectedValue = hfd_UserRoleIdEdit.Value;
        Gv_RolePermission.EditIndex = -1;
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Role Updated Successfull', 'success');", true);


    }

    protected void GvProject_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        GridViewRow row = GvProject.Rows[e.RowIndex];
        HiddenField hfd_ProjectNameIdEdit = (HiddenField)row.FindControl("hfd_ProjectNameIdEdit");
        TextBox txt_ProjectName = (TextBox)row.FindControl("txt_ProjectName");
        comm.ExecuteQuery("update AddProject set ProjectName='" + txt_ProjectName.Text + "' where ProjectID=" + hfd_ProjectNameIdEdit.Value + "");
        if (!string.IsNullOrEmpty(ddlBusAssProject.SelectedValue))
        {
            GetProject();
        }
        DataTable dt = (DataTable)ViewState["ProjetName"];
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["ProjectID"].ToString() == hfd_ProjectNameIdEdit.Value)
            {
                dt.Rows[i]["ProjectName"] = txt_ProjectName.Text;
            }
        }
        dt.AcceptChanges();

        ViewState["ProjetName"] = dt;
        GvProject.EditIndex = -1;
        this.BindProjectGrid();
        //ddlAssProjectName.SelectedValue = hfd_ProjectNameIdEdit.Value;
        
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Project Updated Successfull', 'success');", true);
    }
    public void GetPersonalDetailsForEdit(int EmployeeID)
    {
        //DataTable dtPersonalDetailsForEdit = comm.GetAllDataFromTable("AddEmployee", "*,convert(nvarchar(10),HireDate,120)HireDated", "EmployeeID=" + Request.QueryString["EmployeeIDEdit"].ToString() + "", "");
        //foreach (ListItem itm in rdoBtnUserTypeList.Items)
        //{
        //    if (itm.Value == dtPersonalDetailsForEdit.Rows[0]["UserType"].ToString())
        //    {
        //        itm.Selected = true;
        //    }
        //}

        //if (dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString() != "")
        //    txtSINExpiry.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["SINExpiry"].ToString()).ToString("yyyy-MM-dd");

        //txtFirstName.Text = dtPersonalDetailsForEdit.Rows[0]["FirstName"].ToString();
        //txtMiddleName.Text = dtPersonalDetailsForEdit.Rows[0]["MiddleName"].ToString();
        //txtLastName.Text = dtPersonalDetailsForEdit.Rows[0]["LastName"].ToString();
        //hfdSocialEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["SocialInsuranceNumber"].ToString();
        //ddlEmpEI.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeEIExempt"].ToString().Trim();
        //ddlEmpCPP.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsEmployeeCPPExempt"].ToString().Trim();
        //ddlRTVA.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["RetainVacation"].ToString().Trim();

        //if (dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString() != "")
        //{
        //    hfddboEncrpty.Value = dtPersonalDetailsForEdit.Rows[0]["DateOfBirth"].ToString();
        //}
        //txtSocialNumer.Text = comm.Decrypt(hfdSocialEncrpty.Value);
        //string dbo = comm.Decrypt(hfddboEncrpty.Value);
        //if (dbo != "")
        //{
        //    txtdob.Text = DateTime.Parse(dbo).ToString("yyyy-MM-dd");
        //}

        //ddlgender.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["Gender"].ToString();
        //if (dtPersonalDetailsForEdit.Rows[0]["HireDate"].ToString() != "")
        //{
        //    DateTime HireDate = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["HireDated"].ToString());
        //    txtHiredate.Text = HireDate.ToString("yyyy-MM-dd");
        //}

        //if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
        //    txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        //ddlEmpStatus.SelectedValue = dtPersonalDetailsForEdit.Rows[0]["IsActive"].ToString().Trim();
        //if (ddlEmpStatus.SelectedValue == "NO")
        //{
        //    if (dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString() != "")
        //        txtInActiveDate.Text = DateTime.Parse(dtPersonalDetailsForEdit.Rows[0]["Apartment"].ToString()).ToString("yyyy-MM-dd");
        //}
        //if (dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString() != "")
        //{
        //    divAddres.Attributes.Add("style", "display:block;");
        //    GetAddressDetails(int.Parse(dtPersonalDetailsForEdit.Rows[0]["ADDRESSID"].ToString()));
        //}
        //if (dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString() != "")
        //{
        //    GetPaymentForEdit(int.Parse(dtPersonalDetailsForEdit.Rows[0]["RateId"].ToString()));
        //}
        //GetRateInformation(hfdEmpID.Value);
    }

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

    public void Get_User_Role_Permission_While_Editing_Employee(int EmployeeID)
    {
        //DataTable Dt_Check_Role_Permission = new DataTable();
        //Dt_Check_Role_Permission = comm.GetAllDataFromTable("User_Role_Permission", "Role_Permission_Id,Employee_Id,UserTypeId,Status", "Employee_Id='" + Request.QueryString["EmployeeIDEdit"].ToString() + "' ", "");
        ////Checking if the old employee are not there in User_Role_Permission table or not. Only new employees role will be check from User_Role_Permission table for this functionality.
        //if (Dt_Check_Role_Permission.Rows.Count > 0)
        //{
        //    foreach (GridViewRow row in GridView_User_Role_Permission.Rows)
        //    {
        //        string string_Check_Role_Permission = Dt_Check_Role_Permission.Rows[0]["Role_Permission_Id"].ToString();
        //        string[] splittedArray = string_Check_Role_Permission.Split(',');
        //        HiddenField Role_Permission_Id_ = (HiddenField)row.FindControl("HiddenField_Role_Permission_Id");
        //        DropDownList ddl_Role_Permission_Access_Status_ = (DropDownList)row.FindControl("DropDownList_User_Role_Permission_Access_Status");
        //        foreach (string s in splittedArray)
        //        {
        //            if (Role_Permission_Id_.Value == s)
        //            {
        //                ddl_Role_Permission_Access_Status_.SelectedValue = "YES";
        //            }
        //        }
        //    }
        //}
        //else
        //{
        //    //Div_User_Role_Permission.Visible = false;
        //}
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
}