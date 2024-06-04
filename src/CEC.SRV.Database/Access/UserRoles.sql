CREATE TABLE [Access].[UserRoles] 
(
       [roleId] NVARCHAR(255) NOT NULL,
       [userId] NVARCHAR(255) NOT NULL,
	   CONSTRAINT [FK_UserRoles_IdentityRoles_roleId] FOREIGN KEY ([roleId]) REFERENCES [Access].[IdentityRoles] ([identityRoleId]) ON DELETE CASCADE ON UPDATE CASCADE,
	   CONSTRAINT [FK_UserRoles_IdentityUsers_userId] FOREIGN KEY ([userId]) REFERENCES [Access].[IdentityUsers] ([identityUserId]) ON DELETE CASCADE ON UPDATE CASCADE
)