using System;
using System.Data;
using System.Text.RegularExpressions;

// No database writes or UI dependencies: also usable by employee address forms.
public sealed class AnyminuteBusinessAddressInput
{
    public string Number = "", Street = "", Line2 = "", City = "", Province = "", PostalCode = "", Country = "Canada";
    public static string Postal(string value)
    { return Regex.Replace(value ?? "", @"\s", "").ToUpperInvariant(); }
    private static string Value(DataRow row, params string[] columns)
    {
        foreach (string column in columns)
            if (row.Table.Columns.Contains(column) && row[column] != DBNull.Value && Convert.ToString(row[column]).Trim().Length > 0)
                return Convert.ToString(row[column]).Trim();
        return "";
    }
    public static AnyminuteBusinessAddressInput FromRow(DataRow row, string postal)
    {
        var address = new AnyminuteBusinessAddressInput();
        address.Number = Value(row, "StreetNo", "StreetNumber");
        address.Street = Value(row, "StreetName", "AddressLine1");
        address.Line2 = Value(row, "AddressLine2", "StreetType");
        address.City = Value(row, "City", "CityName");
        address.Province = Value(row, "Province", "ProvinceName", "State");
        address.PostalCode = Postal(Value(row, "Postalcode", "PostalCode"));
        if (address.PostalCode.Length == 0) address.PostalCode = Postal(postal);
        string country = Value(row, "Country");
        if (country.Length > 0) address.Country = country;
        // Legacy Sp_SearchAddress returns a display address rather than structured fields.
        // Only infer the last two comma-separated pieces as city/province. Never guess a number.
        string[] pieces = Value(row, "SEARCHADDRESS").Split(',');
        if (pieces.Length >= 3)
        {
            if (address.City.Length == 0) address.City = pieces[pieces.Length - 2].Trim();
            if (address.Province.Length == 0) address.Province = pieces[pieces.Length - 1].Trim();
            if (address.Street.Length == 0) address.Street = String.Join(",", pieces, 0, pieces.Length - 2).Trim();
        }
        if (address.Number.Length == 0)
        {
            Match number = Regex.Match(address.Street, @"^(\d+[A-Za-z]?(?:-\d+[A-Za-z]?)?)\s+(.+)$");
            if (number.Success) { address.Number = number.Groups[1].Value; address.Street = number.Groups[2].Value; }
        }
        return address;
    }
}
