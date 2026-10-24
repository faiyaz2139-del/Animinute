using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ChangePasswordControllerClass
/// </summary>
public class ChangePasswordControllerClass
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();

    SqlCommand cmd = new SqlCommand();
    DataSet ds = new DataSet();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
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
    public ChangePasswordControllerClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void UpdateChangePassword(ChangePasswordClass cp)
    {
        try
        {
            loginfo = "UpdateChangePassword(" + cp + ") Start";
            Logger.Info(loginfo);
            getconnection();
            cmd.CommandText = "Sp_ChangePasswor";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", cp.UserId);
            cmd.Parameters.AddWithValue("@Password", cp.Password);
            cmd.ExecuteNonQuery();
            loginfo = "UpdateChangePassword(" + cp + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ChangePasswor" + ex.Message);
            throw ex;
        }
    }
    //public DataTable GetAllData()
    //{
    //    DataTable dt = new DataTable();
    //    SqlDataAdapter da = new SqlDataAdapter();
    //    getconnection();
    //    SqlCommand cmd = new SqlCommand();
    //    cmd.CommandText = "GetAllData";
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.Connection = con;
    //    da.SelectCommand = cmd;
    //    da.Fill(dt);

    //    return dt;
    //}
}