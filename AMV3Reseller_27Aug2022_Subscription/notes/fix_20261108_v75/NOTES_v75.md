# AnyMinute v75 — remove destructive UI; standalone clean-test SQL
2026-11-08. Base v74 8ae0526. Code commit 31f8bc4f33d2bc07f3c42c0d2a30840ee3c29b2b.
Stamp v75-20261108; SupportSubscribers stylesheet/script cache-bust 20261108v75.

## Application changes
Removed Clean Test Data, Erase All Subscribers' Data, Wipe Everything and per-subscriber Erase All Data
buttons/links from the list, Details and More menus. Removed their panels/modals and server handler
implementations, not just CSS-hidden controls. Old view/operation names beginning erase/clean/wipe return
HTTP 403 before lookup/data-access/dispatch, including old crafted posts. Old confirmation tokens are removed
when a blocked request is encountered. Password reset, normal CRUD, invitations and legitimate account actions remain.
AnyminuteSubscriberErase, AnyminuteEverythingWipe and AnyminuteTestDataClean remain compiled and byte-identical,
but no web code-behind calls them. Original v72 SQL remains unchanged for release-history integrity.
v74 styling is otherwise untouched.

## Standalone wipe for testing
Use THIS version: notes/fix_20261108_v75/wipe_everything.sql (copied/extended from v72).
BACK UP FIRST: take and VERIFY a fresh backup. Deliberately set the actor, name, nonblank reason and
WIPE EVERYTHING confirmation in the script header. Defaults refuse execution. Manual execution requires
sufficient DELETE/update, metadata, DBCC CHECKIDENT and audit permissions in the intended application database.

The reviewed 62-table target list covers every table referenced by the existing per-subscriber erase and wipe
implementation, plus known login/access/audit dependencies and archives. Audit compares 77 distinct CREATE TABLE
definitions in the supplied schema snapshot plus DB_REQUIRED/DB_OPTIONAL migrations. Every definition is classified.
See COVERAGE.md, tables.txt and retained_master_tables.txt. No additional unclassified module-data table was found
in those sources. Legacy Role_AssginedLink/AssginLink/AnyminuteAccessLog are already optional wipe targets;
Role_DomainMaster is shared role/menu configuration, explicitly retained.

At runtime, ANY user table not explicitly classified causes a fail-closed rollback BEFORE deletes. The script
lists its schema/name so the installed-schema difference can be reviewed, rather than silently leaving data
in an unrecognized module. This includes extra schemas/infrastructure tables: classify intentionally before
rerunning. Metadata must be visible to the executing database administrator.

Keeps SupportAdmin / Support Admin / SuperAdmin / Support UserAcc logins, their minimal profile/mapping/
authentication records and the separate SupportAdmin table. Clears all support address/business/rate links.
Keeps subscription plan DEFINITIONS (former reseller/editor links cleared), shared configuration/menu/role/
geography definitions and Support audit history. Existing audit history may contain historical business details.
Deletes local subscribers, resellers, business/employee/project/time/schedule/access/ticket records, archives,
pending/verified/rejected addresses and local PlanHistory/Email_Payment_Link billing data. No separate local
invoice table was found. No remote Stripe subscription, physical-upload, backup or other database is altered.

Transaction + XACT_ABORT + application/table locks + runtime FK ordering; no constraints disabled or tables
truncated. Unknown dependencies/cycles/trigger-regenerated rows fail/roll back. Counts and reseed results are
written to the audit record. Per-table deleted counts are returned in a result grid AND printed after commit.

## Identity reseeding safety
Uses DBCC CHECKIDENT(RESEED, original_seed - increment) ONLY for integral identity tables that this run
actually emptied (DeletedRows > 0), with no retained rows, initialized identity history and in-range reseed value.
Typical IDENTITY(1,1): reseed 0, next insert 1. Non-default/negative increments use the original seed/increment.
- Protected rows remain: do not reset.
- Never initialized / last_value NULL: already pristine; leave the original seed alone.
- Already empty before this run, but last_value non-NULL: conservatively SKIP because prior manual reseed versus
  DELETE/TRUNCATE lifecycle cannot be proven safely from metadata alone. Report the skip for DBA review.
- Non-integral/nonstandard identities or out-of-range seed-minus-increment: SKIP and report.
Plan/master/audit identity sequences are never reset. Reseeding runs inside the same transaction; errors roll back.

## Verification and limitations
All App_Code plus SupportSubscribers and all three masters compile via /opt/monoenv mcs: ZERO errors.
Existing warnings: App_Code 102, Subscribers 2, Main 7, MasterWithoutMenu 5, Modern 0.
Offline reflection tests reject 10 former/case/whitespace reset operations; legitimate operations remain allowed.
Eight pure safe-identity branch/arithmetic checks passed. They are NOT SQL runtime tests.
Static audit confirms no web callers, complete known-schema classification, and unchanged erase/wipe/clean classes.
SQL was reviewed ONLY statically. NO database connection, SQL execution or data wipe occurred in this task.
No live-site access, CloudAgent or push. Installed-schema/FK/trigger/DBCC behavior needs backed-up staging validation.

## Deployment / operation distinction
Deploy the cumulative site/ overlay normally; no schema migration is required for deployment.
The requested data cleaning is a SEPARATE, DESTRUCTIVE manual execution of the v75 script by the parent/DBA.
The executor has NOT cleaned the live database. Do not mistake deploying this zip for wiping data.

## Code files changed
SupportSubscribers.aspx.cs, SupportSubscribers.aspx, Main.master, Modern.master, MasterWithoutMenu.master.
Additional release files: wipe_everything.sql, COVERAGE.md, tables.txt, retained_master_tables.txt,
TESTS.txt, guard_tests.cs, COMPILE.txt, CHANGES_v75.diff and checksum.
