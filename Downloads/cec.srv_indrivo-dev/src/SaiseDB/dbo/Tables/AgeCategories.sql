CREATE TABLE [dbo].[AgeCategories] (
    [AgeCategoryId] INT            IDENTITY (1, 1) NOT NULL,
    [From]          SMALLINT       NOT NULL,
    [To]            SMALLINT       NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_AgeCategories] PRIMARY KEY CLUSTERED ([AgeCategoryId] ASC),
    CONSTRAINT [UQ_AgeCategories] UNIQUE NONCLUSTERED ([Name] ASC)
);

