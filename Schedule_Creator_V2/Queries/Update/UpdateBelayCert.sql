UPDATE 
    [UWGB].[Staff]
SET 
    belayCert = @belayCert,
    certifiedOn = @certifiedOn,
    expiresOn = @expiresOn
WHERE 
    id = @id