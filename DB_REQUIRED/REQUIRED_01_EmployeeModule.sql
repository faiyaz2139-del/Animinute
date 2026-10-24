/* AnyMinute - Employees module (2026-10-24 v2)
   Additive and idempotent: creates 4 new tables only if they do not exist. No existing table,
   column, procedure or row is changed or deleted. Safe to run more than once.
   Needed for: extra roles per assignment, billing rate / effective & end date / default project / notes,
   reason for pay-rate changes, and Login & Access settings (enable/disable, cancel invitation, 2FA flag).
   The site keeps working without it; those options show a "database update needed" message. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AnyminuteAssignmentDetail','U') IS NULL
CREATE TABLE dbo.AnyminuteAssignmentDetail(
    BusinessID int NOT NULL, EmployeeID int NOT NULL,
    BillingRate decimal(18,2) NULL, EffectiveDate date NULL, EndDate date NULL,
    DefaultProjectID int NULL, Notes nvarchar(500) NULL,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteAssignmentDetail_Updated DEFAULT SYSUTCDATETIME(),
    UpdatedBy int NULL,
    CONSTRAINT PK_AnyminuteAssignmentDetail PRIMARY KEY (BusinessID, EmployeeID));
IF OBJECT_ID('dbo.AnyminuteAssignmentRole','U') IS NULL
CREATE TABLE dbo.AnyminuteAssignmentRole(
    BusinessID int NOT NULL, EmployeeID int NOT NULL, UserTypeId int NOT NULL,
    AddedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteAssignmentRole_Added DEFAULT SYSUTCDATETIME(),
    AddedBy int NULL,
    CONSTRAINT PK_AnyminuteAssignmentRole PRIMARY KEY (BusinessID, EmployeeID, UserTypeId));
IF OBJECT_ID('dbo.AnyminuteRateChangeReason','U') IS NULL
CREATE TABLE dbo.AnyminuteRateChangeReason(
    RateId int NOT NULL CONSTRAINT PK_AnyminuteRateChangeReason PRIMARY KEY,
    Reason nvarchar(200) NOT NULL, ChangedBy int NULL,
    ChangedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteRateChangeReason_At DEFAULT SYSUTCDATETIME());
IF OBJECT_ID('dbo.AnyminuteLoginSecurity','U') IS NULL
CREATE TABLE dbo.AnyminuteLoginSecurity(
    UserId int NOT NULL CONSTRAINT PK_AnyminuteLoginSecurity PRIMARY KEY,
    LoginDisabled bit NOT NULL CONSTRAINT DF_AnyminuteLoginSecurity_Disabled DEFAULT 0,
    RequirePasswordChange bit NOT NULL CONSTRAINT DF_AnyminuteLoginSecurity_Require DEFAULT 0,
    TwoFactorEnabled bit NOT NULL CONSTRAINT DF_AnyminuteLoginSecurity_2FA DEFAULT 0,
    InvitationCancelledAtUTC datetime2(0) NULL,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteLoginSecurity_Updated DEFAULT SYSUTCDATETIME(),
    UpdatedBy int NULL);
COMMIT TRANSACTION;
SELECT name AS created_or_present FROM sys.tables WHERE name IN('AnyminuteAssignmentDetail','AnyminuteAssignmentRole','AnyminuteRateChangeReason','AnyminuteLoginSecurity');
