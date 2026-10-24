/* AnyMinute - Last login (2026-10-24 v7)
   Additive and idempotent: adds one nullable column to dbo.AnyminuteLoginSecurity (created by REQUIRED_01).
   Nothing is changed or deleted. Safe to run more than once. Values are stored in UTC.
   Needed for "Last login" on Edit Employee; the site works without it (shows "Never"). */
IF OBJECT_ID('dbo.AnyminuteLoginSecurity','U') IS NULL
    RAISERROR('Run REQUIRED_01_EmployeeModule.sql first.',16,1);
ELSE IF COL_LENGTH('dbo.AnyminuteLoginSecurity','LastLoginDate') IS NULL
    ALTER TABLE dbo.AnyminuteLoginSecurity ADD LastLoginDate datetime2(0) NULL;
GO
SELECT COL_LENGTH('dbo.AnyminuteLoginSecurity','LastLoginDate') AS LastLoginDate_present;
