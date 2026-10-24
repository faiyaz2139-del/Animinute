using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CreateDomain : System.Web.UI.Page
{
    CommanClass Comm = new CommanClass();
    CreateLinkClass Cl = new CreateLinkClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            GetDomainList();
        }
    }

    public void GetDomainList()
    {
        Comm.FillGridView(GVDomainList, "DomainMaster", "DomainFor,DomainName,Icon,OrderID", "DomainID", "", "OrderID");
    }
    protected void GVDomainList_PreRender(object sender, EventArgs e)
    {
        if (GVDomainList.Rows.Count > 0)
        {
            GVDomainList.UseAccessibleHeader = true;
            GVDomainList.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {

        int result = 0;
        bool cl = false;
        if (btnsave.Text == "Save")
        {
            cl = Comm.IfExistsInTable("DomainMaster", "DomainName", "DomainName='" + txtDomainName.Text + "'");
            if (cl == true)
            {
                this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('warning', '" + txtDomainName.Text + "  Already Exists !!!', 'warning');", true);
                return;
            }
            Cl.InsertDomainLink(txtDomainName.Text, int.Parse(txtOrder_Id.Text), "1", txticon.Text);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Saved!', 'success');", true);
            GetDomainList();
            Clear();
        }
        else
        {
            Cl.UpdateDomainLink(txtDomainName.Text, int.Parse(txtOrder_Id.Text), int.Parse(hfdEditId.Value), "1", txticon.Text);
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('success!', 'Record Updated!', 'success');", true);
            GetDomainList();
            Clear();
        }
    }

    protected void btnedit_Click(object sender, EventArgs e)
    {
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdid = (HiddenField)GVDomainList.Rows[gvr.RowIndex].FindControl("hfdid");
        Label lblDomainName = (Label)GVDomainList.Rows[gvr.RowIndex].FindControl("lblDomainName");
        HiddenField hdfOrderId = (HiddenField)GVDomainList.Rows[gvr.RowIndex].FindControl("hdfOrderId");
        HiddenField hdfIcon = (HiddenField)GVDomainList.Rows[gvr.RowIndex].FindControl("hdfIcon");
        hfdEditId.Value = hfdid.Value;
        txtDomainName.Text = lblDomainName.Text;
        txtOrder_Id.Text = hdfOrderId.Value;
        txticon.Text = hdfIcon.Value;
        btnsave.Text = "Update";
    }
    public void Clear()
    {
        hfdEditId.Value = "";
        txtDomainName.Text = "";
        txtOrder_Id.Text = "";
        txticon.Text = "";
        btnsave.Text = "Save";
    }
}