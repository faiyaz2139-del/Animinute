-- AnyMinute v39 (2026-10-31): Change Login Email (self-service, verified) + Support account recovery audit.
-- Safe to re-run. Until this runs, My Profile > Login & Security shows a "Database update needed" notice and changes nothing.
-- Users table in the DOC = existing dbo.UserAcc (UserId is the permanent User ID; Status = AccountStatus; Email = LoginEmail).
SET NOCOUNT ON; SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON; SET ANSI_PADDING ON; SET ANSI_WARNINGS ON; SET ARITHABORT ON; SET CONCAT_NULL_YIELDS_NULL ON; SET NUMERIC_ROUNDABORT OFF;
IF COL_LENGTH('dbo.UserAcc','EmailVerified') IS NULL ALTER TABLE dbo.UserAcc ADD EmailVerified bit NULL;
IF COL_LENGTH('dbo.UserAcc','UpdatedOn') IS NULL ALTER TABLE dbo.UserAcc ADD UpdatedOn datetime2(0) NULL;
GO
IF OBJECT_ID('dbo.EmailChangeRequests','U') IS NULL
CREATE TABLE dbo.EmailChangeRequests(
  RequestID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmailChangeRequests PRIMARY KEY,
  UserID int NOT NULL,
  PreviousEmail nvarchar(50) NOT NULL,
  NewEmail nvarchar(50) NOT NULL,
  VerificationTokenHash char(64) NOT NULL,          -- SHA-256 hex of the one-time token; the raw token is never stored
  RequestedOn datetime2(0) NOT NULL CONSTRAINT DF_ECR_RequestedOn DEFAULT SYSUTCDATETIME(),
  ExpiresOn datetime2(0) NOT NULL,
  VerifiedOn datetime2(0) NULL,
  Status varchar(10) NOT NULL CONSTRAINT CK_ECR_Status CHECK (Status IN ('Pending','Verified','Expired','Cancelled')),
  RequestedBy int NOT NULL,                          -- = UserID for self-service; Support Admin UserId for recovery
  Method nvarchar(40) NOT NULL                       -- 'Password' (reauthenticated) or 'Support recovery'
);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UX_ECR_TokenHash') CREATE UNIQUE INDEX UX_ECR_TokenHash ON dbo.EmailChangeRequests(VerificationTokenHash);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ECR_User') CREATE INDEX IX_ECR_User ON dbo.EmailChangeRequests(UserID,Status,RequestedOn);
GO
IF OBJECT_ID('dbo.EmailChangeAudit','U') IS NULL
CREATE TABLE dbo.EmailChangeAudit(
  AuditID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmailChangeAudit PRIMARY KEY,
  UserID int NOT NULL,
  PreviousEmail nvarchar(50) NOT NULL,
  NewEmail nvarchar(50) NOT NULL,
  ChangedBy int NOT NULL,
  ChangedOn datetime2(0) NOT NULL CONSTRAINT DF_ECA_ChangedOn DEFAULT SYSUTCDATETIME(),
  VerificationMethod nvarchar(40) NOT NULL,
  IPAddress nvarchar(45) NOT NULL,
  Result nvarchar(60) NOT NULL                       -- never contains passwords or tokens
);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_ECA_User') CREATE INDEX IX_ECA_User ON dbo.EmailChangeAudit(UserID,ChangedOn);
GO
-- Login email uniqueness. Existing duplicates are listed and the unique index is skipped until they are resolved
-- (the app also blocks any change to an email another UserId already uses).
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UX_UserAcc_LoginEmail')
BEGIN
  IF EXISTS (SELECT 1 FROM dbo.UserAcc WHERE ISNULL(LTRIM(RTRIM(Email)),'')<>'' GROUP BY LTRIM(RTRIM(Email)) HAVING COUNT(*)>1)
  BEGIN
    PRINT 'UX_UserAcc_LoginEmail NOT created: duplicate login emails below must be merged or corrected first, then re-run.';
    SELECT LTRIM(RTRIM(Email)) AS DuplicateLoginEmail, COUNT(*) AS Logins, MIN(UserId) AS FirstUserId, MAX(UserId) AS LastUserId
      FROM dbo.UserAcc WHERE ISNULL(LTRIM(RTRIM(Email)),'')<>'' GROUP BY LTRIM(RTRIM(Email)) HAVING COUNT(*)>1;
  END
  ELSE IF EXISTS (SELECT 1 FROM sys.sql_modules WHERE (uses_quoted_identifier=0 OR uses_ansi_nulls=0) AND definition LIKE '%UserAcc%')
  BEGIN
    -- A filtered index makes INSERT/UPDATE on UserAcc fail from modules created with QUOTED_IDENTIFIER/ANSI_NULLS OFF.
    PRINT 'UX_UserAcc_LoginEmail NOT created: these modules touch UserAcc and were created with QUOTED_IDENTIFIER or ANSI_NULLS OFF (recreate them with both ON, then re-run):';
    SELECT OBJECT_SCHEMA_NAME(object_id)+'.'+OBJECT_NAME(object_id) AS ModuleName FROM sys.sql_modules WHERE (uses_quoted_identifier=0 OR uses_ansi_nulls=0) AND definition LIKE '%UserAcc%';
  END
  ELSE IF EXISTS (SELECT 1 FROM dbo.UserAcc WHERE Email<>LTRIM(RTRIM(Email)))
    PRINT 'UX_UserAcc_LoginEmail NOT created: trim leading/trailing spaces in UserAcc.Email first, then re-run.';
  ELSE
    EXEC('CREATE UNIQUE INDEX UX_UserAcc_LoginEmail ON dbo.UserAcc(Email) WHERE Email IS NOT NULL AND Email<>''''');
END
GO
