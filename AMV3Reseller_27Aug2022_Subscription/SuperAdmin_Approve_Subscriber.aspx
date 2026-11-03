<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="SuperAdmin_Approve_Subscriber.aspx.cs" Inherits="SuperAdmin_Approve_Subscriber" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script>
        $(document).ready(function () {
            // v37 (DOCX image4, strict): the subscriber grid no longer gets a second DataTables search box, double sort arrows
            // and a horizontal scrollbar. The page toolbar (Search this list + Status + Clear) and the sort/pager below remain.
            $('#gvEmplistPop').DataTable({

            });
            $('#GridView_PopUp_Business_List').DataTable({

            });
        });
    </script>
    <style>
        .GridPager a, .GridPager span {
            display: block;
            height: 35px;
            width: 35px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            padding: 10px;
        }

        .GridPager a {
            background-color: #fff;
            color: #969696;
            border: 1px solid #dedede;
        }

        .GridPager a:hover {
            color: #337ab7;
        }

        .GridPager span {
            background-color: #337ab7;
            color: #fff;
            border: 1px solid #dedede;
        }

        .buttonhyperlink {
            /*text-align: center;
            padding-top: 10px;
            font-family: sans-serif;
            font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 30px;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 30px;
            color: white;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>

     <style>

        .modalBackground {
            background-color: Black;
            opacity: 0.6;
        }

        .modalPopup {
            background-color: white;
            width: auto;
            border: 2px solid #0275d8;
            padding: 0;
            border-radius: 5px;
        }

        .modalPopup .header {
            background-color: #607DF2;
            height: 50px;
            color: White;
            line-height: 50px;
            text-align: center;
            font-weight: bold;
            border-radius: 0;
            margin-bottom: 10px;
        }

        .modalPopup .body {
            min-height: 50px;
            line-height: 30px;
            margin-bottom: 5px;
            color: black !important;
            padding: 10px;
            overflow-y: scroll;
            height: 350px;
            width: auto;
        }

        .buttonhyperlink {
            border: 1px solid #607DF2;
            display: block;
            background-color:#607DF2 ;
            border-radius: 30px;
            color: white ;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonhyperlink:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color:#607DF2 ;
            border-radius: 30px;
            color: white ;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>

    <script type = "text/javascript">
        function ConfirmStatusChangeBox() {
            var Toggle_activate_deactivate = document.createElement("INPUT");
            Toggle_activate_deactivate.type = "hidden";
            Toggle_activate_deactivate.name = "Toggle_activate_deactivate";
            if (confirm("Would you like to activate/deactivate this user?")) {
                Toggle_activate_deactivate.value = "Yes";
            } else {
                Toggle_activate_deactivate.value = "No";
            }
            document.forms[0].appendChild(Toggle_activate_deactivate);
        }
    </script>

    <link href="css/AnyminuteSupport.css?v=20261103v56" rel="stylesheet" />
    <style>
        /* v37 (DOCX image4): one toolbar, no forced scrollbar, status pill, sortable headers, pager. */
        .am-sa-subwrap{overflow-x:auto;border:1px solid #e2e8f0;border-radius:10px;background:#fff}
        #GridView_New_Registered_BO{width:100%;margin:0;border-collapse:collapse;font-size:13px}
        #GridView_New_Registered_BO th{background:#f7f9fc;color:#475569;font-size:12px;font-weight:650;padding:12px 10px;white-space:nowrap;cursor:pointer;user-select:none;border:0}
        #GridView_New_Registered_BO th::after{content:"";display:inline-block;margin-left:6px;border:4px solid transparent;border-top-color:#94a3b8;transform:translateY(2px);opacity:.6}
        #GridView_New_Registered_BO th[aria-sort=ascending]::after{border-top-color:transparent;border-bottom-color:#1f6fe5;transform:translateY(-2px);opacity:1}
        #GridView_New_Registered_BO th[aria-sort=descending]::after{border-top-color:#1f6fe5;opacity:1}
        #GridView_New_Registered_BO th.am-nosort{cursor:default}#GridView_New_Registered_BO th.am-nosort::after{display:none}
        #GridView_New_Registered_BO td{padding:12px 10px;border-top:1px solid #e2e8f0;vertical-align:middle;background:#fff}
        .am-sa-pill{display:inline-flex;align-items:center;gap:6px;padding:4px 12px;border-radius:999px;font-size:12px;font-weight:600;text-decoration:none!important;white-space:nowrap}
        .am-sa-pill::before{content:"";width:7px;height:7px;border-radius:50%;background:currentColor}
        .am-sa-pill.is-active{background:#e4f8eb;color:#168347!important}.am-sa-pill.is-inactive{background:#ffe8e8;color:#c14949!important}.am-sa-pill.is-pending{background:#fff4dc;color:#a86500!important}/* v48 */
        .am-sa-pg-hide{display:none!important}
        .am-sa-pager{display:flex;justify-content:space-between;align-items:center;gap:10px;padding:12px 4px 0;font-size:12px;color:#64748b}
        .am-sa-pager div{display:flex;gap:6px}.am-sa-pager button{min-width:32px;height:32px;border:1px solid #d5dee8;border-radius:7px;background:#fff;color:#334155;cursor:pointer}
        .am-sa-pager button[aria-current=page]{background:#1f6fe5;border-color:#1f6fe5;color:#fff}.am-sa-pager button:disabled{opacity:.45;cursor:default}
    </style>
    <script>
        (function () {
            var SIZE = 10, page = 0, sortCol = -1, dir = 1;
            function table() { return document.getElementById('GridView_New_Registered_BO'); }
            function body(t) { return t.tBodies[0]; }
            function rows(t) { return Array.prototype.filter.call(body(t).rows, function (r) { return !r.querySelector('th'); }); }
            function shown(r) { return r.style.display !== 'none'; }
            function key(r, i) { var c = r.cells[i]; var v = c ? c.textContent.trim() : ''; var n = v.replace(/[,\s$]/g, ''); return n !== '' && !isNaN(n) ? parseFloat(n) : (/^\d{4}-\d{2}-\d{2}/.test(v) ? v : v.toLowerCase()); }
            function paginate() {
                var t = table(); if (!t) return; var all = rows(t), vis = all.filter(shown), pages = Math.max(1, Math.ceil(vis.length / SIZE));
                if (page >= pages) page = pages - 1;
                all.forEach(function (r) { r.classList.remove('am-sa-pg-hide'); });
                vis.forEach(function (r, i) { if (i < page * SIZE || i >= (page + 1) * SIZE) r.classList.add('am-sa-pg-hide'); });
                var p = document.getElementById('amSaSubPager'); if (!p) { p = document.createElement('nav'); p.id = 'amSaSubPager'; p.className = 'am-sa-pager'; p.setAttribute('aria-label', 'Subscriber pages'); t.parentNode.parentNode.insertBefore(p, t.parentNode.nextSibling); }
                var from = vis.length ? page * SIZE + 1 : 0, to = Math.min(vis.length, (page + 1) * SIZE);
                var h = '<span>Showing ' + from + '-' + to + ' of ' + vis.length + '</span><div><button type="button" data-p="' + (page - 1) + '"' + (page === 0 ? ' disabled' : '') + ' aria-label="Previous page">&lsaquo;</button>';
                for (var i = 0; i < pages; i++) h += '<button type="button" data-p="' + i + '"' + (i === page ? ' aria-current="page"' : '') + '>' + (i + 1) + '</button>';
                p.innerHTML = h + '<button type="button" data-p="' + (page + 1) + '"' + (page >= pages - 1 ? ' disabled' : '') + ' aria-label="Next page">&rsaquo;</button></div>';
            }
            function init() {
                var t = table(); if (!t || t.dataset.amSaSort) return; t.dataset.amSaSort = '1';
                var head = t.tHead ? t.tHead.rows[0] : t.rows[0]; if (!head) return;
                Array.prototype.forEach.call(head.cells, function (th, i) {
                    if (/^(status|delete)$/i.test(th.textContent.trim())) { th.className += ' am-nosort'; return; }
                    th.tabIndex = 0; th.setAttribute('aria-sort', 'none');
                    function go() {
                        dir = sortCol === i ? -dir : 1; sortCol = i;
                        Array.prototype.forEach.call(head.cells, function (c) { if (c.hasAttribute('aria-sort')) c.setAttribute('aria-sort', 'none'); });
                        th.setAttribute('aria-sort', dir === 1 ? 'ascending' : 'descending');
                        var list = rows(t); list.sort(function (a, b) { var x = key(a, i), y = key(b, i); return x < y ? -dir : x > y ? dir : 0; });
                        list.forEach(function (r) { body(t).appendChild(r); }); page = 0; paginate();
                    }
                    th.addEventListener('click', go); th.addEventListener('keydown', function (e) { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); go(); } });
                });
                document.addEventListener('click', function (e) { var b = e.target.closest && e.target.closest('#amSaSubPager button[data-p]'); if (b && !b.disabled) { page = +b.getAttribute('data-p'); paginate(); } });
                // Re-page after the shared "Search this list" / Status filter hides or shows rows.
                document.addEventListener('input', function (e) { if (e.target.closest && e.target.closest('.am-filter')) setTimeout(function () { page = 0; paginate(); }, 0); }, true);
                document.addEventListener('change', function (e) { if (e.target.closest && e.target.closest('.am-filter')) setTimeout(function () { page = 0; paginate(); }, 0); }, true);
                document.addEventListener('click', function (e) { if (e.target.closest && e.target.closest('.am-filter button')) setTimeout(function () { page = 0; paginate(); }, 0); }, true);
                paginate();
            }
            if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
            window.addEventListener('load', function () { var t = table(); if (t && t.dataset.amSaSort) paginate(); else init(); });
        }());
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
     <asp:LinkButton ID="lnkDummy" runat="server"></asp:LinkButton>
    <cc1:ModalPopupExtender ID="ModalPopupBussList" BehaviorID="mpe" runat="server"
        PopupControlID="pnlPopup" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            <asp:Label ID="lblbusName" runat="server"></asp:Label>
            <asp:HiddenField ID="hfdPopUpBusid" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>

        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GridView_PopUp_Business_List" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" Width="110%" AutoGenerateColumns="False" CellPadding="1"
                        ForeColor="#333333" OnRowCommand="GridView_PopUp_Business_List_RowCommand" 
                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="GridView_PopUp_Business_List_PreRender" 
                        OnRowDataBound="GridView_PopUp_Business_List_RowDataBound">
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <AlternatingRowStyle BackColor="#f5f5f5" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name">
                                <ItemTemplate>
                                <asp:Label ID="LBLBusinessName" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "CompanyName")%>'></asp:Label>
                                <asp:HiddenField ID="Bid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="Employee Count">
                                <ItemTemplate>
                                <asp:Label ID="LBLempcnt" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "empcnt")%>'></asp:Label>
                               
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Status">
                                <ItemTemplate>
                                        <asp:HiddenField ID="hdbID" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>'
                                            runat="server" />
                                            <asp:ImageButton ID="btnactivebus" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' CommandName="Businessstatus" />
                                        
                                    <asp:HiddenField ID="hfdStatusForBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                           
                             
                        </Columns>
                        <EmptyDataTemplate>
                            <div>No records found.</div>
                        </EmptyDataTemplate>
                        <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />
                    </asp:GridView>

                </div>
            </div>
        </div>
    </asp:Panel>

    <cc1:ModalPopupExtender ID="modelpopemp" BehaviorID="mpe1" runat="server"
        PopupControlID="pnlPopupemp" TargetControlID="lnkDummy" BackgroundCssClass="modalBackground"
        CancelControlID="btnHide1">
    </cc1:ModalPopupExtender>
    <asp:Panel ID="pnlPopupemp" runat="server" CssClass="modalPopup" Style="display: none">
        <div class="header">
            <asp:Label ID="lblempna" runat="server"></asp:Label>
            <asp:HiddenField ID="HiddenField1" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide1" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>

        <div class="body">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="gvEmplistPop" runat="server" ClientIDMode="Static"
                        CssClass="table nowrap" Width="110%" AutoGenerateColumns="False" CellPadding="1"
                        ForeColor="#333333" OnRowCommand="gvEmplistPop_RowCommand" 
                        GridLines="None" EmptyDataText="No Records Found" OnPreRender="gvEmplistPop_PreRender" 
                        OnRowDataBound="gvEmplistPop_RowDataBound">
                        <%--HeaderStyle-BackColor="#0275d8" HeaderStyle-ForeColor="White">--%>
                        <AlternatingRowStyle BackColor="#f5f5f5" />
                        <%--<HeaderStyle CssClass="bg-clouds" />--%>

                        <Columns>

                            <asp:TemplateField HeaderText="Sl. No.">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex + 1%>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Name">
                                <ItemTemplate>
                                <asp:Label ID="LBLBusinessName" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "bussname")%>'></asp:Label>
                                <asp:HiddenField ID="Bid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "BusinessID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="Employee Name">
                                <ItemTemplate>
                                <asp:Label ID="LBLempnamepop" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "empname")%>'></asp:Label>
                                <asp:HiddenField ID="eid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="User Type">
                                <ItemTemplate>
                                <asp:Label ID="LBLUserType" runat="server"  Text='<%#DataBinder.Eval(Container.DataItem, "UserType")%>'></asp:Label>
                               
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Business Status">
                                <ItemTemplate>
                                        <asp:HiddenField ID="hdbID" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>'
                                            runat="server" />
                                            <asp:ImageButton ID="btnactiveEMP" runat="server" ImageUrl="~/images/active.png" Width="120px" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' CommandName="EMPSTUS" />
                                    <asp:HiddenField ID="hfdStatusForBussOwner" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Status")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                           
                           
                             
                        </Columns>
                        <EmptyDataTemplate>
                            <div>No records found.</div>
                        </EmptyDataTemplate>
                        <PagerStyle  HorizontalAlign = "Right" CssClass = "GridPager" />
                    </asp:GridView>

                </div>
            </div>
        </div>
    </asp:Panel>
    <div class="panel panel-default" id="div1" runat="server" style="display:none;">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <h2 style="text-align: left; margin-top: 12px;">Subscriber List</h2>
                </div>
                <div class="col-md-6 col-sm-6 col-xs-6">
                    <asp:HyperLink ID="HyperLink1" runat="server" Visible="false" Text="Approve Reseller" NavigateUrl="~/SuperAdmin_Approve_Partner.aspx"
                        class="buttonhyperlink" Style="float: right; margin-top: 12px;"></asp:HyperLink>
                </div>
            </div>

        </div>
    </div>


    <div style="position: relative;display:none;">
        <div class="row">
            <div class="col-md-4">
                <a href="SuperAdmin_Approve_Reseller.aspx" id="A1" runat="server" style="font-size:large;">
                    Reseller List
                </a>
            </div>
        </div>
    </div>

    
    <div class="panel panel-default">
        <div class="panel-body" id="div_New_Registered_BO" runat="server">
            <div class="row">
                <asp:Panel ID ="Panel1" runat="server">
                <div class="col-lg-12 col-md-12">
                    <div class="am-sa-subwrap">
                        <asp:GridView ID="GridView_New_Registered_BO" ClientIDMode="Static" runat="server" CssClass="table am-sa-subgrid"
                            CellPadding="1" ForeColor="#333333" GridLines="None"
                              AutoGenerateColumns="False"
                            EmptyDataText="No records Found" OnPreRender="GridView_New_Registered_BO_PreRender"
                             OnRowCommand="GridView_New_Registered_BO_RowCommand" OnRowDataBound="GridView_New_Registered_BO_RowDataBound">
                            <AlternatingRowStyle BackColor="#f5f5f5" />

                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Accountant Name" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Accountant_MotherOrg_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MotherOrgName")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Subscriber/Reseller Name"><%--HeaderText="Subscriber/Reseller Name"--%>
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Employee_Name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EmpName")%>'></asp:Label>
                                        <%--<asp:HiddenField ID="HiddenField_MotherOrg_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "MotherOrgID")%>' />--%>
                                        <asp:HiddenField ID="HiddenField_MotherOrg_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Title")%>' />
                                        <asp:HiddenField ID="HiddenField_Employee_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' /> 
                                        <asp:HiddenField ID="HiddenField_Partner_Id" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "PartnerID")%>' />
                                        <br />
                                        <%--<asp:HyperLink ID="HyperLink_Edit_Password" ClientIDMode="Static" runat="server" Text="Change Password"
                                            NavigateUrl='<%# "~/Change Password.aspx?SubscriberResellerIDEdit=" + Eval("EmployeeID")%>'></asp:HyperLink>--%>
                                        <asp:HyperLink ID="HyperLink_Edit_Password" ClientIDMode="Static" runat="server" Text="Change Password" 
                                            NavigateUrl='<%# (Eval("PartnerID").ToString().Trim() =="0") ? ("~/Change Password.aspx?SubscriberIDEdit=" + Eval("EmployeeID")) 
                                            : ("~/Change Password.aspx?ResellerIDEdit=" + Eval("PartnerID")) %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Plan Name">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Plan_Name" runat="server" Text=""></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                
                                <asp:TemplateField HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Subscriber_Type" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SubscriberType")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="No. of Business">
                                    <ItemTemplate>
                                       <asp:LinkButton ID="lnkBusinessList" runat="server" Text="Business List" CommandName="bussList" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                 <asp:TemplateField HeaderText="No. of Users">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkEmployeeList" runat="server" Text="Employee List" CommandName="empList" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>'></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Contact Number">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Contact_Phone" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "MobilePhone")%>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                

                                <asp:TemplateField HeaderText="Email ID">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Email_Id" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Email")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%--<asp:TemplateField HeaderText="Enrolled Date">--%>
                                <asp:TemplateField HeaderText="Activation Date">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Enrolled_Date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "EnrollDate1")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%--<asp:TemplateField HeaderText="Password" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="Label_Password" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Password")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>--%>

                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="ImageButton_Active_Deactive" runat="server" OnClientClick="ConfirmStatusChangeBox()" CssClass="am-sa-pill" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Activate_Deactivate_New_Account_Business_Owner" ToolTip="Click to change status" />
                                        <asp:HiddenField ID="HiddenField_Business_Owner_IsActive_Status" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "IsActive")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Delete" Visible="false">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="ImageButton_Delete" runat="server" OnClientClick="ConfirmDeleteBox()" ImageUrl="~/images/delete2.png" Width="50px" Height="20px" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Delete_Partner" /> 
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            
                             <PagerStyle   HorizontalAlign = "Right" CssClass = "GridPager" />

                        </asp:GridView>
                    </div>
                </div>
                </asp:Panel>                
            </div>
        </div>
    </div>
</asp:Content>

