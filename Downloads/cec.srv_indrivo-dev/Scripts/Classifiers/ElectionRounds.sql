SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [SRV].[ElectionRounds](
	[electionRoundId] [bigint] IDENTITY(1,1) NOT NULL,
	[electionId] [bigint] NOT NULL,
	[number] [tinyint] NOT NULL,
	[nameRo] [nvarchar](255) NOT NULL,
	[nameRu] [nvarchar](255) NULL,
	[description] [nvarchar](1000) NULL,
	[electionDate] [date] NOT NULL,
	[campaignStartDate] [date] NULL,
	[campaignEndDate] [date] NULL,
	[reportsPath] [nvarchar](255) NULL,
	[status] [tinyint] NOT NULL,
	[created] [datetimeoffset](7) NULL,
	[modified] [datetimeoffset](7) NULL,
	[deleted] [datetimeoffset](7) NULL,
	[createdById] [nvarchar](255) NOT NULL,
	[modifiedById] [nvarchar](255) NULL,
	[deletedById] [nvarchar](255) NULL,
 CONSTRAINT [PK_ElectionRounds] PRIMARY KEY CLUSTERED 
(
	[electionRoundId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_ElectionRounds] UNIQUE NONCLUSTERED 
(
	[electionId] ASC,
	[number] ASC,
	[deleted] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [SRV].[ElectionRounds]  WITH CHECK ADD  CONSTRAINT [FK_ElectionRounds_Elections_electionId] FOREIGN KEY([electionId])
REFERENCES [SRV].[Elections] ([electionId])
GO

ALTER TABLE [SRV].[ElectionRounds] CHECK CONSTRAINT [FK_ElectionRounds_Elections_electionId]
GO

ALTER TABLE [SRV].[ElectionRounds]  WITH CHECK ADD  CONSTRAINT [FK_ElectionRounds_Users_createdById] FOREIGN KEY([createdById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [SRV].[ElectionRounds] CHECK CONSTRAINT [FK_ElectionRounds_Users_createdById]
GO

ALTER TABLE [SRV].[ElectionRounds]  WITH CHECK ADD  CONSTRAINT [FK_ElectionRounds_Users_deletedById] FOREIGN KEY([deletedById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [SRV].[ElectionRounds] CHECK CONSTRAINT [FK_ElectionRounds_Users_deletedById]
GO

ALTER TABLE [SRV].[ElectionRounds]  WITH CHECK ADD  CONSTRAINT [FK_ElectionRounds_Users_modifiedById] FOREIGN KEY([modifiedById])
REFERENCES [Access].[IdentityUsers] ([identityUserId])
GO

ALTER TABLE [SRV].[ElectionRounds] CHECK CONSTRAINT [FK_ElectionRounds_Users_modifiedById]
GO


