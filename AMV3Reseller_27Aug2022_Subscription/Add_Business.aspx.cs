using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Add_Business : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddBusinessEntityClass AddBusiEntity = new AddBusinessEntityClass();
    AddBusinessControllerClass AddBusi = new AddBusinessControllerClass();
    AddBusinessAddress BusinessAddressEntity = new AddBusinessAddress();
    AddBusinessControllerClass AddBeCtrl = new AddBusinessControllerClass();

    protected void Page_Init(object sender,EventArgs e)
    {
        AnyminuteModernScope.Login();ViewStateUserKey=Session.SessionID;
        int business;
        if(Request.QueryString["BusinessIDEdit"]!=null)
        {if(!Int32.TryParse(Request.QueryString["BusinessIDEdit"],out business))throw new System.Web.HttpException(400,"Invalid business.");
         using(var c=AnyminuteTimesheetRepair.Open())AnyminuteApprovalAssignments.RequireOwner(c,null,business);}
        else AnyminuteFormAccess.Subscriber();
        EnsureBusinessActions(Request.QueryString["BusinessIDEdit"] != null);
    }
    // Create missing actions during Init so postback IDs remain stable. Existing
    // declarative controls are reused; neither path needs generated field names.
    private void EnsureBusinessActions(bool editing)
    {
        EnsureBusinessActionButton("RemoveBusinessLogo", "Remove Logo", editing,
            new EventHandler(RemoveLogo_Click), null);
        // v10: Deactivate Business is no longer offered on this page (handler kept for compatibility).
        EnsureBusinessActionButton("DeactivateBusiness", "Deactivate Business", false,
            new EventHandler(DeactivateBusiness_Click),
            "return confirm('Deactivate this business while retaining its records?');");
    }

    private void EnsureBusinessActionButton(string id, string text, bool visible,
        EventHandler click, string confirmation)
    {
        Control existing = FindBusinessActionControl(this, id);
        Button button = existing as Button;
        if (existing != null && button == null)
            throw new InvalidOperationException("Business action control has an unexpected type: " + id);
        if (button == null)
        {
            Control host = FindBusinessActionControl(this, "ContentPlaceHolder1");
            if (host == null)
                throw new InvalidOperationException("The business page content placeholder is missing.");
            Panel actions = FindBusinessActionControl(this, "AnyminuteBusinessActions") as Panel;
            if (actions == null)
            {
                actions = new Panel();
                actions.ID = "AnyminuteBusinessActions";
                actions.CssClass = "am-actions";
                actions.Style["margin-bottom"] = "16px";
                // ASPX inline expressions can make a content collection read-only.
                // Insert immediately before that container in its writable parent.
                Control insertionHost = host;
                int insertionIndex = 0;
                while (insertionHost.Controls.IsReadOnly)
                {
                    Control child = insertionHost;
                    insertionHost = insertionHost.Parent;
                    if (insertionHost == null || insertionHost == this)
                        throw new InvalidOperationException("No writable business action container is available.");
                    insertionIndex = insertionHost.Controls.IndexOf(child);
                }
                insertionHost.Controls.AddAt(insertionIndex, actions);
            }
            button = new Button();
            button.ID = id;
            button.Text = text;
            button.CssClass = id == "DeactivateBusiness" ? "btn2 btn-danger" : "btn2 btn-default";
            actions.Controls.Add(button);
        }
        button.Visible = visible;
        button.CausesValidation = false;
        if (!String.IsNullOrEmpty(confirmation)) button.OnClientClick = confirmation;
        // A declarative OnClick may already be wired. Keep one handler per action.
        button.Click -= click;
        button.Click += click;
    }

    private static Control FindBusinessActionControl(Control root, string id)
    {
        if (root.ID == id) return root;
        foreach (Control child in root.Controls)
        {
            Control found = FindBusinessActionControl(child, id);
            if (found != null) return found;
        }
        return null;
    }

    protected void Page_PreLoad(object sender,EventArgs e) {if(IsPostBack)AnyminuteFormAccess.OwnedSelections(Page);}
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        EnsureSaveKey();
        // v64 (PPT s2/s3): subscriber/owner sessions can arrive without MotherOrgid (e.g. after password reset) — resolve it from the
        // owner's own business assignment so Add/Edit Business no longer crash to the generic error page.
        if (String.IsNullOrEmpty(Convert.ToString(Session["MotherOrgid"])))
        {
            var mo = comm.ExecuteQueryDataTable("select top 1 MotherOrgID from AssginBussiness with (Nolock) where EmployeeID=" + int.Parse(Session["EmployeeId"].ToString()) + " and MotherOrgID is not null order by UserTypeId");
            Session["MotherOrgid"] = mo.Rows.Count > 0 ? mo.Rows[0][0].ToString() : "0";
        }
        if (!IsPostBack)
        {
            string flash = AnyminuteFlash.Take("business");
            if (flash != null) { BusinessFlashText.Text = HttpUtility.HtmlEncode(flash); BusinessFlash.Visible = true; }
        }
        if (!IsPostBack)
        {
            #region show message if maximum limit is reached. This region code is same as of in Save button.
            if (Request.QueryString["BusinessIDEdit"] == null)
            {
                var check_reseller_email = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "Email", "MotherOrgID = '" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                bool check_reseller_or_subscriber_email = comm.IfExistsInTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + (check_reseller_email.Rows.Count > 0 ? check_reseller_email.Rows[0]["Email"].ToString().Replace("'", "''") : "") + "' ") && check_reseller_email.Rows.Count > 0;
                if (!check_reseller_or_subscriber_email)
                {
                    var check_old_subscriber = comm.ExecuteQueryDataTable("select Business from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "'" +
                        "and IsActive=1 ");
                    if (check_old_subscriber.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["EmployeeId"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1) // v82: was ==0 (inverted), blocked every paid/free single-plan subscriber; ==1 = first plan still awaiting payment, same as all other pages
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnSaveBusinessDetails.Visible = false;
                                DisableControls(Page, false);
                                return;
                            }

                            #region amit commented code
                            //AddBusinessControllerClass AddEmpCtrl = new AddBusinessControllerClass();
                            //string confirmValue = Request.Form["confirm_value"];
                            //if (confirmValue == "Yes" || confirmValue == "null" || confirmValue == null)
                            //{
                            //}
                            //else
                            //{
                            //    return;
                            //}

                            //var empid = Session["EmployeeId"].ToString();
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");

                            //DataTable dt = new DataTable();//AddBeCtrl.GetBussForSelectBUSS(int.Parse(empid), int.Parse(Session["UserTypeid"].ToString()));
                            //int BUSSCNT = 0;
                            //string empid1 = "0";
                            //DataTable dtCommaUser = comm.GetAllDataFromTable("AddEmployee ad join EmpAddress ea on ad.ADDRESSID=ea.EmpAddressId join UserAcc ua on ua.Email=ea.MainEmail", "ad.EmployeeID,ua.UserId", "ua.Email='" + Session["email"].ToString() + "'", "");
                            //string CommaUserid = string.Empty;
                            //for (int i = 0; i < dtCommaUser.Rows.Count; i++)
                            //{
                            //    CommaUserid = CommaUserid + dtCommaUser.Rows[i]["UserId"].ToString();
                            //    CommaUserid += (i < dtCommaUser.Rows.Count) ? "," : string.Empty;
                            //}
                            //CommaUserid = CommaUserid.TrimEnd(',');
                            //DataTable dtMapping = new DataTable(); dtMapping = comm.GetAllDataFromTable("MappingTable", "*", "USERID in(" + CommaUserid + ")", "BusinessID,UserTypeId");

                            //int cnt = dtMapping.Rows.Count;
                            //foreach (DataRow row in dtMapping.Rows)
                            //{
                            //    if (row["UserTypeId"].ToString() == "1")
                            //    {
                            //        int rCount = 0;
                            //        int rDeactiveCount = 0;
                            //        DataTable dtIsEdit = new DataTable();
                            //        try
                            //        {
                            //            if (Session["Redirect"] != null)
                            //            {
                            //                if (Session["MotherOrgid_acc"].ToString() != "")
                            //                {
                            //                    dt = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid_acc"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            //                }
                            //                else
                            //                {
                            //                    dt = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            //                }
                            //            }
                            //            if (Session["Redirect"] == null)
                            //            {
                            //                dt = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            //            }
                            //        }
                            //        catch (Exception ex)
                            //        {
                            //            dt = AddEmpCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(row["EmployeeID"].ToString().ToString()), int.Parse("1"));
                            //        }
                            //        rCount = dt.Rows.Count;
                            //        dtIsEdit = comm.GetAllDataFromTable("AddEmployee", "*", "EmployeeID=" + int.Parse(row["EmployeeID"].ToString().ToString()) + "", "");
                            //        DataRow[] rowsFiltered = dt.Select("Status=2");
                            //        rDeactiveCount = int.Parse(rowsFiltered.Length.ToString());
                            //    }
                            //}

                            //BUSSCNT = dt.Rows.Count;
                            //if (dtBussCnt.Rows.Count > 0 && BUSSCNT >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            //{
                            //    Label_Maximum_Limit.Text = "Please contact admin to add more business. You have reached the maximum limit.";
                            //    Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //    btnSaveBusinessDetails.Visible = false;
                            //    DisableControls(Page, false);
                            //    return;
                            //}
                            #endregion

                            var empid = Session["EmployeeId"].ToString();
                            int BUSSCNT = 0;
                            DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory  with (Nolock) GROUP BY EmpId,id,Business having EmpId='" + empid + "' and Id=(select max(id) from PlanHistory where EmpId='" + empid + "' and IsActive=1)");
                            //DataTable dt = comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID,BusinessName,MotherOrgID", "EmployeeID = " +
                            //    "'" + int.Parse(Session["EmployeeID"].ToString()) + "' and UserTypeId=1 and BusinessID <> NULL", "");
                            DataTable dt = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));
                            
                            BUSSCNT = dt.Rows.Count;
                            if (dtBussCnt.Rows.Count > 0 && BUSSCNT >= int.Parse(dtBussCnt.Rows[0][0].ToString()))
                            {
                                Label_Maximum_Limit.Text = "Please contact admin to add more business. You have reached the maximum limit.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnSaveBusinessDetails.Visible = false;
                                DisableControls(Page, false);
                                return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to add more business. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnSaveBusinessDetails.Visible = false;
                            DisableControls(Page, false);
                            return;
                        }
                    }
                    if (check_old_subscriber.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnSaveBusinessDetails.Visible = false;
                        DisableControls(Page, false);
                        return;
                    }
                }
                if (check_reseller_or_subscriber_email)
                {
                    var check_old_reseller = comm.ExecuteQueryDataTable("select Business from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' " +
                        "and IsActive=1 ");
                    if (check_old_reseller.Rows.Count > 0)
                    {
                        var check_cancelled = comm.ExecuteQueryDataTable("select Is_Cancelled from PlanHistory with (Nolock) where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Is_Cancelled=1 ");
                        if (check_cancelled.Rows.Count == 0)
                        {
                            DataTable dt_FindIf1stTimeRegisteredUserDetails = comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            DataTable dt_RequestedPlanDetails = comm.GetAllDataFromTable("PlanHistory  with (Nolock)", "EmpId", "EmpId=" + int.Parse(Session["MotherOrgid"].ToString()) + " and Is_Requested_Plan=1", "");
                            if (dt_FindIf1stTimeRegisteredUserDetails.Rows.Count == 1 && dt_RequestedPlanDetails.Rows.Count == 1)
                            {
                                Label_Maximum_Limit.Text = "Your plan is not active. Please pay the amount from the link received in the registered email id.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnSaveBusinessDetails.Visible = false;
                                DisableControls(Page, false);
                                return;
                            }

                            //This logic commented is as per the reseller not the reseller subscriber
                            //DataTable No_of_business = comm.GetAllDataFromTable("AddBusiness AsB", "AsB.BusinessID, AsB.CompanyName, AsB.Status,(select count(EmployeeID) from AssginBussiness where UserTypeId<>1 AND BusinessID = AsB.BusinessID)empcnt ", "MotherOrgId=" + int.Parse(Session["MotherOrgid"].ToString()) + "", "");
                            //int total_no_bussiness = 0;
                            //total_no_bussiness = int.Parse(No_of_business.Rows.Count.ToString());
                            //DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory GROUP BY EmpId,id,Business having EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["MotherOrgid"].ToString()) + "' and IsActive=1)");
                            //int BUSSCNT_RESELLER = 0;
                            //BUSSCNT_RESELLER = dtBussCnt.Rows.Count > 0 ? int.Parse(dtBussCnt.Rows[0]["Business"].ToString()) : int.MaxValue;
                            //if (total_no_bussiness >= BUSSCNT_RESELLER)
                            //{
                            //    Label_Maximum_Limit.Text = "Please contact admin to add more business. You have reached the maximum limit.";
                            //    Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            //    btnSaveBusinessDetails.Visible = false;
                            //    DisableControls(Page, false);
                            //    return;
                            //}

                            DataTable dt_check_if_res_sub_regsitered_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=1 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_if_res_sub_regsitered_status.Rows.Count == 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact reseller. Your plan is not activated.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnSaveBusinessDetails.Visible = false;
                                DisableControls(Page, false);
                                return;
                            }

                            DataTable dt_check_active_status = comm.ExecuteQueryDataTable("select IsActive from PlanHistory with (Nolock) where IsActive=0 and " +
                                "EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " ");
                            if (dt_check_active_status.Rows.Count == 0)
                            {
                                //DataTable No_of_business = comm.GetAllDataFromTable("AssginBussiness", "BusinessID,EmployeeID,BusinessName,MotherOrgID", "EmployeeID = " +
                                //"'" + int.Parse(Session["EmployeeID"].ToString()) + "' and MotherOrgID=" + int.Parse(Session["MotherOrgid"].ToString()) + " and UserTypeId=1" +
                                //"and BusinessID <> NULL", "");
                                DataTable No_of_business = AddBeCtrl.GetBussList_BusinessOwnerForDashBord1(int.Parse(Session["MotherOrgid"].ToString()), int.Parse(Session["EmployeeID"].ToString()), int.Parse("1"));

                                int total_no_bussiness = 0;
                                total_no_bussiness = int.Parse(No_of_business.Rows.Count.ToString());
                                DataTable dtBussCnt = comm.ExecuteQueryDataTable("select  Business from PlanHistory with (Nolock) GROUP BY EmpId,id,Business having EmpId='" + int.Parse(Session["EmployeeID"].ToString()) + "' and Id=(select max(id) from PlanHistory where EmpId='" + int.Parse(Session["EmployeeId"].ToString()) + "' and IsActive=1)");
                                int BUSSCNT_RESELLER = 0;
                                BUSSCNT_RESELLER = dtBussCnt.Rows.Count > 0 ? int.Parse(dtBussCnt.Rows[0]["Business"].ToString()) : int.MaxValue;
                                if (total_no_bussiness >= BUSSCNT_RESELLER)
                                {
                                    Label_Maximum_Limit.Text = "Please contact admin to add more business. You have reached the maximum limit.";
                                    Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                    btnSaveBusinessDetails.Visible = false;
                                    DisableControls(Page, false);
                                    return;
                                }
                            }
                            if (dt_check_active_status.Rows.Count > 0)
                            {
                                Label_Maximum_Limit.Text = "Please contact admin to add more business. Your plan has been cancelled.";
                                Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                                btnSaveBusinessDetails.Visible = false;
                                DisableControls(Page, false);
                                return;
                            }
                        }
                        if (check_cancelled.Rows.Count > 0)
                        {
                            Label_Maximum_Limit.Text = "Please contact admin to add more business. Your plan has been cancelled.";
                            Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                            btnSaveBusinessDetails.Visible = false;
                            DisableControls(Page, false);
                            return;
                        }
                    }
                    if (check_old_reseller.Rows.Count == 0)
                    {
                        Label_Maximum_Limit.Text = "Please contact admin. Your plan is not active.";
                        Label_Maximum_Limit.Attributes.Add("style", "text-decoration:blink");
                        btnSaveBusinessDetails.Visible = false;
                        DisableControls(Page, false);
                        return;
                    }
                }
            }
            #endregion

            bool Check_Subscriber_Active_Status = comm.IfExistsInTable("AddEmployee with (Nolock)", "IsActive", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and IsActive='NO        ' and UserType='1' ");
            if (Check_Subscriber_Active_Status)
            {
                DisableControls(Page, false);
                btnSaveBusinessDetails.Visible = false;
                return;
            }
            BsnsRegDate.Attributes.Add("max", System.DateTime.Now.Date.ToString("yyyy-MM-dd"));
            ResetAddressChoices(AddressPlaceholder);
            try
            {
                if (Session["Redirect"] != null)
                {
                    if (Session["MotherOrgid_acc"].ToString() != "")
                    {
                        txtaccountantmail.Text = Session["email_acc"].ToString();
                        divAdmin.Attributes.Add("style", "display:none");
                    }
                }
            }
            catch
            {
                txtaccountantmail.Text = "";
                divAdmin.Attributes.Add("style", "display:block");
            }
            if (Request.QueryString["BusinessIDEdit"] != null)
            {
                GetDataforEditBusinessDetails(int.Parse(Request.QueryString["BusinessIDEdit"]));
                divAdd.Attributes.Add("Style", "display:block");
                btnUpdateAddress.Visible = false;
                if (Convert.ToString(Session["UserTypeid"]) == "0" && Convert.ToString(Session["UserType"]) == "Admin")
                {
                    btnback.HRef = "Accountant.aspx";
                    if (Request.QueryString["BusinessIDEdit"] != null)
                    {
                        DisableControls(Page, false);
                    }
                }
                btnSaveAddres.Text = "Update";
            }
            if (Request.QueryString["BusinessIDEdit"] == null)
            {
                btnSaveAddres.Text = "Save";
            }
            if (Convert.ToString(Session["UserType"]) == "BUSINESS OWNER")
            {
                btnback.Visible = true;
            }
        }
    }

    protected void DisableControls(Control parent, bool State)
    {
        foreach (Control c in parent.Controls)
        {
            if (c is DropDownList)
            {
                ((DropDownList)(c)).Enabled = State;
            }
            if (c is TextBox)
            {
                ((TextBox)(c)).Enabled = State;
            }
            //if (c is LinkButton)
            //{
            //    ((LinkButton)(c)).Enabled = State;
            //}
            //if (c is FileUpload)
            //{
            //    ((FileUpload)(c)).Enabled = State;
            //}
            DisableControls(c, State);
        }
        FileUpload1.Visible = false;
    }

    public void GetDataforEditBusinessDetails(int BusinessIDEdit)
    {
        //btnSaveBusinessDetails.Text = "Update Business Details";
        btnSaveBusinessDetails.Text = "Update";
        btnSaveBusinessDetails.Visible = false;
        btnUpdateBusinessDetails.Visible = true;
        DataTable dtBusinessEdit = new DataTable();
        dtBusinessEdit = comm.GetAllDataFromTable("AddBusiness with (Nolock)", "*", "BusinessID=" + BusinessIDEdit + "", "");
        if (dtBusinessEdit.Rows.Count == 0) { Response.Redirect("Dashboard.aspx?view=businesses", false); Context.ApplicationInstance.CompleteRequest(); return; } // v64
        txtCompanyName.Text = dtBusinessEdit.Rows[0]["CompanyName"].ToString();
        hdn_companyname.Value = dtBusinessEdit.Rows[0]["CompanyName"].ToString();
        txtOperatingName.Text = dtBusinessEdit.Rows[0]["OperatingName"].ToString();
        txtBusinessNumber.Text = dtBusinessEdit.Rows[0]["BusinessNumber"].ToString();
        SelectStoredOption(ddlLegalEnityType, dtBusinessEdit.Rows[0]["LegalEntityType"].ToString());
        BsnsRegDate.Text = dtBusinessEdit.Rows[0]["BusinessRegistrationDate"].ToString();
        PrmContName.Text = dtBusinessEdit.Rows[0]["PrimaryContactName"].ToString();
        PrmContNmbr.Text = dtBusinessEdit.Rows[0]["PrimaryContactNumber"].ToString();

        if (File.Exists(Server.MapPath("~/Uploads/" + dtBusinessEdit.Rows[0]["CareOf"].ToString())))
        {
            previewImage.ImageUrl = "~/Uploads/" + dtBusinessEdit.Rows[0]["CareOf"].ToString();
        }

        DataTable dtBusinessEdit1 = new DataTable();
        string editMother = dtBusinessEdit.Rows[0]["MotherOrgId"].ToString(); int editMotherId; // v64 guard: missing MotherOrg row
        if (Int32.TryParse(editMother, out editMotherId)) dtBusinessEdit1 = comm.GetAllDataFromTable("MotherOrg with (Nolock)", "*", "MotherOrgID=" + editMotherId + "", "");
        txtaccountantmail.Text = dtBusinessEdit1.Rows.Count > 0 ? dtBusinessEdit1.Rows[0]["Email"].ToString() : "";
        DataTable chkboisActive = comm.GetAllDataFromTable("AddBusiness with (Nolock)", "*", "BusinessID=" + BusinessIDEdit + " and APPROVED='A'", "");
        if (chkboisActive.Rows.Count == 0)
        {
            txtaccountantmail.Enabled = false;
        }
        else
        {
            txtaccountantmail.Enabled = true;

        }
        DateTime agreeDt; DateBusnsAssgnDate.Text = DateTime.TryParse(dtBusinessEdit.Rows[0]["BusinessAgreementDate"].ToString(), out agreeDt) ? agreeDt.ToString("yyyy-MM-dd") : ""; // v64 guard
        string storedStatus = dtBusinessEdit.Rows[0]["Status"].ToString().Trim();
        if (storedStatus == "2" && ddlClientStats.Items.FindByValue("2") == null)
            ddlClientStats.Items.Add(new ListItem("Not Active (deactivated by plan)", "2"));
        SelectStoredOption(ddlClientStats, storedStatus);
        hfdBusinessId.Value = BusinessIDEdit.ToString();
        if (dtBusinessEdit.Rows[0]["ADDRESSID"].ToString() != "")
        {
            GetAddressDetailEdit(int.Parse(dtBusinessEdit.Rows[0]["ADDRESSID"].ToString()));
        }

        GetBusNameForAllTab(BusinessIDEdit);
    }

    private static void SelectStoredOption(DropDownList list, string value)
    {
        value = (value ?? "").Trim();
        ListItem item = list.Items.FindByValue(value) ?? list.Items.FindByText(value);
        if (item == null) { item = new ListItem(value, value); list.Items.Add(item); }
        list.ClearSelection(); item.Selected = true;
    }

    public void GetAddressDetailEdit(int ADDRESSID)
    {
        btnSaveAddres.Text = "Update";
        btnSaveAddres.Visible = false;
        btnUpdateAddress.Visible = true;

        DataTable dtBusinessAddressEdit = comm.GetAllDataFromTable("BusinessAddress with (Nolock)", "*", "BusinessAddressId=" + ADDRESSID + "", "");

        Country.Text = dtBusinessAddressEdit.Columns.Contains("Country") && dtBusinessAddressEdit.Rows[0]["Country"] != DBNull.Value
            ? Convert.ToString(dtBusinessAddressEdit.Rows[0]["Country"]) : "Canada";
        ViewState["ManualBusinessAddress"] = true;
        EnableAddressFields();
        txtPostalCode.Text = dtBusinessAddressEdit.Rows[0]["Postalcode"].ToString();
        txtPostalCode.Enabled = true;
        SelectStoredOption(ddlNumberSuffix, dtBusinessAddressEdit.Rows[0]["NumberSuffix"].ToString());
        txtStreetNo.Text = dtBusinessAddressEdit.Rows[0]["StreetNo"].ToString();
        txtStreetName.Text = dtBusinessAddressEdit.Rows[0]["StreetName"].ToString();
        txtStreetType.Text = dtBusinessAddressEdit.Rows[0]["StreetType"].ToString();
        txtStreetType1.Text = dtBusinessAddressEdit.Rows[0]["StreetType"].ToString();
        txtStreetDirection.Text = dtBusinessAddressEdit.Rows[0]["StreetDirection"].ToString();
        txtStreetDirection1.Text = dtBusinessAddressEdit.Rows[0]["StreetDirection"].ToString();
        SelectStoredOption(ddlSuitType, dtBusinessAddressEdit.Rows[0]["SuitType"].ToString());
        txtSuiteNumber.Text = dtBusinessAddressEdit.Rows[0]["SuiteNumber"].ToString();
        SelectStoredOption(ddlsuitesuffix, dtBusinessAddressEdit.Rows[0]["SuitSuffix"].ToString());
        txtCity.Text = dtBusinessAddressEdit.Rows[0]["City"].ToString();
        txtCity.Enabled = true;
        txtProvince.Text = dtBusinessAddressEdit.Rows[0]["Province"].ToString();
        txtProvince.Enabled = true;
        txtMainPhone.Text = dtBusinessAddressEdit.Rows[0]["MainPhone"].ToString();
        txtWorkPhone.Text = dtBusinessAddressEdit.Rows[0]["WorkPhone"].ToString();
        txtMobile.Text = dtBusinessAddressEdit.Rows[0]["Mobile"].ToString();
        txtFax.Text = dtBusinessAddressEdit.Rows[0]["Fax"].ToString();
        txtMainEmail.Text = dtBusinessAddressEdit.Rows[0]["MainEmail"].ToString();
        txtCCEmail.Text = dtBusinessAddressEdit.Rows[0]["CCEmail"].ToString();
        txtWebsite.Text = dtBusinessAddressEdit.Rows[0]["Website"].ToString();
        txtOther1.Text = dtBusinessAddressEdit.Rows[0]["Other1"].ToString();
    }

    protected void btnSaveBusinessDetails_Click(object sender, EventArgs e)
    {
        AddressMessage.Visible = false;
        SaveError.Visible = false;
        if (!String.IsNullOrWhiteSpace(Label_Maximum_Limit.Text))
        { SaveFeedback("Business changes are unavailable under the current plan or account status."); return; }
        EnableAddressFields();
        // v3: Status is not on the Add Business form (deck slide 1); a new business starts Active.
        if (Request.QueryString["BusinessIDEdit"] == null && ddlClientStats.SelectedValue == "") ddlClientStats.SelectedValue = "1";
        // Validate (including the 10-digit primary contact number) before any upload or insert.
        string error = ValidateBusinessAndAddress();
        if (error != null) { ExtraBusinessDetails.Attributes["open"] = "open"; SaveFeedback(error); return; }
        int business = ExistingBusinessId();
        bool updating = business > 0;
        string logo;
        try { logo = GenerateUniqueFileName(); }
        catch (InvalidOperationException imageError) { SaveFeedback(imageError.Message); return; }
        var details = new Dictionary<string, object> {
            {"CompanyName", txtCompanyName.Text.Trim()}, {"OperatingName", txtOperatingName.Text.Trim()},
            {"BusinessNumber", txtBusinessNumber.Text.Trim()}, {"LegalEntityType", ddlLegalEnityType.SelectedValue},
            {"BusinessRegistrationDate", BsnsRegDate.Text}, {"PrimaryContactName", PrmContName.Text.Trim()},
            {"PrimaryContactNumber", PrmContNmbr.Text.Trim()}, {"BusinessAgreementDate", DateTime.Now},
            {"Status", ddlClientStats.SelectedValue}, {"MotherOrgId", Convert.ToInt32(Session["MotherOrgid"])},
            {"CareOf", logo}
        };
        var address = new Dictionary<string, object> {
            {"Postalcode", AnyminuteBusinessAddressInput.Postal(txtPostalCode.Text)},
            {"NumberSuffix", ddlNumberSuffix.SelectedItem.Text}, {"StreetName", txtStreetName.Text.Trim()},
            {"StreetType", txtStreetType.Text.Trim()}, {"StreetDirection", txtStreetDirection.Text},
            {"SuitType", ddlSuitType.SelectedItem.Text}, {"SuiteNumber", txtSuiteNumber.Text.Trim()},
            {"SuitSuffix", ddlsuitesuffix.SelectedItem.Text}, {"City", txtCity.Text.Trim()}, {"Province", txtProvince.Text.Trim()},
            {"WorkPhone", txtWorkPhone.Text}, {"MainPhone", String.IsNullOrWhiteSpace(txtMainPhone.Text) ? PrmContNmbr.Text.Trim() : txtMainPhone.Text.Trim()}, {"Mobile", txtMobile.Text.Trim()},
            {"Fax", txtFax.Text}, {"MainEmail", txtMainEmail.Text.Trim()}, {"CCEmail", txtCCEmail.Text},
            {"Website", txtWebsite.Text}, {"Other1", txtOther1.Text}, {"StreetNo", txtStreetNo.Text.Trim()}
        };
        try
        {
            UploadAndSaveAttachment(logo);
            business = AnyminuteBusinessSave.Save(business, details, address, Country.Text.Trim(),
                FileUpload1.HasFile, Convert.ToString(Session["email"]));
        }
        catch (Exception saveError)
        {
            // Only the newly generated upload belongs to this failed save. Never touch an existing logo.
            if (!String.IsNullOrEmpty(logo))
                try { File.Delete(Server.MapPath("~/Uploads/" + logo)); }
                catch (Exception cleanupError) { Trace.Warn("BusinessLogoCleanup", "Could not remove an unreferenced upload.", cleanupError); }
            // Expected business-rule failures (duplicate name, missing owner record, unlinked address)
            // are shown on the form instead of the generic error page. Nothing was committed.
            if (saveError is InvalidOperationException)
            { ExtraBusinessDetails.Attributes["open"] = "open"; SaveFeedback(saveError.Message + " No changes were saved."); return; }
            throw; // Preserve SQL/permission exceptions; no success feedback after failure.
        }
        RememberSavedBusiness(business);
        btnSaveBusinessDetails.Text = "Update"; btnSaveAddres.Text = "Update";
        // Post/Redirect/Get: the confirmation is shown at the top of the page the user lands on,
        // and refreshing that page cannot submit the form a second time.
        string savedName = txtCompanyName.Text.Trim();
        if (updating)
        {
            AnyminuteFlash.Set("business", "Business details for \u201C" + savedName + "\u201D were updated successfully.");
            Response.Redirect("Add_Business.aspx?BusinessIDEdit=" + business, false);
        }
        else
        {
            AnyminuteFlash.Set("dashboard", "Business \u201C" + savedName + "\u201D was saved successfully.");
            Response.Redirect("Dashboard.aspx?view=businesses&saved=" + business, false);
        }
        Context.ApplicationInstance.CompleteRequest();
    }

    // Save problems are shown in a banner directly under the page header (always visible after the postback).
    private void SaveFeedback(string message)
    {
        SaveError.Text = HttpUtility.HtmlEncode(message);
        SaveError.Visible = true;
        AddressMessage.Visible = false;
    }

    private void EnsureSaveKey()
    {
        if (String.IsNullOrEmpty(Convert.ToString(ViewState["AddBusinessSaveKey"])))
            ViewState["AddBusinessSaveKey"] = Guid.NewGuid().ToString("N");
    }

    // Query string (edit), ViewState/hidden id (same form after a first save), then the
    // one-time form key in Session (a second postback from a double-click still carries
    // the original empty ViewState, but Session already has the id from the first request).
    private int ExistingBusinessId()
    {
        int id;
        if (Request.QueryString["BusinessIDEdit"] != null && Int32.TryParse(Request.QueryString["BusinessIDEdit"], out id) && id > 0)
            return id;
        if (ViewState["SavedBusinessId"] != null && Int32.TryParse(Convert.ToString(ViewState["SavedBusinessId"]), out id) && id > 0)
            return id;
        if (Int32.TryParse(hfdBusinessId.Value, out id) && id > 0) return id;
        string key = Convert.ToString(ViewState["AddBusinessSaveKey"]);
        if (String.IsNullOrEmpty(key) || Session == null) return 0;
        object prior = Session["AddBusinessSave:" + key];
        if (prior != null && Int32.TryParse(Convert.ToString(prior), out id) && id > 0) return id;
        return 0;
    }

    private void RememberSavedBusiness(int business)
    {
        ViewState["SavedBusinessId"] = business;
        hfdBusinessId.Value = business.ToString();
        string key = Convert.ToString(ViewState["AddBusinessSaveKey"]);
        if (!String.IsNullOrEmpty(key) && Session != null) Session["AddBusinessSave:" + key] = business;
    }

    private string ValidateBusinessAndAddress()
    {
        if (String.IsNullOrWhiteSpace(txtCompanyName.Text) || String.IsNullOrWhiteSpace(PrmContName.Text))
            return "Enter the company name and primary contact name.";
        if (Regex.Replace(PrmContNmbr.Text, @"\D", "").Length != 10)
            return "Enter a valid 10-digit primary contact number.";
        string postal = AnyminuteBusinessAddressInput.Postal(txtPostalCode.Text);
        // 2026-10-26 v19 (t76u): on an EXISTING business an empty postal code no longer blocks saving the business
        // details (company name, contact, email, status ...). The stored address is kept as it is; postal format and
        // address completeness are only enforced for a new business or when a postal code was actually entered.
        bool editing = Request.QueryString["BusinessIDEdit"] != null || ViewState["SavedBusinessId"] != null;
        // 2026-10-30 v32 (t107u): on Update Business Profile the stored address (incl. a legacy/odd postal code) never blocks
        // Save Changes; new addresses are checked in the address modal. A new business may also be saved with no postal code;
        // the postal format is only checked when a postal code was entered.
        bool keepAddress = editing;
        if (!keepAddress)
        {
            if (String.IsNullOrWhiteSpace(txtStreetNo.Text) || String.IsNullOrWhiteSpace(txtStreetName.Text) ||
                String.IsNullOrWhiteSpace(txtCity.Text) || String.IsNullOrWhiteSpace(txtProvince.Text))
                return "Enter the street number, address line 1, city and province/state.";
            if (String.IsNullOrWhiteSpace(Country.Text) || Country.Text.Trim().Length > 100)
                return "Enter a country (maximum 100 characters).";
            if (postal.Length > 0 && !Regex.IsMatch(postal, @"^[A-Z0-9-]{5,10}$")) return "Enter a valid postal code.";
            if (postal.Length > 0 && !PostalValidFor(Country.Text, postal))
                return String.Equals(Country.Text.Trim(), "United States", StringComparison.OrdinalIgnoreCase)
                    ? "Enter a valid US ZIP code (e.g. 33185)." : "Enter a valid Canadian postal code (e.g. M1B 2W1).";
        }
        else if (Country.Text.Trim().Length > 100) return "Enter a country (maximum 100 characters).";
        // v3: Contact Phone / Mobile are no longer on the form (not in the deck); stored values are kept as they are.
        if (!IsValidEmail(txtMainEmail.Text.Trim())) return "Enter a valid primary email address.";
        // "0" = Not Active (same value the Dashboard Active/Not Active menu writes); a legacy stored "2" is kept as-is.
        if (ddlClientStats.SelectedValue != "1" && ddlClientStats.SelectedValue != "0" && ddlClientStats.SelectedValue != "2")
            return "Select a status (Active or Not Active).";
        return null;
    }

    public void GetBusNameForAllTab(int BusId)
    {
        lblAddressBusName.Text = comm.GetNameFromId("AddBusiness", BusId);
        //lblbusNameForAccessPrivilege.Text = lblAddressBusName.Text;
    }

    public void ClearBusinessDetails()
    {
        txtCompanyName.Text = "";
        txtOperatingName.Text = "";
        txtBusinessNumber.Text = "";
        ddlLegalEnityType.SelectedValue = "";
        BsnsRegDate.Text = "";
        PrmContName.Text = "";
        PrmContNmbr.Text = "";
        DateBusnsAssgnDate.Text = "";
        BsnsRegDate.Text = "";
        ddlClientStats.SelectedValue = "";
    }

    public void ClearAddressContactDetails()
    {
        txtSearchPostalCode.Text = "";
        ResetAddressChoices(AddressPlaceholder);
        txtStreetNo.Text = "";
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtStreetDirection.Text = "";
        txtStreetType1.Text = "";
        txtStreetDirection1.Text = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtProvince.Text = "";
        //txtPostalCode.Visible = false;

        txtMainPhone.Text = "";
        txtWorkPhone.Text = "";
        txtMobile.Text = "";
        txtFax.Text = "";
        txtMainEmail.Text = "";
        txtCCEmail.Text = "";
        txtWebsite.Text = "";
        txtOther1.Text = "";

        txtPostalCode.Text = "";
        txtPostalCode.ReadOnly = false;
        txtStreetName.Text = "";
        txtStreetName.ReadOnly = false;
        txtStreetType.Text = "";
        txtStreetType1.Text = "";
        txtStreetType.ReadOnly = false;
        txtCity.Text = "";
        txtProvince.Text = "";

        btnSaveAddres.Text = "Save";
    }

    public void ClearAddress()
    {
        txtStreetName.Text = "";
        txtStreetType.Text = "";
        txtProvince.Text = "";
        txtStreetNo.Text = "";
        txtSuiteNumber.Text = "";
        txtCity.Text = "";
        txtPostalCode.Text = "";
    }

    protected void lnkSearch_Click(object sender, EventArgs e)
    {
        AddressMessage.Visible = false; AddressNotFound.Visible = false; LookupInfo.Visible = true;
        ResetAddressChoices(AddressPlaceholder);
        gvAddressList.DataSource = null; gvAddressList.DataBind();
        string postal = AnyminuteBusinessAddressInput.Postal(txtSearchPostalCode.Text);
        bool us = LookupCountry.SelectedValue == "United States";
        if (!PostalValidFor(LookupCountry.SelectedValue, postal))
        { AddressFeedback(us ? "Enter a valid US ZIP code (e.g. 33185)." : "Enter a valid Canadian postal code (e.g. M1B 2W1)."); return; }
        DataTable matches = AnyminuteAddressLookup.Search(postal);
        ViewState["BusinessAddressMatches"] = matches;
        ViewState["BusinessAddressLookupPostal"] = postal;
        resultdiv.Visible = true;
        AddressChoices.Items.Clear();
        AddressChoices.Items.Add(new ListItem(AddressPlaceholder, ""));
        for (int i = 0; i < matches.Rows.Count; i++)
        {
            string label = Convert.ToString(matches.Rows[i]["SEARCHADDRESS"]).Trim();
            if (label.Length == 0 || String.Equals(label, "ADD NEW ADDRESS", StringComparison.OrdinalIgnoreCase)) continue;
            var found = AnyminuteBusinessAddressInput.FromRow(matches.Rows[i], postal);
            if (found.Street.Length > 0 && found.City.Length > 0)
            {
                // Deck H result format: "Street, City, Province Postal, Country" (street numbers only when the official table has one).
                string first = (found.Number + " " + found.Street).Trim();
                string region = (found.Province + " " + PostalDisplay(found.PostalCode)).Trim();
                label = first + ", " + found.City + (region.Length > 0 ? ", " + region : "") + (found.Country.Length > 0 ? ", " + found.Country : "");
            }
            AddressChoices.Items.Add(new ListItem(label, i.ToString(System.Globalization.CultureInfo.InvariantCulture)));
        }
        AddressChoices.Enabled = AddressChoices.Items.Count > 1;
        if (AddressChoices.Items.Count == 1)
        {
            // v46 (paused deck s1): no hint, no plain-text message - only the "Address Not Found" card.
            LookupInfo.Visible = false; resultdiv.Visible = false;
            AddressNotFound.Text = AnyminuteAddressLookup.NotFoundHtml(postal); AddressNotFound.Visible = true;
        }
        else
        {
            AddressFeedback((AddressChoices.Items.Count - 1) + " matching address" + (AddressChoices.Items.Count == 2 ? "" : "es") + " found. Select one from the list to fill the address fields.", false);
            ScriptManager.RegisterStartupScript(UpdatePanel1, typeof(Add_Business), "amOpenAddressList", "if(window.AnyminuteBusinessAddress)AnyminuteBusinessAddress.openList();", true);
        }
        // Searching is read-only. Never select or save the first result automatically.
    }

    private void AddressFeedback(string message, bool error = true)
    {
        AddressMessage.Text = HttpUtility.HtmlEncode(message);
        AddressMessage.CssClass = error ? "am-notice am-error" : "am-notice";
        AddressMessage.Visible = true;
    }

    protected void AddressChoices_SelectedIndexChanged(object sender, EventArgs e)
    {
        // v11 Deck H step 3: choosing a result fills the address fields automatically (nothing is saved yet).
        AddressMessage.Visible = false; AddressNotFound.Visible = false; LookupInfo.Visible = true;
        if (AddressChoices.SelectedValue != "") ApplySelectedAddress();
    }

    protected void UseThisAddress_Click(object sender, EventArgs e)
    { ApplySelectedAddress(); }

    private void ApplySelectedAddress()
    {
        DataTable matches = ViewState["BusinessAddressMatches"] as DataTable;
        int index;
        if (matches == null || !Int32.TryParse(AddressChoices.SelectedValue, out index) || index < 0 || index >= matches.Rows.Count)
        { AddressFeedback("Search and select an address first, or select Enter Address Manually."); return; }
        var address = AnyminuteBusinessAddressInput.FromRow(matches.Rows[index], Convert.ToString(ViewState["BusinessAddressLookupPostal"]));
        if (address.Street.Length == 0 || address.City.Length == 0 || address.Province.Length == 0)
        { AddressFeedback("This result is incomplete. Select Enter Address Manually to complete it."); return; }
        ResetAddressFields();
        txtStreetNo.Text = address.Number; txtStreetName.Text = address.Street;
        txtStreetType.Text = address.Line2; txtCity.Text = address.City;
        txtProvince.Text = address.Province; txtPostalCode.Text = address.PostalCode;
        Country.Text = address.Country;
        ViewState["ManualBusinessAddress"] = true;
        EnableAddressFields();
        AddressFeedback("Address filled. Review the fields, add the street number or suite if needed, then click Save Changes.", false);
    }

    public bool IsValidEmail(string email)
    {
        var r = new Regex(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$");

        return !string.IsNullOrEmpty(email) && r.IsMatch(email);
    }

    protected void btnSaveAddres_Click(object sender, EventArgs e)
    {
        // Every Save validates and commits the business and address together.
        btnSaveBusinessDetails_Click(sender, e);
    }

    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetAutoCompleteDataCity(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "Select distinct CityName from CityMaster where CityName LIKE '%'+@SearchText+'%'";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["CityName"].ToString());
                    }
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
    }


    [System.Web.Script.Services.ScriptMethod()]
    [System.Web.Services.WebMethod]
    public static List<string> GetAutoCompleteDataProvince(string prefixText)
    {
        List<string> result = new List<string>();
        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConString"].ConnectionString))
        {
            int len = prefixText.Length;
            if (len > 2)
            {
                string querytxt = "Select distinct ProvinceName from ProvinceMaster where ProvinceName LIKE '%'+@SearchText+'%'";
                using (SqlCommand cmd = new SqlCommand(querytxt, con))
                {
                    con.Open();
                    cmd.Parameters.AddWithValue("@SearchText", prefixText);
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        result.Add(dr["ProvinceName"].ToString());
                    }
                    return result;
                }
            }
            else
            {
                return result;
            }
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (Page.Form != null) Page.Form.Enctype = "multipart/form-data";
        bool editing = Request.QueryString["BusinessIDEdit"] != null || ViewState["SavedBusinessId"] != null;
        BusinessPageTitle.Text = BusinessBreadcrumb.Text = editing ? "Update Business Profile" : "Add Business";
        SaveTopText.Text = editing ? "Save Changes" : "Save";
        // v3 field layout per deck: Add = 2 columns, no Status/Country/Clear Address; Update = 2/3/4 columns with Status and Country.
        BusinessDetailsCard.Attributes["class"] = "am-business-card am-fields-v3 " + (editing ? "am-mode-edit" : "am-mode-add");
        divAdd.Attributes["class"] = "am-al-details " + (editing ? "am-mode-edit" : "am-mode-add");
        // v10 address spec: Country (default Canada) and Clear Address in both Add and Update.
        StatusField.Visible = editing; CountryField.Style["display"] = "";
        ClearBusinessAddress.Visible = false; // v11 Deck H: not on the slide; Cancel restores the saved address instead.
        if (Country.Text.Trim().Length == 0) Country.Text = "Canada";
        if (!IsPostBack && String.Equals(Country.Text.Trim(), "United States", StringComparison.OrdinalIgnoreCase)) LookupCountry.SelectedValue = "United States";
        // Logo: picture when one exists, otherwise initials (edit) or the upload prompt (add).
        bool hasLogo = !String.IsNullOrEmpty(previewImage.ImageUrl);
        string initials = BusinessInitials(txtCompanyName.Text);
        LogoInitials.Text = HttpUtility.HtmlEncode(initials);
        uploadArea.Attributes["class"] = "am-logo-uploader" + (hasLogo ? " has-logo" : editing && initials.Length > 0 ? " has-initials" : "");
        ChangeLogoButton.Visible = editing && FileUpload1.Visible;
        RemoveBusinessLogo.Visible = editing && hasLogo && FileUpload1.Visible;
        SaveBusinessTop.Visible = (btnSaveBusinessDetails.Visible || btnUpdateBusinessDetails.Visible) && String.IsNullOrWhiteSpace(Label_Maximum_Limit.Text);
        btnSaveBusinessDetails.Text = editing ? "Save Changes" : "Save";
        BusinessStatusBadge.Visible = editing && ddlClientStats.SelectedValue != "";
        BusinessStatusBadge.Text = ddlClientStats.SelectedValue == "1" ? "Active" : "Not Active";
        BusinessStatusBadge.CssClass = "am-business-status " + (ddlClientStats.SelectedValue == "1" ? "Active" : "Inactive");
        if(Convert.ToBoolean(ViewState["ManualBusinessAddress"] ?? false))
        { divAdd.Visible = true; divAdd.Style["display"] = "block"; }
    }

    private static string BusinessInitials(string name)
    {
        var words = (name ?? "").Split(new[] { ' ', '.', '-', ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(w => Char.IsLetterOrDigit(w[0])).ToArray();
        if (words.Length == 0) return "";
        return (words.Length == 1 ? words[0].Substring(0, Math.Min(2, words[0].Length)) : String.Concat(words[0][0], words[1][0])).ToUpperInvariant();
    }

    protected void AddNewAddress_Click(object sender, EventArgs e)
    {
        ResetAddressFields();
        txtPostalCode.Text = AnyminuteBusinessAddressInput.Postal(txtSearchPostalCode.Text);
        ViewState["ManualBusinessAddress"] = true;
        EnableAddressFields(); AddressMessage.Visible = false;
        txtStreetNo.Focus();
    }

    protected void ClearBusinessAddress_Click(object sender, EventArgs e)
    {
        ResetAddressFields(); txtSearchPostalCode.Text = "";
        ResetAddressChoices(AddressPlaceholder);
        ViewState.Remove("BusinessAddressMatches"); ViewState.Remove("BusinessAddressLookupPostal");
        ViewState["ManualBusinessAddress"] = true;
        EnableAddressFields(); AddressMessage.Visible = false;
    }

    private const string AddressPlaceholder = "Select an address from the list below";
    private const string NoAddressFound = "No matching address found. Please verify the postal/ZIP code or select Enter Address Manually.";

    // v11 Deck H: postal/ZIP validation follows the selected country.
    private static bool PostalValidFor(string country, string postal)
    {
        string compact = (postal ?? "").Replace(" ", "").Replace("-", "").ToUpperInvariant();
        if (String.Equals((country ?? "").Trim(), "United States", StringComparison.OrdinalIgnoreCase))
            return Regex.IsMatch(compact, @"^\d{5}(\d{4})?$");
        if (String.Equals((country ?? "").Trim(), "Canada", StringComparison.OrdinalIgnoreCase))
            return Regex.IsMatch(compact, @"^[A-Z]\d[A-Z]\d[A-Z]\d$");
        return Regex.IsMatch(compact, @"^[A-Z0-9]{3,10}$");
    }

    private static string PostalDisplay(string postal)
    {
        string compact = (postal ?? "").Replace(" ", "").ToUpperInvariant();
        return Regex.IsMatch(compact, @"^[A-Z]\d[A-Z]\d[A-Z]\d$") ? compact.Substring(0, 3) + " " + compact.Substring(3) : compact;
    }

    private void ResetAddressChoices(string placeholder)
    {
        AddressChoices.Items.Clear();
        AddressChoices.Items.Add(new ListItem(placeholder, ""));
        AddressChoices.Enabled = false; UseThisAddress.Enabled = false;
        resultdiv.Visible = true;
    }

    private void ResetAddressFields()
    {
        ClearAddress(); txtStreetDirection.Text = ""; txtStreetDirection1.Text = "";
        txtStreetType1.Text = "";
        if (ddlNumberSuffix.Items.Count > 0) ddlNumberSuffix.SelectedIndex = 0;
        if (ddlSuitType.Items.Count > 0) ddlSuitType.SelectedIndex = 0;
        if (ddlsuitesuffix.Items.Count > 0) ddlsuitesuffix.SelectedIndex = 0;
        Country.Text = "Canada";
        // Business name, logo, contact details and website deliberately remain intact.
    }

    private void EnableAddressFields()
    {
        divAdd.Visible = true; divAdd.Style["display"] = "block";
        txtPostalCode.ReadOnly = txtStreetName.ReadOnly = txtCity.ReadOnly = txtProvince.ReadOnly = false;
        txtStreetType.ReadOnly = false;
        txtPostalCode.Enabled = txtStreetName.Enabled = txtCity.Enabled = txtProvince.Enabled = true;
    }

    protected void CancelBusiness_Click(object sender, EventArgs e)
    { Response.Redirect("Dashboard.aspx?view=businesses", false); Context.ApplicationInstance.CompleteRequest(); }

    protected void StartDate_Click(object sender, EventArgs e)
    {
        // Compatibility for previously bound result rows; IDs come from signed ViewState.
        LinkButton link = (LinkButton)sender;
        GridViewRow row = (GridViewRow)link.NamingContainer;
        string key = row.RowIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        if (AddressChoices.Items.FindByValue(key) != null)
        { AddressChoices.SelectedValue = key; ApplySelectedAddress(); }
        else AddressFeedback("Search again and select an address from the list.");
    }

    protected void btnUpdateBusinessDetails_Click(object sender, EventArgs e)
    { btnSaveBusinessDetails_Click(sender, e); }

    protected void btnUpdateAddress_Click(object sender, EventArgs e)
    {
        btnSaveAddres_Click(sender, e);
    }

    protected void txtSearchPostalCode_TextChanged(object sender, EventArgs e)
    {
        //int searchtextlenghth = int.Parse(txtSearchPostalCode.Text.Length.ToString());

        //if (searchtextlenghth > 3)
        //{
        //    string serachpostcode = txtSearchPostalCode.Text.Replace(" ", string.Empty).Trim();
        //    string padleft = serachpostcode.Insert(3, " ");
        //    txtSearchPostalCode.Text = padleft;
        //}
    }

    // Retain the handler for older merged markup. This obsolete field does not assign
    // an owner/accountant, and postal lookup must not run accountant validation.
    protected void txtaccountantmail_TextChanged(object sender, EventArgs e) { }

    private string GenerateUniqueFileName()
    {
        return FileUpload1.HasFile ? AnyminuteImageUploads.Name(FileUpload1.PostedFile) : "";
    }

    private void UploadAndSaveAttachment(string fileName)
    {
        if(FileUpload1.HasFile) AnyminuteImageUploads.SaveNamed(FileUpload1.PostedFile,Server,fileName);
    }

    protected void btn_Okay_Click(object sender, EventArgs e)
    {
        Response.Redirect("Home_Page.aspx");
    }

    protected void btn_Cancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("Add_EmployeeV2.aspx");
    }
    protected void RemoveLogo_Click(object sender,EventArgs e)
    {
        int b=Int32.Parse(Request.QueryString["BusinessIDEdit"]);
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(System.Data.IsolationLevel.Serializable))
        {int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,b);
         AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AddBusiness SET CareOf='' WHERE BusinessID=@b; INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,RecordedAtUTC) VALUES(@b,@a,@a,'RemoveBusinessLogo',SYSUTCDATETIME())","@b",b,"@a",actor);tx.Commit();}
        previewImage.ImageUrl="";
    }
    protected void DeactivateBusiness_Click(object sender,EventArgs e)
    {
        int b=Int32.Parse(Request.QueryString["BusinessIDEdit"]);
        using(var c=AnyminuteTimesheetRepair.Open())using(var tx=c.BeginTransaction(System.Data.IsolationLevel.Serializable))
        {int actor=AnyminuteApprovalAssignments.RequireOwner(c,tx,b);
         AnyminuteTimesheetRepair.Execute(c,tx,"UPDATE AddBusiness SET Status=0 WHERE BusinessID=@b; INSERT AnyminuteApprovalAudit(BusinessID,ActorID,EmployeeID,Action,AfterValue,RecordedAtUTC) VALUES(@b,@a,@a,'DeactivateBusiness','0',SYSUTCDATETIME())","@b",b,"@a",actor);tx.Commit();}
        Response.Redirect("Dashboard.aspx?view=businesses");
    }
}