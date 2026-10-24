using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

/// <summary>
/// Summary description for CreateLinkClass
/// </summary>
public class CreateLinkClass
{
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public CreateLinkClass()
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
    public DataTable GetParentLink(string domainid)
    {
        DataTable dt = new DataTable();
        try
        {
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter("select Title,MenuID from LinkMaster where DomainId=" + domainid + "", con);
            da.Fill(dt);

            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int InsertLinkClass(int menuid, int ParentMenuID, string Title, string Url, string Icon, string flag, decimal menuSlno, int DomainId, int mastno, string LinkFor)
    {
        int result;
        try
        {
            loginfo = "UpdateDomainLink(" + menuid + ", " + ParentMenuID + ", " + Title + ", " + Url + ", " + Icon + ", " + flag + ", " + menuSlno + ", " + DomainId + ", " + mastno + ", " + LinkFor + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("SP_InsertLinkCreation", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@ParentMenuID", ParentMenuID);
            cmd.Parameters.AddWithValue("@Title", Title);
            cmd.Parameters.AddWithValue("@Url", Url);
            cmd.Parameters.AddWithValue("@Icon", Icon);
            cmd.Parameters.AddWithValue("@flag", flag);
            cmd.Parameters.AddWithValue("@menuid", menuid);
            cmd.Parameters.AddWithValue("@menuSlno", menuSlno);
            cmd.Parameters.AddWithValue("@DomainId", DomainId);
            cmd.Parameters.AddWithValue("@MastNo", mastno);
            cmd.Parameters.AddWithValue("@LinkFor", LinkFor);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateDomainLink(" + menuid + ", " + ParentMenuID + ", " + Title + ", " + Url + ", " + Icon + ", " + flag + ", " + menuSlno + ", " + DomainId + ", " + mastno + ", " + LinkFor + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_InsertLinkCreation" + ex.Message);
            throw ex;
        }
    }
    public DataTable GetAllLink()
    {
        DataTable dt = new DataTable();
        string query;
        try
        {
            getconnection();
            query = "select MenuID,ParentMenuID, Title ,url,icon,dm.DomainName , dm.id , mastno ,";
            query += "case when Url IS null then 'Parent Menu' else 'Child Menu' end Link_Type , menuslno from LinkMaster lm inner join Role_DomainMaster dm on dm.ID=lm.DomainId";
            SqlDataAdapter da = new SqlDataAdapter(query, con);
            da.Fill(dt);

            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int DeleteLink(int MenuID)
    {
        int result;
        try
        {
            getconnection();
            SqlCommand cmd = new SqlCommand("delete from LinkMaster where MenuID=" + MenuID + "", con);
            result = cmd.ExecuteNonQuery();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public SqlDataReader GetLinkForTree()
    {
        SqlDataReader dr;
        try
        {
            getconnection();
            string query = "SELECT DM.DomainID as id,LM.MenuID,LM.Title+'---'+ LM.LinkFor AS Title,LM.Url,DM.DomainName,LM.MASTNO,LM.MenuSlno,DM.OrderID";
            query += " FROM LinkMaster LM, DomainMaster DM WHERE DM.DomainID=LM.DomainId ORDER BY DM.OrderID,MenuSlno";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            return dr;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public SqlDataReader GetLinkForTreeAdmin(string link)
    {
        SqlDataReader dr;
        try
        {
            getconnection();
            string query = "SELECT DM.ID,LM.MenuID,LM.Title,LM.Url,DM.DomainName,LM.MASTNO,LM.MenuSlno,DM.OrderID";
            query += " FROM LinkMaster LM, Role_DomainMaster DM WHERE DM.ID=LM.DomainId and LM.MenuID in (select value from fn_Split('" + link + "',',')) ORDER BY DM.OrderID,MenuSlno";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            return dr;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable GetUserFromUserType(string UserType)
    {
        DataTable dt = new DataTable();
        try
        {
            getconnection();
            SqlDataAdapter da = new SqlDataAdapter("select Registration_No,Name,Username,Contact_No from Login where User_Type='" + UserType + "'", con);
            da.Fill(dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public int InsertAssigneLink(string INSD_ld, string Links, string UserType)
    {
        int result;
        try
        {
            getconnection();
            //SqlCommand cmd = new SqlCommand("Update login set User_ACC='"+ Links + "' where INSD_ld="+ INSD_ld + " and User_Type='ADMIN'",con);
            SqlCommand cmd = new SqlCommand("insert into Role_AssginedLink (INSD_ld,User_Type,AssginedLink)values('" + INSD_ld + "','" + UserType + "','" + Links + "')", con);
            cmd.CommandType = CommandType.Text;
            result = cmd.ExecuteNonQuery();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    public int UpdateAssigneLink(string INSD_ld, string Links, string UserType)
    {
        int result;
        try
        {
            getconnection();
            SqlCommand cmd = new SqlCommand("Update Role_AssginedLink set AssginedLink='" + Links + "' where INSD_ld=" + INSD_ld + " and User_Type='" + UserType + "'", con);

            cmd.CommandType = CommandType.Text;
            result = cmd.ExecuteNonQuery();
            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public int InsertDomainLink(string DomainName, int OrderID, String DomainFor, string icon)
    {
        int result = 0;
        try
        {
            loginfo = "UpdateDomainLink(" + DomainName + ", " + OrderID + ", " + DomainFor + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("Role_InsertDomain", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DomainName", DomainName);
            cmd.Parameters.AddWithValue("@OrderID", OrderID);
            cmd.Parameters.AddWithValue("@DomainFor", DomainFor);
            cmd.Parameters.AddWithValue("@icon", icon);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateDomainLink(" + DomainName + ", " + OrderID + ", " + DomainFor + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Role_InsertDomain" + ex.Message);
            throw ex;
        }
    }

    public int UpdateDomainLink(string DomainName, int OrderID, int DomainID, string DomainFor, string icon)
    {
        int result = 0;
        try
        {
            loginfo = "UpdateDomainLink(" + DomainName + ", " + OrderID + ", " + DomainID + ", " + DomainFor + ", " + icon + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("Role_UpdateDomain", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@DomainName", DomainName);
            cmd.Parameters.AddWithValue("@OrderID", OrderID);
            cmd.Parameters.AddWithValue("@DomainID", DomainID);
            cmd.Parameters.AddWithValue("@DomainFor", DomainFor);
            cmd.Parameters.AddWithValue("@icon", icon);
            result = cmd.ExecuteNonQuery();

            loginfo = "UpdateDomainLink(" + DomainName + ", " + OrderID + ", " + DomainID + ", " + DomainFor + ", " + icon + ") end";
            Logger.Info(loginfo);

            return result;
        }
        catch (Exception ex)
        {
            Logger.Error("Role_UpdateDomain" + ex.Message);
            throw ex;
        }
    }

    public string GetSelectedLinks(System.Web.UI.WebControls.TreeView tv)
    {
        string links = "0";
        //Get the Selected Links
        for (int i = 0; i < tv.Nodes.Count; i++)
        {
            for (int j = 0; j < tv.Nodes[i].ChildNodes.Count; j++)
            {
                if (tv.Nodes[i].ChildNodes[j].Checked == true)
                    links += "," + tv.Nodes[i].ChildNodes[j].Value;

                if (tv.Nodes[i].ChildNodes[j].ChildNodes.Count > 0)
                {
                    for (int k = 0; k < tv.Nodes[i].ChildNodes[j].ChildNodes.Count; k++)
                    {
                        if (tv.Nodes[i].ChildNodes[j].ChildNodes[k].Checked == true)
                            links += "," + tv.Nodes[i].ChildNodes[j].ChildNodes[k].Value;
                    }
                }
            }
        }
        return links;
    }

    public SqlDataReader GetTreeLinkForUser(string linkAdmin, string regno)
    {
        SqlDataReader dr;
        try
        {
            loginfo = "GetTreeLinkForUser(" + linkAdmin + ", " + regno + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("SP_GetUserLink", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@LinkAdmin", linkAdmin);
            cmd.Parameters.AddWithValue("@Regno", regno);
            dr = cmd.ExecuteReader();

            loginfo = "GetTreeLinkForUser(" + linkAdmin + ", " + regno + ") end";
            Logger.Info(loginfo);
            return dr;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_GetUserLink" + ex.Message);
            throw ex;
        }
    }

    public int SaveUserLink(string link, string regno, int MotherOrgid)
    {
        int result = 0;
        try
        {
            getconnection();
            SqlCommand cmd = new SqlCommand("Update UserAcc set LinkAcc='" + link + "'where UserType='" + regno + "' and MotherOrgid=" + MotherOrgid + "", con);
            result = cmd.ExecuteNonQuery();

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetMenuData(int INSD_ld, string User_Type)
    {
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetMenuData(" + INSD_ld + ", " + User_Type + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("SP_GetUserLinkForMenu", con); //we have procedure

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@INSD_ld", INSD_ld);
            cmd.Parameters.AddWithValue("@User_Type", User_Type);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetMenuData(" + INSD_ld + ", " + User_Type + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_GetUserLinkForMenu" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetMenuDataFromLoginTable(string Regno)
    {

        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetMenuDataFromLoginTable(" + Regno + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("SP_GetUserLinkForMenuLoginTable", con); //we have procedure

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", Regno);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetMenuDataFromLoginTable(" + Regno + ") end";
            Logger.Info(loginfo);
            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("SP_GetUserLinkForMenuLoginTable" + ex.Message);
            throw ex;
        }
    }


    public SqlDataReader GetLinkForAdminTree(int MotherOrgid)
    {
        SqlDataReader dr; ;
        try
        {
            loginfo = "GetLinkForAdminTree(" + MotherOrgid + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("GetLinkForAdminTree", con); //we have procedure

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@MotherOrgid", MotherOrgid);
            dr = cmd.ExecuteReader();

            loginfo = "GetLinkForAdminTree(" + MotherOrgid + ") end";
            Logger.Info(loginfo);

            return dr;
        }
        catch (Exception ex)
        {
            Logger.Error("GetLinkForAdminTree" + ex.Message);
            throw ex;
        }
    }

    public DataTable GetDomainNameForMenu(int INSD_ld, string User_Type)
    {
        DataTable dt = new DataTable();
        try
        {
            loginfo = "GetDomainNameForMenu(" + INSD_ld + ", " + User_Type + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("GetDomainNameForMenu", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@INSD_ld", INSD_ld);
            cmd.Parameters.AddWithValue("@User_Type", User_Type);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "GetDomainNameForMenu(" + INSD_ld + ", " + User_Type + ") end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("GetDomainNameForMenu" + ex.Message);
            throw ex;
        }
    }


    public DataTable GetDomainNameForMenuFromLogin(string regno)
    {
        DataTable dt1 = new DataTable();
        try
        {
            loginfo = "GetDomainNameForMenuFromLogin(" + regno + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand("GetDomainNameForMenuLoginTable", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@UserId", regno);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt1);

            loginfo = "GetDomainNameForMenuFromLogin(" + regno + ") end";
            Logger.Info(loginfo);

            return dt1;
        }
        catch (Exception ex)
        {
            Logger.Error("GetDomainNameForMenuLoginTable" + ex.Message);
            throw ex;
        }
    }
    public void UpdateEmpLink(int empid, string link)
    {
        try
        {
            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Update UserAcc set LinkAcc='" + link + "' where EmployeeId=" + empid + "";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}