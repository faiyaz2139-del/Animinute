using System;
using System.Data;

// 2026-10-24 v10: postal-code lookup for the Add / Update Business address.
// Legacy Sp_SearchAddress only matched AddressMaster with the exact spelling of the postal code
// ("M1B2W1" vs "M1B 2W1"), so many searches showed "No matching addresses found".
// This read-only query matches both spellings and also offers street/city/province combinations
// already saved in EmpAddress (v46: and BusinessAddress) for that postal code (no street numbers, suites or contact details).
public static class AnyminuteAddressLookup
{
    public static DataTable Search(string postal)
    {
        string compact = AnyminuteBusinessAddressInput.Postal(postal);
        string spaced = compact.Length == 6 ? compact.Substring(0, 3) + " " + compact.Substring(3) : compact;
        try
        {
            using (var c = AnyminuteTimesheetRepair.Open())
                return AnyminuteTimesheetRepair.Query(c, null,
                    "SELECT StreetName, City, Province, StreetName + ', ' + City + ', ' + Province AS SEARCHADDRESS FROM (" +
                    " SELECT UPPER(LTRIM(RTRIM(a.Street_Name))) AS StreetName, UPPER(LTRIM(RTRIM(ci.CityName))) AS City, UPPER(LTRIM(RTRIM(p.ProvinceName))) AS Province" +
                    " FROM AddressMaster a JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50)) = CAST(a.City AS nvarchar(50))" +
                    " JOIN ProvinceMaster p ON CAST(p.ProvinceId AS nvarchar(50)) = CAST(a.Province AS nvarchar(50))" +
                    " WHERE a.PostalCode IN (@p, @s)" +
                    " UNION SELECT UPPER(LTRIM(RTRIM(e.StreetName))), UPPER(LTRIM(RTRIM(e.City))), UPPER(LTRIM(RTRIM(e.Province)))" +
                    " FROM EmpAddress e WHERE e.Postalcode IN (@p, @s) OR REPLACE(REPLACE(UPPER(LTRIM(RTRIM(e.Postalcode))), ' ', ''), '-', '') = @p" +
                    // v46 (paused deck s3): employee searches found nothing for addresses that only exist on saved businesses.
                    " UNION SELECT UPPER(LTRIM(RTRIM(b.StreetName))), UPPER(LTRIM(RTRIM(b.City))), UPPER(LTRIM(RTRIM(b.Province)))" +
                    " FROM BusinessAddress b WHERE b.Postalcode IN (@p, @s) OR REPLACE(REPLACE(UPPER(LTRIM(RTRIM(b.Postalcode))), ' ', ''), '-', '') = @p) x" +
                    " WHERE ISNULL(StreetName, '') <> '' AND ISNULL(City, '') <> '' AND ISNULL(Province, '') <> '' ORDER BY StreetName, City",
                    "@p", compact, "@s", spaced);
        }
        catch (Exception error)
        {
            log4net.LogManager.GetLogger("Log").Error("Address lookup query failed; using Sp_SearchAddress", error);
            return new CommanClass().GetSearchAddress(compact);
        }
    }
    // v46 (paused deck s1): the only message shown when a search finds nothing - an "Address Not Found" card.
    public static string NotFoundHtml(string postal)
    {
        string p = AnyminuteBusinessAddressInput.Postal(postal);
        if (p.Length == 6 && System.Text.RegularExpressions.Regex.IsMatch(p, "^[A-Z]\\d[A-Z]\\d[A-Z]\\d$")) p = p.Substring(0, 3) + " " + p.Substring(3);
        return "<div class=\"am-al-nf\" role=\"alert\"><div class=\"am-al-nf-head\"><span class=\"am-al-nf-i\" aria-hidden=\"true\">!</span><strong>Address Not Found</strong></div>" +
            "<p>No matching address was found for postal code <b>" + System.Web.HttpUtility.HtmlEncode(p) + "</b>. Please verify the postal code and search again, or enter the address manually.</p></div>";
    }

}
