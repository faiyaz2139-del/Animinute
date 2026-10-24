using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ErrorPages_index : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lnkhome_Click(object sender, EventArgs e)
    {
        if(Session["UserType"]!=null)
        {
            if (Session["UserType"].ToString() == "SuperAdmin")
            {
                Response.Redirect("../SuperAdmin.aspx", false);
            }

            else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() == "support@anyminute.net")
            {
                Response.Redirect("../SuperAdmin_Approve_Subscriber.aspx", false);
            }

            else if (Session["UserType"].ToString() == "SupportAdmin" && Session["email"].ToString() != "support@anyminute.net")
            {
                Response.Redirect("../SuperAdmin.aspx", false);
            }

            else if (Session["UserType"].ToString() == "Admin")
            {
                Response.Redirect("../Accountant.aspx", false);
            }

            else
            {
                Response.Redirect("../Home_Page.aspx", false);
            }
        }
        else
        {
            Response.Redirect("../Login.aspx", false);
        }
    }
}