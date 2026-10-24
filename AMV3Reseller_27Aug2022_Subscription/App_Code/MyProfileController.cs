using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for MyProfileController
/// </summary>
public class MyProfileController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

    public MyProfileController()
    {
        //
        // TODO: Add constructor logic here
        //
    }
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
    public DataTable GetProfileBusiness(int userid, string Email)
    {
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetProfileBusiness(" + userid + "," + Email + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ProfileBusinessList", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserID", userid);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetProfileBusiness(" + userid + "," + Email + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ProfileBusinessList" + ex.Message);
            throw ex;
        }
    }

    public int UpdateBusinessOwnerProfile(AddEmployeeEntityClass AddempEntity, EmpAddressEntity EmpAddress)
    {
        try
        {
            loginfo = "UpdateBusinessOwnerProfile(" + AddempEntity + "," + EmpAddress + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();
            SqlCommand cmd = new SqlCommand("UpdateBusinessOwnerProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Title", AddempEntity.Title);
            cmd.Parameters.AddWithValue("@EmployeeID", AddempEntity.EmployeeID);
            cmd.Parameters.AddWithValue("@FirstName", AddempEntity.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", AddempEntity.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", AddempEntity.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", AddempEntity.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", AddempEntity.Gender);
            cmd.Parameters.AddWithValue("@MainPhone", EmpAddress.MainPhone);
            cmd.Parameters.AddWithValue("@Mobile", EmpAddress.Mobile);
            cmd.Parameters.AddWithValue("@City", EmpAddress.City);
            cmd.Parameters.AddWithValue("@Province", EmpAddress.Province);
            cmd.Parameters.AddWithValue("@MainEmail", EmpAddress.MainEmail);
            cmd.Parameters.AddWithValue("@CCEmail", EmpAddress.CCEmail);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateBusinessOwnerProfile(" + AddempEntity + "," + EmpAddress + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("UpdateBusinessOwnerProfile" + ex.Message);
            throw ex;
        }
    }

    public int UpdateEmployeeProfile(AddEmployeeEntityClass AddempEntity, EmpAddressEntity EmpAddress)
    {
        try
        {
            loginfo = "UpdateEmployeeProfile(" + AddempEntity + "," + EmpAddress + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_UpdateEmployeeProfile", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Title", AddempEntity.Title);
            cmd.Parameters.AddWithValue("@EmployeeID", AddempEntity.EmployeeID);
            cmd.Parameters.AddWithValue("@FirstName", AddempEntity.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", AddempEntity.MiddleName);
            cmd.Parameters.AddWithValue("@Occupation", AddempEntity.Occupation);
            cmd.Parameters.AddWithValue("@LastName", AddempEntity.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", AddempEntity.DateOfBirth);

            cmd.Parameters.AddWithValue("@SocialInsuranceNumber", AddempEntity.SocialInsuranceNumber);
            cmd.Parameters.AddWithValue("@SINExpiry", AddempEntity.SINExpiry);
            cmd.Parameters.AddWithValue("@Gender", AddempEntity.Gender);
            cmd.Parameters.AddWithValue("@MainPhone", EmpAddress.MainPhone);
            cmd.Parameters.AddWithValue("@Mobile", EmpAddress.Mobile);
            cmd.Parameters.AddWithValue("@City", EmpAddress.City);
            cmd.Parameters.AddWithValue("@Province", EmpAddress.Province);
            cmd.Parameters.AddWithValue("@MainEmail", EmpAddress.MainEmail);
            cmd.Parameters.AddWithValue("@CCEmail", EmpAddress.CCEmail);

            cmd.Parameters.AddWithValue("@StreetName", EmpAddress.StreetName);
            cmd.Parameters.AddWithValue("@StreetNo", EmpAddress.StreetNo);
            cmd.Parameters.AddWithValue("@StreetType", EmpAddress.StreetType);
            cmd.Parameters.AddWithValue("@SuiteNumber", EmpAddress.SuiteNumber);
            cmd.Parameters.AddWithValue("@SuitType", EmpAddress.SuitType);
            cmd.Parameters.AddWithValue("@Postalcode", EmpAddress.Postalcode);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateEmployeeProfile(" + AddempEntity + "," + EmpAddress + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateEmployeeProfile" + ex.Message);
            throw ex;
        }
    }
}