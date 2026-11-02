# AnyMinute notes — Nov 2, 2026 (Pandey docx)

Source: NOTES.docx (0cb4eb62…). Screenshots in media/.

## MUST-FIX
1. **Add Employee postal hint** (EmployeeEditor.aspx / Add Employee address lookup)
   - Change info text from `Enter a valid Canadian postal code (e.g. M1B 2W1).`
   - To: `Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185).`
   - **Do not change** Add Business page (already has the US ZIP wording).

2. **Reports catalog + filters by role**
   - Every report type shown in the notes (Support Ticket Report mock + the multi-report grid) is missing or incomplete for one or more logins: subscriber, employee, supervisor, administrator, reseller, accountant (and Support Admin where those Support/Reseller reports belong).
   - Report list in the Reports hub must show the types that role is allowed to see.
   - When the user clicks a different report type, the filter panel must switch to that report’s filters (as in the mocks) and Generate/Export must use those filters.

### Support Admin report types from notes (left nav + tiles)
Support Admin Reports: Subscriber Master, Subscriber Activity, Subscription Plan, Usage & Limits, Upgrade & Change, Support Ticket, Support Staff Performance, Ticket Audit, Address Verification, Address Correction Audit, Subscriber Login & Access, Support Admin Activity.
Reseller Reports: Reseller Master, Reseller Subscriber, Reseller Business, Reseller Usage & Limits, Reseller Activity, Reseller Performance, Reseller Login & Access, Reseller Audit.

### Business / other-role Reports
Ensure each role’s Reports hub (Reports.aspx / shared layout) exposes that role’s report types and that switching types updates filters. Do not force Support-only reports onto employee/subscriber unless the existing catalog already maps them.

## Out of scope
- Add Business postal hint (already correct per notes).
