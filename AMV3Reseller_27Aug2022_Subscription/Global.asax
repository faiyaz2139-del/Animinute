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

    // 2026-10-31 v38 (DOCX: subscriber "Login & Access" -> ErrorPages/index.aspx?aspxerrorpath=/LoginAccess.aspx even with
    // the page's own Page_Error): failures that happen before page code runs (compile / parse / type load) are logged in full
    // (compiler errors included) and the user gets an in-app notice instead of the generic error page. Login & Access only.
    void Application_Error(object sender, EventArgs e)
    {
        try
        {
            HttpContext h = HttpContext.Current; if (h == null) return;
            string page = System.IO.Path.GetFileName(h.Request.Path ?? "");
            if (!page.Equals("UserLoginAccess.aspx", StringComparison.OrdinalIgnoreCase) && !page.Equals("LoginAccess.aspx", StringComparison.OrdinalIgnoreCase)) return;
            Exception ex = Server.GetLastError(); if (ex == null || ex is System.Threading.ThreadAbortException) return;
            string detail = ex.ToString();
            HttpCompileException ce = ex as HttpCompileException;
            if (ce == null && ex.InnerException != null) ce = ex.InnerException as HttpCompileException;
            if (ce != null && ce.Results != null) foreach (System.CodeDom.Compiler.CompilerError err in ce.Results.Errors) detail += Environment.NewLine + err.ToString();
            log4net.LogManager.GetLogger("Log").Error("Login & Access request failed before page code: " + detail);
            Server.ClearError();
            h.Response.Clear(); h.Response.StatusCode = 200; h.Response.ContentType = "text/html";
            h.Response.Write("<!DOCTYPE html><html><head><meta charset=\"utf-8\"><title>Login &amp; Access</title><link href=\"css/AnyminuteModern.css\" rel=\"stylesheet\" /></head><body class=\"am-modern\" style=\"padding:40px\"><div class=\"am-card\" style=\"max-width:720px;margin:0 auto\"><h1 style=\"font-size:22px;margin:0 0 10px\">Login &amp; Access</h1><div class=\"am-notice am-error\" role=\"alert\">Login &amp; Access could not be opened. The details were logged for support. You can still manage an employee's login from Employees &gt; Employee List &gt; Edit &gt; Login &amp; Access.</div><p><a class=\"am-button\" href=\"Users.aspx\">Employee List</a> <a class=\"am-button secondary\" href=\"Dashboard.aspx\">Back to Dashboard</a></p></div></body></html>");
            h.ApplicationInstance.CompleteRequest();
        }
        catch (Exception) { }
    }

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
