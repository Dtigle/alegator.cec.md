CREATE TABLE [schematmp].[AgeCategories] (
    [AgeCategoryId] INT            NOT NULL,
    [From]          SMALLINT       NOT NULL,
    [To]            SMALLINT       NULL,
    [Name]          NVARCHAR (255) NOT NULL,
    CONSTRAINT [PK_AgeCategories] PRIMARY KEY CLUSTERED ([AgeCategoryId] ASC) WITH (FILLFACTOR = 80)
);

