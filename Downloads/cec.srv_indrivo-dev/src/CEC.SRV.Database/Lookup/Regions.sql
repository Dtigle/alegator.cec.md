CREATE TABLE [Lookup].[Regions]
(
		[regionId] BIGINT IDENTITY NOT NULL,
		name NVARCHAR(255) null,
		[description] NVARCHAR(255) NULL, 
		[parentId] BIGINT NULL,
		[regionTypeId] BIGINT NOT NULL,
		[saiseId] BIGINT NULL,
		[registruId] BIGINT NULL,
		[statisticCode] BIGINT NULL,
		[statisticIdentifier] BIGINT NULL,
 		[hasStreets] BIT NOT NULL default (0),
		geolatitude DOUBLE PRECISION NULL,
		geolongitude DOUBLE PRECISION NULL,
		created DATETIMEOFFSET null,
		modified DATETIMEOFFSET null,
		deleted DATETIMEOFFSET null,
		createdById NVARCHAR(255) null,
		modifiedById NVARCHAR(255) null,
		deletedById NVARCHAR(255) null,		
	    [publicAdministrationId] BIGINT NULL, 

    [circumscription] INT NULL, 
    constraint [PK_LookupRegions] primary key clustered (regionId asc),
	constraint [UQ_LookupRegions] unique nonclustered ([parentId]asc, [name] asc, [regionTypeId] asc, [deleted] asc),
	constraint [FK_Regions_Parent] foreign key (parentId) references Lookup.Regions,
	constraint [FK_Regions_RegionTypes_regionTypeId] foreign key (regionTypeId) references Lookup.RegionTypes,
	constraint [FK_Regions_PublicAdministrations_publicAdministrationId] foreign key (publicAdministrationId) references SRV.PublicAdministrations,
	constraint FK_Regions_Regions_Children foreign key (parentId) references Lookup.Regions,
	constraint FK_Regions_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_Regions_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_Regions_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)

GO

CREATE NONCLUSTERED INDEX [IX_Regions_registruId]
ON [Lookup].[Regions] ([registruId])
INCLUDE ([regionId])

GO

CREATE INDEX [IX_Regions_parentId] ON [Lookup].[Regions] ([parentId])
