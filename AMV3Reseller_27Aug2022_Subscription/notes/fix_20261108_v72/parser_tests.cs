using System;
using System.IO;
using System.Linq;
class Test {
 static int n;
 static void Check(bool b,string s) { if(!b) throw new Exception(s); n++; Console.WriteLine("PASS "+s); }
 static void Reject(Action a,string s) {try { a(); } catch(InvalidDataException) {Check(true,s);return;} throw new Exception("Not rejected: "+s);}
 static void Main() {
 string sheets; var r=AnyminuteAddressImport.Read("multi.xlsx",File.ReadAllBytes("/tmp/v72build/multi.xlsx"),out sheets);
 Check(r.Count==2,"all worksheets + sparse row 10000 does not inflate count");
 Check(r[0][0]=="Inline Street" && r[1][0]=="Other" && r[1][3]=="Canada","inline strings + independently reordered sheet headers + absolute relationship");
 Check(sheets=="Sheet1, Sheet2","worksheet names reported");
 r=AnyminuteAddressImport.Read("shared.xlsx",File.ReadAllBytes("/tmp/v72build/shared.xlsx"));
 Check(r.Count==1 && r[0][0]=="Shared Street" && r[0][1]=="" && r[0][2]=="ON","rich shared strings + sparse missing cell");
 r=AnyminuteAddressImport.Read("4400.xlsx",File.ReadAllBytes("/tmp/v72build/4400.xlsx"));Check(r.Count==4400,"2400 + 2000 worksheets = 4400 data rows");
 Reject(()=>AnyminuteAddressImport.Read("over.xlsx",File.ReadAllBytes("/tmp/v72build/over.xlsx")),"5001 XLSX data rows rejected, never truncated");
 var csv="Address,City,Prov/State,Country,Postal Code/PIN\n";
 var body=String.Join("\n",Enumerable.Repeat("Road,Toronto,ON,Canada,M1A 1A1",5000));
 Check(AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes(csv+body)).Count==5000,"exactly 5000 CSV rows accepted");
 Reject(()=>AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes(csv+body+"\nRoad,Toronto,ON,Canada,M1A 1A1")),"5001 CSV rows rejected");
 Reject(()=>AnyminuteAddressImport.Read("a.csv",new byte[AnyminuteAddressImport.MaxBytes+1]),"2 MB size cap explicit rejection");
 Reject(()=>AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes("Address,City,Country\nRoad,Toronto,Canada")),"partial header rejected, not silently mis-mapped");
 r=AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes("Address;City;Prov/State;Country;Postal Code/PIN\n\"A; B\";Toronto;ON;Canada;M1A 1A1\n;;;;"));
 Check(r.Count==2 && r[0][0]=="A; B" && r[1].All(x=>x==""),"semicolon CSV quoting + blank data row retained for counting");
 Check(AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes("Road,Cityville,ON,Canada,M1A 1A1")).Count==1,"headerless data no longer mistaken for prefix-matching header");
 Reject(()=>AnyminuteAddressImport.Read("a.csv",System.Text.Encoding.UTF8.GetBytes(csv+"\"bad")),"unclosed CSV quote rejected");
 Console.WriteLine(n+" parser regression checks passed; no database used.");
 }
}
