DELETE FROM
    [UWGB].[Schedule]
WHERE
    scheduleName = @scheduleName
INSERT INTO
    [UWGB].[Schedule]
    ([dayOfWeek], staffID, scheduleName)
VALUES
    (@dayOfWeek, @staffID, @scheduleName)