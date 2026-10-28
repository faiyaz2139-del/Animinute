using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-28 v25 deck 2 (Support Tickets): KPI cards, filters, ticket table and the Ticket Actions menu
// (View Details, Edit, Assign/Reassign, Reply, Change Status, Mark as Resolved, View Activity History, Archive).
// Uses the existing Ticket / Ticket_Comments tables and legacy StatusType codes (Ticket_List.aspx keeps working);
// subject/category/priority/assignee/archive live in AnyminuteTicketMeta and every action is written to
// AnyminuteSupportAudit (Ticket Activity History). Both tables come from REQUIRED_07.
public partial class AnyminuteSupportTicketsPage : System.Web.UI.Page
{
    const int PageSize = 10;
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    static string Subject(DataRow r)
    {
        string s = SA.S(r["Subject"]); if (s.Length > 0) return s;
        s = SA.S(r["Description"]).Replace("\r", " ").Replace("\n", " "); return s.Length > 48 ? s.Substring(0, 45) + "..." : (s.Length == 0 ? "(no subject)" : s);
    }
    static string Category(DataRow r) { string s = SA.S(r["Category"]); return s.Length > 0 ? s : SA.LegacyCategory(r["ProblemType"], r["Module"]); }
    static string Priority(DataRow r) { string s = SA.S(r["Priority"]); if (s.Length == 0) s = SA.S(r["TicketSeverity"]); return SA.Priorities.FirstOrDefault(p => p.Equals(s, StringComparison.OrdinalIgnoreCase)) ?? "Medium"; }
    static DateTime? Updated(DataRow r)
    {
        DateTime? best = null;
        if (r["UpdatedAtUTC"] != DBNull.Value) best = DateTime.SpecifyKind(Convert.ToDateTime(r["UpdatedAtUTC"]), DateTimeKind.Utc).ToLocalTime();
        if (r["LastComment"] != DBNull.Value) { var d = Convert.ToDateTime(r["LastComment"]); if (best == null || d > best) best = d; }
        return best;
    }
    static string Local(DateTime? d) { return d == null ? "&mdash;" : d.Value.ToString("MMM d, yyyy", CultureInfo.InvariantCulture) + "<br /><small>" + d.Value.ToString("h:mm tt", CultureInfo.InvariantCulture) + "</small>"; }
    DataTable Load(SqlConnection c, bool ready, int id)
    {
        string meta = ready ? "m.Subject,m.Category,m.Priority,m.AssignedTo,ISNULL(m.Archived,0) Archived,m.UpdatedAtUTC" : "CAST(NULL AS nvarchar(200)) Subject,CAST(NULL AS nvarchar(60)) Category,CAST(NULL AS varchar(10)) Priority,CAST(NULL AS nvarchar(150)) AssignedTo,CAST(0 AS bit) Archived,CAST(NULL AS datetime2) UpdatedAtUTC";
        return SA.Q(c, "SELECT t.TicketId,t.TicketNo,t.Description,t.Name,t.EmailId,t.PhoneNO,t.BusinessName,t.ProblemType,t.Module,t.TicketSeverity,t.StatusType,t.TicketRaiseDate,t.TicketResolveDate,t.DeveloperComment,t.FileUploadName," + meta +
            ",(SELECT MIN(k.CommentsDate) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) FirstComment,(SELECT MAX(k.CommentsDate) FROM Ticket_Comments k WHERE k.TicketId=t.TicketId) LastComment FROM Ticket t" +
            (ready ? " LEFT JOIN AnyminuteTicketMeta m ON m.TicketId=t.TicketId" : "") + (id > 0 ? " WHERE t.TicketId=@i" : "") + " ORDER BY t.TicketId DESC", "@i", id);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack && SA.Form("sa_op").Length > 0) { Act(); return; }
        string q = SA.Get("q"), status = SA.Get("status"), prio = SA.Get("priority"), cat = SA.Get("category"), from = SA.Get("from"), to = SA.Get("to"), panel = SA.Get("panel");
        bool archived = SA.Get("archived") == "1";
        int page; if (!Int32.TryParse(SA.Get("page"), out page) || page < 1) page = 1;
        int id; Int32.TryParse(SA.Get("id"), out id);
        DateTime f, t; bool hasF = DateTime.TryParseExact(from, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out f), hasT = DateTime.TryParseExact(to, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
        Flash.Text = SA.TakeFlash();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteTicketMeta") && SA.Has(c, "AnyminuteSupportAudit");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteTicketMeta", "AnyminuteSupportAudit");
            var all = Load(c, ready, 0).Rows.Cast<DataRow>().ToList();
            var live = all.Where(r => !Convert.ToBoolean(r["Archived"])).ToList();
            Func<string, string, string, int, string> kpi = (icon, label, tone, n) => "<div class='sa-kpi'><i class='sa-kpi-icon sa-" + tone + "' aria-hidden='true'>" + icon + "</i><div><span>" + label + "</span><strong>" + n + "</strong></div></div>";
            Kpis.Text = kpi("&#9742;", "Total Tickets", "blue", live.Count) + kpi("&#10003;", "Open Tickets", "green", live.Count(r => SA.StatusLabel(r["StatusType"]) == "Open"))
                + kpi("&#9719;", "In Progress", "amber", live.Count(r => SA.StatusLabel(r["StatusType"]) == "In Progress")) + kpi("&#10004;", "Resolved", "purple", live.Count(r => SA.StatusLabel(r["StatusType"]) == "Resolved"));
            var rows = (archived ? all.Where(r => Convert.ToBoolean(r["Archived"])) : live.AsEnumerable()).Where(r =>
            {
                if (status.Length > 0 && SA.StatusLabel(r["StatusType"]) != status) return false;
                if (prio.Length > 0 && Priority(r) != prio) return false;
                if (cat.Length > 0 && Category(r) != cat) return false;
                if (q.Length > 0 && !(SA.S(r["TicketNo"]) + " " + Subject(r) + " " + SA.S(r["Name"]) + " " + SA.S(r["EmailId"]) + " " + SA.S(r["Description"])).ToLowerInvariant().Contains(q.ToLowerInvariant())) return false;
                if (r["TicketRaiseDate"] != DBNull.Value) { var d = Convert.ToDateTime(r["TicketRaiseDate"]).Date; if (hasF && d < f) return false; if (hasT && d > t) return false; }
                return true;
            }).ToList();
            Filters.Text = "<div class='sa-filters' data-sa-get='SupportTickets.aspx'>"
                + "<label class='sa-grow'><span class='sr-only'>Search</span><input type='search' name='q' value='" + SA.A(q) + "' placeholder='Search by ticket number, subject, subscriber name or email...' /></label>"
                + "<label><span>Status</span><select name='status'>" + SA.Options(SA.TicketStatuses, status, "All Statuses") + "</select></label>"
                + "<label><span>Priority</span><select name='priority'>" + SA.Options(SA.Priorities, prio, "All Priorities") + "</select></label>"
                + "<label><span>Category</span><select name='category'>" + SA.Options(SA.Categories, cat, "All Categories") + "</select></label>"
                + "<label class='sa-range'><span>Date Range</span><span><input type='date' name='from' value='" + SA.A(from) + "' aria-label='From' /> &ndash; <input type='date' name='to' value='" + SA.A(to) + "' aria-label='To' /></span></label>"
                + "<label class='sa-check'><input type='checkbox' name='archived' value='1'" + (archived ? " checked" : "") + " /> Archived</label>"
                + "<button type='button' class='sa-btn' data-sa-apply='1'>&#128269; Search</button><a class='sa-btn sa-btn-outline' href='SupportTickets.aspx'>&#8635; Clear</a></div>";
            string baseQ = "q=" + HttpUtility.UrlEncode(q) + "&status=" + HttpUtility.UrlEncode(status) + "&priority=" + prio + "&category=" + HttpUtility.UrlEncode(cat) + "&from=" + from + "&to=" + to + (archived ? "&archived=1" : "");
            string here = baseQ + "&page=" + page;
            var sb = new StringBuilder("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th><input type='checkbox' aria-label='Select all' data-sa-all='1' /></th><th>Ticket No.</th><th>Subject</th><th>Subscriber</th><th>Category</th><th>Priority</th><th>Status</th><th>Created Date</th><th>Last Updated</th><th>Assigned To</th><th>Actions</th></tr></thead><tbody>");
            foreach (var r in rows.Skip((page - 1) * PageSize).Take(PageSize))
            {
                int tid = Convert.ToInt32(r["TicketId"]); string st = SA.StatusLabel(r["StatusType"]), pr = Priority(r);
                string no = SA.S(r["TicketNo"]).Length > 0 ? SA.S(r["TicketNo"]) : "TK-" + tid;
                DateTime? created = r["FirstComment"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["FirstComment"]) : r["TicketRaiseDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(r["TicketRaiseDate"]) : null;
                Func<string, string> link = p => "SupportTickets.aspx?" + SA.A(here) + "&amp;id=" + tid + "&amp;panel=" + p;
                sb.Append("<tr").Append(tid == id ? " class='sa-selected'" : "").Append("><td><input type='checkbox' aria-label='Select' /></td><td><a href='").Append(link("details")).Append("' class='sa-link'>#").Append(SA.H(no.TrimStart('#'))).Append("</a></td><td>").Append(SA.H(Subject(r)))
                  .Append("</td><td><strong>").Append(SA.H(r["Name"])).Append("</strong><br /><small>").Append(SA.H(r["EmailId"])).Append("</small></td><td>").Append(SA.H(Category(r)))
                  .Append("</td><td>").Append(SA.Pill(pr, pr == "High" ? "red" : pr == "Medium" ? "amber" : "green")).Append("</td><td>").Append(SA.Pill(st, SA.StatusTone(st)))
                  .Append("</td><td>").Append(Local(created)).Append("</td><td>").Append(Local(Updated(r) ?? created)).Append("</td><td>").Append(SA.S(r["AssignedTo"]).Length > 0 ? SA.H(r["AssignedTo"]) : "<span class='sa-muted'>Unassigned</span>")
                  .Append("</td><td><details class='sa-menu'><summary aria-label='Ticket actions'>&#8942;</summary><div class='sa-menu-pop'><h4>Ticket Actions</h4><small>Ticket #").Append(SA.H(no.TrimStart('#'))).Append(" &middot; ").Append(st).Append("</small>")
                  .Append(MenuItem(link("details"), "&#128065;", "View Ticket Details", "Read the issue, attachments and conversations"))
                  .Append(MenuItem(link("edit"), "&#9998;", "Edit Ticket", "Change category, priority or description"))
                  .Append(MenuItem(link("assign"), "&#128100;", "Assign / Reassign", "Assign a Support Admin to handle the issue"))
                  .Append(MenuItem(link("reply"), "&#128172;", "Reply to Subscriber", "Send a response or request more information"))
                  .Append(MenuItem(link("status"), "&#9719;", "Change Ticket Status", "Open, In Progress, Waiting or Resolved"))
                  .Append(MenuItem(link("resolve"), "&#10003;", "Mark as Resolved", "Close the issue with a resolution note"))
                  .Append(MenuItem(link("history"), "&#8634;", "View Activity History", "See all ticket changes and actions"))
                  .Append("<hr /><button type='submit' name='sa_op' value='archive' formnovalidate='formnovalidate' class='sa-menu-item' data-sa-set='sa_id=").Append(tid).Append("' data-sa-confirm='Archive this ticket? Its history is kept.'><i aria-hidden='true'>&#128451;</i><span><b>").Append(Convert.ToBoolean(r["Archived"]) ? "Unarchive Ticket" : "Archive Ticket").Append("</b><small>Archive resolved tickets without deleting history</small></span></button></div></details></td></tr>");
            }
            if (rows.Count == 0) sb.Append("<tr><td colspan='11' class='sa-empty'>No tickets match these filters.</td></tr>");
            Table.Text = "<input type='hidden' name='sa_id' value='" + id + "' data-sa-id='1' />" + sb.Append("</tbody></table></div>").ToString() + Pager(rows.Count, page, baseQ);
            if (id > 0)
            {
                var one = Load(c, ready, id);
                if (one.Rows.Count == 1) Panel.Text = TicketPanel(c, one.Rows[0], panel, ready, here);
            }
        }
    }
    static string Pager(int total, int page, string baseQ)
    {
        return SA.Pager(total, page, PageSize, baseQ).Replace("<span>Showing", "<span>Showing").Replace(" of " + total + "</span>", " of " + total + " tickets</span>");
    }
    static string MenuItem(string href, string icon, string title, string text) { return "<a class='sa-menu-item' href='" + href + "'><i aria-hidden='true'>" + icon + "</i><span><b>" + title + "</b><small>" + text + "</small></span></a>"; }
    string TicketPanel(SqlConnection c, DataRow r, string panel, bool ready, string back)
    {
        int tid = Convert.ToInt32(r["TicketId"]); string st = SA.StatusLabel(r["StatusType"]); string no = SA.S(r["TicketNo"]).Length > 0 ? SA.S(r["TicketNo"]).TrimStart('#') : "TK-" + tid;
        string dis = ready ? "" : " disabled";
        var titles = new Dictionary<string, string> { { "details", "Ticket Details" }, { "edit", "Edit Ticket" }, { "assign", "Assign / Reassign" }, { "reply", "Reply to Subscriber" }, { "status", "Change Ticket Status" }, { "resolve", "Mark as Resolved" }, { "history", "Ticket Activity History" } };
        if (!titles.ContainsKey(panel)) panel = "details";
        var sb = new StringBuilder("<aside class='sa-card sa-panel' aria-label='" + titles[panel] + "'><div class='sa-panel-head'><div><h2>" + titles[panel] + "</h2><small>Ticket #" + SA.H(no) + " &middot; " + st + "</small></div><a class='sa-x' aria-label='Close' href='SupportTickets.aspx?" + SA.A(back) + "'>&times;</a></div>");
        sb.Append("<input type='hidden' name='sa_tid' value='").Append(tid).Append("' />");
        sb.Append("<nav class='sa-subtabs'>");
        foreach (var k in titles.Keys) sb.Append("<a href='SupportTickets.aspx?").Append(SA.A(back)).Append("&amp;id=").Append(tid).Append("&amp;panel=").Append(k).Append("'").Append(k == panel ? " class='on'" : "").Append(">").Append(titles[k].Replace("Ticket ", "").Replace(" to Subscriber", "")).Append("</a>");
        sb.Append("</nav>");
        Func<string, string, string> field = (label, html) => "<label class='sa-field'><span>" + label + "</span>" + html + "</label>";
        if (panel == "details")
        {
            sb.Append("<dl class='sa-dl'><dt>Subscriber</dt><dd>").Append(SA.H(r["Name"])).Append("<br /><small>").Append(SA.H(r["EmailId"])).Append(" ").Append(SA.H(r["PhoneNO"])).Append("</small></dd><dt>Business</dt><dd>").Append(SA.H(r["BusinessName"]))
              .Append("</dd><dt>Category</dt><dd>").Append(SA.H(Category(r))).Append("</dd><dt>Priority</dt><dd>").Append(SA.H(Priority(r))).Append("</dd><dt>Assigned To</dt><dd>").Append(SA.H(r["AssignedTo"]))
              .Append("</dd><dt>Created</dt><dd>").Append(SA.Day(r["TicketRaiseDate"])).Append("</dd><dt>Resolved</dt><dd>").Append(SA.Day(r["TicketResolveDate"])).Append("</dd></dl><h3>Issue</h3><p class='sa-pre'>").Append(SA.H(r["Description"])).Append("</p>");
            if (SA.S(r["FileUploadName"]).Length > 0) sb.Append("<p><a class='sa-link' target='_blank' rel='noopener' href='Uploads/").Append(SA.A(r["FileUploadName"])).Append("'>&#128206; ").Append(SA.H(r["FileUploadName"])).Append("</a></p>");
            sb.Append("<h3>Conversation</h3><ul class='sa-thread'>");
            foreach (DataRow k in SA.Q(c, "SELECT CommentsBy,Comments,CommentsDate FROM Ticket_Comments WHERE TicketId=@i ORDER BY CommentsDate,TicketCommentId", "@i", tid).Rows)
                sb.Append("<li><b>").Append(SA.H(k["CommentsBy"])).Append("</b> <small>").Append(k["CommentsDate"] == DBNull.Value ? "" : Convert.ToDateTime(k["CommentsDate"]).ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture)).Append("</small><p class='sa-pre'>").Append(SA.H(k["Comments"])).Append("</p></li>");
            sb.Append("</ul>");
        }
        else if (panel == "edit")
            sb.Append(field("Subject", "<input name='sa_subject' maxlength='200' value='" + SA.A(Subject(r)) + "'" + dis + " />"))
              .Append("<div class='sa-row2'>").Append(field("Category", "<select name='sa_category'" + dis + ">" + SA.Options(SA.Categories, Category(r), null) + "</select>")).Append(field("Priority", "<select name='sa_priority'" + dis + ">" + SA.Options(SA.Priorities, Priority(r), null) + "</select>")).Append("</div>")
              .Append(field("Description", "<textarea name='sa_desc' rows='6'" + dis + ">" + SA.H(r["Description"]) + "</textarea>"))
              .Append(field("Comments (reason)", "<textarea name='sa_comment' rows='2' maxlength='1000'" + dis + "></textarea>")).Append(Foot(back, "edit", "Save Changes", dis));
        else if (panel == "assign")
        {
            var admins = SA.Q(c, "SELECT LTRIM(RTRIM(ISNULL(FirstName,'')+' '+ISNULL(LastName,''))) N FROM SupportAdmin WHERE ISNULL(Status,'') NOT IN('0','Inactive','False') ORDER BY 1").Rows.Cast<DataRow>().Select(x => SA.S(x[0])).Where(x => x.Length > 0).Distinct().ToList();
            if (!admins.Contains(SA.ActorName())) admins.Insert(0, SA.ActorName());
            sb.Append(field("Assign To", "<select name='sa_assignee'" + dis + ">" + SA.Options(admins, SA.S(r["AssignedTo"]), "Unassigned") + "</select>"))
              .Append(field("Comments", "<textarea name='sa_comment' rows='3' maxlength='1000'" + dis + "></textarea>")).Append(Foot(back, "assign", "Save Assignment", dis));
        }
        else if (panel == "reply")
            sb.Append("<p class='sa-muted'>Adds a reply to the ticket conversation for ").Append(SA.H(r["Name"])).Append(" (").Append(SA.H(r["EmailId"])).Append(").</p>")
              .Append(field("Message *", "<textarea name='sa_message' rows='6' required" + dis + "></textarea>"))
              .Append("<label class='sa-check'><input type='checkbox' name='sa_waiting' value='1'" + dis + " /> Waiting on subscriber (set status to Waiting)</label>").Append(Foot(back, "reply", "Send Reply", dis));
        else if (panel == "status")
            sb.Append("<div class='sa-row2'>").Append(field("Previous Status", "<input value='" + st + "' disabled />")).Append(field("New Status", "<select name='sa_status'" + dis + ">" + SA.Options(SA.TicketStatuses, st, null) + "</select>")).Append("</div>")
              .Append(field("Comments (reason) *", "<textarea name='sa_comment' rows='3' maxlength='1000' required" + dis + "></textarea>")).Append(Foot(back, "status", "Change Status", dis));
        else if (panel == "resolve")
            sb.Append(field("Resolution Note *", "<textarea name='sa_comment' rows='5' maxlength='1000' required" + dis + "></textarea>")).Append(Foot(back, "resolve", "Mark as Resolved", dis));
        else
            sb.Append(SA.HistoryTable(ready ? SA.History(c, "Ticket", tid.ToString(CultureInfo.InvariantCulture)) : null, "No support actions recorded for this ticket yet."));
        return sb.Append("</aside>").ToString();
    }
    static string Foot(string back, string op, string label, string dis)
    {
        return "<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportTickets.aspx?" + SA.A(back) + "'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='" + op + "'" + dis + ">" + label + "</button></div>";
    }
    void Act()
    {
        string op = SA.Form("sa_op"); int tid; Int32.TryParse(SA.Form(op == "archive" ? "sa_id" : "sa_tid"), out tid); // archive comes from the row menu
        string comment = SA.Form("sa_comment"); if (comment.Length > 1000) comment = comment.Substring(0, 1000);
        using (var c = SA.Open())
        {
            if (!SA.Has(c, "AnyminuteTicketMeta") || !SA.Has(c, "AnyminuteSupportAudit")) { SA.Flash(SA.MissingTables); Back(tid, op); return; }
            var cur = Load(c, true, tid); if (cur.Rows.Count != 1) { SA.Flash("Ticket not found."); Back(0, null); return; }
            var r = cur.Rows[0]; string key = tid.ToString(CultureInfo.InvariantCulture), st = SA.StatusLabel(r["StatusType"]);
            using (var tx = c.BeginTransaction())
            {
                AnyminuteTimesheetRepair.Execute(c, tx, "IF NOT EXISTS(SELECT 1 FROM AnyminuteTicketMeta WHERE TicketId=@i) INSERT AnyminuteTicketMeta(TicketId) VALUES(@i)", "@i", tid);
                Action touch = () => AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteTicketMeta SET UpdatedAtUTC=SYSUTCDATETIME() WHERE TicketId=@i", "@i", tid);
                Action<string, string> status = (label, note) =>
                {
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE Ticket SET StatusType=@s,TicketResolveDate=CASE WHEN @s='1' THEN CAST(GETDATE() AS date) ELSE NULL END WHERE TicketId=@i", "@s", SA.StatusCode(label), "@i", tid);
                    if (label == "Resolved" && note.Length > 0) AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE Ticket SET DeveloperComment=@n WHERE TicketId=@i", "@n", note, "@i", tid);
                };
                string done;
                if (op == "edit")
                {
                    string subj = SA.Form("sa_subject"), cat = SA.Form("sa_category"), pr = SA.Form("sa_priority"), desc = SA.Form("sa_desc");
                    if (!SA.Categories.Contains(cat) || !SA.Priorities.Contains(pr)) { tx.Rollback(); SA.Flash("Choose a valid category and priority."); Back(tid, op); return; }
                    if (subj.Length > 200) subj = subj.Substring(0, 200);
                    var changes = new List<string>(); var olds = new List<string>();
                    if (subj != Subject(r)) { olds.Add("Subject: " + Subject(r)); changes.Add("Subject: " + subj); }
                    if (cat != Category(r)) { olds.Add("Category: " + Category(r)); changes.Add("Category: " + cat); }
                    if (pr != Priority(r)) { olds.Add("Priority: " + Priority(r)); changes.Add("Priority: " + pr); }
                    if (desc != SA.S(r["Description"])) { olds.Add("Description edited"); changes.Add("Description updated"); AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE Ticket SET Description=@d WHERE TicketId=@i", "@d", desc, "@i", tid); }
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteTicketMeta SET Subject=@s,Category=@c,Priority=@p WHERE TicketId=@i", "@s", subj, "@c", cat, "@p", pr, "@i", tid);
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE Ticket SET TicketSeverity=@p WHERE TicketId=@i", "@p", pr, "@i", tid);
                    if (changes.Count == 0) { tx.Rollback(); SA.Flash("No changes to save."); Back(tid, op); return; }
                    SA.Audit(c, tx, "Ticket", key, "Edited", st, st, String.Join("; ", olds), String.Join("; ", changes), comment); done = "Ticket updated.";
                }
                else if (op == "assign")
                {
                    string who = SA.Form("sa_assignee"), was = SA.S(r["AssignedTo"]);
                    if (who == was) { tx.Rollback(); SA.Flash("Assignee is unchanged."); Back(tid, op); return; }
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteTicketMeta SET AssignedTo=@a WHERE TicketId=@i", "@a", who.Length == 0 ? (object)DBNull.Value : who, "@i", tid);
                    SA.Audit(c, tx, "Ticket", key, was.Length == 0 ? "Assigned" : "Reassigned", st, st, was, who.Length == 0 ? "Unassigned" : who, comment); done = "Ticket assignment saved.";
                }
                else if (op == "reply")
                {
                    string msg = SA.Form("sa_message"); if (msg.Length == 0) { tx.Rollback(); SA.Flash("Enter a reply message."); Back(tid, op); return; }
                    AnyminuteTimesheetRepair.Execute(c, tx, "INSERT Ticket_Comments(TicketId,EmpId,Name,EmailId,CommentsBy,Comments,CommentsDate) VALUES(@i,@e,@n,@m,@b,@c,GETDATE())",
                        "@i", tid, "@e", Convert.ToInt32(Session["EmployeeId"] ?? 0), "@n", SA.ActorName(), "@m", Convert.ToString(Session["email"]), "@b", SA.ActorName() + " (Support Admin)", "@c", msg);
                    string next = SA.Form("sa_waiting") == "1" ? "Waiting" : st == "Open" ? "In Progress" : st;
                    if (next != st) status(next, "");
                    SA.Audit(c, tx, "Ticket", key, "Replied", st, next, null, null, msg.Length > 1000 ? msg.Substring(0, 1000) : msg); done = "Reply added to the ticket conversation.";
                }
                else if (op == "status" || op == "resolve")
                {
                    string next = op == "resolve" ? "Resolved" : SA.Form("sa_status");
                    if (!SA.TicketStatuses.Contains(next)) { tx.Rollback(); SA.Flash("Choose a status."); Back(tid, op); return; }
                    if (comment.Length == 0) { tx.Rollback(); SA.Flash(op == "resolve" ? "Enter a resolution note." : "Enter the reason for the status change."); Back(tid, op); return; }
                    if (next == st) { tx.Rollback(); SA.Flash("The ticket is already " + st + "."); Back(tid, op); return; }
                    status(next, comment);
                    SA.Audit(c, tx, "Ticket", key, next == "Resolved" ? "Resolved" : st == "Resolved" ? "Reopened" : "Status Changed", st, next, null, null, comment); done = "Ticket status changed to " + next + ".";
                }
                else if (op == "archive")
                {
                    bool arch = Convert.ToBoolean(r["Archived"]);
                    if (!arch && st != "Resolved") { tx.Rollback(); SA.Flash("Only resolved tickets can be archived."); Back(0, null); return; }
                    AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE AnyminuteTicketMeta SET Archived=@a WHERE TicketId=@i", "@a", !arch, "@i", tid);
                    SA.Audit(c, tx, "Ticket", key, arch ? "Unarchived" : "Archived", st, st, null, null, null); done = arch ? "Ticket restored from the archive." : "Ticket archived. Its history is kept.";
                }
                else { tx.Rollback(); Back(0, null); return; }
                touch(); tx.Commit(); SA.Flash(done);
            }
        }
        Back(op == "archive" ? 0 : tid, op == "archive" ? null : "history");
    }
    void Back(int id, string panel) { Response.Redirect("SupportTickets.aspx" + (id > 0 ? "?id=" + id + "&panel=" + (panel ?? "details") : ""), false); Context.ApplicationInstance.CompleteRequest(); }
}
