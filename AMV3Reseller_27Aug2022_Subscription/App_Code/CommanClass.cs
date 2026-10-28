using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for CommanClass
/// </summary>
public class CommanClass
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
    public string GetCurrentSession()
    {
        string SessionName = string.Empty;
        SqlDataReader dr;
        try
        {
            getconnection();
            cmd = new SqlCommand("  select Start_Year+'-'+End_year as SessionName from Session where Iscurrent=1", con);
            dr = cmd.ExecuteReader();
            dr.Read();
            if (!dr.IsDBNull(0))
            {
                SessionName = dr[0].ToString();
            }
            return SessionName;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    //#region For Report Genration
    //public ConnectionInfo GetCrystalConnection()
    //{
    //    //SET Login Details
    //    //DB DETAILS
    //    ConnectionInfo connectionInfo = new ConnectionInfo();

    //    //Following for Remote Server
    //    connectionInfo.UserID = System.Configuration.ConfigurationManager.AppSettings["UserID"].ToString();
    //    connectionInfo.Password = System.Configuration.ConfigurationManager.AppSettings["Password"].ToString();
    //    connectionInfo.ServerName = System.Configuration.ConfigurationManager.AppSettings["Server"].ToString();
    //    connectionInfo.DatabaseName = System.Configuration.ConfigurationManager.AppSettings["DataBase"].ToString();


    //    return connectionInfo;
    //}
    //public void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
    //{
    //    Tables tables = reportDocument.Database.Tables;
    //    foreach (CrystalDecisions.CrystalReports.Engine.Table table in tables)
    //    {
    //        TableLogOnInfo tableLogonInfo = table.LogOnInfo;
    //        tableLogonInfo.ConnectionInfo = connectionInfo;
    //        table.ApplyLogOnInfo(tableLogonInfo);
    //    }
    //}
    //#endregion

    public void FillDropDownListAJAX(AjaxControlToolkit.ComboBox ddlList, string TableName, string Column1, string Column2, string wherecondition, string orderby)
    {
        try
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);
            ddlList.DataSource = null;
            ddlList.DataBind();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", Column1);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);
            ddlList.Items.Clear();
            //ddlList.Items.Add("Please Select");
            //ddlList.SelectedItem.Value = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddlList.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddlList.DataBind();
                ddlList.SelectedIndex = 0;
            }

            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
        // return ddlList;
    }


    public void FillDropDownList(DropDownList ddlList, string TableName, string Column1, string Column2, string wherecondition, string orderby)
    {
        try
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);
            ddlList.DataSource = null;
            ddlList.DataBind();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", Column1);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);
            ddlList.Items.Clear();
            ddlList.Items.Add("Please Select");
            ddlList.SelectedItem.Value = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddlList.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddlList.DataBind();
                foreach (ListItem item in ddlList.Items) item.Text = AnyminuteDisplayText.Tidy(item.Text); // v27: "Adam  Rhodes", "Supervisor Emplyee"
                ddlList.SelectedIndex = 0;
            }


            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
        // return ddlList;
    }

    public void FillDropDownList_message(DropDownList ddlList, string TableName, string Column1, string Column2, string wherecondition, string orderby, string message)
    {
        try
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);
            ddlList.DataSource = null;
            ddlList.DataBind();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", Column1);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);
            ddlList.Items.Clear();
            ddlList.Items.Add("Please Select " + message);
            ddlList.SelectedItem.Value = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddlList.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddlList.DataBind();
                ddlList.SelectedIndex = 0;
            }

            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillDropDownList(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
        // return ddlList;
    }

    public bool DeleteUser(int EmployeeId)
    {
        bool DeleteUser = false;
        try
        {
            loginfo = "DeleteUser(" + EmployeeId.ToString() + ")";
            Logger.Info(loginfo);

            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("DeleteUser", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmpId", EmployeeId);

            cmd.ExecuteNonQuery();
            DeleteUser = true;
            return DeleteUser;
        }
        catch (Exception ex)
        {
            loginfo = loginfo = "DeleteUser(" + EmployeeId.ToString() + ")" + ex.Message;
            Logger.Error(loginfo);
            
            throw ex;

        }
        
        // return ddlList;
    }



    public void FillGridView(GridView GvDataGridView, string TableName, string ColumnNames, string Column2, string wherecondition, string orderby)
    {
        //Column names with comma seprated in single string
        try
        {
            loginfo = "FillGridView(Select " + ColumnNames + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);

            getconnection();

            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", ColumnNames);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                GvDataGridView.DataSource = ds;

                GvDataGridView.DataBind();

            }

            cmd.Dispose();
            ds.Dispose();

            loginfo = "FillGridView(Select " + ColumnNames + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillGridView(Select " + ColumnNames + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }

    }

    public DataTable GetAllDataFromTable(string TableName, string ColumnNames, string wherecondition, string orderby)
    {
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetAllDataFromTable(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_GetAllDataFromTable", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAMES", ColumnNames);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "GetAllDataFromTable(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
            return dt;

        }
        catch (Exception ex)
        {
            loginfo = "GetAllDataFromTable(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
    }
    public DataTable GetSearchAddress(string PostalCode)
    {
        try
        {
            loginfo = "GetSearchAddress(" + PostalCode + ") Start";
            Logger.Info(loginfo);
            DataTable dtSearch = new DataTable();
            getconnection();
            SqlDataAdapter daSearch = new SqlDataAdapter("Sp_SearchAddress", con);
            daSearch.SelectCommand.CommandType = CommandType.StoredProcedure;
            daSearch.SelectCommand.Parameters.AddWithValue("@PostalCode", PostalCode);
            daSearch.Fill(dtSearch);

            loginfo = "GetSearchAddress(" + PostalCode + ") end";
            Logger.Info(loginfo);
            return dtSearch;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_SearchAddress" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetSearchPostalCode(string PostalCode)
    {
        try
        {
            loginfo = "GetSearchPostalCode(" + PostalCode + ") Start";
            Logger.Info(loginfo);
            DataTable dtSearch = new DataTable();
            getconnection();
            SqlDataAdapter daSearch = new SqlDataAdapter("Sp_SearchPostalCode", con);
            daSearch.SelectCommand.CommandType = CommandType.StoredProcedure;
            daSearch.SelectCommand.Parameters.AddWithValue("@PostalCode", PostalCode);
            daSearch.Fill(dtSearch);

            loginfo = "GetSearchPostalCode(" + PostalCode + ") end";
            Logger.Info(loginfo);
            return dtSearch;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_SearchPostalCode" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetSearchAddress_FillAddress(string PostalCode)
    {
        try
        {
            loginfo = "GetSearchAddress_FillAddress(" + PostalCode + ") Start";
            Logger.Info(loginfo);
            DataTable dtSearch = new DataTable();
            getconnection();
            SqlDataAdapter daSearch = new SqlDataAdapter("Sp_SearchAddress_FillAddress", con);
            daSearch.SelectCommand.CommandType = CommandType.StoredProcedure;
            daSearch.SelectCommand.Parameters.AddWithValue("@PostalCode", PostalCode);
            daSearch.Fill(dtSearch);

            loginfo = "GetSearchAddress_FillAddress(" + PostalCode + ") end";
            Logger.Info(loginfo);
            return dtSearch;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_SearchAddress_FillAddress" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetMenuData(string regno)
    {
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetMenuData(select* from LinkMaster LM INNER JOIN AssginLink AL on AL.Menuid = LM.MenuID where AL.Regno = '" + regno + "' ORDER BY LM.MenuSlno ) Start";
            Logger.Info(loginfo);
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter("select * from LinkMaster LM INNER JOIN AssginLink AL on AL.Menuid=LM.MenuID where AL.Regno='" + regno + "' ORDER BY LM.MenuSlno ", con);
            da.Fill(dt);
            loginfo = "GetMenuData(select* from LinkMaster LM INNER JOIN AssginLink AL on AL.Menuid = LM.MenuID where AL.Regno = '" + regno + "' ORDER BY LM.MenuSlno ) end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            loginfo = "GetMenuData(select* from LinkMaster LM INNER JOIN AssginLink AL on AL.Menuid = LM.MenuID where AL.Regno = '" + regno + "' ORDER BY LM.MenuSlno )" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
    }

    public bool IfExistsInTable(string TableName, string ColumnNames, string wherecondition)
    {
        bool r = false;
        DataTable dt = new DataTable();
        try
        {
            loginfo = "IfExistsInTable(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_IfExistsInTable", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAMES", ColumnNames);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            if (Convert.ToInt32(dt.Rows[0][0].ToString()) > 0)
            {
                r = true;
            }

            loginfo = "IfExistsInTable(Select " + ColumnNames + ", from " + TableName + " where " + wherecondition + ") end";
            Logger.Info(loginfo);

            return r;
        }
        catch (Exception ex)
        {
            loginfo = "IfExistsInTable(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
    }

    public int GetMaxValue(string TableName, string ColumnNames, string wherecondition)
    {
        int result = 0;
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetMaxValue(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_GetMaxValue", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAMES", ColumnNames);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            da.Fill(dt);
            result = Convert.ToInt32(dt.Rows[0][0].ToString());
            loginfo = "GetMaxValue(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + ") Start";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            loginfo = "GetMaxValue(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }

    }
    public DataTable GeGetMonthOnSession(int INSD_Id)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "GeGetMonthOnSession(" + INSD_Id + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("SP_GetMonthOnSession", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@INSD_Id", INSD_Id);

            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "GeGetMonthOnSession(" + INSD_Id + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_GetMonthOnSession" + ex.Message);
            throw ex;
        }
    }
    public int ExecuteQuery(string query)
    {
        int result = 0;
        try
        {
            loginfo = "ExecuteQuery(" + query + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Utils_ExecuteQuery", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@query", query);
            result = cmd.ExecuteNonQuery();
            loginfo = "ExecuteQuery(" + query + ") End";
            Logger.Info(loginfo);
            return result;
        }
        catch (Exception ex)
        {
            loginfo = "ExecuteQuery(" + query + ") " + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }

    }

    public DataTable ExecuteQueryDataTable(string query)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ExecuteQuery(" + query + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Utils_ExecuteQuery", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@query", query);
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "ExecuteQuery(" + query + ") End";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            loginfo = "ExecuteQuery(" + query + ") " + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }

    }
    public string GetNameFromId(string P_TYPE, int P_ID)
    {
        try
        {
            loginfo = "GetNameFromId(" + P_TYPE + "," + P_ID + ") Start";
            Logger.Info(loginfo);
            DataTable dtGetNameFromId = new DataTable();
            String Name;
            getconnection();
            SqlCommand cmd = new SqlCommand("GetNameFromId", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TYPE", P_TYPE);
            cmd.Parameters.AddWithValue("@P_ID", P_ID);

            da.SelectCommand = cmd;
            da.Fill(dtGetNameFromId);
            Name = dtGetNameFromId.Rows[0]["NAME"].ToString();
            loginfo = "GetNameFromId(" + P_TYPE + "," + P_ID + ") end";
            Logger.Info(loginfo);
            return Name;

        }
        catch (Exception ex)
        {
            Logger.Error("GetNameFromId" + ex.Message);
            throw ex;
        }

    }



    public void FillGridViewDataNull(GridView GvDataGridView, string TableName, string ColumnNames, string Column2, string wherecondition, string orderby)
    {
        //Column names with comma seprated in single string
        try
        {
            loginfo = "FillGridViewDataNull(Select " + ColumnNames + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);
            getconnection();

            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", ColumnNames);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);


            if (ds.Tables[0].Rows.Count > 0)
            {
                GvDataGridView.DataSource = ds;

                GvDataGridView.DataBind();

            }
            if (ds.Tables[0].Rows.Count == 0)
            {
                GvDataGridView.DataSource = null;

                GvDataGridView.DataBind();

            }

            cmd.Dispose();
            ds.Dispose();
            loginfo = "FillGridViewDataNull(Select " + ColumnNames + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillGridViewDataNull(Select " + ColumnNames + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }

    }

    public void FillDropDownList_NoPleaseSelect(DropDownList ddlList, string TableName, string Column1, string Column2, string wherecondition, string orderby)
    {
        try
        {
            loginfo = "FillDropDownList_NoPleaseSelect(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") Start";
            Logger.Info(loginfo);
            ddlList.DataSource = null;
            ddlList.DataBind();
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand("Sp_Utils_FillDropdown", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@P_TABLENAME", TableName);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_1", Column1);
            cmd.Parameters.AddWithValue("@P_COLUMNNAME_2", Column2);
            cmd.Parameters.AddWithValue("@P_WHERECONDITION", wherecondition);
            cmd.Parameters.AddWithValue("@P_ORDERBY", orderby);
            cmd.ExecuteNonQuery();
            da.SelectCommand = cmd;
            ds.Reset();
            da.Fill(ds);
            ddlList.Items.Clear();
            //ddlList.Items.Add("Please Select");
            //ddlList.SelectedItem.Value = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlList.DataSource = ds;
                ddlList.DataValueField = ds.Tables[0].Columns[0].ToString();
                ddlList.DataTextField = ds.Tables[0].Columns[1].ToString();
                ddlList.DataBind();
                ddlList.SelectedIndex = 0;
            }
            loginfo = "FillDropDownList_NoPleaseSelect(Select " + Column1 + ", " + Column2 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            loginfo = "FillDropDownList_NoPleaseSelect(Select " + Column1 + " from " + TableName + " where " + wherecondition + " orderby " + orderby + ")" + ex.Message;
            Logger.Error(loginfo);
            throw ex;
        }
        // return ddlList;
    }



    public string encrypt(string encryptString)
    {
        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }

    public string Decrypt(string cipherText)
    {

        string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }

    public DataTable ReportBusinessWise(string BussID, DateTime FromDate, DateTime ToDate)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportBusinessWise(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_ReportBusinessWise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "ReportBusinessWise(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ReportBusinessWise" + ex.Message);
            throw ex;
        }
    }

    public DataTable ReportAcc_BusinessOwner(string BussID, DateTime FromDate, DateTime ToDate)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_BusinessOwner(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_ReportACC_EMPBusinessWise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_BusinessOwner(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_ReportACC_EMPBusinessWise" + ex.Message);
            throw ex;
        }
    }


    public DataTable ReportAcc_CustomerWise_Excel(string BussID, DateTime FromDate, DateTime ToDate, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise(" + BussID + "," + FromDate + "," + ToDate + ", " + Status + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_Excel", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else if (Status == 1)
                cmd.Parameters.AddWithValue("@Status", "0,1");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "ReportAcc_CustomerWise(" + BussID + "," + FromDate + "," + ToDate + ", " + Status + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise_Excel" + ex.Message);
            throw ex;
        }
    }
    public DataTable ReportAcc_CustomerWise(string BussID, DateTime FromDate, DateTime ToDate, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise(" + BussID + "," + FromDate + "," + ToDate + ", " + Status + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            // if (Status == 0)
            //   cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            //else
            cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "ReportAcc_CustomerWise(" + BussID + "," + FromDate + "," + ToDate + ", " + Status + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }

    public DataTable ReportAcc_AllCustomerWise(string BussID, DateTime FromDate, DateTime ToDate, string Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_AllCustomerWise(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_ALLEmp", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@Status", Status);

            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_AllCustomerWise(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise_ALLEmp" + ex.Message);
            throw ex;
        }
    }

    public DataTable ReportAcc_AllCustomerWise3(string Emp, DateTime FromDate, DateTime ToDate, string Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_AllCustomerWise(" + Emp + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_EmployeeWise_Wise_ALLEmp", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Emp", Emp);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_AllCustomerWise(" + Emp + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise_ALLEmp" + ex.Message);
            throw ex;
        }
    }

    public DataTable ReportAcc_Report3(string empid, DateTime FromDate, DateTime ToDate, int MotherId, string BoId, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + ", " + Status + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Employee", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Emp", empid);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@MotherId", MotherId);
            cmd.Parameters.AddWithValue("@BoId", BoId);
            // cmd.Parameters.AddWithValue("@Status", Status);
            // if (Status == 0)
            // cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            // else
            cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;

            da.Fill(dt); loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + "," + Status + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }

    //firoz
    public DataTable ReportAcc_Report3_Excel(string empid, DateTime FromDate, DateTime ToDate, int MotherId, string BoId, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Employee_Excel", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Emp", empid);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@MotherId", MotherId);
            cmd.Parameters.AddWithValue("@BoId", BoId);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }


    public DataTable ReportAcc_Report3_Acc(string empid, DateTime FromDate, DateTime ToDate, int MotherId, string boid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Employee_Acc", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Emp", empid);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@MotherId", MotherId);
            cmd.Parameters.AddWithValue("@BoId", boid);
            // cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_Report3(" + empid + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }



    public DataTable ReportAcc_Report2_Excel(string BussID, DateTime FromDate, DateTime ToDate, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report2_Excel(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Company_eMP_Excel", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_Report2_Excel(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }
    public DataTable ReportAcc_Report2(string BussID, DateTime FromDate, DateTime ToDate, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report2(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Company_eMP", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);

            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_Report2(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }

    //FIROZ
    public DataTable ReportAcc_Report_PDF(string BussID, DateTime FromDate, DateTime ToDate, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_Report_PDF(" + BussID + "," + FromDate + "," + ToDate + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Company_eMP_PDF", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);

            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_Report_PDF(" + BussID + "," + FromDate + "," + ToDate + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_Report_Customer_Wise" + ex.Message);
            throw ex;
        }
    }
    //----------

    public DataTable ReportAcc_CustomerWise_Emp_Excel(string BussID, DateTime FromDate, DateTime ToDate, int empid, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_Emp_Excel", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", empid);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else if (Status == 1)
                cmd.Parameters.AddWithValue("@Status", "0,1");
            else
                cmd.Parameters.AddWithValue("@Status", Status);

            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("ReportAcc_CustomerWise_Emp" + ex.Message);
            throw ex;
        }
    }

    //firoz
    public DataTable ReportAcc_CustomerWise_Emp_Excels(string BussID, DateTime FromDate, DateTime ToDate, int empid, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_Emp_Excel", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", empid);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else if (Status == 1)
                cmd.Parameters.AddWithValue("@Status", "0,1");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("ReportAcc_CustomerWise_Emp" + ex.Message);
            throw ex;
        }
    }

    public DataTable ReportAcc_CustomerWise_Emp(string BussID, DateTime FromDate, DateTime ToDate, int empid)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_Emp", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", empid);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_CustomerWise_Emp(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("ReportAcc_CustomerWise_Emp" + ex.Message);
            throw ex;
        }
    }

    //firoz03
    public DataTable ReportAcc_CustomerWise_EmpApproves(string BussID, DateTime FromDate, DateTime ToDate, int empid, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise_EmpApproves(" + BussID + "," + FromDate + "," + ToDate + "," + empid + "," + Status + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_EmpApprove", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", empid);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_CustomerWise_EmpApproves(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ", " + Status + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("ReportAcc_CustomerWise_EmpApproves" + ex.Message);
            throw ex;
        }
    }
    public DataTable ReportAcc_CustomerWise_EmpApprove(string BussID, DateTime FromDate, DateTime ToDate, int empid, int Status)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportAcc_CustomerWise_EmpApprove(" + BussID + "," + FromDate + "," + ToDate + "," + empid + "," + Status + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("Sp_Report_Customer_Wise_EmpApprove", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@BussID", BussID);
            cmd.Parameters.AddWithValue("@FromDate", FromDate);
            cmd.Parameters.AddWithValue("@ToDate", ToDate);
            cmd.Parameters.AddWithValue("@empid", empid);
            //cmd.Parameters.AddWithValue("@Status", Status);
            if (Status == 0)
                cmd.Parameters.AddWithValue("@Status", "0,1,2,3,4");
            else
                cmd.Parameters.AddWithValue("@Status", Status);
            da.SelectCommand = cmd;
            da.Fill(dt); loginfo = "ReportAcc_CustomerWise_EmpApprove(" + BussID + "," + FromDate + "," + ToDate + "," + empid + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("ReportAcc_CustomerWise_EmpApprove" + ex.Message);
            throw ex;
        }
    }
    public DataTable ReportBusinessOwner(string EmployeeID)
    {
        DataTable dt = new DataTable();
        SqlDataAdapter da = new SqlDataAdapter();
        try
        {
            loginfo = "ReportBusinessOwner(" + EmployeeID + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("SP_ReportBusinessOwner", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            da.SelectCommand = cmd;
            da.Fill(dt);
            loginfo = "ReportBusinessOwner(" + EmployeeID + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_ReportBusinessOwner" + ex.Message);
            throw ex;
        }
    }

    public void GetDatabaseBackup(String FileLocation, String Database)
    {
        try
        {
            loginfo = "GetDatabaseBackup(" + FileLocation + ", " + Database + ") Start";
            Logger.Info(loginfo);
            getconnection();
            SqlCommand cmd = new SqlCommand("backup database " + Database + " to disk = '" + FileLocation + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".bak'");
            cmd.Connection = con;
            cmd.ExecuteNonQuery();

            loginfo = "GetDatabaseBackup(" + FileLocation + ", " + Database + ") end";
            Logger.Info(loginfo);
        }
        catch (Exception ex)
        {
            Logger.Error("GetDatabaseBackup(" + FileLocation + ", " + Database + ")" + ex.Message);
            throw ex;
        }

    }
    public bool checkIfExists(string TableName, string ColumnName1, Object Value1, string ColumnName2, Object Value2)
    {
        getconnection();
        try
        {

            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(*) from " + TableName + " where " + ColumnName1 + "='" + Value1 + "' and " + ColumnName2 + "='" + Value2 + "'", con);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }

            return (rows != 0) ? true : false;
        }
        catch
        {

            return false;
        }

    }
    public bool checkIfExists_NOT(string TableName, string ColumnName1, Object Value1, string ColumnName2, Object Value2)
    {
        getconnection();
        try
        {

            int rows = 0;
            SqlCommand objCheckCommand = new SqlCommand("select count(*) from " + TableName + " where " + ColumnName1 + "='" + Value1 + "' and " + ColumnName2 + "!='" + Value2 + "'", con);
            SqlDataReader objReader = objCheckCommand.ExecuteReader();
            while (objReader.Read())
            {
                rows = Convert.ToInt32(objReader.GetValue(0).ToString());
            }

            return (rows != 0) ? true : false;
        }
        catch
        {

            return false;
        }

    }

    public void Sp_Report3DropList(DropDownList ddlList, int MotherOrgId)
    {
        ddlList.DataSource = null;
        ddlList.DataBind();
        getconnection();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlCommand cmd = new SqlCommand("Sp_Report3DropList", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@MotherOrgId", MotherOrgId);
        cmd.ExecuteNonQuery();
        da.SelectCommand = cmd;
        ds.Reset();
        da.Fill(ds);
        ddlList.Items.Clear();
        ddlList.Items.Add("Please Select");
        ddlList.SelectedItem.Value = "";

        if (ds.Tables[0].Rows.Count > 0)
        {
            ddlList.DataSource = ds;
            ddlList.DataValueField = ds.Tables[0].Columns[0].ToString();
            ddlList.DataTextField = ds.Tables[0].Columns[1].ToString();
            ddlList.DataBind();
            ddlList.SelectedIndex = 0;
        }
    }
}