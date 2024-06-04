CREATE TABLE [Lookup].[PersonAddressTypes]
(
	[personAddressTypeId] BIGINT IDENTITY NOT NULL,
	[name] NVARCHAR(50) NOT NULL,
	[description] NVARCHAR(100) NOT NULL,
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null,
	constraint [PK_LookupPersonAddressTypes] primary key clustered ([personAddressTypeId] asc),
	CONSTRAINT [UQ_LookupPersonAddressTypes] UNIQUE NONCLUSTERED ([name] ASC, [deleted] ASC),
	constraint [FK_PersonAddressTypes_IdentityUsers_createdById] foreign key (createdById) references Access.IdentityUsers,
	constraint [FK_PersonAddressTypes_IdentityUsers_modifiedById] foreign key (modifiedById) references Access.IdentityUsers,
	constraint [FK_PersonAddressTypes_IdentityUsers_deletedById] foreign key (deletedById) references Access.IdentityUsers 
)
