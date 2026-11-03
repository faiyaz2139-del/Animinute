/* 2026-11-01 v46: row menus are fixed pop-overs (never clipped by the table).
   2026-10-29 v30: Custom Range toggles the From / To date inputs. */
/* 2026-10-29 v28: Reports hub - data-sa-noauto (no auto-apply on select change), data-sa-extra (adds e.g. export=pdf)
   and data-sa-form (a button outside the filter box applies that box).
   2026-10-28 v26: Verify Address also queries the existing address lookup (suggestions).
   2026-10-28 v25 Support Admin console (Shamsu decks 1-4): filters, row menus, dialogs, counters and the
   client-side Verify Address check (the server re-verifies on Approve & Add to Database). */
(function () {
  'use strict';
  function $all(sel, root) { return Array.prototype.slice.call((root || document).querySelectorAll(sel)); }
  function apply(box, extra) {
    var q = [];
    $all('input[name],select[name]', box).forEach(function (el) {
      if ((el.type === 'checkbox' || el.type === 'radio') && !el.checked) return;
      if (el.value !== '') q.push(encodeURIComponent(el.name) + '=' + encodeURIComponent(el.value));
    });
    if (extra) q.push(extra);
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
      $all('[data-sa-apply]', box).forEach(function (b) { b.addEventListener('click', function () { apply(box, b.getAttribute('data-sa-extra')); }); });
      if (!box.hasAttribute('data-sa-noauto')) $all('select', box).forEach(function (s) { s.addEventListener('change', function () { apply(box); }); });
    });
    // v30: Report Filters - From / To are only used with Custom Range, so they are disabled for the other presets.
    $all('select[name$="_preset"]').forEach(function (s) {
      var base = s.name.replace(/_preset$/, ''), sync = function () {
        $all('input[name="' + base + '_from"],input[name="' + base + '_to"]').forEach(function (i) { i.disabled = s.value !== 'Custom Range'; });
      };
      s.addEventListener('change', sync); sync();
    });
    $all('[data-sa-genpw]').forEach(function (b) {
      b.addEventListener('click', function () {
        var t = document.getElementById(b.getAttribute('data-sa-genpw')); if (!t) return;
        var sets = ['ABCDEFGHJKLMNPQRSTUVWXYZ', 'abcdefghijkmnpqrstuvwxyz', '23456789', '@#$%!'], all = sets.join(''), r = new Uint32Array(12), out = '';
        (window.crypto || window.msCrypto).getRandomValues(r);
        for (var i = 0; i < 12; i++) { var src = i < 4 ? sets[i] : all; out += src.charAt(r[i] % src.length); }
        t.value = out; t.focus(); t.select();
      });
    });
    $all('[data-sa-form]').forEach(function (b) {
      b.addEventListener('click', function () { var box = document.getElementById(b.getAttribute('data-sa-form')); if (box) apply(box, b.getAttribute('data-sa-extra')); });
    });
    $all('[data-sa-all]').forEach(function (all) {
      all.addEventListener('change', function () { $all('tbody input[type=checkbox]', all.closest('table')).forEach(function (c) { c.checked = all.checked; }); });
    });
    $all('textarea[data-sa-counter]').forEach(function (t) {
      var out = t.parentNode.querySelector('[data-sa-count]'), max = t.getAttribute('data-sa-counter');
      var upd = function () { if (out) out.textContent = t.value.length + '/' + max; };
      t.addEventListener('input', upd); upd();
    });
    // v46 (t148u/t150u): row menus open as a fixed pop-over next to the kebab, so the full menu is visible even inside a
    // scrolling table (.sa-table-wrap no longer clips it). Opens downward, or upward when there is more room above; long
    // menus scroll inside the pop-over. Closes on page scroll / resize.
    var place = function (d) {
      var pop = d.querySelector('.sa-menu-pop'), sum = d.querySelector('summary'); if (!pop || !sum) return;
      pop.classList.add('sa-pop-fixed'); pop.style.maxHeight = ''; pop.style.top = '0px'; pop.style.left = '0px';
      var r = sum.getBoundingClientRect(), w = pop.offsetWidth, h = pop.scrollHeight, vw = document.documentElement.clientWidth, vh = window.innerHeight;
      var left = Math.max(8, Math.min(r.right - w, vw - w - 8)), below = vh - r.bottom - 12, above = r.top - 12;
      var up = h > below && above > below, room = Math.max(160, up ? above : below);
      pop.style.left = left + 'px';
      pop.style.maxHeight = room + 'px';
      pop.style.top = (up ? Math.max(8, r.top - 6 - Math.min(h, room)) : r.bottom + 6) + 'px';
    };
    var unplace = function (d) { var pop = d.querySelector('.sa-menu-pop'); if (pop) { pop.classList.remove('sa-pop-fixed'); pop.style.top = pop.style.left = pop.style.maxHeight = ''; } };
    $all('details.sa-menu').forEach(function (d) {
      d.addEventListener('toggle', function () {
        if (d.open) { $all('details.sa-menu[open]').forEach(function (o) { if (o !== d) o.open = false; }); place(d); }
        else unplace(d);
      });
    });
    window.addEventListener('scroll', function (e) {
      if (e.target && e.target.closest && e.target.closest('.sa-menu-pop')) return;
      $all('details.sa-menu[open]').forEach(function (o) { o.open = false; });
    }, true);
    window.addEventListener('resize', function () { $all('details.sa-menu[open]').forEach(function (o) { o.open = false; }); });
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
      var field = function (n) { return box.querySelector('[name="' + n + '"]'); };
      var show = function (text, ok) { out.textContent = text; out.className = 'sa-verify-result ' + (ok ? 'ok' : 'bad'); };
      // v26: format check, then the existing address lookup (SupportAddresses.aspx?verify=1) with suggestions.
      var run = function () {
        var problem = verify(box);
        if (problem) { show(problem, false); if (approve) approve.disabled = true; return; }
        show('Checking the address lookup...', true);
        var q = ['verify=1'];
        [['id', 'sa_id'], ['street', 'sa_street'], ['city', 'sa_city'], ['prov', 'sa_prov'], ['country', 'sa_country'], ['postal', 'sa_postal']].forEach(function (p) {
          var el = field(p[1]); q.push(p[0] + '=' + encodeURIComponent(el ? el.value.trim() : ''));
        });
        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'SupportAddresses.aspx?' + q.join('&'));
        xhr.onload = function () {
          var r = null; try { r = JSON.parse(xhr.responseText); } catch (err) { r = null; }
          if (!r) { show('Format is valid. The address lookup could not be reached; the server checks again on Approve.', true); if (approve) approve.disabled = false; return; }
          show(r.message, r.level !== 'error');
          if (approve) approve.disabled = r.level === 'error';
          (r.suggestions || []).forEach(function (sg) {
            var b = document.createElement('button');
            b.type = 'button'; b.className = 'sa-btn sa-btn-sm sa-btn-outline sa-suggest';
            b.textContent = 'Use: ' + sg.street + ', ' + sg.city + ', ' + sg.province + ' ' + sg.postal + ' (' + sg.source + ')';
            b.addEventListener('click', function () {
              field('sa_street').value = sg.street; field('sa_city').value = sg.city; field('sa_prov').value = sg.province; field('sa_postal').value = sg.postal; run();
            });
            out.appendChild(document.createElement('br')); out.appendChild(b);
          });
        };
        xhr.onerror = function () { show('Format is valid. The address lookup could not be reached; the server checks again on Approve.', true); if (approve) approve.disabled = false; };
        xhr.send();
      };
      if (btn) btn.addEventListener('click', run);
      if (window.location.hash === '#sa-reject' || /[?&]reject=1/.test(window.location.search)) { var n = box.querySelector('[name="sa_notes"]'); if (n) n.focus(); }
    });
    // v42 PPT 01 s1: Subscriber list select-all checkbox.
    $all('[data-sa-all]').forEach(function (m) { m.addEventListener('change', function () { $all('input[name="' + m.getAttribute('data-sa-all') + '"]').forEach(function (x) { x.checked = m.checked; }); }); });
    // v43 PPT 01 s5/s6: plan select fills the plan limits (empty catalogue limit = Custom, editable); Manage Custom Limits; Change Plan.
    $all('[data-sa-plan]').forEach(function (sel) {
      var box = sel.closest('.sa-card') || document;
      var fill = function () {
        var o = sel.options[sel.selectedIndex]; if (!o) return;
        [['b', 'data-b'], ['e', 'data-e']].forEach(function (k) {
          var inp = box.querySelector('[data-sa-lim="' + k[0] + '"]'); if (!inp || !o.hasAttribute(k[1])) return;
          var v = o.getAttribute(k[1]); if (v !== '') { inp.value = v; inp.readOnly = true; } else { inp.readOnly = false; }
        });
        var p = document.getElementById(sel.getAttribute('data-sa-proj')); if (p && o.hasAttribute('data-p')) p.value = o.getAttribute('data-p') || 'Custom';
      };
      sel.addEventListener('change', fill);
    });
    $all('[data-sa-unlock]').forEach(function (b) { b.addEventListener('click', function () { var box = b.closest('.sa-card') || document; $all('[data-sa-lim]').forEach(function (x) { if (box.contains(x)) x.readOnly = false; }); var f = box.querySelector('[data-sa-lim]'); if (f) f.focus(); }); });
    // v53: a collapsed Address module (Manage Address gate) keeps its fields in the DOM but parks "required" so a hidden empty
    // field cannot block Save; revealing it restores "required", hides the gate card and marks {prefix}_open = 1.
    $all('[data-sa-addrpanel][hidden] [required]').forEach(function (x) { x.removeAttribute('required'); x.setAttribute('data-sa-req', '1'); });
    $all('[data-sa-reveal]').forEach(function (b) { b.addEventListener('click', function () { var el = document.getElementById(b.getAttribute('data-sa-reveal')); if (el) { el.hidden = false;
      var g = b.getAttribute('data-sa-gate'); if (g) { $all('[data-sa-req]').forEach(function (x) { if (el.contains(x)) x.setAttribute('required', 'required'); }); var gc = document.getElementById(g + '_gate'), op = document.getElementById(g + '_open'); if (gc) gc.hidden = true; if (op) op.value = '1'; var fi = el.querySelector('input:not([type=hidden]),select'); if (fi) fi.focus(); return; }
      var s = el.querySelector('select'); if (s) s.focus(); } }); });
    // v43 PPT 01 s5: Search Address (postal code lookup) or enter manually.
    // v46 (Address deck s1/s2, Paused deck s1): Business-style Address Lookup module on Add / Edit Reseller and Add Subscriber.
    var REG = {
      'Canada': ['Alberta','British Columbia','Manitoba','New Brunswick','Newfoundland and Labrador','Northwest Territories','Nova Scotia','Nunavut','Ontario','Prince Edward Island','Quebec','Saskatchewan','Yukon'],
      'United States': ['Alabama','Alaska','Arizona','Arkansas','California','Colorado','Connecticut','Delaware','District of Columbia','Florida','Georgia','Hawaii','Idaho','Illinois','Indiana','Iowa','Kansas','Kentucky','Louisiana','Maine','Maryland','Massachusetts','Michigan','Minnesota','Mississippi','Missouri','Montana','Nebraska','Nevada','New Hampshire','New Jersey','New Mexico','New York','North Carolina','North Dakota','Ohio','Oklahoma','Oregon','Pennsylvania','Rhode Island','South Carolina','South Dakota','Tennessee','Texas','Utah','Vermont','Virginia','Washington','West Virginia','Wisconsin','Wyoming']
    };
    $all('[data-sa-addrmod]').forEach(function (box) {
      var pre = box.getAttribute('data-sa-addr-prefix'), url = box.getAttribute('data-sa-addrmod');
      var q = function (s) { return box.querySelector(s); }, f = function (k) { return document.getElementById(pre + k); };
      var lc = q('[data-al-lcountry]'), lp = q('[data-al-lpostal]'), flag = q('[data-al-flag]'), hint = q('[data-al-lhint]'), hintText = q('[data-al-lhinttext]'),
          nf = q('[data-al-lnf]'), nfCode = q('[data-al-lnfcode]'), hits = q('[data-al-lhits]'), mode = q('[data-al-lmode]'), dc = f('_country'), prov = f('_prov');
      var us = function () { return lc.value === 'United States'; };
      var fillProv = function (country, keep) {
        var list = REG[country] || REG.Canada, v = keep || '', html = '<option value="">' + (country === 'United States' ? 'Select state' : 'Select province') + '</option>', hit = false;
        list.forEach(function (n) { var on = n.toLowerCase() === v.toLowerCase(); hit = hit || on; html += '<option' + (on ? ' selected' : '') + '>' + n + '</option>'; });
        if (v && !hit) html += '<option selected>' + v.replace(/</g, '&lt;') + '</option>';
        prov.innerHTML = html;
      };
      var sync = function () {
        var u = us(); if (flag) flag.classList.toggle('is-us', u);
        lp.placeholder = u ? 'e.g. 33185' : 'e.g. M1B 2W1';
        if (hintText) hintText.textContent = u ? 'Enter a valid US ZIP code (e.g. 33185) or Canadian postal code (e.g. M1B 2W1).' : 'Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).';
      };
      var reset = function () { nf.hidden = true; hint.hidden = false; };
      var fmt = function (v) { var c = v.replace(/[\s-]/g, '').toUpperCase(); return /^[A-Z]\d[A-Z]\d[A-Z]\d$/.test(c) ? c.slice(0, 3) + ' ' + c.slice(3) : v.trim().toUpperCase(); };
      var setCountry = function (c) { if (dc.value !== c) { dc.value = c; fillProv(c, prov.value); } };
      lc.addEventListener('change', function () { sync(); reset(); setCountry(lc.value); });
      dc.addEventListener('change', function () { fillProv(dc.value, ''); });
      q('[data-al-lclear]').addEventListener('click', function () { lp.value = ''; reset(); hits.innerHTML = '<option value="">Select an address from the list below</option>'; hits.disabled = true; lp.focus(); });
      var search = function () {
        var v = lp.value.trim(), c = v.replace(/[\s-]/g, '').toUpperCase();
        var ok = /^[A-Z]\d[A-Z]\d[A-Z]\d$/.test(c) || /^\d{5}(\d{4})?$/.test(c);
        if (!ok) { reset(); lp.setAttribute('aria-invalid', 'true'); lp.focus(); return; }
        lp.removeAttribute('aria-invalid');
        if (/^\d/.test(c) && !us()) { lc.value = 'United States'; sync(); setCountry('United States'); }
        else if (/^[A-Z]/.test(c) && us()) { lc.value = 'Canada'; sync(); setCountry('Canada'); }
        var xhr = new XMLHttpRequest(); xhr.open('GET', url + encodeURIComponent(v));
        xhr.onload = function () {
          var list = []; try { list = JSON.parse(xhr.responseText) || []; } catch (e) { list = []; }
          hits.innerHTML = '<option value="">Select an address from the list below</option>';
          if (!list.length) {
            // Paused deck s1: only the "Address Not Found" card (the blue hint and the plain text line are hidden).
            nfCode.textContent = fmt(v); nf.hidden = false; hint.hidden = true; hits.disabled = true; return;
          }
          reset();
          list.forEach(function (a, i) { var o = document.createElement('option'); o.value = String(i); o.textContent = [a.street, a.city, a.province].filter(Boolean).join(', '); o._a = a; hits.appendChild(o); });
          hits.disabled = false; hits._list = list; hits.focus();
        };
        xhr.send();
      };
      q('[data-al-lsearch]').addEventListener('click', search);
      lp.addEventListener('keydown', function (e) { if (e.key === 'Enter') { e.preventDefault(); search(); } });
      lp.addEventListener('input', function () { if (!nf.hidden) reset(); });
      hits.addEventListener('change', function () {
        var a = hits._list && hits._list[+hits.value]; if (!a) return;
        f('_line1').value = a.street || ''; f('_city').value = a.city || '';
        setCountry(lc.value); fillProv(lc.value, a.province || '');
        f('_postal').value = fmt(lp.value);
        if (mode) mode.textContent = 'Selected Address';
        f('_no').focus();
      });
      q('[data-al-lmanual]').addEventListener('click', function () {
        reset(); if (mode) mode.textContent = 'Manual Entry';
        setCountry(lc.value); if (!f('_postal').value && lp.value.trim()) f('_postal').value = fmt(lp.value);
        f('_no').focus(); f('_no').scrollIntoView({ block: 'center', behavior: 'smooth' });
      });
      sync();
    });
    $all('[data-sa-addr]').forEach(function (b) {
      var sec = b.closest('section') || document, out = sec.querySelector('[data-sa-addr-hits]');
      var q = function (a) { return sec.querySelector('[' + a + ']'); };
      b.addEventListener('click', function () {
        var pc = q('data-sa-postal'); if (!pc || !pc.value.trim()) { if (pc) pc.focus(); return; }
        out.hidden = false; out.textContent = 'Searching...';
        var xhr = new XMLHttpRequest(); xhr.open('GET', b.getAttribute('data-sa-addr') + encodeURIComponent(pc.value.trim()));
        xhr.onload = function () {
          var list = []; try { list = JSON.parse(xhr.responseText); } catch (e) { }
          out.textContent = list.length ? 'Select an address:' : 'No address found for this postal code. Enter the address manually.';
          list.forEach(function (a) {
            var x = document.createElement('button'); x.type = 'button'; x.className = 'sa-btn sa-btn-outline sa-btn-sm'; x.textContent = a.street + ', ' + a.city + ', ' + a.province;
            x.addEventListener('click', function () {
              q('data-sa-street').value = a.street; q('data-sa-city').value = a.city;
              var pv = q('data-sa-prov'), hit = false;
              for (var i = 0; i < pv.options.length; i++) if (pv.options[i].value.toLowerCase() === a.province.toLowerCase()) { pv.selectedIndex = i; hit = true; }
              if (!hit) { var o = document.createElement('option'); o.value = o.textContent = a.province; pv.appendChild(o); pv.value = a.province; }
              out.hidden = true;
            });
            out.appendChild(document.createElement('br')); out.appendChild(x);
          });
        };
        xhr.onerror = function () { out.textContent = 'The address lookup could not be reached. Enter the address manually.'; };
        xhr.send();
      });
    });
    // v43 PPT 01 s5: "Creating Subscriber Account" overlay while the account is created.
    $all('[data-sa-busy]').forEach(function (b) { b.addEventListener('click', function () { var o = document.querySelector('[data-sa-busy-box]'); setTimeout(function () { if (o) o.hidden = false; b.disabled = true; }, 0); }); });
  });
})();
