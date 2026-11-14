/* AnyMinute v81 (2026-11-14) - Duplicate Addresses decisions
   Additive and idempotent: creates dbo.AnyminuteAddressDup only if it does not exist.
   No existing table, column, procedure or row is changed or deleted. Safe to run more than once.
   Used by Address Verification > Duplicate Addresses to remember Keep as Original, Mark as Duplicate,
   Dismiss Duplicate, and soft-deleted records. The tab still lists Potential Duplicates (same
   standardized street + postal code in the same country) if this table is missing; decisions cannot
   be saved until this script has been run (or the app created the table on first use).
   Read-only aside from CREATE TABLE / CREATE INDEX. No data is deleted. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AnyminuteAddressDup','U') IS NULL
CREATE TABLE dbo.AnyminuteAddressDup(
    DupId int IDENTITY(1,1) NOT NULL CONSTRAINT PK_AnyminuteAddressDup PRIMARY KEY,
    AddressMasterId int NOT NULL,
    PeerId int NOT NULL CONSTRAINT DF_AnyminuteAddressDup_Peer DEFAULT 0,
    Action varchar(20) NOT NULL, -- Original | Duplicate | Dismissed | Deleted
    ActorId int NULL,
    ActorName nvarchar(150) NULL,
    ActedAtUTC datetime2(0) NOT NULL CONSTRAINT DF_AnyminuteAddressDup_At DEFAULT SYSUTCDATETIME(),
    Notes nvarchar(500) NULL);
IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteAddressDup_Addr')
CREATE INDEX IX_AnyminuteAddressDup_Addr ON dbo.AnyminuteAddressDup(AddressMasterId,Action,ActedAtUTC);
COMMIT TRANSACTION;
SELECT name AS created_or_present FROM sys.tables WHERE name='AnyminuteAddressDup';
