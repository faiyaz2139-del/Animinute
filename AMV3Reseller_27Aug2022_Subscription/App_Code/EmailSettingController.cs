using log4net;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web;

/// <summary>
/// Summary description for EmailSettingController
/// </summary>
public class EmailSettingController
{
    SqlConnection con = new SqlConnection();
    SqlDataAdapter da = new SqlDataAdapter();
    CommanClass Comm = new CommanClass();
    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    public EmailSettingController()
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

    public int InsertEmailSetting(EmailSettingEntity EsE)
    {
        int Result = 0;
        try
        {
            loginfo = "InsertEmailSetting(" + EsE + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_InsertEmailSetting";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@SenderEmail", EsE.SenderEmail);
            cmd.Parameters.AddWithValue("@Password", EsE.Password);
            cmd.Parameters.AddWithValue("@HostName", EsE.HostName);
            cmd.Parameters.AddWithValue("@PortNo", EsE.PortNo);
            Result = cmd.ExecuteNonQuery();

            loginfo = "InsertEmailSetting(" + EsE + ") end";
            Logger.Info(loginfo);
            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("sp_InsertEmailSetting" + ex.Message);
            throw ex;
        }
    }

    public int UpdateEmailSetting(EmailSettingEntity EsE)
    {
        int Result = 0;
        try
        {
            loginfo = "UpdateEmailSetting(" + EsE + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "sp_UpdateEmailSetting";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Eid", EsE.EID);
            cmd.Parameters.AddWithValue("@SenderEmail", EsE.SenderEmail);
            cmd.Parameters.AddWithValue("@Password", EsE.Password);
            cmd.Parameters.AddWithValue("@HostName", EsE.HostName);
            cmd.Parameters.AddWithValue("@PortNo", EsE.PortNo);
            Result = cmd.ExecuteNonQuery();

            loginfo = "UpdateEmailSetting(" + EsE + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("sp_UpdateEmailSetting" + ex.Message);
            throw ex;
        }
    }

    public int InsertEmailTemplateMater(EmailSettingEntity EsE)
    {
        int Result = 0;
        try
        {
            loginfo = "InsertEmailTemplateMater(" + EsE + ") Start";
            Logger.Info(loginfo);

            getconnection();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Sp_InsertTemplateMaster";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@TempType", EsE.TempType);
            cmd.Parameters.AddWithValue("@TempName", EsE.TempName);
            cmd.Parameters.AddWithValue("@Subject", EsE.Subject);
            cmd.Parameters.AddWithValue("@description", EsE.description);

            Result = cmd.ExecuteNonQuery();

            loginfo = "InsertEmailTemplateMater(" + EsE + ") end";
            Logger.Info(loginfo);

            return Result;
        }
        catch (Exception ex)
        {
            Logger.Error("Sp_InsertTemplateMaster" + ex.Message);
            throw ex;
        }
    }

    public string PopulateBodyWithUrl(string userName, string title, string url, string description, string TemplateName, string LoginId, string OldPassword)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/" + TemplateName + "")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{Title}", title);
        body = body.Replace("{Url}", url);
        body = body.Replace("{Description}", description);
        body = body.Replace("{LoginId}", LoginId);
        body = body.Replace("{OldPassword}", OldPassword);
        return body;
    }

    public string PopulateBodyNoUrl(string userName, string title, string url, string description, string TemplateName, string EmailOfRegisteredPerson, string NameOfRegisteredPerson)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/" + TemplateName + "")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{Title}", title);
        body = body.Replace("{Description}", description);
        body = body.Replace("{EmailOfRegisteredPerson}", EmailOfRegisteredPerson);
        body = body.Replace("{NameOfRegisteredPerson}", NameOfRegisteredPerson);
        return body;
    }

    public void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
    {
        try
        {
            loginfo = "SendHtmlFormattedEmail(" + recepientEmail + ", " + subject + "" + body + ") Start";
            Logger.Info(loginfo);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //if (dtEmailSetting.Rows.Count > 0)
            //{
            //    using (MailMessage mailMessage = new MailMessage())
            //    {
            //        mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //        mailMessage.Subject = subject;
            //        mailMessage.Body = body;
            //        mailMessage.IsBodyHtml = true;
            //        mailMessage.To.Add(new MailAddress(recepientEmail));
            //        SmtpClient smtp = new SmtpClient();
            //        smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //        smtp.EnableSsl = true;
            //        System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //        NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //        NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //        smtp.UseDefaultCredentials = true;
            //        smtp.Credentials = NetworkCred;
            //        smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //        smtp.Send(mailMessage);
            //    }
            //}

            //create the mail message 
            DataTable dtEmailSetting;
            dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(recepientEmail.Trim());

            //set the content 
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            loginfo = "SendHtmlFormattedEmail(" + recepientEmail + ", " + subject + "" + body + ") END";
            Logger.Info(loginfo);
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            loginfo = "SendHtmlFormattedEmail(" + recepientEmail + ", " + subject + "" + body + ")" + ex.Message;
            Logger.Error(loginfo);

            throw ex;
        }
    }

    public string PopulateBodyForForgetPassword(string userName, string title, string url, string description, string TemplateName, string OldPassword)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/" + TemplateName + "")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        //body = body.Replace("{Title}", title);
        body = body.Replace("{Description}", description);
        body = body.Replace("{OldPassword}", OldPassword);
        return body;
    }
    public void SendForgetPasswordEmail(string recepientEmail, string subject, string oldpassword)
    {
        try
        {
            loginfo = "SendForgetPasswordEmail(" + recepientEmail + ", " + subject + "" + oldpassword + ") Start";
            Logger.Info(loginfo);
            //create the mail message 
            DataTable dtEmailSetting;
            dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(recepientEmail);

            //set the content 
            mail.Subject = subject;
            mail.Body = "You have requested for Forgot Password. Your Password is " + oldpassword;
            //mail.Body = "Hi, <br/><br/>Your password reset request has been completed successfully. <br/>Your  password is " + oldpassword + ". Please login to any minute application with the password. <br/><br/>Thank You<br/>Support Team";
            //mail.IsBodyHtml = true;
            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889 //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            loginfo = "SendForgetPasswordEmail(" + recepientEmail + ", " + subject + "" + oldpassword + ") end";
            Logger.Info(loginfo);
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            loginfo = "SendForgetPasswordEmail(" + recepientEmail + ", " + subject + "" + oldpassword + ") end";
            Logger.Error(loginfo);
            throw ex;
        }
    }

    //firoz 08
    public void SendErrorMail(string result, string url, string page, string user)
    {
        try
        {
            DataTable dtEmailSetting;
            dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(ConfigurationManager.AppSettings["Error1"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content 
            mail.Subject = "Exception Raised With Page " + page + "";
            string aa = "<b>Page Url: " + url + "</b> <br>";
            aa += "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            aa += "<b>User Id: " + user + "</b> <br>";
            mail.IsBodyHtml = true;
            mail.Body = aa + result;
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa+= "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SendTicketMail(AddTicketEntityClass AddTicketEntity, string Loginid, string Tikect)
    {
        try
        {
            DataTable dtEmailSetting;
            dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(ConfigurationManager.AppSettings["Error1"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content 
            mail.Subject = "Ticket Raised With Ticket Id " + Tikect + "";
            string aa = "<b>Login id: " + Loginid + "</b> <br>";
            aa += "<b>Full Name: " + AddTicketEntity.Name + "</b> <br>";
            aa += "<b>Email Address: " + AddTicketEntity.EmailId + "</b> <br>";
            aa += "<b>Phone No: " + AddTicketEntity.PhoneNO + "</b> <br>";
            aa += "<b>Ticket Details: " + AddTicketEntity.BusinessName + "</b> <br>";
            mail.IsBodyHtml = true;
            mail.Body = aa + AddTicketEntity.Description;
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");   

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa+= "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SendBackUpMail()
    {
        try
        {
            DataTable dtEmailSetting;
            dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(ConfigurationManager.AppSettings["Error1"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content 
            mail.Subject = "Database Backup Taken on " + DateTime.Now.ToShortDateString() + "";
            mail.Body = "Database Backup Taken on " + DateTime.Now.ToShortDateString() + " with file Name " + DateTime.Now.ToString("yyyy-MM-dd") + ".bak";
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa += "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string PopulateBodyWithPaymentUrl(string userName, string title, string url, string description, string TemplateName, string paymentURL, string payableamount)
    {
        string body = string.Empty;
        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/" + TemplateName + "")))
        {
            body = reader.ReadToEnd();
        }
        body = body.Replace("{UserName}", userName);
        body = body.Replace("{Title}", title);
        body = body.Replace("{Description}", description);
        body = body.Replace("{PaymentURL}", paymentURL);
        body = body.Replace("{PayableAmount}", payableamount);
        return body;
    }

    public void SendTicketDetailsToUser(AddTicketEntityClass AddTicketEntity, string Loginid, string Tikect)
    {
        try
        {
            DataTable dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(Loginid);
            mail.CC.Add(ConfigurationManager.AppSettings["Error1"]);
            //mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            //mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content
            mail.Subject = "Ticket Raised With Ticket Id " + Tikect + "";
            string aa = "Dear " + AddTicketEntity.Name + ",<br><br>";
            //aa += "Your ticket has been created successfully with ticket id '" + Tikect + "'. You have raised the concern for '" + AddTicketEntity.Description + "'.<br><br>";
            //aa += "Your concern will be rectified shortly.<br><br>";
            aa += "Thank you for creating the ticket, one of our support team will look into the issue. We will send a confirmation via email or updates can be viewed in the view ticket screen.<br><br>";
            aa += "Thanks,<br>";
            aa += "AnyMinute Support Team";
            mail.IsBodyHtml = true;
            mail.Body = aa;
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");   

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa+= "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SendTicketDetailsToUserbySupportAdmin(AddTicketEntityClass AddTicketEntity, string Tikect)
    {
        try
        {
            DataTable dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(AddTicketEntity.EmailId);
            mail.CC.Add(ConfigurationManager.AppSettings["Error1"]);
            //mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            //mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content
            mail.Subject = "Ticket Raised With Ticket Id " + Tikect + "";
            string aa = "Dear " + AddTicketEntity.Name + ",<br><br>";
            //aa += "Your ticket has been created successfully with ticket id '" + Tikect + "'. You have raised the concern for '" + AddTicketEntity.Description + "'.<br><br>";
            //aa += "Your concern will be rectified shortly.<br><br>";
            aa += "Please find the comment from support team below.<br><br>";
            aa += "'" + AddTicketEntity.DeveloperComment + "'<br><br>";
            aa += "Thanks,<br>";
            aa += "AnyMinute Support Team";
            mail.IsBodyHtml = true;
            mail.Body = aa;
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");   

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa+= "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SendTicketDetailsCommentsFromUserToSupportAdmin(AddTicketEntityClass AddTicketEntity, string Tikect)
    {
        try
        {
            DataTable dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");
            MailMessage mail = new MailMessage();

            //set the addresses 
            mail.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());//("support@anyminute.net"); //IMPORTANT: This must be same as your smtp authentication address.
            mail.To.Add(ConfigurationManager.AppSettings["Error1"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            //mail.CC.Add(ConfigurationManager.AppSettings["Error2"]);
            mail.CC.Add(ConfigurationManager.AppSettings["Error3"]);

            //set the content
            mail.Subject = "Ticket Raised With Ticket Id " + Tikect + "";
            string aa = "Dear Support Team,<br><br>";
            //aa += "Your ticket has been created successfully with ticket id '" + Tikect + "'. You have raised the concern for '" + AddTicketEntity.Description + "'.<br><br>";
            //aa += "Your concern will be rectified shortly.<br><br>";
            aa += "Please find the comment from user '" + AddTicketEntity.EmailId + "' below.<br><br>";
            aa += "'" + AddTicketEntity.DeveloperComment + "'<br><br>";
            aa += "Thanks,<br>";
            aa += "AnyMinute Support Team";
            mail.IsBodyHtml = true;
            mail.Body = aa;
            mail.IsBodyHtml = true;

            //send the message 
            SmtpClient smtp = new SmtpClient(dtEmailSetting.Rows[0]["HostName"].ToString());//("mail.anyminute.net");   

            //IMPORANT:  Your smtp login email MUST be same as your FROM address. 
            NetworkCredential Credentials = new NetworkCredential(dtEmailSetting.Rows[0]["SenderEmail"].ToString(), dtEmailSetting.Rows[0]["Password"].ToString());//("support@anyminute.net", "Password@123");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = Credentials;
            smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());//25;    //alternative port number is 8889
            smtp.EnableSsl = true;
            smtp.Send(mail);
            //DataTable dtEmailSetting;
            //dtEmailSetting = Comm.GetAllDataFromTable("EmailSeting", "*", "", "");

            //using (MailMessage mailMessage = new MailMessage())
            //{
            //    mailMessage.From = new MailAddress(dtEmailSetting.Rows[0]["SenderEmail"].ToString());
            //    mailMessage.Subject = "Exception Raised With Page " + page + "";
            //    string aa = "<b>Page Url: " + url + "</b> <br>";
            //    aa+= "<b>Date Time: " + System.DateTime.Now + "</b> <br>";
            //    aa += "<b>User Id: " + user + "</b> <br>";
            //    mailMessage.Body = aa+result;
            //    mailMessage.IsBodyHtml = true;
            //    mailMessage.To.Add(new MailAddress("amit@mfksoftware.com"));
            //    SmtpClient smtp = new SmtpClient();
            //    smtp.Host = dtEmailSetting.Rows[0]["HostName"].ToString();
            //    smtp.EnableSsl = true;
            //    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
            //    NetworkCred.UserName = dtEmailSetting.Rows[0]["SenderEmail"].ToString();
            //    NetworkCred.Password = dtEmailSetting.Rows[0]["Password"].ToString();
            //    smtp.UseDefaultCredentials = false;
            //    smtp.Credentials = NetworkCred;
            //    smtp.Port = int.Parse(dtEmailSetting.Rows[0]["PortNo"].ToString());
            //    smtp.Send(mailMessage);
            //}
            //lblMessage.Text = "Mail Sent";
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}