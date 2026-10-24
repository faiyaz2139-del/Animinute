using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using log4net;
using System.Data.SqlClient;

public partial class Billing_Info : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    EmailSettingController Esctrl = new EmailSettingController();
    MotherOrgControllerClass MotherC = new MotherOrgControllerClass();
    MotherOrgEntityClass MotherEc = new MotherOrgEntityClass();
    SupportAdminCtrl supportAdminCtrl = new SupportAdminCtrl();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            Get_Subscriber_Name_List();
        }
    }

    ILog Logger = log4net.LogManager.GetLogger("Log");
    object loginfo;
    private SqlConnection con;
    public void getconnection()
    {
        string strcon = ConfigurationManager.ConnectionStrings["ConString"].ToString();
        con = new SqlConnection(strcon);
        if (con.State == ConnectionState.Open)
        {
            con.Close();
        }
        con.Open();
    }

    protected void GridView_Billing_Info_PreRender(object sender, EventArgs e)
    {
        if (GridView_Billing_Info.Rows.Count > 0)
        {
            GridView_Billing_Info.UseAccessibleHeader = true;
            GridView_Billing_Info.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void GridView_Billing_Info_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField HiddenField_IsActive_Status_ = (e.Row.FindControl("HiddenField_IsActive_Status") as HiddenField);
            HyperLink HyperLink_Edit_Plan_ = (e.Row.FindControl("HyperLink_Edit_Plan") as HyperLink);
            Label Label_No_Of_Business_ = (e.Row.FindControl("Label_No_Of_Business") as Label);
            HiddenField HiddenField_Is_Requested_Plan_ = (e.Row.FindControl("HiddenField_Is_Requested_Plan") as HiddenField);

            DataTable DataTable_GetCountIfSubscriber1stTimeRequestedPlan = Comm.GetAllDataFromTable("PlanHistory", "Id", "EmpId = '" + ddl_Subscriber_Name.SelectedValue + "' ", "Id desc");
            if (DataTable_GetCountIfSubscriber1stTimeRequestedPlan.Rows.Count == 1 && HiddenField_Is_Requested_Plan_.Value == "True")
            {
                HyperLink_Edit_Plan_.Visible = true;
            }
            else
            {
                if (HiddenField_IsActive_Status_.Value == "False" || HiddenField_IsActive_Status_.Value == "")
                {
                    HyperLink_Edit_Plan_.Visible = false;
                }

                DataTable Dt_GetCountIfSubscriberResellerNotEnteredUsersBusinesses = Comm.GetAllDataFromTable("PlanHistory", "Id", "EmpId = '" + ddl_Subscriber_Name.SelectedValue + "' " +
                    "and No_Of_Users IS NULL ", "Id desc");
                if (Dt_GetCountIfSubscriberResellerNotEnteredUsersBusinesses.Rows.Count == 1)
                {
                    HyperLink_Edit_Plan_.Visible = true;
                }

                if (Label_No_Of_Business_.Text == "10000")
                {
                    Label_No_Of_Business_.Text = "Unlimited Business";
                }
            }

            Label SubName_ = (e.Row.FindControl("Label_Subscriber_Name") as Label);
            Label Label_Email_Id_ = (e.Row.FindControl("Label_Email_Id") as Label);

            DataTable Dt_Check_If_Reseller = Comm.GetAllDataFromTable("Register_Partner with (Nolock)", "EMail_primary", "EMail_primary = '" + Label_Email_Id_.Text.ToString() + "'", "");
            if (Dt_Check_If_Reseller.Rows.Count > 0)
            {
                DataTable Dt_GetActualResellerEmailName = Comm.GetAllDataFromTable("MotherOrg with (Nolock)", "ContactName", "Email = '" + Label_Email_Id_.Text.ToString() + "'", "");
                if (Dt_GetActualResellerEmailName.Rows.Count > 0)
                {
                    SubName_.Text = Dt_GetActualResellerEmailName.Rows[0]["ContactName"].ToString();
                }
            }
            if (Dt_Check_If_Reseller.Rows.Count == 0)
            {
                DataTable Dt_GetSubscriberName = Comm.GetAllDataFromTable("AddEmployee", "(FirstName +' '+ MiddleName +' '+LastName) as SubName", "EmployeeID = '" + int.Parse(ddl_Subscriber_Name.SelectedValue) + "' and UserType = 1", "");
                SubName_.Text = Dt_GetSubscriberName.Rows[0]["SubName"].ToString();
            }
        }
    }

    public void Get_Subscriber_Name_List()
    {
        DataTable DataTable_Subscriber_Reseller_List = Get_Subscriber_Reseller_List();
        ddl_Subscriber_Name.Items.Clear();
        ddl_Subscriber_Name.Items.Add(new ListItem("Select Subscriber", ""));
        ddl_Subscriber_Name.DataSource = DataTable_Subscriber_Reseller_List;
        ddl_Subscriber_Name.DataValueField = DataTable_Subscriber_Reseller_List.Columns[1].ToString();
        ddl_Subscriber_Name.DataTextField = DataTable_Subscriber_Reseller_List.Columns[2].ToString();
        ddl_Subscriber_Name.DataBind();
    }

    public DataTable Get_Subscriber_Reseller_List()
    {
        try
        {
            loginfo = "Get_Subscriber_Reseller_List() Start";
            Logger.Info(loginfo);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter();
            getconnection();
            SqlCommand cmd = new SqlCommand();

            //commented because the proc SP_Get_Subscriber_Reseller_List calling extra fields and was copy of supportadmin home page
            //cmd.CommandText = "SP_Get_Subscriber_Reseller_List";
            cmd.CommandText = "SP_Get_Subscriber_Reseller_List_Billing_Info_Page";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Connection = con;
            da.SelectCommand = cmd;
            da.Fill(dt);

            loginfo = "Get_Subscriber_Reseller_List() end";
            Logger.Info(loginfo);

            return dt;
        }
        catch (Exception ex)
        {
            Logger.Error("Get_Subscriber_Reseller_List" + ex.Message);
            throw ex;
        }
    }

    protected void ddl_Subscriber_Name_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddl_Subscriber_Name.SelectedValue != "")
        {
            //giving error in subscriber and reseller name
            //DataTable DataTable_Subscriber_Plan_History = Comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*, dbo.FN_DESC('AddEmployee',EmpId)SubName", "EmpId = '" + ddl_Subscriber_Name.SelectedValue + "' ", "Id desc");
            DataTable DataTable_Subscriber_Plan_History = Comm.GetAllDataFromTable("PlanHistory with (Nolock)", "*, '' as SubName", "EmpId = '" + ddl_Subscriber_Name.SelectedValue + "' ", "Id desc");
            if (DataTable_Subscriber_Plan_History.Rows.Count > 0)
            {
                GridView_Billing_Info.DataSource = DataTable_Subscriber_Plan_History;
                GridView_Billing_Info.DataBind();
            }
            if (DataTable_Subscriber_Plan_History.Rows.Count == 0)
            {
                GridView_Billing_Info.DataSource = null;
                GridView_Billing_Info.DataBind();
            }
        }
        else
        {
            GridView_Billing_Info.DataSource = null;
            GridView_Billing_Info.DataBind();
        }
    }
}
