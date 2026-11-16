# AnyMinute v84 (stamp v84-20261116) - Copy to next week on the Weekly Time Card

## Page
WeekTimeCard.aspx (Weekly Time Card). The screenshot shows the week grid (Attendance, Clock in, Clock out, Next day, Unpaid/paid break, Net hours, Status, Action), the Submit Week help text, and the Work Schedule / Save Weekly Schedule hint.

## Feature
A "Copy to next week" button in the week toolbar (next to "Copy Monday to weekdays", same button style) copies each entered day of the current week into the same weekdays of the following week, for the same employee and business, as draft / unsubmitted entries (TimeSheetEntry.Status=1; the target AnyminuteWeek stays Draft, or Rejected if it was already returned).

Copied for each entered day: attendance (Worked or Day Off), clock in/out, next-day flag (the clock-out datetime is shifted by 7 days), and breaks. Net hours are recomputed from the shift minus unpaid breaks with the existing ValidateSavedBreaks / Total logic. "Not entered" days are skipped. Incomplete Worked days abort the copy with a clear message (nothing is committed).

## Rules
- Works when the current week is Submitted or locked: the source is read-only; the button stays visible.
- Target days that already have entries are skipped by default. If any such days exist, a JS confirm says how many; OK overwrites those days, Cancel still copies but skips them.
- If the target week is Submitted, Approved, or locked (AnyminuteWeek state or TimeCardStatus), nothing is written and the page shows a clear message (return or reopen it first).
- After a successful copy the card opens the next week with: "Copied N days to week of <date>."
- Reuses the existing save/insert SQL (parameterized). No schema changes and no new stored procedures.

## Deploy
Copy site/ over the live site (Web.config is not in the zip; keep the live one). No SQL required. Same compile-on-server method as v83. Check: view-source meta anyminute-build = v84-20261116; open Weekly Time Card, use Copy to next week, confirm the next week shows draft copies and the source week is unchanged.

## Not in this drop
No deploy, no GitHub push, no CloudAgent, no SQL writes outside the normal TimeSheetEntry / BreakTime / AnyminuteWeek save path.
