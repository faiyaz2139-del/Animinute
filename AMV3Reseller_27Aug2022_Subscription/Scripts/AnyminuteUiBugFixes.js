(function(window,document){
 'use strict';
 function suffix(id){return document.getElementById(id)||document.querySelector('[id$="_'+id+'"]');}
 function refresh(){
  var main=document.getElementById('amPage')||document.querySelector('main.am-main');if(!main)return;
  var path=location.pathname.split('/').pop().toLowerCase();
  // Never move/clone server controls or expose server-hidden controls.
  main.querySelectorAll('a').forEach(function(a){if(/^open time card$/i.test(a.textContent.trim()))a.classList.add('am-open-card');});
  var postal=suffix('txtSearchPostalCode');
  if(postal){var td=postal.closest('td');if(td)td.classList.add('am-postal-cell');}
  if(['gridviewnewdesign.aspx','gridviewassignaccountant.aspx','gridviewnewdesign_assignuser.aspx'].indexOf(path)!==-1){
   main.querySelectorAll('table[id*="table_button"]').forEach(function(buttons){
    var row=buttons.closest('.row');if(!row)return;
    row.classList.add('am-assignment-board');
    Array.from(row.children).forEach(function(col){
     if(!col.textContent.trim()&&!col.querySelector('input,select,table,a'))col.hidden=true;
    });
   });
  }
  // Retain empty Ajax popup anchors in the DOM for their extenders.
  // Only blank anchor elements qualify; inputs and labelled/icon controls never do.
  main.querySelectorAll('a').forEach(function(a){
   if(!a.textContent.trim()&&!a.getAttribute('aria-label')&&!a.getAttribute('aria-labelledby')&&!a.getAttribute('title')&&
      !a.querySelector('img,svg,i,input,select,button,[role="img"]'))
   {var before=window.getComputedStyle(a,'::before').content,after=window.getComputedStyle(a,'::after').content;
    if([before,after].every(function(content){return !content||content==='none'||content==='normal'||content==='""';}))
     a.style.setProperty('display','none','important');}
  });
  if(path==='add_project.aspx'){
   ['ListBox_Ddl_Business_Name','ListBox_Business_Name'].forEach(function(id){
    var select=suffix(id),field=select&&select.closest('.form-group');
    if(field)field.classList.add('am-project-business-selector');
   });
  }
  // Some legacy pager templates contain icon-only links with missing icon fonts.
  // Label known navigation controls without replacing their anchor or handler.
  main.querySelectorAll('.dataTables_paginate a,.GridPager a').forEach(function(a){
   if(a.textContent.trim())return;
   var classes=a.className+' '+(a.parentElement?a.parentElement.className:'');
   var match=classes.match(/(?:^|\s)(first|previous|next|last)(?:\s|$)/i);
   if(match){a.textContent={first:'First',previous:'Previous',next:'Next',last:'Last'}[match[1].toLowerCase()];a.style.removeProperty('display');}
  });
  var confirm=suffix('divconfirm');
  if(confirm&&confirm.parentElement)confirm.parentElement.classList.add('am-confirm-email-row');
  document.querySelectorAll('.am-nav-parent>a[href="#"]').forEach(function(a){
   if(a.dataset.amBound)return;a.dataset.amBound='1';
   a.addEventListener('click',function(e){e.preventDefault();var b=a.parentElement.querySelector('.am-nav-expand');if(b)b.click();});
  });
  document.querySelectorAll('.am-business-status-menu').forEach(function(menu){
   if(menu.dataset.amFloat)return;menu.dataset.amFloat='1';
   menu.addEventListener('toggle',function(){positionMenu(menu);});
  });
  var modernToggle=document.querySelector('[data-toggle-sidebar]');
  if(modernToggle&&!modernToggle.dataset.amAria){modernToggle.dataset.amAria='1';modernToggle.setAttribute('aria-expanded',String(!window.matchMedia('(max-width:720px)').matches));}
  document.querySelectorAll('.am-nav-expand').forEach(function(button){
   if(button.dataset.amBound)return;button.dataset.amBound='1';
   button.addEventListener('click',function(){var open=button.parentElement.classList.toggle('am-expanded');button.setAttribute('aria-expanded',String(open));});
  });
 }
 function positionMenu(menu){
  if(!menu.open)return;var panel=menu.querySelector(':scope > div'),summary=menu.querySelector('summary');if(!panel||!summary)return;
  var rect=summary.getBoundingClientRect(),width=Math.min(menu.classList&&menu.classList.contains('am-employee-menu')?280:180,window.innerWidth-24);
  panel.style.position='fixed';panel.style.width=width+'px';panel.style.zIndex='1200';
  panel.style.left=Math.max(12,Math.min(rect.right-width,window.innerWidth-width-12))+'px';
  panel.style.top=Math.max(12,Math.min(rect.bottom+6,window.innerHeight-panel.offsetHeight-12))+'px';
 }
 window.addEventListener('resize',function(){document.querySelectorAll('.am-business-status-menu[open]').forEach(positionMenu);});
 document.addEventListener('scroll',function(){document.querySelectorAll('.am-business-status-menu[open]').forEach(positionMenu);},true);
 document.addEventListener('click',function(e){document.querySelectorAll('.am-header-help[open],.am-account-menu[open],.am-business-status-menu[open]').forEach(function(menu){if(!menu.contains(e.target))menu.open=false;});});
 document.addEventListener('keydown',function(e){if(e.key==='Escape')document.querySelectorAll('.am-header-help[open],.am-account-menu[open],.am-business-status-menu[open]').forEach(function(menu){menu.open=false;var summary=menu.querySelector('summary');if(summary)summary.focus();});});
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',refresh);else refresh();
 window.addEventListener('load',function(){refresh();if(window.Sys&&Sys.Application)Sys.Application.add_load(refresh);});
 window.AnyminuteUiBugFixes={refresh:refresh};
}(window,document));
