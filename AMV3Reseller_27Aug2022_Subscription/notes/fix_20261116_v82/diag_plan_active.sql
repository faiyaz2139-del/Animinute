-- AnyMinute v82 diag: why does Add Business say "Your plan is not active..." for subscriber Fawaz?
-- READ-ONLY. SELECT statements only: no INSERT/UPDATE/DELETE, no temp tables. Password and payment-link URL columns are not selected.
-- Edit @Name / @Email if needed, then run against the AnyMinute DB on the Montreal instance.
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
DECLARE @Name  nvarchar(100) = N'Fawaz%';   -- first-name pattern
DECLARE @Email nvarchar(200) = N'';         -- optional login email (exact, case-insensitive); leave '' to match by name only

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
)
-- 1) Who matched: login + employee flags. Only UserTypeid=1 / UserType=1 rows are subscribers (business owners).
SELECT '1_login' AS q, ae.EmployeeID, ae.FirstName, ae.LastName, ae.UserType AS AE_UserType, ae.IsActive AS AE_IsActive, ae.EMPLOYEETYPE,
       ae.MotherOrgID AS AE_MotherOrgID, u.UserId, u.Email, u.UserTypeid, u.UserType, u.Status AS UA_Status, u.Is_First_Time_Login, u.MotherOrgid AS UA_MotherOrgid
  FROM cand c JOIN AddEmployee ae WITH (NOLOCK) ON ae.EmployeeID = c.EmployeeID
  LEFT JOIN UserAcc u WITH (NOLOCK) ON u.EmployeeId = ae.EmployeeID
 ORDER BY ae.EmployeeID, u.UserId;

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
)
-- 2) Which Add Business branch runs: reseller-subscriber if the MotherOrg email is a Register_Partner (reseller) email.
SELECT DISTINCT '2_branch' AS q, c.EmployeeID, ab.MotherOrgID AS AB_MotherOrgID, mo.MotherOrgName, mo.Email AS MotherOrg_Email,
       CASE WHEN EXISTS (SELECT 1 FROM Register_Partner rp WITH (NOLOCK) WHERE rp.EMail_primary = mo.Email) THEN 'RESELLER-SUBSCRIBER branch (gate uses reseller PlanHistory, EmpId=MotherOrgID)'
            ELSE 'DIRECT SUBSCRIBER branch (gate uses own PlanHistory, EmpId=EmployeeID)' END AS add_business_branch
  FROM cand c
  LEFT JOIN AssginBussiness ab WITH (NOLOCK) ON ab.EmployeeID = c.EmployeeID AND ab.MotherOrgID IS NOT NULL
  LEFT JOIN UserAcc u WITH (NOLOCK) ON u.EmployeeId = c.EmployeeID
  LEFT JOIN MotherOrg mo WITH (NOLOCK) ON mo.MotherOrgID = COALESCE(ab.MotherOrgID, u.MotherOrgid);

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
)
-- 3) Every PlanHistory row for the subscriber (EmpId = EmployeeID).
SELECT '3_plan_rows' AS q, ph.Id, ph.EmpId, ph.EmailId, ph.PlanId, ph.PlanName, ph.StartDate, ph.EndDate, ph.PlanActivationDate, ph.AmountPaid, ph.TransId,
       ph.No_Of_Users, ph.Business, ph.IsActive, ph.Is_Cancelled, ph.Is_Requested_Plan, ph.MotherOrgID, ph.RefundAmt, ph.RefundDate
  FROM PlanHistory ph WITH (NOLOCK) JOIN cand c ON ph.EmpId = c.EmployeeID
 ORDER BY ph.EmpId, ph.Id;

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
)
-- 4) The exact Add Business gate counters (direct-subscriber branch), old v81 rule vs v82 rule.
SELECT '4_gate_direct' AS q, c.EmployeeID,
       SUM(CASE WHEN ph.IsActive = 1 THEN 1 ELSE 0 END)          AS active_rows,       -- 0  => "Please contact admin. Your plan is not active."
       SUM(CASE WHEN ph.Is_Cancelled = 1 THEN 1 ELSE 0 END)      AS cancelled_rows,    -- >0 => "Your plan has been cancelled."
       COUNT(ph.Id)                                              AS total_rows,
       SUM(CASE WHEN ph.Is_Requested_Plan = 1 THEN 1 ELSE 0 END) AS requested_rows,
       CASE WHEN SUM(CASE WHEN ph.IsActive=1 THEN 1 ELSE 0 END) > 0 AND SUM(CASE WHEN ph.Is_Cancelled=1 THEN 1 ELSE 0 END) = 0
                 AND COUNT(ph.Id) = 1 AND SUM(CASE WHEN ph.Is_Requested_Plan=1 THEN 1 ELSE 0 END) = 0 THEN 'BLOCKED' ELSE 'ok' END AS v81_pay_msg,
       CASE WHEN SUM(CASE WHEN ph.IsActive=1 THEN 1 ELSE 0 END) > 0 AND SUM(CASE WHEN ph.Is_Cancelled=1 THEN 1 ELSE 0 END) = 0
                 AND COUNT(ph.Id) = 1 AND SUM(CASE WHEN ph.Is_Requested_Plan=1 THEN 1 ELSE 0 END) = 1 THEN 'BLOCKED (unpaid requested plan)' ELSE 'ok' END AS v82_pay_msg
  FROM cand c LEFT JOIN PlanHistory ph WITH (NOLOCK) ON ph.EmpId = c.EmployeeID
 GROUP BY c.EmployeeID;

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
), mos AS (
    SELECT DISTINCT ab.MotherOrgID FROM cand c JOIN AssginBussiness ab WITH (NOLOCK) ON ab.EmployeeID = c.EmployeeID WHERE ab.MotherOrgID IS NOT NULL
    UNION SELECT u.MotherOrgid FROM cand c JOIN UserAcc u WITH (NOLOCK) ON u.EmployeeId = c.EmployeeID WHERE u.MotherOrgid IS NOT NULL
)
-- 5) Reseller-subscriber branch: the reseller's own PlanHistory (EmpId = MotherOrgID). 1 row + Is_Requested_Plan=1 => the message (reseller unpaid; legit gate).
SELECT '5_reseller_plan' AS q, ph.Id, ph.EmpId AS MotherOrgID, ph.PlanName, ph.StartDate, ph.EndDate, ph.AmountPaid, ph.TransId,
       ph.No_Of_Users, ph.Business, ph.IsActive, ph.Is_Cancelled, ph.Is_Requested_Plan,
       (SELECT COUNT(*) FROM PlanHistory p2 WITH (NOLOCK) WHERE p2.EmpId = ph.EmpId) AS reseller_total_rows
  FROM PlanHistory ph WITH (NOLOCK) JOIN mos ON ph.EmpId = mos.MotherOrgID
 ORDER BY ph.EmpId, ph.Id;

;WITH cand AS (
    SELECT ae.EmployeeID FROM AddEmployee ae WITH (NOLOCK)
     WHERE ae.FirstName LIKE @Name OR (LTRIM(RTRIM(ISNULL(ae.FirstName,'')))+' '+LTRIM(RTRIM(ISNULL(ae.LastName,'')))) LIKE @Name
    UNION
    SELECT u.EmployeeId FROM UserAcc u WITH (NOLOCK) WHERE @Email <> N'' AND LOWER(LTRIM(RTRIM(u.Email))) = LOWER(LTRIM(RTRIM(@Email)))
)
-- 6) Payment links ever emailed to this subscriber (URL column intentionally not selected).
SELECT '6_payment_links' AS q, pl.Id, pl.EmployeeID, pl.Name, pl.Email, pl.Payment_Amount, pl.Reseller_Or_Subscriber
  FROM Email_Payment_Link pl WITH (NOLOCK) JOIN cand c ON pl.EmployeeID = c.EmployeeID
 ORDER BY pl.Id;
