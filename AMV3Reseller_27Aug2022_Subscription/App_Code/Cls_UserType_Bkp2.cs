using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for Cls_UserType_Bkp2
/// </summary>
public class Cls_UserType_Bkp2
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
    public void InsertUserType(string Usertype, string MotherOrgId)
    {
        try
        {
            loginfo = "InsertUserType(" + Usertype + ", " + MotherOrgId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_InsertUserType";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserType", Usertype);
            cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
            cmd.ExecuteNonQuery();

            loginfo = "InsertUserType(" + Usertype + ", " + MotherOrgId + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertUserType" + ex.Message);
            throw ex;
        }
    }
    public void UpdateUserType(int User_TypeId, string Usertype)
    {
        try
        {
            loginfo = "UpdateUserType(" + User_TypeId + ", " + Usertype + ") Start";
            Logger.Info(loginfo);
            getconnection();

            cmd.CommandText = "Sp_UpdateUserType";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_TypeId", User_TypeId);
            cmd.Parameters.AddWithValue("@UserType", Usertype);
            cmd.ExecuteNonQuery();

            loginfo = "UpdateUserType(" + User_TypeId + ", " + Usertype + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_UpdateUserType" + ex.Message);
            throw ex;
        }
    }
    public void DeleteUserType(int User_TypeId)
    {
        try
        {
            loginfo = "DeleteUserType(" + User_TypeId + ") Start";
            Logger.Info(loginfo);

            getconnection();
            cmd.CommandText = "Sp_DeleteUserType";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@User_TypeId", User_TypeId);
            cmd.ExecuteNonQuery();

            loginfo = "DeleteUserType(" + User_TypeId + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_DeleteUserType" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetUserTypeCompanyWise(int BusinessID)
    {
        try
        {
            loginfo = "SP_UserType(" + BusinessID + ", " + BusinessID + ") Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand("SP_UserType", con);

            cmd.Parameters.AddWithValue("@BusinessID", BusinessID);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "SP_UserType(" + BusinessID + ", " + BusinessID + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_UserType" + ex.Message);
            throw ex;
        }
    }
}