/* AnyMinute v90 - Job titles for Add Business Assignment.
   Additive and idempotent. Does not drop or rewrite existing objects. Safe to run more than once.
   Creates dbo.AnyminuteJobTitle (per-business list) and, if AnyminuteAssignmentDetail already
   exists, adds a nullable JobTitleId column. No rows are updated or deleted. */
SET XACT_ABORT ON;
IF OBJECT_ID('dbo.AnyminuteJobTitle','U') IS NULL
BEGIN
    CREATE TABLE dbo.AnyminuteJobTitle(
        JobTitleId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteJobTitle PRIMARY KEY,
        BusinessID int NOT NULL,
        Title nvarchar(150) NOT NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_AnyminuteJobTitle_Del DEFAULT 0,
        CreatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteJobTitle_Created DEFAULT SYSUTCDATETIME(),
        UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteJobTitle_Updated DEFAULT SYSUTCDATETIME(),
        UpdatedBy int NULL
    );
    CREATE INDEX IX_AnyminuteJobTitle_Business ON dbo.AnyminuteJobTitle(BusinessID, IsDeleted);
END;
IF OBJECT_ID('dbo.AnyminuteAssignmentDetail','U') IS NOT NULL
   AND COL_LENGTH('dbo.AnyminuteAssignmentDetail','JobTitleId') IS NULL
    ALTER TABLE dbo.AnyminuteAssignmentDetail ADD JobTitleId int NULL;
SELECT
    CASE WHEN OBJECT_ID('dbo.AnyminuteJobTitle','U') IS NULL THEN 0 ELSE 1 END AS JobTitleTable,
    CASE WHEN COL_LENGTH('dbo.AnyminuteAssignmentDetail','JobTitleId') IS NULL THEN 0 ELSE 1 END AS JobTitleIdColumn;
