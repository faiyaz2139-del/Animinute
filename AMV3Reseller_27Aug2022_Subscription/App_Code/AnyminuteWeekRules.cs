using System;
public static class AnyminuteWeekRules
{
 public static string Next(string state,string action,bool own,bool reviewer,bool owner,string comment)
 {
  if(action=="Submit")
  {if(!own)throw new InvalidOperationException("Only the employee can submit their week.");if(state!="Draft"&&state!="Rejected")throw new InvalidOperationException("Only a draft or returned week can be submitted.");return "Submitted";}
  if(action=="Reopen")
  {if(!owner||state!="Approved")throw new InvalidOperationException("Only an owner can reopen an approved week.");if(String.IsNullOrWhiteSpace(comment))throw new InvalidOperationException("Explain why this week is being reopened.");return "Rejected";}
  if(own||!reviewer)throw new InvalidOperationException("An independent authorized reviewer is required.");
  if(state!="Submitted")throw new InvalidOperationException("Only a submitted week can be reviewed.");
  if(action=="Approve")return "Approved";
  if(action=="Reject") {if(String.IsNullOrWhiteSpace(comment))throw new InvalidOperationException("A comment is required when returning a week.");return "Rejected";}
  throw new InvalidOperationException("Invalid workflow action.");
 }
}
