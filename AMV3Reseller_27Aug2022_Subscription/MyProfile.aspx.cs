using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class MyProfile : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    MyProfileController ProfileCtrl = new MyProfileController();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["EmployeeId"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        // v58 (Support Admin PPT s2/s3): "My profile" for a Support Admin opens the editable Support Admin profile, not the
        // business-profile list (which is always empty for that role).
        if (AnyminuteSupportAdmin.IsSupportAdminOnly()) { Response.Redirect("SupportAdminProfile.aspx", true); return; }
        if (!IsPostBack)
        {
            string aa = Session["UserId"].ToString();
            GetBusinessLis(int.Parse(aa));            
        }
    }

    public void GetBusinessLis(int UserId)
    {
        DataTable dt = new DataTable();
        dt = ProfileCtrl.GetProfileBusiness(UserId, Session["email"].ToString());
        GvBusiness.DataSource = dt;
        GvBusiness.DataBind();
    }

    protected void GvBusiness_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hfdUserType = (e.Row.FindControl("hfdUserType") as HiddenField);
            HiddenField hdfEmployeeID = (e.Row.FindControl("hdfEmployeeID") as HiddenField);
            HyperLink hypditdivAdmin = (e.Row.FindControl("hypditdivAdmin") as HyperLink);
            if(hfdUserType.Value=="1")
            {
                hypditdivAdmin.NavigateUrl = "~/BusinessOwnerProfile.aspx?ProfileEmployeeId="+ hdfEmployeeID.Value;
            }
            else
            {
                hypditdivAdmin.NavigateUrl = "~/EmployeeProfile.aspx?ProfileEmployeeId=" + hdfEmployeeID.Value;
            }
        }
    }

    protected void GvBusiness_PreRender(object sender, EventArgs e)
    {
        if (GvBusiness.Rows.Count > 0)
        {
            GvBusiness.UseAccessibleHeader = true;
            GvBusiness.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
}