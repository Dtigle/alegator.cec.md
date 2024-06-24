CREATE TABLE [dbo].[AssignedRole] (
    [AssignedRoleId] BIGINT   IDENTITY (1, 1) NOT NULL,
    [RoleId]         BIGINT   NOT NULL,
    [SystemUserId]   BIGINT   NOT NULL,
    [EditUserId]     BIGINT   CONSTRAINT [DF_AssignedRole_EditUserId] DEFAULT ((1)) NOT NULL,
    [EditDate]       DATETIME CONSTRAINT [DF_AssignedRole_EditDate] DEFAULT (sysdatetime()) NOT NULL,
    [Version]        INT      CONSTRAINT [DF_AssignedRoles_Version] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AssignedRoles] PRIMARY KEY CLUSTERED ([AssignedRoleId] ASC) WITH (FILLFACTOR = 80),
    CONSTRAINT [FK_AssignedRoles_Role] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([RoleId]),
    CONSTRAINT [FK_AssignedRoles_SystemUser1] FOREIGN KEY ([SystemUserId]) REFERENCES [dbo].[SystemUser] ([SystemUserId]) ON UPDATE CASCADE
);

