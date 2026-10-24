using System;
using System.Web;
using System.Web.UI;

public static class AnyminuteCalendarNavigation
{
    // Normalize legacy manager navigation before either calendar reads query strings.
    public static void NormalizeManager(Page page)
    {
        if (Convert.ToString(page.Session["UserTypeid"]) != "2") return;
        var businesses = AnyminuteDirectoryScope.Businesses();
        if (businesses.Rows.Count == 0) throw new HttpException(403, "No active business is assigned to this manager.");
        var managerBusinesses = businesses.Clone();
        foreach (System.Data.DataRow row in businesses.Rows)
        {
            int identity;
            using (var c = AnyminuteTimesheetRepair.Open())
                if (AnyminuteDirectoryScope.BusinessRole(c, Convert.ToInt32(row["BusinessID"]), out identity) == 2)
                    managerBusinesses.ImportRow(row);
        }
        businesses = managerBusinesses;
        if (businesses.Rows.Count == 0) throw new HttpException(403, "No active manager assignment is available.");
        string requested = page.Request.QueryString["BusinessIdFromDefaultPage"];
        int business;
        if (String.IsNullOrEmpty(requested)) business = Convert.ToInt32(businesses.Rows[0]["BusinessID"]);
        else if (!Int32.TryParse(requested, out business) || business <= 0 ||
            businesses.Select("BusinessID=" + business).Length != 1)
            throw new HttpException(403, "This business is outside your access.");
        int actor, role;
        using (var c = AnyminuteTimesheetRepair.Open()) role = AnyminuteDirectoryScope.BusinessRole(c, business, out actor);
        if (role != 2) throw new HttpException(403, "The selected business is not a manager assignment.");
        if (requested != business.ToString() || page.Request.QueryString["MangerId"] != actor.ToString() ||
            page.Request.QueryString["UserRole"] != "Manager")
        {
            // Use only whitelisted page names, resolved business and persisted actor IDs.
            string file = System.IO.Path.GetFileName(page.Request.FilePath);
            if (file != "Schedular.aspx" && file != "Dashboard_Schedular.aspx")
                throw new HttpException(400, "Unsupported calendar route.");
            page.Response.Redirect(file + "?BusinessIdFromDefaultPage=" + business + "&UserRole=Manager&MangerId=" + actor);
        }
    }
}
