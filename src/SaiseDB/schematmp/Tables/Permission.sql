CREATE TABLE [schematmp].[Permission] (
    [PermissionId] BIGINT         NOT NULL,
    [Name]         NVARCHAR (250) NOT NULL,
    [EditUserId]   BIGINT         NOT NULL,
    [EditDate]     DATETIME       NOT NULL,
    [Version]      INT            NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([PermissionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Permission] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

