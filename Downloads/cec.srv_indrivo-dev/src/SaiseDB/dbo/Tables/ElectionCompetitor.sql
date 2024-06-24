CREATE TABLE [dbo].[ElectionCompetitor] (
    [ElectionCompetitorId]      BIGINT          IDENTITY (1, 1) NOT NULL,
    [ElectionRoundId]           BIGINT          NOT NULL,
    [AssignedCircumscriptionId] BIGINT          NULL,
    [PoliticalPartyId]          BIGINT          NULL,
    [Code]                      NVARCHAR (100)  NOT NULL,
    [NameRo]                    NVARCHAR (MAX)  NOT NULL,
    [NameRu]                    NVARCHAR (MAX)  NOT NULL,
    [ColorLogo]                 VARBINARY (MAX) NULL,
    [DateOfRegistration]        DATETIME        NOT NULL,
    [Status]                    INT             CONSTRAINT [DF_ElectionCompetitor_Status] DEFAULT ((0)) NOT NULL,
    [IsIndependent]             BIT             CONSTRAINT [DF_ElectionCompetitor_IsIndependent] DEFAULT ((0)) NOT NULL,
    [BallotOrder]               INT             CONSTRAINT [DF_ElectionCompetitor_BallotOrder] DEFAULT ((0)) NOT NULL,
    [EditUserId]                BIGINT          CONSTRAINT [DF_ElectionCompetitor_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                  DATETIME        CONSTRAINT [DF_ElectionCompetitor_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                   INT             CONSTRAINT [DF_ElectionCompetitor_Version] DEFAULT ((1)) NOT NULL,
    [PartyOrder]                INT             NULL,
    [DisplayFromNameRo]         NVARCHAR (MAX)  NULL,
    [DisplayFromNameRu]         NVARCHAR (MAX)  NULL,
    [RegistryNumber]            INT             NULL,
    [BlackWhiteLogo]            VARBINARY (MAX) NULL,
    [PartyType]                 INT             CONSTRAINT [DF_Political_Party_PartyType] DEFAULT ((0)) NOT NULL,
    [BallotPaperNameRo]         NVARCHAR (MAX)  NULL,
    [BallotPaperNameRu]         NVARCHAR (MAX)  NULL,
    [BallotPapperCustomCssRo]   NCHAR (128)     NULL,
    [BallotPapperCustomCssRu]   NCHAR (128)     NULL,
    [Color]                     VARCHAR (6)     NULL,
    CONSTRAINT [PK_ElectionCompetitor] PRIMARY KEY CLUSTERED ([ElectionCompetitorId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ElectionCompetitor_AssignedCircumscription_AssignedCircumscriptionId] FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId]),
    CONSTRAINT [FK_ElectionCompetitor_ElectionRound_ElectionRoundId] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitor_CircumscriptionId]
    ON [dbo].[ElectionCompetitor]([AssignedCircumscriptionId] ASC);


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Varianta specifica a denumirii pentru Buletin de Vot. Se accepta si HTML taguri.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ElectionCompetitor', @level2type = N'COLUMN', @level2name = N'BallotPaperNameRo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Varianta specifica a denumirii pentru Buletin de Vot. Se accepta si HTML taguri.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ElectionCompetitor', @level2type = N'COLUMN', @level2name = N'BallotPaperNameRu';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Clasa speciala de stiluri CSS atribuita denumiii partidului in Buletinul de Vot.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ElectionCompetitor', @level2type = N'COLUMN', @level2name = N'BallotPapperCustomCssRo';


GO
EXECUTE sp_addextendedproperty @name = N'MS_Description', @value = N'Clasa speciala de stiluri CSS atribuita denumiii partidului in Buletinul de Vot.', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'TABLE', @level1name = N'ElectionCompetitor', @level2type = N'COLUMN', @level2name = N'BallotPapperCustomCssRu';

