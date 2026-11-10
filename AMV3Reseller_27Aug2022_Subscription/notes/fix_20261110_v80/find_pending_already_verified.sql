/* AnyMinute v80 - READ-ONLY. Lists Pending Verification rows whose address is already in the Verified Address Database
   (same postal code ignoring spaces/dashes/case, same street ignoring case and a leading street number).
   Nothing below changes data. The optional cleanup at the bottom is commented out: review the list first, then run it
   yourself only if you want those duplicate pending rows gone. */
SET NOCOUNT ON;
;WITH P AS (
    SELECT a.AddressMasterId, a.Street_Name, a.City, a.Province, a.PostalCode, r.OrigStreet, r.SubmittedBy, r.SubmittedAtUTC,
           REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','') PC,
           UPPER(LTRIM(RTRIM(a.Street_Name))) S,
           UPPER(LTRIM(RTRIM(CASE WHEN a.Street_Name LIKE '[0-9]% %' THEN SUBSTRING(LTRIM(a.Street_Name), CHARINDEX(' ', LTRIM(a.Street_Name)) + 1, 400) ELSE a.Street_Name END))) S2
    FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId = a.AddressMasterId
    WHERE ISNULL(a.Verifed,0) <> 1 AND ISNULL(r.Status,'Pending') = 'Pending')
SELECT p.AddressMasterId PendingId, p.Street_Name, p.OrigStreet, p.City PendingCity, p.Province PendingProv, p.PostalCode, p.SubmittedBy, p.SubmittedAtUTC,
       v.AddressMasterId VerifiedId, v.Street_Name VerifiedStreet, COALESCE(ci.CityName, v.City) VerifiedCity, COALESCE(pr.ProvinceName, v.Province) VerifiedProv, v.PostalCode VerifiedPostal
FROM P p
CROSS APPLY (SELECT TOP 1 v.* FROM AddressMaster v
             WHERE v.Verifed = 1 AND REPLACE(REPLACE(UPPER(v.PostalCode),' ',''),'-','') = p.PC
               AND UPPER(LTRIM(RTRIM(v.Street_Name))) IN (p.S, p.S2) ORDER BY v.AddressMasterId) v
LEFT JOIN CityMaster ci ON CAST(ci.CityId AS nvarchar(50)) = CAST(v.City AS nvarchar(50))
LEFT JOIN ProvinceMaster pr ON CAST(pr.ProvinceId AS nvarchar(50)) = CAST(v.Province AS nvarchar(50))
ORDER BY p.SubmittedAtUTC DESC, p.AddressMasterId DESC;

-- Also read-only: pending rows still holding City/Province '0' or an id (legacy Sp_InsertEmpAddress / Sp_InsertBusinessAddress rows).
SELECT a.AddressMasterId, a.Street_Name, a.City, a.Province, a.PostalCode, r.SubmittedAtUTC
FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId = a.AddressMasterId
WHERE ISNULL(a.Verifed,0) <> 1 AND ISNULL(r.Status,'Pending') = 'Pending'
  AND (ISNULL(a.City,'') IN ('','0') OR ISNULL(a.Province,'') IN ('','0'))
ORDER BY a.AddressMasterId DESC;

/* ---- OPTIONAL CLEANUP (NOT RUN). Removes only the Pending rows listed by the first query. Uncomment to use. ----
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @dup TABLE (Id int PRIMARY KEY);
INSERT @dup(Id)
SELECT a.AddressMasterId
FROM AddressMaster a LEFT JOIN AnyminuteAddressReview r ON r.AddressMasterId = a.AddressMasterId
WHERE ISNULL(a.Verifed,0) <> 1 AND ISNULL(r.Status,'Pending') = 'Pending'
  AND EXISTS (SELECT 1 FROM AddressMaster v
              WHERE v.Verifed = 1 AND REPLACE(REPLACE(UPPER(v.PostalCode),' ',''),'-','') = REPLACE(REPLACE(UPPER(a.PostalCode),' ',''),'-','')
                AND UPPER(LTRIM(RTRIM(v.Street_Name))) IN (UPPER(LTRIM(RTRIM(a.Street_Name))),
                    UPPER(LTRIM(RTRIM(CASE WHEN a.Street_Name LIKE '[0-9]% %' THEN SUBSTRING(LTRIM(a.Street_Name), CHARINDEX(' ', LTRIM(a.Street_Name)) + 1, 400) ELSE a.Street_Name END)))));
SELECT COUNT(*) AS rows_to_remove FROM @dup;
DELETE r FROM AnyminuteAddressReview r JOIN @dup d ON d.Id = r.AddressMasterId;
DELETE a FROM AddressMaster a JOIN @dup d ON d.Id = a.AddressMasterId WHERE ISNULL(a.Verifed,0) <> 1;
COMMIT TRANSACTION;   -- or ROLLBACK TRANSACTION to undo
---- end optional cleanup ---- */
