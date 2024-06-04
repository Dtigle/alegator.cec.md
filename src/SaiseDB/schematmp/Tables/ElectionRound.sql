CREATE TABLE [schematmp].[ElectionRound] (
    [ElectionRoundId]   BIGINT          NOT NULL,
    [ElectionId]        BIGINT          NOT NULL,
    [Number]            TINYINT         NOT NULL,
    [NameRo]            NVARCHAR (255)  NOT NULL,
    [NameRu]            NVARCHAR (255)  NULL,
    [Description]       NVARCHAR (1000) NULL,
    [ElectionDate]      DATE            NOT NULL,
    [CampaignStartDate] DATE            NULL,
    [CampaignEndDate]   DATE            NULL,
    [Status]            TINYINT         NOT NULL,
    [EditUserId]        BIGINT          NOT NULL,
    [EditDate]          DATETIME        NOT NULL,
    [Version]           INT             NOT NULL,
    CONSTRAINT [PK_ElectionRounds] PRIMARY KEY CLUSTERED ([ElectionRoundId] ASC) WITH (FILLFACTOR = 80)
);

