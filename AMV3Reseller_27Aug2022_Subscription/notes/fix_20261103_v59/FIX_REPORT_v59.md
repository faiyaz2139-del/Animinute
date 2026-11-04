# AnyMinute Fix v59 — 2026-11-03 (Erase data from all modules, t188u/t189u "Fix all with erased data")
Base: v58 `2680c4f` (notes `ae5bb30`) → code commit `a499b1d` · meta `v59-20261103` · cache-bust `?v=20261103v59`. No DB script and no Web.config change.

## What shipped — Support Admin › Subscriber Details › Erase All Module Data (HIGH)
- **Entry points (Support Admin only; hidden for Resellers):** a red **Erase All Data** button in the Subscriber Details header, and **Erase All Module Data** in the Subscriber Actions ⋮ menu (list + details).
- **Confirm modal** (`view=erase`) over Subscriber Details. It shows the per-module record counts (preview), what is kept, a **mandatory reason**, a **typed confirmation** (`ERASE` or the subscriber's name), and an "I understand this cannot be undone" tick. The server checks all three again; if any is missing, nothing is erased.
- **Scope: one subscriber.** It covers the businesses the subscriber owns, using the same owner rule as the Businesses tab. A business that another owner also owns is **skipped** and reported.
- **Hard delete in ONE transaction** (`App_Code/AnyminuteSubscriberErase.cs`); any error rolls everything back. Each table is checked with HasColumn first, so missing optional tables are skipped.
  - Time & attendance: TimeSheetEntry, TimeSheet, BreakTime, TimeCardStatus, AnyminuteWeek
  - Activity, approvals, schedules, permissions: AnyminuteActivity, AnyminuteApprovalAudit, AnyminuteBusinessPolicy, AnyminuteClockOutEstimate, AnyminuteNotification, AnyminuteWeeklySchedule, AnyminuteEmployeeRoleHistory, EmpType_Change_History, AssgineDelegates, AssgineManager, AnyminuteAssignmentDetail/Role, AnyminuteReportAccess, AnyminuteAccessInviteBusiness, User_Role_Permission
  - Projects: ASSIGNPROJECT, AnyminuteProjectAssignment(+Log), AnyminuteProjectDetail, AddProject
  - Tickets (the businesses, erased employees, and the subscriber's own email): Ticket_Comments, AnyminuteTicketMeta, Ticket
  - Employees: the `DeleteAllDetails` SP pattern, run inline in the transaction, covers RateDetails, EmpAddress, AssginBussiness, MappingTable, ASSIGNPROJECT and User_Role_Permission, plus EmpImage, Emp_Active_Inactive_Status and AddEmployee. This applies only to employees who work **solely** for this subscriber. Employees who also work for another business are **only unlinked** from these businesses.
  - Employee logins (erased employees only; never SupportAdmin or subscriber logins): AnyminuteLoginSecurity, AnyminuteActiveSession, AnyminutePasswordResetToken, ExternalLogins, EmailChangeRequests, MappingTable, UserAcc
  - Addresses: BusinessAddress of the erased businesses, plus their **unverified** address-verification rows (AnyminuteAddressReview + AddressMaster with Verifed=0, submitted as "<Business> (Business)")
  - Businesses: AddBusiness
- **Kept:**
  - The subscriber account shell: UserAcc login, owner AddEmployee + EmpAddress, one owner AssginBussiness row and one owner MappingTable row (BusinessID 0, as Add Subscriber writes). The subscriber still appears in the list and can sign in.
  - PlanHistory / billing / Stripe and AnyminuteSubscriptionPlan
  - AnyminuteSupportAudit
  - `*_Archive` tables
  - Shared master data (Verified AddressMaster, City/Province)
  - No DROP or TRUNCATE.
- **Usage/limits reset:** the Businesses / Employees / Projects tiles are live counts, so they show 0 / limit after the erase. The plan limits themselves are unchanged.
- **Audit:** an AnyminuteSupportAudit row is written: Entity `Login`, key UserId, Action **"All Module Data Erased"**, NewValue = per-module row counts (+ any skipped businesses), Comments = reason. It shows in the subscriber's Login Activity History. A failure is written to log4net.

## Files
- NEW `App_Code/AnyminuteSubscriberErase.cs` (Preview + Erase)
- `SupportSubscribers.aspx.cs` (button, menu item, `view=erase` modal, `sa_op=erase` handler, trash icon)
- `css/AnyminuteSupport.css` (`.sa-erase59`)
- `?v=20261103v59` on the 14 pages that had v58; `v59-20261103` meta in Modern.master + Main.master

## Checklist
| Ask | Status |
|---|---|
| Erase data from all modules (t188u) | Done: per subscriber, every module table listed above |
| Support Admin portal | Done (Reseller gets no button/menu; server rejects op=erase from a reseller) |
| Typed confirm + modal | Done (ERASE or name + reason + ack) |
| Hard delete, no drops, no Stripe/billing | Done |
| Audit log | Done |
| Keep subscriber login shell | Done |

## Build
- `cc.sh` App_Code (mono mcs, langversion 5): **Compilation succeeded** (1 pre-existing warning).
- `pc.sh` page compiles all succeeded: SupportSubscribers, Modern.master, Main.master, SupportAdminProfile.
- Not run: live smoke, SQL Server execution of the erase, git push (local executor only).

## Risks
- **Irreversible.** Test on a staging copy or a test subscriber first, and take a DB backup before using it on production data.
- SQL is untested against a live SQL Server. It is written against the 10/23 schema dump plus the DB_REQUIRED/OPTIONAL scripts. Temp tables are created in a parameterless batch so they survive into the parameterised batches (sp_executesql scope).
- Large subscribers: each statement has a 300 s timeout and everything runs in one transaction, so a big erase can lock the timesheet tables briefly.
- Ticket attachment files and employee image files on disk are not deleted; only their DB rows are.
- Shared employees keep their AddEmployee record and login. Only their links to the erased businesses are removed.
- The subscriber's own profile address and its verification status are kept with the account shell.
- Tickets are matched by the subscriber's email, so any ticket raised from that email is erased.
