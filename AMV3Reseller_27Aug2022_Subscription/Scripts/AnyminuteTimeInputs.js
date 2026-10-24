(function (window) {
    'use strict';

    // Use a text input rather than input[type=datetime]. The legacy HTML datetime
    // browser control can interpret supplied values in a different time zone.
    // Timesheet times are local wall-clock values; never convert them through UTC.
    function normalizeExisting(value) {
        value = (value || '').trim();
        var m = value.match(/^(\d{4})-(\d{1,2})-(\d{1,2})[ T](\d{1,2}):(\d{2})(?::\d{2})?\s*(AM|PM)$/i);
        if (!m) {
            m = value.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})\s+(\d{1,2}):(\d{2})(?::\d{2})?\s*(AM|PM)$/i);
            if (m) m = [m[0],m[3],m[1],m[2],m[4],m[5],m[6]];
        }
        if (!m) return value; // preserve unrecognized or manually-entered values
        var year = +m[1], month = +m[2], day = +m[3], hour = +m[4], minute = +m[5];
        if (month < 1 || month > 12 || day < 1 || day > 31 || hour < 1 || hour > 12 || minute > 59) return value;
        function pad(x) { return ('0' + x).slice(-2); }
        return year + '-' + pad(month) + '-' + pad(day) + ' ' + pad(hour) + ':' + pad(minute) + ' ' + m[6].toUpperCase();
    }

    function openPicker(input, defaultTime) {
        if (!input || input.disabled || input.readOnly) return;
        var jq = window.jQuery;
        if (!jq || !jq.fn || typeof jq.fn.datetimepicker !== 'function') return;
        var field = jq(input);
        if (!field.data('xdsoft_datetimepicker')) {
            var original = input.value;
            var normalized = normalizeExisting(original);
            if (normalized !== original) input.value = normalized;
            var row = input.parentNode;
            while (row && row.tagName !== 'TR') row = row.parentNode;
            var label = row && row.querySelector("[id$='lblDate'], [id*='_lblDate_'], [id^='lblDate_']");
            var date = label ? (label.textContent || '').trim() : '';
            var options = {
                format: 'Y-m-d h:i A',
                formatTime: 'h:i A',
                step: 15,
                defaultTime: defaultTime,
                validateOnBlur: false,
                allowBlank: true,
                // No UTC conversion or automatic adjustment of the displayed hour.
                onShow: function () { return true; }
            };
            if (/^\d{4}-\d{2}-\d{2}$/.test(date)) {
                options.formatDate = 'Y-m-d';
                options.defaultDate = date;
            }
            field.datetimepicker(options);
        }
        field.datetimepicker('show');
    }

    window.setStartDateTime = function (input) { openPicker(input, '08:00'); };
    window.setEndDateTime = function (input) { openPicker(input, '18:00'); };
}(window));
