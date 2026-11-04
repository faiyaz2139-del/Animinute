using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MasterPage : System.Web.UI.MasterPage
{
    string _usertype;
    CommanClass comm = new CommanClass();
    DataTable MenusDomain = new DataTable();
    DataTable AllMenu = new DataTable();
    CreateLinkClass cl = new CreateLinkClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        Response.ContentEncoding = Encoding.UTF8;
        Response.HeaderEncoding = Encoding.UTF8;
        Response.Charset = "utf-8";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.AppendHeader("Refresh", Convert.ToString(Session.Timeout * 60) + "; URL=https://timesheet.anyminute.net/");
        if (!IsPostBack)
        {
            if (Session["UserId"] != null && Session["EmployeeId"] != null && Session["UserTypeid"] != null && Session["UserType"] != null && Session["MotherOrgid"] != null)
            {
                Session["UserLoginId"] = Session["UserId"];
                if (Session["UserType"].ToString() == "SuperAdmin")
                {
                    SuperLinkMaster.Visible = true;
                    SuperMother.Visible = true;
                    CreateUser.Visible = true;
                    SuperAdminTicket.Visible = true;
                    EmailSetting.Visible = true;
                    //logo.HRef = "SuperAdmin.aspx";

                    //profile.HRef = "SuperAdminProfile.aspx";
                    profile.PostBackUrl = "~/SuperAdminProfile.aspx";
                }
                else
                {
                    if (Session["UserTypeid"].ToString() == "0" && Session["UserType"].ToString() == "Admin")
                    {
                        MenuReport.Visible = false;
                        //BtnBackToAcc.Visible = false;
                        AllTickets.Visible = false;
                        DataTable dtAccDetails = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                        if (dtAccDetails.Rows.Count > 0)
                        {
                            if (dtAccDetails.Rows[0]["Status"].ToString() != "False")
                            {
                                MenuReport.Visible = true;
                                //logo.HRef = "Accountant.aspx";
                                //profile.HRef = "AccountantProfile.aspx";
                                profile.PostBackUrl = "~/AccountantProfile.aspx";
                                A1.HRef = "Dashboard.aspx"; // v37
                                //BtnBackToAcc.Visible = true;
                                AllTickets.Visible = true;
                            }
                        }
                    }
                    else if (Session["email"].ToString() == "support@anyminute.net" && Session["UserTypeid"].ToString() == "1" && Session["UserType"].ToString() == "SupportAdmin")
                    {
                        //logo.HRef = "SuperAdmin_Approve_Subscriber.aspx"; 
                        //profile.HRef = "SupportAdminProfile.aspx";
                        profile.PostBackUrl = "~/SupportAdminProfile.aspx";
                        SuperAdminTicket.Visible = true;
                        SA_Postalcode.Visible = true;
                        //BtnBackToAcc.Visible = false;
                        MenuReport.Visible = false;
                    }
                    else if (Session["UserTypeid"].ToString() == "1" && Session["UserType"].ToString() == "SupportAdmin")
                    {
                        //logo.HRef = "SuperAdmin.aspx";
                        //profile.HRef = "SupportAdminProfile.aspx";
                        profile.PostBackUrl = "~/SupportAdminProfile.aspx";
                        SuperAdminTicket.Visible = true;
                        SA_Postalcode.Visible = true;
                        //BtnBackToAcc.Visible = false;
                        MenuReport.Visible = false;
                    }
                    if (Session["UserTypeid"].ToString() != "1" && Session["UserType"].ToString() != "SupportAdmin")
                    {
                        DataTable dtAccDetails = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                        if (dtAccDetails.Rows.Count > 0)
                        {
                            if (dtAccDetails.Rows[0]["Status"].ToString() != "False")
                            {
                                //BtnBackToAcc.Visible = false;
                                AllTickets.Visible = true;
                            }
                        }
                    }

                    BindmenuFromLoginTable();
                    GetCompanyName();
                }

                // Resolve the canonical role from UserTypeId where possible.  Session["UserType"]
                // can contain a display label that does not match the menu/security role name.
                string currentUserType = Convert.ToString(Session["UserType"]).Trim();
                int currentUserTypeId;
                if (int.TryParse(Convert.ToString(Session["UserTypeid"]), out currentUserTypeId) && currentUserTypeId > 0)
                {
                    DataTable canonicalRoleRow = comm.GetAllDataFromTable("UserType", "UserType",
                        "User_TypeId=" + currentUserTypeId, "");
                    if (canonicalRoleRow.Rows.Count > 0 && !string.IsNullOrWhiteSpace(Convert.ToString(canonicalRoleRow.Rows[0]["UserType"])))
                        currentUserType = Convert.ToString(canonicalRoleRow.Rows[0]["UserType"]).Trim();
                }

                // Dashboard link: persisted Owner assignment wins over a stale/mismatched role label.
                bool hasOwnerAssignment = false;
                if (Session["EmployeeId"] != null && Session["MotherOrgid"] != null)
                {
                    DataTable ownerAssignment = comm.GetAllDataFromTable("AssginBussiness", "TOP 1 EmployeeID",
                        "EmployeeID=" + Convert.ToInt32(Session["EmployeeId"]) +
                        " and MotherOrgID=" + Convert.ToInt32(Session["MotherOrgid"]) + " and UserTypeId=5", "");
                    hasOwnerAssignment = ownerAssignment.Rows.Count > 0;
                }

                if (hasOwnerAssignment)
                    A1.HRef = "Owner_Dashboard.aspx";
                else if (string.Equals(currentUserType, "BUSINESS OWNER", StringComparison.OrdinalIgnoreCase))
                    A1.HRef = "Business_Owner_Dashboard.aspx";
                else if (string.Equals(currentUserType, "Accountant", StringComparison.OrdinalIgnoreCase))
                    A1.HRef = "Dashboard.aspx"; // v37
                else
                    A1.HRef = "Employee_Dashboard.aspx";

                var menuHandler = new MenuHandler();
                var menuResult = menuHandler.GetMenusDetails(currentUserType);

                // Manager Calendar is a core workflow.  Add it when the DB menu mapping omits it.
                if (string.Equals(currentUserType, "Manager", StringComparison.OrdinalIgnoreCase) &&
                    !menuResult.Any(x => string.Equals(Convert.ToString(x.Href), "Schedular.aspx", StringComparison.OrdinalIgnoreCase)))
                {
                    menuResult.Add(new MenuViewModel
                    {
                        Id = -100, Seq = 999, Name = "Calendar", Href = "Schedular.aspx",
                        Icon = "fa fa-calendar", SubMenu = new List<MenuViewModel>()
                    });
                }
                BindMenuContent(menuResult.OrderBy(x => x.Seq));
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
        else
        {
            if (Session["UserId"] != null && Session["EmployeeId"] != null && Session["UserTypeid"] != null && Session["UserType"] != null && Session["MotherOrgid"] != null)
            {

            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        //DataTable dtBoMenu = new DataTable();
        //DataTable dtBoMenu1 = new DataTable();
        //DataTable dtAccDetails1 = new DataTable();
        //dtAccDetails1 = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
        //if (dtAccDetails1.Rows.Count > 0)
        //{
        //    if (dtAccDetails1.Rows[0]["Status"].ToString() != "False")
        //    {
        //        dtBoMenu = comm.GetAllDataFromTable("MappingTable", "*", "USERID='" + Session["UserId"].ToString() + "' and UserTypeId=1", "");
        //        if (dtBoMenu.Rows.Count > 0)
        //        {
        //            MenuReportBo.Visible = true;
        //        }
        //        else
        //        {
        //            dtBoMenu1 = comm.GetAllDataFromTable("AssginBussiness", "*", "EmployeeID='" + Session["EmployeeId"].ToString() + "' and UserTypeId=1", "");
        //            if (dtBoMenu1.Rows.Count > 0)
        //            {
        //                if (Session["UserType"].ToString() != "SupportAdmin")
        //                {
        //                    MenuReportBo.Visible = true;
        //                }

        //            }
        //        }
        //    }
        //}
    }
   
    public void GetCompanyName()
    {
        DataTable dt = new DataTable();
        DataTable dt1 = new DataTable();
        DataTable dtuid = new DataTable();
        dt = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Session["MotherOrgid"].ToString() + "", "");
        dtuid = comm.GetAllDataFromTable("UserAcc", "*", "UserId='" + Session["UserId"].ToString() + "'", "");

        if (dtuid.Rows[0]["Profile_Image"].ToString() == "")
        {
            imgProfile.Src = "/Uploads/" + "profile.jpg";
            imgProfile1.Src = imgProfile.Src;
        }
        if (File.Exists(Server.MapPath("~/Uploads/" + dtuid.Rows[0]["Profile_Image"].ToString())))
        {
            imgProfile.Src = "/Uploads/" + dtuid.Rows[0]["Profile_Image"].ToString();
            imgProfile1.Src = imgProfile.Src;
        }
        if (!File.Exists(Server.MapPath("~/Uploads/" + dtuid.Rows[0]["Profile_Image"].ToString())))
        {
            imgProfile.Src = "/Uploads/" + "profile.jpg";
            imgProfile1.Src = imgProfile.Src;
        }
        
        if (Session["EmployeeId"].ToString() != "0")
        {
            if (Session["UserType"].ToString() != "SupportAdmin")
            {
                dt1 = comm.GetAllDataFromTable("AddEmployee Emp left join EmpAddress EmpAdd on emp.ADDRESSID=EmpAdd.EmpAddressId join UserType ua on ua.User_TypeId = Emp.UserType", "*", "Emp.EmployeeID=" + Session["EmployeeId"].ToString() + "", "");
            }
            else
            {
                dt1 = comm.GetAllDataFromTable("SupportAdmin SupA JOIN UserAcc Ua on ua.EmployeeId=SupportAdminId", "ua.Email as MainEmail, *", "SupA.SupportAdminId='" + Session["EmployeeId"].ToString() + "' and UserType='SupportAdmin'", "");
            }

            if (dt1.Rows.Count > 0)
            {
                lblemail.Text = dt1.Rows[0]["MainEmail"].ToString();
                //lbluid.Text = "User Id(" + dtuid.Rows[0]["UID"].ToString() + ")";
                lbluid.Text = dtuid.Rows[0]["UID"].ToString();
                lblname.Text = AnyminuteDisplayText.Tidy(dt1.Rows[0]["FirstName"].ToString() + " " + dt1.Rows[0]["MiddleName"].ToString() + " " + dt1.Rows[0]["LastName"].ToString()); // v27
                lblwelcome.Text = "Welcome! " + AnyminuteDisplayText.Tidy(dt1.Rows[0]["FirstName"].ToString() + " " + dt1.Rows[0]["MiddleName"].ToString() + " " + dt1.Rows[0]["LastName"].ToString());

                string username = lblwelcome.Text;
                Session["user"] = username;
                Session["NameOfLoggedInUser"] = AnyminuteDisplayText.Tidy(dt1.Rows[0]["FirstName"].ToString() + " " + dt1.Rows[0]["MiddleName"].ToString() + " " + dt1.Rows[0]["LastName"].ToString());
                // v58 (Support Admin PPT s1/s2): same header on every page - Main.master and Modern.master both read "Welcome! Support Admin".
                if (Session["UserType"].ToString() == "SupportAdmin") lblwelcome.Text = "Welcome! Support Admin";

                if (Session["UserId_acc"] != null)
                {
                   // BtnBackToAcc.Visible = true;
                }
            }
        }
        else
        {
            dt1 = comm.GetAllDataFromTable("userACC UA JOIN MotherOrg Mo on ua.Email=mo.Email", "*", "mo.MotherOrgID=" + Session["MotherOrgid"].ToString() + " and ua.UserType='ADMIN' AND UA.UserId=" + Session["UserId"].ToString() + "", "");
            if (dt1.Rows.Count > 0)
            {
                lblemail.Text = dt1.Rows[0]["Email"].ToString();
                //lblname.Text = dt1.Rows[0]["UserType"].ToString();
                lblname.Text = dt1.Rows[0]["ContactName"].ToString();
                //lbluid.Text = "User Id(" + dtuid.Rows[0]["UID"].ToString() + ")";
                lbluid.Text = dtuid.Rows[0]["UID"].ToString();
                string MotherOrgName_Seperate = dt1.Rows[0]["MotherOrgName"].ToString();
                string[] MotherOrgName_Seperate_Label = MotherOrgName_Seperate.Split('-');
                lblwelcome.Text = "Welcome! " + MotherOrgName_Seperate_Label[0].ToString();
            }
        }
    }

    public void BindmenuFromLoginTable()
    {
        MenusDomain = cl.GetDomainNameForMenuFromLogin(Session["UserLoginId"].ToString());
        AllMenu = cl.GetMenuDataFromLoginTable(Session["UserLoginId"].ToString());
        DataView Domainview = new DataView(MenusDomain);
        this.rptCategories.DataSource = Domainview;
        this.rptCategories.DataBind();
    }

    protected void rptMenu_OnItemBound(object sender, RepeaterItemEventArgs e)
    {
        string linkis;
        DataTable ParentDt = new DataTable();
        StringBuilder sb = new StringBuilder();
        if (MenusDomain != null)
        {
            DataRowView Domaindrv = e.Item.DataItem as DataRowView;
            string DomainID = Domaindrv["DomainId"].ToString();
            string DomainTitle = Domaindrv["DomainName"].ToString();
            ParentDt = AllMenu.Select("DomainId=" + DomainID).CopyToDataTable();
            if (ParentDt.Rows.Count > 0)
            {
                sb.Append("<div class='collapse' id='" + Domaindrv["DomainName"].ToString() + "'>");
                sb.Append("<ul class='nav nav-collapse'>");
                foreach (DataRow dtRow in ParentDt.Rows)
                {
                    if (dtRow[4].ToString() == string.Empty)
                    {
                        sb.Append("<li><a  href='" + dtRow[4].ToString() + "' style='text-decoration:none;text-transform:uppercase; '>" + dtRow[2].ToString() + "</a>");
                        sb = CreateChildmenu(sb, dtRow[3].ToString(), DomainID, dtRow[2].ToString(), ParentDt);
                        sb.Append("</li>");
                    }
                    else
                    {
                        if (dtRow[1].ToString() == dtRow[6].ToString())
                            sb.Append("<li id><a  href='" + dtRow[4].ToString() + "' style='text-decoration:none;text-transform:uppercase; '>" + dtRow[2].ToString() + "</a>");
                        sb.Append("<ul id=>");

                        sb.Append("</ul>");
                        sb.Append("</li>");
                    }
                }
            }
            sb.Append("</ul>");
            sb.Append("</div>");
            (e.Item.FindControl("ltrlSubMenu") as Literal).Text = sb.ToString();
        }
    }

    public StringBuilder CreateChildmenu(StringBuilder sb, string parentId, string domainid, string parentTitle, DataTable dt)
    {
        DataTable dtchild = new DataTable();
        if (dt.Rows.Count > 0)
        {
            dtchild = dt.Select("DomainId=" + domainid + "and ParentMenuID=" + parentId + "and url<>''").CopyToDataTable();
            foreach (DataRow dtRow in dtchild.Rows)
            {
                sb.Append("<li>");
                sb.Append("<a href=" + dtRow[4].ToString() + " style='text-decoration:none;text-transform:uppercase;'>" + dtRow[2].ToString() + "</a>");
                sb.Append("</li>");
            }
        }
        return sb;
    }

    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        AnyminuteSessionGuard.SignedOut(); // v26, never throws
        System.Web.Security.FormsAuthentication.SignOut();
        Session.Clear();
        Session.Abandon();
        Response.Redirect("Login.aspx");
    }

    protected void LinkProfile_Click(object sender, EventArgs e)
    {
        Session["EmployeeIDEdit"] = Session["EmployeeId"];
        Response.Redirect("UpdateProfile.aspx");
    }

    //protected void BtnBackToAcc_Click(object sender, EventArgs e)
    //{
    //    if (Session["UserId_acc"] != null)
    //    {
    //        Session["UserId"] = Session["UserId_acc"];
    //        Session["UserId_acc"] = null;
    //        Session["EmployeeId"] = Session["EmployeeId_acc"];
    //        Session["EmployeeId_acc"] = null;
    //        Session["email"] = Session["email_acc"];
    //        Session["email_acc"] = null;
    //        Session["MotherOrgid"] = Session["MotherOrgid_acc"];
    //        Session["MotherOrgid_acc"] = null;

    //        Session["UserTypeid"] = Session["UserTypeid_acc"];
    //        Session["UserTypeid_acc"] = null;
    //        Session["UserType"] = Session["UserType_acc"];
    //        Session["UserType_acc"] = null;
    //        if (Session["Page"] == null)
    //        {
    //            Response.Redirect("Accountant.aspx", false);
    //        }
    //        else
    //        {
    //            Response.Redirect("BusinessOwnerlist.aspx", false);
    //        }
    //    }
    //}

    protected void BindMenuContent(IEnumerable<MenuViewModel> menu)
    {
        var permitted = AnyminuteUiNavigation.Load().ToList();
        navigation.InnerHtml = AnyminuteUiNavigation.Render(permitted);
        HeaderHelp.Text = AnyminuteUiNavigation.HeaderHelp(permitted);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if(Session["UserId"] != null)
        {
            var accountMenu=AnyminuteUiNavigation.Load().ToList();
            HeaderHelp.Text=AnyminuteUiNavigation.HeaderHelp(accountMenu);
            AccountUpgrade.Text=AnyminuteUiNavigation.AccountUpgrade(accountMenu);
        }
        string name = lblwelcome.Text.Trim();
        while (name.StartsWith("Welcome!", StringComparison.OrdinalIgnoreCase)) name = name.Substring(8).TrimStart();
        lblwelcome.Text = "Welcome! " + HttpUtility.HtmlEncode(HttpUtility.HtmlDecode(name));
    }

    protected void profile_Click(object sender, EventArgs e)
    {
        if (Session["UserId"] != null && Session["EmployeeId"] != null && Session["UserTypeid"] != null && Session["UserType"] != null && Session["MotherOrgid"] != null)
        {
            if (Session["UserType"].ToString() == "SuperAdmin")
            {
                //profile.HRef = "SuperAdminProfile.aspx";
                profile.PostBackUrl = "~/SuperAdminProfile.aspx";
            }
            else
            {
                if (Session["UserTypeid"].ToString() == "0" && Session["UserType"].ToString() == "Admin")
                {
                    DataTable dtAccDetails = comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID='" + int.Parse(Session["MotherOrgid"].ToString()) + "'", "");
                    if (dtAccDetails.Rows.Count > 0)
                    {
                        if (dtAccDetails.Rows[0]["Status"].ToString() != "False")
                        {
                            //profile.HRef = "AccountantProfile.aspx";
                            profile.PostBackUrl = "~/AccountantProfile.aspx";
                        }
                    }
                }
                else if (Session["email"].ToString() == "support@anyminute.net" && Session["UserTypeid"].ToString() == "1" && Session["UserType"].ToString() == "SupportAdmin")
                {
                    //profile.HRef = "SupportAdminProfile.aspx";
                    profile.PostBackUrl = "~/SupportAdminProfile.aspx";
                }
                else if (Session["UserTypeid"].ToString() == "1" && Session["UserType"].ToString() == "SupportAdmin")
                {
                    //profile.HRef = "SupportAdminProfile.aspx";
                    profile.PostBackUrl = "~/SupportAdminProfile.aspx";
                }
                else
                {
                    //profile.PostBackUrl = "~/MyProfile.aspx";
                    Response.Redirect("MyProfile.aspx");
                }
            }
        }
        else
        {
            Response.Redirect("Login.aspx");
        }
    }
}