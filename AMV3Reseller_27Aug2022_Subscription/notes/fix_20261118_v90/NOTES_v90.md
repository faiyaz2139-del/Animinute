# AnyMinute v90 (2026-11-18)

Commit: 530eeb9ae1be2e59b0cfb4b7b9baf73d4512f1c3 (code). Built from that commit; Jquery/build datetimepicker deletions were left unstaged.

Stamp: v90-20261118 / 20261118v90

## Items
1. Work Schedule restyled to Weekly Time Card chrome. Functionality and JS hooks kept. Breadcrumb removed. Totals remain below the grid.
2. Login & Access restyled to the Businesses list (header, stat cards, filter/search, table, action button). No pagination added.
3. Employees list restyled the same way. Pagination kept.
4. Manager (User_TypeId 2) and Administrator/Supervisor (id 5) display as one Manager option. Assign Role on Add/Edit Business Assignment shows Employee (default checked on Add) and Manager only. Accountant stays hidden on this screen only (feature kept). Saving an existing id 5 without a role change keeps id 5; new Manager selections save as id 2. Report Access tiers (Employee / Supervisor / Administrator) were left unchanged — they are a distinct permission system.
5. Add Employee screens (Add_EmployeeV2*, AddEditRole_Add_Employee, EmployeeEditor, Edit_EmployeeV2, Change_Rate_EmployeeV2) offer a single Manager option and save by id so SelectedItem.Text is not corrupted.
6. Add Business Assignment matches the mockup: subtitle, Assignment Details, Assign Role, Job Title * with + Add / Edit / Delete (inline editor, no pop-up), Employee Type, Pay Type, Pay Rate, Billing Rate (Optional), Effective Date, End Date (Optional), Assignment Status, Default Project "None", Notes 0/500, Cancel, + Add Assignment. Job Title is Add-only (not on Edit).
7. Remaining Manager+Supervisor duplicates and Administrator role labels on add/edit screens were collapsed to Manager. Role dashboard "Your Role for This Business" now says Manager.

## SQL
Additive idempotent `DB_REQUIRED/REQUIRED_11_JobTitle.sql` (also copied under the site folder so it is in v90_src.zip). Creates `AnyminuteJobTitle` and, if `AnyminuteAssignmentDetail` exists, adds nullable `JobTitleId`. Deploy script: file backup, then `BACKUP DATABASE` COPY_ONLY (or table-copy fallback), then the script in a transaction. No Web.config change. No data deletes.

## Packaging
- /workspace/deploy/v90_src.zip sha256 1dc02ba5666d266bb55b3ad69dd5ca8c0afb7657507b422b64bac20dbc842bef (786 files, commit 530eeb9)
- /workspace/anymin/AnyMinute_Fix_20261118_v90.zip sha256 6534516aa0f6f34488a7c6f30332288abc07f43519a583f7f23fdfc870e20637
- /workspace/deploy/ssm/deploy-anymin-v90.ps1 sha256 f94e1e6f0688396e14b38985c52d0ee73e62e825cfe2bc7bf601298f6c67e43e

