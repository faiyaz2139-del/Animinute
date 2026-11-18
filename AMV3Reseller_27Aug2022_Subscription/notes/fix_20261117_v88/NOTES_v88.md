# AnyMinute v88 (stamp v88-20261117) - Flat design, Supervisor label, menus, Add Project, report chrome

Base: v87 (89e6051 / notes 22d02a0). Cumulative and lean like v87. No SQL. No data deleted. Not deployed, not pushed.

## 1. Flat design everywhere
Shared stylesheet css/AnyminuteFlat.css is loaded last on Modern.master, Main.master, MasterWithoutMenu.master and the standalone login/registration pages. It turns off box-shadows and text-shadows on every UI surface, removes raised/floating card chrome (borders, radius, shadows) from page wrappers, nested section panels and shaded section header bars, and leaves inputs, buttons, tables and pills with a normal thin border. Sections are separated by whitespace or a single thin divider. The same file also flattens remaining dialogs/modals (`.am-dialog-page`, `.am-addr-dialog`, `.sa-modal-box`, etc.). Decorative Georgia faces in address-lookup step badges were switched to Arial.

## 2. Administrator -> Supervisor (display only)
The employee Assign Roles option stored as "Administrator" (UserType 2) is shown as "Supervisor". Values, User_TypeId and permissions are unchanged. Helper: AnyminuteEmployeeEditor.RoleDisplay. Applied to Edit/Add Business Assignment checkboxes (EmployeeAssignment; Administrator is offered again and labelled Supervisor; Accountant stays hidden as in v86), Add Employee / Login & Access role lists (EmployeeEditor), Employees list chips and role filter, Employee Details chips, assignment role arrays, and the legacy Home_Page label. "Payroll Administrator", "Business Administrator", and the Report Access permission tier named Administrator were left as those names (they are not the employee assignment role).

## 3. Dashboard Business Overview ⋮ menu
The ⋮ menu was clipped by `.am-sd-table-wrap { overflow-x: auto }` (which also clips on the y-axis). Scripts/AnyminuteMenus.js positions open kebab/action menus (`am-sd-menu`, `am-menu`, `am-employee-menu`, `am-business-status-menu`, `am-ed-menu`, `am-pj-menu`, `sa-menu`, `am-business-menu`) as fixed overlays so they sit above the page with no inner scrollbar. Loaded from all three masters.

## 4. Add Project is a full page
`.am-pj-editor` was `max-width: 500px` and looked like a centred pop-up. Flat CSS makes Add/Edit Project (ProjectEditor.aspx) a full-width page in the same style as Edit Employee. The leftover link on View_Project.aspx now goes to ProjectEditor.aspx instead of the legacy Add_Project.aspx.

## 5. Sans page headings
h1 (and leftover editorial tokens) use Arial/Helvetica, not Georgia or STIX Two Text. Covers Add Project, Work Schedule, Time Card Detail Report and every other typography page. Font files are unchanged; they are no longer used for titles.

## 6. Time Card Detail Report (shared report layout)
The role badge at the top right (`.am-rl-role`, e.g. "Administrator") is no longer rendered in AnyminuteReportsUi and is hidden in CSS. The big outer frame around the report list + filters (`.am-rl-body` border/shadow/padding) is removed. Applies to every report that uses AnyminuteReportLayout.

## Edit Info
Buttons already say "Edit" (v85). No leftover "Edit Info" / "Edit Basic Info" labels were found.

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Check: view-source meta anyminute-build = v88-20261117; Employees / Edit Assignment show Supervisor not Administrator; pages are flat; Add Project is a full page; report header has no role badge and no outer frame; Business Overview ⋮ menu is fully visible.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL. Live server visual check was not possible from this box; verification used headless Chrome renders of representative markup with the real CSS/JS (see screenshots below).
