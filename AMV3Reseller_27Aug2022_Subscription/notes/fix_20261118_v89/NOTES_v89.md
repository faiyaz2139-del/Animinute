# AnyMinute v89 (stamp v89-20261118) - Supervisor label everywhere (display only)

Base: v88 (1b8cfb3 / c5374c6). Cumulative and lean like v88. No SQL. No data deleted. Not pushed to GitHub.

## Administrator -> Supervisor (display only)

v88 already labelled the employee role "Administrator" as "Supervisor" on Employees chips, Add Employee / Login & Access role lists, and Edit/Add Business Assignment checkboxes via `AnyminuteEmployeeEditor.RoleDisplay`. The Assignment Details checkboxes still showed "Administrator" after the v88 deploy — most likely because RoleDisplay required an exact (untrimmed) match, and the UserType table stores that option as "Administrator" (standard User_TypeId 5, not 2).

v89:

- RoleDisplay trims spaces and nbsp before matching the exact name "Administrator" (case-insensitive). "Payroll Administrator" and "Business Administrator" are unchanged. Values / User_TypeId / permissions are unchanged.
- RolesDisplay maps each name in a comma-separated list (project assignment Role columns).
- Relabel / StoredName: legacy role radio/dropdown lists (Edit Employee V2, Change Rate) show Supervisor, but if SelectedItem.Text is saved, UserTypeId 2 or 5 is written back as "Administrator".
- Applied also to: Home_Page / Home_Page_EmpPopUpList UserRoleName cells; Home_Page "assigned as a supervisor"; Edit_EmployeeV2 Role link text; role history grid; Enrich UserRoleName/Role; report employee Role cells (UserTypeId 5); SweetAlert "Supervisor or Accountant User Role".

Report Access permission tiers (Employee / Supervisor / Administrator) are left as those names — Administrator and Supervisor are two different tiers there. Generic "contact your administrator" / "ask your administrator" messages are left. Payroll Administrator labels are left.

## Stamp

v89-20261118 on Modern.master, Main.master, MasterWithoutMenu.master (Register_As_A_Bo uses this) and the same cache-busting query strings as v88.

## Deploy

No SQL. Keep the live Web.config. Check: meta anyminute-build = v89-20261118; Assignment Details Assign Roles shows Supervisor (not Administrator) and Employee.
