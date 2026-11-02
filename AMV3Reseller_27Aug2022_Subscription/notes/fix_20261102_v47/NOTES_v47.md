# AnyMinute v47-20261101 — Nov 2 deeper live smoke (Support Admin) fixes

Base v46 (cab9a6b). No DB changes. No Web.config changes. Deploy the site/ files over v46, then recycle the app pool.

1. **Edit Subscriber left-clip** (Modern.master, css/AnyminuteNavDrawer.css?v=20261101v47). An inline script right after the sidebar pins the Support sidebar before the content paints (`am-nav-init`), so there is no margin animation on load. The content column clips horizontally and no longer scrolls under the fixed sidebar. The hamburger toggle is unchanged.
2. **Edit Subscriber Status** (SupportSubscribers.aspx.cs):
   - The options are Pending Activation / Active / Suspended / Inactive, and the current status is preselected (Pending shows as Pending Activation).
   - Saving writes exactly the chosen state: UserAcc.Status, Is_First_Time_Login, and AnyminuteLoginSecurity.LoginDisabled. Suspended or Inactive revokes sessions.
   - An unknown value keeps the current status and never defaults to Active.
   - The list KPI and the filter include Pending and Suspended.
   - The Pending More menu adds Activate Subscriber (opens Edit with Active preselected; the Confirm + Reason step is still required) and Resend Invitation.
3. **Support Admin landing**: Login and Registration go to SupportSubscribers.aspx. SuperAdmin_Approve_Subscriber.aspx redirects Support Admin there; the old grid is still at `?legacy=1`.
4. **Cleanups**:
   - Subscription & Billing: the PlanHistory/Stripe developer text is removed.
   - Reseller details: shows the Created date plus "by …". An empty Address shows "— Add address".
   - Usage & Limits: "+ Add Plan" is hidden on view=usage. "(billing plan)" is now "Legacy plan (not in Subscription Plans)". The footer is plain-language.

Deferred: the legacy approve grid has no Pending concept (it is reachable only via ?legacy=1). Pre-v46 resellers have no stored address. Address Verification "Not linked" is unchanged. Not live-verified.
