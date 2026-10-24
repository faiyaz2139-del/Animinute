using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
public partial class payment_Success : System.Web.UI.Page
{
    CommanClass comm = new CommanClass();
    AddEmployeeControlerClass EmpCtrl = new AddEmployeeControlerClass();
    AddEmployeeEntityClass EmpEntity = new AddEmployeeEntityClass();
    RateDetailsEntity RateEntity = new RateDetailsEntity();
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    EmpAddressController EmpAddCtrl = new EmpAddressController();
    UserAccController Uac = new UserAccController();
    UserAccEntity Uae = new UserAccEntity();
    EmailSettingController Esctrl = new EmailSettingController();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"].ToString();
            var session_id = Request.QueryString["session_id"];
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            if (!string.IsNullOrEmpty(session.Metadata["empId"]))
            {
                UpdatePlanHistory(session, Convert.ToInt32(session.Metadata["empId"]));

                divSuccess.Visible = true;
                divError.Visible = false;
            }
            else
            {
                int empId = SavePersonalDetails(session.Metadata);
                if (empId > 0)
                {
                    if (session.Metadata["source"] == "B")
                    {
                        int saveAddressFlag = SaveAddress(session.Metadata, empId);
                        if (saveAddressFlag > 0)
                        {
                            UpdateOtherDetails(session.Metadata, empId);
                            SendEmail(session.Metadata, empId);
                            UpdatePlanHistory(session, empId);
                        }
                        divSuccess.Visible = true;
                        divError.Visible = false;
                    }
                    else
                    {
                        SendPartnerEmail(session.Metadata, empId);
                        UpdatePlanHistory(session, empId);
                        divSuccess.Visible = true;
                        divError.Visible = false;

                        //Response.Redirect("~/Enterprise_Plan_Reseller.aspx");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            divSuccess.Visible = false;
            divError.Visible = true;
            lblErrorSms.InnerText = ex.Message;
        }
    }

    string Send_Email_Id;
    private int SavePersonalDetails(Dictionary<string, string> metaData)
    {
        try
        {
            EmpEntity.City = "";
            EmpEntity.Province = "";
            EmpEntity.MobilePhone = metaData["phoneNo"];
            EmpEntity.UserType = 0;
            EmpEntity.LegalName = "";
            EmpEntity.Title = "";
            EmpEntity.FirstName = metaData["firstName"];
            EmpEntity.MiddleName = "";
            EmpEntity.LastName = metaData["lastName"];
            EmpEntity.SocialInsuranceNumber = "";
            EmpEntity.DateOfBirth = "";
            EmpEntity.Gender = "";
            EmpEntity.HireDate = DateTime.Parse(DateTime.Now.Date.ToString());
            EmpEntity.IsActive = "YES";
            EmpEntity.Apartment = "";
            EmpEntity.TerminationDate = DateTime.Now.Date;
            EmpEntity.MotherOrgID = 4; //Right now the default accountant mother org id is 4.
            EmpEntity.Occupation = "";
            RateEntity.IsEmployeeEIExempty = "";
            RateEntity.IsEmployeeCPPExempty = "";
            RateEntity.RetainVacation = "";
            int Empid;
            if (metaData["source"] == "B")
            {
                Empid = EmpCtrl.InsertPersonalDetails(EmpEntity, "", RateEntity);
                comm.ExecuteQuery("update AddEmployee set EMPLOYEETYPE='NEW_AC_BO' where EmployeeID='" + Empid + "'");
            }
            else
            {
                EmpEntity.EMail_primary = metaData["emailId"];
                EmpEntity.EMail_secondary = metaData["emailId"];
                Empid = EmpCtrl.InsertPartnerPersonalDetails(EmpEntity, "", RateEntity);

                DataTable Get_Latest_Reseller_Details = comm.GetAllDataFromTable("Register_Partner", "*", "EMail_primary='" + metaData["emailId"] + "'", "");

                //int MotherOrgId = 0;
                var check_mo = comm.IfExistsInTable("MotherOrg", "*", "Email='" + metaData["emailId"] + "'");
                if (!check_mo)
                {
                    MotherEc.MotherOrgName = Get_Latest_Reseller_Details.Rows[0]["FirstName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["MiddleName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["LastName"].ToString() + '-' + Get_Latest_Reseller_Details.Rows[0]["PartnerID"].ToString();
                    MotherEc.StreetNumber = "";
                    MotherEc.StreetName = "";
                    MotherEc.Apartment = "";
                    MotherEc.City = "";
                    MotherEc.Province = "";
                    MotherEc.PostalCode = "";
                    MotherEc.EnrollDate = DateTime.Parse(Get_Latest_Reseller_Details.Rows[0]["HireDate"].ToString());
                    MotherEc.Status = true;
                    MotherEc.Email = metaData["emailId"];
                    Send_Email_Id = metaData["emailId"];
                    MotherEc.ContactName = Get_Latest_Reseller_Details.Rows[0]["FirstName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["MiddleName"].ToString() + ' ' + Get_Latest_Reseller_Details.Rows[0]["LastName"].ToString();
                    MotherEc.ContactNumber = Get_Latest_Reseller_Details.Rows[0]["MobilePhone"].ToString();
                    MotherEc.StreetNumber = Get_Latest_Reseller_Details.Rows[0]["StreetNumber"].ToString();
                    MotherEc.Checked = "N";
                    //MotherOrgId = MotherC.InsertMotherOrg(MotherEc);
                    Empid = MotherC.InsertMotherOrg(MotherEc);
                }
            }
            return Empid;
        }
        catch (Exception ex)
        {
            throw ex;
            lblErrorSmstwo.InnerText = ex.Message;
        }
    }

    private int SaveAddress(Dictionary<string, string> metaData, int empId)
    {
        try
        {
            EmpAddEnity.Postalcode = "";
            EmpAddEnity.NumberSuffix = "";
            EmpAddEnity.StreetName = "";
            EmpAddEnity.StreetType = "";
            EmpAddEnity.StreetDirection = "";
            EmpAddEnity.SuitType = "";
            EmpAddEnity.SuiteNumber = "";
            EmpAddEnity.SuitSuffix = "";
            EmpAddEnity.City = "";
            EmpAddEnity.Province = "";
            EmpAddEnity.MainPhone = metaData["mobileNo"];
            EmpAddEnity.WorkPhone = metaData["phoneNo"];
            EmpAddEnity.Mobile = metaData["mobileNo"];
            EmpAddEnity.Fax = "";
            EmpAddEnity.MainEmail = metaData["emailId"];
            EmpAddEnity.CCEmail = metaData["emailId"];
            EmpAddEnity.Website = "";
            EmpAddEnity.Other1 = "";
            EmpAddEnity.StreetNo = "";
            RateEntity.IsEmployeeEIExempty = "";
            RateEntity.IsEmployeeCPPExempty = "";
            RateEntity.RetainVacation = "";
            EmpAddEnity.MotherOrgId = 4;

            return EmpAddCtrl.InsertEmpAddress(EmpAddEnity, empId);
        }
        catch (Exception ex)
        {
            throw ex;
            lblErrorSmsthree.InnerText = ex.Message;
        }
    }

    private void UpdateOtherDetails(Dictionary<string, string> metaData, int empId)
    {
        comm.ExecuteQuery("update AddEmployee set UserType=" + 1 + " where EmployeeID=" + empId + "");
        Uae.Email = metaData["emailId"];
        Uae.EmployeeId = empId;
        Uae.MotherOrgid = 4;
        Uae.UserType = "BUSINESS OWNER";
        Uae.UserTypeid = 1;
        Uae.ProfileImageName = metaData["fileName"];
        var userId = Uac.Sp_InsertUserAccFromAddress(Uae);

        comm.ExecuteQuery("Insert into AssginBussiness (EmployeeID,EmployeeName,UserTypeId)values('" + empId + "','" + metaData["firstName"] + "',1)");

        Uae.UserId = int.Parse(userId);
        Uae.UserTypeid = 1;
        Uae.EmployeeId = empId;
        Uae.BusinessID = 0;
        Uac.Sp_InsertMapping(Uae);
    }

    private void SendEmail(Dictionary<string, string> metaData, int empId)
    {
        DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.MainEmail='" + metaData["emailId"] + "'", "ae.EmployeeID desc");
        //DataTable dtemp = comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "eAdd.MainEmail IN (select top 1 MainEmail from EmpAddress order by 1 desc)", "ae.EmployeeID desc");
        if (dtemp.Rows.Count > 0)
        {
            string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
            string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=3", "");
            if (dtEmailTemp.Rows.Count > 0)
            {
                DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendHtmlFormattedEmail(dtemp.Rows[0]["MainEmail"].ToString(), dtEmailTemp.Rows[0]["Subject"].ToString(), body);
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
            }
        }

        DataTable dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=6", "");
        if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        {
            string body = Esctrl.PopulateBodyNoUrl("Support Admin", "New Standard Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), "New_Standard_Plan_Request.htm", dtemp.Rows[0]["MainEmail"].ToString(), dtemp.Rows[0]["FirstName"].ToString()+' '+ dtemp.Rows[0]["MiddleName"].ToString()+' '+ dtemp.Rows[0]["LastName"].ToString());
            try
            {
                bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSend)
                    Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", "New Standard Plan Request", body);
            }
            catch
            {
                //ignore
            }
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
        }

        //string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        //string url = Host + empId.ToString();
        //var dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=4", "");
        //if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        //{
        //    var _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = 'supportadmin@gmail.com'", "");
        //    string body = Esctrl.PopulateBodyWithUrl(metaData["firstName"], "New BO Registration Notification", url, dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com" + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
        //    try
        //    {
        //        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
        //        if (IsSend)
        //            Esctrl.SendHtmlFormattedEmail(metaData["emailId"], dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
        //    }
        //    catch
        //    {
        //        //ignore
        //    }

        //}
    }

    private void SendPartnerEmail(Dictionary<string, string> metaData, int empId)
    {
        //string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        //bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
        //string url = Host + empId.ToString();
        //var dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=4", "");
        //if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        //{
        //    try
        //    {
        //        string body = Esctrl.PopulateBodyWithUrl(metaData["firstName"], "New BO Registration Notification", url, dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), dtEmail_Send_SupportAdmin.Rows[0]["TempName"].ToString(), "supportadmin@gmail.com", "");
        //        if (IsSend)
        //            Esctrl.SendHtmlFormattedEmail(metaData["emailId"], dtEmail_Send_SupportAdmin.Rows[0]["Subject"].ToString(), body);
        //    }
        //    catch
        //    {
        //        //ignore
        //    }
        //}

        DataTable Dt_motherOrgId = new DataTable();
        bool IsSend = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
        if (IsSend)
        {
            Dt_motherOrgId = comm.GetAllDataFromTable("MotherOrg", "MotherOrgID,ContactName", "Email='" + metaData["emailId"] + "'", "");
            string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
            //string url = Host + Motherid.ToString() + "&LoginType=Acc"; //commented because motherorgid is coming 0 when activating 3rd or 4th time.
            string url = Host + Dt_motherOrgId.Rows[0]["MotherOrgID"].ToString() + "&LoginType=Acc";
            DataTable dtEmailTemp = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
            //DataTable dtemp = Comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Motherid + "", ""); //commented because giving error as motherorgid is coming 0 when activating 3rd or 4th time.
            DataTable dtemp = comm.GetAllDataFromTable("MotherOrg", "*", "Email='" + metaData["emailId"] + "'", "");
            DataTable _Old_Password = comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + metaData["emailId"] + "' ", "");
            string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["ContactName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), Send_Email_Id.ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
            Esctrl.SendHtmlFormattedEmail(metaData["emailId"], dtEmailTemp.Rows[0]["Subject"].ToString(), body);
        }

        var dtEmail_Send_SupportAdmin = comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=6", "");
        if (dtEmail_Send_SupportAdmin.Rows.Count > 0)
        {
            string body_new = Esctrl.PopulateBodyNoUrl("Support Admin", "New Standard Plan Request", "", dtEmail_Send_SupportAdmin.Rows[0]["description"].ToString(), "New_Standard_Plan_Request.htm", metaData["emailId"], Dt_motherOrgId.Rows[0]["ContactName"].ToString());
            try
            {
                bool IsSendSPAdminMail = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSendMail"].ToString());
                if (IsSendSPAdminMail)
                    Esctrl.SendHtmlFormattedEmail("prashant@mfksoftware.com", "New Standard Plan Request", body_new);
            }
            catch
            {
                //ignore
            }
        }
        else
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email Template Not Found In DataBase', 'warning');", true);
        }
    }


    private void UpdatePlanHistory(Session session, int empId)
    {
        var planId = session.Metadata["productId"];
        var priceId = session.Metadata["priceId"];
        StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"].ToString();
        var service = new ProductService();

        var plan = string.IsNullOrEmpty(session.Metadata["planName"])
            ? service.Get(planId)
            : null;
        var planName = plan == null ? session.Metadata["planName"] : plan.Name;
        var planStartDate = string.IsNullOrEmpty(session.Metadata["planStartDate"]) ? DateTime.Today : Convert.ToDateTime(session.Metadata["planStartDate"]);
        var planEndDate = string.IsNullOrEmpty(session.Metadata["planEndDate"]) ? DateTime.Today.AddDays(365) : Convert.ToDateTime(session.Metadata["planEndDate"]);

        decimal Amount_paid_save = Convert.ToDecimal(session.AmountTotal) / 100;

        if (planName.ToString() == "STANDARD PLAN")
        {
            if (empId != 0)//for the subscriber is upgrading to standard plan from  basic plan, also for subscriber and reseller both first time standard plan registration
            {
                comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + empId + "' and EmailId='" + session.Metadata["emailId"] + "' ");
                comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive) " +
                    "values('" + planId + "','" + priceId + "','" + planName + "','" + planStartDate + "', '" + planEndDate + "','" + Amount_paid_save + "'," +
                    "'" + session.Id + "','" + empId + "','" + session.Metadata["emailId"] + "','10','10000','1')");

                comm.ExecuteQuery("update PlanHistory set Is_Requested_Plan=0 where EmpId='" + empId + "' " +
                    "and EmailId='" + session.Metadata["emailId"] + "' and Is_Requested_Plan=1");
            }
            if (empId == 0)//for the reseller is upgrading to standard plan from  basic plan
            {
                DataTable Get_MotherOrgId_Reseller = comm.GetAllDataFromTable("UserAcc", "MotherOrgid", "Email='" + session.Metadata["emailId"] + "' and EmployeeId=0", "");
                comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + Get_MotherOrgId_Reseller.Rows[0]["MotherOrgid"].ToString() + "' and EmailId='" + session.Metadata["emailId"] + "' ");
                comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive) " +
                    "values('" + planId + "','" + priceId + "','" + planName + "','" + planStartDate + "', '" + planEndDate + "','" + Amount_paid_save + "'," +
                    "'" + session.Id + "','" + Get_MotherOrgId_Reseller.Rows[0]["MotherOrgid"].ToString() + "','" + session.Metadata["emailId"] + "','10','10000','1')");

                comm.ExecuteQuery("update PlanHistory set Is_Requested_Plan=0 where EmpId='" + Get_MotherOrgId_Reseller.Rows[0]["MotherOrgid"].ToString() + "' " +
                    "and EmailId='" + session.Metadata["emailId"] + "' and Is_Requested_Plan=1");
            }
        }
        if (planName.ToString() != "STANDARD PLAN")
        {
            //if (empId != 0)
            //{
            //    comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + empId + "'");
            //    comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive) " +
            //    "values('" + planId + "','" + priceId + "','ENTERPRISE PLAN','" + planStartDate + "', '" + planEndDate + "','" + Amount_paid_save + "'," + "'" + session.Id + "'," +
            //    "'" + empId + "','" + session.Metadata["emailId"] + "','" + session.Metadata["users"] + "','" + session.Metadata["business"] + "','1')");
            //}
            //if (empId == 0)
            //{
            //    DataTable Get_MotherOrgId_Reseller = comm.GetAllDataFromTable("UserAcc", "MotherOrgid", "Email='" + session.Metadata["emailId"] + "' and EmployeeId=0", "");
            //    comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + Get_MotherOrgId_Reseller.Rows[0]["MotherOrgid"].ToString() + "' and EmailId='" + session.Metadata["emailId"] + "' ");
            //    comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive) " +
            //    "values('" + planId + "','" + priceId + "','ENTERPRISE PLAN','" + planStartDate + "', '" + planEndDate + "','" + Amount_paid_save + "'," + "'" + session.Id + "'," +
            //    "'" + Get_MotherOrgId_Reseller.Rows[0]["MotherOrgid"].ToString() + "','" + session.Metadata["emailId"] + "'," +
            //    "'" + session.Metadata["users"] + "','" + session.Metadata["business"] + "','1')");
            //}
            
            comm.ExecuteQuery("update PlanHistory set IsActive=0 where EmpId='" + empId + "' and EmailId='" + session.Metadata["emailId"] + "' ");
            comm.ExecuteQuery("insert into PlanHistory (PlanId,PriceId,PlanName,StartDate,EndDate,AmountPaid,TransId,EmpId,EmailId,No_Of_Users,Business,IsActive) " +
            "values('" + planId + "','" + priceId + "','ENTERPRISE PLAN','" + planStartDate + "', '" + planEndDate + "','" + Amount_paid_save + "'," + "'" + session.Id + "'," +
            "'" + empId + "','" + session.Metadata["emailId"] + "','" + session.Metadata["users"] + "','" + session.Metadata["business"] + "','1')");

            comm.ExecuteQuery("update PlanHistory set Is_Requested_Plan=0 where EmpId='" + empId + "' and EmailId='" + session.Metadata["emailId"] + "' " +
                "and Is_Requested_Plan=1");
        }
    }
}