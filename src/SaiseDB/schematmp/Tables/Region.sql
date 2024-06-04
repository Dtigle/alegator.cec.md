CREATE TABLE [schematmp].[Region] (
    [RegionId]            BIGINT         NOT NULL,
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
    [EditUserId]          BIGINT         NOT NULL,
    [EditDate]            DATETIME       NOT NULL,
    [Version]             INT            NOT NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([RegionId] ASC) WITH (FILLFACTOR = 80)
);

