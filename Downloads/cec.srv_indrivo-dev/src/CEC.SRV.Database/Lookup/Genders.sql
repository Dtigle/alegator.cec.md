CREATE TABLE [Lookup].[Genders]
(
	[genderId] BIGINT IDENTITY NOT NULL,
       name NVARCHAR(255) null,
	   [description] NVARCHAR(255) NULL, 
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
		
	constraint [PK_LookupGenders] primary key clustered (genderId asc),
	constraint [UQ_LookupGenders] unique nonclustered ([name] asc, [deleted] asc),
	constraint FK_Genders_IdentityUsers_createdById foreign key (createdById) references Access.IdentityUsers,
	constraint FK_Genders_IdentityUsers_modifiedById foreign key (modifiedById) references Access.IdentityUsers,
	constraint FK_Genders_IdentityUsers_deletedById foreign key (deletedById) references Access.IdentityUsers
)
