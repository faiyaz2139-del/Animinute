using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for SearchAddressController
/// </summary>
public class SearchAddressController
{
    SearchAddressQry objQry = new SearchAddressQry();
    CommanClass comm = new CommanClass();
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public SearchAddressController()
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
    public DataTable GetSearchData(string DTF, string DTT)
    {
        var AllRole = comm.ExecuteQueryDataTable(objQry.GetSearchQry.Replace("@DTF", "'" + DTF + "'").Replace("@DTT", "'" + DTT + "'"));
        return AllRole;
    }
    public int InsertAddressDetails(string PostalCode, string StreetName, string City, string Province)
    {
        int Empid = 0;
        getconnection();
        SqlTransaction Tran = null;
        try
        {



            Tran = con.BeginTransaction();

            SqlCommand cmdAdressDetails = new SqlCommand();
            cmdAdressDetails.CommandText = objQry.UpdateBusinessAddressQry;
            cmdAdressDetails.CommandType = CommandType.Text;
            cmdAdressDetails.Transaction = Tran;
            cmdAdressDetails.Connection = con;
            cmdAdressDetails.Parameters.AddWithValue("@POSTALCODE", PostalCode);
            cmdAdressDetails.Parameters.AddWithValue("@STREETNAME", StreetName);
            cmdAdressDetails.Parameters.AddWithValue("@CITY", City);
            cmdAdressDetails.Parameters.AddWithValue("@PROVINCE", Province);
            int rw = cmdAdressDetails.ExecuteNonQuery();


            cmdAdressDetails.CommandText = objQry.UpdateEmpAddressQry;

            rw = cmdAdressDetails.ExecuteNonQuery();


            cmdAdressDetails.CommandText = objQry.updateAddressMaster;
            cmdAdressDetails.Parameters.Clear();
            cmdAdressDetails.Parameters.AddWithValue("@POSTALCODE", PostalCode);
            cmdAdressDetails.Parameters.AddWithValue("@STREETNAME", StreetName);
            rw = cmdAdressDetails.ExecuteNonQuery();

            cmdAdressDetails.CommandText = objQry.updateCity;
            cmdAdressDetails.Parameters.Clear();
            cmdAdressDetails.Parameters.AddWithValue("@POSTALCODE", PostalCode);
            cmdAdressDetails.Parameters.AddWithValue("@CITY", City);
            rw = cmdAdressDetails.ExecuteNonQuery();

            cmdAdressDetails.CommandText = objQry.updateProvince;
            cmdAdressDetails.Parameters.Clear();
            cmdAdressDetails.Parameters.AddWithValue("@POSTALCODE", PostalCode);
            cmdAdressDetails.Parameters.AddWithValue("@PROVINCE", Province);
            rw = cmdAdressDetails.ExecuteNonQuery();
            Tran.Commit();
            return 1;
        }
        catch (Exception ex)
        {
            Tran.Rollback();
            //Logger.Error("Sp_InsertEmpPersonalDetails" + ex.Message);
            throw ex;
        }
    }

}
public class SearchAddressQry
{
    public string GetSearchQry = @"SELECT DISTINCT POSTALCODE,STREETNAME,CITY,PROVINCE FROM(
SELECT DISTINCT M.[Postalcode],M.[StreetName],M.[City]
      ,M.[Province]
  FROM
  [eTimehierarchy_Partner_DB2021].[dbo].[BusinessAddress] M
  INNER JOIN [eTimehierarchy_Partner_DB2021].[dbo].[AddBusiness] C
 ON C.ADDRESSID=M.[BusinessAddressId]
  WHERE C.[BusinessAgreementDate] >= @DTF
  AND C.[BusinessAgreementDate] <=@DTT
  UNION
  SELECT DISTINCT M.[Postalcode],M.[StreetName],M.[City]
      ,M.[Province]
  FROM
  [eTimehierarchy_Partner_DB2021].[dbo].[EmpAddress] M
  INNER JOIN [eTimehierarchy_Partner_DB2021].[dbo].[AddBusiness] C
 ON C.ADDRESSID=M.[EmpAddressId]
  WHERE C.[BusinessAgreementDate] >= @DTF
  AND C.[BusinessAgreementDate] <=@DTT)TEMP
  where Postalcode<>''";

    public string UpdateBusinessAddressQry = @"UPDATE BUSINESSADDRESS  SET
[StreetName]=@STREETNAME ,
CITY=@CITY,
PROVINCE=@PROVINCE
WHERE POSTALCODE=@POSTALCODE";

    public string UpdateEmpAddressQry = @"UPDATE EMPADDRESS  SET
[StreetName]=@STREETNAME ,
CITY=@CITY,
PROVINCE=@PROVINCE
WHERE POSTALCODE=@POSTALCODE";

    public string updateCity = @"UPDATE CITYMASTER SET
CITYNAME = @CITY
WHERE CITYID IN (select CITY from AddressMaster WHERE POSTALCODE=@POSTALCODE)";

    public string updateProvince = @"UPDATE PROVINCEMASTER SET
PROVINCENAME = @PROVINCE
WHERE PROVINCEID IN (select PROVINCE from AddressMaster WHERE POSTALCODE=@POSTALCODE)";

    public string updateAddressMaster = @"UPDATE ADDRESSMASTER SET STREET_NAME = @STREETNAME WHERE POSTALCODE = @POSTALCODE";
}