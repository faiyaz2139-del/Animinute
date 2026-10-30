# AnyMinute fix v33 (2026-10-30): Accountant & Payroll Agency Dashboard (Shamsu deck, 1 slide), cumulative on top of v32

The package is cumulative: `site/` holds every file changed since the original source, v33 included. Diff against v32: `CHANGES_v33.diff`.

**Deploy:** back up the site, copy `site/` over the site root (Web.config untouched), recycle the app pool, hard-refresh. Confirm `<meta name="anyminute-build" content="v33-20261030">`. No SQL.

| Slide / input | Page | Result |
|---|---|---|
| Slide 1 header: "Welcome! <name>" + "Accountant & Payroll Admin" | Modern.master.cs | Role line under the welcome for accountant logins |
| Slide 1 nav: Home, Assigned Businesses, Employees, Payroll Hours, Reports, Documents, Access History | AnyminuteUiNavigation.Render to AnyminuteAccountantDashboard.NavLinks | Flat deck nav for accountants (replaces the Dashboard / Time Cards / Notifications drawer in the dash screenshot) |
| Title, subtitle, Select Business (All Assigned Businesses (n)) | Dashboard.aspx (accountant view) | Done; select filters the page by `?b=` |
| KPIs: Assigned Businesses, Employees in Access, Pending Payroll Reviews, Available Reports | same | Done; each card links (businesses list / Users / Submitted timecards / Reports) |
| Assigned Businesses table: Business Name, Contact, Access Type, Employees, Payroll Status, Last Payroll Period, Action + search | same | Done. Access Type comes from the Accountant & Payroll Access invite (Accountant / Payroll Admin / Both). Status: Up to Date / Pending Review (submitted weeks) / Action Required (rejected weeks). Open (up to date) or View (pending) |
| Quick Actions: View Payroll Hours, Export Reports, Open Employee List, Download Documents | same | Done |
| Recent Activity + View All Activity | same | Latest 4 submitted / approved / returned payroll weeks |

Owners, employees, supervisors and Support Admin are unchanged; "Assigned Businesses" (`Dashboard.aspx?view=businesses`) still opens the existing businesses list. v32 fixes untouched.

Compile: mcs -langversion:5 -sdk:4.5 App_Code + Dashboard.aspx + Modern.master, 0 errors. No SQL.

Residual: the Contact column shows the contact name only, because the business has no contact email on the row. Access History links to Recent Activity, since there is no per-accountant audit page. Documents shows only if the role has a document page in its menu (otherwise it falls back to Reports). Activity covers payroll weeks only, not report downloads or document views, because those are not logged.
