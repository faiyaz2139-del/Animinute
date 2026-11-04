<%@ Page Title="Accountant List" Language="C#" MasterPageFile="~/Modern.master" AutoEventWireup="true"
    CodeFile="SuperAdmin.aspx.cs" Inherits="SuperAdmin" %>
<%-- v36 (DOCX image7, strict): Accountant List moved to the same Modern shell / Support Admin card design as the other Support Admin
     pages: one toolbar (Search + Status + Clear), status pills, sortable headers, Previous / page / Next pager, no forced H-scroll. --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <link href="css/AnyminuteSupport.css?v=20261103v59" rel="stylesheet" />
    <style>
        .am-sa-list .sa-head h1{font-size:24px;margin:0}
        .am-sa-list .sa-table-wrap{overflow-x:visible}
        .am-sa-list table.am-sa-grid{width:100%;table-layout:auto;margin:0}
        .am-sa-list table.am-sa-grid td,.am-sa-list table.am-sa-grid th{white-space:normal;word-break:break-word;vertical-align:middle}
        .am-sa-list table.am-sa-grid th{cursor:pointer;user-select:none}
        .am-sa-list table.am-sa-grid th[data-sort]::after{content:" \2195";opacity:.35;font-size:11px}
        .am-sa-list table.am-sa-grid th[data-sort=asc]::after{content:" \2191";opacity:.9}
        .am-sa-list table.am-sa-grid th[data-sort=desc]::after{content:" \2193";opacity:.9}
        .am-sa-list table.am-sa-grid td a{font-size:12px}
        .am-sa-list .am-sa-status{text-decoration:none!important;cursor:pointer}
        .am-sa-list .sa-pager a.off{opacity:.45;pointer-events:none}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="sa-page am-sa-list">
    <div class="sa-head" id="divAddAcc" runat="server">
        <div><h1>Accountant List</h1><p>Accountants and payroll agencies, their businesses and login status.</p></div>
        <asp:HyperLink ID="hyperAdminAddEmp" runat="server" Text="+ Add Accountant" NavigateUrl="~/MotherOrg.aspx" CssClass="sa-btn"></asp:HyperLink>
    </div>
    <div class="sa-card">
        <div class="sa-filters" id="amAccFilters">
            <label class="sa-grow"><span>Search this list</span><input type="search" id="amAccQ" placeholder="Search by name or displayed details" /></label>
            <label><span>Status</span><select id="amAccSt"><option value="">All statuses</option><option>Active</option><option>Inactive</option></select></label>
            <button type="button" class="sa-btn sa-btn-outline" id="amAccClear">Clear</button>
        </div>
        <div id="divAddgrid" runat="server">
                    <div class="sa-table-wrap">
                        <asp:GridView ID="GVMother" ClientIDMode="Static" runat="server" CssClass="sa-table am-sa-grid"
                            CellPadding="1" GridLines="None"
                             OnPageIndexChanging="GVMother_PageIndexChanging" AutoGenerateColumns="False"
                            EmptyDataText="No records Found" OnPreRender="GVMother_PreRender" OnRowDataBound="GVMother_RowDataBound" OnRowCommand="GVMother_RowCommand">
                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Accountant Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDomainName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MotherOrgName")%>'></asp:Label>
                                        <asp:HiddenField ID="hfdid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MotherOrgID")%>' />
                                        <br />
                                        <asp:HyperLink ID="btnedit" runat="server" NavigateUrl='<%# "~/MotherOrg.aspx?MotherOrgId=" + Eval("MotherOrgID")%>'
                                            Text="Update" data-original-title="Edit" data-toggle="tooltip">                                                                                
                                        </asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Business Owner">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBusinessOwner" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "bussOwnerCnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No. of Business">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBusinesscnt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "busscnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No. of Employee">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeescnt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "empcnt")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                               
                                <asp:TemplateField HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="lblContactName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ContactNumber")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Email ID">
                                    <ItemTemplate>
                                        <asp:Label ID="LblEmail" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Enrolled Date">
                                    <ItemTemplate>
                                        <asp:Label ID="LblEnrlDt" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EnrollDate1")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <%-- v32: Password column removed; passwords are never shown in plain text in list UIs. --%>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <%-- v36 (DOCX image7): status pill instead of the active.png image toggle; same "status" command. --%>
                                        <asp:LinkButton ID="btnactive" runat="server" CssClass="sa-pill am-sa-status" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="status" ToolTip="Click to change status" />
                                        <asp:HiddenField ID="hfdStatusForAdminBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
        </div>
        <div class="sa-pager" id="amAccPager"><span id="amAccInfo"></span><nav aria-label="Pages"></nav></div>
    </div>
    </div>
    <script>
    (function(){
     var t=document.getElementById('GVMother');if(!t||!t.tBodies.length)return;
     var body=t.tBodies[0],rows=Array.prototype.slice.call(body.rows).filter(function(r){return r.cells.length>1;}),size=10,page=1,shown=rows;
     var q=document.getElementById('amAccQ'),st=document.getElementById('amAccSt'),info=document.getElementById('amAccInfo'),nav=document.querySelector('#amAccPager nav');
     var heads=t.tHead?t.tHead.rows[0].cells:[],statusCol=heads.length-1;
     function txt(r,i){return (r.cells[i]?r.cells[i].textContent:'').replace(/\s+/g,' ').trim();}
     function link(label,p,on,off){var a=document.createElement('a');a.href='#';a.textContent=label;if(on)a.className='on';if(off)a.className='off';a.addEventListener('click',function(e){e.preventDefault();page=p;draw();});return a;}
     function draw(){var pages=Math.max(1,Math.ceil(shown.length/size));if(page>pages)page=pages;
      rows.forEach(function(r){r.style.display='none';});shown.forEach(function(r,i){r.style.display=(i>=(page-1)*size&&i<page*size)?'':'none';});
      var from=shown.length?(page-1)*size+1:0;info.textContent='Showing '+from+' to '+Math.min(shown.length,page*size)+' of '+shown.length+' accountant'+(shown.length==1?'':'s');
      nav.innerHTML='';nav.appendChild(link('Previous',page-1,false,page<=1));for(var p=1;p<=pages;p++)nav.appendChild(link(String(p),p,p===page,false));nav.appendChild(link('Next',page+1,false,page>=pages));}
     function apply(){var s=q.value.trim().toLowerCase(),v=st.value;shown=rows.filter(function(r){return (!s||r.textContent.toLowerCase().indexOf(s)>=0)&&(!v||txt(r,statusCol)===v);});page=1;draw();}
     Array.prototype.forEach.call(heads,function(th,i){th.setAttribute('data-sort','');th.title='Sort';th.addEventListener('click',function(){var dir=th.getAttribute('data-sort')==='asc'?'desc':'asc';Array.prototype.forEach.call(heads,function(h){h.setAttribute('data-sort','');});th.setAttribute('data-sort',dir);
      rows.sort(function(a,b){var x=txt(a,i),y=txt(b,i),nx=parseFloat(x),ny=parseFloat(y),c=(!isNaN(nx)&&!isNaN(ny)&&/^[\d.\-]+$/.test(x)&&/^[\d.\-]+$/.test(y))?nx-ny:x.localeCompare(y,undefined,{sensitivity:'base'});return dir==='asc'?c:-c;});
      rows.forEach(function(r){body.appendChild(r);});apply();});});
     q.addEventListener('input',apply);st.addEventListener('change',apply);document.getElementById('amAccClear').addEventListener('click',function(){q.value='';st.value='';apply();});
     apply();
    })();
    </script>
</asp:Content>
