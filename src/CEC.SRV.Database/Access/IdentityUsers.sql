CREATE TABLE [Access].[IdentityUsers] 
(
       [identityUserId] NVARCHAR(255) NOT NULL,
       [userName] NVARCHAR(255) NOT NULL,
       [passwordHash] NVARCHAR(255) NULL,
       [securityStamp] NVARCHAR(255) NULL,
       CONSTRAINT [PK_IdentityUsers] PRIMARY KEY ([identityUserId])
)
