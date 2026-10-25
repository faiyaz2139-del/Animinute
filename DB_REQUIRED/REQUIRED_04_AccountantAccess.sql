-- 2026-10-24 v9 Deck G: Accountant & Payroll Access. Additive and idempotent; safe to run more than once.
-- Stores invitation details the legacy schema has no place for (firm name, access type, extra permissions,
-- access expiry, invite status). Actual business access still uses the legacy Accountant role (UserTypeId 12:
-- AddEmployee / AssginBussiness / MappingTable / User_Role_Permission), so existing pages keep working.
IF OBJECT_ID('dbo.AnyminuteAccessInvite','U') IS NULL
CREATE TABLE dbo.AnyminuteAccessInvite(
    InviteID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteAccessInvite PRIMARY KEY,
    FullName nvarchar(150) NOT NULL,
    FirmName nvarchar(150) NOT NULL,
    Email nvarchar(200) NOT NULL,
    Phone nvarchar(40) NULL,
    IsAccountant bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_Acc DEFAULT(0),
    IsPayrollAdmin bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_Pay DEFAULT(0),
    PermPayRates bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_P1 DEFAULT(0),
    PermExport bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_P2 DEFAULT(0),
    PermApprovePayroll bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_P3 DEFAULT(0),
    PermManageAccess bit NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_P4 DEFAULT(0),
    ExpiresOn date NULL,
    Status nvarchar(20) NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_Status DEFAULT('Invited'), -- Invited / Active / Expired / Revoked
    UserID int NULL,                -- legacy login (MappingTable.USERID) once linked
    InvitedAtUTC datetime2 NOT NULL CONSTRAINT DF_AnyminuteAccessInvite_At DEFAULT(SYSUTCDATETIME()),
    InvitedBy int NULL,
    ActivatedAtUTC datetime2 NULL,
    RevokedAtUTC datetime2 NULL,
    RevokedBy int NULL,
    UpdatedAtUTC datetime2 NULL,
    UpdatedBy int NULL);
IF OBJECT_ID('dbo.AnyminuteAccessInviteBusiness','U') IS NULL
CREATE TABLE dbo.AnyminuteAccessInviteBusiness(
    InviteID int NOT NULL,
    BusinessID int NOT NULL,
    CONSTRAINT PK_AnyminuteAccessInviteBusiness PRIMARY KEY(InviteID,BusinessID));
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteAccessInvite_Email' AND object_id=OBJECT_ID('dbo.AnyminuteAccessInvite'))
CREATE INDEX IX_AnyminuteAccessInvite_Email ON dbo.AnyminuteAccessInvite(Email,Status);
SELECT name FROM sys.tables WHERE name IN('AnyminuteAccessInvite','AnyminuteAccessInviteBusiness');
