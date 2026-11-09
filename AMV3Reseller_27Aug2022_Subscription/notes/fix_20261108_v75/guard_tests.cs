using System;
using System.Reflection;
class Tests {
 static void Main() {
 var asm=Assembly.LoadFrom("/tmp/v75build/SupportSubscribers.aspx.dll"); MethodInfo guard=null;
 foreach(var t in asm.GetTypes()) { var m=t.GetMethod("RemovedDataReset",BindingFlags.Static|BindingFlags.NonPublic);if(m!=null)guard=m; }
 if(guard==null)throw new Exception("Guard missing");
 foreach(var op in new[]{"erase","ERASE"," eraseall ","eraseallrun","clean","cleanprev","cleanrun","wipeeverything","wipeeverythingrun"," CleanRun "})
 {if(!(bool)guard.Invoke(null,new object[]{op}))throw new Exception("Not blocked: "+op);Console.WriteLine("PASS blocked: "+op);}
 foreach(var op in new[]{"","add","addsave","edit","save","reset","invite","access","bizaddsave","businesses"})
 {if((bool)guard.Invoke(null,new object[]{op}))throw new Exception("Legitimate op blocked: "+op);}
 Console.WriteLine("PASS legitimate subscriber/password-reset/invitation operations remain allowed");
 }
}
