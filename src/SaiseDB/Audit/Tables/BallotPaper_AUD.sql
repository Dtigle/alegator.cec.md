﻿CREATE TABLE [Audit].[BallotPaper_AUD] (
    [BallotPaperId]          BIGINT         NOT NULL,
    [REV]                    INT            NOT NULL,
    [REVTYPE]                TINYINT        NOT NULL,
    [EntryLevel]             INT            NOT NULL,
    [Type]                   INT            NOT NULL,
    [Status]                 INT            NOT NULL,
    [RegisteredVoters]       BIGINT         NOT NULL,
    [Supplementary]          BIGINT         NOT NULL,
    [BallotsIssued]          BIGINT         NOT NULL,
    [BallotsCasted]          BIGINT         NOT NULL,
    [DifferenceIssuedCasted] BIGINT         NOT NULL,
    [BallotsValidVotes]      BIGINT         NOT NULL,
    [BallotsReceived]        BIGINT         NOT NULL,
    [BallotsUnusedSpoiled]   BIGINT         NOT NULL,
    [BallotsSpoiled]         BIGINT         NOT NULL,
    [BallotsUnused]          BIGINT         NOT NULL,
    [Description]            NVARCHAR (MAX) NULL,
    [Comments]               NVARCHAR (MAX) NULL,
    [DateOfEntry]            DATE           NOT NULL,
    [VotingPointId]          BIGINT         NULL,
    [PollingStationId]       BIGINT         NULL,
    [ElectionRoundId]        BIGINT         NOT NULL,
    [EditUserId]             BIGINT         NOT NULL,
    [EditDate]               DATETIME       NOT NULL,
    [IsResultsConfirmed]     BIT            NOT NULL,
    [ConfirmationUserId]     BIGINT         NULL,
    [ConfirmationDate]       DATETIME       NULL,
    [Version]                INT            NULL,
    CONSTRAINT [PK_Constituency_1] PRIMARY KEY CLUSTERED ([BallotPaperId] ASC, [REV] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_BallotPaper_AUD_REVINFO] FOREIGN KEY ([REV]) REFERENCES [Audit].[REVINFO] ([REV])
);
