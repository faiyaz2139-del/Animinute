using System;
using System.Web;
// One-time confirmation messages that survive a Post/Redirect/Get round trip.
// A message is stored in Session under a page key and removed the first time it is read.
public static class AnyminuteFlash
{
    const string Prefix = "AnyminuteFlash:";
    public static void Set(string key, string message)
    {
        var context = HttpContext.Current;
        if (context == null || context.Session == null || String.IsNullOrWhiteSpace(message)) return;
        context.Session[Prefix + key] = message;
    }
    public static string Take(string key)
    {
        var context = HttpContext.Current;
        if (context == null || context.Session == null) return null;
        string message = context.Session[Prefix + key] as string;
        context.Session.Remove(Prefix + key);
        return message;
    }
}
