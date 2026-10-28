using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;

// 2026-10-28 v27: Help > Tickets / Share suggestion. Null-safe session reads (root cause of the all-roles error page), Modern.master,
// Support Admin goes to the SupportTickets console, failures show a friendly message instead of the error page.
public partial class Raise_Ticket_By_User : System.Web.UI.Page
{
    AddTicketEntityClass AddTicketEntity = new AddTicketEntityClass();
    AddTicketControllerClass AddTicket = new AddTicketControllerClass();
    EmailSettingController emailctrl = new EmailSettingController();
    static readonly string[] Allowed = { ".png", ".jpg", ".jpeg", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

    string S(string key) { return Convert.ToString(Session[key]).Trim(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null) { Response.Redirect("Login.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
        if (AnyminuteSupportAdmin.IsSupportAdminOnly()) { Response.Redirect("SupportTickets.aspx", false); Context.ApplicationInstance.CompleteRequest(); return; }
        if (IsPostBack) return;
        string name = S("NameOfLoggedInUser");
        if (name.Length == 0) { name = S("user"); while (name.StartsWith("Welcome!", StringComparison.OrdinalIgnoreCase)) name = name.Substring(8).Trim(); }
        if (name.Length == 0)
        {
            int me;
            if (Int32.TryParse(S("EmployeeId"), out me))
                try { using (var c = AnyminuteTimesheetRepair.Open()) name = Convert.ToString(AnyminuteTimesheetRepair.Query(c, null, "SELECT dbo.FN_DESC('AddEmployee',@e)", "@e", me).Rows[0][0]); }
                catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("Ticket page name lookup failed", ex); }
        }
        txt_fullname.Text = AnyminuteDisplayText.Tidy(name);
        txt_fullname.ReadOnly = txt_fullname.Text.Length > 0;
        txt_Email.Text = S("email");
        txt_Email.ReadOnly = txt_Email.Text.Length > 0;
    }

    void Show(string text, bool error)
    {
        Notice.Text = HttpUtility.HtmlEncode(text); Notice.CssClass = error ? "am-notice am-error" : "am-notice"; Notice.Visible = true;
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        if (txt_Phone.Text.Replace("-", string.Empty).Trim().Length < 10) { Show("Please enter a 10-digit phone number.", true); return; }
        if (ddl_problemtype.SelectedValue == "" || txt_Describe.Text.Trim().Length == 0 || txt_fullname.Text.Trim().Length == 0 || txt_Email.Text.Trim().Length == 0)
        { Show("Please complete all required fields.", true); return; }
        try
        {
            string attachment = "No_attachment.png";
            if (FileUpload.HasFile)
            {
                string fileName = Path.GetFileName(FileUpload.PostedFile.FileName);
                if (!Allowed.Contains(Path.GetExtension(fileName).ToLowerInvariant())) { Show("Attach a PNG, JPG, PDF, Word or Excel file.", true); return; }
                attachment = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "_" + fileName;
                FileUpload.PostedFile.SaveAs(Server.MapPath("~/images/" + attachment));
            }
            int mother, emp; Int32.TryParse(S("MotherOrgid"), out mother); Int32.TryParse(S("EmployeeId"), out emp);
            AddTicketEntity.MotherOrgId = mother; AddTicketEntity.EmpId = emp;
            AddTicketEntity.Name = txt_fullname.Text.Trim(); AddTicketEntity.EmailId = txt_Email.Text.Trim(); AddTicketEntity.PhoneNO = txt_Phone.Text.Trim();
            AddTicketEntity.ProblemType = ddl_problemtype.SelectedValue; AddTicketEntity.Module = ddl_module.Value;
            AddTicketEntity.Description = txt_Describe.Text.Trim(); AddTicketEntity.FileUploadName = attachment;
            int ticket = AddTicket.InsertTicket(AddTicketEntity);
            using (var c = AnyminuteTimesheetRepair.Open())
                AnyminuteTimesheetRepair.Execute(c, null, @"INSERT INTO Ticket_Comments (TicketId,EmpId,Name,EmailId,PhoneNO,FileUploadName,CommentsBy,Comments,CommentsDate)
                    VALUES (@t,@e,@n,@m,@p,@f,@n,@d,@at)", "@t", ticket, "@e", emp, "@n", AddTicketEntity.Name, "@m", AddTicketEntity.EmailId, "@p", AddTicketEntity.PhoneNO,
                    "@f", attachment, "@d", AddTicketEntity.Description, "@at", DateTime.Now.ToString());
            bool send; Boolean.TryParse(ConfigurationManager.AppSettings["IsSendMail"], out send);
            if (send)
                try { emailctrl.SendTicketMail(AddTicketEntity, txt_Email.Text, ticket.ToString()); emailctrl.SendTicketDetailsToUser(AddTicketEntity, txt_Email.Text, ticket.ToString()); }
                catch (Exception ex) { log4net.LogManager.GetLogger("Log").Warn("Ticket mail failed", ex); }
            ClearTicketDetails();
            Show("Thank you - ticket #" + ticket + " was created. One of our staff members will look into it and get back to you as soon as possible.", false);
        }
        catch (Exception ex)
        {
            log4net.LogManager.GetLogger("Log").Error("Create ticket failed", ex);
            Show("The ticket could not be saved right now. Please try again, or email support@anyminute.net.", true);
        }
    }

    protected void btn_Reset_Click(object sender, EventArgs e) { ClearTicketDetails(); Notice.Visible = false; }

    public void ClearTicketDetails() { txt_Phone.Text = ""; ddl_problemtype.SelectedValue = ""; txt_Describe.Text = ""; }
}
