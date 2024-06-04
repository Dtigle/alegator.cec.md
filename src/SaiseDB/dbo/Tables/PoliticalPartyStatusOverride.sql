CREATE TABLE [dbo].[PoliticalPartyStatusOverride] (
    [PoliticalPartyStatusOverrideId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [PoliticalPartyStatus]           INT      NOT NULL,
    [ElectionCompetitorId]           BIGINT   NOT NULL,
    [ElectionRoundId]                BIGINT   NOT NULL,
    [AssignedCircumscriptionId]      BIGINT   NULL,
    [EditUserId]                     BIGINT   NOT NULL,
    [EditDate]                       DATETIME DEFAULT (sysdatetime()) NOT NULL,
    [Version]                        INT      DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_PoliticalPartyStatusOverride] PRIMARY KEY CLUSTERED ([PoliticalPartyStatusOverrideId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_PoliticalPartyStatusOverride_AssignedCircumscription_AssignedCircumscriptionId] FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId]),
    CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionCompetitor_StatusOverrides] FOREIGN KEY ([ElectionCompetitorId]) REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId]),
    CONSTRAINT [FK_PoliticalPartyStatusOverride_ElectionRound_ElectionRoundId] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId]),
    CONSTRAINT [FK_PoliticalPartyStatusOverride_SystemUsers_editUserId] FOREIGN KEY ([EditUserId]) REFERENCES [dbo].[SystemUser] ([SystemUserId])
);

