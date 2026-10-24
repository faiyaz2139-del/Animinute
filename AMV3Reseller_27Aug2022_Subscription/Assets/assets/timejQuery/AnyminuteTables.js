(function (window) {
    'use strict';
    function enhance() {
        var $ = window.jQuery;
        if (!$ || !$.fn || !$.fn.dataTable) return;
        ['GvEmpListForBusOwner', 'gvbusinessListForManager', 'GvEmpListOwner',
         'gvBusinessListEmp', 'GvPopUpEmpList', 'grdviewProject',
         'GridView_Accountant_User', 'GridView_Custome_User_Role'].forEach(function (id) {
            var table = window.document.getElementById(id);
            if (!table || $.fn.dataTable.isDataTable(table)) return;
            // ASP.NET empty rows use colspan; DataTables cannot process those as data.
            // Keep the ordinary table when headers/body do not form a rectangular grid.
            if (!table.tHead || table.tHead.rows.length !== 1 || !table.tBodies.length) return;
            var columns = table.tHead.rows[0].cells.length;
            if (!columns) return;
            var valid = true, count = 0;
            function check(row) {
                if (row.cells.length !== columns) valid = false;
                Array.prototype.forEach.call(row.cells, function (cell) {
                    if (cell.colSpan !== 1 || cell.rowSpan !== 1) valid = false;
                });
            }
            check(table.tHead.rows[0]);
            Array.prototype.forEach.call(table.tBodies, function (body) {
                Array.prototype.forEach.call(body.rows, function (row) { count++; check(row); });
            });
            if (valid && count) $(table).DataTable({});
        });
    }
    if (window.jQuery) window.jQuery(enhance);
    // Partial postbacks replace tables; initialize each new element just once.
    if (window.Sys && window.Sys.Application) window.Sys.Application.add_load(enhance);
    else if (window.addEventListener) window.addEventListener('load', function () {
        enhance();
        if (window.Sys && window.Sys.Application) window.Sys.Application.add_load(enhance);
    });
}(window));
