// 2026-10-25 v16: employee editor Address Lookup (Add Business design) - country flag/hint, clear button and the
// "Enter Address Manually" modal, which copies its values into the Address Details fields (server IDs unchanged).
(function(){
 'use strict';
 function root(){return document.getElementById('amEmpAddress');}
 function field(name){var r=root();return r?r.querySelector('[data-addr-field="'+name+'"]'):null;}
 function fieldVal(r,name){var el=r&&r.querySelector('[data-addr-field="'+name+'"]');return el?(el.value||'').trim():'';}
 // v83: hide the Address Not Found card once the user starts a manual address or has filled the required fields.
 function hasManualAddress(r){return !!(fieldVal(r,'StreetNo')&&fieldVal(r,'Line1')&&fieldVal(r,'City')&&fieldVal(r,'Province')&&fieldVal(r,'PostalCode'));}
 function dismissNotFound(r){
  if(!r)return;
  Array.prototype.forEach.call(r.querySelectorAll('.am-al-nf'),function(n){n.hidden=true;});
  var flag=r.querySelector('[data-addr-manual-flag],input[id$="AddressManual"]');
  if(flag)flag.value='1';
 }
 function maybeDismiss(){
  Array.prototype.forEach.call(document.querySelectorAll('.am-emp-al46,#amEmpAddress'),function(r){
   var flag=r.querySelector('[data-addr-manual-flag],input[id$="AddressManual"]');
   if((flag&&flag.value==='1')||hasManualAddress(r))dismissNotFound(r);
  });
 }
 function modal(){return document.getElementById('amEmpNewAddress');}
 function clone(sel,src){
  if(!src)return;sel.innerHTML='';
  Array.prototype.forEach.call(src.options,function(o){var n=document.createElement('option');n.value=o.value;n.textContent=o.textContent;sel.appendChild(n);});
 }
 function setSelect(sel,value){
  var v=(value||'').trim(),hit=null;
  Array.prototype.forEach.call(sel.options,function(o){if(!hit&&(o.value===v||o.value.toUpperCase()===v.toUpperCase()))hit=o;});
  if(!hit&&v){hit=document.createElement('option');hit.value=v;hit.textContent=v;sel.appendChild(hit);}
  sel.value=hit?hit.value:(sel.options.length?sel.options[0].value:'');
 }
 function country(){var c=document.getElementById('amEmpLookupCountry');return c?c.value:'Canada';}
 function syncCountry(){
  var us=country()==='United States',f=document.getElementById('amEmpLookupFlag'),h=document.getElementById('amEmpLookupHint'),p=field('LookupPostal');
  if(f)f.classList.toggle('is-us',us);
  if(h)h.textContent='Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).'; // v49 Pandey notes: one hint for CA + US
  if(p)p.placeholder=us?'#####':'X#X #X#'; // v86: X/# masks instead of sample values
  var pc=field('PostalCode'),pcc=field('Country');if(pc&&pcc)pc.placeholder=pcc.value==='United States'?'#####':'X#X #X#';
 }
 var back=null;
 var Lookup={
  open:function(){
   var m=modal();if(!m)return;var err=document.getElementById('amEmpNewAddressError');if(err){err.hidden=true;err.textContent='';}
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(el){
    var name=el.getAttribute('data-addr'),src=field(name);
    if(el.tagName==='SELECT'){if(el.getAttribute('data-clone'))clone(el,src);setSelect(el,src?src.value:'');}
    else el.value=src?src.value:'';
   });
   var pc=m.querySelector('[data-addr="PostalCode"]'),lp=field('LookupPostal');if(pc&&!pc.value&&lp)pc.value=lp.value.trim().toUpperCase();
   var co=m.querySelector('[data-addr="Country"]'),fc=field('Country');if(co&&(!fc||!field('Line1')||!field('Line1').value))setSelect(co,country());
   back=document.activeElement;m.hidden=false;document.body.classList.add('am-addr-open');
   var first=m.querySelector('[data-addr="StreetNo"]');if(first)first.focus();
  },
  close:function(){var m=modal();if(!m||m.hidden)return;m.hidden=true;document.body.classList.remove('am-addr-open');if(back&&back.focus)back.focus();},
  apply:function(){
   var m=modal();if(!m)return;var missing=[];
   Array.prototype.forEach.call(m.querySelectorAll('[data-req]'),function(el){if(!(el.value||'').trim()){missing.push(el.closest('label').querySelector('span').firstChild.textContent.trim());el.setAttribute('aria-invalid','true');}else el.removeAttribute('aria-invalid');});
   var err=document.getElementById('amEmpNewAddressError');
   if(missing.length){if(err){err.textContent='Please complete: '+missing.join(', ')+'.';err.hidden=false;}return;}
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(el){
    var dst=field(el.getAttribute('data-addr'));if(!dst)return;
    if(dst.tagName==='SELECT')setSelect(dst,el.value);else dst.value=el.value.trim();
    try{dst.dispatchEvent(new Event('change',{bubbles:true}));}catch(e){}
   });
   Lookup.close();
   dismissNotFound(root());
   var card=root()&&root().querySelector('.am-al-details');if(card&&card.scrollIntoView)card.scrollIntoView({block:'start',behavior:'smooth'});
  }
 };
 // v46: also the Business-look Employee lookup (.am-emp-al46, Employee Editor and Employee Profile).
