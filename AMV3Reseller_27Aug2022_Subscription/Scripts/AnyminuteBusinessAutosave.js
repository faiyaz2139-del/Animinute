// 2026-10-26 v19 (t76u): Update Business Profile auto-save. On an existing business, a changed Business Details
// field (change/blur) is saved automatically through the page's own Save handler (same server validation and
// owner check as the Save button). The logo upload is left to the Save button. Empty postal codes no longer block it.
(function(){
 'use strict';
 if(!/[?&]BusinessIDEdit=\d+/i.test(location.search))return;
 var timer=null,busy=false,again=false;
 function save(){return document.querySelector('a[id$="SaveBusinessTop"]');}
 function target(){var a=save(),m=a&&/__doPostBack\('([^']+)'/.exec(a.getAttribute('href')||'');return m?m[1]:null;}
 function pill(){
  var p=document.getElementById('amAutosaveStatus');
  if(!p){var host=document.querySelector('.am-business-actions');if(!host)return null;p=document.createElement('span');p.id='amAutosaveStatus';p.className='am-autosave';p.setAttribute('role','status');p.setAttribute('aria-live','polite');host.insertBefore(p,host.firstChild);}
  return p;
 }
 function status(text,kind){var p=pill();if(!p)return;p.textContent=text;p.className='am-autosave'+(kind?' am-autosave-'+kind:'');}
 function banner(msg){
  var e=document.getElementById('SaveError');
  if(!e){e=document.createElement('span');e.id='SaveError';e.className='am-business-flash am-business-flash-error';e.setAttribute('role','alert');var h=document.querySelector('.am-business-header');if(h&&h.parentNode)h.parentNode.insertBefore(e,h.nextSibling);}
  if(msg){e.textContent=msg;e.style.display='';}else{e.style.display='none';}
 }
 function copyState(doc){
  ['__VIEWSTATE','__VIEWSTATEGENERATOR','__EVENTVALIDATION','__VIEWSTATEENCRYPTED'].forEach(function(n){
   var src=doc.querySelector('input[name="'+n+'"]'),dst=document.querySelector('input[name="'+n+'"]');
   if(src&&dst)dst.value=src.value;
  });
 }
 function run(){
  var t=target(),form=document.forms[0];if(!t||!form)return;
  if(busy){again=true;return;}busy=true;status('Saving\u2026','busy');
  var fd=new FormData(form);
  Array.prototype.forEach.call(form.querySelectorAll('input[type=file]'),function(f){if(f.name)fd.delete(f.name);});
  fd.set('__EVENTTARGET',t);fd.set('__EVENTARGUMENT','');fd.delete('__ASYNCPOST');
  fetch(location.href,{method:'POST',body:fd,credentials:'same-origin'}).then(function(r){
   return r.text().then(function(html){
    var doc=new DOMParser().parseFromString(html,'text/html'),err=doc.getElementById('SaveError');
    copyState(doc);
    if(r.ok&&r.redirected&&!(err&&err.textContent.trim())){banner('');status('All changes saved','ok');}
    else{var m=err&&err.textContent.trim();banner(m||'Changes could not be saved automatically. Use Save.');status('Not saved','error');}
   });
  }).catch(function(){status('Not saved \u2014 check your connection','error');}).then(function(){busy=false;if(again){again=false;run();}});
 }
 document.addEventListener('change',function(e){
  var el=e.target;if(!el||!el.closest)return;
  if(el.type==='file'||!el.closest('[id$="BusinessDetailsCard"]'))return;
  clearTimeout(timer);status('Unsaved changes','pending');timer=setTimeout(run,700);
 });
})();
