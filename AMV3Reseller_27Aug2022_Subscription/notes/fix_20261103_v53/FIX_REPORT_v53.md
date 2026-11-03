# AnyMinute v53-20261103 — Fix report (local build; no CloudAgent, no push, no live smoke)
Base: v52 `2c32575`. Meta `anyminute-build` = `v53-20261103` (Main.master, Modern.master). Web.config untouched. No DB schema change. Spec: NOTES_v53.md (docx 2026-11-03, img_01..04).

## Fixed
1. **Resellers list (img_04)** — SupportResellers.aspx.cs list grid: the `RSL-00N` sub-line under Reseller Name is removed; the name link stays. Queries, search-by-RSL, CSV/Excel export, reseller detail header and create message keep the RSL ID (display-only change).
2. **Subscriber Details sticky header on every tab (img_03)** — SupportSubscribers.aspx.cs: `view=access` and `view=history` render inside the same `Details(c,r)` + `DTabs` shell again (v51 pattern). v52 standalone "Manage Subscriber Login & Access" / "Subscriber Login Activity History" chrome (Header()/Tabs()) is no longer used. Tab body only changes: access → `Access()` (in `sa-tabpad`), support → `Support()`, history → `History()`. Labels kept: Login & Access · Support History · Login Activity History. Breadcrumb sub-crumb for access/support/history. `view=reset` and `view=edit` stay separate pages.
3. **Edit Reseller — Manage Address gate (img_02 → img_01)** — new collapsed Address card: pin + "Address", current composed address or **Please Select**, "Address changes are managed through the separate Address module.", outline **Manage Address**. Click reveals the full Address Lookup + Address Details (`AD.Render("rs_addr")`) in place (no navigation). Fields stay in the DOM while collapsed. Hidden `rs_addr_open` flag: not opened → stored Address kept as-is (no re-validation); opened → validated + composed as before. Postback with the module opened re-renders it open. Add Reseller unchanged (always-open module).
4. **Edit Subscriber — same gate** — `AD.Gate("ed_addr", …)` wraps the existing "Address (Optional)" + `AD.Render("ed_addr")`. Not opened → address unchanged (`cur[12]`); opened → existing EmpAddress save path + untouched-form check. Summary uses `SubAddr()` (Clean() on every fragment) so "Please Select" is only ever the empty-state label, never stored or shown in the Details header. Confirm Changes / Reason flow unchanged.

## Shared pieces
- `App_Code/AnyminuteSupportAddress.cs`: `Gate(prefix, summary, open, panel)` + `Opened(prefix)`.
- `Scripts/AnyminuteSupport.js`: existing `data-sa-reveal` extended — for `data-sa-gate` buttons it hides the gate card, sets `{prefix}_open=1`, restores `required` (parked on load inside hidden panels so a hidden empty field can't block Save).
- `css/AnyminuteSupport.css`: `.sa-addr-gate*` styles. CSS `?v=` → `20261103v53`; `AnyminuteSupport.js?v=` → `20261103v53` on every page that loads it.

## Checks
- mcs: App_Code.dll builds; SupportSubscribers.aspx and SupportResellers.aspx compile against it. `node --check` AnyminuteSupport.js OK. css-tree: 0 errors (AnyminuteSupport.css).

## Risk
- Edit Reseller with an empty stored address: Save without opening Manage Address returns "Address is required. Click Manage Address to enter it." (address is required for resellers).
- If JS fails to load, the module stays hidden and Save keeps the stored address (safe default).
- Header()/Tabs() helpers remain in code but unused.

## Deferred (unchanged)
Stripe, Industry, Add Business creation, Max Projects storage, Address Verification Date.
