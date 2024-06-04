CREATE TABLE [Access].[IdentityRoles]
(
       [identityRoleId] NVARCHAR(255) not null,
       [name] NVARCHAR(255) not null,
	   CONSTRAINT [PK_IdentityRoles] PRIMARY KEY ([identityRoleId])
)
