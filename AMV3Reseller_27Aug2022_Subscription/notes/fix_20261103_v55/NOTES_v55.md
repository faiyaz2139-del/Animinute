# AnyMinute v55 — Reseller portal (Fix all)

Source: RESELLER_SCREENS.jpg / slide_01..09.png (held 2026-11-03). Base: v54 `22830cc`.
Local lean cumulative. NO CloudAgent. NO push. NO live smoke.

## Goal
Ship the **Reseller login portal** matching the 9 PPT mocks. Today a reseller UserAcc (MotherOrg Admin via EnsureLogin) lands on Dashboard.aspx with the normal business menu; only “Reseller Reports” is added. Build dedicated Reseller screens + nav + post-login redirect.

## Auth / scope (existing)
- Detect reseller: `AnyminuteSupportReportsScope.ResellerScope()` / LoginEmail match on Active AnyminuteReseller (extend helper if needed into `AnyminuteResellers` e.g. `Current()`).
- Data scope: only `AnyminuteResellerSubscriber` rows for that ResellerID (+ their businesses/employees/projects).
- Permissions CSV: `subscribers`, `businesses`, `plans`, `usage`, `reports`, `upgrades` (see PermissionList). Hide nav items the reseller lacks. Settings always OK (profile/security).

## Nav (PPT sidebar)
Dashboard · Subscribers · Businesses · Subscription Plans · Usage & Limits · Reports · Settings  
Brand line shows **Reseller** (mock). Prefer Support-style drawer classes reused for reseller (`am-rsl-nav` or similar) so content isn’t under the sidebar. Do not show Support Admin menu or normal owner Quick Actions menu for pure reseller logins.

Login.aspx (or post-login): if reseller → `ResellerDashboard.aspx` (not Dashboard.aspx).

## Screens (match mocks; reuse Support CSS `sa-*` / Address lookup where useful)

| # | Page | Notes |
|---|------|--------|
| 1 | ResellerDashboard.aspx | KPIs: Total/Active/Inactive Subscribers; Total Businesses/Employees/Projects (scoped). Quick Actions → Add Subscriber, View Subscribers, Plans, Usage, Reports (permission-gated). |
| 2 | ResellerSubscribers.aspx list | Columns: #, Name, Plan, Businesses, Employees, Projects, Status, View+⋮. Search + Plan/Status filters + pager. |
| 3 | Add Subscriber | Personal, Contact, Plan & Limits (Plan + Max Biz/Emp/Proj), Account Activation (Pending/Active/Inactive + Send invitation). Assign to this reseller on create. Reuse AnyminuteSubscriberCreate / SupportSubscribers add path if possible, scoped. |
| 4 | Edit Subscriber | Header card; Send Password Reset / More; Personal, Contact, Plan & Limits; Save. Only own subscribers. |
| 5 | Subscriber Details | Sticky header (Edit / Send Password Reset / More, contact, limits progress). Tabs: Businesses · Employees · Projects · Subscription & Billing · Login & Access · Activity Log. Businesses table + Add Business if `businesses` perm. |
| 6 | Manage Login & Access | Current login info + Enable/Disable + Reason; Resend Invitation / Send Password Reset / View Login History. Prefer **inside sticky Details** with Login & Access selected (consistent with Support v53/v54 sticky rule) even if mock title looks standalone. |
| 7 | Send Password Reset | Keep sticky Details + Login & Access tab (v54 pattern). Form steps as mock; Cancel → access. |
| 8 | Login Activity History | Inside sticky shell with Activity Log tab selected (or history). Filters + table as mock. |
| 9 | Subscription Plans | Catalogue list (from existing plans source SupportPlans uses). **+ Add Plan**: permission is “View subscription plans” — show list/View as mock; do **not** invent reseller plan-create unless upgrades+clear API exists; if Add is shown, gate or defer with note in FIX_REPORT. |

Also:
- **Businesses** nav → list of businesses under their subscribers (not SupportBusinesses admin).
- **Usage & Limits** → per-subscriber or aggregate usage vs plan limits (read-only).
- **Reports** → existing SupportReports.aspx Reseller scope (already v49).
- **Settings** → MyProfile / LoginSecurity links page, or lightweight ResellerSettings.

## Non-goals / defer
- Stripe billing, creating brand-new plan products as reseller, Support Admin SupportResellers changes (already v53).
- CloudAgent, git push, live smoke.

## Ship
- Meta `v55-20261103`; CSS/JS cache bust `20261103v55`
- Zip `/workspace/anymin/AnyMinute_Fix_20261103_v55.zip` cumulative on v54
- FIX_REPORT_v55.md, CHANGES_v55.diff, notes in site/notes/fix_20261103_v55/
- mcs compile new App_Code + pages
