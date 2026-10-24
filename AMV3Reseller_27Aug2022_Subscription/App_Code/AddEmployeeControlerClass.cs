using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddEmployeeControlerClass
/// </summary>
public class AddEmployeeControlerClass
{
    AddEmployeeEntityClass EmpEc = new AddEmployeeEntityClass();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public AddEmployeeControlerClass()
    {
        //
        // TODO: Add constructor logic here
        //
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
    public void InsertAddEmployee(AddEmployeeEntityClass EmpEc)
    {
        try
        {
            loginfo = "InsertAddEmployee(" + EmpEc + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_InsertAddEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmd.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmd.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmd.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmd.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmd.Parameters.AddWithValue("@HomePhone", EmpEc.HomePhone);
            cmd.Parameters.AddWithValue("@EMail_primary", EmpEc.EMail_primary);
            cmd.Parameters.AddWithValue("@EMail_secondary", EmpEc.EMail_secondary);
            cmd.Parameters.AddWithValue("@StreetNumber", EmpEc.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", EmpEc.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);
            cmd.Parameters.AddWithValue("@City", EmpEc.City);
            cmd.Parameters.AddWithValue("@Province", EmpEc.Province);
            cmd.Parameters.AddWithValue("@PostalCode", EmpEc.PostalCode);
            cmd.Parameters.AddWithValue("@JobTitle", EmpEc.JobTitle);
            cmd.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmd.Parameters.AddWithValue("@BillRate", EmpEc.BillRate);
            cmd.Parameters.AddWithValue("@BillType", EmpEc.BillType);
            cmd.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmd.Parameters.AddWithValue("@IsEmployeeEIExempt", EmpEc.IsEmployeeCPPExempt);
            cmd.Parameters.AddWithValue("@IsEmployeeCPPExempt", EmpEc.IsEmployeeCPPExempt);
            cmd.Parameters.AddWithValue("@RetainVacation", EmpEc.RetainVacation);
            cmd.Parameters.AddWithValue("@MotherOrgID", EmpEc.MotherOrgID);
            cmd.Parameters.AddWithValue("@BusinessID", EmpEc.BusinessID);
            cmd.Parameters.AddWithValue("@MangerId", EmpEc.MangerId);
            cmd.Parameters.AddWithValue("@BusinessOwnerId", EmpEc.BusinessOwnerId);

            cmd.ExecuteNonQuery();

            loginfo = "InsertAddEmployee(" + EmpEc + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAddEmployee" + ex.Message);
            throw ex;
        }
    }

    public void UpdateAddEmployee(AddEmployeeEntityClass EmpEc)
    {
        try
        {
            loginfo = "UpdateAddEmployee(" + EmpEc + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_UpdateAddEmployee", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", EmpEc.EmployeeID);
            cmd.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmd.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmd.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmd.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmd.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmd.Parameters.AddWithValue("@HomePhone", EmpEc.HomePhone);
            cmd.Parameters.AddWithValue("@EMail_primary", EmpEc.EMail_primary);
            cmd.Parameters.AddWithValue("@EMail_secondary", EmpEc.EMail_secondary);
            cmd.Parameters.AddWithValue("@StreetNumber", EmpEc.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", EmpEc.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);
            cmd.Parameters.AddWithValue("@City", EmpEc.City);
            cmd.Parameters.AddWithValue("@Province", EmpEc.Province);
            cmd.Parameters.AddWithValue("@PostalCode", EmpEc.PostalCode);
            cmd.Parameters.AddWithValue("@JobTitle", EmpEc.JobTitle);
            cmd.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmd.Parameters.AddWithValue("@BillRate", EmpEc.BillRate);
            cmd.Parameters.AddWithValue("@BillType", EmpEc.BillType);
            cmd.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmd.Parameters.AddWithValue("@IsEmployeeEIExempt", EmpEc.IsEmployeeCPPExempt);
            cmd.Parameters.AddWithValue("@IsEmployeeCPPExempt", EmpEc.IsEmployeeCPPExempt);
            cmd.Parameters.AddWithValue("@RetainVacation", EmpEc.RetainVacation);

            cmd.Parameters.AddWithValue("@BusinessID", EmpEc.BusinessID);
            cmd.Parameters.AddWithValue("@MangerId", EmpEc.MangerId);
            cmd.Parameters.AddWithValue("@BusinessOwnerId", EmpEc.BusinessOwnerId);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateAddEmployee(" + EmpEc + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateAddEmployee" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetEmpList(int MotherOrgID, int BusinessID)
    {
        try
        {
            loginfo = "GetEmpList(" + EmpEc + ", " + BusinessID + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("GetEmplist", con);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetEmpList(" + EmpEc + ", " + BusinessID + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetEmplist" + ex.Message);
            throw ex;
        }

    }

    public DataTable GetEmplist_forManager(int MotherOrgID, int ManagerID)
    {
        try
        {
            loginfo = "GetEmplist_forManager(" + MotherOrgID + ", " + ManagerID + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("GetEmplist_forManager", con);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetEmplist_forManager(" + MotherOrgID + ", " + ManagerID + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetEmplist_forManager" + ex.Message);
            throw ex;
        }

    }
    public void DeleteEmployee(int EmployeeID)
    {
        try
        {
            loginfo = "DeleteEmployee(" + EmployeeID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_DeleteAddEmployee";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.ExecuteNonQuery();

            loginfo = "DeleteEmployee(" + EmployeeID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_DeleteAddEmployee" + ex.Message);
            throw ex;
        }
    }
    public void UpdateProfile(AddEmployeeEntityClass EmpEc)
    {
        try
        {
            loginfo = "UpdateProfile(" + EmpEc + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_UpdateProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeId", EmpEc.EmployeeID);
            cmd.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmd.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmd.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmd.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmd.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmd.Parameters.AddWithValue("@HomePhone", EmpEc.HomePhone);
            cmd.Parameters.AddWithValue("@EMail_primary", EmpEc.EMail_primary);
            cmd.Parameters.AddWithValue("@EMail_secondary", EmpEc.EMail_secondary);
            cmd.Parameters.AddWithValue("@StreetNumber", EmpEc.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", EmpEc.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);
            cmd.Parameters.AddWithValue("@City", EmpEc.City);
            cmd.Parameters.AddWithValue("@Province", EmpEc.Province);
            cmd.Parameters.AddWithValue("@PostalCode", EmpEc.PostalCode);
            cmd.Parameters.AddWithValue("@JobTitle", EmpEc.JobTitle);

            cmd.ExecuteNonQuery();

            loginfo = "UpdateProfile(" + EmpEc + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateProfile" + ex.Message);
            throw ex;
        }

    }

    public int InsertPersonalDetails(AddEmployeeEntityClass EmpEc, string userTypeUserACC, RateDetailsEntity RateEntity)
    {
        int Empid = 0;
        getconnection();
        try
        {
            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlTransaction Tran = null;
            // Tran = con.BeginTransaction("PersonalDetails");

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "Sp_InsertEmpPersonalDetails";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;
            //cmdPersonalDetails.Transaction = Tran;

            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmdPersonalDetails.Parameters.AddWithValue("@userTypeUserACC", userTypeUserACC);
            cmdPersonalDetails.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmdPersonalDetails.Parameters.AddWithValue("@MotherOrgID", EmpEc.MotherOrgID);
            cmdPersonalDetails.Parameters.AddWithValue("@IsActive", EmpEc.IsActive);
            cmdPersonalDetails.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmdPersonalDetails.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);//Inactivedate
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", "NO");
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", "NO");
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", "NO");

            //if datetime null then "1/1/0001 12:00:00 AM" giving error in bd

            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);

            cmdPersonalDetails.Parameters.Add("@EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;
            //Tran.Save("SavePersonalDetails");
            cmdPersonalDetails.ExecuteNonQuery();
            Empid = int.Parse(cmdPersonalDetails.Parameters["@EmployeeID"].Value.ToString());

            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") end";
            Logger.Info(loginfo);

            return Empid;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertEmpPersonalDetails" + ex.Message);
            throw ex;
        }
    }

    public int UpdatePersonalDetails(AddEmployeeEntityClass EmpEc, RateDetailsEntity RateEntity)
    {

        getconnection();
        int Result = 0;
        try
        {
            loginfo = "UpdatePersonalDetails(" + EmpEc + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "SP_UpdateEmployeeDetails";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);


            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);

            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);

            cmdPersonalDetails.Parameters.AddWithValue("@City", EmpEc.City);
            cmdPersonalDetails.Parameters.AddWithValue("@Province", EmpEc.Province);
            cmdPersonalDetails.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);

            cmdPersonalDetails.Parameters.AddWithValue("@EmployeeID", EmpEc.EmployeeID);
            Result = cmdPersonalDetails.ExecuteNonQuery();

            loginfo = "UpdatePersonalDetails(" + EmpEc + ", " + RateEntity + ") end";
            Logger.Info(loginfo);
            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateEmployeeDetails" + ex.Message);
            throw ex;
        }
    }

    public int InsertPartnerPersonalDetails(AddEmployeeEntityClass EmpEc, string userTypeUserACC, RateDetailsEntity RateEntity)
    {
        getconnection();
        try
        {
            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "Sp_InsertEmpPersonalDetails_Partner";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@EMail_primary", EmpEc.EMail_primary);
            cmdPersonalDetails.Parameters.AddWithValue("@EMail_secondary", EmpEc.EMail_secondary);
            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);
            cmdPersonalDetails.Parameters.AddWithValue("@userTypeUserACC", userTypeUserACC);
            cmdPersonalDetails.Parameters.AddWithValue("@Title", EmpEc.Title);
            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);
            cmdPersonalDetails.Parameters.AddWithValue("@MotherOrgID", EmpEc.MotherOrgID);
            cmdPersonalDetails.Parameters.AddWithValue("@IsActive", EmpEc.IsActive);
            cmdPersonalDetails.Parameters.AddWithValue("@MobilePhone", EmpEc.MobilePhone);
            cmdPersonalDetails.Parameters.AddWithValue("@Apartment", EmpEc.Apartment);//Inactivedate
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);
            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);
            cmdPersonalDetails.Parameters.AddWithValue("@StreetNumber", EmpEc.StreetNumber);
            cmdPersonalDetails.Parameters.Add("@EmployeeID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmdPersonalDetails.ExecuteNonQuery();
            int Empid = int.Parse(cmdPersonalDetails.Parameters["@EmployeeID"].Value.ToString());

            loginfo = "InsertPersonalDetails(" + EmpEc + ", " + userTypeUserACC + ", " + RateEntity + ") end";
            Logger.Info(loginfo);

            return Empid;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertEmpPersonalDetails_Partner" + ex.Message);
            throw ex;
        }
    }


    public int UpdatePersonalDetails_WithSIN(AddEmployeeEntityClass EmpEc, RateDetailsEntity RateEntity)
    {

        getconnection();
        int Result = 0;
        try
        {
            loginfo = "UpdatePersonalDetails_WithSIN(" + EmpEc + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "SP_UpdateEmployeeDetails_WithSIN";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);


            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            //cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);

            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);

            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);

            cmdPersonalDetails.Parameters.AddWithValue("@EmployeeID", EmpEc.EmployeeID);
            Result = cmdPersonalDetails.ExecuteNonQuery();

            loginfo = "UpdatePersonalDetails_WithSIN(" + EmpEc + ", " + RateEntity + ") end";
            Logger.Info(loginfo);
            return Result;

        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateEmployeeDetails_WithSIN" + ex.Message);
            throw ex;
        }
    }

    public int UpdatePersonalDetails_WithDOB(AddEmployeeEntityClass EmpEc, RateDetailsEntity RateEntity)
    {

        getconnection();
        int Result = 0;
        try
        {
            loginfo = "UpdatePersonalDetails_WithDOB(" + EmpEc + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "SP_UpdateEmployeeDetails_WithDOB";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);


            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            //cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);

            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);

            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);

            cmdPersonalDetails.Parameters.AddWithValue("@EmployeeID", EmpEc.EmployeeID);
            Result = cmdPersonalDetails.ExecuteNonQuery();

            loginfo = "UpdatePersonalDetails_WithDOB(" + EmpEc + ", " + RateEntity + ") end";
            Logger.Info(loginfo);
            return Result;

        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateEmployeeDetails_WithDOB" + ex.Message);
            throw ex;
        }
    }

    public int UpdatePersonalDetails_WithoutSINDOD(AddEmployeeEntityClass EmpEc, RateDetailsEntity RateEntity)
    {

        getconnection();
        int Result = 0;
        try
        {
            loginfo = "UpdatePersonalDetails_WithoutSINDOD(" + EmpEc + ", " + RateEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "SP_UpdateEmployeeDetails_WithoutSINDOD";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@UserType", EmpEc.UserType);


            cmdPersonalDetails.Parameters.AddWithValue("@Occupation", EmpEc.Occupation);
            cmdPersonalDetails.Parameters.AddWithValue("@LegalName", EmpEc.LegalName);
            cmdPersonalDetails.Parameters.AddWithValue("@FirstName", EmpEc.FirstName);
            cmdPersonalDetails.Parameters.AddWithValue("@LastName", EmpEc.LastName);
            //cmdPersonalDetails.Parameters.AddWithValue("@SocialInsuranceNumber", EmpEc.SocialInsuranceNumber);
            //cmdPersonalDetails.Parameters.AddWithValue("@DateOfBirth", EmpEc.DateOfBirth);
            cmdPersonalDetails.Parameters.AddWithValue("@Gender", EmpEc.Gender);
            cmdPersonalDetails.Parameters.AddWithValue("@HireDate", EmpEc.HireDate);
            cmdPersonalDetails.Parameters.AddWithValue("@TerminationDate", EmpEc.TerminationDate);
            cmdPersonalDetails.Parameters.AddWithValue("@MiddleName", EmpEc.MiddleName);

            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeEIExempt", RateEntity.IsEmployeeEIExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@IsEmployeeCPPExempt", RateEntity.IsEmployeeCPPExempty);
            cmdPersonalDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);

            cmdPersonalDetails.Parameters.AddWithValue("@SINExpiry", EmpEc.SINExpiry);

            cmdPersonalDetails.Parameters.AddWithValue("@EmployeeID", EmpEc.EmployeeID);
            Result = cmdPersonalDetails.ExecuteNonQuery();

            loginfo = "UpdatePersonalDetails_WithoutSINDOD(" + EmpEc + ", " + RateEntity + ") end";
            Logger.Info(loginfo);
            return Result;

        }
        catch (Exception ex)
        {
            Logger.Error("SP_UpdateEmployeeDetails_WithoutSINDOD" + ex.Message);
            throw ex;
        }
    }

    public void DeleteAllDetails(int empid, int BussId)
    {
        try
        {
            getconnection();
            loginfo = "DeleteAllDetails(" + empid + ", " + BussId + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdPersonalDetails = new SqlCommand();
            cmdPersonalDetails.CommandText = "DeleteAllDetails";
            cmdPersonalDetails.CommandType = CommandType.StoredProcedure;
            cmdPersonalDetails.Connection = con;

            cmdPersonalDetails.Parameters.AddWithValue("@EmployeeId", empid);


            cmdPersonalDetails.Parameters.AddWithValue("@BussId", BussId);
            cmdPersonalDetails.ExecuteNonQuery();

            loginfo = "DeleteAllDetails(" + empid + ", " + BussId + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("DeleteAllDetails" + ex.Message);
            throw ex;
        }
    }
    public DataTable Sp_GetSubsPerDetails(string email, int Bownerid)
    {
        getconnection();
        try
        {
            loginfo = "Sp_GetSubsPerDetails(" + email + ", " + Bownerid + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_GetSubsPerDetails", con);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@EmployeeID", Bownerid);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            return dt;

            loginfo = "Sp_GetSubsPerDetails(" + email + ", " + Bownerid + ") rnd";
            Logger.Info(loginfo);

        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetSubsPerDetails" + ex.Message);
            throw ex;
        }
    }
}