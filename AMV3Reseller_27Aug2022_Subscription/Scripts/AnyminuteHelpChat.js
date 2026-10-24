(function(){
 'use strict';
 function reply(question){
  var q=String(question||'').trim().toLowerCase();
  if(!q)return 'Choose a help topic or enter a question.';
  if(/password|\baccount\b|upgrade|sign out|logout/.test(q))return 'Open the account icon for My profile, Change password and Sign out. Plan Upgrade appears below Change password when your role permits it. Never share your password in chat.';
  if(/accountant/.test(q))return 'Owners can open Businesses > Assign Accountant, choose an owned business, and continue to its accountant assignments. The target page checks ownership again.';
  if(/break|unpaid|paid/.test(q))return 'Save the shift, then open View / edit breaks. Enter a start and end time, choose Paid or Unpaid, and save. Breaks must fit inside the shift and cannot overlap. Only unpaid breaks reduce net hours.';
  if(/approv|reject|submit|lock/.test(q))return 'Save a draft and choose Submit Week when ready. An authorized reviewer uses Approvals to review days and the week. A rejection comment is required. Submitted and approved cards are locked; rejected cards return for correction. You cannot review your own submission.';
  if(/address|postal|logo|business/.test(q))return 'Use Businesses > Edit Business and select an owned business, or Add Business to create one. Search a postal code and select a result, or choose Add New Address. Correct the editable fields, including street number and Country, then Save. Clear Address keeps business and contact details.';
  if(/employee|user/.test(q))return 'Owners use Employees > Add Employee to create a user. Employees > Edit Employee opens the accessible user list; choose the row action Edit User for the employee to change. Access is limited by role and business.';
  if(/project/.test(q))return 'Owners use Projects > Add Project to create a project. Projects > Edit Project opens the existing View Projects tab; choose a project row and its edit action. Existing project permissions still apply.';
  if(/time|clock|day off|worked|week/.test(q))return 'Open Time Cards, choose business, employee and week, then choose Worked or Day Off. Enter clock-in and clock-out; use Next day only for an overnight shift. Save a draft before submitting. Copy Monday to weekdays fills Tuesday through Friday.';
  return 'I can help with time cards, breaks, approvals, business addresses, employees, projects and account options. Choose a topic, open Help Center, or use the support ticket link if available. This chat cannot inspect your records or resolve an account-specific error.';
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={reply:reply};
 if(typeof document==='undefined')return;
 var root=document.getElementById('amHelpChat');if(!root)return;
 var log=document.getElementById('amChatMessages'),input=document.getElementById('amChatInput');
 function add(who,text){var p=document.createElement('p'),label=document.createElement('strong');label.textContent=who+': ';p.appendChild(label);p.appendChild(document.createTextNode(text));log.appendChild(p);while(log.children.length>40)log.removeChild(log.firstElementChild);log.scrollTop=log.scrollHeight;}
 function send(text){text=String(text||'').trim().slice(0,1000);if(!text){input.focus();return;}add('You',text);add('Automated help',reply(text));input.value='';input.focus();}
 document.getElementById('amChatSend').addEventListener('click',function(){send(input.value);});
 input.addEventListener('keydown',function(e){if(e.key==='Enter'&&!e.shiftKey){e.preventDefault();send(input.value);}});
 root.querySelectorAll('[data-chat-topic]').forEach(function(button){button.addEventListener('click',function(){send(button.dataset.chatTopic);});});
 document.getElementById('amChatClear').addEventListener('click',function(){while(log.firstChild)log.removeChild(log.firstChild);input.value='';add('Automated help','Choose a topic or ask a question.');input.focus();});
 add('Automated help','Choose a topic or ask a question. For a person, use the available support ticket link.');
})();
