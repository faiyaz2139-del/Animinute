using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

// Reuses the established stored procedures, on ONE connection/transaction.
// A failed address/assignment/country write rolls back the business write as well.
public static class AnyminuteBusinessSave
{
    private static SqlCommand Procedure(SqlConnection c, SqlTransaction tx, string name, IDictionary<string,object> values)
    {
        var cmd = new SqlCommand(name, c, tx); cmd.CommandType = CommandType.StoredProcedure;
        foreach (var value in values) cmd.Parameters.AddWithValue("@" + value.Key, value.Value ?? DBNull.Value);
        return cmd;
    }
    public static int Save(int business, IDictionary<string,object> details, IDictionary<string,object> address,
        string country, bool imageChanged, string ownerEmail)
    {
        AnyminuteModernScope.Login();
        using (var c = AnyminuteTimesheetRepair.Open())
        using (var tx = c.BeginTransaction(IsolationLevel.Serializable))
        {
            // Country is optional storage: the production schema has no BusinessAddress.Country column
            // unless the separate optional script (DB_OPTIONAL/OPTIONAL_01_BusinessAddress_Country.sql in the fix package; not deployed with the site) was applied.
            // A missing column must never block saving a business; the country is then simply not stored.
            bool countryColumn;
            using (var schema = new SqlCommand("SELECT COL_LENGTH('dbo.BusinessAddress','Country')", c, tx))
                countryColumn = schema.ExecuteScalar() != DBNull.Value;
            bool creating = business == 0;
            if (!creating) AnyminuteApprovalAssignments.RequireOwner(c, tx, business);
            else AnyminuteFormAccess.Subscriber();
            if (!creating)
            {
                using (var duplicate = new SqlCommand("SELECT COUNT(*) FROM dbo.AddBusiness WHERE CompanyName=@n AND BusinessID<>@b", c, tx))
                {
                    duplicate.Parameters.AddWithValue("@n", details["CompanyName"]); duplicate.Parameters.AddWithValue("@b", business);
                    if (Convert.ToInt32(duplicate.ExecuteScalar()) > 0) throw new InvalidOperationException("Business name already exists.");
                }
            }
            var parameters = new Dictionary<string,object>(details);
            if (!creating)
                using (var original = new SqlCommand("SELECT BusinessAgreementDate FROM dbo.AddBusiness WHERE BusinessID=@b", c, tx))
                { original.Parameters.AddWithValue("@b", business); parameters["BusinessAgreementDate"] = original.ExecuteScalar() ?? DBNull.Value; }
            string procedure;
            if (creating) { procedure = "dbo.Sp_InsertBusiness"; parameters["Approved"] = "A"; }
            else
            {
                procedure = imageChanged ? "dbo.SP_UpdateBusinessDetails" : "dbo.SP_UpdateBusinessDetailsWithoutImage";
                parameters["BusinessId"] = business;
                // Keep the exact established update-procedure parameter spelling for case-sensitive databases.
                parameters["MotherOrgID"] = parameters["MotherOrgId"];
                parameters.Remove("MotherOrgId");
                if (!imageChanged) parameters.Remove("CareOf");
            }
            using (var cmd = Procedure(c, tx, procedure, parameters))
            {
                if (creating) cmd.Parameters.Add("@BusinessId", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                if (creating) business = Convert.ToInt32(cmd.Parameters["@BusinessId"].Value);
            }
            if (business <= 0) throw new InvalidOperationException("Business save did not return a valid business ID.");
            if (creating)
            {
                int owner;
                using (var cmd = new SqlCommand("SELECT DISTINCT ad.EmployeeID FROM dbo.AddEmployee ad JOIN dbo.EmpAddress ea ON ad.ADDRESSID=ea.EmpAddressId WHERE ea.MainEmail=@email AND ad.EmployeeID=@actor AND ad.UserType=1", c, tx))
                {
                    cmd.Parameters.AddWithValue("@email", ownerEmail);
                    cmd.Parameters.AddWithValue("@actor", Convert.ToInt32(System.Web.HttpContext.Current.Session["EmployeeId"]));
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) throw new InvalidOperationException("The authenticated subscriber has no business owner employee record.");
                        owner = Convert.ToInt32(reader[0]);
                        if (reader.Read()) throw new InvalidOperationException("The business owner identity is ambiguous.");
                    }
                }
                string name;
                using (var cmd = Procedure(c, tx, "dbo.GetNameFromId", new Dictionary<string,object>{{"P_TYPE","AddEmployee"},{"P_ID",owner}}))
                using (var reader = cmd.ExecuteReader())
                { if (!reader.Read()) throw new InvalidOperationException("Business owner name is missing."); name = Convert.ToString(reader["NAME"]); }
                using (var cmd = Procedure(c, tx, "dbo.Sp_InsertAssginBussiness", new Dictionary<string,object>{
                    {"BusinessID",business},{"EmployeeID",owner},{"BusinessName",details["CompanyName"]},
                    {"EmployeeName",name},{"UserTypeId",1},{"MotherOrgID",details["MotherOrgId"]}})) cmd.ExecuteNonQuery();
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE dbo.MappingTable SET BusinessID=@b WHERE EmployeeID=@e", "@b", business, "@e", owner);
            }
            else
                AnyminuteTimesheetRepair.Execute(c, tx, "UPDATE dbo.AssginBussiness SET BusinessName=@n WHERE BusinessID=@b; UPDATE dbo.AssgineManager SET BusinessName=@n WHERE BusinessID=@b", "@n",details["CompanyName"],"@b",business);
            var addressParameters = new Dictionary<string,object>(address);
            addressParameters["BusinessID"] = business;
            addressParameters["Cityid"] = MasterId(c, tx, "CityMaster", "CityName", "CityId", Convert.ToString(address["City"]));
            addressParameters["Provinceid"] = MasterId(c, tx, "ProvinceMaster", "ProvinceName", "ProvinceId", Convert.ToString(address["Province"]));
            using (var cmd = Procedure(c, tx, "dbo.Sp_InsertBusinessAddress", addressParameters)) cmd.ExecuteNonQuery();
            using (var cmd = new SqlCommand(countryColumn
                ? "UPDATE ba SET Country=@country FROM dbo.BusinessAddress ba JOIN dbo.AddBusiness b ON b.ADDRESSID=ba.BusinessAddressId WHERE b.BusinessID=@b; SELECT @@ROWCOUNT;"
                : "SELECT COUNT(*) FROM dbo.BusinessAddress ba JOIN dbo.AddBusiness b ON b.ADDRESSID=ba.BusinessAddressId WHERE b.BusinessID=@b;", c, tx))
            {
                if (countryColumn) cmd.Parameters.Add("@country", SqlDbType.NVarChar, 100).Value = country;
                cmd.Parameters.Add("@b", SqlDbType.Int).Value = business;
                if (Convert.ToInt32(cmd.ExecuteScalar()) != 1) throw new InvalidOperationException("Business address was not linked to the saved business.");
            }
            tx.Commit();
            return business;
        }
    }
    private static int MasterId(SqlConnection c, SqlTransaction tx, string table, string nameColumn, string idColumn, string name)
    {
        // Identifiers are private constants from the two calls above; all entered values are parameters.
        using (var cmd = new SqlCommand("IF NOT EXISTS(SELECT 1 FROM dbo." + table + " WITH(UPDLOCK,HOLDLOCK) WHERE " + nameColumn + "=@name) INSERT dbo." + table + " (" + nameColumn + ",Duplicate) VALUES(@name,0); SELECT TOP(1) " + idColumn + " FROM dbo." + table + " WHERE " + nameColumn + "=@name ORDER BY " + idColumn, c, tx))
        { cmd.Parameters.AddWithValue("@name", name); return Convert.ToInt32(cmd.ExecuteScalar()); }
    }
}
