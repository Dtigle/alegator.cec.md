CREATE TABLE [Lookup].[RegionTypes]
(
	regionTypeId BIGINT IDENTITY NOT NULL,
       name NVARCHAR(255) not null,
	   [rank] tinyint not null ,
       created DATETIMEOFFSET null,
       modified DATETIMEOFFSET null,
       deleted DATETIMEOFFSET null,
       createdById NVARCHAR(255) null,
       modifiedById NVARCHAR(255) null,
       deletedById NVARCHAR(255) null,
	   [description] NVARCHAR(255) NULL, 
    constraint [PK_LookupRegionTypes] primary key clustered ([regionTypeId] asc),
	   CONSTRAINT [UQ_LookupRegionTypes] UNIQUE NONCLUSTERED ([name] ASC, [deleted] ASC),
	   constraint [FK_RegionTypes_IdentityUsers_createdById] foreign key (createdById) references Access.IdentityUsers,
	   constraint [FK_RegionTypes_IdentityUsers_modifiedById] foreign key (modifiedById) references Access.IdentityUsers,
	   constraint [FK_RegionTypes_IdentityUsers_deletedById] foreign key (deletedById) references Access.IdentityUsers 
)
