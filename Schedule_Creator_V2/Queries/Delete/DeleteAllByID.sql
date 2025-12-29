DELETE FROM 
	[UWGB].[Availability] 
WHERE 
	id = @id;

DELETE FROM 
	[UWGB].[DaysOff] 
WHERE 
	id = @id;

DELETE FROM 
	[UWGB].[Staff] 
WHERE 
	id = @id;
	
DELETE FROM
	[UWGB].[Staff] 
WHERE 
	staffID = @id;