(function(root){
 'use strict';
 function fileError(file){
  if(!file||!file.size)return 'Choose a PNG or JPEG image.';
  if(file.size>2*1024*1024)return 'Choose an image no larger than 2 MB.';
  if(!/\.(png|jpe?g)$/i.test(file.name)||file.type&&!/^image\/(png|jpeg)$/i.test(file.type))return 'Choose a PNG or JPEG image.';
  return '';
 }
 function init(){
  var editor=document.getElementById('amBusinessEditor');if(!editor)return;
  var input=editor.querySelector('input[type=file]'),preview=document.getElementById('previewImage'),area=document.getElementById('uploadArea'),error=document.getElementById('business-logo-error');
  if(input&&!input.dataset.amBusinessBound){
   input.dataset.amBusinessBound='true';
   var previous=preview.getAttribute('src')||'',baseClass=area.className;
   // The circle shows the picture when there is one, otherwise the initials/upload prompt.
   function mark(){var src=preview.getAttribute('src')||'';area.className=src?baseClass.replace(/\s*has-initials/,'').replace(/\s*has-logo/,'')+' has-logo':baseClass;}
   var setSrc=function(v){preview.setAttribute('src',v);mark();};
   function show(){
    var selected=input.files&&input.files[0];
    if(!selected){error.hidden=true;setSrc(previous);return;}
    var message=fileError(selected);error.textContent=message;error.hidden=!message;
    if(message){input.value='';setSrc(previous);return;}
    var reader=new FileReader();reader.onload=function(){if(input.files&&input.files[0]===selected)setSrc(reader.result);};
    reader.onerror=function(){if(input.files&&input.files[0]===selected){input.value='';setSrc(previous);error.textContent='The logo could not be read. Choose another PNG or JPEG.';error.hidden=false;}};
    reader.readAsDataURL(selected);
   }
   input.addEventListener('change',show);
   area.addEventListener('dragover',function(e){e.preventDefault();area.dataset.dragover='true';});
   area.addEventListener('dragleave',function(){delete area.dataset.dragover;});
   area.addEventListener('drop',function(e){
    e.preventDefault();delete area.dataset.dragover;
    var files=e.dataTransfer&&e.dataTransfer.files;
    if(!files||files.length!==1){error.textContent='Choose one logo image.';error.hidden=false;return;}
    var message=fileError(files[0]);if(message){error.textContent=message;error.hidden=false;return;}
    try{input.files=files;show();}catch(unavailable){error.textContent='Use Choose File to select the logo.';error.hidden=false;}
   });
  }
  var flash=document.getElementById('BusinessFlash')||document.getElementById('SaveError');
  if(flash&&!flash.dataset.amShown){flash.dataset.amShown='true';flash.setAttribute('tabindex','-1');try{flash.focus({preventScroll:false});}catch(x){flash.focus();}}
  var notice=document.getElementById('AddressMessage');
  if(notice&&notice.textContent&&notice.dataset.amNotice!==notice.textContent){notice.dataset.amNotice=notice.textContent;notice.focus();}
  addressInit();
  Lock.init();
 }

 // 2026-10-25 v11 Deck H (Address Lookup + Enter Address Manually), following the slides.
 var REGIONS={
  'Canada':[['AB','Alberta'],['BC','British Columbia'],['MB','Manitoba'],['NB','New Brunswick'],['NL','Newfoundland and Labrador'],['NT','Northwest Territories'],['NS','Nova Scotia'],['NU','Nunavut'],['ON','Ontario'],['PE','Prince Edward Island'],['QC','Quebec'],['SK','Saskatchewan'],['YT','Yukon']],
  'United States':[['AL','Alabama'],['AK','Alaska'],['AZ','Arizona'],['AR','Arkansas'],['CA','California'],['CO','Colorado'],['CT','Connecticut'],['DE','Delaware'],['DC','District of Columbia'],['FL','Florida'],['GA','Georgia'],['HI','Hawaii'],['ID','Idaho'],['IL','Illinois'],['IN','Indiana'],['IA','Iowa'],['KS','Kansas'],['KY','Kentucky'],['LA','Louisiana'],['ME','Maine'],['MD','Maryland'],['MA','Massachusetts'],['MI','Michigan'],['MN','Minnesota'],['MS','Mississippi'],['MO','Missouri'],['MT','Montana'],['NE','Nebraska'],['NV','Nevada'],['NH','New Hampshire'],['NJ','New Jersey'],['NM','New Mexico'],['NY','New York'],['NC','North Carolina'],['ND','North Dakota'],['OH','Ohio'],['OK','Oklahoma'],['OR','Oregon'],['PA','Pennsylvania'],['RI','Rhode Island'],['SC','South Carolina'],['SD','South Dakota'],['TN','Tennessee'],['TX','Texas'],['UT','Utah'],['VT','Vermont'],['VA','Virginia'],['WA','Washington'],['WV','West Virginia'],['WI','Wisconsin'],['WY','Wyoming']]
 };
 var POSTAL={'Canada':/^[A-Z]\d[A-Z]\d[A-Z]\d$/,'United States':/^\d{5}(\d{4})?$/};
 function byId(id){return document.getElementById(id);}
 function norm(v){return String(v||'').replace(/\s+/g,' ').trim().toUpperCase();}
 function countryOf(v){v=norm(v);if(v==='UNITED STATES'||v==='USA'||v==='US'||v==='UNITED STATES OF AMERICA')return 'United States';if(v===''||v==='CANADA'||v==='CA')return 'Canada';return '';}
 // Province/State options depend on the country (deck note: Florida = United States). Values are stored upper-case like lookup results.
 function fillRegions(sel,country,current){
  var list=REGIONS[country]||[],cur=norm(current),hit='';
  sel.innerHTML='';var o=document.createElement('option');o.value='';o.textContent='Please Select';sel.appendChild(o);
  list.forEach(function(r){var opt=document.createElement('option');opt.value=r[1].toUpperCase();opt.textContent=r[1];sel.appendChild(opt);if(cur&&(cur===r[0]||cur===r[1].toUpperCase()))hit=opt.value;});
  if(cur&&!hit){var keep=document.createElement('option');keep.value=cur;keep.textContent=current;sel.appendChild(keep);hit=cur;}
  sel.value=hit;
 }
 function syncFromServer(){
  var cSel=byId('amCountrySel'),cBox=byId('Country'),pSel=byId('amProvinceSel'),pBox=byId('txtProvince');if(!cSel||!cBox||!pSel||!pBox)return;
  var c=countryOf(cBox.value);
  if(!c){c=cBox.value.trim();if(!Array.prototype.some.call(cSel.options,function(x){return x.value===c;})){var o=document.createElement('option');o.value=o.textContent=c;cSel.appendChild(o);}}
  cSel.value=c;cBox.value=c;
  fillRegions(pSel,c,pBox.value);pBox.value=pSel.value;
 }
 function snapshot(){
  var ids=['txtStreetNo','txtStreetName','txtStreetType','ddlSuitType','txtSuiteNumber','txtCity','txtProvince','txtPostalCode','Country'],v={};
  ids.forEach(function(id){var f=byId(id);if(f)v[id]=f.value;});return v;
 }
 function setFlag(){
  var c=byId('LookupCountry'),f=byId('amLookupFlag'),q=byId('txtSearchPostalCode');if(!c||!f)return;
  var us=c.value==='United States';f.className='am-al-flag '+(us?'is-us':'is-ca');
  if(q)q.setAttribute('placeholder',us?'e.g. 33185':'e.g. M1B 2W1');
 }
 function addressInit(){
  if(!byId('amCountrySel'))return;
  syncFromServer();setFlag();
  if(!Address.saved)Address.saved=snapshot();
  var md=byId('amMdCountry'),mp=document.querySelector('#amNewAddress [data-province-for]');
  if(md&&mp&&!mp.options.length)fillRegions(mp,md.value,'');
 }
 var Address={
  saved:null,
  openList:function(){setTimeout(Address.showList,80);},
  showList:function(){
   // Deck: the matching addresses are shown as an open list under the field.
   var s=byId('AddressChoices');if(!s||s.disabled||s.options.length<2)return;
   s.size=Math.min(s.options.length,7);s.classList.add('am-list-open');s.focus();
   var close=function(){s.removeAttribute('size');s.classList.remove('am-list-open');s.removeEventListener('change',close);};
   s.addEventListener('change',close);
  },
  openNew:function(){
   var m=byId('amNewAddress');if(!m)return;
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(f){f.value=f.getAttribute('data-addr')==='Country'?'Canada':'';f.removeAttribute('aria-invalid');});
   var mp=m.querySelector('[data-province-for]');if(mp)fillRegions(mp,'Canada','');
   byId('amNewAddressError').hidden=true;
   m.hidden=false;document.body.classList.add('am-addr-open');Address.back=document.activeElement;
   // v86: full page, not a pop-up - hide the rest of the page content (restored on close; form fields stay in the form).
   Address.hid=[];var root=document.querySelector('main.am-main')||byId('page-content-wrapper')||document.body;for(var n=m;n&&n!==root&&n.parentNode;n=n.parentNode){Array.prototype.forEach.call(n.parentNode.children,function(x){if(x!==n&&x.tagName!=='SCRIPT'&&x.tagName!=='STYLE'&&!x.hidden&&x.style.display!=='none'){x.setAttribute('data-am-addr-hid','1');x.style.display='none';Address.hid.push(x);}});if(n.parentNode===root)break;}
   window.scrollTo(0,0);
   var first=m.querySelector('[data-addr]');if(first)first.focus();
  },
  close:function(){var m=byId('amNewAddress');if(!m||m.hidden)return;m.hidden=true;document.body.classList.remove('am-addr-open');(Address.hid||[]).forEach(function(x){x.style.display='';x.removeAttribute('data-am-addr-hid');});Address.hid=[];if(Address.back&&Address.back.focus)Address.back.focus();},
  apply:function(){
   var m=byId('amNewAddress'),missing=[],err=byId('amNewAddressError');
   Array.prototype.forEach.call(m.querySelectorAll('[data-req]'),function(f){var bad=!f.value.trim();if(bad)f.setAttribute('aria-invalid','true');else f.removeAttribute('aria-invalid');if(bad)missing.push(f.parentNode.firstChild.textContent.replace('*','').trim());});
   if(missing.length){err.textContent='Enter '+missing.join(', ')+'.';err.hidden=false;m.querySelector('[aria-invalid]').focus();return;}
   var country=m.querySelector('[data-addr=Country]').value,pf=m.querySelector('[data-addr=txtPostalCode]'),pc=pf.value.replace(/[\s-]+/g,'').toUpperCase();
   if(pc&&POSTAL[country]&&!POSTAL[country].test(pc)){pf.setAttribute('aria-invalid','true');err.textContent=country==='United States'?'Enter a valid US ZIP code (e.g. 33185).':'Enter a valid Canadian postal code (e.g. M1B 2W1).';err.hidden=false;pf.focus();return;}
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(f){
    var t=byId(f.getAttribute('data-addr'));if(!t)return;
    var v=f.value.trim();if(t.id==='txtPostalCode'||t.id==='txtProvince'||t.id==='txtCity'||t.id==='txtStreetName')v=v.toUpperCase();
    if(t.id==='txtPostalCode')v=v.replace(/\s+/g,'');
    t.value=v;
   });
   syncFromServer();
   Address.close();
   if(root.AnyminuteBusinessLock)root.AnyminuteBusinessLock.unlockCard('address',false);
   var h=byId('business-address-heading');if(h){h.setAttribute('tabindex','-1');h.focus();}
  },
  // Address card Cancel: discard edits and keep the saved address (nothing is written to the database).
  cancel:function(){
   var v=Address.saved||{};Object.keys(v).forEach(function(id){var f=byId(id);if(f)f.value=v[id];});
   var r=byId('AddressChoices');if(r)r.selectedIndex=0;
   var n=byId('AddressMessage');if(n)n.hidden=true;
   syncFromServer();
   if(root.AnyminuteBusinessLock)root.AnyminuteBusinessLock.relock('address');
  },
  // Address card Save Changes: runs the page's existing server save (business + address in one transaction).
  save:function(){
   var b=document.querySelector('#amBusinessEditor [id$=btnUpdateBusinessDetails]')||document.querySelector('#amBusinessEditor [id$=btnSaveBusinessDetails]');
   if(b)b.click();
  }
 };
 root.AnyminuteBusinessAddress=Address;

 // 2026-10-25 v16: pencil unlock. When editing an existing business, Business Details and Address Details
 // fields start read-only; the pencil in a field (or the Edit pencil in the card heading) makes them editable.
 // Fields use readOnly (not disabled) so their values still post back; delegated handlers survive UpdatePanel refreshes.
 var PENCIL='<svg viewBox="0 0 24 24" width="16" height="16" aria-hidden="true" focusable="false"><path fill="currentColor" d="M3 17.25V21h3.75L17.8 9.94l-3.75-3.75L3 17.25zM20.7 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>';
 var Lock={
  open:{},
  cards:function(){
   var out=[],b=document.querySelector('#amBusinessEditor [id$=BusinessDetailsCard]'),a=document.querySelector('#amBusinessEditor .am-al-details');
   if(b)out.push({key:'business',el:b,fields:'.am-business-grid .am-business-field',head:'h2'});
   if(a)out.push({key:'address',el:a,fields:'.am-al-grid .am-al-f',head:'.am-al-details-head'});
   return out;
  },
  editing:function(){return !!document.querySelector('#amBusinessEditor [id$=btnUpdateBusinessDetails]');},
  control:function(field){return field.querySelector('input:not([type=hidden]):not([type=file]),select');},
  init:function(){
   if(!Lock.editing())return;
   Lock.cards().forEach(function(card){
    Array.prototype.forEach.call(card.el.querySelectorAll(card.fields),function(field){
     var input=Lock.control(field);if(!input||field.getAttribute('data-am-lockable'))return;
     field.setAttribute('data-am-lockable',card.key);
     var wrap=document.createElement('span');wrap.className='am-v16-lockwrap';input.parentNode.insertBefore(wrap,input);wrap.appendChild(input);
     var b=document.createElement('button');b.type='button';b.className='am-v16-pencil';b.setAttribute('data-am-unlock','field');
     var label=field.querySelector('label');b.setAttribute('aria-label','Edit '+(label?label.textContent.replace(/[*]|\(Required\)|\(Optional\)/g,'').trim():'field'));b.title='Edit';b.innerHTML=PENCIL;
     wrap.appendChild(b);
    });
    var head=card.el.querySelector(card.head);
    if(head&&!head.querySelector('.am-v16-cardedit')){
     var e=document.createElement('button');e.type='button';e.className='am-v16-cardedit';e.setAttribute('data-am-unlock','card');e.setAttribute('data-am-card',card.key);
     e.innerHTML=PENCIL+'<span>Edit</span>';e.setAttribute('aria-label','Edit '+(card.key==='business'?'business details':'address details'));
     head.appendChild(e);
    }
    Lock.apply(card.key);
   });
  },
  fields:function(key){return document.querySelectorAll('#amBusinessEditor [data-am-lockable="'+key+'"]');},
  set:function(field,locked){
   var input=Lock.control(field),b=field.querySelector('.am-v16-pencil');if(!input)return;
   field.classList.toggle('am-v16-locked',locked);
   if(input.tagName==='SELECT'){if(locked){input.setAttribute('aria-readonly','true');input.setAttribute('tabindex','-1');}else{input.removeAttribute('aria-readonly');input.removeAttribute('tabindex');}}
   else input.readOnly=locked;
   if(b)b.hidden=!locked;
  },
  apply:function(key){
   var open=!!Lock.open[key];
   Array.prototype.forEach.call(Lock.fields(key),function(f){Lock.set(f,!open&&!f.getAttribute('data-am-open'));});
   var e=document.querySelector('#amBusinessEditor .am-v16-cardedit[data-am-card="'+key+'"]');if(e)e.hidden=open;
  },
  unlockCard:function(key,focus){
   if(!Lock.editing())return;Lock.open[key]=true;Lock.apply(key);
   if(focus){var f=Lock.fields(key)[0],c=f&&Lock.control(f);if(c)c.focus();}
  },
  relock:function(key){
   Lock.open[key]=false;Array.prototype.forEach.call(Lock.fields(key),function(f){f.removeAttribute('data-am-open');});Lock.apply(key);
  }
 };
 root.AnyminuteBusinessLock=Lock;
 if(typeof document!=='undefined'){
  document.addEventListener('click',function(e){var t=e.target;if(!t.closest)return;
   var b=t.closest('[data-am-unlock]');if(!b)return;e.preventDefault();
   if(b.getAttribute('data-am-unlock')==='card'){Lock.unlockCard(b.getAttribute('data-am-card'),true);return;}
   var field=b.closest('[data-am-lockable]');if(!field)return;
   field.setAttribute('data-am-open','1');Lock.set(field,false);var c=Lock.control(field);if(c)c.focus();
  });
  document.addEventListener('keydown',function(e){var t=e.target;
   if(t&&t.tagName==='SELECT'&&t.closest&&t.closest('.am-v16-locked')&&e.key!=='Tab')e.preventDefault();
  },true);
  document.addEventListener('mousedown',function(e){var t=e.target;
   if(t&&t.tagName==='SELECT'&&t.closest&&t.closest('.am-v16-locked'))e.preventDefault();
  },true);
 }
 if(typeof document!=='undefined'){
  document.addEventListener('click',function(e){var t=e.target;if(!t.closest)return;
   if(t.closest('[data-addr-close]')||t.id==='amNewAddress'){e.preventDefault();Address.close();}
   else if(t.closest('[data-addr-apply]')){e.preventDefault();Address.apply();}
   else if(t.closest('[data-al-clear]')){e.preventDefault();var q=byId('txtSearchPostalCode');if(q){q.value='';q.focus();}}
   else if(t.closest('[data-al-cancel]')){e.preventDefault();Address.cancel();}
   else if(t.closest('[data-al-save]')){e.preventDefault();Address.save();}
  });
  document.addEventListener('change',function(e){var t=e.target;
   if(t.id==='amCountrySel'){var c=byId('Country'),p=byId('txtProvince');if(c)c.value=t.value;var ps=byId('amProvinceSel');if(ps){fillRegions(ps,t.value,p?p.value:'');if(p)p.value=ps.value;}}
   else if(t.id==='amProvinceSel'){var pb=byId('txtProvince');if(pb)pb.value=t.value;}
   else if(t.id==='LookupCountry')setFlag();
   else if(/AddressChoices$/.test(t.id||'')&&t.value&&root.AnyminuteBusinessLock)root.AnyminuteBusinessLock.open.address=true;
   else if(t.id==='amMdCountry'){var mp=document.querySelector('#amNewAddress [data-province-for]');if(mp)fillRegions(mp,t.value,mp.value);}
  });
  document.addEventListener('keydown',function(e){var m=byId('amNewAddress');if(!m||m.hidden)return;if(e.key==='Escape')Address.close();else if(e.key==='Enter'&&e.target.closest&&e.target.closest('#amNewAddress')&&e.target.tagName!=='BUTTON'&&e.target.tagName!=='SELECT'){e.preventDefault();Address.apply();}});
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={fileError:fileError};
 if(typeof document!=='undefined'){
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
  if(root.Sys&&root.Sys.Application)root.Sys.Application.add_load(init);
 }
})(typeof window!=='undefined'?window:globalThis);
