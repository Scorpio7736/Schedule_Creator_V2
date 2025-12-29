UPDATE 
	[UWGB].[Staff] 
SET 
	fName = @fName, 
	mName = @mName, 
	lName = @lName, 
	position = @position, 
	email = @email 
WHERE 
	id = @id