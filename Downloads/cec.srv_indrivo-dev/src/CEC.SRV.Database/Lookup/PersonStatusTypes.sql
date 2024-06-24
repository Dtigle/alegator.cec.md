CREATE TABLE [Lookup].[PersonStatusTypes]
(
	[personStatusTypeId] BIGINT IDENTITY NOT NULL,
	[name] NVARCHAR(50) NOT NULL,
	[description] NVARCHAR(100) NOT NULL,
	[isExcludable] bit NOT NULL default (0),
	created DATETIMEOFFSET null,
    modified DATETIMEOFFSET null,
    deleted DATETIMEOFFSET null,
    createdById NVARCHAR(255) null,
    modifiedById NVARCHAR(255) null,
    deletedById NVARCHAR(255) null,
	constraint [PK_LookupPersonStatusTypes] primary key clustered ([personStatusTypeId] asc),
	CONSTRAINT [UQ_LookupPersonStatusTypes] UNIQUE NONCLUSTERED ([name] ASC, [deleted] ASC),
	constraint [FK_PersonStatusTypes_IdentityUsers_createdById] foreign key (createdById) references Access.IdentityUsers,
	constraint [FK_PersonStatusTypes_IdentityUsers_modifiedById] foreign key (modifiedById) references Access.IdentityUsers,
	constraint [FK_PersonStatusTypes_IdentityUsers_deletedById] foreign key (deletedById) references Access.IdentityUsers 
)
