using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for clsGlobalVar
/// </summary>
/// 


public class clsGlobalVar
{
    SqlConnection objSqlConn;
    public clsGlobalVar()
    {
        objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        //
        // TODO: Add constructor logic here
        //

    }

    public static void Show(string strMsg, Page cntPage)
    {

        Label lbl = new Label();
        lbl.Text = "<script language='javascript'> alert('" + strMsg + "');</Script>";
        cntPage.Controls.Add(lbl);

    }

    public static string ConvertToSql(string strValue)
    {
        return strValue.Replace("'", "''");
    }

    public static bool IsDate(string strDate)
    {
        DateTime dt;
        bool blnRetval = true;
        try
        {
            dt = DateTime.Parse(strDate);
        }
        catch
        {
            blnRetval = false;
        }
        return blnRetval;
    }

    public static bool IsNumeric(string text)
    {
        return Regex.IsMatch(text, "^\\d+$");
    }

    public static DataTable FillData(string strSql)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        DataTable dtRetval = new DataTable();
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlDataAdapter objDa = new SqlDataAdapter(strSql, objSqlConn);
            objDa.Fill(dtRetval);
        }
        catch { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        return dtRetval;
    }

    public static string ExecuteQuery(string strSql)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        string strRetval = "0";
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlCommand objCmd = new SqlCommand(strSql, objSqlConn);
            objCmd.ExecuteNonQuery();
            strRetval = "0";
        }
        catch (Exception ex)
        {
            strRetval = ex.Message.ToString();
        }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        return strRetval;
    }

    public static void DrpDwnDataCollect(DropDownList dd, string str)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            SqlDataReader dr;
            dd.Items.Clear();
            dd.Items.Add("-Select-");
            SqlCommand objCmd = new SqlCommand(str, objSqlConn);
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            dr = objCmd.ExecuteReader();
            string path1 = @"D:\A1.TXT"; ;
            string line1 = null;
            StreamWriter sw1 = File.AppendText(path1);



            while (dr.Read())
            {
                sw1.WriteLine(dr.GetValue(0).ToString());
                dd.Items.Add(dr.GetValue(0).ToString());

            }
            sw1.Close();
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }

        }
        catch (Exception EX)
        {
            string path1 = @"D:\A1.TXT"; ;
            string line1 = null;
            StreamWriter sw1 = File.AppendText(path1);
            sw1.WriteLine(EX.ToString());
            sw1.Close();
        }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
    }

    public static void DrpDwnData2Collect(DropDownList dd, string str)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            SqlDataReader dr;
            dd.Items.Clear();
            dd.Items.Add("- Select -");
            SqlCommand objCmd = new SqlCommand(str, objSqlConn);
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            dr = objCmd.ExecuteReader();
            while (dr.Read())
                dd.Items.Add(dr.GetValue(0).ToString() + "-" + dr.GetValue(1).ToString());
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        catch
        { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
    }

    public static DataSet id(string str)
    {
        DataSet dsToReturn = new DataSet();
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlCommand objFetchCommand = new SqlCommand(str, objSqlConn);
            SqlDataAdapter objAdapter = new SqlDataAdapter(objFetchCommand);
            objAdapter.Fill(dsToReturn);
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        catch
        { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        return dsToReturn;

    }
    public static string autoGenerateID(string str, string desc)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        SqlDataReader dr;
        string sGeneratedID = null;
        int counter = 0;
        try
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlCommand objFetchCommand = new SqlCommand(str, objSqlConn);
            dr = objFetchCommand.ExecuteReader();
            dr.Read();
            if (dr.HasRows == true)
            {
                counter = Convert.ToInt32(dr.GetValue(0).ToString());

            }
            counter = counter + 1;
            sGeneratedID = desc + counter;

            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        catch (Exception ex1)
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        return sGeneratedID;

    }
    public static string autoGenerateID_withzero(string str, string desc)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        SqlDataReader dr;
        string sGeneratedID = null;
        int counter = 0;
        try
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlCommand objFetchCommand = new SqlCommand(str, objSqlConn);
            dr = objFetchCommand.ExecuteReader();
            dr.Read();
            if (dr.HasRows == true)
            {
                counter = Convert.ToInt32(dr.GetValue(0).ToString());

            }
            counter = counter + 1;
            if (counter.ToString().Length == 1)
            {
                sGeneratedID = desc + "00" + counter;
            }
            if (counter.ToString().Length == 2)
            {
                sGeneratedID = desc + "0" + counter;
            }
            if (counter.ToString().Length == 3)
            {
                sGeneratedID = desc + counter;
            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        catch (Exception ex1)
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
        return sGeneratedID;

    }
    public static bool checkIfExists(string pTableName, string pColumnName, Object pValue)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(" + pColumnName + ") from " + pTableName + " where " + pColumnName + "='" + pValue + "' ", objSqlConn);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return (rows != 0) ? true : false;
        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return false;
        }

    }
    public static bool checkIfExists(string TableName, string ColumnName1, Object Value1, string ColumnName2, Object Value2)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(*) from " + TableName + " where " + ColumnName1 + "='" + Value1 + "' and " + ColumnName2 + "='" + Value2 + "'", objSqlConn);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return (rows != 0) ? true : false;
        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return false;
        }

    }

    public static bool checkIfExists(string TableName, string ColumnName1, Object Value1, string ColumnName2, Object Value2, string ColumnName3, Object Value3)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(*) from " + TableName + " where " + ColumnName1 + "='" + Value1 + "' and " + ColumnName2 + "='" + Value2 + "' and " + ColumnName3 + "='" + Value3 + "'", objSqlConn);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return (rows != 0) ? true : false;
        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return false;
        }

    }

    public static void chkboxcollect(CheckBoxList chklist, string str)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            SqlDataReader dr;
            chklist.Items.Clear();
            SqlCommand objCmd = new SqlCommand(str, objSqlConn);
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            dr = objCmd.ExecuteReader();
            while (dr.Read())
            {
                chklist.Items.Add(new ListItem(dr.GetValue(0).ToString(), dr.GetValue(1).ToString()));

            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }

        }
        catch
        { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
    }
    public static bool IUD(string pQuery)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            SqlCommand objIUDCommand = new SqlCommand(pQuery, objSqlConn);
            int iVal = objIUDCommand.ExecuteNonQuery();
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return (iVal == 1) ? true : false;
        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return false;
        }
    }
    public static void DrpDwnDatavaluecollect(DropDownList dd, string str)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            SqlDataReader dr;
            dd.Items.Clear();
            dd.Items.Add("-Select-");
            SqlCommand objCmd = new SqlCommand(str, objSqlConn);
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            dr = objCmd.ExecuteReader();
            while (dr.Read())
            {
                dd.Items.Add(new ListItem(dr.GetValue(0).ToString(), dr.GetValue(1).ToString()));

            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }

        }
        catch
        { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
    }
    public static DataSet FetchData(string pQuery)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            DataSet dsToReturn = new DataSet();
            SqlCommand objFetchCommand = new SqlCommand(pQuery, objSqlConn);
            SqlDataAdapter objAdapter = new SqlDataAdapter(objFetchCommand);
            objAdapter.Fill(dsToReturn);
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return dsToReturn;

        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return null;
        }

    }
    public static void DrpDwnDataShow(DropDownList dd, string str)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);

        try
        {
            SqlDataReader dr;
            SqlCommand objCmd = new SqlCommand(str, objSqlConn);
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            dr = objCmd.ExecuteReader();
            while (dr.Read())
            {
                dd.SelectedItem.Text = dr.GetString(0);

            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }

        }
        catch
        { }
        finally
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
        }
    }

    public static bool checkIfExists_loan(string TableName, string ColumnName1, string Value1, string ColumnName2, string Value2, string ColumnName3, string Value3)
    {
        SqlConnection objSqlConn = new SqlConnection(ConfigurationManager.AppSettings["AIWCdbConn"]);
        try
        {
            if (objSqlConn.State == ConnectionState.Closed) { objSqlConn.Open(); }
            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(*) from " + TableName + " where " + ColumnName1 + "=" + Value1 + " and " + ColumnName2 + "='" + Value2 + "' and " + ColumnName3 + "='" + Value3 + "'", objSqlConn);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return (rows != 0) ? true : false;
        }
        catch
        {
            if (objSqlConn.State == ConnectionState.Open) { objSqlConn.Close(); }
            return false;
        }
    }
}
