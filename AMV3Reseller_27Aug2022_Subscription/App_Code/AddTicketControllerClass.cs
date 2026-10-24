using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddTicketControllerClass
/// </summary>
public class AddTicketControllerClass
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

    public AddTicketControllerClass()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int InsertTicket(AddTicketEntityClass AddTicketEntity)
    {
        int TicketId;
        getconnection();
        try
        {
            loginfo = "InsertTicket(" + AddTicketEntity + ") Start";
            Logger.Info(loginfo);

            SqlCommand cmdInsertTicket = new SqlCommand();
            cmdInsertTicket.CommandText = "Sp_InsertTicket";
            cmdInsertTicket.CommandType = CommandType.StoredProcedure;
            cmdInsertTicket.Connection = con;

            cmdInsertTicket.Parameters.AddWithValue("@MotherOrgId", AddTicketEntity.MotherOrgId);
            cmdInsertTicket.Parameters.AddWithValue("@EmpId", AddTicketEntity.EmpId);
            cmdInsertTicket.Parameters.AddWithValue("@Name", AddTicketEntity.Name);
            cmdInsertTicket.Parameters.AddWithValue("@EmailId", AddTicketEntity.EmailId);
            cmdInsertTicket.Parameters.AddWithValue("@PhoneNO", AddTicketEntity.PhoneNO);
            cmdInsertTicket.Parameters.AddWithValue("@ProblemType", AddTicketEntity.ProblemType);
            cmdInsertTicket.Parameters.AddWithValue("@Module", AddTicketEntity.Module);
            cmdInsertTicket.Parameters.AddWithValue("@FileUploadName", AddTicketEntity.FileUploadName);
            cmdInsertTicket.Parameters.AddWithValue("@Description", AddTicketEntity.Description);

            cmdInsertTicket.Parameters.Add("@TicketId", SqlDbType.Int).Direction = ParameterDirection.Output;

            cmdInsertTicket.ExecuteNonQuery();

            TicketId = int.Parse(cmdInsertTicket.Parameters["@TicketId"].Value.ToString());

            loginfo = "InsertTicket(" + AddTicketEntity + ") end";
            Logger.Info(loginfo);

            return TicketId;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertTicket" + ex.Message);
            throw ex;
        }
    }

    public DataTable UpdateTicket(AddTicketEntityClass AddTicketEntity, int TicketId1)
    {
        getconnection();
        try
        {
            loginfo = "UpdateTicket(" + AddTicketEntity + ", " + TicketId1 + ") Start";
            Logger.Info(loginfo);

            DataTable dtticket = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmdInsertTicket = new SqlCommand();
            cmdInsertTicket.CommandText = "Sp_UpdateTicketNo";
            cmdInsertTicket.CommandType = CommandType.StoredProcedure;
            cmdInsertTicket.Connection = con;

            cmdInsertTicket.Parameters.AddWithValue("@TicketId", TicketId1);
            cmdInsertTicket.Parameters.AddWithValue("@MotherOrgId", AddTicketEntity.MotherOrgId);
            cmdInsertTicket.Parameters.AddWithValue("@EmpId", AddTicketEntity.EmpId);

            cmdInsertTicket.Parameters.AddWithValue("@FileUploadName", AddTicketEntity.FileUploadName);

            da.SelectCommand = cmdInsertTicket;
            da.Fill(dtticket);

            loginfo = "UpdateTicket(" + AddTicketEntity + ", " + TicketId1 + ") end";
            Logger.Info(loginfo);

            return dtticket;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateTicketNo" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetTimeSheetDetailsForEdit(DateTime Stdate, DateTime Enddate, int MotherOrgId, int EmployeeId)
    {
        try
        {
            loginfo = "GetTimeSheetDetailsForEdit(" + Stdate + ", " + Enddate + ", " + MotherOrgId + ", " + EmployeeId + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            cmd.CommandText = "SP_Get_Ticket_List";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", Stdate);
            cmd.Parameters.AddWithValue("@ToDate", Enddate);
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetTimeSheetDetailsForEdit(" + Stdate + ", " + Enddate + ", " + MotherOrgId + ", " + EmployeeId + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_Get_Ticket_List" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetTimeSheetDetailsForAdmin(DateTime Stdate, DateTime Enddate, int MotherOrgId) //, int EmployeeId)
    {
        try
        {
            loginfo = "GetTimeSheetDetailsForAdmin(" + Stdate + ", " + Enddate + ", " + MotherOrgId + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            cmd.CommandText = "SP_Get_Ticket_List_Admin";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", Stdate);
            cmd.Parameters.AddWithValue("@ToDate", Enddate);
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            //cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetTimeSheetDetailsForAdmin(" + Stdate + ", " + Enddate + ", " + MotherOrgId + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_Get_Ticket_List_Admin" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetTimeSheetDetailsForAdmin_All(DateTime Stdate, DateTime Enddate) //, int EmployeeId)
    {
        try
        {
            loginfo = "GetTimeSheetDetailsForAdmin_All(" + Stdate + ", " + Enddate + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            cmd.CommandText = "SP_Get_Ticket_List_Admin_All";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@FromDate", Stdate);
            cmd.Parameters.AddWithValue("@ToDate", Enddate);
            //cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            //cmd.Parameters.AddWithValue("@EmployeeId", EmployeeId);
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetTimeSheetDetailsForAdmin_All(" + Stdate + ", " + Enddate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_Get_Ticket_List_Admin_All" + ex.Message);
            throw ex;
        }
    }
}