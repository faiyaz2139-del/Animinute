using System;
using System.Data;
using System.Data.SqlClient;
using SA = AnyminuteSupportAdmin;

// v72: explicit local-data wipe, one transaction, FK-safe or fail-closed, no remote services.
public static class AnyminuteEverythingWipe
{
    public static DataTable Run(SqlConnection c, string reason)
    {
        if (!SA.IsSupportAdminOnly()) throw new System.Web.HttpException(403, "Only Support Admin may wipe everything.");
        using (var cmd = AnyminuteTimesheetRepair.Command(c, null, Sql,
            "@actor", SA.ActorId(), "@name", SA.ActorName(), "@reason", reason, "@confirm", "WIPE EVERYTHING"))
        {
            cmd.CommandTimeout = 600;
            var result = new DataTable();
            using (var adapter = new SqlDataAdapter(cmd)) adapter.Fill(result);
            return result;
        }
    }
    public const string Sql = @"SET NOCOUNT ON;
SET XACT_ABORT ON;
IF @confirm <> N'WIPE EVERYTHING' OR LEN(LTRIM(RTRIM(ISNULL(@reason,N''))))=0
    THROW 51000, 'Type WIPE EVERYTHING and supply a reason. Nothing was deleted.', 1;
IF OBJECT_ID('dbo.AnyminuteSupportAudit','U') IS NULL
    THROW 51000, 'Support audit table is required. Nothing was deleted.', 1;
BEGIN TRY
 BEGIN TRANSACTION;
 DECLARE @lock int;
 EXEC @lock = sys.sp_getapplock @Resource=N'AnyminuteWipeEverything', @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=0;
 IF @lock < 0 THROW 51000, 'Another wipe is running. Nothing was deleted.', 1;
 CREATE TABLE #wt (Ordinal int IDENTITY PRIMARY KEY, TableName sysname NOT NULL, ObjectId int NULL, Predicate nvarchar(1000) NOT NULL DEFAULT N'1=1', Done bit NOT NULL DEFAULT 0);
 CREATE TABLE #wc (TableName sysname NOT NULL, DeletedRows bigint NOT NULL);
 INSERT #wt(TableName) VALUES
 (N'AnyminuteAccessInviteBusiness'),
 (N'AnyminuteAccessInvite'),
 (N'AnyminuteAccessLog'),
 (N'AnyminuteProjectAssignmentLog'),
 (N'AnyminuteProjectAssignment'),
 (N'AnyminuteProjectDetail'),
 (N'ASSIGNPROJECT'),
 (N'TimeSheetEntry'),
 (N'BreakTime'),
 (N'TimeSheet'),
 (N'TimeCardStatus'),
 (N'AnyminuteWeek'),
 (N'AnyminuteActivity'),
 (N'AnyminuteApprovalAudit'),
 (N'AnyminuteBusinessPolicy'),
 (N'AnyminuteClockOutEstimate'),
 (N'AnyminuteNotification'),
 (N'AnyminuteWeeklySchedule'),
 (N'AnyminuteEmployeeRoleHistory'),
 (N'EmpType_Change_History'),
 (N'AssgineDelegates'),
 (N'AssgineManager'),
 (N'AnyminuteAssignmentDetail'),
 (N'AnyminuteAssignmentRole'),
 (N'AnyminuteRateChangeReason'),
 (N'AnyminuteReportAccess'),
 (N'User_Role_Permission'),
 (N'Role_AssginedLink'),
 (N'AssginLink'),
 (N'AnyminuteTicketMeta'),
 (N'Ticket_Comments'),
 (N'Ticket'),
 (N'AnyminuteResellerSubscriber'),
 (N'AnyminuteReseller'),
 (N'EmailChangeAudit'),
 (N'EmailChangeRequests'),
 (N'AnyminutePasswordResetToken'),
 (N'AnyminuteActiveSession'),
 (N'AnyminuteLoginSecurity'),
 (N'ExternalLogins'),
 (N'Email_Payment_Link'),
 (N'PlanHistory'),
 (N'MappingTable'),
 (N'AssginBussiness'),
 (N'EmpImage'),
 (N'Emp_Active_Inactive_Status'),
 (N'AddProject'),
 (N'UserAcc'),
 (N'AddEmployee'),
 (N'AddBusiness'),
 (N'AnyminuteAddressReview'),
 (N'AddressMaster'),
 (N'StreetMaster'),
 (N'AddEmployee_Archive'),
 (N'AssginBussiness_Archive'),
 (N'UserAcc_01_Aug_2020'),
 (N'Register_Partner'),
 (N'MotherOrg'),
 (N'Files'),
 (N'BusinessAddress'),
 (N'EmpAddress'),
 (N'RateDetails');
 UPDATE #wt SET ObjectId=OBJECT_ID(N'dbo.'+QUOTENAME(TableName),'U');
 -- Absent optional tables are reported as zero, never created.
 INSERT #wc SELECT TableName,0 FROM #wt WHERE ObjectId IS NULL;
 UPDATE #wt SET Done=1 WHERE ObjectId IS NULL;
 DECLARE @table sysname, @obj int, @sql nvarchar(max), @pred nvarchar(1000), @n bigint;
 -- Lock known data tables for the entire atomic wipe, including support-account selection.
 DECLARE locks CURSOR LOCAL FAST_FORWARD FOR SELECT TableName FROM #wt WHERE ObjectId IS NOT NULL ORDER BY TableName;
 OPEN locks; FETCH NEXT FROM locks INTO @table;
 WHILE @@FETCH_STATUS=0
 BEGIN
   SET @sql=N'SELECT @n=COUNT_BIG(*) FROM dbo.'+QUOTENAME(@table)+N' WITH(TABLOCKX,HOLDLOCK)';
   EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT',@n OUTPUT;
   FETCH NEXT FROM locks INTO @table;
 END;
 CLOSE locks; DEALLOCATE locks;
 SELECT UserId,EmployeeId INTO #ku FROM dbo.UserAcc
 WHERE LOWER(REPLACE(LTRIM(RTRIM(ISNULL(UserType,''))),' ','')) IN ('supportadmin','superadmin','support');
 IF NOT EXISTS(SELECT 1 FROM #ku WHERE UserId=@actor)
    THROW 51000, 'The acting login is not a protected Support Admin account. Nothing was deleted.', 1;
 SELECT DISTINCT EmployeeId INTO #ke FROM #ku WHERE EmployeeId IS NOT NULL;
 UPDATE #wt SET Predicate=N'NOT EXISTS(SELECT 1 FROM #ku k WHERE k.UserId=d.UserId)' WHERE TableName IN ('UserAcc','AnyminuteLoginSecurity','AnyminuteActiveSession','ExternalLogins');
 UPDATE #wt SET Predicate=N'NOT EXISTS(SELECT 1 FROM #ku k WHERE k.UserId=d.USERID)' WHERE TableName='MappingTable';
 UPDATE #wt SET Predicate=N'NOT EXISTS(SELECT 1 FROM #ke k WHERE k.EmployeeId=d.EmployeeID)' WHERE TableName='AddEmployee';
 -- Plan definitions survive, but their former reseller/editor references must not.
 IF COL_LENGTH('dbo.AnyminuteSubscriptionPlan','ResellerID') IS NOT NULL
    EXEC(N'UPDATE dbo.AnyminuteSubscriptionPlan SET ResellerID=NULL');
 IF COL_LENGTH('dbo.AnyminuteSubscriptionPlan','UpdatedBy') IS NOT NULL
    EXEC(N'UPDATE dbo.AnyminuteSubscriptionPlan SET UpdatedBy=NULL');
 -- Retain only support login/role shells; detach every address and business reference.
 UPDATE dbo.MappingTable SET BusinessID=NULL WHERE USERID IN(SELECT UserId FROM #ku);
 UPDATE dbo.UserAcc SET MotherOrgid=NULL, LinkAcc=NULL WHERE UserId IN(SELECT UserId FROM #ku);
 UPDATE dbo.AddEmployee SET ADDRESSID=NULL, Rateid=NULL, BusinessID=NULL, MotherOrgID=NULL, MangerId=NULL, BusinessOwnerId=NULL,
     StreetNumber=NULL,StreetName=NULL,Apartment=NULL,City=NULL,Province=NULL,PostalCode=NULL
 WHERE EmployeeID IN(SELECT EmployeeId FROM #ke);
 WHILE EXISTS(SELECT 1 FROM #wt WHERE Done=0)
 BEGIN
   SET @table=NULL;
   SELECT TOP(1) @table=w.TableName,@obj=w.ObjectId,@pred=w.Predicate FROM #wt w
   WHERE w.Done=0 AND NOT EXISTS(
      SELECT 1 FROM sys.foreign_keys fk JOIN #wt child ON child.ObjectId=fk.parent_object_id AND child.Done=0
      WHERE fk.referenced_object_id=w.ObjectId AND fk.parent_object_id<>fk.referenced_object_id AND fk.is_disabled=0)
   ORDER BY w.Ordinal;
   IF @table IS NULL THROW 51000, 'Foreign key cycle found. Wipe rolled back; no constraints were disabled.', 1;
   SET @sql=N'DELETE d FROM dbo.'+QUOTENAME(@table)+N' d WHERE '+@pred+N'; SET @n=@@ROWCOUNT;';
   EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT',@n OUTPUT;
   INSERT #wc VALUES(@table,@n);
   UPDATE #wt SET Done=1 WHERE ObjectId=@obj;
 END;
 -- A trigger must not silently regenerate business/archive records after their delete.
 DECLARE verify_rows CURSOR LOCAL FAST_FORWARD FOR SELECT TableName,Predicate FROM #wt WHERE ObjectId IS NOT NULL;
 OPEN verify_rows; FETCH NEXT FROM verify_rows INTO @table,@pred;
 WHILE @@FETCH_STATUS=0
 BEGIN
   SET @sql=N'SELECT @n=COUNT_BIG(*) FROM dbo.'+QUOTENAME(@table)+N' d WHERE '+@pred;
   EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT',@n OUTPUT;
   IF @n<>0 THROW 51000, 'Rows remained or were regenerated by a trigger. Wipe rolled back.', 1;
   FETCH NEXT FROM verify_rows INTO @table,@pred;
 END;
 CLOSE verify_rows; DEALLOCATE verify_rows;
 DECLARE @summary nvarchar(max)=N'';
 SELECT @summary=@summary+TableName+N': '+CONVERT(nvarchar(30),DeletedRows)+N'; ' FROM #wc ORDER BY TableName;
 INSERT dbo.AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,NewValue,Comments,Success)
 VALUES('Login','wipe-everything','Wipe Everything',@actor,LEFT(@name,150),@summary,LEFT(@reason,1000),1);
 COMMIT TRANSACTION;
 SELECT TableName,DeletedRows FROM #wc ORDER BY TableName;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
";
}
