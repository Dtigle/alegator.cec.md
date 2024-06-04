CREATE TABLE [dbo].[ElectionDay] (
    [ElectionDayId]       BIGINT             IDENTITY (1, 1) NOT NULL,
    [ElectionDayDate]     DATETIMEOFFSET (7) NOT NULL,
    [DeployDbDate]        DATETIMEOFFSET (7) NOT NULL,
    [Name]                NVARCHAR (255)     NOT NULL,
    [Description]         NVARCHAR (500)     NULL,
    [StartDateToReportDb] DATETIME           NULL,
    [EndDateToReportDb]   DATETIME           NULL,
    CONSTRAINT [PK_ElectionDays] PRIMARY KEY CLUSTERED ([ElectionDayId] ASC) WITH (FILLFACTOR = 80)
);

