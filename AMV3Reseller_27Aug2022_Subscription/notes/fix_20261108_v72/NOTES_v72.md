# AnyMinute v72 — 2026-11-08

Base: v71 / 230d99b. Code commit: 76fe266ebab7475d54bb7e2a6101a65b06ce1320.
Build: v72-20261108. Support Addresses/Subscribers CSS/JS cache-bust: 20261108v72.
Local executor only. No push, CloudAgent, live-site call, SQL execution or database connection.

## Fixes
1. Add Address: validation/duplicate/database errors appear INSIDE the reopened modal; entered values survive.
   Success commits, closes the modal, reloads the Verified list and displays the success banner.
   Import upload/format/limit errors also appear inside its modal.
2. Import: reports total DATA rows read (headers excluded), added, already verified, duplicates within file,
   blank/invalid (blank count and each validation reason count), and failed. Categories add to the total.
   Database failure rolls back all additions: added=0, failed includes rolled-back additions and unprocessed rows.
   Detailed invalid examples remain capped at 50 but reason totals count every invalid row.
   Whitespace normalization for within-file identity now matches AddVerified.
3. Separate Support Admin-only Wipe Everything action beside Erase All Subscribers' Data.
   Requires exact WIPE EVERYTHING, nonblank reason, browser confirmation and a single-use session token,
   expiring after 15 minutes. Resellers/SuperAdmin cannot invoke the UI; the backend independently checks
   Support Admin and checks the acting database UserAcc is a protected support-role login.
   One transaction, XACT_ABORT, application lock, data-table locks, runtime child-before-parent FK ordering,
   success audit with per-table counts and results table. Missing optional tables report zero.
   Unknown dependencies, FK cycles, or trigger-regenerated leftovers fail/roll back; constraints are NEVER disabled.
   Failure is logged server-side and audited separately when the audit connection is available.
4. v69/per-subscriber hardening: detach nullable rate references before deleting rates; delete rate-change
   reasons before rates; delete profiles/businesses BEFORE their addresses; retain addresses/rates shared
   with remaining profiles/businesses. Clean project-linked rows even with missing BusinessID; remove
   login access/email-change audit dependencies; protect all normalized support roles and shared-login profiles.
   These are identifiable FK/data-risk patterns, NOT a confirmed diagnosis of the 3 failed live subscribers.
   Their exact errors are unavailable; this work deliberately did not contact any database.

## Import bugs found and corrected
- v71 imported ONLY the first worksheet: other worksheets were silently ignored. v72 imports ALL worksheets
  in workbook order, independently maps each header and reports worksheet names. A 2,400 + 2,000 sheet fixture
  now reads 4,400. This proves the bug, not that it caused the user's specific discrepancy (original workbook unavailable).
- Sparse Excel row-number gaps were expanded into invented blank records and could falsely hit the cap.
  v72 counts actual row elements only; missing cells retain their column positions.
- v71 stopped XLSX parsing early after MaxRows+1. v72 explicitly rejects over-limit data before database work,
  including totals across worksheets, with no partial import/truncation.
- v71 accepted headers with only three recognized columns and prefix-matched data such as Cityville.
  v72 uses exact supported header aliases and requires ALL five columns when a header is recognized.
  Headerless files still use template order. Partial recognized headers are rejected clearly.
- Inline strings, rich shared strings, missing cells and absolute/relative worksheet relationships tested.
  No evidence that shared strings themselves stopped v71 early. The 2 MB and 5,000 data-row limits remain explicit.
  Expanded XLSX data is bounded to 50 MB; malformed unclosed CSV quoting is rejected instead of silently accepted.

## Wipe scope / retained data
The explicit 62-table allowlist is in tables.txt and wipe_everything.sql; every optional table is checked for existence.
All rows are erased except protected rows in UserAcc, AddEmployee, MappingTable, AnyminuteLoginSecurity,
AnyminuteActiveSession and ExternalLogins belonging to SupportAdmin / Support Admin / SuperAdmin / Support accounts.
Support employee shells have all address/business/rate/organization links cleared; retained role mappings have no business.
Subscription plan definitions are kept; former reseller ownership/editor links are cleared, so no orphaned reseller references.
The separate SupportAdmin table and existing AnyminuteSupportAudit history are retained.
Shared configuration/lookup masters are retained: AnyminuteSubscriptionPlan, CityMaster, ProvinceMaster, UserType,
AdminUserType, Role_Permission_Master, LinkMaster, DomainMaster, CodeType, Mst_Menu, Mst_Page,
Map_Menu_Page_User, DefaultMotherOrg, EmailSeting, TemplateMaster, TempleteType (as present).
Pending/verified/rejected AddressMaster/AnyminuteAddressReview, EmpAddress, BusinessAddress, StreetMaster and
business/employee/login archives are erased. Local billing is PlanHistory and Email_Payment_Link; no separate
invoice/billing table was found in the checked schema/data layer. No remote Stripe cancellation/API call occurs.
Physical uploaded files are NOT removed; Files metadata is erased. Audit history can contain historical business details.
This is a local database reset, not erasure of backups, audit history, remote services or physical uploads.

## Deployment
Deploy the cumulative zip site/ overlay using the established v71 process. NO NEW SQL migration is needed.
Existing REQUIRED_07/08 support tables must already be installed (same prerequisite as v71).
notes/fix_20261108_v72/wipe_everything.sql is an OPTIONAL, DESTRUCTIVE alternative to clicking the new tool.
It is NOT a deployment migration. DO NOT run it during deployment. Defaults intentionally refuse execution.
To use that alternative later: take/verify backup, review target DB, intentionally set actor/name/reason/confirmation.
SQL Server 2012+ (THROW), as used by the existing datetime2/SYSUTCDATETIME support schema.

## Verification
/opt/monoenv mcs, C# 5/.NET 4.5: all App_Code; SupportAddresses.aspx.cs; SupportSubscribers.aspx.cs;
Main.master.cs and Modern.master.cs compiled with existing generated control stubs: ZERO errors.
Existing warnings: App_Code 102, Subscribers 2, Main 7; Addresses and Modern 0.
13 offline parser regression checks passed; see TESTS.txt and parser_tests.cs.
Standalone SQL checked byte-for-byte against the embedded implementation, plus static guard/transaction/FK/audit checks.
NO SQL/database tests or live smoke were run. FK behavior and the actual installed database schema require
post-deploy verification on a backed-up/staging copy before anyone intentionally uses Wipe Everything.
