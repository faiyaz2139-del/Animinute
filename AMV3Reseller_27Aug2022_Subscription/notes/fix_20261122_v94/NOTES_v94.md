# AnyMinute v94 (2026-11-22)

Commit: 4be2e8fc5a2192e5cccc61ba76160b3087b3d904 (assignment layout + two-step Delete + stamp). Cumulative on v93 8aad03d / b2e684a. Jquery/build datetimepicker deletions left unstaged.

Stamp: v94-20261122 / 20261122v94

## Root cause
v92 took Job Title out of am-span-2 and placed it as the next cell in the two-column Assignment Details grid, with an empty spacer to hold the right cell. The Job Title controls used flex-wrap:nowrap inside a minmax(0,1fr) column, and .am-dialog-page has overflow:hidden. That clipped the Job Title row (label, dropdown, + Add / Edit / Delete) and overflowed into the Assign Roles cell, so the Employee and Manager checkboxes (which were already bound, Employee checked by default) were not visible while "Select a business" was showing.

## Fix
- Stack Job Title under Business in a left column (am-assign-left) so the row always shows on Add, independent of business selection, with the v92 compact + Add (blue outline), Edit (pencil), and Delete (trash) buttons. Assign Roles stays in the right column and is always visible. Employee stays checked by default. No code-behind change.
- Businesses ⋮ Delete now shows a warning that the business will be deleted, then "Are you sure?". Delete runs only if both are confirmed. Soft delete (Status=2) is unchanged.

## SQL
None.

## Packaging
- /workspace/deploy/v94_src.zip sha256 ed12af17c795cc882332b4ce8a34a8730f5ab2df14f29fa7cf2abf5a2e47b639 (786 files, commit 4be2e8f)
- /workspace/anymin/AnyMinute_Fix_20261122_v94.zip sha256 dfa8e5cf5e623144f53b4ff0edb697ef381d0195c9109e79b12083c457a3559d
- /workspace/deploy/ssm/deploy-anymin-v94.ps1 sha256 8d871414c812177aa4e55adad131c8b8a66b4ed71b4ffe5f49c811a502afb521 (ASCII, ZipSha matches the v94 src zip, no migration)
