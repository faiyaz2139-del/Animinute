using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class CreateLink : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    CreateLinkClass Cl = new CreateLinkClass();
    DataTable dt = new DataTable();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetDomainList();

        }

    }

    public void GetDomainList()
    {
        Comm.FillDropDownList(ddlDomainName, "DomainMaster", "DomainID", "DomainName", "", "DomainID");
    }

    public void getParentLink()
    {
        dt = Cl.GetParentLink(ddlDomainName.SelectedValue);
        if (dt.Rows.Count > 0)
        {
            ddlParent.DataSource = dt;
            ddlParent.DataTextField = "Title";
            ddlParent.DataValueField = "MenuID";
            ddlParent.DataBind();
            ddlParent.Items.Insert(0, new ListItem("Please Select", "0"));
        }

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (btnSave.Text == "Save")
        {
            string flag;
            bool check = false;
            check = Comm.IfExistsInTable("LinkMaster", "Title", "DomainId='" + ddlDomainName.SelectedValue + "' and Title='" + txtname.Text + "'");
            if (check == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning!', '" + txtname.Text + " Already Exists', 'warning');", true);
                return;
            }
            flag = btnSave.Text;

            int? MenuidEdit = string.IsNullOrEmpty(hfdMenuidEdit.Value) ? (int?)null : int.Parse(hfdMenuidEdit.Value);
            int? mastno = string.IsNullOrEmpty(ddlParent.SelectedValue) ? (int?)null : int.Parse(ddlParent.SelectedValue);
            Cl.InsertLinkClass(Convert.ToInt32(MenuidEdit), Convert.ToInt32(ddlParent.SelectedValue), txtname.Text, txtUrl.Text, txticon.Text, flag, Convert.ToDecimal(txtmenuslno.Text), Convert.ToInt32(ddlDomainName.SelectedValue), Convert.ToInt32(mastno), "1");
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved!', 'success');", true);
            GetGridFill();
        }
    }

    protected void GVCreateLinkList_PreRender(object sender, EventArgs e)
    {
        if (GVCreateLinkList.Rows.Count > 0)
        {
            GVCreateLinkList.UseAccessibleHeader = true;
            GVCreateLinkList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    public void GetGridFill()
    {
        Comm.FillGridView(GVCreateLinkList, "LinkMaster lm join DomainMaster Dm on lm.DomainId=dm.DomainID", "dm.DomainID, dm.DomainName,lm.Title,lm.Url,lm.MenuSlno,lm.MenuID,lm.ParentMenuID,lm.mastno", "lm.LinkFor", " dm.DomainID=" + ddlDomainName.SelectedValue + "", "lm.MenuSlno");
    }




    protected void ddlDomainName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (btnSave.Text != "Update")
        {
            getParentLink();
        }
        GetGridFill();
    }


    protected void btnDelete_Click(object sender, EventArgs e)
    {

        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hdfMenuID = (HiddenField)GVCreateLinkList.Rows[gvr.RowIndex].FindControl("hdfMenuID");
        Cl.DeleteLink(int.Parse(hdfMenuID.Value));
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Deleted!', 'success');", true);
        GetGridFill();
    }

    protected void btnyes_Click(object sender, EventArgs e)
    {

    }
}