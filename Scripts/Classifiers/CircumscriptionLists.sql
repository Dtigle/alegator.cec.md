SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [Lookup].[CircumscriptionLists](
	[circumscriptionListId] [bigint] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](255) NOT NULL,
	[created] [datetimeoffset](7) NOT NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NOT NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL,
 CONSTRAINT [PK_CircumscriptionLists] PRIMARY KEY CLUSTERED 
(
	[circumscriptionListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Lookup].[CircumscriptionLists]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionLists_Users_createdById] FOREIGN KEY([createdById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [Lookup].[CircumscriptionLists] CHECK CONSTRAINT [FK_CircumscriptionLists_Users_createdById]
GO

ALTER TABLE [Lookup].[CircumscriptionLists]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionLists_Users_deletedById] FOREIGN KEY([deletedById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [Lookup].[CircumscriptionLists] CHECK CONSTRAINT [FK_CircumscriptionLists_Users_deletedById]
GO

ALTER TABLE [Lookup].[CircumscriptionLists]  WITH CHECK ADD  CONSTRAINT [FK_CircumscriptionLists_Users_modifiedById] FOREIGN KEY([modifiedById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [Lookup].[CircumscriptionLists] CHECK CONSTRAINT [FK_CircumscriptionLists_Users_modifiedById]
GO


