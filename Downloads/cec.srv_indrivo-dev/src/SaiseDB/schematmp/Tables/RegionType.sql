CREATE TABLE [schematmp].[RegionType] (
    [RegionTypeId] BIGINT         NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Description]  NVARCHAR (255) NULL,
    [Rank]         TINYINT        NOT NULL,
    [EditUserId]   BIGINT         NOT NULL,
    [EditDate]     DATETIME       NOT NULL,
    [Version]      INT            NOT NULL,
    CONSTRAINT [PK_RegionTypes] PRIMARY KEY CLUSTERED ([RegionTypeId] ASC) WITH (FILLFACTOR = 80)
);

