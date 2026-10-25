/* 2026-10-24 v9 Deck G: Accountant & Payroll Access filters, live search, counts and invite business search */
(function(){
 'use strict';
 function $$(s,r){return Array.prototype.slice.call((r||document).querySelectorAll(s));}
 function val(k){var e=document.querySelector('[data-ac-filter="'+k+'"]');return e?e.value:'';}
 function apply(){
  var table=document.getElementById('AcTable');if(!table)return;
  var q=((document.querySelector('[data-ac-search]')||{}).value||'').trim().toLowerCase();
  var biz=val('biz'),type=val('type'),st=val('status'),shown=0;
  $$('tr.am-ac-row',table).forEach(function(r){
   var s=r.getAttribute('data-status');
   var ok=(st?s===st:(s==='Active'||s==='Invited'))&&(!biz||r.getAttribute('data-biz').indexOf(','+biz+',')>=0)&&(!type||(' '+r.getAttribute('data-type')+' ').indexOf(' '+type+' ')>=0)&&(!q||r.getAttribute('data-ac-text').toLowerCase().indexOf(q)>=0);
   r.hidden=!ok;if(ok)shown++;
  });
  var none=table.querySelector('.am-ac-none');if(none)none.hidden=shown>0;
  var p=document.querySelector('[data-ac-showing]');
  if(p)p.textContent=shown?('Showing 1 to '+shown+' of '+shown+' user'+(shown===1?'':'s')):'Showing 0 users';
 }
 function bizSearch(){
  var box=document.querySelector('[data-ac-bizsearch]');if(!box)return;
  box.addEventListener('input',function(){var q=box.value.trim().toLowerCase();$$('.am-ac-biz').forEach(function(l){l.hidden=!!q&&l.getAttribute('data-ac-text').toLowerCase().indexOf(q)<0;});});
  box.addEventListener('keydown',function(e){if(e.key==='Enter')e.preventDefault();});
 }
 function init(){
  $$('[data-ac-filter]').forEach(function(e){e.addEventListener('change',apply);});
  var s=document.querySelector('[data-ac-search]');if(s){s.addEventListener('input',apply);s.addEventListener('keydown',function(e){if(e.key==='Enter')e.preventDefault();});}
  apply();bizSearch();
 }
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
})();
