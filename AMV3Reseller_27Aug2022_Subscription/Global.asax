<%@ Application Language="C#" %>

<script RunAt="server">

    EmailSettingController Email = new EmailSettingController();
    void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
        log4net.Config.XmlConfigurator.Configure();
    }
    protected void Application_BeginRequest(Object sender, EventArgs e)
    {
        System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        newCulture.DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
        newCulture.DateTimeFormat.DateSeparator = "-";

        System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
    }
    void Application_End(object sender, EventArgs e)
    {
        // Code that runs on application shutdown
    }
    //firoz 08
    //void Application_Error(object sender, EventArgs e)
    //{
    //    // Code that runs when an unhandled error occurs
    //    if (HttpContext.Current != null)
    //    {
    //        var url = HttpContext.Current.Request.Url;
    //        var page = HttpContext.Current.Handler as System.Web.UI.Page;
    //        HttpUnhandledException httpUnhandledException =
    //        new HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError());
    //        Email.SendErrorMail(httpUnhandledException.GetHtmlErrorMessage(), url.ToString(), page.ToString(), Session["email"].ToString());
    //    }
    //}

    // 2026-10-28 v26: end sessions revoked by Support Admin "Disable Login Access" (no-op until REQUIRED_08 is run).
    void Application_PostAcquireRequestState(object sender, EventArgs e)
    {
        AnyminuteSessionGuard.Check(HttpContext.Current);
    }

    void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started  
        if (Session["UserId"] != null && Session["EmployeeId"] != null && Session["UserTypeid"] != null && Session["UserType"] != null && Session["MotherOrgid"] != null)
        {
            //Redirect to Welcome Page if Session is not null  
            //Response.Redirect("Welcome.aspx");
        }
        else
        {
            //Redirect to Login Page if Session is null & Expires   
            // Response.Redirect("Login.aspx");
        }
    }

    void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }

</script>
