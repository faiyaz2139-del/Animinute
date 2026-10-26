// 2026-10-25 v16: employee editor Address Lookup (Add Business design) - country flag/hint, clear button and the
// "Enter Address Manually" modal, which copies its values into the Address Details fields (server IDs unchanged).
(function(){
 'use strict';
 function root(){return document.getElementById('amEmpAddress');}
 function field(name){var r=root();return r?r.querySelector('[data-addr-field="'+name+'"]'):null;}
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
  if(h)h.textContent=us?'Enter a valid US ZIP code (e.g. 33185).':'Enter a valid Canadian postal code (e.g. M1B 2W1).';
  if(p)p.placeholder=us?'e.g. 33185':'e.g. M1B 2W1';
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
   var card=root()&&root().querySelector('.am-al-details');if(card&&card.scrollIntoView)card.scrollIntoView({block:'start',behavior:'smooth'});
  }
 };
 window.AnyminuteAddressLookup=Lookup;
 document.addEventListener('click',function(e){
  var t=e.target;if(!t||!t.closest)return;
  if(t.closest('#amEmpNewAddress [data-addr-close]')){e.preventDefault();Lookup.close();return;}
  if(t.closest('#amEmpNewAddress [data-addr-apply]')){e.preventDefault();Lookup.apply();return;}
  if(t.id==='amEmpNewAddress'){Lookup.close();return;}
  if(t.closest('#amEmpAddress [data-al-clear]')){e.preventDefault();var p=field('LookupPostal');if(p){p.value='';p.focus();}}
 });
 document.addEventListener('keydown',function(e){if(e.key==='Escape'){var m=modal();if(m&&!m.hidden)Lookup.close();}});
 document.addEventListener('change',function(e){if(e.target&&e.target.id==='amEmpLookupCountry'){try{sessionStorage.setItem('amEmpLookupCountry',e.target.value);}catch(x){}syncCountry();}});
 function init(){
  var c=document.getElementById('amEmpLookupCountry'),fc=field('Country');
  var saved=null;try{saved=sessionStorage.getItem('amEmpLookupCountry');}catch(x){}
  if(c&&(saved||(fc&&fc.value)))setSelect(c,saved||fc.value);
  syncCountry();
 }
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
})();
