using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AssgineManagerController
/// </summary>
public class AssgineManagerController
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

    public void InsertAssgineManger(AssgineManagerEntity Ame)
    {
        try
        {
            loginfo = "InsertAssgineManger(" + Ame + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd1 = new SqlCommand();
            cmd1.CommandText = "Sp_AssgineInsert";
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Connection = con;
            cmd1.Parameters.AddWithValue("@BusinessID", Ame.BusinessID);
            cmd1.Parameters.AddWithValue("@BusinessName", Ame.BusinessName);
            cmd1.Parameters.AddWithValue("@ManagerID", Ame.ManagerID);
            cmd1.Parameters.AddWithValue("@MangerName", Ame.MangerName);
            cmd1.Parameters.AddWithValue("@EmpId", Ame.EmpId);
            cmd1.Parameters.AddWithValue("@EmpName", Ame.EmpName);
            cmd1.ExecuteNonQuery();

            cmd1.Dispose();
            loginfo = "InsertAssgineManger(" + Ame + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_AssgineInsert" + ex.Message);
            throw ex;
        }
    }
}