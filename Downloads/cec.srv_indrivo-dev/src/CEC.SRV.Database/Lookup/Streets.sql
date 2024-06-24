CREATE TABLE [Lookup].[Streets]
(
		[streetId] BIGINT IDENTITY NOT NULL,
		name NVARCHAR(255) null,
		[description] NVARCHAR(255) NULL, 
		[regionId] BIGINT NOT NULL,
		[streetTypeId] BIGINT NOT NULL,
		[ropId] BIGINT NULL,
		[saiseId] BIGINT NULL,
		created DATETIMEOFFSET null,
		modified DATETIMEOFFSET null,
		deleted DATETIMEOFFSET null,
		createdById NVARCHAR(255) null,
		modifiedById NVARCHAR(255) null,
		deletedById NVARCHAR(255) null,
		
	constraint [PK_LookupStreets] primary key clustered (streetId asc),
	constraint [UQ_LookupStreets] unique nonclustered ([regionId] asc, [name] asc, [streetTypeId] asc, [deleted] asc),
	constraint [FK_Streets_Regions_regionId] foreign key (regionId) references Lookup.Regions,
	constraint [FK_Streets_StreetTypes_streetTypeId] foreign key (streetTypeId) references Lookup.StreetTypes,
	
	constraint FK_Streets_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_Streets_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_Streets_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)

GO

CREATE INDEX [IX_Streets_RegionId] ON [Lookup].[Streets] ([regionId])
