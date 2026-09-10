USE Schedule_Creator_V2

SELECT
	s.id,
	s.position,
	CASE a.dayOfTheWeek
    WHEN 1 THEN 'Monday'
    WHEN 2 THEN 'Tuesday'
    WHEN 3 THEN 'Wednesday'
    WHEN 4 THEN 'Thursday'
    WHEN 5 THEN 'Friday'
    WHEN 6 THEN 'Saturday'
    WHEN 7 THEN 'Sunday'
    ELSE 'Unknown'
END as dayOfTheWeek,
	a.startTime,
	a.endTime
FROM
	[UWGB].[Staff] as s
JOIN
	[UWGB].[Availability] as a
	on
	s.id = a.id
WHERE
	s.belayCert = 'True'
	AND
	s.position != 'SUB'