using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddressMasterController
/// </summary>
public class AddressMasterController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

    SqlCommand cmd = new SqlCommand();
    DataSet ds = new DataSet();
    public AddressMasterController()
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

    public int Sp_InsertAddressMaster(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_InsertAddressMaster(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_InsertAddressMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Street_Name", AddMasterEntity.Street_Name);
            cmd.Parameters.AddWithValue("@City", AddMasterEntity.City);
            cmd.Parameters.AddWithValue("@Province", AddMasterEntity.Province);
            cmd.Parameters.AddWithValue("@PostalCode", AddMasterEntity.PostalCode);
            cmd.Parameters.AddWithValue("@Verifed", AddMasterEntity.Verifed);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_InsertAddressMaster(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAddressMaster" + ex.Message);
            throw ex;
        }
    }

    public int Sp_UpdateAddressMaster(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_UpdateAddressMaster(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_UpdateAddressMaster", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@AddressMasterId", AddMasterEntity.AddressMasterId);
            cmd.Parameters.AddWithValue("@Street_Name", AddMasterEntity.Street_Name);
            cmd.Parameters.AddWithValue("@City", AddMasterEntity.City);
            cmd.Parameters.AddWithValue("@Province", AddMasterEntity.Province);
            cmd.Parameters.AddWithValue("@PostalCode", AddMasterEntity.PostalCode);
            cmd.Parameters.AddWithValue("@Verifed", AddMasterEntity.Verifed);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_UpdateAddressMaster(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateAddressMaster" + ex.Message);
            throw ex;
        }
    }

    public int Sp_InsertCity(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_InsertCity(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();

            SqlCommand cmd = new SqlCommand("Sp_InsertCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CityName", AddMasterEntity.CityName);
            cmd.Parameters.AddWithValue("@Duplicate", AddMasterEntity.Duplicate);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_InsertCity(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int Sp_InsertProvince(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_InsertProvince(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();

            SqlCommand cmd = new SqlCommand("Sp_InsertProvince", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProvinceName", AddMasterEntity.ProvinceName);
            cmd.Parameters.AddWithValue("@Duplicate", AddMasterEntity.DuplicateProvince);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_InsertProvince(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public int Sp_UpdateCity(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_UpdateCity(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();

            SqlCommand cmd = new SqlCommand("Sp_UpdateCity", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CityId", AddMasterEntity.CityId);
            cmd.Parameters.AddWithValue("@CityName", AddMasterEntity.CityName);
            cmd.Parameters.AddWithValue("@Duplicate", AddMasterEntity.Duplicate);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_UpdateCity(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public int Sp_UpdateProvince(AddressMasterEntity AddMasterEntity)
    {
        try
        {
            loginfo = "Sp_UpdateProvince(" + AddMasterEntity + ") Start";
            Logger.Info(loginfo);

            int result = 0;
            getconnection();

            SqlCommand cmd = new SqlCommand("Sp_UpdateProvince", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProvinceId", AddMasterEntity.ProvinceId);
            cmd.Parameters.AddWithValue("@ProvinceName", AddMasterEntity.ProvinceName);
            cmd.Parameters.AddWithValue("@Duplicate", AddMasterEntity.DuplicateProvince);
            result = cmd.ExecuteNonQuery();

            loginfo = "Sp_UpdateProvince(" + AddMasterEntity + ") end";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}