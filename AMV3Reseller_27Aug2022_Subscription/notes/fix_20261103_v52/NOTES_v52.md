# NOTES v52 — Fix properly Nov 3, 2026 (no flip-flop)

User: Fix properly and finally, no back and forth.
Sources:
- Login Access PPT (3 slides): Slide 2 Send Password Reset = FIXED. Slides 1 + 3 NOT fixed.
- Shell PPT (4 slides, re-sent as “also not fixed”): Login & Access tab, Support History under subscriber, Activity not separate page, sticky header.
- Shot: Edit Subscriber Address still “Please Select” + Manage Address (old UI).

Base: v51 `e94fda9`. Site: `AMV3Reseller_27Aug2022_Subscription` ONLY. Local lean executor. NO CloudAgent, NO push, NO live smoke. Meta `v52-20261103`. Cumulative zip `AnyMinute_Fix_20261103_v52.zip`.

## Final UX (reconcile both decks — do not undo one to satisfy the other)

### A. Subscriber Details shell (sticky header)
Views: ``, `businesses`, `employees`, `projects`, `billing`, `support`.
- Render `Details(c,r)` + `DTabs` + body. Header card unchanged when switching these tabs.
- **DTabs must include** (in order): Businesses · Employees · Projects · Subscription & Billing · **Login & Access** · Support History · **Login Activity History** (label; was “Activity Log”).
- Login & Access tab href → `view=access` (standalone Manage page below).
- Login Activity History tab href → `view=history`.
- Support History body stays in this shell (`Support(c,r)`), never jump to SupportTickets list as page chrome.

### B. Slide 1 — Manage Subscriber Login & Access (standalone, restore v50)
`view=access` must NOT wrap in Details/DTabs.
- Title: `Manage Subscriber Login & Access`
- Lead/breadcrumb: `Subscriber Details › Login & Access` (+ close/Cancel back to Details)
- Header bar: avatar, name, Subscriber ID, email | Current Plan + View Plan Details | Subscriber Since | Active/Disabled badge (v50 `Header(c,r,false)`)
- Body: existing `Access()` (Current Login Information, Change Login Access radios + warn + Reason 0/500, action cards, Cancel/Save)
- Restore `Header` helper from v50 if missing.

### C. Slide 2 — Send Password Reset — DO NOT CHANGE (already fixed)

### D. Slide 3 — Subscriber Login Activity History (restore v50 chrome, keep in-app)
`view=history` must NOT use the sticky Details card (Edit / limits / More).
- Title: `Subscriber Login Activity History`
- Lead: `View sign-in activity, account changes and security events for this subscriber.`
- Header: v50 `Header(c,r,true)` (name+badge; ID | plan | email | phone; Subscription Since; Last Login)
- Tabs: restore history-page `Tabs` with **Login Activity History** selected; Profile/Businesses/…/Login & Access; **Support History → `view=support`** (NOT SupportTickets.aspx); Login Activity History → view=history
- Body: existing `History()` filters + 8 columns + pager “… activities”

### E. Edit Subscriber Address (red box) — must match Reseller
Live still shows “Please Select” + “Address changes are managed through the separate Address module” + Manage Address.
- Edit form Address section = only `AnyminuteSupportAddress.Render` (lookup + details), same as Reseller. No Manage Address link, no “Please Select” placeholder text as content.
- `Addr()` / header address: `Clean()` every street fragment so “Please Select” never appears in Details header.
- Confirm save path still writes EmpAddress like Add Subscriber.
- If a second old address block remains anywhere in Edit, remove it.

### F. EmployeeProfile address CSS
Keep v51 CSS comment fix + `#amEmpAddress` stacked layout; bump `?v=` to `20261103v52` on pages that use AnyminuteAddressLookup.css / AnyminuteSupport.css.

## Deferred (do not invent)
Stripe, Industry data, Add Business creation, Max Projects storage, Address Verification Date, finer report filters.

## Deliver
- meta `v52-20261103` in Main.master + Modern.master
- commit on top of e94fda9
- FIX_REPORT_v52.md, CHANGES_v52.diff, NOTES copy under site/notes/fix_20261103_v52/
- zip + sha256 under /workspace/anymin/
- Compile check SupportSubscribers + App_Code; css-tree clean
- Verify zip contains SupportSubscribers.aspx.cs with “Manage Subscriber Login” title and AnyminuteSupportAddress.Render in Edit (no “Manage Address” string)
