using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for TimeSheetControllerClass
/// </summary>
public class TimeSheetControllerClass
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
    public TimeSheetControllerClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public int InsertTimeSheet(TimeSheetEntityClass ts)
    {
        try
        {
            loginfo = "InsertTimeSheet(" + ts + ") Start";
            Logger.Info(loginfo);

            int TimeSheetID;
            getconnection();
            cmd.CommandText = "Sp_InsertTimeSheet";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@EmployeeId", ts.EmployeeId);
            cmd.Parameters.AddWithValue("@BusinessID", ts.BusinessID);
            cmd.Parameters.AddWithValue("@BillType", ts.BillType);
            cmd.Parameters.AddWithValue("@BillRate", ts.BillRate);
            cmd.Parameters.AddWithValue("@Entrydate", ts.Entrydate);
            cmd.Parameters.AddWithValue("@StartTime", ts.StartTime);
            cmd.Parameters.AddWithValue("@EndTime", ts.EndTime);
            cmd.Parameters.AddWithValue("@WorkingHr", ts.WorkingHr);
            cmd.Parameters.AddWithValue("@OverTimeHr", ts.OverTimeHr);
            cmd.Parameters.AddWithValue("@BusinessName", ts.BusinessName);
            cmd.Parameters.AddWithValue("@FirstName", ts.FirstName);
            cmd.Parameters.AddWithValue("@Description", ts.Description);
            cmd.Parameters.AddWithValue("@OverStartTime", ts.OverStartTime);
            cmd.Parameters.AddWithValue("@OverEndTime", ts.OverEndTime);
            cmd.Parameters.AddWithValue("@Day", ts.Day);
            cmd.Parameters.AddWithValue("@WorkonHoliday", ts.WorkonHoliday);
            cmd.Parameters.AddWithValue("@MotherOrgId", ts.MotherOrgId);

            cmd.Parameters.Add("@TimeSheetID", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery();
            TimeSheetID = int.Parse(cmd.Parameters["@TimeSheetID"].Value.ToString());

            loginfo = "InsertTimeSheet(" + ts + ") end";
            Logger.Info(loginfo);

            return TimeSheetID;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertTimeSheet" + ex.Message);
            throw ex;
        }
    }

    public void UpdateTimeSheet(TimeSheetEntityClass ts)
    {
        try
        {
            loginfo = "UpdateTimeSheet(" + ts + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_UpdateTimeSheet";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TimeSheetID", ts.TimeSheetID);
            cmd.Parameters.AddWithValue("@EmployeeId", ts.EmployeeId);
            cmd.Parameters.AddWithValue("@BusinessID", ts.BusinessID);
            cmd.Parameters.AddWithValue("@BillType", ts.BillType);
            cmd.Parameters.AddWithValue("@BillRate", ts.BillRate);
            cmd.Parameters.AddWithValue("@Entrydate", ts.Entrydate);
            cmd.Parameters.AddWithValue("@StartTime", ts.StartTime);
            cmd.Parameters.AddWithValue("@EndTime", ts.EndTime);
            cmd.Parameters.AddWithValue("@WorkingHr", ts.WorkingHr);
            cmd.Parameters.AddWithValue("@OverTimeHr", ts.OverTimeHr);
            cmd.Parameters.AddWithValue("@BusinessName", ts.BusinessName);
            cmd.Parameters.AddWithValue("@FirstName", ts.FirstName);
            cmd.Parameters.AddWithValue("@Description", ts.Description);
            cmd.Parameters.AddWithValue("@OverStartTime", ts.OverStartTime);
            cmd.Parameters.AddWithValue("@OverEndTime", ts.OverEndTime);
            cmd.Parameters.AddWithValue("@Day", ts.Day);
            cmd.Parameters.AddWithValue("@WorkonHoliday", ts.WorkonHoliday);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateTimeSheet(" + ts + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateTimeSheet" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetTimeSheetDetailsForEdit(DateTime Stdate, DateTime Enddate, int BussniessId, int EmployeeId)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "GetTimeSheetDetailsForEdit(" + Stdate + ", " + Enddate + ", " + BussniessId + ", " + EmployeeId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "GetTimeSheetDetailsForEdit";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", Stdate);
            cmd.Parameters.AddWithValue("@ToDate", Enddate);
            cmd.Parameters.AddWithValue("@BussniessId", BussniessId);
            cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetTimeSheetDetailsForEdit(" + Stdate + ", " + Enddate + ", " + BussniessId + ", " + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetDetailsForEdit" + ex.Message);
            throw ex;
        }
    }


    public DataTable Sp_GetTimesheetEmpWise(int EmployeeId, DateTime FromDate, DateTime ToDate, int EmpId)
    {
        SqlCommand cmd1 = new SqlCommand();
        DataTable dt1 = new DataTable();
        SqlDataAdapter da1 = new SqlDataAdapter();
        try
        {
            loginfo = "Sp_GetTimesheetEmpWise(" + EmployeeId + ", " + FromDate + ", " + ToDate + ", " + EmpId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd1.CommandText = "Sp_GetTimesheetEmpWise";
            cmd1.Connection = con;
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@BusinessID", EmployeeId);
            cmd1.Parameters.AddWithValue("@FromDate", FromDate);
            cmd1.Parameters.AddWithValue("@Todate", ToDate);
            cmd1.Parameters.AddWithValue("@Empid", EmpId);
            da1.SelectCommand = cmd1;
            da1.Fill(dt1);

            loginfo = "Sp_GetTimesheetEmpWise(" + EmployeeId + ", " + FromDate + ", " + ToDate + ", " + EmpId + ") end";
            Logger.Info(loginfo);

            return dt1;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetTimesheetEmpWise" + ex.Message);
            throw ex;
        }
    }

    public DataTable Sp_GetTimesheetEmpWise1_1(DateTime FromDate, DateTime todate, int MotherOrgID, int ManagerId, string BusinessID, string EmpId)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "Sp_GetTimesheetEmpWise1_1(" + FromDate + ", " + todate + ", " + MotherOrgID + ", " + ManagerId + ", " + BusinessID + ", " + EmpId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_GetTimesheetEmpWise1_1";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", todate);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@ManagerId", ManagerId);
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);

            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Sp_GetTimesheetEmpWise1_1(" + FromDate + ", " + todate + ", " + MotherOrgID + ", " + ManagerId + ", " + BusinessID + ", " + EmpId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetTimesheetEmpWise1_1" + ex.Message);
            throw ex;
        }
    }

    public void GetApproved(int EmployeeId, int BusinessID, string Approvedby, DateTime FromDate, DateTime todate)
    {
        SqlCommand cmdApproved = new SqlCommand();
        try
        {
            loginfo = "GetApproved(" + EmployeeId + ", " + BusinessID + ", " + Approvedby + ", " + FromDate + ", " + todate + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmdApproved.CommandText = "GetApproved";
            cmdApproved.CommandType = CommandType.StoredProcedure;
            cmdApproved.Connection = con;
            cmdApproved.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdApproved.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmdApproved.Parameters.AddWithValue("@Approvedby", Approvedby);
            cmdApproved.Parameters.AddWithValue("@FromDate", FromDate);
            cmdApproved.Parameters.AddWithValue("@todate", todate);

            cmdApproved.ExecuteNonQuery();
            cmdApproved.Dispose();

            loginfo = "GetApproved(" + EmployeeId + ", " + BusinessID + ", " + Approvedby + ", " + FromDate + ", " + todate + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("GetApproved" + ex.Message);
            throw ex;
        }
    }
    public void GetReject(int EmployeeId, int BusinessID, string Approvedby, DateTime FromDate, DateTime todate)
    {
        SqlCommand cmdReject = new SqlCommand();
        try
        {
            loginfo = "GetReject(" + EmployeeId + ", " + BusinessID + ", " + Approvedby + ", " + FromDate + ", " + todate + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmdReject.CommandText = "GetReject";
            cmdReject.CommandType = CommandType.StoredProcedure;
            cmdReject.Connection = con;
            cmdReject.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdReject.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmdReject.Parameters.AddWithValue("@Approvedby", Approvedby);
            cmdReject.Parameters.AddWithValue("@FromDate", FromDate);
            cmdReject.Parameters.AddWithValue("@todate", todate);
            cmdReject.ExecuteNonQuery();
            cmdReject.Dispose();

            loginfo = "GetReject(" + EmployeeId + ", " + BusinessID + ", " + Approvedby + ", " + FromDate + ", " + todate + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("GetReject" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetRateHistory(int EmployeeId)
    {
        try
        {
            loginfo = "GetRateHistory(" + EmployeeId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            cmd.CommandText = "GetRateHistory";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;

            cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);

            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetRateHistory(" + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetRateHistory" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetWeeklyMain(DateTime fromdate, DateTime Todate, int BusinessID, int EmployeeId)
    {
        DataTable dtmain = new DataTable();
        SqlCommand cmdmain = new SqlCommand();
        SqlDataAdapter damain = new SqlDataAdapter();
        try
        {
            loginfo = "GetWeeklyMain(" + fromdate + ", " + Todate + ", " + BusinessID + ", " + EmployeeId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmdmain.CommandText = "GetTimeSheetDataWeekWiseMain";
            cmdmain.CommandType = CommandType.StoredProcedure;
            cmdmain.Connection = con;
            cmdmain.Parameters.AddWithValue("@fromdate", fromdate);
            cmdmain.Parameters.AddWithValue("@Todate", Todate);
            cmdmain.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdmain.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            damain.SelectCommand = cmdmain;
            damain.Fill(dtmain);
            cmdmain.Dispose();

            loginfo = "GetWeeklyMain(" + fromdate + ", " + Todate + ", " + BusinessID + ", " + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dtmain;
        }
        catch (Exception ex)
        {
            Logger.Error("GetTimeSheetDataWeekWiseMain" + ex.Message);
            throw ex;
        }
    }
    public DataTable Getweelychild(int BusinessID, DateTime FromDate, DateTime Todate, int wk, int EmployeeId)
    {
        DataTable dtchild = new DataTable();
        SqlCommand cmdchild = new SqlCommand();
        SqlDataAdapter dachild = new SqlDataAdapter();
        try
        {
            loginfo = "Getweelychild(" + BusinessID + ", " + FromDate + ", " + Todate + ", " + wk + ", " + EmployeeId + ") end";
            Logger.Info(loginfo);

            getconnection();
            cmdchild.CommandText = "Sp_GetWeeklyChid";
            cmdchild.CommandType = CommandType.StoredProcedure;
            cmdchild.Connection = con;
            cmdchild.Parameters.AddWithValue("@fromdate", FromDate);
            cmdchild.Parameters.AddWithValue("@Todate", Todate);
            cmdchild.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmdchild.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            cmdchild.Parameters.AddWithValue("@wk", wk);
            dachild.SelectCommand = cmdchild;
            dachild.Fill(dtchild);
            cmdchild.Dispose();

            loginfo = "Getweelychild(" + BusinessID + ", " + FromDate + ", " + Todate + ", " + wk + ", " + EmployeeId + ") end";
            Logger.Info(loginfo);

            return dtchild;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetWeeklyChid" + ex.Message);
            throw ex;
        }
    }
}