/* AnyMinute - Support Admin console (2026-10-28 v25, Shamsu decks 1-4)
   Additive and idempotent: creates 4 new tables only if they do not exist and seeds the 5 deck plans only when the
   plan table is empty. No existing table, column, procedure or row is changed or deleted. Safe to run more than once.
   Needed for: Address Verification review/audit (original kept, corrected + admin + date/time + reason),
   Support Ticket assignee/category/priority/archive + Ticket Activity History, Subscription Plans list/add/edit with
   Confirm Plan Changes log, and Subscriber Login Activity History. Without it the Support Admin pages open read-only
   and show a "database update needed" message. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AnyminuteSupportAudit','U') IS NULL
CREATE TABLE dbo.AnyminuteSupportAudit(
    AuditID bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteSupportAudit PRIMARY KEY,
    Entity varchar(20) NOT NULL,            -- Ticket | Address | Plan | Login
    EntityKey nvarchar(50) NOT NULL,        -- TicketId | AddressMasterId | PlanID | UserId
    Action varchar(40) NOT NULL,
    ChangedBy int NULL, ChangedByName nvarchar(150) NULL,
    ChangedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteSupportAudit_At DEFAULT SYSUTCDATETIME(),
    PrevStatus nvarchar(40) NULL, NewStatus nvarchar(40) NULL,
    PrevValue nvarchar(max) NULL, NewValue nvarchar(max) NULL,
    Comments nvarchar(1000) NULL, IpAddress varchar(45) NULL, Device nvarchar(200) NULL, Success bit NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteSupportAudit_Entity')
CREATE INDEX IX_AnyminuteSupportAudit_Entity ON dbo.AnyminuteSupportAudit(Entity,EntityKey,ChangedAtUTC);
IF OBJECT_ID('dbo.AnyminuteAddressReview','U') IS NULL
CREATE TABLE dbo.AnyminuteAddressReview(
    AddressMasterId int NOT NULL CONSTRAINT PK_AnyminuteAddressReview PRIMARY KEY,
    SubmittedBy nvarchar(150) NULL, SubmittedAtUTC datetime2(0) NULL,
    OrigStreet nvarchar(200) NULL, OrigCity nvarchar(200) NULL, OrigProvince nvarchar(200) NULL, OrigCountry nvarchar(60) NULL, OrigPostal nvarchar(200) NULL,
    Status varchar(12) NOT NULL CONSTRAINT DF_AnyminuteAddressReview_Status DEFAULT 'Pending', -- Pending | Verified | Rejected
    Country nvarchar(60) NULL, Notes nvarchar(500) NULL,
    ReviewedBy int NULL, ReviewedByName nvarchar(150) NULL, ReviewedAtUTC datetime2(0) NULL);
IF OBJECT_ID('dbo.AnyminuteTicketMeta','U') IS NULL
CREATE TABLE dbo.AnyminuteTicketMeta(
    TicketId int NOT NULL CONSTRAINT PK_AnyminuteTicketMeta PRIMARY KEY,
    Subject nvarchar(200) NULL, Category nvarchar(60) NULL, Priority varchar(10) NULL, AssignedTo nvarchar(150) NULL,
    Archived bit NOT NULL CONSTRAINT DF_AnyminuteTicketMeta_Archived DEFAULT 0,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteTicketMeta_Updated DEFAULT SYSUTCDATETIME());
IF OBJECT_ID('dbo.AnyminuteSubscriptionPlan','U') IS NULL
CREATE TABLE dbo.AnyminuteSubscriptionPlan(
    PlanID int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteSubscriptionPlan PRIMARY KEY,
    PlanName nvarchar(100) NOT NULL, Description nvarchar(300) NULL,
    MonthlyPrice decimal(18,2) NULL, AnnualPrice decimal(18,2) NULL,          -- NULL = Custom
    MaxBusinesses int NULL, MaxEmployees int NULL, MaxProjects int NULL,      -- NULL = Custom
    TrialDays int NOT NULL CONSTRAINT DF_AnyminuteSubscriptionPlan_Trial DEFAULT 14,
    BillingFrequency varchar(10) NOT NULL CONSTRAINT DF_AnyminuteSubscriptionPlan_Freq DEFAULT 'Monthly',
    Features nvarchar(max) NULL, IsActive bit NOT NULL CONSTRAINT DF_AnyminuteSubscriptionPlan_Active DEFAULT 1,
    UpdatedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteSubscriptionPlan_Updated DEFAULT SYSUTCDATETIME(), UpdatedBy int NULL);
IF NOT EXISTS(SELECT 1 FROM dbo.AnyminuteSubscriptionPlan)
INSERT dbo.AnyminuteSubscriptionPlan(PlanName,Description,MonthlyPrice,AnnualPrice,MaxBusinesses,MaxEmployees,MaxProjects,TrialDays) VALUES
 (N'Basic Plan',N'For small teams',29,290,1,10,5,14),
 (N'Standard Plan',N'Growing businesses',69,690,3,25,15,14),
 (N'Professional Plan',N'For established businesses',129,1290,5,50,25,14),
 (N'Enterprise Plan',N'Custom solution for large organizations',NULL,NULL,NULL,NULL,NULL,14),
 (N'Trial Plan',N'Limited time trial',0,0,1,5,5,30);
COMMIT TRANSACTION;
SELECT name AS created_or_present FROM sys.tables WHERE name IN('AnyminuteSupportAudit','AnyminuteAddressReview','AnyminuteTicketMeta','AnyminuteSubscriptionPlan');
