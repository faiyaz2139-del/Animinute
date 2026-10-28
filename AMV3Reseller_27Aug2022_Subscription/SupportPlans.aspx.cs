using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using SA = AnyminuteSupportAdmin;

// 2026-10-28 v25 deck 3 (Subscription Plans): plans table, Add / Edit Subscription Plan, Confirm Plan Changes
// (previous vs new value + mandatory reason) and the success dialog; every save is written to AnyminuteSupportAudit.
// Plans live in AnyminuteSubscriptionPlan (REQUIRED_07, seeded with the 5 deck plans). This is the
// plan catalogue only: Stripe prices and plan-limit enforcement are not changed here (see FIX_REPORT_v25).
public partial class AnyminuteSupportPlansPage : System.Web.UI.Page
{
    class Plan
    {
        public int Id; public string Name = "", Description = "", Frequency = "Monthly", Features = "";
        public decimal? Monthly, Annual; public int? Businesses, Employees, Projects; public int Trial = 14; public bool Active = true;
    }
    static readonly Plan[] Defaults = {
        P(1,"Basic Plan","For small teams",29,290,1,10,5,14), P(2,"Standard Plan","Growing businesses",69,690,3,25,15,14), P(3,"Professional Plan","For established businesses",129,1290,5,50,25,14),
        P(4,"Enterprise Plan","Custom solution for large organizations",null,null,null,null,null,14), P(5,"Trial Plan","Limited time trial",0,0,1,5,5,30) };
    static Plan P(int id, string n, string d, decimal? m, decimal? a, int? b, int? e, int? p, int t) { return new Plan { Id = id, Name = n, Description = d, Monthly = m, Annual = a, Businesses = b, Employees = e, Projects = p, Trial = t }; }
    static decimal? Dec(object o) { return o == null || o == DBNull.Value ? (decimal?)null : Convert.ToDecimal(o); }
    static int? Int(object o) { return o == null || o == DBNull.Value ? (int?)null : Convert.ToInt32(o); }
    static string Money(decimal? d) { return d == null ? "Custom" : "$" + d.Value.ToString("#,0.00", CultureInfo.InvariantCulture); }
    static string Num(int? i) { return i == null ? "Custom" : i.Value.ToString(CultureInfo.InvariantCulture); }
    static string Raw(decimal? d) { return d == null ? "" : d.Value.ToString("0.00", CultureInfo.InvariantCulture); }
    static string Raw(int? i) { return i == null ? "" : i.Value.ToString(CultureInfo.InvariantCulture); }
    List<Plan> Load(SqlConnection c, bool ready)
    {
        if (!ready) return Defaults.ToList();
        return SA.Q(c, "SELECT * FROM AnyminuteSubscriptionPlan ORDER BY PlanID").Rows.Cast<DataRow>().Select(r => new Plan {
            Id = Convert.ToInt32(r["PlanID"]), Name = SA.S(r["PlanName"]), Description = SA.S(r["Description"]), Monthly = Dec(r["MonthlyPrice"]), Annual = Dec(r["AnnualPrice"]),
            Businesses = Int(r["MaxBusinesses"]), Employees = Int(r["MaxEmployees"]), Projects = Int(r["MaxProjects"]), Trial = Convert.ToInt32(r["TrialDays"]),
            Frequency = SA.S(r["BillingFrequency"]), Features = SA.S(r["Features"]), Active = Convert.ToBoolean(r["IsActive"]) }).ToList();
    }
    protected void Page_Init(object sender, EventArgs e) { SA.Require(); }
    protected void Page_Load(object sender, EventArgs e)
    {
        bool usage = SA.Get("view") == "usage";
        Title1.Text = usage ? "Usage &amp; Limits" : "Subscription Plans";
        Lead.Text = usage ? "Plan limits and how many active subscriptions use each plan." : "Create and manage subscription plans, limits, pricing and features.";
        Flash.Text = SA.TakeFlash();
        using (var c = SA.Open())
        {
            bool ready = SA.Has(c, "AnyminuteSubscriptionPlan") && SA.Has(c, "AnyminuteSupportAudit");
            if (!ready) Flash.Text += SA.Notice(c, "AnyminuteSubscriptionPlan", "AnyminuteSupportAudit");
            var plans = Load(c, ready);
            Table.Text = usage ? Usage(c, plans) : List(plans);
            string op = IsPostBack ? SA.Form("sa_op") : "";
            if (op == "review" || op == "save") { Review(c, plans, ready, op == "save"); return; }
            int id, saved;
            if (Int32.TryParse(SA.Get("saved"), out saved)) Modal.Text = Success(saved);
            if (SA.Get("add") == "1") Editor.Text = Form(new Plan(), ready, true);
            else if (Int32.TryParse(SA.Get("edit"), out id)) { var p = plans.FirstOrDefault(x => x.Id == id); if (p != null) Editor.Text = Form(p, ready, false); }
        }
    }
    static string List(List<Plan> plans)
    {
        var sb = new StringBuilder("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Sl. No.</th><th>Plan Name</th><th>Description</th><th>Monthly Price (CAD)</th><th>Annual Price (CAD)</th><th>Max Businesses</th><th>Max Employees</th><th>Max Projects</th><th>Trial Days</th><th>Status</th><th>Actions</th></tr></thead><tbody>");
        int n = 0;
        foreach (var p in plans)
            sb.Append("<tr><td>").Append(++n).Append("</td><td><a class='sa-link' href='SupportPlans.aspx?edit=").Append(p.Id).Append("'>").Append(SA.H(p.Name)).Append("</a></td><td>").Append(SA.H(p.Description)).Append("</td><td>").Append(Money(p.Monthly))
              .Append("</td><td>").Append(Money(p.Annual)).Append("</td><td>").Append(Num(p.Businesses)).Append("</td><td>").Append(Num(p.Employees)).Append("</td><td>").Append(Num(p.Projects)).Append("</td><td>").Append(p.Trial)
              .Append("</td><td>").Append(SA.Pill(p.Active ? "Active" : "Inactive", p.Active ? "green" : "grey")).Append("</td><td class='sa-actions'><a class='sa-btn sa-btn-sm sa-btn-outline' href='SupportPlans.aspx?edit=").Append(p.Id).Append("' aria-label='Edit ").Append(SA.A(p.Name)).Append("'>&#9998;</a></td></tr>");
        return sb.Append("</tbody></table></div><div class='sa-pager'><span>Showing 1 to ").Append(plans.Count).Append(" of ").Append(plans.Count).Append(" plans</span></div>").ToString();
    }
    // v26: PlanHistory 'BASIC PLAN' and catalogue 'Basic Plan' / 'Basic' are the same plan (SA.PlanKey). "Limits applied" counts
    // active subscriptions whose PlanHistory employee / business limits equal the catalogue (the values the app enforces).
    static string Usage(SqlConnection c, List<Plan> plans)
    {
        var counts = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase); var names = new Dictionary<string, string>();
        foreach (DataRow r in SA.Q(c, "SELECT " + String.Format(SA.PlanKeySql, "ISNULL(PlanName,'(unnamed)')") + " K,MIN(PlanName) N,No_Of_Users U,Business B,COUNT(DISTINCT EmailId) C FROM PlanHistory WHERE ISNULL(IsActive,0)=1 AND ISNULL(Is_Cancelled,0)=0 GROUP BY " + String.Format(SA.PlanKeySql, "ISNULL(PlanName,'(unnamed)')") + ",No_Of_Users,Business").Rows)
        {
            string k = SA.S(r["K"]); int[] v; if (!counts.TryGetValue(k, out v)) { v = new int[2]; counts[k] = v; names[k] = SA.S(r["N"]); }
            v[0] += Convert.ToInt32(r["C"]);
            var p = plans.FirstOrDefault(x => SA.PlanKey(x.Name) == k);
            if (p != null && (p.Employees == null || SA.S(r["U"]) == Raw(p.Employees)) && (p.Businesses == null || SA.S(r["B"]) == Raw(p.Businesses))) v[1] += Convert.ToInt32(r["C"]);
        }
        var sb = new StringBuilder("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Plan</th><th>Active Subscriptions</th><th>Limits Applied</th><th>Max Businesses</th><th>Max Employees</th><th>Max Projects</th><th>Trial Days</th></tr></thead><tbody>");
        foreach (var p in plans)
        {
            int[] n; string k = SA.PlanKey(p.Name); if (!counts.TryGetValue(k, out n)) n = new int[2]; counts.Remove(k);
            sb.Append("<tr><td>").Append(SA.H(p.Name)).Append("</td><td>").Append(n[0]).Append("</td><td>").Append(n[0] == 0 ? "&mdash;" : n[1] + " of " + n[0]).Append("</td><td>").Append(Num(p.Businesses)).Append("</td><td>").Append(Num(p.Employees)).Append("</td><td>").Append(Num(p.Projects)).Append("</td><td>").Append(p.Trial).Append("</td></tr>");
        }
        foreach (var k in counts) sb.Append("<tr><td>").Append(SA.H(names[k.Key])).Append(" <small class='sa-muted'>(billing plan)</small></td><td>").Append(k.Value[0]).Append("</td><td colspan='5' class='sa-muted'>Not in the plan catalogue</td></tr>");
        return sb.Append("</tbody></table></div><p class='sa-muted'>Max Businesses and Max Employees are enforced by AnyMinute through each subscription's plan record; saving a plan can apply them to active subscribers. Max Projects and Trial Days are catalogue values only (no AnyMinute page enforces them yet); Stripe prices are not changed here.</p>").ToString();
    }
    // Active, non-cancelled PlanHistory rows for these plan names (old and new name when renamed).
    const string ApplyWhere = " FROM PlanHistory WHERE ISNULL(IsActive,0)=1 AND ISNULL(Is_Cancelled,0)=0 AND {0} IN(@k1,@k2)";
    static int Subscribed(SqlConnection c, SqlTransaction tx, string oldName, string newName)
    {
        return Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "SELECT COUNT(*)" + String.Format(ApplyWhere, String.Format(SA.PlanKeySql, "PlanName")), "@k1", SA.PlanKey(oldName), "@k2", SA.PlanKey(newName)).Rows[0][0]);
    }
    static string Form(Plan p, bool ready, bool add)
    {
        string dis = ready ? "" : " disabled";
        Func<string, string, string, string, string> input = (label, name, value, extra) => "<label class='sa-field'><span>" + label + "</span><input name='" + name + "' value='" + SA.A(value) + "' " + extra + dis + " /></label>";
        var sb = new StringBuilder("<div class='sa-modal' role='dialog' aria-modal='true' aria-labelledby='sa-plan-title'><div class='sa-card sa-dialog'><div class='sa-panel-head'><div><h2 id='sa-plan-title'>" + (add ? "Add Subscription Plan" : "Edit Subscription Plan") + "</h2><small>" + (add ? "Create a new subscription plan with limits and pricing." : "Update plan details, limits and pricing.") + "</small></div><a class='sa-x' href='SupportPlans.aspx' aria-label='Close'>&times;</a></div>");
        sb.Append("<input type='hidden' name='sa_pid' value='").Append(p.Id).Append("' />")
          .Append(input("Plan Name *", "sa_name", p.Name, "maxlength='100' required"))
          .Append(input("Description", "sa_desc", p.Description, "maxlength='300'"))
          .Append("<div class='sa-row2'>").Append(input("Monthly Price (CAD) *", "sa_monthly", Raw(p.Monthly), "inputmode='decimal' placeholder='Blank = Custom'")).Append(input("Annual Price (CAD)", "sa_annual", Raw(p.Annual), "inputmode='decimal' placeholder='Blank = Custom'")).Append("</div>")
          .Append("<div class='sa-row2'><fieldset class='sa-field'><legend>Billing Frequency</legend><label class='sa-radio'><input type='radio' name='sa_freq' value='Monthly'").Append(p.Frequency != "Annual" ? " checked" : "").Append(dis).Append(" /> Monthly</label><label class='sa-radio'><input type='radio' name='sa_freq' value='Annual'").Append(p.Frequency == "Annual" ? " checked" : "").Append(dis).Append(" /> Annual</label></fieldset>")
          .Append(input("Trial Duration (Days)", "sa_trial", p.Trial.ToString(CultureInfo.InvariantCulture), "type='number' min='0' max='365'")).Append("</div>")
          .Append("<h3>Plan Limits</h3><div class='sa-row3'>").Append(input("Max Businesses *", "sa_biz", Raw(p.Businesses), "type='number' min='0' placeholder='Custom'")).Append(input("Max Employees *", "sa_emp", Raw(p.Employees), "type='number' min='0' placeholder='Custom'")).Append(input("Max Projects *", "sa_proj", Raw(p.Projects), "type='number' min='0' placeholder='Custom'")).Append("</div>")
          .Append("<label class='sa-field'><span>Plan Features (Optional, one per line)</span><textarea name='sa_features' rows='4'").Append(dis).Append(">").Append(SA.H(p.Features)).Append("</textarea></label>")
          .Append("<fieldset class='sa-field'><legend>Plan Status</legend><label class='sa-radio'><input type='radio' name='sa_active' value='1'").Append(p.Active ? " checked" : "").Append(dis).Append(" /> Active</label><label class='sa-radio'><input type='radio' name='sa_active' value='0'").Append(!p.Active ? " checked" : "").Append(dis).Append(" /> Inactive</label></fieldset>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportPlans.aspx'>Cancel</a><button type='submit' class='sa-btn' name='sa_op' value='review'").Append(dis).Append(">").Append(add ? "Save Plan" : "Save Changes").Append("</button></div></div></div>");
        return sb.ToString();
    }
    Plan Posted(out string error)
    {
        string err = null; var p = new Plan(); int id; Int32.TryParse(SA.Form("sa_pid"), out id); p.Id = id;
        p.Name = SA.Form("sa_name"); p.Description = SA.Form("sa_desc"); p.Features = SA.Form("sa_features"); p.Frequency = SA.Form("sa_freq") == "Annual" ? "Annual" : "Monthly"; p.Active = SA.Form("sa_active") != "0";
        Func<string, decimal?> dec = k => { string v = SA.Form(k).Replace("$", "").Replace(",", ""); decimal d; if (v.Length == 0) return null; if (!Decimal.TryParse(v, NumberStyles.Number, CultureInfo.InvariantCulture, out d) || d < 0) { err = "Prices must be blank (Custom) or a positive amount."; return null; } return Math.Round(d, 2); };
        Func<string, int?> num = k => { string v = SA.Form(k); int i; if (v.Length == 0) return null; if (!Int32.TryParse(v, out i) || i < 0) { err = "Limits must be blank (Custom) or a whole number."; return null; } return i; };
        p.Monthly = dec("sa_monthly"); p.Annual = dec("sa_annual"); p.Businesses = num("sa_biz"); p.Employees = num("sa_emp"); p.Projects = num("sa_proj");
        int t; p.Trial = Int32.TryParse(SA.Form("sa_trial"), out t) && t >= 0 && t <= 365 ? t : -1; if (p.Trial < 0) err = "Trial duration must be 0-365 days.";
        if (p.Name.Length == 0) err = "Plan name is required.";
        if (p.Name.Length > 100 || p.Description.Length > 300) err = "Plan name or description is too long.";
        error = err;
        return p;
    }
    static List<string[]> Diff(Plan a, Plan b)
    {
        var rows = new List<string[]>();
        Action<string, string, string> add = (f, x, y) => { if (x != y) rows.Add(new[] { f, x, y }); };
        add("Plan Name", a.Name, b.Name); add("Description", a.Description, b.Description); add("Monthly Price (CAD)", Money(a.Monthly), Money(b.Monthly)); add("Annual Price (CAD)", Money(a.Annual), Money(b.Annual));
        add("Billing Frequency", a.Frequency, b.Frequency); add("Max Businesses", Num(a.Businesses), Num(b.Businesses)); add("Max Employees", Num(a.Employees), Num(b.Employees)); add("Max Projects", Num(a.Projects), Num(b.Projects));
        add("Trial Duration (Days)", a.Trial.ToString(CultureInfo.InvariantCulture), b.Trial.ToString(CultureInfo.InvariantCulture)); add("Plan Features", a.Features.Replace("\r", ""), b.Features.Replace("\r", "")); add("Plan Status", a.Active ? "Active" : "Inactive", b.Active ? "Active" : "Inactive");
        return rows;
    }
    void Review(SqlConnection c, List<Plan> plans, bool ready, bool save)
    {
        string error; var p = Posted(out error); bool add = p.Id == 0;
        var before = add ? new Plan { Name = "", Description = "", Trial = 0, Active = false, Frequency = "" } : plans.FirstOrDefault(x => x.Id == p.Id);
        if (!ready) error = SA.MissingTables; else if (before == null) error = "That plan no longer exists.";
        else if (plans.Any(x => x.Id != p.Id && x.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase))) error = "Another plan already uses this name.";
        Editor.Text = Form(p, ready, add);
        if (error != null) { Flash.Text += "<div class='sa-flash sa-warn' role='alert'>" + SA.H(error) + "</div>"; return; }
        var diff = Diff(before, p);
        if (diff.Count == 0) { Flash.Text += "<div class='sa-flash' role='status'>No changes to save.</div>"; return; }
        string reason = SA.Form("sa_reason"); if (reason.Length > 500) reason = reason.Substring(0, 500);
        bool limits = diff.Any(d => d[0] == "Max Businesses" || d[0] == "Max Employees") && (p.Employees != null || p.Businesses != null);
        if (save && reason.Length > 0)
        {
            using (var tx = c.BeginTransaction())
            {
                var args = new object[] { "@n", p.Name, "@d", p.Description, "@m", (object)p.Monthly ?? DBNull.Value, "@a", (object)p.Annual ?? DBNull.Value, "@b", (object)p.Businesses ?? DBNull.Value, "@e", (object)p.Employees ?? DBNull.Value,
                    "@p", (object)p.Projects ?? DBNull.Value, "@t", p.Trial, "@f", p.Frequency, "@ft", p.Features, "@ac", p.Active, "@u", SA.ActorId(), "@i", p.Id };
                int id = p.Id;
                if (add) id = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, @"INSERT AnyminuteSubscriptionPlan(PlanName,Description,MonthlyPrice,AnnualPrice,MaxBusinesses,MaxEmployees,MaxProjects,TrialDays,BillingFrequency,Features,IsActive,UpdatedBy)
                    VALUES(@n,@d,@m,@a,@b,@e,@p,@t,@f,@ft,@ac,@u); SELECT CAST(SCOPE_IDENTITY() AS int)", args).Rows[0][0]);
                else AnyminuteTimesheetRepair.Execute(c, tx, @"UPDATE AnyminuteSubscriptionPlan SET PlanName=@n,Description=@d,MonthlyPrice=@m,AnnualPrice=@a,MaxBusinesses=@b,MaxEmployees=@e,MaxProjects=@p,TrialDays=@t,BillingFrequency=@f,Features=@ft,IsActive=@ac,UpdatedBy=@u,UpdatedAtUTC=SYSUTCDATETIME() WHERE PlanID=@i", args);
                string applied = "";
                if (limits && SA.Form("sa_apply") == "1" && (p.Employees != null || p.Businesses != null))
                {
                    var set = new List<string>(); if (p.Employees != null) set.Add("No_Of_Users=@e"); if (p.Businesses != null) set.Add("Business=@b");
                    int n = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "UPDATE PlanHistory SET " + String.Join(",", set) + " WHERE Id IN(SELECT Id" + String.Format(ApplyWhere, String.Format(SA.PlanKeySql, "PlanName")) + "); SELECT @@ROWCOUNT",
                        "@e", p.Employees == null ? "" : Raw(p.Employees), "@b", p.Businesses == null ? "" : Raw(p.Businesses), "@k1", SA.PlanKey(before.Name), "@k2", SA.PlanKey(p.Name)).Rows[0][0]);
                    applied = " [Limits applied to " + n + " active subscription(s)]";
                }
                SA.Audit(c, tx, "Plan", id.ToString(CultureInfo.InvariantCulture), add ? "Plan Added" : "Plan Updated", add ? null : (before.Active ? "Active" : "Inactive"), p.Active ? "Active" : "Inactive",
                    String.Join("; ", diff.Select(d => d[0] + ": " + d[1])), String.Join("; ", diff.Select(d => d[0] + ": " + d[2])), (reason + applied).Length > 500 ? (reason + applied).Substring(0, 500) : reason + applied);
                tx.Commit();
                Response.Redirect("SupportPlans.aspx?saved=" + id, false); Context.ApplicationInstance.CompleteRequest(); return;
            }
        }
        var sb = new StringBuilder("<div class='sa-modal sa-modal-top' role='dialog' aria-modal='true' aria-labelledby='sa-confirm-title' data-sa-modal='1'><div class='sa-card sa-dialog sa-dialog-sm'><div class='sa-panel-head'><div><h2 id='sa-confirm-title'>Confirm Plan Changes</h2><small>Review the changes below before saving.</small></div><button type='button' class='sa-x' data-sa-close='1' aria-label='Close'>&times;</button></div>");
        sb.Append("<div class='sa-table-wrap'><table class='sa-table'><thead><tr><th>Field</th><th>Previous Value</th><th>New Value</th></tr></thead><tbody>");
        foreach (var d in diff) sb.Append("<tr><td>").Append(SA.H(d[0])).Append("</td><td>").Append(d[1].Length == 0 ? "&mdash;" : SA.H(d[1])).Append("</td><td><strong>").Append(SA.H(d[2])).Append("</strong></td></tr>");
        sb.Append("</tbody></table></div>");
        if (save && reason.Length == 0) sb.Append("<div class='sa-flash sa-warn' role='alert'>Enter the reason for this change.</div>");
        if (limits)
        {
            int n = Subscribed(c, null, before.Name, p.Name);
            sb.Append("<label class='sa-check'><input type='checkbox' name='sa_apply' value='1'").Append(!save || SA.Form("sa_apply") == "1" ? " checked" : "").Append(" /> Apply the new employee / business limits to the ").Append(n).Append(" active subscription(s) on this plan</label><p class='sa-muted sa-small'>AnyMinute enforces these limits from each subscription's plan record (PlanHistory). Leave unchecked to change the catalogue only; new Basic sign-ups always use the catalogue.</p>");
        }
        sb.Append("<label class='sa-field'><span>Reason for Change *</span><textarea name='sa_reason' rows='3' maxlength='500' required data-sa-counter='500'>").Append(SA.H(reason)).Append("</textarea><small data-sa-count='1'>0/500</small></label>")
          .Append("<div class='sa-panel-foot'><a class='sa-btn sa-btn-outline' href='SupportPlans.aspx'>Cancel</a><button type='button' class='sa-btn sa-btn-outline' data-sa-close='1'>Back</button><button type='submit' class='sa-btn' name='sa_op' value='save'>Confirm &amp; Save</button></div></div></div>");
        Modal.Text = sb.ToString();
    }
    static string Success(int id)
    {
        return "<div class='sa-modal sa-modal-top' role='dialog' aria-modal='true' aria-labelledby='sa-ok-title' data-sa-modal='1'><div class='sa-card sa-dialog sa-dialog-sm sa-center'><button type='button' class='sa-x sa-x-abs' data-sa-close='1' aria-label='Close'>&times;</button><div class='sa-ok-icon' aria-hidden='true'>&#10003;</div><h2 id='sa-ok-title'>Plan Updated Successfully!</h2><p>The subscription plan has been updated and the changes have been recorded in the activity log.</p>"
            + "<div class='sa-panel-foot sa-foot-even'><a class='sa-btn sa-btn-outline' href='SupportPlans.aspx?edit=" + id + "'>View Plan Details</a><a class='sa-btn' href='SupportPlans.aspx'>Back to Plans</a></div></div></div>";
    }
}
