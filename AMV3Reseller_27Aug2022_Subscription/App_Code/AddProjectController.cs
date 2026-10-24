using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for AddProjectController
/// </summary>
public class AddProjectController
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

    public AddProjectController()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void InsertAssgineProject(AssignProjectEntity Ape)
    {
        try
        {
            loginfo = "InsertAssgineProject(" + Ape + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd1 = new SqlCommand();
            cmd1.CommandText = "Sp_AssgineProject";
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Connection = con;
            cmd1.Parameters.AddWithValue("@BusinessID", Ape.BusinessID);
            cmd1.Parameters.AddWithValue("@EmployeeID", Ape.EmployeeID);
            cmd1.Parameters.AddWithValue("@ProjectId", Ape.ProjectID);
            cmd1.ExecuteNonQuery();

            cmd1.Dispose();
            loginfo = "InsertAssgineProject(" + Ape + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_AssgineProject" + ex.Message);
            throw ex;
        }
    }

    public DataTable Sp_GetSubsBussProject(string email, int Bownerid, string UserTypeId)
    {
        getconnection();
        try
        {
            loginfo = "Sp_GetSubsBussProject(" + email + ", " + Bownerid + ") Start";
            Logger.Info(loginfo);
            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_GetSubsBussProject", con);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@EmployeeID", Bownerid);
            cmd.Parameters.AddWithValue("@UsertypeId", UserTypeId);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            return dt;

            loginfo = "Sp_GetSubsBussProject(" + email + ", " + Bownerid + ") rnd";
            Logger.Info(loginfo);

        }
        catch (Exception ex)
        {
            Logger.Error("Sp_GetSubsBussProject" + ex.Message);
            throw ex;
        }
    }
}