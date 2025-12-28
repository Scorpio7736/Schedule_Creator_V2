CREATE DATABASE 
	Schedule_Creator_V2
ON 
	(
		NAME = Schedule_Creator_V2, 
		FILENAME = '{databaseFileName}'
	) 
LOG ON 
	(
		NAME = Schedule_Creator_V2_Log, 
		FILENAME = '{logFileName}')