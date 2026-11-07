# AnyMinute v70 (2026-11-07) - "Fix all in 1 go" (docx notes_20261107_v70, 3 items)
Base v69 fd4636f. Code commit 46e2092. Build stamp v70-20261107; cache-bust ?v=20261107v70 (AnyminuteSupport.css/js, AnyminuteAddressLookup.css on ResellerDashboard).

1. Reseller Add Business address (image1 -> image2): ResellerDashboard.aspx never loaded css/AnyminuteAddressLookup.css, so the module rendered unstyled.
   Added the link, and AnyminuteSupportAddress.Render gained an optional `std` title (new overload; old callers unchanged) that renders the
   standard EmployeeEditor layout (.am-emp-al46): Address Lookup card, then a separate "Business Address" card with the "Address Details" pill,
   3-column grid, "City, State/Province and Country" band, image2 placeholders and "Select". Extra "Address" h2 removed. Edit Business uses the same panel behind Manage Address.
2. Reseller My Profile edit address (image3): same standard layout, card title "Address". Field names unchanged, so save/validation unchanged.
3. Employees > Edit Employee: AnyminuteUiNavigation now points the Edit Employee item (legacy AddEditRole_Add_Employee / Users.aspx?action=edit, or the fallback) to the
   new EmployeeSelect.aspx (Modern.master, new-UI dialog). It lists only employees the subscriber fully owns (AnyminuteEmployeesView.Load() where Manage, the same rule
   EmployeeEditor.RequireEditable enforces), with search (name/email/phone/business) + dropdown; Edit Employee (or double-click) -> EmployeeEditor.aspx?EmployeeId=<id>.
   Server re-checks the id is in the owned list. Both Main.master and Modern.master build the menu through AnyminuteUiNavigation, so one change covers every nav.

Files: App_Code/AnyminuteSupportAddress.cs, App_Code/AnyminuteUiNavigation.cs, EmployeeSelect.aspx(+.cs) NEW, ResellerDashboard.aspx(+.cs), Scripts/AnyminuteSupport.js
(province blank "Select" in std layout), css/AnyminuteSupport.css (.sa-addr-std rules), Main.master, Modern.master, Support*/SuperAdmin*.aspx (cache-bust only).
SQL: none.
Compile: full App_Code + ResellerDashboard.aspx.cs + EmployeeSelect.aspx.cs (mcs stub approach, /opt/monoenv) 0 errors. Address markup rendered headless in Chrome with the site CSS and matches image2.
Risks: no live run. Edit Employee picker shows nothing for a user who is not owner of every business an employee belongs to (same as EmployeeEditor rule).
