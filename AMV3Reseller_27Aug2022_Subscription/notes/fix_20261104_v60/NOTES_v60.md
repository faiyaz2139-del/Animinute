# AnyMinute v60-20261104 (cache-bust ?v=20261104v60)
Source: docx notes t190ua0 (held as notes/docx_20261104_v60/NOTES_held.md, now SHIPPED). Base v59 a499b1d.

## 1. HIGH: Send Password Reset showed "Subscriber not found"
- Cause: on Details view=reset the More menu (MoreMenu withUser=true) emitted a 2nd hidden `sa_user`; Request.Form joined it to "5,5" and Int32.TryParse failed, so Act() looked up subscriber 0.
- Fix: `OwnsUser(view)` (reset/access/bizadd/bizedit/erase) suppresses the More-menu sa_user on views that emit their own. Server: new `SA.FormId(key)` takes the first valid positive id from duplicate/comma-joined values; used in SupportSubscribers (sa_user), SupportAddresses, SupportResellers, SupportTickets (sa_id/sa_user) after an audit of every hidden-id post.
- Latent bug found in audit: view=bizview had NO sa_user (More > Resend Invitation would also say "Subscriber not found"); bizview now gets it from the More menu.

## 2. Reseller portal screens (PPT s3-s9 mock), reseller header/sidebar on every screen (Modern.master brand + NavHtml)
- s3 Subscribers list / s4 Details: already v56; Biz tab headers now #, Business Name, Industry, Employees, Projects, Status, Actions.
- s5 Add Subscriber: 2-column cards (Personal, Contact, Subscription Plan & Limits, Account Activation) + Cancel / Save Subscriber; reseller statuses Pending Activation/Active/Inactive; dates default today..+1y; address left blank (country Canada).
- s5 Edit: Email + Contact Phone; status/dates/address/activity kept in collapsed "More subscriber settings" (no function lost).
- s6 Manage Login & Access, s7 Send Password Reset (with close X), s8 Login Activity History: mock titles + leads inside the tab for resellers; History has no IP/Device filters and drops Location / Performed By for resellers.
- s9 Subscription Plans: "+ Add Plan", lead "Create and manage subscription plans, limits, pricing and features.", View + three-dot (View Plan Details, Edit Plan for own plans, Usage & Limits), pager with per page. Add/Edit form = Support Admin plan fields (name, description, monthly/annual CAD or blank=Custom, billing frequency, limits, trial 0-365, status, features). Plans are reseller-owned via new nullable AnyminuteSubscriptionPlan.ResellerID (DB_OPTIONAL/OPTIONAL_06_ResellerPlans.sql); reseller sees catalogue (NULL) + own plans, and its own plans in Add/Edit Subscriber. Name unique across all plans (SA.PlanKey), read-only on edit. Audit Entity "Plan". Without OPTIONAL_06 the button shows a "database update needed" form, disabled.
