CREATE TABLE [dbo].[Region] (
    [RegionId]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100) NOT NULL,
    [NameRu]              NVARCHAR (100) NULL,
    [Description]         NVARCHAR (500) NULL,
    [ParentId]            BIGINT         NULL,
    [RegionTypeId]        BIGINT         NOT NULL,
    [RegistryId]          BIGINT         NULL,
    [StatisticCode]       BIGINT         NULL,
    [StatisticIdentifier] BIGINT         NULL,
    [HasStreets]          BIT            NOT NULL,
    [GeoLatitude]         FLOAT (53)     NULL,
    [GeoLongitude]        FLOAT (53)     NULL,
    [EditUserId]          BIGINT         CONSTRAINT [DF_Region_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]            DATETIME       CONSTRAINT [DF_Region_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]             INT            CONSTRAINT [DF_Region_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([RegionId] ASC),
    CONSTRAINT [FK_Region_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[Region] ([RegionId]),
    CONSTRAINT [FK_Region_RegionType_RegionTypeId] FOREIGN KEY ([RegionTypeId]) REFERENCES [dbo].[RegionType] ([RegionTypeId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Region_RegionTypeId]
    ON [dbo].[Region]([RegionTypeId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RegionParent]
    ON [dbo].[Region]([ParentId] ASC);

