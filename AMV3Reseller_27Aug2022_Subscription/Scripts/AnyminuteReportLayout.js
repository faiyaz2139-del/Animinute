/* 2026-11-01 v46: shared report layout - "Search reports..." filters the left list and opens matching groups. */
(function () {
  'use strict';
  document.addEventListener('input', function (e) {
    var s = e.target; if (!s.hasAttribute || !s.hasAttribute('data-rl-search')) return;
    var nav = s.closest('.am-rl-nav'); if (!nav) return;
    var q = s.value.trim().toLowerCase(), hits = 0;
    Array.prototype.forEach.call(nav.querySelectorAll('.am-rl-group'), function (g) {
      var n = 0;
      Array.prototype.forEach.call(g.querySelectorAll('li[data-rl-text]'), function (li) {
        var on = !q || li.getAttribute('data-rl-text').indexOf(q) >= 0; li.hidden = !on; if (on) n++;
      });
      g.hidden = n === 0; if (q && n) g.open = true; hits += n;
    });
    var none = nav.querySelector('.am-rl-nohit'); if (none) none.hidden = hits > 0;
  });
  document.addEventListener('keydown', function (e) { if (e.key === 'Enter' && e.target.hasAttribute && e.target.hasAttribute('data-rl-search')) e.preventDefault(); });
})();
