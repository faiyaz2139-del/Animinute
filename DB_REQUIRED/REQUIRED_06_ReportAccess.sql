/* AnyMinute v19 (2026-10-26) - REQUIRED_06: Report Access matrix (User Access > Report Access, deck slide 6).
   Additive and idempotent: creates one new table; no existing table or row is changed. Safe to run more than once.
   Without it the Reports hub uses the built-in slide 6 defaults and the Report Access page cannot save.
   OwnerID   = EmployeeID of the business owner whose settings these are.
   BusinessID = 0 for the owner's global defaults, otherwise a business-specific override (replaces the defaults).
   Tier      = Employee | Supervisor | Administrator. */
IF OBJECT_ID('dbo.AnyminuteReportAccess','U') IS NULL
CREATE TABLE dbo.AnyminuteReportAccess(
    OwnerID int NOT NULL,
    BusinessID int NOT NULL,
    ReportKey varchar(40) NOT NULL,
    Tier varchar(16) NOT NULL,
    CanView bit NOT NULL CONSTRAINT DF_AnyminuteReportAccess_View DEFAULT 0,
    CanPdf bit NOT NULL CONSTRAINT DF_AnyminuteReportAccess_Pdf DEFAULT 0,
    CanExcel bit NOT NULL CONSTRAINT DF_AnyminuteReportAccess_Excel DEFAULT 0,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteReportAccess_Updated DEFAULT SYSUTCDATETIME(),
    UpdatedBy int NULL,
    CONSTRAINT PK_AnyminuteReportAccess PRIMARY KEY(OwnerID,BusinessID,ReportKey,Tier),
    CONSTRAINT CK_AnyminuteReportAccess_Tier CHECK(Tier IN('Employee','Supervisor','Administrator'))
);
GO
SELECT name AS created_or_present FROM sys.tables WHERE name='AnyminuteReportAccess';
