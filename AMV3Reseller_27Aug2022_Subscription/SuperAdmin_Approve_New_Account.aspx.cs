using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;

public partial class SuperAdmin_Approve_New_Account : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            Get_New_Account_Business_Owner();
        }
    }
    
    public void Get_New_Account_Business_Owner()
    {
        Comm.FillGridViewDataNull(GridView_New_Registered_BO, "AddEmployee ae join UserAcc ua on ua.EmployeeId=ae.EmployeeID and ua.MotherOrgid=ae.MotherOrgID and ae.EMPLOYEETYPE='NEW_AC_BO' and ae.MotherOrgID='4' left join MotherOrg mo on mo.MotherOrgID=ae.MotherOrgID", "ae.*,CONVERT(nvarchar(10), HireDate,120)as EnrollDate1,mo.MotherOrgName,ae.FirstName+' '+ae.LastName as EmpName", "ua.Email,convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password ", "", "ae.EmployeeID desc");
    }

    protected void GridView_New_Registered_BO_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView_New_Registered_BO.PageIndex = e.NewPageIndex;
        Get_New_Account_Business_Owner();
    }

    protected void GridView_New_Registered_BO_PreRender(object sender, EventArgs e)
    {
        if (GridView_New_Registered_BO.Rows.Count > 0)
        {
            GridView_New_Registered_BO.UseAccessibleHeader = true;
            GridView_New_Registered_BO.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_New_Registered_BO_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "Activate_Deactivate_New_Account_Business_Owner")
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
            HiddenField HiddenField_Business_Owner_IsActive_Status = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Business_Owner_IsActive_Status");
            HiddenField HiddenField_Employee_Id = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Employee_Id");

            if (HiddenField_Business_Owner_IsActive_Status.Value != string.Empty)
            {
                //if (HiddenField_Business_Owner_IsActive_Status.Value == "YES       ")
                //{
                //    Comm.ExecuteQuery("update AddEmployee set IsActive='NO' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");
                //}
                //else
                //{
                //    Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");
                //}

                if (HiddenField_Business_Owner_IsActive_Status.Value == "NO        ")                
                {
                    Comm.ExecuteQuery("update AddEmployee set IsActive='YES' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");

                    Comm.ExecuteQuery("update UserAcc set Is_First_Time_Login='True' where EmployeeID='" + HiddenField_Employee_Id.Value + "'");
                }

                DataTable dtemp = Comm.GetAllDataFromTable("EmpAddress eAdd join AddEmployee ae on eAdd.EmpAddressId=ae.ADDRESSID", "*", "ae.EmployeeID='" + HiddenField_Employee_Id.Value + "'", "ae.EmployeeID desc");
                if (dtemp.Rows.Count > 0)
                {
                    string Host = ConfigurationManager.AppSettings["ResetUrl_New_Account"];
                    string url = Host + dtemp.Rows[0]["EmployeeID"].ToString();
                    DataTable dtEmailTemp = Comm.GetAllDataFromTable("TemplateMaster", "*", "TempType=3", "");
                    if (dtEmailTemp.Rows.Count > 0)
                    {
                        DataTable _Old_Password = Comm.GetAllDataFromTable("UserAcc", "convert(nvarchar(max),DecryptByPassPhrase('MD2',convert (varbinary(200) ,Password ,2),2 ))Password,uid", "Email = '" + dtemp.Rows[0]["MainEmail"].ToString() + "' ", "");
                        string body = Esctrl.PopulateBodyWithUrl(dtemp.Rows[0]["FirstName"].ToString(), "Click to login Anyminute.net", url, dtEmailTemp.Rows[0]["description"].ToString(), dtEmailTemp.Rows[0]["TempName"].ToString(), dtemp.Rows[0]["MainEmail"].ToString() + " <br />User Id - " + _Old_Password.Rows[0]["uid"].ToString(), _Old_Password.Rows[0]["Password"].ToString());
                        //Esctrl.SendHtmlFormattedEmail(dtemp.Rows[0]["MainEmail"].ToString(), dtEmailTemp.Rows[0]["Subject"].ToString(), body);
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Company is activated and email has been sent successfully', 'success');", true);
                        //this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Business Owner Has Been Activated Successfully', 'success');", true);
                    }
                    else
                    {
                        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', 'Email template is not found in dataBase', 'warning');", true);
                    }
                }
                Get_New_Account_Business_Owner();
            }
        }
        if (e.CommandName == "Delete_Partner")
        {
            string Delete_Partner_Confirm_Value = Request.Form["Confirm_delete_partner"];
            if (Delete_Partner_Confirm_Value == "Yes" || Delete_Partner_Confirm_Value == "null" || Delete_Partner_Confirm_Value == null)
            {

            }
            else
            {
                return;
            }

            int index = Convert.ToInt32(e.CommandArgument);
            HiddenField HiddenField_Employee_Id = (HiddenField)GridView_New_Registered_BO.Rows[index].FindControl("HiddenField_Employee_Id");
            Comm.ExecuteQuery("delete from AddEmployee where EmployeeID='" + HiddenField_Employee_Id.Value + "'");
            Comm.ExecuteQuery("delete from UserAcc where EmployeeID='" + HiddenField_Employee_Id.Value + "'");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Record has been deleted successfully', 'success');", true);
            Get_New_Account_Business_Owner();
        }
    }

    protected void GridView_New_Registered_BO_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            ImageButton ImageButton_Active_Deactive = (e.Row.FindControl("ImageButton_Active_Deactive") as ImageButton);
            HiddenField HiddenField_Business_Owner_IsActive_Status = (e.Row.FindControl("HiddenField_Business_Owner_IsActive_Status") as HiddenField);
            if (HiddenField_Business_Owner_IsActive_Status.Value == "YES       ")
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/active.png";
            }
            else
            {
                ImageButton_Active_Deactive.ImageUrl = "~/images/deactive.png";
            }
        }
    }
}