using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for EmpAddressController
/// </summary>
public class EmpAddressController
{
    EmpAddressEntity EmpAddEnity = new EmpAddressEntity();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public EmpAddressController()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    private SqlConnection con;
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
    public int InsertEmpAddress(EmpAddressEntity EmpAddEnity, int EmpID)
    {
        int result = 0;
        try
        {
            loginfo = "InsertEmpAddress(" + EmpAddEnity + "," + EmpID + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand CmdInsertEmpAddress = new SqlCommand();
            CmdInsertEmpAddress.CommandText = "Sp_InsertEmpAddress";
            CmdInsertEmpAddress.Connection = con;
            CmdInsertEmpAddress.CommandType = CommandType.StoredProcedure;

            CmdInsertEmpAddress.Parameters.AddWithValue("@EmpID", EmpID);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Postalcode", EmpAddEnity.Postalcode);
            CmdInsertEmpAddress.Parameters.AddWithValue("@NumberSuffix", EmpAddEnity.NumberSuffix);
            CmdInsertEmpAddress.Parameters.AddWithValue("@StreetName", EmpAddEnity.StreetName);
            CmdInsertEmpAddress.Parameters.AddWithValue("@StreetType", EmpAddEnity.StreetType);
            CmdInsertEmpAddress.Parameters.AddWithValue("@StreetDirection", EmpAddEnity.StreetDirection);
            CmdInsertEmpAddress.Parameters.AddWithValue("@SuitType", EmpAddEnity.SuitType);
            CmdInsertEmpAddress.Parameters.AddWithValue("@SuiteNumber", EmpAddEnity.SuiteNumber);
            CmdInsertEmpAddress.Parameters.AddWithValue("@SuitSuffix", EmpAddEnity.SuitSuffix);
            CmdInsertEmpAddress.Parameters.AddWithValue("@City", EmpAddEnity.City);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Province", EmpAddEnity.Province);
            CmdInsertEmpAddress.Parameters.AddWithValue("@MainPhone", EmpAddEnity.MainPhone);
            CmdInsertEmpAddress.Parameters.AddWithValue("@WorkPhone", EmpAddEnity.WorkPhone);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Mobile", EmpAddEnity.Mobile);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Fax", EmpAddEnity.Fax);
            CmdInsertEmpAddress.Parameters.AddWithValue("@MainEmail", EmpAddEnity.MainEmail);
            CmdInsertEmpAddress.Parameters.AddWithValue("@CCEmail", EmpAddEnity.CCEmail);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Website", EmpAddEnity.Website);
            CmdInsertEmpAddress.Parameters.AddWithValue("@Other1", EmpAddEnity.Other1);

            CmdInsertEmpAddress.Parameters.AddWithValue("@CityId", EmpAddEnity.CityId);
            CmdInsertEmpAddress.Parameters.AddWithValue("@ProvinceId", EmpAddEnity.ProvinceId);

            CmdInsertEmpAddress.Parameters.AddWithValue("@StreetNo", EmpAddEnity.StreetNo);
            CmdInsertEmpAddress.Parameters.AddWithValue("@MotherOrgId", EmpAddEnity.MotherOrgId);
            int amBefore = AnyminuteSupportAddress.MaxMasterId(con, null); // v80: see AnyminuteSupportAddress.AfterLegacyInsert
            result = CmdInsertEmpAddress.ExecuteNonQuery();
            AnyminuteSupportAddress.AfterLegacyInsert(con, null, amBefore, AnyminuteSupportAddress.FromLegacy(con, null, EmpAddEnity.StreetNo, EmpAddEnity.StreetName, EmpAddEnity.StreetType,
                EmpAddEnity.SuitType, EmpAddEnity.SuiteNumber, EmpAddEnity.City, EmpAddEnity.CityId, EmpAddEnity.Province, EmpAddEnity.ProvinceId, EmpAddEnity.Postalcode));

            loginfo = "InsertEmpAddress(" + EmpAddEnity + "," + EmpID + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertEmpAddress" + ex.Message);
            throw ex;
        }
    }
}