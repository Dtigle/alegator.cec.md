CREATE TABLE [Lookup].[ManagerTypes]
(
	[managerTypeId] BIGINT IDENTITY NOT NULL,
       name NVARCHAR(255) null,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   [description] NVARCHAR(255) NULL, 
		
	constraint [PK_LookupManagerTypes] primary key clustered (managerTypeId asc),
	constraint [UQ_LookupManagerTypes] unique nonclustered ([name] asc, [deleted] asc),
	constraint FK_ManagerTypes_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_ManagerTypes_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_ManagerTypes_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)
