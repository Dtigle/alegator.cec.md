CREATE TABLE [schematmp].[ElectionDay] (
    [ElectionDayId]   BIGINT             NOT NULL,
    [ElectionDayDate] DATETIMEOFFSET (7) NOT NULL,
    [DeployDbDate]    DATETIMEOFFSET (7) NOT NULL,
    [Name]            NVARCHAR (255)     NOT NULL,
    [Description]     NVARCHAR (500)     NULL,
    CONSTRAINT [PK_ElectionDays] PRIMARY KEY CLUSTERED ([ElectionDayId] ASC) WITH (FILLFACTOR = 80)
);

