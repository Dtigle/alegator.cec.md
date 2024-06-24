CREATE TABLE [Audit].[ElectionResult_AUD] (
    [ElectionResultId]           BIGINT         NOT NULL,
    [ElectionRoundId]            BIGINT         NOT NULL,
    [REV]                        INT            NOT NULL,
    [REVTYPE]                    TINYINT        NOT NULL,
    [BallotOrder]                INT            NULL,
    [BallotCount]                BIGINT         NULL,
    [Comments]                   NVARCHAR (MAX) NULL,
    [DateOfEntry]                DATETIME       NULL,
    [Status]                     INT            NULL,
    [ElectionCompetitorId]       BIGINT         NULL,
    [ElectionCompetitorMemberId] BIGINT         NULL,
    [BallotPaperId]              BIGINT         NULL,
    [EditUserId]                 BIGINT         NULL,
    [EditDate]                   DATETIME       NULL,
    [Version]                    INT            NULL,
    CONSTRAINT [PK_ElectionResult_AUD] PRIMARY KEY NONCLUSTERED ([ElectionResultId] ASC, [REV] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ElectionResult_AUD_REVINFO] FOREIGN KEY ([REV]) REFERENCES [Audit].[REVINFO] ([REV])
);

