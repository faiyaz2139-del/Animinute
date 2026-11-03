# AnyMinute v55-20261103 — Fix report (local build; no CloudAgent, no push, no live smoke)
Base: v54 `22830cc`. Meta `anyminute-build` = `v55-20261103` (Main.master, Modern.master). CSS/JS `?v=20261103v55`. Web.config untouched. **No DB schema change** (uses REQUIRED_09 AnyminuteReseller / AnyminuteResellerSubscriber). Spec: NOTES_v55.md + RESELLER_SCREENS.jpg (slides 1-9).

## What a reseller login gets now
A reseller signs in with the UserAcc login that `AnyminuteResellers.EnsureLogin` created for `AnyminuteReseller.LoginEmail`.
- **Detection** — new `App_Code/AnyminuteResellerPortal.cs` `Current()`: not Support/SuperAdmin, session login = `RS.LoginUser(email)`, reseller row Active. Permissions CSV parsed (subscribers, businesses, plans, usage, reports, upgrades). Cached 2 min per session (keyed by UserId).
- **Login redirect** — Login.aspx.cs: the UserTypeid 0 / Admin branch sends a reseller to `ResellerDashboard.aspx` (accountants unchanged → Dashboard.aspx). Modern.master also redirects a reseller who opens Dashboard.aspx (brand link / bookmarks).
- **Sidebar** — `AnyminuteUiNavigation.Render`: pure reseller gets only the PPT nav: Dashboard · Subscribers · Businesses · Subscription Plans · Usage & Limits · Reports · Settings (permission-gated; Settings always). `li.am-sa-nav` reuses the pinned Support drawer, so content is never under the sidebar. No Support Admin or owner menus.
- **Header** — AnyMinute mark + "Reseller" label; Welcome = reseller name, initials avatar; help "?" removed.
- **Data scope** — every reseller screen only reads `AnyminuteResellerSubscriber` rows of that ResellerID.

## Screen by screen
| # | Screen | Shipped |
|---|---|---|
| 1 | Dashboard | `ResellerDashboard.aspx`: Total / Active / Inactive Subscribers, Total Businesses / Employees / Projects (scoped, set-based SQL) + Quick Actions Add Subscriber, View Subscribers, Subscription Plans, Usage Limits, Reports (each permission-gated). |
| 2 | Subscriber Management | `SupportSubscribers.aspx` in reseller mode (`RList`): search, Plan / Status filters, Clear / Search; columns #, Subscriber Name, Plan Name, Businesses, Employees, Projects, Status, Actions (View + ⋮); "Showing x to y of z subscribers", per page, pager. |
| 3 | Add Subscriber | Same Add Subscriber form/confirm/created flow; on create the subscriber is inserted into `AnyminuteResellerSubscriber` for this reseller. `MaxSubscribers` enforced. |
| 4 | Edit Subscriber | Same Edit page (header card, Send Password Reset / More, Personal, Contact, Plan & Limits, Save + Confirm/Reason). Without `upgrades`: Change Plan / Manage Custom Limits hidden, dates read-only, and **server keeps plan, limits and dates** on save. |
| 5 | Subscriber Details | Sticky header + tabs **Businesses · Employees · Projects · Subscription & Billing · Login & Access · Activity Log** (Support History tab removed for resellers; `view=support` falls back to Businesses). Upgrade Plan / gear hidden without `upgrades`; Add Business popover hidden without `businesses`. |
| 6 | Login & Access | Inside the sticky shell, Login & Access selected (existing v53 view). |
| 7 | Send Password Reset | Inside the sticky shell, Login & Access selected (v54 pattern), Cancel → access. |
| 8 | Login Activity History | Inside the sticky shell, Activity Log tab selected. |
| 9 | Subscription Plans | `ResellerDashboard.aspx?view=plans`: catalogue (#, Plan, Description, Monthly / Annual CAD, Businesses, Employees, Projects, Status, View) from `AnyminuteSubscriptionPlan`; View opens a read-only plan card. |
| – | Businesses nav | `?view=businesses`: businesses under the reseller's subscribers (search, pager, View → subscriber Businesses tab). |
| – | Usage & Limits | `?view=usage`: per subscriber Businesses / Employees / Projects used vs plan limits (read-only). |
| – | Reports | Existing `SupportReports.aspx` reseller scope (v49), shown only with `reports`. |
| – | Settings | `?view=settings`: reseller account summary (name, ID, permissions, subscriber limit) + My Profile, Login & Security, Change Password. |

Links inside reseller subscriber screens that pointed at Support-only pages are rewritten in `Render` (SupportPlans → plans view, SupportPlans?view=usage → usage view, SupportBusinesses?q= → businesses view). Support Admin rendering untouched (`Rsl == null` → `base.Render`).

## Deferred
- **+ Add Plan** (slide 9): reseller permission is "View subscription plans" only; no reseller plan-create API — not shown.
- Edit Business inside Details for resellers without `businesses` is blocked server-side (Edit buttons on the Businesses tab still render and fall back to Details).
- Stripe billing, API permission, reseller self-edit of its own reseller record.
- URL stays `SupportSubscribers.aspx` for reseller subscriber screens (same precedent as v49 `SupportReports.aspx`) to keep one code path with Support's v53/v54 fixes.

## Checks
- mcs: App_Code.dll builds (incl. AnyminuteResellerPortal.cs); ResellerDashboard.aspx, SupportSubscribers.aspx, Login.aspx, Modern.master compile against it.
- grep: Login.aspx.cs reseller redirect → ResellerDashboard.aspx; Modern.master Dashboard→ResellerDashboard; SupportSubscribers `Page_Init` still `SA.Require()` for non-reseller; Support NavHtml path unchanged.

## Risks
- Untested against a live DB/IIS (no live smoke by brief). SQL uses existing tables/columns only (same as SupportSubscribers / AnyminuteResellers).
- If the `AnyminuteResellerSubscriber` insert fails after Add Subscriber, the subscriber exists but is not visible to the reseller (logged; Support can assign under Resellers).
- `Current()` cache is 2 min: permission or status changes by Support Admin take up to 2 min to apply to an open reseller session (Disable Login still revokes sessions immediately).
- Reseller audit rows show the reseller as "Changed By" (SA.ActorName) — same tables as Support.
