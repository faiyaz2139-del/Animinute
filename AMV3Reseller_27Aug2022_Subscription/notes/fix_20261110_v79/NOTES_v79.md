# AnyMinute v79 (stamp v79-20261110) - Support Admin > Add Subscriber > Confirm & Send Invitation

## Root cause (found in code; matches both screenshots)
SupportSubscribers.aspx.cs AddPost (addsave), after the subscriber was created:
`using (var tx = c.BeginTransaction()) { AD.QueuePending(c, tx, adr, AD.ReviewReady(c), ...) }`
`AD.ReviewReady(c)` runs a query on `c` with no transaction while `c` has a pending local transaction ->
`InvalidOperationException` ("ExecuteReader requires the command to have a transaction..."). The catch only caught
`SqlException`, so it went to ErrorPages/index.aspx?aspxerrorpath=/SupportSubscribers.aspx. Happens whenever an Address is
entered on Add Subscriber. By then UserAcc/AddEmployee/PlanHistory were already written (and no invitation sent), so the retry
said "This email address is already registered to another AnyMinute account." (thakorfawaz@gmail.com most likely exists now as
a Pending Activation subscriber - run check_email.sql to confirm).

## Fixes
1. ReviewReady read before BeginTransaction; address-queue / reseller-assign failures are caught (all exceptions) and logged;
   the created subscriber is kept.
2. AnyminuteSubscriberCreate.Create: legacy SPs use their own connections (one SQL transaction would need MSDTC), so a
   compensating rollback removes exactly the rows THIS attempt wrote (MappingTable, LoginSecurity, UserAcc, PlanHistory
   TransId='SUPPORT', AssginBussiness, EmpAddress, AddEmployee) if any step fails; audit rows are marked "rolled back".
   Non-numeric UserId from Sp_InsertUserAccFromAddress is now a handled error.
3. Invitation email failure / IsSendMail off: no crash; flash "Subscriber created but the invitation email could not be sent
   — use Resend Invitation. (reason)" + logged.
4. Whole Add Subscriber postback wrapped: any exception is logged and shown in-page (form kept).
5. Duplicate email: trimmed, case-insensitive (UserAcc). If the owner is a Pending Activation subscriber, the message names
   SUB-xxx and links to Login & Access to use Resend Invitation. Nothing is deleted automatically.
6. Double submit: one-time token (Session) on the confirm dialog; a repeat Confirm goes to the already-created subscriber
   instead of creating/validating again. JS ignores a second click/Enter after Confirm.
7. Address source (lookup/manual) now carried through the confirm dialog for the verification note.
8. v78 duplicate-address check (Address Verification Add/Import only) is NOT used by Add Subscriber; QueuePending reuses an
   existing pending/verified address, so a subscriber's address is never blocked.

## Files
App_Code/AnyminuteSubscriberCreate.cs, SupportSubscribers.aspx.cs, SupportSubscribers.aspx (js ?v), Scripts/AnyminuteSupport.js,
Main.master, Modern.master, MasterWithoutMenu.master (stamp + ?v).

## SQL (not run; no DB access)
check_email.sql - read-only SELECTs for thakorfawaz@gmail.com + commented-out orphan cleanup.

## Not verified
No live/browser test (box cannot run the app). After deploy: run check_email.sql; if the Pending subscriber exists, open it and
Resend Invitation (or use the optional cleanup), then try Add Subscriber with an address on a new email.
