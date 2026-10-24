using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ReportController
/// </summary>
public class ReportController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public ReportController()
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
    public DataTable GetReportAccindividual(string BussID, string Empid, DateTime FromDate, DateTime ToDate)
    {
        try
        {
            loginfo = "GetReportAccindividual(" + BussID + "," + Empid + "," + FromDate + ", " + ToDate + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Acc_Indvidual_eMP", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", Empid);
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetReportAccindividual(" + BussID + "," + Empid + "," + FromDate + ", " + ToDate + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Acc_Indvidual_eMP" + ex.Message);
            throw ex;
        }
    }
}