/* Optional (2026-10-24 v2): stores the Country field of the employee address. Additive, idempotent.
   Without it the Country box is still shown but not saved. */
IF COL_LENGTH('dbo.EmpAddress','Country') IS NULL
    ALTER TABLE dbo.EmpAddress ADD Country nvarchar(100) NULL;
