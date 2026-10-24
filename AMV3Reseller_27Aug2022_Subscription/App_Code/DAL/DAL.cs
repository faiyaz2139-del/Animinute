using System.Configuration;


/// <summary>
/// Summary description for DAL
/// </summary>
public class DAL
{

    string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();




    public DAL()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void PopulateData()
    {




    }



}