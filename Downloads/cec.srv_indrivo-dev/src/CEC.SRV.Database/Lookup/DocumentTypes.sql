CREATE TABLE [Lookup].[DocumentTypes]
(
		documentTypeId BIGINT IDENTITY NOT NULL,
       isPrimary BIT null,
       name NVARCHAR(255) null,
       description NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
		
	constraint [PK_DocumentTypes] primary key clustered (documentTypeId asc),
	constraint [UQ_DocumentTypes] unique nonclustered ([name] asc, [deleted] asc),
	
	
	constraint FK_DocumentTypes_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_DocumentTypes_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_DocumentTypes_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)

GO
CREATE INDEX IX_DocumentTypes_isPrimary ON Lookup.DocumentTypes(isPrimary);
go
