# AnyMinute Fix v56 — 2026-11-03 (Reseller Dashboard + sidebar, PPT slide_01 strict)

Note: t182u "Dashboard and menu items for reseller are not fixed as given in the reference image." → t183u "Fix all strictly".
Base v55 `404aa6a` → commit `a8a6086` · meta `v56-20261103` · cache-bust `?v=20261103v56`. No DB change. Web.config untouched.

## Changes
- `App_Code/AnyminuteResellerPortal.cs` — NavHtml always emits the full 7-item menu; new `NavIcon` SVG line icons replace the empty `<i>`.
- `ResellerDashboard.aspx.cs` — Dashboard home crumb **Reseller**; all 5 Quick Actions always (no permission omission; target pages gate).
- `ResellerDashboard.aspx` — crumb / KPI / Quick Actions CSS (3×2 KPI grid, 48px circles, solid Active/Inactive).
- `SupportSubscribers.aspx.cs` — reseller without subscribers permission gets a flash before the existing redirect (nav link now always visible).
- Meta bump on Modern.master / Main.master; ?v bump on all aspx carrying 20261103v55.

## Screen checklist (slide_01)
- [x] Sidebar 7 items + icons  - [x] Reseller crumb + title + lead  - [x] 6 KPIs  - [x] 5 Quick Actions always

## Verify
- mcs: App_Code.dll builds; ResellerDashboard.aspx + SupportSubscribers.aspx compile.
- Support Admin NavHtml / SA.Require untouched.
- Local only: no CloudAgent, no push, no live smoke.

## Risks
- A reseller missing a permission now sees the link; clicking lands on Dashboard with a warning (Reports for a reseller without report scope keeps its existing behaviour).
