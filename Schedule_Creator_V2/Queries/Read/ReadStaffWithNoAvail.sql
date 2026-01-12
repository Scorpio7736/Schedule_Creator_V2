SELECT
    s.id,
    s.fName,
    s.mName,
    s.lName,
    s.position,
    s.email,
    s.belayCert,
    s.certifiedOn,
    s.expiresOn
FROM UWGB.Staff AS s
WHERE NOT EXISTS (
    SELECT 1
    FROM UWGB.Availability AS a
    WHERE a.id = s.id
);
