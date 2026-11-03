# AnyMinute v51-20261102 — Fix report (local build; no CloudAgent, no push, no live smoke)

Base: v50 `53a7d1f`. Meta `anyminute-build` = `v51-20261102` (Main.master, Modern.master). Web.config untouched. No new DB schema.

## Fixed
1. **Login & Access tab** on Subscriber Details (`SupportSubscribers.aspx?user=N&view=access`). Tabs are now Businesses · Employees · Projects · Subscription & Billing · Login & Access · Support History · Activity Log.
2. **Support History in the shell** (`view=support`). The subscriber's tickets (matched on Ticket.EmailId) show with KPIs, search, status, priority and date filters, a pager and a View link per ticket. There's also a + Create Ticket button. It no longer jumps to SupportTickets.aspx.
3. **Activity Log / Login Activity History** (`view=history`) renders in the same shell, with the same header card and tabs. The old standalone history page was removed, and unknown views fall back to Subscriber Details.
4. **Header card stays fixed.** Every tab, including the business-edit error path, renders `Details` + the tabbed card. Only the tab body changes.
5. **EmployeeProfile address alignment.** The orphan `); the design uses normal case.` text in `css/AnyminuteAddressLookup.css` (~L107) lost its comment opener, which turned the following label rule into an invalid selector. The comment is repaired, and a scoped `#amEmpAddress` block lays out full-width stacked cards: the search row, info/results in normal flow (not floating), and the details grid, with breakpoints at 900 and 560 px. The CSS checks with 0 errors in css-tree. The `?v=` is now `20261102v51` on EmployeeProfile, EmployeeEditor, SupportResellers and SupportSubscribers.
6. **Subscriber and Reseller lists.** The Actions column has a **View** button and the next column has the ⋮ menu only. "View Subscriber Details" and "View Reseller Details" were removed from the menus.
7. **Edit Subscriber › Address** uses the same Address Lookup and Address Details module as Edit Reseller (`AnyminuteSupportAddress.Render`, postal/ZIP search via `SupportSubscribers.aspx?lookup=1`). It loads from and saves to the subscriber's existing EmpAddress row (via AddEmployee.ADDRESSID), using the same column mapping as Add Subscriber. The address goes through the change summary and audit like the other fields. "Please Select" placeholders are no longer shown as address text. If nothing in the form is touched, nothing is written.
8. **SupportTickets list.** The Actions column has a **View** button that opens the ticket details panel. The next column has the ⋮ menu only, and "View Ticket Details" was removed from it.
9. **Report branding (B+).** PDF and Excel title bands and the PDF footer no longer hard-code "ANYMINUTE". They show `COMPANY | REPORT NAME`, where the company is chosen as follows:
   - **Business reports:** the business reported on.
   - **Support Admin and Reseller reports:** the single reseller, business or company when every row belongs to one of them (for example, filtered to one reseller). Otherwise it uses the signed-in user's own organisation (MotherOrg.MotherOrgName).
   - If no company name is found, the title is just the report name.

   Download file names were not changed.

CSS `AnyminuteSupport.css?v=20261102v51` (adds the `.sa-tabpad` body padding for the in-shell tabs) was bumped on all 9 pages that use it.

## Decisions to confirm
- The v50 standalone Login & Access dialog page now renders inside the Subscriber Details shell, following the newer PPT.
- Opening one ticket from Support History still goes to the SupportTickets details panel, where reply, assign and status actions live.
- For platform-wide Support Admin reports (for example, Address Verification across all subscribers), the brand is the signed-in user's organisation (MotherOrg). If you want a fixed company name instead, that needs a setting (Web.config is untouched by rule).

## Deferred (not implemented, per notes)
Stripe, Industry column data, Add Business creation, Max Projects storage, Address Verification Date, finer report filters.

## Checks
- `mcs` App_Code build succeeded.
- These pages compile against App_Code: SupportSubscribers, SupportResellers, SupportTickets, SupportReports, EmployeeProfile, Reports.
- css-tree reports 0 errors for AnyminuteAddressLookup.css and AnyminuteSupport.css.
- Not live-tested (no smoke run, by rule).
