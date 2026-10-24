(function () {
    'use strict';
    window.AnyminuteConfirmDelete = function (button) {
        if (button.dataset.deleteConfirmed === 'yes') {
            delete button.dataset.deleteConfirmed;
            return true;
        }
        if (document.getElementById('anyminute-delete-dialog')) return false;
        var dialog = document.createElement('dialog');
        if (typeof dialog.showModal !== 'function') {
            return window.confirm('Permanently delete this employee? This action cannot be undone.');
        }
        dialog.id = 'anyminute-delete-dialog';
        dialog.setAttribute('aria-labelledby', 'anyminute-delete-title');
        var title = document.createElement('h3');
        title.id = 'anyminute-delete-title';
        title.textContent = 'Permanently delete this employee?';
        var text = document.createElement('p');
        text.textContent = 'This action cannot be undone. Choose No to keep the employee.';
        var no = document.createElement('button');
        no.type = 'button'; no.textContent = 'No';
        var yes = document.createElement('button');
        yes.type = 'button'; yes.textContent = 'Yes, delete';
        yes.style.marginLeft = '1rem';
        dialog.appendChild(title); dialog.appendChild(text);
        dialog.appendChild(no); dialog.appendChild(yes);
        document.body.appendChild(dialog);
        dialog.addEventListener('close', function () { dialog.remove(); button.focus(); });
        no.addEventListener('click', function () { dialog.close(); });
        yes.addEventListener('click', function () {
            dialog.close();
            button.dataset.deleteConfirmed = 'yes';
            button.click();
        });
        dialog.showModal(); no.focus();
        return false;
    };
}());

// One field bound to the exact clicked business-assignment control.
window.AnyminuteConfirmAssignmentDelete = function (button) {
    var form = button.form || button.closest('form');
    if (!form) return false;
    var field = form.querySelector('input[name="anyminute_assignment_delete"]');
    if (!field) {
        field = document.createElement('input');
        field.type = 'hidden'; field.name = 'anyminute_assignment_delete';
        form.appendChild(field);
    }
    field.value = '';
    if (!window.confirm('Remove this user from this business? Attendance history, address and pay records will be retained.')) return false;
    field.value = button.id;
    return true;
};
