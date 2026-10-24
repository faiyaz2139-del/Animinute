using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for RateDetailsController
/// </summary>
public class RateDetailsController
{
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public RateDetailsController()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private SqlConnection con;
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
    public int InsertRateDetails(RateDetailsEntity RateEntity, int EmployeeId)
    {
        int result = 0;
        try
        {
            loginfo = "InsertRateDetails(" + RateEntity + "," + EmployeeId + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmdRateDetails = new SqlCommand();
            cmdRateDetails.CommandText = "SP_InsertRateDetails";
            cmdRateDetails.CommandType = CommandType.StoredProcedure;
            cmdRateDetails.Connection = con;
            cmdRateDetails.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmdRateDetails.Parameters.AddWithValue("@Rate", RateEntity.Rate);
            cmdRateDetails.Parameters.AddWithValue("@RateType", RateEntity.RateType);
            cmdRateDetails.Parameters.AddWithValue("@StartDate", RateEntity.StartDate);
            cmdRateDetails.Parameters.AddWithValue("@EndDate", RateEntity.EndDate);
            cmdRateDetails.Parameters.AddWithValue("@JobTitle", RateEntity.JobTitle);
            cmdRateDetails.Parameters.AddWithValue("@IsEmployeeEIExempty", RateEntity.IsEmployeeEIExempty);
            cmdRateDetails.Parameters.AddWithValue("@IsEmployeeCPPExempty", RateEntity.IsEmployeeCPPExempty);

            cmdRateDetails.Parameters.AddWithValue("@RetainVacation", RateEntity.RetainVacation);
            cmdRateDetails.Parameters.AddWithValue("@effectivedate", RateEntity.effectivedate);
            result = cmdRateDetails.ExecuteNonQuery();

            loginfo = "InsertRateDetails(" + RateEntity + "," + EmployeeId + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_InsertRateDetails" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetRateHistory(int EmpId)
    {
        try
        {
            loginfo = "GetRateHistory(" + EmpId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            DataTable dtratehistory = new DataTable();
            SqlCommand cmdRateHistory = new SqlCommand();
            SqlDataAdapter daRateHistoy = new SqlDataAdapter();
            cmdRateHistory.CommandText = "GetRateHistory";
            cmdRateHistory.CommandType = CommandType.StoredProcedure;
            cmdRateHistory.Parameters.AddWithValue("@EmployeeId", EmpId);
            cmdRateHistory.Connection = con;
            cmdRateHistory.ExecuteNonQuery();
            daRateHistoy.SelectCommand = cmdRateHistory;
            daRateHistoy.Fill(dtratehistory);

            loginfo = "GetRateHistory(" + EmpId + ") end";
            Logger.Info(loginfo);

            return dtratehistory;
        }
        catch (Exception ex)
        {
            Logger.Error("GetRateHistory" + ex.Message);
            throw ex;
        }
    }

    public DataTable Sp_GetSubsBussRate(string email, int Bownerid, string UserTypeid)
    {
        getconnection();
        try
        {
            loginfo = "Sp_GetSubsBussRate(" + email + ", " + Bownerid + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_GetSubsBussRate", con);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@EmployeeID", Bownerid);
            cmd.Parameters.AddWithValue("@UserTypeId", UserTypeid);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            return dt;

            loginfo = "Sp_GetSubsBussRate(" + email + ", " + Bownerid + ") rnd";
            Logger.Info(loginfo);

        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetSubsBussRate" + ex.Message);
            throw ex;
        }
    }

}