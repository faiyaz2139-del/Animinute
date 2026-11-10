using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddBusinessControllerClass
/// </summary>
public class AddBusinessControllerClass
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    AddBusinessAddress BusinessAddressEntity = new AddBusinessAddress();

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

    SqlCommand cmd = new SqlCommand();
    DataSet ds = new DataSet();
    int i;
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
    public AddBusinessControllerClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void InsertAddBusiness(AddBusinessEntityClass be)
    {
        try
        {
            loginfo = "InsertAddBusiness(" + be + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_InsertAddBusiness";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CompanyName", be.CompanyName);
            cmd.Parameters.AddWithValue("@OperatingName", be.OperatingName);
            cmd.Parameters.AddWithValue("@BusinessNumber", be.BusinessNumber);
            cmd.Parameters.AddWithValue("@LegalEntityType", be.LegalEntityType);
            cmd.Parameters.AddWithValue("@BusinessRegistrationDate", be.BusinessRegistrationDate);
            cmd.Parameters.AddWithValue("@PrimaryContactName", be.PrimaryContactName);
            cmd.Parameters.AddWithValue("@PrimaryContactNumber", be.PrimaryContactNumber);
            cmd.Parameters.AddWithValue("@CareOf", be.CareOf);
            cmd.Parameters.AddWithValue("@StreetNumber", be.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", be.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", be.Apartment);
            cmd.Parameters.AddWithValue("@City", be.City);
            cmd.Parameters.AddWithValue("@Province", be.Province);
            cmd.Parameters.AddWithValue("@PostalCode", be.PostalCode);
            cmd.Parameters.AddWithValue("@BusinessAgreementDate", be.BusinessAgreementDate);
            cmd.Parameters.AddWithValue("@Status", be.Status);
            cmd.Parameters.AddWithValue("@MotherOrgId", be.MotherOrgId);
            cmd.ExecuteNonQuery();

            loginfo = "InsertAddBusiness(" + be + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAddBusiness" + ex.Message);
            throw ex;
        }
    }

    public void UpdateAddBusiness(AddBusinessEntityClass be)
    {
        try
        {
            loginfo = "UpdateAddBusiness(" + be + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_UpdateAddBusiness";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BusinessID", be.BusinessID);
            cmd.Parameters.AddWithValue("@CompanyName", be.CompanyName);
            cmd.Parameters.AddWithValue("@OperatingName", be.OperatingName);
            cmd.Parameters.AddWithValue("@BusinessNumber", be.BusinessNumber);
            cmd.Parameters.AddWithValue("@LegalEntityType", be.LegalEntityType);
            cmd.Parameters.AddWithValue("@BusinessRegistrationDate", be.BusinessRegistrationDate);
            cmd.Parameters.AddWithValue("@PrimaryContactName", be.PrimaryContactName);
            cmd.Parameters.AddWithValue("@PrimaryContactNumber", be.PrimaryContactNumber);
            cmd.Parameters.AddWithValue("@CareOf", be.CareOf);
            cmd.Parameters.AddWithValue("@StreetNumber", be.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", be.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", be.Apartment);
            cmd.Parameters.AddWithValue("@City", be.City);
            cmd.Parameters.AddWithValue("@Province", be.Province);
            cmd.Parameters.AddWithValue("@PostalCode", be.PostalCode);
            cmd.Parameters.AddWithValue("@BusinessAgreementDate", be.BusinessAgreementDate);
            cmd.Parameters.AddWithValue("@Status", be.Status);
            cmd.Parameters.AddWithValue("@MotherOrgId", 1);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateAddBusiness(" + be + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateAddBusiness" + ex.Message);
            throw ex;
        }
    }
    public void DeleteAddBusiness(AddBusinessEntityClass be)
    {
        try
        {
            loginfo = "DeleteAddBusiness(" + be + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_DeleteAddBusiness";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BusinessID", be.BusinessID);
            cmd.ExecuteNonQuery();

            loginfo = "DeleteAddBusiness(" + be + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_DeleteAddBusiness" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetBussList(int MotherOrgId)
    {
        try
        {
            loginfo = "GetBussList(" + MotherOrgId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "GetBussList";
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList(" + MotherOrgId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetBussList" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetBussList_BusinessOwner(int MotherOrgId, int BusinessOwner)
    {
        try
        {
            loginfo = "GetBussList_BusinessOwner(" + MotherOrgId + "," + BusinessOwner + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "GetBussList_BusinessOwner";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@BusinessOwner", BusinessOwner);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_BusinessOwner(" + MotherOrgId + "," + BusinessOwner + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetBussList_BusinessOwner" + ex.Message);
            throw ex;
        }

    }



    public DataTable GetBussList_BusinessOwnerForDashBord1(int MotherOrgId, int EmployeeID, int UserTypeId)
    {
        try
        {
            loginfo = "GetBussList_BusinessOwnerForDashBord1(" + MotherOrgId + "," + EmployeeID + "," + UserTypeId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_GetBusListForDashbord1";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_BusinessOwnerForDashBord1(" + MotherOrgId + "," + EmployeeID + "," + UserTypeId + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetBusListForDashbord" + ex.Message);
            throw ex;
        }

    }

    public DataTable GetBussList_BusinessOwnerForDashBord(int MotherOrgId, int EmployeeID, int UserTypeId)
    {
        try
        {
            loginfo = "GetBussList_BusinessOwnerForDashBord(" + MotherOrgId + "," + EmployeeID + "," + UserTypeId + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_GetBusListForDashbord";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_BusinessOwnerForDashBord(" + MotherOrgId + "," + EmployeeID + "," + UserTypeId + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetBusListForDashbord" + ex.Message);
            throw ex;
        }

    }



    public DataTable GetBussList_OwnerForDashBord(int MotherOrgId, int UserId, int UserTypeId, string Email)
    {
        try
        {
            loginfo = "GetBussList_OwnerForDashBord(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + Email + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_GetBusListForDashbordOwner";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@UserID", UserId);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_OwnerForDashBord(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + Email + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetBusListForDashbordOwner" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetBussList_ManagerForDashBord(int MotherOrgId, int EmployeeID, int UserTypeId, int UserId, string email)
    {
        try
        {
            loginfo = "GetBussList_OwnerForDashBord(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + email + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_GetBusListForDashbord_Manager";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.Parameters.AddWithValue("@userid", UserId);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_ManagerForDashBord(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + email + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetBusListForDashbord_Manager" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetBussList_ManagerForDashBord_Manager_Emp_to_Manager(int MotherOrgId, int EmployeeID, int UserTypeId, int UserId, string email)
    {
        try
        {
            loginfo = "GetBussList_ManagerForDashBord_Manager_Emp_to_Manager(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + email + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_GetBusListForDashbord_Manager_Emp_to_Manager";

            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.Parameters.AddWithValue("@userid", UserId);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetBussList_ManagerForDashBord_Manager_Emp_to_Manager(" + MotherOrgId + "," + UserId + "," + UserTypeId + ", " + email + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetBusListForDashbord_Manager_Emp_to_Manager" + ex.Message);
            throw ex;
        }

    }

    public void AssgineBusiness(int BusinessID, int EmployeeID, string BusinessName, string EmployeeName, int UserTypeId, int MotherOrgID)
    {
        try
        {
            loginfo = "AssgineBusiness(" + BusinessID + "," + EmployeeID + "," + BusinessName + ", " + EmployeeName + ", " + UserTypeId + ", " + MotherOrgID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_InsertAssginBussiness";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@BusinessName", BusinessName);
            cmd.Parameters.AddWithValue("@EmployeeName", EmployeeName);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();

            loginfo = "AssgineBusiness(" + BusinessID + "," + EmployeeID + "," + BusinessName + ", " + EmployeeName + ", " + UserTypeId + ", " + MotherOrgID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAssginBussiness" + ex.Message);
            throw ex;
        }
    }

    public void DeleteAddBusiness(int BusinessID)
    {
        try
        {
            loginfo = "DeleteAddBusiness(" + BusinessID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_DeleteAddBusiness";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.ExecuteNonQuery();

            loginfo = "DeleteAddBusiness(" + BusinessID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_DeleteAddBusiness" + ex.Message);
            throw ex;
        }
    }

    public int InsertBusinessDetails(AddBusinessEntityClass AddBusiEntity)
    {
        int BusinessId = 0;
        getconnection();
        try
        {
            loginfo = "InsertBusinessDetails(" + AddBusiEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdBusinessDetails = new SqlCommand();
            cmdBusinessDetails.CommandText = "Sp_InsertBusiness";
            cmdBusinessDetails.CommandType = CommandType.StoredProcedure;
            cmdBusinessDetails.Connection = con;

            cmdBusinessDetails.Parameters.AddWithValue("@CompanyName", AddBusiEntity.CompanyName);
            cmdBusinessDetails.Parameters.AddWithValue("@OperatingName", AddBusiEntity.OperatingName);
            cmdBusinessDetails.Parameters.AddWithValue("@BusinessNumber", AddBusiEntity.BusinessNumber);
            cmdBusinessDetails.Parameters.AddWithValue("@LegalEntityType", AddBusiEntity.LegalEntityType);
            cmdBusinessDetails.Parameters.AddWithValue("@BusinessRegistrationDate", AddBusiEntity.BusinessRegistrationDate);
            cmdBusinessDetails.Parameters.AddWithValue("@PrimaryContactName", AddBusiEntity.PrimaryContactName);
            cmdBusinessDetails.Parameters.AddWithValue("@PrimaryContactNumber", AddBusiEntity.PrimaryContactNumber);
            cmdBusinessDetails.Parameters.AddWithValue("@BusinessAgreementDate", AddBusiEntity.BusinessAgreementDate);
            cmdBusinessDetails.Parameters.AddWithValue("@Status", AddBusiEntity.Status);
            cmdBusinessDetails.Parameters.AddWithValue("@MotherOrgId", AddBusiEntity.MotherOrgId);
            cmdBusinessDetails.Parameters.AddWithValue("@Approved", AddBusiEntity.Approved);
            cmdBusinessDetails.Parameters.AddWithValue("@CareOf", AddBusiEntity.CareOf);


            cmdBusinessDetails.Parameters.Add("@BusinessId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdBusinessDetails.ExecuteNonQuery();
            BusinessId = int.Parse(cmdBusinessDetails.Parameters["@BusinessId"].Value.ToString());

            loginfo = "InsertBusinessDetails(" + AddBusiEntity + ") end";
            Logger.Info(loginfo);
            return BusinessId;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertBusiness" + ex.Message);
            throw ex;
        }
    }


    public int InsertProjectDetails(AddProject addProject)
    {
        int BusinessId = 0;
        getconnection();
        try
        {
            loginfo = "InsertProjectDetails(" + addProject + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdProjectDetails = new SqlCommand();
            cmdProjectDetails.CommandText = "Sp_InsertProject";
            cmdProjectDetails.CommandType = CommandType.StoredProcedure;
            cmdProjectDetails.Connection = con;

            cmdProjectDetails.Parameters.AddWithValue("@ProjectName", addProject.ProjectName);
            cmdProjectDetails.Parameters.AddWithValue("@StartDate", addProject.StartDate);
            cmdProjectDetails.Parameters.AddWithValue("@EndDate", addProject.EndDate);
            cmdProjectDetails.Parameters.AddWithValue("@BusinessId", addProject.BusinessID);
            cmdProjectDetails.Parameters.Add("@ProjectId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdProjectDetails.ExecuteNonQuery();
            BusinessId = int.Parse(cmdProjectDetails.Parameters["@ProjectId"].Value.ToString());

            loginfo = "InsertBusinessDetails(" + addProject + ") end";
            Logger.Info(loginfo);
            return BusinessId;
        }
        catch (Exception ex)
        {
            Logger.Error("InsertProjectDetails" + ex.Message);
            throw ex;
        }
    }


    public void UpdateProjectDetails(AddProject updateProject)
    {
        int BusinessId = 0;
        getconnection();
        try
        {
            loginfo = "UpdateProjectDetails(" + updateProject + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdProjectDetails = new SqlCommand();
            cmdProjectDetails.CommandText = "Sp_UpdateAddProject";
            cmdProjectDetails.CommandType = CommandType.StoredProcedure;
            cmdProjectDetails.Connection = con;

            cmdProjectDetails.Parameters.AddWithValue("@ProjectName", updateProject.ProjectName);
            cmdProjectDetails.Parameters.AddWithValue("@StartDate", updateProject.StartDate);
            //cmdProjectDetails.Parameters.AddWithValue("@StartDate", updateProject.StartDate);
            cmdProjectDetails.Parameters.AddWithValue("@EndDate", updateProject.EndDate);
            cmdProjectDetails.Parameters.AddWithValue("@BusinessId", updateProject.BusinessID);
            cmdProjectDetails.Parameters.AddWithValue("@ProjectId", updateProject.ProjectID);
            cmdProjectDetails.ExecuteNonQuery();
            //BusinessId = int.Parse(cmdProjectDetails.Parameters["@ProjectId"].Value.ToString());

            loginfo = "UpdateProjectDetails(" + updateProject + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("UpdateProjectDetails" + ex.Message);
            throw ex;
        }
    }
    public void InsertBusinessAddress(AddBusinessAddress BusinessAddressEntity, int BusinessID)
    {
        try
        {
            loginfo = "InsertBusinessAddress(" + BusinessAddressEntity + ", " + BusinessID + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand CmdBusinessAddress = new SqlCommand();
            CmdBusinessAddress.CommandText = "Sp_InsertBusinessAddress";
            CmdBusinessAddress.Connection = con;
            CmdBusinessAddress.CommandType = CommandType.StoredProcedure;
            CmdBusinessAddress.Parameters.AddWithValue("@BusinessID", BusinessID);
            CmdBusinessAddress.Parameters.AddWithValue("@Postalcode", BusinessAddressEntity.Postalcode);
            CmdBusinessAddress.Parameters.AddWithValue("@NumberSuffix", BusinessAddressEntity.NumberSuffix);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetName", BusinessAddressEntity.StreetName);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetType", BusinessAddressEntity.StreetType);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetDirection", BusinessAddressEntity.StreetDirection);
            CmdBusinessAddress.Parameters.AddWithValue("@SuitType", BusinessAddressEntity.SuitType);
            CmdBusinessAddress.Parameters.AddWithValue("@SuiteNumber", BusinessAddressEntity.SuiteNumber);
            CmdBusinessAddress.Parameters.AddWithValue("@SuitSuffix", BusinessAddressEntity.SuitSuffix);
            CmdBusinessAddress.Parameters.AddWithValue("@City", BusinessAddressEntity.City);
            CmdBusinessAddress.Parameters.AddWithValue("@Province", BusinessAddressEntity.Province);
            CmdBusinessAddress.Parameters.AddWithValue("@WorkPhone", BusinessAddressEntity.WorkPhone);
            CmdBusinessAddress.Parameters.AddWithValue("@MainPhone", BusinessAddressEntity.MainPhone);
            CmdBusinessAddress.Parameters.AddWithValue("@Mobile", BusinessAddressEntity.Mobile);
            CmdBusinessAddress.Parameters.AddWithValue("@Fax", BusinessAddressEntity.Fax);
            CmdBusinessAddress.Parameters.AddWithValue("@MainEmail", BusinessAddressEntity.MainEmail);
            CmdBusinessAddress.Parameters.AddWithValue("@CCEmail", BusinessAddressEntity.CCEmail);
            CmdBusinessAddress.Parameters.AddWithValue("@Website", BusinessAddressEntity.Website);
            CmdBusinessAddress.Parameters.AddWithValue("@Other1", BusinessAddressEntity.Other1);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetNo", BusinessAddressEntity.StreetNo);
            CmdBusinessAddress.Parameters.AddWithValue("@Cityid", BusinessAddressEntity.Cityid);
            CmdBusinessAddress.Parameters.AddWithValue("@Provinceid", BusinessAddressEntity.Provinceid);
            int amBefore = AnyminuteSupportAddress.MaxMasterId(con, null); // v80: see AnyminuteSupportAddress.AfterLegacyInsert
            CmdBusinessAddress.ExecuteNonQuery();
            AnyminuteSupportAddress.AfterLegacyInsert(con, null, amBefore, AnyminuteSupportAddress.FromLegacy(con, null, BusinessAddressEntity.StreetNo, BusinessAddressEntity.StreetName, BusinessAddressEntity.StreetType,
                BusinessAddressEntity.SuitType, BusinessAddressEntity.SuiteNumber, BusinessAddressEntity.City, BusinessAddressEntity.Cityid, BusinessAddressEntity.Province, BusinessAddressEntity.Provinceid, BusinessAddressEntity.Postalcode));
            loginfo = "InsertBusinessAddress(" + BusinessAddressEntity + ", " + BusinessID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertBusinessAddress" + ex.Message);
            throw ex;
        }
    }

    public void UpdateBusinessDetails(AddBusinessEntityClass AddBusiEntity)
    {
        try
        {
            loginfo = "UpdateBusinessDetails(" + AddBusiEntity + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand CmdBusinessUPDetails = new SqlCommand();
            CmdBusinessUPDetails.CommandText = "SP_UpdateBusinessDetails";
            CmdBusinessUPDetails.Connection = con;
            CmdBusinessUPDetails.CommandType = CommandType.StoredProcedure;

            CmdBusinessUPDetails.Parameters.AddWithValue("@CompanyName", AddBusiEntity.CompanyName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@OperatingName", AddBusiEntity.OperatingName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessNumber", AddBusiEntity.BusinessNumber);
            CmdBusinessUPDetails.Parameters.AddWithValue("@LegalEntityType", AddBusiEntity.LegalEntityType);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessRegistrationDate", AddBusiEntity.BusinessRegistrationDate);
            CmdBusinessUPDetails.Parameters.AddWithValue("@PrimaryContactName", AddBusiEntity.PrimaryContactName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@PrimaryContactNumber", AddBusiEntity.PrimaryContactNumber);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessAgreementDate", AddBusiEntity.BusinessAgreementDate);
            CmdBusinessUPDetails.Parameters.AddWithValue("@Status", AddBusiEntity.Status);
            CmdBusinessUPDetails.Parameters.AddWithValue("@MotherOrgID", AddBusiEntity.MotherOrgId);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessId", AddBusiEntity.BusinessID);
            CmdBusinessUPDetails.Parameters.AddWithValue("@CareOf", AddBusiEntity.CareOf);

            CmdBusinessUPDetails.ExecuteNonQuery();

            loginfo = "UpdateBusinessDetails(" + AddBusiEntity + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateBusinessDetails" + ex.Message);
            throw ex;
        }

    }

    public DataTable GetEmpListForDefaultFromManager(int ManagerID, int BusinessID, int MotherOrgID, string EmpType)
    {
        try
        {
            loginfo = "GetEmpListForDefaultFromManager(" + ManagerID + ", " + BusinessID + ", " + MotherOrgID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            DataTable dtEmpListForDefaultFromManager = new DataTable();
            SqlDataAdapter daEmpListForDefaultFromManager = new SqlDataAdapter();
            SqlCommand cmdEmpListForDefaultFromManager = new SqlCommand();
            cmdEmpListForDefaultFromManager.CommandText = "GetEmpListForDefaultFromManager";
            cmdEmpListForDefaultFromManager.Connection = con;
            cmdEmpListForDefaultFromManager.CommandType = CommandType.StoredProcedure;
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@EmpType", EmpType);
            daEmpListForDefaultFromManager.SelectCommand = cmdEmpListForDefaultFromManager;
            daEmpListForDefaultFromManager.Fill(dtEmpListForDefaultFromManager);

            loginfo = "GetEmpListForDefaultFromManager(" + ManagerID + ", " + BusinessID + ", " + MotherOrgID + ") end";
            Logger.Info(loginfo);

            return dtEmpListForDefaultFromManager;
        }
        catch (Exception ex)
        {
            Logger.Error("GetEmpListForDefaultFromManager" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetEmpListForDefaultFromManager_FOR_POPUP(int ManagerID, int BusinessID, int MotherOrgID)
    {
        try
        {
            loginfo = "GetEmpListForDefaultFromManager_FOR_POPUP(" + ManagerID + ", " + BusinessID + ", " + MotherOrgID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            DataTable dtEmpListForDefaultFromManager = new DataTable();
            SqlDataAdapter daEmpListForDefaultFromManager = new SqlDataAdapter();
            SqlCommand cmdEmpListForDefaultFromManager = new SqlCommand();
            cmdEmpListForDefaultFromManager.CommandText = "GetEmpListForDefaultFromManager_FOR_POPUP";
            cmdEmpListForDefaultFromManager.Connection = con;
            cmdEmpListForDefaultFromManager.CommandType = CommandType.StoredProcedure;
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdEmpListForDefaultFromManager.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            daEmpListForDefaultFromManager.SelectCommand = cmdEmpListForDefaultFromManager;
            daEmpListForDefaultFromManager.Fill(dtEmpListForDefaultFromManager);

            loginfo = "GetEmpListForDefaultFromManager_FOR_POPUP(" + ManagerID + ", " + BusinessID + ", " + MotherOrgID + ") end";
            Logger.Info(loginfo);

            return dtEmpListForDefaultFromManager;
        }
        catch (Exception ex)
        {
            Logger.Error("GetEmpListForDefaultFromManager_FOR_POPUP" + ex.Message);
            throw ex;
        }
    }
    public void UpdateBusinessAddress(AddBusinessAddress BusinessAddressEntity, int BusinessID)
    {
        try
        {
            loginfo = "UpdateBusinessAddress(" + BusinessAddressEntity + ", " + BusinessID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand CmdBusinessAddress = new SqlCommand();
            CmdBusinessAddress.CommandText = "Sp_UpdateBusinessAddress";
            CmdBusinessAddress.Connection = con;
            CmdBusinessAddress.CommandType = CommandType.StoredProcedure;
            CmdBusinessAddress.Parameters.AddWithValue("@BusinessID", BusinessID);
            CmdBusinessAddress.Parameters.AddWithValue("@Postalcode", BusinessAddressEntity.Postalcode);
            CmdBusinessAddress.Parameters.AddWithValue("@NumberSuffix", BusinessAddressEntity.NumberSuffix);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetName", BusinessAddressEntity.StreetName);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetType", BusinessAddressEntity.StreetType);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetDirection", BusinessAddressEntity.StreetDirection);
            CmdBusinessAddress.Parameters.AddWithValue("@SuitType", BusinessAddressEntity.SuitType);
            CmdBusinessAddress.Parameters.AddWithValue("@SuiteNumber", BusinessAddressEntity.SuiteNumber);
            CmdBusinessAddress.Parameters.AddWithValue("@SuitSuffix", BusinessAddressEntity.SuitSuffix);
            CmdBusinessAddress.Parameters.AddWithValue("@City", BusinessAddressEntity.City);
            CmdBusinessAddress.Parameters.AddWithValue("@Province", BusinessAddressEntity.Province);
            CmdBusinessAddress.Parameters.AddWithValue("@WorkPhone", BusinessAddressEntity.WorkPhone);
            CmdBusinessAddress.Parameters.AddWithValue("@MainPhone", BusinessAddressEntity.MainPhone);
            CmdBusinessAddress.Parameters.AddWithValue("@Mobile", BusinessAddressEntity.Mobile);
            CmdBusinessAddress.Parameters.AddWithValue("@Fax", BusinessAddressEntity.Fax);
            CmdBusinessAddress.Parameters.AddWithValue("@MainEmail", BusinessAddressEntity.MainEmail);
            CmdBusinessAddress.Parameters.AddWithValue("@CCEmail", BusinessAddressEntity.CCEmail);
            CmdBusinessAddress.Parameters.AddWithValue("@Website", BusinessAddressEntity.Website);
            CmdBusinessAddress.Parameters.AddWithValue("@Other1", BusinessAddressEntity.Other1);
            CmdBusinessAddress.Parameters.AddWithValue("@StreetNo", BusinessAddressEntity.StreetNo);

            CmdBusinessAddress.ExecuteNonQuery();

            loginfo = "UpdateBusinessAddress(" + BusinessAddressEntity + ", " + BusinessID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateBusinessAddress" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetOwnerDetails(string email, int MotherOrgId, string EmpType)
    {
        try
        {
            loginfo = "GetOwnerDetails(" + email + ", " + MotherOrgId + ") Start";
            Logger.Info(loginfo);
            getconnection();
            DataTable dtOwnerDetails = new DataTable();
            SqlCommand CmdOwnerDetails = new SqlCommand();
            SqlDataAdapter daCmdOwnerDetails = new SqlDataAdapter();
            CmdOwnerDetails.CommandText = "sp_GetOwnerDetails";
            CmdOwnerDetails.Connection = con;
            CmdOwnerDetails.CommandType = CommandType.StoredProcedure;
            CmdOwnerDetails.Parameters.AddWithValue("@email", email);
            CmdOwnerDetails.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            CmdOwnerDetails.Parameters.AddWithValue("@EmpType", EmpType);
            daCmdOwnerDetails.SelectCommand = CmdOwnerDetails;
            daCmdOwnerDetails.Fill(dtOwnerDetails);
            return dtOwnerDetails;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetSelectBuss(int EmpId)
    {
        try
        {
            loginfo = "Sp_GetSelectBuss(" + EmpId + ") Start";
            Logger.Info(loginfo);
            getconnection();
            DataTable dtOwnerDetails = new DataTable();
            SqlCommand CmdOwnerDetails = new SqlCommand();
            SqlDataAdapter daCmdOwnerDetails = new SqlDataAdapter();
            CmdOwnerDetails.CommandText = "Sp_GetSelectBuss";
            CmdOwnerDetails.Connection = con;
            CmdOwnerDetails.CommandType = CommandType.StoredProcedure;
            CmdOwnerDetails.Parameters.AddWithValue("@EmployeeID", EmpId);

            daCmdOwnerDetails.SelectCommand = CmdOwnerDetails;
            daCmdOwnerDetails.Fill(dtOwnerDetails);
            return dtOwnerDetails;

        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    public DataTable GetBussForSelectBUSS(int EmpId, int UserTypeid)
    {
        try
        {
            loginfo = "GetBussForSelectBUSS(" + EmpId + ") Start";
            Logger.Info(loginfo);
            getconnection();
            DataTable dtOwnerDetails = new DataTable();
            SqlCommand CmdOwnerDetails = new SqlCommand();
            SqlDataAdapter daCmdOwnerDetails = new SqlDataAdapter();
            CmdOwnerDetails.CommandText = "GetBussForSelectBUSS";
            CmdOwnerDetails.Connection = con;
            CmdOwnerDetails.CommandType = CommandType.StoredProcedure;
            CmdOwnerDetails.Parameters.AddWithValue("@EMPID", EmpId);
            CmdOwnerDetails.Parameters.AddWithValue("@usertpeid", UserTypeid);
            daCmdOwnerDetails.SelectCommand = CmdOwnerDetails;
            daCmdOwnerDetails.Fill(dtOwnerDetails);
            return dtOwnerDetails;

        }
        catch (Exception ex)
        {
            throw ex;
        }


    }

    public void UpdateBusinessDetailsWithoutImage(AddBusinessEntityClass AddBusiEntity)
    {
        try
        {
            loginfo = "UpdateBusinessDetailsWithoutImage(" + AddBusiEntity + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand CmdBusinessUPDetails = new SqlCommand();
            CmdBusinessUPDetails.CommandText = "SP_UpdateBusinessDetailsWithoutImage";
            CmdBusinessUPDetails.Connection = con;
            CmdBusinessUPDetails.CommandType = CommandType.StoredProcedure;

            CmdBusinessUPDetails.Parameters.AddWithValue("@CompanyName", AddBusiEntity.CompanyName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@OperatingName", AddBusiEntity.OperatingName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessNumber", AddBusiEntity.BusinessNumber);
            CmdBusinessUPDetails.Parameters.AddWithValue("@LegalEntityType", AddBusiEntity.LegalEntityType);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessRegistrationDate", AddBusiEntity.BusinessRegistrationDate);
            CmdBusinessUPDetails.Parameters.AddWithValue("@PrimaryContactName", AddBusiEntity.PrimaryContactName);
            CmdBusinessUPDetails.Parameters.AddWithValue("@PrimaryContactNumber", AddBusiEntity.PrimaryContactNumber);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessAgreementDate", AddBusiEntity.BusinessAgreementDate);
            CmdBusinessUPDetails.Parameters.AddWithValue("@Status", AddBusiEntity.Status);
            CmdBusinessUPDetails.Parameters.AddWithValue("@MotherOrgID", AddBusiEntity.MotherOrgId);
            CmdBusinessUPDetails.Parameters.AddWithValue("@BusinessId", AddBusiEntity.BusinessID);

            CmdBusinessUPDetails.ExecuteNonQuery();

            loginfo = "UpdateBusinessDetailsWithoutImage(" + AddBusiEntity + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateBusinessDetails" + ex.Message);
            throw ex;
        }

    }
    public DataTable Sp_GetSubsBuss(string email, int Bownerid, string UserTypeid)
    {
        getconnection();
        try
        {
            loginfo = "Sp_GetSubsBuss(" + email + ", " + Bownerid + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_GetSubsBuss", con);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@EmployeeID", Bownerid);
            cmd.Parameters.AddWithValue("@UsertypeId", UserTypeid);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            return dt;

            loginfo = "Sp_GetSubsBuss(" + email + ", " + Bownerid + ") rnd";
            Logger.Info(loginfo);

        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetSubsBuss" + ex.Message);
            throw ex;
        }
    }
}