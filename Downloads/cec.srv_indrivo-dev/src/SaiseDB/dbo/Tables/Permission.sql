CREATE TABLE [dbo].[Permission] (
    [PermissionId] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (250) NOT NULL,
    [EditUserId]   BIGINT         CONSTRAINT [DF_Permission_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]     DATETIME       CONSTRAINT [DF_Permission_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]      INT            CONSTRAINT [DF_Permission_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([PermissionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [IX_Permission] UNIQUE NONCLUSTERED ([Name] ASC) WITH (FILLFACTOR = 80)
);

