-- AnyMinute v41 (2026-10-31): Google / Microsoft sign-in links to the permanent User ID (dbo.UserAcc.UserId).
-- Safe to re-run. Until this runs, the Google / Microsoft buttons explain that the option is not enabled yet and password sign-in is unaffected.
-- Only a SHA-256 hash of provider + subject id is stored (no raw subject, no access/refresh tokens, no passwords).
SET NOCOUNT ON; SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON;
IF OBJECT_ID('dbo.ExternalLogins','U') IS NULL
CREATE TABLE dbo.ExternalLogins(
  ExternalLoginID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalLogins PRIMARY KEY,
  UserId int NOT NULL,                                   -- permanent AnyMinute User ID (UserAcc.UserId)
  Provider varchar(20) NOT NULL CONSTRAINT CK_ExternalLogins_Provider CHECK (Provider IN ('Google','Microsoft')),
  SubjectHash char(64) NOT NULL,                         -- SHA-256 hex of Provider|sub
  EmailAtLink nvarchar(256) NOT NULL,                    -- verified provider email that matched the login email when linked
  LinkedOn datetime2(0) NOT NULL CONSTRAINT DF_ExternalLogins_LinkedOn DEFAULT SYSUTCDATETIME(),
  LastUsedOn datetime2(0) NULL
);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UX_ExternalLogins_Subject') CREATE UNIQUE INDEX UX_ExternalLogins_Subject ON dbo.ExternalLogins(Provider,SubjectHash);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='UX_ExternalLogins_UserProvider') CREATE UNIQUE INDEX UX_ExternalLogins_UserProvider ON dbo.ExternalLogins(UserId,Provider);
GO
PRINT 'OPTIONAL_05_ExternalLogins applied.';
