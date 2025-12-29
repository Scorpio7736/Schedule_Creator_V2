SELECT 
    *
FROM 
    [UWGB].[Staff]
WHERE 
    id IN (
        SELECT 
            id
        FROM 
            [UWGB].[Availability]
        WHERE 
            dayOfTheWeek = @dayOfTheWeek
);