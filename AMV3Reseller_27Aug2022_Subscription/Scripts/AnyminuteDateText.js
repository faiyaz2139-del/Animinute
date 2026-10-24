/* v7: Employee editor dates always show yyyy-mm-dd (Deck B slide 9), whatever the browser locale.
   The original <input type="date"> stays in the form and is still what gets posted (always ISO), so saving
   and parsing are unchanged. It is hidden behind a formatted text box and opened by the calendar button. */
(function(){
 var ISO=/^\d{4}-\d{2}-\d{2}$/;
 function valid(v){if(!ISO.test(v))return false;var d=new Date(v+'T00:00:00');return !isNaN(d.getTime())&&d.toISOString().slice(0,10)===v;}
 function wire(d){
  if(d.dataset.amText)return;d.dataset.amText='1';
  var wrap=document.createElement('span');wrap.className='am-date-text';d.parentNode.insertBefore(wrap,d);
  var t=document.createElement('input');t.type='text';t.className='am-date-shown';t.value=d.value;t.placeholder='YYYY-MM-DD';t.maxLength=10;t.inputMode='numeric';t.autocomplete='off';
  t.setAttribute('aria-label',(d.closest('label')&&d.closest('label').querySelector('span')?d.closest('label').querySelector('span').textContent.trim():'Date')+' (yyyy-mm-dd)');
  var locked=d.readOnly||d.disabled;if(locked){t.readOnly=true;wrap.className+=' am-date-locked';}
  if(d.title)t.title=d.title;
  var b=document.createElement('button');b.type='button';b.className='am-date-btn';b.setAttribute('aria-label','Open calendar');b.tabIndex=locked?-1:0;if(locked)b.disabled=true;
  b.innerHTML='<svg viewBox="0 0 24 24" aria-hidden="true"><rect x="3" y="5" width="18" height="16" rx="2"/><path d="M3 10h18M8 3v4M16 3v4"/></svg>';
  wrap.appendChild(t);wrap.appendChild(d);wrap.appendChild(b);
  d.tabIndex=-1;d.setAttribute('aria-hidden','true');d.classList.add('am-date-native');
  b.addEventListener('click',function(){if(locked)return;try{if(d.showPicker){d.showPicker();return;}}catch(e){}d.focus();d.click();});
  d.addEventListener('change',function(){t.value=d.value;t.setCustomValidity('');});
  t.addEventListener('input',function(){
   var v=t.value.replace(/[^\d]/g,'').slice(0,8);var f=v.slice(0,4)+(v.length>4?'-'+v.slice(4,6):'')+(v.length>6?'-'+v.slice(6,8):'');
   if(f!==t.value)t.value=f;
   if(!f){d.value='';t.setCustomValidity('');}else if(valid(f)){d.value=f;t.setCustomValidity('');}else t.setCustomValidity('Enter a valid date as yyyy-mm-dd.');
  });
  t.addEventListener('blur',function(){if(t.value&&!valid(t.value)){t.value=d.value;t.setCustomValidity('');}});
 }
 function run(){Array.prototype.forEach.call(document.querySelectorAll('.am-editor-v3 input[type=date]'),wire);}
 if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',run);else run();
 if(window.Sys&&Sys.WebForms&&Sys.WebForms.PageRequestManager)Sys.WebForms.PageRequestManager.getInstance().add_endRequest(run);
})();
