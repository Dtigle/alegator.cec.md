CREATE TABLE [schematmp].[ElectionResult] (
    [ElectionResultId]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [ElectionRoundId]            BIGINT         NOT NULL,
    [BallotOrder]                INT            NOT NULL,
    [BallotCount]                BIGINT         NOT NULL,
    [Comments]                   NVARCHAR (MAX) NOT NULL,
    [DateOfEntry]                DATETIME       NOT NULL,
    [Status]                     INT            NOT NULL,
    [ElectionCompetitorId]       BIGINT         NOT NULL,
    [ElectionCompetitorMemberId] BIGINT         NULL,
    [BallotPaperId]              BIGINT         NOT NULL,
    [EditUserId]                 BIGINT         NOT NULL,
    [EditDate]                   DATETIME       NOT NULL,
    [Version]                    INT            NOT NULL,
    CONSTRAINT [PK_ElectionResult] PRIMARY KEY NONCLUSTERED ([ElectionResultId] ASC) WITH (FILLFACTOR = 80)
);

