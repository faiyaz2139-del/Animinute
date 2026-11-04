-- v61-20261104 OPTIONAL_07: READ-ONLY preview of test-looking subscriber owners (safe to re-run; SELECT only, changes nothing).
-- Run before using Support Admin > Subscriber Management > Clean Test Data, and take a full DB backup before cleaning.
-- The in-app filter is a little wider (regex, qa/temp/tmp as whole words); this is a quick server-side cross-check.
SELECT u.UserId, u.EmployeeId, u.Email, ISNULL(u.UserType,'') UserType,
       LTRIM(RTRIM(ISNULL(ae.FirstName,'')+' '+ISNULL(ae.LastName,''))) Name,
       (SELECT COUNT(*) FROM PlanHistory p WHERE (p.EmailId=u.Email OR p.EmpId=CAST(u.EmployeeId AS nvarchar(50)))
           AND ISNULL(CAST(p.AmountPaid AS nvarchar(50)),'') NOT IN ('','0','0.0','0.00','0.000')) PaidRows,
       CASE WHEN OBJECT_ID('dbo.AnyminuteReseller') IS NULL THEN 0 ELSE 1 END ResellerTableExists
FROM UserAcc u LEFT JOIN AddEmployee ae ON ae.EmployeeID=u.EmployeeId
WHERE EXISTS(SELECT 1 FROM MappingTable m WHERE m.USERID=u.UserId AND m.UserTypeId=1)
  AND ISNULL(u.UserType,'') NOT IN ('SupportAdmin','SuperAdmin','Support Admin')
  AND (u.Email LIKE '%test%' OR u.Email LIKE '%demo%' OR u.Email LIKE '%dummy%' OR u.Email LIKE '%fake%' OR u.Email LIKE '%sample%'
       OR u.Email LIKE '%@example.%' OR u.Email LIKE '%mailinator.%' OR u.Email LIKE '%yopmail.%' OR u.Email LIKE '%guerrillamail.%' OR u.Email LIKE '%tempmail%'
       OR ae.FirstName LIKE '%test%' OR ae.LastName LIKE '%test%' OR ae.FirstName LIKE '%demo%' OR ae.LastName LIKE '%demo%')
ORDER BY u.UserId DESC;
