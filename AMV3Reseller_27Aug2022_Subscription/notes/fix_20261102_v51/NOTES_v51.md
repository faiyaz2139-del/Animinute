# NOTES v51 — Fix all Nov 2, 2026

Sources: PPT NOTES.pptx (4 slides) + held chat shots under `/workspace/anymin/notes/shot_20261102_held/`.
Site: `AMV3Reseller_27Aug2022_Subscription` only. Base: v50 commit `53a7d1f`. Cumulative zip `AnyMinute_Fix_20261102_v51.zip`. Local lean only — no CloudAgent, no push, no live smoke.

## A. PPT slides (strict)

### Slide 1 — Login and Access Tab is Missing
On Subscriber Details, add a **Login & Access** tab (missing today). Tabs should include Businesses / Employees / Projects / Subscription & Billing / Login & Access / Support History / Activity Log (names as live, counts where applicable).

### Slide 2 — Support History under the subscriber, not separate
Support History must display **under the subscriber** (same details shell), not jump to a standalone Support Tickets page. Keep filters/list for that subscriber's tickets inside the tab.

### Slide 3 — Login Activity History: opening on separate page should be fixed
Login Activity History must stay in the Subscriber Details shell (same header + tabs). Do **not** open as a standalone page with a different header. Red X on the history-only tab strip means that separate-page chrome is wrong.

### Slide 4 — Header stays the same while clicking tabs
The Subscriber Details header card (avatar, name, plan, email/phone/address, dates, limits, Edit / Send Password Reset / More) **must remain unchanged** when switching Businesses / Employees / Projects / etc. Only the tab body changes.

## B. Held chat notes (same ship)

1. **Employee Profile address alignment** (`EmployeeProfile.aspx?ProfileEmployeeId=…`): Address Lookup / Address Details clipped and misaligned; search results floating. Fix layout (broken CSS comment in `AnyminuteAddressLookup.css` ~L107 orphan `);` is a known suspect). Shot: `EmployeeProfile_align.jpg`.

2. **Subscriber list + Reseller list (both):** Actions = **View** button in Actions column, then **⋮** in the **next** column. Remove **View Subscriber Details** from the ⋮ menu (redundant with View). Shots: `SubscriberList_actions.jpg`, `SubscriberActions_menu.jpg`.

3. **Subscriber Manage Address = Reseller:** Edit Subscriber Address block (red-marked) must use the same Address Lookup flow as Reseller (not “Please Select / separate Address module” only). Shots: `EditSubscriber_address.jpg`, `EditSubscriber_address_marked.jpg`.

## Deferred (do not invent)
Address Verification Date, Industry column data, Add Business creation if still blocked, Max Projects storage, Stripe, finer report mock filters.

## Deliver
meta `v51-20261102`; commit; FIX_REPORT_v51.md; CHANGES_v51.diff; zip + sha256 under `/workspace/anymin/`.

## B+. Report branding (held while v51 builds)
Address Verification (and other) PDF/report titles: use the **relevant company name**, not hard-coded "ANYMINUTE". Shot: `AddressVerification_report_branding.jpg` ("ANYMINUTE | ADDRESS VERIFICATION REPORT" → company name).

8. **Support Tickets list:** same Actions pattern — **View** in Actions column, **⋮** in the next column. Shot: `SupportTickets_actions.jpg`.
