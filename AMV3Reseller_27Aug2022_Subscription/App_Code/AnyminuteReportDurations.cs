using System;
using System.Data;
using System.Globalization;

public static class AnyminuteReportDurations
{
    public static TimeSpan Parse(object value)
    {
        string s=Convert.ToString(value,CultureInfo.InvariantCulture).Trim();
        if(s=="" || s=="0") return TimeSpan.Zero;
        var parts=s.Split(':'); long hours;int minutes,seconds=0;
        if((parts.Length==2 || parts.Length==3) && Int64.TryParse(parts[0],out hours) &&
            Int32.TryParse(parts[1],out minutes) && (parts.Length==2 || Int32.TryParse(parts[2],out seconds)) &&
            hours>=0 && minutes>=0 && minutes<60 && seconds>=0 && seconds<60)
            return TimeSpan.FromTicks(checked((checked(hours*3600)+minutes*60+seconds)*TimeSpan.TicksPerSecond));
        TimeSpan duration;
        if(TimeSpan.TryParseExact(s,"c",CultureInfo.InvariantCulture,out duration) && duration>=TimeSpan.Zero) return duration;
        throw new InvalidOperationException("Invalid report duration: "+s);
    }
    public static string Format(TimeSpan value)
    { if(value<TimeSpan.Zero) throw new InvalidOperationException("Negative report duration.");return ((long)value.TotalHours).ToString("00",CultureInfo.InvariantCulture)+":"+value.Minutes.ToString("00",CultureInfo.InvariantCulture); }
    public static TimeSpan Sum(DataTable table,string kind)
    {
        if(table.Rows.Count==0) return TimeSpan.Zero;
        string[] names=kind=="gross"?new[]{"Gross Hours","Regular","Gross"}:kind=="break"?new[]{"Break Time","BreakTotal","Break Hours"}:new[]{"Net Hours & Minutes","Net Hours","Total","Net"};
        DataColumn column=null;foreach(string name in names) if(table.Columns.Contains(name)) { column=table.Columns[name];break; }
        if(column==null) throw new InvalidOperationException("Report is missing its "+kind+" duration column.");
        TimeSpan sum=TimeSpan.Zero;foreach(DataRow row in table.Rows) sum+=Parse(row[column]);return sum;
    }
    public static void FormatCells(DataTable table)
    {
        foreach(string name in new[]{"Gross Hours","Regular","Gross","Break Time","BreakTotal","Break Hours","Net Hours & Minutes","Net Hours","Total","Net"})
        {
            if(!table.Columns.Contains(name) || table.Columns[name].DataType!=typeof(string)) continue;
            foreach(DataRow row in table.Rows) row[name]=Format(Parse(row[name]));
        }
    }
}
