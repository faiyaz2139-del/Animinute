using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for TimeSheetController
/// </summary>
public class TimeSheetController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public TimeSheetController()
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
    public DataTable GetTimeSheetEntryMainGv(int year, int month, int bid, int empid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand();
        try
        {
            loginfo = "GetTimeSheetEntryMainGv(" + year + "," + month + "," + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmd.CommandText = "GetTimeSheetEntryMainGv";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@month", month);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@bid", bid);
            cmd.Parameters.AddWithValue("@empid", empid);
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            da.Dispose();

            loginfo = "GetTimeSheetEntryMainGv(" + year + "," + month + "," + bid + ", " + empid + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetEntryMainGv" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }

    public DataTable GetTimeSheetEntryChildGv(int year, int month, int wk, int bid, int empid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmdchild = new SqlCommand();
        try
        {
            loginfo = "GetTimeSheetEntryChildGv(" + year + "," + month + ", " + wk + ", " + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmdchild.CommandText = "GetTimeSheetEntryChildGv";
            cmdchild.Connection = con;
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Parameters.AddWithValue("@month", month);
            cmdchild.Parameters.AddWithValue("@year", year);
            cmdchild.Parameters.AddWithValue("@wk", wk);
            cmdchild.Parameters.AddWithValue("@bid", bid);
            cmdchild.Parameters.AddWithValue("@empid", empid);
            da.SelectCommand = cmdchild;
            da.Fill(dt);
            cmdchild.Dispose();
            da.Dispose();

            loginfo = "GetTimeSheetEntryChildGv(" + year + "," + month + ", " + wk + ", " + bid + ", " + empid + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetEntryChildGv" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }

    public void Sp_InsertTimeSheetEntry(TimeSheetEntryEntity Tsee)
    {
        try
        {
            loginfo = "Sp_InsertTimeSheetEntry(" + Tsee + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmdInsert = new SqlCommand();
            cmdInsert.CommandText = "Sp_InsertTimeSheetEntry";
            cmdInsert.Connection = con;
            cmdInsert.CommandType = CommandType.StoredProcedure;
            cmdInsert.Parameters.AddWithValue("@TimeSheetEntryId", 0);
            cmdInsert.Parameters.AddWithValue("@EmpId", Tsee.EmpId);
            cmdInsert.Parameters.AddWithValue("@BussID", Tsee.BussID);
            cmdInsert.Parameters.AddWithValue("@Month", Tsee.Month);
            cmdInsert.Parameters.AddWithValue("@Year", Tsee.Year);
            cmdInsert.Parameters.AddWithValue("@WeekNo", Tsee.WeekNo);
            cmdInsert.Parameters.AddWithValue("@TimeSheetDate", Tsee.TimeSheetDate);
            cmdInsert.Parameters.AddWithValue("@Day", Tsee.Day);

            cmdInsert.Parameters.AddWithValue("@Over1_5x", Tsee.Over1_5x);
            cmdInsert.Parameters.AddWithValue("@Over2x", Tsee.Over2x);
            cmdInsert.Parameters.AddWithValue("@Holiday", Tsee.Holiday);
            cmdInsert.Parameters.AddWithValue("@Regular", Tsee.Regular);
            cmdInsert.Parameters.AddWithValue("@Total", Tsee.Total);
            cmdInsert.Parameters.AddWithValue("@Status", Tsee.Status);
            if (Tsee.ReqularStart != null)
            {
                cmdInsert.Parameters.AddWithValue("@ReqularStart", Tsee.ReqularStart);
            }
            else
            {
                cmdInsert.Parameters.AddWithValue("@ReqularStart", DBNull.Value);

            }
            if (Tsee.Regularend != null)
            {
                cmdInsert.Parameters.AddWithValue("@Regularend", Tsee.Regularend);
            }
            else
            {
                cmdInsert.Parameters.AddWithValue("@Regularend", DBNull.Value);
            }


            cmdInsert.Parameters.AddWithValue("@BreakStart", Tsee.BreakStart);
            cmdInsert.Parameters.AddWithValue("@BreakEnd", Tsee.BreakEnd);
            cmdInsert.Parameters.AddWithValue("@BreakTotal", Tsee.BreakTotal);
            cmdInsert.Parameters.AddWithValue("@Worked", Tsee.Worked);
            cmdInsert.Parameters.AddWithValue("@ProjectId", Tsee.ProjectId);
            cmdInsert.ExecuteNonQuery();

            loginfo = "Sp_InsertTimeSheetEntry(" + Tsee + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertTimeSheetEntry" + ex.Message);
            throw ex;
        }
    }




    public int Sp_InsertTimeSheetEntryProject(TimeSheetEntryEntity Tsee)
    {
        try
        {
            loginfo = "Sp_InsertTimeSheetEntry_Project(" + Tsee + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmdInsert = new SqlCommand();
            cmdInsert.CommandText = "Sp_InsertTimeSheetEntry_Project";
            cmdInsert.Connection = con;
            cmdInsert.CommandType = CommandType.StoredProcedure;
            cmdInsert.Parameters.AddWithValue("@TimeSheetEntryId", 0);
            cmdInsert.Parameters.AddWithValue("@EmpId", Tsee.EmpId);
            cmdInsert.Parameters.AddWithValue("@BussID", Tsee.BussID);
            cmdInsert.Parameters.AddWithValue("@Month", Tsee.Month);
            cmdInsert.Parameters.AddWithValue("@Year", Tsee.Year);
            cmdInsert.Parameters.AddWithValue("@WeekNo", Tsee.WeekNo);
            cmdInsert.Parameters.AddWithValue("@TimeSheetDate", Tsee.TimeSheetDate);
            cmdInsert.Parameters.AddWithValue("@Day", Tsee.Day);

            cmdInsert.Parameters.AddWithValue("@Over1_5x", Tsee.Over1_5x);
            cmdInsert.Parameters.AddWithValue("@Over2x", Tsee.Over2x);
            cmdInsert.Parameters.AddWithValue("@Holiday", Tsee.Holiday);
            cmdInsert.Parameters.AddWithValue("@Regular", Tsee.Regular);
            cmdInsert.Parameters.AddWithValue("@Total", Tsee.Total);
            cmdInsert.Parameters.AddWithValue("@Status", Tsee.Status);
            if (Tsee.ReqularStart != null)
            {
                cmdInsert.Parameters.AddWithValue("@ReqularStart", Tsee.ReqularStart);
            }
            else
            {
                cmdInsert.Parameters.AddWithValue("@ReqularStart", DBNull.Value);

            }
            if (Tsee.Regularend != null)
            {
                cmdInsert.Parameters.AddWithValue("@Regularend", Tsee.Regularend);
            }
            else
            {
                cmdInsert.Parameters.AddWithValue("@Regularend", DBNull.Value);
            }
            cmdInsert.Parameters.AddWithValue("@BreakStart", Tsee.BreakStart);
            cmdInsert.Parameters.AddWithValue("@BreakEnd", Tsee.BreakEnd);
            cmdInsert.Parameters.AddWithValue("@BreakTotal", Tsee.BreakTotal);
            cmdInsert.Parameters.AddWithValue("@Worked", Tsee.Worked);
            cmdInsert.Parameters.AddWithValue("@ProjectName", Tsee.ProjectId);
            var result = cmdInsert.ExecuteScalar();

            loginfo = "Sp_InsertTimeSheetEntry_Project(" + Tsee + ") end";
            Logger.Info(loginfo);

            return Convert.ToInt32(result);

        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertTimeSheetEntry" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetTimeSheetEntryChildGv_Project_Emp(int year, int month, int Date, int bid, int empid, int ProjectId)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmdchild = new SqlCommand();
        try
        {
            loginfo = "GetTimeSheetEntryChildGv_Project_Emp(" + year + "," + month + ", " + Date + ", " + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmdchild.CommandText = "GetTimeSheetEntryChildGv_Project_Emp";
            cmdchild.Connection = con;
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Parameters.AddWithValue("@month", month);
            cmdchild.Parameters.AddWithValue("@year", year);
            cmdchild.Parameters.AddWithValue("@Date", Date);
            cmdchild.Parameters.AddWithValue("@bid", bid);
            cmdchild.Parameters.AddWithValue("@empid", empid);
            cmdchild.Parameters.AddWithValue("@ProjectId", ProjectId);
            da.SelectCommand = cmdchild;
            da.Fill(dt);
            cmdchild.Dispose();
            da.Dispose();

            loginfo = "GetTimeSheetEntryChildGv(" + year + "," + month + ", " + Date + ", " + bid + ", " + empid + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetEntryChildGv" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }


    public DataTable GetTimeSheetEntryChildGv_Project_Emp_ALL(int year, int month, int Date, int bid, int empid, string ProjectId)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmdchild = new SqlCommand();
        try
        {
            loginfo = "GetTimeSheetEntryChildGv_Project_Emp_All(" + year + "," + month + ", " + Date + ", " + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmdchild.CommandText = "GetTimeSheetEntryChildGv_Project_Emp_All";
            cmdchild.Connection = con;
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Parameters.AddWithValue("@month", month);
            cmdchild.Parameters.AddWithValue("@year", year);
            cmdchild.Parameters.AddWithValue("@Date", Date);
            cmdchild.Parameters.AddWithValue("@bid", bid);
            cmdchild.Parameters.AddWithValue("@empid", empid);
            cmdchild.Parameters.AddWithValue("@ProjectId", ProjectId);
            da.SelectCommand = cmdchild;
            da.Fill(dt);
            cmdchild.Dispose();
            da.Dispose();

            loginfo = "GetTimeSheetEntryChildGv_Project_Emp_All(" + year + "," + month + ", " + Date + ", " + bid + ", " + empid + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetEntryChildGv" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }

    public DataTable Get_Cutsom_Role_User_Regular_Emp_List_Timesheet(int bid, int empid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmdchild = new SqlCommand();
        try
        {
            loginfo = "Get_Cutsom_Role_User_Regular_Emp_List_Timesheet(" + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmdchild.CommandText = "Get_Cutsom_Role_User_Regular_Emp_List_Timesheet";
            cmdchild.Connection = con;
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Parameters.AddWithValue("@BusinessID", bid);
            cmdchild.Parameters.AddWithValue("@CustomRoleUser_EmployeeId", empid);
            da.SelectCommand = cmdchild;
            da.Fill(dt);
            cmdchild.Dispose();
            da.Dispose();

            loginfo = "Get_Cutsom_Role_User_Regular_Emp_List_Timesheet(" + bid + ", " + empid + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Get_Cutsom_Role_User_Regular_Emp_List_Timesheet" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }

    public DataTable Get_Cutsom_Role_User_Project_Emp_List_Timesheet(int bid, int empid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmdchild = new SqlCommand();
        try
        {
            loginfo = "Get_Cutsom_Role_User_Project_Emp_List_Timesheet(" + bid + ", " + empid + ") Start";
            Logger.Info(loginfo);

            getconnection();

            cmdchild.CommandText = "Get_Cutsom_Role_User_Project_Emp_List_Timesheet";
            cmdchild.Connection = con;
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Parameters.AddWithValue("@BusinessID", bid);
            cmdchild.Parameters.AddWithValue("@CustomRoleUser_EmployeeId", empid);
            da.SelectCommand = cmdchild;
            da.Fill(dt);
            cmdchild.Dispose();
            da.Dispose();

            loginfo = "Get_Cutsom_Role_User_Project_Emp_List_Timesheet(" + bid + ", " + empid + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Get_Cutsom_Role_User_Project_Emp_List_Timesheet" + ex.Message);
            throw ex;
        }
        finally
        {

        }
    }
}