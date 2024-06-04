CREATE TABLE [dbo].[ElectionRound] (
    [ElectionRoundId]   BIGINT          IDENTITY (1, 1) NOT NULL,
    [ElectionId]        BIGINT          NOT NULL,
    [Number]            TINYINT         NOT NULL,
    [NameRo]            NVARCHAR (255)  NOT NULL,
    [NameRu]            NVARCHAR (255)  NULL,
    [Description]       NVARCHAR (1000) NULL,
    [ElectionDate]      DATE            NOT NULL,
    [CampaignStartDate] DATE            NULL,
    [CampaignEndDate]   DATE            NULL,
    [Status]            TINYINT         NOT NULL,
    [EditUserId]        BIGINT          CONSTRAINT [DF_ElectionRound_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]          DATETIME        CONSTRAINT [DF_ElectionRound_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]           INT             CONSTRAINT [DF_ElectionRound_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ElectionRounds] PRIMARY KEY CLUSTERED ([ElectionRoundId] ASC),
    CONSTRAINT [FK_ElectionRound_Election_ElectionId] FOREIGN KEY ([ElectionId]) REFERENCES [dbo].[Election] ([ElectionId]),
    CONSTRAINT [UQ_ElectionRounds] UNIQUE NONCLUSTERED ([ElectionId] ASC, [Number] ASC)
);

