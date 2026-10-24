<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="Help.aspx.cs" Inherits="AnyminuteHelp" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server" />
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<section class="am-card am-help-center"><h1>AnyMinute Help Center</h1>
<h2>Enter and submit weekly time</h2><p>Open Time Cards, select your business, employee and week. Choose Worked or Day Off for each day. Enter clock-in and clock-out times. Select Next day only when the shift ends on the following date. Save changes to keep a draft, then Submit Week when it is ready for review.</p>
<h2>Manage breaks</h2><p>Save the shift before recording breaks. Open View / edit breaks, enter start and end times and choose Paid or Unpaid. Breaks must stay inside the shift and cannot overlap. Unpaid breaks reduce net hours. Paid breaks remain included in net hours.</p>
<h2>Review submitted time</h2><p>Authorized reviewers open Approvals and select the correct week. Review each worked day before approving the week. A comment is required when returning a week for correction. Submitted and approved timecards are locked. Rejected timecards return to the employee for correction and resubmission.</p>
<h2>Business status</h2><p>Subscribers and business owners can use the three-dot business menu on Dashboard to choose Active or Not Active. This retains existing business records. It does not delete timesheets or change their approval state.</p>
<h2>Account and support</h2><p>Use the profile icon to open My profile or Sign out. Plan Upgrade appears below Change Password in the account menu when permitted. The help menu includes Chat for automated guidance and Tickets when permitted. Use an available ticket form to report a problem or share a suggestion. Include the page, week and the steps that reproduce the problem.</p>
</section></asp:Content>
