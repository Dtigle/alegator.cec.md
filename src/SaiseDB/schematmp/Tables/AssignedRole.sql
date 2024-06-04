CREATE TABLE [schematmp].[AssignedRole] (
    [AssignedRoleId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [RoleId]         BIGINT   NOT NULL,
    [SystemUserId]   BIGINT   NOT NULL,
    [EditUserId]     BIGINT   NOT NULL,
    [EditDate]       DATETIME NOT NULL,
    [Version]        INT      NOT NULL,
    CONSTRAINT [PK_AssignedRoles] PRIMARY KEY CLUSTERED ([AssignedRoleId] ASC) WITH (FILLFACTOR = 80)
);

