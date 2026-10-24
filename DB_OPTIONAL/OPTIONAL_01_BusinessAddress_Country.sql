/* ============================================================================
   OPTIONAL MIGRATION - NOT REQUIRED FOR THE FIX TO WORK.  NOT RUN BY THE PACKAGE.
   AnyMinute 2026-10-24 : BusinessAddress.Country
   ----------------------------------------------------------------------------
   Purpose : lets Add Business / Update Business Profile store the Country field
             (deck A slide 3: "Country - required, default Canada").
   Without : the site still saves businesses; the Country value entered on the
             form is validated but not stored (AnyminuteBusinessSave detects the
             missing column with COL_LENGTH and skips it).
   Safety  : additive only, nullable column, idempotent (guarded), no data change.
   How     : take a database backup first, then run once in SSMS against the
             AnyMinute database. No application restart needed.
   Rollback: ALTER TABLE dbo.BusinessAddress DROP COLUMN Country;
   ============================================================================ */
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH(N'dbo.BusinessAddress', N'Country') IS NULL
BEGIN
    ALTER TABLE dbo.BusinessAddress ADD Country nvarchar(100) NULL;
    PRINT 'BusinessAddress.Country added.';
END
ELSE
    PRINT 'BusinessAddress.Country already exists - nothing to do.';
COMMIT TRANSACTION;
