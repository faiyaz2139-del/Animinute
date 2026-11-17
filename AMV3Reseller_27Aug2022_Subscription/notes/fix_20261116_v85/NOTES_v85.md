# AnyMinute v85 (stamp v85-20261116) - Employee Profile button renamed to Edit

## Page
EmployeeDetails.aspx (Employee Profile, opened from the Employees list). The header has Back to List, the "Employee Profile" title, and the pencil button on the right. The Employee Information card has a short read-only helper sentence under the heading.

## Change
The header button text is now "Edit" (pencil icon kept). It was "Edit Basic Info". The helper sentence now reads "Owners can use Edit where permitted." The control id, visibility, and destination of the button are unchanged.

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Same compile-on-server method as v84. Check: view-source meta anyminute-build = v85-20261116; open Employee Profile and confirm the header button says "Edit" and the helper sentence says "Owners can use Edit where permitted."

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL, no other behaviour change.
