SELECT
    s.id, s.fName, s.lName, a.dayOfTheWeek, a.startTime, a.endTime
FROM
    [UWGB].[Staff] s
RIGHT JOIN
    [UWGB].[Availability] a
ON

    s.id = a.id
WHERE
    a.dayOfTheWeek = @day