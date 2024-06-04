CREATE TABLE [Access].[IdentityUserLogins] 
(
       [Provider] NVARCHAR(255) NOT NULL,
       [Key] NVARCHAR(255) NOT NULL,
       [userId] NVARCHAR(255) NOT NULL,
       CONSTRAINT [PK_IdentityUserLogins] PRIMARY KEY ([Provider], [Key], [userId]),
	   CONSTRAINT [FK_IdentityUserLogins_IdentityUsers_Logins] FOREIGN KEY ([userId]) REFERENCES [Access].[IdentityUsers]
)