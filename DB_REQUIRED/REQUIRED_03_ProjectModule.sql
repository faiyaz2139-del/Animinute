/* AnyMinute - Projects module (2026-10-24 v8)
   Additive and idempotent: creates three new tables only. Existing tables (AddProject, ASSIGNPROJECT,
   TimeSheetEntry) are not altered. Safe to run more than once.
   - AnyminuteProjectDetail            : description + display status (Active/Completed/Inactive/Archived)
   - AnyminuteProjectAssignment        : assigned date / effective end date per project employee
   - AnyminuteProjectAssignmentLog     : assignment history + audit trail for the project screens
   AddProject.Status keeps its legacy meaning ('A' = live, 'D' = deleted), so existing timesheet and report
   pages keep working unchanged. */
IF OBJECT_ID('dbo.AnyminuteProjectDetail','U') IS NULL
CREATE TABLE dbo.AnyminuteProjectDetail(
    ProjectID int NOT NULL CONSTRAINT PK_AnyminuteProjectDetail PRIMARY KEY,
    Description nvarchar(1000) NULL,
    ProjectStatus nvarchar(20) NOT NULL CONSTRAINT DF_AnyminuteProjectDetail_Status DEFAULT('Active'),
    ArchivedAtUTC datetime2(0) NULL,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteProjectDetail_Upd DEFAULT(SYSUTCDATETIME()),
    UpdatedBy int NULL);
GO
IF OBJECT_ID('dbo.AnyminuteProjectAssignment','U') IS NULL
CREATE TABLE dbo.AnyminuteProjectAssignment(
    ProjectID int NOT NULL,
    EmployeeID int NOT NULL,
    BusinessID int NOT NULL,
    AssignedDate date NOT NULL,
    EffectiveEndDate date NULL,
    AssignedBy int NULL,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteProjectAssignment_Upd DEFAULT(SYSUTCDATETIME()),
    CONSTRAINT PK_AnyminuteProjectAssignment PRIMARY KEY(ProjectID,EmployeeID));
GO
IF OBJECT_ID('dbo.AnyminuteProjectAssignmentLog','U') IS NULL
CREATE TABLE dbo.AnyminuteProjectAssignmentLog(
    LogID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteProjectAssignmentLog PRIMARY KEY,
    ProjectID int NOT NULL,
    BusinessID int NOT NULL,
    EmployeeID int NULL,
    Action nvarchar(40) NOT NULL,
    EffectiveDate date NULL,
    Note nvarchar(400) NULL,
    ActorID int NULL,
    RecordedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteProjectAssignmentLog_At DEFAULT(SYSUTCDATETIME()));
GO
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteProjectAssignmentLog_Project' AND object_id=OBJECT_ID('dbo.AnyminuteProjectAssignmentLog'))
    CREATE INDEX IX_AnyminuteProjectAssignmentLog_Project ON dbo.AnyminuteProjectAssignmentLog(ProjectID,RecordedAtUTC);
GO
/* Existing assignments get a best-effort assigned date (project start, or today if unknown). */
INSERT dbo.AnyminuteProjectAssignment(ProjectID,EmployeeID,BusinessID,AssignedDate)
SELECT x.PROJECTID,x.EMPLOYEEID,MIN(x.BUSINESSID),CAST(ISNULL(MIN(p.startDate),GETDATE()) AS date)
FROM dbo.ASSIGNPROJECT x JOIN dbo.AddProject p ON p.ProjectID=x.PROJECTID
WHERE x.PROJECTID IS NOT NULL AND x.EMPLOYEEID IS NOT NULL AND x.BUSINESSID IS NOT NULL
  AND NOT EXISTS(SELECT 1 FROM dbo.AnyminuteProjectAssignment a WHERE a.ProjectID=x.PROJECTID AND a.EmployeeID=x.EMPLOYEEID)
GROUP BY x.PROJECTID,x.EMPLOYEEID;
GO
SELECT OBJECT_ID('dbo.AnyminuteProjectDetail','U') AS ProjectDetail, OBJECT_ID('dbo.AnyminuteProjectAssignment','U') AS ProjectAssignment,
       OBJECT_ID('dbo.AnyminuteProjectAssignmentLog','U') AS ProjectAssignmentLog;
