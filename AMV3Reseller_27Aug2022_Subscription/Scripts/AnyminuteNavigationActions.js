(function(){
 'use strict';
 var done=false;
 function activateProjectEditor(){
  var mode=location.search.match(/(?:\?|&)mode=(add|edit)(?:&|$)/i);
  if(done||!/\/add_project\.aspx$/i.test(location.pathname)||!mode)return;
  var target=mode[1].toLowerCase()==='edit'?'#view_business_wise_project':'#add_project';
  var tab=document.querySelector('a[href="'+target+'"]');
  if(tab&&window.jQuery&&window.jQuery.fn.tab){window.jQuery(tab).tab('show');done=true;}
 }
 var employeeDone=false;
 function activateEmployeeEditor(){
  if(employeeDone||!/\/addeditrole_add_employee\.aspx$/i.test(location.pathname))return;
  var section=location.search.match(/(?:\?|&)EmployeeSection=(pay|projects|profile)(?:&|$)/i);if(!section)return;
  var target=section[1].toLowerCase()==='pay'?'#ratedetils':section[1].toLowerCase()==='projects'?'#project':'#home';
  var tab=document.querySelector('a[href="'+target+'"]');
  if(tab&&window.jQuery&&window.jQuery.fn.tab){window.jQuery(tab).tab('show');employeeDone=true;}
 }
 function ready(){setTimeout(function(){activateProjectEditor();activateEmployeeEditor();},0);}
 if(window.jQuery)window.jQuery(ready);else if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',ready);else ready();
 if(window.Sys&&window.Sys.Application)window.Sys.Application.add_load(ready);
})();
