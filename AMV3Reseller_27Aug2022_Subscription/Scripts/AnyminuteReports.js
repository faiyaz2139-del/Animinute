/* 2026-10-26 v19: Reports hub / Login & Access / Report Access helpers.
   Pages sit inside the Modern.master WebForms <form>, so filter panels are <div data-rp-form> blocks that navigate with GET. */
(function () {
  'use strict';
  function pad(n) { return (n < 10 ? '0' : '') + n; }
  function iso(d) { return d.getFullYear() + '-' + pad(d.getMonth() + 1) + '-' + pad(d.getDate()); }
  function go(box, exportKind) {
    var params = [];
    var fields = box.querySelectorAll('input[name],select[name]');
    for (var i = 0; i < fields.length; i++) {
      var f = fields[i];
      if ((f.type === 'checkbox' || f.type === 'radio') && !f.checked) continue;
      if (f.value === '' || f.value == null) continue;
      if (f.required && !f.value) { f.focus(); return; }
      params.push(encodeURIComponent(f.name) + '=' + encodeURIComponent(f.value));
    }
    var from = box.querySelector('input[name="from"]'), to = box.querySelector('input[name="to"]');
    if (from && from.required && (!from.value || !to.value)) { (from.value ? to : from).focus(); return; }
    if (exportKind) params.push('export=' + exportKind);
    var url = (box.getAttribute('data-action') || location.pathname) + (params.length ? '?' + params.join('&') : '');
    location.href = url;
  }
  function preset(sel) {
    var box = sel.closest('[data-rp-form]'); if (!box) return;
    var from = box.querySelector('input[name="from"]'), to = box.querySelector('input[name="to"]'); if (!from || !to) return;
    var t = new Date(), a = null, b = null, dow = (t.getDay() + 6) % 7;
    switch (sel.value) {
      case 'this-month': a = new Date(t.getFullYear(), t.getMonth(), 1); b = new Date(t.getFullYear(), t.getMonth() + 1, 0); break;
      case 'last-month': a = new Date(t.getFullYear(), t.getMonth() - 1, 1); b = new Date(t.getFullYear(), t.getMonth(), 0); break;
      case 'this-week': a = new Date(t.getFullYear(), t.getMonth(), t.getDate() - dow); b = new Date(a.getFullYear(), a.getMonth(), a.getDate() + 6); break;
      case 'last-week': a = new Date(t.getFullYear(), t.getMonth(), t.getDate() - dow - 7); b = new Date(a.getFullYear(), a.getMonth(), a.getDate() + 6); break;
      case 'ytd': a = new Date(t.getFullYear(), 0, 1); b = t; break;
      case 'any': from.value = ''; to.value = ''; return;
      default: return;
    }
    from.value = iso(a); to.value = iso(b);
  }
  document.addEventListener('click', function (e) {
    var b = e.target.closest ? e.target.closest('[data-rp-go]') : null;
    if (!b) return;
    e.preventDefault();
    go(b.closest('[data-rp-form]'), b.getAttribute('data-rp-export'));
  });
  document.addEventListener('change', function (e) {
    var t = e.target;
    if (t.hasAttribute && t.hasAttribute('data-rp-autosubmit')) go(t.closest('[data-rp-form]'), null);
    else if (t.hasAttribute && t.hasAttribute('data-rp-preset')) preset(t);
  });
  // Enter inside a filter panel previews instead of posting the WebForms page.
  document.addEventListener('keydown', function (e) {
    if (e.key !== 'Enter' || !e.target.closest) return;
    var box = e.target.closest('[data-rp-form]');
    if (box) { e.preventDefault(); go(box, null); }
    else if (e.target.hasAttribute('data-rp-search')) e.preventDefault();
  });
  document.addEventListener('input', function (e) {
    var s = e.target; if (!s.hasAttribute || !s.hasAttribute('data-rp-search')) return;
    var q = s.value.trim().toLowerCase();
    var rows = document.querySelectorAll(s.getAttribute('data-rp-scope') || '.am-rp-item');
    for (var i = 0; i < rows.length; i++) {
      var text = rows[i].getAttribute('data-rp-text') || rows[i].textContent.toLowerCase();
      rows[i].style.display = !q || text.indexOf(q) >= 0 ? '' : 'none';
    }
  });
  // Report Access: "Use Global Defaults" toggle hides/shows the override matrix without a round trip.
  document.addEventListener('change', function (e) {
    var t = e.target; if (!t.hasAttribute || !t.hasAttribute('data-ra-toggle')) return;
    var panel = document.getElementById(t.getAttribute('data-ra-toggle')); if (panel) panel.classList.toggle('is-disabled', t.checked);
    var boxes = panel ? panel.querySelectorAll('input[type=checkbox]') : [];
    for (var i = 0; i < boxes.length; i++) boxes[i].disabled = t.checked;
  });
})();
