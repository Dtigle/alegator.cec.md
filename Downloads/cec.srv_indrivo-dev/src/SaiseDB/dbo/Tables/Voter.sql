CREATE TABLE [dbo].[Voter] (
    [VoterId]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [NameRo]             NVARCHAR (100) NOT NULL,
    [LastNameRo]         NVARCHAR (100) NOT NULL,
    [PatronymicRo]       NVARCHAR (100) NULL,
    [LastNameRu]         NVARCHAR (100) NULL,
    [NameRu]             NVARCHAR (100) NULL,
    [PatronymicRu]       NVARCHAR (100) NULL,
    [DateOfBirth]        DATETIME       NOT NULL,
    [PlaceOfBirth]       NVARCHAR (MAX) NULL,
    [PlaceOfResidence]   NVARCHAR (MAX) NULL,
    [Gender]             INT            NOT NULL,
    [DateOfRegistration] DATETIME       NOT NULL,
    [Idnp]               BIGINT         NOT NULL,
    [DocumentNumber]     NVARCHAR (50)  NOT NULL,
    [DateOfIssue]        DATETIME       NULL,
    [DateOfExpiry]       DATETIME       NULL,
    [Status]             BIGINT         CONSTRAINT [DF_Voter_Status] DEFAULT ((0)) NOT NULL,
    [BatchId]            BIGINT         NULL,
    [StreetId]           BIGINT         NULL,
    [RegionId]           BIGINT         CONSTRAINT [DF_Voter_RegionId] DEFAULT ((-1)) NOT NULL,
    [StreetName]         NVARCHAR (MAX) NULL,
    [StreetNumber]       BIGINT         NULL,
    [StreetSubNumber]    NVARCHAR (50)  NULL,
    [BlockNumber]        BIGINT         NULL,
    [BlockSubNumber]     NVARCHAR (50)  NULL,
    [EditUserId]         BIGINT         CONSTRAINT [DF_Voter_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]           DATETIME       CONSTRAINT [DF_Voter_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]            INT            CONSTRAINT [DF_Person_Version] DEFAULT ((1)) NOT NULL,
    [ElectionListNr]     BIGINT         NULL,
    CONSTRAINT [PK_Person] PRIMARY KEY CLUSTERED ([VoterId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_Voter_Region] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([RegionId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Voter]
    ON [dbo].[Voter]([DateOfBirth] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Voter_idnp]
    ON [dbo].[Voter]([Idnp] ASC);

