using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Script.Serialization;

// 2026-10-28 v26 (Login Activity History "Location"): best-effort IP -> "City, Region, Country".
// No paid service and no key: private / loopback addresses are labelled locally; public addresses are looked up only
// when web.config has <add key="GeoIpLookupUrl" value="..."/> with an {ip} placeholder (any free JSON endpoint returning
// city / region(_code|Name) / country(Code) fields, e.g. https://ipwho.is/{ip} or http://ip-api.com/json/{ip}).
// Results are cached for a day; each page has a small time budget so the page never hangs on the lookup service.
public static class AnyminuteGeoIp
{
    public const string NotConfigured = "\u0001";
    public static bool Configured { get { return (ConfigurationManager.AppSettings["GeoIpLookupUrl"] ?? "").Contains("{ip}"); } }
    public static string Local(string ip)
    {
        IPAddress a; if (String.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out a)) return "";
        if (IPAddress.IsLoopback(a)) return "This server (localhost)";
        var b = a.GetAddressBytes();
        if (b.Length == 4 && (b[0] == 10 || (b[0] == 172 && b[1] >= 16 && b[1] <= 31) || (b[0] == 192 && b[1] == 168) || (b[0] == 169 && b[1] == 254))) return "Private network";
        if (b.Length == 16 && ((b[0] & 0xFE) == 0xFC || (b[0] == 0xFE && (b[1] & 0xC0) == 0x80))) return "Private network";
        return null;
    }
    // Returns a location, "" when it cannot be resolved, or NotConfigured when no lookup URL is set.
    public static string Locate(string ip, ref int budgetMs)
    {
        string local = Local(ip); if (local != null) return local;
        object hit = HttpRuntime.Cache["am_geo_" + ip]; if (hit is string) return (string)hit;
        if (!Configured) return NotConfigured;
        if (budgetMs <= 0) return "";
        var watch = System.Diagnostics.Stopwatch.StartNew(); string result = "";
        try
        {
            var req = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["GeoIpLookupUrl"].Replace("{ip}", Uri.EscapeDataString(ip)));
            req.Timeout = Math.Min(1500, budgetMs); req.ReadWriteTimeout = req.Timeout; req.UserAgent = "AnyMinute";
            using (var resp = req.GetResponse()) using (var r = new StreamReader(resp.GetResponseStream()))
            {
                var d = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(r.ReadToEnd());
                Func<string, string> g = k => d != null && d.ContainsKey(k) && d[k] != null ? Convert.ToString(d[k]).Trim() : "";
                string city = g("city"), region = g("region_code"), country = g("country");
                if (region.Length == 0) region = g("region").Length > 0 && g("region").Length <= 3 ? g("region") : (g("regionName").Length > 0 ? g("regionName") : g("region"));
                if (country.Length == 0) country = g("countryCode").Length > 0 ? g("countryCode") : g("country_code");
                var parts = new List<string>(); foreach (var p in new[] { city, region, country }) if (p.Length > 0 && !parts.Contains(p)) parts.Add(p);
                result = String.Join(", ", parts);
            }
        }
        catch (Exception) { result = ""; }
        budgetMs -= (int)watch.ElapsedMilliseconds;
        HttpRuntime.Cache.Insert("am_geo_" + ip, result, null, DateTime.UtcNow.AddHours(result.Length > 0 ? 24 : 1), Cache.NoSlidingExpiration);
        return result;
    }
}
