using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AssgienDelegateController
/// </summary>
public class AssgienDelegateController
{
    SqlConnection con = new SqlConnection();
    CommanClass comm = new CommanClass();
    AssgienDelegateEntity AssdE = new AssgienDelegateEntity();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

    public AssgienDelegateController()
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
    public void InsertAssgienDelegates(AssgienDelegateEntity AssdE)
    {
        try
        {
            loginfo = "InsertAssgienDelegates(" + AssdE + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd1 = new SqlCommand();
            cmd1.CommandText = "Sp_AssgineDelegateInsert";
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Connection = con;
            cmd1.Parameters.AddWithValue("@BusinessID", AssdE.BusinessID);
            cmd1.Parameters.AddWithValue("@BusinessName", AssdE.BusinessName);
            cmd1.Parameters.AddWithValue("@DelegateID", AssdE.DelegateID);
            cmd1.Parameters.AddWithValue("@DelegateName", AssdE.DelegateName);
            cmd1.Parameters.AddWithValue("@EmpId", AssdE.EmpId);
            cmd1.Parameters.AddWithValue("@EmpName", AssdE.EmpName);
            cmd1.ExecuteNonQuery();

            cmd1.Dispose();
            loginfo = "InsertAssgienDelegates(" + AssdE + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_AssgineDelegateInsert" + ex.Message);
            throw ex;
        }
    }
}