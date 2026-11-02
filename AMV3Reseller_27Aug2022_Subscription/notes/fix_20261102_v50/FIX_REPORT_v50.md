# FIX REPORT v50-20261102 — PPT Login & Access / Password Reset / Login Activity History

Base: v49 (e468781). Cumulative overlay. Only SupportSubscribers.aspx.cs + master meta changed.

## Fixed (to PPT mocks)
### Slide 1 — Manage Subscriber Login & Access (view=access)
- Removed the subscriber tab strip (mock is a standalone dialog).
- Header bar now matches mock: avatar, name, SUB-ID, email | Current Plan + View Plan Details | Subscriber Since | Active badge (phone and Last Login removed from this header).
- Active Sessions reads "1 (Current session)" when one session is active.
- Cancel returns to Subscriber Details (was the Subscribers list).
- Existing (unchanged, already matched): Current Login Information rows, Enable/Disable radios, orange warning, required Reason * with 0/500, three action cards, Save Changes.
### Slide 2 — Send Password Reset (view=reset)
- "Send to Email Address" is now editable (validated email; blank = subscriber email); the Step 3 bullet updates as you type.
- Preview / email body / bullets copy matched to mock ("This link will expire in 24 hours."; bullet "The link will expire in 24 hours.").
- Link is still one-time-use 24h token (behaviour unchanged, only copy trimmed to mock).
### Slide 3 — Subscriber Login Activity History (view=history)
- Header: name + Active badge; "Subscriber ID | Plan | email | phone"; Subscription Since; Last Login.
- Event Type dropdown always lists the mock types (Sign In, Password Reset, Failed Sign In, Activation Email, Account Created, Login Access Enabled) plus any others logged.
- "Subscriber Created" audit rows display/filter as "Account Created".
- Event icons in the Event Type column.
- Performed By: "First Last (Subscriber)" or "Support Admin (admin name)" as on the mock.
- Existing (unchanged): tabs incl. Login Activity History selected, filters Date Range/Event/Status/IP/Device + Search/Clear, 8 columns, "Showing 1 to 10 of N activities".

## Deferred (unchanged)
Address Verification Date, Industry, Add Business, Max Projects, Stripe; v49 finer report mock filters.

## DB / config
- No Web.config change, no new schema.
- Password Reset still requires DB_REQUIRED/REQUIRED_08_SupportAdminStrict.sql on the host (AnyminutePasswordReset token table); without it the page refuses to send. IsSendMail must be true.

## Checks
- App_Code mcs build OK; SupportSubscribers.aspx.cs page compile OK; inline oninput JS parsed with node.
- No live smoke (per instructions).
