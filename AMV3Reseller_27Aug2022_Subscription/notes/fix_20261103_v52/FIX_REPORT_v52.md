# AnyMinute v52-20261103 — Fix report (local build; no CloudAgent, no push, no live smoke)
Base: v51 `e94fda9`. Meta `anyminute-build` = `v52-20261103` (Main.master, Modern.master). Web.config untouched. No new DB schema. Spec: NOTES_v52.md (reconciles the Login Access PPT and the Shell PPT).

## Fixed (SupportSubscribers.aspx.cs)
1. **Sticky Details shell** now covers `""`/businesses/employees/projects/billing/support. DTabs order: Businesses · Employees · Projects · Subscription & Billing · Login & Access (→view=access) · Support History (in-shell Support()) · **Login Activity History** (→view=history; renamed from "Activity Log").
2. **view=access = standalone dialog (Login PPT s1)**. No Details/DTabs wrapper. Title "Manage Subscriber Login & Access", lead "Subscriber Details › Login & Access", close ✕ (HeadActions) + Cancel → Subscriber Details. Content = v50 `Header(c,r,false)` (restored) + existing `Access()`.
3. **view=history = Login Activity History chrome (Login PPT s3)**. No sticky card. Title "Subscriber Login Activity History" + v50 lead; content = `Header(c,r,true)` + restored history `Tabs()` (Login Activity History selected; **Support History → SupportSubscribers.aspx?user=N&view=support**, not SupportTickets.aspx) + existing `History()` table/filters.
4. view=reset unchanged (slide 2 fixed).
5. **Edit Subscriber address**: the only address block is `AD.Render("ed_addr", …)` (Reseller Address Lookup + Address Details). No "Manage Address" string anywhere in SupportSubscribers.aspx.cs (old "separate Address module" comment text removed as well). `Addr()` now composes street from Clean(StreetNo/StreetName/StreetType), so "Please Select" can't reach the Details header. EmpAddress save path unchanged.
6. CSS `?v=` → `20261103v52` on every page that used `20261102v51` (AnyminuteSupport.css / AnyminuteAddressLookup.css). v51 CSS comment fix + `#amEmpAddress` layout kept.

## Note on the "Please Select / Manage Address" screenshot
The v51 source and the v51 zip already contained only the Address Lookup in Edit Subscriber; the old block does not exist in v51 or v52 code. If it still shows after v52 deploy, the server is serving a stale SupportSubscribers.aspx.cs (overwrite not applied, or the app needs a recycle / Temporary ASP.NET Files clear). Check `<meta name="anyminute-build" content="v52-20261103">` on the Edit page.

## Deferred (not invented)
Stripe, Industry, Add Business creation, Max Projects storage, Address Verification Date, finer report filters.

## Checks
- `mcs` App_Code build succeeded; SupportSubscribers compiles against App_Code.
- css-tree: 0 errors for AnyminuteAddressLookup.css and AnyminuteSupport.css.
