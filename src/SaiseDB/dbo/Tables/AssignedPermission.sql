CREATE TABLE [dbo].[AssignedPermission] (
    [AssignedPermissionId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [RoleId]               BIGINT   NOT NULL,
    [PermissionId]         BIGINT   NOT NULL,
    [EditUserId]           BIGINT   CONSTRAINT [DF_AssignedPermission_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]             DATETIME CONSTRAINT [DF_AssignedPermission_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]              INT      CONSTRAINT [DF_AssignedPermissions_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AssignedPermissions] PRIMARY KEY CLUSTERED ([AssignedPermissionId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_AssignedPermissions_Permission] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permission] ([PermissionId]),
    CONSTRAINT [FK_AssignedPermissions_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId])
);

