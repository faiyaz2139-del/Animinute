using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

// 2026-11-16 v86 (Employee Details deck, slide 5 Documents): employee document storage.
// Files live under ~/App_Data/EmployeeDocuments/<employee record id>/ (App_Data is never served directly);
// an index.json beside them keeps name, type, related business, upload date, expiry and uploader.
// No database change. Callers authorise the employee (and business) before calling.
public sealed class AnyminuteEmployeeDocument
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string StoredName { get; set; }
    public string Type { get; set; }
    public int BusinessId { get; set; }
    public string Business { get; set; }
    public DateTime UploadedUtc { get; set; }
    public DateTime? Expiry { get; set; }
    public string UploadedBy { get; set; }
    public long Size { get; set; }
    public string Status { get { return Expiry.HasValue && Expiry.Value.Date < DateTime.Today ? "Expired" : "Active"; } }
    public string UploadedText { get { return UploadedUtc.ToLocalTime().ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture); } }
    public string UploadedSort { get { return UploadedUtc.ToString("yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture); } }
    public string ExpiryText { get { return Expiry.HasValue ? Expiry.Value.ToString("MMM dd, yyyy", System.Globalization.CultureInfo.InvariantCulture) : "\u2014"; } }
    public string ExpirySort { get { return Expiry.HasValue ? Expiry.Value.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) : ""; } }
}
public static class AnyminuteEmployeeDocuments
{
    public static readonly string[] Types = { "Offer Letter", "ID Proof", "Tax Form", "Agreement", "Other" };
    static readonly string[] Allowed = { ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx" };
    const int MaxBytes = 4 * 1024 * 1024;
    static readonly object Gate = new object();
    static string Folder(int employee) { return HttpContext.Current.Server.MapPath("~/App_Data/EmployeeDocuments/" + employee + "/"); }
    static string Index(int employee) { return Path.Combine(Folder(employee), "index.json"); }
    static List<AnyminuteEmployeeDocument> Read(int employee)
    {
        string path = Index(employee);
        if (!File.Exists(path)) return new List<AnyminuteEmployeeDocument>();
        return new JavaScriptSerializer().Deserialize<List<AnyminuteEmployeeDocument>>(File.ReadAllText(path)) ?? new List<AnyminuteEmployeeDocument>();
    }
    static void Write(int employee, List<AnyminuteEmployeeDocument> list)
    {
        Directory.CreateDirectory(Folder(employee)); string path = Index(employee), temp = path + ".tmp";
        File.WriteAllText(temp, new JavaScriptSerializer().Serialize(list)); if (File.Exists(path)) File.Delete(path); File.Move(temp, path);
    }
    // Only documents for businesses the caller can see are returned.
    public static List<AnyminuteEmployeeDocument> List(int employee, IEnumerable<int> businesses)
    {
        var allowed = new HashSet<int>(businesses);
        lock (Gate) return Read(employee).Where(d => allowed.Contains(d.BusinessId)).OrderByDescending(d => d.UploadedUtc).ToList();
    }
    public static AnyminuteEmployeeDocument Find(int employee, IEnumerable<int> businesses, string id)
    {
        return List(employee, businesses).FirstOrDefault(d => d.Id == id);
    }
    public static string PathOf(int employee, AnyminuteEmployeeDocument d) { return Path.Combine(Folder(employee), d.StoredName); }
    public static void Save(int employee, HttpPostedFile file, string type, int business, string businessName, DateTime? expiry, string uploadedBy)
    {
        if (file == null || file.ContentLength <= 0) throw new InvalidOperationException("Choose a document to upload.");
        if (file.ContentLength > MaxBytes) throw new InvalidOperationException("Choose a document no larger than 4 MB.");
        string name = Path.GetFileName(file.FileName ?? "").Trim(), ext = Path.GetExtension(name).ToLowerInvariant();
        if (name.Length == 0 || !Allowed.Contains(ext)) throw new InvalidOperationException("Upload a PDF, Word document, PNG or JPEG file.");
        if (!Types.Contains(type)) throw new InvalidOperationException("Select a document type.");
        if (name.Length > 150) name = name.Substring(name.Length - 150);
        var d = new AnyminuteEmployeeDocument { Id = Guid.NewGuid().ToString("N"), FileName = name, Type = type, BusinessId = business, Business = businessName, UploadedUtc = DateTime.UtcNow, Expiry = expiry, UploadedBy = uploadedBy ?? "", Size = file.ContentLength };
        d.StoredName = d.Id + ext;
        lock (Gate)
        {
            try
            {
                Directory.CreateDirectory(Folder(employee)); file.SaveAs(PathOf(employee, d));
                var list = Read(employee); list.Add(d); Write(employee, list);
            }
            catch (UnauthorizedAccessException) { throw new InvalidOperationException("The server cannot store documents yet (App_Data/EmployeeDocuments is not writable). Nothing was saved."); }
            catch (IOException) { throw new InvalidOperationException("The document could not be stored. Nothing was saved."); }
        }
    }
    public static void Delete(int employee, IEnumerable<int> businesses, string id)
    {
        lock (Gate)
        {
            var allowed = new HashSet<int>(businesses); var list = Read(employee);
            var d = list.FirstOrDefault(x => x.Id == id && allowed.Contains(x.BusinessId));
            if (d == null) throw new InvalidOperationException("Document not found.");
            list.Remove(d); Write(employee, list);
            string path = PathOf(employee, d); if (File.Exists(path)) File.Delete(path);
        }
    }
    public static string ContentType(string fileName)
    {
        switch (Path.GetExtension(fileName).ToLowerInvariant())
        {
            case ".pdf": return "application/pdf"; case ".png": return "image/png"; case ".jpg": case ".jpeg": return "image/jpeg";
            case ".doc": return "application/msword"; case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            default: return "application/octet-stream";
        }
    }
}
