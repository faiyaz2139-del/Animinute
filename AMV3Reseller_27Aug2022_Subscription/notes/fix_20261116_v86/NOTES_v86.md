# AnyMinute v86 (stamp v86-20261116) - Employee Details per deck + subscriber navigation/dashboard items

Base: v85 (d3c3623 / notes 276323f). Cumulative and lean like v85. No SQL. No data deleted. Not deployed, not pushed.

## Deck "Employee View Screen" (9 slides) -> EmployeeDetails.aspx(.cs), css/AnyminuteEmployeeDetails.css, App_Code/AnyminuteEmployeeDocuments.cs
| Slide | Asked (mockup) | Done | Flagged / not done |
|---|---|---|---|
| 1 Profile | "Employee Details" title + read-only subtitle; hero card (initials, name, email, phone, Active pill, edit button); icon side menu; Business Assignment selector card; Profile Information (Employee Information + Role/Employment Type/Join Date/Assigned Business + 3 counters) | Built as shown. Counters: Time Cards This Period, Projects Assigned, Documents Uploaded | Button kept as "Edit" (v85 request) not "Edit Basic Info". "Time Cards This Period" = weeks with entries in the current month. Old "Back to List" + "Employee Profile" heading removed per mockup; the new global Back button now sits top-left. Top-bar menu differences between mockups ignored (site uses its own left nav) |
| 2 Business Assignments | Table: Business, Job Title, Roles, Employee Type, Pay Type, Pay Rate, Billing Rate, Effective Date, End Date, Status, Actions; Add Assignment | Built; Job Title from latest RateDetails row; actions edit + menu (Remove) | - |
| 3 Pay Rate History | Current Pay Summary, Rate History table, Change Audit timeline, export | Built; status Active/Scheduled/Ended; CSV export | Billing rate has no history in the DB, shown on the current row only. Old "Pay Editor" button removed (not in deck) |
| 4 Time Cards | Week/Status filters, 4 KPI tiles, Recent Time Cards table, Hours Trend chart, Export | Built; 5 rows + View All; trend 6/12 weeks; CSV | - |
| 5 Documents | Upload Document, search/type/status filters, table (name, type, business, uploaded on/by, status, actions) | Built; pdf/png/jpg/doc/docx up to 4 MB; owners/managers upload/delete | New file storage at ~/App_Data/EmployeeDocuments/<id>/ - the app-pool identity needs write access to App_Data on the server |
| 6 Address | Residential + Mailing address, Last Updated, Verification Status, View Address History, Edit Address, note | Built | Only one address is stored: Mailing shows the same address as Residential. "Verified" = matching AddressMaster postal code marked verified |
| 7 Projects | 4 counters, Assign Project, Manage Assignments, table (project, client/business, role, dates, hours, status, actions) | Built; hours from time entries | No "On Hold" status in DB: inactive projects shown as "On Hold" |
| 8 Login & Access | Account status, Invitation (Resend), Last Login, Last Password Reset (Send Password Reset), MFA, Active Sessions, View Login History | Built | Password reset reuses the existing invitation/reset e-mail template (and writes a ResetPassword audit row); MFA/OTP shows the stored two-factor setting (Enabled/Disabled) |
| 9 Activity History | Date range, event type, changed by filters, Reset Filters, table | Built from the audit tables | - |
No slide asked to delete or clean data.

## Steering items (user said Fix)
- Edit Business Assignment - Assign Roles: Accountant and Administrator removed from the picker; Manager and Employee remain. UI only: a person who already holds a hidden role keeps it on save (no DB role rows deleted). The same control (EmployeeAssignment.aspx) is used for Add Business Assignment, so it applies there too. Add Employee (EmployeeEditor.aspx, different control) was not changed.
- Subscriber (Business Administrator) Dashboard: new App_Code/AnyminuteSubscriberDashboard.cs + css/AnyminuteSubscriberDashboard.css. Header + Business filter (All Businesses), 6 KPI cards, Quick Actions, Needs Your Attention, Weekly Hours Overview (week selector, Scheduled/Submitted/Approved totals, Mon-Sun Scheduled vs Submitted chart), Business Overview table with row menu, Recent Activity with View All. Real data only; zeros/empty states otherwise. Reference screenshot was not available on the box - built from the written description.
- Nav "View Business" under Businesses (subscriber) -> Businesses list.
- Businesses list: "Edit Business" under the business name renamed "Business Profile" -> new read-only BusinessProfile.aspx; its "Edit" button opens the existing Edit Business screen (owners only).
- Nav "View Employees" under Employees and "View Projects" under Projects (subscriber) -> existing lists. Work Schedule group labelled "Work Schedules" for the subscriber.
- Back button: Scripts/AnyminuteBackNav.js (Modern.master + Main.master) adds "<- Back" top-left above the page header on every screen except the Dashboard home; goes to the previous page on the site, else the screen's parent list, else Dashboard. Screens that already show their own Back link keep it.
- Left nav always visible: Scripts/AnyminuteNavDrawer.js + css/AnyminuteNavDrawer.css. Desktop (>=992px): permanent sidebar beside the content, no hamburger/collapse; sub-menus still expand. Narrow screens: open by default on each page, can be closed with the menu button. Accountant and Support Admin keep their existing pinned modes.
- Add/Edit Employee (EmployeeEditor.aspx): placeholders now use masks instead of sample values - Contact Phone and Mobile "(###) ###-####"; Address Lookup Postal/ZIP "X#X #X#" (switches to "#####" when the lookup country is United States); manual address Postal/ZIP "X#X #X#" (switches to "#####" when Country = United States); SIN "### ### ###" kept. Email label no longer shows "(To grant online access)". Scripts/AnyminuteAddressLookup.js (shared with EmployeeProfile.aspx, cache-bust bumped on both) sets the country-dependent postal placeholders. Left as-is: descriptive placeholders that are not sample values ("Enter first name", "Enter address line 2 (e.g. Unit, Floor)", "e.g. Increase in hourly pay rate." in the rate-change reason box), and the help line under the lookup "Enter a valid Canadian postal code (e.g. M1B 2W1) or US ZIP code (e.g. 33185)." (help text, not a placeholder). Legacy pages (Add_EmployeeV2, Edit_EmployeeV2, etc.) not changed.

## Deploy (when approved)
Copy site/ over the live site (Web.config not in the zip). No SQL. Make sure App_Data is writable by the app pool (Documents). Check: view-source meta anyminute-build = v86-20261116.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL, no data deletion.
