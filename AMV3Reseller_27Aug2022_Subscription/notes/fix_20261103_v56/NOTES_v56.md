# v56 SHIPPED — Reseller Dashboard + menu vs PPT slide_01 (Fix all strictly, t183u)

Source: Faiyaz note (t182u, from iPhone): "Dashboard and menu items for reseller are not fixed as given in the reference image."
No new screenshot attached — compare against slide_01.png / RESELLER_SCREENS.jpg (already held).
Shipped on Fix all strictly (t183u) as v56 `a8a6086` on v55 `404aa6a`.

## Note intent (same phrasing as Support Admin v37 "Menu items … is not fixed")
Live Reseller Dashboard + sidebar do not match the PPT mock (slide 1).

## Gaps vs slide_01 (code review of v55)

| Area | PPT (slide 1) | v55 today | Fix when asked |
|---|---|---|---|
| Sidebar labels | Dashboard · Subscribers · Businesses · Subscription Plans · Usage & Limits · Reports · Settings | Same labels when permissions allow | Confirm live shows this set (not owner Quick Actions menu). If `Current()` fails → owner menu still shows — harden detect / redirect. |
| Sidebar icons | Line-art icons per item | Empty `<i class=''>` (no SVG) | Emit SVG icons per item (accountant `NavIcon` style): home, users, building, paperclip/doc, bars, report, gear. |
| Dashboard crumb | Bold **Reseller** above title | Crumbs empty on home Dashboard() | Set crumb/label to **Reseller** on dashboard home. |
| Title / lead | Reseller Dashboard + “Manage your subscribers…” | Already matches | — |
| KPI row | 6 cards, 2×3, colored circular icons | `rsl-kpis` + SVG KPIs — structure OK | Tighten CSS to PPT (card padding, icon circle sizes, Active/Inactive labels). |
| Quick Actions | Always 5: Add Subscriber, View Subscribers, Subscription Plans, Usage Limits, Reports | Permission-gated (hidden if no perm) | Match PPT: show all five (or show greyed); keep server-side permission checks. |
| Header brand | AnyMinute mark in chrome; “Reseller” in content | Brand injects “Reseller” in top bar | Align with mock: keep Reseller label as content crumb, not only top-bar. |

## Non-goals until clarified
- Other 8 slides (list/add/edit/details/access/reset/history/plans) — note only called out Dashboard + menu.
- + Add Plan still deferred.

## Ship when Fix
- Lean cumulative **v56** on v55; meta `v56-20261103`; local executor only; no CloudAgent / push / smoke unless asked.

## Shipped v56 (a8a6086, meta v56-20261103, ?v=20261103v56)
- Sidebar: always 7 items (Dashboard · Subscribers · Businesses · Subscription Plans · Usage & Limits · Reports · Settings) with SVG line icons (home, users, building, paperclip, bars, document, gear); li.am-sa-nav am-rsl-nav kept; aria-current as before. Permission gates stay server-side (Dashboard views flash + fall back; Subscribers now flashes before redirect; Reports keeps its scope gate).
- Dashboard: bold **Reseller** crumb above "Reseller Dashboard"; lead unchanged.
- KPIs: 6 cards 3×2, 48px circular icons, solid green Active / solid red Inactive.
- Quick Actions: all 5 always (Add Subscriber, View Subscribers, Subscription Plans, Usage Limits, Reports).
- Header: AnyMinute mark + Reseller top-bar label kept; detection/redirect untouched.
