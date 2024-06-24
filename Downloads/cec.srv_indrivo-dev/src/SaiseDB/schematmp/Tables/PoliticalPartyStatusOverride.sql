CREATE TABLE [schematmp].[PoliticalPartyStatusOverride] (
    [PoliticalPartyStatusOverrideId] BIGINT   NOT NULL,
    [PoliticalPartyStatus]           INT      NOT NULL,
    [ElectionCompetitorId]           BIGINT   NOT NULL,
    [ElectionRoundId]                BIGINT   NOT NULL,
    [AssignedCircumscriptionId]      BIGINT   NULL,
    [EditUserId]                     BIGINT   NOT NULL,
    [EditDate]                       DATETIME NOT NULL,
    [Version]                        INT      NOT NULL,
    CONSTRAINT [PK_PoliticalPartyStatusOverride] PRIMARY KEY CLUSTERED ([PoliticalPartyStatusOverrideId] ASC) WITH (FILLFACTOR = 80)
);

