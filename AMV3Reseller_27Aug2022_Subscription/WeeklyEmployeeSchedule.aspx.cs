using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

// 2026-10-25 v11 Deck I: Weekly Employee Schedule. Planned hours only (dbo.AnyminuteWeeklySchedule).
public partial class AnyminuteWeeklySchedulePage : System.Web.UI.Page
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly DayOfWeek[] Order = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday };

    protected void Page_Init(object sender, EventArgs e)
    {
        AnyminuteModernScope.Login(); ViewStateUserKey = Session.SessionID;
        foreach (var d in Order) SelectedDays.Items.Add(new ListItem(d.ToString().Substring(0, 3), d.ToString()));
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        string flash = AnyminuteFlash.Take("schedule"); if (flash != null) { FlashText.Text = HttpUtility.HtmlEncode(flash); Flash.Visible = true; }
        if (IsPostBack) return;
        foreach (DataRow r in AnyminuteModernScope.Businesses().Rows)
            WsBusiness.Items.Add(new ListItem(Convert.ToString(r["CompanyName"]), Convert.ToString(r["BusinessID"])));
        if (WsBusiness.Items.Count == 0) { Error("No business is available for your account."); Disable(); return; }
        if (WsBusiness.Items.FindByValue(Request.QueryString["BusinessId"] ?? "") != null) WsBusiness.SelectedValue = Request.QueryString["BusinessId"];
        BindEmployees();
        if (WsEmployee.Items.FindByValue(Request.QueryString["EmployeeId"] ?? "") != null) WsEmployee.SelectedValue = Request.QueryString["EmployeeId"];
        DateTime w;
        Week = DateTime.TryParseExact(Request.QueryString["week"] ?? "", "yyyy-MM-dd", Inv, DateTimeStyles.None, out w) ? AnyminuteWeeklySchedule.Monday(w) : AnyminuteWeeklySchedule.Monday(DateTime.Today);
        LoadWeek(null);
        if (!AnyminuteWeeklySchedule.Ready()) Error(AnyminuteWeeklySchedule.MissingTable.Replace(" Nothing was saved.", " You can review the page, but saving is unavailable until it is run."));
    }

    private int Business { get { int b; return Int32.TryParse(WsBusiness.SelectedValue, out b) ? b : 0; } }
    private int Employee { get { int v; return Int32.TryParse(WsEmployee.SelectedValue, out v) ? v : 0; } }
    private DateTime Week
    {
        get { DateTime d; return DateTime.TryParseExact(WsWeek.Text, "yyyy-MM-dd", Inv, DateTimeStyles.None, out d) ? AnyminuteWeeklySchedule.Monday(d) : AnyminuteWeeklySchedule.Monday(DateTime.Today); }
        set { WsWeek.Text = AnyminuteWeeklySchedule.Monday(value).ToString("yyyy-MM-dd", Inv); }
    }

    private void BindEmployees()
    {
        WsEmployee.Items.Clear();
        if (Business <= 0) return;
        foreach (DataRow r in AnyminuteModernScope.Users(Business).Rows)
        {
            string id = Convert.ToString(r["EmployeeID"]);
            if (WsEmployee.Items.FindByValue(id) == null) WsEmployee.Items.Add(new ListItem(Convert.ToString(r["EmpName"]), id));
        }
        if (WsEmployee.Items.Count == 0) WsEmployee.Items.Add(new ListItem("No employees in this business", ""));
    }

    // v16: employees (no edit right) see their own Work Schedule read-only.
    private bool viewOnly;
    private void LoadWeek(DateTime? copyFrom)
    {
        DateTime monday = Week; Week = monday;
        List<AnyminuteScheduleDay> week = AnyminuteWeeklySchedule.BlankWeek(monday);
        bool saved = false;
        viewOnly = Business > 0 && Employee > 0 && !AnyminuteWeeklySchedule.CanEdit(Business, Employee);
        if (viewOnly && copyFrom.HasValue) { Error("You can view this schedule but not change it."); copyFrom = null; }
        if (Business > 0 && Employee > 0)
        {
            try
            {
                if (copyFrom.HasValue)
                {
                    var prev = AnyminuteWeeklySchedule.Load(Business, Employee, copyFrom.Value, out saved);
                    if (!saved) { Error("There is no saved schedule for the previous week to copy."); return; }
                    for (int i = 0; i < 7; i++) { prev[i].Date = monday.AddDays(i); prev[i].Saved = false; }
                    week = prev;
                    Notice("Copied the week of " + copyFrom.Value.ToString("yyyy-MM-dd", Inv) + ". Review it, then click Save Weekly Schedule.");
                }
                else week = AnyminuteWeeklySchedule.Load(Business, Employee, monday, out saved);
            }
            catch (InvalidOperationException ex) { Error(ex.Message); }
        }
        Days.DataSource = week; Days.DataBind();
        TotalHours.Text = week.Sum(d => d.Hours).ToString("0.00", Inv);
        SaveWeek.Visible = CopyPrevious.Visible = CancelWeek.Visible = amWsRepeat.Visible = !viewOnly;
        GridWrap.Visible = !viewOnly || saved;
        if (viewOnly) WsSub.InnerText = "Your planned working hours, days off and break time for the selected week.";
        if (viewOnly && !saved) { TotalHours.Text = "0.00"; Notice("No work schedule has been published for this week yet."); }
        // v23 (Pandey #3): an unsaved selection shows a cleared grid (not the 09:00-17:00 default that looked like the previous
        // employee's data); "Use default pattern" fills Mon-Fri 09:00-17:00 on request. Every load says what is shown.
        string who = WsEmployee.SelectedItem != null ? WsEmployee.SelectedItem.Text : "";
        string label = who + " \u00b7 week of " + monday.ToString("MMM d, yyyy", Inv);
        if (!viewOnly && !copyFrom.HasValue && Business > 0 && Employee > 0 && !Flash.Visible)
            Notice(saved ? "Loaded the saved schedule for " + label + "."
                : "No saved schedule for " + label + ". The grid was cleared - set Working or Day Off for each day (or click Use default pattern), then click Save Weekly Schedule. Only saved schedules appear on the Weekly Time Card.");
        UseDefault.Visible = !viewOnly;
        // v17: employees get a clear way to enter their own times (My Time Card), published schedule or not.
        TimeCardCta.Visible = viewOnly;
        if (viewOnly) TimeCardLink.NavigateUrl = "~/WeekTimeCard.aspx?BusinessId=" + Business + "&Week=" + monday.ToString("yyyy-MM-dd", Inv);
    }

    protected void Days_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var d = e.Item.DataItem as AnyminuteScheduleDay; if (d == null) return;
        var type = (DropDownList)e.Item.FindControl("DayType"); var start = (TextBox)e.Item.FindControl("DayStart"); var end = (TextBox)e.Item.FindControl("DayEnd");
        var brk = (DropDownList)e.Item.FindControl("DayBreak"); var notes = (TextBox)e.Item.FindControl("DayNotes"); var hours = (Literal)e.Item.FindControl("DayHours");
        foreach (int m in AnyminuteWeeklySchedule.BreakOptions) brk.Items.Add(new ListItem(m + " min", m.ToString(Inv)));
        if (brk.Items.FindByValue(d.BreakMinutes.ToString(Inv)) == null) brk.Items.Add(new ListItem(d.BreakMinutes + " min", d.BreakMinutes.ToString(Inv)));
        if (d.Blank)
        {
            // v23 (Pandey #3): cleared day - no type, times, break or notes until the user picks Working / Day Off.
            type.SelectedValue = ""; type.CssClass = "am-ws-type is-unset";
            brk.Items.Insert(0, new ListItem("--", "")); brk.SelectedIndex = 0;
            start.Text = end.Text = notes.Text = ""; hours.Text = "0.00";
            if (viewOnly) foreach (System.Web.UI.WebControls.WebControl w in new System.Web.UI.WebControls.WebControl[] { type, start, end, brk, notes }) w.Enabled = false;
            return;
        }
        var unset = type.Items.FindByValue(""); if (unset != null) type.Items.Remove(unset);
        type.SelectedValue = d.Working ? "Working" : "DayOff";
        type.CssClass = "am-ws-type " + (d.Working ? "is-working" : "is-off");
        start.Text = d.Working ? d.Start.ToString(@"hh\:mm", Inv) : ""; end.Text = d.Working ? d.End.ToString(@"hh\:mm", Inv) : "";
        brk.SelectedValue = d.BreakMinutes.ToString(Inv); notes.Text = d.Notes ?? "";
        hours.Text = d.Hours.ToString("0.00", Inv);
        if (viewOnly) foreach (System.Web.UI.WebControls.WebControl w in new System.Web.UI.WebControls.WebControl[] { type, start, end, brk, notes }) w.Enabled = false;
        if (!d.Working)
        {
            // Day Off: the script greys out and disables these inputs (server never reads them for a Day Off).
            foreach (System.Web.UI.WebControls.WebControl w in new System.Web.UI.WebControls.WebControl[] { start, end, brk, notes }) w.Attributes["data-off"] = "1";
            brk.Items.Insert(0, new ListItem("--", "")); brk.SelectedIndex = 0;
        }
    }

    private List<AnyminuteScheduleDay> ReadGrid()
    {
        var list = new List<AnyminuteScheduleDay>();
        foreach (RepeaterItem item in Days.Items)
        {
            var date = DateTime.ParseExact(((HiddenField)item.FindControl("DayDate")).Value, "yyyy-MM-dd", Inv);
            string typeValue = ((DropDownList)item.FindControl("DayType")).SelectedValue;
            if (String.IsNullOrEmpty(typeValue))
                throw new InvalidOperationException(date.ToString("dddd", Inv) + ": choose Working or Day Off (or click Use default pattern).");
            bool working = typeValue == "Working";
            var day = new AnyminuteScheduleDay { Date = date, Working = working, Notes = working ? ((TextBox)item.FindControl("DayNotes")).Text.Trim() : "" };
            if (working)
            {
                string name = date.ToString("dddd", Inv);
                TimeSpan s, x; int b;
                if (!TryTime(((TextBox)item.FindControl("DayStart")).Text, out s) || !TryTime(((TextBox)item.FindControl("DayEnd")).Text, out x))
                    throw new InvalidOperationException(name + ": enter a Start Time and End Time, or set it to Day Off.");
                if (!Int32.TryParse(((DropDownList)item.FindControl("DayBreak")).SelectedValue, out b)) b = 0;
                day.Start = s; day.End = x; day.BreakMinutes = b;
            }
            list.Add(day);
        }
        return list;
    }

    private static bool TryTime(string text, out TimeSpan value)
    {
        DateTime t;
        if (DateTime.TryParseExact((text ?? "").Trim(), new[] { "HH:mm", "HH:mm:ss", "h:mm tt", "hh:mm tt" }, Inv, DateTimeStyles.None, out t)) { value = t.TimeOfDay; return true; }
        value = TimeSpan.Zero; return false;
    }

    // v23 (Pandey #3): every context change (Business, Employee, Week Starting, <, >, Today) resets Repeat Weekly and
    // reloads the grid for the new selection: its saved schedule, or a cleared grid when nothing is saved.
    private void ResetRepeat()
    {
        RepeatOn.Checked = false; RepeatUntil.Text = ""; ApplyAll.Checked = true; ApplyWeekdays.Checked = ApplySelected.Checked = false;
        foreach (ListItem li in SelectedDays.Items) li.Selected = false;
    }
    private void Reload() { ResetRepeat(); PageError.Visible = false; Flash.Visible = false; FlashText.Text = ""; LoadWeek(null); }
    protected void Business_Changed(object sender, EventArgs e) { BindEmployees(); Reload(); }
    protected void Employee_Changed(object sender, EventArgs e) { Reload(); }
    protected void Week_Changed(object sender, EventArgs e) { Reload(); }
    protected void PrevWeek_Click(object sender, EventArgs e) { Week = Week.AddDays(-7); Reload(); }
    protected void NextWeek_Click(object sender, EventArgs e) { Week = Week.AddDays(7); Reload(); }
    protected void TodayWeek_Click(object sender, EventArgs e) { Week = DateTime.Today; Reload(); }
    protected void CopyPrevious_Click(object sender, EventArgs e) { LoadWeek(Week.AddDays(-7)); }
    protected void Cancel_Click(object sender, EventArgs e) { Response.Redirect(SelfUrl(), false); Context.ApplicationInstance.CompleteRequest(); }

    protected void Save_Click(object sender, EventArgs e)
    {
        if (Business <= 0 || Employee <= 0) { Error("Select a business and an employee."); return; }
        if (!AnyminuteWeeklySchedule.CanEdit(Business, Employee)) { Error("You can view this schedule but not change it."); LoadWeek(null); return; }
        try
        {
            var week = ReadGrid();
            DateTime? until = null; string apply = ApplyWeekdays.Checked ? "weekdays" : ApplySelected.Checked ? "selected" : "all";
            var selected = new HashSet<DayOfWeek>();
            foreach (ListItem li in SelectedDays.Items) if (li.Selected) selected.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), li.Value));
            if (RepeatOn.Checked)
            {
                DateTime u;
                if (!DateTime.TryParseExact(RepeatUntil.Text, "yyyy-MM-dd", Inv, DateTimeStyles.None, out u)) throw new InvalidOperationException("Enter the Repeat Until date.");
                until = u;
            }
            int written = AnyminuteWeeklySchedule.Save(Business, Employee, week, until, apply, selected);
            AnyminuteFlash.Set("schedule", "Work schedule saved (" + written + " day" + (written == 1 ? "" : "s") + (until.HasValue ? ", repeated until " + until.Value.ToString("yyyy-MM-dd", Inv) : "") + ").");
            Response.Redirect(SelfUrl(), false); Context.ApplicationInstance.CompleteRequest();
        }
        catch (InvalidOperationException ex) { Error(ex.Message); }
    }

    private string SelfUrl()
    {
        return "~/WeeklyEmployeeSchedule.aspx?BusinessId=" + Business + "&EmployeeId=" + Employee + "&week=" + Week.ToString("yyyy-MM-dd", Inv);
    }
    private void Error(string message) { PageError.Text = HttpUtility.HtmlEncode(message); PageError.Visible = true; }
    private void Notice(string message) { FlashText.Text = HttpUtility.HtmlEncode(message); Flash.Visible = true; }
    private void Disable() { SaveWeek.Enabled = CopyPrevious.Enabled = false; }
}
