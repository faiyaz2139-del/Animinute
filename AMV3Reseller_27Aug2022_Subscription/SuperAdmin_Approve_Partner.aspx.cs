using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class SuperAdmin_Approve_Partner : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Get_New_Account_Partner();
        }
    }

    public void Get_New_Account_Partner()
    {        
        Comm.FillGridView(GridView_Partner_List, "Register_Partner", "*", "FirstName+' '+MiddleName+' '+LastName as PartnerName", "", "PartnerID desc");
    }

    protected void GridView_Partner_List_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_Partner_List.PageIndex = e.NewPageIndex;
        Get_New_Account_Partner();
    }

    protected void GridView_Partner_List_PreRender(object sender, EventArgs e)
    {
        if (GridView_Partner_List.Rows.Count > 0)
        {
            GridView_Partner_List.UseAccessibleHeader = true;
            GridView_Partner_List.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    protected void GridView_Partner_List_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Active_Deactive_Partner")
        {
            string status_change_confirm_value = Request.Form["Toggle_activate_deactivate"];
            if (status_change_confirm_value == "Yes" || status_change_confirm_value == "null" || status_change_confirm_value == null)
            {

            }
            else
            {
                return;
            }

            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField HiddenField_Partner_IsActive_Status_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_Partner_IsActive_Status");
            HiddenField HiddenField_Partner_Id_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_Partner_Id");

            Label Label_Partner_Name_ = (Label)GridView_Partner_List.Rows[index].FindControl("Label_Partner_Name");

            HiddenField HiddenField_HireDate_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_HireDate");
            HiddenField HiddenField_EMail_primary_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_EMail_primary");
            HiddenField HiddenField_MobilePhone_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_MobilePhone");
            HiddenField HiddenField_ProfilePic_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_ProfilePic");
            HiddenField HiddenField_CompanyName_ = (HiddenField)GridView_Partner_List.Rows[index].FindControl("HiddenField_CompanyName");


            if (HiddenField_Partner_IsActive_Status_.Value != string.Empty)
            {
                //MotherEc.MotherOrgName = Label_Partner_Name_.Text+'-'+HiddenField_Partner_Id_.Value;   // kumar commented
                MotherEc.MotherOrgName = HiddenField_CompanyName_.Value + '-' + HiddenField_Partner_Id_.Value;
                MotherEc.StreetNumber = "";
                MotherEc.StreetName = "";
                MotherEc.Apartment = "";
                MotherEc.City = "";
                MotherEc.Province = "";
                MotherEc.PostalCode = "";
                MotherEc.EnrollDate = DateTime.Parse(HiddenField_HireDate_.Value);
                MotherEc.Status = true;
                MotherEc.Email = HiddenField_EMail_primary_.Value;
                Send_Email_Id = HiddenField_EMail_primary_.Value;
                MotherEc.ContactName = Label_Partner_Name_.Text;
                MotherEc.ContactNumber = HiddenField_MobilePhone_.Value;
                MotherEc.StreetNumber = HiddenField_ProfilePic_.Value;
                MotherEc.Checked = "N";
                int MotherOrgId = MotherC.InsertMotherOrg(MotherEc);
                if (HiddenField_Partner_IsActive_Status_.Value == "NO        ")
                {
                    Comm.ExecuteQuery("update Register_Partner set IsActive='YES' where PartnerID='" + int.Parse(HiddenField_Partner_Id_.Value) + "'");
                    SendEmail(HiddenField_EMail_primary_.Value, MotherOrgId.ToString());
                    this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Reseller is activated and email has been sent successfully!', 'success');", true);
                }
                Get_New_Account_Partner();
            }
        }
    }

    protected void GridView_Partner_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ImageButton_Active_Deactive_Partner = (e.Row.FindControl("ImageButton_Active_Deactive_Partner") as ImageButton);
            HiddenField HiddenField_Partner_IsActive_Status = (e.Row.FindControl("HiddenField_Partner_IsActive_Status") as HiddenField);
            if (HiddenField_Partner_IsActive_Status.Value == "YES       ")
            {
                ImageButton_Active_Deactive_Partner.ImageUrl = "~/images/active.png";
            }
            else
            {
                ImageButton_Active_Deactive_Partner.ImageUrl = "~/images/deactive.png";
            }
        }
    }

    string Send_Email_Id;
    public void SendEmail(string email, string Motherid)
    {
        string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
        string url = Host + Motherid.ToString() + "&LoginType=Acc";
        DataTable dtEmailTemp = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=1", "");
        DataTable dtemp = Comm.GetAllDataFromTable("MotherOrg", "*", "MotherOrgID=" + Motherid + "", "");
        DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + Send_Email_Id.ToString() + "' ", "");
        string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["ContactName"].ToString(), "Reset Password", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), Send_Email_Id.ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
        //Esctrl.SendHtmlFormattedEmail(email, dtEmailTemp.Rows[0]["Subject"].ToString(), body);
    }
}