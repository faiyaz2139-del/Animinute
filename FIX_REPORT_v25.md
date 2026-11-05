# AnyMinute v25 Fix Report (2026-10-28): Support Admin console (Shamsu decks 1–4)

v25 builds on v24 (b66ab1b) and keeps every fix from v18 to v24. Copy the whole `site/` folder over the live site, **including App_Code/**. Run **DB_REQUIRED/REQUIRED_07_SupportAdmin.sql**, then recycle the app pool.
- **Deploy check:** view the page source. It must contain `<meta name="anyminute-build" content="v25-20261028" />`. Both Modern.master and Main.master now carry it.
- **Cache bust:** only the new files use `?v=20261028v25` (`css/AnyminuteSupport.css`, `Scripts/AnyminuteSupport.js`). No existing CSS or JS changed. Do a hard refresh (Ctrl+F5).
- **DB:** `REQUIRED_07_SupportAdmin.sql` is additive and idempotent. It only creates the 4 tables below and changes no existing tables.
  - `AnyminuteSupportAudit`: one activity history for tickets, addresses, plans and login events.
  - `AnyminuteAddressReview`: keeps the original address for audit, plus the review status and notes.
  - `AnyminuteTicketMeta`: subject, category, priority, assignee and archived flag.
  - `AnyminuteSubscriptionPlan`: seeded with Basic, Standard, Professional, Enterprise (Custom) and Trial.
  - Without the script, the pages open read-only and show a notice naming the missing tables.

Access: every page is limited to `SupportAdmin` and `SuperAdmin` (`Session["UserType"]`). Anyone else gets a 403.

## 1. Support Admin navigation
- For SupportAdmin and SuperAdmin, `AnyminuteUiNavigation.Render` (used by both masters) now puts the deck nav at the top:
  - Dashboard
  - Subscribers
  - Subscription Plans
  - Usage & Limits
  - Support Tickets, with a red badge for open tickets
  - Address Verification, with a red badge for pending addresses
  - Reports and Settings, shown only when the role is permitted.
- The legacy menu groups still render underneath, minus their duplicate Dashboard link, so no existing page loses its link.

## 2. Address Verification: `SupportAddresses.aspx` (deck 1)
- The page shows the 5-step workflow strip and Pending / Verified / Rejected tabs with counts.
- Filters: search, country, province/state, status and submitted date (7/30 days).
- The table has Select, Address, Submitted By, Country, Status and Actions (Edit / Reject).
- **Edit panel:**
  - Shows the **Original Submitted Address** (read-only), above the corrected fields (street, city, province, postal, country).
  - **Verify Address** is a separate step that checks the postal format for CA/US/UK/India.
  - **Approve & Add to Database** stays disabled until the address verifies, and is disabled again by any later edit.
  - Notes have a 500-character counter. Reject requires notes.
  - The panel ends with the activity history.
- **Server side:** the action runs in a serializable transaction.
  - On first action, the original address is copied once into `AnyminuteAddressReview`, so it is preserved for audit.
  - Approve re-checks the address on the server, updates `AddressMaster` and sets `Verifed=1`. The audit row is "Approved" or "Corrected & Approved".
  - Reject sets the status to Rejected and logs the notes.
  - An address that has already been reviewed can't be actioned twice.

## 3. Support Tickets: `SupportTickets.aspx` (deck 2)
- KPI cards: Total, Open, In Progress, Resolved.
- Filters: search, status, priority, category, date range, and "show archived".
- 10 rows per page, with Ticket ID, Subject, Subscriber, Category, Priority, Status, Assigned To, Created, Last Updated and the ⋮ menu.
- **Ticket Actions menu:**
  - View Details: shows the conversation.
  - Edit: subject, category, priority.
  - Assign / Reassign: choose a Support Admin user.
  - Reply: adds a `Ticket_Comments` row. Open tickets move to In Progress, or you can choose "Waiting for Customer".
  - Change Status: a reason is required.
  - Mark as Resolved: the note goes to `DeveloperComment`, and `TicketResolveDate` is set.
  - View Activity History.
  - Archive: only for resolved tickets, and it toggles.
- Every change writes a `Ticket` audit row (previous and new status, comments). The legacy `Ticket.StatusType` codes are kept, so `Ticket_List.aspx` still agrees.
- "Create Ticket" opens the existing `Raise_Ticket_By_User.aspx`.

## 4. Subscription Plans: `SupportPlans.aspx` (deck 3)
- The list shows plan, description, monthly and annual price, limits, trial, status and actions.
- **Add Plan** and **Edit Plan** modals: a blank price or limit means "Custom".
- Saving opens **Confirm Plan Changes**. It shows each changed field (Previous → New) and requires a reason. After confirming, a **Plan Saved** modal offers View Plan Details or Back to Plans.
- Audit action: "Plan Created" or "Plan Updated", with the full previous and new values.
- **Usage & Limits** (`?view=usage`) shows active subscribers per plan, counted from `PlanHistory`.

## 5. Subscriber Login & Access: `SupportSubscribers.aspx` (deck 4)
- The list has a search box. Manage opens the subscriber header (avatar, SUB-ID, plan, since, last login) with these tabs:
  - Profile: opens the existing approve page.
  - Billing & Subscription
  - Login & Access
  - Support History: the subscriber's tickets.
  - Login Activity History
- **Login & Access:**
  - Current access info.
  - Enable / Disable with a required reason. This uses `AnyminuteLoginSecurity.LoginDisabled`, which Login.aspx already enforces. Enabling also sets `UserAcc.Status=1`.
  - Resend Invitation.
  - Send Password Reset.
  - View Login History.
- **Send Password Reset:** a 3-section page (subscriber, delivery, email preview) that sends "Reset Your AnyMinute Password" with the existing reset link. It honours `IsSendMail` and writes a "Password Reset" audit row.
- **Login Activity History:**
  - Filters: date, event, status, IP and device.
  - Columns: Date & Time, Event, Status, IP, Device, Location, Description.
  - Login.aspx now records "Login Success", "Login Failed" (wrong password) and "Login Failed" (access disabled). Recording never blocks a sign-in.

## Built vs stubbed / gaps
- **Address:** `AddressMaster` has no submitter or submitted date. Pending rows show "Manual entry", and date filters only apply to addresses reviewed from v25 on. Verification is a format check, not a postal lookup service.
- **Login:** there is no IP geolocation, so Location shows "—".
  - Active sessions aren't tracked ("Not tracked"). Disabling access stops the next sign-in but does not end a live session.
  - Login history starts at v25 deploy.
- **Password reset:** uses the legacy `ResetPassord` link, which has no 24-hour expiry or one-time token.
- **Plans:** this is a catalogue only. Prices aren't linked to Stripe and limits aren't enforced on sign-up. Existing `Enterprise_Plan`/`PlanHistory` are untouched.
- **Subscriber tabs:** Businesses / Employees / Projects tabs are not built.
- **Create Ticket:** uses the legacy page.
- **Legacy pages:** `Ticket_List`, `SearchAddress` and `Enterprise_Plan` are unchanged and still reachable from the legacy menu groups.

## Checks
- App_Code compiles (mcs, `-langversion:5`, Mono). Code-behinds for `SupportAddresses`, `SupportTickets`, `SupportPlans`, `SupportSubscribers` and `Login` compile against stubs at C# 5.
- Not run against IIS or the live DB. Nothing was deployed.
