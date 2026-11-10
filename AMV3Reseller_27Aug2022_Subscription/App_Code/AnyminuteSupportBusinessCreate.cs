using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

// 2026-11-03 v57 (Subscriber Details PPT s1 "it should let me add"): Support Admin / Reseller Add Business for a subscriber that is
// under its plan business limit. Same stored procedures and order as the owner's Add Business (AnyminuteBusinessSave.Save):
// Sp_InsertBusiness (Approved A, Status 1) -> Sp_InsertAssginBussiness (owner, UserTypeId 1) -> owner MappingTable business ->
// Sp_InsertBusinessAddress (+ optional BusinessAddress.Country), on the caller's connection / transaction, for the given owner
// (no owner session needed). The plan limit is re-checked inside the transaction.
public static class AnyminuteSupportBusinessCreate
{
    static SqlCommand Proc(SqlConnection c, SqlTransaction tx, string name, IDictionary<string, object> values)
    {
        var cmd = new SqlCommand(name, c, tx); cmd.CommandType = CommandType.StoredProcedure;
        foreach (var v in values) cmd.Parameters.AddWithValue("@" + v.Key, v.Value ?? DBNull.Value);
        return cmd;
    }
    static int MasterId(SqlConnection c, SqlTransaction tx, string table, string nameColumn, string idColumn, string name)
    {
        using (var cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM dbo." + table + " WITH(UPDLOCK,HOLDLOCK) WHERE " + nameColumn + "=@name) INSERT dbo." + table + " (" + nameColumn + ",Duplicate) VALUES(@name,0); SELECT TOP(1) " + idColumn + " FROM dbo." + table + " WHERE " + nameColumn + "=@name ORDER BY " + idColumn, c, tx))
        { cmd.Parameters.AddWithValue("@name", name ?? ""); return Convert.ToInt32(cmd.ExecuteScalar()); }
    }
    // activeSql: COUNT of the owner's non-deleted businesses (same rule as the Businesses tile), args for it; limit < 0 = no limit recorded.
    public static int Create(SqlConnection c, SqlTransaction tx, int owner, int limit, string activeSql, object[] activeArgs,
        string name, string contact, string phone, string email, AnyminuteSupportAddress.Addr a)
    {
        if (limit >= 0)
        {
            int used = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, activeSql, activeArgs).Rows[0][0]);
            if (used >= limit) throw new InvalidOperationException("This subscriber has used " + used + " of " + limit + " businesses on the current plan. Upgrade the plan to add more.");
        }
        if (Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, "SELECT COUNT(*) FROM dbo.AddBusiness WHERE CompanyName=@n", "@n", name).Rows[0][0]) > 0)
            throw new InvalidOperationException("Business name already exists.");
        var o = AnyminuteTimesheetRepair.Query(c, tx, "SELECT ae.EmployeeID,ae.MotherOrgid,LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name FROM dbo.AddEmployee ae WHERE ae.EmployeeID=@e", "@e", owner);
        if (o.Rows.Count != 1) throw new InvalidOperationException("The subscriber has no business owner employee record.");
        int mother; Int32.TryParse(Convert.ToString(o.Rows[0]["MotherOrgid"]), out mother);
        string ownerName = Convert.ToString(o.Rows[0]["Name"]); if (ownerName.Length > 50) ownerName = ownerName.Substring(0, 50);
        int business;
        using (var cmd = Proc(c, tx, "dbo.Sp_InsertBusiness", new Dictionary<string, object> {
            {"CompanyName", name}, {"OperatingName", name}, {"BusinessNumber", ""}, {"LegalEntityType", ""}, {"BusinessRegistrationDate", ""},
            {"PrimaryContactName", contact}, {"PrimaryContactNumber", phone}, {"BusinessAgreementDate", DateTime.Now}, {"Status", "1"},
            {"MotherOrgId", mother}, {"Approved", "A"}, {"CareOf", ""} }))
        {
            cmd.Parameters.Add("@BusinessId", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.ExecuteNonQuery(); business = Convert.ToInt32(cmd.Parameters["@BusinessId"].Value);
        }
        if (business <= 0) throw new InvalidOperationException("Business save did not return a valid business ID.");
        using (var cmd = Proc(c, tx, "dbo.Sp_InsertAssginBussiness", new Dictionary<string, object> {
            {"BusinessID", business}, {"EmployeeID", owner}, {"BusinessName", name}, {"EmployeeName", ownerName}, {"UserTypeId", 1}, {"MotherOrgID", mother} })) cmd.ExecuteNonQuery();
        AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE dbo.MappingTable SET BusinessID=@b WHERE EmployeeID=@e", "@b", business, "@e", owner);
        int amBefore = AnyminuteSupportAddress.MaxMasterId(c, tx); // v80: see AnyminuteSupportAddress.AfterLegacyInsert
        using (var cmd = Proc(c, tx, "dbo.Sp_InsertBusinessAddress", new Dictionary<string, object> {
            {"BusinessID", business}, {"Postalcode", AnyminuteBusinessAddressInput.Postal(a.Postal)}, {"NumberSuffix", "Please Select"}, {"StreetName", a.Line1}, {"StreetType", a.Line2},
            {"StreetDirection", ""}, {"SuitType", a.SuiteType.Length > 0 ? a.SuiteType : "Please Select"}, {"SuiteNumber", a.SuiteNo}, {"SuitSuffix", "Please Select"}, {"City", a.City}, {"Province", a.Province},
            {"MainPhone", phone}, {"WorkPhone", ""}, {"Mobile", ""}, {"Fax", ""}, {"MainEmail", email}, {"CCEmail", ""}, {"Website", ""}, {"StreetNo", a.StreetNo}, {"Other1", ""},
            {"Cityid", MasterId(c, tx, "CityMaster", "CityName", "CityId", a.City)}, {"Provinceid", MasterId(c, tx, "ProvinceMaster", "ProvinceName", "ProvinceId", a.Province)} })) cmd.ExecuteNonQuery();
        AnyminuteSupportAddress.AfterLegacyInsert(c, tx, amBefore, a);
        bool countryColumn = AnyminuteTimesheetRepair.Query(c, tx, "SELECT COL_LENGTH('dbo.BusinessAddress','Country')").Rows[0][0] != DBNull.Value;
        int linked = Convert.ToInt32(AnyminuteTimesheetRepair.Query(c, tx, countryColumn
            ? "UPDATE ba SET Country=@k FROM dbo.BusinessAddress ba JOIN dbo.AddBusiness b ON b.ADDRESSID=ba.BusinessAddressId WHERE b.BusinessID=@b; SELECT @@ROWCOUNT"
            : "SELECT COUNT(*) FROM dbo.BusinessAddress ba JOIN dbo.AddBusiness b ON b.ADDRESSID=ba.BusinessAddressId WHERE b.BusinessID=@b", "@k", a.Country, "@b", business).Rows[0][0]);
        if (linked != 1) throw new InvalidOperationException("Business address was not linked to the saved business.");
        return business;
    }
}
