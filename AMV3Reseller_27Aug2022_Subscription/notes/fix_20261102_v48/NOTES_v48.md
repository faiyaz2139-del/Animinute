# AnyMinute v48-20261102: strict re-check of the NOTES_v46 / NOTES_v47 items

Base v47 (833916c). There are no DB or Web.config changes. Deploy the site/ files over v47, then recycle the app pool. This is a local build: nothing was pushed and there was no live smoke test.

## Still open against the notes, now fixed
1. **NOTES_v46 #8, Employee Profile address (deckP s1/s2/s4).** Four checks in EmployeeProfile.aspx were still rejecting US ZIP codes or blocking Save with a pop-up:
   - the "Please enter 6 digit postal code to search" pop-up when leaving the field
   - the 6-digit swal checks on Save
   - Search Postal being required to Save, which blocked Enter Address Manually
   - Suite Number being required although its label says "(Optional)"

   These are removed. On Save, Postal / ZIP now accepts a Canadian postal code (A1A 1A1) or a US ZIP code (12345 / 12345-6789). An invalid code gets a red border on the field (aria-invalid) instead of a pop-up. This uses css/AnyminuteAddressLookup.css?v=20261102v48.
2. **NOTES_v46 #4 / deferred, deckR "change all report similar … across all module".** The old report pages still used the old layout and were reachable from DB menu links: Company_Report, Daily_Report, Employee_Report, Project_Report, Report3 and My_Time_Report.
   - When the signed-in user has the Reports hub, a GET to one of these pages now opens the matching hub report (Reports.aspx, the shared AnyminuteReportLayout).
   - Mapping: Project_Report goes to project-cost; the others go to time-card.
   - Users without hub reports still get the old page. `?legacy=1` keeps each old page reachable. Postbacks are never redirected.
   - The helper is `AnyminuteReportLayout.LegacyRedirect`.
3. **NOTES_v47 deferred, legacy approve grid (`SuperAdmin_Approve_Subscriber.aspx?legacy=1`).** A subscriber row whose first sign-in is pending (UserAcc.Status=1, Is_First_Time_Login=1) now shows an amber "Pending" pill instead of "Active". A row with login disabled (AnyminuteLoginSecurity) shows "Suspended". These are the same rules as SupportSubscribers. The grid's Status filter picks up the new values automatically.
4. **NOTES_v47 deferred, Address Verification "Not linked".** Submitted By now also matches, from existing tables only:
   - a saved business address (BusinessAddress via AddBusiness.ADDRESSID), shown as "Name (Business)"
   - a reseller's stored address (AnyminuteReseller.Address), shown as "Name (Reseller)"

   "Not linked" now appears only when no subscriber, employee, business or reseller uses the address.

## Deferred, with reasons
- **Address Verification Date "—".** AddressMaster has no date column. Only rows created after REQUIRED_08 have AnyminuteAddressReview.SubmittedAtUTC, so older rows have no submission date anywhere. This is data-only; fixing it needs a schema or backfill.
- **Pre-v46 resellers with no stored address.** This is data-only.
- **Industry column, Support + Add Business, Max Projects storage, Stripe sync.** These are blocked (DB, schema or Stripe), as in NOTES_v43.
- **Not live-verified.**
