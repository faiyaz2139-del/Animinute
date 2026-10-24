using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for ActiveDeactiveController
/// </summary>
public class ActiveDeactiveController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    CommanClass Comm = new CommanClass();

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;

    public ActiveDeactiveController()
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
    public DataTable GetEmpIDFromAccountant(int BoId)
    {
        try
        {
            DataTable dt = new DataTable();
            dt = Comm.GetAllDataFromTable("AssginBussiness m", "*", "m.BusinessID in (select BusinessID from AssginBussiness where EmployeeID = " + BoId + " and UserTypeId = 1)", "m.BusinessID");

            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}