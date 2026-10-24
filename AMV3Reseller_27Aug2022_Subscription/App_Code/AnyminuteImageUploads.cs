using System;
using System.IO;
using System.Web;
public static class AnyminuteImageUploads
{
 public static string Extension(HttpPostedFile file)
 {
  if(file==null||file.ContentLength<=0||file.ContentLength>2*1024*1024)throw new InvalidOperationException("Choose a PNG or JPEG image no larger than 2 MB.");
  string ext=Path.GetExtension(file.FileName).ToLowerInvariant();if(ext!=".png"&&ext!=".jpg"&&ext!=".jpeg")throw new InvalidOperationException("Choose a PNG or JPEG image.");
  byte[] head=new byte[8];if(!file.InputStream.CanSeek)throw new InvalidOperationException("Image could not be validated.");file.InputStream.Position=0;int n=file.InputStream.Read(head,0,8);file.InputStream.Position=0;
  bool png=n==8&&head[0]==137&&head[1]==80&&head[2]==78&&head[3]==71&&head[4]==13&&head[5]==10&&head[6]==26&&head[7]==10;
  bool jpg=n>=3&&head[0]==255&&head[1]==216&&head[2]==255;
  if(ext==".png"?!png:!jpg)throw new InvalidOperationException("The file content does not match a PNG or JPEG image.");return ext;
 }
 public static string Name(HttpPostedFile file){return Guid.NewGuid().ToString("N")+Extension(file);}
 public static void SaveNamed(HttpPostedFile file,HttpServerUtility server,string name)
 {
  string ext=Extension(file);Guid id;if(Path.GetFileName(name)!=name||Path.GetExtension(name).ToLowerInvariant()!=ext||!Guid.TryParseExact(Path.GetFileNameWithoutExtension(name),"N",out id))throw new InvalidOperationException("Invalid image filename.");
  string directory=server.MapPath("~/Uploads/");Directory.CreateDirectory(directory);file.SaveAs(Path.Combine(directory,name));
 }
 public static string Save(HttpPostedFile file,HttpServerUtility server){string name=Name(file);SaveNamed(file,server,name);return name;}
}
