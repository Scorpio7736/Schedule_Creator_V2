SELECT
    s.id,
    s.fName,
    s.lName,
    a.dayOfTheWeek,
    a.startTime,
    a.endTime
FROM
    [UWGB].[Staff] AS s
INNER JOIN
    [UWGB].[Availability] AS a
ON
    s.id = a.id
WHERE
    a.dayOfTheWeek = @day;