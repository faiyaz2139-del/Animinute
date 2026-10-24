using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


/// <summary>
/// Summary description for StreetMasterClass
/// </summary>
public class StreetMasterClass
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
    public void InsertStreetMaster(string StreetPrepix, string StreetName, string MotherOrgId, string City, string Province)
    {
        try
        {
            loginfo = "InsertStreetMaster(" + StreetPrepix + "," + StreetName + "," + MotherOrgId + ", " + City + ", " + Province + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_InsertStreetMaster";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StreetPrepix", StreetPrepix);
            cmd.Parameters.AddWithValue("@StreetName", StreetName);
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@City", City);
            cmd.Parameters.AddWithValue("@Province", Province);
            cmd.ExecuteNonQuery();

            loginfo = "InsertStreetMaster(" + StreetPrepix + "," + StreetName + "," + MotherOrgId + ", " + City + ", " + Province + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertStreetMaster" + ex.Message);
            throw ex;
        }
    }
    public void UpdateStreetMaster(int StreetID, string StreetPrepix, string StreetName, string City, string Province)
    {
        try
        {
            loginfo = "UpdateStreetMaster(" + StreetID + "," + StreetPrepix + "," + StreetName + ", " + City + ", " + Province + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_UpdateStreetMaster";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StreetID", StreetID);
            cmd.Parameters.AddWithValue("@StreetPrepix", StreetPrepix);
            cmd.Parameters.AddWithValue("@StreetName", StreetName);
            cmd.Parameters.AddWithValue("@City", City);
            cmd.Parameters.AddWithValue("@Province", Province);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateStreetMaster(" + StreetID + "," + StreetPrepix + "," + StreetName + ", " + City + ", " + Province + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateStreetMaster" + ex.Message);
            throw ex;
        }
    }
    public void DeleteStreetMaster(int StreetID)
    {
        try
        {
            loginfo = "DeleteStreetMaster(" + StreetID + ") Start";
            Logger.Info(loginfo);
            getconnection();
            cmd.CommandText = "Sp_DeleteStreetMaster";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@StreetID", StreetID);

            cmd.ExecuteNonQuery();

            loginfo = "DeleteStreetMaster(" + StreetID + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_DeleteStreetMaster" + ex.Message);
            throw ex;
        }
    }
}