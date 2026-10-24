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
 }
 if(typeof module!=='undefined'&&module.exports)module.exports={fileError:fileError};
 if(typeof document!=='undefined'){
  if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
  if(root.Sys&&root.Sys.Application)root.Sys.Application.add_load(init);
 }
})(typeof window!=='undefined'?window:globalThis);
