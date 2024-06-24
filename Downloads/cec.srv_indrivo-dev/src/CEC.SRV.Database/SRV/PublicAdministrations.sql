CREATE TABLE [SRV].[PublicAdministrations]
(		
	    [publicAdministrationId] BIGINT IDENTITY NOT NULL, 
		[regionId] BIGINT NOT NULL,
		[name] NVARCHAR(255) null,
		[surname] NVARCHAR(255) NULL, 
		[managerTypeId] BIGINT NOT NULL,
		[created] DATETIMEOFFSET null,
		[modified] DATETIMEOFFSET null,
		[deleted] DATETIMEOFFSET null,
		[createdById] NVARCHAR(255) null,
		[modifiedById] NVARCHAR(255) null,
		[deletedById] NVARCHAR(255) null, 

    CONSTRAINT [PK_PublicAdministrations] PRIMARY KEY (publicAdministrationId),
	constraint [FK_PublicAdministrations_Regions_regionId] foreign key (regionId) references Lookup.Regions,
	constraint [FK_PublicAdministrations_ManagerTypes_managerTypeId] foreign key (managerTypeId) references Lookup.ManagerTypes,
	constraint FK_PublicAdministrations_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_PublicAdministrations_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_PublicAdministrations_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)
