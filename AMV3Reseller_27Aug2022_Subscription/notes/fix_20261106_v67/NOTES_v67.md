# NOTES v67-20261106
Cumulative on v66 (ca9ada2). Build stamp v67-20261106; cache-bust ?v=20261106v67 (same files as v66 + Main.master AnyminuteNavDrawer.css). Code commit 8276955. Source: PPT "Fix all" (5 slides, Reseller portal, Soft Services LLC login).

## s1 Account menu (all Reseller pages)
Modern.master.cs: reseller AccountAvatar = initials circle only (company name text removed; name kept as tooltip). One chevron (span chevron forced visible; the name::after chevron is gone with the name). Main.master reseller pages already used avatar + chevron.

## s2 Help (?) icon (all Reseller pages)
Modern.master.cs no longer blanks HeaderHelp for resellers (Dashboard, Subscribers, Add/Edit Subscriber, Businesses, Plans, Usage, Reports, Settings, Profile). CSS (AnyminuteNavDrawer.css) places it left of the avatar like My Profile (Main.master). Other masters: Support Admin / Subscriber already render HeaderHelp; Accountant keeps Help in the account menu on purpose (v38 Accountant PPT has no ? in the header) - unchanged.

## s3 My Profile
MyProfile.aspx -> ResellerDashboard.aspx?view=profile for reseller logins (was the empty business-profile list). New Profile view: View mode (Company & Contact, Address tables) + Edit Profile / "Add Profile Details" (when empty). Edit: Company Name*, Contact First Name*, Last Name, Email*, Phone*, Alternate Phone, Address module (Address Lookup + Address Details: street no, line 1/2, suite, city, province/state, postal/ZIP, country). Save/Cancel. Stored in the reseller's own AnyminuteReseller row (ResellerName, ContactPerson = first + last, Email, Phone, AltPhone, Address = one composed line, same as Support Admin > Edit Reseller). No schema change / no SQL. Audit "Profile Updated (Reseller)". Settings > My Profile card links here; nav highlights Settings.

## s4 Address on reseller Add / Edit Subscriber
Add: full-width Address card (ad_addr module) between Contact Information and Subscription Plan & Limits; AddPost no longer blanks the reseller address (was v60 rule) so it is validated, shown in the confirm dialog, saved to EmpAddress and queued for Address Verification like Support Admin Add.
Edit: Address card (collapsed current address + Manage Address -> module, ed_addr) between Contact Information and Plan & Limits; EditPost already saves ed_addr (not opened = unchanged).

## s5 Businesses
Header "+ Add Business"; Actions = View + kebab (View Business, Edit Business, View Subscriber). View = in-page business card. Add/Edit form: Subscriber picker (Add; own subscribers only) / read-only subscriber (Edit), Company Name*, Operating Name, Business Number, Legal Entity Type, Registration Date, Status, Primary Contact Name*, Number* (10 digits), Primary Email*, Address module (required on Add; Edit = Manage Address gate, stored address kept when not opened).
Save = AnyminuteBusinessSave.Save(..., ownerEmployee) - same SPs (Sp_InsertBusiness / SP_UpdateBusinessDetailsWithoutImage, Sp_InsertAssginBussiness, Sp_InsertBusinessAddress) in one transaction; the new ownerEmployee overload skips the session-owner checks only for the reseller path. Scope: edit id must be in RP.Businesses(reseller) and not Deleted; add subscriber must be in RP.Subscribers(reseller). Needs the "businesses" permission. Plan MaxBusinesses and reseller MaxBusinessesPerSubscriber limits enforced on Add. Duplicate company name blocked. Logo upload not offered (existing logo kept). Audit "Business Added/Updated (Reseller)".

Compile: App_Code OK (1 pre-existing warning); ResellerDashboard, SupportSubscribers, MyProfile, Modern.master, Add_Business page compile 0 errors (mcs stub approach).
Not live-tested. No CloudAgent. No GitHub push. No new SQL.
