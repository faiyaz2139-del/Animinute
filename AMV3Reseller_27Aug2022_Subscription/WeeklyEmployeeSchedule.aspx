<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="WeeklyEmployeeSchedule.aspx.cs" Inherits="AnyminuteWeeklySchedulePage" Title="Work Schedule" %>
<%-- 2026-10-25 v11 Deck I: Weekly Employee Schedule (planned hours; needs DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql). --%>
<%-- 2026-11-18 v90: chrome matches Weekly Time Card (am-card, am-title, am-toolbar, am-table, am-summary, am-button). JS hooks unchanged. --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261122v94" rel="stylesheet" /><link href="css/AnyminuteWeeklySchedule.css?v=20261122v94" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer="defer"></script><script src="Scripts/AnyminuteWeeklySchedule.js?v=20261122v94" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-card am-ws" id="amWeeklySchedule" aria-labelledby="am-ws-title">
 <div class="am-title"><div><h1 id="am-ws-title">Work Schedule</h1><p id="WsSub" runat="server">Set up weekly working hours, days off and break time for the selected employee. You can also repeat this schedule for multiple weeks.</p></div></div>
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
 <%-- v17: employees cannot edit planned hours here; their own Mon-Sun times are entered in My Time Card (WeekTimeCard.aspx). --%>
 <asp:Panel ID="TimeCardCta" runat="server" Visible="false" CssClass="am-ws-cta" role="note"><svg class="am-ws-cta-ic" viewBox="0 0 24 24" aria-hidden="true"><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/></svg><div class="am-ws-cta-text"><strong>Enter the hours you worked in My Time Card</strong><span>Work Schedule shows the hours your manager planned for you. Record your actual clock-in and clock-out times for Monday to Sunday in My Time Card.</span></div><asp:HyperLink ID="TimeCardLink" runat="server" NavigateUrl="~/WeekTimeCard.aspx" CssClass="am-button"><svg viewBox="0 0 24 24" aria-hidden="true" width="17" height="17"><circle cx="12" cy="12" r="9"/><path d="M12 7v5l3 2"/></svg><span>Enter My Times</span></asp:HyperLink></asp:Panel>
 <div class="am-toolbar am-week-selection am-ws-filters">
  <label><span>Business <b>*</b></span><asp:DropDownList ID="WsBusiness" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="Business_Changed" /></label>
  <label><span><asp:Literal ID="WsEmployeeLabel" runat="server" Text="Employee" /> <b>*</b></span><asp:DropDownList ID="WsEmployee" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="Employee_Changed" /></label>
  <label class="am-datefmt"><span>Week Starting <b>*</b></span><asp:TextBox ID="WsWeek" runat="server" ClientIDMode="Static" TextMode="Date" AutoPostBack="true" OnTextChanged="Week_Changed" /></label>
  <div class="am-actions am-ws-nav">
   <asp:LinkButton ID="PrevWeek" runat="server" CssClass="am-button secondary am-ws-sq am-ws-ctx" OnClick="PrevWeek_Click" ToolTip="Previous week" aria-label="Previous week"><svg viewBox="0 0 24 24" aria-hidden="true" width="17" height="17"><path d="M15 5l-7 7 7 7" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/></svg></asp:LinkButton>
   <asp:LinkButton ID="NextWeek" runat="server" CssClass="am-button secondary am-ws-sq am-ws-ctx" OnClick="NextWeek_Click" ToolTip="Next week" aria-label="Next week"><svg viewBox="0 0 24 24" aria-hidden="true" width="17" height="17"><path d="M9 5l7 7-7 7" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"/></svg></asp:LinkButton>
   <asp:LinkButton ID="TodayWeek" runat="server" CssClass="am-button secondary am-ws-ctx" OnClick="TodayWeek_Click" Text="Today" />
  </div>
 </div>

  <fieldset class="am-ws-repeat" id="amWsRepeat" runat="server" clientidmode="Static">
   <legend>Repeat Weekly (Optional)</legend>
   <label class="am-ws-check"><asp:CheckBox ID="RepeatOn" runat="server" ClientIDMode="Static" /><span>Repeat this schedule for multiple weeks</span></label>
   <div class="am-ws-repeat-row">
    <div class="am-ws-f am-datefmt"><label for="RepeatUntil">Repeat Until</label><asp:TextBox ID="RepeatUntil" runat="server" ClientIDMode="Static" TextMode="Date" /></div>
    <div class="am-ws-apply" role="radiogroup" aria-labelledby="amWsApplyLbl">
     <span id="amWsApplyLbl" class="am-ws-applylbl">Apply To</span>
     <div class="am-ws-applyopts">
      <label class="am-ws-check"><asp:RadioButton ID="ApplyAll" runat="server" ClientIDMode="Static" GroupName="ApplyTo" Checked="true" /><span>All Weeks</span></label>
      <label class="am-ws-check"><asp:RadioButton ID="ApplyWeekdays" runat="server" ClientIDMode="Static" GroupName="ApplyTo" /><span>Weekdays Only (Mon - Fri)</span></label>
      <label class="am-ws-check"><asp:RadioButton ID="ApplySelected" runat="server" ClientIDMode="Static" GroupName="ApplyTo" /><span>Selected Days</span></label>
      <details class="am-ws-days" id="amWsDays"><summary><span id="amWsDaysText">Please Select</span></summary>
       <asp:CheckBoxList ID="SelectedDays" runat="server" ClientIDMode="Static" RepeatLayout="Flow" CssClass="am-ws-daylist" />
      </details>
     </div>
    </div>
   </div>
  </fieldset>

  <div class="am-table-wrap am-ws-tablewrap" id="GridWrap" runat="server">
   <table class="am-table am-ws-table">
    <thead><tr><th scope="col">Day</th><th scope="col">Schedule Type</th><th scope="col">Start Time</th><th scope="col">End Time</th><th scope="col">Break (Minutes)</th><th scope="col">Total Hours</th><th scope="col">Notes (Optional)</th></tr></thead>
    <tbody>
    <asp:Repeater ID="Days" runat="server" OnItemDataBound="Days_ItemDataBound"><ItemTemplate>
     <tr class="am-ws-row" data-day="<%#: Eval("Date", "{0:yyyy-MM-dd}") %>">
      <th scope="row"><%#: Eval("Date", "{0:dddd}") %><asp:HiddenField ID="DayDate" runat="server" Value='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' /></th>
      <td><asp:DropDownList ID="DayType" runat="server" CssClass="am-ws-type" aria-label="Schedule Type"><asp:ListItem Value="" Text="-- Select --" /><asp:ListItem Value="Working" Text="Working" /><asp:ListItem Value="DayOff" Text="Day Off" /></asp:DropDownList></td>
      <td><asp:TextBox ID="DayStart" runat="server" TextMode="Time" CssClass="am-ws-start" aria-label="Start Time" /></td>
      <td><asp:TextBox ID="DayEnd" runat="server" TextMode="Time" CssClass="am-ws-end" aria-label="End Time" /></td>
      <td><asp:DropDownList ID="DayBreak" runat="server" CssClass="am-ws-break" aria-label="Break (Minutes)" /></td>
      <td class="am-ws-hours"><asp:Literal ID="DayHours" runat="server" /></td>
      <td><asp:TextBox ID="DayNotes" runat="server" MaxLength="200" CssClass="am-ws-notes" aria-label="Notes (Optional)" /></td>
     </tr>
    </ItemTemplate></asp:Repeater>
    </tbody>
   </table>
  </div>

  <div class="am-summary">
   <div><label>Total Scheduled Hours (Mon - Sun)</label><strong><span id="amWsTotal"><asp:Literal ID="TotalHours" runat="server" /></span> hrs</strong></div>
  </div>
  <div class="am-actions">
    <asp:PlaceHolder ID="UseDefault" runat="server"><button type="button" class="am-button secondary" id="amWsDefault" title="Fill Mon-Fri Working 09:00-17:00 (30 min break), Sat-Sun Day Off"><span>Use default pattern</span></button></asp:PlaceHolder>
    <asp:LinkButton ID="CopyPrevious" runat="server" CssClass="am-button secondary" OnClick="CopyPrevious_Click"><svg viewBox="0 0 24 24" aria-hidden="true" width="17" height="17"><rect x="8" y="8" width="12" height="12" rx="2" fill="none" stroke="currentColor" stroke-width="2.2"/><path d="M16 8V5a1 1 0 0 0-1-1H5a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h3" fill="none" stroke="currentColor" stroke-width="2.2"/></svg><span>Copy Previous Week</span></asp:LinkButton>
    <asp:LinkButton ID="CancelWeek" runat="server" CssClass="am-button secondary" OnClick="Cancel_Click" Text="Cancel" />
    <asp:LinkButton ID="SaveWeek" runat="server" CssClass="am-button" OnClick="Save_Click"><svg viewBox="0 0 24 24" aria-hidden="true" width="17" height="17"><path d="M5 3h11l3 3v15H5z" fill="none" stroke="currentColor" stroke-width="2.2"/><path d="M8 3v5h7V3M8 21v-7h8v7" fill="none" stroke="currentColor" stroke-width="2.2"/></svg><span>Save Weekly Schedule</span></asp:LinkButton>
  </div>
</section>
</asp:Content>
