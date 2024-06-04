CREATE TABLE [schematmp].[Role] (
    [RoleId]     BIGINT         NOT NULL,
    [Name]       NVARCHAR (MAX) NOT NULL,
    [Level]      INT            NOT NULL,
    [EditUserId] BIGINT         NOT NULL,
    [EditDate]   DATETIME       NOT NULL,
    [Version]    INT            NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleId] ASC) WITH (FILLFACTOR = 80)
);

