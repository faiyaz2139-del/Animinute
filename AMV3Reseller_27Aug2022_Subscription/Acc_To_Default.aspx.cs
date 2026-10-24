using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Acc_To_Default : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
    ActiveDeactiveController ADCtrl = new ActiveDeactiveController();
    LoginClass Loginc = new LoginClass();
    int mangercheck = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetEmplist();
            DisableControls(Page, false);
            EnableEmplist();
            btnback.NavigateUrl = Request.UrlReferrer.ToString();
        }
    }
    public void EnableEmplist()
    {
        foreach (GridViewRow row in GvEmpListForBusOwner.Rows)
        {
            //access control here

            LinkButton lnkBusOnwerEmpList = (LinkButton)row.FindControl("lnkBusOnwerEmpList");
            lnkBusOnwerEmpList.Enabled = true;
        }

        foreach (GridViewRow row in gvbusinessListForManager.Rows)
        {
            //access control here

            LinkButton lnkMangerEmpList = (LinkButton)row.FindControl("lnkMangerEmpList");
            lnkMangerEmpList.Enabled = true;
        }
        btnback.Enabled = true;
    }
    protected void DisableControls(Control parent, bool State)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is LinkButton)
            {
                ((LinkButton)(c)).Enabled = State;
            }
            if (c is HyperLink)
            {
                ((HyperLink)(c)).Enabled = State;
            }
            if (c is Button)
            {
                ((Button)(c)).Enabled = State;
            }

            DisableControls(c, State);

        }
    }
    public void GetEmplist()
    {
        if (Session["EmployeeId_acc"].ToString() == "0" || Session["UserType_acc"].ToString() == "BUSINESS OWNER" || Session["UserType_acc"].ToString() == "LEVEL 2 (SUPERVISOR)" || Session["UserType_acc"].ToString() == "LEVEL 1 (EMPLOYEE)" || Session["UserType_acc"].ToString() == "LEVEL 3 (OWNER)")
        {
            DataTable dtBussList = new DataTable();


            if (Session["EmployeeId_acc"].ToString() == "0")
            {
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridView(GvDivAdminBussOwner, "AddEmployee Ad  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID  join UserAcc ua on ua.Email = EmpAdd.MainEmail", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, EmpAdd.MainPhone, EmpAdd.MainEmail,CONVERT(NVARCHAR,Ad.HireDate, 106) as HireDate,IsActive", "UA.Password", " ad.UserType=1", "AD.EmployeeID desc");
            }
            else
            {
                string empid = "0";
                DataTable dtMapping = new DataTable();
                dtMapping = comm.GetAllDataFromTable("MappingTable", "*", "USERID=" + Session["UserId_acc"].ToString() + "", "BusinessID,UserTypeId");
                int cnt = dtMapping.Rows.Count;
                foreach (DataRow row in dtMapping.Rows)
                {
                    if (row["UserTypeId"].ToString() == "1")
                    {
                        int rCount = 0;
                        int rDeactiveCount = 0;
                        //dividBusinessOwner.Attributes.Add("style", "margin-left:10px; margin-right:10px; display:block");
                        dividBusinessOwner.Attributes.Add("style", "display:block");
                        cardbus.Visible = true;
                        dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(Session["EmployeeId_acc"].ToString()), int.Parse("1"));
                        rCount = dtBussList.Rows.Count;
                        DataRow[] rowsFiltered = dtBussList.Select("Status=2");
                        rDeactiveCount = int.Parse(rowsFiltered.Length.ToString());
                        if (rCount == rDeactiveCount)
                        {
                            GvEmpListForBusOwner.DataSource = null;
                        }
                        else
                        {
                            GvEmpListForBusOwner.DataSource = dtBussList;
                        }

                        GvEmpListForBusOwner.DataBind();
                    }
                    else if (row["UserTypeId"].ToString() == "2")
                    {
                        btnAddBusiness.Visible = false;
                        dividManager.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            dividManagerHead.Attributes.Add("style", "display:none;");
                            //divAllHead.Attributes.Add("style", "display:block;");
                        }

                        DataTable dtManagerBussniess = new DataTable();
                        if (Session["UserTypeid"].ToString() == "4")
                        {
                            DataTable dt_cnt = new DataTable();

                            dtManagerBussniess = AddEmpCtrl.GetBussList_ManagerForDashBord_Manager_Emp_to_Manager(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(row["EmployeeId"].ToString()), int.Parse(row["UserTypeid"].ToString()), int.Parse(Session["UserId_acc"].ToString()), Session["email_acc"].ToString());
                            dtManagerBussniess.Columns.Add("Count", typeof(System.Int32));
                            foreach (DataRow row1 in dtManagerBussniess.Rows)
                            {
                                dt_cnt = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(row1["ManagerID"].ToString()), int.Parse(row1["BusinessID"].ToString()), int.Parse(Session["MotherOrgid_acc"].ToString()), "P,R");
                                row1["Count"] = dt_cnt.Rows.Count;
                            }


                        }
                        else
                        {
                            DataTable dt_cnt = new DataTable();
                            dtManagerBussniess = AddEmpCtrl.GetBussList_ManagerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(row["EmployeeId"].ToString()), int.Parse(row["UserTypeid"].ToString()), int.Parse(Session["UserId_acc"].ToString()), Session["email_acc"].ToString());
                            dtManagerBussniess.Columns.Add("Count", typeof(System.Int32));
                            foreach (DataRow row1 in dtManagerBussniess.Rows)
                            {
                                dt_cnt = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(row1["ManagerID"].ToString()), int.Parse(row1["BusinessID"].ToString()), int.Parse(Session["MotherOrgid_acc"].ToString()), "P,R");
                                row1["Count"] = dt_cnt.Rows.Count;
                            }
                        }

                        gvbusinessListForManager.DataSource = dtManagerBussniess;
                        gvbusinessListForManager.DataBind();
                    }
                    else if (row["UserTypeId"].ToString() == "4")
                    {
                        btnAddBusiness.Visible = false;
                        dividEmp.Attributes.Add("style", "display:block;");
                        if (cnt > 1)
                        {
                            //dividEmpHead.Attributes.Add("style", "display:none;");
                            divAllHead.Attributes.Add("style", "display:none;");
                            dividEmp.Attributes.Add("style", "display:block;");
                        }

                        empid = row["EmployeeID"].ToString() + "," + empid;
                        // empid= empid.Substring(0,empid.Length-1);
                        if (empid.Length > 0)
                        {
                            //comm.FillGridView(gvBusinessListEmp, "AddEmployee Ae join AssginBussiness Assb on ae.EmployeeID=Assb.EmployeeID", "ae.EmployeeID,assb.BusinessID,assb.BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp", "ae.EmployeeID in (" + empid + ") and assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and assb.UserTypeId=" + row["UserTypeid"].ToString() + "", "ae.EmployeeID");
                            comm.FillGridView(gvBusinessListEmp, "AddEmployee Ae join AssginBussiness Assb on ae.EmployeeID=Assb.EmployeeID", "ae.EmployeeID,assb.BusinessID,assb.BusinessName,(select PrimaryContactName from AddBusiness where BusinessID=Assb.BusinessID)MangerName,(select PrimaryContactNumber from AddBusiness where BusinessID=Assb.BusinessID)PrimaryContactNumber", "dbo.FN_DESC('AddEmployee',ae.EmployeeID)emp", "ae.EmployeeID in (" + empid + ")  and assb.UserTypeId=" + row["UserTypeid"].ToString() + " and ae.IsActive='YES'", "ae.EmployeeID");
                            //and assb.MotherOrgID=" + Session["MotherOrgid"].ToString() + "
                        }

                    }
                    else if (row["UserTypeId"].ToString() == "5")
                    {
                        btnAddBusiness.Visible = false;
                        int Chk = 0;
                        for (int i = 0; i < dtMapping.Rows.Count - 1; i++)
                        {
                            if (dtMapping.Rows[i]["Businessid"].ToString() == dtMapping.Rows[i + 1]["Businessid"].ToString())
                            {
                                if (dtMapping.Rows[i]["Usertypeid"].ToString() == "1" && dtMapping.Rows[i + 1]["Usertypeid"].ToString() == "5")
                                {
                                    Chk = 1;
                                }
                            }
                            if (dtMapping.Rows[0]["Businessid"].ToString() == "0")
                            {
                                Chk = 1;
                            }
                        }
                        if (Chk == 1)
                        {
                            Owner.Attributes.Add("style", "display:none");
                            dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(Session["EmployeeId_acc"].ToString()), int.Parse("5"));
                            GvEmpListOwner.DataSource = dtBussList.Select("Status=1");
                            GvEmpListOwner.DataBind();
                        }
                        else if (Chk != 2)
                        {
                            DataTable removeBid;

                            dtBussList = AddEmpCtrl.GetBussList_OwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(Session["UserId_acc"].ToString()), int.Parse("5"), Session["email"].ToString());

                            if (dtBussList.Rows.Count > 1)
                            {
                                removeBid = comm.GetAllDataFromTable("AddEmployee ae join EmpAddress ea on ae.ADDRESSID=ea.EmpAddressId join AssginBussiness assb on assb.EmployeeID=ae.EmployeeID", "assb.BusinessID", "assb.UserTypeId=1 and ea.MainEmail='" + Session["email_acc"].ToString() + "'", "");
                                if (removeBid.Rows.Count > 0)
                                {
                                    foreach (DataRow r in dtBussList.Rows)
                                    {
                                        foreach (DataRow rRemove in removeBid.Rows)
                                        {
                                            if (r["BusinessId"].ToString() == rRemove[0].ToString())
                                            {
                                                r.Delete();
                                                //newdtBussList.Rows.Add(r.ItemArray);
                                                break;
                                            }
                                        }
                                    }
                                    // newdtBussList.AcceptChanges();
                                }
                            }
                            dtBussList.AcceptChanges();
                            if (dtBussList.Rows.Count > 0)
                            {
                                Owner.Attributes.Add("style", "display:block");
                            }
                            DataTable dtowner = new DataTable();
                            DataRow[] dtRow = dtBussList.Select("Status=1");
                            if (dtRow.Length > 0)
                            {
                                dtowner = dtRow.CopyToDataTable();
                            }
                            bool EmpDe = comm.IfExistsInTable("AddEmployee ae join EmpAddress ea on ae.ADDRESSID=ea.EmpAddressId", "*", "ea.MainEmail='" + Session["email_acc"].ToString() + "' and ae.IsActive='NO'");
                            if (EmpDe == true)
                            {
                                GvEmpListOwner.DataSource = null;
                            }
                            else
                            {
                                GvEmpListOwner.DataSource = dtowner;
                            }

                            GvEmpListOwner.DataBind();
                        }

                    }
                }
                if (Session["UserTypeid_acc"].ToString() == "1")
                {
                    int rCount = 0;
                    int rDeactiveCount = 0;
                    divlist.Visible = true;
                    //dividBusinessOwner.Attributes.Add("style", "margin-left:10px; margin-right:10px; display:block");
                    dividBusinessOwner.Attributes.Add("style", "display:block");
                    dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(Session["EmployeeId_acc"].ToString()), 1);
                    rCount = dtBussList.Rows.Count;
                    DataRow[] rowsFiltered = dtBussList.Select("Status=2");
                    rDeactiveCount = int.Parse(rowsFiltered.Length.ToString());
                    if (rCount == rDeactiveCount)
                    {
                        GvEmpListForBusOwner.DataSource = null;
                    }
                    else
                    {
                        GvEmpListForBusOwner.DataSource = dtBussList;
                    }
                    GvEmpListForBusOwner.DataBind();
                }
            }
        }
        else
        {
            divlist.Visible = true;
            DataTable dtBussList = new DataTable();
            //dividBusinessOwner.Attributes.Add("style", "margin-left:10px; margin-right:10px; display:block");
            dividBusinessOwner.Attributes.Add("style", "display:block");
            dtBussList = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(Session["EmployeeId_acc"].ToString()), int.Parse("1"));
            GvEmpListForBusOwner.DataSource = dtBussList;
            GvEmpListForBusOwner.DataBind();
        }
    }

    protected void btnedit_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GVEmployeeList.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
        Response.Redirect("AddBusiness(TAB).aspx?BusinessIDEdit=" + hdfBusinessID.Value);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        string confirmValue = Request.Form["confirm_value"];
        if (confirmValue == "Yes")
        {
            LinkButton lnkbtn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
            HiddenField hdfBusinessID = (HiddenField)GVEmployeeList.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
            Label _lbl_UserType = (Label)GVEmployeeList.Rows[gvr.RowIndex].FindControl("lbl_UserType");

            AddEmpCtrl.DeleteAddBusiness(int.Parse(hdfBusinessID.Value));

            GetEmplist();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);

        }
    }

    protected void GVEmployeeList_PreRender(object sender, EventArgs e)
    {
        if (GVEmployeeList.Rows.Count > 0)
        {
            GVEmployeeList.UseAccessibleHeader = true;
            GVEmployeeList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GVEmployeeList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton btnedit = (e.Row.FindControl("btnedit") as LinkButton);

            LinkButton btnDelete = (e.Row.FindControl("btnDelete") as LinkButton);

            if (Session["UserType_acc"].ToString() == "BUSINESS OWNER")
            {
                btnedit.Visible = true;
                btnDelete.Visible = true;
            }
        }
        if (Session["UserType_acc"].ToString() == "MANAGER")
        {
            e.Row.Cells[8].Visible = false;
            e.Row.Cells[7].Visible = false;
        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdStatus = (e.Row.FindControl("hfdStatus") as HiddenField);
            CheckBox togBtn = (e.Row.FindControl("togBtn") as CheckBox);

            if (hfdStatus.Value.Trim() == "1")
            {
                togBtn.Checked = true;
            }
            else
            {
                togBtn.Checked = false;
            }
        }

    }

    protected void btnAddBusiness_Click(object sender, EventArgs e)
    {
        Session.Remove("BusinessIDEdit");
        Response.Redirect("AddBusiness(TAB).aspx");
    }

    protected void togBtn_CheckedChanged(object sender, EventArgs e)
    {
        int a = 0;
        CheckBox togBtn = (CheckBox)sender;
        GridViewRow gvr = (GridViewRow)togBtn.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GVEmployeeList.Rows[gvr.RowIndex].FindControl("hdfBusinessID");

        HiddenField hfdStatus = (HiddenField)GVEmployeeList.Rows[gvr.RowIndex].FindControl("hfdStatus");
        if (hfdStatus.Value.Trim() == "1")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + hdfBusinessID.Value + "'");
        }
        if (hfdStatus.Value.Trim() == "2")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + hdfBusinessID.Value + "'");
        }
        GetEmplist();
        if (a > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed!', 'success');", true);
        }

    }

    protected void GvEmpListForBusOwner_PreRender(object sender, EventArgs e)
    {
        if (GvEmpListForBusOwner.Rows.Count > 0)
        {
            GvEmpListForBusOwner.UseAccessibleHeader = true;
            GvEmpListForBusOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvEmpListForBusOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdStatus = (e.Row.FindControl("hfdStatus") as HiddenField);
            CheckBox togBtnForBus = (e.Row.FindControl("togBtnForBus") as CheckBox);

            HyperLink hyeditBuss = (e.Row.FindControl("hyeditBuss") as HyperLink);
            HyperLink hyeditManger = (e.Row.FindControl("hyeditManger") as HyperLink);
            LinkButton lnkBusOnwerEmpList = (e.Row.FindControl("lnkBusOnwerEmpList") as LinkButton);
            HyperLink hyAddEmp = (e.Row.FindControl("hyAddEmp") as HyperLink);
            HyperLink hyAssbus = (e.Row.FindControl("hyAssbus") as HyperLink);
            HyperLink hyViewTimeSheet = (e.Row.FindControl("hyViewTimeSheet") as HyperLink);
            HyperLink hyAssManger = (e.Row.FindControl("hyAssManger") as HyperLink);
            lnkBusOnwerEmpList.Enabled = true;

            if (hfdStatus.Value.Trim() == "1")
            {
                togBtnForBus.Checked = true;
            }
            else
            {
                togBtnForBus.Checked = false;
                //all deactive
                hyeditBuss.Enabled = false;
                hyeditManger.Enabled = false;
                lnkBusOnwerEmpList.Enabled = false;
                hyAddEmp.Enabled = false;
                hyAssbus.Enabled = false;
                hyViewTimeSheet.Enabled = false;
                hyAssManger.Enabled = false;
                e.Row.BackColor = System.Drawing.Color.LightGray;
            }
        }
    }

    protected void togBtnForBus_CheckedChanged(object sender, EventArgs e)
    {
        int a = 0;
        CheckBox togBtnForBu = (CheckBox)sender;
        GridViewRow gvr = (GridViewRow)togBtnForBu.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
        DataTable dtAllEmp = comm.GetAllDataFromTable("AssginBussiness", "*", "BusinessID=" + hdfBusinessID.Value + "", "");

        HiddenField hfdStatus = (HiddenField)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("hfdStatus");
        if (hfdStatus.Value.Trim() == "1")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + hdfBusinessID.Value + "'");
            foreach (DataRow r in dtAllEmp.Rows)
            {
                comm.ExecuteQuery("Update AddEmployee set IsActive='No' where EmployeeID='" + r["EmployeeID"].ToString() + "'");
            }
        }
        if (hfdStatus.Value.Trim() == "2")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + hdfBusinessID.Value + "'");
            foreach (DataRow r in dtAllEmp.Rows)
            {
                comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID='" + r["EmployeeID"].ToString() + "'");
            }
        }
        GetEmplist();
        if (a > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed!', 'success');", true);
        }
    }

    protected void btneditForBus_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
        Response.Redirect("AddBusiness(TAB).aspx?BusinessIDEdit=" + hdfBusinessID.Value);
    }

    protected void btnDeleteForBus_Click(object sender, EventArgs e)
    {
        string confirmValue = Request.Form["confirm_value"];
        if (confirmValue == "Yes")
        {
            LinkButton lnkbtn = (LinkButton)sender;
            GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
            HiddenField hdfBusinessID = (HiddenField)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
            Label _lbl_UserType = (Label)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("lbl_UserType");

            AddEmpCtrl.DeleteAddBusiness(int.Parse(hdfBusinessID.Value));

            GetEmplist();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);
        }
    }

    protected void lnkBusOnwerEmpList_Click(object sender, EventArgs e)
    {

        hfdPopUpBusid.Value = "0";
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID");
        Label CompanyName = (Label)GvEmpListForBusOwner.Rows[gvr.RowIndex].FindControl("CompanyName");
        lblbusName.Text = CompanyName.Text;
        hfdPopUpBusid.Value = hdfBusinessID.Value;

        //comm.FillGridView(GvPopUpEmpList, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " AND AE.UserType<>5 and AE.UserType!=1", "AE.EmployeeID");
        comm.FillGridViewDataNull(GvPopUpEmpList, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID and effectivedate <= GETDATE())", "AE.EmployeeID,ASSB.BusinessID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " and AE.UserType!=1  and ae.EmployeeID<>" + Session["EmployeeId_acc"].ToString() + "", "AE.EmployeeID");
        ModalPopupEmpList.Show();
    }

    protected void GvPopUpEmpList_PreRender(object sender, EventArgs e)
    {
        if (GvPopUpEmpList.Rows.Count > 0)
        {
            GvPopUpEmpList.UseAccessibleHeader = true;
            GvPopUpEmpList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void togBtnForBusOwner_CheckedChanged(object sender, EventArgs e)
    {
        int a = 0;
        CheckBox togBtnForBusOwner = (CheckBox)sender;
        GridViewRow gvrBusOwner = (GridViewRow)togBtnForBusOwner.NamingContainer;
        HiddenField hdfEmpNameid = (HiddenField)GvPopUpEmpList.Rows[gvrBusOwner.RowIndex].FindControl("hdfEmpNameid");
        string dt = DateTime.Now.ToShortDateString();
        HiddenField hfdStatusForBussOwner = (HiddenField)GvPopUpEmpList.Rows[gvrBusOwner.RowIndex].FindControl("hfdStatusForBussOwner");
        if (hfdStatusForBussOwner.Value.Trim() == "YES")
        {
            a = comm.ExecuteQuery("Update AddEmployee set IsActive='NO', Apartment='" + dt + "' where EmployeeID='" + hdfEmpNameid.Value + "'");
        }
        if (hfdStatusForBussOwner.Value.Trim() == "NO")
        {
            a = comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID='" + hdfEmpNameid.Value + "'");
        }

        if (a > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed!', 'success');", true);
        }
    }

    protected void GvPopUpEmpList_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            HyperLink hyeditEmp = (e.Row.FindControl("hyeditEmp") as HyperLink);
            HyperLink hyeditEmprate = (e.Row.FindControl("hyeditEmprate") as HyperLink);
            HyperLink hyEnterTimeSheet = (e.Row.FindControl("hyEnterTimeSheet") as HyperLink);
            HiddenField hfdStatusForBussOwner = (e.Row.FindControl("hfdStatusForBussOwner") as HiddenField);
            HiddenField hdfEmpNameid = (e.Row.FindControl("hdfEmpNameid") as HiddenField);
            CheckBox togBtnForBusOwner = (e.Row.FindControl("togBtnForBusOwner") as CheckBox);
            string aa = Session["EmployeeId_acc"].ToString();
            DataTable dtGv = new DataTable();
            DataTable dtlogin = new DataTable();
            dtGv = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId", "ea.MainEmail", "ad.EmployeeID=" + hdfEmpNameid.Value + "", "");
            dtlogin = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId", "ea.MainEmail", "ad.EmployeeID=" + aa + "", "");
            if (dtGv.Rows[0]["MainEmail"].ToString() == dtlogin.Rows[0]["MainEmail"].ToString() && mangercheck > 0)
            {
                hyeditEmp.Visible = false;
                hyeditEmprate.Visible = false;
            }
            if (hfdStatusForBussOwner.Value.Trim() == "YES")
            {
                togBtnForBusOwner.Checked = true;
            }
            else
            {
                togBtnForBusOwner.Checked = false;
                e.Row.BackColor = System.Drawing.Color.LightGray;
                hyeditEmp.Enabled = false;
                hyeditEmprate.Enabled = false;
                hyEnterTimeSheet.Enabled = false;
            }
            hyeditEmp.Enabled = false;
            hyEnterTimeSheet.Enabled = false;
            hyeditEmprate.Enabled = false;
        }
    }

    protected void GvDivAdminBussOwner_PreRender(object sender, EventArgs e)
    {
        if (GvDivAdminBussOwner.Rows.Count > 0)
        {
            GvDivAdminBussOwner.UseAccessibleHeader = true;
            GvDivAdminBussOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btneditdivAdmin_Click(object sender, EventArgs e)
    {
        LinkButton btneditdivAdmin = (LinkButton)sender;

        GridViewRow GvDivAdminBussOwner1 = (GridViewRow)btneditdivAdmin.NamingContainer;
        HiddenField hdfEmployeeIDdivAdmin = (HiddenField)GvDivAdminBussOwner.Rows[GvDivAdminBussOwner1.RowIndex].FindControl("hdfEmployeeIDdivAdmin");
        Response.Redirect("AddNewEmployee_TAB?EmployeeIDEdit=" + hdfEmployeeIDdivAdmin.Value);
    }

    protected void gvbusinessListForManager_PreRender(object sender, EventArgs e)
    {
        if (gvbusinessListForManager.Rows.Count > 0)
        {
            gvbusinessListForManager.UseAccessibleHeader = true;
            gvbusinessListForManager.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void lnkMangerEmpList_Click(object sender, EventArgs e)
    {

        mangercheck = 1;
        DataTable dtMangerEmpList = new DataTable();
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfBusinessIDForManager = (HiddenField)gvbusinessListForManager.Rows[gvr.RowIndex].FindControl("hdfBusinessIDForManager");
        HiddenField hfdMangerIDFormManger = (HiddenField)gvbusinessListForManager.Rows[gvr.RowIndex].FindControl("hfdMangerIDFormManger");
        Label CompanyName = (Label)gvbusinessListForManager.Rows[gvr.RowIndex].FindControl("CompanyName");
        lblbusName.Text = CompanyName.Text;
        hfdPopUpBusid.Value = hdfBusinessIDForManager.Value;
        dtMangerEmpList = AddEmpCtrl.GetEmpListForDefaultFromManager(int.Parse(hfdMangerIDFormManger.Value), int.Parse(hdfBusinessIDForManager.Value), int.Parse(Session["MotherOrgid_acc"].ToString()), "P,R");
        GvPopUpEmpList.DataSource = dtMangerEmpList;
        GvPopUpEmpList.DataBind();
        //comm.FillGridView(GvPopUpEmpList, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId and rd.effectivedate<= GETDATE() and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " AND AE.UserType not in (1,5)", "AE.EmployeeID");
        ModalPopupEmpList.Show();
    }

    protected void gvBusinessListEmp_PreRender(object sender, EventArgs e)
    {
        if (gvBusinessListEmp.Rows.Count > 0)
        {
            gvBusinessListEmp.UseAccessibleHeader = true;
            gvBusinessListEmp.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GvDivAdminBussOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            //Find the TextBox control.
            CheckBox togBtnForAdminBussOwner = (e.Row.FindControl("togBtnForAdminBussOwner") as CheckBox);
            HiddenField hfdStatusForAdminBussOwner = (e.Row.FindControl("hfdStatusForAdminBussOwner") as HiddenField);
            if (hfdStatusForAdminBussOwner.Value.ToString().Trim() == "YES")
            {
                togBtnForAdminBussOwner.Checked = true;
            }
            else
            {
                togBtnForAdminBussOwner.Checked = false;
            }
        }
    }
    string confirmValue;
    [System.Web.Services.WebMethod]
    public static string DeleteClick(int id)
    {
        CommanClass comm = new CommanClass();
        //string conString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        int i = 0;
        DataTable dtActive = comm.GetAllDataFromTable("AddEmployee", "IsActive", "EmployeeID=" + id + "", "");
        string IsActive = dtActive.Rows[0]["IsActive"].ToString();
        if (IsActive.Trim() == "YES")
        {
            i = comm.ExecuteQuery("Update AddEmployee set IsActive='NO' where EmployeeID=" + id + "");
        }
        else
        {
            i = comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID=" + id + "");
        }
        return i > 0 ? "Deleted" : "Failed";
    }

    protected void togBtnForAdminBussOwner_CheckedChanged(object sender, EventArgs e)
    {
        string confirmValue = Request.Form["confirm_value"];
        CheckBox togBtnForAdminBussOwner = (CheckBox)sender;
        GridViewRow gvr = (GridViewRow)togBtnForAdminBussOwner.NamingContainer;
        Label lblBusinessNamedivAdmin = (Label)GvDivAdminBussOwner.Rows[gvr.RowIndex].FindControl("lblBusinessNamedivAdmin");
        HiddenField hfdStatusForAdminBussOwner = (HiddenField)GvDivAdminBussOwner.Rows[gvr.RowIndex].FindControl("hfdStatusForAdminBussOwner");
        if (hfdStatusForAdminBussOwner.Value.ToString().Trim() == "YES")
        {
            comm.ExecuteQuery("Update AddEmployee set IsActive='NO' where EmployeeID=" + lblBusinessNamedivAdmin.ToolTip + "");
            //comm.ExecuteQuery("Update UserAcc set Status='0' where EmployeeID=" + lblBusinessNamedivAdmin.ToolTip + "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Status changed successfully', 'success');", true);

            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1", "AD.EmployeeID desc");
            }
            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "YES")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='YES'", "AD.EmployeeID desc");
            }
            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "NO")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='NO'", "AD.EmployeeID desc");
            }
        }
        else
        {
            comm.ExecuteQuery("Update AddEmployee set IsActive='YES' where EmployeeID=" + lblBusinessNamedivAdmin.ToolTip + "");
            //comm.ExecuteQuery("Update UserAcc set Status='1' where EmployeeID=" + lblBusinessNamedivAdmin.ToolTip + "");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Status changed successfully', 'success');", true);

            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1", "AD.EmployeeID desc");
            }
            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "YES")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='YES'", "AD.EmployeeID desc");
            }
            if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "NO")
            {
                DataTable dtBussList = new DataTable();
                string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
                dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
                dividBusinessOwner.Visible = false;
                divAdmin.Attributes.Add("style", "display:block");
                comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='NO'", "AD.EmployeeID desc");
            }
        }
    }

    protected void ddl_status_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "")
        {
            DataTable dtBussList = new DataTable();
            string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
            dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
            dividBusinessOwner.Visible = false;
            divAdmin.Attributes.Add("style", "display:block");
            comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1", "AD.EmployeeID desc");
        }
        if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "YES")
        {
            DataTable dtBussList = new DataTable();
            string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
            dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
            dividBusinessOwner.Visible = false;
            divAdmin.Attributes.Add("style", "display:block");
            comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='YES'", "AD.EmployeeID desc");
        }
        if (Session["EmployeeId_acc"].ToString() == "0" && ddl_status.SelectedValue == "NO")
        {
            DataTable dtBussList = new DataTable();
            string[] selectedColumns = new[] { "BusinessID", "CompanyName", "OperatingName", "BusinessRegistrationDate", "PrimaryContactName", "PrimaryContactNumber", "BusinessAgreementDate", "Status", "Statusstr" };
            dtBussList = new DataView(AddEmpCtrl.GetBussList(int.Parse(Session["MotherOrgid_acc"].ToString()))).ToTable(true, selectedColumns);
            dividBusinessOwner.Visible = false;
            divAdmin.Attributes.Add("style", "display:block");
            comm.FillGridViewDataNull(GvDivAdminBussOwner, "AddEmployee Ad  join UserAcc ua on ua.EmployeeId = ad.EmployeeID  join EmpAddress EmpAdd on EmpAdd.EmpAddressId=AD.ADDRESSID ", "DISTINCT ''as bussname,AD.EmployeeID,dbo.FN_DESC('UserType',AD.UserType)UserType,UA.UserId,dbo.FN_DESC('AddEmployee',AD.EmployeeID)FirstName, CONVERT(NVARCHAR,AD.HireDate, 106) as HireDate, AD.IsActive, EmpAdd.MainPhone, EmpAdd.MainEmail", "UA.Password", " ad.UserType=1 and ad.IsActive='NO'", "AD.EmployeeID desc");
        }
    }

    protected void GvEmpListOwner_PreRender(object sender, EventArgs e)
    {

    }

    protected void GvEmpListOwner_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (GvEmpListOwner.Rows.Count > 0)
        {
            GvEmpListOwner.UseAccessibleHeader = true;
            GvEmpListOwner.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void togBtnForBus1_CheckedChanged(object sender, EventArgs e)
    {
        int a = 0;
        CheckBox togBtnForBu1 = (CheckBox)sender;
        GridViewRow gvr = (GridViewRow)togBtnForBu1.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GvEmpListOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID1");

        HiddenField hfdStatus = (HiddenField)GvEmpListOwner.Rows[gvr.RowIndex].FindControl("hfdStatus1");
        if (hfdStatus.Value.Trim() == "1")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='2' where BusinessID='" + hdfBusinessID.Value + "'");
        }
        if (hfdStatus.Value.Trim() == "2")
        {
            a = comm.ExecuteQuery("Update AddBusiness set Status='1' where BusinessID='" + hdfBusinessID.Value + "'");
        }

        if (a > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Status Changed!', 'success');", true);
        }
    }

    protected void lnkBusOnwerEmpList1_Click(object sender, EventArgs e)
    {
        hfdPopUpBusid.Value = "0";
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfBusinessID = (HiddenField)GvEmpListOwner.Rows[gvr.RowIndex].FindControl("hdfBusinessID1");
        Label CompanyName = (Label)GvEmpListOwner.Rows[gvr.RowIndex].FindControl("CompanyName1");
        lblbusName.Text = CompanyName.Text;
        hfdPopUpBusid.Value = hdfBusinessID.Value;

        //comm.FillGridView(GvPopUpEmpList, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID)", "AE.EmployeeID,ASSB.BusinessID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " AND AE.UserType<>5 and AE.UserType!=1", "AE.EmployeeID");
        comm.FillGridViewDataNull(GvPopUpEmpList, "AddEmployee AE left JOIN AssginBussiness Assb on AE.EmployeeID=ASSB.EmployeeID left JOIN RateDetails RD ON AE.EmployeeID=RD.EmployeeId  and rd.RateId=(select MAX(RateId)from RateDetails where EmployeeID = AE.EmployeeID and effectivedate <= GETDATE())", "AE.EmployeeID,ASSB.BusinessID", "DBO.FN_DESC('AddEmployee',AE.EmployeeID)EmpName,case when RD.Rate IS null then '' else '$ '+ CAST (RD.Rate AS NVARCHAR) +'/'+RD.RateType end AS Rate,AE.IsActive", "ASSB.BusinessID=" + hfdPopUpBusid.Value + " and AE.UserType!=1  ", "AE.EmployeeID");//AE.UserType!=1 after and ae.EmployeeID<>" + Session["EmployeeId"].ToString() + "
        ModalPopupEmpList.Show();
    }
}