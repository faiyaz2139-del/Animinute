# AnyMinute v54-20261103 — Fix report (local build; no CloudAgent, no push, no live smoke)
Base: v53 `b392854`. Meta `anyminute-build` = `v54-20261103` (Main.master, Modern.master). Web.config untouched. No DB schema change. Spec: NOTES_v54.md (docx 2026-11-03, img_01/img_02).

## Fixed
1. **Send Password Reset keeps the sticky Subscriber Details header (img_01 → img_02)** — SupportSubscribers.aspx.cs: `view=reset` moved out of the standalone `else if` and into the Details shell branch (with businesses/employees/projects/billing/access/support/history). Render = `Details(c,r)` + `DTabs(c,r,"access")` (Login & Access tab selected) + `Reset(r,ready)` in `sa-tabpad`. `Title1`/`Lead` empty. Breadcrumb: Subscribers › Subscriber Details › Login & Access (link) › Send Password Reset. Reset form Cancel → `SupportSubscribers.aspx?user=N&view=access`. Reset steps/UI unchanged. Edit Subscriber stays a separate page.

## Cache bust
CSS/JS `?v=` → `20261103v54` and meta → `v54-20261103` on every page that carried v53 (SupportSubscribers, Support*, SuperAdmin*, EmployeeEditor, EmployeeProfile, Main/Modern masters).

## Checks
- mcs: App_Code.dll builds; SupportSubscribers.aspx compiles against it.
- grep: reset path in shell uses `Details(` + `DTabs(c, r, on)` with on="access"; Cancel href has `view=access`; no standalone `view == "reset"` branch left.

## Risk
- Reset step 1 "Subscriber Information" now repeats some of what the sticky header shows (kept per "do not redesign").
- Validation-error redirects (`Back(u,"reset")`) land on the shelled reset view — intended.

## Deferred (unchanged)
Stripe, Industry, Add Business creation, Max Projects storage, Address Verification Date.
