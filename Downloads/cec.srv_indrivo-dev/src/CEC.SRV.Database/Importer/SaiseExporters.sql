CREATE TABLE [Importer].[SaiseExporters]
(
	[saiseExporterId] BIGINT IDENTITY NOT NULL,
	[status] NVARCHAR(255) NOT NULL,
    electionId BIGINT NOT NULL,
	exportAllVoters BIT NOT NULL DEFAULT(0),
	errorMessage NVARCHAR(MAX) NULL,
	startDate DATETIMEOFFSET null,
	endDate DATETIMEOFFSET null,
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null
			
	CONSTRAINT [PK_SaiseExporters] PRIMARY KEY CLUSTERED (saiseExporterId ASC),
	CONSTRAINT [FK_SaiseExporters_Elections_electionId] FOREIGN KEY (electionId) REFERENCES SRV.Elections,
	CONSTRAINT [FK_SaiseExporters_IdentityUsers_createdById] FOREIGN KEY (createdById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_SaiseExporters_IdentityUsers_modifiedById] FOREIGN KEY (modifiedById) REFERENCES Access.IdentityUsers,
	CONSTRAINT [FK_SaiseExporters_IdentityUsers_deletedById] FOREIGN KEY (deletedById) REFERENCES Access.IdentityUsers
	
)
