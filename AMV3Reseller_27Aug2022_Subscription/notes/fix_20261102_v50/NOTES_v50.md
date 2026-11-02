# NOTES v50 — PPT Nov 2, 2026 (Login & Access)

Source: NOTES.pptx (3 image slides). Match UI strictly to mocks.

## Slide 1 — Manage Subscriber Login & Access (modal)
- Title: Manage Subscriber Login & Access
- Breadcrumb: Subscriber Details > Login & Access
- Header bar: initials avatar, name, SUB-ID, email, Current Plan + View Plan Details, Subscriber Since, Active badge
- Left: Current Login Information — Account Status, Login Access, Invitation Status, Last Login, Last Password Reset, Active Sessions
- Right: Change Login Access — radio Enable / Disable Login Access; orange warning when disable; required Reason for Access Change (0/500)
- Additional Account Actions cards: Resend Invitation, Send Password Reset, View Login History
- Footer: Cancel, Save Changes

## Slide 2 — Send Password Reset (modal)
- Steps: 1 Subscriber Information (name, id, email, Account Status, Login Access, Last Login)
- 2 Password Reset Email: Send to Email Address (editable), Message Preview (subject + body with Reset Your Password link, 24h expiry)
- 3 Confirm and Send: yellow bullets (link sent, 24h expiry, previous password invalid after reset, activity log)
- Footer: Cancel, Send Password Reset

## Slide 3 — Subscriber Login Activity History (Support Admin page/tab)
- Subscriber header: name+Active, SUB-ID, Plan, email, phone, Subscription Since, Last Login
- Tabs include Login Activity History (selected) among Profile, Businesses, Employees, Projects, Subscription & Billing, Login & Access, Support History
- Filters: Date Range, Event Type, Status, IP Address, Device; Search + Clear
- Table columns: Date & Time, Event Type, Description, IP Address, Location, Device/Browser, Performed By, Status
- Event types shown: Sign In, Password Reset, Failed Sign In, Activation Email, Account Created, Login Access Enabled
- Pagination: Showing 1 to 10 of N

## Scope / constraints
- Cumulative overlay on v49; meta v50-20261102; zip AnyMinute_Fix_20261102_v50.zip
- Site folder: AMV3Reseller_27Aug2022_Subscription (ONLY site under src/)
- No CloudAgent; local only; no Web.config/DB schema unless required and documented
- Token-lean: only fix what the 3 slides require; reuse existing Login & Access / password reset / activity if present
- Deferred still: Address Verification Date, Industry, Add Business, Max Projects, Stripe
