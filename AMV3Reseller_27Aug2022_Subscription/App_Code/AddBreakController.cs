using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddBreakController
/// </summary>
public class AddBreakController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public AddBreakController()
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
    public int InsertAddBreak(AddBreakEntity Abe)
    {
        try
        {
            loginfo = "InsertAddBreak(" + Abe + ") Start";
            Logger.Info(loginfo);
            getconnection();
            int Result = 0;
            SqlCommand cmd = new SqlCommand("Sp_InsertAddBreak");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@EmpId", Abe.EmpId);
            cmd.Parameters.AddWithValue("@BussID", Abe.BussID);
            cmd.Parameters.AddWithValue("@Month", Abe.Month);
            cmd.Parameters.AddWithValue("@Year", Abe.Year);
            cmd.Parameters.AddWithValue("@WeekNo", Abe.WeekNo);
            cmd.Parameters.AddWithValue("@TimeSheetDate", Abe.TimeSheetDate);
            cmd.Parameters.AddWithValue("@Day", Abe.Day);
            cmd.Parameters.AddWithValue("@BreakStart", Abe.BreakStart);
            cmd.Parameters.AddWithValue("@BreakEnd", Abe.BreakEnd);
            cmd.Parameters.AddWithValue("@BreakTotal", Abe.BreakTotal);
            Result = cmd.ExecuteNonQuery();
            loginfo = "InsertAddBreak(" + Abe + ") end";
            Logger.Info(loginfo);
            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAddBreak" + ex.Message);
            throw ex;
        }
    }


    public int InsertAddBreak_Project(AddBreakEntity Abe)
    {
        try
        {
            loginfo = "Sp_InsertAddBreak_Project(" + Abe + ") Start";
            Logger.Info(loginfo);
            getconnection();
            int Result = 0;
            SqlCommand cmd = new SqlCommand("Sp_InsertAddBreak_Project");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@EmpId", Abe.EmpId);
            cmd.Parameters.AddWithValue("@BussID", Abe.BussID);
            cmd.Parameters.AddWithValue("@Month", Abe.Month);
            cmd.Parameters.AddWithValue("@Year", Abe.Year);
            cmd.Parameters.AddWithValue("@WeekNo", Abe.WeekNo);
            cmd.Parameters.AddWithValue("@TimeSheetDate", Abe.TimeSheetDate);
            cmd.Parameters.AddWithValue("@Day", Abe.Day);
            cmd.Parameters.AddWithValue("@BreakStart", Abe.BreakStart);
            cmd.Parameters.AddWithValue("@BreakEnd", Abe.BreakEnd);
            cmd.Parameters.AddWithValue("@BreakTotal", Abe.BreakTotal);
            cmd.Parameters.AddWithValue("@ProjectId", Abe.ProjectId);

            Result = cmd.ExecuteNonQuery();
            loginfo = "Sp_InsertAddBreak_Project(" + Abe + ") end";
            Logger.Info(loginfo);
            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertAddBreak" + ex.Message);
            throw ex;
        }
    }



}