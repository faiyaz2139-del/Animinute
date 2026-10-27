// 2026-10-25 v11/v13 Deck I: Weekly Employee Schedule - live Total Hours, Day Off rows, Repeat Weekly options.
// v23 (Pandey #3): cleared (unset) rows, Use default pattern, and a visible grid + Repeat reset on every context change.
(function(root){
 'use strict';
 function minutes(v){var m=/^(\d{1,2}):(\d{2})/.exec(v||'');return m?(+m[1])*60+(+m[2]):null;}
 function hours(type,start,end,brk){
  if(type!=='Working')return 0;
  var s=minutes(start),e=minutes(end);if(s===null||e===null||e<=s)return 0;
  var t=e-s-(parseInt(brk,10)||0);return t>0?t/60:0;
 }
 function row(tr){
  var type=tr.querySelector('.am-ws-type'),start=tr.querySelector('.am-ws-start'),end=tr.querySelector('.am-ws-end'),brk=tr.querySelector('.am-ws-break'),notes=tr.querySelector('.am-ws-notes'),out=tr.querySelector('.am-ws-hours');
  if(!type)return 0;
  var working=type.value==='Working',unset=type.value==='';
  type.classList.toggle('is-working',working);type.classList.toggle('is-off',!working&&!unset);type.classList.toggle('is-unset',unset);
  tr.classList.toggle('am-ws-off',!working&&!unset);tr.classList.toggle('am-ws-unset',unset);
  // v16: a read-only (employee) view keeps every input disabled.
  [start,end,brk,notes].forEach(function(f){if(f)f.disabled=!working||type.disabled;});
  var h=hours(type.value,start.value,end.value,brk.value);out.textContent=h.toFixed(2);return h;
 }
 function total(){
  var sum=0;Array.prototype.forEach.call(document.querySelectorAll('#amWeeklySchedule .am-ws-row'),function(tr){sum+=row(tr);});
  var t=document.getElementById('amWsTotal');if(t)t.textContent=sum.toFixed(2);
 }
 function repeat(){
  var on=document.getElementById('RepeatOn');if(!on)return;
  var box=document.getElementById('amWsRepeat');box.classList.toggle('am-ws-repeat-on',on.checked);
  // v13: Repeat Until / Apply To stay enabled (design shows them fully styled); the server only applies them when Repeat is ticked.
  ['RepeatUntil','ApplyAll','ApplyWeekdays','ApplySelected'].forEach(function(id){var f=document.getElementById(id);if(f)f.disabled=false;});
  var sel=document.getElementById('ApplySelected'),days=document.getElementById('amWsDays'),enabled=!!(sel&&sel.checked);
  if(days){days.classList.toggle('am-ws-disabled',!enabled);if(!enabled)days.open=false;
   Array.prototype.forEach.call(days.querySelectorAll('input'),function(i){i.disabled=!enabled;});
   var picked=Array.prototype.filter.call(days.querySelectorAll('input:checked'),function(){return true;}).map(function(i){var l=days.querySelector('label[for="'+i.id+'"]');return l?l.textContent:'';});
   document.getElementById('amWsDaysText').textContent=picked.length?picked.join(', '):'Please Select';}
 }
 function setSelect(sel,v){if(!sel)return;if(!Array.prototype.some.call(sel.options,function(o){return o.value===v;})){var o=document.createElement('option');o.value=v;o.textContent=v===''?(sel.classList.contains('am-ws-type')?'-- Select --':'--'):v+' min';sel.insertBefore(o,sel.firstChild);}sel.value=v;}
 function rows(){return Array.prototype.slice.call(document.querySelectorAll('#amWeeklySchedule .am-ws-row'));}
 // v23: fill Mon-Fri Working 09:00-17:00 (30 min), Sat/Sun Day Off.
 function useDefault(){
  rows().forEach(function(tr,i){var work=i<5;setSelect(tr.querySelector('.am-ws-type'),work?'Working':'DayOff');
   var s=tr.querySelector('.am-ws-start'),x=tr.querySelector('.am-ws-end'),b=tr.querySelector('.am-ws-break'),n=tr.querySelector('.am-ws-notes');
   if(s)s.value=work?'09:00':'';if(x)x.value=work?'17:00':'';setSelect(b,work?'30':'');if(n&&!work)n.value='';});
  total();
 }
 // v23: Business / Employee / Week / < > Today clear the grid and Repeat Weekly at once, while the new selection loads.
 function clearAll(){
  rows().forEach(function(tr){setSelect(tr.querySelector('.am-ws-type'),'');['.am-ws-start','.am-ws-end','.am-ws-notes'].forEach(function(q){var f=tr.querySelector(q);if(f)f.value='';});setSelect(tr.querySelector('.am-ws-break'),'');});
  var on=document.getElementById('RepeatOn'),u=document.getElementById('RepeatUntil'),all=document.getElementById('ApplyAll');
  if(on)on.checked=false;if(u)u.value='';if(all)all.checked=true;var d=document.getElementById('amWsDays');if(d)Array.prototype.forEach.call(d.querySelectorAll('input'),function(i){i.checked=false;});
  total();repeat();var w=document.getElementById('amWeeklySchedule');if(w)w.classList.add('am-ws-loading');
 }
 function init(){
  if(!document.getElementById('amWeeklySchedule'))return;
  total();repeat();
 }
 if(typeof document!=='undefined'){
  document.addEventListener('change',function(e){
   var t=e.target;if(!t.closest||!t.closest('#amWeeklySchedule'))return;
   if(t.id==='WsBusiness'||t.id==='WsEmployee'||t.id==='WsWeek'){clearAll();return;}
   if(t.classList.contains('am-ws-type')&&t.value==='Working'){
    var tr=t.closest('tr'),s=tr.querySelector('.am-ws-start'),x=tr.querySelector('.am-ws-end'),b=tr.querySelector('.am-ws-break');
    if(!s.value)s.value='09:00';if(!x.value)x.value='17:00';if(!b.value){b.value='30';}
   }
   // v13: choosing a Repeat Until date or an Apply To option turns Repeat on.
   if(t.closest('#amWsRepeat')&&t.id!=='RepeatOn'&&(t.id==='RepeatUntil'||t.name&&/ApplyTo$/.test(t.name)||t.closest('#amWsDays'))){var on=document.getElementById('RepeatOn');if(on&&!on.checked&&(t.id!=='RepeatUntil'||t.value))on.checked=true;}
   if(t.closest('.am-ws-row'))total();else repeat();
  });
  document.addEventListener('input',function(e){if(e.target.closest&&e.target.closest('#amWeeklySchedule .am-ws-row'))total();});
  document.addEventListener('click',function(e){if(e.target.closest&&e.target.closest('#amWsDefault')){e.preventDefault();useDefault();return;}if(e.target.closest&&e.target.closest('#amWeeklySchedule .am-ws-ctx'))clearAll();});
  document.addEventListener('click',function(e){var d=document.getElementById('amWsDays');if(d&&d.open&&!d.contains(e.target))d.open=false;if(d&&e.target.closest&&e.target.closest('#amWsDays summary')&&d.classList.contains('am-ws-disabled'))e.preventDefault();});
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={hours:hours,useDefault:useDefault,clearAll:clearAll};
})(typeof window!=='undefined'?window:globalThis);
