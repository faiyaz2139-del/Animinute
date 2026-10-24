(function (window, document) {
    'use strict';
    if (window.AnyminuteUI2026) return;
    var page = location.pathname.split('/').pop().toLowerCase();
    var businessIds = ['GvEmpListForBusOwner','GvEmpListOwner','gvbusinessListForManager','gvBusinessListEmp','GridView_Accountant_User','GridView_Custome_User_Role'];
    var pages = {
        'home_page.aspx':['Businesses','Manage businesses, projects, users and timecards.'],
        'add_business.aspx':['Add Business','Business details, logo, contacts and address.'],
        'add_employeev2.aspx':['Add User','Personal details, pay and project assignments.'],
        'addeditrole_add_employee.aspx':['Edit User','Update personal details, pay, role history and project assignments.'],
        'add_employeev2_accountant.aspx':['Add Accountant','Accountant details and business assignments.'],
        'home_page_emppopuplist.aspx':['Users','Manage employees, roles, rates and status.'],
        'add_project.aspx':['Projects','Project details, business and dates.'],
        'view_project.aspx':['Projects','View and manage project assignments.'],
        'gridviewnewdesign.aspx':['Assign User to Manager','Choose users and their authorized approving manager.'],
        'gridviewnewdesign_assignuser.aspx':['Assign Users','Manage users assigned to this business.'],
        'gridviewassignaccountant.aspx':['Assign Accountant','Manage this business\'s accountant assignments.'],
        'time_sheet_entry_all.aspx':['Time Card','Enter and review weekly time, breaks and attendance.'],
        'time_sheet_entry.aspx':['Time Card','Enter and review time, breaks and attendance.'],
        'company_report.aspx':['Company Reports','Review company hours and export reports.'],
        'employee_report.aspx':['Employee Reports','Review employee hours and export reports.'],
        'report3.aspx':['Employee Reports','Review employee hours and export reports.'],
        'project_report.aspx':['Project Reports','Review project time and export reports.'],
        'myprofile.aspx':['My Profile','Your saved account and contact details.']
    };
    var paths = {
        menu:'M4 6h16M4 12h16M4 18h16', business:'M6 21V3h12v18M3 21h18M9 7h1m4 0h1M9 11h1m4 0h1M10 21v-5h4v5',
        people:'M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2M16 3a4 4 0 0 1 0 8M22 21v-2a4 4 0 0 0-3-3.8M13 7a4 4 0 1 1-8 0 4 4 0 0 1 8 0',
        clock:'M12 8v4l3 2M22 12a10 10 0 1 1-20 0 10 10 0 0 1 20 0', folder:'M3 7V4h6l2 3h10v13H3z', plus:'M12 5v14M5 12h14'
    };
    function icon(key) { return '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="'+paths[key]+'"/></svg>'; }
    function visible(el) { return !!el && el.getClientRects().length > 0; }
    function element(tag, cls, text) { var el=document.createElement(tag);if(cls)el.className=cls;if(text!==undefined)el.textContent=text;return el; }
    function suffix(id, scope) { return (scope||document).querySelector('[id="'+id+'"],[id$="_'+id+'"]'); }
    function button(text, action, cls) { var b=element('button','am-action '+(cls||''),text);b.type='button';b.addEventListener('click',action);return b; }
    function allRows(table) {
        var $=window.jQuery;
        if($ && $.fn && $.fn.dataTable && $.fn.dataTable.isDataTable(table)) return $(table).DataTable().rows().nodes().toArray();
        return Array.prototype.filter.call(table.rows,function(row){return row.querySelector('td') && !row.querySelector('td[colspan]') && !row.classList.contains('GridPager');});
    }
    function heading(main) {
        if(!pages[page] || document.getElementById('am-heading'))return;
        var info=pages[page],title=info[0];
        if(page==='add_business.aspx' && new URLSearchParams(location.search).has('BusinessIDEdit'))title='Update Profile';
        var row=element('section','am-heading');row.id='am-heading';
        var text=element('div');text.appendChild(element('h1','',title));text.appendChild(element('p','',info[1]));row.appendChild(text);
        var actions=element('div','am-heading-actions');row.appendChild(actions);
        if(page==='home_page.aspx') {
            [[['add_business.aspx'],'Add Business',''],[['employeeeditor.aspx','add_employeev2.aspx'],'Add Employee','EmployeeEditor.aspx']].forEach(function(item){var link=Array.prototype.find.call(document.querySelectorAll('#sidebar-wrapper a, .sidebar-nav a'),function(a){return item[0].indexOf((a.getAttribute('href')||'').split('#')[0].split('?')[0].split('/').pop().toLowerCase())>=0;});if(link){var quick=element('a','am-action',item[1]);quick.href=item[2]||link.href;actions.appendChild(quick);}});
        }
        if(page!=='home_page.aspx') { var back=element('a','am-action am-secondary','Back to List');back.href='Home_Page.aspx';actions.appendChild(back); }
        var saveIds={ 'add_business.aspx':['btnUpdateBusinessDetails','btnSaveBusinessDetails'], 'add_project.aspx':['Button_Update_Project','btnSaveProjectDetails'], 'gridviewnewdesign.aspx':['btnSave'], 'gridviewassignaccountant.aspx':['btnSave'] };
        if(saveIds[page]) actions.appendChild(button('Save changes',function(){var real=saveIds[page].map(function(id){return suffix(id);}).find(function(el){return visible(el)&&!el.disabled;});if(real)real.click();},''));
        if(['add_employeev2.aspx','addeditrole_add_employee.aspx','add_employeev2_accountant.aspx'].indexOf(page)>=0) actions.appendChild(button('Review & Save',function(){var save=suffix('btnFinalSave');if(!save)return;var pane=save.closest('.tab-pane');if(pane&&!pane.classList.contains('active')){var tab=document.querySelector('a[data-toggle="tab"][href="#'+pane.id+'"]');if(tab)tab.click();}save.scrollIntoView({block:'center',behavior:'smooth'});save.focus();},''));
        main.insertBefore(row,main.firstChild);
        // Hide only redundant presentation titles; server controls and validation remain in the DOM.
        main.querySelectorAll('h2').forEach(function(h){if(h.closest('[class*=modalPopup]'))return;var t=h.textContent.trim().toLowerCase();if([title.toLowerCase(),'time sheet entry','add user','edit user','add business'].indexOf(t)>=0)h.style.display='none';});
        main.querySelectorAll('span').forEach(function(span){if(span.children.length||span.closest('[class*=modalPopup]'))return;var text=span.textContent.trim().toLowerCase();if([title.toLowerCase(),'add user','edit user','time card'].indexOf(text)<0)return;var row=span.closest('.row');if(row&&row.querySelector('a[href*=Home_Page]')&&!row.querySelector('input,select,textarea'))row.style.display='none';});
    }
    function navigation() {
        var toggle=document.getElementById('menu-toggle');
        if(toggle&&!toggle.dataset.amBound){toggle.dataset.amBound='1';toggle.innerHTML=icon('menu');toggle.addEventListener('click',function(e){e.preventDefault();var mobile=window.matchMedia('(max-width:720px)').matches;document.body.classList.toggle(mobile?'am-nav-open':'am-nav-collapsed');toggle.setAttribute('aria-expanded',mobile?String(document.body.classList.contains('am-nav-open')):String(!document.body.classList.contains('am-nav-collapsed')));});
            var shade=button('Close navigation',function(){document.body.classList.remove('am-nav-open');toggle.setAttribute('aria-expanded','false');toggle.focus();},'');shade.className='am-sidebar-shade';shade.setAttribute('aria-label','Close navigation');document.body.appendChild(shade);
            document.addEventListener('keydown',function(e){if(e.key==='Escape'){document.body.classList.remove('am-nav-open');toggle.setAttribute('aria-expanded','false');}});
        }
        document.querySelectorAll('#sidebar-wrapper a').forEach(function(a){
            var url=a.getAttribute('href')||'';
            if(url.split('?')[0].split('/').pop().toLowerCase()===page){a.setAttribute('aria-current','page');var parent=a.closest('.am-nav-parent');if(parent){parent.classList.add('am-expanded');parent.querySelector('a').setAttribute('aria-expanded','true');}}
            if(a.parentElement.classList.contains('am-nav-parent')&&a.dataset.amPrimary!=='true'&&!a.dataset.amBound){a.dataset.amBound='1';a.addEventListener('click',function(e){e.preventDefault();var open=a.parentElement.classList.toggle('am-expanded');a.setAttribute('aria-expanded',String(open));});}
        });
    }
    function tables(main) {
        main.querySelectorAll('table').forEach(function(table){
            if(!(table.classList.contains('table')||table.id.indexOf('Gv')>=0||table.id.indexOf('GVChildlist')>=0||table.id.indexOf('GridView')>=0||table.id.indexOf('gv')===0))return;
            if(table.closest('.nav-tabs') || table.querySelector('table'))return;
            table.classList.add('am-grid');if(businessIds.indexOf(table.id)>=0)table.classList.add('am-business-grid');
            if(!table.parentElement.classList.contains('am-grid-wrap')){var wrap=element('div','am-grid-wrap');table.parentNode.insertBefore(wrap,table);wrap.appendChild(table);}
            table.querySelectorAll('a[href*="Time_Sheet_Entry"]').forEach(function(a){a.classList.add('am-open-card');});
            table.querySelectorAll('[data-toggle="dropdown"]').forEach(function(a){if(a.dataset.amMenu)return;a.dataset.amMenu='1';a.classList.add('am-menu-button');a.textContent='\u22ee';a.setAttribute('aria-label','More row actions');a.title='More actions';});
        });
        main.querySelectorAll('span[id*=Status],span[id*=status]').forEach(function(span){var s=span.textContent.trim().toLowerCase();var status={active:'active',approved:'approved','not active':'inactive',inactive:'inactive',terminated:'inactive',rejected:'rejected',reject:'rejected',pending:'pending',submitted:'pending'}[s];if(status){span.classList.add('am-status','am-status-'+status);}});
        main.querySelectorAll('select').forEach(function(select){if(select.id.indexOf('ddlAppRej')<0)return;select.dataset.amStatus=select.value.toLowerCase();});
    }
    function filters(table) {
        if(page==='users.aspx'||page==='timecards.aspx'||table.dataset.amFilter || !visible(table))return;
        var $=window.jQuery;
        // Preserve DataTables' existing full-row search/paging instead of filtering only the current page.
        if($&&$.fn&&$.fn.dataTable&&(businessIds.indexOf(table.id)>=0||['GvPopUpEmpList','grdviewProject'].indexOf(table.id)>=0||$.fn.dataTable.isDataTable(table)))return;
        table.dataset.amFilter='1';var wrap=table.parentElement,bar=element('div','am-filter');
        var label=element('label','',businessIds.indexOf(table.id)>=0?'Search businesses':'Search this list');var search=element('input');search.type='search';search.placeholder='Search by name or displayed details';label.appendChild(search);bar.appendChild(label);
        var statusIndex=-1,header=table.querySelector('tr');if(header)Array.prototype.forEach.call(header.cells,function(cell,i){if(/^status$/i.test(cell.textContent.trim()))statusIndex=i;});
        var status;if(statusIndex>=0){var l=element('label','','Status');status=element('select');status.appendChild(new Option('All statuses',''));var seen={};allRows(table).forEach(function(row){var value=row.cells[statusIndex].textContent.trim();if(value&&!seen[value]){seen[value]=true;status.appendChild(new Option(value,value));}});l.appendChild(status);bar.appendChild(l);}
        var reset=button('Clear',function(){search.value='';if(status)status.value='';apply();},'am-secondary');bar.appendChild(reset);wrap.parentNode.insertBefore(bar,wrap);
        var result=element('p','am-filter-result');result.setAttribute('aria-live','polite');wrap.parentNode.insertBefore(result,wrap);
        function apply(){var rows=allRows(table),count=0,q=search.value.trim().toLowerCase();rows.forEach(function(row){var matches=row.textContent.toLowerCase().indexOf(q)>=0&&(!status||!status.value||row.cells[statusIndex].textContent.trim()===status.value);row.hidden=!matches;if(matches)count++;});result.textContent=count+' of '+rows.length+' rows shown';}
        search.addEventListener('input',apply);if(status)status.addEventListener('change',apply);apply();
    }
    function businessMenus(main) {
        businessIds.forEach(function(id){var table=document.getElementById(id);if(!table||table.dataset.amMenus)return;table.dataset.amMenus='1';
            var rows=allRows(table);if(!rows.length)return;var header=table.querySelector('tr');if(header&&header.querySelector('th'))header.appendChild(element('th','','More'));
            rows.forEach(function(row){var targets=Array.prototype.filter.call(row.querySelectorAll('a'),function(a){return a.getAttribute('href')&&!a.hasAttribute('disabled')&&a.getAttribute('aria-disabled')!=='true';});
                var cell=element('td','am-business-menu'),details=element('details'),summary=element('summary','am-menu-button','\u22ee');summary.setAttribute('aria-label','More business actions');details.appendChild(summary);var menu=element('div','am-row-menu');
                targets.forEach(function(a){var text=a.textContent.replace(/^\s*\|\s*/,'').trim();if(!text)return;menu.appendChild(button(text,function(){details.open=false;a.click();},'am-secondary'));});
                details.appendChild(menu);cell.appendChild(details);row.appendChild(cell);
                details.addEventListener('toggle',function(){if(!details.open)return;main.querySelectorAll('.am-business-menu details[open]').forEach(function(other){if(other!==details)other.open=false;});var rect=summary.getBoundingClientRect();menu.style.top=Math.min(rect.bottom+5,Math.max(80,window.innerHeight-310))+'px';menu.style.left=Math.max(8,Math.min(rect.right-220,window.innerWidth-230))+'px';});
                if(!targets.length){summary.setAttribute('aria-disabled','true');details.hidden=true;}
            });
        });
    }
    function stats(main) {
        if(page!=='home_page.aspx'||document.getElementById('amServerOverview'))return;
        var old=document.getElementById('am-dashboard-stats');if(old)old.remove();
        var lists=businessIds.map(function(id){return document.getElementById(id);}).filter(visible);if(!lists.length)return;
        var businesses={},users=0,known=false;
        lists.forEach(function(table){allRows(table).forEach(function(row){var field=row.querySelector('input[id*=hdfBusinessID]'),name=row.querySelector('[id*=CompanyName]');var key=field?field.value:name?name.textContent.trim():null;if(key&&!businesses[key]){businesses[key]=true;var cnt=row.querySelector('[id$=_CNT],[id=CNT],[id$=_CNT1],[id=CNT1]');if(cnt){var m=cnt.textContent.match(/\d+/);if(m){users+=Number(m[0]);known=true;}}}});});
        var grid=element('section','am-stats');grid.id='am-dashboard-stats';grid.setAttribute('aria-label','Business overview');
        [['Businesses in your lists',Object.keys(businesses).length,'business'],['User assignments',known?users:'\u2014','people'],['Time Cards','Choose business','clock'],['Projects','Choose business','folder']].forEach(function(info,index){var card=element('div','am-stat'),symbol=element('span','am-stat-icon');symbol.innerHTML=icon(info[2]);card.appendChild(symbol);var text=element('div');text.appendChild(element('small','',info[0]));if(index<2)text.appendChild(element('strong','',String(info[1])));else {var b=button(info[1],function(){var target=lists[0].querySelector(index===2?'a[href*=Time_Sheet_Entry]':'a[id*=ViewProj]');if(target){target.scrollIntoView({block:'center',behavior:'smooth'});target.focus();}},'');b.className='am-stat-link';text.appendChild(b);}text.appendChild(element('small','',index===1?'Across displayed businesses':'Within your access'));card.appendChild(text);grid.appendChild(card);});
        var head=document.getElementById('am-heading');head.parentNode.insertBefore(grid,head.nextSibling);
    }
    function forms(main) {
        if(['add_business.aspx','add_employeev2.aspx','addeditrole_add_employee.aspx','add_employeev2_accountant.aspx'].indexOf(page)<0)return;
        document.body.classList.add('am-form-screen');var upload=main.querySelector('#uploadArea');
        if(upload){var col=upload.closest('[class*="col-md-"]'),row=col&&col.parentElement;if(row&&!row.classList.contains('am-details-row')){row.classList.add('am-details-row');col.classList.add('am-photo-column');var file=upload.querySelector('input[type=file]');if(file){var change=button('Change Image',function(){file.click();},'am-secondary');change.className='am-photo-action';col.appendChild(change);}}}
        // Project business context reflects the actual selector; it does not post an additional value.
        main.querySelectorAll('.am-project-context').forEach(function(context){var b=suffix('ddlBusiness')||suffix('Ddl_Business')||suffix('ddlbusinness');if(!b){var selects=main.querySelectorAll('select');b=Array.prototype.find.call(selects,function(s){return /business/i.test(s.id)&&s.options.length;});}if(b&&b.selectedIndex>=0)context.textContent='Business: '+b.options[b.selectedIndex].text;else context.textContent='Project will use the business selected in the form.';});
    }
    function dialogs(main) {
        main.querySelectorAll('[class*=modalPopup]').forEach(function(panel){if(panel.getAttribute('role')!=='dialog'){panel.setAttribute('role','dialog');panel.setAttribute('aria-modal','true');var title=panel.querySelector('.header');if(title){if(!title.id)title.id=panel.id+'-am-title';panel.setAttribute('aria-labelledby',title.id);}}});
    }
    function init() {
        var main=document.getElementById('amPage')||document.querySelector('main.am-main');if(!main)return;
        document.body.classList.add('am-ui');navigation();heading(main);tables(main);businessMenus(main);forms(main);dialogs(main);
        main.querySelectorAll('table.am-grid').forEach(function(table){if(table.id.indexOf('GVChildlist')<0)filters(table);});stats(main);
    }
    window.AnyminuteUI2026={refresh:init};
    document.addEventListener('click',function(e){if(e.target.closest('.am-business-menu'))return;document.querySelectorAll('.am-business-menu details[open]').forEach(function(d){d.open=false;});});
    document.addEventListener('keydown',function(e){if(e.key==='Escape')document.querySelectorAll('.am-business-menu details[open]').forEach(function(d){d.open=false;d.querySelector('summary').focus();});});
    window.addEventListener('resize',function(){document.querySelectorAll('.am-business-menu details[open]').forEach(function(d){d.open=false;});});
    if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',init);else init();
    window.addEventListener('load',function(){init();if(window.Sys&&Sys.Application)Sys.Application.add_load(init);});
}(window,document));
