CREATE TABLE [UWGB].[Availability] (
    [id]           INT      NOT NULL,
    [dayOfTheWeek] INT      NOT NULL,
    [startTime]    TIME (2) NOT NULL,
    [endTime]      TIME (2) NOT NULL
);

CREATE TABLE [UWGB].[DaysOff] (
    [id]     INT           NOT NULL,
    [Date]   DATE          NOT NULL,
    [reason] NVARCHAR (50) NOT NULL
);

CREATE TABLE [UWGB].[JobSettings] (
    [DayOfWeek]   NVARCHAR (50) NOT NULL,
    [OpeningTime] TIME (2)      NOT NULL,
    [ClosingTime] TIME (2)      NOT NULL
);

CREATE TABLE [UWGB].[Schedule] (
    [dayOfWeek]    NVARCHAR (50) NOT NULL,
    [staffID]      INT           NOT NULL,
    [scheduleName] NVARCHAR (50) NOT NULL
);

CREATE TABLE [UWGB].[Staff] (
    [id]          INT           IDENTITY (1, 1) NOT NULL,
    [fName]       NVARCHAR (50) NOT NULL,
    [mName]       NVARCHAR (50) NULL,
    [lName]       NVARCHAR (50) NOT NULL,
    [position]    NVARCHAR (50) NOT NULL,
    [email]       NVARCHAR (50) NOT NULL,
    [belayCert]   NVARCHAR (50) NOT NULL,
    [certifiedOn] DATE          NULL,
    [expiresOn]   DATE          NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);