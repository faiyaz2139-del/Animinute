// 2026-11-16 v86: a "Back" button top-left of every screen (above the page header), except the Dashboard home.
// Goes to the page the user came from (same site, different page); otherwise to the screen's parent list; otherwise Dashboard.
(function(document,location){
 'use strict';
 var PARENT={
  'employeedetails.aspx':'Users.aspx','employeeeditor.aspx':'Users.aspx','employeeassignment.aspx':'Users.aspx','employeeselect.aspx':'Users.aspx','userloginaccess.aspx':'Users.aspx',
  'projectview.aspx':'Projects.aspx','projecteditor.aspx':'Projects.aspx','projectassignments.aspx':'Projects.aspx',
  'add_business.aspx':'Dashboard.aspx?view=businesses','businessprofile.aspx':'Dashboard.aspx?view=businesses','businessactions.aspx':'Dashboard.aspx?view=businesses','businesssettings.aspx':'Dashboard.aspx?view=businesses','accountantaccess.aspx':'Dashboard.aspx?view=businesses',
  'weektimecard.aspx':'TimeCards.aspx'};
 function init(){
  var page=(location.pathname.split('/').pop()||'').toLowerCase(),q=location.search||'';
  if(page===''||page==='default.aspx'||page==='login.aspx'||(page==='dashboard.aspx'&&!/[?&]view=/i.test(q)))return;
  var host=document.querySelector('main.am-main')||document.getElementById('page-content-wrapper');
  if(!host||host.querySelector('.am-backbar,.am-back'))return;
  // A screen that already shows its own back link keeps it.
  var links=host.querySelectorAll('a,button,input[type=submit],input[type=button]');
  for(var i=0;i<links.length;i++){var t=(links[i].textContent||links[i].value||'').replace(/\s+/g,' ').trim();if(/^(\u2190\s*)?back\b/i.test(t)||/^\u2190/.test(t))return;}
  var href=PARENT[page]||'Dashboard.aspx',ref=document.referrer;
  try{if(ref){var r=new URL(ref);if(r.origin===location.origin&&r.pathname!==location.pathname&&!/login\.aspx$/i.test(r.pathname))href=ref;}}catch(x){}
  var bar=document.createElement('div');bar.className='am-backbar';
  var a=document.createElement('a');a.className='am-back';a.href=href;a.innerHTML='<span aria-hidden="true">&larr;</span> Back';a.setAttribute('aria-label','Back');
  bar.appendChild(a);host.insertBefore(bar,host.firstChild);
 }
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
}(document,location));
