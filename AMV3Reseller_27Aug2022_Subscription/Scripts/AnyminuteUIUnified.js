(function (window, document) {
    'use strict';
    var page=location.pathname.split('/').pop().toLowerCase();
    var paths={business:'M6 21V3h12v18M3 21h18M9 7h1m4 0h1M9 11h1m4 0h1M10 21v-5h4v5',people:'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2M16 3a4 4 0 0 1 0 8M22 21v-2a4 4 0 0 0-3-3.8M13 7a4 4 0 1 1-8 0 4 4 0 0 1 8 0',clock:'M12 8v4l3 2M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0',folder:'M3 7V4h6l2 3h10v13H3z',dashboard:'M3 3h7v7H3zM14 3h7v7h-7zM3 14h7v7H3zM14 14h7v7h-7z',reports:'M5 20V10M12 20V4M19 20v-7',settings:'M12 3v3M12 18v3M3 12h3M18 12h3M5.6 5.6l2.1 2.1M16.3 16.3l2.1 2.1M5.6 18.4l2.1-2.1M16.3 7.7l2.1-2.1M16 12a4 4 0 1 1-8 0 4 4 0 0 1 8 0',check:'M4 12l5 5L20 6',help:'M9 9a3 3 0 1 1 5 2c-1 1-2 1-2 3M12 17h.01M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0',profile:'M20 21v-2a7 7 0 0 0-14 0v2M17 7a4 4 0 1 1-8 0 4 4 0 0 1 8 0',signout:'M9 4H4v16h5M10 12h12M17 7l5 5-5 5',ticket:'M3 5h18v5a2 2 0 0 0 0 4v5H3v-5a2 2 0 0 0 0-4zM15 5v3m0 3v2m0 3v3',upgrade:'M12 20V4M5 11l7-7 7 7',message:'M21 3H3v14h5v4l5-4h8z'};
    function icon(key){return '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="'+(paths[key]||paths.folder)+'"/></svg>';}
    function byId(id){return document.getElementById(id)||document.querySelector('[id$="_'+id+'"]');}
    function heading(parent,text){if(!parent||parent.querySelector(':scope > .am-section-title'))return;var h=document.createElement('h2');h.className='am-section-title';h.textContent=text;parent.insertBefore(h,parent.firstChild);}
    function nav(){
        var permission=byId('UiDirectoryAccess');
        document.querySelectorAll('.am-sidebar nav a,#sidebar-wrapper .sidebar-nav a').forEach(function(a){
            var span=a.querySelector('span'),label=(span?span.textContent:a.textContent).trim().toLowerCase();
            if(permission&&permission.value==='false'&&a.closest('.am-sidebar')&&/^(employees|users|user list|employee list)$/.test(label)){a.hidden=true;return;}
            var key=/ticket/.test(label)?'ticket':/plan|upgrade/.test(label)?'upgrade':/accountant/.test(label)?'people':/more tools/.test(label)?'settings':/dashboard|home/.test(label)?'dashboard':/employee|user/.test(label)?'people':/business/.test(label)?'business':/project/.test(label)?'folder':/report/.test(label)?'reports':/approv/.test(label)?'check':/setting/.test(label)?'settings':'clock';
            var target=a.querySelector('i');
            if(target){target.innerHTML=icon(key);target.classList.add('am-svg-icon');}
            else if(span&&!a.querySelector('.am-svg-icon')){
                // Replace only the old decorative icon text, retaining labels and counters.
                Array.prototype.slice.call(a.childNodes).forEach(function(n){if(n.nodeType===3&&n===a.firstChild)a.removeChild(n);});
                var el=document.createElement('span');el.className='am-svg-icon';el.innerHTML=icon(key);a.insertBefore(el,a.firstChild);
            }
        });
        document.querySelectorAll('.am-nav-expand').forEach(function(button){if(button.dataset.amBound)return;button.dataset.amBound='1';button.addEventListener('click',function(){var open=button.parentElement.classList.toggle('am-expanded');button.setAttribute('aria-expanded',String(open));});});
        document.querySelectorAll('[data-am-icon]').forEach(function(el){el.innerHTML=icon(el.dataset.amIcon);});
    }
    function isDeclaredHidden(field,root){
        for(var parent=field;parent&&parent!==root;parent=parent.parentElement){
            if(parent.hidden||parent.style.display==='none')return true;
        }
        return false;
    }
    function formLayout(main){
        var users=/^(add_employeev2|addeditrole_add_employee|add_employeev2_accountant)\.aspx$/.test(page);
        var business=page==='add_business.aspx',project=page==='add_project.aspx';
        if(!users&&!business&&!project)return;
        document.body.classList.add('am-form-screen',users?'am-screen-user':business?'am-screen-business':'am-screen-project');
        if(business&&main.querySelector('.am-business-editor'))return;
        var photo=main.querySelector('#uploadArea'),photoCol=photo&&photo.closest('[class*="col-md-"]');
        if(photoCol){heading(photoCol,business?'Business Logo':'Profile Photo');var row=photoCol.parentElement;row.classList.add('am-details-row');photoCol.classList.add('am-photo-column');Array.prototype.forEach.call(row.children,function(col){if(col!==photoCol&&col.querySelector('input,select')){col.classList.add('am-form-main');heading(col,business?'Business Details':'Personal Details');}});}
        // Change layout in place. Do not move fields across UpdatePanel boundaries,
        // clone inputs or replace nodes that have existing picker/postback bindings.
        main.querySelectorAll('.form-group').forEach(function(field){
            if(field.closest('table,[class*=modalPopup],.am-modal'))return;
            var hidden=isDeclaredHidden(field,main);
            var row=field.closest('.row');
            if(hidden){field.classList.remove('am-form-field');if(row&&row.style.display==='none')row.classList.remove('am-field-row');return;}
            field.classList.add('am-form-field');
            if(row&&!row.classList.contains('am-details-row')&&!row.querySelector('table')&&row.querySelectorAll('.form-group').length<=4)row.classList.add('am-field-row');
        });
        if(project){var panel=byId('add_project');if(panel){panel.classList.add('am-project-editor');heading(panel,'Project Details');}}
        if(users){var anchor=main.querySelector('a[data-toggle="tab"][href="#ratedetils"]');var home=main.querySelector('a[data-toggle="tab"][href="#home"]');var projectTab=main.querySelector('a[data-toggle="tab"][href="#project"]');if(anchor&&home&&projectTab){var ul=home.closest('ul');if(ul&&anchor.closest('li').parentElement===ul&&projectTab.closest('li').parentElement===ul){ul.insertBefore(anchor.closest('li'),projectTab.closest('li'));anchor.textContent='Pay Detail';home.textContent='Personal Details';projectTab.textContent='Assign Project';}}}
    }
    function timeLayout(main){
        if(!/^(time_sheet_entry(_all)?|weektimecard)\.aspx$/.test(page))return;
        document.body.classList.add('am-screen-timecard');
        var employee=byId('ddlemp'),date=byId('txtmonth');
        if(employee&&date){var row=employee.closest('.row');if(row&&row.contains(date)){row.classList.add('am-time-toolbar');var ec=employee.closest('[class*="col-md-"]'),dc=date.closest('[class*="col-md-"]');if(ec)ec.classList.add('am-time-employee');if(dc)dc.classList.add('am-time-date');}}
        main.querySelectorAll('table').forEach(function(table){if(/GVChildlist$/.test(table.id))table.classList.add('am-week-grid');});
    }
    function init(){
        var main=document.getElementById('amPage')||document.querySelector('main.am-main');if(!main)return;
        document.body.classList.add('am-ui','am-unified');document.body.dataset.amUiBuild='20261017';
        nav();formLayout(main);timeLayout(main);
        if(/report/.test(page))document.body.classList.add('am-screen-reports');
        if(page==='dashboard.aspx'||page==='home_page.aspx')document.body.classList.add('am-screen-dashboard');
    }
    if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
    window.addEventListener('load',function(){init();if(window.Sys&&Sys.Application)Sys.Application.add_load(init);});
    window.AnyminuteUIUnified={refresh:init};
}(window,document));
