# AnyMinute Fix v45-20261101 (cumulative on v44 cfd2877)

## 1. DeckB Subscriber Management – slides 3, 4, 6 (SupportSubscribers.aspx)
Gaps found vs. slide UI and fixed:
- Slide 3 (List / Details): breadcrumb with home icon "Subscribers > Subscriber Details" (no page title); large avatar,
  "PLAN | Subscriber since"; Edit / Send Password Reset / More buttons with icons (More was a squeezed 34px summary);
  three icon info columns (Email+Phone, Address, Activation+Next Billing); "Subscription Limits (PLAN)" with
  Upgrade Plan + gear and 3 icon tiles with progress bars (bars were not rendering); full-width tab strip;
  Businesses header "Businesses / List of businesses under this subscriber." + filled "+ Add Business" button;
  table Sl. No / Business Name (plain link) / Industry / No. of Employees / No. of Projects (centered) / Status / Actions (Edit + kebab).
  List: plain name link, Businesses/Users as "used / limit" centered, yyyy-MM-dd activation date, SVG kebab/search/refresh icons,
  pager "1 … window … last", no wrapping of names/plans.
- Slide 4: compact edit conflicts with slide 6; slide 6 layout used (same fields). Business row Edit now opens a
  name-only Business Edit (view=bizedit, reason required, audited "Business Updated").
- Slide 6 (Edit + Confirm + Success): header metas (Subscriber ID, Activation, Next Billing) with icons; plan title case;
  Change Plan button; light-blue Current Plan box with inline "View Plan Details"; New Plan select hidden until Change Plan
  ([hidden] was overridden by CSS); tiles with bars; grey read-only Max fields + Manage Custom Limits; full-width Account Status
  with info box; full-width Subscription Dates; Address row with pin + Manage Address; "Recent Activity (Last 5)" + View Full History;
  Cancel / Save Changes (save icon). Confirm Changes is a modal over the edit page (Field/Previous/New, Reason *, 0/500,
  Cancel, Confirm & Save). Success modal over the edit page: "Subscriber Updated Successfully!", info note,
  View Subscriber Details (outline) / Back to Subscriber List (filled).

## 2. Subscriber welcome email now contains the password
Root cause: SendInvitation passed OldPassword = "" so the {OldPassword} line was blank; new accounts all had the default password.
Fix (mirrors reseller welcome): on Add Subscriber a per-account temporary password is generated (AnyminuteResellers.NewTempPassword),
stored MD2-encrypted, audited without the value ("Temporary Password Issued"), put in the email ({OldPassword} + a fallback
block "Email / Temporary password / Sign in" if a template lacks the token), and shown once to Support in the flash.
Resend invitation for Pending accounts issues a fresh temporary password. First login with that password activates the
pending account (Login.aspx.cs -> AnyminuteSubscriberCreate.ActivateOnTempPassword; only when a 'Temporary Password Issued' audit row exists).
v44 reseller login fix kept unchanged.

## Meta / build
Main.master + Modern.master meta v45-20261101; SupportSubscribers.aspx css/js ?v=20261101v45.
App_Code + SupportSubscribers.aspx + Login.aspx compile clean (1 pre-existing warning). No GitHub push, no live smoke.
