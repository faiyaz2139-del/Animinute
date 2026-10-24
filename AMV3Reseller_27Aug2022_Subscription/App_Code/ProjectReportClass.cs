using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ProjectReportClass
/// </summary>
public class ProjectReportClass
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public ProjectReportClass()
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

    public DataTable GetProjectReportGrid(int projectid, DateTime FromDate, DateTime Todate, int Status)
    {
        try
        {
            loginfo = "Sp_ProjectReportGridView(" + projectid + "," + FromDate + "," + Todate + ") Start";
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ProjectReportGridView", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectId", projectid);
            cmd.Parameters.AddWithValue("@Fromdate", FromDate);
            cmd.Parameters.AddWithValue("@Todate", Todate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            //if (Status == 0)
            //  cmd.Parameters.AddWithValue("@Status", "1,2,3,4");
            //else
            cmd.Parameters.AddWithValue("@Status", Status);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ProjectReportGridView" + ex.Message);
            throw ex;
        }
    }

    //firoz
    public DataTable GetProjectReportGrids(int projectid, DateTime FromDate, DateTime Todate, int Status)
    {
        try
        {
            loginfo = "Sp_ProjectReportGridViews(" + projectid + "," + FromDate + "," + Todate + ") Start";
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ProjectReportGridViews", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectId", projectid);
            cmd.Parameters.AddWithValue("@Fromdate", FromDate);
            cmd.Parameters.AddWithValue("@Todate", Todate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ProjectReportGridViews" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetProjectReportFromGrid(int projectid, DateTime FromDate, DateTime Todate, int Status)
    {
        try
        {
            loginfo = "Sp_ProjectReportFromGridViews(" + projectid + "," + FromDate + "," + Todate + ") Start";
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ProjectReportFromGridViews", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectId", projectid);
            cmd.Parameters.AddWithValue("@Fromdate", FromDate);
            cmd.Parameters.AddWithValue("@Todate", Todate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ProjectReportFromGridViews" + ex.Message);
            throw ex;
        }
    }

    //firoz
    public DataTable GetProjectReportFromGrids(int projectid, DateTime FromDate, DateTime Todate, int Status)
    {
        try
        {
            loginfo = "Sp_ProjectReportFromGridView(" + projectid + "," + FromDate + "," + Todate + ") Start";
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ProjectReportFromGridView", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ProjectId", projectid);
            cmd.Parameters.AddWithValue("@Fromdate", FromDate);
            cmd.Parameters.AddWithValue("@Todate", Todate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ProjectReportFromGridView" + ex.Message);
            throw ex;
        }
    }
}