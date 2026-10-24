using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//21-05-22
using System.Configuration;
using log4net;
using System.Data.SqlClient;
using System.Globalization;

public partial class SearchAddress : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    SearchAddressController objSA = new SearchAddressController();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!Page.IsPostBack)
        {
            Get_New_Account_Business_Owner(); 
        }
    }

    //21-5-22
    public void Get_New_Account_Business_Owner()
    {
        try
        {

       
       
           
        DataTable DataTable_Subscriber_Reseller_List = objSA.GetSearchData(txtstartdate.Text, txtenddate.Text);
        GridView_New_Registered_BO.DataSource = DataTable_Subscriber_Reseller_List;
        GridView_New_Registered_BO.DataBind();
     
        }
        catch (Exception ex)
        {

            
        }
    }

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
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Get_New_Account_Business_Owner();
    }
    protected void btndSave_Click(object sender, EventArgs e)
    {
        LinkButton btn = sender as LinkButton;
        int id = int.Parse(btn.CommandArgument);
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Address Has been changed', 'success');", true);

    }
    protected void btnEdit_Click(object sender, EventArgs e)
    {
        Postalcode.Text = "";
        City.Text = "";
        StreetName.Text = "";
        Province.Text = "";
        LinkButton btn = sender as LinkButton;
        int id = int.Parse(btn.CommandArgument);
        Postalcode.Text = (GridView_New_Registered_BO.Rows[id].FindControl("Postalcode") as Label).Text;
        City.Text = (GridView_New_Registered_BO.Rows[id].FindControl("City") as Label).Text;
        StreetName.Text = (GridView_New_Registered_BO.Rows[id].FindControl("StreetName") as Label).Text;
        Province.Text = (GridView_New_Registered_BO.Rows[id].FindControl("Province") as Label).Text;
        chkFinal.Checked = (GridView_New_Registered_BO.Rows[id].FindControl("ChkCheck") as CheckBox).Checked;
        ModalPopupEmpList.Show();
    }
    protected void btnFinalSave_Click(object sender, EventArgs e)
    {
        if(chkFinal.Checked&& Postalcode.Text.Trim()!="" && StreetName.Text.Trim()!="" && City.Text.Trim()!="" && Province.Text.Trim() != "")
        {
          int rw=  objSA.InsertAddressDetails(Postalcode.Text, StreetName.Text, City.Text, Province.Text);
            if (rw > 0)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Address Has been changed', 'success');", true);
            }
            else
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Error!', 'Some error found while updating', 'error');", true);
            }
            Get_New_Account_Business_Owner();
        }
        else
        { ModalPopupEmpList.Show();
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Warning!', 'Please fill manadatory field', 'success');", true);
           
        }
    }
    protected void GridView_New_Registered_BO_PreRender(object sender, EventArgs e)
    {
        if (GridView_New_Registered_BO.Rows.Count > 0)
        {
            GridView_New_Registered_BO.UseAccessibleHeader = true;
            GridView_New_Registered_BO.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    //end
}