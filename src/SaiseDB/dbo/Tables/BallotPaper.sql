CREATE TABLE [dbo].[BallotPaper] (
    [BallotPaperId]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [PollingStationId]       BIGINT         NULL,
    [ElectionRoundId]        BIGINT         NULL,
    [EntryLevel]             INT            CONSTRAINT [DF_BallotPaper_EntryLevel] DEFAULT ((-1)) NOT NULL,
    [Type]                   INT            CONSTRAINT [DF_BallotPaper_Type_1] DEFAULT ((1)) NOT NULL,
    [Status]                 INT            CONSTRAINT [DF_BallotPaper_Status] DEFAULT ((0)) NOT NULL,
    [RegisteredVoters]       BIGINT         CONSTRAINT [DF_BallotPaper_RegisteredVoters] DEFAULT ((0)) NOT NULL,
    [Supplementary]          BIGINT         CONSTRAINT [DF_BallotPaper_Supplementary] DEFAULT ((0)) NOT NULL,
    [BallotsIssued]          BIGINT         CONSTRAINT [DF_BallotPaper_BallotsIssued] DEFAULT ((0)) NOT NULL,
    [BallotsCasted]          BIGINT         CONSTRAINT [DF_BallotPaper_BallotsCasted] DEFAULT ((0)) NOT NULL,
    [DifferenceIssuedCasted] BIGINT         CONSTRAINT [DF_BallotPaper_DifferenceIssuedCasted] DEFAULT ((0)) NOT NULL,
    [BallotsValidVotes]      BIGINT         CONSTRAINT [DF_BallotPaper_BallotsValidVotes] DEFAULT ((0)) NOT NULL,
    [BallotsReceived]        BIGINT         CONSTRAINT [DF_BallotPaper_BallotsReceived1] DEFAULT ((0)) NOT NULL,
    [BallotsUnusedSpoiled]   BIGINT         CONSTRAINT [DF_BallotPaper_BallotsUnusedSpoiled] DEFAULT ((0)) NOT NULL,
    [BallotsSpoiled]         BIGINT         CONSTRAINT [DF_BallotPaper_BallotsSpoiled] DEFAULT ((0)) NOT NULL,
    [BallotsUnused]          BIGINT         CONSTRAINT [DF_BallotPaper_BallotsUnused] DEFAULT ((0)) NOT NULL,
    [Description]            NVARCHAR (MAX) CONSTRAINT [DF_BallotPaper_Description] DEFAULT ('No Description') NULL,
    [Comments]               NVARCHAR (MAX) CONSTRAINT [DF_BallotPaper_Comments] DEFAULT ('No Comments') NULL,
    [DateOfEntry]            DATE           CONSTRAINT [DF_BallotPaper_DateOfEntry] DEFAULT (sysdatetime()) NOT NULL,
    [VotingPointId]          BIGINT         NULL,
    [EditUserId]             BIGINT         CONSTRAINT [DF_BallotPaper_EditUserId] DEFAULT ((1)) NOT NULL,
    [IsResultsConfirmed]     BIT            CONSTRAINT [DF_BallotPaper_IsResultsConfirmed] DEFAULT ((0)) NOT NULL,
    [ConfirmationUserId]     BIGINT         NULL,
    [ConfirmationDate]       DATETIME       NULL,
    [EditDate]               DATETIME       CONSTRAINT [DF_BallotPaper_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                INT            CONSTRAINT [DF_Constituency_Version_1] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Constituency_1] PRIMARY KEY CLUSTERED ([BallotPaperId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_BallotPaper_ElectionRound] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId]),
    CONSTRAINT [FK_BallotPaper_PollingStation] FOREIGN KEY ([PollingStationId]) REFERENCES [dbo].[PollingStation] ([PollingStationId])
);


GO
CREATE NONCLUSTERED INDEX [IX_BallotPaper_PollingStationId]
    ON [dbo].[BallotPaper]([PollingStationId] ASC, [ElectionRoundId] ASC);

