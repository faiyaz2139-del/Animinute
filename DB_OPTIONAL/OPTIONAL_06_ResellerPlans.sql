-- AnyMinute v60 (2026-11-04, docx t190ua0 #3 Reseller PPT s9 "+ Add Plan"): reseller-owned subscription plans.
-- Additive and idempotent: adds one NULLable column to dbo.AnyminuteSubscriptionPlan (run after REQUIRED_07 and REQUIRED_09).
-- NULL = AnyMinute catalogue plan (all existing rows, unchanged). A value = plan created by that reseller in the Reseller portal;
-- only that reseller (and Support Admin) sees it. Until this runs, the Reseller "+ Add Plan" form explains that a database update is needed.
SET NOCOUNT ON; SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON;
IF OBJECT_ID('dbo.AnyminuteSubscriptionPlan','U') IS NOT NULL AND COL_LENGTH('dbo.AnyminuteSubscriptionPlan','ResellerID') IS NULL
ALTER TABLE dbo.AnyminuteSubscriptionPlan ADD ResellerID int NULL;
GO
IF OBJECT_ID('dbo.AnyminuteSubscriptionPlan','U') IS NOT NULL AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name='IX_AnyminuteSubscriptionPlan_Reseller')
CREATE INDEX IX_AnyminuteSubscriptionPlan_Reseller ON dbo.AnyminuteSubscriptionPlan(ResellerID);
GO
PRINT 'OPTIONAL_06_ResellerPlans applied.';
