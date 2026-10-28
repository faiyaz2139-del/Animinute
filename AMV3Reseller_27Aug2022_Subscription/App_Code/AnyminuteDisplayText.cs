using System;
using System.Data;
using System.Text.RegularExpressions;

// 2026-10-28 v27: display-only tidy for names and role labels (live regression: "Adam  Rhodes" from FN_DESC with an empty middle
// name, and the custom role "Supervisor Emplyee"). Stored data is never changed.
public static class AnyminuteDisplayText
{
    static readonly Regex Spaces = new Regex(@"\s{2,}", RegexOptions.Compiled);
    static readonly Regex Typo = new Regex(@"\b(E)(m)(p)(l)yee\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public static string Tidy(string value)
    {
        if (String.IsNullOrEmpty(value)) return value;
        value = Spaces.Replace(value.Trim(), " ");
        return Typo.Replace(value, m => m.Groups[1].Value + m.Groups[2].Value + m.Groups[3].Value + m.Groups[4].Value +
            (Char.IsUpper(m.Value[m.Value.Length - 1]) ? "OYEE" : "oyee"));
    }
    // Columns that are shown to people: ...name, Role, UserType.
    public static bool IsLabel(string column)
    {
        return column.EndsWith("name", StringComparison.OrdinalIgnoreCase) || String.Equals(column, "Role", StringComparison.OrdinalIgnoreCase)
            || String.Equals(column, "UserType", StringComparison.OrdinalIgnoreCase);
    }
    public static DataTable Tidy(DataTable table)
    {
        if (table == null) return table;
        foreach (DataColumn col in table.Columns)
        {
            if (col.DataType != typeof(string) || col.ReadOnly || !IsLabel(col.ColumnName)) continue;
            foreach (DataRow row in table.Rows)
                if (row[col] != DBNull.Value) { string v = (string)row[col], t = Tidy(v); if (!String.Equals(v, t, StringComparison.Ordinal)) row[col] = t; }
        }
        table.AcceptChanges();
        return table;
    }
}
