CREATE TABLE [schematmp].[AssignedPermission] (
    [AssignedPermissionId] BIGINT   NOT NULL,
    [RoleId]               BIGINT   NOT NULL,
    [PermissionId]         BIGINT   NOT NULL,
    [EditUserId]           BIGINT   NOT NULL,
    [EditDate]             DATETIME NOT NULL,
    [Version]              INT      NOT NULL,
    CONSTRAINT [PK_AssignedPermissions] PRIMARY KEY CLUSTERED ([AssignedPermissionId] ASC) WITH (FILLFACTOR = 80)
);

