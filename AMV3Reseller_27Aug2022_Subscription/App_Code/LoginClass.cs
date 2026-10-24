using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for LoginClass
/// </summary>
public class LoginClass
{
    public LoginClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private SqlConnection con;
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

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
    public DataTable GetLoginDetails(string email, string password, string UID)
    {
        try
        {
            loginfo = "GetLoginDetails(" + email + "," + password + "," + UID + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("SP_LOGIN", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);
            cmd.Parameters.AddWithValue("@UID", UID);
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetLoginDetails(" + email + "," + password + "," + UID + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_LOGIN" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetDetilsForAcc(String email)
    {
        try
        {
            loginfo = "GetDetilsForAcc(" + email + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            DataTable dt = new DataTable();
            SqlCommand cmd = new SqlCommand("GetDetilsForAcc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Email", email);
            //cmd.Parameters.AddWithValue("@Password", password);
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "GetDetilsForAcc(" + email + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetDetilsForAcc" + ex.Message);
            throw ex;
        }

    }
    public string Get_UserIDComaSeprated(String email)
    {
        try
        {
            loginfo = "Get_UserIDComaSeprated(" + email + ") Start";
            Logger.Info(loginfo);
            getconnection();
            string UserId = string.Empty;
            SqlDataAdapter da1 = new SqlDataAdapter();
            DataTable dt1 = new DataTable();
            SqlCommand cmd1 = new SqlCommand("Get_UserIDComaSeprated", con);
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@Email", email);
            // cmd.Parameters.AddWithValue("@Password", password);
            da1.SelectCommand = cmd1;
            da1.Fill(dt1);
            UserId = dt1.Rows[0]["UserId"].ToString();
            loginfo = "Get_UserIDComaSeprated(" + email + ") end";
            Logger.Info(loginfo);
            return UserId;
        }
        catch (Exception ex)
        {
            Logger.Error("Get_UserIDComaSeprated " + ex.Message);
            throw ex;
        }
    }
    public int UpdateBlankUserAccMail(int empid, string Email, int MotherOrgid)
    {
        try
        {
            loginfo = "UpdateBlankUserAccMail(" + empid + "," + Email + ", " + MotherOrgid + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();
            SqlCommand cmd = new SqlCommand("UpdateBlankUserAccMail", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@empid", empid);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@MotherOrgid", MotherOrgid);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateBlankUserAccMail(" + empid + "," + Email + ", " + MotherOrgid + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("UpdateBlankUserAccMail" + ex.Message);
            throw ex;
        }
    }
}