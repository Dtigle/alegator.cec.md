CREATE TABLE [Importer].[SaiseExporterStages]
(
	 saiseExporterStageId BIGINT IDENTITY NOT NULL,
     [description] NVARCHAR(MAX) NULL,
     errorMessage NVARCHAR(1028) NULL,
     [status] NVARCHAR(255) NULL,
	 stageType NVARCHAR(255) NULL,
     saiseExporterId BIGINT NOT NULL,
	 [statistics] nvarchar(MAX) NULL,
	 startDate DATETIMEOFFSET null,
	 endDate DATETIMEOFFSET null,
	 created DATETIMEOFFSET NULL,
     modified DATETIMEOFFSET NULL,
     deleted DATETIMEOFFSET NULL,
     createdById NVARCHAR(255) NULL,
     modifiedById NVARCHAR(255) NULL,
     deletedById NVARCHAR(255) NULL,

     CONSTRAINT [PK_SaiseExporterStages] PRIMARY KEY CLUSTERED (saiseExporterStageId ASC),
	 CONSTRAINT [FK_SaiseExporterStages_SaiseExporters_Stages] FOREIGN KEY (saiseExporterId) REFERENCES Importer.SaiseExporters,

	 CONSTRAINT [FK_SaiseExporterStages_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	 CONSTRAINT [FK_SaiseExporterStages_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	 CONSTRAINT [FK_SaiseExporterStages_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers

	
)
