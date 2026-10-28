# AnyMinute v26 Fix Report (2026-10-28): Support Admin strict deck pass

v26 builds on v25 (f0a6107). It keeps every fix from v18 to v25 and closes the gaps between the v25 Support Admin console and Shamsu's decks.

## Deploying v26
1. Copy the whole `site/` folder over the live site, **including App_Code/ and Global.asax**. Web.config is not shipped.
2. Run **DB_REQUIRED/REQUIRED_08_SupportAdminStrict.sql**. Run REQUIRED_07 first if it has not been run yet.
3. Recycle the app pool.
- **Deploy check:** the page source shows `<meta name="anyminute-build" content="v26-20261028" />`.
- **Cache bust:** `css/AnyminuteSupport.css` and `Scripts/AnyminuteSupport.js` changed, so the 4 Support pages now load them with `?v=20261028v26`. No other CSS or JS changed.
- **Global.asax is new in this package.** It is the repository copy with one handler added (`Application_PostAcquireRequestState`, which calls `AnyminuteSessionGuard.Check`). If the live Global.asax differs from the repository copy, add only that handler to it instead of overwriting the file.
- **REQUIRED_08 is additive and idempotent:**
  - New nullable columns `AnyminuteLoginSecurity.SessionsRevokedAtUTC` and `AnyminuteSupportAudit.Location`.
  - New tables `AnyminuteActiveSession` and `AnyminutePasswordResetToken` (stores only the SHA-256 of each token).
  - An AFTER INSERT trigger on `AddressMaster` that records the submitted date and the original values in `AnyminuteAddressReview`.
  - No existing rows, columns or procedures are changed.
  - Until the script has been run, the new features turn themselves off: no session checks, and the reset email is refused with a notice naming REQUIRED_08.

## Gaps closed
1. **Address Verification: Verify and submitter/date.**
   - "Verify Address" runs the format check, then calls `SupportAddresses.aspx?verify=1`. That endpoint compares the address with the existing lookup data (verified `AddressMaster` rows with the same postal code, plus subscriber and employee addresses).
   - It returns one of four results:
     - **match:** the postal code and street match.
     - **partial:** shown with "Use: …" suggestion buttons that fill the fields and verify again.
     - **new:** the postal code is not in the lookup.
     - **error:** this result blocks Approve.
   - Approve re-runs the check on the server and appends `[Verify: …]` to the notes. City and Province keep their ID values in the master tables.
   - Names are now resolved in the list and the editor; missing values show as "Not linked" or "—". "Submitted By" comes from the review row or from the linked subscriber or employee.
   - "Submitted Date" is recorded for every new address (trigger). Addresses created before v26 show "Not recorded (added before v26)".
2. **Login Activity: Location.**
   - Private and loopback IPs are labelled with no network call.
   - Public IPs are looked up only when the appSetting `GeoIpLookupUrl` is set (for example `https://ipapi.co/{ip}/json/`). The lookup has a 1.5 s timeout and caches results for 24 h.
   - Each resolved location is stored in `AnyminuteSupportAudit.Location`. When the setting is missing, the column shows "Not resolved" (the tooltip names the setting).
3. **Disable Login Access ends live sessions.**
   - Disabling a user stamps `SessionsRevokedAtUTC` and closes their active-session rows in the same transaction.
   - On the user's next request (checked from cache at once, and from the DB at most every 60 s), the session is cleared and they are sent to `Login.aspx?revoked=1`, which explains why.
   - The audit records how many sessions were revoked. "Active Sessions" on Login & Access is now a real count.
4. **24-hour one-time reset link.**
   - "Send Password Reset Link" issues a random token, valid for 24 hours and usable once. Earlier unused tokens for that user are invalidated.
   - `ResetPassord.aspx?…&rt=` shows the new-password form only for a valid token. It re-checks the token on submit and then marks it used ('Password Changed' audit).
   - The old reset flow without `rt` (invitations) is unchanged.
5. **Plan catalogue drives the limits.**
   - Saving a plan whose Max Businesses or Max Employees changed offers "Apply the new employee / business limits to the N active subscription(s) on this plan" (checked by default).
   - That option updates `PlanHistory.No_Of_Users/Business` in the same transaction and records the row count in the audit.
   - New Basic sign-ups (`Register_As_A_Bo`) take their limits from the catalogue.
   - Usage & Limits now matches plan names regardless of case or a " PLAN" suffix, and shows "Limits applied: x of y".
6. **Subscribers: Businesses, Employees, Projects and Billing tabs** in deck order, each with a count. The lists are read-only with search and pages. Billing shows the plan history, usage against limits and catalogue Max Projects.
7. **Create Ticket on behalf of a subscriber** (`SupportTickets.aspx?create=1`):
   - Fields: subscriber, subject, category, priority, assignee, status and description.
   - It writes the Ticket (`TK-<id>`), the first comment ("created by …, Support Admin"), the ticket meta row and a "Created" audit row.
8. **Navigation for the SupportAdmin role.** The deck menu comes first. Legacy pages the deck replaces are hidden: Ticket_List, Raise_Ticket_By_User, SearchAddress, PostalCode, SuperAdmin and LoginAccess. The remaining legacy tools appear under "More tools". Header Help → Tickets opens SupportTickets. **SuperAdmin navigation is unchanged.**

## Known limits (please read)
- **Location** needs `GeoIpLookupUrl` in Web.config (not shipped). Pick the provider and check its terms and quota.
- **Stripe** prices and products are not linked. Plan edits change AnyMinute limits only.
- **Max Projects and Trial Days** are stored in the catalogue only. No AnyMinute page enforces them yet.
- **Basic sign-up limit:** the seeded catalogue Basic plan is 10 employees / 1 business, so new Basic sign-ups now get 10 employees instead of the hard-coded 5. Edit the Basic plan in Subscription Plans if 5 is wanted.
- **Session revocation** relies on the web server and DB clocks agreeing (UTC). Sessions that started before v26 are tracked from their first request after deploy.
- **Pre-existing build note (not new in v26):** `Register_As_A_Bo.aspx.cs` line 638 uses `DateTimeOffset.ToUnixTimeSeconds`, which requires .NET Framework 4.6 or later on the server.

## Verification
- App_Code and every changed page and master compile with mcs at `-langversion:5`. Exception: the pre-existing 4.6 API above, which fails only against the 4.5 reference profile.
- `AnyminuteSupport.js` passes a `node --check` syntax check.
- No live DB or browser test was possible from the build box. Please smoke-test on staging after running REQUIRED_08.
