using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EmailTemplate : System.Web.UI.Page
{
    EmailSettingEntity EsE = new EmailSettingEntity();
    EmailSettingController EsCtrl = new EmailSettingController();
    CommanClass comm = new CommanClass();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            comm.FillDropDownList(ddlTemplateType, "TempleteType", "ID", "TemplateName", "", "");
            comm.FillGridView(GVEmailTemp, "TemplateMaster tm join TempleteType tt on tm.TempType=tt.ID", "TempID,tt.TemplateName as TemplateType", "TempName", "", "");
        }
    }

    protected void GVEmailTemp_PreRender(object sender, EventArgs e)
    {
        if (GVEmailTemp.Rows.Count > 0)
        {
            GVEmailTemp.UseAccessibleHeader = true;
            GVEmailTemp.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        int a = 0;
        EsE.TempType = int.Parse(ddlTemplateType.SelectedValue);
        EsE.TempName = txtpageName.Text;
        EsE.Subject = txtSubject.Text;
        EsE.description = txtDescription.Text;
        a = EsCtrl.InsertEmailTemplateMater(EsE);
        GetClear();
        comm.FillGridView(GVEmailTemp, "TemplateMaster tm join TempleteType tt on tm.TempType=tt.ID", "TempID,tt.TemplateName as TemplateType", "TempName", "", "");
        if (a > 0)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Record Save Successfully', 'success');", true);
        }
    }

    protected void btneditaction_Click(object sender, EventArgs e)
    {
        //int a = 0;
        LinkButton lnkbtn = (LinkButton)sender;
        GridViewRow gvr = (GridViewRow)lnkbtn.NamingContainer;
        HiddenField hfdTicketid = (HiddenField)GVEmailTemp.Rows[gvr.RowIndex].FindControl("hfd_TicketId");
        comm.ExecuteQuery("Delete from TemplateMaster where TempID=" + hfdTicketid.Value + "");

        comm.FillGridView(GVEmailTemp, "TemplateMaster tm join TempleteType tt on tm.TempType=tt.ID", "TempID,tt.TemplateName as TemplateType", "TempName", "", "");
        this.ClientScript.RegisterStartupScript(this.GetType(), "SweetAlert", "swal('Success!', 'Record Deleted Successfully', 'success');", true);
        GetClear();
    }

    public void GetClear()
    {
        txtpageName.Text = string.Empty;
        ddlTemplateType.SelectedValue = "";
    }
}