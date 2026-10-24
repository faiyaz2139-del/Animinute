using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for GraphClass
/// </summary>
public class GraphClass
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();


    SqlCommand cmd = new SqlCommand();
    DataSet ds = new DataSet();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    int i;
    public GraphClass()
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
    public DataTable AccountanGraphFig1(int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("AccountanGraphFig1", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanGraphFig2(int MotherOrgID, int EmployeeID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_AccountanGraphFig2", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanGraphFig3(int MotherOrgID, int EmployeeID, int WeekNo)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_AccountanGraphFig3", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanGraphFig4WeekBussCount(int MotherOrgID, string currdate)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("SP_4weekBussOwnerCount", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@currentdate", currdate);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanGraphFig4Busscnt(int MotherOrgID, string currdate)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_4weekBussCount", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@currentdate", currdate);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanGraphFig4empcnt(int MotherOrgID, string currdate, int BusinessID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("sp_grap4bussEmpCnt", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.Parameters.AddWithValue("@currentdate", currdate);
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable AccountanCard(int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_AccGraphCards", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //----------------------------------------------Accountant dashbord end ------------------------------------

    public DataTable BussOwnerHrWeekGraph(int BussID, int WeekNo)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("BussOwnerHrWeekGraph", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable BussOwnerNoEmpWeekGraph(int BussID, int WeekNo)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("sp_BussOwnerWeekWiseEmpWork", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-----------------------------------------------------BussOwnerEnd-------------------------------------------

    public DataTable ManagerEmpCountGraph(int ManagerID, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_ManagerEmpCountGraph", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable MangerHrWeekGraph(int ManagerID, int WeekNo, int BusinessID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("MangerHrWeekGraph", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    public DataTable MangerEmpWeekGraph(int ManagerID, int WeekNo, int BusinessID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("MangerEmpWeekGraph", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ManagerID", ManagerID);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);
            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable EmpWeekHrGraph(int Empid, int WeekNo)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("EmpWeekHrGraph", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmpId", Empid);
            cmd.Parameters.AddWithValue("@WeekNo", WeekNo);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-------------------------------------------------New Graph Report------------------------------------------------------

    public DataTable AccountantWeekly(int MotherOrgId, DateTime week1)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("AccountentWeekly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@week1", week1);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable AccountantMonthly(int MotherOrgId, DateTime month1)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("AccounantMonthly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@month1", month1);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable AccountantQutar(int MotherOrgId, int Year)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("AccountantQutar", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@Year", Year);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable AccountantYearly(int MotherOrgId, int Year)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("AccountantYearly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.Parameters.AddWithValue("@yr", Year);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable BoWeekly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_BoWeekly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable BoMonthly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("BoMonthly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable BoQuarterly(int Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("BoQuarterly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable BoYearly(DateTime Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("BoYearly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable EmpWeekly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_EmpWeekly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable SupWeekly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_SupWeekly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable EmpMonthly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        DataTable dt = new DataTable();
        try
        {

            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("EmpMonthly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            dt.Columns.AddRange(new DataColumn[4] { new DataColumn("WeekName"), new DataColumn("EmpCnt"), new DataColumn("HrCnt"), new DataColumn("MonthName") });
            dt.Rows.Add("Month-1", 0, 0, "Mar-2022");
            dt.Rows.Add("Month-2", 0, 0, "Feb-2022");
            dt.Rows.Add("Month-3", 0, 0, "Jan-2022");
            dt.Rows.Add("Month-4", 0, 0, "Dec-2021");
            dt.Rows.Add("Month-5", 0, 0, "Nov-2021");
            dt.Rows.Add("Month-6", 0, 0, "Oct-2021");
            return dt;
        }
    }

    public DataTable SupMonthly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("SupMonthly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable EmpQuarterly(int Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("EmpQuarterly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable SupQuarterly(int Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("[SupQuarterly]", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable EmpYearly(DateTime Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("EmpYearly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable SupYearly(DateTime Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("SupYearly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public DataTable OwnerWeekly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_OwerWeekly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable OwnerMonthly(DateTime week, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("OwnerMonthly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", week);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable OwnerQuarterly(int Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("OwnerQuarterly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Year", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable OwnerYearly(DateTime Year, int Bussid, int EmpId, int MotherOrgID)
    {
        try
        {
            DataTable dt = new DataTable();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("OwnerYearly", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@week", Year);
            cmd.Parameters.AddWithValue("@Bussid", Bussid);
            cmd.Parameters.AddWithValue("@EmpId", EmpId);
            cmd.Parameters.AddWithValue("@MotherOrgID", MotherOrgID);

            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}