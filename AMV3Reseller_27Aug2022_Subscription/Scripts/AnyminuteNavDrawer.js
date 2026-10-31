// 2026-10-24 v2 (old deck slide 3): hamburger = slide-in navigation drawer over the page.
// Runs in the capture phase so older collapse handlers on the same button do not also fire.
(function(window,document){
 'use strict';
 // v14 (tester doc item 6): exactly three plain bars, no rounded box around them.
 var ICON='<svg viewBox="0 0 24 24" width="26" height="26" aria-hidden="true" focusable="false"><path d="M4 6.5h16M4 12h16M4 17.5h16" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round"/></svg>';
 var SEL='[data-toggle-sidebar],#menu-toggle';
 function toggles(){return document.querySelectorAll(SEL);}
 function pinned(){return document.body.classList.contains('am-sa-pinned')&&window.innerWidth>=992;}
 function apDesk(){return document.body.classList.contains('am-ap-navbody')&&window.innerWidth>=992;}
 function set(open){
  if(apDesk())open=true; // v37 (PPT s1, strict): accountant sidebar is permanent on desktop
  document.body.classList.toggle('am-drawer-open',open);
  if(document.body.classList.contains('am-sa-pinned')){try{sessionStorage.setItem('amSaNav',open?'1':'0');}catch(x){}}
  document.body.classList.remove('am-nav-open','am-nav-collapsed');
  Array.prototype.forEach.call(toggles(),function(t){t.setAttribute('aria-expanded',String(open));});
 }
 function init(){
  if(!document.body||!toggles().length)return;
  document.body.classList.add('am-drawer-nav');
  if(document.querySelector('li.am-sa-nav'))document.body.classList.add('am-sa-navbody'); // v35: Support Admin drawer sizing
  // v37 (PPT s1, strict): accountant = light, always-visible icon sidebar on desktop; no hamburger there.
  if(document.querySelector('li.am-ap-nav')){document.body.classList.add('am-ap-navbody','am-sa-pinned');if(window.innerWidth>=992)document.body.classList.add('am-drawer-open');}
  // v36 (DOCX image6, strict): Support Admin = pinned sidebar on desktop, content shifted, no dark shade. Hamburger still collapses it.
  if(document.body.classList.contains('am-sa-navbody')&&!document.body.dataset.amSaPin){document.body.dataset.amSaPin='1';document.body.classList.add('am-sa-pinned');var keep=null;try{keep=sessionStorage.getItem('amSaNav');}catch(x){}if(window.innerWidth>=992&&keep!=='0')document.body.classList.add('am-drawer-open');}
  Array.prototype.forEach.call(toggles(),function(t){if(t.dataset.amDrawer)return;t.dataset.amDrawer='1';t.innerHTML=ICON;t.setAttribute('aria-label','Open or close navigation');});
  if(!document.querySelector('.am-drawer-shade')){var s=document.createElement('button');s.type='button';s.className='am-drawer-shade';s.setAttribute('aria-label','Close navigation');s.tabIndex=-1;s.addEventListener('click',function(){set(false);});document.body.appendChild(s);}
  set(document.body.classList.contains('am-drawer-open'));
 }
 document.addEventListener('click',function(e){
  var t=e.target.closest&&e.target.closest(SEL);
  if(t){e.preventDefault();e.stopImmediatePropagation();set(!document.body.classList.contains('am-drawer-open'));return;}
  if(document.body.classList.contains('am-drawer-open')&&!pinned()&&e.target.closest&&e.target.closest('.am-sidebar a[href]:not([href="#"]),#sidebar-wrapper a[href]:not([href="#"]):not([href^="javascript"])'))set(false);
 },true);
 document.addEventListener('keydown',function(e){if(e.key==='Escape'&&document.body.classList.contains('am-drawer-open')){set(false);var t=toggles()[0];if(t)t.focus();}});
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
 window.addEventListener('load',init);
 window.addEventListener('resize',function(){if(apDesk()&&!document.body.classList.contains('am-drawer-open'))set(true);});
}(window,document));
