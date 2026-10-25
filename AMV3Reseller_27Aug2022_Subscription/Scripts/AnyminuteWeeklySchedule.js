// 2026-10-25 v11 Deck I: Weekly Employee Schedule - live Total Hours, Day Off rows, Repeat Weekly options.
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
  var working=type.value==='Working';
  type.classList.toggle('is-working',working);type.classList.toggle('is-off',!working);tr.classList.toggle('am-ws-off',!working);
  [start,end,brk,notes].forEach(function(f){if(f)f.disabled=!working;});
  var h=hours(type.value,start.value,end.value,brk.value);out.textContent=h.toFixed(2);return h;
 }
 function total(){
  var sum=0;Array.prototype.forEach.call(document.querySelectorAll('#amWeeklySchedule .am-ws-row'),function(tr){sum+=row(tr);});
  var t=document.getElementById('amWsTotal');if(t)t.textContent=sum.toFixed(2);
 }
 function repeat(){
  var on=document.getElementById('RepeatOn');if(!on)return;
  var box=document.getElementById('amWsRepeat');box.classList.toggle('am-ws-repeat-on',on.checked);
  ['RepeatUntil','ApplyAll','ApplyWeekdays','ApplySelected'].forEach(function(id){var f=document.getElementById(id);if(f)f.disabled=!on.checked;});
  var sel=document.getElementById('ApplySelected'),days=document.getElementById('amWsDays'),enabled=on.checked&&sel&&sel.checked;
  if(days){days.classList.toggle('am-ws-disabled',!enabled);if(!enabled)days.open=false;
   Array.prototype.forEach.call(days.querySelectorAll('input'),function(i){i.disabled=!enabled;});
   var picked=Array.prototype.filter.call(days.querySelectorAll('input:checked'),function(){return true;}).map(function(i){var l=days.querySelector('label[for="'+i.id+'"]');return l?l.textContent:'';});
   document.getElementById('amWsDaysText').textContent=picked.length?picked.join(', '):'Please Select';}
 }
 function init(){
  if(!document.getElementById('amWeeklySchedule'))return;
  total();repeat();
 }
 if(typeof document!=='undefined'){
  document.addEventListener('change',function(e){
   var t=e.target;if(!t.closest||!t.closest('#amWeeklySchedule'))return;
   if(t.classList.contains('am-ws-type')&&t.value==='Working'){
    var tr=t.closest('tr'),s=tr.querySelector('.am-ws-start'),x=tr.querySelector('.am-ws-end'),b=tr.querySelector('.am-ws-break');
    if(!s.value)s.value='09:00';if(!x.value)x.value='17:00';if(!b.value){b.value='30';}
   }
   if(t.closest('.am-ws-row'))total();else repeat();
  });
  document.addEventListener('input',function(e){if(e.target.closest&&e.target.closest('#amWeeklySchedule .am-ws-row'))total();});
  document.addEventListener('click',function(e){var d=document.getElementById('amWsDays');if(d&&d.open&&!d.contains(e.target))d.open=false;if(d&&e.target.closest&&e.target.closest('#amWsDays summary')&&d.classList.contains('am-ws-disabled'))e.preventDefault();});
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={hours:hours};
})(typeof window!=='undefined'?window:globalThis);
