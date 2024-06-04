CREATE TABLE [dbo].[Alerts] (
    [AlertId]              BIGINT             IDENTITY (1, 1) NOT NULL,
    [VoterId]              BIGINT             NOT NULL,
    [FirstName]            NVARCHAR (100)     NULL,
    [LastName]             NVARCHAR (100)     NULL,
    [Patronymic]           NVARCHAR (100)     NULL,
    [Idnp]                 BIGINT             NULL,
    [DateOfBirth]          DATETIME           NULL,
    [Adress]               NVARCHAR (MAX)     NULL,
    [DocumentNumber]       NVARCHAR (50)      NULL,
    [PollingStationId]     BIGINT             NOT NULL,
    [PollingStationAdress] NVARCHAR (100)     NULL,
    [DateRegistration]     DATETIMEOFFSET (7) NULL,
    [EditUserId]           BIGINT             NOT NULL,
    [EditDate]             DATETIME           NOT NULL,
    [Version]              INT                NOT NULL,
    CONSTRAINT [PK_Alerts] PRIMARY KEY CLUSTERED ([AlertId] ASC),
    CONSTRAINT [FK_Alerts_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId]),
    CONSTRAINT [FK_Alerts_Voter] FOREIGN KEY ([VoterId]) REFERENCES [dbo].[Voter] ([VoterId])
);

