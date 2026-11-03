# AnyMinute v53 — Fix-all notes (docx 2026-11-03)

Source: NOTES.docx (+ img_01..04). Build on v52 (2c32575). Local lean cumulative zip only. NO CloudAgent. NO git push. NO live smoke.

## MUST-FIX (all in one ship)

### 1. Resellers list — remove unique id (img_04)
- SupportResellers.aspx list grid: under Reseller Name, remove the `RSL-00N` / `RS.Rsl(...)` sub-line (circled in red).
- Name link stays. Export/CSV may keep RSL if already there; the on-screen grid must not show it.
- Do not remove ResellerID from data/queries — display only.

### 2. Subscriber Details sticky header for EVERY tab (img_03)
- Red-boxed area (avatar, name, status, plan since, Edit / Send Password Reset / More, Email/Phone/Address, Activation/Next Billing, Subscription Limits) must NOT change when clicking any of:
  Businesses · Employees · Projects · Subscription & Billing · Login & Access · Support History · Login Activity History
- Put `view=access` and `view=history` back inside the same Details + DTabs shell (v51 pattern). Undo v52 standalone Manage Login / History chrome for those tabs.
- Keep tab labels from v52: "Login & Access", "Support History", "Login Activity History".
- Tab body only changes: access → Access(...); history → History(...); support stays Support(...).
- `view=reset` and `view=edit` stay separate pages (not in the sticky shell).
- Breadcrumbs: Details root for businesses; sub-crumb OK for access/support/history.

### 3. Edit Reseller — Manage Address gate (img_02 collapsed + img_01 open)
- Edit Reseller Address section must match the red-boxed collapsed card:
  - Pin icon + heading "Address"
  - Summary line: current composed address, OR literal **Please Select** when empty
  - Small muted: "Address changes are managed through the separate Address module."
  - Button **Manage Address** (outline)
- On Manage Address click: reveal/open the full Address Lookup + Address Details module (`AD.Render` / AnyminuteSupportAddress) in place (img_01). Do NOT navigate to SupportAddresses.aspx.
- Add Reseller can keep the always-open Address module (notes only cover Edit).
- Save must still persist address: keep populated `rs_addr_*` fields in the DOM when collapsed (hidden panel), or preserve existing address when Manage Address was never opened.

### 4. Edit Subscriber — same Manage Address gate (same images)
- Same collapsed card UI as Edit Reseller (Please Select / module text / Manage Address).
- Manage Address reveals `AD.Render("ed_addr", …)` inline (same lookup as Reseller).
- Keep EmpAddress save path + Clean() so "Please Select" never reaches the Details header address.
- Confirm Changes / reason flow unchanged.

## Out of scope
- Stripe / Industry / Add Business / Max Projects / Address Verification Date (still deferred).
- GitHub push.
- Live smoke (wait for Deployed).

## Ship checklist
- Meta `v53-20261103` in Main.master + Modern.master
- CSS `?v=20261103v53` on pages that had v52
- Commit + `AnyMinute_Fix_20261103_v53.zip` under `/workspace/anymin/` (overlay like v52)
- FIX_REPORT_v53.md + CHANGES_v53.diff + notes copy in site/notes/fix_20261103_v53/
- Compile App_Code + SupportSubscribers/SupportResellers with mcs; css-tree if CSS touched
- Grep zip: no RSL- under list name cell pattern as shipped; "Manage Address" present on Edit Subscriber + Edit Reseller; access/history rendered with Details( shell )
