/* AnyMinute v79 - READ-ONLY diagnostic: where does an email exist?
   Run in SSMS against the AnyMinute database. Only SELECTs run; nothing is changed.
   The optional cleanup at the bottom is COMMENTED OUT - review the results first, back up, and run it only if you decide to. */
SET NOCOUNT ON;
DECLARE @m nvarchar(100) = N'thakorfawaz@gmail.com';
DECLARE @k nvarchar(100) = LOWER(LTRIM(RTRIM(@m)));

PRINT '1) Logins (UserAcc) - this is what "already registered" checks';
SELECT u.UserId, u.EmployeeId, u.Email, u.UserTypeid, u.UserType, u.Status, u.Is_First_Time_Login, u.uid, u.MotherOrgid
FROM UserAcc u WHERE LOWER(LTRIM(RTRIM(u.Email))) = @k;

PRINT '2) Profile rows (AddEmployee) behind those logins';
SELECT ae.EmployeeID, ae.FirstName, ae.LastName, ae.EMPLOYEETYPE, ae.UserType, ae.ADDRESSID, ae.MotherOrgID
FROM AddEmployee ae WHERE ae.EmployeeID IN (SELECT EmployeeId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k)
   OR ae.ADDRESSID IN (SELECT EmpAddressId FROM EmpAddress WHERE LOWER(LTRIM(RTRIM(MainEmail))) = @k);

PRINT '3) Contact rows (EmpAddress.MainEmail)';
SELECT ea.EmpAddressId, ea.MainEmail, ea.StreetNo, ea.StreetName, ea.City, ea.Province, ea.Postalcode
FROM EmpAddress ea WHERE LOWER(LTRIM(RTRIM(ea.MainEmail))) = @k;

PRINT '4) Mapping / business-owner / plan rows (missing rows here = the earlier create did not finish)';
SELECT 'MappingTable' T, m.MappingId Id, m.USERID, m.EmployeeID, m.BusinessID FROM MappingTable m
 WHERE m.USERID IN (SELECT UserId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k)
    OR m.EmployeeID IN (SELECT EmployeeId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k);
SELECT 'AssginBussiness' T, a.AssginBussinessId, a.EmployeeID, a.BusinessID, a.EmployeeName FROM AssginBussiness a
 WHERE a.EmployeeID IN (SELECT EmployeeId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k);
SELECT 'PlanHistory' T, p.* FROM PlanHistory p
 WHERE LOWER(LTRIM(RTRIM(p.EmailId))) = @k OR p.EmpId IN (SELECT EmployeeId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k);

PRINT '5) Support audit trail for those logins (Subscriber Created / Temporary Password Issued / Activation Email ...)';
IF OBJECT_ID('dbo.AnyminuteSupportAudit','U') IS NOT NULL
  SELECT s.AuditID, s.EntityKey UserId, s.Action, s.ChangedAtUTC, s.ChangedByName, s.NewStatus, s.NewValue, s.Comments
  FROM AnyminuteSupportAudit s WHERE s.Entity = 'Login'
    AND s.EntityKey IN (SELECT CAST(UserId AS nvarchar(50)) FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k)
  ORDER BY s.ChangedAtUTC, s.AuditID;
IF OBJECT_ID('dbo.AnyminuteLoginSecurity','U') IS NOT NULL
  SELECT * FROM AnyminuteLoginSecurity WHERE UserId IN (SELECT UserId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k);
IF OBJECT_ID('dbo.AnyminuteResellerSubscriber','U') IS NOT NULL
  SELECT * FROM AnyminuteResellerSubscriber WHERE UserId IN (SELECT UserId FROM UserAcc WHERE LOWER(LTRIM(RTRIM(Email))) = @k);

PRINT '6) Every other text column anywhere that holds this email (read-only scan)';
DECLARE @sql nvarchar(max) = N'';
SELECT @sql = @sql + N'SELECT ''' + s.name + '.' + t.name + '.' + c.name + N''' Col, COUNT(*) Hits FROM ' + QUOTENAME(s.name) + '.' + QUOTENAME(t.name)
     + N' WHERE LOWER(LTRIM(RTRIM(CAST(' + QUOTENAME(c.name) + N' AS nvarchar(400))))) = @k HAVING COUNT(*) > 0 UNION ALL '
FROM sys.columns c JOIN sys.tables t ON t.object_id = c.object_id JOIN sys.schemas s ON s.schema_id = t.schema_id
JOIN sys.types ty ON ty.user_type_id = c.user_type_id
WHERE c.name LIKE '%mail%' AND ty.name IN ('varchar','nvarchar','char','nchar');
IF LEN(@sql) > 0 BEGIN SET @sql = LEFT(@sql, LEN(@sql) - 10); EXEC sp_executesql @sql, N'@k nvarchar(100)', @k = @k; END

/* ---------------------------------------------------------------------------------------------------------------
   OPTIONAL CLEANUP - NOT RUN. Only for an ORPHAN left by a failed Add Subscriber attempt, i.e. section 1 shows a
   login with Is_First_Time_Login = 1 that nobody ever used, and section 4 shows it is missing MappingTable and/or
   PlanHistory rows. Prefer Support Admin > Subscriber > Login & Access > Resend Invitation if the account is complete.
   Back up first. Put the exact ids from the results above, then remove the comment markers.

BEGIN TRAN;
DECLARE @UserId int = 0;      -- from section 1
DECLARE @EmployeeId int = 0;  -- from section 1 / 2
IF @UserId <= 0 OR @EmployeeId <= 0 BEGIN RAISERROR('Set @UserId and @EmployeeId first.',16,1); ROLLBACK; RETURN; END
DELETE FROM MappingTable WHERE USERID = @UserId OR EmployeeID = @EmployeeId;
IF OBJECT_ID('dbo.AnyminuteLoginSecurity','U') IS NOT NULL DELETE FROM AnyminuteLoginSecurity WHERE UserId = @UserId;
IF OBJECT_ID('dbo.AnyminuteResellerSubscriber','U') IS NOT NULL DELETE FROM AnyminuteResellerSubscriber WHERE UserId = @UserId;
DELETE FROM PlanHistory WHERE EmpId = @EmployeeId AND TransId = 'SUPPORT';
DELETE FROM AssginBussiness WHERE EmployeeID = @EmployeeId;
DELETE FROM UserAcc WHERE UserId = @UserId AND EmployeeId = @EmployeeId;
DELETE ea FROM EmpAddress ea JOIN AddEmployee ae ON ae.ADDRESSID = ea.EmpAddressId WHERE ae.EmployeeID = @EmployeeId;
DELETE FROM AddEmployee WHERE EmployeeID = @EmployeeId;
-- check the row counts above, then COMMIT; (or ROLLBACK; to undo)
-- COMMIT;
--------------------------------------------------------------------------------------------------------------- */
