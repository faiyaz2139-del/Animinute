/* 2026-10-28 v25 Support Admin console (Shamsu decks 1-4): filters, row menus, dialogs, counters and the
   client-side Verify Address check (the server re-verifies on Approve & Add to Database). */
(function () {
  'use strict';
  function $all(sel, root) { return Array.prototype.slice.call((root || document).querySelectorAll(sel)); }
  function apply(box) {
    var q = [];
    $all('input[name],select[name]', box).forEach(function (el) {
      if ((el.type === 'checkbox' || el.type === 'radio') && !el.checked) return;
      if (el.value !== '') q.push(encodeURIComponent(el.name) + '=' + encodeURIComponent(el.value));
    });
    window.location.href = box.getAttribute('data-sa-get') + (q.length ? '?' + q.join('&') : '');
  }
  var rules = {
    Canada: /^[A-Z]\d[A-Z][ -]?\d[A-Z]\d$/, USA: /^\d{5}(-\d{4})?$/, India: /^\d{6}$/, UK: /^[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}$/
  };
  function verify(box) {
    var v = function (n) { var el = box.querySelector('[name="' + n + '"]'); return el ? el.value.trim() : ''; };
    var country = v('sa_country'), postal = v('sa_postal').toUpperCase();
    if (v('sa_street').length < 3) return 'Street address is required.';
    if (v('sa_city').length < 2) return 'City is required.';
    if (v('sa_prov').length < 2) return 'Province / State is required.';
    if (!rules[country]) return 'Choose a country.';
    if (!rules[country].test(postal)) return 'Postal / ZIP / PIN code does not match the ' + country + ' format.';
    return null;
  }
  document.addEventListener('DOMContentLoaded', function () {
    $all('[data-sa-get]').forEach(function (box) {
      box.addEventListener('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); apply(box); } });
      $all('[data-sa-apply]', box).forEach(function (b) { b.addEventListener('click', function () { apply(box); }); });
      $all('select', box).forEach(function (s) { s.addEventListener('change', function () { apply(box); }); });
    });
    $all('[data-sa-all]').forEach(function (all) {
      all.addEventListener('change', function () { $all('tbody input[type=checkbox]', all.closest('table')).forEach(function (c) { c.checked = all.checked; }); });
    });
    $all('textarea[data-sa-counter]').forEach(function (t) {
      var out = t.parentNode.querySelector('[data-sa-count]'), max = t.getAttribute('data-sa-counter');
      var upd = function () { if (out) out.textContent = t.value.length + '/' + max; };
      t.addEventListener('input', upd); upd();
    });
    $all('details.sa-menu').forEach(function (d) {
      d.addEventListener('toggle', function () { if (d.open) $all('details.sa-menu[open]').forEach(function (o) { if (o !== d) o.open = false; }); });
    });
    document.addEventListener('click', function (e) {
      if (!e.target.closest('details.sa-menu')) $all('details.sa-menu[open]').forEach(function (o) { o.open = false; });
      var close = e.target.closest('[data-sa-close]');
      if (close) { var m = close.closest('.sa-modal'); if (m) m.style.display = 'none'; }
      var btn = e.target.closest('button[name="sa_op"]');
      if (!btn) return;
      var need = btn.getAttribute('data-sa-need-notes');
      if (need) { var n = document.querySelector('[name="sa_notes"]'); if (n && !n.value.trim()) { e.preventDefault(); alert(need); n.focus(); return; } }
      var ask = btn.getAttribute('data-sa-confirm');
      if (ask && !window.confirm(ask)) { e.preventDefault(); return; }
      var set = btn.getAttribute('data-sa-set');
      if (set) { var kv = set.split('='), h = document.querySelector('input[type=hidden][name="' + kv[0] + '"]'); if (h) h.value = kv[1]; }
    });
    document.addEventListener('keydown', function (e) {
      if (e.key === 'Escape') $all('.sa-modal[data-sa-modal]').forEach(function (m) { m.style.display = 'none'; });
    });
    $all('[data-sa-address]').forEach(function (box) {
      var approve = box.querySelector('[data-sa-approve]'), out = box.querySelector('[data-sa-verify-result]'), btn = box.querySelector('[data-sa-verify]');
      var reset = function () { if (approve) approve.disabled = true; if (out) { out.textContent = ''; out.className = 'sa-verify-result'; } };
      $all('input,select', box).forEach(function (el) { el.addEventListener('input', reset); el.addEventListener('change', reset); });
      if (btn) btn.addEventListener('click', function () {
        var problem = verify(box);
        out.textContent = problem ? problem : 'Address verified. Review the notes, then Approve & Add to Database.';
        out.className = 'sa-verify-result ' + (problem ? 'bad' : 'ok');
        if (approve) approve.disabled = !!problem;
      });
      if (window.location.hash === '#sa-reject') { var n = box.querySelector('[name="sa_notes"]'); if (n) n.focus(); }
    });
  });
})();
