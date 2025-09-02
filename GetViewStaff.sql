SELECT
	staff.id,
	staff.fName,
	staff.mName,
	staff.lName,
	pos.position
FROM
	[UWGB].[Staff] staff
INNER JOIN
	[UWGB].[Position] pos
ON
	staff.id = pos.id


