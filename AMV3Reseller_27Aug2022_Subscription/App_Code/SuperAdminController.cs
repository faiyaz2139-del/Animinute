using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for SuperAdminClass
/// </summary>
public class SuperAdminController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    CommanClass Comm = new CommanClass();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public SuperAdminController()
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

    public int UpdateSuperAdmin(string email, string password)
    {
        try
        {
            loginfo = "UpdateSuperAdmin(" + email + "," + password + ") Start";
            Logger.Info(loginfo);

            int Result = 0;
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_UpdateSuperAdmin");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);
            Result = cmd.ExecuteNonQuery();

            loginfo = "UpdateSuperAdmin(" + email + "," + password + ") end";
            Logger.Info(loginfo);
            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateSuperAdmin" + ex.Message);
            throw ex;
        }
    }
}