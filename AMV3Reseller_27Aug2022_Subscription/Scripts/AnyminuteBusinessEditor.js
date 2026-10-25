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
  lockFields();
 }

 // 2026-10-24 v10 address spec: open the result list after a search, Add New Address popup, pencil-locked Business Address fields.
 var PENCIL='<svg viewBox="0 0 24 24" width="15" height="15" aria-hidden="true" focusable="false"><path fill="currentColor" d="M3 17.25V21h3.75L17.8 9.94l-3.75-3.75L3 17.25zM20.7 7.04a1 1 0 0 0 0-1.41l-2.34-2.34a1 1 0 0 0-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/></svg>';
 function lockFields(){
  var grid=document.querySelector('#amBusinessEditor .am-address-v3');if(!grid)return;
  Array.prototype.forEach.call(grid.querySelectorAll('.am-business-field'),function(field){
   var input=field.querySelector('input.am-business-input,select.am-business-input');if(!input||input.dataset.amLock)return;
   input.dataset.amLock='1';
   var wrap=document.createElement('span');wrap.className='am-lock-wrap';input.parentNode.insertBefore(wrap,input);wrap.appendChild(input);
   var b=document.createElement('button');b.type='button';b.className='am-lock-pencil';b.innerHTML=PENCIL;
   var label=field.querySelector('label');b.setAttribute('aria-label','Edit '+(label?label.textContent.trim():'field'));b.title='Edit';wrap.appendChild(b);
   function lock(){field.classList.add('am-locked');if(input.tagName==='SELECT')input.setAttribute('aria-readonly','true');else input.readOnly=true;}
   lock();
   input.addEventListener('keydown',function(e){if(field.classList.contains('am-locked')&&input.tagName==='SELECT'&&e.key!=='Tab')e.preventDefault();});
   b.addEventListener('click',function(){field.classList.remove('am-locked');input.readOnly=false;input.removeAttribute('aria-readonly');b.hidden=true;input.focus();});
  });
 }
 var Address={
  openList:function(){setTimeout(Address.showList,80);},
  showList:function(){
   var s=document.querySelector('#amBusinessEditor .am-business-results select');if(!s||s.disabled||s.options.length<2)return;
   s.focus();
   try{if(typeof s.showPicker==='function'){s.showPicker();return;}}catch(blocked){}
   s.size=Math.min(s.options.length,7);s.classList.add('am-list-open');
   var close=function(){s.removeAttribute('size');s.classList.remove('am-list-open');s.removeEventListener('blur',close);};
   s.addEventListener('blur',close);s.addEventListener('change',close);
  },
  openNew:function(){
   var m=document.getElementById('amNewAddress');if(!m)return;
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(f){f.value=f.getAttribute('data-addr')==='Country'?'Canada':'';f.removeAttribute('aria-invalid');});
   document.getElementById('amNewAddressError').hidden=true;
   m.hidden=false;document.body.classList.add('am-addr-open');Address.back=document.activeElement;
   var first=m.querySelector('[data-addr]');if(first)first.focus();
  },
  close:function(){var m=document.getElementById('amNewAddress');if(!m||m.hidden)return;m.hidden=true;document.body.classList.remove('am-addr-open');if(Address.back&&Address.back.focus)Address.back.focus();},
  apply:function(){
   var m=document.getElementById('amNewAddress'),missing=[];
   Array.prototype.forEach.call(m.querySelectorAll('[data-req]'),function(f){var bad=!f.value.trim();if(bad)f.setAttribute('aria-invalid','true');else f.removeAttribute('aria-invalid');if(bad)missing.push(f.parentNode.firstChild.textContent.replace('(Required)','').trim());});
   var err=document.getElementById('amNewAddressError');
   if(missing.length){err.textContent='Enter '+missing.join(', ')+'.';err.hidden=false;m.querySelector('[aria-invalid]').focus();return;}
   Array.prototype.forEach.call(m.querySelectorAll('[data-addr]'),function(f){
    var t=document.getElementById(f.getAttribute('data-addr'));if(!t)return;
    var v=f.value.trim();if(t.id==='txtPostalCode'||t.id==='txtProvince'||t.id==='txtCity'||t.id==='txtStreetName')v=v.toUpperCase();
    if(t.id==='txtPostalCode')v=v.replace(/\s+/g,'');
    t.value=v;
   });
   Address.close();
   var h=document.getElementById('business-address-heading');if(h){h.setAttribute('tabindex','-1');h.focus();}
  }
 };
 root.AnyminuteBusinessAddress=Address;
 if(typeof document!=='undefined'){
  document.addEventListener('click',function(e){var t=e.target;if(!t.closest)return;if(t.closest('[data-addr-close]')||t.id==='amNewAddress'){e.preventDefault();Address.close();}else if(t.closest('[data-addr-apply]')){e.preventDefault();Address.apply();}});
  document.addEventListener('keydown',function(e){var m=document.getElementById('amNewAddress');if(!m||m.hidden)return;if(e.key==='Escape')Address.close();else if(e.key==='Enter'&&e.target.closest&&e.target.closest('#amNewAddress')&&e.target.tagName!=='BUTTON'){e.preventDefault();Address.apply();}});
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={fileError:fileError};
 if(typeof document!=='undefined'){
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
  if(root.Sys&&root.Sys.Application)root.Sys.Application.add_load(init);
 }
})(typeof window!=='undefined'?window:globalThis);
