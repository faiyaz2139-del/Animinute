using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for UserAccController
/// </summary>
public class UserAccController
{

    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();

    SqlCommand cmd = new SqlCommand();
    DataSet ds = new DataSet();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    int i;
    public UserAccController()
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

    public void InsertUserAcc(UserAccEntity Ue)
    {
        try
        {
            loginfo = "InsertUserAcc(" + Ue + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_InsertUserAcc";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@EmployeeId", Ue.EmployeeId);
            cmd.Parameters.AddWithValue("@Email", Ue.Email);
            cmd.Parameters.AddWithValue("@UserTypeid", Ue.UserTypeid);
            cmd.Parameters.AddWithValue("@UserType", Ue.UserType);
            cmd.Parameters.AddWithValue("@Status", Ue.Status);
            cmd.Parameters.AddWithValue("@MotherOrgid", Ue.MotherOrgid);

            cmd.ExecuteNonQuery();

            loginfo = "InsertUserAcc(" + Ue + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertUserAcc" + ex.Message);
            throw ex;
        }
    }
    public void ResetPassword(int UserId)
    {
        try
        {
            loginfo = "ResetPassword(" + UserId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "ResetPassword";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@UserId", UserId);
            cmd.ExecuteNonQuery();

            loginfo = "ResetPassword(" + UserId + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("ResetPassword" + ex.Message);
            throw ex;
        }
    }
    public string Sp_InsertUserAccFromAddress(UserAccEntity Ue)
    {
        try
        {
            loginfo = "Sp_InsertUserAccFromAddress(" + Ue + ") Start";
            Logger.Info(loginfo);

            string UserId = "";
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_InsertUserAccFromAddress";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@EmpID", Ue.EmployeeId);
            cmd.Parameters.AddWithValue("@MainEmail", Ue.Email);
            cmd.Parameters.AddWithValue("@MotherOrgId", Ue.MotherOrgid);
            cmd.Parameters.AddWithValue("@UserTypeid", Ue.UserTypeid);
            cmd.Parameters.AddWithValue("@UserType", Ue.UserType);
            cmd.Parameters.AddWithValue("@ProfileImage", Ue.ProfileImageName);
            cmd.Parameters.Add("@UserId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            UserId = cmd.Parameters["@UserId"].Value.ToString();

            loginfo = "Sp_InsertUserAccFromAddress(" + Ue + ") end";
            Logger.Info(loginfo);

            return UserId;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertUserAccFromAddress" + ex.Message);
            throw ex;
        }
    }

    public void Sp_InsertMapping(UserAccEntity Ue)
    {
        try
        {
            loginfo = "Sp_InsertMapping(" + Ue + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_InsertMapping";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@USERID", Ue.UserId);
            cmd.Parameters.AddWithValue("@UserTypeId", Ue.UserTypeid);
            cmd.Parameters.AddWithValue("@EmployeeID", Ue.EmployeeId);
            cmd.Parameters.AddWithValue("@BusinessID", Ue.BusinessID);

            cmd.ExecuteNonQuery();

            loginfo = "Sp_InsertMapping(" + Ue + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertMapping" + ex.Message);
            throw ex;
        }
    }
    public void Sp_ReSetPassword(int empid, string Password, string LoginType)
    {
        try
        {
            loginfo = "Sp_ReSetPassword(" + empid + "," + Password + "," + LoginType + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Updatepass";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@empid", empid);
            cmd.Parameters.AddWithValue("@pass", Password);
            cmd.Parameters.AddWithValue("@LoginType", LoginType);
            cmd.ExecuteNonQuery();

            loginfo = "Sp_ReSetPassword(" + empid + "," + Password + "," + LoginType + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Updatepass" + ex.Message);
            throw ex;
        }
    }
}