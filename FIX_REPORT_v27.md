# AnyMinute v27 — Fix report (2026-10-28, America/Toronto)
Build meta `v27-20261028` (Modern.master + Main.master). Base: v26 (`308a129`). Cumulative overlay (v18–v27). Static checks only: no deploy, no live DB, no runtime claims.

| # | Item | Result | Fix |
|---|------|--------|-----|
| 1 | Help → Raise_Ticket_By_User "SOMETHING WENT WRONG" (all roles) | FIXED | Root cause: Page_Load read `Session["NameOfLoggedInUser"].ToString()`, but that key is only set by Main.master Page_Load, which runs **after** the content page, so it was null → NullReferenceException. The page now uses Modern.master, reads the session null-safely (name falls back to Session["user"] / FN_DESC), sends Support Admin to SupportTickets.aspx, uses a parameterised Ticket_Comments insert, and shows friendly notices instead of the error page. |
| 2 | Accountant dashboard (kk3) old UI / empty charts / clipped sidebar / v22 meta | FIXED | kk3 is an Accountant login (UserTypeId 12) and was sent to the old Main.master `Accountant_Newuser_Dashboard.aspx`. Login now sends type 12 to `Dashboard.aspx` (Modern.master, v27 meta, full sidebar). Old Accountant_Newuser_Dashboard / Accountant_Dashboard (type 12) redirect there too. Accountant copy: "Businesses you have accountant access to…", a Reports card in place of Pending Approvals, no "My Time Card", and an explicit empty state ("No businesses have shared accountant access with you yet…"). One ambiguous membership no longer breaks the Dashboard or the header pending count. |
| 3 | AssgineManager.aspx old header/Home, empty pickers, no circular messaging | FIXED | Rewritten on Modern.master. Business picker (owner/admin businesses), Manager picker (active managers), and user checklist are always populated, also without `?BusinessIdFromDefaultPapge`. Users who would close a loop are greyed out with "(already approves this manager - circular chain) [AM-v27]". Saves go through `AnyminuteApprovalAssignments.Save` (RequireOwner, fingerprint, circular guard). The old unparameterised delete/insert is gone. Non-owners get "Access denied". `AnyminuteApprovalAssignments.Build` is now `v27`, so GridviewNewDesign shows the same tag. |
| 4 | LoginAccess.aspx as manager → error page | FIXED | The `HttpException(403)` throw is replaced with an in-page "Access denied" notice. Support Admin → SupportSubscribers.aspx. Scope-check failures are logged and treated as denied. |
| 5 | "Supervisor Emplyee" | FIXED (display) | The string is not in source or the seed DB; it is a custom UserType name in live data. New `AnyminuteDisplayText.Tidy` corrects "Emplyee" → "Employee" in name/Role/UserType columns of every `AnyminuteTimesheetRepair.Query` result and in legacy `CommanClass.FillDropDownList` pickers. The DB row is not changed. Optional: rename the role in Add Role. |
| 6 | Double spaces in names ("Adam  Rhodes") | FIXED (display) | The same Tidy collapses whitespace: FN_DESC First+Middle+Last with an empty middle name; Main.master name/welcome; Modern.master welcome; pickers. |
| 7 | Employee-role Businesses page uses manager copy / Pending Approvals | FIXED | Employee-only users (not owner, no approvals) get "Businesses you work for…" and a "My Time Card" card instead of Pending Approvals. |
| 8 | Accountant Work Schedule uses employee wording | FIXED | Read-only view of someone else: "Read-only view of the planned working hours… for the selected employee or supervisor". No "Enter the hours you worked" prompt. The picker label reads "Employee / Supervisor" for accountants. Personal wording only on your own schedule. |
| 9 | Tom Beasley Estimated base pay $44,394.65 | NOT A CODE BUG (data) | Math is hours × hourly rate. Seed DB: Tom Beasley (EmployeeID 1157) latest RateDetails = **$900.50 Hourly**, and 900.50 × 49.3 h = **$44,394.65** exactly. Fix the rate in Change Rate. Code left unchanged. |
| 10 | Dashboard Total Employees 77 vs Users 43 | FIXED | The card summed per-business assignments, so a person in several businesses was counted several times. It now uses the Users directory count (one per person) when the user can read the directory, else distinct EmployeeIDs. |

## Files (site/)
Raise_Ticket_By_User.aspx(.cs), AssgineManager.aspx(.cs), LoginAccess.aspx(.cs), Dashboard.aspx(.cs), WeeklyEmployeeSchedule.aspx(.cs), Login.aspx.cs, Accountant_Dashboard.aspx.cs, Accountant_Newuser_Dashboard.aspx.cs, Main.master(.cs), Modern.master(.cs), App_Code/AnyminuteDisplayText.cs (new), App_Code/AnyminuteTimesheetRepair.cs, App_Code/AnyminuteModernScope.cs, App_Code/AnyminuteApprovalAssignments.cs, App_Code/CommanClass.cs. No CSS/JS changes, so no cache-bust. No new SQL.

## Deploy
1. Back up the live site folder.
2. Copy `site/*` over the site root (overwrite). App_Code recompiles on first hit.
3. No DB step for v27. DB_REQUIRED 01–08 as before if not yet run.
4. Recycle the app pool, then view-source any page: `anyminute-build` = `v27-20261028`.
5. Smoke: Help → Tickets / Share suggestion (all roles); kk3 login → Dashboard; Assign Users to Manager (owner); rs3 → LoginAccess.aspx shows Access denied.

## Static checks
- App_Code: mcs `-langversion:5 -sdk:4.5` → succeeded, 0 errors.
- 10 touched pages/masters compiled against App_Code with stub designers → 0 errors.

## Residual / known
- Legacy file names `AssgineManager.aspx` and `BusinessIdFromDefaultPapge` are kept (other pages link to them). Both are still accepted, and so is `?BusinessId=`.
- Item 9 needs a data fix on live. Item 5 is display-only (the custom role row still reads Emplyee in the DB).
- Not verified at runtime: accountant Dashboard counts depend on the accountant's MappingTable/AssginBussiness rows on live. Type-0 "Admin" accountants still use the legacy Accountant_Dashboard.
- Ticket_List.aspx (View Tickets) is still the legacy Main.master page. It was not reported broken.
