CREATE TABLE [dbo].[ElectionResult] (
    [ElectionResultId]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [ElectionRoundId]            BIGINT         NOT NULL,
    [BallotOrder]                INT            CONSTRAINT [DF_ElectionResult_BallotOrder] DEFAULT ((-1)) NOT NULL,
    [BallotCount]                BIGINT         CONSTRAINT [DF_ElectionResult_BallotCount] DEFAULT ((0)) NOT NULL,
    [Comments]                   NVARCHAR (MAX) CONSTRAINT [DF_ElectionResult_Comments] DEFAULT ('No Comment') NOT NULL,
    [DateOfEntry]                DATETIME       NOT NULL,
    [Status]                     INT            CONSTRAINT [DF_ElectionResult_Status] DEFAULT ((0)) NOT NULL,
    [ElectionCompetitorId]       BIGINT         NOT NULL,
    [ElectionCompetitorMemberId] BIGINT         CONSTRAINT [DF_ElectionResult_ElectionCompetitorMemberId] DEFAULT ((-1)) NULL,
    [BallotPaperId]              BIGINT         NOT NULL,
    [EditUserId]                 BIGINT         CONSTRAINT [DF_ElectionResult_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                   DATETIME       CONSTRAINT [DF_ElectionResult_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                    INT            CONSTRAINT [DF_ElectionResult_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ElectionResult] PRIMARY KEY NONCLUSTERED ([ElectionResultId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ElectionResult_BallotPaper] FOREIGN KEY ([BallotPaperId]) REFERENCES [dbo].[BallotPaper] ([BallotPaperId]),
    CONSTRAINT [FK_ElectionResult_ElectionCompetitor] FOREIGN KEY ([ElectionCompetitorId]) REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId]),
    CONSTRAINT [FK_ElectionResult_ElectionCompetitorMember] FOREIGN KEY ([ElectionCompetitorMemberId]) REFERENCES [dbo].[ElectionCompetitorMember] ([ElectionCompetitorMemberId]),
    CONSTRAINT [FK_ElectionResult_ElectionRound_ElectionRoundId] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ElectionResult_BallotPaperId]
    ON [dbo].[ElectionResult]([BallotPaperId] ASC);

