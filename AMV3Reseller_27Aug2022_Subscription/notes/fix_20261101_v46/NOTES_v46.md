# AnyMinute v46-20261101: all held notes and decks

Built on v45 (793d67d, meta v45-20261101). Cumulative: everything from v39 to v45 is kept. Work was local only. Nothing was pushed and there was no live smoke test.

## Inputs
- Screenshots shot1–shot4 with the notes "Display at least 10 at one time…", "Reseller list should look like businesses table" and "Full dropdown should be properly visible / Remove toggle and maintain consistency across screens".
- deckR_reports.pptx: "Reports should be same across all Module".
- deckA_address.pptx: "Address module for Reseller and Subscriber". s1 is "Require Address Module" in Add Reseller. s2 is "Address module in Subscriber – Remove the bottom".
- deckP_paused.pptx:
  - s1: Address Not Found card only.
  - s2/s4: Employee should use the Business Address Lookup look, "not the employee one".
  - s3: Employee search finds nothing.

## MUST-FIX and what was done
1. **Reseller list shows 10+ rows; dropdowns sit outside the table and are fully visible** (shots).
   - The Support list page size is 10, with a "10 / 20 / 50 per page" select plus a Subscribers-style footer: Showing x to y of z, pager with ellipsis (`SA.Pager46`).
   - Kebab menus open as fixed-position popovers placed from the button. They flip up near the bottom of the screen, scroll when long, and close on scroll or resize. This applies to every Support screen.
2. **Reseller list looks like the Businesses table.**
   - Both lists share the `sa-list46` table.
   - Resellers columns: Sl. No. / Reseller Name (+RSL id) / Contact Person (+phone) / Email ID / Plan Name / Subscribers / Businesses / Created Date / Status / Actions (kebab only).
   - Businesses columns: Sl. No. / Business Name (+BIZ id) / Owner (+email) / City / Province / Users / Agreement Date / Status / Actions. The kebab has View Business Details, Edit Business and View Subscriber.
3. **Remove the status toggle; keep screens consistent.**
   - The Subscribers list status cell is now a pill only; the switch is gone.
   - The page no longer overflows sideways (`.sa-page` clip, minmax KPI grid).
4. **Reports are the same across all modules** (deckR).
   - The shared AnyminuteReportLayout (cs/css/js) drives the Business Reports hub/preview (Reports.aspx) and the Support Reports page, so both have the same header, filter bar, KPI cards, table and export.
5. **Address module in Add/Edit Reseller** (deckA s1).
   - The free-text address textarea is replaced by the Business Address Lookup module: Country with flag, Postal/ZIP with clear, Search Address, Enter Address Manually, hint, Not Found card, Select Address from Search Results, and the Address Details grid.
   - Required fields are Street No, Line 1, City, Province/State and Postal/ZIP, with CA/US format checks.
   - The address is stored composed in Reseller.Address. Edit parses it back into the fields. The audit log records it.
6. **Address module in Add Subscriber; "remove the bottom"** (deckA s2).
   - The old optional address block at the bottom is replaced by the same module.
   - It is optional, but must be complete once started. It is saved structured to EmpAddress (StreetNo, StreetName, Line 2, Suite type/no, City, Province, Postal, Country).
7. **Address Not Found** (deckP s1).
   - When there is no result, only the neutral "ⓘ Address Not Found" card shows: white, grey border, outlined "i". The hint, the red text and the results list are hidden.
   - Applies to Add Business, Support Reseller/Subscriber, Employee Editor and Employee Profile.
8. **Employee lookup uses the Business look** (deckP s2/s4).
   - EmployeeEditor (Add/Edit Employee → Address) and EmployeeProfile (Address and Contacts) now use the Business lookup layout and the Address Details card.
   - Postback IDs are unchanged, and Save works as before.
   - The swal "Address not recorded previously" / "6 digit postal code" pop-ups are removed. Invalid codes are flagged on the field.
   - US ZIP codes are accepted.
9. **Employee search finds nothing** (deckP s3).
   - Employee searches use the shared `AnyminuteAddressLookup.Search`: AddressMaster plus saved EmpAddress plus BusinessAddress, spaced and compact postal forms, with a fallback to Sp_SearchAddress.

## Deferred
- Legacy report pages (Company_Report, etc.) are not on the new report layout. Only the Business Reports hub and the Support Reports page are.
- Nothing was verified against a live DB or session (no smoke test, as instructed). The Reseller and Subscriber address saves depend on the existing columns (Reseller.Address nvarchar(400), EmpAddress).
