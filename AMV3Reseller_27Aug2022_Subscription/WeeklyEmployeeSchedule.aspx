<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="WeeklyEmployeeSchedule.aspx.cs" Inherits="AnyminuteWeeklySchedulePage" Title="Weekly Employee Schedule" %>
<%-- 2026-10-25 v11 Deck I: Weekly Employee Schedule (planned hours; needs DB_REQUIRED/REQUIRED_05_WeeklySchedule.sql). --%>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteFlash.css?v=20261024v2" rel="stylesheet" /><link href="css/AnyminuteWeeklySchedule.css?v=20261024v11" rel="stylesheet" /><script src="Scripts/AnyminuteDateText.js?v=20261024v8" defer="defer"></script><script src="Scripts/AnyminuteWeeklySchedule.js?v=20261024v11" defer="defer"></script></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-ws" id="amWeeklySchedule" aria-labelledby="am-ws-title">
 <nav class="am-ws-crumb" aria-label="Breadcrumb"><a href="Dashboard.aspx">Home</a><span aria-hidden="true">&gt;</span><a href="Schedular.aspx">Schedule</a><span aria-hidden="true">&gt;</span><span aria-current="page">Weekly Employee Schedule</span></nav>
 <h1 id="am-ws-title">Weekly Employee Schedule</h1>
 <p class="am-ws-sub">Set up weekly working hours, days off and break time for the selected employee. You can also repeat this schedule for multiple weeks.</p>
 <asp:Panel ID="Flash" runat="server" Visible="false" CssClass="am-flash" role="status"><span class="am-flash-icon" aria-hidden="true">&#10003;</span><asp:Literal ID="FlashText" runat="server" /></asp:Panel>
 <asp:Label ID="PageError" runat="server" Visible="false" CssClass="am-flash am-flash-error" role="alert" />
 <div class="am-ws-card">
  <div class="am-ws-filters">
   <div class="am-ws-f"><label for="WsBusiness">Business <b>*</b></label><asp:DropDownList ID="WsBusiness" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="Business_Changed" /></div>
   <div class="am-ws-f"><label for="WsEmployee">Employee <b>*</b></label><asp:DropDownList ID="WsEmployee" runat="server" ClientIDMode="Static" AutoPostBack="true" OnSelectedIndexChanged="Week_Changed" /></div>
   <div class="am-ws-f am-datefmt"><label for="WsWeek">Week Starting <b>*</b></label><asp:TextBox ID="WsWeek" runat="server" ClientIDMode="Static" TextMode="Date" AutoPostBack="true" OnTextChanged="Week_Changed" /></div>
   <div class="am-ws-nav">
    <asp:LinkButton ID="PrevWeek" runat="server" CssClass="am-ws-btn am-ws-outline am-ws-sq" OnClick="PrevWeek_Click" ToolTip="Previous week" aria-label="Previous week"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M15 5l-7 7 7 7"/></svg></asp:LinkButton>
    <asp:LinkButton ID="NextWeek" runat="server" CssClass="am-ws-btn am-ws-outline am-ws-sq" OnClick="NextWeek_Click" ToolTip="Next week" aria-label="Next week"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M9 5l7 7-7 7"/></svg></asp:LinkButton>
    <asp:LinkButton ID="TodayWeek" runat="server" CssClass="am-ws-btn am-ws-outline" OnClick="TodayWeek_Click" Text="Today" />
   </div>
  </div>

  <fieldset class="am-ws-repeat" id="amWsRepeat">
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

  <div class="am-ws-tablewrap">
   <table class="am-ws-table">
    <thead><tr><th scope="col">Day</th><th scope="col">Schedule Type</th><th scope="col">Start Time</th><th scope="col">End Time</th><th scope="col">Break (Minutes)</th><th scope="col">Total Hours</th><th scope="col">Notes (Optional)</th></tr></thead>
    <tbody>
    <asp:Repeater ID="Days" runat="server" OnItemDataBound="Days_ItemDataBound"><ItemTemplate>
     <tr class="am-ws-row" data-day="<%#: Eval("Date", "{0:yyyy-MM-dd}") %>">
      <th scope="row"><%#: Eval("Date", "{0:dddd}") %><asp:HiddenField ID="DayDate" runat="server" Value='<%# Eval("Date", "{0:yyyy-MM-dd}") %>' /></th>
      <td><asp:DropDownList ID="DayType" runat="server" CssClass="am-ws-type" aria-label="Schedule Type"><asp:ListItem Value="Working" Text="Working" /><asp:ListItem Value="DayOff" Text="Day Off" /></asp:DropDownList></td>
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

  <div class="am-ws-foot">
   <div class="am-ws-total"><span>Total Scheduled Hours (Mon - Sun)</span><strong><span id="amWsTotal"><asp:Literal ID="TotalHours" runat="server" /></span> hrs</strong></div>
   <div class="am-ws-actions">
    <asp:LinkButton ID="CopyPrevious" runat="server" CssClass="am-ws-btn am-ws-outline" OnClick="CopyPrevious_Click"><svg viewBox="0 0 24 24" aria-hidden="true"><rect x="8" y="8" width="12" height="12" rx="2"/><path d="M16 8V5a1 1 0 0 0-1-1H5a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h3"/></svg><span>Copy Previous Week</span></asp:LinkButton>
    <asp:LinkButton ID="CancelWeek" runat="server" CssClass="am-ws-btn am-ws-outline am-ws-cancel" OnClick="Cancel_Click" Text="Cancel" />
    <asp:LinkButton ID="SaveWeek" runat="server" CssClass="am-ws-btn am-ws-primary" OnClick="Save_Click"><svg viewBox="0 0 24 24" aria-hidden="true"><path d="M5 3h11l3 3v15H5z"/><path d="M8 3v5h7V3M8 21v-7h8v7"/></svg><span>Save Weekly Schedule</span></asp:LinkButton>
   </div>
  </div>
 </div>
</section>
</asp:Content>
