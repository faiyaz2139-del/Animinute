<%@ Page Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true" CodeFile="BusinessProfile.aspx.cs" Inherits="AnyminuteBusinessProfilePage" %>
<asp:Content ID="Head" ContentPlaceHolderID="head" runat="server"><link href="css/AnyminuteSubscriberDashboard.css?v=20261118v89" rel="stylesheet" /></asp:Content>
<asp:Content ID="Body" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<%-- 2026-11-16 v86: Business Profile = read-only view of one business (opened from "Business Profile" under the business name on the
     Businesses list). "Edit" opens the existing Edit Business screen (Add_Business.aspx?BusinessIDEdit=) and is shown to owners only. --%>
<section class="am-bp" data-build="v89-20261118">
 <asp:Literal ID="ProfileLit" runat="server" />
</section>
</asp:Content>
