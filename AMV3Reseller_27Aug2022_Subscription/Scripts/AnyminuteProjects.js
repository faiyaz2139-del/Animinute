/* 2026-10-24 v8 Deck F: Projects list (kebab menus, group collapse, live search) and Manage Project Assignments
   (select all, selection counts, Selected Employees box, role/department/active filters, end-date popover). */
(function(){
 function $$(s,r){return Array.prototype.slice.call((r||document).querySelectorAll(s));}
 function closeMenus(except){$$('.am-pj-more.am-pj-open').forEach(function(m){if(m!==except){m.classList.remove('am-pj-open');var b=m.querySelector('.am-pj-kebab');if(b)b.setAttribute('aria-expanded','false');}});}
 function searchText(input){return (input.value||'').trim().toLowerCase();}
 var Pj=window.AnyminuteProjects={
  confirmSelected:function(name,question){
   var n=$$('input[name="'+name+'"]:checked').length;
   if(!n){alert(name==='add'?'Select at least one employee to assign.':'Select at least one assigned employee.');return false;}
   return question?confirm(question):true;
  }
 };
 function applyRows(table){
  if(!table)return;var q=table._pjQuery||'';var f=table._pjFilters||{};
  $$('tbody tr[data-pj-text]',table).forEach(function(tr){
   var ok=!q||tr.getAttribute('data-pj-text').toLowerCase().indexOf(q)>=0;
   if(ok&&f.role)ok=(tr.getAttribute('data-role')||'').split(',').map(function(s){return s.trim();}).indexOf(f.role)>=0;
   if(ok&&f.dept)ok=tr.getAttribute('data-dept')===f.dept;
   if(ok&&f.active)ok=tr.getAttribute('data-active')===f.active;
   tr.style.display=ok?'':'none';
  });
  $$('tbody.am-pj-group',table).forEach(function(g){var any=$$('tr.am-pj-row',g).some(function(r){return r.style.display!=='none';});g.style.display=any?'':'none';});
 }
 function counts(name){
  var boxes=$$('input[name="'+name+'"]'),n=boxes.filter(function(b){return b.checked;}).length;
  $$('[data-pj-n="'+name+'"]').forEach(function(s){s.textContent=n;});
  boxes.forEach(function(b){var tr=b.closest('tr');if(tr)tr.classList.toggle('am-pj-checked',b.checked);});
  $$('input[data-pj-all="'+name+'"]').forEach(function(a){var vis=boxes.filter(function(b){return b.closest('tr').style.display!=='none';});a.checked=vis.length>0&&vis.every(function(b){return b.checked;});a.indeterminate=!a.checked&&vis.some(function(b){return b.checked;});});
  if(name==='add')selectedBox(boxes);
 }
 function selectedBox(boxes){
  var body=document.getElementById('PjSelected');if(!body)return;
  $$('tr.am-pj-selrow',body).forEach(function(r){r.parentNode.removeChild(r);});
  var picked=boxes.filter(function(b){return b.checked;});
  var empty=body.querySelector('.am-pj-selempty');if(empty)empty.style.display=picked.length?'none':'';
  picked.forEach(function(b){
   var src=b.closest('tr'),tr=document.createElement('tr');tr.className='am-pj-selrow';
   ['.am-pj-cname','.am-pj-crole','.am-pj-cdept'].forEach(function(c){var td=document.createElement('td');var cell=src.querySelector(c);td.textContent=cell?cell.textContent:'';tr.appendChild(td);});
   var td=document.createElement('td');td.className='am-c';var x=document.createElement('button');x.type='button';x.className='am-pj-ic am-pj-red';x.title='Remove';x.setAttribute('aria-label','Remove '+(src.querySelector('.am-pj-cname')||{}).textContent);
   x.innerHTML='<svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16M9 7V4h6v3M6 7l1 13h10l1-13M10 11v6M14 11v6" fill="none" stroke="currentColor" stroke-width="2"/></svg>';
   x.addEventListener('click',function(){b.checked=false;counts('add');});td.appendChild(x);tr.appendChild(td);body.appendChild(tr);
  });
 }
 function init(){
  document.addEventListener('click',function(e){
   var k=e.target.closest('.am-pj-kebab');
   if(k){e.preventDefault();var m=k.closest('.am-pj-more');closeMenus(m);var open=m.classList.toggle('am-pj-open');k.setAttribute('aria-expanded',open?'true':'false');return;}
   if(!e.target.closest('.am-pj-menu'))closeMenus(null);
   var c=e.target.closest('.am-pj-chev');
   if(c){var g=c.closest('.am-pj-group');var col=g.classList.toggle('am-pj-collapsed');c.setAttribute('aria-expanded',col?'false':'true');c.setAttribute('aria-label',col?'Expand group':'Collapse group');return;}
   var t=e.target.closest('[data-pj-toggle]');
   if(t){var p=document.querySelector(t.getAttribute('data-pj-toggle'));if(p){if(p.hasAttribute('hidden'))p.removeAttribute('hidden');else p.setAttribute('hidden','hidden');var i=p.querySelector('input:not([type=hidden]):not(.am-date-native)');if(i&&!p.hasAttribute('hidden'))i.focus();}return;}
   var pop=document.querySelector('.am-pj-pop:not([hidden])');if(pop&&!e.target.closest('.am-pj-enddate'))pop.setAttribute('hidden','hidden');
  });
  document.addEventListener('keydown',function(e){if(e.key==='Escape'){closeMenus(null);$$('.am-pj-pop').forEach(function(p){p.setAttribute('hidden','hidden');});}});
  $$('[data-pj-search]').forEach(function(input){
   var table=document.querySelector(input.getAttribute('data-pj-search'));
   var go=function(){if(!table)return;table._pjQuery=searchText(input);applyRows(table);['sel','add'].forEach(function(n){if(table.querySelector('input[name="'+n+'"]'))counts(n);});};
   input.addEventListener('input',go);
   input.addEventListener('keydown',function(e){if(e.key==='Enter'&&input.type==='search'){e.preventDefault();go();}});
   if(input.value)go();
  });
  $$('select[data-pj-filter]').forEach(function(s){
   var table=document.getElementById('PjCandidates');
   var go=function(){if(!table)return;var f=table._pjFilters||(table._pjFilters={});f[s.getAttribute('data-pj-filter')]=s.value;applyRows(table);counts('add');};
   s.addEventListener('change',go);go();
  });
  ['sel','add'].forEach(function(n){
   $$('input[data-pj-all="'+n+'"]').forEach(function(a){a.addEventListener('change',function(){$$('input[name="'+n+'"]').forEach(function(b){if(b.closest('tr').style.display!=='none')b.checked=a.checked;});counts(n);});});
   $$('input[name="'+n+'"]').forEach(function(b){b.addEventListener('change',function(){counts(n);});});
   counts(n);
  });
  if(/[?&]unassign=1/.test(location.search)){var first=document.querySelector('input[name="sel"]');if(first)first.focus();}
 }
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
})();
