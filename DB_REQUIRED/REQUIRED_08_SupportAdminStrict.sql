/* AnyMinute - Support Admin strict deck pass (2026-10-28 v26). Run AFTER REQUIRED_01 and REQUIRED_07.
   Additive and idempotent: adds 2 nullable columns, 2 new tables and 1 AFTER INSERT trigger. No existing column,
   procedure or row is changed or deleted. Safe to run more than once.
   - AnyminuteLoginSecurity.SessionsRevokedAtUTC : Disable Login Access ends live sessions (checked on the next request).
   - AnyminuteSupportAudit.Location               : resolved IP location for Login Activity History.
   - AnyminuteActiveSession                       : Active Sessions count on Login & Access.
   - AnyminutePasswordResetToken                  : 24-hour, one-time password reset links (only a SHA-256 hash is stored).
   - TR_AnyminuteAddressMaster_Submitted          : records the submitted date + original values of every new AddressMaster row
                                                    in AnyminuteAddressReview (Address Verification "Submitted Date"). */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AnyminuteLoginSecurity','U') IS NOT NULL AND COL_LENGTH('dbo.AnyminuteLoginSecurity','SessionsRevokedAtUTC') IS NULL
    ALTER TABLE dbo.AnyminuteLoginSecurity ADD SessionsRevokedAtUTC datetime2(0) NULL;
IF OBJECT_ID('dbo.AnyminuteSupportAudit','U') IS NOT NULL AND COL_LENGTH('dbo.AnyminuteSupportAudit','Location') IS NULL
    ALTER TABLE dbo.AnyminuteSupportAudit ADD Location nvarchar(120) NULL;
IF OBJECT_ID('dbo.AnyminuteActiveSession','U') IS NULL
CREATE TABLE dbo.AnyminuteActiveSession(
    SessionId varchar(100) NOT NULL CONSTRAINT PK_AnyminuteActiveSession PRIMARY KEY,
    UserId int NOT NULL,
    StartedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteActiveSession_Start DEFAULT SYSUTCDATETIME(),
    LastSeenAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteActiveSession_Seen DEFAULT SYSUTCDATETIME(),
    IpAddress varchar(45) NULL, Device nvarchar(200) NULL,
    EndedAtUTC datetime2(0) NULL, EndReason nvarchar(100) NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteActiveSession_User')
CREATE INDEX IX_AnyminuteActiveSession_User ON dbo.AnyminuteActiveSession(UserId,EndedAtUTC,LastSeenAtUTC);
IF OBJECT_ID('dbo.AnyminutePasswordResetToken','U') IS NULL
CREATE TABLE dbo.AnyminutePasswordResetToken(
    TokenHash char(64) NOT NULL CONSTRAINT PK_AnyminutePasswordResetToken PRIMARY KEY,
    UserId int NOT NULL, EmployeeId int NOT NULL, Email nvarchar(200) NULL,
    CreatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminutePasswordResetToken_Created DEFAULT SYSUTCDATETIME(),
    ExpiresAtUTC datetime2(0) NOT NULL, UsedAtUTC datetime2(0) NULL, CreatedBy int NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminutePasswordResetToken_User')
CREATE INDEX IX_AnyminutePasswordResetToken_User ON dbo.AnyminutePasswordResetToken(UserId,UsedAtUTC);
COMMIT TRANSACTION;
GO
IF OBJECT_ID('dbo.AnyminuteAddressReview','U') IS NOT NULL AND OBJECT_ID('dbo.TR_AnyminuteAddressMaster_Submitted','TR') IS NULL
EXEC(N'CREATE TRIGGER dbo.TR_AnyminuteAddressMaster_Submitted ON dbo.AddressMaster AFTER INSERT AS
BEGIN
    SET NOCOUNT ON;
    INSERT dbo.AnyminuteAddressReview(AddressMasterId,SubmittedAtUTC,OrigStreet,OrigCity,OrigProvince,OrigPostal,Status)
    SELECT i.AddressMasterId,SYSUTCDATETIME(),LEFT(CONVERT(nvarchar(400),i.Street_Name),200),LEFT(CONVERT(nvarchar(400),i.City),200),LEFT(CONVERT(nvarchar(400),i.Province),200),LEFT(CONVERT(nvarchar(400),i.PostalCode),200),CASE WHEN i.Verifed=1 THEN ''Verified'' ELSE ''Pending'' END
    FROM inserted i WHERE NOT EXISTS(SELECT 1 FROM dbo.AnyminuteAddressReview r WHERE r.AddressMasterId=i.AddressMasterId);
END');
GO
SELECT name AS created_or_present FROM sys.objects WHERE name IN('AnyminuteActiveSession','AnyminutePasswordResetToken','TR_AnyminuteAddressMaster_Submitted')
UNION ALL SELECT 'AnyminuteLoginSecurity.SessionsRevokedAtUTC' WHERE COL_LENGTH('dbo.AnyminuteLoginSecurity','SessionsRevokedAtUTC') IS NOT NULL
UNION ALL SELECT 'AnyminuteSupportAudit.Location' WHERE COL_LENGTH('dbo.AnyminuteSupportAudit','Location') IS NOT NULL;
