CREATE TABLE [dbo].[RegionType] (
    [RegionTypeId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (255) NOT NULL,
    [Description]  NVARCHAR (255) NULL,
    [Rank]         TINYINT        NOT NULL,
    [EditUserId]   BIGINT         CONSTRAINT [DF_RegionType_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]     DATETIME       CONSTRAINT [DF_RegionType_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]      INT            CONSTRAINT [DF_RegionType_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_RegionType] PRIMARY KEY CLUSTERED ([RegionTypeId] ASC)
);

