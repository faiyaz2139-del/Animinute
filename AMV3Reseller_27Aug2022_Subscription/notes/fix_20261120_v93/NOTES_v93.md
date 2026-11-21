# AnyMinute v93 (2026-11-20)

Commit: b2e684a1616c41cb803b67f5715523b37de0343c (Add Business single insert + Businesses Delete + stamp). Cumulative on v92 6c4cf49 / 10a8969. Jquery/build datetimepicker deletions left unstaged.

Stamp: v93-20261120 / 20261120v93

## Root cause
Add Business already validated the 10-digit primary contact number before insert, so a single invalid-phone postback does not write a row. A second Save (double-click or retry while the first request is still running) posts the same original form: ViewState has no SavedBusinessId yet, so Sp_InsertBusiness runs twice and two businesses appear. The phone error can still be on screen from the first attempt, or from client validation, after a later click has already saved. There is no Delete on the Businesses ⋮ menu.

## Fix
- Validate phone before any insert (server first; also client-side so an invalid number does not post).
- Block a second Save click, and reuse the Session id for the same form key so one submit creates one business.
- Delete on the existing Businesses row ⋮ menu (confirm prompt). Soft delete: AddBusiness.Status=2, which list queries already hide (Status<>2). Related data is kept. Existing duplicates are left for Faiyaz to delete.

## SQL
No new script or migration. Soft delete uses the existing AddBusiness.Status column (2), already filtered out of Businesses lists.

## Packaging
- /workspace/deploy/v93_src.zip sha256 1ae15b3546a98cc369ff82ea6d40557bcc13e3ef9298474a96817dc616979994 (786 files, commit b2e684a)
- /workspace/anymin/AnyMinute_Fix_20261120_v93.zip sha256 0ae7ad876df309da02bafa5005a13cea32eaa08bca56f01848b14eb3c6b153d5
- /workspace/deploy/ssm/deploy-anymin-v93.ps1 sha256 a7849e3f7558c2c2e1598b40ecab4222dd5c7f1098bffdb96c009e1e23d5bd55 (ASCII, no migration)
