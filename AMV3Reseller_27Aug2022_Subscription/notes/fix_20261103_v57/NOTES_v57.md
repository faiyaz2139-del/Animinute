# AnyMinute — NOTES v57 (SHIPPED) (PPTX "Include this as well")
- Date: Tue Nov 3, 2026, ~4:10 PM ET (America/Toronto)
- Source: Faiyaz message t184u — "Include this as well" — 4-slide PPTX (source.pptx)
- Status: **SHIPPED v57** (Faiyaz "Fix all" t185u) — code `9ef5cca`, meta v57-20261103, ?v=20261103v57. See FIX_REPORT_v57.md. S1–S4 all done.
- Portal: **Support Admin** portal (sidebar: Dashboard, Resellers, Subscribers, Businesses, Subscription Plans, Usage & Limits, Support Tickets, Address Verification, Account Recovery, Reports) — NOT the Reseller portal from v55/v56. Screens = Subscriber Details (SupportSubscribers.aspx view=detail / view=edit).
- Rule: red boxes + slide captions win over prose.

## Slide 1 — Subscriber Details › Businesses tab › "+ Add Business" (red box)  — HIGH
Shown: Fawaz Thakor, BASIC PLAN, limits Businesses 5/6, Employees 0/8, Projects 0/5. Businesses (5) table.
Caption: "Add Business" asks for an upgrade, and the **Upgrade button is not working**. Shows 1 business can still be added (5/6) but won't let me add — **it should let me add**.
Gaps:
1. Add Business must be allowed when active < limit (5 < 6) — no upgrade prompt. Limit check likely off-by-one / counting wrong (all businesses incl. inactive?, or `>=` vs `>`), or Add Business is still the "deferred creation" stub (v51–v53 deferred "Add Business creation") which always shows the upgrade panel (SupportSubscribers.aspx.cs ~L355 `<details>... businesses used ... Upgrade Plan`).
2. Upgrade Plan button (header + in that panel) does nothing — links to `&view=edit#plan`; anchor/section likely missing or not scrolling/opening. Must open a working plan-change flow.
Pages: SupportSubscribers.aspx(.cs) (detail Businesses tab, limits header, edit #plan), possibly Add_Business.aspx / AnyminuteBusinessEditor.js if reused for support-side create.
Coverage: **NEW** (was deferred; not in v55/v56).

## Slide 2 — Businesses tab › Actions column (red box) — MEDIUM
Caption: Action – change from "Edit" to "View".
Gap: per-row action button label (and icon pencil→eye) "Edit" → "View", linking to the business view page (SupportBusinesses detail). Keep the ⋮ kebab.
Pages: SupportSubscribers.aspx.cs (Businesses tab row render). Check same pattern on Employees/Projects tabs — do NOT change unless asked (ambiguous).
Coverage: **NEW**.

## Slide 3 — Subscriber Edit › Address › "Address Not Found" card (red box) — LOW/MEDIUM
Caption: Can you make it in "Red or Highlight More".
Gap: Address Not Found card currently neutral/grey border with ⓘ icon. Make it red/danger: red border + light red background, red icon + red bold title. Probably shared style (AnyminuteAddressLookup.js / shared CSS) → would apply to Add_Business, EmployeeEditor, EmployeeProfile too (consistent; recommended).
Pages: SupportSubscribers.aspx (edit address block), Scripts/AnyminuteAddressLookup.js / AnyminuteSupport.js, shared CSS.
Coverage: **NEW** (v46 made only-the-card; colour never changed).

## Slide 4 — Subscriber Edit › manual address → Address Verification (red box) — HIGH
Caption: Couldn't find address, used "Enter Address Manually" (168 Bloor St W, Oshawa, ON L1J 1P7). It should go to Address Verification, but no "Pending Verification" or "Verified Address" status shown.
Evidence: Recent Activity logs "Address: 168 Bloor St W, Oshawa, Ontario L1J 1P7, Canada" (saved OK). Address Details badge still says "Selected Address".
Gaps:
1. Manually-entered (not lookup-verified) subscriber address on Save must create/flag an Address Verification record with status **Pending** → appears in SupportAddresses.aspx "Pending Verification" tab (and sidebar badge count).
2. Show the address status on Subscriber Details/Edit: badge "Pending Verification" (amber/red) or "Verified Address" (green) instead of static "Selected Address"; Manual entry badge should not say "Selected Address".
Pages: SupportSubscribers.aspx(.cs) save path + address block, SupportAddresses.aspx(.cs) (source query), AnyminuteAddressLookup.js (manual-mode flag → hidden field).
Coverage: **NEW** (Address Verification Date was deferred in v51–v53; this is broader).

## Already covered by v55/v56
- None of these four. v55/v56 were Reseller portal (Dashboard/sidebar). Subscriber Details layout/limits tiles exist from earlier builds.

## Non-goals / deferred
- Stripe / real payment for Upgrade (Upgrade = change plan in edit #plan only).
- Industry column data ("—"), Max Projects storage — unchanged.
- No changes to Reseller portal.

## Ship plan on "Fix"
- Lean cumulative **v57** on v56 base `a8a6086`, local executor only (no CloudAgent, no push, no live smoke).
- Order: S4 (pending verification) → S1 (Add Business allowed under limit + Upgrade works) → S2 (Edit→View) → S3 (red card).
- Build check, zip AnyMinute_Fix_20261103_v57.zip + SHA256, meta v57-20261103.
