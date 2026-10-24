using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for SupportAdminCtrl
/// </summary>
public class SupportAdminCtrl
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public SupportAdminCtrl()
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

    public int Sp_InsertSupport(SupportAdminEntity SupEntity)
    {
        try
        {
            loginfo = "Sp_InsertSupport(" + SupEntity + ") Start";
            Logger.Info(loginfo);

            getconnection();
            int Result = 0;
            SqlCommand cmd = new SqlCommand("Sp_InsertSupport");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@Title", SupEntity.Title);
            cmd.Parameters.AddWithValue("@FirstName", SupEntity.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", SupEntity.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", SupEntity.LastName);
            cmd.Parameters.AddWithValue("@DOB", SupEntity.DOB);
            cmd.Parameters.AddWithValue("@Gender", SupEntity.Gender);
            cmd.Parameters.AddWithValue("@Contact", SupEntity.Contact);
            cmd.Parameters.AddWithValue("@Mobile", SupEntity.Mobile);
            cmd.Parameters.AddWithValue("@CITY", SupEntity.CITY);
            cmd.Parameters.AddWithValue("@Province", SupEntity.Province);
            cmd.Parameters.AddWithValue("@Email", SupEntity.Email);
            cmd.Parameters.AddWithValue("@Status", SupEntity.Status);

            cmd.Parameters.Add("@SupportAdminId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            Result = int.Parse(cmd.Parameters["@SupportAdminId"].Value.ToString());

            loginfo = "Sp_InsertSupport(" + SupEntity + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertSupport" + ex.Message);
            throw ex;
        }
    }
    public int Sp_UpdateSupport(SupportAdminEntity SupEntity)
    {
        try
        {
            loginfo = "Sp_UpdateSupport(" + SupEntity + ") Start";
            Logger.Info(loginfo);

            getconnection();
            int Result = 0;
            SqlCommand cmd = new SqlCommand("Sp_UpdateSupport");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@SupportAdminId", SupEntity.SupportAdminId);
            cmd.Parameters.AddWithValue("@Title", SupEntity.Title);
            cmd.Parameters.AddWithValue("@FirstName", SupEntity.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", SupEntity.MiddleName);
            cmd.Parameters.AddWithValue("@LastName", SupEntity.LastName);
            cmd.Parameters.AddWithValue("@DOB", SupEntity.DOB);
            cmd.Parameters.AddWithValue("@Gender", SupEntity.Gender);
            cmd.Parameters.AddWithValue("@Contact", SupEntity.Contact);
            cmd.Parameters.AddWithValue("@Mobile", SupEntity.Mobile);
            cmd.Parameters.AddWithValue("@CITY", SupEntity.CITY);
            cmd.Parameters.AddWithValue("@Province", SupEntity.Province);
            cmd.Parameters.AddWithValue("@Email", SupEntity.Email);

            Result = cmd.ExecuteNonQuery();

            loginfo = "Sp_UpdateSupport(" + SupEntity + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateSupport" + ex.Message);
            throw ex;
        }
    }


    public int Sp_SP_InsertUserAccSupportAdmin(UserAccEntity UaEntity)
    {
        try
        {
            loginfo = "Sp_SP_InsertUserAccSupportAdmin(" + UaEntity + ") Start";
            Logger.Info(loginfo);

            getconnection();
            int Result = 0;
            SqlCommand cmd = new SqlCommand("SP_InsertUserAccSupportAdmin");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@EmpID", UaEntity.EmployeeId);
            cmd.Parameters.AddWithValue("@UserType", UaEntity.UserType);
            cmd.Parameters.AddWithValue("@MainEmail", UaEntity.Email);
            cmd.Parameters.AddWithValue("@UserTypeid", UaEntity.UserTypeid);
            Result = cmd.ExecuteNonQuery();

            loginfo = "Sp_SP_InsertUserAccSupportAdmin(" + UaEntity + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_InsertUserAccSupportAdmin" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetEmplist_SuppAdmin(string type, int empid, int select)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("GetEmplist_SuppAdmin", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Type", type);
            cmd.Parameters.AddWithValue("@empid", empid);
            cmd.Parameters.AddWithValue("@selectempid", select);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}