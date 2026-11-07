# v69 — SHIPPED 2026-11-07 (America/Toronto)
Authorized t204u Fix all, clarified global wipe keeping logins, plans and billing.
Code fd4636f; build v69-20261107.

Support Admin > Subscribers: red Erase All Subscribers' Data next to Clean Test Data.
Requires ERASE ALL, a reason, confirm dialog and a single-use session token. Server checks Support Admin authorization. Paid/inactive subscribers included; protected admin/reseller accounts skipped. One transaction per subscriber; result row counts and failures, start/completion/per-subscriber audits.

Erases v59-scoped business/location, employee/assignment/rate, project, time-card/timesheet/break/week, schedule/approval/activity/notification, report/permission, ticket and pending address-verification data. Payroll/reports lose underlying scoped time/rate data. Global mode includes shared-owner businesses and scoped accountant/payroll invitations.
Keeps ALL UserAcc logins and security/session records, login-linked employee/address profile shells, subscriber owner mappings, plans/subscriptions/PlanHistory/billing/Stripe, resellers/support admins, audit/history archives and shared master data. Non-login employee records removed. Existing per-subscriber/clean-test behavior unchanged. This is not an archive purge; custom tables outside v59 scope are not included.

Deploy cumulative zip site/ overlay. No new SQL needed; prior support audit/login-security and module schema prerequisites still apply.
No live DB access, wipe, deployment, CloudAgent or GitHub push performed.
Back up database, use maintenance downtime, test restored staging DB first. No undo except restore. Separate subscriber commits mean partial success is possible; inspect failures and retry. Avoid concurrent global runs. Large databases can exceed proxy/IIS timeouts despite a 3600-second script timeout.

Verification: full recursive App_Code mcs compile and page-stub code-behind compiles (SupportSubscribers, Main.master, Modern.master) all exit 0; warning details in COMPILE.txt. No live SQL/browser test. Python zip CRC and every unchanged v68 entry byte-verified. Four replacements only. CSS unchanged.
Pre-existing Jquery/build deletions untouched and excluded.
Files: App_Code/AnyminuteSubscriberErase.cs, SupportSubscribers.aspx.cs, Main.master, Modern.master.

Zip: /workspace/anymin/AnyMinute_Fix_20261107_v69.zip
SHA256: cd4861b09ccbc03dd1cc7d90c22833f7d727b910796f85df24b8505d4bbad584
