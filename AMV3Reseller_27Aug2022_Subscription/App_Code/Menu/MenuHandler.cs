using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

/// <summary>
/// This  handler is responsible for handling
/// every actions realted to menu and submenu
/// </summary>
public class MenuHandler
{
    readonly static string conString = ConfigurationManager.ConnectionStrings["ConString"].ToString();
    ILog logger = null;

    public MenuHandler()
    {
        logger = LogManager.GetLogger("Log");
    }

    public SqlConnection OpenConnection()
    {
        var con = new SqlConnection(conString);
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
        return con;
    }

    public void CloseConnection()
    {
        var con = new SqlConnection(conString);
        con.Close();
    }

    public DataTable GetMenuDetails(string userType)
    {
        try
        {
            logger.Info("Fetching menu details - Started");
            using (var con = OpenConnection())
            {
                var cmd = new SqlCommand
                {
                    CommandText = "SP_GET_MENU_DETAILS",
                    CommandType = CommandType.StoredProcedure,
                    Connection = con
                };
                cmd.Parameters.AddWithValue("@UserType", userType);
                using (var sda = new SqlDataAdapter(cmd))
                {
                    using (var dt = new DataTable())
                    {
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error("SP_GET_MENU_DETAILS - " + ex.Message);
            throw ex;
        }
        finally
        {
            logger.Info("Fetching menu details - Finished");
            CloseConnection();
        }
    }

    public IList<MenuViewModel> GetMenusDetails(string userType)
    {
        var dt = GetMenuDetails(userType);
        return ComputeMenuDetails(dt);
    }

    private IList<MenuViewModel> ComputeMenuDetails(DataTable dt)
    {
        return dt.AsEnumerable()
            .Where(x => x.Field<int>("MENUID") == x.Field<int>("PARENTID"))
            .Select(x => new MenuViewModel
            {
                Id = x.Field<int>("ID"),
                Name = x.Field<string>("MENUNAME"),
                Icon = x.Field<string>("ICON"),
                Href = x.Field<string>("HREF"),
                Seq = x.Field<int>("SEQ"),
                SubMenu = ComputeSubMenu(dt, x.Field<int>("MENUID"))
            })
            .ToList();
    }

    private IList<MenuViewModel> ComputeSubMenu(DataTable dtBase, int menuId)
    {
        return dtBase.AsEnumerable()
            .Where(y => y.Field<int>("PARENTID") == menuId &&
                        y.Field<int>("MENUID") != menuId)
            .Select(x => new MenuViewModel
            {
                Id = x.Field<int>("ID"),
                Name = x.Field<string>("MENUNAME"),
                Icon = x.Field<string>("ICON"),
                Href = x.Field<string>("HREF"),
                Seq = x.Field<int>("SEQ"),
            })
            .ToList();
    }
}