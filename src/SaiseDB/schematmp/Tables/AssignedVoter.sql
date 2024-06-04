CREATE TABLE [schematmp].[AssignedVoter] (
    [AssignedVoterId]            BIGINT         NOT NULL,
    [RegionId]                   BIGINT         NOT NULL,
    [RequestingPollingStationId] BIGINT         NOT NULL,
    [PollingStationId]           BIGINT         NOT NULL,
    [VoterId]                    BIGINT         NOT NULL,
    [Category]                   BIGINT         NOT NULL,
    [Status]                     BIGINT         NOT NULL,
    [Comment]                    NVARCHAR (MAX) NULL,
    [ElectionListNr]             BIGINT         NULL,
    [EditUserId]                 BIGINT         NOT NULL,
    [EditDate]                   DATETIME       NOT NULL,
    [Version]                    INT            NOT NULL,
    CONSTRAINT [PK_AssignedVoter] PRIMARY KEY CLUSTERED ([AssignedVoterId] ASC) WITH (FILLFACTOR = 80)
);

