CREATE TABLE [dbo].[ElectionCompetitorMember] (
    [ElectionCompetitorMemberId] BIGINT          IDENTITY (1, 1) NOT NULL,
    [AssignedCircumscriptionId]  BIGINT          NULL,
    [ElectionRoundId]            BIGINT          NULL,
    [LastNameRo]                 NVARCHAR (100)  NOT NULL,
    [LastNameRu]                 NVARCHAR (100)  NULL,
    [NameRo]                     NVARCHAR (100)  NOT NULL,
    [NameRu]                     NVARCHAR (100)  NULL,
    [PatronymicRo]               NVARCHAR (100)  NULL,
    [PatronymicRu]               NVARCHAR (100)  NULL,
    [DateOfBirth]                DATETIME        NOT NULL,
    [PlaceOfBirth]               NVARCHAR (100)  NOT NULL,
    [Gender]                     INT             NOT NULL,
    [Occupation]                 NVARCHAR (100)  NULL,
    [OccupationRu]               NVARCHAR (100)  NULL,
    [Designation]                NVARCHAR (100)  NULL,
    [DesignationRu]              NVARCHAR (100)  NULL,
    [Workplace]                  NVARCHAR (200)  NULL,
    [WorkplaceRu]                NVARCHAR (200)  NULL,
    [Idnp]                       BIGINT          NOT NULL,
    [ElectionCompetitorId]       BIGINT          NULL,
    [DateOfRegistration]         DATE            NULL,
    [ColorLogo]                  VARBINARY (MAX) NULL,
    [BlackWhiteLogo]             VARBINARY (MAX) NULL,
    [Picture]                    VARBINARY (MAX) NULL,
    [Status]                     INT             CONSTRAINT [DF_ElectionCompetitorMember_Status] DEFAULT ((0)) NOT NULL,
    [CompetitorMemberOrder]      INT             CONSTRAINT [DF_ElectionCompetitorMember_CompetitorMemberOrder] DEFAULT ((1)) NOT NULL,
    [EditUserId]                 BIGINT          CONSTRAINT [DF_ElectionCompetitorMember_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]                   DATETIME        CONSTRAINT [DF_ElectionCompetitorMember_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]                    INT             CONSTRAINT [DF_ElectionCompetitorMember_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_ElectionCompetitorMember] PRIMARY KEY NONCLUSTERED ([ElectionCompetitorMemberId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_ElectionCompetitorMember_AssignedCircumscription] FOREIGN KEY ([AssignedCircumscriptionId]) REFERENCES [dbo].[AssignedCircumscription] ([AssignedCircumscriptionId]),
    CONSTRAINT [FK_ElectionCompetitorMember_ElectionCompetitor] FOREIGN KEY ([ElectionCompetitorId]) REFERENCES [dbo].[ElectionCompetitor] ([ElectionCompetitorId]),
    CONSTRAINT [FK_ElectionCompetitorMember_ElectionRound] FOREIGN KEY ([ElectionRoundId]) REFERENCES [dbo].[ElectionRound] ([ElectionRoundId])
);


GO
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitorMember_CircumscriptionId]
    ON [dbo].[ElectionCompetitorMember]([AssignedCircumscriptionId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ElectionCompetitorMember_CompetitorId]
    ON [dbo].[ElectionCompetitorMember]([ElectionCompetitorId] ASC);

