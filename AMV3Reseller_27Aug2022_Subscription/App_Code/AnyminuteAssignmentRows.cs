using System.Data;
public static class AnyminuteAssignmentRows
{
    public static DataTable CopyToDataTableOrEmpty(this DataRow[] rows,DataTable schema)
    { var result=schema.Clone();foreach(var row in rows)result.ImportRow(row);return result; }
}