// v17 (Pandey image1/image5): "+ Add New Address" clears the Employee Address fields in place, copies the searched
 // postal code and focuses Street Number (no modal, no postback). Works for any .am-al2 block on the page.
 Lookup.newAddress=function(btn){
  var r=(btn&&btn.closest&&btn.closest('.am-al2,.am-emp-al46'))||root();if(!r)return;
  var get=function(n){return r.querySelector('[data-addr-field="'+n+'"]');};
  ['StreetNo','Line1','Line2','SuiteNumber','SuiteType','City','Province','PostalCode'].forEach(function(n){
   var el=get(n);if(!el)return;if(el.tagName==='SELECT')el.selectedIndex=0;else el.value='';
   try{el.dispatchEvent(new Event('change',{bubbles:true}));}catch(e){}
  });
  var co=get('Country');if(co&&co.tagName==='SELECT')setSelect(co,'Canada');
  var lp=get('LookupPostal'),pc=get('PostalCode');if(lp&&pc)pc.value=lp.value.trim().toUpperCase();
  var ch=r.querySelector('.am-al2-choice select');
  // v46/v83: the Address Not Found card is only for the last search; manual entry dismisses it and keeps it dismissed.
  dismissNotFound(r);if(ch){for(var i=0;i<ch.options.length;i++)if(ch.options[i].value==='new'){ch.selectedIndex=i;break;}}
  var first=get('StreetNo');if(first){if(first.scrollIntoView)first.scrollIntoView({block:'center'});first.focus();}
 };
 // v17: EmployeeProfile keeps its server province TextBox (hidden) behind a Province/State select; keep both in step,
 // also after the Address tab UpdatePanel refreshes.
 function syncProvince(){
  Array.prototype.forEach.call(document.querySelectorAll('select[data-al2-mirror]'),function(sel){
   var box=document.getElementById(sel.getAttribute('data-al2-mirror'));if(!box)return;
   setSelect(sel,(box.value||'').trim().toUpperCase());
   if(!sel.getAttribute('data-al2-bound')){sel.setAttribute('data-al2-bound','1');sel.addEventListener('change',function(){var b=document.getElementById(sel.getAttribute('data-al2-mirror'));if(b)b.value=sel.value;});}
  });
 }
 Lookup.syncProvince=syncProvince;
 window.AnyminuteAddressLookup=Lookup;
 document.addEventListener('click',function(e){
  var t=e.target;if(!t||!t.closest)return;
  if(t.closest('#amEmpNewAddress [data-addr-close]')){e.preventDefault();Lookup.close();return;}
  if(t.closest('#amEmpNewAddress [data-addr-apply]')){e.preventDefault();Lookup.apply();return;}
  if(t.id==='amEmpNewAddress'){Lookup.close();return;}
  var cl=t.closest('.am-emp-al46 [data-al-clear],#amEmpAddress [data-al-clear]');
  if(cl){e.preventDefault();var w=cl.closest('.am-emp-al46')||root(),p=w&&w.querySelector('[data-addr-field="LookupPostal"]');if(p){p.value='';p.focus();}}
 });
 document.addEventListener('keydown',function(e){if(e.key==='Escape'){var m=modal();if(m&&!m.hidden)Lookup.close();}});
 document.addEventListener('change',function(e){if(e.target&&e.target.id==='amEmpLookupCountry'){try{sessionStorage.setItem('amEmpLookupCountry',e.target.value);}catch(x){}syncCountry();}else if(e.target&&e.target.getAttribute&&e.target.getAttribute('data-addr-field')==='Country')syncCountry();});
 function init(){
  var c=document.getElementById('amEmpLookupCountry'),fc=field('Country');
  var saved=null;try{saved=sessionStorage.getItem('amEmpLookupCountry');}catch(x){}
  if(c&&(saved||(fc&&fc.value)))setSelect(c,saved||fc.value);
  syncCountry();
 }
 function init2(){init();syncProvince();maybeDismiss();document.addEventListener('input',maybeDismiss);document.addEventListener('change',maybeDismiss);try{if(window.Sys&&Sys.WebForms&&Sys.WebForms.PageRequestManager)Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function(){init();syncProvince();maybeDismiss();});}catch(e){}}
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init2);else init2();
})();
