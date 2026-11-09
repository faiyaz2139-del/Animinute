/* AnyMinute v75 - STANDALONE DESTRUCTIVE WIPE (based on v72). NOT a deployment migration.
   BACK UP FIRST: take and VERIFY a fresh backup. Review the target database before executing.
   Deletes local database subscriber/reseller/business/employee/billing/address records.
   Keeps SupportAdmin / Support Admin / SuperAdmin / Support UserAcc logins, their employee/role
   shells and active authentication records, SupportAdmin table, subscription plan definitions,
   shared geography/menu/role/configuration masters, and the Support audit history (adds counts).
   No Stripe API calls, remote payment cancellation, physical-file deletion. Safe empty-table identity reseeding is included below.
   Known tables are explicit below; absent optional tables = zero. Runtime FK ordering; cycles or
   unknown FK dependencies fail and roll back. Constraints are never disabled.
   Set @actor to an existing protected UserAcc.UserId, @name, @reason, @confirm intentionally.
   Default confirmation is blank, so accidental execution performs NO deletes.
   This v75 standalone script supersedes the v72 manual script for clean testing.
   The compiled v72 wipe class is historical and no longer reachable from the UI; it is NOT changed.
   Known tables are explicitly classified. Any unclassified user table aborts rather than leaving
   unknown module data behind. Review/classify an installed-schema difference before using this.
   Seeds reset ONLY on EMPTY tables with integral identity columns and safe seed/increment bounds.
   Tables retaining support accounts, plan definitions, audit or master rows are NOT reseeded.
   Already-empty identities with non-NULL history are conservatively skipped: the next-value
   semantics after manual reseed/TRUNCATE cannot be proved from last_value alone.
*/
DECLARE @actor int = 0, @name nvarchar(150)=N'', @reason nvarchar(1000)=N'', @confirm nvarchar(40)=N'';
SET NOCOUNT ON;
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
 CREATE TABLE #wr (TableName sysname NOT NULL, ReseedStatus nvarchar(200) NOT NULL);
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
 -- Fail closed if this installation has tables not covered by this reviewed script.
 CREATE TABLE #keepmaster(TableName sysname PRIMARY KEY);
 INSERT #keepmaster VALUES (N'AnyminuteSubscriptionPlan'), (N'AnyminuteSupportAudit'), (N'SupportAdmin'), (N'CityMaster'), (N'ProvinceMaster'), (N'AdminUserType'), (N'CodeType'), (N'DefaultMotherOrg'), (N'DomainMaster'), (N'EmailSeting'), (N'LinkMaster'), (N'Map_Menu_Page_User'), (N'Mst_Menu'), (N'Mst_Page'), (N'Role_Permission_Master'), (N'TemplateMaster'), (N'TempleteType'), (N'UserType'), (N'Role_DomainMaster');
 IF EXISTS(SELECT 1 FROM sys.tables t JOIN sys.schemas sc ON sc.schema_id=t.schema_id
    WHERE t.is_ms_shipped=0 AND NOT(sc.name='dbo' AND
       (EXISTS(SELECT 1 FROM #wt w WHERE w.TableName=t.name) OR EXISTS(SELECT 1 FROM #keepmaster k WHERE k.TableName=t.name))))
 BEGIN
    SELECT sc.name AS UnclassifiedSchema,t.name AS UnclassifiedTable FROM sys.tables t JOIN sys.schemas sc ON sc.schema_id=t.schema_id
    WHERE t.is_ms_shipped=0 AND NOT(sc.name='dbo' AND
       (EXISTS(SELECT 1 FROM #wt w WHERE w.TableName=t.name) OR EXISTS(SELECT 1 FROM #keepmaster k WHERE k.TableName=t.name)));
    THROW 51000, 'Unclassified user tables found. Nothing deleted. Review module coverage before running this wipe.', 1;
 END;
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
 -- Safe identity reset: DELETE has completed and all target predicates have been verified.
 -- Preserve identity sequences on tables with retained rows. Never TRUNCATE or disable FKs.
 DECLARE @seed decimal(38,0),@increment decimal(38,0),@last decimal(38,0),@reset decimal(38,0),@kind sysname;
 DECLARE identities CURSOR LOCAL FAST_FORWARD FOR
    SELECT w.TableName,CONVERT(decimal(38,0),ic.seed_value),CONVERT(decimal(38,0),ic.increment_value),
           CONVERT(decimal(38,0),ic.last_value),TYPE_NAME(ic.system_type_id)
    FROM #wt w JOIN sys.identity_columns ic ON ic.object_id=w.ObjectId;
 OPEN identities; FETCH NEXT FROM identities INTO @table,@seed,@increment,@last,@kind;
 WHILE @@FETCH_STATUS=0
 BEGIN
   SET @sql=N'SELECT @n=COUNT_BIG(*) FROM dbo.'+QUOTENAME(@table)+N' WITH(TABLOCKX,HOLDLOCK)';
   EXEC sys.sp_executesql @sql,N'@n bigint OUTPUT',@n OUTPUT;
   IF @n<>0 INSERT #wr VALUES(@table,N'Kept: table retains protected rows');
   ELSE IF @last IS NULL INSERT #wr VALUES(@table,N'Already pristine: next value is original seed');
   ELSE IF NOT EXISTS(SELECT 1 FROM #wc WHERE TableName=@table AND DeletedRows>0)
       INSERT #wr VALUES(@table,N'Skipped: already empty; prior identity lifecycle cannot be proven safely');
   ELSE IF @kind NOT IN('tinyint','smallint','int','bigint') OR @increment=0
       INSERT #wr VALUES(@table,N'Skipped: nonstandard identity requires manual review');
   ELSE
   BEGIN
      SET @reset=@seed-@increment;
      IF (@kind='tinyint' AND (@reset<0 OR @reset>255)) OR
         (@kind='smallint' AND (@reset< -32768 OR @reset>32767)) OR
         (@kind='int' AND (@reset< -2147483648 OR @reset>2147483647)) OR
         (@kind='bigint' AND (@reset< CONVERT(decimal(38,0),'-9223372036854775808') OR @reset>CONVERT(decimal(38,0),'9223372036854775807')))
         INSERT #wr VALUES(@table,N'Skipped: seed minus increment is outside identity type range');
      ELSE
      BEGIN
         SET @sql=N'DBCC CHECKIDENT (N''dbo.'+QUOTENAME(@table)+N''', RESEED, '+CONVERT(nvarchar(50),@reset)+N') WITH NO_INFOMSGS;';
         EXEC sys.sp_executesql @sql;
         INSERT #wr VALUES(@table,N'Reseeded to '+CONVERT(nvarchar(50),@reset)+N'; next value '+CONVERT(nvarchar(50),@seed));
      END;
   END;
   FETCH NEXT FROM identities INTO @table,@seed,@increment,@last,@kind;
 END;
 CLOSE identities; DEALLOCATE identities;
 DECLARE @summary nvarchar(max)=N'';
 SELECT @summary=@summary+w.TableName+N': '+CONVERT(nvarchar(30),w.DeletedRows)+N' ['+ISNULL(r.ReseedStatus,N'No identity')+N']; '
 FROM #wc w LEFT JOIN #wr r ON r.TableName=w.TableName ORDER BY w.TableName;
 INSERT dbo.AnyminuteSupportAudit(Entity,EntityKey,Action,ChangedBy,ChangedByName,NewValue,Comments,Success)
 VALUES('Login','wipe-everything','Wipe Everything',@actor,LEFT(@name,150),@summary,LEFT(@reason,1000),1);
 COMMIT TRANSACTION;
 -- Result grid plus printed per-table counts, ONLY after a successful commit.
 SELECT w.TableName,w.DeletedRows,ISNULL(r.ReseedStatus,N'No identity / table absent') AS IdentityReset
 FROM #wc w LEFT JOIN #wr r ON r.TableName=w.TableName ORDER BY w.TableName;
 DECLARE report CURSOR LOCAL FAST_FORWARD FOR SELECT TableName,DeletedRows FROM #wc ORDER BY TableName;
 OPEN report; FETCH NEXT FROM report INTO @table,@n;
 WHILE @@FETCH_STATUS=0
 BEGIN
    PRINT @table+N': '+CONVERT(nvarchar(30),@n)+N' rows deleted';
    FETCH NEXT FROM report INTO @table,@n;
 END;
 CLOSE report; DEALLOCATE report;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
