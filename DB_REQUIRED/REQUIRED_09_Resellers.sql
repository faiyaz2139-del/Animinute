/* AnyMinute - Resellers (2026-10-29 v28, Shamsu deck 2 slides 1-4)
   Additive and idempotent: creates 2 new tables only if they do not exist. No existing table, column, procedure
   or row is changed or deleted. Safe to run more than once. Run after REQUIRED_07 (AnyminuteSupportAudit).
   Needed for: Support Admin > Resellers (list / Add Reseller / View / activate-deactivate / assign subscribers) and the
   8 Reseller reports. Without it Resellers opens read-only with a "database update needed" message and the reseller
   reports show an empty state. Reseller changes are logged in AnyminuteSupportAudit (Entity = 'Reseller'). */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AnyminuteReseller','U') IS NULL
CREATE TABLE dbo.AnyminuteReseller(
    ResellerID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteReseller PRIMARY KEY,
    ResellerName nvarchar(150) NOT NULL,
    ContactPerson nvarchar(150) NOT NULL,
    Email nvarchar(150) NOT NULL,
    Phone nvarchar(40) NOT NULL,
    AltPhone nvarchar(40) NULL,
    Address nvarchar(400) NULL,
    PlanName nvarchar(60) NULL,                       -- Basic | Standard | Professional | Premium
    LoginEmail nvarchar(150) NOT NULL,
    TempPasswordHash varchar(64) NULL,                -- SHA-256 (hex) of salt + temporary password; the password itself is never stored
    TempPasswordSalt varchar(32) NULL,
    InvitationStatus varchar(12) NOT NULL CONSTRAINT DF_AnyminuteReseller_Invite DEFAULT 'Pending', -- Pending | Accepted
    WelcomeEmailSentAtUTC datetime2(0) NULL,
    Status varchar(10) NOT NULL CONSTRAINT DF_AnyminuteReseller_Status DEFAULT 'Active',          -- Active | Inactive
    LoginDisabled bit NOT NULL CONSTRAINT DF_AnyminuteReseller_LoginDisabled DEFAULT 0,
    Permissions nvarchar(400) NULL,                   -- comma list: subscribers,businesses,plans,usage,reports,upgrades,api
    MaxSubscribers int NULL,                          -- NULL = unlimited
    MaxBusinessesPerSubscriber int NULL,              -- NULL = unlimited
    Notes nvarchar(1000) NULL,                        -- internal, Support Admin only
    LastLoginAtUTC datetime2(0) NULL,
    CreatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteReseller_Created DEFAULT SYSUTCDATETIME(), CreatedBy nvarchar(150) NULL,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteReseller_Updated DEFAULT SYSUTCDATETIME(), UpdatedBy nvarchar(150) NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='UX_AnyminuteReseller_LoginEmail')
CREATE UNIQUE INDEX UX_AnyminuteReseller_LoginEmail ON dbo.AnyminuteReseller(LoginEmail);
IF OBJECT_ID('dbo.AnyminuteResellerSubscriber','U') IS NULL
CREATE TABLE dbo.AnyminuteResellerSubscriber(
    UserId int NOT NULL CONSTRAINT PK_AnyminuteResellerSubscriber PRIMARY KEY,   -- subscriber (UserAcc.UserId); one reseller per subscriber
    ResellerID int NOT NULL CONSTRAINT FK_AnyminuteResellerSubscriber_Reseller REFERENCES dbo.AnyminuteReseller(ResellerID),
    AssignedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteResellerSubscriber_At DEFAULT SYSUTCDATETIME(),
    AssignedBy nvarchar(150) NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteResellerSubscriber_Reseller')
CREATE INDEX IX_AnyminuteResellerSubscriber_Reseller ON dbo.AnyminuteResellerSubscriber(ResellerID);
COMMIT TRANSACTION;
SELECT name AS created_or_present FROM sys.tables WHERE name IN('AnyminuteReseller','AnyminuteResellerSubscriber');
