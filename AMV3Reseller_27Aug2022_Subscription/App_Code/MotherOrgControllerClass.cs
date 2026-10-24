using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for MotherOrgControllerClass
/// </summary>
public class MotherOrgControllerClass
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
    public MotherOrgControllerClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int InsertMotherOrg(MotherOrgEntityClass MOrg)
    {
        try
        {
            loginfo = "InsertMotherOrg(" + MOrg + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_InsertMotherOrg";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            int Result = 0;
            cmd.Parameters.AddWithValue("@MotherOrgName", MOrg.MotherOrgName);
            cmd.Parameters.AddWithValue("@StreetNumber", MOrg.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", MOrg.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", MOrg.Apartment);
            cmd.Parameters.AddWithValue("@City", MOrg.City);
            cmd.Parameters.AddWithValue("@Province", MOrg.Province);
            cmd.Parameters.AddWithValue("@PostalCode", MOrg.PostalCode);
            cmd.Parameters.AddWithValue("@EnrollDate", MOrg.EnrollDate);
            cmd.Parameters.AddWithValue("@Status", MOrg.Status);
            cmd.Parameters.AddWithValue("@email", MOrg.Email);
            cmd.Parameters.AddWithValue("@ContactName", MOrg.ContactName);
            cmd.Parameters.AddWithValue("@ContactNumber", MOrg.ContactNumber);
            cmd.Parameters.AddWithValue("@check", MOrg.Checked);
            cmd.Parameters.Add("@MotherOrgId", SqlDbType.Int).Direction = ParameterDirection.Output;
            //Tran.Save("SavePersonalDetails");
            cmd.ExecuteNonQuery();
            Result = int.Parse(cmd.Parameters["@MotherOrgId"].Value.ToString());

            loginfo = "InsertMotherOrg(" + MOrg + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertMotherOrg" + ex.Message);
            throw ex;
        }
    }

    public void UpdateMotherOrg(MotherOrgEntityClass MOrg)
    {
        try
        {
            loginfo = "UpdateMotherOrg(" + MOrg + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_UpdateMotherOrg";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MotherOrgID", MOrg.MotherOrgID);
            cmd.Parameters.AddWithValue("@MotherOrgName", MOrg.MotherOrgName);
            cmd.Parameters.AddWithValue("@StreetNumber", MOrg.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", MOrg.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", MOrg.Apartment);
            cmd.Parameters.AddWithValue("@City", MOrg.City);
            cmd.Parameters.AddWithValue("@Province", MOrg.Province);
            cmd.Parameters.AddWithValue("@PostalCode", MOrg.PostalCode);
            cmd.Parameters.AddWithValue("@EnrollDate", MOrg.EnrollDate);
            cmd.Parameters.AddWithValue("@Status", MOrg.Status);
            cmd.Parameters.AddWithValue("@email", MOrg.Email);
            cmd.Parameters.AddWithValue("@ContactName", MOrg.ContactName);
            cmd.Parameters.AddWithValue("@check", MOrg.Checked);
            cmd.Parameters.AddWithValue("@ContactNumber", MOrg.ContactNumber);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateMotherOrg(" + MOrg + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateMotherOrg" + ex.Message);
            throw ex;
        }
    }

    public void UpdateMotherOrgWithNewEmail(MotherOrgEntityClass MOrg)
    {
        try
        {
            loginfo = "UpdateMotherOrg(" + MOrg + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_UpdateMotherOrgWithNewEmail";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@MotherOrgID", MOrg.MotherOrgID);
            cmd.Parameters.AddWithValue("@MotherOrgName", MOrg.MotherOrgName);
            cmd.Parameters.AddWithValue("@StreetNumber", MOrg.StreetNumber);
            cmd.Parameters.AddWithValue("@StreetName", MOrg.StreetName);
            cmd.Parameters.AddWithValue("@Apartment", MOrg.Apartment);
            cmd.Parameters.AddWithValue("@City", MOrg.City);
            cmd.Parameters.AddWithValue("@Province", MOrg.Province);
            cmd.Parameters.AddWithValue("@PostalCode", MOrg.PostalCode);
            cmd.Parameters.AddWithValue("@EnrollDate", MOrg.EnrollDate);
            cmd.Parameters.AddWithValue("@Status", MOrg.Status);
            cmd.Parameters.AddWithValue("@email", MOrg.Email);
            cmd.Parameters.AddWithValue("@ContactName", MOrg.ContactName);
            cmd.Parameters.AddWithValue("@ContactNumber", MOrg.ContactNumber);
            cmd.Parameters.AddWithValue("@OldEmail", MOrg.OldEmail);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateMotherOrg(" + MOrg + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateMotherOrg" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetMotherOrgList()
    {
        try
        {
            loginfo = "GetMotherOrgList() Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            getconnection();
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter da = new SqlDataAdapter();
            cmd.CommandText = "GetMotherOrgList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetMotherOrgList() end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetMotherOrgList" + ex.Message);
            throw ex;
        }
    }
}